namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class TopSkuPrintViewModel
    {
        public TopSkuSearchModel Query { get; set; }

        public List<Level> Levels { get; set; }

        public List<TopSkuLocationViewModel> Locations { get; set; }

        public TopSkuPrintViewModel()
        {
            Locations = new List<TopSkuLocationViewModel>();
        }
    }
}