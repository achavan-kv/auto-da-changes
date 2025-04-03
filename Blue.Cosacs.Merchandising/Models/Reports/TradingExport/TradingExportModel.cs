using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace Blue.Cosacs.Merchandising.Models
{
    [DelimitedRecord(",")]
    public class TradingExportModel
    {
        public string SortOrder { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string DepartmentCode { get; set; }
        public string Class { get; set; }
        public string ClassCode { get; set; }
        public string Branch { get; set; }
        public string BranchName { get; set; }
        public string SalesType { get; set; }
        public string DayActualValue { get; set; }
        public string DayActualValueLY { get; set; }
        public string DayVariance { get; set; }
        public string DayActualGP { get; set; }
        public string DayActualGPLY { get; set; }
        public string DayVarianceGP { get; set; }
        public string WeekActualValue { get; set; }
        public string WeekActualValueLY { get; set; }
        public string WeekVariance { get; set; }
        public string WeekActualGP { get; set; }
        public string WeekActualGPLY { get; set; }
        public string WeekVarianceGP { get; set; }
        public string PeriodActualValue { get; set; }
        public string PeriodActualValueLY { get; set; }
        public string PeriodVariance { get; set; }
        public string PeriodActualGP { get; set; }
        public string PeriodActualGPLY { get; set; }
        public string PeriodVarianceGP { get; set; }
        public string YTDActualValue { get; set; }
        public string YTDActualValueLY { get; set; }
        public string YTDVariance { get; set; }
        public string YTDActualGP { get; set; }
        public string YTDActualGPLY { get; set; }
        public string YTDVarianceGP { get; set; }
    }
}
