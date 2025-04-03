using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Sales.Models
{
    public class SalesItem
    {
        public PriceData PriceData { get; set; }
        public PriceData PromoData { get; set; }
        public string ProductItemNo { get; set; }
        public string ProductType { get; set; }
        public int ProductItemId { get; set; }
        public string StockBranchNameWarrantyLink { get; set; }
        public short StockBranchProduct { get; set; }
        public string StoreType { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string PosDescription { get; set; }
        public decimal? KitDiscount { get; set; }
        public bool IsKitParent { get; set; }
        public List<HierarchyTags> HierarchyTags { get; set; }
        public IDictionary<string, object[]> Tags { get; set; }

        public bool IsSet
        {
            get { return ProductType.ToLower() == "set"; }
        }
    }
}
