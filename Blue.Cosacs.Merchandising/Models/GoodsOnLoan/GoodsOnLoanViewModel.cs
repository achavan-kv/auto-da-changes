namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class GoodsOnLoanViewModel
    {
        public GoodsOnLoanViewModel()
        {
            this.Products = new List<GoodsOnLoanProductViewModel>();
        }

        public string Type
        {
            get
            {
                return !string.IsNullOrWhiteSpace(BusinessName) ? "business" : !string.IsNullOrWhiteSpace(CustomerId) ?"customer" : null;
            }
        }

        public string Status
        {
            get
            {
                return CollectedDate.HasValue ? "Completed" : DateTime.UtcNow > ExpectedCollectionDate ? "Awaiting Collection" : "In Progress";
            }
        }

        public int? Id { get; set; }

        public int? ServiceRequestId { get; set; }

        public string BusinessName { get; set; }

        public string CustomerId { get; set; }

        public string Title { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string TownCity { get; set; }

        public string PostCode { get; set; }

        public int? StockLocationId { get; set; }

        public DateTime? ExpectedCollectionDate { get; set; }

        public DateTime? PreferredDeliveryDate { get; set; }

        public DateTime? CollectedDate { get; set; }

        public string Comments { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedById { get; set; }

        public string CreatedBy { get; set; }
        public List<GoodsOnLoanProductViewModel> Products { get; set; }

        public string StockLocation { get; set; }

        public List<StringKeyValue> Contacts { get; set; }
        public List<StringKeyValue> DeliveryContactDetails { get; set; }

        public string JobTitle { get; set; }

        public string AddressNotes { get; set; }
    }
}