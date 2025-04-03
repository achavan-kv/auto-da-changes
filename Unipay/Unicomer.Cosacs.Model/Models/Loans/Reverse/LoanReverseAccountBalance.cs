namespace Unicomer.Cosacs.Model.Models.Loans.Reverse
{
    public class LoanReverseAccountBalance
    {
        public decimal TotalBalance { get; set; }
        public decimal AdvancePosition { get; set; }
        public decimal ArrearsPosition { get; set; }
        public decimal ExpectedPrincipalRedraw { get; set; }
        public decimal RedrawBalance { get; set; }
        public decimal PrincipalBalance { get; set; }
    }
}
