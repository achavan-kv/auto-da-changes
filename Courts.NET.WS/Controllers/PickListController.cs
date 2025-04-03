using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blue;

namespace Cosacs.Web.Controllers
{
    public class PickListController : Controller
    {
        private readonly IClock clock;

        public PickListController()
        {
            clock = StructureMap.ObjectFactory.Container.GetInstance<IClock>();
        }

        [HttpGet]
        public JsonResult ListDetails(string id)
        {
            var provider = StructureMap.ObjectFactory.Container.GetInstance<IPickListProvider>(id);
            return Json(
                new
            {
                id = id,
                list = provider.Load()
            },
            JsonRequestBehavior.AllowGet);
        }

        //[Permission(AdminPermissionEnum.ViewPickLists)]
        [HttpGet]
        public ActionResult Load(string ids)
        {
            return Load(ids.Split(',').Where(s => s.Trim().Length > 0).ToArray());
        }

        //[Permission(AdminPermissionEnum.ViewPickLists)]
        [HttpPost]
        public ActionResult Load(string[] ids)
        {
            var providers = StructureMap.ObjectFactory.Container.GetAllInstances<IPickListProvider>().ToDictionary(p => p.Id);
            var result = new Dictionary<string, object>(ids.Length);
            foreach (var id in ids)
            {
                var provider = providers[id];
                if (provider == null)
                {
                    return new HttpNotFoundResult("There is no PickList provider with ID = " + id);
                }

                result[id] = new
                {
                    timestamp = clock.UtcNow.Ticks,
                    rows = ToDictionary(provider.Load())
                };
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private IDictionary<string, string> ToDictionary(IEnumerable<IPickListRow> rows)
        {
            return rows.ToDictionary(r => r.k, r => r.v);
        }
    }
}
