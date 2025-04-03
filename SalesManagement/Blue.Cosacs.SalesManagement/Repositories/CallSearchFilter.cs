using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.SalesManagement.Repositories
{
    public class CallSearchFilter
    {
        public byte? CallTypeId { get; set; }
        public DateTime? ScheduledDateFrom { get; set; }
        public DateTime? ScheduledDateTo { get; set; }
        public string CustomerName { get; set; }
        public string ReasonForCalling { get; set; }
        public int SalesPersonId { get; set; }
        public int? CSRId { get; set; }
        public short Branch { get; set; }
        public int? Take { get; set; }
        public bool UnavailableCSR { get; set; }
        public bool NoCSR { get; set; }
        public bool LockedCSR { get; set; }
        public int[] LockedCSRList { get; set; }
    }
}
