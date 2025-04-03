using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Merchandising.Models
{
    using FileHelpers;

    [DelimitedRecord(",")]
    public class AbbreviatedStockExportModel
    {
        public string Hierarchy { get; set; }
        public string LocationName { get; set; }
        public string Sku { get; set; }
        public string LongDescription { get; set; }
        public string VendorName { get; set; }
        public string VendorUPC { get; set; }
        public string StockOnHand { get; set; }
        public string RegularPrice { get; set; }
        public string CashPrice { get; set; }
        public string LastLandedCost { get; set; }
        public string AverageWeightedCost { get; set; }
    }
}