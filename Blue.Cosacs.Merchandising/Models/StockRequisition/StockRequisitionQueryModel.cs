namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class StockRequisitionQueryModel
    {
        public int? Id { get; set; }

        public int? BookingId { get; set; }

        public int? WarehouseLocationId { get; set; }

        public int? ReceivingLocationId { get; set; }

        public string ReferenceNumber { get; set; }

        public DateTime? CreatedFrom { get; set; }

        public DateTime? CreatedTo { get; set; }

        public string Sku { get; set; }
        
        public string Status { get; set; }
    }
}
