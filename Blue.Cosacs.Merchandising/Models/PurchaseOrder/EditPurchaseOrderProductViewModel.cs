namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class EditPurchaseOrderProductViewModel
    {
        [Required]
        public int Id { get; set; }

        public DateTime? EstimatedDeliveryDate { get; set; }

        public decimal unitCost { get; set; }

        public decimal lineCost { get; set; }
    }
}