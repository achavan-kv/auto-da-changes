
namespace Blue.Cosacs.Warehouse
{
    public class EventCategory
    {
        public const string Index = "Index";
        public const string Warehouse = "Warehouse";
    }

    public class EventType
    {
        public const string FullIndex = "FullIndex";
        public const string DeliveryIndex = "DeliveryIndex";
        public const string PickingIndex = "PickingIndex";

        public const string DriverUpdate = "DriverUpdate";
        public const string DriverCreate = "DriverCreate";
        public const string DriverDelete = "DriverDelete";

        public const string TruckUpdate = "TruckUpdate";
        public const string TruckCreate = "TruckCreate";
        public const string TruckDelete = "TruckDelete";
        
        public const string DriverPaymentCreate = "DriverPaymentCreate";
        public const string DriverPaymentUpdate = "DriverPaymentUpdate";
        public const string DriverPaymentDelete = "DriverPaymentDelete";

        public const string DriverCommissionIndex = "DriverCommissionIndex";
        public const string DriverCommission = "DriverCommission";
        public const string DriverCommissionCreateExport = "DriverCommissionCreateExport";
        public const string DriverCommissionExport = "DriverCommissionExport";
        public const string PickingPicked = "PickingPicked";
        public const string PickingUnPicked = "PickingUnPicked";
        public const string CreatePickListAll = "CreatePickListAll";
        public const string CreatePickListByTruck = "CreatePickListByTruck";
        public const string CreatePickListByTrucks = "CreatePickListByTrucks";
        public const string CreatePickListByWarehouseZone = "CreatePickListByWarehouseZone";
        public const string CreatePickListByCategory = "CreatePickListByCategory";
        public const string ConfirmPickList = "ConfirmPickList";
        public const string AssignBookingToTruck = "AssignBookingToTruck";
     


        public const string PrintMultiplePickList = "PrintMultiplePickList";
        public const string RePrintMultiplePickList = "RePrintMultiplePickList";
        public const string PrintSchedule = "PrintSchedule";
        public const string PrintLoad = "PrintLoad";
        public const string CreateDeliverySchedule = "CreateDeliverySchedule";
        public const string ConfirmDeliverySchedule = "ConfirmDeliverySchedule";
        public const string DeliveryNotify = "DeliveryNotify";
        public const string ConfirmDeliveryShipment = "ConfirmDeliveryShipment";
        public const string Resolve = "Resolve";
        public const string Cancel = "Cancel";
        public const string ConfirmDelivery = "ConfirmDelivery";
        public const string PrintCustomerPickup = "PrintCustomerPickup";
        public const string RePrintCustomerPickup = "RePrintCustomerPickup";
        public const string TransferTruck = "TransferTruck";                            //#15228 - CR15170
    }
}
