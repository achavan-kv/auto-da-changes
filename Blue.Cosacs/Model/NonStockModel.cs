using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Model
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
        public bool? CanApplyToPromotion { get; set; }
        public bool Active { get; set; }
        public decimal? TaxRate { get; set; }
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
