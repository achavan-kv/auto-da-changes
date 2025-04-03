using FileHelpers;

namespace Blue.Cosacs.Merchandising.Models
{
    [DelimitedRecord(",")]
    public class StockCountProductExportModel
    {
        public string Sku { get; set; }

        public string LongDescription { get; set; }

        public int StartStockOnHand { get; set; }

        public int Count { get; set; }

        public int SystemAdjustment { get; set; }

        public int Variance { get; set; }
       
        public string Comments { get; set; }
    }
}