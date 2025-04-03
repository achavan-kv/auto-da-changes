/* 
Author: Suresh -IGT
Date: Feb 1st
Description:JM BlueStart-Pre-Qualification API CamelCaseFormatter
 */
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http.Controllers;

namespace Blue.Cosacs.Unipay.Web.Formatters
{
    public class JsonFormatterAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            var formatter = controllerSettings.Formatters.JsonFormatter;

            controllerSettings.Formatters.Remove(formatter);

            formatter = new JsonMediaTypeFormatter
            {
                SerializerSettings =
                                    {
                                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                                    }
            };

            controllerSettings.Formatters.Insert(0, formatter);
        }
    }
}