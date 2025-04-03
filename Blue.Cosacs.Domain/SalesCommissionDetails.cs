using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Blue.Cosacs.Shared
{
    public class SalesCommissionDetails
    {
        public DataTable SalesCommissions { get; set; }
        public decimal? TotalCommission { get; set; }
        public decimal? TotalCommissionableValue { get; set; }
        public decimal? TotalProductCommissionValue { get; set; }
        public decimal? TotalTermsTypesCommissionValue { get; set; }
        public decimal? TotalWarrantyCommissionValue { get; set; }
    }
}
