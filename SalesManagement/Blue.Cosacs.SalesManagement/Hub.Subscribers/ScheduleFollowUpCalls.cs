using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Hub.Client;
using Blue.Networking;
using System.Xml;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using Blue.Cosacs.SalesManagement.Messages;
using System.Threading.Tasks;

namespace Blue.Cosacs.SalesManagement.Hub.Subscribers
{
    public class ScheduleFollowUpCalls : Subscriber
    {
        private readonly ISalesManagementRepository repository;
        private readonly IHttpClientJson httpClientJson;
        private readonly IClock clock;
        private readonly Blue.Cosacs.SalesManagement.Settings settings;

        public ScheduleFollowUpCalls(IClock clock, ISalesManagementRepository repository, IHttpClientJson httpClientJson, Blue.Cosacs.SalesManagement.Settings settings)
        {
            this.clock = clock;
            this.repository = repository;
            this.httpClientJson = httpClientJson;
            this.settings = settings;
        }

        public override void Sink(int id, XmlReader message)
        {
            var warehouseDeliver = Deserialize<WarehouseDeliver>(message);

            if (string.IsNullOrWhiteSpace(warehouseDeliver.CustomerAccount) || string.Compare(warehouseDeliver.DeliveryOrCollection, "C", true) == 0)
            {
                return;
            }

            var customer = ExternalHttpSources.GetCustomer(httpClientJson, warehouseDeliver.CustomerAccount).First();

            if (customer.TotalAmount >= settings.SalesFollowUpThreshold)
            {
                InsertFollowUpCalls(customer, warehouseDeliver);
            }
        }

        private IList<Contacts> GetData(DateTime deliverDate, string customerEmail)
        {
            var followUpcalls = repository.GetFollowUpCalls();
            var today = this.clock.Now.Date;

            //this select will calculate the correct dates that each call have to be done
            return followUpcalls
                .Select((item, index) =>
                {
                    var delDate = deliverDate;
                    //sum to the to the current row all the previews dates 
                    for (int i = 0; i <= index; i++)
                    {
                        delDate = GetCallDate(followUpcalls[i].ChronologicalTimePeriod, followUpcalls[i].Quantity, delDate);
                    }

                    return new Contacts
                    {
                        ToContactAt = delDate,
                        Icon = item.Icon,
                        ResonToContact = item.ReasonToCall,
                        MailchimpTemplateID = item.MailchimpTemplateID,
                        AlternativeContactMeanId = item.AlternativeContactMeanId.HasValue ? new Nullable<ContactMeanEnum>((ContactMeanEnum)item.AlternativeContactMeanId) : null,
                        Email = customerEmail,
                        ContactMeansId = (ContactMeanEnum)item.ContactMeansId,
                        SmsText = item.SmsText,
                        ContactEmailSubject = item.ContactEmailSubject,
                        FlushedEmailSubject = item.FlushedEmailSubject
                    };
                })
                .Where(p => p.ToContactAt > today)
                .ToList();
        }

        private IList<Call> GetCalls(IList<Contacts> data, Customer customer)
        {
            if (!(string.IsNullOrWhiteSpace(customer.LandLinePhone) && string.IsNullOrWhiteSpace(customer.MobileNumber)))
            {
                return data
                    //contact type = call or email and empty email
                    .Where(p => ((int)p.ContactMeansId == (int)ContactMeanEnum.PhoneCall))
                    .Select(p => new Call
                    {
                        CallClosedReasonId = null,
                        CalledAt = null,
                        SalesPersonId = customer.SalesPerson,
                        CallTypeId = (int)CallTypeEnum.AutoScheduled,
                        Comments = null,
                        CreatedOn = clock.Now,
                        CustomerFirstName = customer.FirstName,
                        CreatedBy = null,
                        CustomerId = customer.CustomerId,
                        CustomerLastName = customer.LastName,
                        PreviousCallId = null,
                        ReasonToCall = p.ResonToContact,
                        Source = (int)CallSourceEnum.FollowUpCalls,
                        SpokeToCustomer = false,
                        ToCallAt = p.ToContactAt,
                        Icon = p.Icon,
                        AlternativeContactMeanId = p.AlternativeContactMeanId.HasValue ? new Nullable<byte>((byte)p.AlternativeContactMeanId) : null,
                        MailchimpTemplateID = p.AlternativeContactMeanId.HasValue ? p.MailchimpTemplateID : null,
                        EmailSubject = p.AlternativeContactMeanId.HasValue ? p.FlushedEmailSubject : null,
                        SmsText = p.AlternativeContactMeanId.HasValue ? p.SmsText : null,
                        LandLinePhone = customer.LandLinePhone,
                        MobileNumber = customer.MobileNumber
                    })
                    .ToList();
            }

            return null;
        }

        private IList<MailsToSend> GetEmails(IList<Contacts> data, Customer customer)
        {
            return data
                .Where(p => p.MailchimpTemplateID.HasValue && !string.IsNullOrWhiteSpace(p.Email) && (int)p.ContactMeansId == (int)ContactMeanEnum.Email)
                .Select(p => new MailsToSend
                {
                    OverrideUnsubscribe = true,
                    CustomerId = customer.CustomerId,
                    DateToSend = p.ToContactAt,
                    MailSudject = p.ContactEmailSubject,
                    TemplateId = p.MailchimpTemplateID.Value,
                    MailAdress = p.Email,
                    CustomerName = string.Format("{0} {1}", customer.FirstName, customer.LastName)
                })
                .ToList();
        }

        private IList<SmsToSend> GetSms(IList<Contacts> data, string phoneNumber, string customerId)
        {
            return data
                .Where(p => (int)p.ContactMeansId == (int)ContactMeanEnum.Sms)
                .Select(p => new SmsToSend
                {
                    Body = p.SmsText,
                    CustomerId = customerId,
                    DateToSend = p.ToContactAt,
                    PhoneNumber = phoneNumber
                })
                .ToList();
        }

        internal void InsertFollowUpCalls(Customer customer, WarehouseDeliver warehouseDeliver)
        {
            var callDats = GetData(warehouseDeliver.Date, customer.Email);
            //return string.Format("{0}{1}{2}", PhoneDialCode, PhoneExtension, PhoneNumber)

            IList<SmsToSend> sms = null;
            IList<MailsToSend> emails = null;

            if (!string.IsNullOrWhiteSpace(customer.MobileNumber))
            {
                sms = GetSms(callDats, customer.MobileNumber, customer.CustomerId);
            }
            else
            {
                //no mobile number? set the contact as phone call
                foreach (var item in callDats.Where(p=> p.ContactMeansId == ContactMeanEnum.Sms))
                {
                    item.ContactMeansId = ContactMeanEnum.PhoneCall;
                }
            }

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                emails = GetEmails(callDats, customer);
            }
            else
            {
                //no email? set the contact as phone call
                foreach (var item in callDats.Where(p => p.ContactMeansId == ContactMeanEnum.Email))
                {
                    item.ContactMeansId = ContactMeanEnum.PhoneCall;
                }
            }

            repository.ScheduleFollowUpCalls(
                customer,
                GetCalls(callDats, customer),
                emails,
                sms);
        }

        private void AddCustomer(Customer customer)
        {
            repository.InsertCustomersSalesPerson(new List<CustomerSalesPerson>
            {
                new CustomerSalesPerson
                {
                    CustomerBranch = short.Parse(customer.CustomerAccount.Substring(0,3)),
                    CustomerId= customer.CustomerId,
                    DoNotCallAgain = false,
                     Email = customer.Email,
                    SalesPersonId = customer.SalesPerson,
                    TempSalesPersonId = null,
                    TempSalesPersonIdBegin = null, 
                    TempSalesPersonIdEnd = null,
                }
            });
        }

        internal DateTime GetCallDate(FollowUpCallsTimePeriods timePeriod, short timeSpam, DateTime deliverDate)
        {
            switch (timePeriod)
            {
                case FollowUpCallsTimePeriods.Day:
                    return deliverDate.AddDays((int)timeSpam).Date;

                case FollowUpCallsTimePeriods.Week:
                    return deliverDate.AddDays((int)timeSpam * 7).Date;

                case FollowUpCallsTimePeriods.Month:
                    return deliverDate.AddMonths((int)timeSpam).Date;

                default:
                    throw new NotImplementedException("No default value for timePeriod. Current value" + (int)timePeriod);
            }
        }

        private class Contacts
        {
            public DateTime ToContactAt { get; set; }
            public string Icon { get; set; }
            public string ResonToContact { get; set; }
            public short? MailchimpTemplateID { get; set; }
            public ContactMeanEnum? AlternativeContactMeanId { get; set; }
            public string Email { get; set; }
            public ContactMeanEnum ContactMeansId { get; set; }
            public string SmsText { get; set; }
            public string ContactEmailSubject { get; set; }
            public string FlushedEmailSubject { get; set; }
        }
    }
}
