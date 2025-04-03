using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Warehouse
{
    public partial class PicklistView
    {
        public string PicklistDisplayNumber 
        {
            get
            {
                return Id.ToString("D8");
            }
        }
    }
}
