namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class StockMovementQueryModel
    {
        public string Division { get; set; }

        public string Department { get; set; }

        public string Class { get; set; }

        public string Sku { get; set; }

        public DateTime? CreatedFrom { get; set; }

        public DateTime? CreatedTo { get; set; }

        public string Fascia { get; set; }

        public int? LocationId { get; set; }

        public int? ProductId { get; set; }
    }
}
