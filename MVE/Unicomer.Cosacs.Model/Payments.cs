using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model
{
    public class Payments
    {
        public string resourceType { get; set; }
        public string source { get; set; }
        public string externalPaymentID { get; set; }
        public int checkoutId { get; set; }
        public string paymentType { get; set; }
        public string adjustmentType { get; set; }
        public string paymentDate { get; set; }
        public int employeeID { get; set; }
        public string paymentMethod { get; set; }
        public string checkNumber { get; set; }
        public List<Orders> orderList { get; set; }

    }

    public class Orders
    {
        public int orderId { get; set; }
        public string externalItemNo { get; set; }
        public decimal amount { get; set; }

    }
}
