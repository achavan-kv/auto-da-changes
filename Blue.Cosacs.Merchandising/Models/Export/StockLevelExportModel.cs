using FileHelpers;

namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    [DelimitedRecord(",")]
    public class StockLevelExportModel
    {
        public string WarehouseNo { get; set; }
        public string ItemNo { get; set; }
        public string StockFactAvailable { get; set; }
        public string StockActual { get; set; }
        public string StockOnOrder { get; set; }
        public string StockLastPlannedDate { get; set; }
        public string StockDamage { get; set; }
    }
}
