using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.CodeCategories
{
    public class INST
    {
        public static class PRIMARYCHARGE
        {
            public const string Category = "INSTPRMCHRG";

            public const string Electrical = "INSTE";
            public const string Furniture  = "INSTF";
        }

        public static class CHARGEBREAKDOWN
        {
            public const string Category = "INSTCHRGBDWN";

            public const string PartsCourts = "PRTC";
            public const string PartsOther  = "PRTO";
            public const string PartsTotal  = "PRTTOT";
            public const string LabourTotal = "LBR";
            public const string Total       = "TOT";
        }

        public static class REBOOKREASON
        {
            public const string Category = "SRREASON";
        }
    }
}
