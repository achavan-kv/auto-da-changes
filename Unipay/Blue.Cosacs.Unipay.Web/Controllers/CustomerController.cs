using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using Unicomer.Cosacs.Business;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;

namespace Blue.Cosacs.Unipay.Web.Controllers
{
    [RoutePrefix("api/Customer")]
    public class CustomerController : ApiController
    {
        private readonly ICustomer _ICustomer;
        private readonly IErrorResponse _IErrorResponse;

        /// <summary>
        /// Initialize interface
        /// </summary>        
        public CustomerController()
        {
            _ICustomer = new CustomerBuiness();
            _IErrorResponse = new JResponseError(); ;
        }

        /// <summary>
        /// Validate Customer by using the objValidateUser model.
        /// </summary>
        /// <param name="objValidateUser"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Route("getUserUnicomer")]
        public dynamic Get([FromUri] ValidatetUser objValidateUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return _ICustomer.ValidateUser(objValidateUser);
                }
                else
                {
                    return _IErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                global::Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }

        /// <summary>
        /// Create new customer in Cosacs system.
        /// </summary>
        /// <param name="objUser"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        [Route("CreateUser")]
        public dynamic Post([FromBody]User objUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return _ICustomer.CreateUser(objUser);
                }
                else
                {
                    return _IErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                global::Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }

        /// <summary>
        /// Get pre defined question and answers for existing customer authentication.
        /// </summary>
        /// <param name="CustId"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Route("getAuthQAndA")]
        public dynamic Get([FromUri]string CustId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return _ICustomer.getAuthQAndA(CustId);
                }
                else
                {
                    return _IErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                global::Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }

        /// <summary>
        /// Update customer information
        /// Right now not in scope.
        /// </summary>
        /// <param name="objUpdateUser"></param>
        /// <returns></returns>
        [AcceptVerbs("Put")]
        [Route("UpdateUser")]
        public dynamic Put([FromUri]string CustId, [FromBody]UpdateUser objUpdateUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //objUpdateUser.CustID = CustId;
                    return _ICustomer.UpdateUser(objUpdateUser, CustId);
                }
                else
                {
                    JResponse objJResponse = new JResponse();
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    objJResponse.Message = (string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    return objJResponse;
                    //return _IErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                global::Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _IErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }
    }
}
