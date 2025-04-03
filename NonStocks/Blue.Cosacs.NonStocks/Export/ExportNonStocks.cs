using System;

namespace Blue.Cosacs.NonStocks.Export
{
    public class Fact2000Interface
    {
        private const string priceFmt = "00000" + "00000";
        public string CreateProductAmendmentFileLine(ProductAmendmentFile f)
        {
            return String.Format(
                "{0,2}," + //      1 –   2 -  2 - Warehouse number
                "{1,-8}," + //     4 -  11 -  8 - Product Code
                "{2,-18}," + //   13 –  30 - 18 - Supplier Product Description
                "{3,-25}," + //   32 -  56 - 25 - Product Main Description
                "{4,-40}," + //   58 -  97 - 40 - Product Extra Description
                "{5,11}," + //    99 – 109 - 11 - HP Price (format S9(8)V99))
                "{6,11}," + //   111 – 121 - 11 - Cash Price (format S9(8)V99))
                "{7,2}," + //    123 - 124 -  2 - Product Category
                "{8,10}," + //   126 - 135 - 10 - Supplier AC No
                "{9,1}," + //          137 -  1 - Product Status (e.g. ‘L’ = Lead Time, ‘D’ = Discontinued)
                "{10,1}," + //         139 -  1 - Warrantable Product (‘Y’ or ‘N’)
                "{11,2}," + //   141 - 142 -  2 - Product Type Type (
                //                                          ‘02’ = Product w/o stock, ‘03’ = Product with stock,
                //                                          ‘06’ = Non-stock item, ‘07’ = Kit product)
                "{12,11}," + //  144 – 154 - 11 - Special Price (format S9(8)V99))
                "{13,2}," + //   156 - 157 -  2 - Warranty Reference

                "{14,13}," + //  159 - 171 - 13 - Product EAN Code
                "{15,3}," + //   173 - 175 -  3 - Product Lead Time
                "{16,1}," + //         177 -  1 - Warranty Renewal (‘Y’ or ‘N’)
                "{17,1}," + //         179 -  1 - Ready to Assemble (‘Y’ or ‘N’)
                "{18,1}," + //         181 -  1 - Deletion indicator (‘Y’ or ‘N’)
                "{19,11}," + //  183 - 193 - 11 - Cost Price
                "{20,25}," +//    195 - 219 - 25 - Supplier Name
                "{21,15}", //    Tax Rate
                f.WarehouseNumber.ToString("00"), //  0
                f.ProductCode, //                     1
                f.SupplierProductDescription, //      2
                f.ProductMainDescription, //          3
                f.ProductExtraDescription, //         4
                FormatPrice(f.HPPrice, "0"), //       5
                FormatPrice(f.CashPrice, "0"), //     6
                f.ProductCategory, //                 7
                f.SupplierACNo, //                    8
                f.ProductStatus, //                   9
                f.WarrantableProduct, //             10
                f.ProductTypeType, //                11
                FormatPrice(f.SpecialPrice, "0"), // 12
                f.WarrantyReference, //              13
                f.ProductEANCode, //                 14
                f.ProductLeadTime, //                15
                f.WarrantyRenewal, //                16
                f.ReadyToAssemble, //                17
                f.DeletionIndicator, //              18
                FormatPrice(f.CostPrice), //         19
                f.SupplierName, //                   20
                f.TaxRate
                ).TrimEnd();
        }

        public string CreatePromotionalPriceFileLine(PromotionalPriceFile f)
        {
            return String.Format(
            "{0,-8}," + //    1 -   8 -  8 - Product Code
            "{1,2}," + //   10 -  11 -  2 - Warehouse Number
            "{2,10}," + //  13 -  22 - 10 - HP Price 1 (format 9(8) V99)
            "{3,6}," + //   24 -  29 -  6 - HP Date ‘From’ 1(format ddmmyy)
            "{4,6}," + //   31 -  36 -  6 - HP Date ‘To’ 1(format ddmmyy)
            "{5,10}," + //  38 -  47 - 10 - HP Price 2 (format 9(8) V99)
            "{6,6}," + //   49 -  54 -  6 - HP Date ‘From’ 2(format ddmmyy)
            "{7,6}," + //   56 -  61 -  6 - HP Date ‘To’ 2(format ddmmyy)
            "{8,10}," + //  63 -  72 - 10 - HP Price 3 (format 9(8) V99)
            "{9,6}," + //   74 -  79 -  6 - HP Date ‘From’ 3 (format ddmmyy)
            "{10,6}," + //   81 -  86 -  6 - HP Date ‘To’ 3 (format ddmmyy)
            "{11,10}," + //  88 -  97 - 10 - Cash Price 1 (format 9(8) V99)
            "{12,6}," + //   99 - 104 -  6 - Cash Date ‘From’ 1 (format ddmmyy)
            "{13,6}," + //  106 - 111 -  6 - Cash Date ‘To’ 1 (format ddmmyy)
            "{14,10}," + // 113 - 122 - 10 - Cash Price 2 (format 9(8) V99)
            "{15,6}," + //  124 - 129 -  6 - Cash Date ‘From’ 2 (format ddmmyy)
            "{16,6}," + //  131 - 136 -  6 - Cash Date ‘To’ 2 (format ddmmyy)
            "{17,10}," + // 138 - 147 - 10 - Cash Price 3 (format 9(8) V99)
            "{18,6}," + //  149 - 154 -  6 - Cash Date ‘From’ 3 (format ddmmyy)
            "{19,6}", //    156 - 161 -  6 - Cash Date ‘To’ 3 (format ddmmyy)
            f.ProductCode, //                   0
            f.WarehouseNumber.ToString("00"),// 1
            FormatPrice(f.HPPrice1, "0"), //    2
            FormatDate(f.HPDateFrom1), //       3
            FormatDate(f.HPDateTo1), //         4
            FormatPrice(f.HPPrice2, "0"), //    5
            FormatDate(f.HPDateFrom2), //       6
            FormatDate(f.HPDateTo2), //         7
            FormatPrice(f.HPPrice3, "0"), //    8
            FormatDate(f.HPDateFrom3), //       9
            FormatDate(f.HPDateTo3), //        10
            FormatPrice(f.CashPrice1, "0"), // 11
            FormatDate(f.CashDateFrom1), //    12
            FormatDate(f.CashDateTo1), //      13
            FormatPrice(f.CashPrice2, "0"), // 14
            FormatDate(f.CashDateFrom2), //    15
            FormatDate(f.CashDateTo2), //      16
            FormatPrice(f.CashPrice3, "0"), // 17
            FormatDate(f.CashDateFrom3), //    18
            FormatDate(f.CashDateTo3) //       19
            ).TrimEnd();
        }

        public string CreateProductLinkFileLine(ProductLinkFile f)
        {
            return String.Format(
            "{0,3}," + //   1 -  3 -  3 - Product Group
            "{1,5}," + //   5 -  9 -  5 - Category
            "{2,3}," + //  11 - 13 -  3 - Class
            "{3,5}," + //  15 - 19 -  5 - Sub Class
            "{4,-18}", //  21 - 38 - 18 - Associated Item Id
            f.ProductGroup,
            f.Category,
            f.Class,
            f.SubClass,
            f.AssociatedItemId
            ).TrimEnd();
        }

        private string FormatPrice(decimal price, string padFiller = "")
        {
            var formattedRetPrice = string.Empty;
            var sign = price >= 0 ? "+" : "-";

            if (padFiller == "0")
            {
                formattedRetPrice =
                    sign + GetPriceFormat98V99Int(price).ToString(priceFmt);
            }
            else
            {
                formattedRetPrice =
                    price == 0 ? string.Empty : sign + GetPriceFormat98V99Int(price).ToString(priceFmt);
            }

            formattedRetPrice = formattedRetPrice.Replace("++", "+");
            formattedRetPrice = formattedRetPrice.Replace("--", "-");

            return formattedRetPrice;
        }

        private int GetPriceFormat98V99Int(decimal price)
        {
            return (int)(price * 100);
        }

        private string FormatDate(DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.Day.ToString("00") +
                    date.Value.Month.ToString("00") +
                    date.Value.ToString("yy");
            }

            return "000000";
        }
    }
}

