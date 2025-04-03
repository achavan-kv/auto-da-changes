using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using STL.Common.Static;
using System.Data;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.FTransaction;
using System.Xml;
using Crownwood.Magic.Menus;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace STL.PL
{
    public partial class PrizeVoucher : CommonForm
    {
        string error = "";

        public PrizeVoucher(Form root, Form parent)
        {
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;

            dynamicMenus = new Hashtable();
            //HashMenus();

            LoadStaticData();
        }

        private void LoadStaticData()
        {
            try
            {
                dtDateFrom.MaxDate = DateTime.Now;
                dtDateTo.MaxDate = DateTime.Now;
                dtDateTo.Value = DateTime.Now;
                dtDateFrom.Value = DateTime.Now.AddMonths(-2);

                StringCollection scBranch = new StringCollection();
                scBranch.Add("ALL");

                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                if (StaticData.Tables[TN.BranchNumber] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));

                if (dropDowns.DocumentElement.ChildNodes.Count > 0)
                {
                    DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out error);
                    if (error.Length > 0)
                        ShowError(error);
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            StaticData.Tables[dt.TableName] = dt;
                        }
                    }
                }

                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                    scBranch.Add(Convert.ToString(row[CN.BranchNo]));

                drpBranch.DataSource = scBranch;
                drpBranch.Text = Config.BranchCode;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Wait();
                
                string custID = txtCustID.Text + "%";
                string acctNo = "";
                string branch = "";
                int invoiceNo = 0;
                int rowCount = 0;
                dgvVouchers.DataSource = null;

                if (txtAccountNo.UnformattedText == "000000000000")
                    acctNo = "%";
                else
                    acctNo = txtAccountNo.UnformattedText + "%";

                if ((string)drpBranch.SelectedItem == "ALL")
                    branch = "%";
                else
                    branch = (string)drpBranch.SelectedItem + "%";

                if (txtInvoiceNo.Text.Length == 0)
                    invoiceNo = -1;
                else
                    invoiceNo = Convert.ToInt32(txtInvoiceNo.Text);

                DataSet ds = CustomerManager.GetPrizeVoucherDetails(acctNo, custID, branch, 
                                                   dtDateFrom.Value,dtDateTo.Value, invoiceNo, out error);

                if (error.Length > 0)
                    ShowError(error);
                else
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            rowCount = dt.Rows.Count;
                            dgvVouchers.DataSource = dt;

                            // Displayed columns
                            dgvVouchers.Columns[CN.CustID].HeaderText = GetResource("T_CUSTID");
                            dgvVouchers.Columns[CN.CustID].ReadOnly = true;
                            dgvVouchers.Columns[CN.CustID].Width = 120;

                            dgvVouchers.Columns[CN.AcctNo].HeaderText = GetResource("T_ACCTNO");
                            dgvVouchers.Columns[CN.AcctNo].ReadOnly = true;
                            dgvVouchers.Columns[CN.AcctNo].Width = 120;

                            dgvVouchers.Columns[CN.VoucherNo].HeaderText = GetResource("T_VOUCHERNO");
                            dgvVouchers.Columns[CN.VoucherNo].ReadOnly = true;
                            dgvVouchers.Columns[CN.VoucherNo].Width = 80;

                            dgvVouchers.Columns[CN.DateIssued].HeaderText = GetResource("T_DATEISSUED");
                            dgvVouchers.Columns[CN.DateIssued].ReadOnly = true;
                            dgvVouchers.Columns[CN.DateIssued].Width = 120;

                            dgvVouchers.Columns[CN.DatePrinted].HeaderText = GetResource("T_DATEPRINTED");
                            dgvVouchers.Columns[CN.DatePrinted].ReadOnly = true;
                            dgvVouchers.Columns[CN.DatePrinted].Width = 120;

                            dgvVouchers.Columns[CN.BuffNo].HeaderText = GetResource("T_INVOICE_NUMBER");
                            dgvVouchers.Columns[CN.BuffNo].ReadOnly = true;
                            dgvVouchers.Columns[CN.BuffNo].Width = 120;

                            dgvVouchers.ReadOnly = true;
                            dgvVouchers.MultiSelect = false;
                            dgvVouchers.AllowUserToAddRows = false;
                            dgvVouchers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                        }
                    }
                }

                ((MainForm)this.FormRoot).statusBar1.Text = rowCount.ToString() + GetResource("M_ROWSRETURNED");
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        private void btnAdditional_Click(object sender, EventArgs e)
        {
            if (dgvVouchers.DataSource != null)
            {
                int index = dgvVouchers.CurrentCell.RowIndex;

                string acctNo = (string)dgvVouchers[CN.AcctNo, index].Value;
                DateTime dateIssued = (DateTime)(dgvVouchers[CN.DateIssued, index].Value);

                PrintPrizeVouchers(acctNo, 0, 0, dateIssued, false, true);
            }
        }

        private void btnReprint_Click(object sender, EventArgs e)
        {
            if (dgvVouchers.DataSource != null)
            {
                AuthorisePrompt ap = new AuthorisePrompt(this, lReprint, GetResource("M_REPRINTVOUCHERS"));
                ap.ShowDialog();
                if (ap.Authorised)
                {
                    int index = dgvVouchers.CurrentCell.RowIndex;
                    string acctNo = (string)dgvVouchers[CN.AcctNo, index].Value;
                    DateTime dateIssued = (DateTime)(dgvVouchers[CN.DateIssued, index].Value);
                    int buffNo = (int)dgvVouchers[CN.BuffNo, index].Value;

                    PrintPrizeVouchers(acctNo, 0, buffNo, dateIssued, true, false);
                }
             }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteVouchers dv = new DeleteVouchers(FormRoot, this, lDelete);
            dv.ShowDialog();

            if (dv.deleted)
                btnSearch_Click(this, null);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dgvVouchers.DataSource = null;
            txtAccountNo.Text = "000000000000";
            txtCustID.Text = "";
            txtInvoiceNo.Text = "";
            dtDateTo.Value = dtDateTo.MaxDate;
            dtDateFrom.Value = DateTime.Now.AddMonths(-2);
            drpBranch.Text = Config.BranchCode;
            btnAdditional.Enabled = false;
            btnReprint.Enabled = false;
        }

        private void dgvVouchers_MouseUp(object sender, MouseEventArgs e)
        {
            if (dgvVouchers.DataSource != null)
            {
                int index = dgvVouchers.CurrentCell.RowIndex;

                btnAdditional.Enabled = dgvVouchers[CN.DatePrinted, index].Value == DBNull.Value;
                btnReprint.Enabled = !btnAdditional.Enabled;
            }
        }
    }
}