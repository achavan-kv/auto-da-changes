using MediatR;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Model.ViewModels;
namespace Unicomer.Cosacs.Business.Commands.LoanAdjust
{
    public class LoanAdjustCommand : IRequest<LoanResponseModel>, IApiRequest
    {
        public ApiRequest GetRequest()
        {
            return new ApiRequest { RequestId = CosacsAccountId, RequestName = "adjust-payment" };
        }

        public string LoanTransactionId { get; set; }
        public string CosacsAccountId { get; set; }
        public string Notes { get; set; }
    }
}
