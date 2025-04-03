using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Service.Repositories
{
    public class SalesManagementRepository
    {
        public bool HasItemInService(string customerId)
        {
            return new SalesManagement.Service().HasItemInService(customerId);
        }
    }
}
