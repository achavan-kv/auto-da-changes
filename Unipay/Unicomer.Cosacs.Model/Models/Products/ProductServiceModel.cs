/* 
Author:  
Date:  
Description:POST Select Product Service  API 
 */

using System.Collections.Generic;

namespace Unicomer.Cosacs.Model.Models.Products
{
    public class ProductServiceModel
    {
        public string CountryIsoCode { get; set; }
        public List<ProductServiceItem> Items { get; set; }
    }
}