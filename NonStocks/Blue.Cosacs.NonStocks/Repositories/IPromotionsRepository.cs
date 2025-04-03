using Blue.Cosacs.NonStocks.Models;
using Blue.Cosacs.NonStocks.Promotions;
using Blue.Data;
using System;
using System.Collections.Generic;

namespace Blue.Cosacs.NonStocks
{
    public interface IPromotionsRepository
    {
        void DeletePromotion(int promotionId);
        IPagedSearchResults<NonStockPromotionModel> GetPromotions(Filter filterValues);
        IEnumerable<NonStockPromotionModel> GetPromotionsForNonStock(int nonStockId, DateTime? endDate,
            IEnumerable<int> nonStockPriceId, bool getCurrentPromotions);
        NonStockPromotionModel SavePromotion(NonStockPromotionModel promotion);
    }
}
