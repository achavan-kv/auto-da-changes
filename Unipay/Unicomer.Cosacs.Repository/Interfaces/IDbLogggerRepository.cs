/* 
Author: suresh-IGT
Date: Feb 15th
Description:JM BlueStart
 */

using System;
using System.Net.Http;
using Unicomer.Cosacs.Model.Interfaces;

namespace Unicomer.Cosacs.Repository.Interfaces
{
    public interface IDbLogggerRepository
    {
        void LogRequest(IApiRequest apiRequest, HttpRequestMessage httpRequestMessage);
        void LogApiRequestResponse(IApiRequest apiRequest, object response, Exception exception);
        void LogApiFailure(string uniqueId, string cosacsAPIName, string mambuAPIName, string apiURL,
                            object request, object errorDetail, object response, object requestTraceId, string responseTraceId,
                            object requestHeaders, object responseHeaders);
        void LogApiSuccess(string uniqueId, string cosacsAPIName, string mambuAPIName, string apiURL, object request, object response,
                            object requestTraceId, string responseTraceId, object requestHeaders, object responseHeaders);
        void LogDbFailure(string uniqueId, string cosacsAPIName, string spName, object request, object errorDetail);
        void LogDbValidation(string uniqueId, string cosacsAPIName, string spName, object request, object errorDetail);
    }
}
