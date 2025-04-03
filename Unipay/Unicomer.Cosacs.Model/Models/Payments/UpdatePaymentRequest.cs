using System;

namespace Unicomer.Cosacs.Model.Models.Payments
{
    public class UpdatePaymentRequest
    {
        public string CountryIsoCode { get; set; }
        public string InvoiceNumber { get; set; }
        public string CreditCustomerId { get; set; }
        public string LineOfCreditId { get; set; }
        public string CreditAccountId { get; set; }
        public string SalesCustomerId { get; set; }
        public decimal TotalLoan { get; set; }
        public int NumberInstallments { get; set; }
        public decimal InstallmentValue { get; set; }
        public decimal LastInstallmentValue { get; set; }
        public DateTime PaymentStartDate { get; set; }
        public decimal AnnualRate { get; set; }
        public InvoiceFee Fee { get; set; }
    }
}
