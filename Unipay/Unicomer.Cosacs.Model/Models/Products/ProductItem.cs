using System.Collections.Generic;

namespace Unicomer.Cosacs.Model.Models.Products
{
    public class ProductItem
    {
        public string countryIsoCode { get; set; }
        public List<ProductItemsDetail> ItemsDetails { get; set; }
    }
}
