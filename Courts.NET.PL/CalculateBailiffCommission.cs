using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Web.Services.Protocols;
using System.Xml;
using System.Data;
using STL.PL.WS5;
using STL.PL.WS1;
using STL.PL.WS2;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.TabPageNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Static;
using STL.Common;
using Crownwood.Magic.Menus;
using mshtml;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Blue.Cosacs.Shared;

namespace STL.PL
{
    /// <summary>
    /// Summary description for CalculateBailiffCommission.
    /// </summary>
    public class CalculateBailiffCommission : CommonForm
    {
        private DataTable _heldTable = null;
        private DataTable _deletedTable = null;

        private System.Windows.Forms.ComboBox drpEmpType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGrid dgStaffDetails;
        private System.Windows.Forms.DataGrid dgTransactions;
        private new string Error = "";
        //private bool staticLoaded = false;
        private System.Windows.Forms.ComboBox drpBranch;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnHold;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnPay;
        public System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.ImageList menuIcons;
        private System.Windows.Forms.ToolTip toolTip1;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Controls.TabControl tcBranch;
        private Crownwood.Magic.Controls.TabPage tbOnHold;
        private Crownwood.Magic.Controls.TabPage tbDeleted;
        private System.Windows.Forms.DataGrid dgTransactionsHeld;
        private System.Windows.Forms.DataGrid dgDeleted;
        private System.Windows.Forms.Button btnRelease;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRestore;
        private System.ComponentModel.IContainer components;

        public CalculateBailiffCommission(Form root, Form parent)
        {
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;

            InitialiseStaticData();

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
        }

        private void InitialiseStaticData()
        {
            try
            {
                Function = "BStaticDataManager::GetDropDownData";
                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                StringCollection branchNos = new StringCollection();
                StringCollection empTypes = new StringCollection();
                empTypes.Add("Staff Types");

                if (StaticData.Tables[TN.EmployeeTypes] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.EmployeeTypes,
                        new string[] { "ET1", "L" }));

                if (StaticData.Tables[TN.BranchNumber] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));

                if (dropDowns.DocumentElement.ChildNodes.Count > 0)
                {
                    DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                            StaticData.Tables[dt.TableName] = dt;
                    }
                }

                foreach (DataRow row in CollectionsManager.RolesGet((int)CosacsPermissionEnum.Bailiff).Tables[0].Rows)
                {
                    string str = string.Format("{0} : {1}", row[0], row[1]);
                    // Only show employee types with 'reference' column set
                    empTypes.Add(str.ToUpper());
                }
                drpEmpType.DataSource = empTypes;

                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                {
                    branchNos.Add(Convert.ToString(row["branchno"]));
                }
                drpBranch.DataSource = branchNos;
                drpBranch.Text = Config.BranchCode;


                //staticLoaded = true;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalculateBailiffCommission));
            this.drpEmpType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dgStaffDetails = new System.Windows.Forms.DataGrid();
            this.dgTransactions = new System.Windows.Forms.DataGrid();
            this.drpBranch = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnHold = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnPay = new System.Windows.Forms.Button();
            this.btnExcel = new System.Windows.Forms.Button();
            this.menuIcons = new System.Windows.Forms.ImageList(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.tcBranch = new Crownwood.Magic.Controls.TabControl();
            this.tbOnHold = new Crownwood.Magic.Controls.TabPage();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRelease = new System.Windows.Forms.Button();
            this.dgTransactionsHeld = new System.Windows.Forms.DataGrid();
            this.tbDeleted = new Crownwood.Magic.Controls.TabPage();
            this.btnRestore = new System.Windows.Forms.Button();
            this.dgDeleted = new System.Windows.Forms.DataGrid();
            ((System.ComponentModel.ISupportInitialize)(this.dgStaffDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgTransactions)).BeginInit();
            this.tcBranch.SuspendLayout();
            this.tbOnHold.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgTransactionsHeld)).BeginInit();
            this.tbDeleted.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDeleted)).BeginInit();
            this.SuspendLayout();
            // 
            // drpEmpType
            // 
            this.drpEmpType.ItemHeight = 13;
            this.drpEmpType.Location = new System.Drawing.Point(40, 96);
            this.drpEmpType.Name = "drpEmpType";
            this.drpEmpType.Size = new System.Drawing.Size(176, 21);
            this.drpEmpType.TabIndex = 61;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(40, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 16);
            this.label2.TabIndex = 62;
            this.label2.Text = "Employee Type";
            // 
            // dgStaffDetails
            // 
            this.dgStaffDetails.CaptionText = "Courts Persons";
            this.dgStaffDetails.DataMember = "";
            this.dgStaffDetails.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgStaffDetails.Location = new System.Drawing.Point(320, 24);
            this.dgStaffDetails.Name = "dgStaffDetails";
            this.dgStaffDetails.ReadOnly = true;
            this.dgStaffDetails.Size = new System.Drawing.Size(440, 120);
            this.dgStaffDetails.TabIndex = 65;
            this.dgStaffDetails.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgStaffDetails_MouseUp);
            // 
            // dgTransactions
            // 
            this.dgTransactions.CaptionText = "Commission Transactions Not Held";
            this.dgTransactions.DataMember = "";
            this.dgTransactions.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgTransactions.Location = new System.Drawing.Point(24, 160);
            this.dgTransactions.Name = "dgTransactions";
            this.dgTransactions.ReadOnly = true;
            this.dgTransactions.Size = new System.Drawing.Size(578, 136);
            this.dgTransactions.TabIndex = 64;
            this.dgTransactions.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgTransactions_MouseUp);
            // 
            // drpBranch
            // 
            this.drpBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranch.Location = new System.Drawing.Point(40, 40);
            this.drpBranch.Name = "drpBranch";
            this.drpBranch.Size = new System.Drawing.Size(48, 21);
            this.drpBranch.TabIndex = 77;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(40, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 16);
            this.label5.TabIndex = 78;
            this.label5.Text = "Branch";
            // 
            // btnSearch
            // 
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.Location = new System.Drawing.Point(192, 32);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(32, 32);
            this.btnSearch.TabIndex = 79;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnHold
            // 
            this.btnHold.Enabled = false;
            this.btnHold.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnHold.Location = new System.Drawing.Point(632, 216);
            this.btnHold.Name = "btnHold";
            this.btnHold.Size = new System.Drawing.Size(120, 24);
            this.btnHold.TabIndex = 80;
            this.btnHold.Text = "Hold Transaction";
            this.btnHold.Click += new System.EventHandler(this.btnHold_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Enabled = false;
            this.btnPrint.Image = ((System.Drawing.Image)(resources.GetObject("btnPrint.Image")));
            this.btnPrint.Location = new System.Drawing.Point(648, 256);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(32, 32);
            this.btnPrint.TabIndex = 82;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnPay
            // 
            this.btnPay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnPay.Location = new System.Drawing.Point(632, 168);
            this.btnPay.Name = "btnPay";
            this.btnPay.Size = new System.Drawing.Size(120, 24);
            this.btnPay.TabIndex = 83;
            this.btnPay.Text = "Pay Commission";
            this.btnPay.Click += new System.EventHandler(this.btnPay_Click);
            // 
            // btnExcel
            // 
            this.btnExcel.Enabled = false;
            this.btnExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnExcel.Image")));
            this.btnExcel.Location = new System.Drawing.Point(712, 256);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(32, 32);
            this.btnExcel.TabIndex = 84;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // menuIcons
            // 
            this.menuIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("menuIcons.ImageStream")));
            this.menuIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.menuIcons.Images.SetKeyName(0, "");
            this.menuIcons.Images.SetKeyName(1, "");
            this.menuIcons.Images.SetKeyName(2, "");
            this.menuIcons.Images.SetKeyName(3, "");
            this.menuIcons.Images.SetKeyName(4, "");
            this.menuIcons.Images.SetKeyName(5, "");
            this.menuIcons.Images.SetKeyName(6, "");
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.Text = "&File";
            // 
            // tcBranch
            // 
            this.tcBranch.IDEPixelArea = true;
            this.tcBranch.Location = new System.Drawing.Point(24, 304);
            this.tcBranch.Name = "tcBranch";
            this.tcBranch.PositionTop = true;
            this.tcBranch.SelectedIndex = 1;
            this.tcBranch.SelectedTab = this.tbDeleted;
            this.tcBranch.Size = new System.Drawing.Size(744, 168);
            this.tcBranch.TabIndex = 85;
            this.tcBranch.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tbOnHold,
            this.tbDeleted});
            // 
            // tbOnHold
            // 
            this.tbOnHold.Controls.Add(this.btnDelete);
            this.tbOnHold.Controls.Add(this.btnRelease);
            this.tbOnHold.Controls.Add(this.dgTransactionsHeld);
            this.tbOnHold.Location = new System.Drawing.Point(0, 25);
            this.tbOnHold.Name = "tbOnHold";
            this.tbOnHold.Selected = false;
            this.tbOnHold.Size = new System.Drawing.Size(744, 143);
            this.tbOnHold.TabIndex = 3;
            this.tbOnHold.Title = "On Hold";
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnDelete.Location = new System.Drawing.Point(608, 96);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(120, 24);
            this.btnDelete.TabIndex = 84;
            this.btnDelete.Text = "Delete Transaction";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnRelease
            // 
            this.btnRelease.Enabled = false;
            this.btnRelease.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRelease.Location = new System.Drawing.Point(608, 40);
            this.btnRelease.Name = "btnRelease";
            this.btnRelease.Size = new System.Drawing.Size(120, 24);
            this.btnRelease.TabIndex = 83;
            this.btnRelease.Text = "Release Transaction";
            this.btnRelease.Click += new System.EventHandler(this.btnRelease_Click);
            // 
            // dgTransactionsHeld
            // 
            this.dgTransactionsHeld.CaptionText = "Commission Transactions On Hold";
            this.dgTransactionsHeld.DataMember = "";
            this.dgTransactionsHeld.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgTransactionsHeld.Location = new System.Drawing.Point(0, 0);
            this.dgTransactionsHeld.Name = "dgTransactionsHeld";
            this.dgTransactionsHeld.ReadOnly = true;
            this.dgTransactionsHeld.Size = new System.Drawing.Size(578, 136);
            this.dgTransactionsHeld.TabIndex = 64;
            this.dgTransactionsHeld.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgTransactionsHeld_MouseUp);
            // 
            // tbDeleted
            // 
            this.tbDeleted.Controls.Add(this.btnRestore);
            this.tbDeleted.Controls.Add(this.dgDeleted);
            this.tbDeleted.Location = new System.Drawing.Point(0, 25);
            this.tbDeleted.Name = "tbDeleted";
            this.tbDeleted.Size = new System.Drawing.Size(744, 143);
            this.tbDeleted.TabIndex = 4;
            this.tbDeleted.Title = "Deleted";
            // 
            // btnRestore
            // 
            this.btnRestore.Enabled = false;
            this.btnRestore.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRestore.Location = new System.Drawing.Point(608, 48);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(120, 24);
            this.btnRestore.TabIndex = 84;
            this.btnRestore.Text = "Restore Transaction";
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // dgDeleted
            // 
            this.dgDeleted.CaptionText = "Commission Transactions DELETED";
            this.dgDeleted.DataMember = "";
            this.dgDeleted.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgDeleted.Location = new System.Drawing.Point(0, 0);
            this.dgDeleted.Name = "dgDeleted";
            this.dgDeleted.ReadOnly = true;
            this.dgDeleted.Size = new System.Drawing.Size(578, 136);
            this.dgDeleted.TabIndex = 64;
            this.dgDeleted.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgDeleted_MouseUp);
            // 
            // CalculateBailiffCommission
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.tcBranch);
            this.Controls.Add(this.btnExcel);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnHold);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.drpBranch);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dgStaffDetails);
            this.Controls.Add(this.dgTransactions);
            this.Controls.Add(this.drpEmpType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnPay);
            this.Name = "CalculateBailiffCommission";
            this.Text = "Calculate Bailiff Commission";
            ((System.ComponentModel.ISupportInitialize)(this.dgStaffDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgTransactions)).EndInit();
            this.tcBranch.ResumeLayout(false);
            this.tbOnHold.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgTransactionsHeld)).EndInit();
            this.tbDeleted.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgDeleted)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            string empType;
            string empTypeStr;
            DataSet ds = null;

            try
            {
                Wait();

                if (drpEmpType.SelectedIndex > 0)
                {
                    empTypeStr = (string)drpEmpType.SelectedItem;
                    int index = empTypeStr.IndexOf(":");
                    empType = empTypeStr.Substring(0, index - 1);

                    int branchNo = Convert.ToInt32(drpBranch.Text);

                    Login.CalculateBailiffCommission(empType, branchNo, out Error);

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        ds = Login.LoadCommissionDetailsByType(empType, branchNo, out Error);

                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            if (ds != null)
                            {
                                dgStaffDetails.DataSource = ds.Tables[TN.SalesStaff].DefaultView;

                                if (dgStaffDetails.TableStyles.Count == 0)
                                {
                                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                                    tabStyle.MappingName = ds.Tables[TN.SalesStaff].TableName;
                                    dgStaffDetails.TableStyles.Add(tabStyle);

                                    tabStyle.GridColumnStyles[CN.BranchNo].Width = 0;
                                    tabStyle.GridColumnStyles[CN.LstCommn].Width = 0;

                                    tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 90;
                                    tabStyle.GridColumnStyles[CN.EmployeeNo].ReadOnly = true;
                                    tabStyle.GridColumnStyles[CN.EmployeeNo].HeaderText = GetResource("T_EMPEENO");

                                    tabStyle.GridColumnStyles[CN.EmployeeName].Width = 140;
                                    tabStyle.GridColumnStyles[CN.EmployeeName].ReadOnly = true;
                                    tabStyle.GridColumnStyles[CN.EmployeeName].HeaderText = GetResource("T_EMPEENAME");

                                    tabStyle.GridColumnStyles[CN.CommnDue].Width = 154; //IP - 10/06/08 - Format screen
                                    tabStyle.GridColumnStyles[CN.CommnDue].ReadOnly = true;
                                    tabStyle.GridColumnStyles[CN.CommnDue].HeaderText = GetResource("T_COMMISSIONDUE");
                                    tabStyle.GridColumnStyles[CN.CommnDue].Alignment = HorizontalAlignment.Right;
                                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.CommnDue]).Format = DecimalPlaces;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSearch_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void dgStaffDetails_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //DataSet ds = null;

            try
            {
                Wait();

                // uat445 rdb 17/06/08 detect if row was clicked
                DataGrid sourceGrid = (DataGrid)sender;

                // Perform hit-test
                DataGrid.HitTestInfo hitTestInfo = sourceGrid.HitTest(e.X, e.Y);

                // Check if a column header was clicked
                if (hitTestInfo.Type != DataGrid.HitTestType.ColumnHeader &&
                    hitTestInfo.Type != DataGrid.HitTestType.Caption &&
                    hitTestInfo.Type != DataGrid.HitTestType.ColumnResize)
                {
                    LoadCommissionDetails();        // jec 14/07/08 UAT484

                }

            }
            catch (Exception ex)
            {
                Catch(ex, "dgStaffDetails_MouseUp");
            }
            finally
            {
                StopWait();
            }
        }
        // jec 14/07/08 UAT484 removed detail from dgStaffDetails_MouseUp event
        private void LoadCommissionDetails()
        {
            DataSet ds = null;
            //dgStaffDetails.Select(dgStaffDetails.CurrentCell.RowNumber);

            //UAT 499 To improve performance all data is brought back via one web service call.
            ds = PaymentManager.GetCommissionTransactions(
                SelectedEmpeeNo(),
                //"P",
                out Error);

            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                LoadTransactions(ds);

                //ds = PaymentManager.GetCommissionTransactions(
                //    SelectedEmpeeNo(),
                //    "H", out Error);

                //if (Error.Length > 0)
                //    ShowError(Error);
                //else
                LoadHeldTransactions(ds);

                //ds = PaymentManager.GetCommissionTransactions(
                //    SelectedEmpeeNo(),
                //    "D", out Error);

                //if (Error.Length > 0)
                //    ShowError(Error);
                //else
                LoadDeletedTransactions(ds);
            }

        }

        private void SetButtons()
        {
            btnExcel.Enabled = btnPrint.Enabled = btnHold.Enabled = dgTransactions.CurrentRowIndex >= 0;
            btnDelete.Enabled = btnRelease.Enabled = dgTransactionsHeld.CurrentRowIndex >= 0;
            btnRestore.Enabled = dgDeleted.CurrentRowIndex >= 0;
        }
        private void LoadTransactions(DataSet ds)
        {
            if (ds != null)
            {
                dgTransactions.DataSource = ds.Tables[0].DefaultView;
                this.SetButtons();

                if (dgTransactions.TableStyles.Count == 0)
                {
                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                    tabStyle.MappingName = ds.Tables[0].TableName;
                    dgTransactions.TableStyles.Add(tabStyle);

                    tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 0;
                    tabStyle.GridColumnStyles[CN.TransRefNo].Width = 0;
                    tabStyle.GridColumnStyles[CN.AmtCommPaidOn].Width = 0;

                    tabStyle.GridColumnStyles[CN.AcctNo].Width = 85;
                    tabStyle.GridColumnStyles[CN.AcctNo].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.AcctNo].HeaderText = GetResource("T_ACCTNO");

                    tabStyle.GridColumnStyles[CN.DateTrans].Width = 90;
                    tabStyle.GridColumnStyles[CN.DateTrans].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.DateTrans].HeaderText = GetResource("T_DATE");

                    tabStyle.GridColumnStyles[CN.TransValue].Width = 100;
                    tabStyle.GridColumnStyles[CN.TransValue].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.TransValue].HeaderText = GetResource("T_VALUE");
                    tabStyle.GridColumnStyles[CN.TransValue].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TransValue]).Format = DecimalPlaces;

                    tabStyle.GridColumnStyles[CN.ChequeColln].Width = 40;
                    tabStyle.GridColumnStyles[CN.ChequeColln].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.ChequeColln].HeaderText = GetResource("T_CHEQUE");

                    tabStyle.GridColumnStyles[CN.Status].Width = 40;
                    tabStyle.GridColumnStyles[CN.Status].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.Status].HeaderText = GetResource("T_STATUS");

                    tabStyle.GridColumnStyles[CN.Code].Width = 40;
                    tabStyle.GridColumnStyles[CN.Code].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.Code].HeaderText = GetResource("T_TYPE");

                    tabStyle.GridColumnStyles[CN.ActionValue].Width = 127; //IP - 10/06/08 - Format screen 
                    tabStyle.GridColumnStyles[CN.ActionValue].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.ActionValue].HeaderText = GetResource("T_ONAMOUNT");
                    tabStyle.GridColumnStyles[CN.ActionValue].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.ActionValue]).Format = DecimalPlaces;
                }
            }
        }

        private void LoadHeldTransactions(DataSet ds)
        {
            if (ds != null)
            {
                this._heldTable = ds.Tables[1];
                dgTransactionsHeld.DataSource = ds.Tables[1].DefaultView;
                this.SetButtons();

                if (dgTransactionsHeld.TableStyles.Count == 0)
                {
                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                    tabStyle.MappingName = ds.Tables[1].TableName;
                    dgTransactionsHeld.TableStyles.Add(tabStyle);

                    tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 0;
                    tabStyle.GridColumnStyles[CN.TransRefNo].Width = 0;
                    tabStyle.GridColumnStyles[CN.AmtCommPaidOn].Width = 0;

                    tabStyle.GridColumnStyles[CN.AcctNo].Width = 85;
                    tabStyle.GridColumnStyles[CN.AcctNo].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.AcctNo].HeaderText = GetResource("T_ACCTNO");

                    tabStyle.GridColumnStyles[CN.DateTrans].Width = 90;
                    tabStyle.GridColumnStyles[CN.DateTrans].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.DateTrans].HeaderText = GetResource("T_DATE");

                    tabStyle.GridColumnStyles[CN.TransValue].Width = 100;
                    tabStyle.GridColumnStyles[CN.TransValue].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.TransValue].HeaderText = GetResource("T_VALUE");
                    tabStyle.GridColumnStyles[CN.TransValue].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TransValue]).Format = DecimalPlaces;

                    tabStyle.GridColumnStyles[CN.ChequeColln].Width = 40;
                    tabStyle.GridColumnStyles[CN.ChequeColln].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.ChequeColln].HeaderText = GetResource("T_CHEQUE");

                    tabStyle.GridColumnStyles[CN.Status].Width = 40;
                    tabStyle.GridColumnStyles[CN.Status].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.Status].HeaderText = GetResource("T_STATUS");

                    tabStyle.GridColumnStyles[CN.Code].Width = 40;
                    tabStyle.GridColumnStyles[CN.Code].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.Code].HeaderText = GetResource("T_TYPE");

                    tabStyle.GridColumnStyles[CN.ActionValue].Width = 127; //IP - 10/06/08 - Format screen
                    tabStyle.GridColumnStyles[CN.ActionValue].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.ActionValue].HeaderText = GetResource("T_ONAMOUNT");
                    tabStyle.GridColumnStyles[CN.ActionValue].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.ActionValue]).Format = DecimalPlaces;
                }
            }
        }

        private void LoadDeletedTransactions(DataSet ds)
        {
            if (ds != null)
            {
                this._deletedTable = ds.Tables[2];
                dgDeleted.DataSource = ds.Tables[2].DefaultView;
                btnRestore.Enabled = ds.Tables[2].Rows.Count > 0;

                if (dgDeleted.TableStyles.Count == 0)
                {
                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                    tabStyle.MappingName = ds.Tables[2].TableName;
                    dgDeleted.TableStyles.Add(tabStyle);

                    tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 0;
                    tabStyle.GridColumnStyles[CN.TransRefNo].Width = 0;
                    tabStyle.GridColumnStyles[CN.AmtCommPaidOn].Width = 0;

                    tabStyle.GridColumnStyles[CN.AcctNo].Width = 85;
                    tabStyle.GridColumnStyles[CN.AcctNo].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.AcctNo].HeaderText = GetResource("T_ACCTNO");

                    tabStyle.GridColumnStyles[CN.DateTrans].Width = 90;
                    tabStyle.GridColumnStyles[CN.DateTrans].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.DateTrans].HeaderText = GetResource("T_DATE");

                    tabStyle.GridColumnStyles[CN.TransValue].Width = 100;
                    tabStyle.GridColumnStyles[CN.TransValue].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.TransValue].HeaderText = GetResource("T_VALUE");
                    tabStyle.GridColumnStyles[CN.TransValue].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TransValue]).Format = DecimalPlaces;

                    tabStyle.GridColumnStyles[CN.ChequeColln].Width = 40;
                    tabStyle.GridColumnStyles[CN.ChequeColln].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.ChequeColln].HeaderText = GetResource("T_CHEQUE");

                    tabStyle.GridColumnStyles[CN.Status].Width = 40;
                    tabStyle.GridColumnStyles[CN.Status].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.Status].HeaderText = GetResource("T_STATUS");

                    tabStyle.GridColumnStyles[CN.Code].Width = 40;
                    tabStyle.GridColumnStyles[CN.Code].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.Code].HeaderText = GetResource("T_TYPE");

                    tabStyle.GridColumnStyles[CN.ActionValue].Width = 127; //IP - 10/06/08 - Format screen
                    tabStyle.GridColumnStyles[CN.ActionValue].ReadOnly = true;
                    tabStyle.GridColumnStyles[CN.ActionValue].HeaderText = GetResource("T_ONAMOUNT");
                    tabStyle.GridColumnStyles[CN.ActionValue].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.ActionValue]).Format = DecimalPlaces;
                }
            }
        }

        private void btnHold_Click(object sender, System.EventArgs e)
        {
            SetUpTransactions("H");
            RefreshCommissionValues();
            this.SetButtons();
        }

        private void btnRelease_Click(object sender, System.EventArgs e)
        {
            SetUpTransactions("P");
            RefreshCommissionValues();
            this.SetButtons();
        }

        private void SetUpTransactions(string status)
        {
            //UAT 487 Changes to the data grid data sources are now made here instead of re-populating them via a web service call.
            DataTable dtTransactions = new DataTable();
            DataTable dtTransactionsHeld = new DataTable();
            DataSet ds = new DataSet();

            try
            {
                Wait();

                DataTable dtTransactionsNew = new DataTable(TN.Transactions);
                dtTransactionsNew.Columns.AddRange(new DataColumn[] {	new DataColumn(CN.EmployeeNo),
																	 new DataColumn(CN.TransRefNo),
																	 new DataColumn(CN.DateTrans, Type.GetType("System.DateTime")),
																	 new DataColumn(CN.Status)});

                //if(status == "H")
                dtTransactions = ((DataView)dgTransactions.DataSource).ToTable();
                //else
                dtTransactionsHeld = ((DataView)dgTransactionsHeld.DataSource).ToTable();

                int transactionsCount = dtTransactions.Rows.Count;
                int transactionsHeldCount = dtTransactionsHeld.Rows.Count;

                //if(status == "H")
                //{
                for (int i = transactionsCount - 1; i >= 0; i--)
                {
                    if (dgTransactions.IsSelected(i))
                    {
                        DataRow row = dtTransactionsNew.NewRow();
                        row[CN.EmployeeNo] = Convert.ToInt32(dtTransactions.Rows[i][CN.EmployeeNo]);
                        row[CN.TransRefNo] = Convert.ToInt32(dtTransactions.Rows[i][CN.TransRefNo]);
                        row[CN.DateTrans] = Convert.ToDateTime(dtTransactions.Rows[i][CN.DateTrans]);
                        row[CN.Status] = status;
                        dtTransactionsNew.Rows.Add(row);
                        DataRow transactionRow = dtTransactions.NewRow();
                        transactionRow = dtTransactions.Rows[i];
                        dtTransactionsHeld.ImportRow(transactionRow);

                        //dgTransactionsHeld.DataSource = dtTransactionsHeld.DefaultView;
                        dtTransactions.Rows[i][CN.AcctNo] = "";
                    }
                }
                //}
                //else
                //{
                for (int i = transactionsHeldCount - 1; i >= 0; i--)
                {
                    if (dgTransactionsHeld.IsSelected(i))
                    {
                        DataRow row = dtTransactionsNew.NewRow();
                        row[CN.EmployeeNo] = Convert.ToInt32(dtTransactionsHeld.Rows[i][CN.EmployeeNo]);
                        row[CN.TransRefNo] = Convert.ToInt32(dtTransactionsHeld.Rows[i][CN.TransRefNo]);
                        row[CN.DateTrans] = Convert.ToDateTime(dtTransactionsHeld.Rows[i][CN.DateTrans]);
                        row[CN.Status] = status;
                        dtTransactionsNew.Rows.Add(row);
                        DataRow transactionRowHeld = dtTransactionsHeld.NewRow();
                        transactionRowHeld = dtTransactionsHeld.Rows[i];
                        dtTransactions.ImportRow(transactionRowHeld);

                        //dgTransactions.DataSource = dtTransactions.DefaultView;
                        dtTransactionsHeld.Rows[i][CN.AcctNo] = "";
                    }
                }
                //}

                ds.Tables.Add(dtTransactionsNew);

                for (int i = transactionsCount - 1; i >= 0; i--)
                {
                    if ((string)dtTransactions.Rows[i][CN.acctno] == "")
                        dtTransactions.Rows.RemoveAt(i);
                }

                for (int i = transactionsHeldCount - 1; i >= 0; i--)
                {
                    if ((string)dtTransactionsHeld.Rows[i][CN.acctno] == "")
                        dtTransactionsHeld.Rows.RemoveAt(i);
                }

                dgTransactionsHeld.DataSource = dtTransactionsHeld.DefaultView;
                dgTransactions.DataSource = dtTransactions.DefaultView;

                PaymentManager.UpdateCommissionTransactionStatus(ds, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
            }
            catch (Exception ex)
            {
                Catch(ex, "dgStaffDetails_MouseUp");
            }
            finally
            {
                StopWait();
            }
        }

        private int SelectedEmpeeNo()
        {
            int empeeNo = 0;
            int index = dgStaffDetails.CurrentRowIndex;
            if (index >= 0)
            {

                dgStaffDetails.Select(dgStaffDetails.CurrentCell.RowNumber);
                empeeNo = Convert.ToInt32(dgStaffDetails[index, 1]);
            }

            return empeeNo;
        }

        private string SelectedEmpeeName()
        {
            string empeeName = "";
            int index = dgStaffDetails.CurrentRowIndex;
            if (index >= 0)
            {

                dgStaffDetails.Select(dgStaffDetails.CurrentCell.RowNumber);
                empeeName = (string)(dgStaffDetails[index, 2]);
            }

            return empeeName;
        }

        private decimal SelectedCommValue()
        {
            decimal commValue = 0;
            int index = dgStaffDetails.CurrentRowIndex;
            if (index >= 0)
            {

                dgStaffDetails.Select(dgStaffDetails.CurrentCell.RowNumber);
                commValue = Convert.ToDecimal((dgStaffDetails[index, 3]));
            }

            return commValue;
        }

        private void btnPrint_Click(object sender, System.EventArgs e)
        {
            int empeeNo = SelectedEmpeeNo();
            string employee = SelectedEmpeeName() + " (" + empeeNo.ToString() + ")";

            PrintBailiffCommissionTransactions(dgTransactions, SelectedEmpeeNo(), SelectedEmpeeName());
        }

        private void btnPay_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (SelectedCommValue() > 0)
                {
                    Wait();

                    PaymentManager.PayBailiffCommission(SelectedEmpeeNo(), SelectedCommValue(), out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        PrintCommissionPayment(dgStaffDetails, dgTransactions, SelectedEmpeeNo(),
                            SelectedEmpeeName(), SelectedCommValue());

                        dgTransactions.DataSource = null;
                        dgTransactionsHeld.DataSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnPay_Click");
            }
            finally
            {
                StopWait();
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
                string line = "";

                if (dgTransactions.CurrentRowIndex >= 0)
                {
                    DataView dv = (DataView)dgTransactions.DataSource;

                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
                    save.Title = "Save Journal Enquiry";
                    save.CreatePrompt = true;

                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        path = save.FileName;
                        FileInfo fi = new FileInfo(path);
                        FileStream fs = fi.OpenWrite();

                        line = GetResource("T_EMPEENO") + comma +
                            GetResource("T_ACCTNO") + comma +
                            GetResource("T_DATE") + comma +
                            GetResource("T_VALUE") + comma +
                            GetResource("T_CHEQUE") + comma +
                            GetResource("T_STATUS") + comma +
                            GetResource("T_TYPE") + comma +
                            GetResource("T_ONAMOUNT") + Environment.NewLine + Environment.NewLine;

                        byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
                        fs.Write(blob, 0, blob.Length);

                        foreach (DataRowView row in dv)
                        {
                            line = SelectedEmpeeNo().ToString() + comma +
                                "'" + row[CN.AcctNo] + "'" + comma +
                                Convert.ToString(row[CN.DateTrans]) + comma +
                                Convert.ToDecimal(row[CN.TransValue]).ToString(DecimalPlaces).Replace(",", "") + comma +
                                row[CN.ChequeColln] + comma +
                                row[CN.Status] + comma +
                                row[CN.Code] + comma +
                                Convert.ToDecimal(row[CN.ActionValue]).ToString(DecimalPlaces).Replace(",", "") + Environment.NewLine;

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
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        public void RefreshCommissionValues()
        {
            int i = 0;

            int prevEmpeeNo = SelectedEmpeeNo();
            btnSearch_Click(this, null);

            foreach (DataRowView row in (DataView)dgStaffDetails.DataSource)
            {
                if ((int)row[CN.EmployeeNo] == prevEmpeeNo)
                {
                    dgStaffDetails.CurrentRowIndex = i;
                    dgStaffDetails.Select(i);

                    //dgStaffDetails_MouseUp(this.dgStaffDetails, null);

                    //UAT 487 This method involves a web service call to 'refresh' the screen. No longer required. 
                    //LoadCommissionDetails();        // jec 14/07/08 UAT484
                }

                i++;
            }
        }

        private void btnDelete_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                for (int i = ((DataView)dgTransactionsHeld.DataSource).Count - 1; i >= 0; i--)
                {
                    if (dgTransactionsHeld.IsSelected(i))
                    {
                        DataRowView curRow = ((DataView)dgTransactionsHeld.DataSource)[i];

                        PaymentManager.DeleteCommissionTransaction((int)curRow[CN.EmployeeNo], (DateTime)curRow[CN.DateTrans], (int)curRow[CN.TransRefNo], out Error);
                        if (Error.Length > 0)
                            ShowError(Error);

                        // Move the row from the On Hold list to the Deleted list
                        DataRow newRow = this._deletedTable.NewRow();
                        newRow[CN.EmployeeNo] = (int)curRow[CN.EmployeeNo];
                        newRow[CN.DateTrans] = (DateTime)curRow[CN.DateTrans];
                        newRow[CN.TransRefNo] = (int)curRow[CN.TransRefNo];
                        newRow[CN.AccountNumber] = (string)curRow[CN.AccountNumber];
                        newRow[CN.TransValue] = (double)curRow[CN.TransValue];
                        newRow[CN.ChequeColln] = (string)curRow[CN.ChequeColln];
                        newRow[CN.Status] = (string)curRow[CN.Status];
                        newRow[CN.Code] = (string)curRow[CN.Code];
                        newRow[CN.AmtCommPaidOn] = (double)curRow[CN.AmtCommPaidOn];
                        newRow[CN.ActionValue] = (double)curRow[CN.ActionValue];
                        this._deletedTable.Rows.Add(newRow);
                        curRow[CN.AccountNumber] = "";
                    }
                }

                for (int i = ((DataView)dgTransactionsHeld.DataSource).Count - 1; i >= 0; i--)
                {
                    DataRowView curRow = ((DataView)dgTransactionsHeld.DataSource)[i];
                    if ((string)curRow[CN.AccountNumber] == "") curRow.Delete();
                }

                ((DataView)dgTransactionsHeld.DataSource).Table.AcceptChanges();
                ((DataView)dgDeleted.DataSource).Table.AcceptChanges();

                this.SetButtons();
            }
            catch (Exception ex)
            {
                Catch(ex, "btnDelete_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnRestore_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                for (int i = ((DataView)dgDeleted.DataSource).Count - 1; i >= 0; i--)
                {
                    if (dgDeleted.IsSelected(i))
                    {
                        DataRowView curRow = ((DataView)dgDeleted.DataSource)[i];

                        PaymentManager.RestoreCommissionTransaction((int)curRow[CN.EmployeeNo], (DateTime)curRow[CN.DateTrans], (int)curRow[CN.TransRefNo], out Error);
                        if (Error.Length > 0)
                            ShowError(Error);

                        // Move the row from the On Hold list to the Deleted list
                        DataRow newRow = this._heldTable.NewRow();
                        newRow[CN.EmployeeNo] = (int)curRow[CN.EmployeeNo];
                        newRow[CN.DateTrans] = (DateTime)curRow[CN.DateTrans];
                        newRow[CN.TransRefNo] = (int)curRow[CN.TransRefNo];
                        newRow[CN.AccountNumber] = (string)curRow[CN.AccountNumber];
                        newRow[CN.TransValue] = (double)curRow[CN.TransValue];
                        newRow[CN.ChequeColln] = (string)curRow[CN.ChequeColln];
                        newRow[CN.Status] = (string)curRow[CN.Status];
                        newRow[CN.Code] = (string)curRow[CN.Code];
                        newRow[CN.AmtCommPaidOn] = (double)curRow[CN.AmtCommPaidOn];
                        newRow[CN.ActionValue] = (double)curRow[CN.ActionValue];
                        this._heldTable.Rows.Add(newRow);
                        curRow[CN.AccountNumber] = "";
                    }
                }

                for (int i = ((DataView)dgDeleted.DataSource).Count - 1; i >= 0; i--)
                {
                    DataRowView curRow = ((DataView)dgDeleted.DataSource)[i];
                    if ((string)curRow[CN.AccountNumber] == "") curRow.Delete();
                }

                ((DataView)dgTransactionsHeld.DataSource).Table.AcceptChanges();
                ((DataView)dgDeleted.DataSource).Table.AcceptChanges();

                this.SetButtons();
            }
            catch (Exception ex)
            {
                Catch(ex, "btnRestore_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void dgTransactions_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Wait();
                this.SetButtons();
            }
            catch (Exception ex)
            {
                Catch(ex, "dgTransactions_MouseUp");
            }
            finally
            {
                StopWait();
            }
        }

        private void dgTransactionsHeld_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Wait();
                this.SetButtons();
            }
            catch (Exception ex)
            {
                Catch(ex, "dgTransactionsHeld_MouseUp");
            }
            finally
            {
                StopWait();
            }
        }

        private void dgDeleted_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Wait();
                this.SetButtons();
            }
            catch (Exception ex)
            {
                Catch(ex, "dgDeleted_MouseUp");
            }
            finally
            {
                StopWait();
            }
        }
    }
}
