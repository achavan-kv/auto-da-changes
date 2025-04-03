using Blue.Cosacs.Report.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Blue.Cosacs.Report.Olap
{
    public class ReportMdxQuery : IReportQuery
    {
        private const string OlapConnectionString = "Olap";
        private const string ReplaceTypeName = "replace";

        public ReportResult ExecuteGeneric(Parameterization parameters, Report.Xml.Report report)
        {
            var queryParser = StructureMap.ObjectFactory.Container.GetInstance<ICellSetParser<List<List<string>>>>();

            var mdxQueryResult = MdxQuery.ExecuteCellSet(OlapConnectionString, GetMdxQuery(report.Query, parameters.Filter));
            var reportData = queryParser.Parse(mdxQueryResult);
            var replaceColumns = GetHeaderNameReplacements(report);
            reportData = ReplaceReportDataHeaders(reportData, replaceColumns);

            return new ReportResult()
            {
                ColumnParameters = report.Columns.Where(e => e.Type != ReplaceTypeName).ToList(),
                ReplaceColumns = replaceColumns,
                Data = reportData
            };
        }

        private List<List<string>> ReplaceReportDataHeaders(List<List<string>> reportData, List<Column> replaceColumns)
        {
            if (reportData == null || reportData.Count == 0 || reportData[0] == null || reportData[0].Count == 0)
            {
                return reportData;
            }

            foreach (var replaceColumn in replaceColumns)
            {
                for (int i = 0; i < reportData[0].Count; i++)
                {
                    if (string.Compare(replaceColumn.ColumnName, reportData[0][i], true) == 0)
                    {
                        reportData[0][i] = replaceColumn.ColumnReplaceValue;
                        break;
                    }
                }
            }

            return reportData;
        }

        private List<Column> GetHeaderNameReplacements(Xml.Report report)
        {
            var retList = new List<Column>();
            if (report == null || report.Columns == null)
            {
                return retList;
            }

            foreach (var col in report.Columns.Where(p => p.Type == ReplaceTypeName))
            {
                retList.Add(col);
            }

            return retList;
        }

        private string GetMdxQuery(string mdx, Dictionary<string, string> filter)
        {
            var returnValue = mdx;
            foreach (var f in filter)
            {
                returnValue = Regex.Replace(returnValue,
                    string.Format(@"\?{0}", f.Key), f.Value, RegexOptions.IgnoreCase | RegexOptions.Compiled);

                if (f.Value == "ALLMEMBERS")
                {
                    returnValue = returnValue.Replace(".&[ALLMEMBERS]", ".ALLMEMBERS");
                }
            }

            return returnValue;
        }
    }
}
