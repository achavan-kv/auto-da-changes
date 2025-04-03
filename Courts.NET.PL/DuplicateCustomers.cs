using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace STL.PL
{
    public partial class DuplicateCustomers : CommonForm
    {

        private DataSet duplicateCustomers = null;

        public DuplicateCustomers()
        {
            InitializeComponent();
        }


        private DataSet GetDuplicateCustomers()
        {
            return CustomerManager.GetDuplicateCustomers();
        }

        private void dgDuplicateCustomers_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == 2)//Checkbox column
            {

                DataRowView dr = ((DataView)(dgDuplicateCustomers.DataSource))[dgDuplicateCustomers.CurrentRow.Index];

                var resolved = Convert.ToBoolean(dr["Resolved / Unresolved"]);
                var custid = Convert.ToString(dr["Potential Duplicate A"]);
                var duplicateCustid = Convert.ToString(dr["Potential Duplicate B"]);

                CustomerManager.UpdateDuplicateCustomers(custid, duplicateCustid, resolved);

                setFilter(); //Call to refresh the grid        

            }
        }

        private void dgDuplicateCustomers_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgDuplicateCustomers.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            setFilter();
        }

        private void setFilter()
        {

            if (cmbFilter.SelectedIndex != -1 && dgDuplicateCustomers.DataSource != null)
            {
                if (cmbFilter.SelectedItem.ToString() == "Resolved")
                {
                    ((DataView)dgDuplicateCustomers.DataSource).RowFilter = "[Resolved / Unresolved] = 'true'";
                }
                else if (cmbFilter.SelectedItem.ToString() == "Unresolved")
                {
                    ((DataView)dgDuplicateCustomers.DataSource).RowFilter = "[Resolved / Unresolved] = 'false'";
                }
                else
                {
                    ((DataView)dgDuplicateCustomers.DataSource).RowFilter = "";
                }
            }

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            ((MainForm)this.FormRoot).statusBar1.Text = "Loading customers...";

            duplicateCustomers = GetDuplicateCustomers();

            DataView dv = new DataView(duplicateCustomers.Tables[0]);

            dgDuplicateCustomers.DataSource = dv;

            ((MainForm)this.FormRoot).statusBar1.Text = "";

            dgDuplicateCustomers.Columns["Potential Duplicate A"].ReadOnly = true;
            dgDuplicateCustomers.Columns["Potential Duplicate B"].ReadOnly = true;

            cmbFilter.Enabled = true;
            btnLoad.Enabled = false;

        }


    }
}
