using MediatR;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Business.Commands.InvoiceRequest
{
    public class InvoiceRequestCommand : IRequest<InvoiceResponseModel>, IApiRequest
    {
        public ApiRequest GetRequest()
        {
            return new ApiRequest { RequestId = CosacsAccountId, RequestName = "invoice-request" };
        }

        public string CosacsAccountId { get; set; }
        public string InvoiceNumber { get; set; }
    }
}