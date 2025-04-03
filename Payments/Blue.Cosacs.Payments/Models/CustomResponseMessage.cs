using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Payments.Models
{
    public class CustomResponseMessage
    {
        public CustomResponseMessage()
        {
            Valid = true;
        }
        public bool Valid { get; set; }
        public string CustomError { get; set; }
        public string[] Errors { get; set; }
    }
}
