namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class StockRequisitionSearchResultModel
    {
        public int Id { get; set; }
        
        public int ProductId { get; set; }

        public string Sku { get; set; }

        public string LongDescription { get; set; }

        public string Status { get; set; }

        public int WarehouseLocationId { get; set; }

        public int ReceivingLocationId { get; set; }

        public string WarehouseLocation { get; set; }

        public string ReceivingLocation { get; set; }

        public string Comments { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedById { get; set; }

        public string CreatedBy { get; set; }

        public string ReferenceNumber { get; set; }

        public int Quantity { get; set; }
        
        public int? QuantityCancelled { get; set; }
        
        public int? QuantityReceived { get; set; }

        public decimal AverageWeightedCost { get; set; }

        public int BookingId { get; set; }
    }
}
