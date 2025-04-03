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
	/// Summary description for WFaultyProductNote.
	/// </summary>
	public partial class WFaultyProductNote : CommonWebPage
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				//Extract variables from the QueryString
				string acctno = Request[CN.acctno];
				string name = Request[CN.name];
				string firstName = Request[CN.FirstName];
				string title = Request[CN.Title];
				string address1 = Request[CN.cusaddr1];
				string address2 = Request[CN.cusaddr2];
				string address3 = Request[CN.cusaddr3];
				string postCode = Request[CN.cuspocode];
				string custNotes = Request[CN.cusnotes];
				string custID = Request[CN.CustID];
				string itemNo = Request[CN.ItemNo];

				string productDescription = Request[Elements.ProductDescription];
				string reason = Request[Elements.Reason];
				string oneForOneTimePeriod = Request[Elements.OneForOneTimePeriod];
				DateTime dateReturn = DateTime.Today; //DateTime.Parse(Request[Tags.DateReturn]);

				int branch = Convert.ToInt32(Request["branch"]);	
				int user = Convert.ToInt32(Request["user"]);
				string userName = Request["userName"];
				string culture = Request["culture"];
				string countryCode = Request["countryCode"];

				//Set the culture for currency formatting
				//Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                base.SetCulture();


				//Build the XML document
				OneForOneReplacementNoteXML xml = new OneForOneReplacementNoteXML(countryCode);
				xml.Load();
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/ADDRESS1", address1);
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/ADDRESS2", address2);
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/ADDRESS3", address3);
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/POSTCODE", postCode);
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/ACCTNO", acctno);
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/FIRSTNAME", firstName+" ");
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/LASTNAME", name);
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/CUSTNOTES", custNotes);
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/BRANCHNO", branch.ToString());
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/USER", user.ToString()+" ");
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/USERNAME", userName);
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/PRINTED", DateTime.Now.ToString());
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/HEADER/CUSTID", custID);

				xml.SetNode("ONEFORONEREPLACEMENTNOTE/PRODUCTDETAILS/PRODUCTDESCRIPTION", productDescription);
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/PRODUCTDETAILS/REASON", reason);
				xml.SetNode("ONEFORONEREPLACEMENTNOTE/PRODUCTDETAILS/ONEFORONETIMEPERIOD", oneForOneTimePeriod);
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

