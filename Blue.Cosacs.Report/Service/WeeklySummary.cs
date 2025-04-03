using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Blue.Cosacs.Report.Service;

namespace Blue.Cosacs.Report
{
    public partial class WeeklySummary
    {
        internal static IList<WeeklySummaryResult> Fill(int firstMonthYear, int firstDayYear, int year, string productgroup, int firstWeek, int lastWeek)
        {
            var ds = new DataSet();
            var ws = new WeeklySummary();

            ws.Fill(ds, firstMonthYear, firstDayYear, year, productgroup, firstWeek, lastWeek);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new WeeklySummaryResult()
                    {
                        Week = Convert.ToInt32(p["week"]),
                        Received = Convert.ToInt32(p["Received"]),
                        Completed = Convert.ToInt32(p["Completed"]),
                        Outstanding = Convert.ToInt32(p["Outstanding"]),
                        AverageTAT = Convert.ToInt32(p["AverageTAT"]),
                        CompletedWithin7Days = Convert.ToInt32(p["CompletedWithin7Days"]),
                        SevenDayPercentage = Convert.ToDouble(p["SevenDayPercentage"]),
                        JobsMore20Days = Convert.ToInt32(p["JobsMore20Days"]),
                        //AverageTimeOpen = Convert.ToInt32(p["AverageTimeOpen"])
                    })
                    .ToList();
            }

            return new List<WeeklySummaryResult>();
        }
    }
}
