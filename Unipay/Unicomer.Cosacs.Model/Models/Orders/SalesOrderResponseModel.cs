namespace Unicomer.Cosacs.Model.Models.Orders
{
    public class SalesOrderResponseModel
    {
        public string CountryIsoCode { get; set; }
        public string CreditCustomerId { get; set; }
        public string LineOfCreditId { get; set; }
        public string CreditAccountId { get; set; }
        public string SalesOrderId { get; set; }
        public string InvoiceNumber { get; set; }
        public string Status { get; set; }
    }
}
