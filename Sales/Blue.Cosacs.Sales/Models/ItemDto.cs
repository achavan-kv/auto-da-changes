using System;
using System.Collections.Generic;

namespace Blue.Cosacs.Sales.Models
{
    [Serializable]
    public class ItemDto
    {
        // Inheritance will create many problems
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int OrderId { get; set; }
        public byte ItemTypeId { get; set; }
        public string ItemNo { get; set; }
        public string Description { get; set; }
        public string PosDescription { get; set; }
        public short Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TaxRate { get; set; }
        public decimal? TaxAmount { get; set; }
        public byte? WarrantyLengthMonths { get; set; }
        public DateTime? WarrantyEffectiveDate { get; set; }
        public string WarrantyContractNo { get; set; }
        public decimal? ManualDiscount { get; set; }
        public string WarrantyTypeCode { get; set; }
        public string ManualDiscountPercentage { get; set; }
        public int? ProductItemId { get; set; }
        public int? WarrantyLinkId { get; set; }
        public bool? Returned { get; set; }
        public bool? IsClaimed { get; set; }
        public bool? IsReplacement { get; set; }
        public short? ReturnQuantity { get; set; }
        public int OriginalId { get; set; }
        public string SelectedDiscount { get; set; }
        public string ReturnReason { get; set; }
        public string ItemUPC { get; set; }
        public string ItemSupplier { get; set; }
        public string Department { get; set; }
        public short? Category { get; set; }
        public string Class { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public string ParentItemNo { get; set; }
        public decimal SalePrice { get; set; }
        public decimal SaleTaxAmount { get; set; }
        public bool IsFixedDiscount { get; set; }
        public List<ItemDto> Installations { get; set; }
        public List<ItemDto> Warranties { get; set; }
        public List<ItemDto> PotentialWarranties { get; set; }
        public List<ItemDto> KitItems { get; set; }
        public List<DiscountDto> Discounts { get; set; }
    }
}