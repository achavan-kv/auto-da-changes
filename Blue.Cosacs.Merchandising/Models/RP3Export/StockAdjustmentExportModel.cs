namespace Blue.Cosacs.Merchandising.Models.RP3Export
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Blue.Cosacs.Merchandising.Helpers;

    using FileHelpers;

    [DelimitedRecord(",")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Required for export")]
    public class StockAdjustmentExportModel
    {
        [FieldQuoted]
        [FieldTitle]
        public string Company;

        [FieldQuoted]
        [FieldTitle]
        public int StockAdjustmentNumber;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string PrimaryReason;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string SecondaryReason;

        [FieldQuoted]
        [FieldTitle]
        public string ReasonSign;

        [FieldQuoted]
        [FieldTitle]
        public string LocationCode;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string LocationName;

        [FieldQuoted]
        [FieldTitle]
        public string TransactionDate;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string AdjustmentStatus;

        [FieldQuoted]
        [FieldTitle]
        public decimal ExtendedCost;

        [FieldQuoted]
        [FieldTitle]
        public decimal ExtendedPrice;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string Notes;
    }
}
