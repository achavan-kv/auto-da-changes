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
	/// Summary description for WDeliveryNote2.
	/// </summary>
	public partial class WDeliveryNote2 : CommonWebPage
	{
		private string address1 = "";
		private string delAddressType = "";
		private string homeAddrType = "H";
		private DataTable addrTable = null;
        string custID = "";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			//SqlConnection conn = null; 
			SqlTransaction trans;
            
            BDelivery deliveryAudit = new BDelivery();
            DDelivery del = new DDelivery();
            DCustomer cust = new DCustomer();
            DSchedule sched = new DSchedule();
            BLogin login = new BLogin();
            
            string userName = "";
			string printedByName = "";
			int user = 0;
			string delDate = "";
			//string relationship = "H";
			double qty = 0;
			int index = 1; 
			decimal amountPayable = 0;
			decimal charges = 0;
			bool cod = false;
			short loadNo = 0;
			short branchNo = 0;
			DateTime dateDel = DateTime.Now;
			string truckID = "";
			short printed = 0;
			string culture = "";
			int locn = 0;
			int printedBy = 0;

			XmlDocument deliveryNotes = new XmlDocument();

			try
			{
				do
				{
               using (SqlConnection conn = new SqlConnection(Connections.Default))
               {
                  try
                  {
                     conn.Open();
                     
                     /* Extract deliveryNotes document from the QueryString */
                     string deliveryNotesXmlStr = Request["deliveryNotes"];
                     string countryCode = Request["countryCode"];
                     deliveryNotes.LoadXml(deliveryNotesXmlStr);

                     //Build the XML document
                     DeliveryNoteXML dxml = new DeliveryNoteXML(countryCode);
                     dxml.Load("<DELIVERYNOTES/>");

                     foreach (XmlNode deliveryNote in deliveryNotes.DocumentElement.ChildNodes)
                     {
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                        loadNo = Convert.ToInt16(deliveryNote.SelectSingleNode("LOADNO").InnerText);
                        branchNo = Convert.ToInt16(deliveryNote.SelectSingleNode("BRANCHNO").InnerText);
                        truckID = deliveryNote.SelectSingleNode("TRUCKID").InnerText;
                        printed = Convert.ToInt16(deliveryNote.SelectSingleNode("PRINTED").InnerText);
                        printedBy = Convert.ToInt32(deliveryNote.SelectSingleNode("USER").InnerText);
                        culture = deliveryNote.SelectSingleNode("CULTURE").InnerText;

                        //Set the culture for currency formatting
                        //Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                        base.SetCulture();

                        dateDel = Convert.ToDateTime(deliveryNote.SelectSingleNode("DATEDEL").InnerText);

                        //del = new DDelivery();
                        DataTable accounts = del.GetDeliveryScheduleCustomerDetails(conn, trans, loadNo, branchNo, dateDel);

                        trans.Commit();

                        //IP - 04/08/08 - UAT5.1 - UAT(508)- Only want to print the Delivery Notes
                        //that have not been printed already (prevent 'Immediate Delivery Notes' already printed
                        // but now added to a load from being printed again).
                        string filter = "DatePrinted is NULL"; 
                        //DataRow[] allAccountRows = accounts.Select();
                        DataRow[] allAccountRows = accounts.Select(filter); //IP - 04/08/08 - UAT5.1 - UAT(508) - Filter applied

                        foreach (DataRow row in allAccountRows)
                        {
                           
                           trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                           DeliveryNoteXML delNote = dxml.CreateDeliveryNote(countryCode);

                           if (((string)row[CN.Alias]).Length > 0)
                           {
                              delNote.SetNode("DELIVERYNOTE/HEADER/ALIAS", "ALIAS: " + (string)row[CN.Alias]);
                           }

                           delNote.SetNode("DELIVERYNOTE/HEADER/CUSTOMERNAME", (string)row[CN.CustomerName]);
                           custID = (string)row[CN.CustID];

                           delNote.SetNode("DELIVERYNOTE/HEADER/ACCTNO", (string)row[CN.AcctNo]);

                           //cust = new DCustomer();
                           cust.GetCustomerAddresses(conn, trans, custID);
                           LoadAddressDetails(cust.Addresses, delNote, homeAddrType);
                           
                           addrTable = cust.Addresses;

                           //retrieve the rest of the required data
                           //sched = new DSchedule();
                           sched.AccountNumber = row[CN.AcctNo].ToString().Replace("-", String.Empty); //IP - 10/02/10 - CR1048 (Ref:3.1.3) Merged - Malaysia Enhancements (CR1072)

                           //get lineitems to be printed
                           sched.GetDeliveryNotes(conn, trans, Convert.ToInt32(row[CN.BuffNo]),
                                                  Convert.ToInt32(row[CN.BuffBranchNo]));

                           DataTable dtSchedules = sched.Schedules;
                           if (dtSchedules.Rows.Count > 0)
                           {
                              del.GetCODCharges(conn, trans, (string)row[CN.AcctNo], Convert.ToInt32(row[CN.BuffNo]),
                                                1, out amountPayable, out charges, out  cod,
                                                Convert.ToDateTime(dtSchedules.Rows[0][CN.DateReqDel]),
                                                (string)dtSchedules.Rows[0][CN.TimeReqDel],
                                                (string)dtSchedules.Rows[0][CN.DeliveryAddress],
                                                Convert.ToInt32(row[CN.BuffBranchNo]));
                           }

                           delNote.SetNode("DELIVERYNOTE/FOOTER/ADDCHARGES", charges.ToString("F2"));
                           delNote.SetNode("DELIVERYNOTE/FOOTER/PAYABLE", amountPayable.ToString("F2"));
                           delNote.SetNode("DELIVERYNOTE/FOOTER/COD", cod ? "Y" : "N");

                           string timeStamp = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();

                           //delNote.SetNode("DELIVERYNOTE/HEADER/BRANCH", branchNo.ToString());
                           delNote.SetNode("DELIVERYNOTE/HEADER/BUFFNO", row[CN.BuffNo].ToString());
                           delNote.SetNode("DELIVERYNOTE/HEADER/PRINTED", timeStamp);

                           int items = 0;
                           //bool found = false;
                           string delOrColl = "";
                           //deliveryAudit = new BDelivery();

                           //add line items to the xml document
                           DataRow[] allItemRows = dtSchedules.Select();
                           foreach (DataRow delRow in allItemRows)
                           {
                              // Add the next line item to the Delivery Note
                              items++;
                              //IP - 12/03/08 - (69461) added 'dtSchedules.Rows.Count' to hold a count of the items to be printed on the delivery note.
                              delAddressType = (string)delRow[CN.DeliveryAddress];
                              delNote.AddLineItem(delRow, dtSchedules.Rows.Count, ref items);
                              // Audit this line item if it has already been printed
                              deliveryAudit.AuditDeliveryReprint(conn, trans,
                                  (string)delRow[CN.AcctNo],
                                  Convert.ToInt32(delRow[CN.AgrmtNo]),
                                  Convert.ToInt32   (delRow[CN.ItemNo]),
                                  Convert.ToInt16(delRow[CN.StockLocn]),
                                  Convert.ToInt32(row[CN.BuffNo]),
                                  printedBy);

                              qty += Convert.ToDouble(delRow[CN.Quantity]);
                              delDate = ((DateTime)delRow[CN.DateReqDel]).ToLongDateString() + " " + (string)delRow[CN.TimeReqDel];
                              user = Convert.ToInt32(delRow[CN.EmpeeNoSale]);
                              locn = Convert.ToInt32(delRow[CN.StockLocn]);
                              delOrColl = (string)delRow[CN.DelOrColl];
                           }
                           // LW 69370 Use delAddressType to load tel numbers
                           LoadTelNumbers(cust.Addresses, delNote, delAddressType.Trim());
                           delNote.SetNode("DELIVERYNOTE/HEADER/DELDATE", delDate);

                           if (qty < 0)
                              delNote.SetNode("DELIVERYNOTE/HEADER/DELTEXT", GetResource("T_COLLTEXT"));
                           else
                           {
                              if (delOrColl == "D")
                                 delNote.SetNode("DELIVERYNOTE/HEADER/DELTEXT", GetResource("T_DELTEXT"));
                              else
                                 delNote.SetNode("DELIVERYNOTE/HEADER/DELTEXT", GetResource("T_REDELIVERY"));
                           }

                           delAddressType = delAddressType.Trim();
                           if (delAddressType != "H")
                              LoadAddressDetails(addrTable, delNote, delAddressType);

                           //login = new BLogin();
                           userName = login.GetEmployeeName(conn, trans, user);
                           printedByName = login.GetEmployeeName(conn, trans, printedBy);

                           delNote.SetNode("DELIVERYNOTE/HEADER/BRANCH", branchNo.ToString());
                           //delNote.SetNode("DELIVERYNOTE/HEADER/BRANCH", locn.ToString());  //NM - UAT(5.2) - 568
                           delNote.SetNode("DELIVERYNOTE/HEADER/LOCATION", locn.ToString());
                           delNote.SetNode("DELIVERYNOTE/HEADER/PRINTEDBY", printedByName);
                           delNote.SetNode("DELIVERYNOTE/FOOTER/USER", user.ToString() + " ");
                           delNote.SetNode("DELIVERYNOTE/FOOTER/USERNAME", userName);

                           //if (index == accounts.Rows.Count) //-- IP - 04/08/08 - UAT5.1 - UAT(508)
                           if (index == allAccountRows.Length) //IP - 04/08/08 - UAT5.1 - UAT(508)
                              delNote.SetNode("DELIVERYNOTE/LAST", "TRUE");
                           else
                           {
                              index++; 
                              delNote.SetNode("DELIVERYNOTE/LAST", "FALSE");
                           }

                           sched.User = printedBy;
                           sched.SetDeliveryNotesPrinted(conn, trans, (string)row[CN.AcctNo],
                                             Convert.ToInt32(row[CN.BuffNo]),
                                             Convert.ToInt32(row[CN.BuffBranchNo]));

                           /* import this delivery note into the main document */
                           dxml.ImportNode(delNote.DocumentElement, true);
                           trans.Commit();

                           allItemRows = null;
                           delNote = null;
                        }
                        accounts = null;
                        allAccountRows = null;
                        System.GC.Collect();
                     }

                     Response.Write(dxml.TransformXml());
                     break;
                  }
                  catch (SqlException ex)
                  {
                     CatchDeadlock(ex, conn);
                  }
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
            //if(conn.State != ConnectionState.Closed)
            //   conn.Close();
            deliveryAudit = null;
            cust = null;
            sched = null;
            login = null;
            del = null;
			}
		}

        //private void LoadCustDetails(DataTable dt, DeliveryNoteXML dxml)
        //{
        //    foreach(DataRow row in dt.Rows)
        //    {
        //        if(((string)row[CN.Alias]).Length > 0)
        //            dxml.SetNode("DELIVERYNOTE/HEADER/ALIAS", "ALIAS: " + (string)row[CN.Alias]);

        //        dxml.SetNode("DELIVERYNOTE/HEADER/FIRSTNAME", (string)row[CN.FirstName] + " ");
        //        dxml.SetNode("DELIVERYNOTE/HEADER/LASTNAME", (string)row[CN.LastName]);
        //    }
        //}

		private void LoadAddressDetails(DataTable dt, DeliveryNoteXML dxml, string adrressType)
		{
			string addType = "";

            DataRow[] allAddressRows = dt.Select();
            foreach (DataRow row in allAddressRows)
            {
				addType = ((string)row["AddressType"]).Trim();
				if(addType == adrressType)
				{
					address1 = (string)row[CN.Address1];
					
					dxml.SetNode("DELIVERYNOTE/HEADER/ADDRESS1", address1);
					dxml.SetNode("DELIVERYNOTE/HEADER/ADDRESS2", (string)row[CN.Address2]);
					dxml.SetNode("DELIVERYNOTE/HEADER/ADDRESS3", (string)row[CN.Address3]);
					dxml.SetNode("DELIVERYNOTE/HEADER/POSTCODE", (string)row[CN.PostCode]);
					dxml.SetNode("DELIVERYNOTE/FOOTER/CUSTNOTES", (string)row["Notes"]);
				}
			}
            
            allAddressRows = null;
		}

      private void LoadTelNumbers(DataTable dt, DeliveryNoteXML dxml, string addressType)
		{
			string addType = String.Empty;

            DataRow[] allTelNumberRows = dt.Select();
            foreach (DataRow row in allTelNumberRows)
            {
				addType = ((string)row["AddressType"]).Trim();
            string address = addType.Substring(0, 1); // 69370 Delivery telephone number to be included
            switch (address)
				{
					case "H":	
						dxml.SetNode("DELIVERYNOTE/HEADER/HOMETEL", GetResource("L_HOME") + ": " + ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"], false);
						break;
					case "W":	dxml.SetNode("DELIVERYNOTE/HEADER/WORKTEL", GetResource("L_WORK") + ": " + ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"], false);
						break;
					case "M":	dxml.SetNode("DELIVERYNOTE/HEADER/MOBILE", GetResource("L_MOBILE") + ": " + (string)row["Phone"],false);
						break;
               case "D":
                  if (addType == addressType)
                  {
                     dxml.SetNode("DELIVERYNOTE/HEADER/DELTEL", GetResource("L_DEL") + ": " + ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"], false);
                  }
                  break;
					default:
						break;
				}
			}

            allTelNumberRows = null;
		}

        //private void LoadCancelledDelNotes(SqlConnection conn, SqlTransaction trans,
        //    DeliveryNoteXML dxml, DataRow r, ref bool found)
        //{
        //    int buffNo = 0;
        //    DateTime datePrinted = DateTime.MinValue.AddYears(1899);

        //    BItem item = new BItem();
        //    item.GetCancelledDelNote(conn, trans,
        //        (string)r[CN.AcctNo],
        //        (int)r[CN.AgrmtNo],
        //        Convert.ToInt32(r[CN.ItemId]),
        //        Convert.ToInt16(r[CN.StockLocn]),
        //        out buffNo,
        //        out datePrinted);
        //    if(buffNo > 0)
        //    {
        //        dxml.SetNode("DELIVERYNOTE/HEADER/CANCELTEXT", GetResource("M_DELIVERYNOTECANCEL", new object[]{buffNo.ToString(),datePrinted.ToShortDateString()}));
        //        found = true;
        //    }
        //}
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
