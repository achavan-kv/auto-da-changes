using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Data;
using System.Web.Services.Protocols;
using System.Xml;
using System.Threading;
using STL.PL.WS1;
using STL.PL.WS2;
using STL.PL.WS3;
using STL.PL.WS4;
using STL.PL.WS5;
using Crownwood.Magic.Menus;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.AccountTypes;
using STL.Common.Static;
using Blue.Cosacs.Shared;
//using STL.PL.Installation;
namespace STL.PL
{
    /// <summary>
    /// Search screen to find an account. Either the account number can be entered
    /// if known, or the search can be on one of the customer id, first name or last
    /// name. When searching on the customer id, first name or last name an there is
    /// an option for an exact match or to match th leading characters in these fields.
    /// </summary>
    public class AccountSearch : CommonForm
    {
        private System.Windows.Forms.GroupBox groupBox3;
        private DataSet accounts;
        private DataView acctView;
        private System.Windows.Forms.DataGrid dgAccounts;
        private new string Error = String.Empty;
        private int searchClicked = 0;
        private int accountExists = 0;
        private string accountType = String.Empty;
        
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.CheckBox chkPhone;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkViewTop;
        private System.Windows.Forms.CheckBox chkSettledAccounts;
        private System.Windows.Forms.Button btnClear;
        private STL.PL.AccountTextBox txtAccountNo;
        private System.Windows.Forms.CheckBox chkExactMatch;
        private System.Windows.Forms.CheckBox chkAddress;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCustomerId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPostCode;
        //private Crownwood.Magic.Menus.MenuControl menuMain;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuClear;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private Control allowReviseSettled;
        private Blue.Cosacs.Shared.Customer Cust = new Blue.Cosacs.Shared.Customer();
        private bool _revise = false;
        public bool Revise
        {
            get { return _revise; }
            set { _revise = value; }
        }

        private bool _details = false;
        public bool Details
        {
            get { return _details; }
            set { _details = value; }
        }

        private bool _addCodes = false;
        public bool AddCodes
        {
            get { return _addCodes; }
            set { _addCodes = value; }
        }

        private string customerID = String.Empty;
        private string firstName = String.Empty;
        private string accountNo = String.Empty;
        private string lastName = String.Empty;
        private string address = String.Empty;
        private string phoneNumber = String.Empty;
        private string postCode = String.Empty;
        private string storeType = string.Empty;
        private bool settledAccounts;
        private bool exactMatch;
        private int limit;
        private Crownwood.Magic.Menus.MenuCommand menuRecent;
        private System.Windows.Forms.Label allowReviseRepo;
        //  private System.Windows.Forms.Label allowStore;
        private bool repo = false;
        private System.Windows.Forms.Label allowLocationChange;
        private Label allowReviseCash;
        private CheckBox chxStore;
        private MenuCommand menuSearch;
        private MenuCommand menuOptions;
        private MenuCommand menuServiceRequest;
        private MenuCommand menuReviseCashLoan;
        private Label label7;
        private TextBox txtPhoneNo;
        private Label allowStore;
        private IContainer components;

        public AccountSearch(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuOptions });
        }

        public AccountSearch()
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuOptions });
            HashMenus();
            ApplyRoleRestrictions();
        }

        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":allowReviseSettled"] = this.allowReviseSettled;
            dynamicMenus[this.Name + ":allowReviseRepo"] = this.allowReviseRepo;
            dynamicMenus[this.Name + ":allowReviseCash"] = this.allowReviseCash;
            dynamicMenus[this.Name + ":allowLocationChange"] = this.allowLocationChange;
            dynamicMenus[this.Name + ":allowStoreCard"] = this.allowStore;

            //IP - 03/12/2007 - UAT(199)
            dynamicMenus[this.Name + ":menuReviseCashLoan"] = this.menuReviseCashLoan;
            dynamicMenus[this.Name + ":menuService"] = this.menuServiceRequest; //jec 70566 //IP - 24/07/09 - Copied change across from 5.1
        }

        private void BuildRecentListMenus()
        {
            int i = 0;
            menuRecent.MenuCommands.Clear();
            string[] str = null;
            string[] accounts = new string[Recent.AccountList.Count];
            foreach (string s in Recent.AccountList)
                accounts[i++] = s;

            if (Revise)
            {
                /* need to retrieve the current status of each of the accounts in the list
                 * because if they are settled it is a restricted function to revise them */
                str = AccountManager.GetAccountStatuses(accounts, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
            }

            i = 0;
            foreach (string s in Recent.AccountList)
            {
                Crownwood.Magic.Menus.MenuCommand m = new Crownwood.Magic.Menus.MenuCommand(s);

                if (Revise && !allowReviseSettled.Enabled)
                    m.Enabled = !(str[i++] == "S");

                menuRecent.MenuCommands.Add(m);
                m.Click += new System.EventHandler(OnRecentClick);
            }
        }

        private void OnRecentClick(object sender, System.EventArgs e)
        {
            Crownwood.Magic.Menus.MenuCommand m = (Crownwood.Magic.Menus.MenuCommand)sender;
            string account = m.Text;

            if (Revise)
            {

                NewAccount reviseAcct = new NewAccount(account, 1, null, false, FormRoot, this);
                reviseAcct.Text = "Revise Sales Order";
                if (reviseAcct.AccountLocked && reviseAcct.AccountLoaded)
                {
                    ((MainForm)this.FormRoot).AddTabPage(reviseAcct, 6);
                    reviseAcct.SupressEvents = false;
                }
            }
            else
            {
                AccountDetails details = new AccountDetails(account, FormRoot, this);
                ((MainForm)this.FormRoot).AddTabPage(details, 7);
            }
        }

        public void InitData(string custID, string acctNo)
        {
            txtCustomerId.Text = custID;
            txtAccountNo.Text = acctNo;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountSearch));
            this.chkExactMatch = new System.Windows.Forms.CheckBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.allowStore = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txtPhoneNo = new System.Windows.Forms.TextBox();
            this.chxStore = new System.Windows.Forms.CheckBox();
            this.allowReviseCash = new System.Windows.Forms.Label();
            this.allowLocationChange = new System.Windows.Forms.Label();
            this.allowReviseRepo = new System.Windows.Forms.Label();
            this.chkViewTop = new System.Windows.Forms.CheckBox();
            this.chkSettledAccounts = new System.Windows.Forms.CheckBox();
            this.txtAccountNo = new STL.PL.AccountTextBox();
            this.chkPhone = new System.Windows.Forms.CheckBox();
            this.chkAddress = new System.Windows.Forms.CheckBox();
            this.txtPostCode = new System.Windows.Forms.TextBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCustomerId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dgAccounts = new System.Windows.Forms.DataGrid();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuClear = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuRecent = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSearch = new Crownwood.Magic.Menus.MenuCommand();
            this.allowReviseSettled = new System.Windows.Forms.Control();
            this.menuOptions = new Crownwood.Magic.Menus.MenuCommand();
            this.menuServiceRequest = new Crownwood.Magic.Menus.MenuCommand();
            this.menuReviseCashLoan = new Crownwood.Magic.Menus.MenuCommand();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // chkExactMatch
            // 
            this.chkExactMatch.CausesValidation = false;
            this.chkExactMatch.Checked = true;
            this.chkExactMatch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExactMatch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkExactMatch.Location = new System.Drawing.Point(496, 76);
            this.chkExactMatch.Name = "chkExactMatch";
            this.chkExactMatch.Size = new System.Drawing.Size(112, 24);
            this.chkExactMatch.TabIndex = 15;
            this.chkExactMatch.Text = "Exact Match";
            // 
            // btnClose
            // 
            this.btnClose.CausesValidation = false;
            this.btnClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClose.Location = new System.Drawing.Point(467, 124);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 27);
            this.btnClose.TabIndex = 19;
            this.btnClose.Text = "&Exit";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.CausesValidation = false;
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.btnClose);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtPhoneNo);
            this.groupBox1.Controls.Add(this.chxStore);
            this.groupBox1.Controls.Add(this.chkViewTop);
            this.groupBox1.Controls.Add(this.chkSettledAccounts);
            this.groupBox1.Controls.Add(this.txtAccountNo);
            this.groupBox1.Controls.Add(this.chkExactMatch);
            this.groupBox1.Controls.Add(this.chkPhone);
            this.groupBox1.Controls.Add(this.chkAddress);
            this.groupBox1.Controls.Add(this.txtPostCode);
            this.groupBox1.Controls.Add(this.txtAddress);
            this.groupBox1.Controls.Add(this.txtLastName);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtFirstName);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtCustomerId);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(8, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 169);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search Criteria";
            // 
            // allowStore
            // 
            this.allowStore.BackColor = System.Drawing.SystemColors.ControlDark;
            this.allowStore.Enabled = false;
            this.allowStore.Location = new System.Drawing.Point(106, 65);
            this.allowStore.Name = "allowStore";
            this.allowStore.Size = new System.Drawing.Size(24, 23);
            this.allowStore.TabIndex = 26;
            this.allowStore.Visible = false;
            // 
            // btnClear
            // 
            this.btnClear.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClear.Location = new System.Drawing.Point(339, 124);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 27);
            this.btnClear.TabIndex = 18;
            this.btnClear.Text = "&Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.CausesValidation = false;
            this.btnSearch.Enabled = false;
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSearch.Location = new System.Drawing.Point(211, 124);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 27);
            this.btnSearch.TabIndex = 17;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(678, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "Phone Number";
            // 
            // txtPhoneNo
            // 
            this.txtPhoneNo.Location = new System.Drawing.Point(681, 40);
            this.txtPhoneNo.Name = "txtPhoneNo";
            this.txtPhoneNo.Size = new System.Drawing.Size(81, 20);
            this.txtPhoneNo.TabIndex = 24;
            this.txtPhoneNo.TextChanged += new System.EventHandler(this.txtPhoneNo_TextChanged);
            // 
            // chxStore
            // 
            this.chxStore.Location = new System.Drawing.Point(56, 96);
            this.chxStore.Name = "chxStore";
            this.chxStore.Size = new System.Drawing.Size(206, 24);
            this.chxStore.TabIndex = 23;
            this.chxStore.Text = "Load Customers From Other Stores";
            // 
            // allowReviseCash
            // 
            this.allowReviseCash.BackColor = System.Drawing.SystemColors.ControlDark;
            this.allowReviseCash.Enabled = false;
            this.allowReviseCash.Location = new System.Drawing.Point(92, 29);
            this.allowReviseCash.Name = "allowReviseCash";
            this.allowReviseCash.Size = new System.Drawing.Size(24, 23);
            this.allowReviseCash.TabIndex = 22;
            this.allowReviseCash.Visible = false;
            // 
            // allowLocationChange
            // 
            this.allowLocationChange.BackColor = System.Drawing.SystemColors.ControlDark;
            this.allowLocationChange.Enabled = false;
            this.allowLocationChange.Location = new System.Drawing.Point(62, 29);
            this.allowLocationChange.Name = "allowLocationChange";
            this.allowLocationChange.Size = new System.Drawing.Size(24, 23);
            this.allowLocationChange.TabIndex = 21;
            this.allowLocationChange.Visible = false;
            // 
            // allowReviseRepo
            // 
            this.allowReviseRepo.BackColor = System.Drawing.SystemColors.ControlDark;
            this.allowReviseRepo.Enabled = false;
            this.allowReviseRepo.Location = new System.Drawing.Point(72, 29);
            this.allowReviseRepo.Name = "allowReviseRepo";
            this.allowReviseRepo.Size = new System.Drawing.Size(24, 23);
            this.allowReviseRepo.TabIndex = 20;
            this.allowReviseRepo.Visible = false;
            // 
            // chkViewTop
            // 
            this.chkViewTop.Checked = true;
            this.chkViewTop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkViewTop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkViewTop.Location = new System.Drawing.Point(616, 76);
            this.chkViewTop.Name = "chkViewTop";
            this.chkViewTop.Size = new System.Drawing.Size(96, 24);
            this.chkViewTop.TabIndex = 16;
            this.chkViewTop.Text = "View Top 200";
            // 
            // chkSettledAccounts
            // 
            this.chkSettledAccounts.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkSettledAccounts.Location = new System.Drawing.Point(328, 76);
            this.chkSettledAccounts.Name = "chkSettledAccounts";
            this.chkSettledAccounts.Size = new System.Drawing.Size(152, 24);
            this.chkSettledAccounts.TabIndex = 14;
            this.chkSettledAccounts.Text = "Include Settled Accounts";
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.Location = new System.Drawing.Point(8, 40);
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.PreventPaste = false;
            this.txtAccountNo.Size = new System.Drawing.Size(88, 20);
            this.txtAccountNo.TabIndex = 1;
            this.txtAccountNo.Tag = "ACCNO";
            this.txtAccountNo.Text = "000-0000-0000-0";
            this.txtAccountNo.TextChanged += new System.EventHandler(this.txtAccountNo_TextChanged);
            // 
            // chkPhone
            // 
            this.chkPhone.CausesValidation = false;
            this.chkPhone.Checked = true;
            this.chkPhone.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPhone.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkPhone.Location = new System.Drawing.Point(176, 76);
            this.chkPhone.Name = "chkPhone";
            this.chkPhone.Size = new System.Drawing.Size(136, 24);
            this.chkPhone.TabIndex = 13;
            this.chkPhone.Text = "View Phone Numbers";
            this.chkPhone.CheckedChanged += new System.EventHandler(this.chkPhone_CheckedChanged);
            // 
            // chkAddress
            // 
            this.chkAddress.CausesValidation = false;
            this.chkAddress.Checked = true;
            this.chkAddress.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAddress.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkAddress.Location = new System.Drawing.Point(56, 76);
            this.chkAddress.Name = "chkAddress";
            this.chkAddress.Size = new System.Drawing.Size(112, 24);
            this.chkAddress.TabIndex = 12;
            this.chkAddress.Text = "View Addresses";
            this.chkAddress.CheckedChanged += new System.EventHandler(this.chkAddress_CheckedChanged);
            // 
            // txtPostCode
            // 
            this.txtPostCode.BackColor = System.Drawing.SystemColors.Window;
            this.txtPostCode.Location = new System.Drawing.Point(584, 40);
            this.txtPostCode.MaxLength = 10;
            this.txtPostCode.Name = "txtPostCode";
            this.txtPostCode.Size = new System.Drawing.Size(88, 20);
            this.txtPostCode.TabIndex = 11;
            this.txtPostCode.Tag = "POSTCODE";
            this.txtPostCode.TextChanged += new System.EventHandler(this.txtPostCode_TextChanged);
            // 
            // txtAddress
            // 
            this.txtAddress.BackColor = System.Drawing.SystemColors.Window;
            this.txtAddress.Location = new System.Drawing.Point(442, 40);
            this.txtAddress.MaxLength = 26;
            this.txtAddress.Multiline = true;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(136, 20);
            this.txtAddress.TabIndex = 9;
            this.txtAddress.Tag = "ADD1";
            this.txtAddress.TextChanged += new System.EventHandler(this.txtAddress_TextChanged);
            // 
            // txtLastName
            // 
            this.txtLastName.BackColor = System.Drawing.SystemColors.Window;
            this.txtLastName.Location = new System.Drawing.Point(300, 40);
            this.txtLastName.MaxLength = 60;
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(136, 20);
            this.txtLastName.TabIndex = 7;
            this.txtLastName.Tag = "LNAME";
            this.txtLastName.TextChanged += new System.EventHandler(this.txtLastName_TextChanged);
            // 
            // label6
            // 
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(581, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 16);
            this.label6.TabIndex = 10;
            this.label6.Text = "Post Code:";
            // 
            // label5
            // 
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(442, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(136, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "Address:";
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(300, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Last Name:";
            // 
            // txtFirstName
            // 
            this.txtFirstName.Location = new System.Drawing.Point(206, 40);
            this.txtFirstName.MaxLength = 30;
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(88, 20);
            this.txtFirstName.TabIndex = 5;
            this.txtFirstName.Tag = "FNAME";
            this.txtFirstName.TextChanged += new System.EventHandler(this.txtFirstName_TextChanged);
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(206, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "First Name:";
            // 
            // txtCustomerId
            // 
            this.txtCustomerId.Location = new System.Drawing.Point(109, 40);
            this.txtCustomerId.MaxLength = 20;
            this.txtCustomerId.Name = "txtCustomerId";
            this.txtCustomerId.Size = new System.Drawing.Size(88, 20);
            this.txtCustomerId.TabIndex = 3;
            this.txtCustomerId.Tag = "CUSTID";
            this.txtCustomerId.TextChanged += new System.EventHandler(this.txtCustomerId_TextChanged);
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(109, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Customer ID:";
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Account Number:";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox3.CausesValidation = false;
            this.groupBox3.Controls.Add(this.dgAccounts);
            this.groupBox3.Controls.Add(this.allowStore);
            this.groupBox3.Controls.Add(this.allowReviseRepo);
            this.groupBox3.Controls.Add(this.allowLocationChange);
            this.groupBox3.Controls.Add(this.allowReviseCash);
            this.groupBox3.Location = new System.Drawing.Point(8, 157);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(776, 320);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Search Results";
            // 
            // dgAccounts
            // 
            this.dgAccounts.CaptionText = "Accounts";
            this.dgAccounts.DataMember = "";
            this.dgAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAccounts.Location = new System.Drawing.Point(8, 18);
            this.dgAccounts.Name = "dgAccounts";
            this.dgAccounts.ReadOnly = true;
            this.dgAccounts.Size = new System.Drawing.Size(760, 304);
            this.dgAccounts.TabIndex = 0;
            this.dgAccounts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgAccounts_MouseUp);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataMember = "";
            this.errorProvider1.Icon = ((System.Drawing.Icon)(resources.GetObject("errorProvider1.Icon")));
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuClear,
            this.menuExit,
            this.menuRecent,
            this.menuSearch});
            this.menuFile.Text = "&File";
            // 
            // menuClear
            // 
            this.menuClear.Description = "MenuItem";
            this.menuClear.Text = "&Clear";
            this.menuClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // menuRecent
            // 
            this.menuRecent.Description = "MenuItem";
            this.menuRecent.Text = "Recent &Accounts";
            // 
            // menuSearch
            // 
            this.menuSearch.Description = "MenuItem";
            this.menuSearch.Text = "&Search";
            this.menuSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // allowReviseSettled
            // 
            this.allowReviseSettled.Enabled = false;
            this.allowReviseSettled.Location = new System.Drawing.Point(0, 0);
            this.allowReviseSettled.Name = "allowReviseSettled";
            this.allowReviseSettled.Size = new System.Drawing.Size(0, 0);
            this.allowReviseSettled.TabIndex = 0;
            this.allowReviseSettled.Visible = false;
            // 
            // menuOptions
            // 
            this.menuOptions.Description = "MenuItem";
            this.menuOptions.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuServiceRequest});
            this.menuOptions.Text = "&Options";
            // 
            // menuServiceRequest
            // 
            //this.menuServiceRequest.Description = "MenuItem";
            //this.menuServiceRequest.Text = "Service Request";
            //this.menuServiceRequest.Click += new System.EventHandler(this.menuServiceRequest_Click);
            // 
            // menuReviseCashLoan
            // 
            this.menuReviseCashLoan.Description = "MenuItem";
            this.menuReviseCashLoan.Enabled = false;
            this.menuReviseCashLoan.Text = "Cash Loan";
            this.menuReviseCashLoan.Visible = false;
            // 
            // AccountSearch
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AccountSearch";
            this.ShowInTaskbar = false;
            this.Text = "Account Search";
            this.Enter += new System.EventHandler(this.AccountSearch_Enter);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.CloseTab();
        }

        public override bool ConfirmClose()
        {
            bool status = true;
            try
            {
                Function = "ConfirmClose()";
                Wait();

                int index = dgAccounts.CurrentRowIndex;
                DataView accts = (DataView)dgAccounts.DataSource;

                if (this.AddCodes)
                {
                    if (index >= 0)
                    {
                        ((AddCustAcctCodes)this.FormParent).txtCustID.Text = (string)accts[index]["Customer ID"];
                        ((AddCustAcctCodes)this.FormParent).txtFirstName.Text = (string)accts[index]["First Name"];
                        ((AddCustAcctCodes)this.FormParent).txtLastName.Text = (string)accts[index]["Last Name"];
                        ((AddCustAcctCodes)this.FormParent).txtAccountNumber.Text = FormatAccountNo((string)accts[index]["Account Number"]);
                    }
                }

                if (FormParent is Payment)
                {
                    if (index >= 0)
                        ((Payment)FormParent).LoadData((string)(dgAccounts[index, 0]), true);
                }

                if (FormParent is RefundAndCorrection)
                {
                    //IP - 08/02/10 - CR1037 Merged - Malaysia Enhancements (CR1072)
                    if (index >= 0)
                    {
                        //-- CR 1037 ----------------------------------------------------------------------
                        if ((bool)Country[CountryParameterNames.BlockCashGoInRefundCorrection] &&
                                AccountManager.IsPaidAndTakenAccount((string)accts[index]["Account Number"], out Error)) //CR 1037
                        {
                            ShowWarning(GetResource("M_PAID_AND_TAKEN_DISALLOWED"));
                            CloseTab((RefundAndCorrection)FormParent);
                            SearchCashAndGo search = new SearchCashAndGo(FormRoot, FormRoot);
                            ((MainForm)FormRoot).AddTabPage(search);
                        }
                        else
                        {
                            if (CheckLoyalty((string)accts[index]["Account Number"]))
                            {
                                ((RefundAndCorrection)FormParent).LoadData(FormatAccountNo((string)accts[index]["Account Number"]));
                            }
                        }
                    }
                }

                if (FormParent is GeneralFinancialTransactions)
                {
                    if (index >= 0 && CheckLoyalty((string)accts[index]["Account Number"]))
                    {
                        ((GeneralFinancialTransactions)FormParent).LoadData((string)accts[index]["Account Number"]);
                    }
                }

                //IP - 31/07/08 - CoSACS Improvements - Once an account has been selected
                //pass the selected account number into the 'LoadStatusData' method that belongs
                //to the 'StatusCode' class to load the details for the selected account.
                if (FormParent is StatusCode)
                {
                    if (index >= 0)
                        ((StatusCode)FormParent).LoadStatusData((string)accts[index]["Account Number"]);
                }

                //IP - 15/08/08 - CoSACS Improvement - Once an account has been selected
                //pass the selected account number into the 'LoadGoodsReturnData' method that belongs
                //to the 'GoodsReturn' class to load the details for the selected account.
                if (FormParent is GoodsReturn)
                {
                    if (index >= 0 && CheckLoyalty((string)accts[index]["Account Number"]))
                        ((GoodsReturn)FormParent).LoadGoodsReturnData((string)accts[index]["Account Number"]);

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

        private bool CheckLoyalty(string CheckAcctno)
        {
            bool check = true;
            
            if ((bool)(Country[CountryParameterNames.LoyaltyScheme]))
            {
                decimal amount = 0;
                string acctno = "";
                bool active = false;

                STL.PL.WS3.Loyalty loyaltyinfo = CustomerManager.LoyaltyGetDatabyacctno(CheckAcctno.Replace("-", ""));
                if (loyaltyinfo.custid != null && loyaltyinfo.custid.Length > 1)
                {
                    CustomerManager.LoyaltyGetCharges(loyaltyinfo.custid, ref acctno, ref amount, ref active);
                    if (acctno == CheckAcctno.Replace("-", ""))
                    {
                        ShowWarning("Finanical transactions on Home Club accounts are not allowed.");
                        check = false;
                    }
                }
            }

            return check;
        }

        private void Search()
        {
            customerID      = txtCustomerId.Text.Trim(); Cust.custid = customerID;
            firstName       = txtFirstName.Text.Trim(); Cust.firstname = firstName;
            accountNo       = txtAccountNo.Text.Replace("-", String.Empty);
            lastName        = txtLastName.Text.Trim(); Cust.name = lastName;
            address         = txtAddress.Text.Trim();
            postCode        = txtPostCode.Text.Trim();
            phoneNumber     = txtPhoneNo.Text.Trim();       //CR1084
            settledAccounts = chkSettledAccounts.Checked;
            exactMatch      = chkExactMatch.Checked;
            limit           = chkViewTop.Checked ? 200 : 1000;
            storeType       = chxStore.Checked ? "%" : Config.StoreType + "%";
            
            Thread dataThread = new Thread(new ThreadStart(SearchThread));
            dataThread.Start();
            dataThread.Join();
        }

        private void SearchThread()
        {
            try
            {
                Wait();
                accounts = AccountManager.AccountsSearch(accountNo,
                                                customerID,
                                                firstName,
                                                lastName,
                                                address,
                                                postCode,
                                                phoneNumber,        //CR1084
                                                settledAccounts,
                                                exactMatch,
                                                limit,
                                                storeType,
                                                out accountExists, 
                                                out accountType, 
                                                out Error);

                if (Error.Length > 0)
                    ShowError(Error);
            }
            catch (SoapException ex)
            {
                Catch(ex, Function);
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
 
        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            Function = "btnSearch_Click";

            try
            {
                Wait();

                // Increment click counter to reset the table if
                // the search button has been clicked previously.
                if (searchClicked > 0 && accounts != null)
                {
                    accounts.Clear();
                    accounts.Dispose();
                }

                ((MainForm)this.FormRoot).StatusBarText = "Searching for accounts";
                searchClicked++;

                Search();                    

                if (accountExists > 0)
                {
                    if (Details && !Revise)
                    {
                        AccountDetails details = new AccountDetails(txtAccountNo.Text.Replace("-", String.Empty), FormRoot, this);
                        ((MainForm)this.FormRoot).AddTabPage(details, 7);
                    }

                    if (Revise)
                    {
                        repo = AccountManager.IsRepossessed(txtAccountNo.UnformattedText, out Error);

                        //IP - 03/12/2007 - UAT(199)
                        //Retrieve details to check if the account is an 'IsLoan' account.
                        DataSet ds = AccountManager.GetAccountDetails(txtAccountNo.UnformattedText, out Error);
                        bool isLoan = Convert.ToBoolean(ds.Tables["AccountDetails"].Rows[0]["IsLoan"]);

                        // UAT 286 Check to see if this is a Service Charge-To account and if it is inform the user that this account cannot be revised
                        if (ds.Tables["AccountDetails"].Rows[0]["ChargeAcctNo"].ToString().Trim() != String.Empty)
                        {
                            ShowError("This account is a Service Charge-To account and cannot be revised.");
                            return;
                        }

                        DataSet ns = AccountManager.IsAccountValidForOnlyNonStockSale(txtAccountNo.UnformattedText, out Error);
                        if (ns != null)
                        {
                            bool isNonStock = Convert.ToBoolean(ns.Tables["AccountDetails"].Rows[0]["Isdel"]);
                            if (isNonStock)
                            {
                                ShowError("For this account only non stock item is sold and cannot be revised.");
                                return;
                            }
                        }
                        if (repo)
                        {
                            //uat289 rdb 18/04/08 clear status bar
                            ((MainForm)this.FormRoot).StatusBarText = "";
                            ShowInfo("M_REPO");
                        }

                       // if (isLoan && !menuReviseCashLoan.Enabled)
                        if (isLoan)
                        {
                            ShowInfo("M_REVISECASHLOAN");
                        }
                        else if (this.accountType == AT.Cash && !allowReviseCash.Enabled)
                        {
                            ShowInfo("M_REVISECASH");
                        }
                        else if (this.accountType == AT.StoreCard && !allowStore.Enabled)
                        {
                            ShowInfo("M_REVISESTORE");
                        }
                        else if (!repo || allowReviseRepo.Enabled)
                        {
                            if ((Convert.ToBoolean(Country[CountryParameterNames.LoyaltyScheme]) && CustomerManager.LoyaltyCheckLinkedAccount(txtAccountNo.Text.Replace("-", ""))))
                            {
                                MessageBox.Show("Loyalty accounts are system accounts can not be revised.", "Account can not be revised", MessageBoxButtons.OK);
                            }
                            else
                            {
                                if (accountType != AT.StoreCard)
                                {
                                    NewAccount reviseAcct = new NewAccount(txtAccountNo.Text.Replace("-", String.Empty), 1, null, false, FormRoot, this);
                                    reviseAcct.Text = "Revise Sales Order";
                                    if (reviseAcct.AccountLocked && reviseAcct.AccountLoaded)
                                    {
                                        ((MainForm)this.FormRoot).AddTabPage(reviseAcct, 6);
                                        reviseAcct.SupressEvents = false;
                                    }
                                }
                                else
                                {
                                    customerID = (string)ds.Tables["AccountDetails"].Rows[0]["Customer ID"];
                                    Cust.title = (string)ds.Tables["AccountDetails"].Rows[0]["Title"];
                                    Cust.custid = customerID;
                                    StoreCard.StoreCardAccount storeCardAccount = new StoreCard.StoreCardAccount(txtAccountNo.Text.Replace("-", String.Empty), FormRoot, this);
                                    storeCardAccount.Text = "Store Card";
                                    ((MainForm)this.FormRoot).AddTabPage(storeCardAccount, 6);

                                }
                            }
                        }
                    }

                    if (AddCodes)
                    {
                        DataSet ds = CustomerManager.GetCustomerAccountsAndDetails(txtAccountNo.Text.Replace("-", String.Empty), out Error);

                        if (Error.Length <= 0)
                        {
                            if (ds.Tables.Contains(TN.Customer))
                            {
                                var dtCust = ds.Tables[TN.Customer];
                                var addCustAcctCodes = FormParent as AddCustAcctCodes;
                                addCustAcctCodes.txtCustID.Text = (string)dtCust.Rows[0][CN.CustomerID];
                                addCustAcctCodes.txtFirstName.Text = (string)dtCust.Rows[0][CN.FirstName];
                                addCustAcctCodes.txtLastName.Text = (string)dtCust.Rows[0][CN.LastName];
                                CloseTab();
                            }
                        }
                        else
                            ShowError(Error);
                    }
                }
                else   //more than one account returned
                {
                    if (accounts.Tables.Contains(TN.Accounts))
                    {
                        var dt = accounts.Tables[TN.Accounts];
                        acctView = new DataView(dt);

                        if (acctView.Count <= 0)
                            ((MainForm)this.FormRoot).StatusBarText = GetResource("M_NOACCOUNT");
                        else
                        {
                            ((MainForm)this.FormRoot).StatusBarText = acctView.Count.ToString() + " rows returned";
                            dgAccounts.DataSource = acctView;
                            if (dgAccounts.TableStyles.Count == 0)
                            {
                                DataGridTableStyle tabStyle = new DataGridTableStyle();
                                tabStyle.MappingName = dt.TableName;
                                dgAccounts.TableStyles.Add(tabStyle);

                                // Set the table style according to the user's preference
                                tabStyle.GridColumnStyles["Customer ID"].Width = 90;
                                tabStyle.GridColumnStyles["Customer ID"].HeaderText = GetResource("T_CUSTID");
                                tabStyle.GridColumnStyles["Customer ID"].Alignment = HorizontalAlignment.Left;
                                tabStyle.GridColumnStyles["Account Number"].Width = 90;
                                tabStyle.GridColumnStyles["Account Number"].HeaderText = GetResource("T_ACCTNO");
                                tabStyle.GridColumnStyles["Account Number"].Alignment = HorizontalAlignment.Left;
                                tabStyle.GridColumnStyles["First Name"].Width = 60;
                                tabStyle.GridColumnStyles["First Name"].HeaderText = GetResource("T_FIRSTNAME");
                                tabStyle.GridColumnStyles["First Name"].Alignment = HorizontalAlignment.Left;
                                tabStyle.GridColumnStyles["Last Name"].Width = 118;
                                tabStyle.GridColumnStyles["Last Name"].HeaderText = GetResource("T_LASTNAME");
                                tabStyle.GridColumnStyles["Last Name"].Alignment = HorizontalAlignment.Left;
                                tabStyle.GridColumnStyles["Account Type"].Width = 40;
                                tabStyle.GridColumnStyles["Account Type"].HeaderText = GetResource("T_TYPE");
                                tabStyle.GridColumnStyles["Account Type"].Alignment = HorizontalAlignment.Left;
                                tabStyle.GridColumnStyles["Address"].HeaderText = GetResource("T_ADDRESS");
                                tabStyle.GridColumnStyles["Post Code"].HeaderText = GetResource("T_POSTCODE");
                                tabStyle.GridColumnStyles["Phone"].HeaderText = GetResource("T_PHONE");

                                if (chkAddress.Checked == true)
                                {
                                    dgAccounts.TableStyles[0].GridColumnStyles["Address"].Width = 118;
                                    dgAccounts.TableStyles[0].GridColumnStyles["Post Code"].Alignment = HorizontalAlignment.Left;
                                    dgAccounts.TableStyles[0].GridColumnStyles["Post Code"].Width = 60;
                                    dgAccounts.TableStyles[0].GridColumnStyles["Post Code"].Alignment = HorizontalAlignment.Left;
                                }
                                else
                                {
                                    tabStyle.GridColumnStyles["Address"].Width = 0;
                                    tabStyle.GridColumnStyles["Post Code"].Width = 0;
                                }
                                if (chkPhone.Checked == true)
                                {
                                    dgAccounts.TableStyles[0].GridColumnStyles["Phone"].Width = 120;
                                    dgAccounts.TableStyles[0].GridColumnStyles["Phone"].Alignment = HorizontalAlignment.Left;
                                }
                                else
                                {
                                    tabStyle.GridColumnStyles["Phone"].Width = 0;
                                }

                                tabStyle.GridColumnStyles["HldOrJnt"].Width = 90;
                                tabStyle.GridColumnStyles["HldOrJnt"].HeaderText = GetResource("T_HLDJNT");
                                tabStyle.GridColumnStyles["HldOrJnt"].Alignment = HorizontalAlignment.Left;
                                tabStyle.GridColumnStyles["Status"].Width = 40;
                                tabStyle.GridColumnStyles["Status"].HeaderText = GetResource("T_STATUS");
                                tabStyle.GridColumnStyles["Status"].Alignment = HorizontalAlignment.Left;
                                tabStyle.GridColumnStyles["HCmemberno"].HeaderText = GetResource("T_HCMember");
                                tabStyle.GridColumnStyles["HCmemberno"].Alignment = HorizontalAlignment.Left;
                                if (Convert.ToBoolean(Country[CountryParameterNames.LoyaltyScheme]))
                                {
                                    tabStyle.GridColumnStyles["HCmemberno"].Width = 120;
                                }
                                else
                                {
                                    tabStyle.GridColumnStyles["HCmemberno"].Width = 0;
                                }
                                tabStyle.GridColumnStyles["Email"].Width = 0;
                                tabStyle.GridColumnStyles["Title"].Width = 0;
                                tabStyle.GridColumnStyles["DOB"].Width = 0;
                            }
                        }                    
                    }
                }
                Function = "End of btnSearch_Click";
            }
            catch (SoapException ex)
            {
                Catch(ex, Function);
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

        private void chkAddress_CheckedChanged(object sender, System.EventArgs e)
        {
            Function = "chkAddress_CheckedChanged";

            try
            {
                if (dgAccounts.TableStyles.Count > 0)
                {
                    if (chkAddress.Checked == true)
                    {
                        dgAccounts.TableStyles[0].GridColumnStyles["Address"].Width = 118;
                        dgAccounts.TableStyles[0].GridColumnStyles["Address"].Alignment = HorizontalAlignment.Left;
                        dgAccounts.TableStyles[0].GridColumnStyles["Post Code"].Width = 60;
                        dgAccounts.TableStyles[0].GridColumnStyles["Post Code"].Alignment = HorizontalAlignment.Left;
                    }
                    else
                    {
                        dgAccounts.TableStyles[0].GridColumnStyles["Address"].Width = 0;
                        dgAccounts.TableStyles[0].GridColumnStyles["Post Code"].Width = 0;
                    }
                }

                Function = "End of chkAddress_CheckedChanged";
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void chkPhone_CheckedChanged(object sender, System.EventArgs e)
        {
            Function = "chkPhone_CheckedChanged";

            try
            {
                if (dgAccounts.TableStyles.Count > 0)
                {
                    if (chkPhone.Checked == true)
                    {
                        dgAccounts.TableStyles[0].GridColumnStyles["Phone"].Width = 120;
                        dgAccounts.TableStyles[0].GridColumnStyles["Phone"].Alignment = HorizontalAlignment.Left;
                    }
                    else
                        dgAccounts.TableStyles[0].GridColumnStyles["Phone"].Width = 0;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            Function = "btnClear_Click";

            try
            {
                if (searchClicked != 0 && accounts != null)
                {
                    accounts.Clear();
                    accounts.Dispose();
                }
                ((MainForm)this.FormRoot).statusBar1.Text = "";
                txtAccountNo.Text = "000-0000-0000-0";
                txtCustomerId.Text = "";
                txtFirstName.Text = "";
                txtLastName.Text = "";
                txtAddress.Text = "";
                txtPhoneNo.Text = "";       //CR1084
                txtPostCode.Text = "";
                btnSearch.Enabled = false;    //CR1084 
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        /// <summary>
        /// dgAccounts Context Menu, "Revise Agreement", calls this screen.
        /// Details of this account are displayed for editing.
        /// </summary>
        private void menuItem2_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                int index = dgAccounts.CurrentRowIndex;

                if (index >= 0)
                {
                    string newAcNo = (string)dgAccounts[dgAccounts.CurrentRowIndex, 1];
                    repo = AccountManager.IsRepossessed(newAcNo, out Error);

                    //IP - 08/07/10 - UAT 995 - UAT5.2 
                    DataSet ds = AccountManager.GetAccountDetails(newAcNo, out Error);
                    bool isLoan = Convert.ToBoolean(ds.Tables["AccountDetails"].Rows[0]["IsLoan"]);
                    accountType = (string)ds.Tables["AccountDetails"].Rows[0]["Account Type"];
                    customerID = (string)ds.Tables["AccountDetails"].Rows[0]["Customer ID"];
                    Cust.title = (string)ds.Tables["AccountDetails"].Rows[0]["Title"];
                    Cust.firstname = (string)ds.Tables["AccountDetails"].Rows[0]["First Name"];
                    Cust.name = (string)ds.Tables["AccountDetails"].Rows[0]["Last Name"];
                    if (isLoan && !menuReviseCashLoan.Enabled)
                    {
                        ShowInfo("M_REVISECASHLOAN");
                        return;
                    }

                    if (repo)
                    {
                        //uat289 rdb 18/04/08 clear status bar
                        ((MainForm)this.FormRoot).statusBar1.Text = "";
                        ShowInfo("M_REPO");
                    }

                    if (!repo || allowReviseRepo.Enabled)
                    {
                        if (accountType != AT.StoreCard)
                        {
                            NewAccount reviseAcct = new NewAccount(newAcNo, 1, null, false, FormRoot, this);
                            reviseAcct.Text = "Revise Sales Order";
                            ((MainForm)this.FormRoot).AddTabPage(reviseAcct, 6);
                            reviseAcct.SupressEvents = false;
                            if (!reviseAcct.AccountLocked)
                            {
                                reviseAcct.Confirm = false;
                                reviseAcct.CloseTab();
                            }
                        }
                        else
                        {
                            var auth = new AuthoriseCheck("MainForm", "menuViewStoreCard", "StoreCard viewing is not permitted. Please authorise with supervisor or cancel.");
                            if (!auth.ControlPermissionCheck(Credential.User).HasValue)
                                auth.ShowDialog();
                            
                            if (auth.IsAuthorised)
                                {
                                    StoreCard.StoreCardAccount storeCardAccount = new StoreCard.StoreCardAccount(newAcNo, FormRoot, this);
                                    storeCardAccount.Text = "Store Card";
                                    ((MainForm)this.FormRoot).AddTabPage(storeCardAccount, 6);
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

        /// <summary>
        /// dgAccounts Context Menu, "Account Details", calls this screen.
        /// Details of the selected account are displayed.
        /// </summary>
        private void menuItem3_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                if (dgAccounts.CurrentRowIndex >= 0)
                {
                    string newAcNo = (string)dgAccounts[dgAccounts.CurrentRowIndex, 1];  // non-formatted account number                    
                    ((MainForm)this.FormRoot).AddTabPage(new AccountDetails(newAcNo, FormRoot, this), 7);
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

        //private void cmenuServiceRequest_Click(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        Function = "cmenuServiceRequest_Click";
        //        Wait();

        //        if (dgAccounts.CurrentRowIndex >= 0)
        //        {
        //            // Open the Service Request screen	
        //            string acctNo = (string)dgAccounts[dgAccounts.CurrentRowIndex, 1];  // non-formatted account number
        //            SERVICE.SR_ServiceRequest serviceRequest = new SERVICE.SR_ServiceRequest(this.FormRoot, this.FormParent, String.Empty, acctNo, String.Empty);
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

        //private void menuServiceRequest_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Function = "menuServiceRequest_Click";
        //        Wait();

        //        string acctNo = this.txtAccountNo.Text;
        //        if (dgAccounts.CurrentRowIndex >= 0)
        //        {
        //            acctNo = (string)dgAccounts[dgAccounts.CurrentRowIndex, 1];  // non-formatted account number
        //        }
        //        // Open the Service Request screen	
        //        SERVICE.SR_ServiceRequest serviceRequest = new SERVICE.SR_ServiceRequest(this.FormRoot, this.FormParent, String.Empty, acctNo, String.Empty);
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

        private void dgAccounts_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Function = "dgAccounts_MouseUp";
                Wait();

                if (dgAccounts.CurrentRowIndex >= 0)
                {
                    dgAccounts.Select(dgAccounts.CurrentCell.RowNumber);
                    string acctType = (string)dgAccounts[dgAccounts.CurrentRowIndex, 7];
                    string stat = (string)dgAccounts[dgAccounts.CurrentRowIndex, 2];

                    string accountNo = (string)dgAccounts[dgAccounts.CurrentRowIndex, 1];
                    string customerID = (string)dgAccounts[dgAccounts.CurrentRowIndex, 0];
                    bool hasInstallations = Convert.ToBoolean(dgAccounts[dgAccounts.CurrentRowIndex, 14]);
                    
                    if (e.Button == MouseButtons.Right)
                    {
                        DataGrid ctl = (DataGrid)sender;

                        MenuCommand m1 = new MenuCommand(GetResource("P_REVISE_ACCOUNT"));
                        MenuCommand m2 = new MenuCommand(GetResource("P_ACCOUNT_DETAILS"));
                        //MenuCommand m3 = new MenuCommand(GetResource("P_SERVICE_REQUEST"));                        

                        m1.Click += new System.EventHandler(this.menuItem2_Click);
                        m2.Click += new System.EventHandler(this.menuItem3_Click);
                        //m3.Click += new System.EventHandler(this.cmenuServiceRequest_Click);

                        // set permission for service request        jec 70566 //IP - 24/07/09 - Copied change across from 5.1
                        //m3.Enabled = menuServiceRequest.Enabled;
                        //m3.Visible = menuServiceRequest.Visible;

                        PopupMenu popup = new PopupMenu();

                        if ((stat == "0" || stat == "S") && acctType == AT.ReadyFinance)
                            m1.Enabled = false;

                        // Revision of Cash accounts is a user right
                        if (this.Revise && (allowReviseCash.Enabled || AT.IsCreditType(acctType))) //Acct Type Translation DSR 29/9/03
                            popup.MenuCommands.Add(m1);
                        else
                            if (allowStore.Enabled && acctType == AT.StoreCard)
                                popup.MenuCommands.Add(m1);

                        if (this.Details)
                            popup.MenuCommands.Add(m2);
                        //if (acctType != AT.StoreCard) //no service request for storecard
                        //    popup.MenuCommands.Add(m3);
                        if (Config.ThermalPrintingEnabled)
                        {
                            MenuCommand menuCommandStatement = new MenuCommand("Statement");
                            menuCommandStatement.Visible = menuCommandStatement.Enabled = true;
                            menuCommandStatement.Click +=
                                delegate(object ssender, EventArgs ee)
                                {
                                    ((MainForm)this.FormRoot).OpenStatementsTab(accountNo, customerID);
                                };
                            popup.MenuCommands.Add(menuCommandStatement);
                        }

                        //if (hasInstallations)
                        //{
                        //    MenuCommand menuInstallation = new MenuCommand(GetResource("P_INSTALLATION"));
                        //    menuInstallation.Click += new System.EventHandler(menuInstallationt_Click);
                        //    popup.MenuCommands.Add(menuInstallation);
                        //}

                        // #8489 jec 01/11/11   cash loan accounts cannot be revised
                        var accounts = (DataView)dgAccounts.DataSource;
                        if (Convert.ToBoolean(accounts[dgAccounts.CurrentRowIndex]["IsLoan"]) == true)
                        {
                            if (popup.MenuCommands.Contains(m1))
                                  popup.MenuCommands.Remove(m1);
                        } 

                        MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                    }
                    else
                    {
                        string str = (string)dgAccounts[dgAccounts.CurrentRowIndex, 1];
                        CommonForm.FormatAccountNo(ref str);
                        txtAccountNo.Text = str;
                        txtCustomerId.Text = (string)dgAccounts[dgAccounts.CurrentRowIndex, 0];
                        txtFirstName.Text = (string)dgAccounts[dgAccounts.CurrentRowIndex, 4];
                        txtLastName.Text = (string)dgAccounts[dgAccounts.CurrentRowIndex, 5];
                        txtAddress.Text = "";
                        if (dgAccounts[dgAccounts.CurrentRowIndex, 9] != DBNull.Value)
                            txtPostCode.Text = (string)dgAccounts[dgAccounts.CurrentRowIndex, 9];       //CR1084 //IP - 27/09/10 UAT(38) 5.4
                        if (dgAccounts[dgAccounts.CurrentRowIndex, 10] != DBNull.Value)
                            txtPhoneNo.Text = (string)dgAccounts[dgAccounts.CurrentRowIndex, 10];       //CR1084 //IP - 27/09/10 UAT(38) 5.4
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
                Function = "End of dgAccounts_MouseUp";
            }
        }

        private void txtAccountNo_TextChanged(object sender, System.EventArgs e)
        {
            //if (!btnSearch.Enabled)
            //    btnSearch.Enabled = true;     //CR1084 jec 18/08/10
            SearchTextChanged();
        }

        private void txtCustomerId_TextChanged(object sender, System.EventArgs e)
        {
            //if (!btnSearch.Enabled)
            //    btnSearch.Enabled = true;     //CR1084 jec 18/08/10
            SearchTextChanged();
        }

        private void txtLastName_TextChanged(object sender, System.EventArgs e)
        {
            //if (!btnSearch.Enabled && txtLastName.Text.Length !=0)
            //    btnSearch.Enabled = true;     //CR1084 jec 18/08/10
            SearchTextChanged();
        }

        private void txtFirstName_TextChanged(object sender, System.EventArgs e)
        {
            //if (txtCustomerId.Text.Length == 0 && txtLastName.Text.Length == 0 && txtAccountNo.Text == "000-0000-0000-0")
            //    btnSearch.Enabled = false;        //CR1084 jec 18/08/10
            SearchTextChanged();
        }

        private void AccountSearch_Enter(object sender, System.EventArgs e)
        {
            BuildRecentListMenus();
            if (Revise)
            {
                chkSettledAccounts.Checked = false;
                chkSettledAccounts.Enabled = allowReviseSettled.Enabled;
            }
        }

        private void txtAddress_TextChanged(object sender, EventArgs e)
        {
            SearchTextChanged();
        }

        private void txtPhoneNo_TextChanged(object sender, EventArgs e)
        {
            SearchTextChanged();
        }
        //CR1084 jec 18/08/10
        private void SearchTextChanged()
        {
            if ((txtAccountNo.Text.Length > 0 && txtAccountNo.Text != "000-0000-0000-0")
                || txtCustomerId.Text.Trim().Length > 0 || txtLastName.Text.Trim().Length > 0
                || txtFirstName.Text.Trim().Length > 0 || txtAddress.Text.Trim().Length > 0 || txtPhoneNo.Text.Trim().Length > 0 || txtPostCode.Text.Trim().Length > 0)
            {
                btnSearch.Enabled = true;
            }
            else
            {
                btnSearch.Enabled = false;
            }
        }

        private void txtPostCode_TextChanged(object sender, EventArgs e)
        {
            SearchTextChanged();
        }

        //private void menuInstallationt_Click(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        Function = "menuInstallationt_Click";
        //        Wait();

        //        InstManagement installationForm = null;

        //        if (FormParent != null && !FormParent.IsDisposed && FormParent is InstManagement)
        //        {
        //            installationForm = (FormRoot as MainForm).GetIfExists(FormParent as InstManagement);
        //        }
                
        //        if(installationForm == null)
        //        {
        //            installationForm = new Installation.InstManagement(this.FormRoot, this.FormParent);
        //            ((MainForm)this.FormRoot).AddTabPage(installationForm);
        //        }
                
        //        installationForm.TriggerSearch(new WSInstallation.InstSearchParameter 
        //        { 
        //            AcctNo = dgAccounts.CurrentRowIndex >= 0 ? 
        //                            Convert.ToString(dgAccounts[dgAccounts.CurrentRowIndex, 1]) : txtAccountNo.Text
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

