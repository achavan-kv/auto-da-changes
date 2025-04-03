using Blue.Cosacs.Credit.Extensions;
using Blue.Cosacs.Credit.Repositories.Reindex;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Credit.Repositories
{
    public class TermsTypeSolrIndex : ITermsTypeSolrIndex
    {
        private const int MAXRECORDS = 10000;

        public int Reindex()
        {
            using (var scope = Context.Read())
            {
                var termsTypes = scope.Context.TermsTypeDetails;
                if (!termsTypes.Any())
                {
                    return 0;
                }
                var top = termsTypes.Max(c => c.Id);
                var count = 0;
                var recordId = 0;
                while (recordId < top)
                {
                    var records = termsTypes.Where(c => c.Id > recordId && c.Id <= recordId + MAXRECORDS).ToList();
                    var ids = records.Select(r => r.Id).ToList();
                    var names = scope.Context.TermsType.ToDictionary(t => t.Id);
                    var customerTags = scope.Context.TermsTypeCustomerTag.Where(c => ids.Contains(c.TermsTypeDetailsId)).ToLookup(c => c.TermsTypeDetailsId);
                    var customerBand = scope.Context.TermsTypeCustomerBand.Where(c => ids.Contains(c.TermsTypeDetailsId)).ToLookup(c => c.TermsTypeDetailsId);
                    var productHierarchyTag = scope.Context.TermsTypeProductHierarchyTag.Where(c => ids.Contains(c.TermsTypeDetailsId)).ToLookup(c => c.TermsTypeDetailsId);
                    var fascia = scope.Context.TermsTypeFascia.Where(c => ids.Contains(c.TermsTypeDetailsId)).ToLookup(c => c.TermsTypeDetailsId);
                    var freeInstalments = scope.Context.TermsTypeFreeInstalments.Where(c => ids.Contains(c.TermsTypeDetailsId)).ToLookup(c => c.TermsTypeDetailsId);
                    if (records.Count() > 0)
                    {
                        PushSolr(names, records, customerTags, customerBand, productHierarchyTag, fascia, freeInstalments);
                    }
                    recordId = records.Max(r => r.Id);
                    count += records.Count();
                }
                return count;
            }
        }

        public void Reindex(int[] termsTypeIds)
        {
            using (var scope = Context.Read())
            {
                var names = scope.Context.TermsType.ToDictionary(t => t.Id);
                var records = scope.Context.TermsTypeDetails.Where(c => termsTypeIds.Contains(c.Id)).ToList();
                var customerTags = scope.Context.TermsTypeCustomerTag.Where(c => termsTypeIds.Contains(c.TermsTypeDetailsId)).ToLookup(c => c.TermsTypeDetailsId);
                var customerBand = scope.Context.TermsTypeCustomerBand.Where(c => termsTypeIds.Contains(c.TermsTypeDetailsId)).ToLookup(c => c.TermsTypeDetailsId);
                var productHierarchyTag = scope.Context.TermsTypeProductHierarchyTag.Where(c => termsTypeIds.Contains(c.TermsTypeDetailsId)).ToLookup(c => c.TermsTypeDetailsId);
                var fascia = scope.Context.TermsTypeFascia.Where(c => termsTypeIds.Contains(c.TermsTypeDetailsId)).ToLookup(c => c.TermsTypeDetailsId);
                var freeInstalments = scope.Context.TermsTypeFreeInstalments.Where(c => termsTypeIds.Contains(c.TermsTypeDetailsId)).ToLookup(c => c.TermsTypeDetailsId);
                if (records.Count() > 0)
                {
                    PushSolr(names, records, customerTags, customerBand, productHierarchyTag, fascia, freeInstalments);
                }
            }
        }

        private void PushSolr(
            IDictionary<int, TermsType> termsType,
            IEnumerable<TermsTypeDetails> termsTypesDetails,
            ILookup<int, TermsTypeCustomerTag> customerTags = null,
            ILookup<int, TermsTypeCustomerBand> customerBands = null,
            ILookup<int, TermsTypeProductHierarchyTag> productHierarchyTag = null,
            ILookup<int, TermsTypeFascia> fascia = null,
            ILookup<int, TermsTypeFreeInstalments> freeInstalments = null)
        {
            const string DocType = "TermsType";

            new Blue.Solr.WebClient().Update(termsTypesDetails
                       .Select(tt => new
                       {
                           Id = string.Format("{0}:{1}", DocType, tt.Id.ToString()),
                           Type = DocType,
                           TermsType = termsType[tt.TermsTypeId].Name.SafeTrim(),
                           MinTermLength = tt.MinTermLength,
                           MaxTermLength = tt.MaxTermLength,
                           DefaultTermLength = tt.DefaultTermLength,
                           InterestRateHolidays = tt.InterestRateHolidays,
                           FullRebate = tt.FullRebate,
                           HasPaymentHolidays = tt.HasPaymentHolidays,
                           CustomerType = string.IsNullOrEmpty(tt.CustomerType) ? "New, Returning" : tt.CustomerType,
                           InterestFree = tt.InterestFree,
                           IsStaff = tt.IsStaff ? "Yes" : "No",
                           IsDisabled = tt.IsDisabled ? "Yes" : "No",
                           Agreements = (new string[] 
                         {
                                                tt.IsRFAgreement ? "RFAgreement" : null,
                                                tt.IsHPAgreement ? "HPAgreement" : null,
                                                tt.IsStoreCardAgreement ? "StoreCardAgreement" : null
                                            }).Where(s => s != null).ToArray(),
                           CashLoanCustomerType = (new string[] 
                         {
                                                                tt.IsCashLoanExistingCustomer ? "ExistingCustomer" : null,
                                                                tt.IsCashLoanNewCustomer ? "LoanNewCustomer" : null,
                                                                tt.IsCashLoanRecentCustomer ? "RecentCustomer" : null,
                                                                tt.IsCashLoanStaffCustomer ? "Staff" : null
                                                            }).Where(s => s != null).ToArray(),
                           IsActive = tt.EndDate.HasValue ? "No" : "Yes",
                           CustomerTags = customerTags[tt.Id].Select(t => t.TagName).ToList(),
                           //CustomerBands = customerBands[tt.Id].Select(t => t.BandName).ToList().Distinct(),
                           ProductHierarchyTag = productHierarchyTag[tt.Id].Select(t => t.TagName).ToList(),
                           Fascias = fascia[tt.Id].Select(t => t.Fascia).ToList(),
                           FreeInstalments = freeInstalments[tt.Id].Select(t => t.FreeInstalmentInMonths).ToList(),
                           CreatedBy = tt.CreatedBy,
                           CreatedOn = tt.CreatedOn.ToSolrDate(),
                           TermsTypeId = tt.Id
                       }).ToList());
        }
    }
}