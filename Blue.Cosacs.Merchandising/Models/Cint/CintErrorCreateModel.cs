namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CintErrorCreateModel
    {
        [Required]
        public string ProductCode { get; set; }

        [Required]
        public string PrimaryReference { get; set; }

        [Required]
        public string SecondaryReference { get; set; }

        [Required]
        public string ReferenceType { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string SaleLocation { get; set; }

        [Required]
        public string StockLocation { get; set; }

        [Required]
        public string ErrorMessage { get; set; }

        [Required]
        public int? RunNo { get; set; }

        [Required]
        public int MessageId { get; set; }

        public bool HasPassed { get; set; }

        public DateTime PassOn { get; set; }

        public bool IsBulkQueue { get; set; }
    }
}