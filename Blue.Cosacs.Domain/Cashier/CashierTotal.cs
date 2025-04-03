using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public class CashierTotal
    {
        public decimal UserTotal { get; set; }
        public decimal SystemTotal { get; set; }
        public decimal DifferenceTotal { get; set; }
        public decimal DepositTotal { get; set; }
        public bool Zero { get { return UserTotal == 0 && SystemTotal == 0 && DifferenceTotal == 0 && DepositTotal == 0; } }
        public bool Difference = false;
        public bool OSFailure = false;
        public string Comments { get; set; }

        public CashierTotal(List<CashierTotalsView> CashTotals, List<ExchangeRate> exchangeRates)
        {
            var user = 0m;
            var system = 0m;
            var diff = 0m;
            var dep = 0m;
            decimal factor;
            foreach (var total in CashTotals)
            {
                var ex = exchangeRates.Find(delegate(ExchangeRate e) { return e.Currency == total.paymethod.Value.ToString(); });
                factor = ex == null ? 1 : Convert.ToDecimal(ex.Rate);

                user += factor * total.UserAmounts;
                system += total.NetReceipts.HasValue ? total.NetReceipts.Value : 0;
                diff += factor * total.Difference;

                if (total.Difference != 0)
                {
                    if (!total.AllowOS.Value)
                    {
                        total.Comments = String.Format("{0} - Shortage and Overage not Allowed.", total.codedescript);
                        Comments += total.Comments + "\n";
                        OSFailure = true;
                    }
                    else
                    {
                        Comments += String.Format("{0} \n", total.codedescript);
                    }
                    Difference = true;
                }
                if (total.IncludeInCashierTotals.HasValue && total.IncludeInCashierTotals.Value)
                    dep += (total.Allocation.HasValue ? total.Allocation.Value : 0m) + (total.PettyCash.HasValue ? total.PettyCash.Value : 0m) + (total.Remittance.HasValue ? total.Remittance.Value : 0m) + (total.Disbursement.HasValue ? total.Disbursement.Value : 0m);
            }
            this.DifferenceTotal = diff;
            this.SystemTotal = system;
            this.UserTotal = user;
        }
    }
}
