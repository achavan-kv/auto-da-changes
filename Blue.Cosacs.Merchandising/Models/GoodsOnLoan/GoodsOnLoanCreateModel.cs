namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class GoodsOnLoanCreateModel
    {
        public GoodsOnLoanCreateModel()
        {
            this.Products = new List<GoodsOnLoanProductCreateModel>();
        }

        public int? ServiceRequestId { get; set; }

        [StringLength(100)]
        public string BusinessName { get; set; }

        [StringLength(100)]
        public string CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [StringLength(100)]
        public string AddressLine1 { get; set; }

        [StringLength(100)]
        public string AddressLine2 { get; set; }

        [StringLength(100)]
        public string TownCity { get; set; }

        [StringLength(100)]
        public string PostCode { get; set; }

        public DateTime ExpectedCollectionDate { get; set; }

        public DateTime? PreferredDeliveryDate { get; set; }

        [Range(1, int.MaxValue)]
        [Required]
        public int StockLocationId { get; set; }

        [StringLength(1000)]
        public string Comments { get; set; }

        public List<GoodsOnLoanProductCreateModel> Products { get; set; }

        public List<StringKeyValue> Contacts { get; set; }
        public List<StringKeyValue> DeliveryContactDetails { get; set; }

        [StringLength(1000)]
        public string AddressNotes { get; set; }

        public string JobTitle { get; set; }
    }
}