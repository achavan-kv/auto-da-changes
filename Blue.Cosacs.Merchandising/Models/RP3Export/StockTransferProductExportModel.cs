namespace Blue.Cosacs.Merchandising.Models.RP3Export
{
    using System.Diagnostics.CodeAnalysis;

    using Blue.Cosacs.Merchandising.Helpers;

    using FileHelpers;

    [DelimitedRecord(",")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Required for export")]
    public class StockTransferProductExportModel
    {
        [FieldQuoted]
        [FieldTitle]
        public string Company;

        [FieldQuoted]
        [FieldTitle]
        public string StockTransferNote;

        [FieldQuoted]
        [FieldTitle]
        public string ProductCode;

        [FieldQuoted]
        [FieldTitle]
        public int SendingUnits;

        [FieldQuoted]
        [FieldTitle]
        public int ReceivingUnits;

        [FieldQuoted]
        [FieldTitle]
        public decimal UnitCost;

        [FieldQuoted]
        [FieldTitle]
        public decimal UnitPrice;
    }
}
