// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KodeAid.Serialization;
using KodeAid.Serialization.Binary;
using Microsoft.Extensions.Logging;

namespace KodeAid.Caching.SqlDb
{
    public class SqlCacheClient : CacheClientBase
    {
        private const string _defaultDefaultTableName = "Cache";
        private const string _defaultSchemaName = "dbo";
        private const string _sqlDateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        private readonly string _connectionString;
        private readonly string _defaultTableName;
        private readonly string _schemaName;
        private readonly ISerializer _serializer;

        public SqlCacheClient(string connectionString, ISerializer<string> serializer, ILogger<SqlCacheClient> logger, string defaultTableName = _defaultDefaultTableName, string schemaName = _defaultSchemaName, bool throwOnError = false)
            : this(connectionString, SerializerHelper.WrapSerializer(serializer), defaultTableName, schemaName, logger, throwOnError)
        {
        }

        public SqlCacheClient(string connectionString, ISerializer<byte[]> serializer, ILogger<SqlCacheClient> logger, string defaultTableName = _defaultDefaultTableName, string schemaName = _defaultSchemaName, bool throwOnError = false)
            : this(connectionString, SerializerHelper.WrapSerializer(serializer), defaultTableName, schemaName, logger, throwOnError)
        {
        }

        public SqlCacheClient(string connectionString, ILogger<SqlCacheClient> logger, string defaultTableName = _defaultDefaultTableName, string schemaName = _defaultSchemaName, bool throwOnError = false)
            : this(connectionString, new DotNetBinarySerializer(), defaultTableName, schemaName, logger, throwOnError)
        {
        }

        private SqlCacheClient(string connectionString, ISerializer serializer, string defaultTableName, string schemaName, ILogger<SqlCacheClient> logger, bool throwOnError)
            : base(throwOnError, logger)
        {
            ArgCheck.NotNullOrEmpty(nameof(connectionString), connectionString);
            ArgCheck.NotNull(nameof(serializer), serializer);
            ArgCheck.NotNullOrEmpty(nameof(schemaName), schemaName);
            _connectionString = connectionString;
            _defaultTableName = defaultTableName;
            _schemaName = schemaName;
            _serializer = serializer;
        }

        public async Task DeleteExpired(string tableName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = $@"IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{_schemaName}' AND TABLE_NAME = '{tableName}') DELETE FROM [{_schemaName}].[{tableName}] WHERE ([Expiration] IS NOT NULL AND [Expiration] >= '{DateTimeOffset.UtcNow.ToString(_sqlDateTimeFormat)}');";
                }
            }
        }

        protected override async Task<IEnumerable<CacheItem<T>>> GetItemsAsync<T>(IEnumerable<string> keys, string tableName)
        {
            tableName = tableName ?? _defaultTableName;
            ArgCheck.NotNull(nameof(tableName), tableName);

            var items = new List<CacheItem<T>>();
            var utcNow = DateTime.UtcNow;

            foreach (var keyParition in keys.Partition(1000))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = GetCheckTableScript(tableName) +
                          $@"SELECT [Key], [Value], [Updated], [Expiry] FROM [{_schemaName}].[{tableName}] WHERE ([Key] IN ({string.Join(",", Enumerable.Range(0, keyParition.Count()).Select(i => "@key" + i))}) AND ([Expiry] IS NULL OR [Expiry] < @now));";
                        var p = -1;
                        foreach (var key in keyParition)
                        {
                            command.AddParameter("@key" + (++p), key, DbType.String);
                        }

                        command.AddParameter("@now", DateTime.UtcNow, DbType.DateTime2);
                        using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                var item = new CacheItem<T>()
                                {
                                    Key = reader.GetString(0),
                                    Value = reader.IsDBNull(1) ? default : DeserializeValue<T>(reader.GetValue(1)),
                                    LastUpdated = reader.GetDateTime(2),
                                    Expiration = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                                };
                                items.Add(item);
                            }
                            reader.Close();
                        }
                    }
                    connection.Close();
                }
            }
            return items;
        }

        protected override async Task SetItemsAsync<T>(IEnumerable<CacheItem<T>> items, string tableName)
        {
            tableName = tableName ?? _defaultTableName;
            ArgCheck.NotNull(nameof(tableName), tableName);

            // max < 2100 parameters allowed, each item has 4 parameters so (524 x 4 = 2096)
            foreach (var itemParition in items.Partition(524))
            {
                var sql = new StringBuilder();
                var p = 0;
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = connection.CreateCommand())
                    {
                        foreach (var item in itemParition)
                        {
                            ++p;
                            sql.Append($@"IF NOT EXISTS (SELECT [Key] FROM [{_schemaName}].[{tableName}] WHERE ([Key] = @pk{p}))
  INSERT INTO [{_schemaName}].[{tableName}] ([Key], [Value], [Updated], [Expiry]) VALUES (@pk{p}, @pv{p}, @pu{p}, @px{p});
ELSE
  UPDATE [{_schemaName}].[{tableName}] SET [Value] = @pv{p}, [Updated] = @pu{p}, [Expiry] = @px{p} WHERE [Key] = @pk{p};
");

                            command.AddParameter("@pk" + p, item.Key, DbType.String);
                            command.AddParameter("@pv" + p, item.Value == null ? DBNull.Value : SerializeValue(item.Value), _serializer.SerializedType == typeof(string) ? DbType.String : DbType.Binary);
                            command.AddParameter("@pu" + p, item.LastUpdated, DbType.DateTime2);
                            command.AddParameter("@px" + p, item.Expiration == null ? DBNull.Value : (object)item.Expiration.Value, DbType.DateTime2);
                        }

                        await connection.OpenAsync().ConfigureAwait(false);
                        command.CommandType = CommandType.Text;
                        command.CommandText = GetCheckTableScript(tableName) + sql.ToString();
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                    connection.Close();
                }
            }
        }

        protected override async Task RemoveKeysAsync(IEnumerable<string> keys, string tableName = null)
        {
            tableName = tableName ?? _defaultTableName;
            ArgCheck.NotNull(nameof(tableName), tableName);

            foreach (var keyParition in keys.Partition(1000))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = GetCheckTableScript(tableName) +
                          $@"DELETE FROM [{_schemaName}].[{tableName}] WHERE ([Key] IN ({string.Join(",", Enumerable.Range(0, keyParition.Count()).Select(i => "@key" + i))}));";
                        var p = -1;
                        foreach (var key in keyParition)
                        {
                            command.AddParameter("@key" + (++p), key, DbType.String);
                        }

                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                    connection.Close();
                }
            }
        }

        private string GetCheckTableScript(string tableName)
        {
            return $@"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{_schemaName}' AND TABLE_NAME = '{tableName}')
BEGIN
  CREATE TABLE [{_schemaName}].[{tableName}] (
    [Key] NVARCHAR(1000) NOT NULL,
    [Value] {(_serializer.SerializedType == typeof(string) ? "NVARCHAR(MAX)" : "VARBINARY(MAX)")} NULL,
    [Updated] DATETIME2 NOT NULL,
    [Expiry] DATETIME2 NULL,
    CONSTRAINT [PK_{tableName}] PRIMARY KEY ([Key]),
  );
  CREATE INDEX [IX_{tableName}_Expiry] ON [{_schemaName}].[{tableName}] ([Expiry]);
END";
        }

        private object SerializeValue<T>(T value)
        {
            if (value == null)
            {
                return null;
            }

            return _serializer.Serialize(value);
        }

        private T DeserializeValue<T>(object value)
        {
            if (value == null)
            {
                return default;
            }

            return _serializer.Deserialize<T>(value);
        }
    }
}
