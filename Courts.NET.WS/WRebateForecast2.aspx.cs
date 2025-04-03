using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using STL.BLL;
using STL.Common;
using STL.Common.Constants.TableNames;
using System.Xml;
using System.Data.SqlClient;
using System.Configuration;
using STL.Common.Constants.ColumnNames;
using System.Threading;
using System.Globalization;
using System.Diagnostics;

namespace STL.WS
{
	/// <summary>
	/// Summary description for WCollectionNote.
	/// </summary>
	public partial class WRebateForecast2 : CommonWebPage
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				/* Extract deliveryNotes document from the QueryString */
				string rebateReportXmlStr = Request["rebateReport"];
				string countryCode = Request["countryCode"];
				
				//Build the XML document
				RebateReport2XML rxml = new RebateReport2XML(countryCode);
				rxml.Load(rebateReportXmlStr);		
				
				Response.Write(rxml.Transform());		
			}
			catch(Exception ex)
			{
				logException(ex, Function);
				Response.Write(ex.Message);
			}
		}
	
		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}
