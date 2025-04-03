using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Data;
using STL.Common.Static;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Categories;
using STL.Common.Constants.FTransaction;

namespace STL.PL
{
	/// <summary>
	/// Popup to sell or reedeem a gift voucher. A gift voucher is sold by
	/// entering a unique reference number and the amount of the gift, along
	/// with some payment details. It is redeemed by searching for same
	/// reference number.
	/// </summary>
	public class GiftVoucher : CommonForm
	{
		private new string Error = "";
		private decimal voucherValue = 0;
		private bool _privilegeClub = false;
		private DataSet _accountSet = null;
		private decimal _soldAmount = 0;
		public decimal soldAmount 
		{
			get 
			{
				return _soldAmount;
			}
			set
			{
				_soldAmount = value;
			}
		}
		private bool Sale = false;

		private Control authExpiredVoucher = null;
		private Crownwood.Magic.Controls.TabControl tcMode;
		private Crownwood.Magic.Controls.TabPage tpSell;
		private Crownwood.Magic.Controls.TabPage tpRedeem;
		private System.Windows.Forms.RadioButton rbOther;
		private System.Windows.Forms.RadioButton rbCourts;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ToolTip tooltip;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ErrorProvider errors;
		private System.Windows.Forms.ComboBox drpCompany;
		private System.Windows.Forms.Button btnSell;
		private System.Windows.Forms.GroupBox gbReference;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox drpCardType;
		private System.Windows.Forms.Label lCardType;
		private System.Windows.Forms.ComboBox drpBank;
		private System.Windows.Forms.ComboBox drpPayMethod;
		private System.Windows.Forms.Label lBankAcctNo;
		private System.Windows.Forms.TextBox txtBankAcctNo;
		private System.Windows.Forms.Label lBank;
		private System.Windows.Forms.Label lCardNo;
		private System.Windows.Forms.TextBox txtCardNo;
		private System.Windows.Forms.Label lPayMethod;
		private System.Windows.Forms.GroupBox gbRedeemType;
		private System.Windows.Forms.GroupBox gbRedeemRef;
		private System.Windows.Forms.Button btnRedeemValidate;
		private System.Windows.Forms.TextBox txtRedeemReference;
		private System.Windows.Forms.GroupBox gbRedeemDetails;
		private System.Windows.Forms.DateTimePicker dtRedeemExpiryDate;
		private System.Windows.Forms.Button btnSellValidate;
		private System.Windows.Forms.TextBox txtSellReference;
		private System.Windows.Forms.TextBox txtSellValue;
		private System.Windows.Forms.DateTimePicker dtSellExpiryDate;
		private System.Windows.Forms.GroupBox gbSellPayment;
		private System.Windows.Forms.GroupBox gbSellDetails;
		private System.Windows.Forms.TextBox txtRedeemValue;
		private System.Windows.Forms.CheckBox chxFree;
		private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnCancelRedeem;
		private System.Windows.Forms.Button btnRedeem;

		private void HashMenus()
		{
			dynamicMenus = new Hashtable();
			dynamicMenus[this.Name+":chxFree"] = this.chxFree;
		}

		public GiftVoucher(TranslationDummy d)
		{
			InitializeComponent();

		}
		public GiftVoucher(Form parent, Form root, bool sale)
		{
			InitializeComponent();
			TranslateControls();
			HashMenus();

			ApplyRoleRestrictions();

			tooltip.SetToolTip(btnSellValidate, GetResource("TT_VALIDATE"));
			tooltip.SetToolTip(btnRedeemValidate, GetResource("TT_VALIDATE"));

			this.FormRoot = root;
			this.FormParent = parent;
            //UAT649 -jec 18/11/09 if Gift voucher a/c not set up, set error 
            if (((string)Country[CountryParameterNames.GiftVoucherAccount]).Trim() == "") //IP - 22/04/10 - UAT(117) UAT5.2
            {
                errors.SetError(txtSellReference, "The Gift Voucher Account must be set up before continuing.");
                txtSellReference.Enabled = false;
                btnSell.Enabled = false;
                //UAT14 jec 15/03/10
                btnSellValidate.Enabled = false;
                txtRedeemReference.Enabled = false;
                btnRedeem.Enabled = false;
                btnRedeem.Enabled = false;
            }

			LoadStaticData();
			Sale = sale;

			if(Sale)
			{
				tpRedeem.Enabled = false;	
				dtSellExpiryDate.Value = DateTime.Today.AddMonths(Convert.ToInt32(Country[CountryParameterNames.DefaultVoucherExpiry]));
			}
			else
			{
				tpSell.Enabled = false;
				tcMode.SelectedTab = tpRedeem;
				
				//Ensure both voucher types are available when trying
				//to redeem a gift voucher from the Cash & Go screen
				//rbOther.Enabled = !(parent is NewAccount);
			}
		}


		public GiftVoucher(Form parent, Form root, int voucherAmount, DataSet accountSet)
		{
			// Constructor to sell a fixed amount for a free Privilege Club voucher

			InitializeComponent();
			TranslateControls();
			HashMenus();

			ApplyRoleRestrictions();

			tooltip.SetToolTip(btnSellValidate, GetResource("TT_VALIDATE"));
			tooltip.SetToolTip(btnRedeemValidate, GetResource("TT_VALIDATE"));

			this.FormRoot = root;
			this.FormParent = parent;

			LoadStaticData();
            Sale = true;

			// Can only sell a fixed value voucher
			this._privilegeClub = true;
			this._accountSet = accountSet;
			this.tpRedeem.Enabled = false;	
			this.dtSellExpiryDate.Value = DateTime.Today.AddMonths(Convert.ToInt32(Country[CountryParameterNames.DefaultVoucherExpiry]));
			this.txtSellValue.Enabled = false;
			this.txtSellValue.Text = voucherAmount.ToString(DecimalPlaces);
			this.chxFree.Enabled = false;
			this.chxFree.Checked = true;
			this.gbSellPayment.Enabled = false;
           
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GiftVoucher));
            this.gbRedeemType = new System.Windows.Forms.GroupBox();
            this.drpCompany = new System.Windows.Forms.ComboBox();
            this.rbCourts = new System.Windows.Forms.RadioButton();
            this.rbOther = new System.Windows.Forms.RadioButton();
            this.gbRedeemRef = new System.Windows.Forms.GroupBox();
            this.btnRedeemValidate = new System.Windows.Forms.Button();
            this.txtRedeemReference = new System.Windows.Forms.TextBox();
            this.gbRedeemDetails = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRedeemValue = new System.Windows.Forms.TextBox();
            this.dtRedeemExpiryDate = new System.Windows.Forms.DateTimePicker();
            this.btnSell = new System.Windows.Forms.Button();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.errors = new System.Windows.Forms.ErrorProvider(this.components);
            this.authExpiredVoucher = new System.Windows.Forms.Control();
            this.tcMode = new Crownwood.Magic.Controls.TabControl();
            this.tpSell = new Crownwood.Magic.Controls.TabPage();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbSellPayment = new System.Windows.Forms.GroupBox();
            this.drpCardType = new System.Windows.Forms.ComboBox();
            this.lCardType = new System.Windows.Forms.Label();
            this.drpBank = new System.Windows.Forms.ComboBox();
            this.drpPayMethod = new System.Windows.Forms.ComboBox();
            this.lBankAcctNo = new System.Windows.Forms.Label();
            this.txtBankAcctNo = new System.Windows.Forms.TextBox();
            this.lBank = new System.Windows.Forms.Label();
            this.txtCardNo = new System.Windows.Forms.TextBox();
            this.lPayMethod = new System.Windows.Forms.Label();
            this.lCardNo = new System.Windows.Forms.Label();
            this.gbReference = new System.Windows.Forms.GroupBox();
            this.btnSellValidate = new System.Windows.Forms.Button();
            this.txtSellReference = new System.Windows.Forms.TextBox();
            this.gbSellDetails = new System.Windows.Forms.GroupBox();
            this.chxFree = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSellValue = new System.Windows.Forms.TextBox();
            this.dtSellExpiryDate = new System.Windows.Forms.DateTimePicker();
            this.tpRedeem = new Crownwood.Magic.Controls.TabPage();
            this.btnCancelRedeem = new System.Windows.Forms.Button();
            this.btnRedeem = new System.Windows.Forms.Button();
            this.gbRedeemType.SuspendLayout();
            this.gbRedeemRef.SuspendLayout();
            this.gbRedeemDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errors)).BeginInit();
            this.tcMode.SuspendLayout();
            this.tpSell.SuspendLayout();
            this.gbSellPayment.SuspendLayout();
            this.gbReference.SuspendLayout();
            this.gbSellDetails.SuspendLayout();
            this.tpRedeem.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbRedeemType
            // 
            this.gbRedeemType.BackColor = System.Drawing.SystemColors.Control;
            this.gbRedeemType.Controls.Add(this.drpCompany);
            this.gbRedeemType.Controls.Add(this.rbCourts);
            this.gbRedeemType.Controls.Add(this.rbOther);
            this.gbRedeemType.Location = new System.Drawing.Point(16, 96);
            this.gbRedeemType.Name = "gbRedeemType";
            this.gbRedeemType.Size = new System.Drawing.Size(240, 104);
            this.gbRedeemType.TabIndex = 0;
            this.gbRedeemType.TabStop = false;
            this.gbRedeemType.Text = "Voucher Type";
            // 
            // drpCompany
            // 
            this.drpCompany.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCompany.Enabled = false;
            this.drpCompany.Location = new System.Drawing.Point(32, 64);
            this.drpCompany.Name = "drpCompany";
            this.drpCompany.Size = new System.Drawing.Size(152, 23);
            this.drpCompany.TabIndex = 2;
            // 
            // rbCourts
            // 
            this.rbCourts.Checked = true;
            this.rbCourts.Location = new System.Drawing.Point(32, 24);
            this.rbCourts.Name = "rbCourts";
            this.rbCourts.Size = new System.Drawing.Size(72, 24);
            this.rbCourts.TabIndex = 1;
            this.rbCourts.TabStop = true;
            this.rbCourts.Text = "Courts";
            this.rbCourts.CheckedChanged += new System.EventHandler(this.rbCourts_CheckedChanged);
            this.rbCourts.Click += new System.EventHandler(this.rbCourts_CheckedChanged);
            // 
            // rbOther
            // 
            this.rbOther.Location = new System.Drawing.Point(112, 24);
            this.rbOther.Name = "rbOther";
            this.rbOther.Size = new System.Drawing.Size(64, 24);
            this.rbOther.TabIndex = 0;
            this.rbOther.Text = "Other";
            // 
            // gbRedeemRef
            // 
            this.gbRedeemRef.BackColor = System.Drawing.SystemColors.Control;
            this.gbRedeemRef.Controls.Add(this.btnRedeemValidate);
            this.gbRedeemRef.Controls.Add(this.txtRedeemReference);
            this.gbRedeemRef.Location = new System.Drawing.Point(16, 16);
            this.gbRedeemRef.Name = "gbRedeemRef";
            this.gbRedeemRef.Size = new System.Drawing.Size(240, 72);
            this.gbRedeemRef.TabIndex = 1;
            this.gbRedeemRef.TabStop = false;
            this.gbRedeemRef.Text = "Reference";
            // 
            // btnRedeemValidate
            // 
            this.btnRedeemValidate.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnRedeemValidate.BackgroundImage")));
            this.btnRedeemValidate.Location = new System.Drawing.Point(200, 34);
            this.btnRedeemValidate.Name = "btnRedeemValidate";
            this.btnRedeemValidate.Size = new System.Drawing.Size(16, 16);
            this.btnRedeemValidate.TabIndex = 1;
            this.btnRedeemValidate.Click += new System.EventHandler(this.btnRedeemValidate_Click);
            // 
            // txtRedeemReference
            // 
            this.txtRedeemReference.Location = new System.Drawing.Point(32, 32);
            this.txtRedeemReference.MaxLength = 32;
            this.txtRedeemReference.Name = "txtRedeemReference";
            this.txtRedeemReference.Size = new System.Drawing.Size(152, 23);
            this.txtRedeemReference.TabIndex = 0;
            // 
            // gbRedeemDetails
            // 
            this.gbRedeemDetails.BackColor = System.Drawing.SystemColors.Control;
            this.gbRedeemDetails.Controls.Add(this.label2);
            this.gbRedeemDetails.Controls.Add(this.label1);
            this.gbRedeemDetails.Controls.Add(this.txtRedeemValue);
            this.gbRedeemDetails.Controls.Add(this.dtRedeemExpiryDate);
            this.gbRedeemDetails.Location = new System.Drawing.Point(16, 208);
            this.gbRedeemDetails.Name = "gbRedeemDetails";
            this.gbRedeemDetails.Size = new System.Drawing.Size(240, 128);
            this.gbRedeemDetails.TabIndex = 2;
            this.gbRedeemDetails.TabStop = false;
            this.gbRedeemDetails.Text = "Details";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(40, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Expiry Date";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(40, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Voucher Value";
            // 
            // txtRedeemValue
            // 
            this.txtRedeemValue.Location = new System.Drawing.Point(40, 40);
            this.txtRedeemValue.Name = "txtRedeemValue";
            this.txtRedeemValue.ReadOnly = true;
            this.txtRedeemValue.Size = new System.Drawing.Size(144, 23);
            this.txtRedeemValue.TabIndex = 1;
            // 
            // dtRedeemExpiryDate
            // 
            this.dtRedeemExpiryDate.Enabled = false;
            this.dtRedeemExpiryDate.Location = new System.Drawing.Point(40, 88);
            this.dtRedeemExpiryDate.Name = "dtRedeemExpiryDate";
            this.dtRedeemExpiryDate.Size = new System.Drawing.Size(144, 23);
            this.dtRedeemExpiryDate.TabIndex = 0;
            // 
            // btnSell
            // 
            this.btnSell.Location = new System.Drawing.Point(72, 376);
            this.btnSell.Name = "btnSell";
            this.btnSell.Size = new System.Drawing.Size(48, 24);
            this.btnSell.TabIndex = 4;
            this.btnSell.Text = "Enter";
            this.btnSell.Click += new System.EventHandler(this.btnSell_Click);
            // 
            // errors
            // 
            this.errors.ContainerControl = this;
            // 
            // authExpiredVoucher
            // 
            this.authExpiredVoucher.Enabled = false;
            this.authExpiredVoucher.Location = new System.Drawing.Point(0, 0);
            this.authExpiredVoucher.Name = "authExpiredVoucher";
            this.authExpiredVoucher.Size = new System.Drawing.Size(0, 0);
            this.authExpiredVoucher.TabIndex = 0;
            this.authExpiredVoucher.Visible = false;
            // 
            // tcMode
            // 
            this.tcMode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tcMode.IDEPixelArea = true;
            this.tcMode.Location = new System.Drawing.Point(0, 0);
            this.tcMode.Name = "tcMode";
            this.tcMode.PositionTop = true;
            this.tcMode.SelectedIndex = 1;
            this.tcMode.SelectedTab = this.tpRedeem;
            this.tcMode.ShrinkPagesToFit = false;
            this.tcMode.Size = new System.Drawing.Size(280, 440);
            this.tcMode.TabIndex = 3;
            this.tcMode.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpSell,
            this.tpRedeem});
            this.tcMode.SelectionChanged += new System.EventHandler(this.tcMode_SelectionChanged);
            // 
            // tpSell
            // 
            this.tpSell.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tpSell.Controls.Add(this.btnCancel);
            this.tpSell.Controls.Add(this.gbSellPayment);
            this.tpSell.Controls.Add(this.gbReference);
            this.tpSell.Controls.Add(this.gbSellDetails);
            this.tpSell.Controls.Add(this.btnSell);
            this.tpSell.Location = new System.Drawing.Point(0, 25);
            this.tpSell.Name = "tpSell";
            this.tpSell.Selected = false;
            this.tpSell.Size = new System.Drawing.Size(280, 415);
            this.tpSell.TabIndex = 3;
            this.tpSell.Title = "Sell";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(144, 376);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(56, 24);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbSellPayment
            // 
            this.gbSellPayment.Controls.Add(this.drpCardType);
            this.gbSellPayment.Controls.Add(this.lCardType);
            this.gbSellPayment.Controls.Add(this.drpBank);
            this.gbSellPayment.Controls.Add(this.drpPayMethod);
            this.gbSellPayment.Controls.Add(this.lBankAcctNo);
            this.gbSellPayment.Controls.Add(this.txtBankAcctNo);
            this.gbSellPayment.Controls.Add(this.lBank);
            this.gbSellPayment.Controls.Add(this.txtCardNo);
            this.gbSellPayment.Controls.Add(this.lPayMethod);
            this.gbSellPayment.Controls.Add(this.lCardNo);
            this.gbSellPayment.Enabled = false;
            this.gbSellPayment.Location = new System.Drawing.Point(8, 176);
            this.gbSellPayment.Name = "gbSellPayment";
            this.gbSellPayment.Size = new System.Drawing.Size(256, 192);
            this.gbSellPayment.TabIndex = 5;
            this.gbSellPayment.TabStop = false;
            this.gbSellPayment.Text = "Payment";
            // 
            // drpCardType
            // 
            this.drpCardType.BackColor = System.Drawing.SystemColors.Window;
            this.drpCardType.Enabled = false;
            this.drpCardType.Items.AddRange(new object[] {
            ""});
            this.drpCardType.Location = new System.Drawing.Point(136, 40);
            this.drpCardType.Name = "drpCardType";
            this.drpCardType.Size = new System.Drawing.Size(104, 23);
            this.drpCardType.TabIndex = 62;
            // 
            // lCardType
            // 
            this.lCardType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lCardType.Location = new System.Drawing.Point(136, 24);
            this.lCardType.Name = "lCardType";
            this.lCardType.Size = new System.Drawing.Size(64, 16);
            this.lCardType.TabIndex = 60;
            this.lCardType.Text = "Card Type";
            // 
            // drpBank
            // 
            this.drpBank.BackColor = System.Drawing.SystemColors.Window;
            this.drpBank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBank.Enabled = false;
            this.drpBank.Location = new System.Drawing.Point(16, 120);
            this.drpBank.Name = "drpBank";
            this.drpBank.Size = new System.Drawing.Size(224, 23);
            this.drpBank.TabIndex = 64;
            // 
            // drpPayMethod
            // 
            this.drpPayMethod.BackColor = System.Drawing.SystemColors.Window;
            this.drpPayMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPayMethod.Location = new System.Drawing.Point(16, 40);
            this.drpPayMethod.Name = "drpPayMethod";
            this.drpPayMethod.Size = new System.Drawing.Size(104, 23);
            this.drpPayMethod.TabIndex = 61;
            this.drpPayMethod.SelectedIndexChanged += new System.EventHandler(this.drpPayMethod_SelectedIndexChanged);
            // 
            // lBankAcctNo
            // 
            this.lBankAcctNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lBankAcctNo.Location = new System.Drawing.Point(16, 144);
            this.lBankAcctNo.Name = "lBankAcctNo";
            this.lBankAcctNo.Size = new System.Drawing.Size(96, 16);
            this.lBankAcctNo.TabIndex = 57;
            this.lBankAcctNo.Text = "Bank Account No";
            // 
            // txtBankAcctNo
            // 
            this.txtBankAcctNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtBankAcctNo.Enabled = false;
            this.txtBankAcctNo.Location = new System.Drawing.Point(16, 160);
            this.txtBankAcctNo.MaxLength = 30;
            this.txtBankAcctNo.Name = "txtBankAcctNo";
            this.txtBankAcctNo.Size = new System.Drawing.Size(224, 23);
            this.txtBankAcctNo.TabIndex = 65;
            // 
            // lBank
            // 
            this.lBank.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lBank.Location = new System.Drawing.Point(16, 104);
            this.lBank.Name = "lBank";
            this.lBank.Size = new System.Drawing.Size(40, 16);
            this.lBank.TabIndex = 56;
            this.lBank.Text = "Bank";
            // 
            // txtCardNo
            // 
            this.txtCardNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtCardNo.Enabled = false;
            this.txtCardNo.Location = new System.Drawing.Point(16, 80);
            this.txtCardNo.MaxLength = 30;
            this.txtCardNo.Name = "txtCardNo";
            this.txtCardNo.Size = new System.Drawing.Size(224, 23);
            this.txtCardNo.TabIndex = 63;
            // 
            // lPayMethod
            // 
            this.lPayMethod.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lPayMethod.Location = new System.Drawing.Point(16, 24);
            this.lPayMethod.Name = "lPayMethod";
            this.lPayMethod.Size = new System.Drawing.Size(80, 16);
            this.lPayMethod.TabIndex = 58;
            this.lPayMethod.Text = "Pay Method";
            // 
            // lCardNo
            // 
            this.lCardNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lCardNo.Location = new System.Drawing.Point(16, 64);
            this.lCardNo.Name = "lCardNo";
            this.lCardNo.Size = new System.Drawing.Size(96, 16);
            this.lCardNo.TabIndex = 59;
            this.lCardNo.Text = "Cheque / Card No";
            // 
            // gbReference
            // 
            this.gbReference.BackColor = System.Drawing.SystemColors.Control;
            this.gbReference.Controls.Add(this.btnSellValidate);
            this.gbReference.Controls.Add(this.txtSellReference);
            this.gbReference.Location = new System.Drawing.Point(8, 8);
            this.gbReference.Name = "gbReference";
            this.gbReference.Size = new System.Drawing.Size(254, 56);
            this.gbReference.TabIndex = 3;
            this.gbReference.TabStop = false;
            this.gbReference.Text = "Reference";
            // 
            // btnSellValidate
            // 
            this.btnSellValidate.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSellValidate.BackgroundImage")));
            this.btnSellValidate.Location = new System.Drawing.Point(176, 24);
            this.btnSellValidate.Name = "btnSellValidate";
            this.btnSellValidate.Size = new System.Drawing.Size(16, 16);
            this.btnSellValidate.TabIndex = 1;
            this.btnSellValidate.Click += new System.EventHandler(this.btnSellValidate_Click);
            // 
            // txtSellReference
            // 
            this.txtSellReference.Location = new System.Drawing.Point(16, 24);
            this.txtSellReference.MaxLength = 32;
            this.txtSellReference.Name = "txtSellReference";
            this.txtSellReference.Size = new System.Drawing.Size(128, 23);
            this.txtSellReference.TabIndex = 0;
            // 
            // gbSellDetails
            // 
            this.gbSellDetails.BackColor = System.Drawing.SystemColors.Control;
            this.gbSellDetails.Controls.Add(this.chxFree);
            this.gbSellDetails.Controls.Add(this.label3);
            this.gbSellDetails.Controls.Add(this.label4);
            this.gbSellDetails.Controls.Add(this.txtSellValue);
            this.gbSellDetails.Controls.Add(this.dtSellExpiryDate);
            this.gbSellDetails.Enabled = false;
            this.gbSellDetails.Location = new System.Drawing.Point(8, 64);
            this.gbSellDetails.Name = "gbSellDetails";
            this.gbSellDetails.Size = new System.Drawing.Size(254, 112);
            this.gbSellDetails.TabIndex = 4;
            this.gbSellDetails.TabStop = false;
            this.gbSellDetails.Text = "Details";
            // 
            // chxFree
            // 
            this.chxFree.Enabled = false;
            this.chxFree.Location = new System.Drawing.Point(176, 32);
            this.chxFree.Name = "chxFree";
            this.chxFree.Size = new System.Drawing.Size(48, 24);
            this.chxFree.TabIndex = 5;
            this.chxFree.Text = "Free";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Expiry Date";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 16);
            this.label4.TabIndex = 2;
            this.label4.Text = "Voucher Value";
            // 
            // txtSellValue
            // 
            this.txtSellValue.Location = new System.Drawing.Point(16, 32);
            this.txtSellValue.Name = "txtSellValue";
            this.txtSellValue.Size = new System.Drawing.Size(128, 23);
            this.txtSellValue.TabIndex = 1;
            this.txtSellValue.Leave += new System.EventHandler(this.txtSellValue_Leave);
            // 
            // dtSellExpiryDate
            // 
            this.dtSellExpiryDate.Location = new System.Drawing.Point(16, 72);
            this.dtSellExpiryDate.Name = "dtSellExpiryDate";
            this.dtSellExpiryDate.Size = new System.Drawing.Size(128, 23);
            this.dtSellExpiryDate.TabIndex = 0;
            // 
            // tpRedeem
            // 
            this.tpRedeem.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tpRedeem.Controls.Add(this.btnCancelRedeem);
            this.tpRedeem.Controls.Add(this.btnRedeem);
            this.tpRedeem.Controls.Add(this.gbRedeemType);
            this.tpRedeem.Controls.Add(this.gbRedeemRef);
            this.tpRedeem.Controls.Add(this.gbRedeemDetails);
            this.tpRedeem.Location = new System.Drawing.Point(0, 25);
            this.tpRedeem.Name = "tpRedeem";
            this.tpRedeem.Size = new System.Drawing.Size(280, 415);
            this.tpRedeem.TabIndex = 4;
            this.tpRedeem.Title = "Redeem";
            // 
            // btnCancelRedeem
            // 
            this.btnCancelRedeem.Location = new System.Drawing.Point(150, 360);
            this.btnCancelRedeem.Name = "btnCancelRedeem";
            this.btnCancelRedeem.Size = new System.Drawing.Size(62, 24);
            this.btnCancelRedeem.TabIndex = 8;
            this.btnCancelRedeem.Text = "Cancel";
            this.btnCancelRedeem.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRedeem
            // 
            this.btnRedeem.Location = new System.Drawing.Point(78, 360);
            this.btnRedeem.Name = "btnRedeem";
            this.btnRedeem.Size = new System.Drawing.Size(48, 24);
            this.btnRedeem.TabIndex = 7;
            this.btnRedeem.Text = "OK";
            this.btnRedeem.Click += new System.EventHandler(this.btnRedeem_Click);
            // 
            // GiftVoucher
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(282, 442);
            this.Controls.Add(this.tcMode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GiftVoucher";
            this.Text = "Gift Voucher";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.GiftVoucher_Closing);
            this.gbRedeemType.ResumeLayout(false);
            this.gbRedeemRef.ResumeLayout(false);
            this.gbRedeemRef.PerformLayout();
            this.gbRedeemDetails.ResumeLayout(false);
            this.gbRedeemDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errors)).EndInit();
            this.tcMode.ResumeLayout(false);
            this.tpSell.ResumeLayout(false);
            this.gbSellPayment.ResumeLayout(false);
            this.gbSellPayment.PerformLayout();
            this.gbReference.ResumeLayout(false);
            this.gbReference.PerformLayout();
            this.gbSellDetails.ResumeLayout(false);
            this.gbSellDetails.PerformLayout();
            this.tpRedeem.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void btnRedeemValidate_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				voucherValue = 0;
				DateTime expiryDate;
				bool redeemed = false;

				/* make sure that the reference number entered related to an extant gift voucher */
				/* unless this is a sale, in which case make sure it doesn't exist */
				PaymentManager.ValidateGiftVoucher(txtRedeemReference.Text, rbCourts.Checked, true, out voucherValue, out expiryDate, out redeemed, out Error);
				if(Error.Length>0)
					ShowError(Error);
				else
				{
					/* if found and not redeemed populate details */
					if(rbCourts.Checked)
					{
						if( voucherValue == 0 || redeemed )
						{
							errors.SetError(txtRedeemReference, "Voucher not found or redeemed");						
							txtRedeemReference.Focus();
							txtRedeemReference.Select(0, txtRedeemReference.Text.Length);
						}
						else
						{
							if(FormParent is NewAccount)
							{
								((NewAccount)FormParent).GiftVoucherValue = voucherValue;
								((NewAccount)FormParent).CourtsVoucher = rbCourts.Checked;
								((NewAccount)FormParent).VoucherReference = txtRedeemReference.Text;
							}

							if(FormParent is PaymentList)
							{
								((PaymentList)FormParent).GiftVoucherValue = voucherValue;
								((PaymentList)FormParent).CourtsVoucher = rbCourts.Checked;
								((PaymentList)FormParent).VoucherReference = txtRedeemReference.Text;
							}

							txtRedeemValue.Text = voucherValue.ToString(DecimalPlaces);
							errors.SetError(txtRedeemReference, "");
							dtRedeemExpiryDate.Value = expiryDate;

							if(expiryDate < DateTime.Today)
							{
								/* ask for authorisation if voucher has expired */
								AuthorisePrompt auth = new AuthorisePrompt(this, authExpiredVoucher, GetResource("M_EXPIREDVOUCHERDAUTH"));
								auth.ShowDialog();

								if(!auth.Authorised)
								{
									if(FormParent is NewAccount)
										((NewAccount)FormParent).chxGiftVoucher.Checked = false;
									if(FormParent is PaymentList)
										((PaymentList)FormParent).GiftVoucherValue = 0;
								}
								else
								{
									if(FormParent is NewAccount)
										((NewAccount)FormParent).VoucherAuthorisedBy = auth.AuthorisedBy;
									if(FormParent is PaymentList)
										((PaymentList)FormParent).VoucherAuthorisedBy = auth.AuthorisedBy;
								}														
							}
						}
					}
					else		/* not courts */
					{
						if( redeemed )
						{
							errors.SetError(txtRedeemReference, "Voucher redeemed");						
							txtRedeemReference.Focus();
							txtRedeemReference.Select(0, txtRedeemReference.Text.Length);
						}
						else
						{
							this.txtRedeemValue.ReadOnly = false;
							txtRedeemValue.Text = voucherValue.ToString(DecimalPlaces);
							errors.SetError(txtRedeemReference, "");					
						}
					}
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "btnRedeemValidate_Click");
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

				if(StaticData.Tables[TN.GiftVoucherOther]==null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.GiftVoucherOther, new string[]{"VCCO", "L"}));
				if (StaticData.Tables[TN.PayMethod] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.PayMethod, new string[]{CAT.FintransPayMethod, "L"}));
				if (StaticData.Tables[TN.CreditCard] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.CreditCard, new string[]{CAT.CreditCardType, "L"}));
				if (StaticData.Tables[TN.Bank] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.Bank, null));
				
				if(dropDowns.DocumentElement.ChildNodes.Count>0)
				{
					DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
					if(Error.Length>0)
						ShowError(Error);
					else
					{
						foreach(DataTable dt in ds.Tables)
							StaticData.Tables[dt.TableName] = dt;
					}
				}

				drpCompany.DataSource = (DataTable)StaticData.Tables[TN.GiftVoucherOther];;
				drpCompany.DisplayMember = CN.CodeDescription;

				// Take a copy of the PayMethod table and delete the Not Applicable
				// method
				DataTable dtPayMethod = ((DataTable)StaticData.Tables[TN.PayMethod]).Copy();
				for (int i = 0; i < dtPayMethod.Rows.Count; i++)
				{
					if (((string)dtPayMethod.Rows[i][CN.Code]).Trim() == "0") // Exclude "Not Applicable"

					{
						dtPayMethod.Rows[i].Delete();
						break;
					}	
				}

				//dtPayMethod.DefaultView.RowFilter = CN.Code + " not = '"+PayMethod.GiftVoucher+"'";	/* exclude gift vouchers - don't want to complicate things*/
				dtPayMethod.DefaultView.RowFilter = "(" + CN.Code + " not in (8,218,228,518) AND " +
					"(" + CN.Code + " not in (100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127," +
					"128,129,130,131,132,133,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,153,154,155,156,157,158,159,160,161,162," +
					"163,164,165,166,167,168,169,170,171,172,173,174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198," +
					"199,200,521,531,502,503,513,582,551,561,532,592,591,541,512,522,571,581,542,552,507,508,221,222,223,241,242,231,232,261,243,271,281)))";
				//	"'" + PayMethod.CashFrenchFrancs + "',"+
				//	"'" + PayMethod.CashUKSterling + "',"+
				//	"'" + PayMethod.CashUSDollars + "')";
																		
				drpPayMethod.DataSource = dtPayMethod.DefaultView;
				drpPayMethod.DisplayMember = CN.CodeDescription;

				drpCardType.DataSource = (DataTable)StaticData.Tables[TN.CreditCard];
				drpCardType.DisplayMember = CN.CodeDescription;

				drpBank.DataSource = (DataTable)StaticData.Tables[TN.Bank];
				drpBank.DisplayMember = CN.BankName;
			}
			catch(Exception ex)
			{
				Catch(ex, "LoadStaticData");
			}
			finally
			{
				StopWait();
			}
		}

		private void rbCourts_CheckedChanged(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				drpCompany.Enabled = !rbCourts.Checked;

				if(!this.txtRedeemValue.ReadOnly)
					this.txtRedeemValue.ReadOnly = rbCourts.Checked;
			}
			catch(Exception ex)
			{
				Catch(ex, "rbCourts_CheckedChanged");
			}
			finally
			{
				StopWait();
			}
		}

		private void btnSell_Click(object sender, System.EventArgs e)
		{
			bool valid = true;
			try
			{
				Wait();

				/* make sure they've entered a value */
				if(txtSellValue.Text.Length==0)
				{
					errors.SetError(txtSellValue, GetResource("M_ENTERMANDATORY"));
					txtSellValue.Select(0, txtSellValue.Text.Length);
					txtSellValue.Focus();
					valid = false;
				}
				else
					errors.SetError(txtSellValue,"");

				/* make sure they've chosen an expiry date in the future */
				if(dtSellExpiryDate.Value <= DateTime.Today)
				{
					errors.SetError(dtSellExpiryDate, GetResource("M_EXPIRYPAST"));
					valid = false;
				}
				else
					errors.SetError(dtSellExpiryDate,"");

				if (this.gbSellPayment.Enabled)
				{
					// The pay method must be entered
					short curPayMethod = Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code].ToString());
					if (curPayMethod == 0)
					{
						errors.SetError(drpPayMethod, GetResource("M_REQUIREPAYMETHOD"));
						valid = false;
					}
					else
						errors.SetError(drpPayMethod,"");

					// When paying by card the card type must be entered
					if (   this.drpCardType.SelectedIndex == 0
						&& (PayMethod.IsPayMethod(curPayMethod, PayMethod.CreditCard) || PayMethod.IsPayMethod(curPayMethod, PayMethod.DebitCard)))
					{
						errors.SetError(drpCardType, GetResource("M_REQUIRECARDTYPE"));
						valid = false;
					}
					else
						errors.SetError(drpCardType,"");

					// When paying by cheque/card the cheque/card number must be entered
					if (   this.txtCardNo.Text.Trim().Length == 0
						&& (   PayMethod.IsPayMethod(curPayMethod, PayMethod.Cheque)
						|| PayMethod.IsPayMethod(curPayMethod, PayMethod.CreditCard)
						|| PayMethod.IsPayMethod(curPayMethod, PayMethod.DebitCard)))
					{
						errors.SetError(txtCardNo, GetResource("M_REQUIRECHEQUENO"));
						valid = false;
					}
					else
						errors.SetError(txtCardNo,"");
				}

				if(valid)
				{
					// Record the sold amount for the calling screen to interrogate
					this._soldAmount = Convert.ToDecimal(StripCurrency(txtSellValue.Text));

					PaymentManager.SellGiftVoucher(txtSellReference.Text,
						soldAmount,
						dtSellExpiryDate.Value,
						Config.CountryCode,
						(string)((DataRowView)drpBank.SelectedItem)[CN.BankCode],
						txtBankAcctNo.Text,
						txtCardNo.Text,
						Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code]),
						Convert.ToInt16(Config.BranchCode),
						chxFree.Checked,
						this._privilegeClub,
						this._accountSet,
						out Error);
					if(Error.Length > 0)
					{
						this._soldAmount = 0;
						ShowError(Error);
					}
					else
						Close();
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "btnSell_Click");
			}
			finally
			{
				StopWait();
			}
		}

		private void txtSellValue_Leave(object sender, System.EventArgs e)
		{
			try
			{
				decimal val = Convert.ToDecimal(StripCurrency(txtSellValue.Text));				
				txtSellValue.Text = val.ToString(DecimalPlaces);
				errors.SetError(txtSellValue, "");
			}
			catch(Exception ex)
			{
				txtSellValue.Focus();
				txtSellValue.Select(0, txtSellValue.Text.Length);
				errors.SetError(txtSellValue, ex.Message);
			}
		}

		private void btnSellValidate_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				voucherValue = 0;
				DateTime expiryDate;
				bool redeemed = false;

				/* make sure that the reference number entered related to an extant gift voucher */
				/* unless this is a sale, in which case make sure it doesn't exist */
				PaymentManager.ValidateGiftVoucher(txtSellReference.Text, true, true, out voucherValue, out expiryDate, out redeemed, out Error);
				if(Error.Length>0)
					ShowError(Error);
				else
				{
					if(voucherValue != 0)
					{
						errors.SetError(txtSellReference, GetResource("M_VOUCHEREXISTS"));						
						txtSellReference.Focus();
						txtSellReference.Select(0, txtSellReference.Text.Length);
					}
					else
					{
						errors.SetError(txtSellReference, "");	
						gbSellDetails.Enabled = true;
						gbSellPayment.Enabled = !this._privilegeClub;
					}
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "btnSellValidate_Click");
			}
			finally
			{
				StopWait();
			}
		}

		private int _lastPayMethod = 0;

		private void drpPayMethod_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				// Make sure this event has been fired when it is useful
				if (drpPayMethod.SelectedIndex >= 0 &&
					drpPayMethod.SelectedIndex != this._lastPayMethod)
				{	
					this._lastPayMethod = drpPayMethod.SelectedIndex;

					int curPayMethod = Convert.ToInt16(((DataRowView)drpPayMethod.SelectedItem)[CN.Code].ToString());

					bool payByEntered = (curPayMethod != 0);
					// For cash payments disable the bank details and display the change field
					bool payByCash = (PayMethod.IsPayMethod(curPayMethod, PayMethod.Cash));
					// When paying by cheque enable the Print Account Number button
					bool payByCheque = (PayMethod.IsPayMethod(curPayMethod, PayMethod.Cheque));
					// When paying with a card enable the Card Type
					bool payByCard = (PayMethod.IsPayMethod(curPayMethod, PayMethod.CreditCard) || PayMethod.IsPayMethod(curPayMethod, PayMethod.DebitCard));

					this.txtCardNo.Enabled		= !payByCash && payByEntered;
					this.drpBank.Enabled		= !payByCash && payByEntered;
					this.txtBankAcctNo.Enabled	= !payByCash && payByEntered;
					this.drpCardType.Enabled	= payByCard;
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "drpPayMethod_SelectedIndexChanged");
			}
			finally
			{
				StopWait();
			}
		}

		private void GiftVoucher_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				bool status = true;
				Wait();

				if(!_cancel)
				{
					if(!Sale)
					{
						if(rbOther.Checked)
						{
							/* validate that a value has been entered*/
							if(txtRedeemValue.Text.Length==0 ||
								!IsStrictMoney(txtRedeemValue.Text))
							{
								errors.SetError(txtRedeemValue, GetResource("M_NONNUMERIC"));
								status = false;
							}
							else
							{							
								errors.SetError(txtRedeemValue, "");
							}

							/* validate the expiry date that's been entered */
							if(dtRedeemExpiryDate.Value < DateTime.Today)
							{
								errors.SetError(dtRedeemExpiryDate, GetResource("M_EXPIRYPAST"));
								status = false;
							}
							else
								errors.SetError(dtRedeemExpiryDate, "");

							if(status)
							{
								if(FormParent is PaymentList)
								{
									((PaymentList)FormParent).GiftVoucherValue = Convert.ToDecimal(StripCurrency(txtRedeemValue.Text));
									((PaymentList)FormParent).CourtsVoucher = rbCourts.Checked;
									((PaymentList)FormParent).VoucherReference = txtRedeemReference.Text;
									((PaymentList)FormParent).VoucherCompanyAcctNo = (string)((DataRowView)drpCompany.SelectedItem)[CN.Code];
								}

								if(FormParent is NewAccount)
								{
									((NewAccount)FormParent).GiftVoucherValue = Convert.ToDecimal(StripCurrency(txtRedeemValue.Text));
									((NewAccount)FormParent).CourtsVoucher = rbCourts.Checked;
									((NewAccount)FormParent).VoucherReference = txtRedeemReference.Text;
									((NewAccount)FormParent).VoucherCompanyAcctNo = (string)((DataRowView)drpCompany.SelectedItem)[CN.Code];
								}

								/* save the voucher details to the giftvoucherother table */
								PaymentManager.RedeemOtherGiftVoucher(	txtRedeemReference.Text,
									(string)((DataRowView)drpCompany.SelectedItem)[CN.Code],
									Convert.ToDecimal(StripCurrency(txtRedeemValue.Text)),
									out Error );	
								if(Error.Length > 0)
								{
									status = false;
									ShowError(Error);
								}
							}

							/* if there were errors ask them if they want to correct them or abandon */
							if(!status)
							{
								if(DialogResult.Retry == ShowInfo("M_ERRORSRETRY", MessageBoxButtons.RetryCancel))
									e.Cancel = true;
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "GiftVoucher_Closing");
			}
			finally
			{
				StopWait();
			}
		}

		private void tcMode_SelectionChanged(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				if(Sale && tcMode.SelectedTab == tpRedeem)
					tcMode.SelectedTab = tpSell;
				if(!Sale && tcMode.SelectedTab == tpSell)
					tcMode.SelectedTab = tpRedeem;
			}
			catch(Exception ex)
			{
				Catch(ex, "tcMode_SelectionChanged");
			}
			finally
			{
				StopWait();
			}
		}

		private bool _cancel = false;

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				_cancel = true;

				if(FormParent is NewAccount)
				{
					((NewAccount)FormParent).GiftVoucherValue = 0;
					((NewAccount)FormParent).VoucherReference = "";
					((NewAccount)FormParent).chxGiftVoucher.Checked = false;
				}

				if(FormParent is PaymentList)
				{
					((PaymentList)FormParent).GiftVoucherValue = 0;
					((PaymentList)FormParent).VoucherReference = "";
				}
				Close();
			}
			catch(Exception ex)
			{
				Catch(ex, "btnTotal_Click");
			}
			finally
			{
				StopWait();
			}
		}

		private void btnRedeem_Click(object sender, System.EventArgs e)
		{
			Close();
		}
	}
}
