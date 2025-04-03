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
using STL.DAL;
using STL.Common;
using STL.Common.Constants.TableNames;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using System.Globalization;
using STL.Common.Printing.AgreementPrinting;
using System.Diagnostics;


namespace STL.WS
{
	/// <summary>
	/// Summary description for WDeliverySchedule.
	/// </summary>
	public partial class WDeliverySchedule : CommonWebPage
	{

		short loadNo = 0;
		short branchNo = 0;
		DateTime dateDel = DateTime.Now;
		string truckID = "";
		string driver = "";
		short printed = 0;
		string culture = "";
		int index = 1;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			SqlConnection conn = null; 
			SqlTransaction trans = null;
			
			try
			{
				conn = new SqlConnection(Connections.Default);
				do
				{
					try
					{
						conn.Open();
						trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
						XmlDocument deliverySchedules = new XmlDocument();
						
						/* Extract deliveryNotes document from the QueryString */
						string deliverySchedsXmlStr = Request["deliverySchedules"];
						string countryCode = Request["countryCode"];
						deliverySchedules.LoadXml(deliverySchedsXmlStr);

						//Build the XML document
						DeliveryScheduleXML dxml = new DeliveryScheduleXML(countryCode);
						dxml.Load("<DELIVERYSCHEDULES/>");						

						foreach(XmlNode delSchedule in deliverySchedules.DocumentElement.ChildNodes)
						{

							loadNo = Convert.ToInt16(delSchedule.SelectSingleNode("LOADNO").InnerText);	
							branchNo = Convert.ToInt16(delSchedule.SelectSingleNode("BRANCHNO").InnerText);
							truckID = delSchedule.SelectSingleNode("TRUCKID").InnerText;
							driver = delSchedule.SelectSingleNode("DRIVERNAME").InnerText;
							printed = Convert.ToInt16(delSchedule.SelectSingleNode("PRINTED").InnerText);
							culture = delSchedule.SelectSingleNode("CULTURE").InnerText;

							//Set the culture for currency formatting
							//Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                            base.SetCulture();

							dateDel = Convert.ToDateTime(delSchedule.SelectSingleNode("DATEDEL").InnerText);

							XmlDocument doc = null;
							DDelivery del = new DDelivery();
							DataTable dt = del.GetDeliveryScheduleCustomerDetails(conn, trans, loadNo, branchNo, dateDel);
							doc = LoadDocument(conn, trans, dt);

							string xPath = "DELIVERYSCHEDULE/LAST";
							XmlNode lastNode = doc.SelectSingleNode(xPath);

							if(index==deliverySchedules.DocumentElement.ChildNodes.Count)
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

							if(!Convert.ToBoolean(printed))
							{
								DSchedule sched =  new DSchedule();
								sched.SetTransportSchedulePrinted(conn, trans, loadNo, branchNo, dateDel);
							}
								
							/* import this delivery note into the main document */
							dxml.ImportNode(doc.DocumentElement, true);							
						}

//#if(XMLTRACE)
//						logMessage(dxml.Xml, User.Identity.Name, EventLogEntryType.Information);
//#endif
						Response.Write(dxml.Transform());
						trans.Commit();
						break;
					}
					catch(SqlException ex)
					{
						CatchDeadlock(ex, conn);
					}
				}while (retries <= maxRetries);				
			}
			catch(Exception ex)
			{
				logException(ex, Function);
				Response.Write(ex.Message);
			}
			finally
			{
				if(conn.State != ConnectionState.Closed)
					conn.Close();
			}
		}

		private XmlDocument LoadDocument(SqlConnection conn, SqlTransaction trans, DataTable dt)
		{
			int numItems = 0;
			decimal totalValue = 0;

			DSchedule sched =  new DSchedule();

			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<DELIVERYSCHEDULE/>");

			XmlNode header = doc.CreateElement("HEADER");
			doc.DocumentElement.AppendChild(header);
			XmlNode load = doc.CreateElement("LOADNO");
			XmlNode branch = doc.CreateElement("BRANCH");
			XmlNode deliveryDate = doc.CreateElement("DELIVERYDATE");
			XmlNode truck = doc.CreateElement("TRUCKID");
			XmlNode driverName = doc.CreateElement("DRIVERNAME");
			XmlNode printText = doc.CreateElement("PRINTTEXT");
			load.InnerText = loadNo.ToString();
			branch.InnerText = branchNo.ToString();
			deliveryDate.InnerText = dateDel.ToShortDateString();
			truck.InnerText = truckID;
			driverName.InnerText = driver;

			if(Convert.ToBoolean(printed))
				printText.InnerText = GetResource("T_REPRINT");
			else
				printText.InnerText = "";

			header.AppendChild(load);
			header.AppendChild(branch);
			header.AppendChild(deliveryDate);
			header.AppendChild(truck);
			header.AppendChild(driverName);
			header.AppendChild(printText);

			XmlNode customers = doc.CreateElement("CUSTOMERS");
			doc.DocumentElement.AppendChild(customers);

			foreach(DataRow row in dt.Rows)
			{
				XmlNode customer = doc.CreateElement("CUSTOMER");
				customers.AppendChild(customer);
                XmlNode locn = doc.CreateElement("LOCN");           //#3467 jec 07/04/11
				XmlNode buffNo = doc.CreateElement("BUFFNO");
				XmlNode customerName = doc.CreateElement("NAME");
				XmlNode acctNo = doc.CreateElement("ACCTNO");
                locn.InnerText = row[CN.BuffBranchNo].ToString();       //#3467 jec 07/04/11
                buffNo.InnerText = row[CN.BuffNo].ToString();           //#3467 jec 07/04/11
				//buffNo.InnerText = row[CN.BuffBranchNo].ToString() + "           " + row[CN.BuffNo].ToString();
				customerName.InnerText = row[CN.CustomerName].ToString();
				acctNo.InnerText = row[CN.AcctNo].ToString();
                customer.AppendChild(locn);                         //#3467 jec 07/04/11
				customer.AppendChild(buffNo);
				customer.AppendChild(customerName);
				customer.AppendChild(acctNo);

				XmlNode lineitems = doc.CreateElement("LINEITEMS");
				customer.AppendChild(lineitems);

				//XmlNode lineitem = doc.CreateElement("LINEITEM");
				//lineitems.AppendChild(lineitem);
			
				DataTable items = sched.DeliveryScheduleGetItems(conn, trans, 
													Convert.ToInt16(row[CN.BuffBranchNo]),Convert.ToInt32(row[CN.BuffNo]));
				foreach(DataRow r in items.Rows)
				{
					numItems++;
					XmlNode lineitem = doc.CreateElement("LINEITEM");
					XmlNode qty = doc.CreateElement("QUANTITY");
					XmlNode itemNo = doc.CreateElement("ITEMNO");
					XmlNode desc = doc.CreateElement("DESC");
					XmlNode price = doc.CreateElement("PRICE");

					decimal lineQty = Convert.ToDecimal(r[CN.Quantity]);

					qty.InnerText = lineQty.ToString("F2");
                    //itemNo.InnerText = r[CN.ItemNo].ToString();

                    //IP - 21/09/11 - RI - #8222 - CR8201
                    if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")
                    {
                        itemNo.InnerText = r[CN.ItemNo].ToString() + " " + r[CN.CourtsCode].ToString();
                        desc.InnerText = r[CN.Description].ToString() + " " + r[CN.Brand].ToString() + " " + r[CN.Style].ToString();
                    }
                    else
                    {
                        itemNo.InnerText = r[CN.ItemNo].ToString();
                        desc.InnerText = r[CN.Description].ToString();
                    }
					
                    price.InnerText = Convert.ToDecimal(lineQty * Convert.ToDecimal(r[CN.Price])).ToString("F2");   //#3467 jec 07/04/11 2 decimal

					lineitem.AppendChild(qty);
					lineitem.AppendChild(itemNo);
					lineitem.AppendChild(desc);
					lineitem.AppendChild(price);
					lineitems.AppendChild(lineitem);

					if(numItems==STL.Common.Printing.AgreementPrinting.Document.DeliveryScheduleItems)
					{
						XmlNode pb = doc.CreateElement("PB");
						lineitem.AppendChild(pb);
						numItems = 0;
					}

					totalValue  += Convert.ToDecimal(lineQty * Convert.ToDecimal(r[CN.Price]));
				}
			}

			XmlNode last = doc.CreateElement("LAST");
			doc.DocumentElement.AppendChild(last);

			XmlNode footer = doc.CreateElement("FOOTER");
			doc.DocumentElement.AppendChild(footer);
			XmlNode total = doc.CreateElement("TOTAL");
            total.InnerText = totalValue.ToString("F2");        //#3467 jec 07/04/11 2 decimal

			footer.AppendChild(total);

			return doc;
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
