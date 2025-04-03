namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class StockAllocationCreateModel
    {
        public StockAllocationCreateModel()
        {
            this.Products = new List<StockAllocationProductCreateModel>();
        }

        public int? Id { get; set; }

        [Range(1, int.MaxValue)]
        [Required]
        public int WarehouseLocationId { get; set; } 

        [StringLength(200)]
        public string Comments { get; set; }

        public List<StockAllocationProductCreateModel> Products { get; set; }
    }
}
