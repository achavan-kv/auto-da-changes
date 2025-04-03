namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class StockTransferCreateModel
    {
        public StockTransferCreateModel()
        {
            this.Products = new List<StockTransferProductCreateModel>();
        }
     
        [Required]
        public int? Id { get; set; }

        [Range(1, int.MaxValue)]
        [Required]
        public int SendingLocationId { get; set; }

        [Range(1, int.MaxValue)]
        [Required]
        public int ReceivingLocationId { get; set; }
        
        public int? ViaLocationId { get; set; }

        [StringLength(200)]
        public string Comments { get; set; }

        [StringLength(200)]
        public string ReferenceNumber { get; set; }

        public List<StockTransferProductCreateModel> Products { get; set; }
    }
}
