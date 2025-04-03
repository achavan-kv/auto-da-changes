using System.Collections.Generic;

namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    public class Product
    {
        public Product()
        {
            this.Attributes = new List<FieldSchema>();
            this.Features = new List<FieldSchema>();
            this.StoreTypes = new List<string>();
            this.Tags = new List<string>();
            this.Suppliers = new List<KeyValuePair<int, string>>();
            this.Hierarchy = new Dictionary<string, string>();
            this.RetailPrices = new List<RetailPriceViewModel>();
            this.Promotions = new List<PromotionalPriceViewModel>();
            this.Incoterms = new List<Incoterm>();
            
            this.AshleySetUps = new List<AshleySetUp>();
            this.AshleySetUp = new AshleySetUp();
            this.ProductAttributes = new ProductAttribute();
            this.ProductStockRanges = new ProductStockRange();
            TaxRates = new List<TaxRateModel>();
        }
        public object[] GetParams(int id)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "ProductId", Value = id, Direction = ParameterDirection.Input });

            parameters.ToArray();
            return parameters.ToArray();
        }
        public DateTime? OnlineDateAdded { get; set; }

        public int User { get; set; }

        public string BrandName { get; set; }
        public string BrandCode { get; set; }

        public List<FieldSchema> Features { get; set; }

        public int? Id { get; set; }

        [Required]
        [StringLength(20)]
        public string SKU { get; set; }

        [Required]
        [StringLength(240)]
        [RegularExpression("^[^,]+$", ErrorMessage = "Please remove comma from field \"Long Description\"")]
        public string LongDescription { get; set; }

        [Required]
        [StringLength(240)]
        [RegularExpression("^[^,]+$", ErrorMessage = "Please remove comma from field \"POS Description\"")]
        public string POSDescription { get; set; }

        [Required]
        [StringLength(1)]
        public string SKUStatus { get; set; }

        [Required]
        [StringLength(20)]
        [RegularExpression("^[^,]+$", ErrorMessage = "Please remove comma from field \"Corporate Description\"")]
        public string CorporateUPC { get; set; }

        [StringLength(16, ErrorMessage = "Vendor UPC exceeds it maximum length i.e. 16 characters")]
        [RegularExpression("^[^,]+$", ErrorMessage = "Please remove comma from field \"Vendor UPC\"")]
        public string VendorUPC { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression("^[^,]+$", ErrorMessage = "Please remove comma from field \"Vendor Style Long\"")]
        public string VendorStyleLong { get; set; }

        [Required]
        [StringLength(2)]
        [RegularExpression("^[^,]+$", ErrorMessage = "Please remove comma from field \"Country Of Origin\"")]
        public string CountryOfOrigin { get; set; }

        public int? VendorWarranty { get; set; }

        [StringLength(20)]
        [RegularExpression("^[^,]+$", ErrorMessage = "Please remove comma from field \"Previous Model\"")]
        public string ReplacingTo { get; set; }

        [Required]
        public string ProductType { get; set; }
        [Required] public string PreviousProductType { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        public IEnumerable<ProductStockLocationView> StockLevel { get; set; }

        [ReadOnly(true)]
        public DateTime? CreatedDate { get; set; }

        [Required]
        public int? BrandId { get; set; }

        [Required]
        public int? PrimaryVendorId { get; set; }

        [ReadOnly(true)]
        public DateTime? LastUpdatedDate { get; set; }

        public List<FieldSchema> Attributes { get; set; }

        public List<string> StoreTypes { get; set; }

        public List<string> Tags { get; set; }

        public List<KeyValuePair<int, string>> Suppliers { get; set; }

        public IDictionary<string, string> Hierarchy { get; set; }

        [Required]
        public bool PriceTicket { get; set; }

        public IEnumerable<TaxRateModel> TaxRates { get; set; }

        public List<RetailPriceViewModel> RetailPrices { get; set; }

        public List<Incoterm> Incoterms { get; set; }

        public List<AshleySetUp> AshleySetUps { get; set; }
        public AshleySetUp AshleySetUp { get; set; }
        public ProductAttribute ProductAttributes { get; set; }
        public ProductStockRange ProductStockRanges { get; set; }
        public List<AdditionalCostPrice> AddtionalCPs { get; set; }

        public ProductSalesViewModel Sales { get; set; }

        public List<PromotionalPriceViewModel> Promotions { get; set; }

        public string MagentoExportType { get; set; }

        public DateTime? ExternalCreationDate { get; set; }

        public RetailPriceViewModel CurrentRetailPrice
        {
            get
            {
                return this.RetailPrices.Where(t => t.EffectiveDate.Date <= DateTime.Now.Date).OrderByDescending(t => t.EffectiveDate).FirstOrDefault();
            }
        }

        public TaxRateModel CurrentTaxRate
        {
            get
            {
                return this.TaxRates.Where(t => t.EffectiveDate.Date <= DateTime.Now.Date).OrderByDescending(t => t.EffectiveDate).FirstOrDefault();
            }
        }

        public CostPriceModel CostPrice { get; set; }

        public bool? LabelRequired { get; set; }

        [Range(1, int.MaxValue)]
        public int? BoxCount { get; set; }

        public string ProductAction { get; set; }

        //CR - Product warranty association need to populate based on warrantable status of product.
        public bool? Warrantable { get; set; }
    }
}