using Blue.Cosacs.Report.Olap;
using Blue.Data;
using StructureMap;
using System;
using System.Linq;

namespace Blue.Cosacs.Report.Generic
{
    public static class GenericReportLoader
    {
        private const string ReportNotFound = "Could not find the report {0}";
        public static ReportResult GetReport(Parameterization rep)
        {
            var report = Xml.ReportDefinitionReader.GetReportDefinition(rep.ReportId);

            if (rep.PageIndex <= 0)
            {
                // For the AngularJs pagination component to work properly, the PageIndex
                // of the first results page MUST ALWAYS BE 1! (never zero or less)
                rep.PageIndex = 1;
            }

            if (report == null)
            {
                throw new Exception(string.Format(ReportNotFound, rep.ReportId));
            }

            if (report.IsServerPaging)
            {
                rep.Filter.Add("PageIndex", rep.PageIndex.ToString());
                rep.Filter.Add("PageSize", rep.PageSize.ToString());
            }
            //TODO: the return from "ExecuteGeneric" should separate the header from the data
            var results = ObjectFactory.Container.GetInstance<IReportQuery>(report.Source).ExecuteGeneric(rep, report);

            if (results.Data == null) return null;

            if (report.IsServerPaging)
            {
                return GetPagedResultServer(rep, results, report.IsServerPaging);
            }

            return GetPagedResult(rep, results);
        }

        private static ReportResult GetPagedResultServer(Parameterization rep, ReportResult results, bool isServerPaging = false)
        {
            var recordCount = 0;
            var index = results.Data.Count < 1 ? -1 : results.Data[0].IndexOf("TotalCount");

            if (index >= 0 && results.Data != null && results.Data.Count > 1)
            {
                int.TryParse(results.Data[1][index], out recordCount);
            }
            else
            {
                if (results.Data.Count > 1)
                    recordCount = results.Data.Count - 1;
            }

            var pageCount = recordCount == 0 ? 1 : (int)Math.Ceiling(((float)recordCount) / ((float)rep.PageSize));

            var previousPageRowsToSkip = rep.PageIndex < 1 ? 0 : rep.PageSize * (rep.PageIndex - 1);

            var returnValue = new ReportResult
            {
                ColumnParameters = results.ColumnParameters,
                AllData = results.Data,
                /*the Data.Take(1) saves the header*/
                Data = results.Data.Take(1)
                    .Union(
                    /* Page.Skip(previousPageRowsToSkip + 1) skips previous rows and the page header */
                        results.Data
                            .Skip((isServerPaging ? 0 : previousPageRowsToSkip) + 1)
                    // If there aren't enough rows, because they here already skipped
                    // .Take(...) takes the remaning rows left and does not crash, no problem :)
                            .Take(rep.PageSize)
                            .ToList()
                    ).ToList(),
                PageCount = pageCount,
                PageIndex = rep.PageIndex,
                PageSize = rep.PageSize
            };

            return returnValue;
        }

        private static ReportResult GetPagedResult(Parameterization rep, ReportResult results)
        {
            var resultsPaged = rep.Page(results.Data.Select(p => p).AsQueryable());

            var returnValue = new ReportResult
            {
                ColumnParameters = results.ColumnParameters,
                /*the Data.Take(1) saves the header*/
                AllData = results.Data,
                Data = results.Data.Take(1)
                    .Union
                    (
                    /*the Page.Skip(1) skips the header from the page*/
                        resultsPaged.Page.Skip(1).ToList()
                    ).ToList(),
                PageCount = resultsPaged.PageCount,
                PageIndex = rep.PageIndex,
                PageSize = rep.PageSize
            };

            return returnValue;
        }
    }
}