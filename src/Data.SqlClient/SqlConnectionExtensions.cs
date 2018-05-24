// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Threading.Tasks;
using KodeAid;

namespace System.Data.SqlClient
{
    public static class SqlConnectionExtensions
    {
        public static Task<bool> DoesTableExistAsync(this SqlConnection connection, string table, string schema = "dbo")
        {
            return DoesObjectExistAsync(connection, table, "U", schema);
        }

        public static Task<bool> DoesTriggerExistAsync(this SqlConnection connection, string trigger, string schema = "dbo")
        {
            return DoesObjectExistAsync(connection, trigger, "TR", schema);
        }

        public static Task<bool> DoesObjectExistAsync(this SqlConnection connection, string name, string type, string schema = "dbo")
        {
            ArgCheck.NotNull(nameof(connection), connection);
            using (var command = connection.CreateCommand())
                return command.DoesObjectExistAsync(name, type, schema);
        }
    }
}
