using Blue.Cosacs.Sales.Models;

namespace Blue.Cosacs.Sales.Repositories
{
    public interface IProductRepository
    {
        SolrResult<IndexedProducts> GetSolrResults(string url, int userId);
        SolrResult<SalesItem> ProjectData(SolrResult<IndexedProducts> solrResponse, int locationId, int userId, string fascia, short branch);
    }
}