using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common.Constants.SanctionStages;
using STL.PL.WS2;
using STL.PL.WS7;
using STL.Common.Static;
using System.Web.Services.Protocols;
using System.Data;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using System.Xml;
using STL.Common;
using STL.Common.Constants.ScreenModes;

namespace STL.PL
{
	/// <summary>
	/// Test screen for the referral screen.
	/// </summary>
	public class Referral_Test : CommonForm
	{
	
		private new string Error = "";

		private string _custid = "";
		public string CustomerID
		{
			get{return _custid;}
			set{_custid = value;}
		}

		private string _acctno = "";
		public string AccountNo
		{
			get{return _acctno;}
			set{_acctno = value;}
		}

		private string _currentStatus = "";
		public string CurrentStatus
		{
			get{return _currentStatus;}
			set{_currentStatus = value;}
		}

		private string _acctType = "";
		public string AccountType
		{
			get{return _acctType;}
			set{_acctType = value;}
		}

		private string _screenMode = "";
		public string ScreenMode
		{
			get{return _screenMode;}
			set{_screenMode = value;}
		}

		private DateTime _dateProp;
		public DateTime DateProp
		{
			get{return _dateProp;}
			set{_dateProp = value;}
		}

		private bool _acctLocked = false;
		private System.Windows.Forms.GroupBox groupBox1;
		private Crownwood.Magic.Controls.TabControl tcReferralDetails;
		private Crownwood.Magic.Controls.TabPage tpUnderWriter;
		private Crownwood.Magic.Controls.TabControl tcUnderWriter;
		private Crownwood.Magic.Controls.TabPage tpDetails;
		private System.Windows.Forms.ComboBox drpPolicyRule3;
		private System.Windows.Forms.ComboBox drpPolicyRule1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.TextBox txtScore;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.TextBox txtRiskCategory;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox txtSysRecommendation;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RichTextBox rtxtAdditionalData;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.ComboBox drpAdditionalData;
		private System.Windows.Forms.ComboBox drpPolicyRule5;
		private System.Windows.Forms.ComboBox drpPolicyRule2;
		private System.Windows.Forms.ComboBox drpPolicyRule4;
		private System.Windows.Forms.ComboBox drpPolicyRule6;
		private System.Windows.Forms.GroupBox groupBox4;
		private Crownwood.Magic.Controls.TabPage tpFinalDecision;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.ComboBox drpOverride;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label lCreditLimit;
		private System.Windows.Forms.TextBox txtCreditLimit;
		private System.Windows.Forms.RichTextBox rtxtNewReferralNotes;
		private System.Windows.Forms.RichTextBox rtxtReferralNotes;
		private System.Windows.Forms.RadioButton rbReject;
		private System.Windows.Forms.RadioButton rbApprove;
		private System.Windows.Forms.NumericUpDown noAvail;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Label label23;
		private Crownwood.Magic.Controls.TabPage tpSummaryInfo;
		private System.Windows.Forms.GroupBox gpPersonal;
		private STL.PL.DatePicker dtCurrEmpStart1;
		private STL.PL.DatePicker dtDateInCurrentAddress1;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label lMortgage1;
		private System.Windows.Forms.TextBox txtMortgage;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lDOB1;
		private System.Windows.Forms.TextBox txtAddIncome;
		private System.Windows.Forms.TextBox txtCommitments;
		private System.Windows.Forms.TextBox txtNetIncome;
		private System.Windows.Forms.DateTimePicker dtDOB1;
		private System.Windows.Forms.ComboBox drpMaritalStat1;
		private System.Windows.Forms.GroupBox gpAgreement;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.TextBox txtWSettled;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.TextBox txtWCurrent;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox txtSAccounts;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.TextBox txtCAccounts;
		private System.Windows.Forms.Label lExpensiveCode;
		private System.Windows.Forms.TextBox txtExpensiveCode;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox txtRepayment;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox txtTermsType;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox txtInstalment;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox txtExpensiveCategory;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox txtDeposit;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox txtAgrmTotal;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtFinalInstalment;
		private System.Windows.Forms.GroupBox gpCustomer;
		private System.Windows.Forms.TextBox txtCustomerID;
		private System.Windows.Forms.DateTimePicker dtDateProp;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lCustomerID;
		private System.Windows.Forms.TextBox txtLastName;
		private System.Windows.Forms.TextBox txtFirstName;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnAccountDetails;
		private System.Windows.Forms.Button btnComplete;
	
		public bool AccountLocked
		{
			get{return _acctLocked;}
			set{_acctLocked = value;}
		}

		private bool _readOnly = false;
		public bool ReadOnly
		{
			get{return _readOnly;}
			set{_readOnly = value;}
		}
		
		private void HashMenus()
		{
			dynamicMenus[this.Name+":txtCreditLimit"] = this.txtCreditLimit; 
			dynamicMenus[this.Name+":lCreditLimit"] = this.lCreditLimit; 
		}

		private BasicCustomerDetails CustomerScreen = null;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.ToolTip toolTip1;
		private Crownwood.Magic.Menus.MenuCommand menuSave;
		private Crownwood.Magic.Menus.MenuCommand menuReferral;
		private Crownwood.Magic.Menus.MenuCommand menuComplete;
		private System.ComponentModel.IContainer components;
		private bool reOpen = false;
		private string propresult = "";

		public Referral_Test(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile, menuReferral});
		}

		public Referral_Test(string custId, DateTime dateProp, string accountNo, 
			string acctType, string mode, 
			Form root, Form parent, BasicCustomerDetails customerScreen, bool removeCancellation)
		{
			InitializeComponent();
			FormRoot = root;
			FormParent = parent;
			CustomerScreen = customerScreen;

			dynamicMenus = new Hashtable();				
			HashMenus();
			ApplyRoleRestrictions();

			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile, menuReferral});
			this.txtCustomerID.Text = _custid = custId;
			this.dtDateProp.Value = _dateProp = dateProp;
			_acctno = accountNo;
			_acctType = acctType;
			_screenMode = mode;
			reOpen = removeCancellation;

			/*
			this.dtDateInCurrentAddress1.Value	= DateTime.Today;
			this.dtCurrEmpStart1.Value			= DateTime.Today;
			*/
			this.dtDateInCurrentAddress1 = new DatePicker();
			this.dtCurrEmpStart1 = new DatePicker();

			ProtectSummaryData();
		}

		private void ProtectSummaryData()
		{
			foreach(Control c in gpPersonal.Controls)
			{
				DisableControl(c);
			}

			foreach(Control c in gpAgreement.Controls)
			{
				DisableControl(c);
			}
		}

		private void DisableControl(Control c)
		{
			if(c is TextBox)
			{
				((TextBox)c).ReadOnly = true;
				((TextBox)c).BackColor = SystemColors.Window;
			}

			if( c is DateTimePicker ||
				c is ComboBox )
			{
				c.Enabled = false;
				c.BackColor = SystemColors.Window;
			}

			if(c is DatePicker)
			{
				((DatePicker)c).Enabled = false;	
				((DatePicker)c).BackColor = SystemColors.Window;
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

		private void SetReadOnly()
		{
			this.txtCustomerID.BackColor = SystemColors.Window;
			this.txtFirstName.BackColor = SystemColors.Window;
			this.txtLastName.BackColor = SystemColors.Window;
			this.rtxtNewReferralNotes.ReadOnly = ReadOnly;
			if(ReadOnly)
				this.rtxtNewReferralNotes.BackColor = SystemColors.Control;
			else
				this.rtxtNewReferralNotes.BackColor = SystemColors.Window;
			this.noAvail.BackColor = SystemColors.Window;
			this.btnSave.Enabled = !ReadOnly;
			this.btnComplete.Enabled = !ReadOnly;
			this.rbApprove.Enabled = !ReadOnly;
			this.rbReject.Enabled = !ReadOnly;
			this.menuComplete.Enabled = !ReadOnly;
			this.menuSave.Enabled = !ReadOnly;
			if(txtCreditLimit.Enabled)
				txtCreditLimit.ReadOnly = ReadOnly;
		}

		private void CalcCharsAvailable()
		{
			string [] lines = rtxtReferralNotes.Lines;
			int len = 0;
			foreach(string s in lines)
				len += s.Length;

			lines = rtxtNewReferralNotes.Lines;
			foreach(string s in lines)
				len += s.Length;

			if((1000-len)>=0)
				noAvail.Value = 1000 - len;	
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Referral_Test));
			this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
			this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
			this.menuReferral = new Crownwood.Magic.Menus.MenuCommand();
			this.menuComplete = new Crownwood.Magic.Menus.MenuCommand();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tcReferralDetails = new Crownwood.Magic.Controls.TabControl();
			this.tpUnderWriter = new Crownwood.Magic.Controls.TabPage();
			this.tcUnderWriter = new Crownwood.Magic.Controls.TabControl();
			this.tpDetails = new Crownwood.Magic.Controls.TabPage();
			this.drpPolicyRule3 = new System.Windows.Forms.ComboBox();
			this.drpPolicyRule1 = new System.Windows.Forms.ComboBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label18 = new System.Windows.Forms.Label();
			this.txtScore = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.txtRiskCategory = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.txtSysRecommendation = new System.Windows.Forms.TextBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.rtxtAdditionalData = new System.Windows.Forms.RichTextBox();
			this.label20 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.drpAdditionalData = new System.Windows.Forms.ComboBox();
			this.drpPolicyRule5 = new System.Windows.Forms.ComboBox();
			this.drpPolicyRule2 = new System.Windows.Forms.ComboBox();
			this.drpPolicyRule4 = new System.Windows.Forms.ComboBox();
			this.drpPolicyRule6 = new System.Windows.Forms.ComboBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.tpFinalDecision = new Crownwood.Magic.Controls.TabPage();
			this.label24 = new System.Windows.Forms.Label();
			this.drpOverride = new System.Windows.Forms.ComboBox();
			this.label21 = new System.Windows.Forms.Label();
			this.lCreditLimit = new System.Windows.Forms.Label();
			this.txtCreditLimit = new System.Windows.Forms.TextBox();
			this.rtxtNewReferralNotes = new System.Windows.Forms.RichTextBox();
			this.rtxtReferralNotes = new System.Windows.Forms.RichTextBox();
			this.rbReject = new System.Windows.Forms.RadioButton();
			this.rbApprove = new System.Windows.Forms.RadioButton();
			this.noAvail = new System.Windows.Forms.NumericUpDown();
			this.label22 = new System.Windows.Forms.Label();
			this.label23 = new System.Windows.Forms.Label();
			this.tpSummaryInfo = new Crownwood.Magic.Controls.TabPage();
			this.gpPersonal = new System.Windows.Forms.GroupBox();
			this.dtCurrEmpStart1 = new STL.PL.DatePicker();
			this.dtDateInCurrentAddress1 = new STL.PL.DatePicker();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.lMortgage1 = new System.Windows.Forms.Label();
			this.txtMortgage = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.lDOB1 = new System.Windows.Forms.Label();
			this.txtAddIncome = new System.Windows.Forms.TextBox();
			this.txtCommitments = new System.Windows.Forms.TextBox();
			this.txtNetIncome = new System.Windows.Forms.TextBox();
			this.dtDOB1 = new System.Windows.Forms.DateTimePicker();
			this.drpMaritalStat1 = new System.Windows.Forms.ComboBox();
			this.gpAgreement = new System.Windows.Forms.GroupBox();
			this.label26 = new System.Windows.Forms.Label();
			this.txtWSettled = new System.Windows.Forms.TextBox();
			this.label27 = new System.Windows.Forms.Label();
			this.txtWCurrent = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.txtSAccounts = new System.Windows.Forms.TextBox();
			this.label25 = new System.Windows.Forms.Label();
			this.txtCAccounts = new System.Windows.Forms.TextBox();
			this.lExpensiveCode = new System.Windows.Forms.Label();
			this.txtExpensiveCode = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.txtRepayment = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.txtTermsType = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.txtInstalment = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.txtExpensiveCategory = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.txtDeposit = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.txtAgrmTotal = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.txtFinalInstalment = new System.Windows.Forms.TextBox();
			this.gpCustomer = new System.Windows.Forms.GroupBox();
			this.txtCustomerID = new System.Windows.Forms.TextBox();
			this.dtDateProp = new System.Windows.Forms.DateTimePicker();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lCustomerID = new System.Windows.Forms.Label();
			this.txtLastName = new System.Windows.Forms.TextBox();
			this.txtFirstName = new System.Windows.Forms.TextBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnAccountDetails = new System.Windows.Forms.Button();
			this.btnComplete = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.tcReferralDetails.SuspendLayout();
			this.tpUnderWriter.SuspendLayout();
			this.tcUnderWriter.SuspendLayout();
			this.tpDetails.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.tpFinalDecision.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.noAvail)).BeginInit();
			this.tpSummaryInfo.SuspendLayout();
			this.gpPersonal.SuspendLayout();
			this.gpAgreement.SuspendLayout();
			this.gpCustomer.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuExit
			// 
			this.menuExit.Description = "MenuItem";
			this.menuExit.Text = "E&xit";
			// 
			// menuFile
			// 
			this.menuFile.Description = "MenuItem";
			this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
																							this.menuExit});
			this.menuFile.Text = "&File";
			// 
			// menuSave
			// 
			this.menuSave.Description = "MenuItem";
			this.menuSave.Text = "&Save";
			// 
			// menuReferral
			// 
			this.menuReferral.Description = "MenuItem";
			this.menuReferral.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
																								this.menuSave,
																								this.menuComplete});
			this.menuReferral.Text = "&Referral";
			// 
			// menuComplete
			// 
			this.menuComplete.Description = "MenuItem";
			this.menuComplete.Text = "&Complete";
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.SystemColors.ControlLight;
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.tcReferralDetails,
																					this.gpCustomer});
			this.groupBox1.Location = new System.Drawing.Point(8, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(776, 472);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// tcReferralDetails
			// 
			this.tcReferralDetails.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.tcReferralDetails.IDEPixelArea = true;
			this.tcReferralDetails.Location = new System.Drawing.Point(8, 128);
			this.tcReferralDetails.Name = "tcReferralDetails";
			this.tcReferralDetails.PositionTop = true;
			this.tcReferralDetails.SelectedIndex = 0;
			this.tcReferralDetails.SelectedTab = this.tpUnderWriter;
			this.tcReferralDetails.Size = new System.Drawing.Size(760, 336);
			this.tcReferralDetails.TabIndex = 7;
			this.tcReferralDetails.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
																								this.tpUnderWriter,
																								this.tpSummaryInfo});
			// 
			// tpUnderWriter
			// 
			this.tpUnderWriter.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.tcUnderWriter});
			this.tpUnderWriter.Location = new System.Drawing.Point(0, 25);
			this.tpUnderWriter.Name = "tpUnderWriter";
			this.tpUnderWriter.Size = new System.Drawing.Size(760, 311);
			this.tpUnderWriter.TabIndex = 1;
			this.tpUnderWriter.Title = "UW Decision";
			// 
			// tcUnderWriter
			// 
			this.tcUnderWriter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcUnderWriter.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.tcUnderWriter.IDEPixelArea = true;
			this.tcUnderWriter.Name = "tcUnderWriter";
			this.tcUnderWriter.PositionTop = true;
			this.tcUnderWriter.SelectedIndex = 0;
			this.tcUnderWriter.SelectedTab = this.tpDetails;
			this.tcUnderWriter.Size = new System.Drawing.Size(760, 311);
			this.tcUnderWriter.TabIndex = 1;
			this.tcUnderWriter.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
																							this.tpDetails,
																							this.tpFinalDecision});
			// 
			// tpDetails
			// 
			this.tpDetails.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.drpPolicyRule3,
																					this.drpPolicyRule1,
																					this.groupBox2,
																					this.groupBox3,
																					this.drpPolicyRule5,
																					this.drpPolicyRule2,
																					this.drpPolicyRule4,
																					this.drpPolicyRule6,
																					this.groupBox4});
			this.tpDetails.Location = new System.Drawing.Point(0, 25);
			this.tpDetails.Name = "tpDetails";
			this.tpDetails.Size = new System.Drawing.Size(760, 286);
			this.tpDetails.TabIndex = 0;
			this.tpDetails.Title = "Details";
			// 
			// drpPolicyRule3
			// 
			this.drpPolicyRule3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpPolicyRule3.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.drpPolicyRule3.ItemHeight = 14;
			this.drpPolicyRule3.Location = new System.Drawing.Point(272, 26);
			this.drpPolicyRule3.Name = "drpPolicyRule3";
			this.drpPolicyRule3.Size = new System.Drawing.Size(216, 22);
			this.drpPolicyRule3.TabIndex = 36;
			// 
			// drpPolicyRule1
			// 
			this.drpPolicyRule1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpPolicyRule1.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.drpPolicyRule1.ItemHeight = 14;
			this.drpPolicyRule1.Location = new System.Drawing.Point(56, 26);
			this.drpPolicyRule1.Name = "drpPolicyRule1";
			this.drpPolicyRule1.Size = new System.Drawing.Size(216, 22);
			this.drpPolicyRule1.TabIndex = 37;
			// 
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.label18,
																					this.txtScore,
																					this.label17,
																					this.txtRiskCategory,
																					this.label16,
																					this.txtSysRecommendation});
			this.groupBox2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox2.Location = new System.Drawing.Point(8, 88);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(736, 80);
			this.groupBox2.TabIndex = 39;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Score";
			// 
			// label18
			// 
			this.label18.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.label18.BackColor = System.Drawing.Color.Transparent;
			this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label18.Location = new System.Drawing.Point(48, 26);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(40, 19);
			this.label18.TabIndex = 32;
			this.label18.Text = "Score:";
			// 
			// txtScore
			// 
			this.txtScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtScore.Location = new System.Drawing.Point(48, 48);
			this.txtScore.MaxLength = 16;
			this.txtScore.Name = "txtScore";
			this.txtScore.Size = new System.Drawing.Size(80, 20);
			this.txtScore.TabIndex = 31;
			this.txtScore.Tag = "lMoreRewards1";
			this.txtScore.Text = "";
			// 
			// label17
			// 
			this.label17.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.label17.BackColor = System.Drawing.Color.Transparent;
			this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label17.Location = new System.Drawing.Point(168, 26);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(96, 19);
			this.label17.TabIndex = 30;
			this.label17.Text = "Risk Category:";
			// 
			// txtRiskCategory
			// 
			this.txtRiskCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtRiskCategory.Location = new System.Drawing.Point(168, 48);
			this.txtRiskCategory.MaxLength = 16;
			this.txtRiskCategory.Name = "txtRiskCategory";
			this.txtRiskCategory.Size = new System.Drawing.Size(104, 20);
			this.txtRiskCategory.TabIndex = 29;
			this.txtRiskCategory.Tag = "lMoreRewards1";
			this.txtRiskCategory.Text = "";
			// 
			// label16
			// 
			this.label16.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.label16.BackColor = System.Drawing.Color.Transparent;
			this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label16.Location = new System.Drawing.Point(328, 26);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(152, 19);
			this.label16.TabIndex = 28;
			this.label16.Text = "System Recommendation:";
			// 
			// txtSysRecommendation
			// 
			this.txtSysRecommendation.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtSysRecommendation.Location = new System.Drawing.Point(328, 48);
			this.txtSysRecommendation.MaxLength = 16;
			this.txtSysRecommendation.Name = "txtSysRecommendation";
			this.txtSysRecommendation.Size = new System.Drawing.Size(184, 20);
			this.txtSysRecommendation.TabIndex = 27;
			this.txtSysRecommendation.Tag = "lMoreRewards1";
			this.txtSysRecommendation.Text = "";
			// 
			// groupBox3
			// 
			this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox3.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.rtxtAdditionalData,
																					this.label20,
																					this.label19,
																					this.drpAdditionalData});
			this.groupBox3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox3.Location = new System.Drawing.Point(8, 168);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(736, 112);
			this.groupBox3.TabIndex = 40;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Additional Data:";
			// 
			// rtxtAdditionalData
			// 
			this.rtxtAdditionalData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.rtxtAdditionalData.Location = new System.Drawing.Point(168, 48);
			this.rtxtAdditionalData.MaxLength = 1000;
			this.rtxtAdditionalData.Name = "rtxtAdditionalData";
			this.rtxtAdditionalData.Size = new System.Drawing.Size(496, 56);
			this.rtxtAdditionalData.TabIndex = 29;
			this.rtxtAdditionalData.Text = "";
			// 
			// label20
			// 
			this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label20.Location = new System.Drawing.Point(168, 24);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(144, 16);
			this.label20.TabIndex = 28;
			this.label20.Text = "Additional Data Comments:";
			// 
			// label19
			// 
			this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label19.Location = new System.Drawing.Point(16, 24);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(136, 18);
			this.label19.TabIndex = 20;
			this.label19.Text = "Additional Data Required";
			// 
			// drpAdditionalData
			// 
			this.drpAdditionalData.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpAdditionalData.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.drpAdditionalData.ItemHeight = 13;
			this.drpAdditionalData.Location = new System.Drawing.Point(16, 48);
			this.drpAdditionalData.Name = "drpAdditionalData";
			this.drpAdditionalData.Size = new System.Drawing.Size(80, 21);
			this.drpAdditionalData.TabIndex = 19;
			this.drpAdditionalData.Tag = "lMaritalStat1";
			// 
			// drpPolicyRule5
			// 
			this.drpPolicyRule5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpPolicyRule5.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.drpPolicyRule5.ItemHeight = 14;
			this.drpPolicyRule5.Location = new System.Drawing.Point(488, 26);
			this.drpPolicyRule5.Name = "drpPolicyRule5";
			this.drpPolicyRule5.Size = new System.Drawing.Size(216, 22);
			this.drpPolicyRule5.TabIndex = 35;
			// 
			// drpPolicyRule2
			// 
			this.drpPolicyRule2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpPolicyRule2.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.drpPolicyRule2.ItemHeight = 14;
			this.drpPolicyRule2.Location = new System.Drawing.Point(56, 50);
			this.drpPolicyRule2.Name = "drpPolicyRule2";
			this.drpPolicyRule2.Size = new System.Drawing.Size(216, 22);
			this.drpPolicyRule2.TabIndex = 34;
			// 
			// drpPolicyRule4
			// 
			this.drpPolicyRule4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpPolicyRule4.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.drpPolicyRule4.ItemHeight = 14;
			this.drpPolicyRule4.Location = new System.Drawing.Point(272, 50);
			this.drpPolicyRule4.Name = "drpPolicyRule4";
			this.drpPolicyRule4.Size = new System.Drawing.Size(216, 22);
			this.drpPolicyRule4.TabIndex = 33;
			// 
			// drpPolicyRule6
			// 
			this.drpPolicyRule6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpPolicyRule6.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.drpPolicyRule6.ItemHeight = 14;
			this.drpPolicyRule6.Location = new System.Drawing.Point(488, 50);
			this.drpPolicyRule6.Name = "drpPolicyRule6";
			this.drpPolicyRule6.Size = new System.Drawing.Size(216, 22);
			this.drpPolicyRule6.TabIndex = 32;
			// 
			// groupBox4
			// 
			this.groupBox4.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox4.Location = new System.Drawing.Point(8, 8);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(736, 80);
			this.groupBox4.TabIndex = 38;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Policy Rules";
			// 
			// tpFinalDecision
			// 
			this.tpFinalDecision.Controls.AddRange(new System.Windows.Forms.Control[] {
																						  this.label24,
																						  this.drpOverride,
																						  this.label21,
																						  this.lCreditLimit,
																						  this.txtCreditLimit,
																						  this.rtxtNewReferralNotes,
																						  this.rtxtReferralNotes,
																						  this.rbReject,
																						  this.rbApprove,
																						  this.noAvail,
																						  this.label22,
																						  this.label23});
			this.tpFinalDecision.Location = new System.Drawing.Point(0, 25);
			this.tpFinalDecision.Name = "tpFinalDecision";
			this.tpFinalDecision.Selected = false;
			this.tpFinalDecision.Size = new System.Drawing.Size(760, 286);
			this.tpFinalDecision.TabIndex = 1;
			this.tpFinalDecision.Title = "Final Decision";
			this.tpFinalDecision.Visible = false;
			// 
			// label24
			// 
			this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label24.Location = new System.Drawing.Point(32, 16);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(96, 18);
			this.label24.TabIndex = 48;
			this.label24.Text = "Override Reason:";
			// 
			// drpOverride
			// 
			this.drpOverride.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpOverride.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.drpOverride.ItemHeight = 13;
			this.drpOverride.Location = new System.Drawing.Point(32, 40);
			this.drpOverride.Name = "drpOverride";
			this.drpOverride.Size = new System.Drawing.Size(144, 21);
			this.drpOverride.TabIndex = 47;
			this.drpOverride.Tag = "lMaritalStat1";
			// 
			// label21
			// 
			this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label21.Location = new System.Drawing.Point(24, 80);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(48, 16);
			this.label21.TabIndex = 46;
			this.label21.Text = "Notes:";
			// 
			// lCreditLimit
			// 
			this.lCreditLimit.Enabled = false;
			this.lCreditLimit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lCreditLimit.Location = new System.Drawing.Point(224, 24);
			this.lCreditLimit.Name = "lCreditLimit";
			this.lCreditLimit.Size = new System.Drawing.Size(88, 16);
			this.lCreditLimit.TabIndex = 45;
			this.lCreditLimit.Text = "RF Credit Limit:";
			this.lCreditLimit.Visible = false;
			// 
			// txtCreditLimit
			// 
			this.txtCreditLimit.Enabled = false;
			this.txtCreditLimit.Location = new System.Drawing.Point(224, 40);
			this.txtCreditLimit.MaxLength = 15;
			this.txtCreditLimit.Name = "txtCreditLimit";
			this.txtCreditLimit.Size = new System.Drawing.Size(128, 18);
			this.txtCreditLimit.TabIndex = 44;
			this.txtCreditLimit.Text = "";
			this.txtCreditLimit.Visible = false;
			// 
			// rtxtNewReferralNotes
			// 
			this.rtxtNewReferralNotes.Location = new System.Drawing.Point(24, 104);
			this.rtxtNewReferralNotes.MaxLength = 1000;
			this.rtxtNewReferralNotes.Name = "rtxtNewReferralNotes";
			this.rtxtNewReferralNotes.Size = new System.Drawing.Size(496, 68);
			this.rtxtNewReferralNotes.TabIndex = 43;
			this.rtxtNewReferralNotes.Text = "";
			// 
			// rtxtReferralNotes
			// 
			this.rtxtReferralNotes.BackColor = System.Drawing.SystemColors.Control;
			this.rtxtReferralNotes.Location = new System.Drawing.Point(24, 176);
			this.rtxtReferralNotes.MaxLength = 1000;
			this.rtxtReferralNotes.Name = "rtxtReferralNotes";
			this.rtxtReferralNotes.ReadOnly = true;
			this.rtxtReferralNotes.Size = new System.Drawing.Size(496, 104);
			this.rtxtReferralNotes.TabIndex = 40;
			this.rtxtReferralNotes.Text = "";
			// 
			// rbReject
			// 
			this.rbReject.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.rbReject.Location = new System.Drawing.Point(512, 48);
			this.rbReject.Name = "rbReject";
			this.rbReject.TabIndex = 39;
			this.rbReject.Text = "Reject";
			// 
			// rbApprove
			// 
			this.rbApprove.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.rbApprove.Location = new System.Drawing.Point(512, 16);
			this.rbApprove.Name = "rbApprove";
			this.rbApprove.TabIndex = 38;
			this.rbApprove.Text = "Approve";
			// 
			// noAvail
			// 
			this.noAvail.Enabled = false;
			this.noAvail.Location = new System.Drawing.Point(600, 256);
			this.noAvail.Maximum = new System.Decimal(new int[] {
																	1000,
																	0,
																	0,
																	0});
			this.noAvail.Name = "noAvail";
			this.noAvail.ReadOnly = true;
			this.noAvail.Size = new System.Drawing.Size(56, 18);
			this.noAvail.TabIndex = 37;
			this.noAvail.Value = new System.Decimal(new int[] {
																  1000,
																  0,
																  0,
																  0});
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(528, 248);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(64, 32);
			this.label22.TabIndex = 36;
			this.label22.Text = "Characters available:";
			this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label23
			// 
			this.label23.Location = new System.Drawing.Point(48, 102);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(48, 16);
			this.label23.TabIndex = 32;
			this.label23.Text = "Notes:";
			// 
			// tpSummaryInfo
			// 
			this.tpSummaryInfo.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.gpPersonal,
																						this.gpAgreement});
			this.tpSummaryInfo.Location = new System.Drawing.Point(0, 25);
			this.tpSummaryInfo.Name = "tpSummaryInfo";
			this.tpSummaryInfo.Selected = false;
			this.tpSummaryInfo.Size = new System.Drawing.Size(760, 311);
			this.tpSummaryInfo.TabIndex = 0;
			this.tpSummaryInfo.Title = "Summary Info";
			this.tpSummaryInfo.Visible = false;
			// 
			// gpPersonal
			// 
			this.gpPersonal.BackColor = System.Drawing.SystemColors.Control;
			this.gpPersonal.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.dtCurrEmpStart1,
																					 this.dtDateInCurrentAddress1,
																					 this.label7,
																					 this.label6,
																					 this.label5,
																					 this.lMortgage1,
																					 this.txtMortgage,
																					 this.label1,
																					 this.lDOB1,
																					 this.txtAddIncome,
																					 this.txtCommitments,
																					 this.txtNetIncome,
																					 this.dtDOB1,
																					 this.drpMaritalStat1});
			this.gpPersonal.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.gpPersonal.Location = new System.Drawing.Point(16, 0);
			this.gpPersonal.Name = "gpPersonal";
			this.gpPersonal.Size = new System.Drawing.Size(736, 152);
			this.gpPersonal.TabIndex = 16;
			this.gpPersonal.TabStop = false;
			this.gpPersonal.Text = "Personal Details";
			// 
			// dtCurrEmpStart1
			// 
			this.dtCurrEmpStart1.DateFrom = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
			this.dtCurrEmpStart1.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.dtCurrEmpStart1.Label = "Curr. Emp. started:";
			this.dtCurrEmpStart1.LinkedBias = false;
			this.dtCurrEmpStart1.LinkedComboBox = null;
			this.dtCurrEmpStart1.LinkedDatePicker = null;
			this.dtCurrEmpStart1.LinkedLabel = null;
			this.dtCurrEmpStart1.Location = new System.Drawing.Point(472, 88);
			this.dtCurrEmpStart1.Months = new System.Decimal(new int[] {
																		   0,
																		   0,
																		   0,
																		   0});
			this.dtCurrEmpStart1.Name = "dtCurrEmpStart1";
			this.dtCurrEmpStart1.Size = new System.Drawing.Size(256, 56);
			this.dtCurrEmpStart1.TabIndex = 21;
			this.dtCurrEmpStart1.Tag = "dtCurrEmpStart1";
			this.dtCurrEmpStart1.Value = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
			this.dtCurrEmpStart1.Years = new System.Decimal(new int[] {
																		  0,
																		  0,
																		  0,
																		  0});
			// 
			// dtDateInCurrentAddress1
			// 
			this.dtDateInCurrentAddress1.DateFrom = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
			this.dtDateInCurrentAddress1.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.dtDateInCurrentAddress1.Label = "Date In Curr Address:";
			this.dtDateInCurrentAddress1.LinkedBias = true;
			this.dtDateInCurrentAddress1.LinkedComboBox = null;
			this.dtDateInCurrentAddress1.LinkedDatePicker = null;
			this.dtDateInCurrentAddress1.LinkedLabel = null;
			this.dtDateInCurrentAddress1.Location = new System.Drawing.Point(472, 24);
			this.dtDateInCurrentAddress1.Months = new System.Decimal(new int[] {
																				   0,
																				   0,
																				   0,
																				   0});
			this.dtDateInCurrentAddress1.Name = "dtDateInCurrentAddress1";
			this.dtDateInCurrentAddress1.Size = new System.Drawing.Size(256, 56);
			this.dtDateInCurrentAddress1.TabIndex = 20;
			this.dtDateInCurrentAddress1.Tag = "dtDateInCurrentAddress1";
			this.dtDateInCurrentAddress1.Value = new System.DateTime(2002, 11, 19, 0, 0, 0, 0);
			this.dtDateInCurrentAddress1.Years = new System.Decimal(new int[] {
																				  0,
																				  0,
																				  0,
																				  0});
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label7.Location = new System.Drawing.Point(336, 96);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(104, 18);
			this.label7.TabIndex = 19;
			this.label7.Text = "Additional Income:";
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label6.Location = new System.Drawing.Point(336, 24);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(80, 18);
			this.label6.TabIndex = 18;
			this.label6.Text = "Marital Status:";
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label5.Location = new System.Drawing.Point(192, 96);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(112, 18);
			this.label5.TabIndex = 17;
			this.label5.Text = "Other Commitments:";
			// 
			// lMortgage1
			// 
			this.lMortgage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.lMortgage1.Location = new System.Drawing.Point(192, 24);
			this.lMortgage1.Name = "lMortgage1";
			this.lMortgage1.Size = new System.Drawing.Size(88, 18);
			this.lMortgage1.TabIndex = 15;
			this.lMortgage1.Text = "Mortgage/Rent:";
			// 
			// txtMortgage
			// 
			this.txtMortgage.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtMortgage.Location = new System.Drawing.Point(192, 48);
			this.txtMortgage.Name = "txtMortgage";
			this.txtMortgage.Size = new System.Drawing.Size(80, 20);
			this.txtMortgage.TabIndex = 16;
			this.txtMortgage.Tag = "lMortgage1";
			this.txtMortgage.Text = "";
			// 
			// label1
			// 
			this.label1.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(24, 96);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 19);
			this.label1.TabIndex = 14;
			this.label1.Text = "Income After Tax:";
			// 
			// lDOB1
			// 
			this.lDOB1.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.lDOB1.BackColor = System.Drawing.Color.Transparent;
			this.lDOB1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.lDOB1.Location = new System.Drawing.Point(24, 24);
			this.lDOB1.Name = "lDOB1";
			this.lDOB1.Size = new System.Drawing.Size(72, 19);
			this.lDOB1.TabIndex = 13;
			this.lDOB1.Text = "Date of Birth:";
			// 
			// txtAddIncome
			// 
			this.txtAddIncome.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtAddIncome.Location = new System.Drawing.Point(336, 120);
			this.txtAddIncome.MaxLength = 16;
			this.txtAddIncome.Name = "txtAddIncome";
			this.txtAddIncome.Size = new System.Drawing.Size(80, 20);
			this.txtAddIncome.TabIndex = 12;
			this.txtAddIncome.Tag = "lMoreRewards1";
			this.txtAddIncome.Text = "";
			// 
			// txtCommitments
			// 
			this.txtCommitments.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtCommitments.Location = new System.Drawing.Point(192, 120);
			this.txtCommitments.MaxLength = 16;
			this.txtCommitments.Name = "txtCommitments";
			this.txtCommitments.Size = new System.Drawing.Size(80, 20);
			this.txtCommitments.TabIndex = 10;
			this.txtCommitments.Tag = "lMoreRewards1";
			this.txtCommitments.Text = "";
			// 
			// txtNetIncome
			// 
			this.txtNetIncome.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtNetIncome.Location = new System.Drawing.Point(24, 120);
			this.txtNetIncome.MaxLength = 16;
			this.txtNetIncome.Name = "txtNetIncome";
			this.txtNetIncome.Size = new System.Drawing.Size(80, 20);
			this.txtNetIncome.TabIndex = 9;
			this.txtNetIncome.Tag = "lMoreRewards1";
			this.txtNetIncome.Text = "";
			// 
			// dtDOB1
			// 
			this.dtDOB1.CustomFormat = "ddd dd MMM yyyy";
			this.dtDOB1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.dtDOB1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtDOB1.Location = new System.Drawing.Point(24, 48);
			this.dtDOB1.Name = "dtDOB1";
			this.dtDOB1.Size = new System.Drawing.Size(131, 20);
			this.dtDOB1.TabIndex = 6;
			this.dtDOB1.Tag = "lDOB1";
			this.dtDOB1.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
			// 
			// drpMaritalStat1
			// 
			this.drpMaritalStat1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpMaritalStat1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.drpMaritalStat1.ItemHeight = 13;
			this.drpMaritalStat1.Location = new System.Drawing.Point(336, 48);
			this.drpMaritalStat1.Name = "drpMaritalStat1";
			this.drpMaritalStat1.Size = new System.Drawing.Size(80, 21);
			this.drpMaritalStat1.TabIndex = 7;
			this.drpMaritalStat1.Tag = "lMaritalStat1";
			// 
			// gpAgreement
			// 
			this.gpAgreement.BackColor = System.Drawing.SystemColors.Control;
			this.gpAgreement.Controls.AddRange(new System.Windows.Forms.Control[] {
																					  this.label26,
																					  this.txtWSettled,
																					  this.label27,
																					  this.txtWCurrent,
																					  this.label15,
																					  this.txtSAccounts,
																					  this.label25,
																					  this.txtCAccounts,
																					  this.lExpensiveCode,
																					  this.txtExpensiveCode,
																					  this.label14,
																					  this.txtRepayment,
																					  this.label13,
																					  this.txtTermsType,
																					  this.label12,
																					  this.txtInstalment,
																					  this.label11,
																					  this.txtExpensiveCategory,
																					  this.label10,
																					  this.txtDeposit,
																					  this.label9,
																					  this.txtAgrmTotal,
																					  this.label8,
																					  this.txtFinalInstalment});
			this.gpAgreement.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.gpAgreement.Location = new System.Drawing.Point(16, 152);
			this.gpAgreement.Name = "gpAgreement";
			this.gpAgreement.Size = new System.Drawing.Size(736, 152);
			this.gpAgreement.TabIndex = 14;
			this.gpAgreement.TabStop = false;
			this.gpAgreement.Text = "Agreement";
			// 
			// label26
			// 
			this.label26.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.label26.BackColor = System.Drawing.Color.Transparent;
			this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label26.Location = new System.Drawing.Point(632, 80);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(88, 32);
			this.label26.TabIndex = 38;
			this.label26.Text = "Worst Settled Status:";
			// 
			// txtWSettled
			// 
			this.txtWSettled.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtWSettled.Location = new System.Drawing.Point(632, 120);
			this.txtWSettled.MaxLength = 16;
			this.txtWSettled.Name = "txtWSettled";
			this.txtWSettled.Size = new System.Drawing.Size(80, 20);
			this.txtWSettled.TabIndex = 37;
			this.txtWSettled.Tag = "";
			this.txtWSettled.Text = "";
			// 
			// label27
			// 
			this.label27.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.label27.BackColor = System.Drawing.Color.Transparent;
			this.label27.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label27.Location = new System.Drawing.Point(632, 16);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(80, 32);
			this.label27.TabIndex = 36;
			this.label27.Text = "Worst Current Status:";
			// 
			// txtWCurrent
			// 
			this.txtWCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtWCurrent.Location = new System.Drawing.Point(632, 48);
			this.txtWCurrent.MaxLength = 16;
			this.txtWCurrent.Name = "txtWCurrent";
			this.txtWCurrent.Size = new System.Drawing.Size(80, 20);
			this.txtWCurrent.TabIndex = 35;
			this.txtWCurrent.Tag = "";
			this.txtWCurrent.Text = "";
			// 
			// label15
			// 
			this.label15.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.label15.BackColor = System.Drawing.Color.Transparent;
			this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label15.Location = new System.Drawing.Point(512, 80);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(64, 32);
			this.label15.TabIndex = 34;
			this.label15.Text = "Number Of Settled";
			// 
			// txtSAccounts
			// 
			this.txtSAccounts.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtSAccounts.Location = new System.Drawing.Point(512, 120);
			this.txtSAccounts.MaxLength = 16;
			this.txtSAccounts.Name = "txtSAccounts";
			this.txtSAccounts.Size = new System.Drawing.Size(80, 20);
			this.txtSAccounts.TabIndex = 33;
			this.txtSAccounts.Tag = "lMoreRewards1";
			this.txtSAccounts.Text = "";
			// 
			// label25
			// 
			this.label25.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.label25.BackColor = System.Drawing.Color.Transparent;
			this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label25.Location = new System.Drawing.Point(512, 16);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(64, 32);
			this.label25.TabIndex = 32;
			this.label25.Text = "Number Of Current:";
			// 
			// txtCAccounts
			// 
			this.txtCAccounts.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtCAccounts.Location = new System.Drawing.Point(512, 48);
			this.txtCAccounts.MaxLength = 16;
			this.txtCAccounts.Name = "txtCAccounts";
			this.txtCAccounts.Size = new System.Drawing.Size(80, 20);
			this.txtCAccounts.TabIndex = 31;
			this.txtCAccounts.Tag = "lMoreRewards1";
			this.txtCAccounts.Text = "";
			// 
			// lExpensiveCode
			// 
			this.lExpensiveCode.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.lExpensiveCode.BackColor = System.Drawing.Color.Transparent;
			this.lExpensiveCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.lExpensiveCode.Location = new System.Drawing.Point(392, 80);
			this.lExpensiveCode.Name = "lExpensiveCode";
			this.lExpensiveCode.Size = new System.Drawing.Size(88, 32);
			this.lExpensiveCode.TabIndex = 30;
			this.lExpensiveCode.Text = "Most Expensive Product: Code";
			// 
			// txtExpensiveCode
			// 
			this.txtExpensiveCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtExpensiveCode.Location = new System.Drawing.Point(392, 118);
			this.txtExpensiveCode.MaxLength = 16;
			this.txtExpensiveCode.Name = "txtExpensiveCode";
			this.txtExpensiveCode.Size = new System.Drawing.Size(80, 20);
			this.txtExpensiveCode.TabIndex = 29;
			this.txtExpensiveCode.Tag = "lMoreRewards1";
			this.txtExpensiveCode.Text = "";
			// 
			// label14
			// 
			this.label14.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.label14.BackColor = System.Drawing.Color.Transparent;
			this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label14.Location = new System.Drawing.Point(16, 16);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(96, 24);
			this.label14.TabIndex = 28;
			this.label14.Text = "Repayment as % of income:";
			// 
			// txtRepayment
			// 
			this.txtRepayment.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtRepayment.Location = new System.Drawing.Point(16, 48);
			this.txtRepayment.MaxLength = 16;
			this.txtRepayment.Name = "txtRepayment";
			this.txtRepayment.Size = new System.Drawing.Size(80, 20);
			this.txtRepayment.TabIndex = 27;
			this.txtRepayment.Tag = "lMoreRewards1";
			this.txtRepayment.Text = "";
			// 
			// label13
			// 
			this.label13.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.label13.BackColor = System.Drawing.Color.Transparent;
			this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label13.Location = new System.Drawing.Point(16, 94);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(72, 19);
			this.label13.TabIndex = 26;
			this.label13.Text = "Terms Type:";
			// 
			// txtTermsType
			// 
			this.txtTermsType.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtTermsType.Location = new System.Drawing.Point(16, 118);
			this.txtTermsType.MaxLength = 16;
			this.txtTermsType.Name = "txtTermsType";
			this.txtTermsType.Size = new System.Drawing.Size(80, 20);
			this.txtTermsType.TabIndex = 25;
			this.txtTermsType.Tag = "lMoreRewards1";
			this.txtTermsType.Text = "";
			// 
			// label12
			// 
			this.label12.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.label12.BackColor = System.Drawing.Color.Transparent;
			this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label12.Location = new System.Drawing.Point(136, 24);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(104, 19);
			this.label12.TabIndex = 24;
			this.label12.Text = "Monthly Instalment:";
			// 
			// txtInstalment
			// 
			this.txtInstalment.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtInstalment.Location = new System.Drawing.Point(136, 48);
			this.txtInstalment.MaxLength = 16;
			this.txtInstalment.Name = "txtInstalment";
			this.txtInstalment.Size = new System.Drawing.Size(80, 20);
			this.txtInstalment.TabIndex = 23;
			this.txtInstalment.Tag = "lMoreRewards1";
			this.txtInstalment.Text = "";
			// 
			// label11
			// 
			this.label11.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.label11.BackColor = System.Drawing.Color.Transparent;
			this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label11.Location = new System.Drawing.Point(392, 16);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(96, 32);
			this.label11.TabIndex = 22;
			this.label11.Text = "Most Expensive Product Category:";
			// 
			// txtExpensiveCategory
			// 
			this.txtExpensiveCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtExpensiveCategory.Location = new System.Drawing.Point(392, 48);
			this.txtExpensiveCategory.MaxLength = 16;
			this.txtExpensiveCategory.Name = "txtExpensiveCategory";
			this.txtExpensiveCategory.Size = new System.Drawing.Size(80, 20);
			this.txtExpensiveCategory.TabIndex = 21;
			this.txtExpensiveCategory.Tag = "lMoreRewards1";
			this.txtExpensiveCategory.Text = "";
			// 
			// label10
			// 
			this.label10.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.label10.BackColor = System.Drawing.Color.Transparent;
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label10.Location = new System.Drawing.Point(264, 24);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(48, 19);
			this.label10.TabIndex = 20;
			this.label10.Text = "Deposit:";
			// 
			// txtDeposit
			// 
			this.txtDeposit.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtDeposit.Location = new System.Drawing.Point(264, 48);
			this.txtDeposit.MaxLength = 16;
			this.txtDeposit.Name = "txtDeposit";
			this.txtDeposit.Size = new System.Drawing.Size(80, 20);
			this.txtDeposit.TabIndex = 19;
			this.txtDeposit.Tag = "lMoreRewards1";
			this.txtDeposit.Text = "";
			// 
			// label9
			// 
			this.label9.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.label9.BackColor = System.Drawing.Color.Transparent;
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label9.Location = new System.Drawing.Point(264, 88);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(96, 19);
			this.label9.TabIndex = 18;
			this.label9.Text = "Agreement Total:";
			// 
			// txtAgrmTotal
			// 
			this.txtAgrmTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtAgrmTotal.Location = new System.Drawing.Point(264, 118);
			this.txtAgrmTotal.MaxLength = 16;
			this.txtAgrmTotal.Name = "txtAgrmTotal";
			this.txtAgrmTotal.Size = new System.Drawing.Size(80, 20);
			this.txtAgrmTotal.TabIndex = 17;
			this.txtAgrmTotal.Tag = "lMoreRewards1";
			this.txtAgrmTotal.Text = "";
			// 
			// label8
			// 
			this.label8.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.label8.BackColor = System.Drawing.Color.Transparent;
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label8.Location = new System.Drawing.Point(136, 88);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(96, 19);
			this.label8.TabIndex = 16;
			this.label8.Text = "Final Instalment:";
			// 
			// txtFinalInstalment
			// 
			this.txtFinalInstalment.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.txtFinalInstalment.Location = new System.Drawing.Point(136, 118);
			this.txtFinalInstalment.MaxLength = 16;
			this.txtFinalInstalment.Name = "txtFinalInstalment";
			this.txtFinalInstalment.Size = new System.Drawing.Size(80, 20);
			this.txtFinalInstalment.TabIndex = 15;
			this.txtFinalInstalment.Tag = "lMoreRewards1";
			this.txtFinalInstalment.Text = "";
			// 
			// gpCustomer
			// 
			this.gpCustomer.BackColor = System.Drawing.SystemColors.Control;
			this.gpCustomer.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.txtCustomerID,
																					 this.dtDateProp,
																					 this.label4,
																					 this.label3,
																					 this.label2,
																					 this.lCustomerID,
																					 this.txtLastName,
																					 this.txtFirstName,
																					 this.btnSave,
																					 this.btnAccountDetails,
																					 this.btnComplete});
			this.gpCustomer.Location = new System.Drawing.Point(8, 8);
			this.gpCustomer.Name = "gpCustomer";
			this.gpCustomer.Size = new System.Drawing.Size(760, 112);
			this.gpCustomer.TabIndex = 6;
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
			this.txtCustomerID.Text = "";
			// 
			// dtDateProp
			// 
			this.dtDateProp.CustomFormat = "ddd dd MMM yyyy";
			this.dtDateProp.Enabled = false;
			this.dtDateProp.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtDateProp.Location = new System.Drawing.Point(632, 40);
			this.dtDateProp.Name = "dtDateProp";
			this.dtDateProp.Size = new System.Drawing.Size(112, 20);
			this.dtDateProp.TabIndex = 23;
			this.dtDateProp.Tag = "";
			this.dtDateProp.Value = new System.DateTime(2002, 5, 21, 0, 0, 0, 0);
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label4.Location = new System.Drawing.Point(632, 24);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(104, 16);
			this.label4.TabIndex = 8;
			this.label4.Text = "Date of Application:";
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.Location = new System.Drawing.Point(360, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(72, 16);
			this.label3.TabIndex = 6;
			this.label3.Text = "Last Name:";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(168, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "First Name:";
			// 
			// lCustomerID
			// 
			this.lCustomerID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lCustomerID.Location = new System.Drawing.Point(16, 24);
			this.lCustomerID.Name = "lCustomerID";
			this.lCustomerID.Size = new System.Drawing.Size(72, 16);
			this.lCustomerID.TabIndex = 4;
			this.lCustomerID.Text = "Customer:";
			// 
			// txtLastName
			// 
			this.txtLastName.Location = new System.Drawing.Point(360, 40);
			this.txtLastName.MaxLength = 60;
			this.txtLastName.Name = "txtLastName";
			this.txtLastName.ReadOnly = true;
			this.txtLastName.Size = new System.Drawing.Size(248, 20);
			this.txtLastName.TabIndex = 2;
			this.txtLastName.Text = "";
			// 
			// txtFirstName
			// 
			this.txtFirstName.Location = new System.Drawing.Point(168, 40);
			this.txtFirstName.MaxLength = 30;
			this.txtFirstName.Name = "txtFirstName";
			this.txtFirstName.ReadOnly = true;
			this.txtFirstName.Size = new System.Drawing.Size(160, 20);
			this.txtFirstName.TabIndex = 1;
			this.txtFirstName.Text = "";
			// 
			// btnSave
			// 
			this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnSave.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnSave.Image")));
			this.btnSave.Location = new System.Drawing.Point(672, 72);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(24, 26);
			this.btnSave.TabIndex = 33;
			// 
			// btnAccountDetails
			// 
			this.btnAccountDetails.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnAccountDetails.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnAccountDetails.Image")));
			this.btnAccountDetails.Location = new System.Drawing.Point(632, 72);
			this.btnAccountDetails.Name = "btnAccountDetails";
			this.btnAccountDetails.Size = new System.Drawing.Size(24, 26);
			this.btnAccountDetails.TabIndex = 35;
			// 
			// btnComplete
			// 
			this.btnComplete.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnComplete.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnComplete.Image")));
			this.btnComplete.Location = new System.Drawing.Point(712, 72);
			this.btnComplete.Name = "btnComplete";
			this.btnComplete.Size = new System.Drawing.Size(24, 26);
			this.btnComplete.TabIndex = 34;
			// 
			// Referral_Test
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(792, 476);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox1});
			this.Name = "Referral_Test";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Referral_Test";
			this.Leave += new System.EventHandler(this.Referral_Test_Leave);
			this.Load += new System.EventHandler(this.Referral_Test_Load);
			this.Enter += new System.EventHandler(this.Referral_Test_Enter);
			this.groupBox1.ResumeLayout(false);
			this.tcReferralDetails.ResumeLayout(false);
			this.tpUnderWriter.ResumeLayout(false);
			this.tcUnderWriter.ResumeLayout(false);
			this.tpDetails.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.tpFinalDecision.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.noAvail)).EndInit();
			this.tpSummaryInfo.ResumeLayout(false);
			this.gpPersonal.ResumeLayout(false);
			this.gpAgreement.ResumeLayout(false);
			this.gpCustomer.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void Referral_Test_Enter(object sender, System.EventArgs e)
		{
			((MainForm)this.FormRoot).tbSanction.CustomerScreen = CustomerScreen;
			((MainForm)this.FormRoot).tbSanction.Settled = false;
         ((MainForm)this.FormRoot).tbSanction.Load(true, this.CustomerID, this.DateProp, this.AccountNo, this.AccountType, this.ScreenMode);
			CurrentStatus = ((MainForm)this.FormRoot).tbSanction.CurrentStatus;
			//((MainForm)this.FormRoot).tbSanction.SetCurrentStage(SS.R);
			((MainForm)this.FormRoot).tbSanction.Visible = true;
			ReadOnly = ((MainForm)this.FormRoot).tbSanction.ReadOnly(SS.R);
			SetReadOnly();
		}

		private void Referral_Test_Leave(object sender, System.EventArgs e)
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
				if(Error.Length>0)
				{
					status = false;
					ShowError(Error);				
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
			}
			return status;
		}

		private void Referral_Test_Load(object sender, System.EventArgs e)
		{
			toolTip1.SetToolTip(this.btnAccountDetails, GetResource("TT_ACCOUNTDETAILS"));
			toolTip1.SetToolTip(this.btnComplete, GetResource("TT_COMPLETE"));
			toolTip1.SetToolTip(this.btnSave, GetResource("TT_SAVE"));

			bool status = true;	

			try
			{
				Wait();
				if(LockAccount())
				{
					LoadStatic();

					DataSet refer = CreditManager.GetReferralData(this.CustomerID,
						this.AccountNo,
						this.DateProp,
						Config.CountryCode,
						out Error);
					if(Error.Length>0)
					{
						ShowError(Error);
						status = false;
					}
					else
					{
						foreach(DataTable dt in refer.Tables)
						{
							switch(dt.TableName)
							{
								case TN.ReferralData:	
									LoadReferralDetails(dt);
									break;
								case TN.ProposalRef:		
									LoadProposalRef(dt);
									break;
								default:
									break;
							}
						}
					}

					if(status)
					{
						DataSet prop = CreditManager.GetProposalStage1(this.CustomerID, this.AccountNo, SM.New, "H", out Error);
						if(Error.Length>0)
						{
							ShowError(Error);
							status = false;
						}
						else
						{
							foreach(DataTable dt in prop.Tables)
							{
								switch(dt.TableName)
								{
									case TN.Customer:	
										LoadCustomerDetails(dt);
										break;
									case TN.Proposal:		
										LoadProposalDetails(dt);
										break;
									case TN.Employment:		
										LoadEmploymentDetails(dt);
										break;
									case TN.AccountTotals:		
										LoadAccountDetails(dt);
										break;
									default:
										break;
								}
							}
						}

						/*DataSet agreement = AccountManager.GetAgreement(this.AccountNo, out Error);
						if(Error.Length>0)
						{
							ShowError(Error);
							status = false;
						}
						else
						{
							foreach(DataTable dt in agreement.Tables)
							{
								switch(dt.TableName)
								{
									case TN.Agreements:	
										LoadAgreementDetails(dt);
										break;
									default:
										break;
								}
							}
						}*/
					}
				}
			}
			catch(SoapException ex)
			{
				Catch(ex, Function);
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				this.CalcCharsAvailable();
				StopWait();
			}	
		}

		private bool LockAccount()
		{
			if(this.AccountNo.Length!=0)
			{
				AccountLocked = AccountManager.LockAccount(this.AccountNo, Credential.UserId.ToString(), out Error);
				if(Error.Length>0)
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

        //private void rtxtNewReferralNotes_TextChanged(object sender, System.EventArgs e)
        //{
        //    CalcCharsAvailable();
        //}

		private void LoadStatic()
		{
			XmlUtilities xml = new XmlUtilities();
			XmlDocument dropDowns = new XmlDocument();
			dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
			
			drpAdditionalData.DataSource = new string [] {"", "No", "Yes"};

			if(StaticData.Tables[TN.MaritalStatus]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.MaritalStatus, new string[]{"MS1", "L"}));
			if(StaticData.Tables[TN.PolicyRules]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.PolicyRules, new string[]{"PR1", "L"}));
			if(StaticData.Tables[TN.Reasons]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.Reasons, new string[]{"OV1", "L"}));

			if(dropDowns.DocumentElement.ChildNodes.Count>0)
			{
				DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
				if(Error.Length>0)
				{
					ShowError(Error);
				}
				else
				{
					foreach(DataTable dt in ds.Tables)
						StaticData.Tables[dt.TableName] = dt;
				}
			}

			drpPolicyRule1.DataSource = (DataTable)StaticData.Tables[TN.PolicyRules];
			drpPolicyRule1.DisplayMember = CN.CodeDescription;
			
			drpPolicyRule2.DataSource = (DataTable)StaticData.Tables[TN.PolicyRules];
			drpPolicyRule2.DisplayMember = CN.CodeDescription;
			
			drpPolicyRule3.DataSource = (DataTable)StaticData.Tables[TN.PolicyRules];
			drpPolicyRule3.DisplayMember = CN.CodeDescription;
			
			drpPolicyRule4.DataSource = (DataTable)StaticData.Tables[TN.PolicyRules];
			drpPolicyRule4.DisplayMember = CN.CodeDescription;
			
			drpPolicyRule5.DataSource = (DataTable)StaticData.Tables[TN.PolicyRules];
			drpPolicyRule5.DisplayMember = CN.CodeDescription;
			
			drpPolicyRule6.DataSource = (DataTable)StaticData.Tables[TN.PolicyRules];
			drpPolicyRule6.DisplayMember = CN.CodeDescription;

			drpMaritalStat1.DataSource = (DataTable)StaticData.Tables[TN.MaritalStatus];
			drpMaritalStat1.DisplayMember = CN.CodeDescription;

			drpOverride.DataSource = (DataTable)StaticData.Tables[TN.Reasons];
			drpOverride.DisplayMember = CN.CodeDescription;
		}

		private void LoadReferralDetails(DataTable dt)
		{
			foreach(DataRow r in dt.Rows)
			{
				propresult = (string)r[CN.PropResult];
				txtCustomerID.Text = (string)r[CN.CustomerID];
				txtFirstName.Text = (string)r[CN.FirstName];
				txtLastName.Text = (string)r[CN.LastName];
				rtxtReferralNotes.Text = (string)r[CN.PropNotes];
				rtxtNewReferralNotes.Text = "";
				rbApprove.Checked = propresult == "A"?true:false;
				rbReject.Checked = propresult == "X"?true:false;
				txtScore.Text = Convert.ToString(r[CN.Score]);
				txtCreditLimit.Text = ((decimal)r[CN.RFCreditLimit]).ToString(DecimalPlaces);
			}
		}
		
		private void LoadProposalRef(DataTable dt)
		{
			foreach(DataRow row in dt.Rows)
			{
				foreach(DataRowView r in drpPolicyRule1.Items)
				{
					if((string)r[CN.Code] == ((string)row[CN.PolicyRule1]).Trim())
					{
						drpPolicyRule1.SelectedItem = r;
						break;
					}
				}
				foreach(DataRowView r in drpPolicyRule2.Items)
				{
					if((string)r[CN.Code] == ((string)row[CN.PolicyRule2]).Trim())
					{
						drpPolicyRule2.SelectedItem = r;
						break;
					}
				}
				foreach(DataRowView r in drpPolicyRule3.Items)
				{
					if((string)r[CN.Code] == ((string)row[CN.PolicyRule3]).Trim())
					{
						drpPolicyRule3.SelectedItem = r;
						break;
					}
				}
				foreach(DataRowView r in drpPolicyRule4.Items)
				{
					if((string)r[CN.Code] == ((string)row[CN.PolicyRule4]).Trim())
					{
						drpPolicyRule4.SelectedItem = r;
						break;
					}
				}
				foreach(DataRowView r in drpPolicyRule5.Items)
				{
					if((string)r[CN.Code] == ((string)row[CN.PolicyRule5]).Trim())
					{
						drpPolicyRule5.SelectedItem = r;
						break;
					}
				}
				foreach(DataRowView r in drpPolicyRule6.Items)
				{
					if((string)r[CN.Code] == ((string)row[CN.PolicyRule6]).Trim())
					{
						drpPolicyRule6.SelectedItem = r;
						break;
					}
				}
				foreach(DataRowView r in drpAdditionalData.Items)
				{
					if((string)r[CN.Code] == ((string)row[CN.ADReqd]).Trim())
					{
						drpAdditionalData.SelectedItem = r;
						break;
					}
				}
				foreach(DataRowView r in drpOverride.Items)
				{
					if((string)r[CN.Code] == ((string)row[CN.Override]).Trim())
					{
						drpOverride.SelectedItem = r;
						break;
					}
				}

				txtRiskCategory.Text = (string)row[CN.RiskCat];
				txtSysRecommendation.Text = (string)row[CN.SysRecommend];
				rtxtAdditionalData.Text = (string)row[CN.ADComment];
				txtExpensiveCategory.Text = (string)row[CN.ProdCat];
				txtExpensiveCode.Text = (string)row[CN.ProdCode];
				txtWCurrent.Text = (string)row[CN.CurWorst];
				txtWSettled.Text = (string)row[CN.SetWorst];
			}
		}

		private void LoadCustomerDetails(DataTable dt)
		{
			foreach(DataRow row in dt.Rows)
			{
				dtDOB1.Value = (DateTime)row[CN.DOB];

				if(DBNull.Value!=row[CN.MonthlyRent])
					this.txtMortgage.Text = ((double)row[CN.MonthlyRent]).ToString(DecimalPlaces);
				
				if((DateTime)row[CN.DateIn] > DatePicker.MinValue)
					dtDateInCurrentAddress1.Value = (DateTime)row[CN.DateIn];
			}
		}

		private void LoadProposalDetails(DataTable dt)
		{
			
			drpMaritalStat1.SelectedIndex = 0;

			foreach(DataRow row in dt.Rows)
			{
				foreach(DataRowView r in drpMaritalStat1.Items)
				{
					if((string)r[CN.Code] == ((string)row[CN.MaritalStatus]).Trim())
					{
						drpMaritalStat1.SelectedItem = r;
						break;
					}
				}

				if(row[CN.MonthlyIncome]!= DBNull.Value)
					txtNetIncome.Text = ((double)row[CN.MonthlyIncome]).ToString(DecimalPlaces);
				else
					txtNetIncome.Text = "";

				if(DBNull.Value != row[CN.AdditionalIncome])
					txtAddIncome.Text = ((decimal)row[CN.AdditionalIncome]).ToString(DecimalPlaces);
				else
					txtAddIncome.Text = "";

				if(DBNull.Value!=row[CN.OtherPayments])
					txtCommitments.Text = ((double)row[CN.OtherPayments]).ToString(DecimalPlaces);
			}
		}

		private void LoadEmploymentDetails(DataTable dt)
		{
			foreach(DataRow row in dt.Rows)
			{
				if((DateTime)row[CN.DateEmployed]>DatePicker.MinValue)
					dtCurrEmpStart1.Value = (DateTime)row[CN.DateEmployed];
			}
		}

        //private void LoadAgreementDetails(DataTable dt)
        //{
        //    foreach(DataRow row in dt.Rows)
        //    {
        //        if(DBNull.Value!=row["Terms Type"])
        //            txtTermsType.Text = (string)row["Terms Type"];

        //        if(DBNull.Value!=row["Deposit"])
        //            txtDeposit.Text = ((decimal)row["Deposit"]).ToString(DecimalPlaces);
        //        else
        //            txtDeposit.Text = (0).ToString(DecimalPlaces);

        //        if(DBNull.Value!=row["Instalment Amount"])
        //            txtInstalment.Text = ((decimal)row["Instalment Amount"]).ToString(DecimalPlaces);
        //        else
        //            txtInstalment.Text = (0).ToString(DecimalPlaces);

        //        if(DBNull.Value!=row["Agreement Total"])
        //            txtAgrmTotal.Text = ((decimal)row["Agreement Total"]).ToString(DecimalPlaces);
        //        else
        //            txtAgrmTotal.Text = (0).ToString(DecimalPlaces);
				
        //        if(DBNull.Value!=row["Final Instalment"])
        //            txtAgrmTotal.Text = ((decimal)row["Agreement Total"]).ToString(DecimalPlaces);
        //        else
        //            txtAgrmTotal.Text = (0).ToString(DecimalPlaces);
        //    }
        //}

		private void LoadAccountDetails(DataTable dt)
		{
			foreach(DataRow row in dt.Rows)
			{
				txtCAccounts.Text = ((int)row[CN.CurrentAccounts]).ToString();
				txtSAccounts.Text = ((int)row[CN.SettledAccounts]).ToString();
			}
		}

	}
}
