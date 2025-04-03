using Blue.Cosacs.NonStocks.Models;
using Blue.Cosacs.NonStocks.Models.WinCosacs;
using System.Collections.Generic;

namespace Blue.Cosacs.NonStocks
{
    public interface IPriceRepository
    {
        NonStockPriceModel DeletePrice(int id);
        List<NonStockPriceModel> GetActiveExpandedPrices(int id, IList<BranchInfo> allBranches);
        List<NonStockPriceModel> GetPrices(int id);
        NonStockPriceModel SavePrice(NonStockPriceModel nonStockPrice);
    }
}
