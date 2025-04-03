namespace Blue.Cosacs.Service
{
    public class EventCategory
    {
        public const string Index = "Index";
        public const string Service = "Service";
        public const string TechnicianDiary = "TechnicianDiary";
        public const string TechnicianPayments = "TechnicianPayments";
        public const string AddressMaster = "AddressMaster"; // Address Standardization CR2019 - 025
    }

    public class EventType
    {
        public const string SavePayment = "SavePayment";
        public const string ServiceIndex = "ServiceIndex";
        public const string CreateRequest = "CreateRequest";
        public const string UpdateRequest = "UpdateRequest";
        public const string Allocation = "Allocation";
        public const string PrintFoodLoss = "PrintFoodLoss";
        public const string PrintServiceRequest = "PrintServiceRequest";
        public const string RequestBatchPrint = "RequestBatchPrint";
        public const string SummaryPrint = "SummaryPrint";
        public const string SaveCostMatrix = "SaveCostMatrix";
        public const string DeleteBooking = "DeleteBooking";
        public const string RejectBooking = "RejectBooking";
        public const string ServiceSupplierCreate = "ServiceSupplierCreate";
        public const string ServiceSupplierEdit = "ServiceSupplierEdit";
        public const string ServiceSupplierDelete = "ServiceSupplierDelete";
        public const string AssignBooking = "AssignBooking";
        public const string DeleteAvailability = "DeleteAvailability";
        public const string AddAvailability = "AddAvailability";
        public const string ExportTechnicianPayments = "ExportTechnicianPayments";
        public const string PrintTechnicianPayments = "PrintTechnicianPayments";

        public const string ResolutionUpdate = "ResolutionUpdate";
        public const string ResolutionCreate = "ResolutionCreate";
        public const string ResolutionDelete = "ResolutionDelete";

        public const string Pay = "Pay";
        public const string Hold = "Hold";
        public const string UnHold = "UnHold";
        public const string Delete = "Delete";

        public const string AddressMasterUpdate = "AddressMasterUpdate"; // Address Standardization CR2019 - 025
        public const string AddressMasterCreate = "AddressMasterCreate"; // Address Standardization CR2019 - 025
        public const string AddressMasterDelete = "AddressMasterDelete"; // Address Standardization CR2019 - 025
    }
}
