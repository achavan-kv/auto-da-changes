namespace Blue.Cosacs.Merchandising.Helpers
{
    using Blue.Cosacs.Merchandising.Models;

    public static class FieldSchemaExtensions
    {
        public static string ToYesNo(this FieldSchema fi)
        {
            if (fi == null || fi.Value == null)
            {
                return "N";
            }
            var val = fi.Value.ToLower();
            return (val == "y" || val == "yes" || val == "true" || val == "1") ? "Y" : "N";
        }
    }
}
