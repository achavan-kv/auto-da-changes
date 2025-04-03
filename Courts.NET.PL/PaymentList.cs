using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common.Static;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using Crownwood.Magic.Menus;
using STL.Common.Constants.EmployeeTypes;
using STL.Common.Constants.FTransaction;
using STL.Common;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.Categories;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Services.StoreCard;
using Blue.Cosacs.Client;
using Microsoft.PointOfService;

namespace STL.PL
{
    /// <summary>
    /// Popup prompt used by the Payment screen to list the payments across
    /// all Ready Finance (RF) accounts for the same customer. The payment
    /// screen will automatically spread the total payment across the set of
    /// RF accounts in proportion to the relative size of the normal
    /// instalments on these accounts. This popup prompt allows the user to
    /// adjust the individual payment amounts. Any account that is allocated
    /// and in arrears will have the calculated fee amount displayed. When the
    /// payment amount is changed, then the fee is automatically re-calculated
    /// for that account. The user may also change the individual fee amounts.
    /// The total payment and fee amounts are returned to the Payment screen.
    /// </summary>
    public partial class PaymentList : CommonForm
    {
        // The values displayed and returned by this form must be rounded
        // to Country.DecimalPlaces. If they are not the calling form might
        // recalculate and overwrite all of the user entered values.
        // 
        private new string Error = "";
        private int _precision = 2;
        private bool validateColumn = true;
        private bool isPageLoaded = false;
        private bool paymentAmountChanged = false;
        private System.Windows.Forms.DataGrid dgPaymentList;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label lCustomerName;
        private System.Windows.Forms.TextBox txtCustId;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Label lCustomerId;
        private System.Windows.Forms.Button btnOK;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Label lAuthorise;

        private DataView itemsStoreCardView;
        private DataRow _curReceipt = null;
        private string _lastReceiptNo = "";
        string error = "";
        string currAccountType = "";
        public int VoucherAuthorisedBy = 0;
        public string VoucherCompanyAcctNo = "";
        public bool CourtsVoucher = true;
        public string VoucherReference = "";
        private DataTable dtCLMambuPayMethod = null;
        private DataTable dtHPMambuPayMethod = null;
        private DataTable dtPayMethod = null;

        public DataSet PaymentDataSet = null;

        public decimal TotalPayment
        {
            get
            {
                decimal totalPayment = 0;

                if (PaymentDataSet != null && PaymentDataSet.Tables.Count > 0)
                {
                    foreach (DataRow paymentRow in PaymentDataSet.Tables[TN.Accounts].Rows)
                    {
                        if (paymentRow[CN.Payment] != null && paymentRow[CN.Payment] != DBNull.Value)
                        {
                            totalPayment = totalPayment + (decimal)paymentRow[CN.Payment];
                        }
                    }
                }

                return totalPayment;
            }
        }

        public decimal TotalFee
        {
            get
            {
                decimal totalFee = 0;

                if (PaymentDataSet != null && PaymentDataSet.Tables.Count > 0)
                {
                    foreach (DataRow paymentRow in PaymentDataSet.Tables[TN.Accounts].Rows)
                    {
                        if (paymentRow[CN.CollectionFee] != null && paymentRow[CN.CollectionFee] != DBNull.Value)
                        {
                            totalFee = totalFee + (decimal)paymentRow[CN.CollectionFee];
                        }
                    }
                }

                return totalFee;
            }
        }

        public string CustomerId
        {
            set
            {
                txtCustId.Text = value;
            }
        }

        public string CustomerName
        {
            set
            {
                txtCustomerName.Text = value;
            }
        }

        public decimal TenderedAmount
        {
            get
            {
                return MoneyStrToDecimal(txtTendered.Text);
            }
            set
            {
                txtTendered.Text = value.ToString(DecimalPlaces);
            }
        }

        public decimal TotalAmount
        {
            get
            {
                return MoneyStrToDecimal(txtTotalAmount.Text);
            }
            set
            {
                txtTotalAmount.Text = value.ToString(DecimalPlaces);
            }
        }

        public decimal CashChangeAmount
        {
            get
            {
                return MoneyStrToDecimal(txtCashChange.Text);
            }
            set
            {
                txtCashChange.Text = value.ToString(DecimalPlaces);
            }
        }

        public decimal ChequeChangeAmount
        {
            get
            {
                return MoneyStrToDecimal(txtChequeChange.Text);
            }
            set
            {
                txtChequeChange.Text = value.ToString(DecimalPlaces);
            }
        }

        public decimal CashTender
        {
            get
            {
                return MoneyStrToDecimal(txtCashAmt.Text);
            }
            set
            {
                txtCashAmt.Text = value.ToString(DecimalPlaces);
            }
        }

        public decimal ChequeTender
        {
            get
            {
                return MoneyStrToDecimal(txtChequeAmt.Text);
            }
            set
            {
                txtChequeAmt.Text = value.ToString(DecimalPlaces);
            }
        }

        public bool IsOkClicked { get; set; }

        public bool PrintMiniStat
        {
            get
            {
                return cbMiniStat.Checked;
            }
            set
            {
                cbMiniStat.Checked = value;
            }
        }

        public decimal GiftVoucherValue
        {
            get;
            set;
        }

        public delegate void InvokeDelegate();

        private Button btnExchange;
        private MaskedTextBox mtb_CardNo;
        private ComboBox drpCardType;
        private Label lCardType;
        private ComboBox drpBank;
        private ComboBox drpPayMethod;
        private Label lBankAcctNo;
        private TextBox txtBankAcctNo;
        private Label lBank;
        private Label lCardNo;
        private Label lPayMethod;
        private Label lReceiptNo;
        private TextBox txtReceiptNo;
        private Label lTendered;
        private TextBox txtTendered;
        private Label lChange;
        private TextBox txtCashChange;
        private Label lTotalAmount;
        private TextBox txtTotalAmount;
        private TextBox txtCardNo;
        private Button btnAddToGrid;
        private Button btnStoreCardManualEntry;
        public StoreCardMagStripeReader MagStripeReader;
        private Button btnPrintAcctNo;
        private ErrorProvider errorProvider1;
        private ErrorProvider errorProviderStoreCard;
        public TextBox txtSelectedAcctNo;
        private Label lAcctNo;
        private Label lPaymentAmount;
        private TextBox txtPayAmount;
        private CheckBox chkDeposit;
        private CheckBox cbMiniStat;
        private Label lblMiniStat;
        private Button btn_calc;
        private ToolTip ttPayment;
        private Label label2;
        private TextBox txtCashAmt;
        private Label label1;
        private TextBox txtChequeAmt;
        private Label label5;
        private TextBox txtChequeChange;
        private Label label4;
        private TextBox txtLocalChange;
        private Label label3;
        private TextBox txtLocalTender;
        Timer displayTotalCalculationTimer = new Timer();

        public PaymentList(Form root, Form parent)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;

            if (IsNumeric(DecimalPlaces.Substring(1, 1)))
            {
                this._precision = System.Convert.ToInt32(DecimalPlaces.Substring(1, 1));
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PaymentList));
            this.dgPaymentList = new System.Windows.Forms.DataGrid();
            this.btnOK = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.lCustomerName = new System.Windows.Forms.Label();
            this.txtCustId = new System.Windows.Forms.TextBox();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.lCustomerId = new System.Windows.Forms.Label();
            this.lAuthorise = new System.Windows.Forms.Label();
            this.btnExchange = new System.Windows.Forms.Button();
            this.mtb_CardNo = new System.Windows.Forms.MaskedTextBox();
            this.drpCardType = new System.Windows.Forms.ComboBox();
            this.lCardType = new System.Windows.Forms.Label();
            this.drpBank = new System.Windows.Forms.ComboBox();
            this.drpPayMethod = new System.Windows.Forms.ComboBox();
            this.lBankAcctNo = new System.Windows.Forms.Label();
            this.txtBankAcctNo = new System.Windows.Forms.TextBox();
            this.lBank = new System.Windows.Forms.Label();
            this.lCardNo = new System.Windows.Forms.Label();
            this.lPayMethod = new System.Windows.Forms.Label();
            this.lReceiptNo = new System.Windows.Forms.Label();
            this.txtReceiptNo = new System.Windows.Forms.TextBox();
            this.lTendered = new System.Windows.Forms.Label();
            this.txtTendered = new System.Windows.Forms.TextBox();
            this.lChange = new System.Windows.Forms.Label();
            this.txtCashChange = new System.Windows.Forms.TextBox();
            this.lTotalAmount = new System.Windows.Forms.Label();
            this.txtTotalAmount = new System.Windows.Forms.TextBox();
            this.txtCardNo = new System.Windows.Forms.TextBox();
            this.btnAddToGrid = new System.Windows.Forms.Button();
            this.btnStoreCardManualEntry = new System.Windows.Forms.Button();
            this.btnPrintAcctNo = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProviderStoreCard = new System.Windows.Forms.ErrorProvider(this.components);
            this.txtSelectedAcctNo = new System.Windows.Forms.TextBox();
            this.lAcctNo = new System.Windows.Forms.Label();
            this.lPaymentAmount = new System.Windows.Forms.Label();
            this.txtPayAmount = new System.Windows.Forms.TextBox();
            this.chkDeposit = new System.Windows.Forms.CheckBox();
            this.cbMiniStat = new System.Windows.Forms.CheckBox();
            this.lblMiniStat = new System.Windows.Forms.Label();
            this.btn_calc = new System.Windows.Forms.Button();
            this.ttPayment = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.txtChequeAmt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCashAmt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLocalTender = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLocalChange = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtChequeChange = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgPaymentList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderStoreCard)).BeginInit();
            this.SuspendLayout();
            // 
            // dgPaymentList
            // 
            this.dgPaymentList.CaptionText = "Payments List";
            this.dgPaymentList.DataMember = "";
            this.dgPaymentList.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgPaymentList.Location = new System.Drawing.Point(17, 287);
            this.dgPaymentList.Name = "dgPaymentList";
            this.dgPaymentList.Size = new System.Drawing.Size(1019, 253);
            this.dgPaymentList.TabIndex = 0;
            this.dgPaymentList.TabStop = false;
            this.dgPaymentList.Enter += new System.EventHandler(this.dgPaymentList_Enter);
            this.dgPaymentList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgPaymentList_MouseUp);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 543);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(108, 543);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(90, 27);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            // 
            // lCustomerName
            // 
            this.lCustomerName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lCustomerName.Location = new System.Drawing.Point(233, 9);
            this.lCustomerName.Name = "lCustomerName";
            this.lCustomerName.Size = new System.Drawing.Size(105, 19);
            this.lCustomerName.TabIndex = 10;
            this.lCustomerName.Text = "Customer Name";
            this.lCustomerName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCustId
            // 
            this.txtCustId.BackColor = System.Drawing.SystemColors.Window;
            this.txtCustId.Location = new System.Drawing.Point(86, 9);
            this.txtCustId.MaxLength = 20;
            this.txtCustId.Name = "txtCustId";
            this.txtCustId.ReadOnly = true;
            this.txtCustId.Size = new System.Drawing.Size(144, 22);
            this.txtCustId.TabIndex = 9;
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.SystemColors.Window;
            this.txtCustomerName.Location = new System.Drawing.Point(340, 9);
            this.txtCustomerName.MaxLength = 80;
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(374, 22);
            this.txtCustomerName.TabIndex = 7;
            this.txtCustomerName.TabStop = false;
            // 
            // lCustomerId
            // 
            this.lCustomerId.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lCustomerId.Location = new System.Drawing.Point(0, 9);
            this.lCustomerId.Name = "lCustomerId";
            this.lCustomerId.Size = new System.Drawing.Size(86, 19);
            this.lCustomerId.TabIndex = 8;
            this.lCustomerId.Text = "Customer ID";
            this.lCustomerId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lAuthorise
            // 
            this.lAuthorise.Enabled = false;
            this.lAuthorise.Location = new System.Drawing.Point(845, 9);
            this.lAuthorise.Name = "lAuthorise";
            this.lAuthorise.Size = new System.Drawing.Size(19, 19);
            this.lAuthorise.TabIndex = 15;
            // 
            // btnExchange
            // 
            this.btnExchange.Image = ((System.Drawing.Image)(resources.GetObject("btnExchange.Image")));
            this.btnExchange.Location = new System.Drawing.Point(962, 62);
            this.btnExchange.Name = "btnExchange";
            this.btnExchange.Size = new System.Drawing.Size(24, 28);
            this.btnExchange.TabIndex = 82;
            this.btnExchange.Visible = false;
            this.btnExchange.Click += new System.EventHandler(this.btnExchange_Click);
            // 
            // mtb_CardNo
            // 
            this.mtb_CardNo.Location = new System.Drawing.Point(810, 177);
            this.mtb_CardNo.Mask = "XXXX-XXXX-XXXX-0000";
            this.mtb_CardNo.Name = "mtb_CardNo";
            this.mtb_CardNo.Size = new System.Drawing.Size(181, 22);
            this.mtb_CardNo.TabIndex = 91;
            this.mtb_CardNo.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.mtb_CardNo.Visible = false;
            this.mtb_CardNo.TextChanged += new System.EventHandler(this.mtb_Cardno_TextChanged);
            // 
            // drpCardType
            // 
            this.drpCardType.BackColor = System.Drawing.SystemColors.Window;
            this.drpCardType.Enabled = false;
            this.drpCardType.Items.AddRange(new object[] {
            ""});
            this.drpCardType.Location = new System.Drawing.Point(810, 150);
            this.drpCardType.Name = "drpCardType";
            this.drpCardType.Size = new System.Drawing.Size(125, 24);
            this.drpCardType.TabIndex = 90;
            // 
            // lCardType
            // 
            this.lCardType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lCardType.Location = new System.Drawing.Point(726, 152);
            this.lCardType.Name = "lCardType";
            this.lCardType.Size = new System.Drawing.Size(77, 19);
            this.lCardType.TabIndex = 71;
            this.lCardType.Text = "Card Type";
            this.lCardType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpBank
            // 
            this.drpBank.BackColor = System.Drawing.SystemColors.Window;
            this.drpBank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBank.Enabled = false;
            this.drpBank.Location = new System.Drawing.Point(810, 205);
            this.drpBank.Name = "drpBank";
            this.drpBank.Size = new System.Drawing.Size(221, 24);
            this.drpBank.TabIndex = 92;
            // 
            // drpPayMethod
            // 
            this.drpPayMethod.BackColor = System.Drawing.SystemColors.Window;
            this.drpPayMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPayMethod.Location = new System.Drawing.Point(809, 66);
            this.drpPayMethod.Name = "drpPayMethod";
            this.drpPayMethod.Size = new System.Drawing.Size(153, 24);
            this.drpPayMethod.TabIndex = 89;
            this.drpPayMethod.SelectedIndexChanged += new System.EventHandler(this.drpPayMethod_SelectedIndexChanged);
            // 
            // lBankAcctNo
            // 
            this.lBankAcctNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lBankAcctNo.Location = new System.Drawing.Point(688, 233);
            this.lBankAcctNo.Name = "lBankAcctNo";
            this.lBankAcctNo.Size = new System.Drawing.Size(115, 19);
            this.lBankAcctNo.TabIndex = 72;
            this.lBankAcctNo.Text = "Bank Account No";
            this.lBankAcctNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBankAcctNo
            // 
            this.txtBankAcctNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtBankAcctNo.Enabled = false;
            this.txtBankAcctNo.Location = new System.Drawing.Point(810, 233);
            this.txtBankAcctNo.MaxLength = 30;
            this.txtBankAcctNo.Name = "txtBankAcctNo";
            this.txtBankAcctNo.Size = new System.Drawing.Size(221, 22);
            this.txtBankAcctNo.TabIndex = 93;
            // 
            // lBank
            // 
            this.lBank.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lBank.Location = new System.Drawing.Point(755, 205);
            this.lBank.Name = "lBank";
            this.lBank.Size = new System.Drawing.Size(48, 19);
            this.lBank.TabIndex = 73;
            this.lBank.Text = "Bank";
            this.lBank.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lCardNo
            // 
            this.lCardNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lCardNo.Location = new System.Drawing.Point(688, 178);
            this.lCardNo.Name = "lCardNo";
            this.lCardNo.Size = new System.Drawing.Size(115, 18);
            this.lCardNo.TabIndex = 74;
            this.lCardNo.Text = "Cheque / Card No";
            this.lCardNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lPayMethod
            // 
            this.lPayMethod.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lPayMethod.Location = new System.Drawing.Point(706, 66);
            this.lPayMethod.Name = "lPayMethod";
            this.lPayMethod.Size = new System.Drawing.Size(96, 18);
            this.lPayMethod.TabIndex = 75;
            this.lPayMethod.Text = "Pay Method";
            this.lPayMethod.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lReceiptNo
            // 
            this.lReceiptNo.Location = new System.Drawing.Point(718, 12);
            this.lReceiptNo.Name = "lReceiptNo";
            this.lReceiptNo.Size = new System.Drawing.Size(86, 18);
            this.lReceiptNo.TabIndex = 76;
            this.lReceiptNo.Text = "Receipt No";
            this.lReceiptNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtReceiptNo
            // 
            this.txtReceiptNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtReceiptNo.Location = new System.Drawing.Point(806, 9);
            this.txtReceiptNo.MaxLength = 10;
            this.txtReceiptNo.Name = "txtReceiptNo";
            this.txtReceiptNo.Size = new System.Drawing.Size(116, 22);
            this.txtReceiptNo.TabIndex = 77;
            this.txtReceiptNo.Leave += new System.EventHandler(this.ReceiptNo_Leave);
            // 
            // lTendered
            // 
            this.lTendered.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lTendered.Location = new System.Drawing.Point(37, 190);
            this.lTendered.Name = "lTendered";
            this.lTendered.Size = new System.Drawing.Size(112, 27);
            this.lTendered.TabIndex = 84;
            this.lTendered.Text = "Total Tendered";
            this.lTendered.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTendered
            // 
            this.txtTendered.BackColor = System.Drawing.SystemColors.Window;
            this.txtTendered.Location = new System.Drawing.Point(155, 192);
            this.txtTendered.MaxLength = 10;
            this.txtTendered.Name = "txtTendered";
            this.txtTendered.ReadOnly = true;
            this.txtTendered.Size = new System.Drawing.Size(138, 22);
            this.txtTendered.TabIndex = 85;
            this.txtTendered.Leave += new System.EventHandler(this.txtTendered_Leave);
            // 
            // lChange
            // 
            this.lChange.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lChange.Location = new System.Drawing.Point(812, 548);
            this.lChange.Name = "lChange";
            this.lChange.Size = new System.Drawing.Size(100, 24);
            this.lChange.TabIndex = 86;
            this.lChange.Text = "Cash Change";
            this.lChange.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCashChange
            // 
            this.txtCashChange.BackColor = System.Drawing.SystemColors.Window;
            this.txtCashChange.Location = new System.Drawing.Point(918, 548);
            this.txtCashChange.MaxLength = 10;
            this.txtCashChange.Name = "txtCashChange";
            this.txtCashChange.ReadOnly = true;
            this.txtCashChange.Size = new System.Drawing.Size(115, 22);
            this.txtCashChange.TabIndex = 89;
            // 
            // lTotalAmount
            // 
            this.lTotalAmount.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lTotalAmount.Location = new System.Drawing.Point(371, 549);
            this.lTotalAmount.Name = "lTotalAmount";
            this.lTotalAmount.Size = new System.Drawing.Size(86, 19);
            this.lTotalAmount.TabIndex = 87;
            this.lTotalAmount.Text = "Total Amount";
            this.lTotalAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTotalAmount
            // 
            this.txtTotalAmount.BackColor = System.Drawing.SystemColors.Window;
            this.txtTotalAmount.Location = new System.Drawing.Point(464, 548);
            this.txtTotalAmount.MaxLength = 10;
            this.txtTotalAmount.Name = "txtTotalAmount";
            this.txtTotalAmount.ReadOnly = true;
            this.txtTotalAmount.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTotalAmount.Size = new System.Drawing.Size(116, 22);
            this.txtTotalAmount.TabIndex = 100;
            // 
            // txtCardNo
            // 
            this.txtCardNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtCardNo.Location = new System.Drawing.Point(810, 177);
            this.txtCardNo.MaxLength = 30;
            this.txtCardNo.Name = "txtCardNo";
            this.txtCardNo.Size = new System.Drawing.Size(182, 22);
            this.txtCardNo.TabIndex = 90;
            this.txtCardNo.Visible = false;
            // 
            // btnAddToGrid
            // 
            this.btnAddToGrid.Location = new System.Drawing.Point(811, 258);
            this.btnAddToGrid.Name = "btnAddToGrid";
            this.btnAddToGrid.Size = new System.Drawing.Size(108, 25);
            this.btnAddToGrid.TabIndex = 92;
            this.btnAddToGrid.Text = "Add to Grid";
            this.btnAddToGrid.Click += new System.EventHandler(this.btnAddToGrid_Click);
            // 
            // btnStoreCardManualEntry
            // 
            this.btnStoreCardManualEntry.Enabled = false;
            this.btnStoreCardManualEntry.Image = ((System.Drawing.Image)(resources.GetObject("btnStoreCardManualEntry.Image")));
            this.btnStoreCardManualEntry.Location = new System.Drawing.Point(1004, 177);
            this.btnStoreCardManualEntry.Name = "btnStoreCardManualEntry";
            this.btnStoreCardManualEntry.Size = new System.Drawing.Size(27, 25);
            this.btnStoreCardManualEntry.TabIndex = 92;
            this.ttPayment.SetToolTip(this.btnStoreCardManualEntry, "Enter Card Number from keyboard");
            this.btnStoreCardManualEntry.Visible = false;
            this.btnStoreCardManualEntry.Click += new System.EventHandler(this.btnStoreCardManualEntry_Click);
            // 
            // btnPrintAcctNo
            // 
            this.btnPrintAcctNo.Enabled = false;
            this.btnPrintAcctNo.Location = new System.Drawing.Point(941, 256);
            this.btnPrintAcctNo.Name = "btnPrintAcctNo";
            this.btnPrintAcctNo.Size = new System.Drawing.Size(90, 27);
            this.btnPrintAcctNo.TabIndex = 93;
            this.btnPrintAcctNo.Text = "Print AcctNo";
            this.btnPrintAcctNo.Click += new System.EventHandler(this.btnPrintAcctNo_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // errorProviderStoreCard
            // 
            this.errorProviderStoreCard.ContainerControl = this;
            // 
            // txtSelectedAcctNo
            // 
            this.txtSelectedAcctNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtSelectedAcctNo.Location = new System.Drawing.Point(376, 39);
            this.txtSelectedAcctNo.MaxLength = 12;
            this.txtSelectedAcctNo.Name = "txtSelectedAcctNo";
            this.txtSelectedAcctNo.ReadOnly = true;
            this.txtSelectedAcctNo.Size = new System.Drawing.Size(115, 22);
            this.txtSelectedAcctNo.TabIndex = 94;
            this.txtSelectedAcctNo.TabStop = false;
            // 
            // lAcctNo
            // 
            this.lAcctNo.AccessibleRole = System.Windows.Forms.AccessibleRole.SplitButton;
            this.lAcctNo.Location = new System.Drawing.Point(240, 40);
            this.lAcctNo.Name = "lAcctNo";
            this.lAcctNo.Size = new System.Drawing.Size(150, 22);
            this.lAcctNo.TabIndex = 95;
            this.lAcctNo.Text = "Selected Account No";
            this.lAcctNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lPaymentAmount
            // 
            this.lPaymentAmount.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lPaymentAmount.Location = new System.Drawing.Point(668, 44);
            this.lPaymentAmount.Name = "lPaymentAmount";
            this.lPaymentAmount.Size = new System.Drawing.Size(135, 18);
            this.lPaymentAmount.TabIndex = 96;
            this.lPaymentAmount.Text = "Payment Amount";
            this.lPaymentAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPayAmount
            // 
            this.txtPayAmount.BackColor = System.Drawing.SystemColors.Window;
            this.txtPayAmount.Location = new System.Drawing.Point(810, 39);
            this.txtPayAmount.MaxLength = 10;
            this.txtPayAmount.Name = "txtPayAmount";
            this.txtPayAmount.Size = new System.Drawing.Size(115, 22);
            this.txtPayAmount.TabIndex = 88;
            this.txtPayAmount.Leave += new System.EventHandler(this.txtPayAmount_Leave);
            // 
            // chkDeposit
            // 
            this.chkDeposit.AutoSize = true;
            this.chkDeposit.Location = new System.Drawing.Point(932, 42);
            this.chkDeposit.Name = "chkDeposit";
            this.chkDeposit.Size = new System.Drawing.Size(89, 20);
            this.chkDeposit.TabIndex = 97;
            this.chkDeposit.Text = "Is Deposit";
            this.chkDeposit.UseVisualStyleBackColor = true;
            this.chkDeposit.CheckedChanged += new System.EventHandler(this.chkDeposit_CheckedChanged);
            // 
            // cbMiniStat
            // 
            this.cbMiniStat.AutoSize = true;
            this.cbMiniStat.Location = new System.Drawing.Point(785, 263);
            this.cbMiniStat.Name = "cbMiniStat";
            this.cbMiniStat.Size = new System.Drawing.Size(18, 17);
            this.cbMiniStat.TabIndex = 99;
            this.cbMiniStat.UseVisualStyleBackColor = true;
            // 
            // lblMiniStat
            // 
            this.lblMiniStat.AutoSize = true;
            this.lblMiniStat.Location = new System.Drawing.Point(688, 262);
            this.lblMiniStat.Name = "lblMiniStat";
            this.lblMiniStat.Size = new System.Drawing.Size(94, 16);
            this.lblMiniStat.TabIndex = 98;
            this.lblMiniStat.Text = "Mini Statement";
            // 
            // btn_calc
            // 
            this.btn_calc.Enabled = false;
            this.btn_calc.Image = global::STL.PL.Properties.Resources.Calc;
            this.btn_calc.Location = new System.Drawing.Point(986, 60);
            this.btn_calc.Name = "btn_calc";
            this.btn_calc.Size = new System.Drawing.Size(44, 35);
            this.btn_calc.TabIndex = 100;
            this.btn_calc.UseVisualStyleBackColor = true;
            this.btn_calc.Click += new System.EventHandler(this.btn_calc_Click);
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(17, 220);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 27);
            this.label1.TabIndex = 101;
            this.label1.Text = "Cheque Tendered";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtChequeAmt
            // 
            this.txtChequeAmt.BackColor = System.Drawing.SystemColors.Window;
            this.txtChequeAmt.Location = new System.Drawing.Point(155, 225);
            this.txtChequeAmt.MaxLength = 10;
            this.txtChequeAmt.Name = "txtChequeAmt";
            this.txtChequeAmt.Size = new System.Drawing.Size(138, 22);
            this.txtChequeAmt.TabIndex = 86;
            this.txtChequeAmt.Leave += new System.EventHandler(this.txtChequeAmt_Leave);
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(7, 251);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 27);
            this.label2.TabIndex = 103;
            this.label2.Text = "Cash Tendered";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCashAmt
            // 
            this.txtCashAmt.BackColor = System.Drawing.SystemColors.Window;
            this.txtCashAmt.Location = new System.Drawing.Point(155, 255);
            this.txtCashAmt.MaxLength = 10;
            this.txtCashAmt.Name = "txtCashAmt";
            this.txtCashAmt.Size = new System.Drawing.Size(138, 22);
            this.txtCashAmt.TabIndex = 87;
            this.txtCashAmt.Leave += new System.EventHandler(this.txtCashAmt_Leave);

            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(690, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 23);
            this.label3.TabIndex = 107;
            this.label3.Text = "Local Tender";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtLocalTender
            // 
            this.txtLocalTender.BackColor = System.Drawing.SystemColors.Window;
            this.txtLocalTender.Enabled = false;
            this.txtLocalTender.Location = new System.Drawing.Point(810, 93);
            this.txtLocalTender.MaxLength = 10;
            this.txtLocalTender.Name = "txtLocalTender";
            this.txtLocalTender.ReadOnly = true;
            this.txtLocalTender.Size = new System.Drawing.Size(115, 22);
            this.txtLocalTender.TabIndex = 106;
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(695, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 23);
            this.label4.TabIndex = 109;
            this.label4.Text = "Local Change";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtLocalChange
            // 
            this.txtLocalChange.BackColor = System.Drawing.SystemColors.Window;
            this.txtLocalChange.Enabled = false;
            this.txtLocalChange.Location = new System.Drawing.Point(811, 122);
            this.txtLocalChange.MaxLength = 10;
            this.txtLocalChange.Name = "txtLocalChange";
            this.txtLocalChange.ReadOnly = true;
            this.txtLocalChange.Size = new System.Drawing.Size(115, 22);
            this.txtLocalChange.TabIndex = 108;
            // 
            // label5
            // 
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(587, 547);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 22);
            this.label5.TabIndex = 110;
            this.label5.Text = "Cheque Change";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtChequeChange
            // 
            this.txtChequeChange.BackColor = System.Drawing.SystemColors.Window;
            this.txtChequeChange.Location = new System.Drawing.Point(708, 548);
            this.txtChequeChange.MaxLength = 10;
            this.txtChequeChange.Name = "txtChequeChange";
            this.txtChequeChange.ReadOnly = true;
            this.txtChequeChange.Size = new System.Drawing.Size(115, 22);
            this.txtChequeChange.TabIndex = 111;
            // 
            // PaymentList
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.ClientSize = new System.Drawing.Size(1041, 575);
            this.ControlBox = false;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtChequeChange);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtLocalChange);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtLocalTender);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtCashAmt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtChequeAmt);
            this.Controls.Add(this.btn_calc);
            this.Controls.Add(this.cbMiniStat);
            this.Controls.Add(this.lblMiniStat);
            this.Controls.Add(this.chkDeposit);
            this.Controls.Add(this.lPaymentAmount);
            this.Controls.Add(this.txtPayAmount);
            this.Controls.Add(this.txtSelectedAcctNo);
            this.Controls.Add(this.lAcctNo);
            this.Controls.Add(this.btnPrintAcctNo);
            this.Controls.Add(this.btnStoreCardManualEntry);
            this.Controls.Add(this.btnAddToGrid);
            this.Controls.Add(this.lChange);
            this.Controls.Add(this.txtCashChange);
            this.Controls.Add(this.lTotalAmount);
            this.Controls.Add(this.txtTotalAmount);
            this.Controls.Add(this.lTendered);
            this.Controls.Add(this.txtTendered);
            this.Controls.Add(this.btnExchange);
            this.Controls.Add(this.mtb_CardNo);
            this.Controls.Add(this.drpCardType);
            this.Controls.Add(this.lCardType);
            this.Controls.Add(this.drpBank);
            this.Controls.Add(this.drpPayMethod);
            this.Controls.Add(this.lBankAcctNo);
            this.Controls.Add(this.txtBankAcctNo);
            this.Controls.Add(this.lBank);
            this.Controls.Add(this.lCardNo);
            this.Controls.Add(this.lPayMethod);
            this.Controls.Add(this.lReceiptNo);
            this.Controls.Add(this.txtReceiptNo);
            this.Controls.Add(this.lAuthorise);
            this.Controls.Add(this.lCustomerName);
            this.Controls.Add(this.txtCustId);
            this.Controls.Add(this.txtCustomerName);
            this.Controls.Add(this.lCustomerId);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.dgPaymentList);
            this.Controls.Add(this.txtCardNo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PaymentList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CoSACS Payment";
            this.Load += new System.EventHandler(this.PaymentList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgPaymentList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderStoreCard)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void LoadData()
        {
            try
            {
                Function = "Payment List Screen: Load Data";
                Wait();

                DataView PaymentListView = null;
                string statusText = "";
                int lockCount = 0;

                foreach (DataTable PaymentDetails in PaymentDataSet.Tables)
                {
                    if (PaymentDetails.TableName == TN.Accounts)
                    {
                        // Add a validation event
                        //dgPaymentList.CurrentCellChanged += new EventHandler(dgPaymentList_CurCellChange);
                        PaymentDetails.ColumnChanging += new DataColumnChangeEventHandler(this.PaymentDetails_ColumnChanging);

                        statusText = PaymentDetails.Rows.Count + GetResource("M_ACCOUNTSLISTED");

                        // Create a view for the DataGrid
                        PaymentListView = new DataView(PaymentDetails);
                        PaymentListView.AllowNew = false;
                        PaymentListView.Sort = CN.acctno + " ASC ";
                        dgPaymentList.CausesValidation = false;
                        dgPaymentList.DataSource = PaymentListView;

                        if (dgPaymentList.TableStyles.Count == 0)
                        {
                            // Create the table style for the DataGrid
                            DataGridTableStyle tabStyle = new DataGridTableStyle();
                            tabStyle.MappingName = PaymentListView.Table.TableName;

                            // Add an icon column if any accounts are locked
                            foreach (DataRow accountRow in PaymentDetails.Rows)
                            {
                                // Check for accounts already locked
                                if (accountRow[CN.LockedBy].ToString().Length > 0)
                                    lockCount++;
                                if ((bool)accountRow[CN.IsMambuAccount] == true && (bool)accountRow[CN.AlreadyAdded] == false)
                                {
                                    accountRow[CN.SettlementFigure] = (decimal)accountRow[CN.SettlementFigure] + (decimal)accountRow[CN.CollectionFee] - (decimal)accountRow[CN.Rebate];
                                    accountRow[CN.AlreadyAdded] = true;
                                }
                            }

                            AccountTextColumn aColumnTextColumn;
                            int numCols = PaymentListView.Table.Columns.Count;
                            for (int i = 0; i < numCols; ++i)
                            {
                                aColumnTextColumn = new AccountTextColumn(i);
                                aColumnTextColumn.HeaderText = PaymentListView.Table.Columns[i].ColumnName;
                                aColumnTextColumn.MappingName = PaymentListView.Table.Columns[i].ColumnName;
                                tabStyle.GridColumnStyles.Add(aColumnTextColumn);
                            }

                            if (lockCount > 0)
                            {
                                if (!PaymentDetails.Columns.Contains("Icon"))
                                {
                                    // Add an icon column to mark locked accounts
                                    PaymentDetails.Columns.Add("Icon");
                                }

                                // Add an unbound stand-alone icon column
                                DataGridIconColumn iconColumn = new DataGridIconColumn(imageList1.Images[0], CN.LockedBy, "");
                                iconColumn.MappingName = "Icon";
                                iconColumn.HeaderText = "";
                                iconColumn.Width = imageList1.Images[0].Size.Width;
                                tabStyle.GridColumnStyles.Add(iconColumn);
                            }

                            //
                            // Style each column that needs to be displayed
                            //

                            // Normal columns
                            tabStyle.GridColumnStyles[CN.AuthorisedBy].Width = 0;
                            tabStyle.GridColumnStyles[CN.SundryCredit].Width = 0;
                            tabStyle.GridColumnStyles[CN.DateAcctOpen].Width = 0;
                            tabStyle.GridColumnStyles[CN.RatioPay].Width = 0;
                            tabStyle.GridColumnStyles[CN.LockedBy].Width = 0;
                            tabStyle.GridColumnStyles[CN.Rebate].Width = 0;
                            tabStyle.GridColumnStyles[CN.NetPayment].Width = 0;
                            tabStyle.GridColumnStyles[CN.CollectionFee].Width = 0;
                            tabStyle.GridColumnStyles[CN.BailiffFee].Width = 0;
                            tabStyle.GridColumnStyles[CN.CalculatedFee].Width = 0;
                            tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 0;
                            tabStyle.GridColumnStyles[CN.SegmentID].Width = 0;
                            tabStyle.GridColumnStyles[CN.ReadOnly].Width = 0;
                            tabStyle.GridColumnStyles[CN.Status].Width = 0;
                            tabStyle.GridColumnStyles[CN.DebitAccount].Width = 0;
                            tabStyle.GridColumnStyles[CN.BDWBalance].Width = 0;
                            tabStyle.GridColumnStyles[CN.BDWCharges].Width = 0;
                            tabStyle.GridColumnStyles[CN.IsMambuAccount].Width = 0;
                            tabStyle.GridColumnStyles["Icon"].Width = 0;
                            tabStyle.GridColumnStyles[CN.EmployeeName].Width = 0;
                            tabStyle.GridColumnStyles[CN.DateFirst].Width = 0;
                            tabStyle.GridColumnStyles[CN.DeliveredIndicator].Width = 0;
                            tabStyle.GridColumnStyles[CN.DeliveryFlag].Width = 0;
                            tabStyle.GridColumnStyles[CN.ToFollowAmount].Width = 0;
                            tabStyle.GridColumnStyles[CN.FreeInstalment].Width = 0;
                            tabStyle.GridColumnStyles[CN.Securitised].Width = 0;
                            tabStyle.GridColumnStyles[CN.PaymentHoliday].Width = 0;
                            tabStyle.GridColumnStyles[CN.AgrmtNo].Width = 0;
                            tabStyle.GridColumnStyles[CN.PaymentHolidayMin].Width = 0;
                            tabStyle.GridColumnStyles[CN.PaymentCardLine].Width = 0;
                            tabStyle.GridColumnStyles[CN.ServiceRequestNoStr].Width = 0;
                            tabStyle.GridColumnStyles[CN.Internal].Width = 0;
                            tabStyle.GridColumnStyles[CN.AccountType].Width = 0;
                            tabStyle.GridColumnStyles[CN.Arrears].Width = 0;
                            tabStyle.GridColumnStyles[CN.SegmentName].Width = 0;
                            tabStyle.GridColumnStyles[CN.TallymanAcct].Width = 0;
                            tabStyle.GridColumnStyles[CN.Permissionid].Width = 0;
                            tabStyle.GridColumnStyles[CN.Userid].Width = 0;
                            tabStyle.GridColumnStyles[CN.Permissionid1].Width = 0;
                            tabStyle.GridColumnStyles[CN.Userid1].Width = 0;
                            tabStyle.GridColumnStyles[CN.IsBailiff].Width = 0;
                            tabStyle.GridColumnStyles[CN.IsTelephoneCaller].Width = 0;
                            tabStyle.GridColumnStyles[CN.ReceiptNo].Width = 0;
                            tabStyle.GridColumnStyles[CN.Paymentmethod].Width = 0;
                            tabStyle.GridColumnStyles[CN.CardType].Width = 0;
                            tabStyle.GridColumnStyles[CN.CardNumber].Width = 0;
                            tabStyle.GridColumnStyles[CN.BankCode].Width = 0;
                            tabStyle.GridColumnStyles[CN.BankAccountNo].Width = 0;
                            tabStyle.GridColumnStyles[CN.TenderedAccountAmt].Width = 0;
                            tabStyle.GridColumnStyles[CN.LocalChange].Width = 0;
                            tabStyle.GridColumnStyles[CN.LocalTender].Width = 0;
                            tabStyle.GridColumnStyles[CN.ChequeClearance].Width = 0;
                            tabStyle.GridColumnStyles[CN.VoucherReference].Width = 0;
                            tabStyle.GridColumnStyles[CN.CourtsVoucher].Width = 0;
                            tabStyle.GridColumnStyles[CN.VoucherAuthorisedBy].Width = 0;
                            tabStyle.GridColumnStyles[CN.AccountNoCompany].Width = 0;
                            tabStyle.GridColumnStyles[CN.ReturnedChequeAuthorisedBy].Width = 0;
                            tabStyle.GridColumnStyles[CN.StoreCardAcctNo].Width = 0;
                            tabStyle.GridColumnStyles[CN.StoreCardNo].Width = 0;
                            tabStyle.GridColumnStyles[CN.DepositAmount].Width = 0;
                            tabStyle.GridColumnStyles[CN.ID].Width = 0;
                            tabStyle.GridColumnStyles[CN.IsDeposit].Width = 0;
                            tabStyle.GridColumnStyles[CN.AlreadyAdded].Width = 0;

                            tabStyle.GridColumnStyles[CN.PayMethodText].Width = 90;
                            tabStyle.GridColumnStyles[CN.PayMethodText].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.PayMethodText].HeaderText = "Payment Method";

                            tabStyle.GridColumnStyles[CN.acctno].Width = 100;
                            tabStyle.GridColumnStyles[CN.acctno].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.acctno].HeaderText = GetResource("T_ACCOUNTNO");

                            tabStyle.GridColumnStyles[CN.OutstandingBalance].Width = 90;
                            tabStyle.GridColumnStyles[CN.OutstandingBalance].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.OutstandingBalance].HeaderText = GetResource("T_OUTBAL");
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.OutstandingBalance]).Format = DecimalPlaces;

                            tabStyle.GridColumnStyles[CN.Arrears].Width = 90;
                            tabStyle.GridColumnStyles[CN.Arrears].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.Arrears].HeaderText = GetResource("T_ARREARS");
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Arrears]).Format = DecimalPlaces;

                            tabStyle.GridColumnStyles[CN.InstalAmount].Width = 90;
                            tabStyle.GridColumnStyles[CN.InstalAmount].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.InstalAmount].HeaderText = GetResource("T_INSTAL");
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.InstalAmount]).Format = DecimalPlaces;

                            tabStyle.GridColumnStyles[CN.SettlementFigure].Width = 90;
                            tabStyle.GridColumnStyles[CN.SettlementFigure].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.SettlementFigure].HeaderText = GetResource("T_SETTLEMENT");
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.SettlementFigure]).Format = DecimalPlaces;

                            tabStyle.GridColumnStyles[CN.Payment].Width = 90;
                            tabStyle.GridColumnStyles[CN.Payment].ReadOnly = false;
                            tabStyle.GridColumnStyles[CN.Payment].HeaderText = GetResource("T_PAYMENT");
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Payment]).Format = DecimalPlaces;

                            tabStyle.GridColumnStyles[CN.Rebate].Width = 90;
                            tabStyle.GridColumnStyles[CN.Rebate].ReadOnly = false;
                            tabStyle.GridColumnStyles[CN.Rebate].HeaderText = GetResource("T_REBATE");
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Rebate]).Format = DecimalPlaces;

                            // The Fee column needs different readonly properties on different rows
                            tabStyle.GridColumnStyles.Remove(tabStyle.GridColumnStyles[CN.ReadOnly]);
                            tabStyle.GridColumnStyles.Remove(tabStyle.GridColumnStyles[CN.CollectionFee]);
                            DataGridEditColumn aColumnEditColumn;
                            aColumnEditColumn = new DataGridEditColumn(CN.ReadOnly, "Y");
                            aColumnEditColumn.MappingName = CN.CollectionFee;
                            aColumnEditColumn.HeaderText = GetResource("T_FEE");
                            aColumnEditColumn.Width = 90;
                            aColumnEditColumn.ReadOnly = false;
                            aColumnEditColumn.Format = DecimalPlaces;
                            tabStyle.GridColumnStyles.Add(aColumnEditColumn);

                            // Display the Net Payment when displaying the Fee
                            //AddColumnStyle(CN.NetPayment,		tabStyle,  90, true,  GetResource("T_NETPAYMENT"),	DecimalPlaces);
                            tabStyle.GridColumnStyles[CN.NetPayment].Width = 90;
                            tabStyle.GridColumnStyles[CN.NetPayment].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.NetPayment].HeaderText = GetResource("T_NETPAYMENT");
                            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.NetPayment]).Format = DecimalPlaces;

                            dgPaymentList.TableStyles.Clear();
                            dgPaymentList.TableStyles.Add(tabStyle);
                            dgPaymentList.DataSource = PaymentListView;
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

        private decimal GetTotalTenderFromGrid()
        {
            DataView PaymentList = (DataView)dgPaymentList.DataSource;
            decimal totalTender = 0;

            foreach (DataRowView dr in PaymentList)
            {
                if (dr[CN.Paymentmethod] != null && dr[CN.Paymentmethod] != DBNull.Value)
                {
                    short currPayMethod = (short)dr[CN.Paymentmethod];

                    if (PayMethod.IsPayByForeign(currPayMethod))
                    {
                        if (dr[CN.LocalTender] != null && dr[CN.LocalTender] != DBNull.Value)
                        {
                            totalTender += (decimal)dr[CN.LocalTender];
                        }
                    }
                    else
                    {
                        if (dr[CN.TenderedAccountAmt] != null && dr[CN.TenderedAccountAmt] != DBNull.Value)
                        {
                            totalTender += (decimal)dr[CN.TenderedAccountAmt];
                        }
                    }
                }
            }

            return totalTender;
        }

        private decimal GetTotalTender()
        {
            return GetTotalTenderFromGrid() + MoneyStrToDecimal(this.txtCashAmt.Text) + MoneyStrToDecimal(this.txtChequeAmt.Text);
        }

        private decimal GetTotalForeignChange()
        {
            DataView PaymentList = (DataView)dgPaymentList.DataSource;
            decimal totalChange = 0;

            foreach (DataRowView dr in PaymentList)
            {
                if (dr[CN.LocalChange] != null && dr[CN.LocalChange] != DBNull.Value)
                {
                    totalChange += (decimal)dr[CN.LocalChange];
                }
            }

            return totalChange;
        }

        private decimal GetTotalCashPaid()
        {
            DataView PaymentList = (DataView)dgPaymentList.DataSource;
            decimal totalCashPaid = 0;

            foreach (DataRowView dr in PaymentList)
            {
                if (dr[CN.Paymentmethod] != null && dr[CN.Paymentmethod] != DBNull.Value)
                {
                    short currPayMethod = (short)dr[CN.Paymentmethod];

                    if (!PayMethod.IsPayByForeign(currPayMethod) && PayMethod.IsPayByCash(currPayMethod))
                    {
                        totalCashPaid += (decimal)dr[CN.Payment];
                    }
                }
            }

            return totalCashPaid;
        }

        private decimal GetTotalChequePaid()
        {
            DataView PaymentList = (DataView)dgPaymentList.DataSource;
            decimal totalChequePaid = 0;

            foreach (DataRowView dr in PaymentList)
            {
                if (dr[CN.Paymentmethod] != null && dr[CN.Paymentmethod] != DBNull.Value)
                {
                    short currPayMethod = (short)dr[CN.Paymentmethod];

                    if (!PayMethod.IsPayByForeign(currPayMethod) && PayMethod.IsPayByCheque(currPayMethod))
                    {
                        if (dr[CN.Payment] != null && dr[CN.Payment] != DBNull.Value)
                        {
                            totalChequePaid += (decimal)dr[CN.Payment];
                        }
                    }
                }
            }

            return totalChequePaid;
        }

        private decimal GetTotalPaid()
        {
            DataView PaymentList = (DataView)dgPaymentList.DataSource;
            decimal totalPaid = 0;

            foreach (DataRowView dr in PaymentList)
            {
                if (dr[CN.Payment] != null && dr[CN.Payment] != DBNull.Value)
                {
                    totalPaid += (decimal)dr[CN.Payment];
                }
            }

            return totalPaid;
        }

        private void DisplayTotalCalculation()
        {
            txtChequeChange.Text = (MoneyStrToDecimal(this.txtChequeAmt.Text) - GetTotalChequePaid()).ToString(DecimalPlaces);
            txtCashChange.Text = ((MoneyStrToDecimal(this.txtCashAmt.Text) - GetTotalCashPaid()) + GetTotalForeignChange()).ToString(DecimalPlaces);
            txtTendered.Text = GetTotalTender().ToString(DecimalPlaces);
            txtTotalAmount.Text = GetTotalPaid().ToString(DecimalPlaces);
        }

        private void LoadDetails()
        {
            int index = dgPaymentList.CurrentRowIndex;
            if (index >= 0)
            {
                CheckPaymentCalc(index);
                // Check this account is not locked
                DataView PaymentList = (DataView)dgPaymentList.DataSource;
                DataRow newPayment = PaymentList[index].Row;
                if (newPayment[CN.LockedBy].ToString().Length > 0)
                {
                    this.txtCustId.Focus();
                    dgPaymentList.UnSelect(index);
                    ShowInfo("M_ACCOUNTLOCKED", new Object[] { newPayment[CN.acctno].ToString(), this.txtCustId.Text });
                }

                txtSelectedAcctNo.Text = (string)newPayment[CN.acctno];
                currAccountType = newPayment[CN.AccountType].ToString().Trim();

                if ((decimal)newPayment[CN.DepositAmount] > 0)
                {
                    chkDeposit.Enabled = false;
                    //chkDeposit.Checked = (bool)newPayment[CN.IsDeposit];
                    chkDeposit.Checked = true;
                }
                else
                {
                    chkDeposit.Enabled = false;
                    chkDeposit.Checked = false;
                }

                PopulatePayMethod(newPayment);

                if (newPayment[CN.Payment] != null && newPayment[CN.Payment] != DBNull.Value)
                {
                    txtPayAmount.Text = ((decimal)newPayment[CN.Payment]).ToString(DecimalPlaces);
                }
                else
                {
                    txtPayAmount.Text = (0).ToString(DecimalPlaces);
                }

                txtReceiptNo.Text = newPayment[CN.ReceiptNo].ToString();
                drpPayMethod.SelectedValue = Convert.ToInt16(newPayment[CN.Paymentmethod]);
                drpCardType.SelectedValue = Convert.ToInt16(newPayment[CN.CardType]);
                txtCardNo.Text = newPayment[CN.CardNumber].ToString();
                mtb_CardNo.Text = newPayment[CN.CardNumber].ToString();
                drpBank.SelectedValue = newPayment[CN.BankCode].ToString();
                txtBankAcctNo.Text = newPayment[CN.BankAccountNo].ToString();
            }
        }

        private bool ValidMoneyField(TextBox moneyField, out decimal moneyValue)
        {
            // Check a blank or zero money value entered
            moneyValue = 0.0M;
            moneyField.Text = moneyField.Text.Trim();
            if (!IsStrictMoney(moneyField.Text))
            {
                ShowInfo("M_NUMERIC");
                // Trap the focus in this field
                moneyField.Focus();
                return false;
            }

            moneyValue = MoneyStrToDecimal(moneyField.Text);
            moneyField.Text = moneyValue.ToString(DecimalPlaces);

            return true;
        }

        private bool ValidMoneyField(string moneyField, out decimal moneyValue)
        {
            // Check a blank or zero money value entered
            moneyValue = 0.0M;
            moneyField = moneyField.Trim();
            if (!IsStrictMoney(moneyField))
            {
                ShowInfo("M_NUMERIC");
                return false;
            }

            // Reformat
            moneyValue = MoneyStrToDecimal(moneyField);
            moneyField = moneyValue.ToString(DecimalPlaces);

            return true;
        }

        private void PopulatePayMethod(DataRow newPayment)
        {
            if (AccountManager.CheckAccountType(newPayment[CN.acctno].ToString(), out error) && (bool)newPayment[CN.IsMambuAccount] == true)
            {
                //accountType = AT.Cash;
                //Set drpPaymentMethod DataSource
                if (dtCLMambuPayMethod != null)
                    dtCLMambuPayMethod = null;
                dtCLMambuPayMethod = PaymentManager.GetMambuCLPaymentMethods(newPayment[CN.acctno].ToString(), out Error);
                drpPayMethod.DataSource = dtCLMambuPayMethod.DefaultView;
                drpPayMethod.ValueMember = CN.Code;
                drpPayMethod.DisplayMember = CN.CodeDescription;
            }
            else if (!AccountManager.CheckAccountType(newPayment[CN.acctno].ToString(), out error) && (bool)newPayment[CN.IsMambuAccount] == true)
            {
                //accountType = AT.ReadyFinance;
                //Set drpPaymentMethod DataSource
                if (chkDeposit.Checked)
                {
                    drpPayMethod.DataSource = dtPayMethod.DefaultView;
                    drpPayMethod.ValueMember = CN.Code;
                    drpPayMethod.DisplayMember = CN.CodeDescription;

                }
                else
                {
                    drpPayMethod.DataSource = dtHPMambuPayMethod.DefaultView;
                    drpPayMethod.ValueMember = CN.Code;
                    drpPayMethod.DisplayMember = CN.CodeDescription;
                }
            }
            else if ((bool)newPayment[CN.IsMambuAccount] == false)
            {
                drpPayMethod.DataSource = dtPayMethod.DefaultView;
                drpPayMethod.ValueMember = CN.Code;
                drpPayMethod.DisplayMember = CN.CodeDescription;
            }
        }

        private void SetPaymentList(decimal totPayment)
        {
            // Some countries prefer payments without cents
            int paymentPrecision = this._precision;

            if ((bool)Country[CountryParameterNames.PayWholeUnits])
            {
                paymentPrecision = 0;
            }

            DataView accountList = PaymentDataSet.Tables[TN.Accounts].DefaultView;
            DataRow recentAcct = null;
            DataRow currentRow = accountList[0].Row;

            decimal totSettlement = (decimal)currentRow[CN.SettlementFigure];
            decimal sumPayment = 0;
            decimal sumTotPayment = 0;

            for (int i = 0; i < accountList.Count; i++)
            {
                if ((decimal)accountList[i].Row[CN.RatioPay] >= 0)
                {
                    // Work out the proportion to be paid to this account
                    DataRow accountRow = accountList[i].Row;
                    decimal curPayment = 0;

                    curPayment = Math.Round(totPayment * (decimal)accountRow[CN.RatioPay], this._precision);

                    string readOnly = "Y";

                    // DSR 9 Sep 2004 - Users can now overpay accounts so that they could go into credit
                    // If an account is overpaid, then reserve the extra amount to pay off another account
                    if ((decimal)accountRow[CN.SettlementFigure] > 0 &&
                        (curPayment >= (decimal)accountRow[CN.SettlementFigure] || totPayment >= totSettlement))
                    {
                        // Don't pay over the Settlement Figure
                        curPayment = (decimal)accountRow[CN.SettlementFigure];
                    }
                    else
                    {
                        // The payment uses a precision which could be for whole units
                        curPayment = Math.Round(curPayment, paymentPrecision);
                    }

                    // Rounding up can cause a larger sum to be paid than is available
                    if (sumPayment + curPayment > totPayment)
                    {
                        decimal excess = (sumPayment + curPayment) - totPayment;
                        curPayment = curPayment - excess;
                    }

                    // Add up what is being paid to work out later if there is a reserve
                    // amount. Hopefully this approach will reduce rounding errors.
                    sumPayment = sumPayment + curPayment;

                    // Determine whether the fee can be entered on this row
                    if (((int)accountRow[CN.EmployeeNo] != 0 || (int)accountRow[CN.SegmentID] != 0)
                        && (int)accountRow[CN.DebitAccount] == 1
                        && MoneyStrToDecimal(accountRow[CN.Arrears].ToString()) >= 0.01M)
                    {
                        readOnly = "N";
                    }

                    accountRow[CN.NetPayment] = curPayment;         // Net Payment (can be changed by user when no fee)
                    accountRow[CN.CollectionFee] = 0;                   // Collection Fee (can be changed by user when a fee)
                    accountRow[CN.BailiffFee] = 0;                      // Bailiff Fee (not visible to the user)
                    accountRow[CN.Payment] = curPayment;        // Total Payment (can be changed by user when a fee)
                    accountRow[CN.CalculatedFee] = 0;                   // Calculated Collection Fee (not visible to the user)
                    accountRow[CN.ReadOnly] = readOnly;
                    accountRow[CN.Paymentmethod] = 0;
                    accountRow[CN.PayMethodText] = "Not Applicable";
                    accountRow[CN.BankCode] = 0;
                    accountRow[CN.CardType] = 0;
                    accountRow[CN.ChequeClearance] = DateTime.Today.AddDays(Convert.ToInt32(Country[CountryParameterNames.ChequeDays]));
                    accountRow[CN.VoucherReference] = "";
                    accountRow[CN.CourtsVoucher] = false;
                    accountRow[CN.VoucherAuthorisedBy] = 0;
                    accountRow[CN.AccountNoCompany] = "";
                    accountRow[CN.ReturnedChequeAuthorisedBy] = 0;
                    accountRow[CN.StoreCardAcctNo] = "";
                    accountRow[CN.StoreCardNo] = 0;
                    accountRow[CN.IsDeposit] = false;
                    accountRow[CN.DepositAmount] = (decimal)PaymentManager.GetDepositAmount(accountRow[CN.acctno].ToString(), out error);

                    // Note the row of the most recent account with a payment
                    DataRow newRow = PaymentDataSet.Tables[TN.Accounts].Rows[PaymentDataSet.Tables[TN.Accounts].Rows.Count - 1];
                    if (recentAcct == null)
                        recentAcct = newRow;
                    else if ((DateTime)newRow[CN.DateAcctOpen] > (DateTime)recentAcct[CN.DateAcctOpen])
                        recentAcct = newRow;
                }
            }

            // Spread any reserve amount across any accounts not yet settled
            decimal reserve = totPayment - sumPayment;
            decimal oldReserve = 0;
            bool allSettled = false;

            while (reserve > 0 && reserve != oldReserve && !allSettled)
            {
                // If the reserve amount does not change then it is too small
                // to add split payments to the accounts
                oldReserve = reserve;
                sumPayment = 0;
                allSettled = true;

                foreach (DataRow accountRow in PaymentDataSet.Tables[TN.Accounts].Rows)
                {
                    // Work out the proportion to be paid to this account
                    decimal curPayment = Math.Round((decimal)accountRow[CN.Payment] + (reserve * (decimal)accountRow[CN.RatioPay]), this._precision);

                    // DSR 9 Sep 2004 - Users can now overpay accounts so that they could go into credit
                    // If an account is overpaid, then reserve the extra amount to pay off another account
                    if ((decimal)accountRow[CN.SettlementFigure] > 0 && curPayment >= (decimal)accountRow[CN.SettlementFigure])
                    {
                        // Don't pay over the Settlement Figure
                        curPayment = (decimal)accountRow[CN.SettlementFigure];
                    }
                    else
                    {
                        // The payment uses a precision which could be for whole units
                        curPayment = Math.Round(curPayment, paymentPrecision);
                        allSettled = false;
                    }

                    // Rounding up can cause a larger sum to be paid than is available
                    if (sumPayment + curPayment > totPayment)
                    {
                        decimal excess = (sumPayment + curPayment) - totPayment;
                        curPayment = curPayment - excess;
                    }

                    accountRow[CN.NetPayment] = curPayment;
                    accountRow[CN.Payment] = curPayment;

                    // Add up what is being paid to work out later if there is a reserve
                    // amount. Hopefully this approach will reduce rounding errors.
                    sumPayment = sumPayment + curPayment;
                }

                // Calc new reserve if any
                reserve = totPayment - sumPayment;
            }

            // Credit any remaining reserve amount onto the account with the largest OS Balance
            // Use country precision here to try to make sure the accounts add up to the entered payment
            if (reserve > 0)
            {
                decimal maxBal = 0;
                DataRow reserveAcct = null;

                foreach (DataRow accountRow in PaymentDataSet.Tables[TN.Accounts].Rows)
                {
                    if ((decimal)accountRow[CN.SettlementFigure] - (decimal)accountRow[CN.Payment] > maxBal)
                    {
                        maxBal = (decimal)accountRow[CN.SettlementFigure] - (decimal)accountRow[CN.Payment];
                        reserveAcct = accountRow;
                    }
                }

                // If all acounts are being settled, then credit the most recent acct
                if (maxBal <= 0 && recentAcct != null)
                    reserveAcct = recentAcct;

                if (reserveAcct != null)
                {
                    reserveAcct[CN.NetPayment] = Math.Round((decimal)reserveAcct[CN.NetPayment] + reserve, this._precision);
                    reserveAcct[CN.Payment] = Math.Round((decimal)reserveAcct[CN.Payment] + reserve, this._precision);
                }
            }
            decimal newTotalAmount = totPayment;

            PaymentManager.CalculateCreditFee(
                ref PaymentDataSet,
                Config.CountryCode,
                TransType.Payment,
                ref newTotalAmount,
                out error);

            if (error.Length > 0)
                ShowError(error);

            foreach (DataRow accountRow in PaymentDataSet.Tables[TN.Accounts].Rows)
            {
                sumTotPayment = sumTotPayment + (decimal)accountRow[CN.Payment];
            }

            if (totPayment == 0)
            {
                txtPayAmount.Text = MoneyStrToDecimal("0").ToString(DecimalPlaces);
            }
        }

        private void ClearPaymentFields()
        {
            VoucherReference = "";
            CourtsVoucher = false;
            VoucherCompanyAcctNo = "";
            VoucherAuthorisedBy = 0;
            txtLocalChange.Text = MoneyStrToDecimal("0").ToString(DecimalPlaces);
            txtLocalTender.Text = MoneyStrToDecimal("0").ToString(DecimalPlaces);

            //IP - 31/01/11 - Sprint 5.9 - #3050 - selected index should be set to 0 if datasource is not null
            if (drpCardType.DataSource != null)
            {
                drpCardType.SelectedIndex = 0;
            }

            mtb_CardNo.ResetText();

            //IP - 31/01/11 - Sprint 5.9 - #3050 - selected index should be set to 0 if datasource is not null
            if (drpBank.DataSource != null)
            {
                drpBank.SelectedIndex = 0;
            }

            txtBankAcctNo.Text = string.Empty;
            txtBankAcctNo.ReadOnly = false;
            txtBankAcctNo.BackColor = Color.White;
            txtCardNo.Text = string.Empty;
            lBankAcctNo.Text = "Bank Account No";
            btnStoreCardManualEntry.Enabled = mtb_CardNo.ReadOnly = ((bool)Country[CountryParameterNames.StoreCardPasswordforManualEntry]);
            errorProvider1.SetError(btnStoreCardManualEntry, "");
        }

        private void SetPayMethod()
        {
            if (drpPayMethod.SelectedItem == null)
            {
                return;
            }

            txtLocalChange.Text = MoneyStrToDecimal("0").ToString(DecimalPlaces);
            txtLocalTender.Text = MoneyStrToDecimal("0").ToString(DecimalPlaces);

            short curPayMethod = Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code].ToString());

            bool payByEntered = (curPayMethod != 0);

            bool payByCash = PayMethod.IsPayByCash(curPayMethod);

            // When paying by cheque enable the Print Account Number button
            bool payByCheque = PayMethod.IsPayByCheque(curPayMethod);

            // When paying with a card enable the Card Type
            bool payByCard = PayMethod.IsPayByCard(curPayMethod);

            // When paying with a Gift Voucher disable the payment amounts
            bool payByGift = PayMethod.IsPayByGift(curPayMethod);

            // All methods of Foreign payments must be able to give change
            bool payByForeign = PayMethod.IsPayByForeign(curPayMethod);

            //bool payByScoreCard = (PayMethod.IsPayMethod(curPayMethod, PayMethod.StoreCard));

            this.lCardNo.Enabled = !payByCash && payByEntered && (!payByForeign || payByCheque);

            this.txtCardNo.Enabled = !payByCash && payByEntered && (!payByForeign || payByCheque) && !payByGift;

            this.txtCardNo.Visible = !payByCash && payByEntered && !payByCard && (!payByForeign || payByCheque);

            this.mtb_CardNo.Visible = !payByCash && !payByCheque && payByEntered && !payByForeign && !payByGift;

            this.lBank.Enabled = !payByCash && payByEntered && (!payByForeign || payByCheque);

            this.drpBank.Enabled = !payByCash && payByEntered && (!payByForeign || payByCheque) && !payByGift;

            this.lBankAcctNo.Enabled = !payByCash && payByEntered && (!payByForeign || payByCheque);

            this.txtBankAcctNo.Enabled = !payByCash && payByEntered && (!payByForeign || payByCheque) && !payByGift;

            this.btnPrintAcctNo.Enabled = payByCheque;
            this.lCardType.Enabled = payByCard;
            this.drpCardType.Enabled = payByCard;

            // Might need to use the Foreign Currency Calculator
            if (payByForeign)
            {
                // Call the Foreign Currency Calculator popup form
                if (isPageLoaded)
                {
                    this.ExchangeCalculator();
                }
                this.btnExchange.Visible = true;
            }
            else
            {
                this.btnExchange.Visible = false;
            }

            DataRow selectedRow = ((DataView)dgPaymentList.DataSource).Table.Rows[dgPaymentList.CurrentRowIndex];

            // Securitised accounts cannot pay with a Gift Voucher
            if (payByGift && selectedRow[CN.Securitised].ToString() == "Y")
            {
                ShowInfo("M_NOGIFTMETHOD");
                this.drpPayMethod.SelectedIndex = 0;
            }
            else
            {
                /* throw up the gift voucher popup if necessary */
                if (payByGift)
                {
                    if (isPageLoaded)
                    {
                        GiftVoucher gv = new GiftVoucher(this, FormRoot, false);
                        gv.ShowDialog();
                        if (GiftVoucherValue == 0)
                            drpPayMethod.SelectedIndex = 0;
                        else
                        {
                            txtPayAmount.Text = GiftVoucherValue.ToString(DecimalPlaces);
                        }
                    }
                }
                else
                {
                    GiftVoucherValue = 0;
                    CourtsVoucher = true;
                    VoucherReference = "";
                    VoucherAuthorisedBy = 0;
                    VoucherCompanyAcctNo = "";
                }
            }

            SetStoreCardControls(PayMethod.IsPayMethod(curPayMethod, PayMethod.StoreCard));
        }

        private bool AuthorisedPayByCheque(short curPayMethod, DataRow accountRow, string accountNo)
        {
            if (PayMethod.IsPayByCheque(curPayMethod))
            {
                DateTime curChequeClearance = DateTime.MinValue;

                // Get a list of any items to be delivered before cheque clearance
                DataSet deliverySet = PaymentManager.CheckDeliveryDate(accountNo, Config.CountryCode, out curChequeClearance, out error);

                if (error.Length > 0)
                {
                    ShowError(error);
                    return false;
                }

                // Set row filter to remove line items not required.
                string rowFilter = CN.Price + " > 0 and " +
                    CN.DateReqDel + " <= '" + curChequeClearance.ToShortDateString() + "' and " +
                    CN.DateReqDel + " not = '" + DateTime.MinValue.AddYears(1899).ToShortDateString() + "' and " +
                    CN.Qtydiff + " = 'N'";

                DataView deliveryView = deliverySet.Tables[0].DefaultView;
                deliveryView.RowFilter = rowFilter;

                if (deliveryView.Count > 0)
                {
                    foreach (DataRowView deliveryRow in deliveryView)
                    {
                        // Display a confirmation prompt for each item delivered before cheque clearance
                        if (DialogResult.No == ShowInfo("M_CHEQUECLEARANCE", new Object[] { deliveryRow[CN.ItemNo].ToString().Trim(), deliveryRow[CN.ItemDescr1].ToString().Trim(), curChequeClearance.ToShortDateString() }, MessageBoxButtons.YesNo))
                        {
                            this.drpPayMethod.SelectedIndex = 0;
                            return false;
                        }
                        return false;
                    }
                }

                //Check the account type.  Dialog only to be shown if Cash or HP
                string accType = currAccountType.ToString().Trim();
                if (accType != AT.Special)
                {
                    //Check to see if returned cheques exceed specified criteria.
                    bool authorisationRequired = false;
                    DataSet dsReturnedCheques = base.PaymentManager.GetPaymentReturnedCheques(txtCustId.Text, out authorisationRequired, out this.error);

                    if (error.Length > 0)
                    {
                        ShowError(error);
                        return false;
                    }

                    if (dsReturnedCheques != null)
                    {
                        if (authorisationRequired && dsReturnedCheques.Tables[TN.ReturnedCheques].Rows.Count > 0)
                        {
                            ChequeAuthorisationPopUp chequeAuthPopop = new ChequeAuthorisationPopUp(this.FormRoot, this);
                            //Display a dialog
                            chequeAuthPopop.ReturnedCheques = dsReturnedCheques;
                            chequeAuthPopop.ShowDialog(this);

                            if (!chequeAuthPopop.Authorised)
                            {
                                //Set the payment option back to cash
                                drpPayMethod.SelectedValue = PayMethod.Cash;
                                //UAT Issue 53 - request to make payment field disabled if cheque authorisation cancelled
                                if (chequeAuthPopop.Cancelled == true)
                                {
                                    ShowError("Cannot Make Payment By Cheque");
                                    return false;
                                }
                                return false;
                            }
                            else
                            {
                                //If Authorised allow payment
                                accountRow[CN.ChequeClearance] = curChequeClearance;
                                accountRow[CN.ReturnedChequeAuthorisedBy] = chequeAuthPopop.AuthorisingUser;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private void ExchangeCalculator()
        {
            // Call the Foreign Currency Calculator popup form

            if (dgPaymentList.CurrentRowIndex >= 0)
            {
                DataRow selectedRow = ((DataView)dgPaymentList.DataSource).Table.Rows[dgPaymentList.CurrentRowIndex];
                if (selectedRow != null && selectedRow[CN.LocalChange] != null && selectedRow[CN.LocalChange] != DBNull.Value)
                {
                    txtLocalChange.Text = Convert.ToDecimal(selectedRow[CN.LocalChange]).ToString(DecimalPlaces);
                    txtLocalTender.Text = Convert.ToDecimal(selectedRow[CN.LocalTender]).ToString(DecimalPlaces);
                }
            }

            int curPayMethod = Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code].ToString());
            ExchangeCalculator ExchangeCalculatorPopup = new ExchangeCalculator(this.FormRoot, this, curPayMethod, MoneyStrToDecimal(this.txtLocalTender.Text, DecimalPlaces));

            ExchangeCalculatorPopup.ShowDialog();
            if (ExchangeCalculatorPopup.convert)
            {
                txtLocalTender.Text = ExchangeCalculatorPopup.newAmount.ToString(DecimalPlaces);
                txtLocalChange.Text = (MoneyStrToDecimal(txtLocalTender.Text) - MoneyStrToDecimal(txtPayAmount.Text)).ToString(DecimalPlaces);
            }
        }

        private void SaveTenderValue(short curPayMethod, DataRow newPayment, decimal newPaymentValue)
        {
            newPayment[CN.LocalChange] = 0;

            if (PayMethod.IsPayByForeign(curPayMethod))
            {
                if (newPayment[CN.LocalTender] != null && newPayment[CN.LocalTender] != DBNull.Value)
                {
                    var localChange = ((decimal)newPayment[CN.LocalTender]) - newPaymentValue;
                    newPayment[CN.LocalChange] = localChange;
                    txtPayAmount.Text = newPaymentValue.ToString(DecimalPlaces);
                    txtLocalChange.Text = localChange.ToString(DecimalPlaces);
                }
            }
            else if (!(PayMethod.IsPayByCash(curPayMethod) || PayMethod.IsPayByCheque(curPayMethod)))
            {
                newPayment[CN.TenderedAccountAmt] = newPaymentValue;
            }
            else
            {
                newPayment[CN.TenderedAccountAmt] = 0;
            }
        }

        private void CheckReceiptAllocation()
        {
            if (this._curReceipt == null) return;
            string EmployeeNo = "";
            int index = dgPaymentList.CurrentRowIndex;

            if (index >= 0)
            {
                // Check this account is not locked
                DataView PaymentList = (DataView)dgPaymentList.DataSource;
                DataRow newPayment = PaymentList[index].Row;
                EmployeeNo = newPayment[CN.EmployeeNo].ToString().Trim();
            }

            if (this._curReceipt[CN.EmployeeNo].ToString().Trim().Length != 0 &&
                EmployeeNo.Trim().Length != 0)
            {
                // Check the Account and the Receipt are allocated to the same employee
                int curReceiptEmployeeNo = Convert.ToInt32(this._curReceipt[CN.EmployeeNo].ToString());
                int curAccountEmployeeNo = Convert.ToInt32(EmployeeNo);
                if (curReceiptEmployeeNo != curAccountEmployeeNo)
                {
                    if (DialogResult.No == ShowInfo("M_DIFFERENTALLOCATION", new Object[] { this.txtReceiptNo.Text, curReceiptEmployeeNo, curAccountEmployeeNo }, MessageBoxButtons.YesNo))
                    {
                        this.txtReceiptNo.Text = "";
                        this._lastReceiptNo = "";
                        this.txtReceiptNo.Focus();
                    }
                }
            }
        }

        // Form Events

        private void txtTendered_Leave(object sender, EventArgs e)
        {
            //try
            //{
            //    Function = "Payment List Screen: Validate Tender Amount";
            //    decimal curTenderAmount = 0.0M;
            //    if (!this.ValidMoneyField(this.txtTendered, out curTenderAmount)) return;
            //    if (lastTenderedAmount > 0 && MoneyStrToDecimal(txtTendered.Text) == lastTenderedAmount)
            //        return;
            //    else
            //        lastTenderedAmount = MoneyStrToDecimal(txtTendered.Text);
            //    Wait();
            //    this.PaymentSet.Tables[TN.Accounts].ColumnChanging -= new DataColumnChangeEventHandler(this.PaymentDetails_ColumnChanging);

            //    SetPaymentList(curTenderAmount);

            //    LoadData();
            //    LoadDetails();
            //}
            //catch (Exception ex)
            //{
            //    Catch(ex, Function);
            //}
            //finally
            //{
            //    StopWait();
            //}
        }

        private void txtChequeAmt_Leave(object sender, EventArgs e)
        {
            try
            {
                Function = "Payment List Screen: Validate Cheque Amount";
                decimal curChequeAmount = 0.0M;
                if (!this.ValidMoneyField(this.txtChequeAmt, out curChequeAmount)) return;
                Wait();
                DisplayTotalCalculation();
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

        private void txtCashAmt_Leave(object sender, EventArgs e)
        {
            try
            {
                Function = "Payment List Screen: Validate Cash Amount";
                decimal curCashAmount = 0.0M;
                if (!this.ValidMoneyField(this.txtCashAmt, out curCashAmount)) return;
                Wait();
                DisplayTotalCalculation();
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

        private void txtPayAmount_Leave(object sender, EventArgs e)
        {
            try
            {
                Function = "Payment List Screen: Validate Payment Amount";
                decimal curPayAmount = 0.0M;
                if (!this.ValidMoneyField(this.txtPayAmount, out curPayAmount))
                {
                    return;
                }

                if (drpPayMethod.SelectedItem != null)
                {
                    short curPayMethod = Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code].ToString());

                    if (PayMethod.IsPayByForeign(curPayMethod) && MoneyStrToDecimal(txtLocalTender.Text) > 0)
                    {
                        txtLocalChange.Text = (MoneyStrToDecimal(txtLocalTender.Text) - MoneyStrToDecimal(txtPayAmount.Text)).ToString(DecimalPlaces);
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

        private void dgPaymentList_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Function = "Payment List Screen: Click on Account List";
                Wait();
                LoadDetails();
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

        private void dgPaymentList_Enter(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Payment List Screen: Tab into Account List";
                Wait();

                int index = dgPaymentList.CurrentRowIndex;
                if (index >= 0)
                {
                    // Check this account is not locked
                    DataView PaymentList = (DataView)dgPaymentList.DataSource;
                    DataRow newPayment;
                    bool rowLocked = true;

                    // Skip rows that are locked
                    dgPaymentList.UnSelect(index);
                    while (rowLocked && index < PaymentList.Count)
                    {
                        newPayment = PaymentList[index].Row;
                        rowLocked = newPayment[CN.LockedBy].ToString().Length > 0;
                        index++;
                    }

                    if (rowLocked)
                    {
                        // All remaining rows were also locked so skip to the CustId
                        this.txtCustId.Focus();
                    }
                    else
                    {
                        // Select the next row not locked
                        dgPaymentList.Select(index - 1);
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

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Payment List Screen: OK button";
                Wait();

                if (CashTender < GetTotalCashPaid())
                {
                    ShowInfo("M_USEDTOTALCASH");
                    this.txtCashAmt.Focus();
                    return;
                }

                if (ChequeTender < GetTotalChequePaid())
                {
                    ShowInfo("M_USEDTOTALCHEQUE");
                    this.txtChequeAmt.Focus();
                    return;
                }

                if (MoneyStrToDecimal(this.txtTendered.Text) <= 0M)
                {
                    ShowInfo("M_REQUIRETENDERED");
                    //this.txtTendered.Focus();
                    return;
                }

                if (MoneyStrToDecimal(this.txtTendered.Text) < MoneyStrToDecimal(this.txtTotalAmount.Text))
                {
                    ShowInfo("M_TENDEVSTOTAL");
                    //this.txtTendered.Focus();
                    return;
                }

                // Validate the Fees and Payments

                foreach (DataTable PaymentDetails in PaymentDataSet.Tables)
                {
                    if (PaymentDetails.TableName == TN.Accounts)
                    {
                        foreach (DataRow paymentRow in PaymentDetails.Rows)
                        {

                            if ((decimal)paymentRow[CN.CollectionFee] < 0
                                || (decimal)paymentRow[CN.CollectionFee] > (decimal)paymentRow[CN.Payment]
                                || (decimal)paymentRow[CN.Payment] < 0)
                            {
                                ShowInfo("M_VALIDPAYMENT");
                                return;
                            }

                            if ((decimal)paymentRow[CN.Payment] <= 0 && Convert.ToInt16(paymentRow[CN.Paymentmethod]) > 0)
                            {
                                ShowInfo("M_VALIDPAYMENT");
                                return;
                            }

                            if ((decimal)paymentRow[CN.Payment] > 0 && Convert.ToInt16(paymentRow[CN.Paymentmethod]) == 0)
                            {
                                ShowInfo("M_SELPAYMETHOD");
                                return;
                            }
                        }
                    }
                }

                // cancel on pop up will fire twice next time we enter this screen
                // because we are going to add this event handler anyway so best get
                // rid of the old hook up here
                this.PaymentDataSet.Tables[TN.Accounts].ColumnChanging -= new DataColumnChangeEventHandler(this.PaymentDetails_ColumnChanging);
                this.IsOkClicked = true;
                Close();
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

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Payment List Screen: Cancel button";
                Wait();
                this.IsOkClicked = false;
                Close();
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

        protected void PaymentDetails_ColumnChanging(object sender, System.Data.DataColumnChangeEventArgs e)
        {
            // Validate the Fee and Payment amounts
            try
            {
                Function = "Payment List Screen: Validate Fee and Payment Amounts";
                if (!(e.Column.ColumnName.ToLower() == CN.CollectionFee || e.Column.ColumnName == CN.Payment))
                {
                    return;
                }

                decimal newAmount = 0.0M;
                if (validateColumn)
                {
                    Wait();
                    // Don't validate columns with this event that were changed by this event
                    validateColumn = false;

                    if (!this.ValidMoneyField(e.ProposedValue.ToString(), out newAmount))
                    {
                        dgPaymentList.CurrentCell = new DataGridCell(dgPaymentList.CurrentCell.RowNumber, dgPaymentList.CurrentCell.ColumnNumber);
                        return;
                    }

                    // Write back the rounded value
                    e.ProposedValue = newAmount;

                    // Get the current row
                    DataRow curPayment = e.Row;

                    if (e.Column.ColumnName.ToLower() == CN.CollectionFee
                        && curPayment[CN.ReadOnly].ToString() == "N"
                        && paymentAmountChanged == false)
                    {
                        // Validate Fee between zero and Payment
                        if (newAmount < 0 || newAmount > (decimal)curPayment[CN.Payment])
                        {
                            ShowInfo("M_VALIDFEE");
                            e.ProposedValue = (decimal)curPayment[CN.CollectionFee];
                            dgPaymentList.CurrentCell = new DataGridCell(dgPaymentList.CurrentCell.RowNumber, 5);
                            return;
                        }

                        AuthorisePrompt ap = new AuthorisePrompt(this, lAuthorise, GetResource("M_CREDITFEE"));
                        ap.ShowDialog();

                        if (ap.Authorised)
                        {
                            curPayment[CN.AuthorisedBy] = ap.AuthorisedBy;
                        }
                        else
                        {
                            curPayment[CN.AuthorisedBy] = 0;
                            e.ProposedValue = (decimal)curPayment[CN.CollectionFee];
                            dgPaymentList.CurrentCell = new DataGridCell(dgPaymentList.CurrentCell.RowNumber, 5);
                            return;
                        }

                        // Recalculate the Net Payment
                        curPayment[CN.NetPayment] = (decimal)curPayment[CN.Payment] - newAmount;
                    }

                    if (e.Column.ColumnName == CN.Payment)
                    {
                        paymentAmountChanged = true;
                        if (newAmount < 0)
                        {
                            ShowInfo("M_VALIDPAYMENT");
                            e.ProposedValue = (decimal)curPayment[CN.Payment];
                            dgPaymentList.CurrentCell = new DataGridCell(dgPaymentList.CurrentCell.RowNumber, 4);
                            return;
                        }

                        if ((bool)curPayment["isDeposit"]
                            && newAmount > 0
                            && chkDeposit.Checked
                            && newAmount != (decimal)curPayment[CN.Payment])
                        {
                            ShowInfo("M_VALIDDEPOSIT");
                            e.ProposedValue = (decimal)curPayment[CN.Payment];
                            dgPaymentList.CurrentCell = new DataGridCell(dgPaymentList.CurrentCell.RowNumber, 4);
                            return;
                        }

                        if (newAmount > ((decimal)curPayment[CN.SettlementFigure])
                            && (bool)curPayment["isDeposit"] == false
                            && !chkDeposit.Checked
                            && (bool)curPayment[CN.IsMambuAccount] == true
                            && (!PayMethod.IsPayByCash((short)curPayment[CN.Paymentmethod])
                            && !PayMethod.IsPayByCheque((short)curPayment[CN.Paymentmethod])))
                        {
                            ShowInfo("M_PAYMENTVALIDATION");
                            e.ProposedValue = (decimal)curPayment[CN.Payment];
                            dgPaymentList.CurrentCell = new DataGridCell(dgPaymentList.CurrentCell.RowNumber, 4);
                            return;
                        }

                        if (newAmount > ((decimal)curPayment[CN.SettlementFigure])
                            && (bool)curPayment["isDeposit"] == false
                            && !chkDeposit.Checked
                            && (bool)curPayment[CN.IsMambuAccount]
                            && (PayMethod.IsPayByCash((short)curPayment[CN.Paymentmethod])
                                || PayMethod.IsPayByCheque((short)curPayment[CN.Paymentmethod])))
                        {
                            e.ProposedValue = (decimal)curPayment[CN.SettlementFigure];
                            newAmount = (decimal)e.ProposedValue;
                            curPayment[CN.NetPayment] = e.ProposedValue;
                        }

                        short currPayMethod = (short)curPayment[CN.Paymentmethod];
                        decimal payAmount = newAmount;
                        decimal newFee = 0.0M;
                        decimal bailiffFee = 0.0M;
                        int debitAccount = 0;

                        if (payAmount >= 0.01M
                            && (decimal)curPayment[CN.Arrears] >= 0.01M
                            && IsStrictNumeric(curPayment[CN.EmployeeNo].ToString())
                            && curPayment[CN.EmployeeNo].ToString().Length > 0)
                        {
                            // Load the new Fee amount for the Payment Amount
                            // Note the fee is only calculated when there is an allocated employee and
                            // the employee number and arrears fields therefore contain non-blank
                            // numeric values. Employee No is zero for Tallyman.

                            int segmentId = 0;
                            int empeeNo = Convert.ToInt32(curPayment[CN.EmployeeNo].ToString());

                            PaymentManager.CalculateCreditFee(
                                curPayment[CN.acctno].ToString(),            // Acct No
                                Config.CountryCode,                             // Country Code
                                TransType.Payment,                              // Payment Type
                                ref empeeNo,                                    // Allocated Courts Person
                                (decimal)curPayment[CN.Arrears],                // Arrears
                                false,                                          // reverse calc #13746
                                ref payAmount,                                  // Payment Amount
                                out newFee,
                                out bailiffFee,
                                out debitAccount,
                                out segmentId,
                                out Error);

                            if (Error.Length > 0)
                            {
                                ShowError(Error);
                                newFee = 0;
                            }
                        }

                        curPayment[CN.DebitAccount] = debitAccount;
                        curPayment[CN.CollectionFee] = Math.Round(newFee, this._precision);
                        curPayment[CN.CalculatedFee] = Math.Round(newFee, this._precision);
                        curPayment[CN.BailiffFee] = Math.Round(bailiffFee, this._precision);

                        // Recalculate the Net Payment
                        curPayment[CN.NetPayment] = newAmount - newFee;

                        paymentAmountChanged = false;

                        SaveTenderValue(currPayMethod, curPayment, newAmount);

                        displayTotalCalculationTimer.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                validateColumn = true;
                StopWait();
            }
        }

        private void drpPayMethod_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Payment Screen: Set Pay Method";
                Wait();

                ClearPaymentFields();
                SetPayMethod();
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

        private void btnExchange_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Payment Screen: Open Exchange Calculator";
                Wait();

                // Call the Foreign Currency Calculator popup form
                this.ExchangeCalculator();
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

        private void btnAddToGrid_Click(object sender, EventArgs e)
        {
            if (drpPayMethod.SelectedItem == null)
            {
                ShowInfo("M_REQUIREPAYMETHOD");
                this.drpPayMethod.Focus();
                return;
            }

            short curPayMethod = Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code].ToString());

            if (PayMethod.IsPayMethod(curPayMethod, PayMethod.StoreCard))
            {
                if (!StoreCardCheckPayButton())
                {
                    return;
                }
            }

            if (PayMethod.IsPayByGift(curPayMethod))
            {
                foreach (DataRow accountRow in PaymentDataSet.Tables[TN.Accounts].Rows)
                {
                    if ((!string.IsNullOrEmpty(VoucherReference)) && accountRow[CN.VoucherReference].ToString() == VoucherReference)
                    {
                        ShowInfo("M_USEDGIFTVOUCHER");
                        return;
                    }
                }
            }

            if (MoneyStrToDecimal(this.txtPayAmount.Text) < 0.01M && curPayMethod != 0)
            {
                ShowInfo("M_REQUIREPAYAMOUNT");
                this.txtPayAmount.Focus();
                return;
            }

            if (curPayMethod == 0 && MoneyStrToDecimal(this.txtPayAmount.Text) > 0.01M)
            {
                ShowInfo("M_REQUIREPAYMETHOD");
                this.drpPayMethod.Focus();
                return;
            }

            if (errorProvider1.GetError(txtPayAmount).Trim().Length != 0)
            {
                ShowInfo("M_AmountPaid");
                return;
            }

            if (errorProvider1.GetError(txtCardNo).Trim().Length != 0)
            {
                ShowInfo("M_CardError");
                return;
            }

            if (drpBank.Visible && drpBank.Enabled && drpBank.SelectedIndex == 0)
            {
                ShowInfo("M_REQUIREBANK");
                this.drpBank.Focus();
                return;
            }

            // When paying by card the card type must be entered
            if (drpCardType.Visible && drpCardType.Enabled && drpCardType.SelectedIndex == 0)
            {
                ShowInfo("M_REQUIRECARDTYPE");
                this.drpCardType.Focus();
                return;
            }

            // When paying by cheque the Bank Account number must be entered
            if (txtBankAcctNo.Visible && txtBankAcctNo.Enabled && (!txtBankAcctNo.ReadOnly) && txtBankAcctNo.Text.Trim().Length == 0)
            {
                ShowInfo("M_REQUIREBANKACCOUNTNO");
                this.txtBankAcctNo.Focus();
                return;
            }

            if (txtCardNo.Visible && this.txtCardNo.Enabled && (!this.txtCardNo.ReadOnly) && this.txtCardNo.Text.Trim().Length == 0)
            {
                ShowInfo("M_REQUIRECHEQUENO");
                this.txtCardNo.Focus();
                return;
            }

            if (mtb_CardNo.Visible && mtb_CardNo.Enabled && (!mtb_CardNo.ReadOnly) && !mtb_CardNo.MaskCompleted)
            {
                ShowInfo("M_INCOMPLETECARDNO");
                this.mtb_CardNo.Focus();
                return;
            }

            //SPECIAL STORECARD ROUTINE
            if (PayMethod.IsPayMethod(curPayMethod, PayMethod.StoreCard))
            {
                if (errorProvider1.GetError(btnStoreCardManualEntry).Trim().Length != 0)
                {
                    ShowInfo("M_SoreCardErrors");
                    this.mtb_CardNo.Focus();
                    return;
                }

                if (errorProviderStoreCard.GetError(drpPayMethod).Trim().Length != 0)
                {
                    ShowInfo("M_SELECTDIFFERENTPAYMETHOD");
                    this.drpPayMethod.Focus();
                    return;
                }
            }

            int index = dgPaymentList.CurrentRowIndex;
            if (index >= 0)
            {
                DataView PaymentList = (DataView)dgPaymentList.DataSource;
                DataRow newPayment = PaymentList[index].Row;
                if (chkDeposit.Checked)
                {
                    if (MoneyStrToDecimal(txtPayAmount.Text) > (decimal)newPayment["DepositAmount"]) //Change for accepting partial payment
                    {
                        ShowInfo("M_INVALIDDEPOSIT", new object[] { ((decimal)newPayment["DepositAmount"]).ToString(DecimalPlaces)}, MessageBoxButtons.OK);
                        return;
                    }
                }

                bool paymentLessThanSettlement = true;

                if (MoneyStrToDecimal(txtPayAmount.Text) > ((decimal)newPayment[CN.SettlementFigure])
                    && !chkDeposit.Checked
                    && (bool)newPayment[CN.IsMambuAccount] == true
                    && (!PayMethod.IsPayByCash(curPayMethod)
                        && !PayMethod.IsPayByCheque(curPayMethod)))
                {
                    paymentLessThanSettlement = false;
                    ShowInfo("M_PAYMENTVALIDATION");
                    return;
                }
                else
                {
                    if (!AuthorisedPayByCheque(Convert.ToInt16(drpPayMethod.SelectedValue), newPayment, (string)newPayment[CN.acctno]))
                    {
                        return;
                    }

                    //Payment Method Text
                    newPayment[CN.PayMethodText] = drpPayMethod.Text;
                    //Payment Method Value
                    newPayment[CN.Paymentmethod] = drpPayMethod.SelectedValue;

                    //Amount
                    if (paymentLessThanSettlement)
                        newPayment[CN.Payment] = txtPayAmount.Text.ToString();
                    else
                        newPayment[CN.Payment] = newPayment[CN.SettlementFigure];

                    //receipt number
                    newPayment[CN.ReceiptNo] = txtReceiptNo.Text;
                    //Card Type
                    if (drpCardType.SelectedIndex > 0)
                        newPayment[CN.CardType] = drpCardType.SelectedValue;
                    //Card No.
                    //Cheque No
                    newPayment[CN.CardNumber] = txtCardNo.Text;
                    //Bank
                    if (drpBank.SelectedIndex > 0)
                        newPayment[CN.BankCode] = drpBank.SelectedValue;
                    //Account No
                    newPayment[CN.BankAccountNo] = txtBankAcctNo.Text;
                    //IsDeposit
                    newPayment[CN.IsDeposit] = chkDeposit.Checked;
                    newPayment[CN.VoucherReference] = VoucherReference;
                    newPayment[CN.CourtsVoucher] = CourtsVoucher;
                    newPayment[CN.AccountNoCompany] = VoucherCompanyAcctNo;
                    newPayment[CN.VoucherAuthorisedBy] = VoucherAuthorisedBy;

                    if (PayMethod.IsPayByForeign(curPayMethod))
                    {
                        newPayment[CN.LocalTender] = MoneyStrToDecimal(txtLocalTender.Text);
                    }
                    else
                    {
                        newPayment[CN.LocalTender] = 0;
                    }

                    SaveTenderValue(curPayMethod, newPayment, (decimal)newPayment[CN.Payment]);

                    DisplayTotalCalculation();

                    VoucherReference = "";
                    CourtsVoucher = false;
                    VoucherCompanyAcctNo = "";
                    VoucherAuthorisedBy = 0;
                }
            }
        }

        private void ReceiptNo_Leave(object sender, System.EventArgs e)
        {
            // Validate a Receipt Number
            try
            {
                Function = "Payment Screen: Validate Receipt Number";

                // Make sure this event has been fired when it is useful
                this.txtReceiptNo.Text = this.txtReceiptNo.Text.Trim();
                if (this.txtReceiptNo.Text == this._lastReceiptNo)
                {
                    return;
                }
                this._lastReceiptNo = this.txtReceiptNo.Text;

                // Check a numeric Receipt No has been entered
                if (this.txtReceiptNo.Text.Length < 1)
                {
                    return;
                }
                else if (!IsNumeric(this.txtReceiptNo.Text) ||
                    this.txtReceiptNo.Text.Substring(0, 1) == "+" ||
                    this.txtReceiptNo.Text.Substring(0, 1) == "-")
                {
                    ShowInfo("M_NUMERIC");
                    this.txtReceiptNo.Text = "";
                    this._lastReceiptNo = "";
                    this.txtReceiptNo.Focus();
                    return;
                }

                Wait();
                // Load the Receipt details
                DataSet TempReceiptSet = PaymentManager.GetTempReceipt(Convert.ToInt32(this.txtReceiptNo.Text), out error);

                if (error.Length > 0)
                {
                    ShowError(error);
                    StopWait();
                    return;
                }

                foreach (DataTable TempReceiptDetails in TempReceiptSet.Tables)
                {
                    if (TempReceiptDetails.TableName == TN.TempReceipt)
                    {
                        if (TempReceiptDetails.Rows.Count > 0)
                            this._curReceipt = TempReceiptDetails.Rows[0];
                    }
                }

                if (this._curReceipt == null)
                {
                    // Receipt number not found
                    ShowInfo("M_RECEIPTNOTALLOCATED", new Object[] { this.txtReceiptNo.Text });
                    this.txtReceiptNo.Text = "";
                    this._lastReceiptNo = "";
                    this.txtReceiptNo.Focus();
                }
                else if (Convert.ToDecimal(this._curReceipt[CN.Amount].ToString()) != 0.0M)
                {
                    // Receipt already has a payment
                    ShowInfo("M_RECEIPTPAID", new Object[] { this.txtReceiptNo.Text });
                    this.txtReceiptNo.Text = "";
                    this._lastReceiptNo = "";
                    this.txtReceiptNo.Focus();
                }
                else
                {
                    this.CheckReceiptAllocation();
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

        private void mtb_Cardno_TextChanged(object sender, EventArgs e)
        {
            txtCardNo.Text = mtb_CardNo.Text;

            if (PayMethod.IsPayMethod(Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code].ToString()), PayMethod.StoreCard))
            {
                if (mtb_CardNo.Text.Length == 16)
                {
                    ValidateStoreCard();
                }
            }
        }

        private void chkDeposit_CheckedChanged(object sender, EventArgs e)
        {
            int index = dgPaymentList.CurrentRowIndex;

            if (index >= 0)
            {
                // Check this account is not locked
                DataView PaymentList = (DataView)dgPaymentList.DataSource;
                DataRow newPayment = PaymentList[index].Row;
                PopulatePayMethod(newPayment);
            }
        }

        private void btnPrintAcctNo_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "Payment Screen: Update Payment Card Button";

                Wait();

                int index = dgPaymentList.CurrentRowIndex;

                if (index >= 0)
                {
                    DataView PaymentList = (DataView)dgPaymentList.DataSource;
                    PrintAcctNoOnCheque(PaymentList[index].Row[CN.acctno].ToString());
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

        private void btnStoreCardManualEntry_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "Payment Screen: StoreCardManualEntry Button";

                Wait();
                AuthoriseCheck Auth = new AuthoriseCheck("Payment", "mtb_Cardno");

                mtb_CardNo.Visible = true;

                if (!Auth.ControlPermissionCheck(Credential.User).HasValue)
                {
                    //    tcMain.SelectedTab = tpEmployee;
                    Auth.ShowDialog();
                    if (Auth.IsAuthorised)
                    {
                        mtb_CardNo.ReadOnly = false;
                        mtb_CardNo.Focus();
                    }
                    else
                        mtb_CardNo.ReadOnly = true;
                }
                else
                {
                    mtb_CardNo.ReadOnly = false;
                    mtb_CardNo.Focus();
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

        private void PaymentList_Load(object sender, EventArgs e)
        {
            try
            {
                this.IsOkClicked = false;
                Function = "Payment Screen: Form Load";
                Wait();

                dtCLMambuPayMethod = PaymentManager.GetMambuCLPaymentMethods(string.Empty, out error);
                dtHPMambuPayMethod = PaymentManager.GetMambuHPPaymentMethods(out error);
                dtPayMethod = ((DataTable)StaticData.Tables[TN.PayMethod]).Copy();
                dtPayMethod.DefaultView.RowFilter = DefaultPaymentFilter;
                if ((bool)Country[CountryParameterNames.StoreCardEnabled] != true)
                {
                    dtPayMethod.DefaultView.RowFilter += AdditionalPaymentFilterStoreCard;
                }
                drpCardType.DataSource = (DataTable)StaticData.Tables[TN.CreditCard];
                drpCardType.ValueMember = CN.Code;
                drpCardType.DisplayMember = CN.CodeDescription;

                drpBank.DataSource = (DataTable)StaticData.Tables[TN.Bank];
                drpBank.ValueMember = CN.BankCode;
                drpBank.DisplayMember = CN.BankName;

                if (TenderedAmount == 0)
                {
                    SetPaymentList(0);
                }

                this.LoadData();
                LoadDetails();

                DisplayTotalCalculation();

                if (((bool)Country[CountryParameterNames.StoreCardEnabled]))
                {
                    MagStripeReader = new StoreCardMagStripeReader(Country[CountryParameterNames.StoreCardMagStripeReaderName].ToString(),
                    readEvent =>
                    {
                        if (readEvent.state == ControlState.Idle && IsStoreCardPaymentSelected())
                        {
                            this.BeginInvoke(new InvokeDelegate(delegate
                            {
                                mtb_CardNo.Text = DecodeMsrTrack.Decode(readEvent.Track1);
                            }));
                        }
                    },
                    errorevent =>
                    {
                        MainForm.Current.ShowStatus("Magnetic Stripe Read failed.");
                    }
                    );
                }

                if (!cbMiniStat.Checked)
                {
                    var currAccountType = AT.Cash;

                    foreach (DataTable PaymentDetails in PaymentDataSet.Tables)
                    {
                        if (PaymentDetails.TableName == TN.Accounts)
                        {
                            foreach (DataRow paymentRow in PaymentDetails.Rows)
                            {
                                if (paymentRow[CN.AccountType].ToString().Trim() == AT.ReadyFinance)
                                {
                                    currAccountType = AT.ReadyFinance;
                                    break;
                                }
                            }

                            break;
                        }
                    }

                    if (currAccountType == AT.Cash)
                    {
                        cbMiniStat.Enabled = false;
                        cbMiniStat.Checked = false;
                    }
                    else
                    {
                        cbMiniStat.Enabled = true;
                        cbMiniStat.Checked = PrintAutomaticMiniStatement;
                    }
                }

                displayTotalCalculationTimer.Interval = 2000;
                displayTotalCalculationTimer.Tick += new EventHandler(displayTotalCalculationTimer_Tick);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
                ((MainForm)this.FormRoot).statusBar1.Text = "";
            }
            finally
            {
                isPageLoaded = true;
                StopWait();
            }
        }

        private void displayTotalCalculationTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                this.IsOkClicked = false;
                Function = "Payment Screen: displayTotalCalculationTimer_Tick";
                Wait();
                displayTotalCalculationTimer.Stop();

                DisplayTotalCalculation();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
                ((MainForm)this.FormRoot).statusBar1.Text = "";
            }
            finally
            {
                StopWait();
            }
        }
    }
}
