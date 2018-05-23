// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections;
using System.Collections.Generic;
using KodeAid;

namespace System.Data
{
    public static class IDbConnectionExtensions
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
    }
}
