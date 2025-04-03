using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Blue.Cosacs.Shared
{
    public class InstResult
    {
        public int? InstNo { get; set; }
        public DateTime? InstDate { get; set; }
        public string Status { get; set; }
        public string FormattedStatus { get; set; }
        public string AcctNo { get; set; }
        public int AgreementNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public string DeliveryAddress1 { get; set; }
        public string DeliveryAddress2 { get; set; }
        public string DeliveryAddress3 { get; set; }
        public string PhoneHome { get; set; }
        public string PhoneWork { get; set; }
        public DateTime? DeliveryDate { get; set; }
        //public bool HasWarranty { get; set; }
        public bool? HasWarranty { get; set; }      //6.5
        public int ItemId { get; set; }
        public string ItemNo { get; set; }
        public string CourtsCode { get; set; }
        public string ProductDescription1 { get; set; }
        public string ProductDescription2 { get; set; }
        public double Quantity { get; set; }
        public decimal InstValue { get; set; }
        public short StockLocation { get; set; }
        public string SupplierName { get; set; }
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? AuthorisedDate { get; set; }
        public string DeliveryStatus { get; set; }
        public string PrimaryChargeCode { get; set; }
        public byte[] ResolutionRowVersion { get; set; }
        public int TechnicianId { get; set; }                   //IP - 19/07/11 - #4303
        public string TechnicianName { get; set; }              //IP - 19/07/11 - #4303
        public string HomeTelNo { get; set; }                   //IP - 19/07/11 - #4312
        public string WorkTelNo { get; set; }                   //IP - 19/07/11 - #4312
    }
}
