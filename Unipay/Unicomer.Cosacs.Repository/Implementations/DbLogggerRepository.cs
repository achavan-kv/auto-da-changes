/* 
Author: suresh-IGT
Description:JM BlueStart
 */
using STL.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Repository.Implementations
{
    internal class DbLogggerRepository : IDbLogggerRepository
    {
        private readonly IJsonSerializer jsonSerializer;

        public DbLogggerRepository(IJsonSerializer jsonSerializer)
        {
            this.jsonSerializer = jsonSerializer;
        }

        public void LogRequest(IApiRequest apiRequest, HttpRequestMessage httpRequestMessage)
        {
            try
            {
                var apiRequestModel = apiRequest.GetRequest();

                IEnumerable<string> requestTraceIds = null;
                httpRequestMessage.Headers.TryGetValues("X-B3-TraceId", out requestTraceIds);
                var traceId = requestTraceIds == null ? "" : requestTraceIds.FirstOrDefault() ?? "";

                if (string.IsNullOrEmpty(traceId))
                {
                    httpRequestMessage.Headers.TryGetValues("x-request-id", out requestTraceIds);
                    traceId = requestTraceIds == null ? "" : requestTraceIds.FirstOrDefault() ?? "";
                }

                LogApiSuccess(apiRequestModel.RequestId, "Request_" + apiRequestModel.RequestName, "", "", apiRequest, "", traceId, "", httpRequestMessage.Headers, "");
            }
            catch { }
        }

        public void LogApiFailure(string uniqueId, string cosacsAPIName, string mambuAPIName, string apiURL,
            object request, object errorDetail, object response, object requestTraceId, string responseTraceId,
            object requestHeaders, object responseHeaders)
        {
            var requestTraceIdValue = "";
            var requestJSON = "";
            var responseJSON = "";
            var requestHeadersJson = "";
            var responseHeadersJSON = "";
            var errorDetailJSON = "";

            try
            {
                errorDetailJSON = this.jsonSerializer.Serialize(errorDetail);
                requestJSON = this.jsonSerializer.Serialize(request);
                responseJSON = this.jsonSerializer.Serialize(response);

                requestHeadersJson = this.jsonSerializer.Serialize(requestHeaders);
                responseHeadersJSON = this.jsonSerializer.Serialize(responseHeaders);

                requestTraceIdValue = Convert.ToString(requestTraceId);
            }
            catch (Exception ex)
            {
                errorDetailJSON = errorDetailJSON + "Serialization Error-" + ex.Message;
            }

            Log(uniqueId, cosacsAPIName, mambuAPIName, apiURL, requestJSON, responseJSON, false, false, errorDetailJSON,
                requestTraceIdValue, responseTraceId, requestHeadersJson, responseHeadersJSON);
        }


        public void LogApiRequestResponse(IApiRequest apiRequest, object response, Exception exception)
        {
            if (ApplicationSettingManager.LogApiRequest)
            {
                try
                {
                    bool isSuccess = exception == null;
                    bool isDbError = false;
                    var apiRequestModel = apiRequest.GetRequest();
                    var requestJSON = this.jsonSerializer.Serialize(apiRequest);
                    var responseJSON = this.jsonSerializer.Serialize(response);
                    object errorDetailObject = null;
                    var errorDetail = "";

                    if (!isSuccess)
                    {
                        if (exception is ArgumentNullException)
                        {
                            var argumentNullException = exception as ArgumentNullException;
                            errorDetailObject = new { argumentNullException.ParamName, argumentNullException.Message };
                        }
                        else if (exception is ValidationException)
                        {
                            isDbError = true;
                            var validationException = exception as ValidationException;
                            errorDetailObject = new { validationException.ErrorMessages, validationException.Message };
                        }
                        else if (exception is EmmaApiException)
                        {
                            var emmaApiException = exception as EmmaApiException;
                            errorDetailObject = new { emmaApiException.RequestModel, emmaApiException.ErrorResponse, emmaApiException.Message };
                        }
                        else if (exception is MambuApiException)
                        {
                            var mambuApiException = exception as MambuApiException;
                            errorDetailObject = new { mambuApiException.RequestModel, mambuApiException.ErrorResponse, mambuApiException.Message };
                        }
                        else if (exception is WorkflowApiException)
                        {
                            var workflowApiException = exception as WorkflowApiException;
                            errorDetailObject = new { workflowApiException.RequestModel, workflowApiException.ErrorResponse, workflowApiException.Message };
                        }
                        else if (exception is DBException)
                        {
                            isDbError = true;
                            var dBException = exception as DBException;
                            errorDetailObject = new { dBException.RequestModel, dBException.Message };
                        }
                        else
                        {
                            errorDetailObject = new { exception.StackTrace, exception.Message, exception.Source };
                        }

                        if (errorDetailObject != null)
                        {
                            try
                            {
                                errorDetail = this.jsonSerializer.Serialize(errorDetailObject);
                            }
                            catch (Exception ex)
                            {
                                errorDetail = ex.Message + " : Exact Error - " + exception.Message;
                            }
                        }
                    }

                    Log(apiRequestModel.RequestId, apiRequestModel.RequestName, apiRequestModel.RequestName + "-request", "", requestJSON, responseJSON, isSuccess, isDbError, errorDetail, "", "", "", "");
                }
                catch { }
            }
        }

        public void LogApiSuccess(string uniqueId, string cosacsAPIName, string mambuAPIName, string apiURL, object request, object response,
            object requestTraceId, string responseTraceId, object requestHeaders, object responseHeaders)
        {
            if (ApplicationSettingManager.LogApiError)
            {
                var requestTraceIdValue = "";
                var requestJSON = "";
                var responseJSON = "";
                var requestHeadersJson = "";
                var responseHeadersJSON = "";
                var errordetails = "";

                try
                {
                    requestJSON = this.jsonSerializer.Serialize(request);
                    responseJSON = this.jsonSerializer.Serialize(response);

                    requestHeadersJson = this.jsonSerializer.Serialize(requestHeaders);
                    responseHeadersJSON = this.jsonSerializer.Serialize(responseHeaders);
                    requestTraceIdValue = Convert.ToString(requestTraceId);

                }
                catch (Exception ex)
                {
                    errordetails = "Serialization Error-" + ex.Message;
                }

                Log(uniqueId, cosacsAPIName, mambuAPIName, apiURL, requestJSON, responseJSON, true, false, errordetails, requestTraceIdValue, responseTraceId, requestHeadersJson, responseHeadersJSON);

            }
        }

        public void LogDbFailure(string uniqueId, string cosacsAPIName, string spName, object request, object errorDetail)
        {
            if (ApplicationSettingManager.LogDbError)
            {
                try
                {
                    var requestJSON = this.jsonSerializer.Serialize(request);
                    var errorDetailJSON = this.jsonSerializer.Serialize(new { Spname = spName, errorDetail });

                    Log(uniqueId, cosacsAPIName, "", "", requestJSON, "", false, true, errorDetailJSON, "", "", "", "");
                }
                catch { }
            }
        }
        public void LogDbValidation(string uniqueId, string cosacsAPIName, string spName, object request, object errorDetail)
        {
            if (ApplicationSettingManager.LogDbValidation)
            {
                try
                {
                    var requestJSON = this.jsonSerializer.Serialize(request);
                    var errorDetailJSON = this.jsonSerializer.Serialize(new { Spname = spName, errorDetail });

                    Log(uniqueId, cosacsAPIName, "", "", requestJSON, "", false, true, errorDetailJSON, "", "", "", "");
                }
                catch { }
            }
        }

        private void Log(string uniqueId, string cosacsAPIName, string mambuAPIName,
            string apiURL, string requestJSON, string responseJSON,
            bool isSuccess, bool isDBError, string errorDetail,
            string requestTraceId, string responseTraceId, string requestHeaders, string responseHeaders)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        try
                        {
                            SqlParameter[] xparams = {
                                        new SqlParameter("@UniqueId", uniqueId),
                                        new SqlParameter("@CoSaCSAPIName", cosacsAPIName),
                                        new SqlParameter("@MambuAPIName", mambuAPIName),
                                        new SqlParameter("@APIURL", apiURL),
                                        new SqlParameter("@RequestJSON", requestJSON ?? ""),
                                        new SqlParameter("@ResponseJSON", responseJSON ?? ""),
                                        new SqlParameter("@IsSuccess", isSuccess),
                                        new SqlParameter("@IsDBError", isDBError),
                                        new SqlParameter("@ErrorDetail", errorDetail ?? ""),
                                        new SqlParameter("@requestTraceId", requestTraceId ?? ""),
                                        new SqlParameter("@responseTraceId", responseTraceId ?? ""),
                                        new SqlParameter("@requestHeaders", requestHeaders??""),
                                        new SqlParameter("@responseHeaders", responseHeaders??""),
                                   };

                            SqlCommand command = conn.CreateCommand();
                            command.Transaction = trans;
                            command.CommandText = "dbo.SP_BS_InsertBSAPILog";
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(xparams);
                            command.ExecuteNonQuery();
                            trans.Commit();
                        }
                        catch (SqlException ex)
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}
