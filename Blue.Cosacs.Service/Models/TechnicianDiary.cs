using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Service.Models
{
    public class HolidayList
    {
        public int Id { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public bool Approved { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
    }

    public class TechnicianDiary
    {
        public IEnumerable<HolidayList> Holidays { get; set; }
        public IEnumerable<HolidayList> PendingHolidays { get; set; }
        public IEnumerable<TechnicianDiary.PublicHoliday> PublicHolidays { get; set; }
        public IEnumerable<TechnicianDiary.TechnicianBooking> Bookings { get; set; }
        public IEnumerable<TechnicianDiary.TechnicianBooking> FreeBookings { get; set; }
        public IEnumerable<Technician> Technician { get; set; }
        public bool Found { get; set; }



        public class PublicHoliday
        {
            public DateTime Date;
        }

        public class TechnicianBooking
        {
            public int Id { get; set; }
            public int RequestId { get; set; }
            public DateTime Date { get; set; }
            public int Slot { get; set; }
            public int SlotExtend { get; set; }
            public string Type { get; set; }
            public bool Reject { get; set; }
            public DateTime CreatedOn { get; set; }
            public string InvoiceNumber { get; set; }
            public string CustomerTitle { get; set; }
            public string CustomerFirstName { get; set; }
            public string CustomerLastName { get; set; }
            public string CustomerAddressLine1 { get; set; }
            public string CustomerAddressLine2 { get; set; }
            public string CustomerAddressLine3 { get; set; }
            public string CustomerPostcode { get; set; }
            public string CustomerNotes { get; set; }
            public string Item { get; set; }
            public string ItemSupplier { get; set; }
            public decimal? EstimateLabourCost { get; set; }
            public decimal? EstimateAdditionalLabourCost { get; set; }

            //Below Code Added By Gurpreet - CR2018-010 - 31/10/18 - Setting of max no. of Jobs & Validation with allocated jobs for a technician.
            public int MaxJobs { get; set; }
            //CR2018-010 Changes End
        }


    }

    public class DiaryExceptions
    {
        public IEnumerable<HolidayList> PendingHolidays { get; set; }
        public IEnumerable<Rejections> RejectBookings { get; set; }

        public class Rejections
        {
            public int Id { get; set; }
            public int RequestId { get; set; }
            public DateTime DateAllocated { get; set; }
            public int UserId { get; set; }
            public DateTime Date { get; set; }
            public string FullName { get; set; }
            private string _Type;
            public string Type
            {
                get { return _Type; }
                set { _Type = value; }
            }
            public DateTime CreatedOn { get; set; }
            public DateTime? LastUpdatedOn { get; set; }
        }

    }
}
