namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class SummaryUpdateControlReportQueryModel
    {
        public int? RunNumber { get; set; }
        public int? LocationId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}