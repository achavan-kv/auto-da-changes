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
using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared;
using STL.Common.Static;

namespace STL.WS
{
	/// <summary>
	/// Summary description for WCreditManager.
	/// </summary>
	/// 
	[WebService(Namespace="http://strategicthought.com/webservices/")]
	public class WEODManager : CommonService
	{
		public WEODManager()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}
		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public DataSet GetEODControl(out string err)
		{
			Function = "WEODManager::GetEODControl()";
			err = "";
			DataSet ds = null;
		
			try
			{
				BInterfaceControl ic = new BInterfaceControl();
				ds = ic.GetEODControl();						
			}
			catch(Exception ex)
			{
				err = Function + ": "+ex.Message;
				logException(ex, Function);
			}
			
			return ds;
		}

		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetInterfaceControl(string eodInterface, string eodInterface2, bool allRuns, out string err)     //UAT1010 jec 09/07/10
		{
			Function = "WEODManager::GetInterfaceControl()";
			err = "";
			DataSet ds = null;
		
			try
			{
				BInterfaceControl ic = new BInterfaceControl();
                ds = ic.GetInterface(eodInterface, eodInterface2, allRuns);		//UAT1010 jec 09/07/10				
			}
			catch(Exception ex)
			{
				err = Function + ": "+ex.Message;
				logException(ex, Function);
			}
			
			return ds;
		}

		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetInterfaceError(string eodInterface, int runno, DateTime startdate, out string err)        //jec 06/04/11
		{
			Function = "WEODManager::GetInterfaceError()";
			err = "";
			DataSet ds = null;
		
			try
			{
				BInterfaceError ie = new BInterfaceError();
				ds = ie.GetInterfaceError(eodInterface, runno, startdate);      //jec 06/04/11
			}
			catch(Exception ex)
			{
				err = Function + ": "+ex.Message;
				logException(ex, Function);
			}
			
			return ds;
		}

		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public int EODControlUpdate(DataSet controls, out string err)
		{
			Function = "WEODManager::EODControlUpdate()";
			err = "";

			SqlConnection conn = null;
			
			BInterfaceControl ic = new BInterfaceControl();

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
                            ic.EODControlUpdate(conn, trans, controls);
                            trans.Commit();
                        }
						break;
					}
					catch(SqlException ex)
					{
						CatchDeadlock(ex, conn);
					}
				}while (retries <= maxRetries);
			}
			catch(Exception ex)
			{
				err = Function + ": "+ex.Message;
				logException(ex, Function);
			}
			finally
			{
				if(conn.State != ConnectionState.Closed)
					conn.Close();
			}
			return 0;
		}

		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public DataSet GetInterfaceValue(string eodInterface, int runno, out string err)
		{
			Function = "WEODManager::GetInterfaceValue()";
			err = "";
			DataSet ds = null;
		
			try
			{
				BInterfaceControl ic = new BInterfaceControl();
				ds = ic.GetInterfaceValue(eodInterface, runno);						
			}
			catch(Exception ex)
			{
				err = Function + ": "+ex.Message;
				logException(ex, Function);
			}
			
			return ds;
		}

		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public DataSet GetInterfaceFinancial(int runno, out string err)
		{
			Function = "WEODManager::GetInterfaceFinancial()";
			err = "";
			DataSet ds = null;
		
			try
			{
				BInterfaceControl ic = new BInterfaceControl();
				ds = ic.GetInterfaceFinancial(runno);						
			}
			catch(Exception ex)
			{
				err = Function + ": "+ex.Message;
				logException(ex, Function);
			}
			
			return ds;
		}

		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public DataSet GetInterfaceBreakdown(int runno, int branchNo, string interfaceAcctNo, out string err)
		{
			Function = "WEODManager::GetInterfaceBreakdown()";
			err = "";
			DataSet ds = null;
		
			try
			{
				BInterfaceControl ic = new BInterfaceControl();
				ds = ic.GetInterfaceBreakdown(runno, branchNo, interfaceAcctNo);						
			}
			catch(Exception ex)
			{
				err = Function + ": "+ex.Message;
				logException(ex, Function);
			}
			
			return ds;
		}

		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public DataSet GetInterfaceTransactions(int runno, int empeeno, string code, 
												string interfaceAcctNo, int branchNo, out string err)		
		{
			Function = "WEODManager::GetInterfaceTransactions()";
			err = "";
			DataSet ds = null;
		
			try
			{
				BInterfaceControl ic = new BInterfaceControl();
				ds = ic.GetInterfaceTransactions(runno, empeeno, code, interfaceAcctNo, branchNo);						
			}
			catch(Exception ex)
			{
				err = Function + ": "+ex.Message;
				logException(ex, Function);
			}
			
			return ds;
		}
		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public DataSet GetSummaryUpdateControlDetails(int firstrunno, int lastrunno, bool useLiveDatabase, out string err)		
		{
			Function = "WEODManager::GetSummaryUpdateControlDetails()";
			err = "";
			DataSet ds = null;
		
			try
			{
				BInterfaceControl ic = new BInterfaceControl();
				ds = ic.GetSummaryUpdateControlDetails(firstrunno, lastrunno, useLiveDatabase);						
			}
			catch(Exception ex)
			{
				err = Function + ": "+ex.Message;
				logException(ex, Function);
            throw;
			}
			
			return ds;
		}
		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public DataSet GetSummaryControlBrancgFigures(int runno, bool useLiveDatabase, out string err)		
		{
			Function = "WEODManager::GetSummaryControlBrancgFigures()";
			err = "";
			DataSet ds = null;
		
			try
			{
				BInterfaceControl ic = new BInterfaceControl();
				ds = ic.GetSummaryControlBrancgFigures(runno, useLiveDatabase);						
			}
			catch(Exception ex)
			{
				err = Function + ": "+ex.Message;
				logException(ex, Function);
			}
			
			return ds;
		}
		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public DataSet GetSummaryControlTotals(int runno, int branchno, string type, bool useLiveDatabase , out string err)		
		{
			Function = "WEODManager::GetSummaryControlTotals()";
			err = "";
			DataSet ds = null;
		
			try
			{
				BInterfaceControl ic = new BInterfaceControl();
				ds = ic.GetSummaryControlTotals(runno,branchno,type, useLiveDatabase);						
			}
			catch(Exception ex)
			{
				err = Function + ": "+ex.Message;
				logException(ex, Function);
			}
			
			return ds;
		}

		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public DataSet GetEodOptionList(string configurationName, out string err)		
		{
			Function = "WEODManager::GetEodOptionList()";
			err = "";
			DataSet ds = new DataSet();
		
			try
			{
				BInterfaceControl ic = new BInterfaceControl();
				DataTable dt = ic.GetEodOptionList(configurationName);
				ds.Tables.Add(dt);
			}
			catch(Exception ex)
			{
				err = Function + ": "+ex.Message;
				logException(ex, Function);
			}
			
			return ds;
		}

		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public int SaveEODConfigurationOptions(string confugurationName, string country, 
			int freqType, int startDate, int startTime, DataSet options,
            DateTime configStartDate, out string err)
		{
			Function = "WEODManager::SaveEODConfigurationOptions()";
			err = "";

			SqlConnection conn = null;
			
			BInterfaceControl ic = new BInterfaceControl();

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
                            ic.User = STL.Common.Static.Credential.UserId;

                            // Identify the root directory of the server app and pass this to the EODSaveJob routine 08/02/2008 JH
                            string url = HttpRuntime.AppDomainAppPath.ToString();

                            ic.SaveEODConfigurationOptions(conn, trans, confugurationName, country,
                                                    freqType, startDate, startTime, options,
                                                    configStartDate, url);
                            trans.Commit();
                        }
						break;
					}
					catch(SqlException ex)
					{
						CatchDeadlock(ex, conn);
					}
				}while (retries <= maxRetries);
			}
			catch(Exception ex)
			{
				err = Function + ": "+ex.Message;
				logException(ex, Function);
			}
			finally
			{
				if(conn.State != ConnectionState.Closed)
					conn.Close();
			}
			return 0;
		}

		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public string GetEodOptionStatus(string configurationName, string optionCode, out string err)		
		{
            Function = "WEODManager::GetEodOptionStatus()";
			err = "";
			string status = "";
		
			try
			{
				BInterfaceControl ic = new BInterfaceControl();
				status = ic.GetEODStatus(configurationName, optionCode);
			}
			catch(Exception ex)
			{
				err = Function + ": "+ex.Message;
				logException(ex, Function);
			}
			
			return status;
		}

		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public DataSet GetEODAdHocScripts(out string err)
		{
			Function = "WEODManager::GetEODAdHocScripts()";
			err = "";
			DataSet ds = null;
		
			try
			{
				BInterfaceControl ic = new BInterfaceControl();
				ds = ic.GetEODAdHocScripts();						
			}
			catch(Exception ex)
			{
				err = Function + ": "+ex.Message;
				logException(ex, Function);
			}
			
			return ds;
		}

		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public int EODStartJob(string configurationName, string country, out string err)
		{
			Function = "WEODManager::EODStartJob()";
			err = "";
		
			try
			{
				BInterfaceControl ic = new BInterfaceControl();
				ic.User = STL.Common.Static.Credential.UserId;
				ic.EODStartJob(configurationName, country);						
			}
			catch(Exception ex)
			{
				err = Function + ": "+ex.Message;
				logException(ex, Function);
			}
			
			return 0;
		}

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int DeleteEODConfiguration(string configurationName, out string err)
        {
            Function = "WEODManager::DeleteEODConfiguration()";
            err = "";

            SqlConnection conn = null;
            
            BInterfaceControl ic = new BInterfaceControl();

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
                            ic.User = STL.Common.Static.Credential.UserId;
                            ic.DeleteEODConfiguration(conn, trans, configurationName);
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
                err = Function + ": " + ex.Message;
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
        public int CheckDiskSpace(string option, out bool enoughSpace, out bool pathError, out string err)
        {
            Function = "WEODManager::CheckDiskSpace()";
            enoughSpace = false;
            pathError = false;
            err = "";

            try
            {
                BInterfaceControl ic = new BInterfaceControl();
                ic.CheckDiskSpace(option, out enoughSpace, out pathError);
            }
            catch (Exception ex)
            {
                err = Function + ": " + ex.Message;
                logException(ex, Function);
            }

            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetEodOptionListDetails(string configurationName, out DateTime startDate, 
                                                out int frequency, out string err)
        {
            Function = "WEODManager::GetEodOptionListDetails()";

            startDate = DateTime.Now;
            frequency = 0;
            err = "";
            DataSet ds = new DataSet();

            try
            {
                BInterfaceControl ic = new BInterfaceControl();
                DataTable dt = ic.GetEodOptionListDetails(configurationName, out startDate,
                                                          out frequency);
                ds.Tables.Add(dt);
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
        public int SetReRunStatus(string configurationName, string status, out string err)
        {
            Function = "WEODManager::SetReRunStatus()";
            err = "";

            SqlConnection conn = null;
            
            BInterfaceControl ic = new BInterfaceControl();

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
                            ic.User = STL.Common.Static.Credential.UserId;
                            ic.SetReRunStatus(conn, trans, configurationName, status);
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
                err = Function + ": " + ex.Message;
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
        public int SaveFACT2000Options(DateTime effDate, string fullProduct, string excludeZeroStock, 
                                    string processEOD, string processEOW, string processEOP, 
                                    string processCINT, out string err)
        {
            Function = "WEODManager::SaveFACT2000Options()";
            err = "";

            SqlConnection conn = null;
            
            BInterfaceControl ic = new BInterfaceControl();

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
                            ic.User = STL.Common.Static.Credential.UserId;
                            ic.SaveFACT2000Options(conn, trans, effDate, fullProduct, excludeZeroStock,
                                            processEOD, processEOW, processEOP, processCINT);
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
                err = Function + ": " + ex.Message;
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
        public void EODSetReRun(string configName, string optionCode, int runNo,out string err)
        {
            Function = "WEODManager::EODSetReRunSP()";
            err = "";            

            try
            {
                EODRepository eod = new EODRepository();

                eod.UpdateConfigurationOption(configName, optionCode, runNo);
            }
            catch (Exception ex)
            {
                err = Function + ": " + ex.Message;
                logException(ex, Function);
            }
            
        }

        //#12156
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool CheckServiceExists(string serviceName, out string err)
        {
            Function = "WEODManager::CheckServiceExists()";
            err = "";

            bool exists = true;

            try
            {
                EODRepository eod = new EODRepository();

                exists = eod.CheckServiceExists(serviceName);

            }
            catch (Exception ex)
            {
                err = Function + ": " + ex.Message;
                logException(ex, Function);
            }
            return exists;
        }

        //#12341 - CR11571
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool CheckToAddSCStatementsOption(out string err)
        {
            Function = "WEODManager::CheckToAddSCStatementsOption()";
            err = "";

            var addJob = false;

            try
            {
                EODRepository eod = new EODRepository();

                addJob = eod.CheckToAddSCStatementsOption();

            }
            catch (Exception ex)
            {
                err = Function + ": " + ex.Message;
                logException(ex, Function);
            }
            return addJob;
        }

        //#12341 - CR11571
        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        //#12341 - CR11571
        public void RemoveOption(string configurationName, string optionCode, out string err)
        {
            Function = "WEODManager::RemoveOption()";
            err = "";

            try
            {
                EODRepository eod = new EODRepository();

                eod.RemoveOption(configurationName, optionCode);

            }
            catch (Exception ex)
            {
                err = Function + ": " + ex.Message;
                logException(ex, Function);
            }
       
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
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion
	}
}

