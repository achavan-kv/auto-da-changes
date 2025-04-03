using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Warranty.Model
{
    public class WarrantyReturnModel
    {
        public int Id { get; set; }
        public IEnumerable<WarrantyReturnFilterModel> WarrantyReturnFilters { get; set; }
        public int? WarrantyLength { get; set; }
        public int ElapsedMonths { get; set; }
        public int? FreeWarrantyLength { get; set; }
        public decimal? PercentageReturn { get; set; }
        public string BranchType { get; set; }
        public short? BranchNumber { get; set; }
        public Warranty Warranty { get; set; }
        public string WarrantyNo { get; set; }              // #17506
        public int? WarrantyId { get; set; }
        public string Level_1 { get; set; }

#if DEBUG
        public override string ToString()
        {
            var strVal = string.Format(
                "Id:{0}, " +
                "WarrantyLength:{1:00}, " +
                "FreeWarrantyLength:{2:00}, " +
                "ElapsedMonths:{3:00}, " +
                "PercentageReturn:{4:000.00}, " +
                "BranchType:{5}, " +
                "BranchNumber:{6}, " +
                "WarrantyId:{7}, " +
                "Level_1:{8}",
                Id,
                WarrantyLength.HasValue ? WarrantyLength.Value : 0,
                FreeWarrantyLength.HasValue ? FreeWarrantyLength.Value : 0,
                ElapsedMonths,
                PercentageReturn.HasValue ? PercentageReturn.Value : 0,
                BranchType,
                BranchNumber.HasValue ? BranchNumber.Value : 0,
                WarrantyId.HasValue ? WarrantyId.Value : 0,
                (WarrantyReturnFilters != null && WarrantyReturnFilters.Count() > 0) ? WarrantyReturnFilters.First().TagId == 5 ? "PCE" :
                (WarrantyReturnFilters.First().TagId == 6 ? "PCF" : string.Empty) : string.Empty
            );

            return strVal;
        }
#endif

    }

    public class WarrantyReturnFilterModel
    {
        public int Id { get; set; }
        public int WarrantyReturnId { get; set; }
        public int? LevelId { get; set; }
        public string LevelName { get; set; }
        public int? TagId { get; set; }
        public string TagName { get; set; }
    }

    public class WarrantiesReturnPercentageAndWarranty
    {
        public WarrantyReturn ReturnPercentage { get; set; }
        public Blue.Cosacs.Warranty.Warranty Warranty { get; set; }
    }
}
