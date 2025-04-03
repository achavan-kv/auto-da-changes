using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Net
{
    public class ResponseStatus
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }

    public class ServerException : ApplicationException
    {
        public ServerException(ResponseStatus status)
            : base(string.Format("Exception: {0}\nMessage: {1}\nStack Trace: {2}\n", status.ErrorCode, status.Message, status.StackTrace))
        {
            this.ServerExceptionName = status.ErrorCode;
            this.ServerStackTrace = status.StackTrace;
        }

        public string ServerExceptionName { get; private set; }
        public string ServerStackTrace { get; private set; }
    }

    public class ResponseStatusWrapper
    {
        public ResponseStatus ResponseStatus { get; set; }
    }
}
