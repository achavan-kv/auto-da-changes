using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Blue.Cosacs.MVE.Web.Controllers
{
    [RoutePrefix("api/Settings")]
    public class SettingsController : ApiController
    {
        [AcceptVerbs("GET")]
        [Route("Metadata")]
        public HttpResponseMessage Get()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent("<h2>No preview available.</h2>");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}
