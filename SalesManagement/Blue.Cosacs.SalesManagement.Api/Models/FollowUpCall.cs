namespace Blue.Cosacs.SalesManagement.Api.Models
{
    public partial class FollowUpCall
    {
        public short? Id { get; set; }
        public byte TimePeriod { get; set; }
        public short Quantity { get; set; }
        public string ReasonToCall { get; set; }
        public string Icon { get; set; }
        public byte? AlternativeContactMeanId { get; set; }
        public short? MailchimpTemplateID { get; set; }
        public string SmsText { get; set; }
        public byte ContactMeansId { get; set; }
        public string ContactEmailSubject { get; set; }
        public string FlushedEmailSubject { get; set; }

        public Blue.Cosacs.SalesManagement.FollowUpCall ToEntitySet()
        {
            return new SalesManagement.FollowUpCall
            {
                Id = Id ?? 0,
                TimePeriod = TimePeriod,
                Quantity = Quantity,
                ReasonToCall = ReasonToCall,
                Icon = Icon,
                MailchimpTemplateID = MailchimpTemplateID,
                AlternativeContactMeanId = AlternativeContactMeanId,
                SmsText = SmsText,
                ContactMeansId = ContactMeansId,
                ContactEmailSubject = ContactEmailSubject,
                FlushedEmailSubject = FlushedEmailSubject
            };
        }
    }
}