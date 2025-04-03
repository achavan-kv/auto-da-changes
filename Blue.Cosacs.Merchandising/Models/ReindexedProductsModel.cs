
using System.Collections.Generic;


namespace Blue.Cosacs.Merchandising.Models
{
   public class ReIndexedStockSummaryModel
    {
        public string Id { get; set; }
        public int ProductId { get; set; }
        public string Sku { get; set; }
        public string LongDescription { get; set; }
        public string PosDescription { get; set; }
        public string ProductType { get; set; }
        public string PreviousProductType { get; set; }
        public string ProductStatus { get; set; }
        public string RepossessedCondition { get; set; }
        public string Type { get; set; }
        public IEnumerable<string> PriceData { get; set; }
        public IEnumerable<string> PromoData { get; set; }
        public string CreatedOn { get; set; }
        public int StockAvailable { get; set; }
        public int StockOnHand { get; set; }
        public int StockOnOrder { get; set; }
        public int StockAllocated { get; set; }
        public bool LabelRequired { get; set; }
        public int BranchesWithStock { get; set; }
        public List<string> Tags { get; set; }
        public string HierarchyTags { get; set; }
        public List<string> StoreTypes { get; set; }
        public List<string> Vendors { get; set; }
        public int SalesThisPeriod { get; set; }
        public int SalesLastPeriod { get; set; }
        public int SalesThisYTD { get; set; }
        public int SalesLastYTD { get; set; }
        public string CorporateUPC { get; set; }
        public string VendorUPC { get; set; }
        public List<HierarchyLevel> HierarchyLevel { get; set; }
        public string MerchandisingLevel_1 { get; set; }
        public string MerchandisingLevel_2 { get; set; }
        public string MerchandisingLevel_3 { get; set; }

    }

    public class ReIndexedStockLevelModel
    {
        public bool VirtualWarehouse { get; set; }
        public string Id { get; set; }
        public int ProductId { get; set; }
        public string Sku { get; set; }
        public string LongDescription { get; set; }       
        public string ProductType { get; set; }
        public string PreviousProductType { get; set; }
        public string ProductStatus { get; set; }
        public string RepossessedCondition { get; set; }
        public string LocationNumber { get; set; }
        public int LocationId { get; set; }
        public string Type { get; set; }
        public string LocationName { get; set; }
        public string Fascia { get; set; }
        public string CreatedOn { get; set; }
        public decimal AverageWeightedCost { get; set; }
        public int StockAvailable { get; set; }
        public int StockOnHand { get; set; }
        public int StockOnOrder { get; set; }
        public int StockAllocated { get; set; }
        public string Warehouse { get; set; }
        public List<string> Tags { get; set; }
        public List<string> StoreTypes { get; set; }
        public List<string> Vendors { get; set; }



    }
}
