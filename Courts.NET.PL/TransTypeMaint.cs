using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.PL.WS1;
using STL.PL.WS2;
using System.Data;
using STL.Common;
using STL.Common.Static;
using System.Web.Services.Protocols;
using System.Xml;
using System.Collections.Specialized;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using Crownwood.Magic.Menus;
using System.Threading;
using STL.Common.Constants.ScreenModes;
using STL.Common.Constants.AccountTypes;
namespace STL.PL
{
	/// <summary>
	/// Maintenance screen for transaction type codes. All monetary amounts 
	/// applied to customer accounts have a transaction type code. These codes
	/// define an amount as a delivery, payment or charge etc. This screen
	/// assigns a description to each code and defines how it should be interfaced
	/// to FACT 2000.
	/// </summary>
	public class TransTypeMaint : CommonForm
	{
		private new string Error = "";
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private DataSet transCodes = null;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.CheckBox chxBranch;
		private System.Windows.Forms.CheckBox chxGFT;
		private System.Windows.Forms.TextBox txtDescription;
		private System.Windows.Forms.TextBox txtIntAcctNo;
		private System.Windows.Forms.TextBox txtBalAcctNo;
		private System.Windows.Forms.TextBox txtTTCode;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtSecIntAcctNo;
		private System.Windows.Forms.ComboBox drpTransTypeCodes;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox drpType;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtEmpLastUpdated;
		private bool status = true;
		private System.Windows.Forms.CheckBox chxMandatory;
		private System.Windows.Forms.CheckBox chxUnique;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox txtBalSecAcctNo;
		private System.Windows.Forms.Label lAllowEdit;
		private System.Windows.Forms.CheckBox chxCentral;
        private TextBox txtSCBalAcctNo;
        private Label lblSCInterfaceBalancing;
        private Label lblSCInterfaceAccount;
        private TextBox txtSCIntAcctNo;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TransTypeMaint(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
		}

		public TransTypeMaint(Form root, Form parent)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});

			FormRoot = root;
			FormParent = parent;

			dynamicMenus = new Hashtable();
			HashMenus();
			ApplyRoleRestrictions();

			LoadData();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransTypeMaint));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.drpTransTypeCodes = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.chxBranch = new System.Windows.Forms.CheckBox();
            this.chxGFT = new System.Windows.Forms.CheckBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtIntAcctNo = new System.Windows.Forms.TextBox();
            this.txtBalAcctNo = new System.Windows.Forms.TextBox();
            this.txtTTCode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSecIntAcctNo = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSCBalAcctNo = new System.Windows.Forms.TextBox();
            this.lblSCInterfaceBalancing = new System.Windows.Forms.Label();
            this.lblSCInterfaceAccount = new System.Windows.Forms.Label();
            this.txtSCIntAcctNo = new System.Windows.Forms.TextBox();
            this.chxCentral = new System.Windows.Forms.CheckBox();
            this.lAllowEdit = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtBalSecAcctNo = new System.Windows.Forms.TextBox();
            this.chxUnique = new System.Windows.Forms.CheckBox();
            this.chxMandatory = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtEmpLastUpdated = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.drpType = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
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
            // drpTransTypeCodes
            // 
            this.drpTransTypeCodes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpTransTypeCodes.Location = new System.Drawing.Point(32, 72);
            this.drpTransTypeCodes.Name = "drpTransTypeCodes";
            this.drpTransTypeCodes.Size = new System.Drawing.Size(192, 21);
            this.drpTransTypeCodes.TabIndex = 1;
            this.drpTransTypeCodes.SelectedIndexChanged += new System.EventHandler(this.drpTransTypeCodes_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(226, 128);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 16);
            this.label5.TabIndex = 83;
            this.label5.Text = "Description";
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Location = new System.Drawing.Point(304, 72);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 13;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click_1);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 128);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 16);
            this.label6.TabIndex = 87;
            this.label6.Text = "Trans Type Code";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(25, 208);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(168, 16);
            this.label3.TabIndex = 82;
            this.label3.Text = "FACT2000 Debtors Account No";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(32, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 77;
            this.label1.Text = "TransType Code";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(242, 208);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(200, 16);
            this.label2.TabIndex = 80;
            this.label2.Text = "FACT2000 Corresponding Account No";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(384, 72);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(64, 24);
            this.btnClear.TabIndex = 14;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click_1);
            // 
            // chxBranch
            // 
            this.chxBranch.Location = new System.Drawing.Point(515, 168);
            this.chxBranch.Name = "chxBranch";
            this.chxBranch.Size = new System.Drawing.Size(120, 16);
            this.chxBranch.TabIndex = 9;
            this.chxBranch.Text = "Allocate to Branch";
            // 
            // chxGFT
            // 
            this.chxGFT.Location = new System.Drawing.Point(515, 136);
            this.chxGFT.Name = "chxGFT";
            this.chxGFT.Size = new System.Drawing.Size(232, 16);
            this.chxGFT.TabIndex = 8;
            this.chxGFT.Text = "Include In General Financial Transactions";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(229, 144);
            this.txtDescription.MaxLength = 40;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(216, 20);
            this.txtDescription.TabIndex = 3;
            // 
            // txtIntAcctNo
            // 
            this.txtIntAcctNo.Location = new System.Drawing.Point(25, 224);
            this.txtIntAcctNo.MaxLength = 8;
            this.txtIntAcctNo.Name = "txtIntAcctNo";
            this.txtIntAcctNo.Size = new System.Drawing.Size(64, 20);
            this.txtIntAcctNo.TabIndex = 4;
            this.txtIntAcctNo.Leave += new System.EventHandler(this.txtIntAcctNo_Leave);
            // 
            // txtBalAcctNo
            // 
            this.txtBalAcctNo.Location = new System.Drawing.Point(229, 208);
            this.txtBalAcctNo.MaxLength = 8;
            this.txtBalAcctNo.Name = "txtBalAcctNo";
            this.txtBalAcctNo.Size = new System.Drawing.Size(64, 20);
            this.txtBalAcctNo.TabIndex = 5;
            this.txtBalAcctNo.TextChanged += new System.EventHandler(this.txtBalAcctNo_TextChanged);
            this.txtBalAcctNo.Leave += new System.EventHandler(this.txtBalAcctNo_Leave);
            // 
            // txtTTCode
            // 
            this.txtTTCode.Location = new System.Drawing.Point(9, 144);
            this.txtTTCode.MaxLength = 3;
            this.txtTTCode.Name = "txtTTCode";
            this.txtTTCode.Size = new System.Drawing.Size(56, 20);
            this.txtTTCode.TabIndex = 2;
            this.txtTTCode.Leave += new System.EventHandler(this.txtTTCode_Leave);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(25, 297);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(192, 16);
            this.label4.TabIndex = 86;
            this.label4.Text = "FACT2000 Securitisation Account No";
            // 
            // txtSecIntAcctNo
            // 
            this.txtSecIntAcctNo.Location = new System.Drawing.Point(25, 313);
            this.txtSecIntAcctNo.MaxLength = 8;
            this.txtSecIntAcctNo.Name = "txtSecIntAcctNo";
            this.txtSecIntAcctNo.Size = new System.Drawing.Size(64, 20);
            this.txtSecIntAcctNo.TabIndex = 6;
            this.txtSecIntAcctNo.Leave += new System.EventHandler(this.txtSecIntAcctNo_Leave);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSCBalAcctNo);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lblSCInterfaceBalancing);
            this.groupBox1.Controls.Add(this.lblSCInterfaceAccount);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtSCIntAcctNo);
            this.groupBox1.Controls.Add(this.chxCentral);
            this.groupBox1.Controls.Add(this.lAllowEdit);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtBalSecAcctNo);
            this.groupBox1.Controls.Add(this.txtDescription);
            this.groupBox1.Controls.Add(this.chxUnique);
            this.groupBox1.Controls.Add(this.chxMandatory);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtEmpLastUpdated);
            this.groupBox1.Controls.Add(this.txtTTCode);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.drpType);
            this.groupBox1.Controls.Add(this.txtBalAcctNo);
            this.groupBox1.Location = new System.Drawing.Point(16, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(736, 328);
            this.groupBox1.TabIndex = 89;
            this.groupBox1.TabStop = false;
            // 
            // txtSCBalAcctNo
            // 
            this.txtSCBalAcctNo.Location = new System.Drawing.Point(231, 253);
            this.txtSCBalAcctNo.MaxLength = 8;
            this.txtSCBalAcctNo.Name = "txtSCBalAcctNo";
            this.txtSCBalAcctNo.Size = new System.Drawing.Size(64, 20);
            this.txtSCBalAcctNo.TabIndex = 102;
            this.txtSCBalAcctNo.Leave += new System.EventHandler(this.txtSCBalAcctNo_Leave);
            // 
            // lblSCInterfaceBalancing
            // 
            this.lblSCInterfaceBalancing.Location = new System.Drawing.Point(228, 237);
            this.lblSCInterfaceBalancing.Name = "lblSCInterfaceBalancing";
            this.lblSCInterfaceBalancing.Size = new System.Drawing.Size(262, 13);
            this.lblSCInterfaceBalancing.TabIndex = 101;
            this.lblSCInterfaceBalancing.Text = "FACT2000 Corresponding Account No Store Card";
            // 
            // lblSCInterfaceAccount
            // 
            this.lblSCInterfaceAccount.Location = new System.Drawing.Point(9, 237);
            this.lblSCInterfaceAccount.Name = "lblSCInterfaceAccount";
            this.lblSCInterfaceAccount.Size = new System.Drawing.Size(222, 13);
            this.lblSCInterfaceAccount.TabIndex = 100;
            this.lblSCInterfaceAccount.Text = "FACT2000 Debtors Account No Store Card";
            // 
            // txtSCIntAcctNo
            // 
            this.txtSCIntAcctNo.Location = new System.Drawing.Point(9, 253);
            this.txtSCIntAcctNo.MaxLength = 8;
            this.txtSCIntAcctNo.Name = "txtSCIntAcctNo";
            this.txtSCIntAcctNo.Size = new System.Drawing.Size(64, 20);
            this.txtSCIntAcctNo.TabIndex = 99;
            this.txtSCIntAcctNo.Leave += new System.EventHandler(this.txtSCIntAcctNo_Leave);
            // 
            // chxCentral
            // 
            this.chxCentral.Location = new System.Drawing.Point(499, 248);
            this.chxCentral.Name = "chxCentral";
            this.chxCentral.Size = new System.Drawing.Size(216, 16);
            this.chxCentral.TabIndex = 98;
            this.chxCentral.Text = "Corresponding Allocate to Branch";
            // 
            // lAllowEdit
            // 
            this.lAllowEdit.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lAllowEdit.Enabled = false;
            this.lAllowEdit.Location = new System.Drawing.Point(382, 61);
            this.lAllowEdit.Name = "lAllowEdit";
            this.lAllowEdit.Size = new System.Drawing.Size(32, 16);
            this.lAllowEdit.TabIndex = 97;
            this.lAllowEdit.Visible = false;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(228, 278);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(272, 16);
            this.label9.TabIndex = 96;
            this.label9.Text = "FACT 2000 Securitisation Corresponding Account No";
            // 
            // txtBalSecAcctNo
            // 
            this.txtBalSecAcctNo.Location = new System.Drawing.Point(231, 297);
            this.txtBalSecAcctNo.MaxLength = 8;
            this.txtBalSecAcctNo.Name = "txtBalSecAcctNo";
            this.txtBalSecAcctNo.Size = new System.Drawing.Size(64, 20);
            this.txtBalSecAcctNo.TabIndex = 7;
            this.txtBalSecAcctNo.Leave += new System.EventHandler(this.txtBalSecAcctNo_Leave);
            // 
            // chxUnique
            // 
            this.chxUnique.Location = new System.Drawing.Point(499, 216);
            this.chxUnique.Name = "chxUnique";
            this.chxUnique.Size = new System.Drawing.Size(136, 16);
            this.chxUnique.TabIndex = 11;
            this.chxUnique.Text = "Unique Reference  No.";
            // 
            // chxMandatory
            // 
            this.chxMandatory.Location = new System.Drawing.Point(499, 184);
            this.chxMandatory.Name = "chxMandatory";
            this.chxMandatory.Size = new System.Drawing.Size(176, 16);
            this.chxMandatory.TabIndex = 10;
            this.chxMandatory.Text = "Reference No. As Mandatory";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(496, 44);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(104, 16);
            this.label8.TabIndex = 92;
            this.label8.Text = "Last Updated By:";
            // 
            // txtEmpLastUpdated
            // 
            this.txtEmpLastUpdated.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtEmpLastUpdated.Location = new System.Drawing.Point(496, 61);
            this.txtEmpLastUpdated.MaxLength = 10;
            this.txtEmpLastUpdated.Name = "txtEmpLastUpdated";
            this.txtEmpLastUpdated.ReadOnly = true;
            this.txtEmpLastUpdated.Size = new System.Drawing.Size(168, 20);
            this.txtEmpLastUpdated.TabIndex = 91;
            this.txtEmpLastUpdated.TabStop = false;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(499, 280);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(168, 16);
            this.label7.TabIndex = 90;
            this.label7.Text = "Include As Deposit Transaction";
            // 
            // drpType
            // 
            this.drpType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpType.Items.AddRange(new object[] {
            "",
            "Deposit",
            "Petty Cash",
            "Disbursement",
            "Petty Cash (Tax-Exempt)",
            "Disbursement (Tax-Exempt)",
            "Petty Cash (Tax-Only)",
            "Disbursement (Tax-Only)",
            "Disbursement (Remittance)"});
            this.drpType.Location = new System.Drawing.Point(499, 296);
            this.drpType.Name = "drpType";
            this.drpType.Size = new System.Drawing.Size(168, 21);
            this.drpType.TabIndex = 12;
            this.drpType.SelectedIndexChanged += new System.EventHandler(this.drpType_SelectedIndexChanged);
            // 
            // TransTypeMaint
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(768, 365);
            this.Controls.Add(this.drpTransTypeCodes);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.chxBranch);
            this.Controls.Add(this.chxGFT);
            this.Controls.Add(this.txtIntAcctNo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtSecIntAcctNo);
            this.Controls.Add(this.groupBox1);
            this.Name = "TransTypeMaint";
            this.Text = "Transaction Type Maintenance";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void LoadData()
		{
			try
			{
				StringCollection codes = new StringCollection(); 	
				codes.Add("Codes");

				transCodes = AccountManager.GetTranstypeByCode("", out Error);

				if(Error.Length > 0)
					ShowError(Error);
				else
				{
					foreach(DataRow row in transCodes.Tables[0].Rows)
						codes.Add((string)(row[CN.TransTypeCode])+" : "+ (string)(row[CN.Description]));

					drpTransTypeCodes.DataSource = codes;
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void menuExit_Click(object sender, System.EventArgs e)
		{
			CloseTab();
		}

		private void drpTransTypeCodes_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			foreach(DataRow row in transCodes.Tables[0].Rows)
			{
				string selectedCode = (string)drpTransTypeCodes.SelectedItem;

				if(selectedCode.Substring(0,3) == (string)row[CN.TransTypeCode])
				{
					txtTTCode.Text = (string)row[CN.TransTypeCode];
					txtDescription.Text = (string)row[CN.Description];
					txtSecIntAcctNo.Text = (string)row[CN.InterfaceSecAccount];
					txtIntAcctNo.Text = (string)row[CN.InterfaceAccount];
					txtBalAcctNo.Text = (string)row[CN.InterfaceBalancing];
                    txtSCIntAcctNo.Text = (string)row[CN.SCInterfaceAccount];                       //IP - 11/04/12 - CR9863 - #9885
                    txtSCBalAcctNo.Text = (string)row[CN.SCInterfaceBalancing];                     //IP - 11/04/12 - CR9863 - #9885
					txtBalSecAcctNo.Text = (string)row[CN.InterfaceSecBalancing];
					chxGFT.Checked = Convert.ToBoolean(row[CN.IncludeINGFT]);
					chxBranch.Checked = Convert.ToBoolean(row[CN.BranchSplit]);
					drpType.SelectedIndex = Convert.ToInt16(row[CN.IsDeposit]);
					GetUserName(Convert.ToInt32(row[CN.EmpeeNoChange]));
					chxMandatory.Checked = Convert.ToBoolean(row[CN.RefNoMandatory]);
					chxUnique.Checked = Convert.ToBoolean(row[CN.RefNoUnique]);
					chxCentral.Checked = Convert.ToBoolean(row[CN.BranchSplitBalancing]);
				}
			}
		}

		private void btnSave_Click_1(object sender, System.EventArgs e)
		{
			try
			{
				//show the hour glass
				Wait();
                status = true; //NM - Status must be reset to true before doing any validations

				if(!lAllowEdit.Enabled)
				{
					ShowInfo("M_SAVETRANSACTIONTYPECHECK");
					status = false;
				}
				
				if(status) ValidateLength(txtBalAcctNo.Text);
				if(status) ValidateLength(txtIntAcctNo.Text);
				if(status) ValidateLength(txtSecIntAcctNo.Text);
				if(status) ValidateLength(txtBalSecAcctNo.Text);
                if(status) ValidateLength(txtSCIntAcctNo.Text);                 //IP - 11/04/12 - CR9863 - #9885
                if(status) ValidateLength(txtSCBalAcctNo.Text);                 //IP - 11/04/12 - CR9863 - #9885

				if(status)
				{  // if one field is not blank then corresponding 
					if(txtBalAcctNo.Text.Length < 4 && txtIntAcctNo.Text.Length >=4 || txtSecIntAcctNo.Text.Length <4 && txtBalSecAcctNo.Text.Length >=4
                        || txtSecIntAcctNo.Text.Length >= 4 && txtBalSecAcctNo.Text.Length < 4 || txtBalAcctNo.Text.Length >= 4 && txtIntAcctNo.Text.Length < 4
                        || txtSCIntAcctNo.Text.Length < 4 && txtSCBalAcctNo.Text.Length >=4 || txtSCIntAcctNo.Text.Length >=4 && txtSCBalAcctNo.Text.Length < 4)        //IP - 11/04/12 - CR9863 - #9885
					{
						ShowInfo("M_TRANSACTIONTYPESAVE");
						status = false;
					}
					else status = true;
				}

				if(status)
				{
					if((string)drpTransTypeCodes.SelectedItem == "Codes" && txtTTCode.Text.Length == 0)
					{
						ShowInfo("M_TRANSACTIONTYPECHECK");
						status = false;
					}
					else status = true;
				}
				
				if(status)
				{
					if (drpType.SelectedIndex == -1)
						drpType.SelectedIndex = 0;

					AccountManager.SaveTransType(txtTTCode.Text, txtDescription.Text, 
						Convert.ToInt16(chxGFT.Checked), txtSecIntAcctNo.Text, txtIntAcctNo.Text,
						Convert.ToInt16(chxBranch.Checked), Convert.ToInt16(drpType.SelectedIndex),
						txtBalAcctNo.Text, Convert.ToInt16(chxMandatory.Checked), 
						Convert.ToInt16(chxUnique.Checked), txtBalSecAcctNo.Text, Convert.ToInt16(chxCentral.Checked), 
                        txtSCIntAcctNo.Text, txtSCBalAcctNo.Text, out Error);                                                               //IP - 11/04/12 - CR9863 - #9885

					if(Error.Length > 0)
						ShowError(Error);
					else
					{
						btnClear_Click_1(this, null);
						LoadData();
					}
				}

				StopWait();
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void btnClear_Click_1(object sender, System.EventArgs e)
		{
			drpTransTypeCodes.SelectedIndex = 0;
			txtTTCode.Text = "";
			txtDescription.Text = "";
			txtSecIntAcctNo.Text = "";
			txtIntAcctNo.Text = "";
			txtBalAcctNo.Text = "";
			txtBalSecAcctNo.Text = "";
			chxGFT.Checked = false;
			chxBranch.Checked = false;
			drpType.SelectedIndex = -1;
			txtEmpLastUpdated.Text = "";
			chxMandatory.Checked = false;
			chxUnique.Checked = false;
		}

		private void drpType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(drpType.SelectedIndex == 1 && lAllowEdit.Enabled)//deposits
			{
				txtSecIntAcctNo.Enabled = false;
				txtBalSecAcctNo.Enabled = false;
				txtSecIntAcctNo.Text = "";
				txtBalSecAcctNo.Text = "";
			}
			else
			{
				txtSecIntAcctNo.Enabled = true;
				txtBalSecAcctNo.Enabled = true;
			}

			if(drpType.SelectedIndex == 0 && lAllowEdit.Enabled)//Financial Transaction
			{
				chxMandatory.Enabled = false;
				chxUnique.Enabled = false;
				chxMandatory.Checked = false;
				chxUnique.Checked = false;
			}
			else
			{
				chxMandatory.Enabled = true;
				chxUnique.Enabled = true;
			}

		}

		private void GetUserName(int empNo)
		{
			try
			{
				string empName = Login.GetEmployeeName(empNo, out Error);
				if(Error.Length > 0)
					ShowError(Error);
				else
				{
					if(empName.Length > 0)
						txtEmpLastUpdated.Text = empNo + " : " + empName;
					else
						txtEmpLastUpdated.Text = "";
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void txtBalAcctNo_Leave(object sender, System.EventArgs e)
		{
			ValidateLength(txtBalAcctNo.Text);
		}

		private void txtTTCode_Leave(object sender, System.EventArgs e)
		{
			foreach(DataRow row in transCodes.Tables[0].Rows)
			{
				if(txtTTCode.Text == (string)row[CN.TransTypeCode])// && Convert.ToInt16(row[CN.IsDeposit]) == 0)
				{
					txtTTCode.Text = (string)row[CN.TransTypeCode];
					txtDescription.Text = (string)row[CN.Description];
					txtSecIntAcctNo.Text = (string)row[CN.InterfaceSecAccount];
					txtIntAcctNo.Text = (string)row[CN.InterfaceAccount];
					txtBalAcctNo.Text = (string)row[CN.InterfaceBalancing];
					txtBalSecAcctNo.Text = (string)row[CN.InterfaceSecBalancing];
					txtBalSecAcctNo.Text = (string)row[CN.InterfaceSecBalancing];
					chxGFT.Checked = Convert.ToBoolean(row[CN.IncludeINGFT]);
					chxBranch.Checked = Convert.ToBoolean(row[CN.BranchSplit]);
					drpType.SelectedIndex = Convert.ToInt16(row[CN.IsDeposit]);
					GetUserName(Convert.ToInt32(row[CN.EmpeeNoChange]));
					chxMandatory.Checked = Convert.ToBoolean(row[CN.RefNoMandatory]);
					chxUnique.Checked = Convert.ToBoolean(row[CN.RefNoUnique]);
					chxCentral.Checked = Convert.ToBoolean(row[CN.BranchSplitBalancing]);
				}
			}
		}

        //private void SetFields(bool setting)
        //{
        //    txtTTCode.Enabled = setting;
        //    txtDescription.Enabled = setting;
        //    txtSecIntAcctNo.Enabled = setting;
        //    txtIntAcctNo.Enabled = setting;
        //    txtBalAcctNo.Enabled = setting;
        //    txtBalSecAcctNo.Enabled = setting;
        //    chxGFT.Enabled = setting;
        //    chxBranch.Enabled = setting;
        //    drpType.Enabled = setting;
        //    chxMandatory.Enabled = setting;
        //    chxUnique.Enabled = setting;
        //}

		private void HashMenus()
		{
			dynamicMenus[this.Name+":lAllowEdit"] = this.lAllowEdit; 
		}

		private void txtIntAcctNo_Leave(object sender, System.EventArgs e)
		{
			ValidateLength(txtIntAcctNo.Text);
		}

		private void txtSecIntAcctNo_Leave(object sender, System.EventArgs e)
		{
			ValidateLength(txtSecIntAcctNo.Text);
		}

		private void txtBalSecAcctNo_Leave(object sender, System.EventArgs e)
		{
			ValidateLength(txtBalSecAcctNo.Text);
		}

		private void ValidateLength(string factAcctNo)
		{
            //if (factAcctNo.Length != 4 && factAcctNo.Length != 0 && factAcctNo.Length !=5) //71101 --Allow 5 digits for Service request balancing account number
            if (factAcctNo.Length != 4 && factAcctNo.Length != 0 && factAcctNo.Length != 5 && factAcctNo.Length < 4) //IP - 19/08/11 - #4567 - UAT57 - Cater for Oracle codes which can be 8 digits.
			{
				ShowInfo("M_FACTACCTNO");
				status = false;
			}
			else
				status = true;
		}

        private void txtBalAcctNo_TextChanged(object sender, EventArgs e)
        {

        }

        //IP - 11/04/12 - CR9863 - #9885
        private void txtSCIntAcctNo_Leave(object sender, EventArgs e)
        {
            ValidateLength(txtSCIntAcctNo.Text);
        }

        //IP - 11/04/12 - CR9863 - #9885
        private void txtSCBalAcctNo_Leave(object sender, EventArgs e)
        {
            ValidateLength(txtSCBalAcctNo.Text);
        }
	}
}
 