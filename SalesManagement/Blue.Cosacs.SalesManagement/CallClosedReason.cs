using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.SalesManagement
{
    public class CallClosedReason
    {
        /// <summary>
        /// The reason for closing a call
        /// </summary>
        public enum CallClosedReasonEnum
        {
            /// <summary>
            /// Call was added through the user interface
            /// </summary>
            CalledTheCustomer = 1,
            ClosedByStoreManager = 2,
            ClosedInBulkByCSR = 3,
            DoNotCallAgain = 4,
            FlushedCall = 5
        }
    }
}
