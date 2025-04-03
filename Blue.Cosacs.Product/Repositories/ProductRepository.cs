using System;
using System.Collections.Generic;
using System.Linq;
using Blue.Config.Repositories;
using Blue.Cosacs.Stock.Models;
using Blue.Linq;
using Blue.Transactions;

namespace Blue.Cosacs.Stock.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public IList<Product> GetAll()
        {
            using (ReadScope<Context> scope = Context.Read())
                return scope.Context.StockItemView.Select(s => Convert(s)).ToList();
        }

        public IList<string> GetStockItemForValidation(WarrantyProductLinkSearch[] productSearch)
        {
            return GetStockItems(productSearch).Select(i => i.ItemNo).Distinct().ToList();
        }

        private static IEnumerable<ProductLinkValidateView> GetStockItems(IEnumerable<WarrantyProductLinkSearch> productSearch)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.ProductLinkValidateView.Where(GetProductMatchPredicate(productSearch));
            }

        }

        private static Func<ProductLinkValidateView, bool> GetProductMatchPredicate(
            IEnumerable<WarrantyProductLinkSearch> warrantyProduct)
        {
            Func<ProductLinkValidateView, bool> predicate = PredicateBuilder.False<ProductLinkValidateView>();

            foreach (WarrantyProductLinkSearch wp in warrantyProduct)
            {

                var productItemPredicate = PredicateBuilder.True<ProductLinkValidateView>();
                if (wp.Level1IsValid()) // Level_1
                {
                    short tmpLevel_2 = 0;

                    productItemPredicate = productItemPredicate.And(e => e.Department == wp.Level_1);

                    // Only is used if the Level_1 exists and is valid...
                    if (wp.Level2IsValid() && short.TryParse(wp.Level_2, out tmpLevel_2)) // Level_2
                    {
                        productItemPredicate = productItemPredicate.And(e => e.Category == tmpLevel_2);

                        // Only is used if the Level_2 exists and is valid...
                        if (wp.Level3IsValid()) // Level_3
                        {
                            productItemPredicate = productItemPredicate.And(e => e.Class == wp.Level_3);
                        }
                    }
                }

                if (wp.StoreTypeIsValid())
                {
                    productItemPredicate = productItemPredicate.And(e => e.StoreType == wp.StoreType);
                }

                short tmpStockBranch = 0;
                if (wp.StockBranchIsValid() && short.TryParse(wp.StockBranchNameWarrantyLink, out tmpStockBranch))
                {
                    productItemPredicate = productItemPredicate.And(e => e.OrigBr == tmpStockBranch);
                }

                if (wp.ItemNoIsValid())
                {
                    productItemPredicate = productItemPredicate.And(e =>
                        string.Compare(Trim2(e.ItemNo), Trim2(wp.ItemNoWarrantyLink), true) == 0);

                }
                else if (wp.RefCodeIsValid()) // The ref code is not used it the more specific ItemNo is present
                {

                    productItemPredicate = productItemPredicate.And(e =>
                        string.Compare(Trim2(e.RefCode), Trim2(wp.RefCodeWarrantyLink), true) == 0);

                }

                predicate = predicate.Or(productItemPredicate);

            }

            return predicate;
        }

        private static string Trim2(string val)
        {
            if (val != null)
                return val.Trim();
            else
                return string.Empty;
        }

        public IList<Installation> GetInstallations(string itemNumber, short location)
        {
            using (ReadScope<Context> scope = Context.Read())
            {
                List<InstallationItemsView> items = (from i in scope.Context.InstallationItemsView
                                                     where i.ItemNumber == itemNumber && i.InstallationLocation == location
                                                     select i).ToList();

                return (from i in items
                        select new Installation
                        {
                            Id = i.ID,
                            ItemID = i.ItemID,
                            ItemNo = i.itemno,
                            ItemDescription = i.itemdescr1,
                            ItemDescription2 = i.itemdescr2,
                            CostPrice = i.CostPrice,
                            TaxRate = i.taxrate,
                            UnitPriceCash = i.unitpricecash,
                            UnitPriceHP = i.unitpricehp,
                            TaxAmount = i.unitpricecash * System.Convert.ToDecimal(i.taxrate) / 100
                        }).ToList();
            }
        }

        public Product Convert(StockItemView stock)
        {
            if (stock == null)
                return null;

            return new Product
            {
                Brand = stock.Brand,
                Category = stock.category,
                Class = stock.Class,
                ColourCode = stock.ColourCode,
                CostPrice = stock.CostPrice,
                ItemID = stock.ItemID,
                ItemDescription = stock.itemdescr1,
                ItemDescription2 = stock.itemdescr2,
                StockLocation = stock.origbr
            };
        }

        public StockItemViewRelations GetProductRelationsByItemNumber(string itemNumber)
        {
            using (ReadScope<Context> scope = Context.Read())
            {
                return scope.Context.StockItemViewRelations
                    .Where(s => s.ItemNumber == itemNumber)
                    .FirstOrDefault();
            }
        }

        public WarrantyProductLinkSearch FindMatchingProduct(string productNumber, short branchNumber)
        {
            if (string.IsNullOrEmpty(productNumber))
            {
                return null;
            }

            using (ReadScope<Context> scope = Context.Read())
            {
                var branch = scope.Context.Branch
                    .Select(p => new
                    {
                        p.branchno,
                        p.StoreType,
                        p.branchname
                    })
                    .ToDictionary(b => b.branchno);

                var products = (from sw in scope.Context.StockItemWarrantyView
                                where sw.itemno == productNumber && sw.origbr == branchNumber && ((sw.category ?? 0) > 0)
                                select sw)
                               .ToList();

                if (!products.Any())
                {
                    return null;
                }

                var productType = products
                    .First().ProductType;
                var locations = products
                    .Select(p => p.Locationid)
                    .ToList();
                var produstId = products
                    .First().ProductId;

                Tuple<decimal?, decimal?> prices = null;

                if (string.Compare(productType, "Set", true) == 0)
                {
                    prices = scope.Context.StockItemWarrantyPricesSetView
                        .Where(p => locations.Contains(p.LocationId) && p.ProductId == produstId)
                        .Select(p => new
                        {
                            p.UnitPriceHP,
                            p.CostPrice
                        })
                        .ToList()
                        .Select(p => new Tuple<decimal?, decimal?>(p.UnitPriceHP, p.CostPrice))
                        .FirstOrDefault();
                }
                else if (string.Compare(productType, "Combo", true) == 0)
                {
                    prices = scope.Context.StockItemWarrantyPricesComboView
                        .Where(p => locations.Contains(p.LocationId) && p.ProductId == produstId)
                        .Select(p => new
                        {
                            p.UnitPriceHP,
                            p.CostPrice
                        })
                        .ToList()
                        .Select(p => new Tuple<decimal?, decimal?>(p.UnitPriceHP, p.CostPrice))
                        .FirstOrDefault();
                }
                else
                {
                    prices = scope.Context.StockItemWarrantyPricesNotSetComboView
                        .Where(p => locations.Contains(p.LocationId) && p.ProductId == produstId)
                        .Select(p => new
                        {
                            p.UnitPriceCash,
                            p.CostPrice
                        })
                        .ToList()
                        .Select(p => new Tuple<decimal?, decimal?>(p.UnitPriceCash, p.CostPrice))
                        .FirstOrDefault();
                }

                var cashPrice = prices == null
                    ? string.Empty
                    : prices.Item1.HasValue ? prices.Item1.Value.ToString() : string.Empty;

                var costPrice = prices == null
                    ? string.Empty
                    : prices.Item2.HasValue ? prices.Item2.Value.ToString() : string.Empty;

                return products
                    .Select(s => new WarrantyProductLinkSearch
                    {
                        CashPrice = cashPrice,
                        CostPrice = costPrice,
                        ItemNoWarrantyLink = s.itemno,
                        RefCodeWarrantyLink = s.Refcode,
                        Level_1 = s.DepartmentId.HasValue ? s.DepartmentId.ToString() : string.Empty,
                        Level_2 = s.CategoryId.HasValue ? s.CategoryId.ToString() : string.Empty,
                        Level_3 = s.ClassId.HasValue? s.ClassId.ToString() : string.Empty,
                        StockBranchNameWarrantyLink = s.origbr.HasValue ? 
                            string.Format("{0} {1}", s.origbr, branch[s.origbr.Value] == null ? "" : branch[s.origbr.Value].branchname) : "",
                        StockBranchProduct = s.origbr > 0 ? (int?)s.origbr : null,
                        StoreType = branch[s.origbr.Value].StoreType,
                        Description = s.itemdescr1,
                        Category = s.CategoryName
                    })
                    .First();
            }
        }

        public virtual List<WarrantyProductLinkSearch> GetProductsByCategory(string department, short category, short branchNumber)
        {
            using (var scope = Context.Read())
            {
                var stockItemView = scope.Context.StockItemView
                    .Where(e => e.Department == department && e.category == category && e.origbr == branchNumber && e.warrantable.Equals("Y"))
                    .Select(s => new
                    {
                        s.Id,
                        s.unitpricecash,
                        s.CostPrice,
                        s.itemno,
                        s.refcode,
                        s.Department,
                        s.category,
                        s.CategoryName,
                        s.Class,
                        s.ClassName,
                        s.origbr,
                        s.itemdescr1
                    });

                var branch = scope.Context.Branch
                    .Select(p => new
                    {
                        p.branchno,
                        p.StoreType,
                        p.branchname
                    })
                    .ToDictionary(b => b.branchno);

                return stockItemView.ToList()
                    .Select(s => new WarrantyProductLinkSearch
                    {
                        CashPrice = s.unitpricecash.HasValue ? s.unitpricecash.Value.ToString() : string.Empty,
                        CostPrice = s.CostPrice.ToString(),
                        ItemNoWarrantyLink = s.itemno,
                        RefCodeWarrantyLink = s.refcode,
                        Level_1 = s.Department,
                        Level_2 = s.category > 0 ? s.category.ToString() : string.Empty,
                        Level_3 = s.Class,
                        StockBranchNameWarrantyLink = string.Format("{0} {1}", s.origbr, branch[s.origbr].branchname),
                        StockBranchProduct = s.origbr > 0 ? (int?)s.origbr : null,
                        StoreType = branch[s.origbr].StoreType,
                        Description = s.itemdescr1,
                        Category = s.CategoryName
                    }).ToList();

            }
        }
    }
}