using System;
using System.Collections.Generic;
using System.Text;

namespace BBSL.Libraries.Printing
{
    public class Account
    {
        List<Transaction>  transactions = new List<Transaction>();

        public string AccountNo { get; set; }
        public decimal AgreementTotal { get; set; }
        public List<Transaction> Transactions 
        {
            get 
            {
                return transactions; 
            }
            set
            {
                transactions = value;
            }
        }
    }
}
