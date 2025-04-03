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

namespace Unicomer.Cosacs.WebApi.Areas.ParentSKU
{
    [RoutePrefix("api/Inventory")]
    public class InventoryController : ApiController
    {
        private readonly ITransaction _ITRansaction;
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IInventory _Inventory;
        private readonly IErrorResponse _IErrorResponse;
        public InventoryController()
        {
            _Inventory = new InventoryBusiness();
            _IErrorResponse = new JResponseError();
            _ITRansaction = new TransactionBusiness();
        }

        //[AcceptVerbs("Put")]
        //[Route("PriceUpdate")]
        //public dynamic PriceUpdate([FromBody]PriceUpdate objPriceUpdate)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            return _Inventory.PriceUpdate(objPriceUpdate);
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

        /// <summary>
        /// Create Stock Transfer Information
        /// </summary>
        /// <param name="objStockTransfer"></param>
        /// <returns>StockTransferId</returns>
        //[AcceptVerbs("POST")]
        //[Route("StockTransfer1")]
        //public dynamic CreateStockTransfer([FromBody]StockTransferModel objStockTransfer)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            return _Inventory.CreateStockTransfer(objStockTransfer);
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

        /// <summary>
        /// Get StockTransfer Details 
        /// </summary>
        /// <param name="StkTrfNo"></param>
        /// <returns></returns>
        /// 

        [AcceptVerbs("GET")]
        [Route("StockTransfer")]
        public dynamic StockTransfer([FromUri]string StkTrfNo,string ID)
        {
            MVEWebClient mVEWebClient = null;
            try
            {
                StockTransferModel result = _Inventory.StockTransfer(StkTrfNo);
                string aPIResult = string.Empty;
                if (result != null)
                {
                    string postUrl = string.Format("{0}/{1}", "v1", "StockTransfer");
                    mVEWebClient = new MVEWebClient(postUrl);
                    string jsonBody = JsonConvert.SerializeObject(result);
                    _log.Info("Info : Send StockTransfer Json to MVE : " + jsonBody);
                    aPIResult = mVEWebClient.ExecuteWebClient("POST", "application/json; charset=utf-8", jsonBody, true);
                    _log.Info("Info : Send StockTransfer Json to MVE Result : " + aPIResult);
                    string objStock = _ITRansaction.SyncDataUpdate("StkTrf", StkTrfNo, true, false, "success", "", ID);
                }
                else
                {
                    string objStock = _ITRansaction.SyncDataUpdate("StkTrf", StkTrfNo, true, false, result.ToString(), "",ID);
                    _log.Error("ERROR" + " - " + "StockTransfer Message: " + result);
                    return Request.CreateResponse(HttpStatusCode.NotFound, result);
                }
                return Request.CreateResponse(HttpStatusCode.OK, aPIResult);
                //return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (WebException wEx)
            {
                if (wEx.Message.Contains("401"))
                {
                    mVEWebClient = new MVEWebClient();
                    mVEWebClient.GetToken();
                    _log.Error("ERROR" + " - " + "StockTransfer Web Exception: " + wEx.Message);
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
                }
                else
                {
                    using (var stream = wEx.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            string strdeserializeObject = Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd()));
                            string objStock = _ITRansaction.SyncDataUpdate("StkTrf", StkTrfNo, true, false, strdeserializeObject, "",ID);
                            _log.Error("ERROR" + " - " + "StockTransfer Web Exception else: " + strdeserializeObject);
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, strdeserializeObject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                _log.Error("ERROR" + " - " + "StockTransfer Exception : " + objHttpResponseMessage);
                return objHttpResponseMessage;

            }
        }
    }
}