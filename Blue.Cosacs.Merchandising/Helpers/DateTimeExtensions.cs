namespace Blue.Cosacs.Merchandising.Helpers
{
    using System;

    public static class DateTimeExtensions
    {
        public static string ToString(this DateTime? dt, string formatString)
        {
            return dt.HasValue ? dt.Value.ToString(formatString) : string.Empty;
        }

        public static DateTime ToServerTime(this DateTime dt)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dt, TimeZoneInfo.Local);
        }

        public static DateTime? ToServerTime(this DateTime? dt)
        {
            if (dt == null)
            {
                return null;
            }
            return TimeZoneInfo.ConvertTimeFromUtc(dt.Value, TimeZoneInfo.Local);
        }

        public static string ToAuditDateString(this DateTime dt)
        {
            return dt.ToString("dd MMMM yyyy");
        }

        public static string ToAuditDateString(this DateTime? dt)
        {
            return dt.HasValue
                ? dt.Value.ToString("dd MMMM yyyy")
                : string.Empty;
        }

        public static string ToAuditDateTimeString(this DateTime dt)
        {
            return ToServerTime(dt).ToString("dd MMMM yyyy T");
        }

        public static string ToAuditDateTimeString(this DateTime? dt)
        {
            return dt.HasValue
                ? ToServerTime(dt.Value).ToString("dd MMMM yyyy T")
                : string.Empty;
        }

        public static string ToPrintDateString(this DateTime dt)
        {
            return dt.ToString("dd MMMM yyyy");
        }

        public static string ToPrintDateString(this DateTime? dt)
        {
            return dt.HasValue
                ? dt.Value.ToString("dd MMMM yyyy")
                : string.Empty;
        }

        public static string ToLocalPrintDateString(this DateTime? dt)
        {
            return dt.HasValue
                ? ToServerTime(dt.Value).ToString("dd MMMM yyyy")
                : string.Empty;
        }

        public static string ToPrintDateTimeString(this DateTime dt)
        {
            return ToServerTime(dt).ToString("dd MMMM yyyy HH:mm");
        }

        public static string ToPrintDateTimeString(this DateTime? dt)
        {
            return dt.HasValue
                ? ToServerTime(dt.Value).ToString("dd MMMM yyyy HH:mm")
                : string.Empty;
        }

        public static string ToSolrDate(this DateTime dt)
        {
            return dt.ToString("s") + "Z";
        }

        public static string ToSolrDate(this DateTime? dt)
        {
            return dt.HasValue
                ? dt.Value.ToString("s") + "Z"
                : null;
        }

        public static DateTime? ToLocalDateTime(this DateTime? utcDateTime)
        {
            return utcDateTime.HasValue ? DateTime.SpecifyKind(utcDateTime.Value, DateTimeKind.Utc).ToLocalTime() : (DateTime?)null;
        }

        public static DateTime ToLocalDateTime(this DateTime utcDateTime)
        {
            return DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc).ToLocalTime();
        }
    }
}
