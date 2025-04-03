namespace Blue.Cosacs.Merchandising.Mappers
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Models;

    public class StockAdjustmentReasonMapper : IStockAdjustmentReasonMapper
    {
        public List<StockAdjustmentReasonViewModel> MapViewModel(List<StockAdjustmentReasonView> reasons)
        {
            return reasons
                .OrderBy(r => r.PrimaryReason)
                .ThenBy(r => r.SecondaryReason)
                .GroupBy(r => r.PrimaryReasonId)
                .Select(g =>
                {
                    var primaryReason = Mapper.Map<StockAdjustmentReasonViewModel>(g.First());
                    primaryReason.SecondaryReasons = g.Where(s => s.SecondaryReasonId != null).Select(Mapper.Map<StockAdjustmentSecondaryReasonViewModel>).ToList();
                    return primaryReason;
                })
                .ToList();
        }
    }
}