
namespace Blue.Cosacs.Warranty
{
    public class EventType
    {
        public const string CreateLevel = "CreateLevel";
        public const string UpdateLevel = "UpdateLevel";
        public const string CreateTag = "CreateTag";
        public const string UpdateTag = "UpdateTag";
        public const string CreateWarrantySale = "CreateWarrantySale";
        public const string CancelWarrantySale = "CancelWarrantySale";
        public const string CreatePotentialWarrantySale = "CreatePotentialWarrantySale";

    }

    public class EventCategory
    {
        public const string Level = "WarrantyLevel";
        public const string Tag = "WarrantyTag";
        public const string WarrantySale = "WarrantySale";
        public const string PotentialWarrantySale = "PotentialWarrantySale";
    }
}
