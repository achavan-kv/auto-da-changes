using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Blue.Cosacs.Credit.Model
{
    public class TermsTypeDetails
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("termsTypeId")]
        public int TermsTypeId { get; set; }

        [JsonProperty("minTermLength")]
        public int MinTermLength { get; set; }
        [JsonProperty("maxTermLength")]
        public int MaxTermLength { get; set; }
        [JsonProperty("defaultTermLength")]
        public int DefaultTermLength { get; set; }

        [JsonProperty("interestRateHolidays")]
        public int? InterestRateHolidays { get; set; }
        [JsonProperty("fullRebate")]
        public int? FullRebate { get; set; }

        [JsonProperty("hasPaymentHolidays")]
        public bool HasPaymentHolidays { get; set; }

        [JsonProperty("customerType")]
        public string CustomerType { get; set; }
        [JsonProperty("interestFree")]
        public int? InterestFree { get; set; }
        [JsonProperty("isStaff")]
        public bool IsStaff { get; set; }

        [JsonProperty("isDisabled")]
        public bool IsDisabled { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }
        [JsonProperty("createdBy")]
        public int CreatedBy { get; set; }

        [JsonProperty("isRfAgreement")]
        public bool IsRFAgreement { get; set; }
        [JsonProperty("isHpAgreement")]
        public bool IsHPAgreement { get; set; }
        [JsonProperty("isStoreCardAgreement")]
        public bool IsStoreCardAgreement { get; set; }

        [JsonProperty("isCashLoanNewCustomer")]
        public bool IsCashLoanNewCustomer { get; set; }
        [JsonProperty("isCashLoanRecentCustomer")]
        public bool IsCashLoanRecentCustomer { get; set; }
        [JsonProperty("isCashLoanExistingCustomer")]
        public bool IsCashLoanExistingCustomer { get; set; }
        [JsonProperty("isCashLoanStaffCustomer")]
        public bool IsCashLoanStaffCustomer { get; set; }
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("productHierarchyTags")]
        public List<ProductHierarchyTag> ProductHierarchyTags { get; set; }
        [JsonProperty("freeInstalments")]
        public List<string> FreeInstalments { get; set; }
        [JsonProperty("fascias")]
        public List<string> Fascias { get; set; }
        [JsonProperty("customerBands")]
        public List<CustomerBand> CustomerBands { get; set; }
        [JsonProperty("customerTags")]
        public List<string> CustomerTags { get; set; }
        [JsonProperty("productSkus")]
        public List<ProductSKU> ProductSkus { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        public TermsTypeDetails()
        {
        }

        internal TermsTypeDetails(Credit.TermsTypeDetails termsType)
        {
            this.MinTermLength = termsType.MinTermLength;
            this.MaxTermLength = termsType.MaxTermLength;
            this.DefaultTermLength = termsType.DefaultTermLength;
            this.InterestRateHolidays = termsType.InterestRateHolidays;
            this.FullRebate = termsType.FullRebate;
            this.HasPaymentHolidays = termsType.HasPaymentHolidays;
            this.CustomerType = termsType.CustomerType;
            this.InterestFree = termsType.InterestFree;
            this.IsStaff = termsType.IsStaff;
            this.IsDisabled = termsType.IsDisabled;
            this.CreatedOn = termsType.CreatedOn;
            this.CreatedBy = termsType.CreatedBy;
            this.IsHPAgreement = termsType.IsHPAgreement;
            this.IsRFAgreement = termsType.IsRFAgreement;
            this.IsStoreCardAgreement = termsType.IsStoreCardAgreement;
            this.IsCashLoanExistingCustomer = termsType.IsCashLoanExistingCustomer;
            this.IsCashLoanNewCustomer = termsType.IsCashLoanNewCustomer;
            this.IsCashLoanRecentCustomer = termsType.IsCashLoanRecentCustomer;
            this.IsCashLoanStaffCustomer = termsType.IsCashLoanStaffCustomer;
            this.EndDate = termsType.EndDate;
        }

        internal TermsTypeDetails(
            Credit.TermsTypeDetails termsType, 
            string name, 
            List<ProductHierarchyTag> productHierarchyTags, 
            List<string> freeInstalments, 
            List<string> fascias,
            List<CustomerBand> customerBands, 
            List<string> customerTags, 
            List<ProductSKU> productSkus)
            : this(termsType)
        {
            this.ProductHierarchyTags = productHierarchyTags;
            this.Name = name;
            this.FreeInstalments = freeInstalments;
            this.Fascias = fascias;
            this.CustomerBands = customerBands;
            this.CustomerTags = customerTags;
            this.ProductSkus = productSkus;
        }

        public Credit.TermsTypeDetails ToTable(DateTime now)
        {
            return new Credit.TermsTypeDetails()
            {
                MinTermLength = this.MinTermLength,
                MaxTermLength = this.MaxTermLength,
                DefaultTermLength = this.DefaultTermLength,
                InterestRateHolidays = this.InterestRateHolidays,
                FullRebate = this.FullRebate,
                HasPaymentHolidays = this.HasPaymentHolidays,
                CustomerType = this.CustomerType,
                InterestFree = this.InterestFree,
                IsStaff = this.IsStaff,
                IsDisabled = this.IsDisabled,
                CreatedOn = this.CreatedOn,
                CreatedBy = this.CreatedBy,
                IsRFAgreement = this.IsRFAgreement,
                IsHPAgreement = this.IsHPAgreement,
                IsStoreCardAgreement = this.IsStoreCardAgreement,
                IsCashLoanNewCustomer = this.IsCashLoanNewCustomer,
                IsCashLoanExistingCustomer = this.IsCashLoanExistingCustomer,
                IsCashLoanRecentCustomer = this.IsCashLoanRecentCustomer,
                IsCashLoanStaffCustomer = this.IsCashLoanStaffCustomer,
                EndDate = this.EndDate
            };
        }
    }

    public class CustomerBand
    {
        [JsonProperty("name")]
        public string BandName { get; set; }
        [JsonProperty("interestRatePercentage")]
        public int InterestRatePercentage { get; set; }
        [JsonProperty("depositRequiredPercentage")]
        public int DepositRequiredPercentage { get; set; }

        [JsonProperty("cpiPercentage")]
        public int CpiPercentage { get; set; }
        [JsonProperty("adminPercentage")]
        public int AdminPercentage { get; set; }
        [JsonProperty("adminValue")]
        public int AdminValue { get; set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }
        [JsonProperty("updatedBy")]
        public int UpdatedBy { get; set; }

        [JsonProperty("updatedOn")]
        public DateTime UpdatedOn { get; set; }
        [JsonProperty("pointsFrom")]
        public int PointsFrom { get; set; }
        [JsonProperty("pointsTo")]
        public int PointsTo { get; set; }
    }

    public class ProductHierarchyTag
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }
        [JsonProperty("level")]
        public string Level { get; set; }
    }

    public class ProductSKU
    {
        [JsonProperty("skuId")]
        public string SkuId { get; set; }
        [JsonProperty("description")]
        public string SkuDescription { get; set; }
    }
}
