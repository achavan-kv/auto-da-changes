using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Warranty.Model
{
    public class PromotionAggregate
    {
        public int PromoId { get; set; }
        public short? BranchNumber { get; set; }
        public int? WarrantyId { get; set; }
        public decimal? PercentageDiscount { get; set; }
        public decimal? RetailPrice { get; set; }

        public List<Warranty.Tag> TagFilters { get; set; }

        public PromotionCalculatedPrice CalculatePromos(WarrantyLinkAggregate warrantyAggregate, WarrantyCalculatedPrice warrantyPrice)
        {
            if (warrantyPrice == null)
                return null;

            var price = RetailPrice.HasValue ? RetailPrice.Value : warrantyPrice.RetailPrice - (PercentageDiscount.Value * warrantyPrice.RetailPrice / 100);

            if (WarrantyId.HasValue && warrantyAggregate.Id == WarrantyId.Value)
            {
                return new PromotionCalculatedPrice
                {
                    LevelMatch = Warranty.Tag.MaxLevel,
                    Price = price,
                    Promotion = this
                };
            }
            else if (TagFilters != null && TagFilters.Count > 0 &&
                    (from pt in TagFilters
                     join wt in warrantyAggregate.WarrantyTags on pt.TagId equals wt.TagId into j
                     from wtj in j.DefaultIfEmpty()
                     where wtj != null
                     select wtj).Count() == TagFilters.Count)
            {
                return new PromotionCalculatedPrice
                {
                    LevelMatch = TagFilters.Max(t => t.LevelId),
                    Price = price,
                    Promotion = this
                };
            }
            return null;
        }
    }



}
