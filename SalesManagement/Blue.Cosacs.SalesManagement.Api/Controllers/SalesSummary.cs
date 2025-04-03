using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [Serializable]
    internal sealed class SalesSummary
    {
        public int SalesPerson { get; set; }
        public decimal Amount { get; set; }
        public short BranchNo { get; set; }
    }
}