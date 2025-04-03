using System;

namespace Blue.Cosacs.Warranty.Model
{
    public class WarrantySaleOrder
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        //public int LineItemIdentifier { get; set; }
        public int WarrantyGroupId { get; set; } 
        public short SaleBranch { get; set; }
        public DateTime? SoldOn { get; set; }
        public DateTime? DeliveredOn { get; set; }
        public SoldByUser SoldBy { get; set; }
        public string CustomerAccount { get; set; }
        public string CustomerId { get; set; }
        public string CustomerTitle { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerAddressLine1 { get; set; }
        public string CustomerAddressLine2 { get; set; }
        public string CustomerAddressLine3 { get; set; }
        public string CustomerPostcode { get; set; }
        public string CustomerNotes { get; set; }
        public int? ItemId { get; set; }
        public string ItemNumber { get; set; }
        public string ItemUPC { get; set; }
        public decimal? ItemPrice { get; set; }
        public decimal? ItemCostPrice { get; set; }
        public string ItemDescription { get; set; }
        public string ItemBrand { get; set; }
        public string ItemModel { get; set; }
        public string ItemSupplier { get; set; }
        public short? ItemStockLocation { get; set; }
        public int? ItemQuantity { get; set; }
        public ItemWarranty[] Warranty { get; set; }
        public ContactItem[] Contacts { get; set; }

        public class ContactItem
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }

        public class SoldByUser
        {
            public int? SoldById { get; set; }
            public string Value { get; set; }
        }

        public class ItemWarranty
        {
            public string WarrantyContractNo { get; set; }
            public int? WarrantyItemId { get; set; }
            public string WarrantyNumber { get; set; }
            public short? WarrantyLength { get; set; }
            public decimal? WarrantyTaxRate { get; set; }
            public string WarrantyType { get; set; }
            public decimal? WarrantyCostPrice { get; set; }
            public decimal? WarrantyRetailPrice { get; set; }
            public decimal? WarrantySalePrice { get; set; }
            public string WarrantyStatus { get; set; }
            public int? WarrantyGroupId {get; set;} 
            public DateTime? WarrantyEffectiveDate { get; set; } 
            public string RedeemContractNo { get; set; } 
            public string ReLinkContractNo { get; set; }
            public DateTime? WarrantyDeliveredOn { get; set; } 
        }
    }
}
