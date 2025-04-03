using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;
using mshtml;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Elements;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.StoreInfo;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Tags;
using STL.Common.Printing.AgreementPrinting;
using STL.Common.Static;
namespace STL.PL
{
    /// <summary>
    /// All of the forms in the Presentation Layer (PL) project are derived
    /// from this base class that contains many common functions including:
    ///  - User Permissions
    ///  - Error Logging
    ///  - Field Validation
    ///  - Field Formatting
    ///  - Language Translation
    ///  - Printing
    /// </summary>
    public partial class CommonForm : System.Windows.Forms.Form
    {

        protected void PrintAgreement(AxSHDocVw.AxWebBrowser b, string accountNo, int agreementNo, string accountType, string customerID, ref int noPrints, bool multiple)
        {

            try
            {
                STL.Common.PrintingAgreementRequest input = new STL.Common.PrintingAgreementRequest();
                input.accountNo = accountNo;
                input.accountType = accountType;
                input.customerID = customerID;
                input.countrycode = Config.CountryCode;

                PrintingAgreementResult agreeresult = Printing.GetAgreeementPrint(input);

                CheckStyleSheet(agreeresult.filename);

                AgreementXML axml = new AgreementXML(Config.CountryCode, agreeresult.filename);
                axml.Load("<AGREEMENT><CUSTOMER/><LAST/></AGREEMENT>");

                double numItems = 0;
                XmlNodeList allLineItems;
                decimal insuranceTax = 0;
                decimal staxValue = 0;

                /* extract the tax on the DT */
                var dtitem = agreeresult.lineitems.SelectSingleNode("//Item[@Code='DT']");

                decimal DTTax = 0;

                if (dtitem != null)
                {
                    DTTax = Convert.ToDecimal(dtitem.Attributes[Tags.TaxAmount].Value);
                }


                // cr 1005 Jamaica want insurance include in service charge for agreement printout but not as a separate line
                if (!(bool)Country[CountryParameterNames.IncInsinServAgrPrint])
                {
                    //allLineItems = agreeresult.lineitems.SelectNodes("//Item[@Code!='DT' and @Quantity!='0' and @Type != 'KitDiscount' and @Type != 'Component' and @SPIFFItem = 'False']");
                    allLineItems = agreeresult.lineitems.SelectNodes("//Item[@Code!='DT' and @Quantity!='0' and @Type != 'KitDiscount' and @SPIFFItem = 'False']");                             //IP - 11/01/12 - #8649 - LW74016
                }
                else
                {
                    //If Tax is Exclusive we need to find the Tax on Insurance, deduct this from the STAX line
                    //and add it to the Charge For Credit amount as Insurance item in this case is not displayed
                    //on the agreement contract.
                    if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                    {
                        var insuranceItem = agreeresult.lineitems.SelectSingleNode("//Item[@IsInsurance = 'True']");
                        var staxItem = agreeresult.lineitems.SelectSingleNode("//Item[@Code='STAX']");

                        if (insuranceItem != null)
                        {
                            insuranceTax = Convert.ToDecimal(insuranceItem.Attributes[Tags.TaxAmount].Value);

                            if (staxItem != null)
                            {
                                staxValue = Convert.ToDecimal(staxItem.Attributes[Tags.Value].Value);
                                staxValue -= insuranceTax;

                                staxItem.Attributes[Tags.UnitPrice].Value = staxValue.ToString();
                                staxItem.Attributes[Tags.CashPrice].Value = staxValue.ToString();
                                staxItem.Attributes[Tags.HPPrice].Value = staxValue.ToString();
                                staxItem.Attributes[Tags.Value].Value = staxValue.ToString();
                            }
                        }
                    }

                    //allLineItems = agreeresult.lineitems.SelectNodes("//Item[@Code!='DT' and @Quantity!='0' and @Type != 'KitDiscount' and @Type != 'Component' and @SPIFFItem = 'False' and @IsInsurance = 'False'] ");
                    allLineItems = agreeresult.lineitems.SelectNodes("//Item[@Code!='DT' and @Quantity!='0' and @Type != 'KitDiscount' and @SPIFFItem = 'False' and @IsInsurance = 'False'] ");  //IP - 11/01/12 - #8649 - LW74016
                }

                if (allLineItems != null)
                    numItems = allLineItems.Count;

                string deladdress = "";
                for (int counter = 0; counter <= numItems - 1; counter++)
                {
                    deladdress = allLineItems[counter].Attributes["DeliveryAddress"].Value.ToString().Trim();
                    if (deladdress.Length > 0)
                    {
                        if (deladdress == "D")
                            break;
                    }

                }

                double Pages = Math.Ceiling(numItems / agreeresult.itemsPerPage);
                Pages = Pages == 0 ? 1 : Pages;


                // 26/03/08 rdb for printing in St Lucia need to add a blank page when physical number of pages is odd
                // so that duplicate copy starts on a fresh sheet of paper
                double modCount = numItems % agreeresult.itemsPerPage;
                double fullItemCount = numItems + agreeresult.itemsPerPage - (numItems % 5);
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
                    page.AppendChild(CreateHeader(axml, agreeresult, input, deladdress));
                    page.AppendChild(axml.Document.CreateElement("LINEITEMS"));
                    axml.GetNode("AGREEMENT").AppendChild(page);
                }


                //UAT 179 Agreement details to be printed in Bhasa as well 
                if (input.countrycode == "Y")
                {
                    for (int i = 0; i < Pages; i++)
                    {
                        XmlNode page = axml.Document.CreateElement("BHASAPAGE");
                        if (i == Pages - 1)
                            page.AppendChild(axml.CreateNode("LAST", Boolean.TrueString));
                        else
                            page.AppendChild(axml.CreateNode("LAST", Boolean.FalseString));
                        page.AppendChild(CreateHeader(axml, agreeresult, input, deladdress));
                        page.AppendChild(axml.Document.CreateElement("LINEITEMS"));
                        axml.GetNode("AGREEMENT").AppendChild(page);
                    }
                }





                decimal trade = 0;
                decimal installation = 0;
                decimal GoodsValue = 0;
                decimal GoodsTax = 0;

                XmlNodeList pages = axml.Document.SelectNodes("//PAGE");


                if (allLineItems != null)
                {
                    for (double i = 1; i <= allLineItems.Count; i++)
                    {
                        double pageIndex = Math.Ceiling(Convert.ToDouble(i / agreeresult.itemsPerPage));
                        XmlNode item = allLineItems[Convert.ToInt32(i) - 1];
                        AddLineItemByPage(axml, pages[Convert.ToInt32(pageIndex) - 1], item, ref GoodsValue, ref GoodsTax, agreeresult.localformat, false, false);

                        if (item.Attributes[Tags.Code].Value == "TRADE")
                            trade += Convert.ToDecimal(item.Attributes[Tags.Value].Value);
                        if (item.Attributes[Tags.Code].Value == "511700")
                            installation += Convert.ToDecimal(item.Attributes[Tags.Value].Value);
                    }

                    // 01/04/08 rdb moving this code as Deposit is not populated at this point
                    //GetInsuranceCharge(bAgreement.Deposit, allLineItems); 
                }


                if (input.countrycode == "Y")
                {
                    GoodsValue = 0;
                    GoodsTax = 0;
                    XmlNodeList pagesBhasa = axml.Document.SelectNodes("//BHASAPAGE");
                    if (allLineItems != null)
                    {
                        for (double i = 1; i <= allLineItems.Count; i++)
                        {
                            double pageIndex = Math.Ceiling(Convert.ToDouble(i / agreeresult.itemsPerPage));
                            XmlNode item = allLineItems[Convert.ToInt32(i) - 1];
                            AddLineItemByPage(axml, pagesBhasa[Convert.ToInt32(pageIndex) - 1], item, ref GoodsValue, ref GoodsTax, agreeresult.localformat, true, CheckHCitem(item.Attributes[Tags.Code].Value.ToString()));
                        }

                        // GetInsuranceCharge(bAgreement.Deposit, allLineItems);
                    }
                }


                axml.GetNode("AGREEMENT").AppendChild(CreateFooter(axml, agreeresult, input, allLineItems, GoodsValue, GoodsTax, DTTax, trade, insuranceTax));

                int noCopies = 0;
                if (multiple)
                    noCopies = agreeresult.noCopies;
                else
                    noCopies = 1;

                if (agreeresult.JointName.Length > 0)
                    noCopies += 1;



                axml.CreateCopies(noCopies, agreeresult.custCopies);
                axml.AddCSSPath("AGREEMENTS");
                axml.AddImagePath("AGREEMENTS", Printing.HostName(), Config.CountryCode);



                object uRL = "about:AgreementPrint", flags = 0, targetFrameName = "", postData = 0, headers = 0;

                b.Navigate2(ref uRL, ref flags, ref targetFrameName, ref postData, ref headers);
                IHTMLDocument2 HTMLDocument = (IHTMLDocument2)b.Document;
                var docHtml = axml.Transform();
                HTMLDocument.write(docHtml);

                ((MainForm)FormRoot).PrinterStat.AddButton(b, "Agreement");

                AccountManager.AuditReprint(accountNo, agreementNo, DocumentType.Agreement);
            }
            catch (Exception ex)
            {
                Catch(ex, "Agreement");
            }



        }


        private bool CheckHCitem(string item)
        {
            if ((bool)Country[CountryParameterNames.LoyaltyScheme] && item == LoyaltyDropStatic.VoucherCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private XmlNode CreateHeader(AgreementXML axml, PrintingAgreementResult agree, STL.Common.PrintingAgreementRequest input, string deladdress)
        {
            XmlNode header = axml.Document.CreateElement("HEADER");

            header.AppendChild(axml.CreateNode("JOINTNAME", agree.JointName));
            header.AppendChild(axml.CreateNode("JOINTID", agree.JointID));
            header.AppendChild(axml.CreateNode("RELATIONSHIP", agree.relationship.Trim()));
            header.AppendChild(axml.CreateNode("COUNTRYNAME", agree.CountryName));

            if (agree.customer.Tables["joint"] != null)
            {
                foreach (DataRow r in agree.customer.Tables["joint"].Rows)
                {
                    header.AppendChild(axml.CreateNode("JOINTADDR1", (string)r[CN.Address1]));
                    header.AppendChild(axml.CreateNode("JOINTADDR2", (string)r[CN.Address2]));
                    header.AppendChild(axml.CreateNode("JOINTADDR3", (string)r[CN.Address3]));
                    header.AppendChild(axml.CreateNode("JOINTPOSTCODE", (string)r[CN.PostCode]));
                }
            }


            foreach (DataRow row in agree.customer.Tables["BasicDetails"].Rows)
            {
                string name = (string)row["Title"] + " " + (string)row["FirstName"] + " " + (string)row["LastName"];
                header.AppendChild(axml.CreateNode("NAME", name));
                header.AppendChild(axml.CreateNode("CUSTID", input.customerID));
                header.AppendChild(axml.CreateNode("ACCTNO", input.accountNo));
            }

            foreach (DataRow row in agree.customer.Tables["CustomerAddresses"].Rows)
            {
                if (((string)row[CN.AddressType]).Trim() == "H" &&
                    (string)row[CN.Category] == "CA1")
                {
                    header.AppendChild(axml.CreateNode("ADDR1", (string)row["Address1"]));
                    header.AppendChild(axml.CreateNode("ADDR2", (string)row["Address2"]));
                    header.AppendChild(axml.CreateNode("ADDR3", (string)row["Address3"]));
                    header.AppendChild(axml.CreateNode("POSTCODE", (string)row["PostCode"]));
                }

                if (((string)row[CN.AddressType]).Trim() == deladdress &&
                    ((string)row[CN.AddressType]).Trim() != "H" &&
                   (string)row[CN.Category] == "CA1")
                {
                    header.AppendChild(axml.CreateNode("DELADDR1", (string)row["Address1"]));
                    header.AppendChild(axml.CreateNode("DELADDR2", (string)row["Address2"]));
                    header.AppendChild(axml.CreateNode("DELADDR3", (string)row["Address3"]));
                    header.AppendChild(axml.CreateNode("DELPOSTCODE", (string)row["PostCode"]));
                }
            }

            foreach (DataRow row in agree.customer.Tables["CustomerAddresses"].Rows)
            {

                switch (row["AddressType"].ToString().Trim())
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

            if (agree.AgrTimePrint)
            {
                header.AppendChild(axml.CreateNode("DATE", DateTime.Now.ToString()));
            }

            return header;
        }

        private XmlNode CreateFooter(AgreementXML axml, PrintingAgreementResult agree, STL.Common.PrintingAgreementRequest input, XmlNodeList allLineItems,
            decimal GoodsValue, decimal GoodsTax, decimal DTTax, decimal trade, decimal insuranceTax = 0)
        {
            bool agreementProcessed = false;
            decimal srvcharge = 0;
            decimal insuranceCharge = 0;
            XmlNode footer = axml.Document.CreateElement("FOOTER");

            decimal Deposit;
            decimal Total;

            DataRow row = agree.agreement.Tables[TN.Agreements].Rows[0];

            //LiveWire Call 68210

            Deposit = Convert.ToDecimal(row["Deposit"]);
            Total = Convert.ToDecimal(row["Agreement Total"]);
            if (!agreementProcessed)
            {
                footer.AppendChild(axml.CreateNode("DEPOSIT", Deposit.ToString(DecimalPlaces, agree.localformat)));
                footer.AppendChild(axml.CreateNode("TOTAL", Total.ToString(DecimalPlaces, agree.localformat)));
                if (agree.percenttopay > 0)
                {
                    footer.AppendChild(axml.CreateNode("TOPAY", (Total * (agree.percenttopay / 100)).ToString(DecimalPlaces, agree.localformat)));
                }

                srvcharge = (decimal)row["Service Charge"];

                if (agree.insIncluded)
                    // 25/03/08 rdb insurance calculation needs to be divided by 100
                    insuranceCharge = (agree.chargeablePrice - Deposit) * (agree.insPcent / 100) * (agree.months / 12);
                else
                    foreach (XmlNode node in agree.lineitems)
                        if (node.Attributes[Tags.Code].Value == (string)Country[CountryParameterNames.InsuranceChargeItem])
                            insuranceCharge = Convert.ToDecimal(node.Attributes[Tags.Quantity].Value) * Convert.ToDecimal(node.Attributes[Tags.UnitPrice].Value);


                //cr 1005 Jamaica want insurance include in service charge for agreement printout but not as a separate line
                if (agree.IncInsinServAgrPrint && agree.insIncluded == false) // Only if agreement is false
                {
                    //If Tax is Exclusive, and Insurance is included in Service Charge
                    //add the tax on Insurance to the Charge For Credit amount and deduct from the STAX line as Insurance item in this case is not displayed
                    //on the agreement contract.
                    if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                    {
                        srvcharge += insuranceCharge + insuranceTax;
                    }
                    else
                    {
                        srvcharge += insuranceCharge;
                    }

                }

                footer.AppendChild(axml.CreateNode("DT", srvcharge.ToString(DecimalPlaces, agree.localformat)));
                int InstalNo = Convert.ToInt16(row["instalno"]) - 1;
                InstalNo -= agree.PaymentHolidaysMax;

                // Instalments are either fixed or variable
                string instalments = this.CreateInstalments(row, agree, InstalNo);

                footer.AppendChild(axml.CreateNode("FIRSTINST", ((decimal)row["Instalment Amount"]).ToString(DecimalPlaces, agree.localformat)));
                footer.AppendChild(axml.CreateNode("FINALINST", ((decimal)row["Final Instalment"]).ToString(DecimalPlaces, agree.localformat)));
                footer.AppendChild(axml.CreateNode("INSTALNO", InstalNo.ToString()));
                footer.AppendChild(axml.CreateNode("INSTALMENTS", instalments));

                agreementProcessed = true;
            }




            if (agree.TermsDescription.ToUpper().IndexOf("90 DAYS") != -1 && Convert.ToBoolean(agree.Print90))
                footer.AppendChild(axml.CreateNode("NINETYDAYS", (GoodsValue / 4).ToString(DecimalPlaces, agree.localformat)));

            if (agree.AgreementText.Trim().Length > 0)
            {
                try
                {
                    if (agree.PaymentHolidays)
                    {
                        // Payment holiday text
                        footer.AppendChild(axml.CreateNode("PAYMENTHOLIDAYS", String.Format(agree.AgreementText, new Object[] { agree.PaymentHolidaysMax, agree.PaymentHolidaysMin })));
                    }
                    else
                    {
                        // Any other text
                        footer.AppendChild(axml.CreateNode("PAYMENTHOLIDAYS", agree.AgreementText));
                    }
                }
                catch (System.FormatException)
                {
                    /* users are able to maintain the text so we can't rely on it formating
                     * properly. If there is a formatting error just use the text without formatting */
                    footer.AppendChild(axml.CreateNode("PAYMENTHOLIDAYS", agree.AgreementText));
                }
            }

            footer.AppendChild(axml.CreateNode("GOODSVAL", GoodsValue.ToString(DecimalPlaces, agree.localformat)));
            footer.AppendChild(axml.CreateNode("CREDIT", (GoodsValue - Deposit).ToString(DecimalPlaces, agree.localformat)));
            footer.AppendChild(axml.CreateNode("BALANCE", (Total - Deposit).ToString(DecimalPlaces, agree.localformat)));
            footer.AppendChild(axml.CreateNode("INSURANCE", insuranceCharge.ToString(DecimalPlaces, agree.localformat)));

            switch ((string)Country[CountryParameterNames.ServicePrint])
            {
                case "A":
                    //footer.AppendChild(axml.CreateNode("INTERESTRATE", ServicePCent.ToString((string)Country[CountryParameterNames.ServicePrintDP])+"%"));
                    footer.AppendChild(axml.CreateNode("INTERESTRATE", agree.ServicePCent.ToString("F2") + "%"));
                    break;
                case "M":
                    // 26/03/08 rdb formats with currency sign, change to use decimal places
                    //footer.AppendChild(axml.CreateNode("INTERESTRATE", CalculateMonthlyInterest(ServicePCent).ToString((string)Country[CountryParameterNames.ServicePrintDP])+"%"));
                    footer.AppendChild(axml.CreateNode("INTERESTRATE", CalculateMonthlyInterest(agree.ServicePCent).ToString("F2") + "%"));
                    break;
                case "L":
                    //footer.AppendChild(axml.CreateNode("INTERESTRATE", (ServicePCent/12).ToString((string)Country[CountryParameterNames.ServicePrintDP])+"%"));
                    footer.AppendChild(axml.CreateNode("INTERESTRATE", (agree.ServicePCent / 12).ToString("F2") + "%"));
                    break;
                default:
                    break;
            }

            footer.AppendChild(axml.CreateNode("SERVICEPRINT", (string)Country[CountryParameterNames.ServicePrint]));
            footer.AppendChild(axml.CreateNode("PCENTTOPAY", ((decimal)Country[CountryParameterNames.PercentToPay]).ToString()));

            footer.AppendChild(axml.CreateNode("PRETAXGOODSVAL", (GoodsValue - GoodsTax).ToString(DecimalPlaces, agree.localformat)));
            footer.AppendChild(axml.CreateNode("GOODSVALTAX", GoodsTax.ToString(DecimalPlaces, agree.localformat)));
            footer.AppendChild(axml.CreateNode("PRETAXDT", (srvcharge - DTTax).ToString(DecimalPlaces, agree.localformat)));
            footer.AppendChild(axml.CreateNode("DTTAX", DTTax.ToString(DecimalPlaces, agree.localformat)));

            // 18/03/08 RDB St Lucia Require extra fields 
            footer.AppendChild(axml.CreateNode("SEVENTYPCTOTAL", (Total * .7m).ToString(DecimalPlaces, agree.localformat)));
            // todo extend when other values available
            // 31/03/08 Installation charge 

            //footer.AppendChild(axml.CreateNode("TOTALSTLUCIA", (Total - (Deposit) + (insuranceCharge )).ToString(DecimalPlaces, LocalFormat)));
            footer.AppendChild(axml.CreateNode("TOTALSTLUCIA", (Total - Deposit - srvcharge + insuranceCharge).ToString(DecimalPlaces, agree.localformat)));

            // InsAmount = round((convert(float,AgrmtTotal) * convert(float,AgrmtYears) * (convert(float,InsPCent) / 100)) ,2)
            // where AgrmtYears = instalno/12
            //decimal insAmount = Total * (InstalNo / 12) * (insPcent / 100);
            // So charge for credit extended = agreement.servicechge - insurance charge
            decimal chrgCreditExt = srvcharge - insuranceCharge;
            footer.AppendChild(axml.CreateNode("CHRGCREDITEXT", chrgCreditExt.ToString(DecimalPlaces)));

            // add element for Chargecredit %, Service Percent - Insurance PErcent
            footer.AppendChild(axml.CreateNode("CHRGCREDITINTRATECOMPOUND", CalculateMonthlyInterest(agree.ServicePCent - agree.insPcent).ToString("F2") + "%"));
            footer.AppendChild(axml.CreateNode("CHRGCREDITINTRATE", ((agree.ServicePCent - agree.insPcent) / 12).ToString("F2") + "%"));

            footer.AppendChild(axml.CreateNode("TRADE", (-(trade)).ToString(DecimalPlaces, agree.localformat)));
            footer.AppendChild(axml.CreateNode("DEPTRADETTL", (Deposit - trade).ToString(DecimalPlaces, agree.localformat)));
            footer.AppendChild(axml.CreateNode("INSPCENT", agree.insPcent.ToString("F2") + "%"));
            ////footer.AppendChild(axml.CreateNode("INSPCENT", CalculateMonthlyInterest(insPcent).ToString(DecimalPlaces) + "%"));

            return footer;
        }

        private decimal CalculateMonthlyInterest(decimal annual)
        {
            double m = 0;
            double y = Convert.ToDouble(annual);

            m = 100 * (Math.Pow(((y + 100D) / 100D), (1D / 12D)) - 1D);

            return Convert.ToDecimal(m);
        }

        private string CreateInstalments(DataRow agreementRow, PrintingAgreementResult agree, int instalno)
        {
            // Check for variable payments
            string instalments = "";

            // Load any variable instalments
            if (agree.customer.Tables[TN.VariableInstal] != null && agree.customer.Tables[TN.VariableInstal].Rows.Count > 0)
            {
                foreach (DataRow variableInstal in agree.customer.Tables[TN.VariableInstal].Rows)
                {
                    if (instalments == "")
                        instalments = GetResource("M_AGREEMENTSUMMARYVARIABLE", new Object[] {((decimal)variableInstal[CN.InstalmentNumber]).ToString(DecimalPlaces,agree.localformat),
																									 ((decimal)variableInstal[CN.InstalAmount]).ToString(DecimalPlaces,agree.localformat)});
                    else
                        instalments += " " + GetResource("M_AGREEMENTSUMMARYVARIABLEFOLLOW", new Object[] {((decimal)variableInstal[CN.InstalmentNumber]).ToString(DecimalPlaces,agree.localformat),
																												  ((decimal)variableInstal[CN.InstalAmount]).ToString(DecimalPlaces,agree.localformat)});
                }
                if (instalments != "")
                    instalments += " " + GetResource("M_AGREEMENTSUMMARYVARIABLEFINAL", new Object[] { ((decimal)agreementRow["Final Instalment"]).ToString(DecimalPlaces, agree.localformat) });

                if (instalments == "")
                {
                    // Default to the standard instalment text
                    instalments = GetResource("M_AGREEMENTSUMMARY", new Object[] {instalno.ToString(),
																				 ((decimal)agreementRow["Instalment Amount"]).ToString(DecimalPlaces,agree.localformat),
																				 ((decimal)agreementRow["Final Instalment"]).ToString(DecimalPlaces,agree.localformat)});
                }
            }

            return instalments;
        }
        private void AddLineItemByPage(AgreementXML axml, XmlNode page, XmlNode item, ref decimal GoodsValue, ref decimal GoodsTax, NumberFormatInfo LocalFormat, bool bhasa, bool HCvoucher)
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

                //IP - 20/09/11 - RI - #8218 - CR8201
                if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")
                {
                    descNode.InnerText = item.Attributes[Tags.Description1].Value + " " + item.Attributes[Tags.Brand].Value + " " + item.Attributes[Tags.Style].Value;
                }
                else
                {
                    descNode.InnerText = item.Attributes[Tags.Description1].Value;
                }

                if (HCvoucher)
                {
                    desc2Node.InnerText = item.Attributes[Tags.Description2].Value + " " + item.Attributes[Tags.ContractNumber].Value;
                }
                else
                {
                    desc2Node.InnerText = item.Attributes[Tags.Description2].Value;
                }

                trimNode.InnerText = item.Attributes[Tags.ColourTrim].Value;
                itemNoNode.InnerText = item.Attributes[Tags.Code].Value;
                addToNode.InnerText = item.Attributes[Tags.AddTo].Value;
                acctnoNode.InnerText = item.Attributes["ACCTNO"].Value;
                //typeNode.InnerText = "";
                typeNode.InnerText = item.Attributes[Tags.Type].Value;                              //IP - 11/01/12 - #8649 - LW74016 

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
                    //#11640
                    if (item.Attributes[Tags.Type].Value != IT.Component)
                    {
                        GoodsValue += itemValue;
                    }

                    GoodsTax += Convert.ToDecimal(item.Attributes[Tags.TaxAmount].Value);
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

                    //IP - 20/09/11 - RI - #8218 - CR8201
                    if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")
                    {
                        descNode.InnerText = item.Attributes[Tags.Description1].Value + " " + item.Attributes[Tags.Brand].Value + " " + item.Attributes[Tags.Style].Value;
                    }
                    else
                    {
                        descNode.InnerText = item.Attributes[Tags.Description1].Value;
                    }

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

                            //IP - 20/09/11 - RI - #8218 - CR8201
                            if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")
                            {
                                compDescNode.InnerText = comp.Attributes[Tags.Description1].Value + " " + comp.Attributes[Tags.Brand].Value + " " + comp.Attributes[Tags.Style].Value;
                            }
                            else
                            {
                                compDescNode.InnerText = comp.Attributes[Tags.Description1].Value;
                            }

                            compDescNode2.InnerText = comp.Attributes[Tags.Description2].Value;

                            compItemNode.AppendChild(compQtyNode);
                            compItemNode.AppendChild(compItemNoNode);
                            compItemNode.AppendChild(compDescNode);
                            compItemNode.AppendChild(compDescNode2);
                            compItemNode.AppendChild(compTypeNode);                     //IP - 11/01/12 - #8649 - LW74016 

                            compNode.AppendChild(compItemNode);
                        }
                    }
                }
            }
        }

        protected void PrintDeliveryNotes(DataGrid grid, bool printAll, string printText, int branchNo, bool rePrint)
        {


            object Zero = 0;
            object EmptyString = "";
            int lastBuffNo = 0;

            try
            {
                SetCulture();

                DataView dv = (DataView)grid.DataSource;

                List<STL.Common.DNparameters> DNparameterlist = new List<STL.Common.DNparameters>();
                List<DNLocalparameters> DNLocalparameterlist = new List<DNLocalparameters>();

                for (int i = dv.Count - 1; i >= 0; i--)
                {
                    if (Convert.ToBoolean(dv[i][CN.Released]) || (rePrint && (grid.IsSelected(i) || printAll)))
                    {
                        if (lastBuffNo == 0 || lastBuffNo != Convert.ToInt32(dv[i][CN.BuffNo]))
                        {
                            if (!DNparameterlist.Exists(delegate(STL.Common.DNparameters DNp)
                            {
                                return DNp.acctno == (string)dv[i][CN.acctno] &&
                                       DNp.buffLocn == Convert.ToInt32(dv[i][CN.StockLocn]) &&
                                       DNp.delnotenum == Convert.ToInt32(dv[i][CN.BuffNo]);
                            }))
                            {
                                STL.Common.DNparameters input = new STL.Common.DNparameters();
                                input.acctno = (string)dv[i][CN.acctno];
                                input.user = Credential.UserId;
                                input.userSale = Convert.ToInt32(dv[i]["empeenosale"]);
                                input.printText = printText;
                                //IP - 19/02/10 - CR1072 - LW 69770 - Printing Fixes from 4.3 - Merge
                                if ((printText.Length != 0 || (string)dv[i][CN.DelOrColl] != "")
                                && Convert.ToInt32(dv[i][CN.BuffNo]) > 0)		/* if it's already in the schedule table*/
                                {
                                    input.delnotenum = Convert.ToInt32(dv[i][CN.BuffNo]);
                                    input.buffLocn = Convert.ToInt32(dv[i][CN.StockLocn]);
                                }
                                else
                                {
                                    input.delnotenum = Convert.ToInt32("0");
                                    input.buffLocn = branchNo;
                                }
                                input.dateReqDel = ((DateTime)dv[i][CN.DateReqDel]);
                                input.timeReqDel = (string)dv[i][CN.TimeReqDel];
                                input.delAddressType = ((string)dv[i][CN.DeliveryAddress]).Trim();
                                input.custID = (string)dv[i][CN.CustID];
                                DNparameterlist.Add(input);

                                var localinput = new DNLocalparameters();
                                localinput.locn = Convert.ToString(dv[i][CN.StockLocn]);
                                localinput.buffBranchNo = Convert.ToString(dv[i][CN.BuffBranchNo]); //SC UAT 165  CR1072
                                localinput.customername = (string)dv[i][CN.FirstName] + " " + (string)dv[i][CN.name];
                                localinput.alias = Convert.ToString(dv[i][CN.Alias]);
                                localinput.lastBuffNo = Convert.ToInt32(dv[i][CN.BuffNo]);
                                DNLocalparameterlist.Add(localinput);
                            }
                        }
                    }
                }

                STL.Common.PrintingDN[] DNoutput = Printing.GetDNPrintInfo(DNparameterlist.ToArray());

                if (DNoutput == null || (DNoutput.Length != DNLocalparameterlist.Count || DNparameterlist.Count != DNoutput.Length))
                {
                    MessageBox.Show("Warning! No delivery notes printed. Selection may already be printed. " +
                                    Environment.NewLine +
                                    Environment.NewLine + "Please refresh the screen and try again.");
                }
                else
                {

                    ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(1);
                    AxSHDocVw.AxWebBrowser[] browsers = CreateBrowserArray(1);

                    DeliveryNoteXML_Printing dxml = ProcessPrintdata(DNoutput, DNparameterlist, DNLocalparameterlist);

                    object uRL = "about:DNPrinting", flags = 0, targetFrameName = "", postData = 0, headers = 0;

                    browsers[0].Navigate2(ref uRL, ref flags, ref targetFrameName, ref postData, ref headers);
                    IHTMLDocument2 HTMLDocument = (IHTMLDocument2)browsers[0].Document;
                    HTMLDocument.write(dxml.Transform());

                    //((MainForm)FormRoot).PrinterStat.AddButton(browsers[0], "DeliveryNote");
                    HTMLDocument.execCommand("Print", false, null);

                    //69558 removes lines from the datagrid after a delivery note is printed
                    for (int i = dv.Count - 1; i >= 0; i--)
                    {
                        if (Convert.ToBoolean(dv[i][CN.Released]))
                        {
                            dv.Delete(i);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        protected void PrintDelNotesOnSchedule(AxSHDocVw.AxWebBrowser b, DataGrid grid)
        {
            object Zero = 0;
            object EmptyString = "";

            //            string queryString = "";
            string decimalplaces;

            try
            {
                //XmlDocument deliveryNotes = new XmlDocument();
                //deliveryNotes.LoadXml("<DELIVERYNOTES/>");
                decimalplaces = Printing.GetDecimalPlaces();

                DataView dv = (DataView)grid.DataSource;
                int count = dv.Count;
                SetCulture();
                //DataSet Items;


                //DataView itemview;

                bool first = true;

                ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(1);
                AxSHDocVw.AxWebBrowser[] browsers = CreateBrowserArray(1);

                DeliveryNoteXML_Printing dxml = new DeliveryNoteXML_Printing(Config.CountryCode, decimalplaces);
                dxml.Load("<DELIVERYNOTES/>");
                dxml.AddCSSPath("DELIVERYNOTES");
                dxml.AddImagePath("DELIVERYNOTES", Printing.HostName(), Config.CountryCode);


                DeliveryNoteXML_Printing delNote = dxml.CreateDeliveryNote(Config.CountryCode, decimalplaces);

                for (int i = count - 1; i >= 0; i--)
                {
                    // create a DeliveryNoteRequest node for each selected row
                    if (grid.IsSelected(i))
                    {
                        // only want to print del notes that have not already been printed
                        if (!Convert.ToBoolean(dv[i][CN.Printed]))
                        {
                            STL.Common.PrintingDN[] Dnotes = Printing.GetDNPrintScheduleItems(Convert.ToInt16(dv[i][CN.LoadNo]), Convert.ToInt16(dv[i][CN.BranchNo]), (DateTime)dv[i][CN.DateDel], Credential.UserId);
                            //   itemview = new DataView(Items.Tables[0], "Dateprinted is NULL", "", DataViewRowState.CurrentRows);

                            foreach (STL.Common.PrintingDN dnote in Dnotes)
                            {
                                if (!first)
                                {
                                    delNote.SetNode("DELIVERYNOTE/LAST", "FALSE");
                                    dxml.ImportNode(delNote.DocumentElement, true);
                                }
                                delNote = dxml.CreateDeliveryNote(Config.CountryCode, decimalplaces);
                                DNProcessPrintdata(ref delNote, dnote);
                                // delNote.SetNode("DELIVERYNOTE/LAST", "FALSE");
                                first = false;
                            }


                        }

                        dv[i][CN.TruckID] = "";
                    }
                }

                for (int i = count - 1; i >= 0; i--)
                {
                    if (((string)dv[i][CN.TruckID]).Length == 0)
                        dv.Delete(i);
                }

                delNote.SetNode("DELIVERYNOTE/LAST", "TRUE");
                dxml.ImportNode(delNote.DocumentElement, true);

                if (!first) // Haven't got any print data so don't print.
                {
                    object uRL = "about:DNPrinting", flags = 0, targetFrameName = "", postData = 0, headers = 0;

                    browsers[0].Navigate2(ref uRL, ref flags, ref targetFrameName, ref postData, ref headers);
                    IHTMLDocument2 HTMLDocument = (IHTMLDocument2)browsers[0].Document;
                    HTMLDocument.write(dxml.Transform());


                    ((MainForm)FormRoot).PrinterStat.AddButton(browsers[0], "DeliveryNote");
                }
                //HTMLDocument.execCommand("Print", false, null);
                //if (deliveryNotes.DocumentElement.ChildNodes.Count > 0)
                //{
                //    queryString = "deliveryNotes=" + deliveryNotes.DocumentElement.OuterXml;
                //    queryString = queryString.Replace("&", "%26");
                //    queryString += "&countryCode=" + Config.CountryCode;
                //    url = Config.Url + "WDeliveryNote2.aspx";
                //    object postData = EncodePostData(queryString);
                //    object headers = PostHeader;
                //    b.Navigate(url, ref Zero, ref EmptyString, ref postData, ref headers);
                //}
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Creates the bulk of the delivery note print based on the data supplied populates address and lineitem details
        /// </summary>
        private DeliveryNoteXML_Printing ProcessPrintdata(STL.Common.PrintingDN[] DNoutput, List<STL.Common.DNparameters> DNinput, List<DNLocalparameters> DNinputlocal)
        {
            // Immediate DN's
            string countryCode = String.Empty;
            countryCode = Config.CountryCode;
            DeliveryNoteXML_Printing dxml = new DeliveryNoteXML_Printing(Config.CountryCode, Printing.GetDecimalPlaces());
            dxml.Load("<DELIVERYNOTES/>");
            dxml.AddCSSPath("DELIVERYNOTES");
            dxml.AddImagePath("DELIVERYNOTES", Printing.HostName(), Config.CountryCode);


            for (int i = 0; i <= DNoutput.Length - 1; i++)
            {
                DeliveryNoteXML_Printing delNote = dxml.CreateDeliveryNote(Config.CountryCode, Printing.GetDecimalPlaces());

                STL.Common.PrintingDN output = DNoutput[i];
                STL.Common.DNparameters input = DNinput[i];
                DNLocalparameters local = DNinputlocal[i];

                delNote.SetNode("DELIVERYNOTE/HEADER/ACCTNO", input.acctno);

                if (local.alias.Length > 0)
                {
                    delNote.SetNode("DELIVERYNOTE/HEADER/ALIAS", "ALIAS: " + local.alias);
                }

                delNote.SetNode("DELIVERYNOTE/HEADER/CUSTOMERNAME", local.customername);

                DNLoadAddressDetails(output.Customer.Tables["Address"], delNote, "H");
                DNLoadTelNumbers(output.Customer.Tables["Address"], delNote, input.delAddressType);

                delNote.SetNode("DELIVERYNOTE/FOOTER/ADDCHARGES", output.charges.ToString("F2"));
                delNote.SetNode("DELIVERYNOTE/FOOTER/PAYABLE", output.amountPayable.ToString("F2"));
                delNote.SetNode("DELIVERYNOTE/FOOTER/COD", output.cod ? "Y" : "N");
                delNote.SetNode("DELIVERYNOTE/HEADER/BRANCH", input.buffLocn.ToString());
                delNote.SetNode("DELIVERYNOTE/HEADER/BUFFNO", input.delnotenum.ToString());
                delNote.SetNode("DELIVERYNOTE/HEADER/LOCATION", local.locn.ToString());
                delNote.SetNode("DELIVERYNOTE/HEADER/PRINTED", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());

                delNote.SetNode("DELIVERYNOTE/HEADER/PRINTEDBY", output.printedby);

                int items = 0;
                double qty = 0;
                string delOrColl = "";
                string delDate = "";

                foreach (DataRow row in output.Customer.Tables["Lineitem"].Rows)
                {
                    items++;
                    delNote.AddLineItem(row, output.Customer.Tables["Lineitem"].Rows.Count, ref items);

                    qty += Convert.ToDouble(row[CN.Quantity]);
                    delOrColl = (string)row[CN.DelOrColl];
                    delDate = ((DateTime)row[CN.DateReqDel]).ToLongDateString() + " " + (string)row[CN.TimeReqDel];
                }
                delNote.SetNode("DELIVERYNOTE/HEADER/PRINTTEXT", input.printText);
                delNote.SetNode("DELIVERYNOTE/HEADER/DELDATE", delDate);

                if (qty < 0)
                {
                    delNote.SetNode("DELIVERYNOTE/HEADER/DELTEXT", GetResource("T_COLLTEXT"));
                    delNote.SetNode("DELIVERYNOTE/FOOTER/USER", input.user.ToString() + " ");
                    delNote.SetNode("DELIVERYNOTE/FOOTER/USERNAME", output.printedby);
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

                    delNote.SetNode("DELIVERYNOTE/FOOTER/USER", input.userSale.ToString() + " ");
                    delNote.SetNode("DELIVERYNOTE/FOOTER/USERNAME", output.empname);
                }


                if (input.delAddressType != "H")
                {
                    DNLoadAddressDetails(output.Customer.Tables["Address"], delNote, input.delAddressType);
                }



                if (i == DNoutput.Length - 1)
                {
                    delNote.SetNode("DELIVERYNOTE/LAST", "TRUE");
                }
                else
                {
                    delNote.SetNode("DELIVERYNOTE/LAST", "FALSE");
                }
                dxml.ImportNode(delNote.DocumentElement, true);
            }

            return dxml;
        }


        private void DNProcessPrintdata(ref DeliveryNoteXML_Printing delNote, STL.Common.PrintingDN DNinfo)
        {
            //Scheduled DN's



            if (DNinfo.alias.Length > 0)
            {
                delNote.SetNode("DELIVERYNOTE/HEADER/ALIAS", "ALIAS: " + DNinfo.alias);
            }

            delNote.SetNode("DELIVERYNOTE/HEADER/CUSTOMERNAME", DNinfo.customername);

            delNote.SetNode("DELIVERYNOTE/HEADER/ACCTNO", DNinfo.acctno);

            delNote.SetNode("DELIVERYNOTE/FOOTER/ADDCHARGES", DNinfo.charges.ToString("F2"));
            delNote.SetNode("DELIVERYNOTE/FOOTER/PAYABLE", DNinfo.amountPayable.ToString("F2"));
            delNote.SetNode("DELIVERYNOTE/FOOTER/COD", DNinfo.cod ? "Y" : "N");
            delNote.SetNode("DELIVERYNOTE/HEADER/BRANCH", (DNinfo.origbuffbranchno ?? DNinfo.locn).ToString()); //UAT(5.2) - 568	
            delNote.SetNode("DELIVERYNOTE/HEADER/BUFFNO", DNinfo.buffno.ToString());
            delNote.SetNode("DELIVERYNOTE/HEADER/LOCATION", DNinfo.locn.ToString());
            delNote.SetNode("DELIVERYNOTE/HEADER/PRINTED", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());

            delNote.SetNode("DELIVERYNOTE/HEADER/PRINTEDBY", DNinfo.printedby);

            delNote.SetNode("DELIVERYNOTE/FOOTER/USER", DNinfo.user.ToString() + " ");
            delNote.SetNode("DELIVERYNOTE/FOOTER/USERNAME", DNinfo.empname.ToString());


            int items = 0;
            double qty = 0;
            string delOrColl = "";
            string delDate = "";

            string deladdress = "";

            foreach (DataRow row in DNinfo.Customer.Tables["Lineitem"].Rows)
            {
                items++;
                delNote.AddLineItem(row, DNinfo.Customer.Tables["Lineitem"].Rows.Count, ref items);

                qty += Convert.ToDouble(row[CN.Quantity]);
                delOrColl = (string)row[CN.DelOrColl];
                delDate = ((DateTime)row[CN.DateReqDel]).ToLongDateString() + " " + (string)row[CN.TimeReqDel];

                deladdress = row[CN.DeliveryAddress].ToString().Trim();
            }

            DNLoadAddressDetails(DNinfo.Customer.Tables["Address"], delNote, deladdress);
            DNLoadTelNumbers(DNinfo.Customer.Tables["Address"], delNote, deladdress);

            delNote.SetNode("DELIVERYNOTE/HEADER/PRINTTEXT", DNinfo.printText);
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

            if (deladdress != "H")
            {
                DNLoadAddressDetails(DNinfo.Customer.Tables["Address"], delNote, deladdress);
            }
        }


        private void DNLoadAddressDetails(DataTable dt, DeliveryNoteXML_Printing dxml, string addressType)
        {
            DataRow[] row = dt.Select("AddressType like '%" + addressType + "%'");

            dxml.SetNode("DELIVERYNOTE/HEADER/ADDRESS1", (string)row[0][CN.Address1]);
            dxml.SetNode("DELIVERYNOTE/HEADER/ADDRESS2", (string)row[0][CN.Address2]);
            dxml.SetNode("DELIVERYNOTE/HEADER/ADDRESS3", (string)row[0][CN.Address3]);
            dxml.SetNode("DELIVERYNOTE/HEADER/POSTCODE", (string)row[0][CN.PostCode]);
            dxml.SetNode("DELIVERYNOTE/FOOTER/CUSTNOTES", (string)row[0]["Notes"]);

        }

        private void DNLoadTelNumbers(DataTable dt, DeliveryNoteXML_Printing dxml, string addressType)
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
                        case "D":
                            if (addressType == ((string)row["AddressType"]).Trim())
                            {
                                dxml.SetNode("DELIVERYNOTE/HEADER/DELTEL", GetResource("L_DEL") + ": " + ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"], false);
                            }
                            break;
                        default:
                            break;
                    }
                }

            }
        }

        protected void PrintActionSheet(DataView dv, bool rePrint, string empeeName,
                int empeeno)
        {
            object Zero = 0;
            object EmptyString = string.Empty;

            try
            {
                SetCulture();

                //decimalplaces = Printing.GetDecimalPlaces();

                int count = dv.Count;

                ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(1);
                AxSHDocVw.AxWebBrowser[] browsers = CreateBrowserArray(1);

                ActionSheetXML axml = new ActionSheetXML(Config.CountryCode);
                //, decimalplaces); check

                axml.Load("<ACTIONSHEETS/>");
                axml.AddCSSPath();

                //loading up Courts branches
                ArrayList branches = new ArrayList();
                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                {
                    if (row[CN.StoreType].ToString() == STL.Common.Constants.StoreInfo.StoreType.Courts)
                    {
                        branches.Add(row[CN.BranchNo].ToString());
                    }
                }


                ActionSheetXML ActionNote = axml.CreateActionSheet(Config.CountryCode);
                //, decimalplaces); TO DO CHECK whether need decimal places 

                bool first = true;

                for (int i = count - 1; i >= 0; i--)
                {
                    if (!first) // so don't do pagethrow if first document... 
                    {
                        ActionNote.SetNode("ACTIONSHEET/LAST", "FALSE"); // TODO Check
                        axml.ImportNode(ActionNote.DocumentElement, true);
                    }

                    ActionNote = axml.CreateActionSheet(Config.CountryCode);
                    //, decimalplaces); TO DO CHECK whether need decimal places 
                    var input = new PrintingAction();

                    input.Acctno = (string)dv[i][CN.acctno];
                    input.user = Credential.UserId;

                    //Process output to ASProcessPrintData should really have added all parameters in struct
                    ASProcessPrintdata(ref ActionNote, Printing.GetASPrintInfo(input.Acctno, empeeno), input, empeeno, empeeName);

                    first = false;

                }

                ActionNote.SetNode("ACTIONSHEET/LAST", "TRUE");

                axml.ImportNode(ActionNote.DocumentElement, true);
                if (!first) // Haven't got any print data so don't print.
                {
                    object uRL = "about:ActionPrinting", flags = 0, targetFrameName = "", postData = 0, headers = 0;

                    browsers[0].Navigate2(ref uRL, ref flags, ref targetFrameName, ref postData, ref headers);
                    IHTMLDocument2 HTMLDocument = (IHTMLDocument2)browsers[0].Document;
                    HTMLDocument.write(axml.Transform());

                    HTMLDocument.execCommand("Print", false, null);
                }

            }
            catch
            {
                throw;
            }

        }

        private void ASProcessPrintdata(ref ActionSheetXML axml, STL.Common.PrintingAction ASinfo,
            STL.Common.PrintingAction input, int empeeNo, string empName)
        {
            try
            {

                //int index = 1;

                string culture = String.Empty;
                string acctNo = String.Empty;
                string countryCode = String.Empty;
                string currStatus = String.Empty;
                string storeType = String.Empty;
                storeType = "Non Courts";
                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                {
                    //Create a string that contains the courts branch numbers
                    if (row[CN.StoreType].ToString() == StoreType.Courts)
                    {
                        storeType = "Courts";
                    }
                }

                ActionSheetXML acSheet = axml.CreateActionSheet(countryCode);


                axml.SetNode("ACTIONSHEET/HEADER/STORETYPE", storeType);
                axml.SetNode("ACTIONSHEET/HEADER/ACCTNO", input.Acctno);
                countryCode = Config.CountryCode;
                culture = Config.Culture;

                axml.SetNode("ACTIONSHEET/DETAILS/COLLECTOR", empName);
                axml.SetNode("ACTIONSHEET/DETAILS/DATE", DateTime.Today.ToShortDateString());
                axml.SetNode("ACTIONSHEET/DETAILS/BALANCE", ((decimal)ASinfo.Outstbal).ToString("F2"));
                axml.SetNode("ACTIONSHEET/DETAILS/ARREARS", ((decimal)ASinfo.Arrears).ToString("F2"));
                axml.SetNode("ACTIONSHEET/DETAILS/COSTS", ASinfo.collectionFee.ToString("F2"));
                axml.SetNode("ACTIONSHEET/DETAILS/TOTAL", ASinfo.paymentAmount.ToString("F2"));
                axml.SetNode("ACTIONSHEET/DETAILS/INSTALMENT", ASinfo.instAmount.ToString("F2"));
                axml.SetNode("ACTIONSHEET/DETAILS/DUEDATE", ASinfo.day.ToString());

                axml.SetNode("ACTIONSHEET/HEADER/TITLE", (string)ASinfo.Title + " ");
                axml.SetNode("ACTIONSHEET/HEADER/FIRSTNAME", (string)ASinfo.Firstname + " ");
                axml.SetNode("ACTIONSHEET/HEADER/LASTNAME", (string)ASinfo.Name);


                if (ASinfo.DateLastPaid != STL.Common.Static.Date.blankDate)
                    axml.SetNode("ACTIONSHEET/DETAILS/DATELASTPAID", (ASinfo.DateLastPaid).ToShortDateString());
                else
                    axml.SetNode("ACTIONSHEET/DETAILS/DATELASTPAID", "");


                if (ASinfo.DeadLineDate != STL.Common.Static.Date.blankDate)
                    axml.SetNode("ACTIONSHEET/DETAILS/DEADLINEDATE", (ASinfo.DeadLineDate).ToShortDateString());
                if (ASinfo.privilegeCount > 0)
                    axml.SetNode("ACTIONSHEET/HEADER/PRIVILEGECUSTOMER", "Privilege Club Customer");

                if (ASinfo.Alias != null && ASinfo.Alias.Length > 0)
                    axml.SetNode("ACTIONSHEET/HEADER/ALIAS", "Alias: " + ASinfo.Alias);


                LoadDeliveryDetails(ASinfo.ActionSet.Tables["Deliveries"], axml);
                //private void LoadServiceRequestDetails(DataTable dt, ActionSheetXML axml)
                ASLoadImages(ref axml, ASinfo);
                //LoadServiceRequestDetails
                LoadAddressDetails(axml, ASinfo);
                LoadEmployerDetails(axml, ASinfo);
                LoadOtherCustomers(axml, ASinfo);
                LoadFinancialDetails(axml, ASinfo);


                if ((bool)Country[CountryParameterNames.PrintCharges])
                    axml.SetNode("ACTIONSHEET/DETAILS/PRINTCHARGES", Boolean.TrueString);
                else
                    axml.SetNode("ACTIONSHEET/DETAILS/PRINTCHARGES", Boolean.FalseString);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private void ASLoadImages(ref ActionSheetXML axml, STL.Common.PrintingAction ASinfo)
        {
            if (Convert.ToBoolean(Country[CountryParameterNames.EnablePhotoPrinting]) == true)
            {
                string passedPath = string.Empty;
                if (ASinfo.Photo != string.Empty && ASinfo.Photo != null)
                {


                    if (LoadPhoto(ASinfo.Photo, "p", out passedPath))
                    {
                        axml.SetNode("ACTIONSHEET/HEADER/PHOTO", passedPath);
                    }
                }
                if (ASinfo.SignatureFile != string.Empty && ASinfo.SignatureFile != null)
                {
                    if (LoadPhoto(ASinfo.SignatureFile, "s", out passedPath))
                    {
                        axml.SetNode("ACTIONSHEET/HEADER/SIGNATURE", passedPath);

                    }
                }

            }


        }


        protected void BatchPrintClient(DataView dv)
        {
            var lastSR = "";
            List<string> srPrinted = new List<string>();

            bool itemsadded = false;
            BatchPrintXML batchPrints = new BatchPrintXML(Config.CountryCode);
            batchPrints.Load("<BATCHPRINTS/>");

            int count = dv.Count;

            for (int i = 0; i < count; i++)
            {
                if (lastSR.Length == 0 || lastSR != dv[i][CN.ServiceRequestNo].ToString())
                {

                    BatchPrintXML dn = batchPrints.CreateBatch(Config.CountryCode);

                    dn.SetNode("BATCHPRINT/HEADER/NAME", dv[i][CN.Name].ToString());
                    dn.SetNode("BATCHPRINT/HEADER/SERVICEREQUESTNO", dv[i][CN.ServiceRequestNo].ToString());
                    dn.SetNode("BATCHPRINT/HEADER/ACCTNO", dv[i][CN.AcctNo].ToString());
                    //CR 949/958 2 new fields (Print Location & Action Required) to be added to the print out
                    dn.SetNode("BATCHPRINT/HEADER/PRINTLOCN", dv[i][CN.PrintLocn].ToString());
                    dn.SetNode("BATCHPRINT/HEADER/ACTION", dv[i][CN.ActionRequired].ToString());
                    dn.SetNode("BATCHPRINT/HEADER/HOMETEL", dv[i][CN.TelHome].ToString());
                    dn.SetNode("BATCHPRINT/HEADER/WORKTEL", dv[i][CN.TelWork].ToString());
                    dn.SetNode("BATCHPRINT/HEADER/MOBILETEL", dv[i][CN.TelMobile].ToString());
                    dn.SetNode("BATCHPRINT/HEADER/ADDRESS1", dv[i][CN.Address1].ToString());
                    dn.SetNode("BATCHPRINT/HEADER/ADDRESS2", dv[i][CN.Address2].ToString());
                    dn.SetNode("BATCHPRINT/HEADER/ADDRESS3", dv[i][CN.Address3].ToString());
                    dn.SetNode("BATCHPRINT/HEADER/ADDRESSPC", dv[i][CN.AddressPC].ToString());
                    dn.SetNode("BATCHPRINT/HEADER/DIRECTIONS", dv[i][CN.Directions].ToString());
                    dn.SetNode("BATCHPRINT/HEADER/INSTRUCTIONS", dv[i][CN.Instructions].ToString()); //IP - 05/08/08 - UAT5.1 - UAT(516) - Require 'Special Instructions' to be printed. 

                    //IP - 23/09/11 - RI - #8238 - CR8201
                    if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")
                    {
                        dn.SetNode("BATCHPRINT/HEADER/PRODCODE", dv[i][CN.ProductCode].ToString() + " " + dv[i][CN.ProdDescription].ToString() + " " + dv[i][CN.ProdDescription2].ToString() + " " + dv[i][CN.Brand].ToString() + " " + dv[i][CN.Style].ToString());
                    }
                    else
                    {
                        dn.SetNode("BATCHPRINT/HEADER/PRODCODE", dv[i][CN.ProductCode].ToString() + " " + dv[i][CN.ProdDescription].ToString() + " " + dv[i][CN.ProdDescription2].ToString()); //SC 70524 - Include prod desc
                    }

                    dn.SetNode("BATCHPRINT/HEADER/COMMENTS", dv[i][CN.Comments].ToString());

                    if (dv[i][CN.SlotDate] != DBNull.Value)
                    {
                        DateTime dtSlotDate = new DateTime();
                        dtSlotDate = (DateTime)dv[i][CN.SlotDate];
                        dn.SetNode("BATCHPRINT/HEADER/SLOTDATE", dtSlotDate.DayOfWeek.ToString() + " " + dtSlotDate.Date.ToShortDateString().ToString());
                    }
                    else
                    {
                        dn.SetNode("BATCHPRINT/HEADER/SLOTDATE", dv[i][CN.SlotDate].ToString());
                    }
                    dn.SetNode("BATCHPRINT/HEADER/SLOT", dv[i][CN.Slot].ToString());
                    if (dv[i][CN.DateLogged] != DBNull.Value)
                    {
                        DateTime dtLogDate = new DateTime();
                        dtLogDate = Convert.ToDateTime(dv[i][CN.DateLogged]);
                        dn.SetNode("BATCHPRINT/HEADER/DATELOGGED", dtLogDate.Date.ToShortDateString().ToString());
                    }
                    else
                    {
                        dn.SetNode("BATCHPRINT/HEADER/DATELOGGED", dv[i][CN.DateLogged].ToString());
                    }
                    if (dv[i][CN.PurchaseDate] != DBNull.Value)
                    {
                        dn.SetNode("BATCHPRINT/HEADER/PURCHASEDATE", ((DateTime)dv[i][CN.PurchaseDate]).Date.ToShortDateString().ToString());
                    }
                    else
                    {
                        dn.SetNode("BATCHPRINT/HEADER/PURCHASEDATE", dv[i][CN.PurchaseDate].ToString());
                    }

                    dn.SetNode("BATCHPRINT/HEADER/DEPOSIT", Convert.ToDecimal(dv[i][CN.Deposit]).ToString("N2"));
                    //UAT 403 Balance does not need to be calculated. It is the charge-to account's oustanding balance
                    dn.SetNode("BATCHPRINT/HEADER/BALANCE", Convert.ToDecimal((decimal)dv[i][CN.Balance]).ToString("N2"));
                    //dn.SetNode("BATCHPRINT/HEADER/BALANCE",Convert.ToDecimal((decimal)dv[i][CN.TotalCost] - (decimal)dv[i][CN.Deposit]).ToString("N2");
                    dn.SetNode("BATCHPRINT/HEADER/EW", dv[i][CN.ExtWarranty].ToString());
                    dn.SetNode("BATCHPRINT/HEADER/FYW", dv[i][CN.FYW].ToString());
                    //RM- 19/03/2010 LW72308 add extra fields
                    dn.SetNode("BATCHPRINT/HEADER/SERIALNO", dv[i][CN.SerialNo].ToString());
                    dn.SetNode("BATCHPRINT/HEADER/SERVICEBRANCHNO", dv[i][CN.ServiceBranchNo].ToString());
                    dn.SetNode("BATCHPRINT/HEADER/CHARGETOCUSTOMER", dv[i][CN.ChargeToCustomer].ToString());



                    if (dv[i][CN.Warrantable].ToString() == "1" && (bool)Country[CountryParameterNames.SRPrintShowWrntyAvailable] &&
                        dv[i][CN.ExtWarranty].ToString() == "No" && dv[i][CN.FYW].ToString() == "No")
                    {
                        dn.SetNode("BATCHPRINT/FOOTER/WARRANTABLE", "Warranty Available");
                    }

                    XmlNode li = dn.DocumentElement.SelectSingleNode("PARTS");
                    for (int j = 0; j < count; j++)
                    {
                        if (dv[j][CN.ServiceRequestNo].ToString() == dv[i][CN.ServiceRequestNo].ToString())
                        {
                            XmlDocument c = new XmlDocument();
                            c.LoadXml(XMLTemplates.BatchPrintParts);
                            XmlNode ci = c.DocumentElement;

                            ci.SelectSingleNode("QUANTITY").InnerText = dv[j]["Quantity"].ToString();
                            ci.SelectSingleNode("PARTNO").InnerText = dv[j]["PartNo"].ToString();
                            ci.SelectSingleNode("TYPE").InnerText = dv[j]["PartType"].ToString();
                            ci = dn.ImportNode(ci, true);
                            li.AppendChild(ci);

                        }
                    }

                    if (i == count - 1)
                        dn.SetNode("BATCHPRINT/LAST", "TRUE");
                    else
                    {
                        dn.SetNode("BATCHPRINT/LAST", "FALSE");
                    }

                    batchPrints.ImportNode(dn.DocumentElement, true);
                    itemsadded = true;
                    lastSR = dv[i][CN.ServiceRequestNo].ToString();
                    srPrinted.Add(lastSR);
                }
            }

            int noOfCopies = Convert.ToInt32(Country[CountryParameterNames.SRBatchPrintCopies]);
            batchPrints = batchPrints.CreateCopies(noOfCopies, Config.CountryCode);

            if (itemsadded) // Haven't got any print data so don't print.
            {
                ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(1);
                AxSHDocVw.AxWebBrowser[] browsers = CreateBrowserArray(1);

                object uRL = "about:BatchPrinting", flags = 0, targetFrameName = "", postData = 0, headers = 0;

                browsers[0].Navigate2(ref uRL, ref flags, ref targetFrameName, ref postData, ref headers);
                IHTMLDocument2 HTMLDocument = (IHTMLDocument2)browsers[0].Document;
                HTMLDocument.write(batchPrints.Transform());

                HTMLDocument.execCommand("Print", false, null);

                ServiceManager.SRBatchPrintUpdatePrinted(srPrinted.ToArray(), Convert.ToInt16(Config.BranchCode));

            }

            //queryString = "batch=" + batchPrints.DocumentElement.OuterXml;
            //queryString = queryString.Replace("&", "%26");
            //queryString += "&countryCode=" + Config.CountryCode;
            //queryString += "&branchCode=" + Config.BranchCode;  //CR 1056

            //url = Config.Url + "WBatchPrint.aspx";
            //object postData = EncodePostData(queryString);
            //object headers = PostHeader;
            //((MainForm)FormRoot).browsers[0].Navigate(url, ref Zero, ref EmptyString, ref postData, ref headers);
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

        private void LoadAddressDetails(ActionSheetXML axml, STL.Common.PrintingAction Asinfo)
        {
            DataTable custAddresses = Asinfo.ActionSet.Tables["Address"];

            if (custAddresses != null)
            {
                string addType = "";
                DataRow[] addressRows = custAddresses.Select();

                foreach (DataRow addrRow in addressRows)
                {
                    addType = ((string)addrRow["AddressType"]).Trim();
                    switch (addType)
                    {
                        case "H": axml.SetNode("ACTIONSHEET/HEADER/ADDR1", (string)addrRow[CN.Address1]);
                            axml.SetNode("ACTIONSHEET/HEADER/ADDR2", (string)addrRow[CN.Address2]);
                            axml.SetNode("ACTIONSHEET/HEADER/ADDR3", (string)addrRow[CN.Address3]);
                            axml.SetNode("ACTIONSHEET/HEADER/POSTCODE", (string)addrRow[CN.PostCode]);
                            axml.SetNode("ACTIONSHEET/HEADER/NOTES", (string)addrRow[CN.Notes]);

                            if (((string)addrRow["Phone"]).Length > 0)
                                axml.SetNode("ACTIONSHEET/HEADER/HOMETEL", (string)addrRow["DialCode"] + " " + (string)addrRow["Phone"]);

                            break;
                        case "D": axml.SetNode("ACTIONSHEET/HEADER/DELIVERYADDR1", (string)addrRow[CN.Address1]);
                            axml.SetNode("ACTIONSHEET/HEADER/DELIVERYADDR2", (string)addrRow[CN.Address2]);
                            axml.SetNode("ACTIONSHEET/HEADER/DELIVERYADDR3", (string)addrRow[CN.Address3]);
                            axml.SetNode("ACTIONSHEET/HEADER/DELIVERYPCODE", (string)addrRow[CN.PostCode]);
                            if (((string)addrRow["Phone"]).Length > 0)
                                axml.SetNode("ACTIONSHEET/HEADER/DELIVERYTEL", (string)addrRow["DialCode"] + " " + (string)addrRow["Phone"]);

                            break;
                        case "W": axml.SetNode("ACTIONSHEET/HEADER/WORKADDR1", (string)addrRow[CN.Address1]);
                            axml.SetNode("ACTIONSHEET/HEADER/WORKADDR2", (string)addrRow[CN.Address2]);
                            axml.SetNode("ACTIONSHEET/HEADER/WORKADDR3", (string)addrRow[CN.Address3]);
                            axml.SetNode("ACTIONSHEET/HEADER/WORKPCODE", (string)addrRow[CN.PostCode]);
                            if (((string)addrRow["Phone"]).Length > 0)
                                axml.SetNode("ACTIONSHEET/HEADER/WORKTEL", (string)addrRow["DialCode"] + " " + (string)addrRow["Phone"]);
                            break;
                        case "M":
                            if (((string)addrRow["Phone"]).Length > 0)
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

        private void LoadEmployerDetails(ActionSheetXML axml, STL.Common.PrintingAction Asinfo)
        {
            DataTable employer = Asinfo.ActionSet.Tables["Employer"];
            DataRow[] employerRows = employer.Select();
            foreach (DataRow row in employerRows)
            {
                axml.SetNode("ACTIONSHEET/HEADER/NAME", (string)row[CN.EmployeeName]);
            }

            employerRows = null;
        }

        private void LoadOtherCustomers(ActionSheetXML axml, STL.Common.PrintingAction Asinfo)
        {
            DataTable otherCustomers = Asinfo.ActionSet.Tables["otherCustomers"];
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

        private void LoadFinancialDetails(ActionSheetXML axml, STL.Common.PrintingAction Asinfo)
        {
            string notes = "";
            DataTable dt = Asinfo.ActionSet.Tables["Fintrans"];
            DataRow[] financialRows = dt.Select();

            foreach (DataRow row in financialRows)
            {
                if (row[CN.DateDel] != DBNull.Value)
                    axml.SetNode("ACTIONSHEET/DETAILS/DELIVERYDATE", ((DateTime)row[CN.DateDel]).ToShortDateString());
                else
                    axml.SetNode("ACTIONSHEET/DETAILS/DELIVERYDATE", "");

                axml.SetNode("ACTIONSHEET/DETAILS/ARREARSLEVEL", ((decimal)row[CN.ArrearsLevel2]).ToString("F2"));

                if (Asinfo.Currstatus == "S")
                {
                    decimal balance = (decimal)row[CN.BDWBalance] + (decimal)row[CN.BDWCharges];
                    axml.SetNode("ACTIONSHEET/DETAILS/INTEREST", ((decimal)row[CN.BDWCharges]).ToString("F2"));
                    axml.SetNode("ACTIONSHEET/DETAILS/BALANCE", balance.ToString("F2"));
                }
                else
                    axml.SetNode("ACTIONSHEET/DETAILS/INTEREST", ((decimal)row[CN.Charges]).ToString("F2"));

                notes = (string)row[CN.Notes];
                if (notes.Length > 0)
                    axml.SetNode("ACTIONSHEET/HEADER/INSTRUCTIONS", notes);
                else
                    axml.SetNode("ACTIONSHEET/HEADER/INSTRUCTIONS", "NONE");
            }

            financialRows = null;
        }





    }
}
