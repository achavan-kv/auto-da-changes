using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;
using STL.BLL;
using STL.Common.Constants.AccountTypes;
using STL.DAL;

namespace Blue.Cosacs.Subscribers
{
    public class ServiceParts : Hub.Client.Subscriber
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
                        var b = Deserialize<Messages.Service.ServiceParts>(message);

                        if (string.IsNullOrEmpty(b.Account))
                        {
                            throw new Exception("Stock Inventory Account must be populated");
                        }

                        var country = ctx.Country.First();
                        var bsr = new BServiceRequest();
                        new InitCountryParamCache().PopulateCacheCountryParams(country.countrycode.Trim());

                        foreach (var part in b.ServicePart.Where(p => p.StockLocn != null))
                        {
                                var itemId = (from s in ctx.StockInfo
                                              join sq in ctx.StockQuantity on s.Id equals sq.ID
                                              where sq.stocklocn == Convert.ToInt16(part.StockLocn)
                                              && s.IUPC == part.ItemNumber
                                              select s.Id).FirstOrDefault();

                                bsr.SaveServiceLineItem(conn, trans, AT.Special, b.Account,
                                b.ServiceRequestNo, Convert.ToInt16(part.StockLocn),
                                part.ItemNumber, part.Quantity, Convert.ToDecimal(part.Price), itemId);
                        }

                        ctx.SubmitChanges();
                        trans.Commit();

                    }

                }
            }
        }
    }
}


