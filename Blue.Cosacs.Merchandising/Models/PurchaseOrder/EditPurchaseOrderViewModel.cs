namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EditPurchaseOrderViewModel
    {
        public EditPurchaseOrderViewModel()
        {
            ReferenceNumbers = new List<StringKeyValue>();
            Products = new List<EditPurchaseOrderProductViewModel>();
        }

        [Required]
        public int Id { get; set; }
        public int VendorId { get; set; }
        public List<StringKeyValue> ReferenceNumbers { get; set; }

        public string Comments { get; set; }

        public List<EditPurchaseOrderProductViewModel> Products { get; set; }
    }
}