using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using STL.Common;
using STL.Common.Constants.ScreenModes;
using STL.BLL;
using System.Data.SqlClient;
using System.Configuration;
using System.Xml;
using STL.DAL;
using Blue.Cosacs;
using Blue.Cosacs.Shared;

namespace STL.WS
{
    /// <summary>
    /// Summary description for WCreditManager.
    /// </summary>
    /// 
    [WebService(Namespace = "http://strategicthought.com/webservices/")]
    public class WCreditManager : CommonService
    {
        public WCreditManager()
        {
            //CODEGEN: This call is required by the ASP.NET Web Services Designer
            InitializeComponent();
        }

        #region Component Designer generated code

        //Required by the Web Services Designer 
        private IContainer components = null;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        [WebMethod(MessageName = "1", Description = "This method will return all propsal details for a particular account or customer - first overload")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetProposal(string customerID, string accountNo, DateTime dateProp, string mode, out string err)
        {
            Function = "WCreditManager::GetProposal()";
            DataSet ds = null;
            err = "";
            try
            {
                /* This version of GetProposal is for use when the user is trying to 
                 * view or edit a specific proposal selected from a list, therefore 
                 * the dateProp is supplied so we can select the exact proposal 
                 * requested.
                 * 
                 * if(mode == ScreenMode.View)
                 * {
                 *		get the data all from the proposal table
                 * }
                 * if(mode == ScreenMode.Edit)
                 * {
                 *		get the data from the live tables and the proposal table
                 *		The accountNo parameter may be blank. If it is then this is a 
                 *		customer proposal (i.e. budget card) this governs which table to look
                 *		in for the second applicant if there is a second applicant. Only
                 *		relevant in edit mode because in read only all info comes from
                 *		proposal table.
                 * }
                 */
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(MessageName = "2", Description = "This method will return all propsal details for a particular account or customer - second overload")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetProposalStage1(string customerID, string accountNo, string mode, string relation, out string err)
        {
            Function = "WCreditManager::GetProposalStage1()";
            DataSet ds = null;
            err = "";
#if(DEBUG)
            logMessage(Function + " - start: " + DateTime.Now.ToLongTimeString(), STL.Common.Static.Credential.UserId.ToString(), EventLogEntryType.Information);
#endif
            try
            {
                /* This version of GetProposal is for use when the user is trying to 
                 * enter a new proposal. Therefore the ScreenMode in this case must be 
                 * ScreenMode.New. 
                 * 
                 * No data will be retrieved from the proposal table in this case because
                 * no proposal record exists. The only thing to check is whether or not the
                 * accountNo passed in is blank. If it is then we are sanctioning a customer
                 * and therefore any second applicant must be identified from the 
                 * customerlinks table rather that the custacct table.
                 */
                if (mode == SM.New)
                {
                    BProposal prop = new BProposal();
                    ds = prop.GetProposalStage1(customerID, accountNo, relation);
                }
                else
                    throw new STLException("Invalid ScreenMode for new proposal.");
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
#if(DEBUG)
            logMessage(Function + " - end: " + DateTime.Now.ToLongTimeString(), STL.Common.Static.Credential.UserId.ToString(), EventLogEntryType.Information);
#endif
            return ds;
        }

        [WebMethod(Description = "This method will return all Stage 2 proposal details for a particular account or customer")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetProposalStage2(string customerID, DateTime dtDateProp, string accountNo, string relation, out string err)
        {
            /* Stage 2 Credit Application
             * Loaded from Proposal, Employment and ProposalRef tables.
            */

            Function = "WCreditManager::GetProposalStage2()";
            DataSet ds = null;
            err = "";
            try
            {
                BProposal p = new BProposal();
                ds = p.GetProposalStage2(customerID, dtDateProp, accountNo, relation);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method retrieves the list of previous references for one Customer")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetReferenceList(string custId, out string err)
        {
            /* All previous references for one Customer
            */

            Function = "WCreditManager::GetReferenceList()";
            DataSet ds = null;
            err = "";
            try
            {
                BProposal p = new BProposal();
                ds = p.GetReferenceList(custId);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method will save a credit proposal")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveProposal(string customerID,
            string accountNo,
            DataSet app1,
            DataSet app2,
            bool sanction,
            out string err)
        {
            
            SqlConnection conn = null;
            Function = "WCreditManager::SaveProposal()";
            err = "";
            try
            {
                //This operation requires a transaction therefore the connection must be created
                //here and passed into each seperate operation
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BProposal prop = new BProposal();
                            prop.User = STL.Common.Static.Credential.UserId;
                            prop.Save(conn, trans, customerID, accountNo, app1, app2, sanction);

                            trans.Commit();		//commit the transaction
                        }
                        break;				//break out of loop
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);	//deadlock loop

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

        [WebMethod(Description = "This method will save a Stage 2 credit proposal")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveProposalStage2(string customerID,
            string accountNo,
            DataSet app1,
            DataSet app2,
            bool complete,
            out string err)
        {
            
            SqlConnection conn = null;
            Function = "WCreditManager::SaveProposalStage2()";
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

                            BProposal prop = new BProposal();
                            prop.User = STL.Common.Static.Credential.UserId;
                            prop.SaveStage2(conn, trans, customerID, accountNo, app1, app2, complete);

                            trans.Commit();		//commit the transaction
                        }
                        break;				//break out of loop
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);	//deadlock loop

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


        [WebMethod(Description = "This method will return all propsal details for a particular account or customer - first overload")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet LoadProposalFlags(string accountNo, string customerID, DateTime dateProp, out bool holdProp, out string currentStatus, out string err)
        {
            Function = "WCreditManager::LoadProposalFlags()";
            DataSet ds = null;
            err = "";
            holdProp = false;
            currentStatus = "";
            try
            {
                BProposal p = new BProposal();
                ds = p.LoadProposalFlags(accountNo, customerID, dateProp, out holdProp, out currentStatus);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method will return the credit limit and available credit for a ready finance customer and account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int GetRFLimit(string customerID, string acctList, string accountType, out decimal limit, out decimal available, out bool wrongType, out string err)
        {
            // The acctList parameter is used to exclude certain accounts from the calculation.
            // Used by the Add-To calculation to exclude the accounts to be settled and the new account.

            Function = "WCreditManager::GetRFLimit()";
            err = "";
            limit = 0;
            available = 0;
            wrongType = false;
            try
            {
                BCustomer cust = new BCustomer();
                cust.GetRFLimit(customerID, acctList, accountType, out limit, out available, out wrongType);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0;
        }
		
		// CR - Revise Cash Loan Disbursement Limits
        [WebMethod(Description = "This method will return the Exist cash loan for the account")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int GetExistCashLoan(string customerID, out decimal limit, out string err)
        {
            Function = "WCreditManager::GetExistCashLoan()";
            err = "";
            limit = 0;
            try
            {
                BCustomer cust = new BCustomer();
                cust.GetExistCashLoan(customerID, out limit);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0;
        }
		
		
        [WebMethod(Description = "This is a hack method to simulate sanctioning for testing purposes")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public decimal DummySanction(string accountNo, string customerID, DateTime timeStamp, out string err)
        {
            Function = "WCreditManager::DummySanction()";
            SqlConnection conn = null;
            err = "";
            
            BProposal prop = null;
            decimal limit = 0;
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
                            limit = prop.DummySanction(conn, trans, accountNo, customerID, timeStamp);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock && retries < maxRetries)
                        {
                            retries++;
                            if (conn.State != ConnectionState.Closed)
                                conn.Close();
                        }
                        else
                            throw ex;
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
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return limit;
        }

        [WebMethod(Description = "This method will return an XML node listing all the scoring rules currently defined for the selected rule type")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public XmlNode GetScoringRules(string country, char scoretype, string region, out string err) //SC CR1034 Behavioural Scoring 15/02/2010
        {
            Function = "WCreditManager::GetScoringRules()";
            err = "";
            XmlNode rules = null;
            try
            {
                BScoring score = new BScoring();
                rules = score.GetRules(country, scoretype,region);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return rules;
        }

		[WebMethod(Description = "This method will Save Values of ApplicantSpendMatrix")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveApplicantSpendMatrix(DataTable dt, out string err)
        {

            BSpend bs = new BSpend();
            SqlConnection conn = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {

                    bs.SaveApplicantSpendFactorMatrix(conn, trans, dt);
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
            return 0;

        }

        [WebMethod(Description = "This method will return Values of ApplicantSpendMatrix")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetApplicantSpendMatrix(out string err)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            BSpend bs = new BSpend();
            SqlConnection conn = null;
             err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {

                    dt = bs.GetApplicantSpendFactorMatrix(conn, trans);

                    ds.Tables.Add(dt);
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


        [WebMethod(Description = "This method will Save ValuesDependentSpendMatrix")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveDependentSpendMatrix(DataTable dt, out string err)
        {

            BSpend bs = new BSpend();
            SqlConnection conn = null;
             err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            bs.SaveDependentSpendFactorMatrix(conn, trans, dt);
                            trans.Commit();
                        }
             }
            catch (Exception ex)
            {
                Catch(ex, Function,ref err);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;

        }

        [WebMethod(Description = "This method will return Value of matching DependentSpendCriteria")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public decimal GetDependentSpendFactor(decimal noOfDep, out string err)
        {
            decimal spendFactor = 0;
            BSpend bs = new BSpend();
            SqlConnection conn = null;
             err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {

                    spendFactor = bs.GetDependentSpendFactor(conn, trans,noOfDep);
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

            return spendFactor;
        }

        [WebMethod(Description = "This method will return Value of matching ApplicantSpendCriteria")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public decimal GetApplicantSpendFactor(string income, out string err)
        {
            decimal spendFactor = 0;
            BSpend bs = new BSpend();
            SqlConnection conn = null;
             err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {

                    spendFactor = bs.GetApplicantSpendFactor(conn, trans, income);
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

            return spendFactor;
        }

        [WebMethod(Description = "This method will return Values of DependentSpendMatrix")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetDependentSpendMatrix(out string err)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable(); 
            BSpend bs = new BSpend();
            SqlConnection conn = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {

                    dt = bs.GetDependentSpendFactorMatrix(conn, trans);

                    ds.Tables.Add(dt);
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
        [WebMethod(Description = "This method will return an XML node listing all the scoring rules currently defined for the selected rule type")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveScoringRules(string country,
                                    XmlNode rules,
                                    string region,
                                    out string err)
        {
            Function = "WCreditManager::SaveScoringRules()";
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
                            var Log = new ArrayList();
                            Log.Add("Scoring Rules Saved Or Imported");
                            
                            EventStore.Instance.Log(Log, "RoleMaintenance", EventCategory.SystemMaintenance
                                    , new { empeeno = User });
                            BScoring score = new BScoring();
                            score.User = STL.Common.Static.Credential.UserId;
                            score.SaveRules(conn, trans, country, rules, region);

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

        [WebMethod(Description = "This method will evaluate the customer account data against the countries scoring rules")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int ScoreProposal(string country, string accountNo, string accountType, string customerID, DateTime dateProp, short branchNo, ref bool referDeclined, out string newBand, out string refCode, out decimal points, out decimal RFLimit, out string result, out string bureauFailure, int user, out string referralReasons, out string err) //IP - 14/03/11 - #3314 - CR1245 - Returning referral reasons
        {
            Function = "WCreditManager::ScoreProposal()";
            err = "";
            refCode = "";
            points = 0;
            result = "";
            RFLimit = 0;
            bureauFailure = "";
            newBand = "";
            referralReasons = string.Empty; //IP - 14/03/11 - #3314 - CR1245

            SqlConnection conn = null;
            
            BProposal prop = null;
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted); //xxhh change back to using once miguel fixed. 
                        {
                            prop = new BProposal();
                            prop.User = STL.Common.Static.Credential.UserId;
                            prop.Score(conn, trans, country, accountNo, accountType, customerID, dateProp, branchNo, out newBand, out refCode, out points, out RFLimit, user, out result, out bureauFailure, ref referDeclined, out referralReasons); //IP - 14/03/11 - #3314 - CR1245 - Returning referral reasons
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

        [WebMethod(Description = "This method will return an XML node listing all the scoring rules currently defined for the selected rule type")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetRFScoringMatrix(string country, char scoretype, string region, out string err)
        {
            Function = "WCreditManager::GetRFScoringMatrix()";
            err = "";
            DataSet matrix = null;
            try
            {
                BScoring score = new BScoring();
                matrix = score.GetMatrix(country,scoretype, region);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return matrix;
        }

        [WebMethod(Description = "Returns the terms type matrix by scoring band")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetTermsTypeMatrix(string country, char scoretype, out string err)
        {
            Function = "WCreditManager::GetTermsTypeMatrix()";
            err = "";
            DataSet matrix = null;
            try
            {
                BScoring score = new BScoring();
                matrix = score.GetTermsTypeMatrix(country, scoretype);// SC CR1034 17-02-10
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return matrix;
        }

        
        [WebMethod(Description = "This method will return an XML node listing all the scoring rules currently defined for the selected rule type")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveRFScoringMatrix(string fileName, string countryCode, char scoretype, string region, DataSet matrix,
         bool newImport, out string err)
        {
            Function = "WCreditManager::SaveRFScoringMatrix()";
            err = "";
            SqlConnection conn = null;
            
            BScoring sm = null;
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
                            var Log = new ArrayList();
                            Log.Add("Scoring Matrix Saved Or Imported");
                            EventStore.Instance.Log(Log, "RoleMaintenance", EventCategory.SystemMaintenance
                                    , new { empeeno = User });
                            sm = new BScoring();
                            sm.User = STL.Common.Static.Credential.UserId;
                            sm.SaveMatrix(conn, trans, fileName, countryCode, scoretype, region, matrix, newImport);
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


        [WebMethod(Description = "Saves the terms type matrix of service charge by scoring band")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveTermsTypeMatrix(string fileName, string countryCode, char scoretype, DataSet matrix,
                bool newImport, out string err)
        {
            Function = "WCreditManager::SaveTermsTypeMatrix()";
            err = "";
            SqlConnection conn = null;
            
            BScoring sm = null;
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
                            sm = new BScoring();
                            sm.User = STL.Common.Static.Credential.UserId;
                            sm.SaveTermsTypeMatrix(conn, trans, fileName, countryCode, scoretype, matrix, newImport);
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

        [WebMethod(Description = "Saves the terms type matrix of service charge by scoring band")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int ApplyTermsTypeMatrix(DateTime startDate,char scorecard, out string err)
        {
            Function = "WCreditManager::ApplyTermsTypeMatrix()";
            err = "";
            SqlConnection conn = null;
            
            BScoring sm = null;
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
                            sm = new BScoring();
                            sm.User = STL.Common.Static.Credential.UserId;
                            sm.ApplyTermsTypeMatrix(conn, trans, startDate, scorecard);
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


        [WebMethod(Description = "This method will return an XML node listing all the scoring rules currently defined for the selected rule type")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int ManualRefer(string customerID, string accountNo, DateTime dateProp, bool isManualRefer, out string err, bool cashLoan = false)
        {
            Function = "WCreditManager::ManualRefer()";
            err = "";
            BProposal prop = new BProposal();
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
                            prop.User = STL.Common.Static.Credential.UserId;
                            prop.ManualRefer(conn, trans, accountNo, customerID, dateProp, isManualRefer, cashLoan);
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

        [WebMethod(Description = "This method will record the referral decision in the proposal table and clear the referral proposal flag")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetReferralData(string customerID, string accountNo, DateTime dateProp, string countryCode, out string err)
        {
            Function = "WCreditManager::GetReferralData()";
            err = "";
            DataSet refData = null;
            try
            {
                BProposal prop = new BProposal();
                refData = prop.GetReferralData(customerID, accountNo, dateProp, countryCode);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return refData;
        }

        [WebMethod(Description = "This method will record the referral decision in the proposal table and clear the referral proposal flag")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int CompleteReferralStage(string customerID, string accountNo, DateTime dateProp, string newNotes, string notes, bool approved, bool rejected, bool reOpen, int branch, decimal creditLimit, string CountryCode, out string err)
        {
            Function = "WCreditManager::CompleteReferralStage()";
            err = "";
            SqlConnection conn = null;
            
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
                            prop.CompleteReferralStage(conn, trans, customerID, accountNo, dateProp, newNotes, notes, approved, rejected, reOpen, branch, creditLimit, CountryCode);
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

        [WebMethod(Description = "This menu will record the referral notes in the proposal table")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveReferralNotes(string customerID, string accountNo, DateTime dateProp, string newNote, decimal creditLimit, string countryCode, out string err)
        {
            Function = "WCreditManager::CompleteReferralStage()";
            err = "";
            SqlConnection conn = null;
            
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
                            prop.SaveReferralNotes(conn, trans, customerID, accountNo, dateProp, newNote, creditLimit, countryCode);
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

        [WebMethod(Description = "This method simply updates the Notes in the Proposal table")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveProposalNotes(string customerID, string accountNo, DateTime dateProp, string notes, out string err)
        {
            Function = "WCreditManager::SaveProposalNotes()";
            err = "";
            SqlConnection conn = null;
            
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
                            prop.SaveProposalNotes(conn, trans, customerID, accountNo, dateProp, notes);
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


        [WebMethod(Description = "This method will return all Stage 2 proposal details for a particular account or customer")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string FindSecondApplicant(string customerID, string accountNo, string relation, out string err)
        {
            Function = "WCreditManager::FindSecondApplicant()";
            err = "";
            string custID = "";

            try
            {
                BProposal p = new BProposal();
                custID = p.FindSecondApplicant(customerID, accountNo, relation);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return custID;
        }

        [WebMethod(Description = "This method will return any data currently stored for this proposal about document confirmation")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetDocConfirmationData(string customerID, string accountNo, DateTime dateProp, out string err)
        {
            Function = "WCreditManager::GetDocConfirmationData()";
            err = "";
            DataSet docData = null;
            try
            {
                BProposal prop = new BProposal();
                docData = prop.GetDocConfirmationData(customerID, accountNo, dateProp);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return docData;
        }

        [WebMethod(Description = "This menu will record the referral notes in the proposal table")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveDocConfirmation(string acctno, DataSet propData, bool complete, out string err)
        {
            Function = "WCreditManager::SaveDocConfirmation()";
            err = "";
            SqlConnection conn = null;
            
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
                            prop.SaveDocConfirmation(conn, trans, propData, complete, acctno);
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

        [WebMethod(Description = "This method will return any data currently stored for this proposal about document confirmation")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int GetUnclearedStage(string accountNo, ref string newAccount, ref string checkType, ref DateTime dateProp, ref string propResult, out string err)
        {
            Function = "WCreditManager::GetUnclearedStage()";
            err = "";
            try
            {
                BProposal prop = new BProposal();
                int points = 0;
                prop.GetUnclearedStage(null, null, accountNo, ref newAccount, ref checkType, ref dateProp, ref propResult,ref points);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0;
        }

        [WebMethod(Description = "This menu will record the referral notes in the proposal table")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int UnClearFlag(string accountNo, string checkType, bool changeStatus, int user, out string err)
        {
            Function = "WCreditManager::UnClearFlag()";
            err = "";
            SqlConnection conn = null;
            
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
                            prop.UnClearFlag(conn, trans, accountNo, checkType, changeStatus, user);
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

        [WebMethod(Description = "Transact test")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int TestTransact(string accountNo, out string err)
        {
            Function = "WCreditManager::TestTransact()";
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
                            BTransact t = new BTransact();
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

        [WebMethod(Description = "test schema validation")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public string TestSchemaValidation(out string err)
        {
            Function = "WCreditManager::TestSchemaValidation()";
            err = "";
            string response = "";
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
                            STL.BLL.CreditBureau.ConsumerEnquiryResponse cer = new STL.BLL.CreditBureau.ConsumerEnquiryResponse();
                            XmlDocument resp = new XmlDocument();
                            resp.Load("c:\\temp\\SampleResponse.xml");
                            cer.ProcessResponse(conn, trans, "WP123456A", resp.DocumentElement);

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
            return response;
        }

        [WebMethod(Description = "")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetReferralSummaryData(string accountNo,
                                            string customerID,
                                            string accountType,
                                            DateTime dateProp,
                                            out XmlNode lineItems,
                                            out string err)
        {
            Function = "WCreditManager::GetReferralSummaryData()";
            err = "";
            lineItems = null;
            DataSet ds = null;
            try
            {
                BProposal p = new BProposal();
                ds = p.GetReferralSummaryData(accountNo, customerID, accountType, dateProp, out lineItems);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "This method will return an XML node listing all the scoring rules currently defined for the selected rule type")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SpendLimitReferral(string customerID, string accountNo, DateTime dateProp,
                                string newNotes, decimal creditLimit, string countryCode, out string err)
        {
            Function = "WCreditManager::SpendLimitReferral()";
            err = "";
            BProposal prop = new BProposal();
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
                            prop.User = STL.Common.Static.Credential.UserId;
                            prop.SpendLimitReferral(conn, trans, accountNo, customerID, dateProp, newNotes, creditLimit, countryCode);
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


        [WebMethod(Description = "This method will save details to the CustomerScoreHistTable")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveScoreHist(string CustomerID, DateTime dateprop, char? scorecard, short? points , float creditlimit,
            string scoringband, int user, string reasonchanged, string AccountNo,
            out string err)
        {
            Function = "WCreditManager::SaveScoreHist()";
            err = "";
            BProposal prop = new BProposal();
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
                            prop.User = STL.Common.Static.Credential.UserId;
                            prop.SaveScoreHist(conn, trans, CustomerID, dateprop, scorecard, points, creditlimit, scoringband,
                                user, reasonchanged, AccountNo);
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

        [WebMethod(Description = "This method will Load customers for behavioural Scoring Rescore")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet LoadBSCustomers(string category, int runno , out string err)
        {
            Function = "WCreditManager::LoadBSCustomers()";
            DataSet ds = null;
            err = "";
            BProposal prop = new BProposal();
            try
            {
                ds = prop.LoadBSCustomers(category, runno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }
        
        [WebMethod(Description = "Load Score/Band History for customers sorted by most recent change")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet LoadScoreHistforCustomer(string CustomerId, out string err)
        {
            Function = "WCreditManager::LoadScoreHistforCustomer()";
            DataSet ds = null;
            err = "";
            BProposal prop = new BProposal();
            try
            {
                ds = prop.LoadScoreHistforCustomer(CustomerId);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        [WebMethod(Description = "Applies Rescore for account ")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int ApplyBSRescore(string CustomerID,int runno, out string err)
        {
            Function = "WCreditManager::ApplyBSRescore()";
            err = "";
            BProposal prop = new BProposal();
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
                            prop.User = STL.Common.Static.Credential.UserId;
                            prop.ApplyBSRescore(conn, trans, CustomerID, 0, runno);
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
      
        //IP - 15/03/11 - #3314 - CR1245
        [WebMethod(Description = "This method will save referral notes when doing a manual referral")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveManualReferralNotes(string customerID, string accountNo, DateTime dateProp,
                                string notes, decimal creditLimit, string countryCode, out string err)
        {
            Function = "WCreditManager::SaveManualReferralNotes()";
            err = "";
            BProposal prop = new BProposal();
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
                            prop.User = STL.Common.Static.Credential.UserId;
                            prop.SaveReferralNotes(conn, trans, customerID, accountNo, dateProp, notes, creditLimit, countryCode);
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

        [WebMethod(Description = "This method returns the MMI matrix")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetMmiMatrix(out string err)
        {
            Function = "WCreditManager::GetMmiMatrix()";
            err = "";
            DataSet matrix = null;
            try
            {
                BMmi mmi = new BMmi();
                matrix = mmi.GetMmiMatrix();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return matrix;
        }

        [WebMethod(Description = "This method saves the MMI matrix")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveMmiMatrix(DataSet matrix, out string err)
        {
            Function = "WCreditManager::SaveMmiMatrix()";
            err = "";
            SqlConnection conn = null;

            BMmi mmi = null;
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
                            mmi = new BMmi();
                            mmi.User = STL.Common.Static.Credential.UserId;
                            mmi.SaveMmiMatrix(conn, trans, matrix);
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

        [WebMethod(Description = "This method saves the customer MMI value by calculating percentage of Deposible income based on customer score configuration inside MMI matrix")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveCustomerMmi(string custId, int userId, string reasonChanged, out string err)
        {
            Function = "WCreditManager::SaveCustomerMmi()";
            err = string.Empty;
            SqlConnection conn = null;

            BMmi mmi = null;
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
                            mmi = new BMmi();
                            mmi.User = STL.Common.Static.Credential.UserId;
                            mmi.SaveCustomerMmi(conn, trans, custId, userId, reasonChanged);
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

        [WebMethod(Description = "This method is used to get sum of MMI value and respective threshold percentage of MMI based on selected Term Type.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int GetMmiThresholdForSale(string custId, string acctNo, string termType, out bool isMmiAllowed, out decimal mmiLimit, out decimal mmiThreshold, out string err)
        {
            Function = "WCreditManager::GetMmiThresholdForSale()";
            err = string.Empty;
            isMmiAllowed = false;
            mmiLimit = 0;
            mmiThreshold = 0;
            SqlConnection conn = null;

            BMmi mmi = null;
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
                            mmi = new BMmi();
                            mmi.User = STL.Common.Static.Credential.UserId;
                            mmi.GetMmiThresholdForSale(conn, trans, custId, acctNo, termType, out isMmiAllowed, out mmiLimit, out mmiThreshold);
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


        [WebMethod(Description = "This method is used to add a audit trail for monitoring instances where MMI threshold is used allowing instalment amount.")]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int AuditMmiThresholdUsedInstalment(string custId, string acctNo, decimal mmi, decimal mmiThreshold, decimal installment, DateTime activityDate, out string err)
        {
            Function = "WCreditManager::AuditMmiThresholdUsedInstalment()";
            err = string.Empty;

            try
            {
                var document = "InstalmentWithMmiThreshold";

                EventStore.Instance.Log(new {
                                                Activity = "Instalment amount get exceed within Threshold %",
                                                Customer = custId,
                                                Account = acctNo,
                                                MMI = "$" + mmi.ToString(),
                                                Threshold = "$" + mmiThreshold.ToString(),
                                                Instalment = "$" + installment.ToString(),
                                                AgreementDate = DateTime.Now
                                            }
                                            , document
                                            , EventCategory.InstalmentWithMmiThreshold
                                            , new { empeeno = STL.Common.Static.Credential.UserId });
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return 0;
        }
    }
}

