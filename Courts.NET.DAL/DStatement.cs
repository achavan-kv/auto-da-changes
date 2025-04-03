using System;
using System.Collections.Generic;
using System.Text;
using BBSL.Libraries.Printing;
using System.Data;
using System.Data.SqlClient;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.FTransaction;

namespace STL.DAL
{
    public class DStatement : DALObject
    {
        BBSCustomer _GetCustomerStatement(string customerID, DateTime dateFrom, DateTime dateTo, bool onlyHolderAccounts)
        {
            BBSCustomer customer = new BBSCustomer();

            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                SqlParameter[] parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@CustID", SqlDbType.NVarChar, 12);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@Date", SqlDbType.DateTime);
                parmArray[1].Value = dateFrom;
                parmArray[2] = new SqlParameter("@onlyHolderAccounts", SqlDbType.Bit);
                parmArray[2].Value = onlyHolderAccounts;
                
                DataSet statementDataset = new DataSet();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = CreateCommand("GetCustomerStatement", parmArray, connection);
                sqlDA.Fill(statementDataset);

                foreach (DataRow accountRow in statementDataset.Tables["Table"].Rows)
                {
                    Account account = new Account();
                    account.AgreementTotal = decimal.Parse(accountRow["agrmttotal"].ToString());
                    account.AccountNo = accountRow["acctno"].ToString();
                    decimal outstandingBalance = decimal.Parse(accountRow["outstbal"].ToString());

                    List<Transaction> transactions = new List<Transaction>();
                    foreach (DataRow transactionRow in statementDataset.Tables["Table1"].Rows)
                    {
                        if (transactionRow["acctno"].ToString() == account.AccountNo)
                            transactions.Add(
                                new Transaction
                                {
                                    ReferenceNo = transactionRow["transrefno"].ToString(),
                                    Date = DateTime.Parse(transactionRow["datetrans"].ToString()),
                                    PaymentType = transactionRow["transtypecode"].ToString(),
                                    Amount = decimal.Parse(transactionRow["transvalue"].ToString())
                                });
                    }

                    transactions.Sort();

                    foreach (Transaction transaction in transactions)
                    {
                        transaction.RemainingBalance = outstandingBalance;
                        outstandingBalance -= transaction.Amount;
                    }

                    foreach (Transaction transaction in transactions)
                        if (transaction.Date >= dateFrom && transaction.Date <= dateTo)
                            account.Transactions.Add(transaction);

                    account.Transactions.Reverse();

                    customer.Accounts.Add(account);
                }

                foreach (DataRow row in statementDataset.Tables["Table2"].Rows)
                {
                    customer.Address1 = row["cusaddr1"].ToString();
                    customer.Address2 = row["cusaddr2"].ToString();
                    customer.Address3 = row["cusaddr3"].ToString();
                    customer.Address4 = row["cuspocode"].ToString();
                    customer.Name = row["title"].ToString() + " " + row["firstname"].ToString() + " " + row["name"].ToString();
                    customer.CreditLimit = decimal.Parse(row["RFCreditLimit"].ToString());
                    customer.AvailableSpend = decimal.Parse(row["AvailableSpend"].ToString());
                }

                if (customer.IsEmpty)
                    return null;
            }

            return customer;
        }

        BBSCustomer _GetAccountStatement(string accountNo, DateTime dateFrom, DateTime dateTo)
        {
            BBSCustomer customer = new BBSCustomer();
            Account account = new Account();
            List<Transaction> transactions = new List<Transaction>();
            
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                SqlParameter[] parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@date", SqlDbType.DateTime);
                parmArray[1].Value = dateFrom;

                DataSet statementDataset = new DataSet();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = CreateCommand("GetAccountStatement", parmArray, connection);
                sqlDA.Fill(statementDataset);

                decimal outstandingBalance = 0;
                foreach (DataTable dt in statementDataset.Tables)
                {
                    switch (dt.TableName)
                    {
                        case "Table":
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    account.AgreementTotal = decimal.Parse(row["agrmttotal"].ToString());
                                    account.AccountNo = row["acctno"].ToString();
                                    outstandingBalance = decimal.Parse(row["outstbal"].ToString());
                                } break;
                            }

                        case "Table1":
                            {
                                foreach (DataRow row in dt.Rows)
                                    transactions.Add(
                                        new Transaction
                                        {
                                            ReferenceNo = row["transrefno"].ToString(),
                                            Date = DateTime.Parse(row["datetrans"].ToString()),
                                            PaymentType = row["transtypecode"].ToString(),
                                            Amount = decimal.Parse(row["transvalue"].ToString())
                                        });

                                transactions.Sort();

                                foreach (Transaction transaction in transactions)
                                {
                                    transaction.RemainingBalance = outstandingBalance;
                                    outstandingBalance -= transaction.Amount;
                                }

                                foreach (Transaction transaction in transactions)
                                    if (transaction.Date >= dateFrom && transaction.Date <= dateTo)
                                        account.Transactions.Add(transaction);

                                account.Transactions.Reverse();

                                break;
                            }

                        case "Table2":
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    customer.Address1 = row["cusaddr1"].ToString();
                                    customer.Address2 = row["cusaddr2"].ToString();
                                    customer.Address3 = row["cusaddr3"].ToString();
                                    customer.Address4 = row["cuspocode"].ToString();
                                    customer.Name = row["title"].ToString() + " " + row["firstname"].ToString() + " " + row["name"].ToString();
                                    customer.CreditLimit = decimal.Parse(row["RFCreditLimit"].ToString());
                                    customer.AvailableSpend = decimal.Parse(row["AvailableSpend"].ToString());
                                }

                                break;
                            }
                    }
                }

                if (string.IsNullOrEmpty(account.AccountNo))
                    return null;

                customer.Accounts.Add(account);
                //account.Transactions = transactions;
            }

            return customer;
        }

        BBSCustomer _GetAccountStatementLastTransactions(string accountNo, int noOfTransactions)
        {
            BBSCustomer customer = new BBSCustomer();
            Account account = new Account();
            List<Transaction> transactions = new List<Transaction>();

            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                SqlParameter[] parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@date", SqlDbType.DateTime);
                parmArray[1].Value = new DateTime(1900, 1,1);

                DataSet statementDataset = new DataSet();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = CreateCommand("GetAccountStatement", parmArray, connection);
                sqlDA.Fill(statementDataset);

                decimal outstandingBalance = 0;
                foreach (DataTable dt in statementDataset.Tables)
                {
                    switch (dt.TableName)
                    {
                        case "Table":
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    account.AgreementTotal = decimal.Parse(row["agrmttotal"].ToString());
                                    account.AccountNo = row["acctno"].ToString();
                                    outstandingBalance = decimal.Parse(row["outstbal"].ToString());
                                } break;
                            }

                        case "Table1":
                            {
                                int i = 1;
                                foreach (DataRow row in dt.Rows)
                                {
                                    if (i > noOfTransactions)
                                        break;
                                    transactions.Add(
                                        new Transaction
                                        {
                                            ReferenceNo = row["transrefno"].ToString(),
                                            Date = DateTime.Parse(row["datetrans"].ToString()),
                                            PaymentType = row["transtypecode"].ToString(),
                                            Amount = decimal.Parse(row["transvalue"].ToString())
                                        });
                                    i++;
                                }

                                transactions.Sort();

                                foreach (Transaction transaction in transactions)
                                {
                                    transaction.RemainingBalance = outstandingBalance;
                                    outstandingBalance -= transaction.Amount;
                                }

                                break;
                            }

                        case "Table2":
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    customer.Address1 = row["cusaddr1"].ToString();
                                    customer.Address2 = row["cusaddr2"].ToString();
                                    customer.Address3 = row["cusaddr3"].ToString();
                                    customer.Address4 = row["cuspocode"].ToString();
                                    customer.Name = row["title"].ToString() + " " + row["firstname"].ToString() + " " + row["name"].ToString();
                                    customer.CreditLimit = decimal.Parse(row["RFCreditLimit"].ToString());
                                    customer.AvailableSpend = decimal.Parse(row["AvailableSpend"].ToString());
                                }

                                break;
                            }
                    }
                }

                if (string.IsNullOrEmpty(account.AccountNo))
                    return null;

                customer.Accounts.Add(account);
                account.Transactions = transactions;
                account.Transactions.Reverse();
            }

            return customer;
        }

        public static BBSCustomer GetAccountStatement(string accountNo, DateTime dateFrom, DateTime dateTo)
        {
            return (new DStatement())._GetAccountStatement(accountNo, dateFrom, dateTo);
        }
        public static BBSCustomer GetCustomerStatement(string customerID, DateTime dateFrom, DateTime dateTo, bool onlyHolderAccounts)
        {
            return (new DStatement())._GetCustomerStatement(customerID, dateFrom, dateTo, onlyHolderAccounts);
        }
        public static BBSCustomer GetAccountStatementLastTransactions(string accountNo, int noOfTransactions)
        {
            return (new DStatement())._GetAccountStatementLastTransactions(accountNo, noOfTransactions);
        }

        //static void Main(string[] args)
        //{
        //    DStatement.GetCustomerStatement("RP040474", new DateTime(2005, 05, 11), DateTime.Now, false);
        //    DStatement.GetAccountStatement("700010208021", new DateTime(2005, 05, 11), DateTime.Now);//new DateTime(2009, 05, 11));
        //    DStatement.GetAccountStatementLastTransactions("700010208021", 5);
        //}
    }
}
