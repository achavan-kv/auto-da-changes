namespace Blue.Cosacs.Merchandising.Helpers
{
    using Blue.Cosacs.Merchandising.Models;

    public static class TaxHelper
    {
        public static decimal ApplyTax(decimal price, decimal? taxRate, bool taxSetting, bool forceTax = false)
        {
            var tax = taxSetting || forceTax;

            if (tax == false || !taxRate.HasValue)
            {
                return price;
            }

            return price * (1 + taxRate.Value);
        }
    }
}
