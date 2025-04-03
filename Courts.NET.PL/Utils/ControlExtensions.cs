using System;
using System.Windows.Forms;
using Blue.Cosacs.Shared.Extensions;
using System.Collections.Generic;
using Blue.Cosacs.Shared.Collections;
using System.Data;
using System.ComponentModel;
using Blue.Cosacs.Shared;

namespace STL.PL.Utils
{
    public static class ControlExtensions
    {
        [Obsolete("Call ColumnStyleStart() and then for each visible column call ColumnStyle() method")]
        public static void ApplyColumnStyle(this DataGridView dgv, params Tuple<string, string>[] mappings)
        {
            dgv.AutoGenerateColumns = false;

            foreach (DataGridViewColumn column in dgv.Columns)
                column.Visible = false;

            var i = 0;
            foreach (var mapping in mappings)
                dgv.ApplyColumnStyle(mapping.Item1, mapping.Item2, i++);
        }

        [Obsolete("Call ColumnStyleStart() and then for each visible column call ColumnStyle() method")]
        public static void ApplyColumnStyle(this DataGridView dgv, params Tuple<string, string, int>[] mappings)
        {
            dgv.AutoGenerateColumns = false;

            foreach (DataGridViewColumn column in dgv.Columns)
                column.Visible = false;

            var i = 0;
            foreach (var mapping in mappings)
                dgv.ApplyColumnStyle(mapping.Item1, mapping.Item2, i++, mapping.Item3);
        }

        [Obsolete("Call ColumnStyleStart() and then for each visible column call ColumnStyle() method")]
        public static void ApplyColumnStyle(this DataGridView dgv, params Tuple<string, string, int, DataGridViewCellStyle>[] mappings)
        {
            dgv.AutoGenerateColumns = false;

            foreach (DataGridViewColumn column in dgv.Columns)
                column.Visible = false;

            var i = 0;
            foreach (var mapping in mappings)
                dgv.ApplyColumnStyle(mapping.Item1, mapping.Item2, i++, mapping.Item3, mapping.Item4);
        }

        [Obsolete("Call ColumnStyle() method")]
        private static void ApplyColumnStyle(this DataGridView dgv, string columnName, string headerText, int displayIndex,
                                                int? width = null, DataGridViewCellStyle cellStyle = null)
        {
            var column = dgv.Columns[columnName];
            column.HeaderText = headerText ?? columnName.SplitCamelCase();
            column.Visible = true;
            column.SortMode = DataGridViewColumnSortMode.Automatic;
            column.DisplayIndex = displayIndex;

            if (width.HasValue)
                column.Width = width.Value;

            if (cellStyle != null)
                column.DefaultCellStyle = cellStyle;
        }

        public static DataGridView ColumnStyleInit(this DataGridView dgv)
        {
            dgv.AutoGenerateColumns = false;

            foreach (DataGridViewColumn column in dgv.Columns)
                column.Visible = false;

            return dgv;
        }
        
        public static DataGridView ColumnStyle(this DataGridView dgv, string columnName, string headerText = null, 
                                                int? width = null, DataGridViewCellStyle cellStyle = null,
                                                DataGridViewColumnSortMode sortMode = DataGridViewColumnSortMode.Automatic)
        {
            var column = dgv.Columns[columnName];
            if (column == null)
                throw new InvalidOperationException("There is no column named: " + columnName);

            var displayIndex = dgv.Columns.GetColumnCount(DataGridViewElementStates.Visible);
            
            column.DisplayIndex = displayIndex;
            column.HeaderText = headerText ?? columnName.SplitCamelCase();
            column.SortMode = sortMode;
            column.Visible = true;

            if (width.HasValue)
                column.Width = width.Value;

            if (cellStyle != null)
                column.DefaultCellStyle = cellStyle;

            return dgv;
        }

        public static DataGridView ColumnStylePreLoad(this DataGridView dgv, 
                                                      string DataPropertyName, 
                                                      string headerText = null,
                                                      int? width = null, 
                                                      int? displayIndex = null, 
                                                      DataGridViewColumnSortMode sortMode = DataGridViewColumnSortMode.Automatic, 
                                                      DataGridViewContentAlignment align = DataGridViewContentAlignment.MiddleLeft,
                                                      string DefaultCellStyleFormat = null,
                                                      bool readOnly = false)
        {
            var column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = DataPropertyName;
            column.Name = DataPropertyName;
            
            column.HeaderText = headerText ?? DataPropertyName.SplitCamelCase();
            column.SortMode = sortMode;
            column.DefaultCellStyle.Alignment = align;
            column.Visible = true;
            column.DefaultCellStyle.Format = DefaultCellStyleFormat ?? string.Empty;
            column.ReadOnly = readOnly;
            if (displayIndex.HasValue)
                column.DisplayIndex = displayIndex.Value;
            if (width.HasValue)
                column.Width = width.Value;

                dgv.Columns.Add(column);

            
            return dgv;
        }

        public static bool TrySetValue(this DateTimePicker dt, DateTime tryValue, DateTime? alternativeValue = null)
        {
            if (tryValue >= dt.MinDate && tryValue <= dt.MaxDate)
            {
                dt.Value = tryValue;
                return true;
            }

            if (alternativeValue.HasValue)
                dt.TrySetValue(alternativeValue.Value);

            return false;
        }

        public static void ForceSetValue(this DateTimePicker dt, DateTime value)
        {
            if (value < dt.MinDate)
                dt.MinDate = value;
            else if (value > dt.MaxDate)
                dt.MaxDate = value;

            dt.Value = value;
        }

        public static string TwoDecimalPlaces(string value)
        {
            return String.Format("{0:0.00}", value);
        }

        // Needed to update datagrids on webserver response.
        public static void DataSourceSafe<T>(this DataGridView grid, IList<T> list)
        {
            if (list != null && list.Count > 0)
            {
                if (grid.DataSource != null)
                {
                    var source = ((ThreadedBindingList<T>)grid.DataSource);
                    source.Clear();
                    source.Add(list);
                }
                else grid.DataSource = ThreadedBindingList.Create(list);
            }
            else grid.DataSource = null;
        }

        public static void Set(this ComboBox combo, object datasource, string displaymember, string valuemember = null)     // #9715
        {
            combo.DisplayMember = displaymember;
            combo.ValueMember = valuemember;
            combo.DataSource = datasource;
        }

        public static void SetStmtDates(this ComboBox combo, List<StoreCardStatement> datasource, string displaymember, string valuemember = null)     // #12360
        {
            DataTable dt = ConvertTo(datasource);

            combo.DisplayMember = displaymember;
            combo.ValueMember = valuemember;
            combo.DataSource = dt;
        }

        public static DataTable ConvertTo(IList<StoreCardStatement> genericList)       // #12360
        {
            //create DataTable Structure
            DataTable dataTable = CreateTable<StoreCardStatement>();
            Type entType = typeof(StoreCardStatement);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entType);
            //get the list item and add into the list
            foreach (StoreCardStatement item in genericList)
            {
                DataRow row = dataTable.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    if (prop.Name == "Id" || prop.Name == "DateFrom" || prop.Name == "DateTo")
                    {
                        row[prop.Name] = prop.GetValue(item);
                    }
                }
                row["FromTo"] = Convert.ToDateTime(row["DateFrom"]).ToShortDateString() + "  to " + Convert.ToDateTime(row["DateTo"]).ToShortDateString();
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

        public static DataTable CreateTable<T>()        // #12360
        {
            //T –> ClassName
            Type entType = typeof(T);
            //set the datatable name as class name
            DataTable dataTable = new DataTable(entType.Name);
            //get the property list
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entType);
            foreach (PropertyDescriptor prop in properties)
            {
                if (prop.Name == "Id" || prop.Name == "DateFrom" || prop.Name == "DateTo")
                {
                    //add property as column
                    dataTable.Columns.Add(prop.Name);
                }
            }
            dataTable.Columns.Add("FromTo");
            return dataTable;
        }


        //public static void Set<T>(this ComboBox combo, IList<T> datasource, string displaymember, string valuemember = null)
        //{
        //    combo.DisplayMember = displaymember;
        //    combo.ValueMember = valuemember;
        //    combo.DataSource = datasource;
        //}

        public static double? ToDouble(this TextBox txtbox)
        {
            double output;

            if (double.TryParse(txtbox.Text,out output))
                return output;
            else
                return null;
        }

        public static Control FindFocusedControl(this Control control)
        {
            ContainerControl container = control as ContainerControl;
            while (container != null)
            {
                control = container.ActiveControl;
                container = control as ContainerControl;
            }
            return control;
        }
      
    }
}

