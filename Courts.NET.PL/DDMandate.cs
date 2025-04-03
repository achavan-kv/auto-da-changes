using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.Giro;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using System.Xml;
using Crownwood.Magic.Menus;




namespace STL.PL
{
	/// <summary>
	/// Direct Debit Mandate maintenance.
	/// This form can be opened in three ways:
	/// 1) Blank with all fields disabled except the Account No. The user
	/// enters an Account No to retrieve details. If no mandate exists then
	/// just the Customer Name and the Instalment Amount will be shown.
	/// 2) An Account No may be passed in which will retrieve the mandate
	/// for the account if one exists. Otherwise just the Customer Name
	/// and the Instalment Amount will be retrieved.
	/// 3) A Mandate Id may be pased in to retrieve the the full mandate
	/// details.
	/// </summary>
	public class DDMandate : CommonForm
	{
        private new string Error = "";

		private System.Windows.Forms.GroupBox grpCreditOfficerOnly;
		private System.Windows.Forms.Label lReason;
		private System.Windows.Forms.Label lTermination;
		private System.Windows.Forms.Label lCommencement;
		private System.Windows.Forms.Label lDateSubmitted;
		private System.Windows.Forms.Label lDateApproved;
		private System.Windows.Forms.Label lDateReturned;
		private System.Windows.Forms.Label lDateReceived;
		private System.Windows.Forms.DateTimePicker dtReceived;
		private System.Windows.Forms.DateTimePicker dtReturned;
		private System.Windows.Forms.DateTimePicker dtSubmitted;
		private System.Windows.Forms.DateTimePicker dtApproved;
		private System.Windows.Forms.DateTimePicker dtCommencement;
		private System.Windows.Forms.DateTimePicker dtTermination;
		private System.Windows.Forms.Button btnReturn;
		private System.Windows.Forms.GroupBox grpComments;
		private System.Windows.Forms.TextBox txtComment;
		private System.Windows.Forms.Label lActiveText;
		private System.Windows.Forms.GroupBox grpMandate;
		private System.Windows.Forms.TextBox txtBankAcctName;
		private System.Windows.Forms.TextBox txtBankAcctNo;
		private System.Windows.Forms.ComboBox drpBank;
		private System.Windows.Forms.ComboBox drpDueDay;
		private System.Windows.Forms.TextBox txtCustomerName;
		private System.Windows.Forms.Label lBankAcctName;
		private System.Windows.Forms.Label lBankAcctNo;
		private System.Windows.Forms.Label lBank;
		private System.Windows.Forms.Label lDueDay;
		private System.Windows.Forms.Label lCustomerName;
		private System.Windows.Forms.Label lAccountNo;
		private STL.PL.AccountTextBox txtAccountNo;
		private System.Windows.Forms.Label lCancelReason;
		private System.Windows.Forms.TextBox txtRejectCount;
		private System.Windows.Forms.Label lRejectCount;
		private System.Windows.Forms.TextBox txtBankBranchNo;
		private System.Windows.Forms.TextBox txtInstalAmount;
		private System.Windows.Forms.Label lInstalAmount;
		private System.Windows.Forms.Label lBankBranchNo;

		private System.Windows.Forms.Button btnEdit;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Button btnSave;
		//private Crownwood.Magic.Menus.MenuCommand menuFile;

		//
		// BL details for this form
		//
		private DataSet _mandateSet = null;
		private DataRow _mandateDetails = null;
		private DataSet _giroDateSet = null;
		private DataRow _giroDates = null;

		private bool _formEditMode = false;
		private bool _acctLoaded = false;
		private string _lastAccountNo = "";
		private string _localBankAcctNo = "";
		private string _localBankCode = "";
		private string _localAccountNo = "";
		private System.Windows.Forms.ToolTip toolTip1;
		private System.ComponentModel.IContainer components;

		//
		// Constructors
		//
        //public DDMandate(TranslationDummy d)
        //{
        //    InitializeComponent();
        //    menuMain = new Crownwood.Magic.Menus.MenuControl();
        //    menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
        //}

		public DDMandate(Form root, Form parent)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Set up
			FormRoot = root;
			FormParent = parent;
			this.Init();
		}

		/// <summary>
		/// This overload is for invocation from another form.
		/// When called from Sanction Stage 1 Bank Details the Mandate Id
		/// will be passed in if there is an existing Mandate. Otherwise the
		/// Account No will be used. The customer Bank Account No and Bank Code
		/// can be passed in from the bank details. When invoked in this way
		/// a new mandate should be opened in edit mode.
		/// </summary>
		/// <param name="root"></param>
		/// <param name="parent"></param>
		/// <param name="piMandateId"></param>
		/// <param name="piAccountNo"></param>
		/// <param name="piBankAcctNo"></param>
		/// <param name="piBankCode"></param>
		/// <param name="piEditMode"></param>
		public DDMandate(Form root, Form parent,
			int piMandateId, string piAccountNo,
			string piBankAcctNo, string piBankCode,
			bool piEditMode)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Set up
			FormRoot = root;
			FormParent = parent;
			this.Init();
			// Supplied Account No will be redisplayed if the form is cleared
			this._localAccountNo = piAccountNo;
			// Supplied bank details will be copied to their fields when
			// the form is put into edit mode.
			this._localBankAcctNo = piBankAcctNo;
			this._localBankCode = piBankCode;

			// Load data if Mandate Id or Account No supplied
			if (piMandateId != 0 || piAccountNo.Trim() != "")
			{
				this.LoadDetails(piMandateId, piAccountNo);
				if (piEditMode && !(bool)_mandateDetails[CN.Loaded])
					// Start in Edit mode for a new mandate
					this.SetEditMode(true);
				else
				{
					// Edit mode not requested and the mandate has been loaded
					this.SetEditMode(false);
					this.txtAccountNo.Enabled = false;
				}
			}

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DDMandate));
			this.grpCreditOfficerOnly = new System.Windows.Forms.GroupBox();
			this.lActiveText = new System.Windows.Forms.Label();
			this.btnReturn = new System.Windows.Forms.Button();
			this.lCancelReason = new System.Windows.Forms.Label();
			this.dtTermination = new System.Windows.Forms.DateTimePicker();
			this.dtCommencement = new System.Windows.Forms.DateTimePicker();
			this.dtApproved = new System.Windows.Forms.DateTimePicker();
			this.dtSubmitted = new System.Windows.Forms.DateTimePicker();
			this.dtReturned = new System.Windows.Forms.DateTimePicker();
			this.dtReceived = new System.Windows.Forms.DateTimePicker();
			this.lReason = new System.Windows.Forms.Label();
			this.lTermination = new System.Windows.Forms.Label();
			this.lCommencement = new System.Windows.Forms.Label();
			this.lDateSubmitted = new System.Windows.Forms.Label();
			this.lDateApproved = new System.Windows.Forms.Label();
			this.lDateReturned = new System.Windows.Forms.Label();
			this.lDateReceived = new System.Windows.Forms.Label();
			this.grpComments = new System.Windows.Forms.GroupBox();
			this.txtComment = new System.Windows.Forms.TextBox();
			this.grpMandate = new System.Windows.Forms.GroupBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnClear = new System.Windows.Forms.Button();
			this.btnEdit = new System.Windows.Forms.Button();
			this.txtAccountNo = new STL.PL.AccountTextBox();
			this.lAccountNo = new System.Windows.Forms.Label();
			this.txtRejectCount = new System.Windows.Forms.TextBox();
			this.lRejectCount = new System.Windows.Forms.Label();
			this.txtBankAcctName = new System.Windows.Forms.TextBox();
			this.txtBankBranchNo = new System.Windows.Forms.TextBox();
			this.txtBankAcctNo = new System.Windows.Forms.TextBox();
			this.drpBank = new System.Windows.Forms.ComboBox();
			this.drpDueDay = new System.Windows.Forms.ComboBox();
			this.txtInstalAmount = new System.Windows.Forms.TextBox();
			this.txtCustomerName = new System.Windows.Forms.TextBox();
			this.lInstalAmount = new System.Windows.Forms.Label();
			this.lBankAcctName = new System.Windows.Forms.Label();
			this.lBankAcctNo = new System.Windows.Forms.Label();
			this.lBankBranchNo = new System.Windows.Forms.Label();
			this.lBank = new System.Windows.Forms.Label();
			this.lDueDay = new System.Windows.Forms.Label();
			this.lCustomerName = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.grpCreditOfficerOnly.SuspendLayout();
			this.grpComments.SuspendLayout();
			this.grpMandate.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpCreditOfficerOnly
			// 
			this.grpCreditOfficerOnly.BackColor = System.Drawing.SystemColors.Control;
			this.grpCreditOfficerOnly.Controls.AddRange(new System.Windows.Forms.Control[] {
																							   this.lActiveText,
																							   this.btnReturn,
																							   this.lCancelReason,
																							   this.dtTermination,
																							   this.dtCommencement,
																							   this.dtApproved,
																							   this.dtSubmitted,
																							   this.dtReturned,
																							   this.dtReceived,
																							   this.lReason,
																							   this.lTermination,
																							   this.lCommencement,
																							   this.lDateSubmitted,
																							   this.lDateApproved,
																							   this.lDateReturned,
																							   this.lDateReceived});
			this.grpCreditOfficerOnly.Location = new System.Drawing.Point(416, 0);
			this.grpCreditOfficerOnly.Name = "grpCreditOfficerOnly";
			this.grpCreditOfficerOnly.Size = new System.Drawing.Size(368, 280);
			this.grpCreditOfficerOnly.TabIndex = 25;
			this.grpCreditOfficerOnly.TabStop = false;
			this.grpCreditOfficerOnly.Text = "Credit officer only";
			// 
			// lActiveText
			// 
			this.lActiveText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lActiveText.ForeColor = System.Drawing.SystemColors.ActiveCaption;
			this.lActiveText.Location = new System.Drawing.Point(96, 232);
			this.lActiveText.Name = "lActiveText";
			this.lActiveText.Size = new System.Drawing.Size(152, 24);
			this.lActiveText.TabIndex = 0;
			this.lActiveText.Text = "Active text";
			this.lActiveText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnReturn
			// 
			this.btnReturn.Enabled = false;
			this.btnReturn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnReturn.Location = new System.Drawing.Point(264, 56);
			this.btnReturn.Name = "btnReturn";
			this.btnReturn.Size = new System.Drawing.Size(72, 40);
			this.btnReturn.TabIndex = 100;
			this.btnReturn.Text = "Return to customer";
			this.btnReturn.Click += new System.EventHandler(this.btnReturn_Click);
			// 
			// lCancelReason
			// 
			this.lCancelReason.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lCancelReason.Location = new System.Drawing.Point(112, 200);
			this.lCancelReason.Name = "lCancelReason";
			this.lCancelReason.Size = new System.Drawing.Size(240, 16);
			this.lCancelReason.TabIndex = 0;
			this.lCancelReason.Text = "Reason text";
			this.lCancelReason.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// dtTermination
			// 
			this.dtTermination.CustomFormat = "ddd dd MMM yyyy";
			this.dtTermination.Enabled = false;
			this.dtTermination.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtTermination.Location = new System.Drawing.Point(112, 160);
			this.dtTermination.Name = "dtTermination";
			this.dtTermination.Size = new System.Drawing.Size(120, 20);
			this.dtTermination.TabIndex = 140;
			// 
			// dtCommencement
			// 
			this.dtCommencement.CustomFormat = "ddd dd MMM yyyy";
			this.dtCommencement.Enabled = false;
			this.dtCommencement.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtCommencement.Location = new System.Drawing.Point(112, 136);
			this.dtCommencement.Name = "dtCommencement";
			this.dtCommencement.Size = new System.Drawing.Size(120, 20);
			this.dtCommencement.TabIndex = 130;
			this.dtCommencement.Leave += new System.EventHandler(this.dtCommencement_Leave);
			// 
			// dtApproved
			// 
			this.dtApproved.CustomFormat = "ddd dd MMM yyyy";
			this.dtApproved.Enabled = false;
			this.dtApproved.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtApproved.Location = new System.Drawing.Point(112, 112);
			this.dtApproved.Name = "dtApproved";
			this.dtApproved.Size = new System.Drawing.Size(120, 20);
			this.dtApproved.TabIndex = 120;
			this.dtApproved.Leave += new System.EventHandler(this.dtApproved_Leave);
			// 
			// dtSubmitted
			// 
			this.dtSubmitted.CustomFormat = "ddd dd MMM yyyy";
			this.dtSubmitted.Enabled = false;
			this.dtSubmitted.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtSubmitted.Location = new System.Drawing.Point(112, 88);
			this.dtSubmitted.Name = "dtSubmitted";
			this.dtSubmitted.Size = new System.Drawing.Size(120, 20);
			this.dtSubmitted.TabIndex = 110;
			this.dtSubmitted.Leave += new System.EventHandler(this.dtSubmitted_Leave);
			// 
			// dtReturned
			// 
			this.dtReturned.CustomFormat = "ddd dd MMM yyyy";
			this.dtReturned.Enabled = false;
			this.dtReturned.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtReturned.Location = new System.Drawing.Point(112, 64);
			this.dtReturned.Name = "dtReturned";
			this.dtReturned.Size = new System.Drawing.Size(120, 20);
			this.dtReturned.TabIndex = 0;
			this.dtReturned.TabStop = false;
			// 
			// dtReceived
			// 
			this.dtReceived.CustomFormat = "ddd dd MMM yyyy";
			this.dtReceived.Enabled = false;
			this.dtReceived.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtReceived.Location = new System.Drawing.Point(112, 40);
			this.dtReceived.Name = "dtReceived";
			this.dtReceived.Size = new System.Drawing.Size(120, 20);
			this.dtReceived.TabIndex = 90;
			this.dtReceived.Leave += new System.EventHandler(this.dtReceived_Leave);
			// 
			// lReason
			// 
			this.lReason.Location = new System.Drawing.Point(24, 200);
			this.lReason.Name = "lReason";
			this.lReason.Size = new System.Drawing.Size(80, 16);
			this.lReason.TabIndex = 0;
			this.lReason.Text = "Reason";
			this.lReason.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lTermination
			// 
			this.lTermination.Location = new System.Drawing.Point(24, 160);
			this.lTermination.Name = "lTermination";
			this.lTermination.Size = new System.Drawing.Size(80, 16);
			this.lTermination.TabIndex = 0;
			this.lTermination.Text = "Termination";
			this.lTermination.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lCommencement
			// 
			this.lCommencement.Location = new System.Drawing.Point(24, 136);
			this.lCommencement.Name = "lCommencement";
			this.lCommencement.Size = new System.Drawing.Size(88, 16);
			this.lCommencement.TabIndex = 0;
			this.lCommencement.Text = "Commencement";
			this.lCommencement.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lDateSubmitted
			// 
			this.lDateSubmitted.Location = new System.Drawing.Point(24, 88);
			this.lDateSubmitted.Name = "lDateSubmitted";
			this.lDateSubmitted.Size = new System.Drawing.Size(80, 16);
			this.lDateSubmitted.TabIndex = 0;
			this.lDateSubmitted.Text = "Date submitted";
			this.lDateSubmitted.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lDateApproved
			// 
			this.lDateApproved.Location = new System.Drawing.Point(24, 112);
			this.lDateApproved.Name = "lDateApproved";
			this.lDateApproved.Size = new System.Drawing.Size(80, 16);
			this.lDateApproved.TabIndex = 0;
			this.lDateApproved.Text = "Date approved";
			this.lDateApproved.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lDateReturned
			// 
			this.lDateReturned.Location = new System.Drawing.Point(24, 64);
			this.lDateReturned.Name = "lDateReturned";
			this.lDateReturned.Size = new System.Drawing.Size(80, 16);
			this.lDateReturned.TabIndex = 0;
			this.lDateReturned.Text = "Date returned";
			this.lDateReturned.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lDateReceived
			// 
			this.lDateReceived.Location = new System.Drawing.Point(24, 40);
			this.lDateReceived.Name = "lDateReceived";
			this.lDateReceived.Size = new System.Drawing.Size(80, 16);
			this.lDateReceived.TabIndex = 0;
			this.lDateReceived.Text = "Date received";
			this.lDateReceived.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// grpComments
			// 
			this.grpComments.BackColor = System.Drawing.SystemColors.Control;
			this.grpComments.Controls.AddRange(new System.Windows.Forms.Control[] {
																					  this.txtComment});
			this.grpComments.Location = new System.Drawing.Point(8, 280);
			this.grpComments.Name = "grpComments";
			this.grpComments.Size = new System.Drawing.Size(776, 192);
			this.grpComments.TabIndex = 67;
			this.grpComments.TabStop = false;
			this.grpComments.Text = "Comments";
			// 
			// txtComment
			// 
			this.txtComment.BackColor = System.Drawing.SystemColors.Window;
			this.txtComment.Enabled = false;
			this.txtComment.Location = new System.Drawing.Point(16, 24);
			this.txtComment.MaxLength = 200;
			this.txtComment.Multiline = true;
			this.txtComment.Name = "txtComment";
			this.txtComment.Size = new System.Drawing.Size(744, 152);
			this.txtComment.TabIndex = 150;
			this.txtComment.Text = "";
			// 
			// grpMandate
			// 
			this.grpMandate.BackColor = System.Drawing.SystemColors.Control;
			this.grpMandate.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.btnSave,
																					 this.btnClear,
																					 this.btnEdit,
																					 this.txtAccountNo,
																					 this.lAccountNo,
																					 this.txtRejectCount,
																					 this.lRejectCount,
																					 this.txtBankAcctName,
																					 this.txtBankBranchNo,
																					 this.txtBankAcctNo,
																					 this.drpBank,
																					 this.drpDueDay,
																					 this.txtInstalAmount,
																					 this.txtCustomerName,
																					 this.lInstalAmount,
																					 this.lBankAcctName,
																					 this.lBankAcctNo,
																					 this.lBankBranchNo,
																					 this.lBank,
																					 this.lDueDay,
																					 this.lCustomerName});
			this.grpMandate.Location = new System.Drawing.Point(8, 0);
			this.grpMandate.Name = "grpMandate";
			this.grpMandate.Size = new System.Drawing.Size(408, 280);
			this.grpMandate.TabIndex = 70;
			this.grpMandate.TabStop = false;
			this.grpMandate.Text = "Mandate";
			// 
			// btnSave
			// 
			this.btnSave.BackgroundImage = ((System.Drawing.Bitmap)(resources.GetObject("btnSave.BackgroundImage")));
			this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnSave.Location = new System.Drawing.Point(368, 16);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(24, 24);
			this.btnSave.TabIndex = 520;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnClear
			// 
			this.btnClear.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnClear.Image")));
			this.btnClear.Location = new System.Drawing.Point(328, 16);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(24, 24);
			this.btnClear.TabIndex = 510;
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// btnEdit
			// 
			this.btnEdit.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnEdit.Image")));
			this.btnEdit.Location = new System.Drawing.Point(288, 16);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Size = new System.Drawing.Size(24, 24);
			this.btnEdit.TabIndex = 500;
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// txtAccountNo
			// 
			this.txtAccountNo.Location = new System.Drawing.Point(112, 16);
			this.txtAccountNo.Name = "txtAccountNo";
			this.txtAccountNo.TabIndex = 10;
			this.txtAccountNo.Text = "000-0000-0000-0";
			this.txtAccountNo.Leave += new System.EventHandler(this.txtAccountNo_Leave);
			// 
			// lAccountNo
			// 
			this.lAccountNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lAccountNo.Location = new System.Drawing.Point(16, 16);
			this.lAccountNo.Name = "lAccountNo";
			this.lAccountNo.Size = new System.Drawing.Size(72, 16);
			this.lAccountNo.TabIndex = 0;
			this.lAccountNo.Text = "Account no";
			this.lAccountNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtRejectCount
			// 
			this.txtRejectCount.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(255)), ((System.Byte)(128)));
			this.txtRejectCount.Enabled = false;
			this.txtRejectCount.ForeColor = System.Drawing.SystemColors.ActiveCaption;
			this.txtRejectCount.Location = new System.Drawing.Point(240, 240);
			this.txtRejectCount.MaxLength = 10;
			this.txtRejectCount.Name = "txtRejectCount";
			this.txtRejectCount.Size = new System.Drawing.Size(40, 20);
			this.txtRejectCount.TabIndex = 0;
			this.txtRejectCount.TabStop = false;
			this.txtRejectCount.Text = "";
			// 
			// lRejectCount
			// 
			this.lRejectCount.Location = new System.Drawing.Point(112, 240);
			this.lRejectCount.Name = "lRejectCount";
			this.lRejectCount.Size = new System.Drawing.Size(128, 16);
			this.lRejectCount.TabIndex = 0;
			this.lRejectCount.Text = "Consecutive rejections";
			this.lRejectCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtBankAcctName
			// 
			this.txtBankAcctName.BackColor = System.Drawing.SystemColors.Window;
			this.txtBankAcctName.Enabled = false;
			this.txtBankAcctName.Location = new System.Drawing.Point(112, 200);
			this.txtBankAcctName.MaxLength = 20;
			this.txtBankAcctName.Name = "txtBankAcctName";
			this.txtBankAcctName.Size = new System.Drawing.Size(192, 20);
			this.txtBankAcctName.TabIndex = 80;
			this.txtBankAcctName.Text = "";
			// 
			// txtBankBranchNo
			// 
			this.txtBankBranchNo.BackColor = System.Drawing.SystemColors.Window;
			this.txtBankBranchNo.Enabled = false;
			this.txtBankBranchNo.Location = new System.Drawing.Point(112, 152);
			this.txtBankBranchNo.MaxLength = 3;
			this.txtBankBranchNo.Name = "txtBankBranchNo";
			this.txtBankBranchNo.Size = new System.Drawing.Size(48, 20);
			this.txtBankBranchNo.TabIndex = 60;
			this.txtBankBranchNo.Text = "";
			// 
			// txtBankAcctNo
			// 
			this.txtBankAcctNo.BackColor = System.Drawing.SystemColors.Window;
			this.txtBankAcctNo.Enabled = false;
			this.txtBankAcctNo.Location = new System.Drawing.Point(112, 176);
			this.txtBankAcctNo.MaxLength = 11;
			this.txtBankAcctNo.Name = "txtBankAcctNo";
			this.txtBankAcctNo.Size = new System.Drawing.Size(192, 20);
			this.txtBankAcctNo.TabIndex = 70;
			this.txtBankAcctNo.Text = "";
			// 
			// drpBank
			// 
			this.drpBank.BackColor = System.Drawing.SystemColors.Window;
			this.drpBank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpBank.Enabled = false;
			this.drpBank.ItemHeight = 13;
			this.drpBank.Location = new System.Drawing.Point(112, 128);
			this.drpBank.Name = "drpBank";
			this.drpBank.Size = new System.Drawing.Size(192, 21);
			this.drpBank.TabIndex = 50;
			// 
			// drpDueDay
			// 
			this.drpDueDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpDueDay.Enabled = false;
			this.drpDueDay.ItemHeight = 13;
			this.drpDueDay.Location = new System.Drawing.Point(112, 104);
			this.drpDueDay.Name = "drpDueDay";
			this.drpDueDay.Size = new System.Drawing.Size(56, 21);
			this.drpDueDay.TabIndex = 40;
			// 
			// txtInstalAmount
			// 
			this.txtInstalAmount.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
			this.txtInstalAmount.Location = new System.Drawing.Point(112, 72);
			this.txtInstalAmount.MaxLength = 10;
			this.txtInstalAmount.Name = "txtInstalAmount";
			this.txtInstalAmount.ReadOnly = true;
			this.txtInstalAmount.Size = new System.Drawing.Size(72, 20);
			this.txtInstalAmount.TabIndex = 30;
			this.txtInstalAmount.TabStop = false;
			this.txtInstalAmount.Text = "";
			// 
			// txtCustomerName
			// 
			this.txtCustomerName.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
			this.txtCustomerName.Location = new System.Drawing.Point(112, 48);
			this.txtCustomerName.MaxLength = 80;
			this.txtCustomerName.Name = "txtCustomerName";
			this.txtCustomerName.ReadOnly = true;
			this.txtCustomerName.Size = new System.Drawing.Size(280, 20);
			this.txtCustomerName.TabIndex = 20;
			this.txtCustomerName.Text = "";
			// 
			// lInstalAmount
			// 
			this.lInstalAmount.Location = new System.Drawing.Point(16, 72);
			this.lInstalAmount.Name = "lInstalAmount";
			this.lInstalAmount.Size = new System.Drawing.Size(80, 16);
			this.lInstalAmount.TabIndex = 0;
			this.lInstalAmount.Text = "Instalment";
			this.lInstalAmount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lBankAcctName
			// 
			this.lBankAcctName.Location = new System.Drawing.Point(16, 200);
			this.lBankAcctName.Name = "lBankAcctName";
			this.lBankAcctName.Size = new System.Drawing.Size(96, 16);
			this.lBankAcctName.TabIndex = 0;
			this.lBankAcctName.Text = "Bank acct name";
			this.lBankAcctName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lBankAcctNo
			// 
			this.lBankAcctNo.Location = new System.Drawing.Point(16, 176);
			this.lBankAcctNo.Name = "lBankAcctNo";
			this.lBankAcctNo.Size = new System.Drawing.Size(80, 16);
			this.lBankAcctNo.TabIndex = 0;
			this.lBankAcctNo.Text = "Bank acct no";
			this.lBankAcctNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lBankBranchNo
			// 
			this.lBankBranchNo.Location = new System.Drawing.Point(16, 152);
			this.lBankBranchNo.Name = "lBankBranchNo";
			this.lBankBranchNo.Size = new System.Drawing.Size(80, 16);
			this.lBankBranchNo.TabIndex = 0;
			this.lBankBranchNo.Text = "Branch no";
			this.lBankBranchNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lBank
			// 
			this.lBank.Location = new System.Drawing.Point(16, 128);
			this.lBank.Name = "lBank";
			this.lBank.Size = new System.Drawing.Size(80, 16);
			this.lBank.TabIndex = 0;
			this.lBank.Text = "Bank";
			this.lBank.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lDueDay
			// 
			this.lDueDay.Location = new System.Drawing.Point(16, 104);
			this.lDueDay.Name = "lDueDay";
			this.lDueDay.Size = new System.Drawing.Size(80, 16);
			this.lDueDay.TabIndex = 0;
			this.lDueDay.Text = "Due day";
			this.lDueDay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lCustomerName
			// 
			this.lCustomerName.Location = new System.Drawing.Point(16, 48);
			this.lCustomerName.Name = "lCustomerName";
			this.lCustomerName.Size = new System.Drawing.Size(96, 16);
			this.lCustomerName.TabIndex = 0;
			this.lCustomerName.Text = "Customer name";
			this.lCustomerName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// DDMandate
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(792, 477);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.grpMandate,
																		  this.grpComments,
																		  this.grpCreditOfficerOnly});
			this.Name = "DDMandate";
			this.Text = "Giro Mandate";
			this.grpCreditOfficerOnly.ResumeLayout(false);
			this.grpComments.ResumeLayout(false);
			this.grpMandate.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		//
		// Local Routines
		//
		private void Init()
		{
			// Set up
			toolTip1.SetToolTip(this.btnEdit,   GetResource("TT_EDIT"));
			toolTip1.SetToolTip(this.btnClear,  GetResource("TT_CLEAR"));
			toolTip1.SetToolTip(this.btnSave,   GetResource("TT_SAVE"));
			toolTip1.SetToolTip(this.btnReturn, GetResource("TT_DDRETURN"));

			LoadStaticData();

			this._mandateSet = null;
			this._mandateDetails = null;
			this._acctLoaded = false;

			this.txtAccountNo.ResetText();
			this._lastAccountNo = this.txtAccountNo.Text;
			this.txtCustomerName.Text = "";
			this.txtInstalAmount.Text = "";
			this.drpDueDay.SelectedIndex = 0;
			this.drpBank.SelectedIndex = 0;
			this.txtBankBranchNo.Text = "";
			this.txtBankAcctNo.Text = "";
			this.txtBankAcctName.Text = "";
			this.txtComment.Text = "";

			this.dtReceived.Value      = Date.blankDate;
			this.dtReturned.Value      = Date.blankDate;
			this.dtSubmitted.Value     = Date.blankDate;
			this.dtApproved.Value      = Date.blankDate;
			this.dtCommencement.Value  = Date.blankDate;
			this.dtTermination.Value   = Date.blankDate;
			this.txtRejectCount.Text   = "0";
			this.lCancelReason.Text    = "";
			this.lActiveText.Text      = "";

			// Initially Account No key field is only changeable field
			this.SetEditMode(false);
		}    // End of Init

		private void LoadStaticData()
		{
			try
			{
				Wait();
		
				XmlUtilities xml = new XmlUtilities();
				XmlDocument dropDowns = new XmlDocument();
				dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

				if(StaticData.Tables[TN.DDDueDate]==null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.DDDueDate, null));

				if(StaticData.Tables[TN.Bank]==null)
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
				drpDueDay.DataSource = (DataTable)StaticData.Tables[TN.DDDueDate];
				drpDueDay.DisplayMember = CN.DueDay;
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
		}    // End of LoadStaticData

		private void LoadDetails(int mandateId, string accountNo)
		{
			try
			{
				Wait();
				Function = "Mandate LoadDetails";

				int rowCount = 0;
				this._mandateSet = null;
				if (mandateId != 0)
					this._mandateSet = PaymentManager.GetDDMandate(Config.CountryCode, mandateId, out rowCount, out Error);
				else
					this._mandateSet = PaymentManager.GetDDMandate(Config.CountryCode, accountNo.Trim(), out rowCount, out Error);

				if (Error.Length > 0)
				{
					this.txtAccountNo.Focus();
					ShowError(Error);
				}
				else if (this._mandateSet == null || rowCount == 0)
				{
					// No data
					this.txtAccountNo.Focus();
					ShowInfo("M_NOACCOUNTDATA");
				}
				else
				{
					this.DisplayProperties();
					this.SetEditMode(false);
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of Mandate LoadDetails";
			}
		}    // End of LoadDetails

		private void DisplayProperties()
		{
			foreach(DataTable dt in this._mandateSet.Tables)
			{
				switch(dt.TableName)
				{
					case TN.DDMandate:
						this._mandateDetails        = dt.Rows[0];
						this.txtAccountNo.Text      = System.Convert.ToString(_mandateDetails[CN.AcctNo]);
						this._lastAccountNo         = this.txtAccountNo.Text;
						this._acctLoaded            = (bool)_mandateDetails[CN.AcctLoaded];
						this.txtBankAcctName.Text   = ((string)_mandateDetails[CN.BankAccountName]).Trim();
						this.txtBankAcctNo.Text     = ((string)_mandateDetails[CN.BankAccountNo]).Trim();
						this.txtBankBranchNo.Text   = ((string)_mandateDetails[CN.BankBranchNo]).Trim();
						this.txtComment.Text        = (string)_mandateDetails[CN.Comment];
						//this.txtNoFees            = (int)_mandateDetails[CN.NoFees];
						this.txtRejectCount.Text    = ((int)_mandateDetails[CN.RejectCount]).ToString();
						this.dtCommencement.Value   = (DateTime)_mandateDetails[CN.StartDate];
						this.txtCustomerName.Text   = (string)_mandateDetails[CN.CustomerName];
						this.txtInstalAmount.Text   = ((decimal)_mandateDetails[CN.InstalAmount]).ToString(DecimalPlaces);
						this.lCancelReason.Text     = (string)_mandateDetails[CN.CancelDesc];
						this.lActiveText.Text       = (string)_mandateDetails[CN.ActiveText];
						//this.lActiveText.ForeColor  = (Color)_mandateDetails[CN.ActiveTextColor];
						this.dtReceived.Value		= (DateTime)_mandateDetails[CN.ReceiveDate];
						this.dtReturned.Value       = (DateTime)_mandateDetails[CN.ReturnDate];
						this.dtSubmitted.Value      = (DateTime)_mandateDetails[CN.SubmitDate];
						this.dtApproved.Value       = (DateTime)_mandateDetails[CN.ApprovalDate];
						this.dtTermination.Value    = (DateTime)_mandateDetails[CN.EndDate];

						// Giro Due Day drop down
						int index = 0;
						foreach(DataRow row in ((DataTable)drpDueDay.DataSource).Rows)
						{
							if((int)row[CN.DueDayId] == (int)_mandateDetails[CN.DueDayId])
								drpDueDay.SelectedIndex = index;
							index++;
						}

						// Bank drop down
						index = 0;
						foreach(DataRow row in ((DataTable)drpBank.DataSource).Rows)
						{
							if(((string)row[CN.BankCode]).Trim() == ((string)_mandateDetails[CN.BankCode]).Trim())
								drpBank.SelectedIndex = index;
							index++;
						}

						break;
					default:
						break;
				}
			}
		}    // End of DisplayProperties

		private void SetEditMode(bool editMode)
		{
			this._formEditMode = editMode;
			if (editMode)
			{
				if ((string)_mandateDetails[CN.Status] == DDStatus.Deleted)
				{
					// A deleted mandate cannot be updated, but the user is given
					// the option to create a new mandate for the account.
					// A history record is kept of the old mandate and the new
					// mandate must to be sent to the bank.
					if (DialogResult.Yes ==
						ShowInfo("M_MANDATEDELETED", MessageBoxButtons.YesNo))
					{
						this.dtReceived.Value      = Date.blankDate;
						this.dtReturned.Value      = Date.blankDate;
						this.dtSubmitted.Value     = Date.blankDate;
						this.dtApproved.Value      = Date.blankDate;
						this.dtCommencement.Value  = Date.blankDate;
						this.dtTermination.Value   = Date.blankDate;
						this.txtRejectCount.Text   = "0";
						this.lCancelReason.Text    = "";

						_mandateDetails[CN.CancelReason]    = DDCancel.Init;
						_mandateDetails[CN.CurAcctNo]       = "";
						_mandateDetails[CN.CurBankAcctNo]   = "";
						_mandateDetails[CN.CurBankBranchNo] = "";
						_mandateDetails[CN.CurBankCode]     = "";
						_mandateDetails[CN.CurDueDayId]     = 0;
						_mandateDetails[CN.CurEndDate]      = new System.DateTime(0);
						_mandateDetails[CN.CurStartDate]    = new System.DateTime(0);
						_mandateDetails[CN.MandateId]       = 0;
						_mandateDetails[CN.ReturnLetter]    = false;
						_mandateDetails[CN.Status]          = DDStatus.Current;

						this.SetDateBias(editMode);
					}
					else
					{
						return;
					}
				}
				
				// An approved mandate can only be updated by Head Office
				if  (this.dtApproved.Value != Date.blankDate
					&& !Credential.IsInRole("N")
					&& !Credential.IsInRole("O"))
				{
					ShowInfo ("M_MANDATEAPPROVED");
					return;
				}

				// Confirm changes required to an active mandate
				if ((string)_mandateDetails[CN.Status] == DDStatus.Active)
				{
					if (DialogResult.No ==
						ShowInfo("M_MANDATEACTIVE", MessageBoxButtons.YesNo))
					{
						return;
					}

				}

				if (!(bool)_mandateDetails[CN.Loaded])
				{
					/* Display the bank account if passed in */
					this.txtBankAcctNo.Text = this._localBankAcctNo;
					// Bank drop down
					int index = 0;
					foreach(DataRow row in ((DataTable)drpBank.DataSource).Rows)
					{
						if((string)row[CN.BankCode] == this._localBankCode)
							drpBank.SelectedIndex = index;
						index++;
					}
				}
			}

			// Set fields
			this.txtAccountNo.ReadOnly = this._acctLoaded;
			this.drpDueDay.Enabled = editMode;
			this.drpBank.Enabled = editMode;
			this.txtBankBranchNo.Enabled = editMode;
			this.txtBankAcctNo.Enabled = editMode;
			this.txtBankAcctName.Enabled = editMode;
			this.txtComment.Enabled = editMode;
			this.btnEdit.Enabled = (!editMode && this._acctLoaded);
			this.btnClear.Enabled = this._acctLoaded;
			this.btnSave.Enabled = editMode;
			this.SetDateBias(editMode);

			if (editMode)
				this.drpDueDay.Focus();
			else
				this.txtAccountNo.Focus();

		}    // End of SetEditMode

		private void SetDateBias(bool editMode)
		{
			if (   editMode
				&& (Credential.IsInRole("N") || Credential.IsInRole("O")))
			{
				/* Enable the date fields in order of required entry */
				this.dtReceived.Enabled = true;
        
				if (this.dtReceived.Value == Date.blankDate)
				{
					this.btnReturn.Enabled = false;
					this.dtSubmitted.Enabled = false;
				}
				else
				{
					this.btnReturn.Enabled = true;
					this.dtSubmitted.Enabled = true;
				}

				/* The Return Date is optional so check SubmitDate next */
				if (this.dtSubmitted.Value == Date.blankDate || !this.dtSubmitted.Enabled)
				{
					this.dtApproved.Enabled = false;
				}
				else
				{
					this.dtApproved.Enabled = true;
				}

				if (this.dtApproved.Value == Date.blankDate || !this.dtApproved.Enabled)
				{
					this.dtCommencement.Enabled = false;
					this.dtTermination.Enabled = false;
				}
				else
				{
					this.dtCommencement.Enabled = true;
					this.dtTermination.Enabled = true;
				}
			}
			else
			{
				this.dtReceived.Enabled      = false;
				this.btnReturn.Enabled       = false;
				this.dtSubmitted.Enabled     = false;
				this.dtApproved.Enabled      = false;
				this.dtCommencement.Enabled  = false;
				this.dtTermination.Enabled   = false;
			}

		}    // End of SetDateBias

		private bool ValidMandate (out bool addNewRecord)
		{
			addNewRecord = false;

			/* Basic validation */
			if (!this.BasicValidation()) return false;
			
			/* Check whether another mandate is current for this Account Number */
			int anMandateId;
			DateTime startDate;
			DateTime endDate;
			DateTime effDate;
			int rowCount = PaymentManager.AnotherMandate(
				Config.CountryCode,
				(int)this._mandateDetails[CN.MandateId],
				this.txtAccountNo.Text.Trim(),
				out anMandateId,
				out startDate,
				out endDate,
				out effDate,
				out Error);

			if(Error.Length > 0)
			{
				ShowError(Error);
				return false;
			}

			if (rowCount != 0)
			{
				/* There is an existing mandate for this account */
				string startDateStr = "a blank Start Date";
				string endDateStr   = "a blank End Date";

				if (startDate != Date.blankDate)
					startDateStr = "the Start Date of " + startDate.ToShortDateString();

				if (endDate != Date.blankDate)
					endDateStr = "the End Date of " + endDate.ToShortDateString();
	
				// Init the new Start Date to be blank
				DateTime newStartDate = Date.blankDate;
				string newStartDateStr = "a blank Start Date";
				if (this.dtCommencement.Value > effDate)
				{
					/* Keep the Start Date only if it is after (today + lead time) */
					newStartDate = this.dtCommencement.Value;
					newStartDateStr = "the Start Date of " + newStartDate.ToShortDateString();
				}
				else if (this.dtCommencement.Value > Date.blankDate && this.dtCommencement.Value <= effDate)
				{
					/* Replace a Start Date before (today + lead time) with the next effective date */
					newStartDate = effDate;
					newStartDateStr = "the Start Date of " + newStartDate.ToShortDateString();
				}

				// Init the new End Date to be blank
				DateTime newEndDate = Date.blankDate;
				string newEndDateStr = "a blank End Date";
				if (this.dtTermination.Value > effDate)
				{
					/* Keep the End Date only if it is after (today + lead time) */
					newEndDate = this.dtTermination.Value;
					newEndDateStr = "the End Date " + newEndDate.ToShortDateString();
				}

				if (DialogResult.No ==
					ShowInfo("M_MANDATEEXISTS", new object[] {this.txtAccountNo, startDateStr, endDateStr, effDate.ToShortDateString(), newStartDateStr, newEndDateStr}, MessageBoxButtons.YesNo))
				{
					return false;
				}

				this.dtCommencement.Value = newStartDate;
				this.dtTermination.Value = newEndDate;
				addNewRecord = true;

			}
			else if (   (DateTime)this._mandateDetails[CN.CurStartDate] > Date.blankDate
				&& (DateTime)this._mandateDetails[CN.CurStartDate] <= effDate)
			{
				/* Check whether a history record is required */
				if (   (((string)_mandateDetails[CN.CurAcctNo]).Trim()		 != "" && ((string)_mandateDetails[CN.CurAcctNo]).Trim()	   != this.txtAccountNo.UnformattedText.Trim())
					|| ((int)_mandateDetails[CN.CurDueDayId]				 != 0  && (int)_mandateDetails[CN.CurDueDayId]				   != (int)((DataRowView)drpDueDay.SelectedItem)[CN.DueDayId])
					|| (((string)_mandateDetails[CN.CurBankCode]).Trim()	 != "" && ((string)_mandateDetails[CN.CurBankCode]).Trim()	   != ((string)((DataRowView)drpBank.SelectedItem)[CN.BankCode]).Trim())
					|| (((string)_mandateDetails[CN.CurBankBranchNo]).Trim() != "" && ((string)_mandateDetails[CN.CurBankBranchNo]).Trim() != this.txtBankBranchNo.Text.Trim())
					|| (((string)_mandateDetails[CN.CurBankAcctNo]).Trim()	 != "" && ((string)_mandateDetails[CN.CurBankAcctNo]).Trim()   != this.txtBankAcctNo.Text.Trim())
					|| (((string)_mandateDetails[CN.CurBankAcctName]).Trim() != "" && ((string)_mandateDetails[CN.CurBankAcctName]).Trim() != this.txtBankAcctName.Text.Trim())
					|| ((DateTime)_mandateDetails[CN.CurEndDate] > Date.blankDate  && (DateTime)_mandateDetails[CN.CurEndDate]			   != this.dtTermination.Value))
				{

					// Init the new Start Date to be blank
					DateTime newStartDate = Date.blankDate;
					string newStartDateStr = "a blank Start Date";
					if (this.dtCommencement.Value > effDate)
					{
						/* Keep the Start Date only if it is after (today + lead time) */
						newStartDate = this.dtCommencement.Value;
						newStartDateStr = "the Start Date of " + newStartDate.ToShortDateString();
					}
					else if (this.dtCommencement.Value > Date.blankDate && this.dtCommencement.Value <= effDate)
					{
						/* Replace a Start Date before (today + lead time) with the next effective date */
						newStartDate = effDate;
						newStartDateStr = "the Start Date of " + newStartDate.ToShortDateString();
					}

					// Init the new End Date to be blank
					DateTime newEndDate = Date.blankDate;
					string newEndDateStr = "a blank End Date";
					if (this.dtTermination.Value > effDate)
					{
						/* Keep the End Date only if it is after (today + lead time) */
						newEndDate = this.dtTermination.Value;
						newEndDateStr = "the End Date " + newEndDate.ToShortDateString();
					}

					if (DialogResult.No ==
						ShowInfo("M_MANDATECHANGED", new object[] {newStartDateStr, newEndDateStr}, MessageBoxButtons.YesNo))
					{
						return false;
					}

					this.dtCommencement.Value = newStartDate;
					this.dtTermination.Value = newEndDate;
					addNewRecord = true;
				}
			}


			if (   ((DateTime)_mandateDetails[CN.CurEndDate] == Date.blankDate && this.dtTermination.Value != Date.blankDate)
				|| (DateTime)_mandateDetails[CN.CurEndDate] != this.dtTermination.Value)
			{
				/* The user added or changed the end date */
				this._mandateDetails[CN.CancelReason] = DDCancel.UserCancelled;
			}
        
			if (   (DateTime)_mandateDetails[CN.CurEndDate] <= effDate
				&& (DateTime)_mandateDetails[CN.CurEndDate] > Date.blankDate
				&& (this.dtTermination.Value == Date.blankDate || this.dtTermination.Value > effDate))
			{
				/* The user has re-instated a non-current mandate by removing or extending the 
				** end date, so the reject count and cancel reason might need to be cleared.
				*/
				if (DialogResult.No ==
					ShowInfo("M_MANDATEREINSTATED", MessageBoxButtons.YesNo))
				{
					return false;
				}

				if (this.dtTermination.Value == Date.blankDate)
				{
					this._mandateDetails[CN.CancelReason] = DDCancel.Init;
				}
				this.txtRejectCount.Text = "0";

			}

			return true;
		}

		/// <summary>
		/// Note that the validation is intended to allow past and future dates
		/// to be entered or updated, except for the 'StartDate' and 'EndDate' (the
		/// mandate period). The 'StartDate' and 'EndDate' can only be entered as
		/// being on or after (today + lead time). If dates before this were allowed
		/// then mandates could be entered that would appear to have missed
		/// previous payments or continued to have raised payments after their end
		/// date. For all records created for the same Courts customer account
		/// number there should not be any overlap in the mandate periods, and only
		/// one mandate should be active at any one time. This validation will
		/// ensure this.
		/// </summary>
		private bool BasicValidation ()
		{
			/* Remove excess spaces from char fields */
			this.txtBankBranchNo.Text	= this.txtBankBranchNo.Text.Trim();
			this.txtBankAcctNo.Text		= this.txtBankAcctNo.Text.Trim();
			this.txtBankAcctName.Text	= this.txtBankAcctName.Text.Trim();
			
			/* Check for embedded dashes and spaces */
			if (   this.txtBankBranchNo.Text.IndexOf(" ") >= 0
				|| this.txtBankBranchNo.Text.IndexOf("-") >= 0
				|| this.txtBankAcctNo.Text.IndexOf(" ") >= 0
				|| this.txtBankAcctNo.Text.IndexOf("-") >= 0)
			{
				ShowInfo("M_MANDATEDASHSPACE");
				return false;
			}
    
			/* The Due Day, Bank Code, Bank Branch No, Bank Account No and Bank Account Name are mandatory */
			if (   this.drpDueDay.Text == ""
				|| this.drpBank.Text == ""
				|| this.txtBankBranchNo.Text == ""
				|| this.txtBankAcctNo.Text == ""
				|| this.txtBankAcctName.Text == "")
			{
				ShowInfo("M_MANDATEMANDATORY");
				return false;
			}
    
			/* Check all the dates are in the correct sequence */

			/* Receive Date must be on or before the optional Return Date and the
			** mandatory Approval Date.
			*/
			if (!ValidDates(
				this.dtReceived.Value,
				this.dtReturned.Value,
				this.dtApproved.Value,
				"Receive",
				"Return",
				"Approval",
				false))
			{
				return false;
			}

			/* The Return Date is optional, but if entered must be on or before Submit Date */
			if (!ValidDates(
				this.dtReturned.Value,
				this.dtSubmitted.Value,
				Date.blankDate,
				"Return",
				"Submit",
				"",
				true))
			{
				return false;
			}

			/* Submit Date must be on or before the Approval Date */
			if (!ValidDates(
				this.dtSubmitted.Value,
				this.dtApproved.Value,
				Date.blankDate,
				"Submit",
				"Approval",
				"",
				false))
			{
				return false;
			}

			/* Approval Date must be on or before the optional Start Date and the
			** mandatory End Date.
			*/
			if (!ValidDates(
				this.dtApproved.Value,
				this.dtCommencement.Value,
				this.dtTermination.Value,
				"Approval",
				"Start",
				"End",
				false))
			{
				return false;
			}

			/* Start Date is optional, but if entered must be on or before End Date */
			if (!ValidDates(
				this.dtCommencement.Value,
				this.dtTermination.Value,
				Date.blankDate,
				"Start",
				"End",
				"",
				true))
			{
				return false;
			}


			/* A changed start date or end date must be on or after (today + lead time) */

			/* Init the mandate object with the effective date */
			this._giroDateSet = PaymentManager.InitDates(Config.CountryCode, System.DateTime.Today, out Error);
			if(Error.Length > 0)
			{
				ShowError(Error);
				return false;
			}
			this._giroDates = _giroDateSet.Tables[TN.DDGiroDates].Rows[0];

			/* A historic Start Date cannot be changed or removed */
			if (   (DateTime)_mandateDetails[CN.CurStartDate] != Date.blankDate
				&& (DateTime)_mandateDetails[CN.CurStartDate] < (DateTime)_giroDates[CN.EffDate]
				&& (this.dtCommencement.Value != (DateTime)_mandateDetails[CN.CurStartDate]))
			{
				ShowInfo("M_HISTORICCHANGE");
				return false;
			}

			/* A Start Date cannot be changed to a historic date */
			if (   this.dtCommencement.Value < (DateTime)_giroDates[CN.EffDate]
				&& this.dtCommencement.Value != (DateTime)_mandateDetails[CN.CurStartDate])
			{
				ShowInfo("M_HISTORICBEFORESTART", new object[] {((DateTime)_giroDates[CN.EffDate]).ToShortDateString()});
				return false;
			}
    
			/* A historic End Date cannot be changed to another historic date or removed.
			** If an End Date is changed from a historic date to a future date, then the
			** save processing will ensure a history record is created.
			*/
			if (   (DateTime)_mandateDetails[CN.CurEndDate] != Date.blankDate
				&& (DateTime)_mandateDetails[CN.CurEndDate] < (DateTime)_giroDates[CN.EffDate]
				&& this.dtTermination.Value != (DateTime)_mandateDetails[CN.CurEndDate]
				&& this.dtTermination.Value < (DateTime)_giroDates[CN.EffDate])
			{
				ShowInfo("M_HISTORICNEW");
				return false;
			}

			/* An End Date cannot be changed to a historic date */
			if (   this.dtTermination.Value < (DateTime)_giroDates[CN.EffDate]
				&& this.dtTermination.Value != (DateTime)_mandateDetails[CN.CurEndDate])
			{
				ShowInfo("M_HISTORICBEFOREEND", new object[] {((DateTime)_giroDates[CN.EffDate]).ToShortDateString()});
				return false;
			}

			return true;
		}    // End of BasicValidation

		
		/// <summary>
		/// Validate dates are in chronological order
		/// Note: dates on or before 01/01/1900 will be treated as blank/null
		/// </summary>
		/// <param name="piDate1"></param>
		/// <param name="piDate2"></param>
		/// <param name="piDate3"></param>
		/// <param name="piDate1Name"></param>
		/// <param name="piDate2Name"></param>
		/// <param name="piDate3Name"></param>
		/// <param name="piOptional"></param>
		/// <returns></returns>
		private bool ValidDates(
			DateTime piDate1,
			DateTime piDate2,
			DateTime piDate3,
			string piDate1Name,
			string piDate2Name,
			string piDate3Name,
			bool   piOptional)
		{
			// Validate Date1 is on or before Date2
			// If piOptional is TRUE then Date1 can be blank even though Date2 has been entered.
			if ((!piOptional && piDate1 <= Date.blankDate && piDate2 > Date.blankDate)
				|| (piDate1 > piDate2) && piDate2 > Date.blankDate)
			{
				ShowInfo("M_CHRONODATES", new object[] {piDate1Name, piDate2Name});
				return false;
			}

			// If Date3 has been entered then Date1 must be on or before Date3
			if (   piDate3 > Date.blankDate
				&& (piDate1 <= Date.blankDate || piDate1 > piDate3))
			{
				ShowInfo("M_CHRONODATES", new object[] {piDate1Name, piDate3Name});
				return false;
			}

			return true;
		}    // End of ValidDates



		//
		// Form Events
		//
		private void txtAccountNo_Leave(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Leaving Account No";

				if (this.txtAccountNo.Text.Trim() != this._lastAccountNo.Trim())
				{
					Wait();
					this._lastAccountNo = this.txtAccountNo.Text;
					this.LoadDetails(0, this.txtAccountNo.UnformattedText);
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of Leaving Account No";
			}
		}

		private void btnEdit_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Set Edit Mode";

				if (!this._formEditMode)
				{
					Wait();
					this.SetEditMode(true);
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of Set Edit Mode";
			}
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Clear Form";
				Wait();

				this.Init();
				/* Show the account number if one was passed in by navigation */
				if (this._localAccountNo.Length > 0)
				{
					this.txtAccountNo.Text = this._localAccountNo.Trim();
					this._lastAccountNo = this.txtAccountNo.Text;
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of Clear Form";
			}
		}

		private void btnReturn_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Return Mandate Button";
				Wait();

				// Return letter to customer to complete the mandate

				if (this.dtReturned.Value != Date.blankDate)
				{
					if (DialogResult.No ==
						ShowInfo("M_MANDATERETURNED", new object[] {this.dtReturned.ToString()}, MessageBoxButtons.YesNo))
					{
						return;
					}
				}

				/* Set the Return Date to today and flag to save a return letter */
				this.dtReturned.Value = System.DateTime.Today;
				this._mandateDetails[CN.ReturnLetter] = true;

			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of Return Mandate Button";
			}
		}

		private void dtReceived_Leave(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Leaving Received Date";
				Wait();

				this.SetDateBias(_formEditMode);
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of Leaving Received Date";
			}
		}

		private void dtSubmitted_Leave(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Leaving Submitted Date";
				Wait();

				this.SetDateBias(_formEditMode);
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of Leaving Submitted Date";
			}
		}

		private void dtApproved_Leave(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Leaving Approved Date";
				Wait();

				this.SetDateBias(_formEditMode);
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of Leaving Approved Date";
			}
		}

		private void dtCommencement_Leave(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Leaving Commencement Date";
				Wait();

				this.SetDateBias(_formEditMode);
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of Leaving Commencement Date";
			}
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Save Mandate";
				Wait();

				bool addNewRecord;
				if (this.ValidMandate(out addNewRecord))
				{
					// Copy screen fields back to the BL
					_mandateDetails[CN.AcctNo]          = this.txtAccountNo.UnformattedText.Trim();
					_mandateDetails[CN.ApprovalDate]    = this.dtApproved.Value;
					_mandateDetails[CN.BankAccountName] = this.txtBankAcctName.Text.Trim();
					_mandateDetails[CN.BankAccountNo]   = this.txtBankAcctNo.Text.Trim();
					_mandateDetails[CN.BankBranchNo]    = this.txtBankBranchNo.Text.Trim();
					_mandateDetails[CN.Comment]         = this.txtComment.Text;
					_mandateDetails[CN.EndDate]         = this.dtTermination.Value;
					//_mandateDetails[CN.NoFees]        = this.txtNoFees;
					_mandateDetails[CN.ReceiveDate]     = this.dtReceived.Value;
					_mandateDetails[CN.RejectCount]     = this.txtRejectCount.Text;
					_mandateDetails[CN.ReturnDate]      = this.dtReturned.Value;
					_mandateDetails[CN.StartDate]       = this.dtCommencement.Value;
					_mandateDetails[CN.SubmitDate]      = this.dtSubmitted.Value;
					_mandateDetails[CN.DueDayId]        = ((DataRowView)drpDueDay.SelectedItem)[CN.DueDayId];
					_mandateDetails[CN.BankCode]        = ((DataRowView)drpBank.SelectedItem)[CN.BankCode];

					// Save the mandate
					// The properties must be re-displayed after the save call
					this._mandateSet = PaymentManager.SaveMandate(Config.CountryCode, addNewRecord, this._mandateSet, out Error);
					if(Error.Length>0)
						ShowError(Error);
					else
					{
						this.DisplayProperties();
						this.SetEditMode(false);
					}
				}

			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of Save Mandate";
			}

		}

	}
}
