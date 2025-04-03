using System.Net.Http.Headers;
using Blue.Cosacs.Sales.Api.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web.Http;

namespace Blue.Cosacs.Sales.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

//            config.EnableCors();

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Formatters.Add(new BrowserJsonFormatter());
            ((DefaultContractResolver)config.Formatters.JsonFormatter.SerializerSettings.ContractResolver)
                .IgnoreSerializableAttribute = true;

            GlobalConfiguration.Configuration.Formatters.JsonFormatter
                .SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            config.Formatters.XmlFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html")); 

            config.Routes.MapHttpRoute(
                "API Default",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                "Print Default",
                "print/{controller}/{action}/{invoiceNo}",
                new { action = "Get", invoiceNo = RouteParameter.Optional });

            Sales.Common.Mapping.MapEntites();
        }
    }
}
