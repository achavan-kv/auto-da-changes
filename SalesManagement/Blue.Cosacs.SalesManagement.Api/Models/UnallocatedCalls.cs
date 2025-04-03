using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blue.Cosacs.SalesManagement.Api.Models
{
    public class UnallocatedCalls
    {
        public int[] SelectedCalls { get; set; }
        public int SalesPersonId { get; set; }
    }
}