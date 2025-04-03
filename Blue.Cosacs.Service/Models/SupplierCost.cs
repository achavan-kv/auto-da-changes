using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Service.Models
{
    // Names map directy to Js.

    public class SupplierCosts
    {
        public string supplier { get; set; }
        public Cost[] costs { get; set; }

        //public SupplierCosts.Cost[] Costs { get; set; }

    }
    public class Cost
    {
        public string product { get; set; }
        public string month { get; set; }
        public short year { get; set; }
        public string partType { get; set; }
        public decimal? partPcent { get; set; }
        public decimal? partVal { get; set; }
        public decimal? labourPcent { get; set; }
        public decimal? labourVal { get; set; }
        public decimal? additionalPcent { get; set; }
        public decimal? additionalVal { get; set; }
    }


}
