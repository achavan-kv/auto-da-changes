namespace Blue.Cosacs.Merchandising.Models
{
    public class TopSkuExportModel
    {
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

        public decimal Cost { get; set; }

        public decimal Margin 
        { 
            get
            {
                return ValueDelivered - Cost;
            } 
        }

        public decimal TotalMargin
        {
            get
            {
                return Margin * QuantityDelivered;
            }
        }

        public decimal Contribution { get; set; }

        public decimal CumulativeValueDelivered { get; set; }

        public decimal CumulativeNetValueDelivered { get; set; }

        public string HierarchyTags { get; set; }
    }
}