using System.Collections.Generic;
using Blue.Cosacs.Shared;
using System.Linq;
using System;
using System.Data.SqlClient;
using STL.DAL;
using System.Data;



namespace Blue.Cosacs.Repositories
{
    /// <summary>
    /// Data access object for accounts
    /// </summary>
    public class StoreCardRatesRepository
    {


        public IEnumerable<Blue.Cosacs.Shared.StoreCardRate> LoadAll()
        {
            using (var ctx = Context.Create())
            {
                var reader = (from S in ctx.StoreCardRate
                              select new { S.Name, S.Id, S.RateFixed, S.IsDefaultRate }).ToArray();


                foreach (var record in reader)
                {
                    StoreCardRate rate = new StoreCardRate();
                    rate.Id = (int)record.Id;
                    rate.Name = record.Name;
                    rate.RateFixed = (bool)record.RateFixed;
                    rate.IsDefaultRate = (bool)record.IsDefaultRate;

                    var rateDetails = new List<StoreCardRateDetails>();

                    foreach (var item in (from D in ctx.StoreCardRateDetails
                                          where D.ParentID == record.Id
                                          select new
                                          {
                                              D.ParentID,
                                              D.Id,
                                              D.PurchaseInterestRate,
                                              D.BehaveScoreTo,
                                              D.BehaveScoreFrom,
                                              D.AppScoreFrom,
                                              D.AppScoreTo
                                          }))
                    {
                        rate.RateDetails.Add(new StoreCardRateDetails
                                   {
                                       ParentID = item.ParentID,
                                       Id = item.Id,
                                       PurchaseInterestRate = item.PurchaseInterestRate,
                                       BehaveScoreTo = item.BehaveScoreTo,
                                       BehaveScoreFrom = item.BehaveScoreFrom,
                                       AppScoreFrom = item.AppScoreFrom,
                                       AppScoreTo = item.AppScoreTo
                                   });

                    }
                    yield return rate;
                }
            }

        }



        public void Save(IEnumerable<StoreCardRate> rates, string userId)
        {
            var RatesUser = new { rates, userId };
            EventStore.Instance.Log(RatesUser, "StoreCardRatesSave", EventCategory.StoreCard, new {empeeno= RatesUser.userId });
          
                
                //( request.storeCardNew, "CreateStoreCard", EventCategory.StoreCard, new { CustId = request.storeCardNew.CustId, AcctNo = request.storeCardNew.AcctNo });

            string userName = "";
            using (var ctx = Context.Create())
            {
                userName = (from p in ctx.UserView
                            where p.Id == Convert.ToInt32(userId)
                            select p).First().FullName;

                StoreCardRateSave rateSave = new StoreCardRateSave();
                StoreCardRateDetailsSave details = new StoreCardRateDetailsSave();

                var newrateId = from r in rates   
                                select r.Id;

                var delrates =     from x in ctx.StoreCardRate
                                   where !(newrateId).Contains(x.Id)
                                   select x.Id;

                foreach (var delId in delrates)
                    new StoreCardRateDelete() { Id = delId, user = userName }.ExecuteNonQuery(); // Delete old rates.
               
                foreach (var rate in rates)
                {
                    if (rate.Modified == true || rate.Id < 0) //  Check if update is needed.
                    {
                        //EventStore.Instance.Log(rate, "StoreCardRatesSave", EventCategory.StoreCard, new { empeeno = RatesUser.userId });
          
              
                        new StoreCardRateDelete() { Id = rate.Id, user = userName }.ExecuteNonQuery();

                        int? parentId = rate.Id;
                        int? idout = 0;

                        if (rate.RateDetails.Count > 0)
                        {
                            rateSave.ExecuteNonQuery(false, rate.Name, parentId, out idout, rate.RateFixed, rate.IsDefaultRate);

                            if (idout != null && idout > 0 ) // using this as passing parameter out or by ref doesn't pass parameter in properly
                                parentId = (int)idout;
                            
                            foreach (var rateDetails in rate.RateDetails)
                            {
                                if (rateDetails.Id == 0)
                                {
                                    var rec = from p in ctx.StoreCardRateDetails
                                              orderby p.Id descending
                                              select p.Id;

                                    rateDetails.Id = rec.FirstOrDefault() + 1; //increment by 1 for the next record... 
                                }
                          //      EventStore.Instance.Log(rateDetails, "StoreCardRatesDetailsSave", EventCategory.StoreCard, new { empeeno = RatesUser.userId });
          
                               details.ExecuteNonQuery(rateDetails.Id, parentId, (short?)rateDetails.AppScoreFrom, (short?)rateDetails.AppScoreTo,
                               (short?)rateDetails.BehaveScoreFrom, (short?)rateDetails.BehaveScoreTo,
                               Convert.ToDouble(rateDetails.PurchaseInterestRate), userName, DateTime.Now, rate.Name);

                            }
                        }
                    }
                }
            }
        }


    

        public static List<Blue.Cosacs.Shared.StoreCardRate> StorecardRatesonPoints(string accountNo)
        {
            var connection = new SqlConnection(Connections.Default);
            var customerId = "";
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string scoreCard = string.Empty; int points = 0;
            using (var ctx = Context.Create(connection))
            {
                var custacct= (from p in ctx.CustAcct
                              where p.acctno == accountNo && p.hldorjnt== "H"
                              
                              select new { p.custid }).AnsiFirstOrDefault(ctx);
                customerId= custacct.custid;
                
                var record = (from p in ctx.Proposal
                              where p.custid == customerId && p.points > 0
                              orderby p.dateprop descending
                              select new { p.scorecard, p.points }).AnsiFirstOrDefault(ctx);
                scoreCard = record.scorecard; points = record.points;
                if (scoreCard == null)
                    scoreCard = "A"; //default to applicant
            };

            var list = new List<StoreCardRate>();
    
            return list;
        }

        public bool RateInUse(int id)
        {
            var connection = new SqlConnection(Connections.Default);

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            using (var ctx = Context.Create(connection))
            {
                var record = (from SCR in ctx.StoreCardRate
                              join SCRD in ctx.StoreCardRateDetails on SCR.Id equals SCRD.ParentID
                              join PD in ctx.StorecardPaymentDetails on SCRD.Id equals PD.RateId
                              where SCR.Id == id
                              select SCR).Count();
               if (record > 0)
                   return true;
               else
                   return false;
            };

        }



    }

}
