namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class GoodsOnLoanUpdateModel
    {
        [Required]
        public int? Id { get; set; }

        [Required]
        public DateTime ExpectedCollectionDate { get; set; }

        public string Comments { get; set; }
    }
}