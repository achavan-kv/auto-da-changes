using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Data;
using STL.Common.Constants.ColumnNames;
using STL.Common.Static;
using System.Xml;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.CustomerTypes;

namespace STL.PL
{

	/// <summary>
	/// Lists transactions for a specified acccount number. Individual
	/// transactions can be slected and transferred to another account
	/// number.
	/// </summary>
	public class TransferTransaction : CommonForm
	{
		//private Crownwood.Magic.Menus.MenuControl menuMain;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private System.Windows.Forms.GroupBox gbTransfer;
		private System.Windows.Forms.GroupBox gbTransactions;
		private System.Windows.Forms.Button btnTransfer;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Button btnExit;
		private STL.PL.AccountTextBox txtFromAccountNo;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton rbFromAccount;
		private System.Windows.Forms.RadioButton rbFromSundry;
		private System.Windows.Forms.GroupBox gbFrom;
		private System.Windows.Forms.GroupBox gbTo;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton rbToSundry;
		private STL.PL.AccountTextBox txtToAccountNo;
		private System.Windows.Forms.RadioButton rbToAccount;
		private System.Windows.Forms.RadioButton rbToJournal;
		private System.Windows.Forms.TextBox txtDate;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtTransRefNo;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ErrorProvider errors;
		private System.Windows.Forms.DataGrid dgTransactions;
		private new string Error = "";
		private string FromAccountNo = "000000000000";
		private string ToAccountNo = "000000000000";
		private System.Windows.Forms.TextBox txtTransType;
		private System.Windows.Forms.TextBox txtValue;
		private string SundryAccountNo = "";
		private string SundryAccountName = "";
		private DataSet SundryTransactions = null;
		private System.Windows.Forms.ComboBox drpReason;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Button btnPrevious;
		private System.Windows.Forms.Button btnNext;
		private System.Windows.Forms.ToolTip toolTip;
		private System.ComponentModel.IContainer components;
		private DateRangeCollection DateRanges;
		private int Pages = 0;
		private decimal _availableTransfer = 0;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtValueAvailable;
		private Crownwood.Magic.Menus.MenuCommand menuHelp;
		private Crownwood.Magic.Menus.MenuCommand menuLaunchHelp;
        private CheckBox chxRowLimit;
        private bool isOverage;                                                     //IP - 15/02/12 - #8819 - CR1234
        private bool isShortage;                                                    //IP - 15/02/12 - #8819 - CR1234
        private decimal toAcctBal;                                                  //IP - 15/02/12 - #8819 - CR1234
		public DateTime JournalDate;
		public decimal JournalValue
		{
			set
			{
				txtValueAvailable.Text = value.ToString(DecimalPlaces);
				txtValue.Text = value.ToString(DecimalPlaces);
				txtValue.ReadOnly = true;
			}
		}	
		
		public TransferTransaction(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
		}

		public TransferTransaction(Form root, Form parent)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile, menuHelp});
			FormRoot = root;
			FormParent = parent;
			LoadStaticData();
			toolTip.SetToolTip(btnNext, GetResource("TT_NEXT"));
			toolTip.SetToolTip(btnPrevious, GetResource("TT_PREVIOUS"));
			DateRanges = new DateRangeCollection();			
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
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.menuHelp = new Crownwood.Magic.Menus.MenuCommand();
            this.menuLaunchHelp = new Crownwood.Magic.Menus.MenuCommand();
            this.gbTransactions = new System.Windows.Forms.GroupBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.dgTransactions = new System.Windows.Forms.DataGrid();
            this.gbTransfer = new System.Windows.Forms.GroupBox();
            this.gbTo = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.rbToSundry = new System.Windows.Forms.RadioButton();
            this.rbToAccount = new System.Windows.Forms.RadioButton();
            this.rbToJournal = new System.Windows.Forms.RadioButton();
            this.gbFrom = new System.Windows.Forms.GroupBox();
            this.chxRowLimit = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtValueAvailable = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.drpReason = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTransType = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTransRefNo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDate = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rbFromSundry = new System.Windows.Forms.RadioButton();
            this.rbFromAccount = new System.Windows.Forms.RadioButton();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnTransfer = new System.Windows.Forms.Button();
            this.errors = new System.Windows.Forms.ErrorProvider(this.components);
            this.txtToAccountNo = new STL.PL.AccountTextBox();
            this.txtFromAccountNo = new STL.PL.AccountTextBox();
            this.gbTransactions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgTransactions)).BeginInit();
            this.gbTransfer.SuspendLayout();
            this.gbTo.SuspendLayout();
            this.gbFrom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errors)).BeginInit();
            this.SuspendLayout();
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuExit});
            this.menuFile.Text = "&File";
            this.menuFile.Click += new System.EventHandler(this.menuFile_Click);
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.Description = "MenuItem";
            this.menuHelp.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuLaunchHelp});
            this.menuHelp.Text = "&Help";
            // 
            // menuLaunchHelp
            // 
            this.menuLaunchHelp.Description = "MenuItem";
            this.menuLaunchHelp.Text = "&About This Screen";
            this.menuLaunchHelp.Click += new System.EventHandler(this.menuLaunchHelp_Click);
            // 
            // gbTransactions
            // 
            this.gbTransactions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTransactions.BackColor = System.Drawing.SystemColors.Control;
            this.gbTransactions.Controls.Add(this.btnNext);
            this.gbTransactions.Controls.Add(this.btnPrevious);
            this.gbTransactions.Controls.Add(this.dgTransactions);
            this.gbTransactions.Location = new System.Drawing.Point(8, 224);
            this.gbTransactions.Name = "gbTransactions";
            this.gbTransactions.Size = new System.Drawing.Size(776, 248);
            this.gbTransactions.TabIndex = 0;
            this.gbTransactions.TabStop = false;
            this.gbTransactions.Text = "Transactions";
            // 
            // btnNext
            // 
            this.btnNext.Enabled = false;
            this.btnNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNext.Location = new System.Drawing.Point(716, 19);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(26, 18);
            this.btnNext.TabIndex = 2;
            this.btnNext.Text = ">>";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Enabled = false;
            this.btnPrevious.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrevious.Location = new System.Drawing.Point(684, 19);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(26, 18);
            this.btnPrevious.TabIndex = 1;
            this.btnPrevious.Text = "<<";
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // dgTransactions
            // 
            this.dgTransactions.AllowSorting = false;
            this.dgTransactions.DataMember = "";
            this.dgTransactions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgTransactions.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgTransactions.Location = new System.Drawing.Point(3, 16);
            this.dgTransactions.Name = "dgTransactions";
            this.dgTransactions.ReadOnly = true;
            this.dgTransactions.Size = new System.Drawing.Size(770, 229);
            this.dgTransactions.TabIndex = 0;
            this.dgTransactions.TabStop = false;
            this.dgTransactions.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgTransactions_MouseUp);
            // 
            // gbTransfer
            // 
            this.gbTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTransfer.BackColor = System.Drawing.SystemColors.Control;
            this.gbTransfer.Controls.Add(this.gbTo);
            this.gbTransfer.Controls.Add(this.gbFrom);
            this.gbTransfer.Controls.Add(this.btnExit);
            this.gbTransfer.Controls.Add(this.btnClear);
            this.gbTransfer.Controls.Add(this.btnTransfer);
            this.gbTransfer.Location = new System.Drawing.Point(8, 0);
            this.gbTransfer.Name = "gbTransfer";
            this.gbTransfer.Size = new System.Drawing.Size(776, 224);
            this.gbTransfer.TabIndex = 0;
            this.gbTransfer.TabStop = false;
            this.gbTransfer.Text = "Transfer Options";
            // 
            // gbTo
            // 
            this.gbTo.Controls.Add(this.label9);
            this.gbTo.Controls.Add(this.txtName);
            this.gbTo.Controls.Add(this.label2);
            this.gbTo.Controls.Add(this.rbToSundry);
            this.gbTo.Controls.Add(this.txtToAccountNo);
            this.gbTo.Controls.Add(this.rbToAccount);
            this.gbTo.Controls.Add(this.rbToJournal);
            this.gbTo.Location = new System.Drawing.Point(400, 24);
            this.gbTo.Name = "gbTo";
            this.gbTo.Size = new System.Drawing.Size(280, 192);
            this.gbTo.TabIndex = 1;
            this.gbTo.TabStop = false;
            this.gbTo.Text = "To";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(40, 104);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 16);
            this.label9.TabIndex = 0;
            this.label9.Text = "Name";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(40, 120);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(224, 20);
            this.txtName.TabIndex = 10;
            this.txtName.TabStop = false;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(40, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Account No:";
            // 
            // rbToSundry
            // 
            this.rbToSundry.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbToSundry.Location = new System.Drawing.Point(144, 40);
            this.rbToSundry.Name = "rbToSundry";
            this.rbToSundry.Size = new System.Drawing.Size(120, 24);
            this.rbToSundry.TabIndex = 6;
            this.rbToSundry.Text = "  Sundry Account";
            // 
            // rbToAccount
            // 
            this.rbToAccount.Checked = true;
            this.rbToAccount.Location = new System.Drawing.Point(16, 40);
            this.rbToAccount.Name = "rbToAccount";
            this.rbToAccount.Size = new System.Drawing.Size(16, 16);
            this.rbToAccount.TabIndex = 5;
            this.rbToAccount.TabStop = true;
            this.rbToAccount.CheckedChanged += new System.EventHandler(this.rbToAccount_CheckedChanged);
            // 
            // rbToJournal
            // 
            this.rbToJournal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbToJournal.Location = new System.Drawing.Point(144, 40);
            this.rbToJournal.Name = "rbToJournal";
            this.rbToJournal.Size = new System.Drawing.Size(120, 24);
            this.rbToJournal.TabIndex = 7;
            this.rbToJournal.Text = "  Journal Account";
            this.rbToJournal.Visible = false;
            // 
            // gbFrom
            // 
            this.gbFrom.Controls.Add(this.chxRowLimit);
            this.gbFrom.Controls.Add(this.label7);
            this.gbFrom.Controls.Add(this.txtValueAvailable);
            this.gbFrom.Controls.Add(this.label8);
            this.gbFrom.Controls.Add(this.drpReason);
            this.gbFrom.Controls.Add(this.label5);
            this.gbFrom.Controls.Add(this.txtTransType);
            this.gbFrom.Controls.Add(this.label6);
            this.gbFrom.Controls.Add(this.txtValue);
            this.gbFrom.Controls.Add(this.label4);
            this.gbFrom.Controls.Add(this.txtTransRefNo);
            this.gbFrom.Controls.Add(this.label3);
            this.gbFrom.Controls.Add(this.txtDate);
            this.gbFrom.Controls.Add(this.label1);
            this.gbFrom.Controls.Add(this.rbFromSundry);
            this.gbFrom.Controls.Add(this.txtFromAccountNo);
            this.gbFrom.Controls.Add(this.rbFromAccount);
            this.gbFrom.Location = new System.Drawing.Point(16, 24);
            this.gbFrom.Name = "gbFrom";
            this.gbFrom.Size = new System.Drawing.Size(376, 192);
            this.gbFrom.TabIndex = 0;
            this.gbFrom.TabStop = false;
            this.gbFrom.Text = "From";
            // 
            // chxRowLimit
            // 
            this.chxRowLimit.Checked = true;
            this.chxRowLimit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chxRowLimit.Location = new System.Drawing.Point(303, 40);
            this.chxRowLimit.Name = "chxRowLimit";
            this.chxRowLimit.Size = new System.Drawing.Size(67, 16);
            this.chxRowLimit.TabIndex = 79;
            this.chxRowLimit.Text = "Top 250";
            this.chxRowLimit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(208, 104);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(144, 16);
            this.label7.TabIndex = 0;
            this.label7.Text = "Value Available for Transfer";
            // 
            // txtValueAvailable
            // 
            this.txtValueAvailable.Location = new System.Drawing.Point(208, 120);
            this.txtValueAvailable.Name = "txtValueAvailable";
            this.txtValueAvailable.ReadOnly = true;
            this.txtValueAvailable.Size = new System.Drawing.Size(144, 20);
            this.txtValueAvailable.TabIndex = 17;
            this.txtValueAvailable.TabStop = false;
            this.txtValueAvailable.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(40, 144);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(96, 16);
            this.label8.TabIndex = 0;
            this.label8.Text = "Transfer Reason";
            // 
            // drpReason
            // 
            this.drpReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpReason.Location = new System.Drawing.Point(40, 160);
            this.drpReason.Name = "drpReason";
            this.drpReason.Size = new System.Drawing.Size(152, 21);
            this.drpReason.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(40, 104);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 16);
            this.label5.TabIndex = 14;
            this.label5.Text = "Type";
            // 
            // txtTransType
            // 
            this.txtTransType.Location = new System.Drawing.Point(40, 120);
            this.txtTransType.Name = "txtTransType";
            this.txtTransType.ReadOnly = true;
            this.txtTransType.Size = new System.Drawing.Size(100, 20);
            this.txtTransType.TabIndex = 13;
            this.txtTransType.TabStop = false;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(208, 144);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 16);
            this.label6.TabIndex = 0;
            this.label6.Text = "Value";
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(208, 160);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(144, 20);
            this.txtValue.TabIndex = 30;
            this.txtValue.Leave += new System.EventHandler(this.txtValue_Leave);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(40, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Trans Ref No";
            // 
            // txtTransRefNo
            // 
            this.txtTransRefNo.Location = new System.Drawing.Point(40, 80);
            this.txtTransRefNo.Name = "txtTransRefNo";
            this.txtTransRefNo.ReadOnly = true;
            this.txtTransRefNo.Size = new System.Drawing.Size(100, 20);
            this.txtTransRefNo.TabIndex = 9;
            this.txtTransRefNo.TabStop = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(208, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "Date";
            // 
            // txtDate
            // 
            this.txtDate.Location = new System.Drawing.Point(208, 80);
            this.txtDate.Name = "txtDate";
            this.txtDate.ReadOnly = true;
            this.txtDate.Size = new System.Drawing.Size(144, 20);
            this.txtDate.TabIndex = 7;
            this.txtDate.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(40, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Account No:";
            // 
            // rbFromSundry
            // 
            this.rbFromSundry.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbFromSundry.Location = new System.Drawing.Point(144, 40);
            this.rbFromSundry.Name = "rbFromSundry";
            this.rbFromSundry.Size = new System.Drawing.Size(120, 16);
            this.rbFromSundry.TabIndex = 6;
            this.rbFromSundry.Text = "  Sundry Account";
            // 
            // rbFromAccount
            // 
            this.rbFromAccount.Checked = true;
            this.rbFromAccount.Location = new System.Drawing.Point(16, 40);
            this.rbFromAccount.Name = "rbFromAccount";
            this.rbFromAccount.Size = new System.Drawing.Size(16, 16);
            this.rbFromAccount.TabIndex = 5;
            this.rbFromAccount.TabStop = true;
            this.rbFromAccount.CheckedChanged += new System.EventHandler(this.rbFromAccount_CheckedChanged);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(704, 96);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(56, 23);
            this.btnExit.TabIndex = 70;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(704, 64);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(56, 23);
            this.btnClear.TabIndex = 60;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnTransfer
            // 
            this.btnTransfer.Location = new System.Drawing.Point(704, 32);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(56, 23);
            this.btnTransfer.TabIndex = 50;
            this.btnTransfer.Text = "Transfer";
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // errors
            // 
            this.errors.ContainerControl = this;
            this.errors.DataMember = "";
            // 
            // txtToAccountNo
            // 
            this.txtToAccountNo.Location = new System.Drawing.Point(40, 40);
            this.txtToAccountNo.Name = "txtToAccountNo";
            this.txtToAccountNo.PreventPaste = false;
            this.txtToAccountNo.Size = new System.Drawing.Size(88, 20);
            this.txtToAccountNo.TabIndex = 40;
            this.txtToAccountNo.Tag = "ACCNO";
            this.txtToAccountNo.Text = "000-0000-0000-0";
            this.txtToAccountNo.Leave += new System.EventHandler(this.txtToAccountNo_Leave);
            // 
            // txtFromAccountNo
            // 
            this.txtFromAccountNo.Location = new System.Drawing.Point(40, 40);
            this.txtFromAccountNo.Name = "txtFromAccountNo";
            this.txtFromAccountNo.PreventPaste = false;
            this.txtFromAccountNo.Size = new System.Drawing.Size(88, 20);
            this.txtFromAccountNo.TabIndex = 10;
            this.txtFromAccountNo.Tag = "ACCNO";
            this.txtFromAccountNo.Text = "000-0000-0000-0";
            this.txtFromAccountNo.Leave += new System.EventHandler(this.txtFromAccountNo_Leave);
            // 
            // TransferTransaction
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.gbTransactions);
            this.Controls.Add(this.gbTransfer);
            this.Name = "TransferTransaction";
            this.Text = "Transfer Transaction";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.TransferTransaction_HelpRequested);
            this.gbTransactions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgTransactions)).EndInit();
            this.gbTransfer.ResumeLayout(false);
            this.gbTo.ResumeLayout(false);
            this.gbTo.PerformLayout();
            this.gbFrom.ResumeLayout(false);
            this.gbFrom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errors)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void LoadStaticData()
		{
			try
			{
				Wait();
		
				XmlUtilities xml = new XmlUtilities();
				XmlDocument dropDowns = new XmlDocument();
				dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

				if(StaticData.Tables[TN.TransferReasons]==null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.TransferReasons, new string[]{"XRS", "L"}));
				
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
			
				drpReason.DataSource = (DataTable)StaticData.Tables[TN.TransferReasons];
				drpReason.DisplayMember = CN.CodeDescription;
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

		private void LoadSundryData()
		{
			Wait();
			/* if sundry account is checked we need to load the right 
			* transactions if they're not already loaded */
			if(rbFromSundry.Checked)
			{
                chxRowLimit.Enabled = false;    // 68674 jec 28/11/06
				if(SundryAccountNo.Length==0)
				{
					SundryAccountNo = AccountManager.GetSundryCreditAccount(Convert.ToInt16(Config.BranchCode),
						out Error);
					if(Error.Length>0)
						ShowError(Error);
						
					if(SundryAccountNo.Length==0)
					{
						errors.SetError(rbFromSundry, GetResource("M_NOSUNDRYACCOUNT"));
						rbFromAccount.Checked = true;
					}
					else
						errors.SetError(rbFromSundry, "");
				}

				if(SundryAccountNo.Length > 0)
				{
					if(rbToAccount.Checked)
					{
						DateRanges.Clear();
						Pages = 0;
                        DateRanges.Add(new DateRange((DateTime)StaticDataManager.GetServerDateTime(),
                                    StaticDataManager.GetServerDateTime()));     //68674
                        Pages++;    //68674

                        SundryTransactions = AccountManager.GetTransactionsForTransfer(SundryAccountNo, StaticDataManager.GetServerDateTime(), chxRowLimit.Checked, out this._availableTransfer, out Error);
						if(Error.Length>0)
							ShowError(Error);
						
						if(SundryTransactions.Tables[0].Rows.Count > 0)
						{
							int index = SundryTransactions.Tables[0].Rows.Count-1;
							DateRanges.Add(	new DateRange(	(DateTime)SundryTransactions.Tables[0].Rows[0][CN.DateTrans],
								(DateTime)SundryTransactions.Tables[0].Rows[index][CN.DateTrans] ) );
						}
					}
				}
					
				FormatDataGrid(SundryTransactions);
                //IP - 01/10/2007
                //If the amount available for transfer from Sundry Credit = 0, then display a warning on the status toolbar.

                if (this._availableTransfer == 0)
                {
                    ((MainForm)FormRoot).statusBar1.Text = GetResource("M_SUNDRYNOVALUE");   
                }
			}
		}

		private void FormatDataGrid(DataSet transactions)
		{
			if(transactions != null &&
				transactions.Tables.Count>0)
			{
				dgTransactions.DataSource = transactions.Tables[0].DefaultView;
				dgTransactions.TableStyles.Clear();

				DataGridTableStyle tabStyle = new DataGridTableStyle();
				tabStyle.MappingName = transactions.Tables[0].TableName;				

				AddColumnStyle(CN.TransRefNo, tabStyle, 100, true, GetResource("T_TRANSREFNO"), "", HorizontalAlignment.Left);
				AddColumnStyle(CN.DateTrans, tabStyle, 120, true, GetResource("T_DATE"), "", HorizontalAlignment.Left);
				AddColumnStyle(CN.TransTypeCode, tabStyle, 70, true, GetResource("T_TYPE"), "", HorizontalAlignment.Left);
				AddColumnStyle(CN.TransValue, tabStyle, 80, true, GetResource("T_VALUE"), DecimalPlaces, HorizontalAlignment.Right);
                AddColumnStyle(CN.ChequeNo, tabStyle, 150, true, GetResource("T_FROMTOACCTNO"), "", HorizontalAlignment.Left);
				AddColumnStyle(CN.Reference, tabStyle, 200, true, GetResource("T_REFERENCE"), "", HorizontalAlignment.Left);
				dgTransactions.TableStyles.Add(tabStyle);

				((MainForm)FormRoot).statusBar1.Text = transactions.Tables[0].DefaultView.Count.ToString() + " Rows returned";

				this.txtValueAvailable.Text = this._availableTransfer.ToString(DecimalPlaces);

				btnNext.Enabled = transactions.Tables[0].DefaultView.Count!=250 ? false:true;   // <250 68674 jec 28/11/04 (button was enabled when all txns returned)
			}			
		}

		private bool ValidValue(bool canBeBlank)
		{
			bool status = true;
			errors.SetError(txtValue, "");
			if(IsStrictMoney(txtValue.Text) && txtValue.Text.Trim() != "") 
			{
				if(Convert.ToDecimal(StripCurrency(txtValue.Text)) > 0)
				{
					//Value is valid
					txtValue.Text = Convert.ToDecimal(StripCurrency(txtValue.Text)).ToString(DecimalPlaces);
				}
				else
				{
					errors.SetError(txtValue, GetResource("M_POSITIVENUM"));
					txtValue.Select(0, txtValue.Text.Length);
					txtValue.Focus();
					status = false;
				}
			}
			else if(txtValue.Text.Trim() != "")
			{
				errors.SetError(txtValue, GetResource("M_NONNUMERIC"));
				txtValue.Select(0, txtValue.Text.Length);
				txtValue.Focus();
				status = false;
			}
			else if(!canBeBlank)
			{		
				//Field is blank
				status = false;
				errors.SetError(txtValue, GetResource("M_POSITIVENUM"));
			}
			return status;
		}
		private bool ValidateTransfer()
		{
			bool valid = true;

            //IP - 21/02/12 - #9633 - CR1234
            if (dgTransactions.CurrentRowIndex >= 0)
            {
                if (Convert.ToBoolean(((DataView)dgTransactions.DataSource)[dgTransactions.CurrentRowIndex][CN.AllowTransfer]) == false)
                {
                    valid = false;
                    errors.SetError(txtTransType, GetResource("M_ALREADYTRANSFERRED"));
                }
                else
                {
                    errors.SetError(txtTransType, "");
                }
            }

			if( rbFromAccount.Checked )
			{
				if( txtFromAccountNo.UnformattedText == AccountTextBox.UnSet || txtFromAccountNo.UnformattedText == "")
				{
					errors.SetError(txtFromAccountNo, GetResource("M_REQUIREACCOUNTNO"));
					valid = false;
				}
				else
					errors.SetError(txtFromAccountNo, "");
			}

			if( rbToAccount.Checked )
			{
				if( txtToAccountNo.UnformattedText == AccountTextBox.UnSet || txtToAccountNo.UnformattedText == "")
				{
					errors.SetError(txtToAccountNo, GetResource("M_REQUIREACCOUNTNO"));
					valid = false;
				}
				else
					errors.SetError(txtToAccountNo, "");
			}

			if( rbFromAccount.Checked && rbToAccount.Checked )
			{
				if(valid)
				{
					if(txtFromAccountNo.UnformattedText == txtToAccountNo.UnformattedText)
					{
						errors.SetError(txtToAccountNo, GetResource("M_CANTTRANSFERTOSAMEACCT"));
						valid = false;
					}
					else
						errors.SetError(txtToAccountNo, "");
				}
			}
			if(valid)
				valid = ValidValue(false); //Check valid transfer value entered

			if(valid)
			{
                //IP - 15/02/12 - #8819 - CR1234 - I had to set the shortage value to a postive in txtValue therefore changed validation below to cater for this.
                var shortageSelected = false;
                var casSelected = false;                        //IP - 21/02/12 - #9683

                if(dgTransactions.CurrentRowIndex >=0)
                {
                    if ((string)((DataView)dgTransactions.DataSource)[dgTransactions.CurrentRowIndex][CN.TransTypeCode] == "SHO")
                    {
                        shortageSelected = true;
                    }
                    else if ((string)((DataView)dgTransactions.DataSource)[dgTransactions.CurrentRowIndex][CN.TransTypeCode] == "CAS")               //IP - 21/02/12 - #9683
                    {
                        casSelected = true;
                    }
                }

                if (shortageSelected || casSelected)        //IP - 21/02/12 - #9683
                {
                   
                    if (Math.Abs(Convert.ToDecimal(StripCurrency(txtValue.Text))) >
                        Math.Abs(Convert.ToDecimal(StripCurrency(txtValueAvailable.Text))))
                    {
                        ShowInfo("M_TRANSFERTOOHIGH", new object[] { txtValue.Text, txtValueAvailable.Text, txtValue.Text }, MessageBoxButtons.OK);
                        valid = false;
                    }
                }
                else
                {
                    if (Convert.ToDecimal(StripCurrency(txtValue.Text)) >
                        Convert.ToDecimal(StripCurrency(txtValueAvailable.Text)))
                    {
                        ShowInfo("M_TRANSFERTOOHIGH", new object[] { txtValue.Text, txtValueAvailable.Text, txtValue.Text }, MessageBoxButtons.OK);
                        valid = false;
                    }
                }

			}

            //IP - 15/02/12 - #8819 - CR1234
            if (valid)
            {
                if (rbToAccount.Checked && isOverage)
                {

                    if ((toAcctBal + Convert.ToDecimal(StripCurrency(txtValue.Text))) > 0)
                    {
                        errors.SetError(txtToAccountNo, GetResource("M_CANTTRANSFERTOOVERAGE"));
                        valid = false;
                    }
                    else
                    {
                        errors.SetError(txtToAccountNo, "");
                    }
                }
                else if (rbToAccount.Checked && isShortage)
                {
                    if ((toAcctBal + (Convert.ToDecimal(StripCurrency(txtValue.Text))*-1)) < 0)
                    {
                        errors.SetError(txtToAccountNo, GetResource("M_CANTTRANSFERTOSHORTAGE"));
                        valid = false;
                    }
                    else
                    {
                        errors.SetError(txtToAccountNo, "");
                    }
                }
            }

			if(drpReason.Text.Length == 0)
			{
				errors.SetError(drpReason, GetResource("M_ENTERTRANSFERREASON"));
				valid = false;
			}
			else
				errors.SetError(drpReason, "");

            //IP - 14/02/12 - #8819 - CR1234 - check if the selected Trans Type Code is a Cashier Shortage Adjustment
            var CashierShortAdj = dgTransactions.DataSource!= null && ((string)((DataView)dgTransactions.DataSource)[dgTransactions.CurrentRowIndex][CN.TransTypeCode] == "OVE"
                || (string)((DataView)dgTransactions.DataSource)[dgTransactions.CurrentRowIndex][CN.TransTypeCode] == "SHO"
                || (string)((DataView)dgTransactions.DataSource)[dgTransactions.CurrentRowIndex][CN.TransTypeCode] == "CAS") ? 1 : 0;                //IP - 21/02/12 - #9683

            if (valid && !rbToSundry.Checked && CashierShortAdj == 0)       //IP - 14/02/12 - #8819 - CR1234 - and not a CAS transaction type
			{
				// Check whether the transfer is more than the settlement
				decimal transferValue = Convert.ToDecimal(StripCurrency(txtValue.Text));
				decimal settlement = 0;
				decimal rebate = 0;
				decimal collectionFee = 0;

                if (!rbToJournal.Checked)   // 68695 jec
				    PaymentManager.GetAccountSettlement(Config.CountryCode, txtToAccountNo.UnformattedText, out settlement, out rebate, out collectionFee, out Error);
                else
                    // call method with Sundry Credit a/c ro resolve error caused when 0 a/c 
                    PaymentManager.GetAccountSettlement(Config.CountryCode, SundryAccountNo, out settlement, out rebate, out collectionFee, out Error);     // 68695 jec 18/12/06

				if (Error.Length > 0)
					ShowError(Error);
                //else if (transferValue > settlement - collectionFee)
				//else if (!rbToJournal.Checked && (transferValue > settlement - collectionFee))  // 68695 jec 15/01/07 do not display settle messages when transferring to journal
                else if (!rbToJournal.Checked && (transferValue > settlement - collectionFee) && txtToAccountNo.UnformattedText.Substring(3,1) != "9") //IP - 13/04/12 - #9920 - UAT150 // 68695 jec 15/01/07 do not display settle messages when transferring to journal
				{
					// Transfer Transaction will ignore any fee due
					settlement = settlement - collectionFee;
					decimal creditAmount = settlement - transferValue;

					switch (ShowInfo("M_SETTLETRANSFER", new Object[]{settlement.ToString(DecimalPlaces), rebate.ToString(DecimalPlaces), transferValue.ToString(DecimalPlaces), creditAmount.ToString(DecimalPlaces), txtToAccountNo.Text}, MessageBoxButtons.YesNoCancel))
					{
						case DialogResult.Cancel:
							// Mark the transfer value with a tooltip giving the settlement amount
							errors.SetError(txtValue, GetResource("M_SETTLEVALUE", new Object[]{settlement.ToString(DecimalPlaces), txtToAccountNo.Text}));
							valid = false;
							break;
						case DialogResult.Yes:
							// Continue but only transfer the Settlement amount
							txtValue.Text = settlement.ToString(DecimalPlaces);
							errors.SetError(txtValue, "");
							break;
						default:
							// Continue with the original transfer value and place the account in credit
							errors.SetError(txtValue, "");
							break;
					}
				}
			}

			return valid;
		}


		private void ProcessTransfer()
		{
			string transType = "";
			string from = "";
			string to = "";
			int oldRefNo = 0;
            var cashierTotID = string.Empty;

			DateTime dateTrans = StaticDataManager.GetServerDateTime();

			if(rbFromAccount.Checked)
				from = from = txtFromAccountNo.UnformattedText;

			if(rbToAccount.Checked)
				to = txtToAccountNo.UnformattedText;

            if (rbToSundry.Checked)
            {
                //Code added to get sundry credit account number   jec 13/11/07  uat 121
                SundryAccountNo = AccountManager.GetSundryCreditAccount(Convert.ToInt16(Config.BranchCode),
                        out Error);
                if (Error.Length > 0)
                    ShowError(Error);

                if (SundryAccountNo.Length == 0)
                {
                    errors.SetError(rbToSundry, GetResource("M_NOSUNDRYACCOUNT"));
                    //rbFromAccount.Checked = true;
                }
                else
                    errors.SetError(rbToSundry, "");

                to = SundryAccountNo;
            }

			if(rbFromSundry.Checked)
				from = SundryAccountNo;

			if( rbFromAccount.Checked && rbToAccount.Checked )
				transType = TransType.Transfer;

			if( rbFromSundry.Checked )
			{
				if( rbToJournal.Checked )
				{
					transType = TransType.TakeonTransfer;
					//dateTrans = JournalDate;
				}
				else
					transType = TransType.SundryCreditTransfer;
				SundryTransactions = null;
			}

			if( rbToSundry.Checked )
			{
				transType = TransType.SundryCreditTransfer;
				SundryTransactions = null;
			}

			int index = dgTransactions.CurrentRowIndex;

            if (index >= 0)
            {
                oldRefNo = (int)((DataView)dgTransactions.DataSource)[index][CN.TransRefNo];

                //IP - 14/02/12 - #8819 - CR1234
                if ((string)((DataView)dgTransactions.DataSource)[index][CN.TransTypeCode] == "OVE"
                    || (string)((DataView)dgTransactions.DataSource)[index][CN.TransTypeCode] == "SHO")
                {
                    transType = TransType.CashiersShortageAdjustment;
                    cashierTotID = (string)((DataView)dgTransactions.DataSource)[index][CN.Reference];  //Reference will hold the CashierTotalID for an Overage/Shortage
                }
            }
	
            
			if(Math.Abs(MoneyStrToDecimal(txtValue.Text)) > 0)
			{
                var transferVal = 0m;                       //IP - 15/02/12 - #8819 - CR1234

                //IP - 15/02/12 - #8819 - CR1234 - if this is a shortage we need to pass the value through as negative to credit the shortage account
                if (dgTransactions.DataSource != null && (string)((DataView)dgTransactions.DataSource)[index][CN.TransTypeCode] == "SHO")    //#18333 - check datasource not null
                {
                    transferVal = MoneyStrToDecimal(txtValue.Text) * -1;
                }
                else
                {
                    transferVal = MoneyStrToDecimal(txtValue.Text);
                }

				AccountManager.TransferTransaction( from,
					to,
					transType,
					//MoneyStrToDecimal(txtValue.Text), 
                    transferVal,                            //IP - 15/02/12 - #8819 - CR1234
					Convert.ToInt16(Config.BranchCode),
					Config.CountryCode,
					dateTrans,
					(string)((DataRowView)drpReason.SelectedItem)[CN.Code],
					oldRefNo,
                    1,              //IP - 29/11/10 - Store Card - Added agrmtno
                    null,              //IP - 30/11/10 - Store Card - Added StoreCardNo
                    cashierTotID,      //IP - 14/02/12 - #8819 - CR1234
					out Error);
				if( Error.Length > 0 )
					ShowError(Error);
				else
				{
					btnClear_Click(null, null);
					((MainForm)FormRoot).statusBar1.Text = GetResource("M_TRANSFERSUCCESSFUL");
				}
			}
		}

		private void rbFromAccount_CheckedChanged(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				btnClear_Click(null, null);

                btnNext.Visible = btnPrevious.Visible = (rbFromSundry.Checked && rbToAccount.Checked) || rbFromAccount.Checked; //68674 jec 24/11/06

				rbToSundry.Visible = rbFromAccount.Checked;
				rbToJournal.Visible = !rbFromAccount.Checked;
				rbToAccount.Checked = true;

				txtFromAccountNo.Enabled = rbFromAccount.Checked;
				txtFromAccountNo.ResetText();
                btnNext.Enabled = false;    //68674 jec 24/11/06
                chxRowLimit.Enabled = true;    // 68674 jec 24/11/06

				LoadSundryData();				
			}
			catch(Exception ex)
			{
				Catch(ex, "rbFromAccount_CheckedChanged");
			}
			finally
			{
				StopWait();
			}
		}

		private void rbToAccount_CheckedChanged(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				txtToAccountNo.Enabled = rbToAccount.Checked;
				txtToAccountNo.ResetText();
                errors.SetError(txtValue, "");      // 68695 jec 15/01/07

                btnNext.Visible = btnPrevious.Visible = (rbFromSundry.Checked && rbToAccount.Checked) || rbFromAccount.Checked; //68674 jec 24/11/06

				if(rbToSundry.Checked)
				{
					if(SundryAccountNo.Length==0)
					{
						SundryAccountNo = AccountManager.GetSundryCreditAccount(Convert.ToInt16(Config.BranchCode),
							out Error);
						if(Error.Length>0)
							ShowError(Error);
						
						if(SundryAccountNo.Length==0)
						{
							errors.SetError(rbToSundry, GetResource("M_NOSUNDRYACCOUNT"));
							rbToAccount.Checked = true;
						}
						else
							errors.SetError(rbToSundry, "");
					}

					if(SundryAccountName.Length == 0)
					{
						DataSet ds = AccountManager.GetAccountName(SundryAccountNo, "", out Error);
						if(Error.Length > 0)
							ShowError(Error);
						else
						{
							foreach (DataTable dt in ds.Tables)
								if(dt.Rows.Count > 0)
								{
									errors.SetError(rbToSundry, "");
									foreach (DataRow dr in dt.Rows)
										SundryAccountName = txtName.Text = (string)dr[CN.name];
								}
								else
								{	
									errors.SetError(rbToSundry, GetResource("M_NOSUCHACCOUNT"));
								}
						}
					}
					else
						txtName.Text = SundryAccountName;
				}
				
				/* if to journal is cheked then from sundry must be checked */
				if(rbToJournal.Checked)
				{
					// 68695 jec 02/01/07 btnClear_Click(null, null);
                    btnClear_Click(null, null);     // reinstated jec 15/01/07
					/* ask for a date to journal up to */
					JournalDate jd = new JournalDate(this);
					jd.ShowDialog();
				}
				else
				{
					txtValue.Text = "";
					txtValue.ReadOnly = false;
				}

				if(rbToAccount.Checked)
					LoadSundryData();
			}
			catch(Exception ex)
			{
				Catch(ex, "rbToAccount_CheckedChanged");
			}
			finally
			{
				StopWait();
			}
		}				

		private void txtFromAccountNo_Leave(object sender, System.EventArgs e)
		{
            try
            {
                Wait();
                SundryAccountNo = "";   // 68674 jec 24/11/06
                chxRowLimit.Enabled = false;    // 68674 jec 24/11/06
                    if (txtFromAccountNo.UnformattedText != FromAccountNo)
                {
                    DateRanges.Clear();     //68674
                    Pages = 0;
                    DateRanges.Add(new DateRange((DateTime)StaticDataManager.GetServerDateTime(),
                                StaticDataManager.GetServerDateTime()));     //68674
                    Pages++;    //68674

                    FromAccountNo = txtFromAccountNo.UnformattedText;

                    // lw69285 rdb 25/9/07 get customer and check if bdw
                    if (CheckCustomerValid(FromAccountNo))
                    {
                        DataSet ds = AccountManager.GetTransactionsForTransfer(txtFromAccountNo.UnformattedText,
                                            StaticDataManager.GetServerDateTime(), chxRowLimit.Checked, out this._availableTransfer, out Error);
                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            // set up date range for btn_next   68674 jec 24/11/06
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                int index = ds.Tables[0].Rows.Count - 1;
                                DateRanges.Add(new DateRange((DateTime)ds.Tables[0].Rows[0][CN.DateTrans],
                                    (DateTime)ds.Tables[0].Rows[index][CN.DateTrans]));

                            }
                            FormatDataGrid(ds);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "txtFromAccountNo_Leave");
            }
			finally
			{
				StopWait();
			}
		}

		private void txtToAccountNo_Leave(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				/* make sure that this account exists and get the customer name
				 * for the account */
				if(this.ToAccountNo != txtToAccountNo.UnformattedText)
				{

                    isOverage = false;                          //IP - 15/02/12 - #8819 - CR1234
                    isShortage = false;                         //IP - 15/02/12 - #8819 - CR1234

					ToAccountNo = txtToAccountNo.UnformattedText;
                    // lw69285 rdb 25/9/07 get customer and check if bdw
                    if (CheckCustomerValid(ToAccountNo))
                    {

                        DataSet ds = AccountManager.GetAccountName(txtToAccountNo.UnformattedText, "", out Error);
                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            foreach (DataTable dt in ds.Tables)
                                if (dt.Rows.Count > 0)
                                {
                                    errors.SetError(txtToAccountNo, "");
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        txtName.Text = (string)dr[CN.name];

                                        //IP - 15/02/12 - #8819 - CR1234 - need to determine if account is overage/shortage
                                        toAcctBal = Convert.ToDecimal(dr[CN.Balance]);

                                        if (Convert.ToString(dr[CN.CustomerID]).Contains("OVERAGE"))
                                        {
                                            isOverage = true;
                                        }
                                        else if (Convert.ToString(dr[CN.CustomerID]).Contains("SHORTAGE"))
                                        {
                                            isShortage = true;
                                        }
                                        else
                                        {
                                            isOverage = false;
                                            isShortage = false;
                                        }
                                    }
                                }
                                else
                                {
                                    errors.SetError(txtToAccountNo, GetResource("M_NOSUCHACCOUNT"));
                                }
                        }
                    }
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "txtToAccountNo_Leave");
			}
			finally
			{
				StopWait();
			}
		}

        /// <summary>
        ///  lw69285 rdb 25/9/07 check if this is a bdw account 
        /// </summary>
        /// <returns></returns>
        private bool CheckCustomerValid(string accountNo)
        {
            bool status = false;
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
                else
                    status = true;
            }
            if (status)
            {
                DataRow r = CustomerSet.Tables[TN.Customer].Rows[0];
                string CustomerID = (string)r[CN.CustomerID];
                //if(BDW.IsBDW(CustomerID))
                string error;
                if (AccountManager.GetAccountHasBDW(accountNo, out error))
                {
                    status = false;
                    ShowInfo("M_BDWINVALIDSCREEN");
                    btnClear_Click(null, null);
                }
            }
            return status;
        }

		private void dgTransactions_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				Wait();
				int index = dgTransactions.CurrentRowIndex;

				if(index >=0 )
				{
                    errors.SetError(txtTransType, "");                  //IP - 21/02/12 - CR1234
					dgTransactions.Select(index);

					DataView v = (DataView)dgTransactions.DataSource;

                        this.txtDate.Text = ((DateTime)v[index][CN.DateTrans]).ToString();
                        this.txtTransRefNo.Text = Convert.ToString(v[index][CN.TransRefNo]);
                        this.txtTransType.Text = (string)v[index][CN.TransTypeCode];

                        //this.txtValue.Text = ((decimal)v[index][CN.TransValue]).ToString(DecimalPlaces);

                        //IP - 15/02/12 - #8819 - CR1234 - if transferring a shortage then display as a positive value 
                        if (Convert.ToString(v[index][CN.TransTypeCode]) == "SHO")
                        {
                            this.txtValue.Text = (Math.Abs((decimal)v[index][CN.TransValue])).ToString(DecimalPlaces);
                        }
                        else
                        {
                            this.txtValue.Text = ((decimal)v[index][CN.TransValue]).ToString(DecimalPlaces);
                        }

                        if (txtToAccountNo.Enabled)
                        {
                            txtToAccountNo.Text = (string)v[index][CN.ChequeNo];
                            txtName.Text = (string)v[index][CN.Reference];
                        }   
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "dgTransactions_MouseUp");
			}
			finally
			{
				StopWait();
			}
		}

		private void txtValue_Leave(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				ValidValue(true); //Check valid transfer value entered
			}
			catch(Exception ex)
			{
				Catch(ex, "txtValue_Leave");
			}
			finally
			{
				StopWait();
			}
		}

		private void btnNext_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				if(SundryAccountNo.Length > 0)
				{
                    SundryTransactions = AccountManager.GetTransactionsForTransfer(SundryAccountNo, DateRanges.Item(Pages).To, chxRowLimit.Checked, out this._availableTransfer, out Error);
					if(Error.Length>0)
						ShowError(Error);
					else
					{
						Pages++;
						btnPrevious.Enabled = true;
						
						int index = SundryTransactions.Tables[0].Rows.Count-1;
						DateRanges.Add(	new DateRange(	(DateTime)SundryTransactions.Tables[0].Rows[0][CN.DateTrans],
							(DateTime)SundryTransactions.Tables[0].Rows[index][CN.DateTrans] ) );

						FormatDataGrid(SundryTransactions);
					}
				}
                // for specific account no  68674 jec 24/11/06
                if (FromAccountNo != "000000000000")
                {
                    DataSet ds = AccountManager.GetTransactionsForTransfer(FromAccountNo,
                                                DateRanges.Item(Pages).To, chxRowLimit.Checked, out this._availableTransfer, out Error);

                    //SundryTransactions = AccountManager.GetTransactionsForTransfer(SundryAccountNo, DateRanges.Item(Pages).To, chxRowLimit.Checked, out this._availableTransfer, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        Pages++;
                        btnPrevious.Enabled = true;

                        int index = ds.Tables[0].Rows.Count - 1;
                        DateRanges.Add(new DateRange((DateTime)ds.Tables[0].Rows[0][CN.DateTrans],
                            (DateTime)ds.Tables[0].Rows[index][CN.DateTrans]));

                        FormatDataGrid(ds);
                    }
                }
			}
			catch(Exception ex)
			{
				Catch(ex, "btnNext_Click");
			}
			finally
			{
				StopWait();
			}
		}

		private void btnPrevious_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				if(SundryAccountNo.Length > 0)
				{
					DateRanges.Remove(Pages);
					Pages--;
                    btnPrevious.Enabled = Pages > 1;    // was 0;  68674 jec 28/11/06

                    //SundryTransactions = AccountManager.GetTransactionsForTransfer(SundryAccountNo, DateRanges.Item(Pages).From, chxRowLimit.Checked, out this._availableTransfer, out Error);
                    SundryTransactions = AccountManager.GetTransactionsForTransfer(SundryAccountNo, DateRanges.Item(Pages-1).To, chxRowLimit.Checked, out this._availableTransfer, out Error);    // 68674 jec
					if(Error.Length>0)
						ShowError(Error);
					else
						FormatDataGrid(SundryTransactions);
				}
                // for specific account no  68674 jec 24/11/06
                if (FromAccountNo != "000000000000")
                {
                    DateRanges.Remove(Pages);
                    Pages--;
                    btnPrevious.Enabled = Pages > 1;    // was 0;  68674 jec 28/11/06

                    DataSet ds = AccountManager.GetTransactionsForTransfer(FromAccountNo,
                                                DateRanges.Item(Pages-1).To, chxRowLimit.Checked, out this._availableTransfer, out Error);  // (pages) 68674 jec
                    
                    //SundryTransactions = AccountManager.GetTransactionsForTransfer(SundryAccountNo, DateRanges.Item(Pages).From, chxRowLimit.Checked, out this._availableTransfer, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                        FormatDataGrid(ds);
                }
			}
			catch(Exception ex)
			{
				Catch(ex, "btnNext_Click");
			}
			finally
			{
				StopWait();
			}
		}
		private void btnTransfer_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				if(ValidateTransfer())
				{
					ProcessTransfer();
				}				
			}
			catch(Exception ex)
			{
				Catch(ex, "btnTransfer_Click");
			}
			finally
			{
				StopWait();
			}
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				//rbFromAccount.Checked = rbToAccount.Checked = true;

				this.txtDate.Text = "";
				this.txtFromAccountNo.ResetText();
				this.txtToAccountNo.ResetText();
				this.txtTransRefNo.Text = "";
				this.txtTransType.Text = "";
				this.txtValue.Text = "";
				// jec 02/01/07 this.txtValueAvailable.Text = "";
                this.txtValueAvailable.Text = "";   // reinstated jec 15/01/07
				this.txtValue.ReadOnly = false;
				this.FromAccountNo = "000000000000";
				this.ToAccountNo = "000000000000";
				this.txtName.Text = "";
				((MainForm)FormRoot).statusBar1.Text = "";
                chxRowLimit.Checked = true;
                chxRowLimit.Enabled = true;     //68674
                
				errors.SetError(txtFromAccountNo, "");
				errors.SetError(txtToAccountNo, "");
				errors.SetError(rbFromSundry, "");
				errors.SetError(rbToSundry, "");
				errors.SetError(rbToJournal, "");
				errors.SetError(txtValue, "");
                errors.SetError(txtTransType, "");      //IP - 21/02/12 - CR1234

				dgTransactions.DataSource = null;
				DateRanges.Clear();
				Pages = 0;
				btnPrevious.Enabled = false;
			}
			catch(Exception ex)
			{
				Catch(ex, "btnClear_Click");
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

		private void TransferTransaction_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
		{
			string fileName = this.Name + ".htm";
			LaunchHelp(fileName);
		}

		private void menuLaunchHelp_Click(object sender, System.EventArgs e)
		{
			TransferTransaction_HelpRequested(this, null);
		}

        private void menuFile_Click(object sender, EventArgs e)
        {

        }

	}
}
