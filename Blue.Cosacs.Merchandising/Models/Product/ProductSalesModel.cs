using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Merchandising.Models
{
    public class ProductSalesModel
    {
        public string Type { get; set; }
        public int ThisPeriod { get; set; }
        public int LastPeriod { get; set; }
        public int ThisYTD { get; set; }
        public int LastYTD { get; set; }   
    }
}
