using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Blue.Cosacs.Messages.Warehouse;
using Blue.Cosacs.Shared;
using STL.Common;
using STL.Common.Constants;
using STL.Common.Constants.Categories;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.InstantCredit;
using STL.Common.Constants.TableNames;
using STL.DAL;
using Blue.Hub.Client;
using System.Xml;
using STL.BLL;
using Blue.Cosacs.Shared.Extensions;
namespace Blue.Cosacs.Repositories
{
    public class AccountRepository
    {

        public string bookingType { get; set; }

        private class SettledLength
        {
            public string acctno { get; set; }
            public DateTime DateDel { get; set; }
            public DateTime DateSettled { get; set; }
        }
        private class DatedelClass
        {
            public DateTime DateDel { get; set; }
        }

        private class SettledAccts
        {
            public string acctno { get; set; }
            public DateTime Datedel { get; set; }
            public DateTime datesetttled { get; set; }
        }

        private CommonObject commonObject = new CommonObject();

        public CountryParameterCollection Country
        {
            get { return commonObject.Country; }
        }

        public void UpdateOutstandingBalance(string acctNo)
        {
            new AccountUpdateOutstandingBalance(acctNo).Execute();
        }
        public static int monthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }

        public Acct Get(Acct.Parameters.Load parameter, int empeeNo, SqlConnection connection, SqlTransaction trans) //IP - 03/03/11 - #3255 - Added empeeno
        {
            if (connection == null)
                connection = new SqlConnection(Connections.Default);

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            if (trans == null)
                trans = connection.BeginTransaction();

            using (var ctx = Context.Create(connection))
            {
                ctx.Transaction = trans;
                ctx.ObjectTrackingEnabled = false;
                var account = new Acct();
               
                if (parameter.AcctGet)
                {
                    account = (from a in ctx.Acct
                               where a.acctno == parameter.AcctNo
                               select a).AnsiFirstOrDefault(ctx);
                }

                if (parameter.AgreementGet)
                {
                    var record = (from p in ctx.Agreement
                                  where p.acctno == parameter.AcctNo
                                  select p).AnsiFirstOrDefault(ctx);

                    account.Agreement = record ?? new Agreement();
                }

                if (parameter.InstalPlanGet)
                {
                    var record = (from p in ctx.Instalplan
                                  where p.acctno == parameter.AcctNo
                                  select p).AnsiFirstOrDefault(ctx);

                    account.InstalPlan = record ?? new Instalplan();
                    CalculateFirstInstalmentPayable(account);
                }

                if (parameter.StoreCardPayDetailsGet)
                {
                    var record = (from p in ctx.StorecardPaymentDetails
                                  where p.acctno == parameter.AcctNo
                                  select p).AnsiFirstOrDefault(ctx);

                    account.StoreCardPaymentDetails = record ?? new StorecardPaymentDetails();
                }

                if (parameter.FinTransGet)
                {
                    var q = from c in ctx.FinTrans  //Putting this in to prevent performance issues
                            where c.acctno == parameter.AcctNo
                            select c;

                    account.FinTrans = q.AnsiToList(ctx); // q.AnsiToList(ctx);
                }

                if (parameter.CustAcctHolderGet ||
                    parameter.CustomerGet ||
                    parameter.HomeAddressGet ||
                    parameter.ProposalFlagsGet)
                {
                    var record = (from p in ctx.CustAcct
                                  where p.acctno == parameter.AcctNo &&
                                        p.hldorjnt == "H"
                                  select p).AnsiFirstOrDefault(ctx);

                    account.CustAcct = record ?? new CustAcct();
                }



                if (parameter.CustomerGet)
                {
                    account.CustomerLight = (from p in ctx.Customer
                                             where p.custid == account.CustAcct.custid
                                             select new Acct.CustomerLite()
                                             {
                                                 Title = p.title,
                                                 FirstName = p.firstname,
                                                 LastName = p.name,
                                                 creditblocked = p.creditblocked
                                             })
                                  .AnsiFirstOrDefault(ctx);

                }

                if (parameter.HomeAddressGet)
                {
                    var record = (from p in ctx.CustAddress
                                  where p.custid == account.CustAcct.custid &&
                                        p.datemoved == null &&
                                        p.addtype == "H"
                                  select p).AnsiFirstOrDefault(ctx);

                    account.HomeAddress = record ?? new CustAddress();
                }

                if (parameter.ProposalGet)
                {
                    var record = (from p in ctx.Proposal
                                  where p.acctno == parameter.AcctNo
                                  orderby p.dateprop descending
                                  select p).AnsiFirstOrDefault(ctx);

                    account.Proposal = record ?? new Proposal();
                }

                if (parameter.ProposalFlagsGet)
                {
                    var q = from c in ctx.ProposalFlag
                            where c.Acctno == parameter.AcctNo &&
                                  c.custid == account.CustAcct.custid
                            select c;

                    account.ProposalFlags = q.AnsiToList(ctx);
                    ProposalFlagCheck(account);
                }

                if (parameter.InstantCreditFlagGet)
                {
                    var q = from c in ctx.InstantCreditFlag
                            where c.acctno == parameter.AcctNo &&
                                  c.custid == account.CustAcct.custid
                            select c;

                    account.InstantCreditFlags = q.AnsiToList(ctx);

                    foreach (InstantCreditFlag icf in account.InstantCreditFlags.Where(f => f.datecleared == null))
                    {
                        switch (icf.checktype)
                        {
                            case "ARR":
                                account.ICArrearsOutstanding = true;
                                break;
                            case "DEP":
                                account.ICDepositFlagOustanding = true;
                                break;
                            case "INST":
                                account.ICInstalFlagOutstanding = true;
                                break;
                            case "CHQ":
                                account.ICChequeFlagOutstanding = true;
                                break;
                            case "REF":
                                account.ICReferralFlagOutstanding = true;
                                break;
                            default:
                                break;
                        }
                    }

                    if (parameter.AgreementGet &&
                        parameter.InstalPlanGet &&
                        parameter.FinTransGet &&
                        parameter.CustAcctHolderGet)
                    {
                        DepositOutstandingCheck(account);
                        InstantCreditSetChequeFlag(account, empeeNo, connection, trans); //IP - 03/03/11 - #3255 - Added empeeno
                    }

                    parameter.MaxCustArrearsGet = true;
                }


                if (parameter.MonthsHistoryGet)
                {
                    DatedelClass record = (from ca in ctx.CustAcct
                                           join ag in ctx.Agreement on ca.acctno equals ag.acctno
                                           join ac in ctx.Acct on ag.acctno equals ac.acctno
                                           join ip in ctx.Instalplan on ac.acctno equals ip.acctno
                                           where ca.custid == account.CustAcct.custid && ca.hldorjnt == "H"
                                           && ac.outstbal > 1
                                           && ac.currstatus != "S" && ag.datedel > DateTime.Now.AddYears(-10)
                                           orderby ag.datedel ascending
                                           select new DatedelClass
                                           {
                                               DateDel = Convert.ToDateTime(ag.datedel)

                                           }).AnsiFirstOrDefault(ctx);

                    if (record != null)
                    { // 
                        account.CustomerLiveMonthsHistory = monthDifference(Convert.ToDateTime(record.DateDel), DateTime.Now);

                    } //gghh
                    account.CustomerSettledMonthsHistory = 0;

                    var settledacs = (from ca in ctx.CustAcct
                                      join ag in ctx.Agreement on ca.acctno equals ag.acctno
                                      join ac in ctx.Acct on ag.acctno equals ac.acctno
                                      join ip in ctx.Instalplan on ac.acctno equals ip.acctno //ensures credit account
                                      join sc in ctx.Status on ip.acctno equals sc.acctno
                                      where ca.custid == account.CustAcct.custid && ca.hldorjnt == "H"
                                      && ac.currstatus == "S" && sc.statuscode == "S"
                                      && ag.datedel > DateTime.Now.AddYears(-10)
                                      && sc.datestatchge > DateTime.Now.AddMonths(-parameter.SettledMonthsToConsider)
                                      select new SettledAccts
                                      {
                                          acctno = ag.acctno,
                                          Datedel = Convert.ToDateTime(ag.datedel),
                                          datesetttled = Convert.ToDateTime(sc.datestatchge)
                                      }).AnsiToList(ctx);

                    foreach (var acct in settledacs)
                    {
                        int monthdifference = monthDifference(Convert.ToDateTime(acct.Datedel),
                            Convert.ToDateTime(acct.datesetttled));
                        if (monthdifference > account.CustomerSettledMonthsHistory)
                            account.CustomerSettledMonthsHistory = monthdifference;
                    }

                }

                if (parameter.MaxCustArrearsGet || parameter.MonthsHistoryGet)
                {
                    GetMaxCustArrearsLevel gmc = new GetMaxCustArrearsLevel(connection, trans);
                    double? maxArrearslevel = 0.0f; double? maxsettledarrearslevel = 0.0f; double? maxHistoricArrearsLevel = 0.0f;
                    gmc.ExecuteNonQuery(account.CustAcct.custid, out maxArrearslevel, out maxsettledarrearslevel,
                                        parameter.SettledArrearsMonthsToCheck, Convert.ToInt32(account.CustomerLiveMonthsHistory),
                                        out maxHistoricArrearsLevel, DateTime.Now);
                    account.MaxCustArrearsLevel = Convert.ToSingle(maxArrearslevel);
                    account.MaxCustSettledArrearsLevel = Convert.ToSingle(maxsettledarrearslevel);
                    account.MaxCustLiveHistoricArrearsLevel = Convert.ToSingle(maxHistoricArrearsLevel);
                }
                return account;
            }
        }

        //public void ICClearOutstandingFlags(SqlConnection conn, SqlTransaction trans, string acctNo, int empeeNo)
        //{
        //    var param = new Acct.Parameters.Load()
        //    {
        //        AcctNo = acctNo,
        //        CustAcctHolderGet = true,
        //        InstantCreditFlagGet = true,
        //    };

        //    var account = Get(param, empeeNo, conn, trans);

        //    //Removing as cleared elsewhere
        //    DHoldFlags holdFlags = new DHoldFlags();

        //    holdFlags.CustomerID = account.CustAcct.custid;
        //    holdFlags.EmployeeNoFlag = empeeNo;

        //    if (account.InstantCreditFlags.Any(f => f.datecleared == null))
        //    {
        //        holdFlags.CheckType = "CHQ";
        //        holdFlags.DateCleared = DateTime.Now;
        //        holdFlags.SaveInstantCreditFlag(conn, trans, account.acctno);
        //    }
        //}

        public void CalculateFirstInstalmentPayable(Acct account)
        {
            DataSet termsTypeSet = new DTermsType().LoadTermsTypeDetails(account.termstype);

            //IP - 05/01/12 - #9382 - LW74386 - Only do the following if the account has a Terms Type.
            if (termsTypeSet.Tables[TN.TermsType].Rows.Count > 0)
            {
                if (termsTypeSet.Tables[TN.TermsType].Rows[0][CN.InstalPreDel].ToString() == "Y" && !account.InstalPlan.InstalmentWaived)
                    account.FirstIntalmentPayable = account.InstalPlan.instalamount;
                else
                    account.FirstIntalmentPayable = 0;
            }
            else
            {
                account.FirstIntalmentPayable = 0;
            }
        }

        //IP - 24/02/11 - #2807 - CR1090 - Qualification for Waiver
        public void UpdateFirstInstalmentWaiver(SqlConnection conn, SqlTransaction trans, string acctNo)
        {
            using (var ctx = Context.Create(conn))
            {
                ctx.Transaction = trans;

                var instalPlan = (from i in ctx.Instalplan
                                  where i.acctno == acctNo
                                  select i).SingleOrDefault();

                if (instalPlan != null)
                {
                    instalPlan.InstalmentWaived = true;
                    ctx.SubmitChanges();
                }
            }
        }

        public void DepositOutstandingCheck(Acct account)
        {
            //Was not false if didn't enter loop. Changed 24/2/2010
            decimal paymentTotal = 0.0m;
            decimal clearedPaymentTotal = 0.0m;

            account.DepositorFIOutstanding = true;

            foreach (FinTrans ft in account.FinTrans)
            {
                if (ft.transtypecode == TransType.Payment || ft.transtypecode == TransType.Correction ||
                    ft.transtypecode == TransType.Transfer || ft.transtypecode == TransType.Refund)
                {
                    if (!Convert.ToString(ft.paymethod).EndsWith("2") ||
                         DateTime.Now > ft.datetrans.AddDays(Country.GetCountryParameterValue<int>(CountryParameterNames.ChequeDays)))
                    {
                        clearedPaymentTotal -= ft.transvalue;
                    }
                    else if (Convert.ToString(ft.paymethod).EndsWith("2"))
                    {
                        account.ICChequeOutstanding = true;
                    }

                    paymentTotal -= ft.transvalue;
                }
            }

            if (clearedPaymentTotal >= account.Agreement.deposit + account.FirstIntalmentPayable)
                account.ICChequeOutstanding = false;

            ////if (paymentTotal >=  account.Agreement.deposit + account.FirstIntalmentPayable )
            //{
            //    account.DepositorFIOutstanding = false;
            //}

            //IP - 08/03/11 - #3288
            if (paymentTotal >= account.Agreement.deposit)
                account.ClearDeposit = true;

            account.ICInstalOutstanding = true;
            if ((paymentTotal >= account.FirstIntalmentPayable || account.InstalPlan.InstalmentWaived))
            {
                account.ClearInstalment = true;
                account.ICInstalOutstanding = false;
            }

            if (paymentTotal >= account.Agreement.deposit && (paymentTotal >= account.FirstIntalmentPayable || account.InstalPlan.InstalmentWaived))
                account.DepositorFIOutstanding = false;
        }

        public void ProposalFlagCheck(Acct account)
        {
            if (account.ProposalFlags.Any(f => f.checktype == "DC" && f.datecleared != null))
                account.DocControlCleared = true;

            foreach (ProposalFlag pcf in account.ProposalFlags.Where(f => f.datecleared == null))
            {
                switch (pcf.checktype)
                {
                    case "S1":
                        account.S1FlagOutstanding = true;
                        break;
                    case "S2":
                        account.S2FlagOutstanding = true;
                        break;
                    case "UW":
                        account.UWFlagOutstanding = true;
                        break;
                    default:
                        break;
                }
            }
        }

        public void InstantCreditHPReferralFlag(Acct account, SqlConnection conn, SqlTransaction trans)
        {
            DHoldFlags holdFlags = new DHoldFlags();

            holdFlags.CustomerID = account.CustAcct.custid;
            holdFlags.EmployeeNoFlag = 0;   //TODO Should be passed in as a parameter

            if (!account.ICReferralFlagOutstanding && account.Proposal.propresult != "A") // save the flag
            {
                holdFlags.CheckType = "REF";
                holdFlags.SaveInstantCreditFlag(conn, trans, account.acctno);
                account.ICReferralFlagOutstanding = true;
            }

            // not removing as this is a manual process
            //if (!Account.ICReferralOutstanding && Account.ICReferralFlagOutstanding) //clear the flag
            //{
            //    if (Account.ICChequeFlagOutstanding)
            //    {
            //        Hflags.CheckType = "REF";
            //        Hflags.DateCleared = DateTime.Now;
            //        Hflags.SaveInstantCreditFlag(conn, trans, Account.acctno);
            //        Account.ICChequeFlagOutstanding = false;
            //    }
            //}
        }

        public void InstantCreditSetChequeFlag(Acct account, int empeeNo, SqlConnection conn, SqlTransaction trans) //IP - 03/03/11 - #3255 - Added empeeno
        {
            DHoldFlags holdFlags = new DHoldFlags();
            holdFlags.CustomerID = account.CustAcct.custid;
            holdFlags.EmployeeNoFlag = empeeNo; //IP - 03/03/11 - #3255

            if (account.ICChequeOutstanding && !account.ICChequeFlagOutstanding) // save the flag
            {
                holdFlags.CheckType = "CHQ";
                holdFlags.SaveInstantCreditFlag(conn, trans, account.acctno);
                account.ICChequeFlagOutstanding = true;
            }

            if (!account.ICChequeOutstanding && account.ICChequeFlagOutstanding) //clear the flag
            {
                holdFlags.CheckType = "CHQ";
                holdFlags.DateCleared = DateTime.Now;
                holdFlags.SaveInstantCreditFlag(conn, trans, account.acctno);
                account.ICChequeFlagOutstanding = false;
            }
        }

        /// <summary>
        /// Will check instant credit accounts and DA them if necessary
        /// </summary>
        public bool InstantCreditDACheck(string acctNo, int empeeNo, SqlConnection conn, SqlTransaction trans) //IP - 02/03/11 - #3255 - added empeeno
        {
            var param = new Acct.Parameters.Load()
            {
                AcctNo = acctNo,
                AcctGet = true,
                FinTransGet = true,
                AgreementGet = true,
                InstalPlanGet = true,
                ProposalFlagsGet = true,
                ProposalGet = true,
                InstantCreditFlagGet = true,
                CustAcctHolderGet = true,
                MaxCustArrearsGet = true
            };

            var account = Get(param, empeeNo, conn, trans); //IP - 03/03/11 - #3255 

            //#19219 - CR15594 - Prevent INST flag for Ready Assist
            var isReadyAssist = this.IsReadyAssist(conn, trans, acctNo, 1);

            var holdFlags = new DHoldFlags()
            {
                CustomerID = account.CustAcct.custid,
                EmployeeNoFlag = empeeNo,                   //IP - 03/03/11 - #3255 
                DateCleared = DateTime.Now
            };

            if (account.Agreement.holdprop != "Y" || account.InstalPlan.InstantCredit.NotIn(IC.Approved, IC.Granted))
                return false;

            holdFlags.DateCleared = DateTime.Now;
            if (account.ICDepositFlagOustanding && account.ClearDeposit) //IP - 08/03/11 - #3288 - Added check on ClearDeposit
            {
                holdFlags.CheckType = "DEP";
                holdFlags.SaveInstantCreditFlag(conn, trans, account.acctno);
                account.ICDepositFlagOustanding = false;
            }

            if (account.ICInstalFlagOutstanding && account.ClearInstalment && !isReadyAssist) //#19219 - CR15594 //IP - 08/03/11 - #3288 - Added check on ClearInstalment
            {
                holdFlags.CheckType = "INST";
                holdFlags.SaveInstantCreditFlag(conn, trans, account.acctno);
            }

            holdFlags.DateCleared = DateTime.MinValue.AddYears(1899);
            if (!account.ICDepositFlagOustanding && account.Agreement.deposit > 0 && !account.ClearDeposit)
            {
                holdFlags.CheckType = "DEP";
                holdFlags.SaveInstantCreditFlag(conn, trans, account.acctno);
                account.ICDepositFlagOustanding = true;
            }

            if (!account.ICInstalFlagOutstanding && !account.ClearInstalment && account.ICInstalOutstanding && !isReadyAssist) //#19219 - CR15594
            {
                holdFlags.CheckType = "INST";
                holdFlags.SaveInstantCreditFlag(conn, trans, account.acctno);
                account.ICInstalFlagOutstanding = true;
            }

            bool noArrears = account.MaxCustArrearsLevel <= Country.GetCountryParameterValue<float>(CountryParameterNames.IC_MaxArrearsLevel);
            holdFlags.CheckType = "ARR";
            if (noArrears && account.ICArrearsOutstanding)
            {
                holdFlags.DateCleared = DateTime.Now;
                holdFlags.SaveInstantCreditFlag(conn, trans, account.acctno);
            }
            else if (!noArrears & !account.ICArrearsOutstanding)
            {
                holdFlags.DateCleared = DateTime.MinValue.AddYears(1899);
                holdFlags.SaveInstantCreditFlag(conn, trans, account.acctno);
                account.ICArrearsOutstanding = true;
            }

            InstantCreditHPReferralFlag(account, conn, trans);

            //#19238 - CR15594 - If Ready Assist ignore Instalment Pre del.
            bool autoDA = (!account.DepositorFIOutstanding || (isReadyAssist && !account.ICDepositFlagOustanding)) &&   
                          noArrears &&
                          !account.UWFlagOutstanding &&
                          !account.S1FlagOutstanding &&
                          !account.S2FlagOutstanding &&
                          account.Proposal.propresult == "A" &&
                            account.DocControlCleared &&
                          !account.ICReferralFlagOutstanding &&
                          account.DocControlCleared &&
                          !account.ICChequeOutstanding;

            if (autoDA)
            {
                //new DAgreement() { User = Users.ICAutoDA }.ClearProposal(conn, trans, acctNo, "AUTO");

               
                // Collect hold property value before clear proposal action.
                var AcctR = new AccountRepository();
                bool holdProp = AcctR.GetAccountData(acctNo, conn, trans);

                DAgreement agree = new DAgreement();
                agree.User = Users.ICAutoDA;
                agree.ClearProposal(conn, trans, acctNo, "AUTO");

                // CR - Jyoti - Create Service Request for Installation items only after Payment is done
                // instead of creating it on line item sale on windows PoS.
                if (holdProp)
                {
                    BTransaction tran = new BTransaction();
                    tran.ServiceRequestCreate(conn, trans, acctNo);
                }

                DataTable lineItemBooking = agree.LineItemBooking;      // #10178

                // #10178 - Send bookings to warehouse for scheduled orders
                if (lineItemBooking != null && lineItemBooking.Rows.Count > 0)          //IP/JC - 30/05/12 - #10178
                {
                    lineItemBooking.Columns.Add("BookingID");

                    this.InsertLineItemBooking(conn, trans, ref lineItemBooking);
                    this.bookingType = "D";

                    foreach (DataRow row in lineItemBooking.Rows)
                    {
                        new WarehouseRepository().UpdateLineItemBookingScheduleBookingId(conn, trans, Convert.ToInt32(row["ID"]), Convert.ToInt32(row["BookingID"]));  //#13829 - Update before GetBookingData    //#13790 - Update LineItemBookingSchedule with the BookingId
                    }

                    var bookings = (IEnumerable<BookingSubmit>)this.GetBookingData(conn, trans, lineItemBooking);

                    new Chub().SubmitMany(bookings, conn, trans); 
                }

                //#19193 - CR15594 - If Ready Assist and auto da, then deliver the ready assist item
                BPayment affinityPayment = new BPayment();
                affinityPayment.User = empeeNo;

                var readyAssist = this.IsReadyAssist(conn, trans, acctNo, 1); 
                affinityPayment.DeliverAffinity(conn, trans, acctNo, readyAssist);

                if (readyAssist == true)
                {
                    var status = ReadyAssistStatus.Active;
                    this.UpdateReadyAssistContractDate(conn, trans, acctNo, 1);
                    this.UpdateReadyAssistStatus(conn, trans, acctNo, 1, status);
                }

            }

            return autoDA;
        }

        public List<Code> GetCodesforCategory(string category, SqlConnection conn, SqlTransaction trans)
        {
            var codes = new List<Code>();
            using (var ctx = Context.Create(conn, trans))
            {
                var q = from c in ctx.Code
                        where c.category == category
                        orderby c.code ascending
                        select c;

                codes = q.AnsiToList(ctx);
            }

            return codes;
        }

        //IP - 14/03/11 - #3314 - CR1245 - Method to return referral reason descriptions from their codes
        public string GetReferralDescriptions(SqlConnection conn, SqlTransaction trans, params string[] refcodes)
        {
            if (refcodes == null)
                return "";

            using (var ctx = Context.Create(conn, trans))
            {
                var codes = from c in ctx.Code
                            where c.category == "SN1" && refcodes.Contains(c.code)
                            select c.codedescript;

                return codes.Stringify(separator: ",");
            }
        }

        public decimal UpliftPercentage(string acctno, SqlConnection conn, SqlTransaction trans)
        {
            var codes = GetCodesforCategory("LXR", conn, trans);

            short settledMonthsToConsider = 0;
            int settledArrearsMonthsToCheck = 0;
            foreach (var code in codes.Where(c => c.code == "HMAS1"))
            {
                settledMonthsToConsider = code.sortorder;
                settledArrearsMonthsToCheck = Convert.ToInt32(code.reference);
                break;
            }

            var param = new Acct.Parameters.Load()
            {
                AcctNo = acctno,
                MaxCustArrearsGet = true,
                CustAcctHolderGet = true,
                MonthsHistoryGet = true,
                SettledMonthsToConsider = settledMonthsToConsider,
                SettledArrearsMonthsToCheck = settledArrearsMonthsToCheck
            };

            var account = Get(param, 0, conn, trans);

            Decimal uplift = CalculateUpliftPercentage(account, codes);

            if (uplift > 0 && uplift < 120)
            {
                var pup = new ProposalUpdateUpliftPercent(conn, trans);
                pup.ExecuteNonQuery(account.acctno, account.CustAcct.custid, Convert.ToInt16(uplift));
            }

            return uplift;
        }

        private decimal CalculateUpliftPercentage(Acct Account, List<Code> Codes)
        {
            var minMonthsHistory = 0.0m;
            //we are storing min months history in sortorder, max value  in reference and percentage in additional  
            foreach (Code code in Codes)
            {
                if (code.code == "HMAC1")
                    minMonthsHistory = code.sortorder;
            }

            decimal upliftPercentage = 0.0m;

            if (Account.CustomerLiveMonthsHistory >= minMonthsHistory)
            {
                foreach (Code code in Codes)
                {
                    if (code.code == "HMAC1" && Account.MaxCustLiveHistoricArrearsLevel < Convert.ToSingle(code.reference))
                    {
                        upliftPercentage = Convert.ToDecimal(code.additional);
                        break;
                    }
                    if (code.code == "HMAC2" && Account.MaxCustLiveHistoricArrearsLevel < Convert.ToSingle(code.reference))
                    {
                        upliftPercentage = Convert.ToDecimal(code.additional);
                        break;
                    }
                    if (code.code == "HMAC3" && Account.MaxCustLiveHistoricArrearsLevel < Convert.ToSingle(code.reference))
                    {
                        upliftPercentage = Convert.ToDecimal(code.additional);
                        break;
                    }
                }
            }
            //else check settled months
            if (Account.CustomerSettledMonthsHistory >= minMonthsHistory && upliftPercentage == 0.0m
                && Account.CustomerLiveMonthsHistory < minMonthsHistory)
            {
                foreach (Code code in Codes)
                {
                    if (code.code == "HMAS1" && Account.MaxCustSettledArrearsLevel < Convert.ToSingle(code.reference))
                    {
                        upliftPercentage = Convert.ToDecimal(code.additional);
                        break;
                    }
                    if (code.code == "HMAS2" && Account.MaxCustSettledArrearsLevel < Convert.ToSingle(code.reference))
                    {
                        upliftPercentage = Convert.ToDecimal(code.additional);
                        break;
                    }
                    if (code.code == "HMAS3" && Account.MaxCustSettledArrearsLevel < Convert.ToSingle(code.reference))
                    {
                        upliftPercentage = Convert.ToDecimal(code.additional);
                        break;
                    }
                }
            }
            return upliftPercentage;
        }

        public bool IsLatestAccountforCustomer(string accountNo, string CustomerId)
        {
            var Act = new AcctCheckIfLatest();
            byte? isLatest = 0;
            Act.ExecuteNonQuery(accountNo, CustomerId, out isLatest);
            if (isLatest == 1)
                return true;
            else
                return false;
        }

        public bool IsSRInstSpecialAccount(string accountNo)
        {
            if (String.IsNullOrEmpty(accountNo))
                return false;

            var cmQuery = Context.Create().CountryMaintenance
                            .Where(cm => cm.Value.Replace("-", "") == accountNo.Replace("-", ""));

            cmQuery = cmQuery.Where(cm => cm.CodeName == CountryParameterNames.ServiceStockAccount ||
                                            cm.CodeName == CountryParameterNames.ServiceWarranty ||
                                            cm.CodeName == CountryParameterNames.ServiceInternal ||
                                            cm.CodeName == CountryParameterNames.InstalChgAcct);

            var codeQuery = Context.Create().Code
                            .Where(c => c.category == CAT.ServiceModel &&
                                        c.reference.Replace("-", "") == accountNo.Replace("-", ""));

            return cmQuery.Any() || codeQuery.Any();
        }

        public void ResetAcctnos(SqlConnection conn, SqlTransaction trans, short branchno, string acctcat)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var q = from aControl in ctx.acctnoctrl
                        where aControl.branchno == branchno &&
                              aControl.acctcat == acctcat
                        select aControl;

                q.Single().hiallocated = 1;
                ctx.SubmitChanges();
            }
        }

        //IP - 15/03/12 - #9797 - select sum OrdVal for an account to give the agreement total. 
        public decimal? SumOrdValForAcct(SqlConnection conn, SqlTransaction trans, string acctNo, int agrmtNo)
        {
            decimal? sumOrdVal = 0.0m;

            using (var ctx = Context.Create(conn, trans))
            {
                sumOrdVal = (from l in ctx.LineItem
                             where l.acctno == acctNo && l.agrmtno == agrmtNo
                             && l.isKit == 0                                                //#15492
                             select l.ordval).Sum();

                return sumOrdVal;
            }
        }

        //IP - 17/05/12 - #9447 - CR1239
        public void InsertIntoCashAndGoReceipt(SqlConnection conn, SqlTransaction trans, string acctNo, int agrmtNo, bool taxExempt, decimal change, int cashierEmpeeno, int payMethod) //IP - 22/05/12 - #10156 - added payMethod
        {
            using (var ctx = Context.Create(conn, trans))
            {
                CashAndGoReceipt receipt = new CashAndGoReceipt
                {
                    AcctNo = acctNo,
                    AgrmtNo = agrmtNo,
                    TaxExempt = taxExempt,
                    Change = change,
                    CashierEmpeeNo = cashierEmpeeno,
                    PayMethod = payMethod                                       //IP - 22/05/12 - #10156
                };

                ctx.CashAndGoReceipt.InsertOnSubmit(receipt);
                ctx.SubmitChanges();

            }
        }

        public void InsertLineItemBooking(SqlConnection conn, SqlTransaction trans, ref DataTable lineItemBooking) //IP/JC - 30/05/12 -  #10178
        {
            using (var ctx = Context.Create(conn, trans))
            {
                HiLo bookingID = HiLo.Cache("Warehouse.Booking");

                //Add DeliveredQty if not exists (exists when DA)
                if (!lineItemBooking.Columns.Contains("DeliveredQty"))  //#10507
                {
                    lineItemBooking.Columns.Add("DeliveredQty");

                    foreach (DataRow dr in lineItemBooking.Rows)
                    {
                        dr["DeliveredQty"] = 0;
                    }
                }

                //#14644
                //Add ScheduledQty if not exists (exists when DA)
                if (!lineItemBooking.Columns.Contains("ScheduledQty"))  //#10507
                {
                    lineItemBooking.Columns.Add("ScheduledQty");

                    foreach (DataRow dr in lineItemBooking.Rows)
                    {
                        dr["ScheduledQty"] = 0;
                    }
                }

                foreach (DataRow dr in lineItemBooking.Rows)
                {
                    var bookID = bookingID.NextId();

                    LineItemBooking booking = new LineItemBooking
                    {
                        ID = bookID,
                        LineItemID = Convert.ToInt32(dr["id"]),
                        Quantity = (Convert.ToSingle(dr["QtyBooked"]) - Convert.ToSingle(dr["DeliveredQty"])) - Convert.ToSingle(dr["ScheduledQty"]) //#14644 - subtract ScheduledQty//#10488
                    };

                    dr["BookingID"] = bookID;

                    ctx.LineItemBooking.InsertOnSubmit(booking);
                    ctx.SubmitChanges();


                }
            }
        }


        public List<BookingSubmit> GetBookingData(SqlConnection conn, SqlTransaction trans, DataTable lineItemBooking,  //IP/JC - 30/05/12 - #10178 - Warehouse & Deliveries Integration
                                    short stockLocn = 0, short retStockLocn = 0, string deliveryArea = "", string comments = "",
                                    int user = 0, string deliveryProcess = "", string deliveryAdr = "")        // # 14927 #12378 #10481 #10489
        {
            using (var ctx = Context.Create(conn, trans))
            {
                List<BookingSubmit> bookings = new List<BookingSubmit>();

                var repoDelUnitPrice = Convert.ToBoolean(Country[CountryParameterNames.RepoDelUnitPrice]);      //#10406               
                var bookingIDs = lineItemBooking.AsEnumerable().Select(s => Convert.ToInt32(s.Field<string>("BookingID"))).ToList(); // Address Standardization CR2019 - 025

                var customerIds = (from lb in ctx.LineItemBooking  // Address Standardization CR2019 - 025
                                   join l in ctx.LineItem on lb.LineItemID equals l.ID
                                   join ls in ctx.LineItemBookingSchedule on lb.ID equals ls.BookingId
                                   join b in ctx.Branch on l.SalesBrnNo != null ? l.SalesBrnNo : Convert.ToInt16(l.acctno.Substring(0, 3)) equals b.branchno
                                   join ca in ctx.CustAcct on l.acctno equals ca.acctno
                                   join a in ctx.Acct on ca.acctno equals a.acctno
                                   join ag in ctx.Agreement on a.acctno equals ag.acctno
                                   join cu in ctx.Customer on ca.custid equals cu.custid
                                   join cadd in ctx.CustAddress on ca.custid equals cadd.custid
                                   join si in ctx.StockInfo on l.ItemID equals si.Id
                                   join sir in ctx.StockInfo on si.IUPC equals sir.IUPC into s
                                   from dm in
                                    (from sir in s select sir).DefaultIfEmpty()
                                   where ca.hldorjnt == "H" &&
                                         cadd.addtype == (deliveryAdr == "" ? l.deliveryaddress : deliveryAdr) &&
                                         cadd.datemoved == null &&                                        
                                         bookingIDs.Contains(lb.ID)
                                   select ca.custid).ToList();

                var customerTelDetails = ctx.CustTel.Where(c => customerIds.Contains(c.custid) && c.datediscon == null).ToList();// Address Standardization CR2019 - 025

                foreach (DataRow dr in lineItemBooking.Rows)
                {
                    var nonStockDetails = (from lb in ctx.LineItemBooking
                                           join l in ctx.LineItem on lb.LineItemID equals l.ID
                                           join ln in ctx.LineItem on l.acctno equals ln.acctno
                                           join s in ctx.StockInfo on ln.ItemID equals s.Id
                                           join inst in ctx.Code on s.itemno equals inst.code 
                                           where ln.agrmtno == l.agrmtno
                                           && ln.ParentItemID == l.ItemID
                                           && (inst.category == "INST" || inst.category == "ASSY")
                                           && lb.ID == Convert.ToInt32(dr["BookingID"])
                                           select new
                                           {
                                               NonStockServiceType = inst.category == "INST" ? NonStockTypes.Installation : NonStockTypes.Assembly,
                                               NonStockServiceItemNo = s.itemno,
                                               NonStockServiceDescription = s.itemdescr1.Trim() + " " + s.itemdescr2.Trim()
                                           }).FirstOrDefault();
                                          

                    var details = (from lb in ctx.LineItemBooking
                                   join l in ctx.LineItem on lb.LineItemID equals l.ID
                                   join ls in ctx.LineItemBookingSchedule on lb.ID equals ls.BookingId      //#13829
                                   join b in ctx.Branch on l.SalesBrnNo != null ? l.SalesBrnNo : Convert.ToInt16(l.acctno.Substring(0, 3)) equals b.branchno     // #11042 l.SalesBrNo may be null
                                   join ca in ctx.CustAcct on l.acctno equals ca.acctno
                                   join a in ctx.Acct on ca.acctno equals a.acctno
                                   join ag in ctx.Agreement on a.acctno equals ag.acctno        //#10338
                                   join cu in ctx.Customer on ca.custid equals cu.custid
                                   join cadd in ctx.CustAddress on ca.custid equals cadd.custid
                                   join si in ctx.StockInfo on l.ItemID equals si.Id
                                   join sir in ctx.StockInfo on si.IUPC equals sir.IUPC into s
                                   from dm in
                                     (from sir in s select sir).DefaultIfEmpty()
                                       //from sRep in
                                       //    (from sir in s where sir.RepossessedItem == true select sir).DefaultIfEmpty()       // #10280 Left join for Repo item only works like this - cannot get to work using where clause?
                                       //join spr in ctx.StockPrice on sRep.ID equals spr.ID into sp                                      // #10406 get repo item price
                                       //from spRep in
                                       //    (from spr in sp where spr.branchno == l.stocklocn select spr).DefaultIfEmpty()                               

                                   where ca.hldorjnt == "H" &&
                                   cadd.addtype == (deliveryAdr == "" ? l.deliveryaddress : deliveryAdr)  &&                     // #10311 
                                   cadd.datemoved == null &&
                                   si.RepossessedItem == false &&
                                   lb.ID == Convert.ToInt32(dr["BookingID"])

                                   select new BookingSubmit
                                   {
                                       Id = lb.ID,
                                       Recipient = cu.title + " " + cu.firstname + " " + cu.name,
                                       AddressLine1 = cadd.cusaddr1,
                                       AddressLine2 = cadd.cusaddr2,
                                       AddressLine3 = cadd.cusaddr3,
                                       PostCode = cadd.cuspocode,
                                       StockBranch = stockLocn == 0 ? l.stocklocn : stockLocn,      // #10481
                                       DeliveryBranch = retStockLocn == 0 ? Convert.ToInt16(l.delnotebranch) : retStockLocn,        // #10489
                                       Type = bookingType,
                                       //DeliveryOrCollectionDate = Convert.ToDateTime(l.datereqdel),
                                       //DeliveryOrCollectionDate = Convert.ToDateTime(l.datereqdel) < DateTime.UtcNow.Date ? DateTime.UtcNow.Date : Convert.ToDateTime(l.datereqdel), //#13056 //#10704
                                       RequestedDate = bookingType != "D" ? DateTime.Now.Date : Convert.ToDateTime(l.datereqdel),            // #13862 
                                       SKU = si.itemno,
                                       ItemId = si.Id,
                                       ItemUPC = si.IUPC,
                                       ProductDescription = si.itemdescr1 + " " + "\n" + si.itemdescr2,         // #11069
                                       ProductBrand = si.Brand == null ? "" : si.Brand,
                                       ProductModel = si.VendorLongStyle == null ? "" : si.VendorLongStyle,
                                       ProductArea = " ",           // this is the zone in warehouse
                                       ProductCategory = Convert.ToString(si.category),
                                       Quantity = Convert.ToInt16(lb.Quantity),
                                       RepoItemId = Convert.ToInt32(ls.RetItemID),   //#13829 
                                       Comment = comments == "" ? l.notes : comments,           // #10481
                                       DeliveryZone = deliveryArea == "" ? l.deliveryarea : deliveryArea,       // #10481
                                                                                                                // #13765 excl "Home","Work" "Mobile" if number is blank                                       
                                       ContactInfo = GetContactInfo(ca.custid, l.deliveryaddress.Trim(), customerTelDetails),// #10299
                                       OrderedOn = bookingType != "D" ? DateTime.Now : Convert.ToDateTime(a.dateacctopen),     //#13984
                                       Damaged = l.damaged == null ? false : l.damaged == "Y" ? true : false,                      // #11402 #10271
                                       AssemblyReq = l.assemblyrequired == null ? false : l.assemblyrequired == "Y" ? true : false,       // #11402 #10271
                                       Express = l.Express == null ? false : l.Express == "Y" ? true : false,                    //IP - 07/06/12 - #10229
                                       Reference = l.acctno,
                                       //UnitPrice = l.price,
                                       UnitPrice = bookingType != "R" ? Convert.ToDecimal(l.price) : repoDelUnitPrice == false ? 0 : Convert.ToDecimal(ls.RepUnitPrice) == null ? 0 : Convert.ToDecimal(ls.RepUnitPrice), //#13829     // #10686  jec #10406 - jec if repo use repo price or zero depending on parameter
                                       CreatedBy = bookingType == "R" ? user : ag.createdby, //#10386  //#10338
                                       AddressNotes = cadd.Notes,            // #10517
                                       Fascia = b.StoreType,                  // #10701
                                       PickUp = deliveryProcess == "" ? l.deliveryprocess == "S" ? false : true : deliveryProcess == "S" ? false : true,  // #12378 #10789
                                       SalesBranch = (short?)l.SalesBrnNo,       //#19331
                                       NonStockServiceType = nonStockDetails == null ? null : nonStockDetails.NonStockServiceType,
                                       NonStockServiceItemNo = nonStockDetails == null ? null : nonStockDetails.NonStockServiceItemNo,
                                       NonStockServiceDescription = nonStockDetails == null ? null : nonStockDetails.NonStockServiceDescription
                                   }).ToList<BookingSubmit>();

                    bookings.Add(details.FirstOrDefault());

                }

                return bookings;
            }
        }
        #region Address Standardization CR2019 - 025
        // The function shown Contact Info By Address Type//
        private string GetContactInfo(string custId, string deliveryAddress, List<CustTel> customerTelDetails)// Address Standardization CR2019 - 025
        {
            System.Text.StringBuilder contactInfo = new System.Text.StringBuilder();
            List<String> preferences = new List<String> { "H", "W", "M", "D", "DM", "D1", "D1M", "D2", "D2M", "D3", "D3M" };
            List<string> contactType = GetContactType(deliveryAddress);
            var contacts = customerTelDetails.Where(c => c.custid == custId && contactType.Contains(c.tellocn.Trim()))
                                             .OrderBy(item => preferences.IndexOf(item.tellocn.Trim()));
            string contactTypeName = string.Empty;
            string telDetails = string.Empty;
            foreach (CustTel cTel in contacts)
            {
                telDetails = GetContactTypeDetails(cTel);
                if (!string.IsNullOrEmpty(telDetails.Trim()))
                {
                    contactTypeName = GetContactTypeLable(cTel.tellocn.Trim());
                    contactInfo.Append(contactTypeName + " " + telDetails + " " + "\n");
                }
            }
            return contactInfo.ToString();
        }

        private string GetContactTypeLable(string deliveryAddress)// Address Standardization CR2019 - 025
        {
            string contactTypeName = string.Empty;

            switch (deliveryAddress)
            {
                case "H":
                    contactTypeName = "Home";// #13765 #11103 #10299
                    break;
                case "W":
                    contactTypeName = "Work"; // #13765
                    break;
                case "M":
                    contactTypeName = "Mobile";// #13765 #11103 
                    break;
                case "D":
                case "D1":
                case "D2":
                case "D3":
                    contactTypeName = "Delivery";
                    break;
                case "DM":
                case "D1M":
                case "D2M":
                case "D3M":
                    contactTypeName = "Delivery Mobile";
                    break;
                default:
                    break;
            }
            return contactTypeName;
        }


        private List<string> GetContactType(string deliveryAddress)// Address Standardization CR2019 - 025
        {
            List<string> contactType = new List<string>();
            contactType.Add("H");  // #13765 #11103 #10299
            contactType.Add("W");   // #13765 #11103 
            contactType.Add("M");  // #13765

            switch (deliveryAddress)
            {
                case "D":
                    contactType.Add("D");
                    contactType.Add("DM");
                    break;
                case "D1":
                    contactType.Add("D1");
                    contactType.Add("D1M");
                    break;
                case "D2":
                    contactType.Add("D2");
                    contactType.Add("D2M");
                    break;
                case "D3":
                    contactType.Add("D3");
                    contactType.Add("D3M");
                    break;
                default:
                    break;
            }
            return contactType;
        }

        private string GetContactTypeDetails(CustTel cTel) // Address Standardization CR2019 - 025
        {
            string[] telltypes = { "M", "DM", "D1M", "D2M", "D3M" };

            if (telltypes.Contains(cTel.tellocn.Trim()))
            {
                return (cTel.telno == null ? "" : cTel.telno.Trim());
            }
            return (cTel.DialCode == null ? "" : cTel.DialCode.Trim()) + " " + (cTel.telno == null ? "" : cTel.telno.Trim()) + " " + (cTel.extnno == null ? "" : cTel.extnno.Trim());
        }

        #endregion

        public IEnumerable<Message<BookingCancel>> GetCancelData(SqlConnection conn, SqlTransaction trans, int lineitemId, int user, string comment) //IP/JC - 30/05/12 - #10178 - Warehouse & Deliveries Integration
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var cancelations = new List<Message<BookingCancel>>();

                var details = (from lb in ctx.LineItemBooking
                               join li in ctx.LineItem on lb.LineItemID equals li.ID
                               where lb.LineItemID == lineitemId

                               select new Message<BookingCancel>
                               {
                                    CorrelationId = li.acctno,
                                    Payload = new BookingCancel
                                    {
                                        Id = lb.ID,
                                        User = user,
                                        //Quantity = Convert.ToInt32(lb.Quantity),             // #10279  Note: may need to calc quantity?
                                        DateTime = DateTime.Now,
                                        Comment = comment
                                    }
                               }
                               ).ToList<Message<BookingCancel>>();

                if (details.Count > 0)
                {
                    cancelations.Add(details.Last());
                    return cancelations;
                }
                else
                    return new List<Message<BookingCancel>>();
            }
        }

        // only getting data
        public List<BookingCancel> GetCancelCollectionNoteData(SqlConnection conn, SqlTransaction trans, int lineitemId, int user, string comment)  // #16525
        {
            using (var ctx = Context.Create(conn, trans))
            {
                List<BookingCancel> cancelations = new List<BookingCancel>();

                var details = (from lb in ctx.LineItemBooking
                               where lb.LineItemID == lineitemId
                               select new BookingCancel
                               {
                                   Id = lb.ID,
                                   User = user,
                                   DateTime = DateTime.Now,
                                   Comment = comment
                               }
                               ).ToList<BookingCancel>();

                cancelations.Add(details.Last());

                return cancelations;
            }
        }

        //#14313 -  Identical Replacement - Cancel Collection Note
        public IEnumerable<Message<BookingCancel>> GetCancelDataIR(SqlConnection conn, SqlTransaction trans, int lineitemId, int bookingIdDel, int user, string comment) 
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var cancelations = new List<Message<BookingCancel>>();

                var details = ( from lb in ctx.LineItemBooking
                                join li in ctx.LineItem on lb.LineItemID equals li.ID
                                where lb.LineItemID == lineitemId 
                                && lb.ID < bookingIdDel

                                select new Message<BookingCancel>
                                {
                                    
                                    Payload = new BookingCancel
                                    {
                                        Id = lb.ID,
                                        User = user,
                                        DateTime = DateTime.Now,
                                        Comment = comment
                                    },
                                    CorrelationId = li.acctno
                                }
                               ).ToList<Message<BookingCancel>>();

                cancelations.Add(details.Last());

                return cancelations;
            }
        }

        public DataTable GetLineitem(SqlConnection conn, SqlTransaction trans, string acctno, int itemId, int stocklocn)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                DataTable LineItemBooking = new DataTable();
                LineItemBooking.Columns.Add(CN.ID);
                LineItemBooking.Columns.Add(CN.QtyBooked);
                LineItemBooking.Columns.Add(CN.BookingID);
                DataRow dr = LineItemBooking.NewRow();

                var details = (from l in ctx.LineItem
                               where l.acctno == acctno && l.ItemID == itemId && l.stocklocn == stocklocn

                               select l
                               ).AnsiFirstOrDefault(ctx);

                dr[CN.ID] = details.ID;
                dr[CN.QtyBooked] = details.quantity;
                dr[CN.BookingID] = 0;

                LineItemBooking.Rows.Add(dr);

                return LineItemBooking;
            }
        }

        //IP - 13/06/12 - #10328 - Warehouse & Deliveries
        public void BookingForReducedQty(SqlConnection conn, SqlTransaction trans, string acctNo, int user)
        {
            var AcctR = new AccountRepository();
            var lineitems = this.GetItemsForAcct(conn, trans, acctNo);

            DataTable lineItemBooking = new DataTable();
            lineItemBooking.Columns.Add(CN.ID);
            lineItemBooking.Columns.Add(CN.QtyBooked);
            lineItemBooking.Columns.Add(CN.BookingID);

            lineitems.ForEach(delegate(LineItem item)
                {
                    var schedule = new WarehouseRepository().GetLineItemBookingSchedule(conn, trans, Convert.ToInt32(item.ID));     // #12849/#12516 get schedule
                    if (Convert.ToSingle(item.quantity) < Convert.ToSingle(item.delqty) && item.itemtype == "S" && schedule.DelOrColl != "C")
                    {
                        lineItemBooking.Rows.Clear();
                        // get delivered quantity
                        var deliveredQuantity = this.GetDeliveredQuantity(conn, trans, item.acctno, item.ItemID, item.stocklocn, item.ParentItemID);

                        DataRow dr = lineItemBooking.NewRow();
                        dr[CN.ID] = item.ID;
                        dr[CN.QtyBooked] = item.quantity - deliveredQuantity;         // #10605
                        dr[CN.BookingID] = 0;

                        lineItemBooking.Rows.Add(dr);

                        var booking = new WarehouseRepository().GetLineItemBooking(conn, trans, Convert.ToInt32(item.ID));                  //IP - 19/06/12 - #10440 - get the last booking
                        var bookingFailure = new WarehouseRepository().GetLineItemBookingFailures(conn, trans, Convert.ToInt32(item.ID), booking.ID);   //IP - 19/06/12 - #10440 - check if there is a failure

                        if (bookingFailure == null || bookingFailure.Actioned != null)     //IP - 19/06/12 - #10440 - do not process a cancellation if in the LineItemBookingFailure table
                        {
                            var cancelations = AcctR.GetCancelData(conn, trans, item.ID, user, "Cancellation - quantity reduced");
                            new Chub().CancelMany(cancelations, conn, trans);

                           // new WarehouseRepository().UpdateLineItemBookingScheduleQuantity(conn, trans, booking.ID, Convert.ToSingle(item.delqty - item.quantity)); //Reduce the quantity of the schedule
                            new WarehouseRepository().UpdateLineItemBookingScheduleQuantity(conn, trans, booking.ID, 0); //#13829 - update schedule to 0 as new booking for remaining is created //Reduce the quantity of the schedule
                        }

                        if (item.quantity > 0)
                        {
                            AcctR.InsertLineItemBooking(conn, trans, ref lineItemBooking);

                            AcctR.bookingType = "D";

                            //IP - 21/06/12 - #10385 - Insert a new schedule for the new booking
                            new WarehouseRepository().InsertLineItemBookingSchedule(conn, trans, item.ID, "D", 0, 0, 0, Convert.ToInt32(lineItemBooking.Rows[0][CN.BookingID]), Convert.ToSingle(item.quantity) - Convert.ToSingle(deliveredQuantity), // #10605
                            item.ItemID, item.stocklocn, Convert.ToDecimal(item.price));    //#13829 - insert before GetBookingData  //#12842

                            var bookings = (IEnumerable<BookingSubmit>)AcctR.GetBookingData(conn, trans, lineItemBooking);
                            new Chub().SubmitMany(bookings, conn, trans);

                        }

                        //Update the lineitem.delqty to the new quantity
                        UpdateLineItemDelQty(conn, trans, item.ID);

                        //Finally if actioned from Failed Delivery / Colelction screen then update as actioned.
                        //new WarehouseRepository().UpdateBookingFailureActioned(conn, trans, Convert.ToInt32(cancelations.First().Id), Convert.ToInt32(lineItemBooking.Rows[0][CN.BookingID]));
                        new WarehouseRepository().UpdateBookingFailureActioned(conn, trans, booking.ID, Convert.ToInt32(lineItemBooking.Rows[0][CN.BookingID]));  //IP - 19/06/12 - #10440 
                    }
                });

        }

        //IP - 12/06/12 - #10328 - Warehouse & Deliveries
        public List<LineItem> GetItemsForAcct(SqlConnection conn, SqlTransaction trans, string acctNo)
        {
            using (var ctx = Context.Create(conn, trans))
            {

                var lineitems = (from li in ctx.LineItem
                                 where li.acctno == acctNo
                                 select li).ToList<LineItem>();

                return lineitems;
            }
        }

        public double GetDeliveredQuantity(SqlConnection conn, SqlTransaction trans, string acctNo, int itemId, short stocklocn, int parentItemId)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var deliveryQuantity = 0.00;
                // #10645 check if any deliveries before summing
                var delivery = (from d in ctx.Delivery
                                where d.acctno == acctNo && d.ItemID == itemId && d.stocklocn == stocklocn && d.ParentItemID == parentItemId
                                select d).AnsiFirstOrDefault(ctx);
                if (delivery != null)
                {
                    deliveryQuantity = (from d in ctx.Delivery
                                        where d.acctno == acctNo && d.ItemID == itemId && d.stocklocn == stocklocn && d.ParentItemID == parentItemId
                                        select d.quantity).Sum();
                }

                return deliveryQuantity;
            }
        }

        //IP - 13/06/12 - #10328 - Update Lineitem.delqty to the Lineitem.Quantity
        public void UpdateLineItemDelQty(SqlConnection conn, SqlTransaction trans, int lineItemId)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var lineitem = ctx.LineItem.Where(li => li.ID == lineItemId).AnsiFirstOrDefault(ctx);

                lineitem.delqty = lineitem.quantity;

                ctx.SubmitChanges();

            }
        }


        //IP - 26/06/12 - #10516 - check if a repossession was the last thing processed on an item
        public bool CheckForRepo(SqlConnection conn, SqlTransaction trans, string acctNo, int agrmtNo, short stockLocn, int itemId)
        {
            var repo = false;

            using (var ctx = Context.Create(conn, trans))
            {
                var delivery = ctx.Delivery.Where(d => d.acctno == acctNo && d.agrmtno == agrmtNo && d.ItemID == itemId && d.stocklocn == stockLocn).OrderByDescending(d => d.datetrans).FirstOrDefault();      // #10542

                if (delivery != null)
                {
                    repo = delivery.delorcoll == "R" && delivery.quantity < 0 ? true : false;
                }
            }

            return repo;
        }

        //#10358 - Method to save notes from the Failure Delivery / Collection screen
        public void SaveLineItemFailureNotes(SqlConnection conn, SqlTransaction trans, string acctNo, string comments, int empeeNo)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var custid = ctx.CustAcct.Where(ca => ca.acctno == acctNo && ca.hldorjnt == "H").Select(ca => ca.custid).AnsiFirstOrDefault(ctx);

                if (custid != null)
                {
                    SR_CustomerInteraction failureNotes = new SR_CustomerInteraction
                    {
                        CustomerId = custid,
                        Date = DateTime.Today,
                        Code = string.Empty,
                        EmpeeNo = empeeNo,
                        AcctNo = acctNo,
                        ServiceRequestNo = 0,
                        Comments = comments
                    };

                    ctx.SR_CustomerInteraction.InsertOnSubmit(failureNotes);
                    ctx.SubmitChanges();
                }
            }
        }

        //#13716 - CR12949
        public List<LinkedContractsView> GetContractsToPrint(List<int>lineItemIds)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var contractsToPrint = ctx.LinkedContractsView.Where(l => lineItemIds.Contains(l.ID)).ToList();

                return contractsToPrint;
            }); 
        }

        public CashLoanDisbursementDetailsView GetCashLoanDisbursementDetails(string acctNo)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var cashLoanElectronicBankTransferDet = ctx.CashLoanDisbursementDetailsView.Where(cld => cld.AccountNumber == acctNo).FirstOrDefault();

                return cashLoanElectronicBankTransferDet;
            });
        }

        // #14552 get HoldProp
        public string AgreementHoldProp(SqlConnection conn, SqlTransaction trans, string acctNo)
        {
            var HoldProp="";
            using (var ctx = Context.Create(conn, trans))
            {
                var record = (from p in ctx.Agreement
                              where p.acctno == acctNo
                              select p).AnsiFirstOrDefault(ctx);

                HoldProp= record.holdprop;
            }

            return HoldProp;
        }

        //#16168
        public void DeliverWarranties(SqlConnection conn, SqlTransaction trans, string accountNo, int agrmtNo, XmlNode replacement) //#18409
        {
            var mainItems = new List<Blue.Cosacs.Model.LineitemBooking>();

            using (var ctx = Context.Create(conn, trans))
            {

                
                mainItems = (from ls in ctx.LineItem
                              join d in ctx.Delivery on ls.acctno equals d.acctno   
                              join lw in ctx.LineItem on ls.acctno equals lw.acctno
                              where ls.acctno == accountNo && ls.agrmtno == agrmtNo &&
                                    ls.agrmtno == d.agrmtno &&
                                    ls.ItemID == d.ItemID &&
                                    ls.stocklocn == d.stocklocn &&
                                    ls.itemtype == "S" &&
                                    ls.agrmtno == lw.agrmtno &&
                                    lw.ParentItemID == ls.ItemID &&
                                    lw.parentlocation == ls.stocklocn &&
                                    lw.contractno != "" &&
                                    lw.quantity != 0 &&
                                    //ctx.Delivery.Any(del => del.acctno == ls.acctno && del.agrmtno == ls.agrmtno && del.ItemID == ls.ItemID && del.stocklocn == ls.stocklocn) &&
                                    !ctx.Delivery.Any(del => del.acctno == lw.acctno && del.agrmtno == lw.agrmtno && del.ItemID == lw.ItemID && del.contractno == lw.contractno && del.ParentItemID == lw.ParentItemID)
                             select new Blue.Cosacs.Model.LineitemBooking
                             {
                                AcctNo = ls.acctno,
                                AgreementNo = ls.agrmtno,
                                ItemId = ls.ItemID,
                                StockLocation = ls.stocklocn,
                                DelLocation = null,
                                Quantity =  null        //#16206
                              }).Distinct().ToList();

                //#16206 - Get sum of delivery
                foreach (var item in mainItems)
                {
                    item.Quantity = Convert.ToInt16((from d in ctx.Delivery
                                        where d.acctno == item.AcctNo &&
                                        d.agrmtno == item.AgreementNo &&
                                        d.ItemID == item.ItemId &&
                                        d.stocklocn == item.StockLocation select d).Sum(d=> d.quantity));
                }
            }

            using (var ctx = Context.Create(conn, trans))
            {
                foreach (var item in mainItems)
                {

                    //#17287 - Only deliver any outstanding warranties where qty delivered matches line qty
                    var lineQty = (from l in ctx.LineItem
                                    where l.acctno == item.AcctNo &&
                                    l.agrmtno == item.AgreementNo &&
                                    l.ItemID == item.ItemId &&
                                    l.stocklocn == item.StockLocation select l.quantity).FirstOrDefault();

                    if (lineQty == item.Quantity)
                    {

                        var itemDelDate = Convert.ToDateTime((from d in ctx.Delivery        // #16831
                                                              where d.acctno == item.AcctNo &&
                                                              d.agrmtno == item.AgreementNo &&
                                                              d.ItemID == item.ItemId &&
                                                              d.stocklocn == item.StockLocation
                                                              select d.datedel).FirstOrDefault());

                        new WarrantyRepository().DeliverItem(conn, trans, item, itemDelDate.Date);  // #17506 
                    }

                }

                      //#18409 - Where Replacement and no warranties. Still need to redeem existing warranty
                if (mainItems.Count == 0 && replacement != null)
                {
                    InstantReplacementDetails replacementDet = InstantReplacementDetails.DeSerialise(replacement); 

                    
                       var warranty = (from r in ctx.WarrantyReplacementView
                                    where r.acctno == accountNo &&
                                          r.agrmtno == agrmtNo &&
                                          r.ItemID == replacementDet.ItemId &&
                                          r.stocklocn == replacementDet.StockLocn select r).OrderByDescending(r => r.ExchangeDate).FirstOrDefault();

                       new WarrantyRepository().ReturnWarranty(warranty.contractno,warranty.stocklocn,conn, trans, WarrantyStatus.Redeemeded);
                }

            }

          
        }

        // #17290 - Update Exchange Replacement ItemId
        public void UpdateExchangeReplaceItem(SqlConnection conn, SqlTransaction trans, string accountNo, int agrmtno, int origItemId, int stockLocn, int replaceItemId)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var exchange = ctx.Exchange.Where(e => e.AcctNo==accountNo && e.AgrmtNo==agrmtno && e.ItemID==origItemId && e.StockLocn==stockLocn).AnsiFirstOrDefault(ctx);

                exchange.ReplacementItemId = replaceItemId;

                ctx.SubmitChanges();

            }
        }

        //#18603 - CR15594
        public void UpdateReadyAssistContractDate(SqlConnection conn, SqlTransaction trans, string acctNo, int agreementNo)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var readyAssistDet = (from i in ctx.ReadyAssistDetails
                              where i.AcctNo == acctNo &&
                              i.AgrmtNo == agreementNo &&
                              i.RAContractDate == null &&                   //#19381
                              i.Status == null                              //#19381
                              select i).AnsiFirstOrDefault(ctx);

                //if (readyAssistDet != null && readyAssistDet.RAContractDate == null)
                if (readyAssistDet != null)                                 //#19381
                {
                    readyAssistDet.RAContractDate = DateTime.Today;
                }
         
                ctx.SubmitChanges();
            }

        }

        //#18605 - CR15594
        public void UpdateReadyAssistStatus(SqlConnection conn, SqlTransaction trans, string acctNo, int agreementNo, string status)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var readyAssistDet = (from i in ctx.ReadyAssistDetails
                                      where i.AcctNo == acctNo &&
                                      i.AgrmtNo == agreementNo &&
                                      (i.Status == null || i.Status == ReadyAssistStatus.Active)               //#19381
                                      select i).AnsiFirstOrDefault(ctx);

                if (readyAssistDet != null)
                {
                    readyAssistDet.Status = status;
                }

                ctx.SubmitChanges();
            }

        }

        //CR15594
        public void UpdateReadyAssistUnusedPortion(SqlConnection conn, SqlTransaction trans, string acctNo, int agreementNo, decimal unusedPortion)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var readyAssistDet = (from i in ctx.ReadyAssistDetails
                                      where i.AcctNo == acctNo &&
                                      i.AgrmtNo == agreementNo &&
                                      (i.Status == null || i.Status == ReadyAssistStatus.Active)  &&             //#19381
                                      i.UnusedPortion == null                           //#19381
                                      select i).AnsiFirstOrDefault(ctx);

                if (readyAssistDet != null)
                {
                    readyAssistDet.UnusedPortion = unusedPortion;
                }

                ctx.SubmitChanges();
            }

        }

        //#18603 - CR15594
        public bool IsReadyAssistContractDateSet(SqlConnection conn, SqlTransaction trans, string acctNo, int agreementNo)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var readyAssistDet = (from i in ctx.ReadyAssistDetails
                                      where i.AcctNo == acctNo &&
                                      i.AgrmtNo == agreementNo
                                      select i).AnsiFirstOrDefault(ctx);

                if (readyAssistDet.RAContractDate == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }

        }

        //#18603 - CR15594
        public bool IsReadyAssist(SqlConnection conn, SqlTransaction trans, string acctNo, int agreementNo)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var readyAssistDet = (from i in ctx.ReadyAssistDetails
                                      where i.AcctNo == acctNo &&
                                      i.AgrmtNo == agreementNo &&
                                      (i.Status == null || i.Status == ReadyAssistStatus.Active)            //#19381
                                      select i).AnsiFirstOrDefault(ctx);

                //if (readyAssistDet != null && (readyAssistDet.Status == null || readyAssistDet.Status == ReadyAssistStatus.Active))
                if (readyAssistDet != null)         //#19381
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        //#18603 - CR15594
        public void SaveRemoveReadyAssistDetails(SqlConnection conn, SqlTransaction trans,bool readyAssist, string acctNo, int agreementNo, int? readyAssistTermLength, int itemId, string contractNo)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var readyAssistDet = (from i in ctx.ReadyAssistDetails
                                      where i.AcctNo == acctNo &&
                                      i.AgrmtNo == agreementNo &&
                                      i.ItemId == itemId &&                     //#19381
                                      i.ContractNo == contractNo                //#19381
                                      select i).AnsiFirstOrDefault(ctx);

                if (readyAssist == false && readyAssistDet != null && (readyAssistDet.Status != ReadyAssistStatus.Active && readyAssistDet.Status != ReadyAssistStatus.Cancelled)) //#19459  //If no Ready Assist and previously inserted, then remove
                {
                    ctx.ReadyAssistDetails.DeleteOnSubmit(readyAssistDet);
                }

                if (readyAssistDet == null && readyAssist)
                {
                    ReadyAssistDetails ra = new ReadyAssistDetails
                    {
                        AcctNo = acctNo,
                        AgrmtNo = agreementNo,
                        RAContractDate = null,
                        RATermLength = readyAssistTermLength,
                        ItemId = itemId,
                        ContractNo = contractNo
                    };

                    ctx.ReadyAssistDetails.InsertOnSubmit(ra);
                }
                //else
                //{
                //    if (readyAssist)
                //    {
                //        readyAssistDet.RATermLength = readyAssistTermLength;   
                //    }
                 
                //}

                ctx.SubmitChanges();
            }

        }


        //#13779
        public DateTime? ReturnDateFirst (string acctno, DateTime datedel)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                DateTime? datefirst = null;
                
                var df =  new DBDateFirst(connection, transaction);

                df.ExecuteNonQuery(acctno, datedel, out datefirst);

                return datefirst;

            });
        }

        public SalesCommissionDetails GetSalesCommissionDetails(SqlConnection conn, SqlTransaction trans, int? branchNo, int? empeeNo, DateTime dateFrom, DateTime dateTo)
        {
            DataSet ds = new SalesCommissionEnquiryGetDetails(conn, trans).ExecuteDataSet(branchNo, empeeNo, dateFrom, dateTo);
            DataTable dt = ds.Tables[0];
            var totalCommission = 0m;

            if(ds.Tables[0].Rows.Count > 0)
            {
                totalCommission = (from commission in dt.AsEnumerable()
                                   select commission).Sum(s => (decimal)s["Total Commission Value"]);
            }

            return new SalesCommissionDetails
            {
            
                SalesCommissions = ds.Tables[0],
                TotalCommission = Math.Round(totalCommission,2)
            };

        }


        public SalesCommissionDetails GetBranchSalesCommissionDetails(SqlConnection conn, SqlTransaction trans, int? branchNo, DateTime dateFrom, DateTime dateTo)
        {
            DataSet ds = new SalesCommissionBranchEnquiryGetDetails(conn, trans).ExecuteDataSet(branchNo, dateFrom, dateTo);
            DataTable dt = ds.Tables[0];
            var totalCommission = 0m;
            var totalCommissionableValue = 0m;
            var totalProductCommissionValue = 0m;
            var totalTermsTypesCommissionValue = 0m;
            var totalWarrantyCommissionValue = 0m;

            if (ds.Tables[0].Rows.Count > 0)
            {
                totalCommission = (from commission in dt.AsEnumerable()
                                   select commission).Sum(s => (decimal)s["Total Commission"]);

                totalCommissionableValue = (from commission in dt.AsEnumerable()
                                            select commission).Sum(s => (decimal)s["Commission Valuable"]);

                totalProductCommissionValue = (from commission in dt.AsEnumerable()
                                               select commission).Sum(s => (decimal)s["Product Commission"]);

                totalTermsTypesCommissionValue = (from commission in dt.AsEnumerable()
                                                  select commission).Sum(s => (decimal)s["Terms Type Commission"]);

                totalWarrantyCommissionValue = (from commission in dt.AsEnumerable()
                                                select commission).Sum(s => (decimal)s["Warranty Commission"]);

            }

            return new SalesCommissionDetails
            {
                SalesCommissions = ds.Tables[0],
                TotalCommission = Math.Round(totalCommission,2),
                TotalCommissionableValue = Math.Round(totalCommissionableValue,2),
                TotalProductCommissionValue = Math.Round(totalProductCommissionValue,2),
                TotalTermsTypesCommissionValue = Math.Round(totalTermsTypesCommissionValue,2),
                TotalWarrantyCommissionValue = Math.Round(totalWarrantyCommissionValue,2)
            };

        }

        public int GetDiscountDeliveryMonths(string custid, int itemId, Context context = null, SqlConnection conn = null, SqlTransaction trans = null)
        {
            var monthsPassed = 0;

            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var discMonthsPassed = new DiscountMonthsPassedSinceDelivery(connection, transaction) { custid = custid, itemId = itemId, currentDate = DateTime.Today };

                discMonthsPassed.ExecuteNonQuery();

                monthsPassed = discMonthsPassed.monthsSinceDelivery.HasValue ? discMonthsPassed.monthsSinceDelivery.Value : -1;

            }, context, conn, trans);
            return monthsPassed;
        }

        //Write CashLoanDisbursement record when disbursing Cash Loan
        //This will be used by tri_delivery_insert as this trigger is fired when inserting
        //into Delivery table for CLD - Cash Loan Disbursement. Details to be written to Fintrans 
        //will be retrieved from CashLoanDisbursement table.
        public void CashLoanDisbursementWrite(SqlConnection conn, SqlTransaction tran, Blue.Cosacs.Shared.CashLoanDisbursementDetails cashLoanDisbursementDet, int agreementNo)      // #17692
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                ctx.CashLoanDisbursement.InsertOnSubmit(new CashLoanDisbursement
                {
                    CustId = cashLoanDisbursementDet.custId,
                    AcctNo = cashLoanDisbursementDet.accountNo,
                    AgrmtNo = agreementNo,
                    LoanAmount = cashLoanDisbursementDet.loanAmount,
                    DisbursementType = cashLoanDisbursementDet.disbursementType,
                    CardType = cashLoanDisbursementDet.cardType,
                    ChequeCardNo = cashLoanDisbursementDet.chequeCardNo,
                    Bank = cashLoanDisbursementDet.bankName,
                    BankAccountType = cashLoanDisbursementDet.bankAccountType,
                    BankBranch = cashLoanDisbursementDet.bankBranch,
                    BankAcctNo = cashLoanDisbursementDet.bankAccountNo,
                    Notes = cashLoanDisbursementDet.notes,
                    BankReferenceNo = cashLoanDisbursementDet.bankReferenceNo,
                    BankAccountName = cashLoanDisbursementDet.bankAccountName
                });

                ctx.SubmitChanges();

            }, conn: conn, trans: tran);
        }

        public CashLoanDisbursementDetailsView GetCashLoanDisbursementDetails (SqlConnection conn, SqlTransaction tran, string acctNo)
        {
            CashLoanDisbursementDetailsView details = null;
            Context.ExecuteTx((ctx, connection, transaction) =>
            {
               details = ctx.CashLoanDisbursementDetailsView.Where(c => c.AccountNumber == acctNo).FirstOrDefault();
            }, conn: conn, trans: tran);

            return details;
        }

        public void CashLoanDisbursementBankTransferSave(SqlConnection conn, SqlTransaction tran, string acctNo, string bankRefNo, DateTime transferDate)
        {
            using (var ctx = Context.Create(conn, tran))
            {
                var bankReferenceNoExists = (from i in ctx.CashLoanDisbursement
                                             where i.AcctNo != acctNo
                                             && i.BankTransferRefNo == bankRefNo
                                             select i).FirstOrDefault();

                if (bankReferenceNoExists != null)
                {
                    throw new STLException("This Bank Reference Number is already used against a different account");
                }
            }

            Context.ExecuteTx((asyncctx, connection, transaction) =>
            {
                var disbursementDet = (from i in asyncctx.CashLoanDisbursement
                                       where i.AcctNo == acctNo
                                       select i).FirstOrDefault();

                if (disbursementDet != null)
                {
                    disbursementDet.BankTransferRefNo = bankRefNo;
                    disbursementDet.BankTransferDate = transferDate;
                }


                asyncctx.SubmitChanges();

            }, conn: conn, trans: tran);
        }

        public bool IsItemScheduled(SqlConnection conn, SqlTransaction trans, int lineItemId)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var scheduled = (from ls in ctx.LineItemBookingSchedule
                                      where ls.LineItemID == lineItemId
                                      select ls).Sum(ls => ls.Quantity);

                if (scheduled != null & scheduled != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //#18603 - CR15594 - not needed as existing code delivers Ready Assist.
        ////public void DeliverReadyAssist(SqlConnection conn, SqlTransaction tran, string acctno, int agreementNo, int empeeno, string acctType) 
        ////{
        ////    Context.ExecuteTx((ctx, connection, transaction) =>
        ////    {

        ////        var countStock = (from l in ctx.LineItem
        ////                            where l.acctno == acctno &&
        ////                            l.agrmtno == agreementNo &&
        ////                            l.quantity > 0 && 
        ////                            l.itemtype == "S" &&
        ////                            !ctx.Delivery.Any(del => del.ItemID == l.ItemID && del.stocklocn == l.stocklocn) select l.ItemID).Count();

        ////        if (countStock > 1)
        ////        {
        ////            var ReadyAssistItems = (from l in ctx.LineItem
        ////                                    join s in ctx.StockInfo on l.ItemID equals s.Id
        ////                                    join c in ctx.Code on s.IUPC equals c.code
        ////                                    where c.category == "RDYAST" &&
        ////                                    l.acctno == acctno &&
        ////                                    l.agrmtno == agreementNo &&
        ////                                    l.quantity > 0 &&
        ////                                    (l.contractno != null || l.contractno != string.Empty) &&
        ////                                    !ctx.Delivery.Any(del => del.ItemID == l.ItemID && del.contractno == l.contractno)

        ////                                    select new
        ////                                    {
        ////                                        ReadyAssistItemId = l.ItemID,
        ////                                        ReadyAssistStockLocn = l.stocklocn,
        ////                                        ReadyAssistQuantity = int.Parse(l.quantity.ToString()),
        ////                                        ReadyAssistItemNo = l.itemno,
        ////                                        ReadyAssistOrdVal = l.ordval,
        ////                                        ReadyAssistContractNo = l.contractno
        ////                                    }).ToList();

        ////            var branches = ctx.Branch.ToDictionary(b => b.branchno);

        ////            ReadyAssistItems.ForEach(r =>
        ////            {

        ////                ctx.Delivery.InsertOnSubmit(new Delivery
        ////                {
        ////                    acctno = acctno,
        ////                    agrmtno = agreementNo,
        ////                    branchno = r.ReadyAssistStockLocn,
        ////                    buffbranchno = r.ReadyAssistStockLocn,
        ////                    buffno = branches[Convert.ToInt16(r.ReadyAssistStockLocn)].hibuffno + 1,
        ////                    contractno = r.ReadyAssistContractNo,
        ////                    transvalue = r.ReadyAssistOrdVal,
        ////                    datedel = DateTime.Today,
        ////                    stocklocn = r.ReadyAssistStockLocn,
        ////                    delorcoll = "D",
        ////                    ItemID = r.ReadyAssistItemId,
        ////                    ParentItemID = 0,
        ////                    ParentItemNo = "",
        ////                    transrefno = branches[Convert.ToInt16(r.ReadyAssistStockLocn)].hirefno + 1,
        ////                    datetrans = DateTime.Now,
        ////                    itemno = r.ReadyAssistItemNo,
        ////                    quantity = r.ReadyAssistQuantity,
        ////                    NotifiedBy = empeeno,
        ////                    ftnotes = "DNWA",
        ////                    BrokerExRunNo = 0,
        ////                    retitemno = string.Empty,
        ////                    runno = 0,
        ////                    RetItemID = 0
        ////                });

        ////                branches[Convert.ToInt16(r.ReadyAssistStockLocn)].hirefno++;
        ////                branches[Convert.ToInt16(r.ReadyAssistStockLocn)].hibuffno++;

        ////                var acct = (from a in ctx.Acct
        ////                            where a.acctno == acctno
        ////                            select a).AnsiFirstOrDefault(ctx);

        ////                acct.outstbal += Convert.ToDecimal(r.ReadyAssistOrdVal);

        ////            });
        ////        }
        ////        else
        ////        {
        ////            BDelivery nonStockDelivery = new BDelivery();
        ////            DBranch branch = new DBranch();
        ////            decimal transTotal = 0;

        ////            int transRefNo = branch.GetTransRefNo(short.Parse(acctno.Substring(0, 3)));

        ////            nonStockDelivery.DeliverNonStocks(conn, tran, acctno, acctType,
        ////            Country[CountryParameterNames.CountryCode].ToString(), short.Parse(acctno.Substring(0, 3)), transRefNo, ref transTotal, agreementNo);
        ////        }



        ////        ctx.SubmitChanges();


        ////    }, conn: conn, trans: tran);
        ////}

        // CR - Jyoti - Create Service Request for Installation items only after Payment is done
        // instead of creating it on line item sale on windows PoS.
        public bool GetAccountData(string accountNo, SqlConnection connection, SqlTransaction trans)
        {
            if (connection == null)
                connection = new SqlConnection(Connections.Default);

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            if (trans == null)
                trans = connection.BeginTransaction();

            using (var ctx = Context.Create(connection))
            {
                ctx.Transaction = trans;
                ctx.ObjectTrackingEnabled = false;
                var account = new Acct();

                var record = (from p in ctx.Agreement
                              where p.acctno == accountNo
                              select p).AnsiFirstOrDefault(ctx);

                account.Agreement = record ?? new Agreement();

                if (account.Agreement.holdprop == "Y")
                    return true;
                else
                    return false;
            }
        }
    }
}

