namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class TopSkuLocationViewModel
    {
        public string Location { get; set; }

        public List<TopSkuProductViewModel> Products { get; set; }

        public TopSkuLocationViewModel()
        {
            Products = new List<TopSkuProductViewModel>();
        }
    }
}