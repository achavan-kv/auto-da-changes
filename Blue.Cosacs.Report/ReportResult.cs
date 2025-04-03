using System.Collections.Generic;
using Blue.Cosacs.Report.Xml;
using Blue.Data;

namespace Blue.Cosacs.Report
{
    public class ReportResult : PagedSearch
    {
        public ReportResult()
        {
            this.PageIndex = 1;
            this.PageSize = int.MaxValue;
            this.PageCount = 0;
        }

        public int PageCount
        {
            get;
            set;
        }

        // All data. 
        public List<List<string>> AllData
        {
            get;
            set;
        }

        public List<List<string>> Data
        {
            get;
            set;
        }

        public List<Column> ColumnParameters
        {
            get;
            set;
        }

        public List<Column> ReplaceColumns
        {
            get;
            set;
        }
    }
}
