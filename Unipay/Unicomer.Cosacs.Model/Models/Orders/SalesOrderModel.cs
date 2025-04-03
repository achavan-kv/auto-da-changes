using System;

namespace Unicomer.Cosacs.Model.Models.Orders
{
    public class SalesOrderModel
    {
        public string SalesOrderId { get; set; }
        public string StoreId { get; set; }
        public string SalePerson { get; set; }
        public string InvoiceDate { get; set; }
        public int Quantity { get; set; }
        public bool? Delivery { get; set; }
        public string SalesCustomerId { get; set; }
        public string DocumentNumber { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public string CreditCustomerId { get; set; }
        public string LineOfCreditId { get; set; }
        public ReceiptData ReceiptData { get; set; }
    }
}
