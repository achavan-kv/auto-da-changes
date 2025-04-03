using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using Blue.Cosacs.Sales.Api;
using Blue.Glaucous.Client.Controllers;
using Newtonsoft.Json.Serialization;

namespace Blue.Cosacs.Sales.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Bootstrapper.Initialize();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(Blue.Glaucous.Client.WebApiConfig.Register);

            // return Json object with camel case properties
            GlobalConfiguration.Configuration
            .Formatters
            .JsonFormatter
            .SerializerSettings
            .ContractResolver = new MySpecialContractResolver();
        }

        protected void Application_PostAuthorizeRequest()
        {
            System.Web.HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }
    }

    public class MySpecialContractResolver : DefaultContractResolver
    {
        private CamelCasePropertyNamesContractResolver camel = new CamelCasePropertyNamesContractResolver();

        public override JsonContract ResolveContract(System.Type type)
        {
            if (!(type == typeof(Blue.Config.SettingMetadata)
                || type == typeof(Blue.Config.SettingMetadata.@decimal)
                || type == typeof(Blue.Config.SettingMetadata.@string)
                || type == typeof(Blue.Config.SettingMetadata.@enum)
                || type == typeof(Blue.Config.SettingMetadata.@int)
                || type == typeof(Blue.Config.SettingMetadata.bit)
                || type == typeof(Blue.Config.SettingMetadata.codeList)
                || type == typeof(Blue.Config.SettingMetadata.date)
                || type == typeof(Blue.Config.SettingMetadata.dateTime)
                || type == typeof(Blue.Config.SettingMetadata.image)
                || type == typeof(Blue.Config.SettingMetadata.list)
                || type == typeof(Blue.Config.SettingMetadata.text)
                || type == typeof(Blue.Cosacs.Sales.Module)
                || type == typeof(System.Linq.Lookup<string, Blue.Glaucous.Client.Controllers.SettingValues>)
                || type.ToString() == "System.Linq.Lookup`2+Grouping[System.String,Blue.Glaucous.Client.Controllers.SettingValues]"
                || type == typeof(Blue.Config.ModuleBase)
                || type == typeof(Setting)
                || type == typeof(SettingValues)
                || type == typeof(List<IGrouping<string, SettingValues>>)
                || type == typeof(IGrouping<string, SettingValues>)))
            {
                return camel.ResolveContract(type);
            }
            return base.ResolveContract(type);
        }
    }
}
