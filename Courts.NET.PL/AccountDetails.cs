using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Collections;
using Blue.Cosacs.Shared.Extensions;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.StoreCard;
using Crownwood.Magic.Menus;
using STL.Common;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Elements;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.TabPageNames;
using STL.Common.Constants.Tags;
using STL.Common.Static;
using STL.PL.Collections;
//using STL.PL.SERVICE;
using STL.PL.Utils;
using STL.PL.WS1;

namespace STL.PL
{
    /// <summary>
    /// Detailed information for a customer account with the name and address details
    /// of the main holder of the account. The screen is split into tabs to show
    /// details that include the agreement, line items, transactions, application status,
    /// letters and allocations etc.
    /// </summary>
    public class AccountDetails : CommonForm
    {
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private string accountNo = String.Empty;
        //private DataSet accountDetails = null;
        private string tmpName;
        private System.Windows.Forms.GroupBox gbOtherAccounts;
        private DataView otherAcctsView;
        private DataSet agreement;
        private DataSet accounts;
        private DataSet deliveries;
        private DataSet custAddresses;
        private DataSet RFCombined;
        decimal addTo = 0;
        private DataSet payments;
        private DataSet service;
        private DateTime _dateAcctOpen = Date.blankDate;
        private bool _ShowAccountTransactions = true;  //CR 822 For the transactions tab to either display transactions or warranty returns [PC] 03-Sep-2006
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private XmlDocument itemDoc = null;
        private DataTable itemsTable = null;
        private DataView itemsView = null;
        private DataSet transactions = null;
        private DataView transView = null;
        private DataView ApplicationStatusView = null;
        private DataView LetterView = null;
        private DataView StatusView = null;
        private DataView StrategiesView = null;
        private DataView WorklistView = null;
        private System.ComponentModel.IContainer components;
        private bool readOnly = true;
        //private Crownwood.Magic.Menus.MenuControl menuMain;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.Button btnToggle;
        private System.Windows.Forms.Button btnCustCodes;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.TextBox txtAccountNo;
        private System.Windows.Forms.TextBox txtCustID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGrid dgAccounts;
        private System.Windows.Forms.GroupBox gbCustomer;
        private Crownwood.Magic.Controls.TabPage tpAgreement;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtAmountPaid;
        private System.Windows.Forms.TextBox txtToFollowAmount;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtTotalFees;
        private System.Windows.Forms.TextBox txtDeposit;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtDateDue;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.TextBox txtDeliveryTotal;
        private System.Windows.Forms.TextBox txtCODFlag;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtAddToPotential;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtDateDelivered;
        private System.Windows.Forms.TextBox txtSChargeAmount;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox txtInstalments;
        private System.Windows.Forms.TextBox txtInstalmentTotal;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.TextBox txtMthsInterestFree;
        private System.Windows.Forms.TextBox txtEarlySettlementFigure;
        private System.Windows.Forms.TextBox txtLMovement;
        private System.Windows.Forms.TextBox txtLastInstalmentAmount;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TextBox txtDateLInstalment;
        private System.Windows.Forms.TextBox txtIAmount;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TextBox txtDateFInstalment;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox txtDateOpened;
        private System.Windows.Forms.TextBox txtAccountType;
        private System.Windows.Forms.TextBox txtArrears;
        private System.Windows.Forms.TextBox txtOutstandingBalance;
        private System.Windows.Forms.TextBox txtDateLastPaid;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.TextBox txtAgreementTotal;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox txtTTDescription;
        private System.Windows.Forms.TextBox txtCurrentStatus;
        private System.Windows.Forms.TextBox txtPercentagePaid;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.TextBox txtTTCode;
        private Crownwood.Magic.Controls.TabPage tpLineItems;
        private System.Windows.Forms.GroupBox gbLineItems;
        public System.Windows.Forms.DataGrid dgLineItems;
        private System.Windows.Forms.Splitter splitter1;
        public System.Windows.Forms.TreeView tvItems;
        private Crownwood.Magic.Controls.TabPage tpTransactions;
        private Crownwood.Magic.Controls.TabPage tpRFCombined;
        private System.Windows.Forms.TextBox txtCardPrinted;
        private System.Windows.Forms.TextBox txtAvailableCredit;
        private System.Windows.Forms.TextBox txtTotalAgreements;
        private System.Windows.Forms.TextBox txtTotalArrears;
        private System.Windows.Forms.TextBox txtTotalBalances;
        private System.Windows.Forms.TextBox txtTotalCredit;
        private System.Windows.Forms.TextBox txtTotalInstalments;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.Label label56;
        private System.Windows.Forms.Label label57;
        private Crownwood.Magic.Controls.TabControl tcAddress;
        private Crownwood.Magic.Controls.TabPage currentTab;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ImageList imageList1;
        private Crownwood.Magic.Controls.TabPage tpEmployee;
        private Crownwood.Magic.Menus.MenuCommand menuRecent;
        private Crownwood.Magic.Controls.TabPage tpApplicationStatus;
        private Crownwood.Magic.Controls.TabPage tpLetters;
        public System.Windows.Forms.DataGrid dgLetters;
        private System.Windows.Forms.Button btnRefresh;
        //Tab to display amortized cash loan schedule
        private Crownwood.Magic.Controls.TabPage tpAmortizationSchedule;
        public System.Windows.Forms.DataGrid dgAmortizationSchedule;
        //private string Empty = "%";
        private string strategy = "";
        private string _showTabName = string.Empty;
        ArrayList customer = new ArrayList();

        private bool _paidAndTaken = false;
        private bool strategyHasWorklists = false; //IP - 26/08/09 - UAT(819)
        private int _precision = 2;

        public bool PaidAndTaken
        {
            get { return _paidAndTaken; }
            set { _paidAndTaken = value; }
        }

        public string CurrentActionCode
        {
            get
            {
                return drpActionCode.SelectedItem != null ? drpActionCode.SelectedValue.ToString() : string.Empty;
            }
        }

        public bool SPASelected
        {
            get
            {
                return (CurrentActionCode == "SPA");
            }
        }

        private bool ShowAccountTransactions
        {
            get { return this._ShowAccountTransactions; }
            set
            {
                //this._ShowAccountTransactions = value;
                //if (value)
                //    btnWarrantyCollections.Text = "Show Warranty Collections"; //TODO move to constant
                //else
                //    btnWarrantyCollections.Text = "Show Transactions";
            }
        }

        //jec - 05/03/10 - CR1072 - Malaysia 3PL for Version 5.2
        private bool thirdPartyDeliveriesEnabled
        {
            get
            {
                return Convert.ToBoolean(Country[CountryParameterNames.ThirdPartyDeliveriesWarehouse]);
            }
        }

        private bool _cancel = false;
        private Crownwood.Magic.Controls.TabPage tpActions;
        private Crownwood.Magic.Controls.TabPage tpAllocation;
        private System.Windows.Forms.Label lEmpType;
        private System.Windows.Forms.Label lEmpName;
        private System.Windows.Forms.TextBox txtEmployeeName;
        private System.Windows.Forms.TextBox txtEmployeeType;
        private System.Windows.Forms.TextBox txtEmployeeNo;
        private System.Windows.Forms.Label lEmployeeNo;
        private System.Windows.Forms.DataGrid dgActions;
        private System.Windows.Forms.TextBox textNotes;
        private System.Windows.Forms.Label label64;

        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
        }

        private int _agreementNo = 1;
        private System.Windows.Forms.ComboBox drpActionCode;
        private System.Windows.Forms.Label label65;
        private System.Windows.Forms.DateTimePicker dtDueDate;
        private System.Windows.Forms.Label label67;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.TextBox txtActionValue;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel pAllocation;
        public System.Windows.Forms.DataGrid dgStatus;
        private System.Windows.Forms.CheckBox chxPrivilegeClub;
        private System.Windows.Forms.TextBox txtRFIssueNo;
        private System.Windows.Forms.Label lRFIssueNo;
        private Crownwood.Magic.Controls.TabPage tpAgreementChanges;
        private Control showAgreementChanges;
        private Crownwood.Magic.Controls.TabControl tcMain;
        private DataSet Audit = null;
        private Crownwood.Magic.Controls.TabPage tpActivitySegments;
        private Crownwood.Magic.Controls.TabPage tpPendingCharges;
        private System.Windows.Forms.DataGrid dgSegment;
        private System.Windows.Forms.DataGrid dgActivities;
        private System.Windows.Forms.DataGrid dgArrangements;
        private System.Windows.Forms.TextBox txtTotalSpend;
        private System.Windows.Forms.Label labelCredit;
        private bool isAccountCancelled = false;
        private System.Windows.Forms.Button btnSPADetails;
        private System.Windows.Forms.GroupBox gbAllocationHist;
        private System.Windows.Forms.DataGrid dgAllocationHist;
        private System.Windows.Forms.GroupBox gbAllocatedTo;
        private System.Windows.Forms.ComboBox drpReason;
        private System.Windows.Forms.Label lbReason;
        private System.Windows.Forms.Label lbActionDate;
        private System.Windows.Forms.Label lbActionValue;
        private System.Windows.Forms.DataGrid dgPendingCharges;
        private System.Windows.Forms.DataGrid dgArrearsCharges;
        private Crownwood.Magic.Menus.MenuCommand menuStatement;
        private System.Windows.Forms.Label lArchive;
        private System.Windows.Forms.Label lPrivilegeClubDesc;
        private TextBox txtRebate;
        private Label label19;
        private Label lRate;
        private Label lBand;
        private TextBox txtBand;
        private TextBox txtRate;
        private Crownwood.Magic.Controls.TabPage tpServiceRequest;
        public DataGrid dgServiceRequestSummary;
        private Crownwood.Magic.Controls.TabPage tpStrategies;
        private DataGrid dgStrategies;
        private DataGrid dgWorklists;
        private Button button1;

        public int AgreementNo
        {
            get { return _agreementNo; }
            set { _agreementNo = value; }
        }

        private bool _addCodes = false;
        private TextBox txtSegmentName;
        private Label lSegmentId;
        private PictureBox Loyalty_HClogo_pb;

        public bool AddCodes
        {
            get { return _addCodes; }
            set { _addCodes = value; }
        }

        private string m_fileName = String.Empty;

        //CR855
        public string fileName
        {
            get { return m_fileName; }
            set { m_fileName = value; }
        }

        //IP - 20/10/11 - CR1232 - Cash Loans
        private bool _isLoan;
        public bool IsLoan
        {
            get { return _isLoan; }
            set { _isLoan = value; }
        }

        //IP - 20/10/11 - CR1232 - Cash Loans
        private bool _fullyDelivered;
        public bool FullyDelivered
        {
            get { return _fullyDelivered; }
            set { _fullyDelivered = value; }
        }

        private decimal Vouchervalue;
        private bool creditBlocked = false;
        private Label label4;
        private TextBox txt_vouchervalue;
        private DataGrid dgInstalPlan;
        private DataGrid dgLineItem;
        private DataGrid dgAgreementAudit;
        private Crownwood.Magic.Controls.TabPage tpInstallation;
        private DataGridView dgvInstallation;
        private DataGridView dgApplicationStatus;
        private MenuCommand menuAgreementDocs;
        private Crownwood.Magic.Controls.TabPage tpDeliveries;
        private GroupBox gbDelivery;
        public DataGrid dgDelivery;     //IP - 01/06/10 - UAT(1006) UAT5.2 Log
        private DataTable dtSPADetails = new DataTable();   //IP - 01/06/10 - UAT(1006) UAT5.2 Log
        private bool isAmortized;

        public AccountDetails(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new[] { menuFile });
        }

        public AccountDetails(string inAccountNo, Form root, Form parent)
        {
            SetupForm(inAccountNo, root, parent);
        }

        public AccountDetails(string inAccountNo, Form root, Form parent, string showTabName)
        {
            _showTabName = showTabName;

            SetupForm(inAccountNo, root, parent);

            foreach (Crownwood.Magic.Controls.TabPage tab in tcMain.TabPages)
            {
                if (tab.Name == _showTabName)
                {
                    tcMain.SelectedTab = (Crownwood.Magic.Controls.TabPage)tab;
                    break;
                }
            }
        }

        private void SetupForm(string inAccountNo, Form root, Form parent)
        {
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new[] { menuFile });

            try
            {
                Recent.AddAccount(inAccountNo);
                accountNo = inAccountNo;
                FormatTextBoxesForAgreement();
                toolTip1.SetToolTip(this.txtTotalInstalments, GetResource("TT_TOTALINSTALMENTS"));
                toolTip1.SetToolTip(this.btnRefresh, GetResource("TT_REFRESH"));

                dynamicMenus = new Hashtable();
                HashMenus();
                ApplyRoleRestrictions();

                //if (!btnCustCodes.Enabled)
                //{
                //    btnCustCodes.Enabled = true;
                //    //IP -16/10/2007 - If the 'Add Customer/Account Codes
                //    //user right is un-ticked, then set the text of the button to the below. (UAT329)
                //    btnCustCodes.Text = "Account/Customer Code";
                //    //IP -16/10/2007 - Make the button visible. (UAT329)
                //    btnCustCodes.Visible = true;
                //    AddCodes = true;
                //}
                //else
                //    AddCodes = false;

                if (!showAgreementChanges.Enabled)
                    tcMain.TabPages.Remove(tpAgreementChanges);

                if (!(bool)Country[CountryParameterNames.LinkToTallyman])
                    tcMain.TabPages.Remove(tpActivitySegments);

                // removed jec - Malaysia 4.3 merge Note: Giro not used need location for Segment
                //txtGiroPending.Visible = lGiro.Visible = ((bool)Country[CountryParameterNames.DDEnabled]);
                // Malaysia 4.3 merge - jec  
                bool linkToTallyman = Convert.ToBoolean(Country[CountryParameterNames.LinkToTallyman]);
                txtSegmentName.Visible = linkToTallyman;
                lSegmentId.Visible = linkToTallyman;

                // Hide collections strategies until version 5.3
                //IP - 22/09/08 - UAT5.2 - UAT(521) - I have commented out the below code as
                //the 'Strategies' tab was not being displayed.
                //tcMain.TabPages.Remove(tpStrategies);

                //IP - 04/02/10 - CR1072 - 3.1.9 - Display Delivery Authorisation History.
                if (tpEmployee.Control == null)
                {
                    tpEmployee.Control = new EmployeeInformation(inAccountNo);
                }

                //Prasad
                tpTransactions.Control = new Transactions();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void HashMenus()
        {
            dynamicMenus[this.Name + ":btnCustCodes"] = this.btnCustCodes;
            dynamicMenus[this.Name + ":pAllocation"] = this.pAllocation;
            dynamicMenus[this.Name + ":showAgreementChanges"] = this.showAgreementChanges;
            dynamicMenus[this.Name + ":menuAddAcctCodes"] = this.btnCustCodes;
        }

        private void BuildRecentListMenus()
        {
            menuRecent.MenuCommands.Clear();
            foreach (string s in Recent.AccountList)
            {
                Crownwood.Magic.Menus.MenuCommand m = new Crownwood.Magic.Menus.MenuCommand(s);
                menuRecent.MenuCommands.Add(m);
                m.Click += new System.EventHandler(OnRecentClick);
            }
        }

        private void OnRecentClick(object sender, System.EventArgs e)
        {
            Crownwood.Magic.Menus.MenuCommand m = (Crownwood.Magic.Menus.MenuCommand)sender;
            string account = m.Text;
            AccountDetails details = new AccountDetails(account, FormRoot, this);
            ((MainForm)this.FormRoot).AddTabPage(details, 7);
            CloseTab(this);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountDetails));
            this.showAgreementChanges = new System.Windows.Forms.Control();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.gbOtherAccounts = new System.Windows.Forms.GroupBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuRecent = new Crownwood.Magic.Menus.MenuCommand();
            this.menuStatement = new Crownwood.Magic.Menus.MenuCommand();
            this.menuAgreementDocs = new Crownwood.Magic.Menus.MenuCommand();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tpAgreement = new Crownwood.Magic.Controls.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtCODFlag = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_vouchervalue = new System.Windows.Forms.TextBox();
            this.txtRebate = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtTotalFees = new System.Windows.Forms.TextBox();
            this.txtDeposit = new System.Windows.Forms.TextBox();
            this.label52 = new System.Windows.Forms.Label();
            this.txtDeliveryTotal = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.txtDateDelivered = new System.Windows.Forms.TextBox();
            this.txtSChargeAmount = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtToFollowAmount = new System.Windows.Forms.TextBox();
            this.txtAmountPaid = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.txtAddToPotential = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.txtInstalments = new System.Windows.Forms.TextBox();
            this.txtInstalmentTotal = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.txtMthsInterestFree = new System.Windows.Forms.TextBox();
            this.txtLMovement = new System.Windows.Forms.TextBox();
            this.txtLastInstalmentAmount = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.txtDateLInstalment = new System.Windows.Forms.TextBox();
            this.txtIAmount = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.txtDateFInstalment = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtDateDue = new System.Windows.Forms.TextBox();
            this.txtEarlySettlementFigure = new System.Windows.Forms.TextBox();
            this.label53 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.Loyalty_HClogo_pb = new System.Windows.Forms.PictureBox();
            this.txtSegmentName = new System.Windows.Forms.TextBox();
            this.lSegmentId = new System.Windows.Forms.Label();
            this.txtBand = new System.Windows.Forms.TextBox();
            this.txtRate = new System.Windows.Forms.TextBox();
            this.lRate = new System.Windows.Forms.Label();
            this.lBand = new System.Windows.Forms.Label();
            this.lArchive = new System.Windows.Forms.Label();
            this.txtDateOpened = new System.Windows.Forms.TextBox();
            this.txtAccountType = new System.Windows.Forms.TextBox();
            this.txtArrears = new System.Windows.Forms.TextBox();
            this.txtOutstandingBalance = new System.Windows.Forms.TextBox();
            this.txtDateLastPaid = new System.Windows.Forms.TextBox();
            this.label41 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.txtAgreementTotal = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.txtTTDescription = new System.Windows.Forms.TextBox();
            this.txtCurrentStatus = new System.Windows.Forms.TextBox();
            this.txtPercentagePaid = new System.Windows.Forms.TextBox();
            this.label42 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.txtTTCode = new System.Windows.Forms.TextBox();
            this.tcMain = new Crownwood.Magic.Controls.TabControl();
            this.tpLineItems = new Crownwood.Magic.Controls.TabPage();
            this.gbLineItems = new System.Windows.Forms.GroupBox();
            this.dgLineItems = new System.Windows.Forms.DataGrid();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.tvItems = new System.Windows.Forms.TreeView();
            this.tpDeliveries = new Crownwood.Magic.Controls.TabPage();
            this.gbDelivery = new System.Windows.Forms.GroupBox();
            this.dgDelivery = new System.Windows.Forms.DataGrid();
            this.tpTransactions = new Crownwood.Magic.Controls.TabPage();
            this.tpAmortizationSchedule = new Crownwood.Magic.Controls.TabPage();
            this.dgAmortizationSchedule = new System.Windows.Forms.DataGrid();
            this.tpRFCombined = new Crownwood.Magic.Controls.TabPage();
            this.label57 = new System.Windows.Forms.Label();
            this.label56 = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.txtTotalBalances = new System.Windows.Forms.TextBox();
            this.txtTotalArrears = new System.Windows.Forms.TextBox();
            this.txtTotalInstalments = new System.Windows.Forms.TextBox();
            this.txtTotalAgreements = new System.Windows.Forms.TextBox();
            this.txtTotalCredit = new System.Windows.Forms.TextBox();
            this.txtAvailableCredit = new System.Windows.Forms.TextBox();
            this.txtCardPrinted = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtRFIssueNo = new System.Windows.Forms.TextBox();
            this.lRFIssueNo = new System.Windows.Forms.Label();
            this.tpEmployee = new Crownwood.Magic.Controls.TabPage();
            this.tpApplicationStatus = new Crownwood.Magic.Controls.TabPage();
            this.dgApplicationStatus = new System.Windows.Forms.DataGridView();
            this.tpLetters = new Crownwood.Magic.Controls.TabPage();
            this.dgStatus = new System.Windows.Forms.DataGrid();
            this.dgLetters = new System.Windows.Forms.DataGrid();
            this.tpAllocation = new Crownwood.Magic.Controls.TabPage();
            this.gbAllocationHist = new System.Windows.Forms.GroupBox();
            this.dgAllocationHist = new System.Windows.Forms.DataGrid();
            this.tpActions = new Crownwood.Magic.Controls.TabPage();
            this.gbAllocatedTo = new System.Windows.Forms.GroupBox();
            this.txtEmployeeType = new System.Windows.Forms.TextBox();
            this.txtEmployeeName = new System.Windows.Forms.TextBox();
            this.lEmpName = new System.Windows.Forms.Label();
            this.txtEmployeeNo = new System.Windows.Forms.TextBox();
            this.lEmployeeNo = new System.Windows.Forms.Label();
            this.lEmpType = new System.Windows.Forms.Label();
            this.pAllocation = new System.Windows.Forms.Panel();
            this.drpReason = new System.Windows.Forms.ComboBox();
            this.btnSPADetails = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.dtDueDate = new System.Windows.Forms.DateTimePicker();
            this.lbActionDate = new System.Windows.Forms.Label();
            this.txtActionValue = new System.Windows.Forms.TextBox();
            this.label65 = new System.Windows.Forms.Label();
            this.label67 = new System.Windows.Forms.Label();
            this.lbActionValue = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.drpActionCode = new System.Windows.Forms.ComboBox();
            this.lbReason = new System.Windows.Forms.Label();
            this.label64 = new System.Windows.Forms.Label();
            this.textNotes = new System.Windows.Forms.TextBox();
            this.dgActions = new System.Windows.Forms.DataGrid();
            this.tpAgreementChanges = new Crownwood.Magic.Controls.TabPage();
            this.dgAgreementAudit = new System.Windows.Forms.DataGrid();
            this.dgInstalPlan = new System.Windows.Forms.DataGrid();
            this.dgLineItem = new System.Windows.Forms.DataGrid();
            this.tpActivitySegments = new Crownwood.Magic.Controls.TabPage();
            this.dgArrangements = new System.Windows.Forms.DataGrid();
            this.dgActivities = new System.Windows.Forms.DataGrid();
            this.dgSegment = new System.Windows.Forms.DataGrid();
            this.tpPendingCharges = new Crownwood.Magic.Controls.TabPage();
            this.dgPendingCharges = new System.Windows.Forms.DataGrid();
            this.dgArrearsCharges = new System.Windows.Forms.DataGrid();
            this.tpServiceRequest = new Crownwood.Magic.Controls.TabPage();
            this.dgServiceRequestSummary = new System.Windows.Forms.DataGrid();
            this.tpStrategies = new Crownwood.Magic.Controls.TabPage();
            this.dgStrategies = new System.Windows.Forms.DataGrid();
            this.dgWorklists = new System.Windows.Forms.DataGrid();
            this.tpInstallation = new Crownwood.Magic.Controls.TabPage();
            this.dgvInstallation = new System.Windows.Forms.DataGridView();
            this.gbCustomer = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txtTotalSpend = new System.Windows.Forms.TextBox();
            this.labelCredit = new System.Windows.Forms.Label();
            this.lPrivilegeClubDesc = new System.Windows.Forms.Label();
            this.chxPrivilegeClub = new System.Windows.Forms.CheckBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnCustCodes = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.txtAccountNo = new System.Windows.Forms.TextBox();
            this.txtCustID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnToggle = new System.Windows.Forms.Button();
            this.tcAddress = new Crownwood.Magic.Controls.TabControl();
            this.dgAccounts = new System.Windows.Forms.DataGrid();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.tpAgreement.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Loyalty_HClogo_pb)).BeginInit();
            this.tcMain.SuspendLayout();
            this.tpLineItems.SuspendLayout();
            this.gbLineItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgLineItems)).BeginInit();
            this.tpDeliveries.SuspendLayout();
            this.gbDelivery.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDelivery)).BeginInit();
            this.tpAmortizationSchedule.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAmortizationSchedule)).BeginInit();
            this.tpRFCombined.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tpApplicationStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgApplicationStatus)).BeginInit();
            this.tpLetters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgLetters)).BeginInit();
            this.tpAllocation.SuspendLayout();
            this.gbAllocationHist.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAllocationHist)).BeginInit();
            this.tpActions.SuspendLayout();
            this.gbAllocatedTo.SuspendLayout();
            this.pAllocation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgActions)).BeginInit();
            this.tpAgreementChanges.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAgreementAudit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgInstalPlan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgLineItem)).BeginInit();
            this.tpActivitySegments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgArrangements)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgActivities)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgSegment)).BeginInit();
            this.tpPendingCharges.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgPendingCharges)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgArrearsCharges)).BeginInit();
            this.tpServiceRequest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgServiceRequestSummary)).BeginInit();
            this.tpStrategies.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgStrategies)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgWorklists)).BeginInit();
            this.tpInstallation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInstallation)).BeginInit();
            this.gbCustomer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // showAgreementChanges
            // 
            this.showAgreementChanges.Enabled = false;
            this.showAgreementChanges.Location = new System.Drawing.Point(0, 0);
            this.showAgreementChanges.Name = "showAgreementChanges";
            this.showAgreementChanges.Size = new System.Drawing.Size(0, 0);
            this.showAgreementChanges.TabIndex = 0;
            // 
            // label11
            // 
            this.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label11.Location = new System.Drawing.Point(16, 136);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(100, 23);
            this.label11.TabIndex = 1;
            this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label10
            // 
            this.label10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label10.Location = new System.Drawing.Point(16, 112);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(100, 23);
            this.label10.TabIndex = 1;
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label13
            // 
            this.label13.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label13.Location = new System.Drawing.Point(16, 184);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(100, 23);
            this.label13.TabIndex = 1;
            this.label13.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label12
            // 
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(16, 160);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(100, 23);
            this.label12.TabIndex = 1;
            this.label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label31
            // 
            this.label31.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label31.Location = new System.Drawing.Point(16, 16);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(120, 23);
            this.label31.TabIndex = 1;
            this.label31.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label38
            // 
            this.label38.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label38.Location = new System.Drawing.Point(32, 64);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(100, 23);
            this.label38.TabIndex = 3;
            // 
            // label37
            // 
            this.label37.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label37.Location = new System.Drawing.Point(8, 40);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(128, 23);
            this.label37.TabIndex = 2;
            // 
            // label33
            // 
            this.label33.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label33.Location = new System.Drawing.Point(48, 24);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(120, 23);
            this.label33.TabIndex = 1;
            this.label33.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(16, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 23);
            this.label5.TabIndex = 1;
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label6
            // 
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(24, 40);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 23);
            this.label6.TabIndex = 1;
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label27
            // 
            this.label27.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label27.Location = new System.Drawing.Point(64, 72);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(100, 23);
            this.label27.TabIndex = 1;
            this.label27.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label21
            // 
            this.label21.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label21.Location = new System.Drawing.Point(8, 16);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(80, 23);
            this.label21.TabIndex = 1;
            this.label21.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label8
            // 
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(16, 64);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 23);
            this.label8.TabIndex = 1;
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label9
            // 
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(8, 88);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(112, 23);
            this.label9.TabIndex = 1;
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // gbOtherAccounts
            // 
            this.gbOtherAccounts.Location = new System.Drawing.Point(392, 16);
            this.gbOtherAccounts.Name = "gbOtherAccounts";
            this.gbOtherAccounts.Size = new System.Drawing.Size(376, 136);
            this.gbOtherAccounts.TabIndex = 16;
            this.gbOtherAccounts.TabStop = false;
            this.gbOtherAccounts.Text = "Other Accounts";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuExit,
            this.menuRecent,
            this.menuStatement,
            this.menuAgreementDocs});
            this.menuFile.Text = "&File";
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.btnClose_Click_1);
            // 
            // menuRecent
            // 
            this.menuRecent.Description = "MenuItem";
            this.menuRecent.Text = "Recent &Accounts";
            // 
            // menuStatement
            // 
            this.menuStatement.Description = "MenuItem";
            this.menuStatement.Text = "Print Statement of Account";
            this.menuStatement.Click += new System.EventHandler(this.menuStatement_Click);
            // 
            // menuAgreementDocs
            // 
            this.menuAgreementDocs.Description = "MenuItem";
            this.menuAgreementDocs.Text = "Print Agreement Documents";
            this.menuAgreementDocs.Click += new System.EventHandler(this.menuAgreementDocs_Click);
            // 
            // tpAgreement
            // 
            this.tpAgreement.Controls.Add(this.groupBox7);
            this.tpAgreement.Location = new System.Drawing.Point(0, 33);
            this.tpAgreement.Name = "tpAgreement";
            this.tpAgreement.Selected = false;
            this.tpAgreement.Size = new System.Drawing.Size(784, 279);
            this.tpAgreement.TabIndex = 3;
            this.tpAgreement.Title = "Agreement Details";
            this.toolTip1.SetToolTip(this.tpAgreement, "Agreement Details");
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.panel1);
            this.groupBox7.Controls.Add(this.panel4);
            this.groupBox7.Controls.Add(this.panel3);
            this.groupBox7.Location = new System.Drawing.Point(0, -8);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(776, 280);
            this.groupBox7.TabIndex = 8;
            this.groupBox7.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtCODFlag);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txt_vouchervalue);
            this.panel1.Controls.Add(this.txtRebate);
            this.panel1.Controls.Add(this.label19);
            this.panel1.Controls.Add(this.txtTotalFees);
            this.panel1.Controls.Add(this.txtDeposit);
            this.panel1.Controls.Add(this.label52);
            this.panel1.Controls.Add(this.txtDeliveryTotal);
            this.panel1.Controls.Add(this.label26);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.label50);
            this.panel1.Controls.Add(this.label22);
            this.panel1.Controls.Add(this.label18);
            this.panel1.Controls.Add(this.txtDateDelivered);
            this.panel1.Controls.Add(this.txtSChargeAmount);
            this.panel1.Controls.Add(this.label15);
            this.panel1.Controls.Add(this.txtToFollowAmount);
            this.panel1.Controls.Add(this.txtAmountPaid);
            this.panel1.Controls.Add(this.label24);
            this.panel1.Controls.Add(this.txtAddToPotential);
            this.panel1.Controls.Add(this.label23);
            this.panel1.Location = new System.Drawing.Point(557, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 256);
            this.panel1.TabIndex = 17;
            // 
            // txtCODFlag
            // 
            this.txtCODFlag.Location = new System.Drawing.Point(167, 80);
            this.txtCODFlag.Name = "txtCODFlag";
            this.txtCODFlag.ReadOnly = true;
            this.txtCODFlag.Size = new System.Drawing.Size(24, 31);
            this.txtCODFlag.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(4, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 16);
            this.label4.TabIndex = 28;
            this.label4.Text = "Voucher";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.label4, "Loyalty Club voucher discount on service change.");
            // 
            // txt_vouchervalue
            // 
            this.txt_vouchervalue.Location = new System.Drawing.Point(67, 80);
            this.txt_vouchervalue.Name = "txt_vouchervalue";
            this.txt_vouchervalue.ReadOnly = true;
            this.txt_vouchervalue.Size = new System.Drawing.Size(65, 31);
            this.txt_vouchervalue.TabIndex = 29;
            this.toolTip1.SetToolTip(this.txt_vouchervalue, "Loyalty Club voucher discount on service change.");
            // 
            // txtRebate
            // 
            this.txtRebate.Location = new System.Drawing.Point(104, 230);
            this.txtRebate.Name = "txtRebate";
            this.txtRebate.ReadOnly = true;
            this.txtRebate.Size = new System.Drawing.Size(72, 31);
            this.txtRebate.TabIndex = 27;
            // 
            // label19
            // 
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label19.Location = new System.Drawing.Point(42, 232);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(56, 16);
            this.label19.TabIndex = 26;
            this.label19.Text = "Rebate";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTotalFees
            // 
            this.txtTotalFees.Location = new System.Drawing.Point(104, 105);
            this.txtTotalFees.Name = "txtTotalFees";
            this.txtTotalFees.ReadOnly = true;
            this.txtTotalFees.Size = new System.Drawing.Size(88, 31);
            this.txtTotalFees.TabIndex = 3;
            // 
            // txtDeposit
            // 
            this.txtDeposit.Location = new System.Drawing.Point(104, 30);
            this.txtDeposit.Name = "txtDeposit";
            this.txtDeposit.ReadOnly = true;
            this.txtDeposit.Size = new System.Drawing.Size(88, 31);
            this.txtDeposit.TabIndex = 1;
            // 
            // label52
            // 
            this.label52.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label52.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label52.Location = new System.Drawing.Point(139, 82);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(38, 20);
            this.label52.TabIndex = 5;
            this.label52.Text = "COD";
            this.label52.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtDeliveryTotal
            // 
            this.txtDeliveryTotal.Location = new System.Drawing.Point(104, 130);
            this.txtDeliveryTotal.Name = "txtDeliveryTotal";
            this.txtDeliveryTotal.ReadOnly = true;
            this.txtDeliveryTotal.Size = new System.Drawing.Size(88, 31);
            this.txtDeliveryTotal.TabIndex = 0;
            // 
            // label26
            // 
            this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label26.Location = new System.Drawing.Point(10, 132);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(88, 16);
            this.label26.TabIndex = 1;
            this.label26.Text = "Delivery Total";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label14.Location = new System.Drawing.Point(26, 32);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(72, 16);
            this.label14.TabIndex = 0;
            this.label14.Text = "Deposit";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label50
            // 
            this.label50.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label50.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label50.Location = new System.Drawing.Point(2, 107);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(96, 16);
            this.label50.TabIndex = 2;
            this.label50.Text = "Total Int + Fees";
            this.label50.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label22
            // 
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label22.Location = new System.Drawing.Point(10, 57);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(88, 16);
            this.label22.TabIndex = 0;
            this.label22.Text = "Service Charge";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.label22, "Service Charge on account minus the voucher value.");
            // 
            // label18
            // 
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label18.Location = new System.Drawing.Point(10, 7);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(88, 16);
            this.label18.TabIndex = 1;
            this.label18.Text = "Date Delivered";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDateDelivered
            // 
            this.txtDateDelivered.Location = new System.Drawing.Point(104, 5);
            this.txtDateDelivered.Name = "txtDateDelivered";
            this.txtDateDelivered.ReadOnly = true;
            this.txtDateDelivered.Size = new System.Drawing.Size(72, 31);
            this.txtDateDelivered.TabIndex = 0;
            // 
            // txtSChargeAmount
            // 
            this.txtSChargeAmount.Location = new System.Drawing.Point(104, 55);
            this.txtSChargeAmount.Name = "txtSChargeAmount";
            this.txtSChargeAmount.ReadOnly = true;
            this.txtSChargeAmount.Size = new System.Drawing.Size(88, 31);
            this.txtSChargeAmount.TabIndex = 1;
            this.toolTip1.SetToolTip(this.txtSChargeAmount, "Service Charge on account minus the voucher value.");
            // 
            // label15
            // 
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label15.Location = new System.Drawing.Point(2, 157);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(96, 16);
            this.label15.TabIndex = 18;
            this.label15.Text = "To Follow Amount";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtToFollowAmount
            // 
            this.txtToFollowAmount.Location = new System.Drawing.Point(104, 155);
            this.txtToFollowAmount.Name = "txtToFollowAmount";
            this.txtToFollowAmount.ReadOnly = true;
            this.txtToFollowAmount.Size = new System.Drawing.Size(88, 31);
            this.txtToFollowAmount.TabIndex = 21;
            // 
            // txtAmountPaid
            // 
            this.txtAmountPaid.Location = new System.Drawing.Point(104, 180);
            this.txtAmountPaid.Name = "txtAmountPaid";
            this.txtAmountPaid.ReadOnly = true;
            this.txtAmountPaid.Size = new System.Drawing.Size(88, 31);
            this.txtAmountPaid.TabIndex = 22;
            // 
            // label24
            // 
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label24.Location = new System.Drawing.Point(26, 182);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(72, 16);
            this.label24.TabIndex = 20;
            this.label24.Text = "Amount Paid";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAddToPotential
            // 
            this.txtAddToPotential.Location = new System.Drawing.Point(104, 205);
            this.txtAddToPotential.Name = "txtAddToPotential";
            this.txtAddToPotential.ReadOnly = true;
            this.txtAddToPotential.Size = new System.Drawing.Size(88, 31);
            this.txtAddToPotential.TabIndex = 0;
            // 
            // label23
            // 
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label23.Location = new System.Drawing.Point(2, 207);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(96, 16);
            this.label23.TabIndex = 1;
            this.label23.Text = "Add To Potential";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.txtInstalments);
            this.panel4.Controls.Add(this.txtInstalmentTotal);
            this.panel4.Controls.Add(this.label16);
            this.panel4.Controls.Add(this.label49);
            this.panel4.Controls.Add(this.txtMthsInterestFree);
            this.panel4.Controls.Add(this.txtLMovement);
            this.panel4.Controls.Add(this.txtLastInstalmentAmount);
            this.panel4.Controls.Add(this.label36);
            this.panel4.Controls.Add(this.label32);
            this.panel4.Controls.Add(this.label35);
            this.panel4.Controls.Add(this.label34);
            this.panel4.Controls.Add(this.txtDateLInstalment);
            this.panel4.Controls.Add(this.txtIAmount);
            this.panel4.Controls.Add(this.label30);
            this.panel4.Controls.Add(this.label28);
            this.panel4.Controls.Add(this.txtDateFInstalment);
            this.panel4.Controls.Add(this.label20);
            this.panel4.Controls.Add(this.txtDateDue);
            this.panel4.Controls.Add(this.txtEarlySettlementFigure);
            this.panel4.Controls.Add(this.label53);
            this.panel4.Location = new System.Drawing.Point(318, 16);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(232, 256);
            this.panel4.TabIndex = 15;
            // 
            // txtInstalments
            // 
            this.txtInstalments.Location = new System.Drawing.Point(136, 32);
            this.txtInstalments.Name = "txtInstalments";
            this.txtInstalments.ReadOnly = true;
            this.txtInstalments.Size = new System.Drawing.Size(24, 31);
            this.txtInstalments.TabIndex = 0;
            // 
            // txtInstalmentTotal
            // 
            this.txtInstalmentTotal.Location = new System.Drawing.Point(136, 152);
            this.txtInstalmentTotal.Name = "txtInstalmentTotal";
            this.txtInstalmentTotal.ReadOnly = true;
            this.txtInstalmentTotal.Size = new System.Drawing.Size(72, 31);
            this.txtInstalmentTotal.TabIndex = 0;
            // 
            // label16
            // 
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label16.Location = new System.Drawing.Point(11, 82);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(120, 16);
            this.label16.TabIndex = 1;
            this.label16.Text = "Instalment Amount";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label49
            // 
            this.label49.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label49.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label49.Location = new System.Drawing.Point(11, 178);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(120, 16);
            this.label49.TabIndex = 9;
            this.label49.Text = "Months Interest Free";
            this.label49.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtMthsInterestFree
            // 
            this.txtMthsInterestFree.Location = new System.Drawing.Point(136, 176);
            this.txtMthsInterestFree.Name = "txtMthsInterestFree";
            this.txtMthsInterestFree.ReadOnly = true;
            this.txtMthsInterestFree.Size = new System.Drawing.Size(24, 31);
            this.txtMthsInterestFree.TabIndex = 10;
            // 
            // txtLMovement
            // 
            this.txtLMovement.Location = new System.Drawing.Point(136, 128);
            this.txtLMovement.Name = "txtLMovement";
            this.txtLMovement.ReadOnly = true;
            this.txtLMovement.Size = new System.Drawing.Size(72, 31);
            this.txtLMovement.TabIndex = 0;
            // 
            // txtLastInstalmentAmount
            // 
            this.txtLastInstalmentAmount.Location = new System.Drawing.Point(136, 104);
            this.txtLastInstalmentAmount.Name = "txtLastInstalmentAmount";
            this.txtLastInstalmentAmount.ReadOnly = true;
            this.txtLastInstalmentAmount.Size = new System.Drawing.Size(72, 31);
            this.txtLastInstalmentAmount.TabIndex = 4;
            // 
            // label36
            // 
            this.label36.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label36.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label36.Location = new System.Drawing.Point(27, 154);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(104, 16);
            this.label36.TabIndex = 1;
            this.label36.Text = "Instalment Total";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label32
            // 
            this.label32.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label32.Location = new System.Drawing.Point(3, 58);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(128, 16);
            this.label32.TabIndex = 1;
            this.label32.Text = "Date of Last Instalment";
            this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label35
            // 
            this.label35.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label35.Location = new System.Drawing.Point(27, 130);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(104, 16);
            this.label35.TabIndex = 1;
            this.label35.Text = "Last Movement";
            this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label34
            // 
            this.label34.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label34.Location = new System.Drawing.Point(6, 106);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(125, 16);
            this.label34.TabIndex = 1;
            this.label34.Text = "Last Instalment Amount";
            this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDateLInstalment
            // 
            this.txtDateLInstalment.Location = new System.Drawing.Point(136, 56);
            this.txtDateLInstalment.Name = "txtDateLInstalment";
            this.txtDateLInstalment.ReadOnly = true;
            this.txtDateLInstalment.Size = new System.Drawing.Size(72, 31);
            this.txtDateLInstalment.TabIndex = 0;
            // 
            // txtIAmount
            // 
            this.txtIAmount.Location = new System.Drawing.Point(136, 80);
            this.txtIAmount.Name = "txtIAmount";
            this.txtIAmount.ReadOnly = true;
            this.txtIAmount.Size = new System.Drawing.Size(72, 31);
            this.txtIAmount.TabIndex = 0;
            // 
            // label30
            // 
            this.label30.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label30.Location = new System.Drawing.Point(27, 34);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(104, 16);
            this.label30.TabIndex = 1;
            this.label30.Text = "No of Instalments";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label28
            // 
            this.label28.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label28.Location = new System.Drawing.Point(3, 10);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(128, 16);
            this.label28.TabIndex = 1;
            this.label28.Text = "Date of First Instalment";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDateFInstalment
            // 
            this.txtDateFInstalment.Location = new System.Drawing.Point(136, 8);
            this.txtDateFInstalment.Name = "txtDateFInstalment";
            this.txtDateFInstalment.ReadOnly = true;
            this.txtDateFInstalment.Size = new System.Drawing.Size(72, 31);
            this.txtDateFInstalment.TabIndex = 0;
            // 
            // label20
            // 
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label20.Location = new System.Drawing.Point(59, 202);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(72, 16);
            this.label20.TabIndex = 1;
            this.label20.Text = "Day Due";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDateDue
            // 
            this.txtDateDue.Location = new System.Drawing.Point(136, 200);
            this.txtDateDue.Name = "txtDateDue";
            this.txtDateDue.ReadOnly = true;
            this.txtDateDue.Size = new System.Drawing.Size(24, 31);
            this.txtDateDue.TabIndex = 0;
            // 
            // txtEarlySettlementFigure
            // 
            this.txtEarlySettlementFigure.Location = new System.Drawing.Point(136, 224);
            this.txtEarlySettlementFigure.Name = "txtEarlySettlementFigure";
            this.txtEarlySettlementFigure.ReadOnly = true;
            this.txtEarlySettlementFigure.Size = new System.Drawing.Size(72, 31);
            this.txtEarlySettlementFigure.TabIndex = 12;
            // 
            // label53
            // 
            this.label53.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label53.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label53.Location = new System.Drawing.Point(9, 226);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(122, 16);
            this.label53.TabIndex = 11;
            this.label53.Text = "Early Settlement Figure";
            this.label53.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.Loyalty_HClogo_pb);
            this.panel3.Controls.Add(this.txtSegmentName);
            this.panel3.Controls.Add(this.lSegmentId);
            this.panel3.Controls.Add(this.txtBand);
            this.panel3.Controls.Add(this.txtRate);
            this.panel3.Controls.Add(this.lRate);
            this.panel3.Controls.Add(this.lBand);
            this.panel3.Controls.Add(this.lArchive);
            this.panel3.Controls.Add(this.txtDateOpened);
            this.panel3.Controls.Add(this.txtAccountType);
            this.panel3.Controls.Add(this.txtArrears);
            this.panel3.Controls.Add(this.txtOutstandingBalance);
            this.panel3.Controls.Add(this.txtDateLastPaid);
            this.panel3.Controls.Add(this.label41);
            this.panel3.Controls.Add(this.label40);
            this.panel3.Controls.Add(this.label39);
            this.panel3.Controls.Add(this.txtAgreementTotal);
            this.panel3.Controls.Add(this.label29);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.label25);
            this.panel3.Controls.Add(this.txtTTDescription);
            this.panel3.Controls.Add(this.txtCurrentStatus);
            this.panel3.Controls.Add(this.txtPercentagePaid);
            this.panel3.Controls.Add(this.label42);
            this.panel3.Controls.Add(this.label43);
            this.panel3.Controls.Add(this.label44);
            this.panel3.Controls.Add(this.txtTTCode);
            this.panel3.Location = new System.Drawing.Point(8, 16);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(304, 256);
            this.panel3.TabIndex = 9;
            // 
            // Loyalty_HClogo_pb
            // 
            this.Loyalty_HClogo_pb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Loyalty_HClogo_pb.Image = ((System.Drawing.Image)(resources.GetObject("Loyalty_HClogo_pb.Image")));
            this.Loyalty_HClogo_pb.Location = new System.Drawing.Point(222, 128);
            this.Loyalty_HClogo_pb.Name = "Loyalty_HClogo_pb";
            this.Loyalty_HClogo_pb.Size = new System.Drawing.Size(75, 21);
            this.Loyalty_HClogo_pb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Loyalty_HClogo_pb.TabIndex = 64;
            this.Loyalty_HClogo_pb.TabStop = false;
            this.Loyalty_HClogo_pb.Visible = false;
            // 
            // txtSegmentName
            // 
            this.txtSegmentName.Location = new System.Drawing.Point(209, 152);
            this.txtSegmentName.Name = "txtSegmentName";
            this.txtSegmentName.ReadOnly = true;
            this.txtSegmentName.Size = new System.Drawing.Size(88, 31);
            this.txtSegmentName.TabIndex = 63;
            // 
            // lSegmentId
            // 
            this.lSegmentId.AutoSize = true;
            this.lSegmentId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lSegmentId.Location = new System.Drawing.Point(153, 155);
            this.lSegmentId.Name = "lSegmentId";
            this.lSegmentId.Size = new System.Drawing.Size(75, 20);
            this.lSegmentId.TabIndex = 62;
            this.lSegmentId.Text = "Segment";
            this.lSegmentId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBand
            // 
            this.txtBand.Location = new System.Drawing.Point(256, 224);
            this.txtBand.Name = "txtBand";
            this.txtBand.ReadOnly = true;
            this.txtBand.Size = new System.Drawing.Size(40, 31);
            this.txtBand.TabIndex = 61;
            // 
            // txtRate
            // 
            this.txtRate.Location = new System.Drawing.Point(120, 224);
            this.txtRate.Name = "txtRate";
            this.txtRate.ReadOnly = true;
            this.txtRate.Size = new System.Drawing.Size(48, 31);
            this.txtRate.TabIndex = 60;
            // 
            // lRate
            // 
            this.lRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lRate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lRate.Location = new System.Drawing.Point(3, 225);
            this.lRate.Name = "lRate";
            this.lRate.Size = new System.Drawing.Size(111, 19);
            this.lRate.TabIndex = 59;
            this.lRate.Text = "Service Charge  %";
            this.lRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lRate.Visible = false;
            // 
            // lBand
            // 
            this.lBand.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lBand.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lBand.Location = new System.Drawing.Point(162, 226);
            this.lBand.Name = "lBand";
            this.lBand.Size = new System.Drawing.Size(90, 16);
            this.lBand.TabIndex = 57;
            this.lBand.Text = "Scoring Band";
            this.lBand.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lBand.Visible = false;
            // 
            // lArchive
            // 
            this.lArchive.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
            this.lArchive.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lArchive.Location = new System.Drawing.Point(192, 12);
            this.lArchive.Name = "lArchive";
            this.lArchive.Size = new System.Drawing.Size(104, 16);
            this.lArchive.TabIndex = 55;
            this.lArchive.Text = "Archived Account";
            this.lArchive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lArchive.Visible = false;
            // 
            // txtDateOpened
            // 
            this.txtDateOpened.Location = new System.Drawing.Point(120, 32);
            this.txtDateOpened.Name = "txtDateOpened";
            this.txtDateOpened.ReadOnly = true;
            this.txtDateOpened.Size = new System.Drawing.Size(72, 31);
            this.txtDateOpened.TabIndex = 4;
            // 
            // txtAccountType
            // 
            this.txtAccountType.Location = new System.Drawing.Point(120, 8);
            this.txtAccountType.Name = "txtAccountType";
            this.txtAccountType.ReadOnly = true;
            this.txtAccountType.Size = new System.Drawing.Size(40, 31);
            this.txtAccountType.TabIndex = 3;
            // 
            // txtArrears
            // 
            this.txtArrears.Location = new System.Drawing.Point(120, 128);
            this.txtArrears.Name = "txtArrears";
            this.txtArrears.ReadOnly = true;
            this.txtArrears.Size = new System.Drawing.Size(96, 31);
            this.txtArrears.TabIndex = 6;
            // 
            // txtOutstandingBalance
            // 
            this.txtOutstandingBalance.Location = new System.Drawing.Point(120, 104);
            this.txtOutstandingBalance.Name = "txtOutstandingBalance";
            this.txtOutstandingBalance.ReadOnly = true;
            this.txtOutstandingBalance.Size = new System.Drawing.Size(96, 31);
            this.txtOutstandingBalance.TabIndex = 4;
            // 
            // txtDateLastPaid
            // 
            this.txtDateLastPaid.Location = new System.Drawing.Point(120, 80);
            this.txtDateLastPaid.Name = "txtDateLastPaid";
            this.txtDateLastPaid.ReadOnly = true;
            this.txtDateLastPaid.Size = new System.Drawing.Size(72, 31);
            this.txtDateLastPaid.TabIndex = 4;
            // 
            // label41
            // 
            this.label41.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label41.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label41.Location = new System.Drawing.Point(35, 130);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(80, 16);
            this.label41.TabIndex = 2;
            this.label41.Text = "Arrears";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label40
            // 
            this.label40.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label40.Location = new System.Drawing.Point(3, 106);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(112, 16);
            this.label40.TabIndex = 2;
            this.label40.Text = "Outstanding Balance";
            this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label39
            // 
            this.label39.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label39.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label39.Location = new System.Drawing.Point(19, 82);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(96, 16);
            this.label39.TabIndex = 2;
            this.label39.Text = "Date Last Paid";
            this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAgreementTotal
            // 
            this.txtAgreementTotal.Location = new System.Drawing.Point(120, 56);
            this.txtAgreementTotal.Name = "txtAgreementTotal";
            this.txtAgreementTotal.ReadOnly = true;
            this.txtAgreementTotal.Size = new System.Drawing.Size(96, 31);
            this.txtAgreementTotal.TabIndex = 0;
            // 
            // label29
            // 
            this.label29.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label29.Location = new System.Drawing.Point(27, 34);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(88, 16);
            this.label29.TabIndex = 1;
            this.label29.Text = "Date Opened";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(19, 58);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(96, 16);
            this.label7.TabIndex = 1;
            this.label7.Text = "Agreement Total";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label25
            // 
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label25.Location = new System.Drawing.Point(27, 10);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(88, 16);
            this.label25.TabIndex = 0;
            this.label25.Text = "Account Type";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTTDescription
            // 
            this.txtTTDescription.Location = new System.Drawing.Point(120, 200);
            this.txtTTDescription.Name = "txtTTDescription";
            this.txtTTDescription.ReadOnly = true;
            this.txtTTDescription.Size = new System.Drawing.Size(136, 31);
            this.txtTTDescription.TabIndex = 4;
            // 
            // txtCurrentStatus
            // 
            this.txtCurrentStatus.Location = new System.Drawing.Point(120, 152);
            this.txtCurrentStatus.Name = "txtCurrentStatus";
            this.txtCurrentStatus.ReadOnly = true;
            this.txtCurrentStatus.Size = new System.Drawing.Size(24, 31);
            this.txtCurrentStatus.TabIndex = 7;
            // 
            // txtPercentagePaid
            // 
            this.txtPercentagePaid.Location = new System.Drawing.Point(120, 176);
            this.txtPercentagePaid.Name = "txtPercentagePaid";
            this.txtPercentagePaid.ReadOnly = true;
            this.txtPercentagePaid.Size = new System.Drawing.Size(40, 31);
            this.txtPercentagePaid.TabIndex = 4;
            // 
            // label42
            // 
            this.label42.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label42.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label42.Location = new System.Drawing.Point(19, 154);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(96, 16);
            this.label42.TabIndex = 2;
            this.label42.Text = "Current Status";
            this.label42.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label43
            // 
            this.label43.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label43.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label43.Location = new System.Drawing.Point(11, 178);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(104, 16);
            this.label43.TabIndex = 2;
            this.label43.Text = "Percentage Paid";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label44
            // 
            this.label44.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label44.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label44.Location = new System.Drawing.Point(27, 202);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(88, 16);
            this.label44.TabIndex = 2;
            this.label44.Text = "Terms Type";
            this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTTCode
            // 
            this.txtTTCode.Location = new System.Drawing.Point(256, 200);
            this.txtTTCode.Name = "txtTTCode";
            this.txtTTCode.ReadOnly = true;
            this.txtTTCode.Size = new System.Drawing.Size(40, 31);
            this.txtTTCode.TabIndex = 5;
            // 
            // tcMain
            // 
            this.tcMain.IDEPixelArea = true;
            this.tcMain.Location = new System.Drawing.Point(3, 166);
            this.tcMain.Name = "tcMain";
            this.tcMain.PositionTop = true;
            this.tcMain.SelectedIndex = 3;
            this.tcMain.SelectedTab = this.tpTransactions;
            this.tcMain.ShowArrows = true;
            this.tcMain.ShrinkPagesToFit = false;
            this.tcMain.Size = new System.Drawing.Size(784, 312);
            this.tcMain.TabIndex = 3;
            this.tcMain.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpAgreement,
            this.tpLineItems,
            this.tpDeliveries,
            this.tpTransactions,
            this.tpAmortizationSchedule,
            this.tpRFCombined,
            this.tpEmployee,
            this.tpApplicationStatus,
            this.tpLetters,
            this.tpAllocation,
            this.tpActions,
            this.tpAgreementChanges,
            this.tpActivitySegments,
            this.tpPendingCharges,
            this.tpServiceRequest,
            this.tpStrategies,
            this.tpInstallation});
            this.tcMain.SelectionChanged += new System.EventHandler(this.tcMain_SelectionChanged);
            // 
            // tpLineItems
            // 
            this.tpLineItems.Controls.Add(this.gbLineItems);
            this.tpLineItems.Location = new System.Drawing.Point(0, 33);
            this.tpLineItems.Name = "tpLineItems";
            this.tpLineItems.Selected = false;
            this.tpLineItems.Size = new System.Drawing.Size(784, 279);
            this.tpLineItems.TabIndex = 4;
            this.tpLineItems.Title = "Line Items";
            // 
            // gbLineItems
            // 
            this.gbLineItems.Controls.Add(this.dgLineItems);
            this.gbLineItems.Controls.Add(this.splitter1);
            this.gbLineItems.Controls.Add(this.tvItems);
            this.gbLineItems.Location = new System.Drawing.Point(4, -1);
            this.gbLineItems.Name = "gbLineItems";
            this.gbLineItems.Size = new System.Drawing.Size(768, 273);
            this.gbLineItems.TabIndex = 2;
            this.gbLineItems.TabStop = false;
            this.gbLineItems.Text = "Line Items";
            // 
            // dgLineItems
            // 
            this.dgLineItems.CaptionText = "Line Items";
            this.dgLineItems.DataMember = "";
            this.dgLineItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgLineItems.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgLineItems.Location = new System.Drawing.Point(121, 27);
            this.dgLineItems.Name = "dgLineItems";
            this.dgLineItems.ReadOnly = true;
            this.dgLineItems.Size = new System.Drawing.Size(644, 243);
            this.dgLineItems.TabIndex = 2;
            // 
            // splitter1
            // 
            this.splitter1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.splitter1.Location = new System.Drawing.Point(118, 27);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 243);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // tvItems
            // 
            this.tvItems.Dock = System.Windows.Forms.DockStyle.Left;
            this.tvItems.ImageIndex = 0;
            this.tvItems.ImageList = this.imageList1;
            this.tvItems.Indent = 19;
            this.tvItems.ItemHeight = 17;
            this.tvItems.Location = new System.Drawing.Point(3, 27);
            this.tvItems.Name = "tvItems";
            this.tvItems.SelectedImageIndex = 0;
            this.tvItems.Size = new System.Drawing.Size(115, 243);
            this.tvItems.TabIndex = 0;
            this.tvItems.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvItems_AfterSelect);
            // 
            // tpDeliveries
            // 
            this.tpDeliveries.Controls.Add(this.gbDelivery);
            this.tpDeliveries.Location = new System.Drawing.Point(0, 33);
            this.tpDeliveries.Name = "tpDeliveries";
            this.tpDeliveries.Selected = false;
            this.tpDeliveries.Size = new System.Drawing.Size(784, 279);
            this.tpDeliveries.TabIndex = 17;
            this.tpDeliveries.Title = "Deliveries";
            // 
            // gbDelivery
            // 
            this.gbDelivery.Controls.Add(this.dgDelivery);
            this.gbDelivery.Location = new System.Drawing.Point(9, 3);
            this.gbDelivery.Name = "gbDelivery";
            this.gbDelivery.Size = new System.Drawing.Size(768, 269);
            this.gbDelivery.TabIndex = 4;
            this.gbDelivery.TabStop = false;
            this.gbDelivery.Text = "Delivery";
            // 
            // dgDelivery
            // 
            this.dgDelivery.CaptionText = "Delivery History";
            this.dgDelivery.CausesValidation = false;
            this.dgDelivery.DataMember = "";
            this.dgDelivery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgDelivery.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgDelivery.Location = new System.Drawing.Point(3, 27);
            this.dgDelivery.Name = "dgDelivery";
            this.dgDelivery.ReadOnly = true;
            this.dgDelivery.Size = new System.Drawing.Size(762, 239);
            this.dgDelivery.TabIndex = 0;
            // 
            // tpTransactions
            // 
            this.tpTransactions.Location = new System.Drawing.Point(0, 33);
            this.tpTransactions.Name = "tpTransactions";
            this.tpTransactions.Size = new System.Drawing.Size(784, 279);
            this.tpTransactions.TabIndex = 5;
            this.tpTransactions.Title = "Transactions";
            // 
            // tpAmortizationSchedule
            // 
            this.tpAmortizationSchedule.Controls.Add(this.dgAmortizationSchedule);
            this.tpAmortizationSchedule.Location = new System.Drawing.Point(0, 33);
            this.tpAmortizationSchedule.Name = "tpAmortizationSchedule";
            this.tpAmortizationSchedule.Selected = false;
            this.tpAmortizationSchedule.Size = new System.Drawing.Size(784, 279);
            this.tpAmortizationSchedule.TabIndex = 18;
            this.tpAmortizationSchedule.Title = "Amortization Schedule";
            // 
            // dgAmortizationSchedule
            // 
            this.dgAmortizationSchedule.CaptionText = "Amortization Schedule";
            this.dgAmortizationSchedule.DataMember = "";
            this.dgAmortizationSchedule.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAmortizationSchedule.Location = new System.Drawing.Point(8, 8);
            this.dgAmortizationSchedule.Name = "dgAmortizationSchedule";
            this.dgAmortizationSchedule.ReadOnly = true;
            this.dgAmortizationSchedule.Size = new System.Drawing.Size(765, 260);
            this.dgAmortizationSchedule.TabIndex = 19;
            this.dgAmortizationSchedule.TabStop = false;
            // 
            // tpRFCombined
            // 
            this.tpRFCombined.Controls.Add(this.label57);
            this.tpRFCombined.Controls.Add(this.label56);
            this.tpRFCombined.Controls.Add(this.label55);
            this.tpRFCombined.Controls.Add(this.label51);
            this.tpRFCombined.Controls.Add(this.label48);
            this.tpRFCombined.Controls.Add(this.label47);
            this.tpRFCombined.Controls.Add(this.label46);
            this.tpRFCombined.Controls.Add(this.txtTotalBalances);
            this.tpRFCombined.Controls.Add(this.txtTotalArrears);
            this.tpRFCombined.Controls.Add(this.txtTotalInstalments);
            this.tpRFCombined.Controls.Add(this.txtTotalAgreements);
            this.tpRFCombined.Controls.Add(this.txtTotalCredit);
            this.tpRFCombined.Controls.Add(this.txtAvailableCredit);
            this.tpRFCombined.Controls.Add(this.txtCardPrinted);
            this.tpRFCombined.Controls.Add(this.groupBox1);
            this.tpRFCombined.Location = new System.Drawing.Point(0, 33);
            this.tpRFCombined.Name = "tpRFCombined";
            this.tpRFCombined.Selected = false;
            this.tpRFCombined.Size = new System.Drawing.Size(784, 279);
            this.tpRFCombined.TabIndex = 6;
            this.tpRFCombined.Title = "Credit Details";
            // 
            // label57
            // 
            this.label57.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label57.Location = new System.Drawing.Point(400, 56);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(104, 16);
            this.label57.TabIndex = 14;
            this.label57.Text = "Total Arrears";
            this.label57.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label56
            // 
            this.label56.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label56.Location = new System.Drawing.Point(400, 104);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(104, 16);
            this.label56.TabIndex = 13;
            this.label56.Text = "Total Balances";
            this.label56.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label55
            // 
            this.label55.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label55.Location = new System.Drawing.Point(128, 200);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(120, 16);
            this.label55.TabIndex = 12;
            this.label55.Text = "Total Agreements";
            this.label55.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label51
            // 
            this.label51.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label51.Location = new System.Drawing.Point(384, 200);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(120, 16);
            this.label51.TabIndex = 11;
            this.label51.Text = "Total Instalments";
            this.label51.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label48
            // 
            this.label48.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label48.Location = new System.Drawing.Point(392, 152);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(112, 16);
            this.label48.TabIndex = 10;
            this.label48.Text = "Credit Limit";
            this.label48.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label47
            // 
            this.label47.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label47.Location = new System.Drawing.Point(136, 56);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(112, 16);
            this.label47.TabIndex = 9;
            this.label47.Text = "Credit Available";
            this.label47.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label46
            // 
            this.label46.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label46.Location = new System.Drawing.Point(144, 104);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(104, 16);
            this.label46.TabIndex = 8;
            this.label46.Text = "Card Printed";
            this.label46.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTotalBalances
            // 
            this.txtTotalBalances.Location = new System.Drawing.Point(512, 104);
            this.txtTotalBalances.Name = "txtTotalBalances";
            this.txtTotalBalances.ReadOnly = true;
            this.txtTotalBalances.Size = new System.Drawing.Size(72, 31);
            this.txtTotalBalances.TabIndex = 7;
            // 
            // txtTotalArrears
            // 
            this.txtTotalArrears.Location = new System.Drawing.Point(512, 56);
            this.txtTotalArrears.Name = "txtTotalArrears";
            this.txtTotalArrears.ReadOnly = true;
            this.txtTotalArrears.Size = new System.Drawing.Size(72, 31);
            this.txtTotalArrears.TabIndex = 6;
            // 
            // txtTotalInstalments
            // 
            this.txtTotalInstalments.Location = new System.Drawing.Point(512, 200);
            this.txtTotalInstalments.Name = "txtTotalInstalments";
            this.txtTotalInstalments.ReadOnly = true;
            this.txtTotalInstalments.Size = new System.Drawing.Size(72, 31);
            this.txtTotalInstalments.TabIndex = 5;
            // 
            // txtTotalAgreements
            // 
            this.txtTotalAgreements.Location = new System.Drawing.Point(256, 200);
            this.txtTotalAgreements.Name = "txtTotalAgreements";
            this.txtTotalAgreements.ReadOnly = true;
            this.txtTotalAgreements.Size = new System.Drawing.Size(72, 31);
            this.txtTotalAgreements.TabIndex = 4;
            // 
            // txtTotalCredit
            // 
            this.txtTotalCredit.Location = new System.Drawing.Point(512, 152);
            this.txtTotalCredit.Name = "txtTotalCredit";
            this.txtTotalCredit.ReadOnly = true;
            this.txtTotalCredit.Size = new System.Drawing.Size(72, 31);
            this.txtTotalCredit.TabIndex = 3;
            // 
            // txtAvailableCredit
            // 
            this.txtAvailableCredit.Location = new System.Drawing.Point(256, 56);
            this.txtAvailableCredit.Name = "txtAvailableCredit";
            this.txtAvailableCredit.ReadOnly = true;
            this.txtAvailableCredit.Size = new System.Drawing.Size(72, 31);
            this.txtAvailableCredit.TabIndex = 2;
            // 
            // txtCardPrinted
            // 
            this.txtCardPrinted.Location = new System.Drawing.Point(256, 104);
            this.txtCardPrinted.Name = "txtCardPrinted";
            this.txtCardPrinted.ReadOnly = true;
            this.txtCardPrinted.Size = new System.Drawing.Size(72, 31);
            this.txtCardPrinted.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtRFIssueNo);
            this.groupBox1.Controls.Add(this.lRFIssueNo);
            this.groupBox1.Location = new System.Drawing.Point(24, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(728, 232);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Combined Details";
            // 
            // txtRFIssueNo
            // 
            this.txtRFIssueNo.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtRFIssueNo.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtRFIssueNo.Location = new System.Drawing.Point(232, 136);
            this.txtRFIssueNo.Name = "txtRFIssueNo";
            this.txtRFIssueNo.ReadOnly = true;
            this.txtRFIssueNo.Size = new System.Drawing.Size(40, 21);
            this.txtRFIssueNo.TabIndex = 57;
            // 
            // lRFIssueNo
            // 
            this.lRFIssueNo.Location = new System.Drawing.Point(128, 136);
            this.lRFIssueNo.Name = "lRFIssueNo";
            this.lRFIssueNo.Size = new System.Drawing.Size(96, 16);
            this.lRFIssueNo.TabIndex = 58;
            this.lRFIssueNo.Text = "Card Issue No";
            this.lRFIssueNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tpEmployee
            // 
            this.tpEmployee.Location = new System.Drawing.Point(0, 33);
            this.tpEmployee.Name = "tpEmployee";
            this.tpEmployee.Selected = false;
            this.tpEmployee.Size = new System.Drawing.Size(784, 279);
            this.tpEmployee.TabIndex = 7;
            this.tpEmployee.Title = "Employee Information";
            // 
            // tpApplicationStatus
            // 
            this.tpApplicationStatus.Controls.Add(this.dgApplicationStatus);
            this.tpApplicationStatus.Location = new System.Drawing.Point(0, 33);
            this.tpApplicationStatus.Name = "tpApplicationStatus";
            this.tpApplicationStatus.Selected = false;
            this.tpApplicationStatus.Size = new System.Drawing.Size(784, 279);
            this.tpApplicationStatus.TabIndex = 8;
            this.tpApplicationStatus.Title = "Application Status";
            // 
            // dgApplicationStatus
            // 
            this.dgApplicationStatus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgApplicationStatus.Location = new System.Drawing.Point(16, 16);
            this.dgApplicationStatus.Name = "dgApplicationStatus";
            this.dgApplicationStatus.ReadOnly = true;
            this.dgApplicationStatus.RowHeadersWidth = 62;
            this.dgApplicationStatus.Size = new System.Drawing.Size(752, 240);
            this.dgApplicationStatus.TabIndex = 8;
            // 
            // tpLetters
            // 
            this.tpLetters.Controls.Add(this.dgStatus);
            this.tpLetters.Controls.Add(this.dgLetters);
            this.tpLetters.Location = new System.Drawing.Point(0, 33);
            this.tpLetters.Name = "tpLetters";
            this.tpLetters.Selected = false;
            this.tpLetters.Size = new System.Drawing.Size(784, 279);
            this.tpLetters.TabIndex = 9;
            this.tpLetters.Title = "Letters";
            // 
            // dgStatus
            // 
            this.dgStatus.CaptionText = "Status History";
            this.dgStatus.DataMember = "";
            this.dgStatus.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgStatus.Location = new System.Drawing.Point(32, 19);
            this.dgStatus.Name = "dgStatus";
            this.dgStatus.ReadOnly = true;
            this.dgStatus.Size = new System.Drawing.Size(204, 240);
            this.dgStatus.TabIndex = 9;
            this.dgStatus.TabStop = false;
            // 
            // dgLetters
            // 
            this.dgLetters.CaptionText = "Letters History";
            this.dgLetters.DataMember = "";
            this.dgLetters.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgLetters.Location = new System.Drawing.Point(264, 16);
            this.dgLetters.Name = "dgLetters";
            this.dgLetters.ReadOnly = true;
            this.dgLetters.Size = new System.Drawing.Size(488, 240);
            this.dgLetters.TabIndex = 8;
            this.dgLetters.TabStop = false;
            // 
            // tpAllocation
            // 
            this.tpAllocation.Controls.Add(this.gbAllocationHist);
            this.tpAllocation.Location = new System.Drawing.Point(0, 33);
            this.tpAllocation.Name = "tpAllocation";
            this.tpAllocation.Selected = false;
            this.tpAllocation.Size = new System.Drawing.Size(784, 279);
            this.tpAllocation.TabIndex = 10;
            this.tpAllocation.Title = "Allocations";
            // 
            // gbAllocationHist
            // 
            this.gbAllocationHist.Controls.Add(this.dgAllocationHist);
            this.gbAllocationHist.Location = new System.Drawing.Point(8, 8);
            this.gbAllocationHist.Name = "gbAllocationHist";
            this.gbAllocationHist.Size = new System.Drawing.Size(760, 256);
            this.gbAllocationHist.TabIndex = 0;
            this.gbAllocationHist.TabStop = false;
            this.gbAllocationHist.Text = "Allocation History";
            // 
            // dgAllocationHist
            // 
            this.dgAllocationHist.DataMember = "";
            this.dgAllocationHist.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAllocationHist.Location = new System.Drawing.Point(8, 16);
            this.dgAllocationHist.Name = "dgAllocationHist";
            this.dgAllocationHist.ReadOnly = true;
            this.dgAllocationHist.Size = new System.Drawing.Size(744, 232);
            this.dgAllocationHist.TabIndex = 0;
            // 
            // tpActions
            // 
            this.tpActions.Controls.Add(this.gbAllocatedTo);
            this.tpActions.Controls.Add(this.pAllocation);
            this.tpActions.Controls.Add(this.label64);
            this.tpActions.Controls.Add(this.textNotes);
            this.tpActions.Controls.Add(this.dgActions);
            this.tpActions.Location = new System.Drawing.Point(0, 33);
            this.tpActions.Name = "tpActions";
            this.tpActions.Selected = false;
            this.tpActions.Size = new System.Drawing.Size(784, 279);
            this.tpActions.TabIndex = 10;
            this.tpActions.Title = "Follow Up";
            // 
            // gbAllocatedTo
            // 
            this.gbAllocatedTo.Controls.Add(this.txtEmployeeType);
            this.gbAllocatedTo.Controls.Add(this.txtEmployeeName);
            this.gbAllocatedTo.Controls.Add(this.lEmpName);
            this.gbAllocatedTo.Controls.Add(this.txtEmployeeNo);
            this.gbAllocatedTo.Controls.Add(this.lEmployeeNo);
            this.gbAllocatedTo.Controls.Add(this.lEmpType);
            this.gbAllocatedTo.Location = new System.Drawing.Point(16, 8);
            this.gbAllocatedTo.Name = "gbAllocatedTo";
            this.gbAllocatedTo.Size = new System.Drawing.Size(544, 48);
            this.gbAllocatedTo.TabIndex = 84;
            this.gbAllocatedTo.TabStop = false;
            this.gbAllocatedTo.Text = "Currently Allocated To Employee..";
            // 
            // txtEmployeeType
            // 
            this.txtEmployeeType.BackColor = System.Drawing.SystemColors.Window;
            this.txtEmployeeType.Location = new System.Drawing.Point(416, 16);
            this.txtEmployeeType.MaxLength = 30;
            this.txtEmployeeType.Name = "txtEmployeeType";
            this.txtEmployeeType.ReadOnly = true;
            this.txtEmployeeType.Size = new System.Drawing.Size(112, 31);
            this.txtEmployeeType.TabIndex = 1;
            this.txtEmployeeType.TabStop = false;
            // 
            // txtEmployeeName
            // 
            this.txtEmployeeName.BackColor = System.Drawing.SystemColors.Window;
            this.txtEmployeeName.Location = new System.Drawing.Point(168, 16);
            this.txtEmployeeName.MaxLength = 80;
            this.txtEmployeeName.Name = "txtEmployeeName";
            this.txtEmployeeName.ReadOnly = true;
            this.txtEmployeeName.Size = new System.Drawing.Size(200, 31);
            this.txtEmployeeName.TabIndex = 6;
            this.txtEmployeeName.TabStop = false;
            // 
            // lEmpName
            // 
            this.lEmpName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lEmpName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lEmpName.Location = new System.Drawing.Point(120, 16);
            this.lEmpName.Name = "lEmpName";
            this.lEmpName.Size = new System.Drawing.Size(40, 16);
            this.lEmpName.TabIndex = 5;
            this.lEmpName.Text = "Name";
            this.lEmpName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtEmployeeNo
            // 
            this.txtEmployeeNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtEmployeeNo.Location = new System.Drawing.Point(32, 16);
            this.txtEmployeeNo.MaxLength = 10;
            this.txtEmployeeNo.Name = "txtEmployeeNo";
            this.txtEmployeeNo.ReadOnly = true;
            this.txtEmployeeNo.Size = new System.Drawing.Size(80, 31);
            this.txtEmployeeNo.TabIndex = 2;
            this.txtEmployeeNo.TabStop = false;
            // 
            // lEmployeeNo
            // 
            this.lEmployeeNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lEmployeeNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lEmployeeNo.Location = new System.Drawing.Point(8, 16);
            this.lEmployeeNo.Name = "lEmployeeNo";
            this.lEmployeeNo.Size = new System.Drawing.Size(24, 16);
            this.lEmployeeNo.TabIndex = 3;
            this.lEmployeeNo.Text = "No";
            this.lEmployeeNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lEmpType
            // 
            this.lEmpType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lEmpType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lEmpType.Location = new System.Drawing.Point(376, 16);
            this.lEmpType.Name = "lEmpType";
            this.lEmpType.Size = new System.Drawing.Size(32, 16);
            this.lEmpType.TabIndex = 4;
            this.lEmpType.Text = "Type";
            this.lEmpType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pAllocation
            // 
            this.pAllocation.Controls.Add(this.drpReason);
            this.pAllocation.Controls.Add(this.btnSPADetails);
            this.pAllocation.Controls.Add(this.btnSave);
            this.pAllocation.Controls.Add(this.dtDueDate);
            this.pAllocation.Controls.Add(this.lbActionDate);
            this.pAllocation.Controls.Add(this.txtActionValue);
            this.pAllocation.Controls.Add(this.label65);
            this.pAllocation.Controls.Add(this.label67);
            this.pAllocation.Controls.Add(this.lbActionValue);
            this.pAllocation.Controls.Add(this.txtNotes);
            this.pAllocation.Controls.Add(this.drpActionCode);
            this.pAllocation.Controls.Add(this.lbReason);
            this.pAllocation.Enabled = false;
            this.pAllocation.Location = new System.Drawing.Point(568, 16);
            this.pAllocation.Name = "pAllocation";
            this.pAllocation.Size = new System.Drawing.Size(209, 256);
            this.pAllocation.TabIndex = 83;
            // 
            // drpReason
            // 
            this.drpReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpReason.Location = new System.Drawing.Point(81, 96);
            this.drpReason.Name = "drpReason";
            this.drpReason.Size = new System.Drawing.Size(112, 33);
            this.drpReason.TabIndex = 83;
            this.drpReason.Visible = false;
            // 
            // btnSPADetails
            // 
            this.btnSPADetails.Location = new System.Drawing.Point(80, 136);
            this.btnSPADetails.Name = "btnSPADetails";
            this.btnSPADetails.Size = new System.Drawing.Size(80, 24);
            this.btnSPADetails.TabIndex = 80;
            this.btnSPADetails.Text = "SPA History";
            this.btnSPADetails.Click += new System.EventHandler(this.btnSPADetails_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Location = new System.Drawing.Point(168, 136);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 78;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dtDueDate
            // 
            this.dtDueDate.CustomFormat = "ddd dd MMM yyyy";
            this.dtDueDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDueDate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtDueDate.Location = new System.Drawing.Point(81, 48);
            this.dtDueDate.Name = "dtDueDate";
            this.dtDueDate.Size = new System.Drawing.Size(112, 31);
            this.dtDueDate.TabIndex = 74;
            this.dtDueDate.Value = new System.DateTime(2005, 9, 5, 0, 0, 0, 0);
            // 
            // lbActionDate
            // 
            this.lbActionDate.Location = new System.Drawing.Point(3, 53);
            this.lbActionDate.Name = "lbActionDate";
            this.lbActionDate.Size = new System.Drawing.Size(72, 16);
            this.lbActionDate.TabIndex = 75;
            this.lbActionDate.Text = "Due Date";
            this.lbActionDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtActionValue
            // 
            this.txtActionValue.Location = new System.Drawing.Point(81, 72);
            this.txtActionValue.Name = "txtActionValue";
            this.txtActionValue.Size = new System.Drawing.Size(112, 31);
            this.txtActionValue.TabIndex = 79;
            this.txtActionValue.Leave += new System.EventHandler(this.txtActionValue_Leave);
            // 
            // label65
            // 
            this.label65.Location = new System.Drawing.Point(8, 8);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(48, 16);
            this.label65.TabIndex = 76;
            this.label65.Text = "Code";
            this.label65.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label67
            // 
            this.label67.Location = new System.Drawing.Point(8, 152);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(40, 16);
            this.label67.TabIndex = 82;
            this.label67.Text = "Notes";
            this.label67.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbActionValue
            // 
            this.lbActionValue.Location = new System.Drawing.Point(2, 74);
            this.lbActionValue.Name = "lbActionValue";
            this.lbActionValue.Size = new System.Drawing.Size(76, 21);
            this.lbActionValue.TabIndex = 80;
            this.lbActionValue.Text = "Action Value";
            this.lbActionValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(8, 168);
            this.txtNotes.MaxLength = 700;
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(184, 80);
            this.txtNotes.TabIndex = 81;
            // 
            // drpActionCode
            // 
            this.drpActionCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpActionCode.ItemHeight = 25;
            this.drpActionCode.Items.AddRange(new object[] {
            "C",
            "R"});
            this.drpActionCode.Location = new System.Drawing.Point(9, 24);
            this.drpActionCode.Name = "drpActionCode";
            this.drpActionCode.Size = new System.Drawing.Size(184, 33);
            this.drpActionCode.TabIndex = 77;
            this.drpActionCode.SelectionChangeCommitted += new System.EventHandler(this.drpActionCode_SelectionChangeCommitted);
            // 
            // lbReason
            // 
            this.lbReason.Location = new System.Drawing.Point(16, 96);
            this.lbReason.Name = "lbReason";
            this.lbReason.Size = new System.Drawing.Size(56, 16);
            this.lbReason.TabIndex = 75;
            this.lbReason.Text = "Reason";
            this.lbReason.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbReason.Visible = false;
            // 
            // label64
            // 
            this.label64.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label64.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label64.Location = new System.Drawing.Point(16, 168);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(40, 16);
            this.label64.TabIndex = 73;
            this.label64.Text = "Notes:";
            this.label64.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // textNotes
            // 
            this.textNotes.Location = new System.Drawing.Point(16, 184);
            this.textNotes.MaxLength = 700;
            this.textNotes.Multiline = true;
            this.textNotes.Name = "textNotes";
            this.textNotes.ReadOnly = true;
            this.textNotes.Size = new System.Drawing.Size(544, 80);
            this.textNotes.TabIndex = 72;
            // 
            // dgActions
            // 
            this.dgActions.CaptionText = "Follow Up Actions";
            this.dgActions.DataMember = "";
            this.dgActions.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgActions.Location = new System.Drawing.Point(16, 64);
            this.dgActions.Name = "dgActions";
            this.dgActions.ReadOnly = true;
            this.dgActions.Size = new System.Drawing.Size(544, 104);
            this.dgActions.TabIndex = 7;
            this.dgActions.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgActions_MouseUp);
            // 
            // tpAgreementChanges
            // 
            this.tpAgreementChanges.Controls.Add(this.dgAgreementAudit);
            this.tpAgreementChanges.Controls.Add(this.dgInstalPlan);
            this.tpAgreementChanges.Controls.Add(this.dgLineItem);
            this.tpAgreementChanges.Location = new System.Drawing.Point(0, 33);
            this.tpAgreementChanges.Name = "tpAgreementChanges";
            this.tpAgreementChanges.Selected = false;
            this.tpAgreementChanges.Size = new System.Drawing.Size(784, 279);
            this.tpAgreementChanges.TabIndex = 11;
            this.tpAgreementChanges.Title = "Agreement Changes";
            // 
            // dgAgreementAudit
            // 
            this.dgAgreementAudit.CaptionFont = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
            this.dgAgreementAudit.CaptionText = "Agreement Audit";
            this.dgAgreementAudit.DataMember = "";
            this.dgAgreementAudit.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAgreementAudit.Location = new System.Drawing.Point(16, 90);
            this.dgAgreementAudit.Name = "dgAgreementAudit";
            this.dgAgreementAudit.ReadOnly = true;
            this.dgAgreementAudit.Size = new System.Drawing.Size(728, 96);
            this.dgAgreementAudit.TabIndex = 4;
            // 
            // dgInstalPlan
            // 
            this.dgInstalPlan.CaptionFont = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
            this.dgInstalPlan.CaptionText = "Instal Plan Audit";
            this.dgInstalPlan.DataMember = "";
            this.dgInstalPlan.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgInstalPlan.Location = new System.Drawing.Point(16, 182);
            this.dgInstalPlan.Name = "dgInstalPlan";
            this.dgInstalPlan.ReadOnly = true;
            this.dgInstalPlan.Size = new System.Drawing.Size(728, 80);
            this.dgInstalPlan.TabIndex = 3;
            // 
            // dgLineItem
            // 
            this.dgLineItem.CaptionFont = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
            this.dgLineItem.CaptionText = "Line Item Audit";
            this.dgLineItem.DataMember = "";
            this.dgLineItem.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgLineItem.Location = new System.Drawing.Point(16, 0);
            this.dgLineItem.Name = "dgLineItem";
            this.dgLineItem.ReadOnly = true;
            this.dgLineItem.Size = new System.Drawing.Size(728, 96);
            this.dgLineItem.TabIndex = 1;
            // 
            // tpActivitySegments
            // 
            this.tpActivitySegments.Controls.Add(this.dgArrangements);
            this.tpActivitySegments.Controls.Add(this.dgActivities);
            this.tpActivitySegments.Controls.Add(this.dgSegment);
            this.tpActivitySegments.Location = new System.Drawing.Point(0, 33);
            this.tpActivitySegments.Name = "tpActivitySegments";
            this.tpActivitySegments.Selected = false;
            this.tpActivitySegments.Size = new System.Drawing.Size(784, 279);
            this.tpActivitySegments.TabIndex = 12;
            this.tpActivitySegments.Title = "Arrears Activities";
            // 
            // dgArrangements
            // 
            this.dgArrangements.CaptionText = "Arrangements";
            this.dgArrangements.DataMember = "";
            this.dgArrangements.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgArrangements.Location = new System.Drawing.Point(344, 16);
            this.dgArrangements.Name = "dgArrangements";
            this.dgArrangements.ReadOnly = true;
            this.dgArrangements.Size = new System.Drawing.Size(424, 248);
            this.dgArrangements.TabIndex = 2;
            // 
            // dgActivities
            // 
            this.dgActivities.CaptionText = "Activities";
            this.dgActivities.DataMember = "";
            this.dgActivities.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgActivities.Location = new System.Drawing.Point(24, 120);
            this.dgActivities.Name = "dgActivities";
            this.dgActivities.ReadOnly = true;
            this.dgActivities.Size = new System.Drawing.Size(312, 144);
            this.dgActivities.TabIndex = 1;
            // 
            // dgSegment
            // 
            this.dgSegment.CaptionText = "Segments";
            this.dgSegment.DataMember = "";
            this.dgSegment.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgSegment.Location = new System.Drawing.Point(24, 16);
            this.dgSegment.Name = "dgSegment";
            this.dgSegment.ReadOnly = true;
            this.dgSegment.Size = new System.Drawing.Size(312, 88);
            this.dgSegment.TabIndex = 0;
            // 
            // tpPendingCharges
            // 
            this.tpPendingCharges.Controls.Add(this.dgPendingCharges);
            this.tpPendingCharges.Controls.Add(this.dgArrearsCharges);
            this.tpPendingCharges.Location = new System.Drawing.Point(0, 33);
            this.tpPendingCharges.Name = "tpPendingCharges";
            this.tpPendingCharges.Selected = false;
            this.tpPendingCharges.Size = new System.Drawing.Size(784, 279);
            this.tpPendingCharges.TabIndex = 13;
            this.tpPendingCharges.Title = "Pending Charges";
            // 
            // dgPendingCharges
            // 
            this.dgPendingCharges.CaptionText = "Pending Charges";
            this.dgPendingCharges.DataMember = "";
            this.dgPendingCharges.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgPendingCharges.Location = new System.Drawing.Point(8, 8);
            this.dgPendingCharges.Name = "dgPendingCharges";
            this.dgPendingCharges.ReadOnly = true;
            this.dgPendingCharges.Size = new System.Drawing.Size(760, 128);
            this.dgPendingCharges.TabIndex = 0;
            // 
            // dgArrearsCharges
            // 
            this.dgArrearsCharges.CaptionText = "Arrears Changes";
            this.dgArrearsCharges.DataMember = "";
            this.dgArrearsCharges.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgArrearsCharges.Location = new System.Drawing.Point(8, 144);
            this.dgArrearsCharges.Name = "dgArrearsCharges";
            this.dgArrearsCharges.ReadOnly = true;
            this.dgArrearsCharges.Size = new System.Drawing.Size(760, 128);
            this.dgArrearsCharges.TabIndex = 0;
            // 
            // tpServiceRequest
            // 
            this.tpServiceRequest.BackColor = System.Drawing.SystemColors.Control;
            this.tpServiceRequest.Controls.Add(this.dgServiceRequestSummary);
            this.tpServiceRequest.Location = new System.Drawing.Point(0, 33);
            this.tpServiceRequest.Name = "tpServiceRequest";
            this.tpServiceRequest.Selected = false;
            this.tpServiceRequest.Size = new System.Drawing.Size(784, 279);
            this.tpServiceRequest.TabIndex = 14;
            this.tpServiceRequest.Title = "Service Summary";
            // 
            // dgServiceRequestSummary
            // 
            this.dgServiceRequestSummary.CaptionText = "Service Request Summary";
            this.dgServiceRequestSummary.DataMember = "";
            this.dgServiceRequestSummary.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgServiceRequestSummary.Location = new System.Drawing.Point(24, 13);
            this.dgServiceRequestSummary.Name = "dgServiceRequestSummary";
            this.dgServiceRequestSummary.ReadOnly = true;
            this.dgServiceRequestSummary.Size = new System.Drawing.Size(727, 248);
            this.dgServiceRequestSummary.TabIndex = 8;
            this.dgServiceRequestSummary.TabStop = false;
            // 
            // tpStrategies
            // 
            this.tpStrategies.Controls.Add(this.dgStrategies);
            this.tpStrategies.Controls.Add(this.dgWorklists);
            this.tpStrategies.Location = new System.Drawing.Point(0, 33);
            this.tpStrategies.Name = "tpStrategies";
            this.tpStrategies.Selected = false;
            this.tpStrategies.Size = new System.Drawing.Size(784, 279);
            this.tpStrategies.TabIndex = 15;
            this.tpStrategies.Title = "Strategies";
            // 
            // dgStrategies
            // 
            this.dgStrategies.CaptionText = "Strategies";
            this.dgStrategies.DataMember = "";
            this.dgStrategies.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgStrategies.Location = new System.Drawing.Point(8, 7);
            this.dgStrategies.Name = "dgStrategies";
            this.dgStrategies.ReadOnly = true;
            this.dgStrategies.Size = new System.Drawing.Size(760, 128);
            this.dgStrategies.TabIndex = 2;
            // 
            // dgWorklists
            // 
            this.dgWorklists.CaptionText = "Worklists";
            this.dgWorklists.DataMember = "";
            this.dgWorklists.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgWorklists.Location = new System.Drawing.Point(8, 143);
            this.dgWorklists.Name = "dgWorklists";
            this.dgWorklists.ReadOnly = true;
            this.dgWorklists.Size = new System.Drawing.Size(760, 128);
            this.dgWorklists.TabIndex = 1;
            // 
            // tpInstallation
            // 
            this.tpInstallation.Controls.Add(this.dgvInstallation);
            this.tpInstallation.Location = new System.Drawing.Point(0, 33);
            this.tpInstallation.Name = "tpInstallation";
            this.tpInstallation.Selected = false;
            this.tpInstallation.Size = new System.Drawing.Size(784, 279);
            this.tpInstallation.TabIndex = 16;
            this.tpInstallation.Title = "Installation";
            // 
            // dgvInstallation
            // 
            this.dgvInstallation.AllowUserToAddRows = false;
            this.dgvInstallation.AllowUserToDeleteRows = false;
            this.dgvInstallation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInstallation.Location = new System.Drawing.Point(8, 7);
            this.dgvInstallation.MultiSelect = false;
            this.dgvInstallation.Name = "dgvInstallation";
            this.dgvInstallation.ReadOnly = true;
            this.dgvInstallation.RowHeadersWidth = 62;
            this.dgvInstallation.Size = new System.Drawing.Size(760, 265);
            this.dgvInstallation.TabIndex = 137;
            // 
            // gbCustomer
            // 
            this.gbCustomer.BackColor = System.Drawing.SystemColors.Control;
            this.gbCustomer.Controls.Add(this.button1);
            this.gbCustomer.Controls.Add(this.txtTotalSpend);
            this.gbCustomer.Controls.Add(this.labelCredit);
            this.gbCustomer.Controls.Add(this.lPrivilegeClubDesc);
            this.gbCustomer.Controls.Add(this.chxPrivilegeClub);
            this.gbCustomer.Controls.Add(this.btnRefresh);
            this.gbCustomer.Controls.Add(this.btnCustCodes);
            this.gbCustomer.Controls.Add(this.label17);
            this.gbCustomer.Controls.Add(this.txtLastName);
            this.gbCustomer.Controls.Add(this.label1);
            this.gbCustomer.Controls.Add(this.txtFirstName);
            this.gbCustomer.Controls.Add(this.txtAccountNo);
            this.gbCustomer.Controls.Add(this.txtCustID);
            this.gbCustomer.Controls.Add(this.label2);
            this.gbCustomer.Controls.Add(this.label3);
            this.gbCustomer.Controls.Add(this.btnToggle);
            this.gbCustomer.Controls.Add(this.tcAddress);
            this.gbCustomer.Controls.Add(this.dgAccounts);
            this.gbCustomer.Location = new System.Drawing.Point(3, 0);
            this.gbCustomer.Name = "gbCustomer";
            this.gbCustomer.Size = new System.Drawing.Size(784, 165);
            this.gbCustomer.TabIndex = 2;
            this.gbCustomer.TabStop = false;
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(182, 114);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 23);
            this.button1.TabIndex = 55;
            this.button1.Text = "Show Photograph";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtTotalSpend
            // 
            this.txtTotalSpend.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtTotalSpend.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtTotalSpend.Location = new System.Drawing.Point(160, 88);
            this.txtTotalSpend.MaxLength = 25;
            this.txtTotalSpend.Name = "txtTotalSpend";
            this.txtTotalSpend.ReadOnly = true;
            this.txtTotalSpend.Size = new System.Drawing.Size(144, 26);
            this.txtTotalSpend.TabIndex = 53;
            // 
            // labelCredit
            // 
            this.labelCredit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelCredit.Location = new System.Drawing.Point(16, 88);
            this.labelCredit.Name = "labelCredit";
            this.labelCredit.Size = new System.Drawing.Size(160, 16);
            this.labelCredit.TabIndex = 54;
            this.labelCredit.Text = "Credit Limit";
            this.labelCredit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lPrivilegeClubDesc
            // 
            this.lPrivilegeClubDesc.Location = new System.Drawing.Point(48, 112);
            this.lPrivilegeClubDesc.Name = "lPrivilegeClubDesc";
            this.lPrivilegeClubDesc.Size = new System.Drawing.Size(240, 16);
            this.lPrivilegeClubDesc.TabIndex = 52;
            this.lPrivilegeClubDesc.Text = "Privilege Club";
            this.lPrivilegeClubDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chxPrivilegeClub
            // 
            this.chxPrivilegeClub.Enabled = false;
            this.chxPrivilegeClub.Location = new System.Drawing.Point(24, 112);
            this.chxPrivilegeClub.Name = "chxPrivilegeClub";
            this.chxPrivilegeClub.Size = new System.Drawing.Size(56, 16);
            this.chxPrivilegeClub.TabIndex = 51;
            // 
            // btnRefresh
            // 
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnRefresh.ImageIndex = 4;
            this.btnRefresh.ImageList = this.imageList1;
            this.btnRefresh.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRefresh.Location = new System.Drawing.Point(305, 136);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(24, 24);
            this.btnRefresh.TabIndex = 50;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnCustCodes
            // 
            this.btnCustCodes.Enabled = false;
            this.btnCustCodes.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCustCodes.Location = new System.Drawing.Point(16, 136);
            this.btnCustCodes.Name = "btnCustCodes";
            this.btnCustCodes.Size = new System.Drawing.Size(160, 24);
            this.btnCustCodes.TabIndex = 24;
            this.btnCustCodes.Text = "Add Account/Customer Code";
            this.btnCustCodes.Visible = false;
            this.btnCustCodes.Click += new System.EventHandler(this.btnCustCodes_Click);
            // 
            // label17
            // 
            this.label17.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label17.Location = new System.Drawing.Point(120, 48);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(80, 16);
            this.label17.TabIndex = 28;
            this.label17.Text = "Last Name:";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtLastName
            // 
            this.txtLastName.Location = new System.Drawing.Point(120, 64);
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.ReadOnly = true;
            this.txtLastName.Size = new System.Drawing.Size(184, 26);
            this.txtLastName.TabIndex = 27;
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(16, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 26;
            this.label1.Text = "First Name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtFirstName
            // 
            this.txtFirstName.Location = new System.Drawing.Point(16, 64);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.ReadOnly = true;
            this.txtFirstName.Size = new System.Drawing.Size(100, 26);
            this.txtFirstName.TabIndex = 25;
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtAccountNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtAccountNo.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtAccountNo.Location = new System.Drawing.Point(200, 24);
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.ReadOnly = true;
            this.txtAccountNo.Size = new System.Drawing.Size(104, 26);
            this.txtAccountNo.TabIndex = 3;
            this.txtAccountNo.Text = "000-0000-0000-0";
            // 
            // txtCustID
            // 
            this.txtCustID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtCustID.Location = new System.Drawing.Point(16, 24);
            this.txtCustID.Name = "txtCustID";
            this.txtCustID.ReadOnly = true;
            this.txtCustID.Size = new System.Drawing.Size(136, 26);
            this.txtCustID.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(200, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Account Number:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(16, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Customer ID:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnToggle
            // 
            this.btnToggle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnToggle.Location = new System.Drawing.Point(182, 136);
            this.btnToggle.Name = "btnToggle";
            this.btnToggle.Size = new System.Drawing.Size(117, 24);
            this.btnToggle.TabIndex = 23;
            this.btnToggle.Tag = "Addresses";
            this.btnToggle.Text = "Show Accounts";
            this.btnToggle.Click += new System.EventHandler(this.btnOtherAccounts_Click_1);
            // 
            // tcAddress
            // 
            this.tcAddress.IDEPixelArea = true;
            this.tcAddress.Location = new System.Drawing.Point(344, 8);
            this.tcAddress.Name = "tcAddress";
            this.tcAddress.PositionTop = true;
            this.tcAddress.ShrinkPagesToFit = false;
            this.tcAddress.Size = new System.Drawing.Size(424, 185);
            this.tcAddress.TabIndex = 30;
            // 
            // dgAccounts
            // 
            this.dgAccounts.CaptionText = "Other Accounts";
            this.dgAccounts.DataMember = "";
            this.dgAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAccounts.Location = new System.Drawing.Point(344, 8);
            this.dgAccounts.Name = "dgAccounts";
            this.dgAccounts.ReadOnly = true;
            this.dgAccounts.Size = new System.Drawing.Size(424, 152);
            this.dgAccounts.TabIndex = 29;
            this.dgAccounts.Click += new System.EventHandler(this.dgAccounts_Click_1);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataMember = "";
            this.errorProvider1.Icon = ((System.Drawing.Icon)(resources.GetObject("errorProvider1.Icon")));
            // 
            // AccountDetails
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 480);
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.gbCustomer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "AccountDetails";
            this.Text = "Account Details";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AccountDetails_FormClosing);
            this.Load += new System.EventHandler(this.AccountDetails_Load);
            this.Enter += new System.EventHandler(this.AccountDetails_Enter);
            this.tpAgreement.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Loyalty_HClogo_pb)).EndInit();
            this.tcMain.ResumeLayout(false);
            this.tpLineItems.ResumeLayout(false);
            this.gbLineItems.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgLineItems)).EndInit();
            this.tpDeliveries.ResumeLayout(false);
            this.gbDelivery.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgDelivery)).EndInit();
            this.tpAmortizationSchedule.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAmortizationSchedule)).EndInit();
            this.tpRFCombined.ResumeLayout(false);
            this.tpRFCombined.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tpApplicationStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgApplicationStatus)).EndInit();
            this.tpLetters.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgLetters)).EndInit();
            this.tpAllocation.ResumeLayout(false);
            this.gbAllocationHist.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAllocationHist)).EndInit();
            this.tpActions.ResumeLayout(false);
            this.tpActions.PerformLayout();
            this.gbAllocatedTo.ResumeLayout(false);
            this.gbAllocatedTo.PerformLayout();
            this.pAllocation.ResumeLayout(false);
            this.pAllocation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgActions)).EndInit();
            this.tpAgreementChanges.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAgreementAudit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgInstalPlan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgLineItem)).EndInit();
            this.tpActivitySegments.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgArrangements)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgActivities)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgSegment)).EndInit();
            this.tpPendingCharges.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgPendingCharges)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgArrearsCharges)).EndInit();
            this.tpServiceRequest.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgServiceRequestSummary)).EndInit();
            this.tpStrategies.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgStrategies)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgWorklists)).EndInit();
            this.tpInstallation.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInstallation)).EndInit();
            this.gbCustomer.ResumeLayout(false);
            this.gbCustomer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private string customerID = String.Empty;
        private string accountType = String.Empty;

        private void LoadData()
        {
            bool status = true;
            bool privilegeClub = false;
            string privilegeClubCode = String.Empty;
            string privilegeClubDesc = String.Empty;
            bool hasDiscount = false;
            string termsType = String.Empty;

            try
            {
                Function = "LoadDataThread";
                Wait();

                //Load customer details		
                //accountDetails = CustomerManager.GetCustomerAccountsAndDetails(accountNo, out Error);

                object[] cus = CustomerManager.GetCustomerAccountsDetailsList(accountNo, out Error);

                foreach (object a in cus)
                {
                    customer.Add(a);
                }

                if (Error.Length > 0)
                {
                    ShowError(Error);
                    status = false;
                }
                else
                {
                    //foreach (DataTable dt in accountDetails.Tables)
                    //    if (dt.TableName == TN.Customer)
                    //        foreach (DataRow r in dt.Rows)
                    //            customerID = (string)r[CN.CustomerID];

                    //IP 04/12/2007 - UAT(125)
                    customerID = customer.Count > 0 ? customer[0].ToString() : string.Empty;
                }

                /* retrieve the privilege club stuff */
                if ((bool)Country[CountryParameterNames.LoyaltyCard])
                {
                    CustomerManager.IsPrivilegeMember(customerID, out privilegeClub, out privilegeClubCode, out privilegeClubDesc, out hasDiscount, out Error);

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        chxPrivilegeClub.Checked = privilegeClub;
                        lPrivilegeClubDesc.Text = privilegeClubDesc;
                    }
                }

                if (status)
                {
                    custAddresses = CustomerManager.GetCustomerAddresses(customerID, out Error);
                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                        status = false;
                    }
                }

                if (status)
                {
                    if (customerID != "PAID & TAKEN")
                    {
                        accounts = CustomerManager.CustomerSearch(customerID, String.Empty, String.Empty, String.Empty, String.Empty, 100, 1, true,     //CR1084
                                                                    Config.StoreType, String.Empty, out Error);
                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                            status = false;
                        }
                    }
                }

                if (status)
                {
                    // Init to today in case we don't get the actual date acct opened
                    this._dateAcctOpen = StaticDataManager.GetServerDate();

                    agreement = AccountManager.GetAgreement(accountNo, AgreementNo, false, out Error);
                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                        status = false;
                    }
                    else
                    {
                        foreach (DataTable dt in agreement.Tables)
                        {
                            switch (dt.TableName)
                            {
                                case TN.AccountDetails:
                                    if (dt.Rows.Count > 0) //prevent error when no rows are brought back
                                    {
                                        if (DBNull.Value != dt.Rows[0]["Account Type"])
                                            accountType = (string)dt.Rows[0]["Account Type"];

                                        accountType.Trim();

                                        if (DBNull.Value != dt.Rows[0]["Account Opened"])
                                            this._dateAcctOpen = Convert.ToDateTime((DateTime)dt.Rows[0]["Account Opened"]);

                                        if (DBNull.Value != dt.Rows[0]["isLoan"])                                                        //IP - 20/10/11 - CR1232
                                            this.IsLoan = Convert.ToBoolean(dt.Rows[0]["isLoan"]);
                                    }
                                    break;
                                case TN.Agreements:
                                    if (dt.Rows.Count > 0)
                                    {
                                        if (DBNull.Value != dt.Rows[0][CN.AgrmtNo])
                                            AgreementNo = (int)dt.Rows[0][CN.AgrmtNo];

                                        termsType = (string)dt.Rows[0]["Terms Type"];
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                if (status)
                {
                    DataSet tt = AccountManager.GetTermsTypeDetails(Config.CountryCode, termsType, accountNo, this._dateAcctOpen, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        this.txtBand.Text = tt.Tables[0].Rows[0][CN.Band].ToString();
                        this.txtRate.Text = Convert.ToDecimal(tt.Tables[0].Rows[0]["servpcent"]).ToString("F3");
                    }
                }

                // PN 30/10/2003
                /*if(status) 
                {
                    itemDoc = new XmlDocument();
                    itemDoc.LoadXml("<ITEMS></ITEMS>");

                    //retrieve line items for this account
                    XmlNode lineItems = AccountManager.GetLineItems(accountNo, AgreementNo, accountType, Config.CountryCode, out Error);
                    if(Error.Length>0)
                    {
                        ShowError(Error);
                        status = false;
                    }
                    else 
                    {
                        //initialise the XML document and the tree view
                        if(lineItems != null)
                        {
                            lineItems = itemDoc.ImportNode(lineItems, true);
                            itemDoc.ReplaceChild(lineItems, itemDoc.DocumentElement);
                        }
                    }
                }*/

                if (status)
                {
                    if (customerID != "PAID & TAKEN")
                    {
                        transactions = AccountManager.GetTransactions(accountNo, out Error);
                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                            status = false;
                        }
                    }
                }

                if (status)
                {
                    if (accountType == AT.ReadyFinance)
                    {
                        RFCombined = CustomerManager.GetRFCombinedDetails(customerID, out Error);
                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                            status = false;
                        }
                    }
                }

                if (status)
                {
                    deliveries = AccountManager.GetDeliveries(accountNo, AgreementNo, out Error);
                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                        status = false;
                    }
                }

                if (status)
                {
                    if (customerID != "PAID & TAKEN")
                    {
                        addTo = 0;
                        payments = PaymentManager.GetPaymentAccounts(customerID, Config.CountryCode, false, out addTo, out Error);
                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                            status = false;
                        }
                    }
                }

                if (status)
                {
                    XmlUtilities xml = new XmlUtilities();
                    XmlDocument dropDowns = new XmlDocument();
                    dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
                    StringCollection actions = new StringCollection();
                    actions.Add("Actions");
                    //DataSet strategyActionsForEmployee;
                    DataSet strategyActions;        //IP - 18/05/10 - UAT(4) UAT5.2 External log
                    LoadStrategiesandWorklists();


                    if (StaticData.Tables[TN.Actions] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Actions,
                            new string[] { "FUP", "L" }));

                    if (StaticData.Tables[TN.Reasons] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Reasons, new string[] { "SP2", "L" }));

                    if (StaticData.Tables[TN.Villages] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Villages, null));

                    if (dropDowns.DocumentElement.ChildNodes.Count > 0)
                    {
                        DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
                        if (Error.Length > 0)
                        {
                            status = false;
                            ShowError(Error);
                        }
                        else
                        {
                            foreach (DataTable dt in ds.Tables)
                                StaticData.Tables[dt.TableName] = dt;
                        }
                    }
                    /*
                    foreach (DataRow row in ((DataTable)StaticData.Tables[TN.Actions]).Rows)
                    {
                        string str = (string)row[0] + " : " + (string)row[1];
                        actions.Add(str.ToUpper());
                    }
                    drpActionCode.DataSource = actions;
                    */
                    drpReason.DataSource = (DataTable)StaticData.Tables[TN.Reasons];
                    drpReason.DisplayMember = CN.CodeDescription;
                    drpReason.ValueMember = CN.Code;


                    //IP - 18/05/10 - UAT(154) UAT5.2.1.0 Log - If Link To Tallyman is true then populate the actions drop down from the FUP category.
                    if ((bool)Country[CountryParameterNames.LinkToTallyman])
                    {

                        drpActionCode.DataSource = StaticData.Tables[TN.Actions];
                        drpActionCode.DisplayMember = CN.CodeDescript;
                        drpActionCode.ValueMember = CN.Code;
                    }
                    else
                    {
                        //NM & IP - 23/12/08 - CR976 - Retrieve the actions that the user has rights to perform
                        //based on the strategy that the account is in.

                        //IP - 26/08/09 - UAT(819) - Only populate the ActionCode drop down on the FollowUp tab, if the strategy
                        //that the account currently is in does not have worklists
                        //else disable the drop down. If the user does not have rights to any actions, then disable the drop down.
                        //if (strategy != "" && strategyHasWorklists == false)
                        if (strategy != "")  //IP - 18/05/10 - UAT(4) UAT5.2 External Log - Populate the actions drop down with the actions for a strategy. 
                        {
                            //strategyActionsForEmployee = CollectionsManager.GetStrategyActionsForEmployee(Credential.UserId, strategy, false, out Error);
                            strategyActions = CollectionsManager.GetStrategyActions(strategy, out Error);   //IP - 18/05/10 - UAT(4) UAT5.2 External log

                            //DataTable dtStrategyActions = strategyActionsForEmployee.Tables[0];
                            DataTable dtStrategyActions = strategyActions.Tables[0];    //IP - 18/05/10 - UAT(4) UAT5.2 External log

                            if (dtStrategyActions.Rows.Count > 0)
                            {

                                drpActionCode.DataSource = dtStrategyActions;
                                //drpActionCode.DisplayMember = CN.ActionDescription;
                                drpActionCode.DisplayMember = CN.Action; //IP - 18/05/10 - UAT(4) UAT5.2 External log
                                drpActionCode.ValueMember = CN.ActionCode;
                            }
                            else
                            {
                                drpActionCode.Enabled = false;
                            }

                        }
                        else
                        {
                            drpActionCode.Enabled = false;
                        }
                    }
                }

                if (status)
                {
                    isAccountCancelled = AccountManager.IsCancelled(accountNo, out Error);
                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                        status = false;
                    }
                }

                if (status)
                {
                    this.service = ServiceManager.GetServiceRequestSummaryForAccount(accountNo, out Error);
                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                        status = false;
                    }
                }

                if (status)
                {
                    if (Convert.ToBoolean(Country[CountryParameterNames.LoyaltyScheme]))
                    {
                        int places;
                        if (this.IsNumeric(((string)Country[CountryParameterNames.DecimalPlaces]).Substring(1, 1)))
                            places = System.Convert.ToInt32(((string)Country[CountryParameterNames.DecimalPlaces]).Substring(1, 1));
                        else
                            places = 2;

                        var Voucher = CustomerManager.LoyaltyGetVoucherValue(accountNo);
                        Vouchervalue = Voucher.HasValue ? Math.Round(Voucher.Value, places) : 0;
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
                Function = "End of LoadDataThread";
            }
        }

        private void AccountDetails_Load(object sender, System.EventArgs e)
        {
            try
            {
                Function = "AccountDetails_Load";

                Wait();

                menuExit.ImageList = ((MainForm)this.FormRoot).menuIcons;
                menuExit.ImageIndex = 3;

                var isSpecialAcct = AccountManager.IsSRInstSpecialAccount(accountNo, out Error);
                if (isSpecialAcct == null)
                {
                    ShowError(String.Format("A web service error has occured{0}{1}",
                                            Environment.NewLine,
                                            Error));
                    CloseTab(this);
                    return;
                }
                else if (isSpecialAcct.Value == true)
                {
                    ShowWarning(GetResource("M_CANNOTENTERSPECIAL"));
                    CloseTab(this);
                    return;
                }

                // Display Privilege / Loyalty club title
                lPrivilegeClubDesc.Text = GetResource((bool)Country[CountryParameterNames.TierPCEnabled] ? "T_LOYALTYCLUB" : "T_PRIVILEGECLUB");

                /* if this is the paid and taken account then prompt the user
                 * for the agreement number they want to look up */
                string PTAcctNo = AccountManager.GetPaidAndTakenAccount(Config.BranchCode, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                    PaidAndTaken = (accountNo == PTAcctNo);

                if (PaidAndTaken)
                {
                    GetAgreementNo ga = new GetAgreementNo(FormRoot, this);
                    ga.ShowDialog();
                    ga.Dispose();
                }

                if (Cancel)
                    CloseTab(this);
                else
                {
                    /* Issue 69238 - SC 3/9/07
                     * Get agreement number so cash and go accounts with agreement numbers greater than 1
                     * line items can be viewed. */

                    //if (!PaidAndTaken && (accountNo.Substring(3, 1) == "5"))
                    if (!PaidAndTaken)  //#18924
                    {
                        AgreementNo = AccountManager.GetAgreementNo(accountNo, out Error);
                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                        }
                    }

                    LoadData();

                    // Display the customer details
                    LoadCustomer(accountNo);

                    //CR855
                    // Determine if Show Photograph button should be enabled
                    EnablePhotographButton();

                    // Load addresses	
                    loadAddresses(readOnly);

                    // Load other accounts for this customer
                    LoadOtherAccounts();

                    // Display the agreement details
                    LoadAgreement(accountNo);

                    // PN 30/10/2003lb
                    // Display Line Items
                    //LoadLineItems(accountNo, txtAccountType.Text);

                    // Display Transactions
                    LoadTransactions(accountNo);

                    LoadRFCombined(txtCustID.Text, txtAccountType.Text);

                    // Display Delivery Details
                    LoadDeliveries(accountNo);

                    LoadRebate(accountNo);

                    LoadLetters();

                    //Load Service Summary
                    LoadServiceSummary();

                    LoadInstallationBooking();

                    CalcPercentagePaid();

                    //if ((bool)Country[CountryParameterNames.CL_Amortized] && isAmortized)

                    if (isAmortized)
                    {
                        LoadEarlySettlementFig(accountNo);
                        LoadAmortizationSchedule();
                    }
                    else
                    {
                        if (tcMain.TabPages.Contains(tpAmortizationSchedule))
                            tcMain.TabPages.Remove(tpAmortizationSchedule);
                    }

                    if (isAccountCancelled)
                        ResetFields();

                    if (((bool)Country[CountryParameterNames.DDEnabled]))
                        LoadGiro();

                    //If we haven't navigated here with a request for pre-selection
                    //of a specific tab, select the first tab
                    //Note: Only works if you 'switch' from another tab to it
                    if (_showTabName.Length == 0)
                    {
                        tcMain.SelectedTab = tpLetters;
                        tcMain.SelectedTab = tpAgreement;
                    }

                    // Show fields for terms type band if this is enabled
                    bool showBand = ((txtAccountType.Text == "R" || txtAccountType.Text == "H") && Convert.ToBoolean(Country[CountryParameterNames.TermsTypeBandEnabled]));
                    this.lBand.Visible = showBand;
                    this.txtBand.Visible = showBand;
                    this.lRate.Visible = showBand;
                    this.txtRate.Visible = showBand;

                    //IP - 19/04/10 - UAT(55) UAT5.2 - Merged from 4.3
                    if (Convert.ToBoolean(Country[CountryParameterNames.LoyaltyScheme]))
                    {
                        STL.PL.WS3.Loyalty loyalty = CustomerManager.LoyaltyGetDatabycustid(txtCustID.Text.Trim());

                        //if (loyalty.memberno != null)
                        if (loyalty.memberno != null && loyalty.statusacct == 1) //IP - 27/05/10 - UAT(182)UAT5.2.1.0 Log - Check that the membership status is 'Current'
                        {
                            Loyalty_HClogo_pb.Visible = true;
                        }
                        else
                        {
                            Loyalty_HClogo_pb.Visible = false;
                        }
                    }

                    txt_vouchervalue.Text = Vouchervalue.ToString();


                    //IP - 20/10/11 - CR1232 - Only display the option to print the agreement documents if the account has been fully delivered.
                    //if (Convert.ToDecimal(StripCurrency(txtAgreementTotal.Text)) ==  Convert.ToDecimal(StripCurrency(txtDeliveryTotal.Text)))
                    if (txtDeliveryTotal.Text != string.Empty && (Convert.ToDecimal(StripCurrency(txtAgreementTotal.Text)) == Convert.ToDecimal(StripCurrency(txtDeliveryTotal.Text)))) //IP - 16/02/12 - check txtDeliveryTotal has a value
                    {
                        menuAgreementDocs.Visible = true;
                    }
                    else
                    {
                        menuAgreementDocs.Visible = false;
                    }

                    ProcessMambuDetails();
                }


            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of AccountDetails_Load";
            }
        }

        private void EnablePhotographButton()
        {
            try
            {
                if (txtCustID.Text != String.Empty)
                {
                    fileName = CustomerManager.GetCustomerPhoto(txtCustID.Text.Trim(), out Error);

                    string signatureFileName = String.Empty;
                    signatureFileName = CustomerManager.GetCustomerSignature(txtCustID.Text.Trim(), out Error);

                    if (fileName != String.Empty || signatureFileName != String.Empty)
                    {
                        button1.Enabled = true;
                    }
                    else
                    {
                        button1.Enabled = false;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private void LoadServiceSummary()
        {
            dgServiceRequestSummary.DataSource = service;
            dgServiceRequestSummary.DataMember = TN.ServiceRequest;
            dgServiceRequestSummary.TableStyles.Clear();

            DataGridTableStyle tabStyle = new DataGridTableStyle();
            tabStyle.MappingName = TN.ServiceRequest;
            dgServiceRequestSummary.TableStyles.Add(tabStyle);

            //tabStyle.GridColumnStyles[CN.ServiceRequestNo].Width = 0;
            ReportUtils.ApplyGridHeadings(tabStyle.GridColumnStyles, this);

            tpServiceRequest.BackColor = tcMain.BackColor;
            if (service != null)
            {
                if (service.Tables[TN.ServiceRequest].Rows.Count > 0)
                {
                    foreach (DataRow dr in service.Tables[TN.ServiceRequest].Rows)
                    {
                        //UAT 337 the FS states that the backgound of the tab should be 'pale red' if there is an open SR. This is the closest I can get.
                        if (dr[CN.DateClosed].ToString().Equals(string.Empty))
                        {
                            tpServiceRequest.BackColor = Color.LightPink;// Color.FromArgb(255, 192, 128);
                            break;
                        }
                        if (Convert.ToDateTime(dr[CN.DateClosed]).Equals(new DateTime(1900, 1, 1)))
                        {
                            tpServiceRequest.BackColor = Color.LightPink; //Color.FromArgb(255, 0, 0);
                            break;
                        }

                    }
                }
            }
            tabStyle.Dispose();
        }

        //private void dgServiceRequestSummary_MouseUp(object sender, MouseEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Button == MouseButtons.Right)
        //        {
        //            DataGrid ctl = (DataGrid)sender;

        //            MenuCommand m1 = new MenuCommand(GetResource("P_SERVICE_REQUEST"));

        //            m1.Click += new EventHandler(menuServiceRequest_Click);

        //            PopupMenu popup = new PopupMenu();
        //            popup.Animate = Animate.Yes;
        //            popup.AnimateStyle = Animation.SlideHorVerPositive;

        //            popup.MenuCommands.AddRange(new MenuCommand[] { m1 });
        //            MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, "dgServiceRequestSummary_MouseUp");
        //    }
        //}

        //private void menuServiceRequest_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Function = "menuServiceRequest";
        //        int index = dgServiceRequestSummary.CurrentRowIndex;

        //        if (index >= 0)
        //        {
        //            string serviceNo = dgServiceRequestSummary[index, 1].ToString();
        //            if (serviceNo.Length != 0)
        //            {
        //                SR_ServiceRequest p = new SR_ServiceRequest(this.FormRoot, this, serviceNo, string.Empty, string.Empty);
        //                ((MainForm)this.FormRoot).AddTabPage(p);
        //            }
        //            else
        //                return;
        //            //ShowInfo("M_NOACCOUNT");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, Function);
        //    }
        //}

        private void LoadLineItems(string accountNo, string accountType)
        {
            itemDoc = new XmlDocument();
            itemDoc.LoadXml("<ITEMS></ITEMS>");

            //retrieve line items for this account
            XmlNode lineItems = AccountManager.GetLineItems(accountNo, AgreementNo, accountType, Config.CountryCode, Convert.ToInt16(Config.BranchCode), out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                //initialise the XML document and the tree view
                if (lineItems != null)
                {
                    lineItems = itemDoc.ImportNode(lineItems, true);
                    itemDoc.ReplaceChild(lineItems, itemDoc.DocumentElement);
                }
            }

            if (itemDoc.DocumentElement.HasChildNodes)
            {
                //Account has LineItems so add the LineItem tab.
                if (!tcMain.TabPages.Contains(tpLineItems))
                {
                    tcMain.TabPages.Insert(1, tpLineItems);
                }
                //Create the tabPage and populate it's datagrid in here
                populateTable();
            }
            else
            {
                //Account does not have LineItems so remove LineItem tab.
                if (tcMain.TabPages.Contains(tpLineItems))
                {
                    tcMain.TabPages.Remove(tpLineItems);
                }
            }
        }

        private void populateTable()
        {
            /* First thing to do is create a line items tab i.e. and 
             * instance of the LineItemsTab class, and add it to the tab
             * control
             */

            //Set up the datagrid columns
            if (itemsTable == null)
            {
                //Create the table to hold the Line items
                itemsTable = new DataTable("Items");
                itemsView = new DataView(itemsTable);
                // uat363 rdb added parentproductcode to key
                //DataColumn[] key = new DataColumn[3];
                DataColumn[] key = new DataColumn[4];

                //itemsTable.Columns.Add(new DataColumn("ProductCode"));
                //itemsTable.Columns.Add(new DataColumn("ProductDescription"));
                //itemsTable.Columns.Add(new DataColumn("StockLocation"));
                //itemsTable.Columns.Add(new DataColumn("QuantityOrdered"));
                //itemsTable.Columns.Add(new DataColumn("UnitPrice"));
                //itemsTable.Columns.Add(new DataColumn("Value"));
                //itemsTable.Columns.Add(new DataColumn("DeliveredQuantity"));
                //itemsTable.Columns.Add(new DataColumn("DateDelivered"));
                //itemsTable.Columns.Add(new DataColumn("DatePlanDel"));
                //itemsTable.Columns.Add(new DataColumn("DateReqDel"));
                //itemsTable.Columns.Add(new DataColumn("TimeReqDel"));
                //itemsTable.Columns.Add(new DataColumn("DelNoteBranch"));
                //itemsTable.Columns.Add(new DataColumn("Notes"));
                //itemsTable.Columns.Add(new DataColumn("ContractNo"));
                //itemsTable.Columns.Add(new DataColumn("DeliveryProcess"));
                //itemsTable.Columns.Add(new DataColumn("Damaged"));
                //itemsTable.Columns.Add(new DataColumn("Assembly"));
                //itemsTable.Columns.Add(new DataColumn("ParentProductCode"));

                //IP - 03/02/10 - CR1072 - 3.1.1 - Rearrange columns to place important information first.

                itemsTable.Columns.Add(new DataColumn("ProductCode"));
                itemsTable.Columns.Add(new DataColumn("ProductDescription"));
                itemsTable.Columns.Add(new DataColumn("StockLocation"));
                itemsTable.Columns.Add(new DataColumn(CN.DeliveryAddress)); //IP - 17/02/10 - CR1072 - UAT(160) - Delivery Fixes from 4.3
                itemsTable.Columns.Add(new DataColumn("QuantityOrdered"));
                itemsTable.Columns.Add(new DataColumn("UnitPrice"));
                itemsTable.Columns.Add(new DataColumn("Value"));
                itemsTable.Columns.Add(new DataColumn("DeliveredQuantity"));
                itemsTable.Columns.Add(new DataColumn("DateDelivered"));
                itemsTable.Columns.Add(new DataColumn("DelNoteBranch"));
                itemsTable.Columns.Add(new DataColumn("DatePlanDel"));
                itemsTable.Columns.Add(new DataColumn("DateReqDel"));
                itemsTable.Columns.Add(new DataColumn("TimeReqDel"));
                itemsTable.Columns.Add(new DataColumn("Damaged"));
                itemsTable.Columns.Add(new DataColumn("Assembly"));
                itemsTable.Columns.Add(new DataColumn("ExpressDelivery")); //IP - 12/06/12 - #10345 - Warehouse & Deliveries
                itemsTable.Columns.Add(new DataColumn("Notes"));
                itemsTable.Columns.Add(new DataColumn("ContractNo"));
                itemsTable.Columns.Add(new DataColumn("DeliveryProcess"));
                itemsTable.Columns.Add(new DataColumn("ParentProductCode"));
                itemsTable.Columns.Add(new DataColumn("DHLInterfaceDate"));
                itemsTable.Columns.Add(new DataColumn("DHLPickingDate"));
                itemsTable.Columns.Add(new DataColumn("DHLDNNo"));
                itemsTable.Columns.Add(new DataColumn(CN.ItemId));
                itemsTable.Columns.Add(new DataColumn(CN.ParentItemId));

                key[0] = itemsTable.Columns[CN.ItemId];
                key[1] = itemsTable.Columns["StockLocation"];
                key[2] = itemsTable.Columns["ContractNo"];
                key[3] = itemsTable.Columns[CN.ParentItemId];
                itemsTable.PrimaryKey = key;
            }

            //NB - when refering to the datagrid remember that it is a member
            //of the LineItemsTab object lt. So is the TreeView tv
            if (dgLineItems.TableStyles.Count == 0)
            {
                dgLineItems.DataSource = itemsView;
                DataGridTableStyle tabStyle = new DataGridTableStyle();
                tabStyle.MappingName = itemsTable.TableName;
                dgLineItems.TableStyles.Add(tabStyle);

                tabStyle.GridColumnStyles["ProductCode"].Width = 90;
                tabStyle.GridColumnStyles["ProductCode"].HeaderText = GetResource("T_ITEMNO");
                tabStyle.GridColumnStyles["ProductCode"].Alignment = HorizontalAlignment.Left;
                tabStyle.GridColumnStyles["ProductDescription"].Width = 200;
                tabStyle.GridColumnStyles["ProductDescription"].HeaderText = GetResource("T_DESCRIPTION");
                tabStyle.GridColumnStyles["StockLocation"].Width = 75;
                tabStyle.GridColumnStyles["StockLocation"].HeaderText = GetResource("T_STOCKLOCN");
                tabStyle.GridColumnStyles["StockLocation"].Alignment = HorizontalAlignment.Center;
                tabStyle.GridColumnStyles[CN.DeliveryAddress].Width = 60; //IP - 17/02/10 - CR1072 - UAT(160) - Delivery Fixes from 4.3
                tabStyle.GridColumnStyles[CN.DeliveryAddress].HeaderText = GetResource("T_DELADDRESS"); //IP - 17/02/10 - CR1072 - UAT(160) - Delivery Fixes from 4.3
                tabStyle.GridColumnStyles["QuantityOrdered"].Width = 40;
                tabStyle.GridColumnStyles["QuantityOrdered"].HeaderText = GetResource("T_QUANTITY");
                tabStyle.GridColumnStyles["QuantityOrdered"].Alignment = HorizontalAlignment.Right;
                tabStyle.GridColumnStyles["UnitPrice"].Width = 70;
                tabStyle.GridColumnStyles["UnitPrice"].HeaderText = GetResource("T_UNITPRICE");
                tabStyle.GridColumnStyles["UnitPrice"].Alignment = HorizontalAlignment.Right;
                tabStyle.GridColumnStyles["Value"].Width = 70;
                tabStyle.GridColumnStyles["Value"].HeaderText = GetResource("T_VALUE");
                tabStyle.GridColumnStyles["Value"].Alignment = HorizontalAlignment.Right;
                ((DataGridTextBoxColumn)tabStyle.GridColumnStyles["Value"]).Format = DecimalPlaces;
                tabStyle.GridColumnStyles["DeliveredQuantity"].Width = 70;
                tabStyle.GridColumnStyles["DeliveredQuantity"].HeaderText = GetResource("T_DELIVEREDQTY");
                tabStyle.GridColumnStyles["DeliveredQuantity"].Alignment = HorizontalAlignment.Right;
                tabStyle.GridColumnStyles["DateDelivered"].Width = 70;
                tabStyle.GridColumnStyles["DateDelivered"].HeaderText = GetResource("T_DELIVERYDATE");
                tabStyle.GridColumnStyles["DateDelivered"].Alignment = HorizontalAlignment.Left;
                tabStyle.GridColumnStyles["DatePlanDel"].Width = 130;           // jec
                tabStyle.GridColumnStyles["DatePlanDel"].HeaderText = GetResource("T_DATEDELPLAN");
                tabStyle.GridColumnStyles["DatePlanDel"].Alignment = HorizontalAlignment.Left;
                tabStyle.GridColumnStyles["DateReqDel"].Width = 105;            // jec
                tabStyle.GridColumnStyles["DateReqDel"].HeaderText = GetResource("T_REQDELDATE");
                tabStyle.GridColumnStyles["DateReqDel"].Alignment = HorizontalAlignment.Center;
                tabStyle.GridColumnStyles["TimeReqDel"].Width = 110;            // jec
                tabStyle.GridColumnStyles["TimeReqDel"].HeaderText = GetResource("T_REQDELTIME");
                tabStyle.GridColumnStyles["TimeReqDel"].Alignment = HorizontalAlignment.Center;
                tabStyle.GridColumnStyles["DelNoteBranch"].Width = 100;
                tabStyle.GridColumnStyles[CN.DelNoteBranch].HeaderText = GetResource("T_DELIVERYLOCATION");     // #10401
                tabStyle.GridColumnStyles["DelNoteBranch"].Alignment = HorizontalAlignment.Center;
                tabStyle.GridColumnStyles["Notes"].Width = 150;
                tabStyle.GridColumnStyles["Notes"].HeaderText = GetResource("T_DELIVERYINSTRUCTIONS");
                tabStyle.GridColumnStyles["Notes"].Alignment = HorizontalAlignment.Left;
                tabStyle.GridColumnStyles["ContractNo"].Width = 80;         // jec
                tabStyle.GridColumnStyles["ContractNo"].HeaderText = GetResource("T_CONTRACTNO");
                tabStyle.GridColumnStyles["ContractNo"].Alignment = HorizontalAlignment.Left;
                tabStyle.GridColumnStyles["DeliveryProcess"].Width = 90;    // jec
                tabStyle.GridColumnStyles["DeliveryProcess"].HeaderText = GetResource("T_DELIVERYPROCESS");
                tabStyle.GridColumnStyles["DeliveryProcess"].Alignment = HorizontalAlignment.Left;
                tabStyle.GridColumnStyles["Damaged"].Width = 60;
                tabStyle.GridColumnStyles["Damaged"].HeaderText = GetResource("T_DAMAGESTOCK");
                tabStyle.GridColumnStyles["Damaged"].Alignment = HorizontalAlignment.Left;
                tabStyle.GridColumnStyles["Assembly"].Width = 60;
                tabStyle.GridColumnStyles["Assembly"].HeaderText = GetResource("T_ASSEMBLY");
                tabStyle.GridColumnStyles["Assembly"].Alignment = HorizontalAlignment.Left;
                tabStyle.GridColumnStyles["ExpressDelivery"].Width = 60;                                                    //IP - 12/06/12 - #10345 - Warehouse & Deliveries
                tabStyle.GridColumnStyles["ExpressDelivery"].HeaderText = GetResource("T_EXPRESS");                         //IP - 12/06/12 - #10345 - Warehouse & Deliveries
                tabStyle.GridColumnStyles["ExpressDelivery"].Alignment = HorizontalAlignment.Left;                          //IP - 12/06/12 - #10345 - Warehouse & Deliveries
                tabStyle.GridColumnStyles["DHLInterfaceDate"].Width = 150;
                tabStyle.GridColumnStyles["DHLInterfaceDate"].HeaderText = GetResource("T_DHLInterfaceDate");
                tabStyle.GridColumnStyles["DHLInterfaceDate"].Alignment = HorizontalAlignment.Left;
                tabStyle.GridColumnStyles["DHLPickingDate"].Width = 150;
                tabStyle.GridColumnStyles["DHLPickingDate"].HeaderText = GetResource("T_DHLPickingDate");
                tabStyle.GridColumnStyles["DHLPickingDate"].Alignment = HorizontalAlignment.Left;
                tabStyle.GridColumnStyles["DHLDNNo"].Width = 60;
                tabStyle.GridColumnStyles["DHLDNNo"].HeaderText = GetResource("T_DHLDNNo");
                tabStyle.GridColumnStyles["DHLDNNo"].Alignment = HorizontalAlignment.Left;
                tabStyle.GridColumnStyles["ParentProductCode"].HeaderText = GetResource("T_PARENT_ITEM");       // RI
                tabStyle.GridColumnStyles["ParentProductCode"].Width = 90;
                tabStyle.GridColumnStyles[CN.ItemId].Width = 0;         //RI  jec 26/05/11
                tabStyle.GridColumnStyles[CN.ParentItemId].Width = 0;         //RI  jec 26/05/11
                //tabStyle.GridColumnStyles[CN.RetItemId].Width = 0;         //RI  jec 26/05/11

                if (!thirdPartyDeliveriesEnabled)
                {
                    // Do not show columns
                    tabStyle.GridColumnStyles["DHLDNNo"].Width = 0;
                    tabStyle.GridColumnStyles["DHLPickingDate"].Width = 0;
                    tabStyle.GridColumnStyles["DHLInterfaceDate"].Width = 0;
                }

                tabStyle.Dispose();
            }

            itemsTable.Clear();
            tvItems.Nodes.Clear();
            tvItems.Nodes.Add(new TreeNode("Account"));

            double subTotal = 0;
            populateTable(itemDoc.DocumentElement, tvItems.Nodes[0], ref subTotal);

            tvItems.Nodes[0].Expand();
        }

        /// <summary>
        /// This function will call itself recursively until all 
        /// item nodes in the XML document have been entered into 
        /// the table.
        /// For each child node of the node passed in (which will
        /// always be an <item></item> node) create a row for that item
        /// and, if it's related items node has children, call this function
        /// again passing in that node.
        /// Also build up the tree view in the same format as the document
        /// </summary>
        /// <param name="relatedItems"></param>
        private void populateTable(XmlNode relatedItems, TreeNode tvNode, ref double sub)
        {
            Function = "populateTable";
            string itemType = String.Empty;
            double qty = 0;
            double delQty = 0;

            //outer loop iterates through <item> tags
            foreach (XmlNode item in relatedItems.ChildNodes)
            {
                string delType = String.Empty;
                if (item.NodeType == XmlNodeType.Element)
                {
                    TreeNode tvChild = new TreeNode();
                    // uat363 add parentItemCode to Tag
                    int parentItemId = 0;
                    if (item.ParentNode.ParentNode.Name == "Item")
                        parentItemId = Convert.ToInt32(item.ParentNode.ParentNode.Attributes[Tags.ItemId].Value);
                    tvChild.Tag = item.Attributes[Tags.Key].Value + "|" + parentItemId;
                    DataRow row = itemsTable.NewRow();
                    bool showRow = true;

                    itemType = item.Attributes[Tags.Type].Value;
                    qty = Convert.ToDouble(item.Attributes[Tags.Quantity].Value);
                    delQty = Convert.ToDouble(item.Attributes[Tags.DeliveredQuantity].Value);

                    //tvChild.Text = itemType;
                    tvChild.Text = item.Attributes[Tags.FreeGift].Value.ToUpper().Trim() == "TRUE" ? "FreeGift" : itemType; //IP - 21/04/10 - UAT(53) UAT5.2 - Merged from 4.3
                    tvChild.ImageIndex = 1;
                    tvChild.SelectedImageIndex = 1;

                    if (itemType == IT.Stock || itemType == IT.Component)
                    {
                        tvChild.ImageIndex = 0;
                        tvChild.SelectedImageIndex = 0;
                    }
                    //if (itemType == IT.Discount || itemType == IT.KitDiscount)
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
                    // 573 need to show if either quantity >0 or delivered >0
                    if (itemType == IT.Kit || (qty <= 0 && delQty <= 0))
                    {
                        showRow = false;
                    }

                    row["ProductCode"] = item.Attributes[Tags.Code].Value;
                    // UAT 218 - Show the second description line on this screen as well
                    // but only if it is different to the first line.
                    string desc1 = item.Attributes[Tags.Description1].Value.Trim();
                    string desc2 = item.Attributes[Tags.Description2].Value.Trim();
                    if (desc1 != desc2 && desc2 != String.Empty)
                        row["ProductDescription"] = desc1 + " - " + desc2;
                    else
                        row["ProductDescription"] = desc1;
                    row["StockLocation"] = item.Attributes[Tags.Location].Value;
                    row["QuantityOrdered"] = item.Attributes[Tags.Quantity].Value;
                    //69065 - Unit Price to be shown to the Country Parameter decimal places setting
                    row["UnitPrice"] = Convert.ToDecimal(item.Attributes[Tags.UnitPrice].Value).ToString(DecimalPlaces);
                    row["DeliveredQuantity"] = item.Attributes[Tags.DeliveredQuantity].Value;
                    row["DateDelivered"] = item.Attributes[Tags.DateDelivered].Value;
                    row["DatePlanDel"] = Convert.ToDateTime(item.Attributes[Tags.PlannedDeliveryDate].Value).ToString("dd/MM/yyy");
                    row["DateReqDel"] = Convert.ToDateTime(item.Attributes[Tags.DeliveryDate].Value).ToString("dd/MM/yyy");
                    row["TimeReqDel"] = item.Attributes[Tags.DeliveryTime].Value;
                    row["DelNoteBranch"] = item.Attributes[Tags.BranchForDeliveryNote].Value;
                    row["Notes"] = item.Attributes[Tags.ColourTrim].Value;
                    row["ContractNo"] = item.Attributes[Tags.ContractNumber].Value;
                    row[CN.DeliveryAddress] = item.Attributes[Tags.DeliveryAddress].Value; //IP - 17/02/10 - CR1072 - UAT(160) - Delivery Fixes from 4.3

                    if (item.Attributes[Tags.DeliveryProcess].Value == "S")
                        delType = GetResource("T_SCHEDULING");
                    else if (item.Attributes[Tags.DeliveryProcess].Value == "I")
                        delType = GetResource("T_IMMEDIATE");

                    row["DeliveryProcess"] = delType;
                    row["Damaged"] = item.Attributes[Tags.Damaged].Value;
                    row["Assembly"] = item.Attributes[Tags.Assembly].Value;
                    row["ExpressDelivery"] = item.Attributes[Tags.Express].Value == string.Empty ? "N" : item.Attributes[Tags.Express].Value;                   //IP - 12/06/12 - #10345 - Warehouse & Deliveries

                    string parentProductCode = string.Empty;
                    if (item.ParentNode.ParentNode.Name == "Item")
                        parentProductCode = item.ParentNode.ParentNode.Attributes[Tags.Code].Value;
                    row["ParentProductCode"] = parentProductCode;
                    row["DhlInterfaceDate"] = item.Attributes[Tags.DhlInterfaceDate].Value;
                    row["DhlPickingDate"] = item.Attributes[Tags.DhlPickingDate].Value;
                    row["DhlDNNo"] = item.Attributes[Tags.DhlDNNo].Value;
                    row[CN.ItemId] = item.Attributes[Tags.ItemId].Value;
                    row[CN.ParentItemId] = item.Attributes[Tags.ParentItemId].Value;

                    if (showRow)
                    {
                        //69065 - Value to be shown to the Country Parameter decimal places setting
                        row["Value"] = Convert.ToDecimal(item.Attributes[Tags.Value].Value).ToString(DecimalPlaces);
                        sub += Convert.ToDouble(StripCurrency(item.Attributes[Tags.Value].Value));
                    }

                    foreach (XmlNode child in item.ChildNodes)
                        if (child.NodeType == XmlNodeType.Element && child.Name == Elements.RelatedItem)
                        {
                            if (child.HasChildNodes)
                                populateTable(child, tvChild, ref sub);
                        }

                    //70267 Show line items that have a delivered quantity (not a quantity) > 0 SL's recommendation
                    if (delQty > 0 || qty > 0) //IP - 27/05/09 - 69962
                        tvNode.Nodes.Add(tvChild);

                    if (showRow)
                        itemsTable.Rows.Add(row);
                }
            }
            Function = "End of populateTable";
        }

        private void ProcessAccountDetails(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (DBNull.Value != row["Account Type"])
                    txtAccountType.Text = (string)row["Account Type"];

                txtAccountType.Text.Trim();

                if (DBNull.Value != row["Account Opened"])
                {
                    txtDateOpened.Text = ((DateTime)row["Account Opened"]).ToShortDateString();
                    //txtCreatedDate.Text = txtDateOpened.Text; IP - 04/02/10 - CR1072 - 3.1.9 - Field now moved to EmployeeInformation.cs
                }

                if (DBNull.Value != row["Date Last Paid"])
                    txtDateLastPaid.Text = ((DateTime)row["Date Last Paid"]).ToShortDateString();

                if (DBNull.Value != row["Outstanding Balance"])
                    txtOutstandingBalance.Text = ((decimal)row["Outstanding Balance"]).ToString(DecimalPlaces);
                else
                    txtOutstandingBalance.Text = (0).ToString(DecimalPlaces);

                if (DBNull.Value != row["Arrears"])
                    txtArrears.Text = ((decimal)row["Arrears"]).ToString(DecimalPlaces);
                else
                    txtArrears.Text = (0).ToString(DecimalPlaces);

                if (txtArrears.Text.ToDecimal() > 0)  // Display in red if > 0
                    txtArrears.ForeColor = Color.Red;
                else
                    txtArrears.ForeColor = SystemColors.ControlText;

                if (DBNull.Value != row["Current Status"])
                    txtCurrentStatus.Text = (string)row["Current Status"];

                lArchive.Visible = Convert.ToInt32(row[CN.Archived]) > 0;

                // cr906 Malaysia Changes
                if (DBNull.Value != row["Segment_Name"])
                    txtSegmentName.Text = Convert.ToString(row["Segment_Name"]);

                if (DBNull.Value != row["isAmortized"])
                    isAmortized = Convert.ToBoolean(row["isAmortized"]);
            }
        }

        private void ProcessAgreements(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (DBNull.Value != row["Percentage Paid"])
                    txtPercentagePaid.Text = ((short)row["Percentage Paid"]).ToString();
                else
                    txtPercentagePaid.Text = (0).ToString(DecimalPlaces);

                if (DBNull.Value != row["Terms Type"])
                    txtTTCode.Text = (string)row["Terms Type"];

                if (DBNull.Value != row["Description"])
                    txtTTDescription.Text = (string)row["Description"];

                if (DBNull.Value != row["Date Delivered"])
                    txtDateDelivered.Text = ((DateTime)row["Date Delivered"]).ToShortDateString();

                if (DBNull.Value != row["Deposit"])
                    txtDeposit.Text = ((decimal)row["Deposit"]).ToString(DecimalPlaces);
                else
                    txtDeposit.Text = (0).ToString(DecimalPlaces);

                if (DBNull.Value != row["Instalment Amount"])
                    txtIAmount.Text = ((decimal)row["Instalment Amount"]).ToString(DecimalPlaces);
                else
                    txtIAmount.Text = (0).ToString(DecimalPlaces);

                if (DBNull.Value != row["Day Due"])
                    txtDateDue.Text = (string)row["Day Due"];
                else
                    txtDateDue.Text = "0";

                if (DBNull.Value != row["Service Charge"])
                    txtSChargeAmount.Text = ((decimal)row["Service Charge"]).ToString(DecimalPlaces);
                else
                    txtSChargeAmount.Text = (0).ToString(DecimalPlaces);
                // Display COD for Cash account only and if not Y display as N.

                if (accountType != AT.Cash)
                    txtCODFlag.BackColor = SystemColors.Control;
                else if (DBNull.Value != row["COD Flag"])
                    txtCODFlag.Text = "Y" == (string)row["COD Flag"] ? "Y" : "N";

                if (DBNull.Value != row["Date First Instalment"])
                    txtDateFInstalment.Text = ((DateTime)row["Date First Instalment"]).ToShortDateString();

                if (DBNull.Value != row["instalno"])
                    txtInstalments.Text = ((short)row["instalno"]).ToString();
                else
                    txtInstalments.Text = (0).ToString(DecimalPlaces);

                if (DBNull.Value != row["Date Last Instalment"])
                    txtDateLInstalment.Text = ((DateTime)row["Date Last Instalment"]).ToShortDateString();

                if (DBNull.Value != row["Final Instalment"])
                    txtLastInstalmentAmount.Text = ((decimal)row["Final Instalment"]).ToString(DecimalPlaces);
                else
                    txtLastInstalmentAmount.Text = (0).ToString(DecimalPlaces);

                if (DBNull.Value != row["Instalment Total"])
                    txtInstalmentTotal.Text = ((decimal)row["Instalment Total"]).ToString(DecimalPlaces);
                else
                    txtInstalmentTotal.Text = (0).ToString(DecimalPlaces);

                if (DBNull.Value != row["Months Interest Free"])
                    txtMthsInterestFree.Text = ((short)row["Months Interest Free"]).ToString();
                else
                    txtMthsInterestFree.Text = (0).ToString(DecimalPlaces);

                if (DBNull.Value != row["Agreement Total"])
                    txtAgreementTotal.Text = ((decimal)row["Agreement Total"]).ToString(DecimalPlaces);
                else
                    txtAgreementTotal.Text = (0).ToString(DecimalPlaces);

                //IP - 04/02/10 - CR1072 - 3.1.9 - Employee Information controls now populated on EmployeeInformation.cs

                //if (DBNull.Value != row["DAed By EmpeeNo"])
                //    txtDAEmpeeNo.Text = (row["DAed By EmpeeNo"]).ToString();
                //if (DBNull.Value != row["DAed By Employee Name"])
                //    txtDAbyEmpeeName.Text = (string)row["DAed By Employee Name"];
                //if (DBNull.Value != row["Date DAed On"])
                //    txtDAedOn.Text = ((DateTime)row["Date DAed On"]).ToShortDateString();
                //if (DBNull.Value != row["Last Changed By"])
                //    txtLstChgEmpeeNo.Text = (row["Last Changed By"]).ToString();
                //if (DBNull.Value != row["LstChanged By Employee Name"])
                //    txtLstChgEmpeeName.Text = (string)row["LstChanged By Employee Name"];
                //if (DBNull.Value != row["Date Last Changed"])
                //    txtLastChangedOn.Text = ((DateTime)row["Date Last Changed"]).ToShortDateString();
                //if (DBNull.Value != row["Sold By"])
                //    txtSoldByEmpeeNo.Text = (row["Sold By"]).ToString();
                //if (DBNull.Value != row["Sold By Employee Name"])
                //    txtSoldBYEmpeeName.Text = (string)row["Sold By Employee Name"];
                //if (DBNull.Value != row["Date Sold On"])
                //    txtSoldOn.Text = ((DateTime)row["Date Sold On"]).ToShortDateString();
                //if (row[CN.CreatedBy] != DBNull.Value)
                //    txtCreatedByNo.Text = Convert.ToString(row[CN.CreatedBy]);
                //if (row[CN.CreatedByName] != DBNull.Value)
                //    txtCreatedByName.Text = Convert.ToString(row[CN.CreatedByName]);

                // blank fields for Cash accounts   jec 22/11/07

                //if (txtAccountType.Text == AT.Cash)       
                if (txtTTCode.Text == "00")         // #15186 
                {
                    txtTTCode.Text = " ";
                    txtTTDescription.Text = "CASH";
                    txtDateFInstalment.Text = " ";
                    txtDateLInstalment.Text = " ";
                }

                //IP - 04/02/10 - CR1072 - Employee Information controls now populated on EmployeeInformation.cs

                //int empNoRev = Convert.ToInt32(row["Reopened By"]);
                //if (empNoRev > 0)
                //{
                //    this.txtReopenOn.BackColor = SystemColors.Window;
                //    this.txtReopenEmpeeNo.BackColor = SystemColors.Window;
                //    this.txtReopenEmpeeName.BackColor = SystemColors.Window;

                //    txtReopenEmpeeNo.Text = empNoRev.ToString();

                //    if (DBNull.Value != row["Date Reopened"])
                //        txtReopenOn.Text = ((DateTime)row["Date Reopened"]).ToShortDateString();
                //    if (row["Reopened By Employee Name"] != DBNull.Value)
                //        txtReopenEmpeeName.Text = Convert.ToString(row["Reopened By Employee Name"]);
                //}
                //else
                //{
                //    this.txtReopenOn.BackColor = SystemColors.Control;
                //    this.txtReopenEmpeeNo.BackColor = SystemColors.Control;
                //    this.txtReopenEmpeeName.BackColor = SystemColors.Control;

                //    txtReopenEmpeeNo.Text = "";
                //    txtReopenOn.Text = "";
                //    txtReopenEmpeeName.Text = "";
                //}

                //IP - 04/02/10 - CR1072 - 3.1.9 - Employee Information controls now populated from EmployeeInformation.cs
                DataRow dr = dt.Rows[0];
                ((EmployeeInformation)tpEmployee.Control).ProcessEmployeeSummary(dr);
                ((EmployeeInformation)tpEmployee.Control).LoadDAHistory(accountNo);
            }
        }

        private void ProcessFinTrans(DataTable dt)
        {
            //tpTransactions.Control = new Transactions();

            foreach (DataRow row in dt.Rows)
            {
                if (DBNull.Value != row["Date Last Movement"])
                    txtLMovement.Text = ((DateTime)row["Date Last Movement"]).ToShortDateString();
                if (DBNull.Value != row["Amount Paid"])
                {
                    //IP - 27/11/2007 - UAT133
                    //I am now returning the 'Warranty Adjustment' in the dataset.
                    //The warranty adjustment will now be added onto the amount paid.
                    //txtAmountPaid.Text = ((decimal)row["Amount Paid"]).ToString(DecimalPlaces);
                    decimal initialAmountPaid = Convert.ToDecimal(row["Amount Paid"]);
                    decimal warranty = Convert.ToDecimal(row["Warranty Adjustment"]);
                    decimal amountPaid = initialAmountPaid + warranty;
                    txtAmountPaid.Text = amountPaid.ToString(DecimalPlaces);
                }
                else
                    txtAmountPaid.Text = (0).ToString(DecimalPlaces);

                if (DBNull.Value != row["Delivery Total"])
                    txtDeliveryTotal.Text = ((decimal)row["Delivery Total"]).ToString(DecimalPlaces);
                else
                    txtDeliveryTotal.Text = (0).ToString(DecimalPlaces);

                if (DBNull.Value != row["To Follow"])
                    txtToFollowAmount.Text = ((decimal)row["To Follow"]).ToString(DecimalPlaces);
                else
                    txtToFollowAmount.Text = (0).ToString(DecimalPlaces);

                if ((decimal)row["To Follow"] > 0)  // Display in red if > 0
                    txtToFollowAmount.ForeColor = Color.Red;
                else
                    txtToFollowAmount.ForeColor = SystemColors.ControlText;

                if (DBNull.Value != row["Total Fees"])
                    txtTotalFees.Text = ((decimal)row["Total Fees"]).ToString(DecimalPlaces);
                else
                    txtTotalFees.Text = (0).ToString(DecimalPlaces);

                //tpTransactions.Control = new Transactions();
                if (DBNull.Value != row["Total Bail Fees"])
                    ((Transactions)tpTransactions.Control).txtTotalBailFees.Text = ((decimal)row["Total Bail Fees"]).ToString(DecimalPlaces);
                else
                    ((Transactions)tpTransactions.Control).txtTotalBailFees.Text = (0).ToString(DecimalPlaces);

                if (DBNull.Value != row["Rebate"])
                    txtRebate.Text = ((decimal)row["Rebate"]).ToString(DecimalPlaces);
                else
                    txtRebate.Text = (0).ToString(DecimalPlaces);

                if (DBNull.Value != row["Add-to Potential"])
                    txtAddToPotential.Text = ((decimal)row["Add-to Potential"]).ToString(DecimalPlaces);
                else
                    txtAddToPotential.Text = (0).ToString(DecimalPlaces);

                if (DBNull.Value != row["Early Settlement"])
                    txtEarlySettlementFigure.Text = ((decimal)row["Early Settlement"]).ToString(DecimalPlaces);
                else
                    txtEarlySettlementFigure.Text = (0).ToString(DecimalPlaces);
            }
        }

        private void LoadAgreement(string accountNumber)
        {
            Function = "LoadAgreement";
            try
            {
                Wait();
                /********** Table Definitions ***************************
                   Table[0] holds values from DN_AccountGetDetailsSP
                   Table[1] holds values from DN_AgreeementGetSP
                   Table[2] holds values from DN_FintransSumAccountGetSP
                *********************************************************/
                if (agreement != null)
                {
                    foreach (DataTable dt in agreement.Tables)
                    {
                        switch (dt.TableName)
                        {
                            case TN.AccountDetails:
                                ProcessAccountDetails(dt);
                                break;
                            case TN.Agreements:

                                ProcessAgreements(dt);
                                break;
                            case TN.FinTrans:
                                ProcessFinTrans(dt);
                                break;
                            default:
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
                Function = "End of LoadAgreement";
                StopWait();
            }
        }

        private void LoadCustomer(string accountNumber)
        {
            Function = "LoadCustomer";

            try
            {
                Wait();

                string formattedAcNo = accountNumber;
                CommonForm.FormatAccountNo(ref formattedAcNo);
                txtAccountNo.Text = formattedAcNo;

                //if (accountDetails != null)
                //{
                //    foreach (DataTable dt in accountDetails.Tables)
                //    {
                //        switch (dt.TableName)
                //        {
                //            case TN.Customer:
                //                if (dt.Rows.Count > 0)
                //                {
                //                    txtCustID.Text = (string)dt.Rows[0][CN.CustomerID];//do stuff
                //                    tmpName = (string)dt.Rows[0][CN.Title] +
                //                        " " + (string)dt.Rows[0][CN.FirstName] +
                //                        " " + (string)dt.Rows[0][CN.LastName];
                //                    txtFirstName.Text = (string)dt.Rows[0][CN.FirstName];
                //                    txtLastName.Text = (string)dt.Rows[0][CN.LastName];
                //                    this.Text += " - " + tmpName;
                //                    txtTotalSpend.Text = Convert.ToDecimal(dt.Rows[0][CN.RFCreditLimit]).ToString(DecimalPlaces);
                //                }
                //                break;
                //            default:
                //                break;
                //        }
                //    }
                //}

                //IP 04/12/2007 - UAT(125)- Changed from 'if (customer != null)'
                if (customer.Count > 0)
                {
                    txtCustID.Text = (string)customer[0];
                    tmpName = (string)customer[1] +
                        " " + (string)customer[2] +
                        " " + (string)customer[3];
                    txtFirstName.Text = (string)customer[2];
                    txtLastName.Text = (string)customer[3];
                    this.Text += " - " + tmpName;
                    txtTotalSpend.Text = Convert.ToDecimal(customer[4]).ToString(DecimalPlaces);
                }
                //IP 04/12/2007 - If the account searched on has no customer linked to it, set the 
                //txtCustid to empty.
                else
                {
                    txtCustID.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of LoadCustomer";
            }
        }

        private void dgAccounts_Click_1(object sender, System.EventArgs e)
        {
            Function = "dgAccounts_Click_1";
            string acctNo = String.Empty;

            try
            {
                int index = dgAccounts.CurrentRowIndex;

                if (index >= 0)
                {
                    acctNo = (string)dgAccounts[dgAccounts.CurrentRowIndex, 4];
                    if (acctNo != accountNo)
                    {
                        accountNo = acctNo;

                        tcAddress.TabPages.Clear();
                        AccountDetails_Load(null, null);

                        Audit = null;
                        if (tcMain.SelectedTab == tpAgreementChanges)
                            LoadAgreementChanges();
                    }
                }

                Function = "End of dgAccounts_Click";
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        protected void dgTransactionsNew_Click(object sender, MouseEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                //Get Fin TransId (ID) from grid
                int FinTransId;

                int index = ((Transactions)tpTransactions.Control).dgTransactionsNew.CurrentRowIndex;
                if (index >= 0)
                {
                    DataView PaymentList = (DataView)((Transactions)tpTransactions.Control).dgTransactionsNew.DataSource;
                    DataRow newPayment = PaymentList[index].Row;
                    FinTransId = int.Parse(newPayment["ID"].ToString());

                    ((Transactions)tpTransactions.Control).dgSummary.DataSource = null;
                    ((Transactions)tpTransactions.Control).dgDetails.DataSource = null;

                    DataSet ds = AccountManager.GetTransactionDetails(FinTransId);
                    if (ds.Tables.Count > 0)
                    {
                        LoadSummary(ds.Tables[0]);
                        LoadDetails(ds.Tables[1]);
                    }                                

                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                Catch(ex, Function);
            }
        }

        /// <summary>
        /// This method is called when a Transactions datatable has been 
        /// returned. It will create a transactions tabPage and
        /// populate the dataGrid with the records from the 
        /// Transactions datatable.
        /// </summary>
        /// <param name="dt"></param>
        private void LoadTransactions(DataTable dt)
        {
            //Format TransValue to 2 decimal places.
            FormatDecimalValues(ref dt, CN.TransValue);

           ((Transactions)tpTransactions.Control).dgTransactionsNew.MouseDoubleClick += new MouseEventHandler(dgTransactionsNew_Click);

            transView = new DataView(dt);
            if (transView.Count > 0)		//make sure we have some records
            {
                ((Transactions)tpTransactions.Control).dgTransactionsNew.DataSource = transView; //

                if (((Transactions)tpTransactions.Control).dgTransactionsNew.TableStyles.Count == 0)
                {
                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                    tabStyle.MappingName = TN.LineItemAudit;

                    AddColumnStyle(CN.acctno, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.TransPrinted, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.TransRefNo, tabStyle, 0, true, GetResource("T_REFNO"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.DateTrans, tabStyle, 110, true, GetResource("T_DATE"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.TransTypeCode, tabStyle, 100, true, GetResource("T_TYPE"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.EmployeeNo, tabStyle, 90, true, GetResource("T_EMPEENO"), "", HorizontalAlignment.Left);       // RI
                    AddColumnStyle(CN.TransValue, tabStyle, 60, true, GetResource("T_VALUE"), "", HorizontalAlignment.Left); //IP - 05/02/10 - CR1072 - 3.1.12 - Renamed column to Stock Location from Location
                    AddColumnStyle(CN.FootNotes, tabStyle, 60, true, GetResource("T_NOTES"), "", HorizontalAlignment.Left); //IP - 05/02/10 - CR1072 - 3.1.12 - Added Del Note Branch
                    AddColumnStyle(CN.EmployeeName, tabStyle, 60, true, GetResource("T_EMPEENAME"), String.Empty, HorizontalAlignment.Right);
                    AddColumnStyle(CN.PayMethod, tabStyle, 60, true, GetResource("T_PAYMETHOD"), String.Empty, HorizontalAlignment.Right);
                    AddColumnStyle(CN.Reference, tabStyle, 80, true, GetResource("T_GIFTREFERENCE"), DecimalPlaces, HorizontalAlignment.Right);
                    AddColumnStyle(CN.ChequeNo, tabStyle, 80, true, GetResource("T_CHEQUENOFIN"), DecimalPlaces, HorizontalAlignment.Right);
                    AddColumnStyle(CN.TransferReference, tabStyle, 80, true, GetResource("T_TRANSFERREF"), DecimalPlaces, HorizontalAlignment.Right);  // 67977 RD 22/02/06
                    AddColumnStyle(CN.PaymentAmount, tabStyle, 80, true, GetResource("T_PAYMENTAMOUNT"), DecimalPlaces, HorizontalAlignment.Right); // 67977 RD 22/02/06
                    AddColumnStyle(CN.Success, tabStyle, 0, true, GetResource("T_SUCCESS"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.Deposit, tabStyle, 0, true, GetResource("T_DEPOSITR"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.ID, tabStyle, 0, true, GetResource("T_ID"), "", HorizontalAlignment.Left);

                    ((Transactions)tpTransactions.Control).dgTransactionsNew.TableStyles.Add(tabStyle); //IP - 18/11/09 - CR929 & 974 - Audit

                    tabStyle.Dispose();
                }                
            }
        }

        private void LoadSummary(DataTable dt)
        {
            //Format TransValue to 2 decimal places.
            FormatDecimalValues(ref dt, new[] { CN.TotalAmountReceived, CN.TotalCash, CN.TotalCheque, CN.CashChange, CN.TotalPaymentPaid, CN.ChequeChange });

            transView = new DataView(dt);
            if (transView.Count > 0)		//make sure we have some records
            {
                ((Transactions)tpTransactions.Control).dgSummary.DataSource = transView; //

                if (((Transactions)tpTransactions.Control).dgSummary.TableStyles.Count == 0)
                {
                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                    tabStyle.MappingName = TN.LineItemAudit;

                    AddColumnStyle(CN.TotalAmountReceived, tabStyle, 110, true, GetResource("T_TOTALAMTREC"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.TotalCash, tabStyle, 100, true, GetResource("T_TOTALCASH"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.TotalCheque, tabStyle, 90, true, GetResource("T_TOTALCHEQUE"), "", HorizontalAlignment.Left);       // RI
                    AddColumnStyle(CN.CashChange, tabStyle, 60, true, GetResource("T_CHANGE"), "", HorizontalAlignment.Left); //IP - 05/02/10 - CR1072 - 3.1.12 - Renamed column to Stock Location from Location
                    AddColumnStyle(CN.TotalPaymentPaid, tabStyle, 60, true, GetResource("T_TOTALPAYMENTPAID"), "", HorizontalAlignment.Left); //IP - 05/02/10 - CR1072 - 3.1.12 - Added Del Note Branch
                    AddColumnStyle(CN.Date, tabStyle, 60, true, GetResource("T_DATES"), "", HorizontalAlignment.Left); //IP - 05/02/10 - CR1072 - 3.1.12 - Added Del Note Branch

                    ((Transactions)tpTransactions.Control).dgSummary.TableStyles.Add(tabStyle); //IP - 18/11/09 - CR929 & 974 - Audit

                    tabStyle.Dispose();
                }
            }            
        }

        //Format TransValue to 2 decimal places.
        private void FormatDecimalValues(ref DataTable dt, params string[] fieldNames)
        {            
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow transRow in dt.Rows)
                {
                    foreach (var fieldName in fieldNames)
                    {
                        if (!DBNull.Value.Equals(transRow[fieldName]))
                            transRow[fieldName] = Math.Round((decimal)transRow[fieldName], this._precision);
                        else
                            transRow[fieldName] = 0.0;
                    }                    
                }
                dt.AcceptChanges();
            }
        }

        private void LoadDetails(DataTable dt)
        {
            //Format TransValue to 2 decimal places.
            FormatDecimalValues(ref dt, CN.TransValue);            

            transView = new DataView(dt);
            if (transView.Count > 0)		//make sure we have some records
            {
                ((Transactions)tpTransactions.Control).dgDetails.DataSource = null;
                ((Transactions)tpTransactions.Control).dgDetails.DataSource = transView; //

                if (((Transactions)tpTransactions.Control).dgDetails.TableStyles.Count == 0)
                {
                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                    tabStyle.MappingName = TN.LineItemAudit;

                    AddColumnStyle(CN.AcctNo, tabStyle, 110, true, GetResource("T_ACCTNOS"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.TransRefNo, tabStyle, 100, true, GetResource("T_TRANSREFNOS"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.DateTrans, tabStyle, 90, true, GetResource("T_DATETRANS"), "", HorizontalAlignment.Left);       // RI
                    AddColumnStyle(CN.TransTypeCode, tabStyle, 60, true, GetResource("T_TRANSTYPECODES"), "", HorizontalAlignment.Left); //IP - 05/02/10 - CR1072 - 3.1.12 - Renamed column to Stock Location from Location
                    AddColumnStyle(CN.TransValue, tabStyle, 60, true, GetResource("T_TRANSVALUE"), "", HorizontalAlignment.Left); //IP - 05/02/10 - CR1072 - 3.1.12 - Added Del Note Branch
                    AddColumnStyle(CN.PayMethod, tabStyle, 60, true, GetResource("T_PAYMETHODS"), "", HorizontalAlignment.Left); //IP - 05/02/10 - CR1072 - 3.1.12 - Added Del Note Branch
                    AddColumnStyle(CN.EmployeeName, tabStyle, 110, true, GetResource("T_EMPLOYEENAMES"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.EmpeeNo, tabStyle, 100, true, GetResource("T_EMPEENOS"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.FtNotes, tabStyle, 90, true, GetResource("T_FTNOTES"), "", HorizontalAlignment.Left);       // RI
                    AddColumnStyle(CN.TransPrinted, tabStyle, 110, true, GetResource("T_TRANSPRINTED"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.Reference, tabStyle, 100, true, GetResource("T_REFERENCES"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.ChequeNo, tabStyle, 90, true, GetResource("T_CHEQUENOS"), "", HorizontalAlignment.Left);       // RI
                    AddColumnStyle(CN.CashierTotalId, tabStyle, 110, true, GetResource("T_CASHIERTOTALID"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.PaymentAmount, tabStyle, 100, true, GetResource("T_PAYMENTAMOUNTS"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.IsDeposit, tabStyle, 90, true, GetResource("T_ISDEPOSITS"), "", HorizontalAlignment.Left);       // RI
                    AddColumnStyle(CN.PostedToMambu, tabStyle, 100, true, GetResource("T_POSTEDTOMAMBU"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.MambuError, tabStyle, 90, true, GetResource("T_MAMBUERROR"), "", HorizontalAlignment.Left);       // RI

                    ((Transactions)tpTransactions.Control).dgDetails.TableStyles.Add(tabStyle); //IP - 18/11/09 - CR929 & 974 - Audit

                    tabStyle.Dispose();
                }
            }
        }

        private void LoadAdmin(DataTable dt)
        {

            //tpTransactions.Control = new Transactions();
            if (Convert.IsDBNull(dt.Rows[0]["Total Admin Fees"]) || dt.Rows[0]["Total Admin Fees"].ToString().Length == 0)
                ((Transactions)tpTransactions.Control).txtTotalAdmin.Text = "0";
            else
                ((Transactions)tpTransactions.Control).txtTotalAdmin.Text = Convert.ToDecimal(dt.Rows[0]["Total Admin Fees"]).ToString(DecimalPlaces);
        }

        private void LoadInterest(DataTable dt)
        {
            //tpTransactions.Control = new Transactions();
            if (Convert.IsDBNull(dt.Rows[0]["Total Interest"]) || dt.Rows[0]["Total Interest"].ToString().Length == 0)
                ((Transactions)tpTransactions.Control).txtTotalInterest.Text = "0";
            else
                ((Transactions)tpTransactions.Control).txtTotalInterest.Text = Convert.ToDecimal(dt.Rows[0]["Total Interest"]).ToString(DecimalPlaces);
        }

        //private void LoadCollectionReasons(DataTable dt)
        //{
        //    transView = new DataView(dt);
        //    dgTransactions.DataSource = transView;
        //    dgTransactions.TableStyles.Clear();
        //    if (dgTransactions.TableStyles.Count == 0)
        //    {
        //        DataGridTableStyle tabStyle = new DataGridTableStyle();
        //        tabStyle.MappingName = dt.TableName;
        //        dgTransactions.TableStyles.Add(tabStyle);

        //         Set the table style according to the user's preference
        //        tabStyle.GridColumnStyles[CN.acctno].HeaderText = GetResource("T_ACCTNO");
        //        tabStyle.GridColumnStyles[CN.ItemNo].HeaderText = GetResource("T_ITEMNO");
        //        tabStyle.GridColumnStyles[CN.DateAuthorised].HeaderText = GetResource("T_DATEAUTH");
        //        tabStyle.GridColumnStyles[CN.CollectionReason].HeaderText = GetResource("T_COLLECTIONREASON");
        //        tabStyle.GridColumnStyles[CN.DateCommissionCalculated].Width = 0;

        //        tabStyle.Dispose();
        //    }
        //}

        private void LoadTransactions(string accountNo)
        {
            /********** Table Definitions ***************************
                   Table[0] holds values from DN_AccountGetDetailsSP
                   Table[1] holds values from DN_AgreeementGetSP
                   Table[2] holds values from DN_FintransSumAccountGetSP
                   Table[3] holds values from DN_CollectionReasonsGetSP 
             *********************************************************/
            //tpTransactions.Control = new Transactions();


            Function = "LoadTransactions";

            try
            {
                if (transactions != null)
                {
                    //loop through the Tables in the dataset
                    foreach (DataTable dt in transactions.Tables)
                    {
                        //Check the table name and perform the appropraite method call
                        //for each of the three tables.
                        switch (dt.TableName)
                        {
                            case "Transactions":
                                LoadTransactions(dt);
                                break;
                            case "TotalAdmin":
                                LoadAdmin(dt);
                                break;
                            case "TotalInterest":
                                LoadInterest(dt);
                                break;
                            case TN.WarrantyCollectionReason:
                                //btnWarrantyCollections.Visible = (dt.Rows.Count > 0);
                                break;
                            default:
                                break;
                        }
                    }
                }
                else if (tcMain.TabPages.Contains(tpTransactions) && !tpTransactions.IsDisposed)
                    tcMain.TabPages.Remove(tpTransactions);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function = "End of LoadTransactions";
            }
        }

        private void FormatTextBoxesForAgreement()
        {
            this.txtAccountNo.BackColor = SystemColors.Window;
            this.txtCustID.BackColor = SystemColors.Window;
            this.txtAccountType.BackColor = SystemColors.Window;
            this.txtAddToPotential.BackColor = SystemColors.Window;
            this.txtAgreementTotal.BackColor = SystemColors.Window;
            this.txtAmountPaid.BackColor = SystemColors.Window;
            this.txtArrears.BackColor = SystemColors.Window;
            this.txtArrears.ForeColor = Color.Red;
            this.txtDateDelivered.BackColor = SystemColors.Window;
            this.txtDateDue.BackColor = SystemColors.Window;
            this.txtDateFInstalment.BackColor = SystemColors.Window;
            this.txtDateLastPaid.BackColor = SystemColors.Window;
            this.txtDateLInstalment.BackColor = SystemColors.Window;
            this.txtDateOpened.BackColor = SystemColors.Window;
            this.txtDeliveryTotal.BackColor = SystemColors.Window;
            this.txtDeposit.BackColor = SystemColors.Window;
            this.txtEarlySettlementFigure.BackColor = SystemColors.Window;
            this.txtIAmount.BackColor = SystemColors.Window;
            this.txtInstalments.BackColor = SystemColors.Window;
            this.txtInstalmentTotal.BackColor = SystemColors.Window;
            this.txtLastInstalmentAmount.BackColor = SystemColors.Window;
            this.txtLMovement.BackColor = SystemColors.Window;
            this.txtMthsInterestFree.BackColor = SystemColors.Window;
            this.txtOutstandingBalance.BackColor = SystemColors.Window;
            this.txtPercentagePaid.BackColor = SystemColors.Window;
            this.txtRebate.BackColor = SystemColors.Window;
            this.txtSChargeAmount.BackColor = SystemColors.Window;
            this.txtToFollowAmount.BackColor = SystemColors.Window;
            this.txtToFollowAmount.ForeColor = Color.Red;
            this.txtTotalFees.BackColor = SystemColors.Window;
            this.txtTTCode.BackColor = SystemColors.Window;
            this.txtTTDescription.BackColor = SystemColors.Window;
            this.txtFirstName.BackColor = SystemColors.Window;
            this.txtLastName.BackColor = SystemColors.Window;
            this.txtCODFlag.BackColor = SystemColors.Window;
            this.txtCurrentStatus.BackColor = SystemColors.Window;
            this.txtAvailableCredit.BackColor = SystemColors.Window;
            this.txtCardPrinted.BackColor = SystemColors.Window;
            this.txtRFIssueNo.BackColor = SystemColors.Window;
            this.txtTotalAgreements.BackColor = SystemColors.Window;
            this.txtTotalCredit.BackColor = SystemColors.Window;
            this.txtTotalArrears.BackColor = SystemColors.Window;
            this.txtTotalBalances.BackColor = SystemColors.Window;
            this.txtTotalInstalments.BackColor = SystemColors.Window;
            //this.txtSoldBYEmpeeName.BackColor = SystemColors.Window;
            //this.txtSoldOn.BackColor = SystemColors.Window;
            //this.txtDAedOn.BackColor = SystemColors.Window;
            //this.txtLastChangedOn.BackColor = SystemColors.Window;
            //this.txtSoldByEmpeeNo.BackColor = SystemColors.Window;
            //this.txtDAEmpeeNo.BackColor = SystemColors.Window;
            //this.txtLstChgEmpeeNo.BackColor = SystemColors.Window;
            //this.txtDAbyEmpeeName.BackColor = SystemColors.Window;
            //this.txtLstChgEmpeeName.BackColor = SystemColors.Window;
            //this.txtCreatedByName.BackColor = SystemColors.Window;
            //this.txtCreatedByNo.BackColor = SystemColors.Window;
            //this.txtCreatedDate.BackColor = SystemColors.Window;
            this.txtTotalSpend.BackColor = SystemColors.Window;
            // removed jec -- Malaysia merge
            //this.txtGiroPending.BackColor = SystemColors.Window;
        }

        private void loadAddresses(bool readOnly)
        {
            Function = "LoadAddresses";
            string mobileNo = String.Empty;
            string mobileNo2 = String.Empty;
            string mobileNo3 = String.Empty;
            string mobileNo4 = String.Empty;
            string addDesc = String.Empty;
            string addType = string.Empty;

            //IP - 17/03/11 - #3317 - CR1245
            string workDialCode2 = string.Empty;
            string workNum2 = string.Empty;
            string workExt2 = string.Empty;

            string workDialCode3 = string.Empty;
            string workNum3 = string.Empty;
            string workExt3 = string.Empty;

            string workDialCode4 = string.Empty;
            string workNum4 = string.Empty;
            string workExt4 = string.Empty;


            // Address Standardization CR2019 - 025
            string deliveryMobile = String.Empty;
            string delivery1Mobile = String.Empty;
            string delivery2Mobile = String.Empty;
            string delivery3Mobile = String.Empty;

            if (custAddresses != null)
            {
                StringCollection tabs = new StringCollection();
                foreach (DataRow row in custAddresses.Tables[TN.CustomerAddresses].Rows)
                {
                    addType = ((string)row["AddressType"]).Trim();
                    if (row["AddressDescription"] != DBNull.Value)
                        addDesc = ((string)row["AddressDescription"]).Trim();
                    else
                        addDesc = String.Empty;

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



                    //IP - 17/03/11 - #3317 - CR1245
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

                    //IP - 17/03/11 - #3317 - CR1245 - Don't add seperate Work tabs for W2, W3, W4
                    if (!tabs.Contains(addType) && addType != GetResource("L_MOBILE") && addType != GetResource("L_MOBILE2") && addType != GetResource("L_MOBILE3") && addType != GetResource("L_MOBILE4")
                        && addType != GetResource("L_WORK2") && addType != GetResource("L_WORK3") && addType != GetResource("L_WORK4") && addType != GetResource("L_DMOBILE") && addType != GetResource("L_D1MOBILE") && addType != GetResource("L_D2MOBILE") && addType != GetResource("L_D3MOBILE"))	//if this is a new tab
                    {
                        tabs.Add(addType);
                        Crownwood.Magic.Controls.TabPage tp = new Crownwood.Magic.Controls.TabPage(addDesc);
                        currentTab = tp;
                        tp.Tag = false;
                        tp.BorderStyle = BorderStyle.Fixed3D;
                        AddressTab at = new AddressTab(readOnly, FormRoot, addType);     //CR1084 jec
                        at.Enable = false; // Address Standardization CR2019 - 025
                        //5.1 uat 165 rdb 20/11/07 removed this check as address tabs
                        //where not being displayed
                        //if (tp.Control != null)
                        //{
                        this.tcAddress.TabPages.Add(tp);
                        //}
                        tp.Controls.Add(at);
                        tp.Name = "tp" + addType;

                        /*tp.Title = addType;*/
                    }

                    foreach (Crownwood.Magic.Controls.TabPage t in tcAddress.TabPages)
                    {
                        if (t.Name == "tp" + addType)
                        {
                            if (((AddressTab)t.Controls[0]).CFirstname.Text.Length == 0)
                                ((AddressTab)t.Controls[0]).CFirstname.Text = (string)row["DELFirstName"];
                            if (((AddressTab)t.Controls[0]).CLastname.Text.Length == 0)
                                ((AddressTab)t.Controls[0]).CLastname.Text = (string)row["DELLastname"];
                            // Address Standardization CR2019 - 025
                            if (row["Address1"] != DBNull.Value)
                                ((AddressTab)t.Controls[0]).txtAddress1.Text = (string)row["Address1"];
                            if (((AddressTab)t.Controls[0]).cmbVillage.Items.Count > 0 &&
                                    row["Address2"] != DBNull.Value)
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
                            row["Address3"] != DBNull.Value)
                            {
                                var regionIndex = ((AddressTab)t.Controls[0]).cmbRegion.FindStringExact((string)row["Address3"]);
                                if (regionIndex != -1)
                                    ((AddressTab)t.Controls[0]).cmbRegion.SelectedIndex = regionIndex;
                                else
                                    ((AddressTab)t.Controls[0]).cmbRegion.SelectedText = (string)row["Address3"];
                            }
                            else if (row["Address3"] != DBNull.Value)
                                ((AddressTab)t.Controls[0]).cmbRegion.SelectedText = (string)row["Address3"];
                            if (row["PostCode"] != DBNull.Value)
                                ((AddressTab)t.Controls[0]).txtPostCode.Text = (string)row["PostCode"];
                            if (!Convert.IsDBNull(row["Latitude"]) && !Convert.IsDBNull(row["Longitude"]))
                                ((AddressTab)t.Controls[0]).txtCoordinate.Text = string.Format("{0},{1}", row["Latitude"].ToString(), row["Longitude"].ToString());
                            // Address Standardization CR2019 - 025
                            if (((AddressTab)t.Controls[0]).txtNotes.Text.Length == 0 && row["Notes"] != DBNull.Value)
                                ((AddressTab)t.Controls[0]).txtNotes.Text = (string)row["Notes"];
                            if (((AddressTab)t.Controls[0]).txtEmail.Text.Length == 0)
                                ((AddressTab)t.Controls[0]).txtEmail.Text = (string)row["Email"];
                            if (((AddressTab)t.Controls[0]).txtDialCode.Text.Length == 0)
                                ((AddressTab)t.Controls[0]).txtDialCode.Text = (string)row["DialCode"];
                            if (((AddressTab)t.Controls[0]).txtPhoneNo.Text.Length == 0)
                                ((AddressTab)t.Controls[0]).txtPhoneNo.Text = (string)row["Phone"];
                            if (((AddressTab)t.Controls[0]).txtExtension.Text.Length == 0)
                                if (DBNull.Value != row["Ext"])
                                    ((AddressTab)t.Controls[0]).txtExtension.Text = (string)row["Ext"];
                            if (!Convert.IsDBNull(row["Date In"]))
                                ((AddressTab)t.Controls[0]).dtDateIn.Value = (DateTime)row["Date In"];
                            // Delivery Area
                            ((AddressTab)t.Controls[0]).SetDeliveryArea((string)row[CN.DeliveryArea]);

                            //Disable Mobile No field if not Home tab
                            if (addType == "W")// Address Standardization CR2019 - 025
                            {
                                ((AddressTab)t.Controls[0]).txtMobile.Enabled = false;
                                ((AddressTab)t.Controls[0]).txtMobile.BackColor = SystemColors.Menu;
                                ((AddressTab)t.Controls[0]).btnMobile.Visible = false;

                            }

                            //IP - 17/03/11 - #3317 - CR1245 - Do not display the button to add multiple work numbers if this is not the work address tab
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

                tabs.Clear();

                //IP - 17/03/11 - #3317 - CR1245 - Display any additional work numbers on the Work Address tab
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
            }

            Function = "End of LoadAddresses";
        }

        private void btnOtherAccounts_Click_1(object sender, System.EventArgs e)
        {
            try
            {
                Function = "btnOtherAccounts_Click_1";

                Wait();

                if ((string)btnToggle.Tag == "Addresses")
                {
                    btnToggle.Tag = "Accounts";
                    btnToggle.Text = "Show Addresses";
                    dgAccounts.BringToFront();
                }
                else
                {
                    btnToggle.Tag = "Addresses";
                    btnToggle.Text = "Show Accounts";
                    tcAddress.BringToFront();
                }

                StopWait();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of btnOtherAccounts_Click_1";
            }
        }


        private void LoadOtherAccounts()
        {
            try
            {
                // Display any other accounts that exist for this customer.
                if (accounts != null)
                {
                    otherAcctsView = new DataView(accounts.Tables[0]);
                    if (otherAcctsView.Count > 0)
                    {
                        dgAccounts.DataSource = otherAcctsView;
                        if (dgAccounts.TableStyles.Count == 0)
                        {
                            DataGridTableStyle tabStyle = new DataGridTableStyle();
                            tabStyle.MappingName = accounts.Tables[0].TableName;
                            dgAccounts.TableStyles.Add(tabStyle);

                            // Set the table style
                            tabStyle.GridColumnStyles["Customer ID"].Width = 0;
                            tabStyle.GridColumnStyles["Title"].Width = 0;
                            tabStyle.GridColumnStyles["First Name"].Width = 0;
                            tabStyle.GridColumnStyles["Last Name"].Width = 0;
                            tabStyle.GridColumnStyles["AccountNumber"].Width = 95;
                            tabStyle.GridColumnStyles["AccountNumber"].HeaderText = GetResource("T_ACCTNO");
                            tabStyle.GridColumnStyles["AccountNumber"].Alignment = HorizontalAlignment.Left;
                            tabStyle.GridColumnStyles["Type"].Width = 0;
                            tabStyle.GridColumnStyles["Date Opened"].Width = 75;
                            tabStyle.GridColumnStyles["Date Opened"].HeaderText = GetResource("T_DATEOPENED");
                            tabStyle.GridColumnStyles["Date Opened"].Alignment = HorizontalAlignment.Left;
                            tabStyle.GridColumnStyles["Outstanding Balance"].Width = 75;
                            tabStyle.GridColumnStyles["Outstanding Balance"].HeaderText = GetResource("T_OUTBAL");
                            tabStyle.GridColumnStyles["Outstanding Balance"].Alignment = HorizontalAlignment.Left;
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles["Outstanding Balance"]).Format = DecimalPlaces;
                            tabStyle.GridColumnStyles["Arrears"].Width = 75;
                            tabStyle.GridColumnStyles["Arrears"].HeaderText = GetResource("T_ARREARS");
                            tabStyle.GridColumnStyles["Arrears"].Alignment = HorizontalAlignment.Left;
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles["Arrears"]).Format = DecimalPlaces;
                            tabStyle.GridColumnStyles["Agreement Total"].Width = 75;
                            tabStyle.GridColumnStyles["Agreement Total"].HeaderText = GetResource("T_AGREETOTAL");
                            tabStyle.GridColumnStyles["Agreement Total"].Alignment = HorizontalAlignment.Left;
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles["Agreement Total"]).Format = DecimalPlaces;
                            tabStyle.GridColumnStyles["Status"].Width = 45;
                            tabStyle.GridColumnStyles["Status"].HeaderText = GetResource("T_STATUS");
                            tabStyle.GridColumnStyles["Status"].Alignment = HorizontalAlignment.Left;
                            tabStyle.GridColumnStyles["Alias"].Width = 0;
                            tabStyle.GridColumnStyles[CN.AddTo].Width = 0;
                            tabStyle.GridColumnStyles[CN.DeliveredIndicator].Width = 0;
                            tabStyle.GridColumnStyles[CN.Reversible].Width = 0;
                            tabStyle.GridColumnStyles[CN.AgrmtNo].Width = 0;

                            tabStyle.Dispose();
                        }
                    }

                    string temp1 = txtAccountNo.Text.Replace("-", String.Empty);

                    int i = 0;
                    foreach (DataRow row in accounts.Tables[0].Rows)
                    {
                        if ((string)row["AccountNumber"] == temp1)
                        {
                            dgAccounts.Select(i);
                            dgAccounts.CurrentRowIndex = i;
                            break;
                        }
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void LoadRFCombined(string custID, string acctType)
        {
            try
            {
                if (acctType.Trim() == "R")
                {
                    // Ready Finance Account so add the RF details tab.
                    if (!tcMain.TabPages.Contains(tpRFCombined))
                    {
                        tcMain.TabPages.Add(tpRFCombined);
                    }

                    DataTable dtRF = null;

                    if (RFCombined != null)
                    {
                        foreach (DataTable dt in RFCombined.Tables)
                        {
                            dtRF = dt;
                        }

                        foreach (DataRow row in dtRF.Rows)
                        {
                            txtAvailableCredit.Text = Convert.ToDecimal(row[CN.AvailableCredit]).ToString(DecimalPlaces);
                            txtCardPrinted.Text = (string)row[CN.CardPrinted];
                            txtTotalAgreements.Text = Convert.ToDecimal(row[CN.TotalAgreements]).ToString(DecimalPlaces);
                            txtTotalArrears.Text = Convert.ToDecimal(row[CN.TotalArrears]).ToString(DecimalPlaces);
                            txtTotalBalances.Text = Convert.ToDecimal(row[CN.TotalBalances]).ToString(DecimalPlaces);
                            txtTotalCredit.Text = Convert.ToDecimal(row[CN.TotalCredit]).ToString(DecimalPlaces);
                            txtTotalInstalments.Text = Convert.ToDecimal(row[CN.TotalDeliveredInstalments]).ToString(DecimalPlaces);
                            txtRFIssueNo.Text = Convert.ToString(row[CN.RFCardSeqNo]);
                        }
                    }
                }
                else if (tcMain.TabPages.Contains(tpRFCombined) && !tpRFCombined.IsDisposed) // This is not a Ready Finance Account so remove the RF details tab.
                {
                    tcMain.TabPages.Remove(tpRFCombined);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void btnClose_Click_1(object sender, System.EventArgs e)
        {
            this.CloseTab();
        }

        private void btnCustCodes_Click(object sender, System.EventArgs e)
        {
            try
            {
                AddCustAcctCodes codes = new AddCustAcctCodes(AddCodes, txtCustID.Text, txtFirstName.Text, txtLastName.Text, txtAccountNo.Text);
                codes.FormRoot = this.FormRoot;
                codes.FormParent = this;
                // UAT 329 JH 16/10/2007
                if (AddCodes == true)
                {
                    codes.Text = "Account/Customer Codes";
                }
                ((MainForm)this.FormRoot).AddTabPage(codes, 8);
                codes.CustomerCodes(txtCustID.Text);
            }
            catch (Exception ex)
            {
                Catch(ex, "btnCustCodes_Click");
            }
        }

        private void tvItems_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            try
            {
                if ((string)e.Node.Tag != null)
                {
                    string key = (string)e.Node.Tag;
                    // uat363 added parentProductNo
                    string[] keyParts = key.Split('|');
                    string itemId = keyParts[0];
                    string location = keyParts[1];
                    string parentItemId = keyParts[2];

                    int index = 0;
                    foreach (DataRowView row in itemsView)
                    {
                        if ((string)row.Row[CN.ItemId] == itemId && (string)row.Row["StockLocation"] == location
                                && row[CN.ParentItemId].ToString() == parentItemId)
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

        private void LoadDeliveries(string accountNumber)
        {
            dgDelivery.DataSource = null;
            dgDelivery.TableStyles.Clear();

            if (deliveries != null)
            {
                DataView dv = new DataView(deliveries.Tables[0]);
                if (dv.Count > 0)
                {
                    //**You cannot put a string into a DataRowView/DataView field that contains a DateTime value JH 07/06/2007**

                    //foreach (DataRowView row in dv)
                    //{
                    //    if (row[CN.ScheduledDate] == DBNull.Value)
                    //    {
                    //        if (Convert.ToDecimal(row[CN.Quantity]) < 0)
                    //            row[CN.ScheduledDate] = GetResource("T_COLLECTED");
                    //        else
                    //            row[CN.ScheduledDate] = GetResource("T_DELIVERED");
                    //    }
                    //}

                    dgDelivery.DataSource = dv;

                    if (dgDelivery.TableStyles.Count == 0)
                    {
                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        tabStyle.MappingName = dv.Table.TableName;
                        dgDelivery.TableStyles.Add(tabStyle);

                        tabStyle.GridColumnStyles[CN.ItemNo].Width = 90;
                        tabStyle.GridColumnStyles[CN.ItemNo].HeaderText = GetResource("T_ITEMNO");

                        tabStyle.GridColumnStyles[CN.ItemId].Width = 0;         //RI  jec 26/05/11
                        tabStyle.GridColumnStyles[CN.ParentItemId].Width = 0;         //RI  jec 26/05/11
                        tabStyle.GridColumnStyles[CN.RetItemId].Width = 0;         //RI  jec 26/05/11

                        tabStyle.GridColumnStyles[CN.ItemDescr1].Width = 150;
                        tabStyle.GridColumnStyles[CN.ItemDescr1].HeaderText = GetResource("T_DESCRIPTION");

                        tabStyle.GridColumnStyles[CN.StockLocn].Width = 50;
                        tabStyle.GridColumnStyles[CN.StockLocn].HeaderText = GetResource("T_STOCKLOCN");

                        tabStyle.GridColumnStyles[CN.Quantity].Width = 50;
                        tabStyle.GridColumnStyles[CN.Quantity].HeaderText = GetResource("T_QUANTITY");
                        tabStyle.GridColumnStyles[CN.Quantity].Alignment = HorizontalAlignment.Right;

                        tabStyle.GridColumnStyles[CN.DateDel].Width = 75;
                        tabStyle.GridColumnStyles[CN.DateDel].HeaderText = GetResource("T_DELIVERYDATE");
                        tabStyle.GridColumnStyles[CN.DateDel].NullText = GetResource("T_SCHEDULED");

                        // 68440 RD 24/08/06
                        tabStyle.GridColumnStyles[CN.ScheduledDate].Width = 75;
                        tabStyle.GridColumnStyles[CN.ScheduledDate].HeaderText = GetResource("T_SCHEDULEDDATE");
                        tabStyle.GridColumnStyles[CN.ScheduledDate].NullText = GetResource("T_DELIVERED");

                        tabStyle.GridColumnStyles[CN.Value].Width = 100;
                        tabStyle.GridColumnStyles[CN.Value].HeaderText = GetResource("T_VALUE");
                        tabStyle.GridColumnStyles[CN.Value].Alignment = HorizontalAlignment.Right;
                        ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Value]).Format = DecimalPlaces;

                        tabStyle.GridColumnStyles[CN.BuffNo].Width = 60;
                        tabStyle.GridColumnStyles[CN.BuffNo].HeaderText = GetResource("T_BUFFNO");

                        //IP - 20/04/10 - UAT(107) UAT5.2
                        if (!thirdPartyDeliveriesEnabled)
                            tabStyle.GridColumnStyles[CN.OrigBuffno].Width = 0;

                        // 68440 RD 24/08/06
                        tabStyle.GridColumnStyles[CN.LoadNo].Width = 60;
                        tabStyle.GridColumnStyles[CN.LoadNo].HeaderText = GetResource("T_LOADNO");

                        // 68440 RD 24/08/06
                        tabStyle.GridColumnStyles[CN.TruckID].Width = 60;
                        tabStyle.GridColumnStyles[CN.TruckID].HeaderText = GetResource("T_TRUCKID");

                        tabStyle.GridColumnStyles[CN.DelOrColl].Width = 40;
                        tabStyle.GridColumnStyles[CN.DelOrColl].HeaderText = GetResource("T_DELORCOLL");

                        tabStyle.GridColumnStyles[CN.ContractNo].Width = 60;
                        tabStyle.GridColumnStyles[CN.ContractNo].HeaderText = GetResource("T_CONTRACTNO");

                        tabStyle.GridColumnStyles[CN.RetItemNo].Width = 60;
                        tabStyle.GridColumnStyles[CN.RetItemNo].HeaderText = GetResource("T_RETITEM");

                        tabStyle.GridColumnStyles[CN.RetStockLocn].Width = 60;
                        tabStyle.GridColumnStyles[CN.RetStockLocn].HeaderText = GetResource("T_RETLOCN");

                        tabStyle.GridColumnStyles[CN.NotifiedBy].Width = 60;
                        tabStyle.GridColumnStyles[CN.NotifiedBy].HeaderText = GetResource("T_NOTIFIEDBY");

                        for (int i = 0; i < dv.Count; i++)
                        {
                            if (dv.Table.Rows[i][CN.ScheduledDate] == DBNull.Value)
                            {
                                if (Convert.ToDecimal(dv.Table.Rows[i][CN.Quantity]) < 0)
                                    //tabStyle.GridColumnStyles[CN.ScheduledDate].NullText = GetResource("T_COLLECTED");
                                    dv.Table.Rows[i][CN.ScheduledDate] = GetResource("T_COLLECTED");
                                else
                                    //tabStyle.GridColumnStyles[CN.ScheduledDate].NullText = GetResource("T_DELIVERED");
                                    dv.Table.Rows[i][CN.ScheduledDate] = GetResource("T_DELIVERED");
                            }
                        }

                        //UAT 525
                        tabStyle.GridColumnStyles[CN.OrdVal].Width = 60;
                        //tabStyle.GridColumnStyles[CN.OrdVal].HeaderText = GetResource("T_VALUE");
                        tabStyle.GridColumnStyles[CN.OrdVal].Alignment = HorizontalAlignment.Center;
                        ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.OrdVal]).Format = DecimalPlaces;

                        tabStyle.GridColumnStyles[CN.DHLInterfaceDate].Width = 75;
                        tabStyle.GridColumnStyles[CN.DHLInterfaceDate].HeaderText = GetResource("T_DHLInterfaceDate");

                        if (!thirdPartyDeliveriesEnabled)       //UAT114 jec 21/04/10
                        {
                            // Do not show columns                            
                            tabStyle.GridColumnStyles["DHLInterfaceDate"].Width = 0;
                        }

                        tabStyle.Dispose();
                    }
                }
            }
        }

        private void tcMain_SelectionChanged(object sender, System.EventArgs e)
        {
            try
            {
                switch (tcMain.SelectedTab.Name)
                {
                    case "tpApplicationStatus":
                        LoadAccountStatus();
                        break;
                    case "tpLetters":
                        LoadLetters();
                        break;
                    case "tpActions":
                        LoadAllocationDetails();
                        break;
                    case TPN.AllocationHistory:
                        LoadAllocationHistory();
                        break;
                    case "tpLineItems":
                        LoadLineItems(accountNo, txtAccountType.Text);
                        break;
                    case "tpAgreementChanges":
                        LoadAgreementChanges();
                        break;
                    case "tpActivitySegments":
                        LoadActivitySegments();
                        break;
                    case "tpPendingCharges":
                        LoadPendingCharges();
                        LoadArrearsCharges();
                        break;
                    case "tpStrategies":
                        LoadStrategiesandWorklists();
                        break;
                    case "tpInstallation":
                        LoadInstallationBooking();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "tcMain_SelectionChanged");
            }
        }

        private void LoadAgreementChanges()
        {
            try
            {
                Wait();

                //IP - 18/11/09 - CR929 & 974 - Audit
                if (tpAgreementChanges.Control == null)
                {
                    tpAgreementChanges.Control = new AuditChanges();
                }

                if (Audit == null)
                {
                    Audit = AccountManager.GetAccountAuditData(this.accountNo, out Error);

                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                        return;
                    }

                    foreach (DataTable dt in Audit.Tables)
                    {
                        switch (dt.TableName)
                        {
                            case TN.LineItemAudit:
                                ((AuditChanges)tpAgreementChanges.Control).dgLineItem.DataSource = dt.DefaultView; //IP - 18/11/09 - CR929 & 974 - Audit
                                //if (dgLineItem.TableStyles.Count == 0)
                                if (((AuditChanges)tpAgreementChanges.Control).dgLineItem.TableStyles.Count == 0)
                                {
                                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                                    tabStyle.MappingName = TN.LineItemAudit;

                                    AddColumnStyle(CN.acctno, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.AgrmtNo, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.EmpeeNoChange, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.DateChange, tabStyle, 110, true, GetResource("T_DATECHANGED"), "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.EmployeeName, tabStyle, 100, true, GetResource("T_CHANGEDBY"), "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.ItemNo, tabStyle, 90, true, GetResource("T_ITEMNO"), "", HorizontalAlignment.Left);       // RI
                                    AddColumnStyle(CN.StockLocn, tabStyle, 60, true, GetResource("T_STOCKLOCN"), "", HorizontalAlignment.Left); //IP - 05/02/10 - CR1072 - 3.1.12 - Renamed column to Stock Location from Location
                                    AddColumnStyle(CN.DelNoteBranch, tabStyle, 60, true, GetResource("T_DELNOTEBRANCH"), "", HorizontalAlignment.Left); //IP - 05/02/10 - CR1072 - 3.1.12 - Added Del Note Branch
                                    AddColumnStyle(CN.QuantityBefore, tabStyle, 60, true, GetResource("T_QUANTITYBEFORE"), String.Empty, HorizontalAlignment.Right);
                                    AddColumnStyle(CN.QuantityAfter, tabStyle, 60, true, GetResource("T_QUANTITYAFTER"), String.Empty, HorizontalAlignment.Right);
                                    AddColumnStyle(CN.ValueBefore, tabStyle, 80, true, GetResource("T_VALUEBEFORE"), DecimalPlaces, HorizontalAlignment.Right);
                                    AddColumnStyle(CN.ValueAfter, tabStyle, 80, true, GetResource("T_VALUEAFTER"), DecimalPlaces, HorizontalAlignment.Right);
                                    AddColumnStyle(CN.TaxAmtBefore, tabStyle, 80, true, GetResource("T_TAXAMTBEFORE"), DecimalPlaces, HorizontalAlignment.Right);  // 67977 RD 22/02/06
                                    AddColumnStyle(CN.TaxAmtAfter, tabStyle, 80, true, GetResource("T_TAXAMTAFTER"), DecimalPlaces, HorizontalAlignment.Right); // 67977 RD 22/02/06
                                    AddColumnStyle(CN.ContractNo, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                    //dgLineItem.TableStyles.Add(tabStyle);
                                    ((AuditChanges)tpAgreementChanges.Control).dgLineItem.TableStyles.Add(tabStyle); //IP - 18/11/09 - CR929 & 974 - Audit

                                    tabStyle.Dispose();
                                }
                                break;
                            //case TN.AgreementAudit: dgAgreement.DataSource = dt.DefaultView;
                            case TN.AgreementAudit:
                                ((AuditChanges)tpAgreementChanges.Control).dgAgreement.DataSource = dt.DefaultView; //IP - 18/11/09 - CR929 & 974 - Audit
                                //if (dgAgreement.TableStyles.Count == 0)
                                if (dgAgreementAudit.TableStyles.Count == 0)
                                {
                                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                                    tabStyle.MappingName = TN.AgreementAudit;

                                    AddColumnStyle(CN.acctno, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.AgrmtNo, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.EmpeeNoChange, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.DateChange, tabStyle, 100, true, GetResource("T_DATECHANGED"), "", HorizontalAlignment.Center);
                                    AddColumnStyle(CN.EmployeeName, tabStyle, 100, true, GetResource("T_CHANGEDBY"), "", HorizontalAlignment.Center);
                                    AddColumnStyle(CN.OldAgreementTotal, tabStyle, 80, true, GetResource("T_OLDAGREEMENTTOTAL"), DecimalPlaces, HorizontalAlignment.Center);
                                    AddColumnStyle(CN.NewAgreementTotal, tabStyle, 85, true, GetResource("T_NEWAGREEMENTTOTAL"), DecimalPlaces, HorizontalAlignment.Center);
                                    AddColumnStyle(CN.OldServiceCharge, tabStyle, 90, true, GetResource("T_OLDSERVICECHARGE"), DecimalPlaces, HorizontalAlignment.Center);
                                    AddColumnStyle(CN.NewServiceCharge, tabStyle, 95, true, GetResource("T_NEWSERVICECHARGE"), DecimalPlaces, HorizontalAlignment.Center);
                                    AddColumnStyle(CN.OldDeposit, tabStyle, 75, true, GetResource("T_OLDDEPOSIT"), DecimalPlaces, HorizontalAlignment.Center);
                                    AddColumnStyle(CN.NewDeposit, tabStyle, 80, true, GetResource("T_NEWDEPOSIT"), DecimalPlaces, HorizontalAlignment.Center);
                                    //69516 COD changes to be displayed
                                    AddColumnStyle(CN.OldCODflag, tabStyle, 50, true, GetResource("T_OLDCOD"), String.Empty, HorizontalAlignment.Center);
                                    AddColumnStyle(CN.NewCODflag, tabStyle, 55, true, GetResource("T_NEWCOD"), String.Empty, HorizontalAlignment.Center);
                                    AddColumnStyle("oldTermstype", tabStyle, 50, true, "Old Termstype", String.Empty, HorizontalAlignment.Center);
                                    AddColumnStyle("NewTermstype", tabStyle, 50, true, "New Termstype", String.Empty, HorizontalAlignment.Center);
                                    AddColumnStyle("OldIntrate", tabStyle, 50, true, "Old Int Rate", String.Empty, HorizontalAlignment.Center);
                                    AddColumnStyle("NewIntrate", tabStyle, 50, true, "New Int Rate", String.Empty, HorizontalAlignment.Center);

                                    dgAgreementAudit.TableStyles.Add(tabStyle);
                                    tabStyle.Dispose();
                                }
                                break;
                            //case TN.InstalPlanAudit: dgInstalPlan.DataSource = dt.DefaultView;
                            case TN.InstalPlanAudit:
                                ((AuditChanges)tpAgreementChanges.Control).dgInstalPlan.DataSource = dt.DefaultView; //IP - 18/11/09 - CR929 & 974 - Audit
                                //if (dgInstalPlan.TableStyles.Count == 0)
                                if (((AuditChanges)tpAgreementChanges.Control).dgInstalPlan.TableStyles.Count == 0)
                                {
                                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                                    tabStyle.MappingName = TN.InstalPlanAudit;

                                    AddColumnStyle(CN.acctno, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.AgrmtNo, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.EmpeeNoChange, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.DateChange, tabStyle, 110, true, GetResource("T_DATECHANGED"), "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.EmployeeName, tabStyle, 120, true, GetResource("T_CHANGEDBY"), "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.OldInstalNo, tabStyle, 80, true, GetResource("T_OLDINSTALNO"), "", HorizontalAlignment.Right);
                                    AddColumnStyle(CN.NewInstalNo, tabStyle, 80, true, GetResource("T_NEWINSTALNO"), "", HorizontalAlignment.Right);
                                    AddColumnStyle(CN.OldInstalment, tabStyle, 140, true, GetResource("T_OLDINSTALMENT"), DecimalPlaces, HorizontalAlignment.Right);
                                    AddColumnStyle(CN.NewInstalment, tabStyle, 140, true, GetResource("T_NEWINSTALMENT"), DecimalPlaces, HorizontalAlignment.Right);
                                    //dgInstalPlan.TableStyles.Add(tabStyle);
                                    ((AuditChanges)tpAgreementChanges.Control).dgInstalPlan.TableStyles.Add(tabStyle); //IP - 18/11/09 - CR929 & 974 - Audit

                                    tabStyle.Dispose();
                                }
                                break;
                            case TN.DeliveryNotificationAudit:
                                ((AuditChanges)tpAgreementChanges.Control).dgDeliveryNotificationAudit.DataSource = dt.DefaultView; //IP - 18/11/09 - CR929 & 974 - Audit
                                if (((AuditChanges)tpAgreementChanges.Control).dgDeliveryNotificationAudit.TableStyles.Count == 0)
                                {
                                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                                    tabStyle.MappingName = TN.DeliveryNotificationAudit;

                                    AddColumnStyle(CN.ItemNo, tabStyle, 60, true, GetResource("T_ITEMNO"), "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.BuffNo, tabStyle, 150, true, GetResource("T_DELNOTENUMBER"), "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.DateRemoved, tabStyle, 150, true, GetResource("T_REMOVEDDELETEDDATE"), "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.RemovedBy, tabStyle, 150, true, GetResource("T_REMOVEDDELETEDBY"), "", HorizontalAlignment.Left);
                                    AddColumnStyle(CN.Action, tabStyle, 100, true, GetResource("T_ACTION"), "", HorizontalAlignment.Left);

                                    ((AuditChanges)tpAgreementChanges.Control).dgDeliveryNotificationAudit.TableStyles.Add(tabStyle); //IP - 18/11/09 - CR929 & 974 - Audit

                                    tabStyle.Dispose();
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "LoadAgreementChanges");
            }
            finally
            {
                StopWait();
            }
        }

        private void AccountDetails_Enter(object sender, System.EventArgs e)
        {
            try
            {
                BuildRecentListMenus();
            }
            catch (Exception ex)
            {
                Catch(ex, "AccountDetails_Enter");
            }
        }

        private void LoadAccountStatus()
        {
            try
            {
                Function = "LoadAccountStatus";
                Wait();

                // Load account status
                if (accountNo.Substring(3, 1) != "9")
                {
                    DataSet accountStatus = AccountManager.AccountApplicationStatus(accountNo, out Error);

                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                    }
                    else
                    {
                        ApplicationStatusView = new DataView(accountStatus.Tables[0]);

                        if (ApplicationStatusView.Count > 0)		//make sure we have some records
                        {
                            //Set the datasource of the datagrid that is a member variable of
                            //the transactionsTab object (set up the column styles also)
                            dgApplicationStatus.Columns.Clear();                           //#15501 //#15467
                            dgApplicationStatus.ColumnStylePreLoad(CN.StatusCode, null, 80, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft);
                            dgApplicationStatus.ColumnStylePreLoad(CN.StatusDescription, null, 600, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft);


                            dgApplicationStatus.DataSource = ApplicationStatusView;

                            //if (dgApplicationStatus.TableStyles.Count == 0)
                            //{
                            //    DataGridTableStyle tabStyle = new DataGridTableStyle();
                            //    tabStyle.MappingName = ApplicationStatusView.Table.TableName;
                            //    dgApplicationStatus.TableStyles.Add(tabStyle);

                            //    // Set the table style according to the user's preference
                            //   tabStyle.GridColumnStyles[CN.StatusCode].Width = 100;
                            //    tabStyle.GridColumnStyles[CN.StatusCode].HeaderText = GetResource("T_STATUSCODE");
                            //    tabStyle.GridColumnStyles[CN.StatusDescription].Width = 300;
                            //    tabStyle.GridColumnStyles[CN.StatusDescription].HeaderText = GetResource("T_STATUSDESC");

                            //    tabStyle.Dispose();
                            //}
                        }
                    }
                }
                else
                {
                    Client.Call(new GetAccountStatusRequest { acctno = accountNo },
                    response =>
                    {
                        Response(response.status);
                    }, this);


                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of LoadAccountStatus";
            }
        }

        private void Response(string status)
        {

            dgApplicationStatus.Columns.Clear();
            dgApplicationStatus.ColumnStyleInit();
            dgApplicationStatus.ColumnStylePreLoad("AccountStatusName", "AccountStatus", 180, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft);
            dgApplicationStatus.DataSource = new List<StatusShow>()
                                                {
                                                    new StatusShow() { AccountStatusName = StoreCardAccountStatus_Lookup.FromString(status).Description }
                                                };
        }

        private class StatusShow
        {
            public string AccountStatusName { get; set; }
            //            public string AccountStatus { get; set; }
        }

        private void LoadLetters()
        {
            /* add the loading of status code history into this tab also */
            try
            {
                Wait();

                if (tcMain.SelectedTab != tpLetters)
                    return;

                DataSet statusHist = null;

                // Load account status
                DataSet letters = AccountManager.GetLetterByAcctNo(accountNo, out Error);

                if (Error.Length > 0)
                {
                    ShowError(Error);
                }
                else
                {
                    LetterView = new DataView(letters.Tables[0]);
                    dgLetters.DataSource = null;

                    if (LetterView.Count > 0)		//make sure we have some records
                    {
                        //Set the datasource of the datagrid that is a member variable of
                        //the transactionsTab object (set up the column styles also)
                        dgLetters.DataSource = LetterView;

                        if (dgLetters.TableStyles.Count == 0)
                        {
                            DataGridTableStyle tabStyle = new DataGridTableStyle();
                            tabStyle.MappingName = LetterView.Table.TableName;
                            dgLetters.TableStyles.Add(tabStyle);

                            // Set the table style according to the user's preference
                            tabStyle.GridColumnStyles[CN.acctno].Width = 0;
                            tabStyle.GridColumnStyles[CN.DateAcctLttr].Width = 100;
                            tabStyle.GridColumnStyles[CN.DateAcctLttr].HeaderText = GetResource("T_DATEACCTLTTR");
                            tabStyle.GridColumnStyles[CN.LetterCode].Width = 80;
                            tabStyle.GridColumnStyles[CN.LetterCode].HeaderText = GetResource("T_LETTERCODE");
                            tabStyle.GridColumnStyles[CN.Description].Width = 160;
                            tabStyle.GridColumnStyles[CN.ExcelGen].Width = 20;

                            tabStyle.Dispose();
                        }
                    }
                }

                statusHist = AccountManager.GetStatusForAccount(accountNo, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    StatusView = new DataView(statusHist.Tables[0]);

                    dgStatus.DataSource = null;

                    if (StatusView.Count > 0)		//make sure we have some records
                    {
                        dgStatus.DataSource = StatusView;

                        if (dgStatus.TableStyles.Count == 0)
                        {
                            DataGridTableStyle tabStyle = new DataGridTableStyle();
                            tabStyle.MappingName = StatusView.Table.TableName;
                            dgStatus.TableStyles.Add(tabStyle);

                            // Set the table style according to the user's preference
                            tabStyle.GridColumnStyles[CN.acctno].Width = 0;
                            tabStyle.GridColumnStyles[CN.EmpeeNoStat].Width = 0;
                            tabStyle.GridColumnStyles[CN.DateStatChge].Width = 110;
                            tabStyle.GridColumnStyles[CN.DateStatChge].HeaderText = GetResource("T_DATESTATCHANGED");
                            tabStyle.GridColumnStyles[CN.StatusCode].Width = 40;
                            tabStyle.GridColumnStyles[CN.StatusCode].HeaderText = GetResource("T_STATUS");
                            tabStyle.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "LoadLetters");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnRefresh_Click(object sender, System.EventArgs e)
        {
            try
            {
                Audit = null;

                if (tcMain.SelectedTab == tpAgreementChanges)
                    LoadAgreementChanges();

                tcAddress.TabPages.Clear();
                AccountDetails_Load(null, null);
            }
            catch (Exception ex)
            {
                Catch(ex, "btnRefresh_Click");
            }
        }

        private void LoadAllocationDetails()
        {
            int empNo = 0;
            string empType = String.Empty;
            string empName = String.Empty;
            txtActionValue.Text = (0).ToString(DecimalPlaces);

            try
            {
                Wait();

                //KEF added to set dtDueDate to today
                this.dtDueDate.Value = DateTime.Now;

                AccountManager.GetAllocatedCourtsPerson(accountNo, ref empNo, ref empType,
                    ref empName, out Error);

                if (Error.Length > 0)
                {
                    ShowError(Error);
                }
                else
                {
                    if (empNo == -1)
                        txtEmployeeNo.Text = "";
                    else
                        txtEmployeeNo.Text = empNo.ToString();

                    txtEmployeeType.Text = empType;
                    txtEmployeeName.Text = empName;
                }

                DataSet ds = AccountManager.GetBailActions(accountNo, out Error);

                if (Error.Length > 0)
                {
                    ShowError(Error);
                }
                else
                {
                    dgActions.DataSource = null; //KEF added for 67656 to prevent error message and red cross over datagrid
                    dgActions.DataSource = ds.Tables["BailActions"].DefaultView;
                    if (dgActions.TableStyles.Count == 0)
                    {
                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        tabStyle.MappingName = ds.Tables["BailActions"].TableName;
                        dgActions.TableStyles.Add(tabStyle);

                        tabStyle.GridColumnStyles["AllocNo"].Width = 0;
                        tabStyle.GridColumnStyles["AmtCommPaidOn"].Width = 0;
                        tabStyle.GridColumnStyles["Notes"].Width = 0;
                        tabStyle.GridColumnStyles["Acctno"].Width = 0;
                        tabStyle.GridColumnStyles["CommissionDays"].Width = 0;

                        tabStyle.GridColumnStyles["ActionNo"].Width = 60;
                        tabStyle.GridColumnStyles["ActionNo"].HeaderText = GetResource("T_ACTIONNO");

                        tabStyle.GridColumnStyles["EmpeeNo"].Width = 108;
                        tabStyle.GridColumnStyles["EmpeeNo"].HeaderText = GetResource("T_ALLOCATEDEMPLOYEE");

                        tabStyle.GridColumnStyles["DateAdded"].Width = 100;  // 68268 RD 06/06/06
                        tabStyle.GridColumnStyles["DateAdded"].HeaderText = GetResource("T_DATEADDED");
                        ((DataGridTextBoxColumn)tabStyle.GridColumnStyles["DateAdded"]).Format = "dd/MM/yyyy HH:mm";

                        tabStyle.GridColumnStyles["Code"].Width = 40;
                        tabStyle.GridColumnStyles["Code"].HeaderText = GetResource("T_CODE");

                        tabStyle.GridColumnStyles["ActionValue"].Width = 80;
                        tabStyle.GridColumnStyles["ActionValue"].Alignment = HorizontalAlignment.Right;
                        tabStyle.GridColumnStyles["ActionValue"].HeaderText = GetResource("T_ACTIONVALUE");
                        ((DataGridTextBoxColumn)tabStyle.GridColumnStyles["ActionValue"]).Format = DecimalPlaces;

                        tabStyle.GridColumnStyles["DateDue"].Width = 70;
                        tabStyle.GridColumnStyles["DateDue"].HeaderText = GetResource("T_DATEDUE");

                        tabStyle.GridColumnStyles["AddedBy"].Width = 60;
                        tabStyle.GridColumnStyles["AddedBy"].HeaderText = GetResource("T_ADDEDBY");

                        tabStyle.Dispose();

                        dgActions_MouseUp(this, null);
                    }

                    foreach (DataRow row in ds.Tables["BailActions"].Rows)
                    {
                        if (row["DateDue"].ToString().IndexOf("1900") > 0)
                            row["DateDue"] = string.Empty;
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


        private void LoadAllocationHistory()
        {
            try
            {
                Wait();
                DataSet ds = AccountManager.GetFollowUpHistory(accountNo, out Error);

                if (Error.Length > 0)
                {
                    ShowError(Error);
                    return;
                }

                DataTable dtAllocHist = ds.Tables["AllocHistory"];
                dgAllocationHist.DataSource = dtAllocHist.DefaultView;
                if (dgAllocationHist.TableStyles.Count == 0)
                {
                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                    tabStyle.MappingName = dtAllocHist.TableName;
                    dgAllocationHist.TableStyles.Add(tabStyle);

                    tabStyle.GridColumnStyles[CN.AllocNo].Width = 65;
                    tabStyle.GridColumnStyles[CN.AllocNo].HeaderText = GetResource("T_ALLOCNO");

                    tabStyle.GridColumnStyles[CN.EmployeeName].Width = 155;
                    tabStyle.GridColumnStyles[CN.EmployeeName].HeaderText = GetResource("T_ALLOCATEDEMPLOYEE");

                    tabStyle.GridColumnStyles[CN.DateAlloc].Width = 80;
                    tabStyle.GridColumnStyles[CN.DateAlloc].HeaderText = GetResource("T_DATEALLOC");

                    tabStyle.GridColumnStyles[CN.DateDealloc].Width = 90;
                    tabStyle.GridColumnStyles[CN.DateDealloc].HeaderText = GetResource("T_DATEDEALLOC");

                    tabStyle.GridColumnStyles[CN.AllocArrears].Width = 60;
                    tabStyle.GridColumnStyles[CN.AllocArrears].HeaderText = GetResource("T_ARREARS") + "  ";
                    tabStyle.GridColumnStyles[CN.AllocArrears].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.AllocArrears]).Format = DecimalPlaces;

                    tabStyle.GridColumnStyles[CN.BailFee].Width = 0;
                    tabStyle.GridColumnStyles[CN.AllocPrtFlag].Width = 0;

                    tabStyle.GridColumnStyles[CN.AllocEmpeeName].Width = 155;
                    tabStyle.GridColumnStyles[CN.AllocEmpeeName].HeaderText = GetResource("T_ALLOCATEDBY");

                    tabStyle.GridColumnStyles[CN.DeallocEmpeeName].Width = 155;
                    tabStyle.GridColumnStyles[CN.DeallocEmpeeName].HeaderText = GetResource("T_DEALLOCATEDBY");

                    ((DataView)dgAllocationHist.DataSource).Sort = CN.AllocNo + " DESC";

                    tabStyle.Dispose();
                }

                foreach (DataRow row in dtAllocHist.Rows)
                {
                    if (row[CN.DateAlloc].ToString().IndexOf("1900") > 0)
                        row[CN.DateAlloc] = string.Empty;

                    if (row[CN.DateDealloc].ToString().IndexOf("1900") > 0)
                        row[CN.DateDealloc] = string.Empty;

                    if (row[CN.DateDealloc].ToString() == string.Empty || row[CN.DateDealloc].ToString() == " ")
                        row[CN.DeallocEmpeeName] = string.Empty;
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

        private void LoadStrategiesandWorklists()
        {
            try
            {
                Function = "LoadStrategiesandWorklists";
                Wait();

                // Load account status
                //IP - 26/08/09 - UAT(819) - Added strategyHasWorklists to check if the current strategy the account is in has worklists.
                DataSet Strategies = AccountManager.LoadCollectionsByacctno(accountNo, out strategyHasWorklists, out Error);

                if (Error.Length > 0)
                {
                    ShowError(Error);
                }
                else
                {
                    StrategiesView = new DataView(Strategies.Tables[0]);

                    if (StrategiesView.Count > 0)		//make sure we have some records
                    {
                        //Set the datasource of the datagrid that is a member variable of
                        //the transactionsTab object (set up the column styles also)
                        dgStrategies.DataSource = StrategiesView;

                        if (dgStrategies.TableStyles.Count == 0)
                        {
                            DataGridTableStyle tabStyle = new DataGridTableStyle();
                            tabStyle.MappingName = StrategiesView.Table.TableName;

                            dgStrategies.TableStyles.Add(tabStyle);

                            //UAT1002 jec-  format grid 
                            tabStyle.GridColumnStyles[CN.AccountNumber2].Width = 90;
                            tabStyle.GridColumnStyles[CN.AccountNumber2].HeaderText = GetResource("T_ACCTNO");
                            tabStyle.GridColumnStyles[CN.Strategy].Width = 150;
                            tabStyle.GridColumnStyles[CN.Strategy].HeaderText = GetResource("T_STRATEGY");
                            tabStyle.GridColumnStyles["Date into Current Step"].Width = 130;
                            tabStyle.GridColumnStyles["Date To"].NullText = " ";          // #3588 jec 06/01/12

                            tabStyle.Dispose();
                            // Set the table style according to the user's preference
                        }
                    }

                    foreach (DataRowView drv in StrategiesView)
                    {
                        if (drv["Date To"].ToString().Trim() == "")     // LW73081 jec 22/12/10
                        {
                            //IP - 25/08/09 UAT(819) - Select the strategy the account is currently in.
                            //string strategy = drv["Strategy"].ToString();
                            strategy = drv["Strategy"].ToString();
                            string[] stratArr = strategy.Split(' ');
                            strategy = stratArr[0].ToString();

                            break;
                        }
                    }

                    WorklistView = new DataView(Strategies.Tables[1]);

                    if (WorklistView.Count > 0)		//make sure we have some records
                    {
                        //Set the datasource of the datagrid that is a member variable of
                        //the transactionsTab object (set up the column styles also)
                        dgWorklists.DataSource = WorklistView;

                        if (dgWorklists.TableStyles.Count == 0)
                        {
                            DataGridTableStyle tabStyle = new DataGridTableStyle();
                            tabStyle.MappingName = WorklistView.Table.TableName;
                            dgWorklists.TableStyles.Add(tabStyle);

                            //UAT1002 jec-  format grid 
                            tabStyle.GridColumnStyles[CN.AccountNumber2].Width = 90;
                            tabStyle.GridColumnStyles[CN.AccountNumber2].HeaderText = GetResource("T_ACCTNO");
                            tabStyle.GridColumnStyles[CN.Strategy].Width = 150;
                            tabStyle.GridColumnStyles[CN.Strategy].HeaderText = GetResource("T_STRATEGY");
                            tabStyle.GridColumnStyles[CN.DateFrom].Width = 90;
                            tabStyle.GridColumnStyles[CN.DateFrom].HeaderText = GetResource("T_DATEFROM");
                            tabStyle.GridColumnStyles[CN.DateTo].Width = 90;
                            tabStyle.GridColumnStyles[CN.DateTo].HeaderText = GetResource("T_DATETO");
                            tabStyle.GridColumnStyles[CN.DateTo].NullText = " ";          // #3588 jec 06/01/12

                            tabStyle.Dispose();

                            // Set the table style according to the user's preference
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
                Function = "End of LoadStrategiesandWorklists";
            }
        }

        private void LoadRebate(string accountNumber)
        {
            Function = "LoadRebate";

            try
            {
                Wait();

                if (payments != null)
                {
                    foreach (DataTable dt in payments.Tables)
                    {
                        switch (dt.TableName)
                        {
                            case TN.Accounts:
                                ProcessRebate(dt);
                                break;
                            default:
                                break;
                        }
                    }
                    Function = "End of LoadRebate";

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

        private void LoadAmortizationSchedule()
        {
            DataView AmortizationScheduleView = new DataView(agreement.Tables[TN.AmortizedSchedule]);
            if (AmortizationScheduleView.Count > 0)
            {
                dgAmortizationSchedule.DataSource = AmortizationScheduleView;
                if (dgAmortizationSchedule.TableStyles.Count == 0)
                {
                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                    tabStyle.MappingName = AmortizationScheduleView.Table.TableName;
                    dgAmortizationSchedule.TableStyles.Add(tabStyle);

                    tabStyle.GridColumnStyles[CN.PaymentNum].Width = 90;
                    tabStyle.GridColumnStyles[CN.PaymentNum].HeaderText = GetResource("T_PaymentNum");
                    tabStyle.GridColumnStyles[CN.PaymentNum].Alignment = HorizontalAlignment.Right;

                    tabStyle.GridColumnStyles[CN.PaymentDate].Width = 120;
                    tabStyle.GridColumnStyles[CN.PaymentDate].HeaderText = GetResource("T_PaymentDate");
                    tabStyle.GridColumnStyles[CN.PaymentDate].Alignment = HorizontalAlignment.Right;

                    tabStyle.GridColumnStyles[CN.BeginningBalance].Width = 120;
                    tabStyle.GridColumnStyles[CN.BeginningBalance].HeaderText = GetResource("T_BeginningBalance");
                    tabStyle.GridColumnStyles[CN.BeginningBalance].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.BeginningBalance]).Format = DecimalPlaces;

                    tabStyle.GridColumnStyles[CN.InstalAmt].Width = 120;
                    tabStyle.GridColumnStyles[CN.InstalAmt].HeaderText = GetResource("T_PaymentAmt");
                    tabStyle.GridColumnStyles[CN.InstalAmt].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.InstalAmt]).Format = DecimalPlaces;

                    tabStyle.GridColumnStyles[CN.Principal].Width = 120;
                    tabStyle.GridColumnStyles[CN.Principal].HeaderText = GetResource("T_Principal");
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Principal]).Format = DecimalPlaces;
                    tabStyle.GridColumnStyles[CN.Principal].Alignment = HorizontalAlignment.Right;

                    tabStyle.GridColumnStyles[CN.AmortizedInterest].Width = 120;
                    tabStyle.GridColumnStyles[CN.AmortizedInterest].HeaderText = GetResource("T_Interest");
                    tabStyle.GridColumnStyles[CN.AmortizedInterest].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.AmortizedInterest]).Format = DecimalPlaces;

                    tabStyle.GridColumnStyles[CN.EndingBalance].Width = 120;
                    tabStyle.GridColumnStyles[CN.EndingBalance].HeaderText = GetResource("T_EndingBalance");
                    tabStyle.GridColumnStyles[CN.EndingBalance].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.EndingBalance]).Format = DecimalPlaces;

                    tabStyle.GridColumnStyles[CN.LatePmtFee].Width = 120;
                    tabStyle.GridColumnStyles[CN.LatePmtFee].HeaderText = GetResource("T_LatePmtFee");
                    tabStyle.GridColumnStyles[CN.LatePmtFee].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.LatePmtFee]).Format = DecimalPlaces;

                    tabStyle.GridColumnStyles[CN.PenaltyFee].Width = 120;
                    tabStyle.GridColumnStyles[CN.PenaltyFee].HeaderText = GetResource("T_PenaltyFee");
                    tabStyle.GridColumnStyles[CN.PenaltyFee].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.PenaltyFee]).Format = DecimalPlaces;

                    tabStyle.GridColumnStyles[CN.AdminFees].Width = 120;
                    tabStyle.GridColumnStyles[CN.AdminFees].HeaderText = GetResource("T_AdminFee");
                    tabStyle.GridColumnStyles[CN.AdminFees].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.AdminFees]).Format = DecimalPlaces;
                    tabStyle.GridColumnStyles[CN.TotalInstalment].Width = 120;

                    tabStyle.GridColumnStyles[CN.TotalInstalment].HeaderText = GetResource("T_TotalInstalment");

                    tabStyle.GridColumnStyles[CN.TotalInstalment].Alignment = HorizontalAlignment.Right;

                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TotalInstalment]).Format = DecimalPlaces;

                    tabStyle.Dispose();
                }
            }
        }
        private void ProcessMambuDetails()
        {
            Function = "ProcessMambuDetails";

            try
            {
                Wait();

                if (payments != null)
                {
                    foreach (DataTable dt in payments.Tables)
                    {
                        switch (dt.TableName)
                        {
                            case TN.Accounts:
                                LoadMambuDetails(dt);
                                break;
                            default:
                                break;
                        }
                    }
                    Function = "End of ProcessMambuDetails";

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

        private void LoadMambuDetails(DataTable dt)
        {

            decimal totRebate = 0;
            decimal totSettlement = 0;
            decimal totFee = 0;
            foreach (DataRow row in dt.Rows)
            {

                if ((bool)row[CN.IsMambuAccount] == true && (string)row[CN.acctno] == accountNo)
                {
                    if (DBNull.Value != row[CN.Rebate])
                        totRebate += Convert.ToDecimal(row[CN.Rebate]);
                    if (DBNull.Value != row[CN.SettlementFigure])
                        totSettlement += Convert.ToDecimal(row[CN.SettlementFigure]);
                    if (DBNull.Value != row[CN.CollectionFee])
                        totFee += Convert.ToDecimal(row[CN.CollectionFee]);
                }

            }

            txtRebate.Text = totRebate.ToString(DecimalPlaces);
            //txtTotalFees.Text = (MoneyStrToDecimal(txtTotalFees.Text) +  totFee).ToString(DecimalPlaces);
            txtEarlySettlementFigure.Text = (totSettlement + totFee - totRebate).ToString(DecimalPlaces);
        }
        private void ProcessRebate(DataTable dt)
        {

            decimal totRebate = 0;
            decimal totSettlement = 0;
            foreach (DataRow row in dt.Rows)
            {

                if ((decimal)row[CN.OutstandingBalance] >= 0.01M
                    && row[CN.DeliveryFlag].ToString() == "Y"
                    && (string)row[CN.acctno] == accountNo)
                {
                    if (DBNull.Value != row[CN.Rebate])
                        totRebate += Convert.ToDecimal(row[CN.Rebate]);
                    if (DBNull.Value != row[CN.SettlementFigure])
                        totSettlement += Convert.ToDecimal(row[CN.SettlementFigure]);
                }

            }

            txtRebate.Text = totRebate.ToString(DecimalPlaces);
            txtEarlySettlementFigure.Text = totSettlement.ToString(DecimalPlaces);
            txtAddToPotential.Text = addTo.ToString(DecimalPlaces);
        }

        private void LoadEarlySettlementFig(string accountNumber)
        {
            Function = "LoadEarlySettlementFig";
            try
            {
                Wait();

                decimal settlementFig = PaymentManager.GetEarlySettlementFig(accountNumber);
                Function = "End of LoadEarlySettlementFig";
                txtEarlySettlementFigure.Text = settlementFig.ToString(DecimalPlaces);
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
                    errorProvider1.SetError(txtActionValue, String.Empty);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnTotal_Click");
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
                ValidateDueDate();
                //If no errors .. do the update..
                if (errorProvider1.GetError(dtDueDate).Length == 0
                    && errorProvider1.GetError(txtActionValue).Length == 0)
                {
                    //if (drpActionCode.SelectedIndex < 1)
                    if (drpActionCode.SelectedIndex < 0)    //IP - 18/05/10 - UAT(4) UAT5.2 External Log
                        errorProvider1.SetError(drpActionCode, GetResource("M_SELECTACTIONCODE"));
                    else
                    {
                        errorProvider1.SetError(drpActionCode, String.Empty);

                        int empeeNo = 0;

                        if (txtEmployeeNo.Text.Length > 0 && IsNumeric(txtEmployeeNo.Text))
                            empeeNo = Convert.ToInt32(txtEmployeeNo.Text);
                        else
                            empeeNo = Credential.UserId;

                        DateTime spaDateExpiry = DateTime.Now;
                        string spaReasonCode = string.Empty;
                        double spaInstal = 0;

                        if (SPASelected)
                        {
                            spaDateExpiry = dtDueDate.Value;
                            spaReasonCode = drpReason.SelectedValue.ToString();
                            spaInstal = Convert.ToDouble(StripCurrency(txtActionValue.Text));
                        }

                        AccountManager.SaveBailActions(accountNo,
                            empeeNo,
                            CurrentActionCode,
                            txtNotes.Text,
                            dtDueDate.Value,
                            Convert.ToDouble(StripCurrency(txtActionValue.Text)),
                            spaDateExpiry,//These values are ignored by the Stored
                            spaReasonCode,//Procedure if the Action's code value
                            spaInstal,    //is not 'SPA'
                            new DateTime(1900, 1, 1), //Will be needed only for REM (reminder) action
                            false,  //Will be necessary only Telephone Action Screen
                            "",  //Will be necessary only for Telephone Action Screen
                            out Error);

                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            txtNotes.Text = String.Empty;
                            drpActionCode.SelectedIndex = 0;
                            txtActionValue.Text = (0).ToString(DecimalPlaces);
                            tcMain_SelectionChanged(null, null);
                            lbReason.Visible = false;
                            drpReason.Visible = false;
                            lbActionDate.Text = "Due Date";
                            lbActionValue.Text = "Action Value";
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
            }
        }

        private void dgActions_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                int index = dgActions.CurrentRowIndex;

                if (index >= 0)
                {
                    DataView dv = (DataView)dgActions.DataSource;

                    if (dv[index]["Notes"] == DBNull.Value)
                        textNotes.Text = "";
                    else
                        textNotes.Text = (string)dv[index]["Notes"];
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void CalcPercentagePaid()
        {
            bool status = true;
            bool addTo = false;
            decimal percentagePaid = 0;
            //tpTransactions.Control = new Transactions();

            /* calculate charges which would deflate the % paid  */

            decimal charges = MoneyStrToDecimal(((Transactions)tpTransactions.Control).txtTotalAdmin.Text) +
                                MoneyStrToDecimal(((Transactions)tpTransactions.Control).txtTotalInterest.Text) +
                                MoneyStrToDecimal(((Transactions)tpTransactions.Control).txtTotalBailFees.Text);

            decimal outStBal = MoneyStrToDecimal(txtOutstandingBalance.Text);
            decimal agrmtTotal = MoneyStrToDecimal(txtAgreementTotal.Text);
            decimal delTotal = MoneyStrToDecimal(txtDeliveryTotal.Text);
            decimal payTotal = MoneyStrToDecimal(txtAmountPaid.Text);

            if (dgLineItems.DataSource != null)
            {
                foreach (DataRowView row in (DataView)dgLineItems.DataSource)
                {
                    if ((string)row["ProductCode"] == "ADDDR")
                        addTo = true;
                }
            }

            if (outStBal > 0 && agrmtTotal > 0 && delTotal >= agrmtTotal)
            {
                percentagePaid = ((agrmtTotal - (outStBal + charges)) / agrmtTotal) * 100;      // jec 20/06/08 correct calculation

                if (percentagePaid > 100)
                    percentagePaid = 100;
                else if (percentagePaid < 0)
                    percentagePaid = 0;
            }
            else
            {
                if (outStBal == 0 && delTotal >= agrmtTotal)
                {
                    percentagePaid = 100;
                    status = false;
                }

                if (delTotal == 0 && outStBal <= 0 && agrmtTotal != 0 && status)
                {
                    percentagePaid = (-outStBal + charges) / agrmtTotal * 100;
                    /* KEF 66828 outstbal is -ve so making +ve so % comes out +ve*/
                    status = false;
                }

                // is a delivery, but paidpcent cannot be calc if
                // ad-to as paytot does not include previous
                // accounts payments
                if (delTotal > 0 && status)
                {
                    if (addTo)
                        percentagePaid = 0;
                    else
                        percentagePaid = ((-payTotal + charges) / delTotal) * 100;
                }
            }

            txtPercentagePaid.Text = Convert.ToInt32(percentagePaid).ToString();
        }

        private void LoadPendingCharges()
        {
            try
            {
                Wait();
                DataSet ds = AccountManager.GetChargesByAcctNo(accountNo, out Error);

                if (Error.Length > 0)
                {
                    ShowError(Error);
                    return;
                }

                DataTable dt = ds.Tables[TN.AccountDetails];
                dgPendingCharges.DataSource = dt.DefaultView;
                dt.DefaultView.AllowNew = false;

                if (dgPendingCharges.TableStyles.Count == 0)
                {
                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                    tabStyle.MappingName = dt.TableName;
                    dgPendingCharges.TableStyles.Add(tabStyle);

                    tabStyle.GridColumnStyles[CN.acctno].Width = 0;

                    tabStyle.GridColumnStyles[CN.Arrears.ToLower()].Width = 70;
                    tabStyle.GridColumnStyles[CN.Arrears.ToLower()].HeaderText = GetResource("T_ARREARS") + ":";
                    tabStyle.GridColumnStyles[CN.Arrears.ToLower()].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Arrears.ToLower()]).Format = DecimalPlaces;

                    tabStyle.GridColumnStyles[CN.Mpr].Width = 50;
                    tabStyle.GridColumnStyles[CN.Mpr].HeaderText = GetResource("T_PERCENTAGERATE");

                    tabStyle.GridColumnStyles[CN.DateFinish].Width = 80;
                    tabStyle.GridColumnStyles[CN.DateFinish].HeaderText = GetResource("T_DATERUN");

                    tabStyle.GridColumnStyles[CN.Runno].Width = 50;
                    tabStyle.GridColumnStyles[CN.Runno].HeaderText = GetResource("T_WEEKNO");

                    tabStyle.GridColumnStyles[CN.DateNextDue].Width = 130;
                    tabStyle.GridColumnStyles[CN.DateNextDue].HeaderText = GetResource("T_DATENEXTDUE");

                    tabStyle.GridColumnStyles[CN.IntChargesDue].Width = 140;
                    tabStyle.GridColumnStyles[CN.IntChargesDue].HeaderText = GetResource("T_INTCHARGESDUE") + ":";
                    tabStyle.GridColumnStyles[CN.IntChargesDue].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.IntChargesDue]).Format = DecimalPlaces;

                    tabStyle.GridColumnStyles[CN.IntChargesCumul].Width = 120;
                    tabStyle.GridColumnStyles[CN.IntChargesCumul].HeaderText = GetResource("T_INTCHARGESCUMUL") + ":";
                    tabStyle.GridColumnStyles[CN.IntChargesCumul].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.IntChargesCumul]).Format = DecimalPlaces;

                    tabStyle.GridColumnStyles[CN.IntChargesApplied].Width = 158;
                    tabStyle.GridColumnStyles[CN.IntChargesApplied].HeaderText = GetResource("T_ACCUMCHARGESAPPLIED") + ":";
                    tabStyle.GridColumnStyles[CN.IntChargesApplied].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.IntChargesApplied]).Format = DecimalPlaces;

                    ((DataView)dgPendingCharges.DataSource).Sort = CN.DateFinish + " DESC";

                    tabStyle.Dispose();
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

        private void LoadArrearsCharges()
        {
            try
            {
                Wait();
                DataSet ds = AccountManager.GetArrearsDailyByAcctNo(accountNo, out Error);

                if (Error.Length > 0)
                {
                    ShowError(Error);
                }
                else
                {
                    DataTable dt = ds.Tables[TN.AccountDetails];

                    dgArrearsCharges.DataSource = dt.DefaultView;
                    dt.DefaultView.AllowNew = false;
                    if (dgArrearsCharges.TableStyles.Count == 0)
                    {
                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        tabStyle.MappingName = dt.TableName;
                        dgArrearsCharges.TableStyles.Add(tabStyle);

                        tabStyle.GridColumnStyles[CN.Arrears.ToLower()].Width = 90;
                        tabStyle.GridColumnStyles[CN.Arrears.ToLower()].HeaderText = GetResource("T_ARREARS") + ":";
                        tabStyle.GridColumnStyles[CN.Arrears.ToLower()].Alignment = HorizontalAlignment.Right;
                        ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Arrears.ToLower()]).Format = DecimalPlaces;

                        tabStyle.GridColumnStyles[CN.DateFrom.ToLower()].Width = 70;
                        tabStyle.GridColumnStyles[CN.DateFrom.ToLower()].HeaderText = GetResource("T_DATEFROM");

                        tabStyle.GridColumnStyles[CN.DateTo.ToLower()].Width = 70;
                        tabStyle.GridColumnStyles[CN.DateTo.ToLower()].HeaderText = GetResource("T_DATETO");

                        ((DataView)dgArrearsCharges.DataSource).Sort = CN.DateTo.ToLower() + " DESC";

                        tabStyle.Dispose();
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

        private void LoadActivitySegments()
        {
            DataSet ds = null;
            ds = AccountManager.GetActivitySegments(accountNo, out Error);

            if (Error.Length > 0)
            {
                ShowError(Error);
                return;
            }

            foreach (DataTable dt in ds.Tables)
            {
                switch (dt.TableName)
                {
                    case TN.Segments:
                        dgSegment.DataSource = dt.DefaultView;
                        if (dgSegment.TableStyles.Count == 0)
                        {
                            DataGridTableStyle tabStyle = new DataGridTableStyle();
                            tabStyle.MappingName = TN.Segments;

                            dgSegment.TableStyles.Add(tabStyle);

                            tabStyle.GridColumnStyles[CN.SegmentID].Width = 70;
                            tabStyle.GridColumnStyles[CN.SegmentID].HeaderText = GetResource("T_SEGMENTID");

                            tabStyle.GridColumnStyles[CN.SegmentName].Width = 120;
                            tabStyle.GridColumnStyles[CN.SegmentName].HeaderText = GetResource("T_NAME");

                            tabStyle.GridColumnStyles[CN.SegmentUser].Width = 60;
                            tabStyle.GridColumnStyles[CN.SegmentUser].HeaderText = GetResource("T_SEGMENTUSER");

                            tabStyle.GridColumnStyles[CN.SegmentDate].Width = 60;
                            tabStyle.GridColumnStyles[CN.SegmentDate].HeaderText = GetResource("T_DATE");

                            // RD & DR 25/11/04 CR721 To hide tallyman percentage rate
                            tabStyle.GridColumnStyles[CN.TMPercentage].Width = 0;

                            tabStyle.Dispose();
                        }
                        break;
                    case TN.Activities:
                        dgActivities.DataSource = dt.DefaultView;
                        if (dgActivities.TableStyles.Count == 0)
                        {
                            DataGridTableStyle tabStyle = new DataGridTableStyle();
                            tabStyle.MappingName = TN.Activities;

                            dgActivities.TableStyles.Add(tabStyle);

                            tabStyle.GridColumnStyles[CN.InstallmentAmount].Width = 0;
                            tabStyle.GridColumnStyles[CN.InstallmentDueDate].Width = 0;
                            tabStyle.GridColumnStyles[CN.InstallmentPaidAmount].Width = 0;

                            tabStyle.GridColumnStyles[CN.ActivityID].Width = 70;
                            tabStyle.GridColumnStyles[CN.ActivityID].HeaderText = GetResource("T_ACTIVITYID");

                            tabStyle.GridColumnStyles[CN.ActivityName].Width = 120;
                            tabStyle.GridColumnStyles[CN.ActivityName].HeaderText = GetResource("T_NAME");

                            tabStyle.GridColumnStyles[CN.SegmentUser].Width = 60;
                            tabStyle.GridColumnStyles[CN.SegmentUser].HeaderText = GetResource("T_SEGMENTUSER");

                            tabStyle.GridColumnStyles[CN.SegmentDate].Width = 60;
                            tabStyle.GridColumnStyles[CN.SegmentDate].HeaderText = GetResource("T_DATE");

                            tabStyle.Dispose();
                        }
                        break;
                    case TN.Arrangements:
                        dgArrangements.DataSource = dt.DefaultView;
                        if (dgArrangements.TableStyles.Count == 0)
                        {
                            DataGridTableStyle tabStyle = new DataGridTableStyle();
                            tabStyle.MappingName = TN.Arrangements;

                            dgArrangements.TableStyles.Add(tabStyle);

                            tabStyle.GridColumnStyles[CN.ActivityID].Width = 70;
                            tabStyle.GridColumnStyles[CN.ActivityID].HeaderText = GetResource("T_ACTIVITYID");

                            tabStyle.GridColumnStyles[CN.ActivityName].Width = 120;
                            tabStyle.GridColumnStyles[CN.ActivityName].HeaderText = GetResource("T_NAME");

                            tabStyle.GridColumnStyles[CN.SegmentUser].Width = 60;
                            tabStyle.GridColumnStyles[CN.SegmentUser].HeaderText = GetResource("T_SEGMENTUSER");

                            tabStyle.GridColumnStyles[CN.SegmentDate].Width = 60;
                            tabStyle.GridColumnStyles[CN.SegmentDate].HeaderText = GetResource("T_DATE");

                            tabStyle.GridColumnStyles[CN.InstallmentAmount].Width = 60;
                            tabStyle.GridColumnStyles[CN.InstallmentAmount].HeaderText = GetResource("T_AMOUNT");
                            tabStyle.GridColumnStyles[CN.InstallmentAmount].Alignment = HorizontalAlignment.Right;
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.InstallmentAmount]).Format = DecimalPlaces;

                            tabStyle.GridColumnStyles[CN.InstallmentDueDate].Width = 60;
                            tabStyle.GridColumnStyles[CN.InstallmentDueDate].HeaderText = GetResource("T_ARRANGEMENTDATE");

                            tabStyle.GridColumnStyles[CN.InstallmentPaidAmount].Width = 60;
                            tabStyle.GridColumnStyles[CN.InstallmentPaidAmount].HeaderText = GetResource("T_PAIDAMOUNT");
                            tabStyle.GridColumnStyles[CN.InstallmentPaidAmount].Alignment = HorizontalAlignment.Right;
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.InstallmentPaidAmount]).Format = DecimalPlaces;

                            tabStyle.Dispose();
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void ResetFields()
        {
            txtIAmount.Text = (0).ToString(DecimalPlaces);
            txtLastInstalmentAmount.Text = (0).ToString(DecimalPlaces);
            txtInstalmentTotal.Text = (0).ToString(DecimalPlaces);
            txtSChargeAmount.Text = (0).ToString(DecimalPlaces);
            txtToFollowAmount.Text = (0).ToString(DecimalPlaces);
        }

        private void btnSPADetails_Click(object sender, System.EventArgs e)
        {
            try
            {
                using (SPADetails spa = new SPADetails(txtAccountNo.Text))
                {
                    spa.FormRoot = this.FormRoot;
                    spa.FormParent = this;
                    spa.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSPADetails_Click");
            }
        }

        private void drpActionCode_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            string code = string.Empty;
            DateTime spaDateExpiry = DateTime.Now;                                              //IP - 01/06/10 - UAT(1006) UAT5.2 Log
            string spaReasonCode = string.Empty;                                                //IP - 01/06/10 - UAT(1006) UAT5.2 Log
            double spaInstal = 0;                                                               //IP - 01/06/10 - UAT(1006) UAT5.2 Log
            DateTime dueDate = dtDueDate.Value;                                                 //IP - 01/06/10 - UAT(1006) UAT5.2 Log
            string notes = txtNotes.Text;                                                       //IP - 01/06/10 - UAT(1006) UAT5.2 Log
            double actionValue = Convert.ToDouble(StripCurrency(txtActionValue.Text));          //IP - 01/06/10 - UAT(1006) UAT5.2 Log
            DateTime reminderDateTime = new DateTime(1900, 1, 1);                               //IP - 01/06/10 - UAT(1006) UAT5.2 Log
            bool cancelOutstandingReminders = false;                                            //IP - 01/06/10 - UAT(1006) UAT5.2 Log
            errorProvider1.SetError(drpActionCode, "");
            btnSave.Enabled = true;                // #9824

            if (drpActionCode.SelectedIndex >= 0)
            {
                if (SPASelected)
                {
                    if (accountNo.Substring(3, 1) == "9")            // #9824 not allowed on storecard
                    {
                        errorProvider1.SetError(drpActionCode, "Special Arrangements cannot be made on StoreCard accounts");
                        btnSave.Enabled = false;
                    }
                    else
                    {
                        lbActionDate.Text = "Expiry Date";
                        lbActionValue.Text = "Instalment";

                        //IP - 01/06/10 - UAT(1006) UAT5.2 Log
                        SpecialArrangements sparr = new SpecialArrangements(accountNo, customerID, txtLastName.Text, creditBlocked, (DataTable)StaticData.Tables[TN.Reasons]);
                        sparr.FormRoot = this.FormRoot;
                        sparr.FormParent = this;
                        sparr.ShowDialog(this);

                        dtSPADetails = sparr.dtSPADetails;

                        if (sparr.AcceptBtn == true)
                        {
                            spaDateExpiry = Convert.ToDateTime(dtSPADetails.Rows[0][CN.FinalPayDate]);
                            spaReasonCode = Convert.ToString(dtSPADetails.Rows[0][CN.ReasonCode]);
                            spaInstal = Convert.ToDouble(StripCurrency(txtActionValue.Text));
                            dueDate = Convert.ToDateTime(dtSPADetails.Rows[0][CN.ReviewDate]);

                            if (Convert.ToString(dtSPADetails.Rows[0][CN.ExtendTerm]) == "True")
                            {
                                notes = "(Refinance) - " + notes;
                                CollectionsManager.SPAWriteRefinance(accountNo, Credential.UserId, CurrentActionCode, notes,
                                dueDate, actionValue, spaDateExpiry, spaReasonCode, spaInstal, reminderDateTime, cancelOutstandingReminders, dtSPADetails, "ACCTDETAILS", out Error);

                                // TO DO
                                string NewAccount = "";
                                double NewAcctNo = 0;
                                NewAccount = Convert.ToString(accountNo);
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
                                CollectionsManager.SaveBailActionsForSPA(accountNo, Credential.UserId, CurrentActionCode, notes,
                                    dueDate, actionValue, spaDateExpiry, spaReasonCode, spaInstal, reminderDateTime, cancelOutstandingReminders, dtSPADetails, "ACCTDETAILS", out Error);
                            }
                        }
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
            lbReason.Visible = SPASelected;
            drpReason.Visible = SPASelected;
        }

        private void ValidateDueDate()
        {
            if (SPASelected && dtDueDate.Value <= DateTime.Today)  // RD 06/06/06 need to change this as getting error
            {
                errorProvider1.SetError(dtDueDate, GetResource("M_EXPIRYPAST"));
                dtDueDate.Focus();
            }
            else
            {
                errorProvider1.SetError(dtDueDate, string.Empty);
            }
        }

        private void menuStatement_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                PrintStatementOfAccount(((CommonForm)FormRoot).CreateBrowserArray(1)[0], accountNo, txtCustID.Text, txtAccountType.Text);
            }
            catch (Exception ex)
            {
                Catch(ex, "menuStatement_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void LoadGiro()
        {
            try
            {
                Wait();

                decimal pending = AccountManager.GetGiroPending(accountNo, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                // removed jec - Malaysia 4.3 merge Note: Giro not used need location for Segment
                //else
                //    txtGiroPending.Text = pending.ToString(DecimalPlaces);
            }
            catch (Exception ex)
            {
                Catch(ex, "LoadGiro");
            }
            finally
            {
                StopWait();
            }
        }

        //private void btnWarrantyCollections_Click(object sender, EventArgs e)
        //{
        //    //This code removed as of walk through for CR 822
        //    //if (this._ShowAccountTransactions)
        //    //    LoadCollectionReasons(this.transactions.Tables[TN.WarrantyCollectionReason]);
        //    //else 
        //    //    LoadTransactions(this.transactions.Tables[TN.Transactions]);

        //    //this.ShowAccountTransactions = !this.ShowAccountTransactions;
        //}

        //CR855
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "button1_Click";
                Wait();

                CustomerPhotograph customerPhoto = new CustomerPhotograph(txtCustID.Text.Trim(), fileName, this.FormRoot, this);
                customerPhoto.ShowDialog();

                customerPhoto.Dispose();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of button1_Click";
            }
        }

        private void AccountDetails_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dispose(true);

            //If dispose is called already then say GC to skip finalize on this instance.
            GC.SuppressFinalize(this);
        }

        // uat257 rdb 07/04/08 this is causing the tab to be destryed as it is being added to the tab collection
        // revert to base implementation
        //public override bool ConfirmClose()
        //{
        //    bool status = true;

        //    Dispose(true);

        //    //If dispose is called already then say GC to skip finalize on this instance.

        //    GC.SuppressFinalize(this);
        //    return status;
        //}

        private void LoadInstallationBooking()
        {
            var results = InstallationManager.Search(new STL.PL.WSInstallation.InstSearchParameter
            {
                AcctNo = accountNo
            },
            out Error);

            if (Error.Length > 0)
                ShowError(Error);

            foreach (var result in results)
                result.FormattedStatus = result.Status == STL.PL.WSInstallation.InstStatus.Unknown.ToString() ? "" : result.Status.SplitCamelCase();

            dgvInstallation.DataSource = SortableBindingListFactory.Create(results);

            dgvInstallation
                .ColumnStyleInit()
                .ColumnStyle("InstNo")
                .ColumnStyle("InstDate")
                .ColumnStyle("FormattedStatus", GetResource("T_STATUS"))
                .ColumnStyle("InstValue")
                .ColumnStyle("AgreementNo")
                .ColumnStyle("ItemNo");

            if (Country.GetCountryParameterValue<bool>(CountryParameterNames.RIDispCourtsCode))
                dgvInstallation.ColumnStyle("CourtsCode");

            dgvInstallation
                .ColumnStyle("ProductDescription1", GetResource("T_ITEM_DESCRIPTION"))
                .ColumnStyle("ProductDescription2", GetResource("T_ITEM_DESCRIPTION2"))
                .ColumnStyle("StockLocation")
                .ColumnStyle("DeliveryStatus")
                .ColumnStyle("TechnicianName");

            dgvInstallation.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
        }

        //IP - 20/10/11 - CR1232 - Print agreement documents for Cash Loan and normal accounts only once they have been delivered.
        private void menuAgreementDocs_Click(object sender, EventArgs e)
        {

            itemDoc = new XmlDocument();
            itemDoc.LoadXml("<ITEMS></ITEMS>");

            //retrieve line items for this account
            XmlNode lineItems = AccountManager.GetLineItems(accountNo, AgreementNo, accountType, Config.CountryCode, Convert.ToInt16(Config.BranchCode), out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                //initialise the XML document and the tree view
                if (lineItems != null)
                {
                    lineItems = itemDoc.ImportNode(lineItems, true);
                    itemDoc.ReplaceChild(lineItems, itemDoc.DocumentElement);
                }
            }


            if (!this.IsLoan)
            {
                PrintAgreementDocuments(this.accountNo,
                        this.accountType,
                        this.customerID,
                        this.PaidAndTaken,
                        false,
                        0, 0,
                        itemDoc.DocumentElement,
                        AgreementNo,
                        this,
                        true,
                        Credential.UserId,
                        //Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code]),
                        1,
                        false, false);
            }
            else
            {
                PrintCashLoanDocuments(this.accountNo,
                        this.accountType,
                        this.customerID,
                        this.PaidAndTaken,
                        false,
                        0, 0,
                        itemDoc.DocumentElement,
                        AgreementNo,
                        this,
                        true,
                        Credential.UserId,
                        1);
                //Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code]));
            }
        }
    }
}
