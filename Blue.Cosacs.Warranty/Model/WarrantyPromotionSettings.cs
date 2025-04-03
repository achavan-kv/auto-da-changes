using System;
using System.Collections.Generic;

namespace Blue.Cosacs.Warranty.Model
{
    public class WarrantyPromotionSettings
    {
        public int Id
        {
            get;
            set;
        }
        public int WarrantyId
        {
            get;
            set;
        }
        public string WarrantyNumber
        {
            get;
            set;
        }
        public string BranchType
        {
            get;
            set;
        }
        public string BranchNumber
        {
            get;
            set;
        }
        internal short? BranchNumberNumeric
        {
            get
            {
                short result;
                if (short.TryParse(this.BranchNumber, out result))
                {
                    return new short?(result);
                }

                return new short?();
            }
        }
        public string BranchName
        {
            get;
            set;
        }
        public DateTime StartDate
        {
            get;
            set;
        }
        public DateTime EndDate
        {
            get;
            set;
        }
        public decimal? PercentageDiscount
        {
            get;
            set;
        }
        public decimal? RetailPrice
        {
            get;
            set;
        }
        public int? WarrantyPriceId
        {
            get;
            set;
        }
    }
}
