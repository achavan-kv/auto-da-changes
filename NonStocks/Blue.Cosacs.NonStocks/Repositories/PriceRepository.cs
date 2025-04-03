using Blue.Cosacs.NonStocks.Models;
using Blue.Cosacs.NonStocks.Models.WinCosacs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.NonStocks
{
    public class PriceRepository : IPriceRepository
    {
        private IClock clock;

        public PriceRepository()
        {
            this.clock = new DateTimeClock();
        }

        public List<NonStockPriceModel> GetPrices(int id)
        {
            using (var scope = Context.Read())
            {
                var prices = (from p in scope.Context.PricesView
                              where p.NonStockId == id
                              select new NonStockPriceModel
                              {
                                  Id = p.Id,
                                  NonStockId = p.NonStockId,
                                  NonStockType = p.Type,
                                  Fascia = p.Fascia,
                                  BranchNumber = p.BranchNumber,
                                  CostPrice = p.CostPrice,
                                  RetailPrice = p.RetailPrice,
                                  TaxInclusivePrice = p.TaxInclusivePrice,
                                  DiscountValue = p.DiscountValue,
                                  EffectiveDate = p.EffectiveDate,
                                  EndDate = p.EndDate,
                                  HasPromotion = p.HasPromotion >= 1
                              })
                              .ToList();

                return prices;
            }
        }

        /// <summary>
        /// This method returns two types of prices: the current generic price
        /// (where both branch number and Fascia are null). And specific prices
        /// (with null fascia and non null branch numbers. This is done because
        /// we need to return only current active prices.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<NonStockPriceModel> GetActiveExpandedPrices(int id, IList<BranchInfo> allBranches)
        {
            var dateForExport = clock.Now.Hour < 12 ? clock.Now.Date : clock.Now.AddDays(1).Date; //If after 12PM then include to export those with Effect Date of the following day

            var allActiveExpandedPrices = new List<NonStockPriceModel>();
            var tmpConcretePrices = new List<NonStockPriceModel>();

            var allPastPrices = GetPrices(id)
                .Where(e => e.EffectiveDate <= dateForExport)
                .ToList();

            var currentActiveGenericPrice = GetCurrentActiveGenericPrice(allPastPrices);
            var currentActiveFasciaPrices = GetAllFasciaPrices(allPastPrices);
            var currentActiveConcretePrices = GetCurrentActiveConcretePrices(allPastPrices, currentActiveGenericPrice);
            var expandedFasciaPrices = ExpandAllFasciaPrices(currentActiveFasciaPrices, allBranches);

            if (currentActiveConcretePrices != null && currentActiveConcretePrices.Count > 0)
            {
                tmpConcretePrices.AddRange(currentActiveConcretePrices);
            }

            if (expandedFasciaPrices != null && expandedFasciaPrices.Count > 0)
            {
                tmpConcretePrices.AddRange(expandedFasciaPrices);
            }

            foreach (var branch in GetBranchesOnPrices(tmpConcretePrices))
            {
                var tmpBranchPrice = tmpConcretePrices
                    .Where(e => e.BranchNumber == branch)
                    .OrderByDescending(e => e.EffectiveDate)
                    .FirstOrDefault();

                if (tmpBranchPrice != null)
                {
                    allActiveExpandedPrices.Add(tmpBranchPrice);
                }
            }

            if (currentActiveGenericPrice != null)
            {
                allActiveExpandedPrices.Add(currentActiveGenericPrice);
            }

            return allActiveExpandedPrices;
        }

        private NonStockPriceModel GetCurrentActiveGenericPrice(List<NonStockPriceModel> allPastPrices)
        {
            // Get current active generic price
            var currentGenericPrice = allPastPrices
                .Where(e => e.Fascia == null && e.BranchNumber == null)
                .OrderByDescending(e => e.EffectiveDate)
                .FirstOrDefault();

            return currentGenericPrice;
        }

        private List<NonStockPriceModel> GetCurrentActiveConcretePrices(
            List<NonStockPriceModel> allPastPrices, NonStockPriceModel currentActiveGenericPrice)
        {
            var allCurrentConcretePrices = new List<NonStockPriceModel>();

            allCurrentConcretePrices = allPastPrices.Where(e => e.BranchNumber != null)
                .ToList();

            // Add current generic price, and filter all past prices that it overrides
            if (currentActiveGenericPrice != null)
            {
                allCurrentConcretePrices = allCurrentConcretePrices
                    .Where(e => e.EffectiveDate >= currentActiveGenericPrice.EffectiveDate)
                    .ToList();
            }

            return allCurrentConcretePrices;
        }

        private List<NonStockPriceModel> GetAllFasciaPrices(List<NonStockPriceModel> allPastPrices)
        {
            var fasciaPrices = new List<NonStockPriceModel>();

            var fasciaCandidates = allPastPrices
                .Where(e => e.Fascia != null && e.BranchNumber == null);

            var fasciaCPrice = fasciaCandidates.Where(e => e.Fascia != "C")
                .OrderByDescending(e => e.EffectiveDate)
                .FirstOrDefault();

            var fasciaNPrice = fasciaCandidates.Where(e => e.Fascia != "N")
                .OrderByDescending(e => e.EffectiveDate)
                .FirstOrDefault();

            if (fasciaCPrice != null)
            {
                fasciaPrices.Add(fasciaCPrice);
            }

            if (fasciaNPrice != null)
            {
                fasciaPrices.Add(fasciaNPrice);
            }

            return fasciaPrices;
        }

        private List<NonStockPriceModel> ExpandAllFasciaPrices(
            List<NonStockPriceModel> fasciaPriceDetails, IList<BranchInfo> allBranches)
        {
            var fasciaExpandedPrices = new List<NonStockPriceModel>();

            var fasciaCPrice = fasciaPriceDetails.Where(e => e.Fascia != "C")
                .OrderByDescending(e => e.EffectiveDate)
                .FirstOrDefault();

            var fasciaNPrice = fasciaPriceDetails.Where(e => e.Fascia != "N")
                .OrderByDescending(e => e.EffectiveDate)
                .FirstOrDefault();

            var allBranchesC = allBranches.Where(e => e.StoreType == "C").ToList();
            var allBranchesN = allBranches.Where(e => e.StoreType == "N").ToList();

            fasciaExpandedPrices.AddRange(CreateFasciaPriceList(fasciaCPrice, allBranchesC));
            fasciaExpandedPrices.AddRange(CreateFasciaPriceList(fasciaNPrice, allBranchesN));

            return fasciaExpandedPrices;
        }

        private List<NonStockPriceModel> CreateFasciaPriceList(NonStockPriceModel fasciaPrice,
            List<BranchInfo> allBranches)
        {
            var activePrices = new List<NonStockPriceModel>();

            if (fasciaPrice != null)
            {
                foreach (var branch in allBranches)
                {
                    activePrices.Add(new NonStockPriceModel()
                    {
                        Id = fasciaPrice.Id,
                        NonStockId = fasciaPrice.NonStockId,
                        NonStockType = fasciaPrice.NonStockType,
                        Fascia = null,
                        BranchNumber = branch.BranchNumber,
                        WarehouseNumber = fasciaPrice.WarehouseNumber,
                        CostPrice = fasciaPrice.CostPrice,
                        RetailPrice = fasciaPrice.RetailPrice,
                        TaxInclusivePrice = fasciaPrice.TaxInclusivePrice,
                        DiscountValue = fasciaPrice.DiscountValue,
                        EffectiveDate = fasciaPrice.EffectiveDate,
                        EndDate = fasciaPrice.EndDate,
                        HasPromotion = fasciaPrice.HasPromotion,
                    });
                }
            }

            return activePrices;
        }

        private List<int?> GetBranchesOnPrices(List<NonStockPriceModel> prices)
        {
            var priceBranches = prices
                .Where(e => e.BranchNumber != null)
                .Select(e => e.BranchNumber)
                .Distinct()
                .ToList();

            return priceBranches;
        }

        private short? GetBranchNumber(NonStockPriceModel nonStockPrice)
        {
            return nonStockPrice != null ? (short?)nonStockPrice.BranchNumber : null;
        }

        private bool CheckNonStockPriceNotDuplicated(NonStockPriceModel nonStockPrice)
        {
            // check if Non Stock Price is unique
            using (var scope = Context.Read())
            {
                var priceCheck = from p in scope.Context.NonStockPrice
                                 where p.NonStockId == nonStockPrice.NonStockId
                                 select p;

                if (string.IsNullOrWhiteSpace(nonStockPrice.Fascia))
                    priceCheck = priceCheck.Where(e => e.Fascia == null);
                else
                    priceCheck = priceCheck.Where(e => e.Fascia == nonStockPrice.Fascia);

                var branchNumber = GetBranchNumber(nonStockPrice);
                if (branchNumber.HasValue)
                    priceCheck = priceCheck.Where(e => e.BranchNumber == branchNumber.Value);
                else
                    priceCheck = priceCheck.Where(e => e.BranchNumber == null);

                priceCheck = priceCheck.Where(e => e.EffectiveDate == nonStockPrice.EffectiveDate.Date);

                return !priceCheck.Any();
            }
        }

        public NonStockPriceModel SavePrice(NonStockPriceModel nonStockPrice)
        {
            using (var scope = Context.Write())
            {
                if (nonStockPrice != null && nonStockPrice.Id == 0)
                {
                    if (CheckNonStockPriceNotDuplicated(nonStockPrice))
                    {
                        var newPrice = scope.Context.NonStockPrice.Add(new NonStockPrice
                        {
                            NonStockId = nonStockPrice.NonStockId,
                            Fascia = nonStockPrice.Fascia,
                            BranchNumber = nonStockPrice.BranchNumber.HasValue ? short.Parse(nonStockPrice.BranchNumber.Value.ToString()) : (short?)null,
                            CostPrice = nonStockPrice.CostPrice,
                            RetailPrice = nonStockPrice.RetailPrice,
                            TaxInclusivePrice = nonStockPrice.TaxInclusivePrice,
                            DiscountValue = nonStockPrice.DiscountValue,
                            EffectiveDate = nonStockPrice.EffectiveDate,
                            EndDate = nonStockPrice.EndDate
                        });

                        scope.Context.SaveChanges();
                        scope.Complete();

                        nonStockPrice.Id = newPrice.Id;

                    }
                    else throw new OperationCanceledException("Cannot create duplicate price.");
                }
                else
                {
                    NonStockPrice price = (from p in scope.Context.NonStockPrice
                                           where p.Id == nonStockPrice.Id
                                           select p).FirstOrDefault();

                    price.CostPrice = nonStockPrice.CostPrice;
                    price.RetailPrice = nonStockPrice.RetailPrice;
                    price.TaxInclusivePrice = nonStockPrice.TaxInclusivePrice;
                    price.DiscountValue = nonStockPrice.DiscountValue;
                    price.EffectiveDate = nonStockPrice.EffectiveDate;
                    price.EndDate = nonStockPrice.EndDate;

                    scope.Context.SaveChanges();
                    scope.Complete();
                }
            }

            return nonStockPrice;
        }

        public NonStockPriceModel DeletePrice(int id)
        {
            using (var scope = Context.Write())
            {
                var price = scope.Context.NonStockPrice.Find(id);
                if (price != null)
                {
                    scope.Context.NonStockPrice.Remove(price);

                    var locationPrice = new NonStockPriceModel
                    {
                        Id = price.Id,
                        NonStockId = price.NonStockId,
                        Fascia = price.Fascia,
                        BranchNumber = price.BranchNumber,
                        CostPrice = price.CostPrice,
                        RetailPrice = price.RetailPrice,
                        TaxInclusivePrice = price.TaxInclusivePrice,
                        EffectiveDate = price.EffectiveDate
                    };

                    scope.Context.SaveChanges();
                    scope.Complete();

                    return locationPrice;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("id", string.Format("Price {0} not found!", id));
                }
            }
        }

    }
}
