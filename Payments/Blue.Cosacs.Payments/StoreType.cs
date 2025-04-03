using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Payments
{
    public static class StoreType
    {
        private static Dictionary<string, string> StoreTypes = new Dictionary<string, string>();
        static StoreType()
        {
            StoreTypes["C"] = "Courts";
            StoreTypes["N"] = "Non-Courts";
        }

        public static string GetNameForType(string storeType)
        {
            if (storeType == null)
            {
                return null;
            }

            if (StoreTypes.ContainsKey(storeType))
            {
                return StoreTypes[storeType];
            }

            return null;
        }

        public static Dictionary<string, string> GetStoreTypes()
        {
            return StoreTypes;
        }
    }
}
