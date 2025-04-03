using Blue.Cosacs.Report.Olap;
using System.Collections.Generic;
using Blue.Cosacs.Report.Sql;

namespace Blue.Cosacs.Report
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            For<ICellSetParser<List<List<string>>>>().Add<GenericCellSetParser>().Named(GenericCellSetParser.StructureMapNamed);
            For<IReportQuery>().Use<ReportMdxQuery>().Named("OLAP");
            For<IReportQuery>().Use<ReportSqlQuery>().Named("SQL");
        }
    }
}
