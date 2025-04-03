using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Merchandising.Models.RP3Export
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string SKUStatusCode { get; set; }
        public string SkuType { get; set; }
        public string ProductCode { get; set; }
        public string CorporateUPC { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string ClassCode { get; set; }
        public string ClassName { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
        public string SupplierModel { get; set; }
        public string Description { get; set; }
        public string CurrencyType { get; set; }
        public decimal AverageCost { get; set; }

        public decimal LastReceptionCost { get; set; }
        public decimal LastSupplierCost { get; set; }
        public decimal LowestReceptionCost { get; set; }

        public decimal? RetailPrice { get; set; }
        public DateTime? LastTransactionDate { get; set; }
        public DateTime? LastReceptionDate { get; set; }
        public DateTime? LastSalesDate { get; set; }
        public string ProductStatus { get; set; }
        public List<string> Tags { get; set; }
        public string ProductType { get; set; }
         public string PreviousProductType { get; set; }
        public string ReplacingTo { get; set; }
        public int? VendorWarranty { get; set; }
        public string CountryOfOrigin { get; set; }

        public string Incoterm { get; set; }

        public string CountryOfDispatch { get; set; }

        public string LeadTime { get; set; }

        public List<FieldSchema> Attributes { get; set; }
        public string SubjectTax { get; set; }
        public decimal TaxPercentage { get; set; }

        public string ProductAction { get; set; }

        public string User { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime? ExternalCreationDate { get; set; }
    }
}
