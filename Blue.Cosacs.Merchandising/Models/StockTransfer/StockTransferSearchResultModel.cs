namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class StockTransferSearchResultModel
    {
        public int Id { get; set; }

        public int SendingLocationId { get; set; }

        public int ReceivingLocationId { get; set; }

        public int? ViaLocationId { get; set; }

        public string SendingLocation { get; set; }

        public string ReceivingLocation { get; set; }

        public string ViaLocation { get; set; }

        public string Comments { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedById { get; set; }

        public string CreatedBy { get; set; }

        public string ReferenceNumber { get; set; }

        public decimal? Total { get; set; }
    }
}