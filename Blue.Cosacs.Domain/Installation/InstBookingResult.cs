using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public class InstBookingResult
    {
        public int InstNo { get; set; }
        public DateTime InstDate { get; set; }
        public int StartSlot { get; set; }
        public int NoOfSlots { get; set; }
        public string FormattedSlots { get; set; }
        public int TechnicianId { get; set; }
        public string TechnicianName { get; set; }
        public int BookedBy { get; set; }
        public DateTime BookedOn { get; set; }
        public int? ReleasedBy { get; set; }
        public DateTime? ReleasedOn { get; set; }  
        public string RebookingReasonText { get; set; }
        public string RebookingReasonCode { get; set; }
        public bool IsActive { get; set; }   
    }
}
