using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model
{
    public class PurchaseOrderModel
    {
        public string PurchaseOrderId { get; set; }
        public string VendorId { get; set; }
        public string Vendor { get; set; }
        [DataType(DataType.Date)]
        public string ReceivingLocationId { get; set; }
        public string Currency { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        [DataType(DataType.Date)]
        public string CreatedDate { get; set; }
        public string CreatedById { get; set; }

        public string PaymentTerms { get; set; }
        public string OriginSystem { get; set; }

        public string CommissionChargeFlag { get; set; }
        public string CommissionPercentage { get; set; }
        [DataType(DataType.Date)]
        public string ExpiredDate { get; set; }

        public List<PurchaseOrderItems> PurchaseOrderItem { get; set; }

    }

    public class PurchaseOrderItems
    {
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public string RequestedDeliveryDate { get; set; }
        public string Quantity { get; set; }
        public string VendorUnitCost { get; set; }
        public string VendorLineCost { get; set; }
        public string Comments { get; set; }
    }
}
