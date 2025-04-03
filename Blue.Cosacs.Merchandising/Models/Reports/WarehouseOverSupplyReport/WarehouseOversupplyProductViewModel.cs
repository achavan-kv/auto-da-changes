namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class WarehouseOversupplyProductViewModel
    {
        public WarehouseOversupplyProductViewModel()
        {
            HierarchyTags = new List<string>();
        }

        public int? Id { get; set; }
        public int ProductId { get; set; }
        public string Sku { get; set; }
        public string LongDescription { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int StockOnHandInWarehouse { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public DateTime? DateLastReceived { get; set; }
        public int StockRequisitionPending { get; set; }
        public int LocationsAssigned { get; set; }
        public List<string> HierarchyTags { get; set; }
    }
}