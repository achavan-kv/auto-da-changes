namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class StockAllocationProductViewModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int ReceivingLocationId { get; set; }

        public int Quantity { get; set; }

        public int? StockAvailable { get; set; }

        public decimal AverageWeightedCost { get; set; }

        public string Sku { get; set; }

        public string Description { get; set; }

        public string ReceivingLocation { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string Category { get; set; }

        public string CorporateUPC { get; set; }

        public int BookingId { get; set; }

        public int? QuantityCancelled { get; set; }

        public int? QuantityReceived { get; set; }

        public int QuantityPending
        {
            get
            {
                return Quantity - (QuantityReceived ?? 0 + QuantityCancelled ?? 0);
            }
        }

        public DateTime? CompletedOn { get; set; }

        public int WarehouseLocationId { get; set; }

        public string WarehouseLocation { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedById { get; set; }

        public string CreatedBy { get; set; }

        public string Comments { get; set; }

        public string WarehouseSalesLocationId { get; set; }

        public string ReceivingSalesLocationId { get; set; }
    }
}