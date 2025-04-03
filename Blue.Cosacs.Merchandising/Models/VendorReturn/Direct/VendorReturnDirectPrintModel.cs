namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class VendorReturnDirectPrintModel
    {
        public VendorReturnDirectPrintModel()
        {
            Products = new List<VendorReturnProductPrintModel>();
        }

        public int Id { get; set; }

        public GoodsReceiptDirectViewModel GoodsReceipt { get; set; }

        public string Comments { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public int CreatedById { get; set; }

        public string FormattedTotalCost { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string ApprovedBy { get; set; }

        public string Vendor { get; set; }

        public string ReferenceNumber { get; set; }

        public List<VendorReturnProductPrintModel> Products { get; set; }
    }
}
