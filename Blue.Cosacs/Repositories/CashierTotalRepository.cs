using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Services.Branch;
using STL.BLL;
using STL.Common;
using Blue.Cosacs.Cashier;
using Blue.Cosacs.Shared.Extensions;
using STL.DAL;
using System.Configuration;

namespace Blue.Cosacs.Repositories
{
    public class CashierTotalRepository
    {
        public GetCashierTotalsResponse LoadCashierTotals(DateTime datefrom, int empeeno, DateTime dateto, SqlConnection conn = null, SqlTransaction trans = null, Context context = null)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
              {
                  //Summaries totals
                  var viewSummary = (from c in ctx.CashierTotalsView
                                     where c.datetrans > datefrom && c.datetrans < dateto
                                     && c.empeeno == empeeno
                                     group c by new { c.paymethod, c.codedescript } into g
                                     select new CashierTotalsView
                                     {
                                         codedescript = g.Key.codedescript,
                                         paymethod = g.Key.paymethod.Value,
                                         foreigntender = g.Sum(x => x.foreigntender),
                                         localchange = g.Sum(x => x.localchange),
                                         Payments = g.Sum(x => x.Payments.HasValue ? x.Payments.Value : 0),
                                         Corrections = g.Sum(x => x.Corrections.HasValue ? x.Corrections.Value : 0),
                                         Refunds = g.Sum(x => x.Refunds.HasValue ? x.Refunds.Value : 0),
                                         PettyCash = g.Sum(x => x.PettyCash.HasValue ? x.PettyCash.Value : 0),
                                         Remittance = g.Sum(x => x.Remittance.HasValue ? x.Remittance.Value : 0),
                                         Allocation = g.Sum(x => x.Allocation.HasValue ? x.Allocation.Value : 0),
                                         Disbursement = g.Sum(x => x.Disbursement.HasValue ? x.Disbursement.Value : 0),
                                         NetReceipts = g.Sum(x => x.NetReceipts.HasValue ? x.NetReceipts.Value : 0)
                                         //   AllowOS = g.Any(x => x.AllowOS.HasValue ? x.AllowOS.Value : false)
                                     }).AnsiToList(ctx);

                 //Get all paymethods used.
                  var paymethods = viewSummary.Where(c => c.paymethod.HasValue).Select(x => x.paymethod.Value.ToString());

                  //Get all paymethods 
                  var codes = (from c in ctx.Code
                               where c.category == "FPM" && c.code != "0"
                               orderby c.sortorder
                               select c).AnsiToList(ctx);

                  //Add missing methods to list.
                  viewSummary = viewSummary.Concat(from c in codes
                                                   where !paymethods.Contains(c.code)
                                                   select new CashierTotalsView
                                                   {
                                                       codedescript = c.codedescript,
                                                       paymethod = Convert.ToInt16(c.code),
                                                       foreigntender = 0m,
                                                       localchange = 0m,
                                                       Payments = 0m,
                                                       Corrections = 0m,
                                                       Refunds = 0m,
                                                       PettyCash = 0m,
                                                       Remittance = 0m,
                                                       Allocation = 0m,
                                                       Disbursement = 0m,
                                                       NetReceipts = 0m,
                                                       AllowOS = true
                                                   }).ToList();

                  //Update pay type with the change from non local currencies.
                  var cash = viewSummary.Find(v => v.paymethod == 1);
                  viewSummary.Where(s => s.localchange != 0).ToList().ForEach(l =>
                  {
                      cash.NetReceipts -= l.localchange;
                      cash.Payments -= l.localchange;
                  });


                  //Add restriction to overage and shortage.
                  codes.ForEach(c =>
                  {
                      viewSummary.Where(v => v.paymethod.ToString() == c.code).ToList().ForEach(v =>
                          {
                              v.AllowOS = c.Additional2.Length == 0 || c.Additional2.Substring(0,1) != "0";
                          });
                  });

                  //Get all exchange rates for later use.
                  var exchange = ctx.ExchangeRate.Where(x => x.Status == "C").ToList();

                  return new GetCashierTotalsResponse { Cashiertotals = viewSummary, ExchangeRates = exchange };
              }, context, conn, trans);
        }


        public List<CashierTotalledView> LoadCashiersTotalled(int empeeno, DateTime datefrom, DateTime dateto, SqlConnection conn = null, SqlTransaction trans = null, Context context = null)
        {
            var Li = new List<CashierTotalledView>();

            Context.ExecuteTx((ctx, connection, transaction) =>
            {

                var Cts = (from c in ctx.CashierTotalledView
                           where c.datefrom
                           >= datefrom && c.dateto <= dateto
                           && c.empeeno == empeeno
                           select c).AnsiToList(ctx);
                var counter = 0;
                // removing and summing duplicates
                foreach (var total in Cts)
                {
                    if (counter > 0)
                    {

                        if (Cts[counter - 1].paymethod == total.paymethod)
                        {
                            Cts[counter].Corrections = Convert.ToDecimal(Cts[counter - 1].Corrections) + Convert.ToDecimal(total.Corrections);
                            Cts[counter].Refunds = Convert.ToDecimal(Cts[counter - 1].Refunds) + Convert.ToDecimal(total.Refunds);
                            Cts[counter].PettyCash = Convert.ToDecimal(Cts[counter - 1].PettyCash) + Convert.ToDecimal(total.PettyCash);
                            Cts[counter].Remittances = Convert.ToDecimal(Cts[counter - 1].Remittances) + Convert.ToDecimal(total.Remittances);
                            Cts[counter].Disbursements = Convert.ToDecimal(Cts[counter - 1].Disbursements) + Convert.ToDecimal(total.Disbursements);
                            Cts[counter].Allocations = Convert.ToDecimal(Cts[counter - 1].Allocations) + Convert.ToDecimal(total.Allocations);
                            Cts[counter].NetReceipts = Convert.ToDecimal(Cts[counter - 1].NetReceipts) + Convert.ToDecimal(total.NetReceipts);
                            Cts[counter].Payments = Convert.ToDecimal(Cts[counter - 1].Payments) + Convert.ToDecimal(total.Payments);


                        }
                        else // its different paymethod so add previous row to new array...
                        {
                            Li.Add(Cts[counter - 1]);
                        }


                    }
                    counter++;

                }
                // at end of loop add last row...
                if (counter > 0)
                    Li.Add(Cts[counter - 1]);

                Li.OrderBy(c => c.paymethod);
                //            Li.Sort(c => c.paymethod);
                //Cashiertotals.GroupBy(po =>1).Select( cashgroup => new CashierTotalsView
                // {
                //     Allocation = cashgroup.Sum(po => po.Allocation),

                // }

            }, context, conn, trans);

            return Li;
        }



        public void CashierTotalsSave(CashierTotalsSaveRequest request, SqlConnection conn = null, SqlTransaction trans = null, Context context = null)
        {
            conn = new SqlConnection(STL.DAL.Connections.Default);
            conn.Open();
            trans = conn.BeginTransaction();



            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var person =
                 //(from c in ctx.UserView
                   (from c in ctx.CourtsPersonTable         //IP - 23/01/13 - #12088
                  where c.UserId == request.employee
                  select c).AnsiFirstOrDefault(ctx);
                person.datelstaudit = request.dateto;


                var ct = new CashierTotals
                {
                    branchno = request.branch,
                    datefrom = request.datefrom,
                    dateto = request.dateto,
                    deposittotal = request.DepositTotal,
                    difference = request.TotalDifference,
                    empeeno = request.employee,
                    empeenoauth = request.authorisdedby,
                    runno = 0,
                    systemtotal = request.SystemTotal,
                    usertotal = request.UserTotal
                };

                ctx.CashierTotals.InsertOnSubmit(ct);
                ctx.SubmitChanges();



                foreach (var total in request.breakdown)
                {
                    total.cashiertotalid = ct.id;
                    //if ((Convert.ToDecimal(total.systemtotal) != 0 || Convert.ToDecimal(total.usertotal) != 0) && total.reason == null)     // #9397 jec 09/01/12
                    if ((Convert.ToDecimal(total.systemtotal) != 0 || Convert.ToDecimal(total.usertotal) != 0 || Convert.ToDecimal(total.deposit) != 0      // #9397 jec 09/01/12
                               || Convert.ToDecimal(total.securitisedtotal) != 0 || (total.Payments.HasValue && Convert.ToDecimal(total.Payments) != 0)     //IP - 24/05/12 - #10127 - LW74978 - Check for other totals
                               || (total.Corrections.HasValue && Convert.ToDecimal(total.Corrections) != 0) || (total.Refunds.HasValue && Convert.ToDecimal(total.Refunds) != 0)
                               || (total.PettyCash.HasValue && Convert.ToDecimal(total.PettyCash) != 0) || (total.Remittances.HasValue && Convert.ToDecimal(total.Remittances) != 0)
                               || (total.Allocations.HasValue && Convert.ToDecimal(total.Allocations) != 0) || (total.Disbursements.HasValue && Convert.ToDecimal(total.Disbursements) != 0)) && total.reason == null)
                        total.reason = "";

                    if (total.reason != null)
                        ctx.CashierTotalsBreakdown.InsertOnSubmit(total);
                }

                //ctx.CashierTotalsBreakdown.InsertAllOnSubmit(request.breakdown);
                var countryCode = CountryParameterCache.GetCountryParameter<string>(CountryParameterNames.CountryCode);
                ctx.SubmitChanges();
                var payment = new BPayment();
                payment.User = Convert.ToInt32(request.employee);
                payment.SaveCashierTotalNew(conn, trans, request.datefrom, request.dateto, request.employee, 0, request.authorisdedby,
                     request.canReverse, request.UserTotal, request.SystemTotal, request.TotalDifference, request.DepositTotal, request.branch, countryCode, request.breakdown);

                //IP - 12/12/11 - #8813 - CR1234
                Dictionary<string, object> cashierAudit = new Dictionary<string, object>();

                cashierAudit.Add("Cashier", request.employee);
                cashierAudit.Add("Date From", request.datefrom);
                cashierAudit.Add("Date To", request.dateto);
                cashierAudit.Add("User Total", request.UserTotal);
                cashierAudit.Add("System Total", request.SystemTotal);
                cashierAudit.Add("Difference", request.TotalDifference);
                cashierAudit.Add("Deposits", request.DepositTotal);

                EventStore.Instance.Log(cashierAudit, "CashierTotalsSave", EventCategory.CashierTotals);

                trans.Commit();
            }, context, conn, trans);
        }


        private class BranchTrans
        {
            public short BranchNo { get; set; }
            public int HiRefNo { get; set; }
            public int Count { get; set; }
        }
        public string CashierTotalWriteOff()
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
             {
                 ctx.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["commandTimeout"]);

                 var codes = new string[] { "WriteOffAccount", "MaxTimeLimit", "CashierMaxOverage", "CashierMaxShortage" };
                 var cparams = ctx.CountryMaintenance.Where(c => codes.Contains(c.CodeName)).ToDictionary(c => c.CodeName);
                 var runNo = (from i in ctx.InterfaceControl
                              where i.Interface == "CASHIERWO"
                              select (int?)i.RunNo).Max();

                 var lastwriteoff = (from w in ctx.CashierTotalWriteOff
                                     select (DateTime?)w.WriteOffDate).Max();

                 var cWriteOff = new CashierWriteOff(cparams, lastwriteoff, DateTime.Now);
                 runNo = runNo.HasValue ? runNo.Value : 1;

                 if (cWriteOff.endDate > cWriteOff.lastWriteOff)
                 {
                     var toAccount = cparams["WriteOffAccount"].Value;
                     var toAccountBranchNo = Convert.ToInt16(toAccount.Substring(0, 3));

                     var summmary = (from wf in ctx.CashierTotalWriteOffView
                                     where wf.datetrans > cWriteOff.lastWriteOff && wf.datetrans <= cWriteOff.endDate
                                     select wf).AnsiToList(ctx);

                     var accounts = cWriteOff.GetAccounts(summmary);

                     // all account numbers
                     var accountNos = accounts.Select(a => a.Account).Concat(new[] { toAccount }).ToList();

                     // gives me the count of accounts per branch that need to be updated
                     var branch = from ab in
                                      (from accountNo in accountNos
                                       select new
                                       {
                                           branchNo = accountNo.ConvertToBranchNo(),
                                           accountNo = accountNo
                                       })
                                  group ab by ab.branchNo into b
                                  select new BranchTrans
                                  {
                                      BranchNo = b.Key.Value,
                                      Count = b.Count(),
                                      HiRefNo = 0
                                  };

                     var blookup = branch.ToDictionary(b => b.BranchNo);

                     foreach (var b in blookup)
                         b.Value.HiRefNo = (int)new BranchIncrementHiRefNo(connection, transaction) { branchno = b.Key, IncrementBranchNo = b.Value.Count }.ExecuteScalar();

                     var empeenos = accounts.Select(a => a.UserID).ToArray();

                     var courtsperson = (from c in ctx.UserView
                                         where empeenos.Contains(c.Id)
                                         select new { c.Id, c.LastName }).ToDictionary(d => d.Id);

                     foreach (var account in accounts)
                     {
                         var acctTransRefNo = blookup[account.Account.ConvertToBranchNo().Value].HiRefNo++;
                         var toAcctTransRefNo = blookup[toAccountBranchNo].HiRefNo++;
                         // Using DFinTrans as there is a linq error(microsoft) when using .InsertAllOnSubmit and table has a identity column    #9703 jec 27/02/12 
                         // From Account....
                         new DFinTrans
                         {
                             AccountNumber = account.Account,
                             TransRefNo = acctTransRefNo,
                             DateTrans = DateTime.Now,
                             TransTypeCode = "BEX",
                             TransValue = -account.Amount,
                             EmployeeNumber = -117,
                             TransPrinted = "Y",
                             TransUpdated = "Y",
                             ChequeNumber=toAccount,
                             FTNotes = runNo.Value.ToString(),
                             Source = "COSACS",
                             PayMethod = 0,
                             RunNumber = 0,
                             Agrmtno = 1,
                             BranchNumber = blookup[account.Account.ConvertToBranchNo().Value].BranchNo
                         }.Write(connection, transaction);

                         // To Account....
                         new DFinTrans
                         {
                             AccountNumber = toAccount,
                             TransRefNo = toAcctTransRefNo,
                             DateTrans = DateTime.Now,
                             TransTypeCode = "BEX",
                             TransValue = account.Amount,
                             EmployeeNumber = -117,
                             TransPrinted = "Y",
                             TransUpdated = "Y",
                             ChequeNumber = account.Account,
                             FTNotes = runNo.Value.ToString(),
                             Source = "COSACS",
                             PayMethod = 0,
                             RunNumber = 0,
                             Agrmtno = 1,
                             BranchNumber = blookup[toAccountBranchNo].BranchNo
                         }.Write(connection, transaction);

                        // ctx.FinTrans.InsertAllOnSubmit(
                        // new[] 
                        //{                            
                        //new FinTrans(account.Account,blookup[account.Account.ConvertToBranchNo().Value].BranchNo,-account.Amount,acctTransRefNo,toAccount,runNo.Value),
                        //new FinTrans(toAccount,blookup[toAccountBranchNo].BranchNo, account.Amount,toAcctTransRefNo,account.Account,runNo.Value),
                        //});

                         ctx.Finxfr.InsertAllOnSubmit(
                             new[] {
                                        new Finxfr { acctname = account.Amount > 0? courtsperson[account.UserID].LastName : String.Format("SUNDRY CREDIT {0}", account.Account.ConvertToBranchNo().Value),
                                                     acctno = account.Account,
                                                     acctnoxfr = toAccount,
                                                     agrmtno = 1, 
                                                     datetrans = DateTime.Now,
                                                     transrefno = acctTransRefNo,
                                                     origbr = blookup[account.Account.ConvertToBranchNo().Value].BranchNo
                                                    },
                                         new Finxfr { 
                                                     acctname = "CashierWriteOff",
                                                     acctno = toAccount,
                                                     acctnoxfr = account.Account,
                                                     agrmtno = 1, 
                                                     datetrans = DateTime.Now,
                                                     transrefno = toAcctTransRefNo,
                                                     origbr = blookup[toAccountBranchNo].BranchNo
                                                    }
                                    });
                     }
                     cWriteOff.GetEndDates().ToList().ForEach(d => ctx.CashierTotalWriteOff.InsertOnSubmit(new CashierTotalWriteOff
                                                                   {
                                                                       RunNo = runNo.Value,
                                                                       WriteOffDate = d
                                                                   }));
                 }
                 ctx.SubmitChanges();
                 return "P";
             });
        }
    }
}
