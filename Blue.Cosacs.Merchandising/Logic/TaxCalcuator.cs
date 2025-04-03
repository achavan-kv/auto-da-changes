namespace Blue.Cosacs.Merchandising.Logic
{
    public static class TaxCalcuator
    {
        public static decimal AddTax(decimal amount, decimal rate)
        {
            return amount * (1 + rate);
        }

        public static decimal ExcludeTax(decimal amount, decimal rate)
        {
            return amount / (1 + rate);
        }
    }
}
