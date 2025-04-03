using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Unicomer.Cosacs.Business;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.WebApi.Controllers
{
    [RoutePrefix("api/EOD")]
    public class EODSchedularController : ApiController
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IVendor _IVendor;
        private readonly IParentSKU _IParentSKU;
        private readonly ITransaction _ITRansaction;
        public EODSchedularController()
        {
            _ITRansaction = new TransactionBusiness();
            _IParentSKU = new ParentSKUBusiness();
            _IVendor = new VendorBusiness();
            //log4net.GlobalContext.Properties["LogFileName"] = @"C://MVE_Unicomer//Log//VE_COSACS_"; //log file path
            log4net.GlobalContext.Properties["LogFileName"] = ConfigurationManager.AppSettings["LogFolderPath"] != null ? ConfigurationManager.AppSettings["LogFolderPath"] : @"C://MVE_Unicomer//Log//VE_COSACS_";
            log4net.Config.XmlConfigurator.Configure();
        }
        [AcceptVerbs("Get")]
        [Route("ExecuteEOD")]
        public dynamic Get(int spanInMinutes,string ProductID,string ID)
        {
            //string fileName = Path.GetFileName("");
            MVEWebClient mVEWebClient = null;
            try
            {
                string aPIResult = string.Empty;
                string jsonBody = string.Empty;
                // JResponse resultPOST = _IParentSKU.GetParentSKUMaster();

                #region temp close


                //if (resultPOST != null && resultPOST.StatusCode.Equals(200) && !string.IsNullOrWhiteSpace(resultPOST.Result))
                //{
                //    try
                //    {
                //        string postUrl = string.Format("{0}/{1}{2}", "v1", "ParentSkus", string.Empty);
                //        mVEWebClient = new MVEWebClient(postUrl);
                //        _log.Info("Info URL " + " - " + postUrl);
                //        aPIResult = mVEWebClient.ExecuteWebClient("POST", "application/json; charset=utf-8", resultPOST.Result, true);
                //        _log.Info("Info Result" + " - " + aPIResult);
                //    }
                //    catch (WebException wEx)
                //    {
                //        if (wEx.Message.Contains("401"))
                //        {
                //            mVEWebClient = new MVEWebClient();
                //            mVEWebClient.GetToken();
                //            _log.Error("ERROR" + " - " + " ParentSkus POST ----: " + wEx.Message);
                //            ////return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
                //        }
                //        else
                //        {
                //            using (var stream = wEx.Response.GetResponseStream())
                //            {
                //                using (var reader = new StreamReader(stream))
                //                {
                //                    _log.Error("ERROR" + " - " + " Parent POST------ : " + Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
                //                    //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
                //                }
                //            }
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        _log.Error("ERROR: " + ex.Message);
                //    }
                //}
                #endregion

                List<Unicomer.Cosacs.Model.ParentSKU> result = _IParentSKU.getParentSKUEOD(spanInMinutes, ProductID);
                if (result != null && result.Count > 0)
                {
                    string putUrl = string.Empty;
                    foreach (Unicomer.Cosacs.Model.ParentSKU parentSku in result)
                    {
                        try
                        {
                            putUrl = string.Format("{0}/{1}{2}", "v1", "ParentSkus", string.Format(""));
                            mVEWebClient = new MVEWebClient(putUrl);
                            List<Unicomer.Cosacs.Model.ParentSKU> listObj = new List<Model.ParentSKU>();
                            listObj.Add(parentSku);
                            jsonBody = JsonConvert.SerializeObject(listObj);

                            _log.Info("Info" + " - " + putUrl);
                            _log.Info("Info" + " - " + jsonBody);

                            aPIResult = mVEWebClient.ExecuteWebClient("POST", "application/json; charset=utf-8", jsonBody, true);
                            string objPostSku = _ITRansaction.SyncDataUpdate("sku", parentSku.ExternalProductID, true, false, "success", "",ID);
                            _log.Info("Info---" + " - " + "Parentskus POST ExternalProductID------" + " - " + parentSku.ExternalProductID + " --" + aPIResult);
                        }
                        catch (WebException wEx)
                        {
                            if (wEx.Message.Contains("401"))
                            {
                                mVEWebClient = new MVEWebClient();
                                mVEWebClient.GetToken();
                                _log.Error("ERROR------" + " - " + " ParentSkus POST -----: " + wEx.Message);
                                ////return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
                            }
                            else
                            {
                                using (var stream = wEx.Response.GetResponseStream())
                                {
                                    using (var reader = new StreamReader(stream))
                                    {
                                        _log.Error("ERROR------" + " - " + " Parent POST ------: " + Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
                                        //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error("ERROR----: " + ex.Message);
                        }
                    }
                    foreach (Unicomer.Cosacs.Model.ParentSKU parentSku in result)
                    {
                        try
                        {
                            putUrl = string.Format("{0}/{1}{2}", "v1", "ParentSkus", string.Format("/{0}", parentSku.ExternalProductID));
                            _log.Info("Info" + " - " + putUrl);
                            mVEWebClient = new MVEWebClient(putUrl);
                            jsonBody = JsonConvert.SerializeObject(parentSku);
                            _log.Info("Info" + " - " + jsonBody);
                            aPIResult = mVEWebClient.ExecuteWebClient("PUT", "application/json; charset=utf-8", jsonBody, true);
                            string objPutSku = _ITRansaction.SyncDataUpdate("sku", parentSku.ExternalProductID, true, false, "success", "",ID);
                            _log.Info("Info---" + " - " + "Parentskus PUT ExternalProductID------" + " - " + parentSku.ExternalProductID + " --" + aPIResult);
                        }
                        catch (Exception ex)
                        {
                            _log.Error("ERROR" + " - " + " ParentSkus: PUT " + ex.Message);
                        }
                    }
                }
                //return Request.CreateResponse(HttpStatusCode.Created, aPIResult);
                //return Request.CreateResponse(HttpStatusCode.OK, aPIResult);  //_IErrorResponse.CreateExceptionResponse(ex.Message);
            }
            catch (WebException wEx)
            {
                if (wEx.Message.Contains("401"))
                {
                    mVEWebClient = new MVEWebClient();
                    mVEWebClient.GetToken();
                    _log.Error("ERROR----" + " - " + " ParentSkus---: : " + wEx.Message);
                    //strResult.AppendLine("Error 11s: " + wEx.Message);
                    //return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
                }
                else
                {
                    using (var stream = wEx.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            _log.Error("ERROR---" + " - " + " ParentSkus: -- " + Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
                            //strResult.AppendLine("Error 2: " + Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
                            //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("ERROR----" + " - " + " ParentSkus: :::: " + ex.Message);
                //strResult.AppendLine("Error: " + ex.Message);
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                //return new HttpResponseMessage(HttpStatusCode.InternalServerError);  //_IErrorResponse.CreateExceptionResponse(ex.Message);
            }
            #region Vendor Comment

            //mVEWebClient = null;
            //try
            //{
            //    List<SupplierMaster> result = _IVendor.GetSupplierEOD(spanInMinutes);
            //    string aPIResult = string.Empty;
            //    string jsonBody = string.Empty;
            //    if (result != null && result.Count > 0)
            //    {
            //        string putUrl = string.Empty;
            //        foreach (SupplierMaster supplier in result)
            //        {
            //            try
            //            {
            //                putUrl = string.Format("{0}/{1}{2}", "v1", "Vendors", string.Format(""));
            //                mVEWebClient = new MVEWebClient(putUrl);
            //                List<SupplierMaster> listObj = new List<SupplierMaster>();
            //                listObj.Add(supplier);
            //                jsonBody = JsonConvert.SerializeObject(listObj);
            //                aPIResult = mVEWebClient.ExecuteWebClient("POST", "application/json; charset=utf-8", jsonBody, true);
            //                _log.Info("Info----" + " - " + "Vendor POST--- : " + aPIResult);
            //            }

            //            catch (WebException wEx)
            //            {
            //                if (wEx.Message.Contains("401"))
            //                {
            //                    mVEWebClient = new MVEWebClient();
            //                    mVEWebClient.GetToken();
            //                    _log.Error("ERROR---" + " - " + " Vendor POST----------- : " + wEx.Message);
            //                    ////return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
            //                }
            //                else
            //                {
            //                    using (var stream = wEx.Response.GetResponseStream())
            //                    {
            //                        using (var reader = new StreamReader(stream))
            //                        {
            //                            _log.Error("ERROR---------" + " - " + " Vendor POST -------------: " + Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
            //                            //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
            //                        }
            //                    }
            //                }
            //            }

            //            catch (Exception ex)
            //            {
            //                _log.Error("ERROR--------" + " - " + " Vendor POST : -------------" + ex.Message);
            //                //strResult.AppendLine("ERROR POST supplier: " + ex.Message);
            //            }
            //        }
            //        foreach (SupplierMaster supplier in result)
            //        {
            //            try
            //            {
            //                putUrl = string.Format("{0}/{1}{2}", "v1", "Vendors", string.Format("/{0}", supplier.ExternalVendorID));
            //                mVEWebClient = new MVEWebClient(putUrl);
            //                jsonBody = JsonConvert.SerializeObject(supplier);
            //                aPIResult = mVEWebClient.ExecuteWebClient("PUT", "application/json; charset=utf-8", jsonBody, true);
            //                _log.Info("Info" + " - " + " Vendor PUT : " + aPIResult);
            //            }
            //            catch (WebException wEx)
            //            {
            //                if (wEx.Message.Contains("401"))
            //                {
            //                    mVEWebClient = new MVEWebClient();
            //                    mVEWebClient.GetToken();
            //                    _log.Error("ERROR--------" + " - " + " Vendor PUT------- : " + wEx.Message);
            //                    ////return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
            //                }
            //                else
            //                {
            //                    using (var stream = wEx.Response.GetResponseStream())
            //                    {
            //                        using (var reader = new StreamReader(stream))
            //                        {
            //                            _log.Error("ERROR--------" + " - " + " Vendor PUT------------- : " + Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
            //                            //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
            //                        }
            //                    }
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                _log.Error("ERROR--=====" + " - " + " Vendor PUT-======: " + ex.Message);

            //            }
            //        }
            //    }
            //    //return Request.CreateResponse(HttpStatusCode.OK, aPIResult); //_IErrorResponse.CreateExceptionResponse(ex.Message);
            //}
            //catch (WebException wEx)
            //{
            //    if (wEx.Message.Contains("401"))
            //    {
            //        mVEWebClient = new MVEWebClient();
            //        mVEWebClient.GetToken();
            //        _log.Error("ERROR------===" + " - " + " Vendor:--- " + wEx.Message);
            //        //strResult.AppendLine("Error: _IVendor " + wEx.Message);
            //        //return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
            //    }
            //    else
            //    {
            //        using (var stream = wEx.Response.GetResponseStream())
            //        {
            //            using (var reader = new StreamReader(stream))
            //            {
            //                _log.Error("ERROR=====" + " - " + " Vendor=====: " + Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
            //                //strResult.AppendLine("Error : " + Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
            //                //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _log.Error("ERROR=====-----" + " - " + " Vendor:====== " + ex.Message);
            //    //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            //    //return new HttpResponseMessage(HttpStatusCode.InternalServerError);  //_IErrorResponse.CreateExceptionResponse(ex.Message);
            //}


            #endregion Vendor Comment
            return string.Empty;
        }
    }
}