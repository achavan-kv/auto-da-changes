namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class PurchaseOrderCreateModel
    {
        public PurchaseOrderCreateModel()
        {
            Attributes = new List<PurchaseOrderAttribute>();
            Products = new List<PurchaseOrderProductImportModel>();
            ReferenceNumbers = new List<StringKeyValue>();
        }

        [Required]
        public int? ReceivingLocationId { get; set; }

        [Required]
        public int? VendorId { get; set; }
        public string Vendor { get; set; }
        public string OriginSystem { get; set; }

        public string CorporatePoNumber { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ShipDate { get; set; }

        [Required]
        public DateTime? RequestedDeliveryDate { get; set; }

        public string CommissionChargeFlag { get; set; }

        public string CommissionPercentage { get; set; }

        public string Currency { get; set; }

        public string ShipVia { get; set; }

        public string PortOfLoading { get; set; }

        public string PaymentTerms { get; set; }

        public string Company { get; set; }

        public int? CreatedById { get; set; }

        public string CreatedBy { get; set; }

        public string Comments { get; set; }

        public List<PurchaseOrderAttribute> Attributes { get; set; }

        public List<PurchaseOrderProductImportModel> Products { get; set; }

        public List<StringKeyValue> ReferenceNumbers { get; set; }
    }
}