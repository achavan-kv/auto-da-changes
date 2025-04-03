namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ComboModel
    {
        public ComboModel()
        {
            Components = new List<ComboProductView>();
            Status = 2;
        }

        public int Id { get; set; }
        public string SKU { get; set; }
        public string LongDescription { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Status { get; set; }
        public IEnumerable<ComboProductView> Components { get; set; }
        public List<string> Tags { get; set; }
        public IEnumerable<ComboPriceLocationModel> ComboPrices { get; set; }
        public bool PriceTicket { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public IDictionary<string, string> Hierarchy { get; set; }
    }
}
