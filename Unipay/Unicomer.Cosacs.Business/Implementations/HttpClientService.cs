using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using STL.Common.Constants.ColumnNames;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Routing;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Model.Models.Deliveries;
using Unicomer.Cosacs.Model.Models.Loans;
using Unicomer.Cosacs.Model.Models.Loans.Repayment;
using Unicomer.Cosacs.Model.Models.Loans.Reverse;
using Unicomer.Cosacs.Model.Models.Loans.Schedule;
using Unicomer.Cosacs.Model.Models.Payments;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Business.Implementations
{
    internal class HttpClientService : IHttpClientService
    {
        private const string TraceIdHeader = "X-B3-TraceId";
        private const string ResponseRequestIdHeader = "x-request-id";

        private string emmaToken = "";
        private string mambuToken = "";
        private string workflowToken = "";
        private readonly IDbLogggerRepository dbLoggger;

        public HttpClientService(IDbLogggerRepository dbLoggger)
        {
            this.dbLoggger = dbLoggger;
        }

        public async Task<LoanScheduleModel> GetLoanSchedule(string mambuAccountId, string processRef, string typeProduct, IApiRequest apiRequest)
        {
            string apiName = "loans-schedule";
            var apiRequestModel = apiRequest.GetRequest();

            string url = ApplicationSettingManager.MambuBaseUrl + "/loans/" + mambuAccountId + "/schedule";

            IFlurlRequest request = null;
            IDictionary<string, object> requestHeader = null;
            object requestTraceId = "";
            IEnumerable<string> responseTraceIds = null;
            string responseContent = "";
            HttpResponseHeaders responseHeaders = null;

            try
            {
                string token = await GetMambuToken(apiRequestModel, apiName);

                request = CreateRequest(url, token, processRef, typeProduct, false);
                requestHeader = request.Headers;


                requestHeader.TryGetValue(TraceIdHeader, out requestTraceId);

                var response = await request.GetAsync();

                responseHeaders = response.Headers;
                responseHeaders.TryGetValues(ResponseRequestIdHeader, out responseTraceIds);

                var payload = await ReadFromJsonAsync<LoanScheduleResponseModel>(response);
                responseContent = payload.Content;

                if (payload.Success)
                {
                    dbLoggger.LogApiSuccess(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, "", responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                }
                else
                {
                    var errorResponse = HandleMambuError(responseContent);

                    dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, "", errorResponse, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);

                    throw new MambuApiException(errorResponse, new Exception("Response Error:" + responseContent), new { mambuAccountId, processRef, typeProduct, url });
                }

                return payload == null ? null : payload.Object == null ? null : payload.Object.Data;
            }
            catch (MambuApiException)
            {
                throw;
            }
            catch (FlurlHttpException ex)
            {
                var errorResponse = HandleMambuError(ex, out responseContent);

                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, "", errorResponse, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);

                throw new MambuApiException(errorResponse, ex, new { mambuAccountId, processRef, typeProduct, url });
            }
            catch (Exception ex)
            {
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, "", ex.Message, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);

                throw new ApiException(ex, new { mambuAccountId, processRef, typeProduct, url });
            }
        }

        public async Task<PreviewPayOffCLAmountModel> LoanPreviewPayOffCL(string mambuAccountId, string processRef, DateTime valueDate, IApiRequest apiRequest)
        {
            string apiName = "loans-previewPayOffAmounts";
            var apiRequestModel = apiRequest.GetRequest();

            string url = ApplicationSettingManager.MambuBaseUrl + "/loan-transactions/" + mambuAccountId + "/preview-payoff-cl";
            var date = DateTimeOffset.Now;
            var apiDate = date.ToOffset(new TimeSpan(-5, 0, 0));
            var requestContent = new { valueDate = apiDate.ToString("yyyy-MM-ddThh:mm:ssK") };

            IFlurlRequest request = null;
            IDictionary<string, object> requestHeader = null;
            object requestTraceId = "";
            IEnumerable<string> responseTraceIds = null;
            string responseContent = "";
            HttpResponseHeaders responseHeaders = null;

            try
            {
                string token = await GetMambuToken(apiRequestModel, apiName);

                request = CreateRequest(url, token, processRef, ApplicationSettingManager.XtypeProductCL, false);
                requestHeader = request.Headers;

                requestHeader.TryGetValue(TraceIdHeader, out requestTraceId);

                var response = await request.PostJsonAsync(requestContent);

                responseHeaders = response.Headers;
                responseHeaders.TryGetValues(ResponseRequestIdHeader, out responseTraceIds);

                var payload = await ReadFromJsonAsync<PreviewPayOffCLAmountResponseModel>(response);
                responseContent = payload.Content;

                if (payload.Success)
                {
                    dbLoggger.LogApiSuccess(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                }
                else
                {
                    var errorResponse = HandleMambuError(responseContent);

                    dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, errorResponse, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                    throw new MambuApiException(errorResponse, new Exception("Response Error:" + responseContent), new { mambuAccountId, processRef, url, valueDate, apiDate = apiDate.ToString("yyyy-MM-ddThh:mm:ssK") });
                }

                return payload == null ? null : payload.Object == null ? null : payload.Object.Data; ;
            }
            catch (MambuApiException)
            {
                throw;
            }
            catch (FlurlHttpException ex)
            {
                var errorResponse = HandleMambuError(ex, out responseContent);
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, errorResponse, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                throw new MambuApiException(errorResponse, ex, new { mambuAccountId, processRef, url, valueDate, apiDate = apiDate.ToString("yyyy-MM-ddThh:mm:ssK") });
            }
            catch (Exception ex)
            {
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, ex.Message, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);

                throw new ApiException(ex, new { mambuAccountId, processRef, url, valueDate, apiDate = apiDate.ToString("yyyy-MM-ddThh:mm:ssK") });
            }
        }

        public async Task<PreviewPayOffHPAmountModel> LoanPreviewPayOffHP(string mambuAccountId, string processRef, IApiRequest apiRequest)
        {
            string apiName = "loans-previewPayoff-hp";

            var apiRequestModel = apiRequest.GetRequest();
            string url = ApplicationSettingManager.MambuBaseUrl + "/loan-transactions/" + mambuAccountId + "/preview-payoff-hp";
            var requestContent = new { };

            IFlurlRequest request = null;
            IDictionary<string, object> requestHeader = null;
            object requestTraceId = "";
            IEnumerable<string> responseTraceIds = null;
            string responseContent = "";
            HttpResponseHeaders responseHeaders = null;

            try
            {
                string token = await GetMambuToken(apiRequestModel, apiName);

                request = CreateRequest(url, token, processRef, ApplicationSettingManager.XtypeProductHP, false);
                requestHeader = request.Headers;

                requestHeader.TryGetValue(TraceIdHeader, out requestTraceId);

                var response = await request.PostJsonAsync(requestContent);

                responseHeaders = response.Headers;
                responseHeaders.TryGetValues(ResponseRequestIdHeader, out responseTraceIds);

                var payload = await ReadFromJsonAsync<PreviewPayOffHPAmountResponseModel>(response);
                responseContent = payload.Content;

                if (payload.Success)
                {
                    dbLoggger.LogApiSuccess(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                }
                else
                {
                    var errorResponse = HandleMambuError(responseContent);

                    dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, errorResponse, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                    throw new MambuApiException(errorResponse, new Exception("Response Error:" + responseContent), new { mambuAccountId, processRef, url });
                }

                return payload == null ? null : payload.Object == null ? null : payload.Object.Data; ;
            }
            catch (MambuApiException)
            {
                throw;
            }
            catch (FlurlHttpException ex)
            {
                MambuErrorModel errorResponse = HandleMambuError(ex, out responseContent);
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, errorResponse, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                throw new MambuApiException(errorResponse, ex, new { mambuAccountId, processRef, url });
            }
            catch (Exception ex)
            {
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, ex.Message, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);

                throw new ApiException(ex, new
                {
                    mambuAccountId,
                    processRef,
                    url
                });
            }
        }

        public async Task<LoanRepaymentModel> LoanPayment(string mambuAccountId, decimal amount, string transactionChannelId, string notes, string typeProduct, IApiRequest apiRequest)
        {
            string apiName = "loans-repayment";
            var apiRequestModel = apiRequest.GetRequest();

            string url = ApplicationSettingManager.MambuBaseUrl + "/loan-transactions/" + mambuAccountId + "/payment";

            var requestContent = new
            {
                amount,
                transactionDetails = new
                {
                    transactionChannelId
                },
                notes
            };

            IFlurlRequest request = null;
            IDictionary<string, object> requestHeader = null;
            object requestTraceId = "";
            IEnumerable<string> responseTraceIds = null;
            string responseContent = "";
            HttpResponseHeaders responseHeaders = null;

            try
            {
                string token = await GetMambuToken(apiRequestModel, apiName);

                request = CreateRequest(url, token, ApplicationSettingManager.Payment, typeProduct, true);
                requestHeader = request.Headers;

                requestHeader.TryGetValue(TraceIdHeader, out requestTraceId);

                var response = await request.PostJsonAsync(requestContent);

                responseHeaders = response.Headers;
                responseHeaders.TryGetValues(ResponseRequestIdHeader, out responseTraceIds);

                var payload = await ReadFromJsonAsync<LoanRepaymentResponseModel>(response);
                responseContent = payload.Content;

                if (payload.Success)
                {
                    dbLoggger.LogApiSuccess(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                }
                else
                {
                    var errorResponse = HandleMambuError(responseContent);

                    dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, errorResponse, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                    throw new MambuApiException(errorResponse, new Exception("Response Error:" + responseContent), new { mambuAccountId, typeProduct, url, amount });
                }

                return payload == null ? null : payload.Object == null ? null : payload.Object.Data; ;
            }
            catch (MambuApiException)
            {
                throw;
            }
            catch (FlurlHttpException ex)
            {
                var errorResponse = HandleMambuError(ex, out responseContent);
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, errorResponse, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                throw new MambuApiException(errorResponse, ex, new { mambuAccountId, typeProduct, url, amount });
            }
            catch (Exception ex)
            {
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, ex.Message, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                throw new ApiException(ex, new { mambuAccountId, typeProduct, url, amount });
            }
        }

        public async Task<LoanReverseAdjustModel> LoanReverse(string loanTransactionId, string notes, string typeProduct, IApiRequest apiRequest)
        {
            string apiName = "loans-adjust";
            var apiRequestModel = apiRequest.GetRequest();
            string url = ApplicationSettingManager.MambuBaseUrl + "/loan-transactions/payment/" + loanTransactionId + "/reverse";
            var requestContent = new { notes = notes };

            IFlurlRequest request = null;
            IDictionary<string, object> requestHeader = null;
            object requestTraceId = "";
            IEnumerable<string> responseTraceIds = null;
            string responseContent = "";
            HttpResponseHeaders responseHeaders = null;

            try
            {
                string token = await GetMambuToken(apiRequestModel, apiName);

                request = CreateRequest(url, token, ApplicationSettingManager.Reverse, typeProduct, true);
                requestHeader = request.Headers;

                requestHeader.TryGetValue(TraceIdHeader, out requestTraceId);

                var response = await request.PostJsonAsync(requestContent);

                responseHeaders = response.Headers;

                responseHeaders.TryGetValues(ResponseRequestIdHeader, out responseTraceIds);

                var payload = await ReadFromJsonAsync<LoanReverseAdjustResponseModel>(response);

                responseContent = payload.Content;

                if (payload.Success)
                {
                    dbLoggger.LogApiSuccess(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                }
                else
                {
                    var errorResponse = HandleMambuError(responseContent);

                    dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, errorResponse, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                    throw new MambuApiException(errorResponse, new Exception("Response Error:" + responseContent), new { loanTransactionId, typeProduct, url, notes });
                }

                return payload == null ? null : payload.Object == null ? null : payload.Object.Data; ;
            }
            catch (MambuApiException)
            {
                throw;
            }
            catch (FlurlHttpException ex)
            {
                var errorResponse = HandleMambuError(ex, out responseContent);
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, errorResponse, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                throw new MambuApiException(errorResponse, ex, new { loanTransactionId, typeProduct, url, notes });
            }
            catch (Exception ex)
            {
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, ex.Message, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                throw new ApiException(ex, new { loanTransactionId, typeProduct, url, notes });
            }
        }

        public async Task<bool> LoanPayOffCL(string mambuAccountId, decimal amount, string transactionChannelId, string notes, IApiRequest apiRequest)
        {
            string apiName = "loans-payOff";
            var apiRequestModel = apiRequest.GetRequest();

            string url = ApplicationSettingManager.MambuBaseUrl + "/loan-transactions/" + mambuAccountId + "/payoff-cl";

            var requestContent = new
            {
                amount,
                transactionDetails = new
                {
                    transactionChannelId
                },
                notes
            };

            IFlurlRequest request = null;
            IDictionary<string, object> requestHeader = null;
            object requestTraceId = "";
            IEnumerable<string> responseTraceIds = null;
            string responseContent = "";
            HttpResponseHeaders responseHeaders = null;

            try
            {
                string token = await GetMambuToken(apiRequestModel, apiName);
                request = CreateRequest(url, token,
                                                    ApplicationSettingManager.Payoff,
                                                    ApplicationSettingManager.XtypeProductCL,
                                                    true);
                requestHeader = request.Headers;

                requestHeader.TryGetValue(TraceIdHeader, out requestTraceId);

                var response = await request.PostJsonAsync(requestContent);

                responseHeaders = response.Headers;
                responseHeaders.TryGetValues(ResponseRequestIdHeader, out responseTraceIds);

                var payload = await ReadFromJsonAsync<ApiResultResponse>(response);
                responseContent = payload.Content;

                if (payload.Success)
                {
                    dbLoggger.LogApiSuccess(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                }
                else
                {
                    var errorResponse = HandleMambuError(responseContent);

                    dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, errorResponse, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                    throw new MambuApiException(errorResponse, new Exception("Response Error:" + responseContent), new { mambuAccountId, url });
                }

                return true;
            }
            catch (MambuApiException)
            {
                throw;
            }
            catch (FlurlHttpException ex)
            {
                var errorResponse = HandleMambuError(ex, out responseContent);
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, errorResponse, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                throw new MambuApiException(errorResponse, ex, new { mambuAccountId, url });
            }
            catch (Exception ex)
            {
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, ex.Message, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                throw new ApiException(ex, new { mambuAccountId, url });
            }
        }

        public async Task<bool> LoanPayOffHP(string mambuAccountId, decimal amount, string transactionChannelId, string notes, IApiRequest apiRequest)
        {
            string apiName = "loans-payOff-hp";
            var apiRequestModel = apiRequest.GetRequest();
            string url = ApplicationSettingManager.MambuBaseUrl + "/loan-transactions/" + mambuAccountId + "/payoff-hp";

            var requestContent = new
            {
                amount,
                transactionDetails = new
                {
                    transactionChannelId
                },
                notes
            };

            IFlurlRequest request = null;
            IDictionary<string, object> requestHeader = null;
            object requestTraceId = "";
            IEnumerable<string> responseTraceIds = null;
            string responseContent = "";
            HttpResponseHeaders responseHeaders = null;

            try
            {
                string token = await GetMambuToken(apiRequestModel, apiName);

                request = CreateRequest(url, token,
                                                ApplicationSettingManager.Payoff,
                                                ApplicationSettingManager.XtypeProductHP,
                                                true);
                requestHeader = request.Headers;

                requestHeader.TryGetValue(TraceIdHeader, out requestTraceId);

                var response = await request.PostJsonAsync(requestContent);

                responseHeaders = response.Headers;
                responseHeaders.TryGetValues(ResponseRequestIdHeader, out responseTraceIds);

                var payload = await ReadFromJsonAsync<ApiResultResponse>(response);
                responseContent = payload.Content;

                if (payload.Success)
                {
                    dbLoggger.LogApiSuccess(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                }
                else
                {
                    var errorResponse = HandleMambuError(responseContent);

                    dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, errorResponse, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                    throw new MambuApiException(errorResponse, new Exception("Response Error:" + responseContent), new { mambuAccountId, url });
                }

                return true;
            }
            catch (MambuApiException)
            {
                throw;
            }
            catch (FlurlHttpException ex)
            {
                MambuErrorModel errorResponse = HandleMambuError(ex, out responseContent);
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, errorResponse, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                throw new MambuApiException(errorResponse, ex, new { mambuAccountId, url });
            }
            catch (Exception ex)
            {
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, ex.Message, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                throw new ApiException(ex, new { mambuAccountId, url });
            }
        }

        public async Task<DeliveryNotificationResponseModel> DeliveryNotification(DeliveryNotificationModel deliveryNotification, IApiRequest apiRequest)
        {
            string apiName = "delivery-notification";
            var apiRequestModel = apiRequest.GetRequest();

            string url = ApplicationSettingManager.WorkflowBaseUrl + "/stageProcess/notification";
            var requestContent = JsonConvert.SerializeObject(deliveryNotification,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            IFlurlRequest request = null;
            IDictionary<string, object> requestHeader = null;
            object requestTraceId = "";
            IEnumerable<string> responseTraceIds = null;
            string responseContent = "";
            HttpResponseHeaders responseHeaders = null;

            try
            {
                string token = await GetWorkflowToken(apiRequestModel, apiName);

                request = CreateRequestNonMambu(url, token);

                requestHeader = request.Headers;

                requestHeader.TryGetValue(TraceIdHeader, out requestTraceId);
                request.Headers.Add("Content-Type", "application/json; charset=utf8");

                var response = await request.PostStringAsync(requestContent);

                responseHeaders = response.Headers;
                responseHeaders.TryGetValues(ResponseRequestIdHeader, out responseTraceIds);

                var payload = await ReadFromJsonAsync<DeliveryNotificationResponseModel>(response);
                responseContent = payload.Content;

                if (payload.Success)
                {
                    dbLoggger.LogApiSuccess(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                }
                else
                {
                    var errorResponse = HandleWorkFlowError(responseContent);

                    dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, errorResponse, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                    throw new WorkflowApiException(errorResponse, new Exception("Response Error:" + responseContent), new { deliveryNotification, url });
                }

                return payload == null ? null : payload.Object;
            }
            catch (WorkflowApiException)
            {
                throw;
            }
            catch (FlurlHttpException ex)
            {
                var errorResponse = HandleWorkFlowError(ex, out responseContent);
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, errorResponse, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                throw new WorkflowApiException(errorResponse, ex, new { deliveryNotification, url });
            }
            catch (Exception ex)
            {
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, ex.Message, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                throw new ApiException(ex, new { deliveryNotification, url });
            }
        }

        public async Task<bool> PaymentRequest(PaymentRequestModel requestObject, string countryIsoCode, IApiRequest apiRequest)
        {
            string apiName = "paymentrequest";
            var apiRequestModel = apiRequest.GetRequest();
            string url = ApplicationSettingManager.EmmaBaseUrl + "/api/v1/unicomer/paymentrequest";

            var requestContent = JsonConvert.SerializeObject(requestObject,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });

            IFlurlRequest request = null;
            IDictionary<string, object> requestHeader = null;
            object requestTraceId = "";
            IEnumerable<string> responseTraceIds = null;
            string responseContent = "";
            HttpResponseHeaders responseHeaders = null;

            try
            {
                string token = await GetEmmaToken(apiRequestModel, apiName);

                request = CreateRequestNonMambu(url, token);

                requestHeader = request.Headers;

                requestHeader.TryGetValue(TraceIdHeader, out requestTraceId);

                var response = await request.PostStringAsync(requestContent);

                responseHeaders = response.Headers;
                responseHeaders.TryGetValues(ResponseRequestIdHeader, out responseTraceIds);

                var payload = await ReadFromJsonAsync<DeliveryNotificationResponseModel>(response);
                responseContent = payload.Content;

                if (payload.Success)
                {
                    dbLoggger.LogApiSuccess(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);
                }
                else
                {
                    var errorResponse = HandleEmmaError(responseContent);

                    dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, errorResponse, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);

                    throw new EmmaApiException(errorResponse, new Exception("Response Error:" + responseContent), new { requestObject, countryIsoCode, url });
                }

                return response.StatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch (EmmaApiException)
            {
                throw;
            }
            catch (FlurlHttpException ex)
            {
                var errorResponse = HandleEmmaError(ex, out responseContent);

                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, errorResponse, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);

                throw new EmmaApiException(errorResponse, ex, new { requestObject, countryIsoCode, url });
            }
            catch (Exception ex)
            {
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, requestContent, ex.Message, responseContent, requestTraceId, responseTraceIds == null ? "" : responseTraceIds.FirstOrDefault() ?? "", requestHeader, responseHeaders);

                throw new ApiException(ex, new { requestObject, countryIsoCode, url });
            }
        }

        private static MambuErrorModel HandleMambuError(FlurlHttpException ex, out string responseContent)
        {
            MambuErrorModel errorResponse = null;
            responseContent = "";

            try
            {
                responseContent = ex.GetResponseStringAsync().Result;
            }
            catch { }

            try
            {
                MambuErrorResponseModel mambuErrorResponseModel = null;

                try
                {
                    mambuErrorResponseModel = ex.GetResponseJsonAsync<MambuErrorResponseModel>().Result;
                }
                catch (Exception)
                { }

                if (mambuErrorResponseModel != null)
                {
                    errorResponse = new MambuErrorModel();

                    if (mambuErrorResponseModel.Result != null && mambuErrorResponseModel.Result.Details != null && mambuErrorResponseModel.Result.Details.Any())
                    {
                        errorResponse.Errors = mambuErrorResponseModel.Result.Details.Select(t =>
                        {
                            var errorCode = 0;

                            int.TryParse(t.InternalCode, out errorCode);

                            return new MambuError
                            {
                                ErrorReason = t.Message,
                                ErrorSource = t.Detail,
                                ErrorCode = errorCode
                            };
                        }).ToList();
                    }
                }
            }
            catch
            {

            }

            if (errorResponse == null || errorResponse.Errors == null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        errorResponse = new MambuErrorModel
                        {
                            Errors = new List<MambuError> {
                                new MambuError {
                                                    ErrorReason=responseContent,
                                                    ErrorSource=ex.Message
                                                }
                            }
                        };
                    }
                }
                catch
                {
                }
            }

            if (errorResponse == null)
            {
                errorResponse = new MambuErrorModel
                {
                    Errors = new List<MambuError> {
                        new MambuError {
                        ErrorReason = responseContent ,
                        ErrorSource = "Unhandled error occured in Emma service"+ex.Message
                        }
                    }
                };
            }


            return errorResponse;
        }

        private static MambuErrorModel HandleMambuError(string responseContent)
        {
            MambuErrorModel errorResponse = null;

            try
            {
                MambuErrorResponseModel mambuErrorResponseModel = null;
                try
                {
                    mambuErrorResponseModel = JsonConvert.DeserializeObject<MambuErrorResponseModel>(responseContent);
                }
                catch (Exception)
                { }

                if (mambuErrorResponseModel != null)
                {
                    errorResponse = new MambuErrorModel();

                    if (mambuErrorResponseModel.Result != null && mambuErrorResponseModel.Result.Details != null && mambuErrorResponseModel.Result.Details.Any())
                    {
                        errorResponse.Errors = mambuErrorResponseModel.Result.Details.Select(t =>
                        {
                            var errorCode = 0;

                            int.TryParse(t.InternalCode, out errorCode);

                            return new MambuError
                            {
                                ErrorReason = t.Message,
                                ErrorSource = t.Detail,
                                ErrorCode = errorCode
                            };
                        }).ToList();
                    }
                }
            }
            catch
            {

            }

            if (errorResponse == null || errorResponse.Errors == null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        errorResponse = new MambuErrorModel
                        {
                            Errors = new List<MambuError> {
                                new MambuError {
                                                    ErrorReason=responseContent,
                                                }
                            }
                        };
                    }
                }
                catch
                {
                }
            }

            if (errorResponse == null)
            {
                errorResponse = new MambuErrorModel
                {
                    Errors = new List<MambuError> {
                        new MambuError {
                            ErrorReason = responseContent ,
                            ErrorSource = "Unhandled error occured in Mambu service"
                        }
                    }
                };
            }

            return errorResponse;
        }

        private static EmmaErrorModel HandleEmmaError(FlurlHttpException ex, out string responseContent)
        {
            EmmaErrorModel errorResponse = null;
            responseContent = "";

            try
            {
                responseContent = ex.GetResponseStringAsync().Result;
            }
            catch { }

            try
            {
                errorResponse = ex.GetResponseJsonAsync<EmmaErrorModel>().Result;
            }
            catch
            {
            }

            if (errorResponse == null || errorResponse.Errors == null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        errorResponse = new EmmaErrorModel
                        {
                            Errors = new List<EmmaError> {
                                new EmmaError
                                {
                                    ErrorReason=responseContent,
                                    ErrorSource=ex.Message
                                }
                            }
                        };
                    }
                }
                catch
                {
                }
            }

            if (errorResponse == null)
            {
                errorResponse = new EmmaErrorModel
                {
                    Errors = new List<EmmaError> {
                        new EmmaError {
                            ErrorReason = responseContent,
                            ErrorSource="Unhandled error occured in Emma service: "+ex.Message
                        }
                    }
                };
            }

            return errorResponse;
        }

        private static EmmaErrorModel HandleEmmaError(string responseContent)
        {
            EmmaErrorModel errorResponse = null;

            try
            {
                errorResponse = JsonConvert.DeserializeObject<EmmaErrorModel>(responseContent);
            }
            catch (Exception)
            { }

            if (errorResponse == null || errorResponse.Errors == null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        errorResponse = new EmmaErrorModel
                        {
                            Errors = new List<EmmaError> {
                                new EmmaError {
                                                    ErrorReason=responseContent,
                                                }
                            }
                        };
                    }
                }
                catch
                {
                }
            }

            if (errorResponse == null)
            {
                errorResponse = new EmmaErrorModel
                {
                    Errors = new List<EmmaError> {
                        new EmmaError {
                            ErrorReason =  responseContent ,
                            ErrorSource = "Unhandled error occured in Emma service"
                        }
                    }
                };
            }

            return errorResponse;
        }

        private static WorkflowErrorResponseModel HandleWorkFlowError(FlurlHttpException ex, out string responseContent)
        {
            WorkflowErrorResponseModel errorResponse = null;
            responseContent = "";

            try
            {
                responseContent = ex.GetResponseStringAsync().Result;
            }
            catch { }

            try
            {
                errorResponse = ex.GetResponseJsonAsync<WorkflowErrorResponseModel>().Result;
            }
            catch
            {
            }

            if (errorResponse == null || errorResponse.GeneralResponse == null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var workflowError = JsonConvert.DeserializeObject<WorkflowError>(responseContent);
                        if (workflowError == null)
                        {
                            workflowError = new WorkflowError
                            {
                                Detail = responseContent,
                                Title = ex.Message
                            };
                        }

                        errorResponse = new WorkflowErrorResponseModel
                        {
                            GeneralResponse = new WorkflowErrorResponse
                            {
                                Errors = workflowError,
                                ResponseDescription = responseContent,
                            }
                        };
                    }
                }
                catch
                {
                }
            }

            if (errorResponse == null)
            {
                errorResponse = new WorkflowErrorResponseModel
                {
                    GeneralResponse = new WorkflowErrorResponse
                    {
                        ResponseDescription = "Unhandled error occured in Workflow service",
                        Errors = new WorkflowError
                        {
                            Detail = responseContent,
                            Title = ex.Message
                        }
                    }
                };
            }

            return errorResponse;
        }

        private static WorkflowErrorResponseModel HandleWorkFlowError(string responseContent)
        {
            WorkflowErrorResponseModel errorResponse = null;

            try
            {
                errorResponse = JsonConvert.DeserializeObject<WorkflowErrorResponseModel>(responseContent);
            }
            catch (Exception)
            { }

            if (errorResponse == null || errorResponse.GeneralResponse == null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var workflowError = JsonConvert.DeserializeObject<WorkflowError>(responseContent);
                        errorResponse = new WorkflowErrorResponseModel
                        {
                            GeneralResponse = new WorkflowErrorResponse
                            {
                                Errors = workflowError,
                                ResponseDescription = responseContent
                            }
                        };
                    }
                }
                catch
                {
                }
            }

            if (errorResponse == null)
            {
                errorResponse = new WorkflowErrorResponseModel
                {
                    GeneralResponse = new WorkflowErrorResponse
                    {
                        ResponseDescription = "Unhandled error occured in Workflow service",
                        Errors = new WorkflowError
                        {
                            Detail = responseContent
                        }
                    }
                };
            }

            return errorResponse;
        }

        private async Task<string> GetEmmaToken(ApiRequest apiRequestModel, string apiName)
        {
            if (!string.IsNullOrWhiteSpace(emmaToken))
            {
                return emmaToken;
            }

            string url = ApplicationSettingManager.EmmaTokenUrl;

            var kvp = new List<KeyValuePair<string, string>>();
            kvp.Add(new KeyValuePair<string, string>("grant_type", ApplicationSettingManager.EmmaGrantType));
            kvp.Add(new KeyValuePair<string, string>("client_id", ApplicationSettingManager.EmmaClientId));
            kvp.Add(new KeyValuePair<string, string>("client_secret", ApplicationSettingManager.EmmaClientSecret));
            kvp.Add(new KeyValuePair<string, string>("scope", ApplicationSettingManager.EmmaClientScope));

            try
            {
                dynamic result = await url.PostAsync(new FormUrlEncodedContent(kvp)).ReceiveJson();
                emmaToken = result.access_token;
                return emmaToken;
            }
            catch (FlurlHttpException ex)
            {
                var responseContent = "";
                var errorResponse = HandleEmmaError(ex, out responseContent);

                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, "", errorResponse, responseContent, "", "", "", "");


                throw new EmmaApiException(errorResponse, ex, new { kvp, url, responseContent });
            }
            catch (Exception ex)
            {
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, "", ex.Message, ex.StackTrace, "", "", "", "");

                throw new ApiException(ex, new { kvp, url });
            }
        }

        private async Task<string> GetMambuToken(ApiRequest apiRequestModel, string apiName)
        {
            if (!string.IsNullOrWhiteSpace(mambuToken))
            {
                return mambuToken;
            }

            string url = ApplicationSettingManager.MambuTokenUrl;

            var kvp = new List<KeyValuePair<string, string>>();
            kvp.Add(new KeyValuePair<string, string>("grant_type", ApplicationSettingManager.MambuGrantType));
            kvp.Add(new KeyValuePair<string, string>("client_id", ApplicationSettingManager.MambuClientId));
            kvp.Add(new KeyValuePair<string, string>("client_secret", ApplicationSettingManager.MambuClientSecret));

            if (!string.IsNullOrWhiteSpace(ApplicationSettingManager.MambuClientScope))
            {
                kvp.Add(new KeyValuePair<string, string>("scope", ApplicationSettingManager.MambuClientScope));
            }

            try
            {
                dynamic result = await url.PostAsync(new FormUrlEncodedContent(kvp)).ReceiveJson();
                mambuToken = result.access_token;
                return mambuToken;
            }
            catch (FlurlHttpException ex)
            {
                var responseContent = "";
                var errorResponse = HandleMambuError(ex, out responseContent);
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, "", errorResponse, responseContent, "", "", "", "");

                throw new MambuApiException(errorResponse, ex, new { kvp, url, responseContent });
            }
            catch (Exception ex)
            {
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, "", ex.Message, ex.StackTrace, "", "", "", "");

                throw new ApiException(ex, new { kvp, url });
            }
        }

        private async Task<string> GetWorkflowToken(ApiRequest apiRequestModel, string apiName)
        {
            if (!string.IsNullOrWhiteSpace(workflowToken))
            {
                return workflowToken;
            }

            string url = ApplicationSettingManager.WorkflowTokenUrl;
            var kvp = new List<KeyValuePair<string, string>>();
            kvp.Add(new KeyValuePair<string, string>("grant_type", ApplicationSettingManager.WorkflowGrantType));
            kvp.Add(new KeyValuePair<string, string>("client_id", ApplicationSettingManager.WorkflowClientId));
            kvp.Add(new KeyValuePair<string, string>("client_secret", ApplicationSettingManager.WorkflowClientSecret));
            kvp.Add(new KeyValuePair<string, string>("scope", ApplicationSettingManager.WorkflowClientScope));

            try
            {
                dynamic result = await url.PostAsync(new FormUrlEncodedContent(kvp)).ReceiveJson();
                workflowToken = result.access_token;
                return workflowToken;
            }
            catch (FlurlHttpException ex)
            {
                var responseContent = "";
                var errorResponse = HandleWorkFlowError(ex, out responseContent);
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, "", errorResponse, responseContent, "", "", "", "");

                throw new WorkflowApiException(errorResponse, ex, new { kvp, url, responseContent });
            }
            catch (Exception ex)
            {
                dbLoggger.LogApiFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, apiName, url, "", ex.Message, ex.StackTrace, "", "", "", "");

                throw new ApiException(ex, new { kvp, url });
            }
        }

        private string GetDateString()
        {
            try
            {
                var easternTime = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

                return easternTime.ToString("yyyy-MM-ddThh:mm:sszzz");
            }
            catch
            {
                return DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz", CultureInfo.InvariantCulture);
            }
        }

        private string GetTraceId()
        {
            return Guid.NewGuid().ToString("N").ToLowerInvariant();
        }

        private string GetSpanId()
        {
            string generateHexString = "";
            var num = DateTime.Now.Ticks;
            var hexString = num.ToString("X", CultureInfo.InvariantCulture).ToLowerInvariant();

            if (hexString.Length > 16)
            {
                generateHexString = hexString.Substring(0, 16);
            }
            else if (hexString.Length == 16)
            {
                generateHexString = hexString;
            }
            else
            {
                const string charList = "0123456789abcdef";
                generateHexString = hexString;
                Random rand = new Random();

                int missingCharecterslength = 16 - hexString.Length;

                for (int i = 0; i < missingCharecterslength; i++)
                {
                    int randIndex = rand.Next(0, charList.Length);
                    generateHexString += charList[randIndex];
                }
            }

            return generateHexString;
        }

        private string GetIdempotencyKey()
        {
            return Guid.NewGuid().ToString().ToLowerInvariant();
        }

        private IFlurlRequest CreateRequestNonMambu(string url, string token)
        {
            var request = url.WithHeader("X-country", ApplicationSettingManager.XCountry)
                        .WithHeader("X-brand", ApplicationSettingManager.EXbrand)
                        .WithHeader("X-storeRef", ApplicationSettingManager.EXstoreRef)
                        .WithHeader("X-processRef", ApplicationSettingManager.EXprocessRef)
                        .WithHeader("X-channelRef", ApplicationSettingManager.XchannelRef)
                        .WithHeader("X-typeProduct", ApplicationSettingManager.XtypeProduct)
                        .WithHeader("X-consumerDateTime", GetDateString())
                        .WithHeader("X-environment", ApplicationSettingManager.Xenvironment)
                        .WithHeader(TraceIdHeader, GetTraceId())
                        .WithHeader("X-idempotency-Key", GetIdempotencyKey())
                        .WithHeader("Authorization", "Bearer " + token);
            return request;
        }

        private IFlurlRequest CreateRequest(string url, string token, string processRef, string typeProduct, bool isTransactional)
        {
            if (string.IsNullOrWhiteSpace(processRef))
            {
                processRef = ApplicationSettingManager.Schedule;
            }

            var request = url.WithHeader("X-country", ApplicationSettingManager.XCountry)
                    .WithHeader("X-brand", ApplicationSettingManager.Xbrand)
                    .WithHeader("X-storeRef", ApplicationSettingManager.XstoreRef)
                    .WithHeader("X-processRef", processRef)
                    .WithHeader("X-channelRef", ApplicationSettingManager.XchannelRef)
                    .WithHeader("X-consumerRef", ApplicationSettingManager.XconsumerRef)
                    .WithHeader("X-typeProcessRef", ApplicationSettingManager.XtypeProcessRef)
                    .WithHeader("X-typeProduct", typeProduct)
                    .WithHeader("X-consumerDateTime", GetDateString())
                    .WithHeader("X-environment", ApplicationSettingManager.Xenvironment)
                    .WithHeader(TraceIdHeader, GetTraceId())
                    .WithHeader("X-B3-SpanId", GetSpanId())
                    .WithHeader("Authorization", "Bearer " + token);

            if (!string.IsNullOrWhiteSpace(ApplicationSettingManager.XuserTx))
            {
                request.WithHeader("X-userTx", ApplicationSettingManager.XuserTx);
            }

            if (isTransactional)
            {
                request.WithHeader("X-idempotency-Key", GetIdempotencyKey());
            }

            return request;
        }

        public static async Task<ResponseObject<TBody>> ReadFromJsonAsync<TBody>(HttpResponseMessage response) where TBody : class
        {
            ResponseObject<TBody> responseObject = new ResponseObject<TBody>();

            if (response.Content == null) return responseObject;

            responseObject.Content = await response.Content.ReadAsStringAsync();
            responseObject.Success = response.IsSuccessStatusCode;

            if (response.IsSuccessStatusCode)
            {
                responseObject.Object = JsonConvert.DeserializeObject<TBody>(responseObject.Content);
            }

            return responseObject;
        }
    }
}
