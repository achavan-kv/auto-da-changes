using Newtonsoft.Json.Serialization;
using System;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Api.Bootstrapper.Initialize();

            GlobalConfiguration.Configure(Blue.Cosacs.Credit.Api.WebApiConfig.Register);
            GlobalConfiguration.Configure(Blue.Glaucous.Client.WebApiConfig.Register);

            GlobalConfiguration.Configuration
              .Formatters
              .JsonFormatter
              .SerializerSettings
              .ContractResolver = new DefaultContractResolver();
        }
    }
}