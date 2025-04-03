using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;

namespace STL.PL
{
    /// <summary>
    /// Summary description for CustomerMailing.
    /// </summary>
    public class CustomerMailing : CommonForm
    {
        private Crownwood.Magic.Controls.TabControl tcAllocated;
        private Crownwood.Magic.Controls.TabPage tpQuery;
        private Crownwood.Magic.Controls.TabPage tpResult;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.DataGrid dgAccounts;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.NumericUpDown ArrearsUpDown;
        private System.Windows.Forms.RadioButton radioNeither;
        private System.Windows.Forms.RadioButton radioValues;
        private System.Windows.Forms.RadioButton radioQuantities;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboNotPurchased;
        private System.Windows.Forms.ComboBox comboLetters;
        private System.Windows.Forms.ComboBox comboPurchased;
        private System.Windows.Forms.ComboBox comboNoLetters;
        private System.Windows.Forms.ComboBox comboBranches;
        private System.Windows.Forms.ComboBox comboAccountCodes;
        private System.Windows.Forms.ComboBox ComboAge;
        private System.Windows.Forms.ComboBox comboNoCustCode;
        private System.Windows.Forms.ComboBox comboNoAccountCode;
        private System.Windows.Forms.TextBox textNoLetter;
        private System.Windows.Forms.ComboBox comboArrears;
        private System.Windows.Forms.TextBox textBranch;
        private System.Windows.Forms.TextBox textNoAccountCode;
        private System.Windows.Forms.RadioButton radioDelivery;
        private System.Windows.Forms.RadioButton radioOrders;
        private System.Windows.Forms.GroupBox groupOrders;
        private System.Windows.Forms.CheckBox checkHP;
        private System.Windows.Forms.CheckBox checkSpecial;
        private System.Windows.Forms.CheckBox checkCash;
        private System.Windows.Forms.DateTimePicker dtPurchaseEnd;
        private System.Windows.Forms.DateTimePicker dtCustPurchaseStart;
        private System.Windows.Forms.DateTimePicker dtNoLettersEnd;
        private System.Windows.Forms.DateTimePicker dtNoLettersStart;
        private System.Windows.Forms.DateTimePicker dtNoPurchaseStart;
        private System.Windows.Forms.DateTimePicker dtLettersStart;
        private System.Windows.Forms.DateTimePicker dtLettersEnd;
        private System.Windows.Forms.DateTimePicker dtNoPurchaseEnd;
        private System.Windows.Forms.ComboBox comboCustCodes;
        private System.Windows.Forms.TextBox textCustomerCode;
        private System.Windows.Forms.TextBox textNoCustomerCode;
        private System.Windows.Forms.TextBox textNoPurchased;
        private System.Windows.Forms.TextBox textLetter;
        private System.Windows.Forms.TextBox textPurchased;
        private System.Windows.Forms.TextBox textAccountCode;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand FileOpen;
        private Label labelCurrStatus;
        private NumericUpDown CurrStatusUpDown;
        private Label label2;
        private NumericUpDown HighestEverUpDown;
        private Label labeland;
        private NumericUpDown OldUpDown;
        private NumericUpDown YoungUpDown;
        private CheckBox checkExcludeCancel;
        private DataSet accounts;
        //private DataView acctView;
        private Button btnLetter;
        private ComboBox drpLetter;
        private GroupBox groupDatabase;
        private RadioButton radioDBLive;
        private RadioButton radioDBReporting;
        private string QueryName = "";
        private string statusText = "";
        private bool fileopen;
        private CheckBox chbExcel;
        private Label labelTotal;
        private TextBox textTotal;
        private ErrorProvider errorProvider1;
        private IContainer components;

        public CustomerMailing(Form root, Form parent)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });

            dtCustPurchaseStart.Value = DateTime.Now.AddYears(-2);
            dtNoPurchaseStart.Value = DateTime.Now.AddYears(-2);
            dtLettersStart.Value = DateTime.Now.AddYears(-2);
            dtNoLettersStart.Value = DateTime.Now.AddYears(-2);

            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

            StringCollection letter = new StringCollection();

            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.AdhocLetter]).Rows)
            {
                string str = (string)row.ItemArray[0] + " - " + (string)row.ItemArray[1];
                letter.Add(str.ToUpper());
            }
            drpLetter.DataSource = letter;




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
            this.tcAllocated = new Crownwood.Magic.Controls.TabControl();
            this.tpResult = new Crownwood.Magic.Controls.TabPage();
            this.textTotal = new System.Windows.Forms.TextBox();
            this.labelTotal = new System.Windows.Forms.Label();
            this.chbExcel = new System.Windows.Forms.CheckBox();
            this.drpLetter = new System.Windows.Forms.ComboBox();
            this.btnLetter = new System.Windows.Forms.Button();
            this.btnExcel = new System.Windows.Forms.Button();
            this.dgAccounts = new System.Windows.Forms.DataGrid();
            this.tpQuery = new Crownwood.Magic.Controls.TabPage();
            this.groupDatabase = new System.Windows.Forms.GroupBox();
            this.radioDBLive = new System.Windows.Forms.RadioButton();
            this.radioDBReporting = new System.Windows.Forms.RadioButton();
            this.checkExcludeCancel = new System.Windows.Forms.CheckBox();
            this.labeland = new System.Windows.Forms.Label();
            this.OldUpDown = new System.Windows.Forms.NumericUpDown();
            this.YoungUpDown = new System.Windows.Forms.NumericUpDown();
            this.HighestEverUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.CurrStatusUpDown = new System.Windows.Forms.NumericUpDown();
            this.labelCurrStatus = new System.Windows.Forms.Label();
            this.textCustomerCode = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.ArrearsUpDown = new System.Windows.Forms.NumericUpDown();
            this.textBranch = new System.Windows.Forms.TextBox();
            this.textNoAccountCode = new System.Windows.Forms.TextBox();
            this.textNoCustomerCode = new System.Windows.Forms.TextBox();
            this.textNoLetter = new System.Windows.Forms.TextBox();
            this.textAccountCode = new System.Windows.Forms.TextBox();
            this.textNoPurchased = new System.Windows.Forms.TextBox();
            this.textLetter = new System.Windows.Forms.TextBox();
            this.textPurchased = new System.Windows.Forms.TextBox();
            this.dtNoLettersEnd = new System.Windows.Forms.DateTimePicker();
            this.dtNoLettersStart = new System.Windows.Forms.DateTimePicker();
            this.dtLettersStart = new System.Windows.Forms.DateTimePicker();
            this.dtLettersEnd = new System.Windows.Forms.DateTimePicker();
            this.dtNoPurchaseEnd = new System.Windows.Forms.DateTimePicker();
            this.dtPurchaseEnd = new System.Windows.Forms.DateTimePicker();
            this.dtNoPurchaseStart = new System.Windows.Forms.DateTimePicker();
            this.dtCustPurchaseStart = new System.Windows.Forms.DateTimePicker();
            this.comboArrears = new System.Windows.Forms.ComboBox();
            this.comboBranches = new System.Windows.Forms.ComboBox();
            this.comboNotPurchased = new System.Windows.Forms.ComboBox();
            this.comboLetters = new System.Windows.Forms.ComboBox();
            this.comboNoLetters = new System.Windows.Forms.ComboBox();
            this.comboAccountCodes = new System.Windows.Forms.ComboBox();
            this.ComboAge = new System.Windows.Forms.ComboBox();
            this.comboNoCustCode = new System.Windows.Forms.ComboBox();
            this.comboNoAccountCode = new System.Windows.Forms.ComboBox();
            this.comboPurchased = new System.Windows.Forms.ComboBox();
            this.comboCustCodes = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkSpecial = new System.Windows.Forms.CheckBox();
            this.checkCash = new System.Windows.Forms.CheckBox();
            this.checkHP = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioValues = new System.Windows.Forms.RadioButton();
            this.radioQuantities = new System.Windows.Forms.RadioButton();
            this.radioNeither = new System.Windows.Forms.RadioButton();
            this.groupOrders = new System.Windows.Forms.GroupBox();
            this.radioOrders = new System.Windows.Forms.RadioButton();
            this.radioDelivery = new System.Windows.Forms.RadioButton();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.FileOpen = new Crownwood.Magic.Menus.MenuCommand();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.tcAllocated.SuspendLayout();
            this.tpResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
            this.tpQuery.SuspendLayout();
            this.groupDatabase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OldUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.YoungUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HighestEverUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CurrStatusUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ArrearsUpDown)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupOrders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // tcAllocated
            // 
            this.tcAllocated.IDEPixelArea = true;
            this.tcAllocated.Location = new System.Drawing.Point(24, 16);
            this.tcAllocated.Name = "tcAllocated";
            this.tcAllocated.PositionTop = true;
            this.tcAllocated.SelectedIndex = 1;
            this.tcAllocated.SelectedTab = this.tpResult;
            this.tcAllocated.Size = new System.Drawing.Size(736, 464);
            this.tcAllocated.TabIndex = 53;
            this.tcAllocated.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpQuery,
            this.tpResult});
            this.tcAllocated.SelectionChanged += new System.EventHandler(this.tcAllocated_SelectionChanged);
            // 
            // tpResult
            // 
            this.tpResult.Controls.Add(this.textTotal);
            this.tpResult.Controls.Add(this.labelTotal);
            this.tpResult.Controls.Add(this.chbExcel);
            this.tpResult.Controls.Add(this.drpLetter);
            this.tpResult.Controls.Add(this.btnLetter);
            this.tpResult.Controls.Add(this.btnExcel);
            this.tpResult.Controls.Add(this.dgAccounts);
            this.tpResult.Location = new System.Drawing.Point(0, 25);
            this.tpResult.Name = "tpResult";
            this.tpResult.Size = new System.Drawing.Size(736, 439);
            this.tpResult.TabIndex = 1;
            this.tpResult.Title = "Result";
            this.tpResult.Visible = false;
            // 
            // textTotal
            // 
            this.textTotal.Location = new System.Drawing.Point(596, 414);
            this.textTotal.Name = "textTotal";
            this.textTotal.Size = new System.Drawing.Size(132, 23);
            this.textTotal.TabIndex = 96;
            // 
            // labelTotal
            // 
            this.labelTotal.AutoSize = true;
            this.labelTotal.Location = new System.Drawing.Point(541, 417);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(53, 15);
            this.labelTotal.TabIndex = 95;
            this.labelTotal.Text = "Quantity";
            this.labelTotal.Visible = false;
            // 
            // chbExcel
            // 
            this.chbExcel.AutoSize = true;
            this.chbExcel.Location = new System.Drawing.Point(464, 26);
            this.chbExcel.Name = "chbExcel";
            this.chbExcel.Size = new System.Drawing.Size(196, 19);
            this.chbExcel.TabIndex = 94;
            this.chbExcel.Text = "Generated immediately via Excel";
            this.chbExcel.UseVisualStyleBackColor = true;
            // 
            // drpLetter
            // 
            this.drpLetter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpLetter.Location = new System.Drawing.Point(154, 26);
            this.drpLetter.Name = "drpLetter";
            this.drpLetter.Size = new System.Drawing.Size(184, 23);
            this.drpLetter.TabIndex = 92;
            // 
            // btnLetter
            // 
            this.btnLetter.Location = new System.Drawing.Point(344, 24);
            this.btnLetter.Name = "btnLetter";
            this.btnLetter.Size = new System.Drawing.Size(114, 23);
            this.btnLetter.TabIndex = 91;
            this.btnLetter.Text = "Generate Letter";
            this.btnLetter.UseVisualStyleBackColor = true;
            this.btnLetter.Click += new System.EventHandler(this.btnLetter_Click);
            // 
            // btnExcel
            // 
            this.btnExcel.Location = new System.Drawing.Point(651, 24);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(68, 23);
            this.btnExcel.TabIndex = 90;
            this.btnExcel.Text = "Excel only";
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // dgAccounts
            // 
            this.dgAccounts.DataMember = "";
            this.dgAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAccounts.Location = new System.Drawing.Point(8, 64);
            this.dgAccounts.Name = "dgAccounts";
            this.dgAccounts.ReadOnly = true;
            this.dgAccounts.Size = new System.Drawing.Size(720, 336);
            this.dgAccounts.TabIndex = 4;
            // 
            // tpQuery
            // 
            this.tpQuery.Controls.Add(this.groupDatabase);
            this.tpQuery.Controls.Add(this.checkExcludeCancel);
            this.tpQuery.Controls.Add(this.labeland);
            this.tpQuery.Controls.Add(this.OldUpDown);
            this.tpQuery.Controls.Add(this.YoungUpDown);
            this.tpQuery.Controls.Add(this.HighestEverUpDown);
            this.tpQuery.Controls.Add(this.label2);
            this.tpQuery.Controls.Add(this.CurrStatusUpDown);
            this.tpQuery.Controls.Add(this.labelCurrStatus);
            this.tpQuery.Controls.Add(this.textCustomerCode);
            this.tpQuery.Controls.Add(this.button1);
            this.tpQuery.Controls.Add(this.btnRun);
            this.tpQuery.Controls.Add(this.ArrearsUpDown);
            this.tpQuery.Controls.Add(this.textBranch);
            this.tpQuery.Controls.Add(this.textNoAccountCode);
            this.tpQuery.Controls.Add(this.textNoCustomerCode);
            this.tpQuery.Controls.Add(this.textNoLetter);
            this.tpQuery.Controls.Add(this.textAccountCode);
            this.tpQuery.Controls.Add(this.textNoPurchased);
            this.tpQuery.Controls.Add(this.textLetter);
            this.tpQuery.Controls.Add(this.textPurchased);
            this.tpQuery.Controls.Add(this.dtNoLettersEnd);
            this.tpQuery.Controls.Add(this.dtNoLettersStart);
            this.tpQuery.Controls.Add(this.dtLettersStart);
            this.tpQuery.Controls.Add(this.dtLettersEnd);
            this.tpQuery.Controls.Add(this.dtNoPurchaseEnd);
            this.tpQuery.Controls.Add(this.dtPurchaseEnd);
            this.tpQuery.Controls.Add(this.dtNoPurchaseStart);
            this.tpQuery.Controls.Add(this.dtCustPurchaseStart);
            this.tpQuery.Controls.Add(this.comboArrears);
            this.tpQuery.Controls.Add(this.comboBranches);
            this.tpQuery.Controls.Add(this.comboNotPurchased);
            this.tpQuery.Controls.Add(this.comboLetters);
            this.tpQuery.Controls.Add(this.comboNoLetters);
            this.tpQuery.Controls.Add(this.comboAccountCodes);
            this.tpQuery.Controls.Add(this.ComboAge);
            this.tpQuery.Controls.Add(this.comboNoCustCode);
            this.tpQuery.Controls.Add(this.comboNoAccountCode);
            this.tpQuery.Controls.Add(this.comboPurchased);
            this.tpQuery.Controls.Add(this.comboCustCodes);
            this.tpQuery.Controls.Add(this.label1);
            this.tpQuery.Controls.Add(this.label10);
            this.tpQuery.Controls.Add(this.label12);
            this.tpQuery.Controls.Add(this.label13);
            this.tpQuery.Controls.Add(this.label14);
            this.tpQuery.Controls.Add(this.label15);
            this.tpQuery.Controls.Add(this.label16);
            this.tpQuery.Controls.Add(this.label17);
            this.tpQuery.Controls.Add(this.label18);
            this.tpQuery.Controls.Add(this.label19);
            this.tpQuery.Controls.Add(this.label20);
            this.tpQuery.Controls.Add(this.groupBox1);
            this.tpQuery.Controls.Add(this.groupBox2);
            this.tpQuery.Controls.Add(this.groupOrders);
            this.tpQuery.Location = new System.Drawing.Point(0, 25);
            this.tpQuery.Name = "tpQuery";
            this.tpQuery.Selected = false;
            this.tpQuery.Size = new System.Drawing.Size(736, 439);
            this.tpQuery.TabIndex = 0;
            this.tpQuery.Title = "Query";
            this.tpQuery.PropertyChanged += new Crownwood.Magic.Controls.TabPage.PropChangeHandler(this.tpQuery_PropertyChanged);
            // 
            // groupDatabase
            // 
            this.groupDatabase.Controls.Add(this.radioDBLive);
            this.groupDatabase.Controls.Add(this.radioDBReporting);
            this.groupDatabase.Location = new System.Drawing.Point(496, 199);
            this.groupDatabase.Name = "groupDatabase";
            this.groupDatabase.Size = new System.Drawing.Size(236, 49);
            this.groupDatabase.TabIndex = 99;
            this.groupDatabase.TabStop = false;
            this.groupDatabase.Text = "Database";
            // 
            // radioDBLive
            // 
            this.radioDBLive.Location = new System.Drawing.Point(16, 18);
            this.radioDBLive.Name = "radioDBLive";
            this.radioDBLive.Size = new System.Drawing.Size(78, 24);
            this.radioDBLive.TabIndex = 97;
            this.radioDBLive.Text = "Live";
            // 
            // radioDBReporting
            // 
            this.radioDBReporting.Checked = true;
            this.radioDBReporting.Location = new System.Drawing.Point(116, 18);
            this.radioDBReporting.Name = "radioDBReporting";
            this.radioDBReporting.Size = new System.Drawing.Size(72, 24);
            this.radioDBReporting.TabIndex = 96;
            this.radioDBReporting.TabStop = true;
            this.radioDBReporting.Text = "Reporting";
            // 
            // checkExcludeCancel
            // 
            this.checkExcludeCancel.AutoSize = true;
            this.checkExcludeCancel.Checked = true;
            this.checkExcludeCancel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkExcludeCancel.Location = new System.Drawing.Point(512, 419);
            this.checkExcludeCancel.Name = "checkExcludeCancel";
            this.checkExcludeCancel.Size = new System.Drawing.Size(140, 19);
            this.checkExcludeCancel.TabIndex = 107;
            this.checkExcludeCancel.Text = "Exclude Cancellations";
            this.checkExcludeCancel.UseVisualStyleBackColor = true;
            // 
            // labeland
            // 
            this.labeland.AutoSize = true;
            this.labeland.Enabled = false;
            this.labeland.Location = new System.Drawing.Point(415, 229);
            this.labeland.Name = "labeland";
            this.labeland.Size = new System.Drawing.Size(27, 15);
            this.labeland.TabIndex = 106;
            this.labeland.Text = "and";
            // 
            // OldUpDown
            // 
            this.OldUpDown.Enabled = false;
            this.OldUpDown.Location = new System.Drawing.Point(446, 225);
            this.OldUpDown.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.OldUpDown.Name = "OldUpDown";
            this.OldUpDown.Size = new System.Drawing.Size(44, 23);
            this.OldUpDown.TabIndex = 105;
            // 
            // YoungUpDown
            // 
            this.YoungUpDown.Enabled = false;
            this.YoungUpDown.Location = new System.Drawing.Point(364, 225);
            this.YoungUpDown.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.YoungUpDown.Name = "YoungUpDown";
            this.YoungUpDown.Size = new System.Drawing.Size(44, 23);
            this.YoungUpDown.TabIndex = 104;
            // 
            // HighestEverUpDown
            // 
            this.HighestEverUpDown.Location = new System.Drawing.Point(418, 406);
            this.HighestEverUpDown.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.HighestEverUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.HighestEverUpDown.Name = "HighestEverUpDown";
            this.HighestEverUpDown.Size = new System.Drawing.Size(46, 23);
            this.HighestEverUpDown.TabIndex = 103;
            this.HighestEverUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(293, 408);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 15);
            this.label2.TabIndex = 102;
            this.label2.Text = "Highest Ever Status:";
            // 
            // CurrStatusUpDown
            // 
            this.CurrStatusUpDown.Location = new System.Drawing.Point(228, 404);
            this.CurrStatusUpDown.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.CurrStatusUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CurrStatusUpDown.Name = "CurrStatusUpDown";
            this.CurrStatusUpDown.Size = new System.Drawing.Size(47, 23);
            this.CurrStatusUpDown.TabIndex = 101;
            this.CurrStatusUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // labelCurrStatus
            // 
            this.labelCurrStatus.AutoSize = true;
            this.labelCurrStatus.Location = new System.Drawing.Point(3, 408);
            this.labelCurrStatus.Name = "labelCurrStatus";
            this.labelCurrStatus.Size = new System.Drawing.Size(129, 15);
            this.labelCurrStatus.TabIndex = 100;
            this.labelCurrStatus.Text = "Current Highest Status:";
            this.labelCurrStatus.Click += new System.EventHandler(this.labelCurrStatus_Click);
            // 
            // textCustomerCode
            // 
            this.textCustomerCode.Location = new System.Drawing.Point(364, 23);
            this.textCustomerCode.Name = "textCustomerCode";
            this.textCustomerCode.ReadOnly = true;
            this.textCustomerCode.Size = new System.Drawing.Size(100, 23);
            this.textCustomerCode.TabIndex = 99;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(620, 15);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 24);
            this.button1.TabIndex = 95;
            this.button1.Text = "Save";
            this.button1.Click += new System.EventHandler(this.Save_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(540, 15);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(72, 24);
            this.btnRun.TabIndex = 94;
            this.btnRun.Text = "Run";
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // ArrearsUpDown
            // 
            this.ArrearsUpDown.Location = new System.Drawing.Point(364, 367);
            this.ArrearsUpDown.Name = "ArrearsUpDown";
            this.ArrearsUpDown.Size = new System.Drawing.Size(100, 23);
            this.ArrearsUpDown.TabIndex = 93;
            // 
            // textBranch
            // 
            this.textBranch.Location = new System.Drawing.Point(364, 335);
            this.textBranch.Name = "textBranch";
            this.textBranch.ReadOnly = true;
            this.textBranch.Size = new System.Drawing.Size(100, 23);
            this.textBranch.TabIndex = 86;
            this.textBranch.TextChanged += new System.EventHandler(this.textBranch_TextChanged);
            // 
            // textNoAccountCode
            // 
            this.textNoAccountCode.Location = new System.Drawing.Point(364, 303);
            this.textNoAccountCode.Name = "textNoAccountCode";
            this.textNoAccountCode.ReadOnly = true;
            this.textNoAccountCode.Size = new System.Drawing.Size(100, 23);
            this.textNoAccountCode.TabIndex = 85;
            this.textNoAccountCode.TextChanged += new System.EventHandler(this.textNoAccountCode_TextChanged);
            // 
            // textNoCustomerCode
            // 
            this.textNoCustomerCode.Location = new System.Drawing.Point(364, 263);
            this.textNoCustomerCode.Name = "textNoCustomerCode";
            this.textNoCustomerCode.ReadOnly = true;
            this.textNoCustomerCode.Size = new System.Drawing.Size(100, 23);
            this.textNoCustomerCode.TabIndex = 84;
            this.textNoCustomerCode.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            // 
            // textNoLetter
            // 
            this.textNoLetter.Location = new System.Drawing.Point(364, 159);
            this.textNoLetter.Name = "textNoLetter";
            this.textNoLetter.ReadOnly = true;
            this.textNoLetter.Size = new System.Drawing.Size(100, 23);
            this.textNoLetter.TabIndex = 83;
            this.textNoLetter.TextChanged += new System.EventHandler(this.textNoLetter_TextChanged);
            // 
            // textAccountCode
            // 
            this.textAccountCode.Location = new System.Drawing.Point(364, 191);
            this.textAccountCode.Name = "textAccountCode";
            this.textAccountCode.ReadOnly = true;
            this.textAccountCode.Size = new System.Drawing.Size(100, 23);
            this.textAccountCode.TabIndex = 82;
            this.textAccountCode.TextChanged += new System.EventHandler(this.textAccountCodes_TextChanged);
            // 
            // textNoPurchased
            // 
            this.textNoPurchased.Location = new System.Drawing.Point(364, 95);
            this.textNoPurchased.Name = "textNoPurchased";
            this.textNoPurchased.ReadOnly = true;
            this.textNoPurchased.Size = new System.Drawing.Size(100, 23);
            this.textNoPurchased.TabIndex = 81;
            this.textNoPurchased.TextChanged += new System.EventHandler(this.textNoPurchaseSet_TextChanged);
            // 
            // textLetter
            // 
            this.textLetter.Location = new System.Drawing.Point(364, 127);
            this.textLetter.Name = "textLetter";
            this.textLetter.ReadOnly = true;
            this.textLetter.Size = new System.Drawing.Size(100, 23);
            this.textLetter.TabIndex = 80;
            this.textLetter.TextChanged += new System.EventHandler(this.textLetterSet_TextChanged);
            // 
            // textPurchased
            // 
            this.textPurchased.Location = new System.Drawing.Point(364, 63);
            this.textPurchased.Name = "textPurchased";
            this.textPurchased.ReadOnly = true;
            this.textPurchased.Size = new System.Drawing.Size(100, 23);
            this.textPurchased.TabIndex = 79;
            this.textPurchased.TextChanged += new System.EventHandler(this.textPurchaseSet_TextChanged);
            // 
            // dtNoLettersEnd
            // 
            this.dtNoLettersEnd.CustomFormat = "dd MMM yyyy";
            this.dtNoLettersEnd.Enabled = false;
            this.dtNoLettersEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtNoLettersEnd.Location = new System.Drawing.Point(612, 159);
            this.dtNoLettersEnd.Name = "dtNoLettersEnd";
            this.dtNoLettersEnd.Size = new System.Drawing.Size(120, 23);
            this.dtNoLettersEnd.TabIndex = 78;
            // 
            // dtNoLettersStart
            // 
            this.dtNoLettersStart.CustomFormat = "dd MMM yyyy";
            this.dtNoLettersStart.Enabled = false;
            this.dtNoLettersStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtNoLettersStart.Location = new System.Drawing.Point(476, 159);
            this.dtNoLettersStart.Name = "dtNoLettersStart";
            this.dtNoLettersStart.Size = new System.Drawing.Size(120, 23);
            this.dtNoLettersStart.TabIndex = 77;
            this.dtNoLettersStart.ValueChanged += new System.EventHandler(this.dtNoLettersStart_ValueChanged);
            // 
            // dtLettersStart
            // 
            this.dtLettersStart.CustomFormat = "dd MMM yyyy";
            this.dtLettersStart.Enabled = false;
            this.dtLettersStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtLettersStart.Location = new System.Drawing.Point(476, 127);
            this.dtLettersStart.Name = "dtLettersStart";
            this.dtLettersStart.Size = new System.Drawing.Size(120, 23);
            this.dtLettersStart.TabIndex = 76;
            // 
            // dtLettersEnd
            // 
            this.dtLettersEnd.CustomFormat = "dd MMM yyyy";
            this.dtLettersEnd.Enabled = false;
            this.dtLettersEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtLettersEnd.Location = new System.Drawing.Point(612, 127);
            this.dtLettersEnd.Name = "dtLettersEnd";
            this.dtLettersEnd.Size = new System.Drawing.Size(120, 23);
            this.dtLettersEnd.TabIndex = 75;
            // 
            // dtNoPurchaseEnd
            // 
            this.dtNoPurchaseEnd.CustomFormat = "dd MMM yyyy";
            this.dtNoPurchaseEnd.Enabled = false;
            this.dtNoPurchaseEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtNoPurchaseEnd.Location = new System.Drawing.Point(612, 95);
            this.dtNoPurchaseEnd.Name = "dtNoPurchaseEnd";
            this.dtNoPurchaseEnd.Size = new System.Drawing.Size(120, 23);
            this.dtNoPurchaseEnd.TabIndex = 74;
            this.dtNoPurchaseEnd.ValueChanged += new System.EventHandler(this.dateTimePicker13_ValueChanged);
            // 
            // dtPurchaseEnd
            // 
            this.dtPurchaseEnd.CustomFormat = "dd MMM yyyy";
            this.dtPurchaseEnd.Enabled = false;
            this.dtPurchaseEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtPurchaseEnd.Location = new System.Drawing.Point(612, 63);
            this.dtPurchaseEnd.Name = "dtPurchaseEnd";
            this.dtPurchaseEnd.Size = new System.Drawing.Size(120, 23);
            this.dtPurchaseEnd.TabIndex = 73;
            // 
            // dtNoPurchaseStart
            // 
            this.dtNoPurchaseStart.CustomFormat = "dd MMM yyyy";
            this.dtNoPurchaseStart.Enabled = false;
            this.dtNoPurchaseStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtNoPurchaseStart.Location = new System.Drawing.Point(476, 95);
            this.dtNoPurchaseStart.Name = "dtNoPurchaseStart";
            this.dtNoPurchaseStart.Size = new System.Drawing.Size(120, 23);
            this.dtNoPurchaseStart.TabIndex = 72;
            this.dtNoPurchaseStart.ValueChanged += new System.EventHandler(this.dTNoPurchaseStart_ValueChanged);
            // 
            // dtCustPurchaseStart
            // 
            this.dtCustPurchaseStart.CustomFormat = "dd MMM yyyy";
            this.dtCustPurchaseStart.Enabled = false;
            this.dtCustPurchaseStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtCustPurchaseStart.Location = new System.Drawing.Point(476, 63);
            this.dtCustPurchaseStart.Name = "dtCustPurchaseStart";
            this.dtCustPurchaseStart.Size = new System.Drawing.Size(120, 23);
            this.dtCustPurchaseStart.TabIndex = 71;
            // 
            // comboArrears
            // 
            this.comboArrears.Items.AddRange(new object[] {
            "No Limitation",
            "Arrears <",
            "Arrears/Instalment <"});
            this.comboArrears.Location = new System.Drawing.Point(228, 367);
            this.comboArrears.Name = "comboArrears";
            this.comboArrears.Size = new System.Drawing.Size(121, 23);
            this.comboArrears.TabIndex = 70;
            this.comboArrears.Text = "No Limitation";
            this.comboArrears.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // comboBranches
            // 
            this.comboBranches.Items.AddRange(new object[] {
            "No Limitation",
            "Branch Set"});
            this.comboBranches.Location = new System.Drawing.Point(228, 335);
            this.comboBranches.Name = "comboBranches";
            this.comboBranches.Size = new System.Drawing.Size(121, 23);
            this.comboBranches.TabIndex = 69;
            this.comboBranches.Text = "No Limitation";
            this.comboBranches.SelectedIndexChanged += new System.EventHandler(this.comboBranches_SelectedIndexChanged);
            // 
            // comboNotPurchased
            // 
            this.comboNotPurchased.Items.AddRange(new object[] {
            "No Limitation",
            "Items in Categories",
            "Items",
            "Items beginning with"});
            this.comboNotPurchased.Location = new System.Drawing.Point(228, 95);
            this.comboNotPurchased.Name = "comboNotPurchased";
            this.comboNotPurchased.Size = new System.Drawing.Size(121, 23);
            this.comboNotPurchased.TabIndex = 68;
            this.comboNotPurchased.Text = "No Limitation";
            this.comboNotPurchased.SelectedIndexChanged += new System.EventHandler(this.comboNotPurchased_SelectedIndexChanged);
            // 
            // comboLetters
            // 
            this.comboLetters.Items.AddRange(new object[] {
            "No Limitation",
            "Letters"});
            this.comboLetters.Location = new System.Drawing.Point(228, 127);
            this.comboLetters.Name = "comboLetters";
            this.comboLetters.Size = new System.Drawing.Size(121, 23);
            this.comboLetters.TabIndex = 67;
            this.comboLetters.Text = "No Limitation";
            this.comboLetters.SelectedIndexChanged += new System.EventHandler(this.comboLetters_SelectedIndexChanged);
            // 
            // comboNoLetters
            // 
            this.comboNoLetters.Items.AddRange(new object[] {
            "No Limitation",
            "The Letter"});
            this.comboNoLetters.Location = new System.Drawing.Point(228, 159);
            this.comboNoLetters.Name = "comboNoLetters";
            this.comboNoLetters.Size = new System.Drawing.Size(121, 23);
            this.comboNoLetters.TabIndex = 66;
            this.comboNoLetters.Text = "No Limitation";
            this.comboNoLetters.SelectedIndexChanged += new System.EventHandler(this.comboNoLetters_SelectedIndexChanged);
            // 
            // comboAccountCodes
            // 
            this.comboAccountCodes.Items.AddRange(new object[] {
            "No Limitation",
            "Account Code Set"});
            this.comboAccountCodes.Location = new System.Drawing.Point(228, 191);
            this.comboAccountCodes.Name = "comboAccountCodes";
            this.comboAccountCodes.Size = new System.Drawing.Size(121, 23);
            this.comboAccountCodes.TabIndex = 65;
            this.comboAccountCodes.Text = "No Limitation";
            this.comboAccountCodes.SelectedIndexChanged += new System.EventHandler(this.comboAccountCodes_SelectedIndexChanged);
            // 
            // ComboAge
            // 
            this.ComboAge.Items.AddRange(new object[] {
            "No Limitation",
            "Between"});
            this.ComboAge.Location = new System.Drawing.Point(228, 223);
            this.ComboAge.Name = "ComboAge";
            this.ComboAge.Size = new System.Drawing.Size(121, 23);
            this.ComboAge.TabIndex = 64;
            this.ComboAge.Text = "No Limitation";
            this.ComboAge.SelectedIndexChanged += new System.EventHandler(this.ComboAge_SelectedIndexChanged);
            // 
            // comboNoCustCode
            // 
            this.comboNoCustCode.Items.AddRange(new object[] {
            "No Limitation",
            "Customer Code Set"});
            this.comboNoCustCode.Location = new System.Drawing.Point(228, 263);
            this.comboNoCustCode.Name = "comboNoCustCode";
            this.comboNoCustCode.Size = new System.Drawing.Size(121, 23);
            this.comboNoCustCode.TabIndex = 63;
            this.comboNoCustCode.Text = "No Limitation";
            this.comboNoCustCode.SelectedIndexChanged += new System.EventHandler(this.comboNoCustCode_SelectedIndexChanged);
            // 
            // comboNoAccountCode
            // 
            this.comboNoAccountCode.Items.AddRange(new object[] {
            "No Limitation",
            "Account Code Set"});
            this.comboNoAccountCode.Location = new System.Drawing.Point(228, 303);
            this.comboNoAccountCode.Name = "comboNoAccountCode";
            this.comboNoAccountCode.Size = new System.Drawing.Size(121, 23);
            this.comboNoAccountCode.TabIndex = 62;
            this.comboNoAccountCode.Text = "No Limitation";
            this.comboNoAccountCode.SelectedIndexChanged += new System.EventHandler(this.comboNoAccountCode_SelectedIndexChanged);
            // 
            // comboPurchased
            // 
            this.comboPurchased.Items.AddRange(new object[] {
            "No Limitation",
            "Items in Categories",
            "Items",
            "Items beginning with"});
            this.comboPurchased.Location = new System.Drawing.Point(228, 63);
            this.comboPurchased.Name = "comboPurchased";
            this.comboPurchased.Size = new System.Drawing.Size(121, 23);
            this.comboPurchased.TabIndex = 61;
            this.comboPurchased.Text = "No Limitation";
            this.comboPurchased.SelectedIndexChanged += new System.EventHandler(this.comboPurchased_SelectedIndexChanged);
            // 
            // comboCustCodes
            // 
            this.comboCustCodes.Items.AddRange(new object[] {
            "No Limitation",
            "Customer Code Set"});
            this.comboCustCodes.Location = new System.Drawing.Point(228, 23);
            this.comboCustCodes.Name = "comboCustCodes";
            this.comboCustCodes.Size = new System.Drawing.Size(121, 23);
            this.comboCustCodes.TabIndex = 60;
            this.comboCustCodes.Text = "No Limitation";
            this.comboCustCodes.SelectedIndexChanged += new System.EventHandler(this.comboCustCodes_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(4, 335);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 59;
            this.label1.Text = "Limit Branches to";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(4, 367);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(100, 23);
            this.label10.TabIndex = 58;
            this.label10.Text = "Limit Arrears to:";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(4, 127);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(200, 23);
            this.label12.TabIndex = 57;
            this.label12.Text = "Limit letters to";
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(4, 95);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(232, 23);
            this.label13.TabIndex = 56;
            this.label13.Text = "Limit to Customers who have not Purchased";
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(4, 263);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(216, 23);
            this.label14.TabIndex = 55;
            this.label14.Text = "Exclude Customers with Customer Code:";
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(4, 223);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(152, 23);
            this.label15.TabIndex = 54;
            this.label15.Text = "Limit Customer Age";
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(4, 191);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(160, 23);
            this.label16.TabIndex = 53;
            this.label16.Text = "Limit Account Codes to";
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(4, 63);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(208, 23);
            this.label17.TabIndex = 52;
            this.label17.Text = "Limit to Customers who have purchased";
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(4, 303);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(208, 23);
            this.label18.TabIndex = 51;
            this.label18.Text = "Exclude Customers with Account Code:";
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(4, 159);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(200, 23);
            this.label19.TabIndex = 50;
            this.label19.Text = "Exclude customers who have received";
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(4, 23);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(152, 23);
            this.label20.TabIndex = 49;
            this.label20.Text = "Limit Customer Codes to:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkSpecial);
            this.groupBox1.Controls.Add(this.checkCash);
            this.groupBox1.Controls.Add(this.checkHP);
            this.groupBox1.Location = new System.Drawing.Point(496, 254);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(236, 49);
            this.groupBox1.TabIndex = 88;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Include";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // checkSpecial
            // 
            this.checkSpecial.Location = new System.Drawing.Point(166, 16);
            this.checkSpecial.Name = "checkSpecial";
            this.checkSpecial.Size = new System.Drawing.Size(64, 24);
            this.checkSpecial.TabIndex = 40;
            this.checkSpecial.Text = "Special";
            this.checkSpecial.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkCash
            // 
            this.checkCash.Location = new System.Drawing.Point(86, 16);
            this.checkCash.Name = "checkCash";
            this.checkCash.Size = new System.Drawing.Size(58, 24);
            this.checkCash.TabIndex = 38;
            this.checkCash.Text = "Cash";
            this.checkCash.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // checkHP
            // 
            this.checkHP.Location = new System.Drawing.Point(16, 16);
            this.checkHP.Name = "checkHP";
            this.checkHP.Size = new System.Drawing.Size(48, 24);
            this.checkHP.TabIndex = 87;
            this.checkHP.Text = "HP";
            this.checkHP.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioValues);
            this.groupBox2.Controls.Add(this.radioQuantities);
            this.groupBox2.Controls.Add(this.radioNeither);
            this.groupBox2.Location = new System.Drawing.Point(496, 309);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(237, 49);
            this.groupBox2.TabIndex = 92;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Display";
            // 
            // radioValues
            // 
            this.radioValues.Location = new System.Drawing.Point(16, 16);
            this.radioValues.Name = "radioValues";
            this.radioValues.Size = new System.Drawing.Size(64, 24);
            this.radioValues.TabIndex = 90;
            this.radioValues.Text = "Values";
            this.radioValues.CheckedChanged += new System.EventHandler(this.radioValues_CheckedChanged);
            // 
            // radioQuantities
            // 
            this.radioQuantities.Location = new System.Drawing.Point(86, 16);
            this.radioQuantities.Name = "radioQuantities";
            this.radioQuantities.Size = new System.Drawing.Size(80, 24);
            this.radioQuantities.TabIndex = 89;
            this.radioQuantities.Text = "Quantities";
            this.radioQuantities.CheckedChanged += new System.EventHandler(this.radioValues_CheckedChanged);
            // 
            // radioNeither
            // 
            this.radioNeither.Checked = true;
            this.radioNeither.Location = new System.Drawing.Point(166, 16);
            this.radioNeither.Name = "radioNeither";
            this.radioNeither.Size = new System.Drawing.Size(61, 24);
            this.radioNeither.TabIndex = 91;
            this.radioNeither.TabStop = true;
            this.radioNeither.Text = "Neither";
            this.radioNeither.CheckedChanged += new System.EventHandler(this.radioValues_CheckedChanged);
            // 
            // groupOrders
            // 
            this.groupOrders.Controls.Add(this.radioOrders);
            this.groupOrders.Controls.Add(this.radioDelivery);
            this.groupOrders.Location = new System.Drawing.Point(496, 364);
            this.groupOrders.Name = "groupOrders";
            this.groupOrders.Size = new System.Drawing.Size(236, 49);
            this.groupOrders.TabIndex = 98;
            this.groupOrders.TabStop = false;
            this.groupOrders.Text = "Figures Based on";
            this.groupOrders.Enter += new System.EventHandler(this.groupOrders_Enter);
            // 
            // radioOrders
            // 
            this.radioOrders.Location = new System.Drawing.Point(16, 18);
            this.radioOrders.Name = "radioOrders";
            this.radioOrders.Size = new System.Drawing.Size(78, 24);
            this.radioOrders.TabIndex = 97;
            this.radioOrders.Text = "Orders";
            this.radioOrders.CheckedChanged += new System.EventHandler(this.radioOrders_CheckedChanged);
            // 
            // radioDelivery
            // 
            this.radioDelivery.Checked = true;
            this.radioDelivery.Location = new System.Drawing.Point(116, 18);
            this.radioDelivery.Name = "radioDelivery";
            this.radioDelivery.Size = new System.Drawing.Size(72, 24);
            this.radioDelivery.TabIndex = 96;
            this.radioDelivery.TabStop = true;
            this.radioDelivery.Text = "Deliveries";
            this.radioDelivery.CheckedChanged += new System.EventHandler(this.radioDelivery_CheckedChanged);
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.FileOpen});
            this.menuFile.Text = "&File";
            // 
            // FileOpen
            // 
            this.FileOpen.Description = "Open Report Query";
            this.FileOpen.Text = "Open Report Query";
            this.FileOpen.Click += new System.EventHandler(this.FileOpen_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // CustomerMailing
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 494);
            this.Controls.Add(this.tcAllocated);
            this.Name = "CustomerMailing";
            this.Text = "Retrieve Customers Based on Criteria";
            this.tcAllocated.ResumeLayout(false);
            this.tpResult.ResumeLayout(false);
            this.tpResult.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
            this.tpQuery.ResumeLayout(false);
            this.tpQuery.PerformLayout();
            this.groupDatabase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.OldUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.YoungUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HighestEverUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CurrStatusUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ArrearsUpDown)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupOrders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        //private void label23_Click(object sender, System.EventArgs e)
        //{

        //}

        //private void drpEmployeeBranch_SelectedIndexChanged(object sender, System.EventArgs e)
        //{

        //}

        //private void label19_Click(object sender, System.EventArgs e)
        //{

        //}

        //private void drpEmployeeTypes_SelectedIndexChanged(object sender, System.EventArgs e)
        //{

        //}

        //private void label17_Click(object sender, System.EventArgs e)
        //{

        //}

        //private void drpEmpName_SelectedIndexChanged(object sender, System.EventArgs e)
        //{

        //}

        //private void drpAcctAllocation_SelectedIndexChanged(object sender, System.EventArgs e)
        //{

        //}

        private void tcAllocated_SelectionChanged(object sender, System.EventArgs e)
        {

        }

        //private void tabQuery_Click(object sender, System.EventArgs e)
        //{

        //}

        private void comboPurchased_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (fileopen == false) //dont kick this off if opening an existing report
            {
                if (comboPurchased.SelectedIndex == 1)//categories
                {
                    SetSelection selection = new SetSelection("Item Categories", 45, 0, 64, this.textPurchased, TN.ProductCategories, "", false);
                    selection.FormRoot = this.FormRoot;
                    selection.FormParent = this;
                    selection.ShowDialog(this);

                }
                if (comboPurchased.SelectedIndex == 2)//Items 
                {
                    SetSelection selection = new SetSelection("Items", 45, 0, 64, this.textPurchased, "Items", "", false);
                    selection.FormRoot = this.FormRoot;
                    selection.FormParent = this;
                    selection.ShowDialog(this);
                }
                if (comboPurchased.SelectedIndex == 3)//Item Likes
                {
                    SetSelection selection = new SetSelection("Items", 45, 0, 64, this.textPurchased, "ItemLikes", "", false);
                    selection.FormRoot = this.FormRoot;
                    selection.FormParent = this;
                    selection.ShowDialog(this);
                }
            }
            if (comboPurchased.SelectedIndex == 0)
            {
                dtCustPurchaseStart.Enabled = false;
                dtPurchaseEnd.Enabled = false;
                textPurchased.Text = "nr";
            }
            else
            {
                dtCustPurchaseStart.Enabled = true;
                dtPurchaseEnd.Enabled = true;
            }


        }

        private void comboNotPurchased_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (fileopen == false) //dont kick this off if opening an existing report
            {
                if (comboNotPurchased.SelectedIndex == 1)//categories
                {
                    SetSelection selection = new SetSelection("Item Categories", 45, 195, 64, this.textNoPurchased, TN.ProductCategories, "", false);
                    selection.FormRoot = this.FormRoot;
                    selection.FormParent = this;
                    selection.ShowDialog(this);

                }
                if (comboNotPurchased.SelectedIndex == 2)//Items 
                {
                    SetSelection selection = new SetSelection("Items", 45, 195, 64, this.textNoPurchased, "Items", "", false);
                    selection.FormRoot = this.FormRoot;
                    selection.FormParent = this;
                    selection.ShowDialog(this);
                }
                if (comboNotPurchased.SelectedIndex == 3)//Item Likes
                {
                    SetSelection selection = new SetSelection("Items", 45, 195, 64, this.textNoPurchased, "ItemLikes", "", false);
                    selection.FormRoot = this.FormRoot;
                    selection.FormParent = this;
                    selection.ShowDialog(this);
                }
            }
            if (comboNotPurchased.SelectedIndex == 0)
            {

                dtNoPurchaseStart.Enabled = false;
                dtNoPurchaseEnd.Enabled = false;
                textNoPurchased.Text = "nr";
            }
            else
            {
                dtNoPurchaseStart.Enabled = true;
                dtNoPurchaseEnd.Enabled = true;
            }


        }

        private void comboNoLetters_SelectedIndexChanged(object sender, System.EventArgs e)
        {


            if (comboNoLetters.SelectedIndex == 1)
            {
                if (fileopen == false) //dont kick this off if opening an existing report
                {
                    SetSelection selection = new SetSelection("Letter Codes", 45, 195, 64, this.textNoLetter, "LT1", TN.Letter, false);
                    selection.FormRoot = this.FormRoot;
                    selection.FormParent = this;
                    selection.ShowDialog(this);
                }
                dtNoLettersStart.Enabled = true;
                dtNoLettersEnd.Enabled = true;
            }
            else
            {
                dtNoLettersStart.Enabled = false;
                dtNoLettersEnd.Enabled = false;
                textNoLetter.Text = "nr";
            }


        }



        private void comboBox2_SelectedIndexChanged(object sender, System.EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void tpQuery_PropertyChanged(Crownwood.Magic.Controls.TabPage page, Crownwood.Magic.Controls.TabPage.Property prop, object oldValue)
        {

        }

        private void groupBox1_Enter(object sender, System.EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, System.EventArgs e)
        {

        }

        private void checkBox4_CheckedChanged(object sender, System.EventArgs e)
        {

        }

        private void groupOrders_Enter(object sender, System.EventArgs e)
        {

        }

        private void radioDelivery_CheckedChanged(object sender, System.EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
        {

        }

        private void radioOrders_CheckedChanged(object sender, System.EventArgs e)
        {

        }

        private void textNoPurchaseSet_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void textLetterSet_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void textPurchaseSet_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void textBranch_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void textNoAccountCode_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void textNoLetter_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void textAccountCodes_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void dateTimePicker13_ValueChanged(object sender, System.EventArgs e)
        {

        }

        private void dTNoPurchaseStart_ValueChanged(object sender, System.EventArgs e)
        {

        }

        private void dtNoLettersStart_ValueChanged(object sender, System.EventArgs e)
        {


        }

        private void comboBranches_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (fileopen == false) //dont kick this off if opening an existing report
            {
                if (comboBranches.SelectedIndex == 1) //selected branch set
                {
                    SetSelection selection = new SetSelection("Branches", 45, 195, 64, this.textBranch, TN.TNameBranch, TN.BranchNumber, false);
                    selection.FormRoot = this.FormRoot;
                    selection.FormParent = this;
                    selection.ShowDialog(this);
                }
            }
        }

        private void comboNoAccountCode_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (fileopen == false) //dont kick this off if opening an existing report
            {
                if (comboNoAccountCode.SelectedIndex == 1) //selected branch set
                {
                    SetSelection selection = new SetSelection("Account Codes", 45, 195, 64, this.textNoAccountCode, "AC1", TN.AccountCodes, false);
                    selection.FormRoot = this.FormRoot;
                    selection.FormParent = this;
                    selection.ShowDialog(this);
                }
                else
                    textNoAccountCode.Text = "nr";
            }
        }

        private void comboAccountCodes_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (fileopen == false) //dont kick this off if opening an existing report
            {
                if (comboAccountCodes.SelectedIndex == 1)
                {

                    SetSelection selection = new SetSelection("Account Codes", 45, 195, 64, this.textAccountCode, "AC1", TN.AccountCodes, false);
                    selection.FormRoot = this.FormRoot;
                    selection.FormParent = this;
                    selection.ShowDialog(this);
                }
                else
                    textAccountCode.Text = "nr";
            }
        }

        private void comboCustCodes_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (fileopen == false) //dont kick this off if opening an existing report
            {
                if (comboCustCodes.SelectedIndex == 1)
                {
                    SetSelection selection = new SetSelection("Customer Codes", 45, 195, 64, this.textCustomerCode, "CC1", TN.CustomerCodes, false);
                    selection.FormRoot = this.FormRoot;
                    selection.FormParent = this;
                    selection.ShowDialog(this);
                }
                else
                    textCustomerCode.Text = "nr";
            }
        }

        private void comboLetters_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (comboLetters.SelectedIndex == 1)
            {
                if (fileopen == false) //dont kick this off if opening an existing report
                {
                    SetSelection selection = new SetSelection("Letter Codes", 45, 195, 64, this.textLetter, "LT1", TN.Letter, false);
                    selection.FormRoot = this.FormRoot;
                    selection.FormParent = this;
                    selection.ShowDialog(this);
                }
                dtLettersStart.Enabled = true;
                dtLettersEnd.Enabled = true;
            }
            else
            {
                dtLettersStart.Enabled = false;
                dtLettersEnd.Enabled = false;
                textLetter.Text = "nr";
            }



        }

        private void comboNoCustCode_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (fileopen == false) //dont kick this off if opening an existing report
            {
                if (comboNoCustCode.SelectedIndex == 1)
                {
                    SetSelection selection = new SetSelection("Customer Codes", 45, 195, 64, this.textNoCustomerCode, "CC1", TN.CustomerCodes, false);
                    selection.FormRoot = this.FormRoot;
                    selection.FormParent = this;
                    selection.ShowDialog(this);
                }
                else
                    textNoCustomerCode.Text = "nr";
            }
        }

        //private void comboBox6_SelectedIndexChanged(object sender, System.EventArgs e)
        //{

        //}
        private void RunOrSave(bool DoRun)
        {
            Function = "Load Customer Mailing Query";
            try
            {
                string arrearsRestriction = "nr";

                //if (comboArrears.DisplayMember.ToString() == "Arrears <")
                if (comboArrears.SelectedIndex == 1)
                {
                    arrearsRestriction = "<";
                }
                //if (comboArrears.DisplayMember.ToString() == "Arrears/Instalment <")
                if (comboArrears.SelectedIndex == 2)
                {
                    arrearsRestriction = "ai<";
                }
                string accounttypes = "";
                if (checkCash.Checked == true)
                    accounttypes = "C";
                if (checkHP.Checked == true)
                    accounttypes = "H";
                if (checkSpecial.Checked == true)
                    accounttypes = "S";
                if (checkCash.Checked == true && checkSpecial.Checked == true && checkHP.Checked == true)
                    accounttypes = "A";
                if (checkCash.Checked == true && checkSpecial.Checked != true && checkHP.Checked == true)
                    accounttypes = "T";
                if (checkCash.Checked == false && checkSpecial.Checked == true && checkHP.Checked == true)
                    accounttypes = "D";  //not cash
                if (checkCash.Checked == true && checkSpecial.Checked == true && checkHP.Checked == false)
                    accounttypes = "I"; // not HP

                string itemset = "nr";
                string itemcatset = "nr";
                string itemstartswith = "nr";

                string noitemset = "nr";
                string noitemcatset = "nr";
                string noitemstartswith = "nr";

                if (comboPurchased.SelectedIndex == 1) //Items in categories
                {
                    itemset = "nr";
                    itemcatset = textPurchased.Text;
                    itemstartswith = "nr";
                }
                if (comboPurchased.SelectedIndex == 2) //spefic items
                {
                    itemset = textPurchased.Text;
                    itemcatset = "nr";
                    itemstartswith = "nr";
                }
                if (comboPurchased.SelectedIndex == 3) //item starts with
                {
                    itemset = "nr";
                    itemcatset = "nr";
                    itemstartswith = textNoPurchased.Text;
                }

                if (comboNotPurchased.SelectedIndex == 1) //Items in categories
                {
                    noitemset = "nr";
                    noitemcatset = textNoPurchased.Text;
                    noitemstartswith = "nr";
                }
                if (comboNotPurchased.SelectedIndex == 2) //spefic items
                {
                    noitemset = textNoPurchased.Text;
                    noitemcatset = "nr";
                    noitemstartswith = "nr";
                }
                if (comboNotPurchased.SelectedIndex == 3) //item starts with
                {
                    noitemset = "nr";
                    noitemcatset = "nr";
                    noitemstartswith = textNoPurchased.Text;
                }
                int young = 0; int old = 0;
                if (ComboAge.SelectedIndex == 1)
                {
                    young = Convert.ToInt32(YoungUpDown.Value); old = Convert.ToInt32(OldUpDown.Value);
                }
                else
                {
                    young = 0; old = 99;    // default old to 99 ref dn_CustomerMailing.sql
                }
                string totals = "N";     // Neither checked - default
                if (radioValues.Checked == true)
                    totals = "V";
                if (radioQuantities.Checked == true)
                    totals = "Q";

                string branch = textBranch.Text;

                string Error = "";
                //accounts.Tables.Clear();
                if (DoRun == true)
                {
                    // set Status text to processing
                    ((MainForm)this.FormRoot).statusBar1.Text = "Selection criteria processing... ";
                    accounts = CustomerManager.CustomerMailingQuery(
                        textCustomerCode.Text,
                          textNoCustomerCode.Text,
                          textAccountCode.Text,
                          textNoAccountCode.Text,
                        //comboArrears.DisplayMember.ToString(),
                          arrearsRestriction,
                          Convert.ToDouble(ArrearsUpDown.Value),
                          CurrStatusUpDown.Value.ToString(),
                          HighestEverUpDown.Value.ToString(),
                          branch,
                          accounttypes,
                          itemset,
                          dtCustPurchaseStart.Value,
                          dtPurchaseEnd.Value,
                          noitemset,
                          dtNoPurchaseStart.Value,
                          dtNoPurchaseEnd.Value,
                          itemcatset,
                           dtCustPurchaseStart.Value,
                          dtPurchaseEnd.Value,
                          noitemcatset,
                          dtNoPurchaseStart.Value,
                          dtNoPurchaseEnd.Value,
                           Convert.ToInt16(radioDelivery.Checked),
                           itemstartswith,
                           dtCustPurchaseStart.Value,
                          dtPurchaseEnd.Value,
                          noitemstartswith,
                          dtNoPurchaseStart.Value,
                          dtNoPurchaseEnd.Value,
                          textNoLetter.Text,
                          dtNoLettersStart.Value,
                          dtNoLettersEnd.Value,
                          textLetter.Text,
                          dtLettersStart.Value,
                          dtLettersEnd.Value,
                          young, old,
                          totals,
                          "",
                          Convert.ToInt16(checkExcludeCancel.Checked),
                          Convert.ToInt16(radioDBLive.Checked),
                          out Error);

                    Function = "End of Load Customer Mailing Query";
                    DataView acctView = new DataView(accounts.Tables[0]);

                    if (acctView.Count > 0)
                    {
                        ((MainForm)this.FormRoot).statusBar1.Text = acctView.Count.ToString() + " rows returned";
                        statusText = ((MainForm)this.FormRoot).statusBar1.Text;
                        dgAccounts.DataSource = acctView;
                        //acctView.d
                        dgAccounts.TableStyles.Clear();

                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        DataTable dt = accounts.Tables[0];
                        tabStyle.MappingName = dt.TableName;

                        dgAccounts.TableStyles.Add(tabStyle);

                        /* Set the table style according to the user's preference  */

                        tabStyle.GridColumnStyles[CN.FirstName].Width = 120; // 145
                        tabStyle.GridColumnStyles[CN.FirstName].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.FirstName].HeaderText = GetResource("T_FIRSTNAME");

                        tabStyle.GridColumnStyles[CN.Title].Width = 49;
                        tabStyle.GridColumnStyles[CN.Title].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.Title].HeaderText = GetResource("T_TITLE");

                        tabStyle.GridColumnStyles[CN.acctno].Width = 88;    // 92
                        tabStyle.GridColumnStyles[CN.acctno].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.acctno].HeaderText = GetResource("T_ACCOUNTNO");

                        tabStyle.GridColumnStyles[CN.name].Width = 120;     // 140
                        tabStyle.GridColumnStyles[CN.name].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.name].HeaderText = GetResource("T_NAME");

                        tabStyle.GridColumnStyles[CN.cusaddr1].Width = 150;     // 200
                        tabStyle.GridColumnStyles[CN.cusaddr1].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.cusaddr1].HeaderText = GetResource("T_ADDRESS1");

                        tabStyle.GridColumnStyles[CN.cusaddr2].Width = 150;     // 200
                        tabStyle.GridColumnStyles[CN.cusaddr2].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.cusaddr2].HeaderText = GetResource("T_ADDRESS2");

                        tabStyle.GridColumnStyles[CN.cusaddr3].Width = 100;     // 200
                        tabStyle.GridColumnStyles[CN.cusaddr3].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.cusaddr3].HeaderText = GetResource("T_ADDRESS3");

                        tabStyle.GridColumnStyles[CN.cuspocode].Width = 70;     // 100
                        tabStyle.GridColumnStyles[CN.cuspocode].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.cuspocode].HeaderText = GetResource("T_POSTCODE");

                        tabStyle.GridColumnStyles[CN.CustID].Width = 80;        // 100
                        tabStyle.GridColumnStyles[CN.CustID].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.CustID].HeaderText = GetResource("T_CUSTID");
                        // available spend
                        tabStyle.GridColumnStyles[CN.AvailableSpend].Width = 100;
                        tabStyle.GridColumnStyles[CN.AvailableSpend].Alignment = HorizontalAlignment.Right;
                        tabStyle.GridColumnStyles[CN.AvailableSpend].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.AvailableSpend].HeaderText = GetResource("T_AVAILABLESPEND");
                        // RF credit limit
                        tabStyle.GridColumnStyles[CN.RFCreditLimit].Width = 80;     // 100
                        tabStyle.GridColumnStyles[CN.RFCreditLimit].Alignment = HorizontalAlignment.Right;
                        tabStyle.GridColumnStyles[CN.RFCreditLimit].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.RFCreditLimit].HeaderText = GetResource("T_RFCREDITLIMIT");
                        // TelNo
                        tabStyle.GridColumnStyles[CN.TelNo].Width = 90;
                        tabStyle.GridColumnStyles[CN.TelNo].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.TelNo].HeaderText = GetResource("T_TELNO");
                        // email
                        tabStyle.GridColumnStyles[CN.Email].Width = 80;     // 100
                        tabStyle.GridColumnStyles[CN.Email].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.Email].HeaderText = GetResource("T_EMAIL");

                        // here add up the totals if the user has selected quantity or value and display at the bottom
                        decimal totalMoney = 0;
                        string varMoney = "";
                        int quantity = 0;
                        textTotal.Visible = false;
                        labelTotal.Visible = false;
                        if (totals == "V" || totals == "N")
                            tabStyle.GridColumnStyles[CN.Quantity].Width = 0;
                        else
                        {
                            foreach (DataRowView row in (DataView)dgAccounts.DataSource)
                            {
                                quantity = quantity + ((Int32)row[CN.Quantity]);
                            }

                            textTotal.Visible = true;
                            textTotal.Text = Convert.ToString(quantity);
                            labelTotal.Visible = true;
                            labelTotal.Text = GetResource("T_QUANTITY");
                            tabStyle.GridColumnStyles[CN.Quantity].Width = 55;
                        }

                        if (totals == "Q" || totals == "N")

                            tabStyle.GridColumnStyles[CN.Value].Width = 0;
                        else
                        {
                            foreach (DataRowView row in (DataView)dgAccounts.DataSource)
                            {
                                totalMoney = totalMoney + ((decimal)row[CN.Value]);
                            }
                            varMoney = Convert.ToString(totalMoney);
                            textTotal.Text = Decimal.Parse(varMoney).ToString("c");
                            labelTotal.Text = GetResource("T_VALUE");
                            textTotal.Visible = true;
                            labelTotal.Visible = true;
                            tabStyle.GridColumnStyles[CN.Value].Width = 85;

                        }




                        // tcAllocated.SelectedTab = tpResult;


                        // else
                        //     ((MainForm)this.FormRoot).statusBar1.Text = "No accounts returned";

                        // display results
                        tcAllocated.SelectedTab = tpResult;
                    }
                    else
                        ((MainForm)this.FormRoot).statusBar1.Text = "No accounts returned";

                }
                else //Saving only
                {
                    CustomerManager.CustomerMailingQuerySave(Credential.UserId,
                        DateTime.Now, QueryName,
                        textCustomerCode.Text,
                        textNoCustomerCode.Text,
                        textAccountCode.Text,
                        textNoAccountCode.Text,
                        //comboArrears.DisplayMember.ToString(),
                        arrearsRestriction,
                          Convert.ToDouble(ArrearsUpDown.Value),
                          CurrStatusUpDown.Value.ToString(),
                          HighestEverUpDown.Value.ToString(),
                          branch,
                          accounttypes,
                          itemset,
                          dtCustPurchaseStart.Value,
                          dtPurchaseEnd.Value,
                          noitemset,
                          dtNoPurchaseStart.Value,
                          dtNoPurchaseEnd.Value,
                          itemcatset,
                           dtCustPurchaseStart.Value,
                          dtPurchaseEnd.Value,
                          noitemcatset,
                          dtNoPurchaseStart.Value,
                          dtNoPurchaseEnd.Value,
                           Convert.ToInt16(radioDelivery.Checked),
                           itemstartswith,
                           dtCustPurchaseStart.Value,
                          dtPurchaseEnd.Value,
                          noitemstartswith,
                          dtNoPurchaseStart.Value,
                          dtNoPurchaseEnd.Value,
                          textNoLetter.Text,
                          dtNoLettersStart.Value,
                          dtNoLettersEnd.Value,
                          textLetter.Text,
                          dtLettersStart.Value,
                          dtLettersEnd.Value,
                          young, old,
                              totals, "", Convert.ToInt16(checkExcludeCancel.Checked), out Error);

                    ((MainForm)this.FormRoot).statusBar1.Text = "Query Saved as " + QueryName;
                }



            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            // enable generate buttons
            btnLetter.Enabled = true;
            btnExcel.Enabled = true;
            chbExcel.Enabled = true;
            Wait();
            RunOrSave(true);


        }

        private void labelCurrStatus_Click(object sender, EventArgs e)
        {

        }

        private void radioValues_CheckedChanged(object sender, EventArgs e)
        {
            if (radioValues.Checked == true || radioQuantities.Checked == true)
            {
                radioOrders.Enabled = true;
                radioDelivery.Enabled = true;
                groupOrders.Enabled = true;
            }
            else // sorry should always be enabled
            {
                radioOrders.Enabled = true;
                radioDelivery.Enabled = true;
                groupOrders.Enabled = true;
            }

        }

        private void ComboAge_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboAge.SelectedIndex > 0)
            {
                YoungUpDown.Enabled = true;
                OldUpDown.Enabled = true;
                labeland.Enabled = true;
            }
            else
            {
                YoungUpDown.Enabled = false;
                OldUpDown.Enabled = false;
                labeland.Enabled = false;
                YoungUpDown.Value = 0;
                OldUpDown.Value = 0;
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            // disable generate buttons
            btnLetter.Enabled = false;
            btnExcel.Enabled = false;
            chbExcel.Enabled = false;
            FileInfo fi = null;
            try
            {
                Function = "LaunchExcel";
                Wait();
                ((MainForm)this.FormRoot).statusBar1.Text = " Launch Excel in progress...";
                /* save the current data grid contents to a CSV */
                string comma = ",";

                if (dgAccounts.CurrentRowIndex >= 0)
                {
                    DataView dv = (DataView)dgAccounts.DataSource;

                    Assembly asm = Assembly.GetExecutingAssembly();
                    string path = "C:\\Temp\\Accounts.csv";
                    fi = new FileInfo(path);
                    if (fi.Exists)
                    {
                        fi.Delete();
                    }
                    FileStream fs = fi.OpenWrite();

                    string line = CN.AccountNo + comma + //CN.acctno + comma +
                        CN.CustomerID + comma +
                        CN.Title + comma +
                        CN.FirstName + comma +
                        CN.Name + comma +
                        //CN.cusaddr1 + comma +
                        //CN.cusaddr2 + comma +
                        //CN.cusaddr3 + comma +
                        //CN.cuspocode + comma +
                        CN.Address1 + comma +
                        CN.Address2 + comma +
                        CN.Address3 + comma +
                        CN.PostCode + comma +
                        CN.Email + comma +
                        //CN.RFAvailable + comma +
                        CN.AvailableSpend + comma +
                        CN.RFCreditLimit + comma +
                        CN.Quantity + comma +
                        CN.Value +
                        /*  select custid,title ,name, firstname,
  cusaddr1 , cusaddr2 , cusaddr3, cuspocode,  
  email, quantity ,   value , acctno,
  rfcreditlimit ,   availablespend        
  from #customers
*/
                         Environment.NewLine + Environment.NewLine;
                    byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
                    fs.Write(blob, 0, blob.Length);

                    foreach (DataRowView row in dv)
                    {
                        line = "'" + row[CN.acctno] + "'" + comma +
                        row[CN.CustID] + comma +
                        row[CN.Title] + comma +
                        row[CN.FirstName] + comma +
                        row[CN.name] + comma +
                        row[CN.cusaddr1] + comma +
                        row[CN.cusaddr2] + comma +
                        row[CN.cusaddr3] + comma +
                        row[CN.cuspocode] + comma +
                        row[CN.Email] + comma +
                            //row[CN.RFAvailable] + comma +
                        row[CN.AvailableSpend] + comma +
                        row[CN.RFCreditLimit] + comma +
                        row[CN.Quantity] + comma +
                        row[CN.Value] + Environment.NewLine;

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

                        ((MainForm)this.FormRoot).statusBar1.Text = statusText;
                    }
                    catch (COMException)
                    {
                        /*change back slashes to forward slashes so the path doesn't
                         * get split into multiple lines */
                        ShowInfo("M_EXCELNOTFOUND", new object[] { path.Replace("\\", "/") });
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

        private void btnLetter_Click(object sender, EventArgs e)
        {
            errorProvider1.SetError(drpLetter, "");

            if (drpLetter.Items.Count > 0) //IP - 22/12/09 - UAT(946)
            {
                // disable generate buttons
                btnLetter.Enabled = false;
                btnExcel.Enabled = false;
                chbExcel.Enabled = false;

                DataView dv = (DataView)dgAccounts.DataSource;
                int counter = 0;
                DateTime LetterDate;
                string excelGen = "0";
                // set up letter date
                if (chbExcel.Checked)
                {
                    LetterDate = DateTime.Today;
                    excelGen = "1";
                }
                else
                {   // letter date will be updated in Letters & charges process
                    LetterDate = Convert.ToDateTime("1-jan-1910");
                }
                string letterCode = (string)drpLetter.SelectedItem;
                letterCode = letterCode.Substring(0, letterCode.IndexOf("-") - 1);
                string letterName = (string)drpLetter.SelectedItem;
                letterName = letterName.Substring(letterName.IndexOf("-") + 1,
                                letterName.Length - (letterName.IndexOf("-") + 1));
                ((MainForm)this.FormRoot).statusBar1.Text = letterName + " Letter generation in progress";
                Wait();
                foreach (DataRowView row in dv) //Write the letter to the Account
                {
                    AccountManager.AddLetterToAccount(row[CN.acctno].ToString(),
                        LetterDate,   //DateTime.Today, 
                        DateTime.Now.AddDays(7), letterCode, 0, excelGen);
                    counter++;
                }

                StopWait();
                // If Gen. immediately via excel - Launch Excel
                if (chbExcel.Checked)
                {
                    this.btnExcel_Click(sender, e);
                }

                ((MainForm)this.FormRoot).statusBar1.Text = Convert.ToString(counter) + " " +
                        letterName + " Letters Generated";
            }
            else
            {
                errorProvider1.SetError(drpLetter, GetResource("M_SETUPADDITIONALLETTERS")); //IP - 22/12/09 - UAT(946)
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {

            bool PopupReturn;

            ConfirmPopup confirmpopup = new ConfirmPopup("Enter Query Name", out QueryName, out PopupReturn);
            confirmpopup.textBox.Text = QueryName;
            confirmpopup.ShowDialog();
            //confirmpopup.`  
            PopupReturn = confirmpopup.Return;
            QueryName = confirmpopup.textBox.Text;
            if (PopupReturn)
                this.RunOrSave(false);

        }
        // retreive saved query
        private void FileOpen_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "GetandLoadCustomerMailingQuery";
                fileopen = true;
                CustomerMailingLoadQuery LoadQuery = new CustomerMailingLoadQuery(out QueryName);
                LoadQuery.ShowDialog();
                QueryName = LoadQuery._QueryName;

                string arrearsRestriction = "";


                string accounttypes = "";

                string itemset = "";
                string itemcatset = "";
                string itemstartswith = "";

                string noitemset = "";
                string noitemcatset = "";
                string noitemstartswith = "";

                int young = 0; int old = 0;
                string totals = "";

                string branch = "";
                DateTime dte, dateitemstart, dateitemend, datenopurchasestart, datenopurchaseend,
                    dte1, dte2, dte3, dte4, dte5, dte6, dte7, dte8, dateletterstart, dateletterend, datenoletterstart, datenoletterend;
                int Delivery = 0;
                int excludeCancellations = 1;
                double ArrearsUpDownValue;
                string currstatus, highestEverstatus;
                string textnoletter = "", textletter = "", customercode, nocustomercode, accountcode, noaccountcode, error;
                if (QueryName != "")    // Query selected (Cancel button not selected)
                {
                    CustomerManager.CustomerMailingQueryGet(Credential.UserId, QueryName, out dte, //3
                            out customercode, out nocustomercode, //5
                            out accountcode, out noaccountcode,
                            out arrearsRestriction, out ArrearsUpDownValue,//9
                            out currstatus, out highestEverstatus, out branch,//12
                            out accounttypes, out itemset,//14
                            out dateitemstart, out dateitemend,//16
                            out noitemset, out datenopurchasestart,
                            out datenopurchaseend, out itemcatset,//20
                            out dte1, out dte2,
                            out noitemcatset, out dte3,//24
                            out dte4, out Delivery,
                            out itemstartswith, out dte5, //28
                            out dte6, out noitemstartswith,
                            out dte7, out dte8,//32
                            out textnoletter, out datenoletterstart,
                            out datenoletterend, out textletter,//36
                            out dateletterstart, out dateletterend,
                            out young, out  old,//40
                            out totals, out error, out excludeCancellations); //42

                    ArrearsUpDown.Value = Convert.ToDecimal(ArrearsUpDownValue);
                    CurrStatusUpDown.Value = Convert.ToDecimal(currstatus);
                    HighestEverUpDown.Value = Convert.ToDecimal(highestEverstatus);
                    dtCustPurchaseStart.Value = dateitemstart;
                    dtPurchaseEnd.Value = dateitemend;
                    dtNoPurchaseStart.Value = datenopurchasestart;
                    dtNoPurchaseEnd.Value = datenopurchaseend;
                    textNoLetter.Text = textnoletter;
                    if (textnoletter != " " & textnoletter != "nr")
                        comboNoLetters.SelectedIndex = 1;

                    else
                        comboNoLetters.SelectedIndex = 0;

                    if (branch != " " && branch != "nr")
                    {
                        comboBranches.SelectedIndex = 1;
                        textBranch.Text = branch;
                    }
                    else
                    {
                        comboBranches.SelectedIndex = 0;
                        textBranch.Text = " ";
                    }

                    textNoCustomerCode.Text = nocustomercode;
                    if (nocustomercode != " " || nocustomercode != "nr")
                        comboNoCustCode.SelectedIndex = 1;
                    else
                        comboNoCustCode.SelectedIndex = 0;

                    textNoAccountCode.Text = noaccountcode;
                    if (noaccountcode != " " || noaccountcode != "nr")
                        comboNoAccountCode.SelectedIndex = 1;
                    else
                        comboNoAccountCode.SelectedIndex = 0;

                    // is this code duplicated from above????
                    if (textnoletter != " " || textnoletter != "nr")
                        comboNoLetters.SelectedIndex = 1;
                    else
                        comboNoLetters.SelectedIndex = 0;


                    dtNoLettersStart.Value = datenoletterstart;
                    dtNoLettersEnd.Value = datenoletterend;
                    textLetter.Text = textletter;
                    if (textletter != " " || textletter != "nr")
                        comboLetters.SelectedIndex = 1;
                    else
                        comboLetters.SelectedIndex = 0;

                    dtLettersStart.Value = dateletterstart;
                    dtLettersEnd.Value = dateletterend;

                    textCustomerCode.Text = customercode;
                    if (customercode != " " || customercode != "nr")
                        comboCustCodes.SelectedIndex = 1;
                    else
                        comboCustCodes.SelectedIndex = 0;

                    textAccountCode.Text = accountcode;
                    if (accountcode != " " || accountcode != "nr")
                        comboAccountCodes.SelectedIndex = 1;
                    else
                        comboAccountCodes.SelectedIndex = 0;

                    if (Delivery == 1)
                    {
                        radioDelivery.Checked = true;
                    }
                    else
                    {
                        radioDelivery.Checked = false;
                    }

                    if (excludeCancellations == 1)
                    {
                        checkExcludeCancel.Checked = true;
                    }
                    else
                    {
                        checkExcludeCancel.Checked = false;
                    }



                    if (arrearsRestriction == "<")
                    {
                        comboArrears.SelectedIndex = 1;
                    }
                    else if (arrearsRestriction == "ai<")
                    {
                        comboArrears.SelectedIndex = 2;
                    }
                    else
                        comboArrears.SelectedIndex = 0;


                    if (accounttypes == "C")
                    {
                        checkCash.Checked = true;
                        checkHP.Checked = false;
                        checkSpecial.Checked = false;
                    }
                    if (accounttypes == "H")
                    {
                        checkCash.Checked = false;
                        checkHP.Checked = true;
                        checkSpecial.Checked = false;
                    }
                    if (accounttypes == "S")
                    {
                        checkCash.Checked = false;
                        checkHP.Checked = false;
                        checkSpecial.Checked = true;
                    }
                    if (accounttypes == "A")
                    {
                        checkCash.Checked = true;
                        checkHP.Checked = true;
                        checkSpecial.Checked = true;
                    }
                    if (accounttypes == "T")
                    {
                        checkCash.Checked = true;
                        checkHP.Checked = true;
                        checkSpecial.Checked = false;
                    }
                    if (accounttypes == "D")
                    {
                        checkCash.Checked = false;
                        checkHP.Checked = true;
                        checkSpecial.Checked = true;
                    }
                    if (accounttypes == "I")
                    {
                        checkCash.Checked = true;
                        checkHP.Checked = false;
                        checkSpecial.Checked = true;
                    }
                    if (itemcatset != "nr")
                    {
                        comboPurchased.SelectedIndex = 1;
                        textPurchased.Text = itemcatset;
                    }
                    else if (itemset != "nr")
                    {
                        comboPurchased.SelectedIndex = 2;
                        textPurchased.Text = itemset;
                    }
                    else if (itemstartswith != "nr")
                    {
                        comboPurchased.SelectedIndex = 3;
                        textPurchased.Text = itemstartswith;
                    }
                    else
                    {
                        comboPurchased.SelectedIndex = 1;
                    }
                    // Items not purchased
                    if (noitemcatset != "nr")
                    {
                        comboNotPurchased.SelectedIndex = 2;
                        textNoPurchased.Text = noitemcatset;
                    }
                    else if (noitemset != "nr")
                    {
                        comboNotPurchased.SelectedIndex = 1;
                        textNoPurchased.Text = noitemset;
                    }
                    else if (noitemstartswith != "nr")
                    {
                        comboNotPurchased.SelectedIndex = 3;
                        textNoPurchased.Text = noitemstartswith;
                    }
                    else
                    {
                        comboNotPurchased.SelectedIndex = 0;
                    }
                    if (young > 0 || old > 0)
                    {
                        ComboAge.SelectedIndex = 1;
                        YoungUpDown.Value = Convert.ToDecimal(young);
                        OldUpDown.Value = Convert.ToDecimal(old);
                    }
                    else
                        ComboAge.SelectedIndex = 0;

                    if (totals == "V")
                    {
                        radioValues.Checked = true;
                        radioQuantities.Checked = false;
                        radioNeither.Checked = false;

                    }
                    else if (totals == "Q")
                    {
                        radioValues.Checked = false;
                        radioQuantities.Checked = true;
                        radioNeither.Checked = false;
                    }
                    else
                    {
                        radioValues.Checked = false;
                        radioQuantities.Checked = false;
                        radioNeither.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                fileopen = false;
            }




        }













    }
}
