using System;
using System.Collections.Generic;
using System.Text;

namespace STL.Common.Services.Model
{
    public class Warrantables
    {
        public List<WarrantyRenewal> warrantyRenewals{ get; set; }
    }

    public class Item 
    {
        public string ItemNumber { get; set; }
        public short ItemLocation { get; set; }
    }
}
