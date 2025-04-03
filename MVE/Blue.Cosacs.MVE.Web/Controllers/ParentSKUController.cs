using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Unicomer.Cosacs.Business;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.WebApi.Areas.ParentSKU
{
    [RoutePrefix("api/ParentSKU")]
    public class ParentSKUController : ApiController
    {
        private readonly IParentSKU _IParentSKU;
        private readonly IErrorResponse _IErrorResponse;
        private readonly ITransaction _ITRansaction;
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ParentSKUController()
        {
            _IParentSKU = new ParentSKUBusiness();
            _IErrorResponse = new JResponseError();
            _ITRansaction = new TransactionBusiness();
        }
        //[AcceptVerbs("Get")]
        //[Route("GetParentSKU")]
        //public dynamic GetParentSKUMaster()
        //{
        //    MVEWebClient mVEWebClient = null;
        //    try
        //    {
        //        JResponse result = _IParentSKU.GetParentSKUMaster();
        //        string aPIResult = string.Empty;
        //        if (result != null && result.StatusCode.Equals(200) && !string.IsNullOrWhiteSpace(result.Result))
        //        {
        //            string postUrl = string.Format("{0}/{1}{2}", "v1", "ParentSkus", string.Empty);
        //            mVEWebClient = new MVEWebClient(postUrl);
        //            _log.Info("Info : Send Parent SKU Json to MVE : " + postUrl);
        //            aPIResult = mVEWebClient.ExecuteWebClient("POST", "application/json; charset=utf-8", result.Result, true);
        //            _log.Info("Info : Send Parent SKU Json to MVE Result : " + aPIResult);

        //        }
        //        return Request.CreateResponse(HttpStatusCode.Created, aPIResult);  //_IErrorResponse.CreateExceptionResponse(ex.Message);
        //    }
        //    catch (WebException wEx)
        //    {
        //        if (wEx.Message.Contains("401"))
        //        {
        //            mVEWebClient = new MVEWebClient();
        //            mVEWebClient.GetToken();
        //            _log.Error("ERROR" + " - " + "Parent SKU Web Exception: " + wEx.Message);
        //            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
        //        }
        //        else
        //        {
        //            using (var stream = wEx.Response.GetResponseStream())
        //            {
        //                using (var reader = new StreamReader(stream))
        //                {
        //                    string strdeserializeObject = Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd()));
        //                    _log.Error("ERROR" + " - " + "Parent SKU Web Exception else: " + strdeserializeObject);
        //                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, strdeserializeObject);

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        //        // Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //        _log.Error("ERROR" + " - " + "Parent SKU Exception : " + objHttpResponseMessage);
        //        return objHttpResponseMessage;  //_IErrorResponse.CreateExceptionResponse(ex.Message);
        //    }
        //}
        //[AcceptVerbs("Put")]
        //[Route("UpdateParentSKU")]
        //public dynamic UpdateParentSKUMaster([FromBody]ParentSKUUpdate ParentSKUUpdate)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            return _IParentSKU.UpdateParentSKUMaster(ParentSKUUpdate);
        //        }
        //        else
        //        {
        //            return _IErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //        return _IErrorResponse.CreateExceptionResponse(ex.Message);
        //    }
        //}
        //[AcceptVerbs("Get")]
        //[Route("GetParentSKUEOD")]
        //public dynamic getParentSKUEOD([FromUri]int spanInMinutes)
        //{
        //    MVEWebClient mVEWebClient = null;
        //    try
        //    {
        //        List<Unicomer.Cosacs.Model.ParentSKU> result = _IParentSKU.getParentSKUEOD(spanInMinutes);
        //        string aPIResult = string.Empty;
        //        if (result != null && result.Count > 0)
        //        {
        //            foreach (Unicomer.Cosacs.Model.ParentSKU parentSku in result)
        //            {
        //                string putUrl = string.Format("{0}/{1}{2}", "v1", "ParentSkus", string.Format("/{0}", parentSku.ExternalProductID));
        //                mVEWebClient = new MVEWebClient(putUrl);
        //                string jsonBody = JsonConvert.SerializeObject(parentSku);
        //                aPIResult = mVEWebClient.ExecuteWebClient("PUT", "application/json; charset=utf-8", jsonBody, true);
        //            }
        //        }
        //        //return Request.CreateResponse(HttpStatusCode.Created, aPIResult);
        //        return Request.CreateResponse(HttpStatusCode.OK, aPIResult);  //_IErrorResponse.CreateExceptionResponse(ex.Message);
        //    }
        //    catch (WebException wEx)
        //    {
        //        if (wEx.Message.Contains("401"))
        //        {
        //            mVEWebClient = new MVEWebClient();
        //            mVEWebClient.GetToken();
        //            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
        //        }
        //        else
        //        {
        //            using (var stream = wEx.Response.GetResponseStream())
        //            {
        //                using (var reader = new StreamReader(stream))
        //                {
        //                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //        return new HttpResponseMessage(HttpStatusCode.InternalServerError);  //_IErrorResponse.CreateExceptionResponse(ex.Message);
        //    }
        //}

        [AcceptVerbs("Get")]
        [Route("GetParentSKU")]
        public dynamic GetParentSKUMaster()
        {
            MVEWebClient mVEWebClient = null;
            string aPIResult = string.Empty;

            try
            {
                JResponse result = _IParentSKU.GetParentSKUMaster();
                if (result != null && result.StatusCode.Equals(200) && !string.IsNullOrWhiteSpace(result.Result))
                {
                    List<Unicomer.Cosacs.Model.ParentSKU> parentSKUList = JsonConvert.DeserializeObject<List<Unicomer.Cosacs.Model.ParentSKU>>(result.Result);
                    string postUrl = string.Format("{0}/{1}{2}", "v1", "ParentSkus", string.Empty);
                    if (parentSKUList != null && parentSKUList.Count > 0)
                    {
                        List<Unicomer.Cosacs.Model.ParentSKU> innerParentSKUList = null;

                        foreach (Unicomer.Cosacs.Model.ParentSKU parentSKU in parentSKUList)
                        {
                            innerParentSKUList = new List<Model.ParentSKU>();
                            innerParentSKUList.Add(parentSKU);
                            string aPIResultAppend = JsonConvert.SerializeObject(innerParentSKUList);
                            _log.Info("Info" + " - " + "GetParentSKUMaster Result Append: " + aPIResultAppend);
                            try
                            {
                                mVEWebClient = new MVEWebClient(postUrl);
                                aPIResult = mVEWebClient.ExecuteWebClient("POST", "application/json; charset=utf-8", aPIResultAppend, true) + parentSKU.ExternalVendorID + ", ";
                                _log.Info("Info" + " - " + "GetParentSKUMaster Result: " + aPIResult);
                            }
                            catch (WebException wEx)
                            {
                                if (wEx.Message.Contains("401"))
                                {
                                    mVEWebClient = new MVEWebClient();
                                    mVEWebClient.GetToken();
                                    _log.Error("ERROR" + " - " + "Parent SKU Web Exception: " + wEx.Message);
                                }
                                else
                                {
                                    using (var stream = wEx.Response.GetResponseStream())
                                    {
                                        using (var reader = new StreamReader(stream))
                                        {
                                            string strdeserializeObject = Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd()));
                                            _log.Error("ERROR" + " - " + "Parent SKU Web Exception else: " + strdeserializeObject);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                                _log.Error("ERROR" + " - " + "Parent SKU Exception : " + objHttpResponseMessage);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                _log.Error("ERROR" + " - " + "Parent SKU Exception : " + objHttpResponseMessage);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError); //_IErrorResponse.CreateExceptionResponse(ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.Created, aPIResult); //_IErrorResponse.CreateExceptionResponse(ex.Message);
        }

        [AcceptVerbs("Get")]
        [Route("GetParentSKUUpdate")]
        public dynamic GetParentSKUUpdate(string ID)
        {
            MVEWebClient mVEWebClient = null;
            string aPIResult = string.Empty;
            string MaintainTime = string.Empty;
            try
            {
                #region Delete Log File
                try
                {
                    var rootAppender = ((Hierarchy)LogManager.GetRepository())
                     .Root.Appenders.OfType<FileAppender>()
                     .FirstOrDefault();
                    string filename = rootAppender != null ? rootAppender.File : string.Empty;

                    int startIndex = filename.IndexOf("\\VE_COSACS");
                    filename = filename.Remove(startIndex);
                    _log.Info("Info - File delting from:" + filename);
                    MaintainTime = ConfigurationManager.AppSettings["ErrorLogMaintainDays"];
                    string[] files = Directory.GetFiles(filename.Trim());

                    foreach (string file in files)
                    {
                        FileInfo fi = new FileInfo(file);

                        if (fi.LastAccessTime < DateTime.Now.AddDays(-(Convert.ToInt32(MaintainTime))))
                        {
                            fi.Delete();
                            _log.Info("File deleted:" + file);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Info("Info - Exception while deleting the logFile before last-" + MaintainTime.Trim() + " Days. Exception-  " + ex.Message);
                }
                finally
                {

                }
                #endregion

                try
                {
                    JResponse result = _IParentSKU.GetParentSKUMasterEOD();
                    if (result != null && result.StatusCode.Equals(200) && !string.IsNullOrWhiteSpace(result.Result))
                    {
                        List<Unicomer.Cosacs.Model.ParentSKU> parentSKUList = JsonConvert.DeserializeObject<List<Unicomer.Cosacs.Model.ParentSKU>>(result.Result);
                        string postUrl = string.Format("{0}/{1}{2}", "v1", "ParentSkus", string.Empty);
                        if (parentSKUList != null && parentSKUList.Count > 0)
                        {
                            List<Unicomer.Cosacs.Model.ParentSKU> innerParentSKUList = null;

                            foreach (Unicomer.Cosacs.Model.ParentSKU parentSKU in parentSKUList)
                            {
                                _log.Info("Info" + " - " + "POST-GetParentSKUMaster Result Append: ");
                                innerParentSKUList = new List<Model.ParentSKU>();
                                innerParentSKUList.Add(parentSKU);
                                string aPIResultAppend = JsonConvert.SerializeObject(innerParentSKUList);
                                _log.Info("Info" + " - " + "POST-GetParentSKUMaster Result Append: " + aPIResultAppend);
                                try
                                {
                                    mVEWebClient = new MVEWebClient(postUrl);
                                    aPIResult = mVEWebClient.ExecuteWebClient("POST", "application/json; charset=utf-8", aPIResultAppend, true) + parentSKU.ExternalVendorID + ", ";
                                    _log.Info("Info" + " - " + "POST-GetParentSKUMaster Result: " + aPIResult);
                                }
                                catch (WebException wEx)
                                {
                                    if (wEx.Message.Contains("401"))
                                    {
                                        mVEWebClient = new MVEWebClient();
                                        mVEWebClient.GetToken();
                                        _log.Error("ERROR" + " - " + "POST-Parent SKU Web Exception: " + wEx.Message);
                                    }
                                    else
                                    {
                                        using (var stream = wEx.Response.GetResponseStream())
                                        {
                                            using (var reader = new StreamReader(stream))
                                            {
                                                string strdeserializeObject = Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd()));
                                                _log.Error("ERROR" + " - " + "POST-Parent SKU Web Exception else: " + strdeserializeObject);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                                    _log.Error("ERROR" + " - " + "POST-Parent SKU Exception : " + objHttpResponseMessage);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                try
                {

                    JResponse result = _IParentSKU.GetParentSKUUpdate();
                    if (result != null && result.StatusCode.Equals(200) && !string.IsNullOrWhiteSpace(result.Result))
                    {
                        List<Unicomer.Cosacs.Model.ParentSKUUpdateWithoutQuantity> parentSKUList = JsonConvert.DeserializeObject<List<Unicomer.Cosacs.Model.ParentSKUUpdateWithoutQuantity>>(result.Result);
                        if (parentSKUList != null && parentSKUList.Count > 0)
                        {
                            List<Unicomer.Cosacs.Model.ParentSKUUpdateWithoutQuantity> innerParentSKUList = null;
                            foreach (Unicomer.Cosacs.Model.ParentSKUUpdateWithoutQuantity parentSKU in parentSKUList)
                            {
                                string putUrl = string.Format("{0}/{1}{2}", "v1", "ParentSkus", string.Format("/{0}", parentSKU.ExternalProductID));
                                innerParentSKUList = new List<Model.ParentSKUUpdateWithoutQuantity>();
                                innerParentSKUList.Add(parentSKU);
                                string aPIResultAppend = JsonConvert.SerializeObject(innerParentSKUList);

                                try
                                {
                                    mVEWebClient = new MVEWebClient(putUrl);
                                    aPIResultAppend = (aPIResultAppend.Substring(1, aPIResultAppend.Length - 1));
                                    _log.Info("Info" + " - " + "PUT-Update ParentSKUMaster Result Append: " + aPIResultAppend);
                                    aPIResult = mVEWebClient.ExecuteWebClient("PUT", "application/json; charset=utf-8", aPIResultAppend, true);
                                    _log.Info("Info" + " - " + "PUT-Update ParentSKUMaster Result: " + aPIResult);
                                }
                                catch (WebException wEx)
                                {
                                    if (wEx.Message.Contains("401"))
                                    {
                                        mVEWebClient = new MVEWebClient();
                                        mVEWebClient.GetToken();
                                        _log.Error("ERROR" + " - " + "PUT-Update Parent SKU Web Exception: " + wEx.Message);
                                    }
                                    else
                                    {
                                        using (var stream = wEx.Response.GetResponseStream())
                                        {
                                            using (var reader = new StreamReader(stream))
                                            {
                                                string strdeserializeObject = Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd()));
                                                _log.Error("ERROR" + " - " + "PUT-Update Parent SKU Web Exception else: " + strdeserializeObject);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                                    _log.Error("ERROR" + " - " + "PUT-Update Parent SKU Exception : " + objHttpResponseMessage);
                                    _ITRansaction.SyncDataUpdate("EOD", "", true, false, objHttpResponseMessage.RequestMessage.ToString(), "", ID);
                                }
                            }
                            string objDel = _ITRansaction.SyncDataUpdate("EOD", "", true, false, "success", "", ID);
                        }
                    }
                    else
                    {
                        string objDel = _ITRansaction.SyncDataUpdate("EOD", "", true, false, "success", "0000", ID);
                    }
                }
                catch (Exception ex)
                { }
            }
            catch (Exception ex)
            {
                HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                _log.Error("ERROR" + " - " + "Main Block-Parent SKU Exception : " + objHttpResponseMessage);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError); //_IErrorResponse.CreateExceptionResponse(ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.Created, aPIResult);
        }
    }
}