using System;
using System.Collections.Generic;

namespace Blue.Cosacs.SalesManagement.Api.Models
{
    public sealed class BulkSmsSettings
    {
        public string SmsText { get; set; }
        public DateTime ToSendAt { get; set; }
        public IList<SmsRecipient> Customers { get; set; }
    }

    public class SmsRecipient
    {
        public string CustomerId { get; set; }
        public string PhoneNumber { get; set; }
    }
    
    public sealed class BulkSmsSettingsAll
    {
        public string SmsText { get; set; }
        public DateTime ToSendAt { get; set; }
        public string CustomerFilter { get; set; }
    }
}