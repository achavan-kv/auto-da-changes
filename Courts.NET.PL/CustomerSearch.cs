using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Crownwood.Magic.Menus;
using STL.Common;
using STL.Common.Constants.AccountHolders;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Static;
//using STL.PL.Installation;
using STL.PL.StoreCard.Common;
using STL.PL.WS1;

namespace STL.PL
{

    // **** NOTE: This screen accesses the columns in the results grid with hard coded
    // **** column numbers. So beware if the columns change!
    //
    /// <summary>
    /// Customer search screen. Allows a search on any combination of the fields
    /// Customer Id, First Name and Last Name. The search on these fields can either 
    /// be an exact match or a wildcard match for fields starting with the entered
    /// characters.
    /// All accounts for the customer are listed, optionally including settled accounts.
    /// This screen can be used to link a customer to a new account. Once a customer has
    /// been found then an 'Associate with Account' option is available. If a customer
    /// cannot be found then navigation to the 'New Customer' screen is available.
    /// </summary>
    public class CustomerSearch : CommonForm
    {
        private System.Windows.Forms.GroupBox grpSearch;
        private System.Windows.Forms.GroupBox grpResults;
        private System.Windows.Forms.TextBox txtCustID;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem cmenuAssociate;
        private System.Windows.Forms.MenuItem cmenuView;
        private System.Windows.Forms.CheckBox chxIncludeSettled;
        private System.Windows.Forms.CheckBox chxLimit;
        private STL.PL.AccountTextBox txtAccountNo;
        private System.Windows.Forms.Label label4;
        private System.ComponentModel.IContainer components;
        private new string Error = "";
        //private bool Associated = false;        //CR1072 Malaysia merge
        private DataSet ds = null;
        private System.Windows.Forms.Button btnEnter;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.MenuItem cmenuAccountDetails;
        private System.Windows.Forms.MenuItem cmenuEdit;
        private DataView dv = null;
        //private DataTable searches = null;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuOptions;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private Crownwood.Magic.Menus.MenuCommand menuSearch;
        private Crownwood.Magic.Menus.MenuCommand menuAssociate;
        private Crownwood.Magic.Menus.MenuCommand menuNewCustomer;
        private Crownwood.Magic.Menus.MenuCommand menuServiceRequest;    //jec 70566
        private string _accountNo = "";
        public string AccountNo
        {
            get { return _accountNo; }
            set { _accountNo = value; }
        }
        private string _accountType = "";
        public string AccountType
        {
            get { return _accountType; }
            set { _accountType = value; }
        }
        private bool _canCreateRF = true;
        public bool CanCreateRF
        {
            get { return _canCreateRF; }
            set { _canCreateRF = value; }
        }
        private string _clickedacct = "";
        public string ClickedAccount
        {
            get { return _clickedacct; }
            set { _clickedacct = value; }
        }

        private string _clickedtype = "";
        private Crownwood.Magic.Menus.MenuCommand menuRFAccount;
        private Crownwood.Magic.Menus.MenuCommand menuCustDetails;
        private System.Windows.Forms.Button btnNewCustomer;

        public string ClickedType
        {
            get { return _clickedtype; }
            set { _clickedtype = value; }
        }
        private string _clickedcust = "";
        public string ClickedCust
        {
            get { return _clickedcust; }
            set { _clickedcust = value; }
        }

        //IP - CR971
        private bool _clickedArchived;
        public bool ClickedArchived
        {
            get { return _clickedArchived; }
            set { _clickedArchived = value; }
        }

        //IP - CR971
        private string _clickedStatus;
        public string ClickedStatus
        {
            get { return _clickedStatus; }
            set { _clickedStatus = value; }
        }

        public SanctionStage1 SanctionScreen = null;
        public string Relationship = "";

        private bool m_associated = false;
        public bool associated
        {
            get
            {
                return m_associated;
            }
            set
            {
                m_associated = value;
            }
        }

        public event RecordIDHandler<StoreCardCustDetails> CustidSelected;

        public string customerID = "";
        private string firstName = "";
        private string lastName = "";
        private string phoneNumber = "";
        private string address = "";
        private string storeType = string.Empty;
        int limit = 0;
        private Crownwood.Magic.Menus.MenuCommand menuRecentCust;
        private Crownwood.Magic.Menus.MenuCommand menuRecentAcct;
        private STL.PL.DataGridCellTips dgResults;
        private System.Windows.Forms.CheckBox chxExact;
        int settled = 0;
        //private MenuCommand menuServiceRequest;        //jec 70566
        private CheckBox chxStore;
        private MenuCommand menuUnarchive;
        private MenuCommand menuUnsettle;
        private bool exactMatch = true;
        private TextBox txtPhoneNo;
        private TextBox txtAddress;
        private Label label6;
        private Label label5;
        private TextBox txtPostCode;
        private Label label7;
        private CheckBox chkPhone;
        private CheckBox chkAddress;
        private bool unsettleAcct = false; //IP - CR971
        private bool AssoicateCustid;

        public CustomerSearch(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuOptions });
            Spouse.Visited = false;     //CR1072 Malaysia merge//jec 70148 29/09/08
        }

        public CustomerSearch()
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuOptions });
            Spouse.Visited = false;     //CR1072 Malaysia merge//jec 70148 29/09/08
            //TranslateControls();			
        }

        public CustomerSearch(bool assoicateCustid)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuOptions });
            Spouse.Visited = false;     //CR1072 Malaysia merge//jec 70148 29/09/08
            AssoicateCustid = assoicateCustid;
        }


        public CustomerSearch(string accountNo, string accountType, bool canCreateRF)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuOptions });
            txtAccountNo.Text = this.AccountNo = accountNo;
            if (accountNo.Length > 0)
            {
                menuAssociate.Enabled = true;
                menuAssociate.Visible = true;
            }
            this.AccountType = accountType;
            this._canCreateRF = canCreateRF;
            Spouse.Visited = false;     //CR1072 Malaysia merge//jec 70148 29/09/08
            //TranslateControls();
        }

        public CustomerSearch(string accountNo, string accountType, SanctionStage1 sanction, string relation)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuOptions });
            this.AccountType = accountType;
            this.SanctionScreen = sanction;
            this.Relationship = relation;
            //TranslateControls();
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
            menuRecentAcct.MenuCommands.Clear();
            foreach (string s in Recent.AccountList)
            {
                Crownwood.Magic.Menus.MenuCommand m = new Crownwood.Magic.Menus.MenuCommand(s);
                menuRecentAcct.MenuCommands.Add(m);
                m.Click += new System.EventHandler(OnRecentAcctClick);
            }
        }

        private void OnRecentCustClick(object sender, System.EventArgs e)
        {
            Crownwood.Magic.Menus.MenuCommand m = (Crownwood.Magic.Menus.MenuCommand)sender;
            string CustomerID = m.Text;
            this._canCreateRF = true; //KEF added as Create RF button was greyed out
            BasicCustomerDetails details = new BasicCustomerDetails(this._canCreateRF, CustomerID, FormRoot, this);
            details.FormParent = this;
            details.FormRoot = this.FormRoot;
            ((MainForm)this.FormRoot).AddTabPage(details, 10);
            details.loaded = true;
        }
        private void OnRecentAcctClick(object sender, System.EventArgs e)
        {
            Crownwood.Magic.Menus.MenuCommand m = (Crownwood.Magic.Menus.MenuCommand)sender;
            string account = m.Text;
            AccountDetails details = new AccountDetails(account, FormRoot, this);
            ((MainForm)this.FormRoot).AddTabPage(details, 7);
        }

        private void HashMenus()
        {
            dynamicMenus[this.Name + ":menuCustDetails"] = this.menuCustDetails;
            dynamicMenus[this.Name + ":menuNewCustomer"] = this.menuNewCustomer;
            dynamicMenus[this.Name + ":menuRFAccount"] = this.menuRFAccount;
            dynamicMenus[this.Name + ":menuService"] = this.menuServiceRequest; //jec 70566
            dynamicMenus[this.Name + ":menuUnarchive"] = this.menuUnarchive; //IP - 30/01/09 - CR971 - Add new menu item to unarchive archived accounts.
            dynamicMenus[this.Name + ":menuUnsettle"] = this.menuUnsettle; //IP - 30/01/09 - CR971 
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomerSearch));
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.cmenuAssociate = new System.Windows.Forms.MenuItem();
            this.cmenuView = new System.Windows.Forms.MenuItem();
            this.cmenuEdit = new System.Windows.Forms.MenuItem();
            this.cmenuAccountDetails = new System.Windows.Forms.MenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuRecentCust = new Crownwood.Magic.Menus.MenuCommand();
            this.menuRecentAcct = new Crownwood.Magic.Menus.MenuCommand();
            this.menuOptions = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSearch = new Crownwood.Magic.Menus.MenuCommand();
            this.menuAssociate = new Crownwood.Magic.Menus.MenuCommand();
            this.menuCustDetails = new Crownwood.Magic.Menus.MenuCommand();
            this.menuNewCustomer = new Crownwood.Magic.Menus.MenuCommand();
            this.menuRFAccount = new Crownwood.Magic.Menus.MenuCommand();
            this.menuServiceRequest = new Crownwood.Magic.Menus.MenuCommand();
            this.menuUnarchive = new Crownwood.Magic.Menus.MenuCommand();
            this.menuUnsettle = new Crownwood.Magic.Menus.MenuCommand();
            this.grpResults = new System.Windows.Forms.GroupBox();
            this.dgResults = new STL.PL.DataGridCellTips();
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.chxExact = new System.Windows.Forms.CheckBox();
            this.chxIncludeSettled = new System.Windows.Forms.CheckBox();
            this.txtAccountNo = new STL.PL.AccountTextBox();
            this.txtPhoneNo = new System.Windows.Forms.TextBox();
            this.chxLimit = new System.Windows.Forms.CheckBox();
            this.chkPhone = new System.Windows.Forms.CheckBox();
            this.chkAddress = new System.Windows.Forms.CheckBox();
            this.txtPostCode = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.chxStore = new System.Windows.Forms.CheckBox();
            this.btnNewCustomer = new System.Windows.Forms.Button();
            this.btnEnter = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.txtCustID = new System.Windows.Forms.TextBox();
            this.grpResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgResults)).BeginInit();
            this.grpSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.cmenuAssociate,
            this.cmenuView,
            this.cmenuEdit,
            this.cmenuAccountDetails});
            // 
            // cmenuAssociate
            // 
            this.cmenuAssociate.Index = 0;
            this.cmenuAssociate.Text = "Associate with account";
            this.cmenuAssociate.Click += new System.EventHandler(this.menuAssociate_Click);
            // 
            // cmenuView
            // 
            this.cmenuView.Enabled = false;
            this.cmenuView.Index = 1;
            this.cmenuView.Text = "View customer details";
            this.cmenuView.Visible = false;
            // 
            // cmenuEdit
            // 
            this.cmenuEdit.Enabled = false;
            this.cmenuEdit.Index = 2;
            this.cmenuEdit.Text = "Edit customer details";
            this.cmenuEdit.Visible = false;
            // 
            // cmenuAccountDetails
            // 
            this.cmenuAccountDetails.Enabled = false;
            this.cmenuAccountDetails.Index = 3;
            this.cmenuAccountDetails.Text = "View account details";
            this.cmenuAccountDetails.Visible = false;
            this.cmenuAccountDetails.Click += new System.EventHandler(this.cmenuAccountDetails_Click);
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuExit,
            this.menuRecentCust,
            this.menuRecentAcct});
            this.menuFile.Text = "&File";
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuRecentCust
            // 
            this.menuRecentCust.Description = "MenuItem";
            this.menuRecentCust.Text = "Recent &Customers";
            // 
            // menuRecentAcct
            // 
            this.menuRecentAcct.Description = "MenuItem";
            this.menuRecentAcct.Text = "Recent &Accounts";
            // 
            // menuOptions
            // 
            this.menuOptions.Description = "MenuItem";
            this.menuOptions.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuSearch,
            this.menuAssociate,
            this.menuCustDetails,
            this.menuNewCustomer,
            this.menuRFAccount,
            this.menuServiceRequest});
            this.menuOptions.Text = "&Options";
            // 
            // menuSearch
            // 
            this.menuSearch.Description = "MenuItem";
            this.menuSearch.Text = "&Search";
            this.menuSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // menuAssociate
            // 
            this.menuAssociate.Description = "MenuItem";
            this.menuAssociate.Enabled = false;
            this.menuAssociate.Text = "&Associate with account";
            this.menuAssociate.Visible = false;
            this.menuAssociate.Click += new System.EventHandler(this.menuAssociate_Click);
            // 
            // menuCustDetails
            // 
            this.menuCustDetails.Description = "MenuItem";
            this.menuCustDetails.Text = "&Customer Details";
            this.menuCustDetails.Click += new System.EventHandler(this.menuCustDetails_Click);
            // 
            // menuNewCustomer
            // 
            this.menuNewCustomer.Description = "MenuItem";
            this.menuNewCustomer.Enabled = false;
            this.menuNewCustomer.Text = "&New Customer";
            this.menuNewCustomer.Visible = false;
            this.menuNewCustomer.Click += new System.EventHandler(this.menuNewCustomer_Click);
            // 
            // menuRFAccount
            // 
            this.menuRFAccount.Description = "MenuItem";
            this.menuRFAccount.Enabled = false;
            this.menuRFAccount.Text = "Create &RF Account";
            this.menuRFAccount.Visible = false;
            this.menuRFAccount.Click += new System.EventHandler(this.cmenuNewRFAccount_Click);
            // 
            // menuServiceRequest
            // 
            this.menuServiceRequest.Description = "MenuItem";
            // 
            // menuUnarchive
            // 
            this.menuUnarchive.Description = "MenuItem";
            this.menuUnarchive.Enabled = false;
            this.menuUnarchive.Text = "Un-archive";
            this.menuUnarchive.Visible = false;
            this.menuUnarchive.Click += new System.EventHandler(this.menuUnarchive_Click);
            // 
            // menuUnsettle
            // 
            this.menuUnsettle.Description = "MenuItem";
            this.menuUnsettle.Enabled = false;
            this.menuUnsettle.Text = "Unsettle";
            this.menuUnsettle.Visible = false;
            this.menuUnsettle.Click += new System.EventHandler(this.menuUnsettle_Click);
            // 
            // grpResults
            // 
            this.grpResults.BackColor = System.Drawing.SystemColors.Control;
            this.grpResults.Controls.Add(this.dgResults);
            this.grpResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpResults.Location = new System.Drawing.Point(0, 160);
            this.grpResults.Name = "grpResults";
            this.grpResults.Size = new System.Drawing.Size(792, 317);
            this.grpResults.TabIndex = 2;
            this.grpResults.TabStop = false;
            this.grpResults.Text = "Results";
            // 
            // dgResults
            // 
            this.dgResults.CaptionText = "Matching Record";
            this.dgResults.DataMember = "";
            this.dgResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgResults.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgResults.Location = new System.Drawing.Point(3, 18);
            this.dgResults.Name = "dgResults";
            this.dgResults.ReadOnly = true;
            this.dgResults.Size = new System.Drawing.Size(786, 296);
            this.dgResults.TabIndex = 0;
            this.dgResults.ToolTipColumn = 4;
            this.dgResults.ToolTipDelay = 300;
            this.dgResults.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgResults_MouseUp);
            // 
            // grpSearch
            // 
            this.grpSearch.BackColor = System.Drawing.SystemColors.Control;
            this.grpSearch.Controls.Add(this.chxExact);
            this.grpSearch.Controls.Add(this.chxIncludeSettled);
            this.grpSearch.Controls.Add(this.txtAccountNo);
            this.grpSearch.Controls.Add(this.txtPhoneNo);
            this.grpSearch.Controls.Add(this.chxLimit);
            this.grpSearch.Controls.Add(this.chkPhone);
            this.grpSearch.Controls.Add(this.chkAddress);
            this.grpSearch.Controls.Add(this.txtPostCode);
            this.grpSearch.Controls.Add(this.label7);
            this.grpSearch.Controls.Add(this.label6);
            this.grpSearch.Controls.Add(this.label5);
            this.grpSearch.Controls.Add(this.txtAddress);
            this.grpSearch.Controls.Add(this.chxStore);
            this.grpSearch.Controls.Add(this.btnNewCustomer);
            this.grpSearch.Controls.Add(this.btnEnter);
            this.grpSearch.Controls.Add(this.btnClear);
            this.grpSearch.Controls.Add(this.label4);
            this.grpSearch.Controls.Add(this.label3);
            this.grpSearch.Controls.Add(this.label2);
            this.grpSearch.Controls.Add(this.label1);
            this.grpSearch.Controls.Add(this.btnSearch);
            this.grpSearch.Controls.Add(this.txtLastName);
            this.grpSearch.Controls.Add(this.txtFirstName);
            this.grpSearch.Controls.Add(this.txtCustID);
            this.grpSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSearch.Location = new System.Drawing.Point(0, 0);
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.Size = new System.Drawing.Size(792, 160);
            this.grpSearch.TabIndex = 1;
            this.grpSearch.TabStop = false;
            this.grpSearch.Text = "Search Criteria";
            this.grpSearch.Enter += new System.EventHandler(this.grpSearch_Enter);
            // 
            // chxExact
            // 
            this.chxExact.Checked = true;
            this.chxExact.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chxExact.Location = new System.Drawing.Point(491, 84);
            this.chxExact.Name = "chxExact";
            this.chxExact.Size = new System.Drawing.Size(125, 30);
            this.chxExact.TabIndex = 15;
            this.chxExact.Text = "Exact Match";
            // 
            // chxIncludeSettled
            // 
            this.chxIncludeSettled.Location = new System.Drawing.Point(300, 81);
            this.chxIncludeSettled.Name = "chxIncludeSettled";
            this.chxIncludeSettled.Size = new System.Drawing.Size(184, 37);
            this.chxIncludeSettled.TabIndex = 3;
            this.chxIncludeSettled.Text = "Include Settled Accounts";
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.Location = new System.Drawing.Point(768, 88);
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.PreventPaste = false;
            this.txtAccountNo.Size = new System.Drawing.Size(120, 22);
            this.txtAccountNo.TabIndex = 8;
            this.txtAccountNo.Text = "000-0000-0000-0";
            // 
            // txtPhoneNo
            // 
            this.txtPhoneNo.Location = new System.Drawing.Point(767, 43);
            this.txtPhoneNo.Name = "txtPhoneNo";
            this.txtPhoneNo.Size = new System.Drawing.Size(138, 22);
            this.txtPhoneNo.TabIndex = 18;
            // 
            // chxLimit
            // 
            this.chxLimit.Checked = true;
            this.chxLimit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chxLimit.Location = new System.Drawing.Point(644, 85);
            this.chxLimit.Name = "chxLimit";
            this.chxLimit.Size = new System.Drawing.Size(113, 28);
            this.chxLimit.TabIndex = 7;
            this.chxLimit.Text = "View Top 250";
            // 
            // chkPhone
            // 
            this.chkPhone.CausesValidation = false;
            this.chkPhone.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkPhone.Location = new System.Drawing.Point(146, 85);
            this.chkPhone.Name = "chkPhone";
            this.chkPhone.Size = new System.Drawing.Size(165, 28);
            this.chkPhone.TabIndex = 24;
            this.chkPhone.Text = "View Phone Numbers";
            this.chkPhone.CheckedChanged += new System.EventHandler(this.chkPhone_CheckedChanged);
            // 
            // chkAddress
            // 
            this.chkAddress.CausesValidation = false;
            this.chkAddress.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkAddress.Location = new System.Drawing.Point(14, 85);
            this.chkAddress.Name = "chkAddress";
            this.chkAddress.Size = new System.Drawing.Size(135, 28);
            this.chkAddress.TabIndex = 23;
            this.chkAddress.Text = "View Addresses";
            this.chkAddress.CheckedChanged += new System.EventHandler(this.chkAddress_CheckedChanged);
            // 
            // txtPostCode
            // 
            this.txtPostCode.BackColor = System.Drawing.SystemColors.Window;
            this.txtPostCode.Location = new System.Drawing.Point(648, 43);
            this.txtPostCode.MaxLength = 10;
            this.txtPostCode.Name = "txtPostCode";
            this.txtPostCode.Size = new System.Drawing.Size(106, 22);
            this.txtPostCode.TabIndex = 22;
            this.txtPostCode.Tag = "POSTCODE";
            // 
            // label7
            // 
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(648, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(106, 19);
            this.label7.TabIndex = 21;
            this.label7.Text = "Post Code:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(472, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 17);
            this.label6.TabIndex = 20;
            this.label6.Text = "Address";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(763, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 17);
            this.label5.TabIndex = 19;
            this.label5.Text = "Phone Number";
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(469, 43);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(169, 22);
            this.txtAddress.TabIndex = 17;
            // 
            // chxStore
            // 
            this.chxStore.Location = new System.Drawing.Point(18, 126);
            this.chxStore.Name = "chxStore";
            this.chxStore.Size = new System.Drawing.Size(271, 27);
            this.chxStore.TabIndex = 16;
            this.chxStore.Text = "Load Customers From Other Stores";
            // 
            // btnNewCustomer
            // 
            this.btnNewCustomer.Image = ((System.Drawing.Image)(resources.GetObject("btnNewCustomer.Image")));
            this.btnNewCustomer.Location = new System.Drawing.Point(456, 123);
            this.btnNewCustomer.Name = "btnNewCustomer";
            this.btnNewCustomer.Size = new System.Drawing.Size(94, 30);
            this.btnNewCustomer.TabIndex = 14;
            this.btnNewCustomer.Click += new System.EventHandler(this.menuNewCustomer_Click);
            // 
            // btnEnter
            // 
            this.btnEnter.Location = new System.Drawing.Point(767, 122);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(90, 30);
            this.btnEnter.TabIndex = 13;
            this.btnEnter.Text = "Exit";
            this.btnEnter.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(612, 121);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(96, 32);
            this.btnClear.TabIndex = 12;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(764, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 19);
            this.label4.TabIndex = 9;
            this.label4.Text = "Tie to account:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(296, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 19);
            this.label3.TabIndex = 6;
            this.label3.Text = "Last Name:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(146, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 19);
            this.label2.TabIndex = 5;
            this.label2.Text = "First Name:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(14, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 19);
            this.label1.TabIndex = 4;
            this.label1.Text = "Customer ID:";
            // 
            // btnSearch
            // 
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.Location = new System.Drawing.Point(300, 122);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(88, 31);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtLastName
            // 
            this.txtLastName.Location = new System.Drawing.Point(299, 43);
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(163, 22);
            this.txtLastName.TabIndex = 2;
            // 
            // txtFirstName
            // 
            this.txtFirstName.Location = new System.Drawing.Point(148, 43);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(144, 22);
            this.txtFirstName.TabIndex = 1;
            // 
            // txtCustID
            // 
            this.txtCustID.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCustID.Location = new System.Drawing.Point(14, 43);
            this.txtCustID.Name = "txtCustID";
            this.txtCustID.Size = new System.Drawing.Size(126, 22);
            this.txtCustID.TabIndex = 0;
            // 
            // CustomerSearch
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.grpResults);
            this.Controls.Add(this.grpSearch);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CustomerSearch";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Customer Search";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.CustomerSearch_Closing);
            this.Load += new System.EventHandler(this.CustomerSearch_Load);
            this.Enter += new System.EventHandler(this.CustomerSearch_Enter);
            this.grpResults.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgResults)).EndInit();
            this.grpSearch.ResumeLayout(false);
            this.grpSearch.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            //CR1072 Malaysia merge 
            // Display Message if search called from Sanction S1 and no customer assigned as 
            // second applicant. jec 70148 29/09/08 
            if (Spouse.Visited)
            {
                if (DialogResult.OK == MessageBox.Show("Second applicant has not been assigned. You must assign a customer as Second applicant", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                {
                    CustomerSearch cust = new CustomerSearch(AccountNo, AccountType, SanctionScreen, Relationship);
                }
                else
                {
                    Spouse.Visited = false;
                    CloseTab();
                }
            }
            else
                CloseTab();
        }

        private void SearchThread()
        {
            try
            {
                Wait();
                Function = "LoadAccountsThread";

                ds = CustomerManager.CustomerSearch(customerID, firstName,
                                                    lastName, address, phoneNumber,         //CR1084 
                                                    limit,
                                                    settled,
                                                    exactMatch,
                                                    storeType,
                                                   "CUSTSRCH",out Error);
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

        private void Search(string custid, string first, string last, string addr123, string phone, int isLimited, int incSettled, bool exact)
        {
            limit = isLimited;
            settled = incSettled;
            customerID = custid;
            firstName = first;
            lastName = last;
            phoneNumber = phone;        //CR1084
            address = addr123;        //CR1084
            exactMatch = exact;
            storeType = chxStore.Checked ? "%" : Config.StoreType + "%";
            ds = null;

            Thread data = new Thread(new ThreadStart(SearchThread));
            data.Start();
            data.Join();

            if (ds == null)
                return;

            dv = new DataView(ds.Tables["CustSearch"]);
            dgResults.DataSource = dv;
            DataGridTableStyle tabStyle = new DataGridTableStyle();
            tabStyle.MappingName = dv.Table.TableName;
            int numCols = ds.Tables["CustSearch"].Columns.Count;


            AccountTextColumn col;
            for (int i = 0; i < numCols; ++i)
            {
                col = new AccountTextColumn(i);
                col.AccountColumn = 4;
                col.HeaderText = ds.Tables["CustSearch"].Columns[i].ColumnName;
                col.MappingName = ds.Tables["CustSearch"].Columns[i].ColumnName;
                tabStyle.GridColumnStyles.Add(col);
                col.Dispose();
            }
            if (dgResults.TableStyles.Count == 0)
            {
                
                dgResults.TableStyles.Add(tabStyle);
                // 67782 RD/DR 19/01/06 Added HldOrJnt
                tabStyle.GridColumnStyles["Customer ID"].Width = 75;
                tabStyle.GridColumnStyles["Customer ID"].HeaderText = GetResource("T_CUSTID");
                tabStyle.GridColumnStyles["Title"].Width = 36;
                tabStyle.GridColumnStyles["Title"].HeaderText = GetResource("T_TITLE");
                tabStyle.GridColumnStyles["First Name"].Width = 75;
                tabStyle.GridColumnStyles["First Name"].HeaderText = GetResource("T_FIRSTNAME");
                tabStyle.GridColumnStyles["Last Name"].Width = 75;
                tabStyle.GridColumnStyles["Last Name"].HeaderText = GetResource("T_LASTNAME");
                tabStyle.GridColumnStyles["AccountNumber"].Width = 85;
                tabStyle.GridColumnStyles["AccountNumber"].HeaderText = GetResource("T_ACCTNO");
                tabStyle.GridColumnStyles["HldOrJnt"].Width = 40;
                tabStyle.GridColumnStyles["HldOrJnt"].HeaderText = GetResource("T_HLDJNT");
                tabStyle.GridColumnStyles["HldOrJnt"].Alignment = HorizontalAlignment.Center;
                tabStyle.GridColumnStyles["Type"].Width = 26;
                tabStyle.GridColumnStyles["Type"].HeaderText = GetResource("T_TYPE");
                tabStyle.GridColumnStyles["Type"].Alignment = HorizontalAlignment.Center;
                tabStyle.GridColumnStyles["Date Opened"].Width = 75;
                tabStyle.GridColumnStyles["Date Opened"].HeaderText = GetResource("T_DATEOPENED");
                tabStyle.GridColumnStyles["Date Opened"].NullText = "";
                tabStyle.GridColumnStyles["Outstanding Balance"].Width = 75;
                tabStyle.GridColumnStyles["Outstanding Balance"].HeaderText = GetResource("T_OUTBAL");
                tabStyle.GridColumnStyles["Outstanding Balance"].Alignment = HorizontalAlignment.Right;
                ((DataGridTextBoxColumn)tabStyle.GridColumnStyles["Outstanding Balance"]).Format = DecimalPlaces;
                tabStyle.GridColumnStyles["Arrears"].Width = 75;
                tabStyle.GridColumnStyles["Arrears"].HeaderText = GetResource("T_ARREARS");
                tabStyle.GridColumnStyles["Arrears"].Alignment = HorizontalAlignment.Right;
                ((DataGridTextBoxColumn)tabStyle.GridColumnStyles["Arrears"]).Format = DecimalPlaces;
                tabStyle.GridColumnStyles["Agreement Total"].Width = 75;
                tabStyle.GridColumnStyles["Agreement Total"].HeaderText = GetResource("T_AGREETOTAL");
                tabStyle.GridColumnStyles["Agreement Total"].Alignment = HorizontalAlignment.Right;
                ((DataGridTextBoxColumn)tabStyle.GridColumnStyles["Agreement Total"]).Format = DecimalPlaces;
                tabStyle.GridColumnStyles["Status"].Width = 36;
                tabStyle.GridColumnStyles["Status"].HeaderText = GetResource("T_STATUS");
                tabStyle.GridColumnStyles["Status"].Alignment = HorizontalAlignment.Center;
                tabStyle.GridColumnStyles["CashPrice"].Width = 75;              //UAT928 jec
                tabStyle.GridColumnStyles["CashPrice"].HeaderText = GetResource("T_CASHPRICE");
                tabStyle.GridColumnStyles["CashPrice"].Alignment = HorizontalAlignment.Right;
                ((DataGridTextBoxColumn)tabStyle.GridColumnStyles["CashPrice"]).Format = DecimalPlaces;
                tabStyle.GridColumnStyles["Branchno"].Width = 45;              //UAT928 jec
                tabStyle.GridColumnStyles["Branchno"].HeaderText = GetResource("T_BRANCH");
                tabStyle.GridColumnStyles["Branchno"].Alignment = HorizontalAlignment.Center;
                //IP - CR971 - Format 'Archived' column
                tabStyle.GridColumnStyles[CN.Archived].Width = 0;
                //tabStyle.GridColumnStyles[CN.Archived].HeaderText = GetResource("T_ARCHIVED");
                //tabStyle.GridColumnStyles[CN.Archived].Alignment = HorizontalAlignment.Right;
                tabStyle.GridColumnStyles["Bdw"].Width = 0;
                tabStyle.GridColumnStyles["Alias"].Width = 0;
                tabStyle.GridColumnStyles[CN.AddTo].Width = 0;
                tabStyle.GridColumnStyles[CN.DeliveredIndicator].Width = 0;
                tabStyle.GridColumnStyles[CN.Reversible].Width = 0;
                tabStyle.GridColumnStyles[CN.AgrmtNo].Width = 0;
                tabStyle.GridColumnStyles["IsMambuAccount"].Width = 0;
                //jec 17/08/10 - Only show HomeClub if Loyalty scheme enabled
                if (Convert.ToBoolean(Country[CountryParameterNames.LoyaltyScheme]) == true)
                {
                    tabStyle.GridColumnStyles["LoyaltyMemNo"].Width = 100;
                    tabStyle.GridColumnStyles["LoyaltyMemNo"].HeaderText = "HomeClub Memberno";
                }
                else
                {
                    tabStyle.GridColumnStyles["LoyaltyMemNo"].Width = 0;
                }

                tabStyle.GridColumnStyles[CN.cusaddr1].HeaderText = "Address Line 1";
                tabStyle.GridColumnStyles[CN.cusaddr2].HeaderText = "Address Line 2";
                tabStyle.GridColumnStyles[CN.cusaddr3].HeaderText = "Address Line 3";
                tabStyle.GridColumnStyles[CN.TelNo].HeaderText = "Phone No";

                if (chkAddress.Checked == true)
                {
                    dgResults.TableStyles[0].GridColumnStyles["cusaddr1"].Width = 118;
                    dgResults.TableStyles[0].GridColumnStyles["cusaddr1"].Alignment = HorizontalAlignment.Left;
                    dgResults.TableStyles[0].GridColumnStyles["cusaddr2"].Width = 118;
                    dgResults.TableStyles[0].GridColumnStyles["cusaddr2"].Alignment = HorizontalAlignment.Left;
                    dgResults.TableStyles[0].GridColumnStyles["cusaddr3"].Width = 118;
                    dgResults.TableStyles[0].GridColumnStyles["cusaddr3"].Alignment = HorizontalAlignment.Left;
                    dgResults.TableStyles[0].GridColumnStyles["postcode"].Width = 60;
                    dgResults.TableStyles[0].GridColumnStyles["postcode"].Alignment = HorizontalAlignment.Left;
                }
                else
                {
                    dgResults.TableStyles[0].GridColumnStyles["cusaddr1"].Width = 0;
                    dgResults.TableStyles[0].GridColumnStyles["cusaddr2"].Width = 0;
                    dgResults.TableStyles[0].GridColumnStyles["cusaddr3"].Width = 0;
                    dgResults.TableStyles[0].GridColumnStyles["postcode"].Width = 0;

                    if (txtAddress.Text == String.Empty)        //CR1084 - only show address if searching for address
                    {
                        dgResults.TableStyles[0].GridColumnStyles[CN.cusaddr1].Width = 0;
                        dgResults.TableStyles[0].GridColumnStyles[CN.cusaddr2].Width = 0;
                        dgResults.TableStyles[0].GridColumnStyles[CN.cusaddr3].Width = 0;
                    }
                    else
                    {
                        dgResults.TableStyles[0].GridColumnStyles[CN.cusaddr1].Width = 118;
                        dgResults.TableStyles[0].GridColumnStyles[CN.cusaddr2].Width = 118;
                        dgResults.TableStyles[0].GridColumnStyles[CN.cusaddr3].Width = 118;
                    }

                    if (txtPostCode.Text != String.Empty)
                    {
                        dgResults.TableStyles[0].GridColumnStyles["postcode"].Width = 60;
                        dgResults.TableStyles[0].GridColumnStyles["postcode"].Alignment = HorizontalAlignment.Left;
                    }
                    else
                        dgResults.TableStyles[0].GridColumnStyles["postcode"].Width = 0;
                }

                if (chkPhone.Checked == true || txtPhoneNo.Text != String.Empty)
                    dgResults.TableStyles[0].GridColumnStyles["telno"].Width = 80;
                else
                    dgResults.TableStyles[0].GridColumnStyles["telno"].Width = 0;

                if (dgResults.TableStyles[0].GridColumnStyles[CN.HoldProp] != null)
                    tabStyle.GridColumnStyles[CN.HoldProp].Width = 0; //IP 13/03/09 - (70849) - Do not want to display the column
            }
            else if (dgResults.TableStyles.Count > 0 && dgResults.TableStyles[0].GridColumnStyles[CN.HoldProp] != null)
            {
                dgResults.TableStyles[0].GridColumnStyles[CN.HoldProp].Width = 0; //IP 13/03/09 - (70849) - Do not want to display the column
            }

            ((MainForm)this.FormRoot).StatusBarText = dv.Count + " Row(s) returned";
        }

        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            Function = "WCustomerManager::CustomerSearch()";
            try
            {
                Wait();
                Search(txtCustID.Text.Trim(),
                        txtFirstName.Text.Trim(),
                        txtLastName.Text.Trim(),
                        txtAddress.Text.Trim(),
                        txtPhoneNo.Text.Trim(),
                        chxLimit.Checked ? 250 : 1000,
                        Convert.ToInt32(chxIncludeSettled.Checked),
                        chxExact.Checked);   //CR1084
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

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            txtCustID.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtAddress.Text = "";        //CR1084
            txtPhoneNo.Text = "";       //CR1084
            txtPostCode.Text = "";
            //txtAccountNo.Text = "000-0000-0000-0";
            if (dv != null)
                dv.Table.Clear();
            ((MainForm)this.FormRoot).StatusBarText = "";
        }


        private void AsscoicateStoreCard(object sender, EventArgs e)
        {

            if (CustidSelected != null && (string)dgResults[dgResults.CurrentRowIndex, 0] != null || (string)dgResults[dgResults.CurrentRowIndex, 0] != string.Empty)
                CustidSelected(this, new RecordIDEventArgs<StoreCardCustDetails>(new StoreCardCustDetails
                {
                    Custid = (string)dgResults[dgResults.CurrentRowIndex, 0],
                    Title = (string)dgResults[dgResults.CurrentRowIndex, 1],
                    FirstName = (string)dgResults[dgResults.CurrentRowIndex, 2],
                    LastName = (string)dgResults[dgResults.CurrentRowIndex, 3]
                }));
            this.CloseTab(this);
        }

        /// <summary>
        /// This will create an entry in the custacct table linking the 
        /// selected customer to the account passed into the screen.
        /// If the account is already associated with a customer, special 
        /// permission will be required to break this link.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuAssociate_Click(object sender, System.EventArgs e)
        {
            string custid = "";
            string newCustId = "";
            int index = 0;
            try
            {
                Wait();
                index = dgResults.CurrentRowIndex;

                if (index >= 0)
                {
                    newCustId = (string)dgResults[index, 0];
                    custid = AccountManager.GetLinkedCustomerID(txtAccountNo.Text.Replace("-", ""), out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        if (custid.Length > 0)
                        {
                            CustAcctOverride over = new CustAcctOverride(txtAccountNo.Text,
                                custid,
                                newCustId, this);
                            over.ShowDialog();
                        }
                        else
                        {
                            bool rescore = false;
                            AccountManager.AddCustomerToAccount(txtAccountNo.Text.Replace("-", ""),
                                                    newCustId,
                                                    "H",
                                                    AccountType, 0,
                                                    out rescore,
                                                    out Error);
                            if (Error.Length > 0)
                                ShowError(Error);
                            else
                            {
                                //Associated = true;      //CR1072 Malaysia merge
                                //CloseTab();
                                //((NewAccount)this.FormParent).LinkCustomer(newCustId);
                                //((NewAccount)this.FormParent).LinkCustomer(newCustId);

                                NewAccount acct = new NewAccount(this.AccountNo, 1, null, false, this.FormRoot, this.FormParent);
                                acct.LinkCustomer(newCustId);
                                BasicCustomerDetails cust = new BasicCustomerDetails(this._canCreateRF, newCustId, this.AccountNo, Holder.Main, AccountType, FormRoot, this);
                                ((MainForm)this.FormRoot).AddTabPage(cust, 10);
                                cust.loaded = true;
                                associated = true;

                                cust.CustidSelected += new RecordIDHandler<StoreCardCustDetails>(CustomerSearch_CustidSelected);
                                if (CustidSelected != null)
                                    CustidSelected(this, new RecordIDEventArgs<StoreCardCustDetails>(new StoreCardCustDetails() { Custid = newCustId }));
                                this.CloseTab(this);
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
                StopWait();
            }

        }

        void CustomerSearch_CustidSelected(object sender, RecordIDEventArgs<StoreCardCustDetails> args)
        {
            if (this.CustidSelected != null)
            {
                this.CustidSelected(sender, args);
            }
        }


        private void cmenuNewRFAccount_Click(object sender, System.EventArgs e)
        {
            try
            {
                STL.PL.WS2.CashLoanDetails det = null;      // jec 25/10/11
                Function = "cmenuNewRFAccount";
                Wait();
                decimal limit = 0;
                decimal available = 0;
                bool wrongType = false;

                CreditManager.GetRFLimit(this.ClickedCust, "", "R", out limit, out available, out wrongType, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    if (available == 0)
                    {
                        ShowInfo("M_NORFCREDIT1");
                    }
                    else
                    {
                        if (FormRoot == FormParent || !((NewAccount)this.FormParent).ManualSale)
                        {
                            bool rescore = false;
                            bool reOpenS1 = false;
                            bool reOpenS2 = false;

                            //if we have available credit we can proceed
                            string acctno = AccountManager.CreateRFAccount(Config.CountryCode,
                                Convert.ToInt16(Config.BranchCode),
                                this.ClickedCust, Credential.UserId, false, ref det, out rescore, out reOpenS1, out reOpenS2, out Error);   //#3915 jec 28/09/11

                            if (Error.Length > 0)
                                ShowError(Error);
                            else
                            {
                                if (acctno.Length != 0)
                                {
                                    if (this.AccountType == AT.ReadyFinance && FormRoot != FormParent)
                                    {
                                        ((NewAccount)this.FormParent).txtAccountNumber.Text = acctno;
                                        ((NewAccount)this.FormParent).loadAccountData(acctno.Replace("-", ""), true);
                                        this.CloseTab();
                                    }
                                    else
                                    {
                                        if (!rescore)
                                        {
                                            NewAccount page = new NewAccount(acctno.Replace("-", ""), 1, null, false, FormRoot, this);
                                            page.Text = "Revise Sales Order";
                                            ((MainForm)this.FormRoot).AddTabPage(page, 6);
                                            page.SupressEvents = false;
                                            if (!page.AccountLocked)
                                            {
                                                page.Confirm = false;
                                                page.CloseTab();
                                            }
                                        }
                                        else
                                        {
                                            BasicCustomerDetails details = new BasicCustomerDetails(this._canCreateRF, FormRoot, FormParent);

                                            if (txtCustID.Text.Length > 0)
                                            {
                                                details.txtCustID.Text = txtCustID.Text;
                                                details.txtCustID_Leave(null, null);
                                            }

                                            ((MainForm)this.FormRoot).AddTabPage(details, 10);
                                            details.loaded = true;
                                            ShowInfo("M_RFRESCOREREQUIRED");
                                        }
                                    }
                                }
                                else
                                {
                                    ShowInfo("M_ACCOUNTNOFAILED");
                                }
                            }
                        }
                        else if (((NewAccount)this.FormParent).ManualSale)
                        {
                            if (this.AccountType == AT.ReadyFinance)
                            {
                                ManualAccountNo man = new ManualAccountNo();
                                man.ShowDialog();

                                string acctno = man.txtAccountNumber.Text;
                                ((NewAccount)this.FormParent).txtAccountNumber.Text = acctno;
                                this.CloseTab();
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
                StopWait();
            }
        }


        private void AssignApplicantTwo(object sender, System.EventArgs e)
        {
            try
            {
                Function = "AssignApplicantTwo";
                Wait();
                bool rescore = false;
                AccountManager.AddCustomerToAccount(SanctionScreen.AccountNo, this.ClickedCust, this.Relationship, SanctionScreen.AccountType, 0, out rescore, out Error); //CR1072 Malaysia merge
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    //CR1072 Malaysia merge
                    // set property to false so message not displayed when closing other search screens
                    Spouse.Visited = false;     //jec 70148 29/09/08 
                    SanctionScreen.LoadAppTwo(Relationship);
                    CloseTab(this);
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

        private void cmenuAccountDetails_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "cmenuAccountDetails";
                int index = dgResults.CurrentRowIndex;

                if (index >= 0)
                {
                    string acctNo = (string)dgResults[index, 4];
                    if (acctNo.Length != 0)
                    {
                        AccountDetails details = new AccountDetails(acctNo.Replace("-", ""), FormRoot, this);
                        ((MainForm)this.FormRoot).AddTabPage(details, 7);
                    }
                    else
                        ShowInfo("M_NOACCOUNT");
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        //private void cmenuServiceRequest_Click(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        Function = "cmenuServiceRequest_Click";
        //        Wait();

        //        if (dgResults.CurrentRowIndex < 0)
        //            return;

        //        string custId = (string)dgResults[dgResults.CurrentRowIndex, 0];
        //        string acctNo = (string)dgResults[dgResults.CurrentRowIndex, 4];

        //        // Open the Service Request screen	
        //        SERVICE.SR_ServiceRequest serviceRequest = new SERVICE.SR_ServiceRequest(this.FormRoot, this.FormParent, String.Empty, acctNo, custId);
        //        ((MainForm)this.FormRoot).AddTabPage(serviceRequest);
        //        this.CloseTab(this);
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, Function);
        //    }
        //    finally
        //    {
        //        StopWait();
        //    }
        //}

        //private void menuServiceRequest_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Function = "menuServiceRequest_Click";
        //        Wait();

        //        if (dgResults.CurrentRowIndex >= 0)
        //        {
        //            string custId = (string)dgResults[dgResults.CurrentRowIndex, 0];
        //            string acctNo = (string)dgResults[dgResults.CurrentRowIndex, 4];

        //            SERVICE.SR_ServiceRequest serviceRequest = new SERVICE.SR_ServiceRequest(this.FormRoot, this.FormParent, String.Empty, acctNo, custId);
        //            ((MainForm)this.FormRoot).AddTabPage(serviceRequest);
        //            this.CloseTab(this);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, Function);
        //    }
        //    finally
        //    {
        //        StopWait();
        //    }
        //}

        private void menuNewCustomer_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "menuNewCustomer_Click";

                if (SanctionScreen == null)
                {
                    BasicCustomerDetails details;
                    this._canCreateRF = true; //KEF added as Create RF button was greyed out
                    if (txtAccountNo.Visible)
                        details = new BasicCustomerDetails(this._canCreateRF, txtAccountNo.Text, AccountType, FormRoot, FormParent);
                    else
                        details = new BasicCustomerDetails(this._canCreateRF, FormRoot, FormParent);
                    details.CustidSelected += new RecordIDHandler<StoreCardCustDetails>(CustomerSearch_CustidSelected);

                    if (txtCustID.Text.Length > 0)
                    {
                        details.txtCustID.Text = txtCustID.Text;
                        details.txtCustID_Leave(null, null);
                    }

                    ((MainForm)this.FormRoot).AddTabPage(details, 10);
                    details.loaded = true;
                }
                else
                {
                    // BasicCustomerDetails cust = new BasicCustomerDetails(this._canCreateRF, SanctionScreen.CustomerID, tmp, SanctionScreen.AccountType, FormRoot, this);
                    // No longer passing in customer id as was causing details to be loaded for main holder for new spouse.
                    // DSR 13 Jan 2005 - Also need to pass a relationship parameter so that sproc does not look for main holder details.
                    BasicCustomerDetails cust = new BasicCustomerDetails(this._canCreateRF, "", FormatAccountNo(SanctionScreen.AccountNo), Relationship, SanctionScreen.AccountType, FormRoot, this);
                    cust.SanctionScreen = this.SanctionScreen;
                    ((MainForm)FormRoot).AddTabPage(cust, 10);

                    foreach (DataRowView row in cust.drpRelationship.Items)
                        if ((string)row[CN.Code] == Relationship)
                            cust.drpRelationship.SelectedItem = row;
                    cust.loaded = true;
                }

                this.CloseTab(this);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        /// <summary>
        /// Selects the whole datagrid row when the user clicks a cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgResults_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int index = dgResults.CurrentRowIndex;

            if (index < 0)
                return;

            dgResults.Select(dgResults.CurrentCell.RowNumber);

            // Always need to set this for either mouse button
            this.ClickedAccount = (string)dgResults[index, 4];
            this.ClickedType = (string)dgResults[index, 6];		// AcctType is now column 6
            this.ClickedCust = txtCustID.Text = (string)dgResults[index, 0];
            txtFirstName.Text = (string)dgResults[index, 2];
            txtLastName.Text = (string)dgResults[index, 3];
            txtPhoneNo.Text = (string)dgResults[index, 27];      //CR1084
            txtPostCode.Text = (string)dgResults[index, 26];      //CR1084
            this.ClickedStatus = (string)dgResults[index, 11]; //IP - CR971
            string customerID = (string)dgResults[dgResults.CurrentRowIndex, 0];

            //IP - CR971 - Check if the selected is an 'Archived' account.
            ClickedArchived = Convert.ToBoolean(dgResults[index, 19]);

            if (e.Button != MouseButtons.Right)
                return;

            //IP - CR971 - Check if the selected has a bdwbalance > 0
            bool isBDW = Convert.ToBoolean(dgResults[index, 20]);
            bool isSettled = ClickedStatus == "S";
            bool hasInstallations = Convert.ToBoolean(dgResults[index, 28]);

            DataGrid ctl = (DataGrid)sender;

            MenuCommand m1 = new MenuCommand(GetResource("P_ASSOCIATE"));
            MenuCommand m2 = new MenuCommand(GetResource("P_CUSTDETAILS"));
            MenuCommand m3 = new MenuCommand(GetResource("P_ACCOUNT_DETAILS"));
            MenuCommand m4 = new MenuCommand(GetResource("P_ASSIGN_APP2"));
            //MenuCommand m5 = new MenuCommand(GetResource("P_SERVICE_REQUEST"));                   //#13742
            MenuCommand m6 = new MenuCommand(GetResource("P_UNARCHIVE")); //IP - 30/01/09 - CR971
            MenuCommand m7 = new MenuCommand(GetResource("P_UNSETTLE"));//IP - 30/01/09 - CR971
            MenuCommand associtateWithStoreCard = new MenuCommand("Associate Customer with Storecard");//IP - 30/01/09 - CR971

            m1.Click += new System.EventHandler(this.menuAssociate_Click);
            m2.Click += new System.EventHandler(this.menuCustDetails_Click);
            m3.Click += new System.EventHandler(this.cmenuAccountDetails_Click);
            m4.Click += new System.EventHandler(this.AssignApplicantTwo);
            //m5.Click += new System.EventHandler(this.cmenuServiceRequest_Click);
            m6.Click += new System.EventHandler(this.menuUnarchive_Click); //IP - 30/01/09 - CR971
            m7.Click += new System.EventHandler(this.menuUnsettle_Click); //IP - 30/01/09 - CR971
            associtateWithStoreCard.Click += new System.EventHandler(this.AsscoicateStoreCard);

            m1.Enabled = menuAssociate.Enabled;
            m1.Visible = menuAssociate.Visible;
            m2.Enabled = menuCustDetails.Enabled;
            m2.Visible = menuCustDetails.Visible;
            // set permission for service request        jec 70566
            //m5.Enabled = menuServiceRequest.Enabled;                                              //#13742
            //m5.Visible = menuServiceRequest.Visible;                                              //#13742

            //IP - CR971 - Display 'Unarchive' option only for 'Archived' accounts.
            //m6.Visible = menuUnarchive.Visible = ClickedArchived; //IP - 30/01/09 - CR971
            //m6.Enabled = menuUnarchive.Enabled = ClickedArchived; //IP - 30/01/09 - CR971
            if (menuUnarchive.Visible && ClickedArchived)
            {
                m6.Visible = true;
                m6.Enabled = true;
            }
            else
            {
                m6.Visible = false;
                m6.Enabled = false;
            }

            //m7.Visible = menuUnsettle.Visible = isSettled; //IP - 30/01/09 - CR971
            //m7.Enabled = menuUnsettle.Enabled = isSettled; //IP - 30/01/09 - CR971
            //Only display the 'Unsettle' option if the account is a settled account, and not a bdw account

            if (menuUnsettle.Visible && isSettled && !isBDW)
            {
                m7.Visible = true;
                m7.Enabled = true;
            }
            else
            {
                m7.Visible = false;
                m7.Enabled = false;
            }


            // 14/11/07 clicking on a blank account if no account for customer dont show account menu option
            bool hasAccount = this.ClickedAccount.Trim() != string.Empty;
            m3.Enabled = hasAccount;
            m3.Visible = hasAccount;

            PopupMenu popup = new PopupMenu();
            popup.Animate = Animate.Yes;
            popup.AnimateStyle = Animation.SlideHorVerPositive;

            if (SanctionScreen != null)
                popup.MenuCommands.AddRange(new MenuCommand[] { m2, m4 });
            else
            {
                if (txtAccountNo.Visible)
                    popup.MenuCommands.AddRange(new MenuCommand[] { m1, m2, m3, m6, m7 }); //#13742 - removed m5
                else
                    popup.MenuCommands.AddRange(new MenuCommand[] { m2, m3, m6, m7 });     //#13742 - removed m5
            }

            if (Config.ThermalPrintingEnabled)
            {
                MenuCommand menuCommandStatement = new MenuCommand("Statement");
                menuCommandStatement.Visible = menuCommandStatement.Enabled = true;
                menuCommandStatement.Click +=
                    delegate(object ssender, EventArgs ee)
                    {
                        ((MainForm)this.FormRoot).OpenStatementsTab(customerID);
                    };
                popup.MenuCommands.Add(menuCommandStatement);
            }

            //if (hasInstallations)
            //{
            //    var menuInstallation = new MenuCommand(GetResource("P_INSTALLATION"));
            //    menuInstallation.Click += new System.EventHandler(menuInstallationt_Click);
            //    popup.MenuCommands.Add(menuInstallation);
            //}

            if (!popup.MenuCommands.Contains(associtateWithStoreCard) && AssoicateCustid)
                popup.MenuCommands.Add(associtateWithStoreCard);

            MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
        }

        private void CustomerSearch_Load(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                if (AccountType == AT.ReadyFinance && SanctionScreen == null)
                {
                    if (FormParent is NewAccount)
                    {
                        ((NewAccount)FormParent).ConfirmClose();
                    }

                    CloseTab((CommonForm)FormParent);

                    /* since the parent form is being closed asign
                     * FormRoot to FormParent */
                    FormParent = FormRoot;
                }

                toolTip1.SetToolTip(this.btnSearch, GetResource("TT_SEARCH"));
                toolTip1.SetToolTip(this.btnNewCustomer, GetResource("TT_NEWCUST"));

                dynamicMenus = new Hashtable();
                HashMenus();
                this.ApplyRoleRestrictions();
                if (menuNewCustomer.Visible == true)
                {
                    btnNewCustomer.Enabled = true;
                }
                if (this.AccountNo.Length == 0)
                {
                    label4.Visible = txtAccountNo.Visible = false;
                    menuAssociate.Visible = false;
                    cmenuAssociate.Visible = false;
                }
                else
                {
                    string custid = txtCustID.Text = AccountManager.GetLinkedCustomerID(this.AccountNo.Replace("-", ""), out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        if (custid.Length > 0)
                        {
                            BasicCustomerDetails cust = new BasicCustomerDetails(this._canCreateRF, custid, this.AccountNo, Holder.Main, AccountType, FormRoot, this);
                            cust.FormRoot = this.FormRoot;
                            cust.FormParent = this;
                            ((MainForm)this.FormRoot).AddTabPage(cust, 10);
                            cust.loaded = true;
                            this.CloseTab(this);
                        }
                    }
                }
                //TranslateControls();
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
            /*
            if(this.AccountType==AT.ReadyFinance)
            {
                if(((NewAccount)this.FormParent).txtAccountNumber.Text == "000-0000-0000-0")
                {
                    ((NewAccount)this.FormParent).Confirm = false;
                    CloseTab((CommonForm)this.FormParent);
                }
            }
            */
            //CR1072 Malaysia merge
            // Display Message if search called from Sanction S1 and no customer assigned as 
            // second applicant. jec 70148 29/09/08 
            if (Spouse.Visited &&
                DialogResult.OK == MessageBox.Show("Second applicant has not been assigned. You must assign a customer as Second applicant", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
            {
                new CustomerSearch(AccountNo, AccountType, SanctionScreen, Relationship);
                return false;
            }

            // End of jec 70148 29/09/08
            return true;
        }

        private void CustomerSearch_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.AccountType == AT.ReadyFinance && (FormParent as NewAccount).txtAccountNumber.Text == "000-0000-0000-0")
            {
                ((NewAccount)this.FormParent).Confirm = false;
                ((NewAccount)this.FormParent).CloseTab();
            }
        }

        private void menuCustDetails_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "menuCustDetails_Click";
                int index = dgResults.CurrentRowIndex;
                BasicCustomerDetails details = null;

                if (index >= 0)
                {
                    string custid = (string)dgResults[index, 0];
                    string title = (string)dgResults[index, 1];
                    string first = (string)dgResults[index, 2];
                    string last = (string)dgResults[index, 3];
                    string alias = (string)dgResults[index, 11];
                    string acctno = (string)dgResults[index, 4];
                    string acctType = (string)dgResults[index, 6];		// AcctType is now column 6
                    string holder = (string)dgResults[index, 5]; //SC 70147 24/09/08

                    if (SanctionScreen == null)
                    {
                        this._canCreateRF = true; //KEF added as Create RF button was greyed out
                        if (txtAccountNo.Visible)
                            details = new BasicCustomerDetails(this._canCreateRF, custid, txtAccountNo.Text, holder, AccountType, FormRoot, this);//SC 70147 24/09/08
                        else
                            details = new BasicCustomerDetails(this._canCreateRF, custid, FormatAccountNo(acctno), holder, acctType, FormRoot, this);//SC 70147 24/09/08

                        ((MainForm)this.FormRoot).AddTabPage(details, 10);
                        details.loaded = true;
                    }
                    else
                    {
                        BasicCustomerDetails cust = new BasicCustomerDetails(this._canCreateRF, custid, FormatAccountNo(SanctionScreen.AccountNo), Holder.Main, SanctionScreen.AccountType, FormRoot, SanctionScreen);
                        cust.FormRoot = this.FormRoot;
                        cust.FormParent = SanctionScreen;
                        cust.SanctionScreen = SanctionScreen;
                        ((MainForm)FormRoot).AddTabPage(cust, 10);
                        cust.loaded = true;

                        foreach (DataRowView row in cust.drpRelationship.Items)
                            if ((string)row[CN.Code] == Relationship)
                                cust.drpRelationship.SelectedItem = row;

                        cust.txtCustID.Text = custid;
                        cust.txtCustID_Leave(this, null);

                        CloseTab(this);
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void CustomerSearch_Enter(object sender, System.EventArgs e)
        {
            BuildRecentListMenus();
        }

        //IP - 30/01/09 - CR971
        private void menuUnarchive_Click(object sender, EventArgs e)
        {
            unsettleAcct = false;

            try
            {
                //Selected account is archived, therefore unarchive do not unsettle.
                AccountManager.UnarchiveUnsettle(ClickedAccount, ClickedArchived, unsettleAcct, out Error);

                if (Error.Length > 0)
                    ShowError(Error);

                //Need to search for the customers account as the account has now been un-archived.
                Search(txtCustID.Text, txtFirstName.Text, txtLastName.Text, txtAddress.Text, txtPhoneNo.Text, limit, settled, chxExact.Checked);    //CR1084
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void menuUnsettle_Click(object sender, EventArgs e)
        {
            unsettleAcct = true;
            try
            {
                AccountManager.UnarchiveUnsettle(ClickedAccount, ClickedArchived, unsettleAcct, out Error);

                if (Error.Length > 0)
                    ShowError(Error);

                //Need to re-load the customers accounts as unsettling will have unsettled and maybe unarchived the
                //account
                Search(txtCustID.Text, txtFirstName.Text, txtLastName.Text, txtAddress.Text, txtPhoneNo.Text, limit, settled, chxExact.Checked);      //CR1084
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void chkPhone_CheckedChanged(object sender, System.EventArgs e)
        {
            Function = "chkPhone_CheckedChanged";

            try
            {
                if (dgResults.TableStyles.Count > 0)
                {
                    if (chkPhone.Checked == true)
                    {
                        dgResults.TableStyles[0].GridColumnStyles["telno"].Width = 120;
                        dgResults.TableStyles[0].GridColumnStyles["telno"].Alignment = HorizontalAlignment.Left;
                    }
                    else
                        dgResults.TableStyles[0].GridColumnStyles["telno"].Width = 0;
                }

                Function = "End of chkPhone_CheckedChanged";
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void chkAddress_CheckedChanged(object sender, System.EventArgs e)
        {
            Function = "chkAddress_CheckedChanged";

            try
            {
                if (dgResults.TableStyles.Count > 0)
                {
                    if (chkAddress.Checked == true)
                    {
                        dgResults.TableStyles[0].GridColumnStyles["cusaddr1"].Width = 118;
                        dgResults.TableStyles[0].GridColumnStyles["cusaddr1"].Alignment = HorizontalAlignment.Left;
                        dgResults.TableStyles[0].GridColumnStyles["cusaddr2"].Width = 118;
                        dgResults.TableStyles[0].GridColumnStyles["cusaddr2"].Alignment = HorizontalAlignment.Left;
                        dgResults.TableStyles[0].GridColumnStyles["cusaddr3"].Width = 118;
                        dgResults.TableStyles[0].GridColumnStyles["cusaddr3"].Alignment = HorizontalAlignment.Left;
                        dgResults.TableStyles[0].GridColumnStyles["postcode"].Width = 60;
                        dgResults.TableStyles[0].GridColumnStyles["postcode"].Alignment = HorizontalAlignment.Left;
                    }
                    else
                    {
                        dgResults.TableStyles[0].GridColumnStyles["cusaddr1"].Width = 0;
                        dgResults.TableStyles[0].GridColumnStyles["cusaddr2"].Width = 0;
                        dgResults.TableStyles[0].GridColumnStyles["cusaddr3"].Width = 0;
                        dgResults.TableStyles[0].GridColumnStyles["postcode"].Width = 0;
                    }
                }

                Function = "End of chkAddress_CheckedChanged";
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        public void SetCustid(string custid)
        {
            txtCustID.Text = custid;
        }

        private void grpSearch_Enter(object sender, EventArgs e)
        {

        }

        //private void menuInstallationt_Click(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        Function = "menuInstallationt_Click";
        //        Wait();

        //        string custId = (string)dgResults[dgResults.CurrentRowIndex, 0];
        //        string acctNo = (string)dgResults[dgResults.CurrentRowIndex, 4];

        //        InstManagement installationForm = null;

        //        if (FormParent != null && !FormParent.IsDisposed && FormParent is InstManagement)
        //        {
        //            installationForm = (FormRoot as MainForm).GetIfExists(FormParent as InstManagement);
        //        }

        //        if (installationForm == null)
        //        {
        //            installationForm = new Installation.InstManagement(this.FormRoot, this.FormParent);
        //            ((MainForm)this.FormRoot).AddTabPage(installationForm);
        //        }

        //        installationForm.TriggerSearch(new WSInstallation.InstSearchParameter
        //        {
        //            AcctNo = acctNo,
        //            CustID = custId
        //        });

        //        (FormRoot as MainForm).FocusIfExists(installationForm);

        //        CloseTab(this);
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, Function);
        //    }
        //    finally
        //    {
        //        StopWait();
        //    }
        //}
    }
}

