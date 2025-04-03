/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using MediatR;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Business.Commands.LoanProcess
{
    public class LoanProcessCommand : IRequest<LoanResponseModel>, IApiRequest
    {
        string apiName;
        public ApiRequest GetRequest()
        {
            return new ApiRequest { RequestId = CosacsAccountId, RequestName = apiName };
        }

        public void SetApiName(string apiName)
        {
            this.apiName = apiName;
        }
        public string CosacsAccountId { get; set; }
        public string MambuAccountId { get; set; }
        public string ProcessRef { get; set; }
    }
}
