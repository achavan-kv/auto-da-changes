using FileHelpers;

namespace Blue.Cosacs.Report.Service
{
    [DelimitedRecord(",")]
    public class ClaimsProductCategoryResult
    {
        public string Name { get; set; }
        public decimal CurrentMonthLabour { get; set; }
        public decimal CurrentMonthParts { get; set; }
        public decimal YearToDateLabour { get; set; }
        public decimal YearToDateParts { get; set; }
    }
}