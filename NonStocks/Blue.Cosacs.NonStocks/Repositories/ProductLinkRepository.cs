using Blue.Cosacs.NonStocks.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.NonStocks
{
    public class ProductLinkRepository : IProductLinkRepository
    {
        public Models.Link SaveLink(Models.Link link)
        {
            var linkEnt = (NonStocks.Link)null;
            var retLink = (Models.Link)null;

            using (var scope = Context.Write())
            {
                if (link.Id > 0)
                {
                    linkEnt = (from l in scope.Context.Link
                               where l.Id == link.Id
                               select l)
                               .FirstOrDefault<NonStocks.Link>();
                }
                else
                {
                    linkEnt = Models.Link.ToEntity(link);
                    scope.Context.Link.Add(linkEnt);
                    scope.Context.SaveChanges();
                }

                #region Validate javascript client save rules

                if (link.EffectiveDate != linkEnt.EffectiveDate && link.EffectiveDate < DateTime.Now.Date.AddDays(1))
                {
                    throw new ArgumentException(
                        "The link's EffectiveDate can only be created/saved with a value " +
                        "bigger that today's date (please choose the dates tomorrow or after).");
                }

                if (link.linkProducts.Count == 0)
                {
                    throw new ArgumentException("Please create at least one valid product link.");
                }

                if (link.linkNonStocks.Count == 0)
                {
                    throw new ArgumentException("Please select at least one valid non-stock item.");
                }

                if (link.Name.Length <= 0)
                {
                    throw new ArgumentException("Cannot save empty names.");
                }

                #endregion

                if (linkEnt != null)
                {
                    linkEnt.Name = link.Name;
                    linkEnt.EffectiveDate = link.EffectiveDate;

                    var newLinkProductsList = UpdateDeleteLinkProdutRows(linkEnt.Id, link.linkProducts);
                    var newLinkNonStockList = UpdateDeleteLinkNonStockRows(linkEnt.Id, link.linkNonStocks);

                    retLink = Models.Link.ToModel(
                        linkEnt,
                        Models.LinkProduct.AllToModel(newLinkProductsList),
                        Models.LinkNonStock.AllToModel(newLinkNonStockList));

                    scope.Context.SaveChanges();
                    scope.Complete();
                }
            }

            return retLink;
        }

        public Models.Link LoadLink(int id)
        {
            if (id > 0)
            {
                return LoadLinks(new int[] { id }, null)
                    .FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public List<Models.Link> LoadLinks(int[] ids, NonStockLinkSearch search)
        {
            var loadAllLinks = false;
            if (ids == null)
            {
                loadAllLinks = true;
                ids = new int[] { };
            }

            var retValue = new List<Models.Link>();

            using (var scope = Context.Read())
            {
                var tmpLinks = (from l in scope.Context.Link
                                join p in scope.Context.LinkProduct on l.Id equals p.LinkId
                                join ns in scope.Context.LinkNonStock on l.Id equals ns.LinkId
                                where loadAllLinks || ids.Contains(l.Id)
                                select new Models.Link()
                                {
                                    Id = l.Id,
                                    Name = l.Name,
                                    EffectiveDate = l.EffectiveDate,
                                })
                                .Distinct();

                if (search != null) // Paged link loading
                {
                    search.PageSize = search.PageSize > 0 ? search.PageSize : 10;
                    search.PageIndex = search.PageIndex > 0 ? search.PageIndex : 1;

                    tmpLinks = tmpLinks
                        .OrderByDescending(e => e.Id)
                        .Skip(search.PageSize * (search.PageIndex - 1))
                        .Take(search.PageSize);
                }

                var links = tmpLinks.ToList();
                if (links.Count > 0)
                {
                    var allLinkProducts = (from p in scope.Context.LinkProduct
                                           select new Models.LinkProduct()
                                           {
                                               Id = p.Id,
                                               LinkId = p.LinkId,
                                               Level_1 = p.Level_1,
                                               Level_2 = p.Level_2,
                                               Level_3 = p.Level_3,
                                               Level_4 = p.Level_4,
                                               Level_5 = p.Level_5,
                                               Order = p.Order.HasValue ? p.Order.Value : (Int16)0,
                                           })
                                           .ToList();

                    var allLinkNonStocks = (from ns in scope.Context.LinkNonStock
                                            select new Models.LinkNonStock()
                                            {
                                                Id = ns.Id,
                                                LinkId = ns.LinkId,
                                                NonStockId = ns.NonStockId,
                                                Order = ns.Order.HasValue ? ns.Order.Value : (Int16)0,
                                            })
                                            .ToList();

                    foreach (var link in links)
                    {
                        link.linkProducts = (from p in allLinkProducts
                                             where link.Id == p.LinkId
                                             select p
                                             )
                                             .ToList();

                        link.linkNonStocks = (from ns in allLinkNonStocks
                                              join s in scope.Context.NonStock on ns.NonStockId equals s.Id
                                              where link.Id == ns.LinkId
                                              select new Models.LinkNonStock()
                                              {
                                                  Id = ns.Id,
                                                  LinkId = ns.LinkId,
                                                  NonStockId = ns.NonStockId,
                                                  Order = ns.Order,
                                                  NonStockObj = new Models.NonStockModel()
                                                  {
                                                      Id = s.Id,
                                                      Type = s.Type,
                                                      SKU = s.SKU,
                                                      VendorUPC = s.VendorUPC,
                                                      ShortDescription = s.ShortDescription,
                                                      LongDescription = s.LongDescription,
                                                      Active = s.Active,
                                                      TaxRate = s.TaxRate,
                                                  }
                                              })
                                              .ToList();
                    }

                    retValue = links;
                }
            }

            return retValue;
        }

        public List<Models.Link> LoadAllLinks(NonStockLinkSearch search)
        {
            // if null is passed instead of an array of ints, all links will be loaded
            return LoadLinks(null, search);
        }

        public bool DeleteLink(int id)
        {
            using (var scope = Context.Write())
            {
                var linkEnt = (from l in scope.Context.Link
                               where l.Id == id
                               select l)
                               .FirstOrDefault<NonStocks.Link>();

                if (linkEnt != null)
                {
                    var emptyNonStocksList = new List<Models.LinkNonStock>() { };
                    UpdateDeleteLinkNonStockRows(linkEnt.Id, emptyNonStocksList); // deletes all links
                    var emptyProductsList = new List<Models.LinkProduct>() { };
                    UpdateDeleteLinkProdutRows(linkEnt.Id, emptyProductsList); // deletes all links

                    // finnaly remove the link
                    scope.Context.Link.Remove(linkEnt);
                    scope.Context.SaveChanges();
                    scope.Complete();

                    return true;
                }

                return false;
            }
        }

        public int[] GetIdsUsingNameDateFilters(string name, DateTime? dateFrom, DateTime? dateTo)
        {
            using (var scope = Context.Read())
            {
                var links = GetLinksApplyingFilters(name, dateFrom, dateTo);

                var retLinkIds = links
                    .Select(e => e.Id)
                    .ToArray();

                return retLinkIds;
            }
        }

        public int GetLinkCount(NonStockLinkSearch search)
        {
            using (var scope = Context.Read())
            {
                if (search.HasFilter())
                {
                    return (from l in GetLinksApplyingFilters(search.Name, search.DateFrom, search.DateTo)
                           select l)
                           .Count();
                }
                else
                {
                    return (from l in scope.Context.Link
                            select l)
                            .Count();
                }
            }
        }

        private static IQueryable<Link> GetLinksApplyingFilters(string name, DateTime? dateFrom, DateTime? dateTo)
        {
            using (var scope = Context.Read())
            {
                var links = (from l in scope.Context.Link
                             select l);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    links = links.Where(e => e.Name.Contains(name));
                }
                ;
                if (dateFrom.HasValue)
                {
                    links = links.Where(e => e.EffectiveDate >= dateFrom);
                }

                if (dateTo.HasValue)
                {
                    links = links.Where(e => e.EffectiveDate <= dateTo);
                }
                return links;
            }
        }

        private List<NonStocks.LinkNonStock> UpdateDeleteLinkNonStockRows(int entId,
            List<Models.LinkNonStock> links)
        {
            var newLinkNonStockList = new List<NonStocks.LinkNonStock>();

            using (var scope = Context.Write())
            {
                var tmpLinkNonStock = (from l in scope.Context.LinkNonStock
                                       where l.LinkId == entId
                                       select l);

                if (tmpLinkNonStock.Count() > 0)
                {
                    scope.Context.LinkNonStock.RemoveRange(tmpLinkNonStock);
                }
                scope.Context.SaveChanges();

                if (links.Count > 0)
                {
                    for (int i = 0; i < links.Count; i++)
                    {
                        var tmp = links[i];
                        tmp.LinkId = entId;
                        tmp.Order = (Int16)i;
                        newLinkNonStockList.Add(Models.LinkNonStock.ToEntity(tmp));
                    }
                    scope.Context.LinkNonStock.AddRange(newLinkNonStockList);
                }
                scope.Context.SaveChanges();

                scope.Complete();
            }

            return newLinkNonStockList;
        }

        private List<NonStocks.LinkProduct> UpdateDeleteLinkProdutRows(int entId,
            List<Models.LinkProduct> links)
        {
            var newLinkProductsList = new List<NonStocks.LinkProduct>();

            using (var scope = Context.Write())
            {
                var tmpLinkProduct = (from l in scope.Context.LinkProduct
                                      where l.LinkId == entId
                                      select l);

                if (tmpLinkProduct.Count() > 0)
                {
                    scope.Context.LinkProduct.RemoveRange(tmpLinkProduct);
                }
                scope.Context.SaveChanges();

                if (links.Count > 0)
                {
                    for (int i = 0; i < links.Count; i++)
                    {
                        var tmp = links[i];
                        tmp.LinkId = entId;
                        tmp.Order = (Int16)i;
                        newLinkProductsList.Add(Models.LinkProduct.ToEntity(tmp));
                    }

                    scope.Context.LinkProduct.AddRange(newLinkProductsList);
                }
                scope.Context.SaveChanges();

                scope.Complete();
            }

            return newLinkProductsList;
        }

        public Link GetLink(int linkId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Link.Find(linkId);
            }
        }


    }
}
