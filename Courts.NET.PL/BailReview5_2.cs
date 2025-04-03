using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using Crownwood.Magic.Menus;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.ScreenModes;     //CR1084
using STL.Common.Constants.TableNames;
using STL.Common.Constants.TabPageNames;
using STL.Common.Static;
using STL.PL.Collections;
using STL.PL.WS1;
using Blue.Cosacs.Shared;


namespace STL.PL
{
    /// <summary>
    /// Lists the allocated accounts for an employee. This screen is used
    /// for Telephone Actions and for Bailiff Review. Action codes can be
    /// reviewed and added for each account. The list of allocated account
    /// details can be copied to a CSV file to be used in Excel.
    /// If authorised a user can re-allocate accounts.
    /// When open for Bailiff Review this screen can de-allocate acounts.
    /// </summary>
    public class BailReview5_2 : CommonForm
    {
        #region --- Designer Variables --------------------------------------------------------------------
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox drpEmpType;
        private System.Windows.Forms.ComboBox drpEmpName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label4;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.ComboBox drpBranch;
        private System.Windows.Forms.Label label5;
        private STL.PL.AccountTextBox txtAccountNo;
        private System.Windows.Forms.Label lAccountNo;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        public System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.Button btnLoadExtraInfo;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.TextBox txtActionValue;
        private System.Windows.Forms.ComboBox drpActionCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DateTimePicker dtDueDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox gbLastAction;
        private System.Windows.Forms.DateTimePicker dtLastActionDate;
        private System.Windows.Forms.TextBox txtLastActionCode;
        private System.Windows.Forms.GroupBox gbNewAction;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnTempReceipt;
        private System.Windows.Forms.Button btnDeAllocate;
        private System.Windows.Forms.GroupBox gbReAllocate;
        private System.Windows.Forms.ComboBox drpRAEmpType;
        private System.Windows.Forms.ComboBox drpRAEmpName;
        private System.Windows.Forms.Button btnReAllocate;
        private System.Windows.Forms.Label lbActionValue;
        private System.Windows.Forms.Label lbActionDate;
        private System.Windows.Forms.ComboBox drpReason;
        private System.Windows.Forms.Label lbReason;
        private Crownwood.Magic.Controls.TabControl tabAccounts;
        private Crownwood.Magic.Controls.TabPage tabAccountDetails;
        private Crownwood.Magic.Controls.TabControl tabSubAccountDetails;
        private Crownwood.Magic.Controls.TabPage tabCustomer;
        private GroupBox gbCustomer;
        private TextBox txtTitle;
        private Label Title;
        private Label label10;
        private Label label11;
        private Label label12;
        private TextBox txtMobile;
        private TextBox txtFirstName;
        private Label label13;
        private TextBox txtLastName;
        private TextBox txtWork;
        private Label label17;
        private TextBox txtHome;
        private Button btnAccountDetails;
        private Button btnAddCode;
        private Button btnFollowUp;
        private Label label33;
        private TextBox txtMonthArrears;
        private TextBox txtPercentage;
        private Label label34;
        private TextBox txtDateLastPaid;
        private Label label35;
        private TextBox txtStatus;
        private Label label36;
        private Label label37;
        private TextBox txtDueDate;
        private TextBox txtArrears;
        private TextBox txtBalance;
        private Label label38;
        private Label label40;
        private TextBox txtAgreementTotal;
        private Label label41;
        private Label label44;
        private TextBox txtInstalment;
        private Label lMoreRewardsDate2;
        private DateTimePicker dtMoreRewardsDate2;
        private Label lMoreRewards2;
        private TextBox txtMoreRewards2;
        private Crownwood.Magic.Controls.TabPage tabProducts;
        private GroupBox gbTransactions;
        private DataGridView dgTransactions;
        private GroupBox gbProducts;
        private DataGridView dgProducts;
        private Crownwood.Magic.Controls.TabPage tabStrategies;
        private GroupBox gbWorklists;
        private DataGridView dgWorklists;
        private GroupBox gbStrategies;
        private DataGridView dgStrategies;
        private Crownwood.Magic.Controls.TabPage tabLetters;
        private GroupBox gbSMS;
        private DataGridView dgSMS;
        private GroupBox gbLetters;
        private DataGridView dgLetters;
        private Button button1;
        private Button button2;
        private Button button3;
        private GroupBox groupBox1;
        private TextBox textBox1;
        private Label label14;
        private Label label15;
        private Label label16;
        private Label label18;
        private TextBox textBox2;
        private TextBox textBox3;
        private Label label19;
        private TextBox textBox4;
        private TextBox textBox5;
        private Label label20;
        private TextBox textBox6;
        private Label label21;
        private TextBox textBox7;
        private TextBox textBox8;
        private Label label22;
        private TextBox textBox9;
        private Label label23;
        private TextBox textBox10;
        private Label label24;
        private Label label25;
        private TextBox textBox11;
        private TextBox textBox12;
        private TextBox textBox13;
        private Label label26;
        private Label label27;
        private TextBox textBox14;
        private Label label28;
        private Label label29;
        private TextBox textBox15;
        private Crownwood.Magic.Controls.TabPage tabAccountList;
        private DataGrid dgAccounts;
        private ComboBox drpStrategies;
        private Label label30;
        private CheckBox chCycleNext;
        private ComboBox drpSendToStrategy;
        private CheckBox chkOverrideMaxAlloc;
        private Crownwood.Magic.Controls.TabPage tabAllocateSingleAccount;
        private Button btnAllocateAcct;
        private ComboBox drpEmpNameAcct;
        private ComboBox drpEmployeeTypesAcct;
        private ComboBox drpBranchAcct;
        private Label label31;
        private AccountTextBox txtAcctNo;
        private Label label32;
        private Label label39;
        private Button btnClearAcct;
        private Label label42;
        private Label showMultipleAccounts;
        private ToolTip ttBailReview;
        private Label label43;
        private Label label45;
        private TextBox txt_provisionamount;
        private Button btnReferences;

        private System.ComponentModel.IContainer components;
        private TextBox txt_Provisions;

        private struct SanctionStage        //CR1084
        {
            //public string CustID;
            //public string AcctType;
            public string AccountNo;
            public DateTime DateProp;
            //public string ScreenMode;
            //public string CheckType;
            //public string Stage;
            //public int ImageIndex;
        }
        private SanctionStage StageToLaunch;
        #endregion End of Designer Variables --------------------------------------------------------------

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BailReview5_2));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.ttBailReview = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkOverrideMaxAlloc = new System.Windows.Forms.CheckBox();
            this.drpStrategies = new System.Windows.Forms.ComboBox();
            this.label30 = new System.Windows.Forms.Label();
            this.drpEmpType = new System.Windows.Forms.ComboBox();
            this.tabAccounts = new Crownwood.Magic.Controls.TabControl();
            this.tabAccountDetails = new Crownwood.Magic.Controls.TabPage();
            this.tabSubAccountDetails = new Crownwood.Magic.Controls.TabControl();
            this.tabCustomer = new Crownwood.Magic.Controls.TabPage();
            this.txt_Provisions = new System.Windows.Forms.TextBox();
            this.btnReferences = new System.Windows.Forms.Button();
            this.label43 = new System.Windows.Forms.Label();
            this.showMultipleAccounts = new System.Windows.Forms.Label();
            this.gbCustomer = new System.Windows.Forms.GroupBox();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.Title = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtMobile = new System.Windows.Forms.TextBox();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.txtWork = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtHome = new System.Windows.Forms.TextBox();
            this.btnAccountDetails = new System.Windows.Forms.Button();
            this.btnAddCode = new System.Windows.Forms.Button();
            this.btnFollowUp = new System.Windows.Forms.Button();
            this.label33 = new System.Windows.Forms.Label();
            this.txtMonthArrears = new System.Windows.Forms.TextBox();
            this.txtPercentage = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.txtDateLastPaid = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.txtDueDate = new System.Windows.Forms.TextBox();
            this.txtArrears = new System.Windows.Forms.TextBox();
            this.txtBalance = new System.Windows.Forms.TextBox();
            this.label38 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.txtAgreementTotal = new System.Windows.Forms.TextBox();
            this.label41 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.txtInstalment = new System.Windows.Forms.TextBox();
            this.lMoreRewardsDate2 = new System.Windows.Forms.Label();
            this.dtMoreRewardsDate2 = new System.Windows.Forms.DateTimePicker();
            this.lMoreRewards2 = new System.Windows.Forms.Label();
            this.txtMoreRewards2 = new System.Windows.Forms.TextBox();
            this.tabProducts = new Crownwood.Magic.Controls.TabPage();
            this.gbTransactions = new System.Windows.Forms.GroupBox();
            this.dgTransactions = new System.Windows.Forms.DataGridView();
            this.gbProducts = new System.Windows.Forms.GroupBox();
            this.dgProducts = new System.Windows.Forms.DataGridView();
            this.tabStrategies = new Crownwood.Magic.Controls.TabPage();
            this.gbWorklists = new System.Windows.Forms.GroupBox();
            this.dgWorklists = new System.Windows.Forms.DataGridView();
            this.gbStrategies = new System.Windows.Forms.GroupBox();
            this.dgStrategies = new System.Windows.Forms.DataGridView();
            this.tabLetters = new Crownwood.Magic.Controls.TabPage();
            this.gbSMS = new System.Windows.Forms.GroupBox();
            this.dgSMS = new System.Windows.Forms.DataGridView();
            this.gbLetters = new System.Windows.Forms.GroupBox();
            this.dgLetters = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.textBox13 = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.textBox14 = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.textBox15 = new System.Windows.Forms.TextBox();
            this.tabAccountList = new Crownwood.Magic.Controls.TabPage();
            this.dgAccounts = new System.Windows.Forms.DataGrid();
            this.tabAllocateSingleAccount = new Crownwood.Magic.Controls.TabPage();
            this.txt_provisionamount = new System.Windows.Forms.TextBox();
            this.label45 = new System.Windows.Forms.Label();
            this.lblshowSingleAccount = new System.Windows.Forms.Label();
            this.btnAllocateAcct = new System.Windows.Forms.Button();
            this.drpEmpNameAcct = new System.Windows.Forms.ComboBox();
            this.drpEmployeeTypesAcct = new System.Windows.Forms.ComboBox();
            this.drpBranchAcct = new System.Windows.Forms.ComboBox();
            this.label31 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.btnClearAcct = new System.Windows.Forms.Button();
            this.label42 = new System.Windows.Forms.Label();
            this.txtAcctNo = new STL.PL.AccountTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gbReAllocate = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.drpRAEmpType = new System.Windows.Forms.ComboBox();
            this.drpRAEmpName = new System.Windows.Forms.ComboBox();
            this.btnReAllocate = new System.Windows.Forms.Button();
            this.btnDeAllocate = new System.Windows.Forms.Button();
            this.btnTempReceipt = new System.Windows.Forms.Button();
            this.gbLastAction = new System.Windows.Forms.GroupBox();
            this.txtLastActionCode = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dtLastActionDate = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.gbNewAction = new System.Windows.Forms.GroupBox();
            this.drpSendToStrategy = new System.Windows.Forms.ComboBox();
            this.chCycleNext = new System.Windows.Forms.CheckBox();
            this.drpReason = new System.Windows.Forms.ComboBox();
            this.lbReason = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnLoadExtraInfo = new System.Windows.Forms.Button();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.txtActionValue = new System.Windows.Forms.TextBox();
            this.drpActionCode = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.dtDueDate = new System.Windows.Forms.DateTimePicker();
            this.lbActionValue = new System.Windows.Forms.Label();
            this.lbActionDate = new System.Windows.Forms.Label();
            this.btnExcel = new System.Windows.Forms.Button();
            this.lAccountNo = new System.Windows.Forms.Label();
            this.drpBranch = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.drpEmpName = new System.Windows.Forms.ComboBox();
            this.txtAccountNo = new STL.PL.AccountTextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox3.SuspendLayout();
            this.tabAccounts.SuspendLayout();
            this.tabAccountDetails.SuspendLayout();
            this.tabSubAccountDetails.SuspendLayout();
            this.tabCustomer.SuspendLayout();
            this.gbCustomer.SuspendLayout();
            this.tabProducts.SuspendLayout();
            this.gbTransactions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgTransactions)).BeginInit();
            this.gbProducts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgProducts)).BeginInit();
            this.tabStrategies.SuspendLayout();
            this.gbWorklists.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgWorklists)).BeginInit();
            this.gbStrategies.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgStrategies)).BeginInit();
            this.tabLetters.SuspendLayout();
            this.gbSMS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgSMS)).BeginInit();
            this.gbLetters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgLetters)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabAccountList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
            this.tabAllocateSingleAccount.SuspendLayout();
            this.gbReAllocate.SuspendLayout();
            this.gbLastAction.SuspendLayout();
            this.gbNewAction.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
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
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox3.Controls.Add(this.chkOverrideMaxAlloc);
            this.groupBox3.Controls.Add(this.drpStrategies);
            this.groupBox3.Controls.Add(this.label30);
            this.groupBox3.Controls.Add(this.drpEmpType);
            this.groupBox3.Controls.Add(this.tabAccounts);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.gbReAllocate);
            this.groupBox3.Controls.Add(this.btnDeAllocate);
            this.groupBox3.Controls.Add(this.btnTempReceipt);
            this.groupBox3.Controls.Add(this.gbLastAction);
            this.groupBox3.Controls.Add(this.gbNewAction);
            this.groupBox3.Controls.Add(this.btnExcel);
            this.groupBox3.Controls.Add(this.lAccountNo);
            this.groupBox3.Controls.Add(this.drpBranch);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.btnPrint);
            this.groupBox3.Controls.Add(this.btnClear);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.btnSearch);
            this.groupBox3.Controls.Add(this.drpEmpName);
            this.groupBox3.Controls.Add(this.txtAccountNo);
            this.groupBox3.Location = new System.Drawing.Point(8, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(776, 472);
            this.groupBox3.TabIndex = 32;
            this.groupBox3.TabStop = false;
            // 
            // chkOverrideMaxAlloc
            // 
            this.chkOverrideMaxAlloc.Location = new System.Drawing.Point(550, 425);
            this.chkOverrideMaxAlloc.Name = "chkOverrideMaxAlloc";
            this.chkOverrideMaxAlloc.Size = new System.Drawing.Size(206, 18);
            this.chkOverrideMaxAlloc.TabIndex = 98;
            this.chkOverrideMaxAlloc.Text = "Override Maximum Allocation";
            this.chkOverrideMaxAlloc.UseVisualStyleBackColor = true;
            this.chkOverrideMaxAlloc.CheckedChanged += new System.EventHandler(this.chkOverrideMaxAlloc_CheckedChanged);
            // 
            // drpStrategies
            // 
            this.drpStrategies.Enabled = false;
            this.drpStrategies.FormattingEnabled = true;
            this.drpStrategies.Location = new System.Drawing.Point(81, 51);
            this.drpStrategies.Name = "drpStrategies";
            this.drpStrategies.Size = new System.Drawing.Size(178, 21);
            this.drpStrategies.TabIndex = 93;
            this.drpStrategies.Visible = false;
            this.drpStrategies.SelectionChangeCommitted += new System.EventHandler(this.drpStrategies_SelectionChangeCommitted);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(24, 54);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(46, 13);
            this.label30.TabIndex = 92;
            this.label30.Text = "Strategy";
            this.label30.Visible = false;
            // 
            // drpEmpType
            // 
            this.drpEmpType.ItemHeight = 13;
            this.drpEmpType.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.drpEmpType.Location = new System.Drawing.Point(83, 25);
            this.drpEmpType.Name = "drpEmpType";
            this.drpEmpType.Size = new System.Drawing.Size(176, 21);
            this.drpEmpType.TabIndex = 2;
            this.drpEmpType.SelectedIndexChanged += new System.EventHandler(this.drpEmpType_SelectedIndexChanged);
            // 
            // tabAccounts
            // 
            this.tabAccounts.IDEPixelArea = true;
            this.tabAccounts.Location = new System.Drawing.Point(8, 188);
            this.tabAccounts.Name = "tabAccounts";
            this.tabAccounts.PositionTop = true;
            this.tabAccounts.SelectedIndex = 0;
            this.tabAccounts.SelectedTab = this.tabAccountDetails;
            this.tabAccounts.Size = new System.Drawing.Size(760, 232);
            this.tabAccounts.TabIndex = 91;
            this.tabAccounts.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tabAccountDetails,
            this.tabAccountList,
            this.tabAllocateSingleAccount});
            this.tabAccounts.SelectionChanged += new System.EventHandler(this.tabAccounts_SelectionChanged);
            // 
            // tabAccountDetails
            // 
            this.tabAccountDetails.Controls.Add(this.tabSubAccountDetails);
            this.tabAccountDetails.Controls.Add(this.button1);
            this.tabAccountDetails.Controls.Add(this.button2);
            this.tabAccountDetails.Controls.Add(this.button3);
            this.tabAccountDetails.Controls.Add(this.groupBox1);
            this.tabAccountDetails.Controls.Add(this.label21);
            this.tabAccountDetails.Controls.Add(this.textBox7);
            this.tabAccountDetails.Controls.Add(this.textBox8);
            this.tabAccountDetails.Controls.Add(this.label22);
            this.tabAccountDetails.Controls.Add(this.textBox9);
            this.tabAccountDetails.Controls.Add(this.label23);
            this.tabAccountDetails.Controls.Add(this.textBox10);
            this.tabAccountDetails.Controls.Add(this.label24);
            this.tabAccountDetails.Controls.Add(this.label25);
            this.tabAccountDetails.Controls.Add(this.textBox11);
            this.tabAccountDetails.Controls.Add(this.textBox12);
            this.tabAccountDetails.Controls.Add(this.textBox13);
            this.tabAccountDetails.Controls.Add(this.label26);
            this.tabAccountDetails.Controls.Add(this.label27);
            this.tabAccountDetails.Controls.Add(this.textBox14);
            this.tabAccountDetails.Controls.Add(this.label28);
            this.tabAccountDetails.Controls.Add(this.label29);
            this.tabAccountDetails.Controls.Add(this.textBox15);
            this.tabAccountDetails.Location = new System.Drawing.Point(0, 25);
            this.tabAccountDetails.Name = "tabAccountDetails";
            this.tabAccountDetails.Size = new System.Drawing.Size(760, 207);
            this.tabAccountDetails.TabIndex = 3;
            this.tabAccountDetails.Title = "Account Details";
            // 
            // tabSubAccountDetails
            // 
            this.tabSubAccountDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSubAccountDetails.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.tabSubAccountDetails.IDEPixelArea = true;
            this.tabSubAccountDetails.Location = new System.Drawing.Point(0, 0);
            this.tabSubAccountDetails.Name = "tabSubAccountDetails";
            this.tabSubAccountDetails.PositionTop = true;
            this.tabSubAccountDetails.SelectedIndex = 0;
            this.tabSubAccountDetails.SelectedTab = this.tabCustomer;
            this.tabSubAccountDetails.Size = new System.Drawing.Size(760, 207);
            this.tabSubAccountDetails.TabIndex = 169;
            this.tabSubAccountDetails.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tabCustomer,
            this.tabProducts,
            this.tabStrategies,
            this.tabLetters});
            // 
            // tabCustomer
            // 
            this.tabCustomer.Controls.Add(this.txt_Provisions);
            this.tabCustomer.Controls.Add(this.btnReferences);
            this.tabCustomer.Controls.Add(this.label43);
            this.tabCustomer.Controls.Add(this.showMultipleAccounts);
            this.tabCustomer.Controls.Add(this.gbCustomer);
            this.tabCustomer.Controls.Add(this.btnAccountDetails);
            this.tabCustomer.Controls.Add(this.btnAddCode);
            this.tabCustomer.Controls.Add(this.btnFollowUp);
            this.tabCustomer.Controls.Add(this.label33);
            this.tabCustomer.Controls.Add(this.txtMonthArrears);
            this.tabCustomer.Controls.Add(this.txtPercentage);
            this.tabCustomer.Controls.Add(this.label34);
            this.tabCustomer.Controls.Add(this.txtDateLastPaid);
            this.tabCustomer.Controls.Add(this.label35);
            this.tabCustomer.Controls.Add(this.txtStatus);
            this.tabCustomer.Controls.Add(this.label36);
            this.tabCustomer.Controls.Add(this.label37);
            this.tabCustomer.Controls.Add(this.txtDueDate);
            this.tabCustomer.Controls.Add(this.txtArrears);
            this.tabCustomer.Controls.Add(this.txtBalance);
            this.tabCustomer.Controls.Add(this.label38);
            this.tabCustomer.Controls.Add(this.label40);
            this.tabCustomer.Controls.Add(this.txtAgreementTotal);
            this.tabCustomer.Controls.Add(this.label41);
            this.tabCustomer.Controls.Add(this.label44);
            this.tabCustomer.Controls.Add(this.txtInstalment);
            this.tabCustomer.Controls.Add(this.lMoreRewardsDate2);
            this.tabCustomer.Controls.Add(this.dtMoreRewardsDate2);
            this.tabCustomer.Controls.Add(this.lMoreRewards2);
            this.tabCustomer.Controls.Add(this.txtMoreRewards2);
            this.tabCustomer.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.tabCustomer.Location = new System.Drawing.Point(0, 25);
            this.tabCustomer.Name = "tabCustomer";
            this.tabCustomer.Size = new System.Drawing.Size(760, 182);
            this.tabCustomer.TabIndex = 0;
            this.tabCustomer.Title = "Customer/Account Information";
            // 
            // txt_Provisions
            // 
            this.txt_Provisions.Location = new System.Drawing.Point(227, 133);
            this.txt_Provisions.Name = "txt_Provisions";
            this.txt_Provisions.ReadOnly = true;
            this.txt_Provisions.Size = new System.Drawing.Size(77, 20);
            this.txt_Provisions.TabIndex = 175;
            this.txt_Provisions.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnReferences
            // 
            this.btnReferences.Location = new System.Drawing.Point(595, 101);
            this.btnReferences.Name = "btnReferences";
            this.btnReferences.Size = new System.Drawing.Size(162, 23);
            this.btnReferences.TabIndex = 174;
            this.btnReferences.Text = "View References";
            this.btnReferences.UseVisualStyleBackColor = true;
            this.btnReferences.Click += new System.EventHandler(this.btnReferences_Click);
            // 
            // label43
            // 
            this.label43.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label43.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label43.Location = new System.Drawing.Point(212, 110);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(98, 20);
            this.label43.TabIndex = 171;
            this.label43.Text = "Provision Amount";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // showMultipleAccounts
            // 
            this.showMultipleAccounts.AutoSize = true;
            this.showMultipleAccounts.Enabled = false;
            this.showMultipleAccounts.Location = new System.Drawing.Point(665, 161);
            this.showMultipleAccounts.Name = "showMultipleAccounts";
            this.showMultipleAccounts.Size = new System.Drawing.Size(0, 14);
            this.showMultipleAccounts.TabIndex = 170;
            this.showMultipleAccounts.Visible = false;
            // 
            // gbCustomer
            // 
            this.gbCustomer.Controls.Add(this.txtTitle);
            this.gbCustomer.Controls.Add(this.Title);
            this.gbCustomer.Controls.Add(this.label10);
            this.gbCustomer.Controls.Add(this.label11);
            this.gbCustomer.Controls.Add(this.label12);
            this.gbCustomer.Controls.Add(this.txtMobile);
            this.gbCustomer.Controls.Add(this.txtFirstName);
            this.gbCustomer.Controls.Add(this.label13);
            this.gbCustomer.Controls.Add(this.txtLastName);
            this.gbCustomer.Controls.Add(this.txtWork);
            this.gbCustomer.Controls.Add(this.label17);
            this.gbCustomer.Controls.Add(this.txtHome);
            this.gbCustomer.Location = new System.Drawing.Point(316, 3);
            this.gbCustomer.Name = "gbCustomer";
            this.gbCustomer.Size = new System.Drawing.Size(268, 178);
            this.gbCustomer.TabIndex = 169;
            this.gbCustomer.TabStop = false;
            this.gbCustomer.Text = "Customer Details";
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(96, 20);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.ReadOnly = true;
            this.txtTitle.Size = new System.Drawing.Size(57, 20);
            this.txtTitle.TabIndex = 133;
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Location = new System.Drawing.Point(63, 23);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(26, 14);
            this.Title.TabIndex = 134;
            this.Title.Text = "Title";
            // 
            // label10
            // 
            this.label10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label10.Location = new System.Drawing.Point(27, 49);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(69, 16);
            this.label10.TabIndex = 130;
            this.label10.Text = "First Name";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(53, 158);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 14);
            this.label11.TabIndex = 138;
            this.label11.Text = "Mobile";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(25, 131);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 14);
            this.label12.TabIndex = 140;
            this.label12.Text = "Work Phone";
            // 
            // txtMobile
            // 
            this.txtMobile.Location = new System.Drawing.Point(96, 155);
            this.txtMobile.Name = "txtMobile";
            this.txtMobile.ReadOnly = true;
            this.txtMobile.Size = new System.Drawing.Size(100, 20);
            this.txtMobile.TabIndex = 137;
            // 
            // txtFirstName
            // 
            this.txtFirstName.Location = new System.Drawing.Point(96, 47);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.ReadOnly = true;
            this.txtFirstName.Size = new System.Drawing.Size(154, 20);
            this.txtFirstName.TabIndex = 129;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(23, 104);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 14);
            this.label13.TabIndex = 139;
            this.label13.Text = "Home Phone";
            // 
            // txtLastName
            // 
            this.txtLastName.Location = new System.Drawing.Point(96, 74);
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.ReadOnly = true;
            this.txtLastName.Size = new System.Drawing.Size(154, 20);
            this.txtLastName.TabIndex = 131;
            // 
            // txtWork
            // 
            this.txtWork.Location = new System.Drawing.Point(96, 128);
            this.txtWork.Name = "txtWork";
            this.txtWork.ReadOnly = true;
            this.txtWork.Size = new System.Drawing.Size(100, 20);
            this.txtWork.TabIndex = 135;
            // 
            // label17
            // 
            this.label17.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label17.Location = new System.Drawing.Point(27, 75);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(63, 16);
            this.label17.TabIndex = 132;
            this.label17.Text = "Last Name";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtHome
            // 
            this.txtHome.Location = new System.Drawing.Point(96, 101);
            this.txtHome.Name = "txtHome";
            this.txtHome.ReadOnly = true;
            this.txtHome.Size = new System.Drawing.Size(100, 20);
            this.txtHome.TabIndex = 136;
            // 
            // btnAccountDetails
            // 
            this.btnAccountDetails.Location = new System.Drawing.Point(595, 70);
            this.btnAccountDetails.Name = "btnAccountDetails";
            this.btnAccountDetails.Size = new System.Drawing.Size(162, 23);
            this.btnAccountDetails.TabIndex = 168;
            this.btnAccountDetails.Text = "Account Details";
            this.btnAccountDetails.UseVisualStyleBackColor = true;
            this.btnAccountDetails.Click += new System.EventHandler(this.btnAccountDetails_Click);
            // 
            // btnAddCode
            // 
            this.btnAddCode.Enabled = false;
            this.btnAddCode.Location = new System.Drawing.Point(595, 39);
            this.btnAddCode.Name = "btnAddCode";
            this.btnAddCode.Size = new System.Drawing.Size(162, 23);
            this.btnAddCode.TabIndex = 167;
            this.btnAddCode.Text = "Add Account/Customer Code";
            this.btnAddCode.UseVisualStyleBackColor = true;
            this.btnAddCode.Click += new System.EventHandler(this.btnAddCode_Click);
            // 
            // btnFollowUp
            // 
            this.btnFollowUp.Location = new System.Drawing.Point(595, 8);
            this.btnFollowUp.Name = "btnFollowUp";
            this.btnFollowUp.Size = new System.Drawing.Size(162, 23);
            this.btnFollowUp.TabIndex = 166;
            this.btnFollowUp.Text = "Follow Up Action Details";
            this.btnFollowUp.UseVisualStyleBackColor = true;
            this.btnFollowUp.Click += new System.EventHandler(this.btnFollowUp_Click);
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(16, 136);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(94, 14);
            this.label33.TabIndex = 164;
            this.label33.Text = "Months in Arrears";
            // 
            // txtMonthArrears
            // 
            this.txtMonthArrears.BackColor = System.Drawing.SystemColors.Control;
            this.txtMonthArrears.Location = new System.Drawing.Point(114, 133);
            this.txtMonthArrears.Name = "txtMonthArrears";
            this.txtMonthArrears.Size = new System.Drawing.Size(104, 20);
            this.txtMonthArrears.TabIndex = 163;
            // 
            // txtPercentage
            // 
            this.txtPercentage.Location = new System.Drawing.Point(227, 81);
            this.txtPercentage.Name = "txtPercentage";
            this.txtPercentage.ReadOnly = true;
            this.txtPercentage.Size = new System.Drawing.Size(77, 20);
            this.txtPercentage.TabIndex = 162;
            // 
            // label34
            // 
            this.label34.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label34.Location = new System.Drawing.Point(221, 60);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(89, 16);
            this.label34.TabIndex = 161;
            this.label34.Text = "Percentage Paid";
            this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDateLastPaid
            // 
            this.txtDateLastPaid.Location = new System.Drawing.Point(114, 157);
            this.txtDateLastPaid.Name = "txtDateLastPaid";
            this.txtDateLastPaid.ReadOnly = true;
            this.txtDateLastPaid.Size = new System.Drawing.Size(104, 20);
            this.txtDateLastPaid.TabIndex = 160;
            // 
            // label35
            // 
            this.label35.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label35.Location = new System.Drawing.Point(24, 157);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(84, 16);
            this.label35.TabIndex = 159;
            this.label35.Text = "Date Last Paid";
            this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(227, 31);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(83, 20);
            this.txtStatus.TabIndex = 158;
            // 
            // label36
            // 
            this.label36.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label36.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label36.Location = new System.Drawing.Point(221, 10);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(89, 16);
            this.label36.TabIndex = 157;
            this.label36.Text = "Current Status";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label37
            // 
            this.label37.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label37.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label37.Location = new System.Drawing.Point(28, 58);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(80, 16);
            this.label37.TabIndex = 156;
            this.label37.Text = "Due Day";
            this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDueDate
            // 
            this.txtDueDate.BackColor = System.Drawing.SystemColors.Control;
            this.txtDueDate.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtDueDate.Location = new System.Drawing.Point(114, 55);
            this.txtDueDate.MaxLength = 20;
            this.txtDueDate.Name = "txtDueDate";
            this.txtDueDate.ReadOnly = true;
            this.txtDueDate.Size = new System.Drawing.Size(104, 21);
            this.txtDueDate.TabIndex = 155;
            this.txtDueDate.TabStop = false;
            // 
            // txtArrears
            // 
            this.txtArrears.BackColor = System.Drawing.SystemColors.Control;
            this.txtArrears.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtArrears.Location = new System.Drawing.Point(114, 106);
            this.txtArrears.MaxLength = 10;
            this.txtArrears.Name = "txtArrears";
            this.txtArrears.ReadOnly = true;
            this.txtArrears.Size = new System.Drawing.Size(104, 21);
            this.txtArrears.TabIndex = 150;
            this.txtArrears.TabStop = false;
            // 
            // txtBalance
            // 
            this.txtBalance.BackColor = System.Drawing.SystemColors.Control;
            this.txtBalance.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtBalance.Location = new System.Drawing.Point(114, 80);
            this.txtBalance.MaxLength = 10;
            this.txtBalance.Name = "txtBalance";
            this.txtBalance.ReadOnly = true;
            this.txtBalance.Size = new System.Drawing.Size(104, 21);
            this.txtBalance.TabIndex = 151;
            this.txtBalance.TabStop = false;
            // 
            // label38
            // 
            this.label38.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label38.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label38.Location = new System.Drawing.Point(36, 107);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(72, 16);
            this.label38.TabIndex = 147;
            this.label38.Text = "Arrears";
            this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label40
            // 
            this.label40.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label40.Location = new System.Drawing.Point(-4, 80);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(110, 19);
            this.label40.TabIndex = 148;
            this.label40.Text = "Outstanding Balance";
            this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAgreementTotal
            // 
            this.txtAgreementTotal.BackColor = System.Drawing.SystemColors.Control;
            this.txtAgreementTotal.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtAgreementTotal.Location = new System.Drawing.Point(114, 3);
            this.txtAgreementTotal.MaxLength = 10;
            this.txtAgreementTotal.Name = "txtAgreementTotal";
            this.txtAgreementTotal.ReadOnly = true;
            this.txtAgreementTotal.Size = new System.Drawing.Size(104, 21);
            this.txtAgreementTotal.TabIndex = 149;
            this.txtAgreementTotal.TabStop = false;
            // 
            // label41
            // 
            this.label41.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label41.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label41.Location = new System.Drawing.Point(8, 30);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(101, 19);
            this.label41.TabIndex = 154;
            this.label41.Text = "Instalment Amount";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label44
            // 
            this.label44.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label44.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label44.Location = new System.Drawing.Point(8, 4);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(100, 16);
            this.label44.TabIndex = 152;
            this.label44.Text = "Agreement Total";
            this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtInstalment
            // 
            this.txtInstalment.BackColor = System.Drawing.SystemColors.Control;
            this.txtInstalment.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtInstalment.Location = new System.Drawing.Point(114, 30);
            this.txtInstalment.MaxLength = 10;
            this.txtInstalment.Name = "txtInstalment";
            this.txtInstalment.ReadOnly = true;
            this.txtInstalment.Size = new System.Drawing.Size(104, 21);
            this.txtInstalment.TabIndex = 153;
            this.txtInstalment.TabStop = false;
            // 
            // lMoreRewardsDate2
            // 
            this.lMoreRewardsDate2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lMoreRewardsDate2.Location = new System.Drawing.Point(416, 223);
            this.lMoreRewardsDate2.Name = "lMoreRewardsDate2";
            this.lMoreRewardsDate2.Size = new System.Drawing.Size(196, 18);
            this.lMoreRewardsDate2.TabIndex = 0;
            this.lMoreRewardsDate2.Text = "Loyalty Card Effective Date:";
            // 
            // dtMoreRewardsDate2
            // 
            this.dtMoreRewardsDate2.CustomFormat = "ddd dd MMM yyyy";
            this.dtMoreRewardsDate2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtMoreRewardsDate2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtMoreRewardsDate2.Location = new System.Drawing.Point(416, 247);
            this.dtMoreRewardsDate2.Name = "dtMoreRewardsDate2";
            this.dtMoreRewardsDate2.Size = new System.Drawing.Size(130, 20);
            this.dtMoreRewardsDate2.TabIndex = 9;
            this.dtMoreRewardsDate2.Tag = "lMoreRewardsDate2";
            this.dtMoreRewardsDate2.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            // 
            // lMoreRewards2
            // 
            this.lMoreRewards2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lMoreRewards2.Location = new System.Drawing.Point(264, 223);
            this.lMoreRewards2.Name = "lMoreRewards2";
            this.lMoreRewards2.Size = new System.Drawing.Size(121, 18);
            this.lMoreRewards2.TabIndex = 0;
            this.lMoreRewards2.Text = "Loyalty Card No:";
            // 
            // txtMoreRewards2
            // 
            this.txtMoreRewards2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtMoreRewards2.Location = new System.Drawing.Point(264, 247);
            this.txtMoreRewards2.MaxLength = 30;
            this.txtMoreRewards2.Name = "txtMoreRewards2";
            this.txtMoreRewards2.Size = new System.Drawing.Size(117, 20);
            this.txtMoreRewards2.TabIndex = 8;
            this.txtMoreRewards2.Tag = "lMoreRewards2";
            // 
            // tabProducts
            // 
            this.tabProducts.Controls.Add(this.gbTransactions);
            this.tabProducts.Controls.Add(this.gbProducts);
            this.tabProducts.Location = new System.Drawing.Point(0, 25);
            this.tabProducts.Name = "tabProducts";
            this.tabProducts.Selected = false;
            this.tabProducts.Size = new System.Drawing.Size(760, 182);
            this.tabProducts.TabIndex = 1;
            this.tabProducts.Title = "Products/Transactions";
            // 
            // gbTransactions
            // 
            this.gbTransactions.Controls.Add(this.dgTransactions);
            this.gbTransactions.Location = new System.Drawing.Point(525, 8);
            this.gbTransactions.Name = "gbTransactions";
            this.gbTransactions.Size = new System.Drawing.Size(217, 174);
            this.gbTransactions.TabIndex = 3;
            this.gbTransactions.TabStop = false;
            this.gbTransactions.Text = "Transactions";
            // 
            // dgTransactions
            // 
            this.dgTransactions.AllowUserToAddRows = false;
            this.dgTransactions.AllowUserToDeleteRows = false;
            this.dgTransactions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgTransactions.Location = new System.Drawing.Point(7, 18);
            this.dgTransactions.Name = "dgTransactions";
            this.dgTransactions.ReadOnly = true;
            this.dgTransactions.RowHeadersVisible = false;
            this.dgTransactions.Size = new System.Drawing.Size(200, 152);
            this.dgTransactions.TabIndex = 1;
            // 
            // gbProducts
            // 
            this.gbProducts.Controls.Add(this.dgProducts);
            this.gbProducts.Location = new System.Drawing.Point(11, 8);
            this.gbProducts.Name = "gbProducts";
            this.gbProducts.Size = new System.Drawing.Size(505, 174);
            this.gbProducts.TabIndex = 2;
            this.gbProducts.TabStop = false;
            this.gbProducts.Text = "Products";
            // 
            // dgProducts
            // 
            this.dgProducts.AllowUserToAddRows = false;
            this.dgProducts.AllowUserToDeleteRows = false;
            this.dgProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgProducts.Location = new System.Drawing.Point(7, 18);
            this.dgProducts.Name = "dgProducts";
            this.dgProducts.ReadOnly = true;
            this.dgProducts.RowHeadersVisible = false;
            this.dgProducts.Size = new System.Drawing.Size(490, 152);
            this.dgProducts.TabIndex = 1;
            // 
            // tabStrategies
            // 
            this.tabStrategies.Controls.Add(this.gbWorklists);
            this.tabStrategies.Controls.Add(this.gbStrategies);
            this.tabStrategies.Location = new System.Drawing.Point(0, 25);
            this.tabStrategies.Name = "tabStrategies";
            this.tabStrategies.Selected = false;
            this.tabStrategies.Size = new System.Drawing.Size(760, 182);
            this.tabStrategies.TabIndex = 2;
            this.tabStrategies.Title = "Strategies/Worklists";
            // 
            // gbWorklists
            // 
            this.gbWorklists.Controls.Add(this.dgWorklists);
            this.gbWorklists.Location = new System.Drawing.Point(385, 8);
            this.gbWorklists.Name = "gbWorklists";
            this.gbWorklists.Size = new System.Drawing.Size(365, 174);
            this.gbWorklists.TabIndex = 3;
            this.gbWorklists.TabStop = false;
            this.gbWorklists.Text = "Worklists";
            // 
            // dgWorklists
            // 
            this.dgWorklists.AllowUserToAddRows = false;
            this.dgWorklists.AllowUserToDeleteRows = false;
            this.dgWorklists.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgWorklists.Location = new System.Drawing.Point(7, 18);
            this.dgWorklists.Name = "dgWorklists";
            this.dgWorklists.ReadOnly = true;
            this.dgWorklists.RowHeadersVisible = false;
            this.dgWorklists.Size = new System.Drawing.Size(348, 152);
            this.dgWorklists.TabIndex = 0;
            // 
            // gbStrategies
            // 
            this.gbStrategies.Controls.Add(this.dgStrategies);
            this.gbStrategies.Location = new System.Drawing.Point(11, 8);
            this.gbStrategies.Name = "gbStrategies";
            this.gbStrategies.Size = new System.Drawing.Size(365, 174);
            this.gbStrategies.TabIndex = 2;
            this.gbStrategies.TabStop = false;
            this.gbStrategies.Text = "Strategies";
            // 
            // dgStrategies
            // 
            this.dgStrategies.AllowUserToAddRows = false;
            this.dgStrategies.AllowUserToDeleteRows = false;
            this.dgStrategies.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgStrategies.Location = new System.Drawing.Point(7, 18);
            this.dgStrategies.Name = "dgStrategies";
            this.dgStrategies.ReadOnly = true;
            this.dgStrategies.RowHeadersVisible = false;
            this.dgStrategies.Size = new System.Drawing.Size(348, 152);
            this.dgStrategies.TabIndex = 1;
            // 
            // tabLetters
            // 
            this.tabLetters.Controls.Add(this.gbSMS);
            this.tabLetters.Controls.Add(this.gbLetters);
            this.tabLetters.Location = new System.Drawing.Point(0, 25);
            this.tabLetters.Name = "tabLetters";
            this.tabLetters.Selected = false;
            this.tabLetters.Size = new System.Drawing.Size(760, 182);
            this.tabLetters.TabIndex = 3;
            this.tabLetters.Title = "Letters/SMS\'s";
            // 
            // gbSMS
            // 
            this.gbSMS.Controls.Add(this.dgSMS);
            this.gbSMS.Location = new System.Drawing.Point(385, 8);
            this.gbSMS.Name = "gbSMS";
            this.gbSMS.Size = new System.Drawing.Size(365, 174);
            this.gbSMS.TabIndex = 3;
            this.gbSMS.TabStop = false;
            this.gbSMS.Text = "SMS\'s";
            // 
            // dgSMS
            // 
            this.dgSMS.AllowUserToAddRows = false;
            this.dgSMS.AllowUserToDeleteRows = false;
            this.dgSMS.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgSMS.Location = new System.Drawing.Point(7, 18);
            this.dgSMS.Name = "dgSMS";
            this.dgSMS.ReadOnly = true;
            this.dgSMS.RowHeadersVisible = false;
            this.dgSMS.Size = new System.Drawing.Size(348, 152);
            this.dgSMS.TabIndex = 1;
            this.dgSMS.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgSMS_MouseUp);
            // 
            // gbLetters
            // 
            this.gbLetters.Controls.Add(this.dgLetters);
            this.gbLetters.Location = new System.Drawing.Point(11, 8);
            this.gbLetters.Name = "gbLetters";
            this.gbLetters.Size = new System.Drawing.Size(365, 174);
            this.gbLetters.TabIndex = 3;
            this.gbLetters.TabStop = false;
            this.gbLetters.Text = "Letters";
            // 
            // dgLetters
            // 
            this.dgLetters.AllowUserToAddRows = false;
            this.dgLetters.AllowUserToDeleteRows = false;
            this.dgLetters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgLetters.Location = new System.Drawing.Point(7, 18);
            this.dgLetters.Name = "dgLetters";
            this.dgLetters.ReadOnly = true;
            this.dgLetters.RowHeadersVisible = false;
            this.dgLetters.Size = new System.Drawing.Size(348, 152);
            this.dgLetters.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(665, 200);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(89, 23);
            this.button1.TabIndex = 168;
            this.button1.Text = "Account Details";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(480, 200);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(165, 23);
            this.button2.TabIndex = 167;
            this.button2.Text = "Add Account/Customer Code";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(328, 200);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(130, 23);
            this.button3.TabIndex = 166;
            this.button3.Text = "Follow Up Action Details";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.textBox4);
            this.groupBox1.Controls.Add(this.textBox5);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.textBox6);
            this.groupBox1.Location = new System.Drawing.Point(368, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(304, 182);
            this.groupBox1.TabIndex = 165;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Customer Details";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(96, 20);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(57, 23);
            this.textBox1.TabIndex = 133;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(63, 23);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(44, 15);
            this.label14.TabIndex = 134;
            this.label14.Text = "label14";
            // 
            // label15
            // 
            this.label15.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label15.Location = new System.Drawing.Point(20, 48);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(70, 16);
            this.label15.TabIndex = 130;
            this.label15.Text = "First Name:";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(53, 158);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(44, 15);
            this.label16.TabIndex = 138;
            this.label16.Text = "Mobile";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(25, 131);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(72, 15);
            this.label18.TabIndex = 140;
            this.label18.Text = "Work Phone";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(96, 155);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 23);
            this.textBox2.TabIndex = 137;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(96, 47);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(184, 23);
            this.textBox3.TabIndex = 129;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(23, 104);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(77, 15);
            this.label19.TabIndex = 139;
            this.label19.Text = "Home Phone";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(96, 74);
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(184, 23);
            this.textBox4.TabIndex = 131;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(96, 128);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(100, 23);
            this.textBox5.TabIndex = 135;
            // 
            // label20
            // 
            this.label20.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label20.Location = new System.Drawing.Point(27, 75);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(63, 16);
            this.label20.TabIndex = 132;
            this.label20.Text = "Last Name:";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(96, 101);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(100, 23);
            this.textBox6.TabIndex = 136;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(16, 149);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(101, 15);
            this.label21.TabIndex = 164;
            this.label21.Text = "Months in Arrears";
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(114, 146);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(104, 23);
            this.textBox7.TabIndex = 163;
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(239, 200);
            this.textBox8.Name = "textBox8";
            this.textBox8.ReadOnly = true;
            this.textBox8.Size = new System.Drawing.Size(40, 23);
            this.textBox8.TabIndex = 162;
            // 
            // label22
            // 
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label22.Location = new System.Drawing.Point(144, 201);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(89, 16);
            this.label22.TabIndex = 161;
            this.label22.Text = "Percentage Paid";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(114, 173);
            this.textBox9.Name = "textBox9";
            this.textBox9.ReadOnly = true;
            this.textBox9.Size = new System.Drawing.Size(104, 23);
            this.textBox9.TabIndex = 160;
            // 
            // label23
            // 
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label23.Location = new System.Drawing.Point(24, 174);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(84, 16);
            this.label23.TabIndex = 159;
            this.label23.Text = "Date Last Paid";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(114, 200);
            this.textBox10.Name = "textBox10";
            this.textBox10.ReadOnly = true;
            this.textBox10.Size = new System.Drawing.Size(24, 23);
            this.textBox10.TabIndex = 158;
            // 
            // label24
            // 
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label24.Location = new System.Drawing.Point(19, 201);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(89, 16);
            this.label24.TabIndex = 157;
            this.label24.Text = "Current Status";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label25
            // 
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label25.Location = new System.Drawing.Point(28, 67);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(80, 16);
            this.label25.TabIndex = 156;
            this.label25.Text = "Due Date";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox11
            // 
            this.textBox11.BackColor = System.Drawing.SystemColors.Window;
            this.textBox11.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.textBox11.Location = new System.Drawing.Point(114, 66);
            this.textBox11.MaxLength = 20;
            this.textBox11.Name = "textBox11";
            this.textBox11.ReadOnly = true;
            this.textBox11.Size = new System.Drawing.Size(104, 21);
            this.textBox11.TabIndex = 155;
            this.textBox11.TabStop = false;
            // 
            // textBox12
            // 
            this.textBox12.BackColor = System.Drawing.SystemColors.Window;
            this.textBox12.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.textBox12.Location = new System.Drawing.Point(114, 119);
            this.textBox12.MaxLength = 10;
            this.textBox12.Name = "textBox12";
            this.textBox12.ReadOnly = true;
            this.textBox12.Size = new System.Drawing.Size(104, 21);
            this.textBox12.TabIndex = 150;
            this.textBox12.TabStop = false;
            // 
            // textBox13
            // 
            this.textBox13.BackColor = System.Drawing.SystemColors.Window;
            this.textBox13.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.textBox13.Location = new System.Drawing.Point(114, 93);
            this.textBox13.MaxLength = 10;
            this.textBox13.Name = "textBox13";
            this.textBox13.ReadOnly = true;
            this.textBox13.Size = new System.Drawing.Size(104, 21);
            this.textBox13.TabIndex = 151;
            this.textBox13.TabStop = false;
            // 
            // label26
            // 
            this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label26.Location = new System.Drawing.Point(36, 120);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(72, 16);
            this.label26.TabIndex = 147;
            this.label26.Text = "Arrears";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label27
            // 
            this.label27.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label27.Location = new System.Drawing.Point(6, 93);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(110, 19);
            this.label27.TabIndex = 148;
            this.label27.Text = "Outstanding Balance";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox14
            // 
            this.textBox14.BackColor = System.Drawing.SystemColors.Window;
            this.textBox14.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.textBox14.Location = new System.Drawing.Point(114, 12);
            this.textBox14.MaxLength = 10;
            this.textBox14.Name = "textBox14";
            this.textBox14.ReadOnly = true;
            this.textBox14.Size = new System.Drawing.Size(104, 21);
            this.textBox14.TabIndex = 149;
            this.textBox14.TabStop = false;
            // 
            // label28
            // 
            this.label28.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label28.Location = new System.Drawing.Point(8, 39);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(101, 19);
            this.label28.TabIndex = 154;
            this.label28.Text = "Instalment Amount";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label29
            // 
            this.label29.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label29.Location = new System.Drawing.Point(8, 13);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(100, 16);
            this.label29.TabIndex = 152;
            this.label29.Text = "Agreement Total";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox15
            // 
            this.textBox15.BackColor = System.Drawing.SystemColors.Window;
            this.textBox15.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.textBox15.Location = new System.Drawing.Point(114, 39);
            this.textBox15.MaxLength = 10;
            this.textBox15.Name = "textBox15";
            this.textBox15.ReadOnly = true;
            this.textBox15.Size = new System.Drawing.Size(104, 21);
            this.textBox15.TabIndex = 153;
            this.textBox15.TabStop = false;
            // 
            // tabAccountList
            // 
            this.tabAccountList.BackColor = System.Drawing.SystemColors.Control;
            this.tabAccountList.Controls.Add(this.dgAccounts);
            this.tabAccountList.Location = new System.Drawing.Point(0, 25);
            this.tabAccountList.Name = "tabAccountList";
            this.tabAccountList.Padding = new System.Windows.Forms.Padding(3);
            this.tabAccountList.Selected = false;
            this.tabAccountList.Size = new System.Drawing.Size(760, 207);
            this.tabAccountList.TabIndex = 1;
            this.tabAccountList.Title = "Accounts";
            // 
            // dgAccounts
            // 
            this.dgAccounts.CaptionText = "Accounts in Worklist";
            this.dgAccounts.DataMember = "";
            this.dgAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAccounts.Location = new System.Drawing.Point(5, 6);
            this.dgAccounts.Name = "dgAccounts";
            this.dgAccounts.ReadOnly = true;
            this.dgAccounts.Size = new System.Drawing.Size(751, 204);
            this.dgAccounts.TabIndex = 1;
            this.dgAccounts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgAccounts_MouseUp);
            // 
            // tabAllocateSingleAccount
            // 
            this.tabAllocateSingleAccount.Controls.Add(this.txt_provisionamount);
            this.tabAllocateSingleAccount.Controls.Add(this.label45);
            this.tabAllocateSingleAccount.Controls.Add(this.lblshowSingleAccount);
            this.tabAllocateSingleAccount.Controls.Add(this.btnAllocateAcct);
            this.tabAllocateSingleAccount.Controls.Add(this.drpEmpNameAcct);
            this.tabAllocateSingleAccount.Controls.Add(this.drpEmployeeTypesAcct);
            this.tabAllocateSingleAccount.Controls.Add(this.drpBranchAcct);
            this.tabAllocateSingleAccount.Controls.Add(this.label31);
            this.tabAllocateSingleAccount.Controls.Add(this.label32);
            this.tabAllocateSingleAccount.Controls.Add(this.label39);
            this.tabAllocateSingleAccount.Controls.Add(this.btnClearAcct);
            this.tabAllocateSingleAccount.Controls.Add(this.label42);
            this.tabAllocateSingleAccount.Controls.Add(this.txtAcctNo);
            this.tabAllocateSingleAccount.Location = new System.Drawing.Point(0, 25);
            this.tabAllocateSingleAccount.Name = "tabAllocateSingleAccount";
            this.tabAllocateSingleAccount.Selected = false;
            this.tabAllocateSingleAccount.Size = new System.Drawing.Size(760, 207);
            this.tabAllocateSingleAccount.TabIndex = 4;
            this.tabAllocateSingleAccount.Title = "Allocate Single Account";
            // 
            // txt_provisionamount
            // 
            this.txt_provisionamount.Location = new System.Drawing.Point(34, 136);
            this.txt_provisionamount.Name = "txt_provisionamount";
            this.txt_provisionamount.ReadOnly = true;
            this.txt_provisionamount.Size = new System.Drawing.Size(99, 23);
            this.txt_provisionamount.TabIndex = 173;
            this.txt_provisionamount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(36, 112);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(103, 15);
            this.label45.TabIndex = 133;
            this.label45.Text = "Provision Amount";
            // 
            // lblshowSingleAccount
            // 
            this.lblshowSingleAccount.AutoSize = true;
            this.lblshowSingleAccount.Location = new System.Drawing.Point(31, 176);
            this.lblshowSingleAccount.Name = "lblshowSingleAccount";
            this.lblshowSingleAccount.Size = new System.Drawing.Size(0, 15);
            this.lblshowSingleAccount.TabIndex = 132;
            this.lblshowSingleAccount.Visible = false;
            // 
            // btnAllocateAcct
            // 
            this.btnAllocateAcct.Enabled = false;
            this.btnAllocateAcct.Location = new System.Drawing.Point(643, 62);
            this.btnAllocateAcct.Name = "btnAllocateAcct";
            this.btnAllocateAcct.Size = new System.Drawing.Size(72, 24);
            this.btnAllocateAcct.TabIndex = 131;
            this.btnAllocateAcct.Text = "Allocate";
            this.btnAllocateAcct.Click += new System.EventHandler(this.btnAllocateAcct_Click);
            // 
            // drpEmpNameAcct
            // 
            this.drpEmpNameAcct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmpNameAcct.Enabled = false;
            this.drpEmpNameAcct.Location = new System.Drawing.Point(412, 62);
            this.drpEmpNameAcct.Name = "drpEmpNameAcct";
            this.drpEmpNameAcct.Size = new System.Drawing.Size(184, 23);
            this.drpEmpNameAcct.TabIndex = 129;
            this.drpEmpNameAcct.SelectedIndexChanged += new System.EventHandler(this.drpEmpNameAcct_SelectedIndexChanged);
            // 
            // drpEmployeeTypesAcct
            // 
            this.drpEmployeeTypesAcct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmployeeTypesAcct.Location = new System.Drawing.Point(158, 62);
            this.drpEmployeeTypesAcct.Name = "drpEmployeeTypesAcct";
            this.drpEmployeeTypesAcct.Size = new System.Drawing.Size(128, 23);
            this.drpEmployeeTypesAcct.TabIndex = 128;
            this.drpEmployeeTypesAcct.SelectedIndexChanged += new System.EventHandler(this.drpEmployeeTypesAcct_SelectedIndexChanged);
            // 
            // drpBranchAcct
            // 
            this.drpBranchAcct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranchAcct.Location = new System.Drawing.Point(305, 62);
            this.drpBranchAcct.Name = "drpBranchAcct";
            this.drpBranchAcct.Size = new System.Drawing.Size(64, 23);
            this.drpBranchAcct.TabIndex = 130;
            this.drpBranchAcct.SelectedIndexChanged += new System.EventHandler(this.drpBranchAcct_SelectedIndexChanged);
            // 
            // label31
            // 
            this.label31.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.label31.Location = new System.Drawing.Point(302, 43);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(48, 16);
            this.label31.TabIndex = 127;
            this.label31.Text = "Branch";
            // 
            // label32
            // 
            this.label32.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.label32.Location = new System.Drawing.Point(155, 43);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(96, 16);
            this.label32.TabIndex = 125;
            this.label32.Text = "Employee Type";
            // 
            // label39
            // 
            this.label39.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.label39.Location = new System.Drawing.Point(409, 43);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(96, 16);
            this.label39.TabIndex = 124;
            this.label39.Text = "Employee Name";
            // 
            // btnClearAcct
            // 
            this.btnClearAcct.Location = new System.Drawing.Point(643, 102);
            this.btnClearAcct.Name = "btnClearAcct";
            this.btnClearAcct.Size = new System.Drawing.Size(72, 24);
            this.btnClearAcct.TabIndex = 123;
            this.btnClearAcct.Text = "Clear";
            this.btnClearAcct.Click += new System.EventHandler(this.btnClearAcct_Click);
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(34, 43);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(99, 15);
            this.label42.TabIndex = 122;
            this.label42.Text = "Account Number";
            // 
            // txtAcctNo
            // 
            this.txtAcctNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtAcctNo.Location = new System.Drawing.Point(37, 62);
            this.txtAcctNo.MaxLength = 20;
            this.txtAcctNo.Name = "txtAcctNo";
            this.txtAcctNo.PreventPaste = false;
            this.txtAcctNo.Size = new System.Drawing.Size(94, 23);
            this.txtAcctNo.TabIndex = 126;
            this.txtAcctNo.Text = "000-0000-0000-0";
            this.txtAcctNo.Leave += new System.EventHandler(this.txtAcctNo_Leave);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(80, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 16);
            this.label2.TabIndex = 60;
            this.label2.Text = "Employee Type";
            // 
            // gbReAllocate
            // 
            this.gbReAllocate.Controls.Add(this.label8);
            this.gbReAllocate.Controls.Add(this.label9);
            this.gbReAllocate.Controls.Add(this.drpRAEmpType);
            this.gbReAllocate.Controls.Add(this.drpRAEmpName);
            this.gbReAllocate.Controls.Add(this.btnReAllocate);
            this.gbReAllocate.Enabled = false;
            this.gbReAllocate.Location = new System.Drawing.Point(8, 422);
            this.gbReAllocate.Name = "gbReAllocate";
            this.gbReAllocate.Size = new System.Drawing.Size(534, 50);
            this.gbReAllocate.TabIndex = 90;
            this.gbReAllocate.TabStop = false;
            this.gbReAllocate.Text = "Re-Allocation";
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.SystemColors.Control;
            this.label8.Location = new System.Drawing.Point(75, 7);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(96, 16);
            this.label8.TabIndex = 93;
            this.label8.Text = "Employee Type";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(259, 7);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(147, 16);
            this.label9.TabIndex = 94;
            this.label9.Text = "Employee Name (Available)";
            // 
            // drpRAEmpType
            // 
            this.drpRAEmpType.ItemHeight = 13;
            this.drpRAEmpType.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.drpRAEmpType.Location = new System.Drawing.Point(78, 23);
            this.drpRAEmpType.Name = "drpRAEmpType";
            this.drpRAEmpType.Size = new System.Drawing.Size(176, 21);
            this.drpRAEmpType.TabIndex = 91;
            this.drpRAEmpType.SelectedIndexChanged += new System.EventHandler(this.drpRAEmpType_SelectedIndexChanged);
            // 
            // drpRAEmpName
            // 
            this.drpRAEmpName.Enabled = false;
            this.drpRAEmpName.ItemHeight = 13;
            this.drpRAEmpName.Location = new System.Drawing.Point(260, 23);
            this.drpRAEmpName.Name = "drpRAEmpName";
            this.drpRAEmpName.Size = new System.Drawing.Size(180, 21);
            this.drpRAEmpName.TabIndex = 92;
            this.drpRAEmpName.SelectedIndexChanged += new System.EventHandler(this.drpRAEmpName_SelectedIndexChanged);
            this.drpRAEmpName.MouseHover += new System.EventHandler(this.drpRAEmpName_MouseHover);
            // 
            // btnReAllocate
            // 
            this.btnReAllocate.Enabled = false;
            this.btnReAllocate.Location = new System.Drawing.Point(446, 10);
            this.btnReAllocate.Name = "btnReAllocate";
            this.btnReAllocate.Size = new System.Drawing.Size(72, 24);
            this.btnReAllocate.TabIndex = 90;
            this.btnReAllocate.Text = "Re-Allocate";
            this.btnReAllocate.Click += new System.EventHandler(this.btnReAllocate_Click);
            // 
            // btnDeAllocate
            // 
            this.btnDeAllocate.Enabled = false;
            this.btnDeAllocate.Location = new System.Drawing.Point(548, 445);
            this.btnDeAllocate.Name = "btnDeAllocate";
            this.btnDeAllocate.Size = new System.Drawing.Size(96, 24);
            this.btnDeAllocate.TabIndex = 84;
            this.btnDeAllocate.Text = "De-Allocate";
            this.btnDeAllocate.Visible = false;
            this.btnDeAllocate.Click += new System.EventHandler(this.btnDeAllocate_Click);
            // 
            // btnTempReceipt
            // 
            this.btnTempReceipt.Location = new System.Drawing.Point(650, 445);
            this.btnTempReceipt.Name = "btnTempReceipt";
            this.btnTempReceipt.Size = new System.Drawing.Size(120, 24);
            this.btnTempReceipt.TabIndex = 83;
            this.btnTempReceipt.Text = "Temporary Receipts";
            this.btnTempReceipt.Click += new System.EventHandler(this.btnTempReceipt_Click);
            // 
            // gbLastAction
            // 
            this.gbLastAction.Controls.Add(this.txtLastActionCode);
            this.gbLastAction.Controls.Add(this.label6);
            this.gbLastAction.Controls.Add(this.dtLastActionDate);
            this.gbLastAction.Controls.Add(this.label7);
            this.gbLastAction.Enabled = false;
            this.gbLastAction.Location = new System.Drawing.Point(8, 72);
            this.gbLastAction.Name = "gbLastAction";
            this.gbLastAction.Size = new System.Drawing.Size(176, 110);
            this.gbLastAction.TabIndex = 82;
            this.gbLastAction.TabStop = false;
            this.gbLastAction.Text = "Last Action";
            // 
            // txtLastActionCode
            // 
            this.txtLastActionCode.Location = new System.Drawing.Point(8, 72);
            this.txtLastActionCode.Name = "txtLastActionCode";
            this.txtLastActionCode.ReadOnly = true;
            this.txtLastActionCode.Size = new System.Drawing.Size(160, 20);
            this.txtLastActionCode.TabIndex = 88;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(8, 56);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 16);
            this.label6.TabIndex = 87;
            this.label6.Text = "Action Code";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dtLastActionDate
            // 
            this.dtLastActionDate.CustomFormat = "ddd dd MMM yyyy";
            this.dtLastActionDate.Enabled = false;
            this.dtLastActionDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtLastActionDate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtLastActionDate.Location = new System.Drawing.Point(8, 32);
            this.dtLastActionDate.Name = "dtLastActionDate";
            this.dtLastActionDate.Size = new System.Drawing.Size(112, 20);
            this.dtLastActionDate.TabIndex = 85;
            this.dtLastActionDate.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(8, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 16);
            this.label7.TabIndex = 86;
            this.label7.Text = "Date Added";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gbNewAction
            // 
            this.gbNewAction.Controls.Add(this.drpSendToStrategy);
            this.gbNewAction.Controls.Add(this.chCycleNext);
            this.gbNewAction.Controls.Add(this.drpReason);
            this.gbNewAction.Controls.Add(this.lbReason);
            this.gbNewAction.Controls.Add(this.label3);
            this.gbNewAction.Controls.Add(this.btnLoadExtraInfo);
            this.gbNewAction.Controls.Add(this.txtNotes);
            this.gbNewAction.Controls.Add(this.txtActionValue);
            this.gbNewAction.Controls.Add(this.drpActionCode);
            this.gbNewAction.Controls.Add(this.label1);
            this.gbNewAction.Controls.Add(this.btnSave);
            this.gbNewAction.Controls.Add(this.dtDueDate);
            this.gbNewAction.Controls.Add(this.lbActionValue);
            this.gbNewAction.Controls.Add(this.lbActionDate);
            this.gbNewAction.Enabled = false;
            this.gbNewAction.Location = new System.Drawing.Point(184, 72);
            this.gbNewAction.Name = "gbNewAction";
            this.gbNewAction.Size = new System.Drawing.Size(584, 110);
            this.gbNewAction.TabIndex = 81;
            this.gbNewAction.TabStop = false;
            this.gbNewAction.Text = "New Action";
            // 
            // drpSendToStrategy
            // 
            this.drpSendToStrategy.FormattingEnabled = true;
            this.drpSendToStrategy.Location = new System.Drawing.Point(325, 28);
            this.drpSendToStrategy.Name = "drpSendToStrategy";
            this.drpSendToStrategy.Size = new System.Drawing.Size(175, 21);
            this.drpSendToStrategy.TabIndex = 94;
            // 
            // chCycleNext
            // 
            this.chCycleNext.AutoSize = true;
            this.chCycleNext.Checked = true;
            this.chCycleNext.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chCycleNext.Location = new System.Drawing.Point(451, 80);
            this.chCycleNext.Name = "chCycleNext";
            this.chCycleNext.Size = new System.Drawing.Size(129, 17);
            this.chCycleNext.TabIndex = 93;
            this.chCycleNext.Text = "Cycle to next account";
            this.chCycleNext.UseVisualStyleBackColor = true;
            // 
            // drpReason
            // 
            this.drpReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpReason.Location = new System.Drawing.Point(461, 52);
            this.drpReason.Name = "drpReason";
            this.drpReason.Size = new System.Drawing.Size(120, 21);
            this.drpReason.TabIndex = 91;
            this.drpReason.Visible = false;
            // 
            // lbReason
            // 
            this.lbReason.Location = new System.Drawing.Point(408, 55);
            this.lbReason.Name = "lbReason";
            this.lbReason.Size = new System.Drawing.Size(48, 16);
            this.lbReason.TabIndex = 90;
            this.lbReason.Text = "Reason";
            this.lbReason.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbReason.Visible = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 16);
            this.label3.TabIndex = 89;
            this.label3.Text = "Notes";
            // 
            // btnLoadExtraInfo
            // 
            this.btnLoadExtraInfo.Location = new System.Drawing.Point(509, 17);
            this.btnLoadExtraInfo.Name = "btnLoadExtraInfo";
            this.btnLoadExtraInfo.Size = new System.Drawing.Size(72, 33);
            this.btnLoadExtraInfo.TabIndex = 88;
            this.btnLoadExtraInfo.Text = "SPA History";
            this.btnLoadExtraInfo.Click += new System.EventHandler(this.btnLoadExtraInfo_Click);
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(48, 56);
            this.txtNotes.MaxLength = 700;
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(344, 48);
            this.txtNotes.TabIndex = 87;
            // 
            // txtActionValue
            // 
            this.txtActionValue.Location = new System.Drawing.Point(328, 29);
            this.txtActionValue.Name = "txtActionValue";
            this.txtActionValue.Size = new System.Drawing.Size(96, 20);
            this.txtActionValue.TabIndex = 85;
            this.txtActionValue.Leave += new System.EventHandler(this.txtActionValue_Leave);
            // 
            // drpActionCode
            // 
            this.drpActionCode.ItemHeight = 13;
            this.drpActionCode.Items.AddRange(new object[] {
            "C",
            "R"});
            this.drpActionCode.Location = new System.Drawing.Point(135, 28);
            this.drpActionCode.Name = "drpActionCode";
            this.drpActionCode.Size = new System.Drawing.Size(184, 21);
            this.drpActionCode.TabIndex = 84;
            this.drpActionCode.SelectionChangeCommitted += new System.EventHandler(this.drpActionCode_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(132, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 83;
            this.label1.Text = "Action Code";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Location = new System.Drawing.Point(400, 80);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 80;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dtDueDate
            // 
            this.dtDueDate.CustomFormat = "ddd dd MMM yyyy";
            this.dtDueDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDueDate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtDueDate.Location = new System.Drawing.Point(16, 28);
            this.dtDueDate.Name = "dtDueDate";
            this.dtDueDate.Size = new System.Drawing.Size(112, 20);
            this.dtDueDate.TabIndex = 81;
            this.dtDueDate.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // lbActionValue
            // 
            this.lbActionValue.Location = new System.Drawing.Point(328, 13);
            this.lbActionValue.Name = "lbActionValue";
            this.lbActionValue.Size = new System.Drawing.Size(96, 16);
            this.lbActionValue.TabIndex = 86;
            this.lbActionValue.Text = "Action Value";
            this.lbActionValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbActionDate
            // 
            this.lbActionDate.Location = new System.Drawing.Point(13, 13);
            this.lbActionDate.Name = "lbActionDate";
            this.lbActionDate.Size = new System.Drawing.Size(104, 16);
            this.lbActionDate.TabIndex = 82;
            this.lbActionDate.Text = "Date Due";
            this.lbActionDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnExcel
            // 
            this.btnExcel.Enabled = false;
            this.btnExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnExcel.Image")));
            this.btnExcel.Location = new System.Drawing.Point(616, 24);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(32, 32);
            this.btnExcel.TabIndex = 80;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // lAccountNo
            // 
            this.lAccountNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lAccountNo.Location = new System.Drawing.Point(461, 9);
            this.lAccountNo.Name = "lAccountNo";
            this.lAccountNo.Size = new System.Drawing.Size(100, 16);
            this.lAccountNo.TabIndex = 78;
            this.lAccountNo.Text = "Account Number:";
            this.lAccountNo.Visible = false;
            // 
            // drpBranch
            // 
            this.drpBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranch.Location = new System.Drawing.Point(16, 24);
            this.drpBranch.Name = "drpBranch";
            this.drpBranch.Size = new System.Drawing.Size(48, 21);
            this.drpBranch.TabIndex = 1;
            this.drpBranch.SelectedIndexChanged += new System.EventHandler(this.drpBranch_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(16, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 16);
            this.label5.TabIndex = 76;
            this.label5.Text = "Branch";
            // 
            // btnPrint
            // 
            this.btnPrint.Enabled = false;
            this.btnPrint.Location = new System.Drawing.Point(664, 9);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(104, 24);
            this.btnPrint.TabIndex = 6;
            this.btnPrint.Text = "New Allocations";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(664, 39);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(104, 24);
            this.btnClear.TabIndex = 7;
            this.btnClear.Text = "&Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(272, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 16);
            this.label4.TabIndex = 72;
            this.label4.Text = "Employee Name";
            // 
            // btnSearch
            // 
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.Location = new System.Drawing.Point(568, 24);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(32, 32);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // drpEmpName
            // 
            this.drpEmpName.Enabled = false;
            this.drpEmpName.ItemHeight = 13;
            this.drpEmpName.Location = new System.Drawing.Point(272, 26);
            this.drpEmpName.Name = "drpEmpName";
            this.drpEmpName.Size = new System.Drawing.Size(176, 21);
            this.drpEmpName.TabIndex = 3;
            this.drpEmpName.SelectedIndexChanged += new System.EventHandler(this.drpEmpName_SelectedIndexChanged);
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.Enabled = false;
            this.txtAccountNo.Location = new System.Drawing.Point(464, 24);
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.PreventPaste = false;
            this.txtAccountNo.Size = new System.Drawing.Size(88, 20);
            this.txtAccountNo.TabIndex = 4;
            this.txtAccountNo.Tag = "ACCNO";
            this.txtAccountNo.Text = "000-0000-0000-0";
            this.txtAccountNo.Visible = false;
            this.txtAccountNo.TextChanged += new System.EventHandler(this.txtAccountNo_TextChanged);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataMember = "";
            // 
            // BailReview5_2
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox3);
            this.Name = "BailReview5_2";
            this.Text = "Bail Review";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabAccounts.ResumeLayout(false);
            this.tabAccountDetails.ResumeLayout(false);
            this.tabAccountDetails.PerformLayout();
            this.tabSubAccountDetails.ResumeLayout(false);
            this.tabCustomer.ResumeLayout(false);
            this.tabCustomer.PerformLayout();
            this.gbCustomer.ResumeLayout(false);
            this.gbCustomer.PerformLayout();
            this.tabProducts.ResumeLayout(false);
            this.gbTransactions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgTransactions)).EndInit();
            this.gbProducts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgProducts)).EndInit();
            this.tabStrategies.ResumeLayout(false);
            this.gbWorklists.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgWorklists)).EndInit();
            this.gbStrategies.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgStrategies)).EndInit();
            this.tabLetters.ResumeLayout(false);
            this.gbSMS.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgSMS)).EndInit();
            this.gbLetters.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgLetters)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabAccountList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
            this.tabAllocateSingleAccount.ResumeLayout(false);
            this.tabAllocateSingleAccount.PerformLayout();
            this.gbReAllocate.ResumeLayout(false);
            this.gbLastAction.ResumeLayout(false);
            this.gbLastAction.PerformLayout();
            this.gbNewAction.ResumeLayout(false);
            this.gbNewAction.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        #region --- Member Variables ----------------------------------------------------------------------
        private ArrayList allocatedAccounts; //IP - 01/06/09 - Credit Collection Walkthrough Changes. Moved 'Allocate Single Account' functionality from 'FollowUp5_2.cs'
        private bool staticLoaded = false;
        //private DataTable dtAllStrategyActions = new DataTable();
        private StringCollection creditstaff = new StringCollection(); //IP - 01/06/09 - Credit Collection Walkthrough Changes. Moved 'Allocate Single Account' functionality from 'FollowUp5_2.cs
        private bool m_strategyChanged = false;
        private string m_strategy = String.Empty;
        private string m_acct = String.Empty;
        private XmlDocument doc = null;
        private DataView dvStrategyActions = new DataView();
        private bool m_addCodes = false;
        private string employee = "";
        private Label lblshowSingleAccount; //IP - 01/06/09 - Credit Collection Walkthrough Changes. Moved 'Allocate Single Account' functionality from 'FollowUp5_2.cs'
        private int m_empeeNo = 0;
        private DataTable dtSPADetails = new DataTable();   //jec CR976 17/09/09        
        private string m_custid = String.Empty;
        private string m_accttype = string.Empty;
        private string m_name = string.Empty;
        private bool m_creditblocked = false;               //IP - 28/04/10 - UAT(983) UAT5.2
        //private bool m_valid_account = false;
        private int m_accountRow = -1;
        private DataTable dtSMSDefinition;
        private DataTable dtLegalDetail;        //jec 05/07/10 UAT1040
        private DataTable dtInsuranceDetail;
        private DataTable dtFraudDetail;
        private DataTable dtTRCDetail;
        private bool reversedBDW; //jec - 08/07/10 - UAT1065


        #endregion --- End of Member Variables ------------------------------------------------------------

        #region --- Properties Variables ------------------------------------------------------------------
        public int EmpeeNo
        {
            get
            {
                return m_empeeNo;
            }
            set
            {
                m_empeeNo = value;
            }
        }

        public bool StrategyChanged
        {
            get
            {
                return m_strategyChanged;
            }
            set
            {
                m_strategyChanged = value;
            }
        }

        public string Strategy
        {
            get
            {
                return m_strategy;
            }
            set
            {
                m_strategy = value;
            }
        }

        public string AcctNo
        {
            get
            {
                return m_acct;
            }
            set
            {
                m_acct = value;
            }
        }

        public bool AddCodes
        {
            get
            {
                return m_addCodes;
            }
            set
            {
                m_addCodes = value;
            }
        }

        public string CurrentActionCode
        {
            get
            {
                return (drpActionCode.Items.Count > 0 && drpActionCode.SelectedIndex >= 0) ? drpActionCode.SelectedValue.ToString() : "";
            }
        }

        public bool SPASelected
        {
            get
            {
                return (CurrentActionCode == "SPA");
            }
        }

        public bool STSSelected
        {
            get
            {
                return (CurrentActionCode == "STS"); //IP - 20/10/08 - UAT5.2 - UAT(551) - Check if 'Send To Strategy' action code has been selected.
            }
        }

        public bool RALLSelected
        {
            get
            {
                return (CurrentActionCode == "RALL"); //UAT5.2 - UAT(754) - Reallocate.
            }
        }
        //jec 05/07/10 - UAT1040 - Extra Bailiff Actions - (TRC - Enter Trace Details)
        public bool TRCSelected
        {
            get
            {
                return (CurrentActionCode == "TRC");
            }
        }
        //jec 05/07/10 - UAT1040 - Extra Bailiff Actions - INS (Insurance Detail)
        public bool INSSelected
        {
            get
            {
                return (CurrentActionCode == "IND");        //UAT866 jec - Insurance Detail is IND (not INS)
            }
        }
        //jec 05/07/10 - UAT1040 - Extra Bailiff Actions - LEG (Legal Details)
        public bool LEGSelected
        {
            get
            {
                return (CurrentActionCode == "LEG");
            }
        }
        //jec 05/07/10 - UAT1040 - Extra Bailiff Actions - FRD (Fraud Details)
        public bool FRDSelected
        {
            get
            {
                return (CurrentActionCode == "FRD");
            }
        }
        //jec - 08/07/10 - UAT1065 - Action Code 'RBDW' (Reverse BDW)
        public bool RBDWSelected
        {
            get
            {
                return (CurrentActionCode == "RBDW");
            }
        }
        //jec - 08/07/10 - UAT1065
        public bool ReversedBDW
        {
            get
            {
                return (reversedBDW);
            }
            set
            {
                reversedBDW = value;
            }
        }

        //IP - 27/09/10 - UAT(36) UAT5.4
        public bool InfoSelected
        {
            get
            {
                return (CurrentActionCode == "INFO");
            }
        }

        //JC - 12/01/09 - CR976 - Special Arrangements
        //Added a property for custID,CustomerName and Acct of the selected account so that this can be passed
        //into the 'Special Arranegements' screen.

        public string Acct
        {
            get
            {
                return m_acct;
            }
            set
            {
                m_acct = value;
            }
        }

        public string CustID
        {
            get
            {
                return m_custid;
            }
            set
            {
                m_custid = value;
            }
        }

        public string CustomerName
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        //IP - 28/04/10 - UAT(983) UAT5.2
        public bool CreditBlocked
        {
            get
            {
                return m_creditblocked;
            }

        }

        #endregion --- End of Properties Variables --------------------------------------------------------

        public BailReview5_2(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
        }

        public BailReview5_2(Form root, Form parent, bool telAction)
        {
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            dynamicMenus = new Hashtable();
            HashMenus();
            ApplyRoleRestrictions();

            if (!btnAddCode.Enabled)
            {
                btnAddCode.Enabled = true;
                AddCodes = true;
            }
            else
            {
                AddCodes = false;
            }

            if (telAction)
            {
                // Telephone Actions - no receipts and no deallocation
                this.Text = GetResource("T_TELACTION");
                btnTempReceipt.Enabled = false;
                btnTempReceipt.Visible = false;
                btnDeAllocate.Enabled = false;
                btnDeAllocate.Visible = false;
            }
            else
            {
                // Bailiff Review - always allow re-allocation
                this.Text = GetResource("T_BAILREVIEW");
                this.gbReAllocate.Visible = true;
            }

            InitialiseStaticData();
            txtAccountNo.Enabled = Credential.HasPermission(CosacsPermissionEnum.BailiffReviewAccountSearch);
            btnReferences.Enabled = Credential.HasPermission(CosacsPermissionEnum.BailiffReviewViewReferences);
            if (!Credential.HasPermission(CosacsPermissionEnum.BailiffReviewAllocateSingleAccount))
                tabAccounts.TabPages.Remove(tabAllocateSingleAccount);

            lAccountNo.Visible = txtAccountNo.Enabled;
            InitScreen();

            //Populate the Strategy drop down
            Collections.CollectionsClasses.StrategyConfigPopulation stratConfig = new Collections.CollectionsClasses.StrategyConfigPopulation();
            DataTable dtStrategies = new DataTable();
            dtStrategies = stratConfig.GetStrategiesForBailiff();

            drpStrategies.DataSource = dtStrategies;
            drpStrategies.DisplayMember = CN.Description;
            drpStrategies.ValueMember = CN.Strategy;
            drpStrategies.Visible = false; // UAT 912 Remove from screen
            //jec 05/07/10 UAT1040 Additional Details
            dtLegalDetail = CreateLegalDetailDT();
            dtInsuranceDetail = CreateInsuranceDetailDT();
            dtFraudDetail = CreateFraudDetailDT();
            dtTRCDetail = CreateTRCDetailDT();

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
            drpStrategies.SelectedValue = String.Empty;
        }

        private void InitialiseStaticData()
        {
            try
            {
                Function = "BStaticDataManager::GetDropDownData";
                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                if (StaticData.Tables[TN.EmployeeTypes] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.EmployeeTypes, new string[] { "ET1", "L" }));

                if (StaticData.Tables[TN.Actions] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Actions, new string[] { "FUP", "L" }));

                if (StaticData.Tables[TN.StrategyActions] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.StrategyActions, null));

                if (StaticData.Tables[TN.BranchNumber] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));

                if (StaticData.Tables[TN.Reasons] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Reasons, new string[] { "SP2", "L" }));

                if (StaticData.Tables[TN.InsuranceTypes] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.InsuranceTypes, null));

                if (dropDowns.DocumentElement.ChildNodes.Count > 0)
                {
                    DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                    }
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                            StaticData.Tables[dt.TableName] = dt;
                    }
                }

                StringCollection empTypes = new StringCollection();
                empTypes.Add("Staff Types");
                StringCollection empRATypes = new StringCollection();
                empRATypes.Add("Staff Types");
                //IP - 02/06/09 - Credit Collection Walkthrough Changes - 'Allocate Single Account' functionality moved from 'FollowUp5_2.cs'.
                StringCollection empTypesAcct = new StringCollection();
                StringCollection branchNos = new StringCollection();
                branchNos.Add("ALL");

                foreach (DataRow row in CollectionsManager.RolesGet((int)CosacsPermissionEnum.Bailiff).Tables[0].Rows)
                {
                   
                        // Only show employee types with 'reference' column set
                        string str = string.Format("{0} : {1}",row[0],row[1]);
                        empTypes.Add(str.ToUpper());
                        empRATypes.Add(str.ToUpper());
                        empTypesAcct.Add(str.ToUpper()); //IP - 02/06/09 - Credit Collection Walkthrough Changes.
                }

                drpEmpType.DataSource = empTypes;

                drpRAEmpType.DataSource = empRATypes;
                drpRAEmpType.SelectedIndex = -1;

                //IP - 01/06/09 - Credit Collection Walkthrough Changes - Moved 'Allocate Single Account' functionality from 'FollowUp5_2.cs'.
                drpEmployeeTypesAcct.DataSource = empTypesAcct;

                dvStrategyActions = ((DataTable)StaticData.Tables[TN.StrategyActions]).DefaultView;
                //populateActions(); //IP - 21/10/08 - UAT5.2 - UAT(529) - Populate the actions once 'Strategy' drop down has been populated.

                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                {
                    branchNos.Add(Convert.ToString(row["branchno"]));
                }

                drpBranch.DataSource = branchNos;
                drpBranch.Text = Config.BranchCode;

                //IP - 01/06/09 - Credit Collection Walkthrough Changes - Moved 'Allocate Single Account' functionality from 'FollowUp5_2.cs'.
                drpBranchAcct.DataSource = branchNos;
                drpBranchAcct.Text = Config.BranchCode;

                drpBranch.Enabled = (Convert.ToInt32(Config.BranchCode) == (decimal)Country[CountryParameterNames.HOBranchNo]);

                this.dtLastActionDate.Value = Date.blankDate;

                drpReason.DataSource = (DataTable)StaticData.Tables[TN.Reasons];
                drpReason.DisplayMember = CN.CodeDescription;
                drpReason.ValueMember = CN.Code;

                if (lblshowSingleAccount.Enabled == false)
                {
                    tabAccounts.TabPages.Remove(tabAllocateSingleAccount);
                }

                staticLoaded = true;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

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

        private void HashMenus()
        {
            dynamicMenus[this.Name + ":txtAccountNo"] = this.txtAccountNo;
            dynamicMenus[this.Name + ":btnDeAllocate"] = this.btnDeAllocate;
            //dynamicMenus[this.Name+":gbReAllocate"] = this.gbReAllocate; //Not used - UAT(5.2) - 754
            //dynamicMenus[this.Name + ":drpStrategies"] = this.drpStrategies; //Not in use. - UAT(5.2) - 789
            dynamicMenus[this.Name + ":btnAddCode"] = this.btnAddCode;
            dynamicMenus[this.Name + ":chkOverrideMaxAlloc"] = this.chkOverrideMaxAlloc;
            dynamicMenus[this.Name + ":showSingleAccount"] = this.lblshowSingleAccount; //IP - 03/06/09 - Credit Collection Walkthrough Changes. Allocate Single Account moved from 'FollowUp5_2.cs'.
            dynamicMenus[this.Name + ":btnReferences"] = this.btnReferences;               //CR1084 UAT28
        }

        private void EnableCriteria(bool enable)
        {
            drpBranch.Enabled = enable;
            drpEmpType.Enabled = enable;
            drpEmpName.Enabled = (enable && drpEmpType.SelectedIndex > 0);
            txtAccountNo.Enabled = enable;
            btnSearch.Enabled = (enable && drpEmpName.SelectedIndex > 0); //IP - 23/07/08 - UAT5.2 - UAT(19)
            //UAT(5.2) - 789 //drpStrategies.Enabled = enable; //IP - 21/10/08 - UAT5.2 - UAT(551) - Disable the 'Strategy' drop down after performing a search.
        }

        private void InitScreen()
        {
            drpBranch.Text = Config.BranchCode;
            drpEmpType.SelectedIndex = 0;
            drpEmpName.Text = "";
            txtAccountNo.Text = "000-0000-0000-0";
            drpRAEmpName.Text = "";

            //IP - 21/10/08 - UAT5.2 - UAT(551)
            drpSendToStrategy.Visible = STSSelected;
            drpSendToStrategy.Enabled = STSSelected;

            drpStrategies.SelectedValue = String.Empty;
            Strategy = String.Empty;

            //Clear all customer/account text boxes
            InitCustomerDetails();

            InitAccountList();
            EnableCriteria(true);

            errorProvider1.SetError(drpEmpName, "");
            errorProvider1.SetError(txtAccountNo, "");
            errorProvider1.SetError(dtDueDate, "");

            ((MainForm)this.FormRoot).statusBar1.Text = "";
        }

        private void InitCustomerDetails()
        {
            txtAgreementTotal.Text = String.Empty;
            txtArrears.Text = String.Empty;
            txtBalance.Text = String.Empty;
            txtDateLastPaid.Text = String.Empty;
            txtDueDate.Text = String.Empty;
            txtFirstName.Text = String.Empty;
            txtHome.Text = String.Empty;
            txtInstalment.Text = String.Empty;
            txtLastActionCode.Text = String.Empty;
            txtLastName.Text = String.Empty;
            txtMobile.Text = String.Empty;
            txtMonthArrears.Text = String.Empty;
            txtPercentage.Text = String.Empty;
            txtStatus.Text = String.Empty;
            txtTitle.Text = String.Empty;
            txtWork.Text = String.Empty;
            txt_provisionamount.Text = string.Empty;
        }

        private void InitLastAction()
        {
            gbLastAction.Enabled = false;
            dtLastActionDate.Value = Date.blankDate;
            txtLastActionCode.Text = "";
        }

        private void InitNewAction()
        {
            gbNewAction.Enabled = true;     //jec 06/07/10 UAT1040
            btnLoadExtraInfo.Visible = false; //jec - 06/07/10 - hiding the button,only visible for SPA,LEG, INS, FRD
            txtActionValue.Text = "0";
            txtNotes.Text = "";
            dtDueDate.Value = DateTime.Today;
            drpActionCode.SelectedIndex = -1;
            btnPrint.Enabled = false;
        }

        private void InitAccountList()
        {
            InitLastAction();
            InitNewAction();
            dgAccounts.DataSource = null;
            dgTransactions.DataSource = null;
            dgProducts.DataSource = null;
            dgStrategies.DataSource = null;
            dgWorklists.DataSource = null;
            dgLetters.DataSource = null;
            dgSMS.DataSource = null;

            dgAccounts.CaptionText = GetResource("T_ALLOCACCOUNTS");
            btnExcel.Enabled = false;
            //gbReAllocate.Enabled = false; //UAT(5.2) - 754
            //drpRAEmpType.SelectedIndex = -1;
            btnDeAllocate.Enabled = false;
            ((MainForm)this.FormRoot).statusBar1.Text = "";
        }

        private bool IsSearchByAcctNo()
        {
            return (txtAccountNo.Text != "000-0000-0000-0" && txtAccountNo.Text.Trim() != "");
        }

        private void RemoveAccountRow(int accountRow)
        {
            // Remove this account from the list
            ((DataView)dgAccounts.DataSource).Delete(accountRow);
            if (((DataView)dgAccounts.DataSource).Count == 0)
            {
                InitAccountList();
            }
            else
            {
                InitLastAction();
                InitNewAction();
                //gbReAllocate.Enabled = false; //UAT(5.2) - 754
            }
            ((MainForm)this.FormRoot).statusBar1.Text = "";
        }

        //private void PrintActionSheet()
        //{
        //    ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(1);

        //    string url = "";
        //    string queryString = "";
        //    string xml = doc.DocumentElement.OuterXml;
        //    StringBuilder sb = new StringBuilder(); //UAT 23 queryString requires branches 

        //    foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
        //    {                
        //       //Create a string that contains the courts branch numbers
        //       if (row[CN.StoreType].ToString() == StoreType.Courts)
        //       {
        //          sb.Append(row[CN.BranchNo].ToString());
        //          sb.Append("|");
        //       }
        //    }

        //    queryString = "empeeNo=" + SelectedEmpeeNo().ToString() + "&" +
        //                  "empName=" + SelectedEmpeeName() + "&" +
        //                  "countryCode=" + Config.CountryCode + "&" +
        //                  "culture=" + Config.Culture + "&" + "courtsBranches=" + sb.ToString();

        //    xml = xml.Replace("&", "%26");
        //    queryString += "&actionSheetXmlStr=" + xml;
        //    url = Config.Url + "WActionSheet.aspx";

        //    object Zero = 0;
        //    object EmptyString = "";
        //    object postData = EncodePostData(queryString);
        //    object headers = PostHeader;
        //    ((MainForm)FormRoot).browsers[0].Navigate(url, ref Zero, ref EmptyString, ref postData, ref headers);
        //}

        private int SelectedEmpeeNo()
        {
            int empeeNo = 0;
            if (drpEmpName.DataSource != null && drpEmpName.SelectedIndex > 0)
            {
                int index = ((string)drpEmpName.SelectedItem).IndexOf(":");
                string empeeNoStr = ((string)drpEmpName.SelectedItem).Substring(0, index - 1);
                empeeNo = Convert.ToInt32(empeeNoStr);
            }
            return empeeNo;
        }

        private string SelectedEmpeeName()
        {
            string empeeName = "";
            if (drpEmpName.DataSource != null && drpEmpName.SelectedIndex > 0)
            {
                string curName = (string)drpEmpName.SelectedItem;
                int index = curName.IndexOf(":");
                int len = curName.Length - 1;
                empeeName = curName.Substring(index + 1, len - index);
            }
            return empeeName;
        }

        private void drpEmpType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string empType;
            string empTypeStr;
            string empTitle;
            DataSet ds = null;
            int branchNo = 0;

            try
            {
                Wait();
                if (staticLoaded && drpEmpType.SelectedIndex != 0)
                {
                    empTypeStr = (string)drpEmpType.SelectedItem;
                    int index = empTypeStr.IndexOf(":");
                    empType = empTypeStr.Substring(0, index - 1);

                    int len = empTypeStr.Length - 1;
                    empTitle = empTypeStr.Substring(index + 1, len - index);

                    StringCollection staff = new StringCollection();
                    staff.Add(empTitle);

                    if (IsSearchByAcctNo() || !IsPositive(drpBranch.Text))
                        branchNo = 0;
                    else
                        branchNo = Convert.ToInt32(drpBranch.Text);

                    ds = Login.GetSalesStaffByType(empType, branchNo, out Error);

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            if (dt.TableName == TN.SalesStaff)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    string str = Convert.ToString(row.ItemArray[0]) + " : " + (string)row.ItemArray[1];
                                    staff.Add(str.ToUpper());
                                }
                            }
                        }
                        drpEmpName.DataSource = staff;
                        drpEmpName.Enabled = true;
                    }
                }
                else if (drpEmpType.SelectedIndex == 0)
                {
                    drpEmpName.DataSource = null;
                    drpEmpName.Enabled = false;
                    drpEmpName.Text = "";
                    btnPrint.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "drpEmpType_SelectedIndexChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void drpEmpName_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                btnPrint.Enabled = (drpEmpName.SelectedIndex > 0);
                //IP - 23/07/08 - UAT5.2 - UAT(19)
                btnSearch.Enabled = (drpEmpName.SelectedIndex > 0);
                if (drpEmpName.SelectedIndex > 0)
                {
                    errorProvider1.SetError(drpEmpName, "");
                    errorProvider1.SetError(txtAccountNo, "");
                    errorProvider1.SetError(dtDueDate, "");
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "drpEmpName_SelectedIndexChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void txtAccountNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                if (txtAccountNo.Text.Trim() == "")
                    txtAccountNo.Text = "000-0000-0000-0";

                if (txtAccountNo.Text != "000-0000-0000-0")
                {
                    errorProvider1.SetError(drpEmpName, "");
                    errorProvider1.SetError(txtAccountNo, "");
                    errorProvider1.SetError(dtDueDate, "");
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "txtAccountNo_TextChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                DataSet ds = null;
                string branchOrAcctFilter = "";
                InitAccountList();

                bool isSearchByAcctNo = this.IsSearchByAcctNo();

                if (isSearchByAcctNo)
                    branchOrAcctFilter = txtAccountNo.UnformattedText;
                else if ((string)drpBranch.SelectedItem == "ALL")
                    branchOrAcctFilter = "%";
                else
                    branchOrAcctFilter = (string)drpBranch.SelectedItem + "%";

                //UAT 18 New error message required for Bailiff screen in 5.2
                if (drpEmpType.SelectedIndex == 0 || drpEmpName.SelectedIndex == 0)
                {
                    errorProvider1.SetError(drpEmpName, GetResource("M_EMPREQUIRED"));
                    //errorProvider1.SetError(txtAccountNo, GetResource("M_EMPORACCTREQUIRED"));
                    return;
                }
                else
                {
                    errorProvider1.SetError(drpEmpName, "");
                    //errorProvider1.SetError(txtAccountNo, "");
                }

                //UAT(5.2) - 811
                string strategy = !drpStrategies.Enabled || drpStrategies.Text == "ALL" ? "%" : drpStrategies.SelectedValue.ToString();

                // UAT 111 - the sproc now only uses the branchOrAcctFilter
                // when the user enters an acct number
                ds = AccountManager.GetStrategyAccountsAllocated(SelectedEmpeeNo(), branchOrAcctFilter, Convert.ToInt16(isSearchByAcctNo), strategy, out Error);

                if (Error.Length > 0)
                {
                    ShowError(Error);
                }
                else
                {
                    dgAccounts.DataSource = ds.Tables[TN.Allocations].DefaultView;

                    #region --- Applying GridTable Style --------------------------------------------------
                    if (dgAccounts.TableStyles.Count == 0)
                    {
                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        tabStyle.MappingName = ds.Tables[TN.Allocations].TableName;
                        dgAccounts.TableStyles.Add(tabStyle);

                        tabStyle.GridColumnStyles[CN.AcctType].Width = 0;
                        tabStyle.GridColumnStyles[CN.TermsType].Width = 0;
                        tabStyle.GridColumnStyles[CN.DateAcctOpen].Width = 0;
                        tabStyle.GridColumnStyles[CN.AgrmtTotal].Width = 0;
                        tabStyle.GridColumnStyles[CN.HiStatus].Width = 0;
                        tabStyle.GridColumnStyles[CN.CurrStatus].Width = 40;
                        tabStyle.GridColumnStyles[CN.CustID].Width = 0;
                        tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 0;
                        tabStyle.GridColumnStyles[CN.EmployeeType].Width = 0;
                        tabStyle.GridColumnStyles[CN.EmployeeName].Width = 0;
                        tabStyle.GridColumnStyles[CN.Description].Width = 0;
                        tabStyle.GridColumnStyles[CN.MobileNo].Width = 0;
                        tabStyle.GridColumnStyles[CN.PercentagePaid].Width = 0;

                        tabStyle.GridColumnStyles[CN.acctno].Width = 90;
                        tabStyle.GridColumnStyles[CN.acctno].HeaderText = GetResource("T_ACCTNO");

                        tabStyle.GridColumnStyles[CN.DateAlloc].Width = 80;
                        tabStyle.GridColumnStyles[CN.DateAlloc].HeaderText = GetResource("T_DATEALLOC");

                        tabStyle.GridColumnStyles[CN.Instalment].Width = 80;
                        tabStyle.GridColumnStyles[CN.Instalment].Alignment = HorizontalAlignment.Right;
                        tabStyle.GridColumnStyles[CN.Instalment].HeaderText = GetResource("T_INSTAL");
                        ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Instalment]).Format = DecimalPlaces;

                        tabStyle.GridColumnStyles[CN.DueDay].Width = 40;
                        tabStyle.GridColumnStyles[CN.DueDay].Alignment = HorizontalAlignment.Right;
                        tabStyle.GridColumnStyles[CN.DueDay].HeaderText = GetResource("T_DATEDUE");

                        tabStyle.GridColumnStyles[CN.OutstBal].Width = 80;
                        tabStyle.GridColumnStyles[CN.OutstBal].Alignment = HorizontalAlignment.Right;
                        tabStyle.GridColumnStyles[CN.OutstBal].HeaderText = GetResource("T_OUTBAL");
                        ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.OutstBal]).Format = DecimalPlaces;

                        tabStyle.GridColumnStyles[CN.Arrears].Width = 80;
                        tabStyle.GridColumnStyles[CN.Arrears].Alignment = HorizontalAlignment.Right;
                        tabStyle.GridColumnStyles[CN.Arrears].HeaderText = GetResource("T_ARREARS");
                        ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Arrears]).Format = DecimalPlaces;

                        tabStyle.GridColumnStyles[CN.MonthsInArrears].Width = 40;
                        tabStyle.GridColumnStyles[CN.MonthsInArrears].Alignment = HorizontalAlignment.Right;
                        tabStyle.GridColumnStyles[CN.MonthsInArrears].HeaderText = GetResource("T_MONTHSARREARS");
                        ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.MonthsInArrears]).Format = "F1";

                        tabStyle.GridColumnStyles[CN.DateLastPaid].Width = 80;
                        tabStyle.GridColumnStyles[CN.DateLastPaid].HeaderText = GetResource("T_DATELASTPAID");

                        tabStyle.GridColumnStyles[CN.Title].Width = 40;
                        tabStyle.GridColumnStyles[CN.Title].HeaderText = GetResource("T_TITLE");

                        tabStyle.GridColumnStyles[CN.name].Width = 90;
                        tabStyle.GridColumnStyles[CN.name].HeaderText = GetResource("T_LASTNAME");

                        tabStyle.GridColumnStyles[CN.FirstName].Width = 90;
                        tabStyle.GridColumnStyles[CN.FirstName].HeaderText = GetResource("T_FIRSTNAME");

                        tabStyle.GridColumnStyles[CN.TelNoHome].Width = 60;
                        tabStyle.GridColumnStyles[CN.TelNoHome].HeaderText = GetResource("T_PHONE");

                        tabStyle.GridColumnStyles[CN.TelNoWork].Width = 60;
                        tabStyle.GridColumnStyles[CN.TelNoWork].HeaderText = GetResource("T_WORKPHONE");

                        tabStyle.GridColumnStyles[CN.TelNoExt].Width = 40;
                        tabStyle.GridColumnStyles[CN.TelNoExt].HeaderText = GetResource("T_EXT");

                        tabStyle.GridColumnStyles[CN.ActionCode].Width = 160;
                        tabStyle.GridColumnStyles[CN.ActionCode].HeaderText = GetResource("T_ACTIONCODE");

                        tabStyle.GridColumnStyles[CN.DateAdded].Width = 80;
                        tabStyle.GridColumnStyles[CN.DateAdded].HeaderText = GetResource("T_DATEADDED");

                        tabStyle.GridColumnStyles[CN.BankOrder].Width = 80;
                        tabStyle.GridColumnStyles[CN.BankOrder].HeaderText = GetResource("T_BANKORDER");
                    }
                    #endregion --- End of Applying GridTable Style ----------------------------------------

                    ((MainForm)this.FormRoot).statusBar1.Text = ds.Tables[0].Rows.Count + " Accounts Loaded";

                    if (dgAccounts.DataSource != null)
                    {
                        if (((DataView)dgAccounts.DataSource).Count > 0)
                        {
                            string employee = drpEmpName.Text;
                            DataView dv = (DataView)dgAccounts.DataSource;

                            //Populate the new Account details tab
                            PopulateFields(0, dv);

                            if (isSearchByAcctNo)
                            {
                                string branchNo = txtAccountNo.UnformattedText.Substring(0, 3);
                                int index = drpBranch.FindString(branchNo);
                                drpBranch.SelectedIndex = (index > 0 ? index : 0);

                                //IP - 03/06/09 - Credit Collection Walkthrough Changes - trim the EmployeeType
                                string staffType = ((string)dv[0][CN.EmployeeType]).Trim().ToUpper() + " : " + ((string)dv[0][CN.Description]).ToUpper();
                                //string staffType = ((string)dv[0][CN.EmployeeType]).ToUpper() + " : "+((string)dv[0][CN.Description]).ToUpper();
                                index = drpEmpType.FindString(staffType);
                                drpEmpType.SelectedIndex = (index > 0 ? index : 0);

                                employee = Convert.ToString(dv[0][CN.EmployeeNo]) + " : " + ((string)dv[0][CN.EmployeeName]).ToUpper();
                                index = drpEmpName.FindString(employee);
                                if (drpEmpName.Items.Count > 0)
                                drpEmpName.SelectedIndex = (index > 0 ? index : 0);
                            }

                            dgAccounts.CaptionText += " - " + employee;
                            btnExcel.Enabled = true;
                            EnableCriteria(false);
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

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            DateTime spaDateExpiry = DateTime.Now;
            string spaReasonCode = string.Empty;
            double spaInstal = 0;


            try
            {
                Wait();
                ValidateDueDate();
                ValidateNotes();      //IP - 27/09/10 - UAT(37) UAT5.4 

                //If no errors .. do the update..
                if (errorProvider1.GetError(dtDueDate).Length == 0 && errorProvider1.GetError(txtActionValue).Length == 0 &&
                        errorProvider1.GetError(txtNotes).Length == 0)//IP - 27/09/10 - UAT(37) UAT5.4 Check txtNotes for errors.
                {
                    //int accountRow = m_accountRow;
                    //int accountRow = dgAccounts.CurrentRowIndex;

                    m_accountRow = dgAccounts.CurrentRowIndex;
                    int accountRow = m_accountRow;

                    if (accountRow >= 0 && drpActionCode.SelectedIndex != -1)
                    {
                        //acctNo = (string)dgAccounts[accountRow, 0];

                        double actionValue = Convert.ToDouble(StripCurrency(txtActionValue.Text));
                        DateTime dueDate = dtDueDate.Value;
                        //jec 06/07/10 UAT1040 
                        string notes = txtNotes.Text;
                        DateTime reminderDateTime = new DateTime(1900, 1, 1);
                        bool cancelOutstandingReminders = false;
                        DataSet dsExtraActionDetail = new DataSet();

                        if (SPASelected)
                        {
                            spaDateExpiry = dtDueDate.Value;
                            spaReasonCode = drpReason.SelectedValue.ToString();
                            spaInstal = Convert.ToDouble(StripCurrency(txtActionValue.Text));
                        }

                        //IP - 21/10/08 - UAT5.2 - UAT(551)
                        //If the 'Action Code', 'STS - Send To Strategy' has been selected then
                        if (STSSelected)
                        {
                            // Update the 'CMStrategyAcct' table and the 'CMWorklistsacct' table for the new strategy.
                            CollectionsManager.UpdateStrategyAccounts(drpSendToStrategy.SelectedValue.ToString(), AcctNo, string.Empty, string.Empty, Credential.UserId, out Error);
                            if (Error.Length > 0)
                            {
                                ShowError(Error);
                            }
                        }

                        else if (LEGSelected || INSSelected || FRDSelected || TRCSelected)
                        {
                            if (LEGSelected)
                            {
                                dtLegalDetail.Rows[0][CN.CMAcctno] = Acct;
                                dtLegalDetail.Rows[0][CN.CMEmpeeno] = Credential.UserId;
                                dsExtraActionDetail.Tables.Add(dtLegalDetail.Copy());
                            }
                            else if (INSSelected)
                            {
                                dtInsuranceDetail.Rows[0][CN.CMAcctno] = Acct;
                                dtInsuranceDetail.Rows[0][CN.CMEmpeeno] = Credential.UserId;
                                dsExtraActionDetail.Tables.Add(dtInsuranceDetail.Copy());
                            }
                            else if (FRDSelected)
                            {
                                dtFraudDetail.Rows[0][CN.CMAcctno] = Acct;
                                dtFraudDetail.Rows[0][CN.CMEmpeeno] = Credential.UserId;
                                dsExtraActionDetail.Tables.Add(dtFraudDetail.Copy());
                            }
                            else if (TRCSelected)
                            {
                                dtTRCDetail.Rows[0][CN.CMAcctno] = Acct;
                                dtTRCDetail.Rows[0][CN.CMEmpeeno] = Credential.UserId;
                                dsExtraActionDetail.Tables.Add(dtTRCDetail.Copy());
                                dueDate = Convert.ToDateTime(dtTRCDetail.Rows[0][CN.CMTRCInitiatedDate]);
                            }
                            //if (strategyChanged == true && drpStrategies.SelectedValue != null)
                            //{
                            //   CollectionsManager.UpdateStrategyAccounts(drpStrategies.SelectedValue.ToString(), acctNo, out Error);
                            //   if (Error.Length > 0)
                            //   {
                            //      ShowError(Error);
                            //   }
                            //}

                        }

                        if (LEGSelected || INSSelected || FRDSelected || TRCSelected)
                        {
                            CollectionsManager.SaveBailActionsWithTelephoneActions(Acct, Credential.UserId, CurrentActionCode, notes,
                                dueDate, actionValue, spaDateExpiry, spaReasonCode, spaInstal, reminderDateTime, cancelOutstandingReminders, dsExtraActionDetail, "TELACTION", out Error);
                        }
                        else
                        {

                            AccountManager.SaveBailActions(AcctNo, SelectedEmpeeNo(), CurrentActionCode, txtNotes.Text.Trim(),
                                                            dueDate, actionValue, spaDateExpiry, spaReasonCode, spaInstal,
                                                            new DateTime(1900, 1, 1), false, "", out Error);
                        }

                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                        }
                        else
                        {
                            //jec - 17/09/09 - CR976/UAT(870) - The below code has now been moved into the below method. (as in TelephoneAction)
                            CycleToNextAccount();

                            //jec 05/07/10 UAT1040 Additional Details
                            dtLegalDetail = CreateLegalDetailDT();
                            dtInsuranceDetail = CreateInsuranceDetailDT();
                            dtFraudDetail = CreateFraudDetailDT();
                            dtTRCDetail = CreateTRCDetailDT();

                            //if (chCycleNext.Checked == true)
                            //{
                            //    int rowNo = 0;
                            //    //Unlock previous account 
                            //    UnlockAccount();

                            //    PopulateFields(accountRow + 1, (DataView)dgAccounts.DataSource);

                            //    //Is it the last row in the datagrid
                            //    if (((DataView)dgAccounts.DataSource).Count <= accountRow + 1)
                            //    {
                            //       rowNo = accountRow;
                            //       if (((DataView)dgAccounts.DataSource).Count <= 1)
                            //       {
                            //          btnSave.Enabled = false;
                            //          InitCustomerDetails();
                            //       }
                            //       else
                            //       {
                            //          PopulateFields(accountRow - 1, (DataView)dgAccounts.DataSource);
                            //       }
                            //    }
                            //    else
                            //    {
                            //       rowNo = accountRow + 1;
                            //    }

                            //    if (((DataView)dgAccounts.DataSource).Count > rowNo)
                            //    {
                            //       dgAccounts.UnSelect(accountRow);
                            //       dgAccounts.Select(rowNo);
                            //       DataGridCell cell = new DataGridCell(rowNo, 0);
                            //       try
                            //       {
                            //          dgAccounts.CurrentCell = cell;
                            //       }
                            //       catch
                            //       {
                            //          //Doesn't always like this
                            //       }
                            //       //dataviewNo++;
                            //       //InitNewAction();

                            //    }

                            //    DataRowView myRow;
                            //    DataView dv = (DataView)dgAccounts.DataSource;

                            //    myRow = dv[accountRow];
                            //    myRow.Delete();  
                            //}

                            //lbReason.Visible = false;
                            //drpReason.Visible = false;
                            //lbActionDate.Text = "Due Date";
                            //lbActionValue.Text = "Action Value";

                            ////Allow further actions to be made
                            //txtActionValue.Text = "0";
                            //txtNotes.Text = "";
                            //dtDueDate.Value = DateTime.Today;
                            //drpActionCode.SelectedIndex = 0;
                            //btnPrint.Enabled = false;
                            //StrategyChanged = false;
                            //drpSendToStrategy.Visible = false; //IP - 20/10/08 - UAT5.2 - UAT(551)
                        }
                    }
                    else
                    {
                        ShowInfo("M_SELECTACTIONCODE");
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSave_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                CloseTab();
            }
            catch (Exception ex)
            {
                Catch(ex, "menuExit_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                InitScreen();
                UnlockAccount();
            }
            catch (Exception ex)
            {
                Catch(ex, "btnClear_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnPrint_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                DataSet ds = AccountManager.SetAllocDate(SelectedEmpeeNo(), out Error);

                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    DataView dv = new DataView(ds.Tables[TN.Allocations]);
                    int count = dv.Count;

                    ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(count);
                    //AxSHDocVw.AxWebBrowser[] browsers = CreateBrowserArray(count); 
                    string empeeName = SelectedEmpeeName();
                    int empeeno = SelectedEmpeeNo();


                    foreach (DataTable dt in ds.Tables)
                    {
                        if (dt.TableName == TN.Allocations)
                        {
                            /*doc = new XmlDocument();
                            doc.LoadXml("<ACTIONSHEET/>");*/
                            //                            DataView dv = dt.DefaultView; 
                            // Common form Print Action Sheet now. 
                            CheckStyleSheet("ActionSheet.xslt");
                            PrintActionSheet(dv, false, empeeName, empeeno);
                            /*    
							foreach(DataRow row in dt.Rows)
							{
								XmlNode acctNo = doc.CreateElement("ACCTNO");
								acctNo.InnerText = row[CN.AcctNo].ToString();
								doc.DocumentElement.AppendChild(acctNo);
							}*/
                        }
                    }

                    /*if(doc.DocumentElement.ChildNodes.Count > 0)
                        PrintActionSheet();*/

                    btnSearch_Click(this, null);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnPrint_Click");
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

                if (dgAccounts.CurrentRowIndex >= 0)
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

                        string line =
                            CN.AccountNumber + comma +
                            CN.CurrStatus + comma +
                            CN.DateAlloc + comma +
                            CN.Instalment + comma +
                            CN.DueDay + comma +
                            CN.OutstBal + comma +
                            CN.Arrears + comma +
                            CN.MonthsInArrears + comma +
                            CN.DateLastPaid + comma +
                            CN.Title + comma +
                            CN.name + comma +
                            CN.FirstName + comma +
                            CN.TelNoHome + comma +
                            CN.TelNoWork + comma +
                            CN.TelNoExt + comma +
                            CN.ActionCode + comma +
                            CN.DateAdded + comma +
                            CN.BankOrder + Environment.NewLine + Environment.NewLine;

                        byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
                        fs.Write(blob, 0, blob.Length);

                        foreach (DataRowView row in (DataView)dgAccounts.DataSource)
                        {
                            line =
                                row[CN.AccountNumber].ToString().Replace(",", "") + comma +
                                row[CN.CurrStatus].ToString().Replace(",", "") + comma +
                                Convert.ToString(row[CN.DateAlloc]) + comma +
                                ((decimal)row[CN.Instalment]).ToString(DecimalPlaces).Replace(",", "") + comma +
                                row[CN.DueDay].ToString().Replace(",", "") + comma +
                                ((decimal)row[CN.OutstBal]).ToString(DecimalPlaces).Replace(",", "") + comma +
                                ((decimal)row[CN.Arrears]).ToString(DecimalPlaces).Replace(",", "") + comma +
                                ((decimal)row[CN.MonthsInArrears]).ToString().Replace(",", "") + comma +
                                Convert.ToString(row[CN.DateLastPaid]) + comma +
                                row[CN.Title].ToString().Replace(",", "") + comma +
                                row[CN.name].ToString().Replace(",", "") + comma +
                                row[CN.FirstName].ToString().Replace(",", "") + comma +
                                row[CN.TelNoHome].ToString().Replace(",", "") + comma +
                                row[CN.TelNoWork].ToString().Replace(",", "") + comma +
                                row[CN.TelNoExt].ToString().Replace(",", "") + comma +
                                row[CN.ActionCode].ToString().Replace(",", "") + comma +
                                Convert.ToString(row[CN.DateAdded]) + comma +
                                row[CN.BankOrder].ToString().Replace(",", "") + Environment.NewLine;

                            blob = System.Text.Encoding.UTF8.GetBytes(line);
                            fs.Write(blob, 0, blob.Length);
                        }
                        fs.Close();

                        /* attempt to launch Excel. May get a COMException if Excel is not installed */
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

        private void dgAccounts_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Wait();
                int accountRow = dgAccounts.CurrentRowIndex;
                if (accountRow >= 0)
                {
                    gbLastAction.Enabled = true;
                    gbNewAction.Enabled = true;
                    drpActionCode.SelectedIndex = drpActionCode.SelectedIndex == -1 ? -1 : 0;
                    //drpActionCode.SelectedIndex = 0;
                    //gbReAllocate.Enabled = true; //UAT(5.2) - 754
                    drpRAEmpType.SelectedIndex = 0;
                    btnDeAllocate.Enabled = true;
                    btnSave.Enabled = true;
                    DataView dv = (DataView)dgAccounts.DataSource;

                    UnlockAccount();
                    //Populate the new Account details tab
                    PopulateFields(accountRow, dv);

                    int count = dv.Count;
                    this.dtLastActionDate.Value = Convert.ToDateTime(dv[accountRow][CN.DateAdded]);
                    this.txtLastActionCode.Text = (string)dv[accountRow][CN.ActionCode];
                    if (dgAccounts.CurrentRowIndex >= 0)
                    {
                        for (int i = count - 1; i >= 0; i--)
                        {
                            // Only interested in selected rows
                            if (dgAccounts.IsSelected(i))
                            {
                                string al = (string)dv[i][CN.CurrStatus];
                                if (al == "5")
                                {
                                    btnDeAllocate.Enabled = false;
                                }
                            }
                        }
                    }

                    if (e.Button == MouseButtons.Right)
                    {
                        DataGrid ctl = (DataGrid)sender;

                        MenuCommand m1 = new MenuCommand(GetResource("P_ACCOUNT_DETAILS"));
                        MenuCommand m2 = new MenuCommand(GetResource("P_FOLLOWUP"));
                        MenuCommand m3 = new MenuCommand(GetResource("P_ALLOCATION_HISTORY"));

                        m1.Click += new System.EventHandler(this.OnAccountDetails);
                        m2.Click += new System.EventHandler(this.OnFollowUpDetails);
                        m3.Click += new System.EventHandler(this.OnAllocationHistory);

                        PopupMenu popup = new PopupMenu();
                        popup.MenuCommands.AddRange(new MenuCommand[] { m1, m2, m3 });
                        MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                    }
                }
                else
                {
                    InitLastAction();
                    InitNewAction();
                    gbLastAction.Enabled = false;
                    gbNewAction.Enabled = false;
                    gbReAllocate.Enabled = false;
                    btnDeAllocate.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "dgAccounts_MouseUp");
            }
            finally
            {
                StopWait();
            }
        }

        private void OnAccountDetails(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                int accountRow = dgAccounts.CurrentRowIndex;

                if (accountRow >= 0)
                {
                    //string acctNo = (string)dgAccounts[accountRow, 0];
                    AccountDetails details = new AccountDetails(AcctNo, FormRoot, this);
                    ((MainForm)this.FormRoot).AddTabPage(details, 7);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "OnAccountDetails");
            }
            finally
            {
                StopWait();
            }
        }

        private void OnAllocationHistory(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                int accountRow = dgAccounts.CurrentRowIndex;

                if (accountRow >= 0)
                {
                    string acctNo = (string)dgAccounts[accountRow, 0];
                    AccountDetails details = new AccountDetails(acctNo, FormRoot, this, TPN.AllocationHistory);
                    ((MainForm)this.FormRoot).AddTabPage(details, 7);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "OnAllocationHistory");
            }
            finally
            {
                StopWait();
            }
        }

        private void OnFollowUpDetails(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                int accountRow = dgAccounts.CurrentRowIndex;

                if (accountRow >= 0)
                {
                    string acctNo = (string)dgAccounts[accountRow, 0];
                    BailActions actions = new BailActions(acctNo, FormRoot, this);
                    actions.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "OnFollowUpDetails");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnLoadExtraInfo_Click(object sender, EventArgs e)
        {
            try
            {
                Wait();
                if (SPASelected)
                {
                    int accountRow = dgAccounts.CurrentRowIndex;

                    if (accountRow >= 0)
                    {
                        //string acctNo = (string)dgAccounts[accountRow, 0];

                        SPADetails spa = new SPADetails(AcctNo);
                        spa.FormRoot = this.FormRoot;
                        spa.FormParent = this;
                        spa.ShowDialog(this);
                    }
                }
                //jec 06/07/10 UAT1040 
                else if (LEGSelected)
                {
                    TelAction_LEG dialog = new TelAction_LEG(dtLegalDetail);
                    dialog.FormRoot = this.FormRoot;
                    dialog.FormParent = this;
                    dialog.ShowDialog(this);

                    if (dialog.ButtonClicked == DialogResult.OK)
                    {
                        //NM & IP - 08/01/09 - CR976 
                        //Set the 'Notes' field on the 'Telephone Actions' screen
                        //to the 'User Notes' entered on the 'Legal Details' screen.
                        if (dtLegalDetail.Rows.Count > 0)
                        {
                            SetNotes(dtLegalDetail);
                        }
                        else
                        {
                            txtNotes.Text = string.Empty;
                        }

                        btnSave_Click(null, null);
                    }
                }
                else if (INSSelected)
                {
                    TelAction_INS dialog = new TelAction_INS(dtInsuranceDetail);
                    dialog.FormRoot = this.FormRoot;
                    dialog.FormParent = this;
                    dialog.ShowDialog(this);

                    //NM & IP - 08/01/09 - CR976 
                    //Set the 'Notes' field on the 'Telephone Actions' screen
                    //to the 'User Notes' entered on the 'Legal Details' screen.
                    if (dialog.ButtonClicked == DialogResult.OK)
                    {
                        if (dtInsuranceDetail.Rows.Count > 0)
                        {
                            SetNotes(dtInsuranceDetail);
                        }
                        else
                        {
                            txtNotes.Text = string.Empty;
                        }

                        btnSave_Click(null, null);
                    }
                }
                else if (FRDSelected)
                {
                    TelAction_FRD dialog = new TelAction_FRD(dtFraudDetail);
                    dialog.FormRoot = this.FormRoot;
                    dialog.FormParent = this;
                    dialog.ShowDialog(this);

                    //NM & IP - 08/01/09 - CR976 
                    //Set the 'Notes' field on the 'Telephone Actions' screen
                    //to the 'User Notes' entered on the 'Legal Details' screen.
                    if (dialog.ButtonClicked == DialogResult.OK)
                    {
                        if (dtFraudDetail.Rows.Count > 0)
                        {
                            SetNotes(dtFraudDetail);
                        }
                        else
                        {
                            txtNotes.Text = string.Empty;
                        }

                        btnSave_Click(null, null);
                    }
                }
                else if (TRCSelected)
                {
                    TelAction_TRC dialog = new TelAction_TRC(dtTRCDetail);
                    dialog.FormRoot = this.FormRoot;
                    dialog.FormParent = this;
                    dialog.ShowDialog(this);
                    // if LEG save button - save Tel action  details - CredCol walkthrough changes jec 01/06/09
                    if (DialogResult.OK == dialog.ButtonClicked)
                    {
                        //NM & IP - 08/01/09 - CR976 
                        //Set the 'Notes' field on the 'Telephone Actions' screen
                        //to the 'User Notes' entered on the 'Legal Details' screen.
                        if (dtTRCDetail.Rows.Count > 0)
                            SetNotes(dtTRCDetail);
                        else
                            txtNotes.Text = string.Empty;

                        btnSave_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSPADetails_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnTempReceipt_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                TemporaryReceiptsDetails tempReceipt = null;

                if (drpEmpName.SelectedIndex != 0)
                    tempReceipt = new TemporaryReceiptsDetails(SelectedEmpeeNo(), FormRoot, this);
                else
                    tempReceipt = new TemporaryReceiptsDetails(FormRoot, this);

                ((MainForm)this.FormRoot).AddTabPage(tempReceipt);
            }
            catch (Exception ex)
            {
                Catch(ex, "btnTempReceipt_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnDeAllocate_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                ArrayList al = new ArrayList();
                CurrencyManager cm = (CurrencyManager)this.BindingContext[dgAccounts.DataSource, dgAccounts.DataMember];
                DataView dv = (DataView)cm.List;

                for (int i = 0; i < dv.Count; ++i)
                {
                    // Only interested in selected rows
                    if (dgAccounts.IsSelected(i))
                        al.Add((string)dgAccounts[i, 0]);
                }

                // De-allocate accounts
                foreach (String acct in al)
                {
                    AccountManager.DeAllocateAccount(acct, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                }

                // Delete rows for de-allocated accounts
                for (int i = dv.Count - 1; i >= 0; i--)
                {
                    // Only interested in de-allocated rows
                    if (al.Contains((string)dgAccounts[i, 0]))
                    {
                        RemoveAccountRow(i);
                        UnlockAccount();
                        if (i == dv.Count)
                        {
                            PopulateFields(i - 1, dv);
                        }
                        else
                        {
                            PopulateFields(i, dv);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnDeAllocate_Click");
            }
            finally
            {
                // The user has to click on another account row to De-Allocate again
                btnDeAllocate.Enabled = false;
                StopWait();
            }
        }

        //IP - 08/10/09 - UAT(909)
        private void btnReAllocate_Click(object sender, System.EventArgs e)
        {
            int countAcctsToRealloc = 0;
            bool canReallocate = false;
            int noCanAlloc = 0;

            try
            {
                Wait();

                if (drpRAEmpName.SelectedIndex != 0)
                {
                    int index = ((string)drpRAEmpName.SelectedItem).IndexOf(":");
                    int index2 = ((string)drpEmpName.SelectedItem).IndexOf(":");
                    string newEmpeeNo = ((string)drpRAEmpName.SelectedItem).Substring(0, index - 1);
                    string empeeno = ((string)drpEmpName.SelectedItem).Substring(0, index2 - 1); //This is the employee the account is currently allocated to

                    ArrayList al = new ArrayList();
                    CurrencyManager cm = (CurrencyManager)this.BindingContext[dgAccounts.DataSource, dgAccounts.DataMember];
                    DataView dv = (DataView)cm.List;

                    for (int i = 0; i < dv.Count; ++i)
                    {
                        // Only interested in selected rows
                        if (dgAccounts.IsSelected(i))
                            al.Add((string)dgAccounts[i, 0]);
                    }

                    // de-allocate and allocate accounts (Re-allocate)
                    //ArrayList alFailedAccts = new ArrayList();
                    //ArrayList alInitialAccts = new ArrayList(al);

                    //If 'Override Maximum Allocation checkbox is checked' or you are re-allocating to the same employee then proceed to re-allocate
                    if (chkOverrideMaxAlloc.Checked || Convert.ToInt32(empeeno) == Convert.ToInt32(newEmpeeNo))
                    {
                        canReallocate = true;
                    }
                    //else you need to check if the accounts can be re-allocated.
                    else
                    {
                        //Get the number of accounts selected to be re-allocated
                        countAcctsToRealloc = al.Count;

                        canReallocate = AccountManager.CheckCanReallocate(countAcctsToRealloc, Convert.ToInt32(newEmpeeNo), ref noCanAlloc, out Error);

                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                        }
                    }


                    if (canReallocate)
                    {
                        foreach (String acct in al)
                        {
                            //bool rtnValue = AccountManager.AllocateDeallocateAccount(acct, Convert.ToInt32(newEmpeeNo), !chkOverrideMaxAlloc.Checked, out Error);
                            AccountManager.AllocateDeallocateAccount(acct, Convert.ToInt32(newEmpeeNo), out Error);

                            if (Error.Length > 0)
                            {
                                al.Remove(acct.ToString());
                                ShowError(Error);
                            }
                            //else if(rtnValue == false)
                            //{
                            //    alFailedAccts.Add(acct.ToString());
                            //}
                        }

                        // Delete rows for de-allocated accounts
                        for (int i = dv.Count - 1; i >= 0; i--)
                        {
                            // Only interested in de-allocated rows
                            // if (alInitialAccts.Contains((string)dgAccounts[i, 0]) && !alFailedAccts.Contains((string)dgAccounts[i, 0]))
                            if (al.Contains((string)dgAccounts[i, 0]))
                            {
                                RemoveAccountRow(i);
                                UnlockAccount();
                                if (i == dv.Count)
                                {
                                    PopulateFields(i - 1, dv);
                                }
                                else
                                {
                                    PopulateFields(i, dv);
                                }
                            }
                        }
                        //jec - 24/11/09 - UAT901 - cycle to next account if allocated to different empeeno
                        if (newEmpeeNo != empeeno)
                        {
                            chCycleNext.Checked = true;
                        }
                        CycleToNextAccount();
                    }
                    else
                    {
                        ShowInfo("M_REALLOCATIONFAILED", new object[] { noCanAlloc });
                    }

                    //if (alFailedAccts.Count > 0)
                    //{
                    //   string msg = "Re-allocation was not succesful for the following accounts due to MaxAccounts exceeded \r\n";
                    //   foreach (string acct in alFailedAccts)
                    //       msg += " " + acct + "\r\n";

                    //   MessageBox.Show(msg);
                    //}

                    chkOverrideMaxAlloc.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnReAllocate_Click");
            }
            finally
            {
                // The user has to click on another account row to enable Re-Allocate again
                gbReAllocate.Enabled = false;
                StopWait();
            }
        }

        private void drpRAEmpType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                if (staticLoaded && drpRAEmpType.SelectedIndex != 0)
                {
                    string empType;
                    string empTypeStr;
                    string empTitle;
                    DataSet ds = null;

                    empTypeStr = (string)drpRAEmpType.SelectedItem;
                    int index = empTypeStr.IndexOf(":");
                    empType = empTypeStr.Substring(0, index - 1);

                    int len = empTypeStr.Length - 1;
                    empTitle = empTypeStr.Substring(index + 1, len - index);

                    StringCollection staff = new StringCollection();
                    staff.Add(empTitle);

                    ds = Login.GetSalesStaffByType(empType, 0, out Error);

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            if (dt.TableName == TN.SalesStaff)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    //string str = Convert.ToString(row.ItemArray[0]) + " : "+(string)row.ItemArray[1];
                                    string str = Convert.ToString(row.ItemArray[0]) + " : " + (string)row.ItemArray[1] + " " + "(" + Convert.ToString(row.ItemArray[3]) + ")"; //IP - 09/10/09 - UAT(909) Display the number of accounts that can be allocated to the employee.
                                    staff.Add(str.ToUpper());
                                }
                            }
                        }

                        // Remove the current employee from the list so
                        // that we cannot re-allocate to the same employee
                        //staff.Remove(drpEmpName.Text); - UAT(5.2) - 754
                        drpRAEmpName.DataSource = staff;
                        drpRAEmpName.Enabled = true;
                    }
                }
                else if (drpRAEmpType.SelectedIndex == 0)
                {
                    drpRAEmpName.DataSource = null;
                    drpRAEmpName.Enabled = false;
                    drpRAEmpName.Text = "";
                    btnReAllocate.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "drpRAEmpType_SelectedIndexChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void drpRAEmpName_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                this.btnReAllocate.Enabled = (drpRAEmpName.SelectedIndex > 0);
            }
            catch (Exception ex)
            {
                Catch(ex, "drpRAEmpName_SelectedIndexChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void drpActionCode_SelectionChangeCommitted(object sender, System.EventArgs e)
        {

            //IP - 21/10/08 - UAT5.2 - UAT(551)
            DataTable dtStrategiesToSendTo = new DataTable();
            string strategy = string.Empty;
            DateTime spaDateExpiry = DateTime.Now;
            string spaReasonCode = string.Empty;
            double spaInstal = 0;
            string notes = txtNotes.Text;
            double actionValue = Convert.ToDouble(StripCurrency(txtActionValue.Text));
            DateTime dueDate = dtDueDate.Value;
            DateTime reminderDateTime = new DateTime(1900, 1, 1);
            bool cancelOutstandingReminders = false;
            btnLoadExtraInfo.Visible = false; //jec - 06/07/10 - hiding the button,only visible for SPA,LEG, INS, FRD

            Collections.CollectionsClasses.StrategyConfigPopulation stratConfig = new Collections.CollectionsClasses.StrategyConfigPopulation();

            try
            {
                Wait();
                if (drpActionCode.SelectedIndex >= 0)
                {
                    gbReAllocate.Enabled = false;
                    btnSave.Enabled = true;

                    if (SPASelected)
                    {
                        btnSave.Enabled = false;        //UAT847 jec 11/09/09
                        btnLoadExtraInfo.Text = "SPA History";
                        btnLoadExtraInfo.Visible = true;
                        lbActionDate.Text = "Expiry Date";
                        lbActionValue.Text = "Instalment";

                        //IP - 29/09/08 - If 'SPA' has been selected then display the 'Special Arrangements' screen.
                        //IP & JC - 12/01/09 - CR976 - Special Arrangements - Changed to pass in the CustomerID of the selected account.
                        //IP - 28/04/10 - UAT(983) UAT5.2 - Added CreditBlocked
                        SpecialArrangements sparr = new SpecialArrangements(Acct, CustID, CustomerName, CreditBlocked, (DataTable)StaticData.Tables[TN.Reasons]);
                        sparr.FormRoot = this.FormRoot;
                        sparr.FormParent = this;
                        sparr.ShowDialog(this);

                        dtSPADetails = sparr.dtSPADetails;
                        // has Accept button been pressed
                        if (sparr.AcceptBtn == true)
                        {
                            spaDateExpiry = Convert.ToDateTime(dtSPADetails.Rows[0][CN.FinalPayDate]);
                            spaReasonCode = Convert.ToString(dtSPADetails.Rows[0][CN.ReasonCode]);
                            spaInstal = Convert.ToDouble(StripCurrency(txtActionValue.Text));
                            dueDate = Convert.ToDateTime(dtSPADetails.Rows[0][CN.ReviewDate]);

                            if (Convert.ToString(dtSPADetails.Rows[0][CN.ExtendTerm]) == "True")
                            {
                                notes = "(Refinance) - " + notes;
                                CollectionsManager.SPAWriteRefinance(Acct, Credential.UserId, CurrentActionCode, notes,
                                dueDate, actionValue, spaDateExpiry, spaReasonCode, spaInstal, reminderDateTime, cancelOutstandingReminders, dtSPADetails, "TELACTION", out Error);

                                // TO DO
                                string NewAccount = "";
                                double NewAcctNo = 0;
                                NewAccount = Convert.ToString(Acct);
                                NewAcctNo = Convert.ToDouble(NewAccount);
                                NewAcctNo += 1;
                                NewAccount = Convert.ToString(NewAcctNo);

                                NewAccount refinance = new NewAccount(NewAccount, 1, null, false, FormRoot, this);
                                refinance.FormRoot = this.FormRoot;
                                refinance.FormParent = this;
                                refinance.SPARefinancePrint(Convert.ToString(dtSPADetails.Rows[0][CN.TermsType]));
                            }
                            else
                            {
                                CollectionsManager.SaveBailActionsForSPA(Acct, Credential.UserId, CurrentActionCode, notes,
                                    dueDate, actionValue, spaDateExpiry, spaReasonCode, spaInstal, reminderDateTime, cancelOutstandingReminders, dtSPADetails, "TELACTION", out Error);
                            }

                            //IP - 10/09/09 - UAT(848) - Cycle to next account.
                            CycleToNextAccount();
                        }
                        //extendedTermsSelected = sparr.ExtendTermSelected;

                    }
                    else if (LEGSelected || INSSelected || FRDSelected || TRCSelected)
                    {
                        //jec - 06/07/10  UAT1040  - retrieve any previous details
                        DataSet ds = CollectionsManager.GetLegalFraudInsuranceDetails(Acct, out Error);

                        foreach (DataTable dt in ds.Tables)
                        {
                            if (dt.TableName == TN.LegalDetails)
                            {
                                dtLegalDetail = ds.Tables[0];
                            }
                            if (dt.TableName == TN.FraudDetails)
                            {
                                dtFraudDetail = ds.Tables[1];
                            }
                            if (dt.TableName == TN.InsuranceDetails)
                            {
                                dtInsuranceDetail = ds.Tables[2];
                            }
                            if (dt.TableName == TN.TraceDetails)
                            {
                                dtTRCDetail = ds.Tables[3];
                            }
                        }
                        // }
                        //else 
                        if (TRCSelected)
                        {
                            btnLoadExtraInfo.Text = "Trace Details";
                            btnLoadExtraInfo.Visible = true;

                            try
                            {
                                Wait();
                                TelAction_TRC dialog = new TelAction_TRC(dtTRCDetail);
                                dialog.FormRoot = this.FormRoot;
                                dialog.FormParent = this;
                                dialog.ShowDialog(this);
                                // if LEG save button - save Tel action  details - CredCol walkthrough changes jec 01/06/09
                                if (DialogResult.OK == dialog.ButtonClicked)
                                {
                                    //NM & IP - 08/01/09 - CR976 
                                    //Set the 'Notes' field on the 'Telephone Actions' screen
                                    //to the 'User Notes' entered on the 'Legal Details' screen.
                                    if (dtTRCDetail.Rows.Count > 0)
                                        SetNotes(dtTRCDetail);
                                    else
                                        txtNotes.Text = string.Empty;

                                    btnSave_Click(null, null);
                                }
                            }
                            catch (Exception ex)
                            {
                                Catch(ex, "drpActionCode_SelectionChangeCommitted (Loading TRC screen)");
                            }
                            finally
                            {
                                StopWait();
                            }
                            //txtActionValue.Visible = false;
                            //lbActionValue.Visible = false;
                            //dtDueDate.Visible = false;
                            //lbActionDate.Visible = false;
                        }
                        else if (LEGSelected)
                        {
                            btnLoadExtraInfo.Text = "Legal Details";
                            btnLoadExtraInfo.Visible = true;

                            try
                            {
                                Wait();
                                TelAction_LEG dialog = new TelAction_LEG(dtLegalDetail);
                                dialog.FormRoot = this.FormRoot;
                                dialog.FormParent = this;
                                dialog.ShowDialog(this);
                                // if LEG save button - save Tel action  details - CredCol walkthrough changes jec 01/06/09
                                if (dialog.ButtonClicked == DialogResult.OK)
                                {
                                    //NM & IP - 08/01/09 - CR976 
                                    //Set the 'Notes' field on the 'Telephone Actions' screen
                                    //to the 'User Notes' entered on the 'Legal Details' screen.
                                    if (dtLegalDetail.Rows.Count > 0)
                                        SetNotes(dtLegalDetail);
                                    else
                                        txtNotes.Text = string.Empty;

                                    btnSave_Click(null, null);
                                }

                            }
                            catch (Exception ex)
                            {
                                Catch(ex, "drpActionCode_SelectionChangeCommitted (Loading LEG screen)");
                            }
                            finally
                            {
                                StopWait();
                            }
                        }
                        else if (INSSelected)
                        {
                            btnLoadExtraInfo.Text = "Insurance Details";
                            btnLoadExtraInfo.Visible = true;

                            try
                            {
                                Wait();
                                TelAction_INS dialog = new TelAction_INS(dtInsuranceDetail);
                                dialog.FormRoot = this.FormRoot;
                                dialog.FormParent = this;
                                dialog.ShowDialog(this);
                                // if INS save button - save Tel action  details - CredCol walkthrough changes jec 01/06/09
                                if (DialogResult.OK == dialog.ButtonClicked)
                                {
                                    //NM & IP - 08/01/09 - CR976 
                                    //Set the 'Notes' field on the 'Telephone Actions' screen
                                    //to the 'User Notes' entered on the 'Insurance Claim Details' screen.
                                    if (dtInsuranceDetail.Rows.Count > 0)
                                        SetNotes(dtInsuranceDetail);
                                    else
                                        txtNotes.Text = String.Empty;

                                    btnSave_Click(null, null);
                                }
                            }
                            catch (Exception ex)
                            {
                                Catch(ex, "drpActionCode_SelectionChangeCommitted (Loading INS screen)");
                            }
                            finally
                            {
                                StopWait();
                            }
                        }
                        else if (FRDSelected)
                        {
                            btnLoadExtraInfo.Text = "Fraud Details";
                            btnLoadExtraInfo.Visible = true;

                            try
                            {
                                Wait();
                                TelAction_FRD dialog = new TelAction_FRD(dtFraudDetail);
                                dialog.FormRoot = this.FormRoot;
                                dialog.FormParent = this;
                                dialog.ShowDialog(this);
                                // if FRD save button - save Tel action  details - CredCol walkthrough changes jec 01/06/09
                                if (DialogResult.OK == dialog.ButtonClicked)
                                {
                                    //NM & IP - 08/01/09 - CR976 
                                    //Set the 'Notes' field on the 'Telephone Actions' screen
                                    //to the 'User Notes' entered on the 'Fraud Details' screen.
                                    if (dtFraudDetail.Rows.Count > 0)
                                        SetNotes(dtFraudDetail);
                                    else
                                        txtNotes.Text = string.Empty;

                                    btnSave_Click(null, null);
                                }
                            }
                            catch (Exception ex)
                            {
                                Catch(ex, "drpActionCode_SelectionChangeCommitted (Loading FRD screen)");
                            }
                            finally
                            {
                                StopWait();
                            }
                        }
                    }
                    else if (STSSelected)
                    {
                        //IP - 21/10/08 - UAT5.2 - UAT(551)
                        //Pass in the strategy that the account belongs to, to return the 
                        //list of strategies that are the 'exit' strategies for accounts
                        //in the selected strategy.
                        strategy = drpStrategies.SelectedValue.ToString();

                        //--NM Copied the changes done for Tel_Action screen
                        //--NM : As SP & KL requested -- Email from SP on 19/08/2009
                        dtStrategiesToSendTo = drpStrategies.DataSource != null ? ((DataTable)drpStrategies.DataSource).Copy() : stratConfig.GetStrategies();
                        DataRow[] drArray = dtStrategiesToSendTo.Select(CN.Strategy + " = '" + strategy + "'");
                        if (drArray.Length > 0)
                            drArray[0].Delete();


                        //IP - 09/07/10 - UAT1042 - UAT5.2 - Remove WOF from the Send to Strategy as there is already a STW (Send To WriteOff) action.
                        drArray = dtStrategiesToSendTo.Select(CN.Strategy + " = 'WOF'");
                        if (drArray.Length > 0)
                        {
                            dtStrategiesToSendTo.Rows.Remove(drArray[0]);
                        }

                        //Do not display the 'Action Value' field.
                        //txtActionValue.Visible = false;
                        lbActionValue.Text = "Send To Strategy";

                        //Populate the drop down that will be used to select the strategy to send
                        //the account to.
                        drpSendToStrategy.DataSource = dtStrategiesToSendTo;
                        drpSendToStrategy.DisplayMember = CN.Description;
                        drpSendToStrategy.ValueMember = CN.Strategy;
                    }
                    else if (RALLSelected) //UAT(5.2) - 754
                    {
                        gbReAllocate.Enabled = (dgAccounts.CurrentRowIndex >= 0);
                        btnSave.Enabled = false;
                    }
                    else if (RBDWSelected) //jec - 08/07/10 - UAT1065 - Load General Financial Transactions screen to process the RDBW.
                    {
                        reversedBDW = false;
                        try
                        {
                            //RefundAndCorrection refCorr = new RefundAndCorrection(this.FormRoot, this, Acct);
                            //((MainForm)this.FormRoot).AddTabPage(refCorr, 0);
                            //jec - 08/07/10 - UAT1065 - Load General Financial Transactions screen
                            GeneralFinancialTransactions gft = new GeneralFinancialTransactions(this.FormRoot, this, Acct);
                            ((MainForm)this.FormRoot).AddTabPage(gft, 0);

                        }
                        catch (Exception ex)
                        {
                            Catch(ex, "drpActionCode_SelectionChangeCommitted (Loading General Financial Transactions screen)");
                        }
                    }
                    else
                    {
                        lbActionDate.Text = "Due Date";
                        lbActionValue.Text = "Action Value";
                    }
                }
                else
                {
                    lbActionDate.Text = "Due Date";
                    lbActionValue.Text = "Action Value";
                    gbReAllocate.Enabled = false;
                }

                lbReason.Visible = SPASelected;
                drpReason.Visible = SPASelected;
                // txtActionValue.Visible = !STSSelected; //IP - 20/10/08 - UAT5.2 - UAT(551)
                drpSendToStrategy.Visible = STSSelected; //IP - 20/10/08 - UAT5.2 - UAT(551) - display this drop down if the 'STS Action Code' has been selected.
                drpSendToStrategy.Enabled = STSSelected; //IP - 20/10/08 - UAT5.2 - UAT(551) - enable this drop down if the 'STS Action Code' has been selected.

                lbActionValue.Visible = !(TRCSelected || InfoSelected); //IP - 27/09/10 - UAT5.4
                txtActionValue.Visible = !(TRCSelected || STSSelected || InfoSelected); //IP - 27/09/10 - UAT5.4
                dtDueDate.Visible = !(TRCSelected || InfoSelected); //IP - 27/09/10 - UAT5.4
                lbActionDate.Visible = !(TRCSelected || InfoSelected); //IP - 27/09/10 - UAT5.4

            }
            catch (Exception ex)
            {
                Catch(ex, "drpActionCode_SelectionChangeCommitted");
            }
            finally
            {
                StopWait();
            }
        }

        private void ValidateDueDate()
        {
            if (SPASelected && dtDueDate.Value <= DateTime.Today)
            {
                errorProvider1.SetError(dtDueDate, GetResource("M_EXPIRYPAST"));
                dtDueDate.Focus();
            }
            else
            {
                errorProvider1.SetError(dtDueDate, "");
            }
        }

        //IP - 27/09/10 - UAT(37)UAT5.4 - Determine if Notes are required for the selected action.
        private void ValidateNotes()
        {
            int actionMinNotesLength = 0;

            if (drpActionCode.DataSource != null && drpActionCode.SelectedIndex >= 0)
            {
                actionMinNotesLength = Convert.ToInt32(((DataTable)drpActionCode.DataSource).Rows[drpActionCode.SelectedIndex][CN.MinNotesLength]);
            }

            if (txtNotes.Text.Trim().Length < actionMinNotesLength)
            {
                errorProvider1.SetError(txtNotes, GetResource("M_MINNOTESLENGTH", actionMinNotesLength));
                txtNotes.Focus();
            }
            else
            {
                errorProvider1.SetError(txtNotes, "");
            }


        }


        private void txtActionValue_Leave(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                if (!IsStrictMoney(txtActionValue.Text))
                {
                    errorProvider1.SetError(txtActionValue, GetResource("M_NONNUMERIC"));
                    txtActionValue.Select(0, txtActionValue.Text.Length);
                }
                else
                {
                    txtActionValue.Text = Convert.ToDecimal(StripCurrency(txtActionValue.Text)).ToString(DecimalPlaces);
                    errorProvider1.SetError(txtActionValue, "");
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "txtActionValue_Leave");
            }
            finally
            {
                StopWait();
            }
        }

        private void drpBranch_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                drpEmpType_SelectedIndexChanged(sender, e);
            }
            catch (Exception ex)
            {
                Catch(ex, "drpBranch_SelectedIndexChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void PopulateFields(int index, DataView dvAccounts)
        {
            decimal agreementTotal = 0.0M;
            decimal arrears = 0.0M;
            decimal balance = 0.0M;
            decimal instalment = 0.0M;
            double monthsArrears = 0.0F;

            //InitAccountList();

            gbLastAction.Enabled = true;
            gbNewAction.Enabled = true;
            drpActionCode.SelectedIndex = drpActionCode.SelectedIndex == -1 ? -1 : 0;
            //drpActionCode.SelectedIndex = 0;
            btnSave.Enabled = true;

            if (dvAccounts.Count != 0 && dvAccounts.Count > index)
            {
                m_accountRow = index;
                m_custid = dvAccounts[index][CN.CustID].ToString();
                m_accttype = dvAccounts[index][CN.AcctType].ToString();
                m_name = dvAccounts[index][CN.Name].ToString();
                m_creditblocked = Convert.ToBoolean(dvAccounts[index][CN.CreditBlocked]); //IP - 28/04/10 - UAT(983) UAT5.2

                dtLastActionDate.Value = Convert.ToDateTime(dvAccounts[index][CN.DateAdded]);
                txtLastActionCode.Text = (string)dvAccounts[index][CN.ActionCode];

                agreementTotal = Math.Round(agreementTotal + Convert.ToDecimal(dvAccounts[index][CN.AgrmtTotal]), 2);
                arrears = Math.Round(arrears + Convert.ToDecimal(dvAccounts[index][CN.Arrears]), 2);
                balance = Math.Round(balance + Convert.ToDecimal(dvAccounts[index][CN.OutstBal]), 2);
                instalment = Math.Round(instalment + Convert.ToDecimal(dvAccounts[index][CN.Instalment]), 2);
                monthsArrears = Math.Round(monthsArrears + Convert.ToDouble(dvAccounts[index][CN.MonthsInArrears]), 2);

                //Populate all the text boxes on the Customer/Account Information tab
                txtAgreementTotal.Text = agreementTotal.ToString(DecimalPlaces);
                txtArrears.Text = arrears.ToString(DecimalPlaces);
                if (arrears > 0)  // Display in red if > 0  UAT33 jec 12/08/10
                    txtArrears.ForeColor = Color.Red;
                else
                    txtArrears.ForeColor = SystemColors.ControlText;
                txtBalance.Text = balance.ToString(DecimalPlaces);
                if (dvAccounts[index][CN.DateLastPaid] != DBNull.Value)
                {
                    txtDateLastPaid.Text = ((DateTime)dvAccounts[index][CN.DateLastPaid]).ToShortDateString();
                }
                else
                {
                    txtDateLastPaid.Text = String.Empty;
                }
                txtDueDate.Text = dvAccounts[index][CN.DueDay].ToString();
                txtFirstName.Text = dvAccounts[index][CN.FirstName].ToString();
                txtHome.Text = dvAccounts[index][CN.TelNoHome].ToString();
                txtInstalment.Text = instalment.ToString(DecimalPlaces);
                txtLastActionCode.Text = dvAccounts[index][CN.ActionCode].ToString();
                txtLastName.Text = dvAccounts[index][CN.Name].ToString();
                txtMobile.Text = dvAccounts[index][CN.MobileNo].ToString();
                txtMonthArrears.Text = monthsArrears.ToString();
                txtPercentage.Text = dvAccounts[index][CN.PercentagePaid].ToString();
                txt_Provisions.Text = Convert.ToDecimal(dvAccounts[index][CN.ProvisionAmount]) > 0 ? Convert.ToDecimal(dvAccounts[index][CN.ProvisionAmount]).ToString(DecimalPlaces) : "0.00";
                txtStatus.Text = dvAccounts[index][CN.CurrStatus].ToString();
                txtTitle.Text = dvAccounts[index][CN.Title].ToString();
                txtWork.Text = dvAccounts[index][CN.TelNoWork].ToString();
                StageToLaunch.DateProp = Convert.ToDateTime(dvAccounts[index][CN.DateProp]);      //CR1084 jec
                StageToLaunch.AccountNo = dvAccounts[index][CN.AcctNo].ToString();      //CR1084 jec

                //Populate the datagrid dgProducts

                AcctNo = dvAccounts[index][CN.AcctNo].ToString();

                DataSet dsWorklistData = new DataSet();
                dsWorklistData = AccountManager.GetWorklistAccountsData(AcctNo, Config.StoreType, out Error);

                dgProducts.DataSource = dsWorklistData.Tables[0];

                dgProducts.Columns[CN.AcctNo].Visible = false;
                dgProducts.Columns[CN.ItemNo].HeaderText = GetResource("T_PRODCODE");
                dgProducts.Columns[CN.ItemNo].ReadOnly = true;
                dgProducts.Columns[CN.ItemNo].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dgProducts.Columns[CN.ItemNo].Width = 80;

                dgProducts.Columns[CN.Description].HeaderText = GetResource("T_PRODDESC");
                dgProducts.Columns[CN.Description].ReadOnly = true;
                dgProducts.Columns[CN.Description].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dgProducts.Columns[CN.Description].Width = 200;

                dgProducts.Columns[CN.DateDelivered].HeaderText = GetResource("T_DELIVERYDATE");
                dgProducts.Columns[CN.DateDelivered].ReadOnly = true;
                dgProducts.Columns[CN.DateDelivered].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgProducts.Columns[CN.DateDelivered].Width = 80;

                dgProducts.Columns[CN.Quantity].HeaderText = GetResource("T_QUANTITY");
                dgProducts.Columns[CN.Quantity].ReadOnly = true;
                dgProducts.Columns[CN.Quantity].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgProducts.Columns[CN.Quantity].DefaultCellStyle.Format = "F2";
                dgProducts.Columns[CN.Quantity].Width = 50;

                dgProducts.Columns[CN.Value].HeaderText = GetResource("T_VALUE");
                dgProducts.Columns[CN.Value].ReadOnly = true;
                dgProducts.Columns[CN.Value].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgProducts.Columns[CN.Value].DefaultCellStyle.Format = DecimalPlaces;
                dgProducts.Columns[CN.Value].Width = 60;

                //Populate the datagrid dgTransactions

                dgTransactions.DataSource = dsWorklistData.Tables[1];

                dgTransactions.Columns[CN.TransTypeCode].HeaderText = GetResource("T_TYPE");
                dgTransactions.Columns[CN.TransTypeCode].ReadOnly = true;
                dgTransactions.Columns[CN.TransTypeCode].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dgTransactions.Columns[CN.TransTypeCode].Width = 40;

                dgTransactions.Columns[CN.DateTrans].HeaderText = GetResource("T_DATE");
                dgTransactions.Columns[CN.DateTrans].ReadOnly = true;
                dgTransactions.Columns[CN.DateTrans].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgTransactions.Columns[CN.DateTrans].Width = 80;

                dgTransactions.Columns[CN.TransValue].HeaderText = GetResource("T_VALUE");
                dgTransactions.Columns[CN.TransValue].ReadOnly = true;
                dgTransactions.Columns[CN.TransValue].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgTransactions.Columns[CN.TransValue].DefaultCellStyle.Format = DecimalPlaces;
                dgTransactions.Columns[CN.TransValue].Width = 60;

                //Populate the datagrid dgStrategies

                dgStrategies.DataSource = dsWorklistData.Tables[2];

                dgStrategies.Columns[CN.Strategy].HeaderText = GetResource("T_STRATEGY");
                dgStrategies.Columns[CN.Strategy].ReadOnly = true;
                dgStrategies.Columns[CN.Strategy].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgStrategies.Columns[CN.Strategy].Width = 50;

                dgStrategies.Columns[CN.Description].HeaderText = GetResource("T_DESCRIPTION");
                dgStrategies.Columns[CN.Description].ReadOnly = true;
                dgStrategies.Columns[CN.Description].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgStrategies.Columns[CN.Description].Width = 80;

                dgStrategies.Columns[CN.DateFrom].HeaderText = GetResource("T_DATEFROM");
                dgStrategies.Columns[CN.DateFrom].ReadOnly = true;
                dgStrategies.Columns[CN.DateFrom].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgStrategies.Columns[CN.DateFrom].Width = 80;

                dgStrategies.Columns[CN.DateTo].HeaderText = GetResource("T_DATETO");
                dgStrategies.Columns[CN.DateTo].ReadOnly = true;
                dgStrategies.Columns[CN.DateTo].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgStrategies.Columns[CN.DateTo].Width = 80;

                if (dsWorklistData.Tables[2].Rows.Count > 0)
                {
                    DataView dvStrategies = new DataView(dsWorklistData.Tables[2]);
                    dvStrategies.RowFilter = CN.DateTo + " IS  NULL";
                    drpStrategies.SelectedValue = dvStrategies[0][CN.Strategy].ToString(); //dsWorklistData.Tables[2].Rows[0][CN.Strategy].ToString();
                    Strategy = drpStrategies.SelectedValue.ToString();

                    populateActions(); //IP - 21/10/08 - UAT5.2 - UAT(529) - Populate the actions drop down once 'Strategy' drop down has been populated.
                }
                else
                {
                    drpStrategies.SelectedValue = String.Empty;
                    Strategy = String.Empty;
                }

                //Populate the datagrid dgWorklists

                dgWorklists.DataSource = dsWorklistData.Tables[3];

                dgWorklists.Columns[CN.WorkList].HeaderText = GetResource("T_WORKLIST");
                dgWorklists.Columns[CN.WorkList].ReadOnly = true;
                dgWorklists.Columns[CN.WorkList].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgWorklists.Columns[CN.WorkList].Width = 50;

                dgWorklists.Columns[CN.Strategy].HeaderText = GetResource("T_STRATEGY");
                dgWorklists.Columns[CN.Strategy].ReadOnly = true;
                dgWorklists.Columns[CN.Strategy].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgWorklists.Columns[CN.Strategy].Width = 50;

                dgWorklists.Columns[CN.Description].HeaderText = GetResource("T_DESCRIPTION");
                dgWorklists.Columns[CN.Description].ReadOnly = true;
                dgWorklists.Columns[CN.Description].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgWorklists.Columns[CN.Description].Width = 80;

                dgWorklists.Columns[CN.DateFrom].HeaderText = GetResource("T_DATEFROM");
                dgWorklists.Columns[CN.DateFrom].ReadOnly = true;
                dgWorklists.Columns[CN.DateFrom].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgWorklists.Columns[CN.DateFrom].Width = 80;

                dgWorklists.Columns[CN.DateTo].HeaderText = GetResource("T_DATETO");
                dgWorklists.Columns[CN.DateTo].ReadOnly = true;
                dgWorklists.Columns[CN.DateTo].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgWorklists.Columns[CN.DateTo].Width = 80;

                //Populate the datagrid dgLetters

                dgLetters.DataSource = dsWorklistData.Tables[4];

                dgLetters.Columns[CN.Code].HeaderText = GetResource("T_CODE");
                dgLetters.Columns[CN.Code].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgLetters.Columns[CN.Code].ReadOnly = true;
                dgLetters.Columns[CN.Code].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgLetters.Columns[CN.Code].Width = 50;

                dgLetters.Columns[CN.CodeDescription].HeaderText = GetResource("T_DESCRIPTION");
                dgLetters.Columns[CN.CodeDescription].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgLetters.Columns[CN.CodeDescription].ReadOnly = true;
                dgLetters.Columns[CN.CodeDescription].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgLetters.Columns[CN.CodeDescription].Width = 120;

                dgLetters.Columns[CN.DateAcctLttr].HeaderText = GetResource("T_DATESENT");
                dgLetters.Columns[CN.DateAcctLttr].ReadOnly = true;
                dgLetters.Columns[CN.DateAcctLttr].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgLetters.Columns[CN.DateAcctLttr].Width = 80;

                //Populate the datagrid dgSMS

                dgSMS.DataSource = dsWorklistData.Tables[5];

                dgSMS.Columns[CN.Code].HeaderText = GetResource("T_CODE");
                dgSMS.Columns[CN.Code].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgSMS.Columns[CN.Code].ReadOnly = true;
                dgSMS.Columns[CN.Code].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgSMS.Columns[CN.Code].Width = 50;

                dgSMS.Columns[CN.Description].HeaderText = GetResource("T_DESCRIPTION");
                dgSMS.Columns[CN.Description].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgSMS.Columns[CN.Description].ReadOnly = true;
                dgSMS.Columns[CN.Description].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgSMS.Columns[CN.Description].Width = 120;

                dgSMS.Columns[CN.DateAdded].HeaderText = GetResource("T_DATESENT");
                dgSMS.Columns[CN.DateAdded].ReadOnly = true;
                dgSMS.Columns[CN.DateAdded].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgSMS.Columns[CN.DateAdded].Width = 80;

                //Lock this account
                CollectionsManager.LockAccount(AcctNo, Credential.UserId, out Error);
            }
            else //UAT(5.2) - 754
            {
                gbReAllocate.Enabled = false;
            }

        }

        private void drpStrategies_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (drpStrategies.SelectedValue.ToString() != Strategy)
            {
                StrategyChanged = true;
                // populateActions();
                Strategy = drpStrategies.SelectedValue.ToString();
            }
            else
            {
                StrategyChanged = false;
            }
        }

        private void populateActions()
        {
            //IP - 21/10/08 - UAT5.2 - UAT(551)
            //((MainForm)this.FormRoot).statusBar1.Text = "";       //UAT24 jec 12/08/10

            string strategy = drpStrategies.SelectedValue == null ? String.Empty : drpStrategies.SelectedValue.ToString();

            if (drpStrategies.SelectedIndex == 0)
            {
                dvStrategyActions.RowFilter = "";
                //dvStrategyActions = dtAllStrategyActions.DefaultView;
            }
            //else
            //{
            //   dvStrategyActions = ((DataTable)StaticData.Tables[TN.StrategyActions]).DefaultView;
            //}
            else if (strategy.Trim() != String.Empty)
            {
                dvStrategyActions.RowFilter = CN.Strategy + " = '" + strategy + "'";
            }

            //string[] columns = { CN.ActionCode, CN.ActionDescription };
            //DataTable dtStrategyActions = dvStrategyActions.ToTable(true, columns);

            DataSet dsStrategyActions = CollectionsManager.GetStrategyActionsForEmployee(Credential.UserId, strategy.Trim(), false, out Error);
            DataTable dtStrategyActions = new DataTable();

            if (dsStrategyActions.Tables.Count > 0)
                dtStrategyActions = dsStrategyActions.Tables[0];

            if (dtStrategyActions.Rows.Count == 0)
            {
                drpActionCode.Text = String.Empty;
                //IP - 21/10/08 - UAT5.2 - UAT(529) - Display a message if the user does not have any worklists
                //allocated to them for the selected strategy that have actions setup for them.
                ((MainForm)this.FormRoot).statusBar1.Text = "You do not have any worklists assigned to you with actions setup against them for this strategy.";
            }
            drpActionCode.DataSource = dtStrategyActions;
            drpActionCode.DisplayMember = CN.ActionDescription;
            drpActionCode.ValueMember = CN.ActionCode;

            drpActionCode_SelectionChangeCommitted(null, null);
        }

        private void btnFollowUp_Click(object sender, EventArgs e)
        {
            try
            {
                Wait();
                int accountRow = dgAccounts.CurrentRowIndex;

                if (accountRow >= 0)
                {
                    //string acctNo = (string)dgAccounts[accountRow, 0];
                    BailActions actions = new BailActions(AcctNo, FormRoot, this);
                    actions.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnFollowUp_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnAddCode_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgAccounts.CurrentRowIndex >= 0)
                {
                    Wait();
                    string custID = String.Empty;
                    int accountRow = dgAccounts.CurrentRowIndex;

                    custID = ((DataView)dgAccounts.DataSource)[accountRow][CN.CustID].ToString();

                    AddCustAcctCodes codes = new AddCustAcctCodes(AddCodes, custID, txtFirstName.Text, txtLastName.Text, AcctNo);
                    codes.FormRoot = this.FormRoot;
                    codes.FormParent = this;
                    ((MainForm)this.FormRoot).AddTabPage(codes, 8);
                    codes.CustomerCodes(custID);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnAddCode_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnAccountDetails_Click(object sender, EventArgs e)
        {
            try
            {
                Wait();
                int accountRow = dgAccounts.CurrentRowIndex;

                if (accountRow >= 0)
                {
                    //string acctNo = (string)dgAccounts[accountRow, 0];
                    AccountDetails details = new AccountDetails(AcctNo, FormRoot, this);
                    ((MainForm)this.FormRoot).AddTabPage(details, 7);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnAccountDetails_Click");
            }
            finally
            {
                StopWait();
            }
        }

        public override bool ConfirmClose()
        {
            try
            {
                Wait();
                UnlockAccount();
            }
            catch (Exception ex)
            {
                Catch(ex, "ConfirmClose");
            }
            finally
            {
                StopWait();
            }
            return true;
        }

        private void UnlockAccount()
        {
            CollectionsManager.UnlockAccount(AcctNo, Credential.UserId, out Error);
        }

        private void chkOverrideMaxAlloc_CheckedChanged(object sender, EventArgs e)
        {
            if (chkOverrideMaxAlloc.Checked == false)
                return;

            try
            {
                AuthorisePrompt auth = new AuthorisePrompt(this, chkOverrideMaxAlloc, GetResource("M_REFUNDMAXALLOC"));
                auth.ShowDialog();

                if (auth.Authorised == false)
                {
                    chkOverrideMaxAlloc.Checked = false;
                }
            }
            catch (Exception ex)
            {
                chkOverrideMaxAlloc.Checked = false;
                Catch(ex, "chkOverrideMaxAlloc_CheckedChanged");
            }
        }

        //IP - 01/06/09 - Credit Collection Walkthrough Changes.
        //Moved 'Allocate Single Account' functionality from 'FollowUp5_2.cs'.
        private void drpEmployeeTypesAcct_SelectedIndexChanged(object sender, EventArgs e)
        {
            string empType;
            string empTypeStr;
            DataSet ds = null;
            string branch;
            creditstaff.Clear();

            try
            {
                if (drpEmployeeTypesAcct.SelectedIndex >= 0 && staticLoaded == true)
                {
                    drpEmpNameAcct.Enabled = true;
                    drpEmpNameAcct.DataSource = null;
                    empTypeStr = (string)drpEmployeeTypesAcct.SelectedItem;
                    empType = empTypeStr.Substring(0, empTypeStr.IndexOf(":") - 1);
                    branch = (string)drpBranchAcct.SelectedItem;
                    if (!IsNumeric(branch)) branch = "0";

                    //IP - 02/06/08 - Credit Collections - Altered to cater for (3) character Employee Types.
                    int empTypeStrRemove = empTypeStr.IndexOf(":") + 1;

                    //creditstaff.Add("0 : " + empTypeStr.Substring(empTypeStr.IndexOf("-") + 1, empTypeStr.Length - 3));
                    creditstaff.Add("0 : " + empTypeStr.Substring(empTypeStr.IndexOf(":") + 1, empTypeStr.Length - empTypeStrRemove));

                    ds = Login.GetStaffAllocationByType(Convert.ToInt32(empType), Convert.ToInt32(branch), out Error);

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            if (dt.TableName == TN.SalesStaff)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    string str = Convert.ToString(row.ItemArray[0]) + " : " + (string)row.ItemArray[1];
                                    creditstaff.Add(str.ToUpper());
                                }
                            }
                        }
                        drpEmpNameAcct.DataSource = creditstaff;

                        if (creditstaff.Count > 0 && drpEmpNameAcct.Visible == true)
                        {
                            drpEmpNameAcct.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        //IP - 01/06/09 - Credit Collection Walkthrough Changes
        //Moved 'Allocate Single Account' functionality from 'FollowUp5_2.cs'.
        private void drpBranchAcct_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.drpEmployeeTypesAcct_SelectedIndexChanged(sender, e);
        }

        //IP - 01/06/09 - Credit Collection Walkthrough Changes
        //Moved 'Allocate Single Account' functionlaity from 'FollowUp5_2.cs'.
        private void drpEmpNameAcct_SelectedIndexChanged(object sender, EventArgs e)
        {
            string employee;
            //int empeeNo;

            if (drpEmpNameAcct.SelectedIndex >= 0)
            {
                employee = (string)drpEmpNameAcct.SelectedItem;
                employee = employee.Substring(0, employee.IndexOf(":") - 1);
                EmpeeNo = Convert.ToInt32(employee);

                string accountNo = txtAcctNo.UnformattedText;
                if (EmpeeNo != 0 && allocatedAccounts.Contains(accountNo))
                    btnAllocateAcct.Enabled = true;
                else
                    btnAllocateAcct.Enabled = false;
            }


        }

        //IP - 01/06/09 - Credit Collection Walkthrough Changes
        //Moved 'Allocate Single Account' functionality from 'FollowUp5_2.cs'.
        private void btnAllocateAcct_Click(object sender, EventArgs e)
        {
            // taTable dtFields = fields.Tables["Deliveries"];
            // int count = dtFields.Rows.Count;
            Wait();
            Function = "btnAllocateAcct_Click";

            AcctNo = txtAcctNo.Text.Replace("-", "");

            //txtAccountNo.Text = acctNo;

            try
            {
                if (txtAcctNo.Text != "000-0000-0000-0")
                {
                    //int empeeNo;

                    employee = (string)drpEmpNameAcct.SelectedItem;
                    employee = employee.Substring(0, employee.IndexOf(":") - 1);
                    EmpeeNo = Convert.ToInt32(employee);

                    //IP - 08/10/09 - UAT(909)
                    //AccountManager.AllocateAccount(txtAcctNo.Text.Replace("-", ""), EmpeeNo, false, out  Error);
                    AccountManager.AllocateAccount(txtAcctNo.Text.Replace("-", ""), EmpeeNo, out  Error);

                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                    }
                    //refresh rowcounts of allocations
                    //this.drpEmployeeTypesAcct_SelectedIndexChanged(this, null);

                    ((MainForm)this.FormRoot).statusBar1.Text = "Account allocated";

                    txtAcctNo.Text = "000-0000-0000-0";

                    drpEmployeeTypesAcct.SelectedIndex = 0;
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

        //IP - 01/06/09 - Credit Collection Walkthrough Changes
        //Moved 'Allocate Single Account' functionality from 'FollowUp5_2.cs'.
        private void btnClearAcct_Click(object sender, EventArgs e)
        {
            ((MainForm)this.FormRoot).statusBar1.Text = " ";

            txtAcctNo.Text = "000-0000-0000-0";

            drpEmployeeTypesAcct.SelectedIndex = 0;
            //drpEmpNameAcct.SelectedIndex = 0;

            if (drpEmpNameAcct.Items.Count > 0)
            {
                drpEmpNameAcct.SelectedIndex = 0;
            }
        }

        //IP - 01/06/09 - Credit Collection Walkthrough Changes
        //Moved 'Allocate Single Account' functionality from 'FollowUp5_2.cs'
        private void tabAccounts_SelectionChanged(object sender, EventArgs e)
        {
            if (tabAccounts.SelectedTab.Name == "tabAllocateSingleAccount")
            {
                if (allocatedAccounts == null)
                {
                    //Populate the array list with those accounts which can be allocated.
                    DataSet dsStrategyAccounts = new DataSet();
                    dsStrategyAccounts = AccountManager.GetStrategyAccountsToAllocate(out Error);
                    allocatedAccounts = new ArrayList();
                    foreach (DataRow dtRow in dsStrategyAccounts.Tables[0].Rows)
                    {
                        allocatedAccounts.Add(dtRow[0].ToString());
                    }
                }
            }
        }

        //Jec - 17/09/09 - UAT(870) - I have moved this out from the btnSave_Click() method 
        //as in some cases actions such as 'SPA' are saved differently in that the action is saved in the drpActionCode_SelectionChangeCommitted event.
        //therefore this is now re-usable.
        private void CycleToNextAccount()
        {
            int accountRow = m_accountRow;

            //if (chCycleNext.Checked == true)
            if (chCycleNext.Checked == true && dgAccounts.DataSource != null) //IP - 04/01/2010 - UAT(954) - Check that the datasource is not null
            {
                int rowNo = 0;
                //Unlock previous account 
                UnlockAccount();

                PopulateFields(accountRow + 1, (DataView)dgAccounts.DataSource);

                //Is it the last row in the datagrid
                if (((DataView)dgAccounts.DataSource).Count <= accountRow + 1)
                {
                    rowNo = accountRow;
                    if (((DataView)dgAccounts.DataSource).Count <= 1)
                    {
                        btnSave.Enabled = false;
                        InitCustomerDetails();
                    }
                    else
                    {
                        PopulateFields(accountRow - 1, (DataView)dgAccounts.DataSource);
                    }
                }
                else
                {
                    rowNo = accountRow + 1;
                }

                if (((DataView)dgAccounts.DataSource).Count > rowNo)
                {
                    dgAccounts.UnSelect(accountRow);
                    dgAccounts.Select(rowNo);
                    DataGridCell cell = new DataGridCell(rowNo, 0);
                    try
                    {
                        dgAccounts.CurrentCell = cell;
                    }
                    catch
                    {
                        //Doesn't always like this
                    }
                    //dataviewNo++;
                    //InitNewAction();

                }

                DataRowView myRow;
                DataView dv = (DataView)dgAccounts.DataSource;

                myRow = dv[accountRow];
                myRow.Delete();
            }

            InitNewAction();     //jec 06/07/10 UAT1040
            lbReason.Visible = false;
            drpReason.Visible = false;
            lbActionDate.Text = "Due Date";
            lbActionValue.Text = "Action Value";

            //Allow further actions to be made
            txtActionValue.Text = "0";
            txtNotes.Text = "";
            dtDueDate.Value = DateTime.Today;
            //drpActionCode.SelectedIndex = 0;
            btnPrint.Enabled = false;
            StrategyChanged = false;
            drpSendToStrategy.Visible = false; //IP - 20/10/08 - UAT5.2 - UAT(551)

        }
        //jec 05/07/10 UAT1040 - Additional Details pop-ups
        private DataTable CreateLegalDetailDT()
        {
            DataTable dtLegalDetail = new DataTable(TN.LegalDetails);

            dtLegalDetail.Columns.Add(CN.CMAcctno);
            dtLegalDetail.Columns.Add(CN.CMEmpeeno);
            dtLegalDetail.Columns.Add(CN.CMSolicitorNo);
            dtLegalDetail.Columns.Add(CN.CMAuctionProceeds);
            dtLegalDetail.Columns.Add(CN.CMAuctionDate);
            dtLegalDetail.Columns.Add(CN.CMAuctionAmount);
            dtLegalDetail.Columns.Add(CN.CMCourtDeposit);
            dtLegalDetail.Columns.Add(CN.CMCourtAmount);
            dtLegalDetail.Columns.Add(CN.CMCourtDate);
            dtLegalDetail.Columns.Add(CN.CMCaseClosed);
            dtLegalDetail.Columns.Add(CN.CMMentionDate);
            dtLegalDetail.Columns.Add(CN.CMMentionCost);
            dtLegalDetail.Columns.Add(CN.CMPaymentRemittance);
            dtLegalDetail.Columns.Add(CN.CMJudgement);
            dtLegalDetail.Columns.Add(CN.CMLegalAttachmentDate);
            dtLegalDetail.Columns.Add(CN.CMLegalInitiatedDate);
            dtLegalDetail.Columns.Add(CN.CMDefaultedDate);
            dtLegalDetail.Columns.Add(CN.CMUserNotes);

            return dtLegalDetail;
        }

        private DataTable CreateInsuranceDetailDT()
        {
            DataTable dtLegalDetail = new DataTable(TN.InsuranceDetails);

            dtLegalDetail.Columns.Add(CN.CMAcctno);
            dtLegalDetail.Columns.Add(CN.CMEmpeeno);
            dtLegalDetail.Columns.Add(CN.CMInitiatedDate);
            dtLegalDetail.Columns.Add(CN.CMFullOrPartClaim);
            dtLegalDetail.Columns.Add(CN.CMInsAmount);
            dtLegalDetail.Columns.Add(CN.CMInsType);
            dtLegalDetail.Columns.Add(CN.CMIsApproved);
            dtLegalDetail.Columns.Add(CN.CMUserNotes);

            return dtLegalDetail;
        }

        private DataTable CreateFraudDetailDT()
        {
            DataTable dtLegalDetail = new DataTable(TN.FraudDetails);

            dtLegalDetail.Columns.Add(CN.CMAcctno);
            dtLegalDetail.Columns.Add(CN.CMEmpeeno);
            dtLegalDetail.Columns.Add(CN.CMFraudInitiatedDate);
            dtLegalDetail.Columns.Add(CN.CMIsResolved);
            dtLegalDetail.Columns.Add(CN.CMUserNotes);

            return dtLegalDetail;
        }

        private DataTable CreateTRCDetailDT()
        {
            DataTable dtLegalDetail = new DataTable(TN.TraceDetails);

            dtLegalDetail.Columns.Add(CN.CMAcctno);
            dtLegalDetail.Columns.Add(CN.CMEmpeeno);
            dtLegalDetail.Columns.Add(CN.CMTRCInitiatedDate);
            dtLegalDetail.Columns.Add(CN.CMIsResolved);
            dtLegalDetail.Columns.Add(CN.CMUserNotes);

            return dtLegalDetail;
        }
        //jec 05/07/10 UAT1040 - Set the 'Notes' field to the 'User Notes' entered
        private void SetNotes(DataTable extractedInfo)
        {
            //If user notes were entered on either the 'Legal Details', 'Insurance Details'
            //or 'Fraud Details' screen then set the 'Notes' field on the 'Telephone Action'
            //screen to the user notes entered.
            txtNotes.Text = extractedInfo.Rows[0][CN.CMUserNotes].ToString();
        }

        private void dgSMS_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Right || dgSMS.CurrentRow == null)
                    return;

                MenuCommand m1 = new MenuCommand(GetResource("P_VIEWSMS"));
                m1.Click += new System.EventHandler(this.ViewSMS_Click);

                PopupMenu popup = new PopupMenu();
                popup.Animate = Animate.Yes;
                popup.AnimateStyle = Animation.SlideHorVerPositive;
                popup.MenuCommands.Add(m1);

                popup.TrackPopup(dgSMS.PointToScreen(new Point(e.X, e.Y)));
            }
            catch (Exception ex)
            {
                Catch(ex, "dgSMS_MouseUp");
            }
        }

        private void ViewSMS_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (dtSMSDefinition == null)
                {
                    DataSet dsTemp = CollectionsManager.GetSMS(out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else if (dsTemp.Tables.Count > 0)
                        dtSMSDefinition = dsTemp.Tables[0];
                }

                if (dtSMSDefinition != null && dgSMS.CurrentRow != null)
                {
                    string smsName = dgSMS[GetResource("T_CODE"), dgSMS.CurrentRow.Index].Value.ToString().Trim();
                    string text = dtSMSDefinition.Select(CN.SMSName + " = '" + smsName + "'")[0][CN.SMSText].ToString();
                    string header = smsName + " - " + dtSMSDefinition.
                                     Select(CN.SMSName + " = '" + smsName + "'")[0][CN.Description].ToString();
                    // UAT 896 - add description to title of pop up
                    ViewSMSPopup smsPop = new ViewSMSPopup(text);
                    smsPop.Text = header;
                    smsPop.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "ViewSMS_Click");
            }
        }

        //IP - 09/10/09 - UAT(909) - Tooltip to display the number of accounts that can be allocated to the selected employee.
        private void drpRAEmpName_MouseHover(object sender, EventArgs e)
        {
            ttBailReview.SetToolTip(drpRAEmpName, "");

            if (drpRAEmpName.SelectedIndex >= 0)
            {

                int index = Convert.ToInt32(Convert.ToString(drpRAEmpName.SelectedText).IndexOf('('));
                if (index >= 0)
                {
                    index = index + 1;
                    int index2 = Convert.ToInt32(Convert.ToString(drpRAEmpName.SelectedText).IndexOf(')'));
                    int length = index2 - index;
                    string noCanAlloc = drpRAEmpName.SelectedText.Substring(index, length);

                    ttBailReview.SetToolTip(drpRAEmpName, noCanAlloc + " " + "account(s) can be allocated to this employee");
                }

            }





        }

        private void txtAcctNo_Leave(object sender, EventArgs e)
        {
            decimal provisionamount = 0;

            if (decimal.TryParse(AccountManager.ProvisionGetForAccount(txtAcctNo.UnformattedText).ToString(), out provisionamount))
            {
                txt_provisionamount.Text = provisionamount.ToString(DecimalPlaces);
            }
            else
            {
                txt_provisionamount.Text = "0.00";
            }
        }

        // Show References
        private void btnReferences_Click(object sender, EventArgs e)
        {
            try
            {
                Wait();
                int accountRow = dgAccounts.CurrentRowIndex;

                if (accountRow >= 0)
                {
                    SanctionStage2 s2 = null;
                    s2 = new SanctionStage2(m_custid, StageToLaunch.DateProp, StageToLaunch.AccountNo, m_accttype, SM.View, FormRoot, this, this);
                    ((MainForm)FormRoot).AddTabPage(s2);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnReferences_Click");
            }
            finally
            {
                StopWait();
            }
        }

    }
}
