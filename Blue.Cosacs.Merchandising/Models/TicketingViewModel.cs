using FileHelpers;

namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    [DelimitedRecord(",")]
    public class TicketingViewModel
    {
        public string SKU { get; set; }
        public string ModelNumber { get; set; }
        public string Brand { get; set; }
        public string Fascia { get; set; }
        public string Location { get; set; }
        public string LongDescription { get; set; }
        public string POSDescription { get; set; }
        public string Features { get; set; }
        public string EffectiveDate { get; set; }
        public string CurrentCashPrice { get; set; }
        public string CurrentRegularPrice { get; set; }
        public string SetCode { get; set; }
        public string SetDescription { get; set; }
        public string Components { get; set; }
        public string NormalCashPrice { get; set; }
        public string NormalRegularPrice { get; set; }
        public string DutyFreePrice { get; set; }
    }
}