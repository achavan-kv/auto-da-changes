using System.Collections.Generic;

namespace Unicomer.Cosacs.Model.Exceptions
{
    public class ErrorMessage
    {
        public string Detail { get; set; }
        public string Id { get; set; }
    }

    public class Error
    {
        public Error()
        {
            Title = "Invalid transaction";
        }
        public string Code { get; set; }
        public string Detail { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
    }

    public class GeneralResponse
    {
        public GeneralResponse(string error)
        {
            ResponseCode = "1";
            ResponseDescription = string.Format("An {0} error has occurred in the requested operation", error);
        }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public List<Error> Errors { get; set; }
    }

    public class ErrorResponse
    {
        public List<GeneralResponse> GeneralResponse { get; set; }
    }
}
