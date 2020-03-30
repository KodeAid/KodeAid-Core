// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KodeAid;

namespace System.Data
{
    public static class IDbCommandExtensions
    {
        private static readonly IEnumerable<IDataParameter> _emptyParameters = Enumerable.Empty<IDataParameter>();

        public static IEnumerable<IDataParameter> AutoGenerateParameters(this IDbCommand command, IEnumerable parameterValues, string prefix = "@p")
        {
            ArgCheck.NotNullOrEmpty("prefix", prefix);
            if (parameterValues == null)
                return _emptyParameters;
            var typedValues = parameterValues.Cast<object>().ToList();
            if (!typedValues.Any())
                return _emptyParameters;
            return typedValues.Select((p, i) =>
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = prefix + i;
                parameter.Value = p ?? DBNull.Value;
                return parameter;
            }).ToList();
        }

        public static int ExecuteNonQuery(this IDbCommand command, string commandText)
        {
            return ExecuteNonQuery(command, commandText, (IEnumerable)null);
        }

        public static int ExecuteNonQuery(this IDbCommand command, string commandText, params object[] parameterValues)
        {
            return ExecuteNonQuery(command, commandText, (IEnumerable)parameterValues);
        }

        public static int ExecuteNonQuery(this IDbCommand command, string commandText, IEnumerable parameterValues)
        {
            return ExecuteNonQuery(command, commandText, AutoGenerateParameters(command, parameterValues));
        }

        public static int ExecuteNonQuery(this IDbCommand command, string commandText, params IDataParameter[] parameters)
        {
            return ExecuteNonQuery(command, commandText, (IEnumerable<IDataParameter>)parameters);
        }

        public static int ExecuteNonQuery(this IDbCommand command, string commandText, IEnumerable<IDataParameter> parameters)
        {
            var shouldCloseConnection = false;
            try
            {
                shouldCloseConnection = PrepareCommandAndEnsureConnectionIsOpen(command, commandText, parameters);
                return command.ExecuteNonQuery();
            }
            finally
            {
                if (shouldCloseConnection)
                    command.Connection.Close();
            }
        }

        public static TScalar ExecuteScalar<TScalar>(this IDbCommand command, string commandText)
        {
            return ExecuteScalar<TScalar>(command, commandText, (IEnumerable)null);
        }

        public static TScalar ExecuteScalar<TScalar>(this IDbCommand command, string commandText, params object[] parameterValues)
        {
            return ExecuteScalar<TScalar>(command, commandText, (IEnumerable)parameterValues);
        }

        public static TScalar ExecuteScalar<TScalar>(this IDbCommand command, string commandText, IEnumerable parameterValues)
        {
            return ExecuteScalar<TScalar>(command, commandText, AutoGenerateParameters(command, parameterValues));
        }

        public static TScalar ExecuteScalar<TScalar>(this IDbCommand command, string commandText, params IDataParameter[] parameters)
        {
            return ExecuteScalar<TScalar>(command, commandText, (IEnumerable<IDataParameter>)parameters);
        }

        public static TScalar ExecuteScalar<TScalar>(this IDbCommand command, string commandText, IEnumerable<IDataParameter> parameters)
        {
            return (TScalar)ExecuteScalar(command, commandText, parameters);
        }

        public static object ExecuteScalar(this IDbCommand command, string commandText)
        {
            return ExecuteScalar(command, commandText, (IEnumerable)null);
        }

        public static object ExecuteScalar(this IDbCommand command, string commandText, params object[] parameterValues)
        {
            return ExecuteScalar(command, commandText, (IEnumerable)parameterValues);
        }

        public static object ExecuteScalar(this IDbCommand command, string commandText, IEnumerable parameterValues)
        {
            return ExecuteScalar(command, commandText, AutoGenerateParameters(command, parameterValues));
        }

        public static object ExecuteScalar(this IDbCommand command, string commandText, params IDataParameter[] parameters)
        {
            return ExecuteScalar(command, commandText, (IEnumerable<IDataParameter>)parameters);
        }

        public static object ExecuteScalar(this IDbCommand command, string commandText, IEnumerable<IDataParameter> parameters)
        {
            var shouldCloseConnection = false;
            try
            {
                shouldCloseConnection = PrepareCommandAndEnsureConnectionIsOpen(command, commandText, parameters);
                var result = command.ExecuteScalar();
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

        public static IDataReader ExecuteReader(this IDbCommand command, string commandText)
        {
            return ExecuteReader(command, commandText, (IEnumerable)null);
        }

        public static IDataReader ExecuteReader(this IDbCommand command, string commandText, params object[] parameterValues)
        {
            return ExecuteReader(command, commandText, (IEnumerable)parameterValues);
        }

        public static IDataReader ExecuteReader(this IDbCommand command, string commandText, IEnumerable parameterValues)
        {
            return ExecuteReader(command, commandText, AutoGenerateParameters(command, parameterValues));
        }

        public static IDataReader ExecuteReader(this IDbCommand command, string commandText, params IDataParameter[] parameters)
        {
            return ExecuteReader(command, commandText, (IEnumerable<IDataParameter>)parameters);
        }

        public static IDataReader ExecuteReader(this IDbCommand command, string commandText, IEnumerable<IDataParameter> parameters)
        {
            var shouldCloseConnection = PrepareCommandAndEnsureConnectionIsOpen(command, commandText, parameters);
            return command.ExecuteReader(shouldCloseConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default);
        }

        public static bool TestConnection(this IDbCommand command)
        {
            return command.ExecuteScalar<int>($@"SELECT 1") == 1;
        }

        public static void AddParameter(this IDbCommand command, string name, object value, DbType type)
        {
            ArgCheck.NotNull(nameof(command), command);
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            parameter.DbType = type;
        }

        public static void AddParameter(this IDbCommand command, string name, object value, DbType type, int size)
        {
            ArgCheck.NotNull(nameof(command), command);
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            parameter.DbType = type;
            parameter.Size = size;
            command.Parameters.Add(parameter);
        }

        public static void AddParameter(this IDbCommand command, string name, object value, DbType type, int scale, int precision)
        {
            ArgCheck.NotNull(nameof(command), command);
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            parameter.DbType = type;
            parameter.Scale = (byte)scale;
            parameter.Precision = (byte)precision;
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Prepares the command for execution and returns true if the connection should be closed within the extension method; otherwise false.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns>True if the connection should be closed within the extension method; otherwise false.</returns>
        private static bool PrepareCommandAndEnsureConnectionIsOpen(IDbCommand command, string commandText, IEnumerable<IDataParameter> parameters)
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
                command.Connection.Open();
                return true;
            }
            return false;
        }
    }
}
