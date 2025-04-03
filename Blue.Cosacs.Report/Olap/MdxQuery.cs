using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Microsoft.AnalysisServices.AdomdClient;

namespace Blue.Cosacs.Report.Olap
{
    internal static class MdxQuery
    {
        public static CellSet ExecuteCellSet(string olapConnectionName, string mdx)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[olapConnectionName].ConnectionString;
            CellSet returnValue;

            using (var connection = new AdomdConnection(connectionString))
            {
                connection.ShowHiddenObjects = true;//required to get Hierarchy.ParentDimension for dimensions like [Fiscal Quarter]
                connection.Open();

                using (var cmd = new AdomdCommand(mdx, connection))
                {
                    var xmlResult = cmd.ExecuteXmlReader();

                    returnValue = CellSet.LoadXml(xmlResult);
                }
            }

            return returnValue;
        }

        public static void ExecuteReader(string olapConnectionName, string mdx, Action<AdomdDataReader> action)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[olapConnectionName].ConnectionString;

            using (var connection = new AdomdConnection(connectionString))
            {
                connection.Open();

                using (var cmd = new AdomdCommand(mdx, connection))
                {
                    action(cmd.ExecuteReader());
                }
            }
        }
    }
}
