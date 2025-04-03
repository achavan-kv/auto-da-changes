using System;
using System.Collections.Generic;
using System.Text;

namespace STL.Common.Services.Model
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
        //public DateTime DeliveryDate { get; set; }
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
}
