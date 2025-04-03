using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.SalesManagement.Repositories
{
    public class PreviousCall
    {
        public int CallId { get; set; }
        public DateTime? CalledAt { get; set; }
        public string ReasonForCalling { get; set; }
        public bool SpokeToCustomer { get; set; }
        public string Comments { get; set; }
        public int SalesPersonId { get; set; }
        public DateTime? RescheduledOn { get; set; }
        public byte CallTypeId { get; set; }
    }
}
