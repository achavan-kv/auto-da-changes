using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Merchandising.Models
{
    public class TradingExportDatesModel
    {
        public DateTime Day { get; set; }
        public DateTime Week { get; set; }
        public DateTime Period { get; set; }
        public DateTime Year { get; set; }
        public DateTime? LastYearDay { get; set; }
        public DateTime LastYearWeek { get; set; }
        public DateTime LastYearPeriod { get; set; }
        public DateTime LastYearYear { get; set; }
        public DateTime? LastYearToDate { get; set; }
    }
}
