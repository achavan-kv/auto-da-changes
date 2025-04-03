using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Unicomer.Cosacs.Business;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.WebApi.Areas.Vendor
{
    [RoutePrefix("api/Vendor")]
    public class VendorController : ApiController
    {
        private readonly IVendor _IVendor;
        private readonly ICustomer _ICustomer;
        private readonly IErrorResponse _IErrorResponse;
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public VendorController()
        {
            _IVendor = new VendorBusiness();
            _ICustomer = new CustomerBusiness();
            _IErrorResponse = new JResponseError();
        }

        /// <summary>
        /// Send Vendor Master Data Information
        /// </summary>
        /// <param name="objUser"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Route("GetVendor")]
        public dynamic GetSupplierMaster()
        {
            MVEWebClient mVEWebClient = null;
            try
            {
                string aPIResult = string.Empty;
                JResponse result = _IVendor.GetSupplierMaster();
                if (result != null && result.StatusCode.Equals(200) && !string.IsNullOrWhiteSpace(result.Result))
                {
                    _log.Info("Info : Send Vendor Master info Json to MVE : " + result.Result);
                    string postUrl = string.Format("{0}/{1}{2}", "v1", "Vendors", string.Empty);
                    mVEWebClient = new MVEWebClient(postUrl);
                    aPIResult = mVEWebClient.ExecuteWebClient("POST", "application/json; charset=utf-8", result.Result, true);
                    _log.Info("Info : Send Vendor Master info Json to MVE Result : " + aPIResult);
                }
                return Request.CreateResponse(HttpStatusCode.Created, aPIResult);
            }
            catch (WebException wEx)
            {
                if (wEx.Message.Contains("401"))
                {
                    mVEWebClient = new MVEWebClient();
                    mVEWebClient.GetToken();
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
                }
                else
                {
                    using (var stream = wEx.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _IErrorResponse.CreateExceptionResponse((ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message)) ? string.Format("{0}\n{1}", ex.Message, ex.InnerException.Message) : ex.Message);
            }
        }

        [AcceptVerbs("Put")]
        [Route("UpdateSupplier")]
        public dynamic UpdateSupplierMaster([FromBody]UpdateSupplier objUpdateSupplier)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return _IVendor.UpdateSupplierMaster(objUpdateSupplier);
                }
                else
                {
                    return _IErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }

        [AcceptVerbs("Get")]
        [Route("GetVendorEOD")]
        public dynamic getSupplierEOD([FromUri]int spanInMinutes)
        {
            MVEWebClient mVEWebClient = null;
            try
            {
                List<SupplierMaster> result = _IVendor.GetSupplierEOD(spanInMinutes);
                string aPIResult = string.Empty;
                if (result != null && result.Count > 0)
                {
                    foreach (SupplierMaster supplier in result)
                    {
                        string putUrl = string.Format("{0}/{1}{2}", "v1", "Vendors", string.Format("/{0}", supplier.ExternalVendorID));
                        mVEWebClient = new MVEWebClient(putUrl);
                        string jsonBody = JsonConvert.SerializeObject(supplier);
                        aPIResult = mVEWebClient.ExecuteWebClient("PUT", "application/json; charset=utf-8", jsonBody, true);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, aPIResult); //_IErrorResponse.CreateExceptionResponse(ex.Message);
            }
            catch (WebException wEx)
            {
                if (wEx.Message.Contains("401"))
                {
                    mVEWebClient = new MVEWebClient();
                    mVEWebClient.GetToken();
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
                }
                else
                {
                    using (var stream = wEx.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);  //_IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }
    }
}