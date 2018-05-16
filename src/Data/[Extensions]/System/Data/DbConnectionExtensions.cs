// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using KodeAid;

namespace System.Data
{
    public static class DbConnectionExtensions
    {
        public static int ExecuteNonQuery(this IDbConnection connection, string commandText)
        {
            return ExecuteNonQuery(connection, commandText, (IEnumerable)null);
        }

        public static int ExecuteNonQuery(this IDbConnection connection, string commandText, params object[] parameterValues)
        {
            return ExecuteNonQuery(connection, commandText, (IEnumerable)parameterValues);
        }

        public static int ExecuteNonQuery(this IDbConnection connection, string commandText, IEnumerable parameterValues)
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.ExecuteNonQuery(commandText, parameterValues);
        }

        public static int ExecuteNonQuery(this IDbConnection connection, string commandText, params IDataParameter[] parameters)
        {
            return ExecuteNonQuery(connection, commandText, (IEnumerable<IDataParameter>)parameters);
        }

        public static int ExecuteNonQuery(this IDbConnection connection, string commandText, IEnumerable<IDataParameter> parameters)
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.ExecuteNonQuery(commandText, parameters);
        }

        public static TScalar ExecuteScalar<TScalar>(this IDbConnection connection, string commandText)
        {
            return ExecuteScalar<TScalar>(connection, commandText, (IEnumerable)null);
        }

        public static TScalar ExecuteScalar<TScalar>(this IDbConnection connection, string commandText, params object[] parameterValues)
        {
            return ExecuteScalar<TScalar>(connection, commandText, (IEnumerable)parameterValues);
        }

        public static TScalar ExecuteScalar<TScalar>(this IDbConnection connection, string commandText, IEnumerable parameterValues)
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.ExecuteScalar<TScalar>(commandText, parameterValues);
        }

        public static TScalar ExecuteScalar<TScalar>(this IDbConnection connection, string commandText, params IDataParameter[] parameters)
        {
            return ExecuteScalar<TScalar>(connection, commandText, (IEnumerable<IDataParameter>)parameters);
        }

        public static TScalar ExecuteScalar<TScalar>(this IDbConnection connection, string commandText, IEnumerable<IDataParameter> parameters)
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.ExecuteScalar<TScalar>(commandText, parameters);
        }

        public static object ExecuteScalar(this IDbConnection connection, string commandText)
        {
            return ExecuteScalar(connection, commandText, (IEnumerable)null);
        }

        public static object ExecuteScalar(this IDbConnection connection, string commandText, params object[] parameterValues)
        {
            return ExecuteScalar(connection, commandText, (IEnumerable)parameterValues);
        }

        public static object ExecuteScalar(this IDbConnection connection, string commandText, IEnumerable parameterValues)
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.ExecuteScalar(commandText, parameterValues);
        }

        public static object ExecuteScalar(this IDbConnection connection, string commandText, params IDataParameter[] parameters)
        {
            return ExecuteScalar(connection, commandText, (IEnumerable<IDataParameter>)parameters);
        }

        public static object ExecuteScalar(this IDbConnection connection, string commandText, IEnumerable<IDataParameter> parameters)
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.ExecuteScalar(commandText, parameters);
        }

        public static IDataReader ExecuteReader(this IDbConnection connection, string commandText)
        {
            return ExecuteReader(connection, commandText, (IEnumerable)null);
        }

        public static IDataReader ExecuteReader(this IDbConnection connection, string commandText, params object[] parameterValues)
        {
            return ExecuteReader(connection, commandText, (IEnumerable)parameterValues);
        }

        public static IDataReader ExecuteReader(this IDbConnection connection, string commandText, IEnumerable parameterValues)
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.ExecuteReader(commandText, parameterValues);
        }

        public static IDataReader ExecuteReader(this IDbConnection connection, string commandText, params IDataParameter[] parameters)
        {
            return ExecuteReader(connection, commandText, (IEnumerable<IDataParameter>)parameters);
        }

        public static IDataReader ExecuteReader(this IDbConnection connection, string commandText, IEnumerable<IDataParameter> parameters)
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.ExecuteReader(commandText, parameters);
        }

        public static bool TestConnection(this IDbConnection connection)
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.TestConnection();
        }

        public static bool DoesTableExist(this IDbConnection connection, string table, string schema = "dbo")
        {
            return DoesObjectExist(connection, table, "U", schema);
        }

        public static bool DoesTriggerExist(this IDbConnection connection, string trigger, string schema = "dbo")
        {
            return DoesObjectExist(connection, trigger, "TR", schema);
        }

        public static bool DoesObjectExist(this IDbConnection connection, string name, string type, string schema = "dbo")
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.DoesObjectExist(name, type, schema);
        }

        public static Task<int> ExecuteNonQueryAsync(this DbConnection connection, string commandText)
        {
            return ExecuteNonQueryAsync(connection, commandText, CancellationToken.None);
        }

        public static Task<int> ExecuteNonQueryAsync(this DbConnection connection, string commandText, CancellationToken cancellationToken)
        {
            return ExecuteNonQueryAsync(connection, commandText, (IEnumerable)null, cancellationToken);
        }

        public static Task<int> ExecuteNonQueryAsync(this DbConnection connection, string commandText, params object[] parameterValues)
        {
            return ExecuteNonQueryAsync(connection, commandText, CancellationToken.None, parameterValues);
        }

        public static Task<int> ExecuteNonQueryAsync(this DbConnection connection, string commandText, CancellationToken cancellationToken, params object[] parameterValues)
        {
            return ExecuteNonQueryAsync(connection, commandText, parameterValues, cancellationToken);
        }

        public static Task<int> ExecuteNonQueryAsync(this DbConnection connection, string commandText, IEnumerable parameterValues)
        {
            return ExecuteNonQueryAsync(connection, commandText, parameterValues, CancellationToken.None);
        }

        public static Task<int> ExecuteNonQueryAsync(this DbConnection connection, string commandText, IEnumerable parameterValues, CancellationToken cancellationToken)
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.ExecuteNonQueryAsync(commandText, parameterValues, cancellationToken);
        }

        public static Task<int> ExecuteNonQueryAsync(this DbConnection connection, string commandText, params IDataParameter[] parameters)
        {
            return ExecuteNonQueryAsync(connection, commandText, CancellationToken.None, parameters);
        }

        public static Task<int> ExecuteNonQueryAsync(this DbConnection connection, string commandText, CancellationToken cancellationToken, params IDataParameter[] parameters)
        {
            return ExecuteNonQueryAsync(connection, commandText, parameters, cancellationToken);
        }

        public static Task<int> ExecuteNonQueryAsync(this DbConnection connection, string commandText, IEnumerable<IDataParameter> parameters)
        {
            return ExecuteNonQueryAsync(connection, commandText, parameters, CancellationToken.None);
        }

        public static Task<int> ExecuteNonQueryAsync(this DbConnection connection, string commandText, IEnumerable<IDataParameter> parameters, CancellationToken cancellationToken)
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.ExecuteNonQueryAsync(commandText, parameters, cancellationToken);
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbConnection connection, string commandText)
        {
            return ExecuteScalarAsync<TScalar>(connection, commandText, CancellationToken.None);
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbConnection connection, string commandText, CancellationToken cancellationToken)
        {
            return ExecuteScalarAsync<TScalar>(connection, commandText, (IEnumerable)null, cancellationToken);
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbConnection connection, string commandText, params object[] parameterValues)
        {
            return ExecuteScalarAsync<TScalar>(connection, commandText, CancellationToken.None, parameterValues);
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbConnection connection, string commandText, CancellationToken cancellationToken, params object[] parameterValues)
        {
            return ExecuteScalarAsync<TScalar>(connection, commandText, parameterValues, cancellationToken);
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbConnection connection, string commandText, IEnumerable parameterValues)
        {
            return ExecuteScalarAsync<TScalar>(connection, commandText, parameterValues, CancellationToken.None);
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbConnection connection, string commandText, IEnumerable parameterValues, CancellationToken cancellationToken)
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.ExecuteScalarAsync<TScalar>(commandText, parameterValues, cancellationToken);
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbConnection connection, string commandText, params IDataParameter[] parameters)
        {
            return ExecuteScalarAsync<TScalar>(connection, commandText, CancellationToken.None, parameters);
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbConnection connection, string commandText, CancellationToken cancellationToken, params IDataParameter[] parameters)
        {
            return ExecuteScalarAsync<TScalar>(connection, commandText, parameters, cancellationToken);
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbConnection connection, string commandText, IEnumerable<IDataParameter> parameters)
        {
            return ExecuteScalarAsync<TScalar>(connection, commandText, parameters, CancellationToken.None);
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbConnection connection, string commandText, IEnumerable<IDataParameter> parameters, CancellationToken cancellationToken)
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.ExecuteScalarAsync<TScalar>(commandText, parameters, cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(this DbConnection connection, string commandText)
        {
            return ExecuteScalarAsync(connection, commandText, CancellationToken.None);
        }

        public static Task<object> ExecuteScalarAsync(this DbConnection connection, string commandText, CancellationToken cancellationToken)
        {
            return ExecuteScalarAsync(connection, commandText, (IEnumerable)null, cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(this DbConnection connection, string commandText, params object[] parameterValues)
        {
            return ExecuteScalarAsync(connection, commandText, CancellationToken.None, parameterValues);
        }

        public static Task<object> ExecuteScalarAsync(this DbConnection connection, string commandText, CancellationToken cancellationToken, params object[] parameterValues)
        {
            return ExecuteScalarAsync(connection, commandText, parameterValues, cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(this DbConnection connection, string commandText, IEnumerable parameterValues)
        {
            return ExecuteScalarAsync(connection, commandText, parameterValues, CancellationToken.None);
        }

        public static Task<object> ExecuteScalarAsync(this DbConnection connection, string commandText, IEnumerable parameterValues, CancellationToken cancellationToken)
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.ExecuteScalarAsync(commandText, parameterValues, cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(this DbConnection connection, string commandText, params IDataParameter[] parameters)
        {
            return ExecuteScalarAsync(connection, commandText, CancellationToken.None, parameters);
        }

        public static Task<object> ExecuteScalarAsync(this DbConnection connection, string commandText, CancellationToken cancellationToken, params IDataParameter[] parameters)
        {
            return ExecuteScalarAsync(connection, commandText, parameters, cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(this DbConnection connection, string commandText, IEnumerable<IDataParameter> parameters)
        {
            return ExecuteScalarAsync(connection, commandText, parameters, CancellationToken.None);
        }

        public static Task<object> ExecuteScalarAsync(this DbConnection connection, string commandText, IEnumerable<IDataParameter> parameters, CancellationToken cancellationToken)
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.ExecuteScalarAsync(commandText, parameters, cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbConnection connection, string commandText)
        {
            return ExecuteReaderAsync(connection, commandText, CancellationToken.None);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbConnection connection, string commandText, CancellationToken cancellationToken)
        {
            return ExecuteReaderAsync(connection, commandText, (IEnumerable)null, cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbConnection connection, string commandText, params object[] parameterValues)
        {
            return ExecuteReaderAsync(connection, commandText, CancellationToken.None, parameterValues);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbConnection connection, string commandText, CancellationToken cancellationToken, params object[] parameterValues)
        {
            return ExecuteReaderAsync(connection, commandText, parameterValues, cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbConnection connection, string commandText, IEnumerable parameterValues)
        {
            return ExecuteReaderAsync(connection, commandText, parameterValues, CancellationToken.None);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbConnection connection, string commandText, IEnumerable parameterValues, CancellationToken cancellationToken)
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.ExecuteReaderAsync(commandText, parameterValues, cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbConnection connection, string commandText, params IDataParameter[] parameters)
        {
            return ExecuteReaderAsync(connection, commandText, CancellationToken.None, parameters);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbConnection connection, string commandText, CancellationToken cancellationToken, params IDataParameter[] parameters)
        {
            return ExecuteReaderAsync(connection, commandText, parameters, cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbConnection connection, string commandText, IEnumerable<IDataParameter> parameters)
        {
            return ExecuteReaderAsync(connection, commandText, parameters, CancellationToken.None);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbConnection connection, string commandText, IEnumerable<IDataParameter> parameters, CancellationToken cancellationToken)
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.ExecuteReaderAsync(commandText, parameters, cancellationToken);
        }

        public static Task<bool> TestConnectionAsync(this DbConnection connection)
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.TestConnectionAsync();
        }

        public static Task<bool> DoesTableExistAsync(this DbConnection connection, string table, string schema = "dbo")
        {
            return DoesObjectExistAsync(connection, table, "U", schema);
        }

        public static Task<bool> DoesTriggerExistAsync(this DbConnection connection, string trigger, string schema = "dbo")
        {
            return DoesObjectExistAsync(connection, trigger, "TR", schema);
        }

        public static Task<bool> DoesObjectExistAsync(this DbConnection connection, string name, string type, string schema = "dbo")
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.DoesObjectExistAsync(name, type, schema);
        }
    }
}
