namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class StockCountSearchModel
    {
        public StockCountSearchModel()
        {
            Results = new List<StockCountSearchResultModel>();
        }     
        public int TotalResults { get; set; }

        public List<StockCountSearchResultModel> Results { get; set; }     
    }
}
