using System.Collections.Generic;
using Newtonsoft.Json;

namespace Blue.Cosacs.Sales
{
    public partial class OrderItem
    {
      [JsonIgnore]
        public virtual Order Order { get; set; }

      [JsonIgnore]
        public virtual OrderItem ParentItem { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
