using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data.SqlClient;
using STL.DAL;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.Subscribers
{
    public class ServiceSummary: Hub.Client.Subscriber
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
                        var b = Deserialize<Blue.Cosacs.Messages.Service.ServiceSummary>(message);

                        var summary = (from s in ctx.SR_Summary
                                       where s.Acctno == b.Acctno && s.ServiceRequestNo == b.ServiceRequestNo
                                       select s).AnsiFirstOrDefault(ctx);

                        if (summary == null)
                        {
                            var serviceSummary = new SR_Summary
                            {
                                Acctno = b.Acctno,
                                ServiceRequestNo = b.ServiceRequestNo,
                                Branch = b.Branch,
                                ItemId = b.ItemId,
                                StockLocn = b.StockLocn,
                                DateLogged = b.DateLogged,
                                DateClosed = b.DateClosed,
                                ReplacementIssued = b.ReplacementIssued                 //#12734
                            };

                            ctx.SR_Summary.InsertOnSubmit(serviceSummary);
                        }
                        else
                        {
                            summary.DateClosed = b.DateClosed;
                        }

                        ctx.SubmitChanges();

                        trans.Commit();
                    }
                }
            }
        }
    }
}
