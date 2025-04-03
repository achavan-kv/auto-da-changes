using MediatR;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Business.Commands.ProcessDelivery
{
    public class ProcessDeliveryCommand : IRequest<DeliveryNotificationResponseModel>, IApiRequest
    {
        public ApiRequest GetRequest()
        {
            return new ApiRequest { RequestId = CosacsAccountId, RequestName = "delivery" };
        }

        public string CosacsAccountId { get; set; }
        public string InvoiceNumber { get; set; }
    }
}