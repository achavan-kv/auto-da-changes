using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public static class WarrantyType
    {
        public const string Free = "F";
        public const string Extended = "E";
        public const string InstantReplacement = "I";

        public static bool IsFree(string warrantyType)
        {
            return  string.Compare(warrantyType, Free, true) ==  0;
        }

        public static bool IsInstantReplacement(string warrantyType)         // #18462
        {
            return string.Compare(warrantyType, InstantReplacement, true) == 0;
        }
    }

    //#18409
    public static class WarrantyStatus
    {
        public const string Expired = "Expired";
        public const string Redeemeded = "Redeemed";
        public const string Cancelled = "Cancelled";
        public const string Active = "Active";
    }
}
