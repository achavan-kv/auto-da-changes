using System;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Stock.Solr
{
    public static class SolrIndex
    {
        private static Blue.Config.Repositories.Settings cosacsSettings = new Blue.Config.Repositories.Settings();
        private const int indexingReadSize = 50000;

        public static void Index(int[] productId = null)
        {
            var stockItemCount = 0L;
            Dictionary<short, Branch> branch = null;

            using (var scope = Context.Read())
            {
                branch = (from b in scope.Context.Branch
                          select b)
                          .ToDictionary(b => b.branchno);

                if (productId != null)
                {
                    stockItemCount = scope.Context.StockItemView
                        .Where(e => productId.Contains(e.ItemID))
                        .Select(p => p.Id)
                        .Max().Value;
                }
                else
                {
                    stockItemCount = scope.Context.StockItemView
                        .Select(p => p.Id)
                        .Max().Value;
                }
            }

            List<StockItemView> products = null;
            for (int i = 0; i * indexingReadSize < stockItemCount; i++)
            {
                using (var scope = Context.Read())
                {
                    var rowsToSkip = i * indexingReadSize;
                    if (productId != null)
                    {
                        products = (from p in scope.Context.StockItemView
                                        .Where(e => productId.Contains(e.ItemID) && e.Id > rowsToSkip)
                                        .Take(indexingReadSize)
                                    select p).ToList();
                    }
                    else
                    {
                        products = (from p in scope.Context.StockItemView
                                        .Where(e => e.Id > rowsToSkip)
                                        .Take(indexingReadSize)
                                    select p).ToList();
                    }
                }

                var solrProducts = products.Select(p =>
                    new SolrProductRecord
                    {
                        CostPrice = p.CostPrice,
                        TaxInclusivePrice = GetTaxInclusivePrice(p),
                        TaxExclusivePrice = GetTaxExclusivePrice(p),
                        TaxAmount = GetTaxAmount(p),
                        ItemNoWarrantyLink = p.itemno,
                        RefCodeWarrantyLink = p.refcode,
                        ProductItemNo = p.itemno,
                        ProductItemId = p.ItemID,
                        StockBranchNameWarrantyLink = // Must match branch picklist format
                            string.Format("{0} {1}", p.origbr, branch[p.origbr].branchname),
                        StockBranchProduct = p.origbr,
                        StoreType = branch[p.origbr].StoreType,
                        Id = String.Format("Product:{0}-{1}", p.ItemID, p.origbr),
                        Type = "Product",
                        Level_1 = ProductDepartment.Data[p.Department],
                        Level_2 = p.category + " - " + p.CategoryName,
                        Level_3 = p.Class + " - " + p.ClassName,
                        DutyFreePrice = p.unitpricedutyfree,
                        TaxRate = p.taxrate,
                        Description1 = p.itemdescr1,
                        Description2 = p.itemdescr2,
                        VendorEAN = p.VendorEAN,
                        Brand = p.Brand,
                        PosDescription = p.ItemPOSDescr,
                    }).ToList();

                new Blue.Solr.WebClient().Update(solrProducts);
            }
        }

        private static decimal GetTaxExclusivePrice(StockItemView product)
        {
            if (!product.unitpricecash.HasValue)
            {
                return 0M;
            }

            var taxInclusivePrice = GetTaxInclusivePrice(product);
            var taxAmount = GetTaxAmount(product);

            return taxInclusivePrice - taxAmount;
        }

        private static decimal GetTaxInclusivePrice(StockItemView product)
        {
            var ret = 0M;

            if (!product.unitpricecash.HasValue)
            {
                return ret;
            }          

            switch (cosacsSettings.TaxType)
            {
                case "E":
                    var taxAmount = GetTaxAmount(product);

                    ret = product.unitpricecash.Value + taxAmount;
                    break;
                case "I":
                    ret = product.unitpricecash.Value;
                    break;
            }

            return ret;
        }

        private static decimal GetTaxAmount(StockItemView product)
        {
            if (!product.unitpricecash.HasValue)
            {
                return 0M;
            }

            if (cosacsSettings.TaxType == "E")
            {
                return Decimal.Round(product.unitpricecash.Value * Convert.ToDecimal(product.taxrate) / 100, 2);
            }

            return cosacsSettings.TaxType == "I" ?
                Decimal.Round(((product.unitpricecash.Value * Convert.ToDecimal(product.taxrate)) / Convert.ToDecimal(100 + product.taxrate)), 2) : 0M;
        }

        private class SolrProductRecord
        {
            public decimal? CostPrice { get; set; }
            public decimal? TaxInclusivePrice { get; set; }
            public decimal? TaxExclusivePrice { get; set; }
            public decimal? TaxAmount { get; set; }
            public string ProductItemNo { get; set; }
            public int ProductItemId { get; set; }
            public string ItemNoWarrantyLink { get; set; }
            public string RefCodeWarrantyLink { get; set; }
            public string StockBranchNameWarrantyLink { get; set; }
            public short StockBranchProduct { get; set; }
            public string StoreType { get; set; }
            public string Id { get; set; }
            public string Type { get; set; }
            public string Level_1 { get; set; }
            public string Level_2 { get; set; }
            public string Level_3 { get; set; }
            public decimal? DutyFreePrice { get; set; }
            public double TaxRate { get; set; }
            public string Description1 { get; set; }
            public string Description2 { get; set; }
            public string VendorEAN { get; set; }
            public string Brand { get; set; }
            public string PosDescription { get; set; }
        }
    }
}
