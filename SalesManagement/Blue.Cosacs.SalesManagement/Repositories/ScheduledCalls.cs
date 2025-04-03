using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.SalesManagement.Repositories
{
    public class ScheduledCalls
    {
        public IList<Call> Calls { get; set; }
        public int NoOfScheduledCalls { get; set; }
        public IList<BranchManagerCall> BranchManagerCalls { get; set; }
    }
}
