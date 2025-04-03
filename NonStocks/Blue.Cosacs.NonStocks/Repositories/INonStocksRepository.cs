using System.Collections.Generic;

namespace Blue.Cosacs.NonStocks
{
    public interface INonStocksRepository
    {
        int SaveNonStockDetails(NonStock nonStock, List<NonStockHierarchy> hierarchy);
        Models.NonStockModel Load(int id);
        NonStock Load(string sku);
        List<Models.NonStockModel> Load(int[] ids);
        List<Models.NonStockModel> LoadAll();
        string ExportProductsFile(string user);
        string ExportPromotionsFile(string user);
        string ExportProductLinksFile(string user);
        List<Models.NonStockModel> GetActiveNonStocks(IList<string> types);
    }
}
