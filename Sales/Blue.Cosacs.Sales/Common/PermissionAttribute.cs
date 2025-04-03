using System;
using System.Web.Mvc;
using System.Web;

namespace Blue.Cosacs.Sales.Common
{
    public class PermissionAttribute : FilterAttribute, IAuthorizationFilter
    {
        public PermissionAttribute(object permission)
        {
            if ((permission as System.Enum) == null)
                throw new ArgumentException("Attribute must be a System.Enum.", "permission");

            this.permission = (int)permission;
        }
        private readonly int permission;

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            //if (!filterContext.HttpContext.GetUser().HasPermission(this.permission))
            //{
            //    var container = StructureMap.ObjectFactory.Container;
            //    throw new Blue.Admin.PermissionException(container, permission);
            //}
        }
    }
}