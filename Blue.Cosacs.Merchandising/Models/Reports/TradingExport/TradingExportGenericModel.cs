using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Merchandising.Models
{
    public class TradingExportGenericModel
    {
        public long? Id { get; set; }
        public int? DateKey { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string Fascia { get; set; }
        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public string SaleType { get; set; }
        public string Division { get; set; }
        public double? Price { get; set; }
        public double? GrossProfit { get; set; }
        public string Department { get; set; }
        public string Class { get; set; }
        public string ClassCode { get; set; }
        public string DepartmentCode { get; set; }
    }
}
