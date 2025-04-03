using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Warehouse.Utils
{
    internal static class BookingStatus
    {
        public static class Status
        {
            public const string Scheduled = "Scheduled";
            public const string Picking = "Picking";
            public const string Picked = "Picked";
            public const string Booked = "Booked";
            public const string Exception = "Exception";
            public const string Rejected = "Rejected";
            public const string Delivered = "Delivered";
            public const string Closed = "Closed";           // #10633
            public const string Collected = "Collected";
            public const string Printed = "Printed";         // #10792
        }

        public static string GetStatus(StatusView booking)
        {
            if (booking.Closed!=0)               // #10633
                return Status.Closed;
            if (booking.CancelledId.HasValue)
                return Status.Rejected;
            if (booking.Exception)
                return Status.Exception;
            if (booking.DeliveryRejected.HasValue && booking.DeliveryOrCollection != "C")
                return Status.Delivered;
            if (booking.DeliveryRejected.HasValue && booking.DeliveryOrCollection == "C")
                return Status.Collected;
            if (booking.ScheduleRejected.HasValue)
                return Status.Scheduled;
            if (booking.PickingRejected.HasValue)
                return Status.Picked;
            if (booking.PickListNo.HasValue)
                return Status.Picking;
            if (booking.PickUpNotePrintedBy.HasValue)    // #10792
                return Status.Printed;
            return Status.Booked;
        }

    }
}
