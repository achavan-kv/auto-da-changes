using System;
using System.Globalization;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Blue.Cosacs.Shared;
using System.Reflection;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace System.Runtime.CompilerServices
{
    public class ExtensionAttribute : Attribute { }
}

namespace Blue.Cosacs.Shared.Extensions
{
    public static class CommonExtensions
    {
        [DebuggerStepThrough]
        public static TimeSpan Seconds(this int seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        [DebuggerStepThrough]
        public static TimeSpan Minutes(this int minutes)
        {
            return TimeSpan.FromMinutes(minutes);
        }

        [DebuggerStepThrough]
        public static TimeSpan Hours(this int hours)
        {
            return TimeSpan.FromHours(hours);
        }

        [DebuggerStepThrough]
        public static int? TryParseInt32(this string s, int? @default = null, NumberStyles style = NumberStyles.Any, IFormatProvider provider = null)
        {
            int value;
            if (Int32.TryParse(s, style, provider, out value))
            {
                return value;
            }

            return @default;
        }

        [DebuggerStepThrough]
        public static short? TryParseInt16(this string s, short? @default = null, NumberStyles style = NumberStyles.Any, IFormatProvider provider = null)
        {
            short value;
            if (Int16.TryParse(s, style, provider, out value))
            {
                return value;
            }

            return @default;
        }

        [DebuggerStepThrough]
        public static decimal? TryParseDecimal(this string s, decimal? @default = null, NumberStyles style = NumberStyles.Any, IFormatProvider provider = null)
        {
            decimal value;
            if (Decimal.TryParse(s, style, provider, out value))
            {
                return value;
            }

            return @default;
        }

        public static decimal? TryParseDecimalStrip(this string s, decimal? @default = null, NumberStyles style = NumberStyles.Any, IFormatProvider provider = null)
        {
            return s.StripNonNumeric().TryParseDecimal(@default, style, provider);
        }

        [DebuggerStepThrough]
        public static DateTime? TryParseDateTime(this string s, DateTime? @default = null, DateTimeStyles style = DateTimeStyles.None, IFormatProvider provider = null)
        {
            DateTime value;
            if (DateTime.TryParse(s, provider, style, out value))
            {
                return value;
            }

            return @default;
        }

        [DebuggerStepThrough]
        public static DateTime? TryParseExactDateTime(this string s, string format, DateTime? @default = null, DateTimeStyles style = DateTimeStyles.None, IFormatProvider provider = null)
        {
            DateTime value;
            if (DateTime.TryParseExact(s, format, provider, style, out value))
            {
                return value;
            }

            return @default;
        }

        public static string SplitCamelCase(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return "";
            string tempStr = Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
            if (Char.IsLower(tempStr[0]) && tempStr.Length > 1)
                tempStr = Char.ToUpper(tempStr[0]) + tempStr.Substring(1);

            return tempStr;
        }

        public static string FirstCaps(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            var sb = new StringBuilder();

            var previousWasSpace = true;
            foreach (var c in s)
            {
                if (char.IsWhiteSpace(c))
                {
                    sb.Append(c);
                    previousWasSpace = true;
                    continue;
                }

                sb.Append(previousWasSpace ? char.ToUpper(c) : char.ToLower(c));
                previousWasSpace = false;
            }
            return sb.ToString();
        }

        //public static string FirstCaps(this string str)
        //{
        //    if (str == string.Empty)
        //        return string.Empty;

        //    char[] newstr;
        //    string strOut = String.Empty;
        //    if (str.Contains(" "))
        //    {
        //        string[] words = str.Split(' ');
        //        foreach (var word in words)
        //        {
        //            if (word != string.Empty)
        //            {
        //                newstr = word.ToLower().ToCharArray();
        //                newstr[0] = char.ToUpper(newstr[0]);
        //                strOut = strOut + new string (newstr) + " ";
        //            }
        //        }
        //    }
        //    else
        //    {
        //        newstr = str.ToLower().ToCharArray();
        //        newstr[0] = char.ToUpper(newstr[0]);
        //        strOut = new string(newstr);
        //    }
        //    return strOut.Trim();
        //}

        //public static string FirstCaps(this string str)
        //{
        //    string pattern = @"\b(\w|['-])+\b";
        //    var x =  Regex.Replace(str, pattern, m => m.Value[0].ToString().ToUpper() + m.Value.Substring(1));
        //    return x;
        //}
       
        public static DataSet ToDataSet(this IDataReader reader)
        {
            DataSet dataSet = new DataSet();

            do
            {
                DataTable schemaTable = reader.GetSchemaTable();
                DataTable dataTable = new DataTable();

                if (schemaTable != null)
                {
                    // A query returning records was executed

                    for (int i = 0; i < schemaTable.Rows.Count; i++)
                    {
                        var dataRow = schemaTable.Rows[i];
                        dataTable.Columns.Add(new DataColumn
                        {
                            ColumnName = Convert.ToString(dataRow["ColumnName"]),
                            DataType = (Type)dataRow["DataType"]
                        });
                    }

                    dataSet.Tables.Add(dataTable);

                    // Fill the data table we just created
                    while (reader.Read())
                    {
                        DataRow dataRow = dataTable.NewRow();

                        for (int i = 0; i < reader.FieldCount; i++)
                            dataRow[i] = reader.GetValue(i);

                        dataTable.Rows.Add(dataRow);
                    }
                }
                else
                {
                    // No records were returned

                    DataColumn column = new DataColumn("RowsAffected");
                    dataTable.Columns.Add(column);
                    dataSet.Tables.Add(dataTable);
                    DataRow dataRow = dataTable.NewRow();
                    dataRow[0] = reader.RecordsAffected;
                    dataTable.Rows.Add(dataRow);
                }
            }
            while (reader.NextResult());

            return dataSet;
        }

        public static TOut MapTo<TIn, TOut>(this TIn tin) where TOut : new()       
        {
            if (tin == null)
                throw new ArgumentNullException("tin");
            PropertyInfo[] propsIn = typeof(TIn).GetProperties();
            PropertyInfo[] propsOut = typeof(TOut).GetProperties();
            
            var tout = new TOut();

            foreach (PropertyInfo propIn in propsIn)
            {
                if (propIn.CanRead)
                {
                    foreach (var propOut in propsOut)
	                {
		                if(propIn.Name == propOut.Name && propIn.PropertyType.Name == propOut.PropertyType.Name &&  propOut.CanWrite)
                        {
                            propOut.SetValue(tout, propIn.GetValue(tin, null), null);
                            break;
                        }
	                }
                }
            }

            return tout;
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> collection, string tableName = null)
        {
            var dt = new DataTable(tableName ?? typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                dt.Columns.Add(prop.Name, t);
            }

            foreach (T item in collection)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                dt.Rows.Add(values);
            }

            return dt;
        }

        public static bool IsNullable(this Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static Type GetCoreType(this Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }

        //Suitable for simple behaviourless DTO/POCO objects.    ---todo: Could Clone() do the work?
        public static void CopyPropertiesTo<T>(this T source, T target) where T : class
        {
            foreach (PropertyInfo pi in target.GetType().GetProperties())
            {
                if (pi.CanWrite && pi.CanRead)
                {
                    pi.SetValue(target, pi.GetValue(source, null), null);
                }
            }
        }

        public static void AddWithDefaultValue<T>(this DataColumnCollection dcCollection, string columnName,
                                                    T defaultValue, bool allowDBNull = true)
        {
            var dc = new DataColumn(columnName, typeof(T)) { AllowDBNull = allowDBNull, DefaultValue = defaultValue };
            dcCollection.Add(dc);
        }

        //similar to javascript slice function, but doesn't handle negative values
        public static string Slice(this string str, int index1, int index2)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            if (index1 < 0 || index1 > str.Length)
                throw new ArgumentOutOfRangeException("index1");

            if (index2 < 0 || index2 > str.Length)
                throw new ArgumentOutOfRangeException("index2");

            return str.Substring(Math.Min(index1, index2), Math.Abs(index2 - index1));
        }

        public static int TotalDays(this DateTime dt)
        {
            TimeSpan ts = dt - new DateTime(0);
            return ts.Days;
        }

        public static string StripNonNumeric(this string text)
        {
            return Regex.Replace(text, @"[^0-9.]", "");
        }

        public static DateTime RoundToDay(this DateTime dt)
        {
            if (dt.Hour > 12)
                dt = dt.AddDays(1);
            return new DateTime(dt.Year, dt.Month, dt.Day);
        }


        public static string DisplayCurrenyPrint(this decimal amount, string places )
        {
            return string.Format("{0} {1}", Math.Abs(amount).ToString(places), amount < 0 ? "CR" : "&nbsp&nbsp");
        }

        public static short? ConvertToBranchNo(this string account)
        {
            short branch;
            if (short.TryParse(account.Substring(0, 3), out branch))
                return branch;
            else
                return null;

        }
    }    
}
