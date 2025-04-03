/* 
Author: ArunKarthik-IGT
Date: Feb 15th
Description:JM BlueStart-Get Product Service API
 */
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Repository.Interfaces
{
    public interface IProductsRepository
    {
        ProductsResponseModel Products(string CountryIsoCode, string StoreId, string ProductName, string Brand, string Sku, string Upc, IApiRequest apiRequest);
    }
}
