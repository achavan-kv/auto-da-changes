using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Blue.Cosacs.Credit.Model
{
    public class TermsTypeSimulator
    {
        [JsonProperty("TermsType")]
        public string Name { get; set; }

        public int? DepositRequiredPercentage { get; set; }

        public int MinTermLength { get; set; }
        public int MaxTermLength { get; set; }
        public int DefaultTermLength { get; set; }

        public int? InterestRateHolidays { get; set; }
        public int? FullRebate { get; set; }

        public bool HasPaymentHolidays { get; set; }
        public int InterestRatePercentage { get; set; }
        public int CpiPercentage { get; set; }

        public string CustomerType { get; set; }
        public int? InterestFree { get; set; }
        public bool IsStaff { get; set; }

        public bool IsDisabled { get; set; }

        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }

        public bool IsRFAgreement { get; set; }
        public bool IsHPAgreement { get; set; }
        public bool IsStoreCardAgreement { get; set; }

        public bool IsCashLoanNewCustomer { get; set; }
        public bool IsCashLoanRecentCustomer { get; set; }
        public bool IsCashLoanExistingCustomer { get; set; }
        public bool IsCashLoanStaffCustomer { get; set; }
        public DateTime? EndDate { get; set; }

        public List<ProductHierarchyTagSimulator> ProductHierarchyTags { get; set; }
        public List<string> FreeInstalments { get; set; }
        public List<string> Fascias { get; set; }
        public List<CustomerBandSimulator> CustomerBands { get; set; }
        public List<string> CustomerTags { get; set; }
        public List<ProductSKUSimulator> ProductSkus { get; set; }

        public TermsTypeSimulator()
        {
        }

        internal TermsTypeSimulator(Credit.TermsTypeDetails termsType)
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

        internal TermsTypeSimulator(
            Credit.TermsTypeDetails termsType,
            string name,
            List<ProductHierarchyTagSimulator> productHierarchyTags,
            List<string> freeInstalments,
            List<string> fascias,
            List<CustomerBandSimulator> customerBands,
            List<string> customerTags,
            List<ProductSKUSimulator> productSkus)
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
    }

    public class CustomerBandSimulator
    {
        public string BandName { get; set; }
        public int InterestRatePercentage { get; set; }
        public int DepositRequiredPercentage { get; set; }

        public int CpiPercentage { get; set; }
        public int AdminPercentage { get; set; }
        public int AdminValue { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class ProductHierarchyTagSimulator
    {
        public string Tag { get; set; }
        public string Level { get; set; }
    }

    public class ProductSKUSimulator
    {
        public string SkuId { get; set; }
        public string SkuDescription { get; set; }
    }
}
