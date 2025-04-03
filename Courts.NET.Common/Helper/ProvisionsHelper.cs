using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace STL.Common
{
    public static class ProvisionsConvert
    {
        public static DataTable ToDataTable(List<ProvisionsItem> items)
        {
            DataTable table = new DataTable();


            table.Columns.Add("Acctype", typeof(char));
            table.Columns.Add("StatusName", typeof(string));
            table.Columns.Add("StatusUpper", typeof(Int32));
            table.Columns.Add("StatusLower", typeof(Int32));
            table.Columns.Add("MonthsName", typeof(string));
            table.Columns.Add("MonthsUpper", typeof(Int32));
            table.Columns.Add("MonthsLower", typeof(Int32));
            table.Columns.Add("Provision", typeof(decimal));

            foreach (var item in items)
            {
                var newrow = table.NewRow();
                newrow["Acctype"] = item.Acctype;
                newrow["StatusName"] = item.StatusName;
                newrow["StatusUpper"] = item.StatusUpper;
                newrow["StatusLower"] = item.StatusLower;
                newrow["MonthsName"] = item.MonthsName;
                newrow["MonthsUpper"] = item.MonthsUpper;
                newrow["MonthsLower"] = item.MonthsLower;
                newrow["Provision"] = item.Provision;
                table.Rows.Add(newrow);
            }

            return table;
        }
    }
}