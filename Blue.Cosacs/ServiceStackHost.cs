using System;
using Funq;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Common.Utils;
using ServiceStack.Configuration;
using ServiceStack.DataAccess;
using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.WebHost.Endpoints;
using System.Reflection;
using ServiceStack.ServiceHost;

namespace Blue.Cosacs
{
    public class ServiceStackHost : AppHostBase
    {
        public ServiceStackHost() : base("Cosacs Service Stack", Assembly.GetExecutingAssembly())
        {
        }

        public override object ExecuteService(object requestDto)
        {
            return base.ExecuteService(requestDto);
        }

        public override void Configure(Container container)
        {
            //Configure User Defined REST Paths
            //Routes
            //  .Add<Hello>("/hello")
            //  .Add<Hello>("/hello/{Name*}")
            //  .Add<Todo>("/todos")
            //  .Add<Todo>("/todos/{Id}");

            //var disableFeatures = Feature.Jsv | Feature.Soap | Feature.Csv | Feature.Html | Feature.Xml;
            SetConfig(new EndpointHostConfig
            {
                EnableFeatures = Feature.Json | Feature.Metadata, //.All.Remove(disableFeatures),
                DebugMode = true, //Show StackTraces when developing
            });
        }
    }

}