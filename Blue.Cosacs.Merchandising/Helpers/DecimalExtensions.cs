namespace Blue.Cosacs.Merchandising.Helpers
{
    public static class DecimalExtensions
    {
        public static string ToCurrency(this decimal amount)
        {
            return amount.ToString("N" + new Config.Settings().DecimalPlaces);
        }

        public static string ToCurrencyWithSymbol(this decimal amount)
        {
            return new Config.Settings().CurrencySymbol + ToCurrency(amount);
        }

        public static string ToCurrency(this decimal? amount)
        {
            return amount.HasValue ? amount.Value.ToString("N" + new Config.Settings().DecimalPlaces) : string.Empty;
        }

        public static string ToCurrencyWithSymbol(this decimal? amount)
        {
            return new Config.Settings().CurrencySymbol + ToCurrency(amount);
        }

        public static string ToPercentage(this decimal d)
        {
            return string.Format("{0:P}", d);
        }

        public static string ToPercentage(this decimal? d)
        {
            return string.Format("{0:P}", d ?? 0);
        }
    }
}