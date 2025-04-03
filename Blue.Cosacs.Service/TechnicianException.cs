using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Service
{
    [Serializable]
    public class TechnicianException : Exception
    {
        public TechnicianException() { }
        public TechnicianException(string message) : base(message) { }
        public TechnicianException(string message, Exception inner) : base(message, inner) { }
        protected TechnicianException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
