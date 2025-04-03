using System.Collections.Generic;
using System.Threading;

namespace STL.PL.WS2
{
    partial class Authentication : IAuthentication { }

    partial class WAccountManager
    {
        public WAccountManager(bool custom)
        {
            Setup();
        }

        public void Setup()
        {
            this.AuthenticationValue = this.Setup<Authentication>(timeout: 20000000); //changed to 20000 secs to allow rebate report to run
        }

        protected new object[] Invoke(string methodName, object[] parameters)
        {
            var result = base.Invoke(methodName, parameters);
            this.PostInvoke(AuthenticationValue);
            return result;
        }

        protected new void InvokeAsync(string methodName, object[] parameters, System.Threading.SendOrPostCallback callback)
        {
            base.InvokeAsync(methodName, parameters, (state) => { this.PostInvoke(AuthenticationValue); if (callback != null) callback(state); });
        }

        protected new void InvokeAsync(string methodName, object[] parameters, SendOrPostCallback callback, object userState)
        {
            base.InvokeAsync(methodName, parameters, (state) => { this.PostInvoke(AuthenticationValue); if (callback != null) callback(state); }, userState);
        }


        /// <summary>
        /// Calls GetAccountStatement(string accountNo, DateTime dateFrom, DateTime dateTo) and converts the return value
        /// to BBSL.Libraries.Printing.BBSCustomer
        /// </summary>
        public BBSL.Libraries.Printing.BBSCustomer GetAccountStatementLocal(string accountNo, System.DateTime dateFrom, System.DateTime dateTo)
        {
            return ConvertToLocalType(GetAccountStatement(accountNo, dateFrom, dateTo));
        }
        /// <summary>
        /// Calls GetCustomerStatement(string customerID, DateTime dateFrom, DateTime dateTo, bool onlyHolderAccounts) and converts the return value
        /// to BBSL.Libraries.Printing.BBSCustomer
        /// </summary>
        public BBSL.Libraries.Printing.BBSCustomer GetCustomerStatementLocal(string customerID, System.DateTime dateFrom, System.DateTime dateTo, bool onlyHolderAccounts)
        {
            return ConvertToLocalType(GetCustomerStatement(customerID, dateFrom, dateTo, onlyHolderAccounts));
        }
        /// <summary>
        /// Calls GetAccountStatementLastTransactions(string accountNo, int NoOfTransactions) and converts the return value
        /// to BBSL.Libraries.Printing.BBSCustomer
        /// </summary>
        public BBSL.Libraries.Printing.BBSCustomer GetAccountStatementLastTransactionsLocal(string accountNo, int NoOfTransactions)
        {
            return ConvertToLocalType(GetAccountStatementLastTransactions(accountNo, NoOfTransactions));
        }

        BBSL.Libraries.Printing.BBSCustomer ConvertToLocalType(STL.PL.WS2.BBSCustomer customer)
        {
            if (customer == null)
                return null;

            return new BBSL.Libraries.Printing.BBSCustomer
            {
                Name = customer.Name,
                Address1 = customer.Address1,
                Address2 = customer.Address2,
                Address3 = customer.Address3,
                Address4 = customer.Address4,
                Address5 = customer.Address5,
                Accounts = ConvertToLocalType(customer.Accounts),
                AvailableSpend = customer.AvailableSpend,
                CreditLimit = customer.CreditLimit
            };
        }
        List<BBSL.Libraries.Printing.Account> ConvertToLocalType(STL.PL.WS2.Account[] accounts)
        {
            List<BBSL.Libraries.Printing.Account> localAccounts = new List<BBSL.Libraries.Printing.Account>();
            foreach (STL.PL.WS2.Account account in accounts)
                localAccounts.Add(ConvertToLocalType(account));
            return localAccounts;
        }
        BBSL.Libraries.Printing.Account ConvertToLocalType(STL.PL.WS2.Account account)
        {
            if (account == null)
                return null;

            return new BBSL.Libraries.Printing.Account
            {
                AccountNo = account.AccountNo,
                AgreementTotal = account.AgreementTotal,
                Transactions = ConvertToLocalType(account.Transactions)
            };
        }
        List<BBSL.Libraries.Printing.Transaction> ConvertToLocalType(STL.PL.WS2.Transaction[] transactions)
        {
            List<BBSL.Libraries.Printing.Transaction> newTransactions = new List<BBSL.Libraries.Printing.Transaction>();
            foreach (STL.PL.WS2.Transaction transaction in transactions)
                newTransactions.Add(
                    new BBSL.Libraries.Printing.Transaction
                    {
                        AccountNo = transaction.AccountNo,
                        AgreementTotal = transaction.AgreementTotal,
                        Amount = transaction.Amount,
                        Date = transaction.Date,
                        PaymentType = transaction.PaymentType,
                        ReferenceNo = transaction.ReferenceNo,
                        RemainingBalance = transaction.RemainingBalance
                    });
            return newTransactions;
        }
    }
}
