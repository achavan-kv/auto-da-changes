using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model
{
    public class DeliveryAuth
    {
        public string resourceType { get; set; }
        public string source { get; set; }
        public Int32 checkoutID { get; set; }
        public bool authorization { get; set; }
        public List<orderDetails> orderDetails { get; set; }
    }
    public class orderDetails
    {
        public int orderID { get; set; }
    }
}

