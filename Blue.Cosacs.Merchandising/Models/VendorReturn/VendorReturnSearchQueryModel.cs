namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class VendorReturnSearchQueryModel
    {       
        public int? VendorId { get; set; }

        public int? LocationId { get; set; }

        public int? MinVendorReturnId { get; set; }

        public int? MaxVendorReturnId { get; set; }

        public DateTime? MinCreatedDate { get; set; }

        public DateTime? MaxCreatedDate { get; set; }

        public int? Approved { get; set; }
        
        public string Type { get; set; }
    }
}
