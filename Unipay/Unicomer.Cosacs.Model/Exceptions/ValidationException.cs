/* 
Author: Suresh -IGT
Date: Feb 1st
Description:JM BlueStart-Pre-Qualification API Validation Exception
 */
using System;

namespace Unicomer.Cosacs.Model.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(ErrorMessage errorMessage, params object[] requests)
           : base("One or more validation failures have occurred.")
        {
            ErrorMessages = new[] { errorMessage };
            RequestModel = requests;
        }

        public ValidationException(ErrorMessage[] errorMessages, params object[] requests)
            : base("One or more validation failures have occurred.")
        {
            ErrorMessages = errorMessages; 
            RequestModel = requests;
        }

        public ErrorMessage[] ErrorMessages { get; set; }
        public object[] RequestModel { get; set; }
    }
}
