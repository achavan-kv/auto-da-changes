using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blue.Cosacs.SalesManagement.Api.Models
{
    public class InactiveCustomersInteraction
    {
        public Blue.Cosacs.SalesManagement.AdditionalCustomersInteraction ToEntitySet()
        {
            return new Blue.Cosacs.SalesManagement.AdditionalCustomersInteraction
            {
                Id = Id,
                MailchimpTemplateID = MailchimpTemplateID,
                AlternativeContactMeanId = AlternativeContactMeanId,
                ContactMeansId = ContactMeansId,
                SmsText = SmsText,
                ContactEmailSubject = ContactEmailSubject,
                FlushedEmailSubject = FlushedEmailSubject
            };
        }

        public byte Id { get; set; }
        public short? MailchimpTemplateID { get; set; }
        public string SmsText { get; set; }
        public byte ContactMeansId { get; set; }
        public byte? AlternativeContactMeanId { get; set; }
        public string ContactEmailSubject { get; set; }
        public string FlushedEmailSubject { get; set; }
    }
}