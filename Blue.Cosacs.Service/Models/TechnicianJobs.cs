/* *
 * Description - Setting of max no. of Jobs & Validation with allocated jobs for a technician.
 * CR Number - CR2018-010
 * Date - 31/10/18
 * Created by - Gurpreet.R.Gill - Zensar
 */


namespace Blue.Cosacs.Service.Models
{
    public class TechnicianJobs
    {
        public int technicianId { get; set; }
        public int maxJobs { get; set; }
        public int currJobs { get; set; }

    }
}
