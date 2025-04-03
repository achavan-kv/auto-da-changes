namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class LocationPriceModel
    {
        public LocationPriceModel()
        {
            Components = new List<SetProductView>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Fascia { get; set; }
        public IEnumerable<SetProductView> Components { get; set; }
    }
}
