using System;

namespace Unicomer.Cosacs.Model.Types
{
    public class AccountDueDetailType
    {
        public DateTime Duedate { get; set; }
        public decimal PrincipalDue { get; set; }
        public decimal InterestDue { get; set; }
        public decimal FeesDue { get; set; }
        public decimal TaxDue { get; set; }
        public decimal PenaltyDue { get; set; }
        public decimal PrincipalPaid { get; set; }
        public decimal InterestPaid { get; set; }
        public decimal FeesPaid { get; set; }
        public decimal TaxPaid { get; set; }
        public decimal PenaltyPaid { get; set; }
        public decimal PrincipalExpected { get; set; }
        public decimal InterestExpected { get; set; }
        public decimal FeesExpected { get; set; }
        public decimal TaxExpected { get; set; }
        public decimal PenaltyExpected { get; set; }
        public string State { get; set; }
    }
}
