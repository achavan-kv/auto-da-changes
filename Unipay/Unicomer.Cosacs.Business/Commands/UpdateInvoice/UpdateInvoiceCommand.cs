using MediatR;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Model.Models.Payments;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Business.Commands.UpdateInvoice
{
    public class UpdateInvoiceCommand : IRequest<UpdatePaymentResponseModel>, IApiRequest
    {
        public ApiRequest GetRequest()
        {
            return new ApiRequest { 
                RequestId = UpdatePaymentRequest == null ? "" : UpdatePaymentRequest.InvoiceNumber, 
                RequestName = "updateinvoice" 
            };
        }

        public UpdatePaymentRequest UpdatePaymentRequest { get; set; }
    }
}