using System;
using System.Collections.Generic;
using System.Text;

namespace STL.BLL.OracleIntegration
{
    public class ARInvoiceHeader
    {
        
        public string TranType=" ";
        public string TranClass = " ";
        public DateTime TranDate;
        public DateTime GLDate;
        public DateTime DelDate;    //jec 29/10/08
        public int empeenosale;
        public string invoicereference = " ";
        public string CredInvRef = " ";        
        public int PayTerm;
        public string SalesPerson = " ";
        public string CustomerId = " ";
        public string AcctNo = " ";
        public int BillAdrRef;
        public int ShipAdrRef;
        public int BranchNo;
        public int RunNo;


    }
}

