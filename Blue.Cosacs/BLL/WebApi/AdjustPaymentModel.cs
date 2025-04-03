using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.BLL.WebApi
{
  public  class AdjustPaymentModel
    {
        public string LoanTransactionId { get; set; }
        public string CosacsAccountId { get; set; }
        public string Notes { get; set; }
    }
}
