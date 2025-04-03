/* 
Author: ArunKarthik-IGT
Date: Feb 15th
Description:JM BlueStart-Get Product Service API
 */
using Blue.Cosacs.Unipay.Web.Exceptions;
using Blue.Cosacs.Unipay.Web.Formatters;
using MediatR;
using System.Threading.Tasks;
using System.Web.Http;
using Unicomer.Cosacs.Business.Commands.SelectProduct;
using Unicomer.Cosacs.Business.Queries.Products;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Blue.Cosacs.Unipay.Web.Controllers.V1
{
    [JsonFormatter]
    [HandleException]
    [RoutePrefix("v1")]
    public class ProductsV1Controller : ApiController
    {
        private readonly IMediator mediator;
        private readonly IDbLogggerRepository dbLoggger;

        public ProductsV1Controller(IMediator mediator, IDbLogggerRepository dbLoggger)
        {
            this.mediator = mediator;
            this.dbLoggger = dbLoggger;
        }

        [HttpGet()]
        [Route("product")]
        public async Task<ProductsResponseModel> ProductsAsync([FromUri] string countryIsoCode = "", string storeId = "", string productName = "", string brand = "", string sku = "", string upc = "")
        {
            var request = new ProductsQuery
            {
                CountryIsoCode = countryIsoCode,
                StoreId = storeId,
                ProductName = productName,
                Brand = brand,
                Sku = sku,
                Upc = upc,
            };

            dbLoggger.LogRequest(request, Request);

            return await mediator.Send(request);
        }

        [HttpPost()]
        [Route("product-selected")]
        public async Task<SelectProductResponsViewModel> Select([FromBody] SelectProductCommand command)
        {
            dbLoggger.LogRequest(command, Request);

            return await mediator.Send(command);
        }
    }
}