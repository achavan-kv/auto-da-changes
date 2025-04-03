namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BuyerSalesHistoryKpiModel
    {
        public string Year { get; set; }
        public int? NumericYear { get; set; }
        public string Month { get; set; }
        public int? NumericMonth { get; set; }
        public int ProductId { get; set; }
        public decimal Value { get; set; }
    }
}