using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using System.Data;
using STL.Common.Static;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.CustomerTypes;

namespace STL.PL
{
    /// <summary>
    /// Allows an authorised user to manually add financial transactions to an account.
    /// A reason can be recorded for each transaction. The transaction can be printed
    /// to a receipt or to a customer payment card.
    /// </summary>
    public class GeneralFinancialTransactions : CommonForm
    {
        //private Crownwood.Magic.Menus.MenuControl menuMain;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.GroupBox gbCustomer;
        private System.Windows.Forms.Button btnAccountDetails;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtArrears;
        private System.Windows.Forms.TextBox txtOutstandingBalance;
        private System.Windows.Forms.Label lArrears;
        private System.Windows.Forms.TextBox txtAgreementTotal;
        private System.Windows.Forms.Label lAgreementTotal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSearchAccount;
        private STL.PL.AccountTextBox txtAccountNo;
        private System.Windows.Forms.Label lAccountNo;
        private STL.PL.AddressTab atCustomerAddress;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ToolTip ttPayment;
        private System.ComponentModel.IContainer components;
        private string lastAccountNo = "";
        private new string Error = "";
        private string CustomerID = "";
        private System.Windows.Forms.GroupBox gbTransaction;
        private System.Windows.Forms.Button btnUpdateCard;
        private System.Windows.Forms.NumericUpDown txtCardRowNo;
        private System.Windows.Forms.Label lCardRowNo;
        private System.Windows.Forms.ComboBox drpCardPrintType;
        private System.Windows.Forms.Label lCardPrintType;
        private System.Windows.Forms.CheckBox cbCardPrint;
        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox drpReason;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnEnter;
        private System.Windows.Forms.ComboBox drpTransaction;
        private System.Windows.Forms.Label label6;
        private string AccountType = "";
        private System.Windows.Forms.GroupBox gbPayments;
        private System.Windows.Forms.DataGrid dgPayments;
        private System.Windows.Forms.RadioButton rbCredit;
        private System.Windows.Forms.RadioButton rbDebit;
        private DataSet transactions = null;
        private decimal total = 0;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private new bool loaded = false;
        private int creditDebit = -1;
        private string AccountStatus = "";
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtServiceCharge;
        private System.Windows.Forms.CheckBox cbPrintReceipt;
        private System.Windows.Forms.TextBox txtOutstanding;
        private System.Windows.Forms.Label label8;
        private decimal ServiceCharge = 0;
        private string _lastTransactionType = "";
        private DataSet _WarrantyReturnDataSet = null;
        private decimal _validationAmount = 0;
        public GeneralFinancialTransactions(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
        }

        public GeneralFinancialTransactions(Form root, Form parent)
        {
            FormRoot = root;
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
            FormParent = parent;
            this.gbCustomer.Controls.Add(this.atCustomerAddress);
        }

        //jec - 08/07/10 - UAT1065 - New constructor called from the Telephone Action/Bailiff Review screen if the 
        //action code 'RBDW - Reverse BDW' is selected.
        public GeneralFinancialTransactions(Form root, Form parent, string acctNo)
        {
            FormRoot = root;
            InitializeComponent();
            this.GeneralFinancialTransactions_Load(null, null);

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });

            //HashMenus();

            FormParent = parent;

            //Screen has been called from Telephone Action screen, therefore load the details for the selected account
            //preventing the user from entering or searching for a different account.
            txtAccountNo.Text = acctNo;
            txtAccountNo.ReadOnly = true;
            btnSearchAccount.Enabled = false;
            LoadData(acctNo);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneralFinancialTransactions));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.gbCustomer = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtServiceCharge = new System.Windows.Forms.TextBox();
            this.btnAccountDetails = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtArrears = new System.Windows.Forms.TextBox();
            this.txtOutstandingBalance = new System.Windows.Forms.TextBox();
            this.lArrears = new System.Windows.Forms.Label();
            this.txtAgreementTotal = new System.Windows.Forms.TextBox();
            this.lAgreementTotal = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSearchAccount = new System.Windows.Forms.Button();
            this.txtAccountNo = new STL.PL.AccountTextBox();
            this.lAccountNo = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.atCustomerAddress = new STL.PL.AddressTab(FormRoot);           //CR1084 jec
            this.ttPayment = new System.Windows.Forms.ToolTip(this.components);
            this.gbTransaction = new System.Windows.Forms.GroupBox();
            this.cbPrintReceipt = new System.Windows.Forms.CheckBox();
            this.rbDebit = new System.Windows.Forms.RadioButton();
            this.rbCredit = new System.Windows.Forms.RadioButton();
            this.drpTransaction = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnUpdateCard = new System.Windows.Forms.Button();
            this.txtCardRowNo = new System.Windows.Forms.NumericUpDown();
            this.lCardRowNo = new System.Windows.Forms.Label();
            this.drpCardPrintType = new System.Windows.Forms.ComboBox();
            this.lCardPrintType = new System.Windows.Forms.Label();
            this.cbCardPrint = new System.Windows.Forms.CheckBox();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.drpReason = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnEnter = new System.Windows.Forms.Button();
            this.gbPayments = new System.Windows.Forms.GroupBox();
            this.txtOutstanding = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.dgPayments = new System.Windows.Forms.DataGrid();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.gbCustomer.SuspendLayout();
            this.gbTransaction.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCardRowNo)).BeginInit();
            this.gbPayments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgPayments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
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
            // gbCustomer
            // 
            this.gbCustomer.BackColor = System.Drawing.SystemColors.Control;
            this.gbCustomer.Controls.Add(this.label7);
            this.gbCustomer.Controls.Add(this.txtServiceCharge);
            this.gbCustomer.Controls.Add(this.btnAccountDetails);
            this.gbCustomer.Controls.Add(this.label3);
            this.gbCustomer.Controls.Add(this.txtArrears);
            this.gbCustomer.Controls.Add(this.txtOutstandingBalance);
            this.gbCustomer.Controls.Add(this.lArrears);
            this.gbCustomer.Controls.Add(this.txtAgreementTotal);
            this.gbCustomer.Controls.Add(this.lAgreementTotal);
            this.gbCustomer.Controls.Add(this.label2);
            this.gbCustomer.Controls.Add(this.label1);
            this.gbCustomer.Controls.Add(this.btnSearchAccount);
            this.gbCustomer.Controls.Add(this.txtAccountNo);
            this.gbCustomer.Controls.Add(this.lAccountNo);
            this.gbCustomer.Controls.Add(this.txtCustomerName);
            this.gbCustomer.Controls.Add(this.btnExit);
            this.gbCustomer.Controls.Add(this.btnClear);
            this.gbCustomer.Location = new System.Drawing.Point(8, 0);
            this.gbCustomer.Name = "gbCustomer";
            this.gbCustomer.Size = new System.Drawing.Size(776, 176);
            this.gbCustomer.TabIndex = 0;
            this.gbCustomer.TabStop = false;
            this.gbCustomer.Text = "Reference";
            // 
            // label7
            // 
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(528, 128);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 16);
            this.label7.TabIndex = 31;
            this.label7.Text = "Service Charge";
            this.label7.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtServiceCharge
            // 
            this.txtServiceCharge.BackColor = System.Drawing.SystemColors.Window;
            this.txtServiceCharge.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtServiceCharge.Location = new System.Drawing.Point(528, 144);
            this.txtServiceCharge.MaxLength = 10;
            this.txtServiceCharge.Name = "txtServiceCharge";
            this.txtServiceCharge.ReadOnly = true;
            this.txtServiceCharge.Size = new System.Drawing.Size(96, 21);
            this.txtServiceCharge.TabIndex = 30;
            this.txtServiceCharge.TabStop = false;
            // 
            // btnAccountDetails
            // 
            this.btnAccountDetails.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAccountDetails.Image = ((System.Drawing.Image)(resources.GetObject("btnAccountDetails.Image")));
            this.btnAccountDetails.Location = new System.Drawing.Point(672, 24);
            this.btnAccountDetails.Name = "btnAccountDetails";
            this.btnAccountDetails.Size = new System.Drawing.Size(24, 24);
            this.btnAccountDetails.TabIndex = 9;
            this.btnAccountDetails.Click += new System.EventHandler(this.btnAccountDetails_Click);
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(528, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 16);
            this.label3.TabIndex = 29;
            this.label3.Text = "Outstanding Balance";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtArrears
            // 
            this.txtArrears.BackColor = System.Drawing.SystemColors.Window;
            this.txtArrears.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtArrears.Location = new System.Drawing.Point(528, 64);
            this.txtArrears.MaxLength = 10;
            this.txtArrears.Name = "txtArrears";
            this.txtArrears.ReadOnly = true;
            this.txtArrears.Size = new System.Drawing.Size(96, 21);
            this.txtArrears.TabIndex = 5;
            this.txtArrears.TabStop = false;
            // 
            // txtOutstandingBalance
            // 
            this.txtOutstandingBalance.BackColor = System.Drawing.SystemColors.Window;
            this.txtOutstandingBalance.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtOutstandingBalance.Location = new System.Drawing.Point(528, 104);
            this.txtOutstandingBalance.MaxLength = 10;
            this.txtOutstandingBalance.Name = "txtOutstandingBalance";
            this.txtOutstandingBalance.ReadOnly = true;
            this.txtOutstandingBalance.Size = new System.Drawing.Size(96, 21);
            this.txtOutstandingBalance.TabIndex = 6;
            this.txtOutstandingBalance.TabStop = false;
            // 
            // lArrears
            // 
            this.lArrears.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lArrears.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lArrears.Location = new System.Drawing.Point(528, 48);
            this.lArrears.Name = "lArrears";
            this.lArrears.Size = new System.Drawing.Size(72, 16);
            this.lArrears.TabIndex = 20;
            this.lArrears.Text = "Arrears";
            this.lArrears.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtAgreementTotal
            // 
            this.txtAgreementTotal.BackColor = System.Drawing.SystemColors.Window;
            this.txtAgreementTotal.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtAgreementTotal.Location = new System.Drawing.Point(528, 24);
            this.txtAgreementTotal.MaxLength = 10;
            this.txtAgreementTotal.Name = "txtAgreementTotal";
            this.txtAgreementTotal.ReadOnly = true;
            this.txtAgreementTotal.Size = new System.Drawing.Size(96, 21);
            this.txtAgreementTotal.TabIndex = 4;
            this.txtAgreementTotal.TabStop = false;
            // 
            // lAgreementTotal
            // 
            this.lAgreementTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lAgreementTotal.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lAgreementTotal.Location = new System.Drawing.Point(528, 8);
            this.lAgreementTotal.Name = "lAgreementTotal";
            this.lAgreementTotal.Size = new System.Drawing.Size(100, 16);
            this.lAgreementTotal.TabIndex = 22;
            this.lAgreementTotal.Text = "Agreement Total";
            this.lAgreementTotal.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(224, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 16);
            this.label2.TabIndex = 17;
            this.label2.Text = "Customer Address";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(224, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 16;
            this.label1.Text = "Customer Name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // btnSearchAccount
            // 
            this.btnSearchAccount.Image = ((System.Drawing.Image)(resources.GetObject("btnSearchAccount.Image")));
            this.btnSearchAccount.Location = new System.Drawing.Point(144, 32);
            this.btnSearchAccount.Name = "btnSearchAccount";
            this.btnSearchAccount.Size = new System.Drawing.Size(20, 20);
            this.btnSearchAccount.TabIndex = 1;
            this.btnSearchAccount.Click += new System.EventHandler(this.btnSearchAccount_Click);
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtAccountNo.Location = new System.Drawing.Point(40, 32);
            this.txtAccountNo.MaxLength = 20;
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.Size = new System.Drawing.Size(88, 20);
            this.txtAccountNo.TabIndex = 0;
            this.txtAccountNo.Text = "000-0000-0000-0";
            this.txtAccountNo.Leave += new System.EventHandler(this.txtAccountNo_Leave);
            // 
            // lAccountNo
            // 
            this.lAccountNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lAccountNo.Location = new System.Drawing.Point(40, 16);
            this.lAccountNo.Name = "lAccountNo";
            this.lAccountNo.Size = new System.Drawing.Size(72, 16);
            this.lAccountNo.TabIndex = 8;
            this.lAccountNo.Text = "Account No";
            this.lAccountNo.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.SystemColors.Window;
            this.txtCustomerName.Location = new System.Drawing.Point(224, 32);
            this.txtCustomerName.MaxLength = 80;
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(240, 20);
            this.txtCustomerName.TabIndex = 2;
            this.txtCustomerName.TabStop = false;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(712, 56);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(48, 23);
            this.btnExit.TabIndex = 8;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(712, 24);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(48, 23);
            this.btnClear.TabIndex = 7;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // atCustomerAddress
            // 
            this.atCustomerAddress.Location = new System.Drawing.Point(216, 48);
            this.atCustomerAddress.Name = "atCustomerAddress";
            this.atCustomerAddress.ReadOnly = true;
            this.atCustomerAddress.SimpleAddress = true;
            this.atCustomerAddress.Size = new System.Drawing.Size(184, 108);
            this.atCustomerAddress.TabIndex = 3;
            this.atCustomerAddress.TabStop = false;
            this.atCustomerAddress.FirstName.Visible = false;
            this.atCustomerAddress.CFirstname.Visible = false;
            this.atCustomerAddress.LastName.Visible = false;
            this.atCustomerAddress.CLastname.Visible = false;
            this.atCustomerAddress.Title.Visible = false;
            this.atCustomerAddress.drptitleC.Visible = false;
            this.atCustomerAddress.txtCoordinate.Visible = false;
            // 
            // gbTransaction
            // 
            this.gbTransaction.BackColor = System.Drawing.SystemColors.Control;
            this.gbTransaction.Controls.Add(this.cbPrintReceipt);
            this.gbTransaction.Controls.Add(this.rbDebit);
            this.gbTransaction.Controls.Add(this.rbCredit);
            this.gbTransaction.Controls.Add(this.drpTransaction);
            this.gbTransaction.Controls.Add(this.label6);
            this.gbTransaction.Controls.Add(this.btnUpdateCard);
            this.gbTransaction.Controls.Add(this.txtCardRowNo);
            this.gbTransaction.Controls.Add(this.lCardRowNo);
            this.gbTransaction.Controls.Add(this.drpCardPrintType);
            this.gbTransaction.Controls.Add(this.lCardPrintType);
            this.gbTransaction.Controls.Add(this.cbCardPrint);
            this.gbTransaction.Controls.Add(this.txtAmount);
            this.gbTransaction.Controls.Add(this.label4);
            this.gbTransaction.Controls.Add(this.drpReason);
            this.gbTransaction.Controls.Add(this.label5);
            this.gbTransaction.Controls.Add(this.btnEnter);
            this.gbTransaction.Enabled = false;
            this.gbTransaction.Location = new System.Drawing.Point(8, 176);
            this.gbTransaction.Name = "gbTransaction";
            this.gbTransaction.Size = new System.Drawing.Size(776, 112);
            this.gbTransaction.TabIndex = 1;
            this.gbTransaction.TabStop = false;
            this.gbTransaction.Text = "Transaction Details";
            // 
            // cbPrintReceipt
            // 
            this.cbPrintReceipt.BackColor = System.Drawing.SystemColors.Control;
            this.cbPrintReceipt.Checked = true;
            this.cbPrintReceipt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPrintReceipt.Location = new System.Drawing.Point(360, 72);
            this.cbPrintReceipt.Name = "cbPrintReceipt";
            this.cbPrintReceipt.Size = new System.Drawing.Size(88, 32);
            this.cbPrintReceipt.TabIndex = 70;
            this.cbPrintReceipt.Text = "Print Receipt";
            this.cbPrintReceipt.UseVisualStyleBackColor = false;
            // 
            // rbDebit
            // 
            this.rbDebit.Location = new System.Drawing.Point(256, 16);
            this.rbDebit.Name = "rbDebit";
            this.rbDebit.Size = new System.Drawing.Size(56, 24);
            this.rbDebit.TabIndex = 69;
            this.rbDebit.Text = "Debit";
            // 
            // rbCredit
            // 
            this.rbCredit.Checked = true;
            this.rbCredit.Location = new System.Drawing.Point(192, 16);
            this.rbCredit.Name = "rbCredit";
            this.rbCredit.Size = new System.Drawing.Size(56, 24);
            this.rbCredit.TabIndex = 68;
            this.rbCredit.TabStop = true;
            this.rbCredit.Text = "Credit";
            this.rbCredit.CheckedChanged += new System.EventHandler(this.rbCredit_CheckedChanged);
            // 
            // drpTransaction
            // 
            this.drpTransaction.BackColor = System.Drawing.SystemColors.Window;
            this.drpTransaction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpTransaction.Location = new System.Drawing.Point(40, 40);
            this.drpTransaction.Name = "drpTransaction";
            this.drpTransaction.Size = new System.Drawing.Size(272, 21);
            this.drpTransaction.TabIndex = 0;
            this.drpTransaction.SelectedIndexChanged += new System.EventHandler(this.drpTransaction_SelectedIndexChanged);
            this.drpTransaction.KeyUp += new System.Windows.Forms.KeyEventHandler(this.drpTransaction_KeyUp);
            // 
            // label6
            // 
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(40, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 16);
            this.label6.TabIndex = 67;
            this.label6.Text = "Transaction Type";
            this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // btnUpdateCard
            // 
            this.btnUpdateCard.Enabled = false;
            this.btnUpdateCard.Image = ((System.Drawing.Image)(resources.GetObject("btnUpdateCard.Image")));
            this.btnUpdateCard.Location = new System.Drawing.Point(632, 40);
            this.btnUpdateCard.Name = "btnUpdateCard";
            this.btnUpdateCard.Size = new System.Drawing.Size(20, 20);
            this.btnUpdateCard.TabIndex = 6;
            this.btnUpdateCard.Click += new System.EventHandler(this.btnUpdateCard_Click);
            // 
            // txtCardRowNo
            // 
            this.txtCardRowNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtCardRowNo.Enabled = false;
            this.txtCardRowNo.Location = new System.Drawing.Point(560, 40);
            this.txtCardRowNo.Name = "txtCardRowNo";
            this.txtCardRowNo.Size = new System.Drawing.Size(48, 20);
            this.txtCardRowNo.TabIndex = 5;
            // 
            // lCardRowNo
            // 
            this.lCardRowNo.Enabled = false;
            this.lCardRowNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lCardRowNo.Location = new System.Drawing.Point(560, 24);
            this.lCardRowNo.Name = "lCardRowNo";
            this.lCardRowNo.Size = new System.Drawing.Size(72, 16);
            this.lCardRowNo.TabIndex = 61;
            this.lCardRowNo.Text = "Row Number";
            // 
            // drpCardPrintType
            // 
            this.drpCardPrintType.BackColor = System.Drawing.SystemColors.Window;
            this.drpCardPrintType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCardPrintType.Enabled = false;
            this.drpCardPrintType.Items.AddRange(new object[] {
            "Long (30 lines)",
            "Short (27 lines)"});
            this.drpCardPrintType.Location = new System.Drawing.Point(440, 40);
            this.drpCardPrintType.Name = "drpCardPrintType";
            this.drpCardPrintType.Size = new System.Drawing.Size(112, 21);
            this.drpCardPrintType.TabIndex = 4;
            // 
            // lCardPrintType
            // 
            this.lCardPrintType.Enabled = false;
            this.lCardPrintType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lCardPrintType.Location = new System.Drawing.Point(440, 24);
            this.lCardPrintType.Name = "lCardPrintType";
            this.lCardPrintType.Size = new System.Drawing.Size(96, 16);
            this.lCardPrintType.TabIndex = 62;
            this.lCardPrintType.Text = "Card Print Type";
            // 
            // cbCardPrint
            // 
            this.cbCardPrint.BackColor = System.Drawing.SystemColors.Control;
            this.cbCardPrint.Location = new System.Drawing.Point(360, 32);
            this.cbCardPrint.Name = "cbCardPrint";
            this.cbCardPrint.Size = new System.Drawing.Size(72, 32);
            this.cbCardPrint.TabIndex = 3;
            this.cbCardPrint.Text = "Payment Card";
            this.cbCardPrint.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.cbCardPrint.UseVisualStyleBackColor = false;
            this.cbCardPrint.CheckedChanged += new System.EventHandler(this.cbCardPrint_CheckedChanged);
            // 
            // txtAmount
            // 
            this.txtAmount.BackColor = System.Drawing.SystemColors.Window;
            this.txtAmount.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtAmount.Location = new System.Drawing.Point(40, 80);
            this.txtAmount.MaxLength = 10;
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(96, 21);
            this.txtAmount.TabIndex = 1;
            this.txtAmount.Leave += new System.EventHandler(this.txtAmount_Leave);
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(40, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 16);
            this.label4.TabIndex = 59;
            this.label4.Text = "Amount";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // drpReason
            // 
            this.drpReason.BackColor = System.Drawing.SystemColors.Window;
            this.drpReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpReason.Location = new System.Drawing.Point(144, 80);
            this.drpReason.Name = "drpReason";
            this.drpReason.Size = new System.Drawing.Size(168, 21);
            this.drpReason.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(144, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 16);
            this.label5.TabIndex = 60;
            this.label5.Text = "Reason";
            this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // btnEnter
            // 
            this.btnEnter.Location = new System.Drawing.Point(712, 32);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(48, 23);
            this.btnEnter.TabIndex = 7;
            this.btnEnter.Text = "Enter";
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // gbPayments
            // 
            this.gbPayments.BackColor = System.Drawing.SystemColors.Control;
            this.gbPayments.Controls.Add(this.txtOutstanding);
            this.gbPayments.Controls.Add(this.label8);
            this.gbPayments.Controls.Add(this.dgPayments);
            this.gbPayments.Location = new System.Drawing.Point(8, 288);
            this.gbPayments.Name = "gbPayments";
            this.gbPayments.Size = new System.Drawing.Size(776, 184);
            this.gbPayments.TabIndex = 2;
            this.gbPayments.TabStop = false;
            this.gbPayments.Text = "Relevant Financial Transactions";
            // 
            // txtOutstanding
            // 
            this.txtOutstanding.BackColor = System.Drawing.SystemColors.Window;
            this.txtOutstanding.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtOutstanding.Location = new System.Drawing.Point(48, 35);
            this.txtOutstanding.MaxLength = 10;
            this.txtOutstanding.Name = "txtOutstanding";
            this.txtOutstanding.ReadOnly = true;
            this.txtOutstanding.Size = new System.Drawing.Size(96, 21);
            this.txtOutstanding.TabIndex = 60;
            // 
            // label8
            // 
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(46, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(184, 16);
            this.label8.TabIndex = 61;
            this.label8.Text = "Total Amount Outstanding";
            this.label8.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // dgPayments
            // 
            this.dgPayments.CaptionVisible = false;
            this.dgPayments.DataMember = "";
            this.dgPayments.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgPayments.Location = new System.Drawing.Point(48, 64);
            this.dgPayments.Name = "dgPayments";
            this.dgPayments.ReadOnly = true;
            this.dgPayments.Size = new System.Drawing.Size(608, 112);
            this.dgPayments.TabIndex = 0;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // GeneralFinancialTransactions
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.gbPayments);
            this.Controls.Add(this.gbTransaction);
            this.Controls.Add(this.gbCustomer);
            this.Name = "GeneralFinancialTransactions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "General Financial Transactions";
            this.Load += new System.EventHandler(this.GeneralFinancialTransactions_Load);
            this.gbCustomer.ResumeLayout(false);
            this.gbCustomer.PerformLayout();
            this.gbTransaction.ResumeLayout(false);
            this.gbTransaction.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCardRowNo)).EndInit();
            this.gbPayments.ResumeLayout(false);
            this.gbPayments.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgPayments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }

        private void btnAccountDetails_Click(object sender, System.EventArgs e)
        {
            if (txtAccountNo.Text != "000-0000-0000-0")
            {
                AccountDetails details = new AccountDetails(txtAccountNo.UnformattedText, FormRoot, this);
                ((MainForm)this.FormRoot).AddTabPage(details, 7);
            }
        }

        private void btnSearchAccount_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                AccountSearch a = new AccountSearch();
                a.FormRoot = this.FormRoot;
                a.FormParent = this;
                ((MainForm)this.FormRoot).AddTabPage(a, 7);
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSearchAccount_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void GeneralFinancialTransactions_Load(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                LoadStaticData();
                ttPayment.SetToolTip(btnUpdateCard, GetResource("TT_PRINTCARD"));
                ttPayment.SetToolTip(this.btnAccountDetails, GetResource("TT_ACCOUNTDETAILS"));

            }
            catch (Exception ex)
            {
                Catch(ex, "RefundAndCorrection_Load");
            }
            finally
            {
                StopWait();
            }
        }

        public bool LoadData(string accountNo)
        {
            bool status = true;

            try
            {
                txtAccountNo.Text = accountNo;

                if (accountNo != lastAccountNo &&
                    accountNo != "000000000000")
                {
                    Wait();

                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_SEARCHACCOUNT");

                    // Find the Customer ID for this Account Number
                    DataSet CustomerSet = CustomerManager.GetCustomerAccountsAndDetails(accountNo, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        if (CustomerSet.Tables[TN.Customer].Rows.Count < 1)
                        {
                            ShowInfo("M_NOSUCHACCOUNT");
                            btnClear_Click(null, null);
                            status = false;
                        }
                    }

                    if (status)
                    {
                        DataRow r = CustomerSet.Tables[TN.Customer].Rows[0];

                        CustomerID = (string)r[CN.CustomerID];

                        // lw69285 rdb 25/9/07 if this is a bdw account do not proceed
                        // rdb 20/11/07 missunderstanding, need to check if account 
                        // has a bdw recird in fintrans

                        //IP - 16/04/09 - CR971 - Archiving - commented out the below as was previously 
                        //preventing performing any general financial transactions on a BDW account.
                        //Need to be able to process an INS (Insurance Claim) on a BDW account.

                        //if(BDW.IsBDW(CustomerID))
                        //string error;
                        //if(AccountManager.GetAccountHasBDW(accountNo, out error))
                        //{

                        //    ShowInfo("M_BDWINVALIDSCREEN");
                        //    status = false;
                        //    btnClear_Click(null, null);
                        //}
                        //else
                        //{
                        txtCustomerName.Text = (string)r[CN.Title] +
                            " " + (string)r[CN.FirstName] +
                            " " + (string)r[CN.LastName];
                        //}
                    }


                    if (status)
                    {
                        DataSet addresses = CustomerManager.GetCustomerAddresses(CustomerID, out Error);
                        foreach (DataTable dt in addresses.Tables)
                            foreach (DataRow row in dt.Rows)
                            {
                                if (((string)row[CN.AddressType]).Trim() == "H")
                                {
                                    atCustomerAddress.txtAddress1.Text = (string)row[CN.Address1];
                                    if (atCustomerAddress.cmbVillage.Items.Count > 0 &&
                                    row[CN.Address2] != DBNull.Value) // Address Standardization CR2019 - 025
                                    {
                                        var villageIndex = atCustomerAddress.cmbVillage.FindStringExact((string)row[CN.Address2]);
                                        if (villageIndex != -1)
                                            atCustomerAddress.cmbVillage.SelectedIndex = villageIndex;
                                        else
                                        {
                                            atCustomerAddress.cmbVillage.Text = string.Empty;
                                            atCustomerAddress.cmbVillage.SelectedText = (string)row[CN.Address2];
                                        }
                                    }
                                    else if (row[CN.Address2] != DBNull.Value)
                                    {
                                        atCustomerAddress.cmbVillage.Text = string.Empty;
                                        atCustomerAddress.cmbVillage.SelectedText = (string)row[CN.Address2];
                                    }
                                    if (atCustomerAddress.cmbRegion.Items.Count > 0 &&
                                    row[CN.Address3] != DBNull.Value) // Address Standardization CR2019 - 025
                                    {
                                        var regionIndex = atCustomerAddress.cmbRegion.FindStringExact((string)row[CN.Address3]);
                                        if (regionIndex != -1)
                                            atCustomerAddress.cmbRegion.SelectedIndex = regionIndex;
                                        else
                                        {
                                            atCustomerAddress.cmbRegion.Text = string.Empty;
                                            atCustomerAddress.cmbRegion.SelectedText = (string)row[CN.Address3];
                                        }
                                    }
                                    else if (row[CN.Address3] != DBNull.Value)
                                    {
                                        atCustomerAddress.cmbRegion.Text = string.Empty;
                                        atCustomerAddress.cmbRegion.SelectedText = (string)row[CN.Address3];
                                    }
                                    atCustomerAddress.txtPostCode.Text = (string)row[CN.PostCode];

                                    atCustomerAddress.Enable = false; // Address Standardization CR2019 - 025
                                }
                            }
                    }

                    if (status)
                    {
                        DataSet acctDetails = AccountManager.GetAccountDetails(accountNo, out Error);
                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                            btnClear_Click(null, null);
                            status = false;
                        }
                        else
                        {
                            foreach (DataTable dt in acctDetails.Tables)
                                if (dt.TableName == TN.AccountDetails)
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        txtArrears.Text = ((decimal)row[CN.Arrears]).ToString(DecimalPlaces);
                                        txtOutstandingBalance.Text = ((decimal)row[CN.OutstandingBalance2]).ToString(DecimalPlaces);
                                        txtAgreementTotal.Text = ((decimal)row[CN.AgreementTotal2]).ToString(DecimalPlaces);
                                        AccountType = (string)row[CN.AccountType2];
                                        AccountStatus = (string)row[CN.AccountStatus2];
                                        ServiceCharge = (decimal)row[CN.ServiceCharge];
                                        txtServiceCharge.Text = ServiceCharge.ToString(DecimalPlaces);
                                    }
                        }

                    }

                    if (status)
                    {
                        DataSet fintrans = PaymentManager.GetAllTransactionsByAccount(accountNo, out Error);
                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                            status = false;
                        }
                        else
                        {
                            dgPayments.DataSource = fintrans.Tables[TN.FinTrans].DefaultView;

                            if (dgPayments.TableStyles.Count == 0)
                            {
                                DataGridTableStyle tabStyle = new DataGridTableStyle();
                                tabStyle.MappingName = TN.FinTrans;
                                AddColumnStyle(CN.TransRefNo, tabStyle, 100, true, GetResource("T_TRANSREFNO"), "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.DateTrans, tabStyle, 120, true, GetResource("T_DATE"), "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.TransTypeCode, tabStyle, 50, true, GetResource("T_TYPE"), "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.EmployeeNo, tabStyle, 100, true, GetResource("T_EMPEENO"), "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.TransValue, tabStyle, 150, true, GetResource("T_VALUE"), DecimalPlaces, HorizontalAlignment.Right);
                                dgPayments.TableStyles.Add(tabStyle);
                            }
                            loaded = true;
                            drpTransaction_SelectedIndexChanged(null, null);
                        }
                    }

                    gbTransaction.Enabled = status;
                }
                //IP - 04/12/2007 - UAT(220)
                if (loaded)
                {
                    lastAccountNo = accountNo;
                }

            }
            catch (Exception ex)
            {
                Catch(ex, "SearchAccountNo");
            }
            finally
            {
                ((MainForm)this.FormRoot).statusBar1.Text = "";
                StopWait();
                loaded = true;
            }

            return status;
        }

        private void LoadStaticData()
        {
            try
            {
                Wait();

                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                if (StaticData.Tables[TN.GeneralTransactionReasons] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.GeneralTransactionReasons, new string[] { "RC1", "RC2", "L" }));
                if (StaticData.Tables[TN.GeneralTransactions] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.GeneralTransactions, null));

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

                drpReason.DataSource = (DataTable)StaticData.Tables[TN.GeneralTransactionReasons];
                drpReason.DisplayMember = CN.CodeDescription;

                drpTransaction.DataSource = (DataTable)StaticData.Tables[TN.GeneralTransactions];
                drpTransaction.DisplayMember = CN.CodeDescription;
            }
            catch (Exception ex)
            {
                Catch(ex, "LoadStaticData");
            }
            finally
            {
                StopWait();
            }
        }

        private void txtAccountNo_Leave(object sender, System.EventArgs e)
        {
            try // UAT 141  UAT57 jec 01/04/10
            {
                if ((bool)(Country[CountryParameterNames.LoyaltyScheme]))
                {
                    decimal amount = 0;
                    string acctno = "";
                    bool active = false;

                    STL.PL.WS3.Loyalty loyaltyinfo = CustomerManager.LoyaltyGetDatabyacctno(txtAccountNo.Text.Trim().Replace("-", ""));
                    if (loyaltyinfo.custid != null && loyaltyinfo.custid.Length > 1)
                    {
                        CustomerManager.LoyaltyGetCharges(loyaltyinfo.custid, ref acctno, ref amount, ref active);
                        if (acctno == txtAccountNo.Text.Trim().Replace("-", ""))
                        {
                            ShowWarning("Financial transactions on Home Club accounts are not allowed.");
                            txtAccountNo.Text = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "SearchAccountNo");
                return;
            }
            if (!this.txtAccountNo.ReadOnly)
                LoadData(txtAccountNo.UnformattedText);
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                loaded = false;

                // Address Standardization CR2019 - 025
                atCustomerAddress.txtAddress1.Text = "";
                atCustomerAddress.cmbVillage.SelectedIndex = -1;
                atCustomerAddress.cmbVillage.Text = "";
                atCustomerAddress.cmbRegion.SelectedIndex = -1;
                atCustomerAddress.cmbRegion.Text = "";
                atCustomerAddress.txtPostCode.Text = "";
                atCustomerAddress.txtCoordinate.Text = "";
                // Address Standardization CR2019 - 025
                txtOutstanding.Text = "";
                txtCustomerName.Text = "";
                txtOutstandingBalance.Text = "";
                txtArrears.Text = "";
                txtAgreementTotal.Text = "";
                txtServiceCharge.Text = "";
                lastAccountNo = txtAccountNo.Text = "000-0000-0000-0";
                txtAccountNo.Focus();
                dgPayments.DataSource = null;
                drpTransaction.SelectedIndex = 0;
                drpReason.SelectedIndex = 0;
                txtAmount.Text = "";
                ServiceCharge = 0;
                total = 0;
                rbCredit.Checked = true;
                errorProvider1.SetError(drpTransaction, "");
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

        private void cbCardPrint_CheckedChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                SetCardPrint();
            }
            catch (Exception ex)
            {
                Catch(ex, "cbCardPrint_CheckedChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void SetCardPrint()
        {
            if (txtAccountNo.Text != "000-0000-0000-0")
            {
                bool canPrintCard = AT.IsCreditType(AccountType);

                cbCardPrint.Checked = (canPrintCard && cbCardPrint.Checked);
            }
            lCardPrintType.Enabled = cbCardPrint.Checked;
            drpCardPrintType.Enabled = cbCardPrint.Checked;
            lCardRowNo.Enabled = cbCardPrint.Checked;
            txtCardRowNo.Enabled = cbCardPrint.Checked;
            btnUpdateCard.Enabled = cbCardPrint.Checked;
            if (cbCardPrint.Checked)
                cbPrintReceipt.Checked = false;
            cbPrintReceipt.Enabled = !cbCardPrint.Checked;
        }

        private void btnUpdateCard_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                if (txtAccountNo.Text != "000-0000-0000-0" &&
                    this.SlipPrinterOK(true))
                {
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_PRINTCARDUPDATE");

                    // Print Payment Card
                    PrintPaymentCardTransactions(LoadTransactions(),
                        Convert.ToInt16(this.txtCardRowNo.Value),
                        Convert.ToDecimal(StripCurrency(this.txtOutstandingBalance.Text)),
                  CustomerID, txtAccountNo.Text, 0, false);

                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnUpdateCard_Click");
            }
            finally
            {
                ((MainForm)this.FormRoot).statusBar1.Text = "";
                StopWait();
            }
        }

        private DataTable LoadTransactions()
        {
            transactions = AccountManager.GetTransactions(txtAccountNo.UnformattedText, out Error);
            if (Error.Length > 0)
                ShowError(Error);

            return transactions.Tables[TN.Transactions];
        }

        private void drpTransaction_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if ((drpTransaction.SelectedIndex < 0 ||
               (string)((DataRowView)drpTransaction.SelectedItem)[CN.Code] == this._lastTransactionType) && sender != null)
            {
                return;
            }
            this._lastTransactionType = (string)((DataRowView)drpTransaction.SelectedItem)[CN.Code];
            try
            {
                if (loaded)
                {
                    bool valid = true;
                    rbDebit.Enabled = true;

                    Wait();

                    total = 0;

                    if (dgPayments.DataSource != null)
                    {
                        DataView dv = (DataView)dgPayments.DataSource;
                        string transType = (string)((DataRowView)drpTransaction.SelectedItem)[CN.Code];

                        switch (transType)
                        {
                            case TransType.CreditAdjustment:
                                if (rbDebit.Checked)
                                    dv.RowFilter = "transtypecode in ('ADJ', 'DDN', 'DDR', 'DDE', 'PAY') and transvalue <= 0";
                                else
                                    dv.RowFilter = "transtypecode in ('DEL', 'GRT')";
                                errorProvider1.SetError(drpTransaction, "");
                                break;
                            case TransType.InsuranceClaim:
                                if (AccountType == AT.Cash)
                                {
                                    errorProvider1.SetError(drpTransaction, GetResource("M_NOTVALIDFORCASH"));
                                    drpTransaction.Focus();
                                    valid = false;
                                }
                                else
                                {
                                    errorProvider1.SetError(drpTransaction, "");
                                    dv.RowFilter = "transtypecode in ('INS') and transvalue * " + creditDebit.ToString() + " <= 0";
                                }
                                break;
                            /* case TransType.BadDebtWriteOff:
                                 if (AccountStatus != "6" ||
                                     Convert.ToDecimal(StripCurrency(txtOutstandingBalance.Text)) < 0)
                                 {
                                     errorProvider1.SetError(drpTransaction, GetResource("M_INVALIDBDW"));
                                     drpTransaction.Focus();
                                     valid = false;
                                 }
                                 else
                                 {
                                     errorProvider1.SetError(drpTransaction, "");
                                     dv.RowFilter = "transtypecode in ('INT','ADM')";
                                     rbCredit.Checked = true;
                                     rbDebit.Enabled = false;
                                 }
                                 break;
                              */
                            // 68473 RD 06/09/06 Display message if account not in status 6 and credit BDW is selected
                            case TransType.BadDebtWriteOff:
                                //LW69125 JH 19/07/2007 Only accounts with a status of 5 or 6 should have the possibility of a BDW
                                dv.RowFilter = "transtypecode in ('INT','ADM')";
                                //lw69699 RDB 30/04/08 only accounts with status 6 should have the possibility of a BDW
                                if (AccountStatus == "6")// || AccountStatus == "5")
                                {
                                    if (rbCredit.Checked)
                                    {
                                        if (Convert.ToDecimal(StripCurrency(txtOutstandingBalance.Text)) <= 0)
                                        {
                                            errorProvider1.SetError(drpTransaction, GetResource("M_INVALIDBDWC"));
                                            drpTransaction.Focus();
                                            valid = false;
                                        }
                                        else
                                        {
                                            errorProvider1.SetError(drpTransaction, "");
                                            //dv.RowFilter = "transtypecode in ('INT','ADM')";
                                            rbCredit.Checked = true;
                                            rbDebit.Enabled = false;
                                            valid = true;
                                        }
                                    }
                                    else if (rbDebit.Checked)
                                    {
                                        errorProvider1.SetError(drpTransaction, GetResource("M_INVALIDBDWD"));
                                        drpTransaction.Focus();
                                        valid = false;
                                    }
                                }
                                else
                                {
                                    errorProvider1.SetError(drpTransaction, GetResource("M_INVALIDBDW"));
                                    valid = false;
                                }
                                //if (AccountStatus != "6" && AccountStatus != "5") //Check if in status code 5 or 6 69125 SC 09/07/07
                                //{
                                //    errorProvider1.SetError(drpTransaction, GetResource("M_INVALIDBDW"));
                                //    valid = false;
                                //}
                                //else
                                //{
                                //    dv.RowFilter = "transtypecode in ('INT','ADM')";
                                //    if (AccountStatus != "6" && rbCredit.Checked &&
                                //         Convert.ToDecimal(StripCurrency(txtOutstandingBalance.Text)) < 0)
                                //    {
                                //        errorProvider1.SetError(drpTransaction, GetResource("M_INVALIDBDWC"));
                                //        drpTransaction.Focus();
                                //        valid = false;
                                //    }
                                //    // 68473 RD 06/09/06 Display message if account not in status 6 and credit BDW is selected
                                //    if (AccountStatus != "S" && rbDebit.Checked)
                                //    {
                                //        errorProvider1.SetError(drpTransaction, GetResource("M_INVALIDBDWD"));
                                //        drpTransaction.Focus();
                                //        valid = false;
                                //    }
                                //    // 68473 RD 06/09/06 Display INT and ADM transactions if account in status 6 and credit BDW is selected
                                //    if (AccountStatus == "6" && rbCredit.Checked &&
                                //         Convert.ToDecimal(StripCurrency(txtOutstandingBalance.Text)) > 0)
                                //    {
                                //        errorProvider1.SetError(drpTransaction, "");
                                //       // dv.RowFilter = "transtypecode in ('INT','ADM')";
                                //        rbCredit.Checked = true;
                                //        rbDebit.Enabled = false;
                                //        valid = true;
                                //    }
                                //    // 68473 RD 06/09/06 Display credit BDW transactions if account settled and Debit BDW is selected
                                //    if (AccountStatus == "S" && rbDebit.Checked)
                                //    {
                                //        errorProvider1.SetError(drpTransaction, "");
                                //        dv.RowFilter = "transtypecode in ('BDW') and transvalue * " + creditDebit.ToString() + " <= 0";
                                //        //rbCredit.Checked = false;
                                //        //rbDebit.Enabled = true;
                                //        valid = true;
                                //    }
                                //}
                                break;
                            case TransType.Redelivery:
                                if (AccountStatus != "6")
                                {
                                    errorProvider1.SetError(drpTransaction, GetResource("M_INVALIDRDL"));
                                    drpTransaction.Focus();
                                    valid = false;
                                }
                                else
                                {
                                    errorProvider1.SetError(drpTransaction, "");
                                    dv.RowFilter = "transtypecode in ('RDL','REP')";
                                }
                                break;
                            case TransType.CreditFee:
                                dv.RowFilter = "transtypecode in ('FEE')";
                                errorProvider1.SetError(drpTransaction, "");
                                break;
                            case TransType.TraceFee:
                                dv.RowFilter = "transtypecode in ('TRC')";
                                errorProvider1.SetError(drpTransaction, "");
                                break;
                            //Cashloan Write off
                            case TransType.CLWriteOff:
                                if (AccountStatus == "6")// || AccountStatus == "5")
                                {
                                    if (rbCredit.Checked)
                                    {
                                        if (Convert.ToDecimal(StripCurrency(txtOutstandingBalance.Text)) <= 0)
                                        {
                                            errorProvider1.SetError(drpTransaction, GetResource("M_INVALIDCLWC"));
                                            drpTransaction.Focus();
                                            valid = false;
                                        }
                                        else
                                        {
                                            errorProvider1.SetError(drpTransaction, "");
                                            //dv.RowFilter = "transtypecode in ('INT','ADM')";
                                            rbCredit.Checked = true;
                                            rbDebit.Enabled = false;
                                            valid = true;
                                        }
                                    }
                                    else if (rbDebit.Checked)
                                    {
                                        errorProvider1.SetError(drpTransaction, GetResource("M_INVALIDCLWC"));
                                        drpTransaction.Focus();
                                        valid = false;
                                    }
                                }
                                else
                                {
                                    errorProvider1.SetError(drpTransaction, GetResource("M_INVALIDCLWAccStatus"));
                                    valid = false;
                                }
                                break;


                            //
                            default:
                                dv.RowFilter = "transtypecode in ('" + transType + "')";
                                errorProvider1.SetError(drpTransaction, "");
                                break;
                        }

                        foreach (DataRowView r in dv)
                            total += (decimal)r[CN.TransValue];






                        if ((transType == "BDW" || transType == "CLW") && valid)
                        {
                            txtAmount.ReadOnly = true;

                            //if( Convert.ToDecimal(StripCurrency(txtOutstandingBalance.Text)) < -total )
                            //	txtAmount.Text = (0).ToString(DecimalPlaces);
                            //else
                            if (transType == "BDW")
                                txtAmount.Text = (Convert.ToDecimal(StripCurrency(txtOutstandingBalance.Text)) - total).ToString(DecimalPlaces);
                            else
                            {
                                txtAmount.Text = PaymentManager.GetEarlySettlementFig(txtAccountNo.Text.Replace("-", "")).ToString();
                            }

                            // if the interest charges exceed the outstanding balance then amount to write off = 0 as int will be reversed first. 
                            if (Convert.ToDecimal(StripCurrency(txtAmount.Text)) < 0)
                                txtAmount.Text = "0";

                            // 68473 RD Added to ensure that users are able to enter amount for BDW Reversal if zero.
                            // if (txtAmount.Text = 0 && rbDebit.Checked)
                            //     txtAmount.ReadOnly = false;
                        }
                        else

                        {
                            txtAmount.ReadOnly = false;
                            txtAmount.Text = ""; //69125 SC 09/07/07
                        }


                        //CR 822 Display a popup for insurance warranty refunds Peter Chong [28-Sep-2006] 
                        if (transType == TransType.InsuranceWarrantyRefund)
                        {
                            this._WarrantyReturnDataSet = null;
                            InsuranceWarrantyPopup p = new InsuranceWarrantyPopup(this.FormRoot, this, txtAccountNo.UnformattedText);
                            p.ShowDialog(this);
                            this._WarrantyReturnDataSet = p.WarrantyReturnDataSet;
                            //this.drpTransaction.SelectedIndex = 0;
                        }

                        //68758 Total should only be sum of totals for non-bdw transactions -- bdw should always be the balance
                        txtOutstanding.Text = total.ToString(DecimalPlaces);
                        btnEnter.Enabled = valid;
                    }
                }

            }
            catch (Exception ex)
            {
                Catch(ex, "drpTransaction_SelectedIndexChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void rbCredit_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rbCredit.Checked)
                creditDebit = -1;
            else
                creditDebit = 1;

            this.drpTransaction_SelectedIndexChanged(null, null);
        }

        private void txtAmount_Leave(object sender, System.EventArgs e)
        {
            bool valid = true;

            try
            {
                Wait();

                /* make sure there is no error on the transaction 
				 * drop down before we proceed */
                if (errorProvider1.GetError(drpTransaction) != "")
                {
                    txtAmount.Text = "";
                    drpTransaction.Focus();
                    valid = false;
                }
                else
                {
                    /* Validate that they've entered something meaningful */
                    if (!IsStrictNumeric(StripCurrency(txtAmount.Text)))
                    {
                        errorProvider1.SetError(txtAmount, GetResource("M_NONNUMERIC"));
                        txtAmount.Select(0, txtAmount.Text.Length);
                        txtAmount.Focus();
                        valid = false;
                    }
                    else
                    {
                        errorProvider1.SetError(txtAmount, "");
                        if (txtAmount.Text.Length > 0)
                            txtAmount.Text = Convert.ToDecimal(StripCurrency(txtAmount.Text)).ToString(DecimalPlaces);
                        else
                            txtAmount.Text = (0).ToString(DecimalPlaces);
                    }

                    if (valid)
                    {
                        /* Make sure the amount they've entered is OK for the transaction type */
                        string transType = (string)((DataRowView)drpTransaction.SelectedItem)[CN.Code];
                        decimal amount = Convert.ToDecimal(StripCurrency(txtAmount.Text));

                        if ((transType == "ADM" ||
                            transType == "TRC" ||
                            transType == "FEE" ||
                            transType == "INT") &&
                            creditDebit == -1)
                        {
                            if (amount > total)
                            {
                                errorProvider1.SetError(txtAmount, GetResource("M_REVERSALAMOUNTTOOHIGH"));
                                txtAmount.Text = total.ToString(DecimalPlaces);
                                txtAmount.Select(0, txtAmount.Text.Length);
                                txtAmount.Focus();
                                valid = false;
                            }
                        }

                        if (transType == "ADJ" &&
                            creditDebit == 1)
                        {
                            if (amount > -total)
                            {
                                errorProvider1.SetError(txtAmount, GetResource("M_REVERSALAMOUNTTOOHIGH"));
                                txtAmount.Text = (-total).ToString(DecimalPlaces);
                                txtAmount.Select(0, txtAmount.Text.Length);
                                txtAmount.Focus();
                                valid = false;
                            }
                        }

                        if (transType == "REB")
                        {
                            if (creditDebit == -1)  /* if it's a credit */
                            {
                                if (amount < 0)
                                    creditDebit = 1;

                                if (amount > ServiceCharge)
                                {
                                    errorProvider1.SetError(txtAmount, GetResource("M_REBATEAMOUNTTOOHIGH"));
                                    txtAmount.Text = ServiceCharge.ToString(DecimalPlaces);
                                    txtAmount.Select(0, txtAmount.Text.Length);
                                    txtAmount.Focus();
                                    valid = false;
                                }

                                if (valid)
                                {
                                    /* make sure we still haven't exceeded service charge.
									/* we have all the fintrans for this account so just temporarily
									/* filter them and add them up. */
                                    DataView dv = (DataView)dgPayments.DataSource;
                                    string temp = dv.RowFilter;
                                    decimal previousRebates = 0;
                                    dv.RowFilter = "transtypecode = 'REB'";

                                    foreach (DataRowView r in dv)
                                        previousRebates += (decimal)r[CN.TransValue];
                                    dv.RowFilter = temp;

                                    if (amount + (creditDebit * previousRebates) > ServiceCharge)
                                    {
                                        errorProvider1.SetError(txtAmount, GetResource("M_REBATEAMOUNTTOOHIGH"));
                                        txtAmount.Text = ServiceCharge.ToString(DecimalPlaces);
                                        txtAmount.Select(0, txtAmount.Text.Length);
                                        txtAmount.Focus();
                                        valid = false;
                                    }
                                }
                            }

                            if (creditDebit == 1) /* if it's a debit */
                            {
                                if (amount < 0)
                                    creditDebit = -1;
                            }
                        }


                        #region CLA Validation
                        // CR           :   Cashloan Outstanding Balance calculation
                        // Details      :   Added new transtype code for cashloan aacount
                        //By            :   Rahul D, Zensar 06/Aug/2019

                        if (transType == TransType.CLAdminFee || transType == TransType.CLAdminReversal || transType == TransType.CLBDRecovey
                            || transType == TransType.CLCreditBalance || transType == TransType.CLLateFeeReversal
                            || transType == TransType.CLPenaltyInterest || transType == TransType.CLServiceChargeCorrection || transType == TransType.CLInsuranceClaim)
                        {
                            int result = PaymentManager.CLGeneralFinanceTransactionValidation(txtAccountNo.Text.Replace("-", ""), transType, creditDebit, out _validationAmount);
                            if (amount > _validationAmount)
                            {
                                errorProvider1.SetError(txtAmount, GetResource("M_REVERSALAMOUNTTOOHIGH"));
                                txtAmount.Text = (_validationAmount).ToString(DecimalPlaces);
                                txtAmount.Focus();
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "txtAmount_Leave");
            }
            finally
            {
                StopWait();
                btnEnter.Enabled = valid;
            }
        }
        private void CollectWarranties()
        {
            if (this._WarrantyReturnDataSet != null)
            {
                if (this._WarrantyReturnDataSet.Tables[0].Rows.Count > 0)
                {
                    AccountManager.SaveInsuranceWarrantyReturns(this._WarrantyReturnDataSet, Error);
                    this._WarrantyReturnDataSet = null;
                    if (Error.Length > 0)
                    {
                        throw new Exception(Error);
                    }
                }
                else
                    return;
            }
        }

        private void btnEnter_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                //IP - 04/12/2007 - UAT(221)
                if (txtAmount.Text == string.Empty)
                {
                    errorProvider1.SetError(txtAmount, GetResource("M_NOAMTENTERED"));
                }
                //else if (this.AccountStatus == "5" && (string)((DataRowView)drpTransaction.SelectedItem)[CN.Code] == "BDW")
                //{
                //    errorProvider1.SetError(drpTransaction, "Unable to perform a BDW off on an account in this status");
                //}
                else if (errorProvider1.GetError(drpTransaction) == String.Empty)
                //if (errorProvider1.GetError(drpTransaction) == String.Empty)
                {
                    // Collect insurance warranties
                    if ((string)((DataRowView)drpTransaction.SelectedItem)[CN.Code] == TransType.InsuranceWarrantyRefund)
                        this.CollectWarranties();

                    PaymentManager.WriteGeneralTransaction(txtAccountNo.UnformattedText,
                                              Convert.ToInt16(Config.BranchCode),
                                              creditDebit * Convert.ToDecimal(StripCurrency(txtAmount.Text)),
                                              (string)((DataRowView)drpTransaction.SelectedItem)[CN.Code],
                                              "", "", "", 0, Config.CountryCode,
                                              (string)((DataRowView)drpReason.SelectedItem)[CN.Code],
                                              creditDebit, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        PrintReceipt();
                        btnClear_Click(null, null);
                    }
                }

            }
            catch (Exception ex)
            {
                Catch(ex, "btnEnter_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void PrintReceipt()
        {
            DataTable trans = null;
            if (this.cbCardPrint.Checked)
                btnUpdateCard_Click(null, null);
            else
            {
                if (cbPrintReceipt.Checked)
                {
                    decimal newBalance = Convert.ToDecimal(StripCurrency(txtOutstandingBalance.Text)) +
                        (creditDebit * Convert.ToDecimal(StripCurrency(txtAmount.Text)));
                    txtOutstandingBalance.Text = newBalance.ToString(DecimalPlaces);

                    trans = LoadTransactions();
                    DataView dv = new DataView(trans, "", CN.DateTrans + " DESC ", DataViewRowState.OriginalRows);
                    dv.AllowNew = false;
                    if (dv.Count > 0)
                    {
                        // Remove transactions with an earlier date/time
                        DateTime latestTransDate = Convert.ToDateTime(dv[0][CN.DateTrans].ToString());
                        foreach (DataRowView r in dv)
                            if (Convert.ToDateTime(r[CN.DateTrans].ToString()) != latestTransDate)
                                r.Delete();

                        trans.AcceptChanges();

                        PrintPaymentReceipt(trans,
                            txtCustomerName.Text,
                            txtAccountNo.Text,
                            newBalance,
                            0);
                    }
                }
            }
        }

        private void drpTransaction_KeyUp(object sender, KeyEventArgs e)
        {
            this._lastTransactionType = drpTransaction.Text;
        }
    }
}
