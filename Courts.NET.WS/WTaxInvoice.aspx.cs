using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Xml;
using BBSL.Libraries.Printing;
using Blue.Cosacs.Shared.Extensions;
using STL.BLL;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Tags;
using STL.DAL;
using Blue.Cosacs;

namespace STL.WS
{
    public partial class WTaxInvoice : CommonWebPage
    {
        DataSet dAgreement = null;
        string accountType = "";
        NumberFormatInfo LocalFormat = null;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            DataSet custDetails = null;
            DataSet branchDetails = null;
            XmlNode lineItems = null;
            string cashDigit = "4";
            string madagascar = "C";
            string fiji = "F";

            try
            {
                new Blue.Cosacs.InitCountryParamCache().PopulateCacheCountryParams("Z");

                if ((bool)Country[CountryParameterNames.LoyaltyScheme])
                {
                    if (LoyaltyDropStatic.LoyatlyDrop == null || LoyaltyDropStatic.LoyatlyDrop.Tables.Count == 0)
                    {
                        BLoyalty bloyalty = new BLoyalty();
                        LoyaltyDropStatic.LoyatlyDrop = bloyalty.GetLoyaltyDropData();
                    }
                }

                //Extract variables from the QueryString
                string acctNo = Request["acctNo"];
                short branch = Request["branch"].TryParseInt16(0).Value;
                string customerID = Request["customerID"];

                string RequestId = Request["RequestId"];
                string countryCode = Request["country"];
                string culture = Request["culture"];
                accountType = Request["accountType"];
                int buffNo = Request["buffno"].TryParseInt32(0).Value;
                bool creditNote = Convert.ToBoolean(Request["creditNote"]);
                bool multiple = Convert.ToBoolean(Request["multiple"]);
                string InvoiceVersionNo = Request["versionNo"];
                string user = Request["user"];
                bool IsProofofPurchase = Convert.ToBoolean(Request["IsProofofPurchase"]);
                string termsType = String.Empty;
                bool collection = false;
                bool taxExempt = false;
                int versionNo = Request["versionNo"].TryParseInt16(0).Value;
                bool ReprintInvoice = Convert.ToBoolean(Request["ReprintInvoice"]);
                string agreementno = Request["agrmtno"];
                base.SetCulture();

                //Set printer display options for numbers and currency
                LocalFormat = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                LocalFormat.CurrencySymbol = (string)Country[CountryParameterNames.CurrencySymbolForPrint];

                //Build the XML document
                var txml = new TaxInvoiceXML((string)Country[CountryParameterNames.TaxType],
                                                        (string)Country[CountryParameterNames.AgreementTaxType],
                                                        countryCode,
                                                        IsProofofPurchase);
                txml.Load();

                txml.SetNode("TAXINVOICE/HEADER/REPRINT", IsProofofPurchase ? Country[CountryParameterNames.TaxReprintHeader].ToString() : "");

                BAccount acct = new BAccount();
                taxExempt = acct.IsTaxExempt(null, null, acctNo, null);

                BBranch b = new BBranch();
                branchDetails = b.GetBranchAddress(branch, 1);

                if (branchDetails.Tables.Contains("BranchDetails"))
                {
                    var dt = branchDetails.Tables["BranchDetails"];
                    foreach (DataRow r in dt.Rows)
                    {
                        txml.SetNode("TAXINVOICE/HEADER/BRANCHNAME", (string)r["BranchName"]);
                        txml.SetNode("TAXINVOICE/HEADER/BRANCHADDR1", (string)r["BranchAddr1"]);
                        txml.SetNode("TAXINVOICE/HEADER/BRANCHADDR2", (string)r["BranchAddr2"]);
                        txml.SetNode("TAXINVOICE/HEADER/BRANCHADDR3", (string)r["BranchAddr3"]);
                        txml.SetNode("TAXINVOICE/HEADER/SERIALNO", (string)r["Hissn"]);
                        txml.SetNode("TAXINVOICE/HEADER/BRANCHTELNO", r["telNo"].ToString());


                    }
                }

                if (buffNo == 0)
                {
                    using (var conn = new SqlConnection(Connections.Default))
                    {
                        conn.Open();
                        using (var trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            buffNo = b.GetBuffNo(conn, trans, branch);
                        }
                    }
                }

                txml.SetNode("TAXINVOICE/HEADER/BUFFNO", buffNo.ToString());
                txml.SetNode("TAXINVOICE/CREDITNOTE", creditNote.ToString());

                //retrieve the basic customer details
                BCustomer cust = new BCustomer();
                custDetails = cust.GetBasicCustomerDetails(null, null, customerID, acctNo, "H");


                string cust_name = string.Empty;
                Decimal storeCardLimit = 0, storeCardAvailable = 0;
                foreach (DataTable dt in custDetails.Tables)
                {
                    switch (dt.TableName)
                    {
                        case "BasicDetails":
                            LoadCustDetails(dt, txml);
                            cust_name = dt.Rows[0]["FirstName"].ToString() + " " + dt.Rows[0]["LastName"].ToString();
                            storeCardLimit = Convert.ToDecimal(dt.Rows[0]["RFCreditLimit"]);
                            //To Check if Available balance is negative then then Zero 
                            if ((Convert.ToDecimal(dt.Rows[0]["AvailableCredit"]) < 0))
                            {
                                storeCardAvailable = 0;
                            }
                            else
                            {
                                storeCardAvailable = Convert.ToDecimal(dt.Rows[0]["AvailableCredit"]);
                            }
                            break;
                        case TN.CustomerAddresses:
                            LoadAddressDetails(dt, txml);
                            break;
                        default:
                            break;
                    }
                }

                txml.SetNode("TAXINVOICE/HEADER/ACCTNO", ":" + acctNo);
                txml.SetNode("TAXINVOICE/HEADER/TAXNAME", (string)Country[CountryParameterNames.TaxName]);
                txml.SetNode("TAXINVOICE/HEADER/NOW", ":" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));

                DataSet dsAcct = acct.GetAccountDetails(acctNo);
                DateTime acctOpen = Convert.ToDateTime(dsAcct.Tables[0].Rows[0]["Account Opened"]);

                txml.SetNode("TAXINVOICE/HEADER/DATE", acctOpen.ToShortDateString());
                txml.SetNode("TAXINVOICE/HEADER/CUSTOMERID", customerID);
                txml.SetNode("TAXINVOICE/HEADER/COLLECTION", collection.ToString());

                if (countryCode != madagascar)
                {
                    if (acctNo.Substring(3, 1) == cashDigit)
                        txml.SetNode("TAXINVOICE/HEADER/SALETEXT", GetResource("T_CASHTEXT"));
                    else
                        txml.SetNode("TAXINVOICE/HEADER/SALETEXT", GetResource("T_CREDITTEXT"));
                }
                else
                {
                    if (acctNo.Substring(3, 1) == cashDigit)
                        txml.SetNode("TAXINVOICE/HEADER/SALETEXT", GetResource("T_MADCASHTEXT"));
                    else
                        txml.SetNode("TAXINVOICE/HEADER/SALETEXT", GetResource("T_MADCREDITTEXT"));
                }
                int agrmtno = Convert.ToInt32(agreementno);
                if (agrmtno == 0)
                {
                    agrmtno = 1;
                }
                DataSet dsAgreement = new BAgreement()
                                            .GetAgreement(null, null, acctNo, agrmtno, false);

                //agrmtno = (int)dsAgreement.Tables[TN.Agreements].Rows[0][CN.AgrmtNo];
                int userSale = (int)dsAgreement.Tables[TN.Agreements].Rows[0]["Sold By"];

                string userName = new BLogin().GetEmployeeName(null, null, userSale);
                string agreementinvoicenumber = Convert.ToString(dsAgreement.Tables[TN.Agreements].Rows[0][CN.AgreementInvoiceNumber]);

                txml.SetNode("TAXINVOICE/HEADER/USER", userSale.ToString());
                txml.SetNode("TAXINVOICE/HEADER/USERNAME", userName);

                //BOC Suvidha CR 2018-13
                string agreemtInvNum = Convert.ToString(dsAgreement.Tables[TN.Agreements].Rows[0]["AgreementInvoiceNumber"]);

                DataSet dsInvoiceDetails = cust.GetInvoiceDetails(acctNo, Convert.ToString(agrmtno), agreemtInvNum);
                DataTable dtInvoiceDetails = dsInvoiceDetails.Tables["InvoiceDetails"];
                DataTable dtInvoicePaymentDetails = dsInvoiceDetails.Tables["InvoicePaymentDetails"];

                string invoiceNO = agreemtInvNum != string.Empty ? agreemtInvNum : "";
                DataRow drInvDet = dtInvoiceDetails.Rows[0];
                string salesman = Convert.ToString(drInvDet["soldbyID"]) + " - " + Convert.ToString(drInvDet["soldByName"]);
                string cashier = Convert.ToString(drInvDet["createdByID"]) + " - " + Convert.ToString(drInvDet["createdByName"]);
                string countryName = Convert.ToString(Country[CountryParameterNames.CountryName]);
                char taxInvoicePrinted = Convert.ToChar(drInvDet["TaxInvoicePrinted"]);
                string regNo = Convert.ToString(Country["BusinessRegNo"]);
                if (string.IsNullOrEmpty(InvoiceVersionNo) || InvoiceVersionNo == "0")//In case of Revise Account screen
                {
                    InvoiceVersionNo = Convert.ToString(drInvDet["inv_version_no"]);
                }

                //txml.SetNode("TAXINVOICE/HEADER/BRANCHNAME", (string)r["BranchName"]);
                txml.SetNode("TAXINVOICE/AGRMTNO", Convert.ToString(agrmtno));
                txml.SetNode("TAXINVOICE/HEADER/COUNTRY", (string)countryName);
                if (invoiceNO != string.Empty)
                {
                    invoiceNO = invoiceNO.Insert(3, "-");
                }

                if (agrmtno == 1)
                {
                    txml.SetNode("TAXINVOICE/HEADER/INVOICENO", ":" + invoiceNO + "-" + InvoiceVersionNo);
                }
                else
                {
                    txml.SetNode("TAXINVOICE/HEADER/INVOICENO", ":" + invoiceNO);
                }

                if (agrmtno == 1)
                {
                    txml.SetNode("TAXINVOICE/HEADER/INVOICEDATE", ":" + acctOpen.ToString("dd-MMM-yyyy"));
                }
                else
                {
                    if (Convert.ToString(drInvDet["createdOn"]).Trim() != string.Empty)
                    {
                        DateTime createdOnDt = Convert.ToDateTime(drInvDet["createdOn"]);
                        txml.SetNode("TAXINVOICE/HEADER/INVOICEDATE", ":" + createdOnDt.ToString("dd-MMM-yyyy"));
                    }
                }

                if (ReprintInvoice == true)
                {
                    txml.SetNode("TAXINVOICE/HEADER/REPRINTCOPY", " - Reprint Copy");
                }
                else if (taxInvoicePrinted == 'Y')
                {
                    txml.SetNode("TAXINVOICE/HEADER/REPRINTCOPY", " - Reprint Copy");
                }

                txml.SetNode("TAXINVOICE/HEADER/CASHIER", ":" + cashier);
                txml.SetNode("TAXINVOICE/HEADER/SALESMAN", ":" + salesman);
                txml.SetNode("TAXINVOICE/HEADER/CUSTOMERNAME", ":" + cust_name);
                txml.SetNode("TAXINVOICE/HEADER/ACCTBLNC", ":" + LocalFormat.CurrencySymbol + storeCardLimit.ToString("N", LocalFormat));
                txml.SetNode("TAXINVOICE/HEADER/AVAILABLESPEND", ":" + LocalFormat.CurrencySymbol + storeCardAvailable.ToString("N", LocalFormat));
                txml.SetNode("TAXINVOICE/HEADER/REGNO", regNo);
                //BOC Suvidha CR 2018-13

                BItem i = new BItem();
                i.PromoBranch = branch;
                if (ReprintInvoice == true)
                {
                    lineItems = i.GetLineItems(null, null, acctNo, accountType, countryCode, agrmtno, versionNo, ReprintInvoice);
                }
                else
                {
                    lineItems = i.GetLineItems(null, null, acctNo, accountType, countryCode, agrmtno, versionNo);
                }

                if (lineItems != null)
                {
                    if (lineItems.SelectNodes("//Item[@Type='Warranty' or @Type='KitWarranty' and @Quantity!='0']").Count > 0)
                    {
                        acct.Populate(acctNo);
                        termsType = acct.TermsType;
                    }
                }

                decimal items = 1;
                if (ReprintInvoice == true)
                {
                    txml.AddLineItemshVersion(termsType, lineItems, collection, taxExempt, LocalFormat, countryCode, ReprintInvoice, ref items);
                    string invoice_branch = agreementinvoicenumber.Substring(0, 3);//Invoicing branch should be displayed for Reprint Screen
                    txml.SetNode("TAXINVOICE/HEADER/BRANCHNO", ":" + (invoice_branch == string.Empty ? Convert.ToString(branch) : invoice_branch));
                }
                else
                {
                    txml.AddLineItems(termsType, lineItems, collection, taxExempt, LocalFormat, countryCode, ref items);
                    txml.SetNode("TAXINVOICE/HEADER/BRANCHNO", ":" + Convert.ToString(branch));
                }


                if (countryCode == fiji)
                {
                    dAgreement = new BAgreement().GetAgreement(null, null, acctNo, 1, false);
                    CreateAgreementFooter(acctNo, txml, lineItems, countryCode);
                }

                txml.AddFooter(LocalFormat);

                Decimal total_amt = txml.AddPayMethods(dtInvoicePaymentDetails, LocalFormat, countryCode);
                txml.SetNode("TAXINVOICE/PAY2/TOTALAMTPAID", LocalFormat.CurrencySymbol + " " + total_amt.ToString("N", LocalFormat));

#if(XMLTRACE)
				logMessage(txml.Xml, User.Identity.Name, EventLogEntryType.Information);
#endif

                if (multiple)
                    txml.CreateCopies(Convert.ToInt32(Country[CountryParameterNames.NoTaxCopies]));
                else
                    txml.CreateCopies(1);

                acct = null;
                cust = null;
                b = null;
                i = null;

                using (SqlConnection conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        BAccount Account = new BAccount();
                        Account.User = Convert.ToInt32(user);
                        Account.AuditReprint(conn, trans, acctNo, 1, "T");
                        trans.Commit();
                    }
                }

                Response.Write(txml.Transform());

                txml = null;
            }
            catch (Exception ex)
            {
                logException(ex, Function);
                Response.Write(ex.Message);
            }
        }

        string addType = String.Empty;
        private void LoadCustDetails(DataTable dt, TaxInvoiceXML txml)
        {
            foreach (DataRow row in dt.Rows)
            {
                txml.SetNode("TAXINVOICE/HEADER/Title", (string)row[CN.Title] + " ");
                txml.SetNode("TAXINVOICE/HEADER/FIRSTNAME", (string)row[CN.FirstName] + " ");
                txml.SetNode("TAXINVOICE/HEADER/LASTNAME", (string)row[CN.LastName]);
                // CR2018-008 new address pop add hear by tosif ali 27/12/2018*@

                string RequestId = Request["RequestId"];
                if (!string.IsNullOrEmpty(RequestId))
                {
                    BCustomer cust = new BCustomer();
                    DataSet DS = cust.GetCustomerDetails(null, null, RequestId);
                    foreach (DataRow item in DS.Tables[0].Rows)
                    {
                        row[CN.DELTitleC] = item[CN.DELTitleC];
                        row[CN.DELFirstName] = item[CN.DELFirstName];
                        row[CN.DELLastName] = item[CN.DELLastName];
                        addType = item["addType"].ToString();
                    }


                }
                //end hear

                txml.SetNode("TAXINVOICE/HEADER/DELTitleC", (string)row[CN.DELTitleC] + " ");
                txml.SetNode("TAXINVOICE/HEADER/DELFirstName", (string)row[CN.DELFirstName] + " ");
                txml.SetNode("TAXINVOICE/HEADER/DELLastName", (string)row[CN.DELLastName]);
            }
        }

        private void LoadAddressDetails(DataTable dt, TaxInvoiceXML txml)
        {
            //string addType = "";
            //IP - 11/05/10 - UAT(135) UAT5.2.1.0 log - Merged from 4.3
            foreach (DataRow row in dt.Rows)
            {
                //addType = ((string)row["AddressType"]).Trim();
                //if(addType == "H")
                if (string.IsNullOrEmpty(addType))
                {
                    addType = "H";
                }
                if (addType.Trim() == ((string)row["AddressType"]).Trim() && row["category"].ToString() == "CA1")      //IP - 11/05/10 - UAT(135) UAT5.2.1.0 log - Merged from 4.3
                {
                    txml.SetNode("TAXINVOICE/HEADER/ADDR1", (string)row["Address1"]);
                    txml.SetNode("TAXINVOICE/HEADER/ADDR2", (string)row["Address2"]);
                    txml.SetNode("TAXINVOICE/HEADER/ADDR3", (string)row["Address3"]);
                    txml.SetNode("TAXINVOICE/HEADER/POSTCODE", (string)row["PostCode"]);

                    // add home phone details
                    txml.SetNode("TAXINVOICE/HEADER/HOMEPHONE", row["Phone"].ToString());
                }
                //IP - 11/05/10 - UAT(135) UAT5.2.1.0 log - Merged from 4.3
                //else if (addType == "H" && row["category"].ToString() == "CT1")
                //{
                //    // add home phone details
                //    txml.SetNode("TAXINVOICE/HEADER/HOMEPHONE", row["Phone"].ToString());
                //}
                //else if (addType == "M" && row["category"].ToString() == "CT1")
                //{
                //    txml.SetNode("TAXINVOICE/HEADER/MOBILEPHONE", row["Phone"].ToString());
                //}
                //else if (addType == "W" && row["category"].ToString() == "CT1")
                //{
                //    txml.SetNode("TAXINVOICE/HEADER/WORKPHONE", row["Phone"].ToString());
                //    txml.SetNode("TAXINVOICE/HEADER/WORKEXT", row["Ext"].ToString());
                //}

                //if (addType == "H" && row["category"].ToString() == "CT1")
                //{
                //    // add home phone details
                //    txml.SetNode("TAXINVOICE/HEADER/HOMEPHONE", row["Phone"].ToString());
                //}

                //IP - 20/05/10 - UAT(135) UAT5.2.1.0 Log
                if ("M" == ((string)row["AddressType"]).Trim() && row["category"].ToString() == "CT1")
                {
                    txml.SetNode("TAXINVOICE/HEADER/MOBILEPHONE", row["Phone"].ToString());
                }

                if ("W" == ((string)row["AddressType"]).Trim() && row["category"].ToString() == "CA1")
                {
                    txml.SetNode("TAXINVOICE/HEADER/WORKPHONE", row["Phone"].ToString());
                    txml.SetNode("TAXINVOICE/HEADER/WORKEXT", row["Ext"].ToString());
                }
            }
        }

        private void CreateAgreementFooter(string acctNo, TaxInvoiceXML txml, XmlNode items, string countryCode)
        {
            decimal GoodsValue = 0;
            decimal ServicePCent = 0;
            decimal Total = 0;
            decimal InstalNo = 0;
            decimal Deposit = 0;
            //string TermsDescription = "";
            string TermsDescription = String.Empty;             //IP - 11/05/10 - UAT(135) UAT5.2.1.0 log - Merged from 4.3

            decimal srvcharge = 0;

            XmlNodeList allLineItems = items.SelectNodes("//Item[@Code!='DT' and @Quantity!='0' and @Type != 'KitDiscount' and @Type != 'Component']");

            foreach (XmlNode item in allLineItems)
                if (item.Attributes[Tags.Type].Value != IT.Kit &&
                    item.Attributes[Tags.Code].Value != "DT" &&
                    Convert.ToDouble(item.Attributes[Tags.Quantity].Value) != 0)
                {
                    GoodsValue += Convert.ToDecimal(item.Attributes[Tags.Value].Value);
                }

            BAccount bAccount = new BAccount();
            bAccount.Populate(acctNo);

            DTermsType dTermsType = new DTermsType();
            dTermsType.GetTermsTypeDetail(null, null, countryCode, bAccount.TermsType, bAccount.AccountNumber, "", bAccount.DateAccountOpen);

            ServicePCent = Convert.ToDecimal(dTermsType.TermsTypeDetails.Rows[0]["servpcent"]);
            TermsDescription = (string)dTermsType.TermsTypeDetails.Rows[0][CN.Description];

            foreach (DataTable dt in dAgreement.Tables)
            {
                if (dt.TableName == TN.Agreements)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Deposit = (decimal)row["Deposit"];
                        txml.SetNode("TAXINVOICE/AGRMNTFOOTER/DEPOSIT", Deposit.ToString(DecimalPlaces, LocalFormat));
                        Total = (decimal)row["Agreement Total"];
                        txml.SetNode("TAXINVOICE/AGRMNTFOOTER/TOTAL", Total.ToString(DecimalPlaces, LocalFormat));
                        srvcharge = (decimal)row["Service Charge"];
                        txml.SetNode("TAXINVOICE/AGRMNTFOOTER/DT", srvcharge.ToString(DecimalPlaces, LocalFormat));
                        InstalNo = Convert.ToInt16(row["instalno"]) - 1;

                        //string instalments = "";
                        string instalments = String.Empty;          //IP - 11/05/10 - UAT(135) UAT5.2.1.0 log - Merged from 4.3
                        instalments = GetResource("M_AGREEMENTSUMMARY", new Object[] {InstalNo.ToString(),
                                                                                         ((decimal)row["Instalment Amount"]).ToString(DecimalPlaces,LocalFormat),
                                                                                         ((decimal)row["Final Instalment"]).ToString(DecimalPlaces,LocalFormat)});
                        txml.SetNode("TAXINVOICE/AGRMNTFOOTER/FIRSTINST", ((decimal)row["Instalment Amount"]).ToString(DecimalPlaces, LocalFormat));
                        txml.SetNode("TAXINVOICE/AGRMNTFOOTER/FINALINST", ((decimal)row["Final Instalment"]).ToString(DecimalPlaces, LocalFormat));
                        txml.SetNode("TAXINVOICE/AGRMNTFOOTER/INSTALNO", InstalNo.ToString());
                        txml.SetNode("TAXINVOICE/AGRMNTFOOTER/INSTALMENTS", instalments);
                    }
                }
            }

            if (TermsDescription.ToUpper().IndexOf("90 DAYS") != -1)
                txml.SetNode("TAXINVOICE/AGRMNTFOOTER/NINETYDAYS", (GoodsValue / 4).ToString(DecimalPlaces, LocalFormat));
            txml.SetNode("TAXINVOICE/AGRMNTFOOTER/GOODSVAL", GoodsValue.ToString(DecimalPlaces, LocalFormat));
            txml.SetNode("TAXINVOICE/AGRMNTFOOTER/CREDIT", (GoodsValue - Deposit).ToString(DecimalPlaces, LocalFormat));
            txml.SetNode("TAXINVOICE/AGRMNTFOOTER/BALANCE", (Total - Deposit).ToString(DecimalPlaces, LocalFormat));

            switch ((string)Country[CountryParameterNames.ServicePrint])
            {
                case "A":
                    txml.SetNode("TAXINVOICE/AGRMNTFOOTER/INTERESTRATE", ServicePCent.ToString(LocalFormat) + "%");
                    break;
                case "M":
                    txml.SetNode("TAXINVOICE/AGRMNTFOOTER/INTERESTRATE", CalculateMonthlyInterest(ServicePCent).ToString("F2", LocalFormat) + "%");
                    break;
                case "L":
                    txml.SetNode("TAXINVOICE/AGRMNTFOOTER/INTERESTRATE", (ServicePCent / 12).ToString("F2", LocalFormat) + "%");
                    break;
                default:
                    break;
            }

            txml.SetNode("TAXINVOICE/AGRMNTFOOTER/SERVICEPRINT", (string)Country[CountryParameterNames.ServicePrint]);
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
