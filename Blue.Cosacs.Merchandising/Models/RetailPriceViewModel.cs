namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class RetailPriceViewModel
    {
        [ReadOnly(true)]
        public int? Id { get; set; }

        public string Fascia { get; set; }

        [ReadOnly(true)]
        public string Location { get; set; }

        public int? LocationId { get; set; }

        public int ProductId { get; set; }

        public DateTime EffectiveDate { get; set; }

        [Required]
        public decimal TaxRate { get; set; }

        [Required]
        public decimal RegularPrice { get; set; }

        [Required]
        public decimal CashPrice { get; set; }

        [Required]
        public decimal DutyFreePrice { get; set; }
    }
}