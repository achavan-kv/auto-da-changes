using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blue.Cosacs.SalesManagement.Api.Models
{
    public sealed class BulkMailSettings
    {
        public List<Recipient> Customers
        {
            get;
            set;
        }
        public DateTime ToMailAt
        {
            get;
            set;
        }
        public short MailchimpTemplateID
        {
            get;
            set;
        }
        public string Body
        {
            get;
            set;
        }
        public string Subject
        {
            get;
            set;
        }
    }

    public class Recipient
    {
        public string Address
        {
            get;
            set;
        }
        public string Id
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
    }

    public class BulkMailSettingsAll
    {
        public string CustomerFilter { get; set; }
        public DateTime ToMailAt
        {
            get;
            set;
        }
        public short MailchimpTemplateID
        {
            get;
            set;
        }
        public string Body
        {
            get;
            set;
        }
        public string Subject
        {
            get;
            set;
        }
    }
}