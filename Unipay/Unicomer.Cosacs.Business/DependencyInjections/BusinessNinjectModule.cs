/* 
Author: Suresh -IGT
Date: Feb 1st
Description:JM BlueStart-Pre-Qualification API 
 */
using MediatR;
using Ninject.Modules;
using Ninject.Extensions.Conventions;
using Unicomer.Cosacs.Business.Interfaces;
using Flurl.Http.Configuration;
using Flurl.Http;
using Unicomer.Cosacs.Business.Factories;
using System.Net;
using Unicomer.Cosacs.Business.Behaviors;
using FluentValidation;
using System.Collections.Generic;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Business.Implementations;

namespace Unicomer.Cosacs.Business.DependencyInjections
{
    public class BusinessNinjectModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind(x => x.FromThisAssembly()
                 .SelectAllClasses()
                 .InheritedFrom(typeof(IRequestHandler<,>))
                 .BindAllInterfaces());
            var types = AssemblyScanner
                .FindValidatorsInAssembly(typeof(BusinessNinjectModule).Assembly);
            foreach (var type in types)
            {
                this.Bind(type.InterfaceType).To(type.ValidatorType);
            }
            //this.Bind(typeof(IValidator<>)).
            this.Bind(typeof(IPipelineBehavior<,>)).To(typeof(ValidationBehavior<,>));

            this.Bind<IHttpClientService>().To<Implementations.HttpClientService>();
            this.Bind<IFlurlClientFactory>().To<PerBaseUrlFlurlClientFactory>();
            this.Bind<IJsonSerializer>().To<JsonSerializer>();

            FlurlHttp.Configure(settings => {
                settings.HttpClientFactory = new MambuHttpClientFactory();
            });

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }
    }
}
