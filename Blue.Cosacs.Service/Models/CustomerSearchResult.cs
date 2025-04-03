using System;

namespace Blue.Cosacs.Service.Models
{
    public class CustomerSearchResult
    {

        public class Contact
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }

        public class ChargeItem
        {
            public string CustomerId { get; set; }
            public string Label { get; set; }
            public string ChargeType { get; set; }
            public string ItemNo { get; set; }
            public string Account { get; set; }
            public decimal Value { get; set; }
            public decimal? Tax { get; set; }
            public int RequestId { get; set; }
        }

        public class HistoryItem
        {
            public DateTime CreatedOn { get; set; }
            public DateTime? UpdatedOn { get; set; }
            public int RequestId { get; set; }
            public string Status { get; set; }
            public decimal? RepairTotal { get; set; }
            public string SerialNumber { get; set; }
        }

        public class Addresses
        {
        
            public string code { get; set; }
            public string category { get; set; }
            public string codedescript { get; set; }
            public string CustomerAddressLine1 { get; set; }
            public string CustomerAddressLine2 { get; set; }
            public string CustomerAddressLine3 { get; set; }
            public string CustomerPostcode { get; set; }
            public string CustomerNotes { get; set; }
            public string addtype { get; set; }

        }
        public string Account { get; set; }
        public string addtype { get; set; }
        
        public string CustomerId { get; set; }
        public string CustomerTitle { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerAddressLine1 { get; set; }
        public string CustomerAddressLine2 { get; set; }
        public string CustomerAddressLine3 { get; set; }
        public string CustomerPostcode { get; set; }
        public string CustomerNotes { get; set; }
        
        public int ItemId { get; set; }
        public string ItemNumber { get; set; }
        public string Iupc { get; set; }
        public decimal ItemAmount { get; set; }
        public decimal ItemCostPrice { get; set; }
        public DateTime ItemSoldOn { get; set; }
        public int ItemSoldBy { get; set; }
        public string ItemSoldByName { get; set; }
        public DateTime ItemDeliveredOn { get; set; }
        public short? ItemStockLocation { get; set; }
        public string ItemStockLocationName { get; set; }
        public string Item { get; set; }
        public string ItemSupplier { get; set; }
        public string ItemInvoiceNo { get; set; }
        public string ProductLevel_1 { get; set; }
        public string ProductLevel_2 { get; set; }
        public string ProductLevel_3 { get; set; }
        public string ItemSerialNumber { get; set; }
        public int? WarrantyGroupId { get; set; } 
        public string WarrantyNumber { get; set; }
        public string WarrantyType { get; set; }
        public string WarrantyContractNumber { get; set; }
        public int? WarrantyContractId { get; set; }
        public int? WarrantyLength { get; set; }
        public CustomerSearchResult.Contact[] Contacts { get; set; }
        public string ManufacturerWarrantyNumber { get; set; }
        public string ManufacturerWarrantyContractNumber { get; set; }
        public int ManufacturerWarrantyLength { get; set; }
        public CustomerSearchResult.ChargeItem[] HistoryCharges { get; set; }
        public CustomerSearchResult.HistoryItem[] History { get; set; }
        public CustomerSearchResult.Addresses[] Address { get; set; }
        public int TotalRequests { get; set; }
        public int TotalItemCount { get; set; }

        public string Quantity
        {
            get
            {
                var remainingItems = TotalItemCount - TotalRequests;
                return string.Format("{0} of {1}", remainingItems, TotalItemCount);
            }
        }
    }
}
