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
using STL.Common.Constants.StoreInfo;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using System.Globalization;
using STL.Common.Printing.AgreementPrinting;
using STL.Common.Constants.FTransaction;
using System.IO;



namespace STL.WS
{
	/// <summary>
	/// Summary description for WActionSheet.
	/// </summary>
	public partial class WActionSheet : CommonWebPage
	{
		SqlConnection conn = null; 
		//SqlTransaction trans = null;

        DCustomer cust = new DCustomer();
        DDelivery del = new DDelivery();
        DFinTrans fintrans = new DFinTrans();

        DataTable custAddresses = null;
        DataTable employer = null;
        DataTable otherCustomers = null;

		int empeeNo = 0;
      string empName = String.Empty;
      string culture = String.Empty;
      string acctNo = String.Empty;
      string countryCode = String.Empty;
      string currStatus = String.Empty;
      string storeType = String.Empty;

      protected void Page_Load(object sender, System.EventArgs e)
      {
          //DataSet custDetails = null;
          ActionSheetXML axml = null;
          int privilegeCount = 0;
          int index = 1;

          try
          {
              //Extract variables from the QueryString
              string actionSheetXmlStr = Request["actionSheetXmlStr"]; // need to pass in from PL as struct
              empeeNo = Convert.ToInt32(Request["empeeNo"]);
              empName = Request["empName"];
              countryCode = Request["countryCode"];
              culture = Request["culture"];

              string courtsBranches = Request["courtsBranches"];
              //CR903 Create an array list to hold all the courts branches
              ArrayList branches = new ArrayList();

              string[] branchArray = courtsBranches.Split('|');

              foreach (string branch in branchArray)
              {
                  if (branch != String.Empty)
                  {
                      branches.Add(branch);
                  }
              }

              //Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
              base.SetCulture();

              //UAT 23 The conn object should not be referred to in the finally block, use using instead
              using (conn = new SqlConnection(Connections.Default))
              {
                  do
                  {
                      try
                      {
                          conn.Open();
                          using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                          {
                              XmlDocument actionSheets = new XmlDocument();
                              actionSheets.LoadXml(actionSheetXmlStr);

                              axml = new ActionSheetXML(countryCode);
                              axml.Load("<ACTIONSHEETS/>");

                              foreach (XmlNode sheet in actionSheets.DocumentElement.ChildNodes)
                              {
                                  acctNo = sheet.InnerText; //.SelectSingleNode("ACCTNO").InnerText;
                                  if (branches.Contains(acctNo.Substring(0, 3)))
                                  {
                                      storeType = "Courts";
                                  }
                                  else
                                  {
                                      storeType = "Non Courts";
                                  }
                                  ActionSheetXML acSheet = axml.CreateActionSheet(countryCode);

                                  acSheet.SetNode("ACTIONSHEET/HEADER/STORETYPE", storeType);
                                  acSheet.SetNode("ACTIONSHEET/HEADER/ACCTNO", acctNo);

                                  //retrieve the rest of the required data
                                  cust = new DCustomer();
                                  cust.GetDetailsForDebtCollector(acctNo);
                                  LoadCustDetails(conn, trans, cust.Customer, acSheet);

                                  //retrieve the rest of the required data
                                  cust = new DCustomer();
                                  cust.GetDetailsForDebtCollector(acctNo);
                                  LoadCustDetails(conn, trans, cust.Customer, acSheet);

                                  del = new DDelivery();
                                  del.GetItemsForDebtCollector(acctNo);
                                  LoadDeliveryDetails(del.Deliveries, acSheet);

                                  fintrans = new DFinTrans();
                                  fintrans.GetDetailsForDebtCollector(acctNo, out privilegeCount);
                                  LoadFinancialDetails(fintrans.FinTrans, acSheet);

                                  if (privilegeCount > 0)
                                      acSheet.SetNode("ACTIONSHEET/HEADER/PRIVILEGECUSTOMER", "Privilege Club Customer");

                                  if ((bool)Country[CountryParameterNames.PrintCharges])
                                      acSheet.SetNode("ACTIONSHEET/DETAILS/PRINTCHARGES", Boolean.TrueString);
                                  else
                                      acSheet.SetNode("ACTIONSHEET/DETAILS/PRINTCHARGES", Boolean.FalseString);

                                  if (index == actionSheets.DocumentElement.ChildNodes.Count)
                                      acSheet.SetNode("ACTIONSHEET/LAST", "TRUE");
                                  else
                                  {
                                      index++;
                                      acSheet.SetNode("ACTIONSHEET/LAST", "FALSE");
                                  }

                                  /* import this delivery note into the main document */
                                  axml.ImportNode(acSheet.DocumentElement, true);

                                  cust = null;
                                  del = null;
                                  fintrans = null;
                                  acSheet = null;
                              }

                              //GC.Collect();
                              Response.Write(axml.Transform());
                              trans.Commit();
                              break;
                          }
                      }
                      catch (SqlException ex)
                      {
                          CatchDeadlock(ex, conn);
                      }
                  } while (retries <= maxRetries);

              }
          }
          catch (Exception ex)
          {
              logException(ex, Function);
              Response.Write(ex.Message);
          }
      }

		private void LoadDeliveryDetails(DataTable dt, ActionSheetXML axml)
		{
			decimal totalVal = 0;
			decimal price = 0;
            DataRow[] deliveryRows = dt.Select();

            foreach (DataRow row in deliveryRows)
            {
				price = Convert.ToDecimal(row[CN.Quantity]) * Convert.ToDecimal(row[CN.Price]);
				axml.AddLineItem(row, price);
				totalVal += price;
			}

			axml.SetNode("ACTIONSHEET/LINEITEMS/TOTALVALUE", totalVal.ToString("C2"));
            deliveryRows = null;
		}

        //private void LoadServiceRequestDetails(DataTable dt, ActionSheetXML axml)
        //{
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        axml.AddServiceRequest(row);   
        //    }
        //}

		private void LoadCustDetails(SqlConnection conn, SqlTransaction trans,
			DataTable dt, ActionSheetXML axml)
		{
            DCustomer cust = new DCustomer();
            BPayment pay = new BPayment();
            DEmployment emp = null;

			decimal instAmount = 0;
			decimal collectionFee = 0;
			decimal bailiffFee = 0;
			decimal paymentAmount = 0;
			decimal arrears = 0;
			int day = 0;
			try
			{
                DataRow[] customerRows = dt.Select();

                foreach (DataRow row in customerRows)
                {
					string custID = (string)row[CN.CustID];
					arrears = (decimal)row[CN.Arrears];
					currStatus = (string)row[CN.CurrStatus];

                    //Get the customer's photo and the virtual path to this photo
                    string photoFileName = String.Empty;
                    string serverPath = String.Empty;
                    string signatureFileName = String.Empty;
                    string serverSignaturePath = String.Empty;

                    photoFileName = cust.GetCustomerPhoto(conn, trans, custID);
                    serverPath = (string)Country[CountryParameterNames.PhotoDirectory];
                    string physicalPath = String.Empty;
                    if (serverPath != String.Empty)
                    {
                      //serverPath = serverPath.Remove(serverPath.Length - 1, 1);
                      try
                      {
                         physicalPath = serverPath.Remove(0, serverPath.LastIndexOf("//"));
                      }
                      catch
                      {
                         //If the server path has syntax errors then catch and leave the path as empty
                      }
                    }

                    //Get the customer's signature and the virtual path to this signature
                    signatureFileName = cust.GetCustomerSignature(conn, trans, custID);
                    serverSignaturePath = (string)Country[CountryParameterNames.SignatureDirectory];
                    string SignaturePath = String.Empty;
                    if (serverSignaturePath != String.Empty)
                    {
                      //serverSignaturePath = serverSignaturePath.Remove(serverSignaturePath.Length - 1, 1);
                      try
                      {
                         SignaturePath = serverSignaturePath.Remove(0, serverSignaturePath.LastIndexOf("//"));
                      }
                      catch
                      {
                         //If the serverSignaturePath has syntax errors then catch and leave the path as empty
                      }
                    }

                    cust = new DCustomer();
                    cust.GetCustomerAddresses(null, null, custID);
                    custAddresses = cust.Addresses;
                    LoadAddressDetails(axml);

                    emp = new DEmployment();
                    emp.CustomerID = custID;
                    emp.GetEmployerDetails(acctNo);
                    employer = emp.EmpDetails;
                    LoadEmployerDetails(axml);

				    otherCustomers = cust.GetOtherCustomers(acctNo);
				    if(otherCustomers != null)
					    LoadOtherCustomers(axml);

				    //pay = new BPayment();
				    int debitAccount = 0;
				    int segmentId = 0;
				    pay.CalculateCreditFee(
					    conn, 
					    trans,
					    countryCode,
					    acctNo,
					    ref paymentAmount,
					    TransType.Payment,
					    ref empeeNo,
					    arrears,
					    out collectionFee,
					    out bailiffFee,
					    out debitAccount,
					    out segmentId);

				    if(row[CN.Instalment] != DBNull.Value)
					    instAmount = (decimal)row[CN.Instalment];
				    if(row[CN.DateFirst] != DBNull.Value)
					    day = ((DateTime)row[CN.DateFirst]).Day;

				    axml.SetNode("ACTIONSHEET/HEADER/TITLE", (string)row[CN.Title] + " ");
				    axml.SetNode("ACTIONSHEET/HEADER/FIRSTNAME", (string)row[CN.FirstName] + " ");
				    axml.SetNode("ACTIONSHEET/HEADER/LASTNAME", (string)row[CN.name]);

                    try
                    {
                      if (Convert.ToBoolean(Country[CountryParameterNames.EnablePhotoPrinting]) == true)
                      {
                         if (photoFileName != String.Empty)//&& File.Exists(physicalPath + photoFileName))
                         //File.Exists(Server.MapPath(virtualPath + photoFileName)))
                         {
                            axml.SetNode("ACTIONSHEET/HEADER/PHOTO", physicalPath + photoFileName);
                            //Server.MapPath(virtualPath + photoFileName));
                         }
                         if (signatureFileName != String.Empty)//&& File.Exists(SignaturePath + signatureFileName))
                         //File.Exists(Server.MapPath(virtualSignaturePath + signatureFileName)))
                         {
                            axml.SetNode("ACTIONSHEET/HEADER/SIGNATURE", SignaturePath + signatureFileName);
                            //Server.MapPath(virtualSignaturePath + signatureFileName));
                         }
                      }
                    }
                    catch
                    {
                      //If there is a problem with accessing the file, i.e. it isn't there, then catch and the sheet will still be printed
                    }

                    if ((decimal)row[CN.RFCreditLimit] > 0 &&
                        (string)row[CN.LimitType] == "A" &&
                        (bool)Country[CountryParameterNames.PrintRFIndicator])
                    {
                        axml.SetNode("ACTIONSHEET/HEADER/RFCUSTOMER", "Ready Finance Customer");
                    }

				    string alias = (string)row[CN.Alias];
				    if(alias.Length > 0)
					    axml.SetNode("ACTIONSHEET/HEADER/ALIAS", "Alias: " + alias);

				    axml.SetNode("ACTIONSHEET/DETAILS/COLLECTOR", empName);
				    axml.SetNode("ACTIONSHEET/DETAILS/DATE", DateTime.Today.ToShortDateString());
				    axml.SetNode("ACTIONSHEET/DETAILS/BALANCE", ((decimal)row[CN.OutstBal]).ToString("F2"));
				    axml.SetNode("ACTIONSHEET/DETAILS/ARREARS", ((decimal)row[CN.Arrears]).ToString("F2"));
				    axml.SetNode("ACTIONSHEET/DETAILS/COSTS", collectionFee.ToString("F2"));
				    axml.SetNode("ACTIONSHEET/DETAILS/TOTAL", paymentAmount.ToString("F2"));
				    axml.SetNode("ACTIONSHEET/DETAILS/INSTALMENT", instAmount.ToString("F2"));
				    axml.SetNode("ACTIONSHEET/DETAILS/DUEDATE", day.ToString());
                    

				    if(row[CN.DateLastPaid] != DBNull.Value)
					    axml.SetNode("ACTIONSHEET/DETAILS/DATELASTPAID", ((DateTime)row[CN.DateLastPaid]).ToShortDateString());
				    else
					    axml.SetNode("ACTIONSHEET/DETAILS/DATELASTPAID", "");

                    cust = null;
                    pay = null;
                    emp = null;
				}

                customerRows = null;
			}

			catch(Exception ex)
			{
				logException(ex, Function);
            Response.Write(ex.Message);
			}
			
		}

		private void LoadAddressDetails(ActionSheetXML axml)
		{
			if(custAddresses != null)
			{
				string addType = "";
                DataRow[] addressRows = custAddresses.Select();

                foreach (DataRow addrRow in addressRows)
                {
					addType = ((string)addrRow["AddressType"]).Trim();
					switch(addType)
					{
						case "H":	axml.SetNode("ACTIONSHEET/HEADER/ADDR1", (string)addrRow[CN.Address1]);
							axml.SetNode("ACTIONSHEET/HEADER/ADDR2", (string)addrRow[CN.Address2]);
							axml.SetNode("ACTIONSHEET/HEADER/ADDR3", (string)addrRow[CN.Address3]);
							axml.SetNode("ACTIONSHEET/HEADER/POSTCODE", (string)addrRow[CN.PostCode]);
							axml.SetNode("ACTIONSHEET/HEADER/NOTES", (string)addrRow[CN.Notes]);
							
							if(((string)addrRow["Phone"]).Length > 0)
								axml.SetNode("ACTIONSHEET/HEADER/HOMETEL", (string)addrRow["DialCode"] + " " + (string)addrRow["Phone"]);
							
							break;
						case "D":	axml.SetNode("ACTIONSHEET/HEADER/DELIVERYADDR1", (string)addrRow[CN.Address1]);
							axml.SetNode("ACTIONSHEET/HEADER/DELIVERYADDR2", (string)addrRow[CN.Address2]);
							axml.SetNode("ACTIONSHEET/HEADER/DELIVERYADDR3", (string)addrRow[CN.Address3]);
							axml.SetNode("ACTIONSHEET/HEADER/DELIVERYPCODE", (string)addrRow[CN.PostCode]);
							if(((string)addrRow["Phone"]).Length > 0)
								axml.SetNode("ACTIONSHEET/HEADER/DELIVERYTEL", (string)addrRow["DialCode"] + " " + (string)addrRow["Phone"]);
							
							break;
						case "W":	axml.SetNode("ACTIONSHEET/HEADER/WORKADDR1", (string)addrRow[CN.Address1]);
							axml.SetNode("ACTIONSHEET/HEADER/WORKADDR2", (string)addrRow[CN.Address2]);
							axml.SetNode("ACTIONSHEET/HEADER/WORKADDR3", (string)addrRow[CN.Address3]);
							axml.SetNode("ACTIONSHEET/HEADER/WORKPCODE", (string)addrRow[CN.PostCode]);
							if(((string)addrRow["Phone"]).Length > 0)
								axml.SetNode("ACTIONSHEET/HEADER/WORKTEL", (string)addrRow["DialCode"] + " " + (string)addrRow["Phone"]);
							break;
						case "M":							
							if(((string)addrRow["Phone"]).Length > 0)
								axml.SetNode("ACTIONSHEET/HEADER/MOBILE", (string)addrRow["Phone"]);
							break;
                        case "M2":
                            if (((string)addrRow["Phone"]).Length > 0)
                                axml.SetNode("ACTIONSHEET/HEADER/MOBILE2", (string)addrRow["Phone"]);
                            break;
                        case "M3":
                            if (((string)addrRow["Phone"]).Length > 0)
                                axml.SetNode("ACTIONSHEET/HEADER/MOBILE3", (string)addrRow["Phone"]);
                            break;
                        case "M4":
                            if (((string)addrRow["Phone"]).Length > 0)
                                axml.SetNode("ACTIONSHEET/HEADER/MOBILE4", (string)addrRow["Phone"]);
                            break;
					}
				}

                addressRows = null;
			}
		}

		private void LoadEmployerDetails(ActionSheetXML axml)
		{
            DataRow[] employerRows = employer.Select();
            foreach (DataRow row in employerRows)
            {
                axml.SetNode("ACTIONSHEET/HEADER/NAME", (string)row[CN.EmployeeName]);
            }

            employerRows = null;
        }

		private void LoadOtherCustomers(ActionSheetXML axml)
		{
            DataRow[] customerRows = otherCustomers.Select();
            foreach (DataRow row in customerRows)
            {
				string custID = (string)row[CN.CustID];
				string name = (string)row[CN.FirstName] + " " + (string)row[CN.name] + " : " +
					(string)row[CN.Description];

				axml.AddOtherCustomers(row, name);
			}

            customerRows = null;
		}

		private void LoadFinancialDetails(DataTable dt, ActionSheetXML axml)
		{
			string notes = "";
            DataRow[] financialRows = dt.Select();

            foreach (DataRow row in financialRows)
            {
				if(row[CN.DateDel] != DBNull.Value)
					axml.SetNode("ACTIONSHEET/DETAILS/DELIVERYDATE", ((DateTime)row[CN.DateDel]).ToShortDateString());
				else
					axml.SetNode("ACTIONSHEET/DETAILS/DELIVERYDATE", "");
				
				axml.SetNode("ACTIONSHEET/DETAILS/ARREARSLEVEL", ((decimal)row[CN.ArrearsLevel2]).ToString("F2"));

				if(currStatus == "S")
				{
					decimal balance = (decimal)row[CN.BDWBalance] + (decimal)row[CN.BDWCharges];
					axml.SetNode("ACTIONSHEET/DETAILS/INTEREST", ((decimal)row[CN.BDWCharges]).ToString("F2"));
					axml.SetNode("ACTIONSHEET/DETAILS/BALANCE", balance.ToString("F2"));
				}
				else
					axml.SetNode("ACTIONSHEET/DETAILS/INTEREST", ((decimal)row[CN.Charges]).ToString("F2"));

				notes = (string)row[CN.Notes];
				if(notes.Length > 0)
					axml.SetNode("ACTIONSHEET/HEADER/INSTRUCTIONS", notes);
				else
					axml.SetNode("ACTIONSHEET/HEADER/INSTRUCTIONS", "NONE");
			}

            financialRows = null;
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
