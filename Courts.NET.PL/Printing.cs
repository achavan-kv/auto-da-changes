using System;
using System.Data;
using System.Reflection;
using System.Xml;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Printing.AgreementPrinting;

namespace STL.PL
{
    /// <summary>
    /// Class of common printing routines to print agreements, delivery notes,
    /// invoices, customer details etc on the laser printer.
    /// This class uses MS Word for printing, and is redundant due to the
    /// licence requirements of MS Word.
    /// </summary>
    public class Printing
    {
        //private AxSHDocVw.AxWebBrowser browser;
        private CommonForm cf = null;
        private string Error = "";
        private XmlDocument itemDoc = null;
        private XmlNode lineItems = null;

        private string acctno = "";
        private string addr1 = "";
        private string addr2 = "";
        private string addr3 = "";
        private string pCode = "";
        private string homeTel = "";
        private string workTel = "";
        private string mobTel = "";
        private string delText = "REQUIRED DELIVERY:";

        private string title = "";
        private string name = "";
        private string firstname = "";
        private string customerID = "";
        private string accountNum = "";
        private string idnumber = "";
        private string dob = "";
        private string cusnotes = "";

        private int buffno = 0;
        private int branch = 0;

        private string accountType = "";
        private string termsType = "";
        private DateTime dateAcctOpen;
        private decimal agrmtTot = 0;
        private decimal deposit = 0;
        private decimal iAmount = 0;
        private decimal sCharge = 0;
        private decimal instalments = 0;
        private decimal lastInstalment = 0;
        //private double intRate = 0;

        private Word._Application oWord;
        private object oMissing;
        private object agrCopies;
        private object taxCopies;
        private object optional;
        private object oWaitToPrint;
        private object saveChanges;
        private object originalFormat;
        private object routeDocument;

        private DataSet _deliveryLineItems;
        private DataSet items;

        public Printing()
        {
            oMissing = System.Reflection.Missing.Value;
            agrCopies = Convert.ToInt32(cf.Country[CountryParameterNames.NoAgrCopies]);
            taxCopies = Convert.ToInt32(cf.Country[CountryParameterNames.NoTaxCopies]);
            optional = Missing.Value;
            oWaitToPrint = false;
            saveChanges = false;
            originalFormat = Missing.Value;
            routeDocument = Missing.Value;
            cf = new CommonForm();
        }

        private void LoadAddressDetails(DataTable dt)
        {
            string addType = "";
            foreach (DataRow row in dt.Rows)
            {
                addType = ((string)row["AddressType"]).Trim();
                if (addType == "H")
                {
                    addr1 = (string)row["Address1"];
                    addr2 = (string)row["Address2"];
                    addr3 = (string)row["Address3"];
                    pCode = (string)row["PostCode"];

                    if (homeTel.Length == 0)
                        homeTel = (string)row["Phone"];
                }

                if (addType == "W")
                {
                    if (workTel.Length == 0)
                        workTel = (string)row["Phone"];
                }

                if (addType == "M")
                {
                    mobTel = (string)row["Phone"];
                }
            }
        }

        private void LoadCustDetails(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                name = (string)row[CN.FirstName] + " " + (string)row[CN.LastName];
                customerID = (string)row["CustomerID"];
                idnumber = (string)row["IDNumber"];
                title = (string)row["Title"];

                if (Convert.IsDBNull(row[CN.DOB]))
                    dob = "";
                else
                    dob = ((DateTime)row[CN.DOB]).ToShortDateString();
            }
        }

        //private void LoadAgreementDetails(DataTable dt)
        //{
        //    foreach(DataRow row in dt.Rows)
        //    {
        //        agrmtTot = Convert.ToDecimal(row["Agreement Total"]);
        //        deposit = Convert.ToDecimal(row["Deposit"]);
        //        iAmount = Convert.ToDecimal(row["Instalment Amount"]);
        //        sCharge = Convert.ToDecimal(row["Service Charge"]);
        //        instalments = Convert.ToDecimal(row["instalno"]);
        //        lastInstalment = Convert.ToDecimal(row["Final Instalment"]);
        //        accountType = Convert.ToString(row["Account Type"]);
        //        termsType = Convert.ToString(row["Terms Type"]);
        //        dateAcctOpen = (DateTime)(row["Date Sold On"]);
        //    }
        //}

        //private void SetPrintSettings(Word._Document oDoc, int type)
        //{
        //    /*
        //    if(type == Document.Agreement)
        //    {
        //        if(PrintSetUp.AgrPrinterName.Length == 0)
        //        {
        //            PrinterProperties p = new PrinterProperties(type);
        //            p.ShowDialog();

        //            PrintSetUp.AgrPrinterName = p.PrinterName;
        //            PrintSetUp.AgrSource = p.Source;
        //        }
        //    }
        //    else if(type == Document.TaxInvoice)
        //    {
        //        if(PrintSetUp.InvPrinterName.Length == 0)
        //        {
        //            PrinterProperties p = new PrinterProperties(type);
        //            p.ShowDialog();

        //            PrintSetUp.InvPrinterName = p.PrinterName;
        //            PrintSetUp.InvSource = p.Source;
        //        }
        //    }
        //    else if(type == Document.Summary)
        //    {
        //        if(PrintSetUp.SumPrinterName.Length == 0)
        //        {
        //            PrinterProperties p = new PrinterProperties(type);
        //            p.ShowDialog();

        //            PrintSetUp.SumPrinterName = p.PrinterName;
        //            PrintSetUp.SumSource = p.Source;
        //        }

        //        if(PrintSetUp.SumSource.Length > 0)
        //        {
        //            if(PrintSetUp.SumSource.IndexOf("Tray 1") >= 0)
        //            {
        //                oDoc.PageSetup.FirstPageTray = Word.WdPaperTray.wdPrinterUpperBin;
        //            }
        //            else if(PrintSetUp.SumSource.IndexOf("Tray 2") >= 0)
        //            {
        //                oDoc.PageSetup.FirstPageTray = Word.WdPaperTray.wdPrinterLowerBin;
        //            }
        //            else
        //            {
        //                oDoc.PageSetup.FirstPageTray = Word.WdPaperTray.wdPrinterDefaultBin;
        //            }
        //        }
        //    }
        //    else if(type == Document.CollNote)
        //    {
        //        if(PrintSetUp.CollPrinterName.Length == 0)
        //        {
        //            PrinterProperties p = new PrinterProperties(type);
        //            p.ShowDialog();

        //            PrintSetUp.CollPrinterName = p.PrinterName;
        //            PrintSetUp.CollSource = p.Source;
        //        }
        //    }
        //    */
        //}

        //private void CalcMpr(ref decimal mpr, decimal instalamount, decimal fininstalamt,
        //    decimal instalno, decimal agrmttotal)
        //{
        //    decimal diff = Convert.ToDecimal(.001);
        //    decimal balance;
        //    decimal newagr;

        //    int i = 0;
        //    int j = 0;
        //    int deferred = 0;

        //    while(0 < 1)
        //    {
        //        newagr = agrmttotal;
        //        balance = agrmttotal;
        //        for(i = 1; i <= instalno; i++)
        //        {
        //            if(i > deferred && i != instalno)/* normal payment - balance reducing, but interest applied */
        //            {
        //                balance= balance + (balance* mpr) - instalamount;
        //            }
        //            else if(i == deferred)/* account is deferred balance is increasing */
        //            {
        //                balance = balance + (balance* mpr);
        //            }
        //            else if(i == instalno)/* last instalment so use the fininstalamt*/
        //            {
        //                balance= balance + (balance* mpr) - fininstalamt;
        //            }
        //        }

        //        if(balance < 0)/* mpr too high */
        //        {
        //            if(diff < 0)
        //            {
        //                diff = -diff/2;
        //            }
        //        }
        //        else if(balance > 0) /* mpr not high enough*/
        //        {
        //            if(diff > 0)
        //            {
        //                diff = -diff/2;
        //            }
        //        }
        //        else if(balance == 0)
        //        {
        //            break;
        //        }

        //        mpr = mpr + diff;

        //        j = j + 1;
        //        if(j > 500)
        //        {
        //            break;
        //        }
        //    }
        //}

        //private void PrintRFSummaryAgrmnt()
        //{
        //    string path = "";
        //    string holder = "H";
        //    string spouse = "S";

        //    Word._Document oDoc;

        //    oWord = new Word.Application();
        //    oWord.Visible = false;

        //    DownLoadFile(summaryTemplate, ref path);
        //    object oTemplate = path;

        //    oDoc = oWord.Documents.AddOld(ref oTemplate, ref oMissing);

        //    oWord.ActivePrinter = XMLPrint.Printer(PrintType.RFSummary);
        //    oWord.Options.DefaultTray = XMLPrint.Source(PrintType.RFSummary);

        //    /* JPJ
        //    SetPrintSettings(oDoc, Document.Summary);
        //    oWord.ActivePrinter = PrintSetUp.SumPrinterName;
        //    */

        //    DataSet prop = cf.CreditManager.GetProposalStage1(customerID, accountNum, SM.New, holder, out Error);
        //    if(Error.Length>0)
        //        cf.ShowError(Error);
        //    else
        //    {
        //        PrintPersonalDetails(oDoc);

        //        foreach(DataTable dt in prop.Tables)
        //        {
        //            switch(dt.TableName)
        //            {
        //                case TN.Employment:
        //                    PrintEmploymentDetails(oDoc, dt, holder);
        //                    break;
        //                case TN.Bank:
        //                    PrintBankDetails(oDoc, dt);
        //                    break;
        //                case TN.Proposal:
        //                    PrintProposalDetails(oDoc, dt);
        //                    break;
        //                case TN.Customer:
        //                    PrintSummaryAddressDetails(oDoc, dt);
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }

        //        string spouseID = cf.CreditManager.FindSecondApplicant(customerID, accountNum, spouse, out Error);

        //        if(Error.Length>0)
        //            cf.ShowError(Error);
        //        else
        //        {
        //            if(spouseID != customerID)
        //            {
        //                prop = cf.CreditManager.GetProposalStage1(customerID, accountNum, SM.New, spouse, out Error);
        //                foreach(DataTable dt in prop.Tables)
        //                {
        //                    switch(dt.TableName)
        //                    {
        //                        case TN.Employment:
        //                            PrintEmploymentDetails(oDoc, dt, spouse);
        //                            break;
        //                        default:
        //                            break;
        //                    }
        //                }
        //            }
        //        }

        //        PrintRFCreditDetails(oDoc);
        //        LoadAccountSummary(oDoc);

        //        oDoc.PrintOutOld(ref oWaitToPrint,ref optional,ref optional,ref optional,
        //            ref optional,ref optional,ref optional,ref optional,ref optional,
        //            ref optional,ref optional,ref optional,ref optional,ref optional);

        //        CloseWord(ref oDoc);
        //    }
        //}

        //private void PrintPersonalDetails(Word._Document oDoc)
        //{
        //    object oBookMark;

        //    string bName = BookMarks.PRName;
        //    string bDate = BookMarks.CurrDate;

        //    oBookMark = BookMarks.Name;
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = title + " " + name;

        //    oBookMark = BookMarks.CustID;
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = customerID;

        //    oBookMark = BookMarks.Addr1;
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = addr1;
        //    oBookMark = BookMarks.Addr2;
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = addr2;
        //    oBookMark = BookMarks.Addr3;
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = addr3;
        //    oBookMark = BookMarks.PCode;
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = pCode;

        //    oBookMark = BookMarks.DOB;
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = dob.ToString();

        //    oBookMark = BookMarks.HomeTel;
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = homeTel;

        //    oBookMark = BookMarks.WorkTel;
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = workTel;

        //    oBookMark = BookMarks.MobTel;
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = mobTel;

        //    oBookMark = BookMarks.IDNum;
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = customerID;

        //    oBookMark = bName;
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = name;
        //    oBookMark = bDate;
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = DateTime.Today.ToString("dd/MM/yyyy");

        //    oBookMark = bName += Convert.ToString(1);
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = name;
        //    oBookMark = bDate += Convert.ToString(1);
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = DateTime.Today.ToString("dd/MM/yyyy");
        //}

        private void PrintBankDetails(Word._Document oDoc, DataTable dt)
        {
            object oBookMark;

            DateTime bAcctOpened;
            DateTime currDate = DateTime.Today;

            string duration = "";

            foreach (DataRow row in dt.Rows)
            {
                oBookMark = BookMarks.BankName;
                oDoc.Bookmarks.Item(ref oBookMark).Range.Text = (string)row[CN.BankName];

                oBookMark = BookMarks.BankAccNo;
                oDoc.Bookmarks.Item(ref oBookMark).Range.Text = (string)row[CN.BankAccountNo];

                bAcctOpened = (DateTime)row[CN.BankAccountOpened];
                duration = CalculateTimeSpan(currDate, bAcctOpened);

                oBookMark = BookMarks.BankTime;
                oDoc.Bookmarks.Item(ref oBookMark).Range.Text = duration;
            }
        }

        //private void PrintEmploymentDetails(Word._Document oDoc, DataTable dt, string relation)
        //{
        //    object oBookMark;

        //    DateTime dateEmployed;
        //    DateTime currDate = DateTime.Today;

        //    string fullpart = "";
        //    string duration = "";

        //    foreach(DataRow row in dt.Rows)
        //    {
        //        fullpart = (string)row[CN.FullOrPartTime];

        //        if(fullpart == "F")
        //        {
        //            fullpart = "Full Time";
        //        }

        //        if(fullpart == "P")
        //        {
        //            fullpart = "Part Time";
        //        }

        //        if(relation == "H")
        //        {
        //            dateEmployed = (DateTime)row[CN.DateEmployed];
        //            duration = CalculateTimeSpan(currDate, dateEmployed);

        //            oBookMark = BookMarks.EmpStatus;
        //            oDoc.Bookmarks.Item(ref oBookMark).Range.Text = fullpart;

        //            oBookMark = BookMarks.Occu;
        //            oDoc.Bookmarks.Item(ref oBookMark).Range.Text = (string)row[CN.JobTitle];

        //            oBookMark = BookMarks.Employer;
        //            oDoc.Bookmarks.Item(ref oBookMark).Range.Text = (string)row[CN.Employer];

        //            oBookMark = BookMarks.EmpTime;
        //            oDoc.Bookmarks.Item(ref oBookMark).Range.Text = duration;

        //            oBookMark = BookMarks.EmpTel;
        //            oDoc.Bookmarks.Item(ref oBookMark).Range.Text = (string)row[CN.PersDialCode] + " " + (string)row[CN.PersTel];
        //        }
        //        else
        //        {
        //            oBookMark = BookMarks.SPEmpStatus;
        //            oDoc.Bookmarks.Item(ref oBookMark).Range.Text = fullpart;

        //            oBookMark = BookMarks.SPOccu;
        //            oDoc.Bookmarks.Item(ref oBookMark).Range.Text = (string)row[CN.JobTitle];
        //        }
        //    }
        //}

        //private void PrintProposalDetails(Word._Document oDoc, DataTable dt)
        //{
        //    object oBookMark;

        //    foreach(DataRow row in dt.Rows)
        //    {
        //        oBookMark = BookMarks.MStatus;
        //        oDoc.Bookmarks.Item(ref oBookMark).Range.Text = (string)row[CN.MaritalStatus];

        //        oBookMark = BookMarks.Dependants;
        //        oDoc.Bookmarks.Item(ref oBookMark).Range.Text = Convert.ToInt32(row[CN.Dependants]).ToString();

        //        oBookMark = BookMarks.MthlyIncome;
        //        oDoc.Bookmarks.Item(ref oBookMark).Range.Text = Convert.ToDecimal(row[CN.MonthlyIncome]).ToString(cf.DecimalPlaces);
        //    }
        //}

        //private void PrintRFCreditDetails(Word._Document oDoc)
        //{
        //    object oBookMark;

        //    string spLimit = "";
        //    string avLimit = "";
        //    string totInstals = "";

        //    DataTable dtRF = null;
        //    DataSet ds = cf.CustomerManager.GetRFCombinedDetails(customerID, out Error);

        //    if(Error.Length > 0)
        //    {
        //        cf.ShowError(Error);
        //    }
        //    else
        //    {
        //        foreach(DataTable dt in ds.Tables)
        //        {
        //            dtRF = dt;
        //        }

        //        foreach(DataRow row in dtRF.Rows)
        //        {
        //            avLimit = Convert.ToDecimal(row[CN.AvailableCredit]).ToString(cf.DecimalPlaces);
        //            spLimit = Convert.ToDecimal(row[CN.TotalCredit]).ToString(cf.DecimalPlaces);
        //            totInstals = Convert.ToDecimal(row[CN.TotalAllInstalments]).ToString(cf.DecimalPlaces);
        //        }

        //        oBookMark = BookMarks.SPLimit;
        //        oDoc.Bookmarks.Item(ref oBookMark).Range.Text = spLimit;

        //        oBookMark = BookMarks.AVLimit;
        //        oDoc.Bookmarks.Item(ref oBookMark).Range.Text = avLimit;

        //        oBookMark = BookMarks.TotMthInstl;
        //        oDoc.Bookmarks.Item(ref oBookMark).Range.Text = totInstals;
        //    }
        //}

        //private void LoadAccountSummary(Word._Document oDoc)
        //{
        //    DataTable dtAccts = null;
        //    int counter = 0;

        //    DataSet accounts = cf.CustomerManager.CustomerSearch(customerID, "", "", "", "", 100, 1, true, "", out Error);      //CR1084


        //    if(Error.Length > 0)
        //    {
        //        cf.ShowError(Error);
        //    }
        //    else
        //    {
        //        foreach(DataTable dt in accounts.Tables)
        //        {
        //            dtAccts = dt;
        //        }

        //        foreach(DataRow row in dtAccts.Rows)
        //        {
        //            string acctType = (string)row["Type"];
        //            if( acctType == "R")
        //            {
        //                string acctNo = (string)row["AccountNumber"];

        //                //DataSet agreement = cf.AccountManager.GetAgreement(acctNo, out Error);
        //                if(Error.Length > 0)
        //                {
        //                    cf.ShowError(Error);
        //                }
        //                else
        //                {
        //                    if(counter < Document.MaxAccts)
        //                    {
        //                        //PrintAcctSummary(agreement, counter, acctNo, oDoc);
        //                    }
        //                }

        //                lineItems = cf.AccountManager.GetLineItems(acctNo, 1, acctType, Config.CountryCode,  Convert.ToInt16(Config.BranchCode), out Error);
        //                if(Error.Length > 0)
        //                {
        //                    cf.ShowError(Error);
        //                }
        //                else
        //                {
        //                    if(counter < Document.MaxAccts)
        //                    {
        //                        PrintLineSummary(lineItems, counter, oDoc);
        //                    }
        //                }
        //                counter++;
        //            }
        //        }
        //    }
        //}

        //private void PrintAcctSummary(DataSet ds, int counter, string acctNo, Word._Document oDoc)
        //{
        //    object oBookMark;
        //    decimal iAmount = 0;
        //    decimal instalments = 0;
        //    string lastInstalDate = "";
        //    string agrmtDate = "";

        //    string datePurch = BookMarks.DatePurch + Convert.ToString(counter);
        //    string accNum = BookMarks.AcctNo + Convert.ToString(counter);
        //    string mnthInstl = BookMarks.InstlAmt + Convert.ToString(counter);
        //    string numInstl = BookMarks.NumInstl + Convert.ToString(counter);
        //    string dateLast = BookMarks.DateLast + Convert.ToString(counter);

        //    foreach(DataTable dt in ds.Tables)
        //    {
        //        switch(dt.TableName)
        //        {
        //            case TN.Agreements:	
        //                iAmount = Convert.ToDecimal(dt.Rows[0]["Instalment Amount"]);
        //                instalments = Convert.ToDecimal(dt.Rows[0]["instalno"]);
        //                lastInstalDate = ((DateTime)dt.Rows[0]["Date Last Instalment"]).ToString("dd/MM/yy");
        //                agrmtDate = ((DateTime)dt.Rows[0]["Agreement Date"]).ToString("dd/MM/yy");
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    oBookMark = accNum;
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = acctNo;

        //    oBookMark = datePurch;
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = agrmtDate;

        //    oBookMark = mnthInstl;
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = iAmount.ToString(cf.DecimalPlaces);

        //    oBookMark = numInstl;
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = instalments.ToString();

        //    oBookMark = dateLast;
        //    oDoc.Bookmarks.Item(ref oBookMark).Range.Text = lastInstalDate;
        //}

        //private void PrintLineSummary(XmlNode lineItems, int counter, Word._Document oDoc)
        //{
        //    if(lineItems != null)
        //    {
        //        object oBookMark;
        //        int lineCount = 0;

        //        string allItems = "";
        //        string line = BookMarks.Item + Convert.ToString(counter);

        //        //initialise the XML document and the tree view
        //        itemDoc = new XmlDocument();
        //        itemDoc.LoadXml("<ITEMS></ITEMS>");

        //        lineItems = itemDoc.ImportNode(lineItems, true);
        //        itemDoc.ReplaceChild(lineItems, itemDoc.DocumentElement);

        //        foreach(XmlNode item in itemDoc.DocumentElement)
        //        {
        //            if(item.NodeType == XmlNodeType.Element && lineCount < Document.ItemsPerSummary)
        //            {
        //                allItems += item.Attributes[Tags.Description1].Value;
        //                allItems += "\n";
        //            }
        //            lineCount++;
        //        }

        //        oBookMark = line;
        //        oDoc.Bookmarks.Item(ref oBookMark).Range.Text = allItems;
        //    }
        //}

        //private void PrintSummaryAddressDetails(Word._Document oDoc, DataTable dt)
        //{
        //    object oBookMark;

        //    DateTime dateIn;
        //    DateTime pDateIn;
        //    DateTime pDateOut;
        //    DateTime currDate = DateTime.Today;

        //    string duration = "";

        //    foreach(DataRow row in dt.Rows)
        //    {
        //        dateIn = (DateTime)row[CN.DateIn];
        //        pDateIn = (DateTime)row[CN.PrevDateIn];
        //        pDateOut = (DateTime)row[CN.PrevDateOut];

        //        duration = CalculateTimeSpan(currDate, dateIn);
        //        oBookMark = BookMarks.CurrAddrTime;
        //        oDoc.Bookmarks.Item(ref oBookMark).Range.Text = duration;

        //        duration = CalculateTimeSpan(pDateOut, pDateIn);
        //        oBookMark = BookMarks.PrevAddrTime;
        //        oDoc.Bookmarks.Item(ref oBookMark).Range.Text = duration;
        //    }
        //}

        private string CalculateTimeSpan(DateTime startDate, DateTime endDate)
        {
            int timeYears;
            int timeMonths;

            string duration = "";
            string yrtext = "Years";
            string mthtext = "Months";

            timeYears = startDate.Year - endDate.Year;
            timeMonths = startDate.Month - endDate.Month;

            if (timeYears == 1)
                yrtext = "Year";

            if (timeMonths == 1)
                mthtext = "Month";

            if (timeYears > 0 && timeMonths > 0)
            {
                duration = timeYears.ToString()
                    + " " + yrtext + " "
                    + timeMonths.ToString()
                    + " " + mthtext;
            }

            else if (timeYears > 0 && timeMonths == 0)
            {
                duration = timeYears.ToString() + " " + yrtext;
            }

            else if (timeYears == 0 && timeMonths > 0)
            {
                duration = timeMonths.ToString() + " " + mthtext;
            }

            return duration;
        }

        //public void PrintCollectionNotes(string acctNo, int location, decimal quantity, 
        //                                 string itemNo, string itemDescr,decimal price, 
        //                                 string custID, int buffNo)
        //{
        //    string path = "";

        //    object oTemplate;

        //    Word._Document oDoc = null;

        //    try
        //    {
        //        agrCopies = 1;
        //        oWord = new Word.Application();
        //        oWord.Visible = false;
        //        acctno = acctNo;
        //        string notesTemplate = "";

        //        if(oWord.Version == "9.0")
        //        {
        //            notesTemplate = Templates.DeliveryNotes;
        //        }
        //        else
        //        {
        //            notesTemplate = Templates.DeliveryNotes97;
        //        }

        //        DownLoadFile(notesTemplate, ref path);

        //        oTemplate = path;

        //        oDoc = oWord.Documents.AddOld(ref oTemplate, ref oMissing);

        //        //SetPrintSettings(oDoc, Document.CollNote);

        //        oWord.ActivePrinter = XMLPrint.Printer(PrintType.CollectionNote);
        //        oWord.Options.DefaultTray = XMLPrint.Source(PrintType.CollectionNote);

        //        /* JPJ
        //        if(PrintSetUp.CollSource.Length > 0)
        //        {
        //            if(PrintSetUp.CollSource.IndexOf("Tray 1") >= 0)
        //            {
        //                oWord.Options.DefaultTray = "Tray 1";
        //            }
        //            else if(PrintSetUp.CollSource.IndexOf("Tray 2") >= 0)
        //            {
        //                oWord.Options.DefaultTray = "Tray 2";
        //            }
        //            else if(PrintSetUp.CollSource.IndexOf("Tray 3") >= 0)
        //            {
        //                oWord.Options.DefaultTray = "Tray 3";
        //            }
        //            else
        //                oWord.Options.DefaultTray = "Use printer settings";
        //        }
        //        */

        //        delText = "COLLECTION";
        //        buffno = buffNo;
        //        branch = Convert.ToInt32(Config.BranchCode);


        //        LoadDetails(acctNo, custID);
        //        PrintAddress(oDoc);
        //        PrintDeliveryNotesDetails(oDoc);
        //        PrintCollectionItems(oDoc, quantity, itemNo, itemDescr, price, location, buffNo);

        //        oDoc.PrintOutOld(ref oWaitToPrint,ref optional,ref optional,ref optional,
        //            ref optional,ref optional,ref optional,ref agrCopies,ref optional,
        //            ref optional,ref optional,ref optional,ref optional,ref optional);

        //        CloseWord(ref oDoc);
        //    }
        //    catch(Exception ex)
        //    {
        //        if (oDoc != null)
        //            CloseWord(ref oDoc);

        //        cf.Catch(ex, cf.Function);
        //    }
        //}

        public void PrintCollectionItems(Word._Document oDoc, decimal quantity, string itemNo,
                                         string itemDescr, decimal price, int location, int buffNo)
        {
            object oBookMark;

            string tmpItem = BookMarks.Item + "0";
            string tmpQty = BookMarks.Qty + "0";
            string tmpPrice = BookMarks.Price + "0";
            string tmpDescr = BookMarks.Description + "0";

            oBookMark = tmpItem;
            oDoc.Bookmarks.Item(ref oBookMark).Range.Text = itemNo;

            oBookMark = tmpQty;
            oDoc.Bookmarks.Item(ref oBookMark).Range.Text = quantity.ToString("F2");

            oBookMark = tmpPrice;
            oDoc.Bookmarks.Item(ref oBookMark).Range.Text = price.ToString(cf.DecimalPlaces);

            oBookMark = tmpDescr;
            oDoc.Bookmarks.Item(ref oBookMark).Range.Text = itemDescr;

            oBookMark = BookMarks.StockLocation;
            oDoc.Bookmarks.Item(ref oBookMark).Range.Text = location.ToString();

            oBookMark = BookMarks.TimeStamp;
            oDoc.Bookmarks.Item(ref oBookMark).Range.Text = DateTime.Now.ToLongDateString();

            oBookMark = BookMarks.Total;
            oDoc.Bookmarks.Item(ref oBookMark).Range.Text = price.ToString(cf.DecimalPlaces);
        }
    }
}
