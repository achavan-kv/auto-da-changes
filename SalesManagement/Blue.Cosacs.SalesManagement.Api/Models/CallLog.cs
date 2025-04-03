using Newtonsoft.Json;
using System;

namespace Blue.Cosacs.SalesManagement.Api.Models
{
    public class CallLog
    {
        public int Id { get; set; }
        public DateTime CalledAt { get; set; }
        public bool SpokeToCustomer { get; set; }
        public string Comments { get; set; }
        public DateTime? ScheduleCallback { get; set; }
        public string ReasonToCallAgain { get; set; }
        public string PreviousReasonToCall { get; set; }
        public int[] PendingCalls { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerId { get; set; }
        public int? SalesPersonId { get; set; }
        public bool DoNotCallAgain { get; set; }
        public int? CreatedBy { get; set; }
        public string MobileNumber { get; set; }
        public string LandLinePhone { get; set; }
        public byte CallTypeId { get; set; }
        public int? CalledBy { get; set; }
        public int? CurrentUser { get; set; }
        public string EmailSubject { get; set; }
        public short? MailchimpTemplateID { get; set; }
        public string SmsText { get; set; }
        public byte? AlternativeContactMeanId { get; set; }
    }
}
