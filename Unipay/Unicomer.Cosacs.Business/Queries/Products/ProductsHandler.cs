/* 
Author: ArunKarthik-IGT
Date: Feb 15th
Description:JM BlueStart-Get Product Service API
 */
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Business.Queries.Products
{
    public class ProductsHandler : IRequestHandler<ProductsQuery, ProductsResponseModel>
    {
        private readonly IProductsRepository productsRepository;
        private readonly IDbLogggerRepository dbLoggger;

        public ProductsHandler(IProductsRepository productsRepository, IDbLogggerRepository dbLoggger)
        {
            this.productsRepository = productsRepository;
            this.dbLoggger = dbLoggger;
        }
        public async Task<ProductsResponseModel> Handle(ProductsQuery request, CancellationToken cancellationToken)
        {
            System.Exception exception = null;
            ProductsResponseModel result = null;

            try
            {
                result = productsRepository.Products(request.CountryIsoCode, request.StoreId, request.ProductName, request.Brand, request.Sku, request.Upc, request);
            }
            catch (System.Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                dbLoggger.LogApiRequestResponse(request, result, exception);
            }

            return await Task.FromResult(result);
        }
    }
}
