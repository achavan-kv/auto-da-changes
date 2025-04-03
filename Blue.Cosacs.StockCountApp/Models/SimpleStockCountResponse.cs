using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.StockCountApp.Models
{
    public class SimpleResponse<T> where T : class
    {
        public SimpleResponse() { }

        public string Status { get; set; }
        public T Data { get; set; }
        public string Code { get; set; }
    }
}
