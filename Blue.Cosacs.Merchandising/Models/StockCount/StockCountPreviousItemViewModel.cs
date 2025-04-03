namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class StockCountPreviousItemViewModel
    {
        public int StockCountId { get; set; }

        public int LocationId { get; set; }

        public string Fascia { get; set; }

        public string LocationName { get; set; }

        public string Type { get; set; }

        public DateTime CountDate { get; set; }

        public DateTime? ClosedDate { get; set; }

        public int Count { get; set; }

        public int Variance { get; set; }

        public string Comments { get; set; }
    }
}
