using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    [Flags]
    public enum InstStatus
    {
        Unknown    = 0x01,
        New        = 0x02,
        Booked     = 0x04,
        ToBeClosed = 0x08,
        Closed     = 0x16
    }
}
