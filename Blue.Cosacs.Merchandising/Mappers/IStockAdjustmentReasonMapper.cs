namespace Blue.Cosacs.Merchandising.Mappers
{
    using System.Collections.Generic;

    using Blue.Cosacs.Merchandising.Models;

    public interface IStockAdjustmentReasonMapper
    {
        List<StockAdjustmentReasonViewModel> MapViewModel(List<StockAdjustmentReasonView> reasons);
    }
}
