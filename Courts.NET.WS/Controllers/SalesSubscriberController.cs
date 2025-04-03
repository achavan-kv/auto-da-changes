using Blue.Cosacs.Repositories;

namespace Cosacs.Web.Controllers
{
    public class SalesSubscriberController : Cosacs.Web.Controllers.HttpHubSubscriberController<Blue.Cosacs.Messages.Order>
    {
        private readonly SalesSubscriberRepository repository;

        public SalesSubscriberController()
        {
            this.repository = new SalesSubscriberRepository();
        }

        protected override void Sink(int id, Blue.Cosacs.Messages.Order message)
        {
            repository.SaveSalesOrders(message);
        }
    }
}