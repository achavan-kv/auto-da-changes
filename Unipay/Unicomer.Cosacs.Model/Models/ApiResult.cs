using System.Collections.Generic;

namespace Unicomer.Cosacs.Model.Models
{
    public class ApiResultResponse
    {
        public ApiResult Result { get; set; }
    }
    public class ApiResult
    {
        public List<ApiResultDetail> Details { get; set; }
        public string Source { get; set; }
    }

    public class ApiResultDetail
    {
        public string InternalCode { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
    }
    
}
