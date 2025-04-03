using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace Blue.Cosacs.Customer.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Api.Bootstrapper.Initialize();

            GlobalConfiguration.Configure(Blue.Cosacs.Customer.Api.WebApiConfig.Register);
            GlobalConfiguration.Configure(Blue.Glaucous.Client.WebApiConfig.Register);
        }
    }
}