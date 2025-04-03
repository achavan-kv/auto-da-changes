using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Unicomer.Cosacs.Business;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.WebApi.Areas.Customer.Controllers
{
    [RoutePrefix("api/Customer")]
    public class CustomerController : ApiController
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICustomer _ICustomer;
        private readonly IErrorResponse _IErrorResponse;

        public CustomerController()
        {
            log4net.GlobalContext.Properties["LogFileName"] = ConfigurationManager.AppSettings["LogFolderPath"] != null ? ConfigurationManager.AppSettings["LogFolderPath"] : @"C://MVE_Unicomer//Log//VE_COSACS_";
            log4net.Config.XmlConfigurator.Configure();
            _ICustomer = new CustomerBusiness();
            _IErrorResponse = new JResponseError(); ;
        }
        //[AcceptVerbs("Get")]
        //[Route("getParentSKU")]
        //public HttpResponseMessage GetParentSKUMaster()
        //{
        //    try
        //    {
        //        JResponse result = _ICustomer.GetParentSKUMaster();
        //        if (result != null && result.StatusCode.Equals(200) && !string.IsNullOrWhiteSpace(result.Result))
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, result);
        //        }
        //        return new HttpResponseMessage(HttpStatusCode.NotFound);
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //        return new HttpResponseMessage(HttpStatusCode.InternalServerError);  //_IErrorResponse.CreateExceptionResponse(ex.Message);
        //    }
        //}

        /// <summary>
        /// Send Vendor Master Data Information
        /// </summary>
        /// <param name="getVendor"></param>
        /// <returns></returns>
        //[AcceptVerbs("Get")]
        //[Route("getVendor")]
        //public dynamic GetSupplierMaster()
        //{
        //    try
        //    {
        //        return _ICustomer.GetSupplierMaster();
        //    }
        //    catch (Exception ex)
        //    {
        //        return _IErrorResponse.CreateExceptionResponse(ex.Message);
        //    }
        //}

        [AcceptVerbs("Post")]
        [Route("CreateCustomer")]
        public dynamic CreateCustomer([FromBody]UserJson objCustomer)
        {
            try
            {
                _log.Info("Info Create Customer initialze" + " - " + objCustomer);
                if (ModelState.IsValid)
                {
                    return _ICustomer.CreateCustomer(objCustomer);
                }
                else
                {
                    _log.Info("Info else Create Customer initialze" + " - " + objCustomer);
                    return _IErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error Exception" + " - " + ex.Message);
                return _IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerRequest"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        [Route("SearchCustomer")]
        public dynamic SearchCustomer([FromBody]CustomerRequest objCustomerSearch)
        {
            try
            {
                _log.Info("Info Search Customer initialze" + " - " + objCustomerSearch);
                if (ModelState.IsValid)
                    return _ICustomer.SearchCustomer(objCustomerSearch);
                else
                    _log.Info("Info else Search Customer initialze" + " - " + objCustomerSearch);
                return _IErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            }
            catch (Exception ex)
            {
                _log.Error("ERROR else Search Customer initialze" + " - " + objCustomerSearch);
                return _IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }
        [AcceptVerbs("Put")]
        [Route("UpdateCustomer")]
        public dynamic UpdateCustomer([FromBody]UserJson objCustomerUpdate)
        {
            try
            {
                _log.Info("Info Update Customer initialze" + " - " + objCustomerUpdate);
                if (ModelState.IsValid)
                {
                    return _ICustomer.UpdateCustomer(objCustomerUpdate);
                }
                else
                {
                    _log.Info("Info else Update Customer initialze" + " - " + objCustomerUpdate);
                    return _IErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                _log.Error("ERROR else Update Customer initialze" + " - " + objCustomerUpdate);
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }
    }
}
