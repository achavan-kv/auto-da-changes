/* 
Author: Suresh -IGT
Date: Feb 1st
Description:JM BlueStart-Pre-Qualification API 
 */
using MediatR;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Business.Queries.PreQualifications
{
    public class PreQualificationQuery : IRequest<PreQualificationResponseModel>, IApiRequest
    {
        public ApiRequest GetRequest()
        {
            return new ApiRequest
            {
                RequestId = SalesCustomerId,
                RequestName = "prequalification-customer"
            };
        }

        public string SalesCustomerId { get; set; }
    }
}
