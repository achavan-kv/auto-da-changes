using System.Linq;
using Blue.Cosacs.Shared;
using System.Collections;
using System.Collections.Generic;
using System;
using STL.Common;
using STL.Common.Constants.FTransaction;
using System.ServiceProcess;
using System.Data;

namespace Blue.Cosacs.Repositories
{
    public class EODRepository
    {
        public void UpdateConfigurationOption(string configName, string optionCode, int runNo)
        {
            using (var context = Context.Create())
            {
                var option = context.EodConfigurationOption.
                                SingleOrDefault(r =>  r.OptionCode == optionCode);
                option.ReRunNo = runNo;

                context.SubmitChanges(); 
            }
        }

        public int GetLastRunNo(string name)
        {
            using (var context = Context.Create())
            {
                return context.InterfaceControl
                        .Where(i => i.Interface == name)
                        .OrderByDescending(i => i.DateStart)
                        .Select(i => i.RunNo)
                        .AnsiFirstOrDefault(context);
            }
        }

        public int GetMaxRunNo(string name)
        {
            //HACK: Max(), DefaultIfEmpty() will fail on empty collection in L2S
            using (var context = Context.Create())
            {
                return context.InterfaceControl
                        .Where(i => i.Interface == name)
                        .OrderByDescending(i => i.RunNo)
                        .Select(i => i.RunNo)
                        .AnsiFirstOrDefault(context);                        
            }
        }

        public InterfaceControl GetRunLeastRecent(string name, int? runNo = null, bool passedOnly = false)
        {
            using (var context = Context.Create())
            {
                return context.InterfaceControl
                        .Where(i => i.Interface == name)
                        .WhereIf(runNo.HasValue, i => i.RunNo == runNo.Value)
                        .WhereIf(passedOnly, i => i.Result == "P")
                        .OrderBy(i => i.DateStart)
                        .AnsiFirstOrDefault(context);
            }
        }

        public InterfaceControl GetRunMostRecent(string name, int? runNo = null, bool passedOnly = false)
        {
            using (var context = Context.Create())
            {
                return context.InterfaceControl
                        .Where(i => i.Interface == name)
                        .WhereIf(runNo.HasValue, i => i.RunNo == runNo.Value)
                        .WhereIf(passedOnly, i => i.Result == "P")
                        .OrderByDescending(i => i.DateStart)
                        .AnsiFirstOrDefault(context);
            }
        }

        //IP - 12/08/11 - Weekly Trading Report - Retrieves the dates 
        public WTRDates WTRDatesGet()
        {
            using (var context = Context.Create())
            {
                return context.WTRDates.AnsiFirstOrDefault(context);
            }
        }

        //IP - 12/08/11 - Weekly Trading Report - Saves the dates
        public void WTRDatesSave(WTRDates wtrDates)
        {
            using (var context = Context.Create())
            {
                var Dates = context.WTRDates.AnsiFirstOrDefault(context);

                Dates.DtActive1 = wtrDates.DtActive1;
                Dates.DtStartCY1 = wtrDates.DtStartCY1;
                Dates.DtEndCY1 = wtrDates.DtEndCY1;
                Dates.DtStartLY1 = wtrDates.DtStartLY1;
                Dates.DtEndLY1 = wtrDates.DtEndLY1;
                Dates.DtFilename1 = wtrDates.DtFilename1;

                Dates.DtActive2 = wtrDates.DtActive2;
                Dates.DtStartCY2 = wtrDates.DtStartCY2;
                Dates.DtEndCY2 = wtrDates.DtEndCY2;
                Dates.DtStartLY2 = wtrDates.DtStartLY2;
                Dates.DtEndLY2 = wtrDates.DtEndLY2; ;
                Dates.DtFilename2 = wtrDates.DtFilename2;

                Dates.DtActive3 = wtrDates.DtActive3;
                Dates.DtStartCY3 = wtrDates.DtStartCY3;
                Dates.DtEndCY3 = wtrDates.DtEndCY3; ;
                Dates.DtStartLY3 = wtrDates.DtStartLY3;
                Dates.DtEndLY3 = wtrDates.DtEndLY3;
                Dates.DtFilename3 = wtrDates.DtFilename3;

                Dates.DtActive4 = wtrDates.DtActive4;
                Dates.DtStartCY4 = wtrDates.DtStartCY4;
                Dates.DtEndCY4 = wtrDates.DtEndCY4;
                Dates.DtStartLY4 = wtrDates.DtStartLY4;
                Dates.DtEndLY4 = wtrDates.DtEndLY4;
                Dates.DtFilename4 = wtrDates.DtFilename4;

                Dates.DtActive5 = wtrDates.DtActive5;
                Dates.DtStartCY5 = wtrDates.DtStartCY5;
                Dates.DtEndCY5 = wtrDates.DtEndCY5;
                Dates.DtStartLY5 = wtrDates.DtStartLY5;
                Dates.DtEndLY5 = wtrDates.DtEndLY5;
                Dates.DtFilename5 = wtrDates.DtFilename5;

                context.SubmitChanges(); 
            }
        }

        public void WriteoffShortages(DateTime Datefrom, DateTime DateTo)
        {
            var CashierTotalsAccount = Convert.ToString(CountryParameterCache.GetCountryParameter<string>(CountryParameterNames.CashierTotalsAccount));
            var CashierTotalsWOFrequency = CountryParameterCache.GetCountryParameter<int>(CountryParameterNames.CashierTotalsWOFrequency);

            var datefrom= Convert.ToDateTime("1-jan-2010");
            if (CashierTotalsAccount.Length==12) //valid account number
            using (var ctx = Context.Create())
            {
                      var Shortages = ctx.FinTrans.Where(f => f.datetrans > datefrom && f.datetrans <DateTo && 
                          f.transtypecode == TransType.Shortage
                    ).Sum(f => f.transvalue);


                    new StoreCardRepository().AddTransaction(CashierTotalsAccount, Convert.ToDouble(-Shortages), DateTime.Now, TransType.ShortageWriteoff,"", context: ctx);
                 
            }

              

        }

        //#12156 - Check if Windows Service exists
        public bool CheckServiceExists(string serviceName)
        {
            using (var context = Context.Create())
            {
                ServiceController ctl = ServiceController.GetServices().Where(s => s.ServiceName == serviceName).FirstOrDefault();

                if (ctl == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        //#12341 - CR11571
        public bool CheckToAddSCStatementsOption()
        {
            var addJob = false;

            using (var ctx = Context.Create())
            {
                var maxDays = CountryParameterCache.GetCountryParameter<int>(CountryParameterNames.StoreCardMaxDaysEODRun);

                var interfaceControl = ctx.InterfaceControl.Where(i => i.Interface == "STStatements" && i.Result == "P").OrderByDescending(e => e.RunNo).FirstOrDefault();

                if(interfaceControl != null)
                {
                    var lastRunDate = interfaceControl.DateStart;
                    var currentDate = DateTime.Now;

                    var days = Math.Round((currentDate - lastRunDate).TotalDays, 0);

                    if (days >= maxDays) //#12426
                    {
                        addJob = true;
                    }
                    else
                    {
                        addJob = false;
                    }
                }

                return addJob;
            }
 
        }

        //#12341 - CR11571
        public void RemoveOption(string configurationName, string optionCode)
        {
            using (var ctx = Context.Create())
            {
                var record = ctx.EodConfigurationOption.Where(e => e.ConfigurationName == configurationName & e.OptionCode == optionCode).FirstOrDefault();

                if (record != null)
                {
                    ctx.EodConfigurationOption.DeleteOnSubmit(record);
                    ctx.SubmitChanges();
                }
                
            }
        }

        public DataTable OnlineProductExport()
        {
            var onlineProducts = new OnlineProductExportSP();
            var ds = onlineProducts.ExecuteDataSet();
            return ds.Tables[0];
        }

        public DataTable IneligibleOnlineProductExport()
        {
            var ineligibleProducts = new OnlineProductIneligibleSP();
            var ds = ineligibleProducts.ExecuteDataSet();
            return ds.Tables[0];
        }

        //#13719
        public DataTable ReadyAssistExport()
        {
            var readyAssistContracts = new ReadyAssistExportSP();
            var ds = readyAssistContracts.ExecuteDataSet();
            return ds.Tables[0];
        }
    }
}
