using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Service.Models
{
    public class FreeTechnicianSearch
    {
        public int? technicianId { get; set; }
        public DateTime? bookingDate { get; set; }
        public string category { get; set; }
        public int requestId { get; set; }
    }
}
