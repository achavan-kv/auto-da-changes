using System;
namespace Blue.Cosacs.Warranty.Repositories
{
    public interface IWarrantyPromotionRepository
    {
        void Delete(int promotionId);
        System.Collections.Generic.List<Blue.Cosacs.Warranty.Model.PromotionAggregate> GetPromotionAggregate(int branch, DateTime? date);
        Blue.Data.IPagedSearchResults<Blue.Cosacs.Warranty.Model.WarrantyPromotionSettings> GetPromotions(Blue.Cosacs.Warranty.Promotions.Filter filterValues);
        System.Collections.Generic.List<Blue.Cosacs.Warranty.Model.PromotionBasic> GetPromotions(System.Collections.Generic.IEnumerable<Blue.Cosacs.Warranty.Model.WarrantyLocation> warrantyLocation);
        System.Collections.Generic.IEnumerable<Blue.Cosacs.Warranty.Model.WarrantyPromotionSettings> GetPromotionsForWarranty(int warrantyId, DateTime? endDate, System.Collections.Generic.IEnumerable<int> warrantyPriceId = null, bool getCurrentPromotions = false);
        Blue.Cosacs.Warranty.Model.WarrantyPromotionSettings Save(Blue.Cosacs.Warranty.Model.WarrantyPromotionSettings promotion);
    }
}
