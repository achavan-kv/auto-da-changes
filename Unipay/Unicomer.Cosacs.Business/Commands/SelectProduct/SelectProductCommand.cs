using MediatR;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Model.Models.Products;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Business.Commands.SelectProduct
{
    public class SelectProductCommand : IRequest<SelectProductResponsViewModel>, IApiRequest
    {
        public ApiRequest GetRequest()
        {
            var requestId = "";

            if (ItemsAvailable != null)
            {
                requestId = ItemsAvailable.CountryIsoCode;
            }

            return new ApiRequest { RequestId = requestId, RequestName = "product-selected" };
        }
         
        public ProductServiceModel ItemsAvailable { get; set; }
    }
}
