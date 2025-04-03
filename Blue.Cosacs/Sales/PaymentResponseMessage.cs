using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Sales
{
    public class PaymentResponseMessage
    {
        public string ErrorMessage { get; set; }
        public decimal Balance { get; set; }
    }
}
