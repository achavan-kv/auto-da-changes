/* 
Author: Suresh -IGT
Date: Feb 1st
Description:JM BlueStart-Pre-Qualification API Validation Exception
 */
using System;

namespace Unicomer.Cosacs.Model.Exceptions
{
    public class MambuApiException : Exception
    {
        public MambuApiException(MambuErrorModel errorResponse, Exception exception, params object[] requests)
            : base(exception.Message, exception)
        {
            ErrorResponse = errorResponse;
            RequestModel = requests;
        }

        public MambuErrorModel ErrorResponse { get; set; }
        public object[] RequestModel { get; set; }
    }
}
