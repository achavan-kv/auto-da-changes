using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Blue.Cosacs.SalesManagement.Api.Models;
using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Events;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [RoutePrefix("api/CallLog")]
    public class CallLogController : ApiController
    {
        private readonly ISalesManagementRepository salesManagementRepository;
        private readonly IClock clock;
        private readonly IEventStore audit;

        public CallLogController(IClock clock, ISalesManagementRepository salesManagementRepository, IEventStore audit)
        {
            this.salesManagementRepository = salesManagementRepository;
            this.clock = clock;
            this.audit = audit;
        }

        [Route("GetScheduledCalls")]
        [Permission(SalesManagementPermissionEnum.CSRCallLog)]
        public HttpResponseMessage GetScheduledCalls(byte? callTypeId, DateTime? fromScheduledDate, DateTime? toScheduledDate, string customerName, string reasonForCalling, int? take)
        {
            var searchFilter = new CallSearchFilter()
            {
                CallTypeId = callTypeId,
                ScheduledDateFrom = fromScheduledDate,
                ScheduledDateTo = toScheduledDate,
                CustomerName = customerName,
                ReasonForCalling = reasonForCalling,
                SalesPersonId = this.GetUser().Id,
                Take = take
            };

            var scheduledCalls = salesManagementRepository.GetScheduledCallsByCSR(searchFilter);

            return Request.CreateResponse(scheduledCalls);
        }

        [Route("GetCallTypes")]
        [HttpGet]
        [Permission(SalesManagementPermissionEnum.CSRCallLog)]
        public HttpResponseMessage GetCallTypes()
        {
            var callTypes = salesManagementRepository.GetCallTypesForCSR()
                .Select(p => new
                {
                    p.Id,
                    p.Name
                })
                .ToList();

            return Request.CreateResponse(callTypes);
        }

        [Permission(SalesManagementPermissionEnum.CSRCallLog)]
        public HttpResponseMessage Post(CallLog callLog)
        {
            this.HandleResource(callLog);
            return Request.CreateResponse();
        }

        private async void HandleResource(CallLog callLog)
        {
            Call callToInsert = null;

            if (callLog.ScheduleCallback != null)
            {
                callToInsert = new Call();

                callToInsert.ToCallAt = callLog.ScheduleCallback.Value;
                callToInsert.PreviousCallId = callLog.Id;
                callToInsert.CreatedOn = clock.Now;
                callToInsert.CustomerFirstName = callLog.CustomerFirstName;
                callToInsert.CustomerLastName = callLog.CustomerLastName;
                callToInsert.Source = (byte)CallSourceEnum.UserInterface;
                callToInsert.CustomerId = callLog.CustomerId;
                callToInsert.SalesPersonId = callLog.SpokeToCustomer ? this.GetUser().Id : callLog.SalesPersonId;
                callToInsert.LandLinePhone = callLog.LandLinePhone;
                callToInsert.MobileNumber = callLog.MobileNumber;
                callToInsert.CreatedBy = this.GetUser().Id;
                callToInsert.CreatedOn = clock.Now;

                if (!callLog.SpokeToCustomer)
                {
                    callToInsert.ReasonToCall = callLog.PreviousReasonToCall;
                    callToInsert.CallTypeId = callLog.CallTypeId;
                    callToInsert.MailchimpTemplateID = callLog.MailchimpTemplateID;
                    callToInsert.AlternativeContactMeanId = callLog.AlternativeContactMeanId;
                    callToInsert.SmsText = callLog.SmsText;
                    callToInsert.EmailSubject = callLog.EmailSubject;
                }
                else
                {
                    callToInsert.ReasonToCall = callLog.ReasonToCallAgain;
                    callToInsert.CallTypeId = (byte)CallTypeEnum.Callback;
                }
            }

            var callToHandle = new Call()
            {
                Id = callLog.Id,
                CalledAt = callLog.CalledAt,
                SpokeToCustomer = callLog.SpokeToCustomer,
                Comments = callLog.Comments,
                CustomerId = callLog.CustomerId,
                CreatedOn = clock.Now,
                CustomerFirstName = callLog.CustomerFirstName,
                CustomerLastName = callLog.CustomerLastName,
                LandLinePhone = callLog.LandLinePhone,
                MobileNumber = callLog.MobileNumber,
                Source = (byte)CallSourceEnum.UserInterface,
                SalesPersonId = this.GetUser().Id,
                ReasonToCall = callLog.ReasonToCallAgain,
                ToCallAt = clock.Now,
                CreatedBy = this.GetUser().Id,
                CalledBy = this.GetUser().Id
            };

            var customerSalesPerson = new CustomerSalesPerson()
            {
                CustomerId = callLog.CustomerId,
                DoNotCallAgain = callLog.DoNotCallAgain
            };

            salesManagementRepository.LogCall(callToHandle, callToInsert, callLog.PendingCalls, audit, customerSalesPerson);
        }

        [Permission(SalesManagementPermissionEnum.CSRCallLog)]
        public HttpResponseMessage Put(CallLog callLog)
        {
            this.HandleResource(callLog);

            return Request.CreateResponse(System.Net.HttpStatusCode.Created);
        }

        [Route("GetCurrentDateTime")]
        [HttpGet]
        [Permission(SalesManagementPermissionEnum.CSRCallLog)]
        public HttpResponseMessage GetCurrentDateTime()
        {
            return Request.CreateResponse(new
            {
                Date = clock.Now.ToUniversalTime()
            });
        }

        [Route("GetCustomers")]
        [Permission(SalesManagementPermissionEnum.CSRCallLog)]
        public HttpResponseMessage GetCustomersWithUnscheduledCalls()
        {
            var customers = salesManagementRepository.GetCustomersWithUnscheduledCallsByCSR(this.GetUser().Id);

            return Request.CreateResponse(customers);
        }

        [Route("GetCustomerDetails")]
        [Permission(SalesManagementPermissionEnum.CSRCallLog)]
        public HttpResponseMessage GetCustomerDetails(string customerId)
        {
            return Request.CreateResponse(salesManagementRepository.GetCustomerDetails(customerId));
        }

        [Route("GetCallsDetails")]
        [Permission(SalesManagementPermissionEnum.CSRCallLog)]
        public HttpResponseMessage GetCallsDetails(string customerId)
        {
            var pendingCalls = salesManagementRepository.GetCallsDetails(customerId);

            return Request.CreateResponse(pendingCalls);
        }
    }
}