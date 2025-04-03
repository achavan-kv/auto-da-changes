namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Blue.Cosacs.Merchandising.Helpers;
    using FileHelpers;

    [DelimitedRecord(",")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Required for export")]
    public class StockReceivedReportModel
    {
        [FieldTitle]
        [FieldQuoted]
        public string Division;

        [FieldTitle]
        [FieldQuoted]
        public string Department;

        [FieldTitle]
        [FieldQuoted]
        public string Class;

        [FieldHidden]
        public int ProductId;

        [FieldTitle]
        [FieldQuoted]
        public string Sku;

        [FieldTitle]
        [FieldQuoted]
        public string Description;

        [FieldHidden]
        public int LocationId;

        [FieldTitle]
        [FieldQuoted]
        public string Location;

        [FieldTitle]
        [FieldQuoted]
        public string Vendor;

        [FieldHidden]
        public int? VendorId;

        [FieldTitle]
        [FieldQuoted]
        public DateTime? Date;

        [FieldTitle]
        [FieldQuoted]
        public DateTime? DateLastReceived;

        [FieldTitle("QtyReceived")]
        [FieldQuoted]
        public int? Quantity;

        [FieldTitle("LandedCost")]
        [FieldQuoted]
        public decimal? LastLandedCost;

        [FieldTitle]
        [FieldQuoted]
        public decimal? ExtendedLandedCost;

        [FieldHidden]
        public ReferenceLink ReferenceNumberCsl;

        [FieldTitle("ReferenceNumber")]
        [FieldQuoted]
        public string ReferenceNumberExport;

        [FieldTitle]
        [FieldQuoted]
        public int StockOnHand;

        [FieldTitle]
        [FieldQuoted]
        public int? PurchaseOrderId;

        [FieldTitle("StockOnOrder")]
        [FieldQuoted]
        public int? PendingStock;
    }
}
