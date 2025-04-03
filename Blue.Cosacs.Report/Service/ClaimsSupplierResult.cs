using System.Collections.Generic;
using System.Linq;
using FileHelpers;

namespace Blue.Cosacs.Report.Service
{
    [DelimitedRecord(",")]
    public class ClaimsSupplierResult
    {
        public string Name { get; set; }
        public decimal CurrentMonth { get; set; }
        public decimal YearToDate { get; set; }
        [FieldHidden]
        public IList<ClaimsProductCategoryResult> ClaimsProductCategories;

        public decimal TotalCurrentMonthLabour
        {
            get {return ClaimsProductCategories.Select(t => t.CurrentMonthLabour).Sum(); }
        }

        public decimal TotalCurrentMonthParts
        {
            get { return ClaimsProductCategories.Select(t => t.CurrentMonthParts).Sum(); }
        }
        public decimal TotalYearToDateLabour
        {
            get { return ClaimsProductCategories.Select(t => t.YearToDateLabour).Sum(); }
        }
        public decimal TotalYearToDateParts
        {
            get { return ClaimsProductCategories.Select(t => t.YearToDateParts).Sum(); }
        }
    }
}