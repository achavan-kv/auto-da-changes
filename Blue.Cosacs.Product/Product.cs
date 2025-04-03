using System;
namespace Blue.Cosacs.Stock
{
    public class WarrantyProductLinkSearch
    {
        /// <summary>
        /// Henrry comment. WTF is this sh... why price is string?
        /// </summary>
        public string CashPrice { get; set; }
        public string CostPrice { get; set; }
        public string ItemNoWarrantyLink { get; set; }
        public string RefCodeWarrantyLink { get; set; }
        public string Level_1 { get; set; }
        public string Level_2 { get; set; }
        public string Level_3 { get; set; }
        public string StockBranchNameWarrantyLink { get; set; }
        public int? StockBranchProduct { get; set; }
        public string StoreType { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        public bool Level1IsValid()
        {
            return IsValid(Level_1);
        }

        public bool Level2IsValid()
        {
            return IsValid(Level_2);
        }

        public bool Level3IsValid()
        {
            return IsValid(Level_3);
        }

        public bool StoreTypeIsValid()
        {
            return IsValid(StoreType);
        }

        public bool StockBranchIsValid()
        {
            return isValid(StockBranchProduct);
        }

        public bool RefCodeIsValid()
        {
            return IsValid(RefCodeWarrantyLink);
        }

        public bool ItemNoIsValid()
        {
            return IsValid(ItemNoWarrantyLink);
        }

        private bool IsValid(string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        private bool isValid(int? num)
        {
            return num.HasValue;
        }
    }
}