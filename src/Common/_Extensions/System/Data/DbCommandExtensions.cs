// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KodeAid;

namespace System.Data
{
    public static class DbCommandExtensions
    {
        public static Task<int> ExecuteNonQueryAsync(this DbCommand command, string commandText)
        {
            return ExecuteNonQueryAsync(command, commandText, CancellationToken.None);
        }

        public static Task<int> ExecuteNonQueryAsync(this DbCommand command, string commandText, CancellationToken cancellationToken)
        {
            return ExecuteNonQueryAsync(command, commandText, (IEnumerable)null, cancellationToken);
        }

        public static Task<int> ExecuteNonQueryAsync(this DbCommand command, string commandText, params object[] parameterValues)
        {
            return ExecuteNonQueryAsync(command, commandText, CancellationToken.None, parameterValues);
        }

        public static Task<int> ExecuteNonQueryAsync(this DbCommand command, string commandText, CancellationToken cancellationToken, params object[] parameterValues)
        {
            return ExecuteNonQueryAsync(command, commandText, parameterValues, cancellationToken);
        }

        public static Task<int> ExecuteNonQueryAsync(this DbCommand command, string commandText, IEnumerable parameterValues)
        {
            return ExecuteNonQueryAsync(command, commandText, parameterValues, CancellationToken.None);
        }

        public static Task<int> ExecuteNonQueryAsync(this DbCommand command, string commandText, IEnumerable parameterValues, CancellationToken cancellationToken)
        {
            return ExecuteNonQueryAsync(command, commandText, command.AutoGenerateParameters(parameterValues), cancellationToken);
        }

        public static Task<int> ExecuteNonQueryAsync(this DbCommand command, string commandText, params IDataParameter[] parameters)
        {
            return ExecuteNonQueryAsync(command, commandText, CancellationToken.None, parameters);
        }

        public static Task<int> ExecuteNonQueryAsync(this DbCommand command, string commandText, CancellationToken cancellationToken, params IDataParameter[] parameters)
        {
            return ExecuteNonQueryAsync(command, commandText, parameters, cancellationToken);
        }

        public static Task<int> ExecuteNonQueryAsync(this DbCommand command, string commandText, IEnumerable<IDataParameter> parameters)
        {
            return ExecuteNonQueryAsync(command, commandText, parameters, CancellationToken.None);
        }

        public static async Task<int> ExecuteNonQueryAsync(this DbCommand command, string commandText, IEnumerable<IDataParameter> parameters, CancellationToken cancellationToken)
        {
            var shouldCloseConnection = false;
            try
            {
                shouldCloseConnection = await PrepareCommandAndEnsureConnectionIsOpenAsync(command, commandText, parameters, cancellationToken).ConfigureAwait(false);
                return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                if (shouldCloseConnection)
                    command.Connection.Close();
            }
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbCommand command, string commandText)
        {
            return ExecuteScalarAsync<TScalar>(command, commandText, CancellationToken.None);
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbCommand command, string commandText, CancellationToken cancellationToken)
        {
            return ExecuteScalarAsync<TScalar>(command, commandText, (IEnumerable)null, cancellationToken);
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbCommand command, string commandText, params object[] parameterValues)
        {
            return ExecuteScalarAsync<TScalar>(command, commandText, CancellationToken.None, parameterValues);
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbCommand command, string commandText, CancellationToken cancellationToken, params object[] parameterValues)
        {
            return ExecuteScalarAsync<TScalar>(command, commandText, parameterValues, cancellationToken);
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbCommand command, string commandText, IEnumerable parameterValues)
        {
            return ExecuteScalarAsync<TScalar>(command, commandText, parameterValues, CancellationToken.None);
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbCommand command, string commandText, IEnumerable parameterValues, CancellationToken cancellationToken)
        {
            return ExecuteScalarAsync<TScalar>(command, commandText, command.AutoGenerateParameters(parameterValues), cancellationToken);
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbCommand command, string commandText, params IDataParameter[] parameters)
        {
            return ExecuteScalarAsync<TScalar>(command, commandText, CancellationToken.None, parameters);
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbCommand command, string commandText, CancellationToken cancellationToken, params IDataParameter[] parameters)
        {
            return ExecuteScalarAsync<TScalar>(command, commandText, parameters, cancellationToken);
        }

        public static Task<TScalar> ExecuteScalarAsync<TScalar>(this DbCommand command, string commandText, IEnumerable<IDataParameter> parameters)
        {
            return ExecuteScalarAsync<TScalar>(command, commandText, parameters, CancellationToken.None);
        }

        public static async Task<TScalar> ExecuteScalarAsync<TScalar>(this DbCommand command, string commandText, IEnumerable<IDataParameter> parameters, CancellationToken cancellationToken)
        {
            return (TScalar)await ExecuteScalarAsync(command, commandText, parameters, cancellationToken).ConfigureAwait(false);
        }

        public static Task<object> ExecuteScalarAsync(this DbCommand command, string commandText)
        {
            return ExecuteScalarAsync(command, commandText, CancellationToken.None);
        }

        public static Task<object> ExecuteScalarAsync(this DbCommand command, string commandText, CancellationToken cancellationToken)
        {
            return ExecuteScalarAsync(command, commandText, (IEnumerable)null, cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(this DbCommand command, string commandText, params object[] parameterValues)
        {
            return ExecuteScalarAsync(command, commandText, CancellationToken.None, parameterValues);
        }

        public static Task<object> ExecuteScalarAsync(this DbCommand command, string commandText, CancellationToken cancellationToken, params object[] parameterValues)
        {
            return ExecuteScalarAsync(command, commandText, parameterValues, cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(this DbCommand command, string commandText, IEnumerable parameterValues)
        {
            return ExecuteScalarAsync(command, commandText, parameterValues, CancellationToken.None);
        }

        public static Task<object> ExecuteScalarAsync(this DbCommand command, string commandText, IEnumerable parameterValues, CancellationToken cancellationToken)
        {
            return ExecuteScalarAsync(command, commandText, command.AutoGenerateParameters(parameterValues), cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(this DbCommand command, string commandText, params IDataParameter[] parameters)
        {
            return ExecuteScalarAsync(command, commandText, CancellationToken.None, parameters);
        }

        public static Task<object> ExecuteScalarAsync(this DbCommand command, string commandText, CancellationToken cancellationToken, params IDataParameter[] parameters)
        {
            return ExecuteScalarAsync(command, commandText, parameters, cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(this DbCommand command, string commandText, IEnumerable<IDataParameter> parameters)
        {
            return ExecuteScalarAsync(command, commandText, parameters, CancellationToken.None);
        }

        public static async Task<object> ExecuteScalarAsync(this DbCommand command, string commandText, IEnumerable<IDataParameter> parameters, CancellationToken cancellationToken)
        {
            var shouldCloseConnection = false;
            try
            {
                shouldCloseConnection = await PrepareCommandAndEnsureConnectionIsOpenAsync(command, commandText, parameters, cancellationToken).ConfigureAwait(false);
                var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                if (result == DBNull.Value)
                    return null;
                return result;
            }
            finally
            {
                if (shouldCloseConnection)
                    command.Connection.Close();
            }
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbCommand command, string commandText)
        {
            return ExecuteReaderAsync(command, commandText, CancellationToken.None);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbCommand command, string commandText, CancellationToken cancellationToken)
        {
            return ExecuteReaderAsync(command, commandText, (IEnumerable)null, cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbCommand command, string commandText, params object[] parameterValues)
        {
            return ExecuteReaderAsync(command, commandText, CancellationToken.None, parameterValues);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbCommand command, string commandText, CancellationToken cancellationToken, params object[] parameterValues)
        {
            return ExecuteReaderAsync(command, commandText, parameterValues, cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbCommand command, string commandText, IEnumerable parameterValues)
        {
            return ExecuteReaderAsync(command, commandText, parameterValues, CancellationToken.None);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbCommand command, string commandText, IEnumerable parameterValues, CancellationToken cancellationToken)
        {
            return ExecuteReaderAsync(command, commandText, command.AutoGenerateParameters(parameterValues), cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbCommand command, string commandText, params IDataParameter[] parameters)
        {
            return ExecuteReaderAsync(command, commandText, CancellationToken.None, parameters);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbCommand command, string commandText, CancellationToken cancellationToken, params IDataParameter[] parameters)
        {
            return ExecuteReaderAsync(command, commandText, parameters, cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbCommand command, string commandText, IEnumerable<IDataParameter> parameters)
        {
            return ExecuteReaderAsync(command, commandText, parameters, CancellationToken.None);
        }

        public static async Task<DbDataReader> ExecuteReaderAsync(this DbCommand command, string commandText, IEnumerable<IDataParameter> parameters, CancellationToken cancellationToken)
        {
            var shouldCloseConnection = await PrepareCommandAndEnsureConnectionIsOpenAsync(command, commandText, parameters, cancellationToken).ConfigureAwait(false);
            return await command.ExecuteReaderAsync(shouldCloseConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<bool> TestConnectionAsync(this DbCommand command)
        {
            return (await command.ExecuteScalarAsync<int>($@"SELECT 1").ConfigureAwait(false)) == 1;
        }

        /// <summary>
        /// Prepares the command for execution and returns true if the connection should be closed within the extension method; otherwise false.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns>True if the connection should be closed within the extension method; otherwise false.</returns>
        private static async Task<bool> PrepareCommandAndEnsureConnectionIsOpenAsync(DbCommand command, string commandText, IEnumerable<IDataParameter> parameters, CancellationToken cancellationToken)
        {
            ArgCheck.NotNull("command", command);
            ArgCheck.NotNullOrEmpty("commandText", commandText);
            command.CommandText = commandText;
            if (parameters != null && parameters.Any())
            {
                foreach (var parameter in parameters)
                    command.Parameters.Add(parameter);
            }

            if (command.Connection.State == ConnectionState.Closed)
            {
                await command.Connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                return true;
            }
            return false;
        }
    }
}
