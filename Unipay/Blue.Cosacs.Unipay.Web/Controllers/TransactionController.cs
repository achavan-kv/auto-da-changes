using Blue.Cosacs.Unipay.Web.Model;
using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using Unicomer.Cosacs.Business;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;


namespace Blue.Cosacs.Unipay.Web.Controllers
{
    [RoutePrefix("api/Transaction")]
    public class TransactionController : ApiController
    {
        private readonly ITransaction _iTRansaction;
        private readonly IErrorResponse _iErrorResponse;

        public TransactionController()
        {
            _iTRansaction = new TransactionBusiness();
            _iErrorResponse = new JResponseError();// = new TransactionBusiness();
        }

        #region getUserAccounts

        /// <summary>
        /// Get Login user credit details- Not in use(It is same for story 5 & story 7)
        /// As per YP this is Story No.5 : 
        /// </summary>
        /// <param name="objCustId"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Route("getUserAccounts")]
        public dynamic Get([FromUri]string CustId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return _iTRansaction.GetUserAccounts(CustId);
                }
                else
                {
                    return _iErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                global::Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _iErrorResponse.CreateExceptionResponse(ex.Message);
            }

        }

        #endregion

        #region CreditApplication
        /// <summary>
        /// Read Login user credit details
        /// As per YP this is Story No.: 
        /// </summary>
        /// <param name="objCustId"></param>
        /// <param name="Country"></param>
        /// <returns>Account Id</returns>
        [AcceptVerbs("Post")]
        [Route("CreditApplication")]
        public dynamic Post([FromUri]string custId, [FromBody]AnswerModel creditAnsModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    creditAnsModel.custId = custId;
                    return _iTRansaction.CreateRFAccount(creditAnsModel);
                }
                else
                {
                    return _iErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                global::Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _iErrorResponse.CreateExceptionResponse(ex.Message);
            }


        }
        #endregion

        #region getCreditAppQuestions
        /// <summary>
        /// Read Credit App Questions.
        /// As per YP this is Story No.: 
        /// </summary>
        /// <param name="custId"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Route("getCreditAppQuestions")]
        public dynamic GetCreditAppQuestions([FromUri]string custId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return _iTRansaction.GetCreditAppQuestions(custId);
                }
                else
                {
                    return _iErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                global::Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _iErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }

        #endregion

        #region getContract
        /// <summary>
        /// Get contract PDF byte array with file name.
        /// As per YP this is Story No.: 
        /// </summary>
        /// <param name="CustId"></param>
        /// <param name="objgetContarct"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        [Route("getContract")]
        public dynamic Get([FromUri]string CustId, [FromBody]GetContract objgetContarct)
        {
            try
            {
                if (objgetContarct != null && ModelState.IsValid)
                {
                    objgetContarct.CustId = CustId;
                    return _iTRansaction.GetContractPDF(objgetContarct);
                }
                else
                {
                    if (objgetContarct == null)
                    {
                        return _iErrorResponse.CreateErrorResponse("The CustId field is required.");
                    }
                    else
                    {
                        return _iErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    }
                }
            }
            catch (Exception ex)
            {
                global::Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _iErrorResponse.CreateExceptionResponse(ex.Message);
            }

        }
        #endregion

        #region SendContractdetailsConf
        /// <summary>
        /// To upload image and send mail format details back in response.
        /// As per YP this is Story No.: 
        /// </summary>
        /// <param name="CustId"></param>
        /// <param name="signedDocumentDetails"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        [Route("SendContractdetailsConf")]
        public dynamic Post([FromUri]string CustId, [FromBody]SignedDocumentFile signedDocumentDetails)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(CustId))
                {
                    signedDocumentDetails.FileName = "test"; //Dummy parameter
                    UploadDocument uploadDocument = new UploadDocument();
                    HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                    uploadDocument.UploadedFiles = files.Count > 0 ? files : null;
                    uploadDocument.ByteArrayFile = (signedDocumentDetails != null && signedDocumentDetails.signedContract != null) ? signedDocumentDetails.signedContract : null;
                    uploadDocument.CustId = CustId;
                    uploadDocument.AccountNumber = (signedDocumentDetails != null && !string.IsNullOrWhiteSpace(signedDocumentDetails.accountNumber)) ? signedDocumentDetails.accountNumber : string.Empty;
                    if (!string.IsNullOrWhiteSpace(uploadDocument.AccountNumber))
                        uploadDocument.TargetFolderPath = string.Format("{0}{1}//{2}//", System.Configuration.ConfigurationManager.AppSettings["UploadDocumentTargetFolderPath"], CustId, uploadDocument.AccountNumber);
                    else
                        uploadDocument.TargetFolderPath = string.Format("{0}{1}//", System.Configuration.ConfigurationManager.AppSettings["UploadDocumentTargetFolderPath"], CustId);
                    uploadDocument.TargetFileName = string.Format("SignedContract_{0}_{1}", CustId, DateTime.Now.ToString("yyyyMMddHHmmss"));
                    uploadDocument.UploadedFileName = (signedDocumentDetails != null && !string.IsNullOrWhiteSpace(signedDocumentDetails.FileName)) ? signedDocumentDetails.FileName : string.Format("{0}_{1}.html", CustId, DateTime.Now.ToString("yyyyMMddHHmmss"));
                    return _iTRansaction.UploadContractDocuments(uploadDocument, uploadDocument.AccountNumber, true);
                }
                else
                {
                    return _iErrorResponse.CreateErrorResponse(string.Join(" | ", "The CustId field is required."));
                }
            }
            catch (Exception ex)
            {
                global::Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _iErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }
        #endregion

        #region getCustomerCreditSummary
        /// <summary>
        /// Read customer credit summary details
        /// As per YP this is Story No.: 
        /// </summary>
        /// <param name="CustId"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Route("getCustomerCreditSummary")]
        public dynamic getCustomerCreditSummary([FromUri]string CustId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return _iTRansaction.GetCustomerCreditSummary(CustId);
                }
                else
                {
                    return _iErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                global::Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _iErrorResponse.CreateExceptionResponse(ex.Message);
            }

        }
        #endregion

        #region getUserTransactions
        /// <summary>
        /// Read customer credit summary details
        /// As per YP this is Story No.6: 
        /// </summary>
        /// <param name="objUserTransactionInputModel"></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Route("getUserTransactions")]
        public dynamic GetUserTransactions([FromUri]UserTransactionInputModel objUserTransactionInputModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return _iTRansaction.GetUserTransactions(objUserTransactionInputModel);
                }
                else
                {
                    return _iErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                global::Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _iErrorResponse.CreateExceptionResponse(ex.Message);
            }

        }
        #endregion

        #region GetCreditStatus
        /// <summary>
        /// Read the Create RF account dcoument status
        /// As per YP this is Story No.: 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Route("GetCreditStatus")]
        public dynamic Get()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return _iTRansaction.getDocumentStatus();
                }
                else
                {
                    return _iErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                global::Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _iErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }
        #endregion

        #region TPConfirmTransaction
        /// <summary>
        /// Register new account for third party
        /// As per YP this is Story No.: 
        /// </summary>
        [AcceptVerbs("Post")]
        [Route("TPConfirmTransaction")]
        public dynamic Post([FromUri]string custId, [FromBody]TPTransactionConfirm tPTransactionConfirm)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(custId) && ModelState.IsValid)
                {
                    //creditAnsModel.custId = custId;
                    return _iTRansaction.TPCreateRFAccount(custId, tPTransactionConfirm);
                }
                else
                {
                    return _iErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                global::Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _iErrorResponse.CreateExceptionResponse(ex.Message);
            }


        }
        #endregion

        #region UpdateTransaction
        /// <summary>
        /// Update transaction
        /// As per YP this is Story No.: 
        /// </summary>
        [AcceptVerbs("Put")]
        [Route("UpdateTransaction")]
        public dynamic Put([FromUri]UpdateTransactionQueryString modelUpdateTransactionQueryString, [FromBody]UpdateTransactionBody modelUpdateTransactionBody)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //creditAnsModel.custId = custId;
                    return _iTRansaction.UpdateTransaction(modelUpdateTransactionQueryString, modelUpdateTransactionBody);
                }
                else
                {
                    return _iErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                global::Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _iErrorResponse.CreateExceptionResponse(ex.Message);
            }


        }
        #endregion

        #region getEmailContract
        /// <summary>
        /// Get EmailContract
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("Get")]
        [Route("getEmailContract")]
        public dynamic getEmailContract()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return _iTRansaction.getEmailContract();
                }
                else
                {
                    return _iErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                global::Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _iErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }
        #endregion

        #region TP getContract
        /// <summary>
        /// Get TP contract PDF byte array with file name.
        /// As per YP this is Story No. 27 : 
        /// </summary>
        /// <param name="CustId"></param>
        /// <param name="objgetContarct"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        [Route("getTPContract")]
        public dynamic Get([FromUri]string CustId, [FromBody]GetTPContractAccount objgetContarct)
        {
            try
            {
                GetContract objgetTPContarct = new GetContract();
                if (objgetContarct != null && ModelState.IsValid)
                {
                    objgetTPContarct.CustId = CustId;
                    objgetTPContarct.accountNumber = objgetContarct.accountNumber;
                    return _iTRansaction.GetContractPDF(objgetTPContarct);
                }
                else
                {
                    if (objgetContarct == null)
                    {
                        return _iErrorResponse.CreateErrorResponse("The CustId field is required.");
                    }
                    else
                    {
                        return _iErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    }
                }
            }
            catch (Exception ex)
            {
                global::Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _iErrorResponse.CreateExceptionResponse(ex.Message);
            }

        }
        #endregion

        #region Update Contract Notification Status
        /// <summary>
        /// Update Contract Notification Status
        /// As per YP this is Story No.: 
        /// </summary>
        [AcceptVerbs("Put")]
        [Route("UpdateContractNotificationStatus")]
        public dynamic UpdateContractNotificationStatus([FromBody]ContractNotificationStatus objContractNotificationStatus)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return _iTRansaction.UpdateContractNotificationStatus(objContractNotificationStatus);
                }
                else
                {
                    //   return _IErrorResponse.CreateErrorResponse(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
                return null;
            }
            catch (Exception ex)
            {
                global::Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return _iErrorResponse.CreateExceptionResponse(ex.Message);
            }
        }
        #endregion

    }
}