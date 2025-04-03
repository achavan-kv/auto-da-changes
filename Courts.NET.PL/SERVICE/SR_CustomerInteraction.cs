using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Crownwood.Magic.Menus;
using STL.Common;
using STL.Common.Constants.Categories;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using STL.PL.Utils;

namespace STL.PL.SERVICE
{
    public partial class SR_CustomerInteraction : CommonForm
    {
        private DataSet dsCustInt;

        public SR_CustomerInteraction()
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new[] { menuFile, menuHelp });
        }

        public string CustId
        {
            set
            {
                if (!txtCustId.Text.Equals(value))
                {
                    txtCustId.Text = value ?? "";
                    LoadCustomerInteraction();
                }
            }
        }

        private void SR_CustomerInteraction_Load(object sender, EventArgs e)
        {
            try
            {
                LoadStaticData();
                //dtInteraction.Value = DateTime.Today;
                btnSave.Enabled = false;    //#3433 jec 01/04/11
            }
            catch (Exception ex)
            {
                Catch(ex, "SR_CustomerInteraction_Load");
            }
            finally
            {
                StopWait();
            }
        }

        #region [ Private Methods]
        private void LoadStaticData()
        {
            var loaded = ConfirmStaticDataLoaded(
                            Tuple.Create(TN.ServiceCustomerInteractionType, new[] { CAT.ServiceCustomerInteraction, "L" }));
            if (!loaded)
                ShowError(Error);

            DataTable dtCustInt = ((DataTable)StaticData.Tables[TN.ServiceCustomerInteractionType]).Copy();
            DataRow dr = dtCustInt.NewRow();
            dr.ItemArray = new string[3] { "", "", "" };
            dtCustInt.Rows.InsertAt(dr, 0);

            drpInteractionType.DisplayMember = CN.Code;
            drpInteractionType.ValueMember = CN.CodeDescription;
            drpInteractionType.DataSource = dtCustInt;
        }

        private void LoadCustomerInteraction()
        {
            try
            {
                string custName;
                dsCustInt = ServiceManager.GetCustomerInteraction(txtCustId.Text.Trim(), out custName, out Error);

                if (Error.Length > 0)
                {
                    ShowError(Error);
                    return;
                }

                txtCustomerName.Text = custName;

                btnSave.Enabled = dsCustInt != null;    //#3433 jec 01/04/11 
                if (dsCustInt == null)
                    return;

                //Fill the customer service request numbers
                var dtSR = dsCustInt.Tables[TN.ServiceRequest];
                dtSR.Rows.InsertAt(dtSR.NewRow(), 0);
                dtSR.Rows[0][CN.ServiceRequestNoStr] = "";

                drpServiceRequestNo.DataSource = dtSR;
                drpServiceRequestNo.DisplayMember = CN.ServiceRequestNoStr;

                //Customer accounts
                dgAccounts.DataSource = dsCustInt.Tables[TN.CustomerAccounts].DefaultView;
                SetAccountRowFilter();

                DataTable dtAcct = dsCustInt.Tables[TN.CustomerAccounts].Copy();

                foreach (DataColumn dc in dsCustInt.Tables[TN.CustomerAccounts].Columns)
                    if (!dc.ColumnName.Equals(CN.AccountNumber))
                        dtAcct.Columns.Remove(dc.ColumnName);

                dtAcct.Rows.InsertAt(dtAcct.NewRow(), 0);
                dtAcct.Rows[0][CN.AccountNumber] = "";
                drpAccountNo.DataSource = dtAcct;
                drpAccountNo.DisplayMember = CN.AccountNumber;
                drpAccountNo.ValueMember = CN.AccountNumber;

                var dvCustInt = dsCustInt.Tables[TN.ServiceCustomerInteraction].DefaultView;
                dvCustInt.Sort = string.Concat(CN.Date, " DESC");
                dgCustomerInteractionLog.DataSource = dvCustInt;
                dgCustomerInteractionLog.Columns[CN.Code].Visible = false;
                dgCustomerInteractionLog.Columns[CN.ServiceRequestNo].Visible = false;

                ReportUtils.ApplyGridHeadings(dgCustomerInteractionLog, this);

                grpEntry.Enabled = true;
                txtCustId.Enabled = false;
            }
            catch (Exception ex)
            {
                Catch(ex, "LoadCustomerInteraction()");
            }
        }

        private bool Save()
        {
            try
            {
                ServiceManager.SaveCutomerInteraction(txtCustId.Text.Trim(), dsCustInt, out Error);

                if (Error.Length > 0)
                {
                    ShowError(Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "Save()");
                return false;
            }
            finally
            {
                txtCustId.Enabled = true;
            }

            return true;
        }
        #endregion

        private void btnCustSearch_Click(object sender, EventArgs e)
        {
            LoadCustomerInteraction();
        }

        private void btnAddInteraction_Click(object sender, EventArgs e)
        {
            errorProvider1.SetError(drpInteractionType, "");
            errorProvider1.SetError(dtInteraction, "");

            if (drpInteractionType.Text.Equals(string.Empty))
            {
                errorProvider1.SetError(drpInteractionType, GetResource("M_ENTERMANDATORY"));
                return;
            }
            else if (dtInteraction.Value.Date > DateTime.Today)
            {
                errorProvider1.SetError(dtInteraction, GetResource("M_INTERACTIONFUTUREDATE"));
                return;
            }

            if (dsCustInt != null)
            {
                DataTable dt = dsCustInt.Tables[TN.ServiceCustomerInteraction];
                DataRow dr = dt.NewRow();

                dr[CN.Date] = dtInteraction.Value;
                dr[CN.Code] = drpInteractionType.Text.ToString();
                dr[CN.Description] = txtInteractionDesc.Text;
                dr[CN.EmployeeNo] = Credential.UserId;
                dr[CN.AcctNo] = drpAccountNo.Text;
                dr[CN.ServiceRequestNoStr] = drpServiceRequestNo.Text.Length > 0 ? drpServiceRequestNo.Text : "0";
                dr[CN.Comments] = txtComments.Text;

                dt.Rows.Add(dr);
                _hasdatachanged = true;
                ClearInteractionFields();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Wait();

            if (ConfirmClose())
                ClearFields();

            StopWait();
        }

        private void ClearFields()
        {
            ClearControls(Controls);
            ClearInteractionFields();
            dgCustomerInteractionLog.DataSource = null;

            grpEntry.Enabled = false;
            _hasdatachanged = false;
            txtCustId.Enabled = true;
            btnSave.Enabled = false;    //#3433 jec 01/04/11 
        }

        private void ClearInteractionFields()
        {
            dtInteraction.Value = DateTime.Today;
            if (drpInteractionType.Items.Count > 0)
                drpInteractionType.SelectedIndex = 0;
            if (drpAccountNo.Items.Count > 0)
                drpAccountNo.SelectedIndex = 0;
            if (drpServiceRequestNo.Items.Count > 0)
                drpServiceRequestNo.SelectedIndex = 0;
            txtComments.Text = "";
            errorProvider1.Clear();
        }

        private void SR_CustomerInteraction_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !ConfirmClose();
        }

        public override bool ConfirmClose()
        {
            bool ret = true;
            if (_hasdatachanged)
            {
                DialogResult dr = ShowInfo("M_SAVECHANGES", MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Yes)
                {
                    if (!Save())
                    {
                        if (DialogResult.Yes != ShowInfo("M_CANTSAVECUST", MessageBoxButtons.YesNo))
                            ret = false;
                    }
                }
                if (dr == DialogResult.Cancel)
                {
                    ret = false;
                }
            }

            return ret;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();
            ClearFields();
        }

        private void chxIncludeAssocAccounts_CheckedChanged(object sender, EventArgs e)
        {
            SetAccountRowFilter();
        }

        private void chxIncludeSettled_CheckedChanged(object sender, EventArgs e)
        {
            SetAccountRowFilter();
        }

        private void SetAccountRowFilter()
        {
            var dv = dgAccounts.DataSource as DataView;

            if (dv == null)
                return;

            dv.RowFilter = string.Format("{0} not = ''", CN.AccountNumber);

            if (!chxIncludeSettled.Checked)
                dv.RowFilter += " and currstatus not = 'S'";
            if (!chxIncludeAssocAccounts.Checked)
                dv.RowFilter += " and HldOrJnt = 'H'";

            dgAccounts.Refresh();
        }

        //private void dgAccounts_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    try
        //    {
        //        Function = "dgAccounts_MouseUp";
        //        if (e.Button == MouseButtons.Right)
        //        {
        //            DataGrid ctl = (DataGrid)sender;

        //            MenuCommand m1 = new MenuCommand(GetResource("P_SERVICE_REQUEST"));

        //            m1.Click += new System.EventHandler(this.menuServiceRequest_Click);

        //            PopupMenu popup = new PopupMenu();
        //            popup.Animate = Animate.Yes;
        //            popup.AnimateStyle = Animation.SlideHorVerPositive;

        //            popup.MenuCommands.AddRange(new MenuCommand[] { m1 });
        //            MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, Function);
        //    }
        //}

        //private void menuServiceRequest_Click(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        Function = "menuServiceRequest";
        //        int index = dgAccounts.CurrentRowIndex;

        //        if (index >= 0)
        //        {
        //            string serviceNo = dgAccounts[index, 0].ToString();
        //            if (serviceNo.Length != 0)
        //            {
        //                SR_ServiceRequest p = new SR_ServiceRequest(this.FormRoot, this, serviceNo, string.Empty, string.Empty);
        //                ((MainForm)this.FormRoot).AddTabPage(p);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, Function);
        //    }
        //}

        private void drpInteractionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtInteractionDesc.Text = drpInteractionType.SelectedValue.ToString();
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            this.CloseTab();
        }

        private void mnuLaunchHelp_Click(object sender, EventArgs e)
        {
            string fileName = "CustomerInteraction.htm";
            LaunchHelp(fileName);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearInteractionFields();

            btnNew.Visible = false;
            btnAddInteraction.Visible = true;
        }

        private void dgCustomerInteractionLog_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgCustomerInteractionLog.CurrentRow != null && dgCustomerInteractionLog.CurrentRow.Index >= 0)
                    dgCustomerInteractionLog.Rows[dgCustomerInteractionLog.CurrentRow.Index].Selected = true;

                if (dgCustomerInteractionLog.SelectedRows.Count <= 0)
                    return;

                var dgvRow = dgCustomerInteractionLog.SelectedRows[0];

                dtInteraction.Value = Convert.ToDateTime(dgvRow.Cells[CN.Date].Value);
                drpInteractionType.SelectedValue = Convert.ToString(dgvRow.Cells[CN.Description].Value);
                txtInteractionDesc.Text = Convert.ToString(dgvRow.Cells[CN.Description].Value);

                var acctNo = Convert.ToString(dgvRow.Cells[CN.AcctNo].Value).Trim();
                drpAccountNo.SelectedValue = String.IsNullOrEmpty(acctNo) ? "" : acctNo;

                var SRNo = Convert.ToString(dgvRow.Cells[CN.ServiceRequestNoStr].Value).Trim();
                drpServiceRequestNo.Text = SRNo != "0" ? SRNo : "";

                txtComments.Text = Convert.ToString(dgvRow.Cells[CN.Comments].Value);

                btnNew.Visible = true;
                btnAddInteraction.Visible = false;
            }
            catch (Exception ex)
            {
                Catch(ex, "dgCustomerInteractionLog_SelectionChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void dgCustomerInteractionLog_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgCustomerInteractionLog_SelectionChanged(sender, e);
        }
    }
}
