/* 
Author: ArunKarthik-IGT
Date: Feb 15th
Description:JM BlueStart-Get Product Service API
 */
using MediatR;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Business.Queries.Products
{
    public class ProductsQuery : IRequest<ProductsResponseModel>, IApiRequest
    {
        public ApiRequest GetRequest()
        {
            return new ApiRequest
            {
                RequestId = CountryIsoCode,
                RequestName = "product"
            };
        }

        public string CountryIsoCode { get; set; }
        public string StoreId { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public string Sku { get; set; }
        public string Upc { get; set; }
    }
}
