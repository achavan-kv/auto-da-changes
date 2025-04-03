using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Blue.Cosacs.Service.Models
{
    // Names map directy to Js.

    public class LastUpdated
    {
        public int? LastUpdatedUser { get; set; }
        public string LastUpdatedUserName { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
    }

    public class StockViewResult
    {
        public string ItemNumber { get; set; }
        public short Location { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public double StockOnHand { get; set; }
        public decimal? CashPrice { get; set; }
        public double TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal CostPrice { get; set; }
        public string Supplier { get; set; }
        public short WarrantyLength { get; set; }
        public string deleted { get; set; }
        public bool SparePart { get; set; }
        public string LocationName { get; set; }
    }

    public class RequestItem
    {
        public class ChargeItem
        {
            public string CustomerId { get; set; }
            public string Label { get; set; }
            public string ChargeType { get; set; }
            public string ItemNo { get; set; }
            public string Account { get; set; }
            public decimal Cost { get; set; }
            public decimal Value { get; set; }
            public decimal? Tax { get; set; }
            public decimal TaxRate { get; set; }
            public int RequestId { get; set; }
            public bool IsExternal { get; set; }
        }

        public class HistoryItem
        {
            public DateTime CreatedOn { get; set; }
            public DateTime? UpdatedOn { get; set; }
            public int RequestId { get; set; }
            public string Status { get; set; }
            public decimal? RepairTotal { get; set; }
        }

        public class ContactItem
        {
            public string Type { get; set; }
            public string Value { get; set; }
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



        }

        public class FoodLossItem
        {
            public string item { get; set; }
            public decimal value { get; set; }
        }

        public class PartItem
        {
            public string number { get; set; }
            public string type { get; set; }
            public short quantity { get; set; }
            public decimal price { get; set; }
            public decimal CostPrice { get; set; }
            public decimal? CashPrice { get; set; }
            public decimal TaxAmount { get; set; }
            public decimal TaxRate { get; set; }
            public string description { get; set; }
            public int? stockbranch { get; set; }
            public string Source { get; set; }
        }

        public class ScriptAnswerItem
        {
            public string Question { get; set; }
            public string Value { get; set; }

        }

        public class RequestComment
        {
            public DateTime Date { get; set; }
            public string AddedBy { get; set; }
            public string Comment { get; set; }
        }

        public class FaultTag
        {
            public string Tag { get; set; }
        }

        public class RequestStockItem
        {
            public decimal? CostPrice { get; set; }
        }

        public int Id { get; set; }
        public string Account { get; set; }
        public DateTime CreatedOn { get; set; }
        public Int32 CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public short Branch { get; set; }
        public string BranchName { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerId { get; set; }
        public string CustomerTitle { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerAddressLine1 { get; set; }
        public string addtype { get; set; }
        public string CustomerAddressLine2 { get; set; }
        public string CustomerAddressLine3 { get; set; }
        public string CustomerPostcode { get; set; }
        public string CustomerNotes { get; set; }
        public string DELTitleC { get; set; }
        public string DELFirstName { get; set; }
        public string DELLastName { get; set; }
        public string ItemId { get; set; }
        [IgnoreDataMember]
        public int? ItemIdNumber
        {
            get
            {
                var testValue = 0;

                if (!int.TryParse(this.ItemId, out testValue))
                {
                    return null;
                }

                return testValue;
            }
        }
        public string ItemNumber { get; set; }
        public decimal? ItemAmount { get; set; }
        public string ItemSoldBy { get; set; }
        public DateTime? ItemDeliveredOn { get; set; }
        public int? ItemStockLocation { get; set; }
        public string Item { get; set; }
        public string ItemSupplier { get; set; }
        public string ProductLevel_1 { get; set; }
        public short? ProductLevel_2 { get; set; }
        public string ProductLevel_3 { get; set; }
        public string Manufacturer { get; set; }
        public string ItemSerialNumber { get; set; }
        public string WarrantyGroupId { get; set; }
        public string WarrantyNumber { get; set; }
        public int? WarrantyLength { get; set; }
        public string WarrantyContractNo { get; set; }
        public int? WarrantyContractId { get; set; }
        public int? ManufacturerWarrantyLength { get; set; }
        public string ManufacturerWarrantyNumber { get; set; }
        public string ManufacturerWarrantyContractNo { get; set; }
        public string TransitNotes { get; set; }
        public string Evaluation { get; set; }
        public bool? EvaluationClaimFoodLoss { get; set; }
        public string EvaluationLocation { get; set; }
        public string EvaluationAction { get; set; }
        public DateTime? AllocationItemReceivedOn { get; set; }
        public DateTime? AllocationPartExpectOn { get; set; }
        public int? AllocationTechnician { get; set; }
        public string AllocationTechnicianName { get; set; }
        public bool AllocationTechnicianIsInternal { get; set; }
        public string AllocationInstructions { get; set; }
        public DateTime? AllocationServiceScheduledOn { get; set; }
        public DateTime? AllocationOn { get; set; }
        public int AllocationSlots { get; set; }
        public int AllocationSlotExtend { get; set; }
        public string Resolution { get; set; }
        public DateTime? ResolutionDate { get; set; }
        public string RepairType { get; set; }
        public string ResolutionSupplierToCharge { get; set; }
        public string ResolutionPrimaryCharge { get; set; }
        public string ResolutionCategory { get; set; }
        public string ResolutionReport { get; set; }
        public string ResolutionDelivererToCharge { get; set; }
        public decimal? ResolutionLabourCost { get; set; }
        public decimal? ResolutionAdditionalCost { get; set; }
        public decimal? ResolutionTransportCost { get; set; }
        public string FinalisedFailure { get; set; }
        public DateTime? FinaliseReturnDate { get; set; }
        public string Comment { get; set; }
        public string ItemModelNumber { get; set; }
        public bool IsClosed { get; set; }
        public bool? IsPaymentRequired { get; set; }
        public decimal? PaymentBalance { get; set; }
        public decimal? RepairTotal { get; set; }
        public bool? ReplacementIssued { get; set; }
        public string ReasonForExchange { get; set; }
        public string Retailer { get; set; }
        public bool DepositAuthorised { get; set; }
        public decimal? DepositRequired { get; set; }
        public bool RepairLimitWarning { get; set; }

        public string TechnicianBookingDeleteReason { get; set; }
        public int TechnicianBookingDeleteId { get; set; }

        public string SlotStartTime { get; set; }
        public string SlotEndTime { get; set; }

        // CR2018-008 by tosif ali 15/10/2018*@
        public RequestItem.Addresses[] Address { get; set; }
        //End hear
        public RequestItem.ContactItem[] Contacts { get; set; }
        public RequestItem.FoodLossItem[] FoodLoss { get; set; }
        public RequestItem.PartItem[] Parts { get; set; }
        public RequestItem.ScriptAnswerItem[] ScriptAnswer { get; set; }
        public RequestItem.RequestComment[] RequestComments { get; set; }
        public RequestItem.HistoryItem[] History { get; set; }
        public RequestItem.ChargeItem[] Charges { get; set; }
        public RequestItem.ChargeItem[] HistoryCharges { get; set; }
        public RequestItem.FaultTag[] FaultTags { get; set; }
        public RequestItem.RequestStockItem StockItem { get; set; }
    }
}
