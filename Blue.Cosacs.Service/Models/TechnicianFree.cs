using System;
using System.Collections.Generic;

namespace Blue.Cosacs.Service.Models
{
    public class TechnicianFree
    {
        public IEnumerable<TechnicianFree.Tech> Technicians { get; set; }
        public IEnumerable<TechnicianFree.Hol> Holidays { get; set; }
        public IEnumerable<TechnicianFree.TechnicianBook> Bookings { get; set; }
        public IEnumerable<TechnicianFree.PHolidays> PublicHolidays { get; set; }
        // Added by Gurpreet - CR2018-010 - 31/10/18 - Setting of max no. of Jobs & Validation with allocated jobs for a technician.
        public IEnumerable<TechnicianFree.TechJobs> TechnicianJobs { get; set; }
        //CR2018-010 Changes End
        public class PHolidays
        {
            public DateTime Date { get; set; }
        }

        public class Hol
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public DateTime EndDate { get; set; }
            public DateTime StartDate { get; set; }
        }

        public class TechnicianBook
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int RequestId { get; set; }
            public DateTime Date { get; set; }
            public int Slot { get; set; }
            public int SlotExtend { get; set; }
            /// <summary>
            /// Service Request Type
            /// </summary>
            public string Type { get; set; }
            public bool Reject { get; set; }
        }

        public partial class Tech
        {
            public int UserId { get; set; }
            public string Name { get; set; }
            public bool Internal { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public int Slots { get; set; }
            public string[] Categories { get; set; }
        }

        // Gurpreet - CR2018-010 - 31/10/18 - Setting of max no. of Jobs & Validation with allocated jobs for a technician.
        /// <summary>
        /// Technician Jobs details
        /// </summary>

        public partial class TechJobs
        {
            public int UserId { get; set; }
            public string ServiceType { get; set; }
            public int AccountNumber { get; set; }
            public int RequestID { get; set; }
            public DateTime LoggedOn { get; set; }
            public DateTime DeliveredOn { get; set; }
            public string WarrantyType { get; set; }
            public string ItemCodeDesc { get; set; }
            public DateTime ServiceScheduledDate { get; set; }

        }
        //CR2018-010 Changes End

    }
}
