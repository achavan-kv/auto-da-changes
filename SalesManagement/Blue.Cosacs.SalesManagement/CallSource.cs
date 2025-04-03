using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.SalesManagement
{
    /// <summary>
    /// The source of the call
    /// </summary>
    public enum CallSourceEnum
    {
        /// <summary>
        /// Call was added through the user interface
        /// </summary>
        UserInterface = 1,

        /// <summary>
        /// Call was added through the CustomerInstalmentEnding job
        /// </summary>
        InstalmentEnd = 2,

        /// <summary>
        /// Call was added through the CustomerInstalmentEnding job
        /// </summary>
        InactiveCustomers = 3,

        /// <summary>
        /// Call was added through the FollowupCalls
        /// </summary>
        FollowUpCalls = 4
    }
}
