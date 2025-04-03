using System.Linq;
using System.Web.Mvc;
using Domain = Blue.Cosacs.Service;
using Blue.Service;
using System.Data.Entity;
using Blue.Events;
using Blue.Glaucous.Client.Mvc;

namespace Blue.Cosacs.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// AddressMasterController
    /// </summary>
    public class AddressMasterController : FormInlineBaseController<Domain.Context, Domain.AddressMaster, Models.AddressMaster, AddressMasterController.Search>
    {
        private IEventStore audit;
        public AddressMasterController(IEventStore audit)
        {
            this.audit = audit;
        }

        [Permission(ServicePermissionEnum.AddressMasterManage)]
        public override ActionResult Index(int? page, Search s)
        {
            ViewData.Add("DefaultNew", DefaultNew());
            return View(ToModels(Load(s)));
        }

        public class Search
        {
            public int? Id { get; set; }

            public string Region { get; set; }
            public string Village { get; set; }
        }

        protected override Transactions.WriteScope<Domain.Context> Write()
        {
            return Domain.Context.Write();
        }

        protected override Transactions.ReadScope<Domain.Context> Read()
        {
            return Domain.Context.Read();
        }

        protected override Models.AddressMaster ToModel(Domain.AddressMaster d)
        {
            return new Models.AddressMaster(d);
        }

        protected override DbSet<Domain.AddressMaster> Query(Domain.Context ctx)
        {
            return ctx.AddressMaster;
        }

        protected override IQueryable<Domain.AddressMaster> Filter(IQueryable<Domain.AddressMaster> query, Search search)
        {
            if (search == null)
                return query;

            if (search.Id != null)
                query = query.Where(q => q.Id == search.Id.Value);

            if (!string.IsNullOrEmpty(search.Region))
                query = query.Where(q => q.Region.Contains(search.Region));

            if (!string.IsNullOrEmpty(search.Village))
                query = query.Where(q => q.Village.Contains(search.Village));

            return query;
        }

        protected override void OnUpdating(Domain.Context ctx, Models.AddressMaster m, Domain.AddressMaster d)
        {
            audit.LogAsync(new
            {
                old = d,
                @new = m
            }, Domain.EventType.AddressMasterUpdate, Domain.EventCategory.AddressMaster);
            Fill(m, d);

        }

        protected override void OnCreating(Domain.Context ctx, Models.AddressMaster m, Domain.AddressMaster d)
        {
            Fill(m, d);
        }

        protected override void OnCreated(Domain.Context ctx, Models.AddressMaster m, Domain.AddressMaster d)
        {
            m.Id = d.Id;
            audit.LogAsync(new
            {
                @new = m
            }, Domain.EventType.AddressMasterCreate, Domain.EventCategory.AddressMaster);
        }

        private void Fill(Models.AddressMaster m, Domain.AddressMaster d)
        {
            d.Region = m.Region;
            d.Village = m.Village;
            d.ZipCode = m.ZipCode;
        }

        protected override void OnDeleting(Domain.AddressMaster d, Domain.Context ctx)
        {
            audit.LogAsync(new
            {
                driver = d
            }, Domain.EventType.AddressMasterDelete, Domain.EventCategory.AddressMaster);
        }
    }
}
