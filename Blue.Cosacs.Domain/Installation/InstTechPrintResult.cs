using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public class InstTechPrintResult
    {
        public int InstNo { get; set; }
        public DateTime InstDate { get; set; }
        public int TechnicianId { get; set; }
        public string TechnicianName { get; set; }
        public int StartSlot { get; set; }
        public int NumberOfSlots { get; set; }
        public int BookedBy { get; set; }
        public DateTime BookedOn { get; set; }
        public string AcctNo { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string ItemNo { get; set; }
        public string CourtsCode { get; set; }
        public string InstItemNo { get; set; }
        public string InstCourtsCode { get; set; }
    }
}
