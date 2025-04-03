using System;
using System.Collections.Generic;

namespace Unicomer.Cosacs.Model.Models.Orders
{
    public class CreditDetail
    {
        public string CreditAccountId { get; set; }
        public string TypeCreditProduct { get; set; }
        public decimal TotalLoan { get; set; }
        public int NumberInstallments { get; set; }
        public decimal InstallmentValue { get; set; }
        public decimal LastInstallmentValue { get; set; }
        public DateTime PaymentStartDate { get; set; }
        public decimal AnnualRate { get; set; }
        public CreditFee Fee { get; set; }
        public DeliveryAddress DeliveryAddress { get; set; }
        public List<ProductDetail> ProductDetail { get; set; }
    }
}