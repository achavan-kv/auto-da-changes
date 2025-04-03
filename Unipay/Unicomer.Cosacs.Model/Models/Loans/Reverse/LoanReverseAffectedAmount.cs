namespace Unicomer.Cosacs.Model.Models.Loans.Reverse
{
    public class LoanReverseAffectedAmount
    {
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal InterestFromArrearsAmount { get; set; }
        public decimal DeferredInterestAmount { get; set; }
        public decimal FeesAmount { get; set; }
        public decimal PenaltyAmount { get; set; }
        public decimal FundersInterestAmount { get; set; }
        public decimal OrganizationCommissionAmount { get; set; }
    }
}
