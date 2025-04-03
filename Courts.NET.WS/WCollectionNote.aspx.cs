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
using System.Data.SqlClient;
using System.Configuration;
using STL.Common.Constants.ColumnNames;
using System.Threading;
using System.Globalization;

namespace STL.WS
{
	/// <summary>
	/// Summary description for WCollectionNote.
	/// </summary>
	public partial class WCollectionNote : CommonWebPage
	{
		private string delAddressType = "";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			DataSet custDetails = null;
			string userName = "";
			int index = 1;
			XmlDocument collectionNotes = new XmlDocument();

			try
			{
				/* Extract deliveryNotes document from the QueryString */
				string collectionNotesXmlStr = Request["collectionNotes"];
				string countryCode = Request["countryCode"];
				collectionNotes.LoadXml(collectionNotesXmlStr);

				//Build the XML document
				DeliveryNoteXML dxml = new DeliveryNoteXML(countryCode);
				dxml.Load("<DELIVERYNOTES/>");	

				foreach(XmlNode collectionNote in collectionNotes.DocumentElement.ChildNodes)
				{
					string acctNo = collectionNote.SelectSingleNode("ACCTNO").InnerText;
					int branch = Convert.ToInt32(collectionNote.SelectSingleNode("BRANCHNO").InnerText);
					int user = Convert.ToInt32(collectionNote.SelectSingleNode("USER").InnerText);
					string customerID = collectionNote.SelectSingleNode("CUSTOMERID").InnerText;
					int buffNo = Convert.ToInt32(collectionNote.SelectSingleNode("BUFFNO").InnerText);
					string culture = collectionNote.SelectSingleNode("CULTURE").InnerText;
					delAddressType = collectionNote.SelectSingleNode("DELADDRESS").InnerText;
						
					//Set the culture for currency formatting
					//Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                    base.SetCulture();

					DateTime collDate = Convert.ToDateTime(collectionNote.SelectSingleNode("COLLDATE").InnerText);

					//Build the XML document using the same templates as the delivery note
					DeliveryNoteXML colNote = dxml.CreateDeliveryNote(countryCode);

					//retrieve the basic customer details
					BCustomer cust = new BCustomer();
					custDetails = cust.GetBasicCustomerDetails(null, null, customerID, acctNo, "H");

					foreach(DataTable dt in custDetails.Tables)
					{
						switch(dt.TableName)
						{
							case "BasicDetails":	
								LoadCustDetails(dt, colNote);
								break;
							case TN.CustomerAddresses:		
								LoadAddressDetails(dt, colNote);
								LoadTelNumbers(dt, colNote);
								break;
							default:
								break;
						}
					}

					BLogin login = new BLogin();
					userName = login.GetEmployeeName(null, null, user);

					colNote.SetNode("DELIVERYNOTE/HEADER/ACCTNO", acctNo);
					colNote.SetNode("DELIVERYNOTE/HEADER/BRANCH", branch.ToString());
					colNote.SetNode("DELIVERYNOTE/HEADER/BUFFNO", buffNo.ToString());
					colNote.SetNode("DELIVERYNOTE/HEADER/DELTEXT", GetResource("M_COLLECTION"));	
					colNote.SetNode("DELIVERYNOTE/FOOTER/USER", user.ToString()+" ");
					colNote.SetNode("DELIVERYNOTE/FOOTER/USERNAME", userName);
					colNote.SetNode("DELIVERYNOTE/HEADER/LOCATION", branch.ToString());
					colNote.SetNode("DELIVERYNOTE/HEADER/PRINTED", DateTime.Now.ToLongDateString());
					colNote.SetNode("DELIVERYNOTE/HEADER/DELDATE", collDate.ToLongDateString());

					int items = 0;

					foreach(XmlNode item in collectionNote.SelectSingleNode("LINEITEMS").ChildNodes)
					{
						items++;
						colNote.AddCollectionItem(item, ref items);
					}

					if(index==collectionNotes.DocumentElement.ChildNodes.Count)
						colNote.SetNode("DELIVERYNOTE/LAST", "TRUE");
					else
					{
						index++;
						colNote.SetNode("DELIVERYNOTE/LAST", "FALSE");
					}

					/* import this collection note into the main document */
					dxml.ImportNode(colNote.DocumentElement, true);
				}
				Response.Write(dxml.Transform());		
			}
			catch(Exception ex)
			{
				logException(ex, Function);
				Response.Write(ex.Message);
			}
		}

		private void LoadCustDetails(DataTable dt, DeliveryNoteXML dxml)
		{
			foreach(DataRow row in dt.Rows)
			{
				dxml.SetNode("DELIVERYNOTE/HEADER/FIRSTNAME", (string)row[CN.FirstName]+" ");
				dxml.SetNode("DELIVERYNOTE/HEADER/LASTNAME", (string)row[CN.LastName]);
			}
		}

		private void LoadAddressDetails(DataTable dt, DeliveryNoteXML dxml)
		{
			string addType = "";
			
			if(delAddressType.Length == 0)
				delAddressType = "H";

			foreach(DataRow row in dt.Rows)
			{
				addType = ((string)row["AddressType"]).Trim();
				if(addType == delAddressType.Trim())
				{
					dxml.SetNode("DELIVERYNOTE/HEADER/ADDRESS1", (string)row["Address1"]);
					dxml.SetNode("DELIVERYNOTE/HEADER/ADDRESS2", (string)row["Address2"]);
					dxml.SetNode("DELIVERYNOTE/HEADER/ADDRESS3", (string)row["Address3"]);
					dxml.SetNode("DELIVERYNOTE/HEADER/POSTCODE", (string)row["PostCode"]);
					dxml.SetNode("DELIVERYNOTE/FOOTER/CUSTNOTES", (string)row["Notes"]);
				}
			}
		}
		
		private void LoadTelNumbers(DataTable dt, DeliveryNoteXML dxml)
		{
			string addType = "";
			foreach(DataRow row in dt.Rows)
			{
				addType = ((string)row["AddressType"]).Trim();
				switch(addType)
				{
					case "H":	
						dxml.SetNode("DELIVERYNOTE/HEADER/HOMETEL", GetResource("L_HOME") + ": " + ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"], false);
						break;
					case "W":	dxml.SetNode("DELIVERYNOTE/HEADER/WORKTEL", GetResource("L_WORK") + ": " + ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"], false);
						break;
					case "M":	dxml.SetNode("DELIVERYNOTE/HEADER/MOBILE", GetResource("L_MOBILE") + ": " + (string)row["Phone"],false);
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
