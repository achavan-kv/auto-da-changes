using System;
using System.Data;
using System.Globalization;
using STL.BLL;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.StoreInfo;
using STL.Common.Constants.TableNames;
using STL.DAL;
using Blue.Cosacs.Repositories;

namespace STL.WS
{
	public partial class WWarrantyContract : CommonWebPage
	{
		NumberFormatInfo LocalFormat = null;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			DataSet custDetails = null;
			DataSet contractDetails = null;
			WarrantyContractXML wxml = null;
			try
			{
				string customerID = Request["customerID"];
				string accountNo = Request["accountNo"];
				string countryCode = Request["countryCode"];
				string culture = Request["culture"];
				string contractNo = Request["contractNo"];
				int agreementNo = Convert.ToInt32(Request["agreementNo"]);
				bool multiple = Convert.ToBoolean(Request["multiple"]);

				//Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                base.SetCulture();

				LocalFormat = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
				LocalFormat.CurrencySymbol = CachedItems.GetCountryParamters(countryCode).GetCountryParameterValue<string>(CountryParameterNames.CurrencySymbolForPrint);

				wxml = new WarrantyContractXML(countryCode);
				wxml.Load();

                // 13/11/07 rdb support non-courts agreements
                DBranch branch = new DBranch();
                string acctNo = accountNo.Substring(0, 3);
                string storeType = branch.GetStoreType(Convert.ToInt16(acctNo));

                BranchRepository br = new BranchRepository();
                var nonCourtsStoreTypes = br.GetNonCourtsStoreType(Convert.ToInt16(acctNo));

                //This can be cleaned up massively just using the display type column
                string xsltFilename = CachedItems.GetCountryParamters(countryCode).GetCountryParameterValue<string>(CountryParameterNames.WarrantyStylesheet);
                if (storeType == StoreType.NonCourts && (nonCourtsStoreTypes.LuckyDollarStore != null && nonCourtsStoreTypes.LuckyDollarStore == true))
                {
                    xsltFilename = xsltFilename.Replace(".xsl", "_LD.xsl");
                }
                else if (storeType == StoreType.NonCourts && (nonCourtsStoreTypes.AshleyStore != null && nonCourtsStoreTypes.AshleyStore == true))
                {
                    xsltFilename = xsltFilename.Replace(".xsl", "_Ashley.xsl");
                }
                else if (storeType == StoreType.NonCourts && (nonCourtsStoreTypes.RadioShackStore != null && nonCourtsStoreTypes.RadioShackStore == true))
                {
                    xsltFilename = xsltFilename.Replace(".xsl", "_Radioshack.xsl");
                }
                else if(storeType == StoreType.NonCourts && (nonCourtsStoreTypes.DisplayType == "Omni"))
                {
                    xsltFilename = xsltFilename.Replace(".xsl", "_Omni.xsl");
                }

                wxml.SetXsltPath(xsltFilename);

				char[] acctSplit = accountNo.ToCharArray();

				for(int i=0; i<acctSplit.Length; i++)
					wxml.SetNode("CONTRACT/ACCTNO"+(i+1), acctSplit[i].ToString());

				wxml.SetNode("CONTRACT/COUNTRYNAME", CachedItems.GetCountryParamters(countryCode).GetCountryParameterValue<object>(CountryParameterNames.CountryName).ToString());
				wxml.SetNode("CONTRACT/ACCTNO", accountNo);
				wxml.SetNode("CONTRACT/CONTRACTNO", contractNo);
				wxml.SetNode("CONTRACT/TODAY", DateTime.Today.ToShortDateString());
				wxml.SetNode("CONTRACT/SPACE", " ");
				wxml.SetNode("CONTRACT/WARRANTYCREDIT", CachedItems.GetCountryParamters(countryCode).GetCountryParameterValue<object>(CountryParameterNames.CreditWarrantyDays).ToString());
				wxml.SetNode("CONTRACT/CUSTOMERID", customerID);

				BCustomer cust = new BCustomer();
				custDetails = cust.GetBasicCustomerDetails(null, null, customerID, accountNo, "H");

				foreach(DataTable dt in custDetails.Tables)
				{
					switch(dt.TableName)
					{
						case "BasicDetails":	
							LoadCustDetails(dt, wxml);
							break;
						case TN.CustomerAddresses:		
							LoadAddressDetails(dt, wxml);
							break;
						default:
							break;
					}
				}


				BItem item = new BItem();
				contractDetails = item.GetWarrantyContractDetails(accountNo, agreementNo, contractNo);
				LoadContractDetails(contractDetails, wxml);

				if(multiple)
				{
					wxml.CreateCopies(Convert.ToInt16(Country[CountryParameterNames.WarrantyCreditCopy]),
									Convert.ToInt16(Country[CountryParameterNames.WarrantyCustCopy]),
									Convert.ToInt16(Country[CountryParameterNames.WarrantyHOCopy]));
				}
				else
				{
					wxml.CreateCopies(1,0,0);
				}
//#if(XMLTRACE)
//				logMessage(wxml.Xml, User.Identity.Name, EventLogEntryType.Information);
//#endif				

				Response.Write(wxml.Transform());
			}
			catch(Exception ex)
			{
				logException(ex, Function);
				Response.Write(ex.Message);
			}
		}

		private void LoadContractDetails(DataSet ds, WarrantyContractXML wxml)
		{
			foreach (DataTable dt in ds.Tables)
				foreach (DataRow row in dt.Rows)
				{
                    //short twelve = 12;
                    //double warrantyLength = Convert.ToDouble(row[CN.WarrantyLength]);
                    //warrantyLength *= twelve;
                    double warrantyLength = Convert.ToDouble(row[CN.WarrantyLength]);               //IP - 26/09/11 - RI - #8228 - CR8201 - Warrantylength now held in months on table. Previously held in years.
               
					DateTime plannedDel = (DateTime)row[CN.DateReqDel];
					if(plannedDel == DateTime.MinValue.AddYears(1899))
						plannedDel = DateTime.Today;

					wxml.SetNode("CONTRACT/ITEMNO", (string)row[CN.ItemNo]);
					wxml.SetNode("CONTRACT/WARRANTYNO", (string)row[CN.WarrantyNo]);
					wxml.SetNode("CONTRACT/STORENO", ((short)row[CN.BranchNo]).ToString());
					wxml.SetNode("CONTRACT/BRANCHNAME", (string)row[CN.BranchName]);
					wxml.SetNode("CONTRACT/SOLDBY", ((int)row[CN.EmployeeNo]).ToString());
					wxml.SetNode("CONTRACT/DATEOFPURCHASE", ((DateTime)row[CN.AgreementDate]).ToShortDateString());

                    //IP - 22/09/11 - RI - #8228 - CR8201 
                    if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")
                    {
                        wxml.SetNode("CONTRACT/ITEMDESC1", (string)row[CN.ItemDescr1] + " " + (string)row[CN.Brand] + " " + (string)row[CN.Style]);
                    }
                    else
                    {
                        wxml.SetNode("CONTRACT/ITEMDESC1", (string)row[CN.ItemDescr1]);
                    }

					wxml.SetNode("CONTRACT/ITEMDESC2", (string)row[CN.ItemDescr2]);
					wxml.SetNode("CONTRACT/MANUFACTURERWARRANTYLENGTH", "12");		//TO DO KF to confirm this is static
					wxml.SetNode("CONTRACT/ITEMPRICE", ((double)row[CN.ItemPrice]).ToString(DecimalPlaces,LocalFormat));
					wxml.SetNode("CONTRACT/WARRANTYPRICE", ((double)row[CN.WarrantyPrice]).ToString(DecimalPlaces,LocalFormat));
					wxml.SetNode("CONTRACT/PLANNEDDELIVERY", plannedDel.ToShortDateString());
					wxml.SetNode("CONTRACT/STARTOFEXTENDEDWARRANTY", (plannedDel.AddMonths(12)).ToShortDateString());	//TO DO KF to confirm this is static
					wxml.SetNode("CONTRACT/EXPIRYOFWARRANTY", GetResource("M_WARRANTYEXPIRY", new object[] {warrantyLength}));
					wxml.SetNode("CONTRACT/WARRANTYDESC1", (string)row[CN.WarrantyDescr1]);
					wxml.SetNode("CONTRACT/WARRANTYDESC2", (string)row[CN.WarrantyDescr2]);
					wxml.SetNode("CONTRACT/SOLDBYNAME", (string)row[CN.EmployeeName]);
					wxml.SetNode("CONTRACT/TERMSTYPE", (string)row[CN.TermsType]);
				}
		}

		private void LoadCustDetails(DataTable dt, WarrantyContractXML wxml)
		{
			foreach(DataRow row in dt.Rows)
			{
				wxml.SetNode("CONTRACT/TITLE", (string)row[CN.Title]+" ");
				wxml.SetNode("CONTRACT/FIRSTNAME", (string)row[CN.FirstName]+" ");
				wxml.SetNode("CONTRACT/LASTNAME", (string)row[CN.LastName]);
			}
		}

		private void LoadAddressDetails(DataTable dt, WarrantyContractXML wxml)
		{
			string addType = "";
			foreach(DataRow row in dt.Rows)
			{
				addType = ((string)row["AddressType"]).Trim();
				switch(addType)
				{
					case "H":	wxml.SetNode("CONTRACT/ADDRESS1", (string)row["Address1"], false);
								wxml.SetNode("CONTRACT/ADDRESS2", (string)row["Address2"], false);
								wxml.SetNode("CONTRACT/ADDRESS3", (string)row["Address3"], false);
								wxml.SetNode("CONTRACT/POSTCODE", (string)row["PostCode"], false);
								wxml.SetNode("CONTRACT/HOMETEL", ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"], false);
						break;
					case "W":	wxml.SetNode("CONTRACT/WORKTEL", ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"], false);
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
