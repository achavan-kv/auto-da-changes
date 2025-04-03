using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public class StockItemDetails
    {
        public StockInfo StockInfo { get; set; }
        public List<StockPrice> StockPrices { get; set; }
        public List<StockQuantity> StockQuantities { get; set; }
    }
}
