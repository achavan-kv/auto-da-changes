namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class StockTransferQueryModel
    {
        public int? Id { get; set; }

        public int? SendingLocationId { get; set; }

        public int? ReceivingLocationId { get; set; }

        public int? ViaLocationId { get; set; }

        public string ReferenceNumber { get; set; }

        public DateTime? CreatedFrom { get; set; }

        public DateTime? CreatedTo { get; set; }

        public string Sku { get; set; }

        public string Type { get; set; }
    }
}
