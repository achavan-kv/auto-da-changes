using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Business.Commands.UpdateInvoice
{
    public class UpdateInvoiceHandler : IRequestHandler<UpdateInvoiceCommand, UpdatePaymentResponseModel>
    {
        private readonly IPaymentRepository paymentRepository;
        private readonly IDbLogggerRepository dbLoggger;

        public UpdateInvoiceHandler(IPaymentRepository paymentRepository, IDbLogggerRepository dbLoggger)
        {
            this.paymentRepository = paymentRepository;
            this.dbLoggger = dbLoggger;
        }

        public async Task<UpdatePaymentResponseModel> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
        {
            System.Exception exception = null;
            bool result = false;

            try
            {
                result = paymentRepository.UpdatePaymentRequest(request.UpdatePaymentRequest, request);
            }
            catch (System.Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                dbLoggger.LogApiRequestResponse(request, result, exception);
            }

            return await Task.FromResult(new UpdatePaymentResponseModel
            {
                UpdatePaymentResponse = new Model.Models.Payments.UpdatePaymentResponse
                {
                    CountryIsoCode = request.UpdatePaymentRequest.CountryIsoCode,
                    CreditAccountId = request.UpdatePaymentRequest.CreditAccountId,
                    RefInvoiceNumber = request.UpdatePaymentRequest.InvoiceNumber,
                    Status = "invoiced"
                }
            });
        }
    }
}