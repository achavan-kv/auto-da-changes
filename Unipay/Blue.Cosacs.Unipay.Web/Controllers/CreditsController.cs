using System;
using System.Linq;
using System.Web.Http;
using Unicomer.Cosacs.Business;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;

namespace Blue.Cosacs.Unipay.Web.Controllers
{
    [RoutePrefix("api/Credits")]
    public class CreditsController : ApiController
    {
        private readonly ICredits _ICredits;
        private readonly IErrorResponse _IErrorResponse;

        public CreditsController()
        {
            _ICredits = new CreditsBuiness();
            _IErrorResponse = new JResponseError(); ;
        }

        #region GetMaxWithdrawalAmountRepository 
        /// <summary>
        /// returns Max available withdrawal amount.
        /// </summary>
        /// <param name="CustId"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Route("getMaxWithdrawalAmount")]
        public dynamic GetMaxWithdrawalAmount([FromUri]string CustId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(CustId))
                {
                    return _ICredits.GetMaxWithdrawalAmount(CustId);
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
        #endregion

        #region GetPaymentOptionsByAmount 
        /// <summary>
        /// Read the Create RF account dcoument status
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Route("GetPaymentOptionsByAmount")]
        public dynamic Get([FromUri] string CustId, [FromUri] string loanAmount)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(CustId) && !string.IsNullOrWhiteSpace(loanAmount))
                {
                    return _ICredits.GetPaymentOptionsByAmount(CustId, loanAmount);
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
        #endregion

        #region Update Credit Information 
        /// <summary>
        ///update credit information.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [AcceptVerbs("Put")]
        [Route("UpdateCreditInformation")]
        public dynamic Put([FromUri] string CustId,[FromBody]CreditInformation CrINformation)
        {
            try
            {
                if (!string.IsNullOrEmpty(CustId))
                {
                    CrINformation.CustId = CustId;
                    return _ICredits.UpdateCreditInformation(CrINformation);
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
        #endregion


    }
}
