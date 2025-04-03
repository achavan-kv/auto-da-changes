using System;
using System.Collections.Generic;

namespace Blue.Cosacs.Warranty.Model
{
    public class WarrantyLinkProduct
    {
        public string Level_1 { get; set; }
        public string Level_2 { get; set; }
        public string Level_3 { get; set; }
        public IDictionary<string, string> Levels { get; set; }
        public string StoreType { get; set; }
        public string RefCodeWarrantyLink { get; set; }
        public int? StockBranchNameWarrantyLink { get; set; }
        public string ItemNoWarrantyLink { get; set; }
        public int LinkId { get; set; }
        public int ProductId { get; set; }
    }

    public class WarrantyLinkWarranty
    {
        public int WarrantyId { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public int LinkId { get; set; }
        public string WarrantyName { get; set; }
        public string WarrantyDescription { get; set; }
        public bool Deleted { get; set; }
        public string WarrantyType { get; set; }
    }

    public class WarrantyLink
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EffectiveDate { get; set; }
        public WarrantyLinkProduct[] products { get; set; }
        public WarrantyLinkWarranty[] warranties { get; set; }
    }

}