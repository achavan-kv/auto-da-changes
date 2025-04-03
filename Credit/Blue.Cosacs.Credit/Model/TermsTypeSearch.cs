using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Blue.Cosacs.Credit.Model
{
    public class TermsTypeSearch
    {
        [JsonProperty("customerType")]
        public string CustomerType { get; set; }

        [JsonProperty("termLength")]
        public int? TermLength { get; set; }

        [JsonProperty("date")]
        public DateTime? Date { get; set; }

        [JsonProperty("points")]
        public int? Points { get; set; }

        [JsonProperty("fascias")]
        public List<string> Fascias { get; set; }

        [JsonProperty("customerTags")]
        public List<string> CustomerTags { get; set; }

        [JsonProperty("productSkus")]
        public List<Model.ProductSKU> ProductSkus { get; set; }

        [JsonProperty("productHierarchyTags")]
        public List<Model.ProductHierarchyTag> ProductHierarchyTags { get; set; }

        [JsonProperty("isStaff")]
        public bool IsStaff { get; set; }

        [JsonProperty("isRfAgreement")]
        public bool IsRfAgreement { get; set; }

        [JsonProperty("IshpAgreement")]
        public bool IsHpAgreement { get; set; }

        [JsonProperty("StoreCardAgreement")]
        public bool IsStoreCardAgreement { get; set; }

        [JsonProperty("isCashLoanNewCustomer")]
        public bool IsCashLoanNewCustomer { get; set; }

        [JsonProperty("isCashLoanExistingCustomer")]
        public bool IsCashLoanExistingCustomer { get; set; }

        [JsonProperty("isCashLoanStaffCustomer")]
        public bool IsCashLoanStaffCustomer { get; set; }

        [JsonProperty("isCashLoanRecentCustomer")]
        public bool IsCashLoanRecentCustomer { get; set; }
    }
}
