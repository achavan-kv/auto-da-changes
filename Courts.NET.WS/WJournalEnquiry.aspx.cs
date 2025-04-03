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
using System.Xml;
using STL.Common.Constants.ColumnNames;
using System.Configuration;
using STL.Common.Constants.Tags;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.Elements;


namespace STL.WS
{
	/// <summary>
	/// Summary description for JournalEnquiry.
	/// </summary>
	public partial class WJournalEnquiry : CommonWebPage
	{
		JournalEnquiryXML jxml = null;
		DataSet ds = null;
		XmlNode pageNode = null;
		XmlNode headerNode = null;
		XmlNode transItems = null;
		XmlNode transItem = null;
		XmlNode totalItems = null;
		XmlNode totalItem = null;
		int transPerPage = 40;
		double numPages = 0;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				int firstRef = Convert.ToInt32(Request["firstRef"]);
				int lastRef = Convert.ToInt32(Request["lastRef"]);
				int empNo = Convert.ToInt32(Request["empNo"]);
				int branch = Convert.ToInt32(Request["branch"]);
				int combination = Convert.ToInt32(Request["combination"]);
				string culture = Request["culture"];
				string countryCode = Request["countryCode"];

				jxml = new JournalEnquiryXML(countryCode);

				//Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                base.SetCulture();

				DateTime dateFirst = Convert.ToDateTime(Request["dateFirst"]);
				DateTime dateLast = Convert.ToDateTime(Request["dateLast"]);

				jxml.Load();

				BTransaction trans = new BTransaction();
				ds = trans.JournalEnquiryGet(dateFirst, dateLast, firstRef, lastRef, empNo, branch, combination);

				headerNode = jxml.GetNode("JOURNALENQUIRY/PAGE/HEADER");
				pageNode = jxml.GetNode("JOURNALENQUIRY/PAGE");

				headerNode.SelectSingleNode("DATE").InnerText = DateTime.Today.ToShortDateString();

				transItems = jxml.GetNode("JOURNALENQUIRY/PAGE/TRANSACTIONS");
				transItem = transItems.RemoveChild(transItems.SelectSingleNode("TRANSACTION"));
				AddTransactions(jxml, ds);

				totalItems = jxml.GetNode("JOURNALENQUIRY/TOTALS");
				totalItem = totalItems.RemoveChild(totalItems.SelectSingleNode("TOTAL"));
				AddTotals(jxml, ds);
				
				//logMessage(jxml.Xml,"debug",EventLogEntryType.Warning);
				Response.Write(jxml.Transform());                
			}
			catch(Exception ex)
			{

				logException(ex, Function);
				Response.Write(ex.Message);
			}
		}

		private void AddTransactions(JournalEnquiryXML jxml, 
						DataSet ds)
		{
			int i=0;
			int pageNum = 1;

			numPages = Math.Ceiling(Convert.ToDouble(ds.Tables[0].Rows.Count) / Convert.ToDouble(transPerPage));
			if(numPages<=1)
				pageNode.SelectSingleNode("LAST").InnerText = Boolean.TrueString;
			else
				pageNode.SelectSingleNode("LAST").InnerText = Boolean.FalseString;

			logMessage(jxml.Xml,"debug - 01",EventLogEntryType.Warning);

			foreach(DataRow r in ds.Tables[0].Rows)
			{
				XmlNode trans = transItem.CloneNode(true);

				trans.SelectSingleNode("BRANCH").InnerText = Convert.ToString(r[CN.BranchNo]);
				trans.SelectSingleNode("EMPLOYEE").InnerText = Convert.ToString(r[CN.EmployeeNo]);
				trans.SelectSingleNode("DATETRANS").InnerText = ((DateTime)r[CN.DateTrans]).ToShortDateString();;
				trans.SelectSingleNode("REFNO").InnerText = Convert.ToString(r[CN.TransRefNo]);
				trans.SelectSingleNode("ACCTNO").InnerText = Convert.ToString(r[CN.AcctNo]);
				trans.SelectSingleNode("TRANSTYPE").InnerText = Convert.ToString(r[CN.TransTypeCode]);
				trans.SelectSingleNode("TRANSVALUE").InnerText = ((decimal)r[CN.TransValue]).ToString(DecimalPlaces);
				
				transItems.AppendChild(trans);

				if(++i >= transPerPage)
				{
					i=0;
					pageNum++;
					XmlNode newPage = pageNode.CloneNode(false);
					newPage.AppendChild(headerNode.CloneNode(true));
					transItems = transItems.CloneNode(false);
					newPage.AppendChild(transItems);
					newPage.AppendChild(newPage.OwnerDocument.CreateElement("LAST"));
					newPage.SelectSingleNode("LAST").InnerText = pageNum == numPages ? Boolean.TrueString : Boolean.FalseString;
					jxml.GetNode("JOURNALENQUIRY").AppendChild(newPage);
				}
			}
		}

		private void AddTotals(JournalEnquiryXML jxml, 
							   DataSet ds)
		{
			foreach(DataRow r in ds.Tables[1].Rows)
			{
				XmlNode tots = totalItem.CloneNode(true);

				tots.SelectSingleNode("TRANSTYPE").InnerText = Convert.ToString(r[CN.TransTypeCode]);
				tots.SelectSingleNode("TRANSVALUE").InnerText = ((decimal)r[CN.TransValue]).ToString(DecimalPlaces);
				
				totalItems.AppendChild(tots);
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
