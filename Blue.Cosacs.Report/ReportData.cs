using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Blue.Cosacs.Report
{
    public static class ReportData
    {
        public static byte[] ToByteArray(this List<List<string>> data)
        {
            var file = new StringBuilder();
            var replacer = new Regex("\"", RegexOptions.Compiled);

            if (data != null)
            {
                foreach (var item in data)
                {
                    file.Append(string.Join(",", item.Select(p => string.Format("\"{0}\"", replacer.Replace(p, "\"\""))).ToList())).AppendLine();
                }
            }
            return Encoding.GetEncoding("Windows-1252").GetBytes(file.ToString());
        }
    }
}
