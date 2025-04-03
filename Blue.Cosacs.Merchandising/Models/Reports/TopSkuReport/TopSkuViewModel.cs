namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class TopSkuViewModel
    {
        public string SearchKey { get; set; }

        public List<Level> Levels { get; set; }

        public List<TopSkuLocationViewModel> Locations { get; set; }

        public TopSkuViewModel()
        {
            Locations = new List<TopSkuLocationViewModel>();
        }
    }
}