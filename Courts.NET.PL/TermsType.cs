using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Data;
using STL.Common.Constants.ColumnNames;
using STL.Common.Static;
using System.Xml;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.RateTypes;
using STL.Common.PrivilegeClub;

namespace STL.PL
{
    /// <summary>
    /// Maintenance screen for Terms Types. Terms Types define the agreement
    /// terms for hire purchase and ready finance accounts. When these accounts
    /// are created they must be linked to a Terms Type in order to calculate
    /// the Service Charge and the Agreement Total. Many parameters are defined
    /// by a Terms Type, such as the percentage rate for the service charge 
    /// calculation, the length of the agreement and the amount of any deposit
    /// required before delivery. History data is held for the percentage rates.
    /// Different documents can be used to print the agreement. Cash Back can be
    /// optionally applied after a specified number of instalments.
    /// </summary>
    public class TermsType : CommonForm
    {
        private bool _detectSelectionChanges = false;
        private DateTime _serverDate = Date.blankDate;
        private DateTime _curEndDate = Date.blankDate;
        private bool _dtStart_HasDropDown = false;
        private bool _copy = false;
        //private int _oldDateIndex = -1;
        private static double _MAX_CASHBACK_PERC = 15.0;
        private DataTable _currentRates = null;
        private DataTable _historyRates = null;
        // UAT83 jec 19/07/07
        private DateTime openDateTo = new System.DateTime(1900, 1, 1, 00, 0, 00, 000);
        private decimal insPercent = 0;
        private decimal adminPercent = 0;
        private decimal adminValue = 0;
        private bool includeWarranties = false;
        private bool includeInsurance = false;
        // end jec 19/07/07

        private decimal insPercent_old = 0;
        private decimal adminPercent_old = 0;
        private decimal adminValue_old = 0;
        private bool includeInsurance_old = false;              //#17856
        private bool includeWarranties_old = false;             //#17856


        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private System.Windows.Forms.GroupBox gbOverview;
        private System.Windows.Forms.GroupBox gbDetails;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTermsType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.ListBox lbAccountTypes;
        private new string Error = "";
        private System.Windows.Forms.RadioButton rbAffinityYes;
        private System.Windows.Forms.RadioButton rbAffinityNo;
        private Crownwood.Magic.Controls.TabControl tcDetails;
        private Crownwood.Magic.Controls.TabPage tpInstallments;
        private Crownwood.Magic.Controls.TabPage tpRates;
        private Crownwood.Magic.Controls.TabPage tpLetters;
        private Crownwood.Magic.Controls.TabPage tpMmiDetails;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox drpRateType;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numInsPercent;
        private System.Windows.Forms.NumericUpDown numAdminPercent;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox chxIncludeWarranties;
        private System.Windows.Forms.CheckBox chxIncludeInsurance;
        private System.Windows.Forms.RadioButton rbDepositTypeP;
        private System.Windows.Forms.RadioButton rbDepositTypeC;
        private System.Windows.Forms.TextBox txtDepositValue;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtSTCMinValue;
        private System.Windows.Forms.RadioButton rbSTCMinTypeC;
        private System.Windows.Forms.RadioButton rbSTCMinTypeP;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.NumericUpDown numLength;
        private System.Windows.Forms.DataGrid dgLengthOptions;
        private System.Windows.Forms.Button btnLengthRemove;
        private System.Windows.Forms.Button btnLengthAdd;
        private System.Windows.Forms.NumericUpDown numMonthsIntFree;
        private System.Windows.Forms.NumericUpDown numMonthsIntDef;
        private System.Windows.Forms.CheckBox chxInstallPreDel;
        private System.Windows.Forms.GroupBox gbAffinity;        
        private System.Windows.Forms.DataGrid dgDateRanges;
        private DataSet TermsTypeDetails = null;
        private System.Windows.Forms.GroupBox gbIsActive;
        private System.Windows.Forms.RadioButton rbActiveNo;
        private System.Windows.Forms.RadioButton rbActiveYes;
        private System.ComponentModel.IContainer components;
        private Control allowEdit = null;
        private Control allowView = null;
        private System.Windows.Forms.RadioButton rbPaymentHolidayNo;
        private System.Windows.Forms.RadioButton rbPaymentHolidayYes;
        private System.Windows.Forms.GroupBox gbPaymentHoliday;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.NumericUpDown numMinInst;
        private System.Windows.Forms.NumericUpDown numMaxInst;
        private System.Windows.Forms.NumericUpDown numDefaultInstal;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.NumericUpDown numRebateDays;
        private System.Windows.Forms.Label label20;
        private Crownwood.Magic.Controls.TabPage tpCashBack;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numCashBackMonth;
        private System.Windows.Forms.RadioButton rbCashBackCurrency;
        private System.Windows.Forms.RadioButton rbCashBackPerc;
        private System.Windows.Forms.GroupBox gbStcDetails;
        private System.Windows.Forms.TextBox txtCashBackValue;
        private System.Windows.Forms.GroupBox gbCashBackOptions;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.GroupBox gbRateDetails;
        private System.Windows.Forms.GroupBox gbLengthOptions;
        private System.Windows.Forms.GroupBox gbInstallmentDetails;
        private System.Windows.Forms.GroupBox gbDeposit;
        private System.Windows.Forms.GroupBox gbDatePeriod;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.CheckBox chxDelNonStocks;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chxNoArrearsLetters;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.ComboBox drpAgreement;
        private System.Windows.Forms.TextBox txtAgreementText;
        private System.Windows.Forms.GroupBox gbLetters;
        private System.Windows.Forms.GroupBox gbMmiDetails;
        private Label lblIsMmiActive;
        private CheckBox cbIsMmiActive;
        private Label lblMmiThresholdPerc;
        private NumericUpDown numMmiThresholdPerc;
        private System.Windows.Forms.TextBox txtAPR;
        private System.Windows.Forms.Label label27;
        private Button btnBands;
        private NumericUpDown numDefaultService;
        private TextBox txtPointsTo;
        private Label label7;
        private TextBox txtPointsFrom;
        private ComboBox drpBand;
        private Label lPointsFrom;
        private Label lBand;
        private DateTimePicker dtStart;
        private Label label3;
        private Label lPointsTo;
        private Crownwood.Magic.Controls.TabPage tpOptRates;
        private GroupBox gbOptionalRates;
        private Label label24;
        private Label label23;
        private Label label22;
        private NumericUpDown numOptRateFromMonth;
        private NumericUpDown numOptRateToMonth;
        private NumericUpDown numOptionalRate;
        private DataGrid dgOptionalRates;
        private Button btnEnterRate;
        private Button btnRemoveRate;
        private Crownwood.Magic.Controls.TabPage tpHistory;
        private DataGrid dgHistory;
        private Button btnAddDateRange;
        private Button btnRemoveDateRange;
        private ComboBox drpLoyaltyClub;
        private Label lLoyaltyClub;
        private CheckBox chxSecuritise;
        private ComboBox drpStoreType;
		private Label label4;
        private GroupBox gbIsLoan;
        private CheckBox cbLoanExisting;
        private CheckBox cbLoanRecent;
        private CheckBox cbLoanNew;
        private CheckBox cbLoanStaff;
        private Label label28;
        private NumericUpDown numAdminValue;
        private Control allowActivate = null;
        


        public TermsType(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
        }

        public TermsType(Form root, Form parent)
        {
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;

            HashMenus();
            ApplyRoleRestrictions();

            //Enable / Disable availability of controls based on the derived permissions.
            //Other control availibility is set in the EnableFields method.
            foreach (System.Windows.Forms.Control control in gbOverview.Controls)
            {
                if (control.Name != "txtTermsType"
                    && control.Name != "btnLoad"
                    && control.Name != "btnExit"
                    && control.Name != "btnSave"
                    && !(control is Label)
                    && !(control is TabPage))
                {
                    control.Enabled = this.allowEdit.Enabled;
                }
            }

            AddKeyPressedEventHandlers(this);
            //Add extra event handlers particularly for this forms functionality
            this.btnAddDateRange.Click += new System.EventHandler(this.CommonDataChanged);
            this.btnRemoveDateRange.Click += new System.EventHandler(this.CommonDataChanged);
            this.btnEnterRate.Click += new System.EventHandler(this.CommonDataChanged);
            this.btnRemoveRate.Click += new System.EventHandler(this.CommonDataChanged);

            gbIsActive.Enabled = this.allowActivate.Enabled;
            chxSecuritise.Visible = (Config.CountryCode == "S"); //CR884 - This field should only be visible to Singapore                                                                    

            if (!Convert.ToBoolean(Country[CountryParameterNames.EnableMMI]))
            {
                tcDetails.TabPages.Remove(tpMmiDetails);
            }

            // Display check boxes for Tiered Privilege Club
            //bool showTierPrivilegeClub = (bool)Country[CountryParameterNames.TierPCEnabled];
            //this.cbTier1.Visible = this.cbTier1.Enabled = showTierPrivilegeClub;
            //this.cbTier2.Visible = this.cbTier2.Enabled = showTierPrivilegeClub;

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
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
            this.allowEdit = new System.Windows.Forms.Control();
            this.allowView = new System.Windows.Forms.Control();
            this.allowActivate = new System.Windows.Forms.Control();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.gbDetails = new System.Windows.Forms.GroupBox();
            this.tcDetails = new Crownwood.Magic.Controls.TabControl();
            this.tpInstallments = new Crownwood.Magic.Controls.TabPage();
            this.gbLengthOptions = new System.Windows.Forms.GroupBox();
            this.numLength = new System.Windows.Forms.NumericUpDown();
            this.dgLengthOptions = new System.Windows.Forms.DataGrid();
            this.btnLengthRemove = new System.Windows.Forms.Button();
            this.btnLengthAdd = new System.Windows.Forms.Button();
            this.gbInstallmentDetails = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.chxDelNonStocks = new System.Windows.Forms.CheckBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.numRebateDays = new System.Windows.Forms.NumericUpDown();
            this.label29 = new System.Windows.Forms.Label();
            this.numMaxInst = new System.Windows.Forms.NumericUpDown();
            this.label25 = new System.Windows.Forms.Label();
            this.numMinInst = new System.Windows.Forms.NumericUpDown();
            this.numDefaultInstal = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.numMonthsIntFree = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.numMonthsIntDef = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.chxInstallPreDel = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.gbStcDetails = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtSTCMinValue = new System.Windows.Forms.TextBox();
            this.rbSTCMinTypeC = new System.Windows.Forms.RadioButton();
            this.rbSTCMinTypeP = new System.Windows.Forms.RadioButton();
            this.gbDeposit = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtDepositValue = new System.Windows.Forms.TextBox();
            this.rbDepositTypeC = new System.Windows.Forms.RadioButton();
            this.rbDepositTypeP = new System.Windows.Forms.RadioButton();
            this.tpRates = new Crownwood.Magic.Controls.TabPage();
            this.btnAddDateRange = new System.Windows.Forms.Button();
            this.btnRemoveDateRange = new System.Windows.Forms.Button();
            this.gbRateDetails = new System.Windows.Forms.GroupBox();
            this.numAdminValue = new System.Windows.Forms.NumericUpDown();
            this.label28 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.numInsPercent = new System.Windows.Forms.NumericUpDown();
            this.chxIncludeWarranties = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.drpRateType = new System.Windows.Forms.ComboBox();
            this.numAdminPercent = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.chxIncludeInsurance = new System.Windows.Forms.CheckBox();
            this.gbDatePeriod = new System.Windows.Forms.GroupBox();
            this.drpLoyaltyClub = new System.Windows.Forms.ComboBox();
            this.lLoyaltyClub = new System.Windows.Forms.Label();
            this.numDefaultService = new System.Windows.Forms.NumericUpDown();
            this.txtPointsTo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtPointsFrom = new System.Windows.Forms.TextBox();
            this.drpBand = new System.Windows.Forms.ComboBox();
            this.lPointsFrom = new System.Windows.Forms.Label();
            this.lBand = new System.Windows.Forms.Label();
            this.dtStart = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.lPointsTo = new System.Windows.Forms.Label();
            this.dgDateRanges = new System.Windows.Forms.DataGrid();
            this.tpHistory = new Crownwood.Magic.Controls.TabPage();
            this.dgHistory = new System.Windows.Forms.DataGrid();
            this.tpOptRates = new Crownwood.Magic.Controls.TabPage();
            this.gbOptionalRates = new System.Windows.Forms.GroupBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.numOptRateFromMonth = new System.Windows.Forms.NumericUpDown();
            this.numOptRateToMonth = new System.Windows.Forms.NumericUpDown();
            this.numOptionalRate = new System.Windows.Forms.NumericUpDown();
            this.dgOptionalRates = new System.Windows.Forms.DataGrid();
            this.btnEnterRate = new System.Windows.Forms.Button();
            this.btnRemoveRate = new System.Windows.Forms.Button();
            this.tpLetters = new Crownwood.Magic.Controls.TabPage();
            this.gbLetters = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.chxNoArrearsLetters = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.drpAgreement = new System.Windows.Forms.ComboBox();
            this.txtAgreementText = new System.Windows.Forms.TextBox();
            this.tpCashBack = new Crownwood.Magic.Controls.TabPage();
            this.gbCashBackOptions = new System.Windows.Forms.GroupBox();
            this.numCashBackMonth = new System.Windows.Forms.NumericUpDown();
            this.label30 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.txtCashBackValue = new System.Windows.Forms.TextBox();
            this.rbCashBackCurrency = new System.Windows.Forms.RadioButton();
            this.rbCashBackPerc = new System.Windows.Forms.RadioButton();
            this.tpMmiDetails = new Crownwood.Magic.Controls.TabPage();
            this.gbMmiDetails = new System.Windows.Forms.GroupBox();
            this.cbIsMmiActive = new System.Windows.Forms.CheckBox();
            this.lblIsMmiActive = new System.Windows.Forms.Label();
            this.numMmiThresholdPerc = new System.Windows.Forms.NumericUpDown();
            this.lblMmiThresholdPerc = new System.Windows.Forms.Label();
            this.gbOverview = new System.Windows.Forms.GroupBox();
            this.chxSecuritise = new System.Windows.Forms.CheckBox();
            this.gbIsLoan = new System.Windows.Forms.GroupBox();
            this.cbLoanStaff = new System.Windows.Forms.CheckBox();
            this.cbLoanExisting = new System.Windows.Forms.CheckBox();
            this.cbLoanRecent = new System.Windows.Forms.CheckBox();
            this.cbLoanNew = new System.Windows.Forms.CheckBox();
            this.drpStoreType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnBands = new System.Windows.Forms.Button();
            this.label27 = new System.Windows.Forms.Label();
            this.txtAPR = new System.Windows.Forms.TextBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.gbPaymentHoliday = new System.Windows.Forms.GroupBox();
            this.rbPaymentHolidayNo = new System.Windows.Forms.RadioButton();
            this.rbPaymentHolidayYes = new System.Windows.Forms.RadioButton();
            this.gbIsActive = new System.Windows.Forms.GroupBox();
            this.rbActiveNo = new System.Windows.Forms.RadioButton();
            this.rbActiveYes = new System.Windows.Forms.RadioButton();
            this.gbAffinity = new System.Windows.Forms.GroupBox();
            this.rbAffinityNo = new System.Windows.Forms.RadioButton();
            this.rbAffinityYes = new System.Windows.Forms.RadioButton();
            this.lbAccountTypes = new System.Windows.Forms.ListBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTermsType = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.gbDetails.SuspendLayout();
            this.tcDetails.SuspendLayout();
            this.tpInstallments.SuspendLayout();
            this.gbLengthOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgLengthOptions)).BeginInit();
            this.gbInstallmentDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRebateDays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxInst)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinInst)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDefaultInstal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMonthsIntFree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMonthsIntDef)).BeginInit();
            this.gbStcDetails.SuspendLayout();
            this.gbDeposit.SuspendLayout();
            this.tpRates.SuspendLayout();
            this.gbRateDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAdminValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numInsPercent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAdminPercent)).BeginInit();
            this.gbDatePeriod.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDefaultService)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgDateRanges)).BeginInit();
            this.tpHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgHistory)).BeginInit();
            this.tpOptRates.SuspendLayout();
            this.gbOptionalRates.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numOptRateFromMonth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOptRateToMonth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOptionalRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgOptionalRates)).BeginInit();
            this.tpLetters.SuspendLayout();
            this.gbLetters.SuspendLayout();
            this.tpCashBack.SuspendLayout();
            this.gbCashBackOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCashBackMonth)).BeginInit();
            this.tpMmiDetails.SuspendLayout();
            this.gbMmiDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMmiThresholdPerc)).BeginInit();
            this.gbOverview.SuspendLayout();
            this.gbIsLoan.SuspendLayout();
            this.gbPaymentHoliday.SuspendLayout();
            this.gbIsActive.SuspendLayout();
            this.gbAffinity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // allowEdit
            // 
            this.allowEdit.Enabled = false;
            this.allowEdit.Location = new System.Drawing.Point(0, 0);
            this.allowEdit.Name = "allowEdit";
            this.allowEdit.Size = new System.Drawing.Size(0, 0);
            this.allowEdit.TabIndex = 0;
            // 
            // allowView
            // 
            this.allowView.Enabled = false;
            this.allowView.Location = new System.Drawing.Point(0, 0);
            this.allowView.Name = "allowView";
            this.allowView.Size = new System.Drawing.Size(0, 0);
            this.allowView.TabIndex = 0;
            // 
            // allowActivate
            // 
            this.allowActivate.Enabled = false;
            this.allowActivate.Location = new System.Drawing.Point(0, 0);
            this.allowActivate.Name = "allowActivate";
            this.allowActivate.Size = new System.Drawing.Size(0, 0);
            this.allowActivate.TabIndex = 0;
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuExit});
            this.menuFile.Text = "&File";
            // 
            // gbDetails
            // 
            this.gbDetails.BackColor = System.Drawing.SystemColors.Control;
            this.gbDetails.Controls.Add(this.tcDetails);
            this.gbDetails.Location = new System.Drawing.Point(8, 160);
            this.gbDetails.Name = "gbDetails";
            this.gbDetails.Size = new System.Drawing.Size(776, 312);
            this.gbDetails.TabIndex = 1;
            this.gbDetails.TabStop = false;
            this.gbDetails.Text = "Details";
            // 
            // tcDetails
            // 
            this.tcDetails.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tcDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcDetails.IDEPixelArea = true;
            this.tcDetails.Location = new System.Drawing.Point(3, 16);
            this.tcDetails.Name = "tcDetails";
            this.tcDetails.PositionTop = true;
            this.tcDetails.SelectedIndex = 6;
            this.tcDetails.SelectedTab = this.tpMmiDetails;
            this.tcDetails.Size = new System.Drawing.Size(770, 293);
            this.tcDetails.TabIndex = 38;
            this.tcDetails.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpInstallments,
            this.tpRates,
            this.tpHistory,
            this.tpOptRates,
            this.tpLetters,
            this.tpCashBack,
            this.tpMmiDetails});
            // 
            // tpInstallments
            // 
            this.tpInstallments.Controls.Add(this.gbLengthOptions);
            this.tpInstallments.Controls.Add(this.gbInstallmentDetails);
            this.tpInstallments.Controls.Add(this.gbStcDetails);
            this.tpInstallments.Controls.Add(this.gbDeposit);
            this.tpInstallments.Location = new System.Drawing.Point(0, 25);
            this.tpInstallments.Name = "tpInstallments";
            this.tpInstallments.Selected = false;
            this.tpInstallments.Size = new System.Drawing.Size(770, 268);
            this.tpInstallments.TabIndex = 0;
            this.tpInstallments.Title = "Instalment Details";
            this.tpInstallments.Visible = false;
            // 
            // gbLengthOptions
            // 
            this.gbLengthOptions.Controls.Add(this.numLength);
            this.gbLengthOptions.Controls.Add(this.dgLengthOptions);
            this.gbLengthOptions.Controls.Add(this.btnLengthRemove);
            this.gbLengthOptions.Controls.Add(this.btnLengthAdd);
            this.gbLengthOptions.Location = new System.Drawing.Point(568, 8);
            this.gbLengthOptions.Name = "gbLengthOptions";
            this.gbLengthOptions.Size = new System.Drawing.Size(192, 232);
            this.gbLengthOptions.TabIndex = 1;
            this.gbLengthOptions.TabStop = false;
            this.gbLengthOptions.Text = "Length Options";
            // 
            // numLength
            // 
            this.numLength.Location = new System.Drawing.Point(16, 40);
            this.numLength.Name = "numLength";
            this.numLength.Size = new System.Drawing.Size(56, 23);
            this.numLength.TabIndex = 0;
            this.numLength.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // dgLengthOptions
            // 
            this.dgLengthOptions.CaptionVisible = false;
            this.dgLengthOptions.DataMember = "";
            this.dgLengthOptions.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgLengthOptions.Location = new System.Drawing.Point(16, 88);
            this.dgLengthOptions.Name = "dgLengthOptions";
            this.dgLengthOptions.ReadOnly = true;
            this.dgLengthOptions.Size = new System.Drawing.Size(160, 128);
            this.dgLengthOptions.TabIndex = 1;
            // 
            // btnLengthRemove
            // 
            this.btnLengthRemove.BackColor = System.Drawing.Color.SlateBlue;
            this.btnLengthRemove.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLengthRemove.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnLengthRemove.Image = global::STL.PL.Properties.Resources.Minus;
            this.btnLengthRemove.Location = new System.Drawing.Point(104, 38);
            this.btnLengthRemove.Name = "btnLengthRemove";
            this.btnLengthRemove.Size = new System.Drawing.Size(22, 22);
            this.btnLengthRemove.TabIndex = 0;
            this.btnLengthRemove.TabStop = false;
            this.btnLengthRemove.UseVisualStyleBackColor = false;
            this.btnLengthRemove.Click += new System.EventHandler(this.btnLengthRemove_Click);
            // 
            // btnLengthAdd
            // 
            this.btnLengthAdd.BackColor = System.Drawing.Color.SlateBlue;
            this.btnLengthAdd.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLengthAdd.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnLengthAdd.Image = global::STL.PL.Properties.Resources.plus;
            this.btnLengthAdd.Location = new System.Drawing.Point(76, 38);
            this.btnLengthAdd.Name = "btnLengthAdd";
            this.btnLengthAdd.Size = new System.Drawing.Size(22, 22);
            this.btnLengthAdd.TabIndex = 17;
            this.btnLengthAdd.TabStop = false;
            this.btnLengthAdd.UseVisualStyleBackColor = false;
            this.btnLengthAdd.Click += new System.EventHandler(this.btnLengthAdd_Click);
            // 
            // gbInstallmentDetails
            // 
            this.gbInstallmentDetails.Controls.Add(this.label12);
            this.gbInstallmentDetails.Controls.Add(this.chxDelNonStocks);
            this.gbInstallmentDetails.Controls.Add(this.label20);
            this.gbInstallmentDetails.Controls.Add(this.label19);
            this.gbInstallmentDetails.Controls.Add(this.numRebateDays);
            this.gbInstallmentDetails.Controls.Add(this.label29);
            this.gbInstallmentDetails.Controls.Add(this.numMaxInst);
            this.gbInstallmentDetails.Controls.Add(this.label25);
            this.gbInstallmentDetails.Controls.Add(this.numMinInst);
            this.gbInstallmentDetails.Controls.Add(this.numDefaultInstal);
            this.gbInstallmentDetails.Controls.Add(this.label18);
            this.gbInstallmentDetails.Controls.Add(this.numMonthsIntFree);
            this.gbInstallmentDetails.Controls.Add(this.label17);
            this.gbInstallmentDetails.Controls.Add(this.numMonthsIntDef);
            this.gbInstallmentDetails.Controls.Add(this.label16);
            this.gbInstallmentDetails.Controls.Add(this.chxInstallPreDel);
            this.gbInstallmentDetails.Controls.Add(this.label15);
            this.gbInstallmentDetails.Location = new System.Drawing.Point(160, 8);
            this.gbInstallmentDetails.Name = "gbInstallmentDetails";
            this.gbInstallmentDetails.Size = new System.Drawing.Size(408, 232);
            this.gbInstallmentDetails.TabIndex = 0;
            this.gbInstallmentDetails.TabStop = false;
            this.gbInstallmentDetails.Text = "Details";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(111, 47);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(120, 22);
            this.label12.TabIndex = 19;
            this.label12.Text = "Deliver non stocks";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chxDelNonStocks
            // 
            this.chxDelNonStocks.Location = new System.Drawing.Point(240, 46);
            this.chxDelNonStocks.Name = "chxDelNonStocks";
            this.chxDelNonStocks.Size = new System.Drawing.Size(24, 24);
            this.chxDelNonStocks.TabIndex = 1;
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(304, 191);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(40, 24);
            this.label20.TabIndex = 17;
            this.label20.Text = "days";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(39, 191);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(192, 24);
            this.label19.TabIndex = 16;
            this.label19.Text = "Full rebate if paid within";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numRebateDays
            // 
            this.numRebateDays.Location = new System.Drawing.Point(240, 193);
            this.numRebateDays.Name = "numRebateDays";
            this.numRebateDays.Size = new System.Drawing.Size(56, 23);
            this.numRebateDays.TabIndex = 7;
            this.numRebateDays.Leave += new System.EventHandler(this.numRebateDays_Leave);
            // 
            // label29
            // 
            this.label29.Location = new System.Drawing.Point(39, 166);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(192, 24);
            this.label29.TabIndex = 14;
            this.label29.Text = "Maximum Number of Instalments";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numMaxInst
            // 
            this.numMaxInst.Location = new System.Drawing.Point(240, 168);
            this.numMaxInst.Name = "numMaxInst";
            this.numMaxInst.Size = new System.Drawing.Size(56, 23);
            this.numMaxInst.TabIndex = 6;
            this.numMaxInst.ValueChanged += new System.EventHandler(this.numMaxInst_ValueChanged);
            this.numMaxInst.Validating += new System.ComponentModel.CancelEventHandler(this.numMaxInst_Validating);
            // 
            // label25
            // 
            this.label25.Location = new System.Drawing.Point(39, 141);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(192, 24);
            this.label25.TabIndex = 12;
            this.label25.Text = "Minimum Number of Instalments";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numMinInst
            // 
            this.numMinInst.Location = new System.Drawing.Point(240, 144);
            this.numMinInst.Name = "numMinInst";
            this.numMinInst.Size = new System.Drawing.Size(56, 23);
            this.numMinInst.TabIndex = 5;
            // 
            // numDefaultInstal
            // 
            this.numDefaultInstal.Location = new System.Drawing.Point(240, 120);
            this.numDefaultInstal.Name = "numDefaultInstal";
            this.numDefaultInstal.Size = new System.Drawing.Size(56, 23);
            this.numDefaultInstal.TabIndex = 4;
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(71, 116);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(160, 24);
            this.label18.TabIndex = 9;
            this.label18.Text = "Default Number of Months";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numMonthsIntFree
            // 
            this.numMonthsIntFree.Location = new System.Drawing.Point(240, 96);
            this.numMonthsIntFree.Name = "numMonthsIntFree";
            this.numMonthsIntFree.Size = new System.Drawing.Size(56, 23);
            this.numMonthsIntFree.TabIndex = 3;
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(79, 93);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(152, 22);
            this.label17.TabIndex = 7;
            this.label17.Text = "No. months interest free";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numMonthsIntDef
            // 
            this.numMonthsIntDef.Location = new System.Drawing.Point(240, 72);
            this.numMonthsIntDef.Name = "numMonthsIntDef";
            this.numMonthsIntDef.Size = new System.Drawing.Size(56, 23);
            this.numMonthsIntDef.TabIndex = 2;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(103, 70);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(128, 22);
            this.label16.TabIndex = 5;
            this.label16.Text = "Months deferred";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chxInstallPreDel
            // 
            this.chxInstallPreDel.Location = new System.Drawing.Point(240, 24);
            this.chxInstallPreDel.Name = "chxInstallPreDel";
            this.chxInstallPreDel.Size = new System.Drawing.Size(24, 24);
            this.chxInstallPreDel.TabIndex = 0;
            this.chxInstallPreDel.CheckedChanged += new System.EventHandler(this.chxInstallPreDel_CheckedChanged);
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(55, 24);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(176, 22);
            this.label15.TabIndex = 3;
            this.label15.Text = "Instalment before delivery";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gbStcDetails
            // 
            this.gbStcDetails.Controls.Add(this.label14);
            this.gbStcDetails.Controls.Add(this.txtSTCMinValue);
            this.gbStcDetails.Controls.Add(this.rbSTCMinTypeC);
            this.gbStcDetails.Controls.Add(this.rbSTCMinTypeP);
            this.gbStcDetails.Enabled = false;
            this.gbStcDetails.Location = new System.Drawing.Point(8, 120);
            this.gbStcDetails.Name = "gbStcDetails";
            this.gbStcDetails.Size = new System.Drawing.Size(152, 120);
            this.gbStcDetails.TabIndex = 0;
            this.gbStcDetails.TabStop = false;
            this.gbStcDetails.Text = "STC Min Installment";
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(24, 56);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(40, 16);
            this.label14.TabIndex = 3;
            this.label14.Text = "Value";
            // 
            // txtSTCMinValue
            // 
            this.txtSTCMinValue.Location = new System.Drawing.Point(24, 72);
            this.txtSTCMinValue.Name = "txtSTCMinValue";
            this.txtSTCMinValue.Size = new System.Drawing.Size(100, 23);
            this.txtSTCMinValue.TabIndex = 2;
            this.txtSTCMinValue.Tag = "";
            this.txtSTCMinValue.Leave += new System.EventHandler(this.txtDepositValue_Leave);
            // 
            // rbSTCMinTypeC
            // 
            this.rbSTCMinTypeC.Location = new System.Drawing.Point(72, 24);
            this.rbSTCMinTypeC.Name = "rbSTCMinTypeC";
            this.rbSTCMinTypeC.Size = new System.Drawing.Size(74, 24);
            this.rbSTCMinTypeC.TabIndex = 1;
            this.rbSTCMinTypeC.Text = "Currency";
            // 
            // rbSTCMinTypeP
            // 
            this.rbSTCMinTypeP.Checked = true;
            this.rbSTCMinTypeP.Location = new System.Drawing.Point(24, 24);
            this.rbSTCMinTypeP.Name = "rbSTCMinTypeP";
            this.rbSTCMinTypeP.Size = new System.Drawing.Size(40, 24);
            this.rbSTCMinTypeP.TabIndex = 0;
            this.rbSTCMinTypeP.TabStop = true;
            this.rbSTCMinTypeP.Text = "%";
            // 
            // gbDeposit
            // 
            this.gbDeposit.Controls.Add(this.label13);
            this.gbDeposit.Controls.Add(this.txtDepositValue);
            this.gbDeposit.Controls.Add(this.rbDepositTypeC);
            this.gbDeposit.Controls.Add(this.rbDepositTypeP);
            this.gbDeposit.Location = new System.Drawing.Point(8, 8);
            this.gbDeposit.Name = "gbDeposit";
            this.gbDeposit.Size = new System.Drawing.Size(152, 112);
            this.gbDeposit.TabIndex = 0;
            this.gbDeposit.TabStop = false;
            this.gbDeposit.Text = "Deposit";
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(24, 56);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(40, 16);
            this.label13.TabIndex = 3;
            this.label13.Text = "Value";
            // 
            // txtDepositValue
            // 
            this.txtDepositValue.Location = new System.Drawing.Point(24, 72);
            this.txtDepositValue.Name = "txtDepositValue";
            this.txtDepositValue.Size = new System.Drawing.Size(100, 23);
            this.txtDepositValue.TabIndex = 2;
            this.txtDepositValue.Tag = "";
            this.txtDepositValue.Leave += new System.EventHandler(this.txtDepositValue_Leave);
            // 
            // rbDepositTypeC
            // 
            this.rbDepositTypeC.Location = new System.Drawing.Point(72, 24);
            this.rbDepositTypeC.Name = "rbDepositTypeC";
            this.rbDepositTypeC.Size = new System.Drawing.Size(74, 24);
            this.rbDepositTypeC.TabIndex = 1;
            this.rbDepositTypeC.Text = "Currency";
            // 
            // rbDepositTypeP
            // 
            this.rbDepositTypeP.Checked = true;
            this.rbDepositTypeP.Location = new System.Drawing.Point(24, 24);
            this.rbDepositTypeP.Name = "rbDepositTypeP";
            this.rbDepositTypeP.Size = new System.Drawing.Size(40, 24);
            this.rbDepositTypeP.TabIndex = 0;
            this.rbDepositTypeP.TabStop = true;
            this.rbDepositTypeP.Text = "%";
            // 
            // tpRates
            // 
            this.tpRates.Controls.Add(this.btnAddDateRange);
            this.tpRates.Controls.Add(this.btnRemoveDateRange);
            this.tpRates.Controls.Add(this.gbRateDetails);
            this.tpRates.Controls.Add(this.gbDatePeriod);
            this.tpRates.Controls.Add(this.dgDateRanges);
            this.tpRates.Location = new System.Drawing.Point(0, 25);
            this.tpRates.Name = "tpRates";
            this.tpRates.Selected = false;
            this.tpRates.Size = new System.Drawing.Size(770, 268);
            this.tpRates.TabIndex = 0;
            this.tpRates.Title = "Rates Setup";
            this.tpRates.Visible = false;
            // 
            // btnAddDateRange
            // 
            this.btnAddDateRange.BackColor = System.Drawing.Color.SlateBlue;
            this.btnAddDateRange.Enabled = false;
            this.btnAddDateRange.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddDateRange.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnAddDateRange.Image = global::STL.PL.Properties.Resources.plus;
            this.btnAddDateRange.Location = new System.Drawing.Point(331, 15);
            this.btnAddDateRange.Name = "btnAddDateRange";
            this.btnAddDateRange.Size = new System.Drawing.Size(20, 20);
            this.btnAddDateRange.TabIndex = 28;
            this.btnAddDateRange.TabStop = false;
            this.btnAddDateRange.UseVisualStyleBackColor = false;
            this.btnAddDateRange.Click += new System.EventHandler(this.btnAddDateRange_Click);
            // 
            // btnRemoveDateRange
            // 
            this.btnRemoveDateRange.BackColor = System.Drawing.Color.SlateBlue;
            this.btnRemoveDateRange.Enabled = false;
            this.btnRemoveDateRange.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemoveDateRange.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnRemoveDateRange.Image = global::STL.PL.Properties.Resources.Minus;
            this.btnRemoveDateRange.Location = new System.Drawing.Point(331, 52);
            this.btnRemoveDateRange.Name = "btnRemoveDateRange";
            this.btnRemoveDateRange.Size = new System.Drawing.Size(20, 20);
            this.btnRemoveDateRange.TabIndex = 29;
            this.btnRemoveDateRange.TabStop = false;
            this.btnRemoveDateRange.UseVisualStyleBackColor = false;
            this.btnRemoveDateRange.Click += new System.EventHandler(this.btnRemoveDateRange_Click);
            // 
            // gbRateDetails
            // 
            this.gbRateDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbRateDetails.Controls.Add(this.numAdminValue);
            this.gbRateDetails.Controls.Add(this.label28);
            this.gbRateDetails.Controls.Add(this.label9);
            this.gbRateDetails.Controls.Add(this.numInsPercent);
            this.gbRateDetails.Controls.Add(this.chxIncludeWarranties);
            this.gbRateDetails.Controls.Add(this.label8);
            this.gbRateDetails.Controls.Add(this.drpRateType);
            this.gbRateDetails.Controls.Add(this.numAdminPercent);
            this.gbRateDetails.Controls.Add(this.label6);
            this.gbRateDetails.Controls.Add(this.chxIncludeInsurance);
            this.gbRateDetails.Location = new System.Drawing.Point(7, 154);
            this.gbRateDetails.Name = "gbRateDetails";
            this.gbRateDetails.Size = new System.Drawing.Size(756, 108);
            this.gbRateDetails.TabIndex = 0;
            this.gbRateDetails.TabStop = false;
            this.gbRateDetails.Text = "Rate Details";
            // 
            // numAdminValue
            // 
            this.numAdminValue.DecimalPlaces = 3;
            this.numAdminValue.Location = new System.Drawing.Point(421, 79);
            this.numAdminValue.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numAdminValue.Name = "numAdminValue";
            this.numAdminValue.Size = new System.Drawing.Size(62, 23);
            this.numAdminValue.TabIndex = 8;
            // 
            // label28
            // 
            this.label28.Location = new System.Drawing.Point(252, 79);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(152, 22);
            this.label28.TabIndex = 7;
            this.label28.Text = "Admin Value (Cash Loan)";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(122, 74);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 22);
            this.label9.TabIndex = 6;
            this.label9.Text = "Admin %";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numInsPercent
            // 
            this.numInsPercent.DecimalPlaces = 3;
            this.numInsPercent.Location = new System.Drawing.Point(184, 47);
            this.numInsPercent.Name = "numInsPercent";
            this.numInsPercent.Size = new System.Drawing.Size(62, 23);
            this.numInsPercent.TabIndex = 1;
            // 
            // chxIncludeWarranties
            // 
            this.chxIncludeWarranties.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chxIncludeWarranties.Location = new System.Drawing.Point(457, 60);
            this.chxIncludeWarranties.Name = "chxIncludeWarranties";
            this.chxIncludeWarranties.Size = new System.Drawing.Size(251, 24);
            this.chxIncludeWarranties.TabIndex = 4;
            this.chxIncludeWarranties.Text = "Include Warranty in  Admin Calc";
            this.chxIncludeWarranties.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(12, 44);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(168, 22);
            this.label8.TabIndex = 4;
            this.label8.Text = "Ins % (of Service Charge)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpRateType
            // 
            this.drpRateType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpRateType.Enabled = false;
            this.drpRateType.Location = new System.Drawing.Point(141, 20);
            this.drpRateType.Name = "drpRateType";
            this.drpRateType.Size = new System.Drawing.Size(104, 23);
            this.drpRateType.TabIndex = 0;
            this.drpRateType.SelectionChangeCommitted += new System.EventHandler(this.drpRateType_SelectionChangeCommitted);
            // 
            // numAdminPercent
            // 
            this.numAdminPercent.DecimalPlaces = 3;
            this.numAdminPercent.Location = new System.Drawing.Point(184, 77);
            this.numAdminPercent.Name = "numAdminPercent";
            this.numAdminPercent.Size = new System.Drawing.Size(62, 23);
            this.numAdminPercent.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(71, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 16);
            this.label6.TabIndex = 0;
            this.label6.Text = "Rate Type";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chxIncludeInsurance
            // 
            this.chxIncludeInsurance.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chxIncludeInsurance.Location = new System.Drawing.Point(401, 30);
            this.chxIncludeInsurance.Name = "chxIncludeInsurance";
            this.chxIncludeInsurance.Size = new System.Drawing.Size(307, 24);
            this.chxIncludeInsurance.TabIndex = 3;
            this.chxIncludeInsurance.Text = "Insurance Included In Service Charge";
            this.chxIncludeInsurance.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gbDatePeriod
            // 
            this.gbDatePeriod.Controls.Add(this.drpLoyaltyClub);
            this.gbDatePeriod.Controls.Add(this.lLoyaltyClub);
            this.gbDatePeriod.Controls.Add(this.numDefaultService);
            this.gbDatePeriod.Controls.Add(this.txtPointsTo);
            this.gbDatePeriod.Controls.Add(this.label7);
            this.gbDatePeriod.Controls.Add(this.txtPointsFrom);
            this.gbDatePeriod.Controls.Add(this.drpBand);
            this.gbDatePeriod.Controls.Add(this.lPointsFrom);
            this.gbDatePeriod.Controls.Add(this.lBand);
            this.gbDatePeriod.Controls.Add(this.dtStart);
            this.gbDatePeriod.Controls.Add(this.label3);
            this.gbDatePeriod.Controls.Add(this.lPointsTo);
            this.gbDatePeriod.Location = new System.Drawing.Point(367, 8);
            this.gbDatePeriod.Name = "gbDatePeriod";
            this.gbDatePeriod.Size = new System.Drawing.Size(396, 144);
            this.gbDatePeriod.TabIndex = 0;
            this.gbDatePeriod.TabStop = false;
            this.gbDatePeriod.Text = "Rate Details";
            // 
            // drpLoyaltyClub
            // 
            this.drpLoyaltyClub.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpLoyaltyClub.Items.AddRange(new object[] {
            "",
            "1",
            "2"});
            this.drpLoyaltyClub.Location = new System.Drawing.Point(138, 112);
            this.drpLoyaltyClub.Name = "drpLoyaltyClub";
            this.drpLoyaltyClub.Size = new System.Drawing.Size(177, 23);
            this.drpLoyaltyClub.TabIndex = 3;
            this.drpLoyaltyClub.Visible = false;
            this.drpLoyaltyClub.SelectedIndexChanged += new System.EventHandler(this.drpLoyaltyTier_SelectedIndexChanged);
            // 
            // lLoyaltyClub
            // 
            this.lLoyaltyClub.Location = new System.Drawing.Point(38, 111);
            this.lLoyaltyClub.Name = "lLoyaltyClub";
            this.lLoyaltyClub.Size = new System.Drawing.Size(95, 21);
            this.lLoyaltyClub.TabIndex = 40;
            this.lLoyaltyClub.Text = "Loyalty Club";
            this.lLoyaltyClub.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lLoyaltyClub.Visible = false;
            // 
            // numDefaultService
            // 
            this.numDefaultService.DecimalPlaces = 3;
            this.numDefaultService.Location = new System.Drawing.Point(138, 55);
            this.numDefaultService.Name = "numDefaultService";
            this.numDefaultService.Size = new System.Drawing.Size(62, 23);
            this.numDefaultService.TabIndex = 1;
            // 
            // txtPointsTo
            // 
            this.txtPointsTo.Location = new System.Drawing.Point(328, 84);
            this.txtPointsTo.MaxLength = 2;
            this.txtPointsTo.Name = "txtPointsTo";
            this.txtPointsTo.ReadOnly = true;
            this.txtPointsTo.Size = new System.Drawing.Size(49, 23);
            this.txtPointsTo.TabIndex = 37;
            this.txtPointsTo.TabStop = false;
            this.txtPointsTo.Visible = false;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(7, 55);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(126, 22);
            this.label7.TabIndex = 38;
            this.label7.Text = "Service Charge %";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPointsFrom
            // 
            this.txtPointsFrom.Location = new System.Drawing.Point(248, 84);
            this.txtPointsFrom.MaxLength = 2;
            this.txtPointsFrom.Name = "txtPointsFrom";
            this.txtPointsFrom.ReadOnly = true;
            this.txtPointsFrom.Size = new System.Drawing.Size(49, 23);
            this.txtPointsFrom.TabIndex = 35;
            this.txtPointsFrom.TabStop = false;
            this.txtPointsFrom.Visible = false;
            // 
            // drpBand
            // 
            this.drpBand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBand.Location = new System.Drawing.Point(138, 84);
            this.drpBand.Name = "drpBand";
            this.drpBand.Size = new System.Drawing.Size(57, 23);
            this.drpBand.TabIndex = 2;
            this.drpBand.Visible = false;
            this.drpBand.SelectedIndexChanged += new System.EventHandler(this.drpBand_SelectedIndexChanged);
            // 
            // lPointsFrom
            // 
            this.lPointsFrom.Location = new System.Drawing.Point(172, 85);
            this.lPointsFrom.Name = "lPointsFrom";
            this.lPointsFrom.Size = new System.Drawing.Size(70, 19);
            this.lPointsFrom.TabIndex = 32;
            this.lPointsFrom.Text = "Points";
            this.lPointsFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lPointsFrom.Visible = false;
            // 
            // lBand
            // 
            this.lBand.Location = new System.Drawing.Point(73, 84);
            this.lBand.Name = "lBand";
            this.lBand.Size = new System.Drawing.Size(60, 21);
            this.lBand.TabIndex = 34;
            this.lBand.Text = "Band";
            this.lBand.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lBand.Visible = false;
            // 
            // dtStart
            // 
            this.dtStart.CustomFormat = "ddd dd MMM yyyy";
            this.dtStart.Enabled = false;
            this.dtStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtStart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtStart.Location = new System.Drawing.Point(138, 26);
            this.dtStart.Name = "dtStart";
            this.dtStart.Size = new System.Drawing.Size(112, 23);
            this.dtStart.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(70, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 21);
            this.label3.TabIndex = 31;
            this.label3.Text = "Start";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lPointsTo
            // 
            this.lPointsTo.Location = new System.Drawing.Point(291, 85);
            this.lPointsTo.Name = "lPointsTo";
            this.lPointsTo.Size = new System.Drawing.Size(31, 18);
            this.lPointsTo.TabIndex = 33;
            this.lPointsTo.Text = "To";
            this.lPointsTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lPointsTo.Visible = false;
            // 
            // dgDateRanges
            // 
            this.dgDateRanges.CaptionVisible = false;
            this.dgDateRanges.DataMember = "";
            this.dgDateRanges.Enabled = false;
            this.dgDateRanges.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgDateRanges.Location = new System.Drawing.Point(7, 15);
            this.dgDateRanges.Name = "dgDateRanges";
            this.dgDateRanges.ReadOnly = true;
            this.dgDateRanges.Size = new System.Drawing.Size(316, 137);
            this.dgDateRanges.TabIndex = 0;
            this.dgDateRanges.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgDateRanges_MouseUp);
            // 
            // tpHistory
            // 
            this.tpHistory.Controls.Add(this.dgHistory);
            this.tpHistory.Location = new System.Drawing.Point(0, 25);
            this.tpHistory.Name = "tpHistory";
            this.tpHistory.Selected = false;
            this.tpHistory.Size = new System.Drawing.Size(770, 268);
            this.tpHistory.TabIndex = 8;
            this.tpHistory.Title = "History Rates";
            // 
            // dgHistory
            // 
            this.dgHistory.CaptionVisible = false;
            this.dgHistory.DataMember = "";
            this.dgHistory.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgHistory.Location = new System.Drawing.Point(11, 15);
            this.dgHistory.Name = "dgHistory";
            this.dgHistory.ReadOnly = true;
            this.dgHistory.Size = new System.Drawing.Size(744, 246);
            this.dgHistory.TabIndex = 19;
            // 
            // tpOptRates
            // 
            this.tpOptRates.Controls.Add(this.gbOptionalRates);
            this.tpOptRates.Location = new System.Drawing.Point(0, 25);
            this.tpOptRates.Name = "tpOptRates";
            this.tpOptRates.Selected = false;
            this.tpOptRates.Size = new System.Drawing.Size(770, 268);
            this.tpOptRates.TabIndex = 7;
            this.tpOptRates.Title = "Optional Rates";
            this.tpOptRates.Visible = false;
            // 
            // gbOptionalRates
            // 
            this.gbOptionalRates.Controls.Add(this.label24);
            this.gbOptionalRates.Controls.Add(this.label23);
            this.gbOptionalRates.Controls.Add(this.label22);
            this.gbOptionalRates.Controls.Add(this.numOptRateFromMonth);
            this.gbOptionalRates.Controls.Add(this.numOptRateToMonth);
            this.gbOptionalRates.Controls.Add(this.numOptionalRate);
            this.gbOptionalRates.Controls.Add(this.dgOptionalRates);
            this.gbOptionalRates.Controls.Add(this.btnEnterRate);
            this.gbOptionalRates.Controls.Add(this.btnRemoveRate);
            this.gbOptionalRates.Location = new System.Drawing.Point(87, 25);
            this.gbOptionalRates.Name = "gbOptionalRates";
            this.gbOptionalRates.Size = new System.Drawing.Size(597, 214);
            this.gbOptionalRates.TabIndex = 10;
            this.gbOptionalRates.TabStop = false;
            this.gbOptionalRates.Text = "Optional Rates";
            this.gbOptionalRates.Visible = false;
            // 
            // label24
            // 
            this.label24.Location = new System.Drawing.Point(328, 100);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(56, 16);
            this.label24.TabIndex = 30;
            this.label24.Text = "To Month";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(312, 71);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(72, 16);
            this.label23.TabIndex = 29;
            this.label23.Text = "From Month";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label22
            // 
            this.label22.Location = new System.Drawing.Point(328, 129);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(55, 15);
            this.label22.TabIndex = 28;
            this.label22.Text = "SC %";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numOptRateFromMonth
            // 
            this.numOptRateFromMonth.Location = new System.Drawing.Point(388, 69);
            this.numOptRateFromMonth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numOptRateFromMonth.Name = "numOptRateFromMonth";
            this.numOptRateFromMonth.Size = new System.Drawing.Size(65, 23);
            this.numOptRateFromMonth.TabIndex = 27;
            this.numOptRateFromMonth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numOptRateToMonth
            // 
            this.numOptRateToMonth.Location = new System.Drawing.Point(388, 98);
            this.numOptRateToMonth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numOptRateToMonth.Name = "numOptRateToMonth";
            this.numOptRateToMonth.Size = new System.Drawing.Size(65, 23);
            this.numOptRateToMonth.TabIndex = 26;
            this.numOptRateToMonth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numOptionalRate
            // 
            this.numOptionalRate.DecimalPlaces = 3;
            this.numOptionalRate.Location = new System.Drawing.Point(388, 126);
            this.numOptionalRate.Name = "numOptionalRate";
            this.numOptionalRate.Size = new System.Drawing.Size(65, 23);
            this.numOptionalRate.TabIndex = 4;
            // 
            // dgOptionalRates
            // 
            this.dgOptionalRates.CaptionVisible = false;
            this.dgOptionalRates.DataMember = "";
            this.dgOptionalRates.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgOptionalRates.Location = new System.Drawing.Point(36, 34);
            this.dgOptionalRates.Name = "dgOptionalRates";
            this.dgOptionalRates.ReadOnly = true;
            this.dgOptionalRates.Size = new System.Drawing.Size(234, 152);
            this.dgOptionalRates.TabIndex = 0;
            // 
            // btnEnterRate
            // 
            this.btnEnterRate.BackColor = System.Drawing.Color.SlateBlue;
            this.btnEnterRate.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnterRate.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnEnterRate.Image = global::STL.PL.Properties.Resources.plus;
            this.btnEnterRate.Location = new System.Drawing.Point(533, 129);
            this.btnEnterRate.Name = "btnEnterRate";
            this.btnEnterRate.Size = new System.Drawing.Size(22, 22);
            this.btnEnterRate.TabIndex = 15;
            this.btnEnterRate.UseVisualStyleBackColor = false;
            // 
            // btnRemoveRate
            // 
            this.btnRemoveRate.BackColor = System.Drawing.Color.SlateBlue;
            this.btnRemoveRate.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemoveRate.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnRemoveRate.Image = global::STL.PL.Properties.Resources.Minus;
            this.btnRemoveRate.Location = new System.Drawing.Point(496, 129);
            this.btnRemoveRate.Name = "btnRemoveRate";
            this.btnRemoveRate.Size = new System.Drawing.Size(22, 22);
            this.btnRemoveRate.TabIndex = 16;
            this.btnRemoveRate.UseVisualStyleBackColor = false;
            // 
            // tpLetters
            // 
            this.tpLetters.Controls.Add(this.gbLetters);
            this.tpLetters.Location = new System.Drawing.Point(0, 25);
            this.tpLetters.Name = "tpLetters";
            this.tpLetters.Selected = false;
            this.tpLetters.Size = new System.Drawing.Size(770, 268);
            this.tpLetters.TabIndex = 5;
            this.tpLetters.Title = "Letters And Documents";
            this.tpLetters.Visible = false;
            // 
            // gbLetters
            // 
            this.gbLetters.Controls.Add(this.label11);
            this.gbLetters.Controls.Add(this.chxNoArrearsLetters);
            this.gbLetters.Controls.Add(this.label10);
            this.gbLetters.Controls.Add(this.label26);
            this.gbLetters.Controls.Add(this.drpAgreement);
            this.gbLetters.Controls.Add(this.txtAgreementText);
            this.gbLetters.Location = new System.Drawing.Point(72, 16);
            this.gbLetters.Name = "gbLetters";
            this.gbLetters.Size = new System.Drawing.Size(616, 232);
            this.gbLetters.TabIndex = 7;
            this.gbLetters.TabStop = false;
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.label11.Location = new System.Drawing.Point(80, 27);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(96, 30);
            this.label11.TabIndex = 16;
            this.label11.Text = "No CoSACS Arrears Letters";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chxNoArrearsLetters
            // 
            this.chxNoArrearsLetters.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.chxNoArrearsLetters.Location = new System.Drawing.Point(184, 35);
            this.chxNoArrearsLetters.Name = "chxNoArrearsLetters";
            this.chxNoArrearsLetters.Size = new System.Drawing.Size(16, 24);
            this.chxNoArrearsLetters.TabIndex = 18;
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.label10.Location = new System.Drawing.Point(280, 21);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(80, 16);
            this.label10.TabIndex = 15;
            this.label10.Text = "Agreement";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label26
            // 
            this.label26.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.label26.Location = new System.Drawing.Point(104, 115);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(408, 16);
            this.label26.TabIndex = 20;
            this.label26.Text = "Text to appear on the Agreement print out for this terms type";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpAgreement
            // 
            this.drpAgreement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpAgreement.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.drpAgreement.ItemHeight = 13;
            this.drpAgreement.Location = new System.Drawing.Point(280, 37);
            this.drpAgreement.Name = "drpAgreement";
            this.drpAgreement.Size = new System.Drawing.Size(216, 21);
            this.drpAgreement.TabIndex = 17;
            // 
            // txtAgreementText
            // 
            this.txtAgreementText.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.txtAgreementText.Location = new System.Drawing.Point(104, 133);
            this.txtAgreementText.Multiline = true;
            this.txtAgreementText.Name = "txtAgreementText";
            this.txtAgreementText.Size = new System.Drawing.Size(400, 78);
            this.txtAgreementText.TabIndex = 19;
            this.txtAgreementText.Text = "The customer may take {0} number of payment holidays during the period of the agr" +
    "eement without falling into arrears, provided that he has paid {1} instalments";
            // 
            // tpCashBack
            // 
            this.tpCashBack.Controls.Add(this.gbCashBackOptions);
            this.tpCashBack.Enabled = false;
            this.tpCashBack.Location = new System.Drawing.Point(0, 25);
            this.tpCashBack.Name = "tpCashBack";
            this.tpCashBack.Selected = false;
            this.tpCashBack.Size = new System.Drawing.Size(770, 268);
            this.tpCashBack.TabIndex = 6;
            this.tpCashBack.Title = "CashBack";
            this.tpCashBack.Visible = false;
            // 
            // gbCashBackOptions
            // 
            this.gbCashBackOptions.Controls.Add(this.numCashBackMonth);
            this.gbCashBackOptions.Controls.Add(this.label30);
            this.gbCashBackOptions.Controls.Add(this.label21);
            this.gbCashBackOptions.Controls.Add(this.txtCashBackValue);
            this.gbCashBackOptions.Controls.Add(this.rbCashBackCurrency);
            this.gbCashBackOptions.Controls.Add(this.rbCashBackPerc);
            this.gbCashBackOptions.Location = new System.Drawing.Point(176, 40);
            this.gbCashBackOptions.Name = "gbCashBackOptions";
            this.gbCashBackOptions.Size = new System.Drawing.Size(400, 176);
            this.gbCashBackOptions.TabIndex = 1;
            this.gbCashBackOptions.TabStop = false;
            this.gbCashBackOptions.Text = "Cash Back Options";
            this.gbCashBackOptions.Leave += new System.EventHandler(this.gbCashBackOptions_Leave);
            // 
            // numCashBackMonth
            // 
            this.numCashBackMonth.Location = new System.Drawing.Point(216, 32);
            this.numCashBackMonth.Name = "numCashBackMonth";
            this.numCashBackMonth.Size = new System.Drawing.Size(56, 23);
            this.numCashBackMonth.TabIndex = 8;
            // 
            // label30
            // 
            this.label30.Location = new System.Drawing.Point(88, 32);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(112, 22);
            this.label30.TabIndex = 7;
            this.label30.Text = "Cash Back Month";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(144, 104);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(40, 16);
            this.label21.TabIndex = 3;
            this.label21.Text = "Value";
            // 
            // txtCashBackValue
            // 
            this.txtCashBackValue.Location = new System.Drawing.Point(144, 120);
            this.txtCashBackValue.Name = "txtCashBackValue";
            this.txtCashBackValue.Size = new System.Drawing.Size(100, 23);
            this.txtCashBackValue.TabIndex = 2;
            this.txtCashBackValue.Tag = "";
            // 
            // rbCashBackCurrency
            // 
            this.rbCashBackCurrency.Location = new System.Drawing.Point(192, 72);
            this.rbCashBackCurrency.Name = "rbCashBackCurrency";
            this.rbCashBackCurrency.Size = new System.Drawing.Size(89, 24);
            this.rbCashBackCurrency.TabIndex = 1;
            this.rbCashBackCurrency.Text = "Currency";
            // 
            // rbCashBackPerc
            // 
            this.rbCashBackPerc.Checked = true;
            this.rbCashBackPerc.Location = new System.Drawing.Point(144, 72);
            this.rbCashBackPerc.Name = "rbCashBackPerc";
            this.rbCashBackPerc.Size = new System.Drawing.Size(40, 24);
            this.rbCashBackPerc.TabIndex = 0;
            this.rbCashBackPerc.TabStop = true;
            this.rbCashBackPerc.Text = "%";
            // 
            // tpMmiDetails
            // 
            this.tpMmiDetails.Controls.Add(this.gbMmiDetails);
            this.tpMmiDetails.Location = new System.Drawing.Point(0, 25);
            this.tpMmiDetails.Name = "tpMmiDetails";
            this.tpMmiDetails.Size = new System.Drawing.Size(770, 268);
            this.tpMmiDetails.TabIndex = 7;
            this.tpMmiDetails.Title = "MMI Details";
            this.tpMmiDetails.Visible = false;
            // 
            // gbMmiDetails
            // 
            this.gbMmiDetails.Controls.Add(this.cbIsMmiActive);
            this.gbMmiDetails.Controls.Add(this.lblIsMmiActive);
            this.gbMmiDetails.Controls.Add(this.numMmiThresholdPerc);
            this.gbMmiDetails.Controls.Add(this.lblMmiThresholdPerc);
            this.gbMmiDetails.Location = new System.Drawing.Point(176, 40);
            this.gbMmiDetails.Name = "gbMmiDetails";
            this.gbMmiDetails.Size = new System.Drawing.Size(400, 176);
            this.gbMmiDetails.TabIndex = 1;
            this.gbMmiDetails.TabStop = false;
            this.gbMmiDetails.Text = "MMI Details";
            // 
            // cbIsMmiActive
            // 
            this.cbIsMmiActive.AutoSize = true;
            this.cbIsMmiActive.Location = new System.Drawing.Point(210, 37);
            this.cbIsMmiActive.Name = "cbIsMmiActive";
            this.cbIsMmiActive.Size = new System.Drawing.Size(15, 14);
            this.cbIsMmiActive.TabIndex = 32;
            this.cbIsMmiActive.UseVisualStyleBackColor = true;
            // 
            // lblIsMmiActive
            // 
            this.lblIsMmiActive.Location = new System.Drawing.Point(80, 33);
            this.lblIsMmiActive.Name = "lblIsMmiActive";
            this.lblIsMmiActive.Size = new System.Drawing.Size(124, 22);
            this.lblIsMmiActive.TabIndex = 10;
            this.lblIsMmiActive.Text = "MMI Applicable? ";
            this.lblIsMmiActive.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numMmiThresholdPerc
            // 
            this.numMmiThresholdPerc.DecimalPlaces = 3;
            this.numMmiThresholdPerc.Location = new System.Drawing.Point(210, 59);
            this.numMmiThresholdPerc.Name = "numMmiThresholdPerc";
            this.numMmiThresholdPerc.Size = new System.Drawing.Size(62, 23);
            this.numMmiThresholdPerc.TabIndex = 9;
            // 
            // lblMmiThresholdPerc
            // 
            this.lblMmiThresholdPerc.Location = new System.Drawing.Point(55, 59);
            this.lblMmiThresholdPerc.Name = "lblMmiThresholdPerc";
            this.lblMmiThresholdPerc.Size = new System.Drawing.Size(149, 22);
            this.lblMmiThresholdPerc.TabIndex = 2;
            this.lblMmiThresholdPerc.Text = "Allow MMI Threshold %";
            this.lblMmiThresholdPerc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gbOverview
            // 
            this.gbOverview.BackColor = System.Drawing.SystemColors.Control;
            this.gbOverview.Controls.Add(this.chxSecuritise);
            this.gbOverview.Controls.Add(this.gbIsLoan);
            this.gbOverview.Controls.Add(this.drpStoreType);
            this.gbOverview.Controls.Add(this.label4);
            this.gbOverview.Controls.Add(this.btnBands);
            this.gbOverview.Controls.Add(this.label27);
            this.gbOverview.Controls.Add(this.txtAPR);
            this.gbOverview.Controls.Add(this.btnLoad);
            this.gbOverview.Controls.Add(this.label5);
            this.gbOverview.Controls.Add(this.gbPaymentHoliday);
            this.gbOverview.Controls.Add(this.gbIsActive);
            this.gbOverview.Controls.Add(this.gbAffinity);
            this.gbOverview.Controls.Add(this.lbAccountTypes);
            this.gbOverview.Controls.Add(this.btnCopy);
            this.gbOverview.Controls.Add(this.label2);
            this.gbOverview.Controls.Add(this.txtTermsType);
            this.gbOverview.Controls.Add(this.label1);
            this.gbOverview.Controls.Add(this.txtDescription);
            this.gbOverview.Controls.Add(this.btnExit);
            this.gbOverview.Controls.Add(this.btnClear);
            this.gbOverview.Controls.Add(this.btnSave);
            this.gbOverview.Location = new System.Drawing.Point(8, 0);
            this.gbOverview.Name = "gbOverview";
            this.gbOverview.Size = new System.Drawing.Size(776, 160);
            this.gbOverview.TabIndex = 0;
            this.gbOverview.TabStop = false;
            this.gbOverview.Text = "Overview";
            // 
            // chxSecuritise
            // 
            this.chxSecuritise.AutoSize = true;
            this.chxSecuritise.Location = new System.Drawing.Point(261, 134);
            this.chxSecuritise.Name = "chxSecuritise";
            this.chxSecuritise.Size = new System.Drawing.Size(109, 17);
            this.chxSecuritise.TabIndex = 29;
            this.chxSecuritise.Text = "Do Not Securitise";
            this.chxSecuritise.UseVisualStyleBackColor = true;
            // 
            // gbIsLoan
            // 
            this.gbIsLoan.Controls.Add(this.cbLoanStaff);
            this.gbIsLoan.Controls.Add(this.cbLoanExisting);
            this.gbIsLoan.Controls.Add(this.cbLoanRecent);
            this.gbIsLoan.Controls.Add(this.cbLoanNew);
            this.gbIsLoan.Location = new System.Drawing.Point(391, 53);
            this.gbIsLoan.Name = "gbIsLoan";
            this.gbIsLoan.Size = new System.Drawing.Size(133, 104);
            this.gbIsLoan.TabIndex = 31;
            this.gbIsLoan.TabStop = false;
            this.gbIsLoan.Text = "Is Loan type?";
            this.gbIsLoan.Enter += new System.EventHandler(this.gbIsLoan_Enter);
            // 
            // cbLoanStaff
            // 
            this.cbLoanStaff.AutoSize = true;
            this.cbLoanStaff.Location = new System.Drawing.Point(9, 83);
            this.cbLoanStaff.Name = "cbLoanStaff";
            this.cbLoanStaff.Size = new System.Drawing.Size(48, 17);
            this.cbLoanStaff.TabIndex = 16;
            this.cbLoanStaff.Text = "Staff";
            this.cbLoanStaff.UseVisualStyleBackColor = true;
            this.cbLoanStaff.UseWaitCursor = true;
            this.cbLoanStaff.CheckedChanged += new System.EventHandler(this.cbLoanStaff_CheckedChanged);
            // 
            // cbLoanExisting
            // 
            this.cbLoanExisting.AutoSize = true;
            this.cbLoanExisting.Location = new System.Drawing.Point(9, 61);
            this.cbLoanExisting.Name = "cbLoanExisting";
            this.cbLoanExisting.Size = new System.Drawing.Size(109, 17);
            this.cbLoanExisting.TabIndex = 15;
            this.cbLoanExisting.Text = "Existing Customer";
            this.cbLoanExisting.UseVisualStyleBackColor = true;
            this.cbLoanExisting.CheckedChanged += new System.EventHandler(this.cbLoanExisting_CheckedChanged);
            // 
            // cbLoanRecent
            // 
            this.cbLoanRecent.AutoSize = true;
            this.cbLoanRecent.Location = new System.Drawing.Point(9, 39);
            this.cbLoanRecent.Name = "cbLoanRecent";
            this.cbLoanRecent.Size = new System.Drawing.Size(108, 17);
            this.cbLoanRecent.TabIndex = 14;
            this.cbLoanRecent.Text = "Recent Customer";
            this.cbLoanRecent.UseVisualStyleBackColor = true;
            this.cbLoanRecent.CheckedChanged += new System.EventHandler(this.cbLoanRecent_CheckedChanged);
            // 
            // cbLoanNew
            // 
            this.cbLoanNew.AutoSize = true;
            this.cbLoanNew.Location = new System.Drawing.Point(9, 17);
            this.cbLoanNew.Name = "cbLoanNew";
            this.cbLoanNew.Size = new System.Drawing.Size(95, 17);
            this.cbLoanNew.TabIndex = 13;
            this.cbLoanNew.Text = "New Customer";
            this.cbLoanNew.UseVisualStyleBackColor = true;
            this.cbLoanNew.CheckedChanged += new System.EventHandler(this.cbLoanNew_CheckedChanged);
            // 
            // drpStoreType
            // 
            this.drpStoreType.FormattingEnabled = true;
            this.drpStoreType.Items.AddRange(new object[] {
            "C",
            "N",
            "A"});
            this.drpStoreType.Location = new System.Drawing.Point(329, 130);
            this.drpStoreType.Name = "drpStoreType";
            this.drpStoreType.Size = new System.Drawing.Size(56, 21);
            this.drpStoreType.TabIndex = 30;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(270, 135);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "Store Type";
            // 
            // btnBands
            // 
            this.btnBands.Enabled = false;
            this.btnBands.Location = new System.Drawing.Point(712, 132);
            this.btnBands.Name = "btnBands";
            this.btnBands.Size = new System.Drawing.Size(48, 23);
            this.btnBands.TabIndex = 13;
            this.btnBands.Text = "Bands";
            this.btnBands.Click += new System.EventHandler(this.btnBands_Click);
            // 
            // label27
            // 
            this.label27.Location = new System.Drawing.Point(3, 130);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(188, 22);
            this.label27.TabIndex = 28;
            this.label27.Text = "APR (for printing payment schedule)";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAPR
            // 
            this.txtAPR.Location = new System.Drawing.Point(199, 132);
            this.txtAPR.MaxLength = 6;
            this.txtAPR.Name = "txtAPR";
            this.txtAPR.Size = new System.Drawing.Size(52, 20);
            this.txtAPR.TabIndex = 8;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(64, 29);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(48, 23);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "Load";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(538, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(128, 16);
            this.label5.TabIndex = 26;
            this.label5.Text = "Apply to Account Types";
            // 
            // gbPaymentHoliday
            // 
            this.gbPaymentHoliday.Controls.Add(this.rbPaymentHolidayNo);
            this.gbPaymentHoliday.Controls.Add(this.rbPaymentHolidayYes);
            this.gbPaymentHoliday.Enabled = false;
            this.gbPaymentHoliday.Location = new System.Drawing.Point(130, 72);
            this.gbPaymentHoliday.Name = "gbPaymentHoliday";
            this.gbPaymentHoliday.Size = new System.Drawing.Size(120, 48);
            this.gbPaymentHoliday.TabIndex = 6;
            this.gbPaymentHoliday.TabStop = false;
            this.gbPaymentHoliday.Text = "Payment Holidays";
            // 
            // rbPaymentHolidayNo
            // 
            this.rbPaymentHolidayNo.Checked = true;
            this.rbPaymentHolidayNo.Location = new System.Drawing.Point(58, 16);
            this.rbPaymentHolidayNo.Name = "rbPaymentHolidayNo";
            this.rbPaymentHolidayNo.Size = new System.Drawing.Size(39, 24);
            this.rbPaymentHolidayNo.TabIndex = 80;
            this.rbPaymentHolidayNo.TabStop = true;
            this.rbPaymentHolidayNo.Text = "No";
            // 
            // rbPaymentHolidayYes
            // 
            this.rbPaymentHolidayYes.Location = new System.Drawing.Point(7, 16);
            this.rbPaymentHolidayYes.Name = "rbPaymentHolidayYes";
            this.rbPaymentHolidayYes.Size = new System.Drawing.Size(48, 24);
            this.rbPaymentHolidayYes.TabIndex = 70;
            this.rbPaymentHolidayYes.TabStop = true;
            this.rbPaymentHolidayYes.Text = "Yes";
            this.rbPaymentHolidayYes.CheckedChanged += new System.EventHandler(this.rbPaymentHolidayYes_CheckedChanged);
            this.rbPaymentHolidayYes.Click += new System.EventHandler(this.rbPaymentHolidayYes_Click);
            // 
            // gbIsActive
            // 
            this.gbIsActive.Controls.Add(this.rbActiveNo);
            this.gbIsActive.Controls.Add(this.rbActiveYes);
            this.gbIsActive.Enabled = false;
            this.gbIsActive.Location = new System.Drawing.Point(7, 72);
            this.gbIsActive.Name = "gbIsActive";
            this.gbIsActive.Size = new System.Drawing.Size(120, 48);
            this.gbIsActive.TabIndex = 5;
            this.gbIsActive.TabStop = false;
            this.gbIsActive.Text = "Is Active?";
            // 
            // rbActiveNo
            // 
            this.rbActiveNo.Checked = true;
            this.rbActiveNo.Location = new System.Drawing.Point(58, 16);
            this.rbActiveNo.Name = "rbActiveNo";
            this.rbActiveNo.Size = new System.Drawing.Size(39, 24);
            this.rbActiveNo.TabIndex = 60;
            this.rbActiveNo.TabStop = true;
            this.rbActiveNo.Text = "No";
            // 
            // rbActiveYes
            // 
            this.rbActiveYes.Location = new System.Drawing.Point(7, 16);
            this.rbActiveYes.Name = "rbActiveYes";
            this.rbActiveYes.Size = new System.Drawing.Size(48, 24);
            this.rbActiveYes.TabIndex = 50;
            this.rbActiveYes.TabStop = true;
            this.rbActiveYes.Text = "Yes";
            // 
            // gbAffinity
            // 
            this.gbAffinity.Controls.Add(this.rbAffinityNo);
            this.gbAffinity.Controls.Add(this.rbAffinityYes);
            this.gbAffinity.Enabled = false;
            this.gbAffinity.Location = new System.Drawing.Point(253, 72);
            this.gbAffinity.Name = "gbAffinity";
            this.gbAffinity.Size = new System.Drawing.Size(120, 48);
            this.gbAffinity.TabIndex = 7;
            this.gbAffinity.TabStop = false;
            this.gbAffinity.Text = "Affinity Terms Type?";
            // 
            // rbAffinityNo
            // 
            this.rbAffinityNo.Checked = true;
            this.rbAffinityNo.Location = new System.Drawing.Point(58, 16);
            this.rbAffinityNo.Name = "rbAffinityNo";
            this.rbAffinityNo.Size = new System.Drawing.Size(49, 24);
            this.rbAffinityNo.TabIndex = 100;
            this.rbAffinityNo.TabStop = true;
            this.rbAffinityNo.Text = "No";
            // 
            // rbAffinityYes
            // 
            this.rbAffinityYes.Location = new System.Drawing.Point(7, 16);
            this.rbAffinityYes.Name = "rbAffinityYes";
            this.rbAffinityYes.Size = new System.Drawing.Size(56, 24);
            this.rbAffinityYes.TabIndex = 90;
            this.rbAffinityYes.TabStop = true;
            this.rbAffinityYes.Text = "Yes";
            this.rbAffinityYes.CheckedChanged += new System.EventHandler(this.rbAffinityYes_CheckedChanged);
            // 
            // lbAccountTypes
            // 
            this.lbAccountTypes.Enabled = false;
            this.lbAccountTypes.Location = new System.Drawing.Point(538, 40);
            this.lbAccountTypes.Name = "lbAccountTypes";
            this.lbAccountTypes.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbAccountTypes.Size = new System.Drawing.Size(158, 108);
            this.lbAccountTypes.TabIndex = 4;
            this.lbAccountTypes.Leave += new System.EventHandler(this.lbAccountTypes_Leave);
            // 
            // btnCopy
            // 
            this.btnCopy.Enabled = false;
            this.btnCopy.Location = new System.Drawing.Point(712, 102);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(48, 23);
            this.btnCopy.TabIndex = 12;
            this.btnCopy.Text = "Copy";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "Code";
            // 
            // txtTermsType
            // 
            this.txtTermsType.Location = new System.Drawing.Point(16, 31);
            this.txtTermsType.MaxLength = 2;
            this.txtTermsType.Name = "txtTermsType";
            this.txtTermsType.Size = new System.Drawing.Size(40, 20);
            this.txtTermsType.TabIndex = 1;
            this.txtTermsType.Leave += new System.EventHandler(this.txtTermsType_Leave);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(128, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Terms Type Description";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(128, 31);
            this.txtDescription.MaxLength = 20;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(288, 20);
            this.txtDescription.TabIndex = 3;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(712, 42);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(48, 23);
            this.btnExit.TabIndex = 10;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(712, 72);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(48, 23);
            this.btnClear.TabIndex = 11;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(712, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(48, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // TermsType
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.gbDetails);
            this.Controls.Add(this.gbOverview);
            this.Name = "TermsType";
            this.Text = "Terms Type Maintenance";
            this.Load += new System.EventHandler(this.TermsType_Load);
            this.gbDetails.ResumeLayout(false);
            this.tcDetails.ResumeLayout(false);
            this.tpInstallments.ResumeLayout(false);
            this.gbLengthOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgLengthOptions)).EndInit();
            this.gbInstallmentDetails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numRebateDays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxInst)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinInst)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDefaultInstal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMonthsIntFree)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMonthsIntDef)).EndInit();
            this.gbStcDetails.ResumeLayout(false);
            this.gbStcDetails.PerformLayout();
            this.gbDeposit.ResumeLayout(false);
            this.gbDeposit.PerformLayout();
            this.tpRates.ResumeLayout(false);
            this.gbRateDetails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numAdminValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numInsPercent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAdminPercent)).EndInit();
            this.gbDatePeriod.ResumeLayout(false);
            this.gbDatePeriod.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDefaultService)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgDateRanges)).EndInit();
            this.tpHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgHistory)).EndInit();
            this.tpOptRates.ResumeLayout(false);
            this.gbOptionalRates.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numOptRateFromMonth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOptRateToMonth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOptionalRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgOptionalRates)).EndInit();
            this.tpLetters.ResumeLayout(false);
            this.gbLetters.ResumeLayout(false);
            this.gbLetters.PerformLayout();
            this.tpCashBack.ResumeLayout(false);
            this.gbCashBackOptions.ResumeLayout(false);
            this.gbCashBackOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCashBackMonth)).EndInit();
            this.tpMmiDetails.ResumeLayout(false);
            this.gbMmiDetails.ResumeLayout(false);
            this.gbMmiDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMmiThresholdPerc)).EndInit();
            this.gbOverview.ResumeLayout(false);
            this.gbOverview.PerformLayout();
            this.gbIsLoan.ResumeLayout(false);
            this.gbIsLoan.PerformLayout();
            this.gbPaymentHoliday.ResumeLayout(false);
            this.gbIsActive.ResumeLayout(false);
            this.gbAffinity.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }

        private void LoadStaticData()
        {
            this._serverDate = StaticDataManager.GetServerDate();
            this.dtStart.Value = this._serverDate;
            this.dtStart.MinDate = this._serverDate;
            this._curEndDate = this._serverDate;

            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

            dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AccountType, new string[] { Config.CountryCode, Config.BranchCode }));
            if (StaticData.Tables[TN.InterestRateTypes] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.InterestRateTypes, new string[] { "IRT", "L" }));

            if (StaticData.Tables[TN.AgreementTypes] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AgreementTypes, new string[] { "AGT", "L" }));

            if (StaticData.Tables[TN.CustomerCodes] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CustomerCodes, new string[] { "CC1", "L" }));

            if (StaticData.Tables[TN.TermsTypeBand] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.TermsTypeBand, new string[] { Config.CountryCode }));

            if (dropDowns.DocumentElement.ChildNodes.Count > 0)
            {
                DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        StaticData.Tables[dt.TableName] = dt;
                    }
                }
            }

            DataTable accountTypeTable = ((DataTable)StaticData.Tables[TN.AccountType]).Clone();
            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.AccountType]).Rows)
            {
                // prevent Buy Now Pay Later from appearing in list
                if (AT.IsCreditType((string)row[CN.AcctCat]) 
                        && row[CN.AcctCat].ToString().ToUpper() != "B")
                {
                    DataRow r = accountTypeTable.NewRow();
                    r[CN.AcctCat] = row[CN.AcctCat];
                    r[CN.Description] = row[CN.Description];
                    accountTypeTable.Rows.Add(r);
                }
            }
            lbAccountTypes.DataSource = accountTypeTable;
            lbAccountTypes.DisplayMember = CN.Description;

            this.drpRateType.DataSource = (DataTable)StaticData.Tables[TN.InterestRateTypes];
            this.drpRateType.DisplayMember = CN.CodeDescription;
            this.drpRateType.ValueMember = CN.Code;

            this.drpAgreement.DataSource = (DataTable)StaticData.Tables[TN.AgreementTypes];
            this.drpAgreement.DisplayMember = CN.CodeDescription;
            this.drpAgreement.ValueMember = CN.Code;

            DataTable bandTable = ((DataTable)StaticData.Tables[TN.TermsTypeBand]).Copy();
            DataRow blankBandRow = bandTable.NewRow();
            bandTable.Rows.InsertAt(blankBandRow, 0);
            this.drpBand.DataSource = bandTable;
            this.drpBand.DisplayMember = CN.Band;
            this.drpBand.ValueMember = CN.Band;

            DataTable loyaltyTable = ((DataTable)StaticData.Tables[TN.CustomerCodes]).Clone();
            DataRow blankLoyaltyRow = loyaltyTable.NewRow();
            loyaltyTable.Rows.Add(blankLoyaltyRow);
            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.CustomerCodes]).Rows)
            {
                if ((string)row[CN.Code] == PCCustCodes.Tier1 || (string)row[CN.Code] == PCCustCodes.Tier2)
                {
                    DataRow r = loyaltyTable.NewRow();
                    r[CN.Code] = row[CN.Code];
                    r[CN.Description] = row[CN.Description];
                    loyaltyTable.Rows.Add(r);
                }
            }
            this.drpLoyaltyClub.DataSource = loyaltyTable;
            this.drpLoyaltyClub.DisplayMember = CN.Description;
            this.drpLoyaltyClub.ValueMember = CN.Code;

            // Translate the Tier1/2 Privilege Club titles
            //foreach (DataRow row in ((DataTable)StaticData.Tables[TN.CustomerCodes]).Rows)
            //{
            //    if ((string)row[CN.Code] == PCCustCodes.Tier1)
            //        cbTier1.Text = (string)row[CN.Description];
            //    if ((string)row[CN.Code] == PCCustCodes.Tier2)
            //        cbTier2.Text = (string)row[CN.Description];
            //}
        }

        private void TermsType_Load(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                // Hide the optional rates tab for now
                tcDetails.TabPages.Remove(tpOptRates);

                LoadStaticData();

                dtStart.Value = _curEndDate = this._serverDate;

                txtDepositValue.Tag = rbDepositTypeP;
                txtSTCMinValue.Tag = rbSTCMinTypeP;
                txtCashBackValue.Tag = rbCashBackPerc;
                lbAccountTypes.SelectedIndex = -1;

                bool showBand = ((bool)Country[CountryParameterNames.TermsTypeBandEnabled]);
                this.btnBands.Visible = showBand;
                this.lBand.Visible = showBand;
                this.lPointsFrom.Visible = showBand;
                this.lPointsTo.Visible = showBand;
                this.drpBand.Visible = showBand;
                this.drpBand.Enabled = showBand;
                this.txtPointsFrom.Visible = showBand;
                this.txtPointsTo.Visible = showBand;
                if (showBand)
                {
                    // For scoring bands change the titles to describe the details
                    // applied to one band and those to all bands.
                    gbDatePeriod.Text = GetResource("T_SERVICECHARGEBANDS");
                    gbRateDetails.Text = GetResource("T_RATEDETAILS");
                }

                bool showLoyaltyClub = (bool)Country[CountryParameterNames.TierPCEnabled];
                this.lLoyaltyClub.Visible = showLoyaltyClub;
                this.drpLoyaltyClub.Visible = showLoyaltyClub;

                //IP - 07/11/08 - (70388) - Default the 'Store Type' drop down 
                //to be 'C' - Courts Store when the Terms Type screen is loaded.
                this.drpStoreType.Text = "C";

                this._hasdatachanged = false;
            }
            catch (Exception ex)
            {
                Catch(ex, "TermsType_Load");
            }
            finally
            {
                StopWait();
            }
        }


        private void btnSave_Click(object sender, System.EventArgs e)
        {
            SaveTermsType();
        }


        private void SaveTermsType()
        {
            try
            {
                Wait();
                _detectSelectionChanges = false;
                bool valid = true;

                // Perhaps it would be a good idea to check the Terms Type code is not blank!
                if (this.txtTermsType.Text.Trim() == "")
                {
                    valid = false;
                    errorProvider1.SetError(txtTermsType, GetResource("M_BLANKTERMSTYPECODE"));
                }
                else
                    errorProvider1.SetError(txtTermsType, "");

                // Store Type required
                if (this.drpStoreType.SelectedIndex <0)
                {
                    valid = false;
                    errorProvider1.SetError(drpStoreType, GetResource("M_BLANKSTORETYPE"));
                }
                else
                    errorProvider1.SetError(drpStoreType, "");

                //KEF Added validation before saving termstype
                // Blank description
                if (this.txtDescription.Text == "")
                {
                    valid = false;
                    errorProvider1.SetError(txtDescription, GetResource("M_BLANKTERMSTYPEDESC"));
                }
                else
                    errorProvider1.SetError(txtDescription, "");

                // Length drop down is between min and max no months need loop
                if (this.dgLengthOptions.DataSource != null)
                {
                    foreach (DataRowView row in (DataView)this.dgLengthOptions.DataSource)
                    {
                        if ((int)row[CN.Length] < this.numMinInst.Value || (int)row[CN.Length] > this.numMaxInst.Value)
                        {
                            valid = false;
                            errorProvider1.SetError(dgLengthOptions, GetResource("M_NOBETWEENMINMAX"));
                            break;
                        }
                        else
                            errorProvider1.SetError(dgLengthOptions, "");
                    }
                }

                /*			if(this.numLength.Value < this.numMinInst.Value || this.numLength.Value > this.numMaxInst.Value)
                            {
                                valid = false;
                                errorProvider1.SetError(dgLengthOptions, GetResource("M_NOBETWEENMINMAX"));
                            }
                            else
                                errorProvider1.SetError(dgLengthOptions, "");
                */

                // Default number of months must be between min and max
                if (this.numDefaultInstal.Value < this.numMinInst.Value || this.numDefaultInstal.Value > this.numMaxInst.Value)
                {
                    valid = false;
                    errorProvider1.SetError(numDefaultInstal, GetResource("M_DEFAULTBETWEENMINMAX"));
                }
                else
                    errorProvider1.SetError(numDefaultInstal, "");


                if (!IsStrictNumeric(txtAPR.Text.Trim()))
                {
                    valid = false;
                    errorProvider1.SetError(txtAPR, GetResource("M_NONNUMERIC"));
                }
                else
                    errorProvider1.SetError(txtAPR, "");


                if (rbDepositTypeP.Checked)
                {
                    // validate Deposit% - between 0 and 100 (jec 67941)
                    if ((MoneyStrToDecimal(txtDepositValue.Text.Replace("%", "")) > 100) ||
                        (MoneyStrToDecimal(txtDepositValue.Text.Replace("%", "")) < 0))
                    {
                        valid = false;
                        errorProvider1.SetError(txtDepositValue, GetResource("M_DEPOSITPCENTRANGE"));
                    }
                    else
                        errorProvider1.SetError(txtDepositValue, "");
                }



                if (valid)
                {
                    // Go through the dataset and make sure all the data is
                    // updated and then send it back to the server


                    foreach (DataTable dt in TermsTypeDetails.Tables)
                    {
                        switch (dt.TableName)
                        {
                            case TN.TermsType:
                                txtDepositValue.Text = txtDepositValue.Text.Replace("%", "");
                                txtSTCMinValue.Text = txtSTCMinValue.Text.Replace("%", "");
                                txtCashBackValue.Text = txtCashBackValue.Text.Replace("%", "");
                                if (dt.Rows.Count == 0)
                                {
                                    DataRow r = dt.NewRow();
                                    dt.Rows.Add(r);
                                }
                                foreach (DataRow r in dt.Rows)
                                {
                                    r[CN.Description] = txtDescription.Text;
                                    r[CN.MonthsIntFree] = numMonthsIntFree.Value;
                                    r[CN.DeferredMonths] = numMonthsIntDef.Value;
                                    r[CN.CashBackMonth] = numCashBackMonth.Value;
                                    r[CN.DefaultTerm] = numDefaultInstal.Value;
                                    r[CN.FullRebateDays] = numRebateDays.Value;
                                    r[CN.InstalPreDel] = chxInstallPreDel.Checked ? "Y" : "N";
                                    r[CN.Affinity] = rbAffinityYes.Checked ? "Y" : "N";
                                    r[CN.NoArrearsLetters] = chxNoArrearsLetters.Checked ? 1 : 0;
                                    r[CN.DefaultDeposit] = MoneyStrToDecimal(txtDepositValue.Text);
                                    r[CN.STCAmount] = MoneyStrToDecimal(txtSTCMinValue.Text);
                                    r[CN.CashBackAmount] = MoneyStrToDecimal(txtCashBackValue.Text);
                                    r[CN.STCPc] = rbSTCMinTypeP.Checked ? 1 : 0;
                                    r[CN.DepositIsPercentage] = this.rbDepositTypeP.Checked;
                                    r[CN.CashBackPc] = this.rbCashBackPerc.Checked ? 1 : 0;
                                    r[CN.IsActive] = Convert.ToInt16(rbActiveYes.Checked);
                                    r[CN.PaymentHoliday] = rbPaymentHolidayYes.Checked;
                                    r[CN.AgreementText] = txtAgreementText.Text;
                                    r[CN.MinTerm] = Convert.ToInt32(numMinInst.Value);
                                    r[CN.MaxTerm] = Convert.ToInt32(numMaxInst.Value);
                                    r[CN.AgreementPrint] = drpAgreement.SelectedValue.ToString();
                                    r[CN.DeliverNonStocks] = Convert.ToInt16(chxDelNonStocks.Checked);
                                    r[CN.APR] = txtAPR.Text.Trim();
                                    r[CN.StoreType] = drpStoreType.SelectedItem.ToString(); // CR903
                                    r[CN.IsLoan] = (cbLoanNew.Checked || cbLoanRecent.Checked || cbLoanExisting.Checked || cbLoanStaff.Checked);        //CR906
                                    r["LoanNewCustomer"] = cbLoanNew.Checked;
                                    r["LoanRecentCustomer"] = cbLoanRecent.Checked;
                                    r["LoanExistingCustomer"] = cbLoanExisting.Checked;
                                    r["LoanStaff"] = cbLoanStaff.Checked;
                                    r[CN.DoNotSecuritise] = Convert.ToInt16(chxSecuritise.Checked);
                                    r[CN.IsMmiActive] = cbIsMmiActive.Checked;
                                    r[CN.MmiThresholdPercentage] = numMmiThresholdPerc.Value;
                                    //r[CN.PClubTier1] = cbTier1.Checked ? "Y" : "N";
                                    //r[CN.PClubTier2] = cbTier2.Checked ? "Y" : "N";
                                }
                                break;

                            case TN.TermsTypeAccountType:
                                dt.Clear();
                                foreach (object o in lbAccountTypes.SelectedItems)
                                {
                                    DataRow r = dt.NewRow();
                                    r[CN.TermsType] = txtTermsType.Text;
                                    r[CN.AccountType] = ((DataRowView)o)[CN.AcctCat];
                                    r[CN.Description] = ((DataRowView)o)[CN.Description];
                                    dt.Rows.Add(r);
                                }
                                break;

                            //case TN.IntRateHistory:
                            case TN.TermsTypeLength:
                            case TN.TermsTypeFreeInstallments:
                            case TN.TermsTypeVariableRates:
                                // when we are copying from another Term Type we need to save records for all the
                                // child items we have copied
                                if (this._copy)
                                {
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        dr[CN.TermsType] = txtTermsType.Text;
                                    }
                                }

                                break;

                            default:
                                break;
                        }
                    }

                    // CR806 Only overwrite the current rates
                    TermsTypeDetails.Tables.Remove(TN.IntRateHistory);
                    TermsTypeDetails.Tables.Add(this._currentRates);

                    //  IntRateHistory table has been replaced
                    // if we are copying replace TermType
                    if (this._copy)
                    {
                        foreach (DataRow dr in TermsTypeDetails.Tables[TN.IntRateHistory].Rows)
                            dr[CN.TermsType] = txtTermsType.Text;
                    }

                    // delete rows where a band has not been set and Termstype Banding hs been enabled

                    if ((bool)Country[CountryParameterNames.TermsTypeBandEnabled])         // jec 30/08/07
                    {
                        int rowCount = TermsTypeDetails.Tables[TN.IntRateHistory].Rows.Count;
                        for (int i = 0; i < rowCount; i++)
                        {
                            DataRow dr = TermsTypeDetails.Tables[TN.IntRateHistory].Rows[i];
                            if (String.IsNullOrEmpty(dr[CN.Band].ToString()))
                            {

                                TermsTypeDetails.Tables[TN.IntRateHistory].Rows.Remove(dr);
                                i--;
                                rowCount--;
                            }
                        }
                    }

                    if (numAdminPercent.Value != adminPercent_old)
                    {
                        foreach (DataRow row in TermsTypeDetails.Tables["IntRateHistory"].Rows)
                        {
                            if (Convert.ToDateTime(row["dateto"]).ToShortDateString() == "01/01/1900")
                            {
                                row["AdminPcent"] = numAdminPercent.Value; 
                            }
                        }
                    }

                    if (numAdminValue.Value != adminValue_old)
                    {
                        foreach (DataRow row in TermsTypeDetails.Tables["IntRateHistory"].Rows)
                        {
                            if (Convert.ToDateTime(row["dateto"]).ToShortDateString() == "01/01/1900")
                            {
                                row["AdminValue"] = numAdminValue.Value;
                            }  
                        }
                    }

                    if (numInsPercent.Value != insPercent_old)
                    {
                        foreach (DataRow row in TermsTypeDetails.Tables["IntRateHistory"].Rows)
                        {
                            if (Convert.ToDateTime(row["dateto"]).ToShortDateString() == "01/01/1900")
                            {
                                row["inspcent"] = numInsPercent.Value;
                            }
                        }
                    }

                    //#17856
                    if (chxIncludeInsurance.Checked != includeInsurance_old)
                    {
                        foreach (DataRow row in TermsTypeDetails.Tables["IntRateHistory"].Rows)
                        {
                            row["InsIncluded"] = chxIncludeInsurance.Checked;
                            row["InsIncludedText"] = chxIncludeInsurance.Checked ? "Yes" : "No";
                        }
                    }

                    //#17856
                    if (chxIncludeWarranties.Checked != includeWarranties_old)
                    {
                        foreach (DataRow row in TermsTypeDetails.Tables["IntRateHistory"].Rows)
                        {
                            row["IncludeWarranty"] = chxIncludeWarranties.Checked;
                            row["IncWarrantyText"] = chxIncludeWarranties.Checked ? "Yes" : "No";
                        }
                    }

                    StaticDataManager.SaveTermsTypeDetails(txtTermsType.Text, TermsTypeDetails, out Error);
                    
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        Clear(true);
                        EnableFields(false);
                    }
                    this._hasdatachanged = false;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "SaveTermsType");
            }
            finally
            {
                _detectSelectionChanges = true;
                StopWait();
            }

        }

        private void LoadTermsType()
        {
            try
            {
                Wait();

                _detectSelectionChanges = false;
                string enteredTermsType = txtTermsType.Text;
                Clear(!this._copy);
                txtTermsType.Text = enteredTermsType;

                /* look up the terms type entered and load the details if there are any */
                TermsTypeDetails = StaticDataManager.LoadTermsTypeDetails(txtTermsType.Text, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    TermsTypeDetails.Tables[TN.IntRateHistory].DefaultView.Sort = CN.DateFrom + " DESC," + CN.Band + " ASC," + CN.DateTo + " DESC";

                    // The rates are split into a current view and a history view
                    // Current rates terminate on or after today
                    TermsTypeDetails.Tables[TN.IntRateHistory].DefaultView.RowFilter =  // Date to could be 1st Jan 1900 or sometimes 31st dec 1899 which means current 
                        CN.DateTo + " >= '" + this._serverDate.ToString() + "' OR " + CN.DateTo + " < '" + Date.blankDate.AddDays(1).ToString() + "'";
                    this._currentRates = TermsTypeDetails.Tables[TN.IntRateHistory].DefaultView.ToTable();
                    dgDateRanges.DataSource = this._currentRates.DefaultView;

                    // History rates have terminated before today
                    TermsTypeDetails.Tables[TN.IntRateHistory].DefaultView.RowFilter = // Date to could be 1st Jan 1900 or sometimes 31st dec 1899 which means current so history should not have blank dates
                        CN.DateTo + " < '" + this._serverDate.ToString() + "' AND " + CN.DateTo + " > '" + Date.blankDate.AddDays(1).ToString() + "'";
                    this._historyRates = TermsTypeDetails.Tables[TN.IntRateHistory].DefaultView.ToTable();
                    dgHistory.DataSource = this._historyRates.DefaultView;

                    foreach (DataTable dt in TermsTypeDetails.Tables)
                    {
                        switch (dt.TableName)
                        {
                            case TN.TermsType:
                                // CR906 if termstype is not new disable IsLoan control, 
                                // changing an exosting termstype to isloan will cause problems because 
                                // we check if an account isLoan by the termstype applied
                                gbIsLoan.Enabled = (dt.Rows.Count == 0);

                                foreach (DataRow r in dt.Rows)
                                {
                                    txtDescription.Text = (string)r[CN.Description];
                                    this.numMonthsIntFree.Value = Convert.ToInt32(r[CN.MonthsIntFree]);
                                    this.numMonthsIntDef.Value = Convert.ToInt32(r[CN.DeferredMonths]);
                                    this.numCashBackMonth.Value = Convert.ToInt32(r[CN.CashBackMonth]);
                                    this.numDefaultInstal.Value = Convert.ToInt32(r[CN.DefaultTerm]);
                                    this.numRebateDays.Value = Convert.ToInt32(r[CN.FullRebateDays]);
                                    this.numMinInst.Value = Convert.ToInt32(r[CN.MinTerm]);
                                    this.numMaxInst.Value = Convert.ToInt32(r[CN.MaxTerm]);
                                    this.chxInstallPreDel.Checked = (string)r[CN.InstalPreDel] == "Y" ? true : false;
                                    this.chxDelNonStocks.Checked = Convert.ToBoolean(r[CN.DeliverNonStocks]);
                                    if (Convert.ToInt16(r[CN.avvtt]) == 0) //allow variable terms types 
                                    {
                                        drpRateType.Enabled = false;
                                    }
                                    else
                                    {
                                        drpRateType.Enabled = true;
                                    }
                                    if (chxInstallPreDel.Checked == true)
                                    {
                                        txtDepositValue.Enabled = false;
                                        gbDeposit.Enabled = false;
                                        txtDepositValue.Text = "0";
                                    }
                                    else
                                    {
                                        txtDepositValue.Enabled = true;
                                        gbDeposit.Enabled = true;
                                    }

                                    this.rbAffinityYes.Checked = (string)r[CN.Affinity] == "Y" ? true : false;
                                    this.rbAffinityNo.Checked = !rbAffinityYes.Checked;
                                    this.chxNoArrearsLetters.Checked = Convert.ToBoolean(r[CN.NoArrearsLetters]);

                                    this.rbDepositTypeP.Checked = (bool)r[CN.DepositIsPercentage];
                                    rbDepositTypeC.Checked = !rbDepositTypeP.Checked;
                                    if (rbDepositTypeC.Checked)
                                        txtDepositValue.Text = Convert.ToDecimal(r[CN.DefaultDeposit]).ToString(DecimalPlaces);
                                    else
                                        txtDepositValue.Text = Convert.ToDecimal(r[CN.DefaultDeposit]).ToString() + "%";

                                    this.rbCashBackPerc.Checked = Convert.ToBoolean(r[CN.CashBackPc]);
                                    rbCashBackCurrency.Checked = !rbCashBackPerc.Checked;
                                    if (rbCashBackCurrency.Checked)
                                        txtCashBackValue.Text = Convert.ToDecimal(r[CN.CashBackAmount]).ToString(DecimalPlaces);
                                    else
                                        txtCashBackValue.Text = Convert.ToDecimal(r[CN.CashBackAmount]).ToString() + "%";

                                    this.cbIsMmiActive.Checked = (bool)r[CN.IsMmiActive];
                                    this.numMmiThresholdPerc.Value = Convert.ToDecimal(r[CN.MmiThresholdPercentage]);

                                    rbActiveYes.Checked = Convert.ToBoolean(r[CN.IsActive]);
                                    rbActiveNo.Checked = !rbActiveYes.Checked;

                                    int stcPercSelected = Int32.Parse(r[CN.STCPc].ToString());
                                    rbSTCMinTypeP.Checked = stcPercSelected > 0 ? true : false;
                                    rbSTCMinTypeC.Checked = !rbSTCMinTypeP.Checked;

                                    if (rbSTCMinTypeC.Checked)
                                        txtSTCMinValue.Text = Convert.ToDecimal(r[CN.STCAmount]).ToString(DecimalPlaces);
                                    else
                                        txtSTCMinValue.Text = Convert.ToDecimal(r[CN.STCAmount]).ToString() + "%";

                                    rbPaymentHolidayYes.Checked = Convert.ToBoolean(r[CN.PaymentHoliday]);
                                    rbPaymentHolidayNo.Checked = !rbPaymentHolidayYes.Checked;
                                    txtAgreementText.Text = (string)r[CN.AgreementText];
                                    txtAPR.Text = (string)r[CN.APR];
                                    //cbTier1.Checked = (((string)r[CN.PClubTier1]) == "Y");
                                    //cbTier2.Checked = (((string)r[CN.PClubTier2]) == "Y");

                                    cbLoanNew.Checked = Convert.ToBoolean(r["LoanNewCustomer"]);
                                    cbLoanRecent.Checked = Convert.ToBoolean(r["LoanRecentCustomer"]);
                                    cbLoanExisting.Checked = Convert.ToBoolean(r["LoanExistingCustomer"]);
                                    cbLoanStaff.Checked = Convert.ToBoolean(r["LoanStaff"]);
                                    
                                    //rbLoanYes.Checked = Convert.ToBoolean(r[CN.IsLoan]);    // CR906
                                    //rbLoanNo.Checked = !rbLoanYes.Checked;               // CR906

                                    //Temporarily set DisplayMember to be the Code Value so we can do a match
                                    //against the retrieved value.                                   
                                    drpAgreement.DisplayMember = CN.Code;
                                    string searchString = r[CN.AgreementPrint].ToString();
                                    int i = drpAgreement.FindStringExact(searchString);
                                    if (i != -1)
                                        drpAgreement.SelectedIndex = i;
                                    else
                                        drpAgreement.SelectedIndex = -1;
                                    //Now set DisplayMember back again..
                                    drpAgreement.DisplayMember = CN.CodeDescription;
                                    this.chxSecuritise.Checked = Convert.ToBoolean(r[CN.DoNotSecuritise]);

                                    if (r[CN.StoreType] != DBNull.Value)
                                    {
                                       drpStoreType.SelectedItem = r[CN.StoreType].ToString();
                                    }
                                }

                                EnableFields(this.allowEdit.Enabled);

                                /* hack so that we don't allow certain terms types to
                                * have payment holidays */
                                /* KEF this is incorrect as 90 day termstype has changed so can have any number (do check for this below). Also shouldn't hardcode 2 termstypes to not have payment holidays as different countries use different numbers */
                                /*if(txtDescription.Text.IndexOf("90 DAYS")!=-1 ||
                                    txtTermsType.Text == "92" ||
                                    txtTermsType.Text == "97" )
                                {
                                    gbPaymentHoliday.Enabled = false;
                                    rbPaymentHolidayYes.Checked = false;
                                    rbPaymentHolidayNo.Checked = true;
                                }
                                else*/

                                gbPaymentHoliday.Enabled = this.allowEdit.Enabled;
                                //if (dt.Rows.Count == 0)
                                //{
                                //    this.AddIntHistForDateRange(this._serverDate);
                                //}
                                   
                                
                                break;
                            case TN.TermsTypeAccountType:

                                for (int i = 0; i < lbAccountTypes.Items.Count; i++)
                                    lbAccountTypes.SetSelected(i, false);

                                foreach (DataRow r in dt.Rows)
                                {
                                    int i = lbAccountTypes.FindStringExact((string)r[CN.Description]);
                                    if (i >= 0)
                                    {
                                        lbAccountTypes.SetSelected(i, true);
                                    }
                                }
                                break;

                            case TN.IntRateHistory:
                                if (dt.Rows.Count == 0)
                                {
                                    this.AddIntHistForDateRange(this._serverDate);
                                }
                                if (dgDateRanges.TableStyles.Count == 0)
                                {
                                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                                    tabStyle.MappingName = TN.IntRateHistory;
                                    AddColumnStyle(CN.DateFrom, tabStyle, 80, true, GetResource("T_DATEFROM"), "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.DateTo, tabStyle, 76, true, GetResource("T_DATETO"), "", HorizontalAlignment.Left);
                                    //AddColumnStyle(CN.RateTypeDesc, tabStyle, 76, true, GetResource("T_RATETYPE"), "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.IntRate, tabStyle, 60, true, GetResource("T_RATE"), "F3", HorizontalAlignment.Center);
                                    //AddColumnStyle(CN.InsPcent, tabStyle, 70, true, GetResource("T_INSPERC"), "F2", HorizontalAlignment.Center);				
                                    //AddColumnStyle(CN.AdminPcent, tabStyle, 60, true, GetResource("T_ADMINPERC"), "F2", HorizontalAlignment.Center);				
                                    //AddColumnStyle(CN.IncWarrantyText, tabStyle, 100, true, GetResource("T_INCLUDEWARRANTY"), "", HorizontalAlignment.Left);				
                                    //AddColumnStyle(CN.InsIncludedText, tabStyle, 100, true, GetResource("T_INCLUDEINSURANCE"), "", HorizontalAlignment.Left);				
                                    if ((bool)Country[CountryParameterNames.TermsTypeBandEnabled] || (bool)Country[CountryParameterNames.TierPCEnabled])
                                    {
                                        AddColumnStyle(CN.Band, tabStyle, 40, true, GetResource("T_BAND"), "", HorizontalAlignment.Center);
                                    }
                                    dgDateRanges.TableStyles.Add(tabStyle);
                                }
                                //this._oldDateIndex = -1;
                                PopulateEditableIntRateHist();

                                if (dgHistory.TableStyles.Count == 0)
                                {
                                    DataGridTableStyle tabStyleHistory = new DataGridTableStyle();
                                    tabStyleHistory.MappingName = TN.IntRateHistory;
                                    AddColumnStyle(CN.DateFrom, tabStyleHistory, 80, true, GetResource("T_DATEFROM"), "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.DateTo, tabStyleHistory, 76, true, GetResource("T_DATETO"), "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.RateTypeDesc, tabStyleHistory, 76, true, GetResource("T_RATETYPE"), "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.IntRate, tabStyleHistory, 60, true, GetResource("T_RATE"), "F3", HorizontalAlignment.Center);
                                    AddColumnStyle(CN.InsPcent, tabStyleHistory, 70, true, GetResource("T_INSPERC"), "F2", HorizontalAlignment.Center);
                                    AddColumnStyle(CN.AdminPcent, tabStyleHistory, 60, true, GetResource("T_ADMINPERC"), "F2", HorizontalAlignment.Center);
                                    AddColumnStyle(CN.IncWarrantyText, tabStyleHistory, 100, true, GetResource("T_INCLUDEWARRANTY"), "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.InsIncludedText, tabStyleHistory, 100, true, GetResource("T_INCLUDEINSURANCE"), "", HorizontalAlignment.Left);
                                    if ((bool)Country[CountryParameterNames.TermsTypeBandEnabled] || (bool)Country[CountryParameterNames.TierPCEnabled])
                                    {
                                        AddColumnStyle(CN.Band, tabStyleHistory, 40, true, GetResource("T_BAND"), "", HorizontalAlignment.Center);
                                    }
                                    AddColumnStyle("AdminValue", tabStyleHistory, 60, true, GetResource("T_ADMINVALUE"), "F2", HorizontalAlignment.Center);
                                    dgHistory.TableStyles.Add(tabStyleHistory);
                                }

                                break;

                            case TN.TermsTypeLength:
                                this.dgLengthOptions.DataSource = dt.DefaultView;
                                dt.DefaultView.Sort = CN.Length;
                                if (dgLengthOptions.TableStyles.Count == 0)
                                {
                                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                                    tabStyle.MappingName = TN.TermsTypeLength;
                                    AddColumnStyle(CN.Length, tabStyle, 50, true, GetResource("T_LENGTH"), "", HorizontalAlignment.Left);
                                    dgLengthOptions.TableStyles.Add(tabStyle);
                                }
                                break;

                            case TN.TermsTypeVariableRates:
                                this.dgOptionalRates.DataSource = dt.DefaultView;
                                dt.DefaultView.RowFilter = CN.IntRateFrom + " >= '" + dtStart.Value.ToString() + "' and " +
                                    CN.IntRateTo + " <= '" + _curEndDate.ToString() + "'";
                                if (dgOptionalRates.TableStyles.Count == 0)
                                {
                                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                                    tabStyle.MappingName = TN.TermsTypeVariableRates;

                                    AddColumnStyle(CN.Rate, tabStyle, 50, true, GetResource("T_SERVICECHARGESHORT"), "", HorizontalAlignment.Right);
                                    AddColumnStyle(CN.FromMonth, tabStyle, 50, true, GetResource("T_FROMMONTHSHORT"), "", HorizontalAlignment.Right);
                                    AddColumnStyle(CN.ToMonth, tabStyle, 50, true, GetResource("T_TOMONTHSHORT"), "", HorizontalAlignment.Right);

                                    dgOptionalRates.TableStyles.Add(tabStyle);
                                }
                                break;

                            default:
                                break;
                        }
                    }
                    //Have to explicitly make tpInstallments visible or it just
                    //doesn't appear the very first time - only works by 'switching' tabs
                    tcDetails.SelectedTab = tpCashBack;
                    tcDetails.SelectedTab = tpInstallments;
                    //Enable/Disable STC details depending on Acct type selections..
                    gbStcDetails.Enabled = CheckIfAccountTypeIsSelected(AT.StoreCard) && this.allowEdit.Enabled;
                    //Make Optional Rate details visible (or not) depending on the Rate Type
                    gbOptionalRates.Visible = (drpRateType.SelectedValue.ToString() == RT.Variable);

                    // problem when users create a new terms type and dont click load button,
                    // so Save button is disabled until they have clicked load
                    this.btnSave.Enabled = true;
                }
                this._hasdatachanged = false;
                 insPercent_old = numInsPercent.Value;
                 adminPercent_old = numAdminPercent.Value;
                 adminValue_old = numAdminValue.Value;
                 includeInsurance_old = chxIncludeInsurance.Checked ? true : false;      //#17856
                 includeWarranties_old = chxIncludeWarranties.Checked ? true : false;   //#17856
            }
            catch (Exception ex)
            {
                Catch(ex, "txtTermsType_Leave");
            }
            finally
            {
                _detectSelectionChanges = true;
                StopWait();
            }
        }

        private void txtTermsType_Leave(object sender, System.EventArgs e)
        {
            // If problem with screen loading (looping) comment out below
            // LoadTermsType();
        }

        private void EnableFields(bool enabled)
        {
            gbDeposit.Enabled = enabled;
            gbInstallmentDetails.Enabled = enabled;
            gbLengthOptions.Enabled = enabled;
            gbOptionalRates.Enabled = enabled;
            gbLetters.Enabled = enabled;
            gbCashBackOptions.Enabled = enabled;
            gbRateDetails.Enabled = enabled;
            gbStcDetails.Enabled = enabled;
            gbMmiDetails.Enabled = enabled;
            this.lbAccountTypes.Enabled = enabled;
            this.dtStart.Enabled = enabled;
            this.gbAffinity.Enabled = enabled;
            this.btnAddDateRange.Enabled = enabled;
            this.btnRemoveDateRange.Enabled = enabled;
            this.btnCopy.Enabled = enabled;
            //this.btnSave.Enabled = enabled;
            this.dgDateRanges.Enabled = enabled;
            this.gbIsActive.Enabled = enabled && this.allowActivate.Enabled;
            this.gbPaymentHoliday.Enabled = enabled;
        }

        private void Clear(bool complete)
        {
            _detectSelectionChanges = false;

            /* for a comlpete clear, reset everything */
            if (complete)
            {
                this._copy = false;
                rbDepositTypeP.Checked = true;
                rbCashBackPerc.Checked = true;
                txtDepositValue.Text = "";
                rbSTCMinTypeP.Checked = true;
                txtSTCMinValue.Text = "";
                txtCashBackValue.Text = "";                
                chxInstallPreDel.Checked = false;
                numMonthsIntDef.Value = 0;
                numMonthsIntFree.Value = 0;
                numCashBackMonth.Value = 0;
                numDefaultInstal.Value = 0;
                numRebateDays.Value = 0;
                numMinInst.Value = 0;
                numMaxInst.Value = 0;
                dgLengthOptions.DataSource = null;
                numDefaultService.Value = 0;
                numOptionalRate.Value = 0;
                numOptRateFromMonth.Value = 1;
                numOptRateToMonth.Value = 1;
                numInsPercent.Value = 0;
                numAdminPercent.Value = 0;
                numAdminValue.Value = 0;
                chxIncludeWarranties.Checked = false;
                chxNoArrearsLetters.Checked = false;
                chxIncludeInsurance.Checked = false;
                dgOptionalRates.DataSource = null;
                txtAgreementText.Text = "";
                numLength.Value = numLength.Minimum;
                chxDelNonStocks.Checked = false;
                rbAffinityYes.Checked = false;
                rbAffinityNo.Checked = true;
                cbIsMmiActive.Checked = false;
                numMmiThresholdPerc.Value = 0;
                cbLoanNew.Checked = false;
                cbLoanRecent.Checked = false;
                cbLoanExisting.Checked = false;
                cbLoanStaff.Checked = false;
                //rbLoanYes.Checked = false;          //CR906
                //rbLoanNo.Checked = true;            //CR906
                rbActiveYes.Checked = true;          //CR906
                rbActiveNo.Checked = false;            //CR906
                txtAPR.Text = "";
                chxSecuritise.Checked = false;
                //cbTier1.Checked = false;
                //cbTier2.Checked = false;
                errorProvider1.SetError(this.numDefaultService, "");
                errorProvider1.SetError(drpBand, "");    //jec 19/07/06
            }

            /* if just a copy, just blank the header info */
            txtDescription.Text = "";
            txtTermsType.Text = "";

            for (int i = 0; i < lbAccountTypes.Items.Count; i++)
                lbAccountTypes.SetSelected(i, false);

            dtStart.Value = _curEndDate = this._serverDate;
            dgDateRanges.DataSource = null;
            //_oldDateIndex = -1;

            _detectSelectionChanges = true;
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                Clear(true);
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



        private void PopulateEditableIntRateHist()
        {
            _detectSelectionChanges = false;

            int index = dgDateRanges.CurrentRowIndex;

            if (index >= 0)
            {
                // CR806 - the user must now click the add btn to make any changes
                //
                /* update the datatable with the values from the old index in case the user has 
                 * changed them */
                //if (_oldDateIndex != -1)
                //{
                //    DataRowView oldRow = ((DataView)dgDateRanges.DataSource)[_oldDateIndex];
                //    oldRow[CN.IntRate] = Convert.ToDouble(numDefaultService.Value);
                //    oldRow[CN.DateFrom] = dtStart.Value;
                //    oldRow[CN.InsPcent] = Convert.ToDouble(numInsPercent.Value);
                //    oldRow[CN.AdminPcent] = Convert.ToDouble(numAdminPercent.Value);
                //    oldRow[CN.IncludeWarranty] = Convert.ToInt16(chxIncludeWarranties.Checked);
                //    oldRow[CN.RateType] = (string)((DataRowView)drpRateType.SelectedItem)[CN.Code];
                //    oldRow[CN.InsIncluded] = Convert.ToInt16(chxViewInsurance.Checked);
                // Terms Type score band - the user cannot change the points from/to
                // here, but the screen will change this range if the band is changed.
                //    oldRow[CN.Band] = (string)((DataRowView)drpBand.SelectedItem)[CN.Band];
                //    oldRow[CN.PointsFrom] = Convert.ToInt16(txtPointsFrom.Text);
                //    oldRow[CN.PointsTo] = Convert.ToInt16(txtPointsTo.Text);

                /*oldRow[CN.MinValue] = Convert.ToInt16(numMinInst.Value);
                oldRow[CN.MaxValue] = Convert.ToInt16(numMaxInst.Value);*/

                //}
                //_oldDateIndex = index;

                dgDateRanges.Select(index);

                string band = (string)((DataView)dgDateRanges.DataSource)[index][CN.Band];
                if (band == PCCustCodes.Tier1 || band == PCCustCodes.Tier2)
                {
                    // This is a loyalty club rate
                    drpLoyaltyClub.SelectedValue = band;
                    if (drpBand.SelectedIndex > 0) drpBand.SelectedIndex = 0;
                    txtPointsFrom.Text = "";
                    txtPointsTo.Text = "";
                }
                else
                {
                    // The band must be set before the existing start date
                    if (drpLoyaltyClub.SelectedIndex > 0) drpLoyaltyClub.SelectedIndex = 0;
                    drpBand.Text = band;
                    txtPointsFrom.Text = ((DataView)dgDateRanges.DataSource)[index][CN.PointsFrom].ToString();
                    txtPointsTo.Text = ((DataView)dgDateRanges.DataSource)[index][CN.PointsTo].ToString();
                }

                // Existing Start Date
                DateTime existingStartDate = (DateTime)((DataView)dgDateRanges.DataSource)[index][CN.DateFrom];
                if (existingStartDate > this._serverDate)
                    dtStart.Value = existingStartDate;
                else
                    dtStart.Value = this._serverDate;

                // Other details
                _curEndDate = (DateTime)((DataView)dgDateRanges.DataSource)[index][CN.DateTo];
                numDefaultService.Value = Convert.ToDecimal(((DataView)dgDateRanges.DataSource)[index][CN.IntRate]);
                numInsPercent.Value = Convert.ToDecimal(((DataView)dgDateRanges.DataSource)[index][CN.InsPcent]);
                numAdminPercent.Value = Convert.ToDecimal(((DataView)dgDateRanges.DataSource)[index][CN.AdminPcent]);
                numAdminValue.Value = Convert.ToDecimal(((DataView)dgDateRanges.DataSource)[index]["AdminValue"]);
                //short insincluded = (short)((DataView)dgDateRanges.DataSource)[index][CN.InsIncluded];
                chxIncludeWarranties.Checked = Convert.ToBoolean((short)((DataView)dgDateRanges.DataSource)[index][CN.IncludeWarranty]);
                this.chxIncludeInsurance.Checked = Convert.ToBoolean((short)((DataView)dgDateRanges.DataSource)[index][CN.InsIncluded]);
                // Save values for change comparison    jec 19/07/07
                insPercent = Convert.ToDecimal(((DataView)dgDateRanges.DataSource)[index][CN.InsPcent]);
                adminPercent = Convert.ToDecimal(((DataView)dgDateRanges.DataSource)[index][CN.AdminPcent]);
                adminValue = Convert.ToDecimal(((DataView)dgDateRanges.DataSource)[index]["AdminValue"]);
                includeWarranties = Convert.ToBoolean((short)((DataView)dgDateRanges.DataSource)[index][CN.IncludeWarranty]);
                includeInsurance = Convert.ToBoolean((short)((DataView)dgDateRanges.DataSource)[index][CN.InsIncluded]);
                
                //Temporarily set DisplayMember to be the Code Value so we can do a match
                //against the retrieved value.
                drpRateType.DisplayMember = CN.Code;
                string searchString = (string)((DataView)dgDateRanges.DataSource)[index][CN.RateType];
                int i = drpRateType.FindStringExact(searchString);
                if (i != -1)
                    drpRateType.SelectedIndex = i;
                drpRateType.DisplayMember = CN.CodeDescription;

                if (dgOptionalRates.DataSource != null)
                {
                    ((DataView)dgOptionalRates.DataSource).RowFilter = CN.IntRateFrom + " >= '" + dtStart.Value.ToString() + "' and " +
                        CN.IntRateTo + " <= '" + _curEndDate.ToString() + "'";
                }

                DateTime selectedStartDate = (DateTime)((DataView)dgDateRanges.DataSource)[index][CN.DateFrom];
                DateTime selectedEndDate = (DateTime)((DataView)dgDateRanges.DataSource)[index][CN.DateTo];

                // CR806 - the user must now click the add btn to make any changes
                //if (this._serverDate.CompareTo(selectedStartDate) <= 0)
                //{
                gbRateDetails.Enabled = this.allowEdit.Enabled;
                gbOptionalRates.Enabled = this.allowEdit.Enabled;
                //}
                //else
                //{
                //    gbRateDetails.Enabled = false;
                //    gbOptionalRates.Enabled = false;
                //}
                gbOptionalRates.Visible = (drpRateType.SelectedValue.ToString() == RT.Variable);
            }
            else
            {
                gbRateDetails.Enabled = false;
                gbOptionalRates.Enabled = false;
                gbOptionalRates.Visible = false;
            }
            chxIncludeInsurance.Enabled = gbRateDetails.Enabled;
        }


        private void dgDateRanges_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Wait();

                PopulateEditableIntRateHist();
            }
            catch (Exception ex)
            {
                Catch(ex, "dgDateRanges_MouseUp");
            }
            finally
            {
                _detectSelectionChanges = true;
                StopWait();
            }
        }

        private void btnRemoveDateRange_Click(object sender, System.EventArgs e)
        {
            int index = dgDateRanges.CurrentRowIndex;
            if (index >= 0)
            {
                DateTime intRateFrom = (DateTime)((DataView)dgDateRanges.DataSource)[index][CN.DateFrom];
                DateTime intRateTo = (DateTime)((DataView)dgDateRanges.DataSource)[index][CN.DateTo];

                if (intRateFrom < this._serverDate)
                {
                    ShowInfo("M_STARTDATEPAST");
                }
                else
                {
                    try
                    {
                        Wait();

                        if (index >= 0)
                        {

                            /* remove the corresponding records from the variable rates table */
                            DataView dv = new DataView();
                            dv = (DataView)dgOptionalRates.DataSource;
                            dv.RowFilter = CN.IntRateFrom + " = '" + intRateFrom.ToString() + "' and " +
                                CN.IntRateTo + " = '" + intRateTo.ToString() + "'";
                            for (int i = dv.Count - 1; i >= 0; i--)
                                dv.Delete(i);

                            DataView dvr = (DataView)dgDateRanges.DataSource;
                            int count = dvr.Count;

                            for (int i = count - 1; i >= 0; i--)
                            {
                                if (dgDateRanges.IsSelected(i))
                                {
                                    dvr[i][CN.Band] = "";
                                }
                            }

                            for (int i = count - 1; i >= 0; i--)
                            {
                                if (((string)dvr[i][CN.Band]).Length == 0)
                                    dvr.Delete(i);
                            }

                            //((DataView)dgDateRanges.DataSource).Delete(index);
                            string band = drpBand.SelectedIndex > 0 ? drpBand.SelectedValue.ToString() : drpLoyaltyClub.SelectedValue.ToString();

                            // reset DateTo for Band deleted        jec 20/07/07
                            foreach (DataRowView row in (DataView)dgDateRanges.DataSource)
                            {
                                if ((DateTime)row[CN.DateTo] == (DateTime)dtStart.Value.AddDays(-1) && (string)row[CN.Band] == band)
                                {
                                    row[CN.DateTo] = openDateTo;
                                }
                            }
                            //this._oldDateIndex = -1;
                            PopulateEditableIntRateHist();
                        }
                    }
                    catch (Exception ex)
                    {
                        Catch(ex, "btnRemoveDateRange_Click");
                    }
                    finally
                    {
                        _detectSelectionChanges = true;
                        StopWait();
                    }
                }
            }
        }


        private bool ValidStartDateInsert(DateTime newStartDate)
        {
            _detectSelectionChanges = false;
            _curEndDate = Date.blankDate;
            bool valid = true;
            string band = drpBand.SelectedIndex > 0 ? drpBand.SelectedValue.ToString() : drpLoyaltyClub.SelectedValue.ToString();

            // New start date cannot be before today
            if (newStartDate < this._serverDate)
            {
                valid = false;
                ShowInfo("M_STARTDATEPAST");
            }
            else
            {
                // Find any period that overlaps the new start date
                DataRowView overlapRow = null;

                foreach (DataRowView row in (DataView)dgDateRanges.DataSource)
                {
                    if (band == (string)row[CN.Band]
                        && newStartDate >= (DateTime)row[CN.DateFrom] // Date to could be 1st Jan 1900 or sometimes 31st dec 1899 which means current 
                        && (newStartDate <= (DateTime)row[CN.DateTo] || (DateTime)row[CN.DateTo] < Date.blankDate.AddDays(1) ))
                    {
                        overlapRow = row;
                    }
                }

                if (overlapRow != null)
                {
                    // Check whether this is the same start date to be overwritten
                    if ((DateTime)overlapRow[CN.DateFrom] == newStartDate)
                    {
                        // Use the same end date
                        _curEndDate = (DateTime)overlapRow[CN.DateTo];
                        // Replace this row
                        overlapRow.Delete();
                    }
                    else if ((DateTime)overlapRow[CN.DateFrom] > newStartDate.AddDays(-1))
                    {
                        // The end date of the overlapping period cannot be reduced by a day
                        // so the overlapping period cannot be adjusted
                        valid = false;
                        ShowInfo("M_OVERLAPPINGPERIOD");
                    }
                    else
                    {
                        // A new row will be inserted 
                        // Set the end date of the new period to the end date of the last period
                        _curEndDate = (DateTime)overlapRow[CN.DateTo];
                        // Set the end date of the last period to a day before the new period
                        overlapRow[CN.DateTo] = newStartDate.AddDays(-1);
                    }
                }
            }
            _detectSelectionChanges = true;
            return valid;
        }


        private void AddIntHistForDateRange(DateTime newStartDate)
        {
            //If we have made it this far without an error, we can slot in the
            //new row to the DataView - note this doesn't save the change, that
            //will be done when the user requests the save - further validation
            //will also occur for dates at that time.
            if (ValidStartDateInsert(newStartDate))
            {
                DataRow newRow = ((DataView)dgDateRanges.DataSource).Table.NewRow();
                newRow[CN.TermsType] = txtTermsType.Text;
                newRow[CN.DateFrom] = newStartDate;
                newRow[CN.DateTo] = _curEndDate;
                newRow[CN.IntRate] = Convert.ToDouble(this.numDefaultService.Value);
                newRow[CN.InsPcent] = Convert.ToDouble(this.numInsPercent.Value);
                newRow[CN.AdminPcent] = Convert.ToDouble(this.numAdminPercent.Value);
                newRow["AdminValue"] = Convert.ToDouble(this.numAdminValue.Value);
                newRow[CN.IncludeWarranty] = Convert.ToInt16(this.chxIncludeWarranties.Checked);
                newRow[CN.InsIncluded] = Convert.ToInt16(this.chxIncludeInsurance.Checked);
                newRow[CN.RateType] = "F";
                newRow[CN.Band] = "";
                newRow[CN.PointsFrom] = 0;
                newRow[CN.PointsTo] = 0;

                // Might also need to save as a band or as a loyalty club tier
                if (drpBand.SelectedIndex > 0)
                {
                    newRow[CN.Band] = drpBand.SelectedValue.ToString();
                    newRow[CN.PointsFrom] = IsPositive(txtPointsFrom.Text) ? Convert.ToInt16(txtPointsFrom.Text) : Convert.ToInt16(0);
                    newRow[CN.PointsTo] = IsPositive(txtPointsTo.Text) ? Convert.ToInt16(txtPointsTo.Text) : Convert.ToInt16(0);
                    //// Moved code outside if/else       jec 19/07/07
                    //// Update across all bands with the values common to all bands
                    //foreach (DataRowView row in (DataView)dgDateRanges.DataSource)
                    //{
                    //    string band = (string)row[CN.Band];
                    //    if ((DateTime)row[CN.DateFrom] == newStartDate &&
                    //        band != PCCustCodes.Tier1 && band != PCCustCodes.Tier2)
                    //    {
                    //        row[CN.DateTo] = _curEndDate;
                    //        row[CN.InsPcent] = Convert.ToDouble(this.numInsPercent.Value);
                    //        row[CN.AdminPcent] = Convert.ToDouble(this.numAdminPercent.Value);
                    //        row[CN.IncludeWarranty] = Convert.ToInt16(this.chxIncludeWarranties.Checked);
                    //        row[CN.InsIncluded] = Convert.ToInt16(this.chxIncludeInsurance.Checked);
                    //        row[CN.RateType] = "F";
                    //    }
                    //}
                }
                else if (drpLoyaltyClub.SelectedIndex > 0)
                {
                    newRow[CN.Band] = drpLoyaltyClub.SelectedValue.ToString();
                }

                // Moved code outside if/else       jec 19/07/07
                // Update across all bands with the values common to all bands
                foreach (DataRowView row in (DataView)dgDateRanges.DataSource)
                {
                    string band = (string)row[CN.Band];
                    //if ((DateTime)row[CN.DateFrom] == newStartDate &&
                    //    band != PCCustCodes.Tier1 && band != PCCustCodes.Tier2)
                    // Allow All bands to be updated            jec 19/07/07
                    if ((DateTime)row[CN.DateFrom] == newStartDate)
                    {
                        row[CN.DateTo] = _curEndDate;
                        row[CN.InsPcent] = Convert.ToDouble(this.numInsPercent.Value);
                        row[CN.AdminPcent] = Convert.ToDouble(this.numAdminPercent.Value);
                        row["AdminValue"] = Convert.ToDouble(this.numAdminValue.Value);
                        row[CN.IncludeWarranty] = Convert.ToInt16(this.chxIncludeWarranties.Checked);
                        row[CN.InsIncluded] = Convert.ToInt16(this.chxIncludeInsurance.Checked);
                        row[CN.RateType] = "F";
                    }
                }
                ((DataView)dgDateRanges.DataSource).Table.Rows.Add(newRow);
                ((DataView)dgDateRanges.DataSource).Table.AcceptChanges();
                // If any common rate details have changed then need to add a
                // new row for all other bands                  jec 19/07/07
                if (insPercent != this.numInsPercent.Value ||
                    adminPercent != this.numAdminPercent.Value ||
                    adminValue != this.numAdminValue.Value ||
                    includeWarranties != this.chxIncludeWarranties.Checked ||
                    includeInsurance != this.chxIncludeInsurance.Checked)
                {
                    foreach (DataRowView row in (DataView)dgDateRanges.DataSource)
                    {
                        // Ignore row just added and rows with close date
                        if ((DateTime)row[CN.DateFrom] != newStartDate && (DateTime)row[CN.DateTo] == openDateTo)
                        {
                            DataRow addRow = ((DataView)dgDateRanges.DataSource).Table.NewRow();
                            addRow[CN.TermsType] = row[CN.TermsType];
                            // cannot set DateFrom to newStartDate here due to sort DESC on dataview
                            addRow[CN.DateFrom] = openDateTo;    //newStartDate;
                            addRow[CN.DateTo] = _curEndDate;
                            addRow[CN.IntRate] = row[CN.IntRate];
                            addRow[CN.InsPcent] = Convert.ToDouble(this.numInsPercent.Value);
                            addRow[CN.AdminPcent] = Convert.ToDouble(this.numAdminPercent.Value);
                            addRow["AdminValue"] = Convert.ToDouble(this.numAdminValue.Value);
                            addRow[CN.IncludeWarranty] = Convert.ToInt16(this.chxIncludeWarranties.Checked);
                            addRow[CN.InsIncluded] = Convert.ToInt16(this.chxIncludeInsurance.Checked);
                            addRow[CN.RateType] = "F";
                            addRow[CN.Band] = row[CN.Band];
                            addRow[CN.PointsFrom] = row[CN.PointsFrom];
                            addRow[CN.PointsTo] = row[CN.PointsTo];

                            // set end date of old rate
                            row[CN.DateTo] = newStartDate.AddDays(-1);
                            ((DataView)dgDateRanges.DataSource).Table.Rows.Add(addRow);
                        }

                    }
                    // Now update DateFrom              jec 20/07/07
                    // done here due to sort DESC on dataview causing duplication when adding row 
                    foreach (DataRowView row in (DataView)dgDateRanges.DataSource)
                    {
                        // Ignore row just added and rows with close date
                        if ((DateTime)row[CN.DateFrom] == openDateTo)
                        {
                            row[CN.DateFrom] = newStartDate;
                        }
                    }

                }

                dgDateRanges.Refresh();
                ((DataView)dgDateRanges.DataSource).Sort = CN.DateFrom + " DESC," + CN.Band + " ASC," + CN.DateTo + " DESC";

                // Find the row for the focus
                int index = 0;
                foreach (DataRowView row in (DataView)dgDateRanges.DataSource)
                {
                    if ((DateTime)row[CN.DateFrom] == newStartDate && (string)row[CN.Band] == drpBand.Text)
                        dgDateRanges.Select(index);
                    index++;
                }

                //Reset 'old index' which is used to track unsaved changes..
                //_oldDateIndex = -1;
            }
        }

        //private string ValidateIntRatePeriodDateRanges()
        //{
        //    string errorMessage = string.Empty;
        //    DateTime startDate;
        //    DateTime endDate;

        //    //Date validation doesn't work with 1/1/1900 signifying an open ended
        //    //date range - use 2900 instead for the year of such dates, and put it
        //    //back afterwards!
        //    foreach (DataRowView row in (DataView)dgDateRanges.DataSource)
        //    {
        //        startDate = (DateTime)row[CN.DateFrom];
        //        endDate = (DateTime)row[CN.DateTo];

        //        if (startDate.Year == 1900)
        //        {
        //            startDate = startDate.AddYears(1000);
        //            row[CN.DateFrom] = startDate;
        //        }
        //        if (endDate.Year == 1900)
        //        {
        //            endDate = endDate.AddYears(1000);
        //            row[CN.DateTo] = endDate;
        //        }
        //    }

        //    //Do tests here...
        //    int numberOfPeriods = ((DataView)dgDateRanges.DataSource).Count;
        //    DateTime currentRowFromDate;
        //    DateTime currentRowToDate;
        //    DateTime nextRowFromDate;
        //    DateTime nextRowToDate;

        //    for (int i = 0; i < numberOfPeriods && errorMessage.Length == 0; i++)
        //    {
        //        currentRowFromDate = (DateTime)((DataView)dgDateRanges.DataSource)[i][CN.DateFrom];
        //        currentRowToDate = (DateTime)((DataView)dgDateRanges.DataSource)[i][CN.DateTo];
        //        if (currentRowFromDate > currentRowToDate)
        //        {
        //            errorMessage = "M_STARTDATEAFTEREND";
        //        }

        //        //If there is a period after the current one, validate the boundary
        //        if (errorMessage.Length == 0 && i <= numberOfPeriods - 2)
        //        {
        //            nextRowFromDate = (DateTime)((DataView)dgDateRanges.DataSource)[i + 1][CN.DateFrom];
        //            nextRowToDate = (DateTime)((DataView)dgDateRanges.DataSource)[i + 1][CN.DateTo];

        //            if (nextRowToDate >= currentRowFromDate)
        //            {
        //                errorMessage = "M_DATEOVERLAP";
        //            }
        //        }
        //    }

        //    //Restore 2900 dates to 1900 dates..
        //    foreach (DataRowView row in (DataView)dgDateRanges.DataSource)
        //    {
        //        startDate = (DateTime)row[CN.DateFrom];
        //        endDate = (DateTime)row[CN.DateTo];

        //        if (startDate.Year == 2900)
        //        {
        //            startDate = startDate.AddYears(-1000);
        //            row[CN.DateFrom] = startDate;
        //        }
        //        if (endDate.Year == 2900)
        //        {
        //            endDate = endDate.AddYears(-1000);
        //            row[CN.DateTo] = endDate;
        //        }
        //    }

        //    return errorMessage;
        //}


        private void btnAddDateRange_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                bool valid = false;

                // Check the two field values for Scoring Band and Loyalty Club
                string errMsg = "";
                if (drpBand.Visible && drpLoyaltyClub.Visible)
                {
                    // Both fields are visible so one must have a value
                    if (drpBand.SelectedIndex < 1 && drpLoyaltyClub.SelectedIndex < 1)
                        errMsg = GetResource("M_BANDORLOYALTYREQUIRED");
                }
                else if (drpBand.Visible)
                {
                    // Only the band field is visible so this must have a value
                    if (drpBand.SelectedIndex < 1)
                        errMsg = GetResource("M_BANDREQUIRED");
                }

                valid = (errMsg == "");
                errorProvider1.SetError(drpBand, errMsg);

                // Ins% must be <= Service Charge% (Note the 'Include Ins' tick does NOT include Admin%)
                errMsg = "";
                if (this.chxIncludeInsurance.Checked)
                {
                    // Check the Insurance% does NOT exceed the SC%
                    if (this.numInsPercent.Value > this.numDefaultService.Value)
                    {
                        errMsg = GetResource("M_PERCENTEXCEEDSSERVICE");
                    }
                    else if (drpBand.SelectedIndex > 0 || drpLoyaltyClub.SelectedIndex > 0)     // jec 19/07/07
                    {
                        // Check other bands with the same start date
                        foreach (DataRowView row in (DataView)dgDateRanges.DataSource)
                        {
                            string band = (string)row[CN.Band];
                            if (//(DateTime)row[CN.DateFrom] == this.dtStart.Value &&
                                (DateTime)row[CN.DateTo] == (DateTime)openDateTo &&
                                band != drpBand.Text && 
                                // band != PCCustCodes.Tier1 && band != PCCustCodes.Tier2 &&    jec 19/07/07
                                this.numInsPercent.Value > Convert.ToDecimal((row[CN.IntRate])))
                            {
                                errMsg = GetResource("M_PERCENTEXCEEDSSERVICEBAND", new object[] { band });
                            }
                        }
                    }
                }

                valid = (valid && errMsg == "");
                errorProvider1.SetError(this.numDefaultService, errMsg);

                if (valid) AddIntHistForDateRange(this.dtStart.Value);

            }
            catch (Exception ex)
            {
                Catch(ex, "btnAddDateRange_Click");
            }
            finally
            {
                StopWait();
            }
        }


        private void btnLengthAdd_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                bool valid = true;

                foreach (DataRowView row in (DataView)this.dgLengthOptions.DataSource)
                {
                    if ((int)row[CN.Length] == this.numLength.Value)
                    {
                        valid = false;
                        ShowInfo("M_LENGTHALREADYTHERE");
                    }
                }

                // KEF Length drop down is between min and max no months
                if (this.numLength.Value < this.numMinInst.Value || this.numLength.Value > this.numMaxInst.Value)
                {
                    valid = false;
                    ShowInfo("M_NOBETWEENMINMAX");
                }

                if (valid)
                {
                    DataRow newRow = ((DataView)dgLengthOptions.DataSource).Table.NewRow();
                    newRow[CN.TermsType] = txtTermsType.Text;
                    newRow[CN.Length] = numLength.Value;
                    ((DataView)dgLengthOptions.DataSource).Table.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnLengthAdd_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnLengthRemove_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                int index = dgLengthOptions.CurrentRowIndex;
                if (index >= 0)
                {
                    //((DataView)dgLengthOptions.DataSource).Delete(index);
                    DataRow dr = ((DataView)dgLengthOptions.DataSource).Table.Rows[index];          //#14627(#14205)
                    ((DataView)dgLengthOptions.DataSource).Table.Rows.Remove(dr);
                    
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnLengthRemove_Click");
            }
            finally
            {
                StopWait();
            }
        }

        //private void btnEnterRate_Click(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        Wait();
        //        bool valid = true;

        //        int index = dgDateRanges.CurrentRowIndex;
        //        if (index >= 0)
        //        {
        //            DateTime mainStart = (DateTime)((DataView)dgDateRanges.DataSource)[index][CN.DateFrom];
        //            DateTime mainEnd = (DateTime)((DataView)dgDateRanges.DataSource)[index][CN.DateTo];

        //            /* make sure the from is before the to */
        //            if (numOptRateFromMonth.Value > numOptRateToMonth.Value)
        //                valid = false;

        //            if (valid)
        //            {
        //                /* make sure that the proposed new row will cause no overlaps */
        //                foreach (DataRowView row in (DataView)dgOptionalRates.DataSource)
        //                {
        //                    int frommonth = (int)row[CN.FromMonth];
        //                    int tomonth = (int)row[CN.ToMonth];

        //                    if ((numOptRateFromMonth.Value >= frommonth &&
        //                        numOptRateFromMonth.Value <= tomonth) ||
        //                        (numOptRateToMonth.Value >= frommonth &&
        //                        numOptRateToMonth.Value <= tomonth))
        //                    {
        //                        valid = false;
        //                    }
        //                }
        //            }

        //            if (valid)
        //            {
        //                DataRow newRow = ((DataView)dgOptionalRates.DataSource).Table.NewRow();
        //                newRow[CN.TermsType] = txtTermsType.Text;
        //                newRow[CN.IntRateFrom] = mainStart;
        //                newRow[CN.IntRateTo] = mainEnd;
        //                newRow[CN.FromMonth] = numOptRateFromMonth.Value;
        //                newRow[CN.ToMonth] = numOptRateToMonth.Value;
        //                newRow[CN.Rate] = numOptionalRate.Value;
        //                ((DataView)dgOptionalRates.DataSource).Table.Rows.Add(newRow);
        //                numOptionalRate.Value = 0;
        //                numOptRateFromMonth.Value = 1;
        //                numOptRateToMonth.Value = 1;
        //            }

        //            if (!valid)
        //            {
        //                ShowInfo("M_DATEOVERLAP");
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, "btnEnterRate_Click");
        //    }
        //    finally
        //    {
        //        StopWait();
        //    }
        //}

        //private void btnRemoveRate_Click(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        Wait();

        //        int index = this.dgOptionalRates.CurrentRowIndex;
        //        if (index >= 0)
        //        {
        //            ((DataView)dgOptionalRates.DataSource).Delete(index);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, "btnRemoveRate_Click");
        //    }
        //    finally
        //    {
        //        StopWait();
        //    }
        //}

        private void btnCopy_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                this._copy = true;
                // Should not clear all fields, just code and description
                //Clear(false);
                txtTermsType.Text = "";
                txtDescription.Text = "";
            }
            catch (Exception ex)
            {
                Catch(ex, "btnCopy_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void txtDepositValue_Leave(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                RadioButton rb = (RadioButton)((TextBox)sender).Tag;

                if (rb.Checked)		/* validate as a percentage */
                {
                    ((TextBox)sender).Text = ((TextBox)sender).Text.Replace("%", "");
                    ((TextBox)sender).Text = MoneyStrToDecimal(((TextBox)sender).Text).ToString() + "%";
                    // validate Deposit% - must be between 0 and 100 (jec 67941)
                    if ((MoneyStrToDecimal(((TextBox)sender).Text.Replace("%", "")) < 0) ||
                        (MoneyStrToDecimal(((TextBox)sender).Text.Replace("%", "")) > 100))
                    {
                        errorProvider1.SetError(txtDepositValue, GetResource("M_DEPOSITPCENTRANGE"));
                    }
                    else
                        errorProvider1.SetError(txtDepositValue, "");


                }
                else				/* validate as money */
                {
                    errorProvider1.SetError(txtDepositValue, "");
                    ((TextBox)sender).Text = MoneyStrToDecimal(((TextBox)sender).Text).ToString(DecimalPlaces);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "txtDepositValue_Leave");
            }
            finally
            {
                StopWait();
            }
        }

        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":allowEdit"] = this.allowEdit;
            dynamicMenus[this.Name + ":allowView"] = this.allowView;
            dynamicMenus[this.Name + ":allowActivate"] = this.allowActivate;
        }

        private void rbPaymentHolidayYes_CheckedChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
            }
            catch (Exception ex)
            {
                Catch(ex, "rbPaymentHolidayYes_CheckedChanged");
            }
            finally
            {
                StopWait();
            }
        }

        //private void txtAgreementText_Leave(object sender, System.EventArgs e)
        //{
        //    string x = txtAgreementText.Text;
        //    //int y = 0;
        //}

        /// <summary>
        /// De-select RF and HP entries in the Account type ListBox
        /// </summary>
        //private void DisableRfAndHp()
        //{
        //    _detectSelectionChanges = false;
        //    foreach (DataRowView o in lbAccountTypes.Items)
        //    {
        //        if (o[0].ToString() == "R" || o[0].ToString() == "H")
        //        {
        //            int i = lbAccountTypes.Items.IndexOf(o);
        //            lbAccountTypes.SetSelected(i, false);
        //        }
        //    }
        //    _detectSelectionChanges = true;
        //}

        private string CheckAccountTypes()
        {
            string errorMessage = string.Empty;

            //If Cash is selected, it is invalid to also select Ready Finance or Hire Purchase
            if (CheckIfAccountTypeIsSelected(AT.Cash))
            {
                if (CheckIfAccountTypeIsSelected(AT.ReadyFinance)
                    || CheckIfAccountTypeIsSelected(AT.HP))
                {
                    return "M_CASHINVALID";
                }
            }

            //If STC is selected, no other account type can be selected with it
            if (CheckIfAccountTypeIsSelected(AT.StoreCard))
            {
                if (lbAccountTypes.SelectedItems.Count > 1)
                {
                    return "M_STCONLY";
                }
            }

            return errorMessage;
        }

        private bool CheckIfAccountTypeIsSelected(string acctType)
        {
            foreach (DataRowView o in lbAccountTypes.SelectedItems)
            {
                if (o[CN.AcctCat].ToString() == acctType)
                {
                    return true;
                }
            }
            return false;
        }

        private void lbAccountTypes_Leave(object sender, System.EventArgs e)
        {
            string errorMessage = CheckAccountTypes();
            if (errorMessage.Length > 0)
            {
                ShowInfo(errorMessage);
            }
            gbStcDetails.Enabled = CheckIfAccountTypeIsSelected(AT.StoreCard) && allowEdit.Enabled;
            if (!(gbStcDetails.Enabled))
            {
                rbSTCMinTypeC.Checked = true;
                txtSTCMinValue.Text = Convert.ToDecimal(0).ToString(DecimalPlaces);
            }
        }

        private void gbCashBackOptions_Leave(object sender, System.EventArgs e)
        {
            if (rbCashBackPerc.Checked && txtCashBackValue.Text.Length > 0)
            {
                double specifiedPerc = System.Convert.ToDouble(txtCashBackValue.Text.Replace("%", ""));
                if (specifiedPerc > _MAX_CASHBACK_PERC)
                {
                    ShowInfo("M_CASHBACKPERCTOOHIGH", new object[] { Convert.ToString(_MAX_CASHBACK_PERC) });
                }
            }
        }

        private void drpRateType_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            //Make Optional Rate details visible (or not) depending on the Rate Type
            gbOptionalRates.Visible = (drpRateType.SelectedValue.ToString() == RT.Variable);
        }

        private void btnLoad_Click(object sender, System.EventArgs e)
        {
            if (txtTermsType.Text.Length > 0)
            {
                LoadTermsType();
            }
        }

        public override bool ConfirmClose()
        {
            bool status = true;
            if (this.allowEdit.Enabled)
            {
                if (this._hasdatachanged == true & txtTermsType.Text.Length > 0)
                {
                    if (DialogResult.Yes == ShowInfo("M_SAVECHANGES", MessageBoxButtons.YesNo))
                    {
                        SaveTermsType();
                    }
                }
            }

            return status;
        }


        private bool ValidStartDateChange()
        {
            // Validate the new start date is ok
            _detectSelectionChanges = false;
            bool valid = true;
            string errorMessage = "";
            int index = dgDateRanges.CurrentRowIndex;
            DataRowView currentRow = ((DataView)dgDateRanges.DataSource)[index];
            DateTime newStartDate = dtStart.Value;
            string band = drpBand.Text;

            // New start date cannot be before today
            if (newStartDate < this._serverDate)
            {
                valid = false;
                errorMessage = "M_STARTDATEPAST";
            }
            else if (newStartDate > _curEndDate && _curEndDate != Date.blankDate)
            {
                // New start date cannot be after the end date
                valid = false;
                errorMessage = "M_STARTDATEAFTEREND";
            }
            else
            {
                // Find any period immediately before the current period
                DataRowView previousRow = null;
                DateTime previousMaxStart = Date.blankDate;
                foreach (DataRowView row in (DataView)dgDateRanges.DataSource)
                {
                    if (band == (string)row[CN.Band]
                        && (DateTime)row[CN.DateFrom] > previousMaxStart
                        && (DateTime)row[CN.DateFrom] < (DateTime)currentRow[CN.DateFrom])
                    {
                        previousMaxStart = (DateTime)row[CN.DateFrom];
                        previousRow = row;
                    }
                }

                if (previousRow != null)
                {
                    // Check whether the previous period can be adjusted
                    if ((DateTime)previousRow[CN.DateFrom] > newStartDate.AddDays(-1))
                    {
                        // The end date cannot be reduced by a day
                        valid = false;
                        errorMessage = "M_PREVIOUSPERIOD";
                    }
                    else
                    {
                        // Set the end date of the last period to a day before the new period
                        previousRow[CN.DateTo] = newStartDate.AddDays(-1);
                    }
                }
            }

            if (!valid)
                ShowInfo(errorMessage);

            _detectSelectionChanges = true;
            return valid;
        }


        private void chxInstallPreDel_CheckedChanged(object sender, System.EventArgs e)
        {
            if (chxInstallPreDel.Checked == true)
            {
                txtDepositValue.Enabled = false;
                gbDeposit.Enabled = false;
                txtDepositValue.Text = "0";
            }
            else
            {
                txtDepositValue.Enabled = true;
                gbDeposit.Enabled = true;
            }

        }


        //KEF added as don't want payment holidays if interest free account
        private void numRebateDays_Leave(object sender, System.EventArgs e)
        {
            if (this.numRebateDays.Value != 0 && this.rbPaymentHolidayYes.Checked)
            {
                ShowInfo("M_NOPAYMENTHOLIDAYWITHINTFREE");
                this.numRebateDays.Value = 0;
            }
        }
        //KEF added as don't want payment holidays if interest free account
        private void rbPaymentHolidayYes_Click(object sender, System.EventArgs e)
        {
            if (this.numRebateDays.Value != 0 && this.rbPaymentHolidayYes.Checked)
            {
                ShowInfo("M_NOPAYMENTHOLIDAYWITHINTFREE");
                this.rbPaymentHolidayNo.Checked = true;
                this.rbPaymentHolidayYes.Checked = false;
            }
        }

        private void numMaxInst_ValueChanged(object sender, System.EventArgs e)
        {
            numMaxInst_Validating(this, null);
        }

        private void numMaxInst_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (rbAffinityYes.Checked && numMaxInst.Value > (decimal)Country[CountryParameterNames.AffinityMaxTerm])
                numMaxInst.Value = (decimal)Country[CountryParameterNames.AffinityMaxTerm];
        }

        private void rbAffinityYes_CheckedChanged(object sender, System.EventArgs e)
        {
            numMaxInst_Validating(this, null);
        }

        //private void dtStart_ValueChanged(object sender, System.EventArgs e)
        //{
        //    // The calendar drop down can go into an endless event cycle
        //    // if it is validated using the ValueChanged event,
        //    // so wait for the CloseUp event if the calendar has been opened
        //    if (!this._dtStart_HasDropDown && _detectSelectionChanges)
        //    {
        //        if (ValidStartDateChange())
        //        {
        //            dgDateRanges.Refresh();
        //        }
        //    }
        //}

        //private void dtStart_CloseUp(object sender, System.EventArgs e)
        //{
        //    // The calendar drop down can go into an endless event cycle
        //    // if it is validated using the ValueChanged event,
        //    // so use the CloseUp event if the calendar has been opened
        //    if (_detectSelectionChanges)
        //    {
        //        if (ValidStartDateChange())
        //        {
        //            dgDateRanges.Refresh();
        //        }
        //    }
        //    this._dtStart_HasDropDown = false;
        //}

        //private void dtStart_DropDown(object sender, System.EventArgs e)
        //{
        //    // The calendar drop down can go into an endless event cycle
        //    // if it is validated using the ValueChanged event,
        //    // so wait for the CloseUp event if the calendar has been opened
        //    this._dtStart_HasDropDown = true;
        //}


        private void drpBand_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Wait();

                if (drpBand.SelectedIndex > 0)
                {
                    txtPointsFrom.Text = ((DataRowView)drpBand.SelectedItem)[CN.PointsFrom].ToString();
                    txtPointsTo.Text = ((DataRowView)drpBand.SelectedItem)[CN.PointsTo].ToString();
                    numDefaultService.Value = Convert.ToDecimal(((DataRowView)drpBand.SelectedItem)[CN.ServiceChargePC]);

                    if (drpLoyaltyClub.SelectedIndex > 0)
                    {
                        // Either the Band or the Loyalty Club can be selected but not both
                        drpLoyaltyClub.SelectedIndex = 0;
                    }

                    if (_detectSelectionChanges && dgDateRanges.DataSource != null)
                    {
                        // Highlight the latest rate for this band
                        int bandIndex = 0;
                        DateTime curDateFrom = Date.blankDate;
                        if (dgDateRanges.CurrentRowIndex >= 0) dgDateRanges.UnSelect(dgDateRanges.CurrentRowIndex);
                        for (int i = 1; i < ((DataView)dgDateRanges.DataSource).Count; i++)
                        {
                            if ((string)(((DataView)dgDateRanges.DataSource)[i][CN.Band]) == drpBand.Text &&
                                (DateTime)(((DataView)dgDateRanges.DataSource)[i][CN.DateFrom]) > curDateFrom)
                            {
                                bandIndex = i;
                                curDateFrom = (DateTime)(((DataView)dgDateRanges.DataSource)[i][CN.DateFrom]);
                            }
                        }
                        dgDateRanges.CurrentRowIndex = bandIndex;
                        if (((DataView)dgDateRanges.DataSource).Count > 0)
                        dgDateRanges.Select(bandIndex);
                    }
                }
                else
                {
                    txtPointsFrom.Text = "";
                    txtPointsTo.Text = "";
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "drpBand_SelectedIndexChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnBands_Click(object sender, EventArgs e)
        {
            try
            {
                Wait();

                TermsTypeOverview ttOverview = new TermsTypeOverview();
                ttOverview.ShowDialog();

                // 5.1 UAT Issue 69.  Refresh the rates tab after adjusting rates for all bands.
                Crownwood.Magic.Controls.TabPage currentTab;
                currentTab = tcDetails.SelectedTab;
                LoadTermsType();
                tcDetails.SelectedTab = currentTab;
            }
            catch (Exception ex)
            {
                Catch(ex, "drpBand_SelectedIndexChanged");
            }
            finally
            {
                StopWait();
            }
        }


        private void drpLoyaltyTier_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Wait();

                if (drpLoyaltyClub.SelectedIndex > 0 && drpBand.SelectedIndex > 0)
                {
                    // Either the Band or the Loyalty Club can be selected but not both
                    drpBand.SelectedIndex = 0;
                }

                if (_detectSelectionChanges && dgDateRanges.DataSource != null)
                {
                    // Highlight the latest rate for this band
                    int bandIndex = 0;
                    DateTime curDateFrom = Date.blankDate;
                    if (dgDateRanges.CurrentRowIndex >= 0) dgDateRanges.UnSelect(dgDateRanges.CurrentRowIndex);
                    for (int i = 1; i < ((DataView)dgDateRanges.DataSource).Count; i++)
                    {
                        if ((string)(((DataView)dgDateRanges.DataSource)[i][CN.Band]) == drpLoyaltyClub.SelectedValue.ToString() &&
                            (DateTime)(((DataView)dgDateRanges.DataSource)[i][CN.DateFrom]) > curDateFrom)
                        {
                            bandIndex = i;
                            curDateFrom = (DateTime)(((DataView)dgDateRanges.DataSource)[i][CN.DateFrom]);
                        }
                    }
                    dgDateRanges.CurrentRowIndex = bandIndex;
                    dgDateRanges.Select(bandIndex);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "drpBand_SelectedIndexChanged");
            }
            finally
            {
                StopWait();
            }

        }

        private void gbIsLoan_Enter(object sender, EventArgs e)
        {

        }

        private void cbLoanNew_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableCashLoanCheckBoxes(sender);

            enableDisableAdminValue();
        }

        private void EnableDisableCashLoanCheckBoxes(object sender)
        {
            if (((CheckBox)sender).Name == "cbLoanNew" && ((CheckBox)sender).Checked)
            {
                cbLoanRecent.Checked = false;
                cbLoanRecent.Enabled = false;

                cbLoanExisting.Checked = false;
                cbLoanExisting.Enabled = false;

                cbLoanStaff.Checked = false;
                cbLoanStaff.Enabled = false;
            }
            else if (((CheckBox)sender).Name == "cbLoanNew" && !((CheckBox)sender).Checked)
            {
                cbLoanRecent.Enabled = true;
                cbLoanExisting.Enabled = true;
                cbLoanStaff.Enabled = true;
            }
            else if (((CheckBox)sender).Name == "cbLoanRecent" && ((CheckBox)sender).Checked)
            {
                cbLoanNew.Checked = false;
                cbLoanNew.Enabled = false;

                cbLoanExisting.Checked = false;
                cbLoanExisting.Enabled = false;

                cbLoanStaff.Checked = false;
                cbLoanStaff.Enabled = false;
            }
            else if (((CheckBox)sender).Name == "cbLoanRecent" && !((CheckBox)sender).Checked)
            {
                cbLoanNew.Enabled = true;
                cbLoanExisting.Enabled = true;
                cbLoanStaff.Enabled = true;
            }
            else if (((CheckBox)sender).Name == "cbLoanExisting" && ((CheckBox)sender).Checked)
            {
                cbLoanNew.Checked = false;
                cbLoanNew.Enabled = false;

                cbLoanRecent.Checked = false;
                cbLoanRecent.Enabled = false;

                cbLoanStaff.Checked = false;
                cbLoanStaff.Enabled = false;
            }
            else if (((CheckBox)sender).Name == "cbLoanExisting" && !((CheckBox)sender).Checked)
            {
                cbLoanNew.Enabled = true;
                cbLoanRecent.Enabled = true;
                cbLoanStaff.Enabled = true;
            }
            else if (((CheckBox)sender).Name == "cbLoanStaff" && ((CheckBox)sender).Checked)
            {
                cbLoanNew.Checked = false;
                cbLoanNew.Enabled = false;

                cbLoanRecent.Checked = false;
                cbLoanRecent.Enabled = false;

                cbLoanExisting.Checked = false;
                cbLoanExisting.Enabled = false;
            }
            else if (((CheckBox)sender).Name == "cbLoanStaff" && !((CheckBox)sender).Checked)
            {
                cbLoanNew.Enabled = true;
                cbLoanRecent.Enabled = true;
                cbLoanExisting.Enabled = true;
            }
        }

        private void enableDisableAdminValue()
        {
            if (cbLoanNew.Checked ||
               cbLoanRecent.Checked ||
               cbLoanExisting.Checked ||
               cbLoanStaff.Checked)
            {
                numAdminValue.Enabled = true;
            }
            else
            {
                numAdminValue.Enabled = false;
                numAdminValue.Value = 0;
            }
        }

        private void cbLoanRecent_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableCashLoanCheckBoxes(sender);

            enableDisableAdminValue();
        }

        private void cbLoanExisting_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableCashLoanCheckBoxes(sender);

            enableDisableAdminValue();
        }

        private void cbLoanStaff_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableCashLoanCheckBoxes(sender);

            enableDisableAdminValue();
        }
    }
}
