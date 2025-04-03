using System.Collections.Generic;

namespace Blue.Cosacs.Warranty.Model
{
    public class Warranty
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public short Length { get; set; }
        public decimal? TaxRate { get; set; }
        public string TypeCode { get; set; }
        public bool IsDeleted { get; set; }
        public List<Tag> WarrantyTags { get; set; }
        public WarrantyRenewals[] RenewalChildren { get; set; }
        public WarrantyRenewals[] RenewalParents  { get; set; }

        public Warranty(Cosacs.Warranty.Warranty warranty)
        {
            IsDeleted = warranty.Deleted;
            Description = warranty.Description;
            TypeCode = warranty.TypeCode;
            Id = warranty.Id;
            Length = warranty.Length;
            Number = warranty.Number;
            TaxRate = warranty.TaxRate;
        }

        public Warranty()
        { }

        public class Tag
        {
            public int TagId { get; set; }
            public int LevelId { get; set; }
            public string TagName { get; set; }

            public const int MaxLevel = 99;
        }

        public class WarrantyRenewals
        {
            public int id { get; set; }
            public string text { get; set; }
        }

    }


}