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
using STL.Common.Constants.Elements;
using System.Threading;
using System.Globalization;
using STL.Common.Constants.ColumnNames;
using System.Diagnostics;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.Tags;
using STL.Common.Constants.ItemTypes;
using System.Configuration;
using STL.Common.Constants.StoreInfo;
using Blue.Cosacs.Repositories;

namespace STL.WS
{
    /// <summary>
    /// Summary description for WAgreement.
    /// </summary>
    public partial class WAgreement : CommonWebPage
    {
        private string CustomerID = "";
        private string AccountNo = "";
        private string AccountType = "";
        private string Culture = "";
        private string CountryCode = "";
        private decimal Deposit = 0;
        private decimal Total = 0;
        private decimal GoodsValue = 0;
        private decimal HCVoucherValue = 0;
        private decimal GoodsTax = 0;
        private decimal DTTax = 0;
        private decimal InstalNo = 0;
        private decimal ServicePCent = 0;
        private decimal ToPay = 0;
        private string TermsDescription = "";
        private bool PaymentHolidays = false;
        private short PaymentHolidaysMax = 0;
        private short PaymentHolidaysMin = 0;
        private string AgreementText = "";
        private string JointName = "";
        private string JointID = "";
        private string relationship = "";
        private bool Multiple = false;
        //private BCountry bCountry = null;
        private BCustomer bCustomer = null;
        private DataSet dCustomer = null;
        private BAccount bAccount = null;
        private DTermsType dTermsType = null;
        //private BAgreement bAgreement = null;
        private DataSet dAgreement = null;
        private BItem bItem = null;
        private XmlNode lineItems = null;
        private double Pages = 0;
        private double itemsPerPage = 10;
        private DataTable dJoint = null;
        private int monthsintfree = 0;
        private int monthsdeferred = 0;
        private decimal insPcent = 0;
        private bool insIncluded = false;
        private decimal chargeableAdminPrice = 0;
        private decimal chargeablePrice = 0;
        private decimal insuranceCharge = 0;
        string addType = "";
        private string Deliveryaddress="XX";

        // 31/03/08 St Lucia have added a trade item
        private decimal _trade;
        private decimal _installation;
        private XmlNodeList allLineItems;

        NumberFormatInfo LocalFormat = null;

        private XmlNode CreateHeader(AgreementXML axml)
        {
            XmlNode header = axml.Document.CreateElement("HEADER");

            header.AppendChild(axml.CreateNode("JOINTNAME", JointName));
            header.AppendChild(axml.CreateNode("JOINTID", JointID));
            header.AppendChild(axml.CreateNode("RELATIONSHIP", relationship.Trim()));
            header.AppendChild(axml.CreateNode("COUNTRYNAME", CachedItems.GetCountryParamters(CountryCode).GetCountryParameterValue<object>(CountryParameterNames.CountryName).ToString()));

            if (dJoint != null)
            {
                foreach (DataRow r in dJoint.Rows)
                {
                    header.AppendChild(axml.CreateNode("JOINTADDR1", (string)r[CN.Address1]));
                    header.AppendChild(axml.CreateNode("JOINTADDR2", (string)r[CN.Address2]));
                    header.AppendChild(axml.CreateNode("JOINTADDR3", (string)r[CN.Address3]));
                    header.AppendChild(axml.CreateNode("JOINTPOSTCODE", (string)r[CN.PostCode]));
                }
            }

            foreach (DataTable dt in dCustomer.Tables)
            {
                if (dt.TableName == "BasicDetails")
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string name = (string)row["Title"] + " " + (string)row["FirstName"] + " " + (string)row["LastName"];
                        header.AppendChild(axml.CreateNode("NAME", name));
                        header.AppendChild(axml.CreateNode("CUSTID", CustomerID));
                        header.AppendChild(axml.CreateNode("ACCTNO", AccountNo));
                    }
                }



                if (dt.TableName == "CustomerAddresses")
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (((string)row[CN.AddressType]).Trim() == "H" &&
                            (string)row[CN.Category] == "CA1") //IP - 13/08/09 - UAT(469) - added or and 'CT1'
                        {
                            header.AppendChild(axml.CreateNode("ADDR1", (string)row["Address1"]));
                            header.AppendChild(axml.CreateNode("ADDR2", (string)row["Address2"]));
                            header.AppendChild(axml.CreateNode("ADDR3", (string)row["Address3"]));
                            header.AppendChild(axml.CreateNode("POSTCODE", (string)row["PostCode"]));
                        }
                        // malaysia requirement appending delivery address
                        //UAT 233 Delivery Address Type should be 'D' not 'D1'          //IP - 11/05/10 - UAT(136) UAT5.2.1.0 log - Merged from 4.3
                        else if (((string)row[CN.AddressType]).Trim() == Deliveryaddress) //Use the delivery address if an items chosen from this address...
                        {
                            header.AppendChild(axml.CreateNode("DELADDR1", (string)row["Address1"]));
                            header.AppendChild(axml.CreateNode("DELADDR2", (string)row["Address2"]));
                            header.AppendChild(axml.CreateNode("DELADDR3", (string)row["Address3"]));
                            header.AppendChild(axml.CreateNode("DELPOSTCODE", (string)row["PostCode"]));
                        }
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        addType = ((string)row["AddressType"]).Trim();
                        switch (addType)
                        {
                            case "H":
                                header.AppendChild(axml.CreateNode("HOMETEL", ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"]));
                                break;
                            case "W": header.AppendChild(axml.CreateNode("WORKTEL", ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"]));
                                break;
                            case "M": header.AppendChild(axml.CreateNode("MOBILE", (string)row["Phone"]));
                                break;
                            default:
                                break;
                        }
                    }
                }

            }

            /*foreach (DataTable dt in dAgreement.Tables)
            {
                if(dt.TableName==TN.AccountDetails)
                {
                    foreach(DataRow row in dt.Rows)
                    {
                        if((bool)Country[CountryParameterNames.AgrTimePrint])
                            header.AppendChild(axml.CreateNode("DATE", ((DateTime)row["Account Opened"]).ToShortDateString()));
                    }
                }
            }*/

            if ((bool)Country[CountryParameterNames.AgrTimePrint])
                header.AppendChild(axml.CreateNode("DATE", DateTime.Now.ToString()));

            return header;
        }

        private XmlNode CreateFooter(AgreementXML axml)
        {
            bool agreementProcessed = false;
            decimal srvcharge = 0;
            XmlNode footer = axml.Document.CreateElement("FOOTER");

            foreach (DataTable dt in dAgreement.Tables)
            {
                if (dt.TableName == TN.Agreements)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        //LiveWire Call 68210
                        if (!agreementProcessed)
                        {
                            Deposit = (decimal)row["Deposit"];
                            footer.AppendChild(axml.CreateNode("DEPOSIT", Deposit.ToString(DecimalPlaces, LocalFormat)));
                            Total = (decimal)row["Agreement Total"];
                            footer.AppendChild(axml.CreateNode("TOTAL", Total.ToString(DecimalPlaces, LocalFormat)));
                            if ((decimal)Country[CountryParameterNames.PercentToPay] > 0)
                            {
                                ToPay = Total * ((decimal)Country[CountryParameterNames.PercentToPay] / 100);
                                footer.AppendChild(axml.CreateNode("TOPAY", ToPay.ToString(DecimalPlaces, LocalFormat)));
                            }
                            
                            GetInsuranceCharge(Deposit, allLineItems); 
                            srvcharge = (decimal)row["Service Charge"];
                            //srvcharge += HCVoucherValue;        //UAT56 jec 06/04/10

                            //cr 1005 Jamaica want insurance include in service charge for agreement printout but not as a separate line
                            if ((bool)Country[CountryParameterNames.IncInsinServAgrPrint] && insIncluded == false) // Only if agreement is false
                            {
                                srvcharge += insuranceCharge;
                            }

                            footer.AppendChild(axml.CreateNode("DT", srvcharge.ToString(DecimalPlaces, LocalFormat)));
                            InstalNo = Convert.ToInt16(row["instalno"]) - 1;
                            InstalNo -= PaymentHolidaysMax;

                            // Instalments are either fixed or variable
                            string instalments = this.CreateInstalments(row);

                            footer.AppendChild(axml.CreateNode("FIRSTINST", ((decimal)row["Instalment Amount"]).ToString(DecimalPlaces, LocalFormat)));
                            footer.AppendChild(axml.CreateNode("FINALINST", ((decimal)row["Final Instalment"]).ToString(DecimalPlaces, LocalFormat)));
                            footer.AppendChild(axml.CreateNode("INSTALNO", InstalNo.ToString()));
                            footer.AppendChild(axml.CreateNode("INSTALMENTS", instalments));

                            agreementProcessed = true;
                        }
                    }
                }
            }


            short ninetyDays = Convert.ToInt16(Country[CountryParameterNames.Print90]);
            if (TermsDescription.ToUpper().IndexOf("90 DAYS") != -1 && Convert.ToBoolean(ninetyDays))
                footer.AppendChild(axml.CreateNode("NINETYDAYS", (GoodsValue / 4).ToString(DecimalPlaces, LocalFormat)));

            if (AgreementText.Trim().Length > 0)
            {
                try
                {
                    if (PaymentHolidays)
                    {
                        // Payment holiday text
                        footer.AppendChild(axml.CreateNode("PAYMENTHOLIDAYS", String.Format(AgreementText, new Object[] { PaymentHolidaysMax, PaymentHolidaysMin })));
                    }
                    else
                    {
                        // Any other text
                        footer.AppendChild(axml.CreateNode("PAYMENTHOLIDAYS", AgreementText));
                    }
                }
                catch (System.FormatException)
                {
                    /* users are able to maintain the text so we can't rely on it formating
                     * properly. If there is a formatting error just use the text without formatting */
                    footer.AppendChild(axml.CreateNode("PAYMENTHOLIDAYS", AgreementText));
                }
            }

            footer.AppendChild(axml.CreateNode("GOODSVAL", GoodsValue.ToString(DecimalPlaces, LocalFormat)));
            footer.AppendChild(axml.CreateNode("CREDIT", (GoodsValue - Deposit).ToString(DecimalPlaces, LocalFormat)));
            footer.AppendChild(axml.CreateNode("BALANCE", (Total - Deposit).ToString(DecimalPlaces, LocalFormat)));
            footer.AppendChild(axml.CreateNode("INSURANCE", insuranceCharge.ToString(DecimalPlaces, LocalFormat)));

            switch ((string)Country[CountryParameterNames.ServicePrint])
            {
                case "A":
                    //footer.AppendChild(axml.CreateNode("INTERESTRATE", ServicePCent.ToString((string)Country[CountryParameterNames.ServicePrintDP])+"%"));
                    footer.AppendChild(axml.CreateNode("INTERESTRATE", ServicePCent.ToString("F2") + "%"));
                    break;
                case "M":
                    // 26/03/08 rdb formats with currency sign, change to use decimal places
                    //footer.AppendChild(axml.CreateNode("INTERESTRATE", CalculateMonthlyInterest(ServicePCent).ToString((string)Country[CountryParameterNames.ServicePrintDP])+"%"));
                    footer.AppendChild(axml.CreateNode("INTERESTRATE", CalculateMonthlyInterest(ServicePCent).ToString("F2") + "%"));
                    break;
                case "L":
                    //footer.AppendChild(axml.CreateNode("INTERESTRATE", (ServicePCent/12).ToString((string)Country[CountryParameterNames.ServicePrintDP])+"%"));
                    footer.AppendChild(axml.CreateNode("INTERESTRATE", (ServicePCent / 12).ToString("F2") + "%"));
                    break;
                default:
                    break;
            }

            footer.AppendChild(axml.CreateNode("SERVICEPRINT", (string)Country[CountryParameterNames.ServicePrint]));
            footer.AppendChild(axml.CreateNode("PCENTTOPAY", ((decimal)Country[CountryParameterNames.PercentToPay]).ToString()));

            footer.AppendChild(axml.CreateNode("PRETAXGOODSVAL", (GoodsValue - GoodsTax).ToString(DecimalPlaces, LocalFormat)));
            footer.AppendChild(axml.CreateNode("GOODSVALTAX", GoodsTax.ToString(DecimalPlaces, LocalFormat)));
            footer.AppendChild(axml.CreateNode("PRETAXDT", (srvcharge - DTTax).ToString(DecimalPlaces, LocalFormat)));
            footer.AppendChild(axml.CreateNode("DTTAX", DTTax.ToString(DecimalPlaces, LocalFormat)));

            // 18/03/08 RDB St Lucia Require extra fields
            footer.AppendChild(axml.CreateNode("SEVENTYPCTOTAL", (Total * .7m).ToString(DecimalPlaces, LocalFormat)));
            // todo extend when other values available
            // 31/03/08 Installation charge 

            //footer.AppendChild(axml.CreateNode("TOTALSTLUCIA", (Total - (Deposit) + (insuranceCharge )).ToString(DecimalPlaces, LocalFormat)));
            footer.AppendChild(axml.CreateNode("TOTALSTLUCIA", (Total - Deposit - srvcharge + insuranceCharge).ToString(DecimalPlaces, LocalFormat)));
            
            // InsAmount = round((convert(float,AgrmtTotal) * convert(float,AgrmtYears) * (convert(float,InsPCent) / 100)) ,2)
            // where AgrmtYears = instalno/12
            //decimal insAmount = Total * (InstalNo / 12) * (insPcent / 100);
            // So charge for credit extended = agreement.servicechge - insurance charge
            decimal chrgCreditExt = srvcharge - insuranceCharge;
            footer.AppendChild(axml.CreateNode("CHRGCREDITEXT", chrgCreditExt.ToString(DecimalPlaces)));

            // add element for Chargecredit %, Service Percent - Insurance PErcent
            footer.AppendChild(axml.CreateNode("CHRGCREDITINTRATECOMPOUND", CalculateMonthlyInterest(ServicePCent - insPcent).ToString("F2") + "%"));
            footer.AppendChild(axml.CreateNode("CHRGCREDITINTRATE", ((ServicePCent - insPcent) / 12).ToString("F2") + "%"));

            footer.AppendChild(axml.CreateNode("TRADE", (-(_trade)).ToString(DecimalPlaces, LocalFormat)));
            footer.AppendChild(axml.CreateNode("DEPTRADETTL", (Deposit - _trade).ToString(DecimalPlaces, LocalFormat)));
            footer.AppendChild(axml.CreateNode("INSPCENT", insPcent.ToString("F2") + "%"));
            ////footer.AppendChild(axml.CreateNode("INSPCENT", CalculateMonthlyInterest(insPcent).ToString(DecimalPlaces) + "%"));
	
            return footer;
        }

        private string CreateInstalments(DataRow agreementRow)
        {
            // Check for variable payments
            string instalments = "";

            // Load any variable instalments
            BInstalPlan bInstalPlan = new BInstalPlan();
            DataSet variableInstalPlanSet = bInstalPlan.GetVariableInstalmentsByAcctNo(AccountNo);

            foreach (DataTable variableInstalList in variableInstalPlanSet.Tables)
            {
                if (variableInstalList.TableName == TN.VariableInstal)
                {
                    foreach (DataRow variableInstal in variableInstalList.Rows)
                    {
                        if (instalments == "")
                            instalments = GetResource("M_AGREEMENTSUMMARYVARIABLE", new Object[] {((decimal)variableInstal[CN.InstalmentNumber]).ToString(DecimalPlaces,LocalFormat),
																									 ((decimal)variableInstal[CN.InstalAmount]).ToString(DecimalPlaces,LocalFormat)});
                        else
                            instalments += " " + GetResource("M_AGREEMENTSUMMARYVARIABLEFOLLOW", new Object[] {((decimal)variableInstal[CN.InstalmentNumber]).ToString(DecimalPlaces,LocalFormat),
																												  ((decimal)variableInstal[CN.InstalAmount]).ToString(DecimalPlaces,LocalFormat)});
                    }
                    if (instalments != "")
                        instalments += " " + GetResource("M_AGREEMENTSUMMARYVARIABLEFINAL", new Object[] { ((decimal)agreementRow["Final Instalment"]).ToString(DecimalPlaces, LocalFormat) });
                }
            }

            if (instalments == "")
            {
                // Default to the standard instalment text
                instalments = GetResource("M_AGREEMENTSUMMARY", new Object[] {InstalNo.ToString(),
																				 ((decimal)agreementRow["Instalment Amount"]).ToString(DecimalPlaces,LocalFormat),
																				 ((decimal)agreementRow["Final Instalment"]).ToString(DecimalPlaces,LocalFormat)});
            }

            bInstalPlan = null;
            return instalments;
        }

        private void AddLineItemByPage(AgreementXML axml, XmlNode page, XmlNode item)
        {
            decimal quantity = 0;
            decimal itemValue = 0;
            decimal kitValue = 0;
            XmlUtilities xml = new XmlUtilities();
            if (item.Attributes[Tags.Type].Value != IT.Kit &&
                item.Attributes[Tags.Code].Value != "DT" &&
                Convert.ToDouble(item.Attributes[Tags.Quantity].Value) != 0)
            {
                quantity = Convert.ToDecimal(item.Attributes[Tags.Quantity].Value);
                itemValue = Convert.ToDecimal(item.Attributes[Tags.Value].Value);
                
                
                XmlNode node = page.SelectSingleNode("LINEITEMS");
                XmlNode itemNode = axml.Document.CreateElement("LINEITEM");
                XmlNode itemNoNode = axml.Document.CreateElement("ITEMNO");
                XmlNode qtyNode = axml.Document.CreateElement("QUANTITY");
                XmlNode priceNode = axml.Document.CreateElement("VALUE");
                XmlNode descNode = axml.Document.CreateElement("DESC");
                XmlNode desc2Node = axml.Document.CreateElement("DESC2");
                XmlNode trimNode = axml.Document.CreateElement("TRIM");
                XmlNode addToNode = axml.Document.CreateElement("ADDTO");
                XmlNode acctnoNode = axml.Document.CreateElement("ACCTNO");
                XmlNode preTaxPriceNode = axml.Document.CreateElement("PRETAXVALUE");
                XmlNode typeNode = axml.Document.CreateElement("TYPE");

                qtyNode.InnerText = quantity.ToString("F2");
                priceNode.InnerText = itemValue.ToString(DecimalPlaces, LocalFormat);
                descNode.InnerText = item.Attributes[Tags.Description1].Value;
                desc2Node.InnerText = item.Attributes[Tags.Description2].Value;
                trimNode.InnerText = item.Attributes[Tags.ColourTrim].Value;
                itemNoNode.InnerText = item.Attributes[Tags.Code].Value;
                addToNode.InnerText = item.Attributes[Tags.AddTo].Value;
                acctnoNode.InnerText = item.Attributes["ACCTNO"].Value;
                typeNode.InnerText = "";

                if ((string)Country[CountryParameterNames.TaxType] == "E")
                    preTaxPriceNode.InnerText = priceNode.InnerText;
                else
                    preTaxPriceNode.InnerText = (itemValue - Convert.ToDecimal(item.Attributes[Tags.TaxAmount].Value)).ToString(DecimalPlaces, LocalFormat);

                itemNode.AppendChild(qtyNode);
                itemNode.AppendChild(priceNode);
                itemNode.AppendChild(descNode);
                itemNode.AppendChild(desc2Node);
                itemNode.AppendChild(trimNode);
                itemNode.AppendChild(itemNoNode);
                itemNode.AppendChild(addToNode);
                itemNode.AppendChild(acctnoNode);
                itemNode.AppendChild(preTaxPriceNode);
                itemNode.AppendChild(typeNode);
                node.AppendChild(itemNode);

                if (item.Attributes[Tags.AddTo].Value == Boolean.FalseString)
                {
                    if (item.Attributes[Tags.Code].Value != "ZXHC")      //UAT56 jec 06/04/10 exclude HC voucher from Goods value
                    {
                        GoodsValue += itemValue;
                        GoodsTax += Convert.ToDecimal(item.Attributes[Tags.TaxAmount].Value);
                    }
                    else                //UAT56 jec 06/04/10
                        HCVoucherValue += itemValue;
                }
            }
            else
            {
                if (item.Attributes[Tags.Type].Value == IT.Kit &&
                    Convert.ToDouble(item.Attributes[Tags.Quantity].Value) != 0)
                {
                    XmlNode related = item.SelectSingleNode(Elements.RelatedItem);
                    foreach (XmlNode comp in related.ChildNodes)
                    {
                        if (comp.Attributes[Tags.Type].Value != IT.KitWarranty)
                        {
                            kitValue += Convert.ToDecimal(comp.Attributes[Tags.Value].Value);

                            if (comp.Attributes[Tags.AddTo].Value == Boolean.FalseString)
                                GoodsValue += Convert.ToDecimal(comp.Attributes[Tags.Value].Value);
                        }
                    }

                    quantity = Convert.ToDecimal(item.Attributes[Tags.Quantity].Value);

                    XmlNode node = page.SelectSingleNode("LINEITEMS");
                    XmlNode itemNode = axml.Document.CreateElement("LINEITEM");
                    XmlNode qtyNode = axml.Document.CreateElement("QUANTITY");
                    XmlNode priceNode = axml.Document.CreateElement("VALUE");
                    XmlNode descNode = axml.Document.CreateElement("DESC");
                    XmlNode itemNoNode = axml.Document.CreateElement("ITEMNO");
                    XmlNode addToNode = axml.Document.CreateElement("ADDTO");
                    XmlNode acctnoNode = axml.Document.CreateElement("ACCTNO");
                    XmlNode typeNode = axml.Document.CreateElement("TYPE");

                    typeNode.InnerText = item.Attributes[Tags.Type].Value;
                    qtyNode.InnerText = quantity.ToString("F2");
                    descNode.InnerText = item.Attributes[Tags.Description1].Value;
                    itemNoNode.InnerText = item.Attributes[Tags.Code].Value;
                    priceNode.InnerText = kitValue.ToString("C2", LocalFormat);
                    addToNode.InnerText = item.Attributes[Tags.AddTo].Value;
                    acctnoNode.InnerText = item.Attributes["ACCTNO"].Value;

                    itemNode.AppendChild(qtyNode);
                    itemNode.AppendChild(priceNode);
                    itemNode.AppendChild(descNode);
                    itemNode.AppendChild(itemNoNode);
                    itemNode.AppendChild(addToNode);
                    itemNode.AppendChild(acctnoNode);
                    itemNode.AppendChild(typeNode);

                    node.AppendChild(itemNode);

                    foreach (XmlNode comp in related.ChildNodes)
                    {
                        if (comp.Attributes[Tags.Type].Value != IT.KitWarranty)
                        {
                            decimal qty = Convert.ToDecimal(comp.Attributes[Tags.Quantity].Value);

                            XmlNode compNode = page.SelectSingleNode("LINEITEMS");
                            XmlNode compItemNode = axml.Document.CreateElement("LINEITEM");
                            XmlNode compTypeNode = axml.Document.CreateElement("TYPE");
                            XmlNode compQtyNode = axml.Document.CreateElement("QUANTITY");
                            XmlNode compItemNoNode = axml.Document.CreateElement("ITEMNO");
                            XmlNode compDescNode = axml.Document.CreateElement("DESC");
                            XmlNode compDescNode2 = axml.Document.CreateElement("DESC2");

                            compTypeNode.InnerText = comp.Attributes[Tags.Type].Value;
                            compQtyNode.InnerText = qty.ToString("F2");
                            compItemNoNode.InnerText = comp.Attributes[Tags.Code].Value;
                            compDescNode.InnerText = comp.Attributes[Tags.Description1].Value;
                            compDescNode2.InnerText = comp.Attributes[Tags.Description2].Value;

                            compItemNode.AppendChild(compQtyNode);
                            compItemNode.AppendChild(compItemNoNode);
                            compItemNode.AppendChild(compDescNode);
                            compItemNode.AppendChild(compDescNode2);

                            compNode.AppendChild(compItemNode);
                        }
                    }
                }
            }
        }

        private void FlagItems(XmlNode item, string flag, string accountNo)
        {
            item.Attributes.Append(item.OwnerDocument.CreateAttribute(Tags.AddTo));
            item.Attributes[Tags.AddTo].Value = flag;
            item.Attributes.Append(item.OwnerDocument.CreateAttribute("ACCTNO"));
            item.Attributes["ACCTNO"].Value = accountNo;
            foreach (XmlNode child in item.SelectSingleNode(Elements.RelatedItem).ChildNodes)
                FlagItems(child, flag, accountNo);
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                Function = "PrintAgreement";
                CustomerID = Request["customerID"];
                AccountNo = Request["accountNo"];
                AccountType = Request["accountType"];
                Culture = Request["culture"];
                CountryCode = Request["countryCode"];
                Multiple = Convert.ToBoolean(Request["multiple"]);

                try
                {
                    itemsPerPage = Convert.ToDouble(ConfigurationSettings.AppSettings["agreementLineItems"]);
                }
                catch (Exception ex)
                {
                    /* don't want to fail if the config key is not set, just use default */
                }

                #region Retrieve all the required data
                //Set the culture for currency formatting
                //Thread.CurrentThread.CurrentCulture = new CultureInfo(Culture);
                base.SetCulture();

                //Set printer display options for numbers and currency
                LocalFormat = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                LocalFormat.CurrencySymbol = (string)Country[CountryParameterNames.CurrencySymbolForPrint];

                bCustomer = new BCustomer();
                dCustomer = bCustomer.GetBasicCustomerDetails(null, null, CustomerID, AccountNo, "H");

                /* retrieve the service percentage */
                bAccount = new BAccount();
                bAccount.Populate(AccountNo);

                /* if there is a joint applicant for credit, include their name */
                if (AT.IsCreditType(AccountType))
                //if (AccountType != AT.Cash && AccountType != AT.Special) //Acct Type Translation DSR 29/9/03
                {
                    JointName = bAccount.GetApplicantTwoName(CustomerID, AccountNo, out JointID, out relationship);
                    if (JointName.Length > 0)
                        dJoint = bCustomer.GetDetails(JointID);
                }

                dTermsType = new DTermsType();
                dTermsType.GetTermsTypeDetail(null, null, CountryCode, bAccount.TermsType, bAccount.AccountNumber, "", bAccount.DateAccountOpen);

                ServicePCent = Convert.ToDecimal(dTermsType.TermsTypeDetails.Rows[0]["servpcent"]);
                TermsDescription = (string)dTermsType.TermsTypeDetails.Rows[0][CN.Description];
                PaymentHolidays = Convert.ToBoolean(dTermsType.TermsTypeDetails.Rows[0][CN.PaymentHoliday]);
                PaymentHolidaysMin = (short)dTermsType.TermsTypeDetails.Rows[0][CN.PaymentHolidayMin];
                AgreementText = (string)dTermsType.TermsTypeDetails.Rows[0][CN.AgreementText];
                insPcent = Convert.ToDecimal(dTermsType.TermsTypeDetails.Rows[0][CN.InsPcent]);
                insIncluded = Convert.ToBoolean(dTermsType.TermsTypeDetails.Rows[0][CN.InsIncluded]);
                string xsltFilename = (string)dTermsType.TermsTypeDetails.Rows[0][CN.AgreementPrintDesc];

                /* load the agreement table stuff */
                BAgreement bAgreement = new BAgreement();
                dAgreement = bAgreement.GetAgreement(null,null, AccountNo, 1, false); //IP - 11/02/11 - Sprint 5.10 - #2978 - added null, null for conn, trans
                PaymentHolidaysMax = (short)dAgreement.Tables[TN.Agreements].Rows[0][CN.PaymentHoliday];

                /* load the line items for the account */
                bItem = new BItem();
                DAccount dAccount = new DAccount();

                if (dAccount.IsWarrantyRenewal(AccountNo))
                    lineItems = bItem.GetWarrantyRenewalItems(AccountNo, AccountType, CountryCode, 1);
                else
                    lineItems = bItem.GetLineItems(null, null, AccountNo, AccountType, CountryCode, 1);

                #endregion

                //CR903 - Lucky Dollar Stores will need a seperate
                //stylesheet displaying the Lucky Dollar logo.
                DBranch branch = new DBranch();
                string acctNo = AccountNo.Substring(0, 3);
                string storeType = branch.GetStoreType(Convert.ToInt16(acctNo));

                BranchRepository br = new BranchRepository();
                var nonCourtsStoreTypes = br.GetNonCourtsStoreType(Convert.ToInt16(acctNo));

                if (storeType == StoreType.NonCourts && (nonCourtsStoreTypes.LuckyDollarStore!= null && nonCourtsStoreTypes.LuckyDollarStore == true))
                {
                    xsltFilename = "Agreement_LD.xslt";
                }
                else if (storeType == StoreType.NonCourts && (nonCourtsStoreTypes.RadioShackStore != null && nonCourtsStoreTypes.RadioShackStore == true))
                {
                    xsltFilename = "Agreement_Radioshack.xslt";
                }

                else if (storeType == StoreType.NonCourts && (nonCourtsStoreTypes.AshleyStore != null && nonCourtsStoreTypes.AshleyStore == true))
                {
                    xsltFilename = "Agreement_Ashley.xslt";
                }
                else if (storeType == StoreType.NonCourts && (nonCourtsStoreTypes.RadioShackStore != null && nonCourtsStoreTypes.RadioShackStore == true))
                {
                    xsltFilename = "Agreement_Radioshack.xslt";
                }
                else if(CountryCode.Trim() == "Q" || CountryCode.Trim() == "B" || CountryCode.Trim() == "X")
                {
                    xsltFilename = "Agreement_CUR.xslt";
                }

                //CR 440 - Additional parameter 'xsltFilename' to AgreementXML constructor call
                AgreementXML axml = new AgreementXML(CountryCode, xsltFilename);
                axml.Load("<AGREEMENT><CUSTOMER/><LAST/></AGREEMENT>");


                /* first thing we need to know is how many pages we need, which 
                 * depends on how many line items */
                double numItems = 0;
                XmlNode[] addToItems = null;

                /* flag all lineitems as not belonging to an added on account */
                foreach (XmlNode i in lineItems.ChildNodes)
                    FlagItems(i, Boolean.FalseString, AccountNo);

                /* if this account has been added to AA or has been refinanced*/
                if (lineItems.SelectSingleNode("//Item[@Quantity != '0' and (@Code = 'ADDDR' or @Code ='REFINDR')]") != null)
                {
                    DataTable addedToAccts = bAccount.GetAccountsAddedTo(null, null, AccountNo);
                    addToItems = new XmlNode[addedToAccts.Rows.Count];
                    for (int x = 0; x < addedToAccts.Rows.Count; x++)
                        addToItems[x] = bItem.GetLineItems(null, null, (string)addedToAccts.Rows[x][CN.AccountNumber], AccountType, CountryCode, 1);

                    /* need to try and combine the lineitem nodes */
                    XmlNode addcr = lineItems.SelectSingleNode("//Item[@Quantity != '0' and (@Code = 'ADDDR' or @Code ='REFINDR')]");
                    addcr.Attributes[Tags.Description1].Value = GetResource("M_BALANCEFROMPREV");
                    for (int i = 0; i < addToItems.Length; i++)
                    {
                        for (int a = 0; a < addToItems[i].ChildNodes.Count; a++)
                        {
                            XmlNode addNode = addToItems[i].ChildNodes[a];
                            if (addNode.Attributes[Tags.Code].Value != "ADDCR" & addNode.Attributes[Tags.Code].Value !="REFINCR")
                            {
                                addNode = lineItems.OwnerDocument.ImportNode(addNode, true);
                                FlagItems(addNode, Boolean.TrueString, (string)addedToAccts.Rows[i][CN.AccountNumber]);	/* flag as add tos */
                                lineItems.InsertAfter(addNode, addcr);
                            }
                        }
                    }
                }

                /* extract the tax on the DT */
                DTTax = Convert.ToDecimal(lineItems.SelectSingleNode("//Item[@Code='DT']").Attributes[Tags.TaxAmount].Value);

              
                // cr 1005 Jamaica want insurance include in service charge for agreement printout but not as a separate line
                if (!(bool)Country[CountryParameterNames.IncInsinServAgrPrint])
                {
                    allLineItems = lineItems.SelectNodes("//Item[@Code!='DT' and @Quantity!='0' and @Type != 'KitDiscount' and @Type != 'Component' and @SPIFFItem = 'False']");
                }
                else
                {
                    allLineItems = lineItems.SelectNodes("//Item[@Code!='DT' and @Quantity!='0' and @Type != 'KitDiscount' and @Type != 'Component' and @SPIFFItem = 'False' and @IsInsurance = 'False'] ");
                }

                if (allLineItems != null) //Check whether delivery address on lines before writing header
                {
                    numItems = allLineItems.Count;
                    for (int i = 1; i <= allLineItems.Count; i++)
                    {
                         XmlNode item = allLineItems[Convert.ToInt32(i) - 1];
                        if (item.Attributes[Tags.DeliveryAddress].Value != ""
                            && item.Attributes[Tags.DeliveryAddress].Value.Trim() !="H" )
                            
                        {
                            Deliveryaddress = item.Attributes[Tags.DeliveryAddress].Value.Trim();
                        }
                        //arse
                    }
                 

                }

                Pages = Math.Ceiling(numItems / itemsPerPage);
                Pages = Pages == 0 ? 1 : Pages;


                // 26/03/08 rdb for printing in St Lucia need to add a blank page when physical number of pages is odd
                // so that duplicate copy starts on a fresh sheet of paper
                double modCount = numItems % itemsPerPage;
                double fullItemCount = numItems + itemsPerPage - (numItems % 5);
                bool blankPageRequired =
                    (modCount == 0 && numItems % 2 > 0) ||
                    (modCount > 0 && fullItemCount % 2 > 0);
                axml.GetNode("AGREEMENT").AppendChild(axml.CreateNode("INSERTBLANKPAGE", blankPageRequired.ToString().ToUpper()));


                for (int i = 0; i < Pages; i++)
                {
                    XmlNode page = axml.Document.CreateElement("PAGE");
                    if (i == Pages - 1)
                        page.AppendChild(axml.CreateNode("LAST", Boolean.TrueString));
                    else
                        page.AppendChild(axml.CreateNode("LAST", Boolean.FalseString));
                    page.AppendChild(CreateHeader(axml));
                    page.AppendChild(axml.Document.CreateElement("LINEITEMS"));
                    axml.GetNode("AGREEMENT").AppendChild(page);
                }

                _trade = 0;
                _installation = 0;

                XmlNodeList pages = axml.Document.SelectNodes("//PAGE");
                if (allLineItems != null)
                {
                    for (double i = 1; i <= allLineItems.Count; i++)
                    {
                        double pageIndex = Math.Ceiling(Convert.ToDouble(i / itemsPerPage));
                        XmlNode item = allLineItems[Convert.ToInt32(i) - 1];
                        AddLineItemByPage(axml, pages[Convert.ToInt32(pageIndex) - 1], item);

                        if (item.Attributes[Tags.Code].Value == "TRADE")
                            _trade += Convert.ToDecimal(item.Attributes[Tags.Value].Value);
                        if (item.Attributes[Tags.Code].Value == "511700")
                            _installation += Convert.ToDecimal(item.Attributes[Tags.Value].Value);
                    }

                    // 01/04/08 rdb moving this code as Deposit is not populated at this point
                    //GetInsuranceCharge(bAgreement.Deposit, allLineItems);
                }

                axml.GetNode("AGREEMENT").AppendChild(CreateFooter(axml));

                int noCopies = 0;
                if (Multiple)
                    noCopies = Convert.ToInt32(Country[CountryParameterNames.NoAgrCopies]);
                else
                    noCopies = 1;

                if (JointName.Length > 0)
                    noCopies += 1;

                int custCopies = Convert.ToInt32(Country[CountryParameterNames.AgreementCustomerCopies]);

                axml.CreateCopies(noCopies, custCopies);

                bCustomer = null;
                bAccount = null;
                bAgreement = null;
                bItem = null;
                dAccount = null;
                dTermsType = null;
                branch = null;
                GC.Collect();

                Response.Write(axml.Transform());
            }
            catch (Exception ex)
            {
                logException(ex, Function);
                Response.Write(ex.Message);
            }
        }

        private void GetInsuranceCharge(decimal deposit, XmlNodeList allItems)
        {
            DAccount acct = new DAccount();
            acct.SelectType(null, null, AccountType, CountryCode, out monthsintfree, out monthsdeferred);

            BItem item = new BItem();

            DInstalPlan ip = new DInstalPlan();
            ip.Populate(null, null, AccountNo, 1);
            decimal months = ip.NumberOfInstalments += monthsdeferred - monthsintfree;

            chargeablePrice = bAccount.GetChargeableCashPrice(null, null, AccountNo, ref chargeableAdminPrice);

            if (insIncluded)
                // 25/03/08 rdb insurance calculation needs to be divided by 100
                insuranceCharge = (chargeablePrice - deposit) * (insPcent / 100) * (months / 12);
            else
                foreach (XmlNode node in lineItems)
                    if (node.Attributes[Tags.Code].Value == (string)Country[CountryParameterNames.InsuranceChargeItem])
                        insuranceCharge = Convert.ToDecimal(node.Attributes[Tags.Quantity].Value) * Convert.ToDecimal(node.Attributes[Tags.UnitPrice].Value);

            acct = null;
            item = null;
            ip = null;
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
