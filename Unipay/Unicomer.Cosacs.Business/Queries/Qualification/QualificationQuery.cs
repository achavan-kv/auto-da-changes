/* 
Author: Swati -IGT
Date: Feb 10 2022
Description:JM BlueStart-Qualification API 
 */
using MediatR;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Business.Queries.Qualification
{
    public class QualificationQuery : IRequest<QualificationResponseModel>, IApiRequest
    {
        public ApiRequest GetRequest()
        {
            return new ApiRequest
            {
                RequestId = SalesCustomerId,
                RequestName = "qualification-customer"
            };
        }

        public string CountryIsoCode { get; set; }
        public string SalesCustomerId { get; set; }
    }
}
