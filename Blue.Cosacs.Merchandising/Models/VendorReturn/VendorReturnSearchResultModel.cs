namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class VendorReturnSearchResultModel
    {
        public int Id { get; set; }

        public int VendorReturnId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Status { get; set; }

        public int GoodsReceiptId { get; set; }

        public string GoodsReceiptType { get; set; }

        public decimal? TotalCost { get; set; }

        public string Vendor { get; set; }
       
        public int VendorId { get; set; }

        public string Location { get; set; }

        public int LocationId { get; set; }
    }
}
