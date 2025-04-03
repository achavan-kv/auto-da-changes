using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Blue.Cosacs.Warranty.Promotions;
using Blue.Events;
using Blue.Cosacs.Warranty.Model;
using Blue.Data;

namespace Blue.Cosacs.Warranty.Repositories
{
    public class WarrantyPromotionRepository : IWarrantyPromotionRepository
    {
        private readonly IEventStore audit;
        private const string AuditEventCategory = "WarrantyPromotions";
        private const string CreatePromotion = "CreatePromotion";
        private const string DeletePromotion = "DeletePromotion";
        private readonly IClock clock;

        public WarrantyPromotionRepository(IEventStore audit, IClock clock)
        {
            this.audit = audit;
            this.clock = clock;
        }

        public WarrantyPromotionSettings Save(WarrantyPromotionSettings promotion)
        {
            using (var scope = Context.Write())
            {
                var repetedPromotion = scope.Context.WarrantyPromotion
                    .Count(p => p.WarrantyId == promotion.WarrantyId
                                && p.BranchNumber == promotion.BranchNumberNumeric
                                && p.BranchType == promotion.BranchType
                                && !(promotion.EndDate < p.StartDate || promotion.StartDate > p.EndDate)
                            );

                if (repetedPromotion != 0)
                {
                    throw new WarrantyPromotionValidationException("The time span of this promotion overlaps with an existing one.");
                }

                var newPromotion = scope.Context.WarrantyPromotion.Add(new WarrantyPromotion
                {
                    BranchType = promotion.BranchType,
                    BranchNumber = GetShort(promotion.BranchNumber),
                    WarrantyId = promotion.WarrantyId,
                    StartDate = promotion.StartDate,
                    EndDate = promotion.EndDate,
                    PercentageDiscount = promotion.PercentageDiscount,
                    RetailPrice = promotion.RetailPrice
                });

                scope.Context.SaveChanges();

                promotion.Id = newPromotion.Id;

                scope.Context.SaveChanges();
                scope.Complete();

                audit.LogAsync(new
                {
                    WarrantyPromotion = promotion
                }, CreatePromotion, AuditEventCategory);
            }

            return promotion;
        }

        public void Delete(int promotionId)
        {
            using (var scope = Context.Write())
            {
                var promotion = scope.Context.WarrantyPromotion.Find(promotionId);
                if (promotion != null)
                {
                    scope.Context.WarrantyPromotion.Remove(promotion);
                }

                audit.LogAsync(new
                {
                    WarrantyPromotion = promotion,
                }, DeletePromotion, AuditEventCategory);

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public IPagedSearchResults<WarrantyPromotionSettings> GetPromotions(Filter filterValues)
        {
            filterValues = filterValues ?? new Filter();
            IPagedSearchResults<WarrantyPromotionSettings> returnValue;

            using (var scope = Context.Read())
            {
                var query = scope.Context.ViewWarrantyPromotion
                    .OrderBy(p => p.StartDate)
                    .ThenBy(p => p.EndDate)
                    .Select(p => new WarrantyPromotionSettings
                    {
                        Id = p.Id,
                        BranchType = p.BranchType,
                        BranchNumber = p.BranchNumber,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        PercentageDiscount = p.PercentageDiscount,
                        RetailPrice = p.RetailPrice,
                        WarrantyId = p.WarrantyId,
                        WarrantyNumber = p.WarrantyNumber
                    });

                if (filterValues.Id.HasValue)
                {
                    query = query.Where(p => p.Id == filterValues.Id.Value);
                }

                if (!string.IsNullOrEmpty(filterValues.BranchType))
                {
                    query = query.Where(p => p.BranchType == filterValues.BranchType);
                }

                if (!string.IsNullOrEmpty(filterValues.BranchNumber))
                {
                    query = query.Where(p => p.BranchNumber == filterValues.BranchNumber);
                }

                if (filterValues.WarrantyId.HasValue)
                {
                    query = query.Where(p => p.WarrantyId == filterValues.WarrantyId.Value);
                }

                if (filterValues.ActiveFrom.HasValue)
                {
                    var x = filterValues.ActiveFrom.Value.Date;
                    query = query.Where(p => p.EndDate >= x);
                }

                if (filterValues.ActiveTo.HasValue)
                {
                    var y = filterValues.ActiveTo.Value.Date;
                    query = query.Where(p => p.StartDate <= y);
                }

                if (!string.IsNullOrWhiteSpace(filterValues.Warranty))
                {
                    query = query.Where(p => p.WarrantyNumber.StartsWith(filterValues.Warranty));
                }

                returnValue = filterValues.Page(query);

                returnValue.Page.ForEach(p =>
                {
                    p.BranchType = StoreType.GetNameForType(p.BranchType);
                });
            }

            return returnValue;
        }

        public IEnumerable<WarrantyPromotionSettings> GetPromotionsForWarranty(int warrantyId, DateTime? endDate,
            IEnumerable<int> warrantyPriceId = null, bool getCurrentPromotions = false)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.WarrantyPromotionsView
                    .Where(p => p.warrantyId == warrantyId);

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

                if (warrantyPriceId != null)
                {
                    query = query
                        .Where(p => p.WarrantyPriceId != null && warrantyPriceId.Contains(p.WarrantyPriceId));
                }

                var promos = query
                    .Select(p => p)
                    .ToList();

                return promos
                    .Select(p => new WarrantyPromotionSettings
                    {
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        BranchType = StoreType.GetNameForType(p.BranchType),
                        RetailPrice = p.RetailPrice,
                        PercentageDiscount = p.PercentageDiscount,
                        WarrantyPriceId = p.WarrantyPriceId,
                        BranchNumber = p.BranchNumber.HasValue ? p.BranchNumber.Value.ToString(CultureInfo.InvariantCulture) : (string)null,
                        WarrantyId = warrantyId,
                        BranchName = p.BranchName,
                        WarrantyNumber = p.WarrantyNumber
                    })
                    .ToList();
            }
        }

        private static short? GetShort(string stringValue)
        {
            short value;
            return short.TryParse(stringValue, out value) ? value : new short?();
        }

        public List<PromotionAggregate> GetPromotionAggregate(int branch, DateTime? date)
        {
            using (var scope = Context.Read())
            {
                var filterDate = date.HasValue ? date.Value.Date : clock.Now.Date;

                return (from wp in scope.Context.WarrantyPromotion
                        where filterDate >= wp.StartDate &&
                              filterDate <= wp.EndDate
                        select new PromotionAggregate
                        {
                            BranchNumber = wp.BranchNumber,
                            PercentageDiscount = wp.PercentageDiscount,
                            RetailPrice = wp.RetailPrice,
                            WarrantyId = wp.WarrantyId,
                            PromoId = wp.Id
                        }).ToList();
            }
        }

        public List<PromotionBasic> GetPromotions(IEnumerable<WarrantyLocation> warrantyLocation)
        {
            using (var scope = Context.Read())
            {
                return (from p in scope.Context.WarrantyPromotion
                        where p.StartDate >= clock.Now && p.EndDate < clock.Now
                        select new PromotionBasic
                        {
                            Branch = p.BranchNumber.Value,
                            PromoPrice = p.RetailPrice,
                            PromoPrecent = p.PercentageDiscount
                        }).ToList();
            }
        }
    }
}
