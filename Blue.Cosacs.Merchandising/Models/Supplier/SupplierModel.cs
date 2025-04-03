using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blue.Cosacs.Merchandising.Models
{
    public class SupplierModel
    {
        public SupplierModel()
        {
            Contacts = new List<StringKeyValue>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(40, ErrorMessage = "Please keep the Vendor Name within 40 characters")]
        [RegularExpression("^[^,]+$", ErrorMessage = "Please remove comma from field \"Vendor Name\"")]
        public string Name { get; set; }

        [Required]
        [MaxLength(20)]
        public string Type { get; set; }

        [MaxLength(240)]
        public string AddressLine1 { get; set; }

        [MaxLength(240)]
        public string AddressLine2 { get; set; }

        [Required]
        [MaxLength(25)]
        public string City { get; set; }

        [MaxLength(150)]
        public string State { get; set; }

        [MaxLength(20)]
        public string PostCode { get; set; }

        [MaxLength(50)]
        public string PaymentTerms { get; set; }

        [MaxLength(100)]
        public string OrderEmail { get; set; }

        [MaxLength(10)]
        public string Currency { get; set; }

        [Required]
        [MaxLength(2)]
        public string Country { get; set; }

        [Required]
        [MaxLength(30)]
        public string Code { get; set; }

        public int Status { get; set; }

        public List<StringKeyValue> Contacts { get; set; }

        public List<string> Tags { get; set; }

        /// <summary>
        /// Author : Rahul Dubey
        /// Date   : 15/02/2019
        /// CR     : #Ashley
        /// Details: "LeadTime" will Capture Lead time of vendor in number of Day's.
        /// </summary>
        public int LeadTime { get; set; }
    }
}
