/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using MediatR;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Business.Commands.LoanSchedule
{
    public class LoanScheduleCommand : IRequest<LoanResponseModel>, IApiRequest
    {
        public ApiRequest GetRequest()
        {
            return new ApiRequest { RequestId = CosacsAccountId, RequestName = "loan-schedule" };
        }

        public string CosacsAccountId { get; set; }
    }
}
