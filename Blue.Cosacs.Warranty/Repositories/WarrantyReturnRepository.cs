using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Blue.Events;
using Blue.Cosacs.Warranty.Model;
using Blue.Data;

namespace Blue.Cosacs.Warranty.Repositories
{
    public class WarrantyReturnRepository
    {
        private IEventStore audit;
        private const string AuditEventCategory = "WarrantyReturn";

        public WarrantyReturnRepository(IEventStore audit)
        {
            this.audit = audit;
        }

        public Model.WarrantyReturnModel Save(Model.WarrantyReturnModel _return)
        {
            using (var scope = Context.Write())
            {
                int? warrantyId = null;
                if (_return.Warranty != null)
                {
                    warrantyId = _return.Warranty.Id;
                }
                var newReturn = scope.Context.WarrantyReturn.Add(new WarrantyReturn
                {
                    WarrantyLength = _return.WarrantyLength,
                    ElapsedMonths = _return.ElapsedMonths,
                    FreeWarrantyLength = (byte?)_return.FreeWarrantyLength,
                    PercentageReturn = _return.PercentageReturn,
                    BranchType = _return.BranchType,
                    BranchNumber = _return.BranchNumber,
                    WarrantyId = warrantyId,
                    Level_1 = _return.WarrantyReturnFilters.Count() == 0 ? string.Empty 
                        : String.Compare(_return.WarrantyReturnFilters.Select(t => t.TagName).FirstOrDefault(), "Furniture", true) == 0 ? "PCF" : "PCE" 
                });

                scope.Context.SaveChanges();

                _return.Id = newReturn.Id;

                if (_return.WarrantyReturnFilters != null)
                {
                    foreach (var item in _return.WarrantyReturnFilters)
                    {
                        scope.Context.WarrantyReturnFilter.Add(new WarrantyReturnFilter
                        {
                            WarrantyReturnId = _return.Id,
                            TagId = item.TagId
                        });
                    }
                }

                scope.Context.SaveChanges();
                scope.Complete();

                audit.LogAsync(new { WarrantyReturn = _return }, EventType.CreateReturn, AuditEventCategory);
            }

            return _return;
        }

        public void Delete(int returnId)
        {
            using (var scope = Context.Write())
            {
                var filters = scope.Context.WarrantyReturnFilter.Where(f => f.WarrantyReturnId == returnId);
                var filterList = filters.ToList();
                filterList.ForEach(f => scope.Context.WarrantyReturnFilter.Remove(f));
                scope.Context.SaveChanges();

                var _return = scope.Context.WarrantyReturn.Find(returnId);
                if (_return != null)
                {
                    scope.Context.WarrantyReturn.Remove(_return);
                }

                audit.LogAsync(new { WarrantyReturn = _return, Filters = filterList }, EventType.DeleteReturn, AuditEventCategory);

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public IEnumerable<Model.WarrantyReturnModel> GetReturns()
        {
            return GetAllReturns().ToList();
        }

        public Model.WarrantyReturnModel Get(string warrantyNumber, int branchNumber, int elapsedMonths, int freeWarrantyLength)
        {
            var warrantyLength = 0;
            using (var scope = Context.Read())
            {
                warrantyLength = scope.Context.Warranty
                    .Where(e => e.Number == warrantyNumber)
                    .Select(e => e.Length)
                    .FirstOrDefault();
            }

            if (warrantyLength <= 0)
            {
                throw new Exception("Warranty Length " + warrantyLength + " is not valid!");
            }

            var allReturns = GetAllReturns(warrantyNumber);

            var specificReturns = SearchForSpecificWarrantyId(allReturns, warrantyNumber, warrantyLength, freeWarrantyLength, elapsedMonths);
            if (specificReturns.Any())
            {
                allReturns = specificReturns;
            }

            var tmpLengthFilter = allReturns.Where(e => e.WarrantyLength == warrantyLength &&
                (e.FreeWarrantyLength == null || e.FreeWarrantyLength.Value == freeWarrantyLength)
                ).ToList();

            allReturns = tmpLengthFilter.Where(w => (w.BranchNumber == null || w.BranchNumber == branchNumber)
                && ((w.Warranty != null && w.Warranty.Number == warrantyNumber) || (w.Warranty == null))
                && warrantyLength == w.WarrantyLength
                && (w.FreeWarrantyLength == null || w.FreeWarrantyLength.Value == freeWarrantyLength)
                && (elapsedMonths <= w.ElapsedMonths)) //Warranty Return Elapsed
                .OrderBy(w => w.ElapsedMonths)
                .ToList();

            var resp = allReturns.FirstOrDefault();

            if (resp == null)
            {
                // No valid warranty return found, return PercentageReturn = 0
                return new WarrantyReturnModel()
                {
                    Id = 0,
                    WarrantyLength = warrantyLength,
                    FreeWarrantyLength = freeWarrantyLength,
                    ElapsedMonths = elapsedMonths,
                    PercentageReturn = 0,
                    BranchType = null,
                    BranchNumber = null,
                    WarrantyNo = warrantyNumber,
                    WarrantyId = null
                };
            }
            else
            {
                return resp;
            }
        }

        private List<WarrantyReturnModel> SearchForSpecificWarrantyId(IEnumerable<WarrantyReturnModel> allReturns,
             string warrantyNumber, int warrantyLength, int freeWarrantyLength, int elapsedMonths)
        {
            using (var scope = Context.Read())
            {
                var dbFilters = GetWarrantyReturnTagLevelFilters(scope);

                var dbReturnCodes = (from war in scope.Context.Warranty
                                     join ret in scope.Context.WarrantyReturn on war.Id equals ret.WarrantyId
                                     where war.Number == warrantyNumber &&
                                     (ret.WarrantyLength.HasValue || war.Length == ret.WarrantyLength.Value) &&
                                     (ret.FreeWarrantyLength == null || ret.FreeWarrantyLength.Value == freeWarrantyLength) &&
                                     ret.ElapsedMonths == elapsedMonths
                                     select new { ret, war })
                                     .ToList(); // get db results

                var specificReturnCode = dbReturnCodes
                    .Select(e => GetWarrantyModel(warrantyNumber, dbFilters, new WarrantiesReturnPercentageAndWarranty() { ReturnPercentage = e.ret, Warranty = e.war }))
                    .Where(e => e.WarrantyLength != null) // Only apply specific WarrantyId rule for ReturnCodes with a 'WarrantyLength' not null
                    .ToList();

                if (specificReturnCode.Any())
                {
                    return specificReturnCode; // If specific codes exist, return only specific WarrantyId codes
                }
            }

            return new List<WarrantyReturnModel>(); // empty list
        }

        private IEnumerable<WarrantyReturnModel> GetAllReturns(string warrantyNumber = null)
        {
            using (var scope = Context.Read())
            {
                ////var departmentFromLink = (
                ////    from w in scope.Context.Warranty
                ////    join lw in scope.Context.LinkWarranty on w.Id equals lw.WarrantyId
                ////    join lp in scope.Context.LinkProduct on lw.LinkId equals lp.LinkId
                ////    where warrantyNumber != null && w.Number == warrantyNumber
                ////    select lp.Level_1
                ////    ).FirstOrDefault();

                var departmentFromWarranty = (
                    from w in scope.Context.Warranty
                    join wt in scope.Context.WarrantyTags on w.Id equals wt.WarrantyId
                    join t in scope.Context.Tag on wt.TagId equals t.Id
                    where warrantyNumber != null && w.Number == warrantyNumber
                    select t.Name
                    ).FirstOrDefault();

           
                var dbFilters = GetWarrantyReturnTagLevelFilters(scope);

                var dbReturns = (from returns in scope.Context.WarrantyReturn
                                 join w in scope.Context.Warranty on returns.WarrantyId equals w.Id into warrantyReturnsLeftJoin
                                 from warranty in warrantyReturnsLeftJoin.DefaultIfEmpty()
                                 select new 
                                 {
                                     ReturnPercentage = returns,
                                     Warranty = warranty,
                                     Level_1 = returns.Level_1
                                 });

                if (!string.IsNullOrEmpty(departmentFromWarranty))
                {
                    dbReturns = dbReturns
                        .Where(p => (departmentFromWarranty == "Electrical" ? "PCE" : "PCF") == p.Level_1);
                }

                return dbReturns
                           .Select(p => new WarrantiesReturnPercentageAndWarranty { ReturnPercentage = p.ReturnPercentage, Warranty = p.Warranty })
                           .ToList()
                           .Select(p => GetWarrantyModel(warrantyNumber, dbFilters, p))
                           .ToList();
            }
        }

        private Model.WarrantyReturnModel GetWarrantyModel(string warrantyNumber,
            ILookup<int, WarrantyReturnFilterModel> dbFilters, WarrantiesReturnPercentageAndWarranty p)
        {
            return new Model.WarrantyReturnModel
            {
                Id = p.ReturnPercentage.Id,
                WarrantyReturnFilters = dbFilters[p.ReturnPercentage.Id],
                WarrantyLength = p.ReturnPercentage.WarrantyLength,
                ElapsedMonths = p.ReturnPercentage.ElapsedMonths,
                FreeWarrantyLength = p.ReturnPercentage.FreeWarrantyLength,
                PercentageReturn = p.ReturnPercentage.PercentageReturn,
                BranchType = StoreType.GetNameForType(p.ReturnPercentage.BranchType),
                BranchNumber = p.ReturnPercentage.BranchNumber,
                Warranty = (p.Warranty == null) ? null : new Model.Warranty { Id = p.Warranty.Id, Number = p.Warranty.Number },
                WarrantyNo = warrantyNumber,
                WarrantyId = (p.Warranty == null) ? null : (int?)p.Warranty.Id
            };
        }

        private static ILookup<int, WarrantyReturnFilterModel> GetWarrantyReturnTagLevelFilters(Transactions.ReadScope<Context> scope)
        {
            var dbFilters = (from filter in scope.Context.WarrantyReturnFilter
                             join tag in scope.Context.Tag on filter.TagId equals tag.Id
                             join level in scope.Context.Level on tag.LevelId equals level.Id
                             select new
                             {
                                 WarrantyReturnId = filter.WarrantyReturnId,
                                 WarrantyReturnFilters = new Model.WarrantyReturnFilterModel { Id = filter.Id, WarrantyReturnId = filter.WarrantyReturnId, LevelId = level.Id, LevelName = level.Name, TagId = tag.Id, TagName = tag.Name }
                             }).ToLookup(f => f.WarrantyReturnId, f => f.WarrantyReturnFilters);

            return dbFilters;
        }

        public IPagedSearchResults<WarrantyReturnModel> GetAllReturns(WarrantyReturnSearch search)
        {
            if (search == null) { search = new WarrantyReturnSearch(); }

            using (var scope = Context.Read())
            {
                var dbFilters = GetWarrantyReturnTagLevelFilters(scope);

                var filtered = (from filter in scope.Context.WarrantyReturnSearchView
                                where (search.SearchCriteria == null ||
                                    filter.Level.ToLower() == search.SearchCriteria ||
                                    filter.Tag.ToLower() == search.SearchCriteria ||
                                    filter.WarrantyDescription.Contains(search.SearchCriteria) ||
                                    filter.WarrantyNo.Contains(search.SearchCriteria))
                                select filter.WarrantyReturnId).Distinct().ToArray();

                var tmp = from returns in scope.Context.WarrantyReturn
                          join w in scope.Context.Warranty on returns.WarrantyId equals w.Id into wj
                          from warranty in wj.DefaultIfEmpty()
                          where filtered.Contains(returns.Id)
                          orderby returns.PercentageReturn descending
                          select new WarrantyReturnModel
                          {
                              Id = returns.Id,
                              WarrantyLength = returns.WarrantyLength,
                              ElapsedMonths = returns.ElapsedMonths,
                              FreeWarrantyLength = (int?)returns.FreeWarrantyLength,
                              PercentageReturn = returns.PercentageReturn,
                              BranchType = returns.BranchType,
                              BranchNumber = returns.BranchNumber

                          };

                var filters = from filter in scope.Context.WarrantyReturnFilter
                              join tag in scope.Context.Tag on filter.TagId equals tag.Id
                              join level in scope.Context.Level on tag.LevelId equals level.Id
                              select new
                              {
                                  filter.WarrantyReturnId,
                                  WarrantyReturnFilters =
                                      new WarrantyReturnFilterModel
                                      {
                                          Id = filter.Id,
                                          WarrantyReturnId = filter.WarrantyReturnId,
                                          LevelId = level.Id,
                                          LevelName = level.Name,
                                          TagId = tag.Id,
                                          TagName = tag.Name
                                      }
                              };



                var query = search.Page(from returns in scope.Context.WarrantyReturn
                                        join w in scope.Context.Warranty on returns.WarrantyId equals w.Id into wj
                                        from warranty in wj.DefaultIfEmpty()
                                        where filtered.Contains(returns.Id)
                                        orderby returns.Id descending
                                        select new WarrantyReturnModel
                                        {
                                            Id = returns.Id,
                                            WarrantyLength = returns.WarrantyLength,
                                            ElapsedMonths = returns.ElapsedMonths,
                                            FreeWarrantyLength = (int?)returns.FreeWarrantyLength,
                                            PercentageReturn = returns.PercentageReturn,
                                            BranchType = returns.BranchType,
                                            BranchNumber = returns.BranchNumber,
                                            WarrantyNo = warranty == null ? null : warranty.Number,
                                            WarrantyId = (warranty == null ? (int?)null : warranty.Id)
                                        });

                if (query.Page == null) return query;

                foreach (var rm in query.Page)
                {
                    rm.Warranty = (rm.WarrantyId.HasValue)
                        ? new Model.Warranty { Id = rm.WarrantyId.Value, Number = rm.WarrantyNo }
                        : null;
                    rm.WarrantyReturnFilters = dbFilters[rm.Id];
                }

                return query;
            }
        }

        private static short? GetShort(string stringValue)
        {
            short? numericValue = null;
            short value = 0;
            short.TryParse(stringValue, out value);
            if (value > 0)
            {
                numericValue = value;
            }
            return numericValue;
        }

        private static string GetString(int? value)
        {
            return value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : null;
        }

        private class EventType
        {
            public const string CreateReturn = "CreateReturnPercentage";
            public const string DeleteReturn = "DeleteReturnPercentage";
        }
    }
}
