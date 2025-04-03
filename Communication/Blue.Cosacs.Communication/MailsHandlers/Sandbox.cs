using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blue.Cosacs.Communication.Repositories;

namespace Blue.Cosacs.Communication.MailsHandlers
{
    internal sealed class Sandbox : IEmail
    {
        private readonly ICommunicationRepository repository;
        private readonly IClock clock;

        public Sandbox(ICommunicationRepository repository, IClock clock)
        {
            this.repository = repository;
            this.clock = clock;
        }

        public IList<BlackEmailList> GetRejected()
        {
            return new List<BlackEmailList>();
        }

        public void Send(Messages.MailMessage mailMessage)
        {
            repository.InsertSandBoxMails(new SandBoxMails
            {
                CreatedOn = clock.Now,
                MailMessage = mailMessage.ToJson()
            });
        }
    }
}
