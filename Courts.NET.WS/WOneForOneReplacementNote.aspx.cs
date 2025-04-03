using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using STL.Common;
using STL.Common.Printing.AgreementPrinting;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Elements;


namespace STL.WS
{
	/// <summary>
	/// Summary description for WOneForOneReplacementNote.
	/// </summary>
	public partial class WOneForOneReplacementNote : CommonWebPage
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				//Extract variables from the QueryString
				string acctno = Request[CN.acctno];
				string itemNo = Request[CN.ItemNo];

				string productDescription = Request[CN.ItemDescr1];
				string reason = Request[Elements.Reason];
				string oneForOneTimePeriod = Request[Elements.OneForOneTimePeriod];
				DateTime dateReturn = DateTime.Today; 

				int branch = Convert.ToInt32(Request[CN.BranchNo]);	
				int user = Convert.ToInt32(Request["user"]);
				string userName = Request["userName"];
				string culture = Request["culture"];
				string notes = Request[CN.Notes];
				string countryCode = Request["countryCode"];

				//Set the culture for currency formatting
				//Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                base.SetCulture();

				//Build the XML document
				OneForOneReplacementNoteXML xml = new OneForOneReplacementNoteXML(countryCode);
				xml.Load();
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/ACCTNO", acctno);
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/BRANCHNO", branch.ToString());
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/USER", user.ToString()+" ");
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/USERNAME", userName);
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/PRINTED", DateTime.Now.ToString());
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/NOTES", notes);

				xml.SetNode("ONEFORONEREPLACEMENTNOTE/PRODUCTDETAILS/PRODUCTDESCRIPTION", productDescription);
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/PRODUCTDETAILS/REASON", reason);
                //xml.SetNode("ONEFORONEREPLACEMENTNOTE/PRODUCTDETAILS/ONEFORONETIMEPERIOD", oneForOneTimePeriod);  //#17290
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/PRODUCTDETAILS/DATERETURN", dateReturn.ToString());
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/PRODUCTDETAILS/ITEMNO", itemNo);

				Response.Write(xml.Transform());
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

