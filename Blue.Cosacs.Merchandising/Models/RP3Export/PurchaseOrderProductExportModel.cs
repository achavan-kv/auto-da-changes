using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Merchandising.Models.RP3Export
{
    using System.Diagnostics.CodeAnalysis;

    using Blue.Cosacs.Merchandising.Helpers;

    using FileHelpers;

    [DelimitedRecord(",")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Required for export")]
    public class PurchaseOrderProductExportModel
    {
        public PurchaseOrderProductExportModel()
        {
            // unused fields
            UnitsFreeOfCharge = 0;
            DiscountPercentage = 0;
            TaxValue = 0;
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
        public string ProductCode;

        [FieldQuoted]
        [FieldTitle]
        public int LineNumber;

        [FieldQuoted]
        [FieldTitle]
        public int UnitsOrdered;

        [FieldQuoted]
        [FieldTitle]
        public int UnitsReceived;

        [FieldQuoted]
        [FieldTitle]
        public int UnitsFreeOfCharge;

        [FieldQuoted]
        [FieldTitle]
        public decimal SupplierUnitCost;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string SupplierCurrency;

        [FieldQuoted]
        [FieldTitle]
        public decimal PreLandedUnitCost;

        [FieldQuoted]
        [FieldTitle]
        public decimal ActualLandedUnitCost;

        [FieldQuoted]
        [FieldTitle]
        public decimal UnitPrice;

        [FieldQuoted]
        [FieldTitle]
        public decimal SubTotal;

        [FieldQuoted]
        [FieldTitle]
        public decimal Total;

        [FieldTitle]
        [FieldQuoted]
        public decimal DiscountPercentage;

        [FieldTitle]
        [FieldQuoted]
        public decimal TaxValue;
    }
}
