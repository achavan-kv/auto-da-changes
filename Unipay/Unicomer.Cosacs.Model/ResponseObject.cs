using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model
{
    public class ResponseObject<T> where T : class
    {
        public string Content { get;set; }
        public T Object { get; set; }
        public bool Success { get; set; }
    }
}
