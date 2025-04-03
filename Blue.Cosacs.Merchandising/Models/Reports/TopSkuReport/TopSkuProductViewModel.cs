namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class TopSkuProductViewModel
    {
        public TopSkuProductViewModel()
        {
            HierarchyTags = new List<string>();
        }

        public int Id { get; set; }

        public int ProductId { get; set; }

        public string Sku { get; set; }

        public string LongDescription { get; set; }

        public string LocationName { get; set; }

        public string Fascia { get; set; }

        public string BrandName { get; set; }

        public string Tags { get; set; }

        public int QuantityDelivered { get; set; }

        public decimal ValueDelivered { get; set; }

        public decimal NetValueDelivered { get; set; }

        public DateTime? TransactionDate { get; set; }

        public decimal Cost { get; set; }

        public decimal Margin { get; set; }

        public decimal Contribution { get; set; }

        public decimal CumulativeValueDelivered { get; set; }

        public decimal CumulativeNetValueDelivered { get; set; }

        public List<string> HierarchyTags { get; set; }
    }
}