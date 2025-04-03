using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using Unicomer.Cosacs.Business;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.WebApi.Controllers
{
    [RoutePrefix("api/SyncService")]
    public class SyncServiceController : ApiController
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ISyncData _ISyncData;
        private readonly IErrorResponse _IErrorResponse;
        public SyncServiceController()
        {
            _ISyncData = new SyncServiceData();
            _IErrorResponse = new JResponseError();
        }

        // GET: SyncService
        [AcceptVerbs("Get")]
        [Route("getSyncServiceData")]
        public HttpResponseMessage getSyncServiceData()
        {
            try
            {
                string result = _ISyncData.getSyncServiceData();
                //if (result != null && result.StatusCode.Equals(200) && !string.IsNullOrWhiteSpace(result.StatusCode))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);  //_IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }

    }
}