namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Blue.Cosacs.Merchandising.Validators;

    public class StockCountCreateModel
    {
        public StockCountCreateModel()
        {
            Hierarchy = new Dictionary<int, string>();
        }

        [Range(1, int.MaxValue)]
        public int? LocationId { get; set; }

        public string Fascia { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        [FutureDate]
        [DataType(DataType.Date)]
        public DateTime? CountDate { get; set; }

        public Dictionary<int, string> Hierarchy { get; set; }
    }
}
