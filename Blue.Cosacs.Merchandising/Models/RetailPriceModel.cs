namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class RetailPriceAuditModel
    {
        public int Id { get; set; }

        public string Fascia { get; set; }

        public string Location { get; set; }

        public int? LocationId { get; set; }

        public int ProductId { get; set; }

        public string EffectiveDate { get; set; }

        public decimal TaxRate { get; set; }

        public string ExclusiveRegularPrice { get; set; }

        public string ExclusiveCashPrice { get; set; }

        public string ExclusiveDutyFreePrice { get; set; }

        public string InclusiveRegularPrice { get; set; }

        public string InclusiveCashPrice { get; set; }

        public string InclusiveDutyFreePrice { get; set; }
    }
}