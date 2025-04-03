using Blue.Cosacs.Report.Olap;
using System.Text;
using System;
using Blue.Cosacs.Report.Xml;
using System.Collections.Generic;

namespace Blue.Cosacs.Report.Service
{
    public class WeeklySummarySqlRunner
    {
        public IList<WeeklySummaryResult> Run(int firstMonthYear, int firstDayYear, int year, string productgroup, int firstWeek, int lastWeek)
        {
            return WeeklySummary.Fill(firstMonthYear, firstDayYear, year, productgroup, firstWeek, lastWeek);
        }
    }
}
