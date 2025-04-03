using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.SalesManagement
{
    public class SalesKpiDto<T>
    {
        public int CSR { get; set; }
        public int BranchNo { get; set; }
        public DateTime FirstWeek { get; set; }
        public T Total { get; set; }
        public int WeekNo { get; set; }
    }
}
