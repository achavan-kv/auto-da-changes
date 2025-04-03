namespace Blue.Cosacs.Merchandising.Models.RP3Export
{
    using System.Diagnostics.CodeAnalysis;

    using Blue.Cosacs.Merchandising.Helpers;

    using FileHelpers;

    [DelimitedRecord(",")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Required for export")]
    public class ProductExportModel
    {
        public ProductExportModel()
        {
            // Fields that we aren't currently sending values for
            this.ExchangeRate = 0;
            this.HighestPrice = 0;
            this.VendorStorage = string.Empty;
            this.Consignment = string.Empty;
            this.HandlesSerial = string.Empty;
            this.ExemptSales = string.Empty;
            this.ExemptPurchase = string.Empty;
            this.LandTime = 0;
            this.MimimumDisplay = 0;
            this.MasterPack = 0;
            this.MOQ = 0;
            this.HarmonizeTariffCode = 0;
            this.Percentage = 0;
        }

        [FieldQuoted]
        [FieldTitle]
        public string Company;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string ProductAction;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string User;

        [FieldQuoted]
        [FieldTitle]
        public string CreationDate;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string SKUStatusCode;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string SkuType;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string ProductCode;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string CorporateUPC;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string DivisionCode;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string DivisionName;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string DepartmentCode;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string DepartmentName;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string ClassCode;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string ClassName;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string VendorCode;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string VendorName;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string BrandCode;

        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldQuoted]
        [FieldTitle]
        public string BrandName;

        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldQuoted]
        [FieldTitle]
        public string SupplierModel;

        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldQuoted]
        [FieldTitle]
        public string Description;

        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldQuoted]
        [FieldTitle]
        public string CurrencyType;

        [FieldQuoted]
        [FieldTitle]
        public int ExchangeRate;

        [FieldQuoted]
        [FieldTitle]
        public decimal AverageCost;

        [FieldQuoted]
        [FieldTitle]
        public decimal LastReceptionCost;

        [FieldQuoted]
        [FieldTitle]
        public decimal LastSupplierCost;

        [FieldQuoted]
        [FieldTitle]
        public decimal LowestReceptionCost;

        [FieldQuoted]
        [FieldTitle]
        public decimal RetailPrice;

        [FieldQuoted]
        [FieldTitle]
        public decimal HighestPrice;

        [FieldQuoted]
        [FieldTitle]
        public string LastTransactionDate;

        [FieldQuoted]
        [FieldTitle]
        public string LastReceptionDate;

        [FieldQuoted]
        [FieldTitle]
        public string LastSalesDate;

        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldQuoted]
        [FieldTitle]
        public string ProductStatus;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string NeverOut;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string ProductStrategy;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string ProductType;

        [FieldQuoted]
        [FieldTitle]
        public string VendorStorage;

        [FieldQuoted]
        [FieldTitle]
        public string Consignment;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string ReplacingTo;

        [FieldQuoted]
        [FieldTitle]
        public string HandlesSerial;

        [FieldQuoted]
        [FieldTitle]
        public string ExemptSales;

        [FieldQuoted]
        [FieldTitle]
        public string ExemptPurchase;

        [FieldQuoted]
        [FieldTitle]
        public int VendorWarranty;

        [FieldQuoted]
        [FieldTitle]
        public string WarrantySelling;

        [FieldQuoted]
        [FieldTitle]
        public string CountryOfOrigin;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string Incoterm;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string CountryOfDispatch;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string LeadTime;

        [FieldQuoted]
        [FieldTitle]
        public int LandTime;

        [FieldQuoted]
        [FieldTitle]
        public int MimimumDisplay;

        [FieldQuoted]
        [FieldTitle]
        public int MasterPack;

        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldQuoted]
        [FieldTitle]
        public string PackSize;

        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldQuoted]
        [FieldTitle]
        public string Voltage;

        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldQuoted]
        [FieldTitle]
        public string Hertz;

        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldQuoted]
        [FieldTitle]
        public string CubicFeet;

        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldQuoted]
        [FieldTitle("20Ft")]
        public string Attr20Ft;

        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldQuoted]
        [FieldTitle("40STD")]
        public string Attr40Std;

        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldQuoted]
        [FieldTitle("40HQ")]
        public string Attr40Hq;

        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldQuoted]
        [FieldTitle("45Ft")]
        public string Attr45Ft;

        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldQuoted]
        [FieldTitle("LCL")]
        public string AttrLCL;

        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldQuoted]
        [FieldTitle("RT53")]
        public string AttrRt53;

        [FieldQuoted]
        [FieldTitle]
        public int MOQ;

        [FieldQuoted]
        [FieldTitle]
        public int HarmonizeTariffCode;

        [FieldQuoted]
        [FieldTitle]
        public int Percentage;

        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldQuoted]
        [FieldTitle]
        public string SubjectTax;

        [FieldQuoted]
        [FieldTitle]
        public string TaxPercentage;
    }
}