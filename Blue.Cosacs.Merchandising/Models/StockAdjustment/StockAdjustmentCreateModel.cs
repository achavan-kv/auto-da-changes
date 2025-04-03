namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class StockAdjustmentCreateModel
    {
        public StockAdjustmentCreateModel()
        {
            Products = new List<StockAdjustmentProductCreateModel>();
        }
     
        [Required]
        public int? Id { get; set; }

        [Range(1, int.MaxValue)]
        [Required]
        public int? LocationId { get; set; }

        [Range(1, int.MaxValue)]
        [Required]
        public int? PrimaryReasonId { get; set; }

        [Range(1, int.MaxValue)]
        [Required]
        public int? SecondaryReasonId { get; set; }

        public string Comments { get; set; }

        public string ReferenceNumber { get; set; }

        public List<StockAdjustmentProductCreateModel> Products { get; set; }
    }
}
