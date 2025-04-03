using System;

namespace Blue.Cosacs.Sales.Models
{
    public class PromoData
    {
        public int? LocationId { get; set; }
        public string LocationName { get; set; }
        public string Fascia { get; set; }
        public string RegularPrice { get; set; }
        public string CashPrice { get; set; }
        public string DutyFreePrice { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}
