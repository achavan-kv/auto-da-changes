using System;
using System.Net;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.Business
{
    public class JResponseError : IErrorResponse
    {
        public JResponse CreateErrorResponse(dynamic ErrorList)
        {
            JResponse objResponse = new JResponse();
            objResponse.Result = Convert.ToString(HttpStatusCode.BadRequest);
            objResponse.Status = false;
            objResponse.StatusCode = (int)HttpStatusCode.BadRequest;
            objResponse.Message = ErrorList;
            return objResponse;
        }
        public JResponse CreateExceptionResponse(dynamic ErrorList)
        {
            JResponse objResponse = new JResponse();
            objResponse.Result = Convert.ToString(HttpStatusCode.InternalServerError);
            objResponse.Status = false;
            objResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
            objResponse.Message = ErrorList;
            return objResponse;
        }
        public dynamic CreateWebExceptionResponse(dynamic ErrorList, int statusCode)
        {
            JResponse objResponse = new JResponse();
            objResponse.Result = Convert.ToString(HttpStatusCode.InternalServerError);
            objResponse.Status = false;
            objResponse.StatusCode = statusCode > 0 ? statusCode : (int)HttpStatusCode.InternalServerError;
            objResponse.Message = ErrorList;
            return objResponse;
        }
    }
}
