using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace Blue.Cosacs.File.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Api.Bootstrapper.Initialize();

            GlobalConfiguration.Configure(Blue.Cosacs.File.Api.WebApiConfig.Register);
            GlobalConfiguration.Configure(Blue.Glaucous.Client.WebApiConfig.Register);

            GlobalConfiguration.Configuration
              .Formatters
              .JsonFormatter
              .SerializerSettings
              .ContractResolver = new DefaultContractResolver();
        }
    }
}