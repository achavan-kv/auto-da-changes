using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace Blue.Cosacs.Customer.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            //config.Formatters.Add(new BrowserJsonFormatter());

            Bootstrapper.Initialize();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            config.Services.Replace(typeof(IHttpControllerActivator), new ServiceActivator(config));
        }
    }
}
