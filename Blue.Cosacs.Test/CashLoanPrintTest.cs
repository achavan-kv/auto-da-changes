using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using STL.DAL.Licensing;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Threading;
using System.Security.Cryptography;
using System.Linq;
using BBSL.Libraries.Printing.PrintDocuments;

namespace Blue.Cosacs.Test
{
    [TestFixture]
    public class PrintTest
    {
       

        [TestFixtureSetUp]
        public void SetUp()
        {
           
        }

        [Test]
        public void PrintCashLoanReceipt()
        {
            BBSL.Libraries.Printing.PrintDocuments.CashLoanReceipt.TitleCustomerName = "Customer Name:";
            BBSL.Libraries.Printing.PrintDocuments.CashLoanReceipt.TitleAccountNo = "Account Number:";
            BBSL.Libraries.Printing.PrintDocuments.CashLoanReceipt.TitleTransactionDate = "Transaction Date:";
            BBSL.Libraries.Printing.PrintDocuments.CashLoanReceipt.TitleType = "Type:";
            BBSL.Libraries.Printing.PrintDocuments.CashLoanReceipt.TitleAmountDisbursed = "Amount Disbursed:";
            BBSL.Libraries.Printing.PrintDocuments.CashLoanReceipt.TitleTransactionNo = "Transaction No:";
            BBSL.Libraries.Printing.PrintDocuments.CashLoanReceipt.TitleAccountBalance = "Balance:";
            BBSL.Libraries.Printing.PrintDocuments.CashLoanReceipt.TitleFirstPaymentDate = "First Payment Date";

            var p = new BBSL.Libraries.Printing.PrintDocuments.CashLoanReceipt();

            p.CustomerName = new BBSL.Libraries.Printing.PrintDataWrapper<string>("Ilyas Parker", true);
            p.AccountNo = new BBSL.Libraries.Printing.PrintDataWrapper<string>("000000000000",true);
            p.TransactionDate = new BBSL.Libraries.Printing.PrintDataWrapper<DateTime> (DateTime.Today,true );
            p.Type = new BBSL.Libraries.Printing.PrintDataWrapper<string> ("CLD",true );
            p.AmountDisbursed = new BBSL.Libraries.Printing.PrintDataWrapper<decimal?> (1900,true );
            p.TransactionNo = new BBSL.Libraries.Printing.PrintDataWrapper<string>( "001",true );
            p.AccountBalance = new BBSL.Libraries.Printing.PrintDataWrapper<decimal?> (500,true ); ;
            p.FirstPaymentDate = new BBSL.Libraries.Printing.PrintDataWrapper<DateTime> (DateTime.Today ,true);

            //p.Print();
        }

      
    }
}
