using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public class StoreCardNewCardAgreement
    {
        public Customer Customer { get; set; }
        public CustAddress CustAddress { get; set; }
        public Customer NewCustomer { get; set; }
        public CustAddress NewCustAddress { get; set; }
    }
}
