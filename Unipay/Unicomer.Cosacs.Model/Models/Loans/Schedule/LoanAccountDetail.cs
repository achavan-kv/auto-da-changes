namespace Unicomer.Cosacs.Model.Models.Loans.Schedule
{
    public class LoanAccountDetail
    {
        public int DaysInArrears { get; set; }
        public string AccountState { get; set; }
        public string ProductType { get; set; }
        public string LoanAccountId { get; set; }
        public decimal TotalAmountExpected { get; set; }
        public decimal TotalAmountDue { get; set; }
        public decimal TotalAmountPaid { get; set; }
    }
}
