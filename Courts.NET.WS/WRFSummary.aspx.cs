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
using STL.DAL;
using Blue.Cosacs.Repositories;

namespace STL.WS
{
	/// <summary>
	/// Summary description for WRFSummary.
	/// </summary>
	public partial class WRFSummary : CommonWebPage
	{
		//private BCountry _country = null;
		private string _countryCode = "";
		private DataSet codes = null;

        NumberFormatInfo LocalFormat = null;        //IP - 11/05/10 - UAT(126) UAT5.2.1.0 - Merged from 4.3

		/*
		public BCountry Country
		{
			get
			{
				if(_country==null)
					_country = new BCountry(_countryCode);
				return _country;
			}
		}
		*/

		protected void Page_Load(object sender, System.EventArgs e)
		{

			BProposal proposal = null;
			BCustomer customer = null;
			BDropDown drops = null;
			DataSet prop = null;
			DataSet spouse = null;
			DataSet accounts = null;
			DataSet custDetails = null;
			DataSet combined = null;
			
			try
			{
				string customerID = Request["customerID"];
				string accountNo = Request["accountNo"];
				string countryCode = _countryCode = Request["countryCode"];
                string storeType = Request["storeType"];
				string culture = Request["culture"];

				//Set the culture for currency formatting
				//Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                base.SetCulture();

                //Set printer display options for numbers and currency                  //IP - 11/05/10 - UAT(126) UAT5.2.1.0 - Merged from 4.3
                LocalFormat = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                LocalFormat.CurrencySymbol = (string)Country[CountryParameterNames.CurrencySymbolForPrint];


                BranchRepository br = new BranchRepository();
                string acctNo = accountNo.Substring(0, 3);
                var nonCourtsStoreTypes = br.GetNonCourtsStoreType(Convert.ToInt16(acctNo));

                RFSummaryXML rfxml = new RFSummaryXML(countryCode, storeType, nonCourtsStoreTypes.AshleyStore, nonCourtsStoreTypes.LuckyDollarStore, nonCourtsStoreTypes.RadioShackStore, nonCourtsStoreTypes.OmniStore);
				rfxml.Load();

				//retrieve all the requied data
				proposal = new BProposal();
				customer = new BCustomer();
				prop = proposal.GetProposalStage1(customerID, accountNo, "H");
				spouse = proposal.GetProposalStage1(customerID, accountNo, "S");
                // rdb we need extra fields get proposal stage 2                        //IP - 11/05/10 - UAT(126) UAT5.2.1.0 - Merged from 4.3
                DataSet prop2 = proposal.GetProposalStage2(customerID, DateTime.Now, accountNo, "H");
                accounts = customer.CustomerSearch(customerID, "", "", "", "", 100, 0, true, storeType, String.Empty);        //CR1084
				custDetails = customer.GetBasicCustomerDetails(null, null, customerID, accountNo, "H");
				combined = customer.GetRFCombinedDetails(customerID);

				XmlUtilities xml = new XmlUtilities();
				XmlDocument dropDowns = new XmlDocument();
				drops = new BDropDown();
				dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.MaritalStatus, new string[]{"MS1", "L"}));
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.PropertyType, new string[]{"PT1", "L"}));
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.EmploymentStatus, new string[]{"ES1", "L"}));

                //IP - 11/05/10 - UAT(126) UAT5.2.1.0 - Merged from 4.3
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.EthnicGroup, new string[] { "EG1", "L" }));
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ResidentialStatus, new string[] { "RS1", "L" }));
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Nationality, new string[] { "NA2", "L" }));

				codes = drops.GetDropDownData(dropDowns.DocumentElement);

				rfxml.SetNode("RFSUMMARY/CUSTDETAILS/CUSTID", customerID);
				rfxml.SetNode("RFSUMMARY/ACCTDETAILS/RFACCTNO", accountNo);
				rfxml.SetNode("RFSUMMARY/CURRENTDATE", DateTime.Today.ToShortDateString());
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

				foreach(DataTable dt in prop.Tables)
				{
					switch(dt.TableName)
					{
						case TN.Employment:
							LoadEmploymentDetails(dt, rfxml, "H");
							break;
						case TN.Bank:
							LoadBankDetails(dt, rfxml);
							break;
						case TN.Proposal:
							LoadProposalDetails(dt, rfxml);
							break;
						case TN.Customer:
							LoadSummaryAddressDetails(dt, rfxml);
							break;
						default:
							break;
					}
				}

				if(spouse!=null)
				{
					foreach(DataTable dt in spouse.Tables)
					{
						switch(dt.TableName)
						{
							case TN.Employment:
								LoadEmploymentDetails(dt, rfxml, "S");
								break;
							default:
								break;
						}
					}
				}

				foreach(DataTable dt in combined.Tables)
				{
					LoadRFCombined(dt, rfxml);
				}

				LoadAccounts(accounts, rfxml);

                LoadProposalSS2Details(prop2, rfxml);        //IP - 11/05/10 - UAT(126) UAT5.2.1.0 - Merged from 4.3

				Response.Write(rfxml.Transform());
			}
			catch(Exception ex)
			{
				logException(ex, Function);
				Response.Write(ex.Message);
			}
		}

		private void LoadAccounts(DataSet accounts, RFSummaryXML rfxml)
		{
			BAgreement agree = new BAgreement();
			BItem item = new BItem();
			foreach (DataTable dt in accounts.Tables)
			{
				int noAccounts = 0;
				foreach (DataRow row in dt.Rows)
				{
					string accountNo = (string)row["AccountNumber"];
					string accountType = (string)row["Type"];
					if(accountType==AT.ReadyFinance)
					{
                        DataSet agreement = agree.GetAgreement(null, null, accountNo, 1, false); //IP - 11/02/11 - Sprint 5.10 - #2978 - added null, null for conn, trans
						XmlNode lineItems = item.GetLineItems(null, null, accountNo, accountType, _countryCode, 1);
						rfxml.AddAccount(accountNo, agreement, lineItems, ref noAccounts);					
					}
				}
			}
		}

		private void LoadRFCombined(DataTable dt, RFSummaryXML rfxml)
		{
			foreach(DataRow row in dt.Rows)
			{
                //IP - 11/05/10 - UAT(126) UAT5.2.1.0 - Merged from 4.3 - Using LocalFormat
                rfxml.SetNode("RFSUMMARY/ACCTDETAILS/SPENDINGLIMIT", (Convert.ToDecimal(row[CN.TotalCredit])).ToString(DecimalPlaces, LocalFormat));
                rfxml.SetNode("RFSUMMARY/ACCTDETAILS/AVAILABLESPEND", (Convert.ToDecimal(row[CN.AvailableCredit])).ToString(DecimalPlaces, LocalFormat));
                rfxml.SetNode("RFSUMMARY/ACCTDETAILS/TOTALMONTHLYINSTALMENT", (Convert.ToDecimal(row[CN.TotalAllInstalments])).ToString(DecimalPlaces, LocalFormat));
			}
		}

		private void LoadSummaryAddressDetails(DataTable dt, RFSummaryXML rfxml)
		{
			foreach(DataRow row in dt.Rows)
			{
				rfxml.SetNode("RFSUMMARY/CUSTDETAILS/TIMEINCURRADDRESS", CalculateTimeSpan(DateTime.Today, (DateTime)row[CN.DateIn]));
				string type = (string)row[CN.PropertyType];		//needs translating into a readable description
				codes.Tables[TN.PropertyType].DefaultView.RowFilter = CN.Code+" = '"+type+"'";
				foreach(DataRowView r in codes.Tables[TN.PropertyType].DefaultView)
					rfxml.SetNode("RFSUMMARY/CUSTDETAILS/PROPTYPE", (string)r[CN.CodeDescription]);
			}
		}

        //IP - 11/05/10 - UAT(126) UAT5.2.1.0 - Merged from 4.3
        private void LoadProposalSS2Details(DataSet ds, RFSummaryXML rfxml)
        {
            // uat173 29/04/08 rdb adding for malaysia
            string gender = ds.Tables["Customer"].Rows[0][CN.Sex].ToString();
            if (gender == "M")
                gender = "Male";
            else if (gender == "F")
                gender = "Female";
            rfxml.SetNode("RFSUMMARY/CUSTDETAILS/GENDER", gender);


            string ethnicity = ds.Tables["Customer"].Rows[0]["Ethnicity"].ToString();		//needs translating into a readable description
            if (ethnicity!= null && ethnicity.Trim() != string.Empty)
            {
                codes.Tables[TN.EthnicGroup].DefaultView.RowFilter = CN.Code + " = '" + ethnicity + "'";
                rfxml.SetNode("RFSUMMARY/CUSTDETAILS/ETHNICITY", codes.Tables[TN.EthnicGroup].DefaultView[0][CN.CodeDescription].ToString());
            }

            string resStat = ds.Tables["Customer"].Rows[0]["ResidentialStatus"].ToString();
            if (resStat != null && resStat.Trim() != string.Empty)
            {
                codes.Tables[TN.ResidentialStatus].DefaultView.RowFilter = CN.Code + " = '" + resStat + "'";
                rfxml.SetNode("RFSUMMARY/CUSTDETAILS/RESIDENTIALSTATUS", codes.Tables[TN.ResidentialStatus].DefaultView[0][CN.CodeDescription].ToString());
            }
            //Mortgage = MonthlyRent????
            rfxml.SetNode("RFSUMMARY/CUSTDETAILS/MORTGAGE", ds.Tables["Customer"].Rows[0]["MonthlyRent"].ToString());
            rfxml.SetNode("RFSUMMARY/CUSTDETAILS/VEHICLEREG", ds.Tables["Proposal"].Rows[0]["VehicleRegistration"].ToString());
            //  employer address
            string empAddr = string.Format("{0}, {1}, {2}, {3}",
                ds.Tables["Proposal"].Rows[0]["EAddress1"],
                ds.Tables["Proposal"].Rows[0]["EAddress2"],
                ds.Tables["Proposal"].Rows[0]["ECity"],
                ds.Tables["Proposal"].Rows[0]["EPostcode"].ToString().Trim());
            while (empAddr.Contains(", , "))
                empAddr = empAddr.Replace(", , ", ", ");
            if (empAddr.EndsWith(", "))
                empAddr = empAddr.Substring(0, empAddr.Length - 2);

            rfxml.SetNode("RFSUMMARY/CUSTDETAILS/EMPLOYERADDRESS", empAddr);


        }

		private void LoadProposalDetails(DataTable dt, RFSummaryXML rfxml)
		{
			foreach(DataRow row in dt.Rows)
			{
				string maritalStat = (string)row[CN.MaritalStatus];		//needs translating into a readable description
				codes.Tables[TN.MaritalStatus].DefaultView.RowFilter = CN.Code+" = '"+maritalStat+"'";
				foreach(DataRowView r in codes.Tables[TN.MaritalStatus].DefaultView)
					rfxml.SetNode("RFSUMMARY/CUSTDETAILS/MARITALSTAT", (string)r[CN.CodeDescription]);
				rfxml.SetNode("RFSUMMARY/CUSTDETAILS/DEPENDENTS", ((int)row[CN.Dependants]).ToString());
				if(row[CN.MonthlyIncome] != DBNull.Value)
                    rfxml.SetNode("RFSUMMARY/CUSTDETAILS/MONTHLYINCOME", ((double)row[CN.MonthlyIncome]).ToString(DecimalPlaces, LocalFormat));  //IP - 11/05/10 - UAT(126) UAT5.2.1.0 - Merged from 4.3 - Added LocalFormat
				else
                    rfxml.SetNode("RFSUMMARY/CUSTDETAILS/MONTHLYINCOME", (0).ToString(DecimalPlaces, LocalFormat)); //IP - 11/05/10 - UAT(126) UAT5.2.1.0 - Merged from 4.3 - Added LocalFormat
				rfxml.SetNode("RFSUMMARY/CUSTDETAILS/PREVADDRESS/ADDRESS1", (string)row[CN.PAddress1]);
				rfxml.SetNode("RFSUMMARY/CUSTDETAILS/PREVADDRESS/ADDRESS2", (string)row[CN.PAddress2]);
				rfxml.SetNode("RFSUMMARY/CUSTDETAILS/PREVADDRESS/ADDRESS3", (string)row[CN.PCity]);
				rfxml.SetNode("RFSUMMARY/CUSTDETAILS/PREVADDRESS/POSTCODE", (string)row[CN.PPostCode]);
				rfxml.SetNode("RFSUMMARY/CUSTDETAILS/EMPLOYER", (string)row[CN.EmployeeName]);

                //IP - 11/05/10 - UAT(126) UAT5.2.1.0 - Merged from 4.3
                string nationality = row[CN.Nationality].ToString();		//needs translating into a readable description
                if (nationality != null && nationality.Trim() != string.Empty)
                {
                    codes.Tables[TN.Nationality].DefaultView.RowFilter = CN.Code + " = '" + nationality + "'";
                    rfxml.SetNode("RFSUMMARY/CUSTDETAILS/NATIONALITY", codes.Tables[TN.Nationality].DefaultView[0][CN.CodeDescription].ToString());
                }

                rfxml.SetNode("RFSUMMARY/CUSTDETAILS/ADDITIONALINCOME", row[CN.AdditionalIncome].ToString());
                //loans/credit cards = otherPayments
                if (row[CN.Commitments2] != DBNull.Value)
                    rfxml.SetNode("RFSUMMARY/CUSTDETAILS/LOANS", (Convert.ToDouble(row[CN.Commitments2])).ToString(DecimalPlaces, LocalFormat));
                //  Utilities
                if (row[CN.Commitments1] != DBNull.Value)
                    rfxml.SetNode("RFSUMMARY/CUSTDETAILS/UTILITIES", (Convert.ToDouble(row[CN.Commitments1])).ToString(DecimalPlaces, LocalFormat));
			}
		}

		private void LoadBankDetails(DataTable dt, RFSummaryXML rfxml)
		{
			foreach(DataRow row in dt.Rows)
			{
				rfxml.SetNode("RFSUMMARY/CUSTDETAILS/BANKNAME", (string)row[CN.BankName]);
				rfxml.SetNode("RFSUMMARY/CUSTDETAILS/BANKACCTNO", (string)row[CN.BankAccountNo]);
				rfxml.SetNode("RFSUMMARY/CUSTDETAILS/TIMEATBANK", CalculateTimeSpan(DateTime.Today, (DateTime)row[CN.BankAccountOpened]));
			}
		}

		private void LoadEmploymentDetails(DataTable dt, RFSummaryXML rfxml, string relation)
		{
			DateTime currDate = DateTime.Now;
			DateTime dateEmployed;

			foreach(DataRow row in dt.Rows)
			{
				
				if(relation=="H")
				{
					string stat = (string)row[CN.EmploymentStatus];		//needs translating into a readable description
					codes.Tables[TN.EmploymentStatus].DefaultView.RowFilter = CN.Code+" = '"+stat+"'";
					foreach(DataRowView r in codes.Tables[TN.EmploymentStatus].DefaultView)
						rfxml.SetNode("RFSUMMARY/CUSTDETAILS/EMPLOYMENTSTAT", (string)r[CN.CodeDescription]);
					dateEmployed = (DateTime)row[CN.DateEmployed];
					rfxml.SetNode("RFSUMMARY/CUSTDETAILS/TIMECURREMPLOYMENT", CalculateTimeSpan(currDate, dateEmployed));
                    rfxml.SetNode("RFSUMMARY/CUSTDETAILS/OCCUPATION", row[CN.Occupation].ToString()); //(string)row[CN.JobTitle]);  //IP - 11/05/10 - UAT(126) UAT5.2.1.0 - Merged from 4.3
					rfxml.SetNode("RFSUMMARY/CUSTDETAILS/EMPLOYERTEL", ((string)row[CN.PersDialCode]).Trim() + " " + (string)row[CN.PersTel]);
				}
				else
				{
					string stat = (string)row[CN.EmploymentStatus];		//needs translating into a readable description
					codes.Tables[TN.EmploymentStatus].DefaultView.RowFilter = CN.Code+" = '"+stat+"'";
					foreach(DataRowView r in codes.Tables[TN.EmploymentStatus].DefaultView)
						rfxml.SetNode("RFSUMMARY/CUSTDETAILS/SPOUSE/EMPLOYMENTSTAT", (string)r[CN.CodeDescription]);
                    rfxml.SetNode("RFSUMMARY/CUSTDETAILS/SPOUSE/OCCUPATION", row[CN.Occupation].ToString());//(string)row[CN.JobTitle]);    //IP - 11/05/10 - UAT(126) UAT5.2.1.0 - Merged from 4.3
				}
			}
		}

		private void LoadCustDetails(DataTable dt, RFSummaryXML rfxml)
		{
			foreach(DataRow row in dt.Rows)
			{
				rfxml.SetNode("RFSUMMARY/CUSTDETAILS/TITLE", (string)row[CN.Title]+" ");
				rfxml.SetNode("RFSUMMARY/CUSTDETAILS/FIRSTNAME", (string)row[CN.FirstName]+" ");
				rfxml.SetNode("RFSUMMARY/CUSTDETAILS/LASTNAME", (string)row[CN.LastName]);
				rfxml.SetNode("RFSUMMARY/CUSTDETAILS/DOB", ((DateTime)row[CN.DOB]).ToShortDateString());
			}
		}

		private void LoadAddressDetails(DataTable dt, RFSummaryXML rfxml)
		{
			string addType = "";
			foreach(DataRow row in dt.Rows)
			{
				addType = ((string)row["AddressType"]).Trim();
				switch(addType)
				{
					case "H":	rfxml.SetNode("RFSUMMARY/CUSTDETAILS/CURRENTADDRESS/ADDRESS1", (string)row["Address1"], false);
						rfxml.SetNode("RFSUMMARY/CUSTDETAILS/CURRENTADDRESS/ADDRESS2", (string)row["Address2"], false);
						rfxml.SetNode("RFSUMMARY/CUSTDETAILS/CURRENTADDRESS/ADDRESS3", (string)row["Address3"], false);
						rfxml.SetNode("RFSUMMARY/CUSTDETAILS/CURRENTADDRESS/POSTCODE", (string)row["PostCode"], false);
						rfxml.SetNode("RFSUMMARY/CUSTDETAILS/HOMETEL", ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"], false);
						break;
					case "W":	rfxml.SetNode("RFSUMMARY/CUSTDETAILS/WORKTEL", ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"], false);
						break;
					case "M":	rfxml.SetNode("RFSUMMARY/CUSTDETAILS/MOBILETEL", ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"], false);
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
