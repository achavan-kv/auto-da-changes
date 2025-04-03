using System;

namespace Unicomer.Cosacs.Model.Models.Loans.Schedule
{
    public class LoanScheduleInstallment
    {
        public DateTime DueDate { get; set; }
        public string EncodedKey { get; set; }
        public Fee Fee { get; set; }
        public Interest Interest { get; set; }
        public bool IsPaymentHoliday { get; set; }
        public string Number { get; set; }
        public string ParentAccountKey { get; set; }
        public Penalty Penalty { get; set; }
        public Principal Principal { get; set; }
        public string State { get; set; }
        public int DaysInArrears { get; set; }
        public decimal AmountExpected { get; set; }
        public decimal AmountDue { get; set; }
        public decimal AmountPaid { get; set; }
    }
}
