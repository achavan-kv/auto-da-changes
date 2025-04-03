using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Sales
{
    public partial class Order
    {
      public virtual ICollection<OrderItem> Items { get; set; }
      public virtual ICollection<OrderPayment> Payments { get; set; }
      public virtual OrderCustomer OrderCustomer { get; set; }
    }
}
