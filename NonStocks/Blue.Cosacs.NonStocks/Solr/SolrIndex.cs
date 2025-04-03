using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.NonStocks.Solr
{
    public class SolrIndex
    {
        private const int indexingReadSize = 50000;

        public static void Index(int id)
        {
            Index(new int[] { id });
        }
        public static void Index(int[] nonStockIds = null)
        {
            var nonStocksItemCount = 0;
            using (var scope = Context.Read())
            {
                if (nonStockIds != null)
                {
                    nonStocksItemCount = scope.Context.NonStock
                        .Where(e => nonStockIds.Contains(e.Id))
                        .Select(p => p.Id)
                        .Max();
                }
                else
                {
                    nonStocksItemCount = scope.Context.NonStock
                        .Select(p => p.Id)
                        .Max();
                }
            }

            List<NonStock> nonStocks = null;
            List<NonStockHierarchy> hierarchy = null;
            for (int i = 0; i * indexingReadSize < nonStocksItemCount; i++)
            {
                using (var scope = Context.Read())
                {
                    var rowsToSkip = i * indexingReadSize;
                    if (nonStockIds != null)
                    {
                        nonStocks = (from p in scope.Context.NonStock
                                        .Where(e => nonStockIds.Contains(e.Id) && e.Id > rowsToSkip)
                                        .Take(indexingReadSize)
                                     select p)
                                     .ToList();
                    }
                    else
                    {
                        nonStocks = (from p in scope.Context.NonStock
                                        .Where(e => e.Id > rowsToSkip)
                                        .Take(indexingReadSize)
                                     select p)
                                     .ToList();
                    }

                    hierarchy = (from ns in nonStocks
                                 join hVals in scope.Context.NonStockHierarchy
                                 on ns.Id equals hVals.NonStockId
                                 select hVals)
                                 .ToList();


                }

                var hierarchyLookup = hierarchy.ToLookup(e => e.NonStockId);


                var solrNonStocks = new List<SolrNonStockstRecord>();
                foreach (var s in nonStocks)
                {
                    solrNonStocks.Add(new SolrNonStockstRecord
                    {
                        Id = s.Id,
                        NonStockType = GetFullTypeString(s.Type),
                        SKU = s.SKU,
                        VendorUPC = s.VendorUPC,
                        Description1 = s.ShortDescription,
                        Description2 = s.LongDescription,
                        Active = s.Active ? "Active" : "Non Active",
                        TaxRate = s.TaxRate,
                        Division = GetLevelN(s, hierarchyLookup, 1),
                        Department = GetLevelN(s, hierarchyLookup, 2),
                        Class = GetLevelN(s, hierarchyLookup, 3),
                        MerchandisingLevel_1 = GetLevelN(s, hierarchyLookup, 1),
                        MerchandisingLevel_2 = GetLevelN(s, hierarchyLookup, 2),
                        MerchandisingLevel_3 = GetLevelN(s, hierarchyLookup, 3)
                    });
                }

                new Blue.Solr.WebClient().Update(solrNonStocks);
            }
        }

        private static string GetFullTypeString(string type)
        {
            var typeStringRet = string.Empty;

            switch (type)
            {
                case NonStockTypes.Installation:
                    typeStringRet = "Installation";
                    break;
                case NonStockTypes.ReadyAssist:
                    typeStringRet = "Ready Assist";
                    break;
                case NonStockTypes.Assembly:
                    typeStringRet = "Assembly Cost";
                    break;
                case NonStockTypes.Generic:
                    typeStringRet = "Generic Service";
                    break;
                case NonStockTypes.Discount:
                    typeStringRet = "Discount";
                    break;
                case NonStockTypes.Annual:
                    typeStringRet = "Annual Service Contract";
                    break;
                default:
                    typeStringRet = string.Empty;
                    break;
            }

            return typeStringRet;
        }

        private static string GetLevelN(NonStock s, ILookup<int, NonStockHierarchy> hierarchyLookup, Int16 levelN)
        {
            var lookupVals = hierarchyLookup[s.Id].Where(l => l.Level == levelN).FirstOrDefault();

            if (lookupVals != null)
                return string.Format("{0}", lookupVals.LevelName);
            else
                return string.Empty;
        }

        private class SolrNonStockstRecord
        {
            public int Id { get; set; }
            public string Type { get { return SolrUtils.NonStocksType; } }
            public string NonStockType { get; set; }
            public string SKU { get; set; }
            public string VendorUPC { get; set; }
            public string Description1 { get; set; }
            public string Description2 { get; set; }
            public string Active { get; set; }
            public decimal? TaxRate { get; set; }
            public string Division { get; set; }
            public string Department { get; set; }
            public string Class { get; set; }
            public string MerchandisingLevel_1 { get; set; }
            public string MerchandisingLevel_2 { get; set; }
            public string MerchandisingLevel_3 { get; set; }
        }
    }
}
