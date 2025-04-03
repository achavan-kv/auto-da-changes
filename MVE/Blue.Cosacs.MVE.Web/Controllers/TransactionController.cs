using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web.Http;
using Unicomer.Cosacs.Business;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.WebApi.Areas.Transaction.Controllers
{
    [RoutePrefix("api/Transaction")]
    public class TransactionController : ApiController
    {
        private readonly ITransaction _ITRansaction;
        private readonly IErrorResponse _IErrorResponse;
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TransactionController()
        {
            log4net.GlobalContext.Properties["LogFileName"] = ConfigurationManager.AppSettings["LogFolderPath"] != null ? ConfigurationManager.AppSettings["LogFolderPath"] : @"C://MVE_Unicomer//Log//VE_COSACS_";
            log4net.Config.XmlConfigurator.Configure();
            _ITRansaction = new TransactionBusiness();
            _IErrorResponse = new JResponseError();// = new TransactionBusiness();
        }

        /// <summary>
        /// Bill Generation Information for Insert...
        /// </summary>
        /// <param name="objBillGenHeader"></param>
        /// <returns>Account Id</returns>
        /// 
        [AcceptVerbs("POST")]
        [Route("BillGeneration")]
        public dynamic BillGeneration([FromBody]BillGenerationHeader objBillGenHeader)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return _ITRansaction.BillGeneration(objBillGenHeader, false);
                }
                else
                {
                    //   return _IErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
                return null;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }

        /// <summary>
        /// Bill Generation Information for Insert
        /// </summary>
        /// <param name="objBillGenHeader"></param>
        /// <returns>Account Id</returns>
        /// 
        [AcceptVerbs("PUT")]
        [Route("BillUpdation")]
        public dynamic BillUpdation([FromBody]BillGenerationHeader objBillGenHeader)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    return _ITRansaction.BillGeneration(objBillGenHeader, true);

                    //  return _ICustomer.CreateUsers(objJSON);
                }
                else
                {
                    //   return _IErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
                return null;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }

        /// <summary>
        /// Create Purchase Order Information
        /// </summary>
        /// <param name="PO"></param>
        /// <returns>Account Id</returns>
        [AcceptVerbs("POST")]
        [Route("CreatePO")]
        public dynamic CreatePurchaseOrder([FromBody]PurchaseOrderModel objPurchaseOrder)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return _ITRansaction.CreatePO(objPurchaseOrder, false);
                    //  return _ICustomer.CreateUsers(objJSON);
                }
                else
                {
                    //   return _IErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
                return null;
            }
            catch (Exception ex)
            {
                return _IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }

        /// <summary>
        /// Create Purchase Order Information
        /// </summary>
        /// <param name="PO"></param>
        /// <returns>Account Id</returns>
        [AcceptVerbs("PUT")]
        [Route("UpdatePO")]
        public dynamic POUpdation([FromBody]PurchaseOrderModel objUpdatePO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return _ITRansaction.CreatePO(objUpdatePO, true);
                    //  return _ICustomer.CreateUsers(objJSON);
                }
                else
                {
                    //   return _IErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
                return null;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }

        /// <summary>
        /// Create Account
        /// </summary>
        /// <param name="objJSON"></param>
        /// <returns>Account Id</returns>
        [AcceptVerbs("POST")]
        [Route("CreateAccount")]
        public dynamic CreateAccount([FromBody]CreateAccount objCreateAccount)
        {
            try
            {
                if (ModelState.IsValid)
                {
                   
                    return _ITRansaction.CreateAccount(objCreateAccount);
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
        /// <summary>
        /// Get Credit Availability Information
        /// </summary>
        /// <param name="CustId"></param>
        /// <returns>Account Id</returns>
        [AcceptVerbs("GET")]
        [Route("GetCreditAvailability")]
        public dynamic GetCreditAvailability([FromUri]string CustId)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    return _ITRansaction.GetCreditAvailability(CustId);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="GRNNo"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [AcceptVerbs("GET")]
        [Route("GetGRN")]
        public dynamic GetGRN([FromUri]string GRNNo, string ID)
        {
            MVEWebClient mVEWebClient = null;
            try
            {
                //List<Unicomer.Cosacs.Model.GRN> result = _ITRansaction.GetGRN(GRNNo);
                GRN result = _ITRansaction.GetGRN(GRNNo);
                string aPIResult = string.Empty;
                if (result != null)
                {
                    string postUrl = string.Format("{0}/{1}", "v1", "GRN");
                    mVEWebClient = new MVEWebClient(postUrl);
                    string jsonBody = JsonConvert.SerializeObject(result);
                    _log.Info("Info : Send GRN Json to MVE : " + jsonBody);
                    aPIResult = mVEWebClient.ExecuteWebClient("POST", "application/json; charset=utf-8", jsonBody, true);
                    _log.Info("Info : Send GRN Json to MVE Result : " + aPIResult);
                    string objGRN = _ITRansaction.SyncDataUpdate("grn", GRNNo, true, false, "success", "", ID);
                }
                else
                {
                    string objGRN = _ITRansaction.SyncDataUpdate("grn", GRNNo, true, false, result.ToString(), "", ID);
                    _log.Error("ERROR" + " - " + "GRN Message: " + result);
                    return Request.CreateResponse(HttpStatusCode.NotFound, result);
                }
                return Request.CreateResponse(HttpStatusCode.OK, aPIResult);

            }
            catch (WebException wEx)
            {
                if (wEx.Message.Contains("401"))
                {
                    mVEWebClient = new MVEWebClient();
                    mVEWebClient.GetToken();
                    _log.Error("ERROR" + " - " + "GRN Web Exception: " + wEx.Message);
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
                }
                else
                {
                    using (var stream = wEx.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            string strdeserializeObject = Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd()));
                            string objPay = _ITRansaction.SyncDataUpdate("grn", GRNNo, true, false, strdeserializeObject, "", ID);
                            _log.Error("ERROR" + " - " + "GRN Web Exception else: " + strdeserializeObject);
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, strdeserializeObject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                _log.Error("ERROR" + " - " + "GRN Exception : " + objHttpResponseMessage);
                return objHttpResponseMessage;

            }
        }

        /// <summary>
        /// Get DeliveryAuthorization
        /// </summary>
        /// <param name="DeliveryAuthorization"></param>
        /// <returns></returns>
        [AcceptVerbs("GET")]
        [Route("DeliveryAuthorization")]
        public dynamic DeliveryAuthorization([FromUri]string AcctNo, string DocType, string ID)
        {
            MVEWebClient mVEWebClient = null;
            try
            {
                DeliveryAuth result = new DeliveryAuth();
                result = _ITRansaction.DeliveryAuthorization(AcctNo, DocType);

                string aPIResult = string.Empty;
                if (result != null && result.authorization == true)
                {
                    string putUrl = string.Format("{0}/{1}{2}", "v1", "DeliveryAuthorizations", string.Format("/{0}", result.checkoutID));
                    mVEWebClient = new MVEWebClient(putUrl);
                    string jsonBody = JsonConvert.SerializeObject(result);
                    _log.Info("Info : Send Get Delivery Authorization OrderList Json to MVE : " + DocType + " JSON" + jsonBody);
                    aPIResult = mVEWebClient.ExecuteWebClient("PUT", "application/json; charset=utf-8", jsonBody, true);
                    _log.Info("Info : Get Delivery Authorization OrderList Json to MVE Result : " + aPIResult);
                    string objAuth = _ITRansaction.SyncDataUpdate(DocType, AcctNo, true, false, "success", "", ID);
                }
                else
                {
                    string objAuth = _ITRansaction.SyncDataUpdate(DocType, AcctNo, true, false, result.ToString(), "", ID);
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No data found");
                }
                return Request.CreateResponse(HttpStatusCode.OK, aPIResult);
                // return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (WebException wEx)
            {
                if (wEx.Message.Contains("401"))
                {
                    mVEWebClient = new MVEWebClient();
                    mVEWebClient.GetToken();
                    _log.Error("ERROR" + " - " + "Delivery Authorization Web Exception: " + wEx.Message);
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
                }
                else
                {
                    using (var stream = wEx.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            string strdeserializeObject = Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd()));
                            //	string objPay = _ITRansaction.SyncDataUpdate("pyt", AcctNo, true, false, strdeserializeObject, "");
                            _log.Error("ERROR" + " - " + "Delivery Authorization Web Exception: " + strdeserializeObject);
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, strdeserializeObject);
                            //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd())));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                _log.Error("ERROR" + " - " + "Delivery Authorization Exception : " + objHttpResponseMessage);
                return objHttpResponseMessage;  //_IErrorResponse.CreateExceptionResponse(ex.Message);

                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                //return new HttpResponseMessage(HttpStatusCode.InternalServerError);  //_IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }

        /// <summary>
        /// Create Vendor Return Information
        /// </summary>
        /// <param name="Vendor"></param>
        /// <returns>VendorReturn Id</returns>
        //[AcceptVerbs("POST")]
        //[Route("VendorReturnFromMVE")]
        //public dynamic CreateVendorReturn([FromBody]VendorReturnModel VR)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            return _ITRansaction.CreateVendorReturn(VR);
        //            //  return _ICustomer.CreateUsers(objJSON);
        //        }
        //        else
        //        {
        //            return _IErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //        return _IErrorResponse.CreateExceptionResponse(ex.Message);
        //    }
        //}

        /// <summary>
        /// Create Commissions
        /// </summary>
        /// <param name="objJSON"></param>
        /// <returns>Account Id</returns>
        //[AcceptVerbs("POST")]
        //[Route("CreateCommissions")]
        //public dynamic CreateCommissions([FromBody]Commissions objJSON)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            return _ITRansaction.CreateCommissions(objJSON, false);
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

        ///// <summary>
        ///// PUT Commissions
        ///// </summary>
        ///// <param name="objJSON"></param>
        ///// <returns>Account Id</returns>
        //[AcceptVerbs("PUT")]
        //[Route("UpdateCommissions")]
        //public dynamic PUTCommissions([FromBody]Commissions objJSON)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            return _ITRansaction.CreateCommissions(objJSON, true);
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
        /// Get Payments Order Wise
        /// </summary>
        /// <param name="AcctNo"></param>
        /// <returns>payments</returns>
        [AcceptVerbs("GET")]
        [Route("GetPayments")]
        public dynamic GetPaymentsOrderList([FromUri]string AcctNo, string ID, string CheckOut)
        {
            MVEWebClient mVEWebClient = null;
            string jsonBody = string.Empty;
            try
            {
                //JResponse result = _ITRansaction.GetPaymentsOrderList(AcctNo);
                Payments result = _ITRansaction.GetPaymentsOrderList(AcctNo, CheckOut);
                _log.Info("Info" + " - " + "Payments Order List Message: " + result.checkoutId);
                string aPIResult = string.Empty;
                if (result.checkoutId != 0)
                {
                    string postUrl = string.Format("{0}/{1}{2}", "v1", "Payments", string.Empty);
                    mVEWebClient = new MVEWebClient(postUrl);
                    jsonBody = JsonConvert.SerializeObject(result);
                    aPIResult = mVEWebClient.ExecuteWebClient("POST", "application/json; charset=utf-8", jsonBody, true);
                    _log.Info("Info : Send Get Payments OrderList Json to MVE : " + jsonBody);
                    _log.Info("Info : Send Get Payments OrderList Json to MVE Result : " + aPIResult);
                    //string objPay = _ITRansaction.SyncDataUpdate("pyt", AcctNo, true, false, "success", "");
                }
                else
                {
                    //	string objPay = _ITRansaction.SyncDataUpdate("pyt", AcctNo, true, false, result.Message, "");
                    _log.Error("ERROR" + " - " + "Payment Message checkoutId: " + result.checkoutId + "---" + jsonBody);
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No data found for Account code: " + AcctNo);
                }
                StringBuilder sb = new StringBuilder();
                foreach (Orders item in result.orderList)
                {
                    sb.Append(',');
                    sb.Append(item.orderId);
                }
                sb.Remove(0, 1);
                string orderid = sb.ToString();

                string objDel = _ITRansaction.SyncDataUpdate("pyt", AcctNo, true, false, "success", orderid, ID);

                return Request.CreateResponse(HttpStatusCode.Created, aPIResult);  //_IErrorResponse.CreateExceptionResponse(ex.Message);
            }
            catch (WebException wEx)
            {
                if (wEx.Message.Contains("401"))
                {
                    mVEWebClient = new MVEWebClient();
                    mVEWebClient.GetToken();
                    _log.Error("ERROR" + " - " + "Payment Web Exception: " + wEx.Message + "---" + jsonBody);
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
                }
                else
                {
                    using (var stream = wEx.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            string strdeserializeObject = Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd()));
                            //	string objPay = _ITRansaction.SyncDataUpdate("pyt", AcctNo, true, false, strdeserializeObject, "");
                            _log.Error("ERROR" + " - " + "Payment Web Exception: " + strdeserializeObject + "---" + jsonBody);
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, strdeserializeObject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                _log.Error("ERROR" + " - " + "Payment Exception : " + objHttpResponseMessage + "---" + jsonBody);
                return objHttpResponseMessage;  //_IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }

        /// <summary>
        /// Get Payments Order Wise
        /// </summary>
        /// <param name="AcctNo"></param>
        /// <returns>payments</returns>
        [AcceptVerbs("GET")]
        [Route("GetPaymentsPut")]
        public dynamic GetPaymentsOrderListPut([FromUri]string AcctNo, string ID, string CheckOut)
        {
            MVEWebClient mVEWebClient = null;
            string jsonBody = string.Empty;
            try
            {
                //JResponse result = _ITRansaction.GetPaymentsOrderList(AcctNo);
                Payments result = _ITRansaction.GetPaymentsOrderList(AcctNo, CheckOut);
                _log.Info("Info" + " - " + "Put Payments Order List Message: " + result.checkoutId);
                string aPIResult = string.Empty;
                if (result.checkoutId != null)
                {
                    //string postUrl = string.Format("{0}/{1}{2}", "v1", "Payments", string.Empty);
                    string putUrl = string.Format("{0}/{1}{2}", "v1", "Payments", string.Format("/{0}", result.externalPaymentID));
                    mVEWebClient = new MVEWebClient(putUrl);
                    jsonBody = JsonConvert.SerializeObject(result);
                    aPIResult = mVEWebClient.ExecuteWebClient("PUT", "application/json; charset=utf-8", jsonBody, true);
                    _log.Info("Info : Send Get Payments OrderList Put Json to MVE : " + jsonBody);
                    _log.Info("Info : Send Get Payments OrderList Put Json to MVE Result : " + aPIResult);
                    //string objPay = _ITRansaction.SyncDataUpdate("pyt", AcctNo, true, false, "success", "");
                }
                else
                {
                    //	string objPay = _ITRansaction.SyncDataUpdate("pyt", AcctNo, true, false, result.Message, "");
                    _log.Error("ERROR" + " - " + "Put Payment Message checkoutId: " + result.checkoutId + "---" + jsonBody);
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No data found for Account code: " + AcctNo);
                }
                StringBuilder sb = new StringBuilder();
                foreach (Orders item in result.orderList)
                {
                    sb.Append(',');
                    sb.Append(item.orderId);
                }
                sb.Remove(0, 1);
                string orderid = sb.ToString();

                string objDel = _ITRansaction.SyncDataUpdate("pyt", AcctNo, true, false, "success", orderid, ID);

                return Request.CreateResponse(HttpStatusCode.Created, aPIResult);  //_IErrorResponse.CreateExceptionResponse(ex.Message);
            }
            catch (WebException wEx)
            {
                if (wEx.Message.Contains("401"))
                {
                    mVEWebClient = new MVEWebClient();
                    mVEWebClient.GetToken();
                    _log.Error("ERROR" + " - " + "Put Payment Web Exception: " + wEx.Message + "---" + jsonBody);
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
                }
                else
                {
                    using (var stream = wEx.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            string strdeserializeObject = Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd()));
                            //	string objPay = _ITRansaction.SyncDataUpdate("pyt", AcctNo, true, false, strdeserializeObject, "");
                            _log.Error("ERROR" + " - " + "Put Payment Web Exception: " + strdeserializeObject + "---" + jsonBody);
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, strdeserializeObject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                _log.Error("ERROR" + " - " + "Put Payment Exception : " + objHttpResponseMessage);
                return objHttpResponseMessage;  //_IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }

        /// <summary>
        /// Get DeliveryConfirmation
        /// </summary>
        /// <param name="DeliveryConfirmation"></param>
        /// <returns></returns>
        [AcceptVerbs("GET")]
        [Route("DeliveryConfirmation")]
        public dynamic DeliveryConfirmation([FromUri]string AcctNo, string ID, string CheckOut)
        {
            MVEWebClient mVEWebClient = null;
            string jsonBody = string.Empty;
            try
            {
                DeliveryConfirm result = new DeliveryConfirm();
                result = _ITRansaction.CreateDeliveryConfirmation(AcctNo, CheckOut);
                string aPIResult = string.Empty;
                string postUrl = string.Empty;
                if (result.checkoutID != null)
                {
                    string putUrl = string.Format("{0}/{1}{2}", "v1", "DeliveryConfirmation", string.Format("/{0}", result.checkoutID));
                    mVEWebClient = new MVEWebClient(putUrl);
                    jsonBody = JsonConvert.SerializeObject(result);
                    aPIResult = mVEWebClient.ExecuteWebClient("PUT", "application/json; charset=utf-8", jsonBody, true);
                    _log.Info("Info : Send DeliveryConfirmation Json to MVE : " + jsonBody);
                    _log.Info("Info : Send DeliveryConfirmation Json to MVE Result : " + aPIResult);
                    StringBuilder sb = new StringBuilder();
                    foreach (items item in result.items)
                    {
                        sb.Append(',');
                        sb.Append(item.orderId);
                    }
                    sb.Remove(0, 1);
                    string orderid = sb.ToString();
                    string objDel = _ITRansaction.SyncDataUpdate("delc", AcctNo, true, false, "success", orderid, ID);
                    Thread.Sleep(3000);
                    _log.Info("Info : Send Delievry Confirmation Json to MVE Result : " + aPIResult);

                    //Payments Paymentresult = _ITRansaction.GetPaymentsOrderList(AcctNo);
                    //postUrl = string.Format("{0}/{1}?AcctNo={2}", "Transaction", "GetPayments", AcctNo);
                    //mVEWebClient = new MVEWebClient(postUrl, false, true);
                    //_log.Info("Info : Send Payments Json to MVE : " + postUrl);
                    //Paymentresult = mVEWebClient.ExecuteGetWebClient("application/json; charset=utf-8", true);
                    //_log.Info("Info : Send Payments Json to MVE Result : " + Paymentresult);
                }
                else
                {
                    string objDelC = _ITRansaction.SyncDataUpdate("delc", AcctNo, true, false, result.ToString(), "", ID);
                    Thread.Sleep(3000);
                    _log.Error("ERROR" + " - " + "Delivery Confirmation Message: " + result + "---" + jsonBody);
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No data found for Account code: " + AcctNo);
                }

                return Request.CreateResponse(HttpStatusCode.OK, aPIResult);  //_IErrorResponse.CreateExceptionResponse(ex.Message);
                                                                              // return Request.CreateResponse(HttpStatusCode.OK, result); //Testing
            }
            catch (WebException wEx)
            {
                if (wEx.Message.Contains("401"))
                {
                    mVEWebClient = new MVEWebClient();
                    mVEWebClient.GetToken();
                    _log.Error("ERROR" + " - " + "Delivery Confirmation Web Exception: " + wEx.Message);
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
                }
                else
                {
                    using (var stream = wEx.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            string strdeserializeObject = Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd()));
                            string objPay = _ITRansaction.SyncDataUpdate("delc", AcctNo, true, false, strdeserializeObject, "", ID);
                            Thread.Sleep(5000);
                            _log.Error("ERROR" + " - " + "Delivery Confirmation Web Exception else: " + strdeserializeObject + "---" + jsonBody);
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, strdeserializeObject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                _log.Error("ERROR" + " - " + "Delivery Confirmation Exception : " + objHttpResponseMessage + "---" + jsonBody);
                return objHttpResponseMessage;
            }
        }

        /// <summary>
		/// Create Customer Return Information
		/// </summary>
		/// <param name="Customer"></param>
		/// <returns>CustomerReturn Id</returns>
		[AcceptVerbs("POST")]
        [Route("CustomerReturns")]
        public dynamic CreateCustomerReturn([FromBody]CustomerReturnModel CR)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return _ITRansaction.CreateCustomerReturn(CR);
                    //  return _ICustomer.CreateUsers(objJSON);
                }
                else
                {
                    return _IErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
                return null;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }

        /// <summary>
        /// Get VendorReturnID Details 
        /// </summary>
        /// <param name="VendorReturnID"></param>
        /// <returns></returns>
        [AcceptVerbs("GET")]
        [Route("VendorReturns")]
        public dynamic VendorReturn([FromUri]string VendorReturnID, string ID)
        {
            MVEWebClient mVEWebClient = null;
            try
            {
                //List<Unicomer.Cosacs.Model.GRN> result = _ITRansaction.GetGRN(GRNNo);
                VendorReturnModel result = _ITRansaction.GetVendorReturn(VendorReturnID);
                string aPIResult = string.Empty;
                if (result != null)
                {
                    string postUrl = string.Format("{0}/{1}", "v1", "VendorReturns");
                    mVEWebClient = new MVEWebClient(postUrl);
                    string jsonBody = JsonConvert.SerializeObject(result);
                    _log.Info("Info : Send VendorReturn Json to MVE : " + jsonBody);
                    aPIResult = mVEWebClient.ExecuteWebClient("POST", "application/json; charset=utf-8", jsonBody, true);
                    _log.Info("Info : Send VendorReturn Json to MVE Result : " + aPIResult);
                    string objVendorReturn = _ITRansaction.SyncDataUpdate("vrn", VendorReturnID, true, false, "success", "", ID);
                }
                else
                {
                    string objVendorReturn = _ITRansaction.SyncDataUpdate("vrn", VendorReturnID, true, false, result.ToString(), "", ID);
                    _log.Error("ERROR" + " - " + "VendorReturn Message: " + result);
                    return Request.CreateResponse(HttpStatusCode.NotFound, result);
                }
                return Request.CreateResponse(HttpStatusCode.OK, aPIResult);

            }
            catch (WebException wEx)
            {
                if (wEx.Message.Contains("401"))
                {
                    mVEWebClient = new MVEWebClient();
                    mVEWebClient.GetToken();
                    _log.Error("ERROR" + " - " + "VendorReturn Web Exception: " + wEx.Message);
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
                }
                else
                {
                    using (var stream = wEx.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            string strdeserializeObject = Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd()));
                            string objPay = _ITRansaction.SyncDataUpdate("vrn", VendorReturnID, true, false, strdeserializeObject, "", ID);
                            _log.Error("ERROR" + " - " + "VendorReturn Web Exception else: " + strdeserializeObject);
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, strdeserializeObject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                _log.Error("ERROR" + " - " + "VendorReturn Exception : " + objHttpResponseMessage);
                return objHttpResponseMessage;

            }
        }

        /// <summary>
        /// Get Cancel Payments Order Wise
        /// </summary>
        /// <param name="AcctNo"></param>
        /// <returns>payments</returns>
        [AcceptVerbs("GET")]
        [Route("GetCancelPayments")]
        public dynamic GetCancelPaymentsOrderList([FromUri]string AcctNo, string ID)
        {
            MVEWebClient mVEWebClient = null;
            string jsonBody = string.Empty;
            try
            {
                //JResponse result = _ITRansaction.GetPaymentsOrderList(AcctNo);
                Payments result = _ITRansaction.GetCancelPaymentsOrderList(AcctNo);
                _log.Info("Info" + " - " + "CancelPayments Order List Message: " + result.checkoutId);
                string aPIResult = string.Empty;
                if (result.checkoutId != null)
                {
                    string postUrl = string.Format("{0}/{1}{2}", "v1", "Payments", string.Empty);
                    mVEWebClient = new MVEWebClient(postUrl);
                    jsonBody = JsonConvert.SerializeObject(result);
                    aPIResult = mVEWebClient.ExecuteWebClient("POST", "application/json; charset=utf-8", jsonBody, true);
                    _log.Info("Info : Send Get CancelPayments OrderList Json to MVE : " + jsonBody);
                    _log.Info("Info : Send Get CancelPayments OrderList Json to MVE Result : " + aPIResult);
                    //string objPay = _ITRansaction.SyncDataUpdate("pyt", AcctNo, true, false, "success", "");
                }
                else
                {
                    //	string objPay = _ITRansaction.SyncDataUpdate("pyt", AcctNo, true, false, result.Message, "");
                    _log.Error("ERROR" + " - " + "Cancel Payment Message checkoutId: " + result.checkoutId + "---" + jsonBody);
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No data found for Account code: " + AcctNo);
                }
                StringBuilder sb = new StringBuilder();
                foreach (Orders item in result.orderList)
                {
                    sb.Append(',');
                    sb.Append(item.orderId);
                }
                sb.Remove(0, 1);
                string orderid = sb.ToString();

                string objDel = _ITRansaction.SyncDataUpdate("pytc", AcctNo, true, false, "success", orderid, ID);
                return Request.CreateResponse(HttpStatusCode.Created, aPIResult);  //_IErrorResponse.CreateExceptionResponse(ex.Message);
            }
            catch (WebException wEx)
            {
                if (wEx.Message.Contains("401"))
                {
                    mVEWebClient = new MVEWebClient();
                    mVEWebClient.GetToken();
                    _log.Error("ERROR" + " - " + "Payment Web Exception: " + wEx.Message + "---" + jsonBody);
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
                }
                else
                {
                    using (var stream = wEx.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            string strdeserializeObject = Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd()));
                            //	string objPay = _ITRansaction.SyncDataUpdate("pyt", AcctNo, true, false, strdeserializeObject, "");
                            _log.Error("ERROR" + " - " + "CancelPayment Web Exception: " + strdeserializeObject + "---" + jsonBody);
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, strdeserializeObject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                _log.Error("ERROR" + " - " + "CancelPayment Exception : " + objHttpResponseMessage + "---" + jsonBody);
                return objHttpResponseMessage;  //_IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }

        /// <summary>
        /// Get Payments Order Wise
        /// </summary>
        /// <param name="AcctNo"></param>
        /// <returns>payments</returns>
        [AcceptVerbs("GET")]
        [Route("GetCancelPaymentsPut")]
        public dynamic GetCancelPaymentsOrderListPut([FromUri]string AcctNo, string ID)
        {
            MVEWebClient mVEWebClient = null;
            string jsonBody = string.Empty;
            try
            {
                //JResponse result = _ITRansaction.GetPaymentsOrderList(AcctNo);
                Payments result = _ITRansaction.GetPaymentsOrderList(AcctNo, ID);
                _log.Info("Info" + " - " + "Put CancelPayments Order List Message: " + result.checkoutId);
                string aPIResult = string.Empty;
                if (result.checkoutId != null)
                {
                    //string postUrl = string.Format("{0}/{1}{2}", "v1", "Payments", string.Empty);
                    string putUrl = string.Format("{0}/{1}{2}", "v1", "Payments", string.Format("/{0}", result.externalPaymentID));
                    mVEWebClient = new MVEWebClient(putUrl);
                    jsonBody = JsonConvert.SerializeObject(result);
                    aPIResult = mVEWebClient.ExecuteWebClient("PUT", "application/json; charset=utf-8", jsonBody, true);
                    _log.Info("Info : Send Get CancelPayments OrderList Put Json to MVE : " + jsonBody);
                    _log.Info("Info : Send Get CancelPayments OrderList Put Json to MVE Result : " + aPIResult);
                    //string objPay = _ITRansaction.SyncDataUpdate("pyt", AcctNo, true, false, "success", "");
                }
                else
                {
                    //	string objPay = _ITRansaction.SyncDataUpdate("pyt", AcctNo, true, false, result.Message, "");
                    _log.Error("ERROR" + " - " + "Put CancelPayment Message checkoutId: " + result.checkoutId + "---" + jsonBody);
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No data found for Account code: " + AcctNo);
                }
                StringBuilder sb = new StringBuilder();
                foreach (Orders item in result.orderList)
                {
                    sb.Append(',');
                    sb.Append(item.orderId);
                }
                sb.Remove(0, 1);
                string orderid = sb.ToString();

                string objDel = _ITRansaction.SyncDataUpdate("pytc", AcctNo, true, false, "success", orderid, ID);
                return Request.CreateResponse(HttpStatusCode.Created, aPIResult);  //_IErrorResponse.CreateExceptionResponse(ex.Message);
            }
            catch (WebException wEx)
            {
                if (wEx.Message.Contains("401"))
                {
                    mVEWebClient = new MVEWebClient();
                    mVEWebClient.GetToken();
                    _log.Error("ERROR" + " - " + "Put CancelPayment Web Exception: " + wEx.Message + "---" + jsonBody);
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, wEx.Message);
                }
                else
                {
                    using (var stream = wEx.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            string strdeserializeObject = Convert.ToString(JsonConvert.DeserializeObject(reader.ReadToEnd()));
                            //	string objPay = _ITRansaction.SyncDataUpdate("pyt", AcctNo, true, false, strdeserializeObject, "");
                            _log.Error("ERROR" + " - " + "Put CancelPayment Web Exception: " + strdeserializeObject + "---" + jsonBody);
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, strdeserializeObject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                _log.Error("ERROR" + " - " + "Put CancelPayment Exception : " + objHttpResponseMessage);
                return objHttpResponseMessage;  //_IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }
    }
}
