using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blue.Cosacs.Merchandising.Helpers
{
    using FileHelpers;

    public static class FileHelperExtensions
    {
        private const BindingFlags Bindings = BindingFlags.GetField | BindingFlags.Public | BindingFlags.Instance;

        public static IEnumerable<string> GetFieldTitles(this Type type)
        {
            var fields = type.GetFields(Bindings).Where(f => f.IsFileHelpersField())
                .Select(f => f.GetCustomAttributes(true).OfType<FieldTitleAttribute>().Single().Name ?? f.Name);
            
            var props = type.GetProperties(Bindings).Where(p => p.IsFileHelpersProperty())
                .Select(f => f.GetCustomAttributes(true).OfType<FieldTitleAttribute>().Single().Name ?? f.Name);

            return fields.Union(props);
        }
 
        public static string GetCsvHeader(this Type type)
        {
            var customDelimeterAttr = type.GetCustomAttributes(true).OfType<DelimitedRecordAttribute>().SingleOrDefault();
            var delim = ",";

            // Reflect on sealed attribute to find the delimeter value
            if (customDelimeterAttr != null)
            {
                var sepField = typeof(DelimitedRecordAttribute).GetField("Separator", BindingFlags.NonPublic | BindingFlags.Instance);
                if (sepField != null)
                {
                    delim = (string)sepField.GetValue(customDelimeterAttr);
                }
            }
            
            var titles = type.GetFieldTitles().ToList();
            return string.Join(delim, titles);
        }
 
        public static bool IsFileHelpersField(this FieldInfo field)
        {
            return field.GetCustomAttributes(true)
                .OfType<FieldTitleAttribute>()
                .Any();
        }

        public static bool IsFileHelpersProperty(this PropertyInfo info)
        {
            return info.GetCustomAttributes(true)
                .OfType<FieldTitleAttribute>()
                .Any();
        }
    }
}
