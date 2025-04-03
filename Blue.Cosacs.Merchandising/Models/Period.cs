namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class Period
    {
        public byte PeriodNo { get; set; }
        public byte Week { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}