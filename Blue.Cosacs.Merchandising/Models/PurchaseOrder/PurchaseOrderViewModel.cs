namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PurchaseOrderViewModel
    {
        public PurchaseOrderViewModel()
        {
            Products = new List<PurchaseOrderProductViewModel>();
            ReferenceNumbers = new List<StringKeyValue>();
            this.ResponseMsg = "";
        }

        public int Id { get; set; }
        public string Vendor { get; set; }

        public string PaymentTerms { get; set; }

        public int VendorId { get; set; }

        public string ReceivingLocation { get; set; }

        public int ReceivingLocationId { get; set; }

        public DateTime RequestedDeliveryDate { get; set; }

        public List<StringKeyValue> ReferenceNumbers { get; set; }

        public string Currency { get; set; }

        public string Comments { get; set; }

        public string Status { get; set; }

        public DateTime? OriginalPrint { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime CreatedDateLocal
        {
            get
            {
                return CreatedDate.ToLocalTime();
            }
        }

        public int CreatedById { get; set; }

        public string CreatedBy { get; set; }

        public string FormattedTotalCost { get; set; }
        
        public string OriginSystem { get; set; }
        
        public string CorporatePoNumber { get; set; }

        public DateTime? ShipDate { get; set; }

        public string ShipVia { get; set; }

        public string PortOfLoading { get; set; }

        public List<PurchaseOrderAttribute> Attributes { get; set; }

        public List<PurchaseOrderProductViewModel> Products { get; set; }

        public decimal Total
        {
            get
            {
                return Products.Sum(p => p.QuantityOrdered * p.UnitCost);
            }
        }

        public string Summary
        {
            get
            {
                return string.Format(
                    "#{0}, Vendor: {1}, Total: {2}, Ref: {3}",
                    Id,
                    Vendor,
                    Total.ToString("C"),
                    string.Join(", ", ReferenceNumbers.Select(r => r.Value)));
            }
        }

        //Code Added By Abhijeet for Ashley PO Cancel Reason 
        public string CancelReason { get; set; }
        public string ResponseMsg { get; set; }
    }
}