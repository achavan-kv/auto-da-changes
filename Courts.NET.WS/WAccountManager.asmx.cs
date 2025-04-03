using AutoMapper;
using Blue.Cosacs;
using Blue.Cosacs.Messages.Service;
using Blue.Cosacs.Messages.Warehouse;
using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared;
using STL.BLL;
using STL.BLL.OracleIntegration;
using STL.Common;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.AuditSource;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Enums;
using STL.Common.Constants.FTransaction; //IP - 7/12/10 - Store Card
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Tags;
using STL.Common.Static;
using STL.Common.Structs;
using STL.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;
using Blue.Cosacs.BLL.WebApi;
using STL.BLL.WebApi;


namespace STL.WS
{
    /// <summary>
    /// This is the interface web service through which all 
    /// account related functions are channelled
    /// </summary>
    ///
    [WebService(Namespace = "http://strategicthought.com/webservices/")]
    public class WAccountManager : CommonService
    {
        private BAccount account;
        private BAgreement agreement;
        private BAccount flags;
        public int InvVersion { get; private set; }

        public WAccountManager()
        {
            //CODEGEN: This call is required by the ASP.NET Web Services Designer
            InitializeComponent();
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public BBSL.Libraries.Printing.BBSCustomer GetCustomerStatement(string customerID, DateTime dateFrom, DateTime dateTo, bool onlyHolderAccounts)
        {
            Function = "BAccountManager::GetCustomerStatement()";
            BBSL.Libraries.Printing.BBSCustomer customer = null;

            try
            {
                customer = DStatement.GetCustomerStatement(customerID, dateFrom, dateTo, onlyHolderAccounts);
            }
            catch (Exception ex)
            {
                string err = "";
                Catch(ex, Function, ref err);
            }

            return customer;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public BBSL.Libraries.Printing.BBSCustomer GetAccountStatement(string accountNo, DateTime dateFrom, DateTime dateTo)
        {
            Function = "BAccountManager::GetAccountStatement()";
            BBSL.Libraries.Printing.BBSCustomer customer = null;

            try
            {
                customer = DStatement.GetAccountStatement(accountNo, dateFrom, dateTo);
            }
            catch (Exception ex)
            {
                string err = "";
                Catch(ex, Function, ref err);
            }

            return customer;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public BBSL.Libraries.Printing.BBSCustomer GetAccountStatementLastTransactions(string accountNo, int noOfTransactions)
        {
            Function = "BAccountManager::GetAccountStatement()";
            BBSL.Libraries.Printing.BBSCustomer customer = null;

            try
            {
                customer = DStatement.GetAccountStatementLastTransactions(accountNo, noOfTransactions);
            }
            catch (Exception ex)
            {
                string err = "";
                Catch(ex, Function, ref err);
            }

            return customer;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetAccountsAwaitingClearance(string branchRestriction, int includeCash, int includeHP,
            int includeRF, int includePaid, int includeUnpaid, int includeItems, string holdFlags, int includeGOL, out string err)
        {
            Function = "BAccountManager::GetAccountsAwaitingClearance()";
            DataSet ds = null;
            err = "";

            try
            {
                account = new BAccount();
                ds = account.GetAccountsAwaitingClearance(branchRestriction,
                    includeCash,
                    includeHP,
                    includeRF,
                    includePaid,
                    includeUnpaid,
                    includeItems,
                    holdFlags,
                    includeGOL);

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetInstantCreditAwaitingClearance(string branchRestriction, int includeHP,
            int includeRF, string holdFlags, out string err)
        {
            Function = "BAccountManager::GetInstantCreditAwaitingClearance()";
            DataSet ds = null;
            err = "";

            try
            {
                using (var conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        account = new BAccount();
                        account.User = STL.Common.Static.Credential.UserId; //IP - 03/03/11 - #3255
                        ds = account.GetInstantCreditAwaitingClearance(branchRestriction,
                                                                       includeHP,
                                                                       includeRF,
                                                                       holdFlags, conn, trans);

                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public decimal GetChargeableCashPrice(string accountNo, ref decimal chargeableAdminPrice, out string err)
        {
            Function = "BAccountManager::GetChargeableCashPrice()";
            //DataSet ds = null;
            err = "";
            decimal cashPrice = 0;

            try
            {
                account = new BAccount();
                cashPrice = account.GetChargeableCashPrice(null, null, accountNo, ref chargeableAdminPrice);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return cashPrice;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetChargesByAcctNo(string acctNo, out string err)
        {
            Function = "BAccountManager::GetChargesByAcctNo()";
            DataSet ds = null;
            err = "";

            try
            {
                account = new BAccount();
                ds = account.GetChargesByAcctNo(acctNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetArrearsDailyByAcctNo(string acctNo, out string err)
        {
            Function = "BAccountManager::GetArrearsDailyByAcctNo()";
            DataSet ds = null;
            err = "";

            try
            {
                account = new BAccount();
                ds = account.GetArrearsDailyByAcctNo(acctNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetUnpaidAccounts(int branchNo, int empeeNoSale, out string err)
        {
            Function = "BAccountManager::GetUnpaidAccounts()";
            DataSet ds = null;
            err = "";

            try
            {
                account = new BAccount();
                ds = account.GetUnpaidAccounts(branchNo, empeeNoSale);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }



        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetFinTransQueryResults(DateTime datestart,
            DateTime datefinish,
            string transtypeoperand,
            string transtypecode,
            string valueoperand,
            int startrunno,
            int endrunno,
            string runnooperand,
            int startrefno,
            int endrefno,
            string refnooperand,
            int empeeno,
            string empeenooperand,
            int branchno,
            string branchnooperand,
            int accountinbranch,
            string dateoperand,
            string branchsetname,
            string transtypesetname,
            string employeesetname,
            int valueonly,
            int includeothercharges,
            out string err)
        {
            Function = "WAccountManager::GetFinTransQueryResults()";
            DataSet ds = null;
            err = "";

            try
            {
                BTransaction transaction = new BTransaction();
                ds = transaction.GetFinTransQueryResults(datestart,
                    datefinish,
                    transtypeoperand,
                    transtypecode,
                    valueoperand,
                    startrunno,
                    endrunno,
                    runnooperand,
                    startrefno,
                    endrefno,
                    refnooperand,
                    empeeno,
                    empeenooperand,
                    branchno,
                    branchnooperand,
                    accountinbranch,
                    dateoperand,
                    branchsetname,
                    transtypesetname,
                    employeesetname,
                    valueonly,
                    includeothercharges);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int ClearProposal(string accountNumber, string source, out string err) //IP - 04/02/10 - CR1072 - 3.1.9 - Added Source of Delivery Authorisation.
        {
            Function = "BAccountManager::ClearProposal()";
            err = "";
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            agreement = new BAgreement();
                            agreement.User = STL.Common.Static.Credential.UserId;
                            agreement.ClearProposal(conn, trans, accountNumber, source);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetHoldFlags(string accountNumber, out string err)
        {
            Function = "BAccountManager::GetHoldFlags()";
            DataSet ds = null;
            err = "";

            try
            {
                flags = new BAccount();
                ds = flags.GetHoldFlags(accountNumber);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetICFlags(string accountNumber, out string err)
        {
            Function = "BAccountManager::GetICFlags()";
            DataSet ds = null;
            err = "";

            try
            {
                flags = new BAccount();
                ds = flags.GetICFlags(accountNumber);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod(Description = "This method generates a new account number")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public XmlNode GenerateAccountNumber(string countryCode, short branchNumber, string accountType, bool manualSale, out string newAccountNo, out string err, int userid = 0)
        {
            Function = "BAccountManager::GenerateAccountNumber()";
            newAccountNo = "";
            err = "";
            XmlNode stampDuty = null;
            SqlConnection conn = null;


            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            BAccount account = new BAccount();
                            stampDuty = account.GenerateAccountNumber(conn, trans, countryCode, branchNumber, accountType, manualSale, out newAccountNo);
                            if (newAccountNo.Length != 0 && newAccountNo != "000-0000-0000-0")
                                account.Lock(conn, trans, newAccountNo.Replace("-", ""), STL.Common.Static.Credential.UserId);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return stampDuty;
        }

        public class GetStockByLocationRequest
        {
            public short StockLocationNo { get; set; }
            public short BranchCode { get; set; }
            public int ShowDeleted { get; set; }
            public int ShowAvailable { get; set; }
            public string ProductDesction { get; set; }
            public bool Limit { get; set; }
        }
        [WebMethod(Description = "This method brings back a list of stockitems based on location and any description entered")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetStockByLocation(GetStockByLocationRequest r, out string err)
        {
            Function = "BAccountManager::GetStockByLocation()";
            err = "";
            DataSet ds = null;
            try
            {
                ds = new BItem()
                    .GetStockByLocation(
                        r.StockLocationNo,
                        r.BranchCode,
                        r.ShowDeleted,
                        r.ShowAvailable,
                        r.ProductDesction,
                        r.Limit);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method brings back a list of stockitems based on location and any description entered")]
        [SoapHeader("authentication")]
#if(TRACE)

		[TraceExtension]
#endif
        public DataSet GetStockByCode(string productCode, out string err)
        {
            Function = "BAccountManager::GetStockByCode()";
            err = "";
            DataSet ds = null;
            try
            {
                BItem item = new BItem();
                ds = item.GetStockByCode(productCode);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        public class GetItemDetailsRequest
        {
            public short AccountBranch { get; set; }
            public string AccountType { get; set; }
            public short BranchCode { get; set; }
            public string CountryCode { get; set; }
            public bool IsDutyFree { get; set; }
            public bool IsTaxExempt { get; set; }
            public string ProductCode { get; set; }
            public short PromoBranch { get; set; }
            public short StockLocationNo { get; set; }
            public string AccountNo { get; set; }
            public int AgrmtNo { get; set; }
            public int ItemID { get; set; }     //CR1212 jec - 21/04/11
        }


        [WebMethod(Description = "This method gets the details for the item selected from the stock query by location screen")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public XmlNode GetItemDetails(GetItemDetailsRequest itemDetailsRequest, out string err)
        {
            Function = "BAccountManager::GetItemDetails()";
            XmlNode itemNode = null;
            err = "";
            try
            {
                itemNode = new BItem
                {
                    PromoBranch = itemDetailsRequest.PromoBranch,
                    AccountBranch = itemDetailsRequest.AccountBranch
                }
                .GetItemDetails(itemDetailsRequest.ItemID,
                                itemDetailsRequest.StockLocationNo,
                                itemDetailsRequest.BranchCode,
                                itemDetailsRequest.AccountType,
                                itemDetailsRequest.CountryCode,
                                itemDetailsRequest.IsDutyFree,
                                itemDetailsRequest.IsTaxExempt);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return itemNode;
        }

        [WebMethod(Description = "This method will attempt to lock an account for a specific user", MessageName = "LockAccount")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool LockAccount(string accountNo, string user, out string err)
        {
            Function = "BAccountManager::LockAccount()";
            err = "";
            bool locked = true;
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BAccount acct = new BAccount();
                            acct.Lock(conn, trans, accountNo, Convert.ToInt32(user));
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                locked = false;
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return locked;
        }


        [WebMethod(Description = "This method will attempt to lock an account for a specific user", MessageName = "LockReviseAccount")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool LockAccount(string accountNo, string user, bool revise, out string err)
        {
            Function = "BAccountManager::LockAccount()";
            err = "";
            bool locked = true;
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            BAccount acct = new BAccount();
                            acct.Lock(conn, trans, accountNo, Convert.ToInt32(user), revise);

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                locked = false;
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return locked;
        }


        [WebMethod(Description = "This method will attempt to unlock an account for a specific user", MessageName = "UnlockAccount")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int UnlockAccount(string accountNo, int user, out string err)
        {
            Function = "BAccountManager::UnlockAccount()";
            err = "";
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BAccount acct = new BAccount();
                            acct.Unlock(conn, trans, accountNo, user);

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod(Description = "This method will validate the sale attempted for cints", MessageName = "ValidateCint")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int ValidateSaleForCINT(string sku,
           int Quantity,
           int salelocation,
           int stocklocation,
           DateTime Transactiondate,
           out string ErrorMessage)

        {
            Function = "BAccountManager::ValidateSaleForCINT()";
            ErrorMessage = "";
            int ErrorCount = -1;
            SqlConnection conn = null;
            try
            {
                BAccount acct = new BAccount();
                acct.ValidateSaleForCINT(sku, Quantity, salelocation, stocklocation, Transactiondate, out ErrorMessage, out ErrorCount);


            }
            catch (SqlException ex)
            {
                CatchDeadlock(ex, conn);
            }
            return ErrorCount;
        }


        [WebMethod(Description = "This method will attempt to unlock an account for a specific user", MessageName = "UnlockReviseAccount")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int UnlockAccount(string accountNo, string user, bool revise, out string err)
        {
            Function = "BAccountManager::UnlockAccount()";
            err = "";
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            BAccount acct = new BAccount();
                            acct.Unlock(conn, trans, accountNo, revise, Convert.ToInt32(user));

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }


        [WebMethod(Description = "This method returns a dataset containing all warranties that can be sold with a particular product")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetProductWarrantiesByCode(string productCode, short branchCode, double unitPrice, string refCode, bool paidAndTaken, out string err)
        {
            Function = "BAccountManager::GetProductWarrantiesByCode()";
            DataSet ds = null;
            err = "";
            try
            {
                var itemId = new StockRepository()
                                .GetStockInfo(productCode, false)        // RI jec
                                .Select(s => s.Id)
                                .FirstOrDefault();

                ds = new BItem().GetProductWarranties(null, null, itemId, branchCode, unitPrice, refCode, paidAndTaken);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method returns a dataset containing all warranties that can be sold with a particular product")]
        [SoapHeader("authentication")]
        public DataSet GetProductWarranties(int itemId, short branchCode, double unitPrice, string refCode, bool paidAndTaken, out string err)
        {
            Function = "BAccountManager::GetProductWarranties()";
            DataSet ds = null;
            err = "";
            try
            {
                BItem item = new BItem();
                ds = item.GetProductWarranties(null, null, itemId, branchCode, unitPrice, refCode, paidAndTaken);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method returns an XmlNode containing all the component items which comprise a kit product")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public XmlNode AddKitToAccount(ref XmlNode doc, XmlNode kitNode, int itemId, short branchCode, string accountType, double kitQty, string countryCode, bool dutyFree, bool taxExempt, short promoBranch, out string err)
        {
            Function = "BAccountManager::AddKitToAccount()";
            err = "";
            XmlNode comps = null;
            try
            {
                //this doesn't actually perform any database updates so does not require
                //a transaction
                BAccount acct = new BAccount();
                comps = acct.AddKitToAccount(ref doc, kitNode, itemId, branchCode, accountType, kitQty, countryCode, dutyFree, taxExempt, promoBranch, out err);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return comps;
        }


        [WebMethod(Description = "This method returns a dataset containing all accounts for a particular customer ID")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet AccountsSearch(string accountNo,
            string custId,
            string firstName,
            string lastName,
            string address,
            string postCode,
            string phoneNumber,     //CR1084
            bool settled,
            bool exactMatch,
            int limit,
            string storeType,
            out int accountExists,
            out string accountType,
            out string err)
        {
            Function = "BAccount::AccountsSearch()";
            DataSet ds = null;
            err = "";
            accountExists = 0;
            accountType = "";
            try
            {
                BAccount account = new BAccount();
                ds = account.AccountsSearch(accountNo,
                    custId,
                    firstName,
                    lastName,
                    address,
                    postCode,
                    phoneNumber,        //CR1084
                    settled,
                    exactMatch,
                    limit,
                    storeType,
                    out accountExists,
                    out accountType);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod(Description = "This method returns a dataset of incomplete credit accounts")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet IncompleteCredits(string branchRestriction,
            string holdFlags,
            bool viewTop,
            string acctno,
            bool ChxOnly,
            bool ChxItems,
            bool ChxUnpaid,
            bool ChxReferralCL,
            bool ChxPendingCL,
            string refCode,
            out string err)
        {
            Function = "BAccount::IncompleteCredits()";
            DataSet ds = null;
            err = "";
            try
            {
                BAccount account = new BAccount();
                ds = account.IncompleteCredits(branchRestriction, holdFlags, viewTop, acctno,
                    ChxOnly, ChxItems, ChxUnpaid, refCode, ChxReferralCL, ChxPendingCL);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }


        [WebMethod(Description = "This method returns a dataset of followup allocations for an account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetFollowUpHistory(string acctNo, out string err)
        {
            Function = "BAccount::GetFollowUpHistory()";
            DataSet ds = null;
            err = "";
            try
            {
                BAccount account = new BAccount();
                ds = account.GetFollowUpHistory(acctNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }



        [WebMethod(Description = "This method returns a dataset of arrears accounts for allocation")]
        [SoapHeader("authentication")]
#if(TRACE)
                [TraceExtension]
#endif
        public DataSet Getacctsforalloc5_2(string alreadyAllocated, string minimumStatus, string currStatus, string
            employeeType, DateTime dateStartAllocated, DateTime dateFinishedAllocated,
            string actionChoice, DateTime actionStart, DateTime actionEnd, double
            minimumArrears, double maximumArrears, string statusType, string arrearsChoice,
            double arrears, string actionCode, string letterCode, string letterRestriction,
            bool letterRadio, DateTime letterStart, DateTime letterFinish, string
            actionRestriction, int empeeno, DateTime actiondateStart, DateTime
            actiondateFinish, bool includePhone, bool includeAddress, short branchno, string branchset, short
            proposalPoints, string propPointsDirection, string codeRestriction,
            string accountBranch, bool viewTop, string code, short numActions, string actionOperand, string balanceOperand,
            short restrictEmployee, decimal balance, short includeCharges, DateTime dateMovedFrom,
            DateTime dateMovedTo, string dateMovedRestriction, DateTime datelastPaid, string dateOperand,
            bool actionDueDate, bool credit, bool cash, bool service, string worklist, string deliveryArea, ref bool rowLimited, out string err)
        {
            Function = "BAccount::Getacctsforalloc()";
            DataSet ds = null;
            err = "";
            try
            {
                BAccount account = new BAccount();
                ds = account.Getacctsforalloc(alreadyAllocated, minimumStatus,
                    currStatus, employeeType, dateStartAllocated, dateFinishedAllocated,
                    actionChoice, actionStart, actionEnd,
                    minimumArrears, maximumArrears, statusType, arrearsChoice, arrears,
                    actionCode, letterCode, letterRestriction, letterRadio, letterStart,
                    letterFinish, actionRestriction, empeeno, actiondateStart,
                    actiondateFinish, includePhone, includeAddress, branchno, branchset,
                    proposalPoints, propPointsDirection, codeRestriction,
                    accountBranch, viewTop, ref rowLimited, code, numActions, actionOperand, balanceOperand,
                    restrictEmployee, balance, includeCharges, dateMovedFrom, dateMovedTo,
                    dateMovedRestriction, datelastPaid, dateOperand, actionDueDate, credit, cash, service, worklist, deliveryArea);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method returns a dataset of arrears accounts for allocation")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet Getacctsforalloc(string alreadyAllocated, string minimumStatus, string currStatus, string
            employeeType, DateTime dateStartAllocated, DateTime dateFinishedAllocated,
            string actionChoice, DateTime actionStart, DateTime actionEnd, double
            minimumArrears, double maximumArrears, string statusType, string arrearsChoice,
            double arrears, string actionCode, string letterCode, string letterRestriction,
            bool letterRadio, DateTime letterStart, DateTime letterFinish, string
            actionRestriction, int empeeno, DateTime actiondateStart, DateTime
             actiondateFinish, bool includePhone, bool includeAddress, short branchno, string branchset, short
            proposalPoints, string propPointsDirection, string codeRestriction,
            string accountBranch, bool viewTop, string code, short numActions, string actionOperand, string balanceOperand,
            short restrictEmployee, decimal balance, short includeCharges, DateTime dateMovedFrom,
            DateTime dateMovedTo, string dateMovedRestriction, DateTime datelastPaid, string dateOperand,
             bool actionDueDate, bool credit, bool cash, bool service, ref bool rowLimited, out string err)
        {
            Function = "BAccount::Getacctsforalloc()";
            DataSet ds = null;
            err = "";
            try
            {
                BAccount account = new BAccount();
                ds = account.Getacctsforalloc(alreadyAllocated, minimumStatus,
                    currStatus, employeeType, dateStartAllocated, dateFinishedAllocated,
                    actionChoice, actionStart, actionEnd,
                    minimumArrears, maximumArrears, statusType, arrearsChoice, arrears,
                    actionCode, letterCode, letterRestriction, letterRadio, letterStart,
                    letterFinish, actionRestriction, empeeno, actiondateStart,
                       actiondateFinish, includePhone, includeAddress, branchno,
                    branchset, proposalPoints, propPointsDirection, codeRestriction,
                    accountBranch, viewTop, ref rowLimited, code, numActions, actionOperand, balanceOperand,
                    restrictEmployee, balance, includeCharges, dateMovedFrom, dateMovedTo,
                    dateMovedRestriction, datelastPaid, dateOperand, actionDueDate, credit, cash, service);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method returns a dataset of accounts in the bailiff or collector strategies.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetStrategyAccountsToAllocate(out string err)
        {
            Function = "BAccount::GetStrategyAccountsToAllocate()";
            DataSet ds = null;
            err = "";
            try
            {
                BAccount account = new BAccount();
                ds = account.GetStrategyAccountsToAllocate();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        [WebMethod(Description = "This method returns a flag true if there exists delivery for given non stock account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet IsAccountValidForOnlyNonStockSale(string accountNumber, out string err)
        {
            Function = "BAccountManager::IsAccountValidForOnlyNonStockSale()";
            DataSet ds = null;
            err = "";
            try
            {
                BAccount accountdetails = new BAccount();
                ds = accountdetails.IsAccountValidForOnlyNonStockSale(accountNumber);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        [WebMethod(Description = "This is to delivery non stock for nonstock sale account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void DeliverNonStock(string accountNumber, out string err)
        { 
            Function = "BAccountManager::DeliverNonStock()";
            err = "";
            try
            {
                SqlConnection conn = new SqlConnection(Connections.Default);

                conn.Open();
                try
                {
                    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        BAccount accountdetails = new BAccount();
                        accountdetails.DeliverNonStock(conn, trans, accountNumber);
                        trans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    Catch(ex, Function, ref err);
                }
                finally
                {
                    if (conn.State != ConnectionState.Closed)
                        conn.Close();
                }

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

        }

        [WebMethod(Description = "This will save non-stock accounts in table")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void SaveAccountForSaleOnlyNonStock(string acctno)
        {
            Function = "WAccountManager::SaveAccountForSaleOnlyNonStock()";
            SqlConnection conn = null;

            string err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BAccount ba = new BAccount();
                            ba.SaveAccountForSaleOnlyNonStock(conn, trans, acctno);

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int DisableSave(string acctno,out string invoice, out int status)
        {
            Function = "WAccountManager::DisableSave()";
            SqlConnection conn = null;

            string err = "";
            invoice = "";
            status = 0;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BAccount ba = new BAccount();
                            ba. DisableSave(conn, trans, acctno,out invoice,out status);
                           
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);

               

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;

        }
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int DisableRFButton(string custid,out int status) //Arunnew
        {
            Function = "WAccountManager::DisableRFButton()";
            SqlConnection conn = null;

            string err = "";
            status = 0;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BAccount ba = new BAccount();
                            ba.DisableRFButton(conn, trans, custid,  out status);

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);



            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;

        }


        [WebMethod(Description = "This will give nonsalable  non-stock items")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet ValidateNonSaleableNonStocks(string productSKU)
        {
            Function = "BAccountManager::ValidateNonSaleableNonStocks()";
            DataSet ds = null;
            string err = "";
            try
            {
                BAccount a = new BAccount();
                ds = a.ValidateNonSaleableNonStocks(productSKU);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        
        [WebMethod(Description = "This method returns a dataset containing all accounts for a particular customer ID")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetAccountDetails(string accountNumber, out string err)
        {
            Function = "BAccountManager::GetAccountDetails()";
            DataSet ds = null;
            err = "";
            try
            {
                BAccount accountdetails = new BAccount();
                ds = accountdetails.GetAccountDetails(accountNumber);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method calculates the instal plan")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int CalculateInstalPlan(decimal subTotal,
            decimal deposit,
            decimal deferredTerms,
            decimal months,
            out decimal monthly,
            out decimal final, out string err)
        {
            Function = "BAccountManager::CalculateInstalPlan()";
            err = "";
            monthly = 0;
            final = 0;
            try
            {
                BAgreement agree = new BAgreement();
                agree.CalculateInstalPlan(subTotal, deposit, deferredTerms, months, out monthly, out final);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0;
        }

        [WebMethod(Description = "This method calculates the service charge to be applied to the account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public decimal CalculateServiceCharge(string countryCode, string termsType, string acctNo, string overrideBand, decimal deposit, decimal months, decimal subTotal, DateTime dateOpened, string accountType, decimal adminSubTotal, ref decimal insuranceCharge, ref decimal adminCharge, ref DataSet variableRatesSet, out string err)
        {
            Function = "BAccountManager::CalculateServiceCharge()";
            err = "";
            decimal serviceCharge = 0;
            insuranceCharge = 0;
            adminCharge = 0;
            try
            {
                BAgreement agree = new BAgreement();
                serviceCharge = agree.CalculateServiceCharge(null, null, countryCode, termsType, acctNo, overrideBand, deposit, months, subTotal, dateOpened, accountType, adminSubTotal, ref insuranceCharge, ref adminCharge, ref variableRatesSet);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return serviceCharge;
        }

        [WebMethod(Description = "This method returns a term month need to extend to adjust installment within MMI threshold limit.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public decimal CalculateMonthToExtend(string countryCode, string termsType, string acctNo, string overrideBand, DateTime dateOpened
                                                , decimal deposit, decimal months, decimal subTotal, string accountType
                                                , decimal adminSubTotal, decimal monthlyInstalment
                                                , decimal cmInsuranceTax, decimal cmAdminTax, decimal cmDtTax, decimal mmiThresholdLimit
                                                , ref DataSet variableRatesSet, decimal saleOrderVoucherValue, XmlNode saleOrderDT, bool taxExempt, out string err)
        {
            Function = "BAccountManager::CalculateMonthToExtend()";
            err = string.Empty;            
            decimal monthToExtend = 0;
            try
            {
                BAgreement agree = new BAgreement();
                monthToExtend = agree.CalculateMonthToExtend(null, null
                                                            , countryCode, termsType, acctNo, overrideBand, dateOpened
                                                            , deposit, months, subTotal
                                                            , accountType, adminSubTotal, monthlyInstalment
                                                            //, ref insuranceCharge, ref adminCharge
                                                            , cmInsuranceTax, cmAdminTax, 
                                                            //cmDtTax, 
                                                            mmiThresholdLimit
                                                            , saleOrderVoucherValue, saleOrderDT, taxExempt);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return monthToExtend;
        }

        [WebMethod(Description = "This method returns a amount to deposite to adjust monthly installment within MMI threshold limit.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public decimal CalculateAmountToDeposite(string countryCode, string termsType, string acctNo, string overrideBand, DateTime dateOpened
                                                , decimal installementAmount, decimal months, decimal subTotal, string accountType
                                                , decimal adminSubTotal, decimal cmInsuranceTax, decimal cmAdminTax, decimal newDefferedTermAmount, out string err)
        {
            Function = "BAccountManager::CalculateAmountToDeposite()";
            err = string.Empty;
            decimal amountToDeposite = 0;
            try
            {
                BAgreement agree = new BAgreement();
                amountToDeposite = agree.CalculateAmountToDeposite(null, null
                                                            , countryCode, termsType, acctNo, overrideBand, dateOpened
                                                            , installementAmount, months, subTotal, accountType
                                                            , adminSubTotal, cmInsuranceTax, cmAdminTax, newDefferedTermAmount);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return amountToDeposite;
        }



        [WebMethod(Description = "This method returns a dataset containing the details for a specific terms type code")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetTermsTypeDetails(string countryCode, string termsType, string acctNo, DateTime dateAcctOpen, out string err)
        {
            Function = "BAccountManager::GetTermsTypeDetails()";
            DataSet ds = null;
            err = "";
            try
            {
                BAgreement agree = new BAgreement();
                ds = agree.GetTermsTypeDetails(countryCode, termsType, acctNo, dateAcctOpen);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method returns a dataset containing the details for a specific terms type code")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet TermsTypeGetDetails(string acctNo, out string err)
        {
            Function = "BAccountManager::TermsTypeGetDetails()";
            DataSet ds = null;
            err = "";
            try
            {
                BAgreement agree = new BAgreement();
                ds = agree.TermsTypeGetDetails(acctNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        public class SaveNewAccountParameters
        {
            public string AccountNumber { get; set; }
            public short BranchNo { get; set; }
            public string AccountType { get; set; }
            public string CODFlag { get; set; }
            public int SalesPerson { get; set; }
            public string SOA { get; set; }
            public double AgreementTotal { get; set; }
            public double Deposit { get; set; }
            public double ServiceCharge { get; set; }
            public XmlNode LineItems { get; set; }
            public string TermsType { get; set; }
            public string NewBand { get; set; }
            public string CountryCode { get; set; }
            public DateTime DateFirst { get; set; }
            public double InstalAmount { get; set; }
            public double FinalInstalment { get; set; }
            public string PaymentMethod { get; set; }
            public int Months { get; set; }
            public bool TaxExempt { get; set; }
            public bool DutyFree { get; set; }
            public decimal TaxAmount { get; set; }
            public bool Collection { get; set; }
            public string BankCode { get; set; }
            public string BankAcctNo { get; set; }
            public string ChequeNo { get; set; }
            public short PayMethod { get; set; }
            public XmlNode ReplacementXml { get; set; }
            public decimal DtTaxAmount { get; set; }
            public string LoyaltyCardNo { get; set; }
            public bool ReScore { get; set; }
            public decimal Tendered { get; set; }
            public decimal GiftVoucherValue { get; set; }
            public bool CourtsVoucher { get; set; }
            public string VoucherReference { get; set; }
            public int VoucherAuthorisedBy { get; set; }
            public string AccountNoCompany { get; set; }
            public short PromoBranch { get; set; }
            public short PaymentHolidays { get; set; }
            public DataSet PayMethodSet { get; set; }
            public short DueDay { get; set; }
            public int ReturnAuthorisedBy { get; set; }
            public DataSet WarrantyRenewalSet { get; set; }
            public DataSet VariableRatesSet { get; set; }

            public int AgreementNo { get; set; }
            public string PropResult { get; set; }
            public DateTime DateProp { get; set; }

            public bool ResetPropResult { get; set; }
            public bool Autoda { get; set; }
            public int User { get; set; }
            public string StoreCardAcctNo { get; set; }  //IP - 17/01/11 - Store Card
            public long? StoreCardNumber { get; set; }   //IP - 17/01/11 - Store Card
            public bool PaidAndTaken { get; set; }   //IP - 18/06/12 - #10410
            public bool HasInstallation { get; set; }   //jec #14432 
            public bool CustLinkRequired { get; set; }   //jec #15122
            public bool CashAndGoReturn { get; set; }   // #16339 
            public string CollectionType { get; set; }   //#17678
            public DataTable WarrantyRefunds { get; set; } //#16607
            public bool ReadyAssist { get; set; }       //#18603 - CR15594
            public int? ReadyAssistTermLength { get; set; }  //#18603 - CR15594
            public bool ReviseAccount { get; set; }//CR2018-13

			public Int16 PrefDay { get; set; }  //CR10.7
            /*
            out string bureauFailure 
            out string err*/
        }

        public class CreditParameters : SaveNewAccountCreditParameters
        {

        }


        public class SaveNewAccountResult
        {
            public int AgreementNo { get; set; }
            public string PropResult { get; set; }
            public DateTime DateProp { get; set; }
            public string BureauFailure { get; set; }
        }

        [WebMethod(Description = "This method performs all the function necessary to save a new account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveNewAccount(SaveNewAccountParameters s, string CustomerId, decimal RFCreditLimit, decimal MaxSpend, ref int agreementNo,
            ref string propResult, ref DateTime dateProp, out string bureauFailure, out int storeCardTransRefNo, out string referralreasons, out string err) //IP - 15/03/11 - #3314 - CR1245 - Return referral reasons
        {
            Function = "BAccountManager::SaveNewAccount()";
            SqlConnection conn = null;
            err = "";
            //propResult = "";              //IP - 19/12/11 - #8931 - LW74456 
            bureauFailure = "";
            storeCardTransRefNo = 0;        //IP - 17/01/11 - Store Card

            BAccount acct = null;
            DataTable payMethodList = new DataTable(TN.PayMethodList);
            DataTable warrantyRenewalList = new DataTable(TN.WarrantyList);
            DataTable variableRates = new DataTable(TN.Rates);
            referralreasons = string.Empty; //IP - 15/03/11 - #3314 - CR1245

            if (s.LineItems != null)        // #15122   
            {
                new WarrantyRepository().SaveWarranty(s.LineItems);
            }

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            acct = new BAccount { User = s.User };

                            // There should be a list of one or more payments
                            //69678 DataSets must not be null
                            if (s.PayMethodSet != null)
                            {
                                if (s.PayMethodSet.Tables.Contains(TN.PayMethodList))
                                    payMethodList = s.PayMethodSet.Tables[TN.PayMethodList];
                            }

                            if (s.WarrantyRenewalSet != null)
                            {
                                if (s.WarrantyRenewalSet.Tables.Contains(TN.WarrantyList))
                                    warrantyRenewalList = s.WarrantyRenewalSet.Tables[TN.WarrantyList];
                            }

                            if (s.VariableRatesSet != null)
                            {
                                if (s.VariableRatesSet.Tables.Contains(TN.Rates))
                                    variableRates = s.VariableRatesSet.Tables[TN.Rates];
                            }

                            if (s.LineItems != null && s.CashAndGoReturn == true)        // #16339
                            {
                                // Collect Warranties for C&G items
                                foreach (XmlNode item in s.LineItems)
                                {
                                    if (Convert.ToString(item.Attributes[Tags.Type].Value) == IT.Stock)
                                    {
                                        var booking = new Blue.Cosacs.Model.LineitemBooking
                                        {
                                            AcctNo = s.AccountNumber,
                                            AgreementNo = agreementNo,
                                            ItemId = Convert.ToInt32(item.Attributes[Tags.ItemId].Value),
                                            StockLocation = Convert.ToInt16(item.Attributes[Tags.Location].Value),
                                            DelLocation = null,
                                            Quantity = Convert.ToInt16(item.Attributes[Tags.Quantity].Value)
                                        };

                                        //#16309 Message to cancel item on WarrantySale
                                        new WarrantyRepository().ReturnItem(booking.AcctNo, booking.AgreementNo,
                                             booking.ItemId, booking.StockLocation, Convert.ToInt32(-booking.Quantity), conn, trans, null);

                                        new WarrantyRepository().CollectCashAndGoWarranties(conn, trans, booking, Convert.ToInt16(item.Attributes[Tags.Quantity].Value), STL.Common.Static.Credential.UserId);          // #17692
                                    }
                                }
                            }
                            DAccount da = new DAccount();
                            int accountStatus = 0;
                            string invoiceNumber = "";

                            da.DisableSave(conn, trans, s.AccountNumber, out invoiceNumber, out accountStatus);
                            if (accountStatus == 1 && s.AccountType == "R" && invoiceNumber != "Legacy")
                            {
                                da.UpdateMaxSpendAndRFCreditLimit(conn, trans, CustomerId, RFCreditLimit, MaxSpend);
                            }
                            acct.SaveNewAccount(conn, trans, s.AccountNumber, s.BranchNo, s.AccountType, s.CODFlag, s.SalesPerson,
                                s.SOA, Convert.ToDecimal(s.AgreementTotal), Convert.ToDecimal(s.Deposit),
                                Convert.ToDecimal(s.ServiceCharge), s.LineItems, s.TermsType, s.NewBand,
                                s.CountryCode, s.DateFirst, Convert.ToDecimal(s.InstalAmount),
                                Convert.ToDecimal(s.FinalInstalment), s.PaymentMethod, s.Months,s.PrefDay,
                                s.TaxExempt, s.DutyFree, s.TaxAmount, s.Collection, s.BankCode,
                                s.BankAcctNo, s.ChequeNo, s.PayMethod, s.ReplacementXml, s.DtTaxAmount,
                                s.LoyaltyCardNo, s.ReScore, s.Tendered, s.GiftVoucherValue,
                                s.CourtsVoucher, s.VoucherReference, s.VoucherAuthorisedBy,
                                s.AccountNoCompany, s.PromoBranch, s.PaymentHolidays, payMethodList, s.DueDay,
                                s.ReturnAuthorisedBy, warrantyRenewalList, variableRates,
                                ref propResult, ref dateProp, ref agreementNo, out bureauFailure, s.User, s.ResetPropResult, s.Autoda, s.StoreCardAcctNo, s.StoreCardNumber, out storeCardTransRefNo, out referralreasons, s.ReadyAssist, s.ReadyAssistTermLength //#18603 - CR15594 //IP - 14/01/11 - Changed to accept StoreCard object //IP - 15/03/11 - #3314 - CR1245 - Return referral reasons
                                , s.ReviseAccount);// this flag is set to true when the account is revised

                            //IP - 15/06/12 - #10410 - Do not do the below for Paid & Taken.
                            if (!s.PaidAndTaken)
                            {
                                if (err.Length == 0)
                                {
                                    // #16281 new AccountRepository().BookingForReducedQty(conn, trans, s.AccountNumber, s.User);         //IP - 12/06/12 - #10328 - Warehouse & Deliveries - send cancellation and booking for reduced quantity.

                                    var hldProp = new AccountRepository().AgreementHoldProp(conn, trans, s.AccountNumber);            // #14522

                                    if (hldProp == "N" && (s.CollectionType == string.Empty || s.CollectionType == "I"))  //#17678       // #14522 - send bookings if Agr tot reduced
                                    {
                                        agreement = new BAgreement();
                                        agreement.User = STL.Common.Static.Credential.UserId;
                                        agreement.ClearProposal(conn, trans, s.AccountNumber, DASource.Auto, s.ReplacementXml);    //#18409       
                                    }

                                    if (warrantyRenewalList.Rows.Count > 0)
                                    {
                                        XmlNode warranties = s.LineItems.SelectSingleNode("//Item[@ContractNumber != '' and @Quantity != '0']");
                                        new WarrantyRepository().DeliverRenewalItem(conn, trans, new Blue.Cosacs.Model.LineitemBooking
                                        {
                                            AcctNo = s.AccountNumber,
                                            AgreementNo = agreementNo,
                                            ItemId = Convert.ToInt32(warrantyRenewalList.Rows[0]["ItemId"]),
                                            StockLocation = Convert.ToInt16(warrantyRenewalList.Rows[0]["StockLocn"]),
                                            DelLocation = Convert.ToInt16(warrantyRenewalList.Rows[0]["StockLocn"])
                                        }, STL.Common.Static.Credential.UserId);            // #17692

                                        //#16237 Deliver STAX
                                        BDelivery del = new BDelivery();
                                        DBranch branch = new DBranch();
                                        var transRefNo = branch.GetTransRefNo(conn, trans, Convert.ToInt16(warrantyRenewalList.Rows[0]["StockLocn"]));  //#15993
                                        var transValue = 0m;

                                        del.DeliverNonStocks(conn, trans, s.AccountNumber, s.AccountType,
                                            CountryParameterCache.GetCountryParameter<string>(CountryParameterNames.CountryCode)
                                            , Convert.ToInt16(warrantyRenewalList.Rows[0]["StockLocn"]), transRefNo, ref transValue, agreementNo);
                                    }

                                }
                            }
                            else
                            {
                                //#15655
                                //Loop through each item for Cash & Go
                                if (s.LineItems != null && s.CashAndGoReturn == false && s.Collection == false)  //#18526      // #16339
                                {
                                    foreach (XmlNode item in s.LineItems)
                                    {
                                        if (Convert.ToString(item.Attributes[Tags.Type].Value) == IT.Stock)
                                        {
                                            new WarrantyRepository().DeliverCGItem(conn, trans, s.AccountNumber, agreementNo,
                                                    Convert.ToInt32(item.Attributes[Tags.ItemId].Value), Convert.ToInt16(item.Attributes[Tags.Location].Value),
                                                    Convert.ToInt16(item.Attributes[Tags.Quantity].Value), s.ReplacementXml); //#18409 //#16206 //#15655          // #15181 Paid&Taken Warranties
                                        }
                                    }
                                }
                                else
                                {   // Collect Warranties for C&G items
                                    //foreach (XmlNode item in s.LineItems)
                                    //{
                                    //    if (Convert.ToString(item.Attributes[Tags.Type].Value) == IT.Stock)
                                    //    {
                                    //        var booking = new Blue.Cosacs.Model.LineitemBooking
                                    //                {AcctNo=s.AccountNumber,
                                    //                 AgreementNo=agreementNo,
                                    //                 ItemId= Convert.ToInt32(item.Attributes[Tags.ItemId].Value), 
                                    //                 StockLocation =  Convert.ToInt16(item.Attributes[Tags.Location].Value),
                                    //                 DelLocation= null,
                                    //                 Quantity= Convert.ToInt16(item.Attributes[Tags.Quantity].Value)
                                    //                 };

                                    //        new WarrantyRepository().CollectCashAndGoWarranties(conn, trans, booking, Convert.ToInt16(item.Attributes[Tags.Quantity].Value));
                                    //    }
                                    //}
                                    //#16607
                                    if (s.WarrantyRefunds.Rows.Count > 0 && s.CashAndGoReturn == true)
                                    {
                                        BBranch branch = new BBranch();

                                        foreach (DataRow row in s.WarrantyRefunds.Rows)
                                        {
                                            var type = (string)row[CN.RefundType] == "E" ? TransType.ElecWarrantyRecovery : TransType.FurnWarrantyRecovery;
                                            int transRefNo = branch.GetTransRefNo(conn, trans, (short)s.BranchNo);

                                            BTransaction t = new BTransaction(conn, trans, s.AccountNumber, s.BranchNo,
                                            transRefNo, Convert.ToDecimal(row["Refund"]), Credential.UserId, type,
                                             "", "", "", 0, CountryParameterCache.GetCountryParameter<string>(CountryParameterNames.CountryCode), DateTime.Now, "", agreementNo);

                                        }
                                    }
                                }

                                //if (s.HasInstallation)
                                //{
                                //    // #14432 P&T check for Installations and create Service Request
                                //    var Inst = new InstallationRepository();

                                //    var installations = (IEnumerable<RequestSubmit>)Inst.GetInstallationDataPaidAndTaken(conn, trans, s.AccountNumber);

                                //    new Chub().SubmitMany(installations, conn, trans);
                                //}

                                /* if(s.LineItems.SelectNodes("//Item[@Type ='Installation' and @Quantity = '0']").Count > 0)
                                     {

                                     }*/
                            }

                            trans.Commit();//Arun 
                            
                            DAccount ds = new DAccount();

                            string acctNo = s.AccountNumber;

                            PaymentWebApi api = new PaymentWebApi();
                            PostPaymentWebApi paymentapi = new PostPaymentWebApi();


                            if (invoiceNumber == "NewMambu")
                            {

                                string json = paymentapi.PostPayment(acctNo);

                            }
                            conn.Close();

                            //CR - Change to close service request when item is removed from sales order screen
                            if (s.LineItems.SelectNodes("//Item[@Type ='Installation' and @Quantity = '0']").Count > 0)
                            {
                                conn = new SqlConnection(Connections.Default);
                                    try
                                    {
                                    conn.Open();
                                    using (SqlTransaction trans1 = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                                    {
                                        foreach (XmlNode item in s.LineItems.ChildNodes)
                                        {
                                            if (item.ChildNodes.Count > 0 && !string.IsNullOrEmpty(item.ChildNodes[0].InnerXml))
                                            {
                                                foreach (XmlNode child in item.ChildNodes)
                                                {
                                                    XmlNodeList cnode = child.ChildNodes;
                                                    foreach (XmlNode l in cnode)
                                                    {
                                                        if (l.Attributes.Count > 0 && Convert.ToString(l.Attributes[Tags.Type].Value) == IT.Installation && Convert.ToString(l.Attributes[Tags.Quantity].Value) == "0")
                                                        {
                                                            BAccount acc = new BAccount();
                                                            acc.CloseServiceRequest(conn, trans, s.AccountNumber, Int32.Parse(Convert.ToString(item.Attributes[Tags.ItemId].Value)), Int32.Parse(Convert.ToString(l.Attributes[Tags.ItemId].Value)));
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                        trans1.Commit();
                                    }
                                    }
                                    catch (SqlException ex)
                                    {
                                        CatchDeadlock(ex, conn);
                                    }
                            }
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod(Description = "This method returns the account no, custid and cust name")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetAccountName(string accountNumber, string customerID, out string err)
        {
            Function = "BAccountManager::GetAccountName()";
            DataSet ds = null;
            err = "";
            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetAccountName(accountNumber, customerID);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method returns the account no, custid and cust name")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetCodesOnAccount(string accountNumber, out bool noSuchAccount, out string err)
        {
            Function = "BAccountManager::GetCodesOnAccount()";
            DataSet ds = null;
            err = "";
            noSuchAccount = false;
            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetCodesOnAccount(accountNumber, out noSuchAccount);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method adds codes to an account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string AddCodesToAccount(string accountNumber, DataSet ds)
        {

            SqlConnection conn = null;
            Function = "BAccountManager::AddCodesToAccount()";
            string err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            BAccount acct = new BAccount();
                            acct.AddCodesToAccount(conn, trans, accountNumber, ds);

                            trans.Commit();
                        }//commit the transaction
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return err;
        }

        [WebMethod(Description = "This method adds a letter to an account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string AddLetterToAccount(string accountNumber, DateTime letterDate, DateTime letterDue, string letterCode,
                            decimal addToValue, string excelGen)
        {

            SqlConnection conn = null;
            Function = "BLetter::Write()";
            string err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            BLetter letter = new BLetter();
                            letter.Write(conn, trans, accountNumber, letterDate, letterDue, letterCode, addToValue, excelGen);

                            trans.Commit();		//commit the transaction
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return err;
        }


        [WebMethod(Description = "This method returns a dataset containing the agreement details for a particular account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetAgreement(string accountNo, int agreementNumber, bool GR, out string err)
        {
            Function = "BAccountManager::GetAgreement()";
            DataSet ds = null;
            err = "";
            try
            {
                BAgreement agreement = new BAgreement();
                agreement.User = Credential.UserId;
                ds = agreement.GetAgreement(null, null, accountNo, agreementNumber, GR); //IP - 11/02/11 - Sprint 5.10 - #2978 - Added null, null for conn, trans
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }


        [WebMethod(Description = "This method adds a letter to an account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string GetInvoiceAccountDetails(string InvoiceNo, out string err)
        {
            Function = "BAccountManager::GetInvoiceAccountDetails()";
            string accountNo = string.Empty;
            err = "";
            try
            {
                BAgreement agreement = new BAgreement();
                agreement.User = Credential.UserId;
                accountNo = agreement.GetInvoiceAccountDetails(null, null, InvoiceNo); //IP - 11/02/11 - Sprint 5.10 - #2978 - Added null, null for conn, trans
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return accountNo;
        }

        [WebMethod(Description = "This method returns a dataset containing the details for an existing account which is being revised")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetAccountForRevision(string accountNo, int agreementNo, out string AccountBand, out string CustomerBand, out string err)
        {
            Function = "BAccountManager::GetAccountForRevision()";
            DataSet ds = null;
            err = "";
            AccountBand = "";
            CustomerBand = "";

            try
            {
                using (var conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        var acct = new BAccount();
                        ds = acct.GetAccountForRevision(accountNo, agreementNo, conn, trans);

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            var accountRow = ds.Tables[0].Rows[0];
                            var customerId = (string)accountRow["CustomerID"];

                            new BProposal().DetermineBands(conn, trans, accountNo, customerId, out AccountBand, out CustomerBand, "", 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod(Description = "This method returns an XmlNode containing the line items for a particular account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public XmlNode GetLineItems(string accountNo, int agreementNo, string accountType, string country, short promoBranch, out string err)
        {
            Function = "BAccountManager::GetLineItems()";
            XmlNode node = null;
            err = "";
            try
            {
                BItem item = new BItem();
                item.PromoBranch = promoBranch;
                node = item.GetLineItems(null, null, accountNo, accountType, country, agreementNo, InvVersion);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return node;
        }

        [WebMethod(Description = "This method returns an XmlNode containing the line items for a particular account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public XmlNode GetLineItemsWithVersion(string accountNo, int agreementNo, string accountType, string country, short promoBranch, int invVersion, out string err)
        {
            InvVersion = invVersion;
            //return GetLineItems(accountNo, agreementNo, accountType, country, promoBranch, out err);
            Function = "BAccountManager::GetLineItemsWithVersion()";
            XmlNode node = null;
            err = "";
            try
            {
                BItem item = new BItem();
                item.PromoBranch = promoBranch;
                node = item.GetLineItems(null, null, accountNo, accountType, country, agreementNo, InvVersion, true);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return node;
        }

        //Suvidha CR 2018-13
        [WebMethod(Description = "This method returns an XmlNode containing the line items for a particular account")]
        [SoapHeader("authentication")]
        public XmlNode GetSalesOrderLineItems(string accountNo, int agreementNo, string accountType, string country, short promoBranch, string agreementInvoiceNumber, out string err)
        {
            Function = "BAccountManager::GetSalesOrderLineItems()";
            XmlNode node = null;
            err = "";
            try
            {
                BItem item = new BItem();
                item.PromoBranch = promoBranch;
                node = item.GetSalesOrderLineItems(null, null, accountNo, accountType, country, agreementNo, agreementInvoiceNumber);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return node;            
        }

        [WebMethod(Description = "This method returns the quantity delivered and scheduled for delivery for a particular line item")]
        [SoapHeader("authentication")]
        public bool HasReturnsItens(
            string accountNo,
            int agreementNo,
            int itemID,
            short location,
            string contractNo,
            int parentItemID)
        {
            using (var ctx = Blue.Cosacs.Context.Create())
            {
                return ctx.LineItemHasItemsToCollectView
                    .Where(p => p.acctno == accountNo && p.agrmtno == agreementNo && p.ItemID == itemID && p.stocklocn == location && p.contractno == contractNo && p.ParentItemID == parentItemID)
                    .Count() > 0;
            }
        }

        [WebMethod(Description = "This method returns the quantity delivered and scheduled for delivery for a particular line item")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int GetItemsDeliveredAndScheduled(string accountNo,
            int agreementNo,
            //string itemNo,
            int itemID,                                     //IP - 18/05/11 - CR1212 - #3627 - changed to use itemID rather than itemNo
            short location,
            string contractNo,
            int parentItemID,                               //IP - 23/06/11 - CR1212 - RI - #4067
            out double delivered,
            out double scheduled,
            out bool repo,                                  //IP - 26/06/12 - #10516
            out string err)
        {
            Function = "BAccountManager::GetItemsDeliveredAndScheduled()";
            err = "";
            delivered = 0;
            scheduled = 0;
            repo = false;                                    //IP - 26/06/12 - #10516
            try
            {
                // TODO uat363 rdb this really needs to take parentPRoductNo as parameter 
                // from client
                BItem item = new BItem();
                item.GetItemsDeliveredAndScheduled(accountNo,
                    agreementNo,
                    //itemNo,
                    itemID,                                 //IP - 18/05/11 - CR1212 - #3627 - changed to use itemID rather than itemNo                                
                    location,
                    contractNo,
                    out delivered,
                    out scheduled, parentItemID, out repo);  //IP - 26/06/12 - #10516  //IP - 18/05/11 - CR1212 - #3627 - replaced "" with 0 for parentItemID //IP - 23/06/11 - CR1212 - RI - #4067 - parentItemID now passed in

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0;
        }

        [WebMethod(Description = "This method returns a dataset containing the scheduled deliveries for this item")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetScheduledDeliveriesForItem(string accountNo,
            int agreementNo,
            //string itemNo,
            int itemID,                                     //IP/NM - 18/05/11 -CR1212 - #3627        
            short location,
            out string err)
        {
            Function = "BAccountManager::GetScheduledDeliveriesForItem()";
            DataSet ds = null;
            err = "";
            try
            {
                BItem item = new BItem();
                ds = item.GetScheduledDeliveriesForItem(accountNo, agreementNo, itemID, location);          //IP/NM - 18/05/11 -CR1212 - #3627 
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method deletes a set of delivery schedules")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string DeleteDeliverySchedules(DataSet ds, bool resetHoldProp, out string err)
        {

            SqlConnection conn = null;
            Function = "BAccountManager::DeleteDeliverySchedules()";
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            //make sure we have a valid lock on the account
                            BAccount acct = new BAccount();
                            acct.ValidLock(conn, trans, (string)ds.Tables[0].Rows[0]["AccountNo"], STL.Common.Static.Credential.UserId);

                            BSchedule sched = new BSchedule();
                            sched.User = STL.Common.Static.Credential.UserId;
                            sched.DeleteDeliverySchedules(conn, trans, ds, resetHoldProp);

                            trans.Commit();		//commit the transaction
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return err;
        }

        [WebMethod(Description = "This method loads a list of schedules (delivery notes) by buffno")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet Schedule_GetByBuffNo(int BranchNo, int BuffNo, out string err)
        {
            Function = "BAccountManager::Schedule_GetByBuffNo()";
            err = "";
            DataSet ds = null;
            try
            {
                BSchedule sched = new BSchedule();
                ds = sched.GetByBuffNo(BranchNo, BuffNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method saves a list of item deliveries and transactions for Immediate Delivery")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string Delivery_DeliverImmediately(string accountNumber, DateTime DateDel, DataSet DeliveryItems, string countryCode, string branchNo, ref bool allItemsCollected, out string err)
        {
            SqlConnection conn = null;

            Function = "BAccountManager::Delivery_DeliverImmediately()";
            string userMsg = "";
            err = "";
            allItemsCollected = false;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            //make sure we have a valid lock on the account
                            BAccount acct = new BAccount();
                            acct.ValidLock(conn, trans, accountNumber, STL.Common.Static.Credential.UserId);

                            BDelivery delivery = new BDelivery();
                            delivery.User = STL.Common.Static.Credential.UserId;
                            delivery.DeliverImmediately(conn, trans, accountNumber, STL.Common.Static.Credential.UserId.ToString(), DateDel, DeliveryItems, countryCode, (short)Convert.ToInt32(branchNo), ref allItemsCollected, out userMsg);

                            if (userMsg.Length == 0)
                                trans.Commit();		//commit the transaction
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }

            // Return either a SQL error or a business level user msg
            err = (err.Length == 0) ? userMsg : err;
            return err;
        }



        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExstension)
#endif
        // CR914 rdb 16/10/07 -Immediate Deivery Failed
        public string Delivery_DeliverImmediatelyFailed(string accountNumber, DateTime DateDel, DataSet DeliveryItems, string countryCode, string branchNo, out string err)
        {
            SqlConnection conn = null;

            Function = "BAccountManager::Delivery_DeliverImmediatelyFailed()";
            string userMsg = "";
            err = "";
            //allItemsCollected = false;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            //make sure we have a valid lock on the account
                            BAccount acct = new BAccount();
                            acct.ValidLock(conn, trans, accountNumber, STL.Common.Static.Credential.UserId);

                            BDelivery delivery = new BDelivery();
                            delivery.User = STL.Common.Static.Credential.UserId;
                            delivery.DeliverImmediatelyFailed(conn, trans, accountNumber, Convert.ToString(User.Identity.Name), DateDel, DeliveryItems, countryCode, (short)Convert.ToInt32(branchNo), out userMsg);

                            if (userMsg.Length == 0)
                                trans.Commit();		//commit the transaction
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }

            // Return either a SQL error or a business level user msg
            err = (err.Length == 0) ? userMsg : err;
            return err;
        }

        [WebMethod(Description = "test")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int DeadlockTest1(out string err)
        {

            Function = "BAccountManager::DeadlockTest()";
            err = "";
            int result = 0;
            SqlConnection conn = new SqlConnection(Connections.Default);
            conn.Open();
            using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {

                    do
                    {
                        try
                        {
                            //start a transaction

                            string query = "update lineitem set itemno = itemno where itemno = '413074' and stocklocn = 870";
                            SqlCommand command1 = new SqlCommand(query, conn);
                            command1.Transaction = trans;
                            command1.ExecuteNonQuery();

                            query = "update accttype set accttype = accttype where accttype = 'H'";
                            SqlCommand command2 = new SqlCommand(query, conn);
                            command2.Transaction = trans;
                            command2.ExecuteNonQuery();

                            trans.Commit();						//commit the transaction

                            result = 0;
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Number == Deadlock &&
                                conn.State == ConnectionState.Open &&
                                retries < maxRetries)
                            {
                                logMessage("DeadLock detected. Retry number: " + retries, STL.Common.Static.Credential.UserId.ToString(), EventLogEntryType.Information);
                                retries++;
                                trans.Rollback();
                                result = (int)Return.Deadlock;
                            }
                            else
                                throw ex;
                        }

                    } while (result == (int)Return.Deadlock && retries < maxRetries);

                    if (result == (int)Return.Deadlock)
                        throw (new STLException("Deadlocked!"));

                }
                catch (Exception ex)
                {
                    err = ex.Message;
                    logException(ex, Function);
                    if (conn.State == ConnectionState.Open)	//Some exceptions cause connection to close
                        trans.Rollback();					//therefore aborting transaction
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            return 0;
        }

        [WebMethod(Description = "test")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int DeadlockTest2(out string err)
        {

            SqlConnection conn = null;
            Function = "BAccountManager::DeadlockTest2()";
            err = "";
            int result = 0;
            try
            {
                conn = new SqlConnection(Connections.Default);
                conn.Open();

                do
                {
                    try
                    {
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            string query = "update accttype set accttype = accttype where accttype = 'H'";
                            SqlCommand command1 = new SqlCommand(query, conn);
                            command1.Transaction = trans;
                            command1.ExecuteNonQuery();

                            query = "update lineitem set itemno = itemno where itemno = '413074' and stocklocn = 870";
                            SqlCommand command2 = new SqlCommand(query, conn);
                            command2.Transaction = trans;
                            command2.ExecuteNonQuery();

                            trans.Commit();		//commit the transaction
                        }
                        result = 0;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock &&
                            conn.State == ConnectionState.Open &&
                            retries < maxRetries)
                        {
                            logMessage("DeadLock detected. Retry number: " + retries, STL.Common.Static.Credential.UserId.ToString(), EventLogEntryType.Information);
                            retries++;
                            //trans.Rollback();
                            result = (int)Return.Deadlock;
                        }
                        else
                            throw ex;
                    }
                } while (result == (int)Return.Deadlock && retries < maxRetries);

            }
            catch (Exception ex)
            {
                err = ex.Message;
                logException(ex, Function);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return 0;
        }

        [WebMethod(Description = "This method deletes a line item from an account, child items will also be deleted, this is done in the stored proc")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int DeleteLineItem(string accountNo,
            int agreementNo,
            XmlNode toDelete,
            out string err)
        {

            SqlConnection conn = null;
            Function = "BAccountManager::DeleteLineItem()";
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            //make sure we have a valid lock on the account
                            BAccount acct = new BAccount();
                            acct.ValidLock(conn, trans, accountNo, STL.Common.Static.Credential.UserId);

                            BItem item = new BItem();
                            item.DeleteLineItem(conn, trans, accountNo, agreementNo, toDelete);

                            trans.Commit();		//commit the transaction
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod(Description = "this method returns all the transactions for an account.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetTransactions(string accountNo, out string err)
        {
            Function = "BAccountManager::GetTransactions()";
            DataSet ds = null;
            err = "";
            try
            {
                BAccount transactions = new BAccount();
                ds = transactions.GetTransactions(accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod(Description = "This method will return the customer ID linked to a particular account number")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string GetLinkedCustomerID(string accountNo, out string err)
        {
            Function = "WAccountManager::GetLinkedCustomerID()";
            err = "";
            string custid = "";
            try
            {


                using (SqlConnection conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        var acct = new BAccount();
                        custid = acct.GetLinkedCustomerID(accountNo, conn, trans);

                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return custid;
        }

        [WebMethod(Description = "This method will return the customer ID linked to a particular account number and type")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string GetLinkedCustomerIDbyType(string accountNo, string relationship, out string err)
        {
            Function = "WAccountManager::GetLinkedCustomerID()";
            err = "";
            string custid = "";
            try
            {


                using (SqlConnection conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        BAccount acct = new BAccount();
                        custid = acct.GetLinkedCustomerIDbyType(accountNo, relationship, conn, trans);

                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return custid;
        }


        [WebMethod(Description = "This method will add a customer to an account number")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool AddCustomerToAccount(string accountNo, string customerID, string relationship, string accountType, out bool rescore, out string err, int userid = 0)
        {
            Function = "WAccountManager::AddCustomerToAccount()";
            err = "";
            SqlConnection conn = null;

            bool exists = false;
            rescore = false;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BAccount acct = new BAccount();
                            acct.User = STL.Common.Static.Credential.UserId;

                            //make sure we have a valid lock on the account
                            acct.ValidLock(conn, trans, accountNo, STL.Common.Static.Credential.UserId);
                            exists = acct.AddCustomerToAccount(conn, trans, accountNo, customerID, relationship, accountType, out rescore);

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return exists;
        }
        // CR-02 Implement Audit on Customer account linking-2/07/2020   
        [WebMethod(Description = "This method will add a Audit trail on customer linking")]
        [SoapHeader("authentication")]
        public bool AuditCustomerAccountLinking(string accountNo, string OldcustomerID, string NewcustomerID, string UserName, out string err)
        {
            Function = "WAccountManager::AuditCustomerAccountLinking()";
            err = "";
            bool exists = false;
            try
            {

                var document = "AccountLinking";
                EventStore.Instance.Log(new
                {
                    CustomerCodeLinkedFrom= OldcustomerID,
                    CustomerCodeAccountLinkedTo = NewcustomerID,
                    AccountNumberLinked = accountNo,
                    AccountLinkedDate = DateTime.Now,
                    AuthorisedUserId = UserName,
                }, document, EventCategory.AccountLinking
                               , new { empeeno = Credential.UserId });

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return exists;

        }

        [WebMethod(Description = "This method will verify that an account number entered as part of capturing a manual sale is valid")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int ValidateAccountNumber(string accountNo, string countryCode, string accountType, out string err)
        {
            Function = "WAccountManager::ValidateAccountNumber()";
            err = "";
            try
            {
                BAccount acct = new BAccount();
                if (!acct.ValidateAccountNumber(accountNo, countryCode, accountType))
                    err = "Account Number is not valid";
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0;
        }


        //CR906 rdb 06/09/07 added isLoan
        [WebMethod(Description = "This method creates a new RF account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string CreateRFAccount(string countryCode, short branchNo, string customerID, int user, bool isLoan, ref Blue.Cosacs.Shared.CashLoanDetails det, out bool rescore,
                                        out bool reOpenS1, out bool reOpenS2, out string err)
        {
            Function = "BAccountManager::CreateRFAccount()";
            SqlConnection conn = null;
            rescore = false;
            reOpenS1 = false;
            reOpenS2 = false;
            err = "";


            BAccount acct = null;
            string accountNo = "";
            string auditSource = string.Empty;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            acct = new BAccount();
                            acct.User = STL.Common.Static.Credential.UserId;

                            if (det == null || (det != null && det.accountNo == "000000000000"))        // or cash from cash loan screen
                            {
                                accountNo = acct.CreateRFAccount(conn, trans, countryCode, branchNo, customerID, user, isLoan, out rescore);


                                reOpenS1 = acct.ReOpenS1;
                                reOpenS2 = acct.ReOpenS2;

                                if (accountNo.Length == 0)
                                    err = "Unable to create RF account.";
                            }

                            if (det != null)
                            {
                                var dateprop = new DateTime();
                                var unclearStage = "";
                                var propResult = "";
                                var points = 0;
                                var newaccountNo = "";

                                if (det.accountNo == "000000000000")
                                {
                                    auditSource = AS.NewAccount;
                                    det.accountNo = accountNo.Replace("-", "");
                                }
                                else
                                {
                                    auditSource = AS.Revise;
                                    det.accountNo = det.accountNo.Replace("-", "");
                                    accountNo = det.accountNo;
                                }

                                //BAccount ba = new BAccount();     // #8877 moved 
                                //ba.User = STL.Common.Static.Credential.UserId;
                                //ba.InsertCashLoanItem(conn, trans, det, auditSource);

                                DProposal pr = new DProposal();
                                pr.GetUnclearedStage(conn, trans, det.accountNo, ref newaccountNo, ref unclearStage, ref dateprop, ref propResult, ref points);   // #8487
                                det.unclearStage = unclearStage;
                                det.dateprop = dateprop;

                                BAccount ba = new BAccount();                   // #8877 
                                ba.User = STL.Common.Static.Credential.UserId;
                                ba.InsertCashLoanItem(conn, trans, det, auditSource, countryCode, branchNo);

                                // #8670 if loan is referred insert referral flag
                                if (det.loanStatus == "R")
                                {
                                    DProposalFlag pf = new DProposalFlag();
                                    pf.EmployeeNoFlag = STL.Common.Static.Credential.UserId;
                                    pf.CustomerID = det.custId;
                                    pf.DateProp = dateprop;
                                    pf.DateCleared = DateTime.MinValue.AddYears(1899);
                                    pf.CheckType = "R";
                                    pf.Save(conn, trans, det.accountNo);
                                }
                            }

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return accountNo;
        }

        //CR906 rdb 06/09/07 added isLoan
        [WebMethod(Description = "This method creates a new Customer account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string CreateCustomerAccount(string countryCode, short branchNo, string customerID, string accountType, out bool rescore, int user, bool isLoan, out string err)
        {
            Function = "BAccountManager::CreateCustomerAccount()";
            SqlConnection conn = null;
            rescore = false;
            err = "";

            BAccount acct = null;
            string accountNo = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {

                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            acct = new BAccount();
                            acct.User = STL.Common.Static.Credential.UserId;

                            if (accountType == AT.StoreCard)
                            {


                                var SCardRep = new StoreCardRepository();

                                var cust = SCardRep.ExistingAccount(customerID, AT.StoreCard, conn, trans);

                                if (cust != null && cust.acctno != null)
                                {
                                    this.ReverseCancellation(cust.acctno, "", "Reverse ", out err);
                                    accountNo = cust.acctno;
                                }

                            }
                            if (accountNo == "")
                                accountNo = acct.CreateCustomerAccount(conn, trans, countryCode, branchNo, customerID, accountType, user, isLoan, out rescore, "NewApp");

                            if (accountNo.Length == 0)
                                err = "Unable to create customer account.";

                            if (accountType == AT.StoreCard)
                            {
                                var storeCardNew = new StoreCardNew
                                {
                                    AcctNo = accountNo.Replace("-", String.Empty),
                                    CustId = customerID,
                                    Source = "NewApp",
                                    User = user
                                };
                                new StoreCardRepository().CreateandScore(storeCardNew, conn, trans);
                            }
                            trans.Commit();
                        }
                        break;


                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return accountNo;
        }

        //IP - 01/05/08 - UAT(362) - Commented out as using below method.
        //        [WebMethod(Description = "This method will calculate the quantity of sales tax that should be added to an account")]
        //        [SoapHeader("authentication")]
        //#if(TRACE)
        //        [TraceExtension]
        //#endif
        //        public decimal CalculateSalesTax(string countryCode, XmlNode lineItems,
        //            string accountType, bool dutyFree,
        //            bool includeWarranty,
        //            ref decimal chargeableAdmin,
        //            ref decimal chargeable, out string err)
        //        {
        //            Function = "WAccountManager::CalculateSalesTax()";
        //            err = "";
        //            decimal tax = 0;
        //            chargeable = 0;
        //            chargeableAdmin = 0;
        //            try
        //            {
        //                BAccount acct = new BAccount();
        //                tax = acct.CalculateSalesTax(countryCode, lineItems, accountType, dutyFree, includeWarranty, ref chargeableAdmin, ref chargeable);
        //            }
        //            catch (Exception ex)
        //            {
        //                Catch(ex, Function, ref err);
        //            }
        //            return tax;
        //        }

        //IP - 30/04/08 - UAT(362) v 5.1  
        //IP - 01/05/08 - UAT(362) - Added 'ref decimal taxWarrantyOnCredit'
        [WebMethod(Description = "This method will calculate the quantity of sales tax that should be added to an account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public decimal CalculateSalesTax(string countryCode, XmlNode lineItems,
            string accountType, bool dutyFree,
            bool includeWarranty, bool warrantiesOnCredit,
            ref decimal chargeableAdmin,
            ref decimal chargeable, ref decimal taxWarrantyOnCredit, out string err)
        {
            Function = "WAccountManager::CalculateSalesTax()";
            err = "";
            decimal tax = 0;
            chargeable = 0;
            chargeableAdmin = 0;
            try
            {
                BAccount acct = new BAccount();
                tax = acct.CalculateSalesTax(countryCode, lineItems, accountType, dutyFree, includeWarranty, warrantiesOnCredit, ref chargeableAdmin, ref chargeable, ref taxWarrantyOnCredit);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return tax;
        }


        [WebMethod(Description = "This Method will check and perform an necessary collections when deleting a warranty")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int DeleteWarranty(XmlNode parent, XmlNode child, string accountNo, short branchNo, string countryCode, int agreementNo, out string err)
        {
            Function = "BAccountManager::DeleteWarranty()";
            SqlConnection conn = null;
            err = "";

            BAccount acct = null;
            BItem lineItem = null;
            decimal transValue = 0;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            acct = new BAccount();
                            lineItem = new BItem();
                            acct.User = STL.Common.Static.Credential.UserId;
                            DBranch b = new DBranch();
                            int refNo = b.GetTransRefNo(conn, trans, branchNo);
                            acct.BuildNonStock(conn, trans, parent, child, lineItem, accountNo, branchNo, refNo, countryCode, ref transValue, agreementNo);
                            if (Math.Abs(transValue) > 0.01M)
                            {
                                string type = transValue > 0 ? "DEL" : "GRT";
                                BTransaction t = new BTransaction(conn, trans, accountNo, branchNo, refNo, transValue, acct.User, type, "", "", "", 0, countryCode, DateTime.Now, "", agreementNo);
                            }
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod(Description = "This method brings back the current branch name and address")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetBranchAddress(int branchNo, int updhissn, out string err)
        {
            Function = "BAccountManager::GetBranchAddress()";
            err = "";
            DataSet ds = null;
            try
            {
                BBranch branch = new BBranch();
                ds = branch.GetBranchAddress(branchNo, updhissn);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method will attempt to delete an RF account if credit has been refused, but only if the status is 0.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool CancelRFAccount(string accountNo, string customerID, out string err)
        {
            Function = "WAccountManager::CancelRFAccount()";
            SqlConnection conn = null;
            err = "";

            BAccount acct = null;
            bool cancelled = false;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            acct = new BAccount();
                            acct.User = STL.Common.Static.Credential.UserId;
                            cancelled = acct.CancelRFAccount(conn, trans, accountNo, customerID);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return cancelled;
        }

        [WebMethod(Description = "This method will cancel an account.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool CancelAccount(string accountNo, string customerID, int branch,
            string code, decimal balance, string countryCode, string notes, short ContactMonths, out string err)
        {
            Function = "WAccountManager::CancelAccount()";
            SqlConnection conn = null;
            err = "";

            BAccount acct = null;
            bool cancelled = false;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            acct = new BAccount();
                            acct.User = STL.Common.Static.Credential.UserId;
                            cancelled = acct.CancelAccount(conn, trans, accountNo, customerID,
                                branch, code, balance, countryCode, notes, ContactMonths);
                            if (accountNo.Substring(3, 1) == "9") //Cancel StoreCard Account
                                new StoreCardRepository().CancelAccount(new View_StoreCardHistory
                                {
                                    Acctno = accountNo,
                                    Reason = "Entire Account Cancelled",
                                    Notes = notes,
                                    StatusCode = StoreCardAccountStatus_Lookup.Cancelled.Code,
                                    Code = StoreCardAccountStatus_Lookup.Cancelled.Code,
                                    Empeeno = STL.Common.Static.Credential.UserId,
                                    ContactMonths = ContactMonths
                                }, conn, trans);

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return cancelled;
        }

        [WebMethod(Description = "This method will convert an RF account to an HP account if the customer is declined RF credit. Only is status is 0")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int ConvertRFToHP(string accountNo, string customerID, string country, DateTime dateProp, out string err)
        {
            Function = "WAccountManager::ConvertRFToHP()";
            SqlConnection conn = null;
            err = "";

            BAccount acct = null;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            acct = new BAccount();
                            acct.User = STL.Common.Static.Credential.UserId;
                            acct.ConvertRFToHP(conn, trans, accountNo, customerID, country, dateProp);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod(Description = "This method brings back the current branch name and address")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetPaymentCardDetails(string customerID, string accountNo, string branchNo, out string err)
        {
            Function = "WAccountManager::GetPaymentCardDetails()";
            err = "";
            DataSet ds = null;
            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetPaymentCardDetails(customerID, accountNo, branchNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method clears flags for the DA screen")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int ClearFlag(string custID, string chkType, DateTime dateProp, bool reOpen, string acctno, out string err)
        {
            Function = "BProposal::ClearFlag()";
            SqlConnection conn = null;
            err = "";

            BProposal prop = null;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            prop = new BProposal();
                            prop.User = STL.Common.Static.Credential.UserId;
                            prop.ClearFlag(conn,
                                trans,
                                custID,
                                chkType,
                                dateProp,
                                reOpen, acctno);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod(Description = "This method returns the paid and taken account number for a specific branch")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string GetPaidAndTakenAccount(string branchNo, out string err)
        {
            Function = "WAccountManager::GetPaidAndTakenAccount()";
            err = "";
            string acctno = "";
            try
            {
                BAccount acct = new BAccount();
                acctno = acct.GetPaidAndTakenAccount(null, null, branchNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return acctno;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int GetDeliveryNotes(String acctno, int user, int branch, string addr1, out String err,
            ref int buffno, ref DataSet DeliveryNotes)
        {
            Function = "WStaticDataManager::GetDeliveryNotes()";
            SqlConnection conn = null;

            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BAccount account = new BAccount();
                            //DeliveryNotes = account.GetDeliveryNotes(conn, trans, acctno, user, branch, addr1, ref buffno);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetDeliveryAccounts(String acctno, int user, int branch, out String err, ref DateTime TimeLocked)
        {
            Function = "WStaticDataManager::GetDeliveryAccounts()";
            SqlConnection conn = null;

            err = "";
            DataSet fields = null;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BAccount account = new BAccount();
                            fields = account.GetDeliveries(conn, trans, acctno, user, branch, ref TimeLocked);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return fields;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int UnlockAccountsLockedAt(int user, DateTime TimeLocked, out String err)
        {
            BAccount acct = null;
            SqlConnection conn = null;

            err = "";
            int status = 0;

            try
            {
                conn = new SqlConnection(Connections.Default);

                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            acct = new BAccount();
                            status = acct.UnlockAccountsLockedAt(user, TimeLocked);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }

            return status;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveRepossArrears(decimal arrears, decimal repossvalue, string acctno, out String err)
        {
            Function = "BAccount::SaveRepossArrears()";
            SqlConnection conn = null;

            BAccount acct = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            acct = new BAccount();
                            acct.SaveRepossArrears(conn, trans, arrears, repossvalue, acctno);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetReposessionAndRedelivery(string accountNo, out String err)
        {
            Function = "BAccountManager::GetReposessionAndRedilivery()";
            DataSet ds = null;
            err = "";
            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetReposessionAndRedelivery(accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveDelivery(DataSet ds, out String err)
        {
            Function = "BAccount::SaveDelivery()";
            SqlConnection conn = null;

            BDelivery del = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            del = new BDelivery();
                            del.Save(conn, trans, ds, 0);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveSchedule(DataSet ds, short branchNo, string countryCode, string accountType,
            string accountNo, int agreementNo, out String err, int? serviceRequestNo = null)        //#11989
        {
            Function = "BAccount::SaveSchedule()";
            SqlConnection conn = null;

            BSchedule sched = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            //new WarrantyRepository().ReturnWarranty(ds);      //#16276

                            sched = new BSchedule();
                            sched.User = STL.Common.Static.Credential.UserId;
                            sched.SaveSchedule(conn, trans, ds, branchNo, countryCode, accountType,
                                accountNo, agreementNo, true, serviceRequestNo);        //#11989
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int CancelCollectionNote(string acctNo, int agrmtNo,
            int itemID, string contractNo, short stockLocn, int buffNo,                 //IP/NM - 18/05/11 -CR1212 - #3627 
            int buffBranchNo, short branchNo, int lineItemId, out string err)           //#10544
        {
            Function = "BAccount::CancelCollectionNote()";
            SqlConnection conn = null;

            BItem lineitem = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            lineitem = new BItem();
                            lineitem.User = STL.Common.Static.Credential.UserId;
                            lineitem.CancelCollectionNote(conn, trans, acctNo, agrmtNo,
                                itemID, contractNo, stockLocn, buffNo, buffBranchNo, branchNo, lineItemId);  //#10544            //IP/NM - 18/05/11 -CR1212 - #3627 
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int UpdateSchedules(DataSet ds, out String err)
        {
            Function = "BAccount::UpdateSchedules()";
            SqlConnection conn = null;

            BSchedule sched = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            sched = new BSchedule();
                            sched.User = STL.Common.Static.Credential.UserId;
                            sched.UpdateSchedule(conn, trans, ds);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int UpdateScheduleForPicklist(DataSet ds, int branchNo, string pickListType,
                                                out String err)
        {
            Function = "BAccount::UpdateScheduleForPicklist()";
            SqlConnection conn = null;

            BSchedule sched = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            sched = new BSchedule();
                            sched.User = STL.Common.Static.Credential.UserId;
                            sched.UpdateScheduleForPicklist(conn, trans, ds, branchNo, pickListType);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int ScheduleAssignNewBufferNo(string acctno, int itemId, short stockLocn,
            int buffNo, int newBuffNo, out String err)
        {
            Function = "BAccount::ScheduleAssignNewBufferNo()";
            SqlConnection conn = null;

            BSchedule sched = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            sched = new BSchedule();
                            sched.User = STL.Common.Static.Credential.UserId;
                            sched.ScheduleAssignNewBufferNo(conn, trans, acctno, itemId, stockLocn, buffNo, newBuffNo);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetForRepo(string accountNumber, out string accountType, out string err)
        {
            Function = "BAccountManager::GetForRepo()";
            DataSet ds = null;
            err = "";
            accountType = "";
            try
            {
                BDelivery del = new BDelivery();
                ds = del.GetForRepo(accountNumber, out accountType);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveItems(DataSet ds, string type, double delQty, out String err)
        {
            Function = "BAccount::SaveItems()";
            SqlConnection conn = null;

            BItem item = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            item = new BItem();
                            item.SaveItems(conn, trans, ds, type, delQty);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveRepoDetails(string type, decimal amount, string acctNo,
            decimal oustBalance, int branchNo, int user, string countryCode,
            DataSet ds, string accountType, out string err)
        {
            Function = "BAccount::SaveRepoDetails()";
            SqlConnection conn = null;

            BTransaction transact = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            new WarrantyRepository().ReturnWarranty(ds, conn, trans);
                            transact = new BTransaction();
                            transact.SaveRepoDetails(conn, trans, type, amount, acctNo, oustBalance,
                                 branchNo, user, countryCode, ds, accountType);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int GetItemCount(int itemId, short location, out string err)     // RI jec 19/05/11
        {
            Function = "BAccountManager::GetItemCount()";
            int rowCount = 0;
            err = "";
            try
            {
                BItem item = new BItem();
                rowCount = item.GetItemCount(itemId, location);            // RI jec 19/05/11
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return rowCount;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetItemsForAccount(string acctno, out string err)
        {
            Function = "BAccountManager::GetItemsForAccount()";
            DataSet ds = null;
            err = "";
            try
            {
                BItem item = new BItem();
                ds = item.GetItemsForAccount(acctno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetDeliveries(string acctno, int agreementNo, out string err)
        {
            Function = "BAccountManager::GetDeliveries()";
            DataSet ds = null;
            err = "";
            try
            {
                BDelivery del = new BDelivery();
                ds = del.GetDeliveries(acctno, agreementNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetCollections(short stockLocn, out string err)
        {
            Function = "BAccountManager::GetCollections()";
            DataSet ds = null;
            err = "";
            try
            {
                BSchedule sched = new BSchedule();
                ds = sched.GetCollections(stockLocn);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int GetBuffNo(short branchNo, out string err)
        {
            Function = "BAccount::GetBuffNo()";
            BBranch branch = null;
            err = "";
            int buffNo = 0;
            try
            {
                using (var conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        branch = new BBranch();
                        buffNo = branch.GetBuffNo(conn, trans, branchNo);
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return buffNo;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int GetPaymentCardType(short branchNo, DateTime dtDateAcctOpen, out string err)
        {
            Function = "BBranch::Get()";
            BBranch branch = null;
            int PaymentCardType = 0;
            err = "";
            try
            {
                branch = new BBranch();
                PaymentCardType = branch.GetPaymentCardType(branchNo, dtDateAcctOpen);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return PaymentCardType;
        }


        [WebMethod(Description = "This method deletes a set of delivery schedules")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string DeleteSchedule(DataSet ds, out string err)
        {

            SqlConnection conn = null;
            Function = "BAccountManager::DeleteSchedule()";
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BSchedule sched = new BSchedule();
                            sched.DeleteSchedule(conn, trans, ds);

                            trans.Commit();		//commit the transaction
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return err;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public string AutoWarranty(string branchNo, out string err)
        {
            Function = "BAccountManager::AutoWarranty()";
            string contractNo = "";
            err = "";
            try
            {
                BItem item = new BItem();
                item.AutoWarranty(branchNo, out contractNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return contractNo;
        }

        [WebMethod(Description = "This method updates the fintrans table to reflect whether a transaction has been printed")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string SetPaymentCardPrinted(string accountNo,
            int transRefNo,
            DateTime transactionDate,
            string printed,
            int startLine,
            out string err)
        {

            SqlConnection conn = null;
            Function = "WAccountManager::SetPaymentCardPrinted()";
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BTransaction t = new BTransaction();
                            t.SetPaymentCardPrinted(conn, trans, accountNo,
                                transRefNo, transactionDate,
                                printed, startLine);

                            trans.Commit();		//commit the transaction
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return err;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetCashAndGoAccts(long BuffNo, int BranchNo, DateTime From, DateTime To,
            bool searchWarrantyReturns, out string err)
        {
            Function = "BAccountManager::GetCashAndGoAccts()";
            DataSet ds = null;
            err = "";

            try
            {
                BDelivery del = new BDelivery();
                ds = del.GetCashAndGoAccts(BuffNo, BranchNo, From, To, searchWarrantyReturns);

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;

        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetAccountsAllocated(int empeeNo, string branchOrAcctFilter, short acctNoSearch, out string err)
        {
            Function = "BAccountManager::GetAccountsAllocated()";
            DataSet ds = null;
            err = "";

            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetAccountsAllocated(empeeNo, branchOrAcctFilter, acctNoSearch);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetStrategyAccountsAllocated(int empeeNo, string branchOrAcctFilter, short acctNoSearch, string strategy, out string err)
        {
            Function = "BAccountManager::GetAccountsAllocated()";
            DataSet ds = null;
            err = "";

            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetStrategyAccountsAllocated(empeeNo, branchOrAcctFilter, acctNoSearch, strategy);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetBailActions(string accountNo, out string err)
        {
            Function = "BAccountManager::GetBailActions()";
            DataSet ds = null;
            err = "";

            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetBailActions(accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveBailActions(string acctNo, int empeeNo, string code, string notes, DateTime dateDue,
            double actionValue, DateTime spaDateExpiry, string spaReasonCode, double spaInstal, DateTime reminderDateTime, bool cancelOutstandingReminders, string callingSource, out string err)
        {
            Function = "BAccount::SaveBailActions()";
            SqlConnection conn = null;

            BAccount acct = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();

                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            acct = new BAccount();
                            acct.User = STL.Common.Static.Credential.UserId;
                            acct.SaveBailActions(conn, trans, acctNo, empeeNo, code, notes,
                                dateDue, actionValue, spaDateExpiry, spaReasonCode, spaInstal
                            , reminderDateTime, cancelOutstandingReminders, callingSource);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetSPAHistory(string accountNo, out string err)
        {
            Function = "BAccountManager::GetSPAHistory()";
            DataSet ds = null;
            err = "";

            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetSPAHistory(accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
        [XmlInclude(typeof(SPAAccountDetails))]
#if(TRACE)
		[TraceExtension]
#endif
        public SPAAccountDetails GetSPAAcctDetails(string acctNo, out string err)
        {
            Function = "WAccountManager::GetSPAAcctDetails()";
            err = "";
            SPAAccountDetails spaacctdetails = new SPAAccountDetails();
            try
            {
                BAccount acct = new BAccount();

                spaacctdetails = acct.GetSPAAcctDetails(acctNo);

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return spaacctdetails;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetStockLocations(ref int itemId, ref string deletedItem, bool includeWarranties, out string err)    // RI  jec
        {
            Function = "BAccountManager::GetStockLocations()";
            DataSet ds = null;
            err = "";

            try
            {
                BItem item = new BItem();
                ds = item.GetStockLocations(ref itemId, ref deletedItem, includeWarranties); //IP - 17/02/10 - CR1072 - LW 71731
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "this method allocates an account to a courtsperson,de-allocating any existing allocations")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        //IP - 08/10/09 - UAT(909)
        //public bool AllocateAccount(string accountNo, int empeeNo, bool checkMaxAccounts, out string err)
        public int AllocateAccount(string accountNo, int empeeNo, out string err)
        {
            Function = "BAccountManager::AllocateAccount()";
            err = "";

            //bool rtnValue = false;

            using (var conn = new SqlConnection(Connections.Default))
            {
                conn.Open();
                try
                {

                    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        BAccount acct = new BAccount();
                        acct.User = STL.Common.Static.Credential.UserId;
                        //rtnValue = acct.SaveAllocation(conn, trans, accountNo, empeeNo, checkMaxAccounts);
                        acct.SaveAllocation(conn, trans, accountNo, empeeNo);

                        trans.Commit();
                    }

                }
                catch (SqlException ex)
                {
                    CatchDeadlock(ex, conn);
                }
            }

            //return rtnValue;
            return 0;
        }

        [WebMethod(Description = "this method deallocates an account to a courtsperson")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int DeAllocateAccount(string accountNo, out string err)
        {
            Function = "BAccountManager::DeAllocateAccount()";
            err = "";
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BAccount acct = new BAccount();
                            acct.User = STL.Common.Static.Credential.UserId;
                            acct.Deallocate(conn, trans, accountNo);

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod(Description = "this method deallocates and then allocates an account from employee to employee")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        //IP - 08/10/09 - UAT(909)
        //public bool AllocateDeallocateAccount(string accountNo, int empeeNo, bool checkMaxAllocation, out string err)
        public int AllocateDeallocateAccount(string accountNo, int empeeNo, out string err)
        {
            Function = "BAccountManager::AllocateDeallocateAccount()";
            err = "";
            SqlConnection conn = null;
            //bool rtnValue = false;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (var trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            BAccount acct = new BAccount();
                            acct.User = STL.Common.Static.Credential.UserId;
                            acct.Deallocate(conn, trans, accountNo);
                            //rtnValue = acct.SaveAllocation(conn, trans, accountNo, empeeNo, checkMaxAllocation);
                            acct.SaveAllocation(conn, trans, accountNo, empeeNo);

                            //if (rtnValue == true)
                            trans.Commit();
                            //else
                            //    trans.Rollback();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            //return rtnValue;
            return 0;
        }

        [WebMethod(Description = "This method returns a dataset containing the details for a specific terms type code")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public double GetInterestRate(string countryCode, string termsType, string acctNo,
            DateTime acctOpen, out string err)
        {
            Function = "BAccountManager::GetInterestRate()";
            double intRate = 0;
            err = "";
            try
            {
                BAgreement agree = new BAgreement();
                intRate = agree.GetInterestRate(countryCode, termsType, acctNo, acctOpen);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return intRate;
        }

        [WebMethod(Description = "this method deallocates an account to a courtsperson")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet SetAllocDate(int empeeNo, out string err)
        {
            Function = "BAccountManager::DeAllocateAccount()";
            err = "";
            SqlConnection conn = null;

            DataSet ds = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BAccount acct = new BAccount();
                            ds = acct.SetAllocDate(conn, trans, empeeNo);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet AccountApplicationStatus(string acctno, out string err)
        {
            Function = "WAccount::AccountApplicationStatus()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                account = new BAccount();
                ds = account.AccountApplicationStatus(acctno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
			[TraceExtension]
#endif
        public DataSet GetLetterByAcctNo(string acctno, out string err)
        {
            Function = "BAccount::GetLetterByAcctNo()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BAccount bo = new BAccount();
                ds = bo.GetLetterByAcctNo(acctno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int BranchUpdate(int branchno, string branchname, string branchaddr1, string branchaddr2,
            string branchaddr3, string branchpocode, string telno, string countrycode, int croffno,
            string oldpctype, string newpctype, DateTime datepcchange, int hissn, int hibuffno,
            string warehouseno, int hirefno, int as400branchno, string region,
            bool depositScreenLocked, DataSet depositSet, string warehouseregion,
             string storeType, bool createRF, bool createCash, bool scoreHPbefore, bool createHP, string Fact2000BranchLetter,
            bool serviceRepairCentre,
            bool behavioural, //IP - 08/04/10 - CR1034 - Removed
            int? defaultPrintLocation, //IP - 23/02/10 - CR1072 - Malaysia 3PL for Version 5.2
            bool isThirdPartyWarehouse, //IP - 23/02/10 - CR1072 - Malaysia 3PL for Version 5.2    
            bool createStore,
            StoreCardBranchQualRules storeCardQualrules, //IP - 7/12/10 - Store Card
            bool isCashLoanBranch,
            bool? luckyDollarStore,
            bool? ashleyStore,
            out string err)
        {
            Function = "WSAccountManager::BranchUpdate()";
            int status = 0;
            err = "";

            SqlConnection conn = null;
            // StoreCardRepository storeCard = new StoreCardRepository();


            using (conn = new SqlConnection(Connections.Default))
            {
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BBranch bo = new BBranch();
                            status = bo.Update(conn, trans, branchno, branchname, branchaddr1, branchaddr2, branchaddr3,
                             branchpocode, telno, countrycode, croffno, oldpctype, newpctype, datepcchange, hissn,
                                hibuffno, warehouseno, hirefno, as400branchno, region, depositScreenLocked, depositSet,
                                warehouseregion, storeType, createRF, createCash, scoreHPbefore, createHP, Fact2000BranchLetter,
                                serviceRepairCentre, behavioural, //CR1034 //IP - 08/04/10 - CR1034 
                                defaultPrintLocation, isThirdPartyWarehouse, createStore, isCashLoanBranch,
                                luckyDollarStore, ashleyStore); //IP - 23/02/10 - CR1072 - Malaysia 3PL for Version 5.2


                            trans.Commit();
                        }

                        //IP - 7/12/10 - If the instance is not null then proceed to save the Store Card Qualification rules.

                        if (storeCardQualrules != null)
                        {
                            new StoreCardRepository().SaveStoreCardBranchQualRules(storeCardQualrules);
                        }

                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }


            return status;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet BranchGet(int branchno, out string err)
        {
            Function = "WAccountManager::BranchGet()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BBranch bo = new BBranch();
                ds = bo.Get(branchno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
			[TraceExtension]
#endif
        public DataSet LoadLettersAndStatuesByAcctNo(string acctno, out string err)
        {
            Function = "WAccountManager::LoadLettersAndStatuesByAcctNo()";
            DataSet ds = new DataSet();
            err = "";


            try
            {
                BAccount bo = new BAccount();
                ds = bo.LettersAndStatusesByAcctNo(acctno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet DeliveryNotesReprintLoad(string acctno, int stockLocn, int bufffnofrom, int bufffnoto, out string err)
        {
            Function = "WAccountManager::DeliveryNotesReprintLoad()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BAccount bo = new BAccount();
                ds = bo.DeliveryNotesReprintLoad(acctno, stockLocn, bufffnofrom, bufffnoto);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int CalculateAddTo(string addToAccount,
            StringCollection accounts,
            string countryCode,
            short branchNo,
            string termsType,
            string scoringBand,
            ref decimal deposit,
            short months,
            short paymentHolidays,
            bool depositChecked,
            out decimal sumBalances,
            out decimal newCashPrice,
            out decimal newAgreementTotal,
            out decimal newMonthlyInstal,
            out decimal newFinalInstal,
            out int newNoInstalments,
            out string err)
        {
            Function = "WAccountManager::CalculateAddTo()";
            err = "";
            sumBalances = 0;
            newCashPrice = 0;
            newAgreementTotal = 0;
            newMonthlyInstal = 0;
            newFinalInstal = 0;
            newNoInstalments = 0;

            try
            {
                BAccount acct = new BAccount();
                acct.CalculateAddTo(addToAccount,
                    accounts,
                    countryCode,
                    branchNo,
                    termsType,
                    scoringBand,
                    ref deposit,
                    months,
                    paymentHolidays,
                    depositChecked,
                    out sumBalances,
                    out newCashPrice,
                    out newAgreementTotal,
                    out newMonthlyInstal,
                    out newFinalInstal,
                    out newNoInstalments);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int ProcessAddTo(string addToAccount,
            StringCollection accounts,
            string countryCode,
            string termsType,
            string scoringBand,
            ref decimal deposit,
            short months,
            short paymentHolidays,
            bool depositChecked,
            out string err)
        {
            Function = "WAccountManager::ProcessAddTo()";
            int status = 0;
            err = "";
            BAccount acct = new BAccount();

            SqlConnection conn = null;


            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            acct.User = STL.Common.Static.Credential.UserId;
                            acct.ProcessAddTo(conn,
                                trans,
                                addToAccount,
                                accounts,
                                countryCode,
                                termsType,
                                scoringBand,
                                ref deposit,
                                months,
                                paymentHolidays,
                                depositChecked);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return status;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int AccountFullyDelivered(string accountNo, out short delivered, out string err)
        {
            Function = "WAccountManager::AccountFullyDelivered()";
            err = "";
            delivered = 0;
            decimal delTot = 0;

            try
            {
                BDelivery del = new BDelivery();
                DAccount account = new DAccount(null, null, accountNo);
                delivered = del.AccountFullyDelivered(null, null, accountNo, account.AgreementTotal, out delTot);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int ContractNoUnique(string accountNo, int agreementNo, string contractNo, out bool unique, out string err)
        {
            Function = "WAccountManager::ContractNoUnique()";
            err = "";
            unique = false;

            try
            {
                BItem item = new BItem();
                unique = item.ContractNoUnique(null, null, accountNo, agreementNo, contractNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int AffinityContractNoUnique(string accountNo, int agreementNo, string contractNo, out bool unique, out string err)
        {
            Function = "WAccountManager::AffinityContractNoUnique()";
            err = "";
            unique = false;

            try
            {
                BItem item = new BItem();
                unique = item.AffinityContractNoUnique(null, null, accountNo, agreementNo, contractNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int HasAddToOrDelivery(string accountNo, out short addto, out string err)
        {
            Function = "WAccountManager::AccountAddedTo()";
            err = "";
            addto = 0;

            try
            {
                BAccount acct = new BAccount();
                addto = acct.HasAddToOrDelivery(null, null, accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SettledByAddTo(string accountNo, out short reversible, out string err)
        {
            Function = "WAccountManager::AccountAddedTo()";
            err = "";
            reversible = 0;

            try
            {
                BAccount acct = new BAccount();
                reversible = acct.SettledByAddTo(null, null, accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int ReverseAddTo(string accountNo,
            string accountType,
            string countryCode,
            out string err)
        {
            Function = "WAccountManager::ReverseAddTo()";
            err = "";
            BAccount acct = new BAccount();

            SqlConnection conn = null;


            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            acct.User = STL.Common.Static.Credential.UserId;
                            acct.ReverseAddTo(conn, trans, accountNo, accountType, countryCode);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod(MessageName = "Second")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public decimal CalculateTaxAmount(string countryCode, XmlNode item, bool taxExempt, out string err)
        {
            Function = "WAccountManager::CalculateTaxAmount()";
            err = "";
            decimal tax = 0;

            try
            {
                BItem i = new BItem();
                tax = i.CalculateTaxAmount(item, taxExempt);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return tax;
        }

        [WebMethod(MessageName = "First")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public decimal CalculateTaxAmount(string countryCode, XmlNode replacement,
            bool taxExempt, decimal oldTaxamt,
            out string err)
        {
            Function = "WAccountManager::CalculateTaxAmount()";
            err = "";
            decimal tax = 0;
            InstantReplacementDetails ir = InstantReplacementDetails.DeSerialise(replacement);

            try
            {
                BItem i = new BItem();
                tax = i.CalculateTaxAmount(ir.Price, false,
                    ir.TaxRate, ir.Quantity, ir.ItemId,
                    taxExempt, oldTaxamt);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return tax;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool IsTaxExempt(string accountNo, string reference, out string err)
        {
            Function = "WAccountManager::IsTaxExempt()";
            err = "";
            bool taxExempt = false;

            try
            {
                BAccount acct = new BAccount();
                taxExempt = acct.IsTaxExempt(null, null, accountNo, reference);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return taxExempt;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetAssociatedWarranties(string accountNo, int itemId, short stockLocn, out string err)       // RI
        {
            Function = "WAccountManager::GetAssociatedWarranties()";
            err = "";
            DataSet ds = new DataSet();
            try
            {
                BItem item = new BItem();
                ds = item.GetAssociatedLineItemWarranties(accountNo, itemId, stockLocn);            // RI
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveProductFaults(string accountNo, int agreementNo,
            string itemNo, string returnItemNo, string notes,
            string reason, DateTime dateCollection,
            short elapsedMonths, short branchNo, out string err)
        {
            Function = "WAccountManager::SaveProductFaults()";
            err = "";
            int newBuffNo = 0;
            try
            {
                SqlConnection conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BItem item = new BItem();
                            newBuffNo = item.SaveProductFaults(conn, trans, accountNo, agreementNo, itemNo,
                                returnItemNo, notes, reason,
                                dateCollection, elapsedMonths, branchNo);

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return newBuffNo;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string GetWarrantyReturnItem(int elapsedMonths, out string err)
        {
            Function = "WAccountManager::GetWarrantyReturnItem()";
            err = "";
            string returnItem = "";
            try
            {
                BItem item = new BItem();
                returnItem = item.GetWarrantyReturnItem(null, null, elapsedMonths);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return returnItem;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string FormatWarrantyReturnCode(string itemNo, DateTime deliveryDate, out string err)
        {
            Function = "WAccountManager::FormatWarrantyReturnCode()";
            err = "";
            string returnCode = "";
            try
            {
                BDelivery del = new BDelivery();
                returnCode = del.FormatWarrantyReturnCode(itemNo, deliveryDate);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return returnCode;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int GetAllocatedCourtsPerson(string accountNo, ref int empNo,
            ref string empType, ref string empName, out string err)
        {
            Function = "WAccountManager::GetAllocatedCourtsPerson()";
            err = "";

            try
            {
                BAccount acct = new BAccount();
                acct.GetAllocatedCourtsPerson(accountNo, ref empNo, ref empType, ref empName);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool CheckAccountType(string acctno, out string err)
        {
            Function = "WAccountManager::CheckAccountType()";
            err = "";
            bool result = false;
            BAccount account = null;
            try
            {
                account = new BAccount();
                account.CheckAccountType(acctno, out result);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return result;

        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int CheckAccountToCancel(string accountNo, string countryCode, ref decimal outstBalance,
            ref bool outstPayments, out string err)
        {
            Function = "WAccountManager::CheckAccountToCancel()";
            err = "";

            SqlConnection conn = null;


            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BAccount acct = new BAccount();
                            acct.User = STL.Common.Static.Credential.UserId;
                            acct.CheckAccountToCancel(conn, trans, accountNo, countryCode, STL.Common.Static.Credential.UserId, ref outstBalance, ref outstPayments);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetScheduledForAccount(string acctno, bool deleteDelLoad, out string err)
        {
            Function = "BAccountManager::GetScheduledForAccount()";
            err = "";
            SqlConnection conn = null;

            DataSet ds = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BSchedule sched = new BSchedule();
                            ds = sched.GetScheduledForAccount(conn, trans, acctno, deleteDelLoad);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
			[TraceExtension]
#endif
        public DataSet GetTranstypeByCode(string transTypeCode, out string err)
        {
            Function = "WAccountManager::GetTranstypeByCode()";
            DataSet ds = new DataSet();
            err = "";


            try
            {
                BTransType tt = new BTransType();
                ds = tt.GetTranstypeByCode(transTypeCode);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveTransType(string transTypeCode, string description, short includeINGFT,
            string intfaceSecAccount, string intfaceAccount, short branchSplit,
            short isDeposit, string intfaceBalancing, short isMandatory,
            short isUnique, string intfaceSecBalancing, short branchSplitBalancing,
            string scInterfaceAccount, string scInterfaceBalancing, out string err)                                         //IP - 11/04/12 - CR9863 - #9885
        {
            Function = "BAccountManager::SaveTransType()";
            err = "";
            SqlConnection conn = null;


            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            EventStore.Instance.Log(trans, new
                            {
                                TransTypeCode = transTypeCode,
                                description = description,
                                IncludeInGFT = includeINGFT,
                                interfaceAccount = intfaceAccount,
                                BranchSplit = branchSplit,
                                DepositType = isDeposit,
                                BalancingAccount = intfaceBalancing
                            }, "TransType", EventCategory.SystemMaintenance);

                            BTransType tt = new BTransType();
                            tt.Save(conn, trans, transTypeCode, description, includeINGFT,
                                intfaceSecAccount, intfaceAccount, branchSplit,
                                isDeposit, intfaceBalancing, isMandatory, isUnique,
                                STL.Common.Static.Credential.UserId, intfaceSecBalancing, branchSplitBalancing,
                                scInterfaceAccount, scInterfaceBalancing);                                                  //IP - 11/04/12 - CR9863 - #9885
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int UpdateStatus(string accountNo, string status, out string err)
        {
            Function = "BAccountManager::UpdateStatus()";
            err = "";
            SqlConnection conn = null;


            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BAccount acct = new BAccount();
                            acct.User = STL.Common.Static.Credential.UserId;
                            acct.UpdateStatus(conn, trans, accountNo, DateTime.Now, status);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
			[TraceExtension]
#endif
        public DataSet GetStatusForAccount(string accountNo, out string err)
        {
            Function = "WAccountManager::GetStatusForAccount()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetStatusForAccount(accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
			[TraceExtension]
#endif
        public DataSet GetForWOReview(string code, string branchFilter, int excludeAccepted, int limit, string category, out string err)
        {
            Function = "WAccountManager::GetForWOReview()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BBDW bdw = new BBDW();
                ds = bdw.GetForWOReview(code, branchFilter, excludeAccepted, limit, category);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int AcceptForWO(string accountNo, out int accepted, out string err)
        {
            Function = "BAccountManager::AcceptForWO()";
            err = "";
            accepted = 0;

            SqlConnection conn = null;


            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BBDW bdw = new BBDW();
                            bdw.AcceptForWO(conn, trans, accountNo, STL.Common.Static.Credential.UserId, out accepted);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SavePending(string acctNo, int user, string code, int runno, int manualUser, out string err)
        {
            Function = "BAccountManager::SavePending()";
            err = "";

            SqlConnection conn = null;


            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BBDW bdw = new BBDW();
                            bdw.SavePending(conn, trans, acctNo, user, code, runno, manualUser);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveRejection(string acctNo, string rejectcode, out string err)
        {
            Function = "BAccountManager::SaveRejection()";
            err = "";

            SqlConnection conn = null;


            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            int user = STL.Common.Static.Credential.UserId;

                            BBDW bdw = new BBDW();
                            bdw.SaveRejection(conn, trans, acctNo, user, rejectcode);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetTransactionsForTransfer(string acctno, DateTime beforeDate, bool limitRows,
                                        out decimal availableTransfer, out string err)
        {
            Function = "WAccountManager::GetTransactionsForTransfer()";
            DataSet ds = null;
            availableTransfer = 0;
            err = "";

            try
            {
                BTransaction bo = new BTransaction();
                ds = bo.GetForTransfer(acctno, beforeDate, limitRows, out availableTransfer);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string GetSundryCreditAccount(short branchNo, out string err)
        {
            Function = "WAccountManager::GetSundryCreditAccount()";
            string sundryAccount = "";
            err = "";

            try
            {
                BAccount acct = new BAccount();
                sundryAccount = acct.GetSundryCreditAccount(null, null, branchNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return sundryAccount;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int TransferTransaction(string fromAccountNo, string toAccountNo, string transType, decimal transValue, short branchNo, string countryCode, DateTime dateTrans, string reasonCode, int oldRefNo, int agrmtNo, Int64? storeCardNo, out string err, string cashierTotID = "") //IP - 14/02/12 - #8819 - CR1234 - added cashierTotID //IP - 29/11/10 - Store Card - Added agrmtNo //IP - 30/11/10 - Added storeCardNo
        {
            Function = "WAccountManager::TransferTransaction()";
            err = "";

            //decimal storeCardAvailable = 0;         //IP - 12/01/11 - Store Card

            SqlConnection conn = null;

            BTransaction t = new BTransaction();

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            //IP - 16/02/12 - #9377 - CR1234

                            EventStore.Instance.Log(new
                            {
                                BranchNo = branchNo,
                                FromAcctNo = fromAccountNo,
                                ToAcctNo = toAccountNo,
                                TransType = transType,
                                TransValue = transValue,
                                DateTrans = dateTrans,
                                TransRefNo = oldRefNo,
                                TransferRef = cashierTotID
                            }, "TransferTransaction", EventCategory.TransferTransaction, new { acctno = toAccountNo, transrefno = oldRefNo, transferRef = cashierTotID });

                            t.User = STL.Common.Static.Credential.UserId;
                            t.TransferTransaction(conn, trans, fromAccountNo, toAccountNo, transType, transValue, branchNo, countryCode, dateTrans, reasonCode, oldRefNo, 0, agrmtNo, storeCardNo, cashierTotID); //IP - 14/02/12 - #8819 - CR1234 //IP - 29/11/10 - Store Card - Added agrmtNo //IP - 30/11/10 - Added storeCardNo 

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int GetSundryAccountTransactionTotal(int branchno, DateTime before, out decimal total, out string err)
        {
            Function = "WAccountManager::GetSundryAccountTransactionTotal()";
            int status = 0;
            err = "";
            total = 0;

            try
            {
                BTransaction bo = new BTransaction();
                status = bo.GetSundryAccountTransactionTotal(branchno, before, out total);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return status;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool CanReverseOverage(int empeeno, decimal overage, out string err)
        {
            Function = "WAccountManager::CanReverseOverage()";
            err = "";
            bool canReverse = false;

            try
            {
                BAccount acct = new BAccount();
                canReverse = acct.CanReverseOverage(null, null, empeeno, overage);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return canReverse;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string GetOveragesAccount(short branchno, out string err)
        {
            Function = "WAccountManager::GetOveragesAccount()";
            err = "";
            string acctno = "";

            try
            {
                BAccount acct = new BAccount();
                acctno = acct.GetOveragesAccount(null, null, branchno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return acctno;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string GetReceivableAccount(int empeeno, out string err)
        {
            Function = "WAccountManager::GetReceivableAccount()";
            err = "";
            string acctno = "";

            try
            {
                BAccount acct = new BAccount();
                acctno = acct.GetReceivableAccount(null, null, empeeno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return acctno;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string GetDefaultTermsType(bool isLoan, out string err)
        {
            Function = "WAccountManager::GetDefaultTermsType()";
            err = "";
            string termstype = "";

            try
            {
                BAccount acct = new BAccount();
                termstype = acct.GetDefaultTermsType(null, null, 0, isLoan);	// CR903 - add branchno 				
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return termstype;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetRebatesTotal(int branchNo, out string err)
        {
            Function = "WAccountManager::GetRebatesTotal()";
            err = "";
            DataSet ds = new DataSet();

            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetRebatesTotal(branchNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DateTime GetRebatesAsAt(out string err)
        {
            Function = "WAccountManager::GetRebatesAsAt()";
            err = "";
            DateTime rebatesAsAt = DateTime.Now;

            try
            {
                BAccount acct = new BAccount();
                rebatesAsAt = acct.GetRebatesAsAt();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return rebatesAsAt;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string[] GetAccountStatuses(StringCollection accounts, out string err)
        {
            Function = "WAccountManager::GetAccountStatuses()";
            err = "";
            string[] s = null;

            try
            {
                BAccount acct = new BAccount();
                s = acct.GetAccountStatuses(accounts);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return s;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetInstalmentAccounts(string acctno, out string err)
        {
            Function = "WAccountManager::GetInstalmentAccounts()";
            DataSet ds = null;
            err = "";

            try
            {
                BInstalPlan instal = new BInstalPlan();
                instal.AccountNumber = acctno;
                ds = instal.GetAccounts();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int UpdateDateFirst(DataSet ds, out string err)
        {
            Function = "WAccountManager::UpdateDateFirst()";
            err = "";

            SqlConnection conn = null;

            BInstalPlan instal = new BInstalPlan();

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            instal.User = STL.Common.Static.Credential.UserId;
                            instal.UpdateDateFirst(conn, trans, ds);

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int IsItemInstantReplacement(int itemId, short branchNo, out bool ir, out string err)
        {
            Function = "WAccountManager::IsItemInstantReplacement()";
            err = "";
            ir = false;

            try
            {
                BItem item = new BItem();
                ir = item.IsItemInstantReplacement(null, null, itemId, branchNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetAccountAuditData(string accountNo, out string err)
        {
            err = "";
            DataSet changes = null;

            try
            {
                BAccount acct = new BAccount();
                changes = acct.GetAccountAuditData(accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return changes;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool IsRepossessed(string accountNo, out string err)
        {
            Function = "WAccountManager::IsRepossessed()";
            err = "";
            bool repo = false;

            try
            {
                BAccount acct = new BAccount();
                repo = acct.IsRepossessed(accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return repo;
        }

        [WebMethod(Description = "This method creates a new RF account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int CreateManualAccount(string countryCode, short branchNo, string customerID, string acctNo, string accountType, bool isLoan, out bool rescore, out string err)
        {
            Function = "BAccountManager::CreateManualAccount()";
            SqlConnection conn = null;
            rescore = false;
            err = "";

            BAccount acct = null;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            acct = new BAccount();
                            acct.User = STL.Common.Static.Credential.UserId;
                            acct.CreateManualAccount(conn, trans, countryCode, branchNo, customerID, acctNo, accountType, isLoan, out rescore);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }


        [WebMethod(Description = "This method creates a new RF account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int CreateManualRFAccount(string countryCode, short branchNo, string customerID, string acctNo, bool isLoan, out bool rescore, out string err)
        {
            Function = "BAccountManager::CreateRFAccount()";
            SqlConnection conn = null;
            rescore = false;
            err = "";

            BAccount acct = null;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            acct = new BAccount();
                            acct.User = STL.Common.Static.Credential.UserId;
                            acct.CreateManualRFAccount(conn, trans, countryCode, branchNo, customerID, acctNo, isLoan, out rescore);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public short GetDueDay(string custID, out string err)
        {
            err = "";
            short dueday = 0;

            try
            {
                BInstalPlan ip = new BInstalPlan();
                dueday = ip.GetDueDay(custID);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return dueday;
        }
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetActivitySegments(string acctNo, out string err)
        {
            err = "";
            DataSet ds = null;

            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetActivitySegments(acctNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod(Description = "This updates the stock location for a particular item.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int UpdateItemLocation(DataSet ds, bool newDelNote, out string err)
        {
            Function = "BAccountManager::UpdateItemLocation()";
            SqlConnection conn = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BItem item = new BItem();
                            item.User = STL.Common.Static.Credential.UserId;
                            item.UpdateItemLocation(conn, trans, ds, newDelNote);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod(Description = "This updates the stock location for a particular item.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int CancelDeliveryNote(string accountNo, DataSet ds, bool isDotNetWarehouse, out string err)
        {
            Function = "BAccountManager::UpdateDeliveryNote()";
            SqlConnection conn = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BItem item = new BItem();
                            item.User = STL.Common.Static.Credential.UserId;
                            item.CancelDeliveryNote(conn, trans, accountNo, ds, isDotNetWarehouse);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }


        [WebMethod(Description = "This calls the calculation routine that updates the Rebates_Total table.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int UpdateRebateTotals(string acctNo,
            DateTime fromThresDate,
            DateTime toThresDate,
            DateTime acctsFromDate,
            DateTime ruleChangeDate,
            DateTime rebateDate,
            out decimal poRebate,
            out decimal poRebateWithin12Mths,
            out decimal poRebateAfter12Mths,
            out string err)
        {
            Function = "WAccountManager::UpdateRebateTotals()";
            SqlConnection conn = null;
            err = "";

            poRebate = 0;
            poRebateWithin12Mths = 0;
            poRebateAfter12Mths = 0;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BAccount acc = new BAccount();
                            acc.UpdateRebateTotals(conn, trans, acctNo, fromThresDate, toThresDate,
                                acctsFromDate, ruleChangeDate, rebateDate,
                                out poRebate, out poRebateWithin12Mths, out poRebateAfter12Mths);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        //IP - 28/04/09 - CR929 & 974 Deliveries - boolean to determine whether order details can be changed
        //prior to being DA'ed.
        public DataSet GetItemsForLocationChange(string acctno, bool loadBeforeDA, out string err)
        {
            Function = "BAccountManager::GetItemsForLocationChange()";
            DataSet ds = null;
            err = "";
            try
            {
                BItem item = new BItem();
                ds = item.GetItemsForLocationChange(acctno, loadBeforeDA);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method returns the quantity delivered and scheduled for delivery for a particular line item")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int GetScheduledDelNote(string accountNo,
            int agreementNo,
            int itemID,                                     //IP/NM - 18/05/11 -CR1212 - #3627 
            short location,
            out bool onPickList,
            out bool delNotePrinted,
            out bool onLoad,
            out string err)
        {
            Function = "BAccountManager::GetScheduledDelNote()";
            err = "";
            onPickList = false;
            delNotePrinted = false;
            onLoad = false;

            try
            {
                BItem item = new BItem();
                item.GetScheduledDelNote(accountNo,
                    agreementNo,
                    itemID,                                 //IP/NM - 18/05/11 -CR1212 - #3627 
                    location,
                    out onPickList,
                    out delNotePrinted,
                    out onLoad);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool IsCancelled(string accountNo, out string err)
        {
            Function = "WAccountManager::IsCancelled()";
            err = "";
            bool cancelled = false;

            try
            {
                BAccount acct = new BAccount();
                cancelled = acct.IsCancelled(accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return cancelled;
        }

        [WebMethod(Description = "This method will reverse a cancellation.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int ReverseCancellation(string accountNo, string code, string notes, out string err)
        {
            Function = "WAccountManager::ReverseCancellation()";
            SqlConnection conn = null;

            err = "";
            BAccount acct = null;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            acct = new BAccount();
                            acct.User = STL.Common.Static.Credential.UserId;
                            acct.UpdateStatus(conn, trans, accountNo, DateTime.Now, "1");
                            acct.ReverseCancellation(conn, trans, accountNo, code, notes);

                            if (accountNo.Substring(3, 1) == "9")
                            {
                                var SCRep = new StoreCardRepository();
                                //  var AccountStatus = SCRep.GetAccountStatus( accountNo,conn,trans);
                                SCRep.AccountStatusUpdate(accountNo, StoreCardAccountStatus_Lookup.CardToBeIssued.Code, conn, trans);

                            }
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        //        [WebMethod]
        //        [SoapHeader("authentication")]
        //#if(TRACE)
        //            [TraceExtension]
        //#endif
        //        public WarrantyReturnModel[] GetWarrantyReturnCode(string acctno, short stocklocn, short replacement,
        //            //string parentItemNo, string WarrantyNo, string contractNo, out decimal refund, out DateTime deliveryDate, out string err)
        //            int parentItemID, int warrantyItemID, string contractNo, out decimal refund, out DateTime deliveryDate, out int warrantyRetCodeItemID, out string err) //IP - 13/09/11 - RI - #8112 - added warrantyRetCodeItemID       //IP - 21/06/11 - CR1212 - RI - #3939
        //        {
        //            Function = "WAccountManager::GetWarrantyReturnCode()";
        //            err = "";
        //            //string returnCode = "";
        //            //refund = 0;
        //            //deliveryDate = DateTime.Now;
        //            warrantyRetCodeItemID = 0;                                  //IP - 13/09/11 - RI - #8112

        //            try
        //            {
        //                //BDelivery del = new BDelivery();
        //                //del.GetWarrantyReturnCode(acctno, stocklocn, replacement, parentItemNo,
        //                //    WarrantyNo, contractNo, out returnCode, out refund, out deliveryDate);

        //                //del.GetWarrantyReturnCode(acctno, stocklocn, replacement, parentItemID,                  //IP - 21/06/11 - CR1212 - RI - #3939
        //                //  warrantyItemID, contractNo, out returnCode, out refund, out deliveryDate, out warrantyRetCodeItemID); //IP - 13/09/11 - RI - #8112 - added warrantyRetCodeItemID
        //                var elapsedMonths = new WarrantyRepository().GetWarrantyElapsedMonths(warrantyItemID,contractNo,stocklocn);
        //                if (elapsedMonths.HasValue)
        //                {
        //                    return new Services(ServiceTypes.CosacsWeb).GetService().GetWarrantyReturn(warrantyItemID, (int)stocklocn, elapsedMonths.Value);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Catch(ex, Function, ref err);
        //            }
        //            return returnCode;
        //        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool ManualCDVExists(string accountNo, out string err)
        {
            Function = "WAccountManager::ManualCDVExists()";
            err = "";
            bool exists = false;
            try
            {
                BAccount bAcct = new BAccount();
                exists = bAcct.ManualCDVExists(accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return exists;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetPeriodEndDates(out string nextPeriodEnd, out string err)
        {
            Function = "BAccountManager::GetPeriodEndDates()";
            DataSet ds = null;
            nextPeriodEnd = "";
            err = "";
            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetPeriodEndDates(out nextPeriodEnd);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        //CR931 - IP - 08/04/08 - Added branchNo 
        public DataSet GetRebateForecastReports(string periodEnd, int branchNo, out string err)
        {
            Function = "BAccountManager::GetRebateForecastReports()";
            DataSet ds = null;
            err = "";
            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetRebateForecastReports(periodEnd, branchNo);     // CR931 Forecast by Branch  jec 04/04/08
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method will reverse a cancellation.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int RunRebateForecastReports(string periodEnd, out string err)
        {
            Function = "WAccountManager::RunRebateForecastReports()";
            SqlConnection conn = null;

            err = "";
            BAccount acct = null;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            acct = new BAccount();
                            acct.User = STL.Common.Static.Credential.UserId;
                            acct.RunRebateForecastReports(conn, trans, periodEnd);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod(Description = "This method will determine if a Cash & Go account has any warranties.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string IsPaidAndTakenWarranty(string accountNo, out string err)
        {
            Function = "WAccountManager::IsPaidAndTakenWarranty()";
            err = "";
            string termsType = "";
            try
            {
                BAccount acct = new BAccount();
                termsType = acct.IsPaidAndTakenWarranty(accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return termsType;
        }

        [WebMethod(Description = "This method will retrieve all bookings for a particular Branch")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetBookings(
            string branchNo,
            string empeeNo,
            DateTime fromDate,
            DateTime toDate,
            int includeCash,
            int includeHP,
            int includeNonSec,
            int includePaidTaken,
            int includeRf,
            int includeSec,
            int rollUpResults,
            int liveDatabase,
            out string err)
        {
            Function = "BAccountManager::GetBookings()";
            SqlConnection conn = null;

            DataSet ds = null;
            err = "";
            try
            {
                // This web service can connect to one of two possible database connections
                if (liveDatabase == 1)
                    conn = new SqlConnection(Connections.Default);
                else
                    conn = new SqlConnection(Connections.Report);

                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BAccount acct = new BAccount();
                            ds = acct.GetBookings(
                                conn,
                                trans,
                                branchNo,
                                empeeNo,
                                fromDate,
                                toDate,
                                includeCash,
                                includeHP,
                                includeNonSec,
                                includePaidTaken,
                                includeRf,
                                includeSec,
                                rollUpResults);

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return ds;
        }

        [WebMethod(Description = "This method will retrieve deliveries/collections based on certain search criteria.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetDeliveryOrders(DateTime fromDate,
            DateTime toDate,
            string deliveryArea,
            int includeDeliveries,
            int includeCollections,
            int includeAddresses,
            int includeLinkedItems,
            string deliveryProcess,
            string majorProductCategory,
            string minorProductCategory,
            string acctNo,
            int user,
            int branchNo,
            int delNotBranchNo,
            bool reqDelSearch,
            bool includeAssembly,
            bool includeNonAssembly,
            out DateTime timeLocked,
            out string err)
        {
            Function = "BAccountManager::GetDeliveryOrders()";
            DataSet ds = null;
            DateTime returnTimeLocked = DateTime.Now;
            err = "";
            try
            {
                BDelivery delivery = new BDelivery();
                ds = delivery.GetDeliveryOrders(fromDate,
                    toDate,
                    deliveryArea,
                    includeDeliveries,
                    includeCollections,
                    includeAddresses,
                    includeLinkedItems,
                    deliveryProcess,
                    majorProductCategory,
                    minorProductCategory,
                    acctNo,
                    user,
                    branchNo,
                    delNotBranchNo,
                    reqDelSearch,
                    includeAssembly,
                    includeNonAssembly,
                    out returnTimeLocked);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                timeLocked = returnTimeLocked;
            }
            return ds;
        }


        [WebMethod(Description = "This method will retrieve deliveries/collections for an account.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetDeliveryNotesForAcct(
            string acctNo,
            int user,
            bool collectionsOnly,
            out DateTime timeLocked,
            out string err)
        {
            Function = "BAccountManager::GetDeliveryNotesForAcct()";
            DataSet ds = null;
            DateTime returnTimeLocked = DateTime.Now;
            err = "";
            try
            {
                BDelivery delivery = new BDelivery();
                ds = delivery.GetDeliveryNotes(acctNo,
                    user,
                    true,
                    out returnTimeLocked);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                timeLocked = returnTimeLocked;
            }
            return ds;
        }

        [WebMethod(Description = "This method will retrieve all deliveries/collections for a particular Branch/Warehouse or Load Number.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetOutstandingDeliveries(int bufferNo,
            int warehouseNo,
            DateTime fromDate,
            DateTime toDate,
            int includeSec,
            int includeNonSec,
            string operand,
            out string err)
        {
            Function = "BAccountManager::GetDeliveries()";
            DataSet ds = null;
            err = "";
            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetDeliveries(bufferNo,
                    warehouseNo,
                    fromDate,
                    toDate,
                    includeSec,
                    includeNonSec,
                    operand);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetRelatedItems(int itemId, short location, out string err)
        {
            Function = "BAccountManager::GetRelatedItems()";
            DataSet ds = null;
            err = "";
            try
            {
                BItem item = new BItem();
                ds = item.GetRelatedItems(itemId, location);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetWarrantyRenewals(string acctNo, bool settled, bool ismenu,
            ref string custID, out string err)
        {
            Function = "BAccountManager::GetWarrantyRenewals()";
            DataSet ds = null;
            err = "";
            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetWarrantyRenewals(acctNo, settled, ismenu, ref custID);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "Returns a list of warranty products for this account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetWarrantyProductsByAccount(string accNo, out string err)
        {
            Function = "BAccountManager::GetWarrantyProductsByAccount()";
            err = "";
            DataSet ds = null;
            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetWarrantyProductsByAccount(accNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method will reverse a cancellation.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int AddWarrantRenewalCode(string acctNo, string contractNo, out string err)
        {
            Function = "WAccountManager::AddWarrantRenewalCode()";
            SqlConnection conn = null;

            err = "";
            BAccount acct = null;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            acct = new BAccount();
                            acct.User = STL.Common.Static.Credential.UserId;
                            acct.AddWarrantRenewalCode(conn, trans, acctNo, contractNo);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod(Description = "This method will load allocated accounts that need to have their action sheet re-printed.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet LoadAllocationsForReprint(int empeeNo, out string err)
        {
            Function = "BAccountManager::LoadAllocationsForReprint()";
            DataSet ds = null;
            err = "";
            try
            {
                BAccount acct = new BAccount();
                ds = acct.LoadAllocationsForReprint(empeeNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet LoadAllocationDetails(int empeeNo, DateTime dateAllocated, out string err)
        {
            Function = "BAccountManager::LoadAllocationDetails()";
            DataSet ds = null;
            err = "";
            try
            {
                BAccount acct = new BAccount();
                ds = acct.LoadAllocationDetails(empeeNo, dateAllocated);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method will update allocated accounts so that action sheets can be re-printed.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int UpdateAllocForReprint(string acctNo, int empeeNo,
            DateTime dateAllocated, bool batch, out string err)
        {
            Function = "WAccountManager::UpdateAllocForReprint()";
            SqlConnection conn = null;

            err = "";
            BAccount acct = null;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            acct = new BAccount();
                            acct.User = STL.Common.Static.Credential.UserId;
                            acct.UpdateAllocForReprint(conn, trans, acctNo, empeeNo,
                                dateAllocated, batch);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetRepossessedItemDetails(string acctNo, out string err)
        {
            Function = "BAccountManager::GetRepossessedItemDetails()";
            DataSet ds = null;
            err = "";
            try
            {
                BDelivery del = new BDelivery();
                ds = del.GetRepossessedItemDetails(acctNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int GetNextPicklistNo(int branchNo, int user, string pickListType,
                                        out int pickListNo, out string err)
        {
            Function = "BAccountManager::GetNextPicklistNo()";
            pickListNo = 0;
            err = "";
            try
            {
                BSchedule sched = new BSchedule();
                sched.GetNextPicklistNo(branchNo, user, pickListType, out pickListNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet DeliveryNotesLoadByLoadNo(short branchNo, int loadNo, DateTime dateDel, out DateTime minDelDate, out string[] acctExceedsAgr, out string err)
        {
            Function = "BAccountManager::DeliveryNotesLoadByLoadNo()";
            DataSet ds = null;
            minDelDate = Date.blankDate;
            //IP - 04/02/08 - Livewire:(69454) - added 'acctExceedsAgr'
            acctExceedsAgr = null;
            err = "";
            try
            {
                BDelivery del = new BDelivery();
                ds = del.DeliveryNotesByLoadNo(branchNo, loadNo, dateDel, out minDelDate, out acctExceedsAgr);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int ChangeReqDeliveryDate(string acctNo, int agrmtNo, int itemId,
                                         string contractNo, short stockLocn, DateTime reqDeliveryDate,
                                         int buffNo, string reason, out string err)
        {
            Function = "WAccountManager::ChangeReqDeliveryDate()";
            SqlConnection conn = null;

            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BSchedule schedule = new BSchedule();
                            schedule.User = STL.Common.Static.Credential.UserId;
                            schedule.ChangeReqDeliveryDate(conn, trans, acctNo, agrmtNo,
                                                           itemId, contractNo, stockLocn,
                                                           reqDeliveryDate, buffNo, reason);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetAcctNoCtrl(int branchNo, out string err)
        {
            Function = "BAccountManager::GetAcctNoCtrl()";
            DataSet ds = null;
            err = "";

            try
            {
                account = new BAccount();
                ds = account.GetAcctNoCtrl(branchNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int RemoveDeliveryNote(
            short branchNo, int buffNo, int empeeNo, string reason, out string err)
        {
            Function = "WAccountManager::RemoveDeliveryNote()";
            SqlConnection conn = null;

            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BSchedule schedule = new BSchedule();
                            schedule.User = STL.Common.Static.Credential.UserId;
                            schedule.RemoveDeliveryNote(conn, trans, branchNo, buffNo, empeeNo, reason);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int RemoveLineItem(
            short stockLocn, int buffNo, string acctNo, int agrmtNo,
            int itemId, int empeeNo, string reason, out string err)
        {
            Function = "WAccountManager::RemoveLineItem()";
            SqlConnection conn = null;

            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BSchedule schedule = new BSchedule();
                            schedule.User = STL.Common.Static.Credential.UserId;
                            schedule.RemoveLineItem(conn, trans,
                                stockLocn, buffNo, acctNo, agrmtNo, itemId, empeeNo, reason);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int ChangeItemQuantity(double newQuantity, short stockLocn, int buffNo,
            string acctNo, int agrmtNo, int itemID, short curBranchNo, int parentItemId, out string err)                  //IP - 07/06/11 - CR1212 - RI
        {
            Function = "WAccountManager::ChangeItemQuantity()";
            SqlConnection conn = null;

            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BSchedule schedule = new BSchedule();
                            schedule.User = STL.Common.Static.Credential.UserId;
                            schedule.ChangeQuantity(conn, trans, newQuantity,
                                stockLocn, buffNo, acctNo, agrmtNo, itemID, curBranchNo, parentItemId);               //IP - 07/06/11 - CR1212 - RI          
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }



        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int SaveAcctNoCtrl(
            int branchNo, string acctCat, string acctCatDesc,
            int hiAllocated, int hiAllowed, out string err)
        {
            Function = "WAccountManager::SaveAcctNoCtrl()";
            SqlConnection conn = null;

            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BAccount bAccount = new BAccount();
                            bAccount.SaveAcctNoCtrl(conn, trans, branchNo, acctCat, acctCatDesc, hiAllocated, hiAllowed);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod(Description = "This method will update allocated accounts so that action sheets can be re-printed.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int ScheduleRedelRepo(short origbr, string acctno, DateTime datedelplan, char delorcoll,
                                int itemID, short stocklocn, short quantity, short retstocklocn,
                                int retItemID, decimal retval, int buffbranchno, int buffno, string delArea,
                                int agrmtNo, string contractNo, int parentItemID, int lineItemId, int user, string deliveryAddress, out string err)    // #14927
        {
            Function = "WAccountManager::ScheduleRedelRepo()";
            SqlConnection conn = null;

            err = "";
            BDelivery del = null;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            del = new BDelivery();
                            del.User = user;            //#10386
                            del.ScheduleRedelRepo(conn, trans, origbr, acctno, datedelplan,
                                            delorcoll, itemID, stocklocn, quantity, retstocklocn,
                                            retItemID, retval, buffbranchno, buffno, delArea,
                                            agrmtNo, contractNo, parentItemID, lineItemId, deliveryAddress);  // #14927

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetTransportList(out string err)
        {
            Function = "BAccountManager::GetTransportList()";
            DataSet ds = null;
            err = "";
            try
            {
                BDelivery del = new BDelivery();
                ds = del.GetTransportList();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetScheduledLoads(short branchNo, DateTime dateFrom, DateTime dateTo,
            short printed, short loadNo, short withSchedules, out string err)
        {
            Function = "BAccountManager::GetScheduledLoads()";
            DataSet ds = null;
            err = "";
            try
            {
                BDelivery del = new BDelivery();
                ds = del.GetScheduledLoads(branchNo, dateFrom, dateTo, printed, loadNo, withSchedules);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetItemsInRegion(int itemId, short branchNo, out string err)
        {
            Function = "BAccountManager::GetItemsInRegion()";
            DataSet ds = null;
            err = "";
            try
            {
                BItem item = new BItem();
                ds = item.GetItemsInRegion(itemId, branchNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetPurchaseOrders(int itemId, short branchNo, out string err)
        {
            Function = "BAccountManager::GetPurchaseOrders()";
            DataSet ds = null;
            err = "";
            try
            {
                BItem item = new BItem();
                ds = item.GetPurchaseOrders(itemId, branchNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetLoadContents(short loadNo, DateTime dateDel, short branchNo, out string err)
        {
            Function = "BAccountManager::GetLoadContents()";
            DataSet ds = null;
            err = "";
            try
            {
                BDelivery del = new BDelivery();
                ds = del.GetLoadContents(loadNo, dateDel, branchNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int TransportSchedAdd(short branchNo, DateTime dateDel, short loadNo, string TruckId, out string err)
        {
            Function = "BAccountManager::TransportSchedAdd()";
            //DataSet ds = null;
            SqlConnection conn = null;

            int result = 0;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BDelivery del = new BDelivery();
                            result = del.TransportSchedAdd(conn, trans, branchNo, dateDel, loadNo, TruckId);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return result;
        }
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int DeliveryScheduleUpdate(int loadNo, int buffNo, int filter, int branchNo, int pickListNo, DateTime dateDel, out string err)
        {
            Function = "BAccountManager::DeliveryScheduleUpdate()";
            //DataSet ds = null;
            SqlConnection conn = null;


            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BDelivery del = new BDelivery();
                            del.DeliveryScheduleUpdate(conn, trans, loadNo, buffNo, filter, branchNo, pickListNo, dateDel);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();

            }
            return 0;
        }
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet LoadAvailablePicklists(short branchNo, char type, out string err)
        {
            Function = "BAccountManager::LoadAvailablePicklists()";
            DataSet ds = null;
            err = "";
            try
            {
                BDelivery del = new BDelivery();
                ds = del.LoadAvailablePicklists(branchNo, type);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int LockItem(string itemNo, short stockLocn, ref string lockString, out string err)
        {
            Function = "BAccountManager::LockItem()";
            //DataSet ds = null;
            SqlConnection conn = null;


            err = "";
            lockString = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BItem item = new BItem();
                            item.User = STL.Common.Static.Credential.UserId;
                            item.LockItem(conn, trans, itemNo, stockLocn, ref lockString);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int UnlockItem(int user, out string err)
        {

            Function = "BAccountManager::UnlockItem()";
            //DataSet ds = null;
            SqlConnection conn = null;


            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BItem item = new BItem();
                            item.User = user;
                            item.UnlockItem(conn, trans);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveTransport(string truckId, string driverName, string phoneNo, string carrierNumber, out string err)
        {
            Function = "WAccountManager::SaveTransport()";

            SqlConnection conn = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BTransport Transport = new BTransport();
                            Transport.Save(conn, trans, truckId, driverName, phoneNo, carrierNumber);

                            trans.Commit();		//commit the transaction
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                err = Function + ":  " + ex.Message;
                logException(ex, Function);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetTransport(string truckId, out string err)
        {
            Function = "WAccountManager::GetTransport()";
            DataSet ds = null;
            err = "";

            try
            {
                BTransport Transport = new BTransport();
                ds = Transport.GetTransport(truckId);
            }
            catch (Exception ex)
            {
                err = Function + ": " + ex.Message;
                logException(ex, Function);
            }

            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int DeleteTransport(string truckId, out string err)
        {
            Function = "WAccountManager::DeleteTransport()";

            SqlConnection conn = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BTransport Transport = new BTransport();
                            Transport.Delete(conn, trans, truckId);

                            trans.Commit();		//commit the transaction
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                err = Function + ":  " + ex.Message;
                logException(ex, Function);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetDeliveryScheduleDetails(short branchNo, DateTime dateDel, short loadNo, out string err)
        {
            Function = "WAccountManager::GetDeliveryScheduleDetails()";
            DataSet ds = null;
            err = "";
            try
            {
                BDelivery del = new BDelivery();
                ds = del.GetDeliveryScheduleDetails(branchNo, dateDel, loadNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int RemoveLoadFromContents(DateTime dateDel, short bufferBranchNo, int buffNo, short loadNo, string accountNo, string notes, out string err)
        {
            Function = "WAccountManager::RemoveLoadFromContents()";
            SqlConnection conn = null;

            int empNo = STL.Common.Static.Credential.UserId;

            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BDelivery del = new BDelivery();
                            del.RemoveLoadFromContents(conn, trans, dateDel, bufferBranchNo, buffNo, loadNo, accountNo, notes, empNo);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }
        [WebMethod(Description = "This method loads a list of schedules (delivery notes) by buffno")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetPickListSchedule(int filter, int PickListNo, int BuffNo, out string err)
        {
            Function = "WAccountManager::GetPickListSchedule()";
            err = "";
            DataSet ds = null;
            try
            {
                BSchedule sched = new BSchedule();
                ds = sched.GetPickListSchedule(filter, PickListNo, BuffNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }
        [WebMethod(Description = "This method loads a list of schedules (delivery notes) by buffno")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string AccountGetAccountNoByBuffNo(int stockLocn, int BuffNo, out string err)
        {
            Function = "WAccountManager::AccountGetAccountNoByBuffNo()";
            err = "";
            string acctNo = "";
            try
            {
                BAccount acct = new BAccount();
                acctNo = acct.AccountGetAccountNoByBuffNo(stockLocn, BuffNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return acctNo;
        }

        [WebMethod(Description = "To audit document re-prints in the DocumentReprint table")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void AuditReprint(string accountNo, int agreementNo, string docType, out string err)
        {
            Function = "WAccountManager::AuditReprint()";
            SqlConnection conn = null;

            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BAccount acct = new BAccount();
                            acct.User = STL.Common.Static.Credential.UserId;
                            acct.AuditReprint(conn, trans, accountNo, agreementNo, docType);

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }

        [WebMethod(Description = "To audit authorised discounts")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void AuditDiscount(
            string accountNo, int agreementNo, string discountItemNo, string parentItemNo,
            short stockLocn, decimal amount, int salesPerson, int authorisedBy, out string err)
        {
            Function = "WAccountManager::AuditDiscount()";
            SqlConnection conn = null;

            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BAccount acct = new BAccount();
                            acct.AuditDiscount(conn, trans,
                                accountNo, agreementNo, discountItemNo, parentItemNo,
                                stockLocn, amount, salesPerson, authorisedBy);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet SUCBGetDelTotals(int runno, out decimal delTotal, bool liveDatabase, out string err)
        {
            Function = "WAccountManager::SUCBGetDelTotals()";
            err = "";
            delTotal = 0;
            DataSet ds = null;
            SqlConnection conn = null;


            if (liveDatabase)
                conn = new SqlConnection(Connections.Default);
            else
                conn = new SqlConnection(Connections.Report);

            using (conn)
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        BDelivery del = new BDelivery();
                        ds = del.SUCBGetDelTotals(runno, conn, trans, out delTotal);
                        trans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    Catch(ex, Function, ref err);
                }
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        //public DataSet SUCBGetDelDetails(int runno, string branchNo, out string err)
        public DataSet SUCBGetDelDetails(string datetrans, string branchNo, out string err)                       //IP - 20/02/12 - #9423 - CR8262
        {
            Function = "WAccountManager::SUCBGetDelDetails()";
            err = "";
            DataSet ds = null;

            try
            {
                BDelivery del = new BDelivery();
                ds = del.SUCBGetDelDetails(datetrans, branchNo);                                                     //IP - 20/02/12 - #9423 - CR8262
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetExchangeDetails(string acctNo, int agrmtNo, out string err)
        {
            Function = "WAccountManager::GetExchangeDetails()";
            err = "";
            DataSet ds = null;

            try
            {
                BDelivery del = new BDelivery();
                ds = del.GetExchangeDetails(acctNo, agrmtNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool IsDotNetWarehouse(short branchNo, out string err)
        {
            Function = "WAccountManager::IsDotNetWarehouse()";
            err = "";
            bool isDotNetWarehouse = false;

            try
            {
                DDelivery del = new DDelivery();
                isDotNetWarehouse = del.IsDotNetWarehouse(branchNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return isDotNetWarehouse;
        }

        //IP - 12/04/10 - UAT(66) UAT5.2
        /// <summary>
        /// Method which determines if a branch is a Third Party Deliveries warehouse.
        /// </summary>
        /// <param name="branchNo"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool IsThirdPartyWarehouse(short branchNo, out string err)
        {
            Function = "WAccountManager::IsThirdPartyWarehouse()";
            err = "";
            bool isThirdPartyWarehouse = false;

            try
            {
                DDelivery del = new DDelivery();
                isThirdPartyWarehouse = del.IsThirdPartyWarehouse(branchNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return isThirdPartyWarehouse;
        }

        [WebMethod(Description = "This method will retrieve deliveries/collections based on certain search criteria.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetDeliverySchedules(DateTime fromDate,
            DateTime toDate,
            string deliveryArea,
            int includeDeliveries,
            int includeCollections,
            string majorProductCategory,
            string minorProductCategory,
            string acctNo,
            int user,
            int branchNo,
            int delNotBranchNo,
            string truckID,
            bool includeAssembly,
            bool includeNonAssembly,
            out DateTime timeLocked,
            out string err)
        {
            Function = "BAccountManager::GetDeliveryOrders()";
            DataSet ds = null;
            timeLocked = DateTime.Now;
            err = "";
            try
            {
                BDelivery delivery = new BDelivery();
                ds = delivery.GetDeliverySchedules(fromDate,
                    toDate,
                    deliveryArea,
                    includeDeliveries,
                    includeCollections,
                    majorProductCategory,
                    minorProductCategory,
                    acctNo,
                    user,
                    branchNo,
                    delNotBranchNo,
                    truckID,
                    includeAssembly,
                    includeNonAssembly,
                    out timeLocked);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                //timeLocked = returnTimeLocked;
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet LoadAvailableTransPicklists(short branchNo, out string err)
        {
            Function = "BAccountManager::LoadAvailableTransPicklists()";
            DataSet ds = null;
            err = "";
            try
            {
                BDelivery del = new BDelivery();
                ds = del.LoadAvailableTransPicklists(branchNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method loads a list of schedules (delivery notes) by buffno")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetTransPickListDetails(short branchNo, int transPickickListNo, out string err)
        {
            Function = "WAccountManager::GetTransPickListDetails()";
            err = "";
            DataSet ds = null;
            try
            {
                BSchedule sched = new BSchedule();
                ds = sched.GetTransPickListDetails(branchNo, transPickickListNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method loads a list of revised schedules")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetRevisedSchedules(short branchNo, int loadNo, int pickNo,
            DateTime reviseFrom, DateTime reviseTo, int user, out DateTime timeLocked,
            out string err)
        {
            Function = "WAccountManager::GetRevisedSchedules()";
            err = "";
            DataSet ds = null;
            DateTime returnTimeLocked = DateTime.Now;
            try
            {
                BSchedule sched = new BSchedule();
                sched.User = user;
                ds = sched.GetRevisedSchedules(branchNo, loadNo, pickNo, reviseFrom,
                                                reviseTo, out returnTimeLocked);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                timeLocked = returnTimeLocked;
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetRevisedScheduleDetails(string acctNo, int buffNo, out string err)
        {
            Function = "WAccountManager::GetRevisedScheduleDetails()";
            err = "";
            DataSet ds = null;
            try
            {
                BSchedule sched = new BSchedule();
                ds = sched.GetRevisedScheduleDetails(acctNo, buffNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetRevisedScheduleChanges(string acctNo, int buffNo, int itemID, short locn, out string err)     //IP - 18/06/11 - CR1212 - RI - #4042
        {
            Function = "WAccountManager::GetRevisedScheduleChanges()";
            err = "";
            DataSet ds = null;
            try
            {
                BSchedule sched = new BSchedule();
                ds = sched.GetRevisedScheduleChanges(acctNo, buffNo, itemID, locn);                                     //IP - 18/06/11 - CR1212 - RI - #4042
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int ConfirmScheduleChanges(short loadNo, int pickListNo, int pickListBranch,
            //string acctNo, int agrmtno, string itemNo, short locn, int buffNo, int origBuffNo,
            string acctNo, int agrmtno, int itemID, short locn, int buffNo, int origBuffNo,                     //IP - 18/06/11 - CR1212 - RI - #4042
                                                                                                                //string removal, string origItemNo, int tranSchedNo, int tranSchedNoBranch,
            string removal, int origItemID, int tranSchedNo, int tranSchedNoBranch,                             //IP - 20/06/11 - CR1212 - RI - #4042     
            out string err)
        {
            Function = "BAccountManager::ConfirmScheduleChanges()";
            err = "";
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BSchedule sched = new BSchedule();
                            sched.User = STL.Common.Static.Credential.UserId;
                            sched.ConfirmScheduleChanges(conn, trans, loadNo, pickListNo, pickListBranch,
                                //acctNo, agrmtno, itemNo, locn, buffNo, origBuffNo, removal,
                                acctNo, agrmtno, itemID, locn, buffNo, origBuffNo, removal,                         //IP - 18/06/11 - CR1212 - RI - #4042
                                                                                                                    //origItemNo, tranSchedNo, tranSchedNoBranch);
                                 origItemID, tranSchedNo, tranSchedNoBranch);                                       //IP - 20/06/11 - CR1212 - RI - #4042
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetAdditionalItems(string acctNo, short loadNo, int pickListNo, int pickListBranch, out string err)
        {
            Function = "WAccountManager::GetAdditionalItems()";
            err = "";
            DataSet ds = null;
            try
            {
                BSchedule sched = new BSchedule();
                ds = sched.GetAdditionalItems(acctNo, loadNo, pickListNo, pickListBranch);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int DeleteDeliveryLoad(short stockLocn, int buffNo, out string err)
        {
            Function = "BAccountManager::DeleteDeliveryLoad()";
            err = "";
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BDelivery del = new BDelivery();
                            del.DeleteDeliveryLoad(conn, trans, stockLocn, buffNo);

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public decimal GetGiroPending(string accountNo, out string err)
        {
            Function = "WAccountManager::GetGiroPending()";
            err = "";
            decimal pending = 0;

            try
            {
                BDDMandate dd = new BDDMandate();
                pending = dd.GetGiroPending(accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return pending;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool IsWarrantyRenewal(string accountNo, out string err)
        {
            Function = "WAccountManager::IsWarrantyRenewal()";
            err = "";
            bool cancelled = false;

            try
            {
                BAccount acct = new BAccount();
                cancelled = acct.IsWarrantyRenewal(accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return cancelled;
        }
        // Check for valid decimal quantity
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool ValidDecimal(int itemId, out string err)
        {
            Function = "WAccountManager::ValidDecimal()";
            err = "";
            bool decimalpoint = false;

            try
            {
                BAccount dec = new BAccount();
                decimalpoint = dec.ValidDecimal(itemId);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return decimalpoint;
        }



        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet FincoBalances(DateTime datefrom, DateTime dateto,
                                           out string err)
        {
            Function = "WAccountManager::FincoBalances()";
            err = "";
            DataSet ds = null;


            using (var conn = new SqlConnection(Connections.Default))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        BAccount wr = new BAccount();
                        ds = wr.FincoBalances(conn, trans, datefrom, dateto);
                    }
                }
                catch (Exception ex)
                {
                    Catch(ex, Function, ref err);
                }
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int DeleteSpiff(string acctNo, string itemNo, short stockLocn, out string err)
        {
            Function = "BAccountManager::DeleteSpiff()";
            err = "";
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BSalesCommission sc = new BSalesCommission();
                            sc.DeleteSpiff(conn, trans, acctNo, itemNo, stockLocn);

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetSpiffs(string itemNo, short stocklocn, int itemId, out string err)
        {
            Function = "WAccountManager::GetSpiffs()";
            err = "";
            DataSet ds = null;
            try
            {
                BSalesCommission sc = new BSalesCommission();
                ds = sc.GetSpiffs(itemNo, stocklocn, itemId);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet WarrantySalesReport(string warrantyType, string branch, string salesPerson, string categorySet,
                                           short includeCash, short includeCredit, short includeSpecial, DateTime dateFrom,
                                           DateTime dateTo, short branchTotal, short categoryTotal, short salesPersonTotal,
                                           short acctTypeTotal, string datesAre, int liveDatabase, short includeRep, short includeCanc, out string err)
        {
            Function = "WAccountManager::WarrantySalesReport()";
            err = "";
            DataSet ds = null;

            SqlConnection conn = null;


            try
            {
                // This web service can connect to one of two possible database connections
                if (liveDatabase == 1)
                    conn = new SqlConnection(Connections.Default);
                else
                    conn = new SqlConnection(Connections.Report);

                using (conn)
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        BWarrantyReport wr = new BWarrantyReport();
                        ds = wr.WarrantySalesReport(warrantyType, branch, salesPerson, categorySet, includeCash,
                                                    includeCredit, includeSpecial, dateFrom, dateTo, branchTotal,
                                                    categoryTotal, salesPersonTotal, acctTypeTotal, datesAre, includeCanc, includeRep);

                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return ds;
        }




        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet FincoTransactions(DateTime datefrom, DateTime dateto, string transtypeset,
                                           out string err)
        {
            Function = "WAccountManager::FincoBalances()";
            err = "";
            DataSet ds = null;

            using (var conn = new SqlConnection(Connections.Default))
            {
                try
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        BAccount wr = new BAccount();
                        ds = wr.FincoTransactions(conn, trans, datefrom, dateto, transtypeset);
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    Catch(ex, Function, ref err);
                }
            }
            return ds;
        }

        [WebMethod(Description = "This method will return the store type of a particular branch")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string GetStoreType(short branchNo, out string err)
        {
            Function = "WAccountManager::GetStoreType()";
            err = string.Empty;
            string storeType = string.Empty;
            try
            {
                BBranch branch = new BBranch();
                storeType = branch.GetStoreType(branchNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return storeType;
        }

        [WebMethod(Description = "This method will return Instant Credit Approval")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        // Instant Credit Approval          CR907  jec 31/07/07
        public string InstantCredit(string customerID, string accountNo, out string err)
        {
            Function = "WAccountManager::InstantCredit()";
            err = string.Empty;
            string approved = string.Empty;
            try
            {
                BAccount instcr = new BAccount();
                approved = instcr.InstantCredit(customerID, accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return approved;
        }



        [WebMethod(Description = "This method will save an instant credit flag")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        // Instant Credit Flag         
        public int SaveInstantCreditFlag(string custID, string chkType, string accountNo, out string err)
        {
            Function = "WAccountManager::SaveInstantCreditFlag()";
            err = string.Empty;

            using (SqlConnection conn = new SqlConnection(Connections.Default))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        BAccount ba = new BAccount();
                        ba.User = STL.Common.Static.Credential.UserId;
                        ba.SaveInstantCreditFlag(conn, trans, custID, chkType, accountNo);
                        trans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    Catch(ex, Function, ref err);
                }
            }
            return 0;
        }



        [WebMethod(Description = "This method will Auto DA Instant Credit Approvals")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        // Instant Credit Approval          CR907  jec 31/07/07
        public int AutoDA(string accountNo, out string err)
        {
            Function = "WAccountManager::AutoDA()";
            err = string.Empty;

            using (SqlConnection conn = new SqlConnection(Connections.Default))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        BTransaction da = new BTransaction();
                        int user = STL.Common.Static.Credential.UserId;
                        da.AutoDA(conn, trans, accountNo, user);

                        trans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    Catch(ex, Function, ref err);
                }
            }
            return 0;
        }

        //[Conditional("DEBUG")]
        //private void logTime(string started, string ended)
        //{
        //    //logPerformanceMessage(Function + "Started at : " + started + " and Ended at : " + ended, User.Identity.Name, Environment.StackTrace, EventLogEntryType.Information);
        //}

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        //IP - 26/08/09 - UAT(819) - Added bool strategyHasWorklists
        public DataSet LoadCollectionsByacctno(string acctNo, out bool strategyHasWorklists,
                                     out string err)
        {
            Function = "WAccountManager::LoadCollectionsByacctno()";
            strategyHasWorklists = false;
            err = "";
            DataSet ds = null;

            try
            {
                BAccount Ac = new BAccount();
                ds = Ac.LoadStrategiesandWorklistsByAcctNo(acctNo, out strategyHasWorklists);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }


        /* Issue 69238 - SC 3/9/07
              * Get agreement number so cash and go accounts with agreement numbers greater than 1
              * line items can be viewed. */
        [WebMethod(Description = "This method returns the agreement number for the account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int GetAgreementNo(string accountNo, out string err)
        {
            Function = "WAccountManager::GetAgreementNo()";
            err = "";
            int agreementNo = 0;
            try
            {
                BAccount acct = new BAccount();
                agreementNo = acct.GetAgreementNo(null, null, accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return agreementNo;
        }

        [WebMethod(Description = "This method returns whether an item is a gift")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool IsGiftItem(int itemId, string location, out string err)     // RI
        {
            Function = "WAccountManager::IsGiftItem";
            err = "";
            bool isGift = false;
            try
            {
                BAccount acct = new BAccount();
                isGift = acct.IsGiftItem(itemId, location);     // RI
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return isGift;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetIRItems(string acctNo, string custID, int buffNo,
                        DateTime dateFrom, DateTime dateTo, string acctType, out string err)
        {
            Function = "WAccountManager::GetIRItems()";
            err = "";
            DataSet ds = null;
            try
            {
                BDelivery del = new BDelivery();
                ds = del.GetIRItems(acctNo, custID, buffNo, dateFrom, dateTo, acctType);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool IsDiscount(string itemNo, out string err)
        {
            Function = "WAccountManager::IsDiscount()";
            err = "";
            bool isDiscount = false;

            try
            {
                BItem item = new BItem();
                isDiscount = item.IsDiscountItem(itemNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return isDiscount;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int AddAdditionalSpiff(string acctNo, int authorisedBy, string itemNo, short stockLocn,
                                      decimal amount, int agrmtNo, int salesPerson, out string err)
        {
            Function = "BAccountManager::AddAdditionalSpiff()";
            err = "";
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BSalesCommission sc = new BSalesCommission();
                            sc.AddAdditionalSpiff(conn, trans, acctNo, authorisedBy, itemNo, stockLocn, amount, agrmtNo, salesPerson);

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveInsuranceWarrantyReturns(DataSet ds, string err)
        {
            //CR 822 created to save warranty returns
            Function = "SaveInsuranceWarrantyReturns";


            BDelivery del = new BDelivery();
            ;
            err = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        del.SaveInsuranceReturns(conn, trans, ds);
                        trans.Commit();                                             //IP - 08/06/11 - CR1212 - RI
                    }


                }

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0;
        }



        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetWorklistAccounts(int empeeNo, string worklist, bool viewTop500, out string err) //IP - 12/11/09 UAT5.2 (882) - added control to either return top 500 or all accounts
        {
            Function = "BAccountManager::GetWorklistAccounts()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetWorklistAccounts(empeeNo, worklist, viewTop500);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetWorklistAccountsData(string acctNo, string storeType, out string err)
        {
            Function = "BAccountManager::GetWorklistAccountsData()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetWorklistAccountsData(acctNo, storeType);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }


        [WebMethod(Description = "This method returns if an account had ever had a bad debt written off.")]
        [SoapHeader("authentication")]
#if(TRACE)
			[TraceExtension]
#endif
        public bool GetAccountHasBDW(string accountNo, out string err)
        {
            Function = "WAccountManager::GetAccountHasBDW()";
            err = "";
            bool accountHasBDW = false;

            try
            {
                BAccount acct = new BAccount();
                accountHasBDW = acct.GetAccountHasBDW(accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return accountHasBDW;
        }


        //IP - 28/11/2007 - 69360
        //Method returns 'True' or 'False' when checking for scheduled records for an account.
        //Returns true if records are found.
        [WebMethod(Description = "This method returns a true if an account has schedule records.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool AccountScheduleExists(string accountNo, out string err)
        {
            Function = "WAccountManager::AccountScheduleExists()";
            err = "";
            bool hasSchedule = false;

            SqlConnection conn = null;


            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BSchedule bsched = new BSchedule();
                            hasSchedule = bsched.AccountScheduleExists(conn, trans, accountNo);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return hasSchedule;
        }


        //IP - 19/05/08 - Method that returns 'Application Status' as well as other 
        //account details to be displayed in the 'Account Status' screen.

        [WebMethod(Description = "This method returns 'Account Status(delivery status)details, as well as other account details")]
        [SoapHeader("authentication")]

#if(TRACE)
		[TraceExtension]    
#endif
        public DataTable
            AccountStatusGet(DateTime dateFrom, DateTime dateTo, int branchno, out string error)
        {
            Function = "WAccountManager::AccountStatusGet()";
            error = "";
            DataTable accountStatusDet = new DataTable();

            try
            {
                BAccount bacct = new BAccount();
                accountStatusDet = bacct.AccountStatusGet(dateFrom, dateTo, branchno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref error);
            }

            return accountStatusDet;
        }

        [WebMethod]
        [SoapHeader("authentication")]
        public AccountDetailsResponse GetAccountDetailsData(string acctNo, int agreementNo, string storeType, string branchNo, string countryCode)
        {
            DateTime start = DateTime.Now;
            string err;
            string custID;

            string accountType;
            DateTime dateAcctOpen = DateTime.MinValue;
            string termsType = string.Empty;


            // GetPaidAndTakenAccount
            AccountDetailsResponse response = new AccountDetailsResponse();
            response.PaidAndTakenAccount = this.GetPaidAndTakenAccount(branchNo, out err);

            // GetCustomerAccountsDetails
            WCustomerManager customerManager = new WCustomerManager();
            response.CustomerAccounts = customerManager.GetCustomerAccountsDetailsList(acctNo, out err);

            custID = response.CustomerAccounts[0].ToString();

            // IsPrivilegeMember
            bool privilegeClub;
            string privilegeClubCode;
            string privilegeClubDesc;
            bool hasDiscount;
            customerManager.IsPrivilegeMember(custID, out privilegeClub, out privilegeClubCode,
                out privilegeClubDesc, out hasDiscount, out err);

            response.PrivilegeClub = privilegeClub;
            response.PrivilegeClubCode = privilegeClubCode;
            response.PrivilegeClubDesc = privilegeClubDesc;
            response.HasDiscount = hasDiscount;

            // GetCustomerAddresses
            response.CustomerAddresses = customerManager.GetCustomerAddresses(custID, out err);

            // CustomerSearch
            response.CustomerSearch = customerManager.CustomerSearch(custID, String.Empty, String.Empty, String.Empty, String.Empty, 100, 1, true,      //CR1084
                                                    storeType, String.Empty,out err);

            // GetServerDate
            WStaticDataManager staticDataManager = new WStaticDataManager();
            response.ServerDate = staticDataManager.GetServerDate();

            // GetAgreement
            response.AgreementData = GetAgreement(acctNo, agreementNo, false, out err);

            // todo may be better to return these values than extract on client
            DataTable dt = response.AgreementData.Tables[TN.AccountDetails];
            if (dt.Rows.Count > 0) //prevent error when no rows are brought back
            {
                if (DBNull.Value != dt.Rows[0]["Account Type"])
                    accountType = (string)dt.Rows[0]["Account Type"];
                if (DBNull.Value != dt.Rows[0]["Account Opened"])
                    dateAcctOpen = Convert.ToDateTime((DateTime)dt.Rows[0]["Account Opened"]);
            }
            dt = response.AgreementData.Tables[TN.Agreements];
            if (dt.Rows.Count > 0)
            {
                if (DBNull.Value != dt.Rows[0][CN.AgrmtNo])
                    agreementNo = (int)dt.Rows[0][CN.AgrmtNo];
                termsType = (string)dt.Rows[0]["Terms Type"];
            }
            // GetTermsTypeDetails
            response.TermsTypeDetails = GetTermsTypeDetails(countryCode, termsType, acctNo, dateAcctOpen, out err);

            // GetTransactions
            response.TransactionsData = GetTransactions(acctNo, out err);

            // GetRFCombined
            response.RFCombinedDetails = customerManager.GetRFCombinedDetails(custID, out err);

            // GetDeliveries
            response.DeliveriesData = GetDeliveries(acctNo, agreementNo, out err);

            // GetPaymentAccounts
            WPaymentManager paymentManager = new WPaymentManager();
            decimal addToValue;
            response.PaymentAccounts = paymentManager.GetPaymentAccounts(custID, countryCode, false, out addToValue, out err);

            response.AddToValue = addToValue;

            // IsCancelled
            response.IsCancelled = IsCancelled(acctNo, out err);

            // GetServiceRequestSummaryForAccount
            WServiceManager serviceManager = new WServiceManager();
            response.ServiceRequestSummaryForAccount = serviceManager.GetServiceRequestSummaryForAccount(acctNo, out err);

            // GetCustomerPhoto
            response.CustomerPhoto = customerManager.GetCustomerPhoto(custID, out err);

            // GetCustomerSignature
            response.CustomerSignature = customerManager.GetCustomerSignature(custID, out err);

            // GetLetterByAcctNo
            response.LetterByAcctNo = GetLetterByAcctNo(acctNo, out err);

            // GetStatusForAccount
            response.StatusForAccount = GetStatusForAccount(acctNo, out err);

            TimeSpan span = (start - DateTime.Now);
            return response;
        }

        public class AccountDetailsResponse
        {
            public string err;
            //GetPaidAndTakenAccount
            public string PaidAndTakenAccount;
            //GetCustomerAccountsDetails
            public ArrayList CustomerAccounts;
            //IsPrivilegeMember
            public bool PrivilegeClub;
            public string PrivilegeClubCode;
            public string PrivilegeClubDesc;
            public bool HasDiscount;
            // GetCustomerAddresses
            public DataSet CustomerAddresses;
            // CustomerSearch
            public DataSet CustomerSearch;
            // GetServerDate
            public DateTime ServerDate;
            // GetAgreement
            public DataSet AgreementData;
            // GetTermsTypeDetails
            public DataSet TermsTypeDetails;
            // GetTransactions
            public DataSet TransactionsData;
            // RFCombinedDetails
            public DataSet RFCombinedDetails;
            // GetDeliveries
            public DataSet DeliveriesData;
            // GetPaymentAccounts
            public DataSet PaymentAccounts;
            public decimal AddToValue;
            // IsCancelled
            public bool IsCancelled;
            // GetServiceRequestSummaryForAccount
            public DataSet ServiceRequestSummaryForAccount;
            // GetCustomerPhoto
            public string CustomerPhoto;
            // GetCustomerSignature
            public string CustomerSignature;
            // LetterByAcctNo
            public DataSet LetterByAcctNo;
            // GetStatusForAccount
            public DataSet StatusForAccount;
        }

        //IP - 06/10/08 - Special Arrangements screen (Credit Collections)
        //Method calculates the SPA Arrangement Schedule for the account and 
        //returns a data table with the schedule to be displayed on the 'Special Arrangements' screen.
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataTable SPACalculateArrangementSchedule(string acctNo,
                                                         char period,
                                                         decimal arrangementAmt,
                                                         int numberOfInstalments,
                                                         decimal instalmentAmt,
                                                         decimal oddPaymentAmt,
                                                         DateTime firstPaymentDate,
                                                         int numberRemainInstals,
                                                         decimal remainInstalAmt,
                                                         out DateTime finalPayDate,
                                                         out string err)
        {
            Function = "BAccount::SPACalculateArrangementSchedule()";
            //SqlConnection conn = null;
            //SqlTransaction trans = null;
            BAccount bacct = null;
            err = "";

            DataTable arrangementSchedule = new DataTable();
            finalPayDate = new DateTime(1900, 1, 1);

            try
            {
                //conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        //conn.Open();
                        //trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        bacct = new BAccount();
                        arrangementSchedule = bacct.SPACalculateArrangementSchedule(//conn, trans,
                                                            acctNo, period,
                                                            arrangementAmt, numberOfInstalments, instalmentAmt,
                                                                oddPaymentAmt, firstPaymentDate,
                                                                numberRemainInstals, remainInstalAmt, out finalPayDate);
                        //trans.Commit();
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock && retries < maxRetries)
                        {
                            retries++;
                            logMessage("Deadlock captured in " + Function + " Retry count = " + retries, STL.Common.Static.Credential.UserId.ToString(), EventLogEntryType.Error);
                        }
                        else
                            throw;
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            //finally
            //{
            //    if (conn.State != ConnectionState.Closed)
            //        conn.Close();
            //}
            return arrangementSchedule;
        }

        //IP - 05/02/09 - CR971 - Method will Unarchive/ un-settle accounts
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public string UnarchiveUnsettle(string acctNo, bool archivedAcct, bool unsettleAcct, out string err)
        {
            Function = "BAccount::UnarchiveUnsettle()";
            SqlConnection conn = null;
            BAccount bacct = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (var trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            bacct = new BAccount();
                            bacct.UnarchiveUnsettle(conn, trans, acctNo, archivedAcct, unsettleAcct);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }

            return err;

        }

        [WebMethod(Description = "This method returns AR Invoice details")]
        [SoapHeader("authentication")]
        [XmlInclude(typeof(ARInvoiceLine))]

#if(TRACE)
		[TraceExtension]    
#endif
        public ARInvoice[] Invoices()
        {
            Function = "WAccountManager::Invoices()";
            string error = "";

            ARInvoice[] invoices = null;

            try
            {
                invoices = ARInvoice.Populate();

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref error);
            }

            return invoices;
        }

        [WebMethod(Description = "This method returns Receipt details")]
        [SoapHeader("authentication")]
        [XmlInclude(typeof(Receipt))]

#if(TRACE)
		[TraceExtension]    
#endif
        public Receipt[] Receipt_1()
        {
            Function = "WAccountManager::Receipts()";
            string error = "";

            Receipt[] receipts = null;

            try
            {
                receipts = Receipt.Populate();

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref error);
            }

            return receipts;
        }

        [WebMethod(Description = "This method returns Customer details")]
        [SoapHeader("authentication")]
        [XmlInclude(typeof(STL.BLL.OracleIntegration.Customer))]

#if(TRACE)
		[TraceExtension]    
#endif
        public STL.BLL.OracleIntegration.Customer[] Customer_1()
        {
            Function = "WAccountManager::Receipts()";
            string error = "";

            STL.BLL.OracleIntegration.Customer[] customer = null;

            try
            {
                customer = STL.BLL.OracleIntegration.Customer.Populate();

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref error);
            }

            return customer;
        }

        [WebMethod(Description = "This method returns all data for Oracle interface")]
        [SoapHeader("authentication")]
        //[XmlInclude(typeof(ARInvoiceLine))]

#if(TRACE)
		[TraceExtension]    
#endif
        public DataContainer Oracledata()
        {
            Function = "WAccountManager::Oracledata()";
            string error = "";

            DataContainer data = new DataContainer();

            try
            {
                int runNo = 0;
                string result = "P";
                bool rerun = false;
                var filedate = "";

                DInterfaceControl st = new DInterfaceControl();
                runNo = st.StartNextRun("Oracle", "Oracle", out rerun, out filedate);       //RI jec 13/04/11

                data.invoice = ARInvoice.Populate();
                data.customers = STL.BLL.OracleIntegration.Customer.Populate();
                data.receipts = Receipt.Populate();

                // Clear export tables
                DOracleIntegration oi = new DOracleIntegration();
                oi.ClearOracleData();

                // Mark the Interface Control entry as finished
                DInterfaceControl end = new DInterfaceControl();
                end.SetRunComplete("Oracle", "Oracle", runNo, result);

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref error);
            }

            return data;
        }

        //        [WebMethod]
        //        [SoapHeader("authentication")]
        //#if (TRACE)
        //		[TraceExtension]
        //#endif
        //        public System.Data.DataSet InvoiceAccountsSearch(Int32 BranchNo, string InvoiceDateFrom, string InvoiceDateTo, string InvoiceNo, string accountNo, out int accountExists, out string accountType, out string err)
        //        {
        //            //object[] results = this.Invoke("InvoiceAccountsSearch", new object[] {
        //            //            BranchNo,
        //            //            InvoiceDateFrom,
        //            //            InvoiceDateTo,
        //            //            InvoiceNo,
        //            //            accountNo,
        //            //            });
        //            //accountExists = ((int)(results[1]));
        //            //accountType = ((string)(results[2]));
        //            //err = ((string)(results[3]));
        //            //return ((System.Data.DataSet)(results[0]));
        //            return null;
        //        }


        /// <summary>
        /// raj Changes
        /// </summary>
        /// <param name="BranchNo"></param>
        /// <param name="InvoiceDateFrom"></param>
        /// <param name="InvoiceDateTo"></param>
        /// <param name="InvoiceNo"></param>
        /// <param name="accountNo"></param>
        /// <param name="accountExists"></param>
        /// <param name="accountType"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        [WebMethod(Description = "This method returns a dataset containing all accounts for a particular customer ID")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet InvoiceAccountsSearch(int BranchNo,
            DateTime InvoiceDateFrom,
            DateTime InvoiceDateTo,
            string InvoiceNo,
            string accountNo
            //out int accountExists,
            //out string accountType,
            //out string err)
            //int Pageindex, 
            //int PageSize,
            //out int RecordCount

            )
        {
            Function = "BAccount::InvoiceAccountsSearch()";
            DataSet ds = null;
            //err = "";
            //accountExists = 0;
            //accountType = "";
            // RecordCount = 0;

            try
            {
                BAccount account = new BAccount();
                //err = "";
                ds = account.InvoiceAccountsSearch(BranchNo,
                    InvoiceDateFrom,
                    InvoiceDateTo,
                    InvoiceNo,
                    accountNo
                    //out accountExists,
                    //out accountType,
                    //out err);
                    //Pageindex,
                    // PageSize,
                    // out RecordCount

                    );
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }




        //IP - 18/02/09 - CR929 & CR974 - Deliveries
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int DeleteDeliveryNoteAndItems(short branchNo, string countryCode, DataTable dnItemsToDelete, DataTable dnItemsLinkedNonStocks,
                                                short stockLocn, int buffNo, int empeeNo, string reason, ref bool allItemsCollected, out string err)
        {
            Function = "WAccountManager::DeleteDeliveryNoteAndItems()";
            SqlConnection conn = null;
            err = "";
            allItemsCollected = false;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (var trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BSchedule schedule = new BSchedule();
                            schedule.User = STL.Common.Static.Credential.UserId;
                            schedule.DeleteDeliveryNoteAndItems(conn, trans, branchNo, countryCode, dnItemsToDelete, dnItemsLinkedNonStocks,
                                stockLocn, buffNo, empeeNo, reason, ref allItemsCollected);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        //IP  -21/02/09 - CR929 & 974 - Deliveries
        //Method to retrieve warranties linked to the items that are being deleted 
        //on the Delivery Note.
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataTable GetNonStockLinkedToDNItems(
            short branchNo, int buffNo, out string err)
        {
            Function = "WAccountManager::GetNonStockLinkedToDNItems()";
            SqlConnection conn = null;
            err = "";
            DataTable _dnItemsLinkedNonStocks = new DataTable();

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (var trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BSchedule schedule = new BSchedule();
                            schedule.User = STL.Common.Static.Credential.UserId;
                            _dnItemsLinkedNonStocks = schedule.GetNonStockLinkedToDNItems(conn, trans, branchNo, buffNo);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return _dnItemsLinkedNonStocks;
        }


        //NM 24/02/2009
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public BLL.OracleIntegration2.OutboundDataContainer GetOracleOutboundSalesOrder(int runNo)
        {
            Function = "BCollectionsModule::GetOracleOutboundSalesOrder()";
            BLL.OracleIntegration2.OutboundDataContainer container = new BLL.OracleIntegration2.OutboundDataContainer();
            SqlConnection conn = null;

            string err = "";
            int newRunNo = runNo;

            try
            {
                conn = new SqlConnection(Connections.Default);
                BLL.OracleIntegration2.OutboundData objOutBound = new BLL.OracleIntegration2.OutboundData();

                try
                {
                    conn.Open();

                    if (runNo == 0)
                        newRunNo = objOutBound.GetNextRunNo("OrInteg2");

                    container = objOutBound.GetOrderAndDeliveries(conn, runNo, newRunNo);
                    container.RunNo = newRunNo;

                    if (runNo == 0)
                    {   // is no rows retrieved for both orders and financials then don't update runnnumber 
                        bool isDelete = ((container.SalesOrderList == null || container.SalesOrderList.Count == 0) &&
                                            (container.ReceiptList == null || container.ReceiptList.Count == 0) &&
                                           (container.ARInvoiceList == null || container.ARInvoiceList.Count == 0) &&
                                           (container.CustomerList == null || container.CustomerList.Count == 0));
                        if (isDelete)
                            container.RunNo = 0;
                        objOutBound.DeleteOrUpdateRunNo("OrInteg2", newRunNo, 'P', isDelete);
                    }
                }
                catch (SqlException ex)
                {
                    if (runNo == 0)
                        objOutBound.ResetRunNo(conn, newRunNo);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn != null && conn.State != ConnectionState.Closed)
                    conn.Close();
            }

            return container;
        }

        //NM 24/02/2009
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int UpdateStockInfo(string strCSV)
        {
            Function = "WAccountManager::UpdateStockInfo()";
            SqlConnection conn = null;

            string err = "";
            string interfaceName = "OrStockInfo";
            int updateCount = 0;
            try
            {
                conn = new SqlConnection(Connections.Default);

                BLL.OracleIntegration2.InboundData inboundData = new BLL.OracleIntegration2.InboundData();
                int runNo = inboundData.GetNextRunNo(interfaceName);

                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            updateCount = inboundData.UpdateStockInfo(conn, trans, strCSV);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);

                inboundData.DeleteOrUpdateRunNo(interfaceName, runNo, 'P', false);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return updateCount;
        }


        //NM 04/03/2009
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int UpdateStockPrice(string strCSV)
        {
            Function = "WAccountManager::UpdateStockPrice()";
            SqlConnection conn = null;

            string err = "";
            string interfaceName = "OrStockPrice";
            int updateCount = 0;
            try
            {
                conn = new SqlConnection(Connections.Default);

                BLL.OracleIntegration2.InboundData inboundData = new BLL.OracleIntegration2.InboundData();
                int runNo = inboundData.GetNextRunNo(interfaceName);

                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            updateCount = inboundData.UpdateStockPrice(conn, trans, strCSV);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);

                inboundData.DeleteOrUpdateRunNo(interfaceName, runNo, 'P', false);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return updateCount;
        }


        //NM 04/03/2009
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int UpdateStockQuantity(string strCSV)
        {
            Function = "WAccountManager::UpdateStockQuantity()";
            SqlConnection conn = null;

            string err = "";
            string interfaceName = "OrStockQty";
            int updateCount = 0;
            try
            {
                conn = new SqlConnection(Connections.Default);

                BLL.OracleIntegration2.InboundData inboundData = new BLL.OracleIntegration2.InboundData();
                int runNo = inboundData.GetNextRunNo(interfaceName);

                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            updateCount = inboundData.UpdateStockQuantity(conn, trans, strCSV);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);

                inboundData.DeleteOrUpdateRunNo(interfaceName, runNo, 'P', false);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return updateCount;
        }

        //NM 04/03/2009
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int UpdatePromoPrice(string strCSV)
        {
            Function = "WAccountManager::UpdatePromoPrice()";
            SqlConnection conn = null;

            string err = "";
            string interfaceName = "OrPromoPrice";
            int updateCount = 0;
            try
            {
                conn = new SqlConnection(Connections.Default);

                BLL.OracleIntegration2.InboundData inboundData = new BLL.OracleIntegration2.InboundData();
                int runNo = inboundData.GetNextRunNo(interfaceName);

                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            updateCount = inboundData.UpdatePromoPrice(conn, trans, strCSV);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);

                inboundData.DeleteOrUpdateRunNo(interfaceName, runNo, 'P', false);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return updateCount;
        }

        //NM 04/03/2009
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int UpdatePurchaseOrder(string strCSV)
        {
            Function = "WAccountManager::UpdatePurchaseOrder()";
            SqlConnection conn = null;

            string err = "";
            string interfaceName = "OrPO";
            int updateCount = 0;
            try
            {
                conn = new SqlConnection(Connections.Default);

                BLL.OracleIntegration2.InboundData inboundData = new BLL.OracleIntegration2.InboundData();
                int runNo = inboundData.GetNextRunNo(interfaceName);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            updateCount = inboundData.UpdatePurchaseOrder(conn, trans, strCSV);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);

                inboundData.DeleteOrUpdateRunNo(interfaceName, runNo, 'P', false);
                //Write the file
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return updateCount;
        }


        //NM 04/03/2009
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int UpdateWarrantyBand(string strCSV)
        {
            Function = "WAccountManager::UpdateWarrantyBand()";
            SqlConnection conn = null;

            string err = "";
            string interfaceName = "OrWarBand";
            int updateCount = 0;
            try
            {
                conn = new SqlConnection(Connections.Default);

                BLL.OracleIntegration2.InboundData inboundData = new BLL.OracleIntegration2.InboundData();
                int runNo = inboundData.GetNextRunNo(interfaceName);

                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            updateCount = inboundData.UpdateWarrantyBand(conn, trans, strCSV);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);

                inboundData.DeleteOrUpdateRunNo(interfaceName, runNo, 'P', false);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return updateCount;
        }

        //NM 09/03/2009
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int UpdateFreightCarrier(string strCSV)
        {
            Function = "WAccountManager::UpdateFreightCarrier()";
            SqlConnection conn = null;

            string err = "";
            string interfaceName = "OrFrCarrier";
            int updateCount = 0;
            try
            {
                conn = new SqlConnection(Connections.Default);

                BLL.OracleIntegration2.InboundData inboundData = new BLL.OracleIntegration2.InboundData();
                int runNo = inboundData.GetNextRunNo(interfaceName);

                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            updateCount = inboundData.UpdateFreightCarrier(conn, trans, strCSV);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);

                inboundData.DeleteOrUpdateRunNo(interfaceName, runNo, 'P', false);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return updateCount;
        }

        //IP - 08/10/09 - UAT(909) - Method to check if an account/accounts can be re-allocated to an employee
        [WebMethod(Description = "This method checks if account/accounts can be re-allocated to an employee")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool CheckCanReallocate(int countAcctsToRealloc, int EmployeeNo, ref int noCanAlloc, out string err)
        {
            Function = "BAccountManager::CheckCanReallocate()";
            err = "";
            SqlConnection conn = null;
            bool canReallocate = false;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (var trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            BAccount acct = new BAccount();
                            canReallocate = acct.CheckCanReallocate(conn, trans, countAcctsToRealloc, EmployeeNo, ref noCanAlloc);

                            trans.Commit();
                        }
                        break;

                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }

            return canReallocate;
        }

        //IP - 04/02/10 - CR1072 - 3.1.9 - Display Delivery Authorisation history in Account Details.
        /// <summary>
        /// Method that retrieves Delivery Authorisation History for an account.
        /// </summary>
        /// <param name="acctno"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataTable LoadDAHistory(string acctno, out string err)
        {
            Function = "BAccountManager::LoadDAHistory()";
            DataTable acctDaHistory = new DataTable();
            err = "";

            try
            {
                BAccount acct = new BAccount();
                acctDaHistory = acct.LoadDAHistory(acctno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return acctDaHistory;
        }

        //IP - 08/02/10 - CR1037 Merged - Malaysia Enhancements (CR1072)
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool IsPaidAndTakenAccount(string accountNo, out string err) //CR 1037 
        {
            Function = "WAccountManager::IsPaidAndTakenAccount()";
            err = "";
            bool rtnValue = false;
            try
            {
                BAccount acct = new BAccount();
                rtnValue = acct.IsPaidAndTakenAccount(accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return rtnValue;
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string LockCheckbyAccount(string acctno, string user) //CR 175 
        {
            Function = "WAccountManager::LockCheckbyAccount()";
            string err = "";

            string lockuser = "";

            try
            {
                BAccount acct = new BAccount();
                lockuser = acct.LockCheckbyAccount(acctno, user);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return lockuser;
        }


        //IP - 11/02/10 - CR1048 (Ref:3.1.2.5) Merged - Malaysia Enhancements (CR1072)
        [WebMethod(Description = "This method returns a dataset containing a table containing last payment method details for a Cash & Go sale.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetCashAndGoLastPayMethod(string acctNo, int agrmtNo, out string err)
        {
            Function = "BAccountManager::GetCashAndGoLastPayMethod()";
            DataSet ds = null;
            err = "";
            try
            {
                BAccount acct = new BAccount();
                ds = acct.GetCashAndGoLastPayMethod(acctNo, agrmtNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        //IP - 25/02/10 - CR1072 - Malaysia 3PL for Version 5.2 - Merged from v4.3
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        [return: XmlElement("bdpl")]
        public System.Collections.Generic.List<BranchDefaultPrintLocation> GetBranchDefaultPrintLocation()
        {
            using (SqlConnection conn = new SqlConnection(Connections.Default))
            {
                conn.Open();
                return new BBranch().GetBranchDefaultPrintLocation(conn, null);
            }
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public decimal ProvisionGetForAccount(string acctno)
        {
            BAccount acct = new BAccount();
            return acct.ProvisionGetForAccount(acctno);
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataTable MaxAction(string accountNo)
        {
            BAccount acct = new BAccount();
            return acct.MaxAction(accountNo);
        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public bool CheckSRAcct(string acctno)
        {
            BAccount acct = new BAccount();
            return acct.CheckSRAcct(acctno);
        }

        [WebMethod(Description = "This method brings back a list of non-stock items based on item number entered")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetNonStockByCode(string itemno, out string err)
        {
            Function = "BAccountManager::GetNonStockByCode()";
            err = "";
            //DataTable dt = null;
            DataSet ds = null;
            try
            {
                BItem item = new BItem();
                ds = item.GetNonStockByCode(itemno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method brings back a list of categories")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataTable GetCategories(out string err)
        {
            Function = "BAccountManager::GetCategoriese()";
            err = "";
            DataTable dt = null;
            try
            {
                BItem item = new BItem();
                dt = item.GetCategories();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return dt;
        }

        //CR1094 jec 10/12/10 
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public void SaveNonStockItem(DataTable nonstock, DataTable prices)
        {
            Function = "WAccountManager::SaveNonStockItem()";
            SqlConnection conn = null;

            string err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BItem ns = new BItem();
                            ns.SaveNonStockItem(conn, trans, nonstock, prices);

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }

        [WebMethod(Description = "This method brings back a list of Warranty Return Codes")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataTable GetWarrantyReturnCodes(out string err)
        {
            Function = "BAccountManager::GetWarrantyReturnCodes()";
            err = "";
            DataTable dt = null;
            try
            {
                BItem item = new BItem();
                dt = item.GetWarrantyReturnCodes();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return dt;
        }

        [WebMethod(Description = "This method saves a list of Warranty Return Codes")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void SaveWarrantyReturnCodes(DataTable returnCodes, DateTime dateNow, out string err)
        {
            Function = "BAccountManager::SaveWarrantyReturnCodes()";
            err = "";
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BItem wrc = new BItem();
                            wrc.SaveWarrantyReturnCodes(conn, trans, returnCodes, dateNow);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }
        [WebMethod(Description = "This method Saves and checks Parameters for an Account")]
        [SoapHeader("authentication")]
        public void NewAccountCreditSave(ref CreditParameters Parms)
        {
            string err = "";
            try
            {
                SqlConnection conn = new SqlConnection(Connections.Default);

                conn.Open();
                using (var trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    //IP - 28/02/11 - #3239 - Code re-instated
                    SaveNewAccountCreditParameters Parameters = new SaveNewAccountCreditParameters();
                    Mapper.CreateMap<CreditParameters, SaveNewAccountCreditParameters>();
                    Mapper.Map(Parms, Parameters);
                    Mapper.AssertConfigurationIsValid();
                    BAccount Acct = new BAccount();
                    Acct.NewAccountCreditSave(conn, trans, ref Parameters);
                    Mapper.CreateMap<SaveNewAccountCreditParameters, CreditParameters>();
                    Mapper.Map(Parameters, Parms);

                    trans.Commit();

                }

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
        }

        //        [WebMethod(Description = "This method Returns Account Details for an account")]
        //        [SoapHeader("authentication")]
        //#if(TRACE)
        //        [TraceExtension]
        //#endif
        //public Acct LoadAccount(Acct.Parameters.Load Parameters)
        //{

        //    AccountRepository account = new AccountRepository();
        //    var empeeno = STL.Common.Static.Credential.UserId; //IP - 03/03/11 - #3255 - Added empeeno
        //    return account.Get(Parameters, empeeno, null, null);

        //}


        [WebMethod(Description = "This method brings back a list of Warranty Items")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataTable GetAllWarrantyItems(out string err)
        {
            Function = "BAccountManager::GetAllWarrantyItems()";
            err = "";
            DataTable dt = null;
            try
            {
                BItem item = new BItem();
                dt = item.GetAllWarrantyItems();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return dt;
        }


        [WebMethod(Description = "This method will attempt to lock an account from a list for a specific user", MessageName = "AccountLockingFindandLockForCaller")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string AccountLockingFindandLockForCaller(string acctlist, string user, out string err)
        {
            Function = "BAccountManager::AccountLockingFindandLockForCaller()";
            err = "";
            DAccountParms Parms = new DAccountParms();

            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            Parms.conn = conn;
                            Parms.trans = trans;
                            Parms.AcctList = acctlist;
                            Parms.user = Convert.ToInt32(user);
                            Parms.RunDate = DateTime.Now;
                            DAccount acct = new DAccount();
                            acct.AccountLockingFindandLockForCaller(ref Parms);

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {

                Catch(ex, Function, ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return Parms.Acctno;
        }

        [WebMethod]
        [SoapHeader("authentication")]
        public bool? IsSRInstSpecialAccount(string accountNo, out string err)
        {
            err = "";

            try
            {
                Function = "WAccountManager::IsSRInstSpecialAccount()";
                return new AccountRepository().IsSRInstSpecialAccount(accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return null;
        }

        [WebMethod]
        [SoapHeader("authentication")]
        public bool? IsLatestAccountforCustomer(string accountNo, string CustomerId, out string err)
        {
            err = "";

            try
            {
                Function = "WAccountManager::IsLatestAccountforCustomer()";

                return new AccountRepository().IsLatestAccountforCustomer(accountNo, CustomerId);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return null;
        }

        //IP - 08/09/11 - RI - #8112
        [WebMethod(Description = "This method returns a count for the item passed into the method from the database")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int GetReturnItemIDForItemCode(string itemNo, short stockLocn, out string err)
        {
            Function = "BAccountManager::GetReturnItemIDForItemCode()";

            var returnItemID = 0;

            err = "";
            try
            {
                returnItemID = new StockRepository()
                                .GetReturnItemIDForItemCode(itemNo, stockLocn);


            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return returnItemID;
        }

        [WebMethod(Description = "")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void CalculateCashLoanTerms(string countryCode, ref Blue.Cosacs.Shared.CashLoanDetails det, short branchNo, bool calledFromCashLoanPopulateScreen,bool isMambuAccount, out string err)
        {
            Function = "BAccountManager::CalculateCashLoanTerms()";
            err = "";

            decimal serviceCharge = 0;
            decimal instalAmt = 0;
            decimal finalinstalAmt = 0;
            decimal adminTax = 0;
            decimal stampDuty = 0;
            decimal agreementTotal = 0;
            DataSet variableRatesSet = null;
            decimal insuranceChg = 0;
            decimal adminChg = 0;
            decimal monthly = 0;
            decimal final = 0;
            double adminTaxRate = 0;
            double insuranceTaxRate = 0;

            try
            {
                //If Admin Charge waived, use the Admin Charge user set in Cash Loan Application screen.
                if (det.waiveAdminCharge)
                {
                    adminChg = det.adminChg;
                }

                BAgreement agree = new BAgreement();
                if (!(bool)Country[CountryParameterNames.CL_Amortized])
                {
                    serviceCharge = agree.CalculateServiceCharge(null, null, countryCode, det.termsType, det.accountNo, det.scoreBand, Convert.ToDecimal(0), det.term,
                        det.loanAmount, DateTime.Now, "R", det.loanAmount,      // #10013
                        ref insuranceChg, ref adminChg, ref variableRatesSet, det.waiveAdminCharge, true);
                }
                else
                {
                    serviceCharge = agree.CalculateAmortizedInstalPlan(null, null, countryCode, det.termsType, det.accountNo, det.scoreBand, det.term,
                        det.loanAmount, DateTime.Now, "R", ref insuranceChg, ref adminChg, out monthly, out final, det.waiveAdminCharge);
                }

                det.serviceChg = serviceCharge;

                var adminItemId = StockItemCache.Get(StockItemKeys.AdminChargeItem);
                var insuranceItemId = StockItemCache.Get(StockItemKeys.InsuranceChargeItem);

                DStockItem itemDetails = new DStockItem();
                itemDetails.GetItemDetails(null, null, adminItemId, branchNo, "R", countryCode, false, false);

                if (calledFromCashLoanPopulateScreen == true && det.accountNo != "000000000000")
                {
                    BItem item = new BItem();
                    XmlNode adminTaxRateFromLineItem = item.GetItemDetails(adminItemId, branchNo, "R", countryCode, false, false, det.accountNo, 1);
                    adminTaxRate = Convert.ToDouble(adminTaxRateFromLineItem.Attributes[Tags.TaxRate].Value);
                }
                else
                {
                    adminTaxRate = itemDetails.TaxRate;
                }


                itemDetails.GetItemDetails(null, null, insuranceItemId, branchNo, "R", countryCode, false, false);

                if (calledFromCashLoanPopulateScreen == true && det.accountNo != "000000000000")
                {
                    BItem item = new BItem();
                    XmlNode insuranceTaxRateFromLineItem = item.GetItemDetails(insuranceItemId, branchNo, "R", countryCode, false, false, det.accountNo, 1);
                    insuranceTaxRate = Convert.ToDouble(insuranceTaxRateFromLineItem.Attributes[Tags.TaxRate].Value);
                }
                else
                {
                    insuranceTaxRate = itemDetails.TaxRate;
                }

                //if ((string)Country[CountryParameterNames.TaxType] == "E")
                //{
                det.insuranceTax = (insuranceChg * Convert.ToDecimal(insuranceTaxRate)) / 100;


                if (!(bool)Country[CountryParameterNames.CL_Amortized])
                {
                    det.adminTax = (adminChg * Convert.ToDecimal(adminTaxRate)) / 100;
                }
                else
                {
                    det.adminTax = adminChg - ((adminChg * 100) / (Convert.ToDecimal(adminTaxRate) + 100));
                }
                //}

                det.insuranceChg = insuranceChg;
                det.adminChg = adminChg;

                //Calculate agreement total
                // if ((bool)Country[CountryParameterNames.CL_Amortized])
                //   det.agreementTotal = det.loanAmount;
                // else
                if ((bool)Country[CountryParameterNames.CL_Amortized])
                {
                    if ((bool)Country[CountryParameterNames.CL_TaxRateApplied])
                    {
                        det.agreementTotal = det.loanAmount + det.serviceChg + det.insuranceChg + det.adminChg + det.insuranceTax + det.stampDuty;//+det.adminTax;   // #10013

                    }
                    else
                    {
                        det.agreementTotal = det.loanAmount + det.serviceChg + det.insuranceChg + det.adminChg + det.insuranceTax + det.stampDuty; //+det.adminTax;   // #10013

                    }
                }
                else
                {
                    det.agreementTotal = det.loanAmount + det.serviceChg + det.insuranceChg + det.adminChg + det.insuranceTax + det.stampDuty + det.adminTax;   // #10013

                }
                    //Calculate service charge for amortized cash loan accounts with instalment schedule
                    if (!(bool)Country[CountryParameterNames.CL_Amortized])
                        this.CalculateInstalPlan(det.loanAmount + det.stampDuty,            // #10013
                        0, det.serviceChg + 0 + det.insuranceChg + det.insuranceTax + det.adminChg + det.adminTax, (det.term - 0), out monthly, out final, out err);
                    //else
                    //{
                    //    serviceCharge = agree.CalculateAmortizedInstalPlan(null, null, countryCode, det.termsType, det.accountNo, det.scoreBand, det.term,
                    //        det.loanAmount, DateTime.Now, "R", out monthly, out final);
                    //    det.serviceChg = serviceCharge;
                    //}

                det.instalment = monthly;
                det.finInstal = final;

                if (isMambuAccount)
                {
                    BAccount bAccount = new BAccount();
                    bAccount.FetchMambuServiceChargeInstalDetails(null, null, det.accountNo, out serviceCharge, out instalAmt, out finalinstalAmt, out adminTax, out stampDuty, out agreementTotal);
                    det.serviceChg = serviceCharge;
                    det.instalment = instalAmt;
                    det.finInstal = finalinstalAmt;
                    det.adminTax = adminTax;
                    det.stampDuty = stampDuty;
                    det.insuranceTax = 0;
                    det.agreementTotal = agreementTotal;
                }
            }

            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

        }

        //Method to deliver Cash Loan Account
        [WebMethod(Description = "")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void CashLoanDeliverAccount(ref Blue.Cosacs.Shared.CashLoanDetails det, short branchNo,
                                                Blue.Cosacs.Shared.CashLoanDisbursementDetails CashLoanDisbursementDet, out string err)    //IP - 21/02/12 - #9626 - added parameter branchNo
        {
            Function = "BAccountManager::CashLoanDeliverAccount()";
            err = "";

            try
            {

                SqlConnection conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BPayment cashLoan = new BPayment();
                            BCustomer cust = new BCustomer();
                            cashLoan.User = STL.Common.Static.Credential.UserId;

                            cashLoan.DeliverCashLoan(conn, trans, ref det, CashLoanDisbursementDet);

                            //IP - 21/02/12 - #9626 - UAT90 - Insert record in the CashierDeposits table
                            cashLoan.CashLoanSaveToCashierDeposits(conn, trans, ref det, branchNo, CashLoanDisbursementDet.disbursementType);

                            //Insert a record into the CashLoan table (status of "D" to indicate loan has been disbursed)
                            CreditRepository c = new CreditRepository();
                            c.CashLoanStausInsert(det, conn, trans);

                            cust.SetAvailableSpend(conn, trans, det.custId);        //#8825 

                            trans.Commit();
                            
                            string accountno = CashLoanDisbursementDet.accountNo;

                            ProcessDeliveryWebApi processapi = new ProcessDeliveryWebApi();
                            PaymentWebApi api = new PaymentWebApi();

                            DataTable dt = api.GetMambuAccountsForPayment(null, accountno);

                            if (dt.Rows.Count > 0)
                            {
                                string json = processapi.PostProcess_Delivery(accountno);
                            }

                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

        }

        //Method to Update status on CashLoan - PromissoryNote printed
        [WebMethod(Description = "")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void CashLoanPromissoryNoteStatus(ref Blue.Cosacs.Shared.CashLoanDetails det, out string err)
        {
            Function = "BAccountManager::CashLoanPromissoryNoteStatus()";
            err = "";

            try
            {
                SqlConnection conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            //Insert a record into the CashLoan table (status of "P" to indicate Promissory note printed)
                            CreditRepository c = new CreditRepository();
                            c.CashLoanStausInsert(det, conn, trans);

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

        }

        //IP - 11/05/12 - #9611 - CR8520
        [WebMethod]
        [SoapHeader("authentication")]
        public void PrintAuditCashAndGo(string acctno, int agrmtNo, int empeeno, string type, bool taxExempt, decimal change, int payMethod, out string err) //IP - 22/05/12 - #10156 - Added payMethod
        {

            err = "";
            try
            {
                SqlConnection conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            EventStore.Instance.Log(new
                            {
                                AccountNo = acctno,
                                InvoiceNo = agrmtNo,
                                SalesPerson = empeeno
                            },
                            type,
                            EventCategory.CashAndGo);

                            if (type == AuditType.CashAndGoPrint)                        //IP - 16/05/12 - #9447 - CR1239
                            {
                                AccountRepository ar = new AccountRepository();
                                ar.InsertIntoCashAndGoReceipt(conn, trans, acctno, agrmtNo, taxExempt, change, STL.Common.Static.Credential.UserId, payMethod);     //IP - 22/05/12 - #10156
                            }

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

        }

        // #10327 Submit booking failure jec 08/0/12
        [WebMethod]
        [SoapHeader("authentication")]
        public bool SubmitBookingFailure(int failureId, int failedQty, int lineItemId, int retItemId, decimal retVal, int retStockLocn, int originalId, out string err) //#13604
        {

            err = "";
            var actioned = false;
            try
            {
                SqlConnection conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            WarehouseRepository wr = new WarehouseRepository();
                            actioned = wr.CheckBookingFailureNotAction(conn, trans, failureId);

                            if (!actioned)
                            {
                                var lineItemBooking = new DataTable();
                                lineItemBooking.Columns.Add("id");
                                lineItemBooking.Columns.Add("QtyBooked");
                                lineItemBooking.Columns.Add("BookingID");

                                var dr = lineItemBooking.NewRow();
                                dr["id"] = lineItemId;
                                dr["QtyBooked"] = failedQty;
                                dr["BookingID"] = failureId;

                                lineItemBooking.Rows.Add(dr);

                                //IP - 21/06/12 - #10479
                                //var bookingSchedule = new WarehouseRepository().GetLineItemBookingSchedule(conn, trans, lineItemId);      // #14638
                                var bookingSchedule = new WarehouseRepository().GetLineItemBookingScheduleFromBookingID(conn, trans, originalId);   // #14638

                                var delOrCol = bookingSchedule != null ? bookingSchedule.DelOrColl : failedQty > 0 ? "D" : "C";

                                //#13604 - we need to insert a new LineItemBookingSchedule record for the new booking
                                new WarehouseRepository().InsertLineItemBookingSchedule(conn, trans, Convert.ToInt32(lineItemBooking.Rows[0][CN.ID]), delOrCol, retItemId,
                                                                                                       retVal, Convert.ToInt16(retStockLocn), 0, delOrCol == "C" ? Convert.ToSingle(lineItemBooking.Rows[0][CN.QtyBooked]) * -1 : Convert.ToSingle(lineItemBooking.Rows[0][CN.QtyBooked]),
                                                                                                       Convert.ToInt32(bookingSchedule.ItemID), Convert.ToInt32(bookingSchedule.StockLocn), Convert.ToDecimal(bookingSchedule.Price)); //#13604

                                var AcctR = new AccountRepository();

                                AcctR.InsertLineItemBooking(conn, trans, ref lineItemBooking);

                                //AcctR.bookingType = failedQty > 0 ? "D" : "C";                      
                                AcctR.bookingType = delOrCol;         //IP - 21/06/12 - #10479


                                //#13604 - update the LineItemBookingSchedule record just added with the BookinId
                                new WarehouseRepository().UpdateLineItemBookingScheduleBookingId(conn, trans, Convert.ToInt32(lineItemBooking.Rows[0][CN.ID]), Convert.ToInt32(lineItemBooking.Rows[0]["BookingID"]));

                                var bookings = (IEnumerable<BookingSubmit>)AcctR.GetBookingData(conn, trans, lineItemBooking);


                                new Chub().SubmitMany(bookings, conn, trans);

                                wr.UpdateBookingFailureActioned(conn, trans, failureId, lineItemBooking);
                                EventStore.Instance.LogAsync(new { Shipments = bookings }, "Cosacs", "ShipmentFailureActioned");
                            }

                            trans.Commit();

                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return actioned;

        }

        [WebMethod]
        [SoapHeader("authentication")]
        public void CancelRedelivery(int origBookingId, int bookingId, out string err) //#13604
        {
            err = "";

            try
            {
                SqlConnection conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            WarehouseRepository wr = new WarehouseRepository();

                            wr.UpdateBookingFailureActioned(conn, trans, origBookingId, bookingId);

                            trans.Commit();

                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
        }

        public class OnlineProductSearchRequest
        {
            public string Location { get; set; }
            public short Category { get; set; }
            public string Online { get; set; }
            public DateTime DateAdded { get; set; }
            public DateTime DateRemoved { get; set; }
            public string ProductDesction { get; set; }
            public bool Limit { get; set; }
        }
        [WebMethod(Description = "This method brings back a list of stockitems based on location and any description entered")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet OnlineProductSearch(OnlineProductSearchRequest r, out string err)
        {
            Function = "BAccountManager::OnlineProductSearch()";
            err = "";
            DataSet ds = null;
            try
            {
                ds = new BItem()
                    .OnlineProductSearch(
                        r.Location,
                        r.Category,
                        r.Online,
                        r.DateAdded,
                        r.DateRemoved,
                        r.ProductDesction,
                        r.Limit);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        //#18125 - Mark booking as actioned when account revised from Failure Booking screen
        [WebMethod]
        [SoapHeader("authentication")]
        public bool ReviseBookingFailure(int originalId, out string err)
        {

            err = "";
            var actioned = false;
            try
            {
                SqlConnection conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            WarehouseRepository wr = new WarehouseRepository();
                            actioned = wr.CheckBookingFailureNotAction(conn, trans, originalId);

                            if (!actioned)
                            {
                                wr.UpdateBookingFailureActioned(conn, trans, originalId, 0);
                            }

                            trans.Commit();

                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return actioned;

        }

        //#17290 - Collect Instant Replacement Item
        [WebMethod]
        [SoapHeader("authentication")]
        public bool InstantReplacementCollection(XmlNode replacementxml, string accountno, short branchno, string countrycode, out string err)
        {
            err = "";
            var actioned = false;
            try
            {
                SqlConnection conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BAccount ba = new BAccount();
                            ba.Populate(accountno);
                            var taxexempt = ba.IsTaxExempt(conn, trans, accountno, "1");

                            ba.SaveReplacement(conn, trans, accountno, ba.AccountType, countrycode, branchno, taxexempt, replacementxml, "", 0);

                            trans.Commit();

                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return actioned;

        }


        //#18630 - CR15594
        [WebMethod]
        [SoapHeader("authentication")]
        public bool IsReadyAssistContractDateSet(string accountno, int agreementNo, out string err)
        {
            err = "";

            var dateSet = false;

            try
            {
                SqlConnection conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            dateSet = new AccountRepository().IsReadyAssistContractDateSet(conn, trans, accountno, agreementNo);

                            trans.Commit();

                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return dateSet;
        }


        [WebMethod(Description = "this method returns Sales Commission Details")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public SalesCommissionDetails GeSalesCommissionDetails(int? branchNo, int? empeeNo, DateTime dateFrom, DateTime dateTo)
        {

            string err = "";
            SalesCommissionDetails commDet = new SalesCommissionDetails();

            try
            {
                SqlConnection conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            commDet = new AccountRepository().GetSalesCommissionDetails(conn, trans, branchNo, empeeNo, dateFrom, dateTo);
                            trans.Commit();

                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return commDet;

        }


        [WebMethod(Description = "this method returns Branch Sales Commission Details")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public SalesCommissionDetails GetBranchSalesCommissionDetails(int? branchNo, DateTime dateFrom, DateTime dateTo)
        {

            string err = "";
            SalesCommissionDetails commDet = new SalesCommissionDetails();

            try
            {
                SqlConnection conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            commDet = new AccountRepository().GetBranchSalesCommissionDetails(conn, trans, branchNo, dateFrom, dateTo);
                            trans.Commit();

                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return commDet;

        }



        [WebMethod(Description = "this method returns Cash Loan Disbursement Details to the Cash Loan Disbursement Bank Transfer Record screen")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public CashLoanDisbursementDetailsView GetCashLoanDisbursementDetails(string acctNo, out string err)
        {
            err = string.Empty;

            CashLoanDisbursementDetailsView disbursementDetails = null;

            try
            {
                SqlConnection conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            disbursementDetails = new AccountRepository().GetCashLoanDisbursementDetails(conn, trans, acctNo);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return disbursementDetails;

        }

        [WebMethod(Description = "this method saves Bank Transfer details to the CashLoanDisbursement table")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void SaveCashLoanDisbursementBankTransfer(string acctNo, string bankTransRef, DateTime transferDate, out string err)
        {
            err = string.Empty;

            try
            {
                SqlConnection conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            new AccountRepository().CashLoanDisbursementBankTransferSave(conn, trans, acctNo, bankTransRef, transferDate);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

        }

        [WebMethod(Description = "this method saves Bank details for a Cash Loan")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void UpdateCashLoanBankDetails(string custID, string acctNo, string Bank, string BankAccountType, string BankBranch,
                                                string BankAcctNo, string Notes, string BankReferenceNo, string BankAccountName, out string err)
        {
            err = string.Empty;

            try
            {
                SqlConnection conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            new CreditRepository().UpdateCashLoanBankDetails(conn, trans, custID, acctNo, Bank, BankAccountType, BankBranch, BankAcctNo, Notes, BankReferenceNo, BankAccountName);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

        }


        [WebMethod]
        [SoapHeader("authentication")]
        public bool IsItemScheduled(int lineItemId, out string err)
        {
            err = "";

            var isItemScheduled = false;

            try
            {
                SqlConnection conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            isItemScheduled = new AccountRepository().IsItemScheduled(conn, trans, lineItemId);

                            trans.Commit();

                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return isItemScheduled;

        }

        [WebMethod(Description = "")]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public int GetDiscountDeliveryMonths(string custid, int itemId)
        {
            string err = "";
            var monthsPassed = 0;

            try
            {
                SqlConnection conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            monthsPassed = new AccountRepository().GetDiscountDeliveryMonths(custid, itemId);
                            trans.Commit();

                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return monthsPassed;
        }

        //CR 2018-13
        /// <summary>
        /// Suvidha Changes
        /// </summary>
        /// <param name="BranchNo"></param>
        /// <param name="InvoiceDateFrom"></param>
        /// <param name="InvoiceDateTo"></param>
        /// <param name="InvoiceNo"></param>
        /// <param name="accountNo"></param>
        /// <param name="accountExists"></param>
        /// <param name="accountType"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        [WebMethod(Description = "This method returns a dataset containing all Invoice Details")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetInvoiceDetails(string accountNo, string agrmtno, string agreementInvoiceNumber)
        {
            Function = "BAccount::GetInvoiceDetails()";
            DataSet ds = null;

            try
            {
                BCustomer cust = new BCustomer();
                ds = cust.GetInvoiceDetails(accountNo, agrmtno, agreementInvoiceNumber);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        [WebMethod]
        public void UpdateInvoiceVersion(string accountNo, int agreementNo, bool GRT = false, DataTable dtReturnItems = null)
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BItem bitem = new BItem();
                            bitem.UpdateInvoiceVersion(conn, trans, accountNo, agreementNo);

                            if(GRT == true)
                            {
                                BSchedule bschedule = new BSchedule();
                                bschedule.UpdateInvoiceVersionForGRT(conn, trans, accountNo, agreementNo, dtReturnItems);
                            }
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }

        [WebMethod(Description = "This method returns the latest invoice number and version for a particular account")]        [SoapHeader("authentication")]        public string GetInvoiceNumberWithVersion(string acctno, out string err)        {            Function = "BAccountManager::GetInvoiceNumberWithVersion()";            string result = string.Empty;            err = string.Empty;            try            {                var acct = new BAccount();                result = acct.GetInvoiveNumberWithVersion(acctno);            }            catch (Exception ex)            {                Catch(ex, Function, ref err);            }
            return result;        }

        [WebMethod(Description = "This method returns a dataset containing transactions of finId & its details")]
        [SoapHeader("authentication")]
        public DataSet GetTransactionDetails(int FinTransId)
        {
            Function = "BAccountManager::GetTransactionDetails()";
            DataSet ds = null;
            
            try
            {
                var acct = new BAccount();
                ds = acct.GetTransactionDetails(FinTransId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
              
        [WebMethod(Description = "This method will return the business title value for Printing Invoice.")]
        [SoapHeader("authentication")]
        public DataSet GetPrintingInfoForCL(string accountNo)
        {
            Function = "BAccountManager::GetPrintingInfoForCL()";
            DataSet ds = null;

            try
            {
                var acct = new BAccount();
                ds = acct.GetPrintingInfoForCL(accountNo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }


    }
}

