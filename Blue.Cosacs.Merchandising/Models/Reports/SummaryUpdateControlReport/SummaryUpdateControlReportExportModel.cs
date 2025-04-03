namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Blue.Cosacs.Merchandising.Helpers;
    using FileHelpers;

    [DelimitedRecord(",")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Required for export")]
    public class SummaryUpdateControlReportExportModel
    {
        [FieldTitle]
        [FieldQuoted]
        public string SKU;

        [FieldTitle]
        [FieldQuoted]
        public string Reference;

        [FieldTitle]
        [FieldQuoted]
        public string TransactionType;

        [FieldTitle]
        [FieldConverter(ConverterKind.Date, "dd/MM/yyyy")]
        public DateTime TransactionDate;

        [FieldTitle]
        [FieldQuoted]
        public string Units;

        [FieldTitle]
        [FieldQuoted]
        public string Value;

        [FieldTitle]
        [FieldQuoted]
        public string SalesId;

        [FieldTitle]
        [FieldQuoted]
        public string Location;

        [FieldTitle]
        [FieldQuoted]
        public string RunNumber;

        [FieldTitle]
        [FieldQuoted]
        public string ProductType;
    }
}
