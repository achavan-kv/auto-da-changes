using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.StoreInfo;               //CR903   jec
using STL.Common.Constants.TableNames;
using STL.Common.Static;
//IP - 7/12/10 - Store Card

namespace STL.PL
{
    /// <summary>
    /// Branch maintenance screen. Branch details such as name, address,
    /// last buff number and last transaction reference are entered here.
    /// Branch deposits can be listed on this screen.
    /// New account numbers and contract numbers can be generated here.
    /// A list of new account numbers and contract numbers is always kept in
    /// reserve so that sales can continue in the event of a system failure.
    /// These numbers are subsequently entered into the system from a paper copy,
    /// instead of being automatically generated.
    /// </summary>
    public class Branch : CommonForm
    {
        private new string Error = "";
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.GroupBox gbBranch;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txtBranchNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.ComboBox drpBranchNo;
        private Crownwood.Magic.Controls.TabPage tbDetails;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label37;
        private Crownwood.Magic.Controls.TabPage tbDeposits;
        private Crownwood.Magic.Controls.TabPage tbGenerate;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.ComboBox drpDeposit;
        private System.Windows.Forms.TextBox txtCroffNo;
        private System.Windows.Forms.TextBox txtTel;
        private System.Windows.Forms.TextBox txtPostCode;
        private System.Windows.Forms.TextBox txtAddress2;
        private System.Windows.Forms.TextBox txtAddress1;
        private System.Windows.Forms.TextBox txtBranchName;
        private System.Windows.Forms.TextBox txtAddress3;
        private System.Windows.Forms.TextBox txtRegion;
        private System.Windows.Forms.TextBox txtAS400BranchNo;
        private System.Windows.Forms.TextBox txtOldCard;
        private System.Windows.Forms.TextBox txtCountryCode;
        private System.Windows.Forms.TextBox txtHiTransRefNo;
        private System.Windows.Forms.TextBox txtWarehouseNo;
        private System.Windows.Forms.TextBox txtHissn;
        private System.Windows.Forms.TextBox txtNewCard;
        private System.Windows.Forms.TextBox txtHiBuffNo;
        private Crownwood.Magic.Controls.TabControl tcBranch;
        private System.Windows.Forms.DateTimePicker dtCardChange;
        private System.Windows.Forms.GroupBox gbContractNo;
        public System.Windows.Forms.Button btnPrintContractNo;
        private System.Windows.Forms.GroupBox gbAccountNo;
        private System.Windows.Forms.ComboBox drpAccountType;
        public System.Windows.Forms.Button btnPrintAccountNo;
        private System.Windows.Forms.CheckBox chxDepositLocked;
        private System.Windows.Forms.NumericUpDown numContractNo;
        private System.Windows.Forms.NumericUpDown numAccountNo;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGrid dgDepositList;

        private bool _userChanged = false;
        private DataView _depositListView = null;
        private System.Windows.Forms.Label lDeposits;
        private System.Windows.Forms.Label lDetails;
        private System.Windows.Forms.Label lNumGeneration;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtWarehouseRegion;
        private CheckBox cbCreateRFAccts;
        private CheckBox cbCreateCashAccts;
        private Label label3;
        private TextBox txtFact2000BranchLetter;
        private ComboBox drpStoreType;
        private CheckBox cbScoreHPbefore;
        private CheckBox cbCreateHPAccts;
        private CheckBox cbServiceRepairCentre;
        private CheckBox Chk_behavioural;
        private GroupBox grp3PLWarehouse;
        private Label lbl3PLDefaultPrintLocn;
        private ComboBox drpDefaultPrintLocation;
        private CheckBox chkWarehouse;
        private CheckBox cbCreateStoreAccts;
        private TextBox texttestvisible;
        private Crownwood.Magic.Controls.TabPage tbStoreCardQualRules;
        private NumericUpDown numPcentInitRFLimit;
        private Label lblPcentInitialLimit;
        private NumericUpDown numMaxPrevMnthsInArrsX;
        private Label lblPrevMaxMnthsInArrs;
        private NumericUpDown numMaxCurrMnthsInArrs;
        private Label lblMaxCurrMnthsInArrs;
        private NumericUpDown numMinMnthsAcctHistX;
        private Label lblMinMnthsAcctHistX;
        private NumericUpDown numMinBehaviouralScore;
        private Label lblMinBehaviouralScore;
        private NumericUpDown numMinAppScore;
        private Label lblMinAppScore;
        private GroupBox grpStoreCardRules;
        private CheckBox chkMinAvailRFLimit;
        private CheckBox chkMaxPrevMthsInArrs;
        private CheckBox chkMaxCurrMthsInArrs;
        private CheckBox chkMinMthsAcctHist;
        private CheckBox chkMinBehaviouralScore;
        private CheckBox chkMinApplicationScore;
        private ErrorProvider errorProviderStoreCard;
        private Label lStoreCardQualRules;
        private Label lblStoreCardCriteria;
        private Label lblEnableDisableStoreCardRules;
        private GroupBox grpApplyTo;
        private RadioButton rbNonCourtsBranches;
        private RadioButton rbCourtsBranches;
        private RadioButton rbAllBranches;
        private RadioButton rbCurrentBranch;
        private Label lblStoreCardApplyTo;
        private ToolTip toolTipStoreCard;
        private Label lblIn1;
        private NumericUpDown numMinMnthsAcctHistY;
        private Label lblMonths1;
        private Label lblIn2;
        private NumericUpDown numMaxPrevMnthsInArrsY;
        private Label lblMonths2;
        private NumericUpDown numMaxCust;
        private Label lblMaxCustForPreApp;
        private CheckBox chkMaxNoCust;
        private Label lblInfo;
        private CheckBox cbCashLoanBranch;
        private GroupBox grpNonCourtsStoreTypes;
        private RadioButton rbAshley;
        private RadioButton rbLuckyDollar;
        private IContainer components;



        public Branch(TranslationDummy dummy)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });

            texttestvisible.Visible = true;

            //if ((bool)Country[CountryParameterNames.StoreCardEnabled])
            //    cbCreateStoreAccts.Visible = true;
        }

        public Branch()
        {
            InitializeComponent();
            dynamicMenus = new Hashtable();
            HashMenus();
            ApplyRoleRestrictions();
            texttestvisible.Visible = true;
            //if ((bool)Country[CountryParameterNames.StoreCardEnabled])
            cbCreateStoreAccts.Visible = true;
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
        }


        private void HashMenus()
        {
            dynamicMenus[this.Name + ":lDetails"] = this.lDetails;
            dynamicMenus[this.Name + ":lDeposits"] = this.lDeposits;
            dynamicMenus[this.Name + ":lNumGeneration"] = this.lNumGeneration;
            dynamicMenus[this.Name + ":lStoreCardQualRules"] = this.lStoreCardQualRules;  //IP - 9/2/10 - Store Card
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Branch));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.toolTipStoreCard = new System.Windows.Forms.ToolTip(this.components);
            this.tcBranch = new Crownwood.Magic.Controls.TabControl();
            this.tbDetails = new Crownwood.Magic.Controls.TabPage();
            this.texttestvisible = new System.Windows.Forms.TextBox();
            this.cbCreateStoreAccts = new System.Windows.Forms.CheckBox();
            this.Chk_behavioural = new System.Windows.Forms.CheckBox();
            this.cbServiceRepairCentre = new System.Windows.Forms.CheckBox();
            this.txtFact2000BranchLetter = new System.Windows.Forms.TextBox();
            this.cbCreateHPAccts = new System.Windows.Forms.CheckBox();
            this.cbScoreHPbefore = new System.Windows.Forms.CheckBox();
            this.cbCreateCashAccts = new System.Windows.Forms.CheckBox();
            this.cbCreateRFAccts = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtWarehouseRegion = new System.Windows.Forms.TextBox();
            this.chxDepositLocked = new System.Windows.Forms.CheckBox();
            this.txtCroffNo = new System.Windows.Forms.TextBox();
            this.txtTel = new System.Windows.Forms.TextBox();
            this.txtPostCode = new System.Windows.Forms.TextBox();
            this.txtAddress2 = new System.Windows.Forms.TextBox();
            this.txtAddress1 = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.dtCardChange = new System.Windows.Forms.DateTimePicker();
            this.txtBranchName = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.txtAddress3 = new System.Windows.Forms.TextBox();
            this.txtRegion = new System.Windows.Forms.TextBox();
            this.txtAS400BranchNo = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.txtOldCard = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.txtCountryCode = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.txtHiTransRefNo = new System.Windows.Forms.TextBox();
            this.txtWarehouseNo = new System.Windows.Forms.TextBox();
            this.txtHissn = new System.Windows.Forms.TextBox();
            this.txtNewCard = new System.Windows.Forms.TextBox();
            this.txtHiBuffNo = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.tbDeposits = new Crownwood.Magic.Controls.TabPage();
            this.drpDeposit = new System.Windows.Forms.ComboBox();
            this.dgDepositList = new System.Windows.Forms.DataGrid();
            this.tbGenerate = new Crownwood.Magic.Controls.TabPage();
            this.gbContractNo = new System.Windows.Forms.GroupBox();
            this.btnPrintContractNo = new System.Windows.Forms.Button();
            this.numContractNo = new System.Windows.Forms.NumericUpDown();
            this.gbAccountNo = new System.Windows.Forms.GroupBox();
            this.label38 = new System.Windows.Forms.Label();
            this.drpAccountType = new System.Windows.Forms.ComboBox();
            this.btnPrintAccountNo = new System.Windows.Forms.Button();
            this.numAccountNo = new System.Windows.Forms.NumericUpDown();
            this.tbStoreCardQualRules = new Crownwood.Magic.Controls.TabPage();
            this.lblInfo = new System.Windows.Forms.Label();
            this.grpApplyTo = new System.Windows.Forms.GroupBox();
            this.rbNonCourtsBranches = new System.Windows.Forms.RadioButton();
            this.rbCourtsBranches = new System.Windows.Forms.RadioButton();
            this.rbAllBranches = new System.Windows.Forms.RadioButton();
            this.rbCurrentBranch = new System.Windows.Forms.RadioButton();
            this.lblStoreCardApplyTo = new System.Windows.Forms.Label();
            this.grpStoreCardRules = new System.Windows.Forms.GroupBox();
            this.chkMaxNoCust = new System.Windows.Forms.CheckBox();
            this.numMaxCust = new System.Windows.Forms.NumericUpDown();
            this.lblMaxCustForPreApp = new System.Windows.Forms.Label();
            this.lblMonths2 = new System.Windows.Forms.Label();
            this.numMaxPrevMnthsInArrsY = new System.Windows.Forms.NumericUpDown();
            this.lblIn2 = new System.Windows.Forms.Label();
            this.lblMonths1 = new System.Windows.Forms.Label();
            this.numMinMnthsAcctHistY = new System.Windows.Forms.NumericUpDown();
            this.lblIn1 = new System.Windows.Forms.Label();
            this.chkMinAvailRFLimit = new System.Windows.Forms.CheckBox();
            this.chkMaxPrevMthsInArrs = new System.Windows.Forms.CheckBox();
            this.chkMaxCurrMthsInArrs = new System.Windows.Forms.CheckBox();
            this.chkMinMthsAcctHist = new System.Windows.Forms.CheckBox();
            this.chkMinBehaviouralScore = new System.Windows.Forms.CheckBox();
            this.chkMinApplicationScore = new System.Windows.Forms.CheckBox();
            this.lblStoreCardCriteria = new System.Windows.Forms.Label();
            this.lblEnableDisableStoreCardRules = new System.Windows.Forms.Label();
            this.numPcentInitRFLimit = new System.Windows.Forms.NumericUpDown();
            this.numMaxPrevMnthsInArrsX = new System.Windows.Forms.NumericUpDown();
            this.numMaxCurrMnthsInArrs = new System.Windows.Forms.NumericUpDown();
            this.numMinMnthsAcctHistX = new System.Windows.Forms.NumericUpDown();
            this.numMinBehaviouralScore = new System.Windows.Forms.NumericUpDown();
            this.numMinAppScore = new System.Windows.Forms.NumericUpDown();
            this.lblMinAppScore = new System.Windows.Forms.Label();
            this.lblPcentInitialLimit = new System.Windows.Forms.Label();
            this.lblMinBehaviouralScore = new System.Windows.Forms.Label();
            this.lblPrevMaxMnthsInArrs = new System.Windows.Forms.Label();
            this.lblMinMnthsAcctHistX = new System.Windows.Forms.Label();
            this.lblMaxCurrMnthsInArrs = new System.Windows.Forms.Label();
            this.gbBranch = new System.Windows.Forms.GroupBox();
            this.grpNonCourtsStoreTypes = new System.Windows.Forms.GroupBox();
            this.rbAshley = new System.Windows.Forms.RadioButton();
            this.rbLuckyDollar = new System.Windows.Forms.RadioButton();
            this.cbCashLoanBranch = new System.Windows.Forms.CheckBox();
            this.grp3PLWarehouse = new System.Windows.Forms.GroupBox();
            this.drpDefaultPrintLocation = new System.Windows.Forms.ComboBox();
            this.lStoreCardQualRules = new System.Windows.Forms.Label();
            this.chkWarehouse = new System.Windows.Forms.CheckBox();
            this.lbl3PLDefaultPrintLocn = new System.Windows.Forms.Label();
            this.lDetails = new System.Windows.Forms.Label();
            this.lNumGeneration = new System.Windows.Forms.Label();
            this.lDeposits = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.drpStoreType = new System.Windows.Forms.ComboBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.label23 = new System.Windows.Forms.Label();
            this.txtBranchNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.drpBranchNo = new System.Windows.Forms.ComboBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProviderStoreCard = new System.Windows.Forms.ErrorProvider(this.components);
            this.tcBranch.SuspendLayout();
            this.tbDetails.SuspendLayout();
            this.tbDeposits.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDepositList)).BeginInit();
            this.tbGenerate.SuspendLayout();
            this.gbContractNo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numContractNo)).BeginInit();
            this.gbAccountNo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAccountNo)).BeginInit();
            this.tbStoreCardQualRules.SuspendLayout();
            this.grpApplyTo.SuspendLayout();
            this.grpStoreCardRules.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxCust)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxPrevMnthsInArrsY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinMnthsAcctHistY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPcentInitRFLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxPrevMnthsInArrsX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxCurrMnthsInArrs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinMnthsAcctHistX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinBehaviouralScore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinAppScore)).BeginInit();
            this.gbBranch.SuspendLayout();
            this.grpNonCourtsStoreTypes.SuspendLayout();
            this.grp3PLWarehouse.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderStoreCard)).BeginInit();
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
            // tcBranch
            // 
            this.tcBranch.Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiDocument;
            this.tcBranch.IDEPixelArea = true;
            this.tcBranch.IDEPixelBorder = false;
            this.tcBranch.Location = new System.Drawing.Point(8, 88);
            this.tcBranch.Name = "tcBranch";
            this.tcBranch.SelectedIndex = 0;
            this.tcBranch.SelectedTab = this.tbDetails;
            this.tcBranch.Size = new System.Drawing.Size(776, 384);
            this.tcBranch.TabIndex = 0;
            this.tcBranch.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tbDetails,
            this.tbDeposits,
            this.tbGenerate,
            this.tbStoreCardQualRules});
            this.tcBranch.SelectionChanged += new System.EventHandler(this.tcBranch_SelectionChanged);
            // 
            // tbDetails
            // 
            this.tbDetails.Controls.Add(this.texttestvisible);
            this.tbDetails.Controls.Add(this.cbCreateStoreAccts);
            this.tbDetails.Controls.Add(this.Chk_behavioural);
            this.tbDetails.Controls.Add(this.cbServiceRepairCentre);
            this.tbDetails.Controls.Add(this.txtFact2000BranchLetter);
            this.tbDetails.Controls.Add(this.cbCreateHPAccts);
            this.tbDetails.Controls.Add(this.cbScoreHPbefore);
            this.tbDetails.Controls.Add(this.cbCreateCashAccts);
            this.tbDetails.Controls.Add(this.cbCreateRFAccts);
            this.tbDetails.Controls.Add(this.label2);
            this.tbDetails.Controls.Add(this.txtWarehouseRegion);
            this.tbDetails.Controls.Add(this.chxDepositLocked);
            this.tbDetails.Controls.Add(this.txtCroffNo);
            this.tbDetails.Controls.Add(this.txtTel);
            this.tbDetails.Controls.Add(this.txtPostCode);
            this.tbDetails.Controls.Add(this.txtAddress2);
            this.tbDetails.Controls.Add(this.txtAddress1);
            this.tbDetails.Controls.Add(this.label19);
            this.tbDetails.Controls.Add(this.label20);
            this.tbDetails.Controls.Add(this.dtCardChange);
            this.tbDetails.Controls.Add(this.txtBranchName);
            this.tbDetails.Controls.Add(this.label22);
            this.tbDetails.Controls.Add(this.label24);
            this.tbDetails.Controls.Add(this.label25);
            this.tbDetails.Controls.Add(this.txtAddress3);
            this.tbDetails.Controls.Add(this.txtRegion);
            this.tbDetails.Controls.Add(this.txtAS400BranchNo);
            this.tbDetails.Controls.Add(this.label26);
            this.tbDetails.Controls.Add(this.label27);
            this.tbDetails.Controls.Add(this.txtOldCard);
            this.tbDetails.Controls.Add(this.label28);
            this.tbDetails.Controls.Add(this.label29);
            this.tbDetails.Controls.Add(this.label30);
            this.tbDetails.Controls.Add(this.txtCountryCode);
            this.tbDetails.Controls.Add(this.label31);
            this.tbDetails.Controls.Add(this.label32);
            this.tbDetails.Controls.Add(this.label33);
            this.tbDetails.Controls.Add(this.label34);
            this.tbDetails.Controls.Add(this.txtHiTransRefNo);
            this.tbDetails.Controls.Add(this.txtWarehouseNo);
            this.tbDetails.Controls.Add(this.txtHissn);
            this.tbDetails.Controls.Add(this.txtNewCard);
            this.tbDetails.Controls.Add(this.txtHiBuffNo);
            this.tbDetails.Controls.Add(this.label35);
            this.tbDetails.Controls.Add(this.label36);
            this.tbDetails.Controls.Add(this.label37);
            this.tbDetails.Location = new System.Drawing.Point(0, 25);
            this.tbDetails.Name = "tbDetails";
            this.tbDetails.Size = new System.Drawing.Size(776, 359);
            this.tbDetails.TabIndex = 0;
            this.tbDetails.Title = "Branch Details";
            // 
            // texttestvisible
            // 
            this.texttestvisible.Location = new System.Drawing.Point(66, 320);
            this.texttestvisible.Name = "texttestvisible";
            this.texttestvisible.Size = new System.Drawing.Size(139, 23);
            this.texttestvisible.TabIndex = 290;
            this.texttestvisible.Visible = false;
            // 
            // cbCreateStoreAccts
            // 
            this.cbCreateStoreAccts.AutoSize = true;
            this.cbCreateStoreAccts.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbCreateStoreAccts.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbCreateStoreAccts.Location = new System.Drawing.Point(598, 236);
            this.cbCreateStoreAccts.Name = "cbCreateStoreAccts";
            this.cbCreateStoreAccts.Size = new System.Drawing.Size(143, 19);
            this.cbCreateStoreAccts.TabIndex = 289;
            this.cbCreateStoreAccts.Text = "Create Store Accounts";
            this.cbCreateStoreAccts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbCreateStoreAccts.UseVisualStyleBackColor = true;
            this.cbCreateStoreAccts.Visible = false;
            // 
            // Chk_behavioural
            // 
            this.Chk_behavioural.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Chk_behavioural.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Chk_behavioural.Location = new System.Drawing.Point(584, 277);
            this.Chk_behavioural.Name = "Chk_behavioural";
            this.Chk_behavioural.Size = new System.Drawing.Size(157, 24);
            this.Chk_behavioural.TabIndex = 288;
            this.Chk_behavioural.Text = "Use Behavioural Scoring";
            this.Chk_behavioural.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbServiceRepairCentre
            // 
            this.cbServiceRepairCentre.AutoSize = true;
            this.cbServiceRepairCentre.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbServiceRepairCentre.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbServiceRepairCentre.Location = new System.Drawing.Point(604, 302);
            this.cbServiceRepairCentre.Name = "cbServiceRepairCentre";
            this.cbServiceRepairCentre.Size = new System.Drawing.Size(137, 19);
            this.cbServiceRepairCentre.TabIndex = 287;
            this.cbServiceRepairCentre.Text = "Service Repair Centre";
            this.cbServiceRepairCentre.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbServiceRepairCentre.UseVisualStyleBackColor = true;
            // 
            // txtFact2000BranchLetter
            // 
            this.txtFact2000BranchLetter.Location = new System.Drawing.Point(152, 87);
            this.txtFact2000BranchLetter.MaxLength = 1;
            this.txtFact2000BranchLetter.Name = "txtFact2000BranchLetter";
            this.txtFact2000BranchLetter.Size = new System.Drawing.Size(72, 23);
            this.txtFact2000BranchLetter.TabIndex = 112;
            this.txtFact2000BranchLetter.Text = " ";
            // 
            // cbCreateHPAccts
            // 
            this.cbCreateHPAccts.AutoSize = true;
            this.cbCreateHPAccts.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbCreateHPAccts.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbCreateHPAccts.Location = new System.Drawing.Point(435, 277);
            this.cbCreateHPAccts.Name = "cbCreateHPAccts";
            this.cbCreateHPAccts.Size = new System.Drawing.Size(132, 19);
            this.cbCreateHPAccts.TabIndex = 286;
            this.cbCreateHPAccts.Text = "Create HP Accounts";
            this.cbCreateHPAccts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbCreateHPAccts.UseVisualStyleBackColor = true;
            // 
            // cbScoreHPbefore
            // 
            this.cbScoreHPbefore.AutoSize = true;
            this.cbScoreHPbefore.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbScoreHPbefore.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbScoreHPbefore.Location = new System.Drawing.Point(390, 302);
            this.cbScoreHPbefore.Name = "cbScoreHPbefore";
            this.cbScoreHPbefore.Size = new System.Drawing.Size(177, 19);
            this.cbScoreHPbefore.TabIndex = 285;
            this.cbScoreHPbefore.Text = "Credit score HP before items";
            this.cbScoreHPbefore.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbScoreHPbefore.UseVisualStyleBackColor = true;
            // 
            // cbCreateCashAccts
            // 
            this.cbCreateCashAccts.AutoSize = true;
            this.cbCreateCashAccts.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbCreateCashAccts.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbCreateCashAccts.Location = new System.Drawing.Point(425, 255);
            this.cbCreateCashAccts.Name = "cbCreateCashAccts";
            this.cbCreateCashAccts.Size = new System.Drawing.Size(142, 19);
            this.cbCreateCashAccts.TabIndex = 284;
            this.cbCreateCashAccts.Text = "Create Cash Accounts";
            this.cbCreateCashAccts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbCreateCashAccts.UseVisualStyleBackColor = true;
            // 
            // cbCreateRFAccts
            // 
            this.cbCreateRFAccts.AutoSize = true;
            this.cbCreateRFAccts.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbCreateRFAccts.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbCreateRFAccts.Location = new System.Drawing.Point(438, 233);
            this.cbCreateRFAccts.Name = "cbCreateRFAccts";
            this.cbCreateRFAccts.Size = new System.Drawing.Size(129, 19);
            this.cbCreateRFAccts.TabIndex = 283;
            this.cbCreateRFAccts.Text = "Create RF Accounts";
            this.cbCreateRFAccts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbCreateRFAccts.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(40, 136);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 16);
            this.label2.TabIndex = 282;
            this.label2.Text = "Warehouse Region";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtWarehouseRegion
            // 
            this.txtWarehouseRegion.Location = new System.Drawing.Point(152, 136);
            this.txtWarehouseRegion.Name = "txtWarehouseRegion";
            this.txtWarehouseRegion.Size = new System.Drawing.Size(72, 23);
            this.txtWarehouseRegion.TabIndex = 114;
            // 
            // chxDepositLocked
            // 
            this.chxDepositLocked.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chxDepositLocked.Location = new System.Drawing.Point(584, 254);
            this.chxDepositLocked.Name = "chxDepositLocked";
            this.chxDepositLocked.Size = new System.Drawing.Size(157, 24);
            this.chxDepositLocked.TabIndex = 280;
            this.chxDepositLocked.Text = "Deposit Screen Locked";
            this.chxDepositLocked.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCroffNo
            // 
            this.txtCroffNo.Location = new System.Drawing.Point(152, 280);
            this.txtCroffNo.Name = "txtCroffNo";
            this.txtCroffNo.Size = new System.Drawing.Size(112, 23);
            this.txtCroffNo.TabIndex = 190;
            this.txtCroffNo.Text = "0";
            this.txtCroffNo.TextChanged += new System.EventHandler(this.txtCroffNo_TextChanged);
            // 
            // txtTel
            // 
            this.txtTel.Location = new System.Drawing.Point(152, 256);
            this.txtTel.Name = "txtTel";
            this.txtTel.Size = new System.Drawing.Size(144, 23);
            this.txtTel.TabIndex = 180;
            // 
            // txtPostCode
            // 
            this.txtPostCode.Location = new System.Drawing.Point(152, 232);
            this.txtPostCode.MaxLength = 10;
            this.txtPostCode.Name = "txtPostCode";
            this.txtPostCode.Size = new System.Drawing.Size(144, 23);
            this.txtPostCode.TabIndex = 170;
            // 
            // txtAddress2
            // 
            this.txtAddress2.Location = new System.Drawing.Point(152, 184);
            this.txtAddress2.MaxLength = 26;
            this.txtAddress2.Name = "txtAddress2";
            this.txtAddress2.Size = new System.Drawing.Size(192, 23);
            this.txtAddress2.TabIndex = 116;
            // 
            // txtAddress1
            // 
            this.txtAddress1.Location = new System.Drawing.Point(152, 159);
            this.txtAddress1.MaxLength = 26;
            this.txtAddress1.Name = "txtAddress1";
            this.txtAddress1.Size = new System.Drawing.Size(192, 23);
            this.txtAddress1.TabIndex = 115;
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(56, 112);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(88, 16);
            this.label19.TabIndex = 205;
            this.label19.Text = "Scoring Region";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(80, 159);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(64, 16);
            this.label20.TabIndex = 202;
            this.label20.Text = "Address 1";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtCardChange
            // 
            this.dtCardChange.Location = new System.Drawing.Point(552, 40);
            this.dtCardChange.Name = "dtCardChange";
            this.dtCardChange.Size = new System.Drawing.Size(120, 23);
            this.dtCardChange.TabIndex = 200;
            this.dtCardChange.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // txtBranchName
            // 
            this.txtBranchName.Location = new System.Drawing.Point(152, 40);
            this.txtBranchName.MaxLength = 30;
            this.txtBranchName.Name = "txtBranchName";
            this.txtBranchName.Size = new System.Drawing.Size(192, 23);
            this.txtBranchName.TabIndex = 110;
            // 
            // label22
            // 
            this.label22.Location = new System.Drawing.Point(88, 280);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(56, 16);
            this.label22.TabIndex = 208;
            this.label22.Text = "Croff No";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label24
            // 
            this.label24.Location = new System.Drawing.Point(80, 256);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(64, 16);
            this.label24.TabIndex = 207;
            this.label24.Text = "Telephone";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label25
            // 
            this.label25.Location = new System.Drawing.Point(64, 40);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(80, 16);
            this.label25.TabIndex = 200;
            this.label25.Text = "Branch Name";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAddress3
            // 
            this.txtAddress3.Location = new System.Drawing.Point(152, 208);
            this.txtAddress3.MaxLength = 26;
            this.txtAddress3.Name = "txtAddress3";
            this.txtAddress3.Size = new System.Drawing.Size(192, 23);
            this.txtAddress3.TabIndex = 160;
            // 
            // txtRegion
            // 
            this.txtRegion.Location = new System.Drawing.Point(152, 112);
            this.txtRegion.Name = "txtRegion";
            this.txtRegion.Size = new System.Drawing.Size(72, 23);
            this.txtRegion.TabIndex = 113;
            // 
            // txtAS400BranchNo
            // 
            this.txtAS400BranchNo.Location = new System.Drawing.Point(152, 64);
            this.txtAS400BranchNo.MaxLength = 15;
            this.txtAS400BranchNo.Name = "txtAS400BranchNo";
            this.txtAS400BranchNo.Size = new System.Drawing.Size(72, 23);
            this.txtAS400BranchNo.TabIndex = 111;
            this.txtAS400BranchNo.Text = "0";
            this.txtAS400BranchNo.TextChanged += new System.EventHandler(this.txtAS400BranchNo_TextChanged);
            // 
            // label26
            // 
            this.label26.Location = new System.Drawing.Point(88, 232);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(56, 16);
            this.label26.TabIndex = 206;
            this.label26.Text = "Postcode";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label27
            // 
            this.label27.Location = new System.Drawing.Point(408, 88);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(136, 16);
            this.label27.TabIndex = 214;
            this.label27.Text = "New Payment Card Type";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtOldCard
            // 
            this.txtOldCard.Location = new System.Drawing.Point(552, 64);
            this.txtOldCard.Name = "txtOldCard";
            this.txtOldCard.Size = new System.Drawing.Size(120, 23);
            this.txtOldCard.TabIndex = 210;
            // 
            // label28
            // 
            this.label28.Location = new System.Drawing.Point(456, 184);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(88, 16);
            this.label28.TabIndex = 213;
            this.label28.Text = "Warehouse No";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label29
            // 
            this.label29.Location = new System.Drawing.Point(432, 208);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(112, 16);
            this.label29.TabIndex = 212;
            this.label29.Text = "Highest Trans Ref No";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label30
            // 
            this.label30.Location = new System.Drawing.Point(472, 160);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(72, 16);
            this.label30.TabIndex = 210;
            this.label30.Text = "Country Code";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCountryCode
            // 
            this.txtCountryCode.Location = new System.Drawing.Point(552, 160);
            this.txtCountryCode.Name = "txtCountryCode";
            this.txtCountryCode.Size = new System.Drawing.Size(48, 23);
            this.txtCountryCode.TabIndex = 250;
            // 
            // label31
            // 
            this.label31.Location = new System.Drawing.Point(496, 112);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(48, 16);
            this.label31.TabIndex = 211;
            this.label31.Text = "Hissn";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label32
            // 
            this.label32.Location = new System.Drawing.Point(384, 40);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(160, 16);
            this.label32.TabIndex = 199;
            this.label32.Text = "Date Payment Card Changed";
            this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label33
            // 
            this.label33.Location = new System.Drawing.Point(408, 64);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(136, 16);
            this.label33.TabIndex = 198;
            this.label33.Text = "Old Payment Card Type";
            this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label34
            // 
            this.label34.Location = new System.Drawing.Point(448, 136);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(96, 16);
            this.label34.TabIndex = 209;
            this.label34.Text = "Highest Buff No";
            this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtHiTransRefNo
            // 
            this.txtHiTransRefNo.Location = new System.Drawing.Point(552, 208);
            this.txtHiTransRefNo.Name = "txtHiTransRefNo";
            this.txtHiTransRefNo.Size = new System.Drawing.Size(120, 23);
            this.txtHiTransRefNo.TabIndex = 270;
            this.txtHiTransRefNo.Text = "0";
            this.txtHiTransRefNo.TextChanged += new System.EventHandler(this.txtHiTransRefNo_TextChanged);
            // 
            // txtWarehouseNo
            // 
            this.txtWarehouseNo.Location = new System.Drawing.Point(552, 184);
            this.txtWarehouseNo.Name = "txtWarehouseNo";
            this.txtWarehouseNo.Size = new System.Drawing.Size(48, 23);
            this.txtWarehouseNo.TabIndex = 260;
            // 
            // txtHissn
            // 
            this.txtHissn.Location = new System.Drawing.Point(552, 112);
            this.txtHissn.Name = "txtHissn";
            this.txtHissn.Size = new System.Drawing.Size(120, 23);
            this.txtHissn.TabIndex = 230;
            this.txtHissn.Text = "0";
            this.txtHissn.TextChanged += new System.EventHandler(this.txtHissn_TextChanged);
            // 
            // txtNewCard
            // 
            this.txtNewCard.Location = new System.Drawing.Point(552, 88);
            this.txtNewCard.Name = "txtNewCard";
            this.txtNewCard.Size = new System.Drawing.Size(120, 23);
            this.txtNewCard.TabIndex = 220;
            // 
            // txtHiBuffNo
            // 
            this.txtHiBuffNo.Location = new System.Drawing.Point(552, 136);
            this.txtHiBuffNo.Name = "txtHiBuffNo";
            this.txtHiBuffNo.Size = new System.Drawing.Size(120, 23);
            this.txtHiBuffNo.TabIndex = 240;
            this.txtHiBuffNo.Text = "0";
            this.txtHiBuffNo.TextChanged += new System.EventHandler(this.txtHiBuffNo_TextChanged);
            // 
            // label35
            // 
            this.label35.Location = new System.Drawing.Point(40, 64);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(104, 16);
            this.label35.TabIndex = 201;
            this.label35.Text = "AS400 Branch No";
            this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label36
            // 
            this.label36.Location = new System.Drawing.Point(80, 184);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(64, 16);
            this.label36.TabIndex = 203;
            this.label36.Text = "Address 2";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label37
            // 
            this.label37.Location = new System.Drawing.Point(72, 208);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(72, 16);
            this.label37.TabIndex = 204;
            this.label37.Text = "Address 3";
            this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbDeposits
            // 
            this.tbDeposits.Controls.Add(this.drpDeposit);
            this.tbDeposits.Controls.Add(this.dgDepositList);
            this.tbDeposits.Location = new System.Drawing.Point(0, 25);
            this.tbDeposits.Name = "tbDeposits";
            this.tbDeposits.Selected = false;
            this.tbDeposits.Size = new System.Drawing.Size(776, 359);
            this.tbDeposits.TabIndex = 0;
            this.tbDeposits.Title = "Bank Deposits";
            // 
            // drpDeposit
            // 
            this.drpDeposit.Location = new System.Drawing.Point(24, 192);
            this.drpDeposit.Name = "drpDeposit";
            this.drpDeposit.Size = new System.Drawing.Size(104, 23);
            this.drpDeposit.TabIndex = 0;
            this.drpDeposit.TabStop = false;
            this.drpDeposit.Visible = false;
            this.drpDeposit.SelectedIndexChanged += new System.EventHandler(this.drpDeposit_SelectedIndexChanged);
            // 
            // dgDepositList
            // 
            this.dgDepositList.DataMember = "";
            this.dgDepositList.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgDepositList.Location = new System.Drawing.Point(160, 24);
            this.dgDepositList.Name = "dgDepositList";
            this.dgDepositList.Size = new System.Drawing.Size(448, 320);
            this.dgDepositList.TabIndex = 0;
            this.dgDepositList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgDepositList_MouseUp);
            // 
            // tbGenerate
            // 
            this.tbGenerate.Controls.Add(this.gbContractNo);
            this.tbGenerate.Controls.Add(this.gbAccountNo);
            this.tbGenerate.Location = new System.Drawing.Point(0, 25);
            this.tbGenerate.Name = "tbGenerate";
            this.tbGenerate.Selected = false;
            this.tbGenerate.Size = new System.Drawing.Size(776, 359);
            this.tbGenerate.TabIndex = 0;
            this.tbGenerate.Title = "Number Generation";
            this.tbGenerate.Visible = false;
            // 
            // gbContractNo
            // 
            this.gbContractNo.Controls.Add(this.btnPrintContractNo);
            this.gbContractNo.Controls.Add(this.numContractNo);
            this.gbContractNo.Location = new System.Drawing.Point(212, 191);
            this.gbContractNo.Name = "gbContractNo";
            this.gbContractNo.Size = new System.Drawing.Size(360, 104);
            this.gbContractNo.TabIndex = 400;
            this.gbContractNo.TabStop = false;
            this.gbContractNo.Text = "Contract Numbers";
            // 
            // btnPrintContractNo
            // 
            this.btnPrintContractNo.Image = ((System.Drawing.Image)(resources.GetObject("btnPrintContractNo.Image")));
            this.btnPrintContractNo.Location = new System.Drawing.Point(280, 40);
            this.btnPrintContractNo.Name = "btnPrintContractNo";
            this.btnPrintContractNo.Size = new System.Drawing.Size(36, 30);
            this.btnPrintContractNo.TabIndex = 420;
            this.btnPrintContractNo.Click += new System.EventHandler(this.btnPrintContractNos_Click);
            // 
            // numContractNo
            // 
            this.numContractNo.Location = new System.Drawing.Point(128, 48);
            this.numContractNo.Name = "numContractNo";
            this.numContractNo.Size = new System.Drawing.Size(96, 23);
            this.numContractNo.TabIndex = 410;
            // 
            // gbAccountNo
            // 
            this.gbAccountNo.Controls.Add(this.label38);
            this.gbAccountNo.Controls.Add(this.drpAccountType);
            this.gbAccountNo.Controls.Add(this.btnPrintAccountNo);
            this.gbAccountNo.Controls.Add(this.numAccountNo);
            this.gbAccountNo.Location = new System.Drawing.Point(212, 47);
            this.gbAccountNo.Name = "gbAccountNo";
            this.gbAccountNo.Size = new System.Drawing.Size(360, 104);
            this.gbAccountNo.TabIndex = 300;
            this.gbAccountNo.TabStop = false;
            this.gbAccountNo.Text = "Account Numbers";
            // 
            // label38
            // 
            this.label38.Location = new System.Drawing.Point(32, 48);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(88, 16);
            this.label38.TabIndex = 0;
            this.label38.Text = "Account Type";
            this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpAccountType
            // 
            this.drpAccountType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpAccountType.ItemHeight = 15;
            this.drpAccountType.Location = new System.Drawing.Point(128, 48);
            this.drpAccountType.Name = "drpAccountType";
            this.drpAccountType.Size = new System.Drawing.Size(40, 23);
            this.drpAccountType.TabIndex = 310;
            // 
            // btnPrintAccountNo
            // 
            this.btnPrintAccountNo.Image = ((System.Drawing.Image)(resources.GetObject("btnPrintAccountNo.Image")));
            this.btnPrintAccountNo.Location = new System.Drawing.Point(280, 40);
            this.btnPrintAccountNo.Name = "btnPrintAccountNo";
            this.btnPrintAccountNo.Size = new System.Drawing.Size(32, 32);
            this.btnPrintAccountNo.TabIndex = 330;
            this.btnPrintAccountNo.Click += new System.EventHandler(this.btnPrintAccountNos_Click);
            // 
            // numAccountNo
            // 
            this.numAccountNo.Location = new System.Drawing.Point(192, 48);
            this.numAccountNo.Name = "numAccountNo";
            this.numAccountNo.Size = new System.Drawing.Size(64, 23);
            this.numAccountNo.TabIndex = 320;
            // 
            // tbStoreCardQualRules
            // 
            this.tbStoreCardQualRules.Controls.Add(this.lblInfo);
            this.tbStoreCardQualRules.Controls.Add(this.grpApplyTo);
            this.tbStoreCardQualRules.Controls.Add(this.grpStoreCardRules);
            this.tbStoreCardQualRules.Location = new System.Drawing.Point(0, 25);
            this.tbStoreCardQualRules.Name = "tbStoreCardQualRules";
            this.tbStoreCardQualRules.Selected = false;
            this.tbStoreCardQualRules.Size = new System.Drawing.Size(776, 359);
            this.tbStoreCardQualRules.TabIndex = 3;
            this.tbStoreCardQualRules.Title = "Store Card Qualification Rules";
            this.tbStoreCardQualRules.Enter += new System.EventHandler(this.tbStoreCardQualRules_Enter);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
            this.lblInfo.Location = new System.Drawing.Point(10, 340);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(651, 15);
            this.lblInfo.TabIndex = 305;
            this.lblInfo.Text = "Please note disabling a criteria will result in the criteria being ignored when p" +
    "re-qualifying a customer for Store Card";
            // 
            // grpApplyTo
            // 
            this.grpApplyTo.Controls.Add(this.rbNonCourtsBranches);
            this.grpApplyTo.Controls.Add(this.rbCourtsBranches);
            this.grpApplyTo.Controls.Add(this.rbAllBranches);
            this.grpApplyTo.Controls.Add(this.rbCurrentBranch);
            this.grpApplyTo.Controls.Add(this.lblStoreCardApplyTo);
            this.grpApplyTo.Location = new System.Drawing.Point(577, 3);
            this.grpApplyTo.Name = "grpApplyTo";
            this.grpApplyTo.Size = new System.Drawing.Size(187, 217);
            this.grpApplyTo.TabIndex = 304;
            this.grpApplyTo.TabStop = false;
            // 
            // rbNonCourtsBranches
            // 
            this.rbNonCourtsBranches.AutoSize = true;
            this.rbNonCourtsBranches.Location = new System.Drawing.Point(20, 138);
            this.rbNonCourtsBranches.Name = "rbNonCourtsBranches";
            this.rbNonCourtsBranches.Size = new System.Drawing.Size(137, 19);
            this.rbNonCourtsBranches.TabIndex = 314;
            this.rbNonCourtsBranches.Text = "Non Courts Branches";
            this.rbNonCourtsBranches.UseVisualStyleBackColor = true;
            this.rbNonCourtsBranches.CheckedChanged += new System.EventHandler(this.rbNonCourtsBranches_CheckedChanged);
            this.rbNonCourtsBranches.MouseHover += new System.EventHandler(this.rbNonCourtsBranches_MouseHover);
            // 
            // rbCourtsBranches
            // 
            this.rbCourtsBranches.AutoSize = true;
            this.rbCourtsBranches.Location = new System.Drawing.Point(20, 110);
            this.rbCourtsBranches.Name = "rbCourtsBranches";
            this.rbCourtsBranches.Size = new System.Drawing.Size(111, 19);
            this.rbCourtsBranches.TabIndex = 313;
            this.rbCourtsBranches.TabStop = true;
            this.rbCourtsBranches.Text = "Courts Branches";
            this.rbCourtsBranches.UseVisualStyleBackColor = true;
            this.rbCourtsBranches.CheckedChanged += new System.EventHandler(this.rbCourtsBranches_CheckedChanged);
            this.rbCourtsBranches.MouseHover += new System.EventHandler(this.rbCourtsBranches_MouseHover);
            // 
            // rbAllBranches
            // 
            this.rbAllBranches.AutoSize = true;
            this.rbAllBranches.Location = new System.Drawing.Point(20, 83);
            this.rbAllBranches.Name = "rbAllBranches";
            this.rbAllBranches.Size = new System.Drawing.Size(90, 19);
            this.rbAllBranches.TabIndex = 312;
            this.rbAllBranches.TabStop = true;
            this.rbAllBranches.Text = "All Branches";
            this.rbAllBranches.UseVisualStyleBackColor = true;
            this.rbAllBranches.CheckedChanged += new System.EventHandler(this.rbAllBranches_CheckedChanged);
            this.rbAllBranches.MouseHover += new System.EventHandler(this.rbAllBranches_MouseHover);
            // 
            // rbCurrentBranch
            // 
            this.rbCurrentBranch.AutoSize = true;
            this.rbCurrentBranch.Checked = true;
            this.rbCurrentBranch.Location = new System.Drawing.Point(20, 56);
            this.rbCurrentBranch.Name = "rbCurrentBranch";
            this.rbCurrentBranch.Size = new System.Drawing.Size(105, 19);
            this.rbCurrentBranch.TabIndex = 311;
            this.rbCurrentBranch.TabStop = true;
            this.rbCurrentBranch.Text = "Current Branch";
            this.rbCurrentBranch.UseVisualStyleBackColor = true;
            this.rbCurrentBranch.CheckedChanged += new System.EventHandler(this.rbCurrentBranch_CheckedChanged);
            this.rbCurrentBranch.MouseHover += new System.EventHandler(this.rbCurrentBranch_MouseHover);
            // 
            // lblStoreCardApplyTo
            // 
            this.lblStoreCardApplyTo.AutoSize = true;
            this.lblStoreCardApplyTo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
            this.lblStoreCardApplyTo.Location = new System.Drawing.Point(55, 19);
            this.lblStoreCardApplyTo.Name = "lblStoreCardApplyTo";
            this.lblStoreCardApplyTo.Size = new System.Drawing.Size(54, 15);
            this.lblStoreCardApplyTo.TabIndex = 310;
            this.lblStoreCardApplyTo.Text = "Apply To";
            this.lblStoreCardApplyTo.MouseHover += new System.EventHandler(this.lblStoreCardApplyTo_MouseHover);
            // 
            // grpStoreCardRules
            // 
            this.grpStoreCardRules.Controls.Add(this.chkMaxNoCust);
            this.grpStoreCardRules.Controls.Add(this.numMaxCust);
            this.grpStoreCardRules.Controls.Add(this.lblMaxCustForPreApp);
            this.grpStoreCardRules.Controls.Add(this.lblMonths2);
            this.grpStoreCardRules.Controls.Add(this.numMaxPrevMnthsInArrsY);
            this.grpStoreCardRules.Controls.Add(this.lblIn2);
            this.grpStoreCardRules.Controls.Add(this.lblMonths1);
            this.grpStoreCardRules.Controls.Add(this.numMinMnthsAcctHistY);
            this.grpStoreCardRules.Controls.Add(this.lblIn1);
            this.grpStoreCardRules.Controls.Add(this.chkMinAvailRFLimit);
            this.grpStoreCardRules.Controls.Add(this.chkMaxPrevMthsInArrs);
            this.grpStoreCardRules.Controls.Add(this.chkMaxCurrMthsInArrs);
            this.grpStoreCardRules.Controls.Add(this.chkMinMthsAcctHist);
            this.grpStoreCardRules.Controls.Add(this.chkMinBehaviouralScore);
            this.grpStoreCardRules.Controls.Add(this.chkMinApplicationScore);
            this.grpStoreCardRules.Controls.Add(this.lblStoreCardCriteria);
            this.grpStoreCardRules.Controls.Add(this.lblEnableDisableStoreCardRules);
            this.grpStoreCardRules.Controls.Add(this.numPcentInitRFLimit);
            this.grpStoreCardRules.Controls.Add(this.numMaxPrevMnthsInArrsX);
            this.grpStoreCardRules.Controls.Add(this.numMaxCurrMnthsInArrs);
            this.grpStoreCardRules.Controls.Add(this.numMinMnthsAcctHistX);
            this.grpStoreCardRules.Controls.Add(this.numMinBehaviouralScore);
            this.grpStoreCardRules.Controls.Add(this.numMinAppScore);
            this.grpStoreCardRules.Controls.Add(this.lblMinAppScore);
            this.grpStoreCardRules.Controls.Add(this.lblPcentInitialLimit);
            this.grpStoreCardRules.Controls.Add(this.lblMinBehaviouralScore);
            this.grpStoreCardRules.Controls.Add(this.lblPrevMaxMnthsInArrs);
            this.grpStoreCardRules.Controls.Add(this.lblMinMnthsAcctHistX);
            this.grpStoreCardRules.Controls.Add(this.lblMaxCurrMnthsInArrs);
            this.grpStoreCardRules.Location = new System.Drawing.Point(4, 3);
            this.grpStoreCardRules.Name = "grpStoreCardRules";
            this.grpStoreCardRules.Size = new System.Drawing.Size(554, 326);
            this.grpStoreCardRules.TabIndex = 303;
            this.grpStoreCardRules.TabStop = false;
            // 
            // chkMaxNoCust
            // 
            this.chkMaxNoCust.AutoSize = true;
            this.chkMaxNoCust.Location = new System.Drawing.Point(491, 304);
            this.chkMaxNoCust.Name = "chkMaxNoCust";
            this.chkMaxNoCust.Size = new System.Drawing.Size(15, 14);
            this.chkMaxNoCust.TabIndex = 319;
            this.chkMaxNoCust.UseVisualStyleBackColor = true;
            this.chkMaxNoCust.CheckedChanged += new System.EventHandler(this.chkMaxNoCust_CheckedChanged);
            // 
            // numMaxCust
            // 
            this.numMaxCust.Location = new System.Drawing.Point(200, 295);
            this.numMaxCust.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numMaxCust.Name = "numMaxCust";
            this.numMaxCust.Size = new System.Drawing.Size(75, 23);
            this.numMaxCust.TabIndex = 318;
            // 
            // lblMaxCustForPreApp
            // 
            this.lblMaxCustForPreApp.AutoSize = true;
            this.lblMaxCustForPreApp.Location = new System.Drawing.Point(6, 297);
            this.lblMaxCustForPreApp.Name = "lblMaxCustForPreApp";
            this.lblMaxCustForPreApp.Size = new System.Drawing.Size(183, 15);
            this.lblMaxCustForPreApp.TabIndex = 317;
            this.lblMaxCustForPreApp.Text = "Max. Customers for Pre-Approval";
            this.lblMaxCustForPreApp.MouseHover += new System.EventHandler(this.lblMaxCustForPreApp_MouseHover);
            // 
            // lblMonths2
            // 
            this.lblMonths2.AutoSize = true;
            this.lblMonths2.Location = new System.Drawing.Point(419, 221);
            this.lblMonths2.Name = "lblMonths2";
            this.lblMonths2.Size = new System.Drawing.Size(48, 15);
            this.lblMonths2.TabIndex = 316;
            this.lblMonths2.Text = "months";
            // 
            // numMaxPrevMnthsInArrsY
            // 
            this.numMaxPrevMnthsInArrsY.Location = new System.Drawing.Point(324, 221);
            this.numMaxPrevMnthsInArrsY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numMaxPrevMnthsInArrsY.Name = "numMaxPrevMnthsInArrsY";
            this.numMaxPrevMnthsInArrsY.Size = new System.Drawing.Size(75, 23);
            this.numMaxPrevMnthsInArrsY.TabIndex = 315;
            // 
            // lblIn2
            // 
            this.lblIn2.AutoSize = true;
            this.lblIn2.Location = new System.Drawing.Point(296, 221);
            this.lblIn2.Name = "lblIn2";
            this.lblIn2.Size = new System.Drawing.Size(17, 15);
            this.lblIn2.TabIndex = 314;
            this.lblIn2.Text = "in";
            // 
            // lblMonths1
            // 
            this.lblMonths1.AutoSize = true;
            this.lblMonths1.Location = new System.Drawing.Point(419, 142);
            this.lblMonths1.Name = "lblMonths1";
            this.lblMonths1.Size = new System.Drawing.Size(48, 15);
            this.lblMonths1.TabIndex = 313;
            this.lblMonths1.Text = "months";
            // 
            // numMinMnthsAcctHistY
            // 
            this.numMinMnthsAcctHistY.Location = new System.Drawing.Point(324, 143);
            this.numMinMnthsAcctHistY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numMinMnthsAcctHistY.Name = "numMinMnthsAcctHistY";
            this.numMinMnthsAcctHistY.Size = new System.Drawing.Size(75, 23);
            this.numMinMnthsAcctHistY.TabIndex = 312;
            // 
            // lblIn1
            // 
            this.lblIn1.AutoSize = true;
            this.lblIn1.Location = new System.Drawing.Point(294, 143);
            this.lblIn1.Name = "lblIn1";
            this.lblIn1.Size = new System.Drawing.Size(17, 15);
            this.lblIn1.TabIndex = 311;
            this.lblIn1.Text = "in";
            // 
            // chkMinAvailRFLimit
            // 
            this.chkMinAvailRFLimit.AutoSize = true;
            this.chkMinAvailRFLimit.Location = new System.Drawing.Point(491, 270);
            this.chkMinAvailRFLimit.Name = "chkMinAvailRFLimit";
            this.chkMinAvailRFLimit.Size = new System.Drawing.Size(15, 14);
            this.chkMinAvailRFLimit.TabIndex = 308;
            this.chkMinAvailRFLimit.UseVisualStyleBackColor = true;
            this.chkMinAvailRFLimit.CheckedChanged += new System.EventHandler(this.chkMinAvailRFLimit_CheckedChanged);
            // 
            // chkMaxPrevMthsInArrs
            // 
            this.chkMaxPrevMthsInArrs.AutoSize = true;
            this.chkMaxPrevMthsInArrs.Location = new System.Drawing.Point(491, 228);
            this.chkMaxPrevMthsInArrs.Name = "chkMaxPrevMthsInArrs";
            this.chkMaxPrevMthsInArrs.Size = new System.Drawing.Size(15, 14);
            this.chkMaxPrevMthsInArrs.TabIndex = 307;
            this.chkMaxPrevMthsInArrs.UseVisualStyleBackColor = true;
            this.chkMaxPrevMthsInArrs.CheckedChanged += new System.EventHandler(this.chkMaxPrevMthsInArrs_CheckedChanged);
            // 
            // chkMaxCurrMthsInArrs
            // 
            this.chkMaxCurrMthsInArrs.AutoSize = true;
            this.chkMaxCurrMthsInArrs.Location = new System.Drawing.Point(491, 189);
            this.chkMaxCurrMthsInArrs.Name = "chkMaxCurrMthsInArrs";
            this.chkMaxCurrMthsInArrs.Size = new System.Drawing.Size(15, 14);
            this.chkMaxCurrMthsInArrs.TabIndex = 306;
            this.chkMaxCurrMthsInArrs.UseVisualStyleBackColor = true;
            this.chkMaxCurrMthsInArrs.CheckedChanged += new System.EventHandler(this.chkMaxCurrMthsInArrs_CheckedChanged);
            // 
            // chkMinMthsAcctHist
            // 
            this.chkMinMthsAcctHist.AutoSize = true;
            this.chkMinMthsAcctHist.Location = new System.Drawing.Point(491, 150);
            this.chkMinMthsAcctHist.Name = "chkMinMthsAcctHist";
            this.chkMinMthsAcctHist.Size = new System.Drawing.Size(15, 14);
            this.chkMinMthsAcctHist.TabIndex = 305;
            this.chkMinMthsAcctHist.UseVisualStyleBackColor = true;
            this.chkMinMthsAcctHist.CheckedChanged += new System.EventHandler(this.chkMinMthsAcctHist_CheckedChanged);
            // 
            // chkMinBehaviouralScore
            // 
            this.chkMinBehaviouralScore.AutoSize = true;
            this.chkMinBehaviouralScore.Location = new System.Drawing.Point(491, 109);
            this.chkMinBehaviouralScore.Name = "chkMinBehaviouralScore";
            this.chkMinBehaviouralScore.Size = new System.Drawing.Size(15, 14);
            this.chkMinBehaviouralScore.TabIndex = 304;
            this.chkMinBehaviouralScore.UseVisualStyleBackColor = true;
            this.chkMinBehaviouralScore.CheckedChanged += new System.EventHandler(this.chkMinBehaviouralScore_CheckedChanged);
            // 
            // chkMinApplicationScore
            // 
            this.chkMinApplicationScore.AutoSize = true;
            this.chkMinApplicationScore.Location = new System.Drawing.Point(491, 61);
            this.chkMinApplicationScore.Name = "chkMinApplicationScore";
            this.chkMinApplicationScore.Size = new System.Drawing.Size(15, 14);
            this.chkMinApplicationScore.TabIndex = 303;
            this.chkMinApplicationScore.UseVisualStyleBackColor = true;
            this.chkMinApplicationScore.CheckedChanged += new System.EventHandler(this.chkMinApplicationScore_CheckedChanged);
            // 
            // lblStoreCardCriteria
            // 
            this.lblStoreCardCriteria.AutoSize = true;
            this.lblStoreCardCriteria.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
            this.lblStoreCardCriteria.Location = new System.Drawing.Point(122, 19);
            this.lblStoreCardCriteria.Name = "lblStoreCardCriteria";
            this.lblStoreCardCriteria.Size = new System.Drawing.Size(48, 15);
            this.lblStoreCardCriteria.TabIndex = 310;
            this.lblStoreCardCriteria.Text = "Criteria";
            this.lblStoreCardCriteria.MouseHover += new System.EventHandler(this.lblStoreCardCriteria_MouseHover);
            // 
            // lblEnableDisableStoreCardRules
            // 
            this.lblEnableDisableStoreCardRules.AutoSize = true;
            this.lblEnableDisableStoreCardRules.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
            this.lblEnableDisableStoreCardRules.Location = new System.Drawing.Point(458, 19);
            this.lblEnableDisableStoreCardRules.Name = "lblEnableDisableStoreCardRules";
            this.lblEnableDisableStoreCardRules.Size = new System.Drawing.Size(88, 15);
            this.lblEnableDisableStoreCardRules.TabIndex = 309;
            this.lblEnableDisableStoreCardRules.Text = "Enable/Disable";
            this.lblEnableDisableStoreCardRules.MouseHover += new System.EventHandler(this.lblEnableDisableStoreCardRules_MouseHover);
            // 
            // numPcentInitRFLimit
            // 
            this.numPcentInitRFLimit.DecimalPlaces = 2;
            this.numPcentInitRFLimit.Location = new System.Drawing.Point(201, 261);
            this.numPcentInitRFLimit.Maximum = new decimal(new int[] {
            25000,
            0,
            0,
            0});
            this.numPcentInitRFLimit.Name = "numPcentInitRFLimit";
            this.numPcentInitRFLimit.Size = new System.Drawing.Size(75, 23);
            this.numPcentInitRFLimit.TabIndex = 302;
            // 
            // numMaxPrevMnthsInArrsX
            // 
            this.numMaxPrevMnthsInArrsX.DecimalPlaces = 2;
            this.numMaxPrevMnthsInArrsX.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numMaxPrevMnthsInArrsX.Location = new System.Drawing.Point(201, 219);
            this.numMaxPrevMnthsInArrsX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numMaxPrevMnthsInArrsX.Name = "numMaxPrevMnthsInArrsX";
            this.numMaxPrevMnthsInArrsX.Size = new System.Drawing.Size(75, 23);
            this.numMaxPrevMnthsInArrsX.TabIndex = 300;
            // 
            // numMaxCurrMnthsInArrs
            // 
            this.numMaxCurrMnthsInArrs.DecimalPlaces = 2;
            this.numMaxCurrMnthsInArrs.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numMaxCurrMnthsInArrs.Location = new System.Drawing.Point(201, 180);
            this.numMaxCurrMnthsInArrs.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numMaxCurrMnthsInArrs.Name = "numMaxCurrMnthsInArrs";
            this.numMaxCurrMnthsInArrs.Size = new System.Drawing.Size(75, 23);
            this.numMaxCurrMnthsInArrs.TabIndex = 298;
            // 
            // numMinMnthsAcctHistX
            // 
            this.numMinMnthsAcctHistX.Location = new System.Drawing.Point(201, 141);
            this.numMinMnthsAcctHistX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numMinMnthsAcctHistX.Name = "numMinMnthsAcctHistX";
            this.numMinMnthsAcctHistX.Size = new System.Drawing.Size(75, 23);
            this.numMinMnthsAcctHistX.TabIndex = 296;
            // 
            // numMinBehaviouralScore
            // 
            this.numMinBehaviouralScore.Location = new System.Drawing.Point(201, 100);
            this.numMinBehaviouralScore.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numMinBehaviouralScore.Name = "numMinBehaviouralScore";
            this.numMinBehaviouralScore.Size = new System.Drawing.Size(75, 23);
            this.numMinBehaviouralScore.TabIndex = 294;
            // 
            // numMinAppScore
            // 
            this.numMinAppScore.Location = new System.Drawing.Point(201, 56);
            this.numMinAppScore.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numMinAppScore.Name = "numMinAppScore";
            this.numMinAppScore.Size = new System.Drawing.Size(75, 23);
            this.numMinAppScore.TabIndex = 292;
            // 
            // lblMinAppScore
            // 
            this.lblMinAppScore.AutoSize = true;
            this.lblMinAppScore.Location = new System.Drawing.Point(6, 58);
            this.lblMinAppScore.Name = "lblMinAppScore";
            this.lblMinAppScore.Size = new System.Drawing.Size(127, 15);
            this.lblMinAppScore.TabIndex = 0;
            this.lblMinAppScore.Text = "Min. Application Score";
            this.lblMinAppScore.MouseHover += new System.EventHandler(this.lblMinAppScore_MouseHover);
            // 
            // lblPcentInitialLimit
            // 
            this.lblPcentInitialLimit.AutoSize = true;
            this.lblPcentInitialLimit.Location = new System.Drawing.Point(6, 263);
            this.lblPcentInitialLimit.Name = "lblPcentInitialLimit";
            this.lblPcentInitialLimit.Size = new System.Drawing.Size(113, 15);
            this.lblPcentInitialLimit.TabIndex = 301;
            this.lblPcentInitialLimit.Text = "% for Initial RF Limit";
            this.lblPcentInitialLimit.MouseHover += new System.EventHandler(this.lblPcentInitialLimit_MouseHover);
            // 
            // lblMinBehaviouralScore
            // 
            this.lblMinBehaviouralScore.AutoSize = true;
            this.lblMinBehaviouralScore.Location = new System.Drawing.Point(6, 102);
            this.lblMinBehaviouralScore.Name = "lblMinBehaviouralScore";
            this.lblMinBehaviouralScore.Size = new System.Drawing.Size(128, 15);
            this.lblMinBehaviouralScore.TabIndex = 293;
            this.lblMinBehaviouralScore.Text = "Min. Behavioural Score";
            this.lblMinBehaviouralScore.MouseHover += new System.EventHandler(this.lblMinBehaviouralScore_MouseHover);
            // 
            // lblPrevMaxMnthsInArrs
            // 
            this.lblPrevMaxMnthsInArrs.AutoSize = true;
            this.lblPrevMaxMnthsInArrs.Location = new System.Drawing.Point(6, 221);
            this.lblPrevMaxMnthsInArrs.Name = "lblPrevMaxMnthsInArrs";
            this.lblPrevMaxMnthsInArrs.Size = new System.Drawing.Size(177, 15);
            this.lblPrevMaxMnthsInArrs.TabIndex = 299;
            this.lblPrevMaxMnthsInArrs.Text = "Max. Previous Months In Arrears";
            this.lblPrevMaxMnthsInArrs.MouseHover += new System.EventHandler(this.lblPrevMaxMnthsInArrs_MouseHover);
            // 
            // lblMinMnthsAcctHistX
            // 
            this.lblMinMnthsAcctHistX.AutoSize = true;
            this.lblMinMnthsAcctHistX.Location = new System.Drawing.Point(6, 143);
            this.lblMinMnthsAcctHistX.Name = "lblMinMnthsAcctHistX";
            this.lblMinMnthsAcctHistX.Size = new System.Drawing.Size(164, 15);
            this.lblMinMnthsAcctHistX.TabIndex = 295;
            this.lblMinMnthsAcctHistX.Text = "Min. Months Account History";
            this.lblMinMnthsAcctHistX.MouseHover += new System.EventHandler(this.lblMinMnthsAcctHistX_MouseHover);
            // 
            // lblMaxCurrMnthsInArrs
            // 
            this.lblMaxCurrMnthsInArrs.AutoSize = true;
            this.lblMaxCurrMnthsInArrs.Location = new System.Drawing.Point(6, 182);
            this.lblMaxCurrMnthsInArrs.Name = "lblMaxCurrMnthsInArrs";
            this.lblMaxCurrMnthsInArrs.Size = new System.Drawing.Size(172, 15);
            this.lblMaxCurrMnthsInArrs.TabIndex = 297;
            this.lblMaxCurrMnthsInArrs.Text = "Max. Current Months In Arrears";
            this.lblMaxCurrMnthsInArrs.MouseHover += new System.EventHandler(this.lblMaxCurrMnthsInArrs_MouseHover);
            // 
            // gbBranch
            // 
            this.gbBranch.BackColor = System.Drawing.SystemColors.Control;
            this.gbBranch.Controls.Add(this.grpNonCourtsStoreTypes);
            this.gbBranch.Controls.Add(this.cbCashLoanBranch);
            this.gbBranch.Controls.Add(this.grp3PLWarehouse);
            this.gbBranch.Controls.Add(this.label3);
            this.gbBranch.Controls.Add(this.drpStoreType);
            this.gbBranch.Controls.Add(this.btnClear);
            this.gbBranch.Controls.Add(this.label23);
            this.gbBranch.Controls.Add(this.txtBranchNo);
            this.gbBranch.Controls.Add(this.label1);
            this.gbBranch.Controls.Add(this.btnSave);
            this.gbBranch.Controls.Add(this.btnExit);
            this.gbBranch.Controls.Add(this.drpBranchNo);
            this.gbBranch.Location = new System.Drawing.Point(8, 0);
            this.gbBranch.Name = "gbBranch";
            this.gbBranch.Size = new System.Drawing.Size(776, 88);
            this.gbBranch.TabIndex = 0;
            this.gbBranch.TabStop = false;
            // 
            // grpNonCourtsStoreTypes
            // 
            this.grpNonCourtsStoreTypes.Controls.Add(this.rbAshley);
            this.grpNonCourtsStoreTypes.Controls.Add(this.rbLuckyDollar);
            this.grpNonCourtsStoreTypes.Location = new System.Drawing.Point(489, 13);
            this.grpNonCourtsStoreTypes.Name = "grpNonCourtsStoreTypes";
            this.grpNonCourtsStoreTypes.Size = new System.Drawing.Size(141, 68);
            this.grpNonCourtsStoreTypes.TabIndex = 291;
            this.grpNonCourtsStoreTypes.TabStop = false;
            this.grpNonCourtsStoreTypes.Text = "Non Courts Store Type";
            // 
            // rbAshley
            // 
            this.rbAshley.AutoSize = true;
            this.rbAshley.Location = new System.Drawing.Point(7, 45);
            this.rbAshley.Name = "rbAshley";
            this.rbAshley.Size = new System.Drawing.Size(56, 17);
            this.rbAshley.TabIndex = 1;
            this.rbAshley.TabStop = true;
            this.rbAshley.Text = "Ashley";
            this.rbAshley.UseVisualStyleBackColor = true;
            // 
            // rbLuckyDollar
            // 
            this.rbLuckyDollar.AutoSize = true;
            this.rbLuckyDollar.Location = new System.Drawing.Point(7, 19);
            this.rbLuckyDollar.Name = "rbLuckyDollar";
            this.rbLuckyDollar.Size = new System.Drawing.Size(84, 17);
            this.rbLuckyDollar.TabIndex = 0;
            this.rbLuckyDollar.TabStop = true;
            this.rbLuckyDollar.Text = "Lucky Dollar";
            this.rbLuckyDollar.UseVisualStyleBackColor = true;
            // 
            // cbCashLoanBranch
            // 
            this.cbCashLoanBranch.AutoSize = true;
            this.cbCashLoanBranch.Location = new System.Drawing.Point(369, 69);
            this.cbCashLoanBranch.Name = "cbCashLoanBranch";
            this.cbCashLoanBranch.Size = new System.Drawing.Size(114, 17);
            this.cbCashLoanBranch.TabIndex = 528;
            this.cbCashLoanBranch.Text = "Cash Loan Branch";
            this.cbCashLoanBranch.UseVisualStyleBackColor = true;
            // 
            // grp3PLWarehouse
            // 
            this.grp3PLWarehouse.Controls.Add(this.drpDefaultPrintLocation);
            this.grp3PLWarehouse.Controls.Add(this.lStoreCardQualRules);
            this.grp3PLWarehouse.Controls.Add(this.chkWarehouse);
            this.grp3PLWarehouse.Controls.Add(this.lbl3PLDefaultPrintLocn);
            this.grp3PLWarehouse.Controls.Add(this.lDetails);
            this.grp3PLWarehouse.Controls.Add(this.lNumGeneration);
            this.grp3PLWarehouse.Controls.Add(this.lDeposits);
            this.grp3PLWarehouse.Location = new System.Drawing.Point(166, 12);
            this.grp3PLWarehouse.Name = "grp3PLWarehouse";
            this.grp3PLWarehouse.Size = new System.Drawing.Size(291, 47);
            this.grp3PLWarehouse.TabIndex = 526;
            this.grp3PLWarehouse.TabStop = false;
            this.grp3PLWarehouse.Text = "Third Party Deliveries Warehouse";
            // 
            // drpDefaultPrintLocation
            // 
            this.drpDefaultPrintLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDefaultPrintLocation.FormattingEnabled = true;
            this.drpDefaultPrintLocation.Location = new System.Drawing.Point(117, 16);
            this.drpDefaultPrintLocation.Name = "drpDefaultPrintLocation";
            this.drpDefaultPrintLocation.Size = new System.Drawing.Size(72, 21);
            this.drpDefaultPrintLocation.TabIndex = 0;
            // 
            // lStoreCardQualRules
            // 
            this.lStoreCardQualRules.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.lStoreCardQualRules.Enabled = false;
            this.lStoreCardQualRules.Location = new System.Drawing.Point(130, 19);
            this.lStoreCardQualRules.Name = "lStoreCardQualRules";
            this.lStoreCardQualRules.Size = new System.Drawing.Size(16, 16);
            this.lStoreCardQualRules.TabIndex = 527;
            this.lStoreCardQualRules.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lStoreCardQualRules.Visible = false;
            // 
            // chkWarehouse
            // 
            this.chkWarehouse.AutoSize = true;
            this.chkWarehouse.Location = new System.Drawing.Point(203, 18);
            this.chkWarehouse.Name = "chkWarehouse";
            this.chkWarehouse.Size = new System.Drawing.Size(81, 17);
            this.chkWarehouse.TabIndex = 2;
            this.chkWarehouse.Text = "Warehouse";
            this.chkWarehouse.UseVisualStyleBackColor = true;
            this.chkWarehouse.CheckedChanged += new System.EventHandler(this.chkWarehouse_CheckedChanged);
            // 
            // lbl3PLDefaultPrintLocn
            // 
            this.lbl3PLDefaultPrintLocn.AutoSize = true;
            this.lbl3PLDefaultPrintLocn.Location = new System.Drawing.Point(3, 19);
            this.lbl3PLDefaultPrintLocn.Name = "lbl3PLDefaultPrintLocn";
            this.lbl3PLDefaultPrintLocn.Size = new System.Drawing.Size(109, 13);
            this.lbl3PLDefaultPrintLocn.TabIndex = 1;
            this.lbl3PLDefaultPrintLocn.Text = "Default Print Location";
            // 
            // lDetails
            // 
            this.lDetails.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.lDetails.Enabled = false;
            this.lDetails.Location = new System.Drawing.Point(130, 19);
            this.lDetails.Name = "lDetails";
            this.lDetails.Size = new System.Drawing.Size(16, 16);
            this.lDetails.TabIndex = 522;
            this.lDetails.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lDetails.Visible = false;
            // 
            // lNumGeneration
            // 
            this.lNumGeneration.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.lNumGeneration.Enabled = false;
            this.lNumGeneration.Location = new System.Drawing.Point(130, 18);
            this.lNumGeneration.Name = "lNumGeneration";
            this.lNumGeneration.Size = new System.Drawing.Size(16, 16);
            this.lNumGeneration.TabIndex = 523;
            this.lNumGeneration.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lNumGeneration.Visible = false;
            // 
            // lDeposits
            // 
            this.lDeposits.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.lDeposits.Enabled = false;
            this.lDeposits.Location = new System.Drawing.Point(130, 19);
            this.lDeposits.Name = "lDeposits";
            this.lDeposits.Size = new System.Drawing.Size(16, 16);
            this.lDeposits.TabIndex = 521;
            this.lDeposits.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lDeposits.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(167, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 524;
            this.label3.Text = "Store Type";
            // 
            // drpStoreType
            // 
            this.drpStoreType.FormattingEnabled = true;
            this.drpStoreType.Items.AddRange(new object[] {
            "<Select Store Type>",
            "Courts Store",
            "Non Courts Store"});
            this.drpStoreType.Location = new System.Drawing.Point(234, 65);
            this.drpStoreType.Name = "drpStoreType";
            this.drpStoreType.Size = new System.Drawing.Size(121, 21);
            this.drpStoreType.TabIndex = 525;
            this.drpStoreType.SelectedIndexChanged += new System.EventHandler(this.drpStoreType_SelectedIndexChanged);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(709, 16);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(56, 24);
            this.btnClear.TabIndex = 510;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(5, 66);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(72, 16);
            this.label23.TabIndex = 171;
            this.label23.Text = "Branch No";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBranchNo
            // 
            this.txtBranchNo.Location = new System.Drawing.Point(80, 66);
            this.txtBranchNo.MaxLength = 3;
            this.txtBranchNo.Name = "txtBranchNo";
            this.txtBranchNo.Size = new System.Drawing.Size(72, 20);
            this.txtBranchNo.TabIndex = 20;
            this.txtBranchNo.TextChanged += new System.EventHandler(this.txtBranchNo_TextChanged);
            this.txtBranchNo.Leave += new System.EventHandler(this.txtBranchNo_Leave);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 17);
            this.label1.TabIndex = 172;
            this.label1.Text = "Branches";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(640, 16);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(56, 24);
            this.btnSave.TabIndex = 500;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(709, 58);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(56, 24);
            this.btnExit.TabIndex = 520;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // drpBranchNo
            // 
            this.drpBranchNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranchNo.ItemHeight = 13;
            this.drpBranchNo.Location = new System.Drawing.Point(80, 29);
            this.drpBranchNo.Name = "drpBranchNo";
            this.drpBranchNo.Size = new System.Drawing.Size(72, 21);
            this.drpBranchNo.TabIndex = 10;
            this.drpBranchNo.SelectedIndexChanged += new System.EventHandler(this.drpBranchNo_SelectedIndexChanged);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // errorProviderStoreCard
            // 
            this.errorProviderStoreCard.ContainerControl = this;
            // 
            // Branch
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.tcBranch);
            this.Controls.Add(this.gbBranch);
            this.Name = "Branch";
            this.Text = "Branch";
            this.Load += new System.EventHandler(this.Branch_Load);
            this.tcBranch.ResumeLayout(false);
            this.tbDetails.ResumeLayout(false);
            this.tbDetails.PerformLayout();
            this.tbDeposits.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgDepositList)).EndInit();
            this.tbGenerate.ResumeLayout(false);
            this.gbContractNo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numContractNo)).EndInit();
            this.gbAccountNo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numAccountNo)).EndInit();
            this.tbStoreCardQualRules.ResumeLayout(false);
            this.tbStoreCardQualRules.PerformLayout();
            this.grpApplyTo.ResumeLayout(false);
            this.grpApplyTo.PerformLayout();
            this.grpStoreCardRules.ResumeLayout(false);
            this.grpStoreCardRules.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxCust)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxPrevMnthsInArrsY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinMnthsAcctHistY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPcentInitRFLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxPrevMnthsInArrsX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxCurrMnthsInArrs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinMnthsAcctHistX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinBehaviouralScore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinAppScore)).EndInit();
            this.gbBranch.ResumeLayout(false);
            this.gbBranch.PerformLayout();
            this.grpNonCourtsStoreTypes.ResumeLayout(false);
            this.grpNonCourtsStoreTypes.PerformLayout();
            this.grp3PLWarehouse.ResumeLayout(false);
            this.grp3PLWarehouse.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderStoreCard)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion


        private void Clear()
        {
            // Disable change events and clear all fields
            this._userChanged = false;

            ClearControls(this.Controls);
            ((MainForm)this.FormRoot).statusBar1.Text = "";

            this.dtCardChange.Value = DateTime.Now;
            this.drpDeposit.Visible = false;
            this.dgDepositList.DataSource = null;

            // Disable Save until after the next successful load
            this.btnSave.Enabled = false;

            // Enable change events
            this._userChanged = true;
            // Enable Store Type
            this.drpStoreType.Enabled = true;           //CR903     jec
            cbCashLoanBranch.Enabled = true;
            errorProvider1.SetError(this.drpStoreType, "");
            //  CR903 
            ((MainForm)this.FormRoot).statusBar1.Text = "";

            //IP - 26/04/11 - Feature 3000 - Clear any errors for Store Card Qualification Criteria screen.
            ClearStoreCardFields();
        }

        private void LoadStaticData()
        {
            this._userChanged = false;
            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

            if (StaticData.Tables[TN.BranchNumber] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));
            if (StaticData.Tables[TN.Deposits] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Deposits, null));

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

            drpBranchNo.DataSource = StaticData.Tables[TN.BranchNumber];
            drpBranchNo.DisplayMember = CN.BranchNo;

            drpDeposit.DataSource = ((DataTable)StaticData.Tables[TN.Deposits]).DefaultView;
            drpDeposit.DisplayMember = CN.CodeDescription;
            drpDeposit.ValueMember = CN.Code;
            ((DataView)drpDeposit.DataSource).RowFilter = CN.IsDeposit + " = 1 and " + CN.Code + " not = 'SAF'";
            this._userChanged = true;
        }

        private void LoadData(string branchNo)
        {
            Clear();

            // Disable change events
            this._userChanged = false;

            // Set to this branch no
            this.txtBranchNo.Text = branchNo;

            // Load this branch
            DataSet branchSet = new DataSet();
            if (IsNumeric(branchNo))
                branchSet = AccountManager.BranchGet(Convert.ToInt32(branchNo), out Error);

            bool branchLoaded = false;

            string defaultPrintLocation = String.Empty; //IP - 23/02/10 - CR1072 - Malaysia 3PL for Version 5.2
            var setStoreCardCheckBoxes = false;  //IP - 10/12/10 - Store Card

            //IP - 13/05/11 - variables will be used to determine if a criteria should be enabled/disabled.
            var enableMinAppScore = false;
            var enableMinBehaviouralScore = false;
            var enableMinMnthsAcctHistX = false;
            var enableMaxCurrMnthsInArrs = false;
            var enableMaxPrevMnthsInArrsX = false;
            var enablePcentInitRFLimit = false;
            var enableMaxCust = false;


            foreach (DataTable dt in branchSet.Tables)
            {
                if (dt.TableName == TN.BranchDetails)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        branchLoaded = true;
                        this.txtBranchName.Text = (string)row[CN.BranchName];
                        this.txtAddress1.Text = (string)row[CN.BranchAddress1];
                        this.txtAddress2.Text = (string)row[CN.BranchAddress2];
                        this.txtAddress3.Text = (string)row[CN.BranchAddress3];
                        this.txtPostCode.Text = (string)row[CN.BranchPostCode];
                        this.txtTel.Text = (string)row[CN.TelNo];
                        this.txtCountryCode.Text = (string)row[CN.CountryCode];
                        this.txtCroffNo.Text = ((int)row[CN.CroffNo]).ToString();
                        this.txtOldCard.Text = (string)row[CN.OldPCType];
                        this.txtNewCard.Text = (string)row[CN.NewPCType];
                        this.dtCardChange.Value = Convert.ToDateTime(row[CN.DatePCChange]);
                        this.txtHissn.Text = ((int)row[CN.HissN]).ToString();
                        this.txtHiBuffNo.Text = ((int)row[CN.HiBuffNo]).ToString();
                        this.txtWarehouseNo.Text = (string)row[CN.WarehouseNo];
                        this.txtHiTransRefNo.Text = ((int)row[CN.HiRefNo]).ToString();
                        this.txtAS400BranchNo.Text = Convert.ToInt16(row[CN.AS400BranchNo]).ToString();
                        this.txtRegion.Text = (string)row[CN.Region];
                        this.chxDepositLocked.Checked = (bool)row[CN.DepositScreenLocked];
                        this.txtWarehouseRegion.Text = (string)row[CN.WarehouseRegion];
                        this.txtFact2000BranchLetter.Text = row[CN.FACT2000BranchLetter].ToString().Trim(); //TODO constant

                        this.cbCreateRFAccts.Checked = (bool)row[CN.CreateRFAccounts];      //CR903   jec 03/08/07
                        this.cbCreateStoreAccts.Checked = (bool)row[CN.CreateStoreAccounts];
                        this.cbCreateCashAccts.Checked = (bool)row[CN.CreateCashAccounts];      //CR903   jec 03/08/07
                        this.cbScoreHPbefore.Checked = (bool)row[CN.ScoreHPbefore];         //CR903   jec 03/08/07
                        this.cbCreateHPAccts.Checked = (bool)row[CN.CreateHPAccounts];      //CR903   jec 03/08/07
                        if ((string)row[CN.StoreType] == Convert.ToString(StoreType.Courts))    //CR903   jec 03/08/07
                        {
                            drpStoreType.SelectedIndex = 1;
                            grpNonCourtsStoreTypes.Visible = false;
                        }
                        else
                        {
                            drpStoreType.SelectedIndex = 2;
                            grpNonCourtsStoreTypes.Visible = true;

                            rbLuckyDollar.Checked = (bool)row["LuckyDollarStore"];
                            rbAshley.Checked = (bool)row["AshleyStore"];
                        }
                        drpStoreType.Enabled = false;       //do not allow change of store type
                        defaultPrintLocation = ((int)row[CN.DefaultPrintLocation]).ToString();

                        cbServiceRepairCentre.Checked = (row[CN.ServiceRepairCentre] != DBNull.Value && (bool)row[CN.ServiceRepairCentre]);

                        cbCashLoanBranch.Enabled = false;
                        cbCashLoanBranch.Checked = (bool)row["CashLoanBranch"];


                        if (Country[CountryParameterNames.BehaviouralScorecard].ToString() == "S" ||
                            Country[CountryParameterNames.BehaviouralScorecard].ToString() == "A" ||
                            Country[CountryParameterNames.BehaviouralScorecard].ToString() == "N")
                        {
                            Chk_behavioural.Enabled = false;
                            if (Country[CountryParameterNames.BehaviouralScorecard].ToString() == "S")
                            {
                                Chk_behavioural.Checked = true;
                            }
                            else
                            {
                                Chk_behavioural.Checked = false;
                            }
                        }
                        else
                        {   // UAT88 check for valid parameter value
                            if (Country[CountryParameterNames.BehaviouralScorecard].ToString() == "P" ||
                            Country[CountryParameterNames.BehaviouralScorecard].ToString() == "B")
                            {
                                Chk_behavioural.Enabled = true;
                                Chk_behavioural.Checked = Convert.ToBoolean(row[CN.BehaviouralScoring]);
                            }
                            else    //disable 
                            {
                                Chk_behavioural.Enabled = false;
                            }
                        } //CR1034 SC 16/02/10
                    }
                }
                else if (dt.TableName == TN.BranchDeposits)
                {
                    _depositListView = new DataView(dt);
                    _depositListView.AllowNew = false;
                    _depositListView.Sort = CN.PayMethod + " ASC ";
                    dgDepositList.DataSource = _depositListView;

                    if (dgDepositList.TableStyles.Count == 0)
                    {
                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        tabStyle.MappingName = _depositListView.Table.TableName;

                        dgDepositList.TableStyles.Clear();
                        dgDepositList.TableStyles.Add(tabStyle);
                        dgDepositList.DataSource = _depositListView;

                        // Hidden columns
                        tabStyle.GridColumnStyles[CN.BranchNo].Width = 0;
                        tabStyle.GridColumnStyles[CN.PayMethod].Width = 0;
                        tabStyle.GridColumnStyles[CN.Deposit].Width = 0;

                        // Displayed columns
                        tabStyle.GridColumnStyles[CN.CodeDescript].Width = 200;
                        tabStyle.GridColumnStyles[CN.CodeDescript].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.CodeDescript].HeaderText = GetResource("T_PAYMETHOD");

                        tabStyle.GridColumnStyles[CN.Description].Width = 200;
                        tabStyle.GridColumnStyles[CN.Description].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.Description].HeaderText = GetResource("T_DEPOSITTYPE");
                    }
                }
                else if (dt.TableName == TN.StoreCardQualRules) //IP - 9/12/10 - Store Card
                {
                    if (tcBranch.TabPages.Contains(tbStoreCardQualRules) == true)
                    {
                        if (dt.Rows.Count > 0)
                        {

                            setStoreCardCheckBoxes = true;

                            foreach (DataRow dr in dt.Rows)
                            {
                                //IP - 13/05/11 - depending on whether the values are empty(not been set) or not we want to enable/disable the fields.
                                enableMinAppScore = !(Convert.ToString(dr[CN.MinApplicationScore]) == string.Empty);
                                enableMinBehaviouralScore = !(Convert.ToString(dr[CN.MinBehaviouralScore]) == string.Empty);
                                enableMinMnthsAcctHistX = !(Convert.ToString(dr[CN.MinMthsAcctHistX]) == string.Empty);
                                enableMaxCurrMnthsInArrs = !(Convert.ToString(dr[CN.MaxCurrMthsInArrs]) == string.Empty);
                                enableMaxPrevMnthsInArrsX = !(Convert.ToString(dr[CN.MaxPrevMthsInArrsX]) == string.Empty);
                                enablePcentInitRFLimit = !(Convert.ToString(dr[CN.PcentInitRFLimit]) == string.Empty);
                                enableMaxCust = !(Convert.ToString(dr[CN.MaxNoCustForApproval]) == string.Empty);

                                numMinAppScore.Value = Convert.ToString(dr[CN.MinApplicationScore]) == string.Empty ? 0 : Convert.ToDecimal(dr[CN.MinApplicationScore]);
                                numMinBehaviouralScore.Value = Convert.ToString(dr[CN.MinBehaviouralScore]) == string.Empty ? 0 : Convert.ToDecimal(dr[CN.MinBehaviouralScore]);
                                numMinMnthsAcctHistX.Value = Convert.ToString(dr[CN.MinMthsAcctHistX]) == string.Empty ? 0 : Convert.ToDecimal(dr[CN.MinMthsAcctHistX]);          //IP - 26/04/11 - Feature 3000 - Renamed
                                numMinMnthsAcctHistY.Value = Convert.ToString(dr[CN.MinMthsAcctHistY]) == string.Empty ? 0 : Convert.ToDecimal(dr[CN.MinMthsAcctHistY]);          //IP - 26/04/11 - Feature 3000 
                                numMaxCurrMnthsInArrs.Value = Convert.ToString(dr[CN.MaxCurrMthsInArrs]) == string.Empty ? 0 : Convert.ToDecimal(dr[CN.MaxCurrMthsInArrs]);
                                numMaxPrevMnthsInArrsX.Value = Convert.ToString(dr[CN.MaxPrevMthsInArrsX]) == string.Empty ? 0 : Convert.ToDecimal(dr[CN.MaxPrevMthsInArrsX]);    //IP - 26/04/11 - Feature 3000 - Renamed
                                numMaxPrevMnthsInArrsY.Value = Convert.ToString(dr[CN.MaxPrevMthsInArrsY]) == string.Empty ? 0 : Convert.ToDecimal(dr[CN.MaxPrevMthsInArrsY]);    //IP - 26/04/11 - Feature 3000 
                                numPcentInitRFLimit.Value = Convert.ToString(dr[CN.PcentInitRFLimit]) == string.Empty ? 0 : Convert.ToDecimal(dr[CN.PcentInitRFLimit]);   //IP - 26/04/11 - Feature 3000 - Renamed
                                numMaxCust.Value = Convert.ToString(dr[CN.MaxNoCustForApproval]) == string.Empty ? 0 : Convert.ToDecimal(dr[CN.MaxNoCustForApproval]);    //IP - 10/05/11 - Feature 3593
                            }

                            //IP - 13/05/11 - Pass in booleans to determine if each criteria should be enabled/disabled.
                            SetStoreCardRuleCheckBoxes(setStoreCardCheckBoxes, enableMinAppScore, enableMinBehaviouralScore, enableMinMnthsAcctHistX, enableMaxCurrMnthsInArrs,
                                                        enableMaxPrevMnthsInArrsX, enablePcentInitRFLimit, enableMaxCust);

                        }

                    }
                }
            }

            //IP - 10/12/10 - Store Card - Set the Store Card controls to disabled as no rules currently setup for the branch
            if (tcBranch.TabPages.Contains(tbStoreCardQualRules) == true && setStoreCardCheckBoxes == false)
            {
                SetStoreCardRuleCheckBoxes(setStoreCardCheckBoxes, enableMinAppScore, enableMinBehaviouralScore, enableMinMnthsAcctHistX, enableMaxCurrMnthsInArrs,
                                                      enableMaxPrevMnthsInArrsX, enablePcentInitRFLimit, enableMaxCust);
            }
            // Account Types are reloaded for each branch
            // Branch numbers are reloaded in case a new branch was created
            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
            dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AccountType, new string[] { Config.CountryCode, branchNo }));
            dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));

            DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.TableName == TN.AccountType)
                    {
                        drpAccountType.DataSource = null;
                        drpAccountType.DataSource = dt;
                        drpAccountType.DisplayMember = CN.AcctCat;
                    }

                    if (dt.TableName == TN.BranchNumber)
                    {
                        drpBranchNo.DataSource = null;
                        drpBranchNo.DataSource = dt;
                        drpBranchNo.DisplayMember = CN.BranchNo;

                        //CR 953 Make the default print location visible in a drop down
                        //IP - 23/02/10 - CR1072 - Malaysia 3PL for Version 5.2
                        DataView dvDefaultPrintLocation = new DataView(dt.Copy());
                        //dvDefaultPrintLocation.RowFilter = "ThirdPartyWarehouse = 'Y'";
                        dvDefaultPrintLocation.RowFilter = Convert.ToBoolean(Country[CountryParameterNames.ThirdPartyDeliveriesWarehouse]) == true ? "ThirdPartyWarehouse = 'Y'" : "ThirdPartyWarehouse = 'N'";     //IP - 25/05/12 - #10182 - Warehouse & Deliveries integration
                        drpDefaultPrintLocation.DataSource = dvDefaultPrintLocation;
                        drpDefaultPrintLocation.DisplayMember = CN.BranchNo;

                        drpDefaultPrintLocation.Text = defaultPrintLocation;

                        //if (Convert.ToBoolean(drpDefaultPrintLocation.FindStringExact(defaultPrintLocation)) == true)
                        //if (Convert.ToBoolean((drpDefaultPrintLocation.Items.Contains(defaultPrintLocation)))
                        //{
                        //    drpDefaultPrintLocation.Text = defaultPrintLocation;
                        //}
                        //else
                        //{
                        //    drpDefaultPrintLocation.SelectedIndex = -1;
                        //}

                    }
                }
            }

            if (!branchLoaded)
            {
                MessageBox.Show(this, "Branch not found!", "Warning");
            }

            int i = drpBranchNo.FindStringExact(branchNo);
            if (i >= 0) drpBranchNo.SelectedIndex = i;
            this.btnSave.Enabled = true;

            // Enable change events
            this._userChanged = true;
        }

        private bool SaveUpdates()
        {
            if (txtAS400BranchNo.Text == "") txtAS400BranchNo.Text = "0";
            if (txtCroffNo.Text == "") txtCroffNo.Text = "0";
            if (txtHissn.Text == "") txtHissn.Text = "0";
            if (txtHiBuffNo.Text == "") txtHiBuffNo.Text = "0";
            if (txtWarehouseNo.Text == "") txtWarehouseNo.Text = "0";
            if (txtCountryCode.Text == "") txtCountryCode.Text = Config.CountryCode;
            if (txtHiTransRefNo.Text == "") txtHiTransRefNo.Text = "0";

            if (!IsPositive(txtAS400BranchNo.Text.Trim())
                || !IsPositive(txtCroffNo.Text.Trim())
                || !IsPositive(txtHissn.Text.Trim())
                || !IsPositive(txtHiBuffNo.Text.Trim())
                || !IsPositive(txtHiTransRefNo.Text.Trim())
                || !IsPositive(txtBranchNo.Text.Trim()) || txtBranchNo.Text == "")
            {
                ShowInfo("M_INVALIDFIELDS");
                return false;
            }
            // Set store type               //CR903         jec
            string storeType = "";
            if (drpStoreType.SelectedIndex == 1)
                storeType = Convert.ToString(StoreType.Courts);
            else
                storeType = Convert.ToString(StoreType.NonCourts);

            DataSet depositSet = new DataSet();
            if (_depositListView.Table.DataSet.HasChanges(DataRowState.Modified))
                depositSet.Tables.Add(_depositListView.Table.GetChanges(DataRowState.Modified));
            else
                depositSet.Tables.Add(new DataTable());

            //IP - 23/02/10 - CR1072 - Malaysia 3PL for Version 5.2
            int? defaultPrintLocation = null;

            if (drpDefaultPrintLocation.Text != String.Empty)
                defaultPrintLocation = Convert.ToInt32(drpDefaultPrintLocation.Text);

            //IP - 7/12/10 - If the tab is visible then save the Store Card Qualification Rules
            Blue.Cosacs.Shared.StoreCardBranchQualRules storeCardQualrules = null;


            if (tcBranch.TabPages.Contains(tbStoreCardQualRules) == true)
            {
                var storeCardRulesApplyTo = string.Empty;          //IP - 9/12/10 - Store Card
                storeCardQualrules = new Blue.Cosacs.Shared.StoreCardBranchQualRules();

                //IP - 10/05/11 - Added extra check on all the below criteria to only save a null value if 0 value and criteria is not enabled.
                storeCardQualrules.BranchNo = Convert.ToInt16(txtBranchNo.Text);
                storeCardQualrules.MinApplicationScore = numMinAppScore.Value == 0 && !(chkMinApplicationScore.Checked) ? (int?)null : Convert.ToInt32(numMinAppScore.Value);
                storeCardQualrules.MinBehaviouralScore = numMinBehaviouralScore.Value == 0 && !(chkMinBehaviouralScore.Checked) ? (int?)null : Convert.ToInt32(numMinBehaviouralScore.Value);
                storeCardQualrules.MinMthsAcctHistX = numMinMnthsAcctHistX.Value == 0 && !(chkMinMthsAcctHist.Checked) ? (int?)null : Convert.ToInt32(numMinMnthsAcctHistX.Value);     //IP - 21/04/11 - Feature 3000
                storeCardQualrules.MinMthsAcctHistY = numMinMnthsAcctHistY.Value == 0 && !(chkMinMthsAcctHist.Checked) ? (int?)null : Convert.ToInt32(numMinMnthsAcctHistY.Value);  //IP - 21/04/11 - Feature 3000
                storeCardQualrules.MaxCurrMthsInArrs = numMaxCurrMnthsInArrs.Value == 0 && !(chkMaxCurrMthsInArrs.Checked) ? (decimal?)null : Convert.ToDecimal(numMaxCurrMnthsInArrs.Value);
                storeCardQualrules.MaxPrevMthsInArrsX = numMaxPrevMnthsInArrsX.Value == 0 && !(chkMaxPrevMthsInArrs.Checked) ? (decimal?)null : Convert.ToDecimal(numMaxPrevMnthsInArrsX.Value); //IP - 21/04/11 - Feature 3000
                storeCardQualrules.MaxPrevMthsInArrsY = numMaxPrevMnthsInArrsY.Value == 0 && !(chkMaxPrevMthsInArrs.Checked) ? (int?)null : Convert.ToInt32(numMaxPrevMnthsInArrsY.Value); //IP - 21/04/11 - Feature 3000
                storeCardQualrules.PcentInitRFLimit = numPcentInitRFLimit.Value == 0 && !(chkMinAvailRFLimit.Checked) ? (double?)null : Convert.ToDouble(numPcentInitRFLimit.Value);  //IP - 21/04/11 - Feature 3000
                storeCardQualrules.MaxNoCustForApproval = numMaxCust.Value == 0 && !(chkMaxNoCust.Checked) ? (int?)null : Convert.ToInt32(numMaxCust.Value); //IP - 10/05/11 - Feature - 3593
                storeCardQualrules.EmpeenoChange = Credential.UserId;

                //Changes will be applied to...
                if (rbAllBranches.Checked)
                { storeCardRulesApplyTo = "All"; }
                else if (rbCurrentBranch.Checked)
                { storeCardRulesApplyTo = "Current"; }
                else if (rbCourtsBranches.Checked)
                { storeCardRulesApplyTo = "Courts"; }
                else if (rbNonCourtsBranches.Checked)
                { storeCardRulesApplyTo = "NonCourts"; }

                storeCardQualrules.ApplyTo = storeCardRulesApplyTo;

            }

            bool? luckyDollarStore = rbLuckyDollar.Checked;
            bool? ashleyStore = rbAshley.Checked;

            if (storeType == Convert.ToString(StoreType.Courts))
            {
                luckyDollarStore = null;
                ashleyStore = null;
            }
            else
            {
                luckyDollarStore = rbLuckyDollar.Checked;
                ashleyStore = rbAshley.Checked;
            }

            AccountManager.BranchUpdate(
                Convert.ToInt32(txtBranchNo.Text),
                this.txtBranchName.Text,
                this.txtAddress1.Text, this.txtAddress2.Text, this.txtAddress3.Text,
                this.txtPostCode.Text, this.txtTel.Text,
                this.txtCountryCode.Text, Convert.ToInt32(this.txtCroffNo.Text),
                this.txtOldCard.Text, this.txtNewCard.Text, this.dtCardChange.Value,
                Convert.ToInt32(this.txtHissn.Text), Convert.ToInt32(this.txtHiBuffNo.Text), this.txtWarehouseNo.Text,
                Convert.ToInt32(this.txtHiTransRefNo.Text), Convert.ToInt32(this.txtAS400BranchNo.Text), this.txtRegion.Text,
                this.chxDepositLocked.Checked, depositSet, this.txtWarehouseRegion.Text,
                storeType, this.cbCreateRFAccts.Checked, this.cbCreateCashAccts.Checked,
                this.cbScoreHPbefore.Checked, this.cbCreateHPAccts.Checked, this.txtFact2000BranchLetter.Text.ToUpper(),
                cbServiceRepairCentre.Checked, Chk_behavioural.Checked, 
                defaultPrintLocation, chkWarehouse.Checked,
                cbCreateStoreAccts.Checked, WebTypeConverter.GetStoreCardBranchQualRulesWS2(storeCardQualrules), cbCashLoanBranch.Checked,
                luckyDollarStore, ashleyStore, out Error);

            if (Error.Length > 0)
            {
                ShowError(Error);
                return false;
            }
            else
            {
                //  CR903 
                ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_BRANCHDETAILSSAVED");
                return true;
            }
        }


        private void Branch_Load(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                this._userChanged = false;
                LoadStaticData();

                //IP - 25/02/10 - CR1072 - Malaysia 3PL for Version 5.2

                //if (Convert.ToBoolean(Country[CountryParameterNames.ThirdPartyDeliveriesWarehouse]) == true)
                //{
                //    grp3PLWarehouse.Visible = true;
                //}
                //else
                //{
                //    grp3PLWarehouse.Visible = false;
                //}

                grp3PLWarehouse.Visible = true;     //IP - 25/05/12 - #10182 - Warehouse & Deliveries integration

                if (Convert.ToBoolean(Country[CountryParameterNames.ThirdPartyDeliveriesWarehouse]) == false)        //IP - 25/05/12 - #10182 - Warehouse & Deliveries integration
                {
                    chkWarehouse.Visible = false;
                    grp3PLWarehouse.Text = string.Empty;
                    lbl3PLDefaultPrintLocn.Text = "Delivery Branch";
                }

                LoadData(Config.BranchCode);
                this._userChanged = true;

                if (!lDetails.Enabled)
                    tcBranch.TabPages.Remove(tbDetails);
                if (!lDeposits.Enabled)
                    tcBranch.TabPages.Remove(tbDeposits);
                if (!lNumGeneration.Enabled)
                    tcBranch.TabPages.Remove(tbGenerate);
                if (!lStoreCardQualRules.Enabled || !Convert.ToBoolean(Country[CountryParameterNames.StoreCardEnabled]))  //IP - 9/12/10 - Store Card
                    tcBranch.TabPages.Remove(tbStoreCardQualRules);

            }
            catch (Exception ex)
            {
                Catch(ex, "Branch_Load");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                bool saveBranch = true;             //CR903    jec
                errorProvider1.SetError(this.drpStoreType, "");

                txtBranchNo.Text = txtBranchNo.Text.Trim();
                if (!IsPositive(txtBranchNo.Text) || txtBranchNo.Text == "")
                {
                    errorProvider1.SetError(this.txtBranchNo, GetResource("M_POSITIVENUM"));
                    saveBranch = false;             //CR903    jec
                }

                //IP - 25/02/10 - CR1072 - Malaysia 3PL for Version 5.2
                //70329 Only prevent saving if there are values in the drpDefaultPrintLocation drop down
                if (drpDefaultPrintLocation.Text == String.Empty && drpDefaultPrintLocation.Items.Count > 0)
                {
                    errorProvider1.SetError(this.drpDefaultPrintLocation, GetResource("M_POSITIVENUM"));
                    saveBranch = false;
                }

                if (drpStoreType.SelectedIndex == 0)        //CR903    jec  
                {
                    errorProvider1.SetError(this.drpStoreType, GetResource("M_STORETYPE"));
                    saveBranch = false;
                }

                //IP - 8/12/10 - Store Card
                if (tcBranch.TabPages.Contains(tbStoreCardQualRules) == true)
                {

                    if ((numMinAppScore.Text == string.Empty) && chkMinApplicationScore.Checked == true) //IP - 10/05/11 - Removed check on 0 value
                    {
                        errorProviderStoreCard.SetError(numMinAppScore, GetResource("M_ENTERMANDATORY"));
                        saveBranch = false;
                    }
                    else
                    {
                        errorProviderStoreCard.SetError(numMinAppScore, "");
                    }

                    if ((numMinBehaviouralScore.Text == string.Empty) && chkMinBehaviouralScore.Checked == true) //IP - 10/05/11 - Removed check on 0 value
                    {
                        errorProviderStoreCard.SetError(numMinBehaviouralScore, GetResource("M_ENTERMANDATORY"));
                        saveBranch = false;
                    }
                    else
                    {
                        errorProviderStoreCard.SetError(numMinBehaviouralScore, "");
                    }

                    if ((numMinMnthsAcctHistX.Text == string.Empty || numMinMnthsAcctHistY.Text == string.Empty)  //IP - 10/05/11 - Removed check on 0 value
                        && chkMinMthsAcctHist.Checked == true) //IP - 26/04/11 - Feature 3000 - added check on numMinMnthsAcctHistY  
                    {
                        //IP - 26/04/11
                        if (numMinMnthsAcctHistX.Text == string.Empty)
                        {
                            errorProviderStoreCard.SetError(numMinMnthsAcctHistX, GetResource("M_ENTERMANDATORY"));
                        }
                        else
                        {
                            errorProviderStoreCard.SetError(numMinMnthsAcctHistX, "");
                        }

                        if (numMinMnthsAcctHistY.Text == string.Empty)
                        {
                            errorProviderStoreCard.SetError(numMinMnthsAcctHistY, GetResource("M_ENTERMANDATORY"));
                        }
                        else
                        {
                            errorProviderStoreCard.SetError(numMinMnthsAcctHistY, "");  //IP - 26/04/11
                        }

                        saveBranch = false;
                    }
                    else
                    {
                        errorProviderStoreCard.SetError(numMinMnthsAcctHistX, "");
                        errorProviderStoreCard.SetError(numMinMnthsAcctHistY, "");  //IP - 26/04/11
                    }

                    //IP - 02/05/12 - #10073
                    if (numMinMnthsAcctHistX.Text != string.Empty && numMinMnthsAcctHistY.Text != string.Empty &&
                       (Convert.ToInt32(numMinMnthsAcctHistX.Text) > Convert.ToInt32(numMinMnthsAcctHistY.Text)) &&
                        chkMinMthsAcctHist.Checked == true)
                    {
                        errorProviderStoreCard.SetError(numMinMnthsAcctHistX, GetResource("M_SCARDQUALMINMTHSHIST"));

                        saveBranch = false;
                    }
                    else
                    {
                        errorProviderStoreCard.SetError(numMinMnthsAcctHistX, "");
                    }


                    if ((numMaxCurrMnthsInArrs.Text == string.Empty) && chkMaxCurrMthsInArrs.Checked == true) //IP - 10/05/11 - Removed check on 0 value
                    {
                        errorProviderStoreCard.SetError(numMaxCurrMnthsInArrs, GetResource("M_ENTERMANDATORY"));
                        saveBranch = false;
                    }
                    else
                    {
                        errorProviderStoreCard.SetError(numMaxCurrMnthsInArrs, "");
                    }

                    if ((numMaxPrevMnthsInArrsX.Text == string.Empty || numMaxPrevMnthsInArrsY.Text == string.Empty)
                        && chkMaxPrevMthsInArrs.Checked == true)  //IP - 26/04/11 - Feature 3000 - added check on numMaxPrevMnthsInArrsY  //IP - 10/05/11 - Removed check on 0 value
                    {
                        //IP - 26/04/11
                        if (numMaxPrevMnthsInArrsX.Text == string.Empty)
                        {
                            errorProviderStoreCard.SetError(numMaxPrevMnthsInArrsX, GetResource("M_ENTERMANDATORY"));
                        }
                        else
                        {
                            errorProviderStoreCard.SetError(numMaxPrevMnthsInArrsX, "");
                        }

                        if (numMaxPrevMnthsInArrsY.Text == string.Empty)
                        {
                            errorProviderStoreCard.SetError(numMaxPrevMnthsInArrsY, GetResource("M_ENTERMANDATORY"));
                        }
                        else
                        {
                            errorProviderStoreCard.SetError(numMaxPrevMnthsInArrsY, "");    //IP - 26/04/11
                        }

                        saveBranch = false;
                    }
                    else
                    {
                        errorProviderStoreCard.SetError(numMaxPrevMnthsInArrsX, "");
                        errorProviderStoreCard.SetError(numMaxPrevMnthsInArrsY, "");    //IP - 26/04/11
                    }

                    //IP - 02/05/12 - #10073
                    if (numMaxPrevMnthsInArrsX.Text != string.Empty && numMaxPrevMnthsInArrsY.Text != string.Empty &&
                       (Convert.ToDecimal(numMaxPrevMnthsInArrsX.Text) > Convert.ToDecimal(numMaxPrevMnthsInArrsY.Text)) &&
                        chkMaxPrevMthsInArrs.Checked == true)
                    {
                        errorProviderStoreCard.SetError(numMaxPrevMnthsInArrsX, GetResource("M_SCARDQUALMAXPREVMTHSARRS"));

                        saveBranch = false;
                    }
                    else
                    {
                        errorProviderStoreCard.SetError(numMaxPrevMnthsInArrsX, "");
                    }

                    if ((numPcentInitRFLimit.Text == string.Empty) && chkMinAvailRFLimit.Checked == true) //IP - 10/05/11 - Removed check on 0 value
                    {
                        errorProviderStoreCard.SetError(numPcentInitRFLimit, GetResource("M_ENTERMANDATORY"));
                        saveBranch = false;
                    }
                    else
                    {
                        errorProviderStoreCard.SetError(numPcentInitRFLimit, "");
                    }

                    //IP - 10/05/11 - Store Card - Feature - #3593
                    if ((numMaxCust.Text == string.Empty) && chkMaxNoCust.Checked == true)
                    {
                        errorProviderStoreCard.SetError(numMaxCust, GetResource("M_ENTERMANDATORY"));
                        saveBranch = false;
                    }
                    else
                    {
                        errorProviderStoreCard.SetError(numMaxCust, "");
                    }

                }

                if (saveBranch == true)
                {
                    errorProvider1.SetError(this.txtBranchNo, "");
                    errorProvider1.SetError(this.drpDefaultPrintLocation, String.Empty);  //IP - 25/02/10 - CR1072 - Malaysia 3PL for Version 5.2
                    if (SaveUpdates())
                    {
                        LoadData(txtBranchNo.Text);
                        drpDefaultPrintLocation.Enabled = chkWarehouse.Checked ? false : true; //IP - 23/02/10 - CR1072 - Malaysia 3PL for Version 5.2
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

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                Clear();
                // When the user clears the screen reset the branch drop down to the
                // first branch in the list and reload that branch - but don't rely
                // on the change event in case the drop down is already on this branch.
                this._userChanged = false;
                this.drpBranchNo.SelectedIndex = 0;
                LoadData(drpBranchNo.Text);
                this._userChanged = true;
                //  CR903 
                ((MainForm)this.FormRoot).statusBar1.Text = "";
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

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                CloseTab();
            }
            catch (Exception ex)
            {
                Catch(ex, "btnExit_Click");
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

        private void drpBranchNo_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                if (_userChanged)
                {
                    LoadData(drpBranchNo.Text);
                    drpDefaultPrintLocation.Enabled = chkWarehouse.Checked ? false : true; //IP - 23/02/10 - CR1072 - Malaysia 3PL for Version 5.2
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "drpBranchNo_SelectedIndexChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void txtBranchNo_Leave(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                if (_userChanged)
                {
                    txtBranchNo.Text = txtBranchNo.Text.Trim();

                    //IP 29/11/2007 - 69390
                    //Prevent users from creating branch numbers that are less than 3 digits.
                    bool branchOk = true;

                    if (txtBranchNo.Text.Length != 3)
                    {
                        errorProvider1.SetError(this.txtBranchNo, "Branch Numbers should be 3 digits");
                        branchOk = false;
                    }

                    if (!IsPositive(txtBranchNo.Text) || txtBranchNo.Text == "")
                    {
                        errorProvider1.SetError(this.txtBranchNo, GetResource("M_POSITIVENUM"));
                        branchOk = false;
                    }

                    if (branchOk)
                    {
                        errorProvider1.SetError(this.txtBranchNo, "");
                        LoadData(txtBranchNo.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "txtBranchNo_Leave");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnPrintAccountNos_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                if (numAccountNo.Value > 0)
                {
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_NUMBERACCOUNTPRINT");
                    string accountType = (string)((DataRowView)drpAccountType.SelectedItem)[CN.AcctCat];
                    PrintAccountNos(CreateBrowserArray(1)[0], numAccountNo.Value, accountType);
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_NUMBERACCOUNTPRINTED");
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnPrintAccountNos_Click");
            }
            finally
            {
                if (numAccountNo.Value < 1) ((MainForm)this.FormRoot).statusBar1.Text = "";
                StopWait();
            }
        }

        private void btnPrintContractNos_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                if (numContractNo.Value > 0)
                {
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_NUMBERCONTRACTPRINT");
                    PrintContractNos(CreateBrowserArray(1)[0], numContractNo.Value);
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_NUMBERCONTRACTPRINTED");
                }

            }
            catch (Exception ex)
            {
                Catch(ex, "btnPrintContractNos_Click");
            }
            finally
            {
                if (numContractNo.Value < 1) ((MainForm)this.FormRoot).statusBar1.Text = "";
                StopWait();
            }
        }

        private void dgDepositList_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Wait();
                if (_depositListView.Table.Columns[CN.Description].Ordinal == dgDepositList.CurrentCell.ColumnNumber)
                {
                    // The user has clicked on the Deposit Type column
                    int left = dgDepositList.GetCellBounds(dgDepositList.CurrentCell).Left;
                    int top = dgDepositList.GetCellBounds(dgDepositList.CurrentCell).Top;
                    int width = dgDepositList.GetCellBounds(dgDepositList.CurrentCell).Width;

                    // Position over this cell
                    drpDeposit.Left = dgDepositList.Left + left;
                    drpDeposit.Top = dgDepositList.Top + top;
                    drpDeposit.Width = width;

                    //IP - 05/05/09 - Livewire (71112) - If no bank deposits have been setup display a message to the user.
                    if (drpDeposit.Items.Count == 0)
                    {
                        ShowInfo("M_BANKDEPOSITSETUP");
                    }
                    else
                    {
                        // Set to the value of this cell
                        string curText = (string)_depositListView[dgDepositList.CurrentCell.RowNumber][CN.Description];
                        int i = drpDeposit.FindStringExact(curText);
                        drpDeposit.SelectedIndex = i >= 0 ? i : 0;

                        // Display the drop down
                        drpDeposit.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "dgDepositList_MouseUp");
            }
            finally
            {
                StopWait();
            }
        }

        //private void dgDepositList_Leave(object sender, System.Windows.Forms.NavigateEventArgs ne)
        //{
        //    try
        //    {
        //        Wait();
        //        // Hide the drop down
        //        drpDeposit.Visible = false;
        //    }
        //    catch(Exception ex)
        //    {
        //        Catch(ex, "dgDepositList_Leave");
        //    }
        //    finally
        //    {
        //        StopWait();
        //    }		
        //}

        private void drpDeposit_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                if (_userChanged)
                {
                    // Write the new value back to the cell
                    if (_depositListView.Table.Columns[CN.Description].Ordinal == dgDepositList.CurrentCell.ColumnNumber)
                    {
                        _depositListView[dgDepositList.CurrentCell.RowNumber][CN.Deposit] = drpDeposit.SelectedValue;
                        _depositListView[dgDepositList.CurrentCell.RowNumber][CN.Description] = drpDeposit.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "drpDeposit_SelectedIndexChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void txtAS400BranchNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                if (!IsPositive(txtAS400BranchNo.Text.Trim()))
                    errorProvider1.SetError(this.txtAS400BranchNo, GetResource("M_POSITIVENUM"));
                else
                    errorProvider1.SetError(this.txtAS400BranchNo, "");
            }
            catch (Exception ex)
            {
                Catch(ex, "txtAS400BranchNo_TextChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void txtCroffNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                if (!IsPositive(txtCroffNo.Text.Trim()))
                    errorProvider1.SetError(this.txtCroffNo, GetResource("M_POSITIVENUM"));
                else
                    errorProvider1.SetError(this.txtCroffNo, "");
            }
            catch (Exception ex)
            {
                Catch(ex, "txtCroffNo_TextChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void txtHissn_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                if (!IsPositive(txtHissn.Text.Trim()))
                    errorProvider1.SetError(this.txtHissn, GetResource("M_POSITIVENUM"));
                else
                    errorProvider1.SetError(this.txtHissn, "");
            }
            catch (Exception ex)
            {
                Catch(ex, "txtHissn_TextChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void txtHiBuffNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                if (!IsPositive(txtHiBuffNo.Text.Trim()))
                    errorProvider1.SetError(this.txtHiBuffNo, GetResource("M_POSITIVENUM"));
                else
                    errorProvider1.SetError(this.txtHiBuffNo, "");
            }
            catch (Exception ex)
            {
                Catch(ex, "txtHiBuffNo_TextChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void txtHiTransRefNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                if (!IsPositive(txtHiTransRefNo.Text.Trim()))
                    errorProvider1.SetError(this.txtHiTransRefNo, GetResource("M_POSITIVENUM"));
                else
                    errorProvider1.SetError(this.txtHiTransRefNo, "");
            }
            catch (Exception ex)
            {
                Catch(ex, "txtHiTransRefNo_TextChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void txtBranchNo_TextChanged(object sender, System.EventArgs e)
        {
            this.btnSave.Enabled = false;
            errorProvider1.SetError(this.txtBranchNo, "");
        }

        //IP - 23/02/10 - CR1072 - Malaysia 3PL for Version 5.2
        private void chkWarehouse_CheckedChanged(object sender, EventArgs e)
        {
            if (chkWarehouse.Checked)
            {
                drpDefaultPrintLocation.Text = txtBranchNo.Text;
            }
            else
            {
                if (drpDefaultPrintLocation.Text == txtBranchNo.Text)
                {
                    try
                    {
                        int index = drpDefaultPrintLocation.FindStringExact(txtBranchNo.Text);
                        ((DataView)drpDefaultPrintLocation.DataSource).Delete(index);
                    }
                    catch
                    {
                        // Nothing required
                    }
                }
                drpDefaultPrintLocation.Text = String.Empty;
            }

            drpDefaultPrintLocation.Enabled = chkWarehouse.Checked ? false : true;
        }

        //IP - 8/12/10 - Store Card
        private void chkMinApplicationScore_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMinApplicationScore.Checked)
            {
                numMinAppScore.Enabled = true;

            }
            else
            {
                numMinAppScore.Enabled = false;
                numMinAppScore.Value = 0;
            }
        }

        //IP - 8/12/10 - Store Card
        private void chkMinBehaviouralScore_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMinBehaviouralScore.Checked)
            {
                numMinBehaviouralScore.Enabled = true;
            }
            else
            {
                numMinBehaviouralScore.Enabled = false;
                numMinBehaviouralScore.Value = 0;
            }
        }

        //IP - 8/12/10 - Store Card
        private void chkMinMthsAcctHist_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMinMthsAcctHist.Checked)
            {
                numMinMnthsAcctHistX.Enabled = true;
                numMinMnthsAcctHistY.Enabled = true;            //IP - 26/04/11 - Feature 3000
            }
            else
            {
                numMinMnthsAcctHistX.Enabled = false;
                numMinMnthsAcctHistX.Value = 0;

                numMinMnthsAcctHistY.Enabled = false;            //IP - 26/04/11 - Feature 3000
                numMinMnthsAcctHistY.Value = 0;                  //IP - 26/04/11 - Feature 3000
            }
        }

        //IP - 8/12/10 - Store Card
        private void chkMaxCurrMthsInArrs_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMaxCurrMthsInArrs.Checked)
            {
                numMaxCurrMnthsInArrs.Enabled = true;
            }
            else
            {
                numMaxCurrMnthsInArrs.Enabled = false;
                numMaxCurrMnthsInArrs.Value = 0;
            }
        }

        //IP - 8/12/10 - Store Card
        private void chkMaxPrevMthsInArrs_CheckedChanged(object sender, EventArgs e)
        {

            if (chkMaxPrevMthsInArrs.Checked)
            {
                numMaxPrevMnthsInArrsX.Enabled = true;
                numMaxPrevMnthsInArrsY.Enabled = true;              //IP - 26/04/11 - Feature 3000
            }
            else
            {
                numMaxPrevMnthsInArrsX.Enabled = false;
                numMaxPrevMnthsInArrsX.Value = 0;

                numMaxPrevMnthsInArrsY.Enabled = false;             //IP - 26/04/11 - Feature 3000
                numMaxPrevMnthsInArrsY.Value = 0;                   //IP - 26/04/11 - Feature 3000
            }
        }

        //IP - 8/12/10 - Store Card
        private void chkMinAvailRFLimit_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMinAvailRFLimit.Checked)
            {
                numPcentInitRFLimit.Enabled = true;
            }
            else
            {
                numPcentInitRFLimit.Enabled = false;
                numPcentInitRFLimit.Value = 0;
            }
        }

        //IP - 13/05/11 - Feature 3593
        private void chkMaxNoCust_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMaxNoCust.Checked)
            {
                numMaxCust.Enabled = true;
            }
            else
            {
                numMaxCust.Enabled = false;
                numMaxCust.Value = 0;
            }
        }


        //IP - 8/12/10 - Store Card - Method that sets the check boxes to enabled or disabled depending on the values returned.
        public void SetStoreCardRuleCheckBoxes(bool Set, bool enableMinAppScore, bool enableMinBehaviouralScore, bool enableMinMnthsAcctHistX, bool enableMaxCurrMnthsInArrs, bool enableMaxPrevMnthsInArrsX,
            bool enablePcentInitRFLimit, bool enableMaxCust)
        {
            //IP - 10/05/11 - Changed below to check for null value instead of 0. If null then the criteria should not be enabled.
            if (Set)
            {
                if (enableMinAppScore == true)
                {
                    chkMinApplicationScore.Checked = true;
                }
                else
                {
                    chkMinApplicationScore_CheckedChanged(null, null);
                }

                if (enableMinBehaviouralScore == true)
                {
                    chkMinBehaviouralScore.Checked = true;
                }
                else
                {
                    chkMinBehaviouralScore_CheckedChanged(null, null);
                }

                if (enableMinMnthsAcctHistX == true)
                {
                    chkMinMthsAcctHist.Checked = true;
                }
                else
                {
                    chkMinMthsAcctHist_CheckedChanged(null, null);
                }

                if (enableMaxCurrMnthsInArrs == true)
                {
                    chkMaxCurrMthsInArrs.Checked = true;
                }
                else
                {
                    chkMaxCurrMthsInArrs_CheckedChanged(null, null);
                }

                if (enableMaxPrevMnthsInArrsX == true)
                {
                    chkMaxPrevMthsInArrs.Checked = true;
                }
                else
                {
                    chkMaxPrevMthsInArrs_CheckedChanged(null, null);
                }

                if (enablePcentInitRFLimit == true)
                {
                    chkMinAvailRFLimit.Checked = true;
                }
                else
                {
                    chkMinAvailRFLimit_CheckedChanged(null, null);
                }

                //IP - 10/05/11
                if (enableMaxCust)
                {
                    chkMaxNoCust.Checked = true;
                }
                else
                {
                    chkMaxNoCust_CheckedChanged(null, null);
                }

            }
            else
            {
                //Need to fire the events manually to ensure that if no rules returned for a branch 
                //the controls need to be disabled for all criteria.
                chkMinApplicationScore_CheckedChanged(null, null);
                chkMinBehaviouralScore_CheckedChanged(null, null);
                chkMinMthsAcctHist_CheckedChanged(null, null);
                chkMaxCurrMthsInArrs_CheckedChanged(null, null);
                chkMaxPrevMthsInArrs_CheckedChanged(null, null);
                chkMinAvailRFLimit_CheckedChanged(null, null);

            }

        }


        private void rbCurrentBranch_CheckedChanged(object sender, EventArgs e)
        {

        }


        private void rbAllBranches_CheckedChanged(object sender, EventArgs e)
        {

        }


        private void rbCourtsBranches_CheckedChanged(object sender, EventArgs e)
        {

        }


        private void rbNonCourtsBranches_CheckedChanged(object sender, EventArgs e)
        {

        }



        private void tcBranch_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void tbStoreCardQualRules_Enter(object sender, EventArgs e)
        {

        }

        private void lblEnableDisableStoreCardRules_MouseHover(object sender, EventArgs e)
        {
            toolTipStoreCard.SetToolTip(lblEnableDisableStoreCardRules, "Check or Uncheck to include or exclude a rule");
        }

        private void lblStoreCardCriteria_MouseHover(object sender, EventArgs e)
        {
            toolTipStoreCard.SetToolTip(lblStoreCardCriteria, "Criteria required for Store Card qualification");
        }

        //private void label4_MouseHover(object sender, EventArgs e)
        //{
        //    toolTipStoreCard.SetToolTip(lblStoreCardCriteria, "Criteria required for Store Card qualification");
        //}

        private void lblStoreCardApplyTo_MouseHover(object sender, EventArgs e)
        {
            toolTipStoreCard.SetToolTip(lblStoreCardApplyTo, "Select where you wish the rules to apply");
        }

        private void rbCurrentBranch_MouseHover(object sender, EventArgs e)
        {
            toolTipStoreCard.SetToolTip(rbCurrentBranch, "Upon saving the rules will apply to the current branch");
        }

        private void rbAllBranches_MouseHover(object sender, EventArgs e)
        {
            toolTipStoreCard.SetToolTip(rbAllBranches, "Upon saving the rules will apply to all branches");
        }

        private void rbCourtsBranches_MouseHover(object sender, EventArgs e)
        {
            toolTipStoreCard.SetToolTip(rbCourtsBranches, "Upon saving the rules will only apply to Courts branches");
        }

        private void rbNonCourtsBranches_MouseHover(object sender, EventArgs e)
        {
            toolTipStoreCard.SetToolTip(rbNonCourtsBranches, "Upon saving the rules will only apply to Non Courts branches");
        }

        //IP - 26/04/11 - Feature 3000
        private void ClearStoreCardFields()
        {
            errorProviderStoreCard.SetError(numMinAppScore, "");
            errorProviderStoreCard.SetError(numMinBehaviouralScore, "");
            errorProviderStoreCard.SetError(numMinMnthsAcctHistX, "");
            errorProviderStoreCard.SetError(numMinMnthsAcctHistY, "");
            errorProviderStoreCard.SetError(numMaxCurrMnthsInArrs, "");
            errorProviderStoreCard.SetError(numMaxPrevMnthsInArrsX, "");
            errorProviderStoreCard.SetError(numMaxPrevMnthsInArrsY, "");
            errorProviderStoreCard.SetError(numPcentInitRFLimit, "");
            errorProviderStoreCard.SetError(numMaxCust, "");
        }

        //IP - 26/04/11 - Added tool tips
        private void lblMinAppScore_MouseHover(object sender, EventArgs e)
        {
            toolTipStoreCard.SetToolTip(lblMinAppScore, "Customer must have an application score >= the value entered on the latest proposal");
        }

        private void lblMinBehaviouralScore_MouseHover(object sender, EventArgs e)
        {
            toolTipStoreCard.SetToolTip(lblMinBehaviouralScore, "Customer must have a behavioural score >= the value entered on the latest proposal");
        }

        private void lblMinMnthsAcctHistX_MouseHover(object sender, EventArgs e)
        {
            toolTipStoreCard.SetToolTip(lblMinMnthsAcctHistX, "Customer must have had an active credit account for the specified number of months (X) in the last (Y) months");
        }

        private void lblMaxCurrMnthsInArrs_MouseHover(object sender, EventArgs e)
        {
            toolTipStoreCard.SetToolTip(lblMaxCurrMnthsInArrs, "Customers active accounts should not have exceeded the specified number of months in arrears");
        }

        private void lblPrevMaxMnthsInArrs_MouseHover(object sender, EventArgs e)
        {
            toolTipStoreCard.SetToolTip(lblPrevMaxMnthsInArrs, "Customer should not have had any credit accounts in arrears greater than the specified number of months (X) in the last (Y) months");
        }

        private void lblPcentInitialLimit_MouseHover(object sender, EventArgs e)
        {
            toolTipStoreCard.SetToolTip(lblPcentInitialLimit, "The minimum percentage RF limit available to quailfy for storecard. ");
        }

        private void lblMaxCustForPreApp_MouseHover(object sender, EventArgs e)
        {
            toolTipStoreCard.SetToolTip(lblPcentInitialLimit, "This is the maximum number of customers that should be pre-qualified for Store Card");
        }

        private void drpStoreType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpStoreType.SelectedIndex == 1) //Courts Store
            {
                grpNonCourtsStoreTypes.Visible = false;
            }
            else //Non Courts Store
            {
                grpNonCourtsStoreTypes.Visible = true;
            }
        }

    }

}
