using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using Blue.Cosacs.Stock;
using Blue.Cosacs.Stock.Repositories;
using Blue.Cosacs.Warranty.Model;
using Blue.Data;
using Blue.Events;
using StructureMap;
using Merch = Blue.Cosacs.Merchandising;

namespace Blue.Cosacs.Warranty.Repositories
{
    public class WarrantyLinkRepository : Blue.Cosacs.Warranty.Repositories.IWarrantyLinkRepository
    {
        private IClock clock;
        private IEventStore audit;
        private const string AuditEventCategory = "Link";

        private readonly IWarrantyPriceRepository warrantyPriceRepository;
        private readonly IWarrantyPromotionRepository warrantyPromotionRepository;

        private readonly Config.Repositories.ISettings cosacsSettings;
        private readonly IProductRepository productRepository;
        private readonly Merch.Repositories.TaxRepository merchandisingTaxRepo;

        public WarrantyLinkRepository(IEventStore audit, IClock clock,
            Blue.Config.Repositories.ISettings cosacsSettings,
            IWarrantyPriceRepository warrantyPriceRepository,
            IWarrantyPromotionRepository warrantyPromotionRepository,
            IProductRepository productRepository,
            Merch.Repositories.TaxRepository merchandisingTaxRepo)
        {
            this.audit = audit;
            this.clock = clock;
            this.warrantyPriceRepository = warrantyPriceRepository;
            this.warrantyPromotionRepository = warrantyPromotionRepository;
            this.cosacsSettings = cosacsSettings;
            this.productRepository = productRepository;
            this.merchandisingTaxRepo = merchandisingTaxRepo;
        }

        public int Save(WarrantyLink warrantyLink)
        {
            int id = 0;
            var validateMsg = ValidateWarrantyLinks(warrantyLink);

            if (!string.IsNullOrEmpty(validateMsg))
            {
                throw new Exception(validateMsg);

            }

            using (var scope = Context.Write())
            {
                var link = scope.Context.Link.Find(warrantyLink.Id);

                if (link != null)
                {
                    scope.Context.Link.Remove(link);
                    scope.Context.SaveChanges();
                }

                link = new Link
                {
                    Name = warrantyLink.Name,
                    EffectiveDate = warrantyLink.EffectiveDate
                };
                scope.Context.Link.Add(link);
                scope.Context.SaveChanges();

                id = link.Id;

                warrantyLink.products.ToList().ForEach(p =>
                {

                    scope.Context.LinkProduct.Add(new LinkProduct
                    {
                        LinkId = link.Id,
                        ItemNumber = p.ItemNoWarrantyLink,
                        RefCode = p.RefCodeWarrantyLink,
                        StoreType = p.StoreType,
                        StockBranch = p.StockBranchNameWarrantyLink,
                        Level_1 = p.Level_1,
                        Level_2 = p.Level_2,
                        Level_3 = p.Level_3,
                    });
                });

                warrantyLink.warranties.ToList().ForEach(w =>
                {
                    var type = GetWarrantyType(w.WarrantyId);

                    scope.Context.LinkWarranty.Add(new LinkWarranty
                    {
                        LinkId = link.Id,
                        ProductMax = w.Max,
                        ProductMin = w.Min,
                        WarrantyId = w.WarrantyId
                    });
                });
                scope.Context.SaveChanges();
                scope.Complete();

                audit.LogAsync(new { Link = link }, EventType.CreateWarrantyLink, AuditEventCategory);
            }

            return id;
        }

        private string ValidateWarrantyLinks(WarrantyLink warrantyLink)
        {
            var validateMsg = "";
            var type = GetWarrantyLinkType(warrantyLink);

            foreach (var productSearch in warrantyLink.products)
            {
                validateMsg = ValidateWarrantyLink(productSearch, type);

                if (!string.IsNullOrEmpty(validateMsg))
                {
                    return validateMsg;
                }
            }

            return validateMsg;

        }

        internal string ValidateWarrantyLink(WarrantyLinkProduct productSearch, string type)
        {
            // Free Warranties can be added to any type of WarrantyLink (extended or instant 
            // replacement links), so no need to check Free Warranties on these WarrantyLinks
            if (type == WarrantyType.Free)
            {
                return string.Empty;
            }

            var currentFilter = MapWarrantyProductLinkFilter(productSearch);

            if (currentFilter == null)
            {
                return string.Empty;
            }

            var currentSet = productRepository.GetStockItemForValidation(currentFilter);
            var filterType = type == WarrantyType.InstantReplacement ?
                WarrantyType.Extended : WarrantyType.InstantReplacement;
            var oppositeSetFilters = GetWarrantyProductLinkFilters(filterType);

            var oppositeSet = productRepository.GetStockItemForValidation(oppositeSetFilters);
            var intersect = currentSet.Intersect(oppositeSet);

            var enumerable = intersect as string[] ?? intersect.ToArray();

            if (!enumerable.Any())
                return string.Empty;

            var matchedItemNos = String.Join(", ", enumerable.ToArray());
            var fromTypeName = WarrantyType.GetNameForType(type);
            var toTypeName = WarrantyType.GetNameForType(filterType);

            return string.Format(
                "The following items: {0} have warranties of type '{1}' that do not match with current warranty type '{2}'",
                matchedItemNos, toTypeName, fromTypeName);
        }

        private WarrantyProductLinkSearch[] MapWarrantyProductLinkFilter(WarrantyLinkProduct productSearch)
        {
            if (productSearch == null)
            {
                return null;
            }

            return new[]
                {
                    new WarrantyProductLinkSearch()
                    {
                        CostPrice = null,
                        CashPrice = null,
                        ItemNoWarrantyLink = productSearch.ItemNoWarrantyLink,
                        Level_1 = productSearch.Level_1,
                        Level_2 = productSearch.Level_2,
                        Level_3 = productSearch.Level_3,
                        RefCodeWarrantyLink = productSearch.RefCodeWarrantyLink,
                        StockBranchNameWarrantyLink =null,
                        StockBranchProduct = productSearch.StockBranchNameWarrantyLink,
                        StoreType = productSearch.StoreType
                    }
                };
        }

        private WarrantyProductLinkSearch[] GetWarrantyProductLinkFilters(string wType)
        {
            var ret = new List<WarrantyProductLinkSearch>();

            using (var scope = Context.Read())
            {
                ret = (from link in scope.Context.LinkWarrantyTypeView
                       where link.TypeCode == wType
                       select new WarrantyProductLinkSearch()
                         {
                             CostPrice = null,
                             CashPrice = null,
                             ItemNoWarrantyLink = link.ItemNumber,
                             Level_1 = link.Department,
                             Level_2 = link.Category,
                             Level_3 = link.Class,
                             RefCodeWarrantyLink = link.RefCode,
                             StockBranchNameWarrantyLink = null,
                             StockBranchProduct = link.StockBranch,
                             StoreType = link.StoreType
                         }).ToList();

            }

            return ret.ToArray();
        }

        public string ValidateNewWarrantyLink(int warrantyId, WarrantyLink warrantyLink)
        {
            var isValidate = "";

            if (!DoesWarrantyLinkHaveWarranties(warrantyLink))
            {
                return isValidate;
            }

            var type = GetWarrantyType(warrantyId);

            if (type == WarrantyType.Free)
            {
                return isValidate;
            }

            foreach (var w in warrantyLink.warranties)
            {
                // If the WarrantyType isn't set, fix it...
                if (w.WarrantyType == null)
                {
                    w.WarrantyType = GetWarrantyType(w.WarrantyId);
                }
            }

            //If the type of current warranty is Extended then check for Instant Replacement warranties and vice versa
            var result = warrantyLink.warranties.Any(w =>
                w.WarrantyType != WarrantyType.Free &&
                w.WarrantyType == (type == WarrantyType.Extended ?
                    WarrantyType.InstantReplacement : WarrantyType.Extended));

            if (result)
            {
                isValidate = GetValidationMsg(type);
            }

            return isValidate;


        }

        private string GetValidationMsg(string type)
        {
            var fromTypeName = WarrantyType.GetNameForType(type);
            var toType = type == WarrantyType.Extended ? WarrantyType.InstantReplacement : WarrantyType.Extended;
            var toTypeName = WarrantyType.GetNameForType(toType);

            return string.Format("You cannot add a warranty of type '{0}' while there are already warranties of type '{1}'.",
                fromTypeName, toTypeName);
        }

        private bool DoesWarrantyLinkHaveWarranties(WarrantyLink warrantyLink)
        {
            return (warrantyLink != null && warrantyLink.warranties != null &&
                warrantyLink.warranties.Count() > 0);
        }

        public string GetWarrantyType(int warrantyId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Warranty.Where(w => w.Id == warrantyId)
                    .Select(w => w.TypeCode).SingleOrDefault();
            }
        }

        private string GetWarrantyLinkType(WarrantyLink warrantyLink)
        {

            return (from w in warrantyLink.warranties
                    where w.WarrantyType != WarrantyType.Free
                    select string.IsNullOrEmpty(w.WarrantyType) ?
                        GetWarrantyType(w.WarrantyId) : w.WarrantyType).FirstOrDefault();

        }

        public IPagedSearchResults<Model.WarrantyLink> Get(WarrantyLinkSearch search)
        {
            if (search == null)
                search = new WarrantyLinkSearch
                {
                };

            using (var scope = Context.Read())
            {
                DateTime? effectiveDateTo = search.EffectiveStartTo;

                var links = search.Page(
                             from link in scope.Context.Link
                             where ((search.Id == null || search.Id.HasValue && search.Id.Value == link.Id)
                                 && (search.Name == null || search.Name == "" || link.Name.Contains(search.Name))
                                 && (search.EffectiveStartFrom == null || search.EffectiveStartFrom.HasValue
                                 && search.EffectiveStartFrom.Value <= link.EffectiveDate)
                                 && (effectiveDateTo == null || effectiveDateTo.HasValue
                                 && effectiveDateTo.Value >= link.EffectiveDate))
                             orderby link.EffectiveDate descending, link.Id descending
                             select new Model.WarrantyLink
                             {
                                 Id = link.Id,
                                 Name = link.Name,
                                 EffectiveDate = link.EffectiveDate
                             });

                var linkIds = links.Page.Select(l => l.Id).ToArray();

                var products = (from p in scope.Context.LinkProduct
                                where linkIds.Contains(p.LinkId)
                                select new Model.WarrantyLinkProduct
                                {
                                    ItemNoWarrantyLink = p.ItemNumber,
                                    RefCodeWarrantyLink = p.RefCode,
                                    StockBranchNameWarrantyLink = p.StockBranch,
                                    StoreType = p.StoreType,
                                    LinkId = p.LinkId,
                                    ProductId = p.Id,
                                    Level_1 = p.Level_1,
                                    Level_2 = p.Level_2,
                                    Level_3 = p.Level_3
                                }).ToList();

                var plookup = products.ToLookup(p => p.LinkId);

                var warranties = (from lw in scope.Context.LinkWarranty
                                  join w in scope.Context.Warranty on lw.WarrantyId equals w.Id
                                  where linkIds.Contains(lw.LinkId)
                                  select new WarrantyLinkWarranty
                                  {
                                      LinkId = lw.LinkId,
                                      WarrantyId = lw.WarrantyId,
                                      Max = lw.ProductMax,
                                      Min = lw.ProductMin,
                                      WarrantyDescription = w.Description,
                                      WarrantyName = w.Number,
                                      Deleted = w.Deleted,
                                      WarrantyType = w.TypeCode
                                  }).ToLookup(w => w.LinkId);

                links.Page.ForEach(l =>
                {
                    l.products = plookup[l.Id].ToArray();
                    l.warranties = warranties[l.Id].ToArray();
                });

                return links;
            }
        }

        public void Delete(int id)
        {
            using (var scope = Context.Write())
            {
                var _return = scope.Context.Link.Find(id);
                if (_return != null)
                {
                    scope.Context.Link.Remove(_return);
                }

                audit.LogAsync(new { Link = _return }, EventType.DeleteWarrantyLink, AuditEventCategory);

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public WarrantySearchResult Search(WarrantySearchByProduct search)
        {
            using (var scope = Context.Read())
            {
                var productWithNoWarranties = new List<WarrantyProductLinkSearch>();

                var items = GetItems(search, ref productWithNoWarranties);

                var wIds = items.Select(i => i.Id)
                    .Distinct()
                    .ToList();

                var tags = (from ts in scope.Context.WarrantyTags
                            join t in scope.Context.Tag on ts.TagId equals t.Id
                            where wIds.Contains(ts.WarrantyId)
                            select new
                            {
                                t.LevelId,
                                t.Name,
                                t.Id,
                                ts.WarrantyId
                            }).ToLookup(t => t.WarrantyId);

                items.ForEach(i =>
                {
                    i.WarrantyTags = tags[i.Id].Select(t => new Model.Warranty.Tag
                    {
                        LevelId = t.LevelId,
                        TagName = t.Name,
                        TagId = t.Id
                    }).ToList();
                });


                return new WarrantySearchResult
                {
                    Items = items.ToArray(),
                    ProductPrice = search.PriceVATEx.HasValue ? search.PriceVATEx.Value : 0,
                    ItemsWithoutWarranties = productWithNoWarranties.ToArray()
                };
            }
        }

        /// CR - Product warranty association need to populate based on warrantable status of product.
        /// <summary>
        /// This method is used to get warrantable status by product SKU.
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        public bool GetProductWarrantableStatus(string sku)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.GetProductWarrantableStatus(sku).Result;
            }
        }

        private List<WarrantyLinkAggregate> GetItems(WarrantySearchByProduct search,
            ref List<WarrantyProductLinkSearch> productWithNoWarranties)
        {
            using (var scope = Context.Read())
            {
                var isAllSearch = false;
                var items = new List<WarrantyLinkAggregate>();
                var products = new List<WarrantyProductLinkSearch>();
                var location = (from b in scope.Context.BranchLookup
                                where b.branchno == search.Location
                                select b).FirstOrDefault();

                if (location == null)
                {
                    return items;
                }

                if (string.IsNullOrEmpty(search.Product))
                {
                    if (search.HasValidCategory)
                    {
                        products = productRepository.GetProductsByCategory(search.Department, search.CategoryId,
                                                                           location.branchno);
                        isAllSearch = true;
                    }
                }
                else
                {
                    // CR - Product warranty association need to populate based on warrantable status of product.
                    // If selected product is warrantable then only allow to return associated warranties.
                    if (GetProductWarrantableStatus(search.Product))
                    {
                        var product = productRepository.FindMatchingProduct(search.Product, location.branchno);

                        if (product != null)
                        {
                            products.Add(product);
                        }
                    }
                }                

                var allProductWarrantyLinks = GetProductWarrantyLink2(
                    string.IsNullOrEmpty(search.WarrantyTypeCode) ? "" : search.WarrantyTypeCode.Trim(),
                    search.Date.HasValue ? search.Date.Value : clock.Now);


                items.AddRange(products
                    .Select(p => GetItemDetails(search, p, allProductWarrantyLinks))
                    .SelectMany(p => p.Select(item => item))
                    .ToList());

                if (isAllSearch)
                {
                    productWithNoWarranties = products
                        .Where(p => items.All(p2 => p2.ProductItemNo != p.ItemNoWarrantyLink))
                        .ToList();
                }

                return items;
            }
        }

        private ILookup<Tuple<decimal, decimal>, ProductWarrantyLinkView> GetProductWarrantyLink2(
            string warrantyTypeCode, DateTime searchDate)
        {
            using (var scope = Context.Read())
            {
                var typeCode = string.IsNullOrEmpty(warrantyTypeCode) ? string.Empty : warrantyTypeCode.Trim();

                var links = scope.Context.ProductWarrantyLinkView
                    .Where(p => p.EffectiveDate <= searchDate);


                if (!string.IsNullOrEmpty(typeCode))
                {
                    links = links.Where(p => p.WarrantyTypeCode == typeCode);
                }

                return links.ToLookup(p => new Tuple<decimal, decimal>(p.ProductMax, p.ProductMin));
            }
        }

        internal List<WarrantyLinkAggregate> GetItemDetails(WarrantySearchByProduct search,
            WarrantyProductLinkSearch product, ILookup<Tuple<decimal, decimal>,
            ProductWarrantyLinkView> productWarrantyLinks)
        {
            decimal price = 0;


            decimal.TryParse(product.CashPrice, out price);
            

            var links = productWarrantyLinks
                .Where(p => price >= p.Key.Item2 && price <= p.Key.Item1)
                .SelectMany(p => p.Select(item => item))
                .ToList();

            var resultLinkFilteredByBranch = links.Where(e =>
                e.StoreType == (
                    // stockbranch is more specific/prioritary than storetype, so when the stockbranch exists
                    // we'll ignore the storetype (compare it with itself, so it's always true - hence ignored)
                e.StockBranch.HasValue ? e.StoreType : (IsEmpty(e.StoreType) ?
                e.StoreType : product.StoreType))
                && e.StockBranch == (
                    // when the link has a stockbranch value we'll try to match it, otherwise we'll ignore it
                    // (compare it with itself, so it's always true - hence ignored)
                e.StockBranch.HasValue ? product.StockBranchProduct : e.StockBranch)
            ).ToList();

            var resultLinksFilterItemNumberAndRefCode = resultLinkFilteredByBranch.Where(e =>
                {
                    //trim the values as convert then to empty string in case of null
                    var refcode = ConvertNullTopEmpty(e.RefCode);
                    var itemNumber = ConvertNullTopEmpty(e.ItemNumber);

                    var RefCodeWarrantyLink = ConvertNullTopEmpty(product.RefCodeWarrantyLink);
                    var ItemNoWarrantyLink = ConvertNullTopEmpty(product.ItemNoWarrantyLink);

                    var returnValue =
                        refcode == (
                        // itemnumber is more specific/prioritary than refcode, so when itemnumber exists
                        // we'll ignore the refcode (compare it with itself, so it's always true - hence ignored)
                        HasValue(itemNumber) ? refcode : (IsEmpty(refcode) ?
                            refcode : RefCodeWarrantyLink))
                        && itemNumber == (
                        // when the link has a itemnumber value we'll try to match it, otherwise we'll ignore it
                        // (compare it with itself, so it's always true - hence ignored)
                        HasValue(itemNumber) ? ItemNoWarrantyLink : itemNumber);

                    return returnValue;
                }
            ).ToList();

            var resultLinksFilterDepCatClassLevels = resultLinksFilterItemNumberAndRefCode.Where(e =>
                // when one of the link levels has a value we'll try to match it, otherwise we'll ignore it
                // (compare it with itself, so it's always true - hence ignored)
                e.Level1 == (HasValue(e.Level1) ? product.Level_1 : e.Level1)
                && e.Level2 == (HasValue(e.Level2) ? product.Level_2 : e.Level2)
                && e.Level3 == (HasValue(e.Level3) ? product.Level_3 : e.Level3)
            ).ToList();

            var TaxRate = GetCountryTaxRate();

            var result = (from l in resultLinksFilterDepCatClassLevels
                          select new WarrantyLinkAggregate
                          {
                              Description = l.WarrantyDescription,
                              Id = l.WarrantyId,
                              TypeCode = l.WarrantyTypeCode,
                              Length = l.WarrantyLenght,
                              Number = l.WarrantyNumber,
                              TaxRate = l.WarrantyTaxRange.HasValue ? l.WarrantyTaxRange : TaxRate,
                              LinkId = l.LinkId,
                              LinkName = l.LinkName,
                              IsDeleted = false,
                              ProductMatch = l.ItemNumber == product.ItemNoWarrantyLink,
                              LevelMatch = LevelMatching(l.Level3, product.Level_3, l.Level2, product.Level_2),
                              ProductItemNo = product.ItemNoWarrantyLink,
                              ProductCategory = product.Category,
                              ProductDescription = product.Description,
                              ProductRetailPrice = product.CashPrice
                          }).ToList();

            return result;
        }

        private string ConvertNullTopEmpty(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.Trim();
        }

        private bool HasValue(string val)
        {
            return !string.IsNullOrEmpty(val);
        }

        private bool IsEmpty(string val)
        {
            return string.IsNullOrEmpty(val);
        }

        public WarrantySearchByProductResult SearchByProduct(WarrantySearchByProduct search, string typeCode = "")
        {
            var warrantyLinks = Search(search);
            var simulatorDate = search.Date.HasValue ? search.Date.Value : clock.Now;
            simulatorDate = simulatorDate.Date; // the warranty simulator doesn't use time, only the date
            typeCode = string.IsNullOrEmpty(search.WarrantyTypeCode) ?
                typeCode.Trim().ToUpper() : search.WarrantyTypeCode.Trim();

            if ((warrantyLinks.Items == null || !warrantyLinks.Items.Any()) &&
                (warrantyLinks.ItemsWithoutWarranties == null || !warrantyLinks.ItemsWithoutWarranties.Any()))
                return null;

            var warrantIds = warrantyLinks.Items.Select(i => i.Id).Distinct().ToList();
            var prices = warrantyPriceRepository.GetWarrantyPrices(warrantIds, search.Location, simulatorDate);
            var promotions = warrantyPromotionRepository.GetPromotionAggregate(search.Location, simulatorDate);

            var items = from i in warrantyLinks.Items
                        join p in prices on i.Id equals p.WarrantyId
                        select new WarrantyLinkResult
                        {
                            warrantyLink = i,
                            price = p,
                            WarrantyProductPricePercentage =
                                p.RetailPrice.HasValue && i.ProductRetailPriceDecimal > 0 ?
                                    p.RetailPrice.Value / i.ProductRetailPriceDecimal * 100 : 0,
                            promotion = this.CalculatePromotions(i, p, promotions)
                        };

            items = (from i in items
                     where (string.IsNullOrEmpty(typeCode) || i.warrantyLink.TypeCode == typeCode)
                           && ((i.price != null && i.price.EffectiveDate <= simulatorDate) || i.price == null)
                     select i).ToList();

            return new WarrantySearchByProductResult
            {
                ProductPrice = warrantyLinks.ProductPrice,
                Items = items,
                ProductSearch = search,
                ItemsWithoutWarranties = warrantyLinks.ItemsWithoutWarranties
            };
        }

        private PromotionCalculatedPrice CalculatePromotions(WarrantyLinkAggregate warranty,
            WarrantyCalculatedPrice price, IList<PromotionAggregate> promotions)
        {
            var query = from p in promotions.Select(p => p.CalculatePromos(warranty, price))
                        where p != null
                        orderby p.LevelMatch descending, p.Price ascending
                        select p;
            return query.FirstOrDefault();
        }

        // Yes this is shit. But can't think of another way. If you feel the need please rewrite.
        private int LevelMatching(string linkProductLevel3, string productLevel3,
            string linkProductLevel2, string productLevel2)
        {
            if (string.IsNullOrWhiteSpace(linkProductLevel3) && linkProductLevel3 == productLevel3)
                return 3;
            if (string.IsNullOrWhiteSpace(linkProductLevel2) && linkProductLevel2 == productLevel2)
                return 2;
            return 1;
        }

        public List<WarrantyRenewal> GetRenewals(WarrantyLocation[] warrantyLocation)
        {
            if (warrantyLocation == null || !warrantyLocation.Any())
                return null;

            var renewals = new List<WarrantyRenewal>();
            warrantyLocation.ToList().ForEach(wl =>
            {
                renewals.Add(new WarrantyRenewal
                {
                    WarrantyNumber = wl.WarrantyNumber,
                    Location = wl.Location
                });
            });

            var ids = warrantyLocation.Select(s => s.WarrantyNumber).ToList();

            using (var scope = Context.Read())
            {

                //#17514
                var warrantyPrices = (from wp in scope.Context.WarrantyPrice
                                      join r in scope.Context.Renewal on wp.WarrantyId equals r.RenewalId
                                      join w in scope.Context.Warranty on r.WarrantyId equals w.Id
                                      join nw in scope.Context.Warranty on r.RenewalId equals nw.Id
                                      where ids.Contains(w.Number)
                                      select wp.WarrantyId)
                                   .ToList();

                var renewalList = (from r in scope.Context.Renewal
                                   join w in scope.Context.Warranty on r.WarrantyId equals w.Id
                                   join nw in scope.Context.Warranty on r.RenewalId equals nw.Id
                                   //join p in scope.Context.WarrantyPrice on r.RenewalId equals p.WarrantyId        //#17514 - commented out // #16647
                                   where warrantyPrices.Contains(nw.Id)                                              //#17514
                                   && ids.Contains(w.Number)
                                   group nw by w.Number into g
                                   select new
                                   {
                                       warranties = g,
                                       key = g.Key
                                   }).ToDictionary(d => d.key);



                if (renewalList.Count > 0)
                {
                    renewals.ForEach(r =>
                    {
                        r.Warranties = (from rn in renewalList[r.WarrantyNumber].warranties
                                        select new WarrantyRenewalPrice
                                        {
                                            Deleted = rn.Deleted,
                                            Description = rn.Description,
                                            Free = rn.TypeCode == WarrantyType.Free,
                                            Id = rn.Id,
                                            Length = rn.Length,
                                            Number = rn.Number,
                                            TaxRate = rn.TaxRate,
                                            Location = r.Location,
                                            TypeCode = rn.TypeCode
                                        }).ToList();
                    });

                    var newWarrantyLocation = renewals.SelectMany(r => r.Warranties).Select(s =>
                         new WarrantyLocation
                     {
                         Location = s.Location,
                         WarrantyNumber = s.Number
                     });


                    var prices = warrantyPriceRepository.GetWarrantyPrices(newWarrantyLocation);
                    var promotions = warrantyPromotionRepository.GetPromotions(newWarrantyLocation);

                    var calPrice = (from p in prices
                                    join pr in promotions on new
                                    {
                                        WarrantyNumber = p.WarrantyId.ToString(),
                                        Branch = (int?)p.BranchNumber
                                    } equals new
                                    {
                                        pr.WarrantyNumber,
                                        pr.Branch
                                    } into prj
                                    from pj in prj.DefaultIfEmpty()
                                    select new
                                    {
                                        WarrantyId = p.WarrantyId,
                                        Location = p.BranchNumber,
                                        Price = CalPrice(pj, p),
                                        CostPrice = p.CostPrice
                                    });

                    renewals.ForEach(r =>
                    {
                        r.Warranties.ForEach(w =>
                        {
                            w.Price = calPrice
                                .Where(c => c.WarrantyId == w.Id && c.Location == w.Location)
                                .FirstOrDefault().Price;
                            w.CostPrice = calPrice
                                .Where(c => c.WarrantyId == w.Id && c.Location == w.Location)
                                .FirstOrDefault().CostPrice.Value;
                        });
                    });
                }

                return renewals;
            }
        }

        private decimal CalPrice(Model.PromotionBasic promo, Model.WarrantyPrice price)
        {
            if (promo == null && price.RetailPrice.HasValue)
                return price.RetailPrice.Value;

            if (promo.PromoPrecent.HasValue && price.RetailPrice.HasValue)
                return promo.PromoPrecent.Value * price.RetailPrice.Value;

            return promo.PromoPrice.Value;
        }

        private class EventType
        {
            public const string CreateWarrantyLink = "CreateWarrantyProductLink";
            public const string DeleteWarrantyLink = "DeleteWarrantyProductLink";
        }

        private decimal GetCountryTaxRate()
        {
            var currentTaxRateObj = merchandisingTaxRepo.GetCurrent();
            if (currentTaxRateObj != null)
            {
                return merchandisingTaxRepo.GetCurrent().Rate * 100;
            }
            return 0;

        }
    }
}
