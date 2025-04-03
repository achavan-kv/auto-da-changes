/* 
Author:  
Date:  
Description:JM BlueStart--Store Service API
 */

using MediatR;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Business.Queries.Store
{
    public class StoreQuery: IRequest<StoreResponseModel>, IApiRequest
    {
        public ApiRequest GetRequest()
        {
            return new ApiRequest
            {
                RequestId = CountryIsoCode,
                RequestName = "stores"
            };
        }

        public string CountryIsoCode { get; set; }
    }
}
