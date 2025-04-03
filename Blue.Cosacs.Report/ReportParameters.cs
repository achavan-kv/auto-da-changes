using System.Collections.Generic;
using System;
using Blue.Data;

namespace Blue.Cosacs.Report
{
    public class Parameterization : PagedSearch
    {

        public Parameterization()
        {
            this.PageSize = int.MaxValue;
        }

        public string ReportId
        {
            get;
            set;
        }

        public Dictionary<string, string> Filter
        {
            get;
            set;
        }
    }
}
