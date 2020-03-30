// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Threading.Tasks;
using KodeAid;

namespace System.Data.SqlClient
{
    public static class SqlCommandExtensions
    {
        public static Task<bool> DoesTableExistAsync(this SqlCommand command, string table, string schema = "dbo")
        {
            return DoesObjectExistAsync(command, table, "U", schema);
        }

        public static Task<bool> DoesTriggerExistAsync(this SqlCommand command, string trigger, string schema = "dbo")
        {
            return DoesObjectExistAsync(command, trigger, "TR", schema);
        }

        public static async Task<bool> DoesObjectExistAsync(this SqlCommand command, string name, string type, string schema = "dbo")
        {
            ArgCheck.NotNull(nameof(command), command);
            return (await command.ExecuteScalarAsync<int>($@"IF OBJECT_ID('[{schema}].[{name}]', '{type}') IS NOT NULL SELECT 1 ELSE SELECT 0").ConfigureAwait(false)) == 1;
        }
    }
}
