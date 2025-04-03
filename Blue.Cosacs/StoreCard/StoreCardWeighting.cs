using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Extensions;

namespace Blue.Cosacs.StoreCardUtil
{
    public static class StoreCardWeighting
    {
        //public static double InterestGetWeightedAverage(List<fintranswithBalancesVW> fintransList, DateTime DateFrom, DateTime DateTo, DateTime DatePaymentDue)
        //{
        //    DateTime PreviousDay = DateFrom; int counter = 1; double balance = 0f;
        //    TimeSpan span; //decimal OutstandingInterest = 0; decimal previousInterest = 0;

        //    var lastdatetrans = fintransList.Last().datetrans;
        //    var fintrans = new List<fintranswithBalancesVW>();

        //    balance = Convert.ToDouble(fintransList[0].total - fintransList[0].transvalue); //opening balance as before first transaction in period

        //    //Need to add opening balance to weighted average
        //    span = fintransList[0].datetrans.Date - DateFrom;
        //    var ft2 = new fintranswithBalancesVW { transvalue = Convert.ToDecimal(balance), datetrans = DateFrom, dayswithBalance = span.TotalDays, Balance = Convert.ToDecimal(balance),
        //    total = Convert.ToDecimal(balance)};
        //    fintrans.Add(ft2);

        //    foreach (var ft in fintransList)
        //    {

        //        if ( ///ft.transtypecode != TransType.StoreCardPayment || ft.transtypecode != TransType.StoreCardRefund
        //             ft.datetrans < DateTo)
        //        {

        //            span = ft.datetrans.Date - PreviousDay;
        //            if (counter > 0)
        //            {
        //                fintrans[counter - 1].dayswithBalance = span.TotalDays;
        //            }

        //            if (ft.datetrans == lastdatetrans)
        //            {
        //                span = DateTo - lastdatetrans;
        //                ft.dayswithBalance = Math.Abs(span.TotalDays);
        //            }

        //            balance = balance + Convert.ToDouble(ft.transvalue);
        //            PreviousDay = ft.datetrans.Date;
        //            ft.total = Convert.ToDecimal(balance);
        //            // only transactions in period included in balance....
        //            if (ft.datetrans >= DateFrom && ft.datetrans < DateTo)
        //            {
        //                fintrans.Add(ft);
        //                counter++;
        //            }
        //        }
        //        else // if first transaction is delivery then we have to calc balance from before so take off
        //        {
        //            if (balance == 0)
        //                balance = Convert.ToDouble(ft.total - ft.transvalue);

        //            if (ft.datetrans == lastdatetrans) //last transaction check if balance 0 then if so balance should be 0 as paid within due date
        //                if (ft.total <= 0)
        //                    balance = 0;
        //        }
        //        //fintrans[counter] = ft;
        //    }

        //    if (balance > 0.01)  // if final balance zero then weighted balance 0
        //        return fintrans.WeightedAverage(record => Convert.ToDouble(record.total), record => record.dayswithBalance);
        //    else
        //        return 0;
        //}

        public static double WeightedAverage(List<fintranswithBalancesVW> fintransList, DateTime DateFrom, DateTime DateTo)
        {
            var length = (DateTo - DateFrom);
            if (length.Days > 0)
            {
                var balance = fintransList.Where(f => f.datetrans < DateFrom).Sum(f => f.transvalue);
                var startdays = DateFrom.TotalDays();
                var runningtotal = 0M;
                var wtotal = 0M;

                for (var i = 0; i <= length.Days; i++)
                {
                    runningtotal += fintransList.Where(f => f.datetrans.TotalDays() - startdays == i).Sum(f => f.transvalue);
                    wtotal += runningtotal;
                }
                return Convert.ToDouble(wtotal / (length.Days + 1) + balance);
            }
            return 0f;
        }

        ///// <summary>
        ///// Returns an enumeration of dates with "dateTo" not inclusive.
        ///// </summary>
        //public static IEnumerable<DateTime> Days(DateTime dateFrom, DateTime dateTo)
        //{
        //    dateTo = dateTo.Date; // ignore the Time part
        //    var current = dateFrom.Date;

        //    while (current < dateTo)
        //    {
        //        yield return current;
        //        current = current.AddDays(1.0);
        //    }
        //}
    }
}
