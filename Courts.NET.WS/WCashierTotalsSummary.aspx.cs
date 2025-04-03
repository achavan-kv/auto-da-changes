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
using STL.Common;
using STL.BLL;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.DAL;
using System.Xml;

namespace STL.WS
{
	/// <summary>
	/// Summary description for WCashierTotalsSummary.
	/// </summary>
	public partial class WCashierTotalsSummary : CommonWebPage
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			DataSet totals = null;
			CashierTotalsSummaryXML cxml = null;
			decimal systemTotal = 0; 
			decimal userTotal = 0;
			decimal depositTotal = 0;
			decimal differenceTotal = 0;
			decimal securitisedTotal = 0;

			try
			{
				short branchNo = Convert.ToInt16(Request[CN.BranchNo]);
				string culture = Request[CN.Culture];

				cxml = new CashierTotalsSummaryXML((string)Country[CountryParameterNames.CountryCode]);

				//Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                base.SetCulture();

				cxml.Load();

				XmlUtilities xml = new XmlUtilities();
				XmlDocument dropDowns = new XmlDocument();
				dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.PayMethod, new string[]{"FPM", "L"}));
				BDropDown drop = new BDropDown();
				DataSet pm = drop.GetDropDownData(dropDowns.DocumentElement);

				/* date time conversion must be moved after the culture has been set */
				DateTime dateFrom = Convert.ToDateTime(Request[CN.DateFrom]);
				DateTime dateTo = Convert.ToDateTime(Request[CN.DateTo]);

				BPayment p = new BPayment();
				totals = p.GetCashierTotalsSummary(branchNo, dateFrom, dateTo);

				if(branchNo==-1)
					cxml.SetNode("CASHIERTOTALS/BRANCH", GetResource("M_ALLBRANCHES"));
				else
					cxml.SetNode("CASHIERTOTALS/BRANCH", branchNo.ToString());
				cxml.SetNode("CASHIERTOTALS/DATEFROM", dateFrom.ToString());
				cxml.SetNode("CASHIERTOTALS/DATETO", dateTo.ToString());

				cxml.AddPayMethods(totals,  
									pm,
									ref systemTotal, 
									ref userTotal,
									ref depositTotal, 
									ref differenceTotal,
									ref securitisedTotal);

				cxml.SetNode("CASHIERTOTALS/SYSTEMTOTAL", systemTotal.ToString(DecimalPlaces));
				cxml.SetNode("CASHIERTOTALS/USERTOTAL", userTotal.ToString(DecimalPlaces));
				cxml.SetNode("CASHIERTOTALS/DEPOSITTOTAL", depositTotal.ToString(DecimalPlaces));
				cxml.SetNode("CASHIERTOTALS/DIFFERENCETOTAL", differenceTotal.ToString(DecimalPlaces));
				cxml.SetNode("CASHIERTOTALS/SECURITISEDTOTAL", securitisedTotal.ToString(DecimalPlaces));

//#if(XMLTRACE)
//				logMessage(cxml.Xml, User.Identity.Name, EventLogEntryType.Information);
//#endif
				Response.Write(cxml.Transform());                
				
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
