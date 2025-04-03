using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace STL.DAL
{
    public static class Connections
    {
        public const string DefaultName = "Default";
        private const string ReportName = "Report";

        public static string Default
        {
            get { return ConfigurationManager.ConnectionStrings[DefaultName].ConnectionString; }
        }

        public static string Report
        {
            get { return ConfigurationManager.ConnectionStrings[ReportName].ConnectionString; }
        }


        public static string DefaultDatabaseName
        {
            get
            {
                var parts = Default.Split(';', '=');

                for (var i = 0; i < parts.Length; i += 2)
                {
                    var name = parts[i].ToLower();
                    var value = parts[i + 1];
                    if (string.Compare(name, "initial catalog") == 0
                     || string.Compare(name, "database") == 0)
                        return value;
                }
                throw new ApplicationException("Database name not found in Default connection string. Must be either 'database' or 'initial catalog'.");
            }
        }
    }
}
