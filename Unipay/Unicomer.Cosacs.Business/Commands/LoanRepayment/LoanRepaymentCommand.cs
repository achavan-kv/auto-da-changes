using MediatR;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Business.Commands.LoanRepayment
{
    public class LoanRepaymentCommand : IRequest<LoanResponseModel>, IApiRequest
    {
        public ApiRequest GetRequest()
        {
            return new ApiRequest { RequestId = CosacsAccountId, RequestName = "repayment" };
        }

        public string CosacsAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
