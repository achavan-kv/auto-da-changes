using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using STL.Common;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.SanctionStages;
using STL.Common.Constants.TableNames;
using STL.Common.Static;

namespace STL.PL
{
    /// <summary>
    /// When a customer opens a hire purchase or Ready Finanance account
    /// additional customer details are entered to check the customer is
    /// credit worthy. These details are split into a number of stages.
    /// The first stage will score the customer and either sanction a credit
    /// limit or decline the customer for credit. Document Confirmation gathers
    /// information that can be useful to follow up an account that goes
    /// into arrears. This stage captures that certain documents have been
    /// presented for verification. These documents include proof of id,
    /// proof of employment and proof of address. All stages have to be
    /// completed before the goods can be authorised for delivery.
    /// </summary>
    public class DocumentConfirmation : CommonForm
    {
        private string EmploymentStatus = "";
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnComplete;
        private System.Windows.Forms.GroupBox gpCustomer;
        private System.Windows.Forms.TextBox txtCustomerID;
        private System.Windows.Forms.DateTimePicker dtDateProp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lCustomerID;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolTip toolTip1;
        //private Crownwood.Magic.Menus.MenuControl menuMain;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuDocConfirmation;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private Crownwood.Magic.Menus.MenuCommand menuSave;
        private Crownwood.Magic.Menus.MenuCommand menuComplete;
        private System.ComponentModel.IContainer components;
        private DataSet propData = null;
        private bool valid = false;

        private new string Error = "";

        private string _custid = "";
        public string CustomerID
        {
            get { return _custid; }
            set { _custid = value; }
        }

        private string _currentStatus = "";
        public string CurrentStatus
        {
            get { return _currentStatus; }
            set { _currentStatus = value; }
        }

        private string _acctno = "";
        public string AccountNo
        {
            get { return _acctno; }
            set { _acctno = value; }
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

        private bool _complete = false;
        private DateTime _dateProp;
        private System.Windows.Forms.ComboBox drpAddress;
        private System.Windows.Forms.ComboBox drpIncome;
        private System.Windows.Forms.ComboBox drpID;

        public DateTime DateProp
        {
            get { return _dateProp; }
            set { _dateProp = value; }
        }

        private bool _acctLocked = false;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private Crownwood.Magic.Menus.MenuCommand menuReopen;

        public bool AccountLocked
        {
            get { return _acctLocked; }
            set { _acctLocked = value; }
        }

        //IP - 09/01/12 - #3811
        public bool createHPAcct
        {
            get;
            set;
        }

        private bool _readOnly = false;
        private System.Windows.Forms.TextBox txtDC2;
        private System.Windows.Forms.TextBox txtDC3;
        private System.Windows.Forms.TextBox txtDC1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label viewPrevious;
        private System.Windows.Forms.Label lPrevIncome;
        private System.Windows.Forms.ComboBox drpPrevIncome;
        private System.Windows.Forms.Label lPrevAddress;
        private System.Windows.Forms.ComboBox drpPrevAddress;
        private System.Windows.Forms.TextBox txtPrevDC3;
        private System.Windows.Forms.TextBox txtPrevDC2;
        private System.Windows.Forms.Label lPrevAddresstxt;
        private System.Windows.Forms.Label lPrevIncometxt;
        private System.Windows.Forms.ImageList menuIcons;

        public bool ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; }
        }

        private BasicCustomerDetails CustomerScreen = null;
        private Label lblProofOfBankNotes;
        private TextBox txtProofOfBank;
        private Label lblProofOfBank;
        private ComboBox drpProofOfBank;

        private NewAccount NewAccountScreen = null;
        private STL.PL.CashLoan.CashLoanApplication CashLoanScreen = null;

        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":menuReopen"] = this.menuReopen;
            dynamicMenus[this.Name + ":viewPrevious"] = this.viewPrevious;
        }

        public DocumentConfirmation(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuDocConfirmation });
        }


        public DocumentConfirmation(string custId, DateTime dateProp, string accountNo,
            string acctType, string mode,
            Form root, Form parent, BasicCustomerDetails customerScreen)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            CustomerScreen = customerScreen;

            HashMenus();
            this.ApplyRoleRestrictions();

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuDocConfirmation });
            txtCustomerID.Text = this.CustomerID = custId;
            dtDateProp.Value = this.DateProp = dateProp;
            this.AccountNo = accountNo;
            this.AccountType = acctType;
            this.ScreenMode = mode;
            //TranslateControls();

            //IP - 09/01/12 - #3811
            if (customerScreen == null)
            {
                CanCreateHPAcctForBranch(accountNo.Substring(0, 3));
            }
        }

        //CR907 - this method replicated from above for launch of DC from New Account screen
        // when account approved for instant credit - jec 24/08/07
        public DocumentConfirmation(string custId, DateTime dateProp, string accountNo,
            string acctType, string mode,
            Form root, Form parent, NewAccount newAccountScreen)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            NewAccountScreen = newAccountScreen;

            HashMenus();
            this.ApplyRoleRestrictions();

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuDocConfirmation });
            txtCustomerID.Text = this.CustomerID = custId;
            dtDateProp.Value = this.DateProp = dateProp;
            this.AccountNo = accountNo;
            this.AccountType = acctType;
            this.ScreenMode = mode;
            //TranslateControls();

            CanCreateHPAcctForBranch(accountNo.Substring(0, 3));                    //IP - 09/01/12 - #3811
        }

        //CR1232 - this method replicated from above for launch of DC from Cash Loan screen - jec 24/10/07
        // when account approved for instant credit - jec 24/08/07
        public DocumentConfirmation(string custId, DateTime dateProp, string accountNo,
            string acctType, string mode,
            Form root, Form parent, STL.PL.CashLoan.CashLoanApplication cashLoanScreen)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            CashLoanScreen = cashLoanScreen;

            HashMenus();
            this.ApplyRoleRestrictions();

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuDocConfirmation });
            txtCustomerID.Text = this.CustomerID = custId;
            dtDateProp.Value = this.DateProp = dateProp;
            this.AccountNo = accountNo;
            this.AccountType = acctType;
            this.ScreenMode = mode;
            //TranslateControls();

            CanCreateHPAcctForBranch(accountNo.Substring(0, 3));                    //IP - 09/01/12 - #3811
        }

        private void SetReadOnly()
        {
            this.txtCustomerID.BackColor = SystemColors.Window;
            this.txtFirstName.BackColor = SystemColors.Window;
            this.txtLastName.BackColor = SystemColors.Window;
            drpID.Enabled = !ReadOnly;
            drpIncome.Enabled = !ReadOnly;
            drpAddress.Enabled = !ReadOnly;
            this.btnComplete.Enabled = !ReadOnly;
            this.menuComplete.Enabled = !ReadOnly;
            this.menuSave.Enabled = !ReadOnly;
            txtDC1.Enabled = !ReadOnly;
            txtDC2.Enabled = !ReadOnly;
            txtDC3.Enabled = !ReadOnly;

            //IP - 14/12/10 - Store Card - drpProofOfBank drop down will only be visible for a Store Card account.
            if (drpProofOfBank.Visible == true)
            {
                drpProofOfBank.Enabled = !ReadOnly;
                txtProofOfBank.Enabled = !ReadOnly;

                if (!txtProofOfBank.Enabled)
                {
                    txtProofOfBank.Enabled = true;
                    txtProofOfBank.ReadOnly = true;
                }
            }

            if (!txtDC1.Enabled)
            {
                txtDC1.Enabled = true;
                txtDC1.ReadOnly = true;
            }
            else
                txtDC1.ReadOnly = false;

            if (!txtDC2.Enabled)
            {
                txtDC2.Enabled = true;
                txtDC2.ReadOnly = true;
            }
            else
                txtDC2.ReadOnly = false;

            if (!txtDC3.Enabled)
            {
                txtDC3.Enabled = true;
                txtDC3.ReadOnly = true;
            }
            else
                txtDC3.ReadOnly = false;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentConfirmation));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblProofOfBankNotes = new System.Windows.Forms.Label();
            this.txtProofOfBank = new System.Windows.Forms.TextBox();
            this.lblProofOfBank = new System.Windows.Forms.Label();
            this.drpProofOfBank = new System.Windows.Forms.ComboBox();
            this.lPrevIncometxt = new System.Windows.Forms.Label();
            this.lPrevAddresstxt = new System.Windows.Forms.Label();
            this.txtPrevDC2 = new System.Windows.Forms.TextBox();
            this.txtPrevDC3 = new System.Windows.Forms.TextBox();
            this.viewPrevious = new System.Windows.Forms.Label();
            this.lPrevIncome = new System.Windows.Forms.Label();
            this.drpPrevIncome = new System.Windows.Forms.ComboBox();
            this.lPrevAddress = new System.Windows.Forms.Label();
            this.drpPrevAddress = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDC1 = new System.Windows.Forms.TextBox();
            this.txtDC3 = new System.Windows.Forms.TextBox();
            this.txtDC2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.drpAddress = new System.Windows.Forms.ComboBox();
            this.drpIncome = new System.Windows.Forms.ComboBox();
            this.drpID = new System.Windows.Forms.ComboBox();
            this.btnComplete = new System.Windows.Forms.Button();
            this.menuIcons = new System.Windows.Forms.ImageList(this.components);
            this.gpCustomer = new System.Windows.Forms.GroupBox();
            this.txtCustomerID = new System.Windows.Forms.TextBox();
            this.dtDateProp = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lCustomerID = new System.Windows.Forms.Label();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuDocConfirmation = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
            this.menuComplete = new Crownwood.Magic.Menus.MenuCommand();
            this.menuReopen = new Crownwood.Magic.Menus.MenuCommand();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            this.gpCustomer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.lblProofOfBankNotes);
            this.groupBox1.Controls.Add(this.txtProofOfBank);
            this.groupBox1.Controls.Add(this.lblProofOfBank);
            this.groupBox1.Controls.Add(this.drpProofOfBank);
            this.groupBox1.Controls.Add(this.lPrevIncometxt);
            this.groupBox1.Controls.Add(this.lPrevAddresstxt);
            this.groupBox1.Controls.Add(this.txtPrevDC2);
            this.groupBox1.Controls.Add(this.txtPrevDC3);
            this.groupBox1.Controls.Add(this.viewPrevious);
            this.groupBox1.Controls.Add(this.lPrevIncome);
            this.groupBox1.Controls.Add(this.drpPrevIncome);
            this.groupBox1.Controls.Add(this.lPrevAddress);
            this.groupBox1.Controls.Add(this.drpPrevAddress);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtDC1);
            this.groupBox1.Controls.Add(this.txtDC3);
            this.groupBox1.Controls.Add(this.txtDC2);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.drpAddress);
            this.groupBox1.Controls.Add(this.drpIncome);
            this.groupBox1.Controls.Add(this.drpID);
            this.groupBox1.Controls.Add(this.btnComplete);
            this.groupBox1.Location = new System.Drawing.Point(8, 82);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 392);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Document Confirmation";
            // 
            // lblProofOfBankNotes
            // 
            this.lblProofOfBankNotes.Location = new System.Drawing.Point(368, 214);
            this.lblProofOfBankNotes.Name = "lblProofOfBankNotes";
            this.lblProofOfBankNotes.Size = new System.Drawing.Size(100, 16);
            this.lblProofOfBankNotes.TabIndex = 45;
            this.lblProofOfBankNotes.Text = "Bank Notes:";
            // 
            // txtProofOfBank
            // 
            this.txtProofOfBank.Location = new System.Drawing.Point(368, 231);
            this.txtProofOfBank.MaxLength = 350;
            this.txtProofOfBank.Multiline = true;
            this.txtProofOfBank.Name = "txtProofOfBank";
            this.txtProofOfBank.Size = new System.Drawing.Size(288, 40);
            this.txtProofOfBank.TabIndex = 44;
            // 
            // lblProofOfBank
            // 
            this.lblProofOfBank.Location = new System.Drawing.Point(112, 212);
            this.lblProofOfBank.Name = "lblProofOfBank";
            this.lblProofOfBank.Size = new System.Drawing.Size(100, 16);
            this.lblProofOfBank.TabIndex = 43;
            this.lblProofOfBank.Text = "Proof of Bank:";
            // 
            // drpProofOfBank
            // 
            this.drpProofOfBank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpProofOfBank.Location = new System.Drawing.Point(112, 231);
            this.drpProofOfBank.Name = "drpProofOfBank";
            this.drpProofOfBank.Size = new System.Drawing.Size(216, 21);
            this.drpProofOfBank.TabIndex = 42;
            this.drpProofOfBank.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // lPrevIncometxt
            // 
            this.lPrevIncometxt.Location = new System.Drawing.Point(368, 280);
            this.lPrevIncometxt.Name = "lPrevIncometxt";
            this.lPrevIncometxt.Size = new System.Drawing.Size(200, 16);
            this.lPrevIncometxt.TabIndex = 41;
            this.lPrevIncometxt.Text = "Previous Income/Employment Notes:";
            // 
            // lPrevAddresstxt
            // 
            this.lPrevAddresstxt.Location = new System.Drawing.Point(368, 336);
            this.lPrevAddresstxt.Name = "lPrevAddresstxt";
            this.lPrevAddresstxt.Size = new System.Drawing.Size(136, 16);
            this.lPrevAddresstxt.TabIndex = 40;
            this.lPrevAddresstxt.Text = "Previous Address Notes:";
            // 
            // txtPrevDC2
            // 
            this.txtPrevDC2.Location = new System.Drawing.Point(368, 296);
            this.txtPrevDC2.MaxLength = 350;
            this.txtPrevDC2.Multiline = true;
            this.txtPrevDC2.Name = "txtPrevDC2";
            this.txtPrevDC2.ReadOnly = true;
            this.txtPrevDC2.Size = new System.Drawing.Size(288, 32);
            this.txtPrevDC2.TabIndex = 39;
            this.txtPrevDC2.Visible = false;
            // 
            // txtPrevDC3
            // 
            this.txtPrevDC3.Location = new System.Drawing.Point(368, 352);
            this.txtPrevDC3.MaxLength = 350;
            this.txtPrevDC3.Multiline = true;
            this.txtPrevDC3.Name = "txtPrevDC3";
            this.txtPrevDC3.ReadOnly = true;
            this.txtPrevDC3.Size = new System.Drawing.Size(288, 32);
            this.txtPrevDC3.TabIndex = 38;
            this.txtPrevDC3.Visible = false;
            // 
            // viewPrevious
            // 
            this.viewPrevious.Enabled = false;
            this.viewPrevious.ForeColor = System.Drawing.SystemColors.Control;
            this.viewPrevious.Location = new System.Drawing.Point(24, 336);
            this.viewPrevious.Name = "viewPrevious";
            this.viewPrevious.Size = new System.Drawing.Size(40, 23);
            this.viewPrevious.TabIndex = 37;
            this.viewPrevious.Text = "label1";
            this.viewPrevious.Visible = false;
            // 
            // lPrevIncome
            // 
            this.lPrevIncome.Location = new System.Drawing.Point(112, 280);
            this.lPrevIncome.Name = "lPrevIncome";
            this.lPrevIncome.Size = new System.Drawing.Size(200, 16);
            this.lPrevIncome.TabIndex = 22;
            this.lPrevIncome.Text = "Previous Proof of Income/Employment:";
            // 
            // drpPrevIncome
            // 
            this.drpPrevIncome.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPrevIncome.Enabled = false;
            this.drpPrevIncome.Location = new System.Drawing.Point(112, 296);
            this.drpPrevIncome.Name = "drpPrevIncome";
            this.drpPrevIncome.Size = new System.Drawing.Size(216, 21);
            this.drpPrevIncome.TabIndex = 23;
            this.drpPrevIncome.Visible = false;
            // 
            // lPrevAddress
            // 
            this.lPrevAddress.Location = new System.Drawing.Point(112, 336);
            this.lPrevAddress.Name = "lPrevAddress";
            this.lPrevAddress.Size = new System.Drawing.Size(144, 16);
            this.lPrevAddress.TabIndex = 18;
            this.lPrevAddress.Text = "Previous Proof of Address:";
            // 
            // drpPrevAddress
            // 
            this.drpPrevAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPrevAddress.Enabled = false;
            this.drpPrevAddress.Location = new System.Drawing.Point(112, 352);
            this.drpPrevAddress.Name = "drpPrevAddress";
            this.drpPrevAddress.Size = new System.Drawing.Size(216, 21);
            this.drpPrevAddress.TabIndex = 19;
            this.drpPrevAddress.Visible = false;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(368, 153);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 16);
            this.label9.TabIndex = 17;
            this.label9.Text = "Address Notes:";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(368, 87);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(144, 16);
            this.label8.TabIndex = 16;
            this.label8.Text = "Income/Employment Notes:";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(368, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 16);
            this.label7.TabIndex = 15;
            this.label7.Text = "ID Notes:";
            // 
            // txtDC1
            // 
            this.txtDC1.Location = new System.Drawing.Point(368, 40);
            this.txtDC1.MaxLength = 350;
            this.txtDC1.Multiline = true;
            this.txtDC1.Name = "txtDC1";
            this.txtDC1.Size = new System.Drawing.Size(288, 40);
            this.txtDC1.TabIndex = 7;
            // 
            // txtDC3
            // 
            this.txtDC3.Location = new System.Drawing.Point(368, 169);
            this.txtDC3.MaxLength = 350;
            this.txtDC3.Multiline = true;
            this.txtDC3.Name = "txtDC3";
            this.txtDC3.Size = new System.Drawing.Size(288, 40);
            this.txtDC3.TabIndex = 11;
            // 
            // txtDC2
            // 
            this.txtDC2.Location = new System.Drawing.Point(368, 103);
            this.txtDC2.MaxLength = 350;
            this.txtDC2.Multiline = true;
            this.txtDC2.Name = "txtDC2";
            this.txtDC2.Size = new System.Drawing.Size(288, 40);
            this.txtDC2.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(112, 87);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(168, 16);
            this.label6.TabIndex = 11;
            this.label6.Text = "Proof of Income/Employment:";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(112, 155);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 16);
            this.label5.TabIndex = 10;
            this.label5.Text = "Proof of Address:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(112, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 16);
            this.label1.TabIndex = 9;
            this.label1.Text = "Proof of ID:";
            // 
            // drpAddress
            // 
            this.drpAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpAddress.Location = new System.Drawing.Point(112, 171);
            this.drpAddress.Name = "drpAddress";
            this.drpAddress.Size = new System.Drawing.Size(216, 21);
            this.drpAddress.TabIndex = 10;
            this.drpAddress.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // drpIncome
            // 
            this.drpIncome.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpIncome.Location = new System.Drawing.Point(112, 103);
            this.drpIncome.Name = "drpIncome";
            this.drpIncome.Size = new System.Drawing.Size(216, 21);
            this.drpIncome.TabIndex = 8;
            this.drpIncome.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // drpID
            // 
            this.drpID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpID.Location = new System.Drawing.Point(112, 40);
            this.drpID.Name = "drpID";
            this.drpID.Size = new System.Drawing.Size(216, 21);
            this.drpID.TabIndex = 6;
            this.drpID.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateControl);
            // 
            // btnComplete
            // 
            this.btnComplete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnComplete.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnComplete.ImageIndex = 1;
            this.btnComplete.ImageList = this.menuIcons;
            this.btnComplete.Location = new System.Drawing.Point(728, 24);
            this.btnComplete.Name = "btnComplete";
            this.btnComplete.Size = new System.Drawing.Size(24, 24);
            this.btnComplete.TabIndex = 5;
            this.btnComplete.Click += new System.EventHandler(this.btnComplete_Click);
            // 
            // menuIcons
            // 
            this.menuIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("menuIcons.ImageStream")));
            this.menuIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.menuIcons.Images.SetKeyName(0, "");
            this.menuIcons.Images.SetKeyName(1, "");
            this.menuIcons.Images.SetKeyName(2, "");
            // 
            // gpCustomer
            // 
            this.gpCustomer.BackColor = System.Drawing.SystemColors.Control;
            this.gpCustomer.Controls.Add(this.txtCustomerID);
            this.gpCustomer.Controls.Add(this.dtDateProp);
            this.gpCustomer.Controls.Add(this.label4);
            this.gpCustomer.Controls.Add(this.label3);
            this.gpCustomer.Controls.Add(this.label2);
            this.gpCustomer.Controls.Add(this.lCustomerID);
            this.gpCustomer.Controls.Add(this.txtLastName);
            this.gpCustomer.Controls.Add(this.txtFirstName);
            this.gpCustomer.Location = new System.Drawing.Point(8, 2);
            this.gpCustomer.Name = "gpCustomer";
            this.gpCustomer.Size = new System.Drawing.Size(776, 80);
            this.gpCustomer.TabIndex = 5;
            this.gpCustomer.TabStop = false;
            // 
            // txtCustomerID
            // 
            this.txtCustomerID.Location = new System.Drawing.Point(16, 40);
            this.txtCustomerID.MaxLength = 20;
            this.txtCustomerID.Name = "txtCustomerID";
            this.txtCustomerID.ReadOnly = true;
            this.txtCustomerID.Size = new System.Drawing.Size(112, 20);
            this.txtCustomerID.TabIndex = 24;
            this.txtCustomerID.Tag = "lCustomerID";
            // 
            // dtDateProp
            // 
            this.dtDateProp.CustomFormat = "ddd dd MMM yyyy";
            this.dtDateProp.Enabled = false;
            this.dtDateProp.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDateProp.Location = new System.Drawing.Point(640, 40);
            this.dtDateProp.Name = "dtDateProp";
            this.dtDateProp.Size = new System.Drawing.Size(112, 20);
            this.dtDateProp.TabIndex = 23;
            this.dtDateProp.Tag = "";
            this.dtDateProp.Value = new System.DateTime(2002, 5, 21, 0, 0, 0, 0);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(640, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "Date of Application:";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(352, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Last Name:";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(160, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "First Name:";
            // 
            // lCustomerID
            // 
            this.lCustomerID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lCustomerID.Location = new System.Drawing.Point(16, 24);
            this.lCustomerID.Name = "lCustomerID";
            this.lCustomerID.Size = new System.Drawing.Size(72, 16);
            this.lCustomerID.TabIndex = 4;
            this.lCustomerID.Text = "Customer:";
            // 
            // txtLastName
            // 
            this.txtLastName.Location = new System.Drawing.Point(352, 40);
            this.txtLastName.MaxLength = 60;
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.ReadOnly = true;
            this.txtLastName.Size = new System.Drawing.Size(248, 20);
            this.txtLastName.TabIndex = 2;
            // 
            // txtFirstName
            // 
            this.txtFirstName.Location = new System.Drawing.Point(160, 40);
            this.txtFirstName.MaxLength = 30;
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.ReadOnly = true;
            this.txtFirstName.Size = new System.Drawing.Size(160, 20);
            this.txtFirstName.TabIndex = 1;
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
            // menuDocConfirmation
            // 
            this.menuDocConfirmation.Description = "MenuItem";
            this.menuDocConfirmation.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuSave,
            this.menuComplete,
            this.menuReopen});
            this.menuDocConfirmation.Text = "&Document Confirmation";
            // 
            // menuSave
            // 
            this.menuSave.Description = "MenuItem";
            this.menuSave.Text = "&Save";
            // 
            // menuComplete
            // 
            this.menuComplete.Description = "MenuItem";
            this.menuComplete.Text = "&Complete";
            this.menuComplete.Click += new System.EventHandler(this.btnComplete_Click);
            // 
            // menuReopen
            // 
            this.menuReopen.Description = "MenuItem";
            this.menuReopen.Enabled = false;
            this.menuReopen.Text = "&Re-open Document Confirmation";
            this.menuReopen.Visible = false;
            this.menuReopen.Click += new System.EventHandler(this.menuReopen_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // DocumentConfirmation
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 476);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gpCustomer);
            this.Name = "DocumentConfirmation";
            this.Text = "Document Confirmation";
            this.Load += new System.EventHandler(this.DocumentConfirmation_Load);
            this.Enter += new System.EventHandler(this.DocumentConfirmation_Enter);
            this.Leave += new System.EventHandler(this.DocumentConfirmation_Leave);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gpCustomer.ResumeLayout(false);
            this.gpCustomer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void DocumentConfirmation_Enter(object sender, System.EventArgs e)
        {

            //IP - 09/01/12 - #3811
            if (CustomerScreen != null)
            {
                createHPAcct = CustomerScreen.createHP;
            }

            ((MainForm)this.FormRoot).tbSanction.CustomerScreen = CustomerScreen;
            ((MainForm)this.FormRoot).tbSanction.Settled = false;
            //((MainForm)this.FormRoot).tbSanction.Load(true,this.CustomerID, this.DateProp, this.AccountNo, this.AccountType, this.ScreenMode);
            ((MainForm)this.FormRoot).tbSanction.Load(createHPAcct, this.CustomerID, this.DateProp, this.AccountNo, this.AccountType, this.ScreenMode); //IP - 09/01/12 - #3811
            CurrentStatus = ((MainForm)this.FormRoot).tbSanction.CurrentStatus;
            //((MainForm)this.FormRoot).tbSanction.SetCurrentStage(SS.DC);
            ((MainForm)this.FormRoot).tbSanction.Visible = true;
            ReadOnly = ((MainForm)this.FormRoot).tbSanction.ReadOnly(SS.DC);
            SetReadOnly();
        }

        private void DocumentConfirmation_Leave(object sender, System.EventArgs e)
        {
            ((MainForm)this.FormRoot).tbSanction.Visible = false;
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

        private void DocumentConfirmation_Load(object sender, System.EventArgs e)
        {
            string idCode = "";
            string addressCode = "";
            string incomeCode = "";
            string proofOfBankCode = string.Empty;                          //IP - 14/12/10 - Store Card
            string prevAddressCode = "";
            string prevIncomeCode = "";
            string prevAddressDesc = "";
            string prevIncomeDesc = "";

            toolTip1.SetToolTip(this.btnComplete, GetResource("TT_COMPLETE"));
            //toolTip1.SetToolTip(this.btnSave, GetResource("TT_SAVE"));

            try
            {
                Wait();
                if (LockAccount())
                {
                    //First load the static data for the drop downs
                    LoadStatic();

                    propData = CreditManager.GetDocConfirmationData(this.CustomerID,
                        this.AccountNo,
                        this.DateProp,
                        out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        foreach (DataTable dt in propData.Tables)
                        {
                            if (dt.TableName == TN.Employment)
                            {
                                foreach (DataRow r in dt.Rows)
                                {
                                    EmploymentStatus = (string)r[CN.EmploymentStatus];
                                }

                                /* can't use % in the middle of a like clause in a rowfilter, 
                                 * hence the convoluted pissing about with regular expressions. */
                                Regex reg1 = new Regex("^pay.*slip$");	/* ^ (starts with) pay (literal) .(any character) *(zero or more) slip$ (ends with slip)	*/
                                Regex reg2 = new Regex("^.*letter.*$");	/* ^ (starts with) .(any character) *(zero or more) letter (literal) .(any character) *(zero or more) $ */
                                DataTable filtered = ((DataView)drpIncome.DataSource).Table.Clone();

                                if (EmploymentStatus == "E")
                                {
                                    if (Config.CountryCode == "H")
                                        ((DataView)drpIncome.DataSource).RowFilter = CN.Code + " in ('M', 'P')";
                                    else
                                    {
                                        /*foreach(DataRow r in ((DataView)drpIncome.DataSource).Table.Rows)
                                        {
                                            if(reg1.IsMatch(((string)r[CN.CodeDescription]).ToLower()) ||
                                                reg2.IsMatch(((string)r[CN.CodeDescription]).ToLower()) ||
                                                ((string)r[CN.CodeDescription]).Length==0 )
                                            {
                                                DataRow n = filtered.NewRow();
                                                n[CN.Code] = r[CN.Code];
                                                n[CN.CodeDescription] = r[CN.CodeDescription];
                                                filtered.Rows.Add(n);
                                            }
                                        }*/

                                        drpIncome.DataSource = null;
                                        drpIncome.DataSource = ((DataTable)StaticData.Tables[TN.ProofOfIncomeEmployed]).DefaultView;
                                        drpIncome.DisplayMember = CN.CodeDescription;
                                    }
                                    //((DataView)drpIncome.DataSource).RowFilter = CN.Code + " in ('M', 'P')";
                                }
                                if (EmploymentStatus == "S")
                                {
                                    if (Config.CountryCode == "H")
                                        ((DataView)drpIncome.DataSource).RowFilter = CN.Code + " not in ('M', 'P')";
                                    else
                                    {
                                        /*foreach(DataRow r in ((DataView)drpIncome.DataSource).Table.Rows)
                                        {
                                            if((!reg1.IsMatch(((string)r[CN.CodeDescription]).ToLower()) &&
                                                !reg2.IsMatch(((string)r[CN.CodeDescription]).ToLower())) ||
                                                ((string)r[CN.CodeDescription]).Length==0 )
                                            {
                                                DataRow n = filtered.NewRow();
                                                n[CN.Code] = r[CN.Code];
                                                n[CN.CodeDescription] = r[CN.CodeDescription];
                                                filtered.Rows.Add(n);												
                                            }
                                        }*/

                                        drpIncome.DataSource = null;
                                        drpIncome.DataSource = ((DataTable)StaticData.Tables[TN.ProofOfIncomeSelfEmployed]).DefaultView;
                                        drpIncome.DisplayMember = CN.CodeDescription;
                                    }
                                }
                            }

                            if (dt.TableName == TN.DocConfirmation)
                            {
                                foreach (DataRow r in dt.Rows)
                                {
                                    txtCustomerID.Text = (string)r[CN.CustomerID];
                                    txtFirstName.Text = (string)r[CN.FirstName];
                                    txtLastName.Text = (string)r[CN.LastName];
                                    idCode = ((string)r[CN.ProofOfID]).Trim();
                                    addressCode = ((string)r[CN.ProofOfAddress]).Trim();
                                    incomeCode = ((string)r[CN.ProofOfIncome]).Trim();
                                    txtDC1.Text = (string)r[CN.DCText1];
                                    txtDC2.Text = (string)r[CN.DCText2];
                                    txtDC3.Text = (string)r[CN.DCText3];

                                    //IP - 14/12/10 - Store Card - If this is a Store Card account then drpProofOfBank will be visible, therefore set the fields.
                                    if (drpProofOfBank.Visible == true)
                                    {
                                        proofOfBankCode = r[CN.ProofOfBank] == DBNull.Value ? string.Empty : ((string)r[CN.ProofOfBank]).Trim();
                                        txtProofOfBank.Text = Convert.ToString(r[CN.ProofOfBankTxt]);
                                    }
                                }
                            }

                            if (dt.TableName == TN.PrevDocConf)
                            {
                                foreach (DataRow r in dt.Rows)
                                {
                                    prevAddressCode = ((string)r[CN.ProofOfAddress]).Trim();
                                    prevIncomeCode = ((string)r[CN.ProofOfIncome]).Trim();
                                    txtPrevDC2.Text = (string)r[CN.DCText2];
                                    txtPrevDC3.Text = (string)r[CN.DCText3];
                                }
                            }

                        }

                        foreach (DataRowView rv in drpID.Items)
                        {
                            if ((string)rv[CN.Code] == idCode)
                            {
                                drpID.SelectedItem = rv;
                                break;
                            }
                        }
                        foreach (DataRowView rv in drpAddress.Items)
                        {
                            if ((string)rv[CN.Code] == addressCode)
                            {
                                drpAddress.SelectedItem = rv;
                                break;
                            }
                        }
                        foreach (DataRowView rv in drpIncome.Items)
                        {
                            if ((string)rv[CN.Code] == incomeCode)
                            {
                                drpIncome.SelectedItem = rv;
                                break;
                            }
                        }

                        foreach (DataRowView rv in drpAddress.Items)
                        {
                            if ((string)rv[CN.Code] == prevAddressCode)
                            {
                                prevAddressDesc = (string)rv[CN.CodeDescription];
                                break;
                            }
                        }
                        foreach (DataRowView rv in drpIncome.Items)
                        {
                            if ((string)rv[CN.Code] == prevIncomeCode)
                            {
                                prevIncomeDesc = (string)rv[CN.CodeDescription];
                                break;
                            }
                        }

                        //IP - 14/12/10 - Store Card
                        if (drpProofOfBank.Visible == true)
                        {
                            foreach (DataRowView rv in drpProofOfBank.Items)
                            {
                                if ((string)rv[CN.Code] == proofOfBankCode)
                                {
                                    drpProofOfBank.SelectedItem = rv;
                                    break;
                                }
                            }
                        }

                        drpPrevAddress.DataSource = new string[] { prevAddressDesc };
                        drpPrevIncome.DataSource = new string[] { prevIncomeDesc };

                    }
                    // do not validate on initial launch of screen - UAT 5.0 iss 51 (jec 29/03/07)

                    // ValidateControl(null, null);
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

        private void LoadStatic()
        {

            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

            if (StaticData.Tables[TN.ProofOfID] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ProofOfID, new string[] { "PID", "L" }));
            if (StaticData.Tables[TN.ProofOfIncome] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ProofOfIncome, new string[] { "PIN", "L" }));
            if (StaticData.Tables[TN.ProofOfIncomeEmployed] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ProofOfIncomeEmployed, new string[] { "PIE", "L" }));
            if (StaticData.Tables[TN.ProofOfIncomeSelfEmployed] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ProofOfIncomeSelfEmployed, new string[] { "PIS", "L" }));
            if (StaticData.Tables[TN.ProofOfAddress] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ProofOfAddress, new string[] { "PAD", "L" }));
            if (StaticData.Tables[TN.ProofOfBank] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ProofOfBank, new string[] { "PBK", "L" }));              //IP - 14/12/10 - Store Card

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

            drpID.DataSource = (DataTable)StaticData.Tables[TN.ProofOfID];
            drpID.DisplayMember = CN.CodeDescription;

            drpAddress.DataSource = (DataTable)StaticData.Tables[TN.ProofOfAddress];
            drpAddress.DisplayMember = CN.CodeDescription;

            drpIncome.DataSource = ((DataTable)StaticData.Tables[TN.ProofOfIncome]).DefaultView;
            drpIncome.DisplayMember = CN.CodeDescription;

            //IP - 14/12/10 - StoreCard. If this is a Store Card account then display the Proof Of Bank drop down
            if (AccountType == AT.StoreCard)
            {
                drpProofOfBank.Visible = true;
                drpProofOfBank.Enabled = true;
                lblProofOfBank.Visible = true;
                txtProofOfBank.Visible = true;
                lblProofOfBankNotes.Visible = true;

                drpProofOfBank.DataSource = (DataTable)StaticData.Tables[TN.ProofOfBank];
                drpProofOfBank.DisplayMember = CN.CodeDescription;
                drpProofOfBank.SelectedIndex = -1;
            }
            else
            {
                drpProofOfBank.Visible = false;
                drpProofOfBank.Enabled = false;
                lblProofOfBank.Visible = false;
                txtProofOfBank.Visible = false;
                lblProofOfBankNotes.Visible = false;
                drpProofOfBank.DataSource = null;
            }

            if (viewPrevious.Enabled)
            {
                lPrevAddress.Visible = true;
                lPrevIncome.Visible = true;
                lPrevAddresstxt.Visible = true;
                lPrevIncometxt.Visible = true;
                drpPrevAddress.Visible = true;
                drpPrevIncome.Visible = true;
                txtPrevDC3.Visible = true;
                txtPrevDC2.Visible = true;
            }
            else
            {
                lPrevAddress.Visible = false;
                lPrevIncome.Visible = false;
                lPrevAddresstxt.Visible = false;
                lPrevIncometxt.Visible = false;
                drpPrevAddress.Visible = false;
                drpPrevIncome.Visible = false;
                txtPrevDC3.Visible = false;
                txtPrevDC2.Visible = false;
            }

        }

        private void ValidateComboBox(ComboBox cb, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Function = "ValidateComboBox()";
                if (cb.Text.Length == 0 && _complete)
                {
                    valid = false;
                    errorProvider1.SetError(cb, GetResource("M_ENTERMANDATORY"));
                }
                else
                {
                    valid = valid ? true : false;
                    errorProvider1.SetError(cb, "");
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void drpID_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e);
        }

        private void drpIncome_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e);
        }

        private void drpAddress_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateComboBox((ComboBox)sender, e);
        }

        ////IP - 14/12/10 - Store Card
        //private void drpProofOfBank_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    ValidateComboBox((ComboBox)sender, e);
        //}

        private bool Valid()
        {
            valid = true;
            drpID_Validating(drpID, new CancelEventArgs());
            drpIncome_Validating(drpIncome, new CancelEventArgs());
            drpAddress_Validating(drpAddress, new CancelEventArgs());

            //IP - 14/12/10 - Store Card - Only want to validate the ProofOfBank control if this is a StoreCard account.
            if (drpProofOfBank.Visible)
            {
                drpAddress_Validating(drpProofOfBank, new CancelEventArgs());
            }
            return valid;
        }

        private void btnComplete_Click(object sender, System.EventArgs e)
        {
            //Save with complete flag
            /*
            _complete = true;
            if(Valid())
                Save(true);
                */
            ValidateControl(null, null);        // UAT 5.0 iss 51 (jec 29/03/07)
            Save(btnComplete.ImageIndex == 2);
        }

        private void Save(bool complete)
        {
            Function = "Save()";
            try
            {
                Wait();

                foreach (DataTable dt in propData.Tables)
                {
                    if (dt.TableName == TN.DocConfirmation)
                    {
                        foreach (DataRow r in dt.Rows)
                        {
                            r[CN.ProofOfAddress] = (string)((DataRowView)drpAddress.SelectedItem)[CN.Code];
                            r[CN.ProofOfID] = (string)((DataRowView)drpID.SelectedItem)[CN.Code];
                            r[CN.ProofOfIncome] = (string)((DataRowView)drpIncome.SelectedItem)[CN.Code];
                            r[CN.DCText1] = txtDC1.Text;
                            r[CN.DCText2] = txtDC2.Text;
                            r[CN.DCText3] = txtDC3.Text;

                            if (drpProofOfBank.Visible) //IP - 14/12/10 - Store Card - ProofOfBank will only be displayed when scoring Store Card accounts.
                            {
                                if (drpProofOfBank.SelectedIndex != -1) //Set to the option selected, else set to empty string if nothing selected.
                                {
                                    r[CN.ProofOfBank] = Convert.ToString(((DataRowView)drpProofOfBank.SelectedItem)[CN.Code]);
                                }
                                else
                                {
                                    r[CN.ProofOfBank] = string.Empty;
                                }
                                r[CN.ProofOfBankTxt] = txtProofOfBank.Text;
                            }
                        }
                    }
                }
                propData.AcceptChanges();

                CreditManager.SaveDocConfirmation(AccountNo, propData, complete, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    if (complete)
                    {
                        ((MainForm)this.FormRoot).tbSanction.Load(true, this.CustomerID, this.DateProp, this.AccountNo, this.AccountType, this.ScreenMode);
                        CurrentStatus = ((MainForm)this.FormRoot).tbSanction.CurrentStatus;
                        //((MainForm)this.FormRoot).tbSanction.SetCurrentStage(SS.DC);
                        ReadOnly = ((MainForm)this.FormRoot).tbSanction.ReadOnly(SS.DC);
                        this.SetReadOnly();

                        if (CustomerScreen != null)
                            CustomerScreen.txtAccountNo_TextChanged(null, null);
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

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }

        private void menuReopen_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (((MainForm)FormRoot).tbSanction.ReadOnly(SS.DC))
                {
                    CreditManager.UnClearFlag(AccountNo, SS.DC, true, Credential.UserId, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        DocumentConfirmation_Enter(sender, e);
                        if (CustomerScreen != null)
                            CustomerScreen.txtAccountNo_TextChanged(null, null);
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

        private void ValidateControl(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Wait();

                _complete = true;

                if (Valid())
                    btnComplete.ImageIndex = 2;
                else
                    btnComplete.ImageIndex = 1;

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


        //IP - 09/01/12 - #3811 - Method to check if HP account can be created for the Branch selected.
        private void CanCreateHPAcctForBranch(string branchNo)
        {
            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
            {
                if (row["branchno"].ToString() == branchNo)
                {
                    createHPAcct = (bool)row[CN.CreateHPAccounts];
                    break;
                }
            }
        }
    }
}
