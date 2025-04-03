namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class StockCountPrintModel
    {
        public StockCountPrintModel()
        {
            Hierarchys = new List<StockCountHierarchyPrintModel>();
        }
        public int Id { get; set; }

        public int LocationId { get; set; }

        public string Location { get; set; }

        public string Type { get; set; }

        public DateTime? StartedDate { get; set; }

        public List<StockCountHierarchyPrintModel> Hierarchys { get; set; }
    }
}
