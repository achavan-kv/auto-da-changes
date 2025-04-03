using FileHelpers;

namespace Blue.Cosacs.Report.Service
{
    [DelimitedRecord(",")]
    public class OutstandingSRsPerProductCategoryResult
    {
        /*
         * Band01: Outstanding with 0 to 3 days period
         * Band02: Outstanding with 4 to 7 days period
         * Band02: Outstanding with 8 to 14 days period
         * Band04: Outstanding with more than 14 days period
         */

        public string ProductCategory { get; set; }
        public string DaysOutstandingBand01 { get; set; }
        public string DaysOutstandingBand02 { get; set; }
        public string DaysOutstandingBand03 { get; set; }
        public string DaysOutstandingBand04 { get; set; }

        [FieldHidden]
        public string ServiceRequestsBand01;

        [FieldHidden]
        public string ServiceRequestsBand02;

        [FieldHidden]
        public string ServiceRequestsBand03;

        [FieldHidden]
        public string ServiceRequestsBand04;

    }
}
