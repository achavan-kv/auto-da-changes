using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.SalesManagement.Hub.Subscribers
{
    public interface ICallSummarySubscriber
    {
        /*Task*/ void FillCallSummary();
    }
}
