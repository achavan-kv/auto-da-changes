namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class GoodsOnLoanQueryModel
    {
        public int? Id { get; set; }

        public int? StockLocationId { get; set; }

        public DateTime? ExpectedCollectionDate { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public DateTime? CreatedFrom { get; set; }

        public DateTime? CreatedTo { get; set; }

        public string Sku { get; set; }

        public string ReferenceNumber { get; set; }
    }
}
