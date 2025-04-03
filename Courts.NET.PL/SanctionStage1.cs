using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using STL.Common;
using STL.Common.Constants;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.SanctionStages;
using STL.Common.Constants.ScreenModes;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using STL.PL.Utils;
using Blue.Cosacs.Shared.Services.Credit;
using Blue.Cosacs.Shared.Services;

namespace STL.PL
{
    /// <summary>
    /// When a customer opens a hire purchase or Ready Finanance account
    /// additional customer details are entered to check the customer is
    /// credit worthy. These details are split into a number of stages.
    /// Sanction Stage One collects the following details: personal; 
    /// residential; employment, financial and the category of the most
    /// expensive goods being sold on this account.
    /// A joint customer can be linked to the credit application. A subset 
    /// of the additional details is entered for the joint customer.
    /// This stage alone is sufficient for the system to score the customer
    /// and either sanction a credit limit or decline the customer for credit.
    /// Subsequent stages gather information that can be useful to follow up
    /// an account that goes into arrears. All stages have to be completed
    /// before the goods can be authorised for delivery.
    /// </summary>
    public class SanctionStage1 : CommonForm
    {
        private bool _suspendEvents = true;
        public bool SuspendEvents
        {
            get { return _suspendEvents; }
            set { _suspendEvents = value; }
        }

        // 5.0.0 UAT338 show/hide sanction flags
        private bool _preventHideSanctionStatus;
        private bool _Reopening;

        private new string Error = String.Empty;
        private bool _complete = false;
        private bool _display = false;
        private Hashtable inputFields = null;
        private DataTable MandatoryFields = null;
        private Hashtable mandatoryFields = null;
        private Hashtable visibleFields = null;
        private Hashtable inError = null;
        private DataSet prop = null;
        private DataSet prop2 = null;
        private bool _mandateExists = false;
        private int _lastDrpPayByGiro1 = -1;
        //private Control app2Tab; // = null;
        //private Control accountsTab = null;
        private string _firstAccountNo = String.Empty;
        private string RFCategory = String.Empty;
        private DataSet mandatoryFieldsDS = null;
        private DataSet dropDownsDS = null;
        private BasicCustomerDetails CustomerScreen = null;
        private string cancellationCode = String.Empty;
        public string Relationship = String.Empty;
        private string ApplicationType = String.Empty;
        private int OldResStatIndex = -1;

        private string _accountNo = String.Empty;
        public string AccountNo
        {
            get { return _accountNo; }
            set { _accountNo = value; }
        }

        private string _custid = String.Empty;
        public string CustomerID
        {
            get { return _custid; }
            set { _custid = value; }
        }

        bool confirm = true;
        public bool Confirm
        {
            get { return confirm; }
            set { confirm = value; }
        }

        private bool _readonly = true;
        public bool ReadOnly
        {
            get { return _readonly; }
            set { _readonly = value; }
        }

        private bool _stageComplete = false;
        public bool Complete
        {
            get { return _stageComplete; }
            set { _stageComplete = value; }
        }

        private bool _acctLocked = false;
        public bool AccountLocked
        {
            get { return _acctLocked; }
            set { _acctLocked = value; }
        }

        private string _acctType = String.Empty;
        public string AccountType
        {
            get { return _acctType; }
            set { _acctType = value; }
        }

        private string _currentStatus = String.Empty;
        public string CurrentStatus
        {
            get { return _currentStatus; }
            set { _currentStatus = value; }
        }


        private string _appstat = "N";
        public string ApplicationStatus
        {
            get { return _appstat; }
            set { _appstat = value; }
        }

        private string _screenMode = SM.Edit;
        public string ScreenMode
        {
            get { return _screenMode; }
            set { _screenMode = value; }
        }

        private bool _settled = false;
        public bool Settled
        {
            get { return _settled; }
            set { _settled = value; }
        }

        private decimal _cashPrice = 0;
        public decimal CashPrice
        {
            get { return _cashPrice; }
            set { _cashPrice = value; }
        }

        //Livewire 69230 new public property required to determine if the 'convert to HP' option should be available in the 'RFCreditRefused' dialog box
        private bool m_allowConversionToHP = false;
        public bool allowConversionToHP
        {
            get { return m_allowConversionToHP; }
            set { m_allowConversionToHP = value; }
        }


        private bool allowReopenS1 = false;         //#10477
        public bool AllowReopenS1
        {
            get { return allowReopenS1; }
            set { allowReopenS1 = value; }
        }

        private string referralReasons = string.Empty;      //IP - 14/03/11 - #3314 - CR1245

        private Crownwood.Magic.Menus.MenuCommand menuManualRefer;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuSanction;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private Crownwood.Magic.Menus.MenuCommand menuComplete;
        private Crownwood.Magic.Menus.MenuCommand menuSave;
        private Crownwood.Magic.Menus.MenuCommand menuReopen;
        private Crownwood.Magic.Menus.MenuCommand menuPrintRFDetails;
        private Crownwood.Magic.Controls.TabControl tcApplicants;
        private Crownwood.Magic.Controls.TabPage tpApp1;
        private Crownwood.Magic.Controls.TabPage tpApp2;
        private Crownwood.Magic.Controls.TabPage tpAccounts;
        private Crownwood.Magic.Controls.TabPage tpComments;
        private Crownwood.Magic.Controls.TabControl tcApp1;
        private Crownwood.Magic.Controls.TabPage tpPersonal;
        private Crownwood.Magic.Controls.TabPage tpResidential;
        private Crownwood.Magic.Controls.TabPage tpEmployment;
        private Crownwood.Magic.Controls.TabPage tpFinancial;
        private Crownwood.Magic.Controls.TabPage tpRFProducts;
        private Crownwood.Magic.Controls.TabControl tcApp2;
        private Crownwood.Magic.Controls.TabPage tpPersonal2;
        private Crownwood.Magic.Controls.TabPage tpEmployment2;
        private Crownwood.Magic.Controls.TabPage tpFinancial2;

        private STL.PL.DatePicker dtDateInCurrentAddress1;
        private STL.PL.DatePicker dtDateInPrevAddress1;
        private STL.PL.DatePicker dtCurrEmpStart1;
        private STL.PL.DatePicker dtPrevEmpStart1;
        private STL.PL.DatePicker dtBankOpened1;
        private STL.PL.DatePicker dtEmploymentStart2;
        private STL.PL.DatePicker dtBankOpened2;
        private STL.PL.CreditCardNo txtCreditCardNo1;

        private System.Windows.Forms.GroupBox gpCustomer;
        private System.Windows.Forms.TextBox txtCustomerID;
        private System.Windows.Forms.DateTimePicker dtDateProp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.GroupBox gbData;
        private System.Windows.Forms.Label lMandatory;
        private System.Windows.Forms.TextBox textCredit;
        private System.Windows.Forms.Label labelCredit;
        private System.Windows.Forms.Button btnComplete;
        private System.Windows.Forms.ComboBox drpApplicationType;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.PictureBox pictRefer;
        private System.Windows.Forms.PictureBox pictReject;
        private System.Windows.Forms.PictureBox pictAccept;
        private System.Windows.Forms.ImageList menuIcons;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtHolderSAccounts;
        private System.Windows.Forms.TextBox txtHolderCAccounts;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtJointCAccounts;
        private System.Windows.Forms.TextBox txtJointSAccounts;
        private System.Windows.Forms.DataGrid dgAccounts;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox drpIDSelection1;
        private System.Windows.Forms.Label lIDSelection1;
        private System.Windows.Forms.Label lMoreRewardsDate1;
        private System.Windows.Forms.DateTimePicker dtMoreRewardsDate1;
        private System.Windows.Forms.Label lMoreRewards1;
        private System.Windows.Forms.TextBox txtMoreRewards1;
        private System.Windows.Forms.Label lNationality1;
        private System.Windows.Forms.ComboBox drpNationality1;
        private System.Windows.Forms.Label lEthnicGroup1;
        private System.Windows.Forms.ComboBox drpEthnicGroup1;
        private System.Windows.Forms.Label lDOB1;
        private System.Windows.Forms.DateTimePicker dtDOB1;
        private System.Windows.Forms.Label lAge1;
        private System.Windows.Forms.TextBox txtAge1;
        private System.Windows.Forms.Label lMaritalStat1;
        private System.Windows.Forms.ComboBox drpMaritalStat1;
        private System.Windows.Forms.ComboBox drpSex1;
        private System.Windows.Forms.Label lSex1;
        private System.Windows.Forms.Label lDependencies1;
        private System.Windows.Forms.NumericUpDown noDependencies1;
        private System.Windows.Forms.ComboBox drpCurrentResidentialStatus1;
        private System.Windows.Forms.Label lMortgage1;
        private System.Windows.Forms.TextBox txtMortgage1;
        private System.Windows.Forms.Label lPrevResidentialStatus1;
        private System.Windows.Forms.Label lPropertyType1;
        private System.Windows.Forms.ComboBox drpPropertyType1;
        private System.Windows.Forms.Label lCurrentResidentialStatus1;
        private System.Windows.Forms.ComboBox drpPrevResidentialStatus1;
        private System.Windows.Forms.TextBox txtEmpTelNum1;
        private System.Windows.Forms.Label lEmpTelNum1;
        private System.Windows.Forms.Label lEmpTelCode1;
        private System.Windows.Forms.TextBox txtEmpTelCode1;
        private System.Windows.Forms.ComboBox drpPayFrequency1;
        private System.Windows.Forms.Label lPayFrequency1;
        private System.Windows.Forms.ComboBox drpOccupation1;
        private System.Windows.Forms.Label lOccupation1;
        private System.Windows.Forms.ComboBox drpEmploymentStat1;
        private System.Windows.Forms.Label lEmploymentStat1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lTotal1;
        private System.Windows.Forms.TextBox txtTotal1;
        private System.Windows.Forms.Label lOther1;
        private System.Windows.Forms.TextBox txtOther1;
        private System.Windows.Forms.Label lMisc1;
        private System.Windows.Forms.TextBox txtMisc1;
        private System.Windows.Forms.Label lLoans1;
        private System.Windows.Forms.TextBox txtLoans1;
        private System.Windows.Forms.Label lUtilities1;
        private System.Windows.Forms.TextBox txtUtilities1;
        private System.Windows.Forms.ComboBox drpPayByGiro1;
        private System.Windows.Forms.Label lBankAccountName1;
        private System.Windows.Forms.Label lGiroDueDate1;
        private System.Windows.Forms.TextBox txtBankAccountName1;
        private System.Windows.Forms.ComboBox drpGiroDueDate1;
        private System.Windows.Forms.Label lCreditCardNo1;
        private System.Windows.Forms.TextBox txtBankAcctNumber1;
        private System.Windows.Forms.ComboBox drpBankAcctType1;
        private System.Windows.Forms.Label lPayByGiro1;
        private System.Windows.Forms.Label lBankAcctNumber1;
        private System.Windows.Forms.Label lBankAcctType1;
        private System.Windows.Forms.ComboBox drpBank1;
        private System.Windows.Forms.Label lBank1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lAddIncome1;
        private System.Windows.Forms.TextBox txtAddIncome1;
        private System.Windows.Forms.Label lNetIncome1;
        private System.Windows.Forms.TextBox txtNetIncome1;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox txtRFCategory;
        private System.Windows.Forms.TreeView tvRFCategory;
        private System.Windows.Forms.Label lRFCategory;
        private System.Windows.Forms.TextBox txtFirstName2;
        private System.Windows.Forms.TextBox txtAlias2;
        private System.Windows.Forms.TextBox txtLastName2;
        private System.Windows.Forms.ComboBox drpTitle2;
        private System.Windows.Forms.Label lTitle2;
        private System.Windows.Forms.Label lFirstName2;
        private System.Windows.Forms.Label lLastName2;
        private System.Windows.Forms.Label lAlias2;
        private System.Windows.Forms.ComboBox drpIDSelection2;
        private System.Windows.Forms.Label lIDSelection2;
        private System.Windows.Forms.Label lMoreRewardsDate2;
        private System.Windows.Forms.DateTimePicker dtMoreRewardsDate2;
        private System.Windows.Forms.Label lMoreRewards2;
        private System.Windows.Forms.TextBox txtMoreRewards2;
        private System.Windows.Forms.Label lDOB2;
        private System.Windows.Forms.DateTimePicker dtDOB2;
        private System.Windows.Forms.Label lAge2;
        private System.Windows.Forms.TextBox txtAge2;
        private System.Windows.Forms.ComboBox drpSex2;
        private System.Windows.Forms.Label lSex2;
        private System.Windows.Forms.ComboBox drpEmploymentStat2;
        private System.Windows.Forms.Label lEmploymentStat2;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label lAddIncome2;
        private System.Windows.Forms.TextBox txtAddIncome2;
        private System.Windows.Forms.Label lNetIncome2;
        private System.Windows.Forms.TextBox txtNetIncome2;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.RichTextBox txtS1Comment;
        private System.Windows.Forms.RichTextBox txtNewS1Comment;
        private System.Windows.Forms.Label lNewS1Comment;
        private System.Windows.Forms.Label lShowResult;
        private System.Windows.Forms.TextBox txtIncome;
        private System.Windows.Forms.Label lIncome;
        private System.Windows.Forms.Label lOccupation2;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Label lDisposable;
        private System.Windows.Forms.TextBox txtDisposable;
        private System.Windows.Forms.TextBox txtAdditionalExpenditure2;
        private System.Windows.Forms.TextBox txtAdditionalExpenditure1;
        private System.Windows.Forms.Label lAdditionalExpenditure2;
        private System.Windows.Forms.Label lAdditionalExpenditure1;
        private System.Windows.Forms.ComboBox drpPaymentMethod;
        private System.Windows.Forms.Label lPayMethod;
        private NumericUpDown txtDistanceFromStore1;
        private Label lDistanceFromStore1;
        private Label lTransportType1;
        private ComboBox drpTransportType1;
        private Label lJobTitle1;
        private ComboBox drpEductation1;
        private Label lEducation1;
        private Label lIndustry1;
        private Label lOrganisation1;
        private ComboBox txtIndustry1;
        private ComboBox txtOrganisation1;
        private ComboBox txtJobTitle1;
        private System.Windows.Forms.ComboBox drpOccupation2;
        private Label lblPurchaseCashLoan;
        private CheckBox cbPurchaseCashLoan;
        private bool loadingscreen = false;

        // To check if spouse is working
        private Label lblIsSpouseWorking;
        private CheckBox chxSpouseWorking;

        // Income calculation variables
        decimal dependentSpendFactor = 0;
        decimal applicantSpendFactor = 0;
               
        public SanctionStage1(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuSanction });
        }

        public SanctionStage1(bool createHP, string[] parms, string mode, Form root, Form parent,
            BasicCustomerDetails customerScreen)
        {
            //
            // Required for Windows Form Designer support
            //
            FormRoot = root;
            FormParent = parent;
            InitializeComponent();
            CustomerScreen = customerScreen;
            HashMenus();
            this.ApplyRoleRestrictions();
            // Date fields are initialised to today
            this.SetDateFrom(true, DateTime.Today);

            // DSR 3 Dec 2002 - UAT fixes M34 and M35
            // Previous address / employment will be invisible when
            // current address / employment years >= Country.SanctionMinYears
            this.dtDateInCurrentAddress1.LinkedBias = true;
            this.dtCurrEmpStart1.LinkedBias = true;

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuSanction });
            Function = "SanctionStage1";

            try
            {
                Wait();

                this.CustomerID = parms[0];
                txtCustomerID.Text = this.CustomerID;

                if (parms.Length > 1)
                {
                    //this.Text += " - "+parms[1];
                    this.AccountNo = parms[1];
                    this.AccountType = parms[2];
                }

                switch (mode)
                {
                    case SM.New: ReadOnly = false;
                        break;
                    case SM.Edit: ReadOnly = false;
                        break;
                    case SM.View: ReadOnly = true;
                        break;
                    default:
                        break;
                }
                ScreenMode = mode;
                Confirm = !ReadOnly;
                btnComplete.Enabled = !ReadOnly;
                menuSave.Enabled = menuComplete.Enabled = !ReadOnly;

                if (menuReopen.Enabled)
                {
                    menuReopen.Enabled = ReadOnly;
                    menuPrintRFDetails.Enabled = ReadOnly;
                }

                lMandatory.BackColor = Color.FromArgb(50, SystemColors.Highlight);
                txtRFCategory.BackColor = SystemColors.Window;

                //CR903 tpRFProducts to be viewable for HP accounts but not for storecard application 
                if (AccountType != AT.ReadyFinance && (AccountType == AT.HP && !ReturnScoreHPResult()) || AccountType == AT.StoreCard)
                {
                    if (tcApp1.TabPages.Contains(tpRFProducts))
                    {
                        tcApp1.TabPages.Remove(tpRFProducts);
                    }
                }

                //Livewire 69230 set allowConversionToHP property to determine if the 'convert to HP' option should be available in the 'RFCreditRefused' dialog box
                if (AccountType != AT.StoreCard)
                    allowConversionToHP = createHP;
                else
                    allowConversionToHP = false;

                //IP - 25/07/11 - CR1254 - RI - #4036
                if (Convert.ToBoolean(Country[CountryParameterNames.RIDispCatAsDept]))
                {
                    tpRFProducts.Title = GetResource("T_DEPARTMENT");
                    groupBox6.Text = groupBox6.Text.Replace("Category", GetResource("T_DEPARTMENT"));
                    lRFCategory.Text = lRFCategory.Text.Replace("category", GetResource("T_DEPARTMENT").ToLower());
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SanctionStage1));
            this.gpCustomer = new System.Windows.Forms.GroupBox();
            this.txtCustomerID = new System.Windows.Forms.TextBox();
            this.dtDateProp = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.gbData = new System.Windows.Forms.GroupBox();
            this.tcApplicants = new Crownwood.Magic.Controls.TabControl();
            this.tpApp1 = new Crownwood.Magic.Controls.TabPage();
            this.tcApp1 = new Crownwood.Magic.Controls.TabControl();
            this.tpPersonal = new Crownwood.Magic.Controls.TabPage();
            this.drpIDSelection1 = new System.Windows.Forms.ComboBox();
            this.lIDSelection1 = new System.Windows.Forms.Label();
            this.lMoreRewardsDate1 = new System.Windows.Forms.Label();
            this.dtMoreRewardsDate1 = new System.Windows.Forms.DateTimePicker();
            this.lMoreRewards1 = new System.Windows.Forms.Label();
            this.txtMoreRewards1 = new System.Windows.Forms.TextBox();
            this.lNationality1 = new System.Windows.Forms.Label();
            this.drpNationality1 = new System.Windows.Forms.ComboBox();
            this.lEthnicGroup1 = new System.Windows.Forms.Label();
            this.drpEthnicGroup1 = new System.Windows.Forms.ComboBox();
            this.lDOB1 = new System.Windows.Forms.Label();
            this.dtDOB1 = new System.Windows.Forms.DateTimePicker();
            this.lAge1 = new System.Windows.Forms.Label();
            this.txtAge1 = new System.Windows.Forms.TextBox();
            this.lMaritalStat1 = new System.Windows.Forms.Label();
            this.drpMaritalStat1 = new System.Windows.Forms.ComboBox();
            this.lblIsSpouseWorking = new System.Windows.Forms.Label();
            this.chxSpouseWorking = new System.Windows.Forms.CheckBox();
            this.drpSex1 = new System.Windows.Forms.ComboBox();
            this.lSex1 = new System.Windows.Forms.Label();
            this.lDependencies1 = new System.Windows.Forms.Label();
            this.noDependencies1 = new System.Windows.Forms.NumericUpDown();
            this.tpResidential = new Crownwood.Magic.Controls.TabPage();
            this.lTransportType1 = new System.Windows.Forms.Label();
            this.drpTransportType1 = new System.Windows.Forms.ComboBox();
            this.lDistanceFromStore1 = new System.Windows.Forms.Label();
            this.txtDistanceFromStore1 = new System.Windows.Forms.NumericUpDown();
            this.dtDateInPrevAddress1 = new STL.PL.DatePicker();
            this.dtDateInCurrentAddress1 = new STL.PL.DatePicker();
            this.drpCurrentResidentialStatus1 = new System.Windows.Forms.ComboBox();
            this.lMortgage1 = new System.Windows.Forms.Label();
            this.txtMortgage1 = new System.Windows.Forms.TextBox();
            this.lPrevResidentialStatus1 = new System.Windows.Forms.Label();
            this.lPropertyType1 = new System.Windows.Forms.Label();
            this.drpPropertyType1 = new System.Windows.Forms.ComboBox();
            this.lCurrentResidentialStatus1 = new System.Windows.Forms.Label();
            this.drpPrevResidentialStatus1 = new System.Windows.Forms.ComboBox();
            this.tpEmployment = new Crownwood.Magic.Controls.TabPage();
            this.txtIndustry1 = new System.Windows.Forms.ComboBox();
            this.txtOrganisation1 = new System.Windows.Forms.ComboBox();
            this.txtJobTitle1 = new System.Windows.Forms.ComboBox();
            this.lIndustry1 = new System.Windows.Forms.Label();
            this.lOrganisation1 = new System.Windows.Forms.Label();
            this.lJobTitle1 = new System.Windows.Forms.Label();
            this.drpEductation1 = new System.Windows.Forms.ComboBox();
            this.lEducation1 = new System.Windows.Forms.Label();
            this.lIncome = new System.Windows.Forms.Label();
            this.txtIncome = new System.Windows.Forms.TextBox();
            this.dtPrevEmpStart1 = new STL.PL.DatePicker();
            this.dtCurrEmpStart1 = new STL.PL.DatePicker();
            this.txtEmpTelNum1 = new System.Windows.Forms.TextBox();
            this.lEmpTelNum1 = new System.Windows.Forms.Label();
            this.lEmpTelCode1 = new System.Windows.Forms.Label();
            this.txtEmpTelCode1 = new System.Windows.Forms.TextBox();
            this.drpPayFrequency1 = new System.Windows.Forms.ComboBox();
            this.lPayFrequency1 = new System.Windows.Forms.Label();
            this.drpOccupation1 = new System.Windows.Forms.ComboBox();
            this.lOccupation1 = new System.Windows.Forms.Label();
            this.drpEmploymentStat1 = new System.Windows.Forms.ComboBox();
            this.lEmploymentStat1 = new System.Windows.Forms.Label();
            this.tpFinancial = new Crownwood.Magic.Controls.TabPage();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.lDisposable = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lAddIncome1 = new System.Windows.Forms.Label();
            this.txtAddIncome1 = new System.Windows.Forms.TextBox();
            this.lNetIncome1 = new System.Windows.Forms.Label();
            this.txtNetIncome1 = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lPayMethod = new System.Windows.Forms.Label();
            this.drpPaymentMethod = new System.Windows.Forms.ComboBox();
            this.dtBankOpened1 = new STL.PL.DatePicker();
            this.drpPayByGiro1 = new System.Windows.Forms.ComboBox();
            this.txtCreditCardNo1 = new STL.PL.CreditCardNo();
            this.lBankAccountName1 = new System.Windows.Forms.Label();
            this.lGiroDueDate1 = new System.Windows.Forms.Label();
            this.txtBankAccountName1 = new System.Windows.Forms.TextBox();
            this.drpGiroDueDate1 = new System.Windows.Forms.ComboBox();
            this.lCreditCardNo1 = new System.Windows.Forms.Label();
            this.txtBankAcctNumber1 = new System.Windows.Forms.TextBox();
            this.drpBankAcctType1 = new System.Windows.Forms.ComboBox();
            this.lPayByGiro1 = new System.Windows.Forms.Label();
            this.lBankAcctNumber1 = new System.Windows.Forms.Label();
            this.lBankAcctType1 = new System.Windows.Forms.Label();
            this.drpBank1 = new System.Windows.Forms.ComboBox();
            this.lBank1 = new System.Windows.Forms.Label();
            this.tpRFProducts = new Crownwood.Magic.Controls.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.cbPurchaseCashLoan = new System.Windows.Forms.CheckBox();
            this.lblPurchaseCashLoan = new System.Windows.Forms.Label();
            this.txtRFCategory = new System.Windows.Forms.TextBox();
            this.tvRFCategory = new System.Windows.Forms.TreeView();
            this.lRFCategory = new System.Windows.Forms.Label();
            this.tpApp2 = new Crownwood.Magic.Controls.TabPage();
            this.tcApp2 = new Crownwood.Magic.Controls.TabControl();
            this.tpPersonal2 = new Crownwood.Magic.Controls.TabPage();
            this.txtFirstName2 = new System.Windows.Forms.TextBox();
            this.txtAlias2 = new System.Windows.Forms.TextBox();
            this.txtLastName2 = new System.Windows.Forms.TextBox();
            this.drpTitle2 = new System.Windows.Forms.ComboBox();
            this.lTitle2 = new System.Windows.Forms.Label();
            this.lFirstName2 = new System.Windows.Forms.Label();
            this.lLastName2 = new System.Windows.Forms.Label();
            this.lAlias2 = new System.Windows.Forms.Label();
            this.drpIDSelection2 = new System.Windows.Forms.ComboBox();
            this.lIDSelection2 = new System.Windows.Forms.Label();
            this.lMoreRewardsDate2 = new System.Windows.Forms.Label();
            this.dtMoreRewardsDate2 = new System.Windows.Forms.DateTimePicker();
            this.lMoreRewards2 = new System.Windows.Forms.Label();
            this.txtMoreRewards2 = new System.Windows.Forms.TextBox();
            this.lDOB2 = new System.Windows.Forms.Label();
            this.dtDOB2 = new System.Windows.Forms.DateTimePicker();
            this.lAge2 = new System.Windows.Forms.Label();
            this.txtAge2 = new System.Windows.Forms.TextBox();
            this.drpSex2 = new System.Windows.Forms.ComboBox();
            this.lSex2 = new System.Windows.Forms.Label();
            this.tpEmployment2 = new Crownwood.Magic.Controls.TabPage();
            this.drpOccupation2 = new System.Windows.Forms.ComboBox();
            this.lOccupation2 = new System.Windows.Forms.Label();
            this.dtEmploymentStart2 = new STL.PL.DatePicker();
            this.drpEmploymentStat2 = new System.Windows.Forms.ComboBox();
            this.lEmploymentStat2 = new System.Windows.Forms.Label();
            this.tpFinancial2 = new Crownwood.Magic.Controls.TabPage();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.dtBankOpened2 = new STL.PL.DatePicker();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.lAddIncome2 = new System.Windows.Forms.Label();
            this.txtAddIncome2 = new System.Windows.Forms.TextBox();
            this.lNetIncome2 = new System.Windows.Forms.Label();
            this.txtNetIncome2 = new System.Windows.Forms.TextBox();
            this.tpAccounts = new Crownwood.Magic.Controls.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtHolderSAccounts = new System.Windows.Forms.TextBox();
            this.txtHolderCAccounts = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtJointCAccounts = new System.Windows.Forms.TextBox();
            this.txtJointSAccounts = new System.Windows.Forms.TextBox();
            this.dgAccounts = new System.Windows.Forms.DataGrid();
            this.tpComments = new Crownwood.Magic.Controls.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtNewS1Comment = new System.Windows.Forms.RichTextBox();
            this.txtS1Comment = new System.Windows.Forms.RichTextBox();
            this.lNewS1Comment = new System.Windows.Forms.Label();
            this.pictRefer = new System.Windows.Forms.PictureBox();
            this.pictReject = new System.Windows.Forms.PictureBox();
            this.pictAccept = new System.Windows.Forms.PictureBox();
            this.textCredit = new System.Windows.Forms.TextBox();
            this.labelCredit = new System.Windows.Forms.Label();
            this.btnComplete = new System.Windows.Forms.Button();
            this.menuIcons = new System.Windows.Forms.ImageList(this.components);
            this.drpApplicationType = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.lMandatory = new System.Windows.Forms.Label();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuPrintRFDetails = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSanction = new Crownwood.Magic.Menus.MenuCommand();
            this.menuComplete = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
            this.menuReopen = new Crownwood.Magic.Menus.MenuCommand();
            this.menuManualRefer = new Crownwood.Magic.Menus.MenuCommand();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.lShowResult = new System.Windows.Forms.Label();
            this.txtDisposable = new System.Windows.Forms.TextBox();
            this.gpCustomer.SuspendLayout();
            this.gbData.SuspendLayout();
            this.tcApplicants.SuspendLayout();
            this.tpApp1.SuspendLayout();
            this.tcApp1.SuspendLayout();
            this.tpPersonal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.noDependencies1)).BeginInit();
            this.tpResidential.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDistanceFromStore1)).BeginInit();
            this.tpEmployment.SuspendLayout();
            this.tpFinancial.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tpRFProducts.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tpApp2.SuspendLayout();
            this.tcApp2.SuspendLayout();
            this.tpPersonal2.SuspendLayout();
            this.tpEmployment2.SuspendLayout();
            this.tpFinancial2.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tpAccounts.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
            this.tpComments.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictRefer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictReject)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictAccept)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // gpCustomer
            // 
            this.gpCustomer.BackColor = System.Drawing.SystemColors.Control;
            this.gpCustomer.Controls.Add(this.txtCustomerID);
            this.gpCustomer.Controls.Add(this.dtDateProp);
            this.gpCustomer.Controls.Add(this.label4);
            this.gpCustomer.Controls.Add(this.label3);
            this.gpCustomer.Controls.Add(this.label2);
            this.gpCustomer.Controls.Add(this.label1);
            this.gpCustomer.Controls.Add(this.txtLastName);
            this.gpCustomer.Controls.Add(this.txtFirstName);
            this.gpCustomer.Location = new System.Drawing.Point(8, 0);
            this.gpCustomer.Name = "gpCustomer";
            this.gpCustomer.Size = new System.Drawing.Size(776, 72);
            this.gpCustomer.TabIndex = 0;
            this.gpCustomer.TabStop = false;
            // 
            // txtCustomerID
            // 
            this.txtCustomerID.Location = new System.Drawing.Point(11, 37);
            this.txtCustomerID.MaxLength = 20;
            this.txtCustomerID.Name = "txtCustomerID";
            this.txtCustomerID.Size = new System.Drawing.Size(132, 20);
            this.txtCustomerID.TabIndex = 0;
            this.txtCustomerID.TabStop = false;
            // 
            // dtDateProp
            // 
            this.dtDateProp.CustomFormat = "ddd dd MMM yyyy";
            this.dtDateProp.Enabled = false;
            this.dtDateProp.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDateProp.Location = new System.Drawing.Point(635, 37);
            this.dtDateProp.Name = "dtDateProp";
            this.dtDateProp.Size = new System.Drawing.Size(131, 20);
            this.dtDateProp.TabIndex = 3;
            this.dtDateProp.TabStop = false;
            this.dtDateProp.Tag = "lDOB1";
            this.dtDateProp.Value = new System.DateTime(2002, 5, 21, 0, 0, 0, 0);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(635, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 18);
            this.label4.TabIndex = 0;
            this.label4.Text = "Date of Application:";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(339, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "Last Name:";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(147, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 18);
            this.label2.TabIndex = 0;
            this.label2.Text = "First Name:";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Customer:";
            // 
            // txtLastName
            // 
            this.txtLastName.Location = new System.Drawing.Point(339, 37);
            this.txtLastName.MaxLength = 60;
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(291, 20);
            this.txtLastName.TabIndex = 2;
            this.txtLastName.TabStop = false;
            // 
            // txtFirstName
            // 
            this.txtFirstName.Location = new System.Drawing.Point(147, 37);
            this.txtFirstName.MaxLength = 30;
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(188, 20);
            this.txtFirstName.TabIndex = 1;
            this.txtFirstName.TabStop = false;
            // 
            // gbData
            // 
            this.gbData.BackColor = System.Drawing.SystemColors.Control;
            this.gbData.Controls.Add(this.tcApplicants);
            this.gbData.Controls.Add(this.pictRefer);
            this.gbData.Controls.Add(this.pictReject);
            this.gbData.Controls.Add(this.pictAccept);
            this.gbData.Controls.Add(this.textCredit);
            this.gbData.Controls.Add(this.labelCredit);
            this.gbData.Controls.Add(this.btnComplete);
            this.gbData.Controls.Add(this.drpApplicationType);
            this.gbData.Controls.Add(this.label10);
            this.gbData.Controls.Add(this.lMandatory);
            this.gbData.Location = new System.Drawing.Point(8, 72);
            this.gbData.Name = "gbData";
            this.gbData.Size = new System.Drawing.Size(776, 400);
            this.gbData.TabIndex = 1;
            this.gbData.TabStop = false;
            this.gbData.Text = "Stage 1 Applicant data";
            // 
            // tcApplicants
            // 
            this.tcApplicants.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.tcApplicants.IDEPixelArea = true;
            this.tcApplicants.Location = new System.Drawing.Point(8, 48);
            this.tcApplicants.Name = "tcApplicants";
            this.tcApplicants.PositionTop = true;
            this.tcApplicants.SelectedIndex = 0;
            this.tcApplicants.SelectedTab = this.tpApp1;
            this.tcApplicants.Size = new System.Drawing.Size(760, 344);
            this.tcApplicants.TabIndex = 0;
            this.tcApplicants.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpApp1,
            this.tpApp2,
            this.tpAccounts,
            this.tpComments});
            this.tcApplicants.SelectionChanged += new System.EventHandler(this.tcApplicants_SelectionChanged);
            // 
            // tpApp1
            // 
            this.tpApp1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tpApp1.Controls.Add(this.tcApp1);
            this.tpApp1.Location = new System.Drawing.Point(0, 25);
            this.tpApp1.Name = "tpApp1";
            this.tpApp1.Size = new System.Drawing.Size(760, 319);
            this.tpApp1.TabIndex = 0;
            this.tpApp1.Title = "Applicant 1";
            // 
            // tcApp1
            // 
            this.tcApp1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcApp1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.tcApp1.IDEPixelArea = true;
            this.tcApp1.Location = new System.Drawing.Point(0, 0);
            this.tcApp1.Name = "tcApp1";
            this.tcApp1.PositionTop = true;
            this.tcApp1.SelectedIndex = 0;
            this.tcApp1.SelectedTab = this.tpPersonal;
            this.tcApp1.Size = new System.Drawing.Size(756, 315);
            this.tcApp1.TabIndex = 0;
            this.tcApp1.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpPersonal,
            this.tpResidential,
            this.tpEmployment,
            this.tpFinancial,
            this.tpRFProducts});
            // 
            // tpPersonal
            // 
            this.tpPersonal.Controls.Add(this.drpIDSelection1);
            this.tpPersonal.Controls.Add(this.lIDSelection1);
            this.tpPersonal.Controls.Add(this.lMoreRewardsDate1);
            this.tpPersonal.Controls.Add(this.dtMoreRewardsDate1);
            this.tpPersonal.Controls.Add(this.lMoreRewards1);
            this.tpPersonal.Controls.Add(this.txtMoreRewards1);
            this.tpPersonal.Controls.Add(this.lNationality1);
            this.tpPersonal.Controls.Add(this.drpNationality1);
            this.tpPersonal.Controls.Add(this.lEthnicGroup1);
            this.tpPersonal.Controls.Add(this.drpEthnicGroup1);
            this.tpPersonal.Controls.Add(this.lDOB1);
            this.tpPersonal.Controls.Add(this.dtDOB1);
            this.tpPersonal.Controls.Add(this.lAge1);
            this.tpPersonal.Controls.Add(this.txtAge1);
            this.tpPersonal.Controls.Add(this.lMaritalStat1);
            this.tpPersonal.Controls.Add(this.drpMaritalStat1);
            this.tpPersonal.Controls.Add(this.lblIsSpouseWorking);
            this.tpPersonal.Controls.Add(this.chxSpouseWorking);
            this.tpPersonal.Controls.Add(this.drpSex1);
            this.tpPersonal.Controls.Add(this.lSex1);
            this.tpPersonal.Controls.Add(this.lDependencies1);
            this.tpPersonal.Controls.Add(this.noDependencies1);
            this.tpPersonal.Location = new System.Drawing.Point(0, 25);
            this.tpPersonal.Name = "tpPersonal";
            this.tpPersonal.Size = new System.Drawing.Size(756, 290);
            this.tpPersonal.TabIndex = 0;
            this.tpPersonal.Title = "Personal";
            // 
            // drpIDSelection1
            // 
            this.drpIDSelection1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpIDSelection1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpIDSelection1.ItemHeight = 13;
            this.drpIDSelection1.Location = new System.Drawing.Point(72, 51);
            this.drpIDSelection1.Name = "drpIDSelection1";
            this.drpIDSelection1.Size = new System.Drawing.Size(152, 21);
            this.drpIDSelection1.TabIndex = 0;
            this.drpIDSelection1.Tag = "lIDSelection1";
            this.drpIDSelection1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lIDSelection1
            // 
            this.lIDSelection1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lIDSelection1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lIDSelection1.Location = new System.Drawing.Point(72, 24);
            this.lIDSelection1.Name = "lIDSelection1";
            this.lIDSelection1.Size = new System.Drawing.Size(124, 16);
            this.lIDSelection1.TabIndex = 0;
            this.lIDSelection1.Text = "ID Selection:";
            this.lIDSelection1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lMoreRewardsDate1
            // 
            this.lMoreRewardsDate1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lMoreRewardsDate1.Location = new System.Drawing.Point(468, 219);
            this.lMoreRewardsDate1.Name = "lMoreRewardsDate1";
            this.lMoreRewardsDate1.Size = new System.Drawing.Size(196, 19);
            this.lMoreRewardsDate1.TabIndex = 0;
            this.lMoreRewardsDate1.Text = "Loyalty Card Effective Date:";
            // 
            // dtMoreRewardsDate1
            // 
            this.dtMoreRewardsDate1.CustomFormat = "ddd dd MMM yyyy";
            this.dtMoreRewardsDate1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtMoreRewardsDate1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtMoreRewardsDate1.Location = new System.Drawing.Point(468, 243);
            this.dtMoreRewardsDate1.Name = "dtMoreRewardsDate1";
            this.dtMoreRewardsDate1.Size = new System.Drawing.Size(130, 20);
            this.dtMoreRewardsDate1.TabIndex = 9;
            this.dtMoreRewardsDate1.Tag = "lMoreRewardsDate1";
            this.dtMoreRewardsDate1.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            this.dtMoreRewardsDate1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lMoreRewards1
            // 
            this.lMoreRewards1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lMoreRewards1.Location = new System.Drawing.Point(272, 219);
            this.lMoreRewards1.Name = "lMoreRewards1";
            this.lMoreRewards1.Size = new System.Drawing.Size(122, 19);
            this.lMoreRewards1.TabIndex = 0;
            this.lMoreRewards1.Text = "Loyalty Card No:";
            // 
            // txtMoreRewards1
            // 
            this.txtMoreRewards1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtMoreRewards1.Location = new System.Drawing.Point(272, 243);
            this.txtMoreRewards1.MaxLength = 16;
            this.txtMoreRewards1.Name = "txtMoreRewards1";
            this.txtMoreRewards1.Size = new System.Drawing.Size(117, 20);
            this.txtMoreRewards1.TabIndex = 8;
            this.txtMoreRewards1.Tag = "lMoreRewards1";
            this.txtMoreRewards1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lNationality1
            // 
            this.lNationality1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lNationality1.Location = new System.Drawing.Point(468, 155);
            this.lNationality1.Name = "lNationality1";
            this.lNationality1.Size = new System.Drawing.Size(84, 18);
            this.lNationality1.TabIndex = 0;
            this.lNationality1.Text = "Nationality:";
            // 
            // drpNationality1
            // 
            this.drpNationality1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpNationality1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpNationality1.ItemHeight = 13;
            this.drpNationality1.Location = new System.Drawing.Point(468, 179);
            this.drpNationality1.Name = "drpNationality1";
            this.drpNationality1.Size = new System.Drawing.Size(156, 21);
            this.drpNationality1.TabIndex = 7;
            this.drpNationality1.Tag = "lNationality1";
            this.drpNationality1.SelectedIndexChanged += new System.EventHandler(this.drpNationality1_SelectedIndexChanged);
            this.drpNationality1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lEthnicGroup1
            // 
            this.lEthnicGroup1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lEthnicGroup1.Location = new System.Drawing.Point(272, 155);
            this.lEthnicGroup1.Name = "lEthnicGroup1";
            this.lEthnicGroup1.Size = new System.Drawing.Size(84, 18);
            this.lEthnicGroup1.TabIndex = 0;
            this.lEthnicGroup1.Text = "Ethnic group:";
            // 
            // drpEthnicGroup1
            // 
            this.drpEthnicGroup1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEthnicGroup1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpEthnicGroup1.ItemHeight = 13;
            this.drpEthnicGroup1.Location = new System.Drawing.Point(272, 179);
            this.drpEthnicGroup1.Name = "drpEthnicGroup1";
            this.drpEthnicGroup1.Size = new System.Drawing.Size(144, 21);
            this.drpEthnicGroup1.TabIndex = 6;
            this.drpEthnicGroup1.Tag = "lEthnicGroup1";
            this.drpEthnicGroup1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lDOB1
            // 
            this.lDOB1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lDOB1.BackColor = System.Drawing.Color.Transparent;
            this.lDOB1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lDOB1.Location = new System.Drawing.Point(272, 24);
            this.lDOB1.Name = "lDOB1";
            this.lDOB1.Size = new System.Drawing.Size(96, 19);
            this.lDOB1.TabIndex = 0;
            this.lDOB1.Text = "Date of Birth:";
            // 
            // dtDOB1
            // 
            this.dtDOB1.CustomFormat = "ddd dd MMM yyyy";
            this.dtDOB1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtDOB1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDOB1.Location = new System.Drawing.Point(272, 51);
            this.dtDOB1.Name = "dtDOB1";
            this.dtDOB1.Size = new System.Drawing.Size(131, 20);
            this.dtDOB1.TabIndex = 1;
            this.dtDOB1.Tag = "lDOB1";
            this.dtDOB1.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            this.dtDOB1.ValueChanged += new System.EventHandler(this.dtDOB1_ValueChanged);
            this.dtDOB1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lAge1
            // 
            this.lAge1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lAge1.Location = new System.Drawing.Point(468, 24);
            this.lAge1.Name = "lAge1";
            this.lAge1.Size = new System.Drawing.Size(37, 19);
            this.lAge1.TabIndex = 0;
            this.lAge1.Text = "Age:";
            // 
            // txtAge1
            // 
            this.txtAge1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtAge1.Location = new System.Drawing.Point(468, 51);
            this.txtAge1.MaxLength = 3;
            this.txtAge1.Name = "txtAge1";
            this.txtAge1.ReadOnly = true;
            this.txtAge1.Size = new System.Drawing.Size(46, 20);
            this.txtAge1.TabIndex = 2;
            this.txtAge1.Tag = "lAge1";
            this.txtAge1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAge1_KeyPress);
            this.txtAge1.Leave += new System.EventHandler(this.txtAge1_Leave);
            this.txtAge1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lMaritalStat1
            // 
            this.lMaritalStat1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lMaritalStat1.Location = new System.Drawing.Point(468, 91);
            this.lMaritalStat1.Name = "lMaritalStat1";
            this.lMaritalStat1.Size = new System.Drawing.Size(117, 18);
            this.lMaritalStat1.TabIndex = 0;
            this.lMaritalStat1.Text = "Marital Status:";
            // 
            // drpMaritalStat1
            // 
            this.drpMaritalStat1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpMaritalStat1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpMaritalStat1.ItemHeight = 13;
            this.drpMaritalStat1.Location = new System.Drawing.Point(468, 115);
            this.drpMaritalStat1.Name = "drpMaritalStat1";
            this.drpMaritalStat1.Size = new System.Drawing.Size(80, 21);
            this.drpMaritalStat1.TabIndex = 5;
            this.drpMaritalStat1.Tag = "lMaritalStat1";
            this.drpMaritalStat1.SelectedIndexChanged += new System.EventHandler(this.drpMaritalStat1_SelectedIndexChanged);
            this.drpMaritalStat1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lblIsSpouseWorking
            // 
            this.lblIsSpouseWorking.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lblIsSpouseWorking.Location = new System.Drawing.Point(595, 91);
            this.lblIsSpouseWorking.Name = "lblIsSpouseWorking";
            this.lblIsSpouseWorking.Size = new System.Drawing.Size(120, 18);
            this.lblIsSpouseWorking.TabIndex = 0;
            this.lblIsSpouseWorking.Text = "Is Spouse Working:";
            this.lblIsSpouseWorking.Visible = false;
            // 
            // chxSpouseWorking
            // 
            this.chxSpouseWorking.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.chxSpouseWorking.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chxSpouseWorking.Location = new System.Drawing.Point(625, 115);
            this.chxSpouseWorking.Name = "chxSpouseWorking";
            this.chxSpouseWorking.Size = new System.Drawing.Size(20, 20);
            this.chxSpouseWorking.TabIndex = 0;
            this.chxSpouseWorking.Visible = false;
            this.chxSpouseWorking.UseVisualStyleBackColor = true;
            this.chxSpouseWorking.CheckedChanged += new System.EventHandler(this.chxSpouseWorking_CheckedChanged);
            // 
            // drpSex1
            // 
            this.drpSex1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpSex1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpSex1.ItemHeight = 13;
            this.drpSex1.Location = new System.Drawing.Point(532, 51);
            this.drpSex1.Name = "drpSex1";
            this.drpSex1.Size = new System.Drawing.Size(92, 21);
            this.drpSex1.TabIndex = 3;
            this.drpSex1.Tag = "lSex1";
            this.drpSex1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lSex1
            // 
            this.lSex1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lSex1.Location = new System.Drawing.Point(532, 24);
            this.lSex1.Name = "lSex1";
            this.lSex1.Size = new System.Drawing.Size(52, 19);
            this.lSex1.TabIndex = 0;
            this.lSex1.Text = "Gender:";
            // 
            // lDependencies1
            // 
            this.lDependencies1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lDependencies1.Location = new System.Drawing.Point(272, 91);
            this.lDependencies1.Name = "lDependencies1";
            this.lDependencies1.Size = new System.Drawing.Size(122, 19);
            this.lDependencies1.TabIndex = 0;
            this.lDependencies1.Text = "No. of Dependants:";
            // 
            // noDependencies1
            // 
            this.noDependencies1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.noDependencies1.Location = new System.Drawing.Point(272, 114);
            this.noDependencies1.Name = "noDependencies1";
            this.noDependencies1.Size = new System.Drawing.Size(37, 20);
            this.noDependencies1.TabIndex = 4;
            this.noDependencies1.Tag = "lDependencies1";
            this.noDependencies1.ValueChanged += new System.EventHandler(this.noDependencies1_ValueChanged);
            // 
            // tpResidential
            // 
            this.tpResidential.Controls.Add(this.lTransportType1);
            this.tpResidential.Controls.Add(this.drpTransportType1);
            this.tpResidential.Controls.Add(this.lDistanceFromStore1);
            this.tpResidential.Controls.Add(this.txtDistanceFromStore1);
            this.tpResidential.Controls.Add(this.dtDateInPrevAddress1);
            this.tpResidential.Controls.Add(this.dtDateInCurrentAddress1);
            this.tpResidential.Controls.Add(this.drpCurrentResidentialStatus1);
            this.tpResidential.Controls.Add(this.lMortgage1);
            this.tpResidential.Controls.Add(this.txtMortgage1);
            this.tpResidential.Controls.Add(this.lPrevResidentialStatus1);
            this.tpResidential.Controls.Add(this.lPropertyType1);
            this.tpResidential.Controls.Add(this.drpPropertyType1);
            this.tpResidential.Controls.Add(this.lCurrentResidentialStatus1);
            this.tpResidential.Controls.Add(this.drpPrevResidentialStatus1);
            this.tpResidential.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.tpResidential.Location = new System.Drawing.Point(0, 25);
            this.tpResidential.Name = "tpResidential";
            this.tpResidential.Selected = false;
            this.tpResidential.Size = new System.Drawing.Size(756, 290);
            this.tpResidential.TabIndex = 1;
            this.tpResidential.Title = "Residential";
            this.tpResidential.Enter += new System.EventHandler(this.tpResidential_Enter);
            // 
            // lTransportType1
            // 
            this.lTransportType1.AutoSize = true;
            this.lTransportType1.Location = new System.Drawing.Point(594, 104);
            this.lTransportType1.Name = "lTransportType1";
            this.lTransportType1.Size = new System.Drawing.Size(82, 13);
            this.lTransportType1.TabIndex = 9;
            this.lTransportType1.Text = "Transport Type:";
            // 
            // drpTransportType1
            // 
            this.drpTransportType1.FormattingEnabled = true;
            this.drpTransportType1.Location = new System.Drawing.Point(597, 136);
            this.drpTransportType1.Name = "drpTransportType1";
            this.drpTransportType1.Size = new System.Drawing.Size(121, 21);
            this.drpTransportType1.TabIndex = 8;
            this.drpTransportType1.Tag = "lTransportType1";
            // 
            // lDistanceFromStore1
            // 
            this.lDistanceFromStore1.AutoSize = true;
            this.lDistanceFromStore1.Location = new System.Drawing.Point(449, 104);
            this.lDistanceFromStore1.Name = "lDistanceFromStore1";
            this.lDistanceFromStore1.Size = new System.Drawing.Size(106, 13);
            this.lDistanceFromStore1.TabIndex = 7;
            this.lDistanceFromStore1.Text = "Distance From Store:";
            // 
            // txtDistanceFromStore1
            // 
            this.txtDistanceFromStore1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.txtDistanceFromStore1.Location = new System.Drawing.Point(452, 136);
            this.txtDistanceFromStore1.Name = "txtDistanceFromStore1";
            this.txtDistanceFromStore1.Size = new System.Drawing.Size(85, 20);
            this.txtDistanceFromStore1.TabIndex = 6;
            this.txtDistanceFromStore1.Tag = "lDistanceFromStore1";
            // 
            // dtDateInPrevAddress1
            // 
            this.dtDateInPrevAddress1.DateFrom = new System.DateTime(2006, 10, 9, 0, 0, 0, 0);
            this.dtDateInPrevAddress1.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtDateInPrevAddress1.Label = "Date In prev Address:";
            this.dtDateInPrevAddress1.LinkedBias = false;
            this.dtDateInPrevAddress1.LinkedComboBox = null;
            this.dtDateInPrevAddress1.LinkedDatePicker = null;
            this.dtDateInPrevAddress1.LinkedLabel = null;
            this.dtDateInPrevAddress1.Location = new System.Drawing.Point(70, 192);
            this.dtDateInPrevAddress1.Months = new decimal(new int[] {
            11,
            0,
            0,
            0});
            this.dtDateInPrevAddress1.Name = "dtDateInPrevAddress1";
            this.dtDateInPrevAddress1.Size = new System.Drawing.Size(256, 56);
            this.dtDateInPrevAddress1.TabIndex = 4;
            this.dtDateInPrevAddress1.Tag = "dtDateInPrevAddress1";
            this.dtDateInPrevAddress1.Value = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtDateInPrevAddress1.Years = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.dtDateInPrevAddress1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // dtDateInCurrentAddress1
            // 
            this.dtDateInCurrentAddress1.DateFrom = new System.DateTime(2006, 10, 10, 0, 0, 0, 0);
            this.dtDateInCurrentAddress1.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtDateInCurrentAddress1.Label = "Date In Curr Address:";
            this.dtDateInCurrentAddress1.LinkedBias = true;
            this.dtDateInCurrentAddress1.LinkedComboBox = null;
            this.dtDateInCurrentAddress1.LinkedDatePicker = this.dtDateInPrevAddress1;
            this.dtDateInCurrentAddress1.LinkedLabel = null;
            this.dtDateInCurrentAddress1.Location = new System.Drawing.Point(70, 32);
            this.dtDateInCurrentAddress1.Months = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtDateInCurrentAddress1.Name = "dtDateInCurrentAddress1";
            this.dtDateInCurrentAddress1.Size = new System.Drawing.Size(256, 56);
            this.dtDateInCurrentAddress1.TabIndex = 0;
            this.dtDateInCurrentAddress1.Tag = "dtDateInCurrentAddress1";
            this.dtDateInCurrentAddress1.Value = new System.DateTime(2006, 10, 9, 0, 0, 0, 0);
            this.dtDateInCurrentAddress1.Years = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtDateInCurrentAddress1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // drpCurrentResidentialStatus1
            // 
            this.drpCurrentResidentialStatus1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCurrentResidentialStatus1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpCurrentResidentialStatus1.ItemHeight = 13;
            this.drpCurrentResidentialStatus1.Location = new System.Drawing.Point(452, 56);
            this.drpCurrentResidentialStatus1.Name = "drpCurrentResidentialStatus1";
            this.drpCurrentResidentialStatus1.Size = new System.Drawing.Size(155, 21);
            this.drpCurrentResidentialStatus1.TabIndex = 1;
            this.drpCurrentResidentialStatus1.Tag = "lCurrentResidentialStatus1";
            this.drpCurrentResidentialStatus1.SelectedIndexChanged += new System.EventHandler(this.drpCurrentResidentialStatus1_SelectedIndexChanged);
            this.drpCurrentResidentialStatus1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lMortgage1
            // 
            this.lMortgage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lMortgage1.Location = new System.Drawing.Point(259, 104);
            this.lMortgage1.Name = "lMortgage1";
            this.lMortgage1.Size = new System.Drawing.Size(102, 18);
            this.lMortgage1.TabIndex = 0;
            this.lMortgage1.Text = "Mortgage/Rent:";
            // 
            // txtMortgage1
            // 
            this.txtMortgage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtMortgage1.Location = new System.Drawing.Point(259, 136);
            this.txtMortgage1.Name = "txtMortgage1";
            this.txtMortgage1.Size = new System.Drawing.Size(105, 20);
            this.txtMortgage1.TabIndex = 3;
            this.txtMortgage1.Tag = "lMortgage1";
            this.txtMortgage1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lPrevResidentialStatus1
            // 
            this.lPrevResidentialStatus1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lPrevResidentialStatus1.Location = new System.Drawing.Point(452, 192);
            this.lPrevResidentialStatus1.Name = "lPrevResidentialStatus1";
            this.lPrevResidentialStatus1.Size = new System.Drawing.Size(171, 18);
            this.lPrevResidentialStatus1.TabIndex = 0;
            this.lPrevResidentialStatus1.Text = "Previous Residential Status:";
            // 
            // lPropertyType1
            // 
            this.lPropertyType1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lPropertyType1.Location = new System.Drawing.Point(75, 104);
            this.lPropertyType1.Name = "lPropertyType1";
            this.lPropertyType1.Size = new System.Drawing.Size(116, 18);
            this.lPropertyType1.TabIndex = 0;
            this.lPropertyType1.Text = "Property Type:";
            // 
            // drpPropertyType1
            // 
            this.drpPropertyType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPropertyType1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpPropertyType1.ItemHeight = 13;
            this.drpPropertyType1.Location = new System.Drawing.Point(75, 136);
            this.drpPropertyType1.Name = "drpPropertyType1";
            this.drpPropertyType1.Size = new System.Drawing.Size(155, 21);
            this.drpPropertyType1.TabIndex = 2;
            this.drpPropertyType1.Tag = "lPropertyType1";
            this.drpPropertyType1.SelectedIndexChanged += new System.EventHandler(this.drpPropertyType1_SelectedIndexChanged);
            this.drpPropertyType1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lCurrentResidentialStatus1
            // 
            this.lCurrentResidentialStatus1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lCurrentResidentialStatus1.Location = new System.Drawing.Point(452, 32);
            this.lCurrentResidentialStatus1.Name = "lCurrentResidentialStatus1";
            this.lCurrentResidentialStatus1.Size = new System.Drawing.Size(168, 18);
            this.lCurrentResidentialStatus1.TabIndex = 0;
            this.lCurrentResidentialStatus1.Text = "Current Residential Status:";
            // 
            // drpPrevResidentialStatus1
            // 
            this.drpPrevResidentialStatus1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPrevResidentialStatus1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpPrevResidentialStatus1.ItemHeight = 13;
            this.drpPrevResidentialStatus1.Location = new System.Drawing.Point(452, 216);
            this.drpPrevResidentialStatus1.Name = "drpPrevResidentialStatus1";
            this.drpPrevResidentialStatus1.Size = new System.Drawing.Size(155, 21);
            this.drpPrevResidentialStatus1.TabIndex = 5;
            this.drpPrevResidentialStatus1.Tag = "lPrevResidentialStatus1";
            this.drpPrevResidentialStatus1.SelectedIndexChanged += new System.EventHandler(this.drpPrevResidentialStatus1_SelectedIndexChanged);
            this.drpPrevResidentialStatus1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // tpEmployment
            // 
            this.tpEmployment.Controls.Add(this.txtIndustry1);
            this.tpEmployment.Controls.Add(this.txtOrganisation1);
            this.tpEmployment.Controls.Add(this.txtJobTitle1);
            this.tpEmployment.Controls.Add(this.lIndustry1);
            this.tpEmployment.Controls.Add(this.lOrganisation1);
            this.tpEmployment.Controls.Add(this.lJobTitle1);
            this.tpEmployment.Controls.Add(this.drpEductation1);
            this.tpEmployment.Controls.Add(this.lEducation1);
            this.tpEmployment.Controls.Add(this.lIncome);
            this.tpEmployment.Controls.Add(this.txtIncome);
            this.tpEmployment.Controls.Add(this.dtPrevEmpStart1);
            this.tpEmployment.Controls.Add(this.dtCurrEmpStart1);
            this.tpEmployment.Controls.Add(this.txtEmpTelNum1);
            this.tpEmployment.Controls.Add(this.lEmpTelNum1);
            this.tpEmployment.Controls.Add(this.lEmpTelCode1);
            this.tpEmployment.Controls.Add(this.txtEmpTelCode1);
            this.tpEmployment.Controls.Add(this.drpPayFrequency1);
            this.tpEmployment.Controls.Add(this.lPayFrequency1);
            this.tpEmployment.Controls.Add(this.drpOccupation1);
            this.tpEmployment.Controls.Add(this.lOccupation1);
            this.tpEmployment.Controls.Add(this.drpEmploymentStat1);
            this.tpEmployment.Controls.Add(this.lEmploymentStat1);
            this.tpEmployment.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.tpEmployment.Location = new System.Drawing.Point(0, 25);
            this.tpEmployment.Name = "tpEmployment";
            this.tpEmployment.Selected = false;
            this.tpEmployment.Size = new System.Drawing.Size(756, 290);
            this.tpEmployment.TabIndex = 2;
            this.tpEmployment.Title = "Employment";
            // 
            // txtIndustry1
            // 
            this.txtIndustry1.FormattingEnabled = true;
            this.txtIndustry1.Location = new System.Drawing.Point(455, 241);
            this.txtIndustry1.Name = "txtIndustry1";
            this.txtIndustry1.Size = new System.Drawing.Size(124, 21);
            this.txtIndustry1.TabIndex = 19;
            this.txtIndustry1.Tag = "lIndustry1";
            // 
            // txtOrganisation1
            // 
            this.txtOrganisation1.FormattingEnabled = true;
            this.txtOrganisation1.Location = new System.Drawing.Point(582, 241);
            this.txtOrganisation1.Name = "txtOrganisation1";
            this.txtOrganisation1.Size = new System.Drawing.Size(172, 21);
            this.txtOrganisation1.TabIndex = 18;
            this.txtOrganisation1.Tag = "lOrganisation1";
            // 
            // txtJobTitle1
            // 
            this.txtJobTitle1.FormattingEnabled = true;
            this.txtJobTitle1.Location = new System.Drawing.Point(458, 105);
            this.txtJobTitle1.Name = "txtJobTitle1";
            this.txtJobTitle1.Size = new System.Drawing.Size(186, 21);
            this.txtJobTitle1.TabIndex = 17;
            this.txtJobTitle1.Tag = "lJobTitle1";
            // 
            // lIndustry1
            // 
            this.lIndustry1.AutoSize = true;
            this.lIndustry1.Location = new System.Drawing.Point(452, 216);
            this.lIndustry1.Name = "lIndustry1";
            this.lIndustry1.Size = new System.Drawing.Size(47, 13);
            this.lIndustry1.TabIndex = 14;
            this.lIndustry1.Text = "Industry:";
            // 
            // lOrganisation1
            // 
            this.lOrganisation1.AutoSize = true;
            this.lOrganisation1.Location = new System.Drawing.Point(579, 216);
            this.lOrganisation1.Name = "lOrganisation1";
            this.lOrganisation1.Size = new System.Drawing.Size(69, 13);
            this.lOrganisation1.TabIndex = 13;
            this.lOrganisation1.Text = "Organisation:";
            // 
            // lJobTitle1
            // 
            this.lJobTitle1.AutoSize = true;
            this.lJobTitle1.Location = new System.Drawing.Point(455, 81);
            this.lJobTitle1.Name = "lJobTitle1";
            this.lJobTitle1.Size = new System.Drawing.Size(50, 13);
            this.lJobTitle1.TabIndex = 11;
            this.lJobTitle1.Text = "Job Title:";
            // 
            // drpEductation1
            // 
            this.drpEductation1.FormattingEnabled = true;
            this.drpEductation1.Location = new System.Drawing.Point(458, 44);
            this.drpEductation1.Name = "drpEductation1";
            this.drpEductation1.Size = new System.Drawing.Size(121, 21);
            this.drpEductation1.TabIndex = 10;
            this.drpEductation1.Tag = "lEducation1";
            this.drpEductation1.SelectedIndexChanged += new System.EventHandler(this.drpEductation1_SelectedIndexChanged);
            // 
            // lEducation1
            // 
            this.lEducation1.AutoSize = true;
            this.lEducation1.Location = new System.Drawing.Point(455, 16);
            this.lEducation1.Name = "lEducation1";
            this.lEducation1.Size = new System.Drawing.Size(87, 13);
            this.lEducation1.TabIndex = 9;
            this.lEducation1.Text = "Education Level:";
            // 
            // lIncome
            // 
            this.lIncome.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lIncome.Location = new System.Drawing.Point(278, 145);
            this.lIncome.Name = "lIncome";
            this.lIncome.Size = new System.Drawing.Size(104, 18);
            this.lIncome.TabIndex = 8;
            this.lIncome.Text = "Income:";
            this.lIncome.Visible = false;
            // 
            // txtIncome
            // 
            this.txtIncome.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtIncome.Location = new System.Drawing.Point(278, 169);
            this.txtIncome.MaxLength = 13;
            this.txtIncome.Name = "txtIncome";
            this.txtIncome.Size = new System.Drawing.Size(94, 20);
            this.txtIncome.TabIndex = 4;
            this.txtIncome.Tag = "lIncome";
            this.txtIncome.Visible = false;
            this.txtIncome.Leave += new System.EventHandler(this.txtIncome_Leave);
            this.txtIncome.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // dtPrevEmpStart1
            // 
            this.dtPrevEmpStart1.DateFrom = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtPrevEmpStart1.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtPrevEmpStart1.Label = "Prev. Emp. started:";
            this.dtPrevEmpStart1.LinkedBias = false;
            this.dtPrevEmpStart1.LinkedComboBox = null;
            this.dtPrevEmpStart1.LinkedDatePicker = null;
            this.dtPrevEmpStart1.LinkedLabel = null;
            this.dtPrevEmpStart1.Location = new System.Drawing.Point(78, 216);
            this.dtPrevEmpStart1.Months = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtPrevEmpStart1.Name = "dtPrevEmpStart1";
            this.dtPrevEmpStart1.Size = new System.Drawing.Size(256, 56);
            this.dtPrevEmpStart1.TabIndex = 7;
            this.dtPrevEmpStart1.Tag = "dtPrevEmpStart1";
            this.dtPrevEmpStart1.Value = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtPrevEmpStart1.Years = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtPrevEmpStart1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // dtCurrEmpStart1
            // 
            this.dtCurrEmpStart1.DateFrom = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtCurrEmpStart1.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtCurrEmpStart1.Label = "Curr. Emp. started:";
            this.dtCurrEmpStart1.LinkedBias = false;
            this.dtCurrEmpStart1.LinkedComboBox = null;
            this.dtCurrEmpStart1.LinkedDatePicker = this.dtPrevEmpStart1;
            this.dtCurrEmpStart1.LinkedLabel = null;
            this.dtCurrEmpStart1.Location = new System.Drawing.Point(78, 16);
            this.dtCurrEmpStart1.Months = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtCurrEmpStart1.Name = "dtCurrEmpStart1";
            this.dtCurrEmpStart1.Size = new System.Drawing.Size(256, 56);
            this.dtCurrEmpStart1.TabIndex = 0;
            this.dtCurrEmpStart1.Tag = "dtCurrEmpStart1";
            this.dtCurrEmpStart1.Value = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtCurrEmpStart1.Years = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtCurrEmpStart1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // txtEmpTelNum1
            // 
            this.txtEmpTelNum1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtEmpTelNum1.Location = new System.Drawing.Point(527, 169);
            this.txtEmpTelNum1.MaxLength = 13;
            this.txtEmpTelNum1.Name = "txtEmpTelNum1";
            this.txtEmpTelNum1.Size = new System.Drawing.Size(117, 20);
            this.txtEmpTelNum1.TabIndex = 6;
            this.txtEmpTelNum1.Tag = "lEmpTelNum1";
            this.txtEmpTelNum1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lEmpTelNum1
            // 
            this.lEmpTelNum1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lEmpTelNum1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lEmpTelNum1.Location = new System.Drawing.Point(527, 145);
            this.lEmpTelNum1.Name = "lEmpTelNum1";
            this.lEmpTelNum1.Size = new System.Drawing.Size(88, 18);
            this.lEmpTelNum1.TabIndex = 0;
            this.lEmpTelNum1.Text = "No.";
            // 
            // lEmpTelCode1
            // 
            this.lEmpTelCode1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lEmpTelCode1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lEmpTelCode1.Location = new System.Drawing.Point(455, 145);
            this.lEmpTelCode1.Name = "lEmpTelCode1";
            this.lEmpTelCode1.Size = new System.Drawing.Size(75, 18);
            this.lEmpTelCode1.TabIndex = 0;
            this.lEmpTelCode1.Text = "Tel. code:";
            // 
            // txtEmpTelCode1
            // 
            this.txtEmpTelCode1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtEmpTelCode1.Location = new System.Drawing.Point(455, 169);
            this.txtEmpTelCode1.MaxLength = 8;
            this.txtEmpTelCode1.Name = "txtEmpTelCode1";
            this.txtEmpTelCode1.Size = new System.Drawing.Size(57, 20);
            this.txtEmpTelCode1.TabIndex = 5;
            this.txtEmpTelCode1.Tag = "lEmpTelCode1";
            this.txtEmpTelCode1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // drpPayFrequency1
            // 
            this.drpPayFrequency1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPayFrequency1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpPayFrequency1.ItemHeight = 13;
            this.drpPayFrequency1.Location = new System.Drawing.Point(86, 169);
            this.drpPayFrequency1.Name = "drpPayFrequency1";
            this.drpPayFrequency1.Size = new System.Drawing.Size(160, 21);
            this.drpPayFrequency1.TabIndex = 3;
            this.drpPayFrequency1.Tag = "lPayFrequency1";
            this.drpPayFrequency1.SelectedIndexChanged += new System.EventHandler(this.drpPayFrequency1_SelectedIndexChanged);
            this.drpPayFrequency1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lPayFrequency1
            // 
            this.lPayFrequency1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lPayFrequency1.Location = new System.Drawing.Point(86, 145);
            this.lPayFrequency1.Name = "lPayFrequency1";
            this.lPayFrequency1.Size = new System.Drawing.Size(123, 18);
            this.lPayFrequency1.TabIndex = 0;
            this.lPayFrequency1.Text = "Pay Frequency:";
            // 
            // drpOccupation1
            // 
            this.drpOccupation1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpOccupation1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpOccupation1.ItemHeight = 13;
            this.drpOccupation1.Location = new System.Drawing.Point(281, 105);
            this.drpOccupation1.Name = "drpOccupation1";
            this.drpOccupation1.Size = new System.Drawing.Size(160, 21);
            this.drpOccupation1.TabIndex = 2;
            this.drpOccupation1.Tag = "lOccupation1";
            this.drpOccupation1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lOccupation1
            // 
            this.lOccupation1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lOccupation1.Location = new System.Drawing.Point(278, 81);
            this.lOccupation1.Name = "lOccupation1";
            this.lOccupation1.Size = new System.Drawing.Size(124, 19);
            this.lOccupation1.TabIndex = 0;
            this.lOccupation1.Text = "Occupation:";
            // 
            // drpEmploymentStat1
            // 
            this.drpEmploymentStat1.DisplayMember = "codedescript";
            this.drpEmploymentStat1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmploymentStat1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpEmploymentStat1.ItemHeight = 13;
            this.drpEmploymentStat1.Location = new System.Drawing.Point(86, 105);
            this.drpEmploymentStat1.Name = "drpEmploymentStat1";
            this.drpEmploymentStat1.Size = new System.Drawing.Size(160, 21);
            this.drpEmploymentStat1.TabIndex = 1;
            this.drpEmploymentStat1.Tag = "lEmploymentStat1";
            this.drpEmploymentStat1.SelectedIndexChanged += new System.EventHandler(this.drpEmploymentStat1_SelectedIndexChanged);
            this.drpEmploymentStat1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lEmploymentStat1
            // 
            this.lEmploymentStat1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lEmploymentStat1.Location = new System.Drawing.Point(86, 81);
            this.lEmploymentStat1.Name = "lEmploymentStat1";
            this.lEmploymentStat1.Size = new System.Drawing.Size(123, 19);
            this.lEmploymentStat1.TabIndex = 0;
            this.lEmploymentStat1.Text = "Employment Status:";
            // 
            // tpFinancial
            // 
            this.tpFinancial.Controls.Add(this.groupBox9);
            this.tpFinancial.Controls.Add(this.groupBox2);
            this.tpFinancial.Controls.Add(this.groupBox3);
            this.tpFinancial.Controls.Add(this.groupBox4);
            this.tpFinancial.Location = new System.Drawing.Point(0, 25);
            this.tpFinancial.Name = "tpFinancial";
            this.tpFinancial.Selected = false;
            this.tpFinancial.Size = new System.Drawing.Size(756, 290);
            this.tpFinancial.TabIndex = 3;
            this.tpFinancial.Title = "Financial";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.txtDisposable);
            this.groupBox9.Controls.Add(this.lDisposable);
            this.groupBox9.Location = new System.Drawing.Point(0, 224);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(392, 64);
            this.groupBox9.TabIndex = 3;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Monthly Disposable Income";
            // 
            // txtDisposable
            // 
            this.txtDisposable.Enabled = false;
            this.txtDisposable.Location = new System.Drawing.Point(208, 24);
            this.txtDisposable.Name = "txtDisposable";
            this.txtDisposable.ReadOnly = true;
            this.txtDisposable.Size = new System.Drawing.Size(105, 18);
            this.txtDisposable.TabIndex = 5;
            this.txtDisposable.Tag = "lDisposable";
            // 
            // lDisposable
            // 
            this.lDisposable.Location = new System.Drawing.Point(16, 24);
            this.lDisposable.Name = "lDisposable";
            this.lDisposable.Size = new System.Drawing.Size(168, 23);
            this.lDisposable.TabIndex = 0;
            this.lDisposable.Text = "Monthly Income - Monthly Commitments - Monthly Rent/Mortgage";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lAddIncome1);
            this.groupBox2.Controls.Add(this.txtAddIncome1);
            this.groupBox2.Controls.Add(this.lNetIncome1);
            this.groupBox2.Controls.Add(this.txtNetIncome1);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(112, 224);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Monthly Income";
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
            this.txtAddIncome1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
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
            this.txtNetIncome1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lAdditionalExpenditure2);
            this.groupBox3.Controls.Add(this.lAdditionalExpenditure1);
            this.groupBox3.Controls.Add(this.txtAdditionalExpenditure2);
            this.groupBox3.Controls.Add(this.txtAdditionalExpenditure1);
            this.groupBox3.Controls.Add(this.txtOther1);
            this.groupBox3.Controls.Add(this.lTotal1);
            this.groupBox3.Controls.Add(this.txtTotal1);
            this.groupBox3.Controls.Add(this.lOther1);
            this.groupBox3.Controls.Add(this.lMisc1);
            this.groupBox3.Controls.Add(this.txtMisc1);
            this.groupBox3.Controls.Add(this.lLoans1);
            this.groupBox3.Controls.Add(this.txtLoans1);
            this.groupBox3.Controls.Add(this.lUtilities1);
            this.groupBox3.Controls.Add(this.txtUtilities1);
            this.groupBox3.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(118, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(274, 224);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Monthly Commitments";
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
            this.txtAdditionalExpenditure2.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // txtAdditionalExpenditure1
            // 
            this.txtAdditionalExpenditure1.Location = new System.Drawing.Point(24, 152);
            this.txtAdditionalExpenditure1.Name = "txtAdditionalExpenditure1";
            this.txtAdditionalExpenditure1.Size = new System.Drawing.Size(96, 20);
            this.txtAdditionalExpenditure1.TabIndex = 2;
            this.txtAdditionalExpenditure1.Tag = "lAdditionalExpenditure1";
            this.txtAdditionalExpenditure1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // txtOther1
            // 
            this.txtOther1.Location = new System.Drawing.Point(144, 96);
            this.txtOther1.Name = "txtOther1";
            this.txtOther1.Size = new System.Drawing.Size(96, 20);
            this.txtOther1.TabIndex = 4;
            this.txtOther1.Tag = "lOther1";
            this.txtOther1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
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
            this.txtTotal1.TextChanged += new System.EventHandler(this.txtTotal1_TextChanged);
            this.txtTotal1.Validating += new System.ComponentModel.CancelEventHandler(this.txtTotal1_Validating);
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
            this.txtMisc1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
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
            this.txtLoans1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
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
            this.txtUtilities1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lPayMethod);
            this.groupBox4.Controls.Add(this.drpPaymentMethod);
            this.groupBox4.Controls.Add(this.dtBankOpened1);
            this.groupBox4.Controls.Add(this.drpPayByGiro1);
            this.groupBox4.Controls.Add(this.txtCreditCardNo1);
            this.groupBox4.Controls.Add(this.lBankAccountName1);
            this.groupBox4.Controls.Add(this.lGiroDueDate1);
            this.groupBox4.Controls.Add(this.txtBankAccountName1);
            this.groupBox4.Controls.Add(this.drpGiroDueDate1);
            this.groupBox4.Controls.Add(this.lCreditCardNo1);
            this.groupBox4.Controls.Add(this.txtBankAcctNumber1);
            this.groupBox4.Controls.Add(this.drpBankAcctType1);
            this.groupBox4.Controls.Add(this.lPayByGiro1);
            this.groupBox4.Controls.Add(this.lBankAcctNumber1);
            this.groupBox4.Controls.Add(this.lBankAcctType1);
            this.groupBox4.Controls.Add(this.drpBank1);
            this.groupBox4.Controls.Add(this.lBank1);
            this.groupBox4.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(392, 0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(360, 288);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Bank Details";
            // 
            // lPayMethod
            // 
            this.lPayMethod.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lPayMethod.Location = new System.Drawing.Point(32, 240);
            this.lPayMethod.Name = "lPayMethod";
            this.lPayMethod.Size = new System.Drawing.Size(96, 16);
            this.lPayMethod.TabIndex = 26;
            this.lPayMethod.Text = "Payment Method:";
            // 
            // drpPaymentMethod
            // 
            this.drpPaymentMethod.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.drpPaymentMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPaymentMethod.DropDownWidth = 80;
            this.drpPaymentMethod.ItemHeight = 14;
            this.drpPaymentMethod.Location = new System.Drawing.Point(32, 256);
            this.drpPaymentMethod.Name = "drpPaymentMethod";
            this.drpPaymentMethod.Size = new System.Drawing.Size(160, 22);
            this.drpPaymentMethod.TabIndex = 25;
            this.drpPaymentMethod.Tag = "lPayMethod";
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
            this.dtBankOpened1.Location = new System.Drawing.Point(24, 128);
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
            this.dtBankOpened1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // drpPayByGiro1
            // 
            this.drpPayByGiro1.Location = new System.Drawing.Point(32, 208);
            this.drpPayByGiro1.Name = "drpPayByGiro1";
            this.drpPayByGiro1.Size = new System.Drawing.Size(57, 22);
            this.drpPayByGiro1.TabIndex = 4;
            this.drpPayByGiro1.Tag = "lPayByGiro1";
            this.drpPayByGiro1.SelectedIndexChanged += new System.EventHandler(this.drpPayByGiro1_SelectedIndexChanged);
            this.drpPayByGiro1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
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
            this.txtCreditCardNo1.TextChanged += new System.EventHandler(this.txtCreditCardNo1_TextChanged);
            this.txtCreditCardNo1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lBankAccountName1
            // 
            this.lBankAccountName1.Enabled = false;
            this.lBankAccountName1.Location = new System.Drawing.Point(184, 184);
            this.lBankAccountName1.Name = "lBankAccountName1";
            this.lBankAccountName1.Size = new System.Drawing.Size(131, 18);
            this.lBankAccountName1.TabIndex = 0;
            this.lBankAccountName1.Text = "Bank Account Name:";
            // 
            // lGiroDueDate1
            // 
            this.lGiroDueDate1.Enabled = false;
            this.lGiroDueDate1.Location = new System.Drawing.Point(104, 184);
            this.lGiroDueDate1.Name = "lGiroDueDate1";
            this.lGiroDueDate1.Size = new System.Drawing.Size(80, 18);
            this.lGiroDueDate1.TabIndex = 0;
            this.lGiroDueDate1.Text = "Giro Due Date:";
            // 
            // txtBankAccountName1
            // 
            this.txtBankAccountName1.Enabled = false;
            this.txtBankAccountName1.Location = new System.Drawing.Point(184, 208);
            this.txtBankAccountName1.MaxLength = 20;
            this.txtBankAccountName1.Name = "txtBankAccountName1";
            this.txtBankAccountName1.Size = new System.Drawing.Size(152, 20);
            this.txtBankAccountName1.TabIndex = 6;
            this.txtBankAccountName1.Tag = "lBankAccountName1";
            this.txtBankAccountName1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // drpGiroDueDate1
            // 
            this.drpGiroDueDate1.Enabled = false;
            this.drpGiroDueDate1.Location = new System.Drawing.Point(104, 208);
            this.drpGiroDueDate1.Name = "drpGiroDueDate1";
            this.drpGiroDueDate1.Size = new System.Drawing.Size(66, 22);
            this.drpGiroDueDate1.TabIndex = 5;
            this.drpGiroDueDate1.Tag = "lGiroDueDate1";
            this.drpGiroDueDate1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
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
            this.txtBankAcctNumber1.MaxLength = 20;
            this.txtBankAcctNumber1.Name = "txtBankAcctNumber1";
            this.txtBankAcctNumber1.Size = new System.Drawing.Size(120, 20);
            this.txtBankAcctNumber1.TabIndex = 2;
            this.txtBankAcctNumber1.Tag = "lBankAcctNumber1";
            this.txtBankAcctNumber1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
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
            this.drpBankAcctType1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lPayByGiro1
            // 
            this.lPayByGiro1.Location = new System.Drawing.Point(32, 184);
            this.lPayByGiro1.Name = "lPayByGiro1";
            this.lPayByGiro1.Size = new System.Drawing.Size(72, 18);
            this.lPayByGiro1.TabIndex = 0;
            this.lPayByGiro1.Text = "Pay By Giro:";
            // 
            // lBankAcctNumber1
            // 
            this.lBankAcctNumber1.Location = new System.Drawing.Point(32, 72);
            this.lBankAcctNumber1.Name = "lBankAcctNumber1";
            this.lBankAcctNumber1.Size = new System.Drawing.Size(96, 18);
            this.lBankAcctNumber1.TabIndex = 0;
            this.lBankAcctNumber1.Text = "Account Number:";
            // 
            // lBankAcctType1
            // 
            this.lBankAcctType1.Location = new System.Drawing.Point(208, 16);
            this.lBankAcctType1.Name = "lBankAcctType1";
            this.lBankAcctType1.Size = new System.Drawing.Size(97, 18);
            this.lBankAcctType1.TabIndex = 0;
            this.lBankAcctType1.Text = "Account Type:";
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
            this.drpBank1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lBank1
            // 
            this.lBank1.Location = new System.Drawing.Point(32, 16);
            this.lBank1.Name = "lBank1";
            this.lBank1.Size = new System.Drawing.Size(41, 18);
            this.lBank1.TabIndex = 0;
            this.lBank1.Text = "Bank:";
            // 
            // tpRFProducts
            // 
            this.tpRFProducts.Controls.Add(this.groupBox6);
            this.tpRFProducts.Location = new System.Drawing.Point(0, 25);
            this.tpRFProducts.Name = "tpRFProducts";
            this.tpRFProducts.Selected = false;
            this.tpRFProducts.Size = new System.Drawing.Size(756, 290);
            this.tpRFProducts.TabIndex = 4;
            this.tpRFProducts.Title = "Category";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.cbPurchaseCashLoan);
            this.groupBox6.Controls.Add(this.lblPurchaseCashLoan);
            this.groupBox6.Controls.Add(this.txtRFCategory);
            this.groupBox6.Controls.Add(this.tvRFCategory);
            this.groupBox6.Controls.Add(this.lRFCategory);
            this.groupBox6.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.groupBox6.Location = new System.Drawing.Point(46, 21);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(664, 248);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Product Category";
            // 
            // cbPurchaseCashLoan
            // 
            this.cbPurchaseCashLoan.AutoSize = true;
            this.cbPurchaseCashLoan.Location = new System.Drawing.Point(371, 206);
            this.cbPurchaseCashLoan.Name = "cbPurchaseCashLoan";
            this.cbPurchaseCashLoan.Size = new System.Drawing.Size(78, 18);
            this.cbPurchaseCashLoan.TabIndex = 1105;
            this.cbPurchaseCashLoan.Text = "Cash Loan";
            this.cbPurchaseCashLoan.UseVisualStyleBackColor = true;
            this.cbPurchaseCashLoan.Visible = false;
            // 
            // lblPurchaseCashLoan
            // 
            this.lblPurchaseCashLoan.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lblPurchaseCashLoan.Location = new System.Drawing.Point(368, 146);
            this.lblPurchaseCashLoan.Name = "lblPurchaseCashLoan";
            this.lblPurchaseCashLoan.Size = new System.Drawing.Size(264, 51);
            this.lblPurchaseCashLoan.TabIndex = 1104;
            this.lblPurchaseCashLoan.Text = "Please check the check box below if the Customer intends to take out a Cash Loan " +
    "right away";
            this.lblPurchaseCashLoan.Visible = false;
            // 
            // txtRFCategory
            // 
            this.txtRFCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtRFCategory.Location = new System.Drawing.Point(376, 104);
            this.txtRFCategory.Name = "txtRFCategory";
            this.txtRFCategory.ReadOnly = true;
            this.txtRFCategory.Size = new System.Drawing.Size(152, 21);
            this.txtRFCategory.TabIndex = 0;
            this.txtRFCategory.TabStop = false;
            this.txtRFCategory.Tag = "lRFCategory";
            this.txtRFCategory.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // tvRFCategory
            // 
            this.tvRFCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.tvRFCategory.Location = new System.Drawing.Point(64, 40);
            this.tvRFCategory.Name = "tvRFCategory";
            this.tvRFCategory.Size = new System.Drawing.Size(240, 184);
            this.tvRFCategory.TabIndex = 0;
            this.tvRFCategory.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvRFCategory_AfterSelect);
            // 
            // lRFCategory
            // 
            this.lRFCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lRFCategory.Location = new System.Drawing.Point(368, 40);
            this.lRFCategory.Name = "lRFCategory";
            this.lRFCategory.Size = new System.Drawing.Size(264, 40);
            this.lRFCategory.TabIndex = 1103;
            this.lRFCategory.Text = "Please select the product category that the customer intends to buy using their a" +
    "ccount.";
            // 
            // tpApp2
            // 
            this.tpApp2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tpApp2.Controls.Add(this.tcApp2);
            this.tpApp2.Location = new System.Drawing.Point(0, 25);
            this.tpApp2.Name = "tpApp2";
            this.tpApp2.Selected = false;
            this.tpApp2.Size = new System.Drawing.Size(760, 319);
            this.tpApp2.TabIndex = 1;
            this.tpApp2.Title = "Applicant 2";
            // 
            // tcApp2
            // 
            this.tcApp2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcApp2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.tcApp2.IDEPixelArea = true;
            this.tcApp2.Location = new System.Drawing.Point(0, 0);
            this.tcApp2.Name = "tcApp2";
            this.tcApp2.PositionTop = true;
            this.tcApp2.SelectedIndex = 0;
            this.tcApp2.SelectedTab = this.tpPersonal2;
            this.tcApp2.Size = new System.Drawing.Size(756, 315);
            this.tcApp2.TabIndex = 0;
            this.tcApp2.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpPersonal2,
            this.tpEmployment2,
            this.tpFinancial2});
            // 
            // tpPersonal2
            // 
            this.tpPersonal2.Controls.Add(this.txtFirstName2);
            this.tpPersonal2.Controls.Add(this.txtAlias2);
            this.tpPersonal2.Controls.Add(this.txtLastName2);
            this.tpPersonal2.Controls.Add(this.drpTitle2);
            this.tpPersonal2.Controls.Add(this.lTitle2);
            this.tpPersonal2.Controls.Add(this.lFirstName2);
            this.tpPersonal2.Controls.Add(this.lLastName2);
            this.tpPersonal2.Controls.Add(this.lAlias2);
            this.tpPersonal2.Controls.Add(this.drpIDSelection2);
            this.tpPersonal2.Controls.Add(this.lIDSelection2);
            this.tpPersonal2.Controls.Add(this.lMoreRewardsDate2);
            this.tpPersonal2.Controls.Add(this.dtMoreRewardsDate2);
            this.tpPersonal2.Controls.Add(this.lMoreRewards2);
            this.tpPersonal2.Controls.Add(this.txtMoreRewards2);
            this.tpPersonal2.Controls.Add(this.lDOB2);
            this.tpPersonal2.Controls.Add(this.dtDOB2);
            this.tpPersonal2.Controls.Add(this.lAge2);
            this.tpPersonal2.Controls.Add(this.txtAge2);
            this.tpPersonal2.Controls.Add(this.drpSex2);
            this.tpPersonal2.Controls.Add(this.lSex2);
            this.tpPersonal2.Location = new System.Drawing.Point(0, 25);
            this.tpPersonal2.Name = "tpPersonal2";
            this.tpPersonal2.Size = new System.Drawing.Size(756, 290);
            this.tpPersonal2.TabIndex = 0;
            this.tpPersonal2.Title = "Personal";
            // 
            // txtFirstName2
            // 
            this.txtFirstName2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtFirstName2.Location = new System.Drawing.Point(352, 47);
            this.txtFirstName2.MaxLength = 30;
            this.txtFirstName2.Name = "txtFirstName2";
            this.txtFirstName2.Size = new System.Drawing.Size(178, 20);
            this.txtFirstName2.TabIndex = 2;
            this.txtFirstName2.Tag = "lFirstName2";
            this.txtFirstName2.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // txtAlias2
            // 
            this.txtAlias2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtAlias2.Location = new System.Drawing.Point(480, 111);
            this.txtAlias2.MaxLength = 25;
            this.txtAlias2.Name = "txtAlias2";
            this.txtAlias2.Size = new System.Drawing.Size(178, 20);
            this.txtAlias2.TabIndex = 4;
            this.txtAlias2.Tag = "lAlias2";
            this.txtAlias2.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // txtLastName2
            // 
            this.txtLastName2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtLastName2.Location = new System.Drawing.Point(264, 111);
            this.txtLastName2.MaxLength = 60;
            this.txtLastName2.Name = "txtLastName2";
            this.txtLastName2.Size = new System.Drawing.Size(178, 20);
            this.txtLastName2.TabIndex = 3;
            this.txtLastName2.Tag = "lLastName2";
            this.txtLastName2.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // drpTitle2
            // 
            this.drpTitle2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpTitle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpTitle2.ItemHeight = 13;
            this.drpTitle2.Location = new System.Drawing.Point(264, 47);
            this.drpTitle2.Name = "drpTitle2";
            this.drpTitle2.Size = new System.Drawing.Size(72, 21);
            this.drpTitle2.TabIndex = 1;
            this.drpTitle2.Tag = "lTitle2";
            this.drpTitle2.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lTitle2
            // 
            this.lTitle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lTitle2.Location = new System.Drawing.Point(264, 23);
            this.lTitle2.Name = "lTitle2";
            this.lTitle2.Size = new System.Drawing.Size(56, 19);
            this.lTitle2.TabIndex = 0;
            this.lTitle2.Text = "Title:";
            // 
            // lFirstName2
            // 
            this.lFirstName2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lFirstName2.Location = new System.Drawing.Point(352, 23);
            this.lFirstName2.Name = "lFirstName2";
            this.lFirstName2.Size = new System.Drawing.Size(117, 19);
            this.lFirstName2.TabIndex = 0;
            this.lFirstName2.Text = "First Name:";
            // 
            // lLastName2
            // 
            this.lLastName2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lLastName2.Location = new System.Drawing.Point(264, 87);
            this.lLastName2.Name = "lLastName2";
            this.lLastName2.Size = new System.Drawing.Size(117, 19);
            this.lLastName2.TabIndex = 0;
            this.lLastName2.Text = "Last Name:";
            // 
            // lAlias2
            // 
            this.lAlias2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lAlias2.Location = new System.Drawing.Point(480, 87);
            this.lAlias2.Name = "lAlias2";
            this.lAlias2.Size = new System.Drawing.Size(117, 19);
            this.lAlias2.TabIndex = 0;
            this.lAlias2.Text = "Alias:";
            // 
            // drpIDSelection2
            // 
            this.drpIDSelection2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpIDSelection2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpIDSelection2.ItemHeight = 13;
            this.drpIDSelection2.Location = new System.Drawing.Point(64, 51);
            this.drpIDSelection2.Name = "drpIDSelection2";
            this.drpIDSelection2.Size = new System.Drawing.Size(152, 21);
            this.drpIDSelection2.TabIndex = 0;
            this.drpIDSelection2.Tag = "lIDSelection2";
            this.drpIDSelection2.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lIDSelection2
            // 
            this.lIDSelection2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lIDSelection2.Location = new System.Drawing.Point(64, 23);
            this.lIDSelection2.Name = "lIDSelection2";
            this.lIDSelection2.Size = new System.Drawing.Size(85, 19);
            this.lIDSelection2.TabIndex = 0;
            this.lIDSelection2.Text = "ID Selection:";
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
            this.dtMoreRewardsDate2.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
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
            this.txtMoreRewards2.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lDOB2
            // 
            this.lDOB2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lDOB2.Location = new System.Drawing.Point(264, 151);
            this.lDOB2.Name = "lDOB2";
            this.lDOB2.Size = new System.Drawing.Size(96, 18);
            this.lDOB2.TabIndex = 0;
            this.lDOB2.Text = "Date of Birth:";
            // 
            // dtDOB2
            // 
            this.dtDOB2.CustomFormat = "ddd dd MMM yyyy";
            this.dtDOB2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtDOB2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDOB2.Location = new System.Drawing.Point(264, 175);
            this.dtDOB2.Name = "dtDOB2";
            this.dtDOB2.Size = new System.Drawing.Size(131, 20);
            this.dtDOB2.TabIndex = 5;
            this.dtDOB2.Tag = "lDOB2";
            this.dtDOB2.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            this.dtDOB2.ValueChanged += new System.EventHandler(this.dtDOB2_ValueChanged);
            this.dtDOB2.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lAge2
            // 
            this.lAge2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lAge2.Location = new System.Drawing.Point(416, 151);
            this.lAge2.Name = "lAge2";
            this.lAge2.Size = new System.Drawing.Size(37, 18);
            this.lAge2.TabIndex = 0;
            this.lAge2.Text = "Age:";
            // 
            // txtAge2
            // 
            this.txtAge2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtAge2.Location = new System.Drawing.Point(416, 175);
            this.txtAge2.MaxLength = 3;
            this.txtAge2.Name = "txtAge2";
            this.txtAge2.ReadOnly = true;
            this.txtAge2.Size = new System.Drawing.Size(46, 20);
            this.txtAge2.TabIndex = 6;
            this.txtAge2.Tag = "lAge2";
            this.txtAge2.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // drpSex2
            // 
            this.drpSex2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpSex2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpSex2.ItemHeight = 13;
            this.drpSex2.Location = new System.Drawing.Point(480, 175);
            this.drpSex2.Name = "drpSex2";
            this.drpSex2.Size = new System.Drawing.Size(96, 21);
            this.drpSex2.TabIndex = 7;
            this.drpSex2.Tag = "lSex2";
            this.drpSex2.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lSex2
            // 
            this.lSex2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lSex2.Location = new System.Drawing.Point(480, 151);
            this.lSex2.Name = "lSex2";
            this.lSex2.Size = new System.Drawing.Size(33, 18);
            this.lSex2.TabIndex = 0;
            this.lSex2.Text = "Sex:";
            // 
            // tpEmployment2
            // 
            this.tpEmployment2.Controls.Add(this.drpOccupation2);
            this.tpEmployment2.Controls.Add(this.lOccupation2);
            this.tpEmployment2.Controls.Add(this.dtEmploymentStart2);
            this.tpEmployment2.Controls.Add(this.drpEmploymentStat2);
            this.tpEmployment2.Controls.Add(this.lEmploymentStat2);
            this.tpEmployment2.Location = new System.Drawing.Point(0, 25);
            this.tpEmployment2.Name = "tpEmployment2";
            this.tpEmployment2.Selected = false;
            this.tpEmployment2.Size = new System.Drawing.Size(756, 290);
            this.tpEmployment2.TabIndex = 1;
            this.tpEmployment2.Title = "Employment";
            // 
            // drpOccupation2
            // 
            this.drpOccupation2.DisplayMember = "codedescript";
            this.drpOccupation2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpOccupation2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpOccupation2.ItemHeight = 13;
            this.drpOccupation2.Location = new System.Drawing.Point(376, 96);
            this.drpOccupation2.Name = "drpOccupation2";
            this.drpOccupation2.Size = new System.Drawing.Size(152, 21);
            this.drpOccupation2.TabIndex = 4;
            this.drpOccupation2.Tag = "lOccupation2";
            // 
            // lOccupation2
            // 
            this.lOccupation2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lOccupation2.Location = new System.Drawing.Point(376, 72);
            this.lOccupation2.Name = "lOccupation2";
            this.lOccupation2.Size = new System.Drawing.Size(124, 19);
            this.lOccupation2.TabIndex = 3;
            this.lOccupation2.Text = "Occupation:";
            // 
            // dtEmploymentStart2
            // 
            this.dtEmploymentStart2.DateFrom = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtEmploymentStart2.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtEmploymentStart2.Label = "Curr. emp. start:";
            this.dtEmploymentStart2.LinkedBias = false;
            this.dtEmploymentStart2.LinkedComboBox = null;
            this.dtEmploymentStart2.LinkedDatePicker = null;
            this.dtEmploymentStart2.LinkedLabel = null;
            this.dtEmploymentStart2.Location = new System.Drawing.Point(232, 152);
            this.dtEmploymentStart2.Months = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtEmploymentStart2.Name = "dtEmploymentStart2";
            this.dtEmploymentStart2.Size = new System.Drawing.Size(256, 56);
            this.dtEmploymentStart2.TabIndex = 1;
            this.dtEmploymentStart2.Tag = "dtEmploymentStart2";
            this.dtEmploymentStart2.Value = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtEmploymentStart2.Years = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtEmploymentStart2.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // drpEmploymentStat2
            // 
            this.drpEmploymentStat2.DisplayMember = "codedescript";
            this.drpEmploymentStat2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmploymentStat2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.drpEmploymentStat2.ItemHeight = 13;
            this.drpEmploymentStat2.Location = new System.Drawing.Point(200, 97);
            this.drpEmploymentStat2.Name = "drpEmploymentStat2";
            this.drpEmploymentStat2.Size = new System.Drawing.Size(152, 21);
            this.drpEmploymentStat2.TabIndex = 0;
            this.drpEmploymentStat2.Tag = "lEmploymentStat2";
            this.drpEmploymentStat2.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lEmploymentStat2
            // 
            this.lEmploymentStat2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lEmploymentStat2.Location = new System.Drawing.Point(200, 73);
            this.lEmploymentStat2.Name = "lEmploymentStat2";
            this.lEmploymentStat2.Size = new System.Drawing.Size(123, 18);
            this.lEmploymentStat2.TabIndex = 0;
            this.lEmploymentStat2.Text = "Employment Status:";
            // 
            // tpFinancial2
            // 
            this.tpFinancial2.Controls.Add(this.groupBox8);
            this.tpFinancial2.Controls.Add(this.groupBox7);
            this.tpFinancial2.Location = new System.Drawing.Point(0, 25);
            this.tpFinancial2.Name = "tpFinancial2";
            this.tpFinancial2.Selected = false;
            this.tpFinancial2.Size = new System.Drawing.Size(756, 290);
            this.tpFinancial2.TabIndex = 2;
            this.tpFinancial2.Title = "Financial";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.dtBankOpened2);
            this.groupBox8.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.groupBox8.Location = new System.Drawing.Point(8, 136);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(736, 144);
            this.groupBox8.TabIndex = 1;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Bank Details";
            // 
            // dtBankOpened2
            // 
            this.dtBankOpened2.DateFrom = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtBankOpened2.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dtBankOpened2.Label = "Bank acct opened:";
            this.dtBankOpened2.LinkedBias = false;
            this.dtBankOpened2.LinkedComboBox = null;
            this.dtBankOpened2.LinkedDatePicker = null;
            this.dtBankOpened2.LinkedLabel = null;
            this.dtBankOpened2.Location = new System.Drawing.Point(112, 56);
            this.dtBankOpened2.Months = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtBankOpened2.Name = "dtBankOpened2";
            this.dtBankOpened2.Size = new System.Drawing.Size(256, 56);
            this.dtBankOpened2.TabIndex = 0;
            this.dtBankOpened2.Tag = "dtBankOpened2";
            this.dtBankOpened2.Value = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
            this.dtBankOpened2.Years = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dtBankOpened2.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.lAddIncome2);
            this.groupBox7.Controls.Add(this.txtAddIncome2);
            this.groupBox7.Controls.Add(this.lNetIncome2);
            this.groupBox7.Controls.Add(this.txtNetIncome2);
            this.groupBox7.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.groupBox7.Location = new System.Drawing.Point(8, 8);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(736, 120);
            this.groupBox7.TabIndex = 0;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Monthly Income";
            // 
            // lAddIncome2
            // 
            this.lAddIncome2.Location = new System.Drawing.Point(272, 34);
            this.lAddIncome2.Name = "lAddIncome2";
            this.lAddIncome2.Size = new System.Drawing.Size(75, 29);
            this.lAddIncome2.TabIndex = 0;
            this.lAddIncome2.Text = "Additional Income:";
            // 
            // txtAddIncome2
            // 
            this.txtAddIncome2.Location = new System.Drawing.Point(272, 66);
            this.txtAddIncome2.Name = "txtAddIncome2";
            this.txtAddIncome2.Size = new System.Drawing.Size(93, 20);
            this.txtAddIncome2.TabIndex = 1;
            this.txtAddIncome2.Tag = "lAddIncome2";
            this.txtAddIncome2.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lNetIncome2
            // 
            this.lNetIncome2.Location = new System.Drawing.Point(120, 42);
            this.lNetIncome2.Name = "lNetIncome2";
            this.lNetIncome2.Size = new System.Drawing.Size(93, 19);
            this.lNetIncome2.TabIndex = 0;
            this.lNetIncome2.Text = "Net Income:";
            // 
            // txtNetIncome2
            // 
            this.txtNetIncome2.Location = new System.Drawing.Point(120, 66);
            this.txtNetIncome2.Name = "txtNetIncome2";
            this.txtNetIncome2.Size = new System.Drawing.Size(93, 20);
            this.txtNetIncome2.TabIndex = 0;
            this.txtNetIncome2.Tag = "lNetIncome2";
            this.txtNetIncome2.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // tpAccounts
            // 
            this.tpAccounts.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tpAccounts.Controls.Add(this.groupBox5);
            this.tpAccounts.Location = new System.Drawing.Point(0, 25);
            this.tpAccounts.Name = "tpAccounts";
            this.tpAccounts.Selected = false;
            this.tpAccounts.Size = new System.Drawing.Size(760, 319);
            this.tpAccounts.TabIndex = 2;
            this.tpAccounts.Title = "Accounts";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtHolderSAccounts);
            this.groupBox5.Controls.Add(this.txtHolderCAccounts);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.txtJointCAccounts);
            this.groupBox5.Controls.Add(this.txtJointSAccounts);
            this.groupBox5.Controls.Add(this.dgAccounts);
            this.groupBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(10, 9);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(736, 296);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Accounts Summary";
            // 
            // txtHolderSAccounts
            // 
            this.txtHolderSAccounts.Location = new System.Drawing.Point(232, 72);
            this.txtHolderSAccounts.Name = "txtHolderSAccounts";
            this.txtHolderSAccounts.Size = new System.Drawing.Size(37, 20);
            this.txtHolderSAccounts.TabIndex = 0;
            this.txtHolderSAccounts.TabStop = false;
            // 
            // txtHolderCAccounts
            // 
            this.txtHolderCAccounts.Location = new System.Drawing.Point(232, 24);
            this.txtHolderCAccounts.Name = "txtHolderCAccounts";
            this.txtHolderCAccounts.Size = new System.Drawing.Size(37, 20);
            this.txtHolderCAccounts.TabIndex = 0;
            this.txtHolderCAccounts.TabStop = false;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(376, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(117, 37);
            this.label5.TabIndex = 0;
            this.label5.Text = "Second applicant\'s settled accounts:";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(376, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(117, 38);
            this.label7.TabIndex = 0;
            this.label7.Text = "Second applicant\'s current accounts:";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(120, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(117, 37);
            this.label6.TabIndex = 0;
            this.label6.Text = "Main applicant\'s settled accounts:";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(120, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(117, 38);
            this.label8.TabIndex = 0;
            this.label8.Text = "Main applicant\'s current accounts:";
            // 
            // txtJointCAccounts
            // 
            this.txtJointCAccounts.Location = new System.Drawing.Point(496, 24);
            this.txtJointCAccounts.Name = "txtJointCAccounts";
            this.txtJointCAccounts.Size = new System.Drawing.Size(37, 20);
            this.txtJointCAccounts.TabIndex = 0;
            this.txtJointCAccounts.TabStop = false;
            // 
            // txtJointSAccounts
            // 
            this.txtJointSAccounts.Location = new System.Drawing.Point(496, 72);
            this.txtJointSAccounts.Name = "txtJointSAccounts";
            this.txtJointSAccounts.Size = new System.Drawing.Size(37, 20);
            this.txtJointSAccounts.TabIndex = 0;
            this.txtJointSAccounts.TabStop = false;
            // 
            // dgAccounts
            // 
            this.dgAccounts.CaptionText = "Main applicant\'s Accounts";
            this.dgAccounts.DataMember = "";
            this.dgAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAccounts.Location = new System.Drawing.Point(32, 120);
            this.dgAccounts.Name = "dgAccounts";
            this.dgAccounts.ReadOnly = true;
            this.dgAccounts.Size = new System.Drawing.Size(664, 160);
            this.dgAccounts.TabIndex = 0;
            this.dgAccounts.TabStop = false;
            // 
            // tpComments
            // 
            this.tpComments.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tpComments.Controls.Add(this.groupBox1);
            this.tpComments.Location = new System.Drawing.Point(0, 25);
            this.tpComments.Name = "tpComments";
            this.tpComments.Selected = false;
            this.tpComments.Size = new System.Drawing.Size(760, 319);
            this.tpComments.TabIndex = 3;
            this.tpComments.Title = "Comments";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtNewS1Comment);
            this.groupBox1.Controls.Add(this.txtS1Comment);
            this.groupBox1.Controls.Add(this.lNewS1Comment);
            this.groupBox1.Location = new System.Drawing.Point(10, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(736, 298);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Comments";
            // 
            // txtNewS1Comment
            // 
            this.txtNewS1Comment.Location = new System.Drawing.Point(24, 24);
            this.txtNewS1Comment.MaxLength = 1000;
            this.txtNewS1Comment.Name = "txtNewS1Comment";
            this.txtNewS1Comment.Size = new System.Drawing.Size(684, 88);
            this.txtNewS1Comment.TabIndex = 1;
            this.txtNewS1Comment.Tag = "lNewS1Comment";
            this.txtNewS1Comment.Text = "";
            // 
            // txtS1Comment
            // 
            this.txtS1Comment.BackColor = System.Drawing.SystemColors.Control;
            this.txtS1Comment.Location = new System.Drawing.Point(24, 112);
            this.txtS1Comment.MaxLength = 1000;
            this.txtS1Comment.Name = "txtS1Comment";
            this.txtS1Comment.ReadOnly = true;
            this.txtS1Comment.Size = new System.Drawing.Size(684, 160);
            this.txtS1Comment.TabIndex = 0;
            this.txtS1Comment.Text = "";
            // 
            // lNewS1Comment
            // 
            this.lNewS1Comment.Location = new System.Drawing.Point(136, 48);
            this.lNewS1Comment.Name = "lNewS1Comment";
            this.lNewS1Comment.Size = new System.Drawing.Size(80, 16);
            this.lNewS1Comment.TabIndex = 2;
            this.lNewS1Comment.Text = "dummy";
            // 
            // pictRefer
            // 
            this.pictRefer.Location = new System.Drawing.Point(712, 16);
            this.pictRefer.Name = "pictRefer";
            this.pictRefer.Size = new System.Drawing.Size(16, 16);
            this.pictRefer.TabIndex = 217;
            this.pictRefer.TabStop = false;
            this.pictRefer.Visible = false;
            // 
            // pictReject
            // 
            this.pictReject.Location = new System.Drawing.Point(712, 16);
            this.pictReject.Name = "pictReject";
            this.pictReject.Size = new System.Drawing.Size(24, 24);
            this.pictReject.TabIndex = 216;
            this.pictReject.TabStop = false;
            this.pictReject.Visible = false;
            // 
            // pictAccept
            // 
            this.pictAccept.Location = new System.Drawing.Point(712, 16);
            this.pictAccept.Name = "pictAccept";
            this.pictAccept.Size = new System.Drawing.Size(24, 24);
            this.pictAccept.TabIndex = 215;
            this.pictAccept.TabStop = false;
            this.pictAccept.Visible = false;
            // 
            // textCredit
            // 
            this.textCredit.Enabled = false;
            this.textCredit.Location = new System.Drawing.Point(560, 16);
            this.textCredit.Name = "textCredit";
            this.textCredit.Size = new System.Drawing.Size(96, 20);
            this.textCredit.TabIndex = 0;
            this.textCredit.TabStop = false;
            this.textCredit.Visible = false;
            // 
            // labelCredit
            // 
            this.labelCredit.Location = new System.Drawing.Point(464, 16);
            this.labelCredit.Name = "labelCredit";
            this.labelCredit.Size = new System.Drawing.Size(88, 16);
            this.labelCredit.TabIndex = 0;
            this.labelCredit.Text = "Spending Limit:";
            this.labelCredit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelCredit.Visible = false;
            // 
            // btnComplete
            // 
            this.btnComplete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnComplete.ImageIndex = 3;
            this.btnComplete.ImageList = this.menuIcons;
            this.btnComplete.Location = new System.Drawing.Point(744, 16);
            this.btnComplete.Name = "btnComplete";
            this.btnComplete.Size = new System.Drawing.Size(20, 18);
            this.btnComplete.TabIndex = 0;
            this.btnComplete.TabStop = false;
            this.btnComplete.Click += new System.EventHandler(this.btnComplete_Click);
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
            // 
            // drpApplicationType
            // 
            this.drpApplicationType.ItemHeight = 13;
            this.drpApplicationType.Location = new System.Drawing.Point(304, 16);
            this.drpApplicationType.Name = "drpApplicationType";
            this.drpApplicationType.Size = new System.Drawing.Size(104, 21);
            this.drpApplicationType.TabIndex = 0;
            this.drpApplicationType.TabStop = false;
            this.drpApplicationType.SelectedIndexChanged += new System.EventHandler(this.drpApplicationType_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(200, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(96, 18);
            this.label10.TabIndex = 0;
            this.label10.Text = "Application Type:";
            // 
            // lMandatory
            // 
            this.lMandatory.BackColor = System.Drawing.SystemColors.Highlight;
            this.lMandatory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lMandatory.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lMandatory.Location = new System.Drawing.Point(8, 16);
            this.lMandatory.Name = "lMandatory";
            this.lMandatory.Size = new System.Drawing.Size(176, 18);
            this.lMandatory.TabIndex = 0;
            this.lMandatory.Text = "mandatory";
            this.lMandatory.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuPrintRFDetails,
            this.menuExit});
            this.menuFile.Text = "&File";
            // 
            // menuPrintRFDetails
            // 
            this.menuPrintRFDetails.Description = "MenuItem";
            this.menuPrintRFDetails.Text = "&Print RF Details";
            this.menuPrintRFDetails.Click += new System.EventHandler(this.menuPrintRFDetails_Click);
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuSanction
            // 
            this.menuSanction.Description = "MenuItem";
            this.menuSanction.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuComplete,
            this.menuSave,
            this.menuReopen,
            this.menuManualRefer});
            this.menuSanction.Text = "&Sanction";
            // 
            // menuComplete
            // 
            this.menuComplete.Description = "MenuItem";
            this.menuComplete.Text = "&Complete";
            this.menuComplete.Click += new System.EventHandler(this.btnComplete_Click);
            // 
            // menuSave
            // 
            this.menuSave.Description = "MenuItem";
            this.menuSave.Text = "&Save";
            this.menuSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // menuReopen
            // 
            this.menuReopen.Description = "MenuItem";
            this.menuReopen.Enabled = false;
            this.menuReopen.Text = "&Re-open Stage 1";
            this.menuReopen.Visible = false;
            this.menuReopen.Click += new System.EventHandler(this.menuReopen_Click);
            // 
            // menuManualRefer
            // 
            this.menuManualRefer.Description = "MenuItem";
            this.menuManualRefer.Enabled = false;
            this.menuManualRefer.Text = "&Manual Refer";
            this.menuManualRefer.Visible = false;
            this.menuManualRefer.Click += new System.EventHandler(this.menuManualRefer_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider1.ContainerControl = this;
            // 
            // lShowResult
            // 
            this.lShowResult.Enabled = false;
            this.lShowResult.Location = new System.Drawing.Point(256, 16);
            this.lShowResult.Name = "lShowResult";
            this.lShowResult.Size = new System.Drawing.Size(88, 16);
            this.lShowResult.TabIndex = 4;
            this.lShowResult.Text = "lShowResult";
            this.lShowResult.Visible = false;
            // 
            // SanctionStage1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 478);
            this.Controls.Add(this.gbData);
            this.Controls.Add(this.gpCustomer);
            this.Controls.Add(this.lShowResult);
            this.Name = "SanctionStage1";
            this.Text = "Credit Sanction Stage 1";
            this.Load += new System.EventHandler(this.SanctionStage1_Load);
            this.Enter += new System.EventHandler(this.SanctionStage1_Enter);
            this.Leave += new System.EventHandler(this.SanctionStage1_Leave);
            this.gpCustomer.ResumeLayout(false);
            this.gpCustomer.PerformLayout();
            this.gbData.ResumeLayout(false);
            this.gbData.PerformLayout();
            this.tcApplicants.ResumeLayout(false);
            this.tpApp1.ResumeLayout(false);
            this.tcApp1.ResumeLayout(false);
            this.tpPersonal.ResumeLayout(false);
            this.tpPersonal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.noDependencies1)).EndInit();
            this.tpResidential.ResumeLayout(false);
            this.tpResidential.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDistanceFromStore1)).EndInit();
            this.tpEmployment.ResumeLayout(false);
            this.tpEmployment.PerformLayout();
            this.tpFinancial.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tpRFProducts.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tpApp2.ResumeLayout(false);
            this.tcApp2.ResumeLayout(false);
            this.tpPersonal2.ResumeLayout(false);
            this.tpPersonal2.PerformLayout();
            this.tpEmployment2.ResumeLayout(false);
            this.tpFinancial2.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.tpAccounts.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
            this.tpComments.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictRefer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictReject)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictAccept)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion


        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":menuReopen"] = this.menuReopen;
            dynamicMenus[this.Name + ":menuManualRefer"] = this.menuManualRefer;
            dynamicMenus[this.Name + ":lShowResult"] = this.lShowResult;
        }

        /// <summary>
        /// This method will load all the required static data for drop down 
        /// fields etc, required by the screen
        /// </summary>
        private bool LoadStatic()
        {
            bool status = true;
            Function = "SanctionStage1::LoadStatic()";
            drpSex2.DataSource = new string[] { String.Empty, "Male", "Female" };
            drpSex1.DataSource = new string[] { String.Empty, "Male", "Female" };
            drpPayByGiro1.DataSource = new string[] { String.Empty, "No", "Yes" };

            if (dropDownsDS != null)
            {
                foreach (DataTable dt in dropDownsDS.Tables)
                {
                    switch (dt.TableName)
                    {
                        case TN.ApplicationType:
                            DataRow r = dt.NewRow();
                            r[CN.Code] = "H";
                            r[CN.CodeDescription] = "Sole";
                            dt.Rows.InsertAt(r, 0);
                            drpApplicationType.DataSource = dt;
                            drpApplicationType.DisplayMember = CN.CodeDescription;
                            break;
                        default:
                            StaticData.Tables[dt.TableName] = dt;
                            break;
                    }
                }
            }

            drpIDSelection2.DataSource = (DataTable)StaticData.Tables[TN.IDSelection];
            drpIDSelection2.DisplayMember = CN.CodeDescription;
            drpIDSelection1.DataSource = ((DataTable)StaticData.Tables[TN.IDSelection]).Copy();
            drpIDSelection1.DisplayMember = CN.CodeDescription;

            drpTitle2.DataSource = (DataTable)StaticData.Tables[TN.Title];
            drpTitle2.DisplayMember = CN.CodeDescription;

            drpMaritalStat1.DataSource = ((DataTable)StaticData.Tables[TN.MaritalStatus]).AddBlankCode();
            drpMaritalStat1.DisplayMember = CN.CodeDescription;
            drpMaritalStat1.ValueMember = CN.Code;

            drpEthnicGroup1.DataSource = (DataTable)StaticData.Tables[TN.EthnicGroup];
            drpEthnicGroup1.DisplayMember = CN.CodeDescription;
            drpEthnicGroup1.ValueMember = CN.Code;

            drpNationality1.DataSource = ((DataTable)StaticData.Tables[TN.Nationality]).AddBlankCode();
            drpNationality1.DisplayMember = CN.CodeDescription;
            drpNationality1.ValueMember = CN.Code;

            drpPropertyType1.DataSource = ((DataTable)StaticData.Tables[TN.PropertyType]).AddBlankCode();
            drpPropertyType1.DisplayMember = CN.CodeDescription;
            drpPropertyType1.ValueMember = CN.Code;

            drpCurrentResidentialStatus1.DataSource = ((DataTable)StaticData.Tables[TN.ResidentialStatus]).AddBlankCode();
            drpCurrentResidentialStatus1.DisplayMember = drpPrevResidentialStatus1.DisplayMember = CN.CodeDescription;
            drpCurrentResidentialStatus1.ValueMember = drpPrevResidentialStatus1.ValueMember = CN.Code;
            drpPrevResidentialStatus1.DataSource = ((DataTable)StaticData.Tables[TN.ResidentialStatus]).Copy().AddBlankCode();

            drpEmploymentStat1.DataSource = (DataTable)StaticData.Tables[TN.EmploymentStatus];
            //drpEmploymentStat1.DisplayMember = drpEmploymentStat2.DisplayMember = CN.CodeDescription;
            drpEmploymentStat2.DataSource = ((DataTable)StaticData.Tables[TN.EmploymentStatus]).Copy();

            drpOccupation1.DataSource = (DataTable)StaticData.Tables[TN.Occupation];
            drpOccupation1.DisplayMember = drpOccupation2.DisplayMember = CN.CodeDescription;
            drpOccupation2.DataSource = ((DataTable)StaticData.Tables[TN.Occupation]).Copy();

            drpPayFrequency1.DataSource = (DataTable)StaticData.Tables[TN.PayFrequency];
            drpPayFrequency1.DisplayMember = CN.CodeDescription;

            drpBank1.DataSource = (DataTable)StaticData.Tables[TN.Bank];
            drpBank1.DisplayMember = CN.BankName;

            drpBankAcctType1.DataSource = (DataTable)StaticData.Tables[TN.BankAccountType];
            drpBankAcctType1.DisplayMember = CN.CodeDescription;

            drpGiroDueDate1.DataSource = (DataTable)StaticData.Tables[TN.DDDueDate];
            drpGiroDueDate1.DisplayMember = CN.DueDay;

            drpPaymentMethod.DataSource = (DataTable)StaticData.Tables[TN.MethodOfPayment];
            drpPaymentMethod.DisplayMember = CN.CodeDescription;

            //CR 866 Added drop downs 
            drpEductation1.DataSource = ((DataTable)StaticData.Tables[TN.EducationLevels]).AddBlankCode();
            drpEductation1.DisplayMember = CN.CodeDescription;
            drpEductation1.ValueMember = CN.Code;

            drpTransportType1.DataSource = (DataTable)StaticData.Tables[TN.TransportTypes];
            drpTransportType1.DisplayMember = CN.CodeDescription;
            drpTransportType1.ValueMember = CN.Code;

            //CR866b converted these text boxes to dropdowns
            txtOrganisation1.DataSource = (DataTable)StaticData.Tables[TN.Organisations];
            txtOrganisation1.DisplayMember = CN.CodeDescription;
            txtOrganisation1.ValueMember = CN.Code;

            txtIndustry1.DataSource = (DataTable)StaticData.Tables[TN.Industries];
            txtIndustry1.DisplayMember = CN.CodeDescription;
            txtIndustry1.ValueMember = CN.Code;

            txtJobTitle1.DataSource = (DataTable)StaticData.Tables[TN.JobTitles];
            txtJobTitle1.DisplayMember = CN.CodeDescription;
            txtJobTitle1.ValueMember = CN.Code;

            //End CR 866

            LoadTreeView((DataTable)StaticData.Tables[TN.ProductCategories]);
            return status;
        }

        private void LoadTreeView(DataTable dt)
        {
            DataTable dtElec = dt.Copy();
            DataTable dtFurn = dt.Copy();
            DataTable dtWork = dt.Copy();
            DataTable dtOther = dt.Copy();

            tvRFCategory.Nodes.Clear();
            dtElec.DefaultView.RowFilter = CN.Category + " = 'PCE'";
            TreeNode e = new TreeNode("Electrical");
            tvRFCategory.Nodes.Add(e);
            foreach (DataRowView d in dtElec.DefaultView)
            {
                TreeNode cat = new TreeNode((string)d[CN.CodeDescription]);
                cat.Tag = d;
                e.Nodes.Add(cat);
            }

            dtFurn.DefaultView.RowFilter = CN.Category + " = 'PCF'";
            TreeNode f = new TreeNode("Furniture");
            tvRFCategory.Nodes.Add(f);
            foreach (DataRowView d in dtFurn.DefaultView)
            {
                TreeNode cat = new TreeNode((string)d[CN.CodeDescription]);
                cat.Tag = d;
                f.Nodes.Add(cat);
            }

            dtWork.DefaultView.RowFilter = CN.Category + " = 'PCW'";
            TreeNode w = new TreeNode("Workstation");
            tvRFCategory.Nodes.Add(w);
            foreach (DataRowView d in dtWork.DefaultView)
            {
                TreeNode cat = new TreeNode((string)d[CN.CodeDescription]);
                cat.Tag = d;
                w.Nodes.Add(cat);
            }

            dtOther.DefaultView.RowFilter = CN.Category + " = 'PCO'";
            TreeNode o = new TreeNode("Other");
            tvRFCategory.Nodes.Add(o);
            foreach (DataRowView d in dtOther.DefaultView)
            {
                TreeNode cat = new TreeNode((string)d[CN.CodeDescription]);
                cat.Tag = d;
                o.Nodes.Add(cat);
            }
        }

        private void LoadData()
        {
            Thread dataThread = new Thread(new ThreadStart(LoadDataThread));
            dataThread.Start();
            dataThread.Join();
        }

        /// <summary>
        /// Data required for the screen is loaded in a seperate thread because 
        /// this give a big performance gain on Windows NT
        /// </summary>
        private void LoadDataThread()
        {
            bool status = true;

            try
            {

                /*#if(DEBUG)
                                logMessage("Thread starting at: "+DateTime.Now.ToLongTimeString(), Credential.User, EventLogEntryType.Information);
                #endif*/

                if ((bool)Country[CountryParameterNames.LoggingEnabled])
                    logMessage("Thread starting at: " + DateTime.Now.ToLongTimeString(), Credential.User, STL.PL.WS4.EventLogEntryType.Information);

                LockAccount();

                //Get mandatory fields 
                mandatoryFieldsDS = StaticDataManager.GetMandatoryFields(Config.CountryCode, this.Name, out Error);
                if (Error.Length > 0)
                {
                    ShowError(Error);
                    status = false;
                }

                //Get static data
                if (status)
                {
                    XmlUtilities xml = new XmlUtilities();
                    XmlDocument dropDowns = new XmlDocument();
                    dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                    if (StaticData.Tables[TN.IDSelection] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.IDSelection, new string[] { "IT1", "L" }));
                    if (StaticData.Tables[TN.Title] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Title, new string[] { "TTL", "L" }));
                    if (StaticData.Tables[TN.MaritalStatus] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.MaritalStatus, new string[] { "MS1", "L" }));
                    if (StaticData.Tables[TN.EthnicGroup] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.EthnicGroup, new string[] { "EG1", "L" }));
                    if (StaticData.Tables[TN.Nationality] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Nationality, new string[] { "NA2", "L" }));
                    if (StaticData.Tables[TN.PropertyType] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.PropertyType, new string[] { "PT1", "L" }));
                    if (StaticData.Tables[TN.ResidentialStatus] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ResidentialStatus, new string[] { "RS1", "L" }));
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
                    if (StaticData.Tables[TN.ApplicationType] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ApplicationType, new string[] { this.CustomerID, this.AccountNo }));
                    if (StaticData.Tables[TN.DDDueDate] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.DDDueDate, null));
                    if (StaticData.Tables[TN.ProductCategories] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ProductCategories, null));
                    if (StaticData.Tables[TN.MethodOfPayment] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.MethodOfPayment, null));

                    if (dropDowns.DocumentElement.ChildNodes.Count > 0)
                    {
                        dropDownsDS = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                            status = false;
                        }
                    }
                }

                if (status)
                {

                    prop = CreditManager.GetProposalStage1(this.CustomerID, this.AccountNo, SM.New, "H", out Error);
                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                        status = false;
                    }
                    else
                    {
                        string rel = String.Empty;
                        foreach (DataTable dt in prop.Tables)
                            if (dt.TableName == TN.Proposal)
                                foreach (DataRow row in dt.Rows)
                                {
                                    rel = (string)row[CN.A2Relation];
                                    this.AllowReopenS1 = Convert.ToBoolean(row[CN.AllowReopenS1]);          //#10477
                                }
                        if (rel.Length != 0)
                        {
                            prop2 = CreditManager.GetProposalStage1(this.CustomerID, this.AccountNo, SM.New, rel, out Error);
                            if (Error.Length > 0)
                                ShowError(Error);
                        }
                    }

                }
                /*#if(DEBUG)
                                logMessage("Thread ending at: "+DateTime.Now.ToLongTimeString(), Credential.User, EventLogEntryType.Information);
                #endif*/
                if ((bool)Country[CountryParameterNames.LoggingEnabled])
                    logMessage("Thread ending at: " + DateTime.Now.ToLongTimeString(), Credential.User, STL.PL.WS4.EventLogEntryType.Information);

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

        /// <summary>
        /// This function will load any details we have in the proposal table for
        /// this customer and populate the screen fields
        /// Also gets information from the customer and custaddress tables
        /// </summary>
        private void LoadStage1Details()
        {
            bool PrevAddressBlank = true;
            bool IsDispIncomeChangesApplied = Convert.ToBoolean(Country[CountryParameterNames.ApplyNewDIChanges]);
            this.lDisposable.Text = IsDispIncomeChangesApplied ? string.Empty : "Monthly Income - Monthly Commitments - Monthly Rent/Mortgage";
            decimal total = 0;
            Function = "SanctionStage1::LoadStage1Details()";
            loadingscreen = true;
            
            try
            {
                if (tcApplicants.TabPages.Contains(tpApp2))
                {
                    tcApplicants.TabPages.Remove(tpApp2);
                }

                //Comparison using hard coded string - Not critical for Thailand.
                if (FormParent != null)
                {
                    if (FormParent.GetType().Name == "BasicCustomerDetails")
                    {
                        if (((BasicCustomerDetails)this.FormParent).title == "MR")
                        {
                            drpSex1.SelectedIndex = 1;
                        }
                        else if (((BasicCustomerDetails)this.FormParent).title == "MRS" || ((BasicCustomerDetails)this.FormParent).title == "MISS" || ((BasicCustomerDetails)this.FormParent).title == "MS")
                        {
                            drpSex1.SelectedIndex = 2;
                        }
                    }
                }

                if (prop != null)
                {
                    foreach (DataTable dt in prop.Tables)
                    {
                        #region customer data
                        if (dt.TableName == TN.Customer)
                        {
                            drpPropertyType1.SelectedIndex = 0;
                            drpEthnicGroup1.SelectedIndex = 0;
                            drpIDSelection1.SelectedIndex = 0;
                            drpCurrentResidentialStatus1.SelectedIndex = 0;
                            drpPrevResidentialStatus1.SelectedIndex = 0;

                            foreach (DataRow row in dt.Rows)		//should only be one row
                            {
                                foreach (DataRowView r in drpPropertyType1.Items)
                                {
                                    if ((string)r[CN.Code] == ((string)row[CN.PropertyType]).Trim())
                                    {
                                        drpPropertyType1.SelectedItem = r;
                                        break;
                                    }
                                }
                                foreach (DataRowView r in drpEthnicGroup1.Items)
                                {
                                    if ((string)r[CN.Code] == ((string)row[CN.Ethnicity]).Trim())
                                    {
                                        drpEthnicGroup1.SelectedItem = r;
                                        break;
                                    }
                                }

                                foreach (DataRowView r in drpIDSelection1.Items)
                                {
                                    if ((string)r[CN.Code] == ((string)row[CN.IDType]).Trim())
                                    {
                                        drpIDSelection1.SelectedItem = r;
                                        break;
                                    }
                                }
                                foreach (DataRowView r in drpCurrentResidentialStatus1.Items)
                                {
                                    if (((string)r[CN.Code]).Trim() == ((string)row[CN.ResidentialStatus]).Trim())
                                    {
                                        drpCurrentResidentialStatus1.SelectedItem = r;
                                        break;
                                    }
                                }
                                foreach (DataRowView r in drpPrevResidentialStatus1.Items)
                                {
                                    if (((string)r[CN.Code]).Trim() == ((string)row[CN.PrevResidentialStatus]).Trim())
                                    {
                                        drpPrevResidentialStatus1.SelectedItem = r;
                                        break;
                                    }
                                }

                                if ((DateTime)row[CN.DateIn] > DatePicker.MinValue)
                                {
                                    dtDateInCurrentAddress1.LinkedDatePicker = dtDateInPrevAddress1;
                                    dtDateInCurrentAddress1.LinkedComboBox = drpPrevResidentialStatus1;
                                    dtDateInCurrentAddress1.LinkedLabel = lPrevResidentialStatus1;
                                    dtDateInCurrentAddress1.Value = (DateTime)row[CN.DateIn];
                                }

                                dtDateInPrevAddress1.DateFrom = dtDateInCurrentAddress1.Value;
                                if ((DateTime)row[CN.PrevDateIn] > DatePicker.MinValue)
                                {
                                    dtDateInPrevAddress1.Value = (DateTime)row[CN.PrevDateIn];
                                    PrevAddressBlank = false;
                                }
                                else
                                {
                                    // DSR 23 Oct 2002 - This can be set by the Proposal data below.
                                    dtDateInPrevAddress1.Value = dtDateInCurrentAddress1.Value;
                                    PrevAddressBlank = true;
                                }

                                txtFirstName.Text = (string)row[CN.FirstName];
                                txtLastName.Text = (string)row[CN.LastName];
                                dtDOB1.TrySetValue((DateTime)row[CN.DOB], DateTime.Now);

                                switch ((string)row[CN.Sex])
                                {
                                    case "M": drpSex1.SelectedIndex = 1;
                                        break;
                                    case "F": drpSex1.SelectedIndex = 2;
                                        break;
                                    default: drpSex1.SelectedIndex = 0;
                                        break;
                                }
                                txtMoreRewards1.Text = (string)row[CN.MoreRewardsNo];

                                if ((DateTime)row[CN.EffectiveDate] > DatePicker.MinValue)
                                    dtMoreRewardsDate1.Value = (DateTime)row[CN.EffectiveDate];
                                else
                                    dtMoreRewardsDate1.Value = DateTime.Today;

                                if (DBNull.Value != row[CN.MonthlyRent])
                                    this.txtMortgage1.Text = ((double)row[CN.MonthlyRent]).ToString(DecimalPlaces);
                            }
                        }
                        #endregion

                        #region employment data
                        if (dt.TableName == TN.Employment)
                        {
                            drpEmploymentStat1.SelectedIndex = 0;
                            drpOccupation1.SelectedIndex = 0;
                            drpPayFrequency1.SelectedIndex = 0;

                            foreach (DataRow row in dt.Rows)		//should only be one row
                            {
                                foreach (DataRowView r in drpEmploymentStat1.Items)
                                {
                                    if ((string)r[CN.Code] == ((string)row[CN.EmploymentStatus]).Trim())
                                    {
                                        drpEmploymentStat1.SelectedItem = r;
                                        break;
                                    }
                                }
                                foreach (DataRowView r in drpOccupation1.Items)
                                {
                                    //CR 866 Changed CN.Occupation to CN.WorkType
                                    if ((string)r[CN.Code] == ((string)row[CN.WorkType]).Trim())
                                    {
                                        drpOccupation1.SelectedItem = r;
                                        break;
                                    }
                                }
                                foreach (DataRowView r in drpPayFrequency1.Items)
                                {
                                    if ((string)r[CN.Code] == ((string)row[CN.PayFrequency]).Trim())
                                    {
                                        drpPayFrequency1.SelectedItem = r;
                                        break;
                                    }
                                }

                                if ((DateTime)row[CN.DateEmployed] > DatePicker.MinValue)
                                    dtCurrEmpStart1.Value = (DateTime)row[CN.DateEmployed];
                                else
                                    dtCurrEmpStart1.Value = DateTime.Today;
                                dtCurrEmpStart1.LinkedDatePicker = dtPrevEmpStart1;

                                dtPrevEmpStart1.DateFrom = dtCurrEmpStart1.Value;
                                if ((DateTime)row[CN.PrevDateEmployed] > DatePicker.MinValue)
                                    dtPrevEmpStart1.Value = (DateTime)row[CN.PrevDateEmployed];
                                else
                                    dtPrevEmpStart1.Value = dtCurrEmpStart1.Value;

                                txtEmpTelCode1.Text = ((string)row[CN.PersDialCode]).Trim();
                                txtEmpTelNum1.Text = ((string)row[CN.PersTel]).Trim();

                                //CR 866 - Thailand scoring [PC]
                                txtJobTitle1.SelectedValue = row[CN.JobTitle].ToString();
                                txtOrganisation1.SelectedValue = row[CN.Organisation].ToString();
                                txtIndustry1.SelectedValue = row[CN.Industry].ToString();
                                SetError(SetSelectedValue(drpEductation1, row[CN.EducationLevel].ToString()), drpEductation1);

                                //End CR 866

                                // --------DSR 11 Apr 2003 - UAT fix S135
                                // This is populating NET monthly income which we cannot derive from GROSS annual income.
                                // This is now loaded from Proposal.MthlyIncome instead which will also mean it is blank 
                                // for each new proposal.
                                // if(row[CN.AnnualGross]!=DBNull.Value)
                                // 	   txtNetIncome1.Text = ((double)row[CN.AnnualGross]/12).ToString(DecimalPlaces);
                                // else
                                // 	   txtNetIncome1.Text = "";
                            }
                        }
                        #endregion

                        #region bank data
                        if (dt.TableName == TN.Bank)
                        {
                            drpBankAcctType1.SelectedIndex = 0;
                            drpBank1.SelectedIndex = 0;
                            drpGiroDueDate1.SelectedIndex = 0;

                            foreach (DataRow row in dt.Rows)		//should only be one row
                            {
                                foreach (DataRowView r in drpBankAcctType1.Items)
                                {
                                    if ((string)r[CN.Code] == ((string)row[CN.Code]).Trim())
                                    {
                                        drpBankAcctType1.SelectedItem = r;
                                        break;
                                    }
                                }
                                foreach (DataRowView r in drpBank1.Items)
                                {
                                    if ((string)r[CN.BankCode] == ((string)row[CN.BankCode]).Trim())
                                    {
                                        drpBank1.SelectedItem = r;
                                        break;
                                    }
                                }
                                if ((DateTime)row[CN.BankAccountOpened] > DatePicker.MinValue)
                                {
                                    dtBankOpened1.Value = (DateTime)row[CN.BankAccountOpened];
                                }
                                txtBankAcctNumber1.Text = (string)row[CN.BankAccountNo];

                                // Check to see if there's a current mandate record
                                // DSR 8/12/2003 Need private bool to disable this elsewhere
                                this._mandateExists = (bool)row[CN.IsMandate];

                                if (this._mandateExists)
                                {
                                    this._lastDrpPayByGiro1 = 2;
                                    drpPayByGiro1.SelectedIndex = 2;
                                    txtBankAccountName1.Text = (string)row[CN.BankAccountName];
                                    foreach (DataRowView r in drpGiroDueDate1.Items)
                                    {
                                        if ((int)r[CN.DueDayId] == (int)row[CN.DueDayId])
                                        {
                                            drpGiroDueDate1.SelectedItem = r;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    this._lastDrpPayByGiro1 = 1;
                                    drpPayByGiro1.SelectedIndex = 1;
                                }
                            }
                        }
                        #endregion

                        #region accountTotals data
                        if (dt.TableName == TN.AccountTotals)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                txtHolderCAccounts.Text = ((int)row[CN.CurrentAccounts]).ToString();
                                txtHolderSAccounts.Text = ((int)row[CN.SettledAccounts]).ToString();
                            }
                        }
                        #endregion

                        #region accounts data
                        if (dt.TableName == TN.Stage1)
                        {
                            if (dgAccounts.TableStyles.Count == 0)
                            {
                                dgAccounts.DataSource = prop;
                                dgAccounts.DataMember = dt.TableName;
                                DataGridTableStyle tabStyle = new DataGridTableStyle();
                                tabStyle.MappingName = dt.TableName;
                                dgAccounts.TableStyles.Add(tabStyle);

                                tabStyle.GridColumnStyles[CN.AccountNo].Width = 100;
                                tabStyle.GridColumnStyles[CN.OutstandingBalance].Width = 100;
                                ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.OutstandingBalance]).Format = DecimalPlaces;
                                tabStyle.GridColumnStyles[CN.Status].Width = 27;
                                tabStyle.GridColumnStyles[CN.Arrears].Width = 100;
                                ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Arrears]).Format = DecimalPlaces;
                                tabStyle.GridColumnStyles[CN.InstalAmount].Width = 100;
                                tabStyle.GridColumnStyles[CN.InstalAmount].NullText = "0";
                                ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.InstalAmount]).Format = DecimalPlaces;
                                tabStyle.GridColumnStyles[CN.DateDelivered].Width = 100;
                                tabStyle.GridColumnStyles[CN.DateDelivered].NullText = "";

                                tabStyle.Dispose();
                            }
                        }
                        #endregion

                        #region proposal data
                        if (dt.TableName == TN.Proposal)
                        {
                            drpPrevResidentialStatus1.SelectedIndex = 0;
                            drpMaritalStat1.SelectedIndex = 0;
                            drpNationality1.SelectedIndex = 0;
							chxSpouseWorking.Checked = false;
                            foreach (DataRow row in dt.Rows)
                            {


                                //find the selected RF category
                                foreach (TreeNode n in tvRFCategory.Nodes)
                                {
                                    foreach (TreeNode cat in n.Nodes)
                                    {
                                        if (cat.Tag != null)
                                        {
                                            DataRowView r = (DataRowView)cat.Tag;
                                            if (((short)row[CN.RFCategory]).ToString() == (string)r[CN.Code])
                                            {
                                                RFCategory = ((short)row[CN.RFCategory]).ToString();
                                                txtRFCategory.Text = (string)r[CN.CodeDescription];
                                            }
                                        }
                                    }
                                }

                                // --------DSR 23 Oct 2002 - UAT fixes J7 & and J11
                                // If there is no previous address in CustAddress, we should still
                                // load the Previous Address info from the Proposal table.

                                if (PrevAddressBlank)
                                {
                                    DateDiff PrevAddressPeriod = new DateDiff(dtDateInPrevAddress1.DateFrom, -(int)row[CN.PrevAddYY], -(int)row[CN.PrevAddMM]);
                                    dtDateInPrevAddress1.Value = PrevAddressPeriod.Date2;
                                    PrevAddressPeriod = null;
                                }

                                if (drpPrevResidentialStatus1.Text.Length < 1)
                                {
                                    foreach (DataRowView r in drpPrevResidentialStatus1.Items)
                                    {
                                        if ((string)r[CN.Code] == ((string)row[CN.PrevResidentialStatus]).Trim())
                                        {
                                            drpPrevResidentialStatus1.SelectedItem = r;
                                            break;
                                        }
                                    }
                                }
                                // --------DSR 23 Oct 2002 - End of UAT fixes J7 & and J11


                                foreach (DataRowView r in drpMaritalStat1.Items)
                                {
                                    if ((string)r[CN.Code] == ((string)row[CN.MaritalStatus]).Trim())
                                    {
                                        drpMaritalStat1.SelectedItem = r;
                                        break;
                                    }
                                }
                                SetChxSpouseWorkingVisibility();

                                foreach (DataRowView r in drpNationality1.Items)
                                {
                                    if ((string)r[CN.Code] == ((string)row[CN.Nationality]).Trim())
                                    {
                                        drpNationality1.SelectedItem = r;
                                        break;
                                    }
                                }

                                if (this.AccountNo != (string)row[CN.AccountNumber])
                                {
                                    // The load function has returned a different Acct No
                                    // for the current RF proposal. This account needs to be
                                    // locked here, and both accounts will need to be unlocked.
                                    this._firstAccountNo = this.AccountNo;
                                    this.AccountNo = (string)row[CN.AccountNumber];
                                    AccountLocked = AccountManager.LockAccount(this.AccountNo, Credential.UserId.ToString(), out Error);
                                    if (Error.Length > 0)
                                    {
                                        ShowError(Error);
                                        ReadOnly = (ReadOnly || !AccountLocked);
                                        this.SetReadOnly();
                                    }
                                }

                                this.Text = "Credit Sanction Stage 1 - " + AccountNo;
                                foreach (Crownwood.Magic.Controls.TabPage tp in ((MainForm)this.FormRoot).MainTabControl.TabPages)
                                {
                                    if (tp.Control == this)
                                        tp.Title = "Credit Sanction Stage 1 - " + this.CustomerID;
                                }

                                dtDateProp.Value = (DateTime)row[CN.DateProp];
                                txtS1Comment.Text = (string)row[CN.S1Comment];
                                noDependencies1.Value = (int)row[CN.Dependants];

                                // --------DSR 11 Apr 2003 - UAT fix S135
                                // Now loaded from Proposal.MthlyIncome and will be blank for each new proposal.
                                if (row[CN.MonthlyIncome] != DBNull.Value)
                                    txtNetIncome1.Text = ((double)row[CN.MonthlyIncome]).ToString(DecimalPlaces);
                                else
                                    txtNetIncome1.Text = "";

                                if (DBNull.Value != row[CN.AdditionalIncome])
                                    txtAddIncome1.Text = ((decimal)row[CN.AdditionalIncome]).ToString(DecimalPlaces);
                                else
                                    txtAddIncome1.Text = "";

                                //M175 - Fields in Sanction Stages not to be prepopulated
                                if (DBNull.Value != row[CN.AdditionalIncome2])
                                    txtAddIncome2.Text = ((decimal)row[CN.AdditionalIncome2]).ToString(DecimalPlaces);                                
                                else
                                    txtAddIncome2.Text = "";

                                if (DBNull.Value != row[CN.OtherPayments])
                                {
                                    txtOther1.Text = ((double)row[CN.OtherPayments]).ToString(DecimalPlaces);
                                    total += Convert.ToDecimal((double)row[CN.OtherPayments]);
                                }
                                else
                                    txtOther1.Text = "";

                                txtCreditCardNo1.One = (string)row[CN.CCardNo1];
                                txtCreditCardNo1.Two = (string)row[CN.CCardNo2];
                                txtCreditCardNo1.Three = (string)row[CN.CCardNo3];
                                txtCreditCardNo1.Four = (string)row[CN.CCardNo4];

                                if (DBNull.Value != row[CN.Commitments1])
                                {
                                    txtUtilities1.Text = ((decimal)row[CN.Commitments1]).ToString(DecimalPlaces);
                                    total += (decimal)row[CN.Commitments1];
                                }
                                else if (IsDispIncomeChangesApplied)
                                    txtUtilities1.Text = "0.00";
                                else
                                    txtUtilities1.Text = "";

                                if (DBNull.Value != row[CN.Commitments2])
                                {
                                    txtLoans1.Text = ((decimal)row[CN.Commitments2]).ToString(DecimalPlaces);
                                    total += (decimal)row[CN.Commitments2];
                                }
                                else if (IsDispIncomeChangesApplied)
                                    txtLoans1.Text = "0.00";
                                else
                                    txtLoans1.Text = "";

                                if (DBNull.Value != row[CN.Commitments3])
                                {
                                    txtMisc1.Text = ((decimal)row[CN.Commitments3]).ToString(DecimalPlaces);
                                    total += (decimal)row[CN.Commitments3];
                                }
                                else if (IsDispIncomeChangesApplied)
                                    txtMisc1.Text = "0.00";
                                else
                                    txtMisc1.Text = "";

                                if (DBNull.Value != row[CN.AdditionalExpenditure1])
                                {
                                    txtAdditionalExpenditure1.Text = ((decimal)row[CN.AdditionalExpenditure1]).ToString(DecimalPlaces);
                                    total += (decimal)row[CN.AdditionalExpenditure1];
                                }
                                else if (IsDispIncomeChangesApplied)
                                    txtAdditionalExpenditure1.Text = "0.00";
                                else
                                    txtAdditionalExpenditure1.Text = "";

                                if (DBNull.Value != row[CN.AdditionalExpenditure2])
                                {
                                    txtAdditionalExpenditure2.Text = ((decimal)row[CN.AdditionalExpenditure2]).ToString(DecimalPlaces);
                                    total += (decimal)row[CN.AdditionalExpenditure2];
                                }
                                else if (IsDispIncomeChangesApplied)
                                    txtAdditionalExpenditure2.Text = "0.00";
                                else
                                    txtAdditionalExpenditure2.Text = "";

                                this.showcrediticons((string)row[CN.PropResult]);
                                txtTotal1.Text = total.ToString(DecimalPlaces);

                                this.ApplicationStatus = (string)row[CN.ApplicationStatus];
                                string rel = ((string)row[CN.A2Relation]).Trim();
                                if (rel.Length != 0)
                                {
                                    this.LoadApplicantTwoDetails(rel);
                                    foreach (DataRowView r in drpApplicationType.Items)
                                    {
                                        if ((string)r[CN.Code] == ((string)row[CN.A2Relation]).Trim())
                                        {
                                            drpApplicationType.SelectedItem = r;
                                            break;
                                        }
                                    }
                                }

                                cbPurchaseCashLoan.Checked = Convert.ToBoolean(row["PurchaseCashLoan"]);
								chxSpouseWorking.Checked = Convert.ToBoolean(row["IsSpouseWorking"]);
                                /* JPJ 17/3/4 
                                 * Add some extra stuff from the proposal table
                                 * but only if the stage is complete. This will
                                 * overwrite stuff written from current tables */
                                SanctionStage1_Enter(null, null);
                                if (Complete || _Reopening)
                                {
                                    // Date controls are since the date last scored instead of today
                                    this.SetDateFrom(false, ((MainForm)this.FormRoot).tbSanction.StageCleared(SS.S1));

                                    drpEmploymentStat1.DisplayMember = CN.Code;
                                    drpOccupation1.DisplayMember = CN.Code;
                                    drpPayFrequency1.DisplayMember = CN.Code;
                                    drpBankAcctType1.DisplayMember = CN.Code;
                                    drpBank1.DisplayMember = CN.BankCode;

                                    /* Employment Status  char(1) */
                                    //index = drpEmploymentStat1.FindStringExact(((string)row[CN.EmploymentStatus]).Trim());
                                    //if (index >= 0)
                                    //    drpEmploymentStat1.SelectedIndex = index;

                                    ///* Occupation  varchar(2) */
                                    //index = drpOccupation1.FindStringExact(((string)row[CN.Occupation]).Trim());
                                    //if (index >= 0)
                                    //    drpOccupation1.SelectedIndex = index;

                                    /* Pay Frequency char(1) */
                                    //index = drpPayFrequency1.FindStringExact(((string)row[CN.PayFrequency]).Trim());
                                    //if (index >= 0)
                                    //    drpPayFrequency1.SelectedIndex = index;

                                    // Employment payment amount is calculated from the financial monthly income


                                    ///* Date Current Employment Started datetime */
                                    //if ((DateTime)row[CN.DateEmpStart] > DatePicker.MinValue)
                                    //    dtCurrEmpStart1.Value = (DateTime)row[CN.DateEmpStart];
                                    //else
                                    //    dtCurrEmpStart1.Value = DateTime.Today;
                                    //dtCurrEmpStart1.LinkedDatePicker = dtPrevEmpStart1;

                                    ///* Date Previous Employment Started datetime */
                                    //dtPrevEmpStart1.DateFrom = dtCurrEmpStart1.Value;
                                    //if ((DateTime)row[CN.DatePEmpStart] > DatePicker.MinValue)
                                    //    dtPrevEmpStart1.Value = (DateTime)row[CN.DatePEmpStart];
                                    //else
                                    //    dtPrevEmpStart1.Value = dtCurrEmpStart1.Value;

                                    ///* Employment tel no and dial code char(13) & char(8)*/
                                    //txtEmpTelCode1.Text = ((string)row[CN.EmploymentDialCode]).Trim();
                                    //txtEmpTelNum1.Text = ((string)row[CN.EmploymentTelNo]).Trim();

                                    //index = drpBankAcctType1.FindStringExact(((string)row[CN.BankAccountType]).Trim());
                                    //if (index >= 0)
                                    //    drpBankAcctType1.SelectedIndex = index;

                                    ///* Bank code varchar(6)*/
                                    //index = drpBank1.FindStringExact(((string)row[CN.BankCode]).Trim());
                                    //if (index >= 0)
                                    //    drpBank1.SelectedIndex = index;

                                    ///* Date Account Opened datetime */
                                    //if ((DateTime)row[CN.BankAccountOpened] > DatePicker.MinValue)
                                    //{
                                    //    dtBankOpened1.Value = (DateTime)row[CN.BankAccountOpened];
                                    //}

                                    ///* Bank acctno varchar(20)*/
                                    //txtBankAcctNumber1.Text = (string)row[CN.BankAccountNo];

                                    drpEmploymentStat1.DisplayMember = CN.CodeDescription;
                                    drpOccupation1.DisplayMember = CN.CodeDescription;
                                    drpPayFrequency1.DisplayMember = CN.CodeDescription;
                                    drpBank1.DisplayMember = CN.BankName;
                                    drpBankAcctType1.DisplayMember = CN.CodeDescription;
                                }
                                else
                                {
                                    // Date controls are since today instead of the date of proposal
                                    this.SetDateFrom(true, DateTime.Today);
                                }

                                if (Complete)
                                {
                                    // Override with proposal if exists

                                    #region customerdata override
                                    foreach (DataRowView r in drpPropertyType1.Items)
                                    {
                                        if ((string)r[CN.Code] == ((string)row[CN.PropertyType]).Trim())
                                        {
                                            drpPropertyType1.SelectedItem = r;
                                            break;
                                        }
                                    }
                                    //foreach (DataRowView r in drpEthnicGroup1.Items)
                                    //{
                                    //    if ((string)r[CN.Code] == ((string)row[CN.Ethnicity]).Trim())
                                    //    {
                                    //        drpEthnicGroup1.SelectedItem = r;
                                    //        break;
                                    //    }
                                    //}

                                    //foreach (DataRowView r in drpIDSelection1.Items)
                                    //{
                                    //    if ((string)r[CN.Code] == ((string)row[CN.IDType]).Trim())
                                    //    {
                                    //        drpIDSelection1.SelectedItem = r;
                                    //        break;
                                    //    }
                                    //}
                                    foreach (DataRowView r in drpCurrentResidentialStatus1.Items)
                                    {
                                        if (((string)r[CN.Code]).Trim() == ((string)row[CN.ResidentialStatus]).Trim())
                                        {
                                            drpCurrentResidentialStatus1.SelectedItem = r;
                                            break;
                                        }
                                    }
                                    foreach (DataRowView r in drpPrevResidentialStatus1.Items)
                                    {
                                        if (((string)r[CN.Code]).Trim() == ((string)row[CN.PrevResidentialStatus]).Trim())
                                        {
                                            drpPrevResidentialStatus1.SelectedItem = r;
                                            break;
                                        }
                                    }

                                    if (Convert.ToDateTime(row[CN.DateIn]) > DatePicker.MinValue)
                                    {
                                        dtDateInCurrentAddress1.LinkedDatePicker = dtDateInPrevAddress1;
                                        dtDateInCurrentAddress1.LinkedComboBox = drpPrevResidentialStatus1;
                                        dtDateInCurrentAddress1.LinkedLabel = lPrevResidentialStatus1;
                                        dtDateInCurrentAddress1.Value = Convert.ToDateTime(row[CN.DateIn]);
                                    }


                                    dtDateInPrevAddress1.Years = Convert.ToInt32(row[CN.PrevAddYY]);
                                    dtDateInPrevAddress1.Months = Convert.ToInt32(row[CN.PrevAddMM]);

                                    #endregion

                                    #region employment overrride

                                    foreach (DataRowView r in drpEmploymentStat1.Items)
                                    {
                                        if ((string)r[CN.Code] == ((string)row[CN.EmploymentStatus]).Trim())
                                        {
                                            drpEmploymentStat1.SelectedItem = r;
                                            break;
                                        }
                                    }
                                    foreach (DataRowView r in drpOccupation1.Items)
                                    {
                                        //CR 866 Changed CN.Occupation to CN.WorkType
                                        if ((string)r[CN.Code] == ((string)row[CN.Occupation]).Trim())
                                        {
                                            drpOccupation1.SelectedItem = r;
                                            break;
                                        }
                                    }
                                    foreach (DataRowView r in drpPayFrequency1.Items)
                                    {
                                        if ((string)r[CN.Code] == ((string)row[CN.PayFrequency]).Trim())
                                        {
                                            drpPayFrequency1.SelectedItem = r;
                                            break;
                                        }
                                    }

                                    if (Convert.ToDateTime(row[CN.DateEmpStart]) > DatePicker.MinValue)
                                        dtCurrEmpStart1.Value = Convert.ToDateTime(row[CN.DateEmpStart]);


                                    dtPrevEmpStart1.DateFrom = dtCurrEmpStart1.Value;
                                    if (Convert.ToDateTime(row[CN.DatePEmpStart]) > DatePicker.MinValue)
                                        dtPrevEmpStart1.Value = Convert.ToDateTime(row[CN.DatePEmpStart]);


                                    txtEmpTelCode1.Text = ((string)row[CN.EmploymentDialCode]).Trim();
                                    txtEmpTelNum1.Text = ((string)row[CN.EmploymentTelNo]).Trim();

                                    //CR 866 - Thailand scoring [PC]
                                    txtJobTitle1.SelectedValue = row[CN.JobTitle].ToString();
                                    txtOrganisation1.SelectedValue = row[CN.Organisation].ToString();
                                    txtIndustry1.SelectedValue = row[CN.Industry].ToString();
                                    SetError(SetSelectedValue(drpEductation1, row[CN.EducationLevel].ToString()), drpEductation1);
                                    //End CR 866

                                    // --------DSR 11 Apr 2003 - UAT fix S135
                                    // This is populating NET monthly income which we cannot derive from GROSS annual income.
                                    // This is now loaded from Proposal.MthlyIncome instead which will also mean it is blank 
                                    // for each new proposal.
                                    // if(row[CN.AnnualGross]!=DBNull.Value)
                                    // 	   txtNetIncome1.Text = ((double)row[CN.AnnualGross]/12).ToString(DecimalPlaces);
                                    // else
                                    // 	   txtNetIncome1.Text = "";



                                    #endregion

                                    #region bank override
                                    foreach (DataRowView r in drpBankAcctType1.Items)
                                    {
                                        if ((string)r[CN.Code] == ((string)row[CN.BankAccountType]).Trim())
                                        {
                                            drpBankAcctType1.SelectedItem = r;
                                            break;
                                        }
                                    }
                                    foreach (DataRowView r in drpBank1.Items)
                                    {
                                        if ((string)r[CN.BankCode] == ((string)row[CN.BankCode]).Trim())
                                        {
                                            drpBank1.SelectedItem = r;
                                            break;
                                        }
                                    }
                                    if ((DateTime)row[CN.BankAccountOpened] > DatePicker.MinValue)
                                    {
                                        dtBankOpened1.Value = (DateTime)row[CN.BankAccountOpened];
                                    }
                                    txtBankAcctNumber1.Text = (string)row[CN.BankAccountNo];

                                    #endregion

                                    this.SetIncome(true);
                                }

                            }
                        }

                        if (dt.TableName == TN.Agreements)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                foreach (DataRowView r in drpPaymentMethod.Items)
                                {
                                    if ((string)r[CN.Code] == ((string)row[CN.PaymentMethod]).Trim())
                                    {
                                        drpPaymentMethod.SelectedItem = r;
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion

                        #region WorkAddress data
                        //uat478 rdb 15/07/08 i have added work address to DataSet, get date in value here
                        //IP - 18/05/10 - UAT(124) UAT5.2.1.0 Log - Do not set the Current Employment start date to the work address date in.
                        //if (dt.TableName == "WorkAddress")
                        //{
                        //    foreach (DataRow row in dt.Rows)
                        //    {
                        //        if ((DateTime)row[CN.DateIn] > DatePicker.MinValue)
                        //        {
                        //            dtCurrEmpStart1.Value = (DateTime)row[CN.DateIn];
                        //        }
                        //    }
                        //}
                        #endregion


                    }
                    // CR 835 fill the residential data from customer defaults if stage 1 is not compelete
                    if (prop.Tables.IndexOf(TN.CustomerAdditionalDetailsResidential) >= 0 && !Complete)
                    {
                        if (prop.Tables[TN.CustomerAdditionalDetailsResidential].Rows.Count > 0)
                        {
                            #region Residential overwrite
                            DataRow row = prop.Tables[TN.CustomerAdditionalDetailsResidential].Rows[0];

                            if (row == null)
                                return;

                            //current residential status
                            if (!Convert.IsDBNull(row[CN.ResidentialStatus]))
                            {
                                SetError(SetSelectedValue(drpCurrentResidentialStatus1, row[CN.ResidentialStatus].ToString().Trim()), drpCurrentResidentialStatus1);
                                if (drpCurrentResidentialStatus1.SelectedValue == null) drpCurrentResidentialStatus1.SelectedValue = string.Empty;
                            }

                            //previous residential status
                            if (!Convert.IsDBNull(row[CN.PrevResidentialStatus]))
                            {
                                SetError(SetSelectedValue(drpPrevResidentialStatus1, row[CN.PrevResidentialStatus].ToString().Trim()), drpPrevResidentialStatus1);
                                if (drpPrevResidentialStatus1.SelectedValue == null) drpPrevResidentialStatus1.SelectedValue = string.Empty;
                            }

                            //property type
                            if (!Convert.IsDBNull(row[CN.PropertyType]))
                            {
                                SetError(SetSelectedValue(drpPropertyType1, row[CN.PropertyType].ToString().Trim()), drpPropertyType1);
                                if (drpPropertyType1.SelectedValue == null) drpPropertyType1.SelectedValue = string.Empty;

                            }

                            //Monthly Rent
                            if (!Convert.IsDBNull(row[CN.MonthlyRent]))
                                txtMortgage1.Text = ((double)row[CN.MonthlyRent]).ToString(DecimalPlaces);

                            if ((DateTime)row[CN.DateIn] > DatePicker.MinValue)
                            {
                                //70363 row[CN.DateIn] is not getting the most recent address date. This is because when first saved the default value of 10/10/2006 is saved to the database
                                DateTime currentCustomerAddressDate = (DateTime)prop.Tables[TN.Customer].Rows[0]["DateIn"];
                                dtDateInCurrentAddress1.LinkedDatePicker = dtDateInPrevAddress1;
                                dtDateInCurrentAddress1.LinkedComboBox = drpPrevResidentialStatus1;
                                dtDateInCurrentAddress1.LinkedLabel = lPrevResidentialStatus1;
                                dtDateInCurrentAddress1.Value = currentCustomerAddressDate;
                            }

                            dtDateInPrevAddress1.DateFrom = dtDateInCurrentAddress1.Value;
                            if (Convert.IsDBNull(row[CN.PrevDateIn]))
                            {
                                dtDateInPrevAddress1.Value = dtDateInCurrentAddress1.Value;
                            }
                            else if ((DateTime)row[CN.PrevDateIn] > DatePicker.MinValue)
                            {
                                dtDateInPrevAddress1.Value = (DateTime)row[CN.PrevDateIn];
                                PrevAddressBlank = false;
                            }
                            else
                            {
                                dtDateInPrevAddress1.Value = dtDateInCurrentAddress1.Value;
                                PrevAddressBlank = true;
                            }


                            #endregion
                        }
                    }

                    // CR 835 fill the stage 1 financial tab from customer defaults if stage 1 is not complete 
                    if (prop.Tables.IndexOf(TN.CustomerAdditionalDetailsFinancial) >= 0 && !Complete)
                    {

                        // TODO consolidate this routine with existing functionality above
                        if (prop.Tables[TN.CustomerAdditionalDetailsFinancial].Rows.Count > 0)
                        {
                            #region Financial overwrite
                            DataRow row = prop.Tables[TN.CustomerAdditionalDetailsFinancial].Rows[0];

                            total = 0;

                            if (DBNull.Value != row[CN.Commitments1])
                            {
                                txtUtilities1.Text = ((decimal)row[CN.Commitments1]).ToString(DecimalPlaces);
                                total += (decimal)row[CN.Commitments1];
                            }
                            else
                                txtUtilities1.Text = "";

                            if (DBNull.Value != row[CN.Commitments2])
                            {
                                txtLoans1.Text = ((decimal)row[CN.Commitments2]).ToString(DecimalPlaces);
                                total += (decimal)row[CN.Commitments2];
                            }
                            else
                                txtLoans1.Text = "";

                            if (DBNull.Value != row[CN.Commitments3])
                            {
                                txtMisc1.Text = ((decimal)row[CN.Commitments3]).ToString(DecimalPlaces);
                                total += (decimal)row[CN.Commitments3];
                            }
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
                            {
                                txtOther1.Text = ((decimal)row[CN.OtherPayments]).ToString(DecimalPlaces);
                                total += Convert.ToDecimal((decimal)row[CN.OtherPayments]);
                            }
                            else
                                txtOther1.Text = "";


                            if (DBNull.Value != row[CN.AdditionalExpenditure1])
                            {
                                txtAdditionalExpenditure1.Text = ((decimal)row[CN.AdditionalExpenditure1]).ToString(DecimalPlaces);
                                total += (decimal)row[CN.AdditionalExpenditure1];
                            }
                            else
                                txtAdditionalExpenditure1.Text = "";

                            if (DBNull.Value != row[CN.AdditionalExpenditure2])
                            {
                                txtAdditionalExpenditure2.Text = ((decimal)row[CN.AdditionalExpenditure2]).ToString(DecimalPlaces);
                                total += (decimal)row[CN.AdditionalExpenditure2];
                            }
                            else
                                txtAdditionalExpenditure2.Text = "";

                            txtTotal1.Text = total.ToString(DecimalPlaces);

                            //Load credit card no
                            txtCreditCardNo1.One = (string)row[CN.CCardNo1];
                            txtCreditCardNo1.Two = (string)row[CN.CCardNo2];
                            txtCreditCardNo1.Three = (string)row[CN.CCardNo3];
                            txtCreditCardNo1.Four = (string)row[CN.CCardNo4];
                            #endregion
                        }
                    }

                    // CR 835 Nationality and marital status overwrite
                    if (prop.Tables.IndexOf(TN.Customer) >= 0 && !Complete)
                    {
                        if (prop.Tables[TN.Customer].Rows.Count > 0)
                        {
                            DataRow row = prop.Tables[TN.Customer].Rows[0];
                            SetError(SetSelectedValue(drpMaritalStat1, row[CN.MaritalStatus].ToString()), drpMaritalStat1);
                            SetError(SetSelectedValue(drpNationality1, row[CN.Nationality].ToString().Trim()), drpNationality1);
                            noDependencies1.Value = Convert.ToDecimal(row[CN.Dependants]);
                        }
                    }

                    //Comparison using hard coded string - Not critical for Thailand.
                    if (FormParent != null)
                    {
                        if (FormParent.GetType().Name == "BasicCustomerDetails")
                        {
                            if (((BasicCustomerDetails)this.FormParent).title == "MR")
                            {
                                drpSex1.SelectedIndex = 1;
                            }
                            else if (((BasicCustomerDetails)this.FormParent).title == "MRS" || ((BasicCustomerDetails)this.FormParent).title == "MISS" || ((BasicCustomerDetails)this.FormParent).title == "MS")
                            {
                                drpSex1.SelectedIndex = 2;
                            }
                        }
                    }


                }
                this.SetDateFrom(false, dtDateProp.Value); //set date from after loading

            }

            catch (Exception ex)
            {
                Catch(ex, "LoadStage1Details");
            }
        }

        private void AssociateSecondApplicant(string accountNo, string customerID, string relation)
        {

            CustomerSearch cust = new CustomerSearch(accountNo, AccountType, this, relation);
            cust.FormRoot = this.FormRoot;
            cust.FormParent = this.FormParent;
            ((MainForm)FormRoot).AddTabPage(cust, 9);

            ((MainForm)FormRoot).MainTabControl.SelectedIndex++;
        }

        public void LoadAppTwo(string rel)
        {
            Relationship = rel;
            Thread t = new Thread(new ThreadStart(LoadAppTwoThread));
            t.Start();
            t.Join();
            LoadApplicantTwoDetails(rel);
        }

        public void LoadAppTwoThread()
        {
            prop2 = CreditManager.GetProposalStage1(this.CustomerID, this.AccountNo, SM.New, Relationship, out Error);
            if (Error.Length > 0)
                ShowError(Error);
        }

        public void LoadApplicantTwoDetails(string relation)
        {
            try
            {
                Wait();
                Function = "SanctionStage1::LoadApplicantTwoDetails()";

                // Insert the applicant two tab
                // Do this before loading the data to avoid blanking some fields
                tcApplicants.TabPages.Remove(tpAccounts);
                tcApplicants.TabPages.Remove(tpComments);
                if (tcApplicants.TabPages.Contains(tpApp2))
                    tcApplicants.TabPages.Remove(tpApp2);
                tcApplicants.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] { tpApp2, tpAccounts, tpComments });

                //make sure this tab page is at the top
                foreach (Crownwood.Magic.Controls.TabPage tp in ((MainForm)FormRoot).MainTabControl.TabPages)
                {
                    if (tp.Control == this)
                    {
                        ((MainForm)FormRoot).MainTabControl.SelectedTab = tp;
                        break;
                    }
                }

                //this means that the second applicant either does not exist or has not been 
                //linked to the customer via this account
                if (prop2 == null)
                {
                    AssociateSecondApplicant(AccountNo, CustomerID, relation);
                }
                else
                {
                    foreach (DataTable dt in prop2.Tables)
                    {
                        #region customer data
                        if (dt.TableName == TN.Customer)
                        {
                            drpIDSelection2.SelectedIndex = 0;
                            drpTitle2.SelectedIndex = 0;
                            foreach (DataRow row in dt.Rows)		//should only be one row
                            {
                                //M175 - Fields in Sanction Stages not to be prepopulated
                                foreach (DataRowView r in drpIDSelection2.Items)
                                {
                                    if ((string)r[CN.Code] == ((string)row[CN.IDType]).Trim())
                                    {
                                        drpIDSelection2.SelectedItem = r;
                                        break;
                                    }
                                }
                                foreach (DataRowView r in drpTitle2.Items)
                                {
                                    if ((string)r[CN.CodeDescription] == ((string)row[CN.Title]).Trim())
                                    {
                                        drpTitle2.SelectedItem = r;
                                        break;
                                    }
                                }
                                txtFirstName2.Text = (string)row[CN.FirstName];
                                txtLastName2.Text = (string)row[CN.LastName];
                                txtAlias2.Text = (string)row[CN.Alias];
                                dtDOB2.Value = (DateTime)row[CN.DOB];

                                switch ((string)row[CN.Sex])
                                {
                                    case "M": drpSex2.SelectedIndex = 1;
                                        break;
                                    case "F": drpSex2.SelectedIndex = 2;
                                        break;
                                    default: drpSex2.SelectedIndex = 0;
                                        break;
                                }
                                txtMoreRewards2.Text = (string)row[CN.MoreRewardsNo];

                                if ((DateTime)row[CN.EffectiveDate] > DatePicker.MinValue)
                                    dtMoreRewardsDate2.Value = (DateTime)row[CN.EffectiveDate];
                            }
                        }
                        #endregion

                        #region employment data
                        if (dt.TableName == TN.Employment)
                        {
                            drpOccupation2.SelectedIndex = 0;
                            drpEmploymentStat2.SelectedIndex = 0;
                            foreach (DataRow row in dt.Rows)		//should only be one row
                            {
                                foreach (DataRowView r in drpEmploymentStat2.Items)
                                {
                                    if ((string)r[CN.Code] == ((string)row[CN.EmploymentStatus]).Trim())
                                    {
                                        drpEmploymentStat2.SelectedItem = r;
                                        break;
                                    }
                                }

                                foreach (DataRowView r in drpOccupation2.Items)
                                {
                                    //CR 866 Change CN.Occupation to CN.WorkType
                                    if ((string)r[CN.Code] == ((string)row[CN.WorkType]).Trim())
                                    {
                                        drpOccupation2.SelectedItem = r;
                                        break;
                                    }
                                }

                                if ((DateTime)row[CN.DateEmployed] > DatePicker.MinValue)
                                    dtEmploymentStart2.Value = (DateTime)row[CN.DateEmployed];
                                else
                                    dtEmploymentStart2.Value = DateTime.Today;

                                if (row[CN.AnnualGross] != DBNull.Value)
                                    txtNetIncome2.Text = ((double)row[CN.AnnualGross] / 12).ToString(DecimalPlaces);
                                else
                                    txtNetIncome2.Text = "";
                            }
                        }
                        #endregion

                        #region bank data
                        if (dt.TableName == TN.Bank)
                        {
                            foreach (DataRow row in dt.Rows)		//should only be one row
                            {
                                if ((DateTime)row[CN.BankAccountOpened] > DatePicker.MinValue)
                                    dtBankOpened2.Value = (DateTime)row[CN.BankAccountOpened];
                                else
                                    dtBankOpened2.Value = DateTime.Today;
                            }
                        }
                        #endregion

                        #region accountTotals data
                        if (dt.TableName == TN.AccountTotals)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                txtJointCAccounts.Text = ((int)row[CN.CurrentAccounts]).ToString();
                                txtJointSAccounts.Text = ((int)row[CN.SettledAccounts]).ToString();
                            }
                        }
                        #endregion
                    }

                }
                ValidateControl(null, null);
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

        private bool LockAccount()
        {
            if (!AccountLocked)
            {
                if (this.AccountNo.Length != 0)
                {
                    AccountLocked = AccountManager.LockAccount(this.AccountNo, Credential.UserId.ToString(), out Error);
                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                        ReadOnly = !AccountLocked;
                        this.SetReadOnly();
                    }
                }
                else
                    AccountLocked = true;
            }
            return AccountLocked;
        }

        private void HideCreditIcons()
        {
            pictAccept.Visible = false;
            pictRefer.Visible = false;
            pictReject.Visible = false;
        }

        private void showcrediticons(string result)
        {
            switch (result)
            {
                case PR.Referred:
                    pictAccept.Visible = false;
                    pictRefer.Visible = true;
                    pictReject.Visible = false;
                    break;
                case PR.Accepted:
                    pictAccept.Visible = true;
                    pictRefer.Visible = false;
                    pictReject.Visible = false;
                    break;
                case PR.Rejected:
                    pictAccept.Visible = false;
                    pictRefer.Visible = false;
                    pictReject.Visible = true;
                    break;
                default: break;
            }

            if (AT.IsCreditType(AccountType) && //Acct Type Translation DSR 29/9/03
                result == PR.Accepted || result == PR.Rejected)
                menuManualRefer.Enabled = true;
            else
                menuManualRefer.Enabled = false;
        }

        private void SetReadOnly()
        {
            SetMandatoryFields();

            drpPayByGiro1.BackColor = SystemColors.Window;
            txtHolderCAccounts.ReadOnly = true;
            txtHolderCAccounts.BackColor = SystemColors.Window;
            txtHolderSAccounts.ReadOnly = true;
            txtHolderSAccounts.BackColor = SystemColors.Window;
            txtJointCAccounts.ReadOnly = true;
            txtJointCAccounts.BackColor = SystemColors.Window;
            txtJointSAccounts.ReadOnly = true;
            txtJointSAccounts.BackColor = SystemColors.Window;
            btnComplete.Enabled = !Complete;
            menuSave.Enabled = !ReadOnly;
            btnComplete.Enabled = menuComplete.Enabled = !ReadOnly;
            //menuReopen.Enabled = ReadOnly;
            menuReopen.Enabled = allowReopenS1 == false? allowReopenS1 : ReadOnly;      //#10477
            menuPrintRFDetails.Enabled = ReadOnly;
            drpApplicationType.Enabled = !ReadOnly;
            tvRFCategory.Enabled = !ReadOnly;
            cbPurchaseCashLoan.Enabled = !ReadOnly;

            SetFieldBackground();
        }

        private void SetFieldBackground()
        {
            if (ReadOnly)
            {
                foreach (object c in inputFields)
                {
                    ((Control)((DictionaryEntry)c).Value).BackColor = SystemColors.Control;
                }
            }
        }

        private void UnHighliteControl(Control c)
        {
            Control found = null;
            if (c.Parent != null)
            {
                foreach (Control child in c.Parent.Controls)
                {
                    if (child.Name == c.Name + "Border")
                    {
                        found = child;
                        break;
                    }
                }
                c.Parent.Controls.Remove(found);
            }
        }

        private void HighliteControl(Control c)
        {
            bool found = false;
            if (c.Parent != null) //prevent exception
            {
                foreach (Control child in c.Parent.Controls)
                {
                    if (child.Name == c.Name + "Border")
                    {
                        found = true;	/* control is already highlited */
                        break;
                    }
                }
            }

            if (!found)
            {
                STL.PL.HighliteBox h = new STL.PL.HighliteBox();
                h.Border = 6;
                h.Alpha = 50;
                h.Color = SystemColors.Highlight;
                h.TabStop = false;
                h.TabIndex = 0;
                h.Name = c.Name + "Border";

                if (c.GetType().Name == "DatePicker")
                    ((DatePicker)c).Highlite(h);
                else
                {
                    if (c.Parent != null)
                    {
                        c.Parent.Controls.Add(h);
                    }
                    h.Location = c.Location;
                    h.Size = c.Size;
                }
                //h.Dispose();
            }
        }

        private void SetCreditLimit(decimal credit, ref bool referDeclined, out bool acctCancelled)
        {
            acctCancelled = false;
            // Must write this to DB here in case it was set back to zero
            CustomerManager.SetCreditLimit(txtCustomerID.Text, credit);

            if (CustomerScreen != null)
            {
                CustomerScreen.RFLimit = credit;

            }

            if (lShowResult.Enabled)
            {
                textCredit.Text = credit.ToString(DecimalPlaces);
                labelCredit.Visible = true;
                textCredit.Visible = true;
                textCredit.BackColor = SystemColors.Window;

                if (credit == 0 /*&& Config.CountryCode != "J"*/ && !referDeclined)
                {
                    RFCreditRefused refuse = new RFCreditRefused(this.AccountNo, FormRoot);

                    //Livewire 69230 use allowConversionToHP property to determine if the 'convert to HP' option should be available in the 'RFCreditRefused' dialog box
                    if (allowConversionToHP == false)
                    {
                        refuse.rbConvert.Visible = false;
                        refuse.rbConvert.Checked = false;
                        //Livewire 69230 Upon AA's advice ManualRefer option set to checked provided it is visible
                        if (refuse.rbManualRefer.Visible)
                        {
                            refuse.rbManualRefer.Checked = true;
                        }
                        else
                        {
                            refuse.rbCancel.Checked = true;
                        }
                    }

                    refuse.ShowDialog();
                    if (refuse.rbCancel.Checked)
                    {
                        acctCancelled = true;
                        decimal balance = 0;
                        bool outstPayments = false;
                        bool cancel = true;

                        AccountManager.CheckAccountToCancel(this.AccountNo, Config.CountryCode, ref balance, ref outstPayments, out Error);
                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            if (outstPayments)
                            {
                                OutstandingPayment op = new OutstandingPayment(FormRoot);
                                op.ShowDialog();
                                cancel = op.rbCancel.Checked;
                                op.Dispose();
                            }
                            if (cancel)
                            {
                                AccountManager.CancelAccount(this.AccountNo, this.CustomerID, Convert.ToInt16(Config.BranchCode),
                                    cancellationCode, balance, Config.CountryCode, String.Empty, 0, out Error);
                                if (Error.Length > 0)
                                    ShowError(Error);
                            }
                        }
                    }
                    else
                    {
                        if (refuse.rbConvert.Checked)
                        {
                            AccountManager.ConvertRFToHP(this.AccountNo, this.CustomerID, Config.CountryCode, dtDateProp.Value, out Error);
                            if (Error.Length > 0)
                                ShowError(Error);
                            else
                            {
                                tcApp1.TabPages.Remove(tpRFProducts);
                                labelCredit.Visible = textCredit.Visible = false;
                                this.AccountType = AT.HP;
                                this.SanctionStage1_Enter(null, null);
                            }
                        }
                        else
                        {
                            if (refuse.rbManualRefer.Checked)
                            {
                                CreditManager.ManualRefer(CustomerID, AccountNo, dtDateProp.Value, true,false, out Error);
                                if (Error.Length > 0)
                                    ShowError(Error);
                                else
                                    SanctionStage1_Enter(null, null);
                            }
                        }
                    }
                    HideCreditIcons();
                    refuse.Dispose();
                }
            }
        }

        private void SetScoreResult()
        {
            bool display = true;
            decimal credit = 0;
            string result = String.Empty;
            string refCode = String.Empty;
            decimal points = 0;
            string bureauFailure = String.Empty;
            decimal balance = 0;
            bool outstPayments = false;
            bool cancel = true;
            string newBand = String.Empty;
            bool referDeclined = false;
            //decimal MaxStoreCardValue = 0; //todo get this back from the score proposal routine.
            _preventHideSanctionStatus = true;

            CreditManager.ScoreProposal(Config.CountryCode, this.AccountNo, this.AccountType, this.CustomerID, dtDateProp.Value, Convert.ToInt16(Config.BranchCode), ref referDeclined, Credential.UserId, out newBand,
                out refCode, out points, out credit, out result, out bureauFailure, out referralReasons, out Error); //IP - 14/03/11 - #3314 - CR1245 - Returning referral reasons
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                //IP - 23/03/11 - CR1245 - Displaying referral reasons on the referral/rejection popup based on Country Parameter.
                if (Convert.ToBoolean(Country[CountryParameterNames.ReasonsReferPopup]) == false)
                {
                    referralReasons = string.Empty;
                }

                if (bureauFailure.Length > 0)
                {
                    ShowWarning(bureauFailure);
                }

                if (AccountType == AT.ReadyFinance && credit == 0 && !referDeclined)
                {
                    display = false;
                }
                else
                {
                    display = true;
                }
            }

            if ((bool)Country[CountryParameterNames.ManualRefer] &&
                result == PR.Rejected &&
                !(this.AccountType == AT.ReadyFinance && credit == 0) &&
                lShowResult.Enabled)
            {
                // Allow a rejected application to be manually referred
                if (DialogResult.Yes == ShowInfo("M_MANUALREFER", MessageBoxButtons.YesNo))
                {
                    CreditManager.ManualRefer(this.CustomerID, this.AccountNo, dtDateProp.Value, true, false, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    result = PR.Referred;
                }
            }

            if (this.AccountType == AT.ReadyFinance && result == PR.Rejected)
            {

                // M10 - Display pop-up to Convert account to HP if Rejected RF
                credit = 0;
                result = PR.Referred;
                display = false;
            }

            if (result == PR.Rejected &&
                !(this.AccountType == AT.ReadyFinance && credit == 0))
            {
                AccountManager.CheckAccountToCancel(this.AccountNo, Config.CountryCode, ref balance, ref outstPayments, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    if (outstPayments)
                    {
                        OutstandingPayment op = new OutstandingPayment(FormRoot);
                        op.ShowDialog();
                        cancel = op.rbCancel.Checked;
                        op.Dispose();
                    }
                    if (cancel)
                    {
                        AccountManager.CancelAccount(this.AccountNo, this.CustomerID, Convert.ToInt16(Config.BranchCode),
                            cancellationCode, balance, Config.CountryCode, String.Empty, 0, out Error);
                        if (Error.Length > 0)
                            ShowError(Error);
                    }
                }
            }

			string custId = this.CustomerID;
            // Calculate and save MMI value for customer based on customer score MMI matrix configuration percentage of Desposible Income.
            CreditManager.SaveCustomerMmi(custId, Credential.UserId, "Score", out Error); 
            if (Error.Length > 0)
                ShowError(Error);
            //reload the sanction status control
            ((MainForm)this.FormRoot).tbSanction.Load(this.allowConversionToHP, this.CustomerID, this.dtDateProp.Value, this.AccountNo, this.AccountType, this.ScreenMode);
            CurrentStatus = ((MainForm)this.FormRoot).tbSanction.CurrentStatus;
            //((MainForm)this.FormRoot).tbSanction.SetCurrentStage(SS.S1);
            Complete = ((MainForm)this.FormRoot).tbSanction.ReadOnly(SS.S1);
            ReadOnly = !(!Complete && CurrentStatus == "0" && ((MainForm)this.FormRoot).tbSanction.HoldProp);
            this._mandateExists = (this.drpPayByGiro1.SelectedIndex == 2);
            this.SetReadOnly();

            if (lShowResult.Enabled)
                this.showcrediticons(result);
            else
                HideCreditIcons();

            var acctCancelled = false;
            var updatingGuiInAsyncCall = false;
            if (this.AccountType == AT.ReadyFinance || this.AccountType == AT.StoreCard)
            {
                SetCreditLimit(credit, ref referDeclined, out acctCancelled);

                //If Customer intends to take out a Cash Loan & qualifies as a New Cash Loan Customer
                //then we need to force them to the UW (Underwriter Referral Screen)
                if (this.AccountType == AT.ReadyFinance)
                {
                    if (cbPurchaseCashLoan.Checked && display)
                    {
                        updatingGuiInAsyncCall = true;

                        CashLoanNewCustomerManualRefer(CustomerID, response =>
                        {
                            if (response.LoanQual == "Y" && response.Customer.CashLoanNew)
                            {
                                var tmpResult = string.Empty;

                                //Refer and force to UW screen
                                if (result != PR.Rejected)
                                {
                                    CreditManager.ManualRefer(this.CustomerID, this.AccountNo, dtDateProp.Value, true, true, out Error);
                                    if (Error.Length > 0)
                                        ShowError(Error);
                                    tmpResult = PR.Referred;
                                }

                                if (!acctCancelled)
                                {
                                    if (referralReasons == string.Empty)
                                    {
                                        referralReasons += "New Cash Loan Customer";
                                    }
                                    else
                                    {
                                        referralReasons += ", New Cash Loan Customer";
                                    }

                                    DisplayResult(tmpResult);
                                }

                                if (CustomerScreen != null)
                                {
                                    CustomerScreen.RefreshData();
                                    this.txtCustomerID.Focus();
                                }
                            }
                            else
                            {
                                int noPrintsCL = 0;
                                if (result == PR.Accepted)
                                {
                                    // Jamaica Issue 169
                                    // RF T&C only printed for Accepted accounts
                                    this.PrintRFTerms(CreateBrowserArray(1)[0], AccountNo, CustomerID, ref noPrintsCL);
                                }
                            }
                        });

                    }
                }

                int noPrints = 0;
                if (!updatingGuiInAsyncCall)
                {
                    if (result == PR.Accepted)
                    {
                        // Jamaica Issue 169
                        // RF T&C only printed for Accepted accounts
                        this.PrintRFTerms(CreateBrowserArray(1)[0], AccountNo, CustomerID, ref noPrints);
                    }
                }

            }
            else
            {
                // we only print docs for non-RF because RF account
                // will probably not have any lineitems on then when
                // being sanctioned
                if (result != PR.Rejected) //only print if not rejected
                {
                    XmlNode lineItems = AccountManager.GetLineItems(this.AccountNo, 1, this.AccountType, Config.CountryCode, Convert.ToInt16(Config.BranchCode), out Error);
                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                    }

                    if (lineItems != null)
                    {
                        // 5.1 uat294 18/12/07 rdb amending IF so that kit product agreements will also be printed
                        //if ((lineItems.SelectNodes("//Item[@Type='Stock' and @Quantity != '0']").Count > 0))
                        if ((lineItems.SelectNodes("//Item[@Type='Stock' and @Quantity != '0']").Count > 0) ||
                            lineItems.SelectNodes("//Item[@Type='Kit']").Count > 0)
                        {
                            PrintAgreementDocuments(this.AccountNo, this.AccountType, this.CustomerID, false, false, 0, 0, lineItems, 1, this, true, 0, 0);
                        }
                    }

                    /*if (result == PR.Accepted && 
                        (bool)Country[CountryParameterNames.PrizeVouchersActive] &&
                        this.AccountType != AT.ReadyFinance)
                    {
                        PrintPrizeVouchers(this.AccountNo, this.CashPrice, 0);
                    }*/
                }
            }

            //if (Config.CountryCode == "J")
            //    display = true;

            if (updatingGuiInAsyncCall)
                return;

            if (display && !acctCancelled)
                DisplayResult(result);

            if (CustomerScreen != null)
            {
                CustomerScreen.RefreshData();
                // 5.0.0 uat338 rdb firing of form leave event seems to be prevented because
                // we call routines on another form
                // try setting focus to a control on this form
                this.txtCustomerID.Focus();
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

        private decimal GetDependentSpendFactor()
        {
            try
            {
                string Error = string.Empty;
                decimal depSpendFactor = CreditManager.GetDependentSpendFactor(noDependencies1.Value,out Error);

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

                if (drpMaritalStat1.SelectedValue != null)
                {
                    if (drpMaritalStat1.SelectedValue.Equals("M") && chxSpouseWorking.Checked)
                    {
                        mortgage = ((rentFactor / 100) * mortgage);
                    }
                }

                disposable = monthlyIncome - (
                                                mortgage
                                                + (noDependencies1.Value * (dependentSpendFactor / 100) * monthlyIncome)
                                                + ((applicantSpendFactor / 100) * monthlyIncome)
                                             );
            }
            else
            {
                disposable = monthlyIncome - commitments - mortgage;
            }

            txtDisposable.Text = (disposable).ToString(DecimalPlaces);
        }

        private void SetIncome(bool fromFinancial)
        {
            string weekly = "W";
            string fortnightly = "F";
            string frequency = ((string)((DataRowView)drpPayFrequency1.SelectedItem)[CN.Code]).Trim();
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

        //private void SetORFRestrictions()
        //{
        //    //if(AccountType==AT.ReadyFinance)
        //    //{
        //    //	this.SetPayByGiro();
        //    //}
        //}

        private void SetMandatoryGiro()
        {
            // The Giro fields can be entered when the PayByGiro combo says 'YES'
            // except when the mandate already exists in which case the combo
            // will say 'YES' but none of the Giro fields can be changed.

            // Bank group title
            if (this._mandateExists)
            {
                this.groupBox4.Text = GetResource("M_BANKDETAILSGIRO");
            }
            else
            {
                this.groupBox4.Text = GetResource("M_BANKDETAILS");
            }

            // The combo box and bank fields can be changed if the mandate does not exist already
            drpPayByGiro1.Enabled = (!ReadOnly && !this._mandateExists); // && AccountType != AT.ReadyFinance);
            lPayByGiro1.Enabled = drpPayByGiro1.Enabled;

            // The giro fields can be changed if the combo box can be changed and it says 'YES'
            bool enableGiro = (drpPayByGiro1.Enabled && this.drpPayByGiro1.SelectedIndex == 2);
            drpGiroDueDate1.Enabled = enableGiro;
            lGiroDueDate1.Enabled = enableGiro;
            txtBankAccountName1.Enabled = enableGiro;
            lBankAccountName1.Enabled = enableGiro;

            if (enableGiro)
            {
                // Set mandatory giro fields
                // These fields are always mandatory when set to 'YES'
                mandatoryFields["drpGiroDueDate1"] = drpGiroDueDate1;
                mandatoryFields["txtBankAccountName1"] = txtBankAccountName1;
                HighliteControl(drpGiroDueDate1);
                HighliteControl(txtBankAccountName1);
            }
            else
            {
                // Clear mandatory giro fields when not enabled
                mandatoryFields["drpGiroDueDate1"] = null;
                mandatoryFields["txtBankAccountName1"] = null;
                UnHighliteControl(drpGiroDueDate1);
                UnHighliteControl(txtBankAccountName1);
                errorProvider1.SetError(drpGiroDueDate1, String.Empty);
                errorProvider1.SetError(txtBankAccountName1, String.Empty);
                inError.Remove(drpGiroDueDate1.Name);
                inError.Remove(txtBankAccountName1.Name);
            }

            // Some other bank fields cannot be enabled with an existing mandate
            bool enableBank = (!this._mandateExists && !ReadOnly);
            drpBank1.Enabled = enableBank;
            lBank1.Enabled = enableBank;
            txtBankAcctNumber1.Enabled = enableBank;
            lBankAcctNumber1.Enabled = enableBank;

            if (!enableBank)
            {
                // This routine is called at the end of SetMandatoryFields()
                // so only needs to clear these fields when not enabled
                mandatoryFields["drpBank1"] = null;
                mandatoryFields["txtBankAcctNumber1"] = null;
                UnHighliteControl(drpBank1);
                UnHighliteControl(txtBankAcctNumber1);
                errorProvider1.SetError(drpBank1, String.Empty);
                errorProvider1.SetError(txtBankAcctNumber1, String.Empty);
                inError.Remove(drpBank1.Name);
                inError.Remove(txtBankAcctNumber1.Name);
            }
        }

        private void DisplayResult(string result)
        {
            if (lShowResult.Enabled)
            {
                if (result == PR.Referred)
                    ShowInfo("M_REFER", new object[] { referralReasons }); //IP - 14/03/11 - #3314 - CR1245 - Display the reasons for referral
                else if (result == PR.Rejected)
                    ShowInfo("M_REJECT", new object[] { referralReasons }); //IP - 15/03/11 - #3314 - CR1245 - Display the reasons for rejection plus any referral reasons
                else if (result == PR.Accepted && this.AccountType == AT.ReadyFinance)
                    ShowInfo("M_ACCEPTRF", new object[] { textCredit.Text });
                else if (result == PR.Accepted)
                    ShowInfo("M_ACCEPT");
            }
        }

        private void SetDateFrom(bool today, DateTime dateFrom)
        {
            // The user control calculates Years and Months using DateFrom

            if (today)
            {

                // DSR 24 Oct 2002 - UAT fixes J7 and J11
                // When the screen initially opens ensure the default dates
                // on the control properties are reset to today.
                // (Unfortunately this has to be be repeated for each instance of
                //  this control on each form instead of just in the user control.)
                this.dtDateInCurrentAddress1.DateFrom = dateFrom;
                this.dtDateInCurrentAddress1.Value = dateFrom;
                this.dtDateInPrevAddress1.DateFrom = dateFrom;
                this.dtDateInPrevAddress1.Value = dateFrom;
                this.dtCurrEmpStart1.DateFrom = dateFrom;
                //this.dtCurrEmpStart1.Value = dateFrom; //IP - 19/05/10 - UAT(125) UAT5.2.1.0 Log - There is always an employment record therefore use the dateemployed from the record.
                this.dtPrevEmpStart1.DateFrom = dateFrom;
                this.dtPrevEmpStart1.Value = DateTime.Now;
                this.dtBankOpened1.DateFrom = dateFrom;
                this.dtBankOpened1.Value = DateTime.Now;

                this.dtEmploymentStart2.DateFrom = dateFrom;
                this.dtEmploymentStart2.Value = dateFrom;
                this.dtBankOpened2.DateFrom = dateFrom;
                this.dtBankOpened2.Value = dateFrom;
                this.txtAge1.Text = this.CalculateAge(dateFrom, dtDOB1.Value);
                this.txtAge2.Text = this.CalculateAge(dateFrom, dtDOB2.Value);
            }
            else
            {
                // The DateProp should be used when Stage One is complete,
                // otherwise the current date should be used.
                ////IP - 19/05/10 - UAT(125) UAT5.2.1.0 Log - Commented out the below as was incorrectly setting the Months fields
                //this.dtDateInCurrentAddress1.DateFrom = dateFrom;
                //this.dtCurrEmpStart1.DateFrom = dateFrom;
                //this.dtBankOpened1.DateFrom = dateFrom;
                //this.dtEmploymentStart2.DateFrom = dateFrom;
                //this.dtBankOpened2.DateFrom = dateFrom;
                this.txtAge1.Text = this.CalculateAge(dateFrom, dtDOB1.Value);
                this.txtAge2.Text = this.CalculateAge(dateFrom, dtDOB2.Value);
            }

            //IP - 15/12/11 - #8783 - LW74336 - Dates were re-set to todays date when selecting 'S2' then 'S1' when 'S1' was open.
            if (prop != null)
            {
                if (prop.Tables[TN.Bank].Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(prop.Tables[TN.Bank].Rows[0][CN.BankAccountOpened]) && Convert.ToDateTime(prop.Tables[TN.Bank].Rows[0][CN.BankAccountOpened]) > new DateTime(1900, 1, 1))
                    {
                        this.dtBankOpened1.Value = Convert.ToDateTime(prop.Tables[TN.Bank].Rows[0][CN.BankAccountOpened]);
                    }
                }

                if (prop.Tables[TN.Employment].Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(prop.Tables[TN.Employment].Rows[0][CN.PrevDateEmployed]) && Convert.ToDateTime(prop.Tables[TN.Employment].Rows[0][CN.PrevDateEmployed]) > new DateTime(1900, 1, 1))
                    {
                        this.dtPrevEmpStart1.DateFrom = dtCurrEmpStart1.Value;
                        this.dtPrevEmpStart1.Value = Convert.ToDateTime(prop.Tables[TN.Employment].Rows[0][CN.PrevDateEmployed]);
                    }
                }


                if (prop.Tables[TN.Customer].Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(prop.Tables[TN.Customer].Rows[0][CN.DateIn]) && Convert.ToDateTime(prop.Tables[TN.Customer].Rows[0][CN.DateIn]) > new DateTime(1900, 1, 1))
                    {
                        dtDateInCurrentAddress1.Value = Convert.ToDateTime(prop.Tables[TN.Customer].Rows[0][CN.DateIn]);
                        dtDateInPrevAddress1.DateFrom = dtDateInCurrentAddress1.Value;
                    }

                    if (Convert.IsDBNull(prop.Tables[TN.Customer].Rows[0][CN.PrevDateIn]))
                    {
                        dtDateInPrevAddress1.Value = dtDateInCurrentAddress1.Value;
                    }
                    else if (!Convert.IsDBNull(prop.Tables[TN.Customer].Rows[0][CN.PrevDateIn]) && Convert.ToDateTime(prop.Tables[TN.Customer].Rows[0][CN.PrevDateIn]) > new DateTime(1900, 1, 1))
                    {
                        dtDateInPrevAddress1.Value = Convert.ToDateTime(prop.Tables[TN.Customer].Rows[0][CN.PrevDateIn]);
                    }
                    else
                    {
                        dtDateInPrevAddress1.Value = dtDateInCurrentAddress1.Value;
                    }

                }

            }
        }

        private string CalculateAge(DateTime dateFrom, DateTime DOB)
        {

            if (dateFrom == Date.blankDate)  //uat(4.3) - 155 //IP - 22/02/10 - CR1072 - UAT(155) - Sanction Fixes from 4.3 - Merge
                dateFrom = ServerTime.Request();

            int y = dateFrom.Year - DOB.Year;
            int m = dateFrom.Month - DOB.Month;
            int d = dateFrom.Day - DOB.Day;
            if (d < 0) m--;
            if (m < 0) y--;
            return y.ToString();
        }


        /// <summary>
        /// This method will store all the input fields and their labels in a hash table and then
        /// display them or not according to country specific requirements stored
        /// in the database
        /// </summary>
        private void SetMandatoryFields()
        {
            Function = "SetMandatoryFields()";

            #region Input field caching
            inputFields = new Hashtable();
            mandatoryFields = new Hashtable();
            visibleFields = new Hashtable();
            inError = new Hashtable();

            //Personal data - app1
            inputFields["drpIDSelection1"] = this.drpIDSelection1;
            inputFields["lIDSelection1"] = this.lIDSelection1;

            inputFields["dtDOB1"] = this.dtDOB1;
            inputFields["lDOB1"] = this.lDOB1;

            inputFields["txtAge1"] = this.txtAge1;
            inputFields["lAge1"] = this.lAge1;

            inputFields["drpSex1"] = this.drpSex1;
            inputFields["lSex1"] = this.lSex1;

            inputFields["noDependencies1"] = this.noDependencies1;
            inputFields["lDependencies1"] = this.lDependencies1;

            inputFields["drpMaritalStat1"] = this.drpMaritalStat1;
            inputFields["lMaritalStat1"] = this.lMaritalStat1;

            inputFields["lblIsSpouseWorking"] = this.lblIsSpouseWorking;
            inputFields["chxSpouseWorking"] = this.chxSpouseWorking;

            inputFields["drpEthnicGroup1"] = this.drpEthnicGroup1;
            inputFields["lEthnicGroup1"] = this.lEthnicGroup1;

            inputFields["drpNationality1"] = this.drpNationality1;
            inputFields["lNationality1"] = this.lNationality1;

            inputFields["txtMoreRewards1"] = this.txtMoreRewards1;
            inputFields["lMoreRewards1"] = this.lMoreRewards1;

            inputFields["dtMoreRewardsDate1"] = this.dtMoreRewardsDate1;
            inputFields["lMoreRewardsDate1"] = this.lMoreRewardsDate1;

            //Residential data - app1
            inputFields["drpPropertyType1"] = this.drpPropertyType1;
            inputFields["lPropertyType1"] = this.lPropertyType1;

            inputFields["dtDateInCurrentAddress1"] = this.dtDateInCurrentAddress1;
            inputFields["lDateInCurrentAddress1"] = this.dtDateInCurrentAddress1;

            inputFields["drpCurrentResidentialStatus1"] = this.drpCurrentResidentialStatus1;
            inputFields["lCurrentResidentialStatus1"] = this.lCurrentResidentialStatus1;

            // 5.1 uat118 rdb 12/11/07 these prevResidentialStatus fields should only be mandatory
            // if length at current addess is less than (decimal)Country[CountryParameterNames.SanctionMinYears])

            if (dtDateInCurrentAddress1.Years < (decimal)Country[CountryParameterNames.SanctionMinYears])
            {
                inputFields["dtDateInPrevAddress1"] = this.dtDateInPrevAddress1;
                inputFields["lDateInPrevAddress1"] = this.dtDateInPrevAddress1;

                inputFields["drpPrevResidentialStatus1"] = this.drpPrevResidentialStatus1;
                inputFields["lPrevResidentialStatus1"] = this.lPrevResidentialStatus1;
            }

            //Employment data - app1
            inputFields["drpEmploymentStat1"] = this.drpEmploymentStat1;
            inputFields["lEmploymentStat1"] = this.lEmploymentStat1;

            inputFields["drpOccupation1"] = this.drpOccupation1;
            inputFields["lOccupation1"] = this.lOccupation1;

            inputFields["drpOccupation2"] = this.drpOccupation2;
            inputFields["lOccupation2"] = this.lOccupation2;

            inputFields["drpPayFrequency1"] = this.drpPayFrequency1;
            inputFields["lPayFrequency1"] = this.lPayFrequency1;

            inputFields["txtIncome"] = this.txtIncome;
            inputFields["lIncome"] = this.lIncome;

            inputFields["txtEmpTelCode1"] = this.txtEmpTelCode1;
            inputFields["lEmpTelCode1"] = this.lEmpTelCode1;

            inputFields["txtEmpTelNum1"] = this.txtEmpTelNum1;
            inputFields["lEmpTelNum1"] = this.lEmpTelNum1;

            inputFields["dtCurrEmpStart1"] = this.dtCurrEmpStart1;
            inputFields["lCurrEmpStart1"] = this.dtCurrEmpStart1;

            // 5.1 uat118 rdb 12/11/07
            if (dtCurrEmpStart1.Years < (decimal)Country[CountryParameterNames.SanctionMinYears])
            {
                inputFields["dtPrevEmpStart1"] = this.dtPrevEmpStart1;
                inputFields["lPrevEmpStart1"] = this.dtPrevEmpStart1;
            }

            //Financial data = app1
            inputFields["txtNetIncome1"] = this.txtNetIncome1;
            inputFields["lNetIncome1"] = this.lNetIncome1;

            inputFields["txtAddIncome1"] = this.txtAddIncome1;
            inputFields["lAddIncome1"] = this.lAddIncome1;

            inputFields["txtMortgage1"] = this.txtMortgage1;
            inputFields["lMortgage1"] = this.lMortgage1;

            inputFields["txtMisc1"] = this.txtMisc1;
            inputFields["lMisc1"] = this.lMisc1;

            inputFields["txtUtilities1"] = this.txtUtilities1;
            inputFields["lUtilities1"] = this.lUtilities1;

            inputFields["txtLoans1"] = this.txtLoans1;
            inputFields["lLoans1"] = this.lLoans1;

            inputFields["txtOther1"] = this.txtOther1;
            inputFields["lOther1"] = this.lOther1;

            inputFields["txtAdditionalExpenditure1"] = this.txtAdditionalExpenditure1;
            inputFields["lAdditionalExpenditure1"] = this.lAdditionalExpenditure1;

            inputFields["txtAdditionalExpenditure2"] = this.txtAdditionalExpenditure2;
            inputFields["lAdditionalExpenditure2"] = this.lAdditionalExpenditure2;

            inputFields["txtTotal1"] = this.txtTotal1;
            inputFields["lTotal1"] = this.lTotal1;

            if (Convert.ToBoolean(Country[CountryParameterNames.ApplyNewDIChanges]))
            {
                groupBox3.Visible = false;
                if (mandatoryFieldsDS != null)
                {
                    foreach (DataTable dt in mandatoryFieldsDS.Tables)
                    {
                        if (dt.TableName == "Fields")
                        {
                            string[] controlName = { "txtLoans1", "txtMisc1", "txtUtilities1" };
                            foreach (DataRow row in dt.Rows)
                            {
                                if(Array.Exists(controlName, element => element == row["control"].ToString()))
                                {
                                    row["mandatory"] = 0;
                                }
                            }
                        }
                    }
                }
            }
            else
                groupBox3.Visible = true;


            inputFields["drpBank1"] = this.drpBank1;
            inputFields["lBank1"] = this.lBank1;

            inputFields["dtBankOpened1"] = this.dtBankOpened1;
            inputFields["lBankOpened1"] = this.dtBankOpened1;

            inputFields["drpBankAcctType1"] = this.drpBankAcctType1;
            inputFields["lBankAcctType1"] = this.lBankAcctType1;

            inputFields["txtBankAcctNumber1"] = this.txtBankAcctNumber1;
            inputFields["lBankAcctNumber1"] = this.lBankAcctNumber1;

            inputFields["drpPayByGiro1"] = this.drpPayByGiro1;
            inputFields["lPayByGiro1"] = this.lPayByGiro1;

            inputFields["txtCreditCardNo1"] = this.txtCreditCardNo1;
            inputFields["lCreditCardNo1"] = this.lCreditCardNo1;

            inputFields["txtBankAccountName1"] = this.txtBankAccountName1;
            inputFields["lBankAccountName1"] = this.lBankAccountName1;

            inputFields["drpGiroDueDate1"] = this.drpGiroDueDate1;
            inputFields["lGiroDueDate1"] = this.lGiroDueDate1;

            inputFields["drpPaymentMethod"] = this.drpPaymentMethod;
            inputFields["lPayMethod"] = this.lPayMethod;

            //Personal data - app2
            inputFields["drpIDSelection2"] = this.drpIDSelection2;
            inputFields["lIDSelection2"] = this.lIDSelection2;

            inputFields["drpTitle2"] = this.drpTitle2;
            inputFields["lTitle2"] = this.lTitle2;

            inputFields["txtFirstName2"] = this.txtFirstName2;
            inputFields["lFirstName2"] = this.lFirstName2;

            inputFields["txtLastName2"] = this.txtLastName2;
            inputFields["lLastName2"] = this.lLastName2;

            inputFields["txtAlias2"] = this.txtAlias2;
            inputFields["lAlias2"] = this.lAlias2;

            inputFields["dtDOB2"] = this.dtDOB2;
            inputFields["lDOB2"] = this.lDOB2;

            inputFields["txtAge2"] = this.txtAge2;
            inputFields["lAge2"] = this.lAge2;

            inputFields["drpSex2"] = this.drpSex2;
            inputFields["lSex2"] = this.lSex2;

            inputFields["txtMoreRewards2"] = this.txtMoreRewards2;
            inputFields["lMoreRewards2"] = this.lMoreRewards2;

            inputFields["dtMoreRewardsDate2"] = this.dtMoreRewardsDate2;
            inputFields["lMoreRewardsDate2"] = this.lMoreRewardsDate2;

            //Employment data - app2
            inputFields["drpEmploymentStat2"] = this.drpEmploymentStat2;
            inputFields["lEmploymentStat2"] = this.lEmploymentStat2;

            inputFields["dtEmploymentStart2"] = this.dtEmploymentStart2;
            inputFields["lEmploymentStart2"] = this.dtEmploymentStart2;

            //Financial data - app2
            inputFields["txtNetIncome2"] = this.txtNetIncome2;
            inputFields["lNetIncome2"] = this.lNetIncome2;

            inputFields["txtAddIncome2"] = this.txtAddIncome2;
            inputFields["lAddIncome2"] = this.lAddIncome2;

            inputFields["dtBankOpened2"] = this.dtBankOpened2;
            inputFields["lBankOpened2"] = this.dtBankOpened2;

            inputFields["txtRFCategory"] = this.txtRFCategory;
            inputFields["lRFCategory"] = this.lRFCategory;

            inputFields["tvRFCategory"] = this.tvRFCategory;

            // Stage 1 Comment
            inputFields["txtNewS1Comment"] = this.txtNewS1Comment;
            inputFields["lNewS1Comment"] = this.lNewS1Comment;


            //CR 866 
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

            //End CR 866


            #endregion

            #region retrieve mandatory fields

            if (mandatoryFieldsDS != null)
            {
                foreach (DataTable dt in mandatoryFieldsDS.Tables)
                {
                    if (dt.TableName == "Fields")
                    {
                        MandatoryFields = dt;
                        foreach (DataRow row in dt.Rows)
                        {
                            string key = (string)row["control"];
                            if (inputFields[key] != null) // 5.1 uat118 rdb 12/11/07
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
                                    case "CheckBox":
                                        ((Control)inputFields[key]).Enabled = (Convert.ToBoolean(row["enabled"]) && !ReadOnly);
                                        break;
                                    default:
                                        break;
                                }
                                ((Control)inputFields[key]).Visible = Convert.ToBoolean(row["visible"]);

                                if (Convert.ToBoolean(row["mandatory"]) &&
                                    Convert.ToBoolean(row["visible"]))			//store for validation
                                {
                                    mandatoryFields[key] = inputFields[key];	//purposes
                                    HighliteControl((Control)mandatoryFields[key]);
                                }
                                if (Convert.ToBoolean(row["visible"]))
                                    visibleFields[key] = inputFields[key];

                                //Set properties for the label stored in the control's Tag
                                if ((string)((Control)inputFields[key]).Tag != null)
                                {
                                    key = (string)((Control)inputFields[key]).Tag;
                                    ((Control)inputFields[key]).Enabled = (Convert.ToBoolean(row["enabled"]) && !ReadOnly);
                                    ((Control)inputFields[key]).Visible = Convert.ToBoolean(row["visible"]);
                                }
                            }
                        }
                    }
                }
            }
            //CR 866 Changed how residential status works
            string residentalStatus = "";
            if (drpCurrentResidentialStatus1.SelectedItem != null)
                residentalStatus = (string)((DataRowView)drpCurrentResidentialStatus1.SelectedItem)[CN.Code];

            if (residentalStatus == "P" || residentalStatus == "R")
            {
                //End CR 866 Changes 
                if (mandatoryFields["txtMortgage1"] == null)
                {
                    mandatoryFields["txtMortgage1"] = txtMortgage1;
                    this.HighliteControl(txtMortgage1);
                }
            }
            else
            {
                if (mandatoryFields["txtMortgage1"] == null)
                {
                    //mandatoryFields.Remove("txtMortgage1");
                    this.UnHighliteControl(txtMortgage1);
                }
            }

            SetChxSpouseWorkingVisibility();        
                       
            // Set up giro mandatory fields depending on whether Giro is 'YES'
            this.SetMandatoryGiro();

            ////IP - 14/12/10 - Store Card - Bank Details beneath the Financial tab must be mandatory when scoring a Store Card account.
            //SetMandatoryForStoreCard(); //IP - 23/12/10 - Bug #2674 - Commented out as feature not required...but possibly maybe in the future.

            SetIncome(true);

            #endregion
        }

        private void SetChxSpouseWorkingVisibility()
        {
            if (drpMaritalStat1.SelectedValue != null)
            {
                if (drpMaritalStat1.SelectedValue.Equals("M"))
                {
                    lblIsSpouseWorking.Visible = true;
                    chxSpouseWorking.Enabled = drpMaritalStat1.Enabled;
                    chxSpouseWorking.Visible = true;
                }
                else
                {
                    lblIsSpouseWorking.Visible = false;
                    chxSpouseWorking.Enabled = drpMaritalStat1.Enabled;
                    chxSpouseWorking.Visible = false;
                }
            }
            else
            {
                lblIsSpouseWorking.Visible = false;
                chxSpouseWorking.Enabled = drpMaritalStat1.Enabled;
                chxSpouseWorking.Visible = false;
            }
        }

        /// <summary>
        /// This will call the field validation event handlers for all the controls
        /// which have them.  Fields in error will be recorded and displayed after 
        /// the validation has completed.
        /// </summary>
        /// <returns></returns>
        private bool ValidateFields()
        {
            bool valid = true;

            //validate dropdowns
            drpIDSelection1_Validating(drpIDSelection1, new CancelEventArgs());
            drpSex1_Validating(drpSex1, new CancelEventArgs());
            drpEthnicGroup1_Validating(drpEthnicGroup1, new CancelEventArgs());
            drpNationality1_Validating(drpNationality1, new CancelEventArgs());
            drpMaritalStat1_Validating(drpMaritalStat1, new CancelEventArgs());
            drpCurrentResidentialStatus1_Validating(drpCurrentResidentialStatus1, new CancelEventArgs());
            drpPrevResidentialStatus1_Validating(drpPrevResidentialStatus1, new CancelEventArgs());
            drpPropertyType1_Validating(drpPropertyType1, new CancelEventArgs());
            drpEmploymentStat1_Validating(drpEmploymentStat1, new CancelEventArgs());
            drpOccupation1_Validating(drpOccupation1, new CancelEventArgs());
            drpPayFrequency1_Validating(drpPayFrequency1, new CancelEventArgs());
            drpBank1_Validating(drpBank1, new CancelEventArgs());
            drpBankAcctType1_Validating(drpBankAcctType1, new CancelEventArgs());
            this.drpGiroDueDate1_Validating(drpGiroDueDate1, new CancelEventArgs());
            this.drpPayByGiro1_Validating(drpPayByGiro1, new CancelEventArgs());

            //CR 866 Additional combo box fields
            this.ValidateComboBox(drpEductation1, new CancelEventArgs());
            this.ValidateComboBox(drpTransportType1, new CancelEventArgs());
            //End CR 866

            //validate money fields
            txtAdditionalExpenditure1_Validating(txtAdditionalExpenditure1, new CancelEventArgs());
            txtAdditionalExpenditure2_Validating(txtAdditionalExpenditure2, new CancelEventArgs());
            txtNetIncome1_Validating(txtNetIncome1, new CancelEventArgs());
            txtAddIncome1_Validating(txtAddIncome1, new CancelEventArgs());
            txtMortgage1_Validating(txtMortgage1, new CancelEventArgs());
            txtMisc1_Validating(txtMisc1, new CancelEventArgs());
            txtUtilities1_Validating(txtUtilities1, new CancelEventArgs());
            txtLoans1_Validating(txtLoans1, new CancelEventArgs());
            txtOther1_Validating(txtOther1, new CancelEventArgs());
            txtTotal1_Validating(txtTotal1, new CancelEventArgs());
            txtMoreRewards1_Validating(txtMoreRewards1, new CancelEventArgs());
            this.txtBankAccountName1_Validating(txtBankAccountName1, new CancelEventArgs());
            this.txtBankAcctNumber1_Validating(txtBankAcctNumber1, new CancelEventArgs());
            this.txtEmpTelCode1_Validating(txtEmpTelCode1, new CancelEventArgs());
            this.txtEmpTelNum1_Validating(txtEmpTelNum1, new CancelEventArgs());

            //CR 866b Combo boxes  
            this.ValidateComboBox(txtIndustry1, new CancelEventArgs());
            this.ValidateComboBox(txtJobTitle1, new CancelEventArgs());
            this.ValidateComboBox(txtOrganisation1, new CancelEventArgs());
            //End CR 866

            if (this.AccountType == AT.ReadyFinance || (this.AccountType == AT.HP && ReturnScoreHPResult()))
                this.txtRFCategory_Validating(txtRFCategory, new CancelEventArgs());

            //Validate app2 fields
            if (prop2 != null)
            {
                drpIDSelection2_Validating(drpIDSelection2, new CancelEventArgs());
                drpTitle2_Validating(drpTitle2, new CancelEventArgs());
                drpSex2_Validating(drpSex2, new CancelEventArgs());
                drpOccupation2_Validating(drpOccupation2, new CancelEventArgs());
                drpEmploymentStat2_Validating(drpEmploymentStat2, new CancelEventArgs());
                txtNetIncome2_Validating(txtNetIncome2, new CancelEventArgs());
                txtAddIncome2_Validating(txtAddIncome2, new CancelEventArgs());
                txtAlias2_Validating(txtAlias2, new CancelEventArgs());
            }
            else
            {
                errorProvider1.SetError(drpIDSelection2, String.Empty);
                errorProvider1.SetError(drpTitle2, String.Empty);
                errorProvider1.SetError(drpSex2, String.Empty);
                errorProvider1.SetError(drpOccupation2, String.Empty);
                errorProvider1.SetError(drpEmploymentStat2, String.Empty);
                errorProvider1.SetError(txtNetIncome2, String.Empty);
                errorProvider1.SetError(txtAddIncome2, String.Empty);
                errorProvider1.SetError(txtAlias2, String.Empty);
                inError.Remove(drpIDSelection2.Name);
                inError.Remove(drpTitle2.Name);
                inError.Remove(drpSex2.Name);
                inError.Remove(drpOccupation2.Name);
                inError.Remove(drpEmploymentStat2.Name);
                inError.Remove(txtNetIncome2.Name);
                inError.Remove(txtAddIncome2.Name);
                inError.Remove(txtAlias2.Name);
            }

            /* TO DO can we use the list of fields in Error to make the 
             * validation more efficient i.e. only validate the control that
             * has been changed and any field in the inError list */
            if (inError.Keys.Count > 0)
            {
                valid = false;

                if (_display)
                {
                    string errors = "The following fields are in error: \n\n";
                    foreach (object o in inError.Keys)
                    {
                        Control c = (Control)inputFields[o.ToString()];
                        Control l = (Control)inputFields[c.Tag.ToString()];

                        errors += l.Text + "\n";
                    }
                    errors += "\nPlease correct and try again.";
                    MessageBox.Show(errors, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            return valid;
        }

        /// <summary>
        /// Generic validation function to ensure that all date values are prior to the 
        /// date of the proposal (or Today, whichever is appropriate)
        /// </summary>
        /// <param name="date"></param>
        /// <param name="e"></param>
        private bool ValidateDate(DateTimePicker date, System.ComponentModel.CancelEventArgs e)
        {
            bool status = false;
            try
            {
                Function = "ValidateDate()";

                if (visibleFields[date.Name] != null && date.Enabled)
                //if(date.Visible && date.Enabled)
                {
                    DateTimePicker c = (DateTimePicker)mandatoryFields[date.Name];
                    if (c != null)
                    {
                        if (date.Value > dtDateProp.Value)
                        {
                            errorProvider1.SetError(date, GetResource("M_INVALIDDATE"));
                            //e.Cancel = true;
                            inError.Remove(date.Name);		//in case it's already there
                            inError.Add(date.Name, true);
                        }
                        else
                        {
                            errorProvider1.SetError(date, String.Empty);
                            status = true;
                            inError.Remove(date.Name);
                        }
                    }
                }
                else
                {
                    errorProvider1.SetError(date, String.Empty);
                    status = true;
                    inError.Remove(date.Name);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            return status;
        }

        /// <summary>
        /// If it's a mandatory field make sure that a selection has been made
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="e"></param>
        private void ValidateComboBox(ComboBox cb, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Function = "ValidateComboBox()";

                if (visibleFields[cb.Name] != null && cb.Enabled)
                {
                    ComboBox c = (ComboBox)mandatoryFields[cb.Name];
                    if (c != null && _complete == true)						//if it's mandatory
                    {
                        if (c.Text.Length == 0)
                        {
                            if (errorProvider1.GetError(cb) != ErrorMessages.SanctionDropErrorText)
                            {
                                errorProvider1.SetError(c, GetResource("M_ENTERMANDATORY"));
                                //e.Cancel = true;
                                inError.Remove(cb.Name);		//in case it's already there
                                inError.Add(cb.Name, true);
                            }
                        }
                        else
                        {
                            if (errorProvider1.GetError(cb) != ErrorMessages.SanctionDropErrorText)
                            {
                                errorProvider1.SetError(cb, string.Empty);
                                inError.Remove(cb.Name);
                            }
                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    errorProvider1.SetError(cb, String.Empty);
                    inError.Remove(cb.Name);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private bool ValidateDatesAgainstDOB()
        {
            //--Clear the Error provider---------------------------------------------------
            if (dtDateInCurrentAddress1.Enabled)
                errorProvider1.SetError(dtDateInCurrentAddress1, "");
            if (dtCurrEmpStart1.Enabled)
                errorProvider1.SetError(dtCurrEmpStart1, "");
            if (dtBankOpened1.Enabled)
                errorProvider1.SetError(dtBankOpened1, "");
            if (dtEmploymentStart2.Enabled)
                errorProvider1.SetError(dtEmploymentStart2, "");
            if (dtBankOpened2.Enabled)
                errorProvider1.SetError(dtBankOpened2, "");
            //-----------------------------------------------------------------------------

            bool valid = true;
            try
            {
                Function = "ValidateDatesAgainstDOB()";

                //-- Applicant 1 -------- Adding one month to Date in current address if user enters same date as Date of Birth then obviously born there. Otherwise alert with error. UAT 134
                if (dtDateInCurrentAddress1.Enabled && dtDateInCurrentAddress1.Value.Date != Date.blankDate.Date && dtDateInCurrentAddress1.Value.AddMonths(1) < dtDOB1.Value)
                {
                    valid = false;
                    errorProvider1.SetError(dtDateInCurrentAddress1, "Date must be later than DOB");
                }
                if (dtCurrEmpStart1.Enabled && dtCurrEmpStart1.Value.Date != Date.blankDate.Date && dtCurrEmpStart1.Value < dtDOB1.Value)
                {

                    valid = false;
                    errorProvider1.SetError(dtCurrEmpStart1, "Date must be later than DOB");
                }
                if (dtBankOpened1.Enabled && dtBankOpened1.Value.Date != Date.blankDate.Date && dtBankOpened1.Value < dtDOB1.Value)
                {
                    valid = false;
                    errorProvider1.SetError(dtBankOpened1, "Date must be later than DOB");
                }
                //---------------------------------------------------------------------------

                //-- Applicant 2 ------------------------------------------------------------
                if (dtEmploymentStart2.Enabled && dtEmploymentStart2.Value.Date != Date.blankDate.Date && dtEmploymentStart2.Value < dtDOB2.Value)
                {
                    valid = false;
                    errorProvider1.SetError(dtEmploymentStart2, "Date must be later than DOB");
                }
                if (dtBankOpened2.Enabled && dtBankOpened2.Value.Date != Date.blankDate.Date && dtBankOpened2.Value < dtDOB2.Value)
                {
                    valid = false;
                    errorProvider1.SetError(dtBankOpened2, "Date must be later than DOB");
                }
                //---------------------------------------------------------------------------

            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }

            return valid;
        }

        /// <summary>
        /// If it's a mandatory field make sure that some text has been entered.
        /// The TextBox itself will limit the length
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="e"></param>
        private void ValidateText(TextBox tb, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Function = "ValidateText()";

                if (visibleFields[tb.Name] != null && tb.Enabled)
                {
                    TextBox t = (TextBox)mandatoryFields[tb.Name];
                    if (t != null && _complete == true)
                    {
                        if (t.Text.Length == 0)
                        {
                            errorProvider1.SetError(t, GetResource("M_ENTERMANDATORY"));
                            inError.Remove(tb.Name);		//in case it's alerady there
                            inError.Add(tb.Name, true);
                        }
                        else
                        {
                            errorProvider1.SetError(t, String.Empty);
                            inError.Remove(tb.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void ValidateNumber(TextBox tb, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Function = "ValidateNumber()";

                if (visibleFields[tb.Name] != null && tb.Enabled)
                {
                    // (M49,M104) DSR 10/4/03 - Need to check IsNumeric even when not mandatory
                    tb.Text = tb.Text.Trim();
                    if (tb.ReadOnly == false)
                    {
                        if (!IsNumeric(tb.Text))
                        {
                            errorProvider1.SetError(tb, GetResource("M_NONNUMERIC"));
                            inError.Remove(tb.Name);		//in case it's already there
                            inError.Add(tb.Name, true);
                        }
                        else if (_complete == true && mandatoryFields[tb.Name] != null && tb.Text.Length == 0)
                        {
                            errorProvider1.SetError(tb, GetResource("M_ENTERMANDATORY"));
                            inError.Remove(tb.Name);		//in case it's already there
                            inError.Add(tb.Name, true);
                        }
                        else
                        {
                            errorProvider1.SetError(tb, String.Empty);
                            inError.Remove(tb.Name);
                        }
                    }
                }
                else
                {
                    errorProvider1.SetError(tb, String.Empty);
                    inError.Remove(tb.Name);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }

        }

        /// <summary>
        /// Validate money fields. Strip the currency symbols, convert them to 
        /// decimal and then format them back into text.
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="e"></param>
        private bool ValidateMoney(TextBox tb, System.ComponentModel.CancelEventArgs e)
        {
            bool valid = true;
            try
            {
                Function = "ValidateMoney()";

                if (visibleFields[tb.Name] != null && tb.Enabled)
                {
                    // (M49,M104) DSR 10/4/03 - Need to check IsNumeric even when not mandatory
                    tb.Text = tb.Text.Trim();
                    tb.Text = StripCurrency(tb.Text);
                    if (!IsStrictNumeric(tb.Text))
                    {
                        valid = false;
                        errorProvider1.SetError(tb, GetResource("M_NONNUMERIC"));
                        inError.Remove(tb.Name);		//in case it's already there
                        inError.Add(tb.Name, true);
                    }
                    //else if (_complete == true && mandatoryFields[tb.Name] != null && tb.Text.Length == 0)
                    else if (_complete == true && mandatoryFields[tb.Name] != null && tb.Text.Trim() == "") //IP - 18/02/10 - CR1072 - LW 70310 - General Fixes from 4.3 - Merge
                    {
                        valid = false;
                        errorProvider1.SetError(tb, GetResource("M_ENTERMANDATORY"));
                        inError.Remove(tb.Name);		//in case it's already there
                        inError.Add(tb.Name, true);
                    }
                    // Check size of entry          //69571 jec 11/02/08
                    else if (tb.Text.Length > 7 && Convert.ToDecimal(tb.Text) > 999999999)
                    {
                        errorProvider1.SetError(tb, GetResource("M_VALUETOOLARGE"));
                        valid = false;
                        inError.Remove(tb.Name);		//in case it's already there
                        inError.Add(tb.Name, true);
                    }
                    else
                    {
                        valid = true;
                        errorProvider1.SetError(tb, String.Empty);
                        inError.Remove(tb.Name);
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

        private bool Save()
        {
            bool valid = true;
            Function = "Save()";
            try
            {
                bool sanction = false;
                Wait();
                //put the value of all the fields back same dataset that was originally returned
                //then send that dataset back to the server to save it.
                //if(ValidateFields())	/* needn't be valid to save it */
                //{
                #region applicant 1
                #region information from the customer tables
                DataRow r = prop.Tables[TN.Customer].Rows[0];

                r[CN.CustomerID] = this.CustomerID;
                r[CN.FirstName] = txtFirstName.Text;
                r[CN.LastName] = txtLastName.Text;
                switch (drpSex1.SelectedIndex)
                {
                    case 0:
                        break;
                    case 1: r[CN.Sex] = "M";
                        break;
                    case 2: r[CN.Sex] = "F";
                        break;
                    default:
                        break;
                }
                //this condittion will cause an primary key violation
                // #9740 Removed as this was causing custaddress.datechange to be updated even though address had not changed     jec 08/03/12
                //if (dtDateInCurrentAddress1.Value == dtDateInPrevAddress1.Value)
                //{
                //    //dtDateInPrevAddress1.Value = dtDateInPrevAddress1.Value.AddMinutes(-1); //Commented as this will cause primary key violation for subsequent saves - uat(4.3) - 153
                //    dtDateInCurrentAddress1.Value = dtDateInCurrentAddress1.Value.AddMinutes(+1); //CR1072 malaysia merge - uat(4.3) - 153
                //}
                if (drpEthnicGroup1.SelectedIndex != -1)
                    r[CN.Ethnicity] = (string)((DataRowView)drpEthnicGroup1.SelectedItem)[CN.Code];
                r[CN.MoreRewardsNo] = txtMoreRewards1.Text;
                r[CN.EffectiveDate] = dtMoreRewardsDate1.Value;
                if (drpIDSelection1.SelectedIndex != -1)
                    r[CN.IDType] = (string)((DataRowView)drpIDSelection1.SelectedItem)[CN.Code];
                r[CN.DOB] = dtDOB1.Value;

                r[CN.DateIn] = dtDateInCurrentAddress1.Value;
                /*AA- 22/10/02 using selectedindex to determine whether drop-down has items in it otherwise exception generated*/

                if (drpCurrentResidentialStatus1.SelectedIndex != -1)
                    r[CN.ResidentialStatus] = (string)((DataRowView)drpCurrentResidentialStatus1.SelectedItem)[CN.Code];
                if (drpPropertyType1.Visible == true && drpPropertyType1.SelectedIndex != -1)
                    r[CN.PropertyType] = (string)((DataRowView)drpPropertyType1.SelectedItem)[CN.Code];
                r[CN.PrevDateIn] = dtDateInPrevAddress1.Value;
                if (drpPrevResidentialStatus1.SelectedIndex != -1)
                    r[CN.PrevResidentialStatus] = (string)((DataRowView)drpPrevResidentialStatus1.SelectedItem)[CN.Code];
                if (txtMortgage1.Text == "")
                    r[CN.MonthlyRent] = DBNull.Value;
                else
                    r[CN.MonthlyRent] = Convert.ToDouble(StripCurrency(txtMortgage1.Text));
                #endregion

                #region information from the bank tables
                r = prop.Tables[TN.Bank].Rows[0];
                r[CN.CustomerID] = this.CustomerID;
                r[CN.BankAccountNo] = this.txtBankAcctNumber1.Text;
                r[CN.BankAccountOpened] = dtBankOpened1.Value;
                r[CN.Code] = (string)((DataRowView)drpBankAcctType1.SelectedItem)[CN.Code];
                if (drpBank1.SelectedIndex != -1)
                    r[CN.BankCode] = (string)((DataRowView)this.drpBank1.SelectedItem)[CN.BankCode];
                if (drpPayByGiro1.SelectedIndex == 2)	//then record extra info
                {
                    if (IsNumeric(((DataRowView)drpGiroDueDate1.SelectedItem)[CN.DueDayId].ToString()))
                        r[CN.DueDayId] = (int)((DataRowView)drpGiroDueDate1.SelectedItem)[CN.DueDayId];
                    else
                        r[CN.DueDayId] = 0;

                    r[CN.BankAccountName] = this.txtBankAccountName1.Text;
                    r[CN.IsMandate] = Boolean.TrueString;
                }

                #endregion

                #region information from the employment tables
                r = prop.Tables[TN.Employment].Rows[0];
                if (dtCurrEmpStart1.Value == dtPrevEmpStart1.Value)
                    dtPrevEmpStart1.Value = dtPrevEmpStart1.Value.AddMinutes(-1);
                r[CN.CustomerID] = this.CustomerID;
                r[CN.DateEmployed] = dtCurrEmpStart1.Value;
                //CR 866 Change CN.Occupation to CN.WorkType
                r[CN.WorkType] = (string)((DataRowView)drpOccupation1.SelectedItem)[CN.Code];
                if (drpEmploymentStat1.SelectedIndex != -1)
                    r[CN.EmploymentStatus] = (string)((DataRowView)drpEmploymentStat1.SelectedItem)[CN.Code];
                if (drpPayFrequency1.SelectedIndex != -1)
                    r[CN.PayFrequency] = (string)((DataRowView)drpPayFrequency1.SelectedItem)[CN.Code];

                if (txtNetIncome1.Text == "")
                    r[CN.AnnualGross] = DBNull.Value;
                else
                    r[CN.AnnualGross] = 12 * Convert.ToDouble((StripCurrency(txtNetIncome1.Text)));
                r[CN.PersDialCode] = txtEmpTelCode1.Text;
                r[CN.PersTel] = txtEmpTelNum1.Text;
                r[CN.PrevDateEmployed] = dtPrevEmpStart1.Value;

                //CR 866b - Convert to drop downs Thailand scoring [PC]
                r[CN.JobTitle] = txtJobTitle1.SelectedValue;
                r[CN.Organisation] = txtOrganisation1.SelectedValue;
                r[CN.Industry] = txtIndustry1.SelectedValue;
                r[CN.EducationLevel] = drpEductation1.SelectedValue;

                //End CR 866
                #endregion

                #region supplementary information which will go in the proposal table
                //there may or may not be a row in the proposal table
                if (prop.Tables[TN.Proposal].Rows.Count == 0)
                {
                    r = prop.Tables[TN.Proposal].NewRow();
                    prop.Tables[TN.Proposal].Rows.Add(r);
                }
                else
                    r = prop.Tables[TN.Proposal].Rows[0];

                r[CN.DateProp] = dtDateProp.Value;
                if (drpApplicationType.SelectedIndex != -1)
                    r[CN.ApplicationType] = (string)((DataRowView)drpApplicationType.SelectedItem)[CN.Code];
                r[CN.NewS1Comment] = txtNewS1Comment.Text;
                r[CN.MaritalStatus] = (string)((DataRowView)drpMaritalStat1.SelectedItem)[CN.Code];
				r[CN.IsSpouseWorking] = chxSpouseWorking.Checked;
                r[CN.Dependants] = noDependencies1.Value;
                if (drpNationality1.SelectedIndex != -1)
                    r[CN.Nationality] = (string)((DataRowView)drpNationality1.SelectedItem)[CN.Code];
                r[CN.PrevEmpYY] = dtPrevEmpStart1.Years;
                r[CN.PrevEmpMM] = dtPrevEmpStart1.Months;
                if (txtAddIncome1.Text == "")
                    r[CN.AdditionalIncome] = DBNull.Value;
                else
                    r[CN.AdditionalIncome] = MoneyStrToDecimal(txtAddIncome1.Text);
                if (txtOther1.Text == "")
                    r[CN.OtherPayments] = DBNull.Value;
                else
                    r[CN.OtherPayments] = Convert.ToDouble(StripCurrency(txtOther1.Text));
                r[CN.CCardNo1] = txtCreditCardNo1.One;
                r[CN.CCardNo2] = txtCreditCardNo1.Two;
                r[CN.CCardNo3] = txtCreditCardNo1.Three;
                r[CN.CCardNo4] = txtCreditCardNo1.Four;
                if (txtUtilities1.Text == "")
                    r[CN.Commitments1] = DBNull.Value;
                else
                    r[CN.Commitments1] = MoneyStrToDecimal(txtUtilities1.Text);
                if (txtLoans1.Text == "")
                    r[CN.Commitments2] = DBNull.Value;
                else
                    r[CN.Commitments2] = MoneyStrToDecimal(txtLoans1.Text);
                if (txtMisc1.Text == "")
                    r[CN.Commitments3] = DBNull.Value;
                else
                    r[CN.Commitments3] = MoneyStrToDecimal(txtMisc1.Text);

                if (txtAdditionalExpenditure1.Text == "")
                    r[CN.AdditionalExpenditure1] = DBNull.Value;
                else
                    r[CN.AdditionalExpenditure1] = MoneyStrToDecimal(txtAdditionalExpenditure1.Text);

                if (txtAdditionalExpenditure2.Text == "")
                    r[CN.AdditionalExpenditure2] = DBNull.Value;
                else
                    r[CN.AdditionalExpenditure2] = MoneyStrToDecimal(txtAdditionalExpenditure2.Text);

                if (txtRFCategory.Text == "")
                    r[CN.RFCategory] = -1;
                else
                    r[CN.RFCategory] = Convert.ToInt16(RFCategory);

                r["PurchaseCashLoan"] = cbPurchaseCashLoan.Checked;

                //CR 866 Added these fields
                r[CN.DistanceFromStore] = txtDistanceFromStore1.Value;
                r[CN.TransportType] = drpTransportType1.SelectedValue;

                //End CR 866 
                #endregion

                #region information from the agreement table
                r = prop.Tables[TN.Agreements].Rows[0];
                if (drpPaymentMethod.SelectedIndex != -1)
                    r[CN.PaymentMethod] = (string)((DataRowView)drpPaymentMethod.SelectedItem)[CN.Code];

                #endregion



                #endregion

                if (prop2 != null)		//this means we have a second applicant
                {
                    string app2ID = null;
                    #region applicant 2
                    #region information from the customer tables
                    r = prop2.Tables[TN.Customer].Rows[0];
                    app2ID = (string)r[CN.CustomerID];
                    r[CN.IDType] = (string)((DataRowView)drpIDSelection2.SelectedItem)[CN.Code];
                    r[CN.Title] = (string)((DataRowView)drpTitle2.SelectedItem)[CN.CodeDescription];
                    r[CN.FirstName] = txtFirstName2.Text;
                    r[CN.LastName] = txtLastName2.Text;
                    r[CN.Alias] = txtAlias2.Text;
                    r[CN.DOB] = dtDOB2.Value;
                    switch (drpSex2.SelectedIndex)
                    {
                        case 0:
                            break;
                        case 1: r[CN.Sex] = "M";
                            break;
                        case 2: r[CN.Sex] = "F";
                            break;
                        default:
                            break;
                    }
                    r[CN.MoreRewardsNo] = txtMoreRewards2.Text;
                    r[CN.EffectiveDate] = dtMoreRewardsDate2.Value;
                    #endregion

                    #region information from the bank tables
                    r = prop2.Tables[TN.Bank].Rows[0];
                    r[CN.CustomerID] = app2ID;
                    r[CN.BankAccountOpened] = dtBankOpened2.Value;
                    #endregion

                    #region information from the employment tables
                    r = prop2.Tables[TN.Employment].Rows[0];
                    r[CN.CustomerID] = app2ID;
                    r[CN.DateEmployed] = dtEmploymentStart2.Value;
                    //CR 866 Change CN.Occupation to CN.WorkType
                    r[CN.WorkType] = (string)((DataRowView)drpOccupation2.SelectedItem)[CN.Code];
                    r[CN.EmploymentStatus] = (string)((DataRowView)drpEmploymentStat2.SelectedItem)[CN.Code];
                    if (txtNetIncome2.Text == "")
                        r[CN.AnnualGross] = DBNull.Value;
                    else
                        r[CN.AnnualGross] = 12 * Convert.ToDecimal((StripCurrency(txtNetIncome2.Text)));
                    #endregion

                    #region supplimentary information which will go in the proposal table
                    if (prop2.Tables[TN.Proposal].Rows.Count == 0)
                    {
                        r = prop2.Tables[TN.Proposal].NewRow();
                        prop2.Tables[TN.Proposal].Rows.Add(r);
                    }
                    else
                        r = prop2.Tables[TN.Proposal].Rows[0];
                    r[CN.DateProp] = dtDateProp.Value;
                    if (txtAddIncome2.Text == "")
                        r[CN.AdditionalIncome2] = DBNull.Value;
                    else
                        r[CN.AdditionalIncome2] = MoneyStrToDecimal(txtAddIncome2.Text);
                    r[CN.A2Relation] = ((DataRowView)drpApplicationType.SelectedItem)[CN.Code].ToString();
                    #endregion
                    #endregion

                }

                CreditManager.SaveProposal(txtCustomerID.Text, this.AccountNo, prop, prop2, sanction, out Error);
                if (Error.Length > 0)
                {
                    ShowError(Error);
                    valid = false;
                }
                else
                {
                    // Disable bank details if a mandate has been created
                    this._mandateExists = (this.drpPayByGiro1.SelectedIndex == 2);
                    this.SetMandatoryFields();
                    // Clear Comment so not replicated if S1 re-opened before exiting 
                    this.txtNewS1Comment.Text = "";         // UAT 5.0 iss 225 jec 28/03/07 
                }

                //}
                //else
                //	valid = false;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
                valid = false;
            }
            finally
            {
                StopWait();
            }
            return valid;
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
                if (this._firstAccountNo != String.Empty)
                {
                    // Two accounts were locked to load the current RF proposal
                    AccountManager.UnlockAccount(this._firstAccountNo, Credential.UserId, out Error);
                    if (Error.Length > 0)
                    {
                        status = false;
                        ShowError(Error);
                    }
                }

                ((MainForm)this.FormRoot).tbSanction.Visible = false;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();

                Dispose(true);

                //If dispose is called already then say GC to skip finalize on this instance.

                GC.SuppressFinalize(this);
            }

            return status;
        }


        private void SanctionStage1_Load(object sender, System.EventArgs e)
        {
            if (this.DesignMode) return;
            //Convert this code to run in a seperate thread for performance reasons
            try
            {
                Wait();
                this.SuspendEvents = true;
                toolTip1.SetToolTip(this.btnComplete, GetResource("TT_COMPLETE"));
                cancellationCode = (string)Country[CountryParameterNames.CancellationRejectionCode];

                //call the load data method which will start a new thread
                LoadData();

                if (this.AccountLocked)
                {
                    // Set up the drop down fields
                    LoadStatic();

                    // Load existing data for this proposal
                    LoadStage1Details();

                    // Determine which fields are mandatory for this country
                    // Can be dependant upon the data loaded above
                    SetMandatoryFields();

                    this.SuspendEvents = false;
                    ValidateControl(null, null);
                }
                loadingscreen = false;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                this.SuspendEvents = false;
                StopWait();
            }
        }

        private void SanctionStage1_Leave(object sender, System.EventArgs e)
        {
            //if (this.DesignMode) return;
            if (!_preventHideSanctionStatus)
            {
                ((MainForm)this.FormRoot).tbSanction.Visible = false;
            }
            else
            {
                // make sure we dont prevent it hiding next time around
                _preventHideSanctionStatus = false;
            }
        }

        private void SanctionStage1_Enter(object sender, System.EventArgs e)
        {
            try
            {
                if (this.DesignMode) return;
                ((MainForm)this.FormRoot).tbSanction.CustomerScreen = CustomerScreen;
                ((MainForm)this.FormRoot).tbSanction.Settled = this.Settled;
                ((MainForm)this.FormRoot).tbSanction.Load(this.allowConversionToHP, this.CustomerID, this.dtDateProp.Value, this.AccountNo, this.AccountType, this.ScreenMode);
                CurrentStatus = ((MainForm)this.FormRoot).tbSanction.CurrentStatus;
                //((MainForm)this.FormRoot).tbSanction.SetCurrentStage(SS.S1);
                ((MainForm)this.FormRoot).tbSanction.Visible = true;
                Complete = ((MainForm)this.FormRoot).tbSanction.ReadOnly(SS.S1);
                // rdb uat236 HP payment made sanction 1 incomplete, Current Status = 1
                //ReadOnly = !(!Complete && CurrentStatus == "0" && ((MainForm)this.FormRoot).tbSanction.HoldProp);
                ReadOnly = !(!Complete && (CurrentStatus == "0" || CurrentStatus == "1") && ((MainForm)this.FormRoot).tbSanction.HoldProp);
                SetReadOnly();

            }
            catch (Exception ex)
            {
                Catch(ex, "SanctionStage1_Enter");
            }
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            _complete = false;
            Save();
        }

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }

        private void btnComplete_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                _complete = true;

                bool outstPayments = false;
                bool cancel = true;

                if (Save() && btnComplete.ImageIndex == 4)
                {
                    if (MoneyStrToDecimal(txtDisposable.Text) <= 0)
                    {
                        // Cancel the account

                        //NegativeIncome ni = new NegativeIncome();
                        //ni.ShowDialog();

                        STLMessageBox mb = new STLMessageBox("M_NEGATIVEINCOME",
                            MessageOption.Review | MessageOption.Cancel);
                        mb.ShowDialog();

                        if (mb.Clicked == MessageOption.Cancel)
                        {
                            decimal balance = 0;
                            AccountManager.CheckAccountToCancel(this.AccountNo, Config.CountryCode, ref balance, ref outstPayments, out Error);
                            if (Error.Length > 0)
                                ShowError(Error);
                            else
                            {
                                if (outstPayments)
                                {
                                    OutstandingPayment op = new OutstandingPayment(FormRoot);
                                    op.ShowDialog();
                                    cancel = op.rbCancel.Checked;
                                    op.Dispose();
                                }
                                if (cancel)
                                {
                                    Settled = AccountManager.CancelAccount(this.AccountNo, this.CustomerID, Convert.ToInt16(Config.BranchCode),
                                        cancellationCode, balance, Config.CountryCode, String.Empty, 0, out Error);
                                    if (Error.Length > 0)
                                        ShowError(Error);
                                }
                            }
                            this.SanctionStage1_Enter(null, null);
                        }
                        mb.Dispose();
                    }
                    else
                    {
                        if (Convert.ToDecimal(txtAge1.Text) < (decimal)Country[CountryParameterNames.MinHPage])
                        {
                            // Applicant too young
                            ShowInfo("M_APPLICANTTOOYOUNG");
                            // ... but still go on to score the account
                        }

                        this.SetScoreResult();
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

        //private void dtDateProp_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    ValidateDate((DateTimePicker)sender, e);
        //}

        //private void dtBankAccountOpened1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    ValidateDate((DateTimePicker)sender, e);
        //}

        //private void dtDOB1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    ValidateDate((DateTimePicker)sender, e);
        //}

        //private void dtCurrEmpStart1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    ValidateDate((DateTimePicker)sender, e);
        //}

        //private void dtPrevEmpStart1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    ValidateDate((DateTimePicker)sender, e);
        //}

        private void noDependencies1_ValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (Convert.ToBoolean(Country[CountryParameterNames.ApplyNewDIChanges]))
                {
                    this.dependentSpendFactor = this.GetDependentSpendFactor();
                    CalculateDisposableIncome();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "noDependencies1_dtDOB1_ValueChanged");
            }


        }
        private void dtDOB1_ValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                int y = dtDateProp.Value.Year - ((DateTimePicker)sender).Value.Year;
                int m = dtDateProp.Value.Month - ((DateTimePicker)sender).Value.Month;
                int d = dtDateProp.Value.Day - ((DateTimePicker)sender).Value.Day;

                if (d < 0)
                    m--;

                if (m < 0)
                {
                    y--;
                    m += 12;
                }
                txtAge1.Text = y.ToString();
            }
            catch (Exception ex)
            {
                Catch(ex, "dtDOB1_ValueChanged");
            }
        }

        //private void dtDOB2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    ValidateDate((DateTimePicker)sender, e);
        //}

        private void dtDOB2_ValueChanged(object sender, System.EventArgs e)
        {
            try
            {

                int y = dtDateProp.Value.Year - ((DateTimePicker)sender).Value.Year;
                int m = dtDateProp.Value.Month - ((DateTimePicker)sender).Value.Month;
                int d = dtDateProp.Value.Day - ((DateTimePicker)sender).Value.Day;

                if (d < 0)
                    m--;

                if (m < 0)
                {
                    y--;
                    m += 12;
                }
                txtAge2.Text = y.ToString();

            }
            catch (Exception ex)
            {
                Catch(ex, "dtDOB2_ValueChanged");
            }
        }

        private void txtCreditCardNo1_TextChanged(object sender, System.EventArgs e)
        {
            if (txtCreditCardNo1.Text.Length < 19)
                txtCreditCardNo1.Text = "0000-0000-0000-0000";
        }

        private void drpIDSelection1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e);
        }

        private void drpSex1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e); ;
        }

        private void drpMaritalStat1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e); ;
        }

        private void drpEthnicGroup1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e); ;
        }

        private void drpNationality1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e); ;
        }

        private void drpPropertyType1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e); ;
        }

        private void drpCurrentResidentialStatus1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e); ;
        }

        private void drpPrevResidentialStatus1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e); ;
        }

        private void drpEmploymentStat1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e); ;
        }

        private void drpOccupation1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e); ;
        }

        private void drpPayFrequency1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e); ;
        }

        private void drpBank1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e); ;
        }

        private void drpBankAcctType1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e); ;
        }

        private void drpIDSelection2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e); ;
        }

        private void drpTitle2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e); ;
        }

        private void drpSex2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e); ;
        }

        private void drpEmploymentStat2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e); ;
        }

        private void drpOccupation2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e); ;
        }

        private void txtNetIncome1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                ValidateMoney((TextBox)sender, e);
                if (Convert.ToBoolean(Country[CountryParameterNames.ApplyNewDIChanges]))
                {
                    this.applicantSpendFactor = this.GetApplicantSpendFactor();
                }
				CalculateDisposableIncome();
            }
            catch (Exception ex)
            {
                Catch(ex, "txtNetIncome1_Validating");
            }
        }

        private void txtAddIncome1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                ValidateMoney((TextBox)sender, e);
                if (Convert.ToBoolean(Country[CountryParameterNames.ApplyNewDIChanges]))
                {
                    this.applicantSpendFactor = this.GetApplicantSpendFactor();
				}
				CalculateDisposableIncome();
            }
            catch (Exception ex)
            {
                Catch(ex, "txtAddIncome1_Validating");
            }
        }

        private void txtAdditionalExpenditure1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ValidateMoney((TextBox)sender, e))
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
                catch
                {
                    //Nothing much to do if this happens, just don't
                    //want it to be unhandled
                }
                txtTotal1.Text = (m + u + l + o + p + a1 + a2).ToString(DecimalPlaces);
            }
        }

        private void txtAdditionalExpenditure2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ValidateMoney((TextBox)sender, e))
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
                catch
                {
                    //Nothing much to do if this happens, just don't
                    //want it to be unhandled
                }
                txtTotal1.Text = (m + u + l + o + p + a1 + a2).ToString(DecimalPlaces);
            }
        }

        private void txtMortgage1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateMoney((TextBox)sender, e);
        }

        private void txtMisc1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ValidateMoney((TextBox)sender, e))
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
                catch
                {
                    //Nothing much to do if this happens, just don't
                    //want it to be unhandled
                }
                txtTotal1.Text = (m + u + l + o + p + a1 + a2).ToString(DecimalPlaces);
            }
        }

        private void txtUtilities1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ValidateMoney((TextBox)sender, e))
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
                catch
                {
                    //Nothing much to do if this happens, just don't
                    //want it to be unhandled
                }
                txtTotal1.Text = (m + u + l + o + p + a1 + a2).ToString(DecimalPlaces);
            }
        }

        private void txtLoans1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ValidateMoney((TextBox)sender, e))
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
                catch
                {
                    //Nothing much to do if this happens, just don't
                    //want it to be unhandled
                }
                txtTotal1.Text = (m + u + l + o + p + a1 + a2).ToString(DecimalPlaces);
            }
        }

        private void txtOther1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ValidateMoney((TextBox)sender, e))
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
                catch
                {
                    //Nothing much to do if this happens, just don't
                    //want it to be unhandled
                }
                txtTotal1.Text = (m + u + l + o + p + a1 + a2).ToString(DecimalPlaces);
            }
        }

        private void txtTotal1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ValidateMoney((TextBox)sender, e))
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
                catch
                {
                    //Nothing much to do if this happens, just don't
                    //want it to be unhandled
                }
                txtTotal1.Text = (m + u + l + o + p + a1 + a2).ToString(DecimalPlaces);
            }
        }

        private void txtNetIncome2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateMoney((TextBox)sender, e);
        }

        private void txtAddIncome2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateMoney((TextBox)sender, e);
        }

        private void txtAge1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateText((TextBox)sender, e);
        }

        private void txtMoreRewards1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateText((TextBox)sender, e);
        }

        private void txtEmpTelCode1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateNumber((TextBox)sender, e);
        }

        private void txtEmpTelNum1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateNumber((TextBox)sender, e);
        }

        private void txtBankAcctNumber1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateText((TextBox)sender, e);
        }

        private void drpPayByGiro1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.ValidateComboBox((ComboBox)sender, e);
        }

        //private void txtFirstName2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    ValidateText((TextBox)sender, e);
        //}

        //private void txtLastName2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    ValidateText((TextBox)sender, e);
        //}

        private void txtAlias2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateText((TextBox)sender, e);
        }

        //private void txtAge2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    ValidateText((TextBox)sender, e);
        //}

        //private void txtMoreRewards2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    ValidateText((TextBox)sender, e);
        //}

        private void drpPayByGiro1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (this.SuspendEvents || drpPayByGiro1.SelectedIndex == this._lastDrpPayByGiro1)
                    return;

                this._lastDrpPayByGiro1 = drpPayByGiro1.SelectedIndex;

                // Modify whether Giro fields should be enabled
                SetMandatoryFields();
                ValidateControl(null, null);
            }
            catch (Exception ex)
            {
                Catch(ex, "drpPayByGiro1_SelectedIndexChanged");
            }
        }

        private void drpGiroDueDate1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.ValidateComboBox((ComboBox)sender, e);
        }

        private void txtBankAccountName1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateText((TextBox)sender, e);
        }

        private void drpApplicationType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //JJ - this event does not only fire when the SelectedIndex changes, which is 
            //extremely annoying and means we have to add extra code to make sure it really
            //has changed
            try
            {
                if (!SuspendEvents)
                {
                    if (ApplicationType != ((DataRowView)drpApplicationType.SelectedItem)[CN.Code] as string)
                    {
                        ApplicationType = ((DataRowView)drpApplicationType.SelectedItem)[CN.Code] as string;

                        switch (drpApplicationType.SelectedIndex)
                        {
                            case 0: if (tcApplicants.TabPages.Contains(tpApp2))
                                    tcApplicants.TabPages.Remove(tpApp2);
                                prop2 = null;
                                break;
                            default: LoadAppTwo(((DataRowView)drpApplicationType.SelectedItem)[CN.Code].ToString());
                                break;
                        }
                        ValidateControl(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "drpApplicationType_SelectedIndexChanged");
            }
        }

        private void tvRFCategory_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            try
            {
                TreeNode n = tvRFCategory.SelectedNode;
                if (n.Tag != null)
                {
                    DataRowView r = (DataRowView)n.Tag;
                    txtRFCategory.Text = (string)r[CN.CodeDescription];
                    RFCategory = (string)r[CN.Code];
                    this.ValidateControl(null, null);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "tvRFCategory_AfterSelect");
            }
        }

        private void txtRFCategory_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateText((TextBox)sender, e);
        }

        private void menuReopen_Click(object sender, System.EventArgs e)
        {
            _preventHideSanctionStatus = true;

            _Reopening = true;
            try
            {

                CreditManager.UnClearFlag(AccountNo, SS.S1, true, Credential.UserId, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    this.Settled = false;
                    ScreenMode = SM.Edit;

                    /* re-load so we get current data */
                    SanctionStage1_Load(null, null);

                    SanctionStage1_Enter(sender, e);
                    if (CustomerScreen != null)
                        CustomerScreen.RefreshData();
                    // 5.0.0 uat338 rdb firing of form leave event seems to be prevented because
                    // we call routines on another form
                    // try setting focus to a control on this form
                    this.txtCustomerID.Focus();

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

        private void menuPrintRFDetails_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "menuPrintRFDetails_Click";
                Wait();

                if (this.CustomerID.Length != 0)
                {
                    XmlNode lineItems = AccountManager.GetLineItems(this.AccountNo, 1, this.AccountType, Config.CountryCode, Convert.ToInt16(Config.BranchCode), out Error);
                    if (Error.Length > 0)
                        ShowError(Error);

                    PrintAgreementDocuments(this.AccountNo,
                        this.AccountType,
                        this.CustomerID,
                        false, false, 0, 0,
                        lineItems, 1, this, true, 0, 0);
                }
                else
                {
                    ShowInfo("M_NOCUSTOMERFORPRINT");
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

        private void menuManualRefer_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "menuPrintRFDetails_Click";
                Wait();

                //IP - 15/03/11 - #3314 - CR1245 - Need to display a new form to enable users to enter referral reasons
                ManualReferral manualRefer = new ManualReferral(this.CustomerID, this.AccountNo, this.dtDateProp.Value, CustomerScreen.RFLimit);
                manualRefer.ShowDialog();

                //IP - 23/03/11 - CR1245 - If true
                if (manualRefer.refer == true)
                {
                    CreditManager.ManualRefer(this.CustomerID, this.AccountNo, dtDateProp.Value, true, false, out Error);
                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                        return;
                    }
                    else
                    {
                        ((MainForm)this.FormRoot).tbSanction.Load(this.allowConversionToHP, this.CustomerID, this.dtDateProp.Value, this.AccountNo, this.AccountType, this.ScreenMode);
                        CurrentStatus = ((MainForm)this.FormRoot).tbSanction.CurrentStatus;
                        showcrediticons("R");

                        ShowInfo("M_REFER", new object[] { referralReasons }); //IP - 15/03/11 - #3314 - CR1245
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

        private void drpEmploymentStat1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (!this.SuspendEvents)
                {
                    string unemployed = "U";

                    if (((string)((DataRowView)drpEmploymentStat1.SelectedItem)[CN.Code]).Trim() == unemployed)
                    {
                        drpOccupation1.Visible = false;
                        txtEmpTelCode1.Visible = false;
                        txtEmpTelNum1.Visible = false;
                        dtCurrEmpStart1.Visible = false;
                        lOccupation1.Visible = false;
                        lEmpTelCode1.Visible = false;
                        lEmpTelNum1.Visible = false;

                        visibleFields.Remove(drpOccupation1.Name);
                        errorProvider1.SetError(drpOccupation1, String.Empty);

                        visibleFields.Remove(txtEmpTelCode1.Name);
                        errorProvider1.SetError(txtEmpTelCode1, String.Empty);

                        visibleFields.Remove(txtEmpTelNum1.Name);
                        errorProvider1.SetError(txtEmpTelNum1, String.Empty);

                        visibleFields.Remove(dtCurrEmpStart1.Name);
                        errorProvider1.SetError(dtCurrEmpStart1, String.Empty);



                        if (FormParent != null)
                        {
                            if (FormParent.GetType().Name == "BasicCustomerDetails")
                                ((BasicCustomerDetails)this.FormParent).isUnemployed = true;
                        }
                    }
                    else
                    {
                        drpOccupation1.Visible = true;
                        txtEmpTelCode1.Visible = true;
                        txtEmpTelNum1.Visible = true;
                        dtCurrEmpStart1.Visible = true;
                        lOccupation1.Visible = true;
                        lEmpTelCode1.Visible = true;
                        lEmpTelNum1.Visible = true;

                        visibleFields[drpOccupation1.Name] = drpOccupation1;

                        visibleFields[txtEmpTelCode1.Name] = txtEmpTelCode1;

                        visibleFields[txtEmpTelNum1.Name] = txtEmpTelNum1;

                        visibleFields[dtCurrEmpStart1.Name] = dtCurrEmpStart1;

                        if (FormParent != null)
                        {
                            if (FormParent.GetType().Name == "BasicCustomerDetails")
                                ((BasicCustomerDetails)this.FormParent).isUnemployed = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "drpEmploymentStat1_SelectedIndexChanged");
            }
        }

        private void drpPayFrequency1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                // Update the employment tab in line with this financial amount
                this.SetIncome(true);
            }
            catch (Exception ex)
            {
                Catch(ex, "drpPayFrequency1_SelectedIndexChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void txtIncome_Leave(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                // Update the financial tab in line with this employment amount
                this.SetIncome(false);
            }
            catch (Exception ex)
            {
                Catch(ex, "txtIncome_Leave");
            }
            finally
            {
                StopWait();
            }
        }

        private void txtNetIncome1_Leave(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                // Update the employment tab in line with this financial amount
                this.SetIncome(true);
            }
            catch (Exception ex)
            {
                Catch(ex, "txtNetIncome1_Leave");
            }
            finally
            {
                StopWait();
            }
        }

        private void drpCurrentResidentialStatus1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (!SuspendEvents &&
                    drpCurrentResidentialStatus1.SelectedIndex != OldResStatIndex)
                {
                    OldResStatIndex = drpCurrentResidentialStatus1.SelectedIndex;
                    SetMandatoryFields();
                    ValidateControl(null, null);
                }
            }
            catch (Exception)
            {
                //Catch(ex, "drpCurrentResidentialStatus1_SelectedIndexChanged");
            }
            RemoveError((Control)sender);
        }

        private void txtTotal1_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                CalculateDisposableIncome();
            }
            catch (Exception ex)
            {
                Catch(ex, "txtTotal1_TextChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void ValidateControl(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Wait();

                _complete = true;

                if (ValidateFields() && ValidateDatesAgainstDOB())
                    btnComplete.ImageIndex = 4;
                else
                    btnComplete.ImageIndex = 3;

                _complete = false;
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

        /// <summary>
        /// CR903 - returns whether or not ScoreHPbefore has been enabled for the current branch
        /// </summary>
        /// <returns></returns>
        protected bool ReturnScoreHPResult()
        {
            string branchNo = AccountNo.Substring(0, 3);
            bool result = false;
            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
            {
                if (row[CN.BranchNo].ToString() == branchNo)
                {
                    result = (bool)row[CN.ScoreHPbefore];
                    break;
                }
            }
            return result;
        }

        private void tpResidential_Enter(object sender, EventArgs e)
        {
            try
            {
                //The residential tab was showing the previous address when it shouldn't
                dtDateInCurrentAddress1.CheckLinkedBias();
            }
            catch (Exception ex)
            {
                Catch(ex, "tpResidential_Enter");
            }
        }

        private void tcApplicants_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void txtAge1_Leave(object sender, EventArgs e)
        {
            //SC 23/10/08 - UPDATE dob when age is changed.



        }

        private void txtAge1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!loadingscreen) //dont set this if loading screen as would be wrong.
                {
                    //RM - CR1016 Only update year of birth when updating age
                    int y = Convert.ToInt32(CalculateAge(((MainForm)this.FormRoot).tbSanction.StageCleared(SS.S1), dtDOB1.Value)); //uat(4.3) - 155 // IP - 22/02/10 - CR1072 - UAT(155) - Sanction Fixes from 4.3 - Merge

                    if (Convert.ToInt16(txtAge1.Text) != y)
                        dtDOB1.Value = dtDOB1.Value.AddYears(-1 * (Convert.ToInt16(txtAge1.Text) - y));
                }
            }
            catch
            {
            }
        }

        //IP - 14/12/10 - Store Card. Bank details beneath Financial tab must be mandatory when scoring Store Card accounts.
        //IP - 23/12/10 - Bug #2674 - Commented out as feature not required...but possibly maybe in the future.
        //private void SetMandatoryForStoreCard()
        //{

        //    if (AccountType == AT.StoreCard)
        //    {
        //        drpBank1.Enabled = !ReadOnly;
        //        drpBank1.Visible = true;

        //        txtBankAcctNumber1.Enabled = !ReadOnly;
        //        txtBankAcctNumber1.Visible = true;

        //        //Set the Bank Detail fields to mandatory.
        //        mandatoryFields["drpBank1"] = drpBank1;
        //        mandatoryFields["txtBankAcctNumber1"] = txtBankAcctNumber1;
        //        HighliteControl(drpBank1);
        //        HighliteControl(txtBankAcctNumber1);
        //    }
        //}

        private void SetError(bool set, Control control)
        {
            if (!set)
            {
                errorProvider1.SetError(control, ErrorMessages.SanctionDropErrorText);
            }
            else
            {
                RemoveError(control);
            }
        }

        private void drpPropertyType1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemoveError((Control)sender);
        }

        private void drpEductation1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemoveError((Control)sender);
        }

        private void drpPrevResidentialStatus1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemoveError((Control)sender);
        }

        private void drpMaritalStat1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemoveError((Control)sender);

            SetChxSpouseWorkingVisibility();

            if (drpMaritalStat1.SelectedValue != null)
            { 
                if (!drpMaritalStat1.SelectedValue.Equals("M"))
                {
                    chxSpouseWorking.Checked = false;
                }
            }

            CalculateDisposableIncome();
        }

        private void drpNationality1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemoveError((Control)sender);
        }

        private void RemoveError(Control control)
        {
            if (errorProvider1.GetError(control) != GetResource("M_ENTERMANDATORY"))
            {
                errorProvider1.SetError(control, "");
            }
        }

        public bool SetSelectedValue(ComboBox cmb, string value)
        {
            cmb.SelectedValue = value;
            if (cmb.SelectedValue == null || cmb.SelectedValue.ToString() != value)
            {
                cmb.SelectedValue = string.Empty;
                if (cmb.SelectedValue == null)
                {
                    cmb.Items.Add(string.Empty);
                    cmb.SelectedValue = string.Empty;
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        private void CashLoanNewCustomerManualRefer(string custId, Action<CashLoanQualificationResponse> guiAction)
        {
            Client.Call(new CashLoanQualificationRequest
            {
                CustId = custId,
                Branch = Convert.ToInt32(Config.BranchCode)
            },
                response =>
                {
                    guiAction(response);
                },
                this);
        }

        private void chxSpouseWorking_CheckedChanged(object sender, EventArgs e)
        {
            CalculateDisposableIncome();
        }
    }
}
