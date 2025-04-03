using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.SalesManagement
{
    [Serializable]
    public sealed class CustomerInArrearsAndUndeliveredAccount
    {
        internal CustomerInArrearsAndUndeliveredAccount GetData(string customerId)
        {
            using (var ctx = Context.Create())
            {
                var ds = new IsCustomerInArrearsAndHasUndeliveredAccount
            {
                customerId = customerId
            }.ExecuteDataSet();

                return new CustomerInArrearsAndUndeliveredAccount
                {
                    IsInArrears = (bool)ds.Tables[0].Rows[0]["IsInArrears"],
                    HasUndeliveredAccount = (bool)ds.Tables[0].Rows[0]["HasUndeliveredAccount"]
                };
            }
        }

        public bool IsInArrears { get; set; }
        public bool HasUndeliveredAccount { get; set; }
    }
}
