namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class StockRequisitionStockInfoModel
    {
        public int ProductId { get; set; }

        public string Sku { get; set; }

        public int AvailableStock { get; set; }

        public int StockOnOrder { get; set; }

        public int LastSales { get; set; }

        public int WarehouseAvailableStock { get; set; }

        public int WarehouseStockOnOrder { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        public int StockOnHand { get; set; }

        public int WarehouseStockOnHand { get; set; }

        public int DistributionsOutstanding { get; set; }
    }
}
