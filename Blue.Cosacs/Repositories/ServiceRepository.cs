using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blue.Cosacs.Shared.Services.Service;
using System.Data.SqlClient;

namespace Blue.Cosacs.Repositories
{
    public class ServiceRepository
    {
        public GetItemsIssuedForReplacementResponse GetItemsForReplacement(short branch)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
                {
                    return new GetItemsIssuedForReplacementResponse
                    {
                        itemsForReplacement = (from i in ctx.BERItemsForReplacementView
                                               where i.Branch == branch
                                               select i).AnsiToList(ctx)
                    };
                });
        }

        public void UpdateReplacementActioned(SqlConnection conn, SqlTransaction trans, string acctNo, int serviceRequestNo, int itemId, short stockLocn, int bookingId)
        {
            using (var ctx = Context.Create(conn))
            {
                ctx.Transaction = trans;

                var srSummary = (from s in ctx.SR_Summary
                                 where s.Acctno == acctNo
                                 && s.ServiceRequestNo == serviceRequestNo
                                 && s.ItemId == itemId
                                 && s.StockLocn == stockLocn
                                 select s).SingleOrDefault();

                if (srSummary != null)
                {
                    srSummary.ReplacementActioned = true;
                    srSummary.BookingId = bookingId;
                    srSummary.ReplacementDate = DateTime.Now;
                    ctx.SubmitChanges();
                }
            }
        }

        //Reverse when doing a Cancel Collection Note
        public void ReverseReplacementActioned(SqlConnection conn, SqlTransaction trans, string acctNo, int itemId, short stockLocn, int bookingId)
        {
            using (var ctx = Context.Create(conn))
            {
                ctx.Transaction = trans;

                var srSummary = (from s in ctx.SR_Summary
                                 where s.Acctno == acctNo
                                 && s.BookingId == bookingId
                                 && s.ItemId == itemId
                                 && s.StockLocn == stockLocn
                                 select s).SingleOrDefault();

                if (srSummary != null)
                {
                    srSummary.ReplacementActioned = false;
                    srSummary.BookingId = null;
                    srSummary.ReplacementDate = null;
                    ctx.SubmitChanges();
                }
            }
        }
    }
}
