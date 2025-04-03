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

namespace STL.WS
{
	/// <summary>
	/// Summary description for WCashierTotals.
	/// </summary>
	public partial class WCashierTotals : CommonWebPage
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			DataSet totals = null;
			DataSet ds = null;
			decimal total = 0;
			CashierTotalsXML cxml = null;
			bool history = true;

			try
			{
				short branchNo = Convert.ToInt16(Request["branchNo"]);
				int employeeNo = Convert.ToInt32(Request["employeeNo"]);
				bool subTotal = Convert.ToBoolean(Request["subTotal"]);
				string culture = Request["culture"];
				string countryCode = Request["countryCode"];
				bool listCheques = Convert.ToBoolean(Request["listCheques"]);
				string employeeName = Request["employeeName"];
				int cashierID = Convert.ToInt32(Request["cashierID"]);

				cxml = new CashierTotalsXML(countryCode);

				//Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                base.SetCulture();

				cxml.Load();

				/* date time conversion must be moved after the culture has been set */
				DateTime dateFrom = Convert.ToDateTime(Request["dateFrom"]);
				DateTime dateTo = Convert.ToDateTime(Request["dateTo"]);

				BPayment p = new BPayment();
				
				employeeName = employeeName + "(" + employeeNo.ToString() + ")";

				totals = p.GetCashierTotalsForPrint(branchNo, employeeNo, dateFrom, dateTo, listCheques, out total);
				
				if(cashierID == -1)
				{
					ds = p.GetBreakDownForEmployee(employeeNo, dateFrom, dateTo);
					history = false;
				}
				else
				{
					ds = p.GetCashierTotalsBreakdown(cashierID, true);
					history = true;
				}

				cxml.SetNode("CASHIERTOTALS/BRANCH", branchNo.ToString());
				cxml.SetNode("CASHIERTOTALS/EMPLOYEE", employeeName);
				cxml.SetNode("CASHIERTOTALS/DATEFROM", dateFrom.ToString());
				cxml.SetNode("CASHIERTOTALS/DATETO", dateTo.ToString());
				cxml.SetNode("CASHIERTOTALS/TOTALVALUE", total.ToString(DecimalPlaces));
				cxml.SetNode("CASHIERTOTALS/SUBTOTAL", subTotal.ToString());
				cxml.SetNode("CASHIERTOTALS/EMPLOYEENAME", employeeName);
				cxml.AddValues(ds, history);
				cxml.AddTransactions(totals, DecimalPlaces, listCheques);

				if(employeeNo==-1)
					cxml.SetXsltPath("CashierTotalsByBranch.xslt");
				else
				{
					if(listCheques)
						cxml.SetXsltPath("CashierChequeTotals.xslt");
					else
						cxml.SetXsltPath("CashierTotalsByEmployee.xslt");
				}

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
