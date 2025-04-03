using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blue.Cosacs.Shared;

namespace Blue.Cosacs
{
    public static class EnumExtensions
    {
        //private static IEnumerable<R> AsEnumerable<E,R>(E e)
        //{
        //    foreach (var value in Enum.GetValues(typeof(E)).Cast<E>())
        //    {
        //        if ((e & value) == value)
        //            yield return (R)value;
        //    }
        //}

        public static List<String> ToStringList(this InstStatus? status)
        {
            var list = new List<String>();

            if (!status.HasValue)
                return list;

            foreach (var value in Enum.GetValues(typeof(InstStatus)).Cast<InstStatus>())
                if ((status.Value & value) == value)
                    list.Add(value.ToString());

            return list;
        }

        public static double WeightedAverage<T>(this IEnumerable<T> records, Func<T, double> value, Func<T, double> weight)
        {
            double weightedValueSum = records.Sum(record => value(record) * weight(record));
            double weightSum = records.Sum(record => weight(record));

            return weightedValueSum / weightSum;
        }
    }
}
