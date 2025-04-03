/* 
Author: Suresh -IGT
Date: Feb 1st
Description:JM BlueStart-Pre-Qualification API Validation Exception
 */
using System;

namespace Unicomer.Cosacs.Model.Exceptions
{
    public class EmmaApiException : Exception
    {
        public EmmaApiException(EmmaErrorModel errorResponse, Exception exception, params object[] requests)
            : base(exception.Message, exception)
        {
            ErrorResponse = errorResponse;
            RequestModel = requests;
        }

        public EmmaErrorModel ErrorResponse { get; set; }
        public object[] RequestModel { get; set; }
    }
}
