using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using STL.Common.Static;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using Crownwood.Magic.Menus;

namespace STL.PL
{
    public partial class SalesCommissionBranchEnquiry : CommonForm
    {
        private bool _staticLoaded = false;
        private int? branchNo = null;
        private string employee = string.Empty;

        public SalesCommissionBranchEnquiry(Form root, Form parent)
        {
            FormRoot = root;
            FormParent = parent;

            InitializeComponent();
            InitialiseStaticData();
        }

        private void InitialiseStaticData()
        {
            try
            {
                // Load Branches
                StringCollection branchNos = new StringCollection();
                if (drpBranchNo.Enabled == true)
                {
                    branchNos.Add("All");
                }

                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                {
                    branchNos.Add(Convert.ToString(row["branchno"]));
                }

                drpBranchNo.DataSource = branchNos;

                int x = drpBranchNo.FindString(Config.BranchCode);
                if (x != -1)
                    drpBranchNo.SelectedIndex = x;

                _staticLoaded = true;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // exit screen
            CloseTab();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ((MainForm)this.FormRoot).statusBar1.Text = "Loading results...";

            dgvCommissions.DataSource = null;
            lblTotCommissionableValue.Text = string.Empty;
            lblTotProductCommission.Text = string.Empty;
            lblTotTermsTypeCommission.Text = string.Empty;
            lblTotWarrantyCommission.Text = string.Empty;
            lblTotal.Text = string.Empty;
            lblTotalCommission.Text = string.Empty;

            STL.PL.WS2.SalesCommissionDetails details = null;

            details = AccountManager.GetBranchSalesCommissionDetails(branchNo, dtpDeliveryDateFrom.Value, dtpDeliveryDateTo.Value);

            DataView dtcommissionDetails = new DataView(details.SalesCommissions);

            dgvCommissions.DataSource = dtcommissionDetails;

            dgvCommissions.ColumnHeadersVisible = true;
            dgvCommissions.AutoGenerateColumns = true;

            dgvCommissions.Columns["EmployeeIdAndName"].Visible = false;
            dgvCommissions.Columns[CN.Employee].Width = 150;
            
            lblTotCommissionableValue.Text = details.TotalCommissionableValue.Value.ToString(DecimalPlaces);
            lblTotProductCommission.Text = details.TotalProductCommissionValue.Value.ToString(DecimalPlaces);
            lblTotTermsTypeCommission.Text = details.TotalTermsTypesCommissionValue.Value.ToString(DecimalPlaces);
            lblTotWarrantyCommission.Text = details.TotalWarrantyCommissionValue.Value.ToString(DecimalPlaces);
            lblTotal.Text = details.TotalCommission.Value.ToString(DecimalPlaces);
            lblTotalCommission.Text = details.TotalCommission.Value.ToString(DecimalPlaces);

            ((MainForm)this.FormRoot).statusBar1.Text = string.Empty;

        }

        private void drpBranchNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.branchNo = drpBranchNo.SelectedValue == "All" ? null : GetNullableInt(drpBranchNo.SelectedValue);
        }

        private int? GetNullableInt(object intObj)
        {
            if (intObj != null) {
                var retInt = -1;
                if (int.TryParse(intObj.ToString(), out retInt))
                    return (int?)retInt;
                else
                    return null;                
            }
            return null;
        }

        private void dgvCommissions_MouseUp(object sender, MouseEventArgs e)
        {
            if (dgvCommissions.CurrentRow.Index >= 0)
            {
                if (e.Button == MouseButtons.Right)
                {
                    DataGridView ctl = (DataGridView)sender;

                    employee = dgvCommissions["EmployeeIdAndName", dgvCommissions.CurrentRow.Index].Value.ToString();

                    MenuCommand m1 = new MenuCommand(GetResource("P_VIEW_COMMISSIONS"));

                    m1.Click += new System.EventHandler(this.viewCommissions_Click);

                    PopupMenu popup = new PopupMenu();
                    popup.MenuCommands.Add(m1);

                    MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                }
            }
        }

        private void viewCommissions_Click(object sender, System.EventArgs e)
        {
            SalesCommissionEnquiry enquiry = new SalesCommissionEnquiry(FormRoot, FormParent, this.branchNo, this.employee, dtpDeliveryDateFrom.Value, dtpDeliveryDateTo.Value);
            ((MainForm)FormRoot).AddTabPage(enquiry, 24);
        }

        private void btnTransactionDetailsExcel_Click(object sender, EventArgs e)
        {
            SaveExcel(dgvCommissions);
        }

        private void SaveExcel(DataGridView dg)
        {
            string filePath = STL.PL.Utils.ReportUtils.CreateCSVFile(dg, "Save Report to Excel");

            if (filePath.Length.Equals(0))
                MessageBox.Show("Save Failed");

            try
            {
                STL.PL.Utils.ReportUtils.OpenExcelCSV(filePath);
            }
            catch { }
        }
    }
}
