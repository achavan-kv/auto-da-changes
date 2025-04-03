using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Business.Commands.ProcessDelivery
{
    public class ProcessDeliveryHandler : IRequestHandler<ProcessDeliveryCommand, DeliveryNotificationResponseModel>
    {
        private readonly IProcessDeliveryRepository processDeliveryRepository;
        private readonly IHttpClientService httpClientService;

        public ProcessDeliveryHandler(IProcessDeliveryRepository processDeliveryRepository, IHttpClientService httpClientService)
        {
            this.processDeliveryRepository = processDeliveryRepository;
            this.httpClientService = httpClientService;
        }

        public async Task<DeliveryNotificationResponseModel> Handle(ProcessDeliveryCommand request, CancellationToken cancellationToken)
        {
            Model.Models.Deliveries.DeliveryNotificationModel model = null;

            model = processDeliveryRepository.GetDeliveryNotification(request.CosacsAccountId, request.InvoiceNumber, request);

            var deliveryNotificationResponse = await httpClientService.DeliveryNotification(model, request);

            return new DeliveryNotificationResponseModel { 
                InvoiceNumber = request.InvoiceNumber, 
                Success = true 
            };
        }
    }
}
