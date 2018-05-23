// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


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
    }
}
