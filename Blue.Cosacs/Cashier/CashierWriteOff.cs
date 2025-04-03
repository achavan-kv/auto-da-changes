using System;
using System.Collections.Generic;
using System.Linq;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Extensions;


namespace Blue.Cosacs.Cashier
{
    public class CashierWriteOff
    {

        public class WriteOffAccount
        {
            public DateTime Date { get; set; }
            public int UserID { get; set; }
            public string Account { get; set; }
            public decimal Amount { get; set; }
            public DateTime PeriodEnd { get; set; }
        }

        private readonly decimal MaxOverage;
        private readonly decimal MaxShortage;
        private readonly string limit;
        private readonly bool perDay;
        public readonly DateTime lastWriteOff;
        public readonly DateTime endDate;

        public CashierWriteOff(IDictionary<string, CountryMaintenance> cparams, DateTime? lastwriteoff, DateTime currentDate)
        {
            this.MaxOverage = Convert.ToDecimal(cparams["CashierMaxOverage"].Value);
            //this.MaxShortage = -Convert.ToDecimal(cparams["CashierMaxShortage"].Value);
            this.MaxShortage = Convert.ToDecimal(cparams["CashierMaxShortage"].Value);          //IP - 27/09/12 - #10483 - LW751054
            this.limit = cparams["MaxTimeLimit"].Value.ToUpper();
            this.perDay = limit == CashierWriteLimits.DayUpper;
            this.lastWriteOff = lastwriteoff.HasValue ? lastwriteoff.Value.RoundToDay() : InitalizeWriteOff(currentDate.RoundToDay());
            this.endDate = CalculateEndDate(currentDate.RoundToDay());
        }

        public DateTime InitalizeWriteOff(DateTime currentDate)
        {
            if (perDay)
                return currentDate.AddDays(-1);

            if ((currentDate.DayOfWeek == DayOfWeek.Saturday && limit == CashierWriteLimits.WeekSatUpper) ||
                (currentDate.DayOfWeek == DayOfWeek.Sunday && limit == CashierWriteLimits.WeekSunUpper))
                return currentDate.AddDays(-7);
            else
            {
                if (limit == CashierWriteLimits.WeekSunUpper)
                    return currentDate.AddDays(-7 - (int)currentDate.DayOfWeek);
                else
                    return currentDate.AddDays(-7 - (int)currentDate.DayOfWeek - 1);
            }
        }

        //Moving end day to correct previous end date.
        public DateTime CalculateEndDate(DateTime currentDate)
        {
            if ((currentDate.DayOfWeek == DayOfWeek.Saturday && limit == CashierWriteLimits.WeekSatUpper) ||
                (currentDate.DayOfWeek == DayOfWeek.Sunday && limit == CashierWriteLimits.WeekSunUpper) ||
                  perDay)
                return currentDate;
            else
            {
                if (limit == CashierWriteLimits.WeekSunUpper)
                    return currentDate.AddDays(-(int)currentDate.RoundToDay().DayOfWeek);
                else
                    return currentDate.AddDays(-(int)currentDate.RoundToDay().DayOfWeek - 1);
            }

        }

        public class Interval
        {
            public DateTime start, end;
        }

        //End date is always on the correct day.
        public static IEnumerable<Interval> Intervals(bool perDay, DateTime start, DateTime end)
        {
            var addition = perDay ? 1 : 7;
            var days = end.Subtract(start).Days % 7;

            var intervals = (from i in Enumerable.Range(0, (int)end.Subtract(start).TotalDays / addition)
                             select new Interval
                             {
                                 start = end.AddDays((i + 1) * -addition),
                                 end = end.AddDays(i * -addition)
                             });

            
            if (!perDay && days > 0)
            {
               intervals = intervals.Concat(new[] { 
                    new Interval { start = start, end = intervals.Count() > 0?intervals.Min(i => i.start):end }});
            }
            return intervals;
        }

        private class SummaryTotal
        {
            public int cashierID;
            public decimal total;
        }

        private static IEnumerable<SummaryTotal> Total(IEnumerable<CashierTotalWriteOffView> summary, DateTime start, DateTime end)
        {
            return from c in summary
                   where c.datetrans >= start && c.datetrans < end
                   group c by c.empeeno into g
                   select new SummaryTotal
                   {
                       cashierID = g.Key,
                       total = g.Sum(s => s.difference)
                   };
        }

        public IEnumerable<DateTime> GetEndDates()
        {
            var intervals = Intervals(perDay, lastWriteOff, endDate);
            return intervals.Select(i => i.end);
        }

        public IEnumerable<WriteOffAccount> GetAccounts(List<CashierTotalWriteOffView> summary)
        {
            var intervals = Intervals(perDay, lastWriteOff, endDate);

            return from i in intervals
                   from totals in Total(summary, i.start, i.end)
                   join cT in summary on totals.cashierID equals cT.empeeno
                   where cT.datetrans >= i.start
                      && cT.datetrans < i.end
                      //&& ((totals.total < MaxOverage && totals.total > 0) || (totals.total > MaxShortage && totals.total < 0))
                       //IP - 27/09/12 - #10483 - LW751054 - CashierTotalsWriteOff stores negative for Overage and positive for Shortage
                       && ((Math.Abs(totals.total) < MaxOverage && totals.total < 0) || (totals.total < MaxShortage && totals.total > 0))    
                   select new WriteOffAccount
                   {
                       Account = cT.acctno,
                       UserID = totals.cashierID,
                       Date = cT.datetrans,
                       Amount = cT.difference
                   };
        }
    }
}
