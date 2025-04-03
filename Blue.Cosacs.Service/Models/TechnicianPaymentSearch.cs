using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blue.Cosacs.Web.Areas.Service.Models
{
    public class TechnicianPaymentSearch
    {
        public int TechnicianId {get;set;}
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string TypeFilter { get; set; }
        public string ServiceRequest { get; set; }
    }

    public class TechnicianPaymentPrint
    {
        public int? TechnicianId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string TypeFilter { get; set; }
        public string ServiceRequest { get; set; }
        public string Technician { get; set; }
    }
}