using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Windows.Forms;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using STL.PL.Utils;



namespace STL.PL
{
    public partial class CommissionsReport : CommonForm
    {
        public string GridDetailLabel;
        public int employeeNo;
        private bool _staticLoaded = false;

        public CommissionsReport()
        {
            InitializeComponent();
            dtpCommDateFrom.Value = DateTime.Today.AddMonths(-1);
            dtpCommDateTo.Value = DateTime.Today.AddDays(1).AddSeconds(-1);
            GridDetailLabel = lblDetailLabel.Text;
        }

        public CommissionsReport(TranslationDummy d)
        {
            InitializeComponent();
        }

        public CommissionsReport(Form root, Form parent)
            : this()
        {
            //InitializeComponent();

            FormRoot = root;
            FormParent = parent;

            HashMenus();
            ApplyRoleRestrictions();
            // Check role permissions        (jec 14/02/07) 
            //if (!lbRolePermission.Visible)          // only user permission
            //    employeeNo = Credential.UserId;       
            //else
            //    employeeNo = 0;                     // full permission - view all Commissions
            InitialiseStaticData();

        }

        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":lbRolePermission"] = this.lbRolePermission;
        }

        private void InitialiseStaticData()
        {
            try
            {
                Function = "CommissionEnquiry::InitialiseStaticData";
                /* initialise the collections */
                // Load Branches
                StringCollection branchNos = new StringCollection();
                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                {
                    branchNos.Add(Convert.ToString(row["branchno"]));
                }

                drpBranchNo.DataSource = branchNos;

                int x = drpBranchNo.FindString(Config.BranchCode);
                if (x != -1)
                    drpBranchNo.SelectedIndex = x;

                //LoadSaleStaff();

                if (Convert.ToInt32(Config.BranchCode) == (decimal)Country[CountryParameterNames.HOBranchNo])
                    drpBranchNo.Enabled = true;
                else
                    drpBranchNo.Enabled = false;

                _staticLoaded = true;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            Function = "btnLoad_Click()";
            try
            {
                Wait();
                ((MainForm)this.FormRoot).statusBar1.Text = "Loading Commissions....";
                // Check role permissions        (jec 14/02/07)  
                if (!lbRolePermission.Enabled)          // only user permission
                    employeeNo = Credential.UserId;
                else
                    employeeNo = 0;                     // full permission - view all Commissions

                this.LoadCommissionsHeader();
                this.LoadCommissionsDetail();


            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                ((MainForm)this.FormRoot).statusBar1.Text = "Done";
                //System.Threading.Thread.Sleep(5000);
                //((MainForm)this.FormRoot).statusBar1.Text = "";
            }
        }

        private void LoadCommissionsHeader()
        {

            DataSet dsHeader = PaymentManager.GetSalesCommissionReportHeader(Convert.ToInt16(drpBranchNo.SelectedValue), employeeNo,
                dtpCommDateFrom.Value, dtpCommDateTo.Value, chkStandardCommission.Checked, chkSPIFF.Checked, out Error);
            if (Error.Length > 0)
                ShowError(Error);

            dgvCommissionsHeader.DataSource = dsHeader;
            dgvCommissionsHeader.DataMember = TN.SalesCommission;

            ReportUtils.ApplyGridHeadings(dgvCommissionsHeader, this);
            dgvCommissionsHeader.Columns[CN.EmployeeNo].Width = 56;

            dgvCommissionsHeader.Columns[CN.RebateTotal].Width = 73;
            dgvCommissionsHeader.Columns[CN.RebateTotal].DefaultCellStyle.Format = DecimalPlaces;
            dgvCommissionsHeader.Columns[CN.RebateTotal].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvCommissionsHeader.Columns[CN.CommissionTotal].Width = 73;
            dgvCommissionsHeader.Columns[CN.CommissionTotal].DefaultCellStyle.Format = DecimalPlaces;
            dgvCommissionsHeader.Columns[CN.CommissionTotal].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvCommissionsHeader.Columns[CN.RepossessionTotal].Width = 73;
            dgvCommissionsHeader.Columns[CN.RepossessionTotal].DefaultCellStyle.Format = DecimalPlaces;
            dgvCommissionsHeader.Columns[CN.RepossessionTotal].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvCommissionsHeader.Columns[CN.SPIFFTotal].Width = 73;
            dgvCommissionsHeader.Columns[CN.SPIFFTotal].DefaultCellStyle.Format = DecimalPlaces;
            dgvCommissionsHeader.Columns[CN.SPIFFTotal].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvCommissionsHeader.Columns[CN.CancellationTotal].Width = 73;
            dgvCommissionsHeader.Columns[CN.CancellationTotal].DefaultCellStyle.Format = DecimalPlaces;
            dgvCommissionsHeader.Columns[CN.CancellationTotal].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvCommissionsHeader.Columns[CN.DeliveryTotal].Width = 73;      //jec 03/07/08 UAT442
            dgvCommissionsHeader.Columns[CN.DeliveryTotal].DefaultCellStyle.Format = DecimalPlaces;
            dgvCommissionsHeader.Columns[CN.DeliveryTotal].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvCommissionsHeader.Columns[CN.CommissionPercent].Width = 73;
            dgvCommissionsHeader.Columns[CN.CommissionPercent].DefaultCellStyle.Format = "P2";
            dgvCommissionsHeader.Columns[CN.CommissionPercent].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCommissionsHeader.Columns[CN.EmployeeName].Width = 140;

            dgvCommissionsHeader.Columns[CN.CancellationTotal].Visible = chkStandardCommission.Checked;
            dgvCommissionsHeader.Columns[CN.RepossessionTotal].Visible = chkStandardCommission.Checked;
            dgvCommissionsHeader.Columns[CN.RebateTotal].Visible = chkStandardCommission.Checked;
            dgvCommissionsHeader.Columns[CN.CommissionTotal].Visible = chkStandardCommission.Checked;
            dgvCommissionsHeader.Columns[CN.SPIFFTotal].Visible = chkSPIFF.Checked;

            btnExcelHeader.Enabled = (dgvCommissionsHeader.Rows.Count > 0);
        }

        private void LoadCommissionsDetail()
        {
            lblDetailLabel.Text = GridDetailLabel;
            if (dgvCommissionsHeader.Rows.Count > 0)
            {
                int empeeNo = Convert.ToInt32(dgvCommissionsHeader.CurrentRow.Cells[CN.EmployeeNo].Value);
                DataSet dsDetail = PaymentManager.GetSalesCommissionReportDetail(empeeNo, dtpCommDateFrom.Value,
                    dtpCommDateTo.Value, chkStandardCommission.Checked, chkSPIFF.Checked, out Error);

                if (Error.Length > 0)
                    ShowError(Error);

                dgvCommissionsDetail.DataSource = dsDetail;
                dgvCommissionsDetail.DataMember = TN.SalesCommission;

                ReportUtils.ApplyGridHeadings(dgvCommissionsDetail, this);
                dgvCommissionsDetail.Columns[CN.RunDate].Width = 98;
                dgvCommissionsDetail.Columns[CN.AccountNumber].Width = 82;
                dgvCommissionsDetail.Columns[CN.InvoiceNo].Width = 45;
                dgvCommissionsDetail.Columns[CN.CommissionType].Width = 80;
                dgvCommissionsDetail.Columns[CN.ItemNo].Width = 60;
                dgvCommissionsDetail.Columns[CN.StockLocn].Width = 40;
                dgvCommissionsDetail.Columns[CN.CommissionAmount].Width = 70;
                dgvCommissionsDetail.Columns[CN.CommissionAmount].DefaultCellStyle.Format = DecimalPlaces;
                dgvCommissionsDetail.Columns[CN.CommissionAmount].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCommissionsDetail.Columns[CN.DeliveryAmount].Width = 71;
                dgvCommissionsDetail.Columns[CN.DeliveryAmount].DefaultCellStyle.Format = DecimalPlaces;
                dgvCommissionsDetail.Columns[CN.DeliveryAmount].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCommissionsDetail.Columns[CN.CommissionPercent].Width = 65;
                dgvCommissionsDetail.Columns[CN.CommissionPercent].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCommissionsDetail.Columns[CN.CommissionPercent].DefaultCellStyle.Format = "P2";
                dgvCommissionsDetail.Columns[CN.Uplift_Commission_pcRate].Width = 65;
                dgvCommissionsDetail.Columns[CN.Uplift_Commission_pcRate].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCommissionsDetail.Columns[CN.Uplift_Commission_pcRate].DefaultCellStyle.Format = "P0";

                dgvCommissionsDetail.Columns[CN.TransTypeCode].Width = 35;


                btnExcelDetail.Enabled = (dgvCommissionsDetail.Rows.Count > 0);
                //GridDetailLabel
                lblDetailLabel.Text = String.Concat(GridDetailLabel, " - ", dgvCommissionsHeader.CurrentRow.Cells[CN.EmployeeName].Value.ToString());
            }
            else
                dgvCommissionsDetail.DataSource = null;
        }

        private void btnExcelHeader_Click(object sender, EventArgs e)
        {
            string path = ReportUtils.CreateCSVFile(dgvCommissionsHeader);

            if (path.Length.Equals(0))
                MessageBox.Show("Save Failed");

            try
            {
                ReportUtils.OpenExcelCSV(path);
            }
            catch { }
        }

        //private void dgvCommissionsHeader_RowEnter(object sender, DataGridViewCellEventArgs e)
        //{

        //}

        private void dgvCommissionsHeader_MouseClick(object sender, MouseEventArgs e)
        {
            if (dgvCommissionsHeader.CurrentRow != null)
            {
                int index = dgvCommissionsHeader.CurrentRow.Index;
                if (index >= 0)
                {
                    this.Wait();
                    this.LoadCommissionsDetail();
                    ((MainForm)this.FormRoot).statusBar1.Text = "Loading Details....";

                    this.StopWait();
                    ((MainForm)this.FormRoot).statusBar1.Text = "Done";
                }
            }
        }

        private void btnExcelDetail_Click(object sender, EventArgs e)
        {
            string path = ReportUtils.CreateCSVFile(dgvCommissionsDetail);

            if (path.Length.Equals(0))
                MessageBox.Show("Save Failed");

            try
            {
                ReportUtils.OpenExcelCSV(path);
            }
            catch { }
        }

        private void dgvCommissionsHeader_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void drpBranchNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_staticLoaded)
                btnLoad_Click(null, null);

        }
    }
}