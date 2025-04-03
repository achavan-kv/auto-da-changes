namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class StockAdjustmentSearchModel
    {
        public StockAdjustmentSearchModel()
        {
            Results = new List<StockAdjustmentSearchResultModel>();
        }     
        public int TotalResults { get; set; }

        public List<StockAdjustmentSearchResultModel> Results { get; set; }     
    }
}
