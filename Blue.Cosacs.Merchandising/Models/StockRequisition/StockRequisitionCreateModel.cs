namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class StockRequisitionCreateModel
    {
        public StockRequisitionCreateModel()
        {
            this.Products = new List<StockRequisitionProductCreateModel>();
        }

        public int? Id { get; set; }

        [Range(1, int.MaxValue)]
        [Required]
        public int ReceivingLocationId { get; set; }

        [Range(1, int.MaxValue)]
        [Required]
        public int WarehouseLocationId { get; set; } 

        public List<StockRequisitionProductCreateModel> Products { get; set; }
    }
}
