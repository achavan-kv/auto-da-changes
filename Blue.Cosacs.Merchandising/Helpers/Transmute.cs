namespace Blue.Cosacs.Merchandising.Helpers
{
    using System;

    public static class Transmute
    {
        public static TConverted ChangeType<TConverted>(IConvertible obj) where TConverted : IConvertible
        {
            return (TConverted)Convert.ChangeType(obj, typeof(TConverted));
        }
    }
}
