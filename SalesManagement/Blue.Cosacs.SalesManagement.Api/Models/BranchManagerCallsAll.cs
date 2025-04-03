using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blue.Cosacs.SalesManagement.Api.Models
{
    public class BranchManagerCallsAll
    {
        public string CustomerFilter { get; set; }
        public DateTime ToCallAt { get; set; }
        public string ReasonForCalling { get; set; }
    }
}