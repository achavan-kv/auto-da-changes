namespace Blue.Cosacs.Merchandising.Models.RP3Export
{
    using System.Diagnostics.CodeAnalysis;

    using Blue.Cosacs.Merchandising.Helpers;

    using FileHelpers;

    [DelimitedRecord(",")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Required for export")]
    public class PurchaseOrderExportModel
    {
        public PurchaseOrderExportModel()
        {
            DiscountPercentage = 0;
            DiscountValue = 0;
        }

        [FieldQuoted]
        [FieldTitle]
        public string Company;

        [FieldQuoted]
        [FieldTitle]
        public int PONumber;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string VendorCode;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string VendorName;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string ReceivingLocationCode;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string ReceivingLocationName;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string POStatus;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string POType;

        [FieldQuoted]
        [FieldTitle]
        public string TransactionDate;

        [FieldQuoted]
        [FieldTitle]
        public string ExpectedDeliveryDate;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string CorporatePONumber;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string SupplierInvoiceNumber;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string ReferenceNumber;

        [FieldQuoted]
        [FieldTitle]
        public decimal SubTotal;

        [FieldQuoted]
        [FieldTitle]
        public decimal Total;

        [FieldQuoted]
        [FieldTitle]
        public decimal DiscountPercentage;

        [FieldQuoted]
        [FieldTitle]
        public decimal DiscountValue;
    }
}
