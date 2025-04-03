using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using STL.BLL;
using STL.Common.Constants.ColumnNames;
using System.Data.SqlClient;
using System.Data;
using STL.Common.Constants.TableNames;
using System.Configuration;
using Blue.Cosacs.Shared;
using STL.DAL;
using System.IO;
using STL.Common;
using Blue.Cosacs.Repositories;

using STL.Batch;
//using STL
using Blue.Cosacs.StoreCardUtil;

 

namespace Blue.Cosacs.TestforFullBuild
{
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
    [TestFixture]
    public class EndofDayTest
    {
        private string CountryCode = "G";
        private short Branchno = 780;
            
    [TestFixtureSetUp]
    public void Setup()
    {
        DCountry da = new DCountry();
        DataTable dt = da.GetMaintenanceParameters(null, null, CountryCode);
        CountryParameterCache.Cache = new CacheProvider();
        CommonObject co = new CommonObject();
        if (dt.Rows.Count > 0)
        {
            co.Cache["Country"] = new CountryParameterCollection(dt);
            StockItemCache.Invalidate(new StockRepository().GetStockItemCache());
        }
    }

        [TestFixtureTearDown]
        public void TearDown()
        {
        }

        [SetUp]
        public void SetupTest()
        {
            
        }   
        
        [TearDown]
        public void TearTest()
        {
        }
        
        [Test] 
        public void ScorexExtractTest()
        {  
            // EODInterface intf = new EODInterface();
            
            Scorex sc = new Scorex();
            sc.RunScorexExtract();
        }

        [Test]
        public void StoreCardInterestRateFeeTest()
        {

            var user = 99999;
            var eodRun = new BInterfaceControl();

            bool reRun = false;
            string fileDate = "";

            int runNo = eodRun.StartNextRun("STINTEREST", "STINTEREST", out reRun, out fileDate);


            var Eod = new EODInterface(CountryCode);
            Eod.runNo = runNo;
            var eodResult = Eod.StoreCardInterest(Convert.ToDateTime("12-mar-2012"));
          //  var eodResult = Eod.StoreCardInterest(Convert.ToDateTime(DateTime.Now));

            Assert.IsTrue(eodResult == "P", "Store Card Interest Failed");

           // EODInterface intf = new EODInterface();
            //var scr = new StoreCardRepository();
            //DateTime datefrom = Convert.ToDateTime("1-aug-2011");
            //DateTime dateto = Convert.ToDateTime("1-sep-2011");

            //scr.InterestAccountsLoad(null, null, dateto);

            //double interest = scr.CalculateInterestForAccount(null, null, "701900049341",
            //    datefrom, dateto, 18, Convert.ToDateTime("11-sep-2011"));
            //Assert.IsTrue(Math.Round(interest, 2) == 4.64, "Interest should be 4.64 for account 701900049341 " + interest.ToString());
            //Assert.IsTrue(scr.Averagebalance < 400, "Average Balance incorrect" + scr.Averagebalance.ToString());
            //Acct Account = new Acct();
            //Account.arrears = 200;
            //Customer cust = new Customer();
            //cust.creditblocked = 0;
            //scr.DetermineBlock();
            


        }

        [Test]
        public void CollectionsEodTest()
        {
            DCollectionsModule CM = new DCollectionsModule();
            var eodResult = CM.DoEndofDayStrategies();
            //IP - 16/07/08 - UAT 5.2 - UAT(24) - Letters for collections were not being generated.

            Assert.IsTrue(eodResult == "P", "Collections Module failed");
                
        }
        [Test]
        public void StoreCardExportTest()
        {

            var user = 99999;
            var eodRun = new BInterfaceControl();

            bool reRun = false;
            string fileDate = "";

            int runNo = eodRun.StartNextRun("STCARDEXPORT", "STCARDEXPORT", out reRun, out fileDate);


            var Eod = new EODInterface(CountryCode);
            Eod.runNo = runNo;
            var eodResult= Eod.StoreCardExport();

           Assert.IsTrue(eodResult == "P", "Store Card Export Failed");

        }

        [Test]
        public void ArrearsTest()
        {

            var eodRun = new BInterfaceControl();

            bool reRun = false;
            string fileDate = "";

            int runNo = eodRun.StartNextRun("Arrears", "Arrears", out reRun, out fileDate);
            var BA  =new BAccount();
            BA.EodArrearsCalculation(false);
            
            
        }


        [Test]
        public void STStatementsTest()
        {
             
            var eodRun = new BInterfaceControl();

            bool reRun = false;
            string fileDate = "";

            int runNo = eodRun.StartNextRun("STStatements", "STStatements", out reRun, out fileDate);

            DateTime rundate = 
            //    Convert.ToDateTime(DateTime.Now.AddHours(6).ToShortDateString()); 
            Convert.ToDateTime("4-jan-2012");
            var Eod = new EODInterface(CountryCode);
            Eod.runNo = runNo;

            var eodResult = Eod.StoreCardStatements(rundate );

            Assert.IsTrue(eodResult == "P", "Store Card Statements Failed");

        }


        [Test]
        public void StoreCardQualifyTest()
        {
            var user = 99999;
            var eodRun = new BInterfaceControl();

            bool reRun = false;
            string fileDate = "";
            
            int runNo = eodRun.StartNextRun("STCARDQUAL", "STCARDQUAL", out reRun, out fileDate);

            var Eod = new EODInterface(user, CountryCode, runNo, "STCARDQUAL");
            var eodResult = Eod.StoreCardQualify(DateTime.Now);
            var GetQualified = new Blue.Cosacs.StoreCardGetRecentlyQualified(null, null);
            var ds = GetQualified.ExecuteDataSet();
            var dt = ds.Tables[0];
            if (dt.Rows.Count >0)
            {
              var Acctno =  dt.Rows[0]["AcctNo"].ToString(); 
            
            }
            Assert.IsTrue(eodResult == "P", "Store Card Export Failed");
            
        }
        [Test]
        public void BehaviouralRescore()
        {
            var user = 99999;//eeff
            var eodRun = new BInterfaceControl();

            bool reRun = false;
            string fileDate = ""; 

            int runNo = eodRun.StartNextRun("BHSRescore", "BHSRESCORE", out reRun, out fileDate);

            EODInterface eodOption = new EODInterface(user, CountryCode, runNo, "BHSRESCORE");

            RFEOD rf = new RFEOD(user, CountryCode, runNo, "BHSRESCORE");
            var eodResult = rf.Rescore('B');
         
            Assert.IsTrue(eodResult == "P", "Behavioural Rescore failed");

        }
        [Test] // Fix this.... 
        public void FACTExport()
        {
            DateTime startTime = DateTime.Now;
            EODInterface intf = new EODInterface(CountryCode);

            intf.interfaceName = "COS FACT";
            
            intf.User = 99999;
            bool dfe = false;
            intf.RunOption("TEST",ref dfe,false,"");       //jec 08/04/11 RI
            System.IO.FileInfo file = new FileInfo("D:\\users\\default\\bmsfcint.dat");

            Assert.IsTrue(file.LastWriteTime > startTime, "No Delivery Fact 2000 file Created - bmsfcint.dat");

            System.IO.FileInfo filef = new FileInfo("D:\\users\\default\\bmsffint.dat");

            Assert.IsTrue(filef.LastWriteTime > startTime, "No Fact 2000 Financial file Created - bmsffint.dat");
            
        }
        [Test]
        public void BehaviouralScoring()
        {
            DateTime startTime = DateTime.Now;
            EODInterface intf = new EODInterface(CountryCode);

            intf.interfaceName = "BHSRESCORE";

            intf.User = 99999;
            bool dfe = false;
            intf.RunOption("TEST", ref dfe, false,"");       //jec 08/04/11 RI
            

        }
        [Test]
        public void FACTIMPORT()
        {
            DateTime startTime = DateTime.Now;
            EODInterface intf = new EODInterface(CountryCode);

            intf.interfaceName = "FACTCOS";

            intf.User = 99999;
            bool dfe = false;
            intf.RunOption("TEST", ref dfe, false,"");       //jec 08/04/11 RI
           

        }

        public void MapAgreementtoDAgreement(ref DAgreement daag, Agreement ag)
        {
            daag.AccountNumber = ag.acctno;
            daag.AgreementNumber = ag.agrmtno;
            daag.AgreementDate = Convert.ToDateTime(ag.dateagrmt);
            daag.AgreementTotal = ag.agrmttotal;
            daag.CashPrice = ag.cashprice;
            daag.CODFlag = ag.codflag;
            daag.CreatedBy = ag.createdby;
            daag.DateAuth = ag.dateauth;
            daag.DateChange = Convert.ToDateTime(ag.datechange);
            daag.DateDel = Convert.ToDateTime(ag.datedel);
            daag.DateNextDue = Convert.ToDateTime(ag.datenextdue);
            daag.Discount = Convert.ToDecimal(ag.discount);
            daag.Deposit = ag.deposit;
            daag.EmployeeNumAuth = ag.empeenoauth;
            daag.EmployeeNumChange = ag.empeenochange;
            daag.FullDelFlag = ag.fulldelflag;
            daag.HoldMerch = ag.holdmerch;
            daag.HoldProp = ag.holdprop;
            daag.PaymentHolidays = ag.paymentholidays;
            daag.PaymentMethod = ag.PaymentMethod;
            daag.PayMethod = ag.paymethod;
            daag.PxAllowed = Convert.ToDecimal(ag.pxallowed);
            daag.SundryChargeTotal = Convert.ToDecimal(ag.sdrychgtot);
            daag.ServiceCharge = ag.servicechg;
            daag.SOA = ag.soa;
            daag.SundryChargeTotal = Convert.ToDecimal(ag.sdrychgtot);
            daag.UnpaidFlag = ag.unpaidflag;

        }
        public void MapInstalplantoDinstalplan(ref DInstalPlan dip, Instalplan ip)
        { 
            dip.AccountNumber
                = ip.acctno;
            dip.AgreementNumber= ip.agrmtno;
            dip.DateFirst= ip.datefirst;
            dip.NumberOfInstalments= ip.instalno;
            dip.InstalmentFrequency= ip.instalfreq;
            dip.DateLast= Convert.ToDateTime(ip.datelast);
            dip.InstalmentAmount = ip.instalamount;
            dip.FinalInstalment= ip.fininstalamt;
            dip.InstalTotal= ip.instaltot;
            dip.MonthsInterestFree = Convert.ToInt16(ip.mthsintfree);
            dip.DueDay = ip.dueday;
            dip.Band = ip.scoringband;
            dip.autoda = ip.AutoDA;
            dip.InstalmentWaived = ip.InstalmentWaived;
        }
        public void MapAccttoDacct(ref DAccount dacct, Acct Account)
        {
            dacct.Arrears = Convert.ToDecimal(Account.arrears);
            dacct.AccountNumber = Account.acctno;
            dacct.AccountType = Account.accttype;
            dacct.AgreementTotal = (decimal)Account.agrmttotal;
            //dacct.AS400Bal = Account.as400bal;
            dacct.BDWBalance = Account.bdwbalance;
            dacct.BDWCharges = Account.bdwcharges;
            dacct.BranchNo = Account.branchno;
            dacct.CurrentStatus = Account.currstatus;
            dacct.DateAccountOpen = Convert.ToDateTime(Account.dateacctopen);
            if (Account.dateintoarrears != null)
                dacct.DateIntoArrears = Convert.ToDateTime(Account.dateintoarrears);
          //  else
          //      dacct.DateIntoArrears = DateTime.MinValue;
            if (Account.datelastpaid != null)
                dacct.DateLastPaid = Convert.ToDateTime(Account.datelastpaid);
          //  else
            //    dacct.DateLastPaid = DateTime.MinValue;
            dacct.HighestStatus = Account.highststatus;
            dacct.HighestStatusDays =Convert.ToInt16( Account.histatusdays);
            dacct.OutstandingBalance = Convert.ToDecimal(Account.outstbal);
            dacct.PaidPcent = Account.paidpcent;
            dacct.TermsType = Account.termstype;
            


        }
        /// <summary>
        /// Tests that if you make a payment it works for instant credit... 
        /// you need a customer that qualifies and then a new account with a deposit, then take a cheque payment on that account... could just save fintrans.... 
        /// </summary>

        public string GenerateNewAccount(Acct Account, SqlConnection conn, SqlTransaction trans 
            )
        {

            string acctno = string.Empty;
            BAccount BAcct = new BAccount();
            BAcct.GenerateAccountNumber(conn, trans, CountryCode, Branchno, Account.accttype, false, out  acctno);
            if (Account.accttype == "H")
                Account.accttype = "O";
            acctno = acctno.Replace("-", "");
            Account.acctno = acctno;
            #region createaccount

            DAccount dacct = new DAccount();
            dacct.AccountNumber = Account.acctno;
            dacct.AccountType = Account.accttype;
            dacct.AgreementTotal = 2000;
            dacct.BranchNo = Branchno;
            dacct.CurrentStatus = "1";
            dacct.DateAccountOpen = DateTime.Now;
            dacct.OutstandingBalance = 0.0m;
            dacct.TermsType = Account.termstype;
            dacct.User = 99999;
            dacct.Save(conn, trans);

            dbcustacctadd custacct = new dbcustacctadd();
            custacct.ExecuteNonQuery(0, Account.CustAcct.custid, acctno, "H");


            DAgreement dag = new DAgreement();
            dag.AccountNumber = Account.acctno;
            dag.HoldProp = "Y";
            dag.AgreementTotal = 2000;
            dag.Deposit = Account.Agreement.deposit; dag.AgreementNumber = 1;
            dag.Discount = 20;
            dag.DeliveryFlag = "N";
            dag.Save(conn, trans);
            if (Account.accttype != "C" && Account.accttype != "S")
            {
                DInstalPlan dip = new DInstalPlan();
                dip.AccountNumber = Account.acctno;
                dip.InstalmentAmount = 100.0m; dip.AgreementNumber = 1;
                dip.NumberOfInstalments = 20; dip.InstalmentWaived = Account.InstalPlan.InstalmentWaived;
                dip.InstalmentFrequency = "M";//monthly
                dip.Save(conn, trans);
                bool cloned = true;
                DProposal dprop = new DProposal();
                dprop.CloneProposal(conn, trans, Account.CustAcct.custid, Account.acctno, out cloned);
                dprop.GetProposal(conn, trans, Account.acctno, Account.CustAcct.custid);
                var random = new Random();
                dprop.MonthlyIncome = random.Next(500, 6000) ; // get a random amount for random credit limit
                dprop.DateChanged = DateTime.Now;
                dprop.DateIn = DateTime.Today.AddYears(-6);
                dprop.Save(conn, trans, Account.CustAcct.custid, Account.acctno);
            }
                //dprop.GetProposal(
            
            #endregion
            return acctno;
        }

        public void Score(SqlConnection conn, SqlTransaction trans, ref string result, Acct Account )
        {

            BProposal prop = new BProposal();
            string referralReasons = string.Empty; //IP - 15/03/11 - #3314 - CR1245
            int user = 99999; string newBand = ""; string refCode = ""; decimal points = 0.0M; decimal RFLimit = 0.0M;
            result = ""; string bureauFailure = ""; bool referDeclined = false;
            prop.Score(conn, trans, CountryCode, Account.acctno, Account.accttype, Account.CustAcct.custid, DateTime.Now, 700,
                out newBand, out refCode, out points, out RFLimit, user, out result, out bureauFailure, ref referDeclined, out referralReasons); //IP - 15/03/11 - #3314 - CR1245 - Return referral reasons
                     
            
        }
        [Test]
        public void UpliftPercentageTest() //eeff
        {
            using (var conn = new SqlConnection(STL.DAL.Connections.Default))
            {
                string referralReasons = string.Empty; //IP - 15/03/11 - #3314 - CR1245
                conn.Open();
                var trans = conn.BeginTransaction();

                GetRandomAccts random = new GetRandomAccts();
                GetRandomAccts.Reader reader = random.Execute(5, false, "R");
                var accts = reader.Select(r => r.acctno).ToArray();
                Acct Account = new Acct();
                Account.accttype = "R";

                DAccount dacct = new DAccount();
                DInstalPlan dip = new DInstalPlan();
                AccountRepository AcctR = new AccountRepository();

                Acct.Parameters.Load Parms = new Acct.Parameters.Load();
                Parms.CustAcctHolderGet = true;
                foreach (var acct in accts)
                {
                    Parms.AcctNo = acct.ToString();
                 //   Parms.AcctNo="700009353561";
                    SetAllParameters(ref Parms);
                    Account = AcctR.Get(Parms, 0, conn, trans);
                    Parms.AcctNo= GenerateNewAccount(Account, conn, trans);
                    Account.acctno = Parms.AcctNo;
                    string result = "";
                    Score(conn, trans, ref result, Account);
                    decimal upLiftPercent = AcctR.UpliftPercentage(Account.acctno, conn, trans);
                    Account = AcctR.Get(Parms, 0, conn, trans);

                    DCustomer cust = new DCustomer();
                    cust.CustID = Account.CustAcct.custid;
                    cust.GetBasicCustomerDetails(conn, trans, cust.CustID,Account.acctno, "H");
                    bool spendLimitReferred = false;
                    if (Account.Proposal.reason == "SL" || Account.Proposal.reason2 == "SL"
                        || Account.Proposal.reason3 == "SL" || Account.Proposal.reason4 == "SL"
                        || Account.Proposal.reason5 == "SL")
                        spendLimitReferred = true;
                    // the rule is for credit limit 10,000 so we need to check this. 
                    if (spendLimitReferred)
                        AssertTrueCommit(Convert.ToDecimal(cust.RFLimit) > 15900.00m + 10000.0m * upLiftPercent / 100.0m, "Referred but uplift percentage says no " + Account.acctno
                            + " Limit:" + Convert.ToDecimal(cust.RFLimit), trans);

                    if (!spendLimitReferred)
                        AssertTrueCommit(Convert.ToDecimal(cust.RFLimit) <= 15900.00m + 10000.0m * upLiftPercent / 100.0m, "Not Referred but score exceeds 10,000 " + Account.acctno, trans);

                    AssertTrueCommit(upLiftPercent == 0.0m || upLiftPercent == 10.0m || upLiftPercent == 20.0m, 
                        " uplift percentage should be 0 , 10 or 20 % " + Account.acctno + " " + Convert.ToString(cust.RFLimit)
                        , trans);


                   


                    
                }
                trans.Commit();
            }
        }
        public void SetAllParameters(ref Acct.Parameters.Load Parms)
        {
            Parms.AcctGet = true; Parms.AgreementGet = true;
            Parms.CustAcctHolderGet = true; Parms.CustomerGet = true;
            Parms.FinTransGet = true; Parms.HomeAddressGet = true;
            Parms.InstalPlanGet = true; Parms.InstantCreditFlagGet = true;
            Parms.ProposalGet = true; Parms.StoreCardGet = true;
            Parms.StoreCardPayDetailsGet = true;
            Parms.ProposalFlagsGet = true;
        }
        public void CheckDepositPayforInstantCredit(string custid, SqlConnection conn,SqlTransaction trans )
        {
            var Account = new Acct(); Account.CustAcct = new CustAcct(); Account.Agreement = new Agreement(); Account.InstalPlan = new Instalplan();
            Account.CustAcct.custid = custid;
            Account.accttype = "R";
            Account.termstype = "01"; //instalpredel = y
            Account.InstalPlan.InstalmentWaived = true; //but waived so not 
            Account.Agreement.deposit = 0;
            string acctno = GenerateNewAccount(Account, conn, trans);
            Account.acctno = acctno;
            DAccount dacct = new DAccount();
            string approved = dacct.InstantCredit(conn, trans, custid, acctno);

            AccountRepository acctR = new AccountRepository();
            Acct.Parameters.Load Parms = new Acct.Parameters.Load();
            Parms.AcctNo = acctno;
            Assert.IsTrue(approved == "Y", "customer should be instant credit approved " + custid);

            Parms.AcctGet = true; Parms.AgreementGet = true; Parms.CustAcctHolderGet = true;
            Parms.FinTransGet = true; Parms.InstalPlanGet = true;

            Account = acctR.Get(Parms, 99999, conn, trans); //IP - 03/03/11 - #3255 - Added empeeno

            Assert.IsTrue(!Account.ICInstalOutstanding , "Account with deposit waived should not have Instal Outstanding " + custid);
            var dip = new DInstalPlan();
            Account.InstalPlan.InstalmentWaived = true;
            MapInstalplantoDinstalplan(ref dip, Account.InstalPlan);
            dip.Save(conn, trans);
            SetAllParameters(ref Parms);
            Account = acctR.Get(Parms, 99999, conn, trans); //IP - 03/03/11 - #3255 - Added empeeno
            Assert.IsTrue(Account.ICInstalOutstanding, "Account without deposit waived should  have Instal Outstanding " + custid);

            var ft = new FinTrans();
            var Dft = new DFinTrans();
            DBranch branch = new DBranch();
            Dft.AccountNumber = Account.acctno;            Dft.Agrmtno = 1;
            Dft.TransValue = -100;
            Dft.TransRefNo = branch.GetTransRefNo(conn, trans, Branchno);
            Dft.PayMethod = 2; Dft.User = 99998; Dft.BranchNumber = Branchno;
            Dft.TransTypeCode = "PAY"; Dft.Source = "COSACS";
            Dft.FTNotes= "UNIT";  Dft.RunNumber = 0;
            Dft.Write(conn, trans);
            
            Parms.AcctGet = true; Parms.AgreementGet = true; Parms.CustAcctHolderGet = true;
            Parms.FinTransGet= true; Parms.InstalPlanGet = true;

            Account = acctR.Get(Parms, 99999, conn, trans); //IP - 03/03/11 - #3255 - Added empeeno

            Assert.IsTrue(Account.ICChequeFlagOutstanding,"Cheque paid cheque so deposit flag should be outstanding" );

            Dft.PayMethod = 1;  Dft.Source = "COSACS";
            Dft.TransRefNo = branch.GetTransRefNo(conn, trans, Branchno);
            Dft.Write(conn, trans);
            Account = acctR.Get(Parms, 99999, conn, trans); //IP - 03/03/11 - #3255 - Added empeeno

            Assert.IsFalse(Account.ICChequeFlagOutstanding,"Cash paid so ic cheque deposit should not be outstanding");


            

            

        }
    /// <summary>
    /// Using this so commit and can check on database... 
    /// </summary>
    /// <param name="Value"></param>
    /// <param name="Text"></param>
    /// <param name="trans"></param>
        public void AssertTrueCommit(bool Value, String Text,SqlTransaction trans ) 
        {
            if (!Value)
                trans.Commit();

            Assert.IsTrue(Value, Text);


        }
        [Test]
        public void CheckInstantCredit()
        {
            var customer = new BCustomer();
            using (var conn = new SqlConnection(STL.DAL.Connections.Default))
            {
                conn.Open();

                GetRandomAccts random = new GetRandomAccts();
                //GetRandomAccts.Reader reader = random.Execute(20,true,"");  //todo do more... 
                GetRandomAccts.Reader reader = random.Execute(5, false, "R");
                //using linq with some syntactic sugar.
                var accts = reader.Select(r => r.acctno).ToArray();
                DAccount dacct = new DAccount();
                Acct Account = new Acct();
                DInstalPlan dip= new DInstalPlan();
                AccountRepository AcctR = new AccountRepository();
                Acct.Parameters.Load Parms = new Acct.Parameters.Load();
               
                int counter = 0;
                    //load up the customer accounts 
                string badAccount = "";
                SqlTransaction trans = conn.BeginTransaction();
         
                foreach (string acctno in accts)
                    {
                        Parms.AcctNo = acctno; Parms.ProposalFlagsGet = true;
                        Parms.CustAcctHolderGet = true; Parms.InstalPlanGet = true; Parms.AgreementGet = true; Parms.InstantCreditFlagGet = true; Parms.ProposalGet = true;
                        Account = AcctR.Get(Parms, 99999, conn, trans); //IP - 03/03/11 - #3255 - Added empeeno
                        Account.Agreement.deposit = 0; Account.accttype = "R"; 
                        Account.InstalPlan.InstalmentWaived = true; Account.termstype = "01";  // first instalment before delivery but should be waived... 
                        Parms.AcctNo = GenerateNewAccount(Account, conn, trans);
                        Account = AcctR.Get(Parms, 99999, conn, trans); //IP - 03/03/11 - #3255 - Added empeeno

                        InstalPlanUpdateInstantCredit iuic = new InstalPlanUpdateInstantCredit(conn, trans);
                        iuic.ExecuteNonQuery(Account.acctno, "Y",true);
                            
                        bool instantDA= AcctR.InstantCreditDACheck(Account.acctno, 99999, conn, trans);
                        if (!Account.ICArrearsOutstanding && Account.MaxCustArrearsLevel < .5 && Account.Proposal.propresult=="A")
                        {
                            if (!instantDA)
                                trans.Commit();

                            Assert.IsTrue(instantDA, "Instant credit account with waived instalment should be DA'd " + Account.acctno);
                        }
                        //Account = AcctR.Get(Parms,99999,conn,trans);
                        
                        if (counter % 3 == 0)
                            Account.accttype = "H";
                        
                        Parms.AcctNo = GenerateNewAccount(Account, conn, trans);
                        if (Account.accttype == "H")
                            Account.accttype = "O";
                        Account = AcctR.Get(Parms, 99999, conn, trans); //IP - 03/03/11 - #3255 - Added empeeno
                        string result = string.Empty;
                        this.Score(conn, trans,ref result, Account);

                        if (Account.accttype == "O" && result == "R")
                        {
                            AcctR.InstantCreditDACheck(Account.acctno, 99999, conn, trans); //IP - 03/03/11 - #3255 - Added empeeno
                            SetAllParameters(ref Parms);
                            
                            Account = AcctR.Get(Parms,99999, conn, trans);
                            if (Account.InstalPlan.InstantCredit != "N")
                            {
                                AssertTrueCommit(Account.ICReferralFlagOutstanding, "Referral Flag should be outstanding " + Account.acctno, trans);
                            }
                           
                        }

                        iuic.ExecuteNonQuery(Account.acctno, "N",false);

                        var letter = new DLetter();
                        letter.AccountNo = Account.acctno; letter.LetterCode = "LOAN"; letter.LetterDate = DateTime.Today;
                        letter.OrigBr = Branchno; letter.User = 99999; letter.LetterDue = DateTime.Today;
                        letter.Write(conn, trans);

                        string approved = dacct.InstantCredit(conn, trans, Account.CustAcct.custid, Parms.AcctNo);

                        //  trans.Commit();
                      //  trans = conn.BeginTransaction();
                        counter++;
                    
                        if (approved == "N") // we are going to store this custid for later use as a joint holder
                        {
                            badAccount = Account.acctno;
                            //test the 
                            var Cust = new DCustomer();
                            Cust.GetBasicCustomerDetails(conn, trans, Account.CustAcct.custid,Account.acctno,"H");

                            var Result=Cust.GetCashLoanQualified(conn,trans,Account.CustAcct.custid, Parms.AcctNo);
                            if ( Cust.LoanQualified)
                                AssertTrueCommit(Result == "Y", "Account should still be loan qualified as has Loan letter sent", trans);
                            //Acct.
                        }
                        if (approved == "Y")                         
                        {
                            if (Account.accttype == "O" && result == "R")
                            {
                                AcctR.InstantCreditDACheck(Account.acctno, 99999, conn, trans); //IP - 03/03/11 - #3255 - Added empeeno
                                Account = AcctR.Get(Parms, 99999, conn, trans);
                                if (!Account.ICReferralFlagOutstanding)
                                    trans.Commit();
                                Assert.IsTrue(Account.ICReferralFlagOutstanding, "Referral Flag should be outstanding for instant credit account" + Account.acctno);

                                Assert.IsFalse(Account.ICReferralFlagOutstanding, "please ignore");
                            }

                            if (counter % 5 == 0)//creating some accounts for manual testing...
                                continue;
                            //todo put back
                            CheckDepositPayforInstantCredit(Account.CustAcct.custid, conn,trans );
                            //reset agreement hold prop then reload details
                            Account.Agreement.holdprop = "Y";
                            DAgreement Dag = new DAgreement();
                            MapAgreementtoDAgreement(ref Dag, Account.Agreement);
                            Dag.Save(conn, trans);
                            BAccount Bacct= new BAccount();
                             
                            Bacct.GetInstantCreditAwaitingClearance(Convert.ToString(Branchno), 0, 1, "", conn, trans);
                            Account = AcctR.Get(Parms, 99999, conn, trans);
                            if (Account.ICArrearsOutstanding == false 
                                && Account.ICDepositFlagOustanding == false && Account.ICInstalOutstanding)
                            {
                                AssertTrueCommit(Account.Agreement.holdprop == "N", "Account should be cleared as Instant Credit but HoldProp still N-" + Account.acctno, trans);
                                //Assert.IsTrue(Account.Agreement.holdprop == "N", "Account should be cleared as Instant Credit but HoldProp still N-" + Account.acctno);
                            }

                            if (Account.ICArrearsOutstanding == true)
                            {
                                Assert.IsTrue(Account.Agreement.holdprop == "Y", "Account should not be cleared as Arrears Oustanding-" + Account.acctno);
                            }
                            // Add bad customer as a joint holder. First check if parameter is false - still should qualify. Then check if parameter true - should not qualify.
                            if (badAccount != "")
                            {
                                dbcustacctadd custacct = new dbcustacctadd();
                                custacct.ExecuteNonQuery(0, Account.CustAcct.custid, badAccount, "J");
                                CountryMaintenanceSetValue CMSV = new CountryMaintenanceSetValue(conn, trans);
                                CMSV.ExecuteNonQuery("IC_JointQualification", "False");
                                iuic.ExecuteNonQuery(Account.acctno, "N",false);
                                approved = dacct.InstantCredit(conn, trans, Account.CustAcct.custid, Account.acctno);
                                Assert.IsTrue(approved == "Y", "Linked customer causing denied instant credit when country parameter set to false"); //balance should always be numeric....
                               
                                CMSV.ExecuteNonQuery("IC_JointQualification", "True");
                                iuic.ExecuteNonQuery(Account.acctno, "N",false);
                                approved = dacct.InstantCredit(conn, trans, Account.CustAcct.custid, acctno);

                                //trans.Commit();
                                //trans = conn.BeginTransaction();
                                //todo put back
                              //  Assert.IsTrue(approved != "Y", "Linked customer allowing instant credit when country parameter set to true " + acctno + " " + Account.Custacct.custid); //balance should always be numeric....
                                badAccount = ""; //reset so don't have multiple linked to one bad account... 
                            }

                            // update the account type to H and recheck 
                            Account.accttype = "O"; //set to hp type
                            MapAccttoDacct(ref dacct, Account);
                            dacct.Save(conn, trans);
                        
                            iuic.ExecuteNonQuery(Account.acctno, "N",false);
                           
                            approved = dacct.InstantCredit(conn, trans, Account.CustAcct.custid, Account.acctno);
                         
                            //trans = conn.BeginTransaction();
                            Assert.IsTrue(approved=="N","HP Accounts granted instant credit should not be - check country parameter"); //balance should always be numeric....
                            if (counter > 20) // want to get a decent number of accounts tested but not too many
                                break;
                        }
                    }
                    trans.Commit();
                  
                    Assert.IsFalse(counter > 218, "Failed to Test instant Credit");
            }
        }

        /// <summary>
        /// This is to check the server version of the database that is being tested. This is so that we can test end of day.... 
        /// This assumes that the application server is the same as the database server. 
        /// </summary>
        public void SetupServer()
        {

        }


        
    }
}
