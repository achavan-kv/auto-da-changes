/* 
Author: Suresh -IGT
Date: Feb 1st
Description:JM BlueStart-Invoice API Controller
*/

using Blue.Cosacs.Unipay.Web.Exceptions;
using Blue.Cosacs.Unipay.Web.Formatters;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Unicomer.Cosacs.Business.Commands.InvoiceRequest;
using Unicomer.Cosacs.Business.Commands.UpdateInvoice;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Blue.Cosacs.Unipay.Web.Controllers.V1
{
    [JsonFormatter]
    [HandleException]
    [RoutePrefix("v1")]
    public class PaymentV1Controller : ApiController
    {
        private readonly IMediator mediator;
        private readonly IDbLogggerRepository dbLoggger;

        public PaymentV1Controller(IMediator mediator, IDbLogggerRepository dbLoggger)
        {
            this.mediator = mediator;
            this.dbLoggger = dbLoggger;
        }

        [HttpPost()]
        [Route("invoice-request")]
        public async Task<InvoiceResponseModel> InvoiceRequest([FromBody] InvoiceRequestCommand invoiceRequest)
        {
            dbLoggger.LogRequest(invoiceRequest, Request);
            return await mediator.Send(invoiceRequest);
        }

        [HttpPost()]
        [Route("updateinvoice")]
        public async Task<UpdatePaymentResponseModel> UpdateInvoice([FromBody] UpdateInvoiceCommand updateInvoice)
        {
            dbLoggger.LogRequest(updateInvoice, Request);

            if (!Request.Headers.Contains("Country"))
            {
                throw new ValidationException(new ErrorMessage { Id = updateInvoice.GetRequest().RequestId, Detail = "Country is required in header" });
            }

            var countryIsoCodes = Request.Headers.GetValues("Country");

            if (countryIsoCodes == null)
            {
                countryIsoCodes = new List<string>();
            }

            countryIsoCodes = countryIsoCodes.Select(t => t != null ? t.Trim() : t);

            if (updateInvoice != null && updateInvoice.UpdatePaymentRequest != null
                && !countryIsoCodes.Contains(updateInvoice.UpdatePaymentRequest.CountryIsoCode))
            {
                throw new ValidationException(new ErrorMessage { Id = updateInvoice.GetRequest().RequestId, Detail = "Country not matched in header" });
            }

            return await mediator.Send(updateInvoice);
        }
    }
}
