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
using System.Xml;

namespace STL.WS
{
	/// <summary>
	/// Summary description for WCommissionPayment.
	/// </summary>
	public partial class WCommissionPayment : CommonWebPage
	{
		int index = 1;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				/* Extract deliveryNotes document from the QueryString */
				string commPaymentXmlStr = Request["commissionPayment"];
				string countryCode = Request["countryCode"];
				short rePrint = Convert.ToInt16(Request["rePrint"]);
				
				if(!Convert.ToBoolean(rePrint))
				{
					CommissionPaymentXML cxml = new CommissionPaymentXML(countryCode, false);
					cxml.Load(commPaymentXmlStr);
					Response.Write(cxml.Transform());		
				}
				else
				{
					XmlDocument commnPayments = new XmlDocument();
					commnPayments.LoadXml(commPaymentXmlStr);

					CommissionPaymentXML cxml = new CommissionPaymentXML(countryCode, true);
					cxml.Load("<COMMISSIONS/>");						

					foreach(XmlNode commn in commnPayments.DocumentElement.ChildNodes)
					{
						XmlDocument doc = new XmlDocument();
						doc.LoadXml(commn.OuterXml);

						string xPath = "COMMISSION/LAST";
						XmlNode lastNode = doc.SelectSingleNode(xPath);

						if(index == commnPayments.DocumentElement.ChildNodes.Count)
						{
							if(lastNode!=null)
								lastNode.InnerText = "TRUE";
						}
						else
						{
							index++;
							if(lastNode!=null)
								lastNode.InnerText = "FALSE";
						}


						cxml.ImportNode(doc.DocumentElement, true);
					}

					Response.Write(cxml.Transform());		
				}
				
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
