using System;

namespace Blue.Cosacs.Warranty.Model
{
    class WarrantyPotentialSaleModel
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string AccountNumber { get; set; }
        public short SaleBranch { get; set; }
        public DateTime? SoldOn { get; set; }
        public DateTime? DeliveredOn { get; set; }
        public SoldByUser SoldBy { get; set; }

        public string CustomerId { get; set; }

        public int? ItemId { get; set; }
        public string ItemNumber { get; set; }
        public decimal? ItemPrice { get; set; }
        public decimal? ItemCostPrice { get; set; }
        public ItemWarranty Warranty { get; set; }
        public bool IsItemReturned { get; set; }

        public bool IsSecondEffort { get; set; }
        public bool SecondEffort { get; set;}
        public int Quantity { get; set; }

        public class SoldByUser
        {
            public int? SoldById { get; set; }
        }

        public class ItemWarranty
        {
            public int? WarrantyId { get; set; }
            public string WarrantyNumber { get; set; }
            public short? WarrantyLength { get; set; }
            public decimal? WarrantyTaxRate { get; set; }
            public string WarrantyType { get; set; }
            public decimal WarrantyCostPrice { get; set; }
            public decimal WarrantyRetailPrice { get; set; }
            public decimal WarrantySalePrice { get; set; }
        }
    }
}
