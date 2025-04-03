using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blue.Cosacs.Merchandising.Models
{
    public class LocationModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string LocationId { get; set; }

        [MaxLength(100)]
        public string SalesId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Fascia { get; set; }

        [Required]
        public bool Warehouse { get; set; }

        [Required]
        public bool VirtualWarehouse { get; set; }

        [Required]
        public bool Active { get; set; }

        [MaxLength(100)]
        public string AddressLine1 { get; set; }

        [MaxLength(100)]
        public string AddressLine2 { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(10)]
        public string PostCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string StoreType { get; set; }

        public List<StringKeyValue> Contacts { get; set; }
    }
}
