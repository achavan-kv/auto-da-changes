using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.StoreCard;
using Crownwood.Magic.Menus;
using STL.Common;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.SanctionStages;
using STL.Common.Constants.ScreenModes;
using STL.Common.Constants.StoreInfo;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using STL.PL.StoreCard.Common;
using WIA;
using WIALib;
using Blue.Cosacs.Shared.Services.Credit;

namespace STL.PL
{
    /// <summary>
    /// Allows creation of a new customer. Name, date of birth, address,
    /// employment and bank details are entered here. Other screens can 
    /// navigate to this screen to review and update customer details.
    /// Existing accounts are listed for the customer and new accounts can
    /// be linked to the customer using a 'Main Holder' or 'Joint' relationship.
    /// </summary>
    /// 


    public partial class BasicCustomerDetails : CommonForm
    {
        public event OnChangeAddressDate onChangeAddressDate;

        private bool userChanged = true;
        private new string Error = String.Empty;
        //private DataSet ds = null;
        private DataView dv = null;
        private bool _readonly = true;
        private bool existingStoreCard = false;
        public new bool loaded = false;
        private string _accountNo = String.Empty;
        private decimal _cashPrice = 0;
        public string title = String.Empty;
        private bool settled = false;
        public SanctionStage1 SanctionScreen = null;
        private bool stage1Complete = false;
        private AddToCollection addToAccounts = null;
        private string propResult = String.Empty;
        public bool isUnemployed = false;
        private string newAccounttoCancel = String.Empty;
        private Control controlAddTo = null;
        private Control controlReverseAddTo = null;
        private Control allowReviseSettled = null;
        private DataView addressView;

        private Control allowHP = null;
        private Control allowRF = null;
        private Control allowCash = null;
        private Control allowStore = null;
        private Image imgStoreCardGrey = null;
        private Image imgStoreCardNormal = null;
        private int OldRelationshipIndex = -1;
        private DataTable EmploymentDetails = null;
        private DataTable BankDetails = null;
        private DataSet dsPhotos = new DataSet();
        private Blue.Cosacs.Shared.StoreCard storeCard = new Blue.Cosacs.Shared.StoreCard();
        private CustomerResult customer = new CustomerResult();
        private bool accountLocked;          // jec 28/06/11
        private string maritalStatusFromProposal = string.Empty;
        private bool IsSpouseWorking = false;
        private int DependantsFromProposal = 0;
        private decimal dependentSpendFactor = 0;
        private decimal applicantSpendFactor = 0;

        public bool staticLoaded = false;
        // CR903 Need to check if customer this is  Courts or Non Courts against store type
        string _customerStoreType = String.Empty;
        bool customerBlacklisted = false; //IP - 01/09/09 - 5.2 UAT(823)

        private bool validAge = false; //IP - 11/05/10 - UAT(141) UAT5.2.1.0 log 
        public string CFirstName = "";
        //UserRight allowUnblockCashLoan = UserRight.Create("menuCashLoanOveride");                   //IP - 21/10/11 - #8425 - CR1232 - Cash Loans
        //private string cashLoanBlocked = string.Empty;                                              //IP - 03/11/11 - CR1232 - Cash Loans

        //CR903 Include a public property to hold the selected branch's  
        // createRFAccount value                        jec
        private bool _createRF = false;
        public bool createRF
        {
            get
            {
                return _createRF;
            }
            set
            {
                _createRF = value;
            }
        }
        //CR903 Include a public property to hold the selected branch's  
        // createHPAccount value                        jec
        private bool _createHP = false;
        public bool createHP
        {
            get
            {
                return _createHP;
            }
            set
            {
                _createHP = value;
            }
        }

        private bool _createStore = false;
        public bool CreateStore
        {
            get
            {
                return _createStore;
            }
            set
            {
                _createStore = value;
            }
        }
        //CR903 Include a public property to hold the selected branch's  
        // createCashAccount value                        jec
        private bool _createCash = false;
        public bool createCash
        {
            get
            {
                return _createCash;
            }
            set
            {
                _createCash = value;
            }
        }
        //CR903 Include a public property to hold the selected branch's store type
        private string m_storeType = String.Empty;
        public string SType
        {
            get
            {
                return m_storeType;
            }
            set
            {
                m_storeType = value;
            }
        }

        private string _newAccountNo;
        public string NewAccountNo
        {
            get { return _newAccountNo; }
            set { _newAccountNo = value; }
        }

        private string _newAccountType = String.Empty;
        public string NewAccountType
        {
            get { return _newAccountType; }
            set { _newAccountType = value; }
        }

        public string AccountNo
        {
            get { return _accountNo; }
            set { _accountNo = value; }
        }
        public decimal CashPrice
        {
            get { return _cashPrice; }
            set { _cashPrice = value; }
        }

        private string _custid = String.Empty;
        public string CustomerID
        {
            get { return _custid; }
            set { _custid = value; }
        }
        private string _currentStatus = String.Empty;
        public string CurrentStatus
        {
            get { return _currentStatus; }
            set { _currentStatus = value; }
        }
        private string _accountType = String.Empty;
        public string AccountType
        {
            get { return _accountType; }
            set { _accountType = value; }
        }
        private bool _canCreateRF = false;
        private string _relationship = String.Empty;
        public string Relationship
        {
            get { return _relationship; }
            set { _relationship = value; }
        }

        public bool ReadOnly
        {
            get { return _readonly; }
            set { _readonly = value; }
        }
        private string _holder = String.Empty;
        private bool repo = false;
        //private decimal _rflimit = 0;
        public decimal RFLimit
        {
            get { return Convert.ToDecimal(StripCurrency(txtRFLimit.Text)); }
            set { txtRFLimit.Text = (value).ToString(DecimalPlaces); }
        }

        private string sc_title;
        private string sc_name;
        private string sc_lastname;

        //IP - 16/12/10 - Store Card
        //  private decimal _storeCardLimit = 0;
        //public decimal StoreCardLimit
        //{
        //    get { return _storeCardLimit; }
        //    set { _storeCardLimit = value; }
        //}

        //IP - 21/12/10 - Store Card
        //private decimal _storeCardAvailable = 0;
        //public decimal StoreCardAvailable
        //{
        //    get { return _storeCardAvailable; }
        //    set { _storeCardAvailable = value; }
        //}

        public string Holder
        {
            get { return _holder; }
            set { _holder = value; }
        }
        private string _linked = String.Empty;
        public string Linked
        {
            get { return _linked; }
            set { _linked = value; }
        }

        //CR903 - include a public property to hold the store type filter
        private string _storeFilter = String.Empty;
        public string StoreFilter
        {
            get { return _storeFilter; }
            set { _storeFilter = value; }
        }

        private bool m_printScreen = true;
        private bool printScreen
        {
            get
            {
                return m_printScreen;
            }
            set
            {
                m_printScreen = value;
            }
        }

        private string m_path = String.Empty;
        private string path
        {
            get
            {
                return m_path;
            }
            set
            {
                m_path = value;
            }
        }

        private string m_sigPath = String.Empty;
        private string localSignaturePath
        {
            get
            {
                return m_sigPath;
            }
            set
            {
                m_sigPath = value;
            }
        }

        private string m_photoPath = String.Empty;
        private string photoPath
        {
            get
            {
                return m_photoPath;
            }
            set
            {
                m_photoPath = value;
            }
        }

        private string m_signaturePath = String.Empty;
        private string signaturePath
        {
            get
            {
                return m_signaturePath;
            }
            set
            {
                m_signaturePath = value;
            }
        }

        private string m_fileName = String.Empty;
        private string fileName
        {
            get
            {
                return m_fileName;
            }
            set
            {
                m_fileName = value;
            }
        }

        private string m_signatureFileName = String.Empty;
        private string signatureFileName
        {
            get
            {
                return m_signatureFileName;
            }
            set
            {
                m_signatureFileName = value;
            }
        }

        private bool m_customerSaved = false;
        private bool customerSaved
        {
            get
            {
                return m_customerSaved;
            }
            set
            {
                m_customerSaved = value;
            }
        }

        private bool m_existingCustomer = false;
        private bool existingCustomer
        {
            get
            {
                return m_existingCustomer;
            }
            set
            {
                m_existingCustomer = value;
            }
        }

        //IP - 08/07/10 - UAT995 - UAT5.2
        private bool isLoan = false;

        private bool _duplicatecustid = false;
        public bool DuplicateCustid
        {
            get { return _duplicatecustid; }
            set { _duplicatecustid = value; }
        }
        private struct SanctionStage
        {
            public string CustID;
            public string AcctType;
            public string AccountNo;
            public DateTime DateProp;
            public string ScreenMode;
            public string CheckType;
            public string Stage;
            public int ImageIndex;

        }
        private SanctionStage StageToLaunch;
        private DataSet basicDetails = null;
        private string firstName = String.Empty;
        private string lastName = String.Empty;
        private string address = String.Empty;          //CR1084
        private string phoneNumber = String.Empty;      //CR1084
        private DataSet accounts = null;
        private string ThreadCustomerID = String.Empty;
        private string ThreadAccountNo = String.Empty;
        private string ThreadRelationship = String.Empty;
        private bool checkedForAcctsInArrs = false;        //IP - 17/03/11 -         

        public event RecordIDHandler<StoreCardCustDetails> CustidSelected;



        private STL.PL.DatePicker dtPrevEmpStart;
        private STL.PL.DatePicker dtCurrEmpStart;
        private STL.PL.DatePicker dtBankOpened;
        private STL.PL.PhoneNumberBox txtEmpTelCode;
        private STL.PL.PhoneNumberBox txtEmpTelNum;
        public STL.PL.AccountTextBox txtAccountNo;
        public STL.PL.CustIdTextBox txtCustID;
        //CR 866 Added mandatoryFieldsDS
        private DataSet mandatoryFieldsDS;
        //70363 
        private DateTime m_currentDateIn = Date.blankDate;
        //71522
        //private Dictionary<string, DateTime> currentDateList = new Dictionary<string, DateTime>();
        private AddressHistory addresshistory = new AddressHistory();
        //private Crownwood.Magic.Menus.MenuControl menuMain;
        private Crownwood.Magic.Menus.MenuCommand menuRefer;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuCustomer;
        private Crownwood.Magic.Menus.MenuCommand menuSave;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private Crownwood.Magic.Menus.MenuCommand menuCustomerCodes;
        private Crownwood.Magic.Controls.TabPage currentTab;
        private Crownwood.Magic.Menus.MenuCommand menuRecentCust;
        private Crownwood.Magic.Menus.MenuCommand menuRevise;
        private Crownwood.Magic.Menus.MenuCommand menuSanction;
        private Crownwood.Magic.Menus.MenuCommand menuS1;
        private Crownwood.Magic.Menus.MenuCommand menuS2;
        private Crownwood.Magic.Menus.MenuCommand menuUW;
        private Crownwood.Magic.Menus.MenuCommand menuDC;
        private Crownwood.Magic.Menus.MenuCommand menuLinkToAccount;
        private Crownwood.Magic.Menus.MenuCommand menuUnblockCredit;
        private Crownwood.Magic.Controls.TabPage tpBasicDetails;
        private Crownwood.Magic.Controls.TabControl tcAddress;
        private Crownwood.Magic.Controls.TabControl tcDetails;
        private Crownwood.Magic.Controls.TabPage tpEmployment;
        private Crownwood.Magic.Controls.TabPage tpBank;
        private Crownwood.Magic.Menus.MenuCommand menuManualRF;

        private System.Windows.Forms.Label lEmploymentDetails = new System.Windows.Forms.Label();
        private System.Windows.Forms.Label lBankDetails = new System.Windows.Forms.Label();
        private System.Windows.Forms.Label lResidentialDetails = new System.Windows.Forms.Label();
        private System.Windows.Forms.Label lFinancialDetails = new System.Windows.Forms.Label();
        private System.Windows.Forms.ImageList menuIcons;
        private System.Windows.Forms.GroupBox grpAccounts;
        private System.Windows.Forms.DataGrid dgAccounts;
        private System.Windows.Forms.CheckBox chxIncludeSettled;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Label lLoyaltyCardNo;
        private System.Windows.Forms.TextBox txtLoyaltyCardNo;
        private System.Windows.Forms.TextBox txtMaidenName;
        private System.Windows.Forms.Label lMaidenName;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.TextBox txtRFLimit;
        private System.Windows.Forms.DateTimePicker dtDOB;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label labelAvailable;
        private System.Windows.Forms.TextBox txtRFAvailable;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.TextBox txtAlias;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.ComboBox drpTitle;
        //  private System.Windows.Forms.TextBox TitleC;//KD
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelCredit;
        private System.Windows.Forms.GroupBox gbAddress;
        private System.Windows.Forms.CheckBox chxCreateHistory;
        private System.Windows.Forms.CheckBox chxCopyName;//KD

        private System.Windows.Forms.GroupBox gbEmployment;
        private System.Windows.Forms.Label lIncome;
        private System.Windows.Forms.TextBox txtIncome;
        private System.Windows.Forms.Label lEmpTelNum1;
        private System.Windows.Forms.Label lEmpTelCode1;
        private System.Windows.Forms.ComboBox drpPayFrequency;
        private System.Windows.Forms.Label lPayFrequency1;
        private System.Windows.Forms.ComboBox drpOccupation;
        private System.Windows.Forms.Label lOccupation1;
        private System.Windows.Forms.ComboBox drpEmploymentStat;
        private System.Windows.Forms.Label lEmploymentStat1;
        private System.Windows.Forms.GroupBox gbBank;
        private System.Windows.Forms.Label lBankAcctNumber1;
        private System.Windows.Forms.Label lBankAcctType1;
        private System.Windows.Forms.Label lBank1;
        private System.Windows.Forms.ComboBox drpBankAcctType;
        private System.Windows.Forms.ComboBox drpBank;
        private System.Windows.Forms.TextBox txtBankAcctNumber;
        private System.Windows.Forms.CheckBox chxPrivilegeClub;
        private System.Windows.Forms.GroupBox gbDetails;
        private System.Windows.Forms.TextBox txtRFIssueNo;
        private System.Windows.Forms.Label lRFIssueNo;
        private System.Windows.Forms.Label allowReviseRepo;
        public System.Windows.Forms.ComboBox drpRelationship;
        private System.Windows.Forms.GroupBox gbNameHistory;
        private Crownwood.Magic.Controls.TabPage tpNameHistory;
        private System.Windows.Forms.DataGrid dgNameHistory;
        private Crownwood.Magic.Controls.TabPage tpAddressHistory;
        private System.Windows.Forms.GroupBox gbAddressHistory;
        private System.Windows.Forms.DataGrid dgAddressHistory;
        private System.Windows.Forms.CheckBox chxIncludeAssocAccounts;
        private System.Windows.Forms.Label lPrivilegeClubDesc;
        private Crownwood.Magic.Menus.MenuCommand menuSpecialAccount;
        private Crownwood.Magic.Menus.MenuCommand menuManualHP;
        private Crownwood.Magic.Menus.MenuCommand menuManualCash;
        private System.Windows.Forms.Label lNewAddress;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ComboBox drpAddressType;
        private GroupBox groupBox5;
        private Label lBranch;
        private ComboBox drpBranch;
        private Button btnCreateCashAccount;
        private Button btnCreateHPAccount;
        private Button btnCreateRFAccount;
        private TextBox txtBand;
        private Label lBand;
        private Crownwood.Magic.Controls.TabPage tpTelHistory;
        private GroupBox gbTelHistory;
        private DataGrid dataGrid1;
        private DataGrid dgTelephoneHistory;
        private Label allowReviseCash;
        private Label label6;
        private ComboBox drpNationality1;
        private Label label7;
        private ComboBox drpMaritalStat1;
        private Label label10;
        private Crownwood.Magic.Controls.TabPage tpResidential;
        private Crownwood.Magic.Controls.TabPage tpFinancial;
        private DatePicker dtDateInPrevAddress1;
        private Button btnSaveResidential;
        private DatePicker dtDateInCurrentAddress1;
        private ComboBox drpCurrentResidentialStatus1;
        private Label lMortgage1;
        private TextBox txtMortgage1;
        private Label lPrevResidentialStatus1;
        private Label lPropertyType1;
        private ComboBox drpPropertyType1;
        private Label lCurrentResidentialStatus1;
        private ComboBox drpPrevResidentialStatus1;
        private GroupBox groupBox7;
        private Label lAdditionalExpenditure2;
        private Label lAdditionalExpenditure1;
        private TextBox txtAdditionalExpenditure2;
        private TextBox txtAdditionalExpenditure1;
        private TextBox txtOther1;
        private Label lTotal1;
        private TextBox txtTotal1;
        private Label lOther1;
        private Label lMisc1;
        private TextBox txtMisc1;
        private Label lLoans1;
        private TextBox txtLoans1;
        private Label lUtilities1;
        private TextBox txtUtilities1;
        private GroupBox grpIncome;
        private TextBox txtDisposable;
        private Label lDisposable;
        private GroupBox groupBox8;
        private Label lAddIncome1;
        private TextBox txtAddIncome1;
        private Label lNetIncome1;
        private TextBox txtNetIncome1;
        private GroupBox gbBankDetails;
        private Button btnSaveFinancial;
        private DatePicker dtBankOpened1;
        private CreditCardNo txtCreditCardNo1;
        private Label lCreditCardNo1;
        private TextBox txtBankAcctNumber1;
        private ComboBox drpBankAcctType1;
        private Label label11;
        private Label label13;
        private ComboBox drpBank1;
        private Label label14;
        private ErrorProvider errorProvider2;
        private Button btnSaveEmployment;
        private Panel grpGiro;
        private Label lPayMethod;
        private ComboBox drpPaymentMethod;
        private ComboBox drpPayByGiro1;
        private Label lBankAccountName1;
        private Label lGiroDueDate1;
        private TextBox txtBankAccountName1;
        private ComboBox drpGiroDueDate1;
        private Label lPayByGiro1;
        private NumericUpDown txtDependants;
        private ComboBox drpEductation1;
        private Label lEducation1;
        private Label lJobTitle1;
        private Label lIndustry1;
        private Label lOrganisation1;
        private Label lTransportType1;
        private ComboBox drpTransportType1;
        private Label lDistanceFromStore1;
        private NumericUpDown txtDistanceFromStore1;
        private ComboBox txtIndustry1;
        private ComboBox txtOrganisation1;
        private ComboBox txtJobTitle1;
        private Crownwood.Magic.Controls.TabPage tpPhoto;
        private PictureBox pbSignature;
        private PictureBox pbPhoto;
        private Button btnPrevious;
        private Button btnAddSignature;
        private Button btnAddPicture;
        private Button btnSavePicture;
        private Button btnTakePicture;
        private Label label15;
        private TextBox txtFileName;
        private DataGridView dgvPreviousPhotos;
        private Label label16;
        private TextBox txtSignature;
        private Button btnSaveSignature;
        private System.Windows.Forms.Label allowManual;
        private Button btnPrivilegeClub;
        private RadioButton radAddressAudit;
        private RadioButton radAddress;
        private RadioButton radTelephoneAudit;
        private RadioButton radTelephoneHistory;
        private PictureBox LoyaltyLogo_pb;
        private Crownwood.Magic.Controls.TabPage tpLoyaltyScheme;
        private TabControl tabControl1;
        private TabPage tp_HcMember;
        private Button btn_override;
        private Button LoyaltyVoucher_btn;
        private Button LoyaltyAcct_btn;
        private TextBox LoyaltyStatusAcct_txt;
        private TextBox LoyaltyStatusVoucher_txt;
        private Label label17;
        private Button LoyaltyJoin_btn;
        private PictureBox pictureBox2;
        private MaskedTextBox loyaltymemno_mtb;
        private Label loyaltyinfo_lbl;
        private Button Loyalty_save_btn;
        private Label label18;
        private Label label19;
        private DateTimePicker LoyaltyEnd_dtp;
        private DateTimePicker LoyaltyStart_dtp;
        private Label label20;
        private Label label21;
        private ComboBox LoyaltyType_cmb;
        private Panel panel1;
        private Label Reason_lbl;
        private Label label22;
        private ComboBox LoyaltyReason_cmb;
        private TabPage tp_HcHistroy;
        private DataGridView dgv_HChistory;
        private Button btnCreateStoreCard;
        private TextBox txtStoreCardAvailable;
        private Label lblStoreCardAvailable;
        private Panel panelStoreCard;
        private PictureBox pictureStoreCardApproved;
        private ErrorProvider errProRFCreateInterval;
        private MenuCommand menuCashLoanOveride;
        private Label lblCashLoan;
        private Panel panel2;
        private Panel panel3;
        private CheckBox chkSms;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private CheckBox Copy;
        private CheckBox checkBox3;
        private CheckBox CopyName;
        private MenuCommand menuReviseCashLoan;



        private void HashMenus()
        {
            controlAddTo = new Control();
            controlAddTo.Enabled = false;
            controlReverseAddTo = new Control();
            controlReverseAddTo.Enabled = false;
            allowHP = new Control();
            allowHP.Enabled = false;
            allowRF = new Control();
            allowRF.Enabled = false;
            allowCash = new Control();
            allowCash.Enabled = false;
            allowStore = new Control();
            allowStore.Enabled = false;

            dynamicMenus[this.Name + ":allowHP"] = this.allowHP;
            dynamicMenus[this.Name + ":allowRF"] = this.allowRF;
            dynamicMenus[this.Name + ":allowCash"] = this.allowCash;
            dynamicMenus[this.Name + ":allowStoreCard"] = this.allowStore;
            dynamicMenus[this.Name + ":menuSpecialAccount"] = this.menuSpecialAccount;
            dynamicMenus[this.Name + ":menuManualHP"] = this.menuManualHP;
            dynamicMenus[this.Name + ":menuManualCash"] = this.menuManualCash;
            dynamicMenus[this.Name + ":btnSave"] = this.btnSave;
            dynamicMenus[this.Name + ":menuSave"] = this.menuSave;
            dynamicMenus[this.Name + ":menuRefer"] = this.menuRefer;
            dynamicMenus[this.Name + ":menuRevise"] = this.menuRevise;
            dynamicMenus[this.Name + ":menuReviseCashLoan"] = this.menuReviseCashLoan;
            dynamicMenus[this.Name + ":menuSanction"] = this.menuSanction;
            dynamicMenus[this.Name + ":menuS1"] = this.menuS1;
            dynamicMenus[this.Name + ":menuS2"] = this.menuS2;
            dynamicMenus[this.Name + ":menuDC"] = this.menuDC;
            dynamicMenus[this.Name + ":menuUW"] = this.menuUW;
            dynamicMenus[this.Name + ":controlAddTo"] = this.controlAddTo;
            dynamicMenus[this.Name + ":controlReverseAddTo"] = this.controlReverseAddTo;
            dynamicMenus[this.Name + ":menuUnblockCredit"] = this.menuUnblockCredit;
            dynamicMenus[this.Name + ":allowReviseSettled"] = this.allowReviseSettled;
            dynamicMenus[this.Name + ":allowReviseRepo"] = this.allowReviseRepo;
            dynamicMenus[this.Name + ":allowReviseCash"] = this.allowReviseCash;
            dynamicMenus[this.Name + ":allowManual"] = this.allowManual;
            dynamicMenus[this.Name + ":lEmploymentDetails"] = this.lEmploymentDetails;
            dynamicMenus[this.Name + ":lBankDetails"] = this.lBankDetails;
            dynamicMenus[this.Name + ":drpBranch"] = this.drpBranch;
            dynamicMenus[this.Name + ":lBranch"] = this.lBranch;
            dynamicMenus[this.Name + ":lResidentialDetails"] = this.lResidentialDetails;
            dynamicMenus[this.Name + ":lFinancialDetails"] = this.lFinancialDetails;
            dynamicMenus[this.Name + ":btnPrevious"] = this.btnPrevious;
            dynamicMenus[this.Name + ":HCMembershipEnd"] = this.LoyaltyEnd_dtp;
            //dynamicMenus[this.Name + ":LoyaltyVoucher_btn"] = this.LoyaltyVoucher_btn; --IP - 20/04/10 - UAT(79) UAT5.2 - Commented out as button should always be visible.
        }

        public BasicCustomerDetails(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuCustomer });

            //Remove this line when version 5.2
            //tcDetails.TabPages.Remove(tpPhoto);
        }


        // Blank constructor used for creating a new customer independent of account
        public BasicCustomerDetails(bool canCreateRF, Form root, Form parent)
        {
            try
            {
                Function = "BasicCustomerDetails";
                Wait();
                ReadOnly = false;
                InitializeComponent();
                FormRoot = root;
                FormParent = parent;
                DimCreditFieldsIfService();
                menuMain = new Crownwood.Magic.Menus.MenuControl();
                menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuCustomer });
                dynamicMenus = new Hashtable();
                HashMenus();
                ApplyRoleRestrictions();
                _canCreateRF = canCreateRF;
                // btnCreateRFAccount.Enabled = canCreateRF;
                btnCreateRFAccount.Enabled = canCreateRF && CheckAge() && CheckAccountRelationship();   //IP - 11/05/10 - UAT(141) UAT5.2.1.0 log - Merged from 4.3
                txtRFLimit.Visible = canCreateRF;
                txtRFAvailable.Visible = canCreateRF;
                labelCredit.Visible = canCreateRF;
                labelAvailable.Visible = canCreateRF;
                txtRFIssueNo.Visible = canCreateRF;
                lRFIssueNo.Visible = canCreateRF;

                dtCurrEmpStart.DateFrom = DateTime.Now;
                dtCurrEmpStart.Value = DateTime.Now;
                dtPrevEmpStart.DateFrom = DateTime.Now;
                dtPrevEmpStart.Value = DateTime.Now;

                //KEF added for 3.5.2.0 pre-release UAT issue 20
                dtBankOpened.DateFrom = DateTime.Now;
                dtBankOpened.Value = DateTime.Now;

                ConfigureTabControl();

                grpAccounts.Enabled = false;
                loadStatic();
                int index = 0;
                foreach (DataRow row in ((DataTable)drpRelationship.DataSource).Rows)
                {
                    if ((string)row["code"] == "H")
                    {
                        OldRelationshipIndex = drpRelationship.SelectedIndex = index;
                        break;
                    }

                    index++;
                }

                this.Relationship = "H";
                //cannot create related customers at this stage because 
                //there will be no accounts
                drpRelationship.Enabled = false;
                txtAccountNo.BackColor = SystemColors.Window;
                txtRFLimit.BackColor = Color.RosyBrown;
                txtRFLimit.Text = (0).ToString(DecimalPlaces);
                txtRFAvailable.BackColor = Color.RosyBrown;
                txtRFIssueNo.BackColor = Color.RosyBrown;
                txtBand.BackColor = Color.RosyBrown;
                txtRFAvailable.Text = (0).ToString(DecimalPlaces);
                txtDependants.Value = 0;
                menuManualRF.Enabled = allowManual.Enabled;
                menuManualRF.Visible = allowManual.Enabled;

                CheckAge();                 //IP - 11/05/10 - UAT(141) UAT5.2.1.0 log - Merged from 4.3
                LoyaltyShow();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of BasicCustomerDetails";
            }
        }

        //Second constructor used for creating a new customer to be tied
        //to a specific account
        public BasicCustomerDetails(bool canCreateRF, string accountNo, string accountType, Form root, Form parent)
        {
            try
            {
                Function = "BasicCustomerDetails";
                Wait();
                ReadOnly = false;
                InitializeComponent();
                FormRoot = root;
                FormParent = parent;
                menuMain = new Crownwood.Magic.Menus.MenuControl();
                menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuCustomer });
                dynamicMenus = new Hashtable();
                HashMenus();
                ApplyRoleRestrictions();
                _canCreateRF = canCreateRF;
                //btnCreateRFAccount.Enabled = canCreateRF;
                btnCreateRFAccount.Enabled = canCreateRF && CheckAge() && CheckAccountRelationship();       //IP - 11/05/10 - UAT(141) UAT5.2.1.0 log - Merged from 4.3
                btnCreateStoreCard.Enabled = CheckAge() && !existingStoreCard;
                StorecardButtonCheckEnabled();
                txtRFLimit.Visible = canCreateRF;
                txtRFAvailable.Visible = canCreateRF;
                labelCredit.Visible = canCreateRF;
                labelAvailable.Visible = canCreateRF;
                txtRFIssueNo.Visible = canCreateRF;
                lRFIssueNo.Visible = canCreateRF;

                txtRFLimit.BackColor = Color.RosyBrown;
                txtRFLimit.Text = (0).ToString(DecimalPlaces);

                txtRFAvailable.BackColor = Color.RosyBrown;
                txtRFAvailable.Text = (0).ToString(DecimalPlaces);
                txtRFIssueNo.BackColor = Color.RosyBrown;
                txtBand.BackColor = Color.RosyBrown;

                dtCurrEmpStart.DateFrom = DateTime.Now;
                dtCurrEmpStart.Value = DateTime.Now;
                dtPrevEmpStart.DateFrom = DateTime.Now;
                dtPrevEmpStart.Value = DateTime.Now;

                //KEF added for 3.5.2.0 pre-release UAT issue 20
                dtBankOpened.DateFrom = DateTime.Now;
                dtBankOpened.Value = DateTime.Now;

                ConfigureTabControl();
                grpAccounts.Enabled = false;
                loadStatic();

                NewAccountNo = AccountNo = accountNo = accountNo.Replace("-", "");
                txtAccountNo.Text = FormatAccountNo(accountNo);
                AccountType = accountType;
                NewAccountType = accountType;

                txtAccountNo.ReadOnly = true;
                txtAccountNo.BackColor = SystemColors.Window;
                drpRelationship.Enabled = false;

                menuManualRF.Enabled = allowManual.Enabled;
                menuManualRF.Visible = allowManual.Enabled;

                int index = 0;
                foreach (DataRow row in ((DataTable)drpRelationship.DataSource).Rows)
                {
                    if ((string)row["code"] == "H")
                    {
                        OldRelationshipIndex = drpRelationship.SelectedIndex = index;
                        break;
                    }

                    index++;
                }

                this.Relationship = "H";

                if (!btnSave.Enabled)
                    SetCashAndGo();

                //Remove this line when version 5.2
                //tcDetails.TabPages.Remove(tpPhoto);
                LoyaltyShow();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of BasicCustomerDetails";
            }
        }

        //constructor for when we only have the customer ID
        public BasicCustomerDetails(bool canCreateRF, string custid, Form root, Form parent)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuCustomer });
            dynamicMenus = new Hashtable();
            HashMenus();
            ApplyRoleRestrictions();
            _canCreateRF = canCreateRF;
            //btnCreateRFAccount.Enabled = canCreateRF;
            btnCreateRFAccount.Enabled = canCreateRF && CheckAge() && CheckAccountRelationship();   //IP - 11/05/10 - UAT(141) UAT5.2.1.0 log - Merged from 4.3
            //            btnCreateStoreCard.Enabled = CheckAge() && !existingStoreCard;
            txtRFLimit.Visible = canCreateRF;
            txtRFAvailable.Visible = canCreateRF;
            labelCredit.Visible = canCreateRF;
            labelAvailable.Visible = canCreateRF;
            txtRFIssueNo.Visible = canCreateRF;
            lRFIssueNo.Visible = canCreateRF;

            CustomerID = custid;

            ConfigureTabControl();

            //Remove this line when version 5.2
            //tcDetails.TabPages.Remove(tpPhoto);

            try
            {
                Function = "BasicCustomerDetails()";
                Wait();

                this.ReadOnly = !btnSave.Enabled;
                this.SetReadOnly();

                txtRFLimit.BackColor = Color.RosyBrown;
                txtRFLimit.Text = (0).ToString(DecimalPlaces);
                txtRFAvailable.BackColor = Color.RosyBrown;
                txtRFAvailable.Text = (0).ToString(DecimalPlaces);
                txtRFIssueNo.BackColor = Color.RosyBrown;
                txtBand.BackColor = Color.RosyBrown;

                toolTip1.SetToolTip(txtFirstName, txtFirstName.Text);
                toolTip1.SetToolTip(txtLastName, txtLastName.Text);
                toolTip1.SetToolTip(txtAlias, txtAlias.Text);

                menuManualRF.Enabled = allowManual.Enabled;
                menuManualRF.Visible = allowManual.Enabled;

                dtCurrEmpStart.DateFrom = DateTime.Now;
                dtCurrEmpStart.Value = DateTime.Now;
                dtPrevEmpStart.DateFrom = DateTime.Now;
                dtPrevEmpStart.Value = DateTime.Now;

                //KEF added for 3.5.2.0 pre-release UAT issue 20
                dtBankOpened.DateFrom = DateTime.Now;
                dtBankOpened.Value = DateTime.Now;

                loadStatic();
                int index = 0;
                foreach (DataRow row in ((DataTable)drpRelationship.DataSource).Rows)
                {
                    if ((string)row["code"] == "H")
                    {
                        OldRelationshipIndex = drpRelationship.SelectedIndex = index;
                        break;
                    }

                    index++;
                }
                this.Relationship = "H";
                //they may not have any accounts
                drpRelationship.Enabled = false;
                loadDetails(custid, String.Empty, "H", ReadOnly);
                LoyaltyShow();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of BasicCustomerDetails";
            }
        }

        private void SetReadOnly()
        {
            btnSave.Enabled = !ReadOnly;
            menuSave.Enabled = !ReadOnly;
            lNewAddress.Enabled = !ReadOnly;
            drpAddressType.Enabled = !ReadOnly;
            btnAdd.Enabled = !ReadOnly;
            btnRemove.Enabled = !ReadOnly;

            txtCustID.ReadOnly = ReadOnly;
            txtCustID.BackColor = SystemColors.Window;

            txtAccountNo.ReadOnly = true;
            txtAccountNo.BackColor = SystemColors.Window;

            txtFirstName.ReadOnly = ReadOnly;
            txtFirstName.BackColor = SystemColors.Window;

            txtLastName.ReadOnly = ReadOnly;
            txtLastName.BackColor = SystemColors.Window;

            txtAlias.ReadOnly = ReadOnly;
            txtAlias.BackColor = SystemColors.Window;

            drpTitle.Enabled = !ReadOnly;
            dtDOB.Enabled = !ReadOnly;

        }

        //constructor for when we have the customer ID and the accountNo.
        public BasicCustomerDetails(bool canCreateRF, string custid, string accountNo, string custRelationship, string accountType, Form root, Form parent)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuCustomer });
            dynamicMenus = new Hashtable();
            HashMenus();
            ApplyRoleRestrictions();
            _canCreateRF = canCreateRF;
            //btnCreateRFAccount.Enabled = canCreateRF;
            //     btnCreateRFAccount.Enabled = canCreateRF && CheckAge() && CheckAccountRelationship();           //IP - 11/05/10 - UAT(141) UAT5.2.1.0 log - Merged from 4.3
            //     btnCreateStoreCard.Enabled = CheckAge() && !existingStoreCard;
            txtRFLimit.Visible = canCreateRF;
            txtRFAvailable.Visible = canCreateRF;
            labelCredit.Visible = canCreateRF;
            labelAvailable.Visible = canCreateRF;
            txtRFIssueNo.Visible = canCreateRF;
            lRFIssueNo.Visible = canCreateRF;
            CustomerID = custid;

            ConfigureTabControl();

            //Remove this line when version 5.2
            //tcDetails.TabPages.Remove(tpPhoto);

            try
            {
                Function = "BasicCustomerDetails()";
                Wait();

                this.ReadOnly = !btnSave.Enabled;
                this.SetReadOnly();

                txtRFLimit.BackColor = Color.RosyBrown;
                txtRFLimit.Text = (0).ToString(DecimalPlaces);
                txtRFAvailable.BackColor = Color.RosyBrown;
                txtRFAvailable.Text = (0).ToString(DecimalPlaces);
                txtRFIssueNo.BackColor = Color.RosyBrown;
                txtBand.BackColor = Color.RosyBrown;

                toolTip1.SetToolTip(txtFirstName, txtFirstName.Text);
                toolTip1.SetToolTip(txtLastName, txtLastName.Text);
                toolTip1.SetToolTip(txtAlias, txtAlias.Text);

                loadStatic();

                AccountType = accountType;
                AccountNo = accountNo = accountNo.Replace("-", "");
                txtAccountNo.Text = FormatAccountNo(accountNo);

                dtCurrEmpStart.DateFrom = DateTime.Now;
                dtCurrEmpStart.Value = DateTime.Now;
                dtPrevEmpStart.DateFrom = DateTime.Now;
                dtPrevEmpStart.Value = DateTime.Now;

                //KEF added for 3.5.2.0 pre-release UAT issue 20
                dtBankOpened.DateFrom = DateTime.Now;
                dtBankOpened.Value = DateTime.Now;

                txtAccountNo.ReadOnly = true;
                int index = 0;
                foreach (DataRow row in ((DataTable)drpRelationship.DataSource).Rows)
                {
                    if ((string)row["code"] == "H")
                    {
                        OldRelationshipIndex = drpRelationship.SelectedIndex = index;
                        break;
                    }
                    index++;
                }
                this.Relationship = custRelationship;

                loadDetails(custid, accountNo, custRelationship, ReadOnly);

                CashLoanCheckQual(custid);      // #14429

                menuManualRF.Enabled = allowManual.Enabled;
                menuManualRF.Visible = allowManual.Enabled;

                if (!btnSave.Enabled)
                    SetCashAndGo();
                LoyaltyShow();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of BasicCustomerDetails";
            }
        }

        private void ConfigureTabControl()
        {
            tcAddress.Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiForm;
            tcAddress.PositionTop = true;
        }

        private void LoadDetailsThread()
        {
            try
            {
                Wait();
                Function = "LoadDetailsThread";
                bool privilegeClub = false;
                string privilegeClubCode = String.Empty;
                string privilegeClubDesc = String.Empty;
                bool hasDiscount = false;

                basicDetails = CustomerManager.GetBasicCustomerDetails(ThreadCustomerID, ThreadAccountNo, ThreadRelationship, out Error);
                if (Error.Length > 0)
                    ShowError(Error);

                else
                {
                    /* retrieve the privilege club stuff */
                    if ((bool)Country[CountryParameterNames.LoyaltyCard])
                    {
                        CustomerManager.IsPrivilegeMember(ThreadCustomerID, out privilegeClub, out privilegeClubCode, out privilegeClubDesc, out hasDiscount, out Error);
                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            chxPrivilegeClub.Checked = privilegeClub;
                            // jec 05/12/07 lPrivilegeClubDesc.Text = privilegeClubDesc;
                            btnPrivilegeClub.Visible = privilegeClub;        // jec 05/12/07
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
                Function = "End of LoadDetailsThread";
            }
        }

        private void loadDetails(string custID, string accountNo, string relationship, bool readOnly)
        {
            Function = "LoadDetails";

            try
            {
                DuplicateCustid = false;
                errorProvider1.SetError(txtCustID, String.Empty);

                this.CustomerID = String.Empty;
                storeCard = new Blue.Cosacs.Shared.StoreCard();
                customer = new CustomerResult();
                ClearControls(this.gbEmployment.Controls);
                this.dtPrevEmpStart.Years = 0;
                this.dtPrevEmpStart.Months = 0;
                ClearControls(this.gbBank.Controls);

                ThreadCustomerID = custID;
                ThreadAccountNo = accountNo;
                ThreadRelationship = relationship;

                LoyaltyGetDetails(custID); // CR1017 HomeClub

                LoadDetailsThread();

                ClearControls(tpFinancial.Controls);
                ClearControls(tpResidential.Controls);
                // CR903 - store the type of customer this is (Courts or Non Courts).

                if (basicDetails.Tables.Count > 0)
                {
                    if (basicDetails.Tables["BasicDetails"].Rows.Count > 0)
                    {

                        _customerStoreType = (string)basicDetails.Tables["BasicDetails"].Rows[0][CN.StoreType];

                        customer.blacklisted = Convert.ToBoolean(basicDetails.Tables["BasicDetails"].Rows[0]["Blacklisted"]); //IP - 01/09/09 - UAT(823)
                        txtStoreCardAvailable.Text = (string)basicDetails.Tables["BasicDetails"].Rows[0]["StoreCardAvailable"];
                        //Ttip.SetToolTip(txtStoreCardAvailable,"StoreCard Limit:" + (string)basicDetails.Tables["BasicDetails"].Rows[0]["StoreCardLimit"]);
                        customer.StoreCardLimit = Convert.ToDecimal(basicDetails.Tables["BasicDetails"].Rows[0]["StoreCardLimit"]);
                        customer.StoreCardAvailable = Convert.ToDecimal(basicDetails.Tables["BasicDetails"].Rows[0]["StoreCardAvailable"]); //IP - 21/12/10 - Store Card

                        customer.StoreCardApproved = Convert.ToBoolean(basicDetails.Tables["BasicDetails"].Rows[0]["StoreCardApproved"]);
                        StorecardButtonCheckEnabled();
                        //IP - 21/12/10 - Set the store card fields if the customer has a Store Card account
                        SetStoreCardFields(existingStoreCard);
                    }

                    if (basicDetails.Tables.IndexOf(TN.CustomerAddresses) >= 0)
                    {
                        DataView dvAddresses = basicDetails.Tables[TN.CustomerAddresses].DefaultView;
                        //71522 need all date ins
                        //dvAddresses.RowFilter = "AddressType = 'H'";
                        //m_currentDateIn = (DateTime)dvAddresses.Table.Rows[0]["Date In"];
                        foreach (DataRow row in dvAddresses.Table.Rows)
                        {
                            addresshistory.Add(drpRelationship.SelectedValue.ToString(), row["AddressType"].ToString(), (DateTime)row["Date In"]);
                        }
                    }
                }

                if (basicDetails != null)
                {
                    foreach (DataTable dt in basicDetails.Tables)
                    {
                        switch (dt.TableName)
                        {
                            case "BasicDetails":
                                loadBasics(dt);
                                break;

                            case TN.CustomerAddresses:
                                loadAddresses(readOnly, dt);
                                break;

                            case TN.Employment:
                                loadEmployment(dt);
                                break;

                            case TN.Bank:
                                loadBank(dt);
                                break;

                            case TN.CustomerAdditionalDetailsFinancial:
                                LoadFinancialData(dt);
                                break;

                            case TN.CustomerAdditionalDetailsResidential:
                                LoadResidentialData(dt);
                                break;

                            default: //if default do nothing. So what's the diference between this and having no code to handl the default?
                                break;
                        }
                    }

                    this.SumExpenditure();

                    /* no need to load the accounts unless we have retrieved a 
                     * customer, otherwise it will bring back 250 accounts (i.e. the results of a blank search) */
                    if (basicDetails.Tables.Count > 0)
                    {
                        loadAccounts(false);
                        existingCustomer = true;
                    }
                    else
                    {
                        existingCustomer = false;
                    }

                    if (tcAddress.TabPages.Count > 0)
                    {
                        tcAddress.SelectedTab = tcAddress.TabPages[0];
                        currentTab = tcAddress.SelectedTab;
                    }

                    // Load customer changes audit tab
                    this.LoadCustomerAudit();
                    // Load address history tab
                    this.LoadAddressHistory();
                    SetUpAccountTypeVisibility();
                }
                else
                {
                    if (Relationship == "H")
                    {
                        txtAccountNo.Text = "000-0000-0000-0";
                    }

                    existingCustomer = false;
                }

                this._hasdatachanged = false;

            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function = "End of LoadDetails";
            }
        }

        private void loadBank(DataTable dt)
        {
            ClearControls(this.gbBank.Controls);
            #region Old loadBank code - pre CR 835
            /*BankDetails = dt;
            int index = 0;

            drpBankAcctType.SelectedIndex = 0;
            drpBank.SelectedIndex = 0;

            drpBankAcctType.DisplayMember = CN.Code;
            drpBank.DisplayMember = CN.BankCode;

            foreach (DataRow row in dt.Rows)		//should only be one row
            {
                index = drpBankAcctType.FindStringExact(((string)row[CN.Code]).Trim());
                if (index >= 0)
                    drpBankAcctType.SelectedIndex = index;

                index = drpBank.FindStringExact(((string)row[CN.BankCode]).Trim());
                if (index >= 0)
                    drpBank.SelectedIndex = index;

                if ((DateTime)row[CN.BankAccountOpened] > DatePicker.MinValue)
                {
                    dtBankOpened.DateFrom = DateTime.Today;
                    dtBankOpened.Value = (DateTime)row[CN.BankAccountOpened];
                }
                txtBankAcctNumber.Text = (string)row[CN.BankAccountNo];
            }
            drpBank.DisplayMember = CN.BankName;
            drpBankAcctType.DisplayMember = CN.CodeDescription;
            */
            #endregion
            //Bank data is now loaded into the financials tab
            int index = 0;
            BankDetails = dt;

            drpBankAcctType1.SelectedIndex = 0;
            drpBank1.SelectedIndex = 0;

            drpBankAcctType1.DisplayMember = CN.Code;
            drpBank1.DisplayMember = CN.BankCode;

            foreach (DataRow row in dt.Rows)		//should only be one row
            {
                index = drpBankAcctType1.FindStringExact(((string)row[CN.Code]).Trim());
                if (index >= 0)
                    drpBankAcctType1.SelectedIndex = index;

                index = drpBank1.FindStringExact(((string)row[CN.BankCode]).Trim());
                if (index >= 0)
                    drpBank1.SelectedIndex = index;

                if ((DateTime)row[CN.BankAccountOpened] > DatePicker.MinValue)
                {
                    dtBankOpened1.DateFrom = DateTime.Today;
                    dtBankOpened1.Value = (DateTime)row[CN.BankAccountOpened];
                }
                txtBankAcctNumber1.Text = (string)row[CN.BankAccountNo];
            }
            drpBank1.DisplayMember = CN.BankName;
            drpBankAcctType1.DisplayMember = CN.CodeDescription;
        }

        private void loadEmployment(DataTable dt)
        {
            ClearControls(this.gbEmployment.Controls);
            this.dtPrevEmpStart.Years = 0;
            this.dtPrevEmpStart.Months = 0;
            EmploymentDetails = dt;
            int index = 0;
            drpEmploymentStat.SelectedIndex = 0;
            drpOccupation.SelectedIndex = 0;
            drpPayFrequency.SelectedIndex = 0;

            drpEmploymentStat.DisplayMember = CN.Code;
            drpOccupation.DisplayMember = CN.Code;
            drpPayFrequency.DisplayMember = CN.Code;
            foreach (DataRow row in dt.Rows)		//should only be one row...if so, what's the foreach then?!?!?!?!?!
            {
                index = drpEmploymentStat.FindStringExact(((string)row[CN.EmploymentStatus]).Trim());
                if (index >= 0)
                    drpEmploymentStat.SelectedIndex = index;

                //CR 866 Changed CN.Occupation to CN.WorkType
                index = drpOccupation.FindStringExact(((string)row[CN.WorkType]).Trim());
                if (index >= 0)
                    drpOccupation.SelectedIndex = index;

                index = drpPayFrequency.FindStringExact(((string)row[CN.PayFrequency]).Trim());
                if (index >= 0)
                    drpPayFrequency.SelectedIndex = index;

                SetIncome(true);

                dtCurrEmpStart.DateFrom = DateTime.Today;
                if ((DateTime)row[CN.DateEmployed] > DatePicker.MinValue)
                    dtCurrEmpStart.Value = (DateTime)row[CN.DateEmployed];
                else
                    dtCurrEmpStart.Value = DateTime.Today;
                dtCurrEmpStart.LinkedDatePicker = dtPrevEmpStart;

                dtPrevEmpStart.DateFrom = dtCurrEmpStart.Value;
                if ((DateTime)row[CN.PrevDateEmployed] > DatePicker.MinValue)
                    dtPrevEmpStart.Value = (DateTime)row[CN.PrevDateEmployed];
                else
                    dtPrevEmpStart.Value = dtCurrEmpStart.Value;

                txtEmpTelCode.Text = ((string)row[CN.PersDialCode]).Trim();
                txtEmpTelNum.Text = ((string)row[CN.PersTel]).Trim();

                if (row[CN.AnnualGross] != DBNull.Value)
                    txtIncome.Text = ((double)row[CN.AnnualGross] / 12).ToString(DecimalPlaces);

                //CR 866 - Thailand scoring [PC]
                //CR 866b
                txtJobTitle1.SelectedValue = row[CN.JobTitle].ToString();
                txtOrganisation1.SelectedValue = row[CN.Organisation].ToString();
                txtIndustry1.SelectedValue = row[CN.Industry].ToString();
                drpEductation1.SelectedValue = row[CN.EducationLevel].ToString();
                //End CR 866
            }

            drpEmploymentStat.DisplayMember = CN.CodeDescription;
            drpOccupation.DisplayMember = CN.CodeDescription;
            drpPayFrequency.DisplayMember = CN.CodeDescription;
        }

        private void loadBasics(DataTable dt)
        {
            Function = "LoadBasics";
            int index = 0;
            foreach (DataRow row in dt.Rows)//how many basic details a customers has?
            {

                chkSms.Checked = (bool)row["ResieveSms"];
                if (row["FirstName"] != DBNull.Value)
                {
                    txtFirstName.Text = (string)row["FirstName"];
                    sc_name = customer.firstname = txtFirstName.Text;
                }

                if (row["LastName"] != DBNull.Value)
                    sc_lastname = txtLastName.Text = (string)row["LastName"];

                if (row["Alias"] != DBNull.Value)
                    txtAlias.Text = (string)row["Alias"];

                if (row["Title"] != DBNull.Value)
                {
                    index = drpTitle.FindString((string)row["Title"]);
                    sc_title = (string)row["Title"];
                }

                if (index != -1)
                    drpTitle.SelectedIndex = index;

                if (row["CustomerID"] != DBNull.Value)
                {
                    this.CustomerID = txtCustID.Text = (string)row["CustomerID"];
                    StageToLaunch.CustID = this.CustomerID;
                    existingCustomer = true;
                }

                if (this.Relationship == "H")
                    this.Holder = this.CustomerID;
                else
                    this.Linked = this.CustomerID;

                if (row[CN.RFCreditLimit] != DBNull.Value)
                    txtRFLimit.Text = ((decimal)row[CN.RFCreditLimit]).ToString(DecimalPlaces);

                if (row[CN.AvailableCredit] != DBNull.Value)
                {
                    if ((decimal)row[CN.AvailableCredit] > 0)
                        txtRFAvailable.Text = ((decimal)row[CN.AvailableCredit]).ToString(DecimalPlaces);
                    else
                        txtRFAvailable.Text = (0).ToString(DecimalPlaces);
                }

                if (row[CN.RFCardSeqNo] != DBNull.Value)
                    txtRFIssueNo.Text = ((byte)row[CN.RFCardSeqNo]).ToString();
                else
                    txtRFIssueNo.Text = "0";

                if (row[CN.DOB] != DBNull.Value)
                    dtDOB.Value = (DateTime)row[CN.DOB];

                if (row[CN.MaidenName] != DBNull.Value)
                    txtMaidenName.Text = (string)row[CN.MaidenName];

                if (txtLoyaltyCardNo.Enabled && row[CN.MoreRewardsNo] != DBNull.Value)
                    txtLoyaltyCardNo.Text = (string)row[CN.MoreRewardsNo];

                if (row[CN.LimitType] != DBNull.Value)
                {
                    if ((string)row[CN.LimitType] == "P")
                    {
                        txtRFAvailable.Text = (0).ToString(DecimalPlaces);
                        txtRFLimit.Text = (0).ToString(DecimalPlaces);
                    }
                }

                // Scoring Band
                if (row[CN.Band] != DBNull.Value)
                    this.txtBand.Text = (string)row[CN.Band];
                else
                    this.txtBand.Text = "";

                if (row[CN.MaritalStatus] != DBNull.Value)
                    foreach (DataRowView r in drpMaritalStat1.Items)
                    {
                        if ((string)r[CN.Code] == ((string)row[CN.MaritalStatus]).Trim())
                        {
                            drpMaritalStat1.SelectedItem = r;
                            break;
                        }
                    }

                if (row[CN.Dependants] != DBNull.Value)
                    txtDependants.Value = Convert.ToDecimal(row[CN.Dependants]);


                if (row[CN.Nationality] != DBNull.Value)
                    drpNationality1.SelectedValue = row[CN.Nationality];

                chkSms.Checked = (bool)row["ResieveSms"];

                if (row[CN.IsSpouseWorking] != DBNull.Value)
                    IsSpouseWorking = (bool)row[CN.IsSpouseWorking];
                else
                    IsSpouseWorking = false;

                if (row[CN.DependantsFromProposal] != DBNull.Value)
                    DependantsFromProposal = (int)row[CN.DependantsFromProposal];
                else
                    DependantsFromProposal = 0;

                if (row[CN.MaritalStatus] != DBNull.Value)
                    maritalStatusFromProposal = ((string)row[CN.MaritalStatus]).Trim();
                else
                    maritalStatusFromProposal = string.Empty;
            }

            Function = "End of LoadBasics";
        }

        private void LoadStaticThread()
        {
            try
            {
                Wait();
                Function = "LoadStaticThread";

                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                if (StaticData.Tables[TN.Title] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Title, new string[] { "TTL", "L" }));
                if (StaticData.Tables[TN.CustomerRelationship] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CustomerRelationship, new string[] { "LCT", "L" }));
                if (StaticData.Tables[TN.AddressType] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.AddressType, null));
                if (StaticData.Tables[TN.EmploymentStatus] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.EmploymentStatus, new string[] { "ES1", "L" }));
                if (StaticData.Tables[TN.Occupation] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Occupation, new string[] { "WT1", "L" }));
                if (StaticData.Tables[TN.PayFrequency] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.PayFrequency, new string[] { "PF1", "L" }));
                if (StaticData.Tables[TN.Bank] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Bank, null));
                if (StaticData.Tables[TN.BankAccountType] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BankAccountType, new string[] { "BA2", "L" }));
                if (StaticData.Tables[TN.BranchNumber] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));
                if (StaticData.Tables[TN.Villages] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Villages, null));

                if (dropDowns.DocumentElement.ChildNodes.Count > 0)
                {
                    //DataSet ds = drop.GetDropDownData(dropDowns.DocumentElement, out Error);
                    DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                            StaticData.Tables[dt.TableName] = dt;
                    }
                }
                //CR 835 Load static data for the financial tab
                drpBank1.DataSource = (DataTable)StaticData.Tables[TN.Bank];
                drpBank1.DisplayMember = CN.BankName;

                drpBankAcctType1.DataSource = (DataTable)StaticData.Tables[TN.BankAccountType];
                drpBankAcctType1.DisplayMember = CN.CodeDescription;

                drpGiroDueDate1.DataSource = (DataTable)StaticData.Tables[TN.DDDueDate];
                drpGiroDueDate1.DisplayMember = CN.DueDay;

                drpPaymentMethod.DataSource = (DataTable)StaticData.Tables[TN.MethodOfPayment];
                drpPaymentMethod.DisplayMember = CN.CodeDescription;

                drpPayByGiro1.DataSource = new string[] { "", GetResource("No"), GetResource("Yes") };
                //End CR 835 Changes

                drpEmploymentStat.DataSource = (DataTable)StaticData.Tables[TN.EmploymentStatus];
                drpEmploymentStat.DisplayMember = CN.CodeDescription;

                drpOccupation.DataSource = (DataTable)StaticData.Tables[TN.Occupation];
                drpOccupation.DisplayMember = CN.CodeDescription;

                drpPayFrequency.DataSource = (DataTable)StaticData.Tables[TN.PayFrequency];
                drpPayFrequency.DisplayMember = CN.CodeDescription;

                drpBank.DataSource = (DataTable)StaticData.Tables[TN.Bank];
                drpBank.DisplayMember = CN.BankName;

                drpBankAcctType.DataSource = (DataTable)StaticData.Tables[TN.BankAccountType];
                drpBankAcctType.DisplayMember = CN.CodeDescription;

                DataTable branches = ((DataTable)StaticData.Tables[TN.BranchNumber]).Copy();
                drpBranch.DataSource = branches.DefaultView;
                drpBranch.DisplayMember = CN.BranchNo;

                //CR 866
                mandatoryFieldsDS = StaticDataManager.GetMandatoryFields(Config.CountryCode, "SanctionStage1", out Error);

                //END CR 866

                int i = drpBranch.FindString(Config.BranchCode);
                if (i != -1) drpBranch.SelectedIndex = i;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of LoadStaticThread";
            }
        }

        private void loadStatic()
        {
            Function = "LoadStatic";

            staticLoaded = false;

            Thread dataThread = new Thread(new ThreadStart(LoadStaticThread));
            dataThread.Start();
            dataThread.Join();

            drpTitle.DataSource = (DataTable)StaticData.Tables[TN.Title];
            drpTitle.DisplayMember = CN.CodeDescription;

            drpRelationship.DataSource = (DataTable)StaticData.Tables[TN.CustomerRelationship];
            drpRelationship.DisplayMember = CN.CodeDescription;
            drpRelationship.ValueMember = CN.Code;

            //CR 835 Added the following two drop downs to the main screen [Peter Chong] 06-Oct-2006
            drpMaritalStat1.DataSource = (DataTable)StaticData.Tables[TN.MaritalStatus];
            drpMaritalStat1.DisplayMember = CN.CodeDescription;
            drpMaritalStat1.ValueMember = CN.Code;

            drpNationality1.DataSource = (DataTable)StaticData.Tables[TN.Nationality];
            drpNationality1.DisplayMember = CN.CodeDescription;
            drpNationality1.ValueMember = CN.Code;

            DisableAll();

            // Mobile Nos should not be displayed as a seperate address type.
            DataTable addTypes = ((DataTable)StaticData.Tables[TN.AddressType]).Copy();
            foreach (DataRow dr in addTypes.Rows)
            {
                string addType = ((string)dr[CN.Code]);
                if (addType.Equals("M") || addType.Equals("M2") || addType.Equals("M3") || addType.Equals("M4"))
                    dr.Delete();
                if (addType.Equals("DM"))   // Address Standardization CR2019 - 025
                    dr.Delete();
                if (addType.Equals("D1M"))   // Address Standardization CR2019 - 025
                    dr.Delete();
                if (addType.Equals("D2M"))   // Address Standardization CR2019 - 025
                    dr.Delete();
                if (addType.Equals("D3M"))   // Address Standardization CR2019 - 025
                    dr.Delete();
            }


            drpAddressType.DataSource = addTypes;
            drpAddressType.DisplayMember = CN.CodeDescription;
            drpAddressType.SelectedIndex = (-1 == drpAddressType.FindString("H")) ? 0 : drpAddressType.FindString("H");

            txtLoyaltyCardNo.Visible = (bool)Country[CountryParameterNames.LoyaltyCard];
            lLoyaltyCardNo.Visible = (bool)Country[CountryParameterNames.LoyaltyCard];
            grpGiro.Visible = Convert.ToBoolean(Country[CountryParameterNames.DDEnabled]);

            //CR 835 Load static data for the residential tab
            drpCurrentResidentialStatus1.DataSource = (DataTable)StaticData.Tables[TN.ResidentialStatus];
            drpCurrentResidentialStatus1.DisplayMember = drpPrevResidentialStatus1.DisplayMember = CN.CodeDescription;
            drpCurrentResidentialStatus1.ValueMember = drpPrevResidentialStatus1.ValueMember = CN.Code;

            if(((DataTable)StaticData.Tables[TN.ResidentialStatus]) !=null)
                drpPrevResidentialStatus1.DataSource = ((DataTable)StaticData.Tables[TN.ResidentialStatus]).Copy();

            drpPropertyType1.DataSource = (DataTable)StaticData.Tables[TN.PropertyType];
            drpPropertyType1.DisplayMember = CN.CodeDescription;
            drpPropertyType1.ValueMember = CN.Code;

            //CR 866 Added drop downs 
            drpEductation1.DataSource = (DataTable)StaticData.Tables[TN.EducationLevels];
            drpEductation1.DisplayMember = CN.CodeDescription;
            drpEductation1.ValueMember = CN.Code;

            drpTransportType1.DataSource = (DataTable)StaticData.Tables[TN.TransportTypes];
            drpTransportType1.DisplayMember = CN.CodeDescription;
            drpTransportType1.ValueMember = CN.Code;

            //CR 866b Changed these to drop downs
            txtIndustry1.DataSource = (DataTable)StaticData.Tables[TN.Industries];
            txtIndustry1.DisplayMember = CN.CodeDescription;
            txtIndustry1.ValueMember = CN.Code;

            txtOrganisation1.DataSource = (DataTable)StaticData.Tables[TN.Organisations];
            txtOrganisation1.DisplayMember = CN.CodeDescription;
            txtOrganisation1.ValueMember = CN.Code;

            txtJobTitle1.DataSource = (DataTable)StaticData.Tables[TN.JobTitles];
            txtJobTitle1.DisplayMember = CN.CodeDescription;
            txtJobTitle1.ValueMember = CN.Code;
            //End CR 866

            //CR903 Find the store type of the selected branch
            SType = FindStoreType();

            // CR903 - Filter the branch drop down to only display
            // branches for the logged in store type.
            StoreFilter = BuildStoreTypeFilter();
            ((DataView)drpBranch.DataSource).RowFilter = StoreFilter;
            // CR903 req. because index not affected by filter, therefore drpBranch displays differnt branch
            int b = drpBranch.FindString(Config.BranchCode);
            if (b != -1) drpBranch.SelectedIndex = b;

            staticLoaded = true;
            // CR903 - CreateRF and Create Cash buttons and createHP etc only visible if 
            // allowed by Branch                jec 07/08/07
            drpBranch_SelectedIndexChanged(null, null);

            if (!Credential.HasPermission(CosacsPermissionEnum.CustDetailsViewBankDetails))
                gbBankDetails.Visible = false;

            if (!Credential.HasPermission(CosacsPermissionEnum.CustDetailsViewFinancialTab))
                tcDetails.TabPages.Remove(tpFinancial);


            if (!Credential.HasPermission(CosacsPermissionEnum.CustDetailsViewEmploymentDetails))
                tcDetails.TabPages.Remove(tpEmployment);

            if (!Credential.HasPermission(CosacsPermissionEnum.CustDetailsViewResidentialDetails))
                tcDetails.TabPages.Remove(tpResidential);

            Function = "End of LoadStatic";
        }

        private void loadAddresses(bool readOnly, DataTable dt)
        {
            Function = "LoadAddresses";
            StringCollection tabs = new StringCollection();
            string addType = String.Empty;
            string addDesc = String.Empty;
            string mobileNo = String.Empty;
            string mobileNo2 = String.Empty;
            string mobileNo3 = String.Empty;
            string mobileNo4 = String.Empty;

            // Address Standardization CR2019 - 025
            string deliveryMobile = String.Empty;
            string delivery1Mobile = String.Empty;
            string delivery2Mobile = String.Empty;
            string delivery3Mobile = String.Empty;


            //IP - 16/03/11 - #3317 - CR1245
            string workDialCode2 = string.Empty;
            string workNum2 = string.Empty;
            string workExt2 = string.Empty;

            string workDialCode3 = string.Empty;
            string workNum3 = string.Empty;
            string workExt3 = string.Empty;

            string workDialCode4 = string.Empty;
            string workNum4 = string.Empty;
            string workExt4 = string.Empty;
            // CFirstName =txtFileName.Text;
            foreach (DataRow row in dt.Rows)
            {
                if (DBNull.Value != row["AddressType"])
                    addType = ((string)row["AddressType"]).Trim();
                if (DBNull.Value != row["AddressDescription"])
                    addDesc = ((string)row["AddressDescription"]).Trim();

                if (addType == GetResource("L_MOBILE") && row["Phone"] != DBNull.Value)
                    mobileNo = ((string)row["Phone"]).Trim();

                if (addType == GetResource("L_MOBILE2") && row["Phone"] != DBNull.Value)
                    mobileNo2 = ((string)row["Phone"]).Trim();

                if (addType == GetResource("L_MOBILE3") && row["Phone"] != DBNull.Value)
                    mobileNo3 = ((string)row["Phone"]).Trim();

                if (addType == GetResource("L_MOBILE4") && row["Phone"] != DBNull.Value)
                    mobileNo4 = ((string)row["Phone"]).Trim();

                // Address Standardization CR2019 - 025
                if (addType == GetResource("L_DMOBILE") && row["Phone"] != DBNull.Value)
                    deliveryMobile = ((string)row["Phone"]).Trim();

                if (addType == GetResource("L_D1MOBILE") && row["Phone"] != DBNull.Value)
                    delivery1Mobile = ((string)row["Phone"]).Trim();

                if (addType == GetResource("L_D2MOBILE") && row["Phone"] != DBNull.Value)
                    delivery2Mobile = ((string)row["Phone"]).Trim();

                if (addType == GetResource("L_D3MOBILE") && row["Phone"] != DBNull.Value)
                    delivery3Mobile = ((string)row["Phone"]).Trim();



                //IP - 16/03/11 - #3317 - CR1245
                if (addType == GetResource("L_WORK2") && row["DialCode"] != DBNull.Value)
                    workDialCode2 = ((string)row["DialCode"]).Trim();

                if (addType == GetResource("L_WORK2") && row["Phone"] != DBNull.Value)
                    workNum2 = ((string)row["Phone"]).Trim();

                if (addType == GetResource("L_WORK2") && row["Ext"] != DBNull.Value)
                    workExt2 = ((string)row["Ext"]).Trim();

                if (addType == GetResource("L_WORK3") && row["DialCode"] != DBNull.Value)
                    workDialCode3 = ((string)row["DialCode"]).Trim();

                if (addType == GetResource("L_WORK3") && row["Phone"] != DBNull.Value)
                    workNum3 = ((string)row["Phone"]).Trim();

                if (addType == GetResource("L_WORK3") && row["Ext"] != DBNull.Value)
                    workExt3 = ((string)row["Ext"]).Trim();

                if (addType == GetResource("L_WORK4") && row["DialCode"] != DBNull.Value)
                    workDialCode4 = ((string)row["DialCode"]).Trim();

                if (addType == GetResource("L_WORK4") && row["Phone"] != DBNull.Value)
                    workNum4 = ((string)row["Phone"]).Trim();

                if (addType == GetResource("L_WORK4") && row["Ext"] != DBNull.Value)
                    workExt4 = ((string)row["Ext"]).Trim();

                //Don't add seperate Mobile tab 
                //IP - 16/03/11 - #3317 - CR1245 - Don't add seperate Work tabs for W2, W3, W4
                if (!tabs.Contains(addType) && addType != GetResource("L_MOBILE") && addType != GetResource("L_MOBILE2") && addType != GetResource("L_MOBILE3") && addType != GetResource("L_MOBILE4")
                    && addType != GetResource("L_WORK2") && addType != GetResource("L_WORK3") && addType != GetResource("L_WORK4") && addType != GetResource("L_DMOBILE") && addType != GetResource("L_D1MOBILE") && addType != GetResource("L_D2MOBILE") && addType != GetResource("L_D3MOBILE"))	//if this is a new tab
                {
                    tabs.Add(addType);
                    Crownwood.Magic.Controls.TabPage tp = new Crownwood.Magic.Controls.TabPage(addDesc);
                    currentTab = tp;
                    tp.Tag = false;
                    tp.BorderStyle = BorderStyle.Fixed3D;
                    AddressTab at = new AddressTab(readOnly, FormRoot, this, addType);     //CR1084 jec
                    if (addType.ToUpper() == "H")
                    {
                        at.onChangeAddressDate += new OnChangeAddressDate(at_onChangeAddressDate);

                    }
                    this.tcAddress.TabPages.Add(tp);
                    tp.Controls.Add(at);
                    tp.Name = "tp" + addType;
                }
                foreach (Crownwood.Magic.Controls.TabPage t in tcAddress.TabPages)
                {
                    if (t.Name == "tp" + addType)
                    {
                        if (((AddressTab)t.Controls[0]).txtAddress1.Text.Length == 0 &&
                            row["Address1"] != DBNull.Value)
                            ((AddressTab)t.Controls[0]).txtAddress1.Text = (string)row["Address1"];
                        if (((AddressTab)t.Controls[0]).cmbVillage.Items.Count > 0 &&
                            row["Address2"] != DBNull.Value) // Address Standardization CR2019 - 025
                        {
                            var villageIndex = ((AddressTab)t.Controls[0]).cmbVillage.FindStringExact((string)row["Address2"]);
                            if (villageIndex != -1)
                                ((AddressTab)t.Controls[0]).cmbVillage.SelectedIndex = villageIndex;
                            else
                                ((AddressTab)t.Controls[0]).cmbVillage.SelectedText = (string)row["Address2"];
                        }
                        else if (row["Address2"] != DBNull.Value)
                            ((AddressTab)t.Controls[0]).cmbVillage.SelectedText = (string)row["Address2"];
                        if (((AddressTab)t.Controls[0]).cmbRegion.Items.Count > 0 &&
                            row["Address3"] != DBNull.Value) // Address Standardization CR2019 - 025
                        {
                            var regionIndex = ((AddressTab)t.Controls[0]).cmbRegion.FindStringExact((string)row["Address3"]);
                            if (regionIndex != -1)
                                ((AddressTab)t.Controls[0]).cmbRegion.SelectedIndex = regionIndex;
                            else
                                ((AddressTab)t.Controls[0]).cmbRegion.SelectedText = (string)row["Address3"];
                        }
                        else if (row["Address3"] != DBNull.Value) // Address Standardization CR2019 - 025
                            ((AddressTab)t.Controls[0]).cmbRegion.SelectedText = (string)row["Address3"];
                        if (((AddressTab)t.Controls[0]).txtPostCode.Text.Length == 0 ||
                            row["PostCode"] != DBNull.Value)
                            ((AddressTab)t.Controls[0]).txtPostCode.Text = (string)row["PostCode"];
                        if (!Convert.IsDBNull(row["Latitude"]) && !Convert.IsDBNull(row["Longitude"])) // Address Standardization CR2019 - 025
                            ((AddressTab)t.Controls[0]).txtCoordinate.Text = string.Format("{0},{1}", row["Latitude"].ToString(), row["Longitude"].ToString());
                        if (((AddressTab)t.Controls[0]).txtNotes.Text.Length == 0 &&
                            row["Notes"] != DBNull.Value)
                            ((AddressTab)t.Controls[0]).txtNotes.Text = (string)row["Notes"];
                        if (((AddressTab)t.Controls[0]).txtEmail.Text.Length == 0 &&
                            row["Email"] != DBNull.Value)
                            ((AddressTab)t.Controls[0]).txtEmail.Text = ((string)row["Email"]).Trim();      //UAT 500 Remove white space from the email text
                        if (((AddressTab)t.Controls[0]).txtDialCode.Text.Length == 0 &&
                            row["DialCode"] != DBNull.Value)
                            ((AddressTab)t.Controls[0]).txtDialCode.Text = ((string)row["DialCode"]).Trim();
                        if (((AddressTab)t.Controls[0]).txtPhoneNo.Text.Length == 0 &&
                            row["Phone"] != DBNull.Value)
                            ((AddressTab)t.Controls[0]).txtPhoneNo.Text = ((string)row["Phone"]).Trim();
                        if (((AddressTab)t.Controls[0]).txtExtension.Text.Length == 0 &&
                            row["Ext"] != DBNull.Value)
                            ((AddressTab)t.Controls[0]).txtExtension.Text = (string)row["Ext"];
                        if (!Convert.IsDBNull(row["Date In"]))
                            ((AddressTab)t.Controls[0]).dtDateIn.Value = (DateTime)row["Date In"];

                        // Delivery Area
                        ((AddressTab)t.Controls[0]).SetDeliveryArea((string)row[CN.DeliveryArea]);
                        // Name in address tab
                        if (((AddressTab)t.Controls[0]).drptitleC.Text.Length == 0 &&
                           row["DELTitleC"] != DBNull.Value)
                            ((AddressTab)t.Controls[0]).drptitleC.Text = (string)row["DELTitleC"];

                        if (((AddressTab)t.Controls[0]).CFirstname.Text.Length == 0 &&
                            row["DELFirstName"] != DBNull.Value)
                            ((AddressTab)t.Controls[0]).CFirstname.Text = (string)row["DELFirstName"];

                        if (((AddressTab)t.Controls[0]).CLastname.Text.Length == 0 &&
                            row["DELLastName"] != DBNull.Value)
                            ((AddressTab)t.Controls[0]).CLastname.Text = (string)row["DELLastName"];

                        // Collection Zone
                        ((AddressTab)t.Controls[0]).PopulateZones();
                        ((AddressTab)t.Controls[0]).SetZone((string)row[CN.Zone]);

                        //Disable Mobile No field if not Home and Delivery tab
                        if (addType == "W")// Address Standardization CR2019 - 025
                        {
                            ((AddressTab)t.Controls[0]).txtMobile.Enabled = false;
                            ((AddressTab)t.Controls[0]).txtMobile.BackColor = SystemColors.Menu;
                            ((AddressTab)t.Controls[0]).btnMobile.Visible = false;

                        }

                        //IP - 16/03/11 - #3317 - CR1245 - Do not display the button to add multiple work numbers if this is not the work address tab
                        if (addType != "W")
                        {
                            ((AddressTab)t.Controls[0]).btnWork.Visible = false;
                        }

                    }
                }

                //Display Mobile No on the Home Address Tab.
                addType = "H";
                foreach (Crownwood.Magic.Controls.TabPage t in tcAddress.TabPages)
                {
                    if (t.Name == "tp" + addType)
                    {
                        ((AddressTab)t.Controls[0]).txtMobile.Text = mobileNo;
                        ((AddressTab)t.Controls[0]).txtMobile2.Text = mobileNo2;
                        ((AddressTab)t.Controls[0]).txtMobile3.Text = mobileNo3;
                        ((AddressTab)t.Controls[0]).txtMobile4.Text = mobileNo4;
                    }
                    //  Display Mobile No on the Delivery Tab
                    if (t.Name == "tpD") // Address Standardization CR2019 - 025
                    {
                        ((AddressTab)t.Controls[0]).txtMobile.Text = deliveryMobile;
                        ((AddressTab)t.Controls[0]).btnMobile.Visible = false;
                    }

                    //  Display Mobile No on the Delivery1 Tab
                    if (t.Name == "tpD1")
                    {
                        ((AddressTab)t.Controls[0]).txtMobile.Text = delivery1Mobile;
                        ((AddressTab)t.Controls[0]).btnMobile.Visible = false;
                    }

                    //  Display Mobile No on the Delivery2 Tab
                    if (t.Name == "tpD2")
                    {
                        ((AddressTab)t.Controls[0]).txtMobile.Text = delivery2Mobile;
                        ((AddressTab)t.Controls[0]).btnMobile.Visible = false;
                    }

                    //  Display Mobile No on the Delivery3 Tab
                    if (t.Name == "tpD3")
                    {
                        ((AddressTab)t.Controls[0]).txtMobile.Text = delivery3Mobile;
                        ((AddressTab)t.Controls[0]).btnMobile.Visible = false;
                    }
                }
            }

            //IP - 16/03/11 - #3317 - CR1245 - Display any additional work numbers on the Work Address tab
            addType = "W";
            foreach (Crownwood.Magic.Controls.TabPage t in tcAddress.TabPages)
            {
                if (t.Name == "tp" + addType)
                {
                    ((AddressTab)t.Controls[0]).txtWorkDialCode2.Text = workDialCode2;
                    ((AddressTab)t.Controls[0]).txtWorkNum2.Text = workNum2;
                    ((AddressTab)t.Controls[0]).txtWorkExt2.Text = workExt2;

                    ((AddressTab)t.Controls[0]).txtWorkDialCode3.Text = workDialCode3;
                    ((AddressTab)t.Controls[0]).txtWorkNum3.Text = workNum3;
                    ((AddressTab)t.Controls[0]).txtWorkExt3.Text = workExt3;

                    ((AddressTab)t.Controls[0]).txtWorkDialCode4.Text = workDialCode4;
                    ((AddressTab)t.Controls[0]).txtWorkNum4.Text = workNum4;
                    ((AddressTab)t.Controls[0]).txtWorkExt4.Text = workExt4;
                }
            }

            Function = "End of LoadAddresses";
        }

        private void at_onChangeAddressDate(object sender, GenericEventHandler<DateTime> Handler)
        {
            if (dtDateInCurrentAddress1.Value != Handler.Results)
                dtDateInCurrentAddress1.Value = Handler.Results;
        }

        private void LoadAccountsThread()
        {
            try
            {
                Wait();
                Function = "LoadAccountsThread";

                accounts = CustomerManager.CustomerSearch(CustomerID, firstName,
                    lastName, address, phoneNumber, 250,        //CR1084    
                    Convert.ToInt32(settled),
                    true,
                "%",
                     "CUSTSRCH", out Error);

                if (Error.Length > 0)
                    ShowError(Error);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of LoadAccountsThread";
            }
        }

        private void loadAccounts(bool useDummy)
        {
            Function = "LoadAccounts";
            settled = chxIncludeSettled.Checked;
            CustomerID = txtCustID.Text;
            firstName = txtFirstName.Text;
            lastName = txtLastName.Text;
            string status = String.Empty;

            /* if the customer has not yet been saved but we have an account
             * this account must be shown in the accounts list but cannot be 
             * retrieved from the database because the linking customer does not
             * yet exist. This will be achieved by placing a dummy record in the 
             * accounts dataset and signalling that the accounts data should not be
             * retrieved from the databse using the useDummy flag. The loadAccounts
             * method can then be called from the appropriate constructor and
             * will push the dummy record into the datagrid
             */
            if (!useDummy)
            {
                Thread dataThread = new Thread(new ThreadStart(LoadAccountsThread));
                dataThread.Start();
                dataThread.Join();
            }

            if (accounts != null)
            {
                dv = new DataView(accounts.Tables["CustSearch"]);
                dgAccounts.TableStyles.Clear();
                DataGridTableStyle tabStyle = new DataGridTableStyle();
                tabStyle.MappingName = dv.Table.TableName;

                int numCols = accounts.Tables["CustSearch"].Columns.Count;

              
                AccountTextColumn col;
                for (int i = 0; i < numCols; ++i)
                {
                    col = new AccountTextColumn(i);
                    col.AccountColumn = 4;
                    col.HeaderText = accounts.Tables["CustSearch"].Columns[i].ColumnName;
                    col.MappingName = accounts.Tables["CustSearch"].Columns[i].ColumnName;
                    tabStyle.GridColumnStyles.Add(col);
                    col.Dispose();
                }
                dgAccounts.TableStyles.Add(tabStyle);
                UpdateHomeClubAcctCode(ref dv);
                dgAccounts.DataSource = dv;
                StorecardButtonCheckEnabled();
                //IP - 18/02/10 - CR1072 - LW 70849 - General Fixes from 4.3 - Merge
                if (dgAccounts.TableStyles[0].GridColumnStyles.Contains("Holdprop"))
                {
                    dgAccounts.TableStyles[0].GridColumnStyles[CN.HoldProp].Width = 0; //IP 13/03/09 - (70849) - Do not want to display the column
                }


                if (dgAccounts.TableStyles[0].GridColumnStyles.Contains("cusaddr1"))
                {
                    dgAccounts.TableStyles[0].GridColumnStyles[CN.cusaddr1].HeaderText = "Customer Address1"; //To Change the column header from cusaddr1 to Address Line1
                }
                if (dgAccounts.TableStyles[0].GridColumnStyles.Contains("cusaddr2"))
                {
                    dgAccounts.TableStyles[0].GridColumnStyles[CN.cusaddr2].HeaderText = "Customer Address2"; //To Change the column header from cusaddr2 to Address Line2
                }
                if (dgAccounts.TableStyles[0].GridColumnStyles.Contains("cusaddr3"))
                {
                    dgAccounts.TableStyles[0].GridColumnStyles[CN.cusaddr3].HeaderText = "Customer Address3"; //To Change the column header from cusaddr3 to Address Line3
                }

                if (dgAccounts.TableStyles[0].GridColumnStyles.Contains("telno"))
                {
                    dgAccounts.TableStyles[0].GridColumnStyles[CN.TelNo].HeaderText = "Telephone No"; //To Change the column header from telno to TelephoneNo
                }
                if (dgAccounts.TableStyles[0].GridColumnStyles.Contains("postcode"))
                {
                    dgAccounts.TableStyles[0].GridColumnStyles[CN.PostCode].HeaderText = "Post Code"; //To Change the column header from postcode to PostCode
                }
                //if (AccountNo == "")
                //    Relationship = (string)dv[0]["HldOrJnt"] == "H";
                var hldOrJnt = "";
                if (dv.Count > 0)         // #14632 - include sett accts unchecked  
                {
                    hldOrJnt = dv[0]["HldOrJnt"].ToString().Trim(); //Bug #3259 - 
                }
                                
                btnCreateRFAccount.Enabled = _canCreateRF && createRF && CheckAge() && CheckAccountRelationship(); //IP - 04/03/11 - #3260 

                //LW73019 jec 23/02/11
                if ((AccountNo == "" && hldOrJnt == "H") || (AccountNo != "" && Relationship == "H"))		//if it's not holder don't do anything
                {
                    //btnCreateRFAccount.Enabled = _canCreateRF && createRF;  //CR903 jec 08/08/07  && createRF  
                    //btnCreateRFAccount.Enabled = _canCreateRF && createRF && CheckAge() && CheckAccountRelationship();  //IP - 11/05/10 - UAT(141) UAT5.2.1.0 log - Merged from 4.3 //IP - 04/03/11 - #3260 - Previously this got set here and in the else. Now only checking in one at the start of the IF
                    txtRFLimit.Visible = _canCreateRF && createRF;
                    txtRFAvailable.Visible = _canCreateRF && createRF;
                    labelCredit.Visible = _canCreateRF && createRF;
                    labelAvailable.Visible = _canCreateRF && createRF;
                    txtRFIssueNo.Visible = _canCreateRF && createRF;
                    lRFIssueNo.Visible = _canCreateRF && createRF;
                    if (dv.Count == 0)
                    {
                        txtAccountNo.Text = "000-0000-0000-0";
                        AccountNo = "000000000000";
                        AccountType = String.Empty;
                        drpRelationship.Enabled = false;
                    }
                    else
                    {
                        //determine what the text for the create RF button should be
                        dv.RowFilter = "AccountNumber not = '' and Type = 'R' and Status not = 'S' and HldOrJnt = 'H'";
                        string createRFText = dv.Count == 0 ? GetResource("T_CREATERF") : GetResource("T_CREATESUB");
                        if (btnCreateRFAccount.Text != "")
                            btnCreateRFAccount.Text = createRFText;
                        else
                            try
                            {
                                toolTip1.SetToolTip(btnCreateRFAccount, createRFText);
                            }
                            catch
                            {
                                //UAT 242 - Prevent unwanted error message when closing the screen
                            }

                        dv.RowFilter = "AccountNumber not = ''";
                        if (!chxIncludeSettled.Checked)
                            dv.RowFilter += " and Status not = 'S'";

                        // 67782 RD/DR Load Associates Accounts
                        if (!chxIncludeAssocAccounts.Checked)
                            dv.RowFilter += " and HldOrJnt  = 'H'";

                        drpRelationship.Enabled = true;
                        grpAccounts.Enabled = true;

                        //if this is the first load
                        if ((txtAccountNo.Text == "000-0000-0000-0" || txtAccountNo.Text.Length == 0)
                            && dv.Count > 0)
                        {
                            string s = (string)dv[0]["AccountNumber"];
                            this.AccountNo = s;
                            this.AccountType = (string)dv[0]["Type"];
                            status = (string)dv[0]["Status"];
                            FormatAccountNo(ref s);
                            txtAccountNo.Text = s;
                            dgAccounts.CurrentRowIndex = 0;
                            dgAccounts.Select(0);
                        }
                        else	//find the currently selected account in the grid
                        {
                            bool found = false;
                            int i = 0;

                            foreach (DataRowView r in dv)
                            {
                                if ((string)r["AccountNumber"] == txtAccountNo.Text.Replace("-", ""))
                                {
                                    dgAccounts.CurrentRowIndex = i;
                                    dgAccounts.Select(i);
                                    found = true;
                                    break;
                                }
                                if (Convert.ToString(r["Type"]) == AT.StoreCard)
                                {
                                    storeCard.AcctNo = Convert.ToString(r["AccountNumber"]);
                                    StorecardButtonCheckEnabled();
                                }
                                i++;
                            }
                            if (!found && dv.Count > 0)
                            {
                                string s = (string)dv[0]["AccountNumber"];
                                this.AccountNo = s;
                                this.AccountType = (string)dv[0]["Type"];
                                status = (string)dv[0]["Status"];
                                FormatAccountNo(ref s);
                                txtAccountNo.Text = s;
                                dgAccounts.CurrentRowIndex = 0;

                                dgAccounts.Select(0);
                            }
                        }
                    }
                }
                else
                {
                    //LW73019 jec 28/02/11  
                    if (this.AccountNo == "")
                    {
                        string s = (string)dv[0]["AccountNumber"];
                        this.AccountNo = s;
                        this.AccountType = (string)dv[0]["Type"];
                        //status = (string)dv[0]["Status"];
                        //FormatAccountNo(ref s);
                        txtAccountNo.Text = s;
                        dgAccounts.CurrentRowIndex = 0;
                        drpRelationship.Enabled = true;

                        dgAccounts.Select(0);

                        int drpindex = 0;
                        foreach (DataRow row in ((DataTable)drpRelationship.DataSource).Rows)
                        {
                            if (dv.Count == 0)      // required as dv.tables is cleared which clears dgAccounts.DataSource
                            {
                                dv = (DataView)dgAccounts.DataSource;
                            }

                            if ((string)row["code"] == (string)dv[0]["HldOrJnt"])
                            {
                                OldRelationshipIndex = drpRelationship.SelectedIndex = drpindex;
                            }

                            drpindex++;
                        }
                    }

                    //LW73019 jec 28/02/11  find the currently selected account in the grid
                    int i = 0;

                    foreach (DataRowView r in dv)
                    {
                        if ((string)r["AccountNumber"] == txtAccountNo.Text.Replace("-", ""))
                        {
                            dgAccounts.CurrentRowIndex = i;
                            dgAccounts.Select(i);

                            break;
                        }
                        if (Convert.ToString(r["Type"]) == AT.StoreCard)
                        {
                            storeCard.AcctNo = Convert.ToString(r["AccountNumber"]);
                            StorecardButtonCheckEnabled();
                        }
                        i++;
                    }

                    grpAccounts.Enabled = true;     //LW73019 jec 28/02/11 enable accounts grid
                    //btnCreateRFAccount.Enabled = false; //IP - 04/03/11 - #3260 - Now checking above before the IF 
                    txtRFLimit.Visible = false;
                    txtRFAvailable.Visible = false;
                    labelCredit.Visible = false;
                    labelAvailable.Visible = false;
                    txtRFIssueNo.Visible = false;
                    lRFIssueNo.Visible = false;
                    dv.RowFilter = "AccountNumber not = ''";

                }

                //IP - 16/03/11 - #3317 - CR1245
                if (dv.Count != 0 && checkedForAcctsInArrs == false) //IP - 17/03/11 - #3343 - Only check if not already checked.
                {
                    CheckForAcctsInArrears(dv);
                    checkedForAcctsInArrs = true; //IP - 17/03/11 - #3343
                }
                
                // 67782 RD/DR 19/01/06 Added HldOrJnt 
                tabStyle.GridColumnStyles["Customer ID"].Width = 0;
                tabStyle.GridColumnStyles["Title"].Width = 0;
                tabStyle.GridColumnStyles["First Name"].Width = 95;
                tabStyle.GridColumnStyles["First Name"].HeaderText = GetResource("T_FIRSTNAME");
                tabStyle.GridColumnStyles["Last Name"].Width = 95;
                tabStyle.GridColumnStyles["Last Name"].HeaderText = GetResource("T_LASTNAME");
                tabStyle.GridColumnStyles["AccountNumber"].Width = 95;
                tabStyle.GridColumnStyles["AccountNumber"].HeaderText = GetResource("T_ACCTNO");
                tabStyle.GridColumnStyles["HldOrJnt"].Width = 36;
                tabStyle.GridColumnStyles["HldOrJnt"].HeaderText = GetResource("T_HLDJNT");
                tabStyle.GridColumnStyles["Type"].Width = 36;
                tabStyle.GridColumnStyles["Type"].HeaderText = GetResource("T_TYPE");
                tabStyle.GridColumnStyles["Date Opened"].Width = 95;    
                tabStyle.GridColumnStyles["Date Opened"].HeaderText = GetResource("T_DATEOPENED");
                tabStyle.GridColumnStyles["Outstanding Balance"].Width = 95;
                tabStyle.GridColumnStyles["Outstanding Balance"].HeaderText = GetResource("T_OUTBAL");
                tabStyle.GridColumnStyles["Outstanding Balance"].Alignment = HorizontalAlignment.Right;
                ((DataGridTextBoxColumn)tabStyle.GridColumnStyles["Outstanding Balance"]).Format = DecimalPlaces;
                tabStyle.GridColumnStyles["Arrears"].Width = 95;
                tabStyle.GridColumnStyles["Arrears"].HeaderText = GetResource("T_ARREARS");
                tabStyle.GridColumnStyles["Arrears"].Alignment = HorizontalAlignment.Right;
                ((DataGridTextBoxColumn)tabStyle.GridColumnStyles["Arrears"]).Format = DecimalPlaces;
                tabStyle.GridColumnStyles["Agreement Total"].Width = 75;
                tabStyle.GridColumnStyles["Agreement Total"].HeaderText = GetResource("T_AGREETOTAL");
                tabStyle.GridColumnStyles["Agreement Total"].Alignment = HorizontalAlignment.Right;
                ((DataGridTextBoxColumn)tabStyle.GridColumnStyles["Agreement Total"]).Format = DecimalPlaces;
                tabStyle.GridColumnStyles["Status"].Width = 36;
                tabStyle.GridColumnStyles["Status"].HeaderText = GetResource("T_STATUS");
                tabStyle.GridColumnStyles["Status"].Alignment = HorizontalAlignment.Right;
                tabStyle.GridColumnStyles["Alias"].Width = 0;
                tabStyle.GridColumnStyles[CN.AddTo].Width = 0;
                tabStyle.GridColumnStyles[CN.DeliveredIndicator].Width = 0;
                tabStyle.GridColumnStyles[CN.Reversible].Width = 0;
                tabStyle.GridColumnStyles[CN.AgrmtNo].Width = 0;
                tabStyle.GridColumnStyles[CN.CashPrice].Width = 0;
                tabStyle.GridColumnStyles[CN.BranchNo].Width = 0;
                tabStyle.GridColumnStyles["IsMambuAccount"].Width = 0;
                tabStyle.Dispose();
            }
            Function = "End of LoadAccounts";
        }

       
        private void LoadCustomerAudit()
        {
            try
            {
                Wait();
                Function = "LoadCustomerAudit";
                ClearControls(this.gbNameHistory.Controls);
                dgNameHistory.DataSource = null;

                if (CustomerID.Trim() != String.Empty)
                {
                    DataSet changeSet = CustomerManager.GetCustomerAudit(CustomerID, out Error);

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        DataView changeView = new DataView(changeSet.Tables[TN.CustomerAudit]);
                        dgNameHistory.TableStyles.Clear();
                        DataGridTableStyle nameStyle = new DataGridTableStyle();
                        nameStyle.MappingName = changeView.Table.TableName;

                        AddColumnStyle(CN.OldCustId, nameStyle, 100, true, GetResource("T_OLDCUSTID"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.NewCustId, nameStyle, 100, true, GetResource("T_NEWCUSTID"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.OldName, nameStyle, 160, true, GetResource("T_OLDNAME"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.NewName, nameStyle, 160, true, GetResource("T_NEWNAME"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.DateChanged, nameStyle, 90, true, GetResource("T_DATECHANGED"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.ChangedBy, nameStyle, 80, true, GetResource("T_CHANGEDBY"), "", HorizontalAlignment.Left);

                        dgNameHistory.TableStyles.Add(nameStyle);
                        dgNameHistory.DataSource = changeView;

                        nameStyle.Dispose();
                    }
                }
                else
                {
                    dgNameHistory.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function = "End of LoadCustomerAudit";
                StopWait();
            }
        }
        //old
        //private void LoadAddressHistory()
        //{
        //    try
        //    {
        //        Wait();
        //        Function = "LoadAddressHistory";
        //    ClearControls(this.gbAddressHistory.Controls);
        //    ClearControls(this.gbTelHistory.Controls);
        //    dgAddressHistory.DataSource = null;
        //    dgTelephoneHistory.DataSource = null;

        //        if (CustomerID.Trim() != String.Empty)
        //        {
        //            DataSet addressSet = CustomerManager.GetAddressHistory(CustomerID, out Error);

        //            if (Error.Length > 0)
        //                ShowError(Error);
        //            else
        //            {
        //                // Address History

        //                DataView addressView = new DataView(addressSet.Tables[TN.AddressHistory]);
        //                dgAddressHistory.TableStyles.Clear();
        //                DataGridTableStyle addressStyle = new DataGridTableStyle();
        //                addressStyle.MappingName = addressView.Table.TableName;

        //                AddColumnStyle(CN.CustID, addressStyle, 0, true, "", "", HorizontalAlignment.Left);
        //                AddColumnStyle(CN.AddType, addressStyle, 30, true, GetResource("T_TYPE"), "", HorizontalAlignment.Left);
        //                AddColumnStyle(CN.DateIn, addressStyle, 80, true, GetResource("T_DATEFROM"), "", HorizontalAlignment.Left);
        //                AddColumnStyle(CN.DateMoved, addressStyle, 80, true, GetResource("T_DATETO"), "", HorizontalAlignment.Left);
        //                AddColumnStyle(CN.cusaddr1, addressStyle, 150, true, GetResource("T_ADDRESS1"), "", HorizontalAlignment.Left);
        //                AddColumnStyle(CN.cusaddr2, addressStyle, 150, true, GetResource("T_ADDRESS2"), "", HorizontalAlignment.Left);
        //                AddColumnStyle(CN.cusaddr3, addressStyle, 150, true, GetResource("T_ADDRESS3"), "", HorizontalAlignment.Left);
        //                AddColumnStyle(CN.cuspocode, addressStyle, 70, true, GetResource("T_POSTCODE"), "", HorizontalAlignment.Left);
        //                AddColumnStyle(CN.Email, addressStyle, 90, true, GetResource("T_EMAIL"), "", HorizontalAlignment.Left);
        //                AddColumnStyle(CN.DeliveryArea, addressStyle, 70, true, GetResource("T_DELIVERYAREA"), "", HorizontalAlignment.Left);

        //                dgAddressHistory.TableStyles.Add(addressStyle);
        //                dgAddressHistory.DataSource = addressView;

        //                // Telephone History

        //                DataView telephoneView = new DataView(addressSet.Tables[TN.TelephoneHistory]);
        //                dgTelephoneHistory.TableStyles.Clear();
        //                DataGridTableStyle telephoneStyle = new DataGridTableStyle();
        //                telephoneStyle.MappingName = telephoneView.Table.TableName;

        //                AddColumnStyle(CN.CustID, telephoneStyle, 0, true, "", "", HorizontalAlignment.Left);
        //                AddColumnStyle(CN.TelLocation, telephoneStyle, 80, true, GetResource("T_LOCATION"), "", HorizontalAlignment.Left);
        //                AddColumnStyle(CN.DateChange, telephoneStyle, 100, true, GetResource("T_DATECHANGED"), "", HorizontalAlignment.Left);
        //                AddColumnStyle(CN.EmpeeNoChange, telephoneStyle, 80, true, GetResource("T_EMPEENO"), "", HorizontalAlignment.Left);
        //                AddColumnStyle(CN.DateTelAdd, telephoneStyle, 80, true, GetResource("T_DATEADDED"), "", HorizontalAlignment.Left);
        //                AddColumnStyle(CN.DateDiscon, telephoneStyle, 100, true, GetResource("T_DATETO"), "", HorizontalAlignment.Left);
        //                AddColumnStyle(CN.DialCode, telephoneStyle, 80, true, GetResource("T_DIALCODE"), "", HorizontalAlignment.Left);
        //                AddColumnStyle(CN.TelNo, telephoneStyle, 110, true, GetResource("T_PHONENO"), "", HorizontalAlignment.Left);
        //                AddColumnStyle(CN.ExtNo, telephoneStyle, 80, true, GetResource("T_EXT"), "", HorizontalAlignment.Left);

        //                dgTelephoneHistory.TableStyles.Add(telephoneStyle);
        //                dgTelephoneHistory.DataSource = telephoneView;

        //          addressStyle.Dispose();
        //                telephoneStyle.Dispose();
        //            }
        //        }
        //        else
        //        {
        //            dgAddressHistory.DataSource = null;
        //            dgTelephoneHistory.DataSource = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, Function);
        //    }
        //    finally
        //    {
        //        Function = "End of LoadAddressHistory";
        //        StopWait();
        //    }
        //}

        //new
        private void LoadAddressHistory()
        {
            try
            {
                Wait();
                Function = "LoadAddressHistory";

                if (CustomerID.Trim() != "")
                {
                    bool audit = radAddressAudit.Checked;
                    DataSet addressSet;
                    if (!audit)
                        addressSet = CustomerManager.GetAddressHistory(CustomerID, out Error);
                    else
                        addressSet = CustomerManager.GetAddressAuditHistory(CustomerID, out Error);

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        addressView = new DataView(addressSet.Tables[TN.AddressHistory]);
                        dgAddressHistory.TableStyles.Clear();
                        DataGridTableStyle addressStyle = new DataGridTableStyle();
                        addressStyle.MappingName = addressView.Table.TableName;

                        AddColumnStyle(CN.CustID, addressStyle, 80, true, GetResource("T_CUSTID"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.AddType, addressStyle, 30, true, GetResource("T_TYPE"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.DateIn, addressStyle, 80, true, GetResource("T_DATEFROM"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.DateMoved, addressStyle, 80, true, GetResource("T_DATETO"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.cusaddr1, addressStyle, 150, true, GetResource("T_ADDRESS1"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.cusaddr2, addressStyle, 150, true, GetResource("T_ADDRESS2"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.cusaddr3, addressStyle, 150, true, GetResource("T_ADDRESS3"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.cuspocode, addressStyle, 70, true, GetResource("T_POSTCODE"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.Email, addressStyle, 90, true, GetResource("T_EMAIL"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.DeliveryArea, addressStyle, 70, true, GetResource("T_DELIVERYAREA"), "", HorizontalAlignment.Left);
                        if (audit)
                        {
                            // display extra columns
                            AddColumnStyle(CN.DateChange, addressStyle, 80, true, GetResource("T_DATECHANGED"), "", HorizontalAlignment.Left);
                            AddColumnStyle(CN.EmpeeNoChange, addressStyle, 80, true, GetResource("T_EMPEENO"), "", HorizontalAlignment.Left);
                            AddColumnStyle("ChangeType", addressStyle, 80, true, "Change Type", "", HorizontalAlignment.Left);

                        }

                        dgAddressHistory.TableStyles.Add(addressStyle);
                        dgAddressHistory.DataSource = addressView;


                        //dgTelephoneHistory.DataSource = addressSet.Tables["TelHistory"];
                        DataView telephoneView = new DataView(addressSet.Tables["TelHistory"]);
                        dgTelephoneHistory.TableStyles.Clear();
                        DataGridTableStyle telephoneStyle = new DataGridTableStyle();
                        telephoneStyle.MappingName = telephoneView.Table.TableName;

                        AddColumnStyle(CN.CustID, telephoneStyle, 0, true, "", "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.TelLocation, telephoneStyle, 80, true, GetResource("T_LOCATION"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.DateChange, telephoneStyle, 100, true, GetResource("T_DATECHANGED"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.EmpeeNoChange, telephoneStyle, 80, true, GetResource("T_EMPEENO"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.DateTelAdd, telephoneStyle, 80, true, GetResource("T_DATEADDED"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.DateDiscon, telephoneStyle, 100, true, GetResource("T_DATETO"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.DialCode, telephoneStyle, 80, true, GetResource("T_DIALCODE"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.TelNo, telephoneStyle, 110, true, GetResource("T_PHONENO"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.ExtNo, telephoneStyle, 80, true, GetResource("T_EXT"), "", HorizontalAlignment.Left);

                        if (audit)
                        {
                            AddColumnStyle("ChangeType", telephoneStyle, 80, true, "Change Type", "", HorizontalAlignment.Left);
                        }

                        dgTelephoneHistory.TableStyles.Add(telephoneStyle);
                        dgTelephoneHistory.DataSource = telephoneView;

                        addressStyle.Dispose();
                        telephoneStyle.Dispose();
                    }
                }
                else
                {
                    dgAddressHistory.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function = "End of LoadAddressHistory";
                StopWait();
            }
        }

        private void LaunchSanctioning()
        {
            try
            {
                Function = "LaunchSanctioning";
                Wait();
                this.newAccounttoCancel = String.Empty; //sanctioning account so don't cancel it now
                bool valid = false;
                string[] parms = null;
                SanctionStage1 stage1 = null;
                SanctionStage2 stage2 = null;
                DocumentConfirmation docComf = null;
                Referral refer = null;

                title = ((DataRowView)drpTitle.SelectedItem)[0].ToString();

                //validate the custid
                valid = !txtCustID.IsBlank(true);

                if (valid)
                {
                    //Save the customer so that if they're new, they will exist in 
                    //the database
                    if (!ReadOnly && this._hasdatachanged == true)
                        valid = Save();
                }

                if (valid)
                {
                    //for HP accounts this was not being populated 
                    if (StageToLaunch.AccountNo.Length == 0)
                        txtAccountNo_TextChanged(this, null);
                    // if account locked do not launch   jec #4119 28/06/11
                    if (!LockAccount())
                    {
                        switch (StageToLaunch.CheckType)
                        {
                            case SS.S1:
                                parms = new String[3];
                                parms[0] = StageToLaunch.CustID;
                                parms[1] = StageToLaunch.AccountNo;
                                parms[2] = StageToLaunch.AcctType;

                                stage1 = new SanctionStage1(createHP, parms,StageToLaunch.ScreenMode, FormRoot, this, this);
                                ((MainForm)FormRoot).AddTabPage(stage1);
                                break;

                            case SS.S2:
                                stage2 = new SanctionStage2(StageToLaunch.CustID,
                                    StageToLaunch.DateProp,
                                    StageToLaunch.AccountNo,
                                    StageToLaunch.AcctType,
                                    StageToLaunch.ScreenMode,
                                    FormRoot,
                                    this, this);
                                ((MainForm)FormRoot).AddTabPage(stage2);
                                break;
                            case SS.DC:
                                docComf = new DocumentConfirmation(StageToLaunch.CustID,
                                    StageToLaunch.DateProp,
                                    StageToLaunch.AccountNo,
                                    StageToLaunch.AcctType,
                                    StageToLaunch.ScreenMode,
                                    FormRoot,
                                    this, this);
                                ((MainForm)FormRoot).AddTabPage(docComf);
                                break;
                            case SS.AD:	//launch additional data
                                break;
                            case SS.R:
                                if (StageToLaunch.Stage == GetResource("P_REFERREJECTED"))
                                {
                                    CreditManager.ManualRefer(StageToLaunch.CustID,
                                        StageToLaunch.AccountNo,
                                        StageToLaunch.DateProp,
                                        true,
                                        false,
                                        out Error);
                                }
                                if (Error.Length > 0)
                                    ShowError(Error);
                                else
                                {
                                    refer = new Referral(this.createHP, StageToLaunch.CustID,
                                        StageToLaunch.DateProp,
                                        StageToLaunch.AccountNo,
                                        StageToLaunch.AcctType,
                                        StageToLaunch.ScreenMode,
                                        FormRoot,
                                        this, this, true);
                                    ((MainForm)FormRoot).AddTabPage(refer);
                                }
                                break;
                            default:
                                parms = new String[3];
                                parms[0] = StageToLaunch.CustID;
                                parms[1] = StageToLaunch.AccountNo;
                                parms[2] = StageToLaunch.AcctType;
                                //Livewire 69230 - include createHP property as parameter passed to SanctionStage1 once code changes for 5.0.5.0 have been applied
                                stage1 = new SanctionStage1(createHP, parms, SM.New, FormRoot, this, this);
                                stage1.Settled = settled;
                                stage1.CashPrice = this.CashPrice;
                                ((MainForm)this.FormRoot).AddTabPage(stage1, 18);
                                if (!stage1.AccountLocked)
                                {
                                    stage1.Confirm = false;
                                    stage1.CloseTab();
                                }
                                break;
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
                Function = "End of LaunchSanctioning";
            }
        }


        //19Apr2019 : Use function to remove first line ENTER value in text //
        #region 
        public static string NotesComments(string StrlotsOfLines)
        {
            string[] lines = StrlotsOfLines.TrimStart().Split('\n');
            string StrNoteValue = "";
            foreach (string line in lines)
            {
                if (line.IndexOf('\r').ToString() != "0")
                {
                    if (StrNoteValue.Length == 0)
                    {
                        StrNoteValue = StrNoteValue + (StrlotsOfLines).TrimStart();
                    }
                }
            }
            return StrNoteValue;

        }
        #endregion

        private DataSet SaveAddresses()
        {
            Function = "SaveAddresses";
            DataSet ds = null;

            if (tcAddress.TabPages.Count > 0)
            {
                ds = new DataSet();
                DataTable dt = new DataTable("Addresses");
                dt.Columns.AddRange(new DataColumn[]{   new DataColumn("AddressType"),
                                                        new DataColumn("Address1"),
                                                        new DataColumn("Address2"),
                                                        new DataColumn("Address3"),
                                                        new DataColumn("PostCode"),
                                                        new DataColumn(CN.DeliveryArea),
                                                        new DataColumn("Notes"),
                                                        new DataColumn("EMail"),
                                                        new DataColumn("DialCode"),
                                                        new DataColumn("PhoneNo"),
                                                        new DataColumn("Ext"),
                                                        new DataColumn("DELTitleC"),
                                                        new DataColumn("DELFirstname"),
                                                        new DataColumn("DELLastname"),
                                                        new DataColumn("DateIn", Type.GetType("System.DateTime")),
                                                        new DataColumn("NewRecord", Type.GetType("System.Boolean")),


                                                        new DataColumn(CN.Zone)});      //CR1084
                var latitudeColumn = new DataColumn("Latitude", Type.GetType("System.Double"));// Address Standardization CR2019 - 025
                latitudeColumn.AllowDBNull = true;
                var longitudeColumn = new DataColumn("Longitude", Type.GetType("System.Double")); // Address Standardization CR2019 - 025
                longitudeColumn.AllowDBNull = true;
                dt.Columns.Add(latitudeColumn);
                dt.Columns.Add(longitudeColumn);
                //the final two columns are used to decide whether to 
                //create history records in the custaddress and custtel tables
                //where what the user intends is ambiguous.

                //IP - 23/02/09 - UAT(630) - removed datetime as was previously adding todays time onto the original date (datein)
                //causing the customer addresses to always be updated even though a change had not been made. This would then update the
                //'datechange' on the 'Custaddress' table.
                //Also set the 'Notes' to an empty string if the 'Address Type' is 'M' (Mobile) as further in the code
                //BCustomer.SaveCustomerAddresses, this is compared to the notes returned for 'M' from the database
                //which is empty string. Previously was incorrectly inserting a record into 'Custaddress' for addresstype 'M'.

                //loop through the the tab pages in the addresses tab control
                foreach (Crownwood.Magic.Controls.TabPage tp in tcAddress.TabPages)
                {
                    //find the AddressTab in the tabPage - should only be one control in 
                    //the tab page controls collection
                    foreach (Control c in tp.Controls)
                    {
                        DataRow row = dt.NewRow();
                        string addtype = tp.Name.Substring(2, tp.Name.Length - 2);
                        row["AddressType"] = addtype;
                        //row["AddressType"] = tp.Title;
                        row["Address1"] = ((AddressTab)c).txtAddress1.Text;
                        row["Address2"] = ((AddressTab)c).cmbVillage.SelectedValue;
                        row["Address3"] = ((AddressTab)c).cmbRegion.SelectedValue;
                        row["PostCode"] = ((AddressTab)c).txtPostCode.Text;
                        //Fix Enter issue at first line : 19Apr2019 //              
                        row["Notes"] = NotesComments(((AddressTab)c).txtNotes.Text);

                        row["EMail"] = ((AddressTab)c).txtEmail.Text;
                        row["DialCode"] = ((AddressTab)c).txtDialCode.Text;
                        row["PhoneNo"] = ((AddressTab)c).txtPhoneNo.Text;
                        row["Ext"] = ((AddressTab)c).txtExtension.Text;
                        //txtJobTitle1.SelectedValue = row[CN.JobTitle].ToString();
                        row["DELTitleC"] = addtype == "D" ? ((AddressTab)c).drptitleC.Text : String.Empty; // Address Standardization CR2019 - 025
                        //row["DELTitleC"] = ((AddressTab)c).TitleC.Text;
                        row["DELFirstname"] = ((AddressTab)c).CFirstname.Text;
                        row["DELLastname"] = ((AddressTab)c).CLastname.Text;
                        row["DateIn"] = ((AddressTab)c).dtDateIn.Value; //IP - 23/02/09 - UAT(630) - removed datetime
                        row["NewRecord"] = (bool)tp.Tag;
                        row[CN.DeliveryArea] = ((AddressTab)c).drpDeliveryArea.SelectedIndex == 0 ? "" : ((AddressTab)c).drpDeliveryArea.Text;
                        row[CN.Zone] = ((AddressTab)c).drpZone.SelectedIndex == ((AddressTab)c).drpZone.Items.Count - 1 ? "" : ((AddressTab)c).drpZone.Text;      //CR1084  //UAT62 - last item is blank  
                        if (!string.IsNullOrEmpty(((AddressTab)c).txtCoordinate.Text)) // Address Standardization CR2019 - 025
                        {
                            var coordinate = ((AddressTab)c).txtCoordinate.Text.Split(',');
                            double latitude, longitude;

                            if (coordinate.Length == 2 && double.TryParse(coordinate[0], out latitude))
                                row["Latitude"] = latitude;
                            else
                                row["Latitude"] = DBNull.Value;

                            if (coordinate.Length == 2 && double.TryParse(coordinate[1], out longitude))
                                row["Longitude"] = longitude;
                            else
                                row["Longitude"] = DBNull.Value;
                        }
                        dt.Rows.Add(row);


                        if (addtype == "H" && ((AddressTab)c).txtMobile.Text.Length > 0)
                        {
                            row = dt.NewRow();
                            row["AddressType"] = "M";
                            //row["Notes"] = ((AddressTab)c).txtNotes.Text; //IP - 23/02/09 - UAT(630)
                            row["Notes"] = ""; //IP - 23/02/09 - UAT(630)
                            row["DialCode"] = ((AddressTab)c).txtDialCode.Text;
                            row["PhoneNo"] = ((AddressTab)c).txtMobile.Text;
                            row["DateIn"] = ((AddressTab)c).dtDateIn.Value; //IP - 23/02/09 - UAT(630) - removed datetime
                            row["NewRecord"] = (bool)tp.Tag;
                            dt.Rows.Add(row);
                        }

                        if (addtype == "D" && ((AddressTab)c).txtMobile.Text.Length > 0)   // Address Standardization CR2019 - 025
                        {
                            row = dt.NewRow();
                            row["AddressType"] = "DM";
                            row["Notes"] = ""; //IP - 23/02/09 - UAT(630)
                            row["DialCode"] = ((AddressTab)c).txtDialCode.Text;
                            row["PhoneNo"] = ((AddressTab)c).txtMobile.Text;
                            row["DateIn"] = ((AddressTab)c).dtDateIn.Value; //IP - 23/02/09 - UAT(630) - removed datetime
                            row["NewRecord"] = (bool)tp.Tag;
                            dt.Rows.Add(row);
                        }
                        if (addtype == "D1" && ((AddressTab)c).txtMobile.Text.Length > 0)   // Address Standardization CR2019 - 025
                        {
                            row = dt.NewRow();
                            row["AddressType"] = "D1M";
                            row["Notes"] = ""; //IP - 23/02/09 - UAT(630)
                            row["DialCode"] = ((AddressTab)c).txtDialCode.Text;
                            row["PhoneNo"] = ((AddressTab)c).txtMobile.Text;
                            row["DateIn"] = ((AddressTab)c).dtDateIn.Value; //IP - 23/02/09 - UAT(630) - removed datetime
                            row["NewRecord"] = (bool)tp.Tag;
                            dt.Rows.Add(row);
                        }
                        if (addtype == "D2" && ((AddressTab)c).txtMobile.Text.Length > 0)   // Address Standardization CR2019 - 025
                        {
                            row = dt.NewRow();
                            row["AddressType"] = "D2M";
                            row["Notes"] = ""; //IP - 23/02/09 - UAT(630)
                            row["DialCode"] = ((AddressTab)c).txtDialCode.Text;
                            row["PhoneNo"] = ((AddressTab)c).txtMobile.Text;
                            row["DateIn"] = ((AddressTab)c).dtDateIn.Value; //IP - 23/02/09 - UAT(630) - removed datetime
                            row["NewRecord"] = (bool)tp.Tag;
                            dt.Rows.Add(row);
                        }
                        if (addtype == "D3" && ((AddressTab)c).txtMobile.Text.Length > 0)   // Address Standardization CR2019 - 025
                        {
                            row = dt.NewRow();
                            row["AddressType"] = "D3M";
                            row["Notes"] = ""; //IP - 23/02/09 - UAT(630)
                            row["DialCode"] = ((AddressTab)c).txtDialCode.Text;
                            row["PhoneNo"] = ((AddressTab)c).txtMobile.Text;
                            row["DateIn"] = ((AddressTab)c).dtDateIn.Value; //IP - 23/02/09 - UAT(630) - removed datetime
                            row["NewRecord"] = (bool)tp.Tag;
                            dt.Rows.Add(row);
                        }


                        if (addtype == "H" && ((AddressTab)c).txtMobile2.Text.Length > 0)
                        {
                            row = dt.NewRow();
                            row["AddressType"] = "M2";
                            //row["Notes"] = ((AddressTab)c).txtNotes.Text; //IP - 23/02/09 - UAT(630)
                            row["Notes"] = ""; //IP - 23/02/09 - UAT(630)
                            row["DialCode"] = ((AddressTab)c).txtDialCode.Text;
                            row["PhoneNo"] = ((AddressTab)c).txtMobile2.Text;
                            row["DateIn"] = ((AddressTab)c).dtDateIn.Value; //IP - 23/02/09 - UAT(630) - removed datetime
                            row["NewRecord"] = (bool)tp.Tag;
                            dt.Rows.Add(row);
                        }
                        if (addtype == "H" && ((AddressTab)c).txtMobile3.Text.Length > 0)
                        {
                            row = dt.NewRow();
                            row["AddressType"] = "M3";
                            //row["Notes"] = ((AddressTab)c).txtNotes.Text; //IP - 23/02/09 - UAT(630)
                            row["Notes"] = ""; //IP - 23/02/09 - UAT(630)
                            row["DialCode"] = ((AddressTab)c).txtDialCode.Text;
                            row["PhoneNo"] = ((AddressTab)c).txtMobile3.Text;
                            row["DateIn"] = ((AddressTab)c).dtDateIn.Value; //IP - 23/02/09 - UAT(630) - removed datetime
                            row["NewRecord"] = (bool)tp.Tag;
                            dt.Rows.Add(row);
                        }
                        if (addtype == "H" && ((AddressTab)c).txtMobile4.Text.Length > 0)
                        {
                            row = dt.NewRow();
                            row["AddressType"] = "M4";
                            //row["Notes"] = ((AddressTab)c).txtNotes.Text; //IP - 23/02/09 - UAT(630)
                            row["Notes"] = ""; //IP - 23/02/09 - UAT(630)
                            row["DialCode"] = ((AddressTab)c).txtDialCode.Text;
                            row["PhoneNo"] = ((AddressTab)c).txtMobile4.Text;
                            row["DateIn"] = ((AddressTab)c).dtDateIn.Value; //IP - 23/02/09 - UAT(630) - removed datetime
                            row["NewRecord"] = (bool)tp.Tag;
                            dt.Rows.Add(row);
                        }

                        //IP - 16/03/11 - #3317 - CR1245 - Add additional work numbers to the datatable
                        if (addtype == "W" && (((AddressTab)c).txtWorkDialCode2.Text.Length > 0
                                            || ((AddressTab)c).txtWorkNum2.Text.Length > 0
                                            || ((AddressTab)c).txtWorkExt2.Text.Length > 0))
                        {
                            row = dt.NewRow();
                            row["AddressType"] = "W2";
                            row["Notes"] = "";
                            row["DialCode"] = ((AddressTab)c).txtWorkDialCode2.Text;
                            row["PhoneNo"] = ((AddressTab)c).txtWorkNum2.Text;
                            row["Ext"] = ((AddressTab)c).txtWorkExt2.Text;
                            row["DateIn"] = ((AddressTab)c).dtDateIn.Value;
                            row["NewRecord"] = (bool)tp.Tag;
                            dt.Rows.Add(row);
                        }

                        //IP - 16/03/11 - #3317 - CR1245 - Add additional work numbers to the datatable
                        if (addtype == "W" && (((AddressTab)c).txtWorkDialCode3.Text.Length > 0
                                            || ((AddressTab)c).txtWorkNum3.Text.Length > 0
                                            || ((AddressTab)c).txtWorkExt3.Text.Length > 0))
                        {
                            row = dt.NewRow();
                            row["AddressType"] = "W3";
                            row["Notes"] = "";
                            row["DialCode"] = ((AddressTab)c).txtWorkDialCode3.Text;
                            row["PhoneNo"] = ((AddressTab)c).txtWorkNum3.Text;
                            row["Ext"] = ((AddressTab)c).txtWorkExt3.Text;
                            row["DateIn"] = ((AddressTab)c).dtDateIn.Value;
                            row["NewRecord"] = (bool)tp.Tag;
                            dt.Rows.Add(row);
                        }

                        //IP - 16/03/11 - #3317 - CR1245 - Add additional work numbers to the datatable
                        if (addtype == "W" && (((AddressTab)c).txtWorkDialCode4.Text.Length > 0
                                            || ((AddressTab)c).txtWorkNum4.Text.Length > 0
                                            || ((AddressTab)c).txtWorkExt4.Text.Length > 0))
                        {
                            row = dt.NewRow();
                            row["AddressType"] = "W4";
                            row["Notes"] = "";
                            row["DialCode"] = ((AddressTab)c).txtWorkDialCode4.Text;
                            row["PhoneNo"] = ((AddressTab)c).txtWorkNum4.Text;
                            row["Ext"] = ((AddressTab)c).txtWorkExt4.Text;
                            row["DateIn"] = ((AddressTab)c).dtDateIn.Value;
                            row["NewRecord"] = (bool)tp.Tag;
                            dt.Rows.Add(row);
                        }


                    }
                }
                ds.Tables.Add(dt);
                dt.Dispose();
            }
            Function = "End of SaveAddresses";
            return ds;
        }

        //private void InitialiseEmpAndBankTables()
        //{
        //    InitialiseEmp();
        //    InitialiseBank();
        //}
        private void InitialiseEmp()
        {
            if (EmploymentDetails == null)
            {
                EmploymentDetails = new DataTable(TN.Employment);
                EmploymentDetails.Columns.AddRange(new DataColumn[] { new DataColumn(CN.CustomerID),
                                                                        new DataColumn(CN.DateEmployed, Type.GetType("System.DateTime")),
                                                                        new DataColumn(CN.WorkType),
                                                                        new DataColumn(CN.EmploymentStatus),
                                                                        new DataColumn(CN.PayFrequency),
                                                                        new DataColumn(CN.AnnualGross, Type.GetType("System.Double")),
                                                                        new DataColumn(CN.PersDialCode),
                                                                        new DataColumn(CN.PersTel),
                                                                        new DataColumn(CN.PrevDateEmployed, Type.GetType("System.DateTime")),
                                                                        new DataColumn(CN.StaffNo),
                                                                        new DataColumn(CN.JobTitle),
                                                                        new DataColumn(CN.Employer),
                                                                        new DataColumn(CN.FullOrPartTime),
                                                                        new DataColumn(CN.Occupation), //CR 866 new col
																		new DataColumn(CN.Industry), //CR 866 new col
																		new DataColumn(CN.Organisation), //CR 866 new col 
																		new DataColumn(CN.EducationLevel), //CR 866 new col 
																	});
            }
        }

        private void InitialiseBank()
        {
            if (BankDetails == null)
            {
                BankDetails = new DataTable(TN.Bank);
                BankDetails.Columns.AddRange(new DataColumn[] { new DataColumn(CN.CustomerID),
                                                                  new DataColumn(CN.BankAccountNo),
                                                                  new DataColumn(CN.BankCode),
                                                                  new DataColumn(CN.BankAccountOpened, Type.GetType("System.DateTime")),
                                                                  new DataColumn(CN.Code),
                                                                  new DataColumn(CN.IsMandate, Type.GetType("System.Boolean")),
                                                                  new DataColumn(CN.DueDayId, Type.GetType("System.Int32")),
                                                                  new DataColumn(CN.BankAccountName),
                                                                  new DataColumn(CN.BankName)});
            }
        }



        private bool Save()
        {
            Function = "Save";
            bool valid = true;
            bool isDefault = false;
            Wait();

            //Before we save, do some validation

            // Check format masking and mandatory Cust Id entered
            valid = txtCustID.IsValid(true);//so it can be valid but empty at the same time? |
            //                                                                               |
            errorProvider1.SetError(dtDOB, ""); //                                           |
            //                                                                               |
            if (!valid && txtCustID.Text.Trim() != "")//<-------------------------------------
            {
                // An existing legacy format will be allowed
                DataSet ds = AccountManager.GetAccountName(String.Empty, txtCustID.Text, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                    valid = (ds.Tables[0].Rows.Count > 0);
            }

            if (valid)
            {
                txtCustID.SetError(String.Empty);
                if (Holder.Length == 0)
                    Holder = txtCustID.Text;
                Linked = txtCustID.Text;
            }

            if (txtLastName.Text.Length == 0)
            {
                valid = false;
                errorProvider1.SetError(txtLastName, GetResource("M_ENTERMANDATORY"));
            }
            else
                errorProvider1.SetError(txtLastName, String.Empty);

            /* must check both that there is at least one address and 
             * that there is a home address supplied */

            bool homeAddress = false;
            if (tcAddress.TabPages.Count == 0)
            {
                valid = false;
                errorProvider1.SetError(tcAddress, GetResource("M_ENTERMANDATORY"));
            }
            else
            {
                errorProvider1.SetError(tcAddress, String.Empty);
                Regex regExp = null;
                Match regMatch = null;

                foreach (Crownwood.Magic.Controls.TabPage tp in tcAddress.TabPages)
                {
                    if (tp.Name == "tpH")
                        homeAddress = true;
                    foreach (Control c in tp.Controls)
                    {
                        if (!((AddressTab)c).IsValid())
                            valid = false;
                        if (string.IsNullOrEmpty(((AddressTab)c).txtAddress1.Text)) // Address Standardization CR2019 - 025
                            isDefault = true;

                        // match any letter in the email address
                        regExp = new Regex("[A-Z]|[a-z]|[@]");
                        regMatch = regExp.Match(((AddressTab)c).txtEmail.Text);

                        // does the email address have an @ sign

                        if ((regMatch.Success && ((AddressTab)c).txtEmail.Text.Contains("@")) || ((AddressTab)c).txtEmail.Text.Trim() == (""))
                        {
                            //isDefault = true;
                            errorProvider1.SetError(((AddressTab)c).txtEmail, String.Empty);   //UAT84 jec 21/10/10
                        }
                        else
                        {
                            errorProvider1.SetError(((AddressTab)c).txtEmail, GetResource("M_INVALIDEMAIL"));   //UAT84 jec 21/10/10
                            valid = false;
                        }
                    }
                }
            }

            if (!homeAddress)
            {
                valid = false;
                errorProvider1.SetError(tcAddress, GetResource("M_NOHOMEADDRESS"));
            }
            else
                errorProvider1.SetError(tcAddress, String.Empty);

            if (valid)
            {
                if (isDefault)
                {
                    valid = false;
                    errorProvider1.SetError(tcAddress, GetResource("M_ENTERMANDATORY"));
                }
                else
                    errorProvider1.SetError(tcAddress, String.Empty);
            }

            #region more address validation. Get a list of delivery address types selected in this customer's lineitems and make sure they are all present
            if (valid)
            {
                string notFound = String.Empty;
                string[] requiredAddresses = CustomerManager.GetRequiredAddressTypes(txtCustID.Text, out Error);
                if (Error.Length > 0)
                {
                    valid = false;
                    ShowError(Error);
                }
                else
                {
                    foreach (string addType in requiredAddresses)
                    {
                        bool found = false;
                        foreach (Crownwood.Magic.Controls.TabPage tp in tcAddress.TabPages)
                        {
                            if (tp.Name == "tp" + addType.ToUpper())
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                            notFound += addType + Environment.NewLine;
                    }
                    if (notFound.Length > 0)
                    {
                        valid = false;
                        ShowInfo("M_MISSINGADDRESS", new object[] { notFound });
                    }
                }
            }
            #endregion

            //70363 row[CN.DateIn] is not getting the most recent address date. This is because when first saved the default value of 08/11/2006 is saved to the database

            DateTime dtDateIn = Date.blankDate;

            if (valid)
            {
                bool error = false;

                foreach (Crownwood.Magic.Controls.TabPage tp in tcAddress.TabPages)
                {
                    foreach (Control c in tp.Controls)
                    {
                        string addType = "";
                        dtDateIn = ((AddressTab)c).dtDateIn.Value;
                        if (tp.Name.Length > 2)
                        {
                            addType = tp.Name.Substring(2, tp.Name.Length - 2);

                            if (addresshistory.AddTest(addType, drpRelationship.SelectedValue.ToString(), dtDateIn) & (bool)tp.Tag)
                            {
                                valid = false;
                                tp.Select();
                                tp.Selected = true;
                                tp.BringToFront();
                                tp.Focus();

                                errorProvider1.SetError(tcAddress, "A new date in must be entered to create history");
                                error = true;
                            }

                        }
                    }
                }
                if (!error)
                    errorProvider1.SetError(tcAddress, String.Empty);

            }

            /* do some validation on income - shouldn't really be necessary because the field validates
			 * but won't do any harm */
            if (!IsStrictMoney(txtIncome.Text))
            {
                valid = false;
                errorProvider1.SetError(txtIncome, GetResource("M_NONNUMERIC"));
            }
            else
                errorProvider1.SetError(txtIncome, String.Empty);

            // CR903 - flag an error on the customer id field as the customer
            // type is different to the logged in branch type.
            if (DuplicateCustid)
            {
                valid = false;
                errorProvider1.SetError(txtCustID, GetResource("M_DUPLICATECUSTID"));
            }
            else
                errorProvider1.SetError(txtCustID, String.Empty);
            if (txtDependants.Value.ToString().Length.Equals(0))
            {
                valid = false;
                errorProvider1.SetError(txtIncome, GetResource("M_ENTERMANDATORY"));
            }
            else
                errorProvider1.SetError(txtDependants, String.Empty);

            if (!base.IsPositive(txtDependants.Value.ToString()))
            {
                valid = false;
                errorProvider1.SetError(txtDependants, GetResource("M_POSITIVENUM"));
            }
            else
                errorProvider1.SetError(txtDependants, String.Empty);


            if (valid)
            {


                //Save the basic details
                string acctNo = Relationship == "H" ? NewAccountNo : AccountNo;
                string acctType = Relationship == "H" ? NewAccountType : AccountType;
                string custid = Relationship == "H" ? Holder : Linked;

                //Pass this in as null if we want to save all tabs separately
                DataSet dsAllTabs = this.SaveAllTabs(custid);
                //DataSet empAndBank = null; //Don't save the employee and bank details together anymore

                CustomerManager.SaveBasicDetails(custid,
                    ((DataRowView)drpTitle.SelectedItem)[CN.CodeDescription].ToString(),
                    txtFirstName.Text,
                    txtLastName.Text,
                    txtAlias.Text,
                    acctNo,
                    Relationship,
                    dtDOB.Value,
                    acctType,
                    txtMaidenName.Text,
                    txtLoyaltyCardNo.Text,
                    Config.CountryCode,
                    Config.StoreType,
                    dsAllTabs,
                    Convert.ToString(drpMaritalStat1.SelectedValue), //((DataRowView)drpMaritalStat1.SelectedItem)[CN.MaritalStatus].ToString(),
                    Convert.ToInt32(txtDependants.Value),
                    Convert.ToString(drpNationality1.SelectedValue),
                    this.chkSms.Checked,
                    out Error);

                if (Error.Length > 0)
                {
                    valid = false;
                    ShowError(Error);
                }
                else
                {
                    if (dsAllTabs.Tables.IndexOf(TN.CustomerAdditionalDetailsResidential) >= 0)
                        m_currentDateIn = (DateTime)dsAllTabs.Tables[TN.CustomerAdditionalDetailsResidential].Rows[0][CN.DateIn];
                    else
                        m_currentDateIn = dtDateIn;

                }
                if (valid)
                {
                    ((MainForm)this.FormRoot).statusBar1.Text = "Saving customer details";

                    //Save the customer addresses
                    DataSet ds = SaveAddresses();
                    CustomerManager.SaveAddresses(Linked, ds, out Error);
                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                        valid = false;
                    }
                }

                if (valid)
                {
                    //addresses successfully saved therefore we can reset
                    //all the create history flags to unchecked.
                    foreach (Crownwood.Magic.Controls.TabPage tp in tcAddress.TabPages)
                        tp.Tag = false;
                    chxCreateHistory.Checked = false;
                    chxCopyName.Checked = false;//KD

                    /* if we came here from the new account screen and this is
                     * the holder, update the NewAccount.CustomerID property */
                    if (FormParent is NewAccount &&
                        Relationship == "H")
                    {
                        ((NewAccount)FormParent).LinkCustomer(Holder);
                    }
                    //record the fact that the customerID has been saved
                    customerSaved = true;
                }
                this._hasdatachanged = false;
            }
            Function = "End of Save";
            ((MainForm)this.FormRoot).statusBar1.Text = "";
            return valid;
        }

        private DataSet SaveBank(string custid)
        {
            //This procedure extracted from SaveEmploymentAndBank() 
            DataSet ds = null;
            DataRow r = null;

            InitialiseBank();

            ds = new DataSet();

            if (BankDetails.DataSet != null)
                BankDetails.DataSet.Tables.Remove(BankDetails);

            /* bank stuff */
            if (BankDetails.Rows.Count > 0)
                r = BankDetails.Rows[0];
            else
            {
                r = BankDetails.NewRow();
                BankDetails.Rows.Add(r);
            }

            r[CN.CustomerID] = custid;
            r[CN.BankAccountNo] = this.txtBankAcctNumber1.Text;
            r[CN.BankAccountOpened] = dtBankOpened1.Value;
            r[CN.Code] = (string)((DataRowView)drpBankAcctType1.SelectedItem)[CN.Code];
            if (drpBank1.SelectedIndex != -1)
                r[CN.BankCode] = (string)((DataRowView)this.drpBank1.SelectedItem)[CN.BankCode];

            ds.Tables.Add(BankDetails);
            return ds;

        }

        private DataSet SaveEmployment(string custid)
        {
            DataSet ds = null;
            DataRow r = null;

            InitialiseEmp();
            ds = new DataSet();

            if (EmploymentDetails.DataSet != null)
                EmploymentDetails.DataSet.Tables.Remove(EmploymentDetails);

            /* Employment stuff */
            if (EmploymentDetails.Rows.Count > 0)
                r = EmploymentDetails.Rows[0];
            else
            {
                r = EmploymentDetails.NewRow();
                EmploymentDetails.Rows.Add(r);
            }

            if (dtCurrEmpStart.Value == dtPrevEmpStart.Value)
                dtPrevEmpStart.Value = dtPrevEmpStart.Value.AddMinutes(-1);
            r[CN.CustomerID] = custid;
            r[CN.DateEmployed] = dtCurrEmpStart.Value;
            //CR 866 Changed CN.Occupation to CN.worktype            
            r[CN.WorkType] = (string)((DataRowView)drpOccupation.SelectedItem)[CN.Code];
            if (drpEmploymentStat.SelectedIndex != -1)
                r[CN.EmploymentStatus] = (string)((DataRowView)drpEmploymentStat.SelectedItem)[CN.Code];
            r[CN.PayFrequency] = (string)((DataRowView)drpPayFrequency.SelectedItem)[CN.Code];

            if (txtIncome.Text == "")
                r[CN.AnnualGross] = DBNull.Value;
            else
                r[CN.AnnualGross] = 12 * Convert.ToDouble((StripCurrency(txtIncome.Text)));
            r[CN.PersDialCode] = txtEmpTelCode.Text;
            r[CN.PersTel] = txtEmpTelNum.Text;
            r[CN.PrevDateEmployed] = dtPrevEmpStart.Value;

            //CR 866 - Thailand scoring [PC]
            r[CN.JobTitle] = txtJobTitle1.Text;
            r[CN.Organisation] = txtOrganisation1.Text;
            r[CN.Industry] = txtIndustry1.Text;
            r[CN.EducationLevel] = drpEductation1.SelectedValue;
            //End CR 866

            ds.Tables.Add(EmploymentDetails);
            return ds;
        }


        public void SetCellColour(object sender, ColourCellEventArgs e)
        {
            //do something based on the row & column to set enable flag
            int index = e.Row;
            //find the account no that corresponds to this row
            if ((string)dv[index]["AccountNumber"] == this.AccountNo)
                e.Colour = true;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BasicCustomerDetails));
            this.menuIcons = new System.Windows.Forms.ImageList(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnPrivilegeClub = new System.Windows.Forms.Button();
            this.pictureStoreCardApproved = new System.Windows.Forms.PictureBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnCreateStoreCard = new System.Windows.Forms.Button();
            this.lblCashLoan = new System.Windows.Forms.Label();
            this.btnCreateRFAccount = new System.Windows.Forms.Button();
            this.lBranch = new System.Windows.Forms.Label();
            this.drpBranch = new System.Windows.Forms.ComboBox();
            this.btnCreateCashAccount = new System.Windows.Forms.Button();
            this.btnCreateHPAccount = new System.Windows.Forms.Button();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuRecentCust = new Crownwood.Magic.Menus.MenuCommand();
            this.menuRefer = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCustomer = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCustomerCodes = new Crownwood.Magic.Menus.MenuCommand();
            this.menuLinkToAccount = new Crownwood.Magic.Menus.MenuCommand();
            this.menuUnblockCredit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuManualRF = new Crownwood.Magic.Menus.MenuCommand();
            this.menuManualHP = new Crownwood.Magic.Menus.MenuCommand();
            this.menuManualCash = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSpecialAccount = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCashLoanOveride = new Crownwood.Magic.Menus.MenuCommand();
            this.menuRevise = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSanction = new Crownwood.Magic.Menus.MenuCommand();
            this.menuS1 = new Crownwood.Magic.Menus.MenuCommand();
            this.menuS2 = new Crownwood.Magic.Menus.MenuCommand();
            this.menuUW = new Crownwood.Magic.Menus.MenuCommand();
            this.menuDC = new Crownwood.Magic.Menus.MenuCommand();
            this.allowReviseSettled = new System.Windows.Forms.Control();
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.menuReviseCashLoan = new Crownwood.Magic.Menus.MenuCommand();
            this.grpAccounts = new System.Windows.Forms.GroupBox();
            this.chxIncludeAssocAccounts = new System.Windows.Forms.CheckBox();
            this.chxIncludeSettled = new System.Windows.Forms.CheckBox();
            this.dgAccounts = new System.Windows.Forms.DataGrid();
            this.tcDetails = new Crownwood.Magic.Controls.TabControl();
            this.tpBasicDetails = new Crownwood.Magic.Controls.TabPage();
            this.gbDetails = new System.Windows.Forms.GroupBox();
            this.lblStoreCardAvailable = new System.Windows.Forms.Label();
            this.txtDependants = new System.Windows.Forms.NumericUpDown();
            this.drpNationality1 = new System.Windows.Forms.ComboBox();
            this.txtRFAvailable = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.drpMaritalStat1 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCustID = new STL.PL.CustIdTextBox();
            this.lLoyaltyCardNo = new System.Windows.Forms.Label();
            this.txtLoyaltyCardNo = new System.Windows.Forms.TextBox();
            this.txtMaidenName = new System.Windows.Forms.TextBox();
            this.lMaidenName = new System.Windows.Forms.Label();
            this.dtDOB = new System.Windows.Forms.DateTimePicker();
            this.label12 = new System.Windows.Forms.Label();
            this.labelAvailable = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtAccountNo = new STL.PL.AccountTextBox();
            this.drpRelationship = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.txtAlias = new System.Windows.Forms.TextBox();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.drpTitle = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelCredit = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lPrivilegeClubDesc = new System.Windows.Forms.Label();
            this.chxPrivilegeClub = new System.Windows.Forms.CheckBox();
            this.txtStoreCardAvailable = new System.Windows.Forms.TextBox();
            this.txtRFLimit = new System.Windows.Forms.TextBox();
            this.allowReviseCash = new System.Windows.Forms.Label();
            this.allowManual = new System.Windows.Forms.Label();
            this.allowReviseRepo = new System.Windows.Forms.Label();
            this.LoyaltyLogo_pb = new System.Windows.Forms.PictureBox();
            this.txtRFIssueNo = new System.Windows.Forms.TextBox();
            this.lRFIssueNo = new System.Windows.Forms.Label();
            this.txtBand = new System.Windows.Forms.TextBox();
            this.lBand = new System.Windows.Forms.Label();
            this.gbAddress = new System.Windows.Forms.GroupBox();
            this.chkSms = new System.Windows.Forms.CheckBox();
            this.chxCreateHistory = new System.Windows.Forms.CheckBox();
            this.chxCopyName = new System.Windows.Forms.CheckBox();
            this.panelStoreCard = new System.Windows.Forms.Panel();
            this.lNewAddress = new System.Windows.Forms.Label();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.drpAddressType = new System.Windows.Forms.ComboBox();
            this.tcAddress = new Crownwood.Magic.Controls.TabControl();
            this.tpLoyaltyScheme = new Crownwood.Magic.Controls.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tp_HcMember = new System.Windows.Forms.TabPage();
            this.btn_override = new System.Windows.Forms.Button();
            this.LoyaltyVoucher_btn = new System.Windows.Forms.Button();
            this.LoyaltyAcct_btn = new System.Windows.Forms.Button();
            this.LoyaltyStatusAcct_txt = new System.Windows.Forms.TextBox();
            this.LoyaltyStatusVoucher_txt = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.LoyaltyJoin_btn = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.loyaltymemno_mtb = new System.Windows.Forms.MaskedTextBox();
            this.loyaltyinfo_lbl = new System.Windows.Forms.Label();
            this.Loyalty_save_btn = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.LoyaltyEnd_dtp = new System.Windows.Forms.DateTimePicker();
            this.LoyaltyStart_dtp = new System.Windows.Forms.DateTimePicker();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.LoyaltyType_cmb = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LoyaltyReason_cmb = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.Reason_lbl = new System.Windows.Forms.Label();
            this.tp_HcHistroy = new System.Windows.Forms.TabPage();
            this.dgv_HChistory = new System.Windows.Forms.DataGridView();
            this.tpEmployment = new Crownwood.Magic.Controls.TabPage();
            this.gbEmployment = new System.Windows.Forms.GroupBox();
            this.txtOrganisation1 = new System.Windows.Forms.ComboBox();
            this.txtIndustry1 = new System.Windows.Forms.ComboBox();
            this.txtJobTitle1 = new System.Windows.Forms.ComboBox();
            this.lOrganisation1 = new System.Windows.Forms.Label();
            this.lIndustry1 = new System.Windows.Forms.Label();
            this.lJobTitle1 = new System.Windows.Forms.Label();
            this.drpEductation1 = new System.Windows.Forms.ComboBox();
            this.lEducation1 = new System.Windows.Forms.Label();
            this.btnSaveEmployment = new System.Windows.Forms.Button();
            this.lIncome = new System.Windows.Forms.Label();
            this.txtIncome = new System.Windows.Forms.TextBox();
            this.dtPrevEmpStart = new STL.PL.DatePicker();
            this.dtCurrEmpStart = new STL.PL.DatePicker();
            this.txtEmpTelNum = new STL.PL.PhoneNumberBox();
            this.lEmpTelNum1 = new System.Windows.Forms.Label();
            this.lEmpTelCode1 = new System.Windows.Forms.Label();
            this.txtEmpTelCode = new STL.PL.PhoneNumberBox();
            this.drpPayFrequency = new System.Windows.Forms.ComboBox();
            this.lPayFrequency1 = new System.Windows.Forms.Label();
            this.drpOccupation = new System.Windows.Forms.ComboBox();
            this.lOccupation1 = new System.Windows.Forms.Label();
            this.drpEmploymentStat = new System.Windows.Forms.ComboBox();
            this.lEmploymentStat1 = new System.Windows.Forms.Label();
            this.tpBank = new Crownwood.Magic.Controls.TabPage();
            this.gbBank = new System.Windows.Forms.GroupBox();
            this.dtBankOpened = new STL.PL.DatePicker();
            this.txtBankAcctNumber = new System.Windows.Forms.TextBox();
            this.drpBankAcctType = new System.Windows.Forms.ComboBox();
            this.lBankAcctNumber1 = new System.Windows.Forms.Label();
            this.lBankAcctType1 = new System.Windows.Forms.Label();
            this.drpBank = new System.Windows.Forms.ComboBox();
            this.lBank1 = new System.Windows.Forms.Label();
            this.tpNameHistory = new Crownwood.Magic.Controls.TabPage();
            this.gbNameHistory = new System.Windows.Forms.GroupBox();
            this.dgNameHistory = new System.Windows.Forms.DataGrid();
            this.tpAddressHistory = new Crownwood.Magic.Controls.TabPage();
            this.gbAddressHistory = new System.Windows.Forms.GroupBox();
            this.dgAddressHistory = new System.Windows.Forms.DataGrid();
            this.panel2 = new System.Windows.Forms.Panel();
            this.radAddressAudit = new System.Windows.Forms.RadioButton();
            this.radAddress = new System.Windows.Forms.RadioButton();
            this.tpTelHistory = new Crownwood.Magic.Controls.TabPage();
            this.gbTelHistory = new System.Windows.Forms.GroupBox();
            this.dgTelephoneHistory = new System.Windows.Forms.DataGrid();
            this.panel3 = new System.Windows.Forms.Panel();
            this.radTelephoneHistory = new System.Windows.Forms.RadioButton();
            this.radTelephoneAudit = new System.Windows.Forms.RadioButton();
            this.tpResidential = new Crownwood.Magic.Controls.TabPage();
            this.lTransportType1 = new System.Windows.Forms.Label();
            this.drpTransportType1 = new System.Windows.Forms.ComboBox();
            this.lDistanceFromStore1 = new System.Windows.Forms.Label();
            this.txtDistanceFromStore1 = new System.Windows.Forms.NumericUpDown();
            this.dtDateInPrevAddress1 = new STL.PL.DatePicker();
            this.btnSaveResidential = new System.Windows.Forms.Button();
            this.dtDateInCurrentAddress1 = new STL.PL.DatePicker();
            this.drpCurrentResidentialStatus1 = new System.Windows.Forms.ComboBox();
            this.lMortgage1 = new System.Windows.Forms.Label();
            this.txtMortgage1 = new System.Windows.Forms.TextBox();
            this.lPrevResidentialStatus1 = new System.Windows.Forms.Label();
            this.lPropertyType1 = new System.Windows.Forms.Label();
            this.drpPropertyType1 = new System.Windows.Forms.ComboBox();
            this.lCurrentResidentialStatus1 = new System.Windows.Forms.Label();
            this.drpPrevResidentialStatus1 = new System.Windows.Forms.ComboBox();
            this.tpFinancial = new Crownwood.Magic.Controls.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.lAdditionalExpenditure2 = new System.Windows.Forms.Label();
            this.lAdditionalExpenditure1 = new System.Windows.Forms.Label();
            this.txtAdditionalExpenditure2 = new System.Windows.Forms.TextBox();
            this.txtAdditionalExpenditure1 = new System.Windows.Forms.TextBox();
            this.txtOther1 = new System.Windows.Forms.TextBox();
            this.lTotal1 = new System.Windows.Forms.Label();
            this.txtTotal1 = new System.Windows.Forms.TextBox();
            this.lOther1 = new System.Windows.Forms.Label();
            this.lMisc1 = new System.Windows.Forms.Label();
            this.txtMisc1 = new System.Windows.Forms.TextBox();
            this.lLoans1 = new System.Windows.Forms.Label();
            this.txtLoans1 = new System.Windows.Forms.TextBox();
            this.lUtilities1 = new System.Windows.Forms.Label();
            this.txtUtilities1 = new System.Windows.Forms.TextBox();
            this.grpIncome = new System.Windows.Forms.GroupBox();
            this.txtDisposable = new System.Windows.Forms.TextBox();
            this.lDisposable = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.lAddIncome1 = new System.Windows.Forms.Label();
            this.txtAddIncome1 = new System.Windows.Forms.TextBox();
            this.lNetIncome1 = new System.Windows.Forms.Label();
            this.txtNetIncome1 = new System.Windows.Forms.TextBox();
            this.gbBankDetails = new System.Windows.Forms.GroupBox();
            this.grpGiro = new System.Windows.Forms.Panel();
            this.lPayMethod = new System.Windows.Forms.Label();
            this.drpPaymentMethod = new System.Windows.Forms.ComboBox();
            this.drpPayByGiro1 = new System.Windows.Forms.ComboBox();
            this.lBankAccountName1 = new System.Windows.Forms.Label();
            this.lGiroDueDate1 = new System.Windows.Forms.Label();
            this.txtBankAccountName1 = new System.Windows.Forms.TextBox();
            this.drpGiroDueDate1 = new System.Windows.Forms.ComboBox();
            this.lPayByGiro1 = new System.Windows.Forms.Label();
            this.btnSaveFinancial = new System.Windows.Forms.Button();
            this.dtBankOpened1 = new STL.PL.DatePicker();
            this.txtCreditCardNo1 = new STL.PL.CreditCardNo();
            this.lCreditCardNo1 = new System.Windows.Forms.Label();
            this.txtBankAcctNumber1 = new System.Windows.Forms.TextBox();
            this.drpBankAcctType1 = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.drpBank1 = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tpPhoto = new Crownwood.Magic.Controls.TabPage();
            this.btnSaveSignature = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.txtSignature = new System.Windows.Forms.TextBox();
            this.dgvPreviousPhotos = new System.Windows.Forms.DataGridView();
            this.label15 = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.btnAddSignature = new System.Windows.Forms.Button();
            this.btnAddPicture = new System.Windows.Forms.Button();
            this.btnSavePicture = new System.Windows.Forms.Button();
            this.btnTakePicture = new System.Windows.Forms.Button();
            this.pbSignature = new System.Windows.Forms.PictureBox();
            this.pbPhoto = new System.Windows.Forms.PictureBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProvider2 = new System.Windows.Forms.ErrorProvider(this.components);
            this.errProRFCreateInterval = new System.Windows.Forms.ErrorProvider(this.components);
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.Copy = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.CopyName = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureStoreCardApproved)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            this.grpAccounts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
            this.tcDetails.SuspendLayout();
            this.tpBasicDetails.SuspendLayout();
            this.gbDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDependants)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LoyaltyLogo_pb)).BeginInit();
            this.gbAddress.SuspendLayout();
            this.tpLoyaltyScheme.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tp_HcMember.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel1.SuspendLayout();
            this.tp_HcHistroy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_HChistory)).BeginInit();
            this.tpEmployment.SuspendLayout();
            this.gbEmployment.SuspendLayout();
            this.tpBank.SuspendLayout();
            this.gbBank.SuspendLayout();
            this.tpNameHistory.SuspendLayout();
            this.gbNameHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgNameHistory)).BeginInit();
            this.tpAddressHistory.SuspendLayout();
            this.gbAddressHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAddressHistory)).BeginInit();
            this.panel2.SuspendLayout();
            this.tpTelHistory.SuspendLayout();
            this.gbTelHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgTelephoneHistory)).BeginInit();
            this.panel3.SuspendLayout();
            this.tpResidential.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDistanceFromStore1)).BeginInit();
            this.tpFinancial.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.grpIncome.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.gbBankDetails.SuspendLayout();
            this.grpGiro.SuspendLayout();
            this.tpPhoto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreviousPhotos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSignature)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPhoto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProRFCreateInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // menuIcons
            // 
            this.menuIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("menuIcons.ImageStream")));
            this.menuIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.menuIcons.Images.SetKeyName(0, "");
            this.menuIcons.Images.SetKeyName(1, "");
            this.menuIcons.Images.SetKeyName(2, "");
            this.menuIcons.Images.SetKeyName(3, "");
            this.menuIcons.Images.SetKeyName(4, "refresh.ico");
            this.menuIcons.Images.SetKeyName(5, "");
            this.menuIcons.Images.SetKeyName(6, "");
            this.menuIcons.Images.SetKeyName(7, "GoldStar.bmp");
            this.menuIcons.Images.SetKeyName(8, "PcLoyalty.bmp");
            // 
            // btnPrivilegeClub
            // 
            this.btnPrivilegeClub.BackColor = System.Drawing.SystemColors.Control;
            this.btnPrivilegeClub.FlatAppearance.BorderSize = 0;
            this.btnPrivilegeClub.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrivilegeClub.ImageIndex = 7;
            this.btnPrivilegeClub.ImageList = this.menuIcons;
            this.btnPrivilegeClub.Location = new System.Drawing.Point(296, 188);
            this.btnPrivilegeClub.Name = "btnPrivilegeClub";
            this.btnPrivilegeClub.Size = new System.Drawing.Size(19, 21);
            this.btnPrivilegeClub.TabIndex = 70;
            this.btnPrivilegeClub.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.toolTip1.SetToolTip(this.btnPrivilegeClub, "Privilege/Loyalty Club");
            this.btnPrivilegeClub.UseVisualStyleBackColor = true;
            // 
            // pictureStoreCardApproved
            // 
            this.pictureStoreCardApproved.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureStoreCardApproved.Image = ((System.Drawing.Image)(resources.GetObject("pictureStoreCardApproved.Image")));
            this.pictureStoreCardApproved.Location = new System.Drawing.Point(319, 25);
            this.pictureStoreCardApproved.Name = "pictureStoreCardApproved";
            this.pictureStoreCardApproved.Size = new System.Drawing.Size(26, 23);
            this.pictureStoreCardApproved.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureStoreCardApproved.TabIndex = 50;
            this.pictureStoreCardApproved.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureStoreCardApproved, "Approved for StoreCard");
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnCreateStoreCard);
            this.groupBox5.Controls.Add(this.pictureStoreCardApproved);
            this.groupBox5.Controls.Add(this.lblCashLoan);
            this.groupBox5.Controls.Add(this.btnCreateRFAccount);
            this.groupBox5.Controls.Add(this.lBranch);
            this.groupBox5.Controls.Add(this.drpBranch);
            this.groupBox5.Controls.Add(this.btnCreateCashAccount);
            this.groupBox5.Controls.Add(this.btnCreateHPAccount);
            this.groupBox5.Location = new System.Drawing.Point(432, 240);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(347, 51);
            this.groupBox5.TabIndex = 43;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Open New Account";
            // 
            // btnCreateStoreCard
            // 
            this.btnCreateStoreCard.Enabled = false;
            this.btnCreateStoreCard.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCreateStoreCard.Location = new System.Drawing.Point(274, 24);
            this.btnCreateStoreCard.Name = "btnCreateStoreCard";
            this.btnCreateStoreCard.Size = new System.Drawing.Size(46, 24);
            this.btnCreateStoreCard.TabIndex = 49;
            this.btnCreateStoreCard.Text = "Store";
            this.btnCreateStoreCard.Visible = false;
            this.btnCreateStoreCard.Click += new System.EventHandler(this.btnCreateStoreCard_Click);
            // 
            // lblCashLoan
            // 
            this.lblCashLoan.AutoSize = true;
            this.lblCashLoan.BackColor = System.Drawing.SystemColors.Control;
            this.lblCashLoan.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
            this.lblCashLoan.ForeColor = System.Drawing.Color.Green;
            this.lblCashLoan.Location = new System.Drawing.Point(119, 9);
            this.lblCashLoan.Name = "lblCashLoan";
            this.lblCashLoan.Size = new System.Drawing.Size(226, 15);
            this.lblCashLoan.TabIndex = 51;
            this.lblCashLoan.Text = "** Customer is qualified for Cash Loan **";
            this.lblCashLoan.Visible = false;
            // 
            // btnCreateRFAccount
            // 
            this.btnCreateRFAccount.Enabled = false;
            this.btnCreateRFAccount.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCreateRFAccount.Location = new System.Drawing.Point(170, 25);
            this.btnCreateRFAccount.Name = "btnCreateRFAccount";
            this.btnCreateRFAccount.Size = new System.Drawing.Size(46, 24);
            this.btnCreateRFAccount.TabIndex = 43;
            this.btnCreateRFAccount.Text = "RF";
            this.btnCreateRFAccount.Visible = false;
            this.btnCreateRFAccount.Click += new System.EventHandler(this.btnCreateRFAccount_Click);
            // 
            // lBranch
            // 
            this.lBranch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lBranch.Location = new System.Drawing.Point(7, 34);
            this.lBranch.Name = "lBranch";
            this.lBranch.Size = new System.Drawing.Size(48, 15);
            this.lBranch.TabIndex = 47;
            this.lBranch.Text = "Branch";
            this.lBranch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpBranch
            // 
            this.drpBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranch.DropDownWidth = 48;
            this.drpBranch.Enabled = false;
            this.drpBranch.Location = new System.Drawing.Point(55, 25);
            this.drpBranch.Name = "drpBranch";
            this.drpBranch.Size = new System.Drawing.Size(58, 23);
            this.drpBranch.TabIndex = 40;
            this.drpBranch.SelectedIndexChanged += new System.EventHandler(this.drpBranch_SelectedIndexChanged);
            // 
            // btnCreateCashAccount
            // 
            this.btnCreateCashAccount.Enabled = false;
            this.btnCreateCashAccount.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCreateCashAccount.Location = new System.Drawing.Point(118, 24);
            this.btnCreateCashAccount.Name = "btnCreateCashAccount";
            this.btnCreateCashAccount.Size = new System.Drawing.Size(46, 24);
            this.btnCreateCashAccount.TabIndex = 45;
            this.btnCreateCashAccount.Text = "CASH";
            this.btnCreateCashAccount.Visible = false;
            this.btnCreateCashAccount.Click += new System.EventHandler(this.btnCreateCashAccount_Click);
            // 
            // btnCreateHPAccount
            // 
            this.btnCreateHPAccount.Enabled = false;
            this.btnCreateHPAccount.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCreateHPAccount.Location = new System.Drawing.Point(222, 24);
            this.btnCreateHPAccount.Name = "btnCreateHPAccount";
            this.btnCreateHPAccount.Size = new System.Drawing.Size(46, 24);
            this.btnCreateHPAccount.TabIndex = 44;
            this.btnCreateHPAccount.Text = "HP";
            this.btnCreateHPAccount.Visible = false;
            this.btnCreateHPAccount.Click += new System.EventHandler(this.btnCreateHPAccount_Click);
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuSave,
            this.menuExit,
            this.menuRecentCust});
            this.menuFile.Text = "&File";
            // 
            // menuSave
            // 
            this.menuSave.Description = "MenuItem";
            this.menuSave.ImageIndex = 2;
            this.menuSave.ImageList = this.menuIcons;
            this.menuSave.Text = "&Save";
            this.menuSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.ImageIndex = 1;
            this.menuExit.ImageList = this.menuIcons;
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuRecentCust
            // 
            this.menuRecentCust.Description = "MenuItem";
            this.menuRecentCust.Text = "Recent &Customers";
            // 
            // menuRefer
            // 
            this.menuRefer.Description = "MenuItem";
            this.menuRefer.Enabled = false;
            this.menuRefer.ImageIndex = 0;
            this.menuRefer.ImageList = this.menuIcons;
            this.menuRefer.Text = "&Refer Rejected Account";
            this.menuRefer.Visible = false;
            // 
            // menuCustomer
            // 
            this.menuCustomer.Description = "MenuItem";
            this.menuCustomer.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuCustomerCodes,
            this.menuLinkToAccount,
            this.menuUnblockCredit,
            this.menuManualRF,
            this.menuManualHP,
            this.menuManualCash,
            this.menuSpecialAccount,
            this.menuCashLoanOveride});
            this.menuCustomer.Text = "C&ustomer";
            // 
            // menuCustomerCodes
            // 
            this.menuCustomerCodes.Description = "MenuItem";
            this.menuCustomerCodes.Text = "&CustomerCodes";
            this.menuCustomerCodes.Click += new System.EventHandler(this.menuCustomerCodes_Click);
            // 
            // menuLinkToAccount
            // 
            this.menuLinkToAccount.Description = "MenuItem";
            this.menuLinkToAccount.ImageIndex = 6;
            this.menuLinkToAccount.ImageList = this.menuIcons;
            this.menuLinkToAccount.Text = "&Link To Account";
            this.menuLinkToAccount.Click += new System.EventHandler(this.menuLinkToAccount_Click);
            // 
            // menuUnblockCredit
            // 
            this.menuUnblockCredit.Description = "MenuItem";
            this.menuUnblockCredit.Enabled = false;
            this.menuUnblockCredit.Text = "Unblock Credit";
            this.menuUnblockCredit.Visible = false;
            this.menuUnblockCredit.Click += new System.EventHandler(this.menuUnblockCredit_Click);
            // 
            // menuManualRF
            // 
            this.menuManualRF.Description = "MenuItem";
            this.menuManualRF.Enabled = false;
            this.menuManualRF.Text = "Create Manual &RF A/C Number";
            this.menuManualRF.Visible = false;
            this.menuManualRF.Click += new System.EventHandler(this.menuManualRF_Click);
            // 
            // menuManualHP
            // 
            this.menuManualHP.Description = "MenuItem";
            this.menuManualHP.Text = "Create Manual &HP A/C Number";
            this.menuManualHP.Visible = false;
            this.menuManualHP.Click += new System.EventHandler(this.menuManualHP_Click);
            // 
            // menuManualCash
            // 
            this.menuManualCash.Description = "MenuItem";
            this.menuManualCash.Text = "Create Manual &CASH A/C Number";
            this.menuManualCash.Visible = false;
            this.menuManualCash.Click += new System.EventHandler(this.menuManualCash_Click);
            // 
            // menuSpecialAccount
            // 
            this.menuSpecialAccount.Description = "MenuItem";
            this.menuSpecialAccount.Text = "Generate &Special Account";
            this.menuSpecialAccount.Visible = false;
            this.menuSpecialAccount.Click += new System.EventHandler(this.menuSpecialAccount_Click);
            // 
            // menuCashLoanOveride
            // 
            this.menuCashLoanOveride.Description = "MenuItem";
            this.menuCashLoanOveride.Text = "Unblock Cash Loan";
            this.menuCashLoanOveride.Visible = false;
            this.menuCashLoanOveride.Click += new System.EventHandler(this.menuCashLoanOveride_Click);
            // 
            // menuRevise
            // 
            this.menuRevise.Description = "MenuItem";
            this.menuRevise.Enabled = false;
            this.menuRevise.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuCustomerCodes});
            this.menuRevise.Text = "C&ustomer";
            this.menuRevise.Visible = false;
            // 
            // menuSanction
            // 
            this.menuSanction.Description = "MenuItem";
            this.menuSanction.Enabled = false;
            this.menuSanction.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuCustomerCodes});
            this.menuSanction.Text = "C&ustomer";
            this.menuSanction.Visible = false;
            // 
            // menuS1
            // 
            this.menuS1.Description = "MenuItem";
            this.menuS1.Enabled = false;
            this.menuS1.Text = "Recent &Customers";
            // 
            // menuS2
            // 
            this.menuS2.Description = "MenuItem";
            this.menuS2.Enabled = false;
            this.menuS2.Text = "Recent &Customers";
            // 
            // menuUW
            // 
            this.menuUW.Description = "MenuItem";
            this.menuUW.Enabled = false;
            this.menuUW.Text = "Recent &Customers";
            // 
            // menuDC
            // 
            this.menuDC.Description = "MenuItem";
            this.menuDC.Enabled = false;
            this.menuDC.Text = "Recent &Customers";
            // 
            // allowReviseSettled
            // 
            this.allowReviseSettled.Enabled = false;
            this.allowReviseSettled.Location = new System.Drawing.Point(0, 0);
            this.allowReviseSettled.Name = "allowReviseSettled";
            this.allowReviseSettled.Size = new System.Drawing.Size(0, 0);
            this.allowReviseSettled.TabIndex = 0;
            // 
            // dataGrid1
            // 
            this.dataGrid1.CaptionText = "Customer Telephone";
            this.dataGrid1.DataMember = "";
            this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid1.Location = new System.Drawing.Point(16, 24);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.ReadOnly = true;
            this.dataGrid1.Size = new System.Drawing.Size(728, 232);
            this.dataGrid1.TabIndex = 0;
            // 
            // menuReviseCashLoan
            // 
            this.menuReviseCashLoan.Description = "MenuItem";
            this.menuReviseCashLoan.Enabled = false;
            this.menuReviseCashLoan.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuCustomerCodes});
            this.menuReviseCashLoan.Text = "Cash Loan";
            this.menuReviseCashLoan.Visible = false;
            // 
            // grpAccounts
            // 
            this.grpAccounts.BackColor = System.Drawing.SystemColors.Control;
            this.grpAccounts.Controls.Add(this.chxIncludeAssocAccounts);
            this.grpAccounts.Controls.Add(this.chxIncludeSettled);
            this.grpAccounts.Controls.Add(this.dgAccounts);
            this.grpAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAccounts.Location = new System.Drawing.Point(0, 318);
            this.grpAccounts.Name = "grpAccounts";
            this.grpAccounts.Size = new System.Drawing.Size(841, 159);
            this.grpAccounts.TabIndex = 2;
            this.grpAccounts.TabStop = false;
            this.grpAccounts.Text = "Accounts";
            // 
            // chxIncludeAssocAccounts
            // 
            this.chxIncludeAssocAccounts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chxIncludeAssocAccounts.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.chxIncludeAssocAccounts.Checked = true;
            this.chxIncludeAssocAccounts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chxIncludeAssocAccounts.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.chxIncludeAssocAccounts.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chxIncludeAssocAccounts.Location = new System.Drawing.Point(441, 20);
            this.chxIncludeAssocAccounts.Name = "chxIncludeAssocAccounts";
            this.chxIncludeAssocAccounts.Size = new System.Drawing.Size(176, 16);
            this.chxIncludeAssocAccounts.TabIndex = 1;
            this.chxIncludeAssocAccounts.Text = "Include Associated Accounts";
            this.chxIncludeAssocAccounts.UseVisualStyleBackColor = false;
            this.chxIncludeAssocAccounts.CheckedChanged += new System.EventHandler(this.chxIncludeAssocAccounts_CheckedChanged);
            // 
            // chxIncludeSettled
            // 
            this.chxIncludeSettled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chxIncludeSettled.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.chxIncludeSettled.Checked = true;
            this.chxIncludeSettled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chxIncludeSettled.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.chxIncludeSettled.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chxIncludeSettled.Location = new System.Drawing.Point(657, 20);
            this.chxIncludeSettled.Name = "chxIncludeSettled";
            this.chxIncludeSettled.Size = new System.Drawing.Size(152, 16);
            this.chxIncludeSettled.TabIndex = 0;
            this.chxIncludeSettled.Text = "Include Settled Accounts";
            this.chxIncludeSettled.UseVisualStyleBackColor = false;
            this.chxIncludeSettled.CheckedChanged += new System.EventHandler(this.chxIncludeSettled_CheckedChanged);
            // 
            // dgAccounts
            // 
            this.dgAccounts.DataMember = "";
            this.dgAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAccounts.Location = new System.Drawing.Point(3, 16);
            this.dgAccounts.Name = "dgAccounts";
            this.dgAccounts.PreferredRowHeight = 10;
            this.dgAccounts.ReadOnly = true;
            this.dgAccounts.Size = new System.Drawing.Size(835, 140);
            this.dgAccounts.TabIndex = 0;
            this.dgAccounts.TabStop = false;
            this.dgAccounts.CurrentCellChanged += new System.EventHandler(this.dgAccounts_CurrentCellChanged);
            this.dgAccounts.DataSourceChanged += new System.EventHandler(this.dgAccounts_DataSourceChanged);
            this.dgAccounts.Click += new System.EventHandler(this.dgAccounts_Click);
            this.dgAccounts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgAccounts_MouseUp);
            // 
            // tcDetails
            // 
            this.tcDetails.AutoScroll = true;
            this.tcDetails.Dock = System.Windows.Forms.DockStyle.Top;
            this.tcDetails.IDEPixelArea = true;
            this.tcDetails.Location = new System.Drawing.Point(0, 0);
            this.tcDetails.Name = "tcDetails";
            this.tcDetails.PositionTop = true;
            this.tcDetails.SelectedIndex = 0;
            this.tcDetails.SelectedTab = this.tpBasicDetails;
            this.tcDetails.ShowArrows = true;
            this.tcDetails.ShrinkPagesToFit = false;
            this.tcDetails.Size = new System.Drawing.Size(841, 318);
            this.tcDetails.TabIndex = 2;
            this.tcDetails.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpBasicDetails,
            this.tpLoyaltyScheme,
            this.tpEmployment,
            this.tpBank,
            this.tpNameHistory,
            this.tpAddressHistory,
            this.tpTelHistory,
            this.tpResidential,
            this.tpFinancial,
            this.tpPhoto});
            this.tcDetails.SelectionChanged += new System.EventHandler(this.tcDetails_SelectionChanged);
            // 
            // tpBasicDetails
            // 
            this.tpBasicDetails.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tpBasicDetails.Controls.Add(this.gbDetails);
            this.tpBasicDetails.Controls.Add(this.LoyaltyLogo_pb);
            this.tpBasicDetails.Controls.Add(this.txtRFIssueNo);
            this.tpBasicDetails.Controls.Add(this.lRFIssueNo);
            this.tpBasicDetails.Controls.Add(this.txtBand);
            this.tpBasicDetails.Controls.Add(this.lBand);
            this.tpBasicDetails.Controls.Add(this.groupBox5);
            this.tpBasicDetails.Controls.Add(this.gbAddress);
            this.tpBasicDetails.Location = new System.Drawing.Point(0, 25);
            this.tpBasicDetails.Name = "tpBasicDetails";
            this.tpBasicDetails.Size = new System.Drawing.Size(841, 293);
            this.tpBasicDetails.TabIndex = 3;
            this.tpBasicDetails.Title = "Main Details";
            // 
            // gbDetails
            // 
            this.gbDetails.Controls.Add(this.lblStoreCardAvailable);
            this.gbDetails.Controls.Add(this.btnPrivilegeClub);
            this.gbDetails.Controls.Add(this.txtDependants);
            this.gbDetails.Controls.Add(this.drpNationality1);
            this.gbDetails.Controls.Add(this.txtRFAvailable);
            this.gbDetails.Controls.Add(this.label10);
            this.gbDetails.Controls.Add(this.label7);
            this.gbDetails.Controls.Add(this.drpMaritalStat1);
            this.gbDetails.Controls.Add(this.label6);
            this.gbDetails.Controls.Add(this.txtCustID);
            this.gbDetails.Controls.Add(this.lLoyaltyCardNo);
            this.gbDetails.Controls.Add(this.txtLoyaltyCardNo);
            this.gbDetails.Controls.Add(this.txtMaidenName);
            this.gbDetails.Controls.Add(this.lMaidenName);
            this.gbDetails.Controls.Add(this.dtDOB);
            this.gbDetails.Controls.Add(this.label12);
            this.gbDetails.Controls.Add(this.labelAvailable);
            this.gbDetails.Controls.Add(this.label8);
            this.gbDetails.Controls.Add(this.txtAccountNo);
            this.gbDetails.Controls.Add(this.drpRelationship);
            this.gbDetails.Controls.Add(this.label9);
            this.gbDetails.Controls.Add(this.txtFirstName);
            this.gbDetails.Controls.Add(this.txtAlias);
            this.gbDetails.Controls.Add(this.txtLastName);
            this.gbDetails.Controls.Add(this.drpTitle);
            this.gbDetails.Controls.Add(this.label5);
            this.gbDetails.Controls.Add(this.label4);
            this.gbDetails.Controls.Add(this.label3);
            this.gbDetails.Controls.Add(this.label2);
            this.gbDetails.Controls.Add(this.label1);
            this.gbDetails.Controls.Add(this.labelCredit);
            this.gbDetails.Controls.Add(this.btnSave);
            this.gbDetails.Controls.Add(this.btnRefresh);
            this.gbDetails.Controls.Add(this.lPrivilegeClubDesc);
            this.gbDetails.Controls.Add(this.chxPrivilegeClub);
            this.gbDetails.Controls.Add(this.txtStoreCardAvailable);
            this.gbDetails.Controls.Add(this.txtRFLimit);
            this.gbDetails.Controls.Add(this.allowReviseCash);
            this.gbDetails.Controls.Add(this.allowManual);
            this.gbDetails.Controls.Add(this.allowReviseRepo);
            this.gbDetails.Location = new System.Drawing.Point(4, -1);
            this.gbDetails.Name = "gbDetails";
            this.gbDetails.Size = new System.Drawing.Size(321, 291);
            this.gbDetails.TabIndex = 2;
            this.gbDetails.TabStop = false;
            // 
            // lblStoreCardAvailable
            // 
            this.lblStoreCardAvailable.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblStoreCardAvailable.Location = new System.Drawing.Point(205, 245);
            this.lblStoreCardAvailable.Name = "lblStoreCardAvailable";
            this.lblStoreCardAvailable.Size = new System.Drawing.Size(116, 16);
            this.lblStoreCardAvailable.TabIndex = 73;
            this.lblStoreCardAvailable.Text = "Store Card Available";
            this.lblStoreCardAvailable.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtDependants
            // 
            this.txtDependants.Location = new System.Drawing.Point(113, 221);
            this.txtDependants.Name = "txtDependants";
            this.txtDependants.Size = new System.Drawing.Size(67, 23);
            this.txtDependants.TabIndex = 69;
            // 
            // drpNationality1
            // 
            this.drpNationality1.FormattingEnabled = true;
            this.drpNationality1.Location = new System.Drawing.Point(190, 221);
            this.drpNationality1.Name = "drpNationality1";
            this.drpNationality1.Size = new System.Drawing.Size(121, 23);
            this.drpNationality1.TabIndex = 12;
            this.drpNationality1.SelectedValueChanged += new System.EventHandler(this.drpNationality1_SelectedValueChanged);
            // 
            // txtRFAvailable
            // 
            this.txtRFAvailable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtRFAvailable.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.txtRFAvailable.Location = new System.Drawing.Point(109, 263);
            this.txtRFAvailable.MaxLength = 25;
            this.txtRFAvailable.Name = "txtRFAvailable";
            this.txtRFAvailable.ReadOnly = true;
            this.txtRFAvailable.Size = new System.Drawing.Size(96, 20);
            this.txtRFAvailable.TabIndex = 11;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(188, 205);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(68, 15);
            this.label10.TabIndex = 68;
            this.label10.Text = "Nationality:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(106, 205);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 15);
            this.label7.TabIndex = 66;
            this.label7.Text = "# Dependants:";
            // 
            // drpMaritalStat1
            // 
            this.drpMaritalStat1.FormattingEnabled = true;
            this.drpMaritalStat1.Location = new System.Drawing.Point(8, 221);
            this.drpMaritalStat1.Name = "drpMaritalStat1";
            this.drpMaritalStat1.Size = new System.Drawing.Size(99, 23);
            this.drpMaritalStat1.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 206);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 15);
            this.label6.TabIndex = 63;
            this.label6.Text = "Marital Status:";
            // 
            // txtCustID
            // 
            this.txtCustID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCustID.Location = new System.Drawing.Point(6, 69);
            this.txtCustID.MaxLength = 20;
            this.txtCustID.Name = "txtCustID";
            this.txtCustID.Size = new System.Drawing.Size(120, 23);
            this.txtCustID.TabIndex = 59;
            this.txtCustID.TextChanged += new System.EventHandler(this.txtCustID_TextChanged);
            this.txtCustID.Leave += new System.EventHandler(this.txtCustID_Leave);
            // 
            // lLoyaltyCardNo
            // 
            this.lLoyaltyCardNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lLoyaltyCardNo.Location = new System.Drawing.Point(168, 173);
            this.lLoyaltyCardNo.Name = "lLoyaltyCardNo";
            this.lLoyaltyCardNo.Size = new System.Drawing.Size(122, 13);
            this.lLoyaltyCardNo.TabIndex = 52;
            this.lLoyaltyCardNo.Text = "Loyalty Card No:";
            this.lLoyaltyCardNo.Visible = false;
            // 
            // txtLoyaltyCardNo
            // 
            this.txtLoyaltyCardNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtLoyaltyCardNo.Location = new System.Drawing.Point(168, 187);
            this.txtLoyaltyCardNo.MaxLength = 16;
            this.txtLoyaltyCardNo.Name = "txtLoyaltyCardNo";
            this.txtLoyaltyCardNo.Size = new System.Drawing.Size(113, 20);
            this.txtLoyaltyCardNo.TabIndex = 9;
            this.txtLoyaltyCardNo.Tag = "lMoreRewards1";
            this.txtLoyaltyCardNo.Visible = false;
            // 
            // txtMaidenName
            // 
            this.txtMaidenName.Location = new System.Drawing.Point(7, 184);
            this.txtMaidenName.MaxLength = 60;
            this.txtMaidenName.Name = "txtMaidenName";
            this.txtMaidenName.Size = new System.Drawing.Size(152, 23);
            this.txtMaidenName.TabIndex = 8;
            this.txtMaidenName.Visible = false;
            // 
            // lMaidenName
            // 
            this.lMaidenName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lMaidenName.Location = new System.Drawing.Point(4, 170);
            this.lMaidenName.Name = "lMaidenName";
            this.lMaidenName.Size = new System.Drawing.Size(100, 13);
            this.lMaidenName.TabIndex = 51;
            this.lMaidenName.Text = "Maiden Name:";
            // 
            // dtDOB
            // 
            this.dtDOB.CustomFormat = "ddd dd MMM yyyy";
            this.dtDOB.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDOB.Location = new System.Drawing.Point(168, 66);
            this.dtDOB.Name = "dtDOB";
            this.dtDOB.Size = new System.Drawing.Size(147, 23);
            this.dtDOB.TabIndex = 3;
            this.dtDOB.ValueChanged += new System.EventHandler(this.dtDOB_ValueChanged);
            // 
            // label12
            // 
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(168, 50);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(100, 16);
            this.label12.TabIndex = 48;
            this.label12.Text = "Date Born:";
            // 
            // labelAvailable
            // 
            this.labelAvailable.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelAvailable.Location = new System.Drawing.Point(110, 245);
            this.labelAvailable.Name = "labelAvailable";
            this.labelAvailable.Size = new System.Drawing.Size(95, 16);
            this.labelAvailable.TabIndex = 46;
            this.labelAvailable.Text = "Credit Available";
            this.labelAvailable.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label8
            // 
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(4, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(96, 16);
            this.label8.TabIndex = 41;
            this.label8.Text = "Linking account:";
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.Location = new System.Drawing.Point(6, 23);
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.PreventPaste = false;
            this.txtAccountNo.ReadOnly = true;
            this.txtAccountNo.Size = new System.Drawing.Size(94, 23);
            this.txtAccountNo.TabIndex = 0;
            this.txtAccountNo.Text = "000-0000-0000-0";
            this.txtAccountNo.TextChanged += new System.EventHandler(this.txtAccountNo_TextChanged);
            // 
            // drpRelationship
            // 
            this.drpRelationship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpRelationship.ItemHeight = 15;
            this.drpRelationship.Location = new System.Drawing.Point(114, 23);
            this.drpRelationship.Name = "drpRelationship";
            this.drpRelationship.Size = new System.Drawing.Size(126, 23);
            this.drpRelationship.TabIndex = 1;
            this.drpRelationship.SelectedIndexChanged += new System.EventHandler(this.drpRelationship_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(111, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 16);
            this.label9.TabIndex = 38;
            this.label9.Text = "Relationship:";
            // 
            // txtFirstName
            // 
            this.txtFirstName.Location = new System.Drawing.Point(168, 106);
            this.txtFirstName.MaxLength = 30;
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(147, 23);
            this.txtFirstName.TabIndex = 5;
            this.txtFirstName.TextChanged += new System.EventHandler(this.txtLastName_TextChanged);
            // 
            // txtAlias
            // 
            this.txtAlias.Location = new System.Drawing.Point(168, 146);
            this.txtAlias.MaxLength = 25;
            this.txtAlias.Name = "txtAlias";
            this.txtAlias.Size = new System.Drawing.Size(147, 23);
            this.txtAlias.TabIndex = 7;
            // 
            // txtLastName
            // 
            this.txtLastName.Location = new System.Drawing.Point(6, 146);
            this.txtLastName.MaxLength = 60;
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(152, 23);
            this.txtLastName.TabIndex = 6;
            this.txtLastName.TextChanged += new System.EventHandler(this.txtLastName_TextChanged);
            // 
            // drpTitle
            // 
            this.drpTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpTitle.ItemHeight = 15;
            this.drpTitle.Location = new System.Drawing.Point(6, 106);
            this.drpTitle.Name = "drpTitle";
            this.drpTitle.Size = new System.Drawing.Size(120, 23);
            this.drpTitle.TabIndex = 4;
            this.drpTitle.SelectedIndexChanged += new System.EventHandler(this.drpTitle_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(3, 92);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 16);
            this.label5.TabIndex = 27;
            this.label5.Text = "Title:";
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(168, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 16);
            this.label4.TabIndex = 26;
            this.label4.Text = "First Name:";
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(4, 132);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 16);
            this.label3.TabIndex = 25;
            this.label3.Text = "Last Name:";
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(168, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 16);
            this.label2.TabIndex = 24;
            this.label2.Text = "Alias:";
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(3, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 23;
            this.label1.Text = "Customer ID:";
            // 
            // labelCredit
            // 
            this.labelCredit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelCredit.Location = new System.Drawing.Point(6, 245);
            this.labelCredit.Name = "labelCredit";
            this.labelCredit.Size = new System.Drawing.Size(100, 16);
            this.labelCredit.TabIndex = 44;
            this.labelCredit.Text = "Credit Limit";
            this.labelCredit.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave.Location = new System.Drawing.Point(287, 16);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 35;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnRefresh.ImageIndex = 4;
            this.btnRefresh.ImageList = this.menuIcons;
            this.btnRefresh.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRefresh.Location = new System.Drawing.Point(251, 16);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(24, 24);
            this.btnRefresh.TabIndex = 49;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lPrivilegeClubDesc
            // 
            this.lPrivilegeClubDesc.Location = new System.Drawing.Point(246, 137);
            this.lPrivilegeClubDesc.Name = "lPrivilegeClubDesc";
            this.lPrivilegeClubDesc.Size = new System.Drawing.Size(69, 11);
            this.lPrivilegeClubDesc.TabIndex = 54;
            this.lPrivilegeClubDesc.Visible = false;
            // 
            // chxPrivilegeClub
            // 
            this.chxPrivilegeClub.Enabled = false;
            this.chxPrivilegeClub.Location = new System.Drawing.Point(8, 224);
            this.chxPrivilegeClub.Name = "chxPrivilegeClub";
            this.chxPrivilegeClub.Size = new System.Drawing.Size(16, 16);
            this.chxPrivilegeClub.TabIndex = 53;
            // 
            // txtStoreCardAvailable
            // 
            this.txtStoreCardAvailable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtStoreCardAvailable.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.txtStoreCardAvailable.Location = new System.Drawing.Point(211, 263);
            this.txtStoreCardAvailable.MaxLength = 25;
            this.txtStoreCardAvailable.Name = "txtStoreCardAvailable";
            this.txtStoreCardAvailable.ReadOnly = true;
            this.txtStoreCardAvailable.Size = new System.Drawing.Size(96, 20);
            this.txtStoreCardAvailable.TabIndex = 72;
            this.txtStoreCardAvailable.MouseHover += new System.EventHandler(this.txtStoreCardAvailable_MouseHover);
            // 
            // txtRFLimit
            // 
            this.txtRFLimit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtRFLimit.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.txtRFLimit.Location = new System.Drawing.Point(7, 263);
            this.txtRFLimit.MaxLength = 25;
            this.txtRFLimit.Name = "txtRFLimit";
            this.txtRFLimit.ReadOnly = true;
            this.txtRFLimit.Size = new System.Drawing.Size(96, 20);
            this.txtRFLimit.TabIndex = 10;
            // 
            // allowReviseCash
            // 
            this.allowReviseCash.BackColor = System.Drawing.SystemColors.ControlDark;
            this.allowReviseCash.Enabled = false;
            this.allowReviseCash.Location = new System.Drawing.Point(291, 20);
            this.allowReviseCash.Name = "allowReviseCash";
            this.allowReviseCash.Size = new System.Drawing.Size(16, 18);
            this.allowReviseCash.TabIndex = 62;
            this.allowReviseCash.Visible = false;
            // 
            // allowManual
            // 
            this.allowManual.BackColor = System.Drawing.SystemColors.ControlDark;
            this.allowManual.Enabled = false;
            this.allowManual.Location = new System.Drawing.Point(291, 19);
            this.allowManual.Name = "allowManual";
            this.allowManual.Size = new System.Drawing.Size(16, 16);
            this.allowManual.TabIndex = 58;
            this.allowManual.Visible = false;
            // 
            // allowReviseRepo
            // 
            this.allowReviseRepo.BackColor = System.Drawing.SystemColors.ControlDark;
            this.allowReviseRepo.Enabled = false;
            this.allowReviseRepo.Location = new System.Drawing.Point(291, 20);
            this.allowReviseRepo.Name = "allowReviseRepo";
            this.allowReviseRepo.Size = new System.Drawing.Size(16, 16);
            this.allowReviseRepo.TabIndex = 57;
            this.allowReviseRepo.Visible = false;
            // 
            // LoyaltyLogo_pb
            // 
            this.LoyaltyLogo_pb.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(132)))), ((int)(((byte)(0)))));
            this.LoyaltyLogo_pb.Image = ((System.Drawing.Image)(resources.GetObject("LoyaltyLogo_pb.Image")));
            this.LoyaltyLogo_pb.InitialImage = null;
            this.LoyaltyLogo_pb.Location = new System.Drawing.Point(332, 233);
            this.LoyaltyLogo_pb.Name = "LoyaltyLogo_pb";
            this.LoyaltyLogo_pb.Size = new System.Drawing.Size(93, 23);
            this.LoyaltyLogo_pb.TabIndex = 61;
            this.LoyaltyLogo_pb.TabStop = false;
            this.LoyaltyLogo_pb.Visible = false;
            // 
            // txtRFIssueNo
            // 
            this.txtRFIssueNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtRFIssueNo.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.txtRFIssueNo.Location = new System.Drawing.Point(393, 269);
            this.txtRFIssueNo.MaxLength = 5;
            this.txtRFIssueNo.Name = "txtRFIssueNo";
            this.txtRFIssueNo.ReadOnly = true;
            this.txtRFIssueNo.Size = new System.Drawing.Size(32, 20);
            this.txtRFIssueNo.TabIndex = 55;
            // 
            // lRFIssueNo
            // 
            this.lRFIssueNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lRFIssueNo.Location = new System.Drawing.Point(375, 255);
            this.lRFIssueNo.Name = "lRFIssueNo";
            this.lRFIssueNo.Size = new System.Drawing.Size(56, 13);
            this.lRFIssueNo.TabIndex = 56;
            this.lRFIssueNo.Text = "Issue No";
            this.lRFIssueNo.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // txtBand
            // 
            this.txtBand.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtBand.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.txtBand.Location = new System.Drawing.Point(332, 269);
            this.txtBand.MaxLength = 5;
            this.txtBand.Name = "txtBand";
            this.txtBand.ReadOnly = true;
            this.txtBand.Size = new System.Drawing.Size(32, 20);
            this.txtBand.TabIndex = 60;
            this.txtBand.Visible = false;
            // 
            // lBand
            // 
            this.lBand.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lBand.Location = new System.Drawing.Point(331, 250);
            this.lBand.Name = "lBand";
            this.lBand.Size = new System.Drawing.Size(41, 18);
            this.lBand.TabIndex = 61;
            this.lBand.Text = "Band";
            this.lBand.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.lBand.Visible = false;
            // 
            // gbAddress
            // 
            this.gbAddress.Controls.Add(this.chkSms);
            this.gbAddress.Controls.Add(this.chxCreateHistory);
            this.gbAddress.Controls.Add(this.chxCopyName);
            this.gbAddress.Controls.Add(this.panelStoreCard);
            this.gbAddress.Controls.Add(this.lNewAddress);
            this.gbAddress.Controls.Add(this.btnRemove);
            this.gbAddress.Controls.Add(this.btnAdd);
            this.gbAddress.Controls.Add(this.drpAddressType);
            this.gbAddress.Controls.Add(this.tcAddress);
            this.gbAddress.Location = new System.Drawing.Point(333, 1);
            this.gbAddress.Name = "gbAddress";
            this.gbAddress.Size = new System.Drawing.Size(451, 238);
            this.gbAddress.TabIndex = 3;
            this.gbAddress.TabStop = false;
            this.gbAddress.Text = "Addresses";
            // 
            // chkSms
            // 
            this.chkSms.AutoSize = true;
            this.chkSms.Location = new System.Drawing.Point(7, 213);
            this.chkSms.Name = "chkSms";
            this.chkSms.Size = new System.Drawing.Size(91, 19);
            this.chkSms.TabIndex = 53;
            this.chkSms.Text = "Receive Sms";
            this.chkSms.UseVisualStyleBackColor = true;
            // 
            // chxCreateHistory
            // 
            this.chxCreateHistory.AutoSize = true;
            this.chxCreateHistory.BackColor = System.Drawing.Color.Transparent;
            this.chxCreateHistory.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chxCreateHistory.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chxCreateHistory.Location = new System.Drawing.Point(349, 21);
            this.chxCreateHistory.Name = "chxCreateHistory";
            this.chxCreateHistory.Size = new System.Drawing.Size(101, 19);
            this.chxCreateHistory.TabIndex = 2;
            this.chxCreateHistory.Text = "Create History";
            this.chxCreateHistory.UseVisualStyleBackColor = false;
            this.chxCreateHistory.CheckedChanged += new System.EventHandler(this.chxCreateHistory_CheckedChanged);
            // 
            // chxCopyName
            // 
            this.chxCopyName.Location = new System.Drawing.Point(258, 20);
            this.chxCopyName.Name = "chxCopyName";
            this.chxCopyName.Size = new System.Drawing.Size(89, 24);
            this.chxCopyName.TabIndex = 4;
            this.chxCopyName.Text = "Copy Name";
            this.chxCopyName.UseVisualStyleBackColor = false;
            this.chxCopyName.CheckedChanged += new System.EventHandler(this.chxCopyName_CheckedChanged);
            // 
            // panelStoreCard
            // 
            this.panelStoreCard.BackColor = System.Drawing.Color.Transparent;
            this.panelStoreCard.Location = new System.Drawing.Point(379, 255);
            this.panelStoreCard.Name = "panelStoreCard";
            this.panelStoreCard.Size = new System.Drawing.Size(46, 30);
            this.panelStoreCard.TabIndex = 50;
            // 
            // lNewAddress
            // 
            this.lNewAddress.AutoSize = true;
            this.lNewAddress.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lNewAddress.Location = new System.Drawing.Point(96, 217);
            this.lNewAddress.Name = "lNewAddress";
            this.lNewAddress.Size = new System.Drawing.Size(101, 15);
            this.lNewAddress.TabIndex = 36;
            this.lNewAddress.Text = "Add New Address";
            // 
            // btnRemove
            // 
            this.btnRemove.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.btnRemove.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRemove.Location = new System.Drawing.Point(389, 211);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(56, 23);
            this.btnRemove.TabIndex = 35;
            this.btnRemove.Text = "Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.btnAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAdd.Location = new System.Drawing.Point(328, 212);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(56, 23);
            this.btnAdd.TabIndex = 34;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // drpAddressType
            // 
            this.drpAddressType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpAddressType.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.drpAddressType.ItemHeight = 13;
            this.drpAddressType.Location = new System.Drawing.Point(202, 212);
            this.drpAddressType.Name = "drpAddressType";
            this.drpAddressType.Size = new System.Drawing.Size(121, 21);
            this.drpAddressType.TabIndex = 33;
            // 
            // tcAddress
            // 
            this.tcAddress.AutoScroll = true;
            this.tcAddress.IDEPixelArea = true;
            this.tcAddress.Location = new System.Drawing.Point(6, 19);
            this.tcAddress.Name = "tcAddress";
            this.tcAddress.PositionTop = true;
            this.tcAddress.ShrinkPagesToFit = false;
            this.tcAddress.Size = new System.Drawing.Size(488, 188);
            this.tcAddress.TabIndex = 1;
            this.tcAddress.SelectionChanged += new System.EventHandler(this.tcAddress_SelectedIndexChanged);
            // 
            // tpLoyaltyScheme
            // 
            this.tpLoyaltyScheme.Controls.Add(this.tabControl1);
            this.tpLoyaltyScheme.Location = new System.Drawing.Point(0, 25);
            this.tpLoyaltyScheme.Name = "tpLoyaltyScheme";
            this.tpLoyaltyScheme.Selected = false;
            this.tpLoyaltyScheme.Size = new System.Drawing.Size(841, 293);
            this.tpLoyaltyScheme.TabIndex = 12;
            this.tpLoyaltyScheme.Title = "LoyaltyScheme";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tp_HcMember);
            this.tabControl1.Controls.Add(this.tp_HcHistroy);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(841, 293);
            this.tabControl1.TabIndex = 1;
            // 
            // tp_HcMember
            // 
            this.tp_HcMember.BackColor = System.Drawing.SystemColors.Control;
            this.tp_HcMember.Controls.Add(this.btn_override);
            this.tp_HcMember.Controls.Add(this.LoyaltyVoucher_btn);
            this.tp_HcMember.Controls.Add(this.LoyaltyAcct_btn);
            this.tp_HcMember.Controls.Add(this.LoyaltyStatusAcct_txt);
            this.tp_HcMember.Controls.Add(this.LoyaltyStatusVoucher_txt);
            this.tp_HcMember.Controls.Add(this.label17);
            this.tp_HcMember.Controls.Add(this.LoyaltyJoin_btn);
            this.tp_HcMember.Controls.Add(this.pictureBox2);
            this.tp_HcMember.Controls.Add(this.loyaltymemno_mtb);
            this.tp_HcMember.Controls.Add(this.loyaltyinfo_lbl);
            this.tp_HcMember.Controls.Add(this.Loyalty_save_btn);
            this.tp_HcMember.Controls.Add(this.label18);
            this.tp_HcMember.Controls.Add(this.label19);
            this.tp_HcMember.Controls.Add(this.LoyaltyEnd_dtp);
            this.tp_HcMember.Controls.Add(this.LoyaltyStart_dtp);
            this.tp_HcMember.Controls.Add(this.label20);
            this.tp_HcMember.Controls.Add(this.label21);
            this.tp_HcMember.Controls.Add(this.LoyaltyType_cmb);
            this.tp_HcMember.Controls.Add(this.panel1);
            this.tp_HcMember.Location = new System.Drawing.Point(4, 24);
            this.tp_HcMember.Name = "tp_HcMember";
            this.tp_HcMember.Padding = new System.Windows.Forms.Padding(3);
            this.tp_HcMember.Size = new System.Drawing.Size(833, 265);
            this.tp_HcMember.TabIndex = 0;
            this.tp_HcMember.Text = "Home Club Membership";
            // 
            // btn_override
            // 
            this.btn_override.Location = new System.Drawing.Point(456, 93);
            this.btn_override.Name = "btn_override";
            this.btn_override.Size = new System.Drawing.Size(74, 20);
            this.btn_override.TabIndex = 73;
            this.btn_override.Text = "Override";
            this.btn_override.UseVisualStyleBackColor = true;
            this.btn_override.Click += new System.EventHandler(this.btn_override_Click);
            // 
            // LoyaltyVoucher_btn
            // 
            this.LoyaltyVoucher_btn.Location = new System.Drawing.Point(239, 189);
            this.LoyaltyVoucher_btn.Name = "LoyaltyVoucher_btn";
            this.LoyaltyVoucher_btn.Size = new System.Drawing.Size(200, 23);
            this.LoyaltyVoucher_btn.TabIndex = 71;
            this.LoyaltyVoucher_btn.Text = "Review Status";
            this.LoyaltyVoucher_btn.UseVisualStyleBackColor = true;
            this.LoyaltyVoucher_btn.Click += new System.EventHandler(this.LoyaltyVoucher_btn_Click);
            // 
            // LoyaltyAcct_btn
            // 
            this.LoyaltyAcct_btn.Location = new System.Drawing.Point(32, 189);
            this.LoyaltyAcct_btn.Name = "LoyaltyAcct_btn";
            this.LoyaltyAcct_btn.Size = new System.Drawing.Size(183, 23);
            this.LoyaltyAcct_btn.TabIndex = 70;
            this.LoyaltyAcct_btn.Text = "Cancel Membership";
            this.LoyaltyAcct_btn.UseVisualStyleBackColor = true;
            this.LoyaltyAcct_btn.Click += new System.EventHandler(this.LoyaltyAcct_btn_Click);
            // 
            // LoyaltyStatusAcct_txt
            // 
            this.LoyaltyStatusAcct_txt.Location = new System.Drawing.Point(32, 154);
            this.LoyaltyStatusAcct_txt.Name = "LoyaltyStatusAcct_txt";
            this.LoyaltyStatusAcct_txt.ReadOnly = true;
            this.LoyaltyStatusAcct_txt.Size = new System.Drawing.Size(183, 23);
            this.LoyaltyStatusAcct_txt.TabIndex = 69;
            // 
            // LoyaltyStatusVoucher_txt
            // 
            this.LoyaltyStatusVoucher_txt.Location = new System.Drawing.Point(239, 153);
            this.LoyaltyStatusVoucher_txt.Name = "LoyaltyStatusVoucher_txt";
            this.LoyaltyStatusVoucher_txt.ReadOnly = true;
            this.LoyaltyStatusVoucher_txt.Size = new System.Drawing.Size(200, 23);
            this.LoyaltyStatusVoucher_txt.TabIndex = 68;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(237, 133);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(85, 15);
            this.label17.TabIndex = 67;
            this.label17.Text = "Voucher Status";
            // 
            // LoyaltyJoin_btn
            // 
            this.LoyaltyJoin_btn.Location = new System.Drawing.Point(34, 28);
            this.LoyaltyJoin_btn.Name = "LoyaltyJoin_btn";
            this.LoyaltyJoin_btn.Size = new System.Drawing.Size(148, 23);
            this.LoyaltyJoin_btn.TabIndex = 66;
            this.LoyaltyJoin_btn.Text = "Create New Membership";
            this.LoyaltyJoin_btn.UseVisualStyleBackColor = true;
            this.LoyaltyJoin_btn.Click += new System.EventHandler(this.LoyaltyJoin_btn_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(482, 189);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(255, 62);
            this.pictureBox2.TabIndex = 62;
            this.pictureBox2.TabStop = false;
            // 
            // loyaltymemno_mtb
            // 
            this.loyaltymemno_mtb.CutCopyMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.loyaltymemno_mtb.Enabled = false;
            this.loyaltymemno_mtb.Location = new System.Drawing.Point(236, 30);
            this.loyaltymemno_mtb.Mask = "####-####-####-####";
            this.loyaltymemno_mtb.Name = "loyaltymemno_mtb";
            this.loyaltymemno_mtb.ReadOnly = true;
            this.loyaltymemno_mtb.Size = new System.Drawing.Size(134, 23);
            this.loyaltymemno_mtb.TabIndex = 65;
            this.loyaltymemno_mtb.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.loyaltymemno_mtb.KeyUp += new System.Windows.Forms.KeyEventHandler(this.loyaltymemno_mtb_KeyPress);
            // 
            // loyaltyinfo_lbl
            // 
            this.loyaltyinfo_lbl.AutoSize = true;
            this.loyaltyinfo_lbl.ForeColor = System.Drawing.Color.Red;
            this.loyaltyinfo_lbl.Location = new System.Drawing.Point(491, 162);
            this.loyaltyinfo_lbl.Name = "loyaltyinfo_lbl";
            this.loyaltyinfo_lbl.Size = new System.Drawing.Size(224, 15);
            this.loyaltyinfo_lbl.TabIndex = 64;
            this.loyaltyinfo_lbl.Text = "No membership found for this customer.";
            this.loyaltyinfo_lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.loyaltyinfo_lbl.Visible = false;
            // 
            // Loyalty_save_btn
            // 
            this.Loyalty_save_btn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Loyalty_save_btn.BackgroundImage")));
            this.Loyalty_save_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Loyalty_save_btn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Loyalty_save_btn.Location = new System.Drawing.Point(754, 16);
            this.Loyalty_save_btn.Name = "Loyalty_save_btn";
            this.Loyalty_save_btn.Size = new System.Drawing.Size(24, 24);
            this.Loyalty_save_btn.TabIndex = 63;
            this.Loyalty_save_btn.Click += new System.EventHandler(this.Loyalty_save_btn_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(19, 72);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(128, 15);
            this.label18.TabIndex = 61;
            this.label18.Text = "Membership Start Date";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(234, 67);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(124, 15);
            this.label19.TabIndex = 60;
            this.label19.Text = "Membership End Date";
            // 
            // LoyaltyEnd_dtp
            // 
            this.LoyaltyEnd_dtp.Location = new System.Drawing.Point(236, 93);
            this.LoyaltyEnd_dtp.Name = "LoyaltyEnd_dtp";
            this.LoyaltyEnd_dtp.Size = new System.Drawing.Size(214, 23);
            this.LoyaltyEnd_dtp.TabIndex = 59;
            this.LoyaltyEnd_dtp.ValueChanged += new System.EventHandler(this.LoyaltyEnd_dtp_ValueChanged);
            // 
            // LoyaltyStart_dtp
            // 
            this.LoyaltyStart_dtp.Location = new System.Drawing.Point(21, 93);
            this.LoyaltyStart_dtp.Name = "LoyaltyStart_dtp";
            this.LoyaltyStart_dtp.Size = new System.Drawing.Size(200, 23);
            this.LoyaltyStart_dtp.TabIndex = 58;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(470, 6);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(120, 15);
            this.label20.TabIndex = 57;
            this.label20.Text = "Loyalty Member Type";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(234, 5);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(120, 15);
            this.label21.TabIndex = 56;
            this.label21.Text = "Loyalty Card Number";
            // 
            // LoyaltyType_cmb
            // 
            this.LoyaltyType_cmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LoyaltyType_cmb.FormattingEnabled = true;
            this.LoyaltyType_cmb.Location = new System.Drawing.Point(472, 30);
            this.LoyaltyType_cmb.Name = "LoyaltyType_cmb";
            this.LoyaltyType_cmb.Size = new System.Drawing.Size(183, 23);
            this.LoyaltyType_cmb.TabIndex = 55;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.LoyaltyReason_cmb);
            this.panel1.Controls.Add(this.label22);
            this.panel1.Controls.Add(this.Reason_lbl);
            this.panel1.Location = new System.Drawing.Point(21, 124);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(429, 140);
            this.panel1.TabIndex = 72;
            // 
            // LoyaltyReason_cmb
            // 
            this.LoyaltyReason_cmb.FormattingEnabled = true;
            this.LoyaltyReason_cmb.Location = new System.Drawing.Point(9, 110);
            this.LoyaltyReason_cmb.Name = "LoyaltyReason_cmb";
            this.LoyaltyReason_cmb.Size = new System.Drawing.Size(407, 23);
            this.LoyaltyReason_cmb.TabIndex = 51;
            this.LoyaltyReason_cmb.Visible = false;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(7, 7);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(150, 15);
            this.label22.TabIndex = 9;
            this.label22.Text = "Loyalty Membership Status";
            // 
            // Reason_lbl
            // 
            this.Reason_lbl.AutoSize = true;
            this.Reason_lbl.Location = new System.Drawing.Point(9, 92);
            this.Reason_lbl.Name = "Reason_lbl";
            this.Reason_lbl.Size = new System.Drawing.Size(45, 15);
            this.Reason_lbl.TabIndex = 52;
            this.Reason_lbl.Text = "Reason";
            this.Reason_lbl.Visible = false;
            // 
            // tp_HcHistroy
            // 
            this.tp_HcHistroy.BackColor = System.Drawing.Color.Transparent;
            this.tp_HcHistroy.Controls.Add(this.dgv_HChistory);
            this.tp_HcHistroy.Location = new System.Drawing.Point(4, 24);
            this.tp_HcHistroy.Name = "tp_HcHistroy";
            this.tp_HcHistroy.Padding = new System.Windows.Forms.Padding(3);
            this.tp_HcHistroy.Size = new System.Drawing.Size(833, 265);
            this.tp_HcHistroy.TabIndex = 1;
            this.tp_HcHistroy.Text = "Home Club History";
            this.tp_HcHistroy.UseVisualStyleBackColor = true;
            // 
            // dgv_HChistory
            // 
            this.dgv_HChistory.AllowUserToAddRows = false;
            this.dgv_HChistory.AllowUserToDeleteRows = false;
            this.dgv_HChistory.AllowUserToOrderColumns = true;
            this.dgv_HChistory.AllowUserToResizeRows = false;
            this.dgv_HChistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgv_HChistory.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgv_HChistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_HChistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_HChistory.Location = new System.Drawing.Point(3, 3);
            this.dgv_HChistory.Name = "dgv_HChistory";
            this.dgv_HChistory.ReadOnly = true;
            this.dgv_HChistory.Size = new System.Drawing.Size(827, 259);
            this.dgv_HChistory.TabIndex = 0;
            // 
            // tpEmployment
            // 
            this.tpEmployment.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tpEmployment.Controls.Add(this.gbEmployment);
            this.tpEmployment.Location = new System.Drawing.Point(0, 25);
            this.tpEmployment.Name = "tpEmployment";
            this.tpEmployment.Selected = false;
            this.tpEmployment.Size = new System.Drawing.Size(841, 293);
            this.tpEmployment.TabIndex = 4;
            this.tpEmployment.Title = "Employment Details";
            // 
            // gbEmployment
            // 
            this.gbEmployment.Controls.Add(this.txtOrganisation1);
            this.gbEmployment.Controls.Add(this.txtIndustry1);
            this.gbEmployment.Controls.Add(this.txtJobTitle1);
            this.gbEmployment.Controls.Add(this.lOrganisation1);
            this.gbEmployment.Controls.Add(this.lIndustry1);
            this.gbEmployment.Controls.Add(this.lJobTitle1);
            this.gbEmployment.Controls.Add(this.drpEductation1);
            this.gbEmployment.Controls.Add(this.lEducation1);
            this.gbEmployment.Controls.Add(this.btnSaveEmployment);
            this.gbEmployment.Controls.Add(this.lIncome);
            this.gbEmployment.Controls.Add(this.txtIncome);
            this.gbEmployment.Controls.Add(this.dtPrevEmpStart);
            this.gbEmployment.Controls.Add(this.dtCurrEmpStart);
            this.gbEmployment.Controls.Add(this.txtEmpTelNum);
            this.gbEmployment.Controls.Add(this.lEmpTelNum1);
            this.gbEmployment.Controls.Add(this.lEmpTelCode1);
            this.gbEmployment.Controls.Add(this.txtEmpTelCode);
            this.gbEmployment.Controls.Add(this.drpPayFrequency);
            this.gbEmployment.Controls.Add(this.lPayFrequency1);
            this.gbEmployment.Controls.Add(this.drpOccupation);
            this.gbEmployment.Controls.Add(this.lOccupation1);
            this.gbEmployment.Controls.Add(this.drpEmploymentStat);
            this.gbEmployment.Controls.Add(this.lEmploymentStat1);
            this.gbEmployment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbEmployment.Location = new System.Drawing.Point(0, 0);
            this.gbEmployment.Name = "gbEmployment";
            this.gbEmployment.Size = new System.Drawing.Size(837, 289);
            this.gbEmployment.TabIndex = 0;
            this.gbEmployment.TabStop = false;
            // 
            // txtOrganisation1
            // 
            this.txtOrganisation1.FormattingEnabled = true;
            this.txtOrganisation1.Location = new System.Drawing.Point(577, 242);
            this.txtOrganisation1.Name = "txtOrganisation1";
            this.txtOrganisation1.Size = new System.Drawing.Size(172, 23);
            this.txtOrganisation1.TabIndex = 48;
            this.txtOrganisation1.Tag = "lOrganisation1";
            // 
            // txtIndustry1
            // 
            this.txtIndustry1.FormattingEnabled = true;
            this.txtIndustry1.Location = new System.Drawing.Point(448, 242);
            this.txtIndustry1.Name = "txtIndustry1";
            this.txtIndustry1.Size = new System.Drawing.Size(124, 23);
            this.txtIndustry1.TabIndex = 49;
            this.txtIndustry1.Tag = "lIndustry1";
            // 
            // txtJobTitle1
            // 
            this.txtJobTitle1.FormattingEnabled = true;
            this.txtJobTitle1.Location = new System.Drawing.Point(448, 108);
            this.txtJobTitle1.Name = "txtJobTitle1";
            this.txtJobTitle1.Size = new System.Drawing.Size(186, 23);
            this.txtJobTitle1.TabIndex = 50;
            this.txtJobTitle1.Tag = "lJobTitle1";
            // 
            // lOrganisation1
            // 
            this.lOrganisation1.AutoSize = true;
            this.lOrganisation1.Location = new System.Drawing.Point(574, 226);
            this.lOrganisation1.Name = "lOrganisation1";
            this.lOrganisation1.Size = new System.Drawing.Size(78, 15);
            this.lOrganisation1.TabIndex = 42;
            this.lOrganisation1.Text = "Organisation:";
            // 
            // lIndustry1
            // 
            this.lIndustry1.AutoSize = true;
            this.lIndustry1.Location = new System.Drawing.Point(449, 226);
            this.lIndustry1.Name = "lIndustry1";
            this.lIndustry1.Size = new System.Drawing.Size(53, 15);
            this.lIndustry1.TabIndex = 43;
            this.lIndustry1.Text = "Industry:";
            // 
            // lJobTitle1
            // 
            this.lJobTitle1.AutoSize = true;
            this.lJobTitle1.Location = new System.Drawing.Point(448, 92);
            this.lJobTitle1.Name = "lJobTitle1";
            this.lJobTitle1.Size = new System.Drawing.Size(53, 15);
            this.lJobTitle1.TabIndex = 36;
            this.lJobTitle1.Text = "Job Title:";
            // 
            // drpEductation1
            // 
            this.drpEductation1.FormattingEnabled = true;
            this.drpEductation1.Location = new System.Drawing.Point(448, 45);
            this.drpEductation1.Name = "drpEductation1";
            this.drpEductation1.Size = new System.Drawing.Size(121, 23);
            this.drpEductation1.TabIndex = 39;
            this.drpEductation1.Tag = "lEducation1";
            // 
            // lEducation1
            // 
            this.lEducation1.AutoSize = true;
            this.lEducation1.Location = new System.Drawing.Point(449, 29);
            this.lEducation1.Name = "lEducation1";
            this.lEducation1.Size = new System.Drawing.Size(93, 15);
            this.lEducation1.TabIndex = 38;
            this.lEducation1.Text = "Education Level:";
            // 
            // btnSaveEmployment
            // 
            this.btnSaveEmployment.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSaveEmployment.BackgroundImage")));
            this.btnSaveEmployment.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSaveEmployment.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSaveEmployment.Location = new System.Drawing.Point(754, 16);
            this.btnSaveEmployment.Name = "btnSaveEmployment";
            this.btnSaveEmployment.Size = new System.Drawing.Size(24, 24);
            this.btnSaveEmployment.TabIndex = 37;
            this.btnSaveEmployment.Click += new System.EventHandler(this.btnSaveEmployment_Click);
            // 
            // lIncome
            // 
            this.lIncome.AutoSize = true;
            this.lIncome.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lIncome.Location = new System.Drawing.Point(255, 145);
            this.lIncome.Name = "lIncome";
            this.lIncome.Size = new System.Drawing.Size(45, 13);
            this.lIncome.TabIndex = 22;
            this.lIncome.Text = "Income:";
            // 
            // txtIncome
            // 
            this.txtIncome.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtIncome.Location = new System.Drawing.Point(255, 171);
            this.txtIncome.MaxLength = 13;
            this.txtIncome.Name = "txtIncome";
            this.txtIncome.Size = new System.Drawing.Size(94, 20);
            this.txtIncome.TabIndex = 18;
            this.txtIncome.Tag = "";
            this.txtIncome.Leave += new System.EventHandler(this.txtIncome_Leave);
            this.txtIncome.Validating += new System.ComponentModel.CancelEventHandler(this.txtIncome_Validating);
            // 
            // dtPrevEmpStart
            // 
            this.dtPrevEmpStart.DateFrom = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtPrevEmpStart.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtPrevEmpStart.Label = "Prev. Emp. started:";
            this.dtPrevEmpStart.LinkedBias = false;
            this.dtPrevEmpStart.LinkedComboBox = null;
            this.dtPrevEmpStart.LinkedDatePicker = null;
            this.dtPrevEmpStart.LinkedLabel = null;
            this.dtPrevEmpStart.Location = new System.Drawing.Point(53, 216);
            this.dtPrevEmpStart.Months = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtPrevEmpStart.Name = "dtPrevEmpStart";
            this.dtPrevEmpStart.Size = new System.Drawing.Size(256, 56);
            this.dtPrevEmpStart.TabIndex = 33;
            this.dtPrevEmpStart.Tag = "dtPrevEmpStart1";
            this.dtPrevEmpStart.Value = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtPrevEmpStart.Years = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // dtCurrEmpStart
            // 
            this.dtCurrEmpStart.DateFrom = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtCurrEmpStart.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtCurrEmpStart.Label = "Curr. Emp. started:";
            this.dtCurrEmpStart.LinkedBias = false;
            this.dtCurrEmpStart.LinkedComboBox = null;
            this.dtCurrEmpStart.LinkedDatePicker = this.dtPrevEmpStart;
            this.dtCurrEmpStart.LinkedLabel = null;
            this.dtCurrEmpStart.Location = new System.Drawing.Point(53, 16);
            this.dtCurrEmpStart.Months = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtCurrEmpStart.Name = "dtCurrEmpStart";
            this.dtCurrEmpStart.Size = new System.Drawing.Size(360, 56);
            this.dtCurrEmpStart.TabIndex = 19;
            this.dtCurrEmpStart.Tag = "dtCurrEmpStart1";
            this.dtCurrEmpStart.Value = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtCurrEmpStart.Years = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // txtEmpTelNum
            // 
            this.txtEmpTelNum.Location = new System.Drawing.Point(511, 170);
            this.txtEmpTelNum.MaxLength = 13;
            this.txtEmpTelNum.Name = "txtEmpTelNum";
            this.txtEmpTelNum.Size = new System.Drawing.Size(123, 23);
            this.txtEmpTelNum.TabIndex = 24;
            // 
            // lEmpTelNum1
            // 
            this.lEmpTelNum1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lEmpTelNum1.AutoSize = true;
            this.lEmpTelNum1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lEmpTelNum1.Location = new System.Drawing.Point(560, 145);
            this.lEmpTelNum1.Name = "lEmpTelNum1";
            this.lEmpTelNum1.Size = new System.Drawing.Size(24, 13);
            this.lEmpTelNum1.TabIndex = 10;
            this.lEmpTelNum1.Text = "No:";
            // 
            // lEmpTelCode1
            // 
            this.lEmpTelCode1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lEmpTelCode1.AutoSize = true;
            this.lEmpTelCode1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lEmpTelCode1.Location = new System.Drawing.Point(497, 145);
            this.lEmpTelCode1.Name = "lEmpTelCode1";
            this.lEmpTelCode1.Size = new System.Drawing.Size(52, 13);
            this.lEmpTelCode1.TabIndex = 9;
            this.lEmpTelCode1.Text = "Tel code:";
            // 
            // txtEmpTelCode
            // 
            this.txtEmpTelCode.Location = new System.Drawing.Point(449, 170);
            this.txtEmpTelCode.MaxLength = 8;
            this.txtEmpTelCode.Name = "txtEmpTelCode";
            this.txtEmpTelCode.Size = new System.Drawing.Size(56, 23);
            this.txtEmpTelCode.TabIndex = 23;
            // 
            // drpPayFrequency
            // 
            this.drpPayFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPayFrequency.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpPayFrequency.ItemHeight = 13;
            this.drpPayFrequency.Location = new System.Drawing.Point(61, 169);
            this.drpPayFrequency.Name = "drpPayFrequency";
            this.drpPayFrequency.Size = new System.Drawing.Size(160, 21);
            this.drpPayFrequency.TabIndex = 27;
            this.drpPayFrequency.Tag = "lPayFrequency1";
            this.drpPayFrequency.SelectedIndexChanged += new System.EventHandler(this.drpPayFrequency_SelectedIndexChanged);
            // 
            // lPayFrequency1
            // 
            this.lPayFrequency1.AutoSize = true;
            this.lPayFrequency1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lPayFrequency1.Location = new System.Drawing.Point(61, 145);
            this.lPayFrequency1.Name = "lPayFrequency1";
            this.lPayFrequency1.Size = new System.Drawing.Size(81, 13);
            this.lPayFrequency1.TabIndex = 22;
            this.lPayFrequency1.Text = "Pay Frequency:";
            // 
            // drpOccupation
            // 
            this.drpOccupation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpOccupation.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpOccupation.ItemHeight = 13;
            this.drpOccupation.Location = new System.Drawing.Point(253, 104);
            this.drpOccupation.Name = "drpOccupation";
            this.drpOccupation.Size = new System.Drawing.Size(160, 21);
            this.drpOccupation.TabIndex = 25;
            this.drpOccupation.Tag = "lOccupation1";
            // 
            // lOccupation1
            // 
            this.lOccupation1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lOccupation1.Location = new System.Drawing.Point(253, 80);
            this.lOccupation1.Name = "lOccupation1";
            this.lOccupation1.Size = new System.Drawing.Size(124, 19);
            this.lOccupation1.TabIndex = 21;
            this.lOccupation1.Text = "Occupation:";
            // 
            // drpEmploymentStat
            // 
            this.drpEmploymentStat.DisplayMember = "codedescript";
            this.drpEmploymentStat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmploymentStat.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpEmploymentStat.ItemHeight = 13;
            this.drpEmploymentStat.Location = new System.Drawing.Point(61, 105);
            this.drpEmploymentStat.Name = "drpEmploymentStat";
            this.drpEmploymentStat.Size = new System.Drawing.Size(160, 21);
            this.drpEmploymentStat.TabIndex = 24;
            this.drpEmploymentStat.Tag = "lEmploymentStat1";
            // 
            // lEmploymentStat1
            // 
            this.lEmploymentStat1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lEmploymentStat1.Location = new System.Drawing.Point(61, 81);
            this.lEmploymentStat1.Name = "lEmploymentStat1";
            this.lEmploymentStat1.Size = new System.Drawing.Size(123, 19);
            this.lEmploymentStat1.TabIndex = 20;
            this.lEmploymentStat1.Text = "Employment Status:";
            // 
            // tpBank
            // 
            this.tpBank.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tpBank.Controls.Add(this.gbBank);
            this.tpBank.Location = new System.Drawing.Point(0, 25);
            this.tpBank.Name = "tpBank";
            this.tpBank.Selected = false;
            this.tpBank.Size = new System.Drawing.Size(841, 293);
            this.tpBank.TabIndex = 5;
            this.tpBank.Title = "Bank Details";
            // 
            // gbBank
            // 
            this.gbBank.Controls.Add(this.dtBankOpened);
            this.gbBank.Controls.Add(this.txtBankAcctNumber);
            this.gbBank.Controls.Add(this.drpBankAcctType);
            this.gbBank.Controls.Add(this.lBankAcctNumber1);
            this.gbBank.Controls.Add(this.lBankAcctType1);
            this.gbBank.Controls.Add(this.drpBank);
            this.gbBank.Controls.Add(this.lBank1);
            this.gbBank.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.gbBank.Location = new System.Drawing.Point(6, 6);
            this.gbBank.Name = "gbBank";
            this.gbBank.Size = new System.Drawing.Size(760, 281);
            this.gbBank.TabIndex = 3;
            this.gbBank.TabStop = false;
            // 
            // dtBankOpened
            // 
            this.dtBankOpened.DateFrom = new System.DateTime(2004, 3, 25, 0, 0, 0, 0);
            this.dtBankOpened.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtBankOpened.Label = "Bank acct opened:";
            this.dtBankOpened.LinkedBias = false;
            this.dtBankOpened.LinkedComboBox = null;
            this.dtBankOpened.LinkedDatePicker = null;
            this.dtBankOpened.LinkedLabel = null;
            this.dtBankOpened.Location = new System.Drawing.Point(120, 152);
            this.dtBankOpened.Months = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtBankOpened.Name = "dtBankOpened";
            this.dtBankOpened.Size = new System.Drawing.Size(256, 56);
            this.dtBankOpened.TabIndex = 3;
            this.dtBankOpened.Tag = "dtBankOpened1";
            this.dtBankOpened.Value = new System.DateTime(2004, 3, 25, 0, 0, 0, 0);
            this.dtBankOpened.Years = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // txtBankAcctNumber
            // 
            this.txtBankAcctNumber.Location = new System.Drawing.Point(440, 88);
            this.txtBankAcctNumber.MaxLength = 11;
            this.txtBankAcctNumber.Name = "txtBankAcctNumber";
            this.txtBankAcctNumber.Size = new System.Drawing.Size(131, 20);
            this.txtBankAcctNumber.TabIndex = 2;
            this.txtBankAcctNumber.Tag = "lLoans1";
            // 
            // drpBankAcctType
            // 
            this.drpBankAcctType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBankAcctType.ItemHeight = 14;
            this.drpBankAcctType.Location = new System.Drawing.Point(296, 88);
            this.drpBankAcctType.Name = "drpBankAcctType";
            this.drpBankAcctType.Size = new System.Drawing.Size(93, 22);
            this.drpBankAcctType.TabIndex = 1;
            this.drpBankAcctType.Tag = "lBankAcctType1";
            // 
            // lBankAcctNumber1
            // 
            this.lBankAcctNumber1.Location = new System.Drawing.Point(440, 72);
            this.lBankAcctNumber1.Name = "lBankAcctNumber1";
            this.lBankAcctNumber1.Size = new System.Drawing.Size(127, 18);
            this.lBankAcctNumber1.TabIndex = 0;
            this.lBankAcctNumber1.Text = "Account Number:";
            // 
            // lBankAcctType1
            // 
            this.lBankAcctType1.Location = new System.Drawing.Point(296, 72);
            this.lBankAcctType1.Name = "lBankAcctType1";
            this.lBankAcctType1.Size = new System.Drawing.Size(97, 18);
            this.lBankAcctType1.TabIndex = 0;
            this.lBankAcctType1.Text = "Account Type:";
            // 
            // drpBank
            // 
            this.drpBank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBank.ItemHeight = 14;
            this.drpBank.Location = new System.Drawing.Point(128, 88);
            this.drpBank.Name = "drpBank";
            this.drpBank.Size = new System.Drawing.Size(123, 22);
            this.drpBank.TabIndex = 0;
            this.drpBank.Tag = "lBank1";
            // 
            // lBank1
            // 
            this.lBank1.Location = new System.Drawing.Point(128, 72);
            this.lBank1.Name = "lBank1";
            this.lBank1.Size = new System.Drawing.Size(41, 18);
            this.lBank1.TabIndex = 0;
            this.lBank1.Text = "Bank:";
            // 
            // tpNameHistory
            // 
            this.tpNameHistory.Controls.Add(this.gbNameHistory);
            this.tpNameHistory.Location = new System.Drawing.Point(0, 25);
            this.tpNameHistory.Name = "tpNameHistory";
            this.tpNameHistory.Selected = false;
            this.tpNameHistory.Size = new System.Drawing.Size(841, 293);
            this.tpNameHistory.TabIndex = 6;
            this.tpNameHistory.Title = "Name History";
            // 
            // gbNameHistory
            // 
            this.gbNameHistory.Controls.Add(this.dgNameHistory);
            this.gbNameHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbNameHistory.Location = new System.Drawing.Point(0, 0);
            this.gbNameHistory.Name = "gbNameHistory";
            this.gbNameHistory.Size = new System.Drawing.Size(841, 293);
            this.gbNameHistory.TabIndex = 0;
            this.gbNameHistory.TabStop = false;
            // 
            // dgNameHistory
            // 
            this.dgNameHistory.CaptionText = "Customer Name Audit";
            this.dgNameHistory.DataMember = "";
            this.dgNameHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgNameHistory.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgNameHistory.Location = new System.Drawing.Point(3, 19);
            this.dgNameHistory.Name = "dgNameHistory";
            this.dgNameHistory.ReadOnly = true;
            this.dgNameHistory.Size = new System.Drawing.Size(835, 271);
            this.dgNameHistory.TabIndex = 0;
            // 
            // tpAddressHistory
            // 
            this.tpAddressHistory.Controls.Add(this.gbAddressHistory);
            this.tpAddressHistory.Location = new System.Drawing.Point(0, 25);
            this.tpAddressHistory.Name = "tpAddressHistory";
            this.tpAddressHistory.Selected = false;
            this.tpAddressHistory.Size = new System.Drawing.Size(841, 293);
            this.tpAddressHistory.TabIndex = 7;
            this.tpAddressHistory.Title = "Address History";
            // 
            // gbAddressHistory
            // 
            this.gbAddressHistory.Controls.Add(this.dgAddressHistory);
            this.gbAddressHistory.Controls.Add(this.panel2);
            this.gbAddressHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbAddressHistory.Location = new System.Drawing.Point(0, 0);
            this.gbAddressHistory.Name = "gbAddressHistory";
            this.gbAddressHistory.Size = new System.Drawing.Size(841, 293);
            this.gbAddressHistory.TabIndex = 1;
            this.gbAddressHistory.TabStop = false;
            // 
            // dgAddressHistory
            // 
            this.dgAddressHistory.CaptionText = "Customer Address";
            this.dgAddressHistory.DataMember = "";
            this.dgAddressHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgAddressHistory.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAddressHistory.Location = new System.Drawing.Point(3, 52);
            this.dgAddressHistory.Name = "dgAddressHistory";
            this.dgAddressHistory.ReadOnly = true;
            this.dgAddressHistory.Size = new System.Drawing.Size(835, 238);
            this.dgAddressHistory.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.radAddressAudit);
            this.panel2.Controls.Add(this.radAddress);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 19);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(835, 33);
            this.panel2.TabIndex = 14;
            // 
            // radAddressAudit
            // 
            this.radAddressAudit.AutoSize = true;
            this.radAddressAudit.Location = new System.Drawing.Point(153, 8);
            this.radAddressAudit.Name = "radAddressAudit";
            this.radAddressAudit.Size = new System.Drawing.Size(99, 19);
            this.radAddressAudit.TabIndex = 13;
            this.radAddressAudit.Text = "Address Audit";
            this.radAddressAudit.UseVisualStyleBackColor = true;
            this.radAddressAudit.CheckedChanged += new System.EventHandler(this.radAddressAudit_CheckedChanged);
            // 
            // radAddress
            // 
            this.radAddress.AutoSize = true;
            this.radAddress.Checked = true;
            this.radAddress.Location = new System.Drawing.Point(16, 8);
            this.radAddress.Name = "radAddress";
            this.radAddress.Size = new System.Drawing.Size(108, 19);
            this.radAddress.TabIndex = 12;
            this.radAddress.TabStop = true;
            this.radAddress.Text = "Address History";
            this.radAddress.UseVisualStyleBackColor = true;
            this.radAddress.CheckedChanged += new System.EventHandler(this.radAddress_CheckedChanged);
            // 
            // tpTelHistory
            // 
            this.tpTelHistory.Controls.Add(this.gbTelHistory);
            this.tpTelHistory.Location = new System.Drawing.Point(0, 25);
            this.tpTelHistory.Name = "tpTelHistory";
            this.tpTelHistory.Selected = false;
            this.tpTelHistory.Size = new System.Drawing.Size(841, 293);
            this.tpTelHistory.TabIndex = 8;
            this.tpTelHistory.Title = "Telephone History";
            // 
            // gbTelHistory
            // 
            this.gbTelHistory.Controls.Add(this.dgTelephoneHistory);
            this.gbTelHistory.Controls.Add(this.panel3);
            this.gbTelHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbTelHistory.Location = new System.Drawing.Point(0, 0);
            this.gbTelHistory.Name = "gbTelHistory";
            this.gbTelHistory.Size = new System.Drawing.Size(841, 293);
            this.gbTelHistory.TabIndex = 2;
            this.gbTelHistory.TabStop = false;
            // 
            // dgTelephoneHistory
            // 
            this.dgTelephoneHistory.CaptionText = "Customer Telephone";
            this.dgTelephoneHistory.DataMember = "";
            this.dgTelephoneHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgTelephoneHistory.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgTelephoneHistory.Location = new System.Drawing.Point(3, 52);
            this.dgTelephoneHistory.Name = "dgTelephoneHistory";
            this.dgTelephoneHistory.ReadOnly = true;
            this.dgTelephoneHistory.Size = new System.Drawing.Size(835, 238);
            this.dgTelephoneHistory.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.radTelephoneHistory);
            this.panel3.Controls.Add(this.radTelephoneAudit);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(3, 19);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(835, 33);
            this.panel3.TabIndex = 4;
            // 
            // radTelephoneHistory
            // 
            this.radTelephoneHistory.AutoSize = true;
            this.radTelephoneHistory.Checked = true;
            this.radTelephoneHistory.Location = new System.Drawing.Point(5, 7);
            this.radTelephoneHistory.Name = "radTelephoneHistory";
            this.radTelephoneHistory.Size = new System.Drawing.Size(120, 19);
            this.radTelephoneHistory.TabIndex = 2;
            this.radTelephoneHistory.TabStop = true;
            this.radTelephoneHistory.Text = "Telephone History";
            this.radTelephoneHistory.UseVisualStyleBackColor = true;
            // 
            // radTelephoneAudit
            // 
            this.radTelephoneAudit.AutoSize = true;
            this.radTelephoneAudit.Location = new System.Drawing.Point(133, 7);
            this.radTelephoneAudit.Name = "radTelephoneAudit";
            this.radTelephoneAudit.Size = new System.Drawing.Size(111, 19);
            this.radTelephoneAudit.TabIndex = 3;
            this.radTelephoneAudit.Text = "Telephone Audit";
            this.radTelephoneAudit.UseVisualStyleBackColor = true;
            this.radTelephoneAudit.CheckedChanged += new System.EventHandler(this.radTelephoneAudit_CheckedChanged);
            // 
            // tpResidential
            // 
            this.tpResidential.Controls.Add(this.lTransportType1);
            this.tpResidential.Controls.Add(this.drpTransportType1);
            this.tpResidential.Controls.Add(this.lDistanceFromStore1);
            this.tpResidential.Controls.Add(this.txtDistanceFromStore1);
            this.tpResidential.Controls.Add(this.dtDateInPrevAddress1);
            this.tpResidential.Controls.Add(this.btnSaveResidential);
            this.tpResidential.Controls.Add(this.dtDateInCurrentAddress1);
            this.tpResidential.Controls.Add(this.drpCurrentResidentialStatus1);
            this.tpResidential.Controls.Add(this.lMortgage1);
            this.tpResidential.Controls.Add(this.txtMortgage1);
            this.tpResidential.Controls.Add(this.lPrevResidentialStatus1);
            this.tpResidential.Controls.Add(this.lPropertyType1);
            this.tpResidential.Controls.Add(this.drpPropertyType1);
            this.tpResidential.Controls.Add(this.lCurrentResidentialStatus1);
            this.tpResidential.Controls.Add(this.drpPrevResidentialStatus1);
            this.tpResidential.Location = new System.Drawing.Point(0, 25);
            this.tpResidential.Name = "tpResidential";
            this.tpResidential.Selected = false;
            this.tpResidential.Size = new System.Drawing.Size(841, 293);
            this.tpResidential.TabIndex = 9;
            this.tpResidential.Title = "Residential Details";
            // 
            // lTransportType1
            // 
            this.lTransportType1.AutoSize = true;
            this.lTransportType1.Location = new System.Drawing.Point(589, 117);
            this.lTransportType1.Name = "lTransportType1";
            this.lTransportType1.Size = new System.Drawing.Size(86, 15);
            this.lTransportType1.TabIndex = 52;
            this.lTransportType1.Text = "Transport Type:";
            // 
            // drpTransportType1
            // 
            this.drpTransportType1.FormattingEnabled = true;
            this.drpTransportType1.Location = new System.Drawing.Point(592, 149);
            this.drpTransportType1.Name = "drpTransportType1";
            this.drpTransportType1.Size = new System.Drawing.Size(121, 23);
            this.drpTransportType1.TabIndex = 51;
            this.drpTransportType1.Tag = "lTransportType1";
            // 
            // lDistanceFromStore1
            // 
            this.lDistanceFromStore1.AutoSize = true;
            this.lDistanceFromStore1.Location = new System.Drawing.Point(444, 117);
            this.lDistanceFromStore1.Name = "lDistanceFromStore1";
            this.lDistanceFromStore1.Size = new System.Drawing.Size(116, 15);
            this.lDistanceFromStore1.TabIndex = 50;
            this.lDistanceFromStore1.Text = "Distance From Store:";
            // 
            // txtDistanceFromStore1
            // 
            this.txtDistanceFromStore1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.txtDistanceFromStore1.Location = new System.Drawing.Point(447, 149);
            this.txtDistanceFromStore1.Name = "txtDistanceFromStore1";
            this.txtDistanceFromStore1.Size = new System.Drawing.Size(85, 20);
            this.txtDistanceFromStore1.TabIndex = 49;
            this.txtDistanceFromStore1.Tag = "lDistanceFromStore1";
            // 
            // dtDateInPrevAddress1
            // 
            this.dtDateInPrevAddress1.DateFrom = new System.DateTime(2006, 10, 10, 0, 0, 0, 0);
            this.dtDateInPrevAddress1.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtDateInPrevAddress1.Label = "Date In Prev Address:";
            this.dtDateInPrevAddress1.LinkedBias = true;
            this.dtDateInPrevAddress1.LinkedComboBox = null;
            this.dtDateInPrevAddress1.LinkedDatePicker = null;
            this.dtDateInPrevAddress1.LinkedLabel = null;
            this.dtDateInPrevAddress1.Location = new System.Drawing.Point(108, 189);
            this.dtDateInPrevAddress1.Months = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtDateInPrevAddress1.Name = "dtDateInPrevAddress1";
            this.dtDateInPrevAddress1.Size = new System.Drawing.Size(256, 56);
            this.dtDateInPrevAddress1.TabIndex = 48;
            this.dtDateInPrevAddress1.Tag = "dtDateInCurrentAddress1";
            this.dtDateInPrevAddress1.Value = new System.DateTime(2006, 10, 9, 0, 0, 0, 0);
            this.dtDateInPrevAddress1.Years = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // btnSaveResidential
            // 
            this.btnSaveResidential.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSaveResidential.BackgroundImage")));
            this.btnSaveResidential.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSaveResidential.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSaveResidential.Location = new System.Drawing.Point(675, 208);
            this.btnSaveResidential.Name = "btnSaveResidential";
            this.btnSaveResidential.Size = new System.Drawing.Size(24, 24);
            this.btnSaveResidential.TabIndex = 47;
            this.btnSaveResidential.Click += new System.EventHandler(this.btnSaveResidential_Click);
            // 
            // dtDateInCurrentAddress1
            // 
            this.dtDateInCurrentAddress1.DateFrom = new System.DateTime(2099, 6, 10, 11, 22, 51, 321);
            this.dtDateInCurrentAddress1.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtDateInCurrentAddress1.Label = "Date In Curr Address:";
            this.dtDateInCurrentAddress1.LinkedBias = true;
            this.dtDateInCurrentAddress1.LinkedComboBox = null;
            this.dtDateInCurrentAddress1.LinkedDatePicker = null;
            this.dtDateInCurrentAddress1.LinkedLabel = null;
            this.dtDateInCurrentAddress1.Location = new System.Drawing.Point(108, 46);
            this.dtDateInCurrentAddress1.Months = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtDateInCurrentAddress1.Name = "dtDateInCurrentAddress1";
            this.dtDateInCurrentAddress1.Size = new System.Drawing.Size(256, 56);
            this.dtDateInCurrentAddress1.TabIndex = 42;
            this.dtDateInCurrentAddress1.Tag = "dtDateInCurrentAddress1";
            this.dtDateInCurrentAddress1.Value = new System.DateTime(2099, 6, 10, 11, 22, 51, 321);
            this.dtDateInCurrentAddress1.Years = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtDateInCurrentAddress1.ValueChanged += new System.EventHandler(this.dtDateInCurrentAddress1_ValueChanged);
            // 
            // drpCurrentResidentialStatus1
            // 
            this.drpCurrentResidentialStatus1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCurrentResidentialStatus1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpCurrentResidentialStatus1.ItemHeight = 13;
            this.drpCurrentResidentialStatus1.Location = new System.Drawing.Point(443, 70);
            this.drpCurrentResidentialStatus1.Name = "drpCurrentResidentialStatus1";
            this.drpCurrentResidentialStatus1.Size = new System.Drawing.Size(155, 21);
            this.drpCurrentResidentialStatus1.TabIndex = 43;
            this.drpCurrentResidentialStatus1.Tag = "lCurrentResidentialStatus1";
            // 
            // lMortgage1
            // 
            this.lMortgage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lMortgage1.Location = new System.Drawing.Point(299, 118);
            this.lMortgage1.Name = "lMortgage1";
            this.lMortgage1.Size = new System.Drawing.Size(102, 18);
            this.lMortgage1.TabIndex = 41;
            this.lMortgage1.Text = "Mortgage/Rent:";
            // 
            // txtMortgage1
            // 
            this.txtMortgage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtMortgage1.Location = new System.Drawing.Point(299, 150);
            this.txtMortgage1.Name = "txtMortgage1";
            this.txtMortgage1.Size = new System.Drawing.Size(105, 20);
            this.txtMortgage1.TabIndex = 45;
            this.txtMortgage1.Tag = "lMortgage1";
            // 
            // lPrevResidentialStatus1
            // 
            this.lPrevResidentialStatus1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lPrevResidentialStatus1.Location = new System.Drawing.Point(440, 191);
            this.lPrevResidentialStatus1.Name = "lPrevResidentialStatus1";
            this.lPrevResidentialStatus1.Size = new System.Drawing.Size(171, 18);
            this.lPrevResidentialStatus1.TabIndex = 38;
            this.lPrevResidentialStatus1.Text = "Previous Residential Status:";
            // 
            // lPropertyType1
            // 
            this.lPropertyType1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lPropertyType1.Location = new System.Drawing.Point(115, 118);
            this.lPropertyType1.Name = "lPropertyType1";
            this.lPropertyType1.Size = new System.Drawing.Size(116, 18);
            this.lPropertyType1.TabIndex = 39;
            this.lPropertyType1.Text = "Property Type:";
            // 
            // drpPropertyType1
            // 
            this.drpPropertyType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPropertyType1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpPropertyType1.ItemHeight = 13;
            this.drpPropertyType1.Location = new System.Drawing.Point(115, 150);
            this.drpPropertyType1.Name = "drpPropertyType1";
            this.drpPropertyType1.Size = new System.Drawing.Size(155, 21);
            this.drpPropertyType1.TabIndex = 44;
            this.drpPropertyType1.Tag = "lPropertyType1";
            // 
            // lCurrentResidentialStatus1
            // 
            this.lCurrentResidentialStatus1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lCurrentResidentialStatus1.Location = new System.Drawing.Point(440, 46);
            this.lCurrentResidentialStatus1.Name = "lCurrentResidentialStatus1";
            this.lCurrentResidentialStatus1.Size = new System.Drawing.Size(168, 18);
            this.lCurrentResidentialStatus1.TabIndex = 40;
            this.lCurrentResidentialStatus1.Text = "Current Residential Status:";
            // 
            // drpPrevResidentialStatus1
            // 
            this.drpPrevResidentialStatus1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPrevResidentialStatus1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpPrevResidentialStatus1.ItemHeight = 13;
            this.drpPrevResidentialStatus1.Location = new System.Drawing.Point(443, 212);
            this.drpPrevResidentialStatus1.Name = "drpPrevResidentialStatus1";
            this.drpPrevResidentialStatus1.Size = new System.Drawing.Size(155, 21);
            this.drpPrevResidentialStatus1.TabIndex = 46;
            this.drpPrevResidentialStatus1.Tag = "lPrevResidentialStatus1";
            // 
            // tpFinancial
            // 
            this.tpFinancial.Controls.Add(this.groupBox7);
            this.tpFinancial.Controls.Add(this.grpIncome);
            this.tpFinancial.Controls.Add(this.groupBox8);
            this.tpFinancial.Controls.Add(this.gbBankDetails);
            this.tpFinancial.Location = new System.Drawing.Point(0, 25);
            this.tpFinancial.Name = "tpFinancial";
            this.tpFinancial.Selected = false;
            this.tpFinancial.Size = new System.Drawing.Size(841, 293);
            this.tpFinancial.TabIndex = 10;
            this.tpFinancial.Title = "Financial Details";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.lAdditionalExpenditure2);
            this.groupBox7.Controls.Add(this.lAdditionalExpenditure1);
            this.groupBox7.Controls.Add(this.txtAdditionalExpenditure2);
            this.groupBox7.Controls.Add(this.txtAdditionalExpenditure1);
            this.groupBox7.Controls.Add(this.txtOther1);
            this.groupBox7.Controls.Add(this.lTotal1);
            this.groupBox7.Controls.Add(this.txtTotal1);
            this.groupBox7.Controls.Add(this.lOther1);
            this.groupBox7.Controls.Add(this.lMisc1);
            this.groupBox7.Controls.Add(this.txtMisc1);
            this.groupBox7.Controls.Add(this.lLoans1);
            this.groupBox7.Controls.Add(this.txtLoans1);
            this.groupBox7.Controls.Add(this.lUtilities1);
            this.groupBox7.Controls.Add(this.txtUtilities1);
            this.groupBox7.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.groupBox7.Location = new System.Drawing.Point(124, 3);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(280, 224);
            this.groupBox7.TabIndex = 9;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Monthly Commitments";
            // 
            // lAdditionalExpenditure2
            // 
            this.lAdditionalExpenditure2.Location = new System.Drawing.Point(144, 128);
            this.lAdditionalExpenditure2.Name = "lAdditionalExpenditure2";
            this.lAdditionalExpenditure2.Size = new System.Drawing.Size(95, 19);
            this.lAdditionalExpenditure2.TabIndex = 8;
            this.lAdditionalExpenditure2.Text = "Additional Exp. 2:";
            this.lAdditionalExpenditure2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lAdditionalExpenditure1
            // 
            this.lAdditionalExpenditure1.Location = new System.Drawing.Point(24, 128);
            this.lAdditionalExpenditure1.Name = "lAdditionalExpenditure1";
            this.lAdditionalExpenditure1.Size = new System.Drawing.Size(104, 16);
            this.lAdditionalExpenditure1.TabIndex = 7;
            this.lAdditionalExpenditure1.Text = "Additional Exp 1:";
            this.lAdditionalExpenditure1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAdditionalExpenditure2
            // 
            this.txtAdditionalExpenditure2.Location = new System.Drawing.Point(144, 152);
            this.txtAdditionalExpenditure2.Name = "txtAdditionalExpenditure2";
            this.txtAdditionalExpenditure2.Size = new System.Drawing.Size(96, 20);
            this.txtAdditionalExpenditure2.TabIndex = 5;
            this.txtAdditionalExpenditure2.Tag = "lAdditionalExpenditure2";
            this.txtAdditionalExpenditure2.Validating += new System.ComponentModel.CancelEventHandler(this.txtAdditionalExpenditure2_Validating);
            // 
            // txtAdditionalExpenditure1
            // 
            this.txtAdditionalExpenditure1.Location = new System.Drawing.Point(24, 152);
            this.txtAdditionalExpenditure1.Name = "txtAdditionalExpenditure1";
            this.txtAdditionalExpenditure1.Size = new System.Drawing.Size(96, 20);
            this.txtAdditionalExpenditure1.TabIndex = 2;
            this.txtAdditionalExpenditure1.Tag = "lAdditionalExpenditure1";
            this.txtAdditionalExpenditure1.Validating += new System.ComponentModel.CancelEventHandler(this.txtAdditionalExpenditure1_Validating);
            // 
            // txtOther1
            // 
            this.txtOther1.Location = new System.Drawing.Point(144, 96);
            this.txtOther1.Name = "txtOther1";
            this.txtOther1.Size = new System.Drawing.Size(96, 20);
            this.txtOther1.TabIndex = 4;
            this.txtOther1.Tag = "lOther1";
            this.txtOther1.Validating += new System.ComponentModel.CancelEventHandler(this.txtOther1_Validating);
            // 
            // lTotal1
            // 
            this.lTotal1.Location = new System.Drawing.Point(104, 192);
            this.lTotal1.Name = "lTotal1";
            this.lTotal1.Size = new System.Drawing.Size(38, 19);
            this.lTotal1.TabIndex = 0;
            this.lTotal1.Text = "Total:";
            // 
            // txtTotal1
            // 
            this.txtTotal1.Enabled = false;
            this.txtTotal1.Location = new System.Drawing.Point(144, 192);
            this.txtTotal1.Name = "txtTotal1";
            this.txtTotal1.ReadOnly = true;
            this.txtTotal1.Size = new System.Drawing.Size(96, 20);
            this.txtTotal1.TabIndex = 6;
            this.txtTotal1.Tag = "lTotal1";
            // 
            // lOther1
            // 
            this.lOther1.Location = new System.Drawing.Point(144, 72);
            this.lOther1.Name = "lOther1";
            this.lOther1.Size = new System.Drawing.Size(46, 19);
            this.lOther1.TabIndex = 0;
            this.lOther1.Text = "Other:";
            this.lOther1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lMisc1
            // 
            this.lMisc1.Location = new System.Drawing.Point(144, 16);
            this.lMisc1.Name = "lMisc1";
            this.lMisc1.Size = new System.Drawing.Size(120, 16);
            this.lMisc1.TabIndex = 0;
            this.lMisc1.Text = "Misc. Living Expenses:";
            this.lMisc1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtMisc1
            // 
            this.txtMisc1.Location = new System.Drawing.Point(144, 40);
            this.txtMisc1.Name = "txtMisc1";
            this.txtMisc1.Size = new System.Drawing.Size(96, 20);
            this.txtMisc1.TabIndex = 3;
            this.txtMisc1.Tag = "lMisc1";
            this.txtMisc1.Validating += new System.ComponentModel.CancelEventHandler(this.txtMisc1_Validating);
            // 
            // lLoans1
            // 
            this.lLoans1.Location = new System.Drawing.Point(24, 72);
            this.lLoans1.Name = "lLoans1";
            this.lLoans1.Size = new System.Drawing.Size(112, 16);
            this.lLoans1.TabIndex = 0;
            this.lLoans1.Text = "Loans/Credit Cards:";
            this.lLoans1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtLoans1
            // 
            this.txtLoans1.Location = new System.Drawing.Point(24, 96);
            this.txtLoans1.Name = "txtLoans1";
            this.txtLoans1.Size = new System.Drawing.Size(96, 20);
            this.txtLoans1.TabIndex = 1;
            this.txtLoans1.Tag = "lLoans1";
            this.txtLoans1.Validating += new System.ComponentModel.CancelEventHandler(this.txtLoans1_Validating);
            // 
            // lUtilities1
            // 
            this.lUtilities1.Location = new System.Drawing.Point(24, 16);
            this.lUtilities1.Name = "lUtilities1";
            this.lUtilities1.Size = new System.Drawing.Size(56, 19);
            this.lUtilities1.TabIndex = 0;
            this.lUtilities1.Text = "Utilities:";
            this.lUtilities1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtUtilities1
            // 
            this.txtUtilities1.Location = new System.Drawing.Point(24, 40);
            this.txtUtilities1.Name = "txtUtilities1";
            this.txtUtilities1.Size = new System.Drawing.Size(96, 20);
            this.txtUtilities1.TabIndex = 0;
            this.txtUtilities1.Tag = "lUtilities1";
            this.txtUtilities1.Validating += new System.ComponentModel.CancelEventHandler(this.txtUtilities1_Validating);
            // 
            // grpIncome
            // 
            this.grpIncome.Controls.Add(this.txtDisposable);
            this.grpIncome.Controls.Add(this.lDisposable);
            this.grpIncome.Location = new System.Drawing.Point(12, 227);
            this.grpIncome.Name = "grpIncome";
            this.grpIncome.Size = new System.Drawing.Size(392, 64);
            this.grpIncome.TabIndex = 11;
            this.grpIncome.TabStop = false;
            this.grpIncome.Text = "Monthly Disposable Income";
            // 
            // txtDisposable
            // 
            this.txtDisposable.Enabled = false;
            this.txtDisposable.Location = new System.Drawing.Point(247, 34);
            this.txtDisposable.Name = "txtDisposable";
            this.txtDisposable.ReadOnly = true;
            this.txtDisposable.Size = new System.Drawing.Size(105, 23);
            this.txtDisposable.TabIndex = 5;
            this.txtDisposable.Tag = "lDisposable";
            // 
            // lDisposable
            // 
            this.lDisposable.Location = new System.Drawing.Point(6, 24);
            this.lDisposable.Name = "lDisposable";
            this.lDisposable.Size = new System.Drawing.Size(203, 31);
            this.lDisposable.TabIndex = 0;
            this.lDisposable.Text = "Monthly Income - Monthly Commitments - Monthly Rent/Mortgage";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.lAddIncome1);
            this.groupBox8.Controls.Add(this.txtAddIncome1);
            this.groupBox8.Controls.Add(this.lNetIncome1);
            this.groupBox8.Controls.Add(this.txtNetIncome1);
            this.groupBox8.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.groupBox8.Location = new System.Drawing.Point(12, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(112, 224);
            this.groupBox8.TabIndex = 8;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Monthly Income";
            // 
            // lAddIncome1
            // 
            this.lAddIncome1.Location = new System.Drawing.Point(9, 80);
            this.lAddIncome1.Name = "lAddIncome1";
            this.lAddIncome1.Size = new System.Drawing.Size(75, 29);
            this.lAddIncome1.TabIndex = 0;
            this.lAddIncome1.Text = "Additional Income:";
            // 
            // txtAddIncome1
            // 
            this.txtAddIncome1.Location = new System.Drawing.Point(9, 120);
            this.txtAddIncome1.Name = "txtAddIncome1";
            this.txtAddIncome1.Size = new System.Drawing.Size(94, 20);
            this.txtAddIncome1.TabIndex = 1;
            this.txtAddIncome1.Tag = "lAddIncome1";
            this.txtAddIncome1.Validating += new System.ComponentModel.CancelEventHandler(this.txtAddIncome1_Validating);
            // 
            // lNetIncome1
            // 
            this.lNetIncome1.Location = new System.Drawing.Point(9, 24);
            this.lNetIncome1.Name = "lNetIncome1";
            this.lNetIncome1.Size = new System.Drawing.Size(94, 19);
            this.lNetIncome1.TabIndex = 0;
            this.lNetIncome1.Text = "Net Income:";
            // 
            // txtNetIncome1
            // 
            this.txtNetIncome1.Location = new System.Drawing.Point(9, 48);
            this.txtNetIncome1.Name = "txtNetIncome1";
            this.txtNetIncome1.Size = new System.Drawing.Size(94, 20);
            this.txtNetIncome1.TabIndex = 0;
            this.txtNetIncome1.Tag = "lNetIncome1";
            this.txtNetIncome1.Leave += new System.EventHandler(this.txtNetIncome1_Leave);
            this.txtNetIncome1.Validating += new System.ComponentModel.CancelEventHandler(this.txtNetIncome1_Validating);
            // 
            // gbBankDetails
            // 
            this.gbBankDetails.Controls.Add(this.grpGiro);
            this.gbBankDetails.Controls.Add(this.btnSaveFinancial);
            this.gbBankDetails.Controls.Add(this.dtBankOpened1);
            this.gbBankDetails.Controls.Add(this.txtCreditCardNo1);
            this.gbBankDetails.Controls.Add(this.lCreditCardNo1);
            this.gbBankDetails.Controls.Add(this.txtBankAcctNumber1);
            this.gbBankDetails.Controls.Add(this.drpBankAcctType1);
            this.gbBankDetails.Controls.Add(this.label11);
            this.gbBankDetails.Controls.Add(this.label13);
            this.gbBankDetails.Controls.Add(this.drpBank1);
            this.gbBankDetails.Controls.Add(this.label14);
            this.gbBankDetails.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.gbBankDetails.Location = new System.Drawing.Point(404, 3);
            this.gbBankDetails.Name = "gbBankDetails";
            this.gbBankDetails.Size = new System.Drawing.Size(360, 288);
            this.gbBankDetails.TabIndex = 10;
            this.gbBankDetails.TabStop = false;
            this.gbBankDetails.Text = "Bank Details";
            // 
            // grpGiro
            // 
            this.grpGiro.Controls.Add(this.lPayMethod);
            this.grpGiro.Controls.Add(this.drpPaymentMethod);
            this.grpGiro.Controls.Add(this.drpPayByGiro1);
            this.grpGiro.Controls.Add(this.lBankAccountName1);
            this.grpGiro.Controls.Add(this.lGiroDueDate1);
            this.grpGiro.Controls.Add(this.txtBankAccountName1);
            this.grpGiro.Controls.Add(this.drpGiroDueDate1);
            this.grpGiro.Controls.Add(this.lPayByGiro1);
            this.grpGiro.Location = new System.Drawing.Point(33, 182);
            this.grpGiro.Name = "grpGiro";
            this.grpGiro.Size = new System.Drawing.Size(321, 104);
            this.grpGiro.TabIndex = 37;
            // 
            // lPayMethod
            // 
            this.lPayMethod.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lPayMethod.Location = new System.Drawing.Point(8, 61);
            this.lPayMethod.Name = "lPayMethod";
            this.lPayMethod.Size = new System.Drawing.Size(96, 16);
            this.lPayMethod.TabIndex = 34;
            this.lPayMethod.Text = "Payment Method:";
            // 
            // drpPaymentMethod
            // 
            this.drpPaymentMethod.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.drpPaymentMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPaymentMethod.DropDownWidth = 80;
            this.drpPaymentMethod.ItemHeight = 14;
            this.drpPaymentMethod.Location = new System.Drawing.Point(8, 77);
            this.drpPaymentMethod.Name = "drpPaymentMethod";
            this.drpPaymentMethod.Size = new System.Drawing.Size(160, 22);
            this.drpPaymentMethod.TabIndex = 33;
            this.drpPaymentMethod.Tag = "lPayMethod";
            // 
            // drpPayByGiro1
            // 
            this.drpPayByGiro1.Location = new System.Drawing.Point(8, 29);
            this.drpPayByGiro1.Name = "drpPayByGiro1";
            this.drpPayByGiro1.Size = new System.Drawing.Size(57, 22);
            this.drpPayByGiro1.TabIndex = 30;
            this.drpPayByGiro1.Tag = "lPayByGiro1";
            this.drpPayByGiro1.Visible = false;
            // 
            // lBankAccountName1
            // 
            this.lBankAccountName1.Location = new System.Drawing.Point(159, 5);
            this.lBankAccountName1.Name = "lBankAccountName1";
            this.lBankAccountName1.Size = new System.Drawing.Size(131, 18);
            this.lBankAccountName1.TabIndex = 27;
            this.lBankAccountName1.Text = "Bank Account Name:";
            // 
            // lGiroDueDate1
            // 
            this.lGiroDueDate1.Location = new System.Drawing.Point(8, 5);
            this.lGiroDueDate1.Name = "lGiroDueDate1";
            this.lGiroDueDate1.Size = new System.Drawing.Size(80, 18);
            this.lGiroDueDate1.TabIndex = 28;
            this.lGiroDueDate1.Text = "Giro Due Date:";
            // 
            // txtBankAccountName1
            // 
            this.txtBankAccountName1.Location = new System.Drawing.Point(159, 29);
            this.txtBankAccountName1.MaxLength = 20;
            this.txtBankAccountName1.Name = "txtBankAccountName1";
            this.txtBankAccountName1.Size = new System.Drawing.Size(152, 20);
            this.txtBankAccountName1.TabIndex = 32;
            this.txtBankAccountName1.Tag = "lBankAccountName1";
            // 
            // drpGiroDueDate1
            // 
            this.drpGiroDueDate1.Location = new System.Drawing.Point(7, 29);
            this.drpGiroDueDate1.Name = "drpGiroDueDate1";
            this.drpGiroDueDate1.Size = new System.Drawing.Size(58, 22);
            this.drpGiroDueDate1.TabIndex = 31;
            this.drpGiroDueDate1.Tag = "lGiroDueDate1";
            // 
            // lPayByGiro1
            // 
            this.lPayByGiro1.Location = new System.Drawing.Point(7, 5);
            this.lPayByGiro1.Name = "lPayByGiro1";
            this.lPayByGiro1.Size = new System.Drawing.Size(72, 18);
            this.lPayByGiro1.TabIndex = 29;
            this.lPayByGiro1.Text = "Pay By Giro:";
            this.lPayByGiro1.Visible = false;
            // 
            // btnSaveFinancial
            // 
            this.btnSaveFinancial.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSaveFinancial.BackgroundImage")));
            this.btnSaveFinancial.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSaveFinancial.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSaveFinancial.Location = new System.Drawing.Point(322, 148);
            this.btnSaveFinancial.Name = "btnSaveFinancial";
            this.btnSaveFinancial.Size = new System.Drawing.Size(24, 24);
            this.btnSaveFinancial.TabIndex = 36;
            this.btnSaveFinancial.Click += new System.EventHandler(this.btnSaveFinancial_Click);
            // 
            // dtBankOpened1
            // 
            this.dtBankOpened1.DateFrom = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtBankOpened1.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtBankOpened1.Label = "Bank acct opened:";
            this.dtBankOpened1.LinkedBias = false;
            this.dtBankOpened1.LinkedComboBox = null;
            this.dtBankOpened1.LinkedDatePicker = null;
            this.dtBankOpened1.LinkedLabel = null;
            this.dtBankOpened1.Location = new System.Drawing.Point(32, 128);
            this.dtBankOpened1.Months = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtBankOpened1.Name = "dtBankOpened1";
            this.dtBankOpened1.Size = new System.Drawing.Size(256, 56);
            this.dtBankOpened1.TabIndex = 3;
            this.dtBankOpened1.Tag = "dtBankOpened1";
            this.dtBankOpened1.Value = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtBankOpened1.Years = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // txtCreditCardNo1
            // 
            this.txtCreditCardNo1.Four = "0000";
            this.txtCreditCardNo1.Location = new System.Drawing.Point(208, 96);
            this.txtCreditCardNo1.Name = "txtCreditCardNo1";
            this.txtCreditCardNo1.One = "0000";
            this.txtCreditCardNo1.Size = new System.Drawing.Size(112, 20);
            this.txtCreditCardNo1.TabIndex = 7;
            this.txtCreditCardNo1.Tag = "lCreditCardNo1";
            this.txtCreditCardNo1.Text = "0000-0000-0000-0000";
            this.txtCreditCardNo1.Three = "0000";
            this.txtCreditCardNo1.Two = "0000";
            // 
            // lCreditCardNo1
            // 
            this.lCreditCardNo1.Location = new System.Drawing.Point(208, 72);
            this.lCreditCardNo1.Name = "lCreditCardNo1";
            this.lCreditCardNo1.Size = new System.Drawing.Size(99, 18);
            this.lCreditCardNo1.TabIndex = 0;
            this.lCreditCardNo1.Text = "Credit Card No:";
            // 
            // txtBankAcctNumber1
            // 
            this.txtBankAcctNumber1.Location = new System.Drawing.Point(32, 96);
            this.txtBankAcctNumber1.MaxLength = 11;
            this.txtBankAcctNumber1.Name = "txtBankAcctNumber1";
            this.txtBankAcctNumber1.Size = new System.Drawing.Size(120, 20);
            this.txtBankAcctNumber1.TabIndex = 2;
            this.txtBankAcctNumber1.Tag = "lLoans1";
            // 
            // drpBankAcctType1
            // 
            this.drpBankAcctType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBankAcctType1.ItemHeight = 14;
            this.drpBankAcctType1.Location = new System.Drawing.Point(208, 40);
            this.drpBankAcctType1.Name = "drpBankAcctType1";
            this.drpBankAcctType1.Size = new System.Drawing.Size(112, 22);
            this.drpBankAcctType1.TabIndex = 1;
            this.drpBankAcctType1.Tag = "lBankAcctType1";
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(32, 72);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(96, 18);
            this.label11.TabIndex = 0;
            this.label11.Text = "Account Number:";
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(208, 16);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(97, 18);
            this.label13.TabIndex = 0;
            this.label13.Text = "Account Type:";
            // 
            // drpBank1
            // 
            this.drpBank1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBank1.ItemHeight = 14;
            this.drpBank1.Location = new System.Drawing.Point(32, 40);
            this.drpBank1.Name = "drpBank1";
            this.drpBank1.Size = new System.Drawing.Size(136, 22);
            this.drpBank1.TabIndex = 0;
            this.drpBank1.Tag = "lBank1";
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(32, 16);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(41, 18);
            this.label14.TabIndex = 0;
            this.label14.Text = "Bank:";
            // 
            // tpPhoto
            // 
            this.tpPhoto.Controls.Add(this.btnSaveSignature);
            this.tpPhoto.Controls.Add(this.label16);
            this.tpPhoto.Controls.Add(this.txtSignature);
            this.tpPhoto.Controls.Add(this.dgvPreviousPhotos);
            this.tpPhoto.Controls.Add(this.label15);
            this.tpPhoto.Controls.Add(this.txtFileName);
            this.tpPhoto.Controls.Add(this.btnPrevious);
            this.tpPhoto.Controls.Add(this.btnAddSignature);
            this.tpPhoto.Controls.Add(this.btnAddPicture);
            this.tpPhoto.Controls.Add(this.btnSavePicture);
            this.tpPhoto.Controls.Add(this.btnTakePicture);
            this.tpPhoto.Controls.Add(this.pbSignature);
            this.tpPhoto.Controls.Add(this.pbPhoto);
            this.tpPhoto.Location = new System.Drawing.Point(0, 25);
            this.tpPhoto.Name = "tpPhoto";
            this.tpPhoto.Selected = false;
            this.tpPhoto.Size = new System.Drawing.Size(841, 293);
            this.tpPhoto.TabIndex = 11;
            this.tpPhoto.Title = "Photo/Signature";
            // 
            // btnSaveSignature
            // 
            this.btnSaveSignature.Location = new System.Drawing.Point(622, 190);
            this.btnSaveSignature.Name = "btnSaveSignature";
            this.btnSaveSignature.Size = new System.Drawing.Size(138, 23);
            this.btnSaveSignature.TabIndex = 12;
            this.btnSaveSignature.Text = "Save Signature";
            this.btnSaveSignature.UseVisualStyleBackColor = true;
            this.btnSaveSignature.Click += new System.EventHandler(this.btnSaveSignature_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(5, 271);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(60, 15);
            this.label16.TabIndex = 11;
            this.label16.Text = "File Name";
            // 
            // txtSignature
            // 
            this.txtSignature.Location = new System.Drawing.Point(64, 268);
            this.txtSignature.Name = "txtSignature";
            this.txtSignature.ReadOnly = true;
            this.txtSignature.Size = new System.Drawing.Size(179, 23);
            this.txtSignature.TabIndex = 10;
            // 
            // dgvPreviousPhotos
            // 
            this.dgvPreviousPhotos.AllowUserToAddRows = false;
            this.dgvPreviousPhotos.AllowUserToDeleteRows = false;
            this.dgvPreviousPhotos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPreviousPhotos.Location = new System.Drawing.Point(264, 18);
            this.dgvPreviousPhotos.Name = "dgvPreviousPhotos";
            this.dgvPreviousPhotos.ReadOnly = true;
            this.dgvPreviousPhotos.Size = new System.Drawing.Size(332, 152);
            this.dgvPreviousPhotos.TabIndex = 9;
            this.dgvPreviousPhotos.Visible = false;
            this.dgvPreviousPhotos.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPreviousPhotos_RowEnter);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(5, 213);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(60, 15);
            this.label15.TabIndex = 8;
            this.label15.Text = "File Name";
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(64, 210);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.ReadOnly = true;
            this.txtFileName.Size = new System.Drawing.Size(179, 23);
            this.txtFileName.TabIndex = 7;
            // 
            // btnPrevious
            // 
            this.btnPrevious.Enabled = false;
            this.btnPrevious.Location = new System.Drawing.Point(622, 236);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(138, 23);
            this.btnPrevious.TabIndex = 6;
            this.btnPrevious.Text = "Previous";
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // btnAddSignature
            // 
            this.btnAddSignature.Location = new System.Drawing.Point(622, 147);
            this.btnAddSignature.Name = "btnAddSignature";
            this.btnAddSignature.Size = new System.Drawing.Size(138, 23);
            this.btnAddSignature.TabIndex = 5;
            this.btnAddSignature.Text = "Add Signature From File";
            this.btnAddSignature.UseVisualStyleBackColor = true;
            this.btnAddSignature.Click += new System.EventHandler(this.btnAddSignature_Click);
            // 
            // btnAddPicture
            // 
            this.btnAddPicture.Location = new System.Drawing.Point(622, 104);
            this.btnAddPicture.Name = "btnAddPicture";
            this.btnAddPicture.Size = new System.Drawing.Size(138, 23);
            this.btnAddPicture.TabIndex = 4;
            this.btnAddPicture.Text = "Add Picture From File";
            this.btnAddPicture.UseVisualStyleBackColor = true;
            this.btnAddPicture.Click += new System.EventHandler(this.btnAddPicture_Click);
            // 
            // btnSavePicture
            // 
            this.btnSavePicture.Location = new System.Drawing.Point(622, 61);
            this.btnSavePicture.Name = "btnSavePicture";
            this.btnSavePicture.Size = new System.Drawing.Size(138, 23);
            this.btnSavePicture.TabIndex = 3;
            this.btnSavePicture.Text = "Save Picture";
            this.btnSavePicture.UseVisualStyleBackColor = true;
            this.btnSavePicture.Click += new System.EventHandler(this.btnSavePicture_Click);
            // 
            // btnTakePicture
            // 
            this.btnTakePicture.Location = new System.Drawing.Point(622, 18);
            this.btnTakePicture.Name = "btnTakePicture";
            this.btnTakePicture.Size = new System.Drawing.Size(138, 23);
            this.btnTakePicture.TabIndex = 2;
            this.btnTakePicture.Text = "Take Picture";
            this.btnTakePicture.UseVisualStyleBackColor = true;
            this.btnTakePicture.Click += new System.EventHandler(this.btnTakePicture_Click);
            // 
            // pbSignature
            // 
            this.pbSignature.Location = new System.Drawing.Point(64, 237);
            this.pbSignature.Name = "pbSignature";
            this.pbSignature.Size = new System.Drawing.Size(340, 25);
            this.pbSignature.TabIndex = 1;
            this.pbSignature.TabStop = false;
            // 
            // pbPhoto
            // 
            this.pbPhoto.Location = new System.Drawing.Point(64, 5);
            this.pbPhoto.Name = "pbPhoto";
            this.pbPhoto.Size = new System.Drawing.Size(179, 201);
            this.pbPhoto.TabIndex = 0;
            this.pbPhoto.TabStop = false;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataMember = "";
            this.errorProvider1.Icon = ((System.Drawing.Icon)(resources.GetObject("errorProvider1.Icon")));
            // 
            // errorProvider2
            // 
            this.errorProvider2.ContainerControl = this;
            // 
            // errProRFCreateInterval
            // 
            this.errProRFCreateInterval.ContainerControl = this;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(280, 0);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(54, 19);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "Copy";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(280, 2);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(54, 19);
            this.checkBox2.TabIndex = 3;
            this.checkBox2.Text = "Copy";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // Copy
            // 
            this.Copy.AutoSize = true;
            this.Copy.Location = new System.Drawing.Point(274, 3);
            this.Copy.Name = "Copy";
            this.Copy.Size = new System.Drawing.Size(54, 19);
            this.Copy.TabIndex = 3;
            this.Copy.Text = "Copy";
            this.Copy.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(236, 2);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(91, 19);
            this.checkBox3.TabIndex = 54;
            this.checkBox3.Text = "Receive Sms";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // CopyName
            // 
            this.CopyName.AutoSize = true;
            this.CopyName.Location = new System.Drawing.Point(248, 1);
            this.CopyName.Name = "CopyName";
            this.CopyName.Size = new System.Drawing.Size(76, 19);
            this.CopyName.TabIndex = 3;
            this.CopyName.Text = "chexcopy";
            this.CopyName.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.CopyName.UseVisualStyleBackColor = true;
            // 
            // BasicCustomerDetails
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(841, 477);
            this.Controls.Add(this.grpAccounts);
            this.Controls.Add(this.tcDetails);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BasicCustomerDetails";
            this.Text = "Customer Record";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BasicCustomerDetails_FormClosing);
            this.Load += new System.EventHandler(this.BasicCustomerDetails_Load);
            this.Enter += new System.EventHandler(this.BasicCustomerDetails_Enter);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.pictureStoreCardApproved)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            this.grpAccounts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
            this.tcDetails.ResumeLayout(false);
            this.tpBasicDetails.ResumeLayout(false);
            this.tpBasicDetails.PerformLayout();
            this.gbDetails.ResumeLayout(false);
            this.gbDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDependants)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LoyaltyLogo_pb)).EndInit();
            this.gbAddress.ResumeLayout(false);
            this.gbAddress.PerformLayout();
            this.tpLoyaltyScheme.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tp_HcMember.ResumeLayout(false);
            this.tp_HcMember.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tp_HcHistroy.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_HChistory)).EndInit();
            this.tpEmployment.ResumeLayout(false);
            this.gbEmployment.ResumeLayout(false);
            this.gbEmployment.PerformLayout();
            this.tpBank.ResumeLayout(false);
            this.gbBank.ResumeLayout(false);
            this.gbBank.PerformLayout();
            this.tpNameHistory.ResumeLayout(false);
            this.gbNameHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgNameHistory)).EndInit();
            this.tpAddressHistory.ResumeLayout(false);
            this.gbAddressHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAddressHistory)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tpTelHistory.ResumeLayout(false);
            this.gbTelHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgTelephoneHistory)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tpResidential.ResumeLayout(false);
            this.tpResidential.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDistanceFromStore1)).EndInit();
            this.tpFinancial.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.grpIncome.ResumeLayout(false);
            this.grpIncome.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.gbBankDetails.ResumeLayout(false);
            this.gbBankDetails.PerformLayout();
            this.grpGiro.ResumeLayout(false);
            this.grpGiro.PerformLayout();
            this.tpPhoto.ResumeLayout(false);
            this.tpPhoto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreviousPhotos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSignature)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPhoto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProRFCreateInterval)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }

        //RM 26/08/09 71326 need to raise event when date in changed to update on address tab
        private void dtDateInCurrentAddress1_ValueChanged(object sender, EventArgs e)
        {
            if (basicDetails != null && basicDetails.Tables != null && TN.CustomerAddresses != null && basicDetails.Tables[TN.CustomerAddresses] != null
                && basicDetails.Tables[TN.CustomerAddresses].DefaultView != null)
            {
                DataView dvAddresses = basicDetails.Tables[TN.CustomerAddresses].DefaultView;

                dvAddresses.RowFilter = "AddressType = 'H'";
                if (dvAddresses.Table.Rows.Count > 0)
                {
                    dvAddresses.Table.Rows[0]["Date In"] = dtDateInCurrentAddress1.Value;
                    //make sure current date list is updated from 71522
                    //currentDateList["H"] = dtDateInCurrentAddress1.Value;                         //IP - 24/06/11 - 5.13  UAT23 - #3625 - Moved to save() method as when comparing new datein to the currentdate these were always the same because of this update.
                    if (addressView != null && addressView.Table != null)
                    {
                        foreach (DataRow row in addressView.Table.Rows)
                        {
                            if (row["AddType"].ToString().Trim() == "H")
                            {
                                row["DateIn"] = dtDateInCurrentAddress1.Value;
                                dgAddressHistory.DataSource = addressView;
                            }
                        }
                    }
                }
                foreach (Crownwood.Magic.Controls.TabPage tp in tcAddress.TabPages)
                {
                    foreach (Control c in tp.Controls)
                    {
                        if (tp.Name.Substring(2, tp.Name.Length - 2) == "H")
                        {
                            ((AddressTab)c).dtDateIn.Value = dtDateInCurrentAddress1.Value;
                        }

                    }
                }
            }

            if (onChangeAddressDate != null)
                onChangeAddressDate(this, new GenericEventHandler<DateTime>(dtDateInCurrentAddress1.Value));
        }

        // 67782 RD/DR 19/01/06 Modified to get Associated Accounts if selected
        private void chxIncludeSettled_CheckedChanged(object sender, System.EventArgs e)
        {
            try
            {
                dv.RowFilter = "AccountNumber not = ''";
                if (!chxIncludeSettled.Checked)
                {
                    dv.RowFilter += " and Status not = 'S'";
                }
                if (!chxIncludeAssocAccounts.Checked)
                {
                    dv.RowFilter += " and HldOrJnt = 'H'";
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "chxIncludeSettled_CheckedChanged");
            }
        }

        // 67782 RD/DR 19/011/06 Added to get Associated Accounts if selected
        private void chxIncludeAssocAccounts_CheckedChanged(object sender, System.EventArgs e)
        {
            try
            {
                dv.RowFilter = "AccountNumber not = ''";
                if (!chxIncludeSettled.Checked)
                {
                    dv.RowFilter += " and Status not = 'S'";
                }
                if (!chxIncludeAssocAccounts.Checked)
                {
                    dv.RowFilter += " and HldOrJnt = 'H'";
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "chxIncludeAssocAccounts_CheckedChanged");
            }
        }

        /// <summary>
        /// This function scans the addresses tab control and creates a dataset 
        /// of customer addresses to include in the SaveBasicDetails web service
        /// call
        /// </summary>
        /// <returns></returns>
        private void btnSave_Click(object sender, System.EventArgs e)
        {
            this.newAccounttoCancel = String.Empty; //user saved so no new account going to be cancelled

            Function = "btnSave_Click";
            try
            {
                if (!Save())
                    ShowInfo("M_INVALIDFIELDS");
                else
                {
                    if (SanctionScreen != null && Relationship != "H")
                    {
                        SanctionScreen.LoadAppTwo(Relationship);
                        CloseTab(this);
                    }
                    else
                    {
                        //refresh the accounts list
                        this.loadAccounts(false);
                        //refresh the audit history
                        this.LoadCustomerAudit();
                        //refresh the address history
                        this.LoadAddressHistory();
                    }

                    //IP - 10/02/10 - Sprint 5.10 - #3111
                    if (CustidSelected != null)
                    {
                        CustidSelected(this, new RecordIDEventArgs<StoreCardCustDetails>(new StoreCardCustDetails()
                        {
                            Custid = _custid,
                            Title = drpTitle.Text,
                            FirstName = txtFirstName.Text,
                            LastName = txtLastName.Text
                        }
                        ));
                    }
                }

                //IP - 10/02/10 - Sprint 5.10 - #3111 - commented code and placed above. Only do this if there are no errors when saving.
                //if (CustidSelected != null)
                //{ 
                //    CustidSelected (this, new RecordIDEventArgs<string>(_custid));
                //}

            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of btnSaveClick";
            }
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "btnAdd_Click";
                Wait();

                if (tcAddress.TabPages.Count >= 5)
                {
                    //TO DO add resource entry
                    /* ShowInfo("M_TOOMANYADDRESSES"); */
                    ShowInfo("M_TOOMANYADDRESSES"); //uat(4.3) - 161
                    return;
                }
                else
                {
                    string type = (string)((DataRowView)drpAddressType.SelectedItem)[CN.Code];
                    string desc = (string)((DataRowView)drpAddressType.SelectedItem)[CN.CodeDescription];
                    bool found = false;

                    Crownwood.Magic.Controls.TabPage tp = new Crownwood.Magic.Controls.TabPage(desc);
                    tp.Name = "tp" + type;

                    foreach (Crownwood.Magic.Controls.TabPage t in tcAddress.TabPages)
                    {
                        if (t.Name == tp.Name)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        AddressTab at = new AddressTab(FormRoot, this, type);        //CR1084 jec

                        //currentDateList.Add(type, at.dtDateIn.Value);              //IP - 24/06/11 - 5.13 - UAT23 - #3625
                        addresshistory.Add(type, drpRelationship.SelectedValue.ToString(), at.dtDateIn.Value);
                        if (type == "H")
                        {
                            at.onChangeAddressDate += new OnChangeAddressDate(at_onChangeAddressDate);
                        }

                        at.PopulateZones();
                        at.drpZone.SelectedIndex = at.drpZone.Items.Count - 1;
                        if (type == "M")
                        {
                            at.txtAddress1.Enabled = false;
                            at.cmbVillage.Enabled = false; // Address Standardization CR2019 - 025
                            at.cmbRegion.Enabled = false; // Address Standardization CR2019 - 025
                            at.txtPostCode.Enabled = false;
                            at.drpDeliveryArea.Enabled = false;
                            at.txtEmail.Enabled = false;
                            at.txtExtension.Enabled = false;
                            at.txtAddress1.BackColor = SystemColors.Menu;
                            at.cmbVillage.BackColor = SystemColors.Menu; // Address Standardization CR2019 - 025
                            at.cmbRegion.BackColor = SystemColors.Menu; // Address Standardization CR2019 - 025
                            at.txtPostCode.BackColor = SystemColors.Menu;
                            at.txtEmail.BackColor = SystemColors.Menu;
                            at.txtExtension.BackColor = SystemColors.Menu;

                        }

                        if (type != "H" && type != "D" && type !="D1" && type != "D2" && type != "D3") // Address Standardization CR2019 - 025
                        {
                            at.txtMobile.Enabled = false;
                            at.txtMobile.BackColor = SystemColors.Menu;
                            at.btnMobile.Visible = false;
                        }
                        at.btnMobile.Visible = (type != "H") ? false : true; // Address Standardization CR2019 - 025

                        //IP - 16/03/11 - #3317 - CR1245
                        if (type != "W")
                        {
                            at.btnWork.Visible = false;
                        }
                        if (type == "D" && type == "D1" && type == "D2" && type == "D3") // Address Standardization CR2019 - 025
                        {
                            at.txtMobile.Enabled = true;
                            at.btnMobile.Visible = false;
                        }

                        tp.Controls.Add(at);
                        tp.BorderStyle = BorderStyle.Fixed3D;
                        currentTab = tp;
                        this.tcAddress.TabPages.Add(tp);
                        tcAddress.SelectedTab = tp;
                        //tp.Tag = chxCreateHistory.Checked = true;                         //IP - 24/06/11 - 5.13 - UAT23 - #3625

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
                Function = "End of btnAdd_Click";
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "btnRemove_Click";
                Wait();

                if (tcAddress.TabPages.Count > 0)
                    tcAddress.TabPages.Remove(tcAddress.SelectedTab);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of btnRemove_Click";
            }

        }


        public override bool ConfirmClose()
        {
            bool status = true;
            try
            {
                if (!ReadOnly)
                {
                    if (this._hasdatachanged == true)
                    {
                        if (DialogResult.Yes == ShowInfo("M_SAVECHANGES", MessageBoxButtons.YesNo))
                        {
                            if (!Save())
                            {
                                if (DialogResult.Yes != ShowInfo("M_CANTSAVECUST", MessageBoxButtons.YesNo))
                                    status = false;
                            }
                            else
                            {
                                if (SanctionScreen != null && Relationship != "H")
                                    SanctionScreen.LoadAppTwo(Relationship);
                            }
                        }

                    }
                }
                printScreen = true;
                // Delete the photo in the local directory

                if (path != string.Empty)
                {
                    ReleaseFiles();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "ConfirmClose");
            }
            finally
            {
                Dispose(true);

                //If dispose is called already then say GC to skip finalize on this instance.

                GC.SuppressFinalize(this);
            }
            return status;
        }

        private void ReleaseFiles()
        {
            if (pbPhoto.Image != null)
            {
                pbPhoto.Image.Dispose();
                pbPhoto.Image = null;
            }
            if (pbSignature.Image != null)
            {
                pbSignature.Image.Dispose();
                pbSignature.Image = null;
            }
            try
            {
                File.Delete(Application.StartupPath + "\\" + fileName);//txtFileName.Text.Trim());
            }
            catch
            {
                //Try to delete the file
            }
            try
            {
                File.Delete(Application.StartupPath + "\\" + signatureFileName);//txtSignature.Text.Trim());
            }
            catch
            {
                //Try to delete the file
            }

            //Clean up any temp files that have not been saved
            string path = Application.StartupPath;
            System.IO.DirectoryInfo fi = new System.IO.DirectoryInfo(path);
            System.IO.FileInfo[] f = fi.GetFiles("*.tmp");
            int n = fi.GetFiles("*.tmp").Length;
            for (int i = 0; i < n; i++)
            {
                try
                {
                    f[i].Delete();
                }
                catch
                {
                    //Try to delete the file. Won't delete if currently being used.
                }
            }

            GC.Collect();
        }

        private void menuCustomerCodes_Click(object sender, System.EventArgs e)
        {
            try
            {
                AddCustAcctCodes codes = new AddCustAcctCodes(true, txtCustID.Text, txtFirstName.Text, txtLastName.Text, String.Empty);
                codes.FormRoot = this.FormRoot;
                codes.FormParent = this;
                ((MainForm)this.FormRoot).AddTabPage(codes, 8);
                codes.CustomerCodes(txtCustID.Text);
            }
            catch (Exception ex)
            {
                Catch(ex, "menuCustomerCodes_Click");
            }
        }

        private void drpRelationship_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                Function = "drpRelationship_SelectedIndexChanged";
                Wait();

                if (loaded)
                {
                    //IP - 11/05/10 - UAT(141) UAT5.2.1.0 log - Merged from 4.3
                    CheckAccountRelationship();
                    CheckAge();

                    /* make sure the index has actually changed */
                    if (drpRelationship.SelectedIndex != OldRelationshipIndex)
                    {
                        OldRelationshipIndex = drpRelationship.SelectedIndex;

                        if (!ReadOnly)
                        {
                            Save();
                            txtCustID.SetError(String.Empty);
                            errorProvider1.SetError(txtLastName, String.Empty);
                            errorProvider1.SetError(tcAddress, String.Empty);
                        }

                        Relationship = (string)((DataRowView)drpRelationship.SelectedItem)["code"];

                        //disable accounts and addresses for linked customers
                        //this.grpAccounts.Enabled = (Relationship == "H");       //LW73019 jec 22/02/11 
                        this.menuLinkToAccount.Enabled = (Relationship == "H");

                        //jj - commented out as it prevents new customers being added as related customers
                        //this.gbAddress.Enabled=(Relationship=="H");

                        //Clear all the current details, then call loadDetails for the new 
                        //relationship type.
                        txtCustID.Text = "";
                        txtFirstName.Text = "";
                        txtLastName.Text = "";
                        txtAlias.Text = "";
                        dtDOB.Value = DateTime.Now; //KEF Pre release 3.5.2.0 UAT issue 82 DOB was defaulting to main holder DOB
                        tcAddress.TabPages.Clear();
                        this.errorProvider1.SetIconAlignment(this.tcAddress, System.Windows.Forms.ErrorIconAlignment.TopLeft);
                        if (dv != null)
                        {
                            dv.Table.Clear();
                        }
                        /*	else
                            {
                                dv = new DataView(accounts.Tables["CustSearch"]);
                            }*/

                        if (Relationship == "H")
                            //loadDetails(Holder, String.Empty, Relationship, this.ReadOnly);
                            loadDetails(String.Empty, txtAccountNo.Text.Replace("-", ""), Relationship, this.ReadOnly);     //LW73019 jec 22/02/11
                        else
                            loadDetails(String.Empty, txtAccountNo.Text.Replace("-", ""), Relationship, this.ReadOnly);
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
                Function = "End of drpRelationship_SelectedIndexChanged";

            }
        }

        private void OnReviseAccount(object sender, System.EventArgs e)
        {
            try
            {
                Function = "OnReviseAccount";
                Wait();
                int index = dgAccounts.CurrentRowIndex;

                if (index >= 0)
                {
                    string acctno = (string)dgAccounts[index, 4];
                    this.newAccounttoCancel = String.Empty; //adding items so don't want to cancel
                    this._hasdatachanged = true;
                    int agrmtno = (int)((DataView)dgAccounts.DataSource)[index][CN.AgrmtNo];

                    repo = AccountManager.IsRepossessed(acctno.Replace("-", ""), out Error);
                    if (repo)
                        ShowInfo("M_REPO");

                    if (Convert.ToBoolean(Country[CountryParameterNames.LoyaltyScheme]) && CheckCanJoinLoyalty(acctno))
                    {
                        this.tcDetails.SelectedTab = tpLoyaltyScheme;
                    }
                    else
                    {
                        //IP - UAT995 UAT5.2
                        if (isLoan && !menuReviseCashLoan.Enabled)
                        {
                            ShowInfo("M_REVISECASHLOAN");
                            return;
                        }

                        if ((repo && allowReviseRepo.Enabled) || (!repo))
                        {
                            NewAccount reviseAcct = new NewAccount(acctno.Replace("-", ""), agrmtno, null, false, FormRoot, this);
                            //LW 70788 - NM - CR1005 - Service Charge Calculation - HP accts
                            if (reviseAcct.ScoringBand.Trim() == "" && reviseAcct.AccountType == AT.HP && reviseAcct.ScoreHPbefore == false)
                            {
                                ShowInfo("M_ACCOUNTNOTSCORED");
                            }
                            else
                            {
                                reviseAcct.Text = "Input/Revise Sales Order";
                                reviseAcct.datePropRF = StageToLaunch.DateProp;
                                ((MainForm)this.FormRoot).AddTabPage(reviseAcct, 6);
                                reviseAcct.SupressEvents = false;
                                if (!reviseAcct.AccountLocked)
                                {
                                    reviseAcct.Confirm = false;
                                    reviseAcct.CloseTab();
                                }
                            }
                        }
                    }
                }
                else
                {
                    ShowInfo("M_NOACCOUNTSELECTED");
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of OnReviseAccount";
            }

        }
        private void storecard_click(object sender, System.EventArgs e)
        {
            try
            {


                Function = "storecard_click";
                Wait();
                int index = dgAccounts.CurrentRowIndex;

                if (index >= 0)
                {
                    var auth = new AuthoriseCheck("MainForm", "menuViewStoreCard", "StoreCard viewing is not permitted. Please authorise with supervisor or cancel.");
                    if (!auth.CheckForRolePermission(Credential.User))
                        auth.ShowDialog();

                    if (auth.IsAuthorised)
                    {
                        string acctno = (string)dgAccounts[index, 4];
                        customer.custid = txtCustID.Text;
                        customer.firstname = txtFirstName.Text;
                        customer.name = txtLastName.Text;
                        customer.title = ((DataRowView)drpTitle.SelectedItem)[0].ToString();
                        StoreCard.StoreCardAccount storecardaccount = new StoreCard.StoreCardAccount(acctno, FormRoot, this);
                        ((MainForm)this.FormRoot).AddTabPage(storecardaccount);
                    }
                }
                else
                {
                    ShowInfo("M_NOACCOUNTSELECTED");
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of storecard_click";
            }
        }
        private void menuSanctionAccount_Click(object sender, System.EventArgs e)
        {
            try
            {
                bool launch = true;
                int index = dgAccounts.CurrentRowIndex;
                if (index < 0)
                {
                    ShowInfo("M_CUSTNOACCOUNTS");
                    launch = false;
                }

                if (launch)
                    LaunchSanctioning();
            }
            catch (Exception ex)
            {
                Catch(ex, "menuSanctionAccount_Click");
            }
        }


        private void tcAddress_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (tcAddress.TabPages.Count > 0)
            {
                currentTab.Tag = chxCreateHistory.Checked;
                chxCreateHistory.Checked = (bool)tcAddress.SelectedTab.Tag;
                currentTab = tcAddress.SelectedTab;

                if (tcAddress.SelectedTab.Controls.Count > 0)
                {
                    chxCopyName.Checked = Convert.ToBoolean(((STL.PL.AddressTab)tcAddress.SelectedTab.Controls[0]).chkSelected.Checked);
                }
            }
        }

        private void chxCreateHistory_CheckedChanged(object sender, System.EventArgs e)//KD
        {
            if (tcAddress.TabPages.Count > 0)
                tcAddress.SelectedTab.Tag = chxCreateHistory.Checked;
        }
        public void chxCopyName_CheckedChanged(object sender, System.EventArgs e) //KD
        {
            AddressTab at = new AddressTab(FormRoot);

            foreach (Crownwood.Magic.Controls.TabPage tp in tcAddress.TabPages)
            {
                string addtype = tp.Name.Substring(2, tp.Name.Length - 2);
                if (tp.Selected)
                {
                    if (chxCopyName.Checked)
                    {
                        if (chxCopyName.Checked && addtype != "W")
                        {
                            foreach (Control c in tp.Controls)
                            {
                                ((AddressTab)c).drptitleC.SelectedIndex = drpTitle.SelectedIndex;
                                ((AddressTab)c).CFirstname.Text = txtFirstName.Text;
                                ((AddressTab)c).CLastname.Text = txtLastName.Text;
                                ((AddressTab)c).chkSelected.Checked = chxCopyName.Checked;
                            }
                        }
                    }
                    else
                    {

                        foreach (Control c in tp.Controls)
                        {
                            if ((txtFirstName.Text.Trim().Equals(((AddressTab)c).CFirstname.Text.Trim())) && ((txtLastName.Text.Trim().Equals(((AddressTab)c).CLastname.Text.Trim()))))
                            {
                                chxCopyName.Checked = true;
                            }
                            else
                            {
                                ((AddressTab)c).chkSelected.Checked = chxCopyName.Checked;
                            }
                        }
                    }
                }
            }

        }


        private void dgAccounts_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                bool canAddTo = false;
                short delivered = 0;
                short reversible = 0;

                Function = "dgAccounts_MouseUp";
                int index = dgAccounts.CurrentRowIndex;

                //LW73019 jec 22/02/11 - set relationship dropdown to hldorjnt of selected account
                dgAccounts.Select(dgAccounts.CurrentCell.RowNumber);
                DataView accounts = (DataView)dgAccounts.DataSource;

                Relationship = (string)accounts[index]["HldOrJnt"];         //LW73019 jec 23/02/11

                int drpindex = 0;
                foreach (DataRow row in ((DataTable)drpRelationship.DataSource).Rows)
                {
                    if (accounts.Count == 0)      // required as dv.tables is cleared which clears dgAccounts.DataSource
                    {
                        accounts = (DataView)dgAccounts.DataSource;
                    }
                    if ((string)row["code"] == (string)accounts[index]["HldOrJnt"])
                    {
                        string s = (string)accounts[index]["AccountNumber"];
                        txtAccountNo.Text = s;
                        this.AccountNo = s;
                        OldRelationshipIndex = drpRelationship.SelectedIndex = drpindex;
                    }
                    drpindex++;
                }

                if ((string)((DataRowView)drpRelationship.SelectedItem)["code"] == "H")
                {
                    if (index >= 0)
                    {
                        dgAccounts.Select(dgAccounts.CurrentCell.RowNumber);
                        //DataView accounts = (DataView)dgAccounts.DataSource;      //LW73019 jec 22/02/11
                        accounts = (DataView)dgAccounts.DataSource;                 //LW73019 jec 22/02/11

                        string accType = (string)accounts[index]["Type"];
                        string acctno = (string)accounts[index]["AccountNumber"];
                        decimal cashPrice = Convert.ToDecimal(accounts[index][CN.CashPrice]);
                        string status = (string)accounts[index]["Status"];
                        short addTo = Convert.ToInt16(accounts[index][CN.AddTo]);
                        reversible = Convert.ToInt16(accounts[index][CN.Reversible]);

                        decimal agreementTotal = Convert.ToDecimal(accounts[index]["Agreement Total"]); // no space in string [CN.AgreementTotal]);

                        this.AccountType = accType;

                        this.CashPrice = cashPrice;
                        this.CurrentStatus = status;
                        txtAccountNo.Text = acctno;
                        if (AccountNo != acctno)
                        {
                            this.AccountNo = acctno;
                            
                        }
                        this.AccountNoChanged();
                        if (!(accType == "HCC" || acctno == LoyaltyAcct))
                        {

                            if (e.Button == MouseButtons.Right)
                            {
                                if (status == "S" &&
                                    ((propResult == "A" || propResult == "D") ||
                                    AT.IsCashType(accType)))
                                {
                                    if (StageToLaunch.CheckType == "")
                                    {
                                        StageToLaunch.CheckType = "S1";
                                        StageToLaunch.ScreenMode = SM.View;
                                        StageToLaunch.Stage = GetResource("P_VIEWPROP");
                                        settled = true;
                                    }
                                    else
                                    {
                                        StageToLaunch.ScreenMode = SM.View;
                                        StageToLaunch.Stage = GetResource("P_VIEWPROP");
                                        settled = true;

                                    }
                                }
                                else
                                    settled = false;

                                #region AddTo processing
                                /* check if this is a valid candidate for an addto 
                             * store account numbers that can be added to this 
                             * account in a specialised collection class */
                                //if(accType != AT.Cash && accType != AT.Special && accType != AT.ReadyFinance && accType != AT.GoodsOnLoan) //Acct Type Translation DSR 29/9/03
                                if (accType != AT.Cash && accType != AT.Special && accType != AT.GoodsOnLoan)
                                {
                                    if (!settled)
                                    {
                                        if (addTo == -1)        /* not set - retrieve from db */
                                        {
                                            AccountManager.HasAddToOrDelivery(AccountNo, out addTo, out Error);
                                            if (Error.Length > 0)
                                                ShowError(Error);
                                            else
                                                accounts[index][CN.AddTo] = addTo;
                                        }
                                        if (addTo == 0)     /* account has not been added to */
                                        {
                                            if (addToAccounts == null)
                                                addToAccounts = new AddToCollection();
                                            else
                                                addToAccounts.Clear();

                                            foreach (DataRowView r in accounts)
                                            {
                                                //if(((string)r[CN.Type]!=AT.Special && (string)r[CN.Type]!=AT.ReadyFinance) &&  //Acct Type Translation DSR 29/9/03
                                                if ((string)r[CN.Type] != AT.Special &&
                                                    (string)r[CN.Status] != "S" &&
                                                    (string)r[CN.AccountNumber2] != AccountNo)
                                                {
                                                    delivered = Convert.ToInt16(r[CN.DeliveredIndicator]);
                                                    if (delivered == -1)
                                                    {
                                                        AccountManager.AccountFullyDelivered((string)r[CN.AccountNumber2], out delivered, out Error);
                                                        if (Error.Length > 0)
                                                            ShowError(Error);
                                                        else
                                                            r[CN.DeliveredIndicator] = delivered;
                                                    }
                                                    if (delivered == 1)
                                                    {
                                                        addToAccounts.Add(new AddToAccount((string)r[CN.AccountNumber2],
                                                            (string)r[CN.Type],
                                                            (decimal)r[CN.AgreementTotal2],
                                                            (decimal)r[CN.OutstandingBalance2],
                                                            (decimal)r[CN.CashPrice]));
                                                    }
                                                }
                                            }
                                            canAddTo = addToAccounts.Count >= 1 ? true : false;
                                        }
                                    }
                                }

                                //if((accType!=AT.Special && accType!=AT.ReadyFinance)  //Acct Type Translation DSR 29/9/03
                                if (accType != AT.Special && settled)
                                {
                                    if (reversible == -1)       /* not set - retrieve from db */
                                    {
                                        AccountManager.SettledByAddTo(AccountNo, out reversible, out Error);
                                        if (Error.Length > 0)
                                            ShowError(Error);
                                        else
                                            accounts[index][CN.Reversible] = reversible;
                                    }
                                }
                                #endregion End of AddTo processing

                                DataGrid ctl = (DataGrid)sender;

                                if (StageToLaunch.Stage == null)
                                    txtAccountNo_TextChanged(this, null);

                                MenuCommand sanction = new MenuCommand(StageToLaunch.Stage);
                                MenuCommand revise = new MenuCommand(GetResource("P_ENTERLINEITEMS"));
                                MenuCommand print = new MenuCommand(GetResource("P_PRINTPCARD"));
                                MenuCommand addto = new MenuCommand(GetResource("P_ADDTOACCOUNT"));
                                MenuCommand reverse = new MenuCommand(GetResource("P_REVERSEADDTO"));
                                MenuCommand storeCard = new MenuCommand(GetResource("P_STORE_CARD"));
                                // CR906 rdb 12/09/07
                                //MenuCommand reviseCashLoan = new MenuCommand("Revise Cash Loan");

                                revise.ImageList = menuIcons;
                                revise.ImageIndex = 3;

                                storeCard.Click += new System.EventHandler(this.storecard_click);
                                sanction.Click += new System.EventHandler(this.menuSanctionAccount_Click);

                                revise.Click += new System.EventHandler(this.OnReviseAccount);

                                print.Click += new System.EventHandler(this.OnPrintPaymentCard);
                                addto.Click += new System.EventHandler(this.OnAddTo);
                                reverse.Click += new System.EventHandler(this.OnReverseAddTo);
                                // CR906 rdb 12/09/07 can we send event straight to OnReviseAccount? try it
                                //reviseCashLoan.Click += new EventHandler(OnReviseAccount);            // #8489 remove ability to revise cash loan accounts

                                // old accounts do not have an accType and cause errors when sanction stage is opened
                                // so prevent this menu item being visible
                                if (AT.IsCashType(accType))
                                    sanction.Enabled = false;

                                if (String.IsNullOrEmpty(accType) || (StageToLaunch.CheckType == "R" && !menuRefer.Enabled))
                                    sanction.Visible = false;

                                // cr906  is this a loan account? 
                                // if so show reviseCashLoan instead of loan because cash loans use a different permission setting
                                string err = string.Empty;
                                DataSet ds = AccountManager.GetAccountDetails(acctno, out err);
                                //bool isLoan = Convert.ToBoolean(ds.Tables["AccountDetails"].Rows[0]["IsLoan"]);
                                //isLoan = Convert.ToBoolean(ds.Tables["AccountDetails"].Rows[0]["IsLoan"]); //IP - UAT955 UAT5.2 -- AA UAT 27
                                isLoan = Convert.ToString(ds.Tables["AccountDetails"].Rows[0]["IsLoan"]) == "" ? false : Convert.ToBoolean(ds.Tables["AccountDetails"].Rows[0]["IsLoan"]); //IP - 09/10/12 - #11420//IP - UAT955 UAT5.2 -- AA UAT 27

                                revise.Visible = !isLoan;
                                //reviseCashLoan.Visible = isLoan;      // #8489 remove ability to revise cash loan accounts

                                if (!isLoan)
                                {
                                    DataSet ns = AccountManager.IsAccountValidForOnlyNonStockSale(txtAccountNo.UnformattedText, out Error);
                                    bool isNonStock = Convert.ToBoolean(ns.Tables["AccountDetails"].Rows[0]["Isdel"]);
                                    revise.Visible = !isNonStock;
                                }
                                PopupMenu popup = new PopupMenu();

                                //popup.MenuCommands.AddRange(new MenuCommand[] { sanction, revise, addto, reverse, reviseCashLoan });
                                popup.MenuCommands.AddRange(new MenuCommand[] { sanction, revise, addto, reverse });     // #8489 remove ability to revise cash loan accounts

                                if (Config.ThermalPrintingEnabled)
                                {
                                    MenuCommand menuCommandStatement = new MenuCommand("Statement");
                                    menuCommandStatement.Visible = menuCommandStatement.Enabled = true;
                                    menuCommandStatement.Click +=
                                        delegate (object ssender, EventArgs ee)
                                        {
                                            ((MainForm)this.FormRoot).OpenStatementsTab(this.AccountNo, this.CustomerID);
                                        };
                                    popup.MenuCommands.Add(menuCommandStatement);
                                }

                                if (propResult == "A" || propResult == "D" || propResult == "R")
                                {
                                    if (accType != AT.StoreCard)
                                        popup.MenuCommands.AddRange(new MenuCommand[] { print });

                                }


                                //CR903 Line items cannot be added to a HP account until it has been sanctioned provided that ScoreHPbefore has been enabled
                                bool ScoreHPbefore = ReturnScoreHPResult();
                                //Store cards will only have access to store card table...
                                if (accType == AT.StoreCard) //&& status != "S") allow settled accounts to be viewed
                                {
                                    popup.MenuCommands.Clear();
                                    popup.MenuCommands.AddRange(new MenuCommand[] { storeCard });




                                }

                                if (AccountType == AT.ReadyFinance || (AccountType == AT.HP && ScoreHPbefore == true))
                                {
                                    switch (status)
                                    {
                                        case "0":
                                            if (!stage1Complete)
                                            {
                                                if (popup.MenuCommands.Contains(revise))
                                                    popup.MenuCommands.Remove(revise);
                                                if (popup.MenuCommands.Contains(addto))
                                                    popup.MenuCommands.Remove(addto);
                                            }
                                            break;
                                        case "S":
                                            if (!stage1Complete)
                                            {
                                                if (popup.MenuCommands.Contains(revise))
                                                    popup.MenuCommands.Remove(revise);
                                                if (popup.MenuCommands.Contains(addto))
                                                    popup.MenuCommands.Remove(addto);
                                                if (popup.MenuCommands.Contains(print))
                                                    popup.MenuCommands.Remove(print);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                if (AT.IsCashType(AccountType))
                                {
                                    if (popup.MenuCommands.Contains(sanction))
                                        popup.MenuCommands.Remove(sanction);
                                    if (popup.MenuCommands.Contains(addto))
                                        popup.MenuCommands.Remove(addto);
                                    if (popup.MenuCommands.Contains(print))
                                        popup.MenuCommands.Remove(print);
                                    if (!allowReviseCash.Enabled && popup.MenuCommands.Contains(revise))
                                        popup.MenuCommands.Remove(revise);
                                }

                                if (status == "S" && !allowReviseSettled.Enabled)
                                {
                                    if (popup.MenuCommands.Contains(revise))
                                        popup.MenuCommands.Remove(revise);
                                }

                                if (!canAddTo || !controlAddTo.Enabled)
                                {
                                    if (popup.MenuCommands.Contains(addto))
                                        popup.MenuCommands.Remove(addto);
                                }

                                if (reversible != 1 || !controlReverseAddTo.Enabled)
                                {
                                    if (popup.MenuCommands.Contains(reverse))
                                        popup.MenuCommands.Remove(reverse);
                                }

                                //IP - 13/03/09 - (70849)
                                //If the user does not have the user right to revise accounts, and if the
                                //account has not yet been dae'd then still allow them to revise the account.
                                string holdProp = Convert.ToString(accounts[index][CN.HoldProp]);
                                if (!menuRevise.Enabled)
                                {
                                    if (holdProp != "Y")
                                    {
                                        if (popup.MenuCommands.Contains(revise))
                                            popup.MenuCommands.Remove(revise);
                                    }
                                }

                                // cr906 revision of Cash Loan accounts is permission based
                                //if (isLoan && !menuReviseCashLoan.Enabled && (agreementTotal > 0))        // #8489 remove ability to revise cash loan accounts
                                //{
                                //    if (popup.MenuCommands.Contains(reviseCashLoan))
                                //        popup.MenuCommands.Remove(reviseCashLoan);
                                //}

                                if (isLoan)                         // #8489 remove ability to revise cash loan accounts - including Addto
                                {
                                    //if (popup.MenuCommands.Contains(reviseCashLoan))
                                    //    popup.MenuCommands.Remove(reviseCashLoan);
                                    if (popup.MenuCommands.Contains(addto))
                                        popup.MenuCommands.Remove(addto);
                                }

                                bool sanctionPermitted = true;
                                switch (StageToLaunch.CheckType)
                                {
                                    case SS.S1:
                                        sanctionPermitted = menuS1.Enabled;
                                        break;
                                    case SS.S2:
                                        sanctionPermitted = menuS2.Enabled;
                                        break;
                                    case SS.DC:
                                        sanctionPermitted = menuDC.Enabled;
                                        break;
                                    case SS.R:
                                        sanctionPermitted = menuUW.Enabled;
                                        break;
                                    default:
                                        break;
                                }

                                if (!sanctionPermitted)
                                {
                                    if (popup.MenuCommands.Contains(sanction))
                                        popup.MenuCommands.Remove(sanction);
                                }

                                if (AccountType == AT.Special)
                                {
                                    if (popup.MenuCommands.Contains(revise))
                                        popup.MenuCommands.Remove(revise);
                                }

                                MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                            }
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
                Function = "End of dgAccounts_MouseUp";
            }
        }

        private void OnAddTo(object sender, System.EventArgs e)
        {
            Function = "OnAddTo";
            try
            {
                AddTo addTo = new AddTo(AccountNo, addToAccounts, FormRoot, this, SType);
                addTo.ShowDialog();
                addTo.Dispose();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void OnReverseAddTo(object sender, System.EventArgs e)
        {
            try
            {
                AccountManager.ReverseAddTo(AccountNo,
                    AccountType,
                    Config.CountryCode,
                    out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                    txtCustID_Leave(null, null);

                foreach (Crownwood.Magic.Controls.TabPage tp in ((MainForm)FormRoot).MainTabControl.TabPages)
                {
                    if (tp.Control is NewAccount)
                    {
                        NewAccount acct = (NewAccount)tp.Control;

                        if (acct.txtAccountNumber.UnformattedText == AccountNo)
                        {
                            acct.SupressEvents = true;
                            acct.loadAccountData(AccountNo, false);
                            acct.SupressEvents = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "OnReverseAddTo");
            }
            finally
            {
                StopWait();
            }
        }


        private void CreateManualAccount(string accountType)
        {
            bool rescore = false;
            ManualAccountEntry manualAcct = new ManualAccountEntry(accountType);
            manualAcct.ShowDialog();

            if (manualAcct.valid)
            {
                string acctNo = manualAcct.txtAccountNo.Text.Replace("-", String.Empty);
                this.newAccounttoCancel = acctNo;
                this._hasdatachanged = true;
                if (Save() || this._hasdatachanged == false)
                {
                    AccountManager.CreateManualAccount(
                        Config.CountryCode,
                        Convert.ToInt16(Config.BranchCode),
                        this.txtCustID.Text,
                        acctNo,
                        accountType,
                   false,
                        out rescore,
                        out Error);

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        this.OpenNewAccount(acctNo, accountType);
                    }
                }
            }
            manualAcct.Dispose();
        }


        private void CreateCustomerAccount(string accountType)
        {
            short branchNo = Convert.ToInt16(drpBranch.Text);
            bool rescore = false;
            if (Save() || this._hasdatachanged == false)
            {
                string acctNo = AccountManager.CreateCustomerAccount(
                    Config.CountryCode,
                    branchNo,
                    this.txtCustID.Text,
                    accountType,
                    Credential.UserId,
                false,
                    out rescore,
                    out Error);
                this.newAccounttoCancel = acctNo;
                this._hasdatachanged = true;
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    //IP - 21/12/10 - Store Card
                    if (accountType == AT.StoreCard)
                    {
                        existingStoreCard = true;
                        storeCard.AcctNo = acctNo;
                        SetStoreCardFields(existingStoreCard);
                        StorecardButtonCheckEnabled();
                    }

                    this.OpenNewAccount(acctNo, accountType);
                }
            }
        }

        /// <summary>
        /// Displays new account details in dgAccounts and opens New Account screen unless an HP account has ScoreHPbefore enabled
        /// </summary>
        /// <param name="acctNo"></param>
        /// <param name="acctType"></param>
        private void OpenNewAccount(string acctNo, string acctType)
        {
            txtAccountNo.Text = acctNo;         // set main acctno to grid row selected (uat 5.0 iss 112 jec 26/03/07)
            acctNo = acctNo.Replace("-", "");
            if (acctNo.Trim().Length != 0)
            {
                this.btnRefresh_Click(null, null);
                // Find the new account in the list
                DataRowView curRow = null;
                DataRowView newRow = null;
                int i = -1;
                while (++i < ((DataView)dgAccounts.DataSource).Count)
                {
                    curRow = ((DataView)dgAccounts.DataSource)[i];
                    if (((string)curRow[CN.AccountNumber2]).Replace("-", "") == acctNo)
                    {
                        dgAccounts.Select(i);
                        newRow = curRow;
                    }
                    else
                        dgAccounts.UnSelect(i);
                }

                //CR903 Prevent New Account screen opening if HP accounts are to be santioned before items are added.
                bool ScoreHPbefore = ReturnScoreHPResult();

                if ((acctType != AT.HP || (acctType == AT.HP && ScoreHPbefore == false)) && acctType != AT.StoreCard)
                {
                    if (newRow != null)
                    {
                        NewAccount newAcct = new NewAccount(acctNo, (int)newRow[CN.AgrmtNo], null, false, FormRoot, this);

                        //LW 70788 - NM - CR1005 - Service Charge Calculation - HP accts
                        if (newAcct.ScoringBand.Trim() == "" && newAcct.AccountType == AT.HP && newAcct.ScoreHPbefore == false)
                        {
                            ShowInfo("M_ACCOUNTNOTSCORED");
                        }
                        else
                        {
                            newAcct.Text = "New Sales Order";
                            //newAcct.date.datePropRF = StageToLaunch.DateProp;
                            ((MainForm)this.FormRoot).AddTabPage(newAcct, 6);
                            newAcct.SupressEvents = false;

                            if (acctNo.Substring(3, 1) == "4")
                                newAcct.PrintVouchers = true;
                        }
                    }
                    else
                    {
                        ShowInfo("M_ACCOUNTNOFAILED");
                    }
                }
            }
            else
            {
                ShowInfo("M_ACCOUNTNOFAILED");
            }
        }

        DateTime? lastRFCreatedTime = null;

        private void btnCreateRFAccount_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (Save())         // #12226 
                {
                    STL.PL.WS2.CashLoanDetails det = null;      // jec 25/10/11
                    errProRFCreateInterval.Clear();
                    if (lastRFCreatedTime.HasValue && DateTime.Now.Minute == lastRFCreatedTime.Value.Minute)
                    {
                        errProRFCreateInterval.SetError(btnCreateRFAccount, "Cannot create RF accounts within the same minute");
                        return;
                    }

                    if (customerBlacklisted == false) //IP - 01/09/09 - 5.2 (823) 
                    {
                        if (VerifyCustomerAge())
                        {
                            short branchNo = Convert.ToInt16(drpBranch.Text);
                            bool rescore = false;
                            bool reOpenS1 = false;
                            bool reOpenS2 = false;

                            Function = "btnCreateRFAccount_Click";
                            Wait();
                            if (Save() || this._hasdatachanged == false)
                            {
                                string acctno = AccountManager.CreateRFAccount(Config.CountryCode, branchNo,
                                    this.txtCustID.Text, Credential.UserId, false, ref det, out rescore,  //#3915 jec 28/09/11
                                    out reOpenS1, out reOpenS2, out Error);

                                if (Error.Length > 0)
                                    ShowError(Error);
                                else
                                {
                                    lastRFCreatedTime = DateTime.Now;
                                    this.Relationship = "H";        //UAT85 jec 20/10/10
                                    if (acctno.Length != 0)
                                    {
                                        this.btnRefresh_Click(null, null);

                                        if (rescore)
                                        {
                                            ShowInfo("M_RFRESCOREREQUIRED");
                                        }

                                        if (reOpenS1 && Convert.ToInt32(Country[CountryParameterNames.RescoreMonths]) > 0)
                                        {
                                            ShowInfo("M_RFSTAGE1");
                                        }

                                        if (reOpenS2 && Convert.ToInt32(Country[CountryParameterNames.RescoreMonths]) > 0)
                                        {
                                            ShowInfo("M_RFSTAGE2");
                                        }
                                    }
                                    else
                                    {
                                        ShowInfo("M_ACCOUNTNOFAILED");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        ShowInfo("M_CUSTOMERBLACKLISTED"); //IP - 01/09/09 - 5.2 (823) 
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
                Function = "End of btnCreateRFAccount_Click";
            }
        }

        private void txtCustID_TextChanged(object sender, System.EventArgs e)
        {
            //if(Relationship=="H")
            //	txtAccountNo.Text = "000-0000-0000-0";
            checkedForAcctsInArrs = false; //IP - 18/03/11 - #3345 - Set to false when customer id changed
        }

        public void txtCustID_Leave(object sender, System.EventArgs e)
        {
            CustIDLeave();
            LoyaltyNewMember = false;
        }

        public void CustIDLeave()
        {
            try
            {
                // This event was recursing and causing a stack overflow
                if (this.userChanged)
                {
                    this._customerStoreType = String.Empty;
                    this.userChanged = false;
                    bool status = true;

                    lblCashLoan.Visible = false;                //IP - 21/11/11 - #8680

                    Function = "txtCustID_Leave";
                    Wait();

                    // Make sure custid has been reformatted
                    // It seems the base leave event may occur after this leave event.
                    txtCustID.Text = txtCustID.Text;
                    addresshistory.Clear();
                    this.CustomerID = txtCustID.Text;
                    if (this.Relationship == "H")
                        this.Holder = this.CustomerID;
                    else
                    {
                        if (CustomerID == Holder)
                        {
                            txtCustID.SetError(GetResource("M_LINKEDCANNOTBEHOLDER"));
                            txtCustID.Select(0, txtCustID.Text.Length);
                            txtCustID.Focus();
                            status = false;
                        }
                        else
                        {
                            txtCustID.SetError(String.Empty);
                            this.Linked = this.CustomerID;
                            this.Holder = String.Empty;         //LW73019 jec 23/02/11
                        }
                    }

                    if (status)
                    {
                        stage1Complete = false;

                        if (!ReadOnly && !txtCustID.IsBlank(false))
                        {
                            //attempt to load the details for the customerID
                            //entered so that existing customers can be linked also
                            txtFirstName.Text = "";
                            txtLastName.Text = "";
                            txtAlias.Text = "";
                            txtMaidenName.Text = "";
                            drpMaritalStat1.Text = "";
                            drpNationality1.SelectedItem = null;


                            txtRFLimit.Text = (0).ToString(DecimalPlaces);
                            txtRFAvailable.Text = (0).ToString(DecimalPlaces);
                            if (tcAddress.TabPages.Count > 0)
                                tcAddress.TabPages.Clear();
                            if (dv != null)
                                dv.Table.Clear();
                            // cr906 rdb 19/09/07
                            //if (this.chxLoan.Visible)                                                         //IP - 24/10/11 - #8484 - CR1232 - No longer required
                            //    chxLoan.Enabled = false;
                            //artificially pass main holder as the relationship to force the
                            //load of this customer instead of linking through the account.
                            loadDetails(this.CustomerID, String.Empty, "H", ReadOnly);

                            if (this.CustomerID != string.Empty)         //#14473          
                            {
                                CashLoanCheckQual(this.CustomerID);      // #14429
                            }

                            customerSaved = false;
                            txtSignature.Text = String.Empty;
                            txtFileName.Text = String.Empty;
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
                this.userChanged = true;
                Function = "End of txtCustID_Leave";
                StopWait();
            }
        }

        public void txtAccountNo_TextChanged(object sender, System.EventArgs e)
        {
            AccountNoChanged();
        }

        public void AccountNoChanged()
        {
            try
            {
                Function = "txtAccountNo_TextChanged";
                this.AccountNo = txtAccountNo.Text.Replace("-", "");
                StageToLaunch.AccountNo = this.AccountNo;
                StageToLaunch.AcctType = this.AccountType;
                StageToLaunch.CheckType = String.Empty;
                StageToLaunch.CustID = CustomerID;
                StageToLaunch.ScreenMode = SM.Edit;
                StageToLaunch.ImageIndex = 0;

                if (AT.IsCreditType(AccountType))
                {
                    CreditManager.GetUnclearedStage(this.AccountNo,
                        ref StageToLaunch.AccountNo,
                        ref StageToLaunch.CheckType,
                        ref StageToLaunch.DateProp, ref propResult, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);

                    switch (StageToLaunch.CheckType)
                    {
                        case SS.S1:
                            StageToLaunch.Stage = GetResource("P_SANCTIONS1");
                            break;
                        case SS.DC:
                            StageToLaunch.Stage = GetResource("P_SANCTIONDC");
                            break;
                        case "":
                            StageToLaunch.Stage = GetResource("P_VIEWPROP");
                            StageToLaunch.CheckType = SS.S1;
                            break;
                        default:
                            StageToLaunch.Stage = GetResource("P_EDITSANCTIONSTAGES");
                            break;
                    }

                    if (propResult == PR.Rejected && menuRefer.Enabled)
                    {
                        StageToLaunch.CheckType = "R";
                        StageToLaunch.CustID = CustomerID;
                        StageToLaunch.ScreenMode = SM.Edit;
                        StageToLaunch.Stage = GetResource("P_REFERREJECTED");
                    }
                }
                Function = "End of txtAccountNo_TextChanged";
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void OnPrintPaymentCard(object sender, System.EventArgs e)
        {
            Function = "OnPrintPaymentCard";
            PrintPaymentCard(this.CustomerID, this.AccountNo);
            Function = "End of OnPrintPaymentCard";
        }

        private void BasicCustomerDetails_Load(object sender, System.EventArgs e)
        {
            try
            {
                //TranslateControls();
                if (this.CustomerID.Length != 0)
                    Recent.AddCustomer(CustomerID);
                toolTip1.SetToolTip(this.btnSave, GetResource("TT_SAVE"));
                toolTip1.SetToolTip(this.chxCreateHistory, GetResource("TT_CREATEHISTORY"));
                toolTip1.SetToolTip(this.chxCopyName, GetResource("TT_COPY"));//KD
                toolTip1.SetToolTip(this.drpAddressType, GetResource("TT_ADDRESSTYPE"));
                toolTip1.SetToolTip(this.btnRefresh, GetResource("TT_REFRESH"));
                toolTip1.SetToolTip(this.btnPrivilegeClub, GetResource("TT_PRIVLOYALCLUB"));

                // Display Privilege / Loyalty club title
                if ((bool)Country[CountryParameterNames.TierPCEnabled])
                    this.lPrivilegeClubDesc.Text = GetResource("T_LOYALTYCLUB");
                else
                    this.lPrivilegeClubDesc.Text = GetResource("T_PRIVILEGECLUB");

                if (!this.lEmploymentDetails.Enabled)
                    tcDetails.TabPages.Remove(this.tpEmployment);

                //if (!this.lBankDetails.Enabled) 
                //CR 835 Bank details removed as it is now incorporated in the financial tab [Peter Chong] 11-Oct-2006
                tcDetails.TabPages.Remove(this.tpBank);

                // CR 835 these tabs are only shown if the user groups has permission to view
                if (!this.lResidentialDetails.Enabled)
                    tcDetails.TabPages.Remove(this.tpResidential);

                if (!this.lFinancialDetails.Enabled)
                    tcDetails.TabPages.Remove(this.tpFinancial);

                //CR 866 Set Mandatory fields
                this.SetMandatoryFields();

                // Load the CASH icon from the server
                string path = String.Empty;
                if (LoadServerIcon("Cash_icon.gif", out path))
                {
                    // Found an icon so remove the button text
                    this.btnCreateCashAccount.Text = "";
                    // replace with the image
                    this.btnCreateCashAccount.BackgroundImage = Image.FromFile(path);
                    // and add a tooltip
                    toolTip1.SetToolTip(btnCreateCashAccount, GetResource("T_CREATECASH"));
                }

                // Load the HP icon from the server
                if (LoadServerIcon("HP_icon.gif", out path))
                {
                    // Found an icon so remove the button text
                    this.btnCreateHPAccount.Text = "";
                    // replace with the image
                    this.btnCreateHPAccount.BackgroundImage = Image.FromFile(path);
                    // and add a tooltip
                    toolTip1.SetToolTip(btnCreateHPAccount, GetResource("T_CREATEHP"));
                }

                // Load the RF icon from the server
                if (LoadServerIcon("ReadyFinance_icon.gif", out path))
                {
                    // Found an icon so remove the button text
                    this.btnCreateRFAccount.Text = "";
                    // replace with the image
                    this.btnCreateRFAccount.BackgroundImage = Image.FromFile(path);
                    // and add a tooltip
                    toolTip1.SetToolTip(btnCreateRFAccount, GetResource("T_CREATERF"));
                }

                // Load the Storedcard icon from the server
                if (LoadServerIcon("StoreCard_icon.gif", out path))
                {
                    // Found an icon so remove the button text
                    this.btnCreateStoreCard.Text = "";
                    // replace with the image
                    this.btnCreateStoreCard.BackgroundImage = Image.FromFile(path);
                    imgStoreCardNormal = Image.FromFile(path);
                    ConvertStoreCardImagetoGrey();
                    StorecardButtonCheckEnabled();
                    this.btnCreateStoreCard.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
                    // and add a tooltip
                    toolTip1.SetToolTip(btnCreateStoreCard, GetResource("T_CREATESTORE"));
                }


                // Show field for terms type band if this is enabled
                this.lBand.Visible = this.txtBand.Visible = (Convert.ToBoolean(Country[CountryParameterNames.TermsTypeBandEnabled]));

                // Determine whether or not Add Signature button should be visible
                btnAddSignature.Visible = Convert.ToBoolean(Country[CountryParameterNames.StoreCustomerSignature]);
                btnSaveSignature.Visible = Convert.ToBoolean(Country[CountryParameterNames.StoreCustomerSignature]);


                SetStoreCardFields(existingStoreCard); //IP - 21/12/10 - Store Card

            }
            catch (Exception ex)
            {
                Catch(ex, "BasicCustomerDetails_Load");
            }
        }

        private void drpTitle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this._hasdatachanged = true;
            if ((string)((DataRowView)drpTitle.SelectedItem)[CN.Code] == "MRS" ||
                (string)((DataRowView)drpTitle.SelectedItem)[CN.Code] == "MS")
            {
                txtMaidenName.Visible = true;
                lMaidenName.Visible = true;
            }
            else
            {
                txtMaidenName.Visible = false;
                lMaidenName.Visible = false;
            }
        }

        private void BasicCustomerDetails_Enter(object sender, System.EventArgs e)
        {
            try
            {
                BuildRecentListMenus();
            }
            catch (Exception ex)
            {
                Catch(ex, "BasicCustomerDetails_Enter");
            }
        }
        private void BuildRecentListMenus()
        {
            menuRecentCust.MenuCommands.Clear();
            foreach (string s in Recent.CustomerList)
            {
                Crownwood.Magic.Menus.MenuCommand m = new Crownwood.Magic.Menus.MenuCommand(s);
                menuRecentCust.MenuCommands.Add(m);
                m.Click += new System.EventHandler(OnRecentCustClick);
            }
        }

        private void OnRecentCustClick(object sender, System.EventArgs e)
        {
            Crownwood.Magic.Menus.MenuCommand m = (Crownwood.Magic.Menus.MenuCommand)sender;
            txtCustID.Text = m.Text;
            this.txtCustID_Leave(null, null);
        }

        public void btnRefresh_Click(object sender, System.EventArgs e)
        {
            RefreshData();
        }

        public void RefreshData()
        {
            try
            {
                errProRFCreateInterval.Clear();
                CustIDLeave();
                AccountNoChanged();
            }
            catch (Exception ex)
            {
                Catch(ex, "btnRefresh_Click");
            }
        }

        private void menuLinkToAccount_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                LinkCustAcct link = new LinkCustAcct(CustomerID, FormRoot, this);
                link.ShowDialog();
                btnRefresh_Click(null, null);

                if (link.Rescore)
                {
                    ShowInfo("M_RFRESCOREREQUIRED");
                }

                link.Dispose();
            }
            catch (Exception ex)
            {
                Catch(ex, "menuLinkToAccount_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void menuUnblockCredit_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (!txtCustID.IsBlank(true))
                {
                    CustomerManager.UnblockCredit(txtCustID.Text, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                        ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_CREDITUNBLOCKED");
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "menuUnblockCredit_Click");
            }
        }

        private void txtIncome_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Wait();

                if (!IsStrictMoney(txtIncome.Text))
                    txtIncome.Text = (0).ToString(DecimalPlaces);
                else
                    txtIncome.Text = MoneyStrToDecimal(txtIncome.Text).ToString(DecimalPlaces);
            }
            catch (Exception ex)
            {
                Catch(ex, "txtIncome_Validating");
            }
            finally
            {
                StopWait();
            }
        }

        private void menuManualRF_Click(object sender, System.EventArgs e)
        {
            bool rescore = false;
            try
            {
                ManualAccountEntry rf = new ManualAccountEntry(AT.ReadyFinance);
                rf.ShowDialog();

                if (rf.valid)
                {
                    string acctNo = rf.txtAccountNo.Text.Replace("-", "");

                    if (Save() || this._hasdatachanged == false)
                    {
                        AccountManager.CreateManualRFAccount(Config.CountryCode,
                            Convert.ToInt16(Config.BranchCode), this.txtCustID.Text,
                      acctNo, false, out rescore, out Error);
                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            if (acctNo.Length != 0)
                            {
                                this.btnRefresh_Click(null, null);

                                if (rescore)
                                    ShowInfo("M_RFRESCOREREQUIRED");
                            }
                            else
                            {
                                ShowInfo("M_ACCOUNTNOFAILED");
                            }
                        }
                    }
                }
                rf.Dispose();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void SetCashAndGo()
        {
            foreach (Crownwood.Magic.Controls.TabPage tp in ((MainForm)FormRoot).MainTabControl.TabPages)
            {
                if (tp.Control is NewAccount)
                {
                    NewAccount acct = (NewAccount)tp.Control;

                    if (acct.PaidAndTaken)
                    {
                        btnSave.Enabled = true;
                        btnSave.Visible = true;
                    }
                }
            }
        }

        //CR 866
        /// <summary>
        /// This method will store all the input fields and their labels in a hash table and then
        /// display them or not according to country specific requirements stored
        /// in the database
        /// </summary>
        private void SetMandatoryFields()
        {
            Function = "SetMandatoryFields()";

            Hashtable inputFields = new Hashtable();

            //new input fields
            inputFields["txtJobTitle1"] = this.txtJobTitle1;
            inputFields["drpEductation1"] = this.drpEductation1;
            inputFields["txtIndustry1"] = this.txtIndustry1;
            inputFields["txtOrganisation1"] = this.txtOrganisation1;
            inputFields["txtDistanceFromStore1"] = this.txtDistanceFromStore1;
            inputFields["drpTransportType1"] = this.drpTransportType1;


            //new labels
            inputFields["lJobTitle1"] = this.lJobTitle1;
            inputFields["lEducation1"] = this.lEducation1;
            inputFields["lIndustry1"] = this.lIndustry1;
            inputFields["lOrganisation1"] = this.lOrganisation1;
            inputFields["lDistanceFromStore1"] = this.lDistanceFromStore1;
            inputFields["lTransportType1"] = this.lTransportType1;

            #region retrieve mandatory fields

            if (mandatoryFieldsDS != null)
            {
                foreach (DataTable dt in mandatoryFieldsDS.Tables)
                {
                    if (dt.TableName == "Fields")
                    {
                        DataTable MandatoryFields = dt;
                        foreach (DataRow row in dt.Rows)
                        {
                            string key = (string)row["control"];
                            if (inputFields.ContainsKey(key))
                            {
                                switch (((Control)inputFields[key]).GetType().Name)
                                {
                                    case "TextBox":
                                        ((TextBox)inputFields[key]).ReadOnly = (!Convert.ToBoolean(row["enabled"]) || ReadOnly);
                                        ((TextBox)inputFields[key]).BackColor = SystemColors.Window;
                                        break;
                                    case "RichTextBox":
                                        ((RichTextBox)inputFields[key]).ReadOnly = (!Convert.ToBoolean(row["enabled"]) || ReadOnly);
                                        ((RichTextBox)inputFields[key]).BackColor = SystemColors.Window;
                                        break;
                                    case "DateTimePicker":
                                    case "ComboBox":
                                    case "Button":
                                    case "NumericUpDown":
                                        ((Control)inputFields[key]).Enabled = (Convert.ToBoolean(row["enabled"]) && !ReadOnly);
                                        ((Control)inputFields[key]).BackColor = SystemColors.Window;
                                        break;
                                    case "DatePicker":
                                        ((Control)inputFields[key]).Enabled = (Convert.ToBoolean(row["enabled"]) && !ReadOnly);
                                        break;
                                    default:
                                        break;
                                }
                                ((Control)inputFields[key]).Visible = Convert.ToBoolean(row["visible"]);


                                //Set properties for the label stored in the control's Tag
                                key = (string)((Control)inputFields[key]).Tag;
                                ((Control)inputFields[key]).Enabled = (Convert.ToBoolean(row["enabled"]) && !ReadOnly);
                                ((Control)inputFields[key]).Visible = Convert.ToBoolean(row["visible"]);
                            }
                        }
                    }
                }
            }



            #endregion

            if (Convert.ToBoolean(Country[CountryParameterNames.ApplyNewDIChanges]))
                groupBox7.Visible = false;
            else groupBox7.Visible = true;

        }

        //END CR 866


        private void btnCreateHPAccount_Click(object sender, System.EventArgs e)
        {
            bool Refresh = true;        // #12226
            try
            {
                if (Save())             // #12226
                {
                    if (customerBlacklisted == false) //IP - 01/09/09 - 5.2 UAT(823)
                    {
                        if (VerifyCustomerAge())
                        {

                            Wait();
                            this.CreateCustomerAccount(AT.HP);

                            this._hasdatachanged = true;
                        }
                    }
                    else
                    {
                        ShowInfo("M_CUSTOMERBLACKLISTED"); //IP - 01/09/09 - 5.2 (823) 
                    }
                }
                else
                {
                    Refresh = false;        // #12226
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnCreateHPAccount_Click");
            }
            finally
            {
                StopWait();
                if (Refresh)        // #12226
                {
                    RefreshData();          // jec 20/11/07
                }
            }
        }

        private void btnCreateCashAccount_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                if (Save())
                {
                    short branchNo = Convert.ToInt16(drpBranch.Text);
                    bool rescore = false;

                    string acctNo = AccountManager.CreateCustomerAccount(
                        Config.CountryCode,
                        branchNo,
                        this.txtCustID.Text,
                        AT.Cash,
                        Credential.UserId,
                       false,
                        out rescore,
                        out Error);
                    this.newAccounttoCancel = acctNo;
                    this._hasdatachanged = true;

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        if (Save())
                        {
                            this.Relationship = "H";        //UAT85 jec 02/11/10
                                                            // UAT9 check loyalty before entering New Sales Order screen for Cash accounts
                            if (Convert.ToBoolean(Country[CountryParameterNames.LoyaltyScheme]) && CheckCanJoinLoyalty(acctNo))
                            {
                                this.tcDetails.SelectedTab = tpLoyaltyScheme;
                            }
                            else
                            {
                                this.OpenNewAccount(acctNo, AT.Cash);
                            }
                        }
                    }
                    this._hasdatachanged = true;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnCreateCashAccount_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void menuSpecialAccount_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                this.CreateCustomerAccount(AT.Special);
            }
            catch (Exception ex)
            {
                Catch(ex, "menuSpecialAccount_Click");
            }
            finally
            {
                StopWait();
            }

        }

        private void menuManualHP_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                this.CreateManualAccount(AT.HP);
            }
            catch (Exception ex)
            {
                Catch(ex, "menuManualHP_Click");
            }
            finally
            {
                StopWait();
            }

        }

        private void menuManualCash_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                this.CreateManualAccount(AT.Cash);
            }
            catch (Exception ex)
            {
                Catch(ex, "menuManualCash_Click");
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// OnKeyPress
        /// should trap whether data changed updating _hasdatachanged...should? does it mean that you are not sure?
        /// </summary>
        /// 
        public void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            this._hasdatachanged = true;
        }

        /// <summary>
        /// VerifyCustomerAge
        /// Users will not be able to create RF & HP accounts if the 
        /// customers age is below the country parameter that sets 
        /// the minimum age for credit accounts.
        /// </summary>
        /// 
        private bool VerifyCustomerAge()
        {
            bool valid = true;

            if (CalculateAge() < (decimal)Country[CountryParameterNames.MinHPage])
            {
                valid = false;
                errorProvider1.SetError(dtDOB, GetResource("M_APPLICANTTOOYOUNG"));
            }
            else
                errorProvider1.SetError(dtDOB, String.Empty);

            return valid;
        }

        private decimal CalculateAge()
        {
            int y = DateTime.Today.Year - dtDOB.Value.Year;
            int m = DateTime.Today.Month - dtDOB.Value.Month;
            int d = DateTime.Today.Day - dtDOB.Value.Day;
            if (d < 0) m--;
            if (m < 0) y--;

            return Convert.ToDecimal(y);
        }

        /// <summary>
        /// CR903 - Finds the store type of the selected branch
        /// </summary>
        /// <returns>Store Type</returns>
        private string FindStoreType()
        {
            string type = String.Empty;

            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
            {
                if (row["branchno"].ToString() == drpBranch.Text)
                {
                    type = row[CN.StoreType].ToString();
                    createRF = (bool)row[CN.CreateRFAccounts];         // jec 07/08/07
                    createCash = (bool)row[CN.CreateCashAccounts];     // jec
                    createHP = (bool)row[CN.CreateHPAccounts];         // jec 28/08/07 aahh
                    var createStoreAccounts = row[CN.CreateStoreAccounts];
                    CreateStore = createStoreAccounts == DBNull.Value ? false : (bool)createStoreAccounts;
                    break;
                }
            }

            if (createHP)
            {
                createHP = allowHP.Enabled;
            }

            if (createRF)
            {
                createRF = allowRF.Enabled;
            }

            if (createCash)
            {
                createCash = allowCash.Enabled;
            }

            if (CreateStore)
            {
                CreateStore = allowStore.Enabled;
            }

            return type;
        }

        /// <summary>
        /// CR903 - Builds a list of branches to filter the branch
        /// drop down field and also to filter customer accounts
        /// </summary>
        /// <returns>string</returns>
        protected string BuildStoreTypeFilter()
        {
            StringBuilder sbStoreType = new StringBuilder();

            if (Config.StoreType == StoreType.Courts)
            {
                sbStoreType.Append("branchno in( ");
            }
            else
            {
                sbStoreType.Append("branchno not in( ");
            }

            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
            {
                if (row[CN.StoreType].ToString() == StoreType.Courts)
                {
                    sbStoreType.Append(row[CN.BranchNo].ToString());
                    sbStoreType.Append(",");
                }
            }

            sbStoreType.Remove(sbStoreType.Length - 1, 1);
            sbStoreType.Append(")");

            return sbStoreType.ToString();
        }

        /// <summary>
        /// CR903 - returns whether or not ScoreHPbefore has been enabled for the current branch
        /// </summary>
        /// <returns></returns>
        protected bool ReturnScoreHPResult()
        {
            bool result = false;
            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
            {
                if (row[CN.BranchNo].ToString() == (string)drpBranch.Text)
                {
                    result = (bool)row[CN.ScoreHPbefore];
                    break;
                }
            }
            return result;
        }

        private void drpNationality1_SelectedValueChanged(object sender, EventArgs e)
        {
            DataRowView row = (DataRowView)drpNationality1.SelectedItem;
            if (row != null)
            {
                txtCustID.curNationality = (row[CN.Reference].ToString() == "0") ? "" : row[CN.Reference].ToString();
                txtCustID.Text = txtCustID.Text;
            }

        }

        // CR903 - CreateRF and Create Cash buttons etc only visible if 
        // allowed by Branch                jec 07/08/07
        private void drpBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetUpAccountTypeVisibility();

        }

        void SetUpAccountTypeVisibility()
        {
            if (staticLoaded == true)
            {
                FindStoreType();
                // CR906 if MaxLoanAmount is 0 dont show loan chackbox
                //chxLoan.Visible = (createRF && (Convert.ToInt32(Country[CountryParameterNames.CL_MaxLoanAmount]) > 0));        //IP - 24/10/11 - #8484 - CR1232 - No longer required       //CR906 rdb 


                btnCreateCashAccount.Enabled = createCash;
                btnCreateCashAccount.Visible = createCash;
                btnCreateRFAccount.Enabled = createRF && validAge;  //IP - 11/05/10 - UAT(141) UAT5.2.1.0 Log - added validAge
                btnCreateRFAccount.Visible = createRF;
                btnCreateHPAccount.Enabled = createHP && validAge;      //CR903 jec     //IP - 11/05/10 - UAT(141) UAT5.2.1.0 Log - added validAge
                btnCreateHPAccount.Visible = createHP;      //CR903 jec 
                                                            //btnCreateStoreCard.Enabled = CreateStore && validAge && !existingStoreCard;
                btnCreateStoreCard.Visible = CreateStore;

                txtRFLimit.Visible = createRF;
                txtRFAvailable.Visible = createRF;
                labelCredit.Visible = createRF;
                labelAvailable.Visible = createRF;
                txtRFIssueNo.Visible = createRF;
                lRFIssueNo.Visible = createRF;
                int status = 0;
                AccountManager.DisableRFButton(CustomerID, out status);
                //Mambu

                if (status == 1 && validAge)
                {
                    btnCreateRFAccount.Enabled = true;

                }
                else
                {
                    btnCreateRFAccount.Enabled = false;
                }
                // CR906 if MaxLoanAmount is 0 dont show loan chackbox
                //chxLoan.Visible = (createRF && (Convert.ToInt32(Country[CountryParameterNames.CL_MaxLoanAmount]) > 0));  //IP - 24/10/11 - #8484 - CR1232 - No longer required             //CR906 rdb


                /*
                if (_customerStoreType != string.Empty && !Config.StoreType.Equals(_customerStoreType))
                {
                    btnCreateCashAccount.Visible = false;
                    btnCreateHPAccount.Visible = false;
                    btnCreateRFAccount.Visible = false;
                    btnCreateStoreCard.Visible = false;
                }
                */
                if (_customerStoreType != string.Empty && Config.StoreType.Equals(_customerStoreType))
                {
                    btnCreateHPAccount.Visible = createHP;      //CR903
                }
            }
        }



        #region Residential Details Tab

        //Note a copy of this function exists in the sanction stage 1 screen
        private void LoadResidentialData(DataTable dt)
        {


            Function = "LoadResidentialData";
            if (dt.Rows.Count == 0)
                return;

            ClearControls(tpResidential.Controls);

            DataRow row = dt.Rows[0];

            if (row == null)
                return;

            if (row[CN.DateIn].ToString() != "") //#14755
            {
                m_currentDateIn = (DateTime)row[CN.DateIn];
            }

            //current residential status
            if (!Convert.IsDBNull(row[CN.ResidentialStatus]))
                drpCurrentResidentialStatus1.SelectedValue = row[CN.ResidentialStatus].ToString().Trim();

            //previous residential status
            if (!Convert.IsDBNull(row[CN.PrevResidentialStatus]))
                drpPrevResidentialStatus1.SelectedValue = row[CN.PrevResidentialStatus].ToString().Trim();

            //property type
            if (!Convert.IsDBNull(row[CN.PropertyType]))
                drpPropertyType1.SelectedValue = row[CN.PropertyType].ToString().Trim();

            //Monthly Rent
            if (!Convert.IsDBNull(row[CN.MonthlyRent]))
                txtMortgage1.Text = ((double)row[CN.MonthlyRent]).ToString(DecimalPlaces);

            if (m_currentDateIn > DatePicker.MinValue)
            {
                dtDateInCurrentAddress1.DateFrom = DateTime.Today;
                dtDateInCurrentAddress1.LinkedBias = true;
                dtDateInCurrentAddress1.LinkedDatePicker = dtDateInPrevAddress1;
                dtDateInCurrentAddress1.LinkedComboBox = drpPrevResidentialStatus1;
                dtDateInCurrentAddress1.LinkedLabel = lPrevResidentialStatus1;
                //70363 row[CN.DateIn] is not getting the most recent address date. This is because when first saved the default value of 08/11/2006 is saved to the database
                dtDateInCurrentAddress1.Value = m_currentDateIn;

                //if (currentDateList.ContainsKey("H"))
                //{
                //    currentDateList["H"] = dtDateInCurrentAddress1.Value;   //IP - 24/06/11 - 5.13 - UAT23 - #3625
                //}
                addresshistory.Add("H", drpRelationship.SelectedValue.ToString(), m_currentDateIn);
            }

            dtDateInPrevAddress1.DateFrom = dtDateInCurrentAddress1.Value;

            if (Convert.IsDBNull(row[CN.PrevDateIn]))
                dtDateInPrevAddress1.Value = dtDateInCurrentAddress1.Value;
            else if ((DateTime)row[CN.PrevDateIn] > DatePicker.MinValue)
                dtDateInPrevAddress1.Value = (DateTime)row[CN.PrevDateIn];
            else
                dtDateInPrevAddress1.Value = dtDateInCurrentAddress1.Value;
        }

        private DataSet SaveResidentialData()
        {

            DataTable dt = null;
            DataRow dr = null;
            bool newRow = false;

            if (basicDetails.Tables.IndexOf(TN.CustomerAdditionalDetailsResidential) < 0)
            {
                dt = new DataTable(TN.CustomerAdditionalDetailsResidential);
                #region Create Residential Table
                dt.Columns.AddRange(new DataColumn[] { new DataColumn(CN.CustID),
                                                                  new DataColumn(CN.DateIn, Type.GetType("System.DateTime")),
                                                                  new DataColumn(CN.PrevDateIn, Type.GetType("System.DateTime")),
                                                                  new DataColumn(CN.ResidentialStatus),
                                                                  new DataColumn(CN.PrevResidentialStatus),
                                                                  new DataColumn(CN.PropertyType),
                                                                  new DataColumn(CN.MonthlyRent)});
                #endregion
            }
            else
                dt = basicDetails.Tables[TN.CustomerAdditionalDetailsResidential];

            if (dt.Rows.Count > 0)
                dr = dt.Rows[0];
            else
            {
                dr = dt.NewRow();
                dr[CN.CustID] = this.CustomerID;
                newRow = true;
            }

            dr[CN.DateIn] = dtDateInCurrentAddress1.Value;
            dr[CN.PrevDateIn] = dtDateInPrevAddress1.Value;

            if (drpCurrentResidentialStatus1.SelectedIndex != -1)
                dr[CN.ResidentialStatus] = (string)((DataRowView)drpCurrentResidentialStatus1.SelectedItem)[CN.Code];

            if (drpPrevResidentialStatus1.SelectedIndex != -1)
                dr[CN.PrevResidentialStatus] = (string)((DataRowView)drpPrevResidentialStatus1.SelectedItem)[CN.Code];

            if (drpPropertyType1.SelectedIndex != -1)
                dr[CN.PropertyType] = (string)((DataRowView)drpPropertyType1.SelectedItem)[CN.Code];

            if (txtMortgage1.Text.Trim().Equals(""))
                dr[CN.MonthlyRent] = DBNull.Value;
            else
                dr[CN.MonthlyRent] = Convert.ToDouble(StripCurrency(txtMortgage1.Text));

            if (newRow)
                dt.Rows.Add(dr);

            DataSet ds = new DataSet();

            ds.Merge(dt);
            dt.Dispose();
            return ds;
        }

        private void btnSaveResidential_Click(object sender, EventArgs e)
        {

            //((MainForm)this.FormRoot).statusBar1.Text = "Saving residential details";
            //DataSet ds = SaveResidentialData();
            //CustomerManager.SaveCustomerAdditionalDetailsResidential(ds, out Error);

            //if (Error.Length > 0)
            //    ShowError(Error);

            //((MainForm)this.FormRoot).statusBar1.Text = string.Empty;

            btnSave_Click(null, null);
        }

        #endregion


        #region Financial Details Tab
        //Note a copy of this function exists in the sanction stage 1 screen
        private void LoadFinancialData(DataTable dt)
        {
            Function = "LoadFinancialData";
            bool IsDispIncomeChangesApplied = Convert.ToBoolean(Country[CountryParameterNames.ApplyNewDIChanges]);
            if (dt.Rows.Count == 0)
                return;
            DataRow row = dt.Rows[0];
            //decimal total = 0;


            if (DBNull.Value != row[CN.Commitments1])
                txtUtilities1.Text = ((decimal)row[CN.Commitments1]).ToString(DecimalPlaces);
            else if (IsDispIncomeChangesApplied)
                txtUtilities1.Text = "0.00";
            else
                txtUtilities1.Text = "";


            if (DBNull.Value != row[CN.Commitments2])
                txtLoans1.Text = ((decimal)row[CN.Commitments2]).ToString(DecimalPlaces);
            else if (IsDispIncomeChangesApplied)
                txtLoans1.Text = "0.00";
            else
                txtLoans1.Text = "";

            if (DBNull.Value != row[CN.Commitments3])
                txtMisc1.Text = ((decimal)row[CN.Commitments3]).ToString(DecimalPlaces);
            else if (IsDispIncomeChangesApplied)
                txtMisc1.Text = "0.00";
            else
                txtMisc1.Text = "";


            if (row[CN.MonthlyIncome] != DBNull.Value)
                txtNetIncome1.Text = ((decimal)row[CN.MonthlyIncome]).ToString(DecimalPlaces);
            else
                txtNetIncome1.Text = "";

            if (DBNull.Value != row[CN.AdditionalIncome])
                txtAddIncome1.Text = ((decimal)row[CN.AdditionalIncome]).ToString(DecimalPlaces);
            else
                txtAddIncome1.Text = "";


            if (DBNull.Value != row[CN.OtherPayments])
                txtOther1.Text = ((decimal)row[CN.OtherPayments]).ToString(DecimalPlaces);
            else if (IsDispIncomeChangesApplied)
                txtOther1.Text = "0.00";
            else
                txtOther1.Text = "";


            if (DBNull.Value != row[CN.AdditionalExpenditure1])
                txtAdditionalExpenditure1.Text = ((decimal)row[CN.AdditionalExpenditure1]).ToString(DecimalPlaces);
            else if (IsDispIncomeChangesApplied)
                txtAdditionalExpenditure1.Text = "0.00";
            else
                txtAdditionalExpenditure1.Text = "";

            if (DBNull.Value != row[CN.AdditionalExpenditure2])
                txtAdditionalExpenditure2.Text = ((decimal)row[CN.AdditionalExpenditure2]).ToString(DecimalPlaces);
            else if (IsDispIncomeChangesApplied)
                txtAdditionalExpenditure2.Text = "0.00";
            else
                txtAdditionalExpenditure2.Text = "";

            //txtTotal1.Text = total.ToString(DecimalPlaces);
            //this.SumExpenditure();

            //Load credit card no
            txtCreditCardNo1.One = (string)row[CN.CCardNo1];
            txtCreditCardNo1.Two = (string)row[CN.CCardNo2];
            txtCreditCardNo1.Three = (string)row[CN.CCardNo3];
            txtCreditCardNo1.Four = (string)row[CN.CCardNo4];

            //Load default giro fields here if they exist
            if (!Convert.IsDBNull(row[CN.DueDayId]))
                foreach (DataRowView r in drpGiroDueDate1.Items)
                {
                    if ((int)r[CN.DueDayId] == (int)row[CN.DueDayId])
                    {
                        drpGiroDueDate1.SelectedItem = r;
                        break;
                    }
                }

            if (!Convert.IsDBNull(row[CN.BankAccountName]))
                txtBankAccountName1.Text = (string)row[CN.BankAccountName];

            if (!Convert.IsDBNull(row[CN.Paymentmethod]))
                foreach (DataRowView r in drpPaymentMethod.Items)
                {
                    if ((string)r[CN.Code] == (string)row[CN.Paymentmethod])
                    {
                        drpPaymentMethod.SelectedItem = r;
                    }
                }
        }


        private DataSet SaveFinancialData()
        {

            //Bank details are now part of financials tab [CR 835 - Katie & Peter Chong] - 12-Oct-2006
            DataSet ds = SaveBank(this.CustomerID);

            DataTable dt = null;
            DataRow dr = null;
            bool newRow = false;

            if (basicDetails.Tables.IndexOf(TN.CustomerAdditionalDetailsFinancial) < 0)
            {
                dt = new DataTable(TN.CustomerAdditionalDetailsFinancial);
                #region create financial table
                dt.Columns.AddRange(new DataColumn[] { new DataColumn(CN.CustID),
                                                                  new DataColumn(CN.MonthlyIncome, Type.GetType("System.Double")),
                                                                  new DataColumn(CN.AdditionalIncome, Type.GetType("System.Double")),
                                                                  new DataColumn(CN.Commitments1, Type.GetType("System.Double")),
                                                                  new DataColumn(CN.Commitments2, Type.GetType("System.Double")),
                                                                  new DataColumn(CN.Commitments3, Type.GetType("System.Double")),
                                                                  new DataColumn(CN.OtherPayments, Type.GetType("System.Double")),
                                                                  new DataColumn(CN.AdditionalExpenditure1, Type.GetType("System.Double")),
                                                                  new DataColumn(CN.AdditionalExpenditure2, Type.GetType("System.Double")),
                                                                  new DataColumn(CN.CCardNo1),
                                                                  new DataColumn(CN.CCardNo2),
                                                                  new DataColumn(CN.CCardNo3),
                                                                  new DataColumn(CN.CCardNo4),
                                                                  new DataColumn(CN.DueDayId, Type.GetType("System.Int32")),
                                                                  new DataColumn(CN.BankAccountName),
                                                                  new DataColumn(CN.Paymentmethod)


                });
                #endregion
            }
            else
                dt = basicDetails.Tables[TN.CustomerAdditionalDetailsFinancial];

            if (dt.Rows.Count > 0)
                dr = dt.Rows[0];
            else
            {

                dr = dt.NewRow();

                dr[CN.CustID] = this.CustomerID;
                newRow = true;
            }
            if (txtNetIncome1.Text.Trim().Equals(""))
                dr[CN.MonthlyIncome] = DBNull.Value;
            else
                dr[CN.MonthlyIncome] = Convert.ToDouble(StripCurrency(txtNetIncome1.Text));

            if (txtAddIncome1.Text.Trim().Equals(""))
                dr[CN.AdditionalIncome] = DBNull.Value;
            else
                dr[CN.AdditionalIncome] = Convert.ToDouble(StripCurrency(txtAddIncome1.Text));

            if (txtUtilities1.Text.Trim().Equals(""))
                dr[CN.Commitments1] = DBNull.Value;
            else
                dr[CN.Commitments1] = Convert.ToDouble(StripCurrency(txtUtilities1.Text));

            if (txtLoans1.Text.Trim().Equals(""))
                dr[CN.Commitments2] = DBNull.Value;
            else
                dr[CN.Commitments2] = Convert.ToDouble(StripCurrency(txtLoans1.Text));

            if (txtMisc1.Text.Trim().Equals(""))
                dr[CN.Commitments3] = DBNull.Value;
            else
                dr[CN.Commitments3] = Convert.ToDouble(StripCurrency(txtMisc1.Text));

            if (txtOther1.Text.Trim().Equals(""))
                dr[CN.OtherPayments] = DBNull.Value;
            else
                dr[CN.OtherPayments] = Convert.ToDouble(StripCurrency(txtOther1.Text));

            if (txtAdditionalExpenditure1.Text.Trim().Equals(""))
                dr[CN.AdditionalExpenditure1] = DBNull.Value;
            else
                dr[CN.AdditionalExpenditure1] = Convert.ToDouble(StripCurrency(txtAdditionalExpenditure1.Text));

            if (txtAdditionalExpenditure2.Text.Trim().Equals(""))
                dr[CN.AdditionalExpenditure2] = DBNull.Value;
            else
                dr[CN.AdditionalExpenditure2] = Convert.ToDouble(StripCurrency(txtAdditionalExpenditure2.Text));

            dr[CN.CCardNo1] = txtCreditCardNo1.One;
            dr[CN.CCardNo2] = txtCreditCardNo1.Two;
            dr[CN.CCardNo3] = txtCreditCardNo1.Three;
            dr[CN.CCardNo4] = txtCreditCardNo1.Four;

            //Save the giro fields here if they are allowed
            //But we always want to set the due date id because it cannot be null

            if ((bool)Country[CountryParameterNames.DDEnabled])
            {
                if (IsNumeric(((DataRowView)drpGiroDueDate1.SelectedItem)[CN.DueDayId].ToString()))
                    dr[CN.DueDayId] = (int)((DataRowView)drpGiroDueDate1.SelectedItem)[CN.DueDayId];
                else
                    dr[CN.DueDayId] = 0;

                dr[CN.BankAccountName] = this.txtBankAccountName1.Text;

                if (drpPaymentMethod.SelectedIndex != -1)
                    dr[CN.Paymentmethod] = (string)((DataRowView)drpPaymentMethod.SelectedItem)[CN.Code];
            }
            else
            {
                dr[CN.BankAccountName] = string.Empty;
                dr[CN.DueDayId] = 0;
                dr[CN.Paymentmethod] = string.Empty;

            }

            if (newRow)
                dt.Rows.Add(dr);

            ds.Merge(dt);
            dt.Dispose();
            return ds;

        }


        private void SumExpenditure()
        {
            decimal m = 0, u = 0, l = 0, o = 0, p = 0, a1 = 0, a2 = 0;
            try
            {

                m = MoneyStrToDecimal(txtMisc1.Text);
                u = MoneyStrToDecimal(txtUtilities1.Text);
                l = MoneyStrToDecimal(txtLoans1.Text);
                o = MoneyStrToDecimal(txtOther1.Text);
                a1 = MoneyStrToDecimal(txtAdditionalExpenditure1.Text);
                a2 = MoneyStrToDecimal(txtAdditionalExpenditure2.Text);
                //p=  MoneyStrToDecimal(txtMortgage1.Text);
            }
            catch (FormatException)
            {
                //Nothing much to do if this happens, just don't
                //want it to be unhandled
            }
            txtTotal1.Text = (m + u + l + o + p + a1 + a2).ToString(DecimalPlaces);
            CalculateDisposableIncome();
        }
        private void CalculateDisposableIncome()
        {
            decimal rentFactor = Convert.ToDecimal(Country[CountryParameterNames.RentFactor]);
            decimal commitments = MoneyStrToDecimal(txtTotal1.Text);
            decimal income = MoneyStrToDecimal(txtNetIncome1.Text);
            decimal addIncome = MoneyStrToDecimal(txtAddIncome1.Text);
            decimal mortgage = MoneyStrToDecimal(txtMortgage1.Text);
            decimal monthlyIncome = income + addIncome;
            decimal disposable = 0;

            if (Convert.ToBoolean(Country[CountryParameterNames.ApplyNewDIChanges]))
            {
                this.lDisposable.Text = string.Empty;
                this.dependentSpendFactor = this.GetDependentSpendFactor();
                this.applicantSpendFactor = this.GetApplicantSpendFactor();

                if (maritalStatusFromProposal.Equals("M") && IsSpouseWorking)
                {
                    mortgage = ((rentFactor / 100) * mortgage);
                }

                disposable = monthlyIncome - (
                                                mortgage
                                                + (DependantsFromProposal * (dependentSpendFactor / 100) * monthlyIncome)
                                                + ((applicantSpendFactor / 100) * monthlyIncome)
                                             );
            }
            else
            {
                disposable = monthlyIncome - commitments - mortgage;
            }

            txtDisposable.Text = (disposable).ToString(DecimalPlaces);
        }

        private void txtUtilities1_Validating(object sender, CancelEventArgs e)
        {
            if (ValidateMoney((TextBox)sender))
                SumExpenditure();
        }

        private void txtMisc1_Validating(object sender, CancelEventArgs e)
        {
            if (ValidateMoney((TextBox)sender))
                SumExpenditure();
        }

        private void txtLoans1_Validating(object sender, CancelEventArgs e)
        {
            if (ValidateMoney((TextBox)sender))
                SumExpenditure();
        }

        private void txtOther1_Validating(object sender, CancelEventArgs e)
        {
            if (ValidateMoney((TextBox)sender))
                SumExpenditure();
        }

        private void txtAdditionalExpenditure1_Validating(object sender, CancelEventArgs e)
        {
            if (ValidateMoney((TextBox)sender))
                SumExpenditure();
        }

        private void txtAdditionalExpenditure2_Validating(object sender, CancelEventArgs e)
        {
            if (ValidateMoney((TextBox)sender))
                SumExpenditure();
        }

        private bool ValidateMoney(TextBox tb)
        {

            bool valid = true;
            try
            {
                Function = "ValidateMoney()";

                if (tb.Enabled)
                {
                    // (M49,M104) DSR 10/4/03 - Need to check IsNumeric even when not mandatory
                    tb.Text = tb.Text.Trim();
                    tb.Text = StripCurrency(tb.Text);
                    if (!IsStrictNumeric(tb.Text))
                    {
                        valid = false;
                        errorProvider1.SetError(tb, GetResource("M_NONNUMERIC"));

                    }

                    else
                    {
                        valid = true;
                        errorProvider1.SetError(tb, String.Empty);
                        if (tb.Text.Length > 0)
                        {
                            tb.Text = (Convert.ToDecimal(tb.Text)).ToString(DecimalPlaces);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
                valid = false;
            }
            return valid;
        }

        private void btnSaveFinancial_Click(object sender, EventArgs e)
        {
            //Use this code to save tab individually
            //((MainForm)this.FormRoot).statusBar1.Text = "Saving financial details";
            //DataSet ds = SaveFinancialData();
            //CustomerManager.SaveCustomerAdditionalDetailsFinancial(this.CustomerID, "", ds, out Error);

            //if (Error.Length > 0)
            //    ShowError(Error);

            //((MainForm)this.FormRoot).statusBar1.Text = string.Empty;

            btnSave_Click(null, null);

        }
        #endregion


        //returns true if the customer has at least one credit account
        private bool ShouldAddtionalDetailsSave()
        {
            //Additional details now alway save [PC]
            return true;

            //old code same...
            #region [ Old Code ]
            //bool bEnable = false;


            //if (accounts != null)
            //{
            //    if (accounts.Tables["CustSearch"].Rows.Count.Equals(0))
            //        bEnable = true;
            //    else
            //    {
            //        foreach (DataRow r in accounts.Tables["CustSearch"].Rows)
            //            if (AT.IsCreditType((string)r[CN.Type]))
            //            {
            //                bEnable = true;
            //                break;
            //            }
            //    }
            //}


            // return bEnable;
            #endregion
        }

        private void btnSaveEmployment_Click(object sender, EventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void txtNetIncome1_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                Function = "txtNetIncome1_Validating";
                this.CalculateDisposableIncome();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void txtAddIncome1_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                Function = "txtAddIncome1_Validating";
                this.CalculateDisposableIncome();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void dtDOB_ValueChanged(object sender, EventArgs e)
        {
            validAge = CheckAge(); //IP - 16/04/10 - UAT(94) UAT5.2 - Merged from version 4.3
        }

        //IP - 16/04/10 - UAT(94) UAT5.2 - Merged from version 4.3
        private bool CheckAge()
        {
            if ((((TimeSpan)DateTime.Now.Subtract(dtDOB.Value)).TotalDays / 365 >= Convert.ToDouble(Country[CountryParameterNames.MinHPage])) &&
        (((TimeSpan)DateTime.Now.Subtract(dtDOB.Value)).TotalDays / 365 <= Convert.ToDouble(Country[CountryParameterNames.MaxHPage])))
            {
                //if ((string)drpRelationship.SelectedValue == "H")
                if (((string)((DataRowView)drpRelationship.SelectedItem)["code"] == "H"))
                {
                    btnCreateRFAccount.Enabled = true;
                    btnCreateHPAccount.Enabled = true;      //IP - 11/05/10 - UAT(141) UAT5.2.1.0 log 
                }
                int status = 0;
                AccountManager.DisableRFButton(CustomerID, out status);
                if (status == 0)
                {
                    btnCreateRFAccount.Enabled = false;

                }
                errorProvider1.SetError(dtDOB, "");
                return true;
            }
            else
            {
                btnCreateRFAccount.Enabled = false;
                btnCreateHPAccount.Enabled = false;         //IP - 11/05/10 - UAT(141) UAT5.2.1.0 log
                if (dtDOB.Enabled == true)          //CR1030 #3095 jec - only show message if enabled
                {
                    errorProvider1.SetError(dtDOB, GetResource("DOBCUSTOMER"));
                }

                return false;
            }
        }

        //IP - 11/05/10 - UAT(141) UAT5.2.1.0 log - Merged from 4.3
        private bool CheckAccountRelationship()
        {
            //if ((string)drpRelationship.SelectedValue == "H"
            if (((string)((DataRowView)drpRelationship.SelectedItem)["code"] == "H"))
            {
                if (CheckAge())
                {
                    btnCreateRFAccount.Enabled = true;
                }
                errorProvider1.SetError(drpRelationship, "");
                return true;
            }
            else
            {
                btnCreateRFAccount.Enabled = false;
                errorProvider1.SetError(drpRelationship, "Note: Only main account holder can create subagreements.");
                return false;
            }
        }

        private void SetIncome(bool fromFinancial)
        {
            string weekly = "W";
            string fortnightly = "F";
            string frequency = ((string)((DataRowView)drpPayFrequency.SelectedItem)[CN.Code]).Trim();
            decimal income = MoneyStrToDecimal(txtIncome.Text);
            decimal monthlyIncome = MoneyStrToDecimal(txtNetIncome1.Text);
            decimal factor = 1;

            bool showPayAmount = (frequency == weekly || frequency == fortnightly);

            lIncome.Visible = showPayAmount;
            txtIncome.Visible = showPayAmount;

            if (frequency == weekly)
            {
                factor = 4.33M;
                lIncome.Text = "Weekly Income";
            }
            else if (frequency == fortnightly)
            {
                factor = 2.16M;
                lIncome.Text = "Fortnightly Income";
            }

            if (fromFinancial)
            {
                // Update the employment tab from the amount on the financial tab
                income = monthlyIncome / factor;
            }
            else
            {
                // Update the financial tab from the amount on the employment tab
                monthlyIncome = income * factor;
            }

            txtIncome.Text = income.ToString(DecimalPlaces);
            txtNetIncome1.Text = monthlyIncome.ToString(DecimalPlaces);
        }

        private void drpPayFrequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SetIncome(true);
            }
            catch (Exception ex)
            {
                Catch(ex, "drpPayFrequency_SelectedIndexChanged");
            }
        }

        private void txtIncome_Leave(object sender, EventArgs e)
        {
            try
            {
                SetIncome(false);
            }
            catch (Exception ex)
            {
                Catch(ex, "txtIncome_Leave");
            }
        }

        private void txtNetIncome1_Leave(object sender, EventArgs e)
        {
            try
            {
                SetIncome(true);
            }
            catch (Exception ex)
            {
                Catch(ex, "drpPayFrequency_SelectedIndexChanged");
            }
        }

        private void btnTakePicture_Click(object sender, EventArgs e)
        {
            // create COM instance of WIALib manager
            WIALib.WiaClass wiaManager = new WiaClass();
            WIALib.CollectionClass wiaDevs = null;
            WIALib.ItemClass wiaRoot = null;
            WIALib.CollectionClass wiaPics = null;
            WIALib.ItemClass wiaItem = null;
            try
            {
                wiaDevs = wiaManager.Devices as WIALib.CollectionClass;
                // call Wia.Devices to get all devices
                if ((wiaDevs == null) || (wiaDevs.Count == 0))
                {
                    MessageBox.Show(this, "No WIA devices found!", "WIA", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                WIA.DeviceManagerClass devMgr = new WIA.DeviceManagerClass();

                Device cameraDevice = null;
                foreach (WIA.DeviceInfo info in devMgr.DeviceInfos)
                {
                    cameraDevice = info.Connect();
                    break;
                }

                //Remove all previous photos from the camera
                int x = cameraDevice.Items.Count + 1;
                for (int i = 1; i < x; i++)
                {
                    cameraDevice.Items.Remove(1);  // remove the item from the device Items collection            
                    cameraDevice.ExecuteCommand(WIA.CommandID.wiaCommandSynchronize);
                }

                object selectUsingUI = System.Reflection.Missing.Value;
                wiaRoot = (WIALib.ItemClass)wiaManager.Create(ref selectUsingUI);

                if (wiaRoot == null)
                {
                    return;
                }

                // this call shows the common WIA dialog to let the user select a picture:

                wiaPics = wiaRoot.GetItemsFromUI(WiaFlag.SingleImage, WiaIntent.ImageTypeColor) as CollectionClass;
                if (wiaPics == null)
                {
                    return;
                }

                bool takeFirst = true;
                // enumerate all the pictures the user selected
                foreach (object wiaObj in wiaPics)
                {
                    if (takeFirst)
                    {
                        // remove previous picture
                        DisposeImage();
                        wiaItem = (WIALib.ItemClass)Marshal.CreateWrapperOfType(wiaObj, typeof(WIALib.ItemClass));
                        // create temporary file for image
                        fileName = Path.GetTempFileName();
                        Cursor.Current = Cursors.WaitCursor;
                        FileInfo temp = new FileInfo(fileName);
                        path = Application.StartupPath + "\\" + temp.Name;
                        this.Refresh();
                        // transfer picture to application startup path
                        wiaItem.Transfer(path, false);
                        pbPhoto.Image = Image.FromFile(path);

                        ImageCodecInfo[] Info = ImageCodecInfo.GetImageEncoders();
                        EncoderParameters Params = new EncoderParameters(1);
                        Params.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                        if (Info[1].MimeType == "image/jpeg")
                        {
                            pbPhoto.Image.Save(temp.FullName, Info[1], Params);
                        }
                        else
                        {
                            //   //Response.ContentType = Info[1].MimeType;
                            //   //thumbnail.Save(Response.OutputStream, Info[1], Params);

                            pbPhoto.Image.Save(temp.FullName, ImageFormat.Jpeg);
                        }
                        Params.Dispose();
                        //pbPhoto.Image.Save(temp.FullName, ImageFormat.Jpeg);
                        pbPhoto.SizeMode = PictureBoxSizeMode.CenterImage;
                        txtFileName.Text = CustomerID + "_" + System.DateTime.Today.ToShortDateString().Replace("/", "_") + ".jpg";// String.Empty;

                        //DisposeImage();
                        fileName = path;
                        temp.Delete();
                        takeFirst = false;

                        btnSavePicture.Enabled = true;
                    }
                    Marshal.ReleaseComObject(wiaObj);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Acquire from WIA Imaging failed\r\n" + ex.Message, "WIA", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            finally
            {
                if (wiaItem != null)
                    Marshal.ReleaseComObject(wiaItem);      // release WIA image COM object
                if (wiaPics != null)
                    Marshal.ReleaseComObject(wiaPics);      // release WIA collection COM object
                if (wiaRoot != null)
                    Marshal.ReleaseComObject(wiaRoot);      // release WIA root device COM object
                if (wiaDevs != null)
                    Marshal.ReleaseComObject(wiaDevs);      // release WIA devices collection COM object
                if (wiaManager != null)
                    Marshal.ReleaseComObject(wiaManager);       // release WIA manager COM object
                Cursor.Current = Cursors.Default;               // restore cursor
            }
        }

        private void BasicCustomerDetails_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CustidSelected != null)
            {
                CustidSelected(this, new RecordIDEventArgs<StoreCardCustDetails>(new StoreCardCustDetails() { Custid = _custid, Title = sc_title, FirstName = firstName, LastName = Name }));
            }
        }

        private void tcDetails_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                Function = "tcDetails_SelectionChanged";
                Wait();

                if (tcDetails.SelectedTab.Name == "tpPhoto")
                {
                    dgvPreviousPhotos.Visible = false;
                    if (CustomerID == String.Empty)
                    {
                        //Disable buttons
                        EnablePhotoButtons(false);
                    }
                    else
                    {
                        if (customerSaved == true || existingCustomer == true)
                        {
                            //Enable buttons
                            EnablePhotoButtons(true);
                        }
                        else
                        {
                            //Disable buttons
                            EnablePhotoButtons(false);
                        }
                    }
                    //Get and load customer photo if one exists

                    fileName = CustomerManager.GetCustomerPhoto(CustomerID, out Error);
                    if (fileName != String.Empty)
                    {
                        LoadImageHolder();
                    }
                    else
                    {
                        //ShowInfo("M_NOPHOTO", MessageBoxButtons.OK);
                    }

                    signatureFileName = CustomerManager.GetCustomerSignature(CustomerID, out Error);
                    if (signatureFileName != String.Empty)
                    {
                        LoadSignatureHolder(signatureFileName);
                    }
                    else
                    {
                        //ShowInfo("M_NOSIGNATURE", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    if (path != string.Empty)
                    {
                        ReleaseFiles();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == GetResource("M_INVALIDPHOTODIRECTORY"))
                {
                    try
                    {
                        ShowInfo("M_INVALIDPHOTODIRECTORY");
                    }
                    catch
                    {

                    }
                }
                else
                {
                    Catch(ex, Function);
                }
            }
            finally
            {
                StopWait();
                Function = "End of tcDetails_SelectionChanged";
            }
        }

        private void EnablePhotoButtons(bool status)
        {
            btnPrevious.Enabled = status && Credential.HasPermission(CosacsPermissionEnum.CustDetailsPhotoPrevious);
            btnTakePicture.Enabled = status;
            btnAddPicture.Enabled = status;
            btnAddSignature.Enabled = status;
            btnSavePicture.Enabled = false;
            btnSaveSignature.Enabled = false;
        }

        private void btnAddPicture_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "btnAddPicture_Click";
                Wait();

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = @"";
                //openFileDialog.Filter = "JPEG Images (*.jpg,*.jpeg)|*.jpg*.jpeg|Gif Images (*.gif)|*.gif|Bitmaps (*.bmp)|*.bmp";
                openFileDialog.Filter = "JPEG Images (*.jpg,*.jpeg)|*.jpg; *.jpeg";
                openFileDialog.FilterIndex = 1;
                Stream stm;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if ((stm = openFileDialog.OpenFile()) != null)
                    {
                        if (path != String.Empty && pbPhoto.Image != null)
                        {
                            pbPhoto.Image.Dispose();
                            File.Delete(path);
                        }
                        photoPath = openFileDialog.FileName;
                        FileInfo localFile = new FileInfo(photoPath);
                        stm.Close();
                        path = Application.StartupPath + "\\" + localFile.Name;
                        File.Copy(photoPath, path, true);
                        pbPhoto.Image = Image.FromFile(path);
                        //txtFileName.Text = localFile.Name;
                        txtFileName.Text = CustomerID + "_" + System.DateTime.Today.ToShortDateString().Replace("/", "_") + ".jpg";

                        fileName = localFile.Name;
                        //blnPicLoaded = true;

                        btnSavePicture.Enabled = true;
                    }
                }
                openFileDialog.Dispose();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of btnAddPicture_Click";
            }

        }

        private void btnAddSignature_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "btnAddSignature_Click";
                Wait();

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = @"";
                openFileDialog.Filter = "JPEG Images (*.jpg,*.jpeg)|*.jpg;*.jpeg";
                //"JPEG Images (*.jpg,*.jpeg)|*.jpg;*.jpeg|Gif Images (*.gif)|*.gif|Bitmaps (*.bmp)|*.bmp";
                //openFileDialog.Filter = "Bitmaps (*.bmp)|*.bmp";
                openFileDialog.FilterIndex = 1;
                Stream stm;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if ((stm = openFileDialog.OpenFile()) != null)
                    {
                        if (localSignaturePath != String.Empty && pbSignature.Image != null)
                        {
                            pbSignature.Image.Dispose();
                            File.Delete(localSignaturePath);
                        }
                        signaturePath = openFileDialog.FileName;
                        FileInfo localFile = new FileInfo(signaturePath);
                        stm.Close();
                        localSignaturePath = Application.StartupPath + "\\" + localFile.Name;
                        File.Copy(signaturePath, localSignaturePath, true);

                        pbSignature.Image = Image.FromFile(localSignaturePath);
                        //txtSignature.Text = localFile.Name;
                        txtSignature.Text = CustomerID + "_SIG_" + System.DateTime.Today.ToShortDateString().Replace("/", "_") + ".jpg";
                        signatureFileName = localFile.Name;

                        btnSaveSignature.Enabled = true;
                    }
                }
                openFileDialog.Dispose();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of btnAddSignature_Click";
            }

        }

        private void btnSavePicture_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "btnSavePicture_Click";
                Wait();

                bool valid = ValidateFileName(txtFileName);
                string photoFileName = String.Empty;

                if (!txtFileName.Text.Trim().ToLower().EndsWith(".jpg"))
                {
                    photoFileName = txtFileName.Text.Trim() + ".jpg";
                }
                else
                {
                    photoFileName = txtFileName.Text.Trim();
                }

                if ((string)Country[CountryParameterNames.PhotoDirectory] != String.Empty)
                {
                    if (pbPhoto.Image != null && valid == true)
                    {
                        if (CustomerManager.SaveCustomerPhoto(CustomerID, photoFileName, Credential.UserId, out Error))
                        {
                            ShowInfo("M_FILEEXISTS");
                        }
                        else
                        {
                            FileInfo localFile = new FileInfo(fileName);

                            string localPath = Application.StartupPath + "\\" + localFile.Name;
                            if (localPath != string.Empty)
                            {
                                pbPhoto.Image.Dispose();
                            }
                            //If file has been renamed in text box then use new name
                            if (localFile.Name != photoFileName)
                            {
                                try
                                {
                                    FileInfo localFileRenamed = new FileInfo(photoFileName);
                                    path = Application.StartupPath + "\\" + localFileRenamed.Name;
                                    //localFile.MoveTo(localFileRenamed.FullName);
                                    File.Copy(localPath, path, true);
                                    UploadPhoto(localFileRenamed.Name, path, "p");
                                    File.Delete(localPath);

                                    pbPhoto.Image = null;
                                    pbPhoto.Image = Image.FromFile(path);
                                    fileName = path;
                                }
                                catch (Exception ex)
                                {
                                    pbPhoto.Image.Dispose();
                                    pbPhoto.Image = null;

                                    if (ex.Message == GetResource("M_INVALIDPHOTODIRECTORY"))
                                    {
                                        ShowInfo("M_INVALIDPHOTODIRECTORY");
                                    }
                                    else
                                    {
                                        throw ex;
                                    }
                                }
                            }
                            else
                            {
                                UploadPhoto(localFile.Name, localFile.FullName, "p");
                                pbPhoto.Image.Dispose();
                                pbPhoto.Image = null;
                                //Load image from local path
                                pbPhoto.Image = Image.FromFile(localPath);
                                fileName = localPath;
                            }

                            ShowInfo("M_PHOTOSAVED");
                            btnSavePicture.Enabled = false;

                            if (photoPath != String.Empty)
                            {
                                try
                                {
                                    File.Delete(photoPath);
                                }
                                catch
                                {
                                    //Try to delete the original file
                                }
                            }

                            btnPrevious.Enabled = Credential.HasPermission(CosacsPermissionEnum.CustDetailsPhotoPrevious);
                        }
                    }
                }
                else
                {
                    ShowInfo("M_INVALIDPHOTODIRECTORY");
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of btnSavePicture_Click";
            }

        }

        private bool ValidateFileName(TextBox tb)
        {
            bool valid = true;
            if (tb.Text.Length == 0 || tb.Text.Trim().ToLower().EndsWith("*.jpg"))
            {
                valid = false;
                errorProvider1.SetError(tb, GetResource("M_ENTERMANDATORY"));
            }
            else
            {
                errorProvider1.SetError(tb, String.Empty);
            }
            return valid;
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "btnPrevious_Click";
                dgvPreviousPhotos.Visible = true;

                dsPhotos = CustomerManager.GetAllCustomerPhotos(CustomerID, out Error);
                dgvPreviousPhotos.DataSource = dsPhotos.Tables[0];
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void LoadImageHolder()
        {
            try
            {
                string passedPath = String.Empty;
                if (LoadPhoto(fileName, "p", out passedPath))
                {
                    pbPhoto.Image = Image.FromFile(passedPath);
                    pbPhoto.SizeMode = PictureBoxSizeMode.CenterImage;
                    path = passedPath;
                    txtFileName.Text = fileName;
                }
                else //else, do nothing...fantastic
                {
                    //if (fileName == String.Empty)
                    //{
                    //ShowInfo("M_NOPHOTO", MessageBoxButtons.OK);
                    //}
                    //else
                    //{
                    //   ShowInfo("M_INVALIDPHOTODIRECTORY", MessageBoxButtons.OK);
                    //}
                }
            }
            catch
            {
                throw;
            }
        }

        private void LoadSignatureHolder(string signatureFileName)
        {
            try
            {
                string passedPath = String.Empty;
                if (LoadPhoto(signatureFileName, "s", out passedPath))
                {
                    pbSignature.Image = Image.FromFile(passedPath);
                    //pbSignature.SizeMode = PictureBoxSizeMode.CenterImage;
                    localSignaturePath = passedPath;
                    txtSignature.Text = signatureFileName;
                }
                else
                {
                    //if (fileName == String.Empty)
                    //{
                    //ShowInfo("M_NOSIGNATURE", MessageBoxButtons.OK);
                    //}
                    //else
                    //{
                    //   ShowInfo("M_INVALIDPHOTODIRECTORY", MessageBoxButtons.OK);
                    //}
                }
            }
            catch
            {
                throw; //if you just throw why catch the exception?
            }
        }

        private void dgvPreviousPhotos_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Function = "dgvPreviousPhotos_RowEnter";
                Wait();

                DisposeImage();
                fileName = dgvPreviousPhotos[0, e.RowIndex].Value.ToString();
                LoadImageHolder();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of dgvPreviousPhotos_RowEnter";
            }
        }

        private void DisposeImage()
        {
            Image oldImg = pbPhoto.Image;
            pbPhoto.Image = null;
            // dispose old image (free memory, unlock file)
            if (oldImg != null)
            {
                oldImg.Dispose();
            }

            if (fileName != null)
            {
                // try to delete the temporary image file
                try
                {
                    File.Delete(Application.StartupPath + "\\" + fileName);
                }
                catch (Exception)
                { }
            }
        }

        private void btnSaveSignature_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "btnSaveSignature_Click";
                Wait();

                //Save the signature file to the server directory
                bool valid = ValidateFileName(txtSignature);
                string sigFileName = string.Empty;

                if (!txtSignature.Text.Trim().ToLower().EndsWith(".jpg"))
                {
                    sigFileName = txtSignature.Text.Trim() + ".jpg";
                }
                else
                {
                    sigFileName = txtSignature.Text.Trim();
                }

                if ((string)Country[CountryParameterNames.SignatureDirectory] != String.Empty)
                {
                    if (pbSignature.Image != null && valid == true)
                    {
                        if (CustomerManager.SaveCustomerSignature(CustomerID, sigFileName, out Error))
                        {
                            ShowInfo("M_FILEEXISTS");
                        }
                        else
                        {
                            FileInfo localFile = new FileInfo(signatureFileName);
                            string localPath = Application.StartupPath + "\\" + localFile.Name;

                            if (localPath != string.Empty)
                            {
                                pbSignature.Image.Dispose();
                            }
                            //If file has been renamed in text box then use new name
                            if (signatureFileName != sigFileName)
                            {
                                try
                                {
                                    FileInfo localFileRenamed = new FileInfo(sigFileName);
                                    path = Application.StartupPath + "\\" + localFileRenamed.Name;
                                    //localFile.MoveTo(localFileRenamed.FullName);
                                    File.Copy(localPath, path, true);
                                    UploadPhoto(localFileRenamed.Name, path, "s");
                                    File.Delete(localPath);

                                    pbSignature.Image = null;
                                    pbSignature.Image = Image.FromFile(path);
                                }
                                catch (Exception ex)
                                {
                                    if (ex.Message == GetResource("M_INVALIDPHOTODIRECTORY"))
                                    {
                                        ShowInfo("M_INVALIDPHOTODIRECTORY");
                                    }
                                    else
                                    {
                                        throw ex;
                                    }
                                }
                            }
                            else
                            {
                                UploadPhoto(localFile.Name, localFile.FullName, "s");
                                pbSignature.Image.Dispose();
                                pbSignature.Image = null;
                                //Load image from local path
                                pbSignature.Image = Image.FromFile(localPath);
                            }

                            if (signaturePath != String.Empty)
                            {
                                try
                                {
                                    FileStream fs = new FileStream(signaturePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                                    fs.Close();
                                    fs.Dispose();

                                    File.Delete(signaturePath);
                                }
                                catch
                                {
                                    //Try to delete the original file
                                }
                            }
                            ShowInfo("M_SIGNATURESAVED");
                            btnSaveSignature.Enabled = false;
                        }
                    }
                }
                else
                {
                    ShowInfo("M_INVALIDPHOTODIRECTORY");
                }
                //blnPicLoaded = true;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of btnSaveSignature_Click";
            }
        }

        private DataSet SaveAllTabs(string custid)
        {


            DataSet ds = SaveEmployment(custid);
            ds.Merge(SaveEmployment(custid));

            //Only save residential and financial data in certain circumstances...the rest of the time, is like "who cares lets just pretend that we save it"
            if (this.ShouldAddtionalDetailsSave())
            {
                if (tcDetails.TabPages.IndexOf(tpResidential) >= 0)
                    ds.Merge(SaveResidentialData());

                if (tcDetails.TabPages.IndexOf(tpFinancial) >= 0)
                    ds.Merge(SaveFinancialData());
            }

            return ds;
        }

        /// <summary>
        /// Event required to prevent commas being used as part of a name - 69471...what about if you use the keydown the cancel the comma?
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtLastName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Function = "txtLastName_TextChanged";
                Wait();

                TextBox tb = sender as TextBox;//what is this???? really...OMG

                if (tb.Text.Contains(","))//why do you have to check if contain, just try to replace it, if it has no comma it will replace nothing
                {
                    ShowInfo("M_ACCOUNTNAMEVALIDATION", MessageBoxButtons.OK);
                    tb.Text = tb.Text.Replace(",", " ");
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of txtLastName_TextChanged";
            }
        }

        private void radAddress_CheckedChanged(object sender, EventArgs e)
        {
            radTelephoneHistory.Checked = radAddress.Checked;
            radTelephoneAudit.Checked = !radAddress.Checked;
            LoadAddressHistory();
        }

        private void radTelephoneAudit_CheckedChanged(object sender, EventArgs e)
        {
            radAddress.Checked = radTelephoneHistory.Checked;
            radAddressAudit.Checked = !radTelephoneHistory.Checked;
            LoadAddressHistory();
        }

        private void btnCreateStoreCard_Click(object sender, EventArgs e)
        {
            try
            {

                if (customerBlacklisted == false) //IP - 01/09/09 - 5.2 (823) 
                {
                    if (VerifyCustomerAge())
                    {
                        short branchNo = Convert.ToInt16(drpBranch.Text);

                        Function = "btnCreateStoreAccount_Click";
                        Wait();
                        if (Save() || this._hasdatachanged == false)
                        {
                            if (VerifyCustomerAge())
                            {

                                Wait();
                                Client.Call(new CheckQualifyRequest { custid = CustomerID }, response =>
                                    { QualifyReponse(response.qualified); }, this);

                            }

                        }
                    }
                }
                else
                {
                    ShowInfo("M_CUSTOMERBLACKLISTED"); //IP - 01/09/09 - 5.2 (823) 
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of btnCreateStoreAccount_Click";
            }

        }

        private void QualifyReponse(bool Qualified)
        {
            if (Qualified)
            {
                this.CreateCustomerAccount(AT.StoreCard);
                this._hasdatachanged = true;

            }
            else
                Error = "Customer Does not Qualify for Store Card or Credit Blocked";

            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                this.Relationship = "H";        //UAT85 jec 20/10/10

            }
            StopWait();
        }

        private void dgAccounts_Click(object sender, EventArgs e)
        {

        }

        private void dgAccounts_CurrentCellChanged(object sender, EventArgs e)
        {
            
        }

        //IP - 21/12/10 - Store Card
        public void SetStoreCardFields(bool storeCardExists)
        {

            if (storeCardExists)
            {
                txtStoreCardAvailable.Visible = true;
                lblStoreCardAvailable.Visible = true;

                txtStoreCardAvailable.Text = Convert.ToDecimal(customer.StoreCardAvailable).ToString(DecimalPlaces);
                txtStoreCardAvailable.BackColor = Color.RosyBrown;
            }
            else
            {
                txtStoreCardAvailable.Visible = false;
                lblStoreCardAvailable.Visible = false;
            }

        }

        private void txtStoreCardAvailable_MouseHover(object sender, EventArgs e)
        {
            ToolTip Ttip = new ToolTip();

            Ttip.SetToolTip(txtStoreCardAvailable, "StoreCard Limit:" + Convert.ToDecimal(customer.StoreCardLimit).ToString(DecimalPlaces));
        }

        private void dgAccounts_DataSourceChanged(object sender, EventArgs e)
        {
            if (imgStoreCardGrey != null)
                StorecardButtonCheckEnabled();
        }

        private void StorecardButtonCheckEnabled()
        {
            if (customer.StoreCardApproved == true)
                pictureStoreCardApproved.Visible = true;
            else
                pictureStoreCardApproved.Visible = false;

            if (dgAccounts.DataSource != null)
            {
                DataView accounts = (DataView)dgAccounts.DataSource;

                var viewEnumerator = accounts.GetEnumerator();

                while (viewEnumerator.MoveNext())
                {
                    var dataRowView = (DataRowView)viewEnumerator.Current;

                    if (Convert.ToString(dataRowView["Type"]) == AT.StoreCard)
                    {
                        storeCard.AcctNo = Convert.ToString(dataRowView["AccountNumber"]);

                    }
                }
                StoreCardChecks Check = new StoreCardChecks();
                ////There are is a Store Card account in the list
                if (!Check.allowNewStoreCardAccount(storeCard, customer))      // #13655 && CreateStore)
                {
                    btnCreateStoreCard.Enabled = false;
                    existingStoreCard = true;
                    btnCreateStoreCard.BackgroundImage = imgStoreCardGrey;
                    SetStoreCardFields(existingStoreCard);
                }
                else
                if (CreateStore)        // #13655
                {
                    btnCreateStoreCard.Enabled = true;
                    existingStoreCard = false;
                    btnCreateStoreCard.BackgroundImage = imgStoreCardNormal;
                }
            }
        }
        /// <summary>
        /// Converts any image to a grey horrible thing that users wont want to click on
        /// </summary>
        private void ConvertStoreCardImagetoGrey()
        {

            imgStoreCardGrey = btnCreateStoreCard.BackgroundImage;
            Bitmap bm = new Bitmap(imgStoreCardGrey.Width, imgStoreCardGrey.Height);
            Graphics g = Graphics.FromImage(bm);

            ColorMatrix cm = new ColorMatrix(new float[][]{   new float[]{0.5f,0.5f,0.5f,0,0},
                                  new float[]{0.5f,0.5f,0.5f,0,0},
                                  new float[]{0.5f,0.5f,0.5f,0,0},
                                  new float[]{0,0,0,1,0,0},
                                  new float[]{0,0,0,0,1,0},
                                  new float[]{0,0,0,0,0,1}});


            ImageAttributes ia = new ImageAttributes();
            ia.SetColorMatrix(cm);
            g.DrawImage(imgStoreCardGrey, new Rectangle(0, 0, imgStoreCardGrey.Width, imgStoreCardGrey.Height),
                0, 0, imgStoreCardGrey.Width, imgStoreCardGrey.Height, GraphicsUnit.Pixel, ia);
            g.Dispose();
            imgStoreCardGrey = bm;
        }

        private void DimCreditFieldsIfService()
        {
            if (FormParent.Text == "Service Request")
            {
                drpMaritalStat1.Enabled = false;
                dtDOB.Enabled = false;
                txtDependants.Enabled = false;
                drpNationality1.Enabled = false;
            } // not doing else for now as only called when screen is opened
        }


        //IP - 16/03/11 - #3317 - CR1245 - display a popup to alert the user to customers account(s) being in arrears
        private void CheckForAcctsInArrears(DataView dv)
        {

            var acctNoInArrears = string.Empty;
            var countAcctsInArrs = 0;
            StringBuilder sb = new StringBuilder();

            foreach (DataRowView r in dv)
            {
                if (Convert.ToDecimal(r[CN.Arrears]) > 0)
                {
                    countAcctsInArrs = countAcctsInArrs + 1;
                    if (countAcctsInArrs == 1)
                    {
                        acctNoInArrears = Convert.ToString(r[CN.AccountNumber2]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (countAcctsInArrs > 1)
            {
                sb.Append("Accounts are in arrears, please inform the customer");
            }

            else if (countAcctsInArrs == 1)
            {
                sb.Append("Account number ");
                sb.Append(acctNoInArrears);
                sb.Append(" ");
                sb.Append("is in arrears, please inform the customer");
            }

            if (countAcctsInArrs > 0)
            {
                MessageBox.Show(Convert.ToString(sb), "Customers accounts in arrears", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        //jec #4119 28/06/11
        private bool LockAccount()
        {
            accountLocked = AccountManager.LockAccount(this.AccountNo, Credential.UserId.ToString(), out Error);
            if (Error.Length > 0)
            {
                ShowError(Error);
                accountLocked = true;
            }
            else
            {
                AccountManager.UnlockAccount(this.AccountNo, Credential.UserId, out Error);
                accountLocked = false;
            }

            return accountLocked;
        }

        //IP - 21/10/11 - #8425 - CR1232 - Method to unblock Cash Loan
        private void menuCashLoanOveride_Click(object sender, EventArgs e)
        {

            try
            {
                if (!txtCustID.IsBlank(true))
                {

                    //if (cashLoanBlocked == CashLoanBlockedStatus.Blocked)               //IP - 03/11/11 - CR1232 - Only if Cash Loan is blocked then proceed to unblock
                    //{
                    //    Client.Call(new UpdateCashLoanBlockedRequest
                    //    {
                    //        CustId = txtCustID.Text,
                    //        BlockedStatus = CashLoanBlockedStatus.UnBlocked
                    //    },
                    //                   response =>
                    //                   {
                    //                   });

                    //    ((MainForm)this.FormRoot).statusBar1.Text = "Customers Cash Loan has been unblocked";
                    //}
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "menuCashLoanOveride_Click");
            }
        }


        //#14065 - Check Cash Loan Qualification
        private void CashLoanCheckQual(string custId)       // #14429
        {

            Client.Call(new CheckLoanQualificationRequest
            {
                CustId = custId,        // #14429
                Branch = Convert.ToInt32(Config.BranchCode)
            },
                response =>
                {

                    if (response.LoanQualified == "Y")
                    {

                        lblCashLoan.Visible = true;

                    }
                    else
                    {
                        lblCashLoan.Visible = false;

                    }


                },
                this);

        }

        private void radAddressAudit_CheckedChanged(object sender, EventArgs e)
        {

        }

        private decimal GetDependentSpendFactor()
        {
            try
            {
                string Error = string.Empty;
                decimal depSpendFactor = CreditManager.GetDependentSpendFactor(DependantsFromProposal, out Error);

                if (Error.Length > 0)
                {
                    ShowError(Error);
                }

                return depSpendFactor;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        private decimal GetApplicantSpendFactor()
        {
            try
            {
                string monthlyIncome = "0";
                string Error = string.Empty;
                float Income = 0, AddIncome = 0;

                float.TryParse(txtNetIncome1.Text, out Income);
                float.TryParse(txtAddIncome1.Text, out AddIncome);
                monthlyIncome = (Income + AddIncome).ToString();

                decimal appSpendFactor = CreditManager.GetApplicantSpendFactor(monthlyIncome, out Error);

                if (Error.Length > 0)
                {
                    ShowError(Error);
                }

                return appSpendFactor;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

    }


}
