namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class StockTransferProductViewModel
    {
        public int Id { get; set; }

        public string Sku { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public string Comments { get; set; }

        public string ReferenceNumber { get; set; }

        public decimal? AverageWeightedCost { get; set; }

        public int ProductId { get; set; }

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
    }
}