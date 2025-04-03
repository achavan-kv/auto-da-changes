using FileHelpers;

namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    [DelimitedRecord(",")]
    public class PurchaseOrderExportModel
    {
        public string WarehouseNo { get; set; }
        public string ItemNo { get; set; }
        public string Supplier { get; set; }
        public string OrderNo { get; set; }
        public string DeliveryDate { get; set; }
        public string OrderQuantity { get; set; }
    }
}
