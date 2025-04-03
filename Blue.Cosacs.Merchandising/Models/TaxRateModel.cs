using System;

namespace Blue.Cosacs.Merchandising.Models
{
    using System.ComponentModel.DataAnnotations;

    using Blue.Cosacs.Merchandising.Validators;

    public class TaxRateModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Rate { get; set; }

        [Required, FutureDate]
        public DateTime EffectiveDate { get; set; }

        public int? ProductId { get; set; }
    }
}
