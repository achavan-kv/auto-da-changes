using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Data;
using System.Web.Services.Protocols;
using STL.PL.WS9;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using System.Collections.Specialized;
using System.Xml;
using STL.Common.Constants.Elements;
using STL.Common.Constants.Tags;
using Crownwood.Magic.Menus;
using STL.Common.Constants.ItemTypes;
using STL.Common.Static;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;


namespace STL.PL
{
    /// <summary>
    /// Summary description for FinancialInterface.
    /// </summary>
    public class FinancialInterface : CommonForm
    {
        private System.Windows.Forms.DataGrid dgTransactions;
        private System.Windows.Forms.DataGrid dgBranch;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private string branchFilter = "";
        private string err = "";
        public System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textMoney;
        public System.Windows.Forms.Button btnExcelTotals;
        DataSet dsSUCBDetails = new DataSet();

        public FinancialInterface(Form root, Form parent, int runNo, int branchNo, bool liveDatabase)
        {
            FormRoot = root;
            FormParent = parent;
            InitializeComponent();

            if (branchNo > 0)
                branchFilter = CN.BranchNo + " = " + branchNo.ToString();

            LoadTotals(runNo, liveDatabase);
        }

        private void LoadTotals(int runNo, bool liveDatabase)
        {
            try
            {
                dsSUCBDetails = PaymentManager.GetSUCBFinancialDetails(runNo, liveDatabase, out err);
                if (err.Length > 0)
                    ShowError(err);
                else
                {
                    if (dsSUCBDetails != null)
                    {
                        dgBranch.DataSource = dsSUCBDetails.Tables[TN.FinTrans].DefaultView;

                        if (branchFilter.Length > 0)
                            ((DataView)dgBranch.DataSource).RowFilter = branchFilter;

                        if (dgBranch.TableStyles.Count == 0)
                        {
                            DataGridTableStyle tabStyle = new DataGridTableStyle();
                            tabStyle.MappingName = dsSUCBDetails.Tables[TN.FinTrans].TableName;
                            dgBranch.TableStyles.Add(tabStyle);

                            tabStyle.GridColumnStyles[CN.BranchNo].Width = 70;
                            tabStyle.GridColumnStyles[CN.BranchNo].HeaderText = GetResource("T_BRANCH");

                            tabStyle.GridColumnStyles[CN.TransTypeCode].Width = 75;
                            tabStyle.GridColumnStyles[CN.TransTypeCode].HeaderText = GetResource("T_TYPE");

                            tabStyle.GridColumnStyles[CN.TransValue].Width = 95;
                            tabStyle.GridColumnStyles[CN.TransValue].HeaderText = GetResource("T_VALUE");
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TransValue]).Format = DecimalPlaces;
                            tabStyle.GridColumnStyles[CN.Description].Width = 115;
                        }
                        decimal totalMoney = 0;
                        string tMoney;
                        foreach (DataRowView row in (DataView)dgBranch.DataSource)
                        {
                            totalMoney = totalMoney + ((decimal)row[CN.TransValue]);
                        }
                        tMoney = Convert.ToString(totalMoney);

                        textMoney.Text = Decimal.Parse(tMoney).ToString("c");


                    }
                }
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

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FinancialInterface));
            this.dgTransactions = new System.Windows.Forms.DataGrid();
            this.dgBranch = new System.Windows.Forms.DataGrid();
            this.btnExcel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textMoney = new System.Windows.Forms.TextBox();
            this.btnExcelTotals = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgTransactions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgBranch)).BeginInit();
            this.SuspendLayout();
            // 
            // dgTransactions
            // 
            this.dgTransactions.CaptionText = "Transactions";
            this.dgTransactions.DataMember = "";
            this.dgTransactions.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgTransactions.Location = new System.Drawing.Point(12, 243);
            this.dgTransactions.Name = "dgTransactions";
            this.dgTransactions.ReadOnly = true;
            this.dgTransactions.Size = new System.Drawing.Size(671, 178);
            this.dgTransactions.TabIndex = 60;
            // 
            // dgBranch
            // 
            this.dgBranch.CaptionText = "Branch Totals";
            this.dgBranch.DataMember = "";
            this.dgBranch.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgBranch.Location = new System.Drawing.Point(12, 24);
            this.dgBranch.Name = "dgBranch";
            this.dgBranch.ReadOnly = true;
            this.dgBranch.Size = new System.Drawing.Size(412, 187);
            this.dgBranch.TabIndex = 59;
            this.dgBranch.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgBranch_MouseUp);
            this.dgBranch.Navigate += new System.Windows.Forms.NavigateEventHandler(this.dgBranch_Navigate);
            // 
            // btnExcel
            // 
            this.btnExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnExcel.Image")));
            this.btnExcel.Location = new System.Drawing.Point(689, 243);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(32, 32);
            this.btnExcel.TabIndex = 61;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(430, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 24);
            this.label1.TabIndex = 62;
            this.label1.Text = "Total";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // textMoney
            // 
            this.textMoney.Location = new System.Drawing.Point(433, 86);
            this.textMoney.Name = "textMoney";
            this.textMoney.ReadOnly = true;
            this.textMoney.Size = new System.Drawing.Size(158, 20);
            this.textMoney.TabIndex = 63;
            this.textMoney.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // btnExcelTotals
            // 
            this.btnExcelTotals.Image = ((System.Drawing.Image)(resources.GetObject("btnExcelTotals.Image")));
            this.btnExcelTotals.Location = new System.Drawing.Point(430, 24);
            this.btnExcelTotals.Name = "btnExcelTotals";
            this.btnExcelTotals.Size = new System.Drawing.Size(32, 32);
            this.btnExcelTotals.TabIndex = 65;
            this.btnExcelTotals.Click += new System.EventHandler(this.btnExcelTotals_Click_2);
            // 
            // FinancialInterface
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 470);
            this.Controls.Add(this.btnExcelTotals);
            this.Controls.Add(this.textMoney);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnExcel);
            this.Controls.Add(this.dgTransactions);
            this.Controls.Add(this.dgBranch);
            this.Name = "FinancialInterface";
            this.Text = "Financial Interface";
            this.Load += new System.EventHandler(this.FinancialInterface_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgTransactions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgBranch)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void dgBranch_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                string type = "";
                string branchNo = "";

                int index = dgBranch.CurrentRowIndex;
                if (index >= 0)
                {
                    DataView dv = (DataView)dgBranch.DataSource;
                    type = (string)dv[index][CN.TransTypeCode];
                    // todo rename the table so that it says which transaction code description... 
                    /*    try
                        {
                            StringCollection codes = new StringCollection();
                            codes.Add("Codes");

                            transCodes = AccountManager.GetTranstypeByCode("", out Error);

                            if (Error.Length > 0)
                                ShowError(Error);
                            else
                            {
                                foreach (DataRow row in transCodes.Tables[0].Rows)
                                    codes.Add((string)(row[CN.TransTypeCode]) + " : " + (string)(row[CN.Description]));

                                drpTransTypeCodes.DataSource = codes;
                            }
                        }
                        catch (Exception ex)
                        {
                            Catch(ex, Function);
                        }*/


                    branchNo = (string)dv[index][CN.BranchNo];

                    if (dsSUCBDetails != null)
                    {
                        dgTransactions.DataSource = dsSUCBDetails.Tables[TN.Transactions].DefaultView;

                        ((DataView)dgTransactions.DataSource).RowFilter = CN.BranchNo + " = '" + branchNo + "' and " + CN.TransTypeCode + " = '" + type + "'";

                        if (dgTransactions.TableStyles.Count == 0)
                        {
                            DataGridTableStyle tabStyle = new DataGridTableStyle();
                            tabStyle.MappingName = dsSUCBDetails.Tables[TN.Transactions].TableName;
                            dgTransactions.TableStyles.Add(tabStyle);

                            tabStyle.GridColumnStyles[CN.AcctNo].Width = 85;
                            tabStyle.GridColumnStyles[CN.AcctNo].HeaderText = GetResource("T_ACCTNO");

                            tabStyle.GridColumnStyles[CN.TransTypeCode].Width = 75;
                            tabStyle.GridColumnStyles[CN.TransTypeCode].HeaderText = GetResource("T_TYPE");

                            tabStyle.GridColumnStyles[CN.DateTrans].Width = 75;
                            tabStyle.GridColumnStyles[CN.DateTrans].HeaderText = GetResource("T_DATE");

                            tabStyle.GridColumnStyles[CN.TransValue].Width = 75;
                            tabStyle.GridColumnStyles[CN.TransValue].HeaderText = GetResource("T_VALUE");
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TransValue]).Format = DecimalPlaces;

                            tabStyle.GridColumnStyles[CN.StoreCard].Width = 75;                                                     //IP - 20/02/12 - #9423 - CR8262
                            tabStyle.GridColumnStyles[CN.StoreCard].HeaderText = GetResource("T_STORECARD");

                            tabStyle.GridColumnStyles[CN.BranchNo].Width = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void btnExcel_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                /* save the current data grid contents to a CSV */
                string comma = ",";
                string path = "";

                if (dgTransactions.CurrentRowIndex >= 0)
                {
                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
                    save.Title = GetResource("T_EXCELSAVE");
                    save.CreatePrompt = true;

                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        path = save.FileName;
                        FileInfo fi = new FileInfo(path);
                        FileStream fs = fi.OpenWrite();
                        //write the top line header
                        string line =
                            CN.InterfaceAccount + comma +
                            CN.AcctNo + comma +
                            CN.TransTypeCode + comma +
                            CN.DateTrans + comma +
                            CN.TransValue + comma +
                            CN.Category + comma +
                            CN.BranchNo + comma +
                            CN.TransRefNo + comma +
                            CN.ItemNo + comma +
                            CN.StoreCard + comma +                                          //IP - 20/02/12 - #9423 - CR8262

                            Environment.NewLine + Environment.NewLine;
                        byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
                        fs.Write(blob, 0, blob.Length);

                        foreach (DataRowView row in (DataView)dgTransactions.DataSource)
                        {
                            line =
                                row[CN.InterfaceAccount].ToString() + comma +
                                row[CN.AcctNo].ToString().Replace(",", "") + comma +
                                row[CN.TransTypeCode].ToString().Replace(",", "") + comma +
                                Convert.ToString(row[CN.DateTrans]) + comma +
                                ((decimal)row[CN.TransValue]).ToString(DecimalPlaces).Replace(",", "") + comma +
                                row[CN.Category].ToString() + comma +
                                row[CN.BranchNo].ToString() + comma +
                                 row[CN.TransRefNo].ToString() + comma +
                                 row[CN.ItemNo].ToString() + comma +
                                 row[CN.StoreCard].ToString() + comma +                      //IP - 20/02/12 - #9423 - CR8262
                         Environment.NewLine;

                            blob = System.Text.Encoding.UTF8.GetBytes(line);
                            fs.Write(blob, 0, blob.Length);
                        }
                        fs.Close();

                        /* attempt to launch Excel. May get a COMException if Excel is not 
                            * installed */
                        try
                        {
                            /* we have to use Reflection to call the Open method because 
                                * the methods have different argument lists for the 
                                * different versions of Excel - JJ */
                            object[] args = null;
                            Excel.Application excel = new Excel.Application();

                            if (excel.Version == "10.0")	/* Excel2002 */
                                args = new object[] { path, 0, true, 2, "", "", true, Excel.XlPlatform.xlWindows, comma, true, false, 0, true, false, false };
                            else
                                args = new object[] { path, 0, true, 2, "", "", true, Excel.XlPlatform.xlWindows, comma, true, false, 0, true };

                            /* Retrieve the Workbooks property */
                            object wbs = excel.GetType().InvokeMember("Workbooks", BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty, null, excel, new Object[] { });

                            /* call the Open method */
                            object wb = wbs.GetType().InvokeMember("Open", BindingFlags.Public | BindingFlags.InvokeMethod, null, wbs, args);

                            excel.Visible = true;
                        }
                        catch (COMException)
                        {
                            /*change back slashes to forward slashes so the path doesn't
                                * get split into multiple lines */
                            ShowInfo("M_EXCELNOTFOUND", new object[] { path.Replace("\\", "/") });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnExcel_Click");
            }
            finally
            {
                StopWait();
            }

        }

        private void textBox1_TextChanged(object sender, System.EventArgs e)
        {

        }


        private void btnExcelTotals_Click_2(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                /* save the current data grid contents to a CSV */
                string comma = ",";
                string path = "";

                if (dgBranch.CurrentRowIndex >= 0)
                {
                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
                    save.Title = GetResource("T_EXCELSAVE");
                    save.CreatePrompt = true;

                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        path = save.FileName;
                        FileInfo fi = new FileInfo(path);
                        FileStream fs = fi.OpenWrite();
                        //write the top line header
                        string line =
                            CN.BranchNo + comma +
                            CN.TransTypeCode + comma +
                            CN.TransValue + comma +
                            CN.Description +
                            Environment.NewLine + Environment.NewLine;
                        byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
                        fs.Write(blob, 0, blob.Length);

                        foreach (DataRowView row in (DataView)dgBranch.DataSource)
                        {
                            line =
                                row[CN.BranchNo].ToString().Replace(",", "") + comma +
                                row[CN.TransTypeCode].ToString().Replace(",", "") + comma +
                                ((decimal)row[CN.TransValue]).ToString(DecimalPlaces).Replace(",", "") + comma
                                + Convert.ToString(row[CN.Description]) + comma
                                + Environment.NewLine;

                            blob = System.Text.Encoding.UTF8.GetBytes(line);
                            fs.Write(blob, 0, blob.Length);
                        }
                        fs.Close();

                        /* attempt to launch Excel. May get a COMException if Excel is not 
                            * installed */
                        try
                        {
                            /* we have to use Reflection to call the Open method because 
                                * the methods have different argument lists for the 
                                * different versions of Excel - JJ */
                            object[] args = null;
                            Excel.Application excel = new Excel.Application();

                            if (excel.Version == "10.0")	/* Excel2002 */
                                args = new object[] { path, 0, true, 2, "", "", true, Excel.XlPlatform.xlWindows, comma, true, false, 0, true, false, false };
                            else
                                args = new object[] { path, 0, true, 2, "", "", true, Excel.XlPlatform.xlWindows, comma, true, false, 0, true };

                            /* Retrieve the Workbooks property */
                            object wbs = excel.GetType().InvokeMember("Workbooks", BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty, null, excel, new Object[] { });

                            /* call the Open method */
                            object wb = wbs.GetType().InvokeMember("Open", BindingFlags.Public | BindingFlags.InvokeMethod, null, wbs, args);

                            excel.Visible = true;
                        }
                        catch (COMException)
                        {
                            /*change back slashes to forward slashes so the path doesn't
                                * get split into multiple lines */
                            ShowInfo("M_EXCELNOTFOUND", new object[] { path.Replace("\\", "/") });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnExcelTotals_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void FinancialInterface_Load(object sender, System.EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dgBranch_Navigate(object sender, NavigateEventArgs ne)
        {

        }
    }
}
