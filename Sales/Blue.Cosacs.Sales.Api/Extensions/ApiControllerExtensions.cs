using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using RazorEngine;
using RazorEngine.Templating;
using Encoding = System.Text.Encoding;

namespace Blue.Cosacs.Sales.Api.Extensions
{
    public static class ApiControllerExtensions
    {
        private const string PrintTemplatePattern = @"~/Print/{0}.cshtml";

        public static HttpResponseMessage GetJsonResponseMessage(this ApiController controller, string json)
        {
            var response = controller.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }

        public static string RenderViewToString(this ApiController controller, string view, object model)
        {
            var viewString = string.Format("Cannot find template file for the view '{0}'", view);

            try
            {
                var filePath = string.Format(PrintTemplatePattern, view);
                var path = HostingEnvironment.MapPath(filePath);

                if (string.IsNullOrEmpty(path))
                {
                    throw new Exception(viewString);
                }

                var template = File.ReadAllText(path);

                viewString = Engine.Razor.RunCompile(template, view + DateTime.Now.Ticks, null, model);
            }
            catch (FileNotFoundException)
            {
                //do nothing
            }
            catch (Exception ex)
            {
                viewString = ex.Message;
            }

            return viewString;
        }

        public static bool CheckViewExists(this ApiController controller, string view)
        {
            var filePath = string.Format(PrintTemplatePattern, view);
            var path = HostingEnvironment.MapPath(filePath);

            return File.Exists(path);
        }
    }
}