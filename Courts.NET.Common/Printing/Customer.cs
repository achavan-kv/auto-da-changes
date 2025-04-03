using System;
using System.Collections.Generic;
using System.Text;

namespace BBSL.Libraries.Printing
{
    public class BBSCustomer
    {
        List<Account> accounts = new List<Account>();

        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Address5 { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal AvailableSpend { get; set; }
        public List<Account> Accounts 
        { 
            get { return accounts; } 
            set 
            {
                if (value == null)
                    accounts = new List<Account>();
                else
                    accounts = value; 
            }
        }

        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(Name)
                    && string.IsNullOrEmpty(Address1)
                    && string.IsNullOrEmpty(Address2)
                    && string.IsNullOrEmpty(Address3)
                    && string.IsNullOrEmpty(Address4)
                    && string.IsNullOrEmpty(Address5)
                    && CreditLimit == default(decimal)
                    && AvailableSpend == default(decimal)
                    && Accounts.Count == 0;
            }
        }

        public int TotalTransactions
        {
            get
            {
                int total = 0;
                foreach (Account account in accounts)
                    total += account.Transactions.Count;
                return total;
            }
        }
    }
}
