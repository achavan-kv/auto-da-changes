using System;

namespace Blue.Cosacs.Sales.Models
{
    [Serializable]
    public class PaymentDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public byte PaymentMethodId { get; set; }
        public string PaymentMethod { get; set; }

        public decimal Amount { get; set; }
        public string Bank { get; set; }

        public string CardType { get; set; }
        public short? CardNo { get; set; }

        public string ChequeNo { get; set; }
        public string BankAccountNo { get; set; }

        public decimal? CurrencyRate { get; set; }
        public decimal? CurrencyAmount { get; set; }

        public long? StoreCardNo { get; set; }
        public string VoucherNo { get; set; }

        public string CurrencyCode { get; set; }
        public string VoucherIssuer { get; set; }

        public string VoucherIssuerCode { get; set; }
        public short TempPaymentId { get; set; }
    }
}