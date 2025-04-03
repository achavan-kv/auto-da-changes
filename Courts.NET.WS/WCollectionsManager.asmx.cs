using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using STL.DAL;
using STL.Common;
using STL.Common.Static;
using STL.BLL;
using STL.Common.Constants.ColumnNames;



namespace STL.WS
{
    [WebService(Namespace = "http://strategicthought.com/webservices/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class WCollectionsManager : CommonService
    {
        public WCollectionsManager()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 


        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetConditions(out string err)
        {
            Function = "BCollectionsModule::GetConditions()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetConditions();
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
        public DataSet GetActions(out string err)
        {
            Function = "BCollectionsModule::GetActions()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetActions();
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
        public DataSet GetActionsWithStrategy(out string err)
        {
            Function = "BCollectionsModule::GetActions()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetActionsWithStrategy();
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
        public DataSet GetStrategyActions(string strategy, out string err)
        {
            Function = "BCollectionsModule::GetStrategyActions()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetStrategyActions(strategy);
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
        public DataSet GetWorkLists(out string err)
        {
            Function = "BCollectionsModule::GetWorkLists()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetWorkLists();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

//        [WebMethod]
//        [SoapHeader("authentication")]
//#if(TRACE)
//        [TraceExtension]
//#endif
//        public int SaveWorkList(string workList, string description, string action, DataSet dsWorkList, out string err)
//        {
//            Function = "BCollectionsModule::SaveWorkList()";
            
//            err = "";

//            try
//            {
//                using (SqlConnection conn = new SqlConnection(Connections.Default))
//                {
//                    do
//                    {
//                        try
//                        {
//                            conn.Open();
//                            using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
//                            {
//                                BCollectionsModule collections = new BCollectionsModule();
//                                collections.SaveWorkList(conn, trans, workList, description, action, dsWorkList);
//                                trans.Commit();
//                            }
//                            break;
//                        }
//                        catch (SqlException ex)
//                        {
//                            CatchDeadlock(ex, conn);
//                        }
//                    } while (retries <= maxRetries);
//                }
//            }
//            catch (Exception ex)
//            {
//                Catch(ex, Function, ref err);
//            }
             
//            return 0;
//        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetStrategyConditions(string strategy, out string err)
        {
            Function = "BCollectionsModule::GetStrategyConditions()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetStrategyConditions(strategy);
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
        public DataSet GetStrategies(out string err)
        {
            Function = "BCollectionsModule::GetStrategies()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetStrategies();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        //IP - 20/10/08 - UAT5.2 - UAT(551)
        //Return the 'Exit Condition Strategies' for the selected strategy.
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetStrategiesToSendTo(string strategy, out string err)
        {
            Function = "BCollectionsModule::GetStrategiesToSendTo()";
            DataSet dsStrategiesToSendTo = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                dsStrategiesToSendTo = collections.GetStrategiesToSendTo(strategy);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return dsStrategiesToSendTo;
        }
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetStrategiesForBailiff(out string err)
        {
            Function = "BCollectionsModule::GetStrategies()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetStrategiesForBailiff();
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
        public DataSet GetLetters(out string err)
        {
            Function = "BCollectionsModule::GetLetters()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetLetters();
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
        public DataSet GetSMS(out string err)
        {
            Function = "BCollectionsModule::GetSMS()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetSMS();
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
        public DataSet GetWorklistDataSet(out string err)
        {
            Function = "BCollectionsModule::GetStrategyWorklists()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetStrategyWorklists();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }

            return ds;
        }

        //IP - 02/06/09 - Credit Collection Walkthrough Changes - Allocations check
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveStrategyCondition(string strategy, string description, DataSet dsStrategy, bool canBeAllocated, bool manual, int empeeno, out string err)    //UAT987
        {
            Function = "BCollectionsModule::SaveStrategyCondition()";
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
                        using (var trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BCollectionsModule collections = new BCollectionsModule();
                            collections.SaveStrategyCondition(conn, trans, strategy, description, dsStrategy, canBeAllocated, manual, empeeno);     //UAT987
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
        public int SaveSMS(string smsName, string smsBody, string description, out string err)
        {
            Function = "BCollectionsModule::SaveSMS()";
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
                        using (var trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BCollectionsModule collections = new BCollectionsModule();
                            collections.SaveSMS(conn, trans, smsName, smsBody, description);
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
        public int SetStrategyActive(string strategy, int activeValue, out string err)
        {
            Function = "BCollectionsModule::SetStrategyActive()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        BCollectionsModule collections = new BCollectionsModule();
                        collections.SetStrategyActive(conn, trans, strategy, activeValue);
                        trans.Commit();
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
        public int DeleteWorkList(string worklist, out string err)
        {
            Function = "BCollectionsModule::DeleteWorkList()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        BCollectionsModule collections = new BCollectionsModule();
                        collections.DeleteWorkList(conn, trans, worklist);
                        trans.Commit();
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
        public int DeleteSMS(string sms, out string err)
        {
            Function = "BCollectionsModule::DeleteSMS()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        BCollectionsModule collections = new BCollectionsModule();
                        collections.DeleteSMS(conn, trans, sms);
                        trans.Commit();
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
        //NM & IP - 06/01/09 - CR976 - Included 'worklist' parameter, for manually sending an account to a worklist.
        //NM & IP - 07/01/09 - CR976 - Extra Telephone Actions - 'STW' (Send to WriteOff) - added parameters 'reasonForWriteOff' and 'empeeno'
        public int UpdateStrategyAccounts(string strategy, string acct, string worklist, string reasonForWriteOff, int empeeno, out string err)
        {
            Function = "BCollectionsModule::UpdateStrategyAccounts()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        BCollectionsModule collections = new BCollectionsModule();
                        collections.UpdateStrategyAccounts(conn, trans, acct, strategy, worklist, reasonForWriteOff, empeeno);
                        trans.Commit();
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
        public int LockAccount(string acct, int user, out string err)
        {
            Function = "BCollectionsModule::LockAccount()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        BCollectionsModule collections = new BCollectionsModule();
                        collections.LockAccount(conn, trans, acct, user);
                        trans.Commit();
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
        public int UnlockAccount(string acct, int user, out string err)
        {
            Function = "BCollectionsModule::UnlockAccount()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        BCollectionsModule collections = new BCollectionsModule();
                        collections.UnlockAccount(conn, trans, acct, user);
                        trans.Commit();
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

        //IP - UAT(514) - Delete the selected existing Strategy
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int DeleteExistingStrategy(string strategy, out string err)
        {
            Function = "BCollectionsModule::DeleteExistingStrategy()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        BCollectionsModule collections = new BCollectionsModule();
                        collections.DeleteExistingStrategy(conn, trans, strategy);
                        trans.Commit();
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

        //NS & IP - 23/12/08 - CR976
        //Method which retrieves all the actions that the employee has rights to
        //for the strategy associated with the selected account
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetStrategyActionsForEmployee(int empeeno, string strategy, bool checkForSupervisorRight, out string err)
        {
            Function = "BCollectionsModule::GetStrategyActionsForEmployee()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetStrategyActionsForEmployee(empeeno, strategy, checkForSupervisorRight);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        //NM - 29/12/08 - CR976
        //Method to update or insert customer telephone numbers from 
        //TelephoneActions screen
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int UpdateCustomerTelephoneNo(string custid, string HTelNo, string HDialCode,
            string WTelNo, string WDialCode, string MTelNo, string MDialCode, int Empeeno, bool HomeTelephoneChanged, bool WorkTelephoneChanged, bool MobileTelephoneChanged, out string err)
        {
            Function = "BCollectionsModule::UpdateCustomerTelephoneNo()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        BCollectionsModule collections = new BCollectionsModule();
                        collections.UpdateCustomerTelephoneNo(conn, trans, custid, HTelNo, HDialCode, WTelNo, WDialCode, MTelNo, MDialCode, Empeeno, HomeTelephoneChanged, WorkTelephoneChanged, MobileTelephoneChanged);
                        trans.Commit();
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

        //NM - 06/01/09 - CR976
        //Method to insert customer bailaction with Extra Telephone Actions
        //TelephoneActions screen
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveBailActionsWithTelephoneActions(string acctNo, int empeeNo, string code, string notes, DateTime dateDue,
            double actionValue, DateTime spaDateExpiry, string spaReasonCode, double spaInstal, DateTime reminderDateTime, bool cancelOutstandingReminders,
            DataSet dsExtraActionDetail, string callingSource, out string err)
        {
            Function = "BCollectionsModule::SaveBailActionsWithTelephoneActions()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";
            string baCode = code;
            DataRow dr = dsExtraActionDetail.Tables[0].Rows[0];          

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        BAccount acct = new BAccount();
                        acct.User = STL.Common.Static.Credential.UserId;
                        if (code == "TRC" && Convert.ToString(Convert.ToBoolean(dr[CN.CMIsResolved])) == "True")     //TRC resolved
                        {
                            baCode = "TRR";
                        }

                        acct.SaveBailActions(conn, trans, acctNo, empeeNo, baCode, notes,
                            dateDue, actionValue, spaDateExpiry, spaReasonCode, spaInstal, reminderDateTime, cancelOutstandingReminders, callingSource);

                        BCollectionsModule collections = new BCollectionsModule();
                        collections.UpdateExtraTelephoneActionDetail(conn, trans, code, dsExtraActionDetail);
                        trans.Commit();
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

        //NM - 20/01/2009 - CR976
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int UpdateWorkList_ActionRights(DataSet dsRightsInfo, out string err)
        {
            Function = "BCollectionsModule::UpdateWorkList_ActionRights()";
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

                            BCollectionsModule collections = new BCollectionsModule();
                            collections.UpdateWorkList_ActionRights(conn, trans, dsRightsInfo, STL.Common.Static.Credential.UserId);
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

        //NM - 22/01/2009 - CR976
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveWorkLists(DataSet dsWorkList, out string err)
        {
            Function = "BCollectionsModule::SaveWorkLists()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                        BCollectionsModule collections = new BCollectionsModule();
                        collections.SaveWorkLists(conn, trans, dsWorkList);
                        trans.Commit();
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

        //NM 22/01/2009
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet LoadWorkListOrderColumns(out string err)
        {
            Function = "BCollectionsModule::LoadWorkListOrderColumns()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.LoadWorkListOrderColumns();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }


        //NM 22/01/2009
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet LoadWorkListSortOrder(out string err)
        {
            Function = "BCollectionsModule::LoadWorkListSortOrder()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.LoadWorkListSortOrder();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        //NM - 22/01/2009 - CR976
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int UpdateWorkListSortOrder(DataSet dsSortOrder, out string err)
        {
            Function = "BCollectionsModule::UpdateWorkListSortOrder()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                        BCollectionsModule collections = new BCollectionsModule();
                        collections.UpdateWorkListSortOrder(conn, trans, dsSortOrder);
                        trans.Commit();
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


        //NM 26/01/2009
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetDistinctLetterCodesByRunNo(int runNo, out string err)
        {
            Function = "BCollectionsModule::LoadWorkListSortOrder()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetDistinctLetterCodesByRunNo(runNo);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }


        //NM 27/01/2009
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet LoadLetterFieldsbyCode(string letterCode, int runNo, char storeType, bool spouseDetailRequired, bool guarantorDetailRequired, out string err)
        {
            Function = "BCollectionsModule::LoadLetterFieldsbyCode()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.LoadLetterFieldsbyCode(letterCode, runNo, storeType, spouseDetailRequired, guarantorDetailRequired);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        //NM 29/01/2009
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetBranchByStoreType(char storeType, out string err)
        {
            Function = "BCollectionsModule::GetBranchByStoreType()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetBranchByStoreType(storeType);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        //NM 03/02/2009
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetZones(out string err)
        {
            Function = "BCollectionsModule::GetZones()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetZones();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        //NM 11/02/2009
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetZoneAllocatableEmployeeInfo(out string err)
        {
            Function = "BCollectionsModule::GetZoneAllocatableEmployeeInfo()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetZoneAllocatableEmployeeInfo();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }


        //NM 29/01/2009
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetActionRightsHierarchy(out string err)
        {
            Function = "BCollectionsModule::GetActionRightsHierarchy()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetActionRightsHierarchy();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        //NM 29/01/2009
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetWorklistRightsHierarchy(out string err)
        {
            Function = "BCollectionsModule::GetWorklistRightsHierarchy()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetWorkListRightsHierarchy();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }


        //NM & IP - 02/01/09 - CR976 - Method that will retrieve all the call reminders
        //for the selected account for the current user
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetCallReminderInfo(string acctno, int empeeno, out string err)
        {
            Function = "BCollectionsModule::GetCallReminderInfo()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetCallReminderInfo(acctno, empeeno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }



        //NM & IP - 02/01/09 - CR976 - CR976 -Method that will retrieve any 'Legal', 'Fraud', or 
        //'Insurance' details for a selected account if any have been entered.
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetLegalFraudInsuranceDetails(string acctno, out string err)
        {
            Function = "BCollectionsModule::GetLegalFraudInsuranceDetails()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetLegalFraudInsuranceDetails(acctno);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

//        [WebMethod]
//        [SoapHeader("authentication")]
//#if(TRACE)
//        [TraceExtension]
//#endif
//        public int SaveWorklistRights(
//        int Employee, string Worklist, string EmployeeType, int user, out string err)
//        {
//            Function = "BCollectionsModule::SaveWorklistRights()";
//            SqlConnection conn = null;
//            SqlTransaction trans = null;
//            err = "";

//            try
//            {
//                conn = new SqlConnection(Connections.Default);
//                do
//                {
//                    try
//                    {
//                        conn.Open();
//                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
//                        BCollectionsModule collections = new BCollectionsModule();
//                        collections.SaveWorklistRights(conn, trans, Employee, Worklist, EmployeeType, user);
//                        trans.Commit();
//                        break;
//                    }
//                    catch (SqlException ex)
//                    {
//                        CatchDeadlock(ex, conn);
//                    }
//                } while (retries <= maxRetries);
//            }
//            catch (Exception ex)
//            {
//                Catch(ex, Function, ref err);
//            }
//            finally
//            {
//                if (conn.State != ConnectionState.Closed)
//                    conn.Close();
//            }
//            return 0;
//        }

//        [WebMethod]
//        [SoapHeader("authentication")]
//#if(TRACE)
//        [TraceExtension]
//#endif


//        public int SaveActionRights(
//          int Employee, string Strategy, string Action, string EmployeeType, bool CycleToNextFlag,
//     int MinNotesLength, int user, out string err)
//        {
//            Function = "BCollectionsModule::SaveActionRights()";
//            SqlConnection conn = null;
//            SqlTransaction trans = null;
//            err = "";

//            try
//            {
//                conn = new SqlConnection(Connections.Default);
//                do
//                {
//                    try
//                    {
//                        conn.Open();
//                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
//                        BCollectionsModule collections = new BCollectionsModule();
//                        collections.SaveActionRights(conn, trans, Employee, Strategy, Action, EmployeeType, CycleToNextFlag, MinNotesLength, user);
//                        trans.Commit();
//                        break;
//                    }
//                    catch (SqlException ex)
//                    {
//                        CatchDeadlock(ex, conn);
//                    }
//                } while (retries <= maxRetries);
//            }
//            catch (Exception ex)
//            {
//                Catch(ex, Function, ref err);
//            }
//            finally
//            {
//                if (conn.State != ConnectionState.Closed)
//                    conn.Close();
//            }
//            return 0;
//        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet LoadAllocateableStaffandTypes(out string err)
        {
            Function = "BCollectionsModule::LoadAllocateableStaffandTypes()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.LoadAllocateableStaffandTypes();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

//        [WebMethod]
//        [SoapHeader("authentication")]
//#if(TRACE)
//        [TraceExtension]
//#endif

        //public int UpdateCycleToforActions(string strategy, string actioncode, bool CycleToNextFlag, out string err)
        //{
        //    Function = "BCollectionsModule::UpdateCycleToforActions()";
        //    SqlConnection conn = null;
        //    SqlTransaction trans = null;
        //    err = "";

        //    try
        //    {
        //        conn = new SqlConnection(Connections.Default);
        //        do
        //        {
        //            try
        //            {
        //                conn.Open();
        //                trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
        //                BCollectionsModule collections = new BCollectionsModule();
        //                collections.UpdateCycleToforActions(conn, trans, strategy, actioncode, CycleToNextFlag);
        //                trans.Commit();
        //                break;
        //            }
        //            catch (SqlException ex)
        //            {
        //                CatchDeadlock(ex, conn);
        //            }
        //        } while (retries <= maxRetries);
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, Function, ref err);
        //    }
        //    finally
        //    {
        //        if (conn.State != ConnectionState.Closed)
        //            conn.Close();
        //    }
        //    return 0;
        //}



        //IP & JC - 12/01/09 - CR976 - Method which will retrieve the details for 
        //combined Ready Finance accounts for a Customer to be displayed
        //on the 'Special Arrangements Consolidated' screen.
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet AccountGetCombinedSPADetails(string custID, out string err)
        {
            Function = "BCollectionsModule::AccountGetCombinedSPADetails()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.AccountGetCombinedSPADetails(custID);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        //IP & JC - 15/01/09 - CR976 
        //Method that will save a record to BailAction for the SPA action, and also 
        //write records to the 'SPA' table for the SPA Schedule for the account or
        //consolidated accounts.
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveBailActionsForSPA(string acctNo, int empeeNo, string code, string notes, DateTime dateDue,
            double actionValue, DateTime spaDateExpiry, string spaReasonCode, double spaInstal, DateTime reminderDateTime, bool cancelOutstandingReminders,
            DataTable dtSPADetails, string callingSource, out string err)
        {
            Function = "BAccount::SaveBailActionsForSPA()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        BAccount acct = new BAccount();
                        acct.User = STL.Common.Static.Credential.UserId;

                        //Write SPA first because of lock on BailAction
                        acct.SPAWriteArrangementSchedule(conn, trans, empeeNo, dtSPADetails);

                        foreach (DataRow dr in dtSPADetails.Rows)
                        {
                            if (Convert.ToDecimal(dr[CN.ArrangementAmount]) > 0)
                            {
                                acct.SaveBailActions(conn, trans, dr[CN.AccountNo].ToString(), empeeNo, code, notes,
                                    dateDue, Convert.ToDouble(dr[CN.ArrangementAmount]), spaDateExpiry, spaReasonCode, Convert.ToDouble(dr[CN.ArrangementAmount]), reminderDateTime, cancelOutstandingReminders, callingSource);
                            }
                        }

                        trans.Commit();
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

        //IP & JC - CR976 - 21/01/09
        //Method that will write the new Instalplan and Agreement records for Extended Term SPA.
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SPAWriteRefinance(string acctNo, int empeeNo, string code, string notes, DateTime dateDue,
            double actionValue, DateTime spaDateExpiry, string spaReasonCode, double spaInstal, DateTime reminderDateTime, bool cancelOutstandingReminders,
            DataTable dtSPADetails, string callingSource, out string err)
        {
            Function = "BAccount::SPAWriteRefinance()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        
                        BAccount acct = new BAccount();
                        acct.User = STL.Common.Static.Credential.UserId;

                        //Write SPA first because of lock on BailAction
                        acct.SPAWriteRefinance(conn, trans, empeeNo, dtSPADetails);

                        //conn.Open(); Don't need any of this - AA
                        //trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        //// temporarily removed
                        ////foreach (DataRow dr in dtSPADetails.Rows)
                        ////{

                        ////    acct.SaveBailActions(conn, trans, dr[CN.AccountNo].ToString(), empeeNo, code, notes,
                        ////        dateDue, Convert.ToDouble(dr[CN.ArrangementAmount]), spaDateExpiry, spaReasonCode, Convert.ToDouble(dr[CN.ArrangementAmount]), reminderDateTime, cancelOutstandingReminders, callingSource);

                        ////}

                        //trans.Commit();
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

        //NM
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        
        public int SaveZoneRule(string Zone, DataSet dsZone, out string err)
        {
            Function = "BCollectionsModule::SaveZoneRule()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        BCollectionsModule collections = new BCollectionsModule();
                        collections.SaveZoneRule(conn, trans, Zone, dsZone);
                        trans.Commit();
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


        //NM
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public int SaveBailiffZoneAllocation(int empeeNo, DataSet dsZoneAllocation, out string err)
        {
            Function = "BCollectionsModule::SaveBailiffZoneAllocation()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        BCollectionsModule collections = new BCollectionsModule();
                        collections.SaveBailiffZoneAllocation(conn, trans, empeeNo, dsZoneAllocation, STL.Common.Static.Credential.UserId);
                        trans.Commit();
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

        public int DeleteZone(string Zone, out string err)
        {
            Function = "BCollectionsModule::DeleteZone()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        BCollectionsModule collections = new BCollectionsModule();
                        collections.DeleteZone(conn, trans, Zone);
                        trans.Commit();
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

        public int SaveZones(string code, string description, out string err)
        {
            Function = "BCollectionsModule::SaveZones()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        BCollectionsModule collections = new BCollectionsModule();
                        collections.SaveZones(conn, trans, code, description);
                        trans.Commit();
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

        public int ApplyZones(string zone, out string err)
        {
            Function = "BCollectionsModule::ApplyZones()";
            SqlConnection conn = null;
            SqlTransaction trans = null;
            err = "";

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        BCollectionsModule collections = new BCollectionsModule();
                        collections.ApplyZones(conn, trans, zone);
                        trans.Commit();
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
        public DataSet LoadUnallocatedAddressZones(out string err)
        {
            Function = "BCollectionsModule::LoadUnallocatedAddressZones()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.LoadUnallocatedAddressZones();
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
        public DataSet LoadZoneRules(string zone,out string err)
        {
            Function = "BCollectionsModule::LoadZoneRules()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.LoadZoneRules(zone);
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
#pragma warning disable 1587
        /// <summary>
        /// Loads up details from the CMBailiffAllocationRules - loads up the following columns 
        /// empeeno,empeetype,BranchorZone,IsZone,AllocationOrder,empeenochange,datechange,reallocate 
        /// </summary>
        ///
#pragma warning restore 1587
        public DataSet BailiffAllocationRulesLoad( out string err)
        {
            Function = "BCollectionsModule::BailiffAllocationRulesLoad()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.BailiffAllocationRulesLoad();
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
#pragma warning disable 1587
        /// <summary>
        /// </summary>
        ///
#pragma warning restore 1587
        public DataSet RolesGet(int permission)
        {
            Function = "BCollectionsModule::BailiffRolesGet()";
            var err = "";
            try
            {
                return new BCollectionsModule().GetRoles(permission);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return new DataSet();
        }

        //        [WebMethod]
        //        [SoapHeader("authentication")]
        //#if(TRACE)
        //        [TraceExtension]
        //#endif
        //        public int BailiffAllocationRulesSave(int empeeno, string empeetype, string branchorZone, bool isZone, Int16 allocationorder, int empeenochange,
        //                bool reallocate, out string err)
        //        {
        //            Function = "BCollectionsModule::BailiffAllocationRulesSave()";
        //            SqlConnection conn = null;
        //            SqlTransaction trans = null;
        //            err = "";

        //            try
        //            {
        //                conn = new SqlConnection(Connections.Default);
        //                do
        //                {
        //                    try
        //                    {
        //                        conn.Open();
        //                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
        //                        BCollectionsModule collections = new BCollectionsModule();
        //                        collections.BailiffAllocationRulesSave(conn, trans, empeeno,empeetype,branchorZone,isZone,allocationorder,empeenochange,reallocate);
        //                        trans.Commit();
        //                        break;
        //                    }
        //                    catch (SqlException ex)
        //                    {
        //                        CatchDeadlock(ex, conn);
        //                    }
        //                } while (retries <= maxRetries);
        //            }
        //            catch (Exception ex)
        //            {
        //                Catch(ex, Function, ref err);
        //            }
        //            finally
        //            {
        //                if (conn.State != ConnectionState.Closed)
        //                    conn.Close();
        //            }
        //            return 0;
        //        }
        ////        [WebMethod]
        ////        [SoapHeader("authentication")]
        ////#if(TRACE)
        ////        [TraceExtension]
        ////#endif
        ////        public int CheckCommissionDays(out int DaysSinceRun, out string err)
        ////        {
        ////            Function = "BCollectionsModule::BailiffAllocationRulesSave()";
        ////            err = "";

        ////            DaysSinceRun = 1000;
        ////                     try
        ////                    {
        ////                        BCollectionsModule collections = new BCollectionsModule();
        ////                        collections.CheckCommissionDays(out DaysSinceRun);
        ////                    }
        ////                    catch (SqlException )
        ////                    {
        ////                    }

        ////            return 0;
        ////        }

        // Address Standardization CR2019 - 025
        //SW 03/04/2020
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetVillages(out string err)
        {
            Function = "BCollectionsModule::GetVillages()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetVillages();
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        //SW 03/04/2020
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public DataSet GetRegions(string village, out string err)
        {
            Function = "BCollectionsModule::GetRegions()";
            DataSet ds = new DataSet();
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                ds = collections.GetRegions(village);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return ds;
        }

        //SW 4/13/2020
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif

        public string GetZipCode(string region, string village, out string err)
        {
            Function = "BCollectionsModule::GetZipCode()";
            string zipCode = string.Empty;
            err = "";

            try
            {
                BCollectionsModule collections = new BCollectionsModule();
                zipCode = collections.GetZipCode(region, village);
            }
            catch (Exception ex)
            {
                Catch(ex, Function, ref err);
            }
            return zipCode;
        }
        // Address Standardization CR2019 - 025
    }
}
