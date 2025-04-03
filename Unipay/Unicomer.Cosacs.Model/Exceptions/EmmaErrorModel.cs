using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model.Exceptions
{
    public class EmmaError
    {
        public int ErrorCode { get; set; }
        public string ErrorReason { get; set; }
        public string ErrorSource { get; set; }
    }

    public class EmmaErrorModel
    {
        public List<EmmaError> Errors { get; set; }
    }
}
