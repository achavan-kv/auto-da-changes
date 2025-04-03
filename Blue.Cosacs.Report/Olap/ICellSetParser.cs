using Microsoft.AnalysisServices.AdomdClient;

namespace Blue.Cosacs.Report.Olap
{
    public interface ICellSetParser<T>
    {
        T Parse(CellSet set);
    }
}
