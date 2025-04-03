using System;

namespace Blue.Cosacs.Service.Models
{
    public class TechnicianJobAllocation
    {

        public string ServiceType { get; set; }

        public string AccountNumber { get; set; }

        public int RequestId { get; set; }

        public DateTime DateLogged { get; set; }

        public DateTime DeliveredOn { get; set; }

        public DateTime SlotDate { get; set; }

        public string WarrantyType { get; set; }

        public string ItemCodeDescription { get; set; }

        public string CurrentJobState { get; set; }

        public string strDateLogged
        {
            get { return DateLogged.ToString("yyyy-MM-dd"); }
            set { }
        }

        public string strDeliveredOn
        {
            get { return DeliveredOn.ToString("yyyy-MM-dd"); }
            set { }
        }

        public string strSlotDate
        {
            get { return SlotDate.ToString("yyyy-MM-dd"); }
            set { }
        }
    }
}
