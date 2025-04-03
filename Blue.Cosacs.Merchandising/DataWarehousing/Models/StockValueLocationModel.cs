namespace Blue.Cosacs.Merchandising.DataWarehousing.Models
{
    using System.Collections.Generic;

    using Percolator.AnalysisServices.Attributes;

    public class StockValueLocationModel
    {
        [MapTo("GrandParentName")]
        public string GrandParent { get; set; }

        [MapTo("ParentName")]
        public string Parent { get; set; }

        [MapTo("Name")]
        public string Name { get; set; }

        [MapTo("LocId")]
        public string LocationId { get; set; }

        [MapTo("LocName")]
        public string Location { get; set; }

        [MapTo("FascName")]
        public string Fascia { get; set; }

        [MapTo("WareName")]
        public string WareName { get; set; }

        public bool IsWarehouse
        {
            get
            {
                return WareName == "True";
            }
        }

        [MapTo("StockOnHandQuantity")]
        public int StockOnHandQuantity { get; set; }

        [MapTo("StockOnHandValue")]
        public decimal StockOnHandValue { get; set; }

        [MapTo("StockOnHandSalesValue")]
        public decimal StockOnHandSalesValue { get; set; }
    }
}