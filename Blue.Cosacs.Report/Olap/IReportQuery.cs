using Microsoft.AnalysisServices.AdomdClient;

namespace Blue.Cosacs.Report.Olap
{
    public interface IReportQuery
    {
        ReportResult ExecuteGeneric(Parameterization parameters, Report.Xml.Report report);
    }
}
