using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Stock.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ItemNo { get; set; }
        public short StockLocation { get; set; }
        public string SupplierCode { get; set; }
        public string ItemDescription { get; set; }
        public string ItemDescription2 { get; set; }
        public decimal? UnitPriceHP { get; set; }
        public decimal? UnitPriceCash { get; set; }
        public double TaxRate { get; set; }
        public double StockQuantity { get; set; }
        public double StockQuantityActual { get; set; }
        public double StockQuantityOnOrder { get; set; }
        public double StockDamage { get; set; }
        public DateTime? StockLastPlannedDate { get; set; }
        public double StockFactAvailable { get; set; }
        public short? Category { get; set; }
        public string Supplier { get; set; }
        public string ProductStatus { get; set; }
        public short? Warrantable { get; set; }
        public string ItemType { get; set; }
        public decimal? UnitPriceDutyFree { get; set; }
        public string ReferenceCode { get; set; }
        public string WarrantyRenewalFlag { get; set; }
        public short LeadTime { get; set; }
        public string AssemblyRequired { get; set; }
        public string Deleted { get; set; }
        public decimal? CostPrice { get; set; }
        public string SupplierName { get; set; }
        public DateTime? DateActivated { get; set; }
        public string IUPC { get; set; }
        public string ColourName { get; set; }
        public string ColourCode { get; set; }
        public string VendorStyle { get; set; }
        public string VendorLongStyle { get; set; }
        public int ItemID { get; set; }
        public string SKU { get; set; }
        public string VendorEAN { get; set; }
        public bool RepossessedItem { get; set; }
        public short? VendorWarranty { get; set; }
        public bool SparePart { get; set; }
        public string Class { get; set; }
        public string SubClass { get; set; }
        public string Brand { get; set; }
        public string Department { get; set; }
    }
}
