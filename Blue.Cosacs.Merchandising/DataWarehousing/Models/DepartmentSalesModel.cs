using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Merchandising.DataWarehousing.Models
{
    using Percolator.AnalysisServices.Attributes;

    /// <summary>
    /// </summary>
    public class RowModel
    {
        [MapTo("RowName")]
        public string RowName { get; set; }

        [MapTo("CategoryName")]
        public string CategoryName { get; set; }

        [MapTo("TodaySales")]
        public decimal TodaySales { get; set; }

        [MapTo("TodayGrossProfit")]
        public decimal TodayGrossProfit { get; set; }

        [MapTo("TodayLastYearSales")]
        public decimal TodayLastYearSales { get; set; }

        [MapTo("TodayLastYearGrossProfit")]
        public decimal TodayLastYearGrossProfit { get; set; }

        [MapTo("ThisWeekSales")]
        public decimal ThisWeekSales { get; set; }

        [MapTo("ThisWeekGrossProfit")]
        public decimal ThisWeekGrossProfit { get; set; }

        [MapTo("ThisWeekLastYearSales")]
        public decimal ThisWeekLastYearSales { get; set; }

        [MapTo("ThisWeekLastYearGrossProfit")]
        public decimal ThisWeekLastYearGrossProfit { get; set; }

        [MapTo("ThisYearSales")]
        public decimal ThisYearSales { get; set; }

        [MapTo("ThisYearGrossProfit")]
        public decimal ThisYearGrossProfit { get; set; }

        [MapTo("LastYearSales")]
        public decimal LastYearSales { get; set; }

        [MapTo("LastYearGrossProfit")]
        public decimal LastYearGrossProfit { get; set; }

        [MapTo("ThisPeriodSales")]
        public decimal ThisPeriodSales { get; set; }

        [MapTo("ThisPeriodGrossProfit")]
        public decimal ThisPeriodGrossProfit { get; set; }

        [MapTo("ThisPeriodLastYearSales")]
        public decimal ThisPeriodLastYearSales { get; set; }

        [MapTo("ThisPeriodLastYearGrossProfit")]
        public decimal ThisPeriodLastYearGrossProfit { get; set; }

        [MapTo("VarianceDaySales")]
        public decimal VarianceDaySales { get; set; }

        [MapTo("VarianceDayGrossProfit")]
        public decimal VarianceDayGrossProfit { get; set; }

        [MapTo("VarianceWeekSales")]
        public decimal VarianceWeekSales { get; set; }

        [MapTo("VarianceWeekGrossProfit")]
        public decimal VarianceWeekGrossProfit { get; set; }

        [MapTo("VariancePeriodSales")]
        public decimal VariancePeriodSales { get; set; }

        [MapTo("VariancePeriodGrossProfit")]
        public decimal VariancePeriodGrossProfit { get; set; }

        [MapTo("VarianceYearSales")]
        public decimal VarianceYearSales { get; set; }

        [MapTo("VarianceYearGrossProfit")]
        public decimal VarianceYearGrossProfit { get; set; }
    }
}
