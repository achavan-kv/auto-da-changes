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
using STL.Common;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using STL.PL.WS2;

namespace STL.PL
{
    public partial class SalesCommissionEnquiry : CommonForm
    {

        private bool _staticLoaded = false;
        private StringCollection salesStaff = null;
        private bool isBranchManager = false;
        private int? branchNo = null;
        private int? empeenNo = null;
        private DataSet salesCommissionDetails = null;

        public SalesCommissionEnquiry(Form root, Form parent)
        {
            FormRoot = root;
            FormParent = parent;

            InitializeComponent();

            if (Credential.HasPermission(Blue.Cosacs.Shared.CosacsPermissionEnum.SalesCommissionBranchEnquiry))
            {
                isBranchManager = true;
            }

            if (isBranchManager == false)
            {
                drpBranchNo.Visible = false;
                drpEmployee.Visible = false;

                lblBranch.Visible = false;
                lblEmployee.Visible = false;
               
            }
            else
            {
                drpBranchNo.Visible = true;
                drpEmployee.Visible = true;
                drpBranchNo.Enabled = true;
                drpEmployee.Enabled = true;

                lblBranch.Visible = true;
                lblEmployee.Visible = true;
            }

            InitialiseStaticData();
        }


        public SalesCommissionEnquiry(Form root, Form parent, int? branch, string employee, DateTime dateFrom, DateTime DateTo)
        {
            FormRoot = root;
            FormParent = parent;

            InitializeComponent();

            if (Credential.HasPermission(Blue.Cosacs.Shared.CosacsPermissionEnum.SalesCommissionBranchEnquiry))
            {
                isBranchManager = true;
            }

            if (isBranchManager == false)
            {
                drpBranchNo.Visible = false;
                drpEmployee.Visible = false;

                lblBranch.Visible = false;
                lblEmployee.Visible = false;

            }
            else
            {
                drpBranchNo.Visible = true;
                drpEmployee.Visible = true;
                drpBranchNo.Enabled = true;
                drpEmployee.Enabled = true;

                lblBranch.Visible = true;
                lblEmployee.Visible = true;
            }

            InitialiseStaticData();

            LoadCommissionsFromBranchEnquiry(branch, employee, dateFrom, DateTo);
        }

        private void LoadCommissionsFromBranchEnquiry(int? branch, string employee, DateTime dateFrom, DateTime DateTo)
        {
            if (branch == null)
            {
                var index = drpBranchNo.FindString("All");
                if (index != -1)
                {
                    drpBranchNo.SelectedIndex = index;
                }
            }
            else
            {
                var index = drpBranchNo.FindString(branch.ToString());

                if (index != -1)
                {
                    drpBranchNo.SelectedIndex = index;
                }
            }

            var employeeIndex = drpEmployee.FindString(employee);

            if (employeeIndex != 1)
            {
                drpEmployee.SelectedIndex = employeeIndex;
            }

            dtpDeliveryDateFrom.Value = dateFrom;
            dtpDeliveryDateTo.Value = DateTo;

            btnSearch_Click(null, null);
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

                LoadSaleStaff();

                _staticLoaded = true;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        // load sales staff for Branch
        private void LoadSaleStaff()
        {
            try
            {
                drpEmployee.DataSource = null;
                salesStaff = new StringCollection();

                salesStaff.Add("All Sales Staff");

                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.SalesCommStaff, new string[] { Convert.ToString(drpBranchNo.SelectedValue), " " }));

                DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (dt.TableName == TN.SalesCommStaff)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                string str = Convert.ToString(row.ItemArray[0]) + " : " + (string)row.ItemArray[1];
                                salesStaff.Add(str.ToUpper());
                            }

                            drpEmployee.DataSource = salesStaff;
                            drpEmployee.SelectedIndex = 0;

                            if (Credential.IsInRole("S"))   // Sales Staff
                            {
                                int i = drpEmployee.FindString(Credential.UserId.ToString() + " : " + Credential.Name);
                                if (i != -1)
                                {
                                    drpEmployee.SelectedIndex = i;

                                }
                            }
                            else  // this duplicated code can be removed when "if (Credential.IsInRole("S"))" removed 
                            {
                                int i = drpEmployee.FindString(Credential.UserId.ToString() + " : " + Credential.Name);
                                if (i != -1)
                                {
                                    drpEmployee.SelectedIndex = i;
                                }
                            }

                        }
                    }
                }
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
            lblTotalCommissionsValue.Text = string.Empty;

            if (isBranchManager == false)
            {
                empeenNo = Credential.UserId;
                branchNo = int.Parse(Config.BranchCode);
            }
            else
            {
                branchNo = GetNullableInt(drpBranchNo.SelectedValue);

                if (drpEmployee.SelectedItem.ToString() != "All Sales Staff")
                {
                    var employeeString = drpEmployee.SelectedItem.ToString();

                    var index = employeeString.ToString().IndexOf(':');
                    empeenNo = int.Parse(employeeString.Substring(0, index).Trim());
                }
                else
                {
                    empeenNo = null;
                }

             
            }

            STL.PL.WS2.SalesCommissionDetails details = null;

            details = AccountManager.GeSalesCommissionDetails(branchNo, empeenNo, dtpDeliveryDateFrom.Value, dtpDeliveryDateTo.Value);

            DataView dtcommissionDetails = new DataView(details.SalesCommissions);

            dgvCommissions.DataSource = dtcommissionDetails;

            dgvCommissions.ColumnHeadersVisible = true;
            dgvCommissions.AutoGenerateColumns = true;

            if (!isBranchManager)
            {
                dgvCommissions.Columns[CN.Employee].Visible = false;
            }

            dgvCommissions.Columns["Buff No"].Visible = false;
            dgvCommissions.Columns["Employee"].Width = 150;
            dgvCommissions.Columns["Invoice Number"].Width = 60;
            dgvCommissions.Columns["Item Number"].Width = 80;
            dgvCommissions.Columns["Product Value (excluding VAT)"].Width = 100;
            dgvCommissions.Columns["Total Commission Value"].Width = 100;
            dgvCommissions.Columns["Total Commission %"].Width = 100;
            dgvCommissions.Columns["Item Delivery Date"].Width = 80;
            dgvCommissions.Columns["Item Description"].Width = 150;
            dgvCommissions.Columns["Item Description 2"].Width = 150;
            dgvCommissions.Columns["Commission Type"].Width = 100;
            dgvCommissions.Columns["Delivery Type"].Width = 50;
            dgvCommissions.Columns["Product Category"].Width = 100;

            lblTotalCommissionsValue.Text = details.TotalCommission.Value.ToString(DecimalPlaces);

            ((MainForm)this.FormRoot).statusBar1.Text = string.Empty;
         
        }

        private void drpBranchNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // avoid executing routine during initial load
            if (_staticLoaded)
            {
                LoadSaleStaff();
            }
        }

        private int? GetNullableInt(object intObj)
        {
            if (intObj != null)
            {
                var retInt = -1;
                if (int.TryParse(intObj.ToString(), out retInt))
                    return (int?)retInt;
                else
                    return null;
            }
            return null;
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
