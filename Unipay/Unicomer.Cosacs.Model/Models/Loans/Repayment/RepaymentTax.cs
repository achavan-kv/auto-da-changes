namespace Unicomer.Cosacs.Model.Models.Loans.Repayment
{
    public class RepaymentTax
    {
        public decimal TaxOnInterestAmount { get; set; }
        public decimal TaxOnInterestFromArrearsAmount { get; set; }
        public decimal DeferredTaxOnInterestAmount { get; set; }
        public decimal TaxOnFeesAmount { get; set; }
        public decimal TaxOnPenaltyAmount { get; set; }
    }
}
