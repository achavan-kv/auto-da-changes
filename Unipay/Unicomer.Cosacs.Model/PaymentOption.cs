using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model
{
    public class PaymentOption
    {
        public string numberOfInstallments { get; set; }
        public string amount { get; set; }
        public string interest { get; set; }
        public string interestRateAnnual { get; set; }
        public string effectiveTaxes { get; set; }
    }

    public class PaymentOptionList
    {
        public List<PaymentOption> content { get; set; }
        public string message { get; set; }
        public string status { get; set; }
    }
}
