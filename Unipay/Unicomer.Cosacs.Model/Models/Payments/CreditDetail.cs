using System.Collections.Generic;

namespace Unicomer.Cosacs.Model.Models.Payments
{
    public class CreditDetail
    {
        public string TypeCreditProduct { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal PurchaseAmountTax { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal DepositAmount { get; set; }
        public List<Fee> Fee { get; set; }
        
    }
}
