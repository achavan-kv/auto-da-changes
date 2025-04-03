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
    public class WeeklyMerchandisingReportExportModel
    {
        [FieldQuoted]
        [FieldTitle]
        public string Country;

        [FieldQuoted]
        [FieldTitle("A/CYear")]
        public string ACYear;

        [FieldQuoted]
        [FieldTitle("A/Cmonth")]
        public string ACMonth;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string Store;

        [FieldQuoted]
        [FieldTitle("Prod.Cat")]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string ProdCat;

        [FieldQuoted]
        [FieldTitle("Cat.Desc")]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string CatDesc;

        [FieldQuoted]
        [FieldTitle("Dept.Code")]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string DeptCode;

        [FieldQuoted]
        [FieldTitle("Dept.Desc")]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string DeptDesc;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string BrandCode;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string BrandDesc;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string VendorCode;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string VendorName;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string Style;

        [FieldQuoted]
        [FieldTitle]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string Product;

        [FieldQuoted]
        [FieldTitle("Prod.Desc")]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        public string ProdDesc;

        [FieldQuoted]
        [FieldTitle("Init.S/Units")]
        public int InitSUnits;

        [FieldQuoted]
        [FieldTitle("Init.S/Value")]
        public decimal InitSValue;

        [FieldQuoted]
        [FieldTitle("Init.S/Sales")]
        public decimal InitSSales;

        [FieldQuoted]
        [FieldTitle("Purch.Units")]
        public int PurchUnits;

        [FieldQuoted]
        [FieldTitle("Purch.Value")]
        public decimal PurchValue;

        [FieldQuoted]
        [FieldTitle("Purch.Sales")]
        public decimal PurchSales;

        [FieldQuoted]
        [FieldTitle]
        public int UnitSales;

        [FieldQuoted]
        [FieldTitle]
        public decimal CostSales;

        [FieldQuoted]
        [FieldTitle]
        public decimal RetailSales;

        [FieldQuoted]
        [FieldTitle("UnitAdj.")]
        public int UnitAdj;

        [FieldQuoted]
        [FieldTitle("CostAdj.")]
        public decimal CostAdj;

        [FieldQuoted]
        [FieldTitle("SalesAdj.")]
        public decimal SalesAdj;

        [FieldQuoted]
        [FieldTitle("UnitTrans.")]
        public int UnitTrans;

        [FieldQuoted]
        [FieldTitle("CostTrans.")]
        public decimal CostTrans;

        [FieldQuoted]
        [FieldTitle("SalesTrans.")]
        public decimal SalesTrans;

        [FieldQuoted]
        [FieldTitle("FinalS/Units")]
        public int FinalSUnits;

        [FieldQuoted]
        [FieldTitle("FinalS/Sales")]
        public decimal FinalSSales;

        [FieldQuoted]
        [FieldTitle("FinalS/Value")]
        public decimal FinalSValue;

        [FieldQuoted]
        [FieldTitle]
        public decimal MarkUp;

        [FieldQuoted]
        [FieldTitle]
        public decimal MarkDown;

        [FieldQuoted]
        [FieldTitle]
        public string LastPDate;

        [FieldQuoted]
        [FieldTitle]
        public string LastSDate;

        [FieldQuoted]
        [FieldTitle]
        public string LastTDate;

        [FieldQuoted]
        [FieldTitle]
        public string FirstRDate;
    }
}
