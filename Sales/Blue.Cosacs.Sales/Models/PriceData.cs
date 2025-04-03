using Newtonsoft.Json;
using System;

namespace Blue.Cosacs.Sales.Models
{
    public class PriceData
    {
        [JsonProperty("LocationId")]
        public int? LocationId { get; set; }

        [JsonProperty("LocationName")]
        public string LocationName { get; set; }

         [JsonProperty("BranchNumber")]
        public short? BranchNumber { get; set; }

        [JsonProperty("Fascia")]
        public string Fascia { get; set; }

        [JsonProperty("RegularPrice")]
        public decimal RegularPrice { get; set; }

        [JsonProperty("CashPrice")]
        public decimal CashPrice { get; set; }

        [JsonProperty("DutyFreePrice")]
        public decimal DutyFreePrice { get; set; }

        [JsonProperty("AverageWeightedCost")]
        public decimal? AverageWeightedCost { get; set; }

        [JsonProperty("Margin")]
        public decimal? Margin { get; set; }

        [JsonProperty("EffectiveDate")]
        public DateTime EffectiveDate { get; set; }

	[JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }

        [JsonProperty("TaxRate")]
        public decimal TaxRate { get; set; }

        public decimal RegularPriceTaxInclusive
        {
            get
            {
                return ApplyTax(RegularPrice);
            }
        }

        public decimal CashPriceTaxInclusive
        {
            get
            {
                return ApplyTax(CashPrice);
            }
        }

        public decimal DutyFreePriceTaxInclusive
        {
            get
            {
                return ApplyTax(DutyFreePrice);
            }
        }

        private decimal ApplyTax(decimal price)
        {
            return price * (1 + TaxRate);
        }
    }
}
