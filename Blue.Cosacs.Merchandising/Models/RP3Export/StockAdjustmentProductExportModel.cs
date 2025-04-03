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
    public class StockAdjustmentProductExportModel
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
        public string ProductCode;

        [FieldQuoted]
        [FieldTitle]
        public int UnitQuantity;

        [FieldQuoted]
        [FieldTitle]
        public decimal UnitCost;

        [FieldQuoted]
        [FieldTitle]
        public decimal UnitRetailPrice;
    }
}
