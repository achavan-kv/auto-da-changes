using System;
using FileHelpers;

namespace Blue.Cosacs.Report.Service
{
    [DelimitedRecord(",")]
    public class WeeklySummaryResult
    {
        public int Week
        {
            get;
            set;
        }
        public int Received
        {
            get;
            set;
        }
        public int Completed
        {
            get;
            set;
        }
        public int Outstanding
        {
            get;
            set;
        }
        public int AverageTAT
        {
            get;
            set;
        }
        public int CompletedWithin7Days
        {
            get;
            set;
        }
        public double SevenDayPercentage
        {
            get;
            set;
        }
        public int JobsMore20Days
        {
            get;
            set;
        }
        //public int AverageTimeOpen { get; set; }
    }
}
