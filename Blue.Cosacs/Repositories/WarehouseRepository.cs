using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Blue.Cosacs.Shared.Services.Warehouse;
using Blue.Cosacs.Shared;
using STL.Common.Constants.Delivery;

namespace Blue.Cosacs.Repositories
{
    public class WarehouseRepository
    {
        public GetLineItemBookingFailuresResponse GetBookingFailures(int? branch, int salesperson) 
        {              
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                return new GetLineItemBookingFailuresResponse
                    {
                        Failures = (from bf in ctx.BookingFailuresView
                                    where (branch == null || bf.SalesBrnNo == branch)
                                       && bf.Actioned==null
                                       && (bf.SalesPerson == salesperson || salesperson == 0)        // #10618 - add Salesperson dropdown
                                    select bf).AnsiToList(ctx)
                    };
            });
        }

        // check if Failure has been actioned
        public bool CheckBookingFailureNotAction(SqlConnection conn, SqlTransaction trans, int bookingFailureId) 
        {
            using (var ctx = Context.Create(conn, trans))
            {
                {
                    var actioned = (from bf in ctx.LineItemBookingFailures
                                    where bf.ID == bookingFailureId
                                    select bf.Actioned).AnsiFirstOrDefault(ctx);

                    if (actioned != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                };
                
            }
        }

        // update BookingFalure to actioned
        public void UpdateBookingFailureActioned(SqlConnection conn, SqlTransaction trans, int bookingFailureId, DataTable lineItemBooking)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var queryExistingBooking = ctx.LineItemBookingFailures
                                            .Where(b => b.ID == bookingFailureId && b.Actioned == null );

                foreach (var booking in queryExistingBooking)
                {
                    foreach (DataRow dr in lineItemBooking.Rows)
                    {
                        // set actioned to new BookingID
                        booking.Actioned = Convert.ToInt32(dr["BookingID"]);
                    }
                }

                ctx.SubmitChanges();

            }
            
        }

        //IP - 13/06/12 - #10328 - Update LineItemBookingFailures table actioned column with the new booking id
        public void UpdateBookingFailureActioned(SqlConnection conn, SqlTransaction trans, int bookingId, int newBookingId)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var bookingFailure = ctx.LineItemBookingFailures.Where(f => f.OriginalBookingID == bookingId && f.Actioned == null).FirstOrDefault();   // #10385 
               
                if (bookingFailure != null)
                {
                    bookingFailure.Actioned = newBookingId;
                    ctx.SubmitChanges();
                }
            }
        }

        //IP - 15/06/12 - #10387 - Updates the LineItemBookingSchedule.BookingId
        public void UpdateLineItemBookingScheduleBookingId(SqlConnection conn, SqlTransaction trans, int lineItemId, int bookingId)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var bookingSchedule = ctx.LineItemBookingSchedule.Where(s => s.LineItemID == lineItemId).OrderByDescending(s => s.ID).First();

                if (bookingSchedule != null)
                {
                    bookingSchedule.BookingId = bookingId;
                    ctx.SubmitChanges();
                }
            }
        }

        //IP - 19/06/12 - #10440 - Return the LineItemBookingFailures record for an item
        public LineItemBookingFailures GetLineItemBookingFailures(SqlConnection conn, SqlTransaction trans, int lineItemId, int bookingId)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var bookingFailure = ctx.LineItemBookingFailures.Where(lbf => lbf.OriginalBookingID == bookingId).OrderByDescending(lbf => lbf.ID).FirstOrDefault(); //#10490
                                     
                return bookingFailure;

            }
        }

        //IP - 19/06/12 - #10440 - Return the last booking for an item
        public LineItemBooking GetLineItemBooking(SqlConnection conn, SqlTransaction trans, int lineItemId)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var booking = ctx.LineItemBooking.Where(lb => lb.LineItemID == lineItemId).OrderByDescending(lb => lb.ID).FirstOrDefault();     // #10593

                return booking;
            }
        }

        //IP - 19/06/12 - Delete a booking schedule
        public void DeleteLineItemBookingSchedule(SqlConnection conn, SqlTransaction trans, int bookingId)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var bookingSchedule = ctx.LineItemBookingSchedule.Where(s => s.BookingId == bookingId).FirstOrDefault();

                if (bookingSchedule != null)
                {
                    ctx.LineItemBookingSchedule.DeleteOnSubmit(bookingSchedule);
                    ctx.SubmitChanges();
                }
            }
        }

        //IP/JC - 20/06/12 - 
        public void UpdateLineItemBookingScheduleQuantity(SqlConnection conn, SqlTransaction trans, int bookingId, float quantity)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var bookingSchedule = ctx.LineItemBookingSchedule.Where(s => s.BookingId == bookingId).FirstOrDefault();

                if (bookingSchedule != null && bookingSchedule.Quantity != 0)  //#17001
                {
                    //if (quantity == Convert.ToSingle(bookingSchedule.Quantity))
                    //{
                    //    this.DeleteLineItemBookingSchedule(conn, trans, bookingId);
                    //}
                    //else
                    //{
                    //bookingSchedule.Quantity  = bookingSchedule.Quantity < 0? bookingSchedule.Quantity += quantity: bookingSchedule.Quantity -=quantity; // #10475
                    bookingSchedule.Quantity = quantity == 0? 0: bookingSchedule.Quantity < 0 ? bookingSchedule.Quantity += quantity : bookingSchedule.Quantity -= quantity; //#13829  // #10475
                        ctx.SubmitChanges();
                    //}
                }
            }
        }

        //IP - 21/06/12 - #10479 - Return the last LineItemBookingSchedule for an item
        public LineItemBookingSchedule GetLineItemBookingSchedule(SqlConnection conn, SqlTransaction trans, int lineItemId)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var bookingSchedule = ctx.LineItemBookingSchedule.Where(ls => ls.LineItemID == lineItemId).OrderByDescending(ls => ls.ID).FirstOrDefault();    // #12516

                return bookingSchedule;
            }
        }

        //#13764
        public LineItemBookingSchedule GetLineItemBookingScheduleFromBookingID(SqlConnection conn, SqlTransaction trans, int bookingId)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var bookingSchedule = ctx.LineItemBookingSchedule.Where(s => s.BookingId == bookingId).FirstOrDefault();

                return bookingSchedule;
            }
        }

        //IP - 21/06/12 Insert a new LineItemBookingSchedule
        public void InsertLineItemBookingSchedule(SqlConnection conn, SqlTransaction trans, int lineItemId, string bookingType, int retItemID, decimal retVal, short retStockLocn, int bookingId, float quantity,
         int itemID, int stockLocn, decimal price) //#12842
        {
            using (var ctx = Context.Create(conn, trans))
            {

                LineItemBookingSchedule bookingSchedule = new LineItemBookingSchedule
                 {
                     LineItemID = lineItemId,
                     DelOrColl = bookingType,
                     RetItemID = retItemID,
                     RetVal = retVal,
                     RetStockLocn = retStockLocn,
                     BookingId = bookingId,
                     Quantity = quantity,
                     ItemID = itemID,                   
                     StockLocn = stockLocn,
                     Price = price

                 };

                ctx.LineItemBookingSchedule.InsertOnSubmit(bookingSchedule);
                ctx.SubmitChanges();

            }
        }

        public bool HasOutstanding(string accountNumber)
        {
            using (var ctx = Context.Create())
            {
                return (from l in ctx.LineItem
                        join lb in ctx.LineItemBookingSchedule on l.ID equals lb.LineItemID
                        where l.acctno == accountNumber && lb.Quantity != 0
                        && !ctx.LineItemBookingFailures.Any(f => f.OriginalBookingID == lb.BookingId)         //#15284
                        select lb).Any();
            }
        }

        //#15284 - Check for any orders that are yet to be actioned in the Failed Deliveries \ Collection screen.
        public bool HasOutstandingCollectionsToBeActioned(string accountNumber)
        {
            using (var ctx = Context.Create())
            {
                return (from l in ctx.LineItem
                        join lb in ctx.LineItemBookingSchedule on l.ID equals lb.LineItemID
                        where l.acctno == accountNumber
                        && lb.DelOrColl == DelType.Collection
                        && lb.Quantity != 0
                        && ctx.LineItemBookingFailures.Any(f => f.OriginalBookingID == lb.BookingId && f.Actioned == null)
                        select lb).Any();
            }
        }

    }
}
