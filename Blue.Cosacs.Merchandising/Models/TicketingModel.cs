using System.Collections.Generic;

namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class TicketingModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductType { get; set; }
        public string SKU { get; set; }
        public string LongDescription { get; set; }
        public string POSDescription { get; set; }
        public int? LocationId { get; set; }

        public string LocationName { get; set; }
        public string Fascia { get; set; }
        public string BrandName { get; set; }
        public string VendorStyleLong { get; set; }
        public string Hierarchy { get; set; }
        public List<FieldSchema> Features { get; set; }
        public string EffectiveDate { get; set; }
        public decimal CurrentCashPrice { get; set; }
        public decimal CurrentRegularPrice { get; set; }
        public decimal TaxRate { get; set; }
        public int? SetId { get; set; }
        public string SetCode { get; set; }
        public string SetDescription { get; set; }
        public decimal NormalCashPrice { get; set; }
        public decimal NormalRegularPrice { get; set; }
        public decimal DutyFreePrice { get; set; }
        public List<ComponentView> Components { get; set; }
    }
}
