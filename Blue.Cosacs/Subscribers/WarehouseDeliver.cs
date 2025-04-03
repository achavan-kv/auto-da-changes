using Blue.Cosacs.Repositories;
using Blue.Cosacs.Warehouse.Common;
using STL.BLL;
using STL.DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;

namespace Blue.Cosacs.Subscribers
{
    /// <summary>
    /// Receives a Booking delivered from warehouse.
    /// </summary>
    public class WarehouseDeliver : Hub.Client.Subscriber
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
                        var b = Deserialize<Blue.Cosacs.Messages.Warehouse.WarehouseDeliver>(message);

                        var booking = (from lb in ctx.LineItemBooking
                                       join li in ctx.LineItem on lb.LineItemID equals li.ID
                                       join ls in ctx.LineItemBookingSchedule on lb.ID equals ls.BookingId
                                       join si in ctx.StockInfo on li.ItemID equals si.Id
                                       join sp in ctx.StockInfo on li.ParentItemID equals sp.Id into sParent
                                       from parent in sParent.DefaultIfEmpty()
                                       join sr in ctx.StockInfo on ls.RetItemID equals sr.Id into sReturn
                                       from ret in sReturn.DefaultIfEmpty()
                                       where lb.ID == b.OrigBookingId
                                       && li.ItemID == ls.ItemID && li.stocklocn == ls.StockLocn && li.price == ls.Price

                                       select new
                                       {
                                           BuffBranchNo = li.delnotebranch.HasValue ? li.delnotebranch : 0,
                                           DelNoteBranch = li.delnotebranch.HasValue ? li.delnotebranch : 0,
                                           BuffNo = b.Id,
                                           AcctNo = li.acctno,
                                           AgrmtNo = li.agrmtno,
                                           DelOrColl = ls.DelOrColl == "D" ? "Delivery" : ls.DelOrColl == "C" || ls.DelOrColl == "R" ? "Collection" : "",       //IP - 15/06/12 - #10387
                                           ItemNo = si.IUPC == null ? string.Empty : si.IUPC,
                                           ParentItemNo = parent.IUPC == null ? string.Empty : parent.IUPC,
                                           Quantity = ls.DelOrColl == "C" ? b.Quantity * -1 : b.Quantity,
                                           StockLocn = li.stocklocn,
                                           DateDelPlan = li.datereqdel,
                                           RetStockLocn = ls.RetStockLocn == null ? (short)0 : ls.DelOrColl == "C" ? (short)ls.RetStockLocn.Value : (short)0,  //#16970   //IP - 15/06/12 - #10387
                                           RetItemNo = ret.IUPC == null ? string.Empty : ret.IUPC,      //IP - 15/06/12 - #10387
                                           RetVal = ls.RetVal == null ? 0m : ls.RetVal.Value, //#16970 //IP - 15/06/12 - #10387
                                           VanNo = string.Empty,
                                           LoadNo = 0,
                                           ItemType = li.itemtype,
                                           DelType = ls.DelOrColl,                  //IP - 15/06/12 - #10387
                                           PickListNumber = 0,
                                           ContractNo = li.contractno,
                                           DatePrinted = DateTime.Today,
                                           ItemID = li.ItemID,
                                           ParentItemID = li.ParentItemID,
                                           RetItemID = ls.RetItemID == null ? (int)0 : ls.RetItemID.Value, //#16970 //IP - 15/06/12 - #10387
                                           TotalQuantity = li.quantity
                                       }).ToList();

                        var country = ctx.Country.FirstOrDefault();

                        if (booking.Count > 0)
                        {
                            new Blue.Cosacs.InitCountryParamCache().PopulateCacheCountryParams(country.countrycode.Trim());

                            // Need to add user -134 as auto warehouse user.

                            var forDelivery = booking;
                            //var acctNo = Convert.ToString(booking.Select(e => e.AcctNo[0]));

                            var ds = new DataSet();
                            var schedule = forDelivery.ToDataTable();
                            schedule.TableName = "Schedules";
                            ds.Tables.Add(schedule);

                            var allItemsCollected = false;
                            string error;
                            var delNoteBranch = Convert.ToInt16(schedule.Rows[0]["DelNoteBranch"]);
                            var acctNo = Convert.ToString(schedule.Rows[0]["AcctNo"]);

                            BDelivery bdel = new BDelivery();
                            bdel.User = b.ConfirmedBy;
                            bdel.DateDel = b.Date;

                            BAgreement bagreement = new BAgreement();
                            bagreement.Populate(conn, trans, booking.First().AcctNo, booking.First().AgrmtNo);

                            if (booking[0].DelOrColl == DeliveryOrCollectionText.Collection)
                            {
                                var lb = new Blue.Cosacs.Model.LineitemBooking
                                {
                                    AcctNo = booking.First().AcctNo,
                                    AgreementNo = booking.First().AgrmtNo,
                                    ItemId = booking.First().ItemID,
                                    StockLocation = booking.First().StockLocn,
                                    DelLocation = booking.First().BuffBranchNo
                                };

                                //#16309 Message to cancel item on WarrantySale
                                new WarrantyRepository().ReturnItem(booking.First().AcctNo, booking.First().AgrmtNo,
                                     booking.First().ItemID, booking.First().StockLocn, -booking[0].Quantity, conn, trans, int.Parse(booking[0].TotalQuantity.ToString()));

                                new WarrantyRepository().CollectItem(conn, trans, lb, booking[0].Quantity, b.ConfirmedBy, (int?)booking[0].TotalQuantity);      // #17692 #15073 Collect warranties

                                bdel.WarrantyRemoved = true;         // #16909 - force recalc of Service Chg in DeliverImmediately
                            }

                            bdel.DeliverImmediately(conn, trans, acctNo, Convert.ToString(b.ConfirmedBy), b.Date, ds, country.countrycode.Trim(),  //#10787 - added ConfirmedBy
                                delNoteBranch, ref allItemsCollected, out error);

                            //new WarehouseRepository().DeleteLineItemBookingSchedule(conn, trans, b.Id);
                            if (error.Length == 0)
                            {
                                var lb = new Blue.Cosacs.Model.LineitemBooking
                                {
                                    AcctNo = booking.First().AcctNo,
                                    AgreementNo = booking.First().AgrmtNo,
                                    ItemId = booking.First().ItemID,
                                    StockLocation = booking.First().StockLocn,
                                    DelLocation = booking.First().BuffBranchNo,
                                    Quantity = Convert.ToInt16(booking.First().Quantity)
                                };
                                new WarehouseRepository().UpdateLineItemBookingScheduleQuantity(conn, trans, b.OrigBookingId, b.Quantity);
                                if (booking[0].DelOrColl != DeliveryOrCollectionText.Collection)
                                {
                                    new WarrantyRepository().DeliverItem(conn, trans, lb, b.Date.Date, b.ConfirmedBy, bagreement.TaxFree); // #17692 #17506 //this to put a message on the queue for each item sold regardless it has a warranty linked or not
                                }
                            }
                            else
                            {
                                throw new Exception(error);    // #12842 
                            }

                            trans.Commit();
                            conn.Close();
                        }
                        else
                        {
                            throw new Exception("LineItem and LineItemBookingSchedule do not match");    // #12842
                        }
                    }
                }
            }
        }
    }

}
