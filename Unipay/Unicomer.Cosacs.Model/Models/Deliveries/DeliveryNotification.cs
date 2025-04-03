namespace Unicomer.Cosacs.Model.Models.Deliveries
{
    public class DeliveryNotification
    {
        public string CountryIsoCode { get; set; }
        public string CreditCustomerId { get; set; }
        public string LineOfCreditId { get; set; }
        public string CreditAccountId { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string Status { get; set; }
    }
}
