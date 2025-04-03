/* 
Author: ArunKarthik-IGT
Date: Feb 15th
Description:JM BlueStart-Get Product Service API
 */
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.DbCommands.Products;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Repository.Implementations
{
    internal class ProductsRepository : IProductsRepository
    {
        private readonly IDbLogggerRepository dbLoggger;

        public ProductsRepository(IDbLogggerRepository dbLoggger)
        {
            this.dbLoggger = dbLoggger;
        }

        public ProductsResponseModel Products(string countryIsoCode, string storeId, string productName, string brand, string sku, string upc, IApiRequest apiRequest)
        {
            return new ProductsCommand(countryIsoCode, storeId, productName, brand, sku, upc).Execute(apiRequest, dbLoggger);
        }
    }
}
