using System;
using System.Collections.Generic;
using System.Text;

namespace BBSL.Libraries.General
{
    public static class Extensions
    {
        public static List<T> RemoveDuplicates<T>(this List<T> list) where T : class, IComparable<T>
        {
            list.Sort();
            Int32 index = 0;
            while (index < list.Count - 1)
            {
                if (list[index] == list[index + 1])
                    list.RemoveAt(index);
                else
                    index++;
            }

            return list;
        }

        public static bool AreAllTrue(this bool[] values)
        {
            foreach (bool value in values)
                if (!value)
                    return false;

            return true;
        }

        public static bool OneIsTrue(this bool[] values)
        {
            foreach (bool value in values)
                if (value)
                    return true;

            return false;
        }

        /// <summary>
        /// Tries to convert the object and returns the value if successful, else zero will be returned
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>returns the converted value if successful, else zero will be returned</returns>
        public static decimal ToDecimal(this object obj)
        {
            decimal value = 0;
            decimal.TryParse(obj.ToString(), out value);
            return value;
        }
        /// <summary>
        /// Tries to convert the object and returns the value if successful, else zero will be returned
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>returns the converted value if successful, else zero will be returned</returns>
        public static int ToInt32(this object obj)
        {
            int value = 0;
            int.TryParse(obj.ToString(), out value);
            return value;
        }

        public static decimal ToPositive(this decimal value)
        {
            if (value < 0)
                value = value * -1;
            return value;
        }

        public static string ToCommandLineArg(this string str, string arg)
        {
            return " " + str.Trim() + "=" + arg.Replace(' ', '%');
        }

        public static string SafeSubstring(this string str, int startIndex, int length)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            length = Math.Min(str.Length - startIndex, length);
            return str.Substring(startIndex, length);
        }
    }
}
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute
    {
        public ExtensionAttribute() { }
    }
}
