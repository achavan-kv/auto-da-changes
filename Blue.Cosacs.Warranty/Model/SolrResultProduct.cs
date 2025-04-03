using System;

namespace Blue.Cosacs.Warranty.Model
{
    public class Product
    {
        public string CostPrice { get; set; }
        public string TaxInclusivePrice { get; set; }
        public string ItemNoWarrantyLink { get; set; }
        public string StockBranchNameWarrantyLink { get; set; }
        public string StoreType { get; set; }
        public int StockBranchProduct { get; set; }
        public string Level_1 { get; set; }
        public string Level_2 { get; set; }
        public string Level_3 { get; set; }
    }

    public class SolrResultProduct
    {
        public SolrResponse response { get; set; }
    }

    public class SolrResponse
    {
        public Product[] docs { get; set; }
    }
}
