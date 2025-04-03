using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace Blue.Cosacs.Payments.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(Blue.Cosacs.Payments.Api.WebModuleConfig.Register);
            GlobalConfiguration.Configure(Blue.Glaucous.Client.WebApiConfig.Register);
            //AreaRegistration.RegisterAllAreas();
        }
    }
}
