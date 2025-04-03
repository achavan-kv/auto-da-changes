using System.Collections.Generic;
using Blue.Cosacs.Stock.Models;

namespace Blue.Cosacs.Stock.Repositories
{
    public interface IProductRepository
    {
        IList<Product> GetAll();
        IList<string> GetStockItemForValidation(WarrantyProductLinkSearch[] productSearch);
        IList<Installation> GetInstallations(string itemNumber, short location);
        Product Convert(StockItemView stock);
        StockItemViewRelations GetProductRelationsByItemNumber(string itemNumber);
        WarrantyProductLinkSearch FindMatchingProduct(string productNumber, short branchNumber);
        List<WarrantyProductLinkSearch> GetProductsByCategory(string department, short category, short branchNumber);
    }
}
