using MediatR;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Business.Commands.InvoiceRequest
{
    public class InvoiceRequestHandler : IRequestHandler<InvoiceRequestCommand, InvoiceResponseModel>
    {
        private readonly IAccountRepository accountRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IHttpClientService httpClientService;

        public InvoiceRequestHandler(IAccountRepository accountRepository,
                                    IPaymentRepository paymentRepository,
                                    IHttpClientService httpClientService)
        {
            this.accountRepository = accountRepository;
            this.paymentRepository = paymentRepository;
            this.httpClientService = httpClientService;
        }

        public async Task<InvoiceResponseModel> Handle(InvoiceRequestCommand request, CancellationToken cancellationToken)
        {
            string countryIsoCode = "";

            var paymentRequest = paymentRepository.GetPaymentRequest(request.CosacsAccountId, request.InvoiceNumber, request, out countryIsoCode);

            await httpClientService.PaymentRequest(paymentRequest, countryIsoCode, request);

            return new InvoiceResponseModel
            {
                CosacsAccountId = request.CosacsAccountId,
                InvoiceNumber = request.InvoiceNumber,
                Success = true
            };
        }
    }
}
