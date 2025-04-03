
using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.Warranty
{
    partial class SaveWarrantyStockinfoRequest
    {
        public List<WarrantyStockInfo> StockInfo { get; set; }
        public class WarrantyStockInfo
        {
            public int Id { get; set; }
            public string ItemNo { get; set; }
            public string Description { get; set; }
            public decimal TaxRate { get; set; }
            public int Length { get; set; }
            public short Location { get; set; }
            public decimal Price { get; set; }
            public decimal CostPrice { get; set; }
            public string WarrantyType { get; set; }            // #17313
        }
    }

    partial class SaveWarrantyStockinfoResponse
    {
    }
}
