using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.Security;
using System.Xml;
using STL.DAL;
using STL.Common;
using STL.BLL;
using System.Data.SqlClient;
using System.Configuration;
using STL.Common.Constants.ScreenModes;
using System.Collections.Specialized;
using System.Collections.Generic;
using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared;
using Blue.Cosacs.ComLib;
using System.Linq;

namespace STL.WS
{
    /// <summary>
    /// Summary description for BCustomerManager.
    /// </summary>
    /// 
    [WebService(Namespace = "http://strategicthought.com/webservices/")]
    public class WCustomerManager : CommonService
    {
        public WCustomerManager()
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

        [WebMethod(Description = "this method returns the codes for a particular customer")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetCodesForCustomer(string customerID, out bool noSuchCust, out string err)
        {
            Function = "BCustomerManager::GetCodesForCustomer()";
            DataSet ds = null;
            err = "";
            noSuchCust = false;
            try
            {
                BCustomer cust = new BCustomer();
                ds = cust.GetCodesForCustomer(customerID, out noSuchCust);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method adds codes to a customer")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string AddCodesToCustomer(string customerID, DataSet ds, string custCode) //IP - 01/09/09 - 5.2 UAT(823) - Added custCode
        {

            SqlConnection conn = null;
            Function = "BCustomerManager::AddCodesToCustomer()";
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
                            BCustomer cust = new BCustomer();
                            cust.User = STL.Common.Static.Credential.UserId;
                            cust.AddCodesToAccount(conn, trans, customerID, ds, custCode);


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

        [WebMethod(Description = "this method returns the customer details and list of accounts for a customer.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetCustomerAccountsAndDetails(string accountNo, out string err)
        {
            Function = "BCustomerManager::GetCustomerAccountsAndDetails()";

            DataSet ds = null;
            //decimal StoreCardAvailable = 0.0m;
            err = "";
            SqlConnection conn = null;
            Function = "BCustomerManager::SaveBasicDetails()";
            err = "";
            string start = DateTime.Now.ToLongTimeString();

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

                            BCustomer custAccsAndDetails = new BCustomer();
                            ds = custAccsAndDetails.GetCustomerAccountsAndDetails(conn, trans, accountNo);
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


        [WebMethod(Description = "this method returns the customer details and list of accounts for a customer and also the storecard available amount")]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif

        public DataSet GetCustomerAccountsDetails(string accountNo, out string err)
        {
            Function = "BCustomerManager::GetCustomerAccountsDetailsAndSCardAvailable()";

            DataSet ds = null;
            err = "";
            SqlConnection conn = null;
            string start = DateTime.Now.ToLongTimeString();

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

                            BCustomer custAccsAndDetails = new BCustomer();
                            ds = custAccsAndDetails.GetCustomerAccountsAndDetails(conn, trans, accountNo);

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


        [WebMethod(Description = "this method returns the customer details and list of accounts for a customer as a datareader.")]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public ArrayList GetCustomerAccountsDetailsList(string accountNo, out string err)
        {
            Function = "BCustomerManager::GetCustomerAccountsDetails()";

            ArrayList customerDetails = new ArrayList();
            err = String.Empty;
            try
            {
                BCustomer custAccsAndDetails = new BCustomer();
                customerDetails = custAccsAndDetails.GetCustomerAccountsDetails(accountNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return customerDetails;
        }

        [WebMethod(Description = "This method returns the spa account details for those for which a special arrangement has not yet expired.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetSPADetails(string acctNo, out string err)       //CR1084 jec
        {
            Function = "BCustomerManager::GetSPADetails()";
            DataSet ds = null;
            err = "";
            try
            {
                BCustomer custAccsAndDetails = new BCustomer();
                ds = custAccsAndDetails.GetSPADetails(acctNo);      //CR1084 jec
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }


        [WebMethod(Description = "this method returns the accounts for a particular customer")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet CustomerSearch(string customerID,
            string firstName,
            string lastName,
            string address,     //CR1084
            string phoneNumber,     //CR1084
            int limit,
            int settled,
            bool exactMatch,
            string storeType,
            string source, out string err )
        {
            Function = "BCustomerManager::GetCodesForCustomer()";
            DataSet ds = null;
            err = "";
            try
            {
                BCustomer accts = new BCustomer();
                ds = accts.CustomerSearch(customerID, firstName, lastName, address, phoneNumber, limit, settled, exactMatch, storeType, source);      //CR1084
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "this method returns all addresses for a particular customerID")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetCustomerAddresses(string customerID, out string err)
        {
            Function = "WCustomerManager::GetCustomerAddresses()";
            DataSet ds = null;
            err = "";
            try
            {
                BCustomer addr = new BCustomer();
                ds = addr.GetCustomerAddresses(null, null, customerID);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "this method returns all addresses for a particular customerID")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetBasicCustomerDetails(string customerID, string accountNo, string relationship, out string err)
        {
            Function = "WCustomerManager::GetBasicCustomerDetails()";
            DataSet ds = null;
            err = "";
            try
            {
                BCustomer cust = new BCustomer();
                ds = cust.GetBasicCustomerDetails(null, null, customerID, accountNo, relationship);

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }


        [WebMethod(Description = "This method will save the basic customer details")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveBasicDetails(string custid,
                                    string title,
                                    string firstName,
                                    string lastName,
                                    string alias,
                                    string accountNo,
                                    string relationship,
                                    DateTime dob,
                                    string accountType,
                                    string maidenName,
                                    string loyaltyCardNo,
                                    string countryCode,
                                    string storeType,
                                    DataSet otherTabs,
                                    string maritalStat,
                                    int dependants,
                                    string nationality,
                                    bool resieveSms,
                                    out string err)
        {

            SqlConnection conn = null;
            Function = "BCustomerManager::SaveBasicDetails()";
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
                            BCustomer cust = new BCustomer();
                            cust.SaveBasicDetails(conn, trans, custid, title, firstName, lastName, alias, accountNo,
                                                    relationship, STL.Common.Static.Credential.UserId.ToString(), dob, accountType, maidenName,
                                                    loyaltyCardNo, countryCode, otherTabs, maritalStat, dependants, nationality, storeType, resieveSms);

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

                using (var context = Blue.Cosacs.Context.Create())
                {
                    var phones = context.CustTel
                                          .Where(p => p.custid == custid && p.datediscon == null)
                                          .Select(p => p.telno)
                                          .ToArray();

                    if (phones.Any() || !resieveSms)
                    {
                        Chub.SubmitSmsUnsubscriptions(new Blue.Cosacs.Messages.CustomerPhoneNumbers.CustomerPhoneNumbers
                        {
                            CustomerId = custid,
                            PhoneNumbers = phones,
                            Unsubscribe = !resieveSms
                        });
                    }
                }

            }

            return 0;
        }

        [WebMethod(Description = "This method will save the customers employment details")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveCustomerEmploymentDetails(string custid, DataSet dsEmployment, out string err)
        {

            SqlConnection conn = null;
            Function = "BCustomerManager::SaveBasicDetails()";
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
                            BCustomer cust = new BCustomer();
                            cust.SaveEmploymentDetails(conn, trans, custid, dsEmployment);

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



        [WebMethod(Description = "This method will save additional customer details residential fields")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveCustomerAdditionalDetailsResidential(DataSet ds, out string err)
        {

            SqlConnection conn = null;
            Function = "BCustomerManager::SaveBasicDetails()";
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
                            BCustomer cust = new BCustomer();
                            cust.SaveCustomerAdditionalDetailsResidential(conn, trans, ds);

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

        [WebMethod(Description = "This method will save additional customer details financial fields and bank fields")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveCustomerAdditionalDetailsFinancial(string custId, string accountNo, DataSet ds, out string err)
        {

            SqlConnection conn = null;
            Function = "BCustomerManager::SaveCustomerAdditionalDetailsFinancial()";
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
                            BCustomer cust = new BCustomer();
                            cust.SaveCustomerAdditionalDetailsFinancial(conn, trans, custId, accountNo, ds);

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

        [WebMethod(Description = "This method will save the customer addresses")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveAddresses(string linked,
                                DataSet ds,
                                out string err)
        {

            SqlConnection conn = null;
            Function = "BCustomerManager::SaveAddresses()";
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
                            BCustomer cust = new BCustomer();
                            cust.SaveCustomerAddresses(conn, trans, linked, ds, STL.Common.Static.Credential.UserId.ToString());

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

        [WebMethod(Description = "This method returns combined RF data for a specific customer")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetRFCombinedDetails(string customerID, out string err)
        {
            Function = "WCustomerManager::GetRFCombinedDetails()";
            DataSet ds = null;
            err = "";
            string start = DateTime.Now.ToLongTimeString();
            try
            {
                BCustomer cust = new BCustomer();
                ds = cust.GetRFCombinedDetails(customerID);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            //logTime(start, DateTime.Now.ToLongTimeString());
            return ds;
        }

        [WebMethod(Description = "Return combined RF Transactions for a customer")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetRFCombinedTransactions(bool groupedSum, string customerID, out string err)
        {
            Function = "WCustomerManager::GetRFCombinedTransactions()";
            DataSet ds = null;
            err = "";
            string start = DateTime.Now.ToLongTimeString();
            try
            {
                BCustomer cust = new BCustomer();
                ds = cust.GetRFCombinedTransactions(groupedSum, customerID);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            //logTime(start, DateTime.Now.ToLongTimeString());
            return ds;
        }





        [WebMethod(Description = "This method returns a customer's morerewards no.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string GetMoreRewardsNo(string customerID, out string err)
        {
            Function = "WCustomerManager::GetMoreRewardsNo()";
            string m = "";
            err = "";
            try
            {
                BCustomer cust = new BCustomer();
                m = cust.GetMoreRewardsNo(customerID);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return m;
        }

        [WebMethod(Description = "Returns Customer Cash Loan Qualification")]
        [SoapHeader("authentication")]
#if(TRACE)
            [TraceExtension]
#endif
        public bool GetCashLoanQualified(string custID, string acctNo, out string err)
        {
            Function = "WCustomerManager::GetCashLoanQualified()";
            err = String.Empty;
            bool qualified = false;
            try
            {
                BCustomer cust = new BCustomer();
                qualified = cust.GetCashLoanQualified(custID, acctNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return qualified;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public StringCollection GetRequiredAddressTypes(string customerID, out string err)
        {
            Function = "WCustomerManager::GetRequiredAddressTypes()";
            StringCollection addressTypes = null;
            err = "";
            try
            {
                BCustomer cust = new BCustomer();
                addressTypes = cust.GetRequiredAddressTypes(customerID);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return addressTypes;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public StringCollection GetDistinctAddressTypes(string customerID, out string err)
        {
            Function = "WCustomerManager::GetDistinctAddressTypes()";
            StringCollection addressTypes = null;
            err = "";
            try
            {
                BCustomer cust = new BCustomer();
                addressTypes = cust.GetDistinctAddressTypes(customerID);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return addressTypes;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public Byte[] GetImage(string custid, out string err)
        {
            Function = "WCustomerManager::GetImage()";

            err = "";

            byte[] image = null;


            try
            {
                BCustomerImage bo = new BCustomerImage();
                image = bo.Get(custid);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return image;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int UpdateImage(string custid, byte[] image, out string err)
        {
            Function = "WCustomerManager::UpdateImage()";

            err = "";

            try
            {
                BCustomerImage bo = new BCustomerImage();
                bo.Update(custid, image);
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
        public int UnblockCredit(string custid, out string err)
        {

            SqlConnection conn = null;
            Function = "BCustomerManager::UnblockCredit()";
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
                            BCustomer cust = new BCustomer();
                            cust.UnblockCredit(conn, trans, custid);

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

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int IsPrivilegeMember(string custid, out bool privilegeClub, out string privilegeClubCode, out string privilegeClubDesc, out bool hasDiscount, out string err)
        {
            Function = "WCustomerManager::IsPrivilegeMember()";
            privilegeClub = false;
            privilegeClubCode = "";
            privilegeClubDesc = "";
            hasDiscount = false;
            err = "";

            try
            {
                BCustomer cust = new BCustomer();
                privilegeClub = cust.IsPrivilegeMember(custid, out privilegeClubCode, out privilegeClubDesc, out hasDiscount);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return 0;
        }

        // 5.1 uat 253 rdb 10/12/07 is customer priviledge and if so are current priviledge terms types valid for when the account was opened
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int IsPrivilegeMemberWithValidTermsType(string custid, out bool privilegeClub, out string privilegeClubCode, out string privilegeClubDesc, out bool hasDiscount, out string err,
            string TermsType, DateTime dateAccountOpened, out bool termsTypeValid)
        {
            Function = "WCustomerManager::IsPrivilegeMemberWithValidTermsType()";
            privilegeClub = false;
            privilegeClubCode = "";
            privilegeClubDesc = "";
            hasDiscount = false;
            err = "";
            termsTypeValid = false;

            try
            {
                BCustomer cust = new BCustomer();
                privilegeClub = cust.IsPrivilegeMember(custid, out privilegeClubCode, out privilegeClubDesc, out hasDiscount);
                if (privilegeClub)
                {
                    termsTypeValid = cust.ValidatePrivilegeTermsType(TermsType, privilegeClubCode, dateAccountOpened);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return 0;
        }

        [WebMethod(Description = "This method returns combined RF data for a specific customer")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetRFCombinedDetailsForPrint(string customerID, out string err)
        {
            Function = "WCustomerManager::GetRFCombinedDetails()";
            DataSet ds = null;
            err = "";

            try
            {
                BCustomer cust = new BCustomer();
                ds = cust.GetRFCombinedDetailsForPrint(customerID);
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
        public int UpdateCustomerID(string newCustID, string oldCustID, out string err)
        {

            SqlConnection conn = null;
            Function = "BCustomerManager::UpdateCustomerID()";
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
                            BCustomer cust = new BCustomer();
                            cust.User = STL.Common.Static.Credential.UserId;
                            cust.UpdateCustomerID(conn, trans, newCustID, oldCustID);

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

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void SetCreditLimit(string custID, decimal creditLimit, out string err)
        {

            SqlConnection conn = null;
            Function = "BCustomerManager::SetCreditLimit()";
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
                            BCustomer cust = new BCustomer();
                            cust.SetCreditLimit(conn, trans, custID, creditLimit);

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

        [WebMethod(Description = "Return an audit list of Customer Id and Name changes")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetCustomerAudit(string customerId, out string err)
        {
            Function = "WCustomerManager::GetCustomerAudit()";
            DataSet ds = null;
            err = "";

            try
            {
                BCustomer cust = new BCustomer();
                ds = cust.GetCustomerAudit(customerId);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "Return a list of Customer Address History")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetAddressHistory(string customerId, out string err)
        {
            Function = "WCustomerManager::GetAddressHistory()";
            DataSet ds = null;
            err = "";

            try
            {
                BCustomer cust = new BCustomer();
                ds = cust.GetAddressHistory(customerId);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "Return an audit list of Customer Address History")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetAddressAuditHistory(string customerId, out string err)
        {
            Function = "WCustomerManager::GetAddressHistory()";
            DataSet ds = null;
            err = "";

            try
            {
                BCustomer cust = new BCustomer();
                ds = cust.GetAddressAuditHistory(customerId);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }



        [WebMethod(Description = "Saves Customer Mailing Query")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet CustomerMailingQuerySave(int EmpeenoSave, DateTime datesaved, string QueryName, string CustomerCodeSet, string NoCustomerCodeSet, string AccountCodeSet, string NoAccountCodeSet, string ArrearsRestriction, double Arrears, string maxcurrstatus, string maxeverstatus, string branchset, string accttypes, string itemset, DateTime itemsetstartdate, DateTime itemsetenddate, string noitemset, DateTime noitemsetstartdate, DateTime noitemsetenddate, string itemcatset, DateTime itemCatsetstartdate, DateTime itemCatsetenddate, string noitemCatset, DateTime noitemCatsetstartdate, DateTime noitemCatsetenddate, int itemsdelivered, string itemstartswithset, DateTime itemstartswithstartdate, DateTime itemstartswithenddate, string noitemstartswithset, DateTime noitemstartswithstartdate, DateTime noitemstartswithenddate, string noletterset, DateTime nolettersetStartdate, DateTime nolettersetEnddate, string letterset, DateTime lettersetstartdate, DateTime lettersetenddate, int customerstartage, int customerEndage, string totals, string resulttype, int excludecancellations, out string err)
        {
            Function = "WCustomerManager::CustomerMailingQuerySave()";
            DataSet ds = new DataSet();
            err = "";


            try
            {
                BCustomerMailing bo = new BCustomerMailing();
                ds = bo.BCustomerMailingQuerySave(EmpeenoSave, datesaved, QueryName, CustomerCodeSet, NoCustomerCodeSet, AccountCodeSet, NoAccountCodeSet, ArrearsRestriction, Arrears, maxcurrstatus, maxeverstatus, branchset, accttypes, itemset, itemsetstartdate, itemsetenddate, noitemset, noitemsetstartdate, noitemsetenddate, itemcatset, itemCatsetstartdate, itemCatsetenddate, noitemCatset, noitemCatsetstartdate, noitemCatsetenddate, itemsdelivered, itemstartswithset, itemstartswithstartdate, itemstartswithenddate, noitemstartswithset, noitemstartswithstartdate, noitemstartswithenddate, noletterset, nolettersetStartdate, nolettersetEnddate, letterset, lettersetstartdate, lettersetenddate, customerstartage, customerEndage, totals, resulttype, excludecancellations);


            }
            catch (Exception ex)
            {
                err = Function + ": " + ex.Message;
                logException(ex, Function);
            }
            return ds;
        }

        [WebMethod(Description = "Runs the Customer Mailing Query ")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet CustomerMailingQuery(string CustomerCodeSet, string NoCustomerCodeSet, string AccountCodeSet, string NoAccountCodeSet, string ArrearsRestriction, double Arrears, string maxcurrstatus, string maxeverstatus, string branchset, string accttypes, string itemset, DateTime itemsetstartdate, DateTime itemsetenddate, string noitemset, DateTime noitemsetstartdate, DateTime noitemsetenddate, string itemcatset, DateTime itemCatsetstartdate, DateTime itemCatsetenddate, string noitemCatset, DateTime noitemCatsetstartdate, DateTime noitemCatsetenddate, int itemsdelivered, string itemstartswithset, DateTime itemstartswithstartdate, DateTime itemstartswithenddate, string noitemstartswithset, DateTime noitemstartswithstartdate, DateTime noitemstartswithenddate, string noletterset, DateTime nolettersetStartdate, DateTime nolettersetEnddate, string letterset, DateTime lettersetstartdate, DateTime lettersetenddate, int customerstartage, int customerEndage, string totals, string resulttype, int excludecancellations, int liveDatabase, out string err)
        {
            Function = "WCustomerManager::CustomerMailingQuery()";
            DataSet ds = new DataSet();
            err = "";
            SqlConnection conn = null;


            try
            {

                // This web service can connect to one of two possible database connections
                if (liveDatabase == 1)
                    conn = new SqlConnection(Connections.Default);
                else
                    conn = new SqlConnection(Connections.Report);

                do
                {
                    using (conn)
                    {
                        try
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                            {
                                BCustomerMailing bo = new BCustomerMailing();
                                ds = bo.CustomerMailingQuery(
                                    CustomerCodeSet,
                                    NoCustomerCodeSet,
                                    AccountCodeSet,
                                    NoAccountCodeSet,
                                    ArrearsRestriction,
                                    Arrears,
                                    maxcurrstatus,
                                    maxeverstatus,
                                    branchset,
                                    accttypes,
                                    itemset,
                                    itemsetstartdate,
                                    itemsetenddate,
                                    noitemset,
                                    noitemsetstartdate,
                                    noitemsetenddate,
                                    itemcatset,
                                    itemCatsetstartdate,
                                    itemCatsetenddate,
                                    noitemCatset,
                                    noitemCatsetstartdate,
                                    noitemCatsetenddate,
                                    itemsdelivered,
                                    itemstartswithset,
                                    itemstartswithstartdate,
                                    itemstartswithenddate,
                                    noitemstartswithset,
                                    noitemstartswithstartdate,
                                    noitemstartswithenddate,
                                    noletterset,
                                    nolettersetStartdate,
                                    nolettersetEnddate,
                                    letterset,
                                    lettersetstartdate,
                                    lettersetenddate,
                                    customerstartage,
                                    customerEndage,
                                    totals,
                                    resulttype,
                                    excludecancellations);

                                trans.Commit();
                            }
                            break;
                        }
                        catch (SqlException ex)
                        {
                            CatchDeadlock(ex, conn);
                        }
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                err = Function + ": " + ex.Message;
                logException(ex, Function);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return ds;
        }

        [WebMethod(Description = "Loads A particular Mailing Query for this employee ")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int CustomerMailingQueryGet(int EmpeenoSave, out DateTime datesaved, string QueryName, out string CustomerCodeSet, out string NoCustomerCodeSet, out string AccountCodeSet, out string NoAccountCodeSet, out string ArrearsRestriction, out double Arrears, out string maxcurrstatus, out string maxeverstatus, out string branchset, out string accttypes, out string itemset, out DateTime itemsetstartdate, out DateTime itemsetenddate, out string noitemset, out DateTime noitemsetstartdate, out DateTime noitemsetenddate, out string itemcatset, out DateTime itemCatsetstartdate, out DateTime itemCatsetenddate, out string noitemCatset, out DateTime noitemCatsetstartdate, out DateTime noitemCatsetenddate, out int itemsdelivered, out string itemstartswithset, out DateTime itemstartswithstartdate, out DateTime itemstartswithenddate, out string noitemstartswithset, out DateTime noitemstartswithstartdate, out DateTime noitemstartswithenddate, out string noletterset, out DateTime nolettersetStartdate, out DateTime nolettersetEnddate, out string letterset, out DateTime lettersetstartdate, out DateTime lettersetenddate, out int customerstartage, out int customerEndage, out string totals, out string resulttype, out int excludecancellations)
        {
            int status = 0;
            BCustomerMailing BC = new BCustomerMailing();
            status = BC.QueryGet(EmpeenoSave, out datesaved, QueryName, out CustomerCodeSet, out NoCustomerCodeSet, out AccountCodeSet, out NoAccountCodeSet, out ArrearsRestriction, out Arrears, out maxcurrstatus, out maxeverstatus, out branchset, out accttypes, out itemset, out itemsetstartdate, out itemsetenddate, out noitemset, out noitemsetstartdate, out noitemsetenddate, out itemcatset, out itemCatsetstartdate, out itemCatsetenddate, out noitemCatset, out noitemCatsetstartdate, out noitemCatsetenddate, out itemsdelivered, out itemstartswithset, out itemstartswithstartdate, out itemstartswithenddate, out noitemstartswithset, out noitemstartswithstartdate, out noitemstartswithenddate, out noletterset, out nolettersetStartdate, out nolettersetEnddate, out letterset, out lettersetstartdate, out lettersetenddate, out customerstartage, out customerEndage, out totals, out resulttype, out excludecancellations);

            return status;
        }

        [WebMethod(Description = "Loads all saved Customer Mailing Query for this employee ")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet CustomerMailingQueryLoadbyEmpeeno(int EmpeenoSave)
        {
            DataSet ds = new DataSet();
            BCustomerMailing Ba = new BCustomerMailing();
            ds = Ba.QueryLoadbyEmpeeno(EmpeenoSave);
            return ds;
        }

        [WebMethod(Description = "This method issues prize vouchers to a customer")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int IssuePrizeVouchers(string acctNo, decimal cashPrice, int buffNo,
                                          out DateTime dateIssued, out string err)
        {
            Function = "BCustomerManager::IssuePrizeVouchers()";

            SqlConnection conn = null;
            //XmlNode vouchers = null;
            err = "";
            dateIssued = DateTime.Now;
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
                            BCustomer cust = new BCustomer();
                            cust.User = STL.Common.Static.Credential.UserId;
                            cust.IssuePrizeVouchers(conn, trans, acctNo, cashPrice, buffNo, out dateIssued);

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

        [WebMethod(Description = "Return a list of allocated prize vouchers")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetPrizeVoucherDetails(string acctNo, string custID, string branchFilter,
            DateTime dateFrom, DateTime dateTo, int buffNo, out string err)
        {
            Function = "WCustomerManager::GetPrizeVoucherDetails()";
            DataSet ds = null;
            err = "";

            try
            {
                BCustomer cust = new BCustomer();
                ds = cust.GetPrizeVoucherDetails(acctNo, custID, branchFilter, dateFrom, dateTo, buffNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [Conditional("DEBUG")]
        private void logTime(string started, string ended)
        {
            logPerformanceMessage(Function + "Started at : " + started + " and Ended at : " + ended, STL.Common.Static.Credential.UserId.ToString(), Environment.StackTrace, EventLogEntryType.Information);
        }


        [WebMethod(Description = "Delete prize vouchers issued prior to the specified end date")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int DeletePrizeVouchers(DateTime enddate, string acctNo, bool isCancellation, out string err)
        {
            Function = "BCustomerManager::DeletePrizeVouchers()";

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
                            BCustomer cust = new BCustomer();
                            cust.User = STL.Common.Static.Credential.UserId;
                            cust.DeletePrizeVouchers(conn, trans, enddate, acctNo, isCancellation);

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

        [WebMethod(Description = "Saves a customer photo to the database.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool SaveCustomerPhoto(string custID, string fileName, int takenBy, out string err)
        {
            Function = "BCustomerManager::SaveCustomerPhoto()";

            SqlConnection conn = null;
            err = "";
            bool fileExists = true;
            try
            {
                conn = new SqlConnection(Connections.Default);
                BCustomer cust = new BCustomer();
                conn.Open();
                do
                {
                    try
                    {
                        //conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            //BCustomer cust = new BCustomer();
                            //cust.User = STL.Common.Static.Credential.UserId;
                            fileExists = cust.SaveCustomerPhoto(conn, trans, custID, fileName, takenBy);

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
            return fileExists;
        }

        [WebMethod(Description = "Saves a customer signature to the database.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool SaveCustomerSignature(string custID, string fileName, out string err)
        {
            Function = "BCustomerManager::SaveCustomerSignature()";

            SqlConnection conn = null;
            err = "";
            bool fileExists = true;
            try
            {
                conn = new SqlConnection(Connections.Default);
                BCustomer cust = new BCustomer();
                conn.Open();
                do
                {
                    try
                    {
                        //conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            //BCustomer cust = new BCustomer();
                            //cust.User = STL.Common.Static.Credential.UserId;
                            fileExists = cust.SaveCustomerSignature(conn, trans, custID, fileName);

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
            return fileExists;
        }

        [WebMethod(Description = "Gets the most recent customer photo.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string GetCustomerPhoto(string custID, out string err)
        {
            Function = "BCustomerManager::GetCustomerPhoto()";

            SqlConnection conn = null;
            err = "";
            string fileName = String.Empty;
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
                            BCustomer cust = new BCustomer();
                            //cust.User = STL.Common.Static.Credential.UserId;
                            fileName = cust.GetCustomerPhoto(conn, trans, custID);

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
            return fileName;
        }

        [WebMethod(Description = "Return all photos for the selected customer")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetAllCustomerPhotos(string custID, out string err)
        {
            Function = "WCustomerManager::GetAllCustomerPhotos()";

            SqlConnection conn = null;
            DataSet ds = null;
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
                            BCustomer cust = new BCustomer();
                            ds = cust.GetAllCustomerPhotos(conn, trans, custID);

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
            return ds;
        }

        [WebMethod(Description = "Gets the most recent customer signature.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string GetCustomerSignature(string custID, out string err)
        {
            Function = "BCustomerManager::GetCustomerSignature()";

            SqlConnection conn = null;
            err = "";
            string fileName = String.Empty;
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
                            BCustomer cust = new BCustomer();
                            //cust.User = STL.Common.Static.Credential.UserId;
                            fileName = cust.GetCustomerSignature(conn, trans, custID);

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
            return fileName;
        }

        [WebMethod(Description = "Return a list of warrantable items without warranties for second effort solicitation")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetWarrantySecondEffortSolicitationItems(string customerId, int numberOfPrompts, int warrantyAfterDeliveryDays, out string err)
        {
            Function = "WCustomerManager::GetWarrantySecondEffortSolicitationItems()";
            DataSet ds = null;
            err = "";
            try
            {
                BCustomer cust = new BCustomer();
                ds = cust.GetWarrantySecondEffortSolicitationItems(customerId, numberOfPrompts, warrantyAfterDeliveryDays);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }


        [WebMethod(Description = "Return a list of warrantable items without warranties for second effort solicitation")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveWarrantySESConfirmation(DataSet ds, int empNo, out string err)
        {

            SqlConnection conn = null;
            Function = "WCustomerManager::SaveWarrantySESConfirmation()";
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
                            BCustomer cust = new BCustomer();
                            cust.SaveWarrantySESConfirmation(conn, trans, ds, empNo);

                            //#16992 - Send Warranty Potential message
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                new WarrantyRepository().SendWarrantyPotential(conn, trans, row["acctno"].ToString(), Convert.ToInt32(row["agrmtno"]), Convert.ToInt32(row["itemID"]),
                                                                                    Convert.ToInt16(row["stocklocn"]));
                            }

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

        //IP - 18/11/08 -  UAT5.1 - UAT(580) - Method that will generate a new Customer ID for a
        //Cash & Go Service Request and populate the Customer ID field on the Service Request screen.
        [WebMethod(Description = "Generates a new Customer ID for Cash & Go Service requests")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string GenerateSRCashAndGoCustid(int branchNo, out string err)
        {
            Function = "BCustomerManager::GenerateSRCashAndGoCustid()";

            SqlConnection conn = null;
            err = "";
            string SRCashAndGoCustid = String.Empty;
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
                            BCustomer cust = new BCustomer();

                            SRCashAndGoCustid = cust.GenerateSRCashAndGoCustid(conn, trans, branchNo);

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
            return SRCashAndGoCustid;
        }


        [WebMethod(Description = "Returns loyatly info for custid")]
        [SoapHeader("authentication")]
        public Loyalty LoyaltyGetDatabycustid(string custid)
        {
            Loyalty loyalty = new Loyalty();
            Function = "WCustomerManager::LoyaltyGetDatabycustid()";
            string err = "";
            try
            {
                BLoyalty custloyalty = new BLoyalty();
                loyalty = custloyalty.LoyaltyGetByCustid(custid);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return loyalty;
        }

        [WebMethod(Description = "Returns loyatly info for custid")]
        [SoapHeader("authentication")]
        public Loyalty LoyaltyGetDatabyacctno(string acctno)
        {
            Loyalty loyalty = new Loyalty();
            Function = "WCustomerManager::LoyaltyGetDatabyacctno()";
            string err = "";
            try
            {
                BLoyalty custloyalty = new BLoyalty();
                loyalty = custloyalty.LoyaltyGetDatabyacctno(acctno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return loyalty;
        }

        [WebMethod(Description = "Returns loyatly info for custid")]
        [SoapHeader("authentication")]
        public Loyalty LoyaltyGetDatabyMemberno(string membershipno)
        {
            Loyalty loyalty = new Loyalty();
            Function = "WCustomerManager::LoyaltyGetDatabycustid()";
            string err = "";
            try
            {
                BLoyalty custloyalty = new BLoyalty();
                loyalty = custloyalty.LoyaltyGetByMemberno(membershipno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return loyalty;
        }

        [WebMethod(Description = "Add Loyalty Membership Fee")]
        [SoapHeader("authentication")]
        public void LoyaltyAddFee(string acctno, string membertype, string custid, int user)
        {

            Function = "WCustomerManager::LoyaltyAddFee()";
            string err = "";
            try
            {
                BLoyalty bloyal = new BLoyalty();
                bloyal.LoyaltyAddFee(acctno, membertype, custid, user);

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }


        }

        [WebMethod(Description = "LoyaltySave")]
        [SoapHeader("authentication")]
        public string LoyaltySave(Loyalty loyalty)
        {
            string output = "";

            Function = "WCustomerManager::LoyaltyAddFee()";
            string err = "";
            try
            {
                BLoyalty bloyal = new BLoyalty();
                output = bloyal.LoyaltySave(loyalty);

            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return output;
        }

        [WebMethod(Description = "LoyaltyCheckCustomer")]
        [SoapHeader("authentication")]
        public bool LoyaltyCheckCustomer(string custid)
        {
            bool check = false;
            Function = "WCustomerManager::LoyaltyCheckCustomer()";
            string err = "";
            try
            {
                BLoyalty bloyal = new BLoyalty();
                check = bloyal.LoyaltyCheckCustomer(custid);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return check;
        }


        [WebMethod(Description = "LoyaltyGetCashAccount")]
        [SoapHeader("authentication")]
        public string LoyaltyGetCashAccount(string custid)
        {
            string account = "";
            Function = "WCustomerManager::LoyaltyGetCashAccount()";
            string err = "";
            try
            {
                BLoyalty bloyal = new BLoyalty();
                account = bloyal.LoyaltyGetCashAccount(custid);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return account;
        }

        [WebMethod(Description = "LoyaltyGetVouchers")]
        [SoapHeader("authentication")]
        public List<LoyaltyVoucher> LoyaltyGetVouchers(string custid)
        {
            List<LoyaltyVoucher> vouchers = new List<LoyaltyVoucher>();
            Function = "WCustomerManager::LoyaltyGetVouchers()";
            string err = "";
            try
            {
                BLoyalty bloyal = new BLoyalty();
                vouchers = bloyal.LoyaltyGetVouchers(custid);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return vouchers;
        }

        [WebMethod(Description = "LoyaltyAddRejection")]
        [SoapHeader("authentication")]
        public void LoyaltyAddRejection(string custid, string acctno)
        {

            Function = "WCustomerManager::LoyaltyAddRejection()";
            string err = "";
            try
            {
                BLoyalty bloyal = new BLoyalty();
                bloyal.LoyaltyAddRejection(custid, acctno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }


        }

        [WebMethod(Description = "LoyaltyGetCharges")]
        [SoapHeader("authentication")]
        public void LoyaltyGetCharges(string custid, ref string acctno, ref decimal amount, ref bool active)
        {
            acctno = "";
            amount = 0;
            Function = "WCustomerManager::LoyaltyGetCharges()";
            string err = "";
            try
            {
                BLoyalty bloyal = new BLoyalty();
                bloyal.LoyaltyGetCharges(custid, ref acctno, ref amount, ref active);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

        }

        [WebMethod(Description = "LoyaltySaveVouchers")]
        [SoapHeader("authentication")]
        public void LoyaltySaveVouchers(int voucher, bool add, string acctno)
        {

            Function = "WCustomerManager::LoyaltySaveVouchers()";
            string err = "";
            try
            {
                BLoyalty bloyal = new BLoyalty();
                bloyal.LoyaltySaveVouchers(voucher, add, acctno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

        }



        [WebMethod(Description = "LoyaltyLinkAccount")]
        [SoapHeader("authentication")]
        public bool LoyaltyIsLinkAccount(string acctno, string custid)
        {
            bool linked = false;

            Function = "WCustomerManager::LoyaltyLinkAccount()";
            string err = "";
            try
            {
                BLoyalty bloyal = new BLoyalty();
                linked = bloyal.LoyaltyIsLinkAccount(acctno, custid);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return linked;
        }

        [WebMethod(Description = "LoyaltyPay")]
        [SoapHeader("authentication")]
        public void LoyaltyPay(string acctno)
        {

            Function = "WCustomerManager::LoyaltyLinkAccount()";
            string err = "";
            try
            {
                BLoyalty bloyal = new BLoyalty();
                bloyal.LoyaltyPay(acctno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
        }

        [WebMethod(Description = "LoyaltyGetValidAddress")]
        [SoapHeader("authentication")]
        public List<LoyaltyValidAddresses> LoyaltyGetValidAddress(string custid)
        {
            List<LoyaltyValidAddresses> accttype = new List<LoyaltyValidAddresses>();

            Function = "WCustomerManager::LoyaltyGetValidAddress()";
            string err = "";
            try
            {
                BLoyalty bloyal = new BLoyalty();
                accttype = bloyal.LoyaltyGetValidAddress(custid);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return accttype;

        }

        [WebMethod(Description = "LoyaltyCancelAccount")]
        [SoapHeader("authentication")]
        public void LoyaltyCancelAccount(string acctno, int user)
        {

            Function = "WCustomerManager::LoyaltyCancelAccount()";
            string err = "";
            try
            {
                BLoyalty bloyal = new BLoyalty();
                bloyal.LoyaltyCancelAccount(acctno, user);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
        }

        [WebMethod(Description = "LoyaltyCheckAccountPeriod")]
        [SoapHeader("authentication")]
        public bool LoyaltyCheckAccountPeriod(string acctno)
        {

            Function = "WCustomerManager::LoyaltyCheckAccountPeriod";
            string err = "";

            bool valid = false;

            try
            {
                BLoyalty bloyal = new BLoyalty();
                valid = bloyal.LoyaltyCheckAccountPeriod(acctno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return valid;
        }
        [WebMethod(Description = "LoyaltyCheckRemoveFreeDel")]
        [SoapHeader("authentication")]
        public void LoyaltyCheckRemoveFreeDel(string acctno, int user)
        {

            Function = "WCustomerManager::LoyaltyCheckAccountPeriod";
            string err = "";

            try
            {
                BLoyalty bloyal = new BLoyalty();
                bloyal.LoyaltyCheckRemoveFreeDel(acctno, user);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
        }

        [WebMethod(Description = "LoyaltyGRTCancelFreeDelAdjust")]
        [SoapHeader("authentication")]
        public void LoyaltyGRTCancelFreeDelAdjust(string acctno, int user)
        {

            Function = "WCustomerManager::LoyaltyCheckAccountPeriod";
            string err = "";

            try
            {
                BLoyalty bloyal = new BLoyalty();
                bloyal.LoyaltyGRTCancelFreeDelAdjust(acctno, user);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
        }

        [WebMethod(Description = "LoyaltyGetHistory")]
        [SoapHeader("authentication")]
        public DataSet LoyaltyGetHistory(string custid)
        {

            Function = "WCustomerManager::LoyaltyGetHistory";
            string err = "";

            DataSet LoyaltyHistory = new DataSet();

            try
            {
                BLoyalty bloyal = new BLoyalty();
                LoyaltyHistory.Tables.Add(bloyal.LoyaltyGetHistory(custid));
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return LoyaltyHistory;
        }

        [WebMethod(Description = "LoyaltyCheckLinkedAccount")]
        [SoapHeader("authentication")]
        public bool LoyaltyCheckLinkedAccount(string acct)
        {

            Function = "WCustomerManager::LoyaltyCheckLinkedAccount";
            string err = "";

            bool IsAccount = false;
            try
            {
                BLoyalty bloyal = new BLoyalty();
                IsAccount = bloyal.LoyaltyCheckLinkedAccount(acct);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return IsAccount;
        }

        [WebMethod(Description = "LoyaltyGetVoucherValue")]
        [SoapHeader("authentication")]
        public decimal? LoyaltyGetVoucherValue(string acct)
        {

            Function = "WCustomerManager::LoyaltyGetVoucherValue";
            string err = "";

            try
            {
                BLoyalty bloyal = new BLoyalty();
                return bloyal.LoyaltyGetVoucherValue(acct);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
                return 0;
            }

        }

        [WebMethod(Description = "LoyaltyRemoveVoucher")]
        [SoapHeader("authentication")]
        public void LoyaltyRemoveVoucher(string acct)
        {

            Function = "WCustomerManager::LoyaltyRemoveVoucher";
            string err = "";

            try
            {
                BLoyalty bloyal = new BLoyalty();
                bloyal.LoyaltyRemoveVoucher(acct);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

        }


        [WebMethod(Description = "Return a customers current band")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string CustomerGetBand(string acctno, out string err)
        {
            Function = "WCustomerManager::CustomerGetBand()";

            string band = "";

            err = "";

            try
            {
                BCustomer cust = new BCustomer();
                band = cust.CustomerGetBand(acctno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return band;
        }

        [WebMethod(Description = "Save a customers current band")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void CustomerSaveBand(string custid, char band)
        {
            Function = "WCustomerManager::CustomerSaveBand()";

            try
            {
                BCustomer cust = new BCustomer();
                cust.CustomerSaveBand(custid, band);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //IP - 17/01/11 - Store Card
        [WebMethod(Description = "Retrieve a customer object")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public Customer LoadCustomer(string custid)
        {
            return new CustomerRepository().Load(custid);
        }


//        [WebMethod(Description = "Retrieve a customer MMI value")]
//        [SoapHeader("authentication")]
//#if(TRACE)
//		[TraceExtension]
//#endif
//        public decimal GetCustomerMmiLimit(string CustId)
//        {
//            Function = "WCustomerManager::GetCustomerMmiLimit";
//            string err = string.Empty;
//            decimal dCustMMI = 0;
//            try
//            {
//                BCustomer bCust = new BCustomer();
//                dCustMMI = bCust.GetCustomerMmiLimit(CustId);
//            }
//            catch (Exception ex)
//            {
//                Catch(ex, Function, ref err);
//            }
//            return dCustMMI;
//        }

        [WebMethod(Description = "Retrieve a customer MMI value")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public decimal GetCustomerMmiLimit(string CustId, out bool isMmiCalculated)
        {
            BCustomer bCust = new BCustomer();
            return bCust.GetCustomerMmiLimit(CustId, out isMmiCalculated);
        }

        [WebMethod(Description = "this method returns duplicate Customers")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetDuplicateCustomers()
        {
            DataSet ds = null;
            string err = "";

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
                            ds = new CustomerRepository().GetDuplicateCustomers(conn, trans);

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

            return ds;
        }


        //#19422 - CR17976
        [WebMethod(Description = "this method Resolves / Unresolves duplicate customers")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void UpdateDuplicateCustomers(string custid, string duplicateCustid, bool resolved)
        {
            string err = "";

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

                            new CustomerRepository().UpdateDuplicateCustomers(conn, trans, custid, duplicateCustid, resolved);

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


        //CR 2018-13 - 21/12/18
        [WebMethod(Description = "This method generates and returns the invoice number for a Sale done in POS.")]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExtension]
#endif
        public string GenerateInvoiceNumber(string branch_no)
        {
            Function = "BCustomerManager::GenerateInvoiceNumber()";

            string agr_inv_no = string.Empty;
            string err = "";
            try
            {
                BCustomer cust = new BCustomer();
                agr_inv_no = cust.GenerateAgreementInvNo(branch_no);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return agr_inv_no;
        }

        //Added by Suvidha
        [WebMethod(Description = "This method returns customer details for a particular agreement generated from Web")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetBasicCustomerDetails_Web(string agrmtno, out string err)
        {
            Function = "WCustomerManager::GetBasicCustomerDetails_Web()";
            DataSet ds = null;
            err = "";
            try
            {
                BCustomer cust = new BCustomer();
                ds = cust.GetBasicCustomerDetailsForReprint(null, null, agrmtno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }


    }
}
