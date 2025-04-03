using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Merchandising.Models.Ashley
{
    public class LineItemDetails
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public int ItemId { get; set; }
        public short Category { get; set; }
        public short Location { get; set; }
        public decimal AvailableStock { get; set; }
        public decimal DamagedStock { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string SupplierCode { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CashPrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal HPPrice { get; set; }
        public decimal DutyFreePrice { get; set; }
        public bool ValueControlled { get; set; }
        public decimal Quantity { get; set; }
        public string DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public short BranchForDeliveryNote { get; set; }
        public string ColourTrim { get; set; }
        public decimal TaxRate { get; set; }
        public decimal DeliveredQuantity { get; set; }
        public string PlannedDeliveryDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryArea { get; set; }
        public string DeliveryProcess { get; set; }
        public string DateDelivered { get; set; }
        public string QuantityDiff { get; set; }
        public decimal ScheduledQuantity { get; set; }
        public decimal TaxAmount { get; set; }
        public string ContractNo { get; set; }
        public string ParentItemNo { get; set; }
        public int ParentItemId { get; set; }
        public bool RepoItem { get; set; }
        public string ReturnItemNo { get; set; }
        public short ReturnLocation { get; set; }
        public bool FreeGift { get; set; }
        public string ExpectedReturnDate { get; set; }
        public decimal QtyOnOrder { get; set; }
        public bool PurchaseOrder { get; set; }
        public short LeadTime { get; set; }
        public string Damaged { get; set; }
        public string Assembly { get; set; }
        public string ProductCategory { get; set; }
        public string Deleted { get; set; }
        public string Class { get; set; }
        public string SubClass { get; set; }

        //public void AddRelatedItem(LineItemNode li) { get; set; }
        //public void AddServiceRequests(LineItemNode li) { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public bool ReplacementItem { get; set; }
        public bool SPIFFItem { get; set; }
        public bool IsInsurance { get; set; }
        public string RefCode { get; set; }
        public string VanNo { get; set; }
        public DateTime DhlInterfaceDate { get; set; }
        public DateTime DhlPickingDate { get; set; }
        public string DhlDNNo { get; set; }
        public string ShipQty { get; set; }
        public string SortOrder { get; set; }
        public bool ItemRejected { get; set; }
        public string ModelNumber { get; set; }
        public short? SalesBrnNo { get; set; }
        public string Brand { get; set; }
        public string Style { get; set; }
        public string Express { get; set; }
        public int LineItemId { get; set; }
        public bool ReadyAssist { get; set; }
        public string WarrantyType { get; set; }
        public decimal AdditionalTaxRate { get; set; }

    }
}