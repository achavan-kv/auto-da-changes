using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Glaucous.Client.Api;
using Blue.Networking;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Threading.Tasks;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [Permission(SalesManagementPermissionEnum.InactiveCustomersJob)]
    public class InactiveCustomersController : ApiController
    {
        private readonly IClock clock;
        private readonly Blue.Cosacs.SalesManagement.Settings cosacsSettings;
        private readonly ISalesManagementRepository repository;
        private readonly IHttpClientJson httpClientJson;

        private const int RangeBeginnin = 30;
        private const int NumberOfRecordsToReturn = 1000;
        private const string ReasonToCall = "Inactive since {0}";

        public InactiveCustomersController(IClock clock, ISalesManagementRepository repository, IHttpClientJson httpClientJson)
        {
            this.cosacsSettings = new Blue.Cosacs.SalesManagement.Settings();
            this.clock = clock;
            this.repository = repository;
            this.httpClientJson = httpClientJson;
        }

        [HttpGet]
        [LongRunningQueries]
        [CronJob]
        public HttpResponseMessage Get()
        {
            var url = new StringBuilder();

            url.Append("/Courts.NET.WS/SalesManagement/GetInactiveCustomers?howOldCash=")
              .Append(InactiveDateCash().ToString(CultureInfo.InvariantCulture))
              .Append("&howOldCredit=")
              .Append(InactiveDateCredit().ToString(CultureInfo.InvariantCulture))
              .Append("&numberOfDaysBefore=")
              .Append(RangeBeginnin)
              .Append("&numberOfRecordsToReturn=")
              .Append(NumberOfRecordsToReturn);

            var winCosacsData = ExternalHttpSources.Post<List<CustomerLastPurchase>>(url.ToString(), httpClientJson);
            var customersInteraction = repository.LoadInactiveCustomersInteraction(AdditionalCustomersInteraction.InteractionType.InactiveCustomer).First();
            var customersSalesPerson = GetCustomersSalesParson(new HashSet<string>(winCosacsData.Select(p => p.CustomerId), StringComparer.InvariantCultureIgnoreCase));

            InsertCustomerCsr(winCosacsData, customersSalesPerson);
            /*
             1-> GetCustomersToInsert:          Will exclude any customer that say "do not call me again"
             2-> ExcludeCustomersWithCalls:     Will remove from the list any customer with a call with the reason 
             3-> JoinCustomerWincosacsAndCalls: Will join customers from wincoacs with sales management
             4-> GetCustomerWithNoCallsSince:   Get sales management customers that did not receive any call since a specific date              
             5-> InactiveDateCash:              Just get a date
            */
            var customersToInsert = GetCustomersToInsert(ExcludeCustomersWithCalls(JoinCustomerWincosacsAndCalls(GetCustomerWithNoCallsSince(InactiveDateCash()), winCosacsData)));
            var dateToContact = GetDateToCall();

            switch (customersInteraction.ContactMeansIdEnum)
            {
                case ContactMeanEnum.Email:
                    InsertMails(customersToInsert, customersInteraction, dateToContact, customersSalesPerson);
                    break;

                case ContactMeanEnum.PhoneCall:
                    InsertCalls(customersToInsert, customersSalesPerson, dateToContact, customersInteraction);
                    break;

                case ContactMeanEnum.Sms:
                    InsertSms(customersToInsert, customersInteraction, dateToContact, customersSalesPerson);
                    break;

                default:
                    throw new Exception("Unknown ContactMean option");
            }

            return Request.CreateResponse();
        }

        internal IList<CustomerLastPurchase> ExcludeCustomersWithCalls(IList<CustomerLastPurchase> allRecords)
        {
            var customerWithCalls = repository.GetCustomerWithCallsFromSource(CallSourceEnum.InactiveCustomers);

            return allRecords
                .Where(p => !customerWithCalls.Contains(p.CustomerId))
                .ToList();
        }

        internal void InsertMails(IList<CustomerLastPurchase> customersToInsert, AdditionalCustomersInteraction customerInteraction, DateTime dateToMail, Dictionary<string, int?> customersSalesPerson)
        {
            repository.InsertMailsToSend(customersToInsert
                .Where(p => !string.IsNullOrWhiteSpace(p.Email))
                .Select(p => new MailsToSend
                {
                    CustomerId = p.CustomerId,
                    DateToSend = dateToMail,
                    MailAdress = p.Email,
                    MailSudject = string.Format(ReasonToCall, p.DateLastPaid.ToString("dd-mm-yyyy")),
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

        internal void InsertCalls(IList<CustomerLastPurchase> customersToInsert, Dictionary<string, int?> customersSalesPerson, DateTime dateToCall, AdditionalCustomersInteraction customerInteraction)
        {
            //customersSalesPerson = GetCustomersSalesParson(new HashSet<string>(customersToInsert.Select(p => p.CustomerId), StringComparer.InvariantCultureIgnoreCase));
            var icon = GetInactiveCustomersCallsIcon();

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
                    ReasonToCall = string.Format(ReasonToCall, p.DateLastPaid.ToString("dd-MM-yy")),
                    SpokeToCustomer = false,
                    ToCallAt = dateToCall,
                    Source = (byte)CallSourceEnum.InactiveCustomers,
                    Icon = icon,
                    AlternativeContactMeanId = customerInteraction.AlternativeContactMeanId,
                    MailchimpTemplateID = customerInteraction.AlternativeContactMeanId.HasValue ? customerInteraction.MailchimpTemplateID : null,
                    EmailSubject = customerInteraction.AlternativeContactMeanId.HasValue ? string.Format(ReasonToCall, p.DateLastPaid.ToString("dd-mm-yyyy")) : null,
                    SmsText = customerInteraction.AlternativeContactMeanId.HasValue ? customerInteraction.SmsText : null,
                    MobileNumber = p.MobileNumber,
                    LandLinePhone = p.LandLinePhone
                })
                .AsParallel<Call>()
                .ToList();

            repository.SaveCalls(calls);
        }

        internal void InsertSms(IList<CustomerLastPurchase> customersToInsert, AdditionalCustomersInteraction customerInteraction, DateTime dateToSend, Dictionary<string, int?> customersSalesPerson)
        {
            repository.InsertSmsToSend(customersToInsert
                .Where(p => !string.IsNullOrWhiteSpace(p.MobileNumber))
                .Select(p => new SmsToSend
                {
                    CustomerId = p.CustomerId,
                    DateToSend = dateToSend,
                    Body = customerInteraction.SmsText,
                    PhoneNumber = p.MobileNumber
                })
                .ToList());

            //for customers with no email call then
            InsertCalls(
                customersToInsert
                    .Where(p => string.IsNullOrWhiteSpace(p.MobileNumber)).ToList(),
                customersSalesPerson,
                dateToSend,
                customerInteraction);
        }

        internal IList<CustomerLastPurchase> JoinCustomerWincosacsAndCalls(HashSet<CustomerLastPurchase> salesMCustomers, IList<CustomerLastPurchase> wincosacsCustomers)
        {
            //if customers are in both sides this will remove the from salesMCustomers
            salesMCustomers.ExceptWith(wincosacsCustomers);

            //now the hole set of unique customers is returned
            return salesMCustomers
                .Union(wincosacsCustomers)
                .ToList();
        }

        internal HashSet<CustomerLastPurchase> GetCustomerWithNoCallsSince(DateTime lastCallDate)
        {
            using (var scope = Context.Read())
            {
                return new HashSet<CustomerLastPurchase>((from c in scope.Context.Call
                                                          join csr in scope.Context.CustomerSalesPerson
                                                              on c.CustomerId equals csr.CustomerId
                                                          where c.CalledAt.HasValue
                                                          group new
                                                          {
                                                              c.CustomerId,
                                                              c.CalledAt.Value,
                                                              SalesPersonId = csr.TempSalesPersonId.HasValue && csr.TempSalesPersonIdBegin <= lastCallDate && csr.TempSalesPersonIdEnd >= lastCallDate ? csr.TempSalesPersonId : csr.SalesPersonId,
                                                              c.CustomerLastName,
                                                              csr.MobileNumber,
                                                              csr.LandLinePhone,
                                                              c.CustomerFirstName,
                                                              c.CalledAt,
                                                              csr.Email
                                                          }
                                                          by c.CustomerId into customers
                                                          where customers.Max(p => p.CalledAt) <= lastCallDate
                                                          select customers)
                                                           .ToList()
                                                           .SelectMany(p => p.ToList())
                                                           .Select(p => new CustomerLastPurchase
                                                           {
                                                               CustomerId = p.CustomerId,
                                                               DateLastPaid = p.CalledAt.Value,
                                                               FirstName = p.CustomerFirstName,
                                                               LastName = p.CustomerLastName,
                                                               LandLinePhone = p.LandLinePhone,
                                                               MobileNumber = p.MobileNumber,
                                                               SalesPerson = p.SalesPersonId.Value,
                                                               Email = !string.IsNullOrEmpty(p.Email) ? p.Email.Trim() : (string)null
                                                           })
                                                           .ToList());
            }
        }

        private DateTime InactiveDateCash()
        {
            var days = cosacsSettings.HowManyDaysToBeAnInactiveCashCustomer;

            return this.clock.Now.AddDays(0 - days);
        }

        private DateTime InactiveDateCredit()
        {
            var days = cosacsSettings.HowManyDaysToBeAnInactiveCreditCustomer;

            return this.clock.Now.AddDays(0 - days);
        }

        private DateTime GetDateToCall()
        {
            var days = cosacsSettings.DaysToScheduleCallToInactiveCustomers;

            return this.clock.Now.AddDays(days);
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

        internal IList<CustomerLastPurchase> GetCustomersToInsert(IList<CustomerLastPurchase> customers)
        {
            var customersSalesPerson = repository.GetCustomersSalesPerson(true, customers.Select(p => p.CustomerId).ToList())
                .Select(p => p.CustomerId)
                .ToList();

            return customers
                .Where(p => !customersSalesPerson.Contains(p.CustomerId))
                .ToList();
        }

        internal void InsertCustomerCsr(IList<CustomerLastPurchase> allCustomers, Dictionary<string, int?> customersSalesPerson)
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
                    CustomerBranch = p.CustomerBranch,
                    Email = p.Email,
                    DoNotCallAgain = false
                })
                .ToList();

            repository.UpdateCustomersSalesPerson(customerNoCsr
                .Where(p => existingCustomers.ContainsKey(p.CustomerId))
                .ToDictionary(p => p.CustomerId));

            repository.InsertCustomersSalesPerson(customerNoCsr
                .Where(p => !existingCustomers.ContainsKey(p.CustomerId))
                .ToList());
        }

        internal string GetInactiveCustomersCallsIcon()
        {
            using (var scope = Context.Write())
            {
                return scope.Context.IconTypes
                    .Select(p => p.Icon)
                    .FirstOrDefault();
            }
        }
    }
}