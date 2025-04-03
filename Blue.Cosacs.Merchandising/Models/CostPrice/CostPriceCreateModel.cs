using System;

namespace Blue.Cosacs.Merchandising.Models
{
    using Blue.Cosacs.Merchandising.Helpers;
    using System.ComponentModel.DataAnnotations;

    public class CostPriceCreateModel
    {
        [Required]
        public int? ProductId { get; set; }

        [Required, Range(typeof(Decimal), "0.0", Constants.DecimalMax, ErrorMessage = "Please enter a valid supplier cost")]
        public decimal? SupplierCost { get; set; }

        [Required, Range(typeof(Decimal), "0.0", Constants.DecimalMax, ErrorMessage = "Please enter a valid landed cost")]
        public decimal? LastLandedCost { get; set; }

        public decimal AverageWeightedCost { get; set; }

        [Required]
        public string SupplierCurrency { get; set; }
    }
}
