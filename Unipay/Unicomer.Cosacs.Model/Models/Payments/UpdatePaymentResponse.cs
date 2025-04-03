namespace Unicomer.Cosacs.Model.Models.Payments
{
    public class UpdatePaymentResponse
    {
        public string CountryIsoCode { get; set; }
        public string CreditAccountId { get; set; }
        public string RefInvoiceNumber { get; set; }
        public string Status { get; set; }
    }
}
