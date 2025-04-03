namespace Blue.Cosacs.Service.Models
{
    public class TechnicianProfile
    {
        public int UserId { get; set; }
        public bool Internal { get; set; }
        public string[] Zones { get; set; }
        public string startHour { get; set; }
        public string endHour { get; set; }
        public string startMinute { get; set; }
        public string endMinute { get; set; }
        public int slots { get; set; }
        // Code added by Gurpreet - CR2018-010 - 31/10/18 - Setting of max no. of Jobs & Validation with allocated jobs for a technician.
        public int maxJobs { get; set; }

        public int currJobs { get; set; }
        //CR2018-010 Changes End

    }
}
