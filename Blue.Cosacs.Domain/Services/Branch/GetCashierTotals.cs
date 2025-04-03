using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.Branch
{
    partial class GetCashierTotalsRequest
    {
        public DateTime DateFrom { get; set; }
        public DateTime dateto { get; set; }
        public int Empeeno { get; set; }
    }

    partial class GetCashierTotalsResponse
    {
        public List<CashierTotalsView> Cashiertotals { get; set; }
        public List<ExchangeRate> ExchangeRates { get; set; }
    }
}
