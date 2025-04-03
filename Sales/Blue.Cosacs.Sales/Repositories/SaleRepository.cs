using System;
using System.Collections.Generic;
using System.Linq;
using Blue.Cosacs.Sales.Models;
using Blue.Events;
//using Blue.Glaucous.Client.Api;
using Blue.Networking;
using System.Net;

namespace Blue.Cosacs.Sales.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly IClock clock;
        private readonly IEventStore audit;
        private IHttpClientJson httpClientJson;

        public SaleRepository(IClock clock, IEventStore audit, IHttpClientJson httpClientJson)
        {
            this.clock = clock;
            this.audit = audit;
            this.httpClientJson = httpClientJson;
        }

        #region Web Methods

        public decimal GetDiscountLimit(int branchNumber)
        {
            var uriString = string.Format("/Courts.NET.WS/DBOInfo/Branch?id={0}", branchNumber);
            var branchDetails = httpClientJson.Do<byte[], List<BranchInfo>>(RequestJson<byte[]>.Create(uriString, WebRequestMethods.Http.Get)).Body.SingleOrDefault();

            using (var scope = Context.Read())
            {
                var dataList = (from d in scope.Context.DiscountLimit
                                where (d.BranchNumber == null || d.BranchNumber == branchNumber)
                                && (d.StoreType == null || branchDetails.StoreType.Equals(d.StoreType))
                                select d).ToList();

                var branchSpecificData = dataList.Where(d => d.BranchNumber == branchNumber);
                if (branchSpecificData.Any())
                {
                    var branchStoreTypeSpecificData = branchSpecificData.Where(d => branchDetails.StoreType.Equals(d.StoreType));
                    if (branchStoreTypeSpecificData.Any())
                    {
                        return branchStoreTypeSpecificData.Select(x => x.LimitPercentage).SingleOrDefault();
                    }
                    return branchSpecificData.Select(x => x.LimitPercentage).SingleOrDefault();
                }

                var storeTypeSpecificData = dataList.Where(d => branchDetails.StoreType.Equals(d.StoreType));
                if (storeTypeSpecificData.Any())
                {
                    return storeTypeSpecificData.Select(x => x.LimitPercentage).SingleOrDefault();
                }

                return dataList.Select(x => x.LimitPercentage).SingleOrDefault();
            }
        }

        #region Discount Limit Setup Screen

        public IEnumerable<DiscountLimit> GetDiscountLimitData(DiscountLimit discountLimitDetail)
        {
            using (var scope = Context.Read())
            {
                var dataList = (from d in scope.Context.DiscountLimit
                                where (discountLimitDetail.BranchNumber == null || d.BranchNumber == discountLimitDetail.BranchNumber)
                                && (discountLimitDetail.StoreType == null || discountLimitDetail.StoreType.Equals(d.StoreType))
                                select d)
                                    .OrderByDescending(x => x.CreatedOn)
                                    .Take(50)
                                    .ToList();

                return dataList;
            }
        }

        public CustomResponseMessage InsertDiscountLimit(DiscountLimit discountLimitDetail, int currentUserId)
        {
            var response = new CustomResponseMessage();
            try
            {
                using (var scope = Context.Write())
                {
                    var existingData = (from d in scope.Context.DiscountLimit
                                        where ((d.BranchNumber == null && discountLimitDetail.BranchNumber == null) || (d.BranchNumber == discountLimitDetail.BranchNumber))
                                        && ((d.StoreType == null && discountLimitDetail.StoreType == null) || (d.StoreType == discountLimitDetail.StoreType))
                                        select d).SingleOrDefault();

                    if (existingData == null)
                    {
                        var newDiscountLimit = new DiscountLimit
                        {
                            BranchNumber = discountLimitDetail.BranchNumber,
                            StoreType = discountLimitDetail.StoreType,
                            LimitPercentage = discountLimitDetail.LimitPercentage,
                            CreatedOn = clock.Now,
                            CreatedBy = currentUserId
                        };

                        scope.Context.DiscountLimit.Add(newDiscountLimit);
                        scope.Context.SaveChanges();
                        scope.Complete();

                        audit.LogAsync(
                          new
                          {
                              newDiscountLimit.Id,
                              newDiscountLimit.BranchNumber,
                              newDiscountLimit.StoreType,
                              newDiscountLimit.LimitPercentage,
                          },
                         EventType.InsertDiscountLimit,
                        EventCategory.DiscountLimitSetup);
                    }
                    else
                    {
                        response.Valid = false;
                        response.CustomError = "Entry for same Store Type and Branch already exists.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Valid = false;
                response.Errors = new[] { ex.Message };
            }
            return response;
        }

        public CustomResponseMessage UpdateDiscountLimit(DiscountLimit discountLimitDetail, int currentUserId)
        {
            try
            {
                using (var scope = Context.Write())
                {
                    var existingData = (from d in scope.Context.DiscountLimit
                                        where ((d.BranchNumber == null && discountLimitDetail.BranchNumber == null) || (d.BranchNumber == discountLimitDetail.BranchNumber))
                                        && ((d.StoreType == null && discountLimitDetail.StoreType == null) || (d.StoreType == discountLimitDetail.StoreType))
                                        select d).SingleOrDefault();

                    if (existingData != null)
                    {
                        existingData.LimitPercentage = discountLimitDetail.LimitPercentage;
                        existingData.CreatedOn = clock.Now;
                        existingData.CreatedBy = currentUserId;

                        scope.Context.SaveChanges();
                        scope.Complete();

                        audit.LogAsync(
                            new
                            {
                                existingData.Id,
                                existingData.BranchNumber,
                                existingData.StoreType,
                                existingData.LimitPercentage,
                            },
                            EventType.UpdateDiscountLimit,
                            EventCategory.DiscountLimitSetup);
                    }
                }
            }
            catch (Exception ex)
            {
                new CustomResponseMessage
                {
                    Valid = false,
                    Errors = new[] { ex.Message }
                };
            }
            return new CustomResponseMessage();
        }

        public void DeleteDiscountLimit(DiscountLimit discountLimitDetail)
        {
            using (var scope = Context.Write())
            {
                var existingData = (from d in scope.Context.DiscountLimit
                                    where ((d.BranchNumber == null && discountLimitDetail.BranchNumber == null) || (d.BranchNumber == discountLimitDetail.BranchNumber))
                                    && ((d.StoreType == null && discountLimitDetail.StoreType == null) || (d.StoreType == discountLimitDetail.StoreType))
                                    select d).SingleOrDefault();

                if (existingData != null)
                {
                    scope.Context.DiscountLimit.Remove(existingData);
                    scope.Context.SaveChanges();
                    scope.Complete();

                    audit.LogAsync(
                      new
                      {
                          existingData.Id,
                          existingData.StoreType,
                          existingData.BranchNumber,
                          existingData.LimitPercentage
                      },
                      EventType.DeleteDiscountLimit,
                      EventCategory.DiscountLimitSetup);
                }
            }
        }

        #endregion

        #endregion
    }
}