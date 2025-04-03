namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using FileHelpers;

    [DelimitedRecord(",")]
    public class WarehouseOversupplyExportModel
    {
        public string Division { get; set; }
        public string Department { get; set; }
        public string Class { get; set; }
        public string Sku { get; set; }
        public string LongDescription { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int StockOnHandInWarehouse { get; set; }
        public string LocationName { get; set; }
        public DateTime DateLastReceived { get; set; }
        public int StockRequisitionPending { get; set; }
        public int LocationsAssigned { get; set; }
    }
}