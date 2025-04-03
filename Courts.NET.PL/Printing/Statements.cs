using System;
using System.Collections.Generic;
using System.Text;
using BBSL.Libraries.Printing;
using BBSL.Libraries.Printing.PrintDocuments;
using STL.Common;
using System.Threading;
using System.Windows.Forms;

namespace STL.PL
{
    internal class Statements
    {
        static Statements()
        {
        }

        public event Action<String> AccountNotFoundEvent;
        public event Action<String> CustomerNotFoundEvent;
        public event Action<Exception> ExceptionEvent;

        /// <summary>
        /// This event will be fired when over 50 transactions are returned for printing
        /// </summary>
        public event Action<int> LotsOfTransactionsEvent;

        public bool Print { get; set; }

        /// <summary>
        /// Prints a statement for the last <paramref name="noOfTransactions"/> for an account
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="noOfTransactions"></param>
        /// <param name="AccountManager"></param>
        /// <param name="Country"></param>
        /// <param name="showPreview">True to show the print preview before printing</param>
        public void PrintStatementForAccount
            (
                string accountNo,
                int noOfTransactions,
                STL.PL.WS2.WAccountManager AccountManager,
                CountryParameterCollection Country,
                bool showPreview
            )
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(
                delegate
                {
                    try
                    {
                        Print = true;
                        BBSCustomer customer = AccountManager.GetAccountStatementLastTransactionsLocal(accountNo, noOfTransactions);

                        if (customer == null)
                        {
                            if (AccountNotFoundEvent != null)
                                AccountNotFoundEvent("");
                            return;
                        }

                        if (customer.TotalTransactions > 50)
                        {
                            if (LotsOfTransactionsEvent != null)
                                LotsOfTransactionsEvent(customer.TotalTransactions);
                        }

                        if (Print)
                        {
                            MiniStatement miniStatement = new MiniStatement();
                            MiniStatement.BusinessTaxNo.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.StatementDisplayVATNumber);
                            
                            miniStatement.Title.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.StatementTitle);
                            miniStatement.DocumentName = miniStatement.Title.Value;
                            miniStatement.AvailableSpend.Value = customer.AvailableSpend;
                            miniStatement.AvailableSpend.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.PrintAvailableSpendOnMiniStatement);
                            miniStatement.CreditLimit.Value = customer.CreditLimit;
                            miniStatement.CustomerAddress1.Value = customer.Address1;
                            miniStatement.CustomerAddress2.Value = customer.Address2;
                            miniStatement.CustomerAddress3.Value = customer.Address3;
                            miniStatement.CustomerAddress4.Value = customer.Address4;
                            miniStatement.CustomerName.Value = customer.Name;
                            miniStatement.Footer.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.StatementFooter);

                            miniStatement.DateFrom.ShouldBePrinted = miniStatement.DateTo.ShouldBePrinted = false;
                            miniStatement.DateTo.ShouldBePrinted = miniStatement.DateTo.ShouldBePrinted = false;

                            miniStatement.Accounts.Value = customer.Accounts;

                            if (showPreview)
                            {
                                PrintPreviewDialog preview = new PrintPreviewDialog();
                                preview.Document = miniStatement;
                                preview.Width = (int)(Screen.PrimaryScreen.Bounds.Width * 0.50);
                                preview.Height = (int)(Screen.PrimaryScreen.Bounds.Height * 0.75);
                                preview.PrintPreviewControl.Zoom = 1.0;
                                preview.ShowDialog();
                            }
                            else
                                miniStatement.Print();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ExceptionEvent != null)
                            ExceptionEvent(ex);
                    }
                }));
        }

        public void PrintStatementForAccount
            (
                string accountNo,
                DateTime dateFrom,
                DateTime dateTo,
                STL.PL.WS2.WAccountManager AccountManager,
                CountryParameterCollection Country,
                bool showPreview
            )
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(
                delegate
                {
                    try
                    {
                        Print = true;
                        BBSCustomer customer = AccountManager.GetAccountStatementLocal(accountNo, dateFrom, dateTo);

                        if (customer == null)
                        {
                            if (AccountNotFoundEvent != null)
                                AccountNotFoundEvent("");
                            return;
                        }

                        if (customer.TotalTransactions > 50)
                        {
                            if (LotsOfTransactionsEvent != null)
                                LotsOfTransactionsEvent(customer.TotalTransactions);
                        }

                        if (Print)
                        {
                            MiniStatement miniStatement = new MiniStatement();

                            MiniStatement.BusinessTaxNo.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.StatementDisplayVATNumber);

                            miniStatement.Title.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.StatementTitle);
                            miniStatement.DocumentName = miniStatement.Title.Value;
                            miniStatement.AvailableSpend.Value = customer.AvailableSpend;
                            miniStatement.AvailableSpend.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.StatementDisplayAvailableSpend);
                            miniStatement.CreditLimit.Value = customer.CreditLimit;
                            miniStatement.CustomerAddress1.Value = customer.Address1;
                            miniStatement.CustomerAddress2.Value = customer.Address2;
                            miniStatement.CustomerAddress3.Value = customer.Address3;
                            miniStatement.CustomerAddress4.Value = customer.Address4;
                            miniStatement.CustomerName.Value = customer.Name;
                            miniStatement.Footer.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.StatementFooter);

                            if (dateFrom == new DateTime(1990, 1, 1))
                                miniStatement.DateFrom.ShouldBePrinted = miniStatement.DateTo.ShouldBePrinted = false;
                            else
                            {
                                miniStatement.DateFrom.Value = dateFrom;
                                miniStatement.DateTo.Value = dateTo;
                            }

                            miniStatement.Accounts.Value = customer.Accounts;

                            if (showPreview)
                            {
                                PrintPreviewDialog preview = new PrintPreviewDialog();
                                preview.Document = miniStatement;
                                preview.Width = (int)(Screen.PrimaryScreen.Bounds.Width * 0.50);
                                preview.Height = (int)(Screen.PrimaryScreen.Bounds.Height * 0.75);
                                preview.PrintPreviewControl.Zoom = 1.0;
                                preview.ShowDialog();
                            }
                            else
                                miniStatement.Print();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ExceptionEvent != null)
                            ExceptionEvent(ex);
                    }
                }));
        }

        public void PrintStatementForCustomer
            (
                string customerID,
                DateTime dateFrom, 
                DateTime dateTo, 
                STL.PL.WS2.WAccountManager AccountManager, 
                CountryParameterCollection Country, 
                bool onlyHolderAccounts,
                bool showPreview
            )
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(
                delegate
                {
                    try
                    {
                        Print = true;
                        BBSCustomer customer = AccountManager.GetCustomerStatementLocal(customerID, dateFrom, dateTo, onlyHolderAccounts);

                        if (customer == null)
                        {
                            if (CustomerNotFoundEvent != null)
                                CustomerNotFoundEvent("");
                            return;
                        }

                        if (customer.TotalTransactions > 50)
                        {
                            if (LotsOfTransactionsEvent != null)
                                LotsOfTransactionsEvent(customer.TotalTransactions);
                        }

                        if (Print)
                        {
                            MiniStatement miniStatement = new MiniStatement();
                            MiniStatement.BusinessTaxNo.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.StatementDisplayVATNumber);

                            miniStatement.Title.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.StatementTitle);
                            miniStatement.DocumentName = miniStatement.Title.Value;
                            miniStatement.AvailableSpend.Value = customer.AvailableSpend;
                            miniStatement.AvailableSpend.ShouldBePrinted = Country.GetCountryParameterValue<bool>(CountryParameterNames.Printing.StatementDisplayAvailableSpend);
                            miniStatement.CreditLimit.Value = customer.CreditLimit;
                            miniStatement.CustomerAddress1.Value = customer.Address1;
                            miniStatement.CustomerAddress2.Value = customer.Address2;
                            miniStatement.CustomerAddress3.Value = customer.Address3;
                            miniStatement.CustomerAddress4.Value = customer.Address4;
                            miniStatement.CustomerName.Value = customer.Name;
                            miniStatement.Footer.Value = Country.GetCountryParameterValue<string>(CountryParameterNames.Printing.StatementFooter);

                            if (dateFrom == new DateTime(1990, 1, 1))
                                miniStatement.DateFrom.ShouldBePrinted = miniStatement.DateTo.ShouldBePrinted = false;
                            else
                            {
                                miniStatement.DateFrom.Value = dateFrom;
                                miniStatement.DateTo.Value = dateTo;
                            }

                            miniStatement.Accounts.Value = customer.Accounts;

                            if (showPreview)
                            {
                                PrintPreviewDialog preview = new PrintPreviewDialog();
                                preview.Document = miniStatement;
                                preview.Width = (int)(Screen.PrimaryScreen.Bounds.Width * 0.50);
                                preview.Height = (int)(Screen.PrimaryScreen.Bounds.Height * 0.75);
                                preview.PrintPreviewControl.Zoom = 1.0;
                                preview.ShowDialog();
                            }
                            else
                                miniStatement.Print();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ExceptionEvent != null)
                            ExceptionEvent(ex);
                    }
                }));
        }
    }
}


