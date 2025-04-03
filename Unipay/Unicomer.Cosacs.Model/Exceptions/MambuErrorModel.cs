using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model.Models;

namespace Unicomer.Cosacs.Model.Exceptions
{
    public class MambuError
    {
        public int ErrorCode { get; set; }
        public string ErrorReason { get; set; }
        public string ErrorSource { get; set; }
    }

    public class MambuErrorModel
    {
        public List<MambuError> Errors { get; set; }
    }

    public class MambuErrorResponseModel
    {
        public ApiResult Result { get; set; }
    }
}
