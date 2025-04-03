using System.ComponentModel.DataAnnotations;

namespace Blue.Cosacs.Web.Areas.Admin.Models
{
    /// <summary>
    /// AddressMaster
    /// </summary>
    public class AddressMaster
    {
        public AddressMaster()
        {
        }
        public AddressMaster(Blue.Cosacs.Service.AddressMaster addressMaster)
        {
            this.Id = addressMaster.Id;
            this.Region = addressMaster.Region;
            this.Village = addressMaster.Village;
            this.ZipCode = addressMaster.ZipCode;
        }

        public int Id
        {
            get;
            set;
        }

        [Required(ErrorMessage = "Region or Parish is required")]
        [StringLength(50, ErrorMessage = "Region or Parish is too long")]
        [RegularExpression("[a-zA-Z0-9'.\\-/\\s]+", ErrorMessage = "Region contains invalid characters")]
        public string Region
        {
            get;
            set;
        }

        [Required(ErrorMessage = "Village is required")]
        [StringLength(50, ErrorMessage = "Village is too long")]
        [RegularExpression("[a-zA-Z0-9'.\\-/\\s]+", ErrorMessage = "Village contains invalid characters")]
        public string Village
        {
            get;
            set;
        }

        [StringLength(10, ErrorMessage = "Zip Code is too long")]
        [RegularExpression("[a-zA-Z0-9'.\\-/\\s]+", ErrorMessage = "Zip code contains invalid characters")]
        public string ZipCode
        {
            get;
            set;
        }
    }
}