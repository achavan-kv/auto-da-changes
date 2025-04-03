using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blue.Cosacs.Sales.Api.Extensions
{
    public static class StringExtensions
    {
        public static string Capitalize(this string input)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(input[0]) + input.ToLower().Substring(1);
        }
    }
}