using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Sales.Models
{
    public class ReceiptReprintDto
    {
        public int InvoiceNo { get; set; }
        public string AgreementInvoiceNumber { get; set; }
        public string OriginalAgreementInvoiceNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ItemId { get; set; }
        public int? ParentId { get; set; }
        public string ItemNo { get; set; }
        public string ItemDescription { get; set; }
        public string PosDescription { get; set; }
        public short Branch { get; set; }
        public short Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? Discount { get; set; }
        public string WarrantyContractNo { get; set; }
        public byte? WarrantyLength { get; set; }
        public string OriginalOrderId { get; set; }
        public int Count { get; set; }
        public bool IsFreeWarranty { get; set; }
    }
}
