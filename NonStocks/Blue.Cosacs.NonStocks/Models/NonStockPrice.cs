using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.NonStocks.Models
{
    public class NonStockPriceModel
    {
        public int Id { get; set; }
        public int NonStockId { get; set; }
        public string NonStockType { get; set; }
        public string Fascia { get; set; }
        public int? BranchNumber { get; set; }
        public int? WarehouseNumber { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? TaxInclusivePrice { get; set; }
        public decimal? DiscountValue { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool HasPromotion { get; set; }

#if DEBUG
        public override string ToString()
        {
            return
                "Id: " + this.Id.ToString("00000") +
                ", NonStockId: " + this.NonStockId.ToString("00000") +
                ", Type: " + this.NonStockType +
                ", Fascia: " + this.Fascia +
                ", BranchNumber: " + (this.BranchNumber.HasValue ? this.BranchNumber.Value.ToString("000") : "---") +
                ", WarehouseNumber: " + (this.WarehouseNumber.HasValue ? this.WarehouseNumber.Value.ToString("00") : "--") +
                ", CostPrice: " + (this.CostPrice.HasValue ? this.CostPrice.Value.ToString("0.000") : "-------") +
                ", RetailPrice: " + (this.RetailPrice.HasValue ? this.RetailPrice.Value.ToString("0.000") : "-------") +
                ", TaxInclusivePrice: " + (this.TaxInclusivePrice.HasValue ? this.TaxInclusivePrice.Value.ToString("0.000") : "-------") +
                ", DiscountValue: " + (this.DiscountValue.HasValue ? this.DiscountValue.Value.ToString("0.000") : "-------") +
                ", EffectiveDate: " + this.EffectiveDate.ToString("u") +
                ", EndDate: " + (this.EndDate.HasValue ? this.EndDate.Value.ToString("u") : "-------") +
                ", HasPromotion: " + (this.HasPromotion ? "Yes" : "No");
        }
#endif
    }
}
