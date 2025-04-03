using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model
{
    public class UserTransactionInputModel
    {
        public String CustId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

    }

    public class TransactionDetails
    {
        //public string CustomerName { get; set; }
        public string id { get; set; }
        public string concept { get; set; }//StoreName
        public Int64 date { get; set; } //DateofTransaction
        public decimal amount { get; set; }//TotalAmount
        //public decimal AmountDue { get; set; }//AmountDue
        public string type { get; set; }//Accttype
    }

    public class UserTransactions
    {
        public List<TransactionDetails> transactions { get; set; }
        public int totalElements { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int totalPages { get; set; }
        public string sort { get; set; }
        public bool firstPage { get; set; }
        public bool lastPage { get; set; }
        public int numberOfElements { get; set; }
    }

    //public class UserAccountsModel
    //{
    //    public List<ListUserAccounts> listUserAccounts { get; set; }
    //    public string CustId { get; set; }
    //    public string CustomerName { get; set; }
    //}
    //public class ListUserAccounts
    //{
    //    public string accountNumber { get; set; }
    //    public string outstandingBalance { get; set; }
    //    public DateTime dueDate { get; set; }
    //    public string creditLimit { get; set; }
    //    public string creditAvailable { get; set; }
    //    public string amountPaid { get; set; }
    //    public string MonthlyInstalmentAmount { get; set; }

    //}
}
