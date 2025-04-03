using System;
using System.Collections;
using System.Collections.Generic;

namespace Blue.Cosacs.SalesManagement.Repositories
{
    public interface IDashboardRepository
    {
        IDictionary<int, Hashtable> CallSummary(IList<int> csr);
        bool IsLoadingDashboard { get; }
        void ManageDashboardLock(bool lockIt);
        IDictionary<int, Tuple<decimal, decimal, decimal, decimal>> SalesPersonTargets(int year);
    }
}
