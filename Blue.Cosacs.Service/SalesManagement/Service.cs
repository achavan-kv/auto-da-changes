using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Service.SalesManagement
{
    [Serializable]
    public sealed class Service
    {
        internal bool HasItemInService(string customerId)
        {
            using (var scope = Context.Write())
            {
                return scope.Context.Request
                    .Where(p => p.State != "Closed" &&
                           p.CustomerId == customerId)
                    .Any();
            }
        }
    }
}
