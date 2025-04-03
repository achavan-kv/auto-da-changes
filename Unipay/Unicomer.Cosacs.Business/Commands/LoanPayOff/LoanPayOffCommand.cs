using MediatR;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Business.Commands.LoanPayOff
{
    public class LoanPayOffCommand : IRequest<LoanResponseModel>, IApiRequest
    {
        public ApiRequest GetRequest()
        {
            return new ApiRequest { RequestId = CosacsAccountId, RequestName = "loan-payoff" };
        }

        public string CosacsAccountId { get; set; }
    }
}
