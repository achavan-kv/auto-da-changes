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
using System.Threading;
using Blue.Cosacs.Shared;
using Blue.Cosacs;

namespace STL.WS
{
	/// <summary>
	/// Summary description for WSystemConfig.
	/// </summary>
	/// 
	[WebService(Namespace="http://strategicthought.com/webservices/")]
	public class WSystemConfig : CommonService
	{
		public WSystemConfig()
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
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public DataSet GetFunctionsForType(string userType,out string err)
		{
			Function = "WSystemConfig::GetFunctionsForType()";
			DataSet ds = null;
			err = "";

			try
			{
				BUser user = new BUser();
				ds = user.GetFunctionsForType(userType);
				
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
		public int UpdateFunctionsForRole(string userType, string[] functions, out string err)
		{
			Function = "WSystemConfig::UpdateFunctionsForRole()";
		
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

                            BUser user = new BUser();
                            var Log = new ArrayList();
                            Log.Add("UserType = " + userType);
                            foreach (string function in functions )
                                Log.Add(function);
                            
                            //Log.Add(functions);


                            EventStore.Instance.Log(Log, "RoleMaintenance", EventCategory.SystemMaintenance
                                        , new { empeeno = user });
                            user.UpdateFunctionsForRole(conn, trans, userType, functions);

                            trans.Commit();		//commit the transaction
                        }
						break;
					}
					catch(SqlException ex)
					{
						CatchDeadlock(ex, conn);
					}
				}while(retries<=maxRetries);
			}
			catch(Exception ex)
			{
				err = Function + ":  " + ex.Message;
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
        public TallymanExtractResponse TallymanExtract(TallymanExtractRequest tallyRequest)
        {
            string err;
            TallymanExtractResponse response;
            using (SqlConnection conn = new SqlConnection(Connections.Default))
            {

                //				try
                //				{
                conn.Open();
                //trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                response = BTallymanExtract.Run(conn, null, tallyRequest);
                //				}
                //				catch(SqlException ex)
                //				{
                //					err = Function + ":  " + ex.Message;
                //					logException(ex, Function);
                //					trans.Rollback();
                //				}
            }
            return response;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public TallymanImportResponse TallymanSegmentImport()
        {
            using (SqlConnection conn = new SqlConnection(Connections.Default))
            {
                conn.Open();
                //SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                return BTallymanExtract.RunImport(conn, null);
            }
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public bool RunJobs(string[] jobNameList)
        {
            using (SqlConnection conn = new SqlConnection(Connections.Default))
            {
                conn.Open();
                //SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                return BTallymanExtract.RunJobs(conn, null, jobNameList);
            }
        }



        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public MonitorResponse MonitorJobStatus(string jobName)
        {
            using (SqlConnection conn = new SqlConnection(Connections.Default))
            {
                conn.Open();
                //SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                return BTallymanExtract.MonitorJobStatus(conn, null, jobName);
            }
        }
	}
}
