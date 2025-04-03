using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.NonStocks.Models
{
    public class NonStockPromotionModel
    {
        public int Id
        {
            get;
            set;
        }
        public int NonStockId
        {
            get;
            set;
        }
        public string NonStockNumber
        {
            get;
            set;
        }
        public string Fascia
        {
            get;
            set;
        }
        public string BranchNumber
        {
            get;
            set;
        }

        public int? WarehouseNumber
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
        public int? NonStockPriceId
        {
            get;
            set;
        }

        public string ShortDescription
        {
            get;
            set;
        }

        public string LongDescription
        {
            get;
            set;
        }
    }
}
