using FileHelpers;

namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    [DelimitedRecord(",")]
    public class ProductExportModel
    {
        public string WarehouseNo { get; set; }
        public string ItemNo { get; set; }
        public string SupplierCode { get; set; }
        public string ItemDescr1 { get; set; }
        public string ItemDescr2 { get; set; }
        public string UnitPriceHP { get; set; }
        public string UnitPriceCash { get; set; }
        public string Category { get; set; }
        public string Supplier { get; set; }
        public string ProdStatus { get; set; }
        public string Warrantable { get; set; }
        public string ProdType { get; set; }
        public string DutyFreePrice { get; set; }
        public string RefCode { get; set; }
        public string BarCode { get; set; }
        public string LeadTime { get; set; }
        public string WarrantyRenewalFlag { get; set; }
        public string AssemblyRequired { get; set; }
        public string Deleted { get; set; }
        public string CostPrice { get; set; }
        public string SupplierName { get; set; }
        public string Class { get; set; }
        public string SubClass { get; set; }
        public string TaxRate { get; set; }
    }
}
