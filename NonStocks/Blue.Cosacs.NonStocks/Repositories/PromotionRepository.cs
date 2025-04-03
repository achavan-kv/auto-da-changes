using Blue.Cosacs.NonStocks.Models;
using Blue.Cosacs.NonStocks.Promotions;
using Blue.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Blue.Cosacs.NonStocks
{
    public class PromotionsRepository : IPromotionsRepository
    {
        private readonly IClock clock;

        public PromotionsRepository(IClock clock)
        {
            this.clock = clock;
        }

        public IPagedSearchResults<NonStockPromotionModel> GetPromotions(Filter filterValues)
        {
            filterValues = filterValues ?? new Filter();
            var returnValue = (IPagedSearchResults<NonStockPromotionModel>)null;

            using (var scope = Context.Read())
            {
                var query = scope.Context.ViewNonStockPromotion
                    .OrderBy(p => p.StartDate)
                    .ThenBy(p => p.EndDate)
                    .Select(p => new NonStockPromotionModel
                    {
                        Id = p.Id,
                        Fascia = p.Fascia,
                        BranchNumber = p.BranchNumber,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        PercentageDiscount = p.PercentageDiscount,
                        RetailPrice = p.RetailPrice,
                        NonStockId = p.NonStockId,
                        NonStockNumber = p.NonStockNumber,
                        ShortDescription = p.ShortDescription,
                        LongDescription = p.LongDescription
                    });

                if (filterValues.Id.HasValue)
                {
                    query = query.Where(p => p.Id == filterValues.Id.Value);
                }

                if (!string.IsNullOrEmpty(filterValues.Fascia))
                {
                    query = query.Where(p => p.Fascia == filterValues.Fascia);
                }

                if (!string.IsNullOrEmpty(filterValues.BranchNumber))
                {
                    query = query.Where(p => p.BranchNumber == filterValues.BranchNumber);
                }

                if (!string.IsNullOrEmpty(filterValues.SKU))
                {
                    query = query.Where(p => p.NonStockNumber == filterValues.SKU);
                }

                if (filterValues.NonStockId.HasValue)
                {
                    query = query.Where(p => p.NonStockId == filterValues.NonStockId.Value);
                }

                if (filterValues.ActiveFrom.HasValue && filterValues.ActiveTo.HasValue)
                {
                    var x = filterValues.ActiveFrom.Value.Date;
                    var y = filterValues.ActiveTo.Value.Date;
                    query = query.Where(p =>
                        (p.StartDate <= x && p.EndDate >= x) ||
                        (p.StartDate <= y && p.EndDate >= y));
                }
                else
                {
                    if (filterValues.ActiveFrom.HasValue)
                    {
                        var x = filterValues.ActiveFrom.Value.Date;
                        query = query.Where(p => p.StartDate <= x && p.EndDate >= x);
                    }

                    if (filterValues.ActiveTo.HasValue)
                    {
                        var y = filterValues.ActiveTo.Value.Date;
                        query = query.Where(p => p.StartDate <= y && p.EndDate >= y);
                    }
                }

                if (!string.IsNullOrWhiteSpace(filterValues.NonStock))
                {
                    query = query.Where(p => p.NonStockNumber.StartsWith(filterValues.NonStock));
                }

                returnValue = filterValues.Page(query);

                returnValue.Page.ForEach(p =>
                {
                    p.Fascia = StoreType.GetNameForType(p.Fascia);
                });
            }

            return returnValue;
        }

        public IEnumerable<NonStockPromotionModel> GetPromotionsForNonStock(int nonStockId, DateTime? endDate,
           IEnumerable<int> nonStockPriceId = null, bool getCurrentPromotions = false)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.NonStockPromotionsView
                    .Where(p => p.NonStockId == nonStockId);

                if (getCurrentPromotions)
                {
                    query = query.Where(p => p.EndDate >= clock.Now.Date);
                }

                if (endDate.HasValue)
                {
                    var value = endDate.Value.Date;// linq do not support endDate.Value.Date. So i put that value in a variable as work around
                    query = query
                        .Where(p => p.EndDate >= value);
                }

                if (nonStockPriceId != null)
                {
                    query = query
                        .Where(p => p.NonStockPriceId != null && nonStockPriceId.Contains(p.NonStockPriceId));
                }

                var promos = query.ToList();

                return promos
                    .Select(p => new NonStockPromotionModel
                    {
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        Fascia = StoreType.GetNameForType(p.Fascia),
                        RetailPrice = p.RetailPrice,
                        PercentageDiscount = p.PercentageDiscount,
                        NonStockPriceId = p.NonStockPriceId,
                        BranchNumber = p.BranchNumber.HasValue ? p.BranchNumber.Value.ToString(CultureInfo.InvariantCulture) : (string)null,
                        NonStockId = nonStockId,
                        BranchName = p.BranchName,
                        NonStockNumber = p.NonStockNumber
                    })
                    .ToList();
            }
        }

        private static short? GetShort(string stringValue)
        {
            short value;
            return short.TryParse(stringValue, out value) ? value : new short?();
        }

        public NonStockPromotionModel SavePromotion(NonStockPromotionModel promotion)
        {
            using (var scope = Context.Write())
            {
                var repetedPromotion = scope.Context.NonStockPromotion
                    .Count(p => p.NonStockId == promotion.NonStockId
                                && p.BranchNumber == promotion.BranchNumberNumeric
                                && p.Fascia == promotion.Fascia
                                && !(promotion.EndDate < p.StartDate || promotion.StartDate > p.EndDate)
                            );

                if (repetedPromotion != 0)
                {
                    throw new OperationCanceledException("The time span of this promotion overlaps with an existing one.");
                }

                var nonStock = scope.Context.NonStock.Where(n => n.Id == promotion.NonStockId).FirstOrDefault();

                var newPromotion = scope.Context.NonStockPromotion.Add(new NonStockPromotion
                {
                    Fascia = promotion.Fascia,
                    BranchNumber = GetShort(promotion.BranchNumber),
                    NonStockId = promotion.NonStockId,
                    StartDate = promotion.StartDate,
                    EndDate = promotion.EndDate,
                    PercentageDiscount = promotion.PercentageDiscount,
                    RetailPrice = promotion.RetailPrice
                });

                scope.Context.SaveChanges();
                scope.Complete();

                promotion.Id = newPromotion.Id;
                promotion.NonStockNumber = nonStock.SKU;
                promotion.ShortDescription = nonStock.ShortDescription;
                promotion.LongDescription = nonStock.LongDescription;
               
            }

            return promotion;
        }

        public void DeletePromotion(int promotionId)
        {
            using (var scope = Context.Write())
            {
                var promotion = scope.Context.NonStockPromotion.Find(promotionId);

                if (promotion != null)
                {
                    scope.Context.NonStockPromotion.Remove(promotion);
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public NonStockPromotion GetPromotion (int promotionId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.NonStockPromotion.Find(promotionId);
            }
        }

    }
}
