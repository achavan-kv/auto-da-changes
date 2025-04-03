using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Sales.Models
{
    public class BranchInfo
    {
        public short BranchNumber { get; set; }
        public string BranchName { get; set; }
        public string StoreType { get; set; }
        public string CountryCode { get; set; }
        public string BranchAddress1 { get; set; }
        public string BranchAddress2 { get; set; }
        public string BranchAddress3 { get; set; }
    }
}
