using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.BLL.WebApi
{
    public class PostPaymentErrorModel
    {
        public List<General> generalResponse { get; set; }
    }

    public class General
    { 
        public List<Error> errors { get; set; }
        public string responseDescription { get; set; }
    }

    public class Error
    { 
        public string detail { get; set; }

    }
}
