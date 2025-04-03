using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blue.Cosacs.Communication.Messages;

namespace Blue.Cosacs.Communication.Repositories
{
    public interface ICommunicationRepository
    {
        /*MailchimpTemplateID*/
        IList<MailchimpTemplateID> GetMailchimpTemplateID(int? id = null);
        void SaveMailchimpTemplateID(MailchimpTemplateID templateId);
        bool DeleteMailchimpTemplateID(MailchimpTemplateID id);
        bool DeleteMailchimpTemplateID(int id);

        void InsertSandBoxMails(SandBoxMails value);

        /*Email*/
        IList<BlackEmailList> GetEmailUnsubcription(IList<string> emails);
        void InsertBlackEmailList(BlackEmailList value);
        void InsertBlackEmailList(IList<BlackEmailList> value);

        void InsertSmsToSend(IList<SmsToSend> sms);
        int GetTotalSmsNotSent();
        IList<Tuple<DateTime, int>> GetPreviousExports(int totalToReturn);
        IList<Tuple<string, string, string>> GetExport(DateTime? exportedOn);

        /*SmsUnsubcription*/
        void InsertSmsUnsubcription(SmsUnsubcription value);
        void InsertSmsUnsubcription(CustomerPhoneNumbers value);
        IList<SmsUnsubcription> GetSmsUnsubcription(IList<string> customerId);
        void UpdateSmsUnsubcription(SmsUnsubcription value);
        void DeleteSmsUnsubcription(string customerId);

        /*CustomerInteraction*/
        void UpdateCustomerInteraction(List<CustomerInteraction> customers);
    }
}