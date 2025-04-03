using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Elements;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.SanctionStages;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Tags;
using STL.Common.Static;

namespace STL.PL
{
    /// <summary>
    /// When a customer credit application fails to qualify for any credit it can
    /// be left in a referral state. A credit officer will use this screen to review
    /// any application that has been referred. The system reason for the referral,
    /// the credit points scored and any user notes can be seen here. The credit 
    /// officer has the option to manually enter a credit limit and approve the
    /// application, or to reject the application.
    /// </summary>
    public class Referral : CommonForm
    {
        //private Crownwood.Magic.Menus.MenuControl menuMain;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuExit;

        private DataTable dtScoringDetails = null;

        private new string Error = "";
        private bool _preventSanctionStatusHide;

        private string _custid = "";
        public string CustomerID
        {
            get { return _custid; }
            set { _custid = value; }
        }

        private string _acctno = "";
        public string AccountNo
        {
            get { return _acctno; }
            set { _acctno = value; }
        }

        private string _currentStatus = "";
        public string CurrentStatus
        {
            get { return _currentStatus; }
            set { _currentStatus = value; }
        }

        private string _acctType = "";
        public string AccountType
        {
            get { return _acctType; }
            set { _acctType = value; }
        }

        private string _screenMode = "";
        public string ScreenMode
        {
            get { return _screenMode; }
            set { _screenMode = value; }
        }

        private DateTime _dateProp;
        public DateTime DateProp
        {
            get { return _dateProp; }
            set { _dateProp = value; }
        }
        //70260 The account number selected on Customer Details which is to be approved or rejected is to be made available at class level
        private string m_selectedAccount;

        private bool _acctLocked = false;
        public bool AccountLocked
        {
            get { return _acctLocked; }
            set { _acctLocked = value; }
        }

        //Livewire 69230 new public property required to determine if the 'convert to HP' option should be available in the 'RFCreditRefused' dialog box
        private bool m_allowConversionToHP = false;
        public bool allowConversionToHP
        {
            get { return m_allowConversionToHP; }
            set { m_allowConversionToHP = value; }
        }

        private bool _readOnly = false;
        private System.Windows.Forms.ToolTip toolTip1;
        private Crownwood.Magic.Menus.MenuCommand menuReferral;
        private Crownwood.Magic.Menus.MenuCommand menuComplete;
        private System.ComponentModel.IContainer components;
        private bool reOpen = false;
        private string propresult = "";
        private Crownwood.Magic.Controls.TabPage tpBureau;
        private System.Windows.Forms.TextBox txtLitigationNo24;
        private System.Windows.Forms.ImageList menuIcons;
        //CR 843 Begin Changes //CR843 - Renamed  
        private Crownwood.Magic.Controls.TabPage tabPage1;  //CR843
        //CR 843 End Changes
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private string _band = "";
        private new bool loaded = false;
        private TabControl tabControl1;
        private TabPage tabPage2;
        private Label labelband;
        private Label label6;
        private ComboBox cmb_reason;
        private ComboBox cmb_band;
        private Label label25;
        private TextBox txtSysRecommend;
        private Label lCreditLimit;
        private TextBox txtCreditLimit;
        private TextBox txtScore;
        private Label label7;
        private RadioButton rbReject;
        private RadioButton rbApprove;
        private Label label1;
        private TextBox txtReason;
        private SplitContainer splitContainer2;
        private TabControl tabControl2;
        private TabPage tab_notes;
        private SplitContainer splitContainer1;
        private RichTextBox rtxtReferralNotes;
        private RichTextBox rtxtNewReferralNotes;
        private TabPage tab_ReferralHistory;
        private DataGridView dgv_referralhistory;
        private SplitContainer splitContainer3;
        private DataGrid dgPolicyRules;
        private Label label27;
        private DataGrid dgReferralAudit;
        private Label label26;
        private TextBox txtAccountNo;
        private Label label28;
        private TextBox txtFirstName;
        private TextBox txtCustomerID;
        private Button btnComplete;
        private DateTimePicker dtDateProp;
        private Button btnAccountDetails;
        private Label label4;
        private TextBox txtLastName;
        private Label label3;
        private Label lCustomerID;
        private Label label2;
        private TabPage tabPage3;
        private GroupBox groupBox1;
        private DatePicker dtDateInCurrentEmp;
        private Label label8;
        private TextBox txtAge;
        private Label label5;
        private Label label9;
        private TextBox textBox1;
        private TextBox txtIncomeAfterTax;
        private Label label32;
        private TextBox txtMaritalStatus;
        private TextBox txtOccupation;
        private Label label29;
        private Label label31;
        private TextBox txtResidentialStatus;
        private TextBox txtExpenses;
        private Label label30;
        private DatePicker dtDateInCurrentAddress;
        private GroupBox groupBox2;
        public DataGrid dgLineItems;
        public TreeView tvItems;
        private TextBox txtRFAccts;
        private Label label24;
        private TextBox textBox2;
        private Label label23;
        private Label label10;
        private TextBox txtAcctType;
        private Label label11;
        private TextBox txtAgreementTotal;
        private Label label21;
        private Label label20;
        private TextBox txtFinalInstalment;
        private TextBox txtInstalment;
        private TextBox txtDeposit;
        private Label label18;
        private Label label12;
        private TextBox txtTermsType;
        private Label label13;
        private TextBox txtRepaymentPcent;
        private TabPage tabPage4;
        private Label label22;
        private TextBox txtValueOfAllArrears;
        private Label label19;
        private TextBox txtNoOfReturnedCheques;
        private Label label17;
        private TextBox txtRepaymentPcentCurrent;
        private Label label16;
        private TextBox txtCashAccountsSettled;
        private Label label15;
        private TextBox txtFeesAndInterest;
        private Label label14;
        private TextBox txtLargestAgreement;
        private Label label33;
        private Label label34;
        private TextBox txtLongestAgreement;
        private Label label35;
        private TextBox txtNoAccountsInArrears;
        private Label label36;
        private TextBox txtTotalInstalmentValue;
        private Label label37;
        private TextBox txtOutstandingBalance;
        private Label label38;
        private TextBox txtWorstSettledStatus;
        private Label label39;
        private TextBox txtWorstCurrentStatus;
        private Label label40;
        private TextBox txtNoOfSettled;
        private Label label41;
        private TextBox txtNoOfCurrent;
        private bool txtCreditLimitAuth = false;
        private XmlNode LineItems = null;
        private DataTable itemsTable = null;
        private Label lblStoreCardLimit;
        private TextBox txtStoreCardLimit;
        private TextBox txtPercentUplift;
        private Label label42;
        private Label label43;
        private TextBox txtReferralLimit;
        private Label lblMmi;
        private TextBox txtMmi;
        private XmlDocument itemDoc = null;

        public bool ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; }
        }

        private void HashMenus()
        {
            //dynamicMenus[this.Name+":txtCreditLimit"] = this.txtCreditLimit; 
            //dynamicMenus[this.Name+":lCreditLimit"] = this.lCreditLimit; 
        }

        private BasicCustomerDetails CustomerScreen = null;

        public Referral(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuReferral });
        }

        public Referral(bool createHP, string custId, DateTime dateProp, string accountNo,
            string acctType, string mode,
            Form root, Form parent, BasicCustomerDetails customerScreen, bool removeCancellation)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            CustomerScreen = customerScreen;

            dynamicMenus = new Hashtable();
            HashMenus();
            ApplyRoleRestrictions();



            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuReferral });
            this.txtCustomerID.Text = _custid = custId;
            this.dtDateProp.Value = _dateProp = dateProp;
            // UAT579  jec 22/05/09            
            //    this.txtAccountNo.Text = FormatAccountNo(customerScreen.AccountNo);
            //    m_selectedAccount = customerScreen.AccountNo;          

            m_selectedAccount = customerScreen != null ? customerScreen.AccountNo : accountNo; //Part of LW 70260
            this.txtAccountNo.Text = FormatAccountNo(m_selectedAccount);
            accountNo = m_selectedAccount;

            _acctno = accountNo;
            _acctType = acctType;
            _screenMode = mode;
            reOpen = removeCancellation;
            //TranslateControls();

            //Livewire 69230 set allowConversionToHP property to determine if the 'convert to HP' option should be available in the 'RFCreditRefused' dialog box
            allowConversionToHP = createHP;

            cmb_reason.DataSource = (DataTable)StaticData.Tables[TN.BandLimitChange];
            cmb_reason.DisplayMember = CN.CodeDescription;
            cmb_reason.ValueMember = CN.Code;

            DataSet details = CreditManager.GetReferralSummaryData(accountNo, custId, acctType, dateProp, out LineItems, out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                Populate(details);
                BuildLineItems();
            }


        }

        private void SetReadOnly()
        {
            this.txtCustomerID.BackColor = SystemColors.Window;
            this.txtFirstName.BackColor = SystemColors.Window;
            this.txtLastName.BackColor = SystemColors.Window;
            this.txtReason.BackColor = SystemColors.Window;
            this.rtxtNewReferralNotes.ReadOnly = ReadOnly;
            if (ReadOnly)
                this.rtxtNewReferralNotes.BackColor = SystemColors.Control;
            else
                this.rtxtNewReferralNotes.BackColor = SystemColors.Window;
            //this.noAvail.BackColor = SystemColors.Window;
            this.btnComplete.Enabled = !ReadOnly;
            this.rbApprove.Enabled = !ReadOnly;
            this.rbReject.Enabled = !ReadOnly;
            this.menuComplete.Enabled = !ReadOnly;
            if (txtCreditLimit.Enabled)
                txtCreditLimit.ReadOnly = ReadOnly;
            this.txtAccountNo.BackColor = SystemColors.Window;
        }

        private void CalcCharsAvailable()
        {
            //string [] lines = rtxtReferralNotes.Lines;
            //int len = 0;
            //foreach(string s in lines)
            //    len += s.Length;

            //lines = rtxtNewReferralNotes.Lines;
            //foreach(string s in lines)
            //    len += s.Length;

            //if((1000-len)>=0)
            //    noAvail.Value = 1000 - len;	

            if (rtxtNewReferralNotes.Text.Length == 1000)
            {
                errorProvider1.SetError(tabControl1, "Maximum text limit reached");
            }
            else
            {
                errorProvider1.SetError(tabControl1, "");
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Referral));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuReferral = new Crownwood.Magic.Menus.MenuCommand();
            this.menuComplete = new Crownwood.Magic.Menus.MenuCommand();
            this.menuIcons = new System.Windows.Forms.ImageList(this.components);
            this.tabPage1 = new Crownwood.Magic.Controls.TabPage();
            this.tpBureau = new Crownwood.Magic.Controls.TabPage();
            this.txtLitigationNo24 = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtPercentUplift = new System.Windows.Forms.TextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label43 = new System.Windows.Forms.Label();
            this.txtReferralLimit = new System.Windows.Forms.TextBox();
            this.label42 = new System.Windows.Forms.Label();
            this.lblStoreCardLimit = new System.Windows.Forms.Label();
            this.txtStoreCardLimit = new System.Windows.Forms.TextBox();
            this.labelband = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmb_reason = new System.Windows.Forms.ComboBox();
            this.cmb_band = new System.Windows.Forms.ComboBox();
            this.label25 = new System.Windows.Forms.Label();
            this.txtSysRecommend = new System.Windows.Forms.TextBox();
            this.lCreditLimit = new System.Windows.Forms.Label();
            this.txtCreditLimit = new System.Windows.Forms.TextBox();
            this.txtScore = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.rbReject = new System.Windows.Forms.RadioButton();
            this.rbApprove = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.txtReason = new System.Windows.Forms.TextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tab_notes = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.rtxtReferralNotes = new System.Windows.Forms.RichTextBox();
            this.rtxtNewReferralNotes = new System.Windows.Forms.RichTextBox();
            this.tab_ReferralHistory = new System.Windows.Forms.TabPage();
            this.dgv_referralhistory = new System.Windows.Forms.DataGridView();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.dgPolicyRules = new System.Windows.Forms.DataGrid();
            this.label27 = new System.Windows.Forms.Label();
            this.dgReferralAudit = new System.Windows.Forms.DataGrid();
            this.label26 = new System.Windows.Forms.Label();
            this.txtAccountNo = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.txtCustomerID = new System.Windows.Forms.TextBox();
            this.btnComplete = new System.Windows.Forms.Button();
            this.dtDateProp = new System.Windows.Forms.DateTimePicker();
            this.btnAccountDetails = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lCustomerID = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgLineItems = new System.Windows.Forms.DataGrid();
            this.tvItems = new System.Windows.Forms.TreeView();
            this.txtRFAccts = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtAcctType = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtAgreementTotal = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.txtFinalInstalment = new System.Windows.Forms.TextBox();
            this.txtInstalment = new System.Windows.Forms.TextBox();
            this.txtDeposit = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtTermsType = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtRepaymentPcent = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtDateInCurrentEmp = new STL.PL.DatePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.txtAge = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.txtIncomeAfterTax = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.txtMaritalStatus = new System.Windows.Forms.TextBox();
            this.txtOccupation = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.txtResidentialStatus = new System.Windows.Forms.TextBox();
            this.txtExpenses = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.dtDateInCurrentAddress = new STL.PL.DatePicker();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label22 = new System.Windows.Forms.Label();
            this.txtValueOfAllArrears = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtNoOfReturnedCheques = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtRepaymentPcentCurrent = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtCashAccountsSettled = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtFeesAndInterest = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtLargestAgreement = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.txtLongestAgreement = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.txtNoAccountsInArrears = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.txtTotalInstalmentValue = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.txtOutstandingBalance = new System.Windows.Forms.TextBox();
            this.label38 = new System.Windows.Forms.Label();
            this.txtWorstSettledStatus = new System.Windows.Forms.TextBox();
            this.label39 = new System.Windows.Forms.Label();
            this.txtWorstCurrentStatus = new System.Windows.Forms.TextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.txtNoOfSettled = new System.Windows.Forms.TextBox();
            this.label41 = new System.Windows.Forms.Label();
            this.txtNoOfCurrent = new System.Windows.Forms.TextBox();
            this.lblMmi = new System.Windows.Forms.Label();
            this.txtMmi = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tab_notes.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tab_ReferralHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_referralhistory)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgPolicyRules)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgReferralAudit)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgLineItems)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabPage4.SuspendLayout();
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
            // menuReferral
            // 
            this.menuReferral.Description = "MenuItem";
            this.menuReferral.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuComplete});
            this.menuReferral.Text = "&Referral";
            // 
            // menuComplete
            // 
            this.menuComplete.Description = "MenuItem";
            this.menuComplete.Text = "&Complete";
            // 
            // menuIcons
            // 
            this.menuIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("menuIcons.ImageStream")));
            this.menuIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.menuIcons.Images.SetKeyName(0, "");
            this.menuIcons.Images.SetKeyName(1, "");
            this.menuIcons.Images.SetKeyName(2, "");
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(0, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Selected = false;
            this.tabPage1.Size = new System.Drawing.Size(770, 324);
            this.tabPage1.TabIndex = 7;
            // 
            // tpBureau
            // 
            this.tpBureau.Location = new System.Drawing.Point(0, 0);
            this.tpBureau.Name = "tpBureau";
            this.tpBureau.Selected = false;
            this.tpBureau.Size = new System.Drawing.Size(200, 100);
            this.tpBureau.TabIndex = 0;
            // 
            // txtLitigationNo24
            // 
            this.txtLitigationNo24.Location = new System.Drawing.Point(0, 25);
            this.txtLitigationNo24.Name = "txtLitigationNo24";
            this.txtLitigationNo24.ReadOnly = true;
            this.txtLitigationNo24.Size = new System.Drawing.Size(770, 20);
            this.txtLitigationNo24.TabIndex = 11;
            this.txtLitigationNo24.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtPercentUplift
            // 
            this.txtPercentUplift.Location = new System.Drawing.Point(255, 67);
            this.txtPercentUplift.MaxLength = 15;
            this.txtPercentUplift.Name = "txtPercentUplift";
            this.txtPercentUplift.ReadOnly = true;
            this.txtPercentUplift.Size = new System.Drawing.Size(88, 20);
            this.txtPercentUplift.TabIndex = 74;
            this.toolTip1.SetToolTip(this.txtPercentUplift, "Good Customers may have their credit limit uplifted if they exceed the referral l" +
        "imit");
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(792, 476);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.lblMmi);
            this.tabPage2.Controls.Add(this.txtMmi);
            this.tabPage2.Controls.Add(this.label43);
            this.tabPage2.Controls.Add(this.txtReferralLimit);
            this.tabPage2.Controls.Add(this.txtPercentUplift);
            this.tabPage2.Controls.Add(this.label42);
            this.tabPage2.Controls.Add(this.lblStoreCardLimit);
            this.tabPage2.Controls.Add(this.txtStoreCardLimit);
            this.tabPage2.Controls.Add(this.labelband);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.cmb_reason);
            this.tabPage2.Controls.Add(this.cmb_band);
            this.tabPage2.Controls.Add(this.label25);
            this.tabPage2.Controls.Add(this.txtSysRecommend);
            this.tabPage2.Controls.Add(this.lCreditLimit);
            this.tabPage2.Controls.Add(this.txtCreditLimit);
            this.tabPage2.Controls.Add(this.txtScore);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.rbReject);
            this.tabPage2.Controls.Add(this.rbApprove);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.txtReason);
            this.tabPage2.Controls.Add(this.splitContainer2);
            this.tabPage2.Controls.Add(this.txtAccountNo);
            this.tabPage2.Controls.Add(this.label28);
            this.tabPage2.Controls.Add(this.txtFirstName);
            this.tabPage2.Controls.Add(this.txtCustomerID);
            this.tabPage2.Controls.Add(this.btnComplete);
            this.tabPage2.Controls.Add(this.dtDateProp);
            this.tabPage2.Controls.Add(this.btnAccountDetails);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.txtLastName);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.lCustomerID);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(784, 450);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "Notes";
            // 
            // label43
            // 
            this.label43.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label43.Location = new System.Drawing.Point(355, 51);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(88, 16);
            this.label43.TabIndex = 76;
            this.label43.Text = "Referral Limit ";
            // 
            // txtReferralLimit
            // 
            this.txtReferralLimit.Location = new System.Drawing.Point(355, 67);
            this.txtReferralLimit.MaxLength = 15;
            this.txtReferralLimit.Name = "txtReferralLimit";
            this.txtReferralLimit.ReadOnly = true;
            this.txtReferralLimit.Size = new System.Drawing.Size(88, 20);
            this.txtReferralLimit.TabIndex = 75;
            // 
            // label42
            // 
            this.label42.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label42.Location = new System.Drawing.Point(255, 51);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(88, 16);
            this.label42.TabIndex = 73;
            this.label42.Text = "Credit % Uplift";
            // 
            // lblStoreCardLimit
            // 
            this.lblStoreCardLimit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStoreCardLimit.Location = new System.Drawing.Point(455, 51);
            this.lblStoreCardLimit.Name = "lblStoreCardLimit";
            this.lblStoreCardLimit.Size = new System.Drawing.Size(88, 16);
            this.lblStoreCardLimit.TabIndex = 72;
            this.lblStoreCardLimit.Text = "Store Card Limit";
            this.lblStoreCardLimit.Visible = false;
            // 
            // txtStoreCardLimit
            // 
            this.txtStoreCardLimit.Location = new System.Drawing.Point(455, 67);
            this.txtStoreCardLimit.MaxLength = 15;
            this.txtStoreCardLimit.Name = "txtStoreCardLimit";
            this.txtStoreCardLimit.ReadOnly = true;
            this.txtStoreCardLimit.Size = new System.Drawing.Size(88, 20);
            this.txtStoreCardLimit.TabIndex = 71;
            this.txtStoreCardLimit.Visible = false;
            // 
            // labelband
            // 
            this.labelband.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelband.Location = new System.Drawing.Point(391, 106);
            this.labelband.Name = "labelband";
            this.labelband.Size = new System.Drawing.Size(59, 16);
            this.labelband.TabIndex = 67;
            this.labelband.Text = "Band";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(504, 104);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(128, 16);
            this.label6.TabIndex = 69;
            this.label6.Text = "Reason for change";
            // 
            // cmb_reason
            // 
            this.cmb_reason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_reason.FormattingEnabled = true;
            this.cmb_reason.Location = new System.Drawing.Point(507, 123);
            this.cmb_reason.Name = "cmb_reason";
            this.cmb_reason.Size = new System.Drawing.Size(154, 21);
            this.cmb_reason.TabIndex = 70;
            // 
            // cmb_band
            // 
            this.cmb_band.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_band.FormattingEnabled = true;
            this.cmb_band.Location = new System.Drawing.Point(394, 123);
            this.cmb_band.Name = "cmb_band";
            this.cmb_band.Size = new System.Drawing.Size(56, 21);
            this.cmb_band.TabIndex = 68;
            this.cmb_band.SelectedIndexChanged += new System.EventHandler(this.cmb_band_SelectedIndexChanged);
            // 
            // label25
            // 
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(223, 95);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(92, 29);
            this.label25.TabIndex = 65;
            this.label25.Text = "System Recommendation";
            // 
            // txtSysRecommend
            // 
            this.txtSysRecommend.Location = new System.Drawing.Point(228, 124);
            this.txtSysRecommend.MaxLength = 2;
            this.txtSysRecommend.Name = "txtSysRecommend";
            this.txtSysRecommend.ReadOnly = true;
            this.txtSysRecommend.Size = new System.Drawing.Size(64, 20);
            this.txtSysRecommend.TabIndex = 64;
            // 
            // lCreditLimit
            // 
            this.lCreditLimit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lCreditLimit.Location = new System.Drawing.Point(155, 51);
            this.lCreditLimit.Name = "lCreditLimit";
            this.lCreditLimit.Size = new System.Drawing.Size(88, 16);
            this.lCreditLimit.TabIndex = 63;
            this.lCreditLimit.Text = "Credit Limit";
            this.lCreditLimit.Click += new System.EventHandler(this.lCreditLimit_Click);
            // 
            // txtCreditLimit
            // 
            this.txtCreditLimit.Location = new System.Drawing.Point(155, 67);
            this.txtCreditLimit.MaxLength = 15;
            this.txtCreditLimit.Name = "txtCreditLimit";
            this.txtCreditLimit.ReadOnly = true;
            this.txtCreditLimit.Size = new System.Drawing.Size(88, 20);
            this.txtCreditLimit.TabIndex = 62;
            this.txtCreditLimit.TextChanged += new System.EventHandler(this.txtCreditLimit_Enter);
            // 
            // txtScore
            // 
            this.txtScore.Location = new System.Drawing.Point(316, 124);
            this.txtScore.MaxLength = 15;
            this.txtScore.Name = "txtScore";
            this.txtScore.ReadOnly = true;
            this.txtScore.Size = new System.Drawing.Size(72, 20);
            this.txtScore.TabIndex = 61;
            this.txtScore.Tag = "lCustomerID";
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(312, 106);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 16);
            this.label7.TabIndex = 60;
            this.label7.Text = "Score";
            // 
            // rbReject
            // 
            this.rbReject.Location = new System.Drawing.Point(695, 124);
            this.rbReject.Name = "rbReject";
            this.rbReject.Size = new System.Drawing.Size(65, 24);
            this.rbReject.TabIndex = 59;
            this.rbReject.Text = "Reject";
            this.rbReject.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
            // 
            // rbApprove
            // 
            this.rbApprove.Location = new System.Drawing.Point(695, 100);
            this.rbApprove.Name = "rbApprove";
            this.rbApprove.Size = new System.Drawing.Size(65, 24);
            this.rbApprove.TabIndex = 58;
            this.rbApprove.Text = "Approve";
            this.rbApprove.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 108);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 16);
            this.label1.TabIndex = 57;
            this.label1.Text = "Reason for referral";
            // 
            // txtReason
            // 
            this.txtReason.Location = new System.Drawing.Point(8, 124);
            this.txtReason.Name = "txtReason";
            this.txtReason.ReadOnly = true;
            this.txtReason.Size = new System.Drawing.Size(213, 20);
            this.txtReason.TabIndex = 56;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Location = new System.Drawing.Point(6, 153);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tabControl2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(775, 289);
            this.splitContainer2.SplitterDistance = 497;
            this.splitContainer2.TabIndex = 66;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tab_notes);
            this.tabControl2.Controls.Add(this.tab_ReferralHistory);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(497, 289);
            this.tabControl2.TabIndex = 50;
            // 
            // tab_notes
            // 
            this.tab_notes.Controls.Add(this.splitContainer1);
            this.tab_notes.Location = new System.Drawing.Point(4, 22);
            this.tab_notes.Name = "tab_notes";
            this.tab_notes.Padding = new System.Windows.Forms.Padding(3);
            this.tab_notes.Size = new System.Drawing.Size(489, 263);
            this.tab_notes.TabIndex = 1;
            this.tab_notes.Text = "Referral Notes";
            this.tab_notes.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.rtxtReferralNotes);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rtxtNewReferralNotes);
            this.splitContainer1.Size = new System.Drawing.Size(483, 257);
            this.splitContainer1.SplitterDistance = 169;
            this.splitContainer1.TabIndex = 0;
            // 
            // rtxtReferralNotes
            // 
            this.rtxtReferralNotes.BackColor = System.Drawing.SystemColors.Control;
            this.rtxtReferralNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtReferralNotes.Location = new System.Drawing.Point(0, 0);
            this.rtxtReferralNotes.MaxLength = 1000;
            this.rtxtReferralNotes.Name = "rtxtReferralNotes";
            this.rtxtReferralNotes.ReadOnly = true;
            this.rtxtReferralNotes.Size = new System.Drawing.Size(483, 169);
            this.rtxtReferralNotes.TabIndex = 42;
            this.rtxtReferralNotes.Text = "";
            // 
            // rtxtNewReferralNotes
            // 
            this.rtxtNewReferralNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtNewReferralNotes.Location = new System.Drawing.Point(0, 0);
            this.rtxtNewReferralNotes.MaxLength = 1000;
            this.rtxtNewReferralNotes.Name = "rtxtNewReferralNotes";
            this.rtxtNewReferralNotes.Size = new System.Drawing.Size(483, 84);
            this.rtxtNewReferralNotes.TabIndex = 42;
            this.rtxtNewReferralNotes.Text = "";
            this.rtxtNewReferralNotes.TextChanged += new System.EventHandler(this.rtxtNewReferralNotes_TextChanged);
            // 
            // tab_ReferralHistory
            // 
            this.tab_ReferralHistory.Controls.Add(this.dgv_referralhistory);
            this.tab_ReferralHistory.Location = new System.Drawing.Point(4, 22);
            this.tab_ReferralHistory.Name = "tab_ReferralHistory";
            this.tab_ReferralHistory.Padding = new System.Windows.Forms.Padding(3);
            this.tab_ReferralHistory.Size = new System.Drawing.Size(489, 263);
            this.tab_ReferralHistory.TabIndex = 0;
            this.tab_ReferralHistory.Text = "History";
            this.tab_ReferralHistory.UseVisualStyleBackColor = true;
            // 
            // dgv_referralhistory
            // 
            this.dgv_referralhistory.AllowUserToAddRows = false;
            this.dgv_referralhistory.AllowUserToDeleteRows = false;
            this.dgv_referralhistory.AllowUserToResizeRows = false;
            this.dgv_referralhistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgv_referralhistory.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgv_referralhistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_referralhistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_referralhistory.Location = new System.Drawing.Point(3, 3);
            this.dgv_referralhistory.Name = "dgv_referralhistory";
            this.dgv_referralhistory.ReadOnly = true;
            this.dgv_referralhistory.Size = new System.Drawing.Size(483, 257);
            this.dgv_referralhistory.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.dgPolicyRules);
            this.splitContainer3.Panel1.Controls.Add(this.label27);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.dgReferralAudit);
            this.splitContainer3.Panel2.Controls.Add(this.label26);
            this.splitContainer3.Size = new System.Drawing.Size(274, 289);
            this.splitContainer3.SplitterDistance = 142;
            this.splitContainer3.TabIndex = 0;
            // 
            // dgPolicyRules
            // 
            this.dgPolicyRules.CaptionVisible = false;
            this.dgPolicyRules.DataMember = "";
            this.dgPolicyRules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgPolicyRules.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgPolicyRules.Location = new System.Drawing.Point(0, 16);
            this.dgPolicyRules.Name = "dgPolicyRules";
            this.dgPolicyRules.ReadOnly = true;
            this.dgPolicyRules.RowHeadersVisible = false;
            this.dgPolicyRules.Size = new System.Drawing.Size(274, 126);
            this.dgPolicyRules.TabIndex = 50;
            // 
            // label27
            // 
            this.label27.Dock = System.Windows.Forms.DockStyle.Top;
            this.label27.Location = new System.Drawing.Point(0, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(274, 16);
            this.label27.TabIndex = 49;
            this.label27.Text = "Policy Rules Referred On";
            // 
            // dgReferralAudit
            // 
            this.dgReferralAudit.CaptionVisible = false;
            this.dgReferralAudit.DataMember = "";
            this.dgReferralAudit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgReferralAudit.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgReferralAudit.Location = new System.Drawing.Point(0, 16);
            this.dgReferralAudit.Name = "dgReferralAudit";
            this.dgReferralAudit.ReadOnly = true;
            this.dgReferralAudit.RowHeadersVisible = false;
            this.dgReferralAudit.Size = new System.Drawing.Size(274, 127);
            this.dgReferralAudit.TabIndex = 51;
            // 
            // label26
            // 
            this.label26.Dock = System.Windows.Forms.DockStyle.Top;
            this.label26.Location = new System.Drawing.Point(0, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(274, 16);
            this.label26.TabIndex = 52;
            this.label26.Text = "Referral Audit";
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.Location = new System.Drawing.Point(12, 67);
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.ReadOnly = true;
            this.txtAccountNo.Size = new System.Drawing.Size(112, 20);
            this.txtAccountNo.TabIndex = 38;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(12, 51);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(67, 13);
            this.label28.TabIndex = 37;
            this.label28.Text = "Account No:";
            // 
            // txtFirstName
            // 
            this.txtFirstName.Location = new System.Drawing.Point(155, 29);
            this.txtFirstName.MaxLength = 30;
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.ReadOnly = true;
            this.txtFirstName.Size = new System.Drawing.Size(160, 20);
            this.txtFirstName.TabIndex = 27;
            // 
            // txtCustomerID
            // 
            this.txtCustomerID.Location = new System.Drawing.Point(12, 29);
            this.txtCustomerID.MaxLength = 20;
            this.txtCustomerID.Name = "txtCustomerID";
            this.txtCustomerID.ReadOnly = true;
            this.txtCustomerID.Size = new System.Drawing.Size(112, 20);
            this.txtCustomerID.TabIndex = 36;
            this.txtCustomerID.Tag = "lCustomerID";
            // 
            // btnComplete
            // 
            this.btnComplete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnComplete.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnComplete.ImageIndex = 1;
            this.btnComplete.ImageList = this.menuIcons;
            this.btnComplete.Location = new System.Drawing.Point(724, 63);
            this.btnComplete.Name = "btnComplete";
            this.btnComplete.Size = new System.Drawing.Size(24, 24);
            this.btnComplete.TabIndex = 31;
            this.btnComplete.Click += new System.EventHandler(this.btnComplete_Click);
            // 
            // dtDateProp
            // 
            this.dtDateProp.CustomFormat = "ddd dd MMM yyyy";
            this.dtDateProp.Enabled = false;
            this.dtDateProp.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDateProp.Location = new System.Drawing.Point(612, 29);
            this.dtDateProp.Name = "dtDateProp";
            this.dtDateProp.Size = new System.Drawing.Size(136, 20);
            this.dtDateProp.TabIndex = 35;
            this.dtDateProp.Tag = "";
            this.dtDateProp.Value = new System.DateTime(2002, 5, 21, 0, 0, 0, 0);
            // 
            // btnAccountDetails
            // 
            this.btnAccountDetails.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAccountDetails.Image = ((System.Drawing.Image)(resources.GetObject("btnAccountDetails.Image")));
            this.btnAccountDetails.Location = new System.Drawing.Point(684, 63);
            this.btnAccountDetails.Name = "btnAccountDetails";
            this.btnAccountDetails.Size = new System.Drawing.Size(24, 24);
            this.btnAccountDetails.TabIndex = 32;
            this.btnAccountDetails.Click += new System.EventHandler(this.btnAccountDetails_Click);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(609, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 16);
            this.label4.TabIndex = 34;
            this.label4.Text = "Date of Application:";
            // 
            // txtLastName
            // 
            this.txtLastName.Location = new System.Drawing.Point(348, 29);
            this.txtLastName.MaxLength = 60;
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.ReadOnly = true;
            this.txtLastName.Size = new System.Drawing.Size(248, 20);
            this.txtLastName.TabIndex = 28;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(348, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 16);
            this.label3.TabIndex = 33;
            this.label3.Text = "Last Name:";
            // 
            // lCustomerID
            // 
            this.lCustomerID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lCustomerID.Location = new System.Drawing.Point(12, 13);
            this.lCustomerID.Name = "lCustomerID";
            this.lCustomerID.Size = new System.Drawing.Size(72, 16);
            this.lCustomerID.TabIndex = 29;
            this.lCustomerID.Text = "Customer:";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(155, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 30;
            this.label2.Text = "First Name:";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.groupBox2);
            this.tabPage3.Controls.Add(this.groupBox1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(784, 450);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "Summary";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgLineItems);
            this.groupBox2.Controls.Add(this.tvItems);
            this.groupBox2.Controls.Add(this.txtRFAccts);
            this.groupBox2.Controls.Add(this.label24);
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.txtAcctType);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.txtAgreementTotal);
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.txtFinalInstalment);
            this.groupBox2.Controls.Add(this.txtInstalment);
            this.groupBox2.Controls.Add(this.txtDeposit);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.txtTermsType);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.txtRepaymentPcent);
            this.groupBox2.Location = new System.Drawing.Point(8, 146);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(752, 298);
            this.groupBox2.TabIndex = 66;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Agreement Details";
            // 
            // dgLineItems
            // 
            this.dgLineItems.CaptionText = "Line Items";
            this.dgLineItems.DataMember = "";
            this.dgLineItems.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgLineItems.Location = new System.Drawing.Point(139, 116);
            this.dgLineItems.Name = "dgLineItems";
            this.dgLineItems.ReadOnly = true;
            this.dgLineItems.Size = new System.Drawing.Size(606, 182);
            this.dgLineItems.TabIndex = 105;
            // 
            // tvItems
            // 
            this.tvItems.Indent = 19;
            this.tvItems.ItemHeight = 17;
            this.tvItems.Location = new System.Drawing.Point(12, 116);
            this.tvItems.Name = "tvItems";
            this.tvItems.Size = new System.Drawing.Size(121, 182);
            this.tvItems.TabIndex = 104;
            this.tvItems.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvItems_AfterSelect);
            // 
            // txtRFAccts
            // 
            this.txtRFAccts.Location = new System.Drawing.Point(108, 87);
            this.txtRFAccts.Name = "txtRFAccts";
            this.txtRFAccts.ReadOnly = true;
            this.txtRFAccts.Size = new System.Drawing.Size(55, 20);
            this.txtRFAccts.TabIndex = 103;
            this.txtRFAccts.Text = "0";
            this.txtRFAccts.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label24
            // 
            this.label24.Location = new System.Drawing.Point(105, 71);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(120, 16);
            this.label24.TabIndex = 102;
            this.label24.Text = "No. of RF Accounts:";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(14, 90);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(80, 20);
            this.textBox2.TabIndex = 101;
            this.textBox2.Text = "0";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(11, 71);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(80, 16);
            this.label23.TabIndex = 100;
            this.label23.Text = "Credit Limit:";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(105, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(58, 32);
            this.label10.TabIndex = 99;
            this.label10.Text = "Account Type";
            // 
            // txtAcctType
            // 
            this.txtAcctType.Location = new System.Drawing.Point(108, 48);
            this.txtAcctType.Name = "txtAcctType";
            this.txtAcctType.ReadOnly = true;
            this.txtAcctType.Size = new System.Drawing.Size(48, 20);
            this.txtAcctType.TabIndex = 98;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(634, 32);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(88, 16);
            this.label11.TabIndex = 96;
            this.label11.Text = "Agreement Total";
            // 
            // txtAgreementTotal
            // 
            this.txtAgreementTotal.Location = new System.Drawing.Point(635, 48);
            this.txtAgreementTotal.Name = "txtAgreementTotal";
            this.txtAgreementTotal.ReadOnly = true;
            this.txtAgreementTotal.Size = new System.Drawing.Size(80, 20);
            this.txtAgreementTotal.TabIndex = 97;
            this.txtAgreementTotal.TabStop = false;
            this.txtAgreementTotal.Text = "0";
            this.txtAgreementTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(538, 32);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(102, 16);
            this.label21.TabIndex = 94;
            this.label21.Text = "Final Instalment";
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(444, 32);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(64, 16);
            this.label20.TabIndex = 93;
            this.label20.Text = "Instalment";
            // 
            // txtFinalInstalment
            // 
            this.txtFinalInstalment.Location = new System.Drawing.Point(541, 48);
            this.txtFinalInstalment.Name = "txtFinalInstalment";
            this.txtFinalInstalment.ReadOnly = true;
            this.txtFinalInstalment.Size = new System.Drawing.Size(80, 20);
            this.txtFinalInstalment.TabIndex = 95;
            this.txtFinalInstalment.TabStop = false;
            this.txtFinalInstalment.Text = "0";
            this.txtFinalInstalment.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtInstalment
            // 
            this.txtInstalment.Location = new System.Drawing.Point(446, 48);
            this.txtInstalment.Name = "txtInstalment";
            this.txtInstalment.ReadOnly = true;
            this.txtInstalment.Size = new System.Drawing.Size(80, 20);
            this.txtInstalment.TabIndex = 92;
            this.txtInstalment.TabStop = false;
            this.txtInstalment.Text = "0";
            this.txtInstalment.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtDeposit
            // 
            this.txtDeposit.Location = new System.Drawing.Point(356, 48);
            this.txtDeposit.Name = "txtDeposit";
            this.txtDeposit.ReadOnly = true;
            this.txtDeposit.Size = new System.Drawing.Size(80, 20);
            this.txtDeposit.TabIndex = 91;
            this.txtDeposit.Text = "0";
            this.txtDeposit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(353, 32);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(48, 16);
            this.label18.TabIndex = 90;
            this.label18.Text = "Deposit:";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(170, 32);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(72, 16);
            this.label12.TabIndex = 89;
            this.label12.Text = "Terms Type";
            // 
            // txtTermsType
            // 
            this.txtTermsType.Location = new System.Drawing.Point(173, 48);
            this.txtTermsType.Name = "txtTermsType";
            this.txtTermsType.ReadOnly = true;
            this.txtTermsType.Size = new System.Drawing.Size(174, 20);
            this.txtTermsType.TabIndex = 88;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(11, 18);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(96, 27);
            this.label13.TabIndex = 87;
            this.label13.Text = "Repayment as % of net Disposable Income";
            // 
            // txtRepaymentPcent
            // 
            this.txtRepaymentPcent.Location = new System.Drawing.Point(14, 48);
            this.txtRepaymentPcent.Name = "txtRepaymentPcent";
            this.txtRepaymentPcent.ReadOnly = true;
            this.txtRepaymentPcent.Size = new System.Drawing.Size(48, 20);
            this.txtRepaymentPcent.TabIndex = 86;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtDateInCurrentEmp);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtAge);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.txtIncomeAfterTax);
            this.groupBox1.Controls.Add(this.label32);
            this.groupBox1.Controls.Add(this.txtMaritalStatus);
            this.groupBox1.Controls.Add(this.txtOccupation);
            this.groupBox1.Controls.Add(this.label29);
            this.groupBox1.Controls.Add(this.label31);
            this.groupBox1.Controls.Add(this.txtResidentialStatus);
            this.groupBox1.Controls.Add(this.txtExpenses);
            this.groupBox1.Controls.Add(this.label30);
            this.groupBox1.Controls.Add(this.dtDateInCurrentAddress);
            this.groupBox1.Location = new System.Drawing.Point(8, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(752, 130);
            this.groupBox1.TabIndex = 65;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Personal Details";
            // 
            // dtDateInCurrentEmp
            // 
            this.dtDateInCurrentEmp.DateFrom = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtDateInCurrentEmp.Enabled = false;
            this.dtDateInCurrentEmp.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtDateInCurrentEmp.Label = "Curr. Emp. started:";
            this.dtDateInCurrentEmp.LinkedBias = false;
            this.dtDateInCurrentEmp.LinkedComboBox = null;
            this.dtDateInCurrentEmp.LinkedDatePicker = null;
            this.dtDateInCurrentEmp.LinkedLabel = null;
            this.dtDateInCurrentEmp.Location = new System.Drawing.Point(291, 61);
            this.dtDateInCurrentEmp.Months = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtDateInCurrentEmp.Name = "dtDateInCurrentEmp";
            this.dtDateInCurrentEmp.Size = new System.Drawing.Size(256, 56);
            this.dtDateInCurrentEmp.TabIndex = 58;
            this.dtDateInCurrentEmp.Tag = "dtCurrEmpStart1";
            this.dtDateInCurrentEmp.Value = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtDateInCurrentEmp.Years = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(578, 17);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(95, 15);
            this.label8.TabIndex = 52;
            this.label8.Text = "Income After Tax";
            // 
            // txtAge
            // 
            this.txtAge.Location = new System.Drawing.Point(151, 34);
            this.txtAge.Name = "txtAge";
            this.txtAge.ReadOnly = true;
            this.txtAge.Size = new System.Drawing.Size(40, 20);
            this.txtAge.TabIndex = 49;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(9, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(136, 16);
            this.label5.TabIndex = 64;
            this.label5.Text = "System Recommendation:";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(151, 18);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 16);
            this.label9.TabIndex = 50;
            this.label9.Text = "Age";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(14, 34);
            this.textBox1.MaxLength = 2;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(48, 20);
            this.textBox1.TabIndex = 63;
            // 
            // txtIncomeAfterTax
            // 
            this.txtIncomeAfterTax.Location = new System.Drawing.Point(580, 33);
            this.txtIncomeAfterTax.Name = "txtIncomeAfterTax";
            this.txtIncomeAfterTax.ReadOnly = true;
            this.txtIncomeAfterTax.Size = new System.Drawing.Size(88, 20);
            this.txtIncomeAfterTax.TabIndex = 51;
            this.txtIncomeAfterTax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label32
            // 
            this.label32.Location = new System.Drawing.Point(392, 18);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(95, 16);
            this.label32.TabIndex = 62;
            this.label32.Text = "Occupation";
            // 
            // txtMaritalStatus
            // 
            this.txtMaritalStatus.Location = new System.Drawing.Point(197, 34);
            this.txtMaritalStatus.Name = "txtMaritalStatus";
            this.txtMaritalStatus.ReadOnly = true;
            this.txtMaritalStatus.Size = new System.Drawing.Size(88, 20);
            this.txtMaritalStatus.TabIndex = 53;
            // 
            // txtOccupation
            // 
            this.txtOccupation.Location = new System.Drawing.Point(392, 34);
            this.txtOccupation.Name = "txtOccupation";
            this.txtOccupation.ReadOnly = true;
            this.txtOccupation.Size = new System.Drawing.Size(88, 20);
            this.txtOccupation.TabIndex = 61;
            // 
            // label29
            // 
            this.label29.Location = new System.Drawing.Point(197, 18);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(95, 16);
            this.label29.TabIndex = 54;
            this.label29.Text = "Marital Status";
            // 
            // label31
            // 
            this.label31.Location = new System.Drawing.Point(484, 18);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(95, 16);
            this.label31.TabIndex = 60;
            this.label31.Text = "Expenses";
            // 
            // txtResidentialStatus
            // 
            this.txtResidentialStatus.Location = new System.Drawing.Point(291, 34);
            this.txtResidentialStatus.Name = "txtResidentialStatus";
            this.txtResidentialStatus.ReadOnly = true;
            this.txtResidentialStatus.Size = new System.Drawing.Size(88, 20);
            this.txtResidentialStatus.TabIndex = 55;
            // 
            // txtExpenses
            // 
            this.txtExpenses.Location = new System.Drawing.Point(486, 34);
            this.txtExpenses.Name = "txtExpenses";
            this.txtExpenses.ReadOnly = true;
            this.txtExpenses.Size = new System.Drawing.Size(88, 20);
            this.txtExpenses.TabIndex = 59;
            this.txtExpenses.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label30
            // 
            this.label30.Location = new System.Drawing.Point(291, 18);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(113, 16);
            this.label30.TabIndex = 56;
            this.label30.Text = "Residential Status";
            // 
            // dtDateInCurrentAddress
            // 
            this.dtDateInCurrentAddress.DateFrom = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtDateInCurrentAddress.Enabled = false;
            this.dtDateInCurrentAddress.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtDateInCurrentAddress.Label = "Date In Curr Address:";
            this.dtDateInCurrentAddress.LinkedBias = true;
            this.dtDateInCurrentAddress.LinkedComboBox = null;
            this.dtDateInCurrentAddress.LinkedDatePicker = null;
            this.dtDateInCurrentAddress.LinkedLabel = null;
            this.dtDateInCurrentAddress.Location = new System.Drawing.Point(6, 61);
            this.dtDateInCurrentAddress.Months = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtDateInCurrentAddress.Name = "dtDateInCurrentAddress";
            this.dtDateInCurrentAddress.Size = new System.Drawing.Size(256, 56);
            this.dtDateInCurrentAddress.TabIndex = 57;
            this.dtDateInCurrentAddress.Tag = "dtDateInCurrentAddress1";
            this.dtDateInCurrentAddress.Value = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtDateInCurrentAddress.Years = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.label22);
            this.tabPage4.Controls.Add(this.txtValueOfAllArrears);
            this.tabPage4.Controls.Add(this.label19);
            this.tabPage4.Controls.Add(this.txtNoOfReturnedCheques);
            this.tabPage4.Controls.Add(this.label17);
            this.tabPage4.Controls.Add(this.txtRepaymentPcentCurrent);
            this.tabPage4.Controls.Add(this.label16);
            this.tabPage4.Controls.Add(this.txtCashAccountsSettled);
            this.tabPage4.Controls.Add(this.label15);
            this.tabPage4.Controls.Add(this.txtFeesAndInterest);
            this.tabPage4.Controls.Add(this.label14);
            this.tabPage4.Controls.Add(this.txtLargestAgreement);
            this.tabPage4.Controls.Add(this.label33);
            this.tabPage4.Controls.Add(this.label34);
            this.tabPage4.Controls.Add(this.txtLongestAgreement);
            this.tabPage4.Controls.Add(this.label35);
            this.tabPage4.Controls.Add(this.txtNoAccountsInArrears);
            this.tabPage4.Controls.Add(this.label36);
            this.tabPage4.Controls.Add(this.txtTotalInstalmentValue);
            this.tabPage4.Controls.Add(this.label37);
            this.tabPage4.Controls.Add(this.txtOutstandingBalance);
            this.tabPage4.Controls.Add(this.label38);
            this.tabPage4.Controls.Add(this.txtWorstSettledStatus);
            this.tabPage4.Controls.Add(this.label39);
            this.tabPage4.Controls.Add(this.txtWorstCurrentStatus);
            this.tabPage4.Controls.Add(this.label40);
            this.tabPage4.Controls.Add(this.txtNoOfSettled);
            this.tabPage4.Controls.Add(this.label41);
            this.tabPage4.Controls.Add(this.txtNoOfCurrent);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(784, 450);
            this.tabPage4.TabIndex = 2;
            this.tabPage4.Text = "Account History";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // label22
            // 
            this.label22.Location = new System.Drawing.Point(31, 51);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(88, 24);
            this.label22.TabIndex = 92;
            this.label22.Text = "Value of all arrears";
            // 
            // txtValueOfAllArrears
            // 
            this.txtValueOfAllArrears.Location = new System.Drawing.Point(34, 83);
            this.txtValueOfAllArrears.Name = "txtValueOfAllArrears";
            this.txtValueOfAllArrears.ReadOnly = true;
            this.txtValueOfAllArrears.Size = new System.Drawing.Size(80, 20);
            this.txtValueOfAllArrears.TabIndex = 93;
            this.txtValueOfAllArrears.TabStop = false;
            this.txtValueOfAllArrears.Text = "0";
            this.txtValueOfAllArrears.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(31, 251);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(88, 24);
            this.label19.TabIndex = 90;
            this.label19.Text = "No. of returned cheques";
            // 
            // txtNoOfReturnedCheques
            // 
            this.txtNoOfReturnedCheques.Location = new System.Drawing.Point(34, 283);
            this.txtNoOfReturnedCheques.Name = "txtNoOfReturnedCheques";
            this.txtNoOfReturnedCheques.ReadOnly = true;
            this.txtNoOfReturnedCheques.Size = new System.Drawing.Size(64, 20);
            this.txtNoOfReturnedCheques.TabIndex = 91;
            this.txtNoOfReturnedCheques.TabStop = false;
            this.txtNoOfReturnedCheques.Text = "0";
            this.txtNoOfReturnedCheques.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(567, 219);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(88, 56);
            this.label17.TabIndex = 88;
            this.label17.Text = "Repayment as % of net disposable for current accounts";
            // 
            // txtRepaymentPcentCurrent
            // 
            this.txtRepaymentPcentCurrent.Location = new System.Drawing.Point(570, 283);
            this.txtRepaymentPcentCurrent.Name = "txtRepaymentPcentCurrent";
            this.txtRepaymentPcentCurrent.ReadOnly = true;
            this.txtRepaymentPcentCurrent.Size = new System.Drawing.Size(80, 20);
            this.txtRepaymentPcentCurrent.TabIndex = 89;
            this.txtRepaymentPcentCurrent.TabStop = false;
            this.txtRepaymentPcentCurrent.Text = "0";
            this.txtRepaymentPcentCurrent.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(426, 147);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(88, 24);
            this.label16.TabIndex = 86;
            this.label16.Text = "No. of cash accounts settled";
            // 
            // txtCashAccountsSettled
            // 
            this.txtCashAccountsSettled.Location = new System.Drawing.Point(429, 179);
            this.txtCashAccountsSettled.Name = "txtCashAccountsSettled";
            this.txtCashAccountsSettled.ReadOnly = true;
            this.txtCashAccountsSettled.Size = new System.Drawing.Size(56, 20);
            this.txtCashAccountsSettled.TabIndex = 87;
            this.txtCashAccountsSettled.TabStop = false;
            this.txtCashAccountsSettled.Text = "0";
            this.txtCashAccountsSettled.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(426, 51);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(104, 24);
            this.label15.TabIndex = 84;
            this.label15.Text = "Fees && Interest since last acct opened";
            // 
            // txtFeesAndInterest
            // 
            this.txtFeesAndInterest.Location = new System.Drawing.Point(429, 83);
            this.txtFeesAndInterest.Name = "txtFeesAndInterest";
            this.txtFeesAndInterest.ReadOnly = true;
            this.txtFeesAndInterest.Size = new System.Drawing.Size(80, 20);
            this.txtFeesAndInterest.TabIndex = 85;
            this.txtFeesAndInterest.TabStop = false;
            this.txtFeesAndInterest.Text = "0";
            this.txtFeesAndInterest.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(290, 51);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(88, 24);
            this.label14.TabIndex = 82;
            this.label14.Text = "Largest Agreement";
            // 
            // txtLargestAgreement
            // 
            this.txtLargestAgreement.Location = new System.Drawing.Point(293, 83);
            this.txtLargestAgreement.Name = "txtLargestAgreement";
            this.txtLargestAgreement.ReadOnly = true;
            this.txtLargestAgreement.Size = new System.Drawing.Size(80, 20);
            this.txtLargestAgreement.TabIndex = 83;
            this.txtLargestAgreement.TabStop = false;
            this.txtLargestAgreement.Text = "0";
            this.txtLargestAgreement.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label33
            // 
            this.label33.Location = new System.Drawing.Point(475, 287);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(48, 16);
            this.label33.TabIndex = 81;
            this.label33.Text = "months";
            // 
            // label34
            // 
            this.label34.Location = new System.Drawing.Point(426, 251);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(72, 24);
            this.label34.TabIndex = 79;
            this.label34.Text = "Longest Agreement";
            // 
            // txtLongestAgreement
            // 
            this.txtLongestAgreement.Location = new System.Drawing.Point(429, 283);
            this.txtLongestAgreement.Name = "txtLongestAgreement";
            this.txtLongestAgreement.ReadOnly = true;
            this.txtLongestAgreement.Size = new System.Drawing.Size(40, 20);
            this.txtLongestAgreement.TabIndex = 80;
            this.txtLongestAgreement.TabStop = false;
            this.txtLongestAgreement.Text = "0";
            this.txtLongestAgreement.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label35
            // 
            this.label35.Location = new System.Drawing.Point(290, 147);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(88, 24);
            this.label35.TabIndex = 77;
            this.label35.Text = "No. of accounts in arrears";
            // 
            // txtNoAccountsInArrears
            // 
            this.txtNoAccountsInArrears.Location = new System.Drawing.Point(293, 179);
            this.txtNoAccountsInArrears.Name = "txtNoAccountsInArrears";
            this.txtNoAccountsInArrears.ReadOnly = true;
            this.txtNoAccountsInArrears.Size = new System.Drawing.Size(80, 20);
            this.txtNoAccountsInArrears.TabIndex = 78;
            this.txtNoAccountsInArrears.TabStop = false;
            this.txtNoAccountsInArrears.Text = "0";
            this.txtNoAccountsInArrears.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label36
            // 
            this.label36.Location = new System.Drawing.Point(567, 51);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(88, 24);
            this.label36.TabIndex = 75;
            this.label36.Text = "Total Instalment value of current";
            // 
            // txtTotalInstalmentValue
            // 
            this.txtTotalInstalmentValue.Location = new System.Drawing.Point(570, 83);
            this.txtTotalInstalmentValue.Name = "txtTotalInstalmentValue";
            this.txtTotalInstalmentValue.ReadOnly = true;
            this.txtTotalInstalmentValue.Size = new System.Drawing.Size(80, 20);
            this.txtTotalInstalmentValue.TabIndex = 76;
            this.txtTotalInstalmentValue.TabStop = false;
            this.txtTotalInstalmentValue.Text = "0";
            this.txtTotalInstalmentValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label37
            // 
            this.label37.Location = new System.Drawing.Point(159, 51);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(88, 24);
            this.label37.TabIndex = 73;
            this.label37.Text = "Outstanding Balance";
            // 
            // txtOutstandingBalance
            // 
            this.txtOutstandingBalance.Location = new System.Drawing.Point(162, 83);
            this.txtOutstandingBalance.Name = "txtOutstandingBalance";
            this.txtOutstandingBalance.ReadOnly = true;
            this.txtOutstandingBalance.Size = new System.Drawing.Size(80, 20);
            this.txtOutstandingBalance.TabIndex = 74;
            this.txtOutstandingBalance.TabStop = false;
            this.txtOutstandingBalance.Text = "0";
            this.txtOutstandingBalance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label38
            // 
            this.label38.Location = new System.Drawing.Point(290, 251);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(88, 24);
            this.label38.TabIndex = 71;
            this.label38.Text = "Worst settled status ever";
            // 
            // txtWorstSettledStatus
            // 
            this.txtWorstSettledStatus.Location = new System.Drawing.Point(293, 283);
            this.txtWorstSettledStatus.Name = "txtWorstSettledStatus";
            this.txtWorstSettledStatus.ReadOnly = true;
            this.txtWorstSettledStatus.Size = new System.Drawing.Size(56, 20);
            this.txtWorstSettledStatus.TabIndex = 72;
            this.txtWorstSettledStatus.TabStop = false;
            // 
            // label39
            // 
            this.label39.Location = new System.Drawing.Point(159, 251);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(88, 24);
            this.label39.TabIndex = 69;
            this.label39.Text = "Worst current status ever";
            // 
            // txtWorstCurrentStatus
            // 
            this.txtWorstCurrentStatus.Location = new System.Drawing.Point(162, 283);
            this.txtWorstCurrentStatus.Name = "txtWorstCurrentStatus";
            this.txtWorstCurrentStatus.ReadOnly = true;
            this.txtWorstCurrentStatus.Size = new System.Drawing.Size(55, 20);
            this.txtWorstCurrentStatus.TabIndex = 70;
            this.txtWorstCurrentStatus.TabStop = false;
            // 
            // label40
            // 
            this.label40.Location = new System.Drawing.Point(159, 147);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(55, 24);
            this.label40.TabIndex = 67;
            this.label40.Text = "No. of Settled";
            // 
            // txtNoOfSettled
            // 
            this.txtNoOfSettled.Location = new System.Drawing.Point(162, 179);
            this.txtNoOfSettled.Name = "txtNoOfSettled";
            this.txtNoOfSettled.ReadOnly = true;
            this.txtNoOfSettled.Size = new System.Drawing.Size(55, 20);
            this.txtNoOfSettled.TabIndex = 68;
            this.txtNoOfSettled.TabStop = false;
            this.txtNoOfSettled.Text = "0";
            this.txtNoOfSettled.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label41
            // 
            this.label41.Location = new System.Drawing.Point(31, 147);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(55, 24);
            this.label41.TabIndex = 65;
            this.label41.Text = "No. of Current";
            // 
            // txtNoOfCurrent
            // 
            this.txtNoOfCurrent.Location = new System.Drawing.Point(34, 179);
            this.txtNoOfCurrent.Name = "txtNoOfCurrent";
            this.txtNoOfCurrent.ReadOnly = true;
            this.txtNoOfCurrent.Size = new System.Drawing.Size(55, 20);
            this.txtNoOfCurrent.TabIndex = 66;
            this.txtNoOfCurrent.TabStop = false;
            this.txtNoOfCurrent.Text = "0";
            this.txtNoOfCurrent.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblMmi
            // 
            this.lblMmi.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMmi.Location = new System.Drawing.Point(555, 51);
            this.lblMmi.Name = "lblMmi";
            this.lblMmi.Size = new System.Drawing.Size(125, 16);
            this.lblMmi.TabIndex = 78;
            this.lblMmi.Text = "Max. Monthly Instalment";
            this.lblMmi.Visible = false;
            // 
            // txtMmi
            // 
            this.txtMmi.Location = new System.Drawing.Point(555, 67);
            this.txtMmi.MaxLength = 15;
            this.txtMmi.Name = "txtMmi";
            this.txtMmi.ReadOnly = true;
            this.txtMmi.Size = new System.Drawing.Size(88, 20);
            this.txtMmi.TabIndex = 77;
            this.txtMmi.Visible = false;
            // 
            // Referral
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 476);
            this.Controls.Add(this.tabControl1);
            this.Name = "Referral";
            this.Text = "Referral Processing";
            this.Load += new System.EventHandler(this.Referral_Load);
            this.Enter += new System.EventHandler(this.Referral_Enter);
            this.Leave += new System.EventHandler(this.Referral_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tab_notes.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tab_ReferralHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_referralhistory)).EndInit();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgPolicyRules)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgReferralAudit)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgLineItems)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private void Referral_Enter(object sender, System.EventArgs e)
        {
            ((MainForm)this.FormRoot).tbSanction.CustomerScreen = CustomerScreen;
            ((MainForm)this.FormRoot).tbSanction.Settled = false;
            ((MainForm)this.FormRoot).tbSanction.Load(this.allowConversionToHP, this.CustomerID, this.DateProp, this.AccountNo, this.AccountType, this.ScreenMode);
            CurrentStatus = ((MainForm)this.FormRoot).tbSanction.CurrentStatus;
            //((MainForm)this.FormRoot).tbSanction.SetCurrentStage(SS.R);
            ((MainForm)this.FormRoot).tbSanction.Visible = true;
            ReadOnly = ((MainForm)this.FormRoot).tbSanction.ReadOnly(SS.R);
            SetReadOnly();
        }

        private void Referral_Leave(object sender, System.EventArgs e)
        {
            // when an account is approved sanction items dissapear
            if (!_preventSanctionStatusHide)
            {
                ((MainForm)this.FormRoot).tbSanction.Visible = false;

            }
            //else
            //{
            //    // make sure we dont prevent it hiding next time around
            //    _preventSanctionStatusHide = false;
            //}
        }

        public override bool ConfirmClose()
        {
            bool status = true;
            try
            {
                Function = "ConfirmClose()";
                Wait();
                AccountManager.UnlockAccount(this.AccountNo, Credential.UserId, out Error);
                if (Error.Length > 0)
                {
                    status = false;
                    ShowError(Error);
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
            return status;
        }

        private void Referral_Load(object sender, System.EventArgs e)
        {
            toolTip1.SetToolTip(this.btnAccountDetails, GetResource("TT_ACCOUNTDETAILS"));
            toolTip1.SetToolTip(this.btnComplete, GetResource("TT_COMPLETE"));
            //toolTip1.SetToolTip(this.btnSave, GetResource("TT_SAVE"));
            try
            {
                Wait();
                if (LockAccount())
                {
                    DataSet ds = CreditManager.GetReferralData(this.CustomerID,
                        this.AccountNo,
                        this.DateProp,
                        Config.CountryCode,
                        out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            if (dt.TableName == TN.ScoringDetails)
                                dtScoringDetails = dt;

                            if (dt.TableName == TN.ReferralData)
                            {
                                foreach (DataRow r in dt.Rows)
                                {
                                    propresult = (string)r[CN.PropResult];
                                    txtCustomerID.Text = (string)r[CN.CustomerID];
                                    txtFirstName.Text = (string)r[CN.FirstName];
                                    txtLastName.Text = (string)r[CN.LastName];
                                    if (r[CN.PropNotes] != DBNull.Value)
                                    {
                                        rtxtReferralNotes.Text = (string)r[CN.PropNotes];
                                    }
                                    else
                                    {
                                        rtxtReferralNotes.Text = "";
                                    }

                                    rtxtNewReferralNotes.Text = "";
                                    txtReason.Text = (string)r[CN.Reason] + " - " + (string)r[CN.CodeDescription];
                                    rbApprove.Checked = propresult == "A" ? true : false;
                                    rbReject.Checked = propresult == "X" ? true : false;
                                    txtScore.Text = Convert.ToString(r[CN.Score]);
                                    txtCreditLimit.Text = ((decimal)r[CN.RFCreditLimit]).ToString(DecimalPlaces);
                                    Decimal percentUplift = 0.0m;
                                    if (r["CreditPercentUplift"] != DBNull.Value)
                                        percentUplift = Convert.ToDecimal(r["CreditPercentUplift"]);

                                    Decimal referralLimit = Convert.ToDecimal(Country[CountryParameterNames.MaxSpendLimitRefer]);
                                    referralLimit = referralLimit + percentUplift * referralLimit / 100.0m;

                                    txtReferralLimit.Text = Convert.ToString(referralLimit);
                                    txtPercentUplift.Text = Convert.ToString(percentUplift);
                                    

                                    //aahh
                                    if (AccountType == "T")
                                    {
                                        //txtCreditLimit.Text = ((decimal)r[CN.RFCreditLimit] * (decimal)Country[CountryParameterNames.StoreCardPercent] / 100.0M).ToString(DecimalPlaces);
                                        //lCreditLimit.Text = "Store Card Limit";
                                        lblStoreCardLimit.Visible = true;        //IP - 13/12/10 - Store Card
                                        txtStoreCardLimit.Visible = true;       //IP - 13/12/10 - Store Card
                                        txtStoreCardLimit.Text = ((decimal)r[CN.RFCreditLimit] * (decimal)Country[CountryParameterNames.StoreCardPercent] / 100.0M).ToString(DecimalPlaces); //IP - 10/12/10 - Store Card
                                    }
                                    else
                                    {
                                        txtMmi.Left = txtReferralLimit.Right + 12;
                                        lblMmi.Left = txtMmi.Left;
                                    }
                                    txtSysRecommend.Text = (string)r[CN.SysRecommend];

                                    if(Convert.ToBoolean(Country[CountryParameterNames.EnableMMI]))
                                    {
                                        lblMmi.Visible = true;
                                        txtMmi.Visible = true;
                                        txtMmi.Text = ((decimal)r[CN.MmiLimit]).ToString(DecimalPlaces);
                                    }
                                }
                            }

                            if (dt.TableName == TN.ReferralAudit)
                            {
                                // Cleared by history
                                dgReferralAudit.DataSource = dt.DefaultView;
                                dgReferralAudit.TableStyles.Clear();

                                DataGridTableStyle tabStyle = new DataGridTableStyle();
                                tabStyle.MappingName = dt.TableName;
                                AddColumnStyle(CN.ClearedBy, tabStyle, 60, true, GetResource("T_CLEAREDBY"), "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.DateCleared, tabStyle, 100, true, GetResource("T_DATE"), "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.UWResult, tabStyle, 40, true, GetResource("T_RESULT"), "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.UWName, tabStyle, 150, true, GetResource("T_NAME"), "", HorizontalAlignment.Left);
                                dgReferralAudit.TableStyles.Add(tabStyle);
                            }

                            //if(dt.TableName == TN.CreditBureau)
                            //{
                            //    foreach(DataRow r in dt.Rows)
                            //    {
                            //        this.txtDaysSinceLitigation.Text = ((short)r[CN.LawsuitTimeSinceLast]).ToString();
                            //        this.txtDaysSinceBankruptcy.Text = ((short)r[CN.BankruptciesTimeSinceLast]).ToString();
                            //        this.txtLitigationNo.Text = ((short)r[CN.Lawsuits]).ToString();
                            //        this.txtLitigationNo12.Text = ((short)r[CN.Lawsuits12Months]).ToString();
                            //        this.txtLitigationNo24.Text = ((short)r[CN.Lawsuits24Months]).ToString();
                            //        this.txtLitigationAverage.Text = ((decimal)r[CN.LawsuitsAvgValue]).ToString(DecimalPlaces);
                            //        this.txtLitigationTotal.Text = ((decimal)r[CN.LawsuitsTotalValue]).ToString(DecimalPlaces);
                            //        this.txtBankruptcyNo.Text = ((short)r[CN.Bankruptcies]).ToString();
                            //        this.txtBankruptcyNo12.Text = ((short)r[CN.Bankruptcies12Months]).ToString();
                            //        this.txtBankruptcyNo24.Text = ((short)r[CN.Bankruptcies24Months]).ToString();
                            //        this.txtBankruptcyAverage.Text = ((decimal)r[CN.BankruptciesAvgValue]).ToString(DecimalPlaces);
                            //        this.txtBankruptcyTotal.Text = ((decimal)r[CN.BankruptciesTotalValue]).ToString(DecimalPlaces);
                            //        //CR 843 Removed Short Conversion
                            //        this.txtPrevNo.Text = r[CN.PreviousEnquiries].ToString();
                            //        this.txtPrevTotal.Text = ((decimal)r[CN.PreviousEnquiriesTotalValue]).ToString(DecimalPlaces);
                            //        this.txtPrevAverage.Text = ((decimal)r[CN.PreviousEnquiriesAvgValue]).ToString(DecimalPlaces);
                            //        //CR 843 Removed Short Conversion
                            //        this.txtPrevNo12.Text = r[CN.PreviousEnquiries12Months].ToString();
                            //        this.txtPrevAverage12.Text = ((decimal)r[CN.PreviousEnquiriesAvgValue12Months]).ToString(DecimalPlaces);
                            //        this.html.LoadDocument((string)r[CN.ResponseXML]);
                            //        //CR 843 Added html2
                            //        this.html2.LoadDocument((string)r[CN.ResponseXML2]); 
                            //    }
                            //    this.txtDaysSinceLitigation.BackColor = SystemColors.Window;
                            //    this.txtDaysSinceBankruptcy.BackColor = SystemColors.Window;
                            //    this.txtLitigationNo.BackColor = SystemColors.Window;
                            //    this.txtLitigationNo12.BackColor = SystemColors.Window;
                            //    this.txtLitigationNo24.BackColor = SystemColors.Window;
                            //    this.txtLitigationAverage.BackColor = SystemColors.Window;
                            //    this.txtLitigationTotal.BackColor = SystemColors.Window;
                            //    this.txtBankruptcyNo.BackColor = SystemColors.Window;
                            //    this.txtBankruptcyNo12.BackColor = SystemColors.Window;
                            //    this.txtBankruptcyNo24.BackColor = SystemColors.Window;
                            //    this.txtBankruptcyAverage.BackColor = SystemColors.Window;
                            //    this.txtBankruptcyTotal.BackColor = SystemColors.Window;
                            //    this.txtPrevNo.BackColor = SystemColors.Window;
                            //    this.txtPrevTotal.BackColor = SystemColors.Window;
                            //    this.txtPrevAverage.BackColor = SystemColors.Window;
                            //    this.txtPrevNo12.BackColor = SystemColors.Window;
                            //    this.txtPrevAverage12.BackColor = SystemColors.Window;
                            //}

                            //if(dt.TableName == TN.CreditBureauDefaults)
                            //{
                            //    dgPaymentDefaults.DataSource = dt.DefaultView;
                            //    dgPaymentDefaults.TableStyles.Clear();

                            //    DataGridTableStyle tabStyle = new DataGridTableStyle();
                            //    tabStyle.MappingName = dt.TableName;
                            //    AddColumnStyle(CN.CodeDescription, tabStyle, 100, true, GetResource("T_DESCRIPTION"), "", HorizontalAlignment.Left);
                            //    AddColumnStyle(CN.Defaults, tabStyle, 80, true, GetResource("T_DEFAULTS"), "", HorizontalAlignment.Left);
                            //    AddColumnStyle(CN.DefaultsBalance, tabStyle, 80, true, GetResource("T_DEFAULTSBAL"), DecimalPlaces, HorizontalAlignment.Right);
                            //    AddColumnStyle(CN.DefaultsExMotor, tabStyle, 80, true, GetResource("T_DEFAULTSEXMOTOR"), "", HorizontalAlignment.Left);
                            //    AddColumnStyle(CN.DefaultsExMotorBalance, tabStyle, 80, true, GetResource("T_DEFAULTSEXMOTORBAL"), DecimalPlaces, HorizontalAlignment.Right);
                            //    dgPaymentDefaults.TableStyles.Add(tabStyle);
                            //}

                            if (dt.TableName == TN.ReferralRules)
                            {
                                dgPolicyRules.DataSource = dt.DefaultView;
                                dgPolicyRules.TableStyles.Clear();

                                DataGridTableStyle tabStyle = new DataGridTableStyle();
                                tabStyle.MappingName = dt.TableName;
                                AddColumnStyle(CN.CodeDescription, tabStyle, 300, true, GetResource("T_DESCRIPTION"), "", HorizontalAlignment.Left);
                                dgPolicyRules.TableStyles.Add(tabStyle);
                            }
                        }
                    }

                    //decimal bureauCode = (decimal)Country[CountryParameterNames.TransactEnabled];

                    //bool contactBayCorp = (bureauCode == 2 || bureauCode == 4);
                    //bool contactDPGroup = (bureauCode == 3 || bureauCode == 4);

                    //if (!contactBayCorp)
                    //{
                    //    //CR 843 Renamed tpBureau1 and added tpBureau2
                    //    if (tcReferral.TabPages.Contains(tpBureau1))
                    //        tcReferral.TabPages.Remove(tpBureau1); //CR 843 [PC]
                    //}

                    //if (!contactDPGroup)
                    //{
                    //    //CR 843 Renamed tpBureau1 and added tpBureau2
                    //    if (tcReferral.TabPages.Contains(tpBureau2))
                    //        tcReferral.TabPages.Remove(tpBureau2); //CR 843 [PC]
                    //}

                    //if (!contactBayCorp && !contactDPGroup)
                    //{
                    //    if (tcReferral.TabPages.Contains(tpBureauSummary)) 
                    //        tcReferral.TabPages.Remove(tpBureauSummary);
                    //}

                    //if(tpSummary.Control == null)
                    //    tpSummary.Control = new ReferralSummary();
                    //((ReferralSummary)tpSummary.Control).LoadDetails(FormRoot, this, AccountNo, CustomerID, DateProp, AccountType);

                    ValidateControl(null, null);
                }

                LoadScoreHistforCustomer();
                loaded = false;

                if ((bool)Country[CountryParameterNames.TermsTypeBandEnabled])
                {
                    SetBandDetails();
                }
                else
                {
                    cmb_band.Enabled = false;
                    labelband.Enabled = false;
                }
                loaded = true;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                this.CalcCharsAvailable();
                StopWait();
            }
        }

        private void SetBandDetails()
        {
            string err = "";

            try
            {
                _band = CustomerManager.CustomerGetBand(AccountNo, out err);
                char scoretype = 'A';

                DataRow[] bandrows = ((DataTable)StaticData.Tables[TN.TermsTypeBandList]).Select("band = '" + _band + "'");

                if (bandrows.Length > 0)
                {
                    scoretype = Convert.ToChar(bandrows[0]["scoretype"]);

                    DataView bandview = new DataView(((DataTable)StaticData.Tables[TN.TermsTypeBandList]), "scoretype = '" + scoretype + "'", "", DataViewRowState.OriginalRows);
                    cmb_band.DataSource = bandview;
                    cmb_band.DisplayMember = CN.Band;
                    cmb_band.SelectedIndex = cmb_band.FindString(_band);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "ScoreBand Dropdown");
            }


        }

        private void LoadScoreHistforCustomer()
        {
            string err = "";
            dgv_referralhistory.DataSource = CreditManager.LoadScoreHistforCustomer(_custid, out err).Tables[0];

            if (err.Length != 0)
            {
                ShowError(err);
            }
        }

        private bool LockAccount()
        {
            if (this.AccountNo.Length != 0)
            {
                AccountLocked = AccountManager.LockAccount(this.AccountNo, Credential.UserId.ToString(), out Error);
                if (Error.Length > 0)
                {
                    ShowError(Error);
                    ReadOnly = !AccountLocked;
                    SetReadOnly();
                }
            }
            else
                AccountLocked = true;

            return AccountLocked;
        }

        private void UnlockAccount()
        {
            if (this.AccountNo.Length != 0)
            {
                AccountManager.UnlockAccount(this.AccountNo, Credential.UserId, out Error);
            }
        }

        private void rtxtNewReferralNotes_TextChanged(object sender, System.EventArgs e)
        {
            CalcCharsAvailable();
        }

        private void btnAccountDetails_Click(object sender, System.EventArgs e)
        {
            AccountDetails details = new AccountDetails(this.AccountNo, FormRoot, this);
            ((MainForm)this.FormRoot).AddTabPage(details, 7);
        }

        private void btnComplete_Click(object sender, System.EventArgs e)
        {
            //Save the notes, record the decision and clear the proposal flag
            try
            {
                Wait();

                if (btnComplete.ImageIndex == 2)		/* valid */
                {
                    // Comment out for now - too risky... 
                    //var Request = new CompleteReferralStageRequest()
                    //{
                    //    AccountNo = this.AccountNo,
                    //    CustomerId = this.CustomerID,
                    //    DateProp = DateProp,
                    //    NewNotes = rtxtNewReferralNotes.Text,
                    //    RefNotes = rtxtNewReferralNotes.Text,
                    //    Approved = rbApprove.Checked,
                    //    rejected = rbReject.Checked,
                    //    reOpen = reOpen,
                    //    branchno = Convert.ToInt16(Config.BranchCode),
                    //    CreditLimit = Convert.ToDecimal(StripCurrency(txtCreditLimit.Text)),
                    //    User= Credential.UserId
                    //};

                    //if (txtStoreCardLimit.Text != "")
                    //    Request.CardLimit = Convert.ToDecimal(StripCurrency(txtStoreCardLimit.Text));

                    //Client.Call(Request,
                    //  response =>
                    //  {

                    //      if (response.Errormessage.Length > 3)
                    //          ShowError(response.Errormessage);
                    //  },null


                    CreditManager.CompleteReferralStage(this.CustomerID, this.AccountNo, this.DateProp,
                        rtxtNewReferralNotes.Text, rtxtReferralNotes.Text, rbApprove.Checked,
                        rbReject.Checked, reOpen, Convert.ToInt16(Config.BranchCode),
                        Convert.ToDecimal(StripCurrency(txtCreditLimit.Text)), Config.CountryCode, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        if (rbReject.Checked)
                        {
                            bool cancel = true;
                            decimal balance = 0;
                            bool outstPayments = false;

                            AccountManager.CheckAccountToCancel(this.m_selectedAccount, Config.CountryCode, ref balance, ref outstPayments, out Error);
                            if (Error.Length > 0)
                                ShowError(Error);
                            else
                            {
                                if (outstPayments)
                                {
                                    OutstandingPayment op = new OutstandingPayment(FormRoot);
                                    op.ShowDialog();
                                    cancel = op.rbCancel.Checked;
                                }
                                if (cancel)
                                {
                                    AccountManager.CancelAccount(this.m_selectedAccount, this.CustomerID, Convert.ToInt16(Config.BranchCode),
                                        (string)Country[CountryParameterNames.CancellationRejectionCode],
                                        balance, Config.CountryCode, "", 0, out Error);
                                    if (Error.Length > 0)
                                        ShowError(Error);
                                }
                            }
                        }

                        /*if (rbApprove.Checked)
                        {
                            if ((bool)Country[CountryParameterNames.PrizeVouchersActive] && this.AccountType != AT.ReadyFinance)
                            {
                                foreach (Crownwood.Magic.Controls.TabPage tp in ((MainForm)FormRoot).MainTabControl.TabPages)
                                {
                                    if (tp.Control is NewAccount)
                                    {
                                        NewAccount acct = (NewAccount)tp.Control;

                                        if (acct.txtAccountNumber.UnformattedText == this.AccountNo)
                                            acct.PrintPrizeVouchers = true;
                                    }
                                }
                            }
                        }*/

                        ((MainForm)this.FormRoot).tbSanction.Load(this.allowConversionToHP, this.CustomerID, this.DateProp, this.AccountNo, this.AccountType, this.ScreenMode);
                        CurrentStatus = ((MainForm)this.FormRoot).tbSanction.CurrentStatus;
                        //((MainForm)this.FormRoot).tbSanction.SetCurrentStage(SS.R);
                        ReadOnly = ((MainForm)this.FormRoot).tbSanction.ReadOnly(SS.R);
                        this.SetReadOnly();

                        if (CustomerScreen != null)
                        {
                            _preventSanctionStatusHide = true;
                            CustomerScreen.RefreshData();
                            // 5.0.0 uat338 rdb firing of form leave event seems to be prevented because
                            // we call routines on another form
                            // try setting focus to a control on this form
                            this.txtCustomerID.Focus();
                            //CustomerScreen.btnRefresh_Click(null, null);
                            _preventSanctionStatusHide = false;
                        }

                        string err = "";

                        CreditManager.SaveScoreHist(CustomerID, DateTime.Now, null, null, Convert.ToSingle(StripCurrency(txtCreditLimit.Text)), cmb_band.Text, Credential.UserId, cmb_reason.Text, AccountNo, out err);

                        if (err.Length > 0)
                        {
                            ShowError(err);
                        }

                        char band = ' ';

                        if (cmb_band.Text.Trim().Length > 0)
                        {
                            band = Convert.ToChar(cmb_band.Text.Trim());
                        }
                        CustomerManager.CustomerSaveBand(CustomerID, band);

                    }
                }
                else
                {
                    CreditManager.SaveReferralNotes(this.CustomerID, this.AccountNo, this.DateProp,
                        rtxtNewReferralNotes.Text, Convert.ToDecimal(StripCurrency(txtCreditLimit.Text)), Config.CountryCode, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                }
                UnlockAccount();
                Referral_Load(null, null);
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

        private void menuExit_Click(object sender, System.EventArgs e)
        {

            CloseTab();
        }

        private void ValidateControl(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Wait();
                bool valid = true;

                if (!IsStrictMoney(txtCreditLimit.Text))
                    txtCreditLimit.Text = (0).ToString(DecimalPlaces);
                else
                    txtCreditLimit.Text = MoneyStrToDecimal(txtCreditLimit.Text).ToString(DecimalPlaces);

                if (!rbApprove.Checked && !rbReject.Checked && rbApprove.Enabled)
                {
                    valid = false;
                    errorProvider1.SetError(rbApprove, GetResource("M_DECISIONSCHECK"));
                }
                else
                    errorProvider1.SetError(rbApprove, "");

                btnComplete.ImageIndex = valid ? 2 : 1;

            }
            catch (Exception ex)
            {
                Catch(ex, "ValidateControl");
            }
            finally
            {
                StopWait();
            }
        }

        private void OnCheckedChanged(object sender, System.EventArgs e)
        {
            ValidateControl(null, null);
        }

        //private void LaunchSummary()
        //{
        //    while (Math.Abs(gbDetails.Location.Y - destY)> 1)
        //    {
        //        int newLocation = gbDetails.Location.Y - ((gbDetails.Location.Y - destY) / 2);
        //        int newHeight = gbDetails.Height + gbDetails.Location.Y - newLocation;

        //        if(direction == "UP")
        //        {
        //            gbDetails.Location = new Point(gbDetails.Location.X, newLocation);
        //            gbDetails.Height = newHeight;
        //        }
        //        else
        //        {
        //            gbDetails.Height = newHeight;
        //            gbDetails.Location = new Point(gbDetails.Location.X, newLocation);					
        //        }
        //        Thread.Sleep(40);		
        //    }
        //    gbDetails.Location = new Point(gbDetails.Location.X, destY);
        //}

        //private void tcReferral_SelectionChanged(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        Wait();

        //        Thread summary = null;

        //        switch(tcReferral.SelectedTab.Name)
        //        {
        //            case "tpSummary":						
        //                destY = 8;
        //                direction = "UP";
        //                summary = new Thread(new ThreadStart(LaunchSummary));
        //                summary.Start();	
        //                break;
        //            default:
        //                destY = 104;
        //                direction = "DOWN";
        //                summary = new Thread(new ThreadStart(LaunchSummary));
        //                summary.Start();
        //                break;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        Catch(ex, "tcReferral_SelectionChanged");
        //    }
        //    finally
        //    {
        //        StopWait();
        //    }
        //}

        private void cmb_band_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_band.Text != _band && loaded == true && cmb_band.Text != string.Empty)
            {
                AuthorisePrompt ap = new AuthorisePrompt(this, cmb_band, "Please authourise band change.");
                ap.ShowDialog();
                if (!ap.Authorised)
                {
                    cmb_band.SelectedIndex = cmb_band.FindString(_band);
                }
                else // need to be able to save changes 
                {
                    _band = cmb_band.Text; // store this so doesn't prompt annoyingly twice
                    btnComplete.Enabled = true;
                }
            }

        }

        private void txtCreditLimit_Enter(object sender, EventArgs e)
        {
            try
            {
                if (!txtCreditLimitAuth)
                {
                    AuthoriseCheck Auth = new AuthoriseCheck(this.Name, "txtcreditlimit");

                    if (Auth.ControlPermissionCheck(Credential.User).HasValue)
                    {
                        txtCreditLimitAuth = true;
                    }
                    else
                    {
                        Auth.ShowDialog();
                        if (Auth.IsAuthorised)
                        {
                            txtCreditLimitAuth = true;
                        }
                    }

                    txtCreditLimit.ReadOnly = !txtCreditLimitAuth;
                }

                //IP - 13/12/10 - Store Card
                //If this is a Store Card account, if the credit limit is changed this needs to reflect on the Store Card limit.
                if (AccountType == "T")
                {
                    txtStoreCardLimit.Text = (txtCreditLimit.Text == "" ? 0 : Convert.ToDecimal(StripCurrency(txtCreditLimit.Text)) * (decimal)Country[CountryParameterNames.StoreCardPercent] / 100.0M).ToString(DecimalPlaces);
                }

            }
            catch (Exception ex)
            {
                Catch(ex, "ReferalAuth");
            }
        }

        private void BuildLineItems()
        {
            itemDoc = new XmlDocument();
            itemDoc.LoadXml("<ITEMS></ITEMS>");

            //initialise the XML document and the tree view
            if (LineItems != null)
            {
                LineItems = itemDoc.ImportNode(LineItems, true);
                itemDoc.ReplaceChild(LineItems, itemDoc.DocumentElement);
            }

            if (itemDoc.DocumentElement.HasChildNodes)
            {
                populateTable();
            }
        }

        private void Populate(DataSet details)
        {
            foreach (DataRow row in details.Tables[TN.ReferralData].Rows)
            {
                /* Personal Details */
                txtAge.Text = Convert.ToString(row[CN.Age]);
                txtMaritalStatus.Text = (string)row[CN.MaritalStatus];
                txtResidentialStatus.Text = (string)row[CN.ResidentialStatus];
                txtOccupation.Text = (string)row[CN.Occupation];
                txtExpenses.Text = ((decimal)row[CN.Expenses]).ToString(DecimalPlaces);

                if ((DateTime)row[CN.DateInAddress] > DatePicker.MinValue)
                {
                    dtDateInCurrentAddress.DateFrom = DateTime.Today;
                    dtDateInCurrentAddress.Value = (DateTime)row[CN.DateInAddress];
                }

                if ((DateTime)row[CN.DateEmployed] > DatePicker.MinValue)
                {
                    dtDateInCurrentEmp.DateFrom = DateTime.Today;
                    dtDateInCurrentEmp.Value = (DateTime)row[CN.DateEmployed];
                }

                txtIncomeAfterTax.Text = ((decimal)row[CN.Income]).ToString(DecimalPlaces);

                /* Agreement Details */
                txtRepaymentPcent.Text = ((decimal)row[CN.RepaymentPcent]).ToString("F2") + "%";
                txtAcctType.Text = _acctType;
                txtTermsType.Text = (string)row[CN.TermsType];
                txtDeposit.Text = ((decimal)row[CN.Deposit]).ToString(DecimalPlaces);
                txtInstalment.Text = ((decimal)row[CN.MonthlyInstal]).ToString(DecimalPlaces);
                txtFinalInstalment.Text = ((decimal)row[CN.FinalInstal]).ToString(DecimalPlaces);
                txtAgreementTotal.Text = ((decimal)row[CN.AgreementTotal]).ToString(DecimalPlaces);

                /* Account History */
                txtValueOfAllArrears.Text = ((decimal)row[CN.ValueOfArrears]).ToString(DecimalPlaces);
                txtOutstandingBalance.Text = ((decimal)row[CN.OutstBal]).ToString(DecimalPlaces);
                txtLargestAgreement.Text = ((decimal)row[CN.LargestAgreement]).ToString(DecimalPlaces);
                txtFeesAndInterest.Text = ((decimal)row[CN.FeesAndInterest]).ToString(DecimalPlaces);
                txtTotalInstalmentValue.Text = ((decimal)row[CN.TotalCurrentInstalments]).ToString(DecimalPlaces);
                txtNoOfCurrent.Text = Convert.ToString(row[CN.NumCurrent]);
                txtNoOfSettled.Text = Convert.ToString(row[CN.NumSettled]);
                txtNoAccountsInArrears.Text = Convert.ToString(row[CN.NumInArrears]);
                txtCashAccountsSettled.Text = Convert.ToString(row[CN.NumCashSettled]);
                txtNoOfReturnedCheques.Text = Convert.ToString(row[CN.NumReturnedCheques]);
                txtWorstCurrentStatus.Text = (string)row[CN.WorstCurrent];
                txtWorstSettledStatus.Text = (string)row[CN.WorstSettled];
                txtLongestAgreement.Text = Convert.ToString(row[CN.LongestAgreement]);
                txtRepaymentPcentCurrent.Text = ((decimal)row[CN.RepaymentPcentCurrent]).ToString("F2") + "%";
                txtCreditLimit.Text = ((decimal)row[CN.CreditLimit]).ToString(DecimalPlaces);
                txtMmi.Text = ((decimal)row[CN.MmiLimit]).ToString(DecimalPlaces);
                txtRFAccts.Text = Convert.ToString(row[CN.NumRFAccounts]);
                txtSysRecommend.Text = (string)row[CN.SysRecommend];
            }
        }

        private void populateTable()
        {
            //Set up the datagrid columns
            if (itemsTable == null)
            {
                //Create the table to hold the Line items
                itemsTable = new DataTable("Items");
                DataColumn[] key = new DataColumn[4];

                itemsTable.Columns.AddRange(new DataColumn[]{ new DataColumn("ProductCode"),
                                                         new DataColumn("ItemId"),                  //IP - 27/05/11 - CR1212 - RI - #3756
														 new DataColumn("ProductDescription"),
														 new DataColumn("StockLocation"),
														 new DataColumn("QuantityOrdered"),
														 new DataColumn("UnitPrice"),
														 new DataColumn("Value"),
														 new DataColumn("DeliveredQuantity"),
														 new DataColumn("DateDelivered"),
														 new DataColumn("DatePlanDel"),
														 new DataColumn("DateReqDel"),
														 new DataColumn("TimeReqDel"),
														 new DataColumn("DelNoteBranch"),
														 new DataColumn("Notes"),
														 new DataColumn("ContractNo"),
                                                         new DataColumn("ParentItemID")});          //IP - 05/10/11 - RI - #8132

                //key[0] = itemsTable.Columns["ProductCode"];
                key[0] = itemsTable.Columns["ItemId"];                                              //IP - 27/05/11 - CR1212 - RI - #3756
                key[1] = itemsTable.Columns["StockLocation"];
                key[2] = itemsTable.Columns["ContractNo"];
                key[3] = itemsTable.Columns["ParentItemID"];                                        //IP - 05/10/11 - RI - #8132
                itemsTable.PrimaryKey = key;
            }

            dgLineItems.DataSource = itemsTable.DefaultView;

            if (dgLineItems.TableStyles.Count == 0)
            {
                DataGridTableStyle tabStyle = new DataGridTableStyle();
                tabStyle.MappingName = itemsTable.TableName;

                AddColumnStyle("ProductCode", tabStyle, 90, true, GetResource("T_PRODCODE"), "", HorizontalAlignment.Left);
                AddColumnStyle("ProductDescription", tabStyle, 200, true, GetResource("T_DESCRIPTION"), "", HorizontalAlignment.Left);
                AddColumnStyle("StockLocation", tabStyle, 75, true, GetResource("T_STOCKLOCN"), "", HorizontalAlignment.Left);
                AddColumnStyle("QuantityOrdered", tabStyle, 40, true, GetResource("T_QUANTITY"), "", HorizontalAlignment.Left);
                AddColumnStyle("UnitPrice", tabStyle, 70, true, GetResource("T_UNITPRICE"), DecimalPlaces, HorizontalAlignment.Right);
                AddColumnStyle("Value", tabStyle, 70, true, GetResource("T_VALUE"), DecimalPlaces, HorizontalAlignment.Right);
                AddColumnStyle("DeliveredQuantity", tabStyle, 70, true, GetResource("T_DELIVEREDQTY"), "", HorizontalAlignment.Left);
                AddColumnStyle("DateDelivered", tabStyle, 70, true, GetResource("T_DELIVERYDATE"), "", HorizontalAlignment.Left);
                AddColumnStyle("DatePlanDel", tabStyle, 150, true, GetResource("T_DATEDELPLAN"), "", HorizontalAlignment.Left);
                AddColumnStyle("DateReqDel", tabStyle, 150, true, GetResource("T_REQDELDATE"), "", HorizontalAlignment.Left);
                AddColumnStyle("TimeReqDel", tabStyle, 150, true, GetResource("T_REQDELTIME"), "", HorizontalAlignment.Left);
                AddColumnStyle("DelNoteBranch", tabStyle, 100, true, GetResource("T_DELNOTEBRANCH"), "", HorizontalAlignment.Left);
                AddColumnStyle("Notes", tabStyle, 150, true, GetResource("T_NOTES"), "", HorizontalAlignment.Left);
                AddColumnStyle("ContractNo", tabStyle, 100, true, GetResource("T_CONTRACTNO"), "", HorizontalAlignment.Left);

                dgLineItems.TableStyles.Add(tabStyle);
            }

            itemsTable.Clear();
            tvItems.Nodes.Clear();
            tvItems.Nodes.Add(new TreeNode("Account"));

            double subTotal = 0;
            populateTable(itemDoc.DocumentElement, tvItems.Nodes[0], ref subTotal);

            tvItems.Nodes[0].Expand();
        }

        private void populateTable(XmlNode relatedItems, TreeNode tvNode, ref double sub)
        {
            Function = "populateTable";
            string itemType = "";
            double qty = 0;

            //outer loop iterates through <item> tags
            foreach (XmlNode item in relatedItems.ChildNodes)
            {
                if (item.NodeType == XmlNodeType.Element)
                {
                    TreeNode tvChild = new TreeNode();
                    tvChild.Tag = item.Attributes[Tags.Key].Value;
                    DataRow row = itemsTable.NewRow();
                    bool showRow = true;

                    itemType = item.Attributes[Tags.Type].Value;
                    qty = Convert.ToDouble(item.Attributes[Tags.Quantity].Value);

                    //tvChild.Text = itemType;
                    tvChild.Text = item.Attributes[Tags.FreeGift].Value.ToUpper().Trim() == "TRUE" ? "FreeGift" : itemType; //IP - 21/04/10 - UAT(53) UAT5.2 - Merged from 4.3
                    tvChild.ImageIndex = 1;
                    tvChild.SelectedImageIndex = 1;

                    if (itemType == IT.Stock || itemType == IT.Component)
                    {
                        tvChild.ImageIndex = 0;
                        tvChild.SelectedImageIndex = 0;
                    }
                    //if(itemType==IT.Discount||itemType==IT.KitDiscount)
                    if (itemType == IT.Discount || itemType == IT.KitDiscount || item.Attributes[Tags.FreeGift].Value.ToUpper().Trim() == "TRUE") //IP - 21/04/10 - UAT(53) UAT5.2 - Merged from 4.3
                    {
                        tvChild.ImageIndex = 2;
                        tvChild.SelectedImageIndex = 2;
                    }
                    if (itemType == IT.Warranty)
                    {
                        tvChild.ImageIndex = 3;
                        tvChild.SelectedImageIndex = 3;
                    }
                    if (itemType == IT.Kit || qty <= 0)
                    {
                        showRow = false;
                    }

                    row["ProductCode"] = item.Attributes[Tags.Code].Value;
                    row["ItemId"] = item.Attributes[Tags.ItemId].Value;
                    row["ProductDescription"] = item.Attributes[Tags.Description1].Value;
                    row["StockLocation"] = item.Attributes[Tags.Location].Value;
                    row["QuantityOrdered"] = item.Attributes[Tags.Quantity].Value;
                    row["UnitPrice"] = item.Attributes[Tags.UnitPrice].Value;
                    row["DeliveredQuantity"] = item.Attributes[Tags.DeliveredQuantity].Value;
                    row["DateDelivered"] = item.Attributes[Tags.DateDelivered].Value;
                    row["DatePlanDel"] = Convert.ToDateTime(item.Attributes[Tags.PlannedDeliveryDate].Value).ToString("dd/MM/yyy");
                    row["DateReqDel"] = Convert.ToDateTime(item.Attributes[Tags.DeliveryDate].Value).ToString("dd/MM/yyy");
                    row["TimeReqDel"] = item.Attributes[Tags.DeliveryTime].Value;
                    row["DelNoteBranch"] = item.Attributes[Tags.BranchForDeliveryNote].Value;
                    row["Notes"] = item.Attributes[Tags.ColourTrim].Value;
                    row["ContractNo"] = item.Attributes[Tags.ContractNumber].Value;
                    row["ParentItemID"] = item.ParentNode.ParentNode.Name == "Item" ? item.ParentNode.ParentNode.Attributes[Tags.ItemId].Value : "0";       //IP - 05/10/11 - RI - #8132

                    if (showRow)
                    {
                        row["Value"] = item.Attributes[Tags.Value].Value;
                        sub += Convert.ToDouble(StripCurrency(item.Attributes[Tags.Value].Value));
                    }

                    foreach (XmlNode child in item.ChildNodes)
                        if (child.NodeType == XmlNodeType.Element && child.Name == Elements.RelatedItem)
                        {
                            if (child.HasChildNodes)
                                populateTable(child, tvChild, ref sub);
                        }

                    if (qty > 0)
                        tvNode.Nodes.Add(tvChild);

                    if (showRow)
                        itemsTable.Rows.Add(row);
                }
            }
            Function = "End of populateTable";
        }


        private void tvItems_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            try
            {
                if ((string)e.Node.Tag != null)
                {
                    string key = (string)e.Node.Tag;
                    string product = key.Substring(0, key.IndexOf("|"));
                    string location = key.Substring(key.IndexOf("|") + 1, key.Length - (key.IndexOf("|") + 1));

                    int index = 0;
                    foreach (DataRowView row in itemsTable.DefaultView)
                    {
                        if ((string)row.Row["ProductCode"] == product && (string)row.Row["StockLocation"] == location)
                        {
                            dgLineItems.Select(index);
                            dgLineItems.CurrentRowIndex = index;
                        }
                        else
                        {
                            dgLineItems.UnSelect(index);
                        }
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lCreditLimit_Click(object sender, EventArgs e)
        {

        }
    }
}
