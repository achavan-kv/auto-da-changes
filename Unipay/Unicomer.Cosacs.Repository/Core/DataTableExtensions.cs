using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Unicomer.Cosacs.Repository.Core
{
    public static class DataTableExtensions
    {
        public static DataTable ToDataTable<T>(List<T> items, string typeName = "")
        {
            if(string.IsNullOrWhiteSpace(typeName))
            {
                typeName = typeof(T).Name;
            }

            DataTable dataTable = new DataTable(typeName);

            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                dataTable.Columns.Add(prop.Name, type);
            }

            if (items != null)
            {
                foreach (T item in items)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }
            }

            return dataTable;
        }
    }
}
