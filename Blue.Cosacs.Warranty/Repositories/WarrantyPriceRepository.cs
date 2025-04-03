using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Blue.Cosacs.Warranty.Model;
using Blue.Cosacs.Warranty.Promotions;
using Blue.Events;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using Blue.Caching;
using Merch = Blue.Cosacs.Merchandising;

namespace Blue.Cosacs.Warranty.Repositories
{
    public class WarrantyPriceRepository : IWarrantyPriceRepository
    {
        private const string priceCalcViewTable = "PriceCalcViewTable";
        private static object sync = new object();
        private IEventStore audit;
        private IClock clock;
        private ICacheClient cache;
        private readonly Blue.Config.Repositories.Settings cosacsSettings;
        private const string AuditEventCategory = "WarrantyPricing";
        private readonly Merch.Settings merchandisingSettings;
        private readonly Merch.Repositories.TaxRepository merchandisingTaxRepo;

        public WarrantyPriceRepository(IEventStore audit, IClock clock, Blue.Config.Repositories.Settings cosacsSettings, ICacheClient cache, Merch.Settings merchandisingSettings,
            Merch.Repositories.TaxRepository merchandisingTaxRepo)
        {
            this.audit = audit;
            this.clock = clock;
            this.cosacsSettings = cosacsSettings;
            this.cache = cache;
            this.merchandisingSettings = merchandisingSettings;
            this.merchandisingTaxRepo = merchandisingTaxRepo;
        }

        public WarrantyLocationPrice Save(WarrantyLocationPrice warrantyPrice)
        {
            if (warrantyPrice != null && PriceDateIsInThePast(warrantyPrice.EffectiveDate))
            {
                throw new ArgumentOutOfRangeException("EffectiveDate", "Cannot insert or update prices for a past date!");
            }

            using (var scope = Context.Write())
            {
                if (warrantyPrice.Id == 0)
                {
                    if (CheckWarrantyPriceNotDuplicated(scope, warrantyPrice))
                    {
                        var newPrice = scope.Context.WarrantyPrice.Add(new WarrantyPrice
                        {
                            WarrantyId = warrantyPrice.WarrantyId,
                            BranchType = warrantyPrice.BranchType,
                            BranchNumber = GetBranchNumber(warrantyPrice),
                            CostPrice = warrantyPrice.CostPrice,
                            RetailPrice = warrantyPrice.RetailPrice,
                            EffectiveDate = warrantyPrice.EffectiveDate
                        });

                        scope.Context.SaveChanges();
                        scope.Complete();

                        warrantyPrice.Id = newPrice.Id;

                        audit.LogAsync(new { WarrantyPrice = warrantyPrice }, EventType.CreatePrice, AuditEventCategory);
                    }
                    else
                        throw new OperationCanceledException("Cannot create duplicate price.");
                }
                else
                {
                    var dbPrice = scope.Context.WarrantyPrice.Find(warrantyPrice.Id);
                    if (dbPrice.BulkEditId.HasValue && dbPrice.BulkEditId.Value > 0)
                    {
                        throw new InvalidOperationException("Error while editing Warranty Prices!\r\nDetails of Bulk Edit price rows, cannot be edited individually!");
                    }

                    dbPrice.CostPrice = warrantyPrice.CostPrice;
                    dbPrice.RetailPrice = warrantyPrice.RetailPrice;
                    dbPrice.EffectiveDate = warrantyPrice.EffectiveDate;

                    scope.Context.SaveChanges();
                    scope.Complete();

                    audit.LogAsync(new { WarrantyPrice = warrantyPrice }, EventType.EditPrice, AuditEventCategory);
                }
            }

            warrantyPrice.BranchType = StoreType.GetNameForType(warrantyPrice.BranchType);
            DeletePriceCalcViewCache();
            return warrantyPrice;
        }

        private static bool CheckWarrantyPriceNotDuplicated(Transactions.WriteScope<Context> scope, WarrantyLocationPrice warrantyPrice)
        {
            // check if WarrantyPrice is unique
            var priceCheck = from p in scope.Context.WarrantyPrice
                             where p.WarrantyId == warrantyPrice.WarrantyId
                             select p;

            if (string.IsNullOrWhiteSpace(warrantyPrice.BranchType))
                priceCheck = priceCheck.Where(e => e.BranchType == null);
            else
                priceCheck = priceCheck.Where(e => e.BranchType == warrantyPrice.BranchType);

            var branchNumber = GetBranchNumber(warrantyPrice);
            if (branchNumber.HasValue)
                priceCheck = priceCheck.Where(e => e.BranchNumber == branchNumber.Value);
            else
                priceCheck = priceCheck.Where(e => e.BranchNumber == null);

            priceCheck = priceCheck.Where(e => e.EffectiveDate == warrantyPrice.EffectiveDate.Date);

            return !priceCheck.Any();
        }

        public IEnumerable<WarrantyLocationPrice> GetWarrantyPrices(int warrantyId)
        {
            using (var scope = Context.Read())
            {
                var promoRep = StructureMap.ObjectFactory.GetInstance<WarrantyPromotionRepository>();
                var dbPrices = (from wp in scope.Context.WarrantyPrice
                                where wp.WarrantyId == warrantyId
                                && wp.CostPrice.HasValue && wp.RetailPrice.HasValue // comment out bulk edit
                                select new
                                {
                                    wp.Id,
                                    wp.WarrantyId,
                                    wp.BranchNumber,
                                    wp.BranchType,
                                    wp.CostPrice,
                                    wp.RetailPrice,
                                    wp.EffectiveDate
                                })
                                .ToList();

                var promos = promoRep.GetPromotionsForWarranty(warrantyId, null, dbPrices.Select(p => p.Id), true);

                Func<WarrantyLocationPrice, WarrantyLocationPrice> getValue = price =>
                {
                    price.PromotionCount = promos
                        .Count(p => p.WarrantyPriceId.Value == price.Id);

                    return price;
                };

                return dbPrices
                    .Select(p => new WarrantyLocationPrice
                    {
                        Id = p.Id,
                        WarrantyId = p.WarrantyId,
                        BranchNumber = GetBranchNumberString(p.BranchNumber),
                        BranchType = StoreType.GetNameForType(p.BranchType),
                        CostPrice = p.CostPrice,
                        RetailPrice = p.RetailPrice,
                        EffectiveDate = p.EffectiveDate,
                    })
                    .Select(getValue)
                    .OrderByDescending(e => e.EffectiveDate)
                    .ToList();
            }
        }

        private List<PriceCalcView> GetPriceCalcViewCache()
        {
            var table = cache.Get<List<PriceCalcView>>(priceCalcViewTable);
            if (table == null)
            {
                lock (sync)
                {
                    using (var scope = Context.Read())
                    {
                        table = scope.Context.PriceCalcView.ToList();
                        cache.Add(priceCalcViewTable, table);
                    }
                }
            }
            return table;
        }

        public void DeletePriceCalcViewCache()
        {
            lock (sync)
            {
                cache.Delete(priceCalcViewTable);
            }
        }

        public IEnumerable<WarrantyCalculatedPrice> GetWarrantyPrices(IEnumerable<int> warrantyIds, short branch, DateTime? date)
        {
            using (var scope = Context.Read())
            {
                var branchStoreType = scope.Context.BranchLookup.Find(branch).StoreType;
                var simulatorDate = date.HasValue ? date.Value : clock.Now;
                simulatorDate = simulatorDate.Date; // the warranty simulator doesn't use time, only the date
                var calculatedPrices = (from p in GetPriceCalcViewCache()
                                        where p.EffectiveDate <= simulatorDate && // exclude all prices after simulation date
                                        (p.BranchNumber == null || p.BranchNumber == branch) &&
                                        (p.BranchType == null || p.BranchType == branchStoreType) &&
                                        p.WarrantyId.HasValue && warrantyIds.Contains(p.WarrantyId.Value)
                                        select p).ToList();

                var filteredPricesByDateAndWarrantyId =
                    GetSimulatorCurrentPricesPerDateAndWarrantyId(warrantyIds.ToList(), calculatedPrices, simulatorDate);

                var chosenPrices = WarrantySpecificPriceFiltering(filteredPricesByDateAndWarrantyId);

                var TaxRate = GetCountryTaxRate();

                var prices = from p in chosenPrices
                             select new WarrantyCalculatedPrice
                             {
                                 Id = p.Id.HasValue ? p.Id.Value : 0,
                                 WarrantyId = p.WarrantyId.HasValue ? p.WarrantyId.Value : 0,
                                 BranchNumber = p.BranchNumber,
                                 BranchType = p.BranchType,
                                 CostPrice = p.CostPrice,
                                 TaxInclusivePriceChange = p.TaxInclusivePriceChange,
                                 RetailPrice = p.RetailPrice,
                                 EffectiveDate = p.EffectiveDate.HasValue ? p.EffectiveDate.Value : DateTime.MaxValue,
                                 TaxAmount = p.RetailPrice.HasValue ?
                                                p.RetailPrice.Value * (p.TaxRate.HasValue ? (p.TaxRate / 100) : 0) 
                                                : 0
                             };

                return prices;
            }
        }

        private List<PriceCalcView> GetSimulatorCurrentPricesPerDateAndWarrantyId(
            List<int> warrantyIds, List<PriceCalcView> calculatedPrices, DateTime simulatorDate)
        {
            var retPrices = new List<PriceCalcView>();

            // filter dates for simulation - exclude all prices after simulation date 
            calculatedPrices = calculatedPrices.Where(e => e.EffectiveDate <= simulatorDate).ToList();
            foreach (var warId in warrantyIds)
            {
                // process generic prices
                var warrantyCurrentPrice = calculatedPrices
                    .Where(e =>
                        e.WarrantyId == warId && // filter specific warranty id
                        string.IsNullOrWhiteSpace(e.BranchType) && e.BranchNumber == null) // get only generic prices
                    .OrderByDescending(e => e.EffectiveDate)
                    .FirstOrDefault();


                if (warrantyCurrentPrice != null)
                {
                    retPrices.Add(warrantyCurrentPrice);
                }
                
                // process specific prices
                var nonGenericPrices = calculatedPrices
                    .Where(e =>
                        e.WarrantyId == warId && // filter specific warranty id
                        (!string.IsNullOrWhiteSpace(e.BranchType) || e.BranchNumber.HasValue)) // get only specific prices
                    .ToList();

                if (nonGenericPrices.Any())
                {
                    if (warrantyCurrentPrice != null)
                    {
                        // exclude specific prices out of date
                        nonGenericPrices = nonGenericPrices
                            .Where(e => e.EffectiveDate >= warrantyCurrentPrice.EffectiveDate)
                            .ToList();
                    }

                    retPrices.AddRange(nonGenericPrices);
                }
            }

            return retPrices;
        }

        private List<PriceCalcView> WarrantySpecificPriceFiltering(List<PriceCalcView> calculatedPrices)
        {
            var chosenPrices = new List<PriceCalcView>();


            var priceChoiceGroups = (from p in calculatedPrices
                                     where p.WarrantyId.HasValue
                                     group p by p.WarrantyId.Value into choiceGroups
                                     select choiceGroups)
                                     .ToList();

            priceChoiceGroups.ForEach(group =>
            {
                if (group.Count() == 1)
                {
                    chosenPrices.Add(group.First());
                }
                else
                {
                    var classifiedChoices = new List<Tuple<int, PriceCalcView>>();
                    foreach (var elem in group)
                    {
                        var points = 0;
                        if (elem.BranchType != null)
                            points += 1;
                        if (elem.BranchNumber.HasValue)
                            points += 2;

                        classifiedChoices.Add(new Tuple<int, PriceCalcView>(points, elem));
                    }
                    classifiedChoices = classifiedChoices.OrderByDescending(e => e.Item1).ToList();

                    chosenPrices.Add(classifiedChoices.First().Item2);
                }
            });

            return chosenPrices;
        }

        private string GetPriceChangeInfo(decimal? currentPrice, decimal? valueChange, decimal? percentageChange)
        {
            if (percentageChange.HasValue && percentageChange.Value > 0)
            {
                if (currentPrice.HasValue)
                {
                    // 110.00 (+10%)
                    return string.Format("{0:0.00} ({1}{2:0.00}%)", currentPrice.Value, percentageChange.Value >= 0 ? "+" : "", percentageChange.Value);
                }
            }
            else if (valueChange.HasValue && valueChange.Value > 0)
            {
                if (currentPrice.HasValue)
                {
                    // 110.00 (+10)
                    return string.Format("{0:0.00} ({1}{2:0.00})", currentPrice.Value, valueChange.Value >= 0 ? "+" : "", valueChange.Value);
                }
            }
            else // !newPrice.HasValue
            {
                return
                    string.Format("{0:0.00}", currentPrice.Value) + " (no changes)";
            }

            return null;
        }

        public IEnumerable<Model.WarrantyPrice> GetWarrantyPrices(IEnumerable<WarrantyLocation> warrantyLocation)
        {
            var ids = warrantyLocation.Select(s => s.WarrantyNumber).ToList();
            var priceCalcIds = GetPriceCalcViewCache()
                .Where(p => p.Id.HasValue)
                .Select(p => p.Id.Value)
                .ToList();

            using (var scope = Context.Read())
            {
                var warranty = scope.Context.Warranty.Where(w => priceCalcIds.Contains(w.Id) && ids.Contains(w.Number)).ToList();

                return (from w in warranty
                        join wl in warrantyLocation on w.Number equals wl.WarrantyNumber
                        join pc in GetPriceCalcViewCache() on w.Id equals pc.WarrantyId
                        where pc.BranchNumber == wl.Location || pc.BranchNumber == null
                        select new Blue.Cosacs.Warranty.Model.WarrantyPrice
                        {
                            Id = pc.Id.Value,
                            WarrantyId = w.Id,
                            BranchType = pc.BranchType == null ? string.Empty : pc.BranchType,
                            BranchNumber = (short?)wl.Location,
                            CostPrice = Convert.ToDecimal(pc.CostPrice),
                            RetailPrice = Convert.ToDecimal(pc.RetailPrice),
                            EffectiveDate = pc.EffectiveDate.Value
                        });
            }
        }

        public Model.WarrantyLocationPrice Delete(int id)
        {
            using (var scope = Context.Write())
            {
                var price = scope.Context.WarrantyPrice.Find(id);
                if (price != null)
                {
                    // Cannot delete prices defined in the past
                    if (PriceDateIsInThePast(price.EffectiveDate))
                    {
                        throw new ArgumentOutOfRangeException("WarrantyPrice.EffectiveDate", "Cannot delete past prices!");
                    }

                    scope.Context.WarrantyPrice.Remove(price);

                    var locationPrice = new Model.WarrantyLocationPrice
                    {
                        Id = price.Id,
                        WarrantyId = price.WarrantyId,
                        BranchNumber = GetBranchNumberString(price.BranchNumber),
                        BranchType = StoreType.GetNameForType(price.BranchType),
                        CostPrice = price.CostPrice,
                        RetailPrice = price.RetailPrice
                    };

                    audit.LogAsync(new { WarrantyPrice = price }, EventType.DeletePrice, AuditEventCategory);

                    scope.Context.SaveChanges();
                    scope.Complete();

                    DeletePriceCalcViewCache();

                    return locationPrice;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("id", string.Format("Warranty {0} not found!", id));
                }
               
            }
        }

        /// <summary>
        /// Returns true if date is before tomorrow.
        /// </summary>
        private bool PriceDateIsInThePast(DateTime priceDate)
        {
            var tomorrow = clock.Now.Date.AddDays(1);
            return priceDate.Date < tomorrow;
        }

        public string GetBulkEditInfo(int[] filteredIds, Model.WarrantyEditRequest editRequest)
        {
            var bulkEditInfo = "{}";

            using (var scope = Context.Read())
            {
                var sw = new StringWriter();
                var converter = new Newtonsoft.Json.Converters.DataTableConverter();

                var dt = GetInsertBulkEditData(filteredIds, editRequest);

                OnlyIncludeColumns(dt,
                    new List<string>() { "WarrantyNumber", "BranchType",  "BranchNumber",
                        "CostPrice", "RetailPrice", "EffectiveDate", "TaxRate" });

                converter.WriteJson(new Newtonsoft.Json.JsonTextWriter(sw), dt, new Newtonsoft.Json.JsonSerializer());

                bulkEditInfo = sw.ToString();
            }

            return bulkEditInfo;
        }

        private static void OnlyIncludeColumns(DataTable dt, List<string> includeColumns)
        {
            List<string> columnNames = dt.Columns
                .Cast<DataColumn>()
                .Select(x => x.ColumnName)
                .ToList();

            #region bulk edit data columns
            // WarrantyId
            // WarrantyNumber
            // BranchType
            // BranchNumber
            // CostPrice
            // RetailPrice
            // IsCostChangeTo
            // IsRetailChangeTo
            // EffectiveDate
            // CostPriceChange
            // CostPricePercentChange
            // RetailPriceChange
            // RetailPricePercentChange
            // TaxInclusivePriceChange
            // TaxInclusivePricePercentChange
            // BulkEditId
            // AgrmtTaxType
            // TaxRate
            // IsFree
            #endregion
            for (int i = 0; i < columnNames.Count; i++)
            {
                if (!includeColumns.Select(e => e.ToLower()).ToList().Contains(columnNames[i].ToLower()))
                {
                    dt.Columns.Remove(columnNames[i]);
                }
            }
        }

        public void InsertBulkEdit(int[] filteredIds, Model.WarrantyEditRequest editRequest)
        {
            List<int> warrantyIdsUpdated = new List<int>();

            using (var scope = Context.Write())
            {
                var dt = GetInsertBulkEditData(filteredIds, editRequest, true);

                var sw = new StringWriter();
                var jw = new Newtonsoft.Json.JsonTextWriter(sw);
                var converter = new Newtonsoft.Json.Converters.DataTableConverter();
                var serializer = new Newtonsoft.Json.JsonSerializer();
                converter.WriteJson(jw, dt, serializer);

                var tableJson = sw.ToString();

                scope.Complete();
            }

            audit.LogAsync(new
            {
                WarrantyIds = string.Join(",", warrantyIdsUpdated.ToArray()),
                EditRequest = editRequest
            }, EventType.BulkEdit, AuditEventCategory);
        }

        public bool DeleteBulkEdit(int bulkEditId)
        {
            var result = false;
            using (var scope = Context.Write())
            {
                var targetBulkEdit = (from be in scope.Context.WarrantyPrice
                                      where be.BulkEditId == bulkEditId
                                      select be).ToList();

                foreach (var be in targetBulkEdit)
                {
                    if (be.EffectiveDate > clock.Now)
                    { // the price has not yet come into effect
                        result = true;
                        scope.Context.WarrantyPrice.Remove(be);
                    }
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
            DeletePriceCalcViewCache();
            return result;
        }

        private DataTable GetInsertBulkEditData(int[] filteredIds, Model.WarrantyEditRequest editRequest,
                                                bool executeInsert = false)
        {
            var _costPrice = editRequest.CostPrice;
            var _retailPrice = editRequest.RetailPrice;
            var _taxInclusivePrice = editRequest.TaxInclusivePrice;

            // '+': 'Increase by ',   // '+%': 'Increase by %',
            // '-': 'Decrease by ',   // '-%': 'Decrease by %',
            // '=': 'Change to '

            if (_costPrice != null)
                _costPrice.Operation += _costPrice.IsPercentage ? "%" : string.Empty;
            if (_retailPrice != null)
                _retailPrice.Operation += _retailPrice.IsPercentage ? "%" : string.Empty;
            if (_taxInclusivePrice != null)
                _taxInclusivePrice.Operation += _taxInclusivePrice.IsPercentage ? "%" : string.Empty;

            var warrantyIdsFilter = string.Join(",", filteredIds);
            if (warrantyIdsFilter.Length > 4999)
            {
                throw new ArgumentException("Bulk Edit Error: Too many IDs to filter!", "filteredIds");
            }

            var _req = new GetInsertBulkEdit()
            {
                FilteredWarrantyIds = warrantyIdsFilter,

                BulkEditEffectiveYear = (short)editRequest.EffectiveDate.Year,
                BulkEditEffectiveMonth = (short)editRequest.EffectiveDate.Month,
                BulkEditEffectiveDay = (short)editRequest.EffectiveDate.Day,

                CostPriceChange = _costPrice == null ? null :
                _costPrice.Operation == "+" ? (decimal?)_costPrice.Amount :
                (_costPrice.Operation == "-" ? (decimal?)(_costPrice.Amount * -1) : null),
                CostPricePercentChange = _costPrice == null ? null :
                _costPrice.Operation == "+%" ? (decimal?)_costPrice.Amount :
                (_costPrice.Operation == "-%" ? (decimal?)(_costPrice.Amount * -1) : null),
                CostPriceChangeTo = _costPrice == null ? null :
                _costPrice.Operation == "=" ? (decimal?)_costPrice.Amount : null,

                RetailPriceChange = _retailPrice == null ? null :
                _retailPrice.Operation == "+" ? (decimal?)_retailPrice.Amount :
                (_retailPrice.Operation == "-" ? (decimal?)(_retailPrice.Amount * -1) : null),
                RetailPricePercentChange = _retailPrice == null ? null :
                _retailPrice.Operation == "+%" ? (decimal?)_retailPrice.Amount :
                (_retailPrice.Operation == "-%" ? (decimal?)(_retailPrice.Amount * -1) : null),
                RetailPriceChangeTo = _retailPrice == null ? null :
                _retailPrice.Operation == "=" ? (decimal?)_retailPrice.Amount : null,

                TaxInclusivePriceChange = _taxInclusivePrice == null ? null :
                _taxInclusivePrice.Operation == "+" ? (decimal?)_taxInclusivePrice.Amount :
                (_taxInclusivePrice.Operation == "-" ? (decimal?)(_taxInclusivePrice.Amount * -1) : null),
                TaxInclusivePricePercentChange = _taxInclusivePrice == null ? null :
                _taxInclusivePrice.Operation == "+%" ? (decimal?)_taxInclusivePrice.Amount :
                (_taxInclusivePrice.Operation == "-%" ? (decimal?)(_taxInclusivePrice.Amount * -1) : null),
                TaxInclusivePriceChangeTo = _taxInclusivePrice == null ? null :
                _taxInclusivePrice.Operation == "=" ? (decimal?)_taxInclusivePrice.Amount : null,

                InsertBulkEdit = executeInsert
            };

            var ds = new DataSet();
            _req.Fill(ds);

            return ds.Tables[0];
        }

        private decimal? GetUpdatedPrice(decimal? currentPrice, Model.PriceEditRequest editRequest, decimal? taxRate = null)
        {

            if (currentPrice.HasValue && taxRate.HasValue)
                currentPrice = currentPrice.Value + (currentPrice.Value * (taxRate.Value / 100));

            var updatedPrice = currentPrice ?? 0;

            if (editRequest != null && currentPrice.HasValue)
            {
                var change = editRequest.Amount;
                if (editRequest.IsPercentage)
                {
                    change = currentPrice.Value * editRequest.Amount / 100;
                }

                if (editRequest.Operation == "+")
                {
                    updatedPrice = currentPrice.Value + change;
                }
                else if (editRequest.Operation == "-")
                {
                    updatedPrice = currentPrice.Value - change;
                }
                else if (editRequest.Operation == "=")
                {
                    updatedPrice = editRequest.Amount;
                }
            }

            if (updatedPrice < 0)
                updatedPrice = 0;

            if (taxRate.HasValue)
                return updatedPrice / ((100 + taxRate.Value) / 100);
            else
                return updatedPrice;
        }

        private static short? GetBranchNumber(Model.WarrantyLocationPrice warrantyPrice)
        {
            short? branchNumber = null;
            short value = 0;
            short.TryParse(warrantyPrice.BranchNumber, out value);
            if (value > 0)
            {
                branchNumber = value;
            }
            return branchNumber;
        }

        private static string GetBranchNumberString(short? branchNumber)
        {
            return branchNumber.HasValue ? branchNumber.Value.ToString(CultureInfo.InvariantCulture) : null;
        }

        private class EventType
        {
            public const string CreatePrice = "CreatePrice";
            public const string EditPrice = "EditPrice";
            public const string DeletePrice = "DeletePrice";
            public const string BulkEdit = "BulkEdit";
        }

        private decimal GetCountryTaxRate()
        {
            var currentTaxRateObj = merchandisingTaxRepo.GetCurrent();
            if (currentTaxRateObj != null)
            {
                return merchandisingTaxRepo.GetCurrent().Rate * 100;
            }
            return 0;
        }
    }
}
