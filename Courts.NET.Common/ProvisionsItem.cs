using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace STL.Common
{
[Serializable]
    public class ProvisionsItem
    {
        public char Acctype { get; set; }
        public string StatusName { get; set; }
        public int StatusUpper { get; set; }
        public int StatusLower { get; set; }
        public string MonthsName { get; set; }
        public int MonthsUpper { get; set; }
        public int MonthsLower { get; set; }
        public decimal Provision { get; set; }

        
    }
}
