using Blue.Glaucous.Client.Api;
using System;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Payments.Api.Controllers
{
    [RoutePrefix("api/PaymentSetup")]
    public class PaymentSetupController : ApiController
    {
        private readonly IPaymentSetupRepository paymentSetupRepository;

        public PaymentSetupController(IPaymentSetupRepository paymentSetupRepository)
        {
            this.paymentSetupRepository = paymentSetupRepository;
        }

        [Route("GetAllPaymentMethods")]
        [HttpGet]
        public HttpResponseMessage GetAllPaymentMethods()
        {
            var paymentMethods = paymentSetupRepository.GetAllPaymentMethods();

            return Request.CreateResponse(new { Result = "Done", @PaymentMethods = paymentMethods });
        }

        [Route("CardTypes")]
        [HttpGet]
        public HttpResponseMessage GetCardTypes()
        {
            var settings = new Settings();
            return Request.CreateResponse(new { Result = "Done", @CardTypes = settings.PaymentCardType });
        }

        [Route("GetActivePaymentMethods")]
        [HttpGet]
        public HttpResponseMessage GetActivePaymentMethods()
        {
            var paymentMethods = paymentSetupRepository.GetActivePaymentMethods();

            return Request.CreateResponse(new { Result = "Done", @PaymentMethods = paymentMethods });
        }

        public HttpResponseMessage Post(PaymentMethod paymentMethod)
        {
            try
            {
                var updatedPayMethod = paymentSetupRepository.PaymentMethodSetup(paymentMethod, this.GetUser().Id);

                return Request.CreateResponse(new { Result = "Success", @PaymentMethods = updatedPayMethod });
            }
            catch (OperationCanceledException ex)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

    }
}
