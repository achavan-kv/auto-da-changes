using System.Collections.Generic;

namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class ProductExportViewModel
    {
        public int? Id { get; set; }
        public string WarehouseNo { get; set; }
        public int ProductId { get; set; }
        public string ItemNo { get; set; }
        public string SupplierCode { get; set; }
        public string ItemDescr1 { get; set; }
        public string ItemDescr2 { get; set; }
        public decimal? UnitPriceHP { get; set; }
        public decimal? UnitPriceCash { get; set; }
        public string Category { get; set; }
        public string Supplier { get; set; }
        public string ProdStatus { get; set; }
        public string Warrantable { get; set; }
        public string ProdType { get; set; }
        public decimal? DutyFreePrice { get; set; }
        public string RefCode { get; set; }
        public string BarCode { get; set; }
        public string LeadTime { get; set; }
        public string WarrantyRenewalFlag { get; set; }
        public string AssemblyRequired { get; set; }
        public string Deleted { get; set; }
        public decimal? CostPrice { get; set; }
        public string SupplierName { get; set; }
        public string Class { get; set; }
        public string SubClass { get; set; }
        public decimal TaxRate { get; set; }  
        public string ProductType { get; set; }
        public List<FieldSchema> Attributes { get; set; }
    }
}
