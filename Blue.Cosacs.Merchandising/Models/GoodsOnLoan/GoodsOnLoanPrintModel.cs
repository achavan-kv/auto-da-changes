namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class GoodsOnLoanPrintModel
    {
        public GoodsOnLoanPrintModel()
        {
            this.Products = new List<GoodsOnLoanProductPrintModel>();
        }

        public string Type
        {
            get
            {
                return !string.IsNullOrWhiteSpace(BusinessName) ? "Business" : !string.IsNullOrWhiteSpace(CustomerId) ? "Customer" : null;
            }
        }

        public int? Id { get; set; }

        public int? ServiceRequestId { get; set; }

        public string BusinessName { get; set; }

        public string JobTitle { get; set; }

        public string CustomerId { get; set; }

        public string Title { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string TownCity { get; set; }

        public string PostCode { get; set; }

        public string AddressNotes { get; set; }

        public string Comments { get; set; }

        public DateTime? ExpectedCollectionDate { get; set; }

        public DateTime? CollectedDate { get; set; }

        public DateTime CreatedOn { get; set; }

        public int? CreatedById { get; set; }

        public string CreatedBy { get; set; }

        public string StockLocation { get; set; }

        public bool DeliveryPrinted { get; set; }
        public bool CollectionPrinted { get; set; }
        
        public List<GoodsOnLoanProductPrintModel> Products { get; set; }

        public List<StringKeyValue> Contacts { get; set; }

        public List<StringKeyValue> DeliveryContactDetails { get; set; }
    }
}