
using System;
namespace Blue.Cosacs.Warehouse.Search
{
    public class Booking
    {
        public string Id { get { return String.Format("Booking:{0}", BookingNo); } }
        public string Type { get { return "Booking"; } }
        public int BookingNo { get; set; }
        public string Customer { get; set; }
        public string Address { get; set; }
        public string Account { get; set; }
        public short StockBranch { get; set; }
        public short BookingBranch { get; set; }
        public string DelCol { get; set; }
        public string DueOn { get; set; }
        public string ItemNo { get; set; }
        public int ItemId { get; set; }
        public string ItemUPC { get; set; }
        public string ItemDescription { get; set; }
        public string ProductCat { get; set; }
        public int ItemsCount { get; set; }
        public string CreatedOn { get; set; }
        public string Damaged { get; set; }
        public int? PickListNo { get; set; }
        public int? ScheduleNo { get; set; }
        public int? Truck { get; set; }
        public string BookingStatus { get; set; }
        public string StockBranchName { get; set; }
        public string DeliveryBranchName { get; set; }
        public int? OriginalId { get; set; }
        public string DeliveryZone { get; set; }
        public string Fascia { get; set; }
        public bool PickUp { get; set; }
        public string DeliveryOrCollection { get; set; }
        public string DeliveryOrCollectionSlot { get; set; }  //#1406
        //public decimal UnitPrice { get; set; } //#10783
    }
  
}

namespace Blue.Cosacs.Warehouse
{
    partial class Booking
    {
        public string Status { get; set; }
    }


}
