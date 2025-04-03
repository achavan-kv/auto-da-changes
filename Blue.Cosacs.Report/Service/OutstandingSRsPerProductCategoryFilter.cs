
using System;
using System.ComponentModel;
using System.Text;

namespace Blue.Cosacs.Report.Service
{
    /// <summary>
    /// 'OutstandingSRsPerProductCategory' SPrroc parameters
    /// </summary>
    public class OutstandingSRsPerProductCategoryFilter
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public DateTime? CurrentDate { get; set; }
        public string Status { get; set; }
        public string Supplier { get; set; }
        public int? Technician { get; set; }
        public string WarrantyType { get; set; }

        public bool Validate()
        {
            return DateFrom.HasValue && DateTo.HasValue && !string.IsNullOrWhiteSpace(Status);
        }
    }
}
