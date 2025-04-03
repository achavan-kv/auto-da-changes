namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class SetModel
    {
        public SetModel()
        {
            Components = new List<SetProductView>();
            Locations = new List<SetLocationView>();
        }
        
        public IEnumerable<SetProductView> Components { get; set; }
        public IEnumerable<SetLocationView> Locations { get; set; }
        public IDictionary<string, string> Hierarchy { get; set; }
        public int Id { get; set; }
        public string Sku { get; set; }
        public string LongDescription { get; set; }
        public int Status { get; set; }
        public bool PriceTicket { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? CreatedDate { get; set; }

        public List<string> Tags { get; set; }
    }
}
