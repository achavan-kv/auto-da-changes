//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using Blue.Cosacs.Repositories;
//using Blue.Cosacs.Shared;
//using Blue.Cosacs.Shared.Services.StoreCard;
////using STL
//using Blue.Cosacs.StoreCardUtil;
//using Microsoft.Practices.EnterpriseLibrary.Data;
//using NUnit.Framework;
//using STL.BLL;
//using STL.Common;
//using STL.DAL;


//namespace Blue.Cosacs.Test
//{
//    using STL.Common.Constants.FTransaction;

//    public class CacheProvider : ICache
//    {
//        Dictionary<string, object> dictionary = new Dictionary<string,object>();

//        public object Get(string key)
//        {
//            if (dictionary.ContainsKey(key))
//                return dictionary[key];

//            return null;
//        }

//        public object Insert(string key, object item)
//        {
//            dictionary.Add(key, item);
//            return item;
//        }
//    }

     
//    [TestFixture]
//    public class StoreCardTest
//    {
//        public const short branchno = 700;

//        public const string tname = "test";
//        public const int tscorefrom = 0;
//        public const int tscoreto = 100;
//        public const decimal tfixed = 3.4M;
//        public const decimal tvarible = 5.3M;
//        // acct type T is StoreCard

//        Customer customer = new Customer
//            {
//                custid = "M246812IRHFKIDSLPTOF",
//                branchnohdle = 0,
//                name = "NUNIT TESTER",
//                dateborn = DateTime.Now,
//                morerewardsno = "ABCDEFG",
//                IdNumber = "IDNumber",
//                IdType = "TYPE",
//                creditblocked = 0,
//                RFCreditLimit = 0,
//                RFCardSeqNo = 0,
//                RFCardPrinted = "n",
//                empeenochange = 0,
//                OldRFCreditLimit = 0,
//                LimitType = "n",
//                AvailableSpend = 0,
//                InstantCredit = "n",
//                StoreType = "n",
//                LoanQualified = false,
//                dependants = 0,
//                maritalstat = "n",
//                Nationality = "n",
//                ScoreCardType = "n",
//                title= "Mr",
//                firstname="Roger"
//            };

//        Acct account = new Acct
//        {
//            acctno = "700900001581",
//            accttype = "T",
//            branchno = branchno ,
//            paidpcent = 0,
//            termstype = "A",
//            repossarrears = 0,
//            repossvalue = 0,
//            lastupdatedby = 0,
//            Securitised = "A",
//            bdwbalance = 0,
//            bdwcharges = 0,
//            hasstocklineitems = false
//        };


//        [SetUp]
//        public void Setup()
//        {
//            CountryParameterCache.Cache = new CacheProvider();

//            var db = DatabaseFactory.CreateDatabase();

//            CleanUp(db);

//            db.ExecuteNonQuery(new SqlCommand(string.Format(@"
//INSERT INTO [customer]
//([custid],[branchnohdle],[name],[dateborn],[morerewardsno],[IdNumber],[IdType],[creditblocked],[RFCreditLimit],[RFCardSeqNo],[RFCardPrinted],
//[empeenochange],[OldRFCreditLimit],[LimitType],[AvailableSpend],[InstantCredit],[StoreType],[LoanQualified],[dependants],[maritalstat],[Nationality],
//[ScoreCardType],[firstname],[title])
//VALUES('{0}',{1},'{2}','{3}','{4}','{5}','{6}',{7},{8},{9},'{10}',{11},{12},'{13}',{14},'{15}','{16}','{17}',{18},'{19}','{20}','{21}','{22}','{23}')", 
//             customer.custid,
//             customer.branchnohdle,
//             customer.name,
//             "2011-01-01",
//             customer.morerewardsno,
//             customer.IdNumber,
//             customer.IdType,
//             customer.creditblocked,
//             customer.RFCreditLimit,
//             customer.RFCardSeqNo,
//             customer.RFCardPrinted,//10
//             customer.empeenochange,
//             customer.OldRFCreditLimit,
//             customer.LimitType,
//             customer.AvailableSpend,
//             customer.InstantCredit,//15
//             customer.StoreType,
//             customer.LoanQualified,
//             customer.dependants,
//             customer.maritalstat,
//             customer.Nationality,
//             customer.ScoreCardType,
//             customer.firstname,
//             customer.title)));

//            db.ExecuteNonQuery(new SqlCommand(string.Format(@"
//INSERT INTO [acct]
//           ([acctno]
//           ,[accttype]
//           ,[branchno]
//           ,[paidpcent]
//           ,[termstype]
//           ,[repossarrears]
//           ,[repossvalue]
//           ,[lastupdatedby]
//           ,[Securitised]
//           ,[bdwbalance]
//           ,[bdwcharges]
//           ,[hasstocklineitems])
//     VALUES
//           ('{0}', '{1}', {2}, {3}, '{4}', {5}, {6}, {7}, '{8}', {9}, {10}, '{11}')", 
//            account.acctno, 
//            account.accttype, 
//            account.branchno, 
//            account.paidpcent, 
//            account.termstype, 
//            account.repossarrears, 
//            account.repossvalue, 
//            account.lastupdatedby, 
//            account.Securitised,
//            account.bdwbalance,
//            account.bdwcharges,
//            account.hasstocklineitems
//            )));

//            db.ExecuteNonQuery(new SqlCommand(@"
//INSERT INTO [custacct]
//           ([origbr]
//           ,[custid]
//           ,[acctno]
//           ,[hldorjnt])
//     VALUES
//           (0
//           ,'M246812IRHFKIDSLPTOF'
//           ,'700900001581'
//           ,'H')"));
//        }

//        List<long> testCardNumbers = new List<long>();

//        [TearDown]
//        public void TearDown()
//        {
//            CleanUp(DatabaseFactory.CreateDatabase());
//        }

//        public void CleanUp(Database db)
//        {
//            foreach (var item in testCardNumbers)
//            {
//                db.ExecuteNonQuery(new SqlCommand(string.Format(@"DELETE FROM STORECARDSTATUS WHERE CARDNUMBER = '{0}'", item)));
//            }

//            db.ExecuteNonQuery(new SqlCommand(string.Format(@"DELETE FROM STORECARD WHERE ACCTNO = '700900001581'")));
//            db.ExecuteNonQuery(new SqlCommand(string.Format(@"DELETE FROM STORECARDpaymentdetails WHERE ACCTNO = '700900001581'")));
//            db.ExecuteNonQuery(new SqlCommand(string.Format(@"DELETE FROM CUSTACCT WHERE ACCTNO = '700900001581' AND CUSTID = 'M246812IRHFKIDSLPTOF'")));
//            db.ExecuteNonQuery(new SqlCommand(string.Format(@"DELETE FROM proposal WHERE ACCTNO = '700900001581'")));
//            db.ExecuteNonQuery(new SqlCommand(string.Format(@"DELETE FROM instalplan WHERE ACCTNO = '700900001581'")));
//            db.ExecuteNonQuery(new SqlCommand(string.Format(@"DELETE FROM AGREEMENT WHERE ACCTNO = '700900001581'")));
//            db.ExecuteNonQuery(new SqlCommand(string.Format(@"DELETE FROM ACCT WHERE ACCTNO = '700900001581'")));
//            db.ExecuteNonQuery(new SqlCommand(string.Format(@"DELETE FROM CUSTOMER WHERE CUSTID = 'M246812IRHFKIDSLPTOF'")));
//        }

//        //[Test]
//        //public void CanCorrectlyMapStoreCardRate()
//        //{
//        //    using (var session = sessionFactory.OpenSession())
//        //    {
//        //        new PersistenceSpecification<Domain.StoreCardRate>(session)
//        //            .CheckProperty(c => c.Name, "Rate 1")
//        //            .CheckProperty(c => c.ScoreFrom, 0)
//        //            .CheckProperty(c => c.ScoreTo, 99)
//        //            .CheckProperty(c => c.RetailRateFixed, 10.0m)
//        //        .VerifyTheMappings();
//        //    }
//        //}

//        //[Test]
//        //public void StorecardSaveUpdateTest()
//        //{
//        ////    //Save test rate
//        //    var repository = new StoreCardRatesRepository();
//        //    //var rates = new List<StoreCardRate>();

//        //    var rate1 = new StoreCardRate { Name = "Rate1", };

//        //    rate1.Add(new StoreCardRateDetails { RetailRateFixed = 10.2m, RetailRateVariable = 12.34m, ScoreFrom = 0, ScoreTo = 30 });
//        //    rate1.Add(new StoreCardRateDetails { RetailRateFixed = 10.2m, RetailRateVariable = 12.34m, ScoreFrom = 31, ScoreTo = 60 });

//        //    repository.SaveUpdate(new[]{rate1});

//        //    //var rate2 = repository.Load(r => r.Id == rate1.Id).First();
//        //    var rate2 = repository.Load(q => q.Where(r => r.Id == rate1.Id).Take(1).ToList()).First();
//        //    Assert.IsNotNull(rate2);

//        //    Assert.AreEqual(2, rate2.RateDetails.Count);
//        //    Assert.AreEqual(31, rate2.RateDetails[1].ScoreFrom);

//        ////    //Check if saved correctly
//        ////    var initrate = rates.First();
//        ////    var resultRates = repository.List();
//        ////    Assert.AreEqual(resultRates.Count(), 1);
//        ////    CompareStoreCardRate(resultRates.First(), initrate);

//        ////    //Update test rate
//        ////    initrate.Name = initrate.Name + "2";
//        ////    initrate.ScoreFrom += 3;
//        ////    initrate.ScoreTo += 5;
//        ////    initrate.RetailRateFixed += 3;
//        ////    initrate.RetailRateVariable += 5;
//        ////    repository.SaveUpdate(rates);

//        ////    //check if save correctly
//        ////    resultRates = repository.List();
//        ////    CompareStoreCardRate(resultRates.First(), initrate);

//        ////    //reset rates
//        ////    repository.Delete(rates);
//        ////    resultRates = repository.List();
//        ////    Assert.AreEqual(resultRates.Count(), 0);
//        //}



//        //private void CompareStoreCardRate(StoreCardRate s1, StoreCardRate s2)
//        //{
//        //    Assert.AreEqual(s1.Name, s2.Name);
//        //    Assert.AreEqual(s1.RetailRateFixed, s2.RetailRateFixed);
//        //    Assert.AreEqual(s1.RetailRateVariable, s2.RetailRateVariable);
//        //    Assert.AreEqual(s1.ScoreFrom, s2.ScoreFrom);
//        //    Assert.AreEqual(s1.ScoreTo, s2.ScoreTo);
//        //}

//        //public StoreCardRate CreateRate()
//        //{
//        //    return new StoreCardRate { Name = tname, tscorefrom, tscoreto, tfixed, tvarible);
//        //}

//        //[Test]
//        public void CanCorrectlyMapStoreCard()
//        {
//            // TODO get a reference to an account
//            //using (var session = sessionFactory.OpenSession())
//            //{
//            //    new PersistenceSpecification<Shared.StoreCard>(session)
//            //        .CheckProperty(c => c.CardNumber, 1111222233334444)
//            //        .CheckProperty(c => c.CardName, "MR KERMIT THE FROG")
//            //        .CheckProperty(c => c.IssueYear, (short)2010)
//            //        .CheckProperty(c => c.IssueMonth, (byte)11)
//            //        .CheckProperty(c => c.ExpirationYear, (short)2011)
//            //        .CheckProperty(c => c.ExpirationMonth, (byte)10)
//            //        .CheckProperty(c => c.AcctNo, "...")
//            //    .VerifyTheMappings();
//            //}
//        }


//        //[Test] 
//        public void CheckScoring()
//        {
//             using (var conn = new SqlConnection(STL.DAL.Connections.Default))
//             {
//                 DCountry da = new DCountry();
//                 DataTable dt = da.GetMaintenanceParameters(null, null, "G");
//                 CommonObject co = new CommonObject();

//                 string referralReasons = string.Empty; //IP - 15/03/11 - #3314 - CR1245

//                 if (dt.Rows.Count > 0)
//                     co.Cache["Country"] = new CountryParameterCollection(dt);

//                 conn.Open();
//                    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted)) 
//                    {
                       
//                        BProposal prop = new BProposal();
//                        int user = 99999; string newBand = ""; string refCode = ""; decimal points = 0.0M; decimal RFLimit = 0.0M;
//                        string result = ""; string bureauFailure = ""; bool referDeclined = false;
//                        prop.Score(conn, trans, "G", "700010641061", "R", "AN111152", Convert.ToDateTime("01-Jan-2009 08:48:00"), 700,
//                            out newBand, out refCode, out points, out RFLimit, user, out result, out bureauFailure, ref referDeclined, out referralReasons); //IP - 15/03/11 - #3314 - CR1245 - Return referral reasons
//                        trans.Commit(); 
//                    }
//             }
//        }

//        [Test]
//        public void CheckInterest()
//        {
//            using (var conn = new SqlConnection(STL.DAL.Connections.Default))
//            {
//                DCountry da = new DCountry();
//                DataTable dt = da.GetMaintenanceParameters(null, null, "G");
//                CommonObject co = new CommonObject();

//                string referralReasons = string.Empty; //IP - 15/03/11 - #3314 - CR1245

//                if (dt.Rows.Count > 0)
//                    co.Cache["Country"] = new CountryParameterCollection(dt);

//                conn.Open();
//                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
//                {
//                    var branch = new DBranch();
//                    var transrefno = branch.GetTransRefNo(conn, trans, branchno);
                    
                    
//                    trans.Commit();
//                }
//            }
//        }



//       // [Test]
//        public void AnnualFeeTest()
//        {
//            var sd = new StoreCardRepository();

//            SqlTransaction trans; var acctno= "706900117921";
//            DateTime rundate = DateTime.Now;
//                //Convert.ToDateTime("2012-05-15 10:11:21.023");
//            var ctx = Context.Create(null, null);
//             using (var conn = new SqlConnection(STL.DAL.Connections.Default))
             
//            {
//                var AnnualFee = new FinTrans();
//                CountryMaintenanceSetValue CMSV = new CountryMaintenanceSetValue();
//                CMSV.ExecuteNonQuery("SCAnnualFee", "10");
//                 conn.Open();
//                trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
//                try
//                {
//                    var spd = (ctx.StorecardPaymentDetails
//                       .Where(x => x.acctno == acctno//&& x.DatePaymentDue > DateTime.MinValue
//                       )).AnsiFirstOrDefault(ctx);

//                    spd.LastInterestDate = DateTime.Today.AddYears(-1).AddDays(-1);

//                    spd.DatePaymentDue = DateTime.Today.AddYears(-1).AddMonths(10);
//                    var icd = (ctx.InterfaceControl
//                        .Where(x => x.Interface.ToUpper() == "STINTEREST")
//                        .OrderByDescending(x => x.RunNo  ) ).AnsiFirstOrDefault(ctx);

//                    var ic = new InterfaceControl
//                    {
//                        Interface = "STInterest",
//                        RunNo = icd.RunNo + 1,
//                        DateStart = DateTime.Today.AddDays(-2).AddMinutes(22),
//                        DateFinish = DateTime.Today.AddDays(-2).AddMinutes(26),
//                        Result = "P",
//                        StageCompleted = "1"

//                    };
//                    ctx.InterfaceControl.InsertOnSubmit(ic);
//                    var scss = (ctx.StoreCardStatus
//                        .Where(x => x.CardNumber == 4901000000035842 && x.StatusCode=="A")
//                        ).AnsiFirstOrDefault(ctx);

//                    scss.DateChanged = DateTime.Today.AddYears(-1).AddDays(-1);                    //var scs = new StoreCardStatus
//                    //{
//                    //    CardNumber = 4901000000035842,
//                    //    DateChanged = DateTime.Today.AddYears(-1).AddDays(-1),
//                    //    StatusCode = "A",
//                    //    EmpeeNo = 99999,
//                    //    BranchNo = branchno,
//                    //    Notes = "AA test fee"
//                    //};
//                    //ctx.StoreCardStatus.InsertOnSubmit(scs);
//                    ctx.SubmitChanges();
//                    sd.LoadAnnualFees(conn, trans, rundate);
//                    trans.Commit();
                    
//                    AnnualFee = (ctx.FinTrans
//                        .Where(x => x.acctno == acctno && x.transtypecode == TransType.StoreCardAnnualFee
//                        && x.datetrans >= rundate)).AnsiFirstOrDefault(ctx);

//                    //double interest = sd.CalculateInterestForAccount(null, null, pd.acctno, Convert.ToDateTime(pd.LastInterestDate), 
//                    //    Convert.ToDateTime(pd.DatePaymentDue).AddDays(-5),
//                    //    Convert.ToSingle(pd.InterestRate), Convert.ToDateTime(pd.DatePaymentDue));

//                    //trans.Rollback();
//                    //  Assert.IsTrue(interest > 0, "interest not > 0");
//                    //    Assert.IsTrue(sd.Averagebalance > 70, "Average Balance incorrect" + Convert.ToString(sd.Averagebalance));
//                }
//                catch
//                {
//                    trans.Rollback();
//                }

//                Assert.IsTrue(AnnualFee != null, " No annual Fee");
//                Assert.IsTrue(AnnualFee.transvalue == 10, "Annual Fee for card should be 10");
                    
//            };

//        }
//        [Test, Ignore]
//        public void StoreCardBlockandFeeTest()
//        {
//            var scr = new StoreCardRepository();
//            var creditArrearsAccount = new Acct();
//            #region checkcreditaccountwitharrearsfails
//            var vardate = DateTime.Now;
//            var Paymentdetails = new StorecardPaymentDetails {LastInterestDate= vardate.AddMonths(-1).AddDays(-10) };
//            Acct StoreAccount = new Acct();
//            creditArrearsAccount.arrears = 200;
//            Customer cust = new Customer();
//            cust.creditblocked = 0;
            
//            StoreAccount.outstbal = 222m;
//            var result = scr.DetermineBlock(creditArrearsAccount, null, vardate, Paymentdetails); //,StoreAccount);
//            Assert.IsTrue(result == true, "Account Should be blocked");
     
//            var Transactions = new List<fintranswithBalancesVW>();
//            vardate = Convert.ToDateTime("2012-01-23 11:22:24.140");

//            bool paid = false; 
//            //leave for testing individual accounts
//      //      Context.ExecuteTx((ctx, connection, transaction) =>
//      //{
//      //    StoreAccount = (from a in ctx.Acct
//      //               join cu in ctx.CustAcct on a.acctno equals cu.acctno
//      //               join ip in ctx.Instalplan on cu.acctno equals ip.acctno
//      //               where a.acctno == "700900000801"
//      //               select a).AnsiFirstOrDefault(ctx);
//      //    Transactions = (ctx.fintranswithBalancesVW
//      //                      .Where(x => x.acctno == StoreAccount.acctno 
//      //                          //&& x.datetrans >= vardate.AddMonths(-2) 
//      //                          && x.datetrans <= vardate)
//      //                      .OrderBy(x => x.datetrans)).AnsiToList(ctx);
//      //    Paymentdetails = (ctx.StorecardPaymentDetails
//      //        .Where(x => x.acctno == StoreAccount.acctno
//      //        )).AnsiFirstOrDefault(ctx);
//      //}, null, null, null);
//      //      result = scr.DetermineBlockAndMinPayment(null, Transactions, vardate, cust, ref paid,
//      //      Paymentdetails ,StoreAcct: StoreAccount);
//      //      Assert.IsFalse(result == "Block", "Account should not be blocked");
//            #endregion

//            #region account no transactions should not be blocked
            
//            creditArrearsAccount = null;
//            Transactions.Clear();
//            vardate = DateTime.Now;
//            StoreAccount.outstbal = 0m;
//            result = scr.DetermineBlockAndMinPayment(creditArrearsAccount, null, vardate, ref paid,
//                Paymentdetails,0); //, StoreAccount);
//            Assert.IsTrue(paid == true, "Account should be paid as no transactions");
//            #endregion

//            #region account with no payments should be blocked
            
//            var trans = new fintranswithBalancesVW
//            {
//                total = 221,                datetrans = vardate.AddMonths(-3),                transvalue = 221,                transtypecode = TransType.StoreCardPayment
//            };
//            Transactions.Add(trans); 

//            StoreAccount.outstbal = 200m;
//            result = scr.DetermineBlockAndMinPayment(creditArrearsAccount,Transactions, vardate,
//                ref paid, Paymentdetails, 0); //, StoreAccount);
//            Assert.IsTrue(result == true, "Account should be blocked as no transactions in last 2 months but outstanding balance");

//            #endregion
//            cust.creditblocked =1;
//            creditArrearsAccount= null;
//             trans = new fintranswithBalancesVW {
//                total=211,                datetrans=vardate.AddDays(-10),                transvalue=-1,                transtypecode=TransType.Payment         };
//            Transactions.Add(trans); 

//            trans = new fintranswithBalancesVW
//            {
//                total = 310,                datetrans = vardate.AddDays(-40),                transvalue = 100,                transtypecode = TransType.StoreCardPayment
//            };
//            Transactions.Add(trans); 

//            result = scr.DetermineBlock(creditArrearsAccount,Transactions,vardate, Paymentdetails);
//            Assert.IsTrue(result != false, "Account Should not be unblocked");

//            trans = new fintranswithBalancesVW
//            {
//                total = 100,
//                datetrans = vardate.AddDays(-10),
//                transvalue = -70,
//                transtypecode = "PAY"
//            };
//            Transactions.Add(trans);

//            result = scr.DetermineBlock(creditArrearsAccount, Transactions, vardate, Paymentdetails);
//            Assert.IsTrue(result == false, "Account Should not be blocked");


//            creditArrearsAccount = new Acct { arrears = 100, outstbal = 100 };
//            result = scr.DetermineBlock(creditArrearsAccount, Transactions, vardate, Paymentdetails);
//            Assert.IsFalse(result == false, "Account Should not be unblocked as arrears");

//            result = scr.DetermineBlockAndMinPayment(creditArrearsAccount, Transactions, vardate,
//                ref paid,Paymentdetails);
//            Assert.IsTrue(paid == true, "Account should be paid");
//            creditArrearsAccount = null;
            
//            trans = new fintranswithBalancesVW
//            {
//                total = 100,
//                datetrans = vardate.AddDays(-9),
//                transvalue = 70,
//                transtypecode = "REF"
//            };
//            Transactions.Add(trans);
//            result = scr.DetermineBlock(creditArrearsAccount, Transactions, vardate, Paymentdetails );
//            Assert.IsFalse(result == false, "Account Should not be unblocked as refund");

//            result = scr.DetermineBlockAndMinPayment(creditArrearsAccount, Transactions, 
//                vardate, ref paid, Paymentdetails);
//            Assert.IsFalse(paid == true, "Account should be unpaid as returned");

//            #region account with no payments should be blocked
//            Transactions.Clear();
//             trans = new fintranswithBalancesVW
//            {
//                total = 221,
//                datetrans = vardate.AddMonths(-1),
//                transvalue = 221,
//                transtypecode = TransType.StoreCardPayment
//            };
//            Transactions.Add(trans);

//            StoreAccount.outstbal = 200m;
//            result = scr.DetermineBlockAndMinPayment(creditArrearsAccount, Transactions, vardate, ref paid,Paymentdetails,0); //, StoreAccount);
//            Assert.IsFalse(result == true, "Account should not be blocked as no payments but only first missed payment");
//            #endregion
//            // scr.BlockStoreCardAccountCheck(null, null, DateTime.Now, "700900000801 ");
//        }

//        [Test]
//        public void CheckStoreCard()
//        {
//            var customer = new BCustomer();
//            using (var conn = new SqlConnection(STL.DAL.Connections.Default))
//            {
//                conn.Open();

//                var random = new GetRandomAccts();
//                //GetRandomAccts.Reader reader = random.Execute(20,true,"");  //todo do more... 
//                GetRandomAccts.Reader reader = random.Execute(1, false, "R");
//                //using linq with some syntactic sugar.
//                var accts = reader.Select(r => r.acctno).ToArray();

//                using (var trans = conn.BeginTransaction())
//                {
//                    //load up the customer accounts 

//                    foreach (string acctno in accts)
//                    {
//                        decimal availablescbalance = 0.0m;
//                        // customer.GetCustomerAccountsAndDetails(conn, trans, acctno, out availablescbalance);
//                        Assert.NotNull(availablescbalance); //balance should always be numeric....
//                    }
//                }
//            }
//        }

//        [Test]
//        public void CountrySaveTest()
//        {
//            int x;
//            //Get next number
//            Assert.True(int.TryParse(new CountryMaintenanceGetValue { codename = StoreCM.StoreCardNumber }.ExecuteScalar().ToString(), out x), "Is int from country maintenance?");
//            x++;
//            //Update number
//            CountryMaintenanceSetValue CMSV = new CountryMaintenanceSetValue();
//            CMSV.ExecuteNonQuery(StoreCM.StoreCardNumber, x.ToString());
//            //new CountryMaintenanceSetValue { codename = StoreCM.StoreCardNumber, value = x.ToString() }.Execute();
//            Assert.AreEqual(x, int.Parse(new CountryMaintenanceGetValue { codename = StoreCM.StoreCardNumber }.ExecuteScalar().ToString()), "Updated correctly?");
//        }

//        [Test]
//        public void StoreCardGenerate()
//        {
//            var newcardno = StoreCardGen.GenerateCardNumber((string)(new CountryMaintenanceGetValue { codename = StoreCM.StoreCardPrefix }.ExecuteScalar()), int.Parse(new CountryMaintenanceGetValue { codename = StoreCM.StoreCardNumber }.ExecuteScalar().ToString()));
//            Assert.True(StoreCardValidation.IsStoreCardValid(newcardno.PadStoreCard()));
//        }

        
//        public void StoreCardSaveandLoad()
//        {
//            //var card = new StoreCard();

//            var random = new GetRandomAccts();
//            var reader = random.Execute(1, false, "R"); //get some random RF credit accounts
//            //var accts = reader.ToArray();

//            //var accts = reader.Select(r => r.acctno).ToArray();
//            using (var conn = new SqlConnection(STL.DAL.Connections.Default)) // ConfigurationManager.ConnectionStrings["Default"].ConnectionString))
//            {
//                conn.Open();

//                using (var trans = conn.BeginTransaction())
//                {
//                    //load up the customer accounts 

//                    foreach (var item in reader)
//                    {
//                        var strcardno = item.acctno + "1234"; //16 digit number
//                        long cardNo = System.Convert.ToInt64(strcardno);

//           //             new StoreCardSave().ExecuteNonQuery(cardNo, "Ayscough", 2010, 9, 2013, 8, 10.0m, 1, item.acctno, System.DateTime.Now, null, 0, item.custid, 0, "", "", false,
//             //               "Monthly", "Mothers First Name", "AmrutaSimoneRuthSanjeethaKatieAngela");

//                        var StoreCardGet = new Blue.Cosacs.StoreCardGetbyAcctNo();
//                        var reader2 = StoreCardGet.Execute(item.acctno);

//                        int counter = 0;

//                        foreach (var account in reader2)
//                        {
//                            counter++; 
//                            Assert.False(account.CardNumber == 1234567890123459);
//                            if (account.ActivatedOn > DateTime.Now.AddMinutes(-2)) //check most recent one or test was failing
//                                Assert.True(account.CardNumber == cardNo);   
//                            break;
//                        }

//                        Assert.True(counter > 0);
//                    }

//                    trans.Rollback();
//                }
//            }
//        }

//        //IP - 06/12/10 - Test for a valid Store Card number and an invalid Store Card Number
//       //[Test]  
//        public void CheckIsStoreCardNoValid()
//        {
//            // Retrieve a new Store Card number (should be valid)
//            var newcardno = StoreCardGen.GenerateCardNumber((string)(new CountryMaintenanceGetValue { codename = StoreCM.StoreCardPrefix }.ExecuteScalar()), int.Parse(new CountryMaintenanceGetValue { codename = StoreCM.StoreCardNumber }.ExecuteScalar().ToString()));

//            var result = StoreCardValidation.ValidateCardNumber(Convert.ToString(newcardno));
//            Assert.True(result.isValid == true);
//            Assert.False(result.isValid == false);

//            //Test for an incorrect Store Card number
//            newcardno = newcardno + 1;

//            result = StoreCardValidation.ValidateCardNumber(Convert.ToString(newcardno));
//            Assert.True(result.isValid == false);
//            Assert.False(result.isValid == true);
//        }

//        //IP - 04/01/2011 - Store Card validation test
//        //[Test]
//        //public void StoreCardValidationTest()
//        //{

//        //    DateTime? activatedOn;
//        //    DateTime? lostOrStolen;
//        //    int? deleted;
//        //    byte expirationMonth = 8;
//        //    short expirationYear = 2013;

//        //    //Perform a test where store card is valid.
//        //    activatedOn = System.DateTime.Now;
//        //    lostOrStolen = null;
//        //    deleted = 0;

//        //    var result = ValidateStoreCardValues(activatedOn, deleted, lostOrStolen, expirationMonth, expirationYear);

//        //    Assert.True(result.isValid == true);
//        //    Assert.False(result.isValid == false);

//        //    //Perform a test where card has not been activated yet (not valid)
//        //    activatedOn = null;
//        //    lostOrStolen = null;
//        //    deleted = 0;

//        //    result = ValidateStoreCardValues(activatedOn, deleted, lostOrStolen, expirationMonth, expirationYear);

//        //    Assert.True(result.isValid == false);
//        //    Assert.False(result.isValid == true);

//        //    ////Perform a test where the card has been marked as deleted (not valid) -- removing as deleted not on database - need to check in and rethink...
//        //    //activatedOn = System.DateTime.Now;
//        //    //lostOrStolen = null;
//        //    //deleted = 1;

//        //    //result = ValidateStoreCardValues(activatedOn, deleted, lostOrStolen, expirationMonth, expirationYear);

//        //    //Assert.True(result.isValid == false);
//        //    //Assert.False(result.isValid == true);

//        //    //Perform a test where the card has been marked as lost or stolen (not valid)
//        //    activatedOn = System.DateTime.Now;
//        //    lostOrStolen = System.DateTime.Now;
//        //    deleted = 0;

//        //    result = ValidateStoreCardValues(activatedOn, deleted, lostOrStolen, expirationMonth, expirationYear);
//        //    Assert.True(result.isValid == false);
//        //    Assert.False(result.isValid == true);

//        //    //Perform a test where the card has expired (not valid)
//        //    activatedOn = System.DateTime.Now;
//        //    lostOrStolen = null;
//        //    deleted = 0;
//        //    expirationMonth = 8;
//        //    expirationYear = 2010;

//        //    result = ValidateStoreCardValues(activatedOn, deleted, lostOrStolen, expirationMonth, expirationYear);
//        //    Assert.True(result.isValid == false);
//        //    Assert.False(result.isValid == true);

//        //}

//        ////IP - 04/01/2011 - Store Card validation test TO DO UNCOMMENT AND FIX
//        //public Blue.Cosacs.Shared.StoreCardValidation.ValidatationResult ValidateStoreCardValues(DateTime? activatedOn, int? deleted, DateTime? lostOrStolen, byte expirationMonth, short expirationYear)
//        //{   
//        //    var newcardno = StoreCardGen.GenerateAndSaveCountryStoreCardNumber();

//        //    //Retrieve a new Store Card number (should be valid)
//        //    //var newcardno = StoreCardGen.GenerateCardNumber((string)(new CountryMaintenanceGetValue { codename = StoreCM.StoreCardPrefix }.ExecuteScalar()), int.Parse(new CountryMaintenanceGetValue { codename = StoreCM.StoreCardNumber }.ExecuteScalar().ToString()));

//        //    var random = new GetRandomAccts();
//        //    var reader = random.Execute(1, false, "R"); //get some random RF credit accounts

//        //    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Default"].ConnectionString))
//        //    {
//        //        conn.Open();

//        //        using (SqlTransaction trans = conn.BeginTransaction())
//        //        {
//        //            foreach (var item in reader)
//        //            {
//        //                var acctNo = item.acctno;
//        //                var custid = item.custid;

//        //                //First save Store Card Details to db
//        //                new StoreCardSave().ExecuteNonQuery(Convert.ToInt64(newcardno), "Test", 2010, 9, expirationYear, expirationMonth, 10.0m, 1, acctNo, activatedOn, lostOrStolen, deleted, custid, 0, "", "", false,
//        //                   "Monthly", "Mothers First Name", "Mariam");
//        //            }
//        //            StoreCardRepository SCR = new StoreCardRepository();
//        //            StoreCard sCard = SCR.Get(Convert.ToInt64(newcardno));
                    
//        //            var result = StoreCardValidation.ValidateCardNumber(Convert.ToString(newcardno));

//        //            //Perform other validation checks on the saved store card
//        //            result = StoreCardValidation.VaildateCard(sCard);

//        //            trans.Rollback();

//        //            return result;
//        //        }
//        //    }

//        //}

//        public long GenerateCardNumber()
//        {
//            return StoreCardGen.GenerateCardNumber((string)(new CountryMaintenanceGetValue { codename = StoreCM.StoreCardPrefix }.ExecuteScalar()), int.Parse(new CountryMaintenanceGetValue { codename = StoreCM.StoreCardNumber }.ExecuteScalar().ToString()));
//        }

//        [Test]
//        public void StoreCardSaveandLoadRepository()
//        {
//            //StoreCardRepository RateMap = new StoreCardRepository();

//            //RateMap.StoreCardGetbyAcctnoCustid(

//            //StoreCardGetbyAcctnoCustid(string Accountno,string CustomerId, out DateTime? ActivatedOn,out string CardName,
//            //out long? CardNumber,out bool? Deleted, out byte? ExpirationMonth,out short? ExpirationYear,
//            //out byte? IssueMonth,out short? IssueYear,out DateTime? LostorStolenOn, out decimal? FixedRate,int? Id,
//            //out decimal? VariableRate )

//        }

//        //private static ISessionFactory CreateSessionFactory()
//        //{
//        //    return Fluently.Configure()
//        //        .Database(MsSqlConfiguration.MsSql2005.ConnectionString(c => c.FromConnectionStringWithKey("Default")))
//        //        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Mappings.StoreCardRateMap>())
//        //        .BuildSessionFactory();
//        //}

//  //      [Test]
//        public void CreateTest()
//        {
//            var random = new GetRandomAccts();
//            //GetRandomAccts.Reader reader = random.Execute(20,true,"");  //todo do more... 
//            GetRandomAccts.Reader reader = random.Execute(1, false, "R");
//            //using linq with some syntactic sugar.
//            //var accts = reader.Select(r => r.acctno).ToArray();

//            //accts[0].ToString();
            
                
//            //        foreach (string acctno in accts)
//            //        {

//            //}
//            var repo = new StoreCardRepository();
//            var conn = new SqlConnection(STL.DAL.Connections.Default);
//            conn.Open();
//            SqlTransaction trans = conn.BeginTransaction();
//            var createResponse =
//                repo.Create(new CreateRequest
//                {
//                    storeCardNew = new StoreCardNew
//                        {
//                            AcctNo = account.acctno,
//                            CustId = customer.custid
//                        }
//                }, conn: conn, trans: trans);

//            Assert.IsNotNull(createResponse.CardNumber);

//            testCardNumbers.Add(createResponse.CardNumber);

//            Console.WriteLine(string.Format(@"
//Card Number : {0}", createResponse.CardNumber));
//            trans.Commit(); 
//        }

//   //   [Test]

//        public void ActivationTest()
//        {
           
//            var repo = new StoreCardRepository();
//            var createResponse =
//                repo.Create(new CreateRequest
//                {
//                    storeCardNew = new StoreCardNew
//                        {
//                            AcctNo = account.acctno,
//                            CustId = customer.custid
//                        }
//                });

//            testCardNumbers.Add(createResponse.CardNumber);

//            var request = new ActivateRequest
//            {
//                BranchNo = 700,
//                EmpeeNo = 99999,
//                CardNumber= createResponse.CardNumber
//            };

//            repo.Activate(request);
//        }

//        //[Test]
//        //public void CancelScoreTest()
//        //{
//        //    CountryMaintenanceSetValue CMSV = new CountryMaintenanceSetValue();
//        //    CMSV.ExecuteNonQuery(StoreCM.StoreCardNumber, x.ToString());
      

//        //}

//        [Test]
//        public void StoreCardCredit()
//        {

//            var storecardcredit = new StoreCardCredit(23)
//            {
//                RFAvailable = 1414.99M,
//                RFLimit = 3443,
//                Active = false,
//                Fintrans = new List<View_StoreCardTransactionsByCustid>() 
//                { 
//                    new View_StoreCardTransactionsByCustid { transvalue = 334.34M, transtypecode = "PAY"},
//                    new View_StoreCardTransactionsByCustid { transvalue = -311.34M, transtypecode = "rt"},
//                    new View_StoreCardTransactionsByCustid { transvalue = 445.34M, transtypecode = "PAY"},
//                    new View_StoreCardTransactionsByCustid { transvalue = 345.343M, transtypecode = "PAY"},
//                     new View_StoreCardTransactionsByCustid { transvalue = 35.35M, transtypecode = "Int"},
//                      new View_StoreCardTransactionsByCustid { transvalue = -75.34M, transtypecode = "STI"}
//                }
//            };

//        }
//    }
//}
