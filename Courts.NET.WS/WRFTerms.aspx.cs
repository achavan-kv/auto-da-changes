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
using STL.Common.Constants.ColumnNames;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using System.Globalization;
using STL.Common.Printing.AgreementPrinting;
using STL.Common.Constants.Elements;
using STL.Common.Constants.AccountTypes;
using Blue.Cosacs.Repositories;

namespace STL.WS
{
	/// <summary>
	/// Summary description for WRFTerms.
	/// </summary>
	public partial class WRFTerms : CommonWebPage
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				decimal limit = 0;
				decimal available = 0;
				bool wrongType = false;

				string customerID = Request[CN.CustomerID];
				string accountNo = Request[CN.AccountNo];
				string culture = Request[CN.Culture];
				string countryCode = Request["countryCode"];
                string storeType = Request["storeType"];        //#19687

				//Set the culture for currency formatting
				//Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                base.SetCulture();

                BranchRepository br = new BranchRepository();
                string acctNo = accountNo.Substring(0, 3);
                var nonCourtsStoreTypes = br.GetNonCourtsStoreType(Convert.ToInt16(acctNo));

                RFTermsXML rfxml = new RFTermsXML(countryCode, storeType, nonCourtsStoreTypes.AshleyStore, nonCourtsStoreTypes.LuckyDollarStore, nonCourtsStoreTypes.RadioShackStore);          //#19687
				rfxml.Load();

				//retrieve all the requied data
				BCustomer customer = new BCustomer();
				DataSet custDetails = customer.GetBasicCustomerDetails(null, null, customerID, accountNo, "H");
				customer.GetRFLimit(customerID, "", "R", out limit, out available, out wrongType);

				rfxml.SetNode("RFTERMS/ACCOUNTNO", accountNo);
				rfxml.SetNode("RFTERMS/VALID", DateTime.Today.ToShortDateString());
				rfxml.SetNode("RFTERMS/LIMIT", limit.ToString("C2"));
				foreach(DataTable dt in custDetails.Tables)
				{
					switch(dt.TableName)
					{
						case "BasicDetails":	
							LoadCustDetails(dt, rfxml);
							break;
						case TN.CustomerAddresses:		
							LoadAddressDetails(dt, rfxml);
							break;
						default:
							break;
					}
				}

				Response.Write(rfxml.Transform());

			}
			catch(Exception ex)
			{
				logException(ex, Function);
				Response.Write(ex.Message);
			}
		}

		private void LoadCustDetails(DataTable dt, RFTermsXML rfxml)
		{
			string name = "";
			foreach(DataRow row in dt.Rows)
			{
				name = (string)row[CN.Title] + " " +
						(string)row[CN.FirstName] + " " +
						(string)row[CN.LastName];
				rfxml.SetNode("RFTERMS/NAME", name);
			}
		}

		private void LoadAddressDetails(DataTable dt, RFTermsXML rfxml)
		{
			string addType = "";
			foreach(DataRow row in dt.Rows)
			{
				addType = ((string)row["AddressType"]).Trim();
				switch(addType)
				{
					case "H":	rfxml.SetNode("RFTERMS/ADDRESS1", (string)row["Address1"], false);
						rfxml.SetNode("RFTERMS/ADDRESS2", (string)row["Address2"], false);
						rfxml.SetNode("RFTERMS/ADDRESS3", (string)row["Address3"], false);
						rfxml.SetNode("RFTERMS/POSTCODE", (string)row["PostCode"], false);
						break;
					default:
						break;
				}
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
