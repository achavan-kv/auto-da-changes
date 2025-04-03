using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.SalesManagement
{
    public enum FollowUpCallsTimePeriods
    {
        Day = 1,
        Week = 2,
        Month = 3
    }

    public partial class FollowUpCall
    {
        public FollowUpCallsTimePeriods ChronologicalTimePeriod
        {
            get
            {
                return (FollowUpCallsTimePeriods)this.TimePeriod;
            }
        }
    }
}
