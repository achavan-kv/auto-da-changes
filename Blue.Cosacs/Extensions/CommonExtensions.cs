using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace Blue.Cosacs
{
    public static class CommonExtensions
    {
        private static string DATTIME_DEFAULT_LONG_FORMAT = "f";
        private static string DATTIME_DEFAULT_SHORT_FORMAT = "g";
        private const string DEFAULT_UI_CULTURE = "en-GB";

        #region This should not be here

        private static string defaultUiCulture = null;
        private static string DefaultUiCulture
        {
            get
            {
                if (string.IsNullOrEmpty(defaultUiCulture))
                {
                    defaultUiCulture = DEFAULT_UI_CULTURE;
                }
                return defaultUiCulture;
            }
        }

        /// <summary>
        /// Sets culture used by the user interface.
        /// </summary>
        /// <param name="cultureName">the format "languagecode2" "country/regioncode2".</param>
        /// <example>"en-GB"</example>
        public static void SetUiCulture(string cultureName)
        {
            defaultUiCulture = cultureName;
        }

        #endregion

        //public static void With<T>(this T obj, Action<T> action)
        //{
        //    action(obj);
        //}

        public static TReturn NullSafe<TIn, TReturn>(this TIn obj, Func<TIn, TReturn> ifNotNull, TReturn ifNull = default(TReturn))
            where TIn : class
        {
            return obj != null ? ifNotNull(obj) : ifNull;
        }

        public static TReturn NullSafe<TIn, TReturn>(this Nullable<TIn> value, Func<Nullable<TIn>, TReturn> ifNotNull, TReturn ifNull = default(TReturn))
            where TIn : struct
        {
            return value.HasValue ? ifNotNull(value) : ifNull;
        }

        public static string Stringify<T>(this IEnumerable<T> collection, Func<T, string> elementToString = null, string separator = " ")
        {
            if (collection == null || separator == null)
                return "";

            return String.Join(separator,
                               collection.Select(elementToString ?? (t => t.ToString())));
        }

        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }

        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }

        public static IEnumerable<TSource> WhereIfNot<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            return source.WhereIf(!condition, predicate);
        }

        public static IQueryable<TSource> WhereIfNot<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            return source.WhereIf(!condition, predicate);
        }

        public static IEnumerable<TSource> TakeIf<TSource>(this IEnumerable<TSource> source, bool condition, int count)
        {
            if (condition)
                return source.Take(count);
            else
                return source;
        }

        public static IQueryable<TSource> TakeIf<TSource>(this IQueryable<TSource> source, bool condition, int count)
        {
            if (condition)
                return source.Take(count);
            else
                return source;
        }

        public static bool In<T>(this T value, params T[] list)
        {
            list = list ?? new T[] { };
            return list.Contains(value);
        }

        public static DateTime IncludeDay(this DateTime date)
        {
            return date.AddDays(1).AddMilliseconds(-1);
        }

        public static bool NotIn<T>(this T value, params T[] list)
        {
            return !value.In(list);
        }

        public static string[] SplitAt(this string str, params int[] indices)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            if (indices == null)
                indices = new int[] { };

            if (indices.Any(i => i < 0 || i > str.Length))
                throw new ArgumentOutOfRangeException("indices");

            indices = indices
                        .Union(new[] { str.Length })
                        .Except(new[] { 0 })
                        .OrderBy(index => index)
                        .Distinct()
                        .ToArray();

            var previousIndex = 0;

            return indices.Select(index =>
            {
                var s = str.Substring(previousIndex, index - previousIndex);
                previousIndex = index;
                return s;
            })
            .ToArray();
        }

        /// <summary>
        /// Convert a <c>System.DateTime</c> value to string to be displayed on the UI
        /// </summary>
        /// <param name="value">A <c>System.DateTime</c> to be convertted</param>
        /// <returns>
        /// By defualt it uses the "f" to convert the <paramref name="value"/> to string
        /// </returns>
        public static string DateToUILongString(this DateTime value)
        {
            return value.DateToUILongString(DATTIME_DEFAULT_LONG_FORMAT);
        }

        /// <summary>
        /// Convert a <c>System.DateTime</c> value to string to be displayed on the UI
        /// </summary>
        /// <param name="value">A <c>System.DateTime</c> to be convertted</param>
        /// <returns>
        /// <param name="defaultFormat">The default format.</param>
        public static string DateToUILongString(this DateTime value, string defaultFormat)
        {
            return value.ToString(defaultFormat, System.Globalization.CultureInfo.GetCultureInfo(DefaultUiCulture));
        }

        /// <summary>
        /// Convert a <c>System.DateTime</c> value to string to be displayed on the UI
        /// </summary>
        /// <param name="value">A <c>System.DateTime</c> to be convertted</param>
        /// <returns>
        /// By defualt it uses the "g" to convert the <paramref name="value"/> to string
        /// </returns>
        public static string DateToUIShortString(this DateTime value)
        {
            return value.DateToUIShortString(DATTIME_DEFAULT_SHORT_FORMAT);
        }

        /// <summary>
        /// Convert a <c>System.DateTime</c> value to string to be displayed on the UI
        /// </summary>
        /// <param name="value">A <c>System.DateTime</c> to be convertted</param>
        /// <returns>
        /// <param name="defaultFormat">The default format.</param>
        public static string DateToUIShortString(this DateTime value, string defaultFormat)
        {
            return value.ToString(defaultFormat, System.Globalization.CultureInfo.GetCultureInfo(DefaultUiCulture));
        }

        public static DataTable ToDataTable<T>(this List<T> list)
        {
            Type type = typeof(T);
            DataTable dt = new DataTable(type.Name);

            var propertyInfos = type.GetProperties().ToList();

            propertyInfos.ForEach(propertyInfo =>
            {
                Type columnType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                dt.Columns.Add(propertyInfo.Name, columnType);
            });

            list.ForEach(item =>
            {
                DataRow row = dt.NewRow();
                propertyInfos.ForEach(
                propertyInfo =>
                row[propertyInfo.Name] = propertyInfo.GetValue(item, null) ?? DBNull.Value
                );
                dt.Rows.Add(row);
            });

            return dt;
        }

        public static int MonthDifference(this DateTime date1, DateTime date2)
        {
            //return (date1.Month - date2.Month) + 12 * (date1.Year - date2.Year);
            return (date1.Month - date2.Month) + ((date1.Year - date2.Year) * 12) + (date1.Day >= date2.Day ? 1 : 0);
        }

        public static decimal RoundTo(this decimal d, int places)
        {
            return Math.Round(d, places);
        }

    }
}
