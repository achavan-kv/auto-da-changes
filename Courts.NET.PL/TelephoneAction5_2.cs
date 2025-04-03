using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Crownwood.Magic.Menus;
using STL.Common;
using STL.Common.Constants.AccountHolders;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.ScreenModes;     //CR1084
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using STL.PL.Collections;
using Blue.Cosacs.Shared;
using System.Text;

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
    public class TelephoneAction5_2 : CommonForm
    {
        #region -- Designer Variables -------------------------------------------------------------
        private System.Windows.Forms.GroupBox groupBox3;
        private System.ComponentModel.IContainer components;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button btnLoadExtraInfo;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.TextBox txtActionValue;
        private System.Windows.Forms.ComboBox drpActionCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSave;
        private ErrorProviderExtended WarningProvider;
        private System.Windows.Forms.DateTimePicker dtDueDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox gbLastAction;
        private System.Windows.Forms.DateTimePicker dtLastActionDate;
        private System.Windows.Forms.TextBox txtLastActionCode;
        private System.Windows.Forms.GroupBox gbNewAction;
        private System.Windows.Forms.Label lbActionValue;
        private System.Windows.Forms.Label lbActionDate;
        private System.Windows.Forms.ComboBox drpReason;
        private System.Windows.Forms.Label lbReason;
        private Crownwood.Magic.Controls.TabControl tabAccounts;
        private Crownwood.Magic.Controls.TabPage tabAccountList;
        private DataGrid dgAccounts;
        private Label label2;
        private CheckBox chCycleNext;
        private Crownwood.Magic.Controls.TabPage tabAccountDetails;
        private Button button1;
        private Button button2;
        private Button button3;
        private GroupBox groupBox1;
        private TextBox textBox1;
        private Label label11;
        private Label label12;
        private Label label13;
        private Label label14;
        private TextBox textBox2;
        private TextBox textBox3;
        private Label label15;
        private TextBox textBox4;
        private TextBox textBox5;
        private Label label16;
        private TextBox textBox6;
        private Label label18;
        private TextBox textBox7;
        private TextBox textBox8;
        private Label label19;
        private TextBox textBox9;
        private Label label20;
        private TextBox textBox10;
        private Label label21;
        private Label label22;
        private TextBox textBox11;
        private TextBox textBox12;
        private TextBox textBox13;
        private Label label23;
        private Label label24;
        private TextBox textBox14;
        private Label label25;
        private Label label26;
        private TextBox textBox15;
        private Crownwood.Magic.Controls.TabControl tabSubAccountDetails;
        private Crownwood.Magic.Controls.TabPage tabCustomer;
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
        private Crownwood.Magic.Controls.TabPage tabStrategies;
        private GroupBox gbCustomer;
        private Label label5;
        private Label label9;
        private TextBox txtMobile;
        private Label label8;
        private TextBox txtWork;
        private TextBox txtHome;
        private DataGridView dgProducts;
        private GroupBox gbTransactions;
        private DataGridView dgTransactions;
        private GroupBox gbProducts;
        private GroupBox gbWorklists;
        private GroupBox gbStrategies;
        private DataGridView dgStrategies;
        private DataGridView dgWorklists;
        private Crownwood.Magic.Controls.TabPage tabLetters;
        private GroupBox gbSMS;
        private DataGridView dgSMS;
        private GroupBox gbLetters;
        private Label showMultipleAccounts;
        private ComboBox drpStrategies;
        private Label enableStrategies;
        private ComboBox drpSendToStrategy;
        private Label label10;
        private ComboBox cmbWorkList;
        private GroupBox gbTelephoneNumber;
        private TextBox txtWDialCode;
        private TextBox txtHDialCode;
        private TextBox txtMDialCode;
        private Button btnSaveTelephone;
        private Label label30;
        private TextBox txt_provisions;
        private TextBox txtPostCode;
        private Label label29;
        private TextBox txtAdd3;
        private Label label28;
        private TextBox txtAdd2;
        private Label label27;
        private TextBox txtAdd1;
        private TextBox txtTitle;
        private Label Title;
        private Label label4;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private Label label17;
        private Button btnCustomerDetails;
        private DateTimePicker dtpReminderTime;
        private GroupBox gbReminder;
        private PictureBox pbAnimatedGif;
        private Label label32;
        private Label label31;
        private TextBox txtReminderComment;
        private TextBox txtReminderDateTime;
        private CheckBox chkCancelReminders;
        private Crownwood.Magic.Controls.TabPage tabOtherAccounts;
        private DataGridView dgLetters;
        private DataGridView dgOtherAccounts;
        //private DataTable dw;       //CR1084
        private string[] arr;   //CR1084
        private Label lblDummyForErrorProvider;
        private ErrorProvider errorProviderForWarning;
        private ComboBox cmbBranch;
        private CheckBox chkTop500;
        private CheckBox chkApplyAllAccounts;
        private Label lblReferReason;
        private Button btnReferences;
        private CheckBox chkFilterByBranch;
        private Label label39;

        private ComboBox drpReferReason;
        private Button btn_calc;
        private ToolTip toolTip1;

        private struct SanctionStage        //CR1084
        {
            public string AccountNo;
            public DateTime DateProp;

        }
        private SanctionStage StageToLaunch;

        #endregion --------------------------------------------------------------------------------

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TelephoneAction5_2));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.drpReferReason = new System.Windows.Forms.ComboBox();
            this.lblReferReason = new System.Windows.Forms.Label();
            this.lblDummyForErrorProvider = new System.Windows.Forms.Label();
            this.enableStrategies = new System.Windows.Forms.Label();
            this.drpStrategies = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gbLastAction = new System.Windows.Forms.GroupBox();
            this.txtLastActionCode = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dtLastActionDate = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.gbNewAction = new System.Windows.Forms.GroupBox();
            this.chkApplyAllAccounts = new System.Windows.Forms.CheckBox();
            this.chkCancelReminders = new System.Windows.Forms.CheckBox();
            this.dtpReminderTime = new System.Windows.Forms.DateTimePicker();
            this.drpSendToStrategy = new System.Windows.Forms.ComboBox();
            this.chCycleNext = new System.Windows.Forms.CheckBox();
            this.drpReason = new System.Windows.Forms.ComboBox();
            this.chkTop500 = new System.Windows.Forms.CheckBox();
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
            this.gbReminder = new System.Windows.Forms.GroupBox();
            this.txtReminderComment = new System.Windows.Forms.TextBox();
            this.txtReminderDateTime = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.pbAnimatedGif = new System.Windows.Forms.PictureBox();
            this.tabAccounts = new Crownwood.Magic.Controls.TabControl();
            this.tabAccountList = new Crownwood.Magic.Controls.TabPage();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cmbWorkList = new System.Windows.Forms.ComboBox();
            this.dgAccounts = new System.Windows.Forms.DataGrid();
            this.chkFilterByBranch = new System.Windows.Forms.CheckBox();
            this.tabAccountDetails = new Crownwood.Magic.Controls.TabPage();
            this.tabSubAccountDetails = new Crownwood.Magic.Controls.TabControl();
            this.tabCustomer = new Crownwood.Magic.Controls.TabPage();
            this.btn_calc = new System.Windows.Forms.Button();
            this.txt_provisions = new System.Windows.Forms.TextBox();
            this.btnReferences = new System.Windows.Forms.Button();
            this.label30 = new System.Windows.Forms.Label();
            this.btnCustomerDetails = new System.Windows.Forms.Button();
            this.gbTelephoneNumber = new System.Windows.Forms.GroupBox();
            this.btnSaveTelephone = new System.Windows.Forms.Button();
            this.txtMDialCode = new System.Windows.Forms.TextBox();
            this.txtWDialCode = new System.Windows.Forms.TextBox();
            this.txtHDialCode = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtHome = new System.Windows.Forms.TextBox();
            this.txtWork = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtMobile = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.showMultipleAccounts = new System.Windows.Forms.Label();
            this.gbCustomer = new System.Windows.Forms.GroupBox();
            this.label39 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.txtAdd3 = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.txtAdd2 = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.txtAdd1 = new System.Windows.Forms.TextBox();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.Title = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtPostCode = new System.Windows.Forms.TextBox();
            this.btnAccountDetails = new System.Windows.Forms.Button();
            this.btnAddCode = new System.Windows.Forms.Button();
            this.btnFollowUp = new System.Windows.Forms.Button();
            this.lMoreRewardsDate2 = new System.Windows.Forms.Label();
            this.dtMoreRewardsDate2 = new System.Windows.Forms.DateTimePicker();
            this.lMoreRewards2 = new System.Windows.Forms.Label();
            this.txtMoreRewards2 = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.txtMonthArrears = new System.Windows.Forms.TextBox();
            this.txtPercentage = new System.Windows.Forms.TextBox();
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
            this.label34 = new System.Windows.Forms.Label();
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
            this.tabOtherAccounts = new Crownwood.Magic.Controls.TabPage();
            this.dgOtherAccounts = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.textBox13 = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.textBox14 = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.textBox15 = new System.Windows.Forms.TextBox();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProviderForWarning = new System.Windows.Forms.ErrorProvider(this.components);
            this.WarningProvider = new STL.PL.ErrorProviderExtended(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox3.SuspendLayout();
            this.gbLastAction.SuspendLayout();
            this.gbNewAction.SuspendLayout();
            this.gbReminder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAnimatedGif)).BeginInit();
            this.tabAccounts.SuspendLayout();
            this.tabAccountList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
            this.tabAccountDetails.SuspendLayout();
            this.tabSubAccountDetails.SuspendLayout();
            this.tabCustomer.SuspendLayout();
            this.gbTelephoneNumber.SuspendLayout();
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
            this.tabOtherAccounts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgOtherAccounts)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderForWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WarningProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox3.Controls.Add(this.drpReferReason);
            this.groupBox3.Controls.Add(this.lblReferReason);
            this.groupBox3.Controls.Add(this.lblDummyForErrorProvider);
            this.groupBox3.Controls.Add(this.enableStrategies);
            this.groupBox3.Controls.Add(this.drpStrategies);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.gbLastAction);
            this.groupBox3.Controls.Add(this.gbNewAction);
            this.groupBox3.Controls.Add(this.gbReminder);
            this.groupBox3.Controls.Add(this.tabAccounts);
            this.groupBox3.Location = new System.Drawing.Point(8, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(776, 472);
            this.groupBox3.TabIndex = 32;
            this.groupBox3.TabStop = false;
            // 
            // drpReferReason
            // 
            this.drpReferReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpReferReason.FormattingEnabled = true;
            this.drpReferReason.Location = new System.Drawing.Point(13, 180);
            this.drpReferReason.Name = "drpReferReason";
            this.drpReferReason.Size = new System.Drawing.Size(165, 21);
            this.drpReferReason.TabIndex = 92;
            // 
            // lblReferReason
            // 
            this.lblReferReason.AutoSize = true;
            this.lblReferReason.Location = new System.Drawing.Point(10, 163);
            this.lblReferReason.Name = "lblReferReason";
            this.lblReferReason.Size = new System.Drawing.Size(84, 13);
            this.lblReferReason.TabIndex = 91;
            this.lblReferReason.Text = "Referral Reason";
            // 
            // lblDummyForErrorProvider
            // 
            this.lblDummyForErrorProvider.Location = new System.Drawing.Point(399, 216);
            this.lblDummyForErrorProvider.Name = "lblDummyForErrorProvider";
            this.lblDummyForErrorProvider.Size = new System.Drawing.Size(53, 15);
            this.lblDummyForErrorProvider.TabIndex = 89;
            this.lblDummyForErrorProvider.Text = "Dummy";
            // 
            // enableStrategies
            // 
            this.enableStrategies.AutoSize = true;
            this.enableStrategies.Enabled = false;
            this.enableStrategies.Location = new System.Drawing.Point(530, 31);
            this.enableStrategies.Name = "enableStrategies";
            this.enableStrategies.Size = new System.Drawing.Size(0, 13);
            this.enableStrategies.TabIndex = 87;
            this.enableStrategies.Visible = false;
            // 
            // drpStrategies
            // 
            this.drpStrategies.Enabled = false;
            this.drpStrategies.FormattingEnabled = true;
            this.drpStrategies.Location = new System.Drawing.Point(69, 23);
            this.drpStrategies.Name = "drpStrategies";
            this.drpStrategies.Size = new System.Drawing.Size(178, 21);
            this.drpStrategies.TabIndex = 86;
            this.drpStrategies.SelectionChangeCommitted += new System.EventHandler(this.drpStrategies_SelectionChangeCommitted);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 85;
            this.label2.Text = "Strategy";
            // 
            // gbLastAction
            // 
            this.gbLastAction.Controls.Add(this.txtLastActionCode);
            this.gbLastAction.Controls.Add(this.label6);
            this.gbLastAction.Controls.Add(this.dtLastActionDate);
            this.gbLastAction.Controls.Add(this.label7);
            this.gbLastAction.Enabled = false;
            this.gbLastAction.Location = new System.Drawing.Point(8, 55);
            this.gbLastAction.Name = "gbLastAction";
            this.gbLastAction.Size = new System.Drawing.Size(176, 99);
            this.gbLastAction.TabIndex = 82;
            this.gbLastAction.TabStop = false;
            this.gbLastAction.Text = "Last Action";
            // 
            // txtLastActionCode
            // 
            this.txtLastActionCode.Location = new System.Drawing.Point(8, 70);
            this.txtLastActionCode.Name = "txtLastActionCode";
            this.txtLastActionCode.ReadOnly = true;
            this.txtLastActionCode.Size = new System.Drawing.Size(160, 20);
            this.txtLastActionCode.TabIndex = 88;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(8, 54);
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
            this.dtLastActionDate.Location = new System.Drawing.Point(8, 30);
            this.dtLastActionDate.Name = "dtLastActionDate";
            this.dtLastActionDate.Size = new System.Drawing.Size(112, 20);
            this.dtLastActionDate.TabIndex = 85;
            this.dtLastActionDate.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(8, 14);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 16);
            this.label7.TabIndex = 86;
            this.label7.Text = "Date Added";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gbNewAction
            // 
            this.gbNewAction.Controls.Add(this.chkApplyAllAccounts);
            this.gbNewAction.Controls.Add(this.chkCancelReminders);
            this.gbNewAction.Controls.Add(this.dtpReminderTime);
            this.gbNewAction.Controls.Add(this.drpSendToStrategy);
            this.gbNewAction.Controls.Add(this.chCycleNext);
            this.gbNewAction.Controls.Add(this.drpReason);
            this.gbNewAction.Controls.Add(this.chkTop500);
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
            this.gbNewAction.Location = new System.Drawing.Point(184, 55);
            this.gbNewAction.Name = "gbNewAction";
            this.gbNewAction.Size = new System.Drawing.Size(584, 152);
            this.gbNewAction.TabIndex = 81;
            this.gbNewAction.TabStop = false;
            this.gbNewAction.Text = "New Action";
            // 
            // chkApplyAllAccounts
            // 
            this.chkApplyAllAccounts.AutoSize = true;
            this.chkApplyAllAccounts.Location = new System.Drawing.Point(409, 82);
            this.chkApplyAllAccounts.Name = "chkApplyAllAccounts";
            this.chkApplyAllAccounts.Size = new System.Drawing.Size(124, 17);
            this.chkApplyAllAccounts.TabIndex = 97;
            this.chkApplyAllAccounts.Text = "Apply to all accounts";
            this.chkApplyAllAccounts.UseVisualStyleBackColor = true;
            // 
            // chkCancelReminders
            // 
            this.chkCancelReminders.AutoSize = true;
            this.chkCancelReminders.Location = new System.Drawing.Point(409, 100);
            this.chkCancelReminders.Name = "chkCancelReminders";
            this.chkCancelReminders.Size = new System.Drawing.Size(172, 17);
            this.chkCancelReminders.TabIndex = 96;
            this.chkCancelReminders.Text = "Cancel Outstanding Reminders";
            this.chkCancelReminders.UseVisualStyleBackColor = true;
            // 
            // dtpReminderTime
            // 
            this.dtpReminderTime.CustomFormat = "hh:mm tt";
            this.dtpReminderTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpReminderTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtpReminderTime.Location = new System.Drawing.Point(383, 30);
            this.dtpReminderTime.Name = "dtpReminderTime";
            this.dtpReminderTime.ShowUpDown = true;
            this.dtpReminderTime.Size = new System.Drawing.Size(74, 20);
            this.dtpReminderTime.TabIndex = 95;
            this.dtpReminderTime.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtpReminderTime.Visible = false;
            // 
            // drpSendToStrategy
            // 
            this.drpSendToStrategy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpSendToStrategy.FormattingEnabled = true;
            this.drpSendToStrategy.Location = new System.Drawing.Point(380, 29);
            this.drpSendToStrategy.Name = "drpSendToStrategy";
            this.drpSendToStrategy.Size = new System.Drawing.Size(173, 21);
            this.drpSendToStrategy.TabIndex = 93;
            // 
            // chCycleNext
            // 
            this.chCycleNext.AutoSize = true;
            this.chCycleNext.Checked = true;
            this.chCycleNext.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chCycleNext.Location = new System.Drawing.Point(409, 118);
            this.chCycleNext.Name = "chCycleNext";
            this.chCycleNext.Size = new System.Drawing.Size(129, 17);
            this.chCycleNext.TabIndex = 92;
            this.chCycleNext.Text = "Cycle to next account";
            this.chCycleNext.UseVisualStyleBackColor = true;
            // 
            // drpReason
            // 
            this.drpReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpReason.Location = new System.Drawing.Point(421, 54);
            this.drpReason.Name = "drpReason";
            this.drpReason.Size = new System.Drawing.Size(153, 21);
            this.drpReason.TabIndex = 91;
            this.drpReason.Visible = false;
            // 
            // chkTop500
            // 
            this.chkTop500.AutoSize = true;
            this.chkTop500.Location = new System.Drawing.Point(409, 134);
            this.chkTop500.Name = "chkTop500";
            this.chkTop500.Size = new System.Drawing.Size(88, 17);
            this.chkTop500.TabIndex = 4;
            this.chkTop500.Text = "View top 500";
            this.chkTop500.UseVisualStyleBackColor = true;
            this.chkTop500.CheckedChanged += new System.EventHandler(this.chkTop500_CheckedChanged);
            // 
            // lbReason
            // 
            this.lbReason.Location = new System.Drawing.Point(373, 56);
            this.lbReason.Name = "lbReason";
            this.lbReason.Size = new System.Drawing.Size(48, 16);
            this.lbReason.TabIndex = 90;
            this.lbReason.Text = "Reason";
            this.lbReason.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbReason.Visible = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(17, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 16);
            this.label3.TabIndex = 89;
            this.label3.Text = "Notes";
            // 
            // btnLoadExtraInfo
            // 
            this.btnLoadExtraInfo.Location = new System.Drawing.Point(476, 53);
            this.btnLoadExtraInfo.Name = "btnLoadExtraInfo";
            this.btnLoadExtraInfo.Size = new System.Drawing.Size(102, 24);
            this.btnLoadExtraInfo.TabIndex = 88;
            this.btnLoadExtraInfo.Text = "SPA History";
            this.btnLoadExtraInfo.Click += new System.EventHandler(this.btnLoadExtraInfo_Click);
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(11, 76);
            this.txtNotes.MaxLength = 700;
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(344, 64);
            this.txtNotes.TabIndex = 87;
            // 
            // txtActionValue
            // 
            this.txtActionValue.Location = new System.Drawing.Point(376, 30);
            this.txtActionValue.Name = "txtActionValue";
            this.txtActionValue.Size = new System.Drawing.Size(120, 20);
            this.txtActionValue.TabIndex = 85;
            this.txtActionValue.Leave += new System.EventHandler(this.txtActionValue_Leave);
            // 
            // drpActionCode
            // 
            this.drpActionCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpActionCode.ItemHeight = 13;
            this.drpActionCode.Items.AddRange(new object[] {
            "C",
            "R"});
            this.drpActionCode.Location = new System.Drawing.Point(10, 30);
            this.drpActionCode.Name = "drpActionCode";
            this.drpActionCode.Size = new System.Drawing.Size(224, 21);
            this.drpActionCode.TabIndex = 84;
            this.drpActionCode.SelectedIndexChanged += new System.EventHandler(this.drpActionCode_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 14);
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
            this.btnSave.Location = new System.Drawing.Point(376, 113);
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
            this.dtDueDate.Location = new System.Drawing.Point(240, 30);
            this.dtDueDate.Name = "dtDueDate";
            this.dtDueDate.Size = new System.Drawing.Size(120, 20);
            this.dtDueDate.TabIndex = 81;
            this.dtDueDate.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // lbActionValue
            // 
            this.lbActionValue.Location = new System.Drawing.Point(373, 11);
            this.lbActionValue.Name = "lbActionValue";
            this.lbActionValue.Size = new System.Drawing.Size(130, 15);
            this.lbActionValue.TabIndex = 86;
            this.lbActionValue.Text = "Action Value";
            this.lbActionValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbActionDate
            // 
            this.lbActionDate.Location = new System.Drawing.Point(240, 10);
            this.lbActionDate.Name = "lbActionDate";
            this.lbActionDate.Size = new System.Drawing.Size(104, 16);
            this.lbActionDate.TabIndex = 82;
            this.lbActionDate.Text = "Date Due";
            this.lbActionDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gbReminder
            // 
            this.gbReminder.Controls.Add(this.txtReminderComment);
            this.gbReminder.Controls.Add(this.txtReminderDateTime);
            this.gbReminder.Controls.Add(this.label32);
            this.gbReminder.Controls.Add(this.label31);
            this.gbReminder.Controls.Add(this.pbAnimatedGif);
            this.gbReminder.Location = new System.Drawing.Point(272, 12);
            this.gbReminder.Name = "gbReminder";
            this.gbReminder.Size = new System.Drawing.Size(496, 50);
            this.gbReminder.TabIndex = 88;
            this.gbReminder.TabStop = false;
            // 
            // txtReminderComment
            // 
            this.txtReminderComment.Location = new System.Drawing.Point(214, 6);
            this.txtReminderComment.Multiline = true;
            this.txtReminderComment.Name = "txtReminderComment";
            this.txtReminderComment.ReadOnly = true;
            this.txtReminderComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReminderComment.Size = new System.Drawing.Size(272, 37);
            this.txtReminderComment.TabIndex = 4;
            // 
            // txtReminderDateTime
            // 
            this.txtReminderDateTime.Location = new System.Drawing.Point(60, 25);
            this.txtReminderDateTime.Name = "txtReminderDateTime";
            this.txtReminderDateTime.ReadOnly = true;
            this.txtReminderDateTime.Size = new System.Drawing.Size(106, 20);
            this.txtReminderDateTime.TabIndex = 3;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(172, 8);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(38, 13);
            this.label32.TabIndex = 2;
            this.label32.Text = "Notes:";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(61, 9);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(104, 13);
            this.label31.TabIndex = 1;
            this.label31.Text = "Reminder Due Date:";
            // 
            // pbAnimatedGif
            // 
            this.pbAnimatedGif.Image = ((System.Drawing.Image)(resources.GetObject("pbAnimatedGif.Image")));
            this.pbAnimatedGif.Location = new System.Drawing.Point(8, 8);
            this.pbAnimatedGif.Name = "pbAnimatedGif";
            this.pbAnimatedGif.Size = new System.Drawing.Size(38, 38);
            this.pbAnimatedGif.TabIndex = 0;
            this.pbAnimatedGif.TabStop = false;
            // 
            // tabAccounts
            // 
            this.tabAccounts.IDEPixelArea = true;
            this.tabAccounts.Location = new System.Drawing.Point(8, 213);
            this.tabAccounts.Name = "tabAccounts";
            this.tabAccounts.PositionTop = true;
            this.tabAccounts.SelectedIndex = 0;
            this.tabAccounts.SelectedTab = this.tabAccountDetails;
            this.tabAccounts.Size = new System.Drawing.Size(760, 259);
            this.tabAccounts.TabIndex = 83;
            this.tabAccounts.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tabAccountDetails,
            this.tabAccountList});
            this.tabAccounts.SelectionChanged += new System.EventHandler(this.tabAccounts_SelectionChanged);
            // 
            // tabAccountList
            // 
            this.tabAccountList.BackColor = System.Drawing.SystemColors.Control;
            this.tabAccountList.Controls.Add(this.cmbBranch);
            this.tabAccountList.Controls.Add(this.label10);
            this.tabAccountList.Controls.Add(this.cmbWorkList);
            this.tabAccountList.Controls.Add(this.dgAccounts);
            this.tabAccountList.Controls.Add(this.chkFilterByBranch);
            this.tabAccountList.Location = new System.Drawing.Point(0, 25);
            this.tabAccountList.Name = "tabAccountList";
            this.tabAccountList.Padding = new System.Windows.Forms.Padding(3);
            this.tabAccountList.Selected = false;
            this.tabAccountList.Size = new System.Drawing.Size(760, 234);
            this.tabAccountList.TabIndex = 1;
            this.tabAccountList.Title = "Accounts";
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.Enabled = false;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Location = new System.Drawing.Point(328, 8);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(112, 23);
            this.cmbBranch.TabIndex = 90;
            this.cmbBranch.SelectedIndexChanged += new System.EventHandler(this.cmbBranch_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 11);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(50, 15);
            this.label10.TabIndex = 89;
            this.label10.Text = "Worklist";
            // 
            // cmbWorkList
            // 
            this.cmbWorkList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWorkList.FormattingEnabled = true;
            this.cmbWorkList.Location = new System.Drawing.Point(64, 7);
            this.cmbWorkList.Name = "cmbWorkList";
            this.cmbWorkList.Size = new System.Drawing.Size(194, 23);
            this.cmbWorkList.TabIndex = 88;
            this.cmbWorkList.SelectedIndexChanged += new System.EventHandler(this.cmbWorkList_SelectedIndexChanged);
            // 
            // dgAccounts
            // 
            this.dgAccounts.CaptionText = "Accounts in Worklist";
            this.dgAccounts.DataMember = "";
            this.dgAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAccounts.Location = new System.Drawing.Point(5, 35);
            this.dgAccounts.Name = "dgAccounts";
            this.dgAccounts.ReadOnly = true;
            this.dgAccounts.Size = new System.Drawing.Size(751, 193);
            this.dgAccounts.TabIndex = 1;
            this.dgAccounts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgAccounts_MouseUp);
            // 
            // chkFilterByBranch
            // 
            this.chkFilterByBranch.AutoSize = true;
            this.chkFilterByBranch.Location = new System.Drawing.Point(264, 11);
            this.chkFilterByBranch.Name = "chkFilterByBranch";
            this.chkFilterByBranch.Size = new System.Drawing.Size(63, 19);
            this.chkFilterByBranch.TabIndex = 93;
            this.chkFilterByBranch.Text = "Branch";
            this.chkFilterByBranch.UseVisualStyleBackColor = true;
            this.chkFilterByBranch.CheckedChanged += new System.EventHandler(this.chkFilterByBranch_CheckedChanged);
            // 
            // tabAccountDetails
            // 
            this.tabAccountDetails.Controls.Add(this.tabSubAccountDetails);
            this.tabAccountDetails.Controls.Add(this.button1);
            this.tabAccountDetails.Controls.Add(this.button2);
            this.tabAccountDetails.Controls.Add(this.button3);
            this.tabAccountDetails.Controls.Add(this.groupBox1);
            this.tabAccountDetails.Controls.Add(this.label18);
            this.tabAccountDetails.Controls.Add(this.textBox7);
            this.tabAccountDetails.Controls.Add(this.textBox8);
            this.tabAccountDetails.Controls.Add(this.label19);
            this.tabAccountDetails.Controls.Add(this.textBox9);
            this.tabAccountDetails.Controls.Add(this.label20);
            this.tabAccountDetails.Controls.Add(this.textBox10);
            this.tabAccountDetails.Controls.Add(this.label21);
            this.tabAccountDetails.Controls.Add(this.label22);
            this.tabAccountDetails.Controls.Add(this.textBox11);
            this.tabAccountDetails.Controls.Add(this.textBox12);
            this.tabAccountDetails.Controls.Add(this.textBox13);
            this.tabAccountDetails.Controls.Add(this.label23);
            this.tabAccountDetails.Controls.Add(this.label24);
            this.tabAccountDetails.Controls.Add(this.textBox14);
            this.tabAccountDetails.Controls.Add(this.label25);
            this.tabAccountDetails.Controls.Add(this.label26);
            this.tabAccountDetails.Controls.Add(this.textBox15);
            this.tabAccountDetails.Location = new System.Drawing.Point(0, 25);
            this.tabAccountDetails.Name = "tabAccountDetails";
            this.tabAccountDetails.Size = new System.Drawing.Size(760, 234);
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
            this.tabSubAccountDetails.Size = new System.Drawing.Size(760, 234);
            this.tabSubAccountDetails.TabIndex = 169;
            this.tabSubAccountDetails.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tabCustomer,
            this.tabProducts,
            this.tabStrategies,
            this.tabLetters,
            this.tabOtherAccounts});
            // 
            // tabCustomer
            // 
            this.tabCustomer.Controls.Add(this.btn_calc);
            this.tabCustomer.Controls.Add(this.txt_provisions);
            this.tabCustomer.Controls.Add(this.btnReferences);
            this.tabCustomer.Controls.Add(this.label30);
            this.tabCustomer.Controls.Add(this.btnCustomerDetails);
            this.tabCustomer.Controls.Add(this.gbTelephoneNumber);
            this.tabCustomer.Controls.Add(this.showMultipleAccounts);
            this.tabCustomer.Controls.Add(this.gbCustomer);
            this.tabCustomer.Controls.Add(this.btnAccountDetails);
            this.tabCustomer.Controls.Add(this.btnAddCode);
            this.tabCustomer.Controls.Add(this.btnFollowUp);
            this.tabCustomer.Controls.Add(this.lMoreRewardsDate2);
            this.tabCustomer.Controls.Add(this.dtMoreRewardsDate2);
            this.tabCustomer.Controls.Add(this.lMoreRewards2);
            this.tabCustomer.Controls.Add(this.txtMoreRewards2);
            this.tabCustomer.Controls.Add(this.label33);
            this.tabCustomer.Controls.Add(this.txtMonthArrears);
            this.tabCustomer.Controls.Add(this.txtPercentage);
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
            this.tabCustomer.Controls.Add(this.label34);
            this.tabCustomer.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.tabCustomer.Location = new System.Drawing.Point(0, 25);
            this.tabCustomer.Name = "tabCustomer";
            this.tabCustomer.Size = new System.Drawing.Size(760, 209);
            this.tabCustomer.TabIndex = 0;
            this.tabCustomer.Title = "Customer/Account Information";
            // 
            // btn_calc
            // 
            this.btn_calc.Image = global::STL.PL.Properties.Resources.Calc;
            this.btn_calc.Location = new System.Drawing.Point(9, 75);
            this.btn_calc.Name = "btn_calc";
            this.btn_calc.Size = new System.Drawing.Size(37, 29);
            this.btn_calc.TabIndex = 174;
            this.toolTip1.SetToolTip(this.btn_calc, "Payment Calculator");
            this.btn_calc.UseVisualStyleBackColor = true;
            this.btn_calc.Click += new System.EventHandler(this.btn_calc_Click);
            // 
            // txt_provisions
            // 
            this.txt_provisions.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_provisions.Location = new System.Drawing.Point(61, 104);
            this.txt_provisions.Name = "txt_provisions";
            this.txt_provisions.ReadOnly = true;
            this.txt_provisions.Size = new System.Drawing.Size(67, 21);
            this.txt_provisions.TabIndex = 169;
            this.txt_provisions.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnReferences
            // 
            this.btnReferences.Location = new System.Drawing.Point(590, 87);
            this.btnReferences.Name = "btnReferences";
            this.btnReferences.Size = new System.Drawing.Size(164, 23);
            this.btnReferences.TabIndex = 173;
            this.btnReferences.Text = "View References";
            this.btnReferences.UseVisualStyleBackColor = true;
            this.btnReferences.Click += new System.EventHandler(this.btnReferences_Click);
            // 
            // label30
            // 
            this.label30.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label30.Location = new System.Drawing.Point(1, 104);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(56, 23);
            this.label30.TabIndex = 170;
            this.label30.Text = "Provision";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCustomerDetails
            // 
            this.btnCustomerDetails.Location = new System.Drawing.Point(590, 184);
            this.btnCustomerDetails.Name = "btnCustomerDetails";
            this.btnCustomerDetails.Size = new System.Drawing.Size(165, 22);
            this.btnCustomerDetails.TabIndex = 172;
            this.btnCustomerDetails.Text = "Customer Details";
            this.btnCustomerDetails.UseVisualStyleBackColor = true;
            this.btnCustomerDetails.Click += new System.EventHandler(this.btnCustomerDetails_Click);
            // 
            // gbTelephoneNumber
            // 
            this.gbTelephoneNumber.Controls.Add(this.btnSaveTelephone);
            this.gbTelephoneNumber.Controls.Add(this.txtMDialCode);
            this.gbTelephoneNumber.Controls.Add(this.txtWDialCode);
            this.gbTelephoneNumber.Controls.Add(this.txtHDialCode);
            this.gbTelephoneNumber.Controls.Add(this.label8);
            this.gbTelephoneNumber.Controls.Add(this.txtHome);
            this.gbTelephoneNumber.Controls.Add(this.txtWork);
            this.gbTelephoneNumber.Controls.Add(this.label5);
            this.gbTelephoneNumber.Controls.Add(this.txtMobile);
            this.gbTelephoneNumber.Controls.Add(this.label9);
            this.gbTelephoneNumber.Location = new System.Drawing.Point(544, 3);
            this.gbTelephoneNumber.Name = "gbTelephoneNumber";
            this.gbTelephoneNumber.Size = new System.Drawing.Size(211, 82);
            this.gbTelephoneNumber.TabIndex = 171;
            this.gbTelephoneNumber.TabStop = false;
            this.gbTelephoneNumber.Text = "Telephone Numbers";
            // 
            // btnSaveTelephone
            // 
            this.btnSaveTelephone.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSaveTelephone.BackgroundImage")));
            this.btnSaveTelephone.Enabled = false;
            this.btnSaveTelephone.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSaveTelephone.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveTelephone.Location = new System.Drawing.Point(182, 45);
            this.btnSaveTelephone.Name = "btnSaveTelephone";
            this.btnSaveTelephone.Size = new System.Drawing.Size(24, 24);
            this.btnSaveTelephone.TabIndex = 81;
            this.btnSaveTelephone.Click += new System.EventHandler(this.btnSaveTelephone_Click);
            // 
            // txtMDialCode
            // 
            this.txtMDialCode.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMDialCode.Location = new System.Drawing.Point(47, 56);
            this.txtMDialCode.Name = "txtMDialCode";
            this.txtMDialCode.ReadOnly = true;
            this.txtMDialCode.Size = new System.Drawing.Size(35, 20);
            this.txtMDialCode.TabIndex = 145;
            // 
            // txtWDialCode
            // 
            this.txtWDialCode.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWDialCode.Location = new System.Drawing.Point(47, 36);
            this.txtWDialCode.Name = "txtWDialCode";
            this.txtWDialCode.ReadOnly = true;
            this.txtWDialCode.Size = new System.Drawing.Size(35, 20);
            this.txtWDialCode.TabIndex = 144;
            // 
            // txtHDialCode
            // 
            this.txtHDialCode.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHDialCode.Location = new System.Drawing.Point(47, 16);
            this.txtHDialCode.Name = "txtHDialCode";
            this.txtHDialCode.ReadOnly = true;
            this.txtHDialCode.Size = new System.Drawing.Size(35, 20);
            this.txtHDialCode.TabIndex = 143;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(7, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 14);
            this.label8.TabIndex = 139;
            this.label8.Text = "Home";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtHome
            // 
            this.txtHome.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHome.Location = new System.Drawing.Point(85, 16);
            this.txtHome.Name = "txtHome";
            this.txtHome.ReadOnly = true;
            this.txtHome.Size = new System.Drawing.Size(91, 20);
            this.txtHome.TabIndex = 136;
            // 
            // txtWork
            // 
            this.txtWork.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWork.Location = new System.Drawing.Point(85, 36);
            this.txtWork.Name = "txtWork";
            this.txtWork.ReadOnly = true;
            this.txtWork.Size = new System.Drawing.Size(91, 20);
            this.txtWork.TabIndex = 135;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(7, 59);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 14);
            this.label5.TabIndex = 138;
            this.label5.Text = "Mobile";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtMobile
            // 
            this.txtMobile.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMobile.Location = new System.Drawing.Point(85, 56);
            this.txtMobile.Name = "txtMobile";
            this.txtMobile.ReadOnly = true;
            this.txtMobile.Size = new System.Drawing.Size(91, 20);
            this.txtMobile.TabIndex = 137;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(7, 39);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 14);
            this.label9.TabIndex = 140;
            this.label9.Text = "Work";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // showMultipleAccounts
            // 
            this.showMultipleAccounts.AutoSize = true;
            this.showMultipleAccounts.Enabled = false;
            this.showMultipleAccounts.Location = new System.Drawing.Point(679, 159);
            this.showMultipleAccounts.Name = "showMultipleAccounts";
            this.showMultipleAccounts.Size = new System.Drawing.Size(0, 14);
            this.showMultipleAccounts.TabIndex = 170;
            this.showMultipleAccounts.Visible = false;
            // 
            // gbCustomer
            // 
            this.gbCustomer.Controls.Add(this.label39);
            this.gbCustomer.Controls.Add(this.label29);
            this.gbCustomer.Controls.Add(this.txtAdd3);
            this.gbCustomer.Controls.Add(this.label28);
            this.gbCustomer.Controls.Add(this.txtAdd2);
            this.gbCustomer.Controls.Add(this.label27);
            this.gbCustomer.Controls.Add(this.txtAdd1);
            this.gbCustomer.Controls.Add(this.txtTitle);
            this.gbCustomer.Controls.Add(this.Title);
            this.gbCustomer.Controls.Add(this.label4);
            this.gbCustomer.Controls.Add(this.txtFirstName);
            this.gbCustomer.Controls.Add(this.txtLastName);
            this.gbCustomer.Controls.Add(this.label17);
            this.gbCustomer.Controls.Add(this.txtPostCode);
            this.gbCustomer.Location = new System.Drawing.Point(276, 3);
            this.gbCustomer.Name = "gbCustomer";
            this.gbCustomer.Size = new System.Drawing.Size(264, 198);
            this.gbCustomer.TabIndex = 169;
            this.gbCustomer.TabStop = false;
            this.gbCustomer.Text = "Customer Details";
            // 
            // label39
            // 
            this.label39.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label39.Location = new System.Drawing.Point(12, 173);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(56, 23);
            this.label39.TabIndex = 174;
            this.label39.Text = "Postcode";
            this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label29
            // 
            this.label29.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label29.Location = new System.Drawing.Point(6, 152);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(62, 14);
            this.label29.TabIndex = 168;
            this.label29.Text = "Address 3";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAdd3
            // 
            this.txtAdd3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAdd3.Location = new System.Drawing.Point(70, 149);
            this.txtAdd3.Name = "txtAdd3";
            this.txtAdd3.ReadOnly = true;
            this.txtAdd3.Size = new System.Drawing.Size(181, 21);
            this.txtAdd3.TabIndex = 167;
            // 
            // label28
            // 
            this.label28.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label28.Location = new System.Drawing.Point(6, 128);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(62, 14);
            this.label28.TabIndex = 166;
            this.label28.Text = "Address 2";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAdd2
            // 
            this.txtAdd2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAdd2.Location = new System.Drawing.Point(70, 125);
            this.txtAdd2.Name = "txtAdd2";
            this.txtAdd2.ReadOnly = true;
            this.txtAdd2.Size = new System.Drawing.Size(181, 21);
            this.txtAdd2.TabIndex = 165;
            // 
            // label27
            // 
            this.label27.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label27.Location = new System.Drawing.Point(6, 104);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(62, 14);
            this.label27.TabIndex = 164;
            this.label27.Text = "Address 1";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAdd1
            // 
            this.txtAdd1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAdd1.Location = new System.Drawing.Point(70, 101);
            this.txtAdd1.Name = "txtAdd1";
            this.txtAdd1.ReadOnly = true;
            this.txtAdd1.Size = new System.Drawing.Size(181, 21);
            this.txtAdd1.TabIndex = 163;
            // 
            // txtTitle
            // 
            this.txtTitle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTitle.Location = new System.Drawing.Point(74, 29);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.ReadOnly = true;
            this.txtTitle.Size = new System.Drawing.Size(54, 21);
            this.txtTitle.TabIndex = 161;
            // 
            // Title
            // 
            this.Title.Location = new System.Drawing.Point(6, 35);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(62, 14);
            this.Title.TabIndex = 162;
            this.Title.Text = "Title";
            this.Title.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(6, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 14);
            this.label4.TabIndex = 158;
            this.label4.Text = "First Name";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFirstName
            // 
            this.txtFirstName.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFirstName.Location = new System.Drawing.Point(70, 53);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.ReadOnly = true;
            this.txtFirstName.Size = new System.Drawing.Size(181, 21);
            this.txtFirstName.TabIndex = 157;
            // 
            // txtLastName
            // 
            this.txtLastName.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLastName.Location = new System.Drawing.Point(70, 77);
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.ReadOnly = true;
            this.txtLastName.Size = new System.Drawing.Size(181, 21);
            this.txtLastName.TabIndex = 159;
            // 
            // label17
            // 
            this.label17.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label17.Location = new System.Drawing.Point(6, 80);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(62, 14);
            this.label17.TabIndex = 160;
            this.label17.Text = "Last Name";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPostCode
            // 
            this.txtPostCode.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPostCode.Location = new System.Drawing.Point(70, 173);
            this.txtPostCode.Name = "txtPostCode";
            this.txtPostCode.ReadOnly = true;
            this.txtPostCode.Size = new System.Drawing.Size(181, 21);
            this.txtPostCode.TabIndex = 169;
            // 
            // btnAccountDetails
            // 
            this.btnAccountDetails.Location = new System.Drawing.Point(590, 160);
            this.btnAccountDetails.Name = "btnAccountDetails";
            this.btnAccountDetails.Size = new System.Drawing.Size(165, 22);
            this.btnAccountDetails.TabIndex = 168;
            this.btnAccountDetails.Text = "Account Details";
            this.btnAccountDetails.UseVisualStyleBackColor = true;
            this.btnAccountDetails.Click += new System.EventHandler(this.btnAccountDetails_Click);
            // 
            // btnAddCode
            // 
            this.btnAddCode.Enabled = false;
            this.btnAddCode.Location = new System.Drawing.Point(590, 136);
            this.btnAddCode.Name = "btnAddCode";
            this.btnAddCode.Size = new System.Drawing.Size(165, 22);
            this.btnAddCode.TabIndex = 167;
            this.btnAddCode.Text = "Add Account/Customer Code";
            this.btnAddCode.UseVisualStyleBackColor = true;
            this.btnAddCode.Click += new System.EventHandler(this.btnAddCode_Click);
            // 
            // btnFollowUp
            // 
            this.btnFollowUp.Location = new System.Drawing.Point(590, 112);
            this.btnFollowUp.Name = "btnFollowUp";
            this.btnFollowUp.Size = new System.Drawing.Size(165, 22);
            this.btnFollowUp.TabIndex = 166;
            this.btnFollowUp.Text = "Follow Up Action Details";
            this.btnFollowUp.UseVisualStyleBackColor = true;
            this.btnFollowUp.Click += new System.EventHandler(this.btnFollowUp_Click);
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
            // label33
            // 
            this.label33.Location = new System.Drawing.Point(63, 131);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(108, 14);
            this.label33.TabIndex = 164;
            this.label33.Text = "Months in Arrears";
            this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtMonthArrears
            // 
            this.txtMonthArrears.BackColor = System.Drawing.SystemColors.Control;
            this.txtMonthArrears.Location = new System.Drawing.Point(176, 128);
            this.txtMonthArrears.Name = "txtMonthArrears";
            this.txtMonthArrears.ReadOnly = true;
            this.txtMonthArrears.Size = new System.Drawing.Size(93, 20);
            this.txtMonthArrears.TabIndex = 163;
            this.txtMonthArrears.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtPercentage
            // 
            this.txtPercentage.Location = new System.Drawing.Point(235, 175);
            this.txtPercentage.Name = "txtPercentage";
            this.txtPercentage.ReadOnly = true;
            this.txtPercentage.Size = new System.Drawing.Size(35, 20);
            this.txtPercentage.TabIndex = 162;
            // 
            // txtDateLastPaid
            // 
            this.txtDateLastPaid.Location = new System.Drawing.Point(176, 152);
            this.txtDateLastPaid.Name = "txtDateLastPaid";
            this.txtDateLastPaid.ReadOnly = true;
            this.txtDateLastPaid.Size = new System.Drawing.Size(93, 20);
            this.txtDateLastPaid.TabIndex = 160;
            this.txtDateLastPaid.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label35
            // 
            this.label35.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label35.Location = new System.Drawing.Point(63, 154);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(108, 14);
            this.label35.TabIndex = 159;
            this.label35.Text = "Date Last Paid";
            this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(116, 175);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(25, 20);
            this.txtStatus.TabIndex = 158;
            // 
            // label36
            // 
            this.label36.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label36.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label36.Location = new System.Drawing.Point(6, 177);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(108, 14);
            this.label36.TabIndex = 157;
            this.label36.Text = "Current Status";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label37
            // 
            this.label37.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label37.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label37.Location = new System.Drawing.Point(63, 58);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(108, 14);
            this.label37.TabIndex = 156;
            this.label37.Text = "Due Day";
            this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDueDate
            // 
            this.txtDueDate.BackColor = System.Drawing.SystemColors.Control;
            this.txtDueDate.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtDueDate.Location = new System.Drawing.Point(176, 56);
            this.txtDueDate.MaxLength = 20;
            this.txtDueDate.Name = "txtDueDate";
            this.txtDueDate.ReadOnly = true;
            this.txtDueDate.Size = new System.Drawing.Size(93, 21);
            this.txtDueDate.TabIndex = 155;
            this.txtDueDate.TabStop = false;
            this.txtDueDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtArrears
            // 
            this.txtArrears.BackColor = System.Drawing.SystemColors.Control;
            this.txtArrears.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtArrears.Location = new System.Drawing.Point(176, 104);
            this.txtArrears.MaxLength = 10;
            this.txtArrears.Name = "txtArrears";
            this.txtArrears.ReadOnly = true;
            this.txtArrears.Size = new System.Drawing.Size(93, 21);
            this.txtArrears.TabIndex = 150;
            this.txtArrears.TabStop = false;
            this.txtArrears.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtBalance
            // 
            this.txtBalance.BackColor = System.Drawing.SystemColors.Control;
            this.txtBalance.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtBalance.Location = new System.Drawing.Point(176, 80);
            this.txtBalance.MaxLength = 10;
            this.txtBalance.Name = "txtBalance";
            this.txtBalance.ReadOnly = true;
            this.txtBalance.Size = new System.Drawing.Size(93, 21);
            this.txtBalance.TabIndex = 151;
            this.txtBalance.TabStop = false;
            this.txtBalance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label38
            // 
            this.label38.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label38.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label38.Location = new System.Drawing.Point(63, 106);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(108, 14);
            this.label38.TabIndex = 147;
            this.label38.Text = "Arrears";
            this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label40
            // 
            this.label40.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label40.Location = new System.Drawing.Point(61, 82);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(108, 14);
            this.label40.TabIndex = 148;
            this.label40.Text = "Outstanding Balance";
            this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAgreementTotal
            // 
            this.txtAgreementTotal.BackColor = System.Drawing.SystemColors.Control;
            this.txtAgreementTotal.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAgreementTotal.Location = new System.Drawing.Point(176, 8);
            this.txtAgreementTotal.MaxLength = 10;
            this.txtAgreementTotal.Name = "txtAgreementTotal";
            this.txtAgreementTotal.ReadOnly = true;
            this.txtAgreementTotal.Size = new System.Drawing.Size(94, 21);
            this.txtAgreementTotal.TabIndex = 149;
            this.txtAgreementTotal.TabStop = false;
            this.txtAgreementTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label41
            // 
            this.label41.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label41.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label41.Location = new System.Drawing.Point(63, 34);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(108, 14);
            this.label41.TabIndex = 154;
            this.label41.Text = "Instalment Amount";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label44
            // 
            this.label44.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label44.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label44.Location = new System.Drawing.Point(63, 10);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(108, 14);
            this.label44.TabIndex = 152;
            this.label44.Text = "Agreement Total";
            this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtInstalment
            // 
            this.txtInstalment.BackColor = System.Drawing.SystemColors.Control;
            this.txtInstalment.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtInstalment.Location = new System.Drawing.Point(176, 32);
            this.txtInstalment.MaxLength = 10;
            this.txtInstalment.Name = "txtInstalment";
            this.txtInstalment.ReadOnly = true;
            this.txtInstalment.Size = new System.Drawing.Size(93, 21);
            this.txtInstalment.TabIndex = 153;
            this.txtInstalment.TabStop = false;
            this.txtInstalment.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label34
            // 
            this.label34.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label34.Location = new System.Drawing.Point(143, 177);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(90, 16);
            this.label34.TabIndex = 161;
            this.label34.Text = "Percentage Paid";
            this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabProducts
            // 
            this.tabProducts.Controls.Add(this.gbTransactions);
            this.tabProducts.Controls.Add(this.gbProducts);
            this.tabProducts.Location = new System.Drawing.Point(0, 25);
            this.tabProducts.Name = "tabProducts";
            this.tabProducts.Selected = false;
            this.tabProducts.Size = new System.Drawing.Size(760, 209);
            this.tabProducts.TabIndex = 1;
            this.tabProducts.Title = "Products/Transactions";
            // 
            // gbTransactions
            // 
            this.gbTransactions.Controls.Add(this.dgTransactions);
            this.gbTransactions.Location = new System.Drawing.Point(525, 8);
            this.gbTransactions.Name = "gbTransactions";
            this.gbTransactions.Size = new System.Drawing.Size(217, 193);
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
            this.dgTransactions.Size = new System.Drawing.Size(200, 166);
            this.dgTransactions.TabIndex = 1;
            // 
            // gbProducts
            // 
            this.gbProducts.Controls.Add(this.dgProducts);
            this.gbProducts.Location = new System.Drawing.Point(11, 8);
            this.gbProducts.Name = "gbProducts";
            this.gbProducts.Size = new System.Drawing.Size(505, 193);
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
            this.dgProducts.Size = new System.Drawing.Size(490, 166);
            this.dgProducts.TabIndex = 1;
            // 
            // tabStrategies
            // 
            this.tabStrategies.Controls.Add(this.gbWorklists);
            this.tabStrategies.Controls.Add(this.gbStrategies);
            this.tabStrategies.Location = new System.Drawing.Point(0, 25);
            this.tabStrategies.Name = "tabStrategies";
            this.tabStrategies.Selected = false;
            this.tabStrategies.Size = new System.Drawing.Size(760, 209);
            this.tabStrategies.TabIndex = 2;
            this.tabStrategies.Title = "Strategies/Worklists";
            // 
            // gbWorklists
            // 
            this.gbWorklists.Controls.Add(this.dgWorklists);
            this.gbWorklists.Location = new System.Drawing.Point(385, 8);
            this.gbWorklists.Name = "gbWorklists";
            this.gbWorklists.Size = new System.Drawing.Size(365, 193);
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
            this.dgWorklists.Size = new System.Drawing.Size(348, 166);
            this.dgWorklists.TabIndex = 0;
            // 
            // gbStrategies
            // 
            this.gbStrategies.Controls.Add(this.dgStrategies);
            this.gbStrategies.Location = new System.Drawing.Point(11, 8);
            this.gbStrategies.Name = "gbStrategies";
            this.gbStrategies.Size = new System.Drawing.Size(365, 193);
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
            this.dgStrategies.Size = new System.Drawing.Size(348, 166);
            this.dgStrategies.TabIndex = 1;
            // 
            // tabLetters
            // 
            this.tabLetters.Controls.Add(this.gbSMS);
            this.tabLetters.Controls.Add(this.gbLetters);
            this.tabLetters.Location = new System.Drawing.Point(0, 25);
            this.tabLetters.Name = "tabLetters";
            this.tabLetters.Selected = false;
            this.tabLetters.Size = new System.Drawing.Size(760, 209);
            this.tabLetters.TabIndex = 3;
            this.tabLetters.Title = "Letters/SMS\'s";
            // 
            // gbSMS
            // 
            this.gbSMS.Controls.Add(this.dgSMS);
            this.gbSMS.Location = new System.Drawing.Point(385, 8);
            this.gbSMS.Name = "gbSMS";
            this.gbSMS.Size = new System.Drawing.Size(365, 193);
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
            this.dgSMS.Size = new System.Drawing.Size(348, 166);
            this.dgSMS.TabIndex = 1;
            this.dgSMS.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgSMS_MouseUp);
            // 
            // gbLetters
            // 
            this.gbLetters.Controls.Add(this.dgLetters);
            this.gbLetters.Location = new System.Drawing.Point(11, 8);
            this.gbLetters.Name = "gbLetters";
            this.gbLetters.Size = new System.Drawing.Size(365, 193);
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
            this.dgLetters.Size = new System.Drawing.Size(348, 166);
            this.dgLetters.TabIndex = 1;
            // 
            // tabOtherAccounts
            // 
            this.tabOtherAccounts.Controls.Add(this.dgOtherAccounts);
            this.tabOtherAccounts.Location = new System.Drawing.Point(0, 25);
            this.tabOtherAccounts.Name = "tabOtherAccounts";
            this.tabOtherAccounts.Selected = false;
            this.tabOtherAccounts.Size = new System.Drawing.Size(760, 209);
            this.tabOtherAccounts.TabIndex = 4;
            this.tabOtherAccounts.Title = "Customer Accounts (Other)";
            // 
            // dgOtherAccounts
            // 
            this.dgOtherAccounts.AllowUserToAddRows = false;
            this.dgOtherAccounts.AllowUserToDeleteRows = false;
            this.dgOtherAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgOtherAccounts.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgOtherAccounts.Location = new System.Drawing.Point(6, 5);
            this.dgOtherAccounts.Name = "dgOtherAccounts";
            this.dgOtherAccounts.RowHeadersVisible = false;
            this.dgOtherAccounts.Size = new System.Drawing.Size(748, 199);
            this.dgOtherAccounts.TabIndex = 2;
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
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.textBox4);
            this.groupBox1.Controls.Add(this.textBox5);
            this.groupBox1.Controls.Add(this.label16);
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
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(63, 23);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(44, 15);
            this.label11.TabIndex = 134;
            this.label11.Text = "label11";
            // 
            // label12
            // 
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(20, 48);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(70, 16);
            this.label12.TabIndex = 130;
            this.label12.Text = "First Name:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(53, 158);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(44, 15);
            this.label13.TabIndex = 138;
            this.label13.Text = "Mobile";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(25, 131);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(72, 15);
            this.label14.TabIndex = 140;
            this.label14.Text = "Work Phone";
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
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(23, 104);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(77, 15);
            this.label15.TabIndex = 139;
            this.label15.Text = "Home Phone";
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
            // label16
            // 
            this.label16.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label16.Location = new System.Drawing.Point(27, 75);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(63, 16);
            this.label16.TabIndex = 132;
            this.label16.Text = "Last Name:";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(96, 101);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(100, 23);
            this.textBox6.TabIndex = 136;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(16, 149);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(101, 15);
            this.label18.TabIndex = 164;
            this.label18.Text = "Months in Arrears";
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
            // label19
            // 
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label19.Location = new System.Drawing.Point(144, 201);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(89, 16);
            this.label19.TabIndex = 161;
            this.label19.Text = "Percentage Paid";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(114, 173);
            this.textBox9.Name = "textBox9";
            this.textBox9.ReadOnly = true;
            this.textBox9.Size = new System.Drawing.Size(104, 23);
            this.textBox9.TabIndex = 160;
            // 
            // label20
            // 
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label20.Location = new System.Drawing.Point(24, 174);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(84, 16);
            this.label20.TabIndex = 159;
            this.label20.Text = "Date Last Paid";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(114, 200);
            this.textBox10.Name = "textBox10";
            this.textBox10.ReadOnly = true;
            this.textBox10.Size = new System.Drawing.Size(24, 23);
            this.textBox10.TabIndex = 158;
            // 
            // label21
            // 
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label21.Location = new System.Drawing.Point(19, 201);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(89, 16);
            this.label21.TabIndex = 157;
            this.label21.Text = "Current Status";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label22
            // 
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label22.Location = new System.Drawing.Point(28, 67);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(80, 16);
            this.label22.TabIndex = 156;
            this.label22.Text = "Due Date";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // label23
            // 
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label23.Location = new System.Drawing.Point(36, 120);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(72, 16);
            this.label23.TabIndex = 147;
            this.label23.Text = "Arrears";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label24
            // 
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label24.Location = new System.Drawing.Point(6, 93);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(110, 19);
            this.label24.TabIndex = 148;
            this.label24.Text = "Outstanding Balance";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // label25
            // 
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label25.Location = new System.Drawing.Point(8, 39);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(101, 19);
            this.label25.TabIndex = 154;
            this.label25.Text = "Instalment Amount";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label26
            // 
            this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label26.Location = new System.Drawing.Point(8, 13);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(100, 16);
            this.label26.TabIndex = 152;
            this.label26.Text = "Agreement Total";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataMember = "";
            // 
            // errorProviderForWarning
            // 
            this.errorProviderForWarning.ContainerControl = this;
            this.errorProviderForWarning.Icon = ((System.Drawing.Icon)(resources.GetObject("errorProviderForWarning.Icon")));
            // 
            // WarningProvider
            // 
            this.WarningProvider.ContainerControl = this;
            // 
            // TelephoneAction5_2
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox3);
            this.Name = "TelephoneAction5_2";
            this.Text = "Telephone Actions";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TelephoneAction5_2_FormClosing);
            this.Load += new System.EventHandler(this.TelephoneAction5_2_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gbLastAction.ResumeLayout(false);
            this.gbLastAction.PerformLayout();
            this.gbNewAction.ResumeLayout(false);
            this.gbNewAction.PerformLayout();
            this.gbReminder.ResumeLayout(false);
            this.gbReminder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAnimatedGif)).EndInit();
            this.tabAccounts.ResumeLayout(false);
            this.tabAccountList.ResumeLayout(false);
            this.tabAccountList.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
            this.tabAccountDetails.ResumeLayout(false);
            this.tabAccountDetails.PerformLayout();
            this.tabSubAccountDetails.ResumeLayout(false);
            this.tabCustomer.ResumeLayout(false);
            this.tabCustomer.PerformLayout();
            this.gbTelephoneNumber.ResumeLayout(false);
            this.gbTelephoneNumber.PerformLayout();
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
            this.tabOtherAccounts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgOtherAccounts)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderForWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WarningProvider)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private DataView dvStrategyActions = new DataView();
        private DataSet dsCallReminders = new DataSet();
        private int m_dataviewNo = 0;
        private bool m_strategyChanged = false;
        private string m_strategy = String.Empty;
        private bool m_addCodes = false;
        private string m_acct = String.Empty;
        private string m_custid = String.Empty;
        private string m_accttype = string.Empty;
        private string m_name = string.Empty;
        private bool m_creditblocked = false;               //IP - 28/04/10 - UAT(983) UAT5.2
        private bool m_valid_account = false;
        private int m_accountRow = -1;
        private DataSet dsWorklistAccounts = new DataSet();
        private DataView dvAccounts = new DataView();
        private DataTable dtLegalDetail;
        private DataTable dtInsuranceDetail;
        private DataTable dtFraudDetail;
        private DataTable dtTRCDetail;
        private DataTable dtSPADetails = new DataTable();   //jec CR976 14/01/09
        //private bool extendedTermsSelected;
        private bool reversedBDW; //IP - 28/08/09 - UAT(737)
        //private string m_termstype;
        private DataTable dtSMSDefinition;

        //IP - 28/08/09 - UAT(737)
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

        //IP & JC - 12/01/09 - CR976 - Special Arrangements
        //Added a property to set the custID of the selected account so that this can be passed
        //into the 'Special Arranegements' screen.
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

        public int DataViewNo
        {
            get
            {
                return m_dataviewNo;
            }
            set
            {
                m_dataviewNo = value;
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
                return drpActionCode.SelectedIndex >= 0 ? drpActionCode.SelectedValue.ToString() : "";
            }


        }

        public bool SPASelected
        {
            get
            {
                return (CurrentActionCode == "SPA");
            }
        }

        //IP - 20/10/08 - UAT5.2 - UAT(551) - Check if 'Send To Strategy' action code has been selected.
        public bool STSSelected
        {
            get
            {
                return (CurrentActionCode == "STS");
            }
        }

        //NM & IP - 31/12/08 - CR976 - Check if the 'Send a call reminder' action code has been selected.
        public bool REMSelected
        {
            get
            {
                return (CurrentActionCode == "REM");
            }
        }

        //NM & IP - 02/01/09 - CR976 - Check if the 'Send a personal call reminder' action code has been selected.
        public bool PREMSelected
        {
            get
            {
                return (CurrentActionCode == "PREM");
            }
        }

        //NM & IP - 07/01/09 - CR976 - Extra Telephone Actions - STW (Send To WriteOff Strategy)
        public bool STWSelected
        {
            get
            {
                return (CurrentActionCode == "STW");
            }
        }

        //NM & IP - 07/01/09 - CR976 - Extra Telephone Actions - LEG (Legal Details)
        public bool LEGSelected
        {
            get
            {
                return (CurrentActionCode == "LEG");
            }
        }

        //jec - 29/05/09 - PTP (Promise to Pay)
        public bool PTPSelected
        {
            get
            {
                return (CurrentActionCode == "PTP");
            }
        }

        //NM & IP - 07/01/09 - CR976 - Extra Telephone Actions - INS (Insurance Detail)
        public bool INSSelected
        {
            get
            {
                return (CurrentActionCode == "IND");        //UAT866 jec - Insurance Detail is IND (not INS)
            }
        }

        //NM & IP - 07/01/09 - CR976 - Extra Telephone Actions - FRD (Fraud Detail)
        public bool FRDSelected
        {
            get
            {
                return (CurrentActionCode == "FRD");
            }
        }

        //NM & IP - 07/01/09 - CR976 - Extra Telephone Actions - (RFC - Blacklist Customer)
        public bool RFCSelected
        {
            get
            {
                return (CurrentActionCode == "RFC");
            }
        }

        //NM & IP - 07/09/01 - CR976 - Extra Telephone Actions - (TRC - Enter Trace Details)
        public bool TRCSelected
        {
            get
            {
                return (CurrentActionCode == "TRC");
            }
        }

        //NM 14/07/09 - Assign Trace of Publication (Walkthrough changes)
        public bool TPUBSelected
        {
            get
            {
                return (CurrentActionCode == "TPUB");
            }
        }

        //IP - 28/08/09 - UAT(737) - Action Code 'RBDW' (Reverse BDW)
        public bool RBDWSelected
        {
            get
            {
                return (CurrentActionCode == "RBDW");
            }
        }

        //IP - 27/09/10 - UAT(36)UAT5.4 - Action Code 'INFO' (Additional Information Required)
        public bool InfoSelected
        {
            get
            {
                return (CurrentActionCode == "INFO");
            }
        }


        //NM & IP - 06/01/09 - CR976 - Check if the action selected is a worklist
        private bool SendToWorklist = false;
        public bool initialised = false;    //inhibit data get when top500 checkbox initially set
        public bool prevTop500 = false;
        public string prevWorklist = null;
        public bool initTop500 = false;
        public bool initWorklist = false;

        public TelephoneAction5_2(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
        }

        public TelephoneAction5_2(Form root, Form parent, bool telAction)
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
            }
            else
            {
                // Bailiff Review - always allow re-allocation
                this.Text = GetResource("T_BAILREVIEW");
                //this.gbReAllocate.Visible = true;
            }

            InitialiseStaticData();
            //lAccountNo.Visible = txtAccountNo.Enabled;
            InitScreen();

            dtLegalDetail = CreateLegalDetailDT();
            dtInsuranceDetail = CreateInsuranceDetailDT();
            dtFraudDetail = CreateFraudDetailDT();

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });

            //Enable tel action if permissions available.
            AuthoriseCheck AC = new AuthoriseCheck("TelephoneAction5_2", "PhoneNumbers");

            if (AC.ControlPermissionCheck(Credential.User).HasValue)
            {
                txtHDialCode.ReadOnly = false;
                txtHome.ReadOnly = false;
                txtWDialCode.ReadOnly = false;
                txtWork.ReadOnly = false;
                txtMDialCode.ReadOnly = false;
                txtMobile.ReadOnly = false;
                btnSaveTelephone.Enabled = true;
            }
        }

        private void InitialiseStaticData()
        {
            try
            {
                Function = "BStaticDataManager::GetDropDownData";
                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                StringCollection empTypes = new StringCollection();
                empTypes.Add("Staff Types");
                StringCollection empRATypes = new StringCollection();
                empRATypes.Add("Staff Types");
                StringCollection actions = new StringCollection();
                actions.Add("Actions");
                //StringCollection branchNos = new StringCollection(); 	
                //branchNos.Add("ALL");

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

                //NM & IP - 07/01/09 - CR976 - Extra Telephone Actions - STW (Send To WriteOff Strategy)
                //Retrieve the writeoff reasons.
                if (StaticData.Tables[TN.WriteOffCodes] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.WriteOffCodes, null));


                //NM & IP - 07/01/09 - CR976 - Get the 'Insurance Types' to populate the
                //Insurance Types drop down on the 'Insurance Details' screen.
                if (StaticData.Tables[TN.InsuranceTypes] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.InsuranceTypes, null));


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
                    empTypes.Add(str.ToUpper());
                    empRATypes.Add(str.ToUpper());
                }


                dtLastActionDate.Value = Date.blankDate;

                drpReason.DataSource = (DataTable)StaticData.Tables[TN.Reasons];
                drpReason.DisplayMember = CN.CodeDescription;
                drpReason.ValueMember = CN.Code;

                cmbBranch.DataSource = (DataTable)StaticData.Tables[TN.BranchNumber];
                cmbBranch.DisplayMember = CN.BranchNo;
                cmbBranch.ValueMember = CN.BranchNo;

                //CR852
                if (showMultipleAccounts.Enabled == false)
                {
                    //NM & IP - 23/12/08 - CR976 
                    //tabAccountList.Visible = false;
                    chkTop500.Enabled = false;      //jec   UAT936

                    tabAccounts.TabPages.Remove(tabAccountList);
                    //tabAccountList.Enabled = false;
                    dgAccounts.Enabled = false;
                    dgAccounts.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void PopulateActions()
        {
            string strategy = drpStrategies.SelectedValue == null ? String.Empty : drpStrategies.SelectedValue.ToString();

            //NM & IP - 23/12/08 - CR976 - Retrieve the actions that the user has rights to perform
            //based on the strategy that the account is in.
            DataSet strategyActionsForEmployee = CollectionsManager.GetStrategyActionsForEmployee(Credential.UserId, strategy, true, out Error);
            DataTable dtStrategyActions = strategyActionsForEmployee.Tables[0];



            if (dtStrategyActions.Rows.Count == 0)
            {
                drpActionCode.Text = String.Empty;
                //IP - 26/09/08 - UAT5.2 - UAT(529) - Display a message if the user does not have any worklists
                //allocated to them for the selected strategy that have actions setup for them.
                ((MainForm)this.FormRoot).statusBar1.Text = "You do not have any actions assigned to you for the strategy that the account is in";
            }

            //UAT(5.2) - 754
            if (dtStrategyActions.Select("ActionCode = 'RALL'").Length > 0)
                dtStrategyActions.Rows.Remove(dtStrategyActions.Select("ActionCode = 'RALL'")[0]);

            drpActionCode.DataSource = dtStrategyActions;
            drpActionCode.DisplayMember = CN.ActionDescription;
            drpActionCode.ValueMember = CN.ActionCode;
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

        private void HashMenus()
        {
            //dynamicMenus[this.Name+":txtAccountNo"] = this.txtAccountNo;
            //dynamicMenus[this.Name+":btnDeAllocate"] = this.btnDeAllocate;
            dynamicMenus[this.Name + ":drpStrategies"] = this.drpStrategies;
            dynamicMenus[this.Name + ":showMultipleAccounts"] = this.showMultipleAccounts;
            dynamicMenus[this.Name + ":btnAddCode"] = this.btnAddCode;
            dynamicMenus[this.Name + ":chkApplyAllAccounts"] = this.chkApplyAllAccounts;       //CR1084 UAT28
            dynamicMenus[this.Name + ":btnReferences"] = this.btnReferences;               //CR1084 UAT28

        }

        private void InitScreen()
        {
            //drpBranch.Text = Config.BranchCode;
            //drpEmpType.SelectedIndex = 0;
            //drpEmpName.Text = "";
            //txtAccountNo.Text = "000-0000-0000-0";
            //drpRAEmpName.Text = "";
            //drpRAEmpType.SelectedIndex = 0;

            //IP - 21/10/08 - UAT5.2 - UAT(551) - Disable the strategy drop down so that this cannot
            //be changed by the user.
            drpStrategies.Enabled = false;

            InitAccountList();
            //EnableCriteria(true);

            //errorProvider1.SetError(drpEmpName, "");
            //errorProvider1.SetError(txtAccountNo, "");
            errorProvider1.SetError(dtDueDate, "");

            ((MainForm)this.FormRoot).statusBar1.Text = "";
        }

        //private void InitLastAction()
        //{
        //    gbLastAction.Enabled = false;
        //    dtLastActionDate.Value = Date.blankDate;
        //    txtLastActionCode.Text = "";
        //}

        private void InitNewAction()
        {
            gbNewAction.Enabled = true;
            txtActionValue.Text = "0";
            txtNotes.Text = "";
            dtDueDate.Value = DateTime.Today;
            //drpActionCode.SelectedIndex = 0;
            drpActionCode.SelectedIndex = -1;
            btnSave.Enabled = false;        //CR1084
            //btnPrint.Enabled = false;
        }

        private void InitAccountList()
        {
            //InitLastAction();
            InitNewAction();
            ((MainForm)this.FormRoot).statusBar1.Text = "";
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            DateTime spaDateExpiry = DateTime.Now;
            string spaReasonCode = string.Empty;
            double spaInstal = 0;
            int originalDataRow = DataViewNo;

            errorProvider1.SetError(txtNotes, "");
            errorProvider1.SetError(dtpReminderTime, "");

            try
            {
                Wait();

                int actionMinNotesLength = 0;
                int maxDaysAllowedForReminder = 0;

                Int32.TryParse(Country[CountryParameterNames.MaxReminderDaysInAdv].ToString(), out maxDaysAllowedForReminder);

                //NM & IP - 29/12/08 - CR976 
                //If the characters entered in the Notes field is less than the minimum notes
                //length required for the selected action then error handling is used
                //to inform the user of the minimum notes length required to save the selected
                //action.
                if (drpActionCode.DataSource != null && drpActionCode.SelectedIndex >= 0)
                {
                    actionMinNotesLength = Convert.ToInt32(((DataTable)drpActionCode.DataSource).Rows[drpActionCode.SelectedIndex][CN.MinNotesLength]);
                }

                if (txtNotes.Text.Trim().Length < actionMinNotesLength)
                {
                    errorProvider1.SetError(txtNotes, GetResource("M_MINNOTESLENGTH", actionMinNotesLength));
                }

                //NM & IP - CR976 - Extra Telephone Actions - (PTP - Mandatory fields)
                //If 'PTP' is selected then the 'txtNotes' must be mandatory

                if (txtNotes.Text.Trim().Length == 0 && drpActionCode.SelectedValue.ToString() == "PTP")
                {
                    errorProvider1.SetError(txtNotes, GetResource("M_ENTERMANDATORY"));
                }

                //NM & IP - CR976 - Extra Telephone Actions - (PTP - Mandatory fields)
                //If 'PTP' is selected then the 'txtAction' value must be > 0
                if (Convert.ToDecimal((StripCurrency(txtActionValue.Text))) <= 0 && drpActionCode.SelectedValue.ToString() == "PTP")
                {
                    errorProvider1.SetError(txtActionValue, GetResource("M_GREATERTHANZERO"));
                }
                else
                {
                    errorProvider1.SetError(txtActionValue, "");
                }

                //NM & IP - 05/01/09 - CR976 - If the Reminder Date is later than the no of daysallowed in Country Parameter
                if ((dtDueDate.Value - DateTime.Today).Days > maxDaysAllowedForReminder && (PREMSelected || REMSelected))
                {
                    errorProvider1.SetError(dtpReminderTime, GetResource("M_MAXDAYSALLOWED", maxDaysAllowedForReminder));
                }
                else
                {
                    errorProvider1.SetError(dtpReminderTime, "");
                }

                //NM & IP - 07/01/09 - CR976 - If extra action information table doesn't have any info
                if (LEGSelected && dtLegalDetail.Rows.Count == 0)
                {
                    errorProvider1.SetError(txtNotes, GetResource("M_LEGDETAILSREQUIRED"));
                }
                else if (INSSelected && dtInsuranceDetail.Rows.Count == 0)
                {
                    errorProvider1.SetError(txtNotes, GetResource("M_INSDETAILSREQUIRED"));
                }
                else if (FRDSelected && dtFraudDetail.Rows.Count == 0)
                {
                    errorProvider1.SetError(txtNotes, GetResource("M_FRDDETAILSREQUIRED"));
                }

                //NM & IP - 30/12/08 - CR976 - If the Telephone Numbers have been changed by the user
                //but have not yet been saved, need to prompt the user to save the changes.

                if (txtHDialCode.Text != txtHDialCode.Tag.ToString() || txtHome.Text != txtHome.Tag.ToString() ||
                   txtWDialCode.Text != txtWDialCode.Tag.ToString() || txtWork.Text != txtWork.Tag.ToString() ||
                   txtMDialCode.Text != txtMDialCode.Tag.ToString() || txtMobile.Text != txtMobile.Tag.ToString())
                {
                    //If the user selects to save the changes call the event that is called
                    //when saving the telephone numbers.
                    if (DialogResult.Yes == ShowInfo("M_SAVECHANGESTELEPHONE", MessageBoxButtons.YesNo))
                    {
                        if (SavedTelephoneNumbers() == false)
                            return;
                    }
                }

                ValidateDueDate();
                //If no errors .. do the update..
                if (errorProvider1.GetError(dtDueDate).Length == 0
                    && errorProvider1.GetError(txtActionValue).Length == 0
                    && errorProvider1.GetError(txtNotes).Length == 0
                    && errorProvider1.GetError(dtpReminderTime).Length == 0)
                {

                    int accountRow = m_accountRow;

                    //IP - 07/10/08 - UAT5.2 - UAT(522) - Changed .SelectedIndex != -1 as was previously
                    //set to 0, meaning that the first item in the drop down was excluded.
                    if (m_valid_account && accountRow >= 0 && drpActionCode.SelectedIndex != -1)
                    {
                        string notes = txtNotes.Text;
                        double actionValue = Convert.ToDouble(StripCurrency(txtActionValue.Text));
                        DateTime dueDate = dtDueDate.Value;
                        DateTime reminderDateTime = new DateTime(1900, 1, 1);
                        bool cancelOutstandingReminders = chkCancelReminders.Checked;
                        DataSet dsExtraActionDetail = new DataSet();


                        if (SPASelected)
                        {
                            dueDate = Convert.ToDateTime(dtSPADetails.Rows[0][CN.ReviewDate]);

                        }

                        //IP - 21/10/08 - UAT5.2 - UAT(551)
                        //If the 'Action Code', 'STS - Send To Strategy' has been selected then strategy to send to will be passed
                        //
                        else if (STSSelected)
                        {
                            //IP - 11/06/09 - Credit Collection Walkthrough Changes - added the description of the strategy account being
                            //sent, to the notes.
                            int index = drpSendToStrategy.SelectedIndex;

                            string strategyDescription = Convert.ToString(((DataTable)drpSendToStrategy.DataSource).Rows[index][CN.Description]);
                            notes = notes + " " + "Sent to strategy:" + " " + strategyDescription;

                            // Update the 'CMStrategyAcct' table and the 'CMWorklistsacct' table
                            //NM & IP - 06/01/09 - CR976 - If we are sending the account to a strategy then do the following
                            CollectionsManager.UpdateStrategyAccounts(drpSendToStrategy.SelectedValue.ToString(), Acct, string.Empty, string.Empty, Credential.UserId, out Error);

                            // Send all accounts to same strategy here!!!
                            if (chkApplyAllAccounts.Checked == true)
                            {
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    string accountNumber = Convert.ToString(arr[i]);
                                    CollectionsManager.UpdateStrategyAccounts(drpSendToStrategy.SelectedValue.ToString(), accountNumber, string.Empty, string.Empty, Credential.UserId, out Error);

                                }
                            }

                            if (Error.Length > 0)
                            {
                                ShowError(Error);
                            }
                        }

                        //NM & IP - 07/01/09 - CR976 - Extra Telephone Actions - STW (Send To WriteOff Strategy)
                        else if (STWSelected)
                        {
                            //Need to pass in the reason code, and employee
                            CollectionsManager.UpdateStrategyAccounts("WOF", Acct, string.Empty, drpReason.SelectedValue.ToString(), Credential.UserId, out Error);

                            if (chkApplyAllAccounts.Checked == true)        //CR1084 
                            {
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    string accountNumber = Convert.ToString(arr[i]);
                                    CollectionsManager.UpdateStrategyAccounts("WOF", accountNumber, string.Empty, drpReason.SelectedValue.ToString(), Credential.UserId, out Error);
                                }
                            }


                            if (Error.Length > 0)
                            {
                                ShowError(Error);
                            }
                        }



                        else if (REMSelected || PREMSelected)
                        {
                            reminderDateTime = Convert.ToDateTime(Convert.ToString((dtDueDate.Value.ToShortDateString() + " " + dtpReminderTime.Value.ToShortTimeString())));
                        }
                        else if (LEGSelected || INSSelected || FRDSelected || TRCSelected)
                        {
                            if (LEGSelected)
                            {
                                dtLegalDetail.Rows[0][CN.CMAcctno] = Acct;
                                dtLegalDetail.Rows[0][CN.CMEmpeeno] = Credential.UserId.ToString();
                                dsExtraActionDetail.Tables.Add(dtLegalDetail.Copy());
                            }
                            else if (INSSelected)
                            {
                                dtInsuranceDetail.Rows[0][CN.CMAcctno] = Acct;
                                dtInsuranceDetail.Rows[0][CN.CMEmpeeno] = Credential.UserId.ToString();
                                dsExtraActionDetail.Tables.Add(dtInsuranceDetail.Copy());
                            }
                            else if (FRDSelected)
                            {
                                dtFraudDetail.Rows[0][CN.CMAcctno] = Acct;
                                dtFraudDetail.Rows[0][CN.CMEmpeeno] = Credential.UserId.ToString();
                                dsExtraActionDetail.Tables.Add(dtFraudDetail.Copy());
                            }
                            else if (TRCSelected)
                            {
                                dtTRCDetail.Rows[0][CN.CMAcctno] = Acct;
                                dtTRCDetail.Rows[0][CN.CMEmpeeno] = Credential.UserId.ToString();
                                dsExtraActionDetail.Tables.Add(dtTRCDetail.Copy());
                                dueDate = Convert.ToDateTime(dtTRCDetail.Rows[0][CN.CMTRCInitiatedDate]);
                            }

                        }

                        //IP - 11/06/09 - Credit Collection Walkthrough Changes
                        if (LEGSelected || INSSelected || FRDSelected || TRCSelected)
                        {
                            CollectionsManager.SaveBailActionsWithTelephoneActions(Acct, Credential.UserId, CurrentActionCode, notes,
                                dueDate, actionValue, spaDateExpiry, spaReasonCode, spaInstal, reminderDateTime, cancelOutstandingReminders, dsExtraActionDetail, "TELACTION", out Error);

                            if (chkApplyAllAccounts.Checked == true)        //CR1084 
                            {
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    string accountNumber = Convert.ToString(arr[i]);
                                    CollectionsManager.SaveBailActionsWithTelephoneActions(accountNumber, Credential.UserId, CurrentActionCode, notes,
                                    dueDate, actionValue, spaDateExpiry, spaReasonCode, spaInstal, reminderDateTime, cancelOutstandingReminders,
                                    dsExtraActionDetail, "TELACTION", out Error);

                                }
                            }
                        }
                        else
                        {
                            AccountManager.SaveBailActions(Acct, Credential.UserId, CurrentActionCode, notes,
                                dueDate, actionValue, spaDateExpiry, spaReasonCode, spaInstal, reminderDateTime, cancelOutstandingReminders, "TELACTION", out Error);

                            if (chkApplyAllAccounts.Checked == true)        //CR1084 
                            {
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    string accountNumber = Convert.ToString(arr[i]);
                                    AccountManager.SaveBailActions(accountNumber, Credential.UserId, CurrentActionCode, notes,
                                    dueDate, actionValue, spaDateExpiry, spaReasonCode, spaInstal, reminderDateTime, cancelOutstandingReminders, "TELACTION", out Error);
                                }
                            }
                        }


                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                        }
                        else
                        {

                            if (SendToWorklist)
                            {
                                //IP - 11/06/09 - Credit Collection Walkthrough Changes - added the description of the worklist account being
                                //sent, to the notes. AA 15/07/11 Send to Worklist date has to be after Bailaction Save date.

                                int index = drpActionCode.SelectedIndex;
                                string worklistDescription = Convert.ToString(((DataTable)drpActionCode.DataSource).Rows[index][CN.ActionDescription]);

                                notes = notes + " " + "Sent to worklist:" + " " + worklistDescription;

                                CollectionsManager.UpdateStrategyAccounts(string.Empty, Acct, drpActionCode.SelectedValue.ToString(), string.Empty, Credential.UserId, out Error);

                                if (chkApplyAllAccounts.Checked == true)        //CR1084 
                                {
                                    for (int i = 0; i < arr.Length; i++)
                                    {
                                        string accountNumber = Convert.ToString(arr[i]);
                                        CollectionsManager.UpdateStrategyAccounts(string.Empty, accountNumber, drpActionCode.SelectedValue.ToString(), string.Empty,
                                            Credential.UserId, out Error);

                                    }
                                }
                                if (Error.Length > 0)
                                {
                                    ShowError(Error);
                                }
                            }
                            //IP - 10/09/09 - UAT(848) - The below code has now been moved into the below method.
                            CycleToNextAccount();


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

            //NM & IP - 29/12/08 - CR976 - Initialise the text boxes that will display the 
            //customers address.
            txtAdd1.Text = string.Empty;
            txtAdd2.Text = string.Empty;
            txtAdd3.Text = string.Empty;
            txt_provisions.Text = string.Empty;
            txtPostCode.Text = string.Empty;
            txtHDialCode.Text = string.Empty;
            txtWDialCode.Text = string.Empty;
            txtMDialCode.Text = string.Empty;

            txtHDialCode.Tag = string.Empty;
            txtWDialCode.Tag = string.Empty;
            txtMDialCode.Tag = string.Empty;

            errorProviderForWarning.SetError(lblDummyForErrorProvider, "");
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


        private void dgAccounts_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (dgAccounts.CurrentRowIndex < 0)
                    return;

                Wait();

                btnSave.Enabled = true;
                DataView dv = (DataView)dgAccounts.DataSource;

                for (int i = dv.Count - 1; i >= 0; i--)
                {
                    // Only interested in selected rows
                    if (dgAccounts.IsSelected(i))
                    {
                        DataViewNo = i;
                        //Unlock previous account
                        UnlockAccount();
                        PopulateFields(DataViewNo);
                        break;
                    }
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

        private int LockAccount()
        {
            // Lock Account
            Function = "LockAccount()";

            AccountManager.LockAccount(Acct, Credential.UserId.ToString(), out Error);
            return Error.Length > 0 ? 1 : 0;
        }

        private void UnlockAccount()
        {
            try
            {
                CollectionsManager.UnlockAccount(Acct, Credential.UserId, out Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private void btnLoadExtraInfo_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                if (SPASelected)
                {
                    if (dgAccounts.CurrentRowIndex >= 0)
                    {
                        //string acctNo = (string)dgAccounts[accountRow, 0];
                        SPADetails dialog = new SPADetails(Acct);
                        dialog.FormRoot = this.FormRoot;
                        dialog.FormParent = this;
                        dialog.ShowDialog(this);
                    }
                }
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

            }
            catch (Exception ex)
            {
                Catch(ex, "btnLoadExtraInfo_Click");
            }
            finally
            {
                StopWait();
            }
        }


        private void drpActionCode_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            //NM & IP - 08/01/09 - CR976 - Extra Telephone Actions
            //Don't Clear the notes field if an action is changed.
            //txtNotes.Text = string.Empty;

            errorProvider1.SetError(txtNotes, "");
            errorProvider1.SetError(dtpReminderTime, "");
            errorProvider1.SetError(drpActionCode, "");
            //IP - 20/10/08 - UAT5.2 - UAT(551)
            var dtStrategiesToSendTo = new DataTable();
            string strategy = string.Empty;
            var stratConfig = new Collections.CollectionsClasses.StrategyConfigPopulation();
            btnLoadExtraInfo.Visible = false; //NM - 07/01/09 - hiding the button,only visible for SPA,LEG, INS, FRD
            btnSave.Enabled = true;     //UAT847 jec 11/09/09
            chkApplyAllAccounts.Enabled = Credential.HasPermission(CosacsPermissionEnum.TelephoneActionAllToAllAccounts);
            chkApplyAllAccounts.Checked = false;

            var spaDateExpiry = DateTime.Now;
            string spaReasonCode = string.Empty;
            double spaInstal = 0;
            string notes = txtNotes.Text;
            double actionValue = Convert.ToDouble(StripCurrency(txtActionValue.Text));
            DateTime dueDate = dtDueDate.Value;
            var reminderDateTime = new DateTime(1900, 1, 1);
            bool cancelOutstandingReminders = chkCancelReminders.Checked;
            // jec 29/05/09 - only show action value for PTP
            if (PTPSelected)
            {
                txtActionValue.Visible = lbActionValue.Visible = true;
                chkApplyAllAccounts.Checked = false;        //CR1084 PTP only allowed on selected account
                chkApplyAllAccounts.Enabled = false;
            }
            else
            {
                txtActionValue.Visible = lbActionValue.Visible = false;
            }

            try
            {
                Wait();
                if (drpActionCode.SelectedIndex >= 0)
                {
                    //NM & IP - 06/01/09 - CR976 - Check if the action selected is a worklist
                    SendToWorklist = Convert.ToBoolean(((DataTable)drpActionCode.DataSource).Rows[drpActionCode.SelectedIndex][CN.IsWorklist]);

                    lbActionDate.Text = "Due Date";
                    lbActionValue.Text = "Action Value";

                    if (SPASelected)
                    {
                        if (Acct.Substring(3, 1) == "9")            // #9824 not allowed on storecard
                        {
                            errorProvider1.SetError(drpActionCode, "Special Arrangements cannot be made on StoreCard accounts");
                        }
                        else
                        {
                            btnSave.Enabled = false;        //UAT847 jec 11/09/09
                            btnLoadExtraInfo.Text = "SPA History";
                            btnLoadExtraInfo.Visible = true;
                            lbActionDate.Text = "Expiry Date"; //IP - 29/09/08 - Special Arrangement change - comment this out
                            lbActionValue.Text = "Instalment"; //IP - 29/09/08 - Special Arrangement change - comment this out
                            chkApplyAllAccounts.Checked = false;        //CR1084 Spa only allowed on selected account
                            chkApplyAllAccounts.Enabled = false;

                            //IP - 29/09/08 - If 'SPA' has been selected then display the 'Special Arrangements' screen.
                            //IP & JC - 12/01/09 - CR976 - Special Arrangements - Changed to pass in the CustomerID of the selected account.
                            //IP - 28/04/10 - UAT(983) UAT5.2 - Added CreditBlocked to the parameter list.
                            var sparr = new SpecialArrangements(Acct, CustID, CustomerName, CreditBlocked, (DataTable)StaticData.Tables[TN.Reasons]);
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
                                    refinance.spaPrint = true;          //UAT1012  jec
                                    refinance.printSchedule = Convert.ToBoolean(dtSPADetails.Rows[0]["PrintSched"]);          //UAT1012  jec
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

                    }
                    // IP - 20/10/08 - UAT5.2 - UAT(551)
                    // If 'Action Code' - 'Send To Strategy' has been selected then do the following.
                    else if (STSSelected)
                    {
                        //IP - 21/10/08 - UAT5.2 - UAT(551)
                        //Pass in the strategy that the account belongs to, to return the 
                        //list of strategies that are the 'exit' strategies for accounts
                        //in the selected strategy.
                        strategy = drpStrategies.SelectedValue.ToString();

                        if (showMultipleAccounts.Enabled) //--NM : As SP & KL requested -- Email from SP on 19/08/2009
                        {
                            dtStrategiesToSendTo = drpStrategies.DataSource != null ? ((DataTable)drpStrategies.DataSource).Copy() : stratConfig.GetStrategies();

                        }
                        else
                        {
                            dtStrategiesToSendTo = stratConfig.GetStrategiesToSendTo(strategy);
                        }

                        //IP - 09/07/10 - UAT1042 - UAT5.2 - Moved from above.
                        DataRow[] drArray = dtStrategiesToSendTo.Select(CN.Strategy + " = '" + strategy + "'");
                        if (drArray.Length > 0) //UAT(5.2) - 833
                        {
                            dtStrategiesToSendTo.Rows.Remove(drArray[0]); //Removing the current strategy already selected in the strategy dropdown
                        }

                        //IP - 09/07/10 - UAT1042 - UAT5.2 - Remove WOF from the Send to Strategy as there is already a STW (Send To WriteOff) action.
                        drArray = dtStrategiesToSendTo.Select(CN.Strategy + " = 'WOF'");
                        if (drArray.Length > 0)
                        {
                            dtStrategiesToSendTo.Rows.Remove(drArray[0]);
                        }

                        //Do not display the 'Action Value' field.
                        txtActionValue.Visible = false;
                        lbActionValue.Text = "Select Strategy"; // UAT 825 change lable

                        //Populate the drop down that will be used to select the strategy to send
                        //the account to.
                        drpSendToStrategy.DataSource = dtStrategiesToSendTo;
                        drpSendToStrategy.DisplayMember = CN.Description;
                        drpSendToStrategy.ValueMember = CN.Strategy;
                    }
                    //NM & IP - 31/12/08 - CR976 - If the 'REM' - set reminder date & time
                    //action code has been selected then hide the 'Action Value' field and
                    //change the label text.
                    else if (REMSelected || PREMSelected)
                    {
                        //Do not display the 'Action Value' field.
                        txtActionValue.Visible = false;
                        drpSendToStrategy.Visible = false;
                        lbActionValue.Text = "Set reminder date & time";
                        dtpReminderTime.Value = DateTime.Today;
                    }
                    //NM & IP - 06/01/09 - CR976 - Extra Telephone Actions - Send to supervisor worklist
                    //if manually sending an account to a worklist
                    else if (SendToWorklist)
                    {
                        txtActionValue.Visible = false;
                        lbActionValue.Visible = false;
                    }
                    //NM & IP - 07/01/09 - CR976 - Extra Telephone Actions - STW (Send To WriteOff Strategy)
                    else if (STWSelected)
                    {
                        DataView dvWriteOffReasons = new DataView();

                        DataTable dtWriteOffReasons = (DataTable)StaticData.Tables[TN.WriteOffCodes];

                        dvWriteOffReasons = dtWriteOffReasons.DefaultView;
                        dvWriteOffReasons.RowFilter = CN.Category + " = 'BDM'";

                        //Display the writeoff reasons in the 'Reasons' drop down.
                        drpReason.DataSource = dvWriteOffReasons;
                        drpReason.DisplayMember = CN.CodeDescript;
                        drpReason.ValueMember = CN.Code;
                        lbActionValue.Visible = false;
                        txtActionValue.Visible = false;
                    }
                    //NM & IP - 07/01/09 - CR976 - Extra Telephone Actions - RFC(Blacklist Customer)
                    else if (RFCSelected)
                    {
                        //Do not display the 'Due Date' field and label if 'RFC' (blacklist customer)
                        //action code has been selected.
                        dtDueDate.Visible = false;
                        lbActionDate.Visible = false;
                        txtActionValue.Visible = false;
                        lbActionValue.Visible = false;
                    }
                    //NM & IP - 07/01/09 - CR976 - Extra Telephone Actions - TRC(Enter Trace Details)
                    else if (TRCSelected)
                    {
                        btnLoadExtraInfo.Text = "Trace Details";
                        btnLoadExtraInfo.Visible = true;

                        try
                        {
                            Wait();
                            var dialog = new TelAction_TRC(dtTRCDetail);
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
                        txtActionValue.Visible = false;
                        lbActionValue.Visible = false;
                        dtDueDate.Visible = false;
                        lbActionDate.Visible = false;
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
                            var dialog = new TelAction_INS(dtInsuranceDetail);
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
                            var dialog = new TelAction_FRD(dtFraudDetail);
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
                    else if (TPUBSelected)
                    {
                        lbActionDate.Text = "Publication Date";
                    }
                    else if (RBDWSelected) //IP - 28/08/09 - UAT(737) - Load the Refunds & Corrections screen to process the RDBW.
                    {
                        reversedBDW = false;
                        try
                        {
                            //jec - 08/07/10 - UAT1065 - Load General Financial Transactions screen
                            var gft = new GeneralFinancialTransactions(this.FormRoot, this, Acct);
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
                }

                //NM - 28/12/2008 
                if (drpActionCode.DataSource != null && drpActionCode.SelectedIndex >= 0)
                    chCycleNext.Checked = ((DataTable)drpActionCode.DataSource).Rows[drpActionCode.SelectedIndex][CN.CycleToNextFlag].ToString() == "True";

                chCycleNext.Enabled = chCycleNext.Checked;

                //lbReason.Visible = SPASelected; //IP - 29/09/08 - Special Arrangement change - comment this out
                //drpReason.Visible = SPASelected; //IP - 29/09/08 - Special Arrangement change - comment this out
                //txtActionValue.Visible =  !STSSelected; //IP - 20/10/08 - UAT5.2 - UAT(551) 

                drpSendToStrategy.Visible = STSSelected; //IP - 20/10/08 - UAT5.2 - UAT(551) - display this drop down if the 'STS Action Code' has been selected.
                drpSendToStrategy.Enabled = STSSelected; //IP - 20/10/08 - UAT5.2 - UAT(551) - enable this drop down if the 'STS Action Code' has been selected.

                //NM & IP - 02/01/09 - CR976 - Only display the reminder time picker if the following are 
                //selected.
                dtpReminderTime.Enabled = REMSelected || PREMSelected;
                dtpReminderTime.Visible = REMSelected || PREMSelected;


                //NM & IP - 07/01/09 - CR976 - Extra Telephone Actions - STW (Send To WriteOff Strategy)
                //Only display the 'Reason' drop down if 'STW' selected.
                drpReason.Visible = STWSelected;
                lbReason.Visible = STWSelected;     //CR1084  jec

                //NM & IP - 31/12/08 - CR976 
                // NM & IP - 06/01/09 - CR976 - or do not display if manually sending to worklist
                // jec 29/05/09 - only display for PTP
                //txtActionValue.Visible = !(REMSelected || PREMSelected || SendToWorklist || RFCSelected || STSSelected || STWSelected || TRCSelected);
                //lbActionValue.Visible = !(SendToWorklist || RFCSelected || STWSelected || TRCSelected);

                //NM & IP - 07/01/09 - CR976
                dtDueDate.Visible = !(RFCSelected || TRCSelected || InfoSelected); //IP - 27/09/10 - UAT(36) UAT5.4 - Added InfoSelected
                lbActionDate.Visible = !(RFCSelected || TRCSelected || InfoSelected); //IP - 27/09/10 - UAT(36) UAT5.4 - Added InfoSelected
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

        private void TelephoneAction5_2_Load(object sender, EventArgs e)
        {
            try
            {
                Wait();

                //Populate the Strategy drop down
                var stratConfig = new Collections.CollectionsClasses.StrategyConfigPopulation();
                var dtStrategies = new DataTable();
                dtStrategies = stratConfig.GetStrategies();

                drpStrategies.DataSource = dtStrategies;
                drpStrategies.DisplayMember = CN.Description;
                drpStrategies.ValueMember = CN.Strategy;
                drpStrategies.Visible = true;

                chkTop500.Checked = true; //IP - 12/11/09 - UAT(882) 

                btnReferences.Enabled = Credential.HasPermission(CosacsPermissionEnum.TelephoneActionReviewButton);
                chkApplyAllAccounts.Enabled = Credential.HasPermission(CosacsPermissionEnum.TelephoneActionAllToAllAccounts);

                if (dvAccounts.Count > 0)
                {
                    Acct = dvAccounts[0][CN.AcctNo].ToString();      //CR1084 jec
                    // If  first account locked - cycle to next account
                    //if (LockAccount() != 0)     //CR1084 jec
                    //{
                        m_accountRow = -1;
                        CycleToNextAccount();
                    //}


                    //NM Setting the dummyLable's width to 0 (to hide from the screen)
                    lblDummyForErrorProvider.Width = 0;

                    txtHDialCode.Tag = txtHDialCode.Text;
                    txtWDialCode.Tag = txtWDialCode.Text;
                    txtMDialCode.Tag = txtMDialCode.Text;
                    btn_calc.Enabled = true;       // #14606
                }
                else
                {
                    drpStrategies.Text = "No Accounts Loaded";
                    drpActionCode.Enabled = false;
                    btnSave.Enabled = false;
                    btnSaveTelephone.Enabled = false;
                    btn_calc.Enabled = false;       // #14606
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "TelephoneAction5_2_Load");
            }
            finally
            {
                StopWait();
            }
        }

        private void PopulateFields(int index)
        {
            decimal agreementTotal = 0.0M;
            decimal arrears = 0.0M;
            decimal balance = 0.0M;
            decimal instalment = 0.0M;
            double monthsArrears = 0.0F;
            m_valid_account = false;

            InitAccountList();

            gbLastAction.Enabled = true;
            gbNewAction.Enabled = true;
            btnSave.Enabled = true;

            if (dvAccounts.Count != 0 && index >= 0 && dvAccounts.Count > index)
            {

                dtLastActionDate.Value = Convert.ToDateTime(dvAccounts[index][CN.DateAdded]);
                var maxActionDays = Convert.ToInt32(Country[CountryParameterNames.NoOfDaysSinceAction]);
                if (dtLastActionDate.Value > DateTime.Now.AddHours(-1)) //ffgg
                    //errorProviderForWarning.SetError(lblDummyForErrorProvider, 
                    errorProviderForWarning.SetError(dtLastActionDate, "Account has had Action within last hour!!! - ask for a new worklist");
                else
                    if (dtLastActionDate.Value > DateTime.Now.AddDays(-1))
                        errorProviderForWarning.SetError(dtLastActionDate, "Account has had Action within last day - ask for a new worklist");
                    else
                        if (dtLastActionDate.Value > DateTime.Now.AddDays(-maxActionDays))
                            errorProviderForWarning.SetError(dtLastActionDate, "Account has had a recent action within the last " + Convert.ToString(maxActionDays) + " Day");
                        else
                        {
                            errorProviderForWarning.Clear();

                        }

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
                //IP - 09/01/09 - Was causing a specified cast exception as datelastpaid was null, therefore if it is null then display nothing.
                txtDateLastPaid.Text = dvAccounts[index][CN.DateLastPaid] != DBNull.Value ? ((DateTime)dvAccounts[index][CN.DateLastPaid]).ToShortDateString() : "";
                txtDueDate.Text = dvAccounts[index][CN.DueDay].ToString();
                txtFirstName.Text = dvAccounts[index][CN.FirstName].ToString();
                txtHome.Text = dvAccounts[index][CN.TelNoHome].ToString();
                txtInstalment.Text = instalment.ToString(DecimalPlaces);
                txtLastActionCode.Text = dvAccounts[index][CN.ActionCode].ToString();
                txtLastName.Text = dvAccounts[index][CN.Name].ToString();
                txtMobile.Text = dvAccounts[index][CN.MobileNo].ToString();
                txtMonthArrears.Text = monthsArrears.ToString();
                txtPercentage.Text = dvAccounts[index][CN.PercentagePaid].ToString();
                txtStatus.Text = dvAccounts[index][CN.CurrStatus].ToString();
                txtTitle.Text = dvAccounts[index][CN.Title].ToString();
                txtWork.Text = dvAccounts[index][CN.TelNoWork].ToString();
                //NM & IP - 29/12/08 - CR976 - Display the Customers 'Home' address
                txtAdd1.Text = dvAccounts[index][CN.Address1].ToString();
                txtAdd2.Text = dvAccounts[index][CN.Address2].ToString();
                txtAdd3.Text = dvAccounts[index][CN.Address3].ToString();
                txt_provisions.Text = Convert.ToDecimal(dvAccounts[index][CN.ProvisionAmount]) > 0 ? Convert.ToDecimal(dvAccounts[index][CN.ProvisionAmount]).ToString(DecimalPlaces) : "0";
                txtPostCode.Text = dvAccounts[index][CN.AddressPC].ToString();
                txtHDialCode.Text = dvAccounts[index][CN.HomeDialCode].ToString();
                txtWDialCode.Text = dvAccounts[index][CN.WorkDialCode].ToString();
                txtMDialCode.Text = dvAccounts[index][CN.MobileDialCode].ToString();
                gbCustomer.Text = "Customer Details - Account " + dvAccounts[index][CN.AcctNo].ToString();      //CR1084 jec
                var referrals = new StringCollection();    //CR1084 jec
                if (dvAccounts[index][CN.ReferralReason].ToString() != "")
                {
                    referrals.Add(dvAccounts[index][CN.ReferralReason].ToString());      //CR1084 jec
                }
                if (dvAccounts[index][CN.ReferralReason2].ToString() != "")
                {
                    referrals.Add(dvAccounts[index][CN.ReferralReason2].ToString());      //CR1084 jec
                }
                if (dvAccounts[index][CN.ReferralReason3].ToString() != "")
                {
                    referrals.Add(dvAccounts[index][CN.ReferralReason3].ToString());      //CR1084 jec
                }
                if (dvAccounts[index][CN.ReferralReason4].ToString() != "")
                {
                    referrals.Add(dvAccounts[index][CN.ReferralReason4].ToString());      //CR1084 jec
                }
                if (dvAccounts[index][CN.ReferralReason5].ToString() != "")
                {
                    referrals.Add(dvAccounts[index][CN.ReferralReason5].ToString());      //CR1084 jec
                }
                if (dvAccounts[index][CN.ReferralReason6].ToString() != "")
                {
                    referrals.Add(dvAccounts[index][CN.ReferralReason5].ToString());      //CR1084 jec
                }

                drpReferReason.DataSource = referrals;           //CR1084 jec
                if (referrals.Count == 0)
                {
                    drpReferReason.Enabled = false;
                }
                else
                {
                    drpReferReason.Enabled = true;
                }

                StageToLaunch.DateProp = Convert.ToDateTime(dvAccounts[index][CN.DateProp]);      //CR1084 jec
                StageToLaunch.AccountNo = dvAccounts[index][CN.AcctNo].ToString();      //CR1084 jec


                //NM & IP - 30/12/08 - CR976 - Need to set the tag properties of the text boxes
                //for there telephone numbers to the original values.
                txtHDialCode.Tag = txtHDialCode.Text;
                txtHome.Tag = txtHome.Text;
                txtWDialCode.Tag = txtWDialCode.Text;
                txtWork.Tag = txtWork.Text;
                txtMDialCode.Tag = txtMDialCode.Text;
                txtMobile.Tag = txtMobile.Text;

                errorProvider1.SetError(txtHDialCode, "");
                errorProvider1.SetError(txtHome, "");
                errorProvider1.SetError(txtWDialCode, "");
                errorProvider1.SetError(txtWork, "");
                errorProvider1.SetError(txtMDialCode, "");
                errorProvider1.SetError(txtMobile, "");

                Acct = dvAccounts[index][CN.AcctNo].ToString();
                m_custid = dvAccounts[index][CN.CustID].ToString();
                m_accttype = dvAccounts[index][CN.AcctType].ToString();
                m_name = dvAccounts[index][CN.Name].ToString();
                m_creditblocked = Convert.ToBoolean(dvAccounts[index][CN.CreditBlocked]); //IP - 28/04/10 - UAT(983) UAT5.2
                m_valid_account = true;


                //if (showMultipleAccounts.Enabled && dgAccounts.CurrentRowIndex >= 0)
                //    m_accountRow = dgAccounts.CurrentRowIndex;
                //else
                m_accountRow = index;


                //NM & IP 02/01/09 - CR976- Bringing call reminder info for this account
                dsCallReminders = CollectionsManager.GetCallReminderInfo(Acct, Credential.UserId, out Error);

                //If there are reminders for the account and the current logged in employee
                //display the group box and set the text boxes to display the reminder date & time
                //and the reminder comments.
                if (dsCallReminders.Tables[0].Rows.Count > 0)
                {
                    gbReminder.Visible = true;
                    txtReminderDateTime.Text = dsCallReminders.Tables[0].Rows[0][CN.ReminderDateTime].ToString();
                    txtReminderComment.Text = dsCallReminders.Tables[0].Rows[0][CN.Comment].ToString();

                }
                else
                {
                    gbReminder.Visible = false;
                }

                lblReferReason.Visible = true;      //CR1084 jec
                drpReferReason.Visible = true;      //CR1084 jec

                //NM & IP - 08/01/09 - CR976 - Need to retrieve any 'Legal', 'Fraud' or 'Insurance'
                //details for the account if any have been entered for the selected account previously.
                var dsLegalFraudInsuranceDetails = new DataSet();
                dsLegalFraudInsuranceDetails = CollectionsManager.GetLegalFraudInsuranceDetails(Acct, out Error);
                if (dsLegalFraudInsuranceDetails.Tables.Count > 2)
                {
                    dtLegalDetail = dsLegalFraudInsuranceDetails.Tables[TN.LegalDetails];
                    dtInsuranceDetail = dsLegalFraudInsuranceDetails.Tables[TN.InsuranceDetails];
                    dtFraudDetail = dsLegalFraudInsuranceDetails.Tables[TN.FraudDetails];
                    dtTRCDetail = dsLegalFraudInsuranceDetails.Tables[TN.TraceDetails];

                    dtLegalDetail.AcceptChanges();
                    dtInsuranceDetail.AcceptChanges();
                    dtFraudDetail.AcceptChanges();
                    dtTRCDetail.AcceptChanges();
                }

                var dsWorklistData = new DataSet();
                dsWorklistData = AccountManager.GetWorklistAccountsData(Acct, Config.StoreType, out Error);

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
                dgStrategies.Columns[CN.Description].Width = 100;

                dgStrategies.Columns[CN.DateFrom].HeaderText = GetResource("T_DATEFROM");
                dgStrategies.Columns[CN.DateFrom].ReadOnly = true;
                dgStrategies.Columns[CN.DateFrom].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgStrategies.Columns[CN.DateFrom].Width = 80;

                dgStrategies.Columns[CN.DateTo].HeaderText = GetResource("T_DATETO");
                dgStrategies.Columns[CN.DateTo].ReadOnly = true;
                dgStrategies.Columns[CN.DateTo].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgStrategies.Columns[CN.DateTo].Width = 80;

                //IP - 12/01/09 - CR976 - Need to check that the filter returns a row
                //when checking for the strategy name needed to populate the 'Strategy'
                //drop down for the selected account. If the account is not linked to a strategy
                //and was manually sent to a supervisor worklist (which may not be linked to a strategy)
                //then the 'Strategy' drop down should be set to empty string. Previously was
                //causing 'IndexOutOfRange' exception.
                var dvStrategies = new DataView(dsWorklistData.Tables[2]);
                dvStrategies.RowFilter = CN.DateTo + " IS  NULL";

                //if (dsWorklistData.Tables[2].Rows.Count > 0)
                if (dvStrategies.Count > 0)
                {
                    //IP - 12/01/09 - Moved to above.
                    //DataView dvStrategies = new DataView(dsWorklistData.Tables[2]);
                    //dvStrategies.RowFilter = CN.DateTo + " IS  NULL";   
                    drpStrategies.SelectedValue = dvStrategies[0][CN.Strategy].ToString(); //dsWorklistData.Tables[2].Rows[0][CN.Strategy].ToString();
                    Strategy = drpStrategies.SelectedValue.ToString();
                    PopulateActions(); //IP - 29/09/08 - UAT5.2 - UAT(529) - Populate the actions drop down once 'Strategy' drop down has been populated.
                }
                else
                {
                    drpStrategies.SelectedValue = String.Empty;
                    Strategy = String.Empty;
                    PopulateActions(); //IP - 12/01/09 - Still need to populate actions for the user.
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

                arr = new string[Convert.ToInt32(dsWorklistData.Tables[6].Rows.Count)];     //CR1084
                for (int i = 0; i < dsWorklistData.Tables[6].Rows.Count; i++)
                {
                    arr[i] = Convert.ToString(dsWorklistData.Tables[6].Rows[i][0]);
                }

                dgSMS.Columns[CN.DateAdded].HeaderText = GetResource("T_DATESENT");
                dgSMS.Columns[CN.DateAdded].ReadOnly = true;
                dgSMS.Columns[CN.DateAdded].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgSMS.Columns[CN.DateAdded].Width = 80;

                // NM - To display other customer accounts
                if (dsWorklistData.Tables.Contains("CustSearch"))
                {
                    DataView dvOtherAccounts = dsWorklistData.Tables["CustSearch"].DefaultView;
                    dvOtherAccounts.Table.Columns["Outstanding Balance"].ColumnName = "OutstandingBalance"; // To make sure column name has no spaces, as filter opertaion needs them without whitespaces
                    dvOtherAccounts.RowFilter = "AccountNumber <> '" + Acct.Trim() + "' AND OutstandingBalance > 0.0";
                    dgOtherAccounts.DataSource = dvOtherAccounts;

                    //Formatting Datagrid columns
                    foreach (DataGridViewColumn dgvc in dgOtherAccounts.Columns)
                    {
                        dgvc.Visible = false;
                        dgvc.ReadOnly = true;
                    }

                    dgOtherAccounts.Columns["AccountNumber"].Visible = true;
                    dgOtherAccounts.Columns["AccountNumber"].HeaderText = GetResource("T_ACCTNO");

                    dgOtherAccounts.Columns["HldOrJnt"].Visible = true;
                    dgOtherAccounts.Columns["HldOrJnt"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    dgOtherAccounts.Columns["Date Opened"].Visible = true;
                    dgOtherAccounts.Columns["Date Opened"].HeaderText = GetResource("T_DATEOPENED");
                    dgOtherAccounts.Columns["Date Opened"].DefaultCellStyle.Format = "d"; //Short datetime

                    dgOtherAccounts.Columns["OutstandingBalance"].Visible = true;
                    dgOtherAccounts.Columns["OutstandingBalance"].HeaderText = GetResource("T_OUTBAL");
                    dgOtherAccounts.Columns["OutstandingBalance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgOtherAccounts.Columns["OutstandingBalance"].DefaultCellStyle.Format = DecimalPlaces;

                    dgOtherAccounts.Columns["Arrears"].Visible = true;
                    dgOtherAccounts.Columns["Arrears"].HeaderText = GetResource("T_ARREARS");
                    dgOtherAccounts.Columns["Arrears"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgOtherAccounts.Columns["Arrears"].DefaultCellStyle.Format = DecimalPlaces;

                    dgOtherAccounts.Columns["Agreement Total"].Visible = true;
                    dgOtherAccounts.Columns["Agreement Total"].HeaderText = GetResource("T_AGREETOTAL");
                    dgOtherAccounts.Columns["Agreement Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgOtherAccounts.Columns["Agreement Total"].DefaultCellStyle.Format = DecimalPlaces;

                    dgOtherAccounts.Columns["Status"].Visible = true;
                    dgOtherAccounts.Columns["Status"].HeaderText = GetResource("T_STATUS");
                    dgOtherAccounts.Columns["Status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    dgOtherAccounts.Columns["CashPrice"].Visible = true;
                    dgOtherAccounts.Columns["CashPrice"].HeaderText = GetResource("T_CASHPRICE");
                    dgOtherAccounts.Columns["CashPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgOtherAccounts.Columns["CashPrice"].DefaultCellStyle.Format = DecimalPlaces;

                    dgOtherAccounts.Columns["branchno"].Visible = true;
                    dgOtherAccounts.Columns["branchno"].HeaderText = GetResource("T_BRANCH");

                    dgOtherAccounts.AutoResizeColumns();
                }
                else
                {
                    dgOtherAccounts.DataSource = null;
                }

                // Showing the error provider warning if there are any other accounts for this customer 
                // with outstanding balance > 0.0
                if (dgOtherAccounts.Rows.Count > 0)
                {
                    errorProviderForWarning.SetError(lblDummyForErrorProvider, "This customer has other accounts that are outstanding");
                }
                else
                {
                    errorProviderForWarning.SetError(lblDummyForErrorProvider, "");
                }

                //Lock this account -- Removing this as called earlier before deciding to load account...
                //CollectionsManager.LockAccount(acct, Credential.UserId, out Error);
            }

        }

        private void drpStrategies_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ((MainForm)this.FormRoot).statusBar1.Text = "";

            if (drpStrategies.SelectedValue.ToString() != Strategy)
            {
                StrategyChanged = true;
                //IP - 26/09/08 - UAT5.2 - UAT(529)
                //When the strategy was being changed back to what was orignally selected then the 
                //actions were not being loaded as the value of 'strategy' was set to original value
                //and was never changing.
                Strategy = drpStrategies.SelectedValue.ToString();
                //IP - 21/10/08 - The 'Strategy' drop down will now be disabled when the screen is loaded as we do not want the user
                //to be able to change the 'Strategy'.
                //populateActions();
            }
            else
            {
                StrategyChanged = false;
            }
        }

        private void TelephoneAction5_2_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Function = "TelephoneAction5_2_FormClosing";
                Wait();
                UnlockAccount();
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

        public override bool ConfirmClose()
        {
            try
            {
                Wait();

                // if (dvAccounts.Table != null)       // UAT787  jec 16/11/09 //IP - 15/12/09 - UAT5.2 (787) - added check on dvAccounts.Table.Rows.Count
                if (dvAccounts.Table != null && dvAccounts.Table.Rows.Count != 0)
                {
                    if (txtHDialCode.Text != txtHDialCode.Tag.ToString() || txtHome.Text != txtHome.Tag.ToString() ||
                        txtWDialCode.Text != txtWDialCode.Tag.ToString() || txtWork.Text != txtWork.Tag.ToString() ||
                        txtMDialCode.Text != txtMDialCode.Tag.ToString() || txtMobile.Text != txtMobile.Tag.ToString())
                    {
                        //If the user selects to save the changes call the event that is called
                        //when saving the telephone numbers.
                        if (DialogResult.Yes == ShowInfo("M_SAVECHANGESTELEPHONE", MessageBoxButtons.YesNo))
                        {
                            SavedTelephoneNumbers();
                        }
                    }
                    UnlockAccount();
                }
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

        private void btnFollowUp_Click(object sender, EventArgs e)
        {
            try
            {
                Wait();
                //int accountRow = dgAccounts.CurrentRowIndex;
                //IP - 14/08/09 - UAT(782)
                if (m_accountRow >= 0)
                {
                    //string acctNo = (string)dgAccounts[accountRow, 0];
                    BailActions actions = new BailActions(Acct, FormRoot, this);
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
                // if (dgAccounts.CurrentRowIndex >= 0)
                if (m_accountRow >= 0) //IP - 14/08/09 - UAT(782)
                {
                    Wait();
                    string custID = String.Empty;

                    custID = dvAccounts[DataViewNo][CN.CustID].ToString();

                    AddCustAcctCodes codes = new AddCustAcctCodes(AddCodes, custID, txtFirstName.Text, txtLastName.Text, Acct);
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
                //int accountRow = dgAccounts.CurrentRowIndex;
                if (dvAccounts.Count > 0)
                {
                    //string acctNo = (string)dgAccounts[accountRow, 0];
                    var details = new AccountDetails(Acct, FormRoot, this);
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

        private string CurrentWorklistCode
        {
            get
            {
                string code = "";
                int index = this.cmbWorkList.Text.IndexOf(":");
                if (index > 0)
                {
                    code = this.cmbWorkList.Text.Substring(0, index);
                }
                return code;
            }
        }
        //NM & IP - 23/12/2008 - CR976
        //When the 'Worklist' is selected from the 'Worklist' drop down
        //then the accounts retrieved should be filtered according to the worklist selected.
        private void cmbWorkList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Wait();
                //jec - 15/12/09 - UAT936 - retrieve data on change on worklist if supervisor
                if (showMultipleAccounts.Enabled == true && initWorklist == true && initTop500 == true && this.CurrentWorklistCode != prevWorklist)
                {
                    //IP - 03/06/11 - 5.4.4 Update - #3799 - when changing worklists should unlock the current account from the user.
                    if (Acct != string.Empty)
                    {
                        UnlockAccount();
                    }

                    dsWorklistAccounts = AccountManager.GetWorklistAccounts(Credential.UserId, this.CurrentWorklistCode, chkTop500.Checked, out Error);
                    dvAccounts = new DataView(dsWorklistAccounts.Tables[0]);
                    dgAccounts.DataSource = dvAccounts;
                    prevWorklist = CurrentWorklistCode;
                }
                initWorklist = true;

                if (dvAccounts.Table.Rows.Count > 0)
                {
                    DataViewNo = 0;


                    if (dgAccounts.TableStyles.Count != 0)
                    {
                        dgAccounts.TableStyles.RemoveAt(0);
                    }

                    //NM & IP - 23/12/2008 - If the tabpage (accountlists) has been removed
                    //from the tab (accounts) then we do not want to apply the tab styles.
                    #region tabstylesettings
                    if (dgAccounts.TableStyles.Count == 0 && dgAccounts.Visible == true)
                    {
                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        tabStyle.MappingName = dsWorklistAccounts.Tables[0].TableName;
                        dgAccounts.TableStyles.Add(tabStyle);

                        tabStyle.GridColumnStyles[CN.AcctType].Width = 0;
                        tabStyle.GridColumnStyles[CN.DateAcctOpen].Width = 0;
                        tabStyle.GridColumnStyles[CN.AgrmtTotal].Width = 0;
                        tabStyle.GridColumnStyles[CN.HiStatus].Width = 0;
                        tabStyle.GridColumnStyles[CN.CurrStatus].Width = 40;
                        tabStyle.GridColumnStyles[CN.CustID].Width = 0;

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

                        //Don't show reasons in grid CR1084
                        tabStyle.GridColumnStyles["CreditBlocked"].Width = 0;
                        tabStyle.GridColumnStyles["ReferralReason"].Width = 0;
                        tabStyle.GridColumnStyles["ReferralReason2"].Width = 0;
                        tabStyle.GridColumnStyles["ReferralReason3"].Width = 0;
                        tabStyle.GridColumnStyles["ReferralReason4"].Width = 0;
                        tabStyle.GridColumnStyles["ReferralReason5"].Width = 0;
                        tabStyle.GridColumnStyles["ReferralReason6"].Width = 0;

                    }

                    #endregion


                    ////jec - 15/12/09 - UAT - retrieve data on change on worklist
                    dgAccounts.DataSource = dvAccounts;

                    ApplyFilterAccountsGrid();
                    PopulateFields(0);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "cmbWorkList_SelectedIndexChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private bool SavedTelephoneNumbers()
        {
            bool saved = false;

            //NM - 29/12/2008 Need to validate telephone numbera
            //errorProvider1.SetError(txt, "XXXX");

            //IP - 30/12/08 - Retrieve the customer id for the selected account
            //as this will be needed to update any changes to the customers telephone numbers.
            errorProvider1.SetError(txtHDialCode, "");
            errorProvider1.SetError(txtHome, "");
            errorProvider1.SetError(txtWDialCode, "");
            errorProvider1.SetError(txtWork, "");
            errorProvider1.SetError(txtMDialCode, "");
            errorProvider1.SetError(txtMobile, "");

            //IP - 30/12/08 - Validate the telephone numbers and prevent from saving if they are
            //non-numeric.
            if (!IsNumeric(txtHDialCode.Text))
            {
                errorProvider1.SetError(txtHDialCode, GetResource("M_NONNUMERIC"));
            }
            else if (!IsNumeric(txtHome.Text))
            {
                errorProvider1.SetError(txtHome, GetResource("M_NONNUMERIC"));
            }
            else if (!IsNumeric(txtWDialCode.Text))
            {
                errorProvider1.SetError(txtWDialCode, GetResource("M_NONNUMERIC"));
            }
            else if (!IsNumeric(txtWork.Text))
            {
                errorProvider1.SetError(txtWork, GetResource("M_NONNUMERIC"));
            }
            else if (!IsNumeric(txtMDialCode.Text))
            {
                errorProvider1.SetError(txtMDialCode, GetResource("M_NONNUMERIC"));
            }
            else if (!IsNumeric(txtMobile.Text))
            {
                errorProvider1.SetError(txtMobile, GetResource("M_NONNUMERIC"));
            }
            else
            {
                bool HomeTelephoneChanged = false;
                bool WorkTelephoneChanged = false;
                bool MobileTelephoneChanged = false;

                //NM & IP - 30/12/08 - If the telephone numbers have not changed then we need to send an empty string
                //else the new value so that the record can be updated.

                string HDialCode = txtHDialCode.Text.Trim();
                string HTelephone = txtHome.Text.Trim();
                string WDialCode = txtWDialCode.Text.Trim();
                string WTelephone = txtWork.Text.Trim();
                string MDialCode = txtMDialCode.Text.Trim();
                string MTelephone = txtMobile.Text.Trim();

                //If the telephone numbers have changed then set a boolean to true
                //so that the stored procedure will only update the record
                //if the numbers have changed.
                if (txtHDialCode.Text.Trim() != txtHDialCode.Tag.ToString().Trim() ||
                    txtHome.Text.Trim() != txtHome.Tag.ToString().Trim())
                {
                    HomeTelephoneChanged = true;
                }

                if (txtWDialCode.Text.Trim() != txtWDialCode.Tag.ToString().Trim() ||
                   txtWork.Text.Trim() != txtWork.Tag.ToString())
                {
                    WorkTelephoneChanged = true;
                }

                if (txtMDialCode.Text.Trim() != txtMDialCode.Tag.ToString().Trim() ||
                   txtMobile.Text.Trim() != txtMobile.Tag.ToString().Trim())
                {
                    MobileTelephoneChanged = true;
                }

                CollectionsManager.UpdateCustomerTelephoneNo(m_custid, HTelephone, HDialCode, WTelephone, WDialCode,
                                                                MTelephone, MDialCode, Credential.UserId, HomeTelephoneChanged, WorkTelephoneChanged, MobileTelephoneChanged, out Error);

                //NM & IP - 30/12/08 - Once the changed telephone numbers have been saved
                //we need to update the tag properties values for each of the telephone numbers
                //text boxes to the changed values.

                if (Error.Length > 0)
                {
                    ShowError(Error);
                }
                else
                {
                    txtHDialCode.Tag = txtHDialCode.Text;
                    txtHome.Tag = txtHome.Text;
                    txtWDialCode.Tag = txtWDialCode.Text;
                    txtWork.Tag = txtWork.Text;
                    txtMDialCode.Tag = txtMDialCode.Text;
                    txtMobile.Tag = txtMobile.Text;

                    ShowInfo("M_TELEPHONEUPDATED");

                    saved = true;
                }
            }

            return saved;
        }

        //NM & IP - 30/12/08 - CR976
        private void btnSaveTelephone_Click(object sender, EventArgs e)
        {
            try
            {
                Wait();
                SavedTelephoneNumbers();
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSaveTelephone_Click");
            }
            finally
            {
                StopWait();
            }
        }

        //IP - 30/12/08 - CR976 - Customer Details button will display the 
        //Customer Record screen for the selected account.
        private void btnCustomerDetails_Click(object sender, EventArgs e)
        {
            //string custid = string.Empty;
            //string accttype = string.Empty;
            //string acctno = string.Empty;

            //custid = dvAccounts[dgAccounts.CurrentRowIndex][CN.CustID].ToString();
            //accttype = dvAccounts[dgAccounts.CurrentRowIndex][CN.AcctType].ToString();
            //acctno = dvAccounts[dgAccounts.CurrentRowIndex][CN.acctno].ToString();

            try
            {
                if (dvAccounts.Count > 0)
                {
                    var details = new BasicCustomerDetails(true, m_custid, Acct, Holder.Main, m_accttype, FormRoot, FormParent);
                    details.FormRoot = this.FormRoot;
                    details.FormParent = this.FormParent;
                    ((MainForm)this.FormRoot).AddTabPage(details, 10);
                    details.loaded = true;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnCustomerDetails_Click");
            }
        }



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

        //private DataTable CreateTRCDetailDT()
        //{
        //    DataTable dtLegalDetail = new DataTable(TN.TraceDetails);

        //    dtLegalDetail.Columns.Add(CN.CMAcctno);
        //    dtLegalDetail.Columns.Add(CN.CMEmpeeno);
        //    dtLegalDetail.Columns.Add(CN.CMTRCInitiatedDate);
        //    dtLegalDetail.Columns.Add(CN.CMIsResolved);
        //    dtLegalDetail.Columns.Add(CN.CMUserNotes);

        //    return dtLegalDetail;
        //}

        //NM & IP - 08/01/09 - CR976 - Set the 'Notes' field to the 'User Notes' entered
        private void SetNotes(DataTable extractedInfo)
        {
            //If user notes were entered on either the 'Legal Details', 'Insurance Details'
            //or 'Fraud Details' screen then set the 'Notes' field on the 'Telephone Action'
            //screen to the user notes entered.
            txtNotes.Text = extractedInfo.Rows[0][CN.CMUserNotes].ToString();
        }

        private void chkFilterByBranch_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                cmbBranch.Enabled = chkFilterByBranch.Checked;
                ApplyFilterAccountsGrid();
            }
            catch (Exception ex)
            {
                Catch(ex, "cmbBranch_SelectedIndexChanged");
            }
        }

        private void cmbBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ApplyFilterAccountsGrid();
            }
            catch (Exception ex)
            {
                Catch(ex, "cmbBranch_SelectedIndexChanged");
            }
        }

        private void ApplyFilterAccountsGrid()
        {
            if (dgAccounts.DataSource == null || !(dgAccounts.DataSource is DataView))
                return;

            DataView dvGrid = (DataView)dgAccounts.DataSource;
            string strFilter = "";

            if (cmbWorkList.SelectedIndex >= 0)
            {
                strFilter = CN.WorkList + " = '" + CurrentWorklistCode + "' ";
            }

            if (cmbBranch.Enabled && cmbBranch.SelectedIndex >= 0)
            {
                strFilter = (strFilter.Trim().Length > 0) ? strFilter + " AND " : "";
                strFilter = strFilter + CN.BranchNo + " = " + cmbBranch.SelectedValue.ToString();
            }

            dvGrid.RowFilter = strFilter;
            PopulateFields(0);
        }

        //IP - 28/08/09 - UAT(737) - This method is called from the Refunds & Corrections screen.
        //Once the bdw has been reversed, we need to save the action.
        public void SaveRbdwAction()
        {
            if (reversedBDW == true)
            {
                btnSave_Click(null, null);
                ((MainForm)this.FormRoot).statusBar1.Text = "The action has been saved against the account.";
            }
        }

        //IP - 10/09/09 - UAT(848) - I have moved this out from the btnSave_Click() method 
        //as in some cases actions such as 'SPA' are saved differently in that the action is saved in the drpActionCode_SelectionChangeCommitted event.
        //therefore this is now re-usable.
        private void CycleToNextAccount()
        {

            ((MainForm)this.FormRoot).statusBar1.Text = "";
            int accountRow = m_accountRow;
            string AcctnoLockedforUser = "";
            if (chCycleNext.Checked == true)
            {
                int rowNo = accountRow + 1; //account row is supposed to be the current row that is being looked at in the array.
                //Unlock previous account 
                if (accountRow >= 0)
                {
                    UnlockAccount();
                }
                
                var acctnoList = new StringBuilder();
                while (!(rowNo > 30 || rowNo > dvAccounts.Count - 1))
                {
                    acctnoList.Append(dvAccounts[rowNo][CN.AcctNo].ToString());
                    rowNo++;
                    acctnoList.Append(","); //separate with comma when sending to server
                }

                AcctnoLockedforUser = AccountManager.AccountLockingFindandLockForCaller(acctnoList.ToString(), Credential.UserId.ToString(), out Error); // returns the first account which is not locked

                if (AcctnoLockedforUser == null || String.IsNullOrEmpty(AcctnoLockedforUser.Trim())) //if no accounts returned then load up the screen again...
                {
                    m_accountRow = 0;
                    prevWorklist = "settoreloadplease";
                    chkTop500_CheckedChanged(this, null);
                    if (dvAccounts.Count > 0)
                    {
                          ((MainForm)this.FormRoot).statusBar1.Text = "End of List Reached - Reloaded";
                          CycleToNextAccount();
                    }
                    else
                        ((MainForm)this.FormRoot).statusBar1.Text = "End of List Reached - No accounts available";
                }
                else
                {
                    var rowcounter = 0;
                    while (true)
                    {
                        //rowcounter holds the index of the next current account row.
                        rowcounter = accountRow + 1;

                        //If the number of accounts > current account row retrieve the account number.
                        if (dvAccounts.Count > rowcounter)
                        {
                            Acct = dvAccounts[rowcounter][CN.AcctNo].ToString();
                            if (Acct == AcctnoLockedforUser)
                                break;
                        }
                        else
                        {
                            break; // No more accounts
                        }
                        accountRow++;
                    }

                    //Populate the fields for the current account row.
                    PopulateFields(accountRow + 1);

                    //if the number of accounts < current account row keep existing customer details... 
                    //If the current account row is the last row.
                    if (dvAccounts.Count <= accountRow + 1)
                    {
                        rowNo = accountRow;
                        if (((DataView)dgAccounts.DataSource).Count <= 1)
                        {
                            btnSave.Enabled = false;
                            InitCustomerDetails();
                            //eeff
                        }
                        else
                        {
                            //else populate for the next row down.
                            PopulateFields(accountRow - 1);
                        }
                    }
                    else
                    {
                        rowNo = accountRow + 1;
                    }

                    //NM & IP - 31/12/08 - CR976 - only delete the current row if the accounts
                    //grid is visible.
                    //If the number of accounts > current account row and user right to display the 'Accounts' tab is enabled
                    //unselect the previous account row and select the current row.
                    if (dvAccounts.Count > rowNo && showMultipleAccounts.Enabled == true)
                    {
                        if (dgAccounts.CurrentRowIndex > 0)
                            dgAccounts.UnSelect(accountRow);

                        try
                        {
                            dgAccounts.Select(rowNo);
                            dgAccounts.CurrentCell = new DataGridCell(rowNo, 0);

                            DataRowView myRow;
                            var dv = (DataView)dgAccounts.DataSource;

                            myRow = dv[accountRow];
                            myRow.Delete();
                        }
                        catch
                        {
                            //Doesn't always like this --14/apr/11 not quite sure what the issue is but hide for now
                        }

                        //Remove the row from the datagrid

                    }
                }

                InitNewAction();
                lbReason.Visible = false;
                drpReason.Visible = false;
                lbActionDate.Text = "Due Date";
                lbActionValue.Text = "Action Value";
                StrategyChanged = false;
                drpSendToStrategy.Visible = false; //IP - 20/10/08 - UAT5.2 - UAT(551)
                chkApplyAllAccounts.Checked = false; //CR1084
            }
        }

        private void dgSMS_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Right || dgSMS.CurrentRow == null)
                    return;

                var m1 = new MenuCommand(GetResource("P_VIEWSMS"));
                m1.Click += new System.EventHandler(this.ViewSMS_Click);

                var popup = new PopupMenu();
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
                    new ViewSMSPopup(text).ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "ViewSMS_Click");
            }
        }



        //IP - 12/11/09 - UAT5.2 (882) - //IP - 12/11/09 UAT5.2 (882) - added control to return top500 accounts
        private void chkTop500_CheckedChanged(object sender, EventArgs e)
        {
            string worklist;
            if (cmbWorkList.SelectedIndex != -1)
            {
                worklist = CurrentWorklistCode;
            }
            else
            {
                worklist = "";
            }
            //IP - 12/11/09 - UAT5.2 (882) - added control to return top500 accounts
            if ((prevWorklist != worklist && prevWorklist != "") || chkTop500.Checked == false)   //UAT936 only get data if necessary
            {
                dsWorklistAccounts = AccountManager.GetWorklistAccounts(Credential.UserId, worklist, chkTop500.Checked, out Error);
            }
            prevWorklist = worklist;        //UAT936
            dvAccounts = new DataView(dsWorklistAccounts.Tables[0]);

            if (Error.Length > 0)
            {
                ShowError(Error);
            }
            else
            {
                if (dsWorklistAccounts != null)
                {
                    //NM & IP - 24/12/2008 - CR976 - If this is not a supervisor then
                    //we do not want to display the 'Worklist' drop down as accounts should not be
                    //filtered by worklist.

                    if (showMultipleAccounts.Enabled == false)
                    {
                        if (dvAccounts.Count > 0)
                        {
                            PopulateFields(0);
                            DataViewNo = 0;

                            //Populate the datagrid dgAccounts
                            dgAccounts.DataSource = dvAccounts;
                        }
                        else
                        {
                            gbCustomer.Text = "No Accounts Loaded";
                            btnSave.Enabled = false;
                            tabCustomer.Enabled = false;
                            errorProvider1.SetError(lblDummyForErrorProvider,
                            "No Accounts left - ask for another worklist - then exit and re-enter screen");

                            errorProviderForWarning.Clear();
                            drpActionCode.Enabled = false;
                            //eeff
                        }
                    }
                    //Else this is the supervisor as they have been given the right 
                    //'Telephone Action - View Multiple Accounts'.
                    //NM & IP - 23/12/08 - CR976 - Populate the 'Worklist' drop down
                    //with the worklists the employee has rights to view.
                    else if (dsWorklistAccounts.Tables[1].Rows.Count > 0)
                    {
                        cmbWorkList.DisplayMember = CN.WorkList;
                        cmbWorkList.ValueMember = CN.WorkList;
                        cmbWorkList.DataSource = dsWorklistAccounts.Tables[1];
                        cmbWorkList.SelectedText = dsWorklistAccounts.Tables[1].Rows[0][CN.WorkList].ToString();
                        initTop500 = true;          // UAT936 
                        if (dsWorklistAccounts.Tables[0].Rows.Count > 0)
                        {
                            int i = cmbWorkList.FindString(dsWorklistAccounts.Tables[0].Rows[0][CN.WorkList].ToString());
                            if (i != -1)
                            {
                                cmbWorkList.SelectedIndex = i;
                            }
                        }

                        PopulateFields(0);
                    }
                }
                else
                {
                    gbLastAction.Enabled = false;
                    gbNewAction.Enabled = false;
                    drpActionCode.SelectedIndex = -1;
                }
            }
        }

        private void btnReferences_Click(object sender, EventArgs e)
        {
            try
            {
                Wait();

                if (StageToLaunch.AccountNo != "")
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

        private void tabAccounts_SelectionChanged(object sender, EventArgs e)
        {
            if (tabAccounts.SelectedTab.Name == "tabAccountList")
            {
                cmbWorkList_SelectedIndexChanged(null, null);
            }
        }

        private void btn_calc_Click(object sender, EventArgs e)
        {
            var calc = new TelActionPaymentCalc(Acct, Convert.ToDecimal(StripCurrency(txtArrears.Text)), Convert.ToDecimal(StripCurrency(txtBalance.Text)));
            calc.FormRoot = this.FormRoot;
            calc.FormParent = this;
            calc.ShowDialog(this);


        }
    }
}
