using System.Xml;
using System.Data.SqlClient;
using STL.DAL;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.Subscribers
{
    /// <summary>
    /// Receives a Booking cancelation from warehouse.
    /// </summary>
    public class WarehouseCancel : Hub.Client.Subscriber
    {
        public override void Sink(int id, XmlReader message)
        {
          
            using (var conn = new SqlConnection(Connections.Default))
            {
                conn.Open();

                using (var trans = conn.BeginTransaction())
                {
                    using (var ctx = Context.Create(conn, trans))
                    {
                        var b = Deserialize<Blue.Cosacs.Messages.Warehouse.WarehouseCancel>(message);

                        var bookingFailure = new LineItemBookingFailures
                        {
                            BookingID = b.Id,
                            OriginalBookingID = b.OrigBookingId,
                            Quantity = b.Quantity,
                            CancelReason = b.Comment,
                            Actioned = null
                        };

                        ctx.LineItemBookingFailures.InsertOnSubmit(bookingFailure);
                        ctx.SubmitChanges();

                        var bookingSchedule = new WarehouseRepository().GetLineItemBookingScheduleFromBookingID(conn, trans, b.OrigBookingId);  //#13764

                        //#13764
                        if (bookingSchedule != null && bookingSchedule.DelOrColl != "C")
                        { 
                            new WarehouseRepository().UpdateLineItemBookingScheduleQuantity(conn, trans, b.OrigBookingId, b.Quantity);
                        }

                        trans.Commit();
                    }
                }
            }
        }
    }
}
