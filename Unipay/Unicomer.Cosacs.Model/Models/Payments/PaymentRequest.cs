using System.Collections.Generic;

namespace Unicomer.Cosacs.Model.Models.Payments
{
    public class PaymentRequest
    {
        public string ReferenceinvoiceId { get; set; }
        public string StoreId { get; set; }
        public string SalePerson { get; set; }
        public string InvoiceDate { get; set; }
        public string Delivery { get; set; }
        public string SalesCustomerId { get; set; }
        public string CreditCustomerId { get; set; }
        public string LineOfCreditId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public ReceiptData ReceiptData { get; set; }
       
    }
}
