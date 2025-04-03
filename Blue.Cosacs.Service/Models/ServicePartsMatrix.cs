
namespace Blue.Cosacs.Service.Models
{
    public class ServicePartsMatrix
    {
        public string Level_1 { get; set; }
        public string Level_2 { get; set; }
        public string Level_3 { get; set; }
        public string Level_4 { get; set; }
        public string Level_5 { get; set; }
        public string Level_6 { get; set; }
        public string Level_7 { get; set; }
        public string Level_8 { get; set; }
        public string Level_9 { get; set; }
        public string Level_10 { get; set; }
        public string Supplier { get; set; }
        public string RepairType { get; set; }
        public string ItemList { get; set; }
        public decimal ChargeInternal{ get; set; }
        public decimal ChargeFirstYearWarranty { get; set; }
        public decimal ChargeExtendedWarranty { get; set; }
        public decimal ChargeCustomer { get; set; }
        public int? Id { get; set; }
        public bool IsGroupFilter { get; set; }
        public string Label { get; set; }
    }
}
