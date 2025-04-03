using Blue.Cosacs.Credit.Repositories.Reindex;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Credit.Repositories
{
    public class TermsTypeRepository : ITermsTypeRepository
    {
        private readonly IClock clock;
        private readonly ITermsTypeSolrIndex index;

        public TermsTypeRepository(IClock clock, ITermsTypeSolrIndex index)
        {
            this.clock = clock;
            this.index = index;
        }

        private int GetTermsTypeId(string name)
        {
            using (var scope = Context.Write())
            {
                var termsType = scope.Context.TermsType.Where(t => t.Name == name).FirstOrDefault();
                if (termsType == null)
                {
                    termsType = new TermsType() { Name = name };
                    scope.Context.TermsType.Add(termsType);
                    scope.Context.SaveChanges();
                }
                scope.Complete();
                return termsType.Id;
            }
        }

        public void Save(Model.TermsTypeDetails termsType, int updatedBy, DateTime updatedOn)
        {
            using (var scope = Context.Write())
            {
                if (termsType.State == "new")
                {
                    if (ExistsTermsTypeName(termsType.Name))
                    {
                        throw new Exception("The Terms Type name already exists. You cannot create a new terms type with an existing name. Please choose another name and try again.");
                    }
                }
                else
                {
                    if (ExistsTermsTypeName(termsType.Name, termsType.TermsTypeId))
                    {
                        throw new Exception("The Terms Type name already exists. You cannot create a new terms type with an existing name. Please choose another name and try again.");
                    }
                }

                var indexId = new List<int>();
                var result = termsType.ToTable(clock.Now);

                var id = GetTermsTypeId(termsType.Name);
                result.TermsTypeId = id;

                var oldTermsType = (from td in scope.Context.TermsTypeDetails
                                    join t in scope.Context.TermsType on td.TermsTypeId equals t.Id
                                    where t.Name == termsType.Name && !td.EndDate.HasValue
                                    select td).FirstOrDefault();

                scope.Context.TermsTypeDetails.Add(result);
                scope.Context.SaveChanges();
                indexId.Add(result.Id);

                if (oldTermsType != null)
                {
                    oldTermsType.EndDate = clock.Now;
                    indexId.Add(oldTermsType.Id);
                }

                SaveProductHierarchyTag(termsType.ProductHierarchyTags, result.TermsTypeId);
                SaveFreeInstalments(termsType.FreeInstalments, result.TermsTypeId);
                SaveFascia(termsType.Fascias, result.TermsTypeId);
                SaveTermsTypeCustomerBand(termsType.CustomerBands, result.TermsTypeId, updatedBy, updatedOn);
                SaveTermsTypeCustomerTag(termsType.CustomerTags, result.TermsTypeId);
                SaveTermsTypeProductSKU(termsType.ProductSkus, result.TermsTypeId);

                scope.Context.SaveChanges();
                index.Reindex(indexId.ToArray());
                scope.Complete();
            }
        }

        private bool ExistsTermsTypeName(string name, int? id = null)
        {
            using (var scope = Context.Read())
            {
                var termsType = scope.Context.TermsType.Where(p => p.Name == name);

                if (id.HasValue)
                {
                    termsType.Where(p => p.Id != id);
                }

                return termsType.Any();
            }
        }

        public Model.TermsTypeDetails Get(int id)
        {
            var termsType = new Model.TermsTypeDetails();
            using (var scope = Context.Read())
            {
                termsType = (from td in scope.Context.TermsTypeDetails
                             join t in scope.Context.TermsType on td.TermsTypeId equals t.Id
                             where td.TermsTypeId == id
                             select new Model.TermsTypeDetails()
                             {
                                 TermsTypeId = td.TermsTypeId,
                                 Name = t.Name,
                                 IsDisabled = td.IsDisabled,
                                 IsCashLoanStaffCustomer = td.IsCashLoanStaffCustomer,
                                 IsCashLoanRecentCustomer = td.IsCashLoanRecentCustomer,
                                 IsCashLoanNewCustomer = td.IsCashLoanNewCustomer,
                                 InterestFree = td.InterestFree,
                                 HasPaymentHolidays = td.HasPaymentHolidays,
                                 FullRebate = td.FullRebate,
                                 InterestRateHolidays = td.InterestRateHolidays,
                                 IsCashLoanExistingCustomer = td.IsCashLoanExistingCustomer,
                                 CreatedOn = td.CreatedOn,
                                 DefaultTermLength = td.DefaultTermLength,
                                 CustomerType = td.CustomerType,
                                 IsHPAgreement = td.IsHPAgreement,
                                 EndDate = td.EndDate,
                                 IsRFAgreement = td.IsRFAgreement,
                                 IsStoreCardAgreement = td.IsStoreCardAgreement,
                                 CreatedBy = td.CreatedBy,
                                 IsStaff = td.IsStaff,
                                 MaxTermLength = td.MaxTermLength,
                                 MinTermLength = td.MinTermLength,
                             }).FirstOrDefault();

                if (termsType != null)
                {
                    termsType.FreeInstalments = scope.Context.TermsTypeFreeInstalments.Where(p => p.TermsTypeDetailsId == termsType.TermsTypeId).Select(p => p.FreeInstalmentInMonths.ToString()).ToList();
                    termsType.ProductHierarchyTags = scope.Context.TermsTypeProductHierarchyTag.Where(p => p.TermsTypeDetailsId == termsType.TermsTypeId).Select(p => new Model.ProductHierarchyTag()
                    {
                        Level = p.Level,
                        Tag = p.TagName
                    }).ToList();
                    termsType.Fascias = scope.Context.TermsTypeFascia.Where(p => p.TermsTypeDetailsId == termsType.TermsTypeId).Select(p => p.Fascia).ToList();
                    termsType.CustomerBands = scope.Context.TermsTypeCustomerBand.Where(p => p.TermsTypeDetailsId == termsType.TermsTypeId).Select(p => new Model.CustomerBand()
                    {
                        AdminPercentage = p.AdminPercentage,
                        AdminValue = p.AdminValue,
                        CpiPercentage = p.CpiPercentage,
                        DepositRequiredPercentage = p.DepositRequiredPercentage,
                        EndDate = p.EndDate,
                        InterestRatePercentage = p.InterestRatePercentage,
                        StartDate = p.StartDate,
                        UpdatedBy = p.UpdatedBy,
                        UpdatedOn = p.UpdatedOn,
                        BandName = p.BandName,
                        PointsFrom = p.PointsFrom,
                        PointsTo = p.PointsTo
                    }).ToList();
                    termsType.CustomerTags = scope.Context.TermsTypeCustomerTag.Where(p => p.TermsTypeDetailsId == termsType.TermsTypeId).Select(p => p.TagName).ToList();
                    termsType.ProductSkus = scope.Context.TermsTypeProductSKU.Where(p => p.TermsTypeDetailsId == termsType.TermsTypeId).Select(p => new Model.ProductSKU()
                    {
                        SkuDescription = p.Description,
                        SkuId = p.SkuId
                    }).ToList();
                }

                return termsType;
            }
        }

        private List<Model.TermsTypeSimulator> GetMany(IEnumerable<int> ids)
        {
            using (var scope = Context.Read())
            {
                var termsTypes = (from ttd in scope.Context.TermsTypeDetails
                                  join t in scope.Context.TermsType on ttd.TermsTypeId equals t.Id
                                  where ids.Contains(t.Id)
                                  select new { t, ttd }).ToList();

                var freeInstalments = scope.Context.TermsTypeFreeInstalments.Where(p => ids.Contains(p.TermsTypeDetailsId)).ToLookup(s => s.TermsTypeDetailsId);
                var productHierarchyTags = scope.Context.TermsTypeProductHierarchyTag.Where(h => ids.Contains(h.TermsTypeDetailsId))
                    .Select(p => new
                    {
                        id = p.TermsTypeDetailsId,
                        productHierarchyTag = new Model.ProductHierarchyTagSimulator()
                            {
                                Level = p.Level,
                                Tag = p.TagName
                            }
                    }).ToLookup(l => l.id);

                var fascias = scope.Context.TermsTypeFascia.Where(p => ids.Contains(p.TermsTypeDetailsId)).ToLookup(l => l.TermsTypeDetailsId);
                var customerBands = scope.Context.TermsTypeCustomerBand.Where(c => ids.Contains(c.TermsTypeDetailsId)).Select(p => new
                {
                    id = p.TermsTypeDetailsId,
                    band = new Model.CustomerBandSimulator()
                        {
                            AdminPercentage = p.AdminPercentage,
                            AdminValue = p.AdminValue,
                            CpiPercentage = p.CpiPercentage,
                            DepositRequiredPercentage = p.DepositRequiredPercentage,
                            EndDate = p.EndDate,
                            InterestRatePercentage = p.InterestRatePercentage,
                            StartDate = p.StartDate,
                            UpdatedBy = p.UpdatedBy,
                            UpdatedOn = p.UpdatedOn,
                            BandName = p.BandName
                        }
                }).ToLookup(l => l.id);
                var customerTags = scope.Context.TermsTypeCustomerTag.Where(c => ids.Contains(c.TermsTypeDetailsId)).ToLookup(l => l.TermsTypeDetailsId);
                var productSkus = scope.Context.TermsTypeProductSKU.Where(p => ids.Contains(p.TermsTypeDetailsId)).Select(p => new
                {
                    id = p.TermsTypeDetailsId,
                    sku = new Model.ProductSKUSimulator()
                        {
                            SkuDescription = p.Description,
                            SkuId = p.SkuId
                        }
                }).ToLookup(s => s.id);

                var termsTypeDetails = new List<Model.TermsTypeSimulator>();

                termsTypes.ForEach(termsType =>
                {
                    termsTypeDetails.Add(new Model.TermsTypeSimulator(
                            termsType.ttd,
                            termsType.t.Name,
                            productHierarchyTags[termsType.ttd.Id].Select(p => p.productHierarchyTag).ToList(),
                            freeInstalments[termsType.ttd.Id].Select(p => p.FreeInstalmentInMonths.ToString()).ToList(),
                            fascias[termsType.ttd.Id].Select(p => p.Fascia).ToList(),
                            customerBands[termsType.ttd.Id].Select(p => p.band).ToList(),
                            customerTags[termsType.ttd.Id].Select(p => p.TagName).ToList(),
                            productSkus[termsType.ttd.Id].Select(p => p.sku).ToList()));
                });

                return termsTypeDetails.OrderByDescending(p => p.CreatedOn).Take(50).ToList();
            }
        }

        public List<Model.TermsTypeSimulator> Search(Model.TermsTypeSearch search)
        {
            using (var scope = Context.Read())
            {
                var allTermsTypes = from details in scope.Context.TermsTypeDetails
                                    join type in scope.Context.TermsType on details.TermsTypeId equals type.Id
                                    select new { Details = details, Type = type };

                if (search.IsCashLoanExistingCustomer)
                {
                    allTermsTypes = allTermsTypes.Where(t => t.Details.IsCashLoanExistingCustomer);
                }

                if (search.IsCashLoanNewCustomer)
                {
                    allTermsTypes = allTermsTypes.Where(t => t.Details.IsCashLoanNewCustomer);
                }

                if (search.IsCashLoanRecentCustomer)
                {
                    allTermsTypes = allTermsTypes.Where(t => t.Details.IsCashLoanRecentCustomer);
                }

                if (search.IsCashLoanStaffCustomer)
                {
                    allTermsTypes = allTermsTypes.Where(t => t.Details.IsCashLoanStaffCustomer);
                }

                if (search.IsHpAgreement)
                {
                    allTermsTypes = allTermsTypes.Where(t => t.Details.IsHPAgreement);
                }

                if (search.IsRfAgreement)
                {
                    allTermsTypes = allTermsTypes.Where(t => t.Details.IsRFAgreement);
                }

                if (search.IsStaff)
                {
                    allTermsTypes = allTermsTypes.Where(t => t.Details.IsStaff);
                }

                if (search.IsStoreCardAgreement)
                {
                    allTermsTypes = allTermsTypes.Where(t => t.Details.IsStoreCardAgreement);
                }

                if (!string.IsNullOrEmpty(search.CustomerType))
                {
                    allTermsTypes = allTermsTypes.Where(t => t.Details.CustomerType == search.CustomerType);
                }

                if (search.Date.HasValue || search.Points.HasValue)
                {
                    allTermsTypes = from a in allTermsTypes
                                    join band in scope.Context.TermsTypeCustomerBand on a.Details.Id equals band.TermsTypeDetailsId
                                    where (search.Date.HasValue &&
                                          (search.Date.Value >= a.Details.CreatedOn &&
                                           search.Date.Value < a.Details.CreatedOn &&
                                           search.Date.Value >= band.StartDate &&
                                           search.Date.Value < band.EndDate)) ||
                                           (search.Points.HasValue && (search.Points.Value >= band.PointsFrom &&
                                                                search.Points.Value < band.PointsTo))
                                    select a;
                }

                if (search.CustomerTags.Any())
                {
                    allTermsTypes = from a in allTermsTypes
                                    join tags in scope.Context.TermsTypeCustomerTag on a.Details.Id equals tags.TermsTypeDetailsId
                                    where search.CustomerTags.Contains(tags.TagName)
                                    select a;
                }

                if (search.Fascias.Any())
                {
                    allTermsTypes = from a in allTermsTypes
                                    join fascia in scope.Context.TermsTypeFascia on a.Details.Id equals fascia.TermsTypeDetailsId
                                    where search.Fascias.Contains(fascia.Fascia)
                                    select a;
                }

                if (search.ProductHierarchyTags.Any())
                {
                    var productHierarchyTagNames = search.ProductHierarchyTags.Select(p => p.Tag);
                    allTermsTypes = from a in allTermsTypes
                                    join hierarchyTags in scope.Context.TermsTypeProductHierarchyTag on a.Details.Id equals hierarchyTags.TermsTypeDetailsId
                                    where productHierarchyTagNames.Contains(hierarchyTags.TagName)
                                    select a;
                }

                if (search.ProductSkus.Any())
                {
                    var productSkusDescription = search.ProductSkus.Select(p => p.SkuDescription);
                    allTermsTypes = from a in allTermsTypes
                                    join sku in scope.Context.TermsTypeProductSKU on a.Details.Id equals sku.TermsTypeDetailsId
                                    where productSkusDescription.Contains(sku.Description)
                                    select a;
                }

                if (search.TermLength.HasValue)
                {
                    allTermsTypes = allTermsTypes.Where(t => search.TermLength >= t.Details.MinTermLength && search.TermLength < t.Details.MaxTermLength);
                }

                return GetMany(allTermsTypes.Select(t => t.Details.Id).ToList());
            }
        }

        private void SaveTermsTypeCustomerTag(List<string> customerTags, int termsTypeId)
        {
            using (var scope = Context.Write())
            {
                var customerTagsList = customerTags.Select(p => new TermsTypeCustomerTag() { TagName = p, TermsTypeDetailsId = termsTypeId });
                scope.Context.TermsTypeCustomerTag.AddRange(customerTagsList);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private void SaveTermsTypeCustomerBand(List<Model.CustomerBand> termsTypeCustomerBands, int termsTypeId, int updatedBy, DateTime updatedOn)
        {
            using (var scope = Context.Write())
            {
                var customerBandsList = termsTypeCustomerBands.Select(p => new TermsTypeCustomerBand()
                {
                    TermsTypeDetailsId = termsTypeId,
                    BandName = p.BandName,
                    AdminPercentage = p.AdminPercentage,
                    AdminValue = p.AdminValue,
                    CpiPercentage = p.CpiPercentage,
                    DepositRequiredPercentage = p.DepositRequiredPercentage,
                    EndDate = p.EndDate,
                    InterestRatePercentage = p.InterestRatePercentage,
                    StartDate = p.StartDate.Date,
                    UpdatedBy = updatedBy,
                    UpdatedOn = updatedOn,
                    PointsFrom = p.PointsFrom,
                    PointsTo = p.PointsTo
                });
                scope.Context.TermsTypeCustomerBand.AddRange(customerBandsList);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private void SaveProductHierarchyTag(List<Model.ProductHierarchyTag> productHierarchyTags, int termsTypeId)
        {
            using (var scope = Context.Write())
            {
                var productHierarchyTagsList = productHierarchyTags.Select(p => new TermsTypeProductHierarchyTag() { Level = p.Level, TagName = p.Tag, TermsTypeDetailsId = termsTypeId });
                scope.Context.TermsTypeProductHierarchyTag.AddRange(productHierarchyTagsList);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private void SaveFascia(List<string> fascias, int termsTypeId)
        {
            using (var scope = Context.Write())
            {
                var fasciaList = fascias.Select(p => new TermsTypeFascia() { Fascia = p, TermsTypeDetailsId = termsTypeId });
                scope.Context.TermsTypeFascia.AddRange(fasciaList);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private void SaveFreeInstalments(List<string> freeInstalments, int termsTypeId)
        {
            using (var scope = Context.Write())
            {
                var freeInstalmentsList = freeInstalments.Select(p => new TermsTypeFreeInstalments() { FreeInstalmentInMonths = int.Parse(p), TermsTypeDetailsId = termsTypeId });
                scope.Context.TermsTypeFreeInstalments.AddRange(freeInstalmentsList);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private void SaveTermsTypeProductSKU(List<Model.ProductSKU> productSKUs, int termsTypeId)
        {
            using (var scope = Context.Write())
            {
                var skusList = productSKUs.Select(p => new TermsTypeProductSKU() { TermsTypeDetailsId = termsTypeId, Description = p.SkuDescription, SkuId = p.SkuId });
                scope.Context.TermsTypeProductSKU.AddRange(skusList);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }
    }
}
