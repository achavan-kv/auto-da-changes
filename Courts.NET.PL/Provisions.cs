using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.Common.Constants.ColumnNames;
using STL.Common;

namespace STL.PL
{
    public partial class Provisions : CommonForm
    {
        ComboItems[] cmbitem_status;
        ComboItems[] cmbitem_MonthsInArrears;

        MainForm mainform;

        public Provisions(Form main)
        {

            InitializeComponent();

            mainform = (MainForm)main;

            cmbitem_status = new ComboItems[]
            {
                new ComboItems() {DispName = "1-5", Ranges = new Range() {Lower = 1, Upper = 5}},
                new ComboItems() {DispName = "6",  Ranges = new Range() {Lower = 6, Upper = 6}},
                new ComboItems() {DispName = "7",  Ranges = new Range() {Lower = 7, Upper = 7}},
                new ComboItems() {DispName = "5-7",  Ranges = new Range() {Lower = 5, Upper = 7}}
            };

            cmbitem_MonthsInArrears = new ComboItems[]
            {
                new ComboItems() {DispName = "1 to 2", Ranges = new Range() {Lower = 1, Upper = 2}},
                new ComboItems() {DispName = "2 to 3",  Ranges = new Range() {Lower = 2, Upper = 3}},
                new ComboItems() {DispName = "3 to 4",  Ranges = new Range() {Lower = 3, Upper = 4}},
                new ComboItems() {DispName = "4 to 6",  Ranges = new Range() {Lower = 4, Upper = 5}},
                new ComboItems() {DispName = "6 to 12", Ranges = new Range() {Lower = 6, Upper = 12}},
                new ComboItems() {DispName = "> 12",  Ranges = new Range() {Lower = 12, Upper = int.MaxValue}},
                new ComboItems() {DispName = "ALL",  Ranges = new Range() {Lower = int.MinValue, Upper = int.MaxValue}},
            };

            DataGridViewComboBoxColumn cbc_status = new DataGridViewComboBoxColumn();
            DataGridViewComboBoxColumn cbc_months = new DataGridViewComboBoxColumn();
            DataGridViewTextBoxColumn col_pro = new DataGridViewTextBoxColumn();
            col_pro.Name = CN.Provision;
            cbc_status.Name = CN.Provision_status;
            cbc_months.Name = CN.Provision_Months;

            cbc_status.DataSource = cmbitem_status;
            cbc_months.DataSource = cmbitem_MonthsInArrears;

            cbc_months.DisplayMember = "DispName";
            cbc_months.ValueMember = "Ranges";

            cbc_status.DisplayMember = "DispName";
            cbc_status.ValueMember = "Ranges";

            var cbc_months_cash = (DataGridViewComboBoxColumn)cbc_months.Clone();
            var cbc_status_cash = (DataGridViewComboBoxColumn)cbc_status.Clone();
            var col_pro_cash = (DataGridViewTextBoxColumn)col_pro.Clone();


            dgv_credit.Columns.Add(cbc_status);
            dgv_credit.Columns.Add(cbc_months);
            dgv_credit.Columns.Add(col_pro);

            dgv_cash.Columns.Add(cbc_status_cash);
            dgv_cash.Columns.Add(cbc_months_cash);
            dgv_cash.Columns.Add(col_pro_cash);


            dgv_cash.Columns[0].HeaderText = CN.Provision_status;
            dgv_cash.Columns[1].HeaderText = CN.Provision_Months;
            dgv_cash.Columns[2].HeaderText = CN.Provision;

            LoadData();

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SetErrors(dgv_cash) && SetErrors(dgv_credit))
            {
                if (Save())
                {
                    mainform.statusBar1.Text = "Provisions information updated.";
                }
            }
            else
            {
                mainform.statusBar1.Text = "Please fix errors before saving provisions.";
            }

        }

        private bool Save()
        {
            bool ok = false;

            try
            {
                List<STL.PL.WS10.ProvisionsItem> ProvisionTab = new List<STL.PL.WS10.ProvisionsItem>();
                SaveGrid(dgv_cash, ref ProvisionTab, 'C');
                SaveGrid(dgv_credit, ref ProvisionTab, 'R');
                SetDataManager.SaveProvisions(ProvisionTab.ToArray());

                ok = true;
            }
            catch
            {

            }
            return ok;
        }


        private bool SetErrors(DataGridView dg)
        {
            bool valid = true;
            int count; 
            decimal dec;
            foreach (DataGridViewRow row in dg.Rows)
            {
                if (!row.IsNewRow)
                {
                    foreach (DataGridViewCell col in row.Cells)
                    {
                        if (col.Value == null)
                        {
                            col.ErrorText = "Value Missing. Please set or delete row";
                            valid = false;
                        }
                        else
                        {
                            col.ErrorText = "";
                        }
                    }

                    if (row.Cells[2].Value == null || (!decimal.TryParse(row.Cells[2].Value.ToString(), out dec) || dec > 100 || dec < 0))
                    {
                        row.Cells[2].ErrorText = "Please enter a valid percentage.";
                        valid = false;
                    }

                    count = 0;
                    foreach (DataGridViewRow row2 in dg.Rows)
                    {
                        if (row.Cells[0].Value == row2.Cells[0].Value && row.Cells[1].Value == row2.Cells[1].Value && !row2.IsNewRow)
                        {
                            count++;
                            if (count > 1)
                            {
                                row2.ErrorText = "Duplicate Entry. Please revise.";
                                valid = false;
                            }
                        }
                    }

                }


            }
            return valid;
        }


        private void LoadData()
        {

            dgv_cash.DataSource = null;
            dgv_credit.DataSource = null;

           

            List<STL.PL.WS10.ProvisionsItem> Prolist = new List<STL.PL.WS10.ProvisionsItem>();
            Prolist.AddRange(SetDataManager.LoadProvisions());
           
            foreach (var row in Prolist)
            {
                if (row.Acctype == 'C')
                {
                    SetupDgv(dgv_cash, row);
                }
                else
                {
                    SetupDgv(dgv_credit, row);
                }
            }
        }

        private void SetupDgv(DataGridView dgv, STL.PL.WS10.ProvisionsItem row)
        {
            var rownumber = dgv.Rows.Add();
            ((DataGridViewComboBoxCell)dgv.Rows[rownumber].Cells[0]).Value = cmbitem_status[FindIndex(cmbitem_status, row, true)].Ranges;
            ((DataGridViewComboBoxCell)dgv.Rows[rownumber].Cells[1]).Value = cmbitem_MonthsInArrears[FindIndex(cmbitem_MonthsInArrears, row, false)].Ranges;
            dgv.Rows[rownumber].Cells[2].Value = row.Provision;
        }

        private int FindIndex(ComboItems[] Clist, STL.PL.WS10.ProvisionsItem item, bool status)
        {
        
            int upper;
            int lower;
            string name;

            if (status)
            {
                upper = item.StatusUpper;
                lower = item.StatusLower;
                name = item.StatusName;
            }
            else
            {
                upper = item.MonthsUpper;
                lower = item.MonthsLower;
                name = item.MonthsName;
            }
          
                for (int i = 0; i < Clist.Length; i++)
                {
                    if (Clist[i].DispName == name && Clist[i].Ranges.Lower == lower && Clist[i].Ranges.Upper == upper)
                    {
                        return i;
                    }
                }
            return 0;
        }

        private void SaveGrid(DataGridView dgv, ref  List<STL.PL.WS10.ProvisionsItem> protab, char acctype)
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (!row.IsNewRow)
                {
                    var item = new STL.PL.WS10.ProvisionsItem();
                    item.Acctype = acctype;
                    item.StatusName = row.Cells[0].FormattedValue.ToString();
                    item.StatusLower = Convert.ToInt32(((Range)row.Cells[0].Value).Lower);
                    item.StatusUpper = Convert.ToInt32(((Range)row.Cells[0].Value).Upper);
                    item.MonthsName = row.Cells[1].FormattedValue.ToString();
                    item.MonthsLower = Convert.ToInt32(((Range)row.Cells[1].Value).Lower);
                    item.MonthsUpper = Convert.ToInt32(((Range)row.Cells[1].Value).Upper);
                    item.Provision = Convert.ToDecimal(row.Cells[2].Value);
                    protab.Add(item);
                }
            }
        }


       

      
    }


    public class ComboItems
    {
        private string __DispName = "";
        public string DispName
        {
            get { return __DispName; }
            set { __DispName = value; }
        }

        private Range _ranges;
        public Range Ranges
        {
            get { return _ranges; }
            set { _ranges = value; }
        }
    }

    public class Range
    {
        private int _lower;
        public int Lower
        {
            get { return _lower; }
            set { _lower = value; }
        }

        private int _upper;
        public int Upper
        {
            get { return _upper; }
            set { _upper = value; }
        }
    }
}
