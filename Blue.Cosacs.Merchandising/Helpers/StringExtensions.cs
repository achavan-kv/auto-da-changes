using System;

namespace Blue.Cosacs.Merchandising.Helpers
{
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    public enum TitleCaseOptions
    {
        PreserveAcronyms,
        ConvertAll
    }

    public static class StringExtensions
    {
        public static string ToCamelCase(this string theString, bool removeSpaces = true)
        {
            if (theString == null || theString.Length < 2)
            {
                return theString;
            }

            var words = theString.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);
            var result = words[0].ToLower();
            for (var i = 1; i < words.Length; i++) 
            {
                result += (removeSpaces ? string.Empty : " ") + words[i].Substring(0, 1).ToUpper() + words[i].Substring(1);
            }
            return result;
        }

        public static string ToTitleCase(this string str, TitleCaseOptions options = TitleCaseOptions.PreserveAcronyms)
        {
            if (str == null)
            {
                return null;
            }
            if (options == TitleCaseOptions.ConvertAll)
            {
                str = str.ToLower();
            }
            var ti = new CultureInfo("en-US", false).TextInfo;

            return ti.ToTitleCase(str);
        }

        public static string AlphaNumericOnly(this string str)
        {
            return Regex.Replace(str ?? string.Empty, "[^a-zA-Z0-9_.]+", string.Empty, RegexOptions.Compiled);
        }

        public static string FormatForExport(this decimal number, int integralDigits, int decimalDigits)
        {
            var format = string.Join(string.Empty, Enumerable.Repeat("0", integralDigits)) + "." + string.Join(string.Empty, Enumerable.Repeat("0", decimalDigits));
            var zeroFormat = string.Join(string.Empty, Enumerable.Repeat("0", integralDigits + decimalDigits));

            var stringFormat = string.Format("+{0};-{0};+{1}", format, zeroFormat);

            var result = number.ToString(stringFormat).Replace(".", string.Empty);

            return result;
        }

        public static string FormatForExport(this int number, int integralDigits, int decimalDigits)
        {
            return FormatForExport(Convert.ToDecimal(number), integralDigits, decimalDigits);
        }

        public static string FormatForExport(this decimal? number, int integralDigits, int decimalDigits)
        {
            return FormatForExport(number.GetValueOrDefault(0), integralDigits, decimalDigits);
        }

        public static string ReplaceFirst(this string source, string find, string replace)
        {
            var place = source.IndexOf(find, StringComparison.InvariantCulture);
            return source.Remove(place, find.Length).Insert(place, replace);
        }

        public static string ReplaceLast(this string source, string find, string replace)
        {
            var place = source.LastIndexOf(find, StringComparison.InvariantCulture);
            return source.Remove(place, find.Length).Insert(place, replace);
        }
    }
}
