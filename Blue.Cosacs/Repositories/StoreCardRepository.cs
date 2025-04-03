using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Services.StoreCard;
using Blue.Cosacs.StoreCardUtil;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.FTransaction;
using STL.DAL;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Repositories
{
    public class StoreCardRepository
    {
        /// <summary>
        /// Creates a new Card
        /// </summary>
        /// <param name="createRequest"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="DefaultCardMonths"></param>
        /// <returns></returns>
        /// 
        private int decimalPlaces;
        private decimal storecardPercent;
        private int storecardDefaultMonths;
        private decimal CtySmallbalance;
        private decimal CtyAbsoluteMinPayment;
        private double IntFreeDays;
        public double Averagebalance { get; set; }
        public double interest { get; set; }
        public DateTime batchStatementsDatePrinted { get; set; }            //#12200
        public bool reprintBatch { get; set; }            //#12200
        public int batchRunNo { get; set; }            //#12200

        public StoreCardRepository()
        {
            decimalPlaces = Convert.ToInt32(CountryParameterCache.GetCountryParameter<string>(CountryParameterNames.DecimalPlaces).Substring(1, 1));
            storecardPercent = CountryParameterCache.GetCountryParameter<decimal>(CountryParameterNames.StoreCardPaymentPercent);
            storecardDefaultMonths = CountryParameterCache.GetCountryParameter<int>(CountryParameterNames.StoreCardDefaultCardMonths);
            CtySmallbalance = CountryParameterCache.GetCountryParameter<int>(CountryParameterNames.STMinBalanceforFee);
            CtyAbsoluteMinPayment = CountryParameterCache.GetCountryParameter<decimal>(CountryParameterNames.StoreCardMinPayment);
            IntFreeDays = CountryParameterCache.GetCountryParameter<double>(CountryParameterNames.SCardInterestFreeDays);
        }




        public CustAcct ExistingAccount(string custid, string accttype, SqlConnection conn = null, SqlTransaction trans = null, Context context = null)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
                 {
                     return (from c in ctx.CustAcct
                             join a in ctx.Acct on c.acctno equals a.acctno
                             where c.custid == custid &&
                             c.hldorjnt == "H" && a.accttype == accttype
                             select c).AnsiFirstOrDefault(ctx);
                 }, context, conn, trans);
        }

        public void AccountStatusUpdate(string acctno, string status, SqlConnection conn = null, SqlTransaction trans = null, Context context = null)
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var payment = ctx.StorecardPaymentDetails
                              .Where(p => p.acctno == acctno).AnsiFirstOrDefault(ctx);
                payment.Status = status;
                if (context == null)
                    ctx.SubmitChanges();
            }, context, conn, trans);
        }

        public VIEW_StoreCardStatusLatest ExistingCard(SqlConnection conn, SqlTransaction trans, string AccountNo)
        {
            var storecardStatus = new VIEW_StoreCardStatusLatest();
            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                storecardStatus = (from c in ctx.VIEW_StoreCardStatusLatest
                                   where c.AcctNo == AccountNo && c.StatusCode != StoreCardCardStatus_Lookup.Cancelled.Code
                                   select c).AnsiFirstOrDefault(ctx);

            }, null, conn, trans);

            return storecardStatus;
        }

        public CreateResponse CreateandScore(StoreCardNew newSC, SqlConnection conn = null, SqlTransaction trans = null)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                RecalculateAvailable(newSC.CustId, connection, transaction, ctx);
                return Create(new CreateRequest { storeCardNew = newSC }, connection, transaction, ctx);
            }, conn: conn, trans: trans);
        }

        public CreateResponse Create(CreateRequest createRequest, SqlConnection conn = null, SqlTransaction trans = null, Context context = null)
        {
            //EventStore.Instance.Write(createRequest.storeCardNew);
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var scNew = createRequest.storeCardNew;
                var newCardNumber = StoreCardGen.GenerateAndSaveCountryStoreCardNumber();
                var expirationDate = DateTime.Now.AddMonths(storecardDefaultMonths);

                var cust = ctx.Customer
                            .Where(c => c.custid == createRequest.storeCardNew.CustId).AnsiFirstOrDefault(ctx);

                var name = new StoreCardCalcs.StoreCardCustDetails() { Custid = cust.custid, LastName = cust.name, Name = cust.firstname, Title = cust.title }.NameConCat();

                ctx.StoreCard.InsertOnSubmit(new Blue.Cosacs.Shared.StoreCard
                {
                    CardNumber = newCardNumber,
                    CustID = scNew.CustId,
                    CardName = name,
                    AcctNo = scNew.AcctNo,
                    IssueMonth = Convert.ToByte(DateTime.Now.Month),
                    IssueYear = Convert.ToInt16(DateTime.Now.Year),
                    ExpirationMonth = Convert.ToByte(expirationDate.Month),
                    ExpirationYear = Convert.ToInt16(expirationDate.Year),
                    Source = scNew.Source
                });

                if (scNew.Source == "Replacement")
                {
                    CheckReplacementFee(scNew.AcctNo, connection, transaction, scNew.CustId);
                }

                ctx.StoreCardStatus.InsertOnSubmit(new StoreCardStatus
                {
                    CardNumber = newCardNumber,
                    DateChanged = DateTime.Now,
                    StatusCode = Blue.Cosacs.Shared.StoreCardAccountStatus_Lookup.CardToBeIssued.Code,
                    EmpeeNo = scNew.User,
                    BranchNo = Convert.ToInt16(scNew.AcctNo.Substring(0, 3))
                });

                var StoreCardWithPayments = (from s in ctx.View_StoreCardWithPayments
                                             where s.Acctno == scNew.AcctNo
                                             orderby s.Cancelled ascending, s.Holder, s.CardNumber descending // get the most recent cards which are not cancelled first
                                             select s).AnsiToList(ctx);

                var accountstatus = string.Empty;

                if (StoreCardWithPayments.Count == 0)
                {
                    var rate = (from s in ctx.View_StoreCardRateDetailsGetforPoints
                                where s.Custid == cust.custid &&
                                s.IsDefaultRate &&
                                (s.Points == 0 ||
                                (s.Points != 0 && s.Scorecard.Trim() == "B" && s.Points >= s.BehaveScoreFrom && s.Points <= s.BehaveScoreTo) ||
                                (s.Points != 0 && s.Scorecard.Trim() != "B" && s.Points >= s.AppScoreFrom && s.Points <= s.AppScoreTo))
                                select s).AnsiFirstOrDefault(ctx);

                    if (rate == null)
                        throw new Exception(String.Format("No rate matching for points and scorecard for custid {0}", cust.custid));

                    // var IntFreeDays = CountryParameterCache.GetCountryParameter<double>(CountryParameterNames.SCardInterestFreeDays);


                    var pay = new StorecardPaymentDetails
                        {
                            acctno = scNew.AcctNo,
                            MonthlyAmount = 0,
                            PaymentMethod = "CASH", //todo fix this...
                            PaymentOption = "FB", // and this maybe.. shouldn't hard code
                            RateId = rate.Id, // Storecardratedetails ID. 
                            InterestRate = Convert.ToDouble(rate.PurchaseInterestRate),
                            NoStatements = false,
                            StatementFrequency = CountryParameterCache.GetCountryParameter<string>(CountryParameterNames.StoreCardStatementFrequency),
                            DateLastStatementPrinted = DateTime.Today.AddDays(-1),
                            DatePaymentDue = NotLastDay(DateTime.Today.AddMonths(1).AddDays(IntFreeDays - 1)),
                            RateFixed = rate.RateFixed,
                            ContactMethod = "Post",
                            Status = StoreCardAccountStatus_Lookup.CardToBeIssued.Code,
                            LastUpdatedBy = scNew.User,
                            CardLimit = scNew.StoreCardLimit,
                            CardAvailable = scNew.StoreCardAvailable,
                            LastInterestDate = DateTime.Today.AddDays(-1)
                        };
                    accountstatus = pay.Status = CalculateAccountStatus(ctx, cust: cust, acctno: scNew.AcctNo);

                    ctx.StorecardPaymentDetails.InsertOnSubmit(pay);

                }
                else
                {
                    var payment = ctx.StorecardPaymentDetails
                                  .Where(p => p.acctno == scNew.AcctNo).SingleOrDefault();//).AnsiFirstOrDefault(ctx);
                    accountstatus = payment.Status = CalculateAccountStatus(ctx, cust: cust, acctno: scNew.AcctNo);
                }
                ctx.SubmitChanges();

                return new CreateResponse
                {
                    CardNumber = newCardNumber,
                    CustID = scNew.CustId,
                    CardName = name,
                    AcctNo = scNew.AcctNo,
                    IssueMonth = Convert.ToByte(DateTime.Now.Month),
                    IssueYear = Convert.ToInt16(DateTime.Now.Year),
                    ExpirationMonth = Convert.ToByte(expirationDate.Month),
                    ExpirationYear = Convert.ToInt16(expirationDate.Year),
                    AccountStatus = accountstatus,
                    CustAddress = ctx.CustAddress
                                  .Where(c => c.custid == scNew.CustId && c.addtype == "H" && c.datemoved == null).AnsiFirstOrDefault(ctx),
                    DOB = cust.dateborn
                };
            }, context, conn, trans);
        }


        public StoreCardCredit StoreCardAvailGet(string custid, Context context = null, SqlConnection conn = null, SqlTransaction trans = null)
        {
            var storeCardCredit = new StoreCardCredit(storecardPercent);
            Context.ExecuteTx((ctx, connection, transaction) =>
           {

               var rfLimit = new DN_CustomerGetRFLimitSP(connection, transaction) { custid = custid, AcctList = string.Empty };
               rfLimit.ExecuteNonQuery();

               storeCardCredit.Customer = (ctx.Customer
                                           .Where(c => c.custid == custid)
                                           ).AnsiFirstOrDefault(ctx);

               storeCardCredit.Fintrans = ctx.View_StoreCardTransactionsByCustid
                                          .Where(s => s.custid == storeCardCredit.Customer.custid).AnsiToList(ctx);

               if (storeCardCredit.Fintrans.Count > 0 && storeCardCredit.Fintrans[0].acctno != null)
                   storeCardCredit.Active = ctx.VIEW_StoreCardStatusLatest
                                            .Any(s => s.AcctNo == storeCardCredit.Fintrans[0].acctno && s.StatusCode == StoreCardCardStatus_Lookup.Active.Code);

               storeCardCredit.RFLimit = rfLimit.limit;
               storeCardCredit.RFAvailable = rfLimit.available;
           }, context, conn, trans);
            return storeCardCredit;
        }

        public GetDeliveryDetailsResponse GetDeliveryDetails(string acctno, int agrmtno)
        {
            using (var ctx = Context.Create())
            {
                var Result = new GetDeliveryDetailsResponse
                {

                    Details = (from d in ctx.view_LineDetails
                               where d.AcctNo == acctno && d.AgrmtNo == agrmtno
                               select d).AnsiToList(ctx)

                };
                return Result;
            }

        }
        public StoreCardAccountResult GetStoreCardAccountResult(string acctno, SqlConnection conn = null, SqlTransaction trans = null, Context context = null)
        {

            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var customer = (from c in ctx.CustAcct
                                join cust in ctx.Customer on c.custid equals cust.custid
                                where c.acctno == acctno &&
                                c.hldorjnt == "H"
                                select cust).AnsiFirstOrDefault(ctx);

                RecalculateAvailable(customer.custid, connection, transaction, ctx);

                var result = new StoreCardAccountResult()
                {
                    MainCustid = customer.custid,

                    StoreCardWithPayments = (from s in ctx.View_StoreCardWithPayments
                                             where s.Acctno == acctno //&& s.custid == customer.custid 
                                             orderby s.Cancelled ascending, s.Holder, s.CardNumber descending // get the most recent cards which are not cancelled first
                                             select s).AnsiToList(ctx),
                    Acct = (from a in ctx.Acct
                            where a.acctno == acctno
                            select a).AnsiFirstOrDefault(ctx),

                    StoreCardStatements = (from s in ctx.StoreCardStatement
                                           where s.Acctno == acctno
                                           orderby s.DateFrom descending
                                           select s).AnsiToList(ctx),

                    //CustAddress = ctx.CustAddress
                    //              .Where(ad => ad.custid == customer.custid)
                    //              .Where(ad => ad.datemoved == null && ad.addtype == "H").AnsiFirstOrDefault(ctx),

                    //Customer = customer,
                    Addresses = (from ca in ctx.CustAddress
                                 join st in ctx.StoreCard on ca.custid equals st.CustID
                                 where st.AcctNo == acctno && ca.datemoved == null && ca.addtype == "H"
                                 select ca).AnsiToList(ctx),

                    Customers = (from ca in ctx.Customer
                                 join st in ctx.StoreCard on ca.custid equals st.CustID
                                 where st.AcctNo == acctno
                                 select ca).AnsiToList(ctx),

                    History = (from h in ctx.View_StoreCardHistory
                               where h.Acctno == acctno
                               orderby h.DateChanged descending
                               select h).AnsiToList(ctx),

                    AcceptedAgreement = (from s in ctx.StoreCardStatus
                                         join st in ctx.StoreCard on s.CardNumber equals st.CardNumber
                                         where st.AcctNo == acctno
                                         select s).Any(s => StoreCardCardStatus_Lookup.Active.Code == s.StatusCode),

                    Fintransfers = (from f in ctx.view_FintranswithTransfers
                                    where f.AcctNo == acctno
                                    orderby f.DateTrans descending
                                    select f).AnsiToList(ctx)
                };


                if (result.Acct.outstbal > 0)
                {
                    result.StoreCardWithPayments[0].PendingInterest = PendingInterestCalc(result.StoreCardWithPayments[0]);
                    result.StoreCardWithPayments[0].AverageBalance = Convert.ToDecimal(Averagebalance);
                }
                else
                    result.StoreCardWithPayments[0].PendingInterest = 0;
                result.Rates = (from s in ctx.View_StoreCardRateDetailsGetforPoints
                                where s.Custid == customer.custid &&
                                (s.Points == 0 ||
                                (s.Points != 0 && s.Scorecard.Trim() == "B" && s.Points >= s.BehaveScoreFrom && s.Points <= s.BehaveScoreTo) ||
                                (s.Points != 0 && s.Scorecard.Trim() != "B" && s.Points >= s.AppScoreFrom && s.Points <= s.AppScoreTo) ||
                                 s.Id == result.StoreCardWithPayments[0].RateId)
                                select s).AnsiToList(ctx);

                if (result.AccountStatus == null)
                    result.AccountStatus = CalculateAccountStatus(ctx, cust: customer, stpay: result.StoreCardWithPayments, acct: result.Acct);
                return result;
            }, context, conn, trans);
        }

        public decimal PendingInterestCalc(View_StoreCardWithPayments SCP)
        {
            return Convert.ToDecimal(CalculateInterestForAccount(SCP.Acctno, SCP.LastInterestDate.Value.AddDays(1), SCP.InterestRate.Value));       // #9545 jec 31/01/12cctno, SCP.LastInterestDate.Value.AddDays(1),  SCP.InterestRate.Value, dateFrom: SCP.LastInterestDate.Value.AddMonths(1).IncludeDay(), dateTo: SCP.LastInterestDate.Value.AddMonths(1).IncludeDay(),));       // #9545 jec 31/01/12
        }

        public CheckQualifyResponse CheckQualified(CheckQualifyRequest request, SqlConnection conn = null, SqlTransaction trans = null, Context context = null)
        {
            //EventStore.Instance.Write(request);

            bool? qualified;
            new StoreCardQualify().ExecuteDataSet(request.custid, 0, "", out qualified, DateTime.Now);
            return new CheckQualifyResponse
            {
                qualified = qualified.HasValue ? qualified.Value : false
            };
        }

        public string GetAccountStatus(string acctno, SqlConnection conn = null, SqlTransaction trans = null)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                return CalculateAccountStatus(ctx, acctno: acctno);
            }, null, conn, trans);

        }

        private string CalculateAccountStatus(Context ctx, string custid = null, Customer cust = null, string acctno = null, List<View_StoreCardWithPayments> stpay = null, Acct acct = null)
        {
            if (custid != null && acctno != null && (stpay != null || cust != null))
                throw new Exception("Calculate status not used correctly.");

            if (acctno == null && stpay == null)
            {
                acctno = (from c in ctx.StoreCard
                          where c.CustID == custid
                          select c.AcctNo).AnsiFirstOrDefault(ctx);
            }

            if (acct == null)
            {
                acct = (from a in ctx.Acct
                        where a.acctno == acctno
                        select a).AnsiFirstOrDefault(ctx);

            }

            if (acct.currstatus == "S")
                return StoreCardAccountStatus_Lookup.Cancelled.Code;

            if (custid == null && cust == null)
            {
                custid = (from c in ctx.StoreCard
                          join cu in ctx.CustAcct on c.AcctNo equals cu.acctno
                          where c.AcctNo == acctno &&
                          cu.hldorjnt == "H"
                          select cu.custid).AnsiFirstOrDefault(ctx);
            }

            if (cust == null)
            {
                cust = ctx.Customer
                       .Where(c => c.custid == custid).AnsiFirstOrDefault(ctx);
            }

            if (cust == null)
                return StoreCardAccountStatus_Lookup.Unknown.Code;

            if (Convert.ToBoolean(cust.creditblocked))
                return StoreCardAccountStatus_Lookup.Blocked.Code;

            if (cust.StoreCardApproved.HasValue && !cust.StoreCardApproved.Value)
                return StoreCardAccountStatus_Lookup.Suspended.Code;

            if (stpay == null)
            {
                stpay = ctx.View_StoreCardWithPayments
                       .Where(p => p.Acctno == acctno).AnsiToList(ctx);
            }

            if (stpay == null || stpay.Count == 0)
                return StoreCardAccountStatus_Lookup.CardToBeIssued.Code;

            //Only Storecard payment details status is repeated so take first one.
            if (StoreCardAccountStatus_Lookup.Suspended.Equals(stpay[0].SPDStatus) || StoreCardAccountStatus_Lookup.Cancelled.Equals(stpay[0].SPDStatus))
            {
                return stpay[0].SPDStatus;
            }

            var active = stpay.FindAll(s => StoreCardAccountStatus_Lookup.Active.Equals(s.CardStatus));

            if (active != null && active.Count > 0)
                return StoreCardAccountStatus_Lookup.Active.Code;

            var awaiting = stpay.FindAll(s => StoreCardAccountStatus_Lookup.AwaitingActivation.Equals(s.CardStatus));

            if (awaiting != null && awaiting.Count > 0)
                return StoreCardAccountStatus_Lookup.AwaitingActivation.Code;

            return StoreCardAccountStatus_Lookup.CardToBeIssued.Code;
        }

        public CustDetails GetCustDetails(long CardNo, SqlConnection conn = null, SqlTransaction trans = null, Context context = null)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
              {
                  return (from c in ctx.StoreCard
                          join ca in ctx.CustAcct on c.AcctNo equals ca.acctno
                          join cust in ctx.Customer on ca.custid equals cust.custid
                          join custadd in ctx.CustAddress on ca.custid equals custadd.custid
                          where ca.hldorjnt == "H" &&
                          custadd.addtype == "H" &&
                          custadd.datemoved == null &&
                          c.CardNumber == CardNo
                          select new CustDetails
                          {
                              AddressLine1 = custadd.cusaddr1,
                              AddressLine2 = custadd.cusaddr2,
                              AddressLine3 = custadd.cusaddr3,
                              AddressLine4 = custadd.cuspocode,
                              FirstName = cust.firstname,
                              LastName = cust.name,
                              Title = cust.title
                          }).SingleOrDefault();
              }, context, conn, trans);
        }


        public ActivateResponse Activate(ActivateRequest activateRequest, SqlConnection conn = null, SqlTransaction trans = null, Context context = null)
        {
            //  EventStore.Instance.Write(activateRequest);

            var ScStatus = new StoreCardStatus();
            var name = string.Empty;

            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var storeCard = ctx.StoreCard.Where(x => x.CardNumber == activateRequest.CardNumber).AnsiFirstOrDefault(ctx);

                var cust = (from c in ctx.Customer
                            join ca in ctx.CustAcct on c.custid equals ca.custid
                            where ca.acctno == storeCard.AcctNo &&
                            ca.hldorjnt == "H"
                            select c).AnsiFirstOrDefault(ctx);

                var status = ctx.StoreCardStatus
                    .Where(c => c.CardNumber == activateRequest.CardNumber)
                    .OrderByDescending(x => x.DateChanged)
                    .AnsiFirstOrDefault(ctx);

                var cardIsActive = status == null ? false : status.StatusCode == StoreCardAccountStatus_Lookup.Active.Code ? true : false;

                var notes = string.Empty;

                //if (!string.IsNullOrWhiteSpace(activateRequest.SecurityQ.Trim())) //Ignore for preapproval activation.
                if (activateRequest.SecurityQ != null && activateRequest.SecurityQ.Trim() != string.Empty) //Ignore for preapproval activation. //IP - 08/03/12 -  #9753 
                {
                    storeCard.ProofAddNotes = activateRequest.ProofAddNotes;
                    storeCard.ProofAddress = activateRequest.ProofAddress;
                    storeCard.ProofID = activateRequest.ProofID;
                    storeCard.ProofIDNotes = activateRequest.ProofIDNotes;
                    storeCard.SecurityA = activateRequest.SecurityA;
                    storeCard.SecurityQ = activateRequest.SecurityQ;
                    notes = activateRequest.Reason;
                    if (activateRequest.Reason == "Save Details")
                        notes += " Security Answer: " + activateRequest.SecurityA;
                }

                ////check previous status if was cancelled then should go back to previous status.
                //var PreviousStatus = (from sa in ctx.StoreCardStatus
                //                       where sa.CardNumber == activateRequest.CardNumber
                //                          && sa.StatusCode != Blue.Cosacs.Shared.StoreCardAccountStatus_Lookup.Cancelled.Code
                //                       orderby sa.DateChanged descending
                //                       select sa).AnsiFirstOrDefault(ctx);
                //var statusCode = "";
                //if (PreviousStatus.StatusCode == Blue.Cosacs.Shared.StoreCardAccountStatus_Lookup.AwaitingActivation.Code
                //    || PreviousStatus.StatusCode == Blue.Cosacs.Shared.StoreCardAccountStatus_Lookup.Active.Code) //if previous status was awaiting activation then activate.
                //{
                //    statusCode = Blue.Cosacs.Shared.StoreCardAccountStatus_Lookup.Active.Code;
                //else
                //    statusCode = StoreCardStatus.StatusCode;
                // only save if not active already
                //ffgg

                if (!cardIsActive)
                {
                    var pay = ctx.StorecardPaymentDetails
                              .Where(c => c.acctno == storeCard.AcctNo).AnsiFirstOrDefault(ctx);

                    if (storeCard.CustID == cust.custid && storeCard.Source != "Replacement" && storeCard.Source != "Additional")
                    {//if main holder and first card
                        CheckAnnualFee(storeCard.AcctNo, connection, transaction, cust.custid);
                        // var IntFreeDays = CountryParameterCache.GetCountryParameter<double>(CountryParameterNames.SCardInterestFreeDays);

                        pay.DateLastStatementPrinted = DateTime.Today.AddDays(-1);
                        pay.DatePaymentDue = NotLastDay(DateTime.Today.AddMonths(1).AddDays(IntFreeDays));
                        pay.LastInterestDate = DateTime.Today.AddDays(-1);

                    }

                    if (!Convert.ToBoolean(cust.creditblocked))
                    {
                        ScStatus = new StoreCardStatus
                       {
                           CardNumber = activateRequest.CardNumber,
                           DateChanged = DateTime.Now,
                           StatusCode = StoreCardCardStatus_Lookup.Active.Code,
                           EmpeeNo = activateRequest.EmpeeNo,
                           BranchNo = activateRequest.BranchNo,
                           Notes = notes
                       };
                        ctx.StoreCardStatus.InsertOnSubmit(ScStatus);
                        //calculate account status looks at db but not updated yet
                        pay.Status = CalculateAccountStatus(ctx, cust: cust, acctno: storeCard.AcctNo);
                        if (pay.Status == StoreCardAccountStatus_Lookup.AwaitingActivation.Code)
                            pay.Status = StoreCardAccountStatus_Lookup.Active.Code; //as we are currently activating. 
                    }
                }
                ctx.SubmitChanges();
                return new ActivateResponse
              {
                  StoreCardStatus = ScStatus,
                  Empeename = (ctx.UserView
                               .Where(c => c.Id == activateRequest.EmpeeNo)
                               .Select(c => c.FullName)).AnsiFirstOrDefault(ctx)
              };
            }, context, conn, trans);
        }

        public Blue.Cosacs.Shared.StoreCard Load(long cardNumber)
        {
            using (var ctx = Context.Create())
            {
                return ctx.StoreCard
                    .Where(sc => sc.CardNumber == cardNumber)
                    .AnsiFirstOrDefault(ctx);
            }
        }
        public void LoadAnnualFees(SqlConnection conn, SqlTransaction trans, DateTime rundate, int? commandTimeout = null)
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                //IP - 16/01/13 - Merged from CoSACS 6.5
                if (commandTimeout.HasValue)
                    ctx.CommandTimeout = commandTimeout.Value;
                //get previous rundate
                var ic = (from c in ctx.InterfaceControl
                          where c.Interface.ToUpper() == "STINTEREST"
                              && c.DateStart < rundate && c.Result == "P"
                          orderby rundate descending
                          select c).AnsiFirstOrDefault(ctx);

                if (ic != null && CountryParameterCache.GetCountryParameter<double>(CountryParameterNames.STAnnualFee) > 0)
                {
                    //ActivateDate is always moved on by one year as datediff passes -- see view definition. 
                    var fees = (from c in ctx.StoreCardLoadforAnnualFee_View
                                where c.ActivateAnniversary >= ic.DateStart.AddYears(-1) && c.ActivateDate < rundate.AddYears(-1)
                                && c.ActivateDate < DateTime.Today.AddYears(-9) // should only have one per year.
                                select c).AnsiToList(ctx);

                    foreach (var account in fees)
                        CheckAnnualFee(account.acctno, connection, transaction, account.custid, commandTimeout: commandTimeout); //IP - 16/01/13 - Merged from CoSACS 6.5
                }
            }, null, conn, trans);

        }
        public void CheckAnnualFee(string acctno, SqlConnection conn, SqlTransaction trans, string custid, Customer cust = null, int? commandTimeout = null) //IP - 16/01/13 - Merged from CoSACS 6.5
        {
            var annualFee = CountryParameterCache.GetCountryParameter<double>(CountryParameterNames.STAnnualFee);
            if (annualFee > 0)
            {
                var branchno = Convert.ToInt16(acctno.Substring(0, 3));
                AddTransaction(acctno, annualFee, DateTime.Now, TransType.StoreCardAnnualFee, custid, branchno: branchno, conn: conn, trans: trans, timeout: commandTimeout); //IP - 16/01/13 - Merged from CoSACS 6.5
            }
        }
        public void CheckReplacementFee(string Acctno, SqlConnection conn, SqlTransaction trans, string custid)
        {

            var ReplacementFee = CountryParameterCache.GetCountryParameter<double>(CountryParameterNames.STReplacementFee);
            if (ReplacementFee > 0)
            {
                short branchno = Convert.ToInt16(Acctno.Substring(0, 3));
                AddTransaction(Acctno, ReplacementFee, DateTime.Now, TransType.StoreCardCardReplaceFee, custid, branchno: branchno, conn: conn, trans: trans);
            }

        }
        public View_StoreCardValidationandLimits Validate(long cardNumber, SqlConnection conn = null, SqlTransaction trans = null, Context context = null)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                if (ctx.StoreCard
                    .Where(s => s.CardNumber == cardNumber).Any())
                {
                    RecalculateAvailable(cardNumber, connection, transaction, ctx);

                    return ctx.View_StoreCardValidationandLimits
                        .Where(sc => sc.CardNumber == cardNumber)
                        .AnsiFirstOrDefault(ctx);
                }
                else
                    return null;
            }, context, conn, trans);
        }

        public View_StoreCardAll GetStoreCardAll(long cardNo)
        {
            using (var ctx = Context.Create())
            {
                return ctx.View_StoreCardAll
                       .Where(s => s.CardNumber == cardNo).Single();
            }
        }


        public List<View_StoreCard> Search(SearchRequest p)
        {
            using (var ctx = Context.Create())
            {
                return (ctx.View_StoreCard
                                .Where(s => s.DateAcctOpen >= p.StartDate &&
                                            s.DateAcctOpen <= p.EndDate)
                                .WhereIf(p.AcctNo != null, s => s.AccountNo == p.AcctNo)
                                .WhereIf(p.StoreCardNo != 0, s => s.CardNumber == p.StoreCardNo)
                                .WhereIf(p.LastName != null, s => s.LastName == p.LastName)
                                .WhereIf(p.Status != StoreCardAccountStatus_Lookup.Unknown.Code, s => s.Status == p.Status)
                                .WhereIf(p.Holder == true, s => s.Holder == "Yes")
                    //.WhereIf(p.Status == StoreCardStatus_Lookup.Suspended.Code, s => s.SCode == "C" && s.Notes=="Suspended")
                                .WhereIf(p.Source != null, s => s.Source.ToUpper().Trim() == p.Source))
                                .WhereIf(p.Branch != null, s => s.AccountNo.StartsWith(p.Branch))
                                .Take(200).AnsiToList(ctx);
            }
        }


        public StorecardPaymentDetails LoadPaymentDetails(string AccountNo)
        {
            using (var ctx = Context.Create())
            {
                return ctx.StorecardPaymentDetails
                    .Where(sc => sc.acctno == AccountNo)
                    .AnsiFirstOrDefault(ctx);
            }


        }

        public void SavePaymentDetails(PayDetailsSaveRequest payrequest, SqlConnection conn = null, SqlTransaction trans = null, Context context = null)
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
           {
               var paynew = payrequest.StorecardPaymentDetails;

               var cust = (from c in ctx.Customer
                           join cu in ctx.CustAcct on c.custid equals cu.custid
                           where cu.acctno == paynew.acctno &&
                           cu.hldorjnt == "H"
                           select c).AnsiFirstOrDefault(ctx);

               var paycurrent = ctx.StorecardPaymentDetails
                            .Where(p => p.acctno == paynew.acctno).AnsiFirstOrDefault(ctx);

               paycurrent.RateId = paynew.RateId;
               paycurrent.StatementFrequency = paynew.StatementFrequency;
               paycurrent.InterestRate = paynew.InterestRate;
               paycurrent.RateFixed = paynew.RateFixed;
               paycurrent.ContactMethod = paynew.ContactMethod;
               paycurrent.CardLimit = paynew.CardLimit;
               paycurrent.LastUpdatedBy = paynew.LastUpdatedBy;
               paycurrent.DatePaymentDue = paynew.DatePaymentDue;
               paycurrent.DateLastStatementPrinted = paynew.DateLastStatementPrinted;
               //      paycurrent.LastInterestDate = paynew.LastInterestDate; shouldn't change from client only from eod

               if (paynew.CardLimit > 0)
               {
                   cust.StoreCardLimit = paynew.CardLimit;
               }
               if (paynew.CardAvailable > 0)
               {
                   cust.StoreCardAvailable = paynew.CardAvailable;
               }

               paycurrent.Status = CalculateAccountStatus(ctx, cust: cust, acctno: paynew.acctno);

               if (paycurrent.Status == StoreCardAccountStatus_Lookup.CardToBeIssued.Code || paycurrent.Status == StoreCardAccountStatus_Lookup.AwaitingActivation.Code) //only change interest date if card to be issued
               {
                   //                   var IntFreeDays = CountryParameterCache.GetCountryParameter<double>(CountryParameterNames.SCardInterestFreeDays);

                   paycurrent.LastInterestDate = Convert.ToDateTime(paynew.DatePaymentDue).AddMonths(-1).AddDays(-IntFreeDays);
               }

               ctx.SubmitChanges();
           }, context, conn, trans);
        }

        public void CancelAccount(View_StoreCardHistory ScHist, SqlConnection conn = null, SqlTransaction trans = null)
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
           {
               var CurrentDate = DateTime.Now;

               var cards = (from s in ctx.StoreCard
                            where s.AcctNo == ScHist.Acctno
                            select s.CardNumber).AnsiToList(ctx);

               var Pay = ctx.StorecardPaymentDetails
                   .Where(s => s.acctno == ScHist.Acctno).AnsiFirstOrDefault(ctx);
               Pay.LastUpdatedBy = ScHist.Empeeno;
               Pay.Status = StoreCardAccountStatus_Lookup.Cancelled.Code;

               if (ScHist.ContactMonths > 0)
                   Pay.NextContactDate = CurrentDate.AddMonths(ScHist.ContactMonths);
               else
                   Pay.NextContactDate = null;

               foreach (var card in cards)
               {
                   ScHist.CardNumber = card;
                   CancelCard(ScHist, ctx, connection, transaction, true);
               }

           }, conn: conn, trans: trans);
        }

        public CancelResponse CancelCard(View_StoreCardHistory ScHist, Context context = null, SqlConnection conn = null, SqlTransaction trans = null, bool CancelAcct = false)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var CurrentDate = DateTime.Now;

                var scstatus = new StoreCardStatus
                {
                    Notes = ScHist.Notes,
                    EmpeeNo = ScHist.Empeeno,
                    CardNumber = ScHist.CardNumber,
                    DateChanged = DateTime.Now,
                    BranchNo = Convert.ToInt16(ScHist.Acctno.Substring(0, 3)),
                    StatusCode = ScHist.StatusCode
                };
                ctx.StoreCardStatus.InsertOnSubmit(scstatus);

                var oldaction = (from sa in ctx.Bailaction
                                 where sa.acctno == ScHist.Acctno &&
                                 sa.allocno == Convert.ToInt32(Convert.ToString(ScHist.CardNumber).Substring(12, 4))
                                 orderby sa.actionno descending
                                 select sa).AnsiFirstOrDefault(ctx);

                var action = new Bailaction
                {
                    origbr = Convert.ToInt16(ScHist.Acctno.Substring(0, 3)),
                    acctno = ScHist.Acctno,
                    allocno = Convert.ToInt32(Convert.ToString(ScHist.CardNumber).Substring(12, 4))
                };

                if (oldaction != null && oldaction.actionno > 0)
                    action.actionno = Convert.ToInt16(oldaction.actionno + 1);
                else
                    action.actionno = 1;

                action.empeeno = ScHist.Empeeno;
                action.dateadded = CurrentDate;
                action.code = ScHist.Code;
                action.actionvalue = 0;
                action.datedue = action.dateadded;
                action.amtcommpaidon = 0;
                action.notes = ScHist.Notes;
                action.addedby = ScHist.Empeeno;
                ctx.Bailaction.InsertOnSubmit(action);

                if (!CancelAcct)
                {
                    var AccountStatus = CalculateAccountStatus(ctx, acctno: ScHist.Acctno);
                    AccountStatusUpdate(ScHist.Acctno, AccountStatus, connection, transaction, ctx);
                }

                ctx.SubmitChanges();

                if (!CancelAcct)
                    return new CancelResponse
                    {
                        StoreCardHistory = (from h in ctx.View_StoreCardHistory
                                            where h.Acctno == ScHist.Acctno
                                            orderby h.DateChanged descending
                                            select h).AnsiToList(ctx),
                        View_StoreCardWithPayments = (from s in ctx.View_StoreCardWithPayments
                                                      where s.Acctno == ScHist.Acctno //&& s.custid == customer.custid 
                                                      orderby s.Cancelled ascending, s.Holder, s.CardNumber descending // get the most recent cards which are not cancelled first
                                                      select s).AnsiToList(ctx)
                    };
                else
                    return new CancelResponse();

            }, context, conn, trans);
        }


        public void SaveDateNotedPrinted(SaveDateNotePrintedRequest request, SqlConnection conn = null, SqlTransaction trans = null, Context context = null)
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
         {
             var spd = ctx.StorecardPaymentDetails
                       .Where(s => s.acctno == request.AcctNo).AnsiFirstOrDefault(ctx);

             spd.DateNotePrinted = DateTime.Now;
             spd.LastUpdatedBy = request.Empeeno;

             ctx.SubmitChanges();
         }, context, conn, trans);
        }

        public DataTable StoreCardExport()
        {
            var selectStoreCards = new StoreCardSelectCardsToExport();
            var ds = selectStoreCards.ExecuteDataSet();
            return ds.Tables[0];
        }

        public View_StoreCardWithPayments Get(Int64 cardno, SqlConnection conn, SqlTransaction trans)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                return ctx.View_StoreCardWithPayments
                       .Where(s => s.CardNumber == cardno).Single();
            }
        }

        public bool? SetAwaitingActivation(bool StoreCardCheckQual, string acctno)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var pay = ctx.StorecardPaymentDetails
                             .Where(s => s.acctno == acctno).Single();

                pay.Status = "AA";

                var cust = (from a in ctx.CustAcct
                            join c in ctx.Customer on a.custid equals c.custid
                            where a.acctno == acctno && a.hldorjnt == "H"
                            select c).Single();

                bool? approved = false;

                if (StoreCardCheckQual)
                {
                    var parms = ctx.View_StoreCardQualParams
                                 .Where(s => s.acctno == acctno).Single();

                    new StoreCardQualify().ExecuteDataSet(parms.custid, parms.points, parms.ScoreCardType, out approved, DateTime.Now);

                }

                if (approved.HasValue ? approved.Value : false)
                {
                    //cust.StoreCardApproved = true;                // set in SP
                    //cust.SCardApprovedDate = DateTime.Today;
                }

                ctx.SubmitChanges();
                return approved;
            });
        }


        public void SaveStoreCardBranchQualRules(StoreCardBranchQualRules rules)
        {
            rules.DateChanged = System.DateTime.Now;
            var storeCardQualRules = new StoreCardBranchQualRulesSave();
            EventStore.Instance.Log(rules, "StoreCardBranchQualRules", EventCategory.StoreCard, new { empeeno = rules.EmpeenoChange });


            storeCardQualRules.ExecuteNonQuery(rules.BranchNo,
                                       rules.MinApplicationScore,
                                       rules.MinBehaviouralScore,
                                       rules.MinMthsAcctHistX,
                                       rules.MinMthsAcctHistY,      //IP - 21/04/11 - Feature 3000
                                       rules.MaxCurrMthsInArrs,
                                       rules.MaxPrevMthsInArrsX,
                                       rules.MaxPrevMthsInArrsY,    //IP - 21/04/11 - Feature 3000
                                       rules.PcentInitRFLimit,
                                       rules.MaxNoCustForApproval,  //IP - 10/05/11 - Feature 3593
                                       rules.DateChanged,
                                       rules.EmpeenoChange,
                                       rules.ApplyTo
                                      );
        }


        public string CustomerIdFromStoreCard(Int64? cardno)
        {
            using (var ctx = Context.Create())
            {
                return (from s in ctx.StoreCard
                        join c in ctx.CustAcct on s.AcctNo equals c.acctno
                        where s.CardNumber == cardno.Value
                        select s.CustID).AnsiFirstOrDefault(ctx);
            }
        }

        //IP - 8/12/10 - Method to get Store Card Qualification Rules for a Branch
        public DataTable StoreCardBranchQualRulesGet(Int16 branchNo)
        {
            var storeCardQualRules = new StoreCardBranchQualRulesGet();

            DataSet storeCardQualRulesDS = storeCardQualRules.ExecuteDataSet(branchNo);  //IP - 26/04/11 - Modified to return a dataset

            DataTable storeCardQualRulesDt = new DataTable();

            storeCardQualRulesDt.Columns.Add(CN.MinApplicationScore);
            storeCardQualRulesDt.Columns.Add(CN.MinBehaviouralScore);
            storeCardQualRulesDt.Columns.Add(CN.MinMthsAcctHistX);      //IP - 26/04/11 - Feature 3000 - Renamed
            storeCardQualRulesDt.Columns.Add(CN.MinMthsAcctHistY);      //IP - 26/04/11 - Feature 3000 - Renamed
            storeCardQualRulesDt.Columns.Add(CN.MaxCurrMthsInArrs);
            storeCardQualRulesDt.Columns.Add(CN.MaxPrevMthsInArrsX);    //IP - 26/04/11 - Feature 3000 - Renamed
            storeCardQualRulesDt.Columns.Add(CN.MaxPrevMthsInArrsY);    //IP - 26/04/11 - Feature 3000 - Renamed
            storeCardQualRulesDt.Columns.Add(CN.PcentInitRFLimit);      //IP - 26/04/11 - Feature 3000 - Renamed
            storeCardQualRulesDt.Columns.Add(CN.MaxNoCustForApproval);  //IP - 10/05/11 - Feature 3593

            DataRow dr = storeCardQualRulesDt.NewRow();

            //IP - 26/04/11 - Feature 3000 - Set the values of the datatable
            foreach (DataRow row in storeCardQualRulesDS.Tables[0].Rows)
            {

                dr[CN.MinApplicationScore] = Convert.ToString(row[CN.MinApplicationScore]);
                dr[CN.MinBehaviouralScore] = Convert.ToString(row[CN.MinBehaviouralScore]);
                dr[CN.MinMthsAcctHistX] = Convert.ToString(row[CN.MinMthsAcctHistX]);
                dr[CN.MinMthsAcctHistY] = Convert.ToString(row[CN.MinMthsAcctHistY]);
                dr[CN.MaxCurrMthsInArrs] = Convert.ToString(row[CN.MaxCurrMthsInArrs]);
                dr[CN.MaxPrevMthsInArrsX] = Convert.ToString(row[CN.MaxPrevMthsInArrsX]);
                dr[CN.MaxPrevMthsInArrsY] = Convert.ToString(row[CN.MaxPrevMthsInArrsY]);
                dr[CN.PcentInitRFLimit] = Convert.ToString(row[CN.PcentInitRFLimit]);
                dr[CN.MaxNoCustForApproval] = Convert.ToString(row[CN.MaxNoCustForApproval]);   //IP - 10/05/11 - Feature 3593

                storeCardQualRulesDt.Rows.Add(dr);
            }
            return storeCardQualRulesDt;
        }

        public SCStatement GetStatements(int Id)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {

                var statement = (ctx.StoreCardStatement
                                .Where(s => s.Id == Id)).AnsiFirstOrDefault(ctx);

                //statement.DateFrom = statement.DateFrom.AddDays(1);           //IP - 28/03/12 - #9834 
                statement.DateTo = statement.DateTo.IncludeDay();

                if (statement == null)
                    throw new Exception("Statement not found");

                var paymentDetails = (from p in ctx.StorecardPaymentDetails
                                      where p.acctno == statement.Acctno
                                      select p).AnsiFirstOrDefault(ctx);

                var interestfreedays = CountryParameterCache.GetCountryParameter<Int32>(CountryParameterNames.SCardInterestFreeDays);

                var allFin = (ctx.view_FintransStoreCardStatements
                              .Where(f => f.AcctNo == statement.Acctno)).AnsiToList(ctx);

                return new SCStatement
                {
                    StoreCardStatement = statement,

                    PaymentDetails = paymentDetails,

                    CustAddress = (from ca in ctx.CustAddress
                                   where ca.custid == statement.CustID &&
                                   ca.addtype == "H" && ca.datemoved == null
                                   select ca).AnsiFirstOrDefault(ctx),

                    Customer = (ctx.Customer
                               .Where(c => c.custid == statement.CustID)).AnsiFirstOrDefault(ctx),

                    StoreCard = (ctx.StoreCard
                                .Where(s => s.AcctNo == statement.Acctno)).AnsiFirstOrDefault(ctx),

                    AllFin = allFin,

                    CountryMaintenance = (ctx.CountryMaintenance
                                          .Where(c => c.CodeName == "SCardInterestFreeDays" ||
                                                      c.CodeName == "decimalplaces" ||
                                                      c.CodeName == "StoreCardPaymentPercent" ||
                                                      c.CodeName == "StoreCardMinPayment")).AnsiToList(ctx),
                    InterestDue = CalculateInterestForAccount(statement.Acctno, statement.DateFrom, paymentDetails.InterestRate, ctx, connection, transaction, 
                                                              dateFrom: statement.DateFrom, dateTo: statement.DateTo)  // #9545 jec 31/01/12


                };
            });
        }

        //#12200 - CR11571
        public SCStatement GetStatementsBatchPrint(int batchNo)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {

                var statement = (ctx.StoreCardStatement
                                .Where(s => s.BatchNo == batchNo && s.RunNo == batchRunNo)).AnsiFirstOrDefault(ctx);

                if (!reprintBatch)
                {
                    statement.ManualDatePrinted = Convert.ToDateTime(this.batchStatementsDatePrinted);

                    ctx.SubmitChanges();
                }

                //statement.DateFrom = statement.DateFrom.AddDays(1);           //IP - 28/03/12 - #9834 
                statement.DateTo = statement.DateTo.IncludeDay();
               
                if (statement == null)
                    throw new Exception("Statement not found");

                var paymentDetails = (from p in ctx.StorecardPaymentDetails
                                      where p.acctno == statement.Acctno
                                      select p).AnsiFirstOrDefault(ctx);

                var interestfreedays = CountryParameterCache.GetCountryParameter<Int32>(CountryParameterNames.SCardInterestFreeDays);

                var allFin = (ctx.view_FintransStoreCardStatements
                              .Where(f => f.AcctNo == statement.Acctno)).AnsiToList(ctx);

                

                return new SCStatement
                {
                    StoreCardStatement = statement,

                    PaymentDetails = paymentDetails,

                    CustAddress = (from ca in ctx.CustAddress
                                   where ca.custid == statement.CustID &&
                                   ca.addtype == "H" && ca.datemoved == null
                                   select ca).AnsiFirstOrDefault(ctx),

                    Customer = (ctx.Customer
                               .Where(c => c.custid == statement.CustID)).AnsiFirstOrDefault(ctx),

                    StoreCard = (ctx.StoreCard
                                .Where(s => s.AcctNo == statement.Acctno)).AnsiFirstOrDefault(ctx),

                    AllFin = allFin,

                    CountryMaintenance = (ctx.CountryMaintenance
                                          .Where(c => c.CodeName == "SCardInterestFreeDays" ||
                                                      c.CodeName == "decimalplaces" ||
                                                      c.CodeName == "StoreCardPaymentPercent" ||
                                                      c.CodeName == "StoreCardMinPayment")).AnsiToList(ctx),
                    InterestDue = CalculateInterestForAccount(statement.Acctno, statement.DateFrom, paymentDetails.InterestRate, ctx, connection, transaction, dateFrom: statement.DateTo, dateTo: statement.DateTo)  // #9545 jec 31/01/12


                };

    

            });
        }
       

        public SCStatement GetStatementsEod(int Id, List<CountryMaintenance> country)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {

                var statement = (ctx.StoreCardStatement
                                .Where(s => s.Id == Id)).AnsiFirstOrDefault(ctx);

                statement.DateFrom = statement.DateFrom.AddDays(1);
                statement.DateTo = statement.DateTo.IncludeDay();

                //if (statement == null)
                //    throw new Exception("Statement not found");

                var paymentDetails = (from p in ctx.StorecardPaymentDetails
                                      where p.acctno == statement.Acctno
                                      select p).AnsiFirstOrDefault(ctx);

                var interestfreedays = CountryParameterCache.GetCountryParameter<Int32>(CountryParameterNames.SCardInterestFreeDays);

                var allFin = (ctx.view_FintransStoreCardStatements
                              .Where(f => f.AcctNo == statement.Acctno)).AnsiToList(ctx);

                return new SCStatement
                {
                    StoreCardStatement = statement,

                    PaymentDetails = paymentDetails,

                    CustAddress = (from ca in ctx.CustAddress
                                   where ca.custid == statement.CustID &&
                                   ca.addtype == "H"
                                   select ca).AnsiFirstOrDefault(ctx),

                    Customer = (ctx.Customer
                               .Where(c => c.custid == statement.CustID)).AnsiFirstOrDefault(ctx),

                    StoreCard = (ctx.StoreCard
                                .Where(s => s.AcctNo == statement.Acctno)).AnsiFirstOrDefault(ctx),

                    AllFin = allFin,

                    CountryMaintenance = country,

                    InterestDue = CalculateInterestForAccount(statement.Acctno, statement.DateFrom, paymentDetails.InterestRate, ctx, connection, transaction, 
                                                              dateFrom: statement.DateFrom, dateTo: statement.DateTo)    // #9545 jec 31/01/12
                };
            });
        }

        public SCAgreement GetAgreement(string acctno)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {

                var storecard = (ctx.View_StoreCardWithPayments
                                .Where(s => s.Acctno == acctno)).AnsiToList(ctx);

                if (storecard == null)
                    throw new Exception("Agreement not found");

                var custid = ctx.CustAcct
                             .Where(c => c.acctno == acctno && c.hldorjnt == "H")
                             .Select(c => c.custid)
                             .AnsiFirstOrDefault(ctx);

                var customer = ctx.Customer
                               .Where(c => c.custid == custid).AnsiFirstOrDefault(ctx);

                return new SCAgreement
                {
                    PaymentDetails = storecard,

                    CustAddress = (from ca in ctx.CustAddress
                                   where ca.custid == custid &&
                                   ca.addtype == "H" &&
                                   ca.datemoved == null
                                   select ca).AnsiFirstOrDefault(ctx),

                    Customer = customer,

                    CustTel = (ctx.CustTel
                              .Where(c => c.custid == custid &&
                                          c.datediscon == null)).AnsiToList(ctx),

                    Proposal = ctx.view_StoreCardGetProposal
                               .Where(p => p.custid == custid).AnsiFirstOrDefault(ctx),

                    Code = ctx.Code
                           .Where(c => c.category == "RS1" ||
                                       c.category == "MS1" ||
                                       c.category == "WT1").AnsiToList(ctx),

                    DateActive = (from c in ctx.StoreCard
                                  join s in ctx.StoreCardStatus on c.CardNumber equals s.CardNumber
                                  where c.AcctNo == acctno &&
                                  StoreCardCardStatus_Lookup.Active.Code == s.StatusCode
                                  orderby s.DateChanged descending
                                  select s.DateChanged).AnsiFirstOrDefault(ctx),

                    CountryMaintenance = (ctx.CountryMaintenance
                                           ).AnsiToList(ctx),

                    AccountStatus = CalculateAccountStatus(ctx, cust: customer, acctno: acctno),

                    employment = (ctx.View_employment
                                  .Where(e => e.custid == custid)
                                  .OrderByDescending(e => e.dateemployed)).AnsiFirstOrDefault(ctx)
                };
            });
        }

        public DataSet EODQualify(SqlConnection conn, SqlTransaction trans, DateTime rundate)
        {
            bool? ignore;

            return new Blue.Cosacs.StoreCardQualify(conn, trans) { CommandTimeout = 0 }
                .ExecuteDataSet(string.Empty, (int?)0, string.Empty, out ignore, rundate);
        }

        public DataSet EODStatements(SqlConnection conn, SqlTransaction trans, DateTime rundate)
        {

            var Statements = new Blue.Cosacs.SCardEodLoadforStatements(conn, trans);

            Statements.CommandTimeout = DALObject.CommandTimeout;
            return Statements.ExecuteDataSet((rundate));

        }

        //
        //var applyRules = new Blue.Cosacs.StoreCardQualify(conn, trans) { CommandTimeout = 0 }; //IP - 06/05/11 - Added CommandTimeout to make timeout limitless
        //bool? ignore = false; int? points = 0; // these parameters will run procedure for all accounts
        //var ds = applyRules.ExecuteDataSet(string.Empty, points, string.Empty, out ignore, rundate);

        //var issueCard = Convert.ToBoolean(Country[CountryParameterNames.StoreCardIssueCardPreAppr]); //IP - 09/05/11 - Store Card - Feature #3004

        ////IP - Store Card - Feature #3004- 05/05/11
        //if (ds.Tables[0].Rows.Count > 0 && issueCard) //If Country Parameter to issue card at preapproval is true
        //{
        //    //StoreCardRepository storeCard = new StoreCardRepository();


        //    DataTable dt = CreateStoreCardAcct(conn, trans, ds.Tables[0]); //Create the customers account for the Store Card

        //}
        //}}

        private class InterestReferenceData
        {
            public StorecardPaymentDetails PaymentDetails { get; set; }
            public Customer Customer { get; set; }
            public Acct Account { get; set; }
        }

        private class InterestException : ApplicationException
        {
            public InterestException(Exception ex) : base("InterestException", ex) { }
            public InterestReferenceData InterestData { get; set; }
        }

        public void InterestAccountsLoad(SqlConnection conn, SqlTransaction trans, DateTime rundate, int? commandTimeout) //IP - 16/01/13 - Merged from CoSACS 6.5
        {
            var exs = new List<InterestException>();
            var intFreePeriod = CountryParameterCache.GetCountryParameter<Int32>(CountryParameterNames.SCardInterestFreeDays);
            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                //IP - 16/01/13 - Merged from CoSACS 6.5
                if (commandTimeout.HasValue)
                    ctx.CommandTimeout = commandTimeout.Value;

                var spd = (from pd in ctx.StorecardPaymentDetails
                           join ca in ctx.CustAcct on pd.acctno equals ca.acctno
                           join c in ctx.Customer on ca.custid equals c.custid
                           join a in ctx.Acct on ca.acctno equals a.acctno
                           where ca.hldorjnt == "H" 
                              && pd.DatePaymentDue < rundate
                              && pd.DatePaymentDue != null
                           // && x.DatePaymentDue > DateTime.MinValue && x.acctno == "700901124241" for testing...
                           select new InterestReferenceData { PaymentDetails = pd, Customer = c, Account = a }
                           ).ToList(); // .AnsiToList(ctx)

                Parallel.ForEach(spd, (i) =>
                    {
                        try
                        {
                            InterestAccount(connection, transaction, /*ctx,*/ rundate, intFreePeriod, i, commandTimeout); //IP - 16/01/13 - Merged from 
                        }
                        catch (Exception ex)
                        {
                            exs.Add(new InterestException(ex) { InterestData = i });
                        }
                    });

                if (exs.Count > 0)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Exception calculating interest:");

                    foreach (var ex in exs)
                        sb.AppendFormat("Account: {0} Exception: {1} = {2}\nStack Trace: {3}\n",
                            ex.InterestData.Account.acctno, ex.InnerException.Message, ex.InnerException.GetType().FullName, ex.InnerException.StackTrace);
                    throw new ApplicationException(sb.ToString());
                }
                //foreach (var i in spd)
                //    InterestAccount(connection, transaction, ctx, rundate, intFreePeriod, i);

                ctx.SubmitChanges(); // UPDATE PaymentDetails and anything else

            }, null, conn, trans);
        }


        private void InterestAccount(SqlConnection c, SqlTransaction t, /*Context ctx,*/ DateTime rundate, int intFreePeriod, InterestReferenceData i, int? commandTimeout = null) //IP - 16/01/13 - Merged from CoSACS 6.5
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                //IP - 16/01/13 - Merged from CoSACS 6.5
                if (commandTimeout.HasValue)
                    ctx.CommandTimeout = commandTimeout.Value;

                var pd = i.PaymentDetails;
                if (pd.LastInterestDate == null)
                    throw new Exception("Last Interest Date Should never be null.");
                var interest = CalculateInterestForAccount(pd.acctno,
                                    dateZeroBal: pd.DatePaymentDue.Value.IncludeDay(),
                                    interestRate: pd.InterestRate, context: ctx, conn: connection, trans: transaction, timeout: commandTimeout, updateInt: true); //IP - 16/01/13 - Merged from CoSACS 6.5

                if (interest > 0)
                {
                    // 1 INSERT + 1 UPDATE customer
                    AddTransaction(pd.acctno, interest, pd.DatePaymentDue.Value.AddDays(-intFreePeriod + 1),
                        TransType.Interest, i.Customer.custid, conn: connection, trans: transaction, context: ctx, timeout: commandTimeout); //IP - 16/01/13 - Merged from CoSACS 6.5
                }
               
                BlockStoreCardAccountFeeCheck(connection, transaction, rundate, pd.acctno, false,
                minimumPayment: pd.MinimumPayment, context: ctx, paymentDetails: pd, cust: i.Customer);

                pd.LastInterestDate = NotLastDay(pd.LastInterestDate.Value.AddMonths(1));
                pd.DatePaymentDue = NotLastDay(pd.DatePaymentDue.Value.AddMonths(1));

                ctx.SubmitChanges();
            }, null, c, t);
        }

        private DateTime NotLastDay(DateTime DateCheck)
        {
            if (DateCheck.AddDays(1).Day == 1)
                return DateCheck.AddDays(1);

            return DateCheck;
        }

        /// <summary>
        /// Will determine if store card has had a minimum payment in last x months. If not then will block account 
        /// </summary>
        public void BlockStoreCardAccountFeeCheck(SqlConnection conn, SqlTransaction trans, DateTime rundate, string acctno, bool payment,
                            decimal? minimumPayment = null, Context context = null, StorecardPaymentDetails paymentDetails = null,
                            Customer cust = null)
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                if (cust == null)
                {
                    cust = (from ca in ctx.CustAcct
                            join c in ctx.Customer on ca.custid equals c.custid
                            where ca.acctno == acctno
                               && ca.hldorjnt == "H"
                            select c).AnsiFirstOrDefault(ctx);
                }

                if (!payment || cust.creditblocked == 1) // only checking further for performance reasons 
                {
                    var blockcreditmonths = CountryParameterCache.GetCountryParameter<decimal>(CountryParameterNames.blockcreditmonths);
                    var creditArrearsAccount = (from a in ctx.Acct
                                                join ca in ctx.CustAcct on a.acctno equals ca.acctno
                                                join ip in ctx.Instalplan on ca.acctno equals ip.acctno
                                                where ca.custid == cust.custid
                                                   && a.arrears >= blockcreditmonths * ip.instalamount
                                                   && a.outstbal >= a.arrears
                                                   && ca.hldorjnt == "H"
                                                   && ip.instalamount > 0
                                                   && a.accttype != "T" //exclude store card accounts for now
                                                   && a.outstbal > 0
                                                select a).AnsiFirstOrDefault(ctx);

                    var transactions = (from f in ctx.fintranswithBalancesVW
                                        where f.acctno == acctno // && x.datetrans >= rundate.AddMonths(-2) 
                                           && f.datetrans <= rundate
                                        orderby f.datetrans
                                        select f).AnsiToList(ctx);
                    //  CountryParameterCache.GetCountryParameter<decimal>(CountryParameterNames.StoreCardPaymentPercent); 

                    if (paymentDetails == null)
                        paymentDetails = (from a in ctx.StorecardPaymentDetails
                                          where a.acctno == acctno
                                          select a).AnsiFirstOrDefault(ctx);

                    var minimumpaid = false;

                    // next statement doesn't hit the database
                    var block = DetermineBlockAndMinPayment(creditArrearsAccount, transactions, rundate, ref minimumpaid,
                                    agreedMinimumPayment: minimumPayment, details: paymentDetails);

                    if (block.HasValue)
                    {
                        var cmd = new SqlCommand("UPDATE Customer SET creditblocked = @blocked WHERE custid = @custId", connection, transaction);
                        cmd.Parameters.Add("@custId", SqlDbType.VarChar).Value = cust.custid;
                        cmd.Parameters.Add("@blocked", SqlDbType.TinyInt).Value = block.Value ? 1 : 0;
                        cmd.ExecuteNonQuery(); // save customer credit blocked/unblocked
                    }

                    if (!minimumpaid)
                    {
                        var fee = CountryParameterCache.GetCountryParameter<double>(CountryParameterNames.SCLatePaymentFees);
                        if (fee > 0)
                            // 1 INSERT + 1 UPDATE customer + 1 UPDATE account
                            AddTransaction(acctno, fee, rundate, TransType.AdminCharge, cust.custid, conn: connection, trans: transaction, context: ctx);
                    }
                }
                //if (payinlastmonth)
            }, context, conn, trans);
        }

        public StoreCardUpdateMinimumPaymentResponse UpdateMinimumPayment(StoreCardUpdateMinimumPaymentRequest Request, SqlConnection conn = null, SqlTransaction trans = null, Context context = null)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
             {
                 var payment = ctx.StorecardPaymentDetails
                               .Where(p => p.acctno == Request.AcctNo).AnsiFirstOrDefault(ctx);

                 var IntFreePeriod = CountryParameterCache.GetCountryParameter<Int32>(CountryParameterNames.SCardInterestFreeDays);

                 //using this to get average balance
                 CalculateInterestForAccount(payment.acctno, payment.LastInterestDate.Value.AddDays(1), payment.InterestRate, ctx, connection, transaction);      // #9545 jec 31/01/12

                 var countryMinPay = Convert.ToDecimal(Averagebalance) * CountryParameterCache.GetCountryParameter<decimal>(CountryParameterNames.StoreCardPaymentPercent) / 100;
                 countryMinPay = Math.Round(countryMinPay, 0);
                 if (countryMinPay < CtyAbsoluteMinPayment)
                     countryMinPay = CtyAbsoluteMinPayment;

                 if (countryMinPay > Request.MinimumPayment && countryMinPay < Convert.ToDecimal(Averagebalance))
                     //payment.MinimumPayment = countryMinPay;
                     payment.MonthlyAmount = countryMinPay;         // #9841 jec 27/03/12 set fixed payment
                 else
                     //payment.MinimumPayment = Request.MinimumPayment;
                     payment.MonthlyAmount = Request.MonthlyAmount;         // #9841 jec 27/03/12 set fixed payment

                 ctx.SubmitChanges();

                 return new StoreCardUpdateMinimumPaymentResponse { calcMinPayment = payment.MinimumPayment };

             }, context, conn, trans);
        }
        public bool? DetermineBlock(Acct Account, IList<fintranswithBalancesVW> Transactions, DateTime rundate,
                                    StorecardPaymentDetails details)
        {
            bool minimumpaid = false;
            return DetermineBlockAndMinPayment(Account, Transactions, rundate,
                ref minimumpaid, details, agreedMinimumPayment: 0.0m);
        }


        public bool? DetermineBlockAndMinPayment(Acct creditArrearsAccount,
            IList<fintranswithBalancesVW> transactions, DateTime rundate, ref bool minimumpaid,
            StorecardPaymentDetails details, decimal? agreedMinimumPayment = null) // , Acct StoreAcct = null)
        {
            decimal balanceatEndofMonth = 0.0m,
                    payvalue = 0.0m,
                    previousbalance = 0.0m,
                    payinlastmonth = 0.0m,
                    minPay = 0.0m,
                    receipts = 0.0m;

            if (details != null)
            {
                var endsaledate = Convert.ToDateTime(details.LastInterestDate); // rundate.AddMonths(-1).AddDays(-IntFreeDays-2);
                var startdate = Convert.ToDateTime(details.LastInterestDate).AddMonths(-1);
                if (transactions != null)
                {
                    receipts = transactions.Where(f => f.datetrans > rundate.AddMonths(-3)
                                                   && (f.transtypecode == TransType.Payment
                                                    || f.transtypecode == TransType.Refund
                                                    || f.transtypecode == TransType.Correction)
                    ).Sum(f => f.transvalue);

                    foreach (var fin in transactions)
                    {
                        if (fin.datetrans < startdate)
                            previousbalance = Convert.ToDecimal(fin.total);
                        if (fin.datetrans < endsaledate)
                            balanceatEndofMonth = Convert.ToDecimal(fin.total);
                        if (fin.datetrans >= startdate)
                        {
                            if (minPay == 0)
                            {
                                //previousbalance = Convert.ToDecimal(fin.total) - Convert.ToDecimal(fin.transvalue);

                                minPay = CountryParameterCache.GetCountryParameter<decimal>(CountryParameterNames.StoreCardPaymentPercent) / 100 * previousbalance;

                                if (CtyAbsoluteMinPayment > minPay)
                                    minPay = CtyAbsoluteMinPayment;

                                if (Convert.ToDecimal(agreedMinimumPayment) > minPay)
                                    minPay = Convert.ToDecimal(agreedMinimumPayment);
                            }

                        }
                        if (fin.transtypecode == TransType.Payment
                         || fin.transtypecode == TransType.Correction
                         || fin.transtypecode == TransType.Refund)
                        {
                            payvalue = payvalue + fin.transvalue;
                            if (fin.datetrans > startdate)
                            {

                                payinlastmonth = payinlastmonth + fin.transvalue;
                                if (-payinlastmonth >= minPay
                                 || balanceatEndofMonth == 0
                                 || balanceatEndofMonth < CtySmallbalance)
                                    minimumpaid = true;
                                else
                                    minimumpaid = false;
                            }
                        }
                    }
                }
            }
            if (balanceatEndofMonth == 0 || balanceatEndofMonth < CtySmallbalance)
                minimumpaid = true;

            //receipts will be negative if payments
            if ((payvalue >= 0 && previousbalance > 0 && receipts >= 0) || creditArrearsAccount != null)
                return true; // "Block";

            if (minimumpaid && creditArrearsAccount == null)
                return false; // "Unblock";

            return null;
        }

        public double CalculateInterestForAccount(string acctno, DateTime dateZeroBal, double interestRate,
                                                  Context context = null, SqlConnection conn = null, SqlTransaction trans = null, int? timeout = null, DateTime? dateFrom = null, DateTime? dateTo = null, bool updateInt = false ) //IP - 16/01/13 - Merged from CoSACS 6/5
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                //IP - 16/01/13 - Merged from CoSACS 6.5
                if (timeout.HasValue)
                    ctx.CommandTimeout = timeout.Value;

                var transactions = (from f in ctx.fintranswithBalancesVW
                                    where f.acctno == acctno
                                       && f.datetrans <= dateZeroBal
                                    orderby f.datetrans
                                    select f).AnsiToList(ctx);

                if (!dateFrom.HasValue || !dateTo.HasValue)
                {
                    var statements = (from s in ctx.StoreCardStatement
                                      where s.Acctno == acctno &&
                                      s.InterestCalculatedDate == null
                                      select s).ToList();
                    if (statements.Count() > 0)
                    {
                        dateFrom = statements.Min(s => s.DateFrom);
                        dateTo = statements.Max(s => s.DateTo);
                    }
                    if (updateInt)
                    {
                        statements.ForEach(s =>
                        {
                            s.InterestCalculatedDate = dateZeroBal;
                        });
                        ctx.SubmitChanges();
                    }

                }

                if (transactions == null || transactions.Count == 0 || !dateFrom.HasValue || !dateTo.HasValue)
                    return 0;

                // 0 SELECTS: next statement does not hit the database
                Averagebalance = StoreCardWeighting.WeightedAverage(transactions, dateFrom.Value, dateTo.Value);
                if (Averagebalance <= 0)
                    return 0;

                var closingbalance = transactions.Last().total;
                if (closingbalance <= 0)
                    return 0;

                // 0 SELECTS: next statement does not hit the database
                return Math.Round(StoreCardInterest.GetInterest(interestRate, Averagebalance, dateFrom.Value, dateTo.Value), decimalPlaces);

            }, context, conn, trans);
        }


        public double GetInterest(string acctno)
        {
            using (var ctx = Context.Create())
            {
                return ctx.StorecardPaymentDetails
                        .Where(p => p.acctno == acctno)
                        .Select(p => p.InterestRate).AnsiFirstOrDefault(ctx);

            }
        }

        public void AddTransaction(string acctno, double value, DateTime transactionDate, string transtypeCode, string custId, short? branchno = null, /*Customer cust = null,*/ SqlConnection conn = null, SqlTransaction trans = null, Context context = null, int? timeout = null) //IP - 16/01/13 - Merged from CoSACS 6.5 //, Acct account = null)
        {
            if (custId == null) // && cust == null)
                throw new Exception("Add transaction in storecard repository used incorrectly.");

            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                //IP - 16/01/13 - Merged from CoSACS 6.5
                if (timeout.HasValue)
                    ctx.CommandTimeout = timeout.Value;

                if (branchno == null)
                    branchno = CountryParameterCache.GetCountryParameter<short>(CountryParameterNames.HOBranchNo);

                var branch = new DBranch();
                var transrefno = branch.GetTransRefNo(conn, trans, Convert.ToInt16(branchno));

                if (value == 0)
                    return;

                /*if (account == null)
                    account = (from a in ctx.Acct
                               where a.acctno == acctno
                               select a).AnsiFirstOrDefault(ctx);*/
                //account.outstbal += Convert.ToDecimal(value);

                /*if (cust == null)
                    cust = ctx.Customer.Where(c => c.custid == custId).AnsiFirstOrDefault(ctx);*/

                //var storeCardAvailable = Math.Max(cust.StoreCardAvailable.Value - Convert.ToDecimal(value), 0.0m);
                //var availableSpend = Math.Max(cust.AvailableSpend - Convert.ToDecimal(value), 0.0m);

                var cmd = new SqlCommand(
                @"UPDATE Acct 
                SET outstbal = outstbal + @value 
                WHERE acctno = @acctno;
                                 
                UPDATE Customer 
                SET StoreCardAvailable = CASE WHEN StoreCardAvailable - @value < 0 THEN 0 ELSE StoreCardAvailable - @value END,
                    AvailableSpend     = CASE WHEN AvailableSpend     - @value < 0 THEN 0 ELSE AvailableSpend     - @value END
                WHERE CustID = @custId", connection, transaction);
                cmd.Parameters.Add("@value", SqlDbType.Decimal).Value = value;
                cmd.Parameters.Add("@acctno", SqlDbType.VarChar).Value = acctno;
                cmd.Parameters.Add("@custId", SqlDbType.VarChar).Value = custId;
                cmd.ExecuteNonQuery();

                new DFinTrans
                    {
                        AccountNumber = acctno,
                        TransRefNo = transrefno,
                        DateTrans = transactionDate,
                        TransTypeCode = transtypeCode,
                        TransValue = Convert.ToDecimal(value),
                        EmployeeNumber = 99999,
                        TransPrinted = "N",
                        TransUpdated = "N",
                        FTNotes = "NETI",
                        Source = "COSACS",
                        PayMethod = 0,
                        RunNumber = 0,
                        Agrmtno = 1,
                        BranchNumber = Convert.ToInt16(branchno)
                    }.Write(connection, transaction);

                // ctx.SubmitChanges();
            }, context, conn, trans);
        }

        public void RecalculateAvailable(long cardno, SqlConnection conn = null, SqlTransaction trans = null, Context context = null)
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var custid = (from s in ctx.StoreCard
                              join ca in ctx.CustAcct on s.AcctNo equals ca.acctno
                              where s.CardNumber == cardno &&
                              ca.hldorjnt == "H"
                              select ca.custid).AnsiFirstOrDefault(ctx);

                RecalculateAvailable(custid, connection, transaction, ctx);
                if (context == null)
                    ctx.SubmitChanges();
            }, context, conn, trans);
        }

        public void RecalculateAvailable(string custid, SqlConnection conn = null, SqlTransaction trans = null, Context context = null)
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
           {
               var storecardcredit = StoreCardAvailGet(custid, ctx, connection, transaction);
               storecardcredit.UpdateStoreCardAmount();
               if (context == null)
                   ctx.SubmitChanges();
           }, context, conn, trans);
        }

        public StoreCardCalcCustomer GetCalc(string acctno)
        {
            using (var ctx = Context.Create())
            {
                return new StoreCardCalcCustomer
                {

                    CustAddress = (from cadd in ctx.CustAddress
                                   join ca in ctx.CustAcct on cadd.custid equals ca.custid
                                   where ca.acctno == acctno &&
                                   ca.hldorjnt == "H" &&
                                   cadd.datemoved == null &&
                                   cadd.addtype == "H"
                                   select cadd).AnsiFirstOrDefault(ctx),

                    Customer = (from cust in ctx.Customer
                                join ca in ctx.CustAcct on cust.custid equals ca.custid
                                where ca.acctno == acctno &&
                                ca.hldorjnt == "H"
                                select cust).AnsiFirstOrDefault(ctx)
                };
            }
        }

        public StoreCardNewCardAgreement GetNewCardAgreement(string acctno, string newcustid)
        {
            using (var ctx = Context.Create())
            {
                return new StoreCardNewCardAgreement
                {
                    CustAddress = (from cadd in ctx.CustAddress
                                   join ca in ctx.CustAcct on cadd.custid equals ca.custid
                                   where ca.acctno == acctno &&
                                   ca.hldorjnt == "H" &&
                                   cadd.datemoved == null &&
                                   cadd.addtype == "H"
                                   select cadd).AnsiFirstOrDefault(ctx),

                    Customer = (from cust in ctx.Customer
                                join ca in ctx.CustAcct on cust.custid equals ca.custid
                                where ca.acctno == acctno &&
                                ca.hldorjnt == "H"
                                select cust).AnsiFirstOrDefault(ctx),

                    NewCustomer = (ctx.Customer
                                     .Where(c => c.custid == newcustid)).AnsiFirstOrDefault(ctx),

                    NewCustAddress = (ctx.CustAddress
                                      .Where(c => c.addtype == "H" && c.datemoved == null && c.custid == newcustid)).AnsiFirstOrDefault(ctx)
                };
            }
        }


        public void StoreCardStatementEodToCSV(DateTime rundate, SqlConnection conn, SqlTransaction trans, string filePath)
        {
            string comma = ",";
            var sb = new StringBuilder();

            string header = "Acctno" + comma + "custid" +
                             comma + "title" + comma + "firstname" + comma +
                             "name" + comma + "cusaddr1" + comma +
                             "cusaddr2" + comma + "cusaddr3" + comma +
                             "cuspocode" + comma +
                             "DateFrom" + comma +
                             "DateTo" + comma +
                             "DatePrinted" + comma +
                             "StoreCardLimit" + comma +
                             "StoreCardAvailable" + comma +
                             "InterestRate" + comma +
                             "OpenBalance" + comma +
                             "Purchases" + comma +
                             "Fees" + comma +
                             "InterestDue" + comma +
                             "Payments" + comma +
                             "EndBalance" + comma +
                             "OverDue" + comma +            // #10082
                             "MinPayment" + comma +
                             "PendingInterest" + comma +
                             "DueDate" + comma +
                       "TransactionDate " + comma + "Transaction Description" + comma + "MoneyOut"
                       + comma + "MoneyIn" + comma + "Balance" + comma + "BranchNo"
                        + comma + "BranchName" + comma + "CardNumber" + comma +
                        "AccountNumber" + comma +
                        "InvoiceNumber";

            sb.AppendLine(header);

            using (var ctx = Context.Create(conn, trans))
            {
                var scview = (from sc in ctx.StoreCardStatements_View
                              where sc.DatePrinted == rundate
                              orderby sc.Acctno
                              select sc).AnsiToList(ctx);

                var Countrym = (ctx.CountryMaintenance
                                         .Where(c => c.CodeName == "SCardInterestFreeDays" ||
                                                     c.CodeName == "decimalplaces" ||
                                                     c.CodeName == "StoreCardPaymentPercent" ||
                                                     c.CodeName == "StoreCardMinPayment")).AnsiToList(ctx);
                var PreviousAcctno = ""; //decimal payments = 0.0m; decimal fees = 0.0m; decimal interest = 0.0m;
                var scPreviousline = new StoreCardStatements_View();
                //decimal amountdue = 0.0m; decimal minimumPayment = 0.0m; 
                double pendinginterest = 0.0f;
                foreach (var scline in scview)
                {
                    scPreviousline = scline;
                    //write csv file and reset

                    //payments = 0.0m;
                    //  fees = 0.0m;
                    // interest = 0.0m;
                    //    amountdue = 0.0m;
                    //  minimumPayment = 0.0m;
                    pendinginterest = CalculateInterestForAccount(scline.Acctno, scline.DateFrom, scline.InterestRate, ctx, conn, trans,dateFrom: scline.DateFrom, dateTo: scline.DateTo);         // #9545 jec 31/01/12

                    PreviousAcctno = scline.Acctno;

                    var allFin = (ctx.view_FintransStoreCardStatements
                          .Where(f => f.AcctNo == scline.Acctno)).AnsiToList(ctx);


                    //scline.DateFrom = scline.DateFrom.AddDays(1);
                    scline.DateTo = scline.DateTo.IncludeDay();

                    var sc = new SCStatement
                    {
                        //StoreCardStatement = new StoreCardStatement { DateFrom = scline.DateFrom, DateTo = scline.DateTo },
                        StoreCardStatement = (from s in ctx.StoreCardStatement              // #10081
                                              where s.DatePrinted == rundate && s.Acctno == scline.Acctno
                                              select s).AnsiFirstOrDefault(ctx),

                        //PaymentDetails = new StorecardPaymentDetails { acctno = scline.Acctno, InterestRate = scline.InterestRate, DatePaymentDue = scline.DatePaymentDue },
                        PaymentDetails = (from p in ctx.StorecardPaymentDetails             // #10081
                                      where p.acctno == scline.Acctno
                                      select p).AnsiFirstOrDefault(ctx),

                        //CustAddress = new CustAddress { cusaddr1 = scline.cusaddr1, cusaddr2 = scline.cusaddr2, cusaddr3 = scline.cusaddr3, cuspocode = scline.cuspocode, addtype = "H" },

                        //Customer = new Customer { custid = scline.custid, name = scline.name, firstname = scline.firstname, title = scline.title },

                        //StoreCard = new StoreCard { AcctNo = scline.Acctno, },
                        StoreCard = (from st in ctx.StoreCard                               // #10081
                                     where st.AcctNo == scline.Acctno
                                     select st).AnsiFirstOrDefault(ctx),

                        AllFin = allFin,

                        CountryMaintenance = Countrym,
                        InterestDue = pendinginterest    // #9545 jec 31/01/12
                    };

                    //var fintrans = allFin.Where(d => d.DateTrans >= scline.DateFrom &&
                    //                 d.DateTrans < scline.DateTo ).ToList();
                    #region writelineforeachaccount
                    string line = scline.Acctno + comma + scline.custid.Replace(",", "") +
                             comma + scline.title.Replace(",", "") + comma + scline.firstname.Replace(",", "") + comma +
                             scline.name.Replace(",", "") + comma + scline.cusaddr1.Replace(",", "") + comma +
                             scline.cusaddr2.Replace(",", "") + comma + scline.cusaddr3.Replace(",", "") + comma +
                             scline.cuspocode.Replace(",", "") + comma +
                             Convert.ToString(scline.DateFrom) + comma +
                             Convert.ToString(scline.DateTo) + comma +
                             Convert.ToString(scline.DatePrinted) + comma +
                        //Convert.ToString(scline.Acctno) + comma +
                             Convert.ToString(scline.StoreCardLimit) + comma +
                             Convert.ToString(scline.StoreCardAvailable) + comma +
                             Convert.ToString(scline.InterestRate) + comma +
                             Convert.ToString(sc.OpenBalance) + comma +
                             Convert.ToString(sc.Purchases) + comma +
                             Convert.ToString(sc.Fees) + comma +
                             Convert.ToString(sc.Interest) + comma +
                             Convert.ToString(sc.Payments) + comma +
                             Convert.ToString(sc.EndBalance) + comma +
                             Convert.ToString(sc.OverDue) + comma +           // #10082 jec 24/05/12
                             Convert.ToString(sc.MinAmount) + comma +           // #8445 jec 23/01/12

                             //RM #10232 - Statement Minimum payment is already calculated in the SP: SCardEodLoadforStatements

                        //Convert.ToString(scline.PrevStmtMinPayment + sc.Payments < 0 ? 0 + sc.MinAmount : 
                        //                        scline.PrevStmtMinPayment + sc.Payments + sc.MinAmount > sc.EndBalance ? sc.EndBalance :  // #10120 jec 21/05/12 set minpay= account balance
                        //                        scline.PrevStmtMinPayment + sc.Payments + sc.MinAmount) + comma +           // #8445 jec 30/01/12 include any unpaid amount from previous statement
                             Convert.ToString(sc.InterestDue) + comma +
                             Convert.ToString(sc.DueDate) + comma;
                    #endregion
                    bool foundtransactions = false;
                    string finline = "";
                    var Statementfin = sc.StatementFin.OrderBy(d => d.DateTrans);
                    foreach (var fin in Statementfin)
                    {
                        foundtransactions = true;
                        finline =
                        Convert.ToString(fin.DateTrans) + comma +
                        Convert.ToString(sc.ConvertCode(fin.Code));         // #10082
                        if (fin.Code == TransType.StoreCardPayment)         // #10082
                        {
                            finline = finline + " x-" + Convert.ToString(fin.CardNumber).Substring(Convert.ToString(fin.CardNumber).Length - 4);
                            finline = finline + " at " + Convert.ToString(fin.branchname);
                        }
                        finline = finline + comma;              // #10082

                        if (fin.Value > 0)
                        {
                            finline = finline + Convert.ToString(fin.Value) + comma + "" + comma;
                        }

                        if (fin.Value < 0)
                            finline = finline + "" + comma + (Math.Abs(fin.Value)) + comma;

                        finline = finline + Convert.ToString(fin.RunningTotal) + comma;
                        finline = finline + Convert.ToString(fin.BranchNo) + comma;
                        finline = finline + fin.branchname + comma;
                        var cardNumber = Convert.ToString(scline.CardNumber);
                        if (cardNumber.Length > 0)
                            finline = finline + cardNumber + comma;     //.Substring(cardNumber.Length - 4) + comma;
                        else
                            finline = finline + comma;

                        //if (fin.agrmtno == 1) // if cash account use acctno- else use agrmtno
                        finline = finline + fin.TransferAccount;         // #10082 jec 24/05/12
                        finline = finline + comma;                           // #10082 jec 24/05/12
                        //else
                        finline = finline + Convert.ToString(fin.agrmtno);    // #10082 jec 24/05/12
                        //finline = finline + fin.TransferAccount + "/" + Convert.ToString(fin.agrmtno);      // #10082 output acctno & agrmtno in same column

                        finline = finline + comma;

                        sb.AppendLine(line + finline);

                    }

                    if (!foundtransactions)
                    {


                        var Acct = (ctx.Acct
                              .Where(f => f.acctno == scline.Acctno)).AnsiFirstOrDefault(ctx);

                        line = line + "" + comma + "" + comma + "" + comma + ""
                       + comma + Convert.ToString(Acct.outstbal) + comma + "" + comma + "" + comma + Convert.ToString(scline.CardNumber) +      // #9983 jec 30/04/12
                        comma + "" + comma + "" + comma;        // #10082 jec 24/05/12


                        sb.AppendLine(line);
                    }
                    line = "";
                }

                System.IO.File.WriteAllText(filePath, sb.ToString());

            }
        }

        public decimal? GetMinimumPayment(string acctno)
        {
            using (var ctx = Context.Create())
            {
                return ctx.StorecardPaymentDetails
                        .Where(p => p.acctno == acctno)
                        .Select(p => p.MonthlyAmount).AnsiFirstOrDefault(ctx);

            }
        }

        //CR11571 - #12200
        public DataSet GetStatementRuns()
        {
            using (var ctx = Context.Create())
            {
                return new StoreCardGetStatementRuns().ExecuteDataSet();
            }

        }

        //#12424
        public DateTime? GetStatementDatePrinted(int statementNo, int runno)
        {
            using (var ctx = Context.Create())
            {
                return ctx.StoreCardStatement
                    .Where(s => s.RunNo == runno & s.BatchNo == statementNo)
                    .Select(s => s.ManualDatePrinted).AnsiFirstOrDefault(ctx);
            }

        }


        /// <summary>
        /// Takes the transactions on the account and determines whether the customer has paid off in time. If not then will calculate the average balance on the statement date. 
        /// </summary>
        /// <param name="fintransList"></param>
        /// <param name="DateFrom"></param>
        /// <param name="DateTo"></param>
        /// <param name="StatementDate"></param>
        /// <returns></returns>

        public class StoreCardActivationException : Exception
        {
            public StoreCardActivationException(string message) : base(message) { }
        }

        
    }
}
