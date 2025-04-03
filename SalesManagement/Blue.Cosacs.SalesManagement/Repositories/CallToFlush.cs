namespace Blue.Cosacs.SalesManagement.Repositories
{
    public sealed class CallToFlush
    {
        public int Id { get; set; }
        public byte? AlternativeContactMeanId { get; set; }
        public short? MailchimpTemplateID { get; set; }
        public string EmailSubject { get; set; }
        public string SmsText { get; set; }
        public string CustomerId { get; set; }
        public string MobileNumber { get; set; }
        public string LandLinePhone { get; set; }
        public string Email { get; set; }

        public ContactMeanEnum AlternativeContactMeanIdEnum
        {
            get 
            {
                if (this.AlternativeContactMeanId.HasValue)
                {
                    return (ContactMeanEnum)this.AlternativeContactMeanId.Value;
                }

                return ContactMeanEnum.None;
            }
        }
    }
}
