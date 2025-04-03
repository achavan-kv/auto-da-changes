using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model
{
    public class VendorReturnModel
    {
        // public string ProductCode { get; set; }
        public string resourceType { get; set; }    
        public string source { get; set; }
        public string externalVendorReturnId { get; set; }
        public string externalGRNId { get; set; }
        public Int32? createdById { get; set; }
        public string referenceNumber { get; set; }
        [DataType(DataType.Date)]
        public string createdDate { get; set; }
        //[DataType(DataType.Date)]
        public string approvedDate { get; set; }
        public Int32? approvedById { get; set; }
        //public Int32? GoodsReceiptId { get; set; }
        //public Int32? Id { get; set; }
        //public string ReceiptType { get; set; }

        public string vendorDeliveryNumber { get; set; }
        public string vendorInvoiceNumber { get; set; }
        public string externalPONumber { get; set; }

        public List<VendorReturnList> VendorReturnList { get; set; }
    }

    public class VendorReturnList
    {
        public string productType { get; set; }
        [Required]
        public string externalItemNo { get; set; }
        public string description { get; set; }
        public string comments { get; set; }
        public int quantityReturned { get; set; }
        public decimal unitLandedCost { get; set; }

        //public string ProductCode { get; set; }
        //public double LineLandedCost { get; set; }
    }
}
