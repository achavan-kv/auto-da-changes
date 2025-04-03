using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using STL.Common;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.CustomerTypes;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.TableNames;
using STL.Common.Static;

namespace STL.PL
{
    /// <summary>
    /// A refund or correction of a customer payment can be entered here. The 
    /// payments for a specified account are listed and can be individually selected
    /// for either a refund or a correction. A payment can be corrected by returning
    /// the same amount on the same day that the payment was taken. The user is not
    /// able to amend the amount being corrected. For payments that are more than a
    /// day old a refund has to be made. A refund can be the value of the selected
    /// payment, or any amount that the user chooses to enter.
    /// Printing options allow the customer payment card to be updated or a receipt
    /// to be printed.
    /// </summary>
    public class RefundAndCorrection : CommonForm
    {
        //private Crownwood.Magic.Menus.MenuControl menuMain;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.GroupBox gbCustomer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSearchAccount;
        private STL.PL.AccountTextBox txtAccountNo;
        private System.Windows.Forms.Label lAccountNo;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtArrears;
        private System.Windows.Forms.TextBox txtOutstandingBalance;
        private System.Windows.Forms.Label lArrears;
        private System.Windows.Forms.TextBox txtAgreementTotal;
        private System.Windows.Forms.Label lAgreementTotal;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnEnter;
        private STL.PL.AddressTab atCustomerAddress;
        private System.Windows.Forms.ComboBox drpPayMethod;
        private System.Windows.Forms.Label lCardNo;
        private System.Windows.Forms.TextBox txtCardNo;
        private System.Windows.Forms.Label lPayMethod;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.ComboBox drpReason;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox gbPayments;
        private System.Windows.Forms.DataGrid dgPayments;
        private System.Windows.Forms.CheckBox cbCardPrint;
        private string lastAccountNo = "";
        private new string Error = "";
        private string CustomerID = "";
        private System.Windows.Forms.RadioButton rbCorrection;
        private System.Windows.Forms.RadioButton rbRefund;
        private System.Windows.Forms.GroupBox gbTransaction;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label authCorrection;
        private System.Windows.Forms.Label authRefund;
        private System.Windows.Forms.Button btnAccountDetails;
        private System.Windows.Forms.Button btnUpdateCard;
        private System.Windows.Forms.NumericUpDown txtCardRowNo;
        private System.Windows.Forms.Label lCardRowNo;
        private System.Windows.Forms.Label lCardPrintType;
        private string AccountType = "";
        private System.Windows.Forms.ToolTip ttPayment;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ComboBox drpCardPrintType;
        private System.Windows.Forms.ImageList imageList1;
        private Button btnBDWReverse;
        private Label lblBDWReverse;
        private DataSet transactions = null;
        private DataView Deposits = null;
        private DataTable dtCLMambuPayMethod = null;
        private DataTable dtHPMambuPayMethod = null;
        private bool StorecardReverse = false;
        private bool isMambuAccount = false;
        private decimal refundAmount = 0;
        private short paymentMethod = 0;
        const string DefaultPaymentFilter = CN.Additional + " in ('B', 'P', '') and code < 200 and code >= 0";
        private int FintransId = 0;
        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":authCorrection"] = this.authCorrection;
            dynamicMenus[this.Name + ":authRefund"] = this.authRefund;
        }

        public RefundAndCorrection(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
        }

        public RefundAndCorrection(Form root, Form parent)
        {
            FormRoot = root;
            FormParent = parent;
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });

            HashMenus();


        }

        //IP - 28/08/09 - UAT(737) - New constructor called from the Telephone Action screen if the 
        //action code 'RBDW - Reverse BDW' is selected.
        public RefundAndCorrection(Form root, Form parent, string acctNo)
        {
            FormRoot = root;
            FormParent = parent;
            InitializeComponent();
            this.RefundAndCorrection_Load(null, null);

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });

            HashMenus();

            //Screen has been called from Telephone Action screen, therefore load the details for the selected account
            //preventing the user from entering or searching for a different account.
            txtAccountNo.Text = acctNo;
            txtAccountNo.ReadOnly = true;
            btnSearchAccount.Enabled = false;
            LoadData(txtAccountNo.Text);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RefundAndCorrection));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.gbCustomer = new System.Windows.Forms.GroupBox();
            this.btnBDWReverse = new System.Windows.Forms.Button();
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
            this.atCustomerAddress = new STL.PL.AddressTab(FormRoot);       //CR1084 jec
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnEnter = new System.Windows.Forms.Button();
            this.drpPayMethod = new System.Windows.Forms.ComboBox();
            this.lCardNo = new System.Windows.Forms.Label();
            this.txtCardNo = new System.Windows.Forms.TextBox();
            this.lPayMethod = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.drpReason = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.gbTransaction = new System.Windows.Forms.GroupBox();
            this.drpCardPrintType = new System.Windows.Forms.ComboBox();
            this.btnUpdateCard = new System.Windows.Forms.Button();
            this.txtCardRowNo = new System.Windows.Forms.NumericUpDown();
            this.lCardRowNo = new System.Windows.Forms.Label();
            this.lCardPrintType = new System.Windows.Forms.Label();
            this.rbRefund = new System.Windows.Forms.RadioButton();
            this.rbCorrection = new System.Windows.Forms.RadioButton();
            this.cbCardPrint = new System.Windows.Forms.CheckBox();
            this.gbPayments = new System.Windows.Forms.GroupBox();
            this.authRefund = new System.Windows.Forms.Label();
            this.authCorrection = new System.Windows.Forms.Label();
            this.dgPayments = new System.Windows.Forms.DataGrid();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.ttPayment = new System.Windows.Forms.ToolTip(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.lblBDWReverse = new System.Windows.Forms.Label();
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
            this.gbCustomer.Controls.Add(this.atCustomerAddress);
            this.gbCustomer.Controls.Add(this.txtCustomerName);
            this.gbCustomer.Controls.Add(this.btnExit);
            this.gbCustomer.Controls.Add(this.btnClear);
            this.gbCustomer.Location = new System.Drawing.Point(8, 0);
            this.gbCustomer.Name = "gbCustomer";
            this.gbCustomer.Size = new System.Drawing.Size(776, 168);
            this.gbCustomer.TabIndex = 0;
            this.gbCustomer.TabStop = false;
            this.gbCustomer.Text = "Reference";
            // 
            // btnBDWReverse
            // 
            this.btnBDWReverse.Location = new System.Drawing.Point(496, 87);
            this.btnBDWReverse.Name = "btnBDWReverse";
            this.btnBDWReverse.Size = new System.Drawing.Size(112, 23);
            this.btnBDWReverse.TabIndex = 67;
            this.btnBDWReverse.Text = "Reverse BDW";
            this.btnBDWReverse.UseVisualStyleBackColor = true;
            this.btnBDWReverse.Visible = false;
            this.btnBDWReverse.Click += new System.EventHandler(this.btnBDWReverse_Click);
            // 
            // btnAccountDetails
            // 
            this.btnAccountDetails.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAccountDetails.Image = ((System.Drawing.Image)(resources.GetObject("btnAccountDetails.Image")));
            this.btnAccountDetails.Location = new System.Drawing.Point(672, 24);
            this.btnAccountDetails.Name = "btnAccountDetails";
            this.btnAccountDetails.Size = new System.Drawing.Size(24, 24);
            this.btnAccountDetails.TabIndex = 30;
            this.btnAccountDetails.Click += new System.EventHandler(this.btnAccountDetails_Click);
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(552, 95);
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
            this.txtArrears.Location = new System.Drawing.Point(552, 72);
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
            this.txtOutstandingBalance.Location = new System.Drawing.Point(552, 112);
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
            this.lArrears.Location = new System.Drawing.Point(552, 56);
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
            this.txtAgreementTotal.Location = new System.Drawing.Point(552, 32);
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
            this.lAgreementTotal.Location = new System.Drawing.Point(552, 16);
            this.lAgreementTotal.Name = "lAgreementTotal";
            this.lAgreementTotal.Size = new System.Drawing.Size(100, 16);
            this.lAgreementTotal.TabIndex = 22;
            this.lAgreementTotal.Text = "Agreement Total";
            this.lAgreementTotal.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(248, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 16);
            this.label2.TabIndex = 17;
            this.label2.Text = "Customer Address";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(248, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 16;
            this.label1.Text = "Customer Name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // btnSearchAccount
            // 
            this.btnSearchAccount.Image = ((System.Drawing.Image)(resources.GetObject("btnSearchAccount.Image")));
            this.btnSearchAccount.Location = new System.Drawing.Point(144, 40);
            this.btnSearchAccount.Name = "btnSearchAccount";
            this.btnSearchAccount.Size = new System.Drawing.Size(20, 20);
            this.btnSearchAccount.TabIndex = 1;
            this.btnSearchAccount.Click += new System.EventHandler(this.btnSearchAccount_Click);
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtAccountNo.Location = new System.Drawing.Point(40, 40);
            this.txtAccountNo.MaxLength = 20;
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.PreventPaste = false;
            this.txtAccountNo.Size = new System.Drawing.Size(88, 20);
            this.txtAccountNo.TabIndex = 0;
            this.txtAccountNo.Text = "000-0000-0000-0";
            this.txtAccountNo.Leave += new System.EventHandler(this.txtAccountNo_Leave);
            // 
            // lAccountNo
            // 
            this.lAccountNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lAccountNo.Location = new System.Drawing.Point(40, 24);
            this.lAccountNo.Name = "lAccountNo";
            this.lAccountNo.Size = new System.Drawing.Size(72, 16);
            this.lAccountNo.TabIndex = 8;
            this.lAccountNo.Text = "Account No";
            this.lAccountNo.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // atCustomerAddress
            // 
            this.atCustomerAddress.Location = new System.Drawing.Point(240, 53);
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
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.SystemColors.Window;
            this.txtCustomerName.Location = new System.Drawing.Point(248, 32);
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
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
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
            // btnEnter
            // 
            this.btnEnter.Enabled = false;
            this.btnEnter.Location = new System.Drawing.Point(712, 40);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(48, 23);
            this.btnEnter.TabIndex = 5;
            this.btnEnter.Text = "Enter";
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // drpPayMethod
            // 
            this.drpPayMethod.BackColor = System.Drawing.SystemColors.Window;
            this.drpPayMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPayMethod.Enabled = false;
            this.drpPayMethod.Location = new System.Drawing.Point(384, 40);
            this.drpPayMethod.Name = "drpPayMethod";
            this.drpPayMethod.Size = new System.Drawing.Size(104, 21);
            this.drpPayMethod.TabIndex = 2;
            // 
            // lCardNo
            // 
            this.lCardNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lCardNo.Location = new System.Drawing.Point(496, 24);
            this.lCardNo.Name = "lCardNo";
            this.lCardNo.Size = new System.Drawing.Size(80, 16);
            this.lCardNo.TabIndex = 55;
            this.lCardNo.Text = "Cheque No";
            this.lCardNo.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtCardNo
            // 
            this.txtCardNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtCardNo.Enabled = false;
            this.txtCardNo.Location = new System.Drawing.Point(496, 40);
            this.txtCardNo.MaxLength = 30;
            this.txtCardNo.Name = "txtCardNo";
            this.txtCardNo.Size = new System.Drawing.Size(128, 20);
            this.txtCardNo.TabIndex = 3;
            // 
            // lPayMethod
            // 
            this.lPayMethod.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lPayMethod.Location = new System.Drawing.Point(384, 24);
            this.lPayMethod.Name = "lPayMethod";
            this.lPayMethod.Size = new System.Drawing.Size(80, 16);
            this.lPayMethod.TabIndex = 54;
            this.lPayMethod.Text = "Pay Method";
            this.lPayMethod.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(104, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 16);
            this.label4.TabIndex = 59;
            this.label4.Text = "Amount";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtAmount
            // 
            this.txtAmount.BackColor = System.Drawing.SystemColors.Window;
            this.txtAmount.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtAmount.Location = new System.Drawing.Point(104, 40);
            this.txtAmount.MaxLength = 10;
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.ReadOnly = true;
            this.txtAmount.Size = new System.Drawing.Size(96, 21);
            this.txtAmount.TabIndex = 0;
            this.txtAmount.TabStop = false;
            this.txtAmount.Leave += new System.EventHandler(this.txtAmount_Leave);
            // 
            // drpReason
            // 
            this.drpReason.BackColor = System.Drawing.SystemColors.Window;
            this.drpReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpReason.Location = new System.Drawing.Point(208, 40);
            this.drpReason.Name = "drpReason";
            this.drpReason.Size = new System.Drawing.Size(168, 21);
            this.drpReason.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(208, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 16);
            this.label5.TabIndex = 60;
            this.label5.Text = "Reason";
            this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // gbTransaction
            // 
            this.gbTransaction.BackColor = System.Drawing.SystemColors.Control;
            this.gbTransaction.Controls.Add(this.lblBDWReverse);
            this.gbTransaction.Controls.Add(this.btnBDWReverse);
            this.gbTransaction.Controls.Add(this.drpCardPrintType);
            this.gbTransaction.Controls.Add(this.btnUpdateCard);
            this.gbTransaction.Controls.Add(this.txtCardRowNo);
            this.gbTransaction.Controls.Add(this.lCardRowNo);
            this.gbTransaction.Controls.Add(this.lCardPrintType);
            this.gbTransaction.Controls.Add(this.rbRefund);
            this.gbTransaction.Controls.Add(this.rbCorrection);
            this.gbTransaction.Controls.Add(this.cbCardPrint);
            this.gbTransaction.Controls.Add(this.drpPayMethod);
            this.gbTransaction.Controls.Add(this.txtAmount);
            this.gbTransaction.Controls.Add(this.label4);
            this.gbTransaction.Controls.Add(this.lPayMethod);
            this.gbTransaction.Controls.Add(this.lCardNo);
            this.gbTransaction.Controls.Add(this.txtCardNo);
            this.gbTransaction.Controls.Add(this.drpReason);
            this.gbTransaction.Controls.Add(this.label5);
            this.gbTransaction.Controls.Add(this.btnEnter);
            this.gbTransaction.Enabled = false;
            this.gbTransaction.Location = new System.Drawing.Point(8, 168);
            this.gbTransaction.Name = "gbTransaction";
            this.gbTransaction.Size = new System.Drawing.Size(776, 120);
            this.gbTransaction.TabIndex = 1;
            this.gbTransaction.TabStop = false;
            this.gbTransaction.Text = "Transaction Details";
            // 
            // drpCardPrintType
            // 
            this.drpCardPrintType.BackColor = System.Drawing.SystemColors.Window;
            this.drpCardPrintType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCardPrintType.Enabled = false;
            this.drpCardPrintType.Location = new System.Drawing.Point(104, 88);
            this.drpCardPrintType.Name = "drpCardPrintType";
            this.drpCardPrintType.Size = new System.Drawing.Size(112, 21);
            this.drpCardPrintType.TabIndex = 66;
            // 
            // btnUpdateCard
            // 
            this.btnUpdateCard.Enabled = false;
            this.btnUpdateCard.Image = ((System.Drawing.Image)(resources.GetObject("btnUpdateCard.Image")));
            this.btnUpdateCard.Location = new System.Drawing.Point(304, 88);
            this.btnUpdateCard.Name = "btnUpdateCard";
            this.btnUpdateCard.Size = new System.Drawing.Size(20, 20);
            this.btnUpdateCard.TabIndex = 65;
            this.btnUpdateCard.Click += new System.EventHandler(this.btnUpdateCard_Click);
            // 
            // txtCardRowNo
            // 
            this.txtCardRowNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtCardRowNo.Enabled = false;
            this.txtCardRowNo.Location = new System.Drawing.Point(232, 88);
            this.txtCardRowNo.Name = "txtCardRowNo";
            this.txtCardRowNo.Size = new System.Drawing.Size(48, 20);
            this.txtCardRowNo.TabIndex = 64;
            // 
            // lCardRowNo
            // 
            this.lCardRowNo.Enabled = false;
            this.lCardRowNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lCardRowNo.Location = new System.Drawing.Point(232, 72);
            this.lCardRowNo.Name = "lCardRowNo";
            this.lCardRowNo.Size = new System.Drawing.Size(72, 16);
            this.lCardRowNo.TabIndex = 61;
            this.lCardRowNo.Text = "Row Number";
            // 
            // lCardPrintType
            // 
            this.lCardPrintType.Enabled = false;
            this.lCardPrintType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lCardPrintType.Location = new System.Drawing.Point(104, 72);
            this.lCardPrintType.Name = "lCardPrintType";
            this.lCardPrintType.Size = new System.Drawing.Size(96, 16);
            this.lCardPrintType.TabIndex = 62;
            this.lCardPrintType.Text = "Card Print Type";
            // 
            // rbRefund
            // 
            this.rbRefund.Location = new System.Drawing.Point(24, 48);
            this.rbRefund.Name = "rbRefund";
            this.rbRefund.Size = new System.Drawing.Size(80, 24);
            this.rbRefund.TabIndex = 7;
            this.rbRefund.Text = "Refund";
            this.rbRefund.CheckedChanged += new System.EventHandler(this.rbRefund_CheckedChanged);
            // 
            // rbCorrection
            // 
            this.rbCorrection.Checked = true;
            this.rbCorrection.Location = new System.Drawing.Point(24, 24);
            this.rbCorrection.Name = "rbCorrection";
            this.rbCorrection.Size = new System.Drawing.Size(80, 24);
            this.rbCorrection.TabIndex = 6;
            this.rbCorrection.TabStop = true;
            this.rbCorrection.Text = "Correction";
            this.rbCorrection.CheckedChanged += new System.EventHandler(this.rbCorrection_CheckedChanged);
            // 
            // cbCardPrint
            // 
            this.cbCardPrint.BackColor = System.Drawing.SystemColors.Control;
            this.cbCardPrint.Location = new System.Drawing.Point(24, 80);
            this.cbCardPrint.Name = "cbCardPrint";
            this.cbCardPrint.Size = new System.Drawing.Size(72, 32);
            this.cbCardPrint.TabIndex = 4;
            this.cbCardPrint.Text = "Payment Card";
            this.cbCardPrint.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.cbCardPrint.UseVisualStyleBackColor = false;
            this.cbCardPrint.CheckedChanged += new System.EventHandler(this.cbCardPrint_CheckedChanged);
            // 
            // gbPayments
            // 
            this.gbPayments.BackColor = System.Drawing.SystemColors.Control;
            this.gbPayments.Controls.Add(this.authRefund);
            this.gbPayments.Controls.Add(this.authCorrection);
            this.gbPayments.Controls.Add(this.dgPayments);
            this.gbPayments.Location = new System.Drawing.Point(8, 288);
            this.gbPayments.Name = "gbPayments";
            this.gbPayments.Size = new System.Drawing.Size(776, 184);
            this.gbPayments.TabIndex = 2;
            this.gbPayments.TabStop = false;
            this.gbPayments.Text = "Payments on Account";
            // 
            // authRefund
            // 
            this.authRefund.Enabled = false;
            this.authRefund.Location = new System.Drawing.Point(672, 93);
            this.authRefund.Name = "authRefund";
            this.authRefund.Size = new System.Drawing.Size(80, 23);
            this.authRefund.TabIndex = 2;
            this.authRefund.Visible = false;
            // 
            // authCorrection
            // 
            this.authCorrection.Enabled = false;
            this.authCorrection.Location = new System.Drawing.Point(672, 56);
            this.authCorrection.Name = "authCorrection";
            this.authCorrection.Size = new System.Drawing.Size(80, 23);
            this.authCorrection.TabIndex = 1;
            this.authCorrection.Visible = false;
            // 
            // dgPayments
            // 
            this.dgPayments.CaptionVisible = false;
            this.dgPayments.DataMember = "";
            this.dgPayments.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgPayments.Location = new System.Drawing.Point(88, 40);
            this.dgPayments.Name = "dgPayments";
            this.dgPayments.ReadOnly = true;
            this.dgPayments.Size = new System.Drawing.Size(568, 120);
            this.dgPayments.TabIndex = 0;
            this.dgPayments.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgPayments_MouseUp);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            // 
            // lblBDWReverse
            // 
            this.lblBDWReverse.AutoSize = true;
            this.lblBDWReverse.Location = new System.Drawing.Point(493, 71);
            this.lblBDWReverse.Name = "lblBDWReverse";
            this.lblBDWReverse.Size = new System.Drawing.Size(145, 13);
            this.lblBDWReverse.TabIndex = 68;
            this.lblBDWReverse.Text = "This account has been BDW";
            this.lblBDWReverse.Visible = false;
            // 
            // RefundAndCorrection
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.gbPayments);
            this.Controls.Add(this.gbTransaction);
            this.Controls.Add(this.gbCustomer);
            this.Name = "RefundAndCorrection";
            this.Text = "Refunds and Corrections";
            this.Load += new System.EventHandler(this.RefundAndCorrection_Load);
            this.gbCustomer.ResumeLayout(false);
            this.gbCustomer.PerformLayout();
            this.gbTransaction.ResumeLayout(false);
            this.gbTransaction.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCardRowNo)).EndInit();
            this.gbPayments.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgPayments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private bool SetCanBeCorrected()
        {
            ((MainForm)this.FormRoot).statusBar1.Text = "";
            int index = dgPayments.CurrentRowIndex;
            if (index >= 0)
            {
                DataRowView curRow = ((DataView)dgPayments.DataSource)[dgPayments.CurrentRowIndex];
                bool canCorrect = true;

                // Do not allow correction of a payment already corrected
                if (curRow[CN.Corrected].ToString() == "1")
                {
                    // Already corrected
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_CORRECTEDPAY");
                    canCorrect = false;
                }
                else if (curRow[CN.TransTypeCode].ToString() == TransType.DebtPayment)
                {
                    // BDW (DPY) payment to correct against written off account
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_CORRECTBDW");
                }
                this.rbRefund.Enabled = true;

                if (curRow[CN.TransTypeCode].ToString() == TransType.StoreCardPayment)
                {
                    canCorrect = true;
                    this.rbRefund.Enabled = false;
                    StorecardReverse = true;
                }

                // Do not allow correction of a payment not paid on today's date unless it's a Giro payment
                // RD 18/11/04 FR66185 Allow giro transaction types as well but on any date.
                else if (System.Convert.ToDateTime(curRow[CN.DateTrans]).ToShortDateString() != StaticDataManager.GetServerDate().ToShortDateString() &&
                    curRow[CN.TransTypeCode].ToString() != TransType.GiroNormal &&
                    curRow[CN.TransTypeCode].ToString() != TransType.GiroExtra &&
                    curRow[CN.TransTypeCode].ToString() != TransType.GiroFeePaid &&
                    curRow[CN.TransTypeCode].ToString() != TransType.GiroRepresent)
                {
                    // Transaction date not today (and not DPY) (and not Giro)
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_CORRECTTODAY");
                    canCorrect = false;
                }

                this.rbCorrection.Enabled = canCorrect;
                this.rbCorrection.Checked = canCorrect;
                this.rbRefund.Checked = !canCorrect;
            }
            this.SetRefundOrCorrection();
            return this.rbCorrection.Enabled;
        }

        private void SetRefundOrCorrection()
        {
            txtCardNo.Text = "";
            drpPayMethod.SelectedIndex = 0;

            txtCardNo.Enabled = !rbCorrection.Checked;
            drpPayMethod.Enabled = !rbCorrection.Checked;

            if (rbCorrection.Checked || BDW.IsBDW(this.CustomerID))
            {
                if (((DataView)dgPayments.DataSource).Count > 0)
                {
                    txtAmount.Text = ((decimal)((DataView)dgPayments.DataSource)[dgPayments.CurrentRowIndex][CN.TransValue]).ToString(DecimalPlaces);
                    btnEnter.Enabled = true;

                    foreach (DataRowView r in drpPayMethod.Items)
                    {
                        if ((string)r[CN.Code] == (string)((DataView)dgPayments.DataSource)[dgPayments.CurrentRowIndex][CN.Code])
                        {
                            drpPayMethod.SelectedItem = r;
                            break;
                        }
                    }
                }
                else
                    btnEnter.Enabled = false;

                txtAmount.ReadOnly = true;
                drpReason.DataSource = (DataTable)StaticData.Tables[TN.CorrectionReasons];
            }
            else if (rbRefund.Checked)
            {
                btnEnter.Enabled = true;
                txtAmount.ReadOnly = false;
                if (isMambuAccount)
                {
                    txtAmount.ReadOnly = true;
                    txtAmount.Text = ((decimal)((DataView)dgPayments.DataSource)[dgPayments.CurrentRowIndex][CN.TransValue]).ToString(DecimalPlaces);
                }
                drpReason.DataSource = (DataTable)StaticData.Tables[TN.RefundReasons];
                ((MainForm)this.FormRoot).statusBar1.Text = "";
            }

        }

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }

        private void btnSearchAccount_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                bool status = true;
                //UAT 1 - jec 16/04/10
                if ((bool)Country[CountryParameterNames.BlockCashGoInRefundCorrection] && txtAccountNo.Text.Trim() != ""
                        && AccountManager.IsPaidAndTakenAccount(txtAccountNo.Text.Trim().Replace("-", ""), out Error)) //CR 1037
                {
                    txtAccountNo.Text = "";
                    ShowWarning(GetResource("M_PAID_AND_TAKEN_DISALLOWED"));
                    CloseTab();
                    SearchCashAndGo search = new SearchCashAndGo(FormRoot, FormParent);
                    ((MainForm)FormParent).AddTabPage(search);

                }
                else
                {
                    // FA - UAT 637 - only create Account Search tab if account searched for doesn't exist
                    if (!this.txtAccountNo.ReadOnly)
                    {
                        //IP - 14/04/10 - UAT(57) -  Do not allow HCC accounts to be loadded.
                        status = IsHomeClubAcct();

                        if (status == true)
                        {
                            if (!LoadData(txtAccountNo.Text))
                            {
                                AccountSearch a = new AccountSearch();
                                a.FormRoot = this.FormRoot;
                                a.FormParent = this;
                                ((MainForm)this.FormRoot).AddTabPage(a, 7);
                            }
                        }

                    }
                }

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

        private void SetMambuPaymentMethod()
        {

            string accountNo = txtAccountNo.Text.Replace("-", ""); ;
            if (accountNo != "000000000000")
            {

                if (AccountManager.CheckAccountType(accountNo, out Error) && isMambuAccount == true)
                {
                    //Get the PayMethods based on the cash loan created
                    if (dtCLMambuPayMethod != null)
                        dtCLMambuPayMethod = null;
                    dtCLMambuPayMethod = PaymentManager.GetMambuCLPaymentMethods(accountNo, out Error);
                    //Set drpPaymentMethod DataSource
                    drpPayMethod.DataSource = dtCLMambuPayMethod.DefaultView;
                    drpPayMethod.ValueMember = CN.Code;
                    drpPayMethod.DisplayMember = CN.CodeDescription;
                    refundAmount = 0;
                }
                else if (!AccountManager.CheckAccountType(accountNo, out Error) && isMambuAccount == true)
                {
                    //Set drpPaymentMethod DataSource
                    drpPayMethod.DataSource = dtHPMambuPayMethod.DefaultView;
                    drpPayMethod.ValueMember = CN.Code;
                    drpPayMethod.DisplayMember = CN.CodeDescription;
                    if (refundAmount > 0 && paymentMethod < 200)
                    {
                        if (PaymentManager.CheckIfDepositPayment(accountNo, refundAmount, out Error) == true)
                        {
                            DataTable dtPayMethod = ((DataTable)StaticData.Tables[TN.PayMethod]).Copy();
                            dtPayMethod.DefaultView.RowFilter = DefaultPaymentFilter;
                            drpPayMethod.DataSource = dtPayMethod.DefaultView;
                            drpPayMethod.ValueMember = CN.Code;
                            drpPayMethod.DisplayMember = CN.CodeDescription;

                        }

                    }
                }
                else if (isMambuAccount == false)
                {
                    DataTable dtPayMethod = ((DataTable)StaticData.Tables[TN.PayMethod]).Copy();
                    dtPayMethod.DefaultView.RowFilter = DefaultPaymentFilter;
                    drpPayMethod.DataSource = dtPayMethod.DefaultView;
                    drpPayMethod.ValueMember = CN.Code;
                    drpPayMethod.DisplayMember = CN.CodeDescription;
                    refundAmount = 0;
                }
            }



        }
        public bool LoadData(string accountNo)
        {
            bool status = true;
            DataView PaymentView = null;

            try
            {
                // Check an Account No has been entered
                txtAccountNo.Text = accountNo.Trim();
                accountNo = accountNo.Replace("-", "");
                // FA - UAT 637
                if (accountNo == "000000000000")
                {
                    status = false;
                }
                else
                    if (accountNo != lastAccountNo)
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
                        txtCustomerName.Text = (string)r[CN.Title] +
                            " " + (string)r[CN.FirstName] +
                            " " + (string)r[CN.LastName];
                        CustomerID = (string)r[CN.CustomerID];

                        DataSet addresses = CustomerManager.GetCustomerAddresses(CustomerID, out Error);
                        foreach (DataTable dt in addresses.Tables)
                            foreach (DataRow row in dt.Rows)
                            {
                                if (((string)row[CN.AddressType]).Trim() == "H")
                                {
                                    atCustomerAddress.txtAddress1.Text = (string)row[CN.Address1];
                                    if (atCustomerAddress.cmbVillage.Items.Count > 0 &&
                                   row[CN.Address2] != DBNull.Value)
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
                                    row[CN.Address3] != DBNull.Value)
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
                                        AccountType.Trim();
                                        // uat376 rdb BDW Reversal
                                        if (Convert.ToString(row["Current Status"]) == "S" &&
                                           Convert.ToDecimal(row["bdwbalance"]) > 0 || //IP - 16/12/09 - UAT(940) - Changed from && to ||
                                           Convert.ToDecimal(row["bdwCharges"]) > 0)
                                        {
                                            lblBDWReverse.Visible = true;
                                            btnBDWReverse.Visible = true;
                                        }
                                    }
                        }

                    }

                    if (status)
                    {
                        /* load the payment transactions for this account */
                        DataSet payments = PaymentManager.GetAccountPayments(accountNo, out Error);
                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                            btnClear_Click(null, null);
                            status = false;
                        }
                        else
                        {
                            // Display list of Payments
                            PaymentView = new DataView(payments.Tables[0]);
                            PaymentView.AllowNew = false;
                            PaymentView.Sort = CN.DateTrans + " ASC ";
                            dgPayments.DataSource = PaymentView;

                            if (dgPayments.TableStyles.Count == 0)
                            {
                                DataGridTableStyle tabStyle = new DataGridTableStyle();
                                tabStyle.MappingName = PaymentView.Table.TableName;

                                // Add an unbound stand-alone icon column to mark corrected payments
                                PaymentView.Table.Columns.Add("Icon");
                                DataGridIconColumn iconColumn = new DataGridIconColumn(imageList1.Images[0], CN.Corrected, "0");
                                iconColumn.HeaderText = "";
                                iconColumn.MappingName = "Icon";
                                iconColumn.Width = imageList1.Images[0].Size.Width;
                                tabStyle.GridColumnStyles.Add(iconColumn);

                                AddColumnStyle(CN.TransRefNo, tabStyle, 100, true, GetResource("T_REFNO"), "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.DateTrans, tabStyle, 120, true, GetResource("T_DATE"), "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.Code, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.CodeDescription, tabStyle, 142, true, GetResource("T_PAYMETHOD"), "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.TransValue, tabStyle, 150, true, GetResource("T_VALUE"), DecimalPlaces, HorizontalAlignment.Right);
                                AddColumnStyle(CN.ChequeNo, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.BankCode, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.BankAccountNo2, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.BranchNo, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.ID, tabStyle, 0, true, "", "", HorizontalAlignment.Left);

                                dgPayments.TableStyles.Clear();
                                dgPayments.TableStyles.Add(tabStyle);
                                dgPayments.DataSource = PaymentView;
                            }
                            else
                            {
                                // Add an unbound stand-alone icon column to mark corrected transactions
                                PaymentView.Table.Columns.Add("Icon");
                            }
                        }

                        if (((DataView)dgPayments.DataSource).Count > 0)
                        {
                            dgPayments.CurrentRowIndex = 0;
                            this.SetCanBeCorrected();
                            //btnEnter.Enabled = true;
                            //txtAmount.Text = ((decimal)((DataView)dgPayments.DataSource)[0][CN.TransValue]).ToString(DecimalPlaces);
                        }
                    }

                    lastAccountNo = txtAccountNo.Text.Replace("-", "");
                    gbTransaction.Enabled = status;
                    DataRow r1 = CustomerSet.Tables["Mambu"].Rows[0];
                    isMambuAccount = (bool)r1["isMambuAccount"];
                    SetMambuPaymentMethod();
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
            }

            return status;
        }

        private void txtAccountNo_Leave(object sender, System.EventArgs e)
        {
            //IP - 08/02/10 - CR1037 Merged - Malaysia Enhancements (CR1072)
            //--- CR 1037 --------------------------------------------------------------------
            try
            {
                if ((bool)Country[CountryParameterNames.BlockCashGoInRefundCorrection] && txtAccountNo.Text.Trim() != ""
                        && AccountManager.IsPaidAndTakenAccount(txtAccountNo.Text.Trim().Replace("-", ""), out Error)) //CR 1037
                {
                    txtAccountNo.Text = "";
                    ShowWarning(GetResource("M_PAID_AND_TAKEN_DISALLOWED"));
                    CloseTab();
                    SearchCashAndGo search = new SearchCashAndGo(FormRoot, FormParent);
                    ((MainForm)FormParent).AddTabPage(search);

                }

            }
            catch (Exception ex)
            {
                Catch(ex, "SearchAccountNo");
                return;
            }
            //--------------------------------------------------------------------------------

            // FA - UAT 637
            //if (!this.txtAccountNo.ReadOnly) 
            //    LoadData(txtAccountNo.Text);
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                StorecardReverse = false;
                this.rbRefund.Enabled = true;
                ((MainForm)this.FormRoot).statusBar1.Text = "";
                txtAmount.Text = "";
                if (drpPayMethod.DataSource != null)
                    drpPayMethod.SelectedIndex = 0;
                txtCardNo.Text = "";
                if (drpReason.DataSource != null)
                    drpReason.SelectedIndex = 0;
                cbCardPrint.Checked = false;
                // Default the payment card length to the branch setting
                if (drpCardPrintType.DataSource != null)
                    this.drpCardPrintType.SelectedIndex = AccountManager.GetPaymentCardType((short)Convert.ToInt32(Config.BranchCode), StaticDataManager.GetServerDate(), out Error);

                atCustomerAddress.txtAddress1.Text = "";
                atCustomerAddress.cmbVillage.Text = "";
                atCustomerAddress.cmbRegion.Text = "";
                atCustomerAddress.txtPostCode.Text = "";
                txtCustomerName.Text = "";
                dgPayments.DataSource = null;
                txtOutstandingBalance.Text = "";
                txtArrears.Text = "";
                txtAgreementTotal.Text = "";
                txtAccountNo.Text = "000-0000-0000-0";
                txtAccountNo.Focus();
                gbTransaction.Enabled = false;
                lastAccountNo = "000000000000";
                // uat376 rdb BDW Reversal
                lblBDWReverse.Visible = false;
                btnBDWReverse.Visible = false;
                isMambuAccount = false;
                refundAmount = 0;
                FintransId = 0;
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

        private void LoadStaticData()
        {
            try
            {
                Wait();

                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                if (StaticData.Tables[TN.PayMethod] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.PayMethod, new string[] { "FPM", "L" }));
                if (StaticData.Tables[TN.RefundReasons] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.RefundReasons, new string[] { "RF1", "RF2", "L" }));
                if (StaticData.Tables[TN.CorrectionReasons] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CorrectionReasons, new string[] { "FT1", "FT2", "L" }));

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

                drpPayMethod.DataSource = (DataTable)StaticData.Tables[TN.PayMethod];
                drpPayMethod.DisplayMember = CN.CodeDescription;

                drpReason.DataSource = (DataTable)StaticData.Tables[TN.CorrectionReasons];
                drpReason.DisplayMember = CN.CodeDescription;

                drpCardPrintType.DataSource = (DataTable)StaticData.Tables[TN.CardPrintType];
                drpCardPrintType.DisplayMember = CN.CodeDescription;

                dtCLMambuPayMethod = PaymentManager.GetMambuCLPaymentMethods(string.Empty,out Error);
                dtHPMambuPayMethod = PaymentManager.GetMambuHPPaymentMethods(out Error);

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

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }

        private void RefundAndCorrection_Load(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                LoadStaticData();
                ttPayment.SetToolTip(btnUpdateCard, GetResource("TT_PRINTCARD"));
                ttPayment.SetToolTip(this.btnAccountDetails, GetResource("TT_ACCOUNTDETAILS"));
                ttPayment.SetToolTip(dgPayments, GetResource("TT_ROWCORRECTED"));

                // Default the payment card length to the branch setting
                if (drpCardPrintType.DataSource != null)
                    this.drpCardPrintType.SelectedIndex = AccountManager.GetPaymentCardType((short)Convert.ToInt32(Config.BranchCode), StaticDataManager.GetServerDate(), out Error);

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

        private void rbCorrection_CheckedChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                this.SetRefundOrCorrection();
            }
            catch (Exception ex)
            {
                Catch(ex, "rbCorrection_CheckedChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void txtAmount_Leave(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                if (!IsStrictNumeric(StripCurrency(txtAmount.Text)))
                {
                    errorProvider1.SetError(txtAmount, GetResource("M_NONNUMERIC"));
                    txtAmount.Select(0, txtAmount.Text.Length);
                    txtAmount.Focus();
                }
                else
                {
                    errorProvider1.SetError(txtAmount, "");
                    if (txtAmount.Text.Length > 0)
                        txtAmount.Text = Convert.ToDecimal(StripCurrency(txtAmount.Text)).ToString(DecimalPlaces);
                    else
                        txtAmount.Text = (0).ToString(DecimalPlaces);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "txtAmount_Leave");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnEnter_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                bool status = false;

                /* make sure they've entered an amount */
                txtAmount_Leave(null, null);

                if (Convert.ToDecimal(StripCurrency(txtAmount.Text)) != 0)
                {
                    if (rbCorrection.Checked && !StorecardReverse)
                        status = ProcessCorrection();
                    else
                        status = ProcessRefund();

                    if (status)
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
                decimal newBalance = Convert.ToDecimal(StripCurrency(txtOutstandingBalance.Text)) +
                    Convert.ToDecimal(StripCurrency(txtAmount.Text));
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
                        Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code]));
                }
            }
        }

        private bool ProcessCorrection()
        {
            bool status = false;

            int index = dgPayments.CurrentRowIndex;
            AuthorisePrompt auth = new AuthorisePrompt(this, authCorrection, GetResource("M_CORRECTIONAUTH"));

            if (index < 0)
                ShowInfo("M_NOPAYMENTSELECTED");
            else
                status = this.SetCanBeCorrected();

            if (status)
            {
                DataView dv = (DataView)dgPayments.DataSource;
                DateTime dateTrans = (DateTime)dv[index][CN.DateTrans];

                /* is the payment we're correcting more than one day old */
                /* date check is done in SetCanBeCorrected - JJ */
                //if(((TimeSpan)(DateTime.Now - dateTrans)).Days >= 0 &&
                if ((bool)Country[CountryParameterNames.SecureRefunds])
                    auth.ShowDialog();
                else
                    auth.Authorised = true;

                if (auth.Authorised)
                {

                    if (dv[index][CN.TransTypeCode].ToString() == TransType.DebtPayment)
                    {
                        // BDW (DPY) payments can only be corrected
                        // The account written off will be reversed and unsettled
                        PaymentManager.SaveBDWCorrection(
                            txtAccountNo.Text.Replace("-", ""),
                            Convert.ToInt16(Config.BranchCode),
                            Convert.ToDecimal(StripCurrency(txtAmount.Text)),
                            Config.CountryCode,
                            (DateTime)dv[index][CN.DateTrans],
                            (short)dv[index][CN.BranchNo],
                            (int)dv[index][CN.TransRefNo],
                            Credential.UserId,
                            out Error);
                    }
                    else
                    {
                        /* if the payment we're correcting is a cheque we must use the 
                         * cheque no for the correction so that the cheque can not
                         * subsequently be reversed in the cheque return screen which 
                         * would effectively result in a double correction */

                        PaymentManager.SaveCorrection(txtAccountNo.Text.Replace("-", ""),
                            Convert.ToInt16(Config.BranchCode),
                            Convert.ToDecimal(StripCurrency(txtAmount.Text)),
                            TransType.Correction, (string)dv[index][CN.BankCode],
                            (string)dv[index][CN.BankAccountNo2],
                            (string)dv[index][CN.ChequeNo],
                            Convert.ToInt16((string)dv[index][CN.Code]),
                            Config.CountryCode, DateTime.Now,
                            (string)((DataRowView)drpReason.SelectedItem)[CN.Code],
                            (int)dv[index][CN.TransRefNo],
                            Credential.UserId, FintransId, out Error);
                    }

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                        status = true;
                }
                else
                    status = false;
            }
            authCorrection.Enabled = authCorrection.Visible = false;

            return status;
        }

        private bool ProcessRefund()
        {
            bool status = false;
            // A refund is allowed on a DPY transaction, but must be linked to the customer account
            DateTime linkedDateTrans = Date.blankDate;
            short linkedBranchNo = 0;
            int linkedRefNo = 0;
            int index = dgPayments.CurrentRowIndex;
            DataView dv = (DataView)dgPayments.DataSource;

            GetCashierDeposits();

            var payMethod = Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code]);
            var amount = Convert.ToDecimal(StripCurrency(txtAmount.Text));

            decimal forDeposit = 0;
            Deposits.RowFilter = CN.Code + " = '" + payMethod + "'";
            foreach (DataRowView r in Deposits)
                forDeposit += (decimal)r[CN.ForDeposit];

            if (amount > forDeposit && payMethod == PayMethod.Cash)
            {
                ShowInfo("M_REFUNDEXCEEDSDEPOSIT", new Object[] { amount.ToString(DecimalPlaces), forDeposit.ToString(DecimalPlaces) });
                amount = 0;

                return false;
            }
            #region refund of excess amount for cashloan amortized account
            //Check Below condition
            //1. if acct is cash loan amortized
            //2. if account has no outstanding Bal
            //3. if account has excess of payment i.e. negative arrers
            //4. if the amount paid is greater than arrers
            if (PaymentManager.IsCashLoanAmortizedAccount(txtAccountNo.Text.Replace("-", "")) && Convert.ToDecimal(StripCurrency(txtOutstandingBalance.Text)) <= 0 && Convert.ToDecimal(StripCurrency(txtArrears.Text)) < 0 && amount > -1 * Convert.ToDecimal(StripCurrency(txtArrears.Text)))
            {
                ShowInfo("M_REFUNDEXCEEDSARREARS", new Object[] { amount.ToString(DecimalPlaces), Convert.ToDecimal(StripCurrency(txtArrears.Text)) });
                amount = 0;

                return false;

            }

            #endregion

            if (BDW.IsBDW(this.CustomerID))
            {
                //
                // BDW accounts
                //
                // Check that a DPY transaction has been selected for the DPY refund

                if (index >= 0)
                {
                    if (dv[index][CN.TransTypeCode].ToString() == TransType.DebtPayment)
                    {
                        // DPY transaction selecte
                        linkedDateTrans = (DateTime)dv[index][CN.DateTrans];
                        linkedBranchNo = (short)dv[index][CN.BranchNo];
                        linkedRefNo = (int)dv[index][CN.TransRefNo];
                        status = true;
                    }
                    else
                    {
                        // Warn this is not a DPY so cannot link refund to a customer account
                        if (ShowInfo("M_NODPYNOLINK", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            status = true;
                    }
                }
                else
                    // No transaction selected
                    ShowInfo("M_NODPYPAYMENTSELECTED");
            }
            else
            {
                //
                // Normal accounts
                //
                /* make sure that the sum of all the relevant transactions on 
                 * the account is > 0 and that the amount to be refunded does
                 * not exceed this total */
                decimal amountPaid = PaymentManager.GetAmountPaid(txtAccountNo.Text.Replace("-", ""), out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    amountPaid += PaymentManager.GetWarrantyAdjustment(txtAccountNo.Text.Replace("-", ""), out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        if (Convert.ToDecimal(StripCurrency(txtAmount.Text)) <= -amountPaid)
                            status = true;
                        else
                            ShowInfo("M_REFUNDTOOHIGH", new object[] { (-amountPaid).ToString(DecimalPlaces) });
                    }
                }
            }
            string transType = TransType.Refund; string CardNumber = "";
            if (index >= 0)
            {
                if (dv[index][CN.TransTypeCode].ToString() == TransType.StoreCardPayment)
                {
                    transType = TransType.StoreCardRefund;
                    CardNumber = dv[index][CN.CardNumber].ToString();
                    txtCardNo.Text = dv[index][CN.ChequeNo].ToString();
                    linkedRefNo = (int)dv[index][CN.TransRefNo];

                }
            }



            if (status)
            {
                status = false;
                /* every refund requires authorisation */
                AuthorisePrompt auth = new AuthorisePrompt(this, authRefund, GetResource("M_REFUNDAUTH"));
                if ((bool)Country[CountryParameterNames.SecureRefunds])
                    auth.ShowDialog();
                else
                    auth.Authorised = true;

                if (auth.Authorised)
                {
                    PaymentManager.SaveRefund(txtAccountNo.Text.Replace("-", ""), Convert.ToInt16(Config.BranchCode),
                        Convert.ToDecimal(StripCurrency(txtAmount.Text)),
                        transType, "", CardNumber, txtCardNo.Text,
                        Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code]),
                        Config.CountryCode, DateTime.Now,
                        linkedDateTrans, linkedBranchNo, linkedRefNo,
                        (string)((DataRowView)drpReason.SelectedItem)[CN.Code],
                        Credential.UserId, FintransId, out Error);

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                        status = true;
                }
                authRefund.Enabled = authRefund.Visible = false;
            }

            return status;
        }

        private void btnAccountDetails_Click(object sender, System.EventArgs e)
        {
            if (txtAccountNo.Text != "000-0000-0000-0")
            {
                AccountDetails details = new AccountDetails(txtAccountNo.Text.Replace("-", ""), FormRoot, this);
                ((MainForm)this.FormRoot).AddTabPage(details, 7);
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
        }

        private void btnUpdateCard_Click(object sender, System.EventArgs e)
        {
            decimal newBalance = 0;
            try
            {
                Wait();

                DataSet acctDetails = AccountManager.GetAccountDetails(txtAccountNo.UnformattedText, out Error);
                if (Error.Length > 0)
                {
                    ShowError(Error);
                }
                else
                {
                    foreach (DataTable dt in acctDetails.Tables)
                        if (dt.TableName == TN.AccountDetails)
                            foreach (DataRow row in dt.Rows)
                            {
                                newBalance = (decimal)row[CN.OutstandingBalance2];
                            }
                }

                if (txtAccountNo.Text != "000-0000-0000-0" &&
                    this.SlipPrinterOK(true))
                {
                    ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_PRINTCARDUPDATE");

                    // Print Payment Card
                    PrintPaymentCardTransactions(LoadTransactions(),
                                                Convert.ToInt16(this.txtCardRowNo.Value),
                                    newBalance, CustomerID, txtAccountNo.Text, 0, false);

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
            transactions = AccountManager.GetTransactions(txtAccountNo.Text.Replace("-", ""), out Error);
            if (Error.Length > 0)
                ShowError(Error);

            return transactions.Tables[TN.Transactions];
        }

        private void dgPayments_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Wait();

                if (dgPayments != null && dgPayments.CurrentRowIndex >= 0)
                {
                    refundAmount = ((decimal)((DataView)dgPayments.DataSource)[dgPayments.CurrentRowIndex][CN.TransValue]);
                    string pMethod = ((string)((DataView)dgPayments.DataSource)[dgPayments.CurrentRowIndex][CN.Code]);
                    FintransId = ((int)((DataView)dgPayments.DataSource)[dgPayments.CurrentRowIndex][CN.ID]);
                    paymentMethod = Convert.ToInt16(pMethod);
                    SetMambuPaymentMethod();
                    if (this.SetCanBeCorrected())
                    {
                        txtAmount.Text = ((decimal)((DataView)dgPayments.DataSource)[dgPayments.CurrentRowIndex][CN.TransValue]).ToString(DecimalPlaces);
                    }
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

        // uat376 rdb BDW Reversal
        private void btnBDWReverse_Click(object sender, EventArgs e)
        {
            ReverseBDW();

            //IP - 28/08/09 - UAT(737) - If the Refunds & Corrections screen has been opened
            //from the Telephone Actions screen, once the 'Reverse BDW' has been processed close the screen,
            //and save the RBDW Action against the account.
            if (FormParent is TelephoneAction5_2)
            {
                //Set the public property on the Telephone Actions screen.
                (FormParent as TelephoneAction5_2).ReversedBDW = true;

                CloseTab();

                (FormParent as TelephoneAction5_2).SaveRbdwAction();

            }
            else
            {
                ((MainForm)this.FormRoot).statusBar1.Text = "The BDW on the account has been reversed";
            }
        }

        private void ReverseBDW()
        {
            string acctno = txtAccountNo.Text.Replace("-", "");


            //AuthorisePrompt auth = new AuthorisePrompt(this, authCorrection, GetResource("M_CORRECTIONAUTH"));
            ///* is the payment we're correcting more than one day old */
            ///* date check is done in SetCanBeCorrected - JJ */
            ////if(((TimeSpan)(DateTime.Now - dateTrans)).Days >= 0 &&
            //if ((bool)Country[CountryParameterNames.SecureRefunds])
            //    auth.ShowDialog();
            //else
            //    auth.Authorised = true;

            PaymentManager.ReverseBDW(acctno, Config.CountryCode);
        }

        //IP - 14/04/10 - UAT(57) UAT5.2 - Method to check if the entered account
        //is a Home Club Account (HCC Account). If it is, do not load.
        private bool IsHomeClubAcct()
        {
            bool status = true;

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
                        status = false;
                    }
                }
            }
            return status;
        }

        private void GetCashierDeposits()
        {
            short branchno = Convert.ToInt16(Config.BranchCode);
            DataSet ds = PaymentManager.GetCashierOutstandingIncomeByPayMethod(Credential.UserId, branchno, out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
                Deposits = ds.Tables[TN.CashierOutstandingIncome].DefaultView;
        }

        private void rbRefund_CheckedChanged(object sender, EventArgs e)
        {
            SetMambuPaymentMethod();
        }
    }
}
