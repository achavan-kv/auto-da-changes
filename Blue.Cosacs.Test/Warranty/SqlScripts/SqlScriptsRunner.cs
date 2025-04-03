using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Blue.Cosacs.Test.Warranty.SqlScripts
{
    public class SqlScriptsRunner
    {
        public static void ExecuteScript(string connectionString, string fileName)
        {
            var script = File.ReadAllText(fileName);

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                _execScript(conn, script);
                conn.Close();
            }
        }

        private static void _execScript(SqlConnection connection, string script)
        {
            var commStr = Regex.Split(script, "\n[\t ]*GO", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            using (var cmd = new SqlCommand(string.Empty, connection))
            {
                foreach (string c in commStr.Where(c => c.Trim() != string.Empty))
                {
                    if (c.Length >= 3 && c.Substring(0, 3).ToLower() == "use")
                    {
                        throw new Exception("Cannot execute USE statements!");
                    }

                    cmd.CommandText = c;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void RunStoredProcedure(string connectionString, string storedProcedureName)
        {
            RunStoredProcedure(connectionString, storedProcedureName, new List<SqlParameter>());
        }

        public static void RunStoredProcedure(string connectionString, string storedProcedureName, List<SqlParameter> parameterCollection)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand(storedProcedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameterCollection.ToArray());

                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

    }
}
