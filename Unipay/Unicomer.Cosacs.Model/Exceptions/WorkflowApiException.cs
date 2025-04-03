/* 
Author: Suresh -IGT
Date: Feb 1st
Description:JM BlueStart-Pre-Qualification API Validation Exception
 */
using System;

namespace Unicomer.Cosacs.Model.Exceptions
{
    public class WorkflowApiException : Exception
    {
        public WorkflowApiException(WorkflowErrorResponseModel errorResponse, Exception exception, params object[] requests)
            : base(exception.Message, exception)
        {
            ErrorResponse = errorResponse;
            RequestModel = requests;
        }

        public WorkflowErrorResponseModel ErrorResponse { get; set; }
        public object[] RequestModel { get; set; }
    }
}
