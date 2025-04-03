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
    public class VendorReturnProductExportModel
    {
        [FieldQuoted]
        [FieldTitle]
        public string Company;

        [FieldQuoted]
        [FieldTitle]
        public int RTSNumber;

        [FieldQuoted]
        [FieldTitle]
        public int GRNNumber;

        [FieldQuoted]
        [FieldTitle]
        public int PONumber;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string ProductCode;

        [FieldQuoted]
        [FieldTitle]
        public int UnitsReturned;

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
        public decimal LastLandedCost;

        [FieldQuoted]
        [FieldTitle]
        public decimal UnitPrice;
    }
}
