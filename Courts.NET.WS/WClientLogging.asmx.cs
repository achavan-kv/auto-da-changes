using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using STL.DAL;
using STL.Common;
using System.Web.Services.Protocols;

namespace STL.WS
{
	/// <summary>
	/// Summary description for BClientLogging.
	/// </summary>
	/// 
	[WebService(Namespace="http://strategicthought.com/webservices/")]
	public class WClientLogging : CommonService
	{
		private Logging log;

		public WClientLogging()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		[WebMethod]	
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public void logMessage(string message, string user, EventLogEntryType type)
		{
			log = new Logging();
			log.logMessage(message, user, type);
		}

		[WebMethod]
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public void logMessages(string[] message, string user, EventLogEntryType type)
		{
			log = new Logging();
			log.logMessage(message, user, type);
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
		protected override void Dispose( bool disposing )
		{
		}
	}
}
