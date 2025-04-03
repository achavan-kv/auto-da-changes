using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;
using Blue.Cosacs;
using Blue.Cosacs.Repositories;
using STL.BLL;
using STL.BLL.BrokerFinancial;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.EOD;
using STL.Common.Constants.ImportFiles;
using STL.Common.Constants.StoreInfo;
using STL.Common.Constants.TableNames;
using STL.DAL;
using Blue.Cosacs.Services.StoreCard;
using Blue.Cosacs.Shared;
using STL.Common.Constants.AccountTypes;
using Blue.Cosacs.Shared.Services.StoreCard;
using System.Configuration;
using Financial = Blue.Cosacs.Financial;


namespace STL.Batch
{
    /// <summary>
    /// Summary description for EODInterface.
    /// </summary>
    public class EODInterface : CommonObject
    {

        private const string FinancialTransactionInterface = "FIN.TRAN";
        private new int _user = 0;
        public int user
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
            }
        }

        private string _countrycode = "";
        public string CountryCode
        {
            get
            {
                return _countrycode;
            }
            set
            {
                _countrycode = value;
            }
        }

        private int _runNo = 0;
        public int runNo
        {
            get
            {
                return _runNo;
            }
            set
            {
                _runNo = value;
            }
        }

        private string _interfaceName = "";
        public string interfaceName
        {
            get
            {
                return _interfaceName;
            }
            set
            {
                _interfaceName = value;
            }
        }

        private struct Source
        {
            public const string NonStocks = "NonStocks";
            public const string Merchandising = "Merchandising";
        }


        public EODInterface(int user, string countryCode, int runNo, string interfaceName)
            : this(countryCode)
        {
            _user = user;
            _runNo = runNo;
            _interfaceName = interfaceName;
        }

        public EODInterface(string countryCode)
        {
            _countrycode = countryCode;
            if (Cache["Country"] == null)
            {
                DCountry da = new DCountry();
                DataTable dt = da.GetMaintenanceParameters(null, null, CountryCode);

                if (dt.Rows.Count > 0)
                {
                    Cache["Country"] = new CountryParameterCollection(dt);
                    StockItemCache.Invalidate(new StockRepository().GetStockItemCache());
                }
            }
        }

        public class CacheProvider : ICache
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            public object Get(string key)
            {
                if (dictionary.ContainsKey(key))
                    return dictionary[key];

                return null;
            }

            public object Insert(string key, object item)
            {
                dictionary.Add(key, item);
                return item;
            }
        }
        public string RunOption(string configuration, ref bool donefactexport, bool rerun, string filedate)
        {
            string eodResult = EODResult.Fail;
            CountryParameterCache.Cache = new CacheProvider();

            try
            {
                // Any exceptions will be trapped and logged in the calling class EODConfiguration

                // The SQL transaction is opened within each EOD process because
                // some of these processes may need to roll back an error for one
                // customer or account but then continue with the rest. This also reduces
                // the size of each transaction.

                switch (interfaceName.ToUpper())
                {
                    case "ARCHIVE":
                        // Archive Accounts
                        //BAccount bar = new BAccount();
                        eodResult = this.ArchiveAccounts();
                        break;
                    case "ARREARS":
                        // Arrears Calc for current Day
                        BAccount ba = new BAccount();
                        eodResult = ba.EodArrearsCalculation(false);
                        break;
                    case "ARREARSEOD":
                        // Arrears Calculation for next day. 
                        BAccount ba1 = new BAccount();
                        eodResult = ba1.EodArrearsCalculation(true);
                        break;
                    case "AUTOBDW":
                        // Automated Bad Debt Write off
                        BInterfaceControl ic = new BInterfaceControl();
                        eodResult = ic.AutomatedBDW();
                        break;
                    case "BHSRESCORE":
                        DateTime starttime = DateTime.Now;
                        RFEOD rf = new RFEOD(user, CountryCode, runNo, interfaceName);
                        //eodResult = rf.Rescore('B');  //CR2018-005 Equifax, Commented due to add new equifax score card calculation
                        string ScoreType = Convert.ToString(Country[CountryParameterNames.BehaviouralScorecard]);
                        if (ScoreType == "A" || ScoreType == "B" || ScoreType == "P" || ScoreType == "S")

                            eodResult = rf.Rescore('B');
                        else
                            eodResult = rf.Rescore('D');
                        DateTime endtime = DateTime.Now;
                        if (eodResult == "P" && (bool)Country[CountryParameterNames.BehaveApplyEodImmediate])
                        {
                            this.CreateCSharpLetterCSV(runNo, "BHBI", starttime, endtime);
                            this.CreateCSharpLetterCSV(runNo, "BHHL", starttime, endtime);
                            this.CreateCSharpLetterCSV(runNo, "BHHB", starttime, endtime);
                        }
                        ScoreType = String.Empty;
                        break;
                    case "BHSRESCOREAP":
                        starttime = DateTime.Now;
                        RFEOD rf2 = new RFEOD(user, CountryCode, runNo, interfaceName);
                        eodResult = rf2.ApplyBHRescore();
                        endtime = DateTime.Now;
                        if (eodResult == "P" && !(bool)Country[CountryParameterNames.BehaveApplyEodImmediate])
                        {
                            this.CreateCSharpLetterCSV(runNo, "BHBI", starttime, endtime);
                            this.CreateCSharpLetterCSV(runNo, "BHHL", starttime, endtime);
                            this.CreateCSharpLetterCSV(runNo, "BHHB", starttime, endtime);

                        }
                        break;
                    case "CHARGES":

                        // Letter and charges
                        eodResult = this.LetterAndCharges(runNo);

                        if (eodResult == "P")
                        {
                            this.CreateLetterCSV(runNo, "CHARGES");
                        }
                        break;
                    case "COLLECTIONS":
                        // Strategies, Worklists Letters and SMS
                        DCollectionsModule CM = new DCollectionsModule();
                        eodResult = CM.DoEndofDayStrategies();
                        //IP - 16/07/08 - UAT 5.2 - UAT(24) - Letters for collections were not being generated.
                        if (eodResult == "P")
                        {
                            //IP - 18/07/08 - Passing in type 'COLLECTIONS' rather than 'CHARGES' as previously this would take the dates 
                            //of the last charges run and generate letters for the incorrect dates.
                            this.CreateLetterCSV(runNo, "COLLECTIONS");
                        }
                        break;
                    case "COLLCOMMNS":
                        //Collection Commissions 
                        DCollectionsModule CC = new DCollectionsModule();
                        eodResult = CC.CalculateCollectionCommissions(runNo);
                        break;
                    case "SMS":
                        // Create SMS data file
                        DCollectionsModule sms = new DCollectionsModule();
                        eodResult = sms.CreateSMSdatafile(DateTime.Now);                //IP - 16/04/12 - #9929
                        break;
                    case "COS FACT":
                        // Cosacs to FACT Export + Summary Update Control
                        BFactExport bfe = new BFactExport();
                        bfe.User = user;
                        eodResult = bfe.Process(CountryCode, configuration);
                        donefactexport = true;
                        break;
                    case "UPDSMRY":
                        // Cosacs to FACT Export + Summary Update Control
                        eodResult = this.DoFinancialTotals(configuration);

                        break;
                    case "DD DAILY":
                        DDGiro giroDaily = new DDGiro(user, CountryCode, interfaceName, runNo);
                        eodResult = giroDaily.DDEOD_Daily();
                        break;
                    case "DD PAYMENT":
                        DDGiro giroPayment = new DDGiro(user, CountryCode, interfaceName, runNo);
                        eodResult = giroPayment.DDEOD_Payment();
                        break;
                    case "DD REJECTION":
                        DDGiro giroRejection = new DDGiro(user, CountryCode, interfaceName, runNo);
                        eodResult = giroRejection.DDEOD_Rejection();
                        break;
                    case "FACTCOS":
                        // Product File Import
                        eodResult = this.ProductFileImport(donefactexport);
                        break;
                    case "FACTFPR":
                        // Product File Import -- full product refresh, but files identical and process the same as product file import
                        eodResult = this.ProductFileImport(donefactexport);
                        break;
                    case "GENBDW":
                        // Generate Bad Debt Write off accounts
                        BInterfaceControl ic2 = new BInterfaceControl();
                        eodResult = ic2.GenerateBDW();
                        break;
                    case "ORF":
                        rf = new RFEOD(user, CountryCode, runNo, interfaceName);
                        eodResult = rf.Go();
                        break;
                    case "PCLUBTIERS":
                        eodResult = this.QualifyPrivilegeClub();
                        if (eodResult == "P")
                        {
                            this.CreateLetterCSV(runNo, "PCLUBTIERS");
                        }
                        break;
                    case "REBATEDATA":
                        // Create rebate report data
                        BInterfaceControl ic3 = new BInterfaceControl();
                        eodResult = ic3.RebateReport();
                        break;
                    case "REBFORCAST":
                        // Create rebate forecast 
                        eodResult = RebateForecast(DateTime.Now);
                        break;

                    case "SCOREX":
                        // Scorex Procedures
                        Scorex s1 = new Scorex();
                        eodResult = s1.RunScorexExtract();
                        break;
                    case "SEASONEDDATA":
                        // Create Seasoned/Unseasoned report data
                        DInterfaceControl di = new DInterfaceControl();
                        eodResult = di.SeasonedData();
                        break;
                    case "SECURITISE":
                        eodResult = this.SecuritiseEod();
                        break;
                    case "STORDER":
                        // Standing Order Processing
                        eodResult = this.StandingOrder();
                        break;
                    case "STCARDQUAL":
                        // Store Card Qualification
                        // RFEOD rf = new RFEOD(user, countryCode, runNo, interfaceName);
                        eodResult = this.StoreCardQualify(DateTime.Now);
                        break;
                    case "STCARDEXPORT":
                        eodResult = this.StoreCardExport();
                        break;
                    case "STSTATEMENTS":
                        eodResult = this.StoreCardStatements(DateTime.Now);
                        break;
                    case "STINTEREST":
                        eodResult = this.StoreCardInterest(DateTime.Now);
                        break;
                    case "SUMRYDATA":
                        // Create Summary report data
                        //BInterfaceControl ic4 = new BInterfaceControl();
                        eodResult = this.SummaryRptData();
                        break;
                    case "SUMRYDATAFUL":
                        // Create Summary report data - Full Refresh
                        //BInterfaceControl ic5 = new BInterfaceControl();
                        eodResult = this.SummaryRptDataFull();
                        break;
                    case "COMMISSIONS":
                        // Commission Calculation Processing
                        eodResult = this.SalesCommissions();
                        break;
                    case "SERVICEREQ":
                        // Service Request - Balance Special Accounts
                        eodResult = this.SR_BalanceAccounts();
                        break;
                    case "INSTCREDAPP":                     // CR907 jec 31/07/07
                        // Instant Credit Approval Processing
                        BInterfaceControl bi = new BInterfaceControl();
                        eodResult = bi.InstantCredit();
                        if (eodResult == "P")
                        {
                            this.CreateLetterCSV(runNo, "INSTCR");
                        }
                        break;

                    case "CASHLOANQUAL":                     // CR906 jec 04/09/07
                        // Cash Loan Qualification Processing
                        BInterfaceControl loan = new BInterfaceControl();
                        starttime = DateTime.Now;                                                           //IP - 26/10/11 - #3904 - CR1232
                        eodResult = loan.CashLoanQualification();
                        endtime = DateTime.Now;                                                             //IP - 26/10/11 - #3904 - CR1232
                        if (eodResult == "P")
                        {
                            //this.CreateLetterCSV(runNo, "LOAN");                  //IP - 26/10/11 - #3904 - CR1232 - Cash Loan letter - New Customers
                            //this.CreateLetterCSV(runNo, "LoanS");                 //IP - 26/10/11 - #3905 - CR1232 - Cash Loan letter - Previous Customers
                            //this.CreateLetterCSV(runNo, "LoanP");                 //IP - 27/10/11 - #3906 - CR1232 - Cash Loan letter - Current Customers 

                            this.CreateLetterCSV(runNo, "CASHLOANQUAL");            //IP - 24/02/12 - #9601 - Calling this with "CASHLOANQUAL" should pickup New Customers, Previous Customers, Current Customers 
                        }
                        break;
                    //IP - 23/04/08 - UAT(260)
                    case "ADHOC":
                        //Adhoc scripts processing
                        DInterfaceControl dic = new DInterfaceControl();
                        eodResult = dic.RunEODAdHocScripts(true);
                        break;
                    //IP - CR946 
                    case "BROKERX":
                        //Broker Financial Export Processing
                        eodResult = this.BrokerFinancialExport();
                        break;

                    case "COSACS2RI":
                        // Create RI Interface data files
                        RIinterface cri = new RIinterface();
                        eodResult = cri.CreateCos2RIExport(runNo, interfaceName, rerun);
                        break;

                    case "RI2COSACS":
                        // Process RI Interface data files
                        RIinterface ric = new RIinterface();
                        eodResult = ric.RI2CosacsImport(runNo, interfaceName, rerun, filedate);
                        break;

                    case "WKLYTRAD":
                        // Weekly Trading Report
                        BInterfaceControl wtr = new BInterfaceControl();
                        eodResult = wtr.WeeklyTrading();
                        break;

                    case "CASHIERWO":
                        eodResult = new CashierTotalRepository().CashierTotalWriteOff();
                        break;

                    case "HYPERION":                     // CR10450 jec 12/10/12
                        // Hyperion data extract Processing
                        BInterfaceControl hyp = new BInterfaceControl();
                        eodResult = hyp.HyperionExtract();
                        break;
                    case "ECOMMERCE":
                        // #13890 jec  Online Product Export
                        eodResult = this.OnlineProductExport();
                        break;
                    case "RDYASTEXP":                                       //#13719
                        eodResult = this.ReadyAssistExport();
                        break;

                    case FinancialTransactionInterface:
                        eodResult = this.FinancialTransaction(runNo);       // #19037
                        break;

                    default:
                        // Back up procedures
                        string strInterfaceName = interfaceName.ToUpper().Substring(0, 4);
                        if (strInterfaceName == "BACK")
                        {
                            eodResult = this.DatabaseBackup(interfaceName.ToUpper());
                        }

                        break;
                }
            }
            catch
            {
                throw;
            }
            return eodResult;
        }

        private string FinancialTransaction(int runNo)
        {
            try
            {
                var export = new Financial.Export();
                var bfe = new BBrokerFinancialExport();

                bfe.ExportBrokerData(export.Run(runNo)); 
                export.MarkAsExported();
            }
            catch (Exception ex)
            {

                var message = string.Empty;
                if (ex.StackTrace != null)
                {
                    message = ex.StackTrace + "\n\n";
                }
                message += ex.Message;

               BInterfaceError ie = new BInterfaceError(
                  null,
                  null,
                  FinancialTransactionInterface,
                  runNo,
                  DateTime.Now,
                  message,
                  "E");

                return EODResult.Fail;
            }

            return EODResult.Pass;
        }

        private string RebateForecast(DateTime periodEnd)
        {
            SqlConnection conn = null;
            SqlTransaction trans = null;
            BAccount acct = new BAccount();

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        acct.User = 99999;
                        acct.RunRebateForecastReports(conn, trans, periodEnd.ToString());
                        trans.Commit();
                        break;
                    }
                    catch
                    {
                    }
                } while (retries <= maxRetries);

            }
            catch
            {
                return EODResult.Fail;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return EODResult.Pass;
        }


        private string DoFinancialTotals(string configuration)
        {
            SqlConnection conn = new SqlConnection(Connections.Default);
            try
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                BFactExport finexport = new BFactExport();

                finexport.User = user;
                finexport.SummaryRunno = this.runNo;
                finexport.DoFinancialTotals(conn, trans, configuration);
                trans.Commit();
            }
            catch
            {
                return EODResult.Fail;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return EODResult.Pass;
        }

        private string SecuritiseEod()
        {
            string eodResult = EODResult.Pass;
            string progress = "Securitising accounts ...";
            Console.WriteLine(progress);

            // Load Country Parameters
            BCountry c = new BCountry();
            c.GetMaintenanceParameters(null, null, _countrycode);
            SqlConnection conn = null;


            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            BAccount secAccounts = new BAccount();
                            secAccounts.Cache = c.Cache;
                            secAccounts.User = User;
                            secAccounts.SecuritiseAccounts(conn, trans, "SECURITISE", runNo);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock && retries < maxRetries)
                        {
                            retries++;
                            if (conn.State != ConnectionState.Closed)
                                conn.Close();
                        }
                        else
                        {
                            StringBuilder errorMessages = new StringBuilder();
                            for (int i = 0; i < ex.Errors.Count; i++)
                            {
                                errorMessages.Append("Message: " + ex.Errors[i].Message + "\n" +
                                    "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                    "Source: " + ex.Errors[i].Source + "\n" +
                                    "Procedure: " + ex.Errors[i].Procedure + "\n");
                            }

                            throw new STLException(errorMessages.ToString());
                        }
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            progress = "Data Securitisation finished.";
            Console.WriteLine(progress);
            return eodResult;
        }

        private string SummaryRptData()
        {
            string eodResult = EODResult.Pass;
            string progress = "Populating summary tables ...";
            Console.WriteLine(progress);

            // Set Full refresh to false
            DInterfaceControl di = new DInterfaceControl();
            eodResult = di.SummaryRptData(0);

            //string dbname = GetDatabaseName();

            try
            {
                di.SummaryRun();                                                        //IP - 08/11/11 - this now replaces below.
                //    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                //    proc.EnableRaisingEvents = false;

                //    // Running Summary_Sql.Bat Batch file
                //    proc.StartInfo.FileName = @"D:\cosprog\eod\summary\Summary_sql.bat";
                //    proc.StartInfo.Arguments = dbname;
                //    proc.StartInfo.UseShellExecute = true;

                //    proc.Start();
                // Wait for completion
                //do
                //{
                //    // Forces the Process component to get a new set 
                //    // of property values.
                //    //proc.Refresh();
                //    // Writes the property value to the console screen.
                //    //Console.WriteLine(proc.WorkingSet.ToString());
                //    // Waits 1 second before running the next loop.
                //    System.Threading.Thread.Sleep(1000);
                //} while (proc.HasExited == false);

                //// Checking for any error in the log file 
                //if (!File.Exists(@"D:\cosprog\eod\summary\summary.log"))
                //{
                //    return EODResult.Fail;
                //}


                //foreach (var line in File.ReadAllLines(@"D:\cosprog\eod\summary\summary.log"))
                //{
                //    if (line.IndexOf("FAIL", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                //          line.IndexOf("MSG", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                //          line.IndexOf("DENIED", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                //          line.IndexOf("TIMEOUT", StringComparison.InvariantCultureIgnoreCase) != -1)
                //    {
                //        return EODResult.Fail;
                //    }
                //}


                //FileStream fs = File.OpenRead(@"D:\cosprog\eod\summary\summary.log");
                //    fs.Position = 0;
                //    byte[] bytes = new byte[fs.Length];
                //    fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                //    System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                //    string byteStr = encoder.GetString(bytes);
                //    byteStr = byteStr.ToLower(); //KEF 68978 Added to eliminate Case mis-match
                //    if (byteStr.IndexOf("fail") != -1 ||
                //        byteStr.IndexOf("msg") != -1 ||
                //        byteStr.IndexOf("denied") != -1 ||
                //        byteStr.IndexOf("timeout") != -1)
                //    {
                //        eodResult = EODResult.Fail;
                //        throw new Exception("Please check D:\\cosprog\\eod\\summary\\Summary.log for error");
                //    }
                //    fs.Close();
                //}

            }
            catch (Exception)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw;
            }

            progress = "Populate Summary Data Finished .";
            Console.WriteLine(progress);
            return eodResult;
        }


        private string SummaryRptDataFull()
        {
            string eodResult = EODResult.Pass;
            string progress = "Populating summary tables for full refresh...";
            Console.WriteLine(progress);

            // Set Full refresh to true
            DInterfaceControl di = new DInterfaceControl();
            eodResult = di.SummaryRptData(1);

            //string dbname = GetDatabaseName();

            try
            {

                di.SummaryRun();                                                        //IP - 08/11/11 - this now replaces below.

                //{
                //    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                //    proc.EnableRaisingEvents = false;

                //    // Running Summary_Sql.Bat Batch file
                //    proc.StartInfo.FileName = @"D:\cosprog\eod\summary\Summary_sql.bat";
                //    proc.StartInfo.Arguments = dbname;
                //    proc.StartInfo.UseShellExecute = true;

                //    proc.Start();
                //    // Wait for completion
                //    do
                //    {
                //        // Forces the Process component to get a new set 
                //        // of property values.
                //        //proc.Refresh();
                //        // Writes the property value to the console screen.
                //        //Console.WriteLine(proc.WorkingSet.ToString());
                //        // Waits two seconds before running the next loop.
                //        System.Threading.Thread.Sleep(2000);
                //    } while (proc.HasExited == false);

                //    // Checking for any error in the log file 
                //    if (File.Exists(@"D:\cosprog\eod\summary\summary.log"))
                //    {
                //        FileStream fs = File.OpenRead(@"D:\cosprog\eod\summary\summary.log");
                //        fs.Position = 0;
                //        byte[] bytes = new byte[fs.Length];
                //        fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                //        System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                //        string byteStr = encoder.GetString(bytes);
                //        if (byteStr.IndexOf("fail") != -1 ||
                //            byteStr.IndexOf("msg") != -1 ||
                //            byteStr.IndexOf("denied") != -1 ||
                //            byteStr.IndexOf("Timeout") != -1)
                //        {
                //            eodResult = EODResult.Fail;
                //            throw new Exception("Please check D:\\cosprog\\eod\\summary\\Summary.log for error");
                //        }
                //        fs.Close();
                //    }
                //}
            }
            catch (Exception)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw;
            }

            progress = "Populate Summary Data Fully Refresh completed .";
            Console.WriteLine(progress);
            return eodResult;
        }

        private string ArchiveAccounts()
        {
            string eodResult = EODResult.Pass;
            string progress = "Running Archiving ...";
            Console.WriteLine(progress);

            try
            {
                // Archive Accounts
                BAccount bar = new BAccount();
                eodResult = bar.ArchiveAccounts();

                // Reindexing
                DInterfaceControl di = new DInterfaceControl();
                eodResult = di.Archiving();
            }
            catch (Exception)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw;
            }

            progress = "Archiving completed.";
            Console.WriteLine(progress);
            return eodResult;
        }

        private string LetterAndCharges(int runNo)
        {
            string eodResult = EODResult.Pass;
            string progress = "Running Letters and Charges...";
            Console.WriteLine(progress);

            try
            {
                // Run Letters and charges
                BAccount bar = new BAccount();
                eodResult = bar.LetterAndCharges(runNo);
            }
            catch (Exception)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw;
            }

            progress = "Letters and Charges generated now writing .csv letter files";
            Console.WriteLine(progress);
            return eodResult;
        }
        private string CreateLetterCSV(int runNo, string type)
        {
            string eodResult = EODResult.Pass;
            string progress = "Generating Letter Files";
            Console.WriteLine(progress);

            try
            {
                // Run Letters and charges
                BAccount bar = new BAccount();
                bar.LettersGenerateCSVfiles(runNo, type);
            }
            catch (Exception)
            {
                eodResult = EODResult.Fail;
                // Pgass this on for logging in EODConfiguration
                throw;
            }

            progress = "Letters Generated";
            Console.WriteLine(progress);
            return eodResult;
        }
        /// <summary>
        /// Creates a letters on the application server not using BCP
        /// </summary>
        /// <param name="runNo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string CreateCSharpLetterCSV(int runNo, string lettercode,
            DateTime starttime, DateTime endtime)
        {
            string eodResult = EODResult.Pass;
            string progress = "Generating Letter Files";
            Console.WriteLine(progress);

            try
            {
                // Run Letters and charges
                BAccount bar = new BAccount();
                bar.LettersGenerateCSharpCSVfiles(runNo, lettercode, starttime, endtime);
            }
            catch (Exception)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw;
            }

            progress = "Letters Generated";
            Console.WriteLine(progress);
            return eodResult;
        }




        private string QualifyPrivilegeClub()
        {
            string eodResult = EODResult.Pass;
            string progress = "Promoting / Demoting Loyalty Club Customers ...";
            Console.WriteLine(progress);

            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (trans = conn.BeginTransaction())
                        {
                            BCustomer pcCustomer = new BCustomer();
                            pcCustomer.User = User;
                            pcCustomer.QualifyPrivilegeClub(conn, trans);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock && retries < maxRetries)
                        {
                            retries++;
                            if (conn.State != ConnectionState.Closed)
                                conn.Close();
                        }
                        else
                        {
                            StringBuilder errorMessages = new StringBuilder();
                            for (int i = 0; i < ex.Errors.Count; i++)
                            {
                                errorMessages.Append("Message: " + ex.Errors[i].Message + "\n" +
                                    "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                    "Source: " + ex.Errors[i].Source + "\n" +
                                    "Procedure: " + ex.Errors[i].Procedure + "\n");
                            }

                            throw new STLException(errorMessages.ToString());
                        }
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            progress = "Loyalty Club qualification finished.";
            Console.WriteLine(progress);
            return eodResult;
        }


        //private string ScorexEod()
        //{
        //    string eodResult = EODResult.Pass;
        //    SqlConnection conn = null;

        //    try
        //    {
        //        conn = new SqlConnection(Connections.Default);
        //        do
        //        {
        //            try
        //            {
        //                conn.Open();
        //                using (SqlTransaction trans = conn.BeginTransaction())
        //                {
        //                    // Execute Stored procedures
        //                    DScorex ds = new DScorex();
        //                    ds.RunScorexExtract(conn, trans);
        //                    trans.Commit();
        //                }
        //                break;
        //            }
        //            catch (SqlException ex)
        //            {
        //                if (ex.Number == Deadlock && retries < maxRetries)
        //                {
        //                    retries++;
        //                    if (conn.State != ConnectionState.Closed)
        //                        conn.Close();
        //                }
        //                else
        //                {
        //                    StringBuilder errorMessages = new StringBuilder();
        //                    for (int i = 0; i < ex.Errors.Count; i++)
        //                    {
        //                        errorMessages.Append("Message: " + ex.Errors[i].Message + "\n" +
        //                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
        //                            "Source: " + ex.Errors[i].Source + "\n" +
        //                            "Procedure: " + ex.Errors[i].Procedure + "\n");
        //                    }

        //                    throw new STLException(errorMessages.ToString());
        //                }
        //            }
        //        } while (retries <= maxRetries);
        //    }
        //    catch (Exception)
        //    {
        //        eodResult = EODResult.Fail;
        //        // Pass this on for logging in EODConfiguration
        //        throw;
        //    }
        //    finally
        //    {
        //        if (conn.State != ConnectionState.Closed)
        //            conn.Close();
        //    }
        //    return eodResult;
        //}


        // Database backup
        private string DatabaseBackup(string interfaceName)
        {
            string eodResult = EODResult.Pass;
            try
            {
                BInterfaceControl ic = new BInterfaceControl();
                ic.DatabaseBackup(interfaceName);
            }
            catch (Exception)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw;
            }
            finally
            {

            }
            return eodResult;

        }

        //private string GetDatabaseName()
        //{
        //    return Connections.DefaultDatabaseName;
        //}

        public string StoreCardQualify(DateTime rundate)
        {
            string eodResult = EODResult.Pass;

            try
            {

                using (var conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        var storeCardRepository = new StoreCardRepository();
                        var ds = storeCardRepository.EODQualify(conn, trans, rundate);
                        var stcustomers = ds.Tables[0];


                        if (stcustomers.Rows.Count > 0)
                        {
                            var Acct = new BAccount();
                            bool rescore;
                            var ba = new BAccount();
                            string accountNo = "";

                            foreach (DataRow dr in stcustomers.Rows)
                            {

                                var SCardRep = new StoreCardRepository();

                                var cust = SCardRep.ExistingAccount(Convert.ToString(dr[CN.CustID]), AT.StoreCard, conn, trans);

                                if (cust != null && cust.acctno != null)
                                {
                                    accountNo = cust.acctno;
                                    ba.ReverseCancellation(conn, trans, accountNo, "RS", "Reverse Store Card");
                                    ba.UpdateStatus(conn, trans, accountNo, DateTime.Now, "1");
                                    storeCardRepository.AccountStatusUpdate(accountNo, StoreCardAccountStatus_Lookup.CardToBeIssued.Code, conn, trans);
                                }

                                if (accountNo == "" && Convert.ToBoolean(Country[CountryParameterNames.StoreCardIssueCardPreAppr]))
                                    accountNo = ba.CreateCustomerAccount(conn, trans, CountryCode, Convert.ToInt16(dr[CN.BranchNo]), Convert.ToString(dr[CN.CustID]), "T", this.user, false, out rescore, "PreApprove");


                                var existing = storeCardRepository.ExistingCard(conn, trans, accountNo);

                                if (existing == null && Convert.ToBoolean(Country[CountryParameterNames.StoreCardIssueCardPreAppr]))
                                {
                                    var card = storeCardRepository.CreateandScore
                                        (new StoreCardNew
                                            {
                                                AcctNo = accountNo.Replace("-", ""),
                                                CustId = Convert.ToString(dr[CN.CustID]),
                                                Source = "PreApprove",
                                                User = user
                                            }, conn, trans
                                        );
                                    if (Convert.ToBoolean(Country[CountryParameterNames.StoreCardActivate]))
                                        storeCardRepository.Activate(new ActivateRequest
                                        {
                                            BranchNo = Convert.ToInt16(card.AcctNo.Substring(0, 3)),
                                            Reason = "PreApprove",
                                            Date = DateTime.Now,
                                            CardNumber = card.CardNumber,
                                            EmpeeNo = -116

                                        }, conn: conn, trans: trans);
                                }
                                accountNo = "";
                                cust = null;
                                existing = null;
                            }

                        }

                        ds.Clear();
                        var GetQualified = new Blue.Cosacs.StoreCardGetRecentlyQualified(conn, trans);
                        ds = GetQualified.ExecuteDataSet();

                        string strfilepath =
                        (string)Country[CountryParameterNames.SystemDrive] + "\\LTR" + "STQ" +
                            Convert.ToString(runNo) + ".csv";

                        DTransfer Transfer = new DTransfer();
                        Transfer.ExportToCSV(ds.Tables[0], strfilepath);
                        //BAccount Acct = new BAccount();
                        //Acct.LettersGenerateCSharpCSVfiles(runNo, "STQ", rundate, rundate);
                        trans.Commit();
                    }

                }

            }
            catch (Exception)
            {
                eodResult = EODResult.Fail;

                throw;
            }
            return eodResult;

        }

        public string StoreCardStatements(DateTime rundate)
        {
            string eodResult = EODResult.Pass;
            try
            {
                using (var conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        //rundate = Convert.ToDateTime(rundate.AddHours(6).ToShortDateString());
                        rundate = rundate.AddHours(6);
                        rundate = new DateTime(rundate.Year, rundate.Month, rundate.Day, rundate.Hour, rundate.Minute, 0);
                        StoreCardRepository scr = new StoreCardRepository();
                        var ds = scr.EODStatements(conn, trans, rundate);

                        string strfilepath =
                         (string)Country[CountryParameterNames.SystemDrive] + "\\StoreCardStatements" +
                         Convert.ToString(runNo) + ".csv";

                        scr.StoreCardStatementEodToCSV(rundate, conn, trans, strfilepath);


                        //DTransfer Transfer = new DTransfer();
                        //Transfer.ExportToCSV(ds.Tables[0], strfilepath);
                        trans.Commit();
                    }
                }
            }
            catch (Exception)
            {
                eodResult = EODResult.Fail;

                throw;
            }

            return eodResult;
        }

        //IP -  04/10/12 - #11401 - LW75479 - replaced with below. 
        //public string StoreCardInterest(DateTime rundate)
        //{
        //    using (var c = new SqlConnection(Connections.Default))
        //    {
        //        c.Open();
        //        using (var tx = c.BeginTransaction())
        //        {
        //            new StoreCardInterestRatesUpdate(c, tx)
        //            {
        //                rundate = rundate
        //            }.ExecuteNonQuery(rundate);

        //            var scr = new StoreCardRepository();
        //            scr.LoadAnnualFees(c, tx, rundate);
        //            scr.InterestAccountsLoad(c, tx, rundate);
        //            tx.Commit();
        //        }
        //    }
        //    return EODResult.Pass;
        //}

        //IP - 04/10/12 - #11401 - LW75479 - replaced the above with below. 
        //Now setting commandTimeout based on EOD app.config command timeout.
        public string StoreCardInterest(DateTime rundate)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                //ctx.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["commandTimeout"]);
                var timeout = ctx.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["commandTimeout"]);     //IP - 16/01/13 - Merged from CoSACS 6.5

                new StoreCardInterestRatesUpdate(connection, transaction)
                {
                    CommandTimeout = 0,             //IP - 16/01/13 - Merged from CoSACS 6.5
                    rundate = rundate
                }.ExecuteNonQuery(rundate);

                var scr = new StoreCardRepository();
                scr.LoadAnnualFees(connection, transaction, rundate, timeout);  //IP - 16/01/13 - Merged from CoSACS 6.5
                scr.InterestAccountsLoad(connection, transaction, rundate, timeout);    //IP - 16/01/13 - Merged from CoSACS 6.5

                ctx.SubmitChanges();
                return EODResult.Pass;
            });
        }

        //IP - 05/01/11 - Store Card 
        public string StoreCardExport()
        {
            SqlConnection conn = null;
            string eodResult = EODResult.Pass;
            try
            {
                //Select the Store Cards to export
                DataTable dt = new StoreCardRepository().StoreCardExport();


                //Export the Store Cards to a csv file
                string strfilepath =
                (string)Country[CountryParameterNames.SystemDrive] + "\\StoreCardExport" +
                Convert.ToString(runNo) + ".csv";

                DTransfer Transfer = new DTransfer();
                Transfer.ExportToCSV(dt, strfilepath);

                //Only if there were Store Cards to export then update the CardIssued and Runno for these Store Cards
                if (dt.Rows.Count > 0)
                {
                    conn = new SqlConnection(Connections.Default);
                    conn.Open();

                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        var updateStoreCards = new StoreCardExportUpdateCardIssued(conn, trans);

                        updateStoreCards.ExecuteNonQuery(runNo);

                        trans.Commit();
                    }
                }


            }
            catch (Exception)
            {
                eodResult = EODResult.Fail;

                throw;
            }
            finally
            {
                if (conn != null)
                    if (conn.State != ConnectionState.Closed)
                        conn.Close();
            }

            return eodResult;
        }


        // Standing Order
        public string StandingOrder()
        {
            string eodResult = EODResult.Pass;
            string progress = "Standing Order processing ...";
            Console.WriteLine(progress);

            // Load Country Parameters
            BCountry c = new BCountry();
            c.GetMaintenanceParameters(null, null, _countrycode);

            SqlConnection conn = null;


            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                       {
                            // Load and validate
                            BInterfaceControl so1 = new BInterfaceControl();
                            eodResult = so1.StandingOrderP1(runNo);
                            // Update Fintrans
                            BStorderProcess so2 = new BStorderProcess();
                            so2.Cache = c.Cache;
                            so2.User = User;
                            so2.GetByRunNo(conn, trans, runNo);

                           trans.Commit();
                       }
                        // Rename Files
                        BStorderProcess rn = new BStorderProcess();
                        rn.RenameFiles(runNo);                      //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements - Added runNo
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock && retries < maxRetries)
                        {
                            retries++;
                            if (conn.State != ConnectionState.Closed)
                                conn.Close();
                        }
                        else
                        {
                            StringBuilder errorMessages = new StringBuilder();
                            for (int i = 0; i < ex.Errors.Count; i++)
                            {
                                errorMessages.Append("Message: " + ex.Errors[i].Message + "\n" +
                                    "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                    "Source: " + ex.Errors[i].Source + "\n" +
                                    "Procedure: " + ex.Errors[i].Procedure + "\n");
                            }

                            throw new STLException(errorMessages.ToString());
                        }
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            progress = "Standing Order processing finished.";
            Console.WriteLine(progress);
            return eodResult;
        }
        /// <summary>
        /// Copies the product files from the fact 2000 drive to the local drive -needs to be on the local drive for sql server copy in function to work
        /// </summary>
        private void CopyProductFile(string filename)
        {
            try
            {
                string Fact2000filename = (string)Country[CountryParameterNames.fact2000driveandddirectory] + "/" + filename;
                string LocalFilename = (string)Country[CountryParameterNames.SystemDrive] + "\\" + filename;
                //                string LocalFilename = "d:\\users\\default\\" + filename;  

                if (File.Exists(Fact2000filename))
                {
                    if (File.Exists(LocalFilename))
                        File.Delete(LocalFilename);
                    File.Copy(Fact2000filename, LocalFilename);
                }
            }
            catch (Exception ex)
            {
                throw new STLException(ex.Message.ToString());
            }

        }
        private string ProductFileImport(bool donefactexport)
        {
            string eodResult = EODResult.Pass;
            string progress = "Product File Import ...";
            Console.WriteLine(progress);

            string drive = (string)Country[CountryParameterNames.SystemDrive];


            SqlConnection conn = new SqlConnection(Connections.Default);

            try
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    if (File.Exists(CountryParameterNames.fact2000driveandddirectory + "/factauto.err"))
                    {
                        string ErrorText = "FACT 2000 Error file still present - Check whether FACT 2000 completed interface successfully";
                        BInterfaceError ie = new BInterfaceError(
                            conn,
                            trans,
                            "COS FACT",
                            runNo,
                            DateTime.Now,
                            ErrorText,
                            "E");

                        /*throw new STLException("FACT 2000 Error Found - Aborting Product file import");*/

                    }
                }
                //FileInfo okInfo = new FileInfo(CountryParameterNames.fact2000driveandddirectory + "/factauto.ok");
                //if (File.Exists((string)Country[CountryParameterNames.fact2000driveandddirectory] + "/FACTAUTO.ODF"))
                //{
                //    //int i = 1;
                //}

                DateTime StartTime = DateTime.Now;

                if (File.Exists((string)Country[CountryParameterNames.FACT2000ProgramDirectory] + "\\FACTAUTO.BAT"))
                {
                    if (donefactexport)
                    {
                        progress = "File factauto.bat exists waiting for FACTAUTO.ODF to be created";
                        Console.WriteLine(progress);
                        while (!File.Exists((string)Country[CountryParameterNames.fact2000driveandddirectory] + "/FACTAUTO.ODF")) //wait for FACT 2000 to finish
                        {
                            if (File.Exists((string)Country[CountryParameterNames.fact2000driveandddirectory] + "/FACTAUTO.ODF"))
                                break;
                            //waiting 80 minutes and then will exit anyway
                            Thread.Sleep(2000); //sleep for 2 seconds
                            DateTime stopTime = DateTime.Now;
                            TimeSpan duration = stopTime - StartTime;

                            if (duration.Minutes >= 80)
                            {
                                FileInfo check = new FileInfo(CountryParameterNames.fact2000driveandddirectory + "/factauto.err");
                                if (check.Exists)
                                {
                                    throw new STLException("Stock System Error Found - Aborting Product file import after 80 minutes when factauto.err still exists");
                                }
                                else
                                {
                                    throw new STLException("Stock system Timeout - Aborting Product file import as no response after 80 minutes - copy files please run manually");
                                }
                            }
                        }
                    }

                    progress = "copying product files";
                    Console.WriteLine(progress);
                    CopyProductFile("bmsfcprd.dat");

                    //IP - 17/10/11 - #8405 - LW74090
                    if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) == "FACT")
                    {
                        CopyProductFile("bmsfckit.dat");
                    }

                    //IP - 17/10/11 - #8405 - LW74090
                    if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) == "FACT")
                    {
                        CopyProductFile("bmsfcprm.dat");
                    }

                    CopyProductFile("bmsfpstk.dat");

                    //IP - 17/10/11 - #8405 - LW74090
                    if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) == "FACT")
                    {
                        CopyProductFile("bmsfaprd.dat");
                    }


                    //IP - 17/10/11 - #8405 - LW74090
                    if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) == "FACT")
                    {
                        CopyProductFile("BMSFPORD.dat");
                    }
                }



                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {

                            BInterfaceControl ic = new BInterfaceControl();
                            DStockItem stock = new DStockItem();
                            progress = "starting product file import";
                            Console.WriteLine(progress);
                            //Process Products
                            FileInfo productFile = new FileInfo(drive + IF.Product);
                            if (productFile.Exists)
                                ic.ProductImport(conn, trans);
                            else
                                ReportDataFileMissing(conn, trans, drive + IF.Product);

                            //Process Non Stock Products
                            progress = "starting non stock product file import";
                            Console.WriteLine(progress);
                            FileInfo nonStockProductFile = new FileInfo(drive + "\\" + IF.NonStockProduct);
                            if (nonStockProductFile.Exists)
                                ic.NonStockProductImport(conn, trans);
                            else
                                ReportDataFileMissing(conn, trans, drive + "\\" + IF.NonStockProduct);


                            //IP - 17/10/11 - #8405 - LW74090
                            if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) == "FACT")
                            {
                                //Process Kit Products
                                progress = "starting kit file import";
                                Console.WriteLine(progress);
                                FileInfo kitProductFile = new FileInfo(drive + IF.KitProduct);
                                if (kitProductFile.Exists)
                                    ic.KitProductImport(conn, trans, interfaceName, runNo);
                                else
                                    ReportDataFileMissing(conn, trans, drive + IF.KitProduct);

                                //Process Promotional Price Details
                                progress = "starting promo price file import";
                                Console.WriteLine(progress);
                                FileInfo promoPriceFile = new FileInfo(drive + IF.PromotionalPrice);
                                if (promoPriceFile.Exists)
                                    ic.PromoPriceImport(conn, trans, interfaceName, runNo);
                                else
                                    ReportDataFileMissing(conn, trans, drive + IF.PromotionalPrice);

                                //Process Non Stock Promotional Price Details
                                progress = "starting non stock promo price file import";
                                Console.WriteLine(progress);
                                FileInfo nonStockPromoPriceFile = new FileInfo(drive + "\\" + IF.NonStockPromotionalPrice);
                                if (nonStockPromoPriceFile.Exists)
                                    ic.NonStockPromoPriceImport(conn, trans, interfaceName, runNo);
                                else
                                    ReportDataFileMissing(conn, trans, drive + "\\" + IF.NonStockPromotionalPrice);


                            }

                            //Process Stock Qty
                            progress = "starting stock quantity file import";
                            Console.WriteLine(progress);
                            FileInfo stockQtyFile = new FileInfo(drive + IF.StockQty);
                            if (stockQtyFile.Exists)
                                ic.StockQtyImport(conn, trans, interfaceName, runNo);
                            else
                                ReportDataFileMissing(conn, trans, drive + IF.StockQty);

                            //IP - 17/10/11 - #8405 - LW74090
                            if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) == "FACT")
                            {
                                //Process Associated Products
                                FileInfo assocProductFile = new FileInfo(drive + IF.AssociatedProducts);
                                progress = "starting associated file import";
                                Console.WriteLine(progress);
                                if (assocProductFile.Exists)
                                    ic.AssociatedProductImport(conn, trans, Source.Merchandising);
                                else
                                    ReportDataFileMissing(conn, trans, drive + IF.AssociatedProducts);

                                //Process Non Stock Associated Products
                                FileInfo assocNonStockProductFile = new FileInfo(drive + "\\" + IF.NonStockAssociatedProducts);
                                progress = "starting non stock associated file import";
                                Console.WriteLine(progress);
                                if (assocNonStockProductFile.Exists)
                                    ic.AssociatedProductImport(conn, trans, Source.NonStocks);
                                else
                                    ReportDataFileMissing(conn, trans, drive + "\\" + IF.NonStockAssociatedProducts);

                                //Process Purchase Orders    
                                progress = "starting Purchase Order import";
                                Console.WriteLine(progress);
                                FileInfo PurchaseOrderFile = new FileInfo(drive + IF.PurchaseOrder);
                                if (PurchaseOrderFile.Exists)
                                    this.PurchaseOrderImport(conn, trans);
                                else
                                    ReportDataFileMissing(conn, trans, drive + IF.PurchaseOrder);
                            }

                            if (CountryCode == "H")
                            {
                                progress = "translating thai procucts ";
                                Console.WriteLine(progress);
                                //Translate the stockitem descriptions for Thailand
                                stock.TranslateStockItems(conn, trans);
                            }

                            //IP - 17/10/11 - #8405 - LW74090
                            if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) == "FACT")
                            {
                                //Make sure carriage returns are not in the 
                                //WarrantyBand.RefCode column
                                progress = "removing refcodes carriage returns -nearly finished";
                                Console.WriteLine(progress);
                                stock.RemoveRefCodeCR(conn, trans);
                            }

                            NonStockMarkDeleted nsm = new NonStockMarkDeleted(conn, trans);
                            nsm.ExecuteNonQuery();

                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock && retries < maxRetries)
                        {
                            retries++;
                        }
                        else
                        {
                            StringBuilder errorMessages = new StringBuilder();
                            for (int i = 0; i < ex.Errors.Count; i++)
                            {
                                errorMessages.Append("Message: " + ex.Errors[i].Message + "\n" +
                                    "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                    "Source: " + ex.Errors[i].Source + "\n" +
                                    "Procedure: " + ex.Errors[i].Procedure + "\n");
                            }

                            throw new STLException(errorMessages.ToString());
                        }
                    }
                    finally
                    {
                        if (conn.State != ConnectionState.Closed)
                        {
                            conn.Close();
                        }
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw;
            }


            progress = "Product File Import finished.";
            Console.WriteLine(progress);

            return eodResult;
        }

        private class PurchaseOrder
        {
            public string SupplierCode
            {
                get;
                set;
            }
            public string WarehouseNo
            {
                get;
                set;
            }
            public string StringDateDue
            {
                get;
                set;
            }
            public int Quantity
            {
                get;
                set;
            }
            //public DateTime DateDue { get; set; }
            public string OrderNumber
            {
                get;
                set;
            }
            public string itemNo
            {
                get;
                set;
            }
        }

        private void PurchaseOrderImport(SqlConnection conn, SqlTransaction trans)
        {
            try
            { //eeff
                CSVReader csv = new CSVReader((string)Country[CountryParameterNames.SystemDrive] + "\\BMSFPORD.dat");
                string[] fields;

                var PurchaseOrderDelete = new PurchaseOrderDeleteAll(conn, trans);
                PurchaseOrderDelete.ExecuteNonQuery();

                //Removing all existing Purchase Order information as 
                //completely refreshed each time
                var Order = new PurchaseOrder();

                //Read csv file line by line and save details to
                //the warrantyband table

                /* Apart from the refcode the csv file repeats items horizontally example below
                 * 15,LH053   ,ACW0      ,P/O:032917,20112010,000000100,ACW0      ,P/O:033078,03122010,000000200,ACW0      ,P/O:034434,28022011,000000200
                 * whouse,supplier,purchase order,datedue,quantity *100, supplier, purchase order,datedue,quantity etc...
                 
                */
                while ((fields = csv.GetCSVLine()) != null)
                {
                    short supplierplace = 2;
                    short poplace = 3;
                    short dateplace = 4;
                    short quantityplace = 5;
                    Order.itemNo = fields[1].Trim();
                    Order.WarehouseNo = fields[2].Trim();
                    while (supplierplace < fields.Length)
                    {
                        Order.WarehouseNo = fields[0];
                        Order.SupplierCode = fields[supplierplace].Trim();
                        Order.OrderNumber = fields[poplace].Trim();
                        Order.StringDateDue = fields[dateplace].Trim();
                        Order.Quantity = Convert.ToInt32((fields[quantityplace]).Trim()) / 100;

                        var PSave = new PurchaseOrderSave(conn, trans);
                        PSave.ExecuteNonQuery(Order.WarehouseNo, Order.SupplierCode, Order.OrderNumber,
                                              Order.StringDateDue, Order.Quantity, Order.itemNo);
                        supplierplace += 4;
                        poplace += 4;
                        dateplace += 4;
                        quantityplace += 4;

                    }
                }

                csv.Dispose();
            }
            catch (Exception)
            {

                // Pass this on for logging in EODConfiguration
                throw;
            }
        }

        private void ReportDataFileMissing(SqlConnection conn, SqlTransaction trans,
                                            string fileName)
        {
            string msg = "Unable to open file " + fileName;

            BInterfaceError ie = new BInterfaceError(
                                        conn,
                                        trans,
                                        interfaceName,
                                        runNo,
                                        DateTime.Now,
                                        msg,
                                        "W");
        }

        // Calculate Sales Commissions
        private string SalesCommissions()
        {
            string eodResult = EODResult.Pass;
            string progress = "Calculate Sales Commissions processing ...";
            Console.WriteLine(progress);

            // Load Country Parameters
            //BCountry c = new BCountry();
            //c.GetMaintenanceParameters(null, null, _countrycode);

            SqlConnection conn = null;


            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {

                            // Commissions
                            BSalesCommission comm = new BSalesCommission();
                            comm.User = User;
                            comm.EODCommCalc(conn, trans, runNo);

                            //Commissions CSV Extract

                            DataSet ds = comm.EODCommissionExtract(conn, trans, runNo);

                            trans.Commit();


                            //TODO store this in the config file
                            string fileName = "SalesCommissions_Run" + runNo.ToString() + ".csv";
                            string outputFile = Path.Combine("d:\\users\\default\\", fileName); //This should go in the app.config file
                            Console.WriteLine(String.Format("Outputing to CSV file {0}", outputFile));
                            CSVWriter csvWrite = new CSVWriter(outputFile, ds.Tables[TN.SalesCommission]);

                            csvWrite.WriteFile();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock && retries < maxRetries)
                        {
                            retries++;
                            if (conn.State != ConnectionState.Closed)
                                conn.Close();
                        }
                        else
                            throw ex;
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw ex;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            progress = "Calculate Sales Commissions finished.";
            Console.WriteLine(progress);
            return eodResult;
        }


        private string SR_BalanceAccounts()
        {
            string eodResult = EODResult.Pass;
            string progress = "Balancing Service Request Special Accounts ...";
            Console.WriteLine(progress);

            SqlConnection conn = null;


            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            BServiceRequest srAccounts = new BServiceRequest();
                            srAccounts.User = User;
                            srAccounts.BalanceAccounts(conn, trans);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock && retries < maxRetries)
                        {
                            retries++;
                            if (conn.State != ConnectionState.Closed)
                                conn.Close();
                        }
                        else
                            throw ex;
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw ex;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            progress = "Balancing Service Request Special Accounts finished.";
            Console.WriteLine(progress);
            return eodResult;
        }

        //IP - CR946 - BrokerFinancialExport
        private string BrokerFinancialExport()
        {
            string eodResult = EODResult.Pass;
            string progress = "Broker Financial Export processing...";
            Console.WriteLine(progress);

            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(Connections.Default);

                do
                {
                    try
                    {

                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {

                            //Extract the Broker Financial data from the 'Interface_financial' table.
                            BBrokerFinancialExport bf = new BBrokerFinancialExport();
                            DataSet brokerData = bf.GetBrokerExtractData(conn, trans);

                            trans.Commit();


                            //Export the financial data to the Broker (create csv and send string)
                            string fileName = "BrokerXRun" + runNo.ToString() + ".csv";
                            string outputFile = Path.Combine("d:\\cosdata\\", fileName);
                            DataTable brokerDT = brokerData.Tables[0];
                            bf.ExportBrokerData(runNo, brokerDT);
                        }

                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock && retries < maxRetries)
                        {
                            retries++;
                            if (conn.State != ConnectionState.Closed)
                                conn.Close();
                        }
                        else
                            throw ex;
                    }

                } while (retries <= maxRetries);

            }
            catch (Exception ex)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw ex;
            }

            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            progress = "Broker Financial Export Finished.";
            Console.WriteLine(progress);
            return eodResult;
        }

        // #13890 jec  Online Product Export
        public string OnlineProductExport()
        {
            SqlConnection conn = null;
            string eodResult = EODResult.Pass;
            try
            {
                //Select the Online Products to export
                DataTable dt = new EODRepository().OnlineProductExport();
                //Select Ineligible Online Products
                DataTable ie = new EODRepository().IneligibleOnlineProductExport();

                var runNox = runNo < 9 ? "00" + Convert.ToString(runNo) : runNo < 99 ? "0" + Convert.ToString(runNo) : Convert.ToString(runNo);
                //Export the Online Products to a csv file
                string strfilepath =
                (string)Country[CountryParameterNames.SystemDrive] + "\\" + (string)Country[CountryParameterNames.ISOCountryCode] + "ecom" + runNox + ".csv";

                DTransfer Transfer = new DTransfer();
                Transfer.ExportToCSV(dt, strfilepath);

                //Export Ineligible Online Products to a csv file
                if (ie.Rows.Count > 0)
                {
                    string strfilepath2 = (string)Country[CountryParameterNames.SystemDrive] + "\\" + "IneligibleOnlineProducts" + runNox + ".csv";
                    DTransfer Transfer2 = new DTransfer();
                    Transfer.ExportToCSV(ie, strfilepath2);
                }

            }
            catch (Exception)
            {
                eodResult = EODResult.Fail;

                throw;
            }
            finally
            {
                if (conn != null)
                    if (conn.State != ConnectionState.Closed)
                        conn.Close();
            }

            return eodResult;
        }

        //#13719
        public string ReadyAssistExport()
        {
            SqlConnection conn = null;
            var eodResult = EODResult.Pass;
            var date = DateTime.Now.ToString("yyyyMMdd");

            try
            {
                //Select Ready Assist Contract Sales to export
                DataTable dt = new EODRepository().ReadyAssistExport();

                string strfilepath =
                (string)Country[CountryParameterNames.SystemDrive] + "\\" + (string)Country[CountryParameterNames.ISOCountryCode] + "_Courts_" + date + ".txt";

                DTransfer Transfer = new DTransfer();
                Transfer.ExportToCSV(dt, strfilepath, '|');

            }
            catch (Exception)
            {
                eodResult = EODResult.Fail;

                throw;
            }
            finally
            {
                if (conn != null)
                    if (conn.State != ConnectionState.Closed)
                        conn.Close();
            }

            return eodResult;
        }

        ////IP - Store Card - Feature #3004- 06/05/11 - Method to create a store card account for a customer pre-approved for Store Card
        //private DataTable CreateStoreCardAcct(SqlConnection conn, SqlTransaction trans, DataTable dt )
        //{
        //    BAccount ba = new BAccount();
        //    ba.Source = "EOD";

        //    bool rescore = false;

        //    try
        //    {

        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            //Firstly generate a store card number. done in other method
        //      //      var cardNumber = StoreCardGen.GenerateAndSaveCountryStoreCardNumber();

        //            //dr[CN.CardNumber] = cardNumber;
        //            var Acct = new BAccount();
        //            //Create a Store Card Account and link to the Customer
        //            var acctNo = ba.CreateCustomerAccount(conn, trans, CountryCode, Convert.ToInt16(dr[CN.BranchNo]), Convert.ToString(dr[CN.CustID]), "T", this.user, false, out rescore, "PreApprove");

        //            //Update the datatable with the account just created
        //            //dr[CN.StoreCardAcctNo] = acctNo.Replace("-", "").Trim();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //    return dt;
        //}

    }
}
