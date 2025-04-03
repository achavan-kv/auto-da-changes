using System.Collections.Generic;
using Blue.Cosacs.Communication.Messages;

namespace Blue.Cosacs.Communication.MailsHandlers
{
    public interface IEmail
    {
        void Send(MailMessage mailMessage);
        IList<BlackEmailList> GetRejected();
    }
}
