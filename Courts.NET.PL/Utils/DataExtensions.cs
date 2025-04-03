using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using STL.Common.Constants.ColumnNames;
using Blue.Cosacs.Shared.Extensions;
using System.Text.RegularExpressions;

namespace STL.PL.Utils
{
    public static class DataExtensions
    {
        public static DataTable AddBlankCode(this DataTable dt)
        {
            if ((dt.Select(CN.CodeDescription + " = '' and " + CN.CodeDescription + " = ' '")).Length == 0)
            {
                DataRow row = dt.NewRow();
                row[CN.CodeDescription] = string.Empty;
                row[CN.Code] = string.Empty;
                dt.Rows.Add(row);
            }
            return dt;
        }

     

        public static decimal ToDecimal(this string text)
        {
            var d = 0m;
            if (decimal.TryParse(text, out d))
                return d;
            else
                return 0;
        }
    }
}
