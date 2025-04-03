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


namespace STL.WS
{
	/// <summary>
	/// Summary description for WTaxInvoice.
	/// </summary>
	public partial class WPaidAndTakenTaxInvoice : CommonWebPage
	{
		NumberFormatInfo LocalFormat = null;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			DataSet custDetails = null;
			DataSet branchDetails = null;
			XmlNode lineItems = null;            
            DateTime createdOnDt;

            try
			{
				//Extract variables from the QueryString
				string acctNo = Request["acctNo"];
				short branch = Convert.ToInt16(Request["branch"]);
				string customerID = Request["customerID"];
				string countryCode = Request["country"];
				string culture = Request["culture"];
				string accountType = Request["accountType"];
				string lineItemStr = Request["lineItems"];
                string reference = Request["agrmtno"];
				bool collection = Convert.ToBoolean(Request["collection"]);
				int buffNo = Convert.ToInt32(Request["buffno"]);
				bool taxExempt = false;
				bool multiple = Convert.ToBoolean(Request["multiple"]);
				string termsType = "";
                bool ReprintInvoice = Convert.ToBoolean(Request["ReprintInvoice"]);                
                int versionNo = Convert.ToInt32(Request["versionNo"]);
                string agreementinvoicenumber = Request["agreementinvoicenumber"];               
                string cust_name = string.Empty;
                Decimal storeCardLimit = 0, storeCardAvailable = 0;
                 

                XmlDocument doc  = new XmlDocument();
				doc.LoadXml(lineItemStr);
				lineItems = doc.DocumentElement;

				//Set the culture for currency formatting
				//Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                base.SetCulture();

				LocalFormat = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
				LocalFormat.CurrencySymbol = (string)Country[CountryParameterNames.CurrencySymbolForPrint];



                //            //Build the XML document
                TaxInvoiceXML txml = new TaxInvoiceXML((string)Country[CountryParameterNames.TaxType], (string)Country[CountryParameterNames.AgreementTaxType], countryCode,false );
                txml.Load();

                /* find out if account is tax exempt */
                BAccount acct = new BAccount();
				taxExempt = acct.IsTaxExempt(null, null, acctNo,null);

				if(buffNo==0)
				{
                    using (var conn = new SqlConnection(STL.DAL.Connections.Default))
                    {
                        conn.Open();
                        using (var trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            BBranch br = new BBranch();
                            buffNo = br.GetBuffNo(conn, trans, branch);
                        }
                    }
				}
				txml.SetNode("TAXINVOICE/HEADER/BUFFNO", buffNo.ToString());

				//retrieve the basic customer details
				BCustomer cust = new BCustomer();
				custDetails = cust.GetBasicCustomerDetailsForReprint(null, null, reference);

                if (custDetails != null)
                {
                    //foreach (DataTable dt in custDetails.Tables)
                    //{
                    DataTable dt = custDetails.Tables[0];
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            LoadCustDetails(dt, txml);
                            if (ReprintInvoice == true)
                            {
                                cust_name = Convert.ToString(dt.Rows[0]["FirstName"]) + " " + Convert.ToString(dt.Rows[0]["LastName"]);
                                storeCardLimit = Convert.ToDecimal(dt.Rows[0]["RFCreditLimit"]);
                                storeCardAvailable = Convert.ToDecimal(dt.Rows[0]["AvailableSpend"]);
                            }
                        }
                    }
                    //}
                }

                //Retrieve the branch information
                BBranch b = new BBranch();
				branchDetails = b.GetBranchAddress(branch, 1);
				foreach(DataTable dt in branchDetails.Tables)
					if(dt.TableName == "BranchDetails")
						foreach(DataRow r in dt.Rows)
						{
							txml.SetNode("TAXINVOICE/HEADER/BRANCHNAME", (string)r["BranchName"]);
							txml.SetNode("TAXINVOICE/HEADER/BRANCHADDR1", (string)r["BranchAddr1"]);
							txml.SetNode("TAXINVOICE/HEADER/BRANCHADDR2", (string)r["BranchAddr2"]);
							txml.SetNode("TAXINVOICE/HEADER/BRANCHADDR3", (string)r["BranchAddr3"]);
							txml.SetNode("TAXINVOICE/HEADER/SERIALNO", (string)r["Hissn"]);
						}

				txml.SetNode("TAXINVOICE/HEADER/ACCTNO", ":" + acctNo);
				txml.SetNode("TAXINVOICE/HEADER/TAXNAME", (string)Country[CountryParameterNames.TaxName]);
				txml.SetNode("TAXINVOICE/HEADER/DATE", DateTime.Today.ToShortDateString());
				txml.SetNode("TAXINVOICE/HEADER/CUSTOMERID", customerID);
				txml.SetNode("TAXINVOICE/HEADER/COLLECTION", collection.ToString());

                //BOC Raj  CR 2018-13: To Print Details
                if (ReprintInvoice == true)
                {
                    txml.SetNode("TAXINVOICE/HEADER/NOW", ":" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));                    

                    DataSet dsAgreement = new BAgreement().GetAgreement(null, null, acctNo, Convert.ToInt32(reference), false);  //CR 2018-13: To Print Details
                    string agreemtInvNum = Convert.ToString(dsAgreement.Tables[TN.Agreements].Rows[0]["AgreementInvoiceNumber"]);
                    DataSet dsInvoiceDetails = cust.GetInvoiceDetails(acctNo, Convert.ToString(reference), agreemtInvNum);
                    DataTable dtInvoiceDetails = dsInvoiceDetails.Tables["InvoiceDetails"];
                    DataTable dtInvoicePaymentDetails = dsInvoiceDetails.Tables["InvoicePaymentDetails"];
                    
                    DataRow drInvDet = dtInvoiceDetails.Rows[0];
                    string invoiceNO = agreemtInvNum != string.Empty ? agreemtInvNum : Convert.ToString(drInvDet["AgreementInvoiceNumber"]);
                    string invoice_branch = invoiceNO.Substring(0, 3);
                    txml.SetNode("TAXINVOICE/HEADER/BRANCHNO", ":" + (invoice_branch == string.Empty ? Convert.ToString(branch) : invoice_branch));

                    string salesman = Convert.ToString(drInvDet["soldbyID"]) + " - " + Convert.ToString(drInvDet["soldByName"]);
                    string cashier = Convert.ToString(drInvDet["createdByID"]) + " - " + Convert.ToString(drInvDet["createdByName"]);
                    txml.SetNode("TAXINVOICE/HEADER/INVOICENO", ":" + invoiceNO);
                    if (Convert.ToDateTime(drInvDet["createdOn"]) != null)
                    {
                        createdOnDt = Convert.ToDateTime(drInvDet["createdOn"]);
                        txml.SetNode("TAXINVOICE/HEADER/INVOICEDATE", ":" + createdOnDt.ToString("dd-MMM-yyyy"));
                    }
                    txml.SetNode("TAXINVOICE/HEADER/REPRINTCOPY", " - Reprint Copy");
                    txml.SetNode("TAXINVOICE/HEADER/CASHIER", ":" + cashier);
                    txml.SetNode("TAXINVOICE/HEADER/SALESMAN", ":" + salesman);
                    txml.SetNode("TAXINVOICE/HEADER/CUSTOMERNAME", ":" + cust_name);
                    txml.SetNode("TAXINVOICE/HEADER/ACCTBLNC", ":" + LocalFormat.CurrencySymbol + storeCardLimit.ToString("N", LocalFormat));
                    txml.SetNode("TAXINVOICE/HEADER/AVAILABLESPEND", ":" + LocalFormat.CurrencySymbol + storeCardAvailable.ToString("N", LocalFormat));
                    // txml.SetNode("TAXINVOICE/HEADER/REGNO", regNo); 
                    string descr = dtInvoicePaymentDetails.Rows[0]["payMethod"].ToString();
                    Decimal amt = Convert.ToDecimal(Math.Abs(Convert.ToDecimal(dtInvoicePaymentDetails.Rows[0]["amount"])));
                    Decimal total_amt = txml.AddPayMethods(dtInvoicePaymentDetails, LocalFormat, countryCode, ReprintInvoice, Convert.ToInt32(reference));
                    txml.SetNode("TAXINVOICE/PAY2/TOTALAMTPAID", LocalFormat.CurrencySymbol + " " + total_amt.ToString("N", LocalFormat));

                }
                //EOC Raj CR 2018-13: To Print Details

                decimal items = 1;
				if(lineItems.SelectNodes("//Item[@Type='Warranty' or @Type='KitWarranty' and @Quantity!='0']").Count > 0)
				{
					BAccount a = new BAccount();
					a.Populate(acctNo);
					termsType = a.TermsType;
				}

                if (ReprintInvoice == true)
                {
                    txml.AddLineItemshVersion(termsType, lineItems, collection, taxExempt, LocalFormat, countryCode, ReprintInvoice, ref items);
                }
                else
                {
                    txml.AddLineItems(termsType, lineItems, collection, taxExempt, LocalFormat, countryCode, ref items);
                }
                //txml.AddLineItems(termsType, lineItems, collection, taxExempt, LocalFormat, countryCode, ref items);
				txml.AddFooter(LocalFormat);

				if(multiple)
					txml.CreateCopies(Convert.ToInt32(Country[CountryParameterNames.NoTaxCopies]));
				else
					txml.CreateCopies(1);

				Response.Write(txml.Transform());
	
			}
			catch(Exception ex)
			{
				logException(ex, Function);
				Response.Write(ex.Message);
			}
		}

        

        private void LoadCustDetails(DataTable dt, TaxInvoiceXML txml)
		{
			foreach(DataRow row in dt.Rows)
			{
				//txml.SetNode("TAXINVOICE/HEADER/FIRSTNAME", (string)row["FirstName"] + " ");
				//txml.SetNode("TAXINVOICE/HEADER/LASTNAME", (string)row["Lastname"]);
                txml.SetNode("TAXINVOICE/HEADER/ADDR1", (string)row["AddressLine1"]);
                txml.SetNode("TAXINVOICE/HEADER/ADDR2", (string)row["AddressLine2"]);
                txml.SetNode("TAXINVOICE/HEADER/ADDR3", (string)row["AddressLine3"]);
                txml.SetNode("TAXINVOICE/HEADER/POSTCODE", (string)row["PostCode"]);
            }
		}

		private void LoadAddressDetails(DataTable dt, TaxInvoiceXML txml)
		{
			string addType = "";
			foreach(DataRow row in dt.Rows)
			{
				addType = ((string)row["AddressType"]).Trim();
				if(addType == "H")
				{
					txml.SetNode("TAXINVOICE/HEADER/ADDR1", (string)row["Address1"]);
					txml.SetNode("TAXINVOICE/HEADER/ADDR2", (string)row["Address2"]);
					txml.SetNode("TAXINVOICE/HEADER/ADDR3", (string)row["Address3"]);
					txml.SetNode("TAXINVOICE/HEADER/POSTCODE", (string)row["PostCode"]);
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
            Page_Load(null, EventArgs.Empty);//CR 2018-13
        }
		#endregion
	}
}
