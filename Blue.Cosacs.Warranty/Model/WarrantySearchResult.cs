
using System.Collections.Generic;
using Blue.Cosacs.Stock;

namespace Blue.Cosacs.Warranty.Model
{
    public class WarrantySearchResult
    {
        public WarrantyLinkAggregate[] Items { get; set; }
        public decimal ProductPrice { get; set; }
        public IEnumerable<WarrantyProductLinkSearch> ItemsWithoutWarranties { get; set; }
    }

    public class WarrantyLinkAggregate : Warranty
    {
        public int LinkId { get; set; }
        public string LinkName { get; set; }
        public bool ProductMatch { get; set; }
        public int LevelMatch { get; set; }
        public string ProductItemNo { get; set; }
        public string ProductCategory  { get; set; }
        public string ProductDescription { get; set; }
        public string ProductRetailPrice { get; set; }

        public decimal ProductRetailPriceDecimal
        {
            get
            {
                decimal retPercentage = 0;
    decimal.TryParse(ProductRetailPrice, out retPercentage);

                return retPercentage;
            }
        }

    }

    public class WarrantyLinkResult
    {
        public WarrantyLinkAggregate warrantyLink { get; set; }
        public WarrantyCalculatedPrice price { get; set; }
        public decimal WarrantyProductPricePercentage { get; set; }
        public PromotionCalculatedPrice promotion { get; set; }
    }
  
    public class WarrantySearchByProductResult
    {
        public IEnumerable<WarrantyLinkResult> Items { get; set; }
        public Model.WarrantySearchByProduct ProductSearch { get; set; }
        public decimal ProductPrice { get; set; }
        public IEnumerable<WarrantyProductLinkSearch> ItemsWithoutWarranties { get; set; }
        public int CountWarranties { get; set; }
        public int CountNoWarranties { get; set; }
    }
}
    