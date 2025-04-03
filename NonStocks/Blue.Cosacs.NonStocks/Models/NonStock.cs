using System.Collections.Generic;

namespace Blue.Cosacs.NonStocks.Models
{
    public class NonStockModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string SKU { get; set; }
        public string VendorUPC { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public int? Length { get; set; }
        public int? DiscountRecurringPeriod { get; set; }
        public bool? IsStaffDiscount { get; set; }
        public bool Active { get; set; }
        public decimal? TaxRate { get; set; }
        public bool HasProductLink { get; set; }
        public bool? IsFullRefund { get; set; }
        public decimal? CoverageValue { get; set; }

        public List<NonStockHierarchyModel> Hierarchy { get; set; }
        public NonStockHierarchyModel Level1
        {
            get
            {
                var lev = 0;
                return GetHierarchyLevelValue(lev);
            }
        }
        public NonStockHierarchyModel Level2
        {
            get
            {
                var lev = 1;
                return GetHierarchyLevelValue(lev);
            }
        }
        public NonStockHierarchyModel Level3
        {
            get
            {
                var lev = 2;
                return GetHierarchyLevelValue(lev);
            }
        }
        public NonStockHierarchyModel Level4
        {
            get
            {
                var lev = 3;
                return GetHierarchyLevelValue(lev);
            }
        }
        public NonStockHierarchyModel Level5
        {
            get
            {
                var lev = 4;
                return GetHierarchyLevelValue(lev);
            }
        }

        public NonStock ToEntity()
        {
            return new NonStock()
            {
                Id = this.Id,
                Type = this.Type,
                SKU = this.SKU,
                VendorUPC = this.VendorUPC,
                ShortDescription = this.ShortDescription,
                LongDescription = this.LongDescription,
                Length = this.Length,
                DiscountRecurringPeriod = this.DiscountRecurringPeriod,
                IsStaffDiscount = this.IsStaffDiscount,
                Active = this.Active,
                TaxRate = this.TaxRate,
                IsFullRefund = this.IsFullRefund,
                CoverageValue = this.CoverageValue
            };
        }

        public void ApplyChanges(NonStock item)
        {
            item.Type = this.Type;
            item.SKU = this.SKU;
            item.VendorUPC = this.VendorUPC;
            item.ShortDescription = this.ShortDescription;
            item.LongDescription = this.LongDescription;
            item.Active = this.Active;
            item.Length = this.Length;
            item.DiscountRecurringPeriod = this.DiscountRecurringPeriod;
            item.IsStaffDiscount = this.IsStaffDiscount;
            item.TaxRate = this.TaxRate;
            item.IsFullRefund = this.IsFullRefund;
            item.CoverageValue = this.CoverageValue;
        }

        public static NonStockModel ToModel(NonStock item, List<NonStockHierarchy> hierarchy)
        {
            var retVal = ToModel(item);
            retVal.Hierarchy = new List<NonStockHierarchyModel>();

            for (int i = 0; i < hierarchy.Count; i++)
            {
                retVal.Hierarchy.Add(new NonStockHierarchyModel()
                {
                    Level = hierarchy[i].Level,
                    SelectedKey = hierarchy[i].LevelKey,
                    SelectedValue = hierarchy[i].LevelName,
                });
            }

            return retVal;
        }

        private static NonStockModel ToModel(NonStock item)
        {
            return new NonStockModel()
            {
                Id = item.Id,
                Type = item.Type,
                SKU = item.SKU,
                VendorUPC = item.VendorUPC,
                ShortDescription = item.ShortDescription,
                LongDescription = item.LongDescription,
                Length = item.Length,
                DiscountRecurringPeriod = item.DiscountRecurringPeriod,
                IsStaffDiscount = item.IsStaffDiscount,
                Active = item.Active,
                TaxRate = item.TaxRate,
                IsFullRefund = item.IsFullRefund,
                CoverageValue = item.CoverageValue
            };
        }

        public class NonStockHierarchyModel
        {
            public byte Level { get; set; }
            public string LevelName { get; set; }
            public string SelectedKey { get; set; }
            public string SelectedValue { get; set; }
        }

        private NonStockHierarchyModel GetHierarchyLevelValue(int lev)
        {
            if (Hierarchy != null && Hierarchy.Count > lev)
            {
                if (Hierarchy[lev].Level == lev + 1)
                {
                    return Hierarchy[lev];
                }
            }

            return null;
        }
    }
}
