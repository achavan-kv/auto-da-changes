namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Blue.Cosacs.Merchandising.Validators;

    public class StockCountStartViewModel
    {
        public StockCountStartViewModel()
        {
            Questions = new Dictionary<string, bool>();
        }

        [Range(1, int.MaxValue)]
        [Required]
        public int? StockCountId { get; set; }
       
        public DateTime? CountDate { get; set; }

        public string Location { get; set; }
        
        public string Type { get; set; }

        public Dictionary<string, bool> Questions { get; set; }

        public Dictionary<int, string> Hierarchy { get; set; }

        public int LocationId { get; set; }
    }
}
