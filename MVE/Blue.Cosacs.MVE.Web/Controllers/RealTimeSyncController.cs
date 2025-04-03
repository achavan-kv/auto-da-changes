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
    [RoutePrefix("api/RealTimeSync")]
    public class RealTimeSyncController : ApiController
    {
        string strGetResponse = string.Empty;
        string flag = "false";
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IVendor _IVendor;
        private readonly ITransaction _ITRansaction;

        public RealTimeSyncController()
        {
            _IVendor = new VendorBusiness();
            _ITRansaction = new TransactionBusiness();
            log4net.GlobalContext.Properties["LogFileName"] = ConfigurationManager.AppSettings["LogFolderPath"] != null ? ConfigurationManager.AppSettings["LogFolderPath"] : @"C://MVE_Unicomer//Log//VE_COSACS_";
            log4net.Config.XmlConfigurator.Configure();
        }

        [AcceptVerbs("Get")]
        [Route("RealTimeSynchronize")]
        public dynamic Get(bool isInsertRecord, string serviceCode, string code,string ID)
        {
            MVEWebClient mVEWebClient = null;
            switch (serviceCode)
            {
                case "vdr":
                    try
                    {
                        List<SupplierMaster> result = _IVendor.GetSupplierRTS(code);
                        string aPIResult = string.Empty;
                        string jsonBody = string.Empty;
                        if (result != null && result.Count > 0)
                        {
                            string putUrl = string.Empty;
                            if (isInsertRecord)
                            {
                                foreach (SupplierMaster supplier in result)
                                {
                                    try
                                    {
                                        putUrl = string.Format("{0}/{1}{2}", "v1", "Vendors", string.Format(""));
                                        mVEWebClient = new MVEWebClient(putUrl);
                                        List<SupplierMaster> listObj = new List<SupplierMaster>();
                                        listObj.Add(supplier);
                                        jsonBody = JsonConvert.SerializeObject(listObj);
                                        _log.Info("Info" + " - " + " Vendor POST jsonBody : " + jsonBody);
                                        aPIResult = mVEWebClient.ExecuteWebClient("POST", "application/json; charset=utf-8", jsonBody, true);
                                        string objVendorReturn = _ITRansaction.SyncDataUpdate("vdr", supplier.ExternalVendorID, true, false, "success", "",ID);
                                        _log.Info("Info" + " - " + "Vendor POST : " + aPIResult);
                                        dynamic data = JObject.Parse(aPIResult);

                                    }

                                    catch (WebException wEx)
                                    {
                                        if (wEx.Message.Contains("401"))
                                        {
                                            mVEWebClient = new MVEWebClient();
                                            mVEWebClient.GetToken();
                                            _log.Error("ERROR" + " - " + " Vendor POST : " + wEx.Message);
                                            // return wEx.Message;
                                            ////return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
                                        }
                                        else
                                        {

                                            using (var stream = wEx.Response.GetResponseStream())
                                            {
                                                using (var reader = new StreamReader(stream))
                                                {
                                                    string strdeserializeObject = Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd()));
                                                    string objPay = _ITRansaction.SyncDataUpdate("vdr", supplier.ExternalVendorID, true, false, strdeserializeObject, "",ID);
                                                    _log.Error("ERROR" + " - " + "Vendor POST Exception else: " + strdeserializeObject);
                                                    // return Request.CreateErrorResponse(HttpStatusCode.BadRequest, strdeserializeObject);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _log.Error("ERROR" + " - " + " Vendor POST : " + ex.Message);
                                        //strResult.AppendLine("ERROR POST supplier: " + ex.Message);
                                        // return ex.Message;
                                    }
                                }

                            }
                            //else
                            {
                                foreach (SupplierMaster supplier in result)
                                {
                                    try
                                    {
                                        putUrl = string.Format("{0}/{1}{2}", "v1", "Vendors", string.Format("/{0}", supplier.ExternalVendorID));
                                        mVEWebClient = new MVEWebClient(putUrl);
                                        jsonBody = JsonConvert.SerializeObject(supplier);
                                        _log.Info("Info" + " - " + " Vendor PUT jsonBody : " + jsonBody);
                                        aPIResult = mVEWebClient.ExecuteWebClient("PUT", "application/json; charset=utf-8", jsonBody, true);
                                        string objVendorReturn = _ITRansaction.SyncDataUpdate("vdr", supplier.ExternalVendorID, true, false, "success", "",ID);
                                        _log.Info("Info" + " - " + " Vendor PUT : " + aPIResult);
                                        dynamic data = JObject.Parse(aPIResult);
                                        return data.message;
                                    }
                                    catch (WebException wEx)
                                    {
                                        if (wEx.Message.Contains("401"))
                                        {
                                            mVEWebClient = new MVEWebClient();
                                            mVEWebClient.GetToken();
                                            _log.Error("ERROR" + " - " + " Vendor PUT : " + wEx.Message);
                                            ////return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
                                            return wEx.Message;
                                        }
                                        else
                                        {
                                            using (var stream = wEx.Response.GetResponseStream())
                                            {
                                                using (var reader = new StreamReader(stream))
                                                {
                                                    string strdeserializeObject = Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd()));
                                                    string objPay = _ITRansaction.SyncDataUpdate("vdr", supplier.ExternalVendorID, true, false, strdeserializeObject, "",ID);
                                                    _log.Error("ERROR" + " - " + "Vendor PUT Exception else: " + strdeserializeObject);
                                                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, strdeserializeObject);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _log.Error("ERROR" + " - " + " Vendor PUT: " + ex.Message);
                                        return ex.Message;
                                    }
                                }
                            }
                        }
                        //return Request.CreateResponse(HttpStatusCode.OK, aPIResult); //_IErrorResponse.CreateExceptionResponse(ex.Message);
                    }
                    catch (WebException wEx)
                    {
                        if (wEx.Message.Contains("401"))
                        {
                            mVEWebClient = new MVEWebClient();
                            mVEWebClient.GetToken();
                            _log.Error("ERROR" + " - " + " Vendor: " + wEx.Message);
                            //strResult.AppendLine("Error: _IVendor " + wEx.Message);
                            //return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
                            return wEx.Message;
                        }
                        else
                        {
                            using (var stream = wEx.Response.GetResponseStream())
                            {
                                using (var reader = new StreamReader(stream))
                                {
                                    _log.Error("ERROR" + " - " + " Vendor: " + Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
                                    //strResult.AppendLine("Error : " + Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
                                    //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
                                    return Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd()));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error("ERROR" + " - " + " Vendor: " + ex.Message);
                        //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        //return new HttpResponseMessage(HttpStatusCode.InternalServerError);  //_IErrorResponse.CreateExceptionResponse(ex.Message);
                        return ex.Message;
                    }
                    break;
                default:
                    break;
            }
            return string.Empty;
        }


    }
}
