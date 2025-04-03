using System;
using System.Globalization;

namespace Unicomer.Cosacs.Business
{
    public class CommonHelper
    {
        public static DateTime GetCultDateTime(string dateString)
        {
            DateTime dateVal = DateTime.MinValue;
            string[] formats = { "dd/MM/yyyy", "MM/dd/yyyy", "MM/dd/yyyy hh:mm:ss tt", "M/d/yyyy", "d/M/yyyy", "M-d-yyyy", "d-M-yyyy", "d-MMM-yy", "d-MMMM-yyyy" };
            for (int i = 0; i < formats.Length; i++)
            {
                DateTime date;
                if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    dateVal = date;
                    break;
                }
            }
            return dateVal;
        }
    }
}
