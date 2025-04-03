using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model
{
    public class GRN
    {
        public string resourceType { get; set; }
        public string source { get; set; }
        public string externalGRNId { get; set; }
        public int locationId { get; set; }
        public int receivedById { get; set; }
        public string vendorInvoiceNumber { get; set; }
        public string vendorInvoiceDate { get; set; }
        public int purchaseOrderId { get; set; }
        public string dateReceived { get; set; }
        public string comments { get; set; }
        public List<grnItems> grnItems { get; set; }
        public string VendorDeliveryNumber { get; set; }

    }
    public class grnItems
    {
        public string productType { get; set; }
        public string externalItemNo { get; set; }
        public string description { get; set; }
        public int quantityReceived { get; set; }
        public int quantityBackOrdered { get; set; }
        public int quantityCancelled { get; set; }
        public string reasonForCancellation { get; set; }
        public decimal lastLandedCost { get; set; }

    }
}
