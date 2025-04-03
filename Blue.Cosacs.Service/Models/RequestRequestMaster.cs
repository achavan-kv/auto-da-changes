using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Service.Models
{
    public class CurrentUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public short Branch { get; set; }   
        public string Country { get; set; }
        public IEnumerable<int> Permissions { get; set; }
    }
}
