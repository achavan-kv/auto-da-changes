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
	/// Summary description for WDeliveryNote.
	/// </summary>
	public partial class WDeliveryNote : CommonWebPage
	{
		private string address1 = "";
		private string delAddressType = "";
		private string homeAddrType = "H";
		private DataTable addrTable = null;

		private void Page_Load(object sender, System.EventArgs e)
		{
			//SqlConnection conn = null; 
			SqlTransaction trans;

            BAccount acct = new BAccount();
            BLogin login = new BLogin();
            BDelivery deliveryAudit = new BDelivery();
            DCustomer cust = new DCustomer();
            DSchedule sched = new DSchedule();

            string userName = "";
			string printedByName = "";
			string delDate = "";
			//string relationship = "H";
			double qty = 0;
			int index = 1;
			decimal amountPayable = 0;
			decimal charges = 0;
			bool cod = false;

			XmlDocument deliveryNotes = new XmlDocument();

			try
			{
            DataTable lineItems;
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

						foreach(XmlNode deliveryNote in deliveryNotes.DocumentElement.ChildNodes)
						{

                     trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);   
                            string acctno = deliveryNote.SelectSingleNode("ACCTNO").InnerText;
                            int buffLocn = Convert.ToInt32(deliveryNote.SelectSingleNode("BRANCHNO").InnerText);	
							int user = Convert.ToInt32(deliveryNote.SelectSingleNode("USER").InnerText);
							int userSale = Convert.ToInt32(deliveryNote.SelectSingleNode("SALESUSER").InnerText);
							string printText = deliveryNote.SelectSingleNode("PRINTTEXT").InnerText;
							string culture = deliveryNote.SelectSingleNode("CULTURE").InnerText;
							int delnotenum = Convert.ToInt32(deliveryNote.SelectSingleNode("DELNOTENUM").InnerText);
							string timeReqDel = deliveryNote.SelectSingleNode("TIMEREQDEL").InnerText;
							delAddressType = deliveryNote.SelectSingleNode("DELADDRESS").InnerText;
							int locn = Convert.ToInt32(deliveryNote.SelectSingleNode("LOCATION").InnerText);
                            string custID = deliveryNote.SelectSingleNode("CUSTID").InnerText;
                            string customerName = deliveryNote.SelectSingleNode("CUSTOMERNAME").InnerText;
                            string alias = deliveryNote.SelectSingleNode("ALIAS").InnerText;

							int buffno = 0;
							bool gol = acctno.Replace("-","").Substring(3,1)=="7";
							if(gol)
							{
								if(printText.Length==0)
									printText = "GOL";
								else
									printText += " - GOL";
							}

							//Set the culture for currency formatting
							//Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                            base.SetCulture();

							DateTime dateReqDel = Convert.ToDateTime(deliveryNote.SelectSingleNode("DATEREQDEL").InnerText);

							DeliveryNoteXML delNote = dxml.CreateDeliveryNote(countryCode);

							delNote.SetNode("DELIVERYNOTE/HEADER/ACCTNO", acctno);

                            if (alias.Length > 0)
                            {
                                delNote.SetNode("DELIVERYNOTE/HEADER/ALIAS", "ALIAS: " + alias);
                            }

                            delNote.SetNode("DELIVERYNOTE/HEADER/CUSTOMERNAME", customerName);

                        //cust = new DCustomer();
                            cust.GetCustomerAddresses(conn, trans, custID);
                            LoadAddressDetails(cust.Addresses, delNote, homeAddrType);
                            LoadTelNumbers(cust.Addresses, delNote,delAddressType.Trim());
                            addrTable = cust.Addresses;

                        //login = new BLogin();
							userName = login.GetEmployeeName(conn, trans, userSale);
							printedByName = login.GetEmployeeName(conn, trans, user);

							//retrieve the rest of the required data
                        //acct = new BAccount();
                            acct.AccountNumber = acctno.Replace("-", String.Empty); //IP - 10/02/10 - CR1048 (Ref:3.1.3) Merged - Malaysia Enhancements (CR1072)

							if(printText == GetResource("T_REPRINT"))
							{
                               // lineItems = acct.GetReprintDetails(conn, trans, acctno.Replace("-", ""), locn, buffLocn, delnotenum,    //CR1072 Malaysia merge -LW71408
                                lineItems = acct.GetReprintDetails(conn, trans, acctno.Replace("-", ""), buffLocn, delnotenum,    //CR1072 Malaysia merge -LW71408 --IP - 22/02/10 - Undone 71408 - reinstate later
									dateReqDel, delAddressType, timeReqDel, out amountPayable, out charges, out cod);
								
								buffno = delnotenum;
							}
							else
							{								
								lineItems = acct.GetDeliveryNotes(acctno.Replace("-",""), conn, trans, buffLocn, delnotenum,
                                                                  dateReqDel, delAddressType, timeReqDel, 
                                                                  out amountPayable, out charges, out cod);
							}

                            

							delNote.SetNode("DELIVERYNOTE/FOOTER/ADDCHARGES", charges.ToString("F2"));
							delNote.SetNode("DELIVERYNOTE/FOOTER/PAYABLE", amountPayable.ToString("F2"));
							delNote.SetNode("DELIVERYNOTE/FOOTER/COD", cod ? "Y" : "N" );

							string timeStamp = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();

							delNote.SetNode("DELIVERYNOTE/HEADER/BRANCH", buffLocn.ToString());
							delNote.SetNode("DELIVERYNOTE/HEADER/BUFFNO", delnotenum.ToString());
							delNote.SetNode("DELIVERYNOTE/HEADER/LOCATION", locn.ToString());
							delNote.SetNode("DELIVERYNOTE/HEADER/PRINTED", timeStamp);
							
							delNote.SetNode("DELIVERYNOTE/HEADER/PRINTEDBY", printedByName);


							int items = 0;
						//	bool found = false;
							string delOrColl = "";
							qty = 0;
                        //deliveryAudit = new BDelivery();
                            
                            //add line items to the xml document
                            DataRow[] allItemRows = lineItems.Select();
                                                   
                            foreach (DataRow row in allItemRows)
                            {
                                // Add the next line item to the Delivery Note
                                items++;
                                //IP - 12/03/08 - (69461) added 'lineItems.Rows.Count' to hold a count of the items to be printed on the delivery note.
                                delNote.AddLineItem(row, lineItems.Rows.Count, ref items); 
                                // Audit this line item if it has already been printed
                                deliveryAudit.AuditDeliveryReprint(conn, trans,
                                    (string)row[CN.AcctNo],
                                    Convert.ToInt32(row[CN.AgrmtNo]),
                                    Convert.ToInt32(row[CN.ItemNo]),
                                    Convert.ToInt16(row[CN.StockLocn]),
                                    delnotenum,
                                    user);

                                qty += Convert.ToDouble(row[CN.Quantity]);
                                delOrColl = (string)row[CN.DelOrColl];
                                delDate = ((DateTime)row[CN.DateReqDel]).ToLongDateString() + " " + (string)row[CN.TimeReqDel];
                                /* UAT 631 - this is not required as already handled by later stored procedure
                                 * change if (printText != GetResource("T_REPRINT"))
                                { //check if already printed if not reprint
                                    if (!Convert.IsDBNull(row[CN.DatePrinted]))
                                    {
                                        //already printed so resetting date to allow for reprint but force reprint
                                        sched.SetDeliveryNotesPrinted(conn, trans, acctno.Replace("-", ""), delnotenum, buffLocn);
                                        trans.Commit();
                                        throw new STLException(" Duplicate delivery note detected not printed for " + acctno + " buffno: " + delnotenum.ToString()
                                            + " Please reload screen or choose reprint");
                                        //printText = //GetResource("T_DELDUPLICATETEXT")
                                        //  "Check Duplicate Delivery Note Printed" + " " + ((DateTime)row[CN.DatePrinted]).ToLongDateString()
                                        //+ " " + ((DateTime)row[CN.DatePrinted]).ToLongTimeString(); ;
                                    }
                                }*/
                            }

                            delNote.SetNode("DELIVERYNOTE/HEADER/PRINTTEXT", printText);
							delNote.SetNode("DELIVERYNOTE/HEADER/DELDATE", delDate);

                            //69818 Collection Note Header should only show if quantity is less than zero
                            if (qty < 0)
                            {
                                delNote.SetNode("DELIVERYNOTE/HEADER/DELTEXT", GetResource("T_COLLTEXT"));
                                //IP - 19/02/10 - CR1072 - LW 69818 - Printing Fixes from 4.3 - Merge
                                if (countryCode == "Y")
                                {
                                    delNote.SetNode("DELIVERYNOTE/HEADER/NOTETEXT", GetResource("M_COLLECTIONNOTEUPPERCASE"));
                                }
                                delNote.SetNode("DELIVERYNOTE/FOOTER/USER", user.ToString() + " ");
                                delNote.SetNode("DELIVERYNOTE/FOOTER/USERNAME", printedByName);
                            }
                            else
                            {
                                if (delOrColl == "D")
                                    delNote.SetNode("DELIVERYNOTE/HEADER/DELTEXT", GetResource("T_DELTEXT"));
                                else
                                    delNote.SetNode("DELIVERYNOTE/HEADER/DELTEXT", GetResource("T_REDELIVERY"));

                                //IP - 19/02/10 - CR1072 - LW 69818 - Printing Fixes from 4.3 - Merge
                                if (countryCode == "Y")
                                {
                                    delNote.SetNode("DELIVERYNOTE/HEADER/NOTETEXT", GetResource("M_DELIVERYNOTEUPPERCASE"));
                                }

                                delNote.SetNode("DELIVERYNOTE/FOOTER/USER", userSale.ToString() + " ");
                                delNote.SetNode("DELIVERYNOTE/FOOTER/USERNAME", userName);
                            }

							delAddressType = delAddressType.Trim();
							if(delAddressType != "H")
								LoadAddressDetails(addrTable, delNote, delAddressType);

							if(index==deliveryNotes.DocumentElement.ChildNodes.Count)
								delNote.SetNode("DELIVERYNOTE/LAST", "TRUE");
							else
							{
								index++;
								delNote.SetNode("DELIVERYNOTE/LAST", "FALSE");
							}

                        //sched = new DSchedule();
							sched.User = user;
							sched.SetDeliveryNotesPrinted(conn, trans, acctno.Replace("-",""), delnotenum, buffLocn);

							/* import this delivery note into the main document */
							dxml.ImportNode(delNote.DocumentElement, true);
                            trans.Commit();

                            allItemRows = null;
                            delNote = null;
                        lineItems = null;
						}

                        GC.Collect();
                        Response.Write(dxml.Transform());
                        dxml = null;
                        break;
                    }
					catch(SqlException ex)
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
            acct = null;
            login = null;
            deliveryAudit = null;
            cust = null;
            sched = null;
            deliveryNotes = null;
			}
		}

        //private void LoadCustDetails(DataTable dt, DeliveryNoteXML dxml)
        //{
        //    foreach(DataRow row in dt.Rows)
        //    {
        //        if(((string)row[CN.Alias]).Length > 0)
        //            dxml.SetNode("DELIVERYNOTE/HEADER/ALIAS", "ALIAS: " + (string)row[CN.Alias]);

        //        dxml.SetNode("DELIVERYNOTE/HEADER/CUSTOMERNAME", (string)row[CN.FirstName] + " " + (string)row[CN.LastName]);
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
            //string addType = String.Empty;
            //DataRow[] allTelNumberRows = dt.Select();

            foreach (DataRow row in dt.Rows)
            {
                //addType = ((string)row["AddressType"]).Trim();
                if (((string)row["AddressType"]).Trim().Length > 0)
                {
                    //  string address = addType.Substring(0, 1); // 69370 Deliver telephone number to be included
                    switch (((string)row["AddressType"]).Trim().Substring(0, 1))
                    {
                        case "H":
                            dxml.SetNode("DELIVERYNOTE/HEADER/HOMETEL", GetResource("L_HOME") + ": " + ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"], false);
                            break;
                        case "W": dxml.SetNode("DELIVERYNOTE/HEADER/WORKTEL", GetResource("L_WORK") + ": " + ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"], false);
                            break;
                        case "M": dxml.SetNode("DELIVERYNOTE/HEADER/MOBILE", GetResource("L_MOBILE") + ": " + (string)row["Phone"], false);
                            break;
                        //CR1072 Malaysia merge -LW71408
                        //case "D":
                        //    if (addressType == ((string)row["AddressType"]).Trim())
                        //    {
                        //        dxml.SetNode("DELIVERYNOTE/HEADER/DELTEL", GetResource("L_DEL") + ": " + ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"], false);
                        //    }
                        //    break;
                        default:
                            break;
                    }
                }
             //   allTelNumberRows = null;
            }
		}

        //private void LoadCancelledDelNotes(SqlConnection conn, SqlTransaction trans,
        //                                DeliveryNoteXML dxml, DataRow r, ref bool found)
        //{
        //    int buffNo = 0;
        //    DateTime datePrinted = DateTime.MinValue.AddYears(1899);

        //    BItem item = new BItem();
        //    item.GetCancelledDelNote(conn, trans,
        //                            (string)r[CN.AcctNo],
        //                            (int)r[CN.AgrmtNo],
        //                            Convert .ToInt32(r[CN.ItemId]),
        //                            Convert.ToInt16(r[CN.StockLocn]),
        //                            out buffNo,
        //                            out datePrinted);
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
