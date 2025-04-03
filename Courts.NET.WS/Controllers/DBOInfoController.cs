using Blue.Cosacs.Model;
using Blue.Cosacs.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Cosacs.Web.Controllers
{
    public class DBOInfoController : Controller
    {
        public JsonResult Branches()
        {
            var branches = GetBranchInfo();

            return Json(branches, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Branch(int id)
        {
            var branches = GetBranchInfo();

            if (id > 0)
            {
                return Json(
                    branches.Where(e => e.BranchNumber == id),
                    JsonRequestBehavior.AllowGet);
            }

            return Json(branches, JsonRequestBehavior.AllowGet);
        }

        private List<BranchInfo> GetBranchInfo()
        {
            var repository = new BranchRepository();

            return repository.GetBranchInfo();
        }
    }
}
