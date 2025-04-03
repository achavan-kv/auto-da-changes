using Blue.Cosacs.Report.Olap;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace Blue.Cosacs.Report.Sql
{
    public class ReportSqlQuery : IReportQuery
    {
        private const string SqlConnectionString = "Report";
        private const int _timeout = 300;              // 5 minutes

        public ReportResult ExecuteGeneric(Parameterization parameters, Report.Xml.Report report)
        {
            var queryParser = StructureMap.ObjectFactory.Container.GetInstance<ICellSetParser<List<List<string>>>>();
            var returnValue = new ReportResult()
            {
                ColumnParameters = report.Columns,
                Data = new List<List<string>>()
            };

            var columns = new List<string>();
            var connectionString = ConfigurationManager.ConnectionStrings[SqlConnectionString].ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var cmd = new SqlCommand(report.Query.Trim(), connection);
                cmd.Parameters.AddRange(GetParameters(parameters.Filter));
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = GetTimeout();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        //get columns names
                        var colrow = new object[reader.FieldCount];
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            //returnValue.Data.Add
                            colrow[i] = reader.GetName(i).ToString();
                        }

                        returnValue.Data.Add(colrow.Select(p => (p ?? string.Empty).ToString())
                                .ToList());

                        //get data
                        while (reader.Read())
                        {
                            var row = new object[reader.FieldCount];
                            reader.GetValues(row);
                            returnValue.Data.Add(row.Select(p => (p ?? string.Empty).ToString()).ToList());
                        }
                    }
                }

                return returnValue;
            }
        }

        private SqlParameter[] GetParameters(Dictionary<string, string> filter)
        {
            return filter
                .Select(p => new SqlParameter(p.Key, p.Value))
                .ToArray();
        }

        private int GetTimeout()
        {
            var retTimeout = _timeout;
            var longRunningQueryTimeoutStr =
                ConfigurationManager.AppSettings["LongRunningQueryCommandTimeoutInSecs"];

            if (string.IsNullOrWhiteSpace(longRunningQueryTimeoutStr))
            {
                return _timeout;
            }

            var tmpInt = -1;
            if (int.TryParse(longRunningQueryTimeoutStr, out tmpInt))
            {
                retTimeout = tmpInt;
            }

            return retTimeout;
        }

    }
}
