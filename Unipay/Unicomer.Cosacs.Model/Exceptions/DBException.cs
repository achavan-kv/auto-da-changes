/* 
Author: Suresh -IGT
Date: Feb 1st
Description:JM BlueStart-Pre-Qualification API Validation Exception
 */
using System;

namespace Unicomer.Cosacs.Model.Exceptions
{
    public class DBException : Exception
    {
        public DBException(Exception exception, params object[] requests)
            : base(exception.Message, exception)
        {
            RequestModel = requests;
        }

        public object[] RequestModel { get; set; }
    }
}
