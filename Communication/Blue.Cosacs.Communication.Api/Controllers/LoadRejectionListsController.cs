using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Blue.Cosacs.Communication.MailsHandlers;
using Blue.Cosacs.Communication.Repositories;
using Blue.Glaucous.Client.Api;
using System.Linq;

namespace Blue.Cosacs.Communication.Api.Controllers
{
    public class LoadRejectionListsController : ApiController
    {
        private readonly ICommunicationRepository repository;

        public LoadRejectionListsController(ICommunicationRepository repository)
        {
            this.repository = repository;
        }

        [CronJob]
        [Permission(CommunicationPermissionEnum.LoadEmailUnsubscribeList)]
        public HttpResponseMessage Get()
        {
            var providers = StructureMap.ObjectFactory.Container.GetAllInstances<IEmail>();
            var rejected = new ConcurrentBag<BlackEmailList>();

            Parallel.ForEach<IEmail>(providers,  (current) =>
                {
                    foreach (var item in current.GetRejected())
                    {
                        rejected.Add(item);
                    }
                });

            repository.InsertBlackEmailList(rejected.ToList());
            
            return Request.CreateResponse();
        }
    }
}
