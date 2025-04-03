using System.Collections.Generic;

namespace Blue.Cosacs.Warranty
{
    public static class WarrantyType
    {
        public const string Free = "F";
        public const string Extended = "E";
        public const string InstantReplacement = "I";

        private static readonly Dictionary<string, string> WarrantyTypes = new Dictionary<string, string>();
        static WarrantyType()
        {
            WarrantyTypes[Extended] = "Extended";
            WarrantyTypes[Free] = "Free";
            WarrantyTypes[InstantReplacement] = "Instant Replacement";
        }

        public static string GetNameForType(string storeType)
        {
            if (storeType == null)
            {
                return null;
            }

            return WarrantyTypes.ContainsKey(storeType) ? WarrantyTypes[storeType] : null;
        }

        public static Dictionary<string, string> GetWarrantyTypes()
        {
            return WarrantyTypes;
        }
    }
}
