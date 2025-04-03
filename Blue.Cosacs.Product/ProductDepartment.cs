using System.Collections.Generic;
using Blue.Collections.Generic;

namespace Blue.Cosacs.Stock
{
    public static class ProductDepartment
    {
        private static ReadOnlyDictionary<string, string> department = null;

        public static ReadOnlyDictionary<string, string> Data
        {
            get
            {
                return department ?? (department = new ReadOnlyDictionary<string, string>(GetDepartmentCodes()));
            }
        }

        private static Dictionary<string, string> GetDepartmentCodes()
        {
            return new Dictionary<string, string>
            {
                { "8", "ELECTRICAL" },
                { "11", "FREE GIFTS" },
                { "7", "FURNITURE" },
                { "2", "OPTICAL" },
                { "13", "REPOSSED GOODS" },
                { "12", "SPARE PARTS" },
                { "6", "SPECIALTIES" }
            };
        }
    }
}