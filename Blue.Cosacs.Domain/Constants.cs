using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public static class StoreCM
    {
       public const string StoreCardNumber = "StoreCardNumber";
       public const string StoreCardPrefix = "StoreCardPrefix";
    }

    public static class CashierWriteLimits
    {
        public const string Day = "Day";
        public const string WeekSat = "Week - Saturday";
        public const string WeekSun = "Week - Sunday";
        public static readonly string DayUpper = Day.ToUpper();
        public static readonly string WeekSatUpper = WeekSat.ToUpper();
        public static readonly string WeekSunUpper = WeekSun.ToUpper();
    }
}
