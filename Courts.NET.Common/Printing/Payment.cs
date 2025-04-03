using System;
using System.Collections.Generic;
using System.Text;

namespace BBSL.Libraries.Printing
{
    public class Transaction : IComparable<Transaction>
    {
        public string AccountNo { get; set; }
        public string ReferenceNo { get; set; }
        public decimal AgreementTotal { get; set; }
        public string PaymentType { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public decimal RemainingBalance { get; set; }

        #region IComparable<Payment> Members

        public int CompareTo(Transaction other)
        {
            if (other.Date == this.Date)
                return 0;
            else if (other.Date < this.Date)
                return -1;
            else
                return 1;
        }

        #endregion
    }
}
