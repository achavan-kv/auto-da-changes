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
using STL.Common.Constants.CountryCodes;

namespace STL.PL
{
    /// <summary>
    /// This screen allows the user to request the calculation of Rebate totals and 
    /// displays the results at both the Branch level and the Country level (all branches).
    /// The user also has the option to only view Branch totals for a specific branch.
    /// </summary>
    public class RebateCalculation : CommonForm
    {
        private string error = "";
        private DataTable _branchData;
        private StringCollection _sc = new StringCollection();
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.ComboBox drpBranchNo;
        private System.Windows.Forms.Label lBranch;
        private System.Windows.Forms.DataGrid dgBranchData;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.DateTimePicker dtThresholdTo;
        private System.Windows.Forms.DateTimePicker dtThresholdFrom;
        private System.Windows.Forms.Label lTo;
        private System.Windows.Forms.Label lFrom;
        private System.Windows.Forms.GroupBox gbDeliveries;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem cmenuComments;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.DataGrid dgCountryData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox drpRuleChangeDate;
        private System.Windows.Forms.DateTimePicker dtAcctsOpened;
        private System.Windows.Forms.DateTimePicker dtRebateCalc;
        public System.Windows.Forms.Button btnExcelCountry;
        private System.Windows.Forms.GroupBox gbResults;
        public System.Windows.Forms.Button btnExcelBranch;
        private System.Windows.Forms.Label lbRuleChangeDate;
		private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtAsAtDate;
        private DateTimePicker dtpSLRuleChange;
        private Label lbSLRuleChange;
        //private System.ComponentModel.IContainer components;

        public RebateCalculation(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
        }

        public RebateCalculation(Form root, Form parent)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            //if( disposing )
            //{
            //    if(components != null)
            //    {
            //        components.Dispose();
            //    }
            //}
            base.Dispose( disposing );
        }

		#region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(RebateCalculation));
			this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
			this.gbDeliveries = new System.Windows.Forms.GroupBox();
            this.lbSLRuleChange = new System.Windows.Forms.Label();
            this.dtpSLRuleChange = new System.Windows.Forms.DateTimePicker();
			this.label3 = new System.Windows.Forms.Label();
			this.txtAsAtDate = new System.Windows.Forms.TextBox();
			this.gbResults = new System.Windows.Forms.GroupBox();
			this.btnExcelBranch = new System.Windows.Forms.Button();
			this.btnExcelCountry = new System.Windows.Forms.Button();
			this.dgCountryData = new System.Windows.Forms.DataGrid();
			this.dgBranchData = new System.Windows.Forms.DataGrid();
			this.btnCalculate = new System.Windows.Forms.Button();
			this.dtThresholdTo = new System.Windows.Forms.DateTimePicker();
			this.dtThresholdFrom = new System.Windows.Forms.DateTimePicker();
			this.lTo = new System.Windows.Forms.Label();
			this.lFrom = new System.Windows.Forms.Label();
			this.lBranch = new System.Windows.Forms.Label();
			this.drpBranchNo = new System.Windows.Forms.ComboBox();
			this.btnClear = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.dtAcctsOpened = new System.Windows.Forms.DateTimePicker();
			this.dtRebateCalc = new System.Windows.Forms.DateTimePicker();
			this.label2 = new System.Windows.Forms.Label();
			this.drpRuleChangeDate = new System.Windows.Forms.ComboBox();
			this.lbRuleChangeDate = new System.Windows.Forms.Label();
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.cmenuComments = new System.Windows.Forms.MenuItem();
			this.gbDeliveries.SuspendLayout();
			this.gbResults.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgCountryData)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgBranchData)).BeginInit();
			this.SuspendLayout();
			// 
			// menuFile
			// 
			this.menuFile.Description = "MenuItem";
			this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
																							this.menuExit});
			this.menuFile.Text = "&File";
			// 
			// menuExit
			// 
			this.menuExit.Description = "MenuItem";
			this.menuExit.Text = "E&xit";
			this.menuExit.Click += new System.EventHandler(this.fileExit_Click);
			// 
			// gbDeliveries
			// 
			this.gbDeliveries.BackColor = System.Drawing.SystemColors.Control;
			this.gbDeliveries.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                       this.lbSLRuleChange,
                                                                                       this.dtpSLRuleChange,
																					   this.label3,
																					   this.txtAsAtDate,
																					   this.gbResults,
																					   this.btnCalculate,
																					   this.dtThresholdTo,
																					   this.dtThresholdFrom,
																					   this.lTo,
																					   this.lFrom,
																					   this.lBranch,
																					   this.drpBranchNo,
																					   this.btnClear,
																					   this.label1,
																					   this.dtAcctsOpened,
																					   this.dtRebateCalc,
																					   this.label2,
																					   this.drpRuleChangeDate,
																					   this.lbRuleChangeDate});
			this.gbDeliveries.Location = new System.Drawing.Point(8, 0);
			this.gbDeliveries.Name = "gbDeliveries";
			this.gbDeliveries.Size = new System.Drawing.Size(776, 480);
			this.gbDeliveries.TabIndex = 0;
			this.gbDeliveries.TabStop = false;
			//
            // lbSLRuleChange
            // 
            this.lbSLRuleChange.AutoSize = true;
            this.lbSLRuleChange.Location = new System.Drawing.Point(366, 70);
            this.lbSLRuleChange.Name = "lbSLRuleChange";
            this.lbSLRuleChange.Size = new System.Drawing.Size(131, 13);
            this.lbSLRuleChange.TabIndex = 60;
            this.lbSLRuleChange.Text = "S. Line Rule Change Date";
            // 
            // dtpSLRuleChange
            // 
            this.dtpSLRuleChange.CustomFormat = "dd MMM yyyy";
            this.dtpSLRuleChange.Enabled = false;
            this.dtpSLRuleChange.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpSLRuleChange.Location = new System.Drawing.Point(504, 64);
            this.dtpSLRuleChange.Name = "dtpSLRuleChange";
            this.dtpSLRuleChange.Size = new System.Drawing.Size(112, 20);
            this.dtpSLRuleChange.TabIndex = 59;
            this.dtpSLRuleChange.Value = new System.DateTime(2008, 3, 31, 0, 0, 0, 0);
            // 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(328, 456);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(224, 16);
			this.label3.TabIndex = 58;
			this.label3.Text = "Rebate Calculation Date for Report Shown";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// txtAsAtDate
			// 
			this.txtAsAtDate.Location = new System.Drawing.Point(560, 456);
			this.txtAsAtDate.Name = "txtAsAtDate";
			this.txtAsAtDate.ReadOnly = true;
            this.txtAsAtDate.Size = new System.Drawing.Size(100, 20);
			this.txtAsAtDate.TabIndex = 57;
			this.txtAsAtDate.Text = "";
			// 
			// gbResults
			// 
			this.gbResults.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.btnExcelBranch,
																					this.btnExcelCountry,
																					this.dgCountryData,
																					this.dgBranchData});
			this.gbResults.Location = new System.Drawing.Point(16, 88);
			this.gbResults.Name = "gbResults";
			this.gbResults.Size = new System.Drawing.Size(744, 360);
			this.gbResults.TabIndex = 56;
			this.gbResults.TabStop = false;
			// 
			// btnExcelBranch
			// 
			this.btnExcelBranch.Enabled = false;
			this.btnExcelBranch.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnExcelBranch.Image")));
			this.btnExcelBranch.Location = new System.Drawing.Point(680, 208);
			this.btnExcelBranch.Name = "btnExcelBranch";
			this.btnExcelBranch.Size = new System.Drawing.Size(32, 32);
			this.btnExcelBranch.TabIndex = 56;
			this.btnExcelBranch.Click += new System.EventHandler(this.btnExcelBranch_Click);
			// 
			// btnExcelCountry
			// 
			this.btnExcelCountry.Enabled = false;
			this.btnExcelCountry.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnExcelCountry.Image")));
			this.btnExcelCountry.Location = new System.Drawing.Point(680, 32);
			this.btnExcelCountry.Name = "btnExcelCountry";
			this.btnExcelCountry.Size = new System.Drawing.Size(32, 32);
			this.btnExcelCountry.TabIndex = 55;
			this.btnExcelCountry.Click += new System.EventHandler(this.btnExcelCountry_Click);
			// 
			// dgCountryData
			// 
			this.dgCountryData.CaptionText = "Country Totals";
			this.dgCountryData.DataMember = "";
			this.dgCountryData.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgCountryData.Location = new System.Drawing.Point(0, 8);
			this.dgCountryData.Name = "dgCountryData";
			this.dgCountryData.ReadOnly = true;
			this.dgCountryData.Size = new System.Drawing.Size(744, 176);
			this.dgCountryData.TabIndex = 0;
			this.dgCountryData.TabStop = false;
			// 
			// dgBranchData
			// 
			this.dgBranchData.CaptionText = "Branch Totals";
			this.dgBranchData.DataMember = "";
			this.dgBranchData.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgBranchData.Location = new System.Drawing.Point(0, 184);
			this.dgBranchData.Name = "dgBranchData";
			this.dgBranchData.ReadOnly = true;
			this.dgBranchData.Size = new System.Drawing.Size(744, 176);
			this.dgBranchData.TabIndex = 0;
			this.dgBranchData.TabStop = false;
			// 
			// btnCalculate
			// 
			this.btnCalculate.Location = new System.Drawing.Point(688, 16);
			this.btnCalculate.Name = "btnCalculate";
			this.btnCalculate.Size = new System.Drawing.Size(72, 23);
			this.btnCalculate.TabIndex = 40;
			this.btnCalculate.Text = "Calculate";
			this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
			// 
			// dtThresholdTo
			// 
			this.dtThresholdTo.CustomFormat = "dd MMM yyyy";
			this.dtThresholdTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtThresholdTo.Location = new System.Drawing.Point(248, 64);
			this.dtThresholdTo.Name = "dtThresholdTo";
			this.dtThresholdTo.Size = new System.Drawing.Size(112, 20);
			this.dtThresholdTo.TabIndex = 39;
			this.dtThresholdTo.Tag = "";
			this.dtThresholdTo.Value = new System.DateTime(2005, 7, 28, 0, 0, 0, 0);
			this.dtThresholdTo.ValueChanged += new System.EventHandler(this.dtThresholdTo_ValueChanged);
			// 
			// dtThresholdFrom
			// 
			this.dtThresholdFrom.CustomFormat = "dd MMM yyyy";
			this.dtThresholdFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtThresholdFrom.Location = new System.Drawing.Point(248, 40);
			this.dtThresholdFrom.Name = "dtThresholdFrom";
			this.dtThresholdFrom.Size = new System.Drawing.Size(112, 20);
			this.dtThresholdFrom.TabIndex = 38;
			this.dtThresholdFrom.Tag = "";
			this.dtThresholdFrom.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
			this.dtThresholdFrom.ValueChanged += new System.EventHandler(this.dtThresholdFrom_ValueChanged);
			// 
			// lTo
			// 
			this.lTo.Location = new System.Drawing.Point(200, 64);
			this.lTo.Name = "lTo";
			this.lTo.Size = new System.Drawing.Size(32, 24);
			this.lTo.TabIndex = 37;
			this.lTo.Text = "Until";
			this.lTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lFrom
			// 
			this.lFrom.Location = new System.Drawing.Point(8, 40);
			this.lFrom.Name = "lFrom";
			this.lFrom.Size = new System.Drawing.Size(232, 24);
			this.lFrom.TabIndex = 36;
			this.lFrom.Text = "Date Acct Reached Delivery Threshold From";
			this.lFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lBranch
			// 
			this.lBranch.Location = new System.Drawing.Point(8, 456);
			this.lBranch.Name = "lBranch";
			this.lBranch.Size = new System.Drawing.Size(120, 16);
			this.lBranch.TabIndex = 2;
			this.lBranch.Text = "View Report by Branch";
			this.lBranch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// drpBranchNo
			// 
			this.drpBranchNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpBranchNo.Location = new System.Drawing.Point(136, 456);
			this.drpBranchNo.Name = "drpBranchNo";
			this.drpBranchNo.Size = new System.Drawing.Size(184, 21);
			this.drpBranchNo.TabIndex = 0;
			this.drpBranchNo.SelectionChangeCommitted += new System.EventHandler(this.drpBranchNo_SelectionChangeCommitted);
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(688, 48);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(72, 23);
			this.btnClear.TabIndex = 40;
			this.btnClear.Text = "Clear";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(104, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(136, 24);
			this.label1.TabIndex = 2;
			this.label1.Text = "New Accts Opened From";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// dtAcctsOpened
			// 
			this.dtAcctsOpened.CustomFormat = "dd MMM yyyy";
			this.dtAcctsOpened.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtAcctsOpened.Location = new System.Drawing.Point(248, 16);
			this.dtAcctsOpened.Name = "dtAcctsOpened";
			this.dtAcctsOpened.Size = new System.Drawing.Size(112, 20);
			this.dtAcctsOpened.TabIndex = 39;
			this.dtAcctsOpened.Tag = "";
			this.dtAcctsOpened.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
			this.dtAcctsOpened.ValueChanged += new System.EventHandler(this.dtAcctsOpened_ValueChanged);
			// 
			// dtRebateCalc
			// 
			this.dtRebateCalc.CustomFormat = "dd MMM yyyy";
			this.dtRebateCalc.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtRebateCalc.Location = new System.Drawing.Point(504, 16);
			this.dtRebateCalc.Name = "dtRebateCalc";
			this.dtRebateCalc.Size = new System.Drawing.Size(112, 20);
			this.dtRebateCalc.TabIndex = 39;
			this.dtRebateCalc.Tag = "";
			this.dtRebateCalc.Value = new System.DateTime(2005, 7, 28, 0, 0, 0, 0);
			this.dtRebateCalc.ValueChanged += new System.EventHandler(this.dtRebateCalc_ValueChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(368, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(128, 24);
			this.label2.TabIndex = 2;
			this.label2.Text = "Rebate Calculation Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// drpRuleChangeDate
			// 
			this.drpRuleChangeDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpRuleChangeDate.Items.AddRange(new object[] {
																   "01/04/2002",
																   "01/01/2003"});
			this.drpRuleChangeDate.Location = new System.Drawing.Point(504, 40);
			this.drpRuleChangeDate.Name = "drpRuleChangeDate";
			this.drpRuleChangeDate.Size = new System.Drawing.Size(112, 21);
			this.drpRuleChangeDate.TabIndex = 0;
			this.drpRuleChangeDate.SelectionChangeCommitted += new System.EventHandler(this.drpRuleChangeDate_SelectionChangeCommitted);
			// 
			// lbRuleChangeDate
			// 
			this.lbRuleChangeDate.Location = new System.Drawing.Point(392, 40);
			this.lbRuleChangeDate.Name = "lbRuleChangeDate";
			this.lbRuleChangeDate.Size = new System.Drawing.Size(104, 24);
			this.lbRuleChangeDate.TabIndex = 2;
			this.lbRuleChangeDate.Text = "Rule Change Date";
			this.lbRuleChangeDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// contextMenu1
			// 
			this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.cmenuComments});
			// 
			// cmenuComments
			// 
			this.cmenuComments.Index = 0;
			this.cmenuComments.Text = "View Comments";
			// 
			// RebateCalculation
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(792, 477);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.gbDeliveries});
			this.Name = "RebateCalculation";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Rebate Calculation Frame";
			this.Load += new System.EventHandler(this.RebateCalculation_Load);
			this.gbDeliveries.ResumeLayout(false);
            this.gbDeliveries.PerformLayout();
			this.gbResults.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgCountryData)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgBranchData)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

        /// <summary>
        /// Set all screen controls to their default values.
        /// </summary>
        private void Clear()
        {
            // Initial custom settings
            // Set to this user's branch
            drpBranchNo.SelectedValue = Config.BranchCode;
            this.dtThresholdFrom.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtThresholdTo.Value = DateTime.Today;
            this.dtAcctsOpened.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtRebateCalc.Value = DateTime.Today;
            this.drpRuleChangeDate.SelectedIndex = 0;
            ClearGrid();
            ((MainForm)this.FormRoot).statusBar1.Text = "";
        }

        /// <summary>
        /// Populate dropdowns and retrieve any static data that can be preserved for the
        /// duration of this forms usage.
        /// </summary>
        private void LoadStaticData()
        {
            this.dtThresholdFrom.MaxDate = DateTime.Today;
            this.dtThresholdTo.MaxDate = DateTime.Today;
            this.dtAcctsOpened.MaxDate = DateTime.Today;

            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
				
            if(StaticData.Tables[TN.BranchNumber]==null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.BranchNumber, null));
			
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

            //Now customise the dropdowns..         

            //Customise the Branch data to be displayed in the dropdown..
            if (_branchData == null)
            {
                _branchData = ((DataTable)StaticData.Tables[TN.BranchNumber]).Clone();

                DataRow row = _branchData.NewRow();
                row[CN.BranchNo] = -1;
                row[CN.BranchName] = GetResource("AllValues");
                _branchData.Rows.Add(row);
                foreach (DataRow copyRow in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                {
                    row = _branchData.NewRow();
                    row[CN.BranchNo] = copyRow[CN.BranchNo];
                    row[CN.BranchName] = copyRow[CN.BranchNo].ToString() 
                        + " : " + copyRow[CN.BranchName].ToString();
                    _branchData.Rows.Add(row);
                }
            }
       
            drpBranchNo.DataSource = _branchData;
            drpBranchNo.ValueMember = CN.BranchNo;
            drpBranchNo.DisplayMember = CN.BranchName;
            drpBranchNo.SelectedValue = Config.BranchCode;
            // CR938 Straight Line rule change date - only for St.Lucia     jec 03/04/08
            if ((string)Country[CountryParameterNames.CountryCode] == "L ")
                dtpSLRuleChange.Value = Convert.ToDateTime(Country[CountryParameterNames.SLRebateCalculationRuleDate]);
            else
            {
                dtpSLRuleChange.Visible = false;
                lbSLRuleChange.Visible = false;
            }
        }

        /// <summary>
        /// Populate the DataGrid based on current selection criteria.
        /// </summary>
        private void LoadRebateData()
        {
            bool branchDataFound = false;
            bool countryDataFound = false;
            DataSet rebateDataSet = null;
            DataView branchDataListView = null;
            DataView countryDataListView = null;

            try 
            {
                Wait();
                // Abort if the Start Date is after the End Date
                if (this.dtThresholdFrom.Value > this.dtThresholdTo.Value)
                {
                    ShowInfo("M_STARTDATEAFTEREND");
                    StopWait();
                    return;
                }

                int branch = -1;
                if (drpBranchNo.SelectedIndex > 0)//ie. Not 'All'
                {
                    branch = Int32.Parse(drpBranchNo.SelectedValue.ToString());
                }


                rebateDataSet = AccountManager.GetRebatesTotal(branch,out error);

                if(error.Length > 0)
                {
                    ShowError(error);
                    StopWait();
                    return;
                }

                foreach (DataTable dt in rebateDataSet.Tables)
                {
                    if (dt.TableName == TN.RebatesByBranch)
                    {
                        branchDataFound = (dt.Rows.Count > 0);

                        if (branchDataFound) 
                        {
                            btnExcelBranch.Enabled = true;
                            branchDataListView = new DataView(dt);
                            dgBranchData.DataSource = branchDataListView;
                            dgBranchData.ReadOnly = true;
                            branchDataListView.AllowNew = false;

                            if (dgBranchData.TableStyles.Count == 0)
                            {
                                DataGridTableStyle tabStyle = new DataGridTableStyle();
                                tabStyle.MappingName = branchDataListView.Table.TableName;

                                dgBranchData.TableStyles.Clear();
                                dgBranchData.TableStyles.Add(tabStyle);
                                dgBranchData.DataSource = branchDataListView;

                                // Set column display preferences that are specific to BRANCH
                                tabStyle.GridColumnStyles[CN.BranchNo].Width = 80;
                                tabStyle.GridColumnStyles[CN.ArrearsGroup].Width = 140;
                                tabStyle.GridColumnStyles[CN.Rebate.ToLower()].Width = 100;
                                tabStyle.GridColumnStyles[CN.RebateWithin12Mths].Width = 160;
                                tabStyle.GridColumnStyles[CN.RebateAfter12Mths].Width = 160;
                                tabStyle.GridColumnStyles[CN.BranchNo].HeaderText = GetResource("T_BRANCH");

                                SetCommonTabStyleDetails(tabStyle);
                            }
                        }
                        else 
                        {
                            btnExcelBranch.Enabled = false;
                        }
                    }
                    else if (dt.TableName == TN.RebatesTotals)
                    {
                        countryDataFound = (dt.Rows.Count > 0);

                        if (countryDataFound) 
                        {
                            btnExcelCountry.Enabled = true;
                            countryDataListView = new DataView(dt);
                            dgCountryData.DataSource = countryDataListView;
                            dgCountryData.ReadOnly = true;
                            countryDataListView.AllowNew = false;

                            if (dgCountryData.TableStyles.Count == 0)
                            {
                                DataGridTableStyle tabStyle = new DataGridTableStyle();
                                tabStyle.MappingName = countryDataListView.Table.TableName;

                                dgCountryData.TableStyles.Clear();
                                dgCountryData.TableStyles.Add(tabStyle);
                                dgCountryData.DataSource = countryDataListView;

                                // Set column display preferences that are specific to COUNTRY
                                tabStyle.GridColumnStyles[CN.Sequence].Width = 0;
                                tabStyle.GridColumnStyles[CN.ArrearsGroup].Width = 220;
                                tabStyle.GridColumnStyles[CN.Rebate.ToLower()].Width = 100;
                                tabStyle.GridColumnStyles[CN.RebateWithin12Mths].Width = 160;
                                tabStyle.GridColumnStyles[CN.RebateAfter12Mths].Width = 160;
                                SetCommonTabStyleDetails(tabStyle);
                            }
                        }
                        else 
                        {
                            btnExcelCountry.Enabled = false;
                        }                    
                    }
                }
                DateTime asAtDate = AccountManager.GetRebatesAsAt(out error);
                if(error.Length > 0)
                {
                    ShowError(error);
                    StopWait();
                    return;
                }
                else
                {
                    txtAsAtDate.Text = asAtDate.ToShortDateString();
                }
            }
            catch(Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }	
        }

        //
        // Events
        //

        //This method is called once, just before the screen is displayed.
        private void RebateCalculation_Load(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Rebate Calculation screen: Form Load";
                Wait();

                this.LoadStaticData();
                if (Config.CountryCode == CC.Indonesia)
                {
                    drpRuleChangeDate.Enabled = true;
                }
                else
                {
                    drpRuleChangeDate.Enabled = false;
                }
                Clear();
                LoadRebateData();
                // Initial focus
                btnCalculate.Focus();
            }
            catch(Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        private void btnCalculate_Click(object sender, System.EventArgs e)
        {
            // Create the Rebates_Totals data and select it for display on the screen.
            try
            {
                Function = "Rebate Calculation Screen: Retrieve Data";
                Wait();
                ClearGrid();
                //Update the database, calculating rebate totals based on the screen dates
                UpdateTotals();
                //Now retrieve the calculated data from the Rebates_Total table.
                LoadRebateData();
            }
            catch(Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }
 
        private void drpBranchNo_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            // Selected Branch has changed, just reselect and display the data
            try
            {
                Function = "Rebate Calculation Screen: Retrieve Data for changed branch";
                Wait();
                ClearGrid();
                LoadRebateData();
            }
            catch(Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        private void fileExit_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Rebate Calculation Screen: File - Exit";
                Wait();
                CloseTab();
            }
            catch(Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            Clear();
        }

        private void dtThresholdFrom_ValueChanged(object sender, System.EventArgs e)
        {
            ClearGrid();        
        }

        private void dtThresholdTo_ValueChanged(object sender, System.EventArgs e)
        {
            ClearGrid();
        }

        private void ClearGrid()
        {
            ClearControls(gbResults.Controls);
            //We may need to change what is displayed or hidden in response
            //to this change in selection so clear the TableStyles so a new
            //entry (and therefore grid layout) will be created.
            dgBranchData.TableStyles.Clear();
            dgCountryData.TableStyles.Clear();
            btnExcelBranch.Enabled = false;
            btnExcelCountry.Enabled = false;
        }

        private void btnExcelBranch_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                /* save the current data grid contents to a CSV */
                string comma = ",";
                string path = "";

                if(dgBranchData.CurrentRowIndex >= 0)
                {
                    DataView dv = (DataView)dgBranchData.DataSource;

                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*" ;
                    save.Title = "Save Branch Rebate Calculation Data";
                    save.CreatePrompt = true;

                    if(save.ShowDialog() == DialogResult.OK)
                    {
                        path = save.FileName;
                        FileInfo fi = new FileInfo(path);
                        FileStream fs = fi.OpenWrite();
			
                        //Write heading line..
                        string line = string.Empty;

                        line = line + GetResource("T_BRANCH") + comma + 
                            GetResource("T_MONTHSARREARS") + comma + 
                            GetResource("T_REBATE") + comma + 
                            GetResource("T_REBATEWITHIN12MTHS") + comma + 
                            GetResource("T_REBATEAFTER12MTHS") + Environment.NewLine + Environment.NewLine;	

                        byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
                        fs.Write(blob,0,blob.Length);
			
                        foreach(DataRowView row in dv)
                        {					
                            line = string.Empty;

                            line +=	row[CN.BranchNo].ToString() + comma +
                                row[CN.ArrearsGroup].ToString() + comma + 
                                (Convert.ToDecimal(row[CN.Rebate.ToLower()])).ToString(DecimalPlaces).Replace(",","") + comma +
                                (Convert.ToDecimal(row[CN.RebateWithin12Mths])).ToString(DecimalPlaces).Replace(",","") + comma +
                                (Convert.ToDecimal(row[CN.RebateAfter12Mths])).ToString(DecimalPlaces).Replace(",","") + 
                                Environment.NewLine;

                            blob = System.Text.Encoding.UTF8.GetBytes(line);
                            fs.Write(blob,0,blob.Length);
                        }
                        fs.Close();						
                        LaunchExcel(path);
                    }
                }
            }
            catch(Exception ex)
            {
                Catch(ex, Function);
            }	
            finally
            {
                StopWait();
            }
        }

        private void btnExcelCountry_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                /* save the current data grid contents to a CSV */
                string comma = ",";
                string path = "";

                if(dgCountryData.CurrentRowIndex >= 0)
                {
                    DataView dv = (DataView)dgCountryData.DataSource;

                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*" ;
                    save.Title = "Save Rebate Calculation Totals";
                    save.CreatePrompt = true;

                    if(save.ShowDialog() == DialogResult.OK)
                    {
                        path = save.FileName;
                        FileInfo fi = new FileInfo(path);
                        FileStream fs = fi.OpenWrite();
			
                        //Write heading line..
                        string line = string.Empty;

                        line = line + GetResource("T_MONTHSARREARS") + comma + 
                            GetResource("T_REBATE") + comma + 
                            GetResource("T_REBATEWITHIN12MTHS") + comma + 
                            GetResource("T_REBATEAFTER12MTHS") + Environment.NewLine + Environment.NewLine;	

                        byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
                        fs.Write(blob,0,blob.Length);
			
                        foreach(DataRowView row in dv)
                        {					
                            line = string.Empty;

                            line +=	row[CN.ArrearsGroup].ToString() + comma + 
                                (Convert.ToDecimal(row[CN.Rebate.ToLower()])).ToString(DecimalPlaces).Replace(",","") + comma +
                                (Convert.ToDecimal(row[CN.RebateWithin12Mths])).ToString(DecimalPlaces).Replace(",","") + comma +
                                (Convert.ToDecimal(row[CN.RebateAfter12Mths])).ToString(DecimalPlaces).Replace(",","") + 
                                Environment.NewLine;

                            blob = System.Text.Encoding.UTF8.GetBytes(line);
                            fs.Write(blob,0,blob.Length);
                        }
                        fs.Close();						
                        LaunchExcel(path);
                     }
                }
            }
            catch(Exception ex)
            {
                Catch(ex, Function);
            }	
            finally
            {
                StopWait();
            }        
        }

        private void dtAcctsOpened_ValueChanged(object sender, System.EventArgs e)
        {
            ClearGrid();
        }

        private void dtRebateCalc_ValueChanged(object sender, System.EventArgs e)
        {
            ClearGrid();
        }

        private void drpRuleChangeDate_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            ClearGrid();
        }

        private void SetCommonTabStyleDetails(DataGridTableStyle tabStyle)
        {
            //Headers
            tabStyle.GridColumnStyles[CN.ArrearsGroup].HeaderText = GetResource("T_MONTHSARREARS");
            tabStyle.GridColumnStyles[CN.Rebate.ToLower()].HeaderText = GetResource("T_REBATE");
            tabStyle.GridColumnStyles[CN.RebateWithin12Mths].HeaderText = GetResource("T_REBATEWITHIN12MTHS");
            tabStyle.GridColumnStyles[CN.RebateAfter12Mths].HeaderText = GetResource("T_REBATEAFTER12MTHS");
            //Formatting
            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Rebate.ToLower()]).Format = DecimalPlaces;
            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.RebateWithin12Mths]).Format = DecimalPlaces;
            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.RebateAfter12Mths]).Format = DecimalPlaces;
            //Alignment
            tabStyle.GridColumnStyles[CN.Rebate.ToLower()].Alignment = HorizontalAlignment.Right;
            tabStyle.GridColumnStyles[CN.RebateWithin12Mths].Alignment = HorizontalAlignment.Right;
            tabStyle.GridColumnStyles[CN.RebateAfter12Mths].Alignment = HorizontalAlignment.Right;
        }

        /// <summary>
        /// Makes a call to the AccountManager web service to calculate new rebate totals
        /// and insert them into the database.
        /// </summary>
        private void UpdateTotals()
        {
            //The acctNo parameter expected by the stored procedure needs to be
            //set to 'all' for this screen - not account specific.
            string all = GetResource("AllValues").ToLower();

            //These values are returned as output parameters from the stored procedure.
            //We don't actually need them for this screen.
            decimal poRebate = 0;
            decimal poRebateWithin12Mths = 0;
            decimal poRebateAfter12Mths = 0;
            try 
            {
                Wait();
                DateTime dtRuleChangeDate = new DateTime(2002,4,1);
                if (drpRuleChangeDate.SelectedIndex == 1)
                {
                    dtRuleChangeDate = new DateTime(2003,1,1);
                }
                AccountManager.UpdateRebateTotals(all,dtThresholdFrom.Value,dtThresholdTo.Value,
                    dtAcctsOpened.Value,dtRuleChangeDate,dtRebateCalc.Value,
                    out poRebate,out poRebateWithin12Mths,out poRebateAfter12Mths,out error);
                if(error.Length > 0)
                {
                    ShowError(error);
                    StopWait();
                    return;
                }
// Left for debug purposes-->
//                MessageBox.Show("Update called successfully, returned values : "
//                    + poRebate.ToString() + ", " + poRebateWithin12Mths.ToString()
//                    + ", " + poRebateAfter12Mths.ToString());
            }
            catch(Exception ex)
            {
                Catch(ex, Function);
            }	
            finally
            {
                StopWait();
            } 
        }

        private void LaunchExcel(string path)
        {
            string comma = ",";
            /* attempt to launch Excel. May get a COMException if Excel is not 
                            * installed */
            try
            {
                /* we have to use Reflection to call the Open method because 
                                * the methods have different argument lists for the 
                                * different versions of Excel - JJ */
                object[] args = null;
                Excel.Application excel = new Excel.Application();

                if(excel.Version == "10.0")	/* Excel2002 */
                    args = new object[]{path, 0, true, 2, "", "", true, Excel.XlPlatform.xlWindows, comma, true, false, 0, true, false, false };
                else
                    args = new object[]{path, 0, true, 2, "", "", true, Excel.XlPlatform.xlWindows, comma, true, false, 0, true };

                /* Retrieve the Workbooks property */
                object wbs = excel.GetType().InvokeMember("Workbooks", BindingFlags.Public|BindingFlags.GetField|BindingFlags.GetProperty, null, excel, new Object[] {});

                /* call the Open method */
                object wb = wbs.GetType().InvokeMember("Open", BindingFlags.Public | BindingFlags.InvokeMethod, null, wbs, args);

                excel.Visible = true;
            }
            catch(COMException)
            {
                /*change back slashes to forward slashes so the path doesn't
                                * get split into multiple lines */
                ShowInfo("M_EXCELNOTFOUND", new object[] {path.Replace("\\", "/")});
            }
        }
    }
}