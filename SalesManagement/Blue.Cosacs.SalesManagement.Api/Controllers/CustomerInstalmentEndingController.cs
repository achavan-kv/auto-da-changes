using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Networking;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [Permission(SalesManagementPermissionEnum.CustomerInstalmentEndingJob)]
    public class CustomerInstalmentEndingController : ApiController
    {
        private readonly IClock clock;
        private readonly Settings cosacsSettings;
        private readonly ISalesManagementRepository repository;
        private readonly IHttpClientJson httpClientJson;
        private readonly int daysToCallBeforeInstallmentEnds;
        private const string ReasonToContact = "Final Instalment coming up for account #{0}";

        public CustomerInstalmentEndingController(IClock clock, ISalesManagementRepository repository, IHttpClientJson httpClientJson)
        {
            this.cosacsSettings = new Blue.Cosacs.SalesManagement.Settings();
            this.clock = clock;
            this.repository = repository;
            this.httpClientJson = httpClientJson;
            this.daysToCallBeforeInstallmentEnds = GetDaysToCallBeforeInstallmentEnds();
        }

        [HttpGet]
        [LongRunningQueries]
        [CronJob]
        public HttpResponseMessage Get()
        {
            var dateToSchedule = GetDateToSchedule();
            var dateToCall = GetDateToCall();
            var existingCalls = repository.GetCustomersWithAutoScheduledCalls(clock.Now.Date, dateToSchedule);
            var customersFromWinCosacs = ExternalHttpSources.Post<CustomersInstalmentResult>(
                "/Courts.NET.WS/SalesManagement/GetCustomersInstalments?dateToSchedule=" + dateToSchedule.ToString(CultureInfo.InvariantCulture),
                httpClientJson);
            var customersInteraction = repository.LoadInactiveCustomersInteraction(AdditionalCustomersInteraction.InteractionType.InstalmentEnding).First();

            var customersWithOutOfRangeInstalments = GetCustomersWithOutOfRangeInstalments(
                new HashSet<string>(customersFromWinCosacs.CustomersWithinTheRange.Select(p => p.CustomerId), StringComparer.InvariantCultureIgnoreCase),
                new HashSet<string>(customersFromWinCosacs.CustomersExactDate.Select(p => p.CustomerId), StringComparer.InvariantCultureIgnoreCase),
                new HashSet<string>(existingCalls, StringComparer.InvariantCultureIgnoreCase));

            var callToDelete = customersWithOutOfRangeInstalments
                .Where(p => p.Value == 'D')
                .Select(p => p.Key)
                .ToList();

            repository.DeleteCustomersWithAutoScheduledCalls(dateToSchedule, callToDelete);

            var callToUpdate = customersFromWinCosacs.CustomersWithinTheRange
                .Join(
                    customersWithOutOfRangeInstalments.Where(p => p.Value == 'U'),
                    left => left.CustomerId,
                    right => right.Key,
                    (l, r) => new
                    {
                        l.CustomerId,
                        NewCallDate = GetNewDateToCallForMovedInstalments(l.LastInstalmentDate.Value.Date, dateToCall)
                    })
                .ToDictionary(p => p.CustomerId, p => p.NewCallDate);

            repository.UpdateAutoScheduledCalls(clock.Now.Date, dateToSchedule.Date, callToUpdate);

            var callsToInsert = customersFromWinCosacs.CustomersWithinTheRange
                .Join(
                    customersWithOutOfRangeInstalments.Where(p => p.Value == 'I'),
                    left => left.CustomerId,
                    right => right.Key,
                    (l, r) => l)
                .Union(customersFromWinCosacs.CustomersExactDate.Where(p => !callToDelete.Contains(p.CustomerId)))
                .ToList();

            var customersSalesPerson = GetCustomersSalesParson(new HashSet<string>(callsToInsert.Select(p => p.CustomerId), StringComparer.InvariantCultureIgnoreCase));

            CommitCustomerCsr(callsToInsert, customersSalesPerson);
            callsToInsert = GetCustomersToCallAgain(callsToInsert);

            switch (customersInteraction.ContactMeansIdEnum)
            {
                case ContactMeanEnum.Email:
                    InsertMails(callsToInsert, customersInteraction, GetDateToCall(), customersSalesPerson);
                    break;

                case ContactMeanEnum.PhoneCall:
                    InsertCalls(callsToInsert, customersSalesPerson, GetDateToCall(), customersInteraction);
                    break;

                case ContactMeanEnum.Sms:
                    InsertSms(callsToInsert, customersInteraction, GetDateToCall(), customersSalesPerson);
                    break;

                default:
                    throw new Exception("Unknown ContactMean option");
            }

            return Request.CreateResponse();
        }

        internal void CommitCustomerCsr(IList<CustomerInstalments> allCustomers, Dictionary<string, int?> customersSalesPerson)
        {
            var existingCustomers = repository.GetCustomersSalesPerson(null, allCustomers.Select(p => p.CustomerId).ToList())
                .ToDictionary(p => p.CustomerId);

            var customerNoCsr = allCustomers
                .Where(p => !customersSalesPerson[p.CustomerId].HasValue)
                /* in case that there are 2+ records for the same customer in allCustomers collection*/
                .GroupBy(p => p.CustomerId)
                .Select(p => p.First())
                /*end*/
                .Select(p => new CustomerSalesPerson
                {
                    CustomerId = p.CustomerId,
                    SalesPersonId = p.SalesPerson,
                    LandLinePhone = p.LandLinePhone,
                    MobileNumber = p.MobileNumber,
                    DoNotCallAgain = false,
                    CustomerBranch = p.CustomerBranch,
                    Email = p.Email
                })
                .ToList();

            repository.UpdateCustomersSalesPerson(customerNoCsr
                .Where(p => existingCustomers.ContainsKey(p.CustomerId))
                .ToDictionary(p => p.CustomerId));

            repository.InsertCustomersSalesPerson(customerNoCsr
                .Where(p => !existingCustomers.ContainsKey(p.CustomerId))
                .ToList());
        }

        internal DateTime GetNewDateToCallForMovedInstalments(DateTime lastInstalmentDate, DateTime dateToCall)
        {
            var newDate = lastInstalmentDate.Date.AddDays(0 - this.daysToCallBeforeInstallmentEnds);

            if (newDate.Date <= clock.Now.Date)
            {
                newDate = lastInstalmentDate.Date;
            }

            return new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 0);
        }

        internal IList<KeyValuePair<string, char>> GetCustomersWithOutOfRangeInstalments(
            HashSet<string> customersFromWinCosacsWininRange,
            HashSet<string> customersFromWinCosacsExactDate,
            HashSet<string> customersWithCalls)
        {
            var movedBack = new HashSet<string>(customersFromWinCosacsWininRange, StringComparer.InvariantCultureIgnoreCase);
            var deleted = new HashSet<string>(customersWithCalls, StringComparer.InvariantCultureIgnoreCase);
            var movedForward = new HashSet<string>(customersFromWinCosacsExactDate, StringComparer.InvariantCultureIgnoreCase);

            /*
                customersFromWinCosacsWinInRange = 5,6,7,10
                customersFromWinCosacsExactDate = 9,6
                customersWithCalls = 4,5,6,7

                the result should be:
                    a = 10 -> this is a moved instalment to an early (already processed) date (INSERT)
                    b = 4  -> this is a call already in the DB but the instalment was deleted (DELETE)
                    c = 6  -> this is a call whos instalment was moved forward                (UPDATE)
            */

            /*a ->*/
            movedBack.ExceptWith(customersWithCalls);
            /*b ->*/
            deleted.ExceptWith(customersFromWinCosacsWininRange);
            /*c ->*/
            movedForward.IntersectWith(customersFromWinCosacsWininRange);

            return movedBack
                .Select(p => new KeyValuePair<string, char>(p, 'I'))
                .Union(deleted
                   .Select(p => new KeyValuePair<string, char>(p, 'D')))
                .Union(movedForward
                   .Select(p => new KeyValuePair<string, char>(p, 'U')))
                .ToList();
        }

        internal void InsertCalls(IEnumerable<CustomerInstalments> customersToInsert, Dictionary<string, int?> customersSalesPerson, DateTime toCallAt, AdditionalCustomersInteraction customerInteraction)
        {
            var icon = GetCustomerInstalmentEndingCallsIcon();

            var calls = customersToInsert
                .Select(p => new Call
                {
                    CallClosedReasonId = null,
                    CalledAt = (DateTime?)null,
                    SalesPersonId = customersSalesPerson[p.CustomerId].HasValue ? customersSalesPerson[p.CustomerId].Value : p.SalesPerson,
                    CallTypeId = (int)CallTypeEnum.AutoScheduled,
                    Comments = null,
                    CreatedBy = null,
                    CreatedOn = clock.Now,
                    CustomerFirstName = p.FirstName,
                    CustomerId = p.CustomerId,
                    CustomerLastName = p.LastName,
                    PreviousCallId = (int?)null,
                    ReasonToCall = string.Format(ReasonToContact, p.AccountNumber),
                    SpokeToCustomer = false,
                    ToCallAt = GetNewDateToCallForMovedInstalments(p.LastInstalmentDate.Value, toCallAt),
                    Source = (byte)CallSourceEnum.InstalmentEnd,
                    Icon = icon,
                    AlternativeContactMeanId = customerInteraction.AlternativeContactMeanId.HasValue ? customerInteraction.AlternativeContactMeanId : null,
                    EmailSubject = customerInteraction.AlternativeContactMeanId.HasValue ? string.Format(ReasonToContact, p.AccountNumber) : null,
                    SmsText = customerInteraction.AlternativeContactMeanId.HasValue ? customerInteraction.SmsText : null,
                    LandLinePhone = p.LandLinePhone,
                    MailchimpTemplateID = customerInteraction.MailchimpTemplateID,
                    MobileNumber = p.MobileNumber
                })
                .ToList();

            repository.SaveCalls(calls);
        }

        internal void InsertMails(IList<CustomerInstalments> customersToInsert, AdditionalCustomersInteraction customerInteraction, DateTime dateToMail, Dictionary<string, int?> customersSalesPerson)
        {
            repository.InsertMailsToSend(customersToInsert
                .Where(p => !string.IsNullOrWhiteSpace(p.Email))
                .Select(p => new MailsToSend
                {
                    CustomerId = p.CustomerId,
                    DateToSend = GetNewDateToCallForMovedInstalments(p.LastInstalmentDate.Value, dateToMail),
                    MailAdress = p.Email,
                    MailSudject = string.Format(ReasonToContact, p.AccountNumber),
                    OverrideUnsubscribe = true,
                    TemplateId = customerInteraction.MailchimpTemplateID.Value,
                    CustomerName = string.Format("{0} {1}", p.FirstName, p.LastName)
                })
                .ToList());

            //for customers with no email call then
            InsertCalls(
                customersToInsert
                    .Where(p => string.IsNullOrEmpty(p.Email)).ToList(),
                customersSalesPerson,
                dateToMail,
                customerInteraction);
        }

        internal void InsertSms(IList<CustomerInstalments> customersToInsert, AdditionalCustomersInteraction customerInteraction, DateTime dateToSend, Dictionary<string, int?> customersSalesPerson)
        {
            repository.InsertSmsToSend(customersToInsert
                .Where(p => !string.IsNullOrWhiteSpace(p.MobileNumber))
                .Select(p => new SmsToSend
                {
                    CustomerId = p.CustomerId,
                    DateToSend = GetNewDateToCallForMovedInstalments(p.LastInstalmentDate.Value, dateToSend),
                    Body = customerInteraction.SmsText,
                    PhoneNumber = p.MobileNumber
                })
                .ToList());

            //for customers with no email call then
            InsertCalls(
                customersToInsert
                    .Where(p => string.IsNullOrEmpty(p.Email)).ToList(),
                customersSalesPerson,
                dateToSend,
                customerInteraction);
        }

        private DateTime GetDateToSchedule()
        {
            var days = cosacsSettings.DaysToScheduleCallBeforeInstallmentEnds;

            return this.clock.Now.AddDays(days);
        }

        private DateTime GetDateToCall()
        {
            var value = GetDateToSchedule().AddDays(daysToCallBeforeInstallmentEnds * -1);

            return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
        }

        private int GetDaysToCallBeforeInstallmentEnds()
        {
            return cosacsSettings.DaysToCallBeforeInstallmentEnds;
        }

        internal Dictionary<string, int?> GetCustomersSalesParson(HashSet<string> customersId)
        {
            var customersNoSalesPerson = repository.GetCustomersSalesPerson(null, customersId.ToList())
                    .Select(p => new
                    {
                        p.CustomerId,
                        p.SalesPersonId
                    })
                    .ToList()
                    .Select(p => new KeyValuePair<string, int>(p.CustomerId, p.SalesPersonId))
                    .ToList();
            var returnValue = new List<IEnumerable<KeyValuePair<string, int?>>>();

            customersId.ExceptWith(customersNoSalesPerson.Select(p => p.Key));

            return new Dictionary<string, int?>(
                customersNoSalesPerson
                    .Select(p => new KeyValuePair<string, int?>(p.Key, new Nullable<int>(p.Value)))
                    .Union(customersId
                        .Select(p => new KeyValuePair<string, int?>(p, (int?)null)))
                    .ToDictionary(p => p.Key, p => p.Value),
                StringComparer.InvariantCultureIgnoreCase);
        }

        internal List<CustomerInstalments> GetCustomersToCallAgain(IList<CustomerInstalments> customers)
        {
            var customersSalesPerson = repository.GetCustomersSalesPerson(true, customers.Select(p => p.CustomerId).ToList())
                .Select(p => p.CustomerId)
                .ToList();

            return customers
                .Where(p => !customersSalesPerson.Contains(p.CustomerId))
                .ToList();
        }

        private string GetCustomerInstalmentEndingCallsIcon()
        {
            using (var scope = Context.Write())
            {
                return scope.Context.IconTypes
                    .Where(p => p.Id == 2)
                    .Select(p => p.Icon)
                    .FirstOrDefault();
            }
        }
    }
}