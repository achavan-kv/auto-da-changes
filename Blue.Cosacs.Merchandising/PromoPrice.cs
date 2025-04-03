namespace Blue.Cosacs.Merchandising
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Helpers;

    public class PromoPrice
    {
        private readonly Settings settings;
        private readonly List<int> roundings;
        private readonly int decimalPlaces;

        public PromoPrice(Settings settings, Config.Settings config)
        {
            this.settings = settings;
            this.roundings = GetRoundings();
            this.decimalPlaces = config.DecimalPlaces;
        }

        public List<int> GetRoundings()
        {
            return settings.PriceRounding
                .Select(p =>
                    {
                        int val;
                        if (int.TryParse(p, out val))
                        {
                            return (int?)val;
                        }
                        return null;
                    })
                .Where(p => p != null)
                .OrderBy(p => p)
                .Select(p => (int)p)
                .ToList();
        }

        public decimal RoundPromoPrice(decimal discountedPrice, decimal originalPrice)
        {
            // strip decimals
            discountedPrice = Math.Round(discountedPrice, 0, MidpointRounding.AwayFromZero);
            var discountedPriceStr = discountedPrice.ToString();
            var originalPriceStr = originalPrice.ToString();
            
            // only use round values that are lower than the original price
            var roundValues = roundings.Where(r => r <= originalPrice).ToList();

            // try to find the lowest round value above the discounted price
            var roundTo = roundValues.Where(r =>
                                            {
                                                var digits = r.ToString().Length;
                                                var priceSubstr =
                                                    discountedPriceStr.Substring((discountedPriceStr.Length > digits ? discountedPriceStr.Length : digits)  - digits, digits);
                                                return r >= int.Parse(priceSubstr);
                                            }).MinOrDefault();

            // if not found, try to find the highest round value lower than the discount
            if (roundTo == null)
            {
                roundTo = roundValues.Where(r =>
                {
                    var digits = r.ToString().Length;
                    var priceSubstr = originalPriceStr.Substring(originalPriceStr.Length - digits, digits);
                    return r <= int.Parse(priceSubstr);
                }).MaxOrDefault();
            }

            // if no rounding point found return the discounted price
            if (roundTo == null)
            {
                return Math.Round(discountedPrice, decimalPlaces);
            }

            //The following block of code commented for CR '5575406 : Promotion Price Round off'
            /*
            var replaceStr = roundTo.ToString();
            var promoPrice = discountedPriceStr.Substring(0, discountedPriceStr.Length - replaceStr.Length);
            promoPrice += replaceStr;
            */

            return int.Parse(discountedPriceStr);//CR '5575406 : Promotion Price Round off'
        }
    }
}
