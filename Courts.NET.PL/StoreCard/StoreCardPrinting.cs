using System;
namespace STL.PL
{
     partial class CommonForm : System.Windows.Forms.Form
     {
         private class SummaryDetails
         {
             private decimal previousBalance = 0.0m;
             private decimal newTransactions = 0.0m;
             private decimal paymentsReceived = 0.0m;
             private decimal interestcharges = 0.0m;

             public decimal PreviousBalance
             {
                 get { return previousBalance; }
                 set { previousBalance = value; }
             }

             public decimal NewTransactions
             {
                 get { return newTransactions; }
                 set { newTransactions = value; }
             }

             public decimal PaymentsReceived
             {
                 get { return paymentsReceived; }
                 set { paymentsReceived = value; }
             }

             public decimal Interestcharges
             {
                 get { return interestcharges; }
                 set { interestcharges = value; }
             }
         }

         public class Purchases
         {
             public decimal PurchaseValue { get; set; }
             public DateTime PurchaseDate { get; set; }
             public string card           { get; set; }
             public string reference      { get; set; }
         }
         //public void PrintStoreCardStatement(StoreCardStatement statement)
         //{

         //    var summary = new SummaryDetails();

         //    CheckStyleSheet("StoreCardStatement.xslt");

         //    ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(1);
         //    AxSHDocVw.AxWebBrowser[] browsers = CreateBrowserArray(1);
          
         //    try
         //    {

         //        var Purchase = new Purchases();
         //        var PurchaseList = new List<Purchases>();

         //        AgreementXML axml = new AgreementXML(Config.CountryCode, "StoreCardStatement.xslt");
         //        axml.Load("<AGREEMENT><CUSTOMER/><LAST/></AGREEMENT>");

         //        double numTransactions = 0;

         //        //foreach (STL.PL.WS2.FinTrans ft in Account.FinTrans)
         //        //{
         //        //    if (ft.datetrans < Account.StoreCardPaymentDetails.DateLastStatementPrinted)
         //        //    {
         //        //        summary.PreviousBalance = summary.PreviousBalance + ft.transvalue;
         //        //    }
         //        //    else
         //        //    {
         //        //        numTransactions++;

         //        //        if (ft.transtypecode == TransType.Payment || ft.transtypecode == TransType.Correction
         //        //            || ft.transtypecode == TransType.Transfer || ft.transtypecode == TransType.Refund)
         //        //            summary.PaymentsReceived = summary.PaymentsReceived + ft.transvalue;
         //        //        else if (ft.transtypecode == TransType.StoreCardPayment)
         //        //        {
         //        //            summary.NewTransactions = summary.NewTransactions + ft.transvalue;
         //        //            Purchase.card = ft.chequeno;
         //        //            Purchase.PurchaseValue = ft.transvalue;
         //        //            Purchase.reference = "";
         //        //            Purchase.PurchaseDate = ft.datetrans;
         //        //            PurchaseList.Add(Purchase);
         //        //        }
         //        //        else if (ft.transtypecode == TransType.StoreCardInterest)
         //        //            summary.Interestcharges = summary.Interestcharges + ft.transvalue;

         //        //    }

         //        //}  //  ft.datetrans


         //        double Pages = Math.Ceiling(numTransactions / 10);
         //        Pages = Pages == 0 ? 1 : Pages;


         //        for (int i = 0; i < Pages; i++)
         //        {
         //            XmlNode page = axml.Document.CreateElement("PAGE");
         //            if (i == Pages - 1)
         //                page.AppendChild(axml.CreateNode("LAST", Boolean.TrueString));
         //            else
         //                page.AppendChild(axml.CreateNode("LAST", Boolean.FalseString));
         //            page.AppendChild(CreateHeader(axml, statement, summary));
         //            page.AppendChild(axml.Document.CreateElement("LINEITEMS"));
         //            axml.GetNode("AGREEMENT").AppendChild(page);
         //        }

         //        XmlNodeList pages = axml.Document.SelectNodes("//PAGE");
         //        //    XmlSerializer serializer = new XmlSerializer(typeof(Purchases));
         //        XmlSerializer serializer = new XmlSerializer(typeof(Purchases));

         //        //MemoryStream memStream;
         //        //memStream = new MemoryStream();
         //        //XmlTextWriter xmlWriter;
         //        //xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);
         //        //xmlWriter.Namespaces = true;
         //        //serializer.Serialize(xmlWriter,PurchaseList);



         //        // axml.GetNode("AGREEMENT").AppendChild(CreateFooter(axml, agreeresult, input, allLineItems, GoodsValue, GoodsTax, DTTax, trade));

         //        axml.CreateCopies(1, 0);
         //        axml.AddCSSPath("AGREEMENTS");
         //        //axml.AddImagePath("AGREEMENTS", Printing.HostName(), Config.CountryCode);

         //        object uRL = "about:AgreementPrint", flags = 0, targetFrameName = "", postData = 0, headers = 0;

         //        browsers[0].Navigate2(ref uRL, ref flags, ref targetFrameName, ref postData, ref headers);
         //        IHTMLDocument2 HTMLDocument = (IHTMLDocument2)browsers[0].Document;
         //        HTMLDocument.write(axml.Transform());

         //        //      ((MainForm)FormRoot).PrinterStat.AddButton(browsers[0], "Agreement");
         //        HTMLDocument.execCommand("Print", false, null);
         //        //AccountManager.AuditReprint(accountNo, agreementNo, DocumentType.Agreement);
         //    }
         //    catch (Exception ex)
         //    {
         //        Catch(ex, "Agreement");
         //    }
         //}

//        private XmlNode CreateHeader(AgreementXML axml,StoreCardStatement statement,SummaryDetails summary)
//        {
//            XmlNode header = axml.Document.CreateElement("HEADER");

//            header.AppendChild(axml.CreateNode("CardLimit",statement.StoreCardLimit));
//            header.AppendChild(axml.CreateNode("DueDate", Convert.ToString(statement.PaymentDetails.DatePaymentDue)));
//            header.AppendChild(axml.CreateNode("InterestCharges", 23));
//            header.AppendChild(axml.CreateNode("InterestRate", Convert.ToString(statement.PaymentDetails.InterestRate)));
//            header.AppendChild(axml.CreateNode("NewPurchases", Convert.ToString(summary.NewTransactions)));
//            header.AppendChild(axml.CreateNode("Payments", Convert.ToString(-summary.PaymentsReceived)));
//            header.AppendChild(axml.CreateNode("PreviousBalance", Convert.ToString(summary.PreviousBalance)));
//            header.AppendChild(axml.CreateNode("StatementBalance", Convert.ToString(summary.PreviousBalance)));
            
//           // header.AppendChild(axml.CreateNode("JOINTID", agree.JointID));
//           // header.AppendChild(axml.CreateNode("RELATIONSHIP", agree.relationship.Trim()));

//           // stxml.SetNode("STATEMENT/HEADER/CARDLIMIT", "20"); //todo populate card limit...
//           // //    stxml.SetNode("STATEMENT/HEADER/POSTCODE",Account.HomeAddress.cuspocode );
//           // stxml.SetNode("STATEMENT/HEADER/OUTSTANDINGBAL", Convert.ToString(Account.outstbal));
//           // stxml.SetNode("STATEMENT/HEADER/DATE", Convert.ToString(DateTime.Today)); //TODO put  a statement date??
//           // stxml.SetNode("STATEMENT/HEADER/ARREARS", Convert.ToString(Account.arrears));
//           // stxml.SetNode("STATEMENT/HEADER/ACCTNO", Account.acctno);
//           ///* if (agree.customer.Tables["joint"] != null)
//            //{
//            //    foreach (DataRow r in agree.customer.Tables["joint"].Rows)
//            //    {
//            //        header.AppendChild(axml.CreateNode("JOINTADDR1", (string)r[CN.Address1]));
//            //        header.AppendChild(axml.CreateNode("JOINTADDR2", (string)r[CN.Address2]));
//            //        header.AppendChild(axml.CreateNode("JOINTADDR3", (string)r[CN.Address3]));
//            //        header.AppendChild(axml.CreateNode("JOINTPOSTCODE", (string)r[CN.PostCode]));
//            //    }
//            //}
//            //*/

//                header.AppendChild(axml.CreateNode("NAME", statement.CustomerName));
//                header.AppendChild(axml.CreateNode("CUSTID", statement.CustId));
//                header.AppendChild(axml.CreateNode("ACCTNO", statement.PaymentDetails.acctno));
           
//            //foreach (DataRow row in agree.customer.Tables["CustomerAddresses"].Rows)
//            //{
//            //    if (((string)row[CN.AddressType]).Trim() == "H" &&
//            //        (string)row[CN.Category] == "CT1")
//            //    {
//            //        header.AppendChild(axml.CreateNode("ADDR1", (string)row["Address1"]));
//            //        header.AppendChild(axml.CreateNode("ADDR2", (string)row["Address2"]));
//            //        header.AppendChild(axml.CreateNode("ADDR3", (string)row["Address3"]));
//            //        header.AppendChild(axml.CreateNode("POSTCODE", (string)row["PostCode"]));
//            //    }

//            //    if (((string)row[CN.AddressType]).Trim() == deladdress &&
//            //        ((string)row[CN.AddressType]).Trim() != "H" &&
//            //       (string)row[CN.Category] == "CT1")
//            //    {
//            //        header.AppendChild(axml.CreateNode("DELADDR1", (string)row["Address1"]));
//            //        header.AppendChild(axml.CreateNode("DELADDR2", (string)row["Address2"]));
//            //        header.AppendChild(axml.CreateNode("DELADDR3", (string)row["Address3"]));
//            //        header.AppendChild(axml.CreateNode("DELPOSTCODE", (string)row["PostCode"]));
//            //    }
//            //}

//            //foreach (DataRow row in agree.customer.Tables["CustomerAddresses"].Rows)
//            //{

//            //    switch (row["AddressType"].ToString().Trim())
//            //    {
//            //        case "H":
//            //            header.AppendChild(axml.CreateNode("HOMETEL", ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"]));
//            //            break;
//            //        case "W": header.AppendChild(axml.CreateNode("WORKTEL", ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"]));
//            //            break;
//            //        case "M": header.AppendChild(axml.CreateNode("MOBILE", (string)row["Phone"]));
//            //            break;
//            //        default:
//            //            break;
//            //    }
//            //}

//            //if (agree.AgrTimePrint)
//            //{
//            //    header.AppendChild(axml.CreateNode("DATE", DateTime.Now.ToString()));
//            //}

//            return header;
//        }

//        private XmlNode CreateFooter(AgreementXML axml, WS2.Acct Account)
//        {
//            //bool agreementProcessed = false;
//            //decimal srvcharge = 0;
//            //decimal insuranceCharge = 0;
//            XmlNode footer = axml.Document.CreateElement("FOOTER");

//            //decimal Deposit;
//            //decimal Total;

////            DataRow row = agree.agreement.Tables[TN.Agreements].Rows[0];

//            //LiveWire Call 68210

//            //Deposit = Convert.ToDecimal(row["Deposit"]);
//            //Total = Convert.ToDecimal(row["Agreement Total"]);
//            //if (!agreementProcessed)
//            //{
//            //    footer.AppendChild(axml.CreateNode("DEPOSIT", Deposit.ToString(DecimalPlaces, agree.localformat)));
//            //    footer.AppendChild(axml.CreateNode("TOTAL", Total.ToString(DecimalPlaces, agree.localformat)));
//            //    if (agree.percenttopay > 0)
//            //    {
//            //        footer.AppendChild(axml.CreateNode("TOPAY", (Total * (agree.percenttopay / 100)).ToString(DecimalPlaces, agree.localformat)));
//            //    }

//            //    srvcharge = (decimal)row["Service Charge"];

//            //    if (agree.insIncluded)
//            //        // 25/03/08 rdb insurance calculation needs to be divided by 100
//            //        insuranceCharge = (agree.chargeablePrice - Deposit) * (agree.insPcent / 100) * (agree.months / 12);
//            //    else
//            //        foreach (XmlNode node in agree.lineitems)
//            //            if (node.Attributes[Tags.Code].Value == (string)Country[CountryParameterNames.InsuranceChargeItem])
//            //                insuranceCharge = Convert.ToDecimal(node.Attributes[Tags.Quantity].Value) * Convert.ToDecimal(node.Attributes[Tags.UnitPrice].Value);


//            //    //cr 1005 Jamaica want insurance include in service charge for agreement printout but not as a separate line
//            //    if (agree.IncInsinServAgrPrint && agree.insIncluded == false) // Only if agreement is false
//            //    {
//            //        srvcharge += insuranceCharge;
//            //    }

//            //    footer.AppendChild(axml.CreateNode("DT", srvcharge.ToString(DecimalPlaces, agree.localformat)));
//            //    int InstalNo = Convert.ToInt16(row["instalno"]) - 1;
//            //    InstalNo -= agree.PaymentHolidaysMax;

//            //    // Instalments are either fixed or variable
//            //    string instalments = this.CreateInstalments(row, agree, InstalNo);

//            //    footer.AppendChild(axml.CreateNode("FIRSTINST", ((decimal)row["Instalment Amount"]).ToString(DecimalPlaces, agree.localformat)));
//            //    footer.AppendChild(axml.CreateNode("FINALINST", ((decimal)row["Final Instalment"]).ToString(DecimalPlaces, agree.localformat)));
//            //    footer.AppendChild(axml.CreateNode("INSTALNO", InstalNo.ToString()));
//            //    footer.AppendChild(axml.CreateNode("INSTALMENTS", instalments));

//            //    agreementProcessed = true;
//            //}




//            //if (agree.TermsDescription.ToUpper().IndexOf("90 DAYS") != -1 && Convert.ToBoolean(agree.Print90))
//            //    footer.AppendChild(axml.CreateNode("NINETYDAYS", (GoodsValue / 4).ToString(DecimalPlaces, agree.localformat)));

//            //if (agree.AgreementText.Trim().Length > 0)
//            //{
//            //    try
//            //    {
//            //        if (agree.PaymentHolidays)
//            //        {
//            //            // Payment holiday text
//            //            footer.AppendChild(axml.CreateNode("PAYMENTHOLIDAYS", String.Format(agree.AgreementText, new Object[] { agree.PaymentHolidaysMax, agree.PaymentHolidaysMin })));
//            //        }
//            //        else
//            //        {
//            //            // Any other text
//            //            footer.AppendChild(axml.CreateNode("PAYMENTHOLIDAYS", agree.AgreementText));
//            //        }
//            //    }
//            //    catch (System.FormatException)
//            //    {
//            //        /* users are able to maintain the text so we can't rely on it formating
//            //         * properly. If there is a formatting error just use the text without formatting */
//            //        footer.AppendChild(axml.CreateNode("PAYMENTHOLIDAYS", agree.AgreementText));
//            //    }
//            //}

//            //footer.AppendChild(axml.CreateNode("GOODSVAL", GoodsValue.ToString(DecimalPlaces, agree.localformat)));
//            //footer.AppendChild(axml.CreateNode("CREDIT", (GoodsValue - Deposit).ToString(DecimalPlaces, agree.localformat)));
//            //footer.AppendChild(axml.CreateNode("BALANCE", (Total - Deposit).ToString(DecimalPlaces, agree.localformat)));
//            //footer.AppendChild(axml.CreateNode("INSURANCE", insuranceCharge.ToString(DecimalPlaces, agree.localformat)));

//            //switch ((string)Country[CountryParameterNames.ServicePrint])
//            //{
//            //    case "A":
//            //        //footer.AppendChild(axml.CreateNode("INTERESTRATE", ServicePCent.ToString((string)Country[CountryParameterNames.ServicePrintDP])+"%"));
//            //        footer.AppendChild(axml.CreateNode("INTERESTRATE", agree.ServicePCent.ToString("F2") + "%"));
//            //        break;
//            //    case "M":
//            //        // 26/03/08 rdb formats with currency sign, change to use decimal places
//            //        //footer.AppendChild(axml.CreateNode("INTERESTRATE", CalculateMonthlyInterest(ServicePCent).ToString((string)Country[CountryParameterNames.ServicePrintDP])+"%"));
//            //        footer.AppendChild(axml.CreateNode("INTERESTRATE", CalculateMonthlyInterest(agree.ServicePCent).ToString("F2") + "%"));
//            //        break;
//            //    case "L":
//            //        //footer.AppendChild(axml.CreateNode("INTERESTRATE", (ServicePCent/12).ToString((string)Country[CountryParameterNames.ServicePrintDP])+"%"));
//            //        footer.AppendChild(axml.CreateNode("INTERESTRATE", (agree.ServicePCent / 12).ToString("F2") + "%"));
//            //        break;
//            //    default:
//            //        break;
//            //}

//            //footer.AppendChild(axml.CreateNode("SERVICEPRINT", (string)Country[CountryParameterNames.ServicePrint]));
//            //footer.AppendChild(axml.CreateNode("PCENTTOPAY", ((decimal)Country[CountryParameterNames.PercentToPay]).ToString()));

//            //footer.AppendChild(axml.CreateNode("PRETAXGOODSVAL", (GoodsValue - GoodsTax).ToString(DecimalPlaces, agree.localformat)));
//            //footer.AppendChild(axml.CreateNode("GOODSVALTAX", GoodsTax.ToString(DecimalPlaces, agree.localformat)));
//            //footer.AppendChild(axml.CreateNode("PRETAXDT", (srvcharge - DTTax).ToString(DecimalPlaces, agree.localformat)));
//            //footer.AppendChild(axml.CreateNode("DTTAX", DTTax.ToString(DecimalPlaces, agree.localformat)));

//            //// 18/03/08 RDB St Lucia Require extra fields 
//            //footer.AppendChild(axml.CreateNode("SEVENTYPCTOTAL", (Total * .7m).ToString(DecimalPlaces, agree.localformat)));
//            //// todo extend when other values available
//            //// 31/03/08 Installation charge 

//            ////footer.AppendChild(axml.CreateNode("TOTALSTLUCIA", (Total - (Deposit) + (insuranceCharge )).ToString(DecimalPlaces, LocalFormat)));
//            //footer.AppendChild(axml.CreateNode("TOTALSTLUCIA", (Total - Deposit - srvcharge + insuranceCharge).ToString(DecimalPlaces, agree.localformat)));

//            //// InsAmount = round((convert(float,AgrmtTotal) * convert(float,AgrmtYears) * (convert(float,InsPCent) / 100)) ,2)
//            //// where AgrmtYears = instalno/12
//            ////decimal insAmount = Total * (InstalNo / 12) * (insPcent / 100);
//            //// So charge for credit extended = agreement.servicechge - insurance charge
//            //decimal chrgCreditExt = srvcharge - insuranceCharge;
//            //footer.AppendChild(axml.CreateNode("CHRGCREDITEXT", chrgCreditExt.ToString(DecimalPlaces)));

//            //// add element for Chargecredit %, Service Percent - Insurance PErcent
//            //footer.AppendChild(axml.CreateNode("CHRGCREDITINTRATECOMPOUND", CalculateMonthlyInterest(agree.ServicePCent - agree.insPcent).ToString("F2") + "%"));
//            //footer.AppendChild(axml.CreateNode("CHRGCREDITINTRATE", ((agree.ServicePCent - agree.insPcent) / 12).ToString("F2") + "%"));

//            //footer.AppendChild(axml.CreateNode("TRADE", (-(trade)).ToString(DecimalPlaces, agree.localformat)));
//            //footer.AppendChild(axml.CreateNode("DEPTRADETTL", (Deposit - trade).ToString(DecimalPlaces, agree.localformat)));
//            //footer.AppendChild(axml.CreateNode("INSPCENT", agree.insPcent.ToString("F2") + "%"));
//            //////footer.AppendChild(axml.CreateNode("INSPCENT", CalculateMonthlyInterest(insPcent).ToString(DecimalPlaces) + "%"));

//            return footer;
//        }


    }
}
