using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blue.Cosacs.Merchandising.Models
{
    public class SupplierImportModel
    {
        public SupplierImportModel()
        {
            Contacts = new List<StringKeyValue>();
        }

        [Required]
        [MaxLength(240)]
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

        [Required]
        [MaxLength(150)]
        public string State { get; set; }

        [MaxLength(20)]
        public string PostCode { get; set; }

        [MaxLength(50)]
        public string PaymentTerms { get; set; }

        [Required]
        [MaxLength(2)]
        public string Country { get; set; }

        [Required]
        [MaxLength(30)]
        public string Code { get; set; }

        public int Status { get; set; }

        public List<StringKeyValue> Contacts { get; set; }
    }
}
