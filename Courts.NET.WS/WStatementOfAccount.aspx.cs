using System;
using System.Data;
using System.Xml;
using STL.BLL;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.DAL;

namespace STL.WS
{
	public partial class WStatementOfAccount : CommonWebPage
	{
		XmlNode transItems = null;
		XmlNode transItem = null;
		DataTable finTrans = null;
		XmlNode pageNode = null;
		XmlNode headerNode = null;
		int transPerPage = 28;
		double numPages = 0;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{

				string customerID = Request[CN.CustomerID];
				string accountNo = Request[CN.AccountNumber];
				string culture = Request[CN.Culture];
				string countryCode = Request["countryCode"];
				string accountType = Request[CN.AccountType];
				//decimal RFAgreementTotal = 0;
				

				//Set the culture for currency formatting
				//Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                base.SetCulture();
				
				StatementOfAccountXML sxml = new StatementOfAccountXML(countryCode);
				sxml.Load();				

				#region retrieve required data

				BCustomer customer = new BCustomer();
				DataSet custDetails = customer.GetBasicCustomerDetails(null, null, customerID, accountNo, "H");

				DInstalPlan ip = new DInstalPlan();
				ip.Populate(null, null, accountNo, 1);

				DAccount acct = new DAccount(null, null, accountNo);

				// Only print the one account even if it is RF
				//if(accountType==AT.ReadyFinance)
				//	RFAgreementTotal = acct.GetRFAgreementTotal(customerID);

				DAgreement agree = new DAgreement(null, null, accountNo, 1);

				DFinTrans ft = new DFinTrans();
				
				ft.GetByAcctNo(null, null, accountNo);
				finTrans = ft.FinTrans;

                #endregion

				headerNode = sxml.GetNode("STATEMENT/PAGE/HEADER");
				pageNode = sxml.GetNode("STATEMENT/PAGE");

				headerNode.SelectSingleNode("COUNTRYNAME").InnerText = (string)Country[CountryParameterNames.CountryName];
				headerNode.SelectSingleNode("DATE").InnerText = DateTime.Today.ToShortDateString();
				headerNode.SelectSingleNode("ACCTNO").InnerText = accountNo;
				
				headerNode.SelectSingleNode("INSTALMENT").InnerText = ip.InstalmentAmount.ToString(DecimalPlaces);
				headerNode.SelectSingleNode("DUEDATE").InnerText = ip.DateFirst.Day.ToString();
				headerNode.SelectSingleNode("AGREEMENTTOTAL").InnerText = agree.AgreementTotal.ToString(DecimalPlaces); 
                headerNode.SelectSingleNode("ARREARS").InnerText = acct.Arrears.ToString(DecimalPlaces); 

				transItems = sxml.GetNode("STATEMENT/PAGE/TRANSACTIONS");
				transItem = transItems.RemoveChild(transItems.SelectSingleNode("TRANSACTION"));
	

				foreach(DataTable dt in custDetails.Tables)
				{
					if(dt.TableName=="BasicDetails")
					{
						foreach(DataRow row in dt.Rows)
						{
							string name = (string)row[CN.Title] + " " + (string)row[CN.FirstName] + " " + (string)row[CN.LastName];
							headerNode.SelectSingleNode("CUSTOMERNAME").InnerText = name;
						}
					}
					if(dt.TableName=="CustomerAddresses")
					{
						foreach(DataRow row in dt.Rows)
						{
							if(((string)row[CN.AddressType]).Trim()=="H" &&
								(string)row[CN.Category] == "CA1" )
							{
								headerNode.SelectSingleNode("ADDR1").InnerText = (string)row[CN.Address1];
								headerNode.SelectSingleNode("ADDR2").InnerText = (string)row[CN.Address2];
								headerNode.SelectSingleNode("ADDR3").InnerText = (string)row[CN.Address3];
								headerNode.SelectSingleNode("POSTCODE").InnerText = (string)row[CN.PostCode];
							}
						}
					}
				}

				decimal bal = AddTransactions(sxml, accountType, accountNo, finTrans);

				foreach(XmlNode h in sxml.GetNode("STATEMENT").SelectNodes("//HEADER"))
					h.SelectSingleNode("OUTSTANDINGBAL").InnerText = bal.ToString(DecimalPlaces);

//#if(XMLTRACE)
//				logMessage(sxml.Xml, User.Identity.Name, EventLogEntryType.Information);
//#endif

				Response.Write(sxml.Transform());

			}
			catch(Exception ex)
			{
				logException(ex, Function);
				Response.Write(ex.Message);
			}
		}

		private decimal AddTransactions(StatementOfAccountXML sxml, 
									string accountType, 
									string acctNo,
									DataTable finTrans)
		{
			int i=0;
			int pageNum = 1;

			// Only print the one account even if it is RF
			//if(accountType == AT.ReadyFinance)
			//	finTrans = MergeRFTransactions(finTrans);

			numPages = Math.Ceiling(Convert.ToDouble(finTrans.Rows.Count) / Convert.ToDouble(transPerPage));
			if(numPages<=1)
				pageNode.SelectSingleNode("LAST").InnerText = Boolean.TrueString;
			else
				pageNode.SelectSingleNode("LAST").InnerText = Boolean.FalseString;

			DTransType tt = new DTransType();
			decimal openingBal = 0;
			finTrans.DefaultView.Sort = CN.DateTrans + " ASC";

			foreach(DataRowView row in finTrans.DefaultView)
			{
				XmlNode trans = transItem.CloneNode(true);
				trans.SelectSingleNode("DATE").InnerText = ((DateTime)row[CN.DateTrans]).ToShortDateString();

				if((string)row[CN.TransTypeCode] == "XFR")
				{
					DFinTrans ft = new DFinTrans();
					string reference = ft.GetGiftVoucherReference(acctNo, Convert.ToInt32(row[CN.TransRefNo]));
					trans.SelectSingleNode("TRANSTYPE").InnerText = tt.GetDescription((string)row[CN.TransTypeCode]) + " (REF: " + reference + ")";
				}
				else
					trans.SelectSingleNode("TRANSTYPE").InnerText = tt.GetDescription((string)row[CN.TransTypeCode]);
				
				decimal transVal = (decimal)row[CN.TransValue];
				openingBal -= transVal;
				if(transVal < 0)
					trans.SelectSingleNode("CREDIT").InnerText = (-transVal).ToString(DecimalPlaces);
				else
					trans.SelectSingleNode("DEBIT").InnerText = transVal.ToString(DecimalPlaces);
				trans.SelectSingleNode("BALANCE").InnerText = openingBal.ToString(DecimalPlaces);
				if(openingBal>=0)
					trans.SelectSingleNode("BALANCE").InnerText += "CR";
				else
					trans.SelectSingleNode("BALANCE").InnerText = trans.SelectSingleNode("BALANCE").InnerText.Substring(1,trans.SelectSingleNode("BALANCE").InnerText.Length -1 ); 
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
					sxml.GetNode("STATEMENT").AppendChild(newPage);
				}
			}
			return -openingBal;
		}

        //private DataTable MergeRFTransactions(DataTable transactions)
        //{
        //    DataView transView = transactions.DefaultView;
        //    DataTable merged = transactions.Clone();
				
        //    DataRow prevRow = null;
        //    foreach(DataRowView drv in transView)
        //    {
        //        if( prevRow==null ||
        //            (((DateTime)prevRow[CN.DateTrans]).Date != ((DateTime)drv[CN.DateTrans]).Date ||		//this is a new row
        //            (string)prevRow[CN.TransTypeCode]  != (string)drv[CN.TransTypeCode]) )
        //        {
        //            DataRow row = merged.NewRow();
        //            row[CN.acctno] = drv[CN.acctno];
        //            row[CN.DateTrans] = drv[CN.DateTrans];
        //            row[CN.TransRefNo] = drv[CN.TransRefNo];
        //            row[CN.TransTypeCode] = drv[CN.TransTypeCode];
        //            row[CN.TransValue] = drv[CN.TransValue];
        //            merged.Rows.Add(row);
        //            prevRow = row;
        //        }
        //        else
        //        {
        //            prevRow[CN.TransValue] = (decimal)prevRow[CN.TransValue] + (decimal)drv[CN.TransValue];
        //        }
        //    }
        //    return merged;
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
