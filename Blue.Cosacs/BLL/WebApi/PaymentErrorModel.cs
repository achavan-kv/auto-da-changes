using System.Collections.Generic;

namespace Blue.Cosacs.BLL.WebApi
{
    public class PaymentErrorModel
    {
        public List<GeneralErrors> GeneralResponse { get; set; }
    }

    public class GeneralErrors
    {
        public List<Errors> Errors { get; set; }
    }

    public class Errors
    {
        public string ErrorReason { get; set; }
        public int ErrorCode { get; set; }
        public string Detail { get; set; }
        public string Code { get; set; }
    }
}
