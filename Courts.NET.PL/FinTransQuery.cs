using System;
using STL.PL.WS2;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using Crownwood.Magic.Menus;
using System.Windows.Forms;
using System.Web.Services.Protocols;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace STL.PL
{
    /// <summary>
    /// Reporting screen to list financial transactions that the user
    /// can filter on criteria such as: date range; transaction type;
    /// branch; employee number and run number.
    /// </summary>
    public class FinTransQuery : CommonForm
    {
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnClose;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.DateTimePicker dtEnd;
        private System.Windows.Forms.DateTimePicker dtStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private IContainer components;
        private System.Windows.Forms.Label menuInstantReplacement;
        private new string Error = "";
        private Crownwood.Magic.Controls.TabControl tcMain;
        private System.Windows.Forms.Label label63;
        public System.Windows.Forms.DataGrid dgTransactions;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Label label54;
        private Crownwood.Magic.Controls.TabPage tpResults;
        private Crownwood.Magic.Controls.TabPage tpQuery;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtTransactionTypes;
        private System.Windows.Forms.Button btnTransTypes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox drpLimitValues;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnBranches;
        public System.Windows.Forms.TextBox txtBranches;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnEmployees;
        private System.Windows.Forms.ComboBox drpAcctsInBranch;
        private System.Windows.Forms.Label lbRunNoTo;
        private System.Windows.Forms.NumericUpDown numSecondRunNo;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown numFirstRunNo;
        private System.Windows.Forms.ComboBox drpRunNoCriteria;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGrid dgResults;
        private System.Windows.Forms.NumericUpDown numFirstTransRefNo;
        private System.Windows.Forms.Label lbTransRefNoTo;
        private System.Windows.Forms.NumericUpDown numSecondTransRefNo;
        private System.Windows.Forms.ComboBox drpTransRefNoCriteria;
        private System.Windows.Forms.Label lbBetweenDates;
        private System.Windows.Forms.ComboBox drpDateCriteria;
        private System.Windows.Forms.ComboBox drpTransTypeCriteria;
        private System.Windows.Forms.ComboBox drpTransTypes;
        private System.Windows.Forms.ComboBox drpBranchCriteria;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox drpBranches;
        private System.Windows.Forms.CheckBox chxIncludeInterest;
        private System.Windows.Forms.CheckBox chxValueOnly;
        public System.Windows.Forms.TextBox txtEmployees;
        private System.Windows.Forms.ComboBox drpEmployeeCriteria;
        private System.Windows.Forms.ComboBox drpEmployees;
        public System.Windows.Forms.Button btnExcel;
	   	private string error = "";
        private string _noSetSelection;


        public FinTransQuery(TranslationDummy d )
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
			// DSR - don't do this here so we can see the form in design view
            //_noSetSelection = GetResource("NoSetSpecified");
        }

        public FinTransQuery()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            ApplyRoleRestrictions();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            dtStart.Value = DateTime.Today;
            dtEnd.Value = DateTime.Today;
			// DSR - don't do this here so we can see the form in design view
			//_noSetSelection = GetResource("NoSetSpecified");
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

		#region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FinTransQuery));
            this.tcMain = new Crownwood.Magic.Controls.TabControl();
            this.tpQuery = new Crownwood.Magic.Controls.TabPage();
            this.btnSearch = new System.Windows.Forms.Button();
            this.chxIncludeInterest = new System.Windows.Forms.CheckBox();
            this.numFirstTransRefNo = new System.Windows.Forms.NumericUpDown();
            this.drpLimitValues = new System.Windows.Forms.ComboBox();
            this.btnTransTypes = new System.Windows.Forms.Button();
            this.txtTransactionTypes = new System.Windows.Forms.TextBox();
            this.dtStart = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.dtEnd = new System.Windows.Forms.DateTimePicker();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lbBetweenDates = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnBranches = new System.Windows.Forms.Button();
            this.txtBranches = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lbTransRefNoTo = new System.Windows.Forms.Label();
            this.numSecondTransRefNo = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.btnEmployees = new System.Windows.Forms.Button();
            this.drpAcctsInBranch = new System.Windows.Forms.ComboBox();
            this.lbRunNoTo = new System.Windows.Forms.Label();
            this.numSecondRunNo = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.numFirstRunNo = new System.Windows.Forms.NumericUpDown();
            this.drpRunNoCriteria = new System.Windows.Forms.ComboBox();
            this.drpTransRefNoCriteria = new System.Windows.Forms.ComboBox();
            this.drpDateCriteria = new System.Windows.Forms.ComboBox();
            this.drpTransTypeCriteria = new System.Windows.Forms.ComboBox();
            this.drpTransTypes = new System.Windows.Forms.ComboBox();
            this.drpBranchCriteria = new System.Windows.Forms.ComboBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.drpBranches = new System.Windows.Forms.ComboBox();
            this.chxValueOnly = new System.Windows.Forms.CheckBox();
            this.txtEmployees = new System.Windows.Forms.TextBox();
            this.drpEmployeeCriteria = new System.Windows.Forms.ComboBox();
            this.drpEmployees = new System.Windows.Forms.ComboBox();
            this.tpResults = new Crownwood.Magic.Controls.TabPage();
            this.btnExcel = new System.Windows.Forms.Button();
            this.dgResults = new System.Windows.Forms.DataGrid();
            this.menuInstantReplacement = new System.Windows.Forms.Label();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.label63 = new System.Windows.Forms.Label();
            this.dgTransactions = new System.Windows.Forms.DataGrid();
            this.label45 = new System.Windows.Forms.Label();
            this.label54 = new System.Windows.Forms.Label();
            this.tcMain.SuspendLayout();
            this.tpQuery.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFirstTransRefNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSecondTransRefNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSecondRunNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFirstRunNo)).BeginInit();
            this.tpResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgTransactions)).BeginInit();
            this.SuspendLayout();
            // 
            // tcMain
            // 
            this.tcMain.IDEPixelArea = true;
            this.tcMain.Location = new System.Drawing.Point(0, 0);
            this.tcMain.Name = "tcMain";
            this.tcMain.PositionTop = true;
            this.tcMain.SelectedIndex = 0;
            this.tcMain.SelectedTab = this.tpQuery;
            this.tcMain.Size = new System.Drawing.Size(792, 480);
            this.tcMain.TabIndex = 45;
            this.tcMain.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpQuery,
            this.tpResults});
            // 
            // tpQuery
            // 
            this.tpQuery.Controls.Add(this.btnSearch);
            this.tpQuery.Controls.Add(this.chxIncludeInterest);
            this.tpQuery.Controls.Add(this.numFirstTransRefNo);
            this.tpQuery.Controls.Add(this.drpLimitValues);
            this.tpQuery.Controls.Add(this.btnTransTypes);
            this.tpQuery.Controls.Add(this.txtTransactionTypes);
            this.tpQuery.Controls.Add(this.dtStart);
            this.tpQuery.Controls.Add(this.label4);
            this.tpQuery.Controls.Add(this.dtEnd);
            this.tpQuery.Controls.Add(this.btnClear);
            this.tpQuery.Controls.Add(this.btnClose);
            this.tpQuery.Controls.Add(this.lbBetweenDates);
            this.tpQuery.Controls.Add(this.label2);
            this.tpQuery.Controls.Add(this.label3);
            this.tpQuery.Controls.Add(this.label5);
            this.tpQuery.Controls.Add(this.btnBranches);
            this.tpQuery.Controls.Add(this.txtBranches);
            this.tpQuery.Controls.Add(this.label6);
            this.tpQuery.Controls.Add(this.label7);
            this.tpQuery.Controls.Add(this.lbTransRefNoTo);
            this.tpQuery.Controls.Add(this.numSecondTransRefNo);
            this.tpQuery.Controls.Add(this.label9);
            this.tpQuery.Controls.Add(this.btnEmployees);
            this.tpQuery.Controls.Add(this.drpAcctsInBranch);
            this.tpQuery.Controls.Add(this.lbRunNoTo);
            this.tpQuery.Controls.Add(this.numSecondRunNo);
            this.tpQuery.Controls.Add(this.label11);
            this.tpQuery.Controls.Add(this.numFirstRunNo);
            this.tpQuery.Controls.Add(this.drpRunNoCriteria);
            this.tpQuery.Controls.Add(this.drpTransRefNoCriteria);
            this.tpQuery.Controls.Add(this.drpDateCriteria);
            this.tpQuery.Controls.Add(this.drpTransTypeCriteria);
            this.tpQuery.Controls.Add(this.drpTransTypes);
            this.tpQuery.Controls.Add(this.drpBranchCriteria);
            this.tpQuery.Controls.Add(this.comboBox1);
            this.tpQuery.Controls.Add(this.drpBranches);
            this.tpQuery.Controls.Add(this.chxValueOnly);
            this.tpQuery.Controls.Add(this.txtEmployees);
            this.tpQuery.Controls.Add(this.drpEmployeeCriteria);
            this.tpQuery.Controls.Add(this.drpEmployees);
            this.tpQuery.Location = new System.Drawing.Point(0, 25);
            this.tpQuery.Name = "tpQuery";
            this.tpQuery.Size = new System.Drawing.Size(792, 455);
            this.tpQuery.TabIndex = 5;
            this.tpQuery.Title = "Query";
            // 
            // btnSearch
            // 
            this.btnSearch.CausesValidation = false;
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSearch.Location = new System.Drawing.Point(576, 40);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(72, 23);
            this.btnSearch.TabIndex = 49;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // chxIncludeInterest
            // 
            this.chxIncludeInterest.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chxIncludeInterest.Checked = true;
            this.chxIncludeInterest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chxIncludeInterest.Location = new System.Drawing.Point(216, 360);
            this.chxIncludeInterest.Name = "chxIncludeInterest";
            this.chxIncludeInterest.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chxIncludeInterest.Size = new System.Drawing.Size(312, 24);
            this.chxIncludeInterest.TabIndex = 48;
            this.chxIncludeInterest.Text = "Include Interest and Admin Charges Posted in this Period";
            this.chxIncludeInterest.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numFirstTransRefNo
            // 
            this.numFirstTransRefNo.Location = new System.Drawing.Point(288, 240);
            this.numFirstTransRefNo.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numFirstTransRefNo.Name = "numFirstTransRefNo";
            this.numFirstTransRefNo.Size = new System.Drawing.Size(80, 21);
            this.numFirstTransRefNo.TabIndex = 47;
            this.numFirstTransRefNo.Visible = false;
            // 
            // drpLimitValues
            // 
            this.drpLimitValues.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpLimitValues.Items.AddRange(new object[] {
            "",
            ">= 0",
            "< 0"});
            this.drpLimitValues.Location = new System.Drawing.Point(216, 120);
            this.drpLimitValues.Name = "drpLimitValues";
            this.drpLimitValues.Size = new System.Drawing.Size(64, 21);
            this.drpLimitValues.TabIndex = 46;
            // 
            // btnTransTypes
            // 
            this.btnTransTypes.Location = new System.Drawing.Point(520, 80);
            this.btnTransTypes.Name = "btnTransTypes";
            this.btnTransTypes.Size = new System.Drawing.Size(24, 23);
            this.btnTransTypes.TabIndex = 45;
            this.btnTransTypes.Text = "...";
            this.btnTransTypes.Visible = false;
            this.btnTransTypes.Click += new System.EventHandler(this.btnTransTypes_Click);
            // 
            // txtTransactionTypes
            // 
            this.txtTransactionTypes.Location = new System.Drawing.Point(288, 107);
            this.txtTransactionTypes.Name = "txtTransactionTypes";
            this.txtTransactionTypes.ReadOnly = true;
            this.txtTransactionTypes.Size = new System.Drawing.Size(224, 21);
            this.txtTransactionTypes.TabIndex = 44;
            this.txtTransactionTypes.Visible = false;
            // 
            // dtStart
            // 
            this.dtStart.CustomFormat = "ddd dd MMM yyyy";
            this.dtStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtStart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtStart.Location = new System.Drawing.Point(288, 40);
            this.dtStart.Name = "dtStart";
            this.dtStart.Size = new System.Drawing.Size(112, 21);
            this.dtStart.TabIndex = 2;
            this.dtStart.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtStart.Visible = false;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(56, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(152, 24);
            this.label4.TabIndex = 43;
            this.label4.Text = "Select Transactions with Date  ";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtEnd
            // 
            this.dtEnd.CustomFormat = "ddd dd MMM yyyy";
            this.dtEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtEnd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtEnd.Location = new System.Drawing.Point(432, 40);
            this.dtEnd.Name = "dtEnd";
            this.dtEnd.Size = new System.Drawing.Size(112, 21);
            this.dtEnd.TabIndex = 3;
            this.dtEnd.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtEnd.Visible = false;
            // 
            // btnClear
            // 
            this.btnClear.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClear.Location = new System.Drawing.Point(576, 120);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "&Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnClose
            // 
            this.btnClose.CausesValidation = false;
            this.btnClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClose.Location = new System.Drawing.Point(576, 80);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "&Exit";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lbBetweenDates
            // 
            this.lbBetweenDates.Location = new System.Drawing.Point(392, 40);
            this.lbBetweenDates.Name = "lbBetweenDates";
            this.lbBetweenDates.Size = new System.Drawing.Size(32, 24);
            this.lbBetweenDates.TabIndex = 43;
            this.lbBetweenDates.Text = "To";
            this.lbBetweenDates.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbBetweenDates.Visible = false;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(72, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 24);
            this.label2.TabIndex = 43;
            this.label2.Text = "For Transaction Types";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(72, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 24);
            this.label3.TabIndex = 43;
            this.label3.Text = "With Values";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(72, 160);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(136, 24);
            this.label5.TabIndex = 43;
            this.label5.Text = "For Branches";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnBranches
            // 
            this.btnBranches.Location = new System.Drawing.Point(520, 160);
            this.btnBranches.Name = "btnBranches";
            this.btnBranches.Size = new System.Drawing.Size(24, 23);
            this.btnBranches.TabIndex = 45;
            this.btnBranches.Text = "...";
            this.btnBranches.Visible = false;
            this.btnBranches.Click += new System.EventHandler(this.btnBranches_Click);
            // 
            // txtBranches
            // 
            this.txtBranches.Location = new System.Drawing.Point(288, 160);
            this.txtBranches.Name = "txtBranches";
            this.txtBranches.ReadOnly = true;
            this.txtBranches.Size = new System.Drawing.Size(224, 21);
            this.txtBranches.TabIndex = 44;
            this.txtBranches.Visible = false;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(56, 200);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(152, 24);
            this.label6.TabIndex = 43;
            this.label6.Text = "For Accounts in Branch";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(72, 240);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(136, 24);
            this.label7.TabIndex = 43;
            this.label7.Text = "With Transaction Ref No";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbTransRefNoTo
            // 
            this.lbTransRefNoTo.Location = new System.Drawing.Point(368, 240);
            this.lbTransRefNoTo.Name = "lbTransRefNoTo";
            this.lbTransRefNoTo.Size = new System.Drawing.Size(32, 24);
            this.lbTransRefNoTo.TabIndex = 43;
            this.lbTransRefNoTo.Text = "To";
            this.lbTransRefNoTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbTransRefNoTo.Visible = false;
            // 
            // numSecondTransRefNo
            // 
            this.numSecondTransRefNo.Location = new System.Drawing.Point(408, 240);
            this.numSecondTransRefNo.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numSecondTransRefNo.Name = "numSecondTransRefNo";
            this.numSecondTransRefNo.Size = new System.Drawing.Size(80, 21);
            this.numSecondTransRefNo.TabIndex = 47;
            this.numSecondTransRefNo.Value = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numSecondTransRefNo.Visible = false;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(72, 280);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(136, 24);
            this.label9.TabIndex = 43;
            this.label9.Text = "For Courtspersons";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnEmployees
            // 
            this.btnEmployees.Location = new System.Drawing.Point(520, 280);
            this.btnEmployees.Name = "btnEmployees";
            this.btnEmployees.Size = new System.Drawing.Size(24, 23);
            this.btnEmployees.TabIndex = 45;
            this.btnEmployees.Text = "...";
            this.btnEmployees.Click += new System.EventHandler(this.btnEmployees_Click);
            // 
            // drpAcctsInBranch
            // 
            this.drpAcctsInBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpAcctsInBranch.Location = new System.Drawing.Point(216, 200);
            this.drpAcctsInBranch.Name = "drpAcctsInBranch";
            this.drpAcctsInBranch.Size = new System.Drawing.Size(64, 21);
            this.drpAcctsInBranch.TabIndex = 46;
            // 
            // lbRunNoTo
            // 
            this.lbRunNoTo.Location = new System.Drawing.Point(376, 320);
            this.lbRunNoTo.Name = "lbRunNoTo";
            this.lbRunNoTo.Size = new System.Drawing.Size(24, 24);
            this.lbRunNoTo.TabIndex = 43;
            this.lbRunNoTo.Text = "To";
            this.lbRunNoTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbRunNoTo.Visible = false;
            // 
            // numSecondRunNo
            // 
            this.numSecondRunNo.Location = new System.Drawing.Point(408, 320);
            this.numSecondRunNo.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numSecondRunNo.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSecondRunNo.Name = "numSecondRunNo";
            this.numSecondRunNo.Size = new System.Drawing.Size(80, 21);
            this.numSecondRunNo.TabIndex = 47;
            this.numSecondRunNo.Value = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numSecondRunNo.Visible = false;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(72, 320);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(136, 24);
            this.label11.TabIndex = 43;
            this.label11.Text = "With Run Numbers";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numFirstRunNo
            // 
            this.numFirstRunNo.Location = new System.Drawing.Point(288, 320);
            this.numFirstRunNo.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numFirstRunNo.Name = "numFirstRunNo";
            this.numFirstRunNo.Size = new System.Drawing.Size(80, 21);
            this.numFirstRunNo.TabIndex = 47;
            this.numFirstRunNo.Visible = false;
            // 
            // drpRunNoCriteria
            // 
            this.drpRunNoCriteria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpRunNoCriteria.Items.AddRange(new object[] {
            "=",
            ">=",
            "From"});
            this.drpRunNoCriteria.Location = new System.Drawing.Point(216, 320);
            this.drpRunNoCriteria.Name = "drpRunNoCriteria";
            this.drpRunNoCriteria.Size = new System.Drawing.Size(64, 21);
            this.drpRunNoCriteria.TabIndex = 46;
            this.drpRunNoCriteria.SelectionChangeCommitted += new System.EventHandler(this.drpRunNoCriteria_SelectionChangeCommitted);
            // 
            // drpTransRefNoCriteria
            // 
            this.drpTransRefNoCriteria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpTransRefNoCriteria.Items.AddRange(new object[] {
            "",
            "=",
            ">=",
            "<",
            "From"});
            this.drpTransRefNoCriteria.Location = new System.Drawing.Point(216, 240);
            this.drpTransRefNoCriteria.Name = "drpTransRefNoCriteria";
            this.drpTransRefNoCriteria.Size = new System.Drawing.Size(64, 21);
            this.drpTransRefNoCriteria.TabIndex = 46;
            this.drpTransRefNoCriteria.SelectionChangeCommitted += new System.EventHandler(this.drpTransRefNoCriteria_SelectionChangeCommitted);
            // 
            // drpDateCriteria
            // 
            this.drpDateCriteria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDateCriteria.Items.AddRange(new object[] {
            "",
            ">=",
            "<",
            "From"});
            this.drpDateCriteria.Location = new System.Drawing.Point(216, 40);
            this.drpDateCriteria.Name = "drpDateCriteria";
            this.drpDateCriteria.Size = new System.Drawing.Size(64, 21);
            this.drpDateCriteria.TabIndex = 46;
            this.drpDateCriteria.SelectionChangeCommitted += new System.EventHandler(this.drpDateCriteria_SelectionChangeCommitted);
            // 
            // drpTransTypeCriteria
            // 
            this.drpTransTypeCriteria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpTransTypeCriteria.Items.AddRange(new object[] {
            "",
            "=",
            "In Set"});
            this.drpTransTypeCriteria.Location = new System.Drawing.Point(216, 80);
            this.drpTransTypeCriteria.Name = "drpTransTypeCriteria";
            this.drpTransTypeCriteria.Size = new System.Drawing.Size(64, 21);
            this.drpTransTypeCriteria.TabIndex = 46;
            this.drpTransTypeCriteria.SelectionChangeCommitted += new System.EventHandler(this.drpTransTypeCriteria_SelectionChangeCommitted);
            this.drpTransTypeCriteria.SelectedIndexChanged += new System.EventHandler(this.drpTransTypeCriteria_SelectedIndexChanged);
            // 
            // drpTransTypes
            // 
            this.drpTransTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpTransTypes.Location = new System.Drawing.Point(288, 80);
            this.drpTransTypes.Name = "drpTransTypes";
            this.drpTransTypes.Size = new System.Drawing.Size(64, 21);
            this.drpTransTypes.TabIndex = 46;
            this.drpTransTypes.Visible = false;
            // 
            // drpBranchCriteria
            // 
            this.drpBranchCriteria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranchCriteria.Items.AddRange(new object[] {
            "",
            "=",
            "In Set"});
            this.drpBranchCriteria.Location = new System.Drawing.Point(216, 160);
            this.drpBranchCriteria.Name = "drpBranchCriteria";
            this.drpBranchCriteria.Size = new System.Drawing.Size(64, 21);
            this.drpBranchCriteria.TabIndex = 46;
            this.drpBranchCriteria.SelectionChangeCommitted += new System.EventHandler(this.drpBranchCriteria_SelectionChangeCommitted);
            // 
            // comboBox1
            // 
            this.comboBox1.Location = new System.Drawing.Point(288, 80);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(64, 21);
            this.comboBox1.TabIndex = 46;
            this.comboBox1.Visible = false;
            // 
            // drpBranches
            // 
            this.drpBranches.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranches.Location = new System.Drawing.Point(288, 160);
            this.drpBranches.Name = "drpBranches";
            this.drpBranches.Size = new System.Drawing.Size(64, 21);
            this.drpBranches.TabIndex = 46;
            // 
            // chxValueOnly
            // 
            this.chxValueOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chxValueOnly.Location = new System.Drawing.Point(216, 392);
            this.chxValueOnly.Name = "chxValueOnly";
            this.chxValueOnly.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chxValueOnly.Size = new System.Drawing.Size(312, 24);
            this.chxValueOnly.TabIndex = 48;
            this.chxValueOnly.Text = "Retrieve values only";
            this.chxValueOnly.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chxValueOnly.CheckedChanged += new System.EventHandler(this.chxValueOnly_CheckedChanged);
            // 
            // txtEmployees
            // 
            this.txtEmployees.Location = new System.Drawing.Point(288, 280);
            this.txtEmployees.Name = "txtEmployees";
            this.txtEmployees.ReadOnly = true;
            this.txtEmployees.Size = new System.Drawing.Size(224, 21);
            this.txtEmployees.TabIndex = 44;
            this.txtEmployees.Visible = false;
            // 
            // drpEmployeeCriteria
            // 
            this.drpEmployeeCriteria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmployeeCriteria.Items.AddRange(new object[] {
            "",
            "=",
            "In Set"});
            this.drpEmployeeCriteria.Location = new System.Drawing.Point(216, 280);
            this.drpEmployeeCriteria.Name = "drpEmployeeCriteria";
            this.drpEmployeeCriteria.Size = new System.Drawing.Size(64, 21);
            this.drpEmployeeCriteria.TabIndex = 46;
            this.drpEmployeeCriteria.SelectionChangeCommitted += new System.EventHandler(this.drpEmployeeCriteria_SelectionChangeCommitted);
            // 
            // drpEmployees
            // 
            this.drpEmployees.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmployees.Location = new System.Drawing.Point(288, 280);
            this.drpEmployees.Name = "drpEmployees";
            this.drpEmployees.Size = new System.Drawing.Size(224, 21);
            this.drpEmployees.TabIndex = 46;
            // 
            // tpResults
            // 
            this.tpResults.Controls.Add(this.btnExcel);
            this.tpResults.Controls.Add(this.dgResults);
            this.tpResults.Location = new System.Drawing.Point(0, 25);
            this.tpResults.Name = "tpResults";
            this.tpResults.Selected = false;
            this.tpResults.Size = new System.Drawing.Size(792, 455);
            this.tpResults.TabIndex = 6;
            this.tpResults.Title = "Results";
            // 
            // btnExcel
            // 
            this.btnExcel.Enabled = false;
            this.btnExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnExcel.Image")));
            this.btnExcel.Location = new System.Drawing.Point(752, 16);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(32, 32);
            this.btnExcel.TabIndex = 49;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // dgResults
            // 
            this.dgResults.DataMember = "";
            this.dgResults.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgResults.Location = new System.Drawing.Point(16, 16);
            this.dgResults.Name = "dgResults";
            this.dgResults.ReadOnly = true;
            this.dgResults.Size = new System.Drawing.Size(728, 416);
            this.dgResults.TabIndex = 0;
            // 
            // menuInstantReplacement
            // 
            this.menuInstantReplacement.Enabled = false;
            this.menuInstantReplacement.Location = new System.Drawing.Point(248, 80);
            this.menuInstantReplacement.Name = "menuInstantReplacement";
            this.menuInstantReplacement.Size = new System.Drawing.Size(72, 16);
            this.menuInstantReplacement.TabIndex = 44;
            this.menuInstantReplacement.Text = "dummyMenu";
            this.menuInstantReplacement.Visible = false;
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
            this.menuExit.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // label63
            // 
            this.label63.Location = new System.Drawing.Point(400, 224);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(112, 16);
            this.label63.TabIndex = 12;
            this.label63.Text = "Total Fees";
            // 
            // dgTransactions
            // 
            this.dgTransactions.CaptionText = "Transactions";
            this.dgTransactions.DataMember = "";
            this.dgTransactions.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgTransactions.Location = new System.Drawing.Point(80, -16);
            this.dgTransactions.Name = "dgTransactions";
            this.dgTransactions.ReadOnly = true;
            this.dgTransactions.Size = new System.Drawing.Size(752, 232);
            this.dgTransactions.TabIndex = 6;
            this.dgTransactions.TabStop = false;
            // 
            // label45
            // 
            this.label45.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label45.Location = new System.Drawing.Point(48, 224);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(144, 16);
            this.label45.TabIndex = 9;
            this.label45.Text = "Total Admin Charges";
            this.label45.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label54
            // 
            this.label54.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label54.Location = new System.Drawing.Point(216, 224);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(152, 16);
            this.label54.TabIndex = 10;
            this.label54.Text = "Total Interest Charged";
            this.label54.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // FinTransQuery
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.tcMain);
            this.Name = "FinTransQuery";
            this.Text = "Financial Transactions Query";
            this.Load += new System.EventHandler(this.FinTransQuery_Load);
            this.tcMain.ResumeLayout(false);
            this.tpQuery.ResumeLayout(false);
            this.tpQuery.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFirstTransRefNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSecondTransRefNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSecondRunNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFirstRunNo)).EndInit();
            this.tpResults.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgTransactions)).EndInit();
            this.ResumeLayout(false);

        }
		#endregion


        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            Function = "btnSearch_Click";
			DataSet dsResults;
            try
            {
                Wait();

                DateTime fromDate = DateTime.Today;
                DateTime toDate = DateTime.Today;
                if (dtStart.Visible)
                {
                    fromDate = dtStart.Value;
                }
                if (dtEnd.Visible)
                {
                    toDate = dtEnd.Value;
                }
                string transType = string.Empty;
                if (drpTransTypes.Visible && drpTransTypes.SelectedIndex >= 0)
                {
                    transType = drpTransTypes.SelectedValue.ToString();
                }
                int firstRunNo = 0;
                int secondRunNo = 0;
                if (numFirstRunNo.Visible)
                {
                    firstRunNo = Convert.ToInt32(numFirstRunNo.Value);
                }
                if (numSecondRunNo.Visible)
                {
                    secondRunNo = Convert.ToInt32(numSecondRunNo.Value);
                }
                int firstrefno = 0;
                int secondrefno = 0;
                if (numFirstTransRefNo.Visible) 
                {
                    firstrefno = Convert.ToInt32(numFirstTransRefNo.Value);
                }
                if (numSecondTransRefNo.Visible)
                {
                    secondrefno = Convert.ToInt32(numSecondTransRefNo.Value);
                }
                int employee = 0;
                if (drpEmployees.Visible && drpEmployees.SelectedIndex >= 0)
                {
                    employee = int.Parse(drpEmployees.SelectedValue.ToString());
                }
                int branch = 0;
                if (drpBranches.Visible && drpBranches.SelectedIndex >= 0)
                {
                    branch = int.Parse(drpBranches.SelectedValue.ToString());
                }
                int acctsInBranch = 0;
                if (drpAcctsInBranch.SelectedIndex > 0)//Entry 0 is blank - meaning ALL
                {
                    acctsInBranch = int.Parse(drpAcctsInBranch.SelectedValue.ToString());
                }
                int valueOnly = chxValueOnly.Checked ? 1:0;
                int includeOtherCharges = chxIncludeInterest.Checked ? 1:0;
                //If SET was specified, but no actual set was selected, override
                //the users request.
                string transTypeOperand = drpTransTypeCriteria.Text;
                string branchOperand = drpBranchCriteria.Text;
                string employeeOperand = drpEmployeeCriteria.Text;
                if (txtTransactionTypes.Visible 
                    && txtTransactionTypes.Text == _noSetSelection)
                {
                    transTypeOperand = string.Empty;
                }
                if (txtBranches.Visible 
                    && txtBranches.Text == _noSetSelection)
                {
                    branchOperand = string.Empty;
                }
                if (txtEmployees.Visible 
                    && txtEmployees.Text == _noSetSelection)
                {
                    employeeOperand = string.Empty;
                }
                // Clear the data grid
                dgResults.DataSource = null;
                ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_SEARCHTRANSACTIONS");

                dsResults = AccountManager.GetFinTransQueryResults(fromDate,
                                                                    toDate,
                                                                    transTypeOperand,
                                                                    transType,
                                                                    drpLimitValues.Text,
                                                                    firstRunNo,
                                                                    secondRunNo,
                                                                    drpRunNoCriteria.Text,
                                                                    firstrefno,
                                                                    secondrefno,
                                                                    drpTransRefNoCriteria.Text,
                                                                    employee,
                                                                    employeeOperand,
                                                                    branch,
                                                                    branchOperand,
                                                                    acctsInBranch,
                                                                    drpDateCriteria.Text,
                                                                    txtBranches.Text,
                                                                    txtTransactionTypes.Text,
                                                                    txtEmployees.Text,
                                                                    valueOnly,
                                                                    includeOtherCharges,
                                                                    out Error);

                if(Error.Length>0)
                    ShowError(Error);
                else
                {
                    if(dsResults!=null && dsResults.Tables[0] != null)
                    {
                        DataTable dtResults = dsResults.Tables[0];

                        ((MainForm)this.FormRoot).statusBar1.Text = dtResults.Rows.Count + GetResource("M_ROWSRETURNED");

                        if(dtResults.Rows.Count > 0)
                        {
                            btnExcel.Enabled = true;
                            dtResults.DefaultView.AllowNew = false;
                            dgResults.DataSource = dtResults.DefaultView;

                            if(dgResults.TableStyles.Count==0)
                            {
                                DataGridTableStyle tabStyle = new DataGridTableStyle();
                                tabStyle.MappingName = dtResults.TableName;
                                dgResults.TableStyles.Add(tabStyle);

                                /* set up the header text */
                                if (chxValueOnly.Checked)
                                {
                                    tabStyle.GridColumnStyles[CN.TransValue].HeaderText = GetResource("T_VALUE") + ":";

                                    /* set up the column widths */
                                    tabStyle.GridColumnStyles[CN.TransValue].Width = 75;

                                    /* set up column formatting */
                                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TransValue]).Format = DecimalPlaces;

                                    /* set alignment */
                                    tabStyle.GridColumnStyles[CN.TransValue].Alignment = HorizontalAlignment.Right;
                                }
                                else 
                                {
                                    tabStyle.GridColumnStyles[CN.AcctNo].HeaderText = GetResource("T_ACCTNO");
                                    tabStyle.GridColumnStyles[CN.DateTrans].HeaderText = GetResource("T_TRANSACTIONDATE");
                                    tabStyle.GridColumnStyles[CN.TransTypeCode].HeaderText = GetResource("T_TYPE");
                                    tabStyle.GridColumnStyles[CN.TransValue].HeaderText = GetResource("T_VALUE") + ":";
                                    tabStyle.GridColumnStyles[CN.EmployeeNo].HeaderText = GetResource("T_EMPEENO");
                                    tabStyle.GridColumnStyles[CN.Runno].HeaderText = GetResource("T_RUNNO");
                                    tabStyle.GridColumnStyles[CN.TransRefNo].HeaderText = GetResource("T_TRANSREFNO");
                                    tabStyle.GridColumnStyles[CN.Source].HeaderText = GetResource("T_SOURCE");
                                    tabStyle.GridColumnStyles[CN.FTNotes].HeaderText = GetResource("T_NOTES");


                                    /* set up the column widths */
                                    tabStyle.GridColumnStyles[CN.AcctNo].Width = 100;
                                    tabStyle.GridColumnStyles[CN.DateTrans].Width = 105;
                                    tabStyle.GridColumnStyles[CN.TransTypeCode].Width = 55;
                                    tabStyle.GridColumnStyles[CN.TransValue].Width = 75;
                                    tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 85;
                                    tabStyle.GridColumnStyles[CN.Runno].Width = 95;
                                    tabStyle.GridColumnStyles[CN.TransRefNo].Width = 90;
                                    tabStyle.GridColumnStyles[CN.Source].Width = 100;
                                    tabStyle.GridColumnStyles[CN.FTNotes].Width = 100;

                                    /* set up column formatting */
                                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TransValue]).Format = DecimalPlaces;

                                    /* set alignment */
                                    tabStyle.GridColumnStyles[CN.TransValue].Alignment = HorizontalAlignment.Right;
                                    tabStyle.GridColumnStyles[CN.EmployeeNo].Alignment = HorizontalAlignment.Right;
                                    tabStyle.GridColumnStyles[CN.Runno].Alignment = HorizontalAlignment.Center;
                                }
                            }
                        }
                        else//no data found 
                        {
                            btnExcel.Enabled = false;
                        }
                    }
                    else
                    {
                        ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_ZEROROWS");
                    }

                }
                Function = "End of btnSearch_Click";
            }
            catch(Exception ex)
            {
                Catch(ex, Function);
            }	
            finally
            {
                if (dgResults.VisibleRowCount > 0) 
                {
                    //Display the results tab..
                    //Having already 'switched' to the Query tab earlier in _Load method
                    //this now works..
                    tcMain.SelectedTab = tpResults;
                }
                StopWait();
            }		
        }

        private void Clear() 
        {
            dgResults.DataSource = null;
            dtStart.Value = DateTime.Today.AddMonths(-1);
            dtEnd.Value = DateTime.Today.AddDays(1).AddSeconds(-1);
            dtStart.Visible = false;
            lbBetweenDates.Visible = false;
            dtEnd.Visible = false;

            drpLimitValues.SelectedIndex = -1;
            drpAcctsInBranch.SelectedIndex = 0;
            drpRunNoCriteria.SelectedIndex = 1;

            drpTransTypeCriteria.SelectedIndex = 0;
            drpTransTypes.SelectedIndex = -1;
            txtTransactionTypes.Text = _noSetSelection;
            drpTransTypes.Visible = false;
            txtTransactionTypes.Visible = false;
            btnTransTypes.Visible = false;

            drpBranchCriteria.SelectedIndex = 0;
            drpBranches.SelectedIndex = -1;
            txtBranches.Text = _noSetSelection;
            drpBranches.Visible = false;
            txtBranches.Visible = false;
            btnBranches.Visible = false;

            drpEmployeeCriteria.SelectedIndex = 0;
            drpEmployees.SelectedIndex = -1;
            txtEmployees.Text = _noSetSelection;
            drpEmployees.Visible = false;
            txtEmployees.Visible = false;
            btnEmployees.Visible = false;

            drpDateCriteria.SelectedIndex = 0;
            lbRunNoTo.Visible = false;
            numFirstRunNo.Visible = true;
            numSecondRunNo.Visible = false;
            drpTransRefNoCriteria.SelectedIndex = 0;
            lbTransRefNoTo.Visible = false;
            numFirstTransRefNo.Visible = false;
            numSecondTransRefNo.Visible = false;
            chxIncludeInterest.Checked = true;
            chxValueOnly.Checked = false;
            numFirstRunNo.Value += 1;//This looks like .Net bug, need to do this first..
            numFirstRunNo.Value = numFirstRunNo.Minimum;
            numSecondRunNo.Value -= 1;//This looks like .Net bug, need to do this first..
            numSecondRunNo.Value = numSecondRunNo.Maximum;
            numFirstTransRefNo.Value += 1;//This looks like .Net bug, need to do this first..
            numFirstTransRefNo.Value = numFirstTransRefNo.Minimum;
            numSecondTransRefNo.Value -= 1;//This looks like .Net bug, need to do this first..
            numSecondTransRefNo.Value = numSecondTransRefNo.Maximum;
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }

        private void drpRunNoCriteria_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            string selection = drpRunNoCriteria.SelectedItem.ToString();
            switch (selection)
            {
                case "=" : 
                case ">=":
                {
                    numFirstRunNo.Visible = true;
                    lbRunNoTo.Visible = false;
                    numSecondRunNo.Visible = false;
                    break;
                }
                case "From" : 
                {
                    numFirstRunNo.Visible = true;
                    lbRunNoTo.Visible = true;
                    numSecondRunNo.Visible = true;
                    break;
                }
                default : 
                {
                    numFirstRunNo.Visible = false;
                    lbRunNoTo.Visible = false;
                    numSecondRunNo.Visible = false;
                    break;
                }
            }
        }

        private void FinTransQuery_Load(object sender, System.EventArgs e)
        {
            try 
            {
                LoadStaticData();
                Clear();
                //Display the query tab - need to 'switch' between tabs for this 
                //and other tab selection to work - ie. this somehow gives the tab control
                //focus so that future tab selection method calls work properly
                tcMain.SelectedTab = tpResults;
                tcMain.SelectedTab = tpQuery;
            }
            catch (Exception ex) 
            {
                Catch(ex, Function);
            }
        }

        private void btnTransTypes_Click(object sender, System.EventArgs e)
        {
            SetSelection selection = new SetSelection("Transaction Types",45,180,64,this.txtTransactionTypes,TN.TNameTransType,TN.NonDeposits,false);
            selection.FormRoot = this.FormRoot;
            selection.FormParent = this;
            selection.ShowDialog(this);
        }

        private void LoadStaticData()
        {
            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
				
            if(StaticData.Tables[TN.BranchNumber]==null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.BranchNumber, null));

            if(StaticData.Tables[TN.AllStaff]==null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.AllStaff, null));

            //Non-deposits represents the Fin Trans TransTypes that we
            //are interested in..
            if (StaticData.Tables[TN.NonDeposits]==null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.NonDeposits, null));
			
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

            //Now customise the dropdowns, the Branch dropdown needs an 'ALL' type
            //entry added..
            StringCollection branchNos = new StringCollection(); 	
            branchNos.Add(string.Empty);

            foreach(DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
            {
                branchNos.Add(Convert.ToString(row[CN.BranchNo]));
            }

            drpAcctsInBranch.DataSource = branchNos;
            drpAcctsInBranch.Text = Config.BranchCode;

            drpTransTypes.DataSource = ((DataTable)StaticData.Tables[TN.NonDeposits]);
            drpTransTypes.DisplayMember = CN.Code;
            drpTransTypes.ValueMember = CN.Code;

            drpBranches.DataSource = ((DataTable)StaticData.Tables[TN.BranchNumber]);
            drpBranches.DisplayMember = CN.BranchNo;
            drpBranches.ValueMember = CN.BranchNo;

            drpEmployees.DataSource = ((DataTable)StaticData.Tables[TN.AllStaff]);
            drpEmployees.DisplayMember = CN.EmployeeName.ToLower();
            drpEmployees.ValueMember = CN.EmployeeNo.ToLower();

			_noSetSelection = GetResource("NoSetSpecified");
        }

        private void drpTransRefNoCriteria_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            string selection = drpTransRefNoCriteria.SelectedItem.ToString();
            switch (selection)
            {
                case "" : 
                {
                    numFirstTransRefNo.Visible = false;
                    lbTransRefNoTo.Visible = false;
                    numSecondTransRefNo.Visible = false;
                    break;
                }
                case "=" : 
                case ">=":
                case "<" :
                {
                    numFirstTransRefNo.Visible = true;
                    lbTransRefNoTo.Visible = false;
                    numSecondTransRefNo.Visible = false;
                    break;
                }
                case "From" : 
                {
                    numFirstTransRefNo.Visible = true;
                    lbTransRefNoTo.Visible = true;
                    numSecondTransRefNo.Visible = true;
                    break;
                }
                default : 
                {
                    numFirstTransRefNo.Visible = false;
                    lbTransRefNoTo.Visible = false;
                    numSecondTransRefNo.Visible = false;
                    break;
                }
            }        
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            Clear();
        }

        private void drpDateCriteria_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            string selection = drpDateCriteria.SelectedItem.ToString();
            switch (selection)
            {
                case "" : 
                {
                    dtStart.Visible = false;
                    lbBetweenDates.Visible = false;
                    dtEnd.Visible = false;
                    break;
                }
                case ">=":
                case "<" :
                {
                    dtStart.Visible = true;
                    lbBetweenDates.Visible = false;
                    dtEnd.Visible = false;
                    break;
                }
                case "From" : 
                {
                    dtStart.Visible = true;
                    lbBetweenDates.Visible = true;
                    dtEnd.Visible = true;
                    break;
                }
                default : 
                {
                    dtStart.Visible = true;
                    lbBetweenDates.Visible = false;
                    dtEnd.Visible = false;
                    break;
                }
            }
        }

        private void drpTransTypeCriteria_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            string selection = drpTransTypeCriteria.SelectedItem.ToString();
            switch (selection)
            {
                case "" : 
                {
                    drpTransTypes.Visible = false;
                    txtTransactionTypes.Visible = false;
                    btnTransTypes.Visible = false;
                    break;
                }
                case "=":
                {
                    drpTransTypes.Visible = true;
                    txtTransactionTypes.Visible = false;
                    btnTransTypes.Visible = false;
                    break;
                }
                case "In Set" : 
                {
                    drpTransTypes.Visible = false;
                    txtTransactionTypes.Visible = true;
                    btnTransTypes.Visible = true;
                    break;
                }
                default : 
                {
                    drpTransTypes.Visible = false;
                    txtTransactionTypes.Visible = false;
                    btnTransTypes.Visible = false;
                    break;
                }
            }
        }

        private void drpBranchCriteria_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            string selection = drpBranchCriteria.SelectedItem.ToString();
            switch (selection)
            {
                case "" : 
                {
                    drpBranches.Visible = false;
                    txtBranches.Visible = false;
                    btnBranches.Visible = false;
                    break;
                }
                case "=":
                {
                    drpBranches.Visible = true;
                    txtBranches.Visible = false;
                    btnBranches.Visible = false;
                    break;
                }
                case "In Set" : 
                {
                    drpBranches.Visible = false;
                    txtBranches.Visible = true;
                    btnBranches.Visible = true;
                    break;
                }
                default : 
                {
                    drpBranches.Visible = false;
                    txtBranches.Visible = false;
                    btnBranches.Visible = false;
                    break;
                }
            }        
        }

        private void btnBranches_Click(object sender, System.EventArgs e)
        {
            SetSelection selection = new SetSelection("Branches",45,195,64,this.txtBranches,TN.TNameBranch,TN.BranchNumber,false);
            selection.FormRoot = this.FormRoot;
            selection.FormParent = this;
            selection.ShowDialog(this);
        }

        private void drpEmployeeCriteria_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            string selection = drpEmployeeCriteria.SelectedItem.ToString();
            switch (selection)
            {
                case "" : 
                {
                    drpEmployees.Visible = false;
                    txtEmployees.Visible = false;
                    btnEmployees.Visible = false;
                    break;
                }
                case "=":
                {
                    drpEmployees.Visible = true;
                    txtEmployees.Visible = false;
                    btnEmployees.Visible = false;
                    break;
                }
                case "In Set" : 
                {
                    drpEmployees.Visible = false;
                    txtEmployees.Visible = true;
                    btnEmployees.Visible = true;
                    break;
                }
                default : 
                {
                    drpEmployees.Visible = false;
                    txtEmployees.Visible = false;
                    btnEmployees.Visible = false;
                    break;
                }
            }                
        }

        private void btnEmployees_Click(object sender, System.EventArgs e)
        {
            SetSelection selection = new SetSelection("Courtspersons",45,193,64,this.txtEmployees,TN.TNameEmployee,TN.AllStaff,false);
            selection.FormRoot = this.FormRoot;
            selection.FormParent = this;
            selection.ShowDialog(this);
        }

        private void btnExcel_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                /* save the current data grid contents to a CSV */
                string comma = ",";
                string path = "";

                if(dgResults.CurrentRowIndex >= 0)
                {
                    DataView dv = (DataView)dgResults.DataSource;

                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*" ;
                    save.Title = "Save Fin Trans query data";
                    save.CreatePrompt = true;

                    if(save.ShowDialog() == DialogResult.OK)
                    {
                        path = save.FileName;
                        FileInfo fi = new FileInfo(path);
                        FileStream fs = fi.OpenWrite();
			
                        //Write heading line..
                        string line = string.Empty;

                        if (this.chxValueOnly.Checked) 
                        {
                            line = line + GetResource("T_VALUE") +
                                Environment.NewLine + Environment.NewLine;
                        }
                        else
                        {
                            line = line + GetResource("T_ACCTNO") + comma +
                                GetResource("T_TRANSACTIONDATE") + comma +
                                GetResource("T_TYPE") + comma +
                                GetResource("T_VALUE") + comma +
                                GetResource("T_EMPEENO") + comma +
                                GetResource("T_RUNNO") + comma +
                                GetResource("T_TRANSREFNO") + comma +
                                GetResource("T_SOURCE") + comma +
                                GetResource("T_NOTES") + Environment.NewLine + Environment.NewLine;
                        }
                        byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
                        fs.Write(blob,0,blob.Length);
			
                        foreach(DataRowView row in dv)
                        {					
                            line = string.Empty;

                            if (this.chxValueOnly.Checked) 
                            {
                                line +=	"'" + ((decimal)row[CN.TransValue]).ToString(DecimalPlaces).Replace(",","") +
                                    Environment.NewLine;
                            }
                            else
                            {
                                line +=	"'" + row[CN.AcctNo].ToString().Replace(",","") + "'" + comma +
                                    Convert.ToString(row[CN.DateTrans]).Replace(",","") + comma +
                                    row[CN.TransTypeCode].ToString().Replace(",","") + comma +
                                    ((decimal)row[CN.TransValue]).ToString(DecimalPlaces).Replace(",","") + comma +
                                    row[CN.EmployeeNo].ToString().Replace(",","") + comma +
                                    row[CN.Runno].ToString().Replace(",","") + comma +
                                    row[CN.TransRefNo].ToString().Replace(",","") + comma +
                                    row[CN.Source].ToString().Replace(",","") + comma +
                                    row[CN.FTNotes].ToString().Replace(",", "") +
                                    Environment.NewLine;
                            }

                            blob = System.Text.Encoding.UTF8.GetBytes(line);
                            fs.Write(blob,0,blob.Length);
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
            catch(Exception ex)
            {
                Catch(ex, Function);
            }	
            finally
            {
                StopWait();
            }
        }

        private void chxValueOnly_CheckedChanged(object sender, System.EventArgs e)
        {
            //Force the layout of the DataGrid to be recreated next time data is retrieved.
            dgResults.TableStyles.Clear();
        }

        private void drpTransTypeCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    

    }
}
