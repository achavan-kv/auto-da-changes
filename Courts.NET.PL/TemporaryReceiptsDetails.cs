using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Data;
using System.Web.Services.Protocols;
using STL.PL.WS1;
using STL.PL.WS2;
using STL.PL.WS3;
using STL.PL.WS4;
using STL.PL.WS5;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using System.Collections.Specialized;
using System.Xml;
using STL.Common.Constants.Elements;
using STL.Common.Constants.Tags;
using Crownwood.Magic.Menus;
using STL.Common.Constants.ItemTypes;
using STL.Common.Static;
using Blue.Cosacs.Shared;

namespace STL.PL
{
	/// <summary>
	/// Summary description for TemporaryReceiptsDetails.
	/// </summary>
	public class TemporaryReceiptsDetails : CommonForm
	{
		private System.Windows.Forms.NotifyIcon notifyIcon1;
		private System.ComponentModel.IContainer components;
		private string errorTxt;
		private Crownwood.Magic.Controls.TabControl tcMain;
		private Crownwood.Magic.Controls.TabPage tpTempReceiptInvest;
		private Crownwood.Magic.Controls.TabPage tpTempReceiptAllocation;
		private System.Windows.Forms.TextBox txtAlloEmpeeNo;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Button btnReallocate;
		private System.Windows.Forms.Button btnAllocate;
		private System.Windows.Forms.Button btnAllClear;
		private System.Windows.Forms.ComboBox drpAlloStaffCat;
		DataGridTableStyle tabStyle = new DataGridTableStyle();
		private System.Windows.Forms.ComboBox drpAlloStaffMemeber;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.NumericUpDown numAllocLast;
		private System.Windows.Forms.NumericUpDown numAllocFirst;
        private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox txtEmpeeName;
		private System.Windows.Forms.TextBox txtEmpeeNo;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.NumericUpDown numLastReceipt;
		private System.Windows.Forms.NumericUpDown numEmpoyeeNo;
		private System.Windows.Forms.NumericUpDown numFirstReceipt;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnFind;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox txtAmount;
		public STL.PL.AccountTextBox txtAccountNumber;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtDatePresented;
		private System.Windows.Forms.TextBox txtDateAllocated;
		private System.Windows.Forms.TextBox txtBranchNo;
		private System.Windows.Forms.TextBox txtReceiptNo;
		private System.Windows.Forms.DataGrid dgTempReceipts;
		private System.Windows.Forms.Button btnCancelReceipt;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.DateTimePicker dtDateIssued;
		private bool staticLoaded = false;
		private System.Windows.Forms.Label lInvestigate;
		private System.Windows.Forms.Label lAllocate;
		private string _screenType = "Enquiry";

		public TemporaryReceiptsDetails(Form root, Form parent)
		{
			//
			// Required for Windows Form Designer support
			//
			InitialiseGenericData (root, parent);
			btnCancelReceipt.Text="Void";
			btnCancelReceipt.Enabled = false;
			
		}
		public TemporaryReceiptsDetails(int empeeNo, Form root, Form parent)
		{
			//
			// Required for Windows Form Designer support
			//
			InitialiseGenericData (root, parent);
			numEmpoyeeNo.Value = empeeNo;
			doBailiffTempReceiptSearch();
			btnCancelReceipt.Text="Cancel Receipt";
			_screenType = "BailiffReview";
			btnClear.Enabled = false;
			btnFind.Enabled = false;

			
		}

		private void HashMenus()
		{
			dynamicMenus[this.Name+":lInvestigate"] = this.lInvestigate; 
			dynamicMenus[this.Name+":lAllocate"] = this.lAllocate; 
		}

		private void InitialiseGenericData(Form root, Form parent)	
		{
			int nextTempReceiptNo =0;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			dynamicMenus = new Hashtable();
			HashMenus();
			ApplyRoleRestrictions();

			FormRoot = root;
			FormParent = parent;
			InitialiseStaticData();
			dtDateIssued.Value = DateTime.Today;
			PaymentManager.GetNextTemporaryReceptNo(ref nextTempReceiptNo, out errorTxt);
			numAllocFirst.Value =  nextTempReceiptNo + 1;
			numAllocLast.Value = Convert.ToInt32(Country[CountryParameterNames.ReceiptsPerBook]) +  (int)(numAllocFirst.Value);

		}
		private void InitialiseStaticData()	
		{		
			try
			{
				Function = "TemporaryReceiptsDetails::InitialiseStaticData";
				XmlUtilities xml = new XmlUtilities();
				XmlDocument dropDowns = new XmlDocument();
				dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
							
				StringCollection empTypes = new StringCollection();
				empTypes.Add("Staff Types");
		
				if(StaticData.Tables[TN.EmployeeTypes]==null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.EmployeeTypes,
						new string[] {"ET1", "L"}));    
				
				if(dropDowns.DocumentElement.ChildNodes.Count>0)
				{
					DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out errorTxt);
					if (errorTxt.Length > 0)
						ShowError(errorTxt);
					else
					{
						foreach(DataTable dt in ds.Tables)
							StaticData.Tables[dt.TableName] = dt;
					}
				}

			   foreach (DataRow row in CollectionsManager.RolesGet((int)CosacsPermissionEnum.Bailiff).Tables[0].Rows)
                {
                        string str = string.Format("{0} : {1}",row[0],row[1]);
						empTypes.Add(str.ToUpper());
				}
				drpAlloStaffCat.DataSource = empTypes;

				StringCollection empMembers = new StringCollection();
				empMembers.Add("Staff Members");
				drpAlloStaffMemeber.DataSource = empMembers;

				staticLoaded = true;
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
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
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.tcMain = new Crownwood.Magic.Controls.TabControl();
            this.tpTempReceiptInvest = new Crownwood.Magic.Controls.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtEmpeeName = new System.Windows.Forms.TextBox();
            this.txtEmpeeNo = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numLastReceipt = new System.Windows.Forms.NumericUpDown();
            this.numEmpoyeeNo = new System.Windows.Forms.NumericUpDown();
            this.numFirstReceipt = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnFind = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.txtAccountNumber = new STL.PL.AccountTextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDatePresented = new System.Windows.Forms.TextBox();
            this.txtDateAllocated = new System.Windows.Forms.TextBox();
            this.txtBranchNo = new System.Windows.Forms.TextBox();
            this.txtReceiptNo = new System.Windows.Forms.TextBox();
            this.dgTempReceipts = new System.Windows.Forms.DataGrid();
            this.btnCancelReceipt = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.tpTempReceiptAllocation = new Crownwood.Magic.Controls.TabPage();
            this.dtDateIssued = new System.Windows.Forms.DateTimePicker();
            this.numAllocLast = new System.Windows.Forms.NumericUpDown();
            this.numAllocFirst = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.btnAllClear = new System.Windows.Forms.Button();
            this.btnAllocate = new System.Windows.Forms.Button();
            this.btnReallocate = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.drpAlloStaffMemeber = new System.Windows.Forms.ComboBox();
            this.drpAlloStaffCat = new System.Windows.Forms.ComboBox();
            this.txtAlloEmpeeNo = new System.Windows.Forms.TextBox();
            this.lInvestigate = new System.Windows.Forms.Label();
            this.lAllocate = new System.Windows.Forms.Label();
            this.tcMain.SuspendLayout();
            this.tpTempReceiptInvest.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLastReceipt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEmpoyeeNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFirstReceipt)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgTempReceipts)).BeginInit();
            this.tpTempReceiptAllocation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAllocLast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAllocFirst)).BeginInit();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // tcMain
            // 
            this.tcMain.IDEPixelArea = true;
            this.tcMain.Location = new System.Drawing.Point(8, 0);
            this.tcMain.Name = "tcMain";
            this.tcMain.PositionTop = true;
            this.tcMain.SelectedIndex = 0;
            this.tcMain.SelectedTab = this.tpTempReceiptInvest;
            this.tcMain.Size = new System.Drawing.Size(776, 456);
            this.tcMain.TabIndex = 14;
            this.tcMain.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpTempReceiptInvest,
            this.tpTempReceiptAllocation});
            // 
            // tpTempReceiptInvest
            // 
            this.tpTempReceiptInvest.Controls.Add(this.dgTempReceipts);
            this.tpTempReceiptInvest.Controls.Add(this.lInvestigate);
            this.tpTempReceiptInvest.Controls.Add(this.groupBox3);
            this.tpTempReceiptInvest.Controls.Add(this.lAllocate);
            this.tpTempReceiptInvest.Controls.Add(this.groupBox2);
            this.tpTempReceiptInvest.Controls.Add(this.groupBox1);
            this.tpTempReceiptInvest.Controls.Add(this.btnCancelReceipt);
            this.tpTempReceiptInvest.Controls.Add(this.btnClear);
            this.tpTempReceiptInvest.Location = new System.Drawing.Point(0, 25);
            this.tpTempReceiptInvest.Name = "tpTempReceiptInvest";
            this.tpTempReceiptInvest.Size = new System.Drawing.Size(776, 431);
            this.tpTempReceiptInvest.TabIndex = 3;
            this.tpTempReceiptInvest.Title = "Temporary Receipt Investigation";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.txtEmpeeName);
            this.groupBox3.Controls.Add(this.txtEmpeeNo);
            this.groupBox3.Location = new System.Drawing.Point(496, 16);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(232, 104);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Allocated To";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(8, 48);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(40, 23);
            this.label12.TabIndex = 6;
            this.label12.Text = "Name:";
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(8, 24);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 23);
            this.label11.TabIndex = 5;
            this.label11.Text = "Staff No:";
            // 
            // txtEmpeeName
            // 
            this.txtEmpeeName.Location = new System.Drawing.Point(118, 48);
            this.txtEmpeeName.Name = "txtEmpeeName";
            this.txtEmpeeName.ReadOnly = true;
            this.txtEmpeeName.Size = new System.Drawing.Size(108, 23);
            this.txtEmpeeName.TabIndex = 2;
            // 
            // txtEmpeeNo
            // 
            this.txtEmpeeNo.Location = new System.Drawing.Point(118, 24);
            this.txtEmpeeNo.Name = "txtEmpeeNo";
            this.txtEmpeeNo.ReadOnly = true;
            this.txtEmpeeNo.Size = new System.Drawing.Size(108, 23);
            this.txtEmpeeNo.TabIndex = 0;
            this.txtEmpeeNo.Text = "0";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numLastReceipt);
            this.groupBox2.Controls.Add(this.numEmpoyeeNo);
            this.groupBox2.Controls.Add(this.numFirstReceipt);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.btnFind);
            this.groupBox2.Location = new System.Drawing.Point(48, 312);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(376, 104);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Enquiry Details ";
            // 
            // numLastReceipt
            // 
            this.numLastReceipt.Location = new System.Drawing.Point(139, 48);
            this.numLastReceipt.Maximum = new decimal(new int[] {
            -727379969,
            232,
            0,
            0});
            this.numLastReceipt.Name = "numLastReceipt";
            this.numLastReceipt.Size = new System.Drawing.Size(104, 23);
            this.numLastReceipt.TabIndex = 14;
            this.numLastReceipt.Leave += new System.EventHandler(this.numLastReceipt_Leave);
            // 
            // numEmpoyeeNo
            // 
            this.numEmpoyeeNo.Location = new System.Drawing.Point(139, 72);
            this.numEmpoyeeNo.Maximum = new decimal(new int[] {
            2147483646,
            0,
            0,
            0});
            this.numEmpoyeeNo.Name = "numEmpoyeeNo";
            this.numEmpoyeeNo.Size = new System.Drawing.Size(104, 23);
            this.numEmpoyeeNo.TabIndex = 15;
            this.numEmpoyeeNo.Leave += new System.EventHandler(this.numEmpoyeeNo_Leave);
            // 
            // numFirstReceipt
            // 
            this.numFirstReceipt.Location = new System.Drawing.Point(139, 24);
            this.numFirstReceipt.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numFirstReceipt.Name = "numFirstReceipt";
            this.numFirstReceipt.Size = new System.Drawing.Size(104, 23);
            this.numFirstReceipt.TabIndex = 13;
            this.numFirstReceipt.Leave += new System.EventHandler(this.numFirstReceipt_Leave);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 16);
            this.label3.TabIndex = 12;
            this.label3.Text = "OR   Employee No:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "Last Receipt No:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "First Receipt No:";
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(274, 40);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(75, 23);
            this.btnFind.TabIndex = 10;
            this.btnFind.Text = "Find";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtAmount);
            this.groupBox1.Controls.Add(this.txtAccountNumber);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtDatePresented);
            this.groupBox1.Controls.Add(this.txtDateAllocated);
            this.groupBox1.Controls.Add(this.txtBranchNo);
            this.groupBox1.Controls.Add(this.txtReceiptNo);
            this.groupBox1.Location = new System.Drawing.Point(496, 120);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(232, 192);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Temporary Receipt Details";
            // 
            // txtAmount
            // 
            this.txtAmount.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.txtAmount.Location = new System.Drawing.Point(118, 152);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.ReadOnly = true;
            this.txtAmount.Size = new System.Drawing.Size(108, 23);
            this.txtAmount.TabIndex = 16;
            this.txtAmount.TabStop = false;
            this.txtAmount.Text = "0";
            this.txtAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtAccountNumber
            // 
            this.txtAccountNumber.Location = new System.Drawing.Point(118, 128);
            this.txtAccountNumber.Name = "txtAccountNumber";
            this.txtAccountNumber.PreventPaste = false;
            this.txtAccountNumber.ReadOnly = true;
            this.txtAccountNumber.Size = new System.Drawing.Size(108, 23);
            this.txtAccountNumber.TabIndex = 15;
            this.txtAccountNumber.TabStop = false;
            this.txtAccountNumber.Text = "000-0000-0000-0";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(8, 152);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(92, 23);
            this.label10.TabIndex = 14;
            this.label10.Text = "Amount:";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(8, 104);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(92, 23);
            this.label9.TabIndex = 13;
            this.label9.Text = "Date Presented:";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(8, 128);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 23);
            this.label7.TabIndex = 12;
            this.label7.Text = "Account No:";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 23);
            this.label5.TabIndex = 11;
            this.label5.Text = "Date Allocated:";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(8, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(92, 23);
            this.label8.TabIndex = 10;
            this.label8.Text = "Branch:";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(8, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 23);
            this.label6.TabIndex = 8;
            this.label6.Text = "Receipt No:";
            // 
            // txtDatePresented
            // 
            this.txtDatePresented.Location = new System.Drawing.Point(118, 104);
            this.txtDatePresented.Name = "txtDatePresented";
            this.txtDatePresented.ReadOnly = true;
            this.txtDatePresented.Size = new System.Drawing.Size(108, 23);
            this.txtDatePresented.TabIndex = 3;
            // 
            // txtDateAllocated
            // 
            this.txtDateAllocated.Location = new System.Drawing.Point(118, 80);
            this.txtDateAllocated.Name = "txtDateAllocated";
            this.txtDateAllocated.ReadOnly = true;
            this.txtDateAllocated.Size = new System.Drawing.Size(108, 23);
            this.txtDateAllocated.TabIndex = 2;
            // 
            // txtBranchNo
            // 
            this.txtBranchNo.Location = new System.Drawing.Point(118, 48);
            this.txtBranchNo.Name = "txtBranchNo";
            this.txtBranchNo.ReadOnly = true;
            this.txtBranchNo.Size = new System.Drawing.Size(108, 23);
            this.txtBranchNo.TabIndex = 1;
            // 
            // txtReceiptNo
            // 
            this.txtReceiptNo.Location = new System.Drawing.Point(118, 24);
            this.txtReceiptNo.Name = "txtReceiptNo";
            this.txtReceiptNo.ReadOnly = true;
            this.txtReceiptNo.Size = new System.Drawing.Size(108, 23);
            this.txtReceiptNo.TabIndex = 0;
            // 
            // dgTempReceipts
            // 
            this.dgTempReceipts.CaptionText = "Temporary Receipts Issued by Bailiff";
            this.dgTempReceipts.DataMember = "";
            this.dgTempReceipts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgTempReceipts.Location = new System.Drawing.Point(48, 12);
            this.dgTempReceipts.Name = "dgTempReceipts";
            this.dgTempReceipts.ReadOnly = true;
            this.dgTempReceipts.Size = new System.Drawing.Size(416, 300);
            this.dgTempReceipts.TabIndex = 16;
            this.dgTempReceipts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgTempReceipts_MouseUp);
            // 
            // btnCancelReceipt
            // 
            this.btnCancelReceipt.Enabled = false;
            this.btnCancelReceipt.Location = new System.Drawing.Point(592, 352);
            this.btnCancelReceipt.Name = "btnCancelReceipt";
            this.btnCancelReceipt.Size = new System.Drawing.Size(64, 24);
            this.btnCancelReceipt.TabIndex = 20;
            this.btnCancelReceipt.Text = "Cancel Receipt";
            this.btnCancelReceipt.Click += new System.EventHandler(this.btnCancelReceipt_Click);
            // 
            // btnClear
            // 
            this.btnClear.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClear.Location = new System.Drawing.Point(592, 384);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(64, 23);
            this.btnClear.TabIndex = 14;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // tpTempReceiptAllocation
            // 
            this.tpTempReceiptAllocation.Controls.Add(this.dtDateIssued);
            this.tpTempReceiptAllocation.Controls.Add(this.numAllocLast);
            this.tpTempReceiptAllocation.Controls.Add(this.numAllocFirst);
            this.tpTempReceiptAllocation.Controls.Add(this.label17);
            this.tpTempReceiptAllocation.Controls.Add(this.label18);
            this.tpTempReceiptAllocation.Controls.Add(this.btnAllClear);
            this.tpTempReceiptAllocation.Controls.Add(this.btnAllocate);
            this.tpTempReceiptAllocation.Controls.Add(this.btnReallocate);
            this.tpTempReceiptAllocation.Controls.Add(this.label16);
            this.tpTempReceiptAllocation.Controls.Add(this.label15);
            this.tpTempReceiptAllocation.Controls.Add(this.drpAlloStaffMemeber);
            this.tpTempReceiptAllocation.Controls.Add(this.drpAlloStaffCat);
            this.tpTempReceiptAllocation.Controls.Add(this.txtAlloEmpeeNo);
            this.tpTempReceiptAllocation.Location = new System.Drawing.Point(0, 25);
            this.tpTempReceiptAllocation.Name = "tpTempReceiptAllocation";
            this.tpTempReceiptAllocation.Selected = false;
            this.tpTempReceiptAllocation.Size = new System.Drawing.Size(776, 431);
            this.tpTempReceiptAllocation.TabIndex = 4;
            this.tpTempReceiptAllocation.Title = "Temporary Receipt Allocation";
            this.tpTempReceiptAllocation.Visible = false;
            // 
            // dtDateIssued
            // 
            this.dtDateIssued.CustomFormat = "ddd dd MMM yyyy ";
            this.dtDateIssued.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDateIssued.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtDateIssued.Location = new System.Drawing.Point(192, 120);
            this.dtDateIssued.Name = "dtDateIssued";
            this.dtDateIssued.Size = new System.Drawing.Size(120, 23);
            this.dtDateIssued.TabIndex = 20;
            this.dtDateIssued.Value = new System.DateTime(2002, 5, 8, 0, 0, 0, 0);
            // 
            // numAllocLast
            // 
            this.numAllocLast.Location = new System.Drawing.Point(192, 88);
            this.numAllocLast.Maximum = new decimal(new int[] {
            -727379969,
            232,
            0,
            0});
            this.numAllocLast.Name = "numAllocLast";
            this.numAllocLast.Size = new System.Drawing.Size(120, 23);
            this.numAllocLast.TabIndex = 19;
            this.numAllocLast.Leave += new System.EventHandler(this.numAllocLast_Leave);
            // 
            // numAllocFirst
            // 
            this.numAllocFirst.Location = new System.Drawing.Point(192, 56);
            this.numAllocFirst.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numAllocFirst.Name = "numAllocFirst";
            this.numAllocFirst.Size = new System.Drawing.Size(120, 23);
            this.numAllocFirst.TabIndex = 18;
            this.numAllocFirst.Leave += new System.EventHandler(this.numAllocFirst_Leave);
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(64, 88);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(88, 23);
            this.label17.TabIndex = 17;
            this.label17.Text = "Last Receipt No:";
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(64, 56);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(88, 16);
            this.label18.TabIndex = 16;
            this.label18.Text = "First Receipt No:";
            // 
            // btnAllClear
            // 
            this.btnAllClear.Location = new System.Drawing.Point(360, 136);
            this.btnAllClear.Name = "btnAllClear";
            this.btnAllClear.Size = new System.Drawing.Size(75, 23);
            this.btnAllClear.TabIndex = 12;
            this.btnAllClear.Text = "Clear";
            this.btnAllClear.Click += new System.EventHandler(this.btnAllClear_Click);
            // 
            // btnAllocate
            // 
            this.btnAllocate.Location = new System.Drawing.Point(360, 56);
            this.btnAllocate.Name = "btnAllocate";
            this.btnAllocate.Size = new System.Drawing.Size(75, 23);
            this.btnAllocate.TabIndex = 11;
            this.btnAllocate.Text = "Allocate";
            this.btnAllocate.Click += new System.EventHandler(this.btnAllocate_Click);
            // 
            // btnReallocate
            // 
            this.btnReallocate.Location = new System.Drawing.Point(360, 96);
            this.btnReallocate.Name = "btnReallocate";
            this.btnReallocate.Size = new System.Drawing.Size(75, 23);
            this.btnReallocate.TabIndex = 10;
            this.btnReallocate.Text = "Reallocate";
            this.btnReallocate.Click += new System.EventHandler(this.btnReallocate_Click);
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(64, 152);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(100, 23);
            this.label16.TabIndex = 9;
            this.label16.Text = "Staff No";
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(64, 120);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(100, 23);
            this.label15.TabIndex = 8;
            this.label15.Text = "Data of Issue";
            // 
            // drpAlloStaffMemeber
            // 
            this.drpAlloStaffMemeber.Location = new System.Drawing.Point(64, 232);
            this.drpAlloStaffMemeber.Name = "drpAlloStaffMemeber";
            this.drpAlloStaffMemeber.Size = new System.Drawing.Size(232, 23);
            this.drpAlloStaffMemeber.TabIndex = 5;
            this.drpAlloStaffMemeber.SelectedIndexChanged += new System.EventHandler(this.drpAlloStaffMemeber_SelectedIndexChanged);
            // 
            // drpAlloStaffCat
            // 
            this.drpAlloStaffCat.Location = new System.Drawing.Point(64, 200);
            this.drpAlloStaffCat.Name = "drpAlloStaffCat";
            this.drpAlloStaffCat.Size = new System.Drawing.Size(232, 23);
            this.drpAlloStaffCat.TabIndex = 4;
            this.drpAlloStaffCat.SelectedIndexChanged += new System.EventHandler(this.drpAlloStaffCat_SelectedIndexChanged);
            // 
            // txtAlloEmpeeNo
            // 
            this.txtAlloEmpeeNo.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.txtAlloEmpeeNo.Location = new System.Drawing.Point(192, 152);
            this.txtAlloEmpeeNo.Name = "txtAlloEmpeeNo";
            this.txtAlloEmpeeNo.Size = new System.Drawing.Size(120, 21);
            this.txtAlloEmpeeNo.TabIndex = 3;
            this.txtAlloEmpeeNo.Text = "0";
            // 
            // lInvestigate
            // 
            this.lInvestigate.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.lInvestigate.Enabled = false;
            this.lInvestigate.Location = new System.Drawing.Point(128, 247);
            this.lInvestigate.Name = "lInvestigate";
            this.lInvestigate.Size = new System.Drawing.Size(16, 16);
            this.lInvestigate.TabIndex = 524;
            this.lInvestigate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lInvestigate.Visible = false;
            // 
            // lAllocate
            // 
            this.lAllocate.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.lAllocate.Enabled = false;
            this.lAllocate.Location = new System.Drawing.Point(128, 231);
            this.lAllocate.Name = "lAllocate";
            this.lAllocate.Size = new System.Drawing.Size(16, 16);
            this.lAllocate.TabIndex = 523;
            this.lAllocate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lAllocate.Visible = false;
            // 
            // TemporaryReceiptsDetails
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 466);
            this.Controls.Add(this.tcMain);
            this.Name = "TemporaryReceiptsDetails";
            this.Text = "Temporary Receipts";
            this.Load += new System.EventHandler(this.TemporaryReceiptsDetails_Load);
            this.tcMain.ResumeLayout(false);
            this.tpTempReceiptInvest.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numLastReceipt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEmpoyeeNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFirstReceipt)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgTempReceipts)).EndInit();
            this.tpTempReceiptAllocation.ResumeLayout(false);
            this.tpTempReceiptAllocation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAllocLast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAllocFirst)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void TemporaryReceiptsDetails_Load(object sender, System.EventArgs e)
		{
			if (!lInvestigate.Enabled)
				tcMain.TabPages.Remove(tpTempReceiptInvest);
			if (!lAllocate.Enabled)
				tcMain.TabPages.Remove(tpTempReceiptAllocation);
		}

		private void numEmpoyeeNo_Leave(object sender, System.EventArgs e)
		{

			numFirstReceipt.ReadOnly = true;
			numFirstReceipt.Value = 0;
			numLastReceipt.ReadOnly = true;
			numLastReceipt.Value = 0;
		}

		private void numFirstReceipt_Leave(object sender, System.EventArgs e)
		{
			numEmpoyeeNo.ReadOnly = true;
			numEmpoyeeNo.Value = 0;
			numLastReceipt.Value = numFirstReceipt.Value;
		}

		private void numLastReceipt_Leave(object sender, System.EventArgs e)
		{
			numEmpoyeeNo.ReadOnly = true;
			numEmpoyeeNo.Value = 0;
		}

		private void btnFind_Click(object sender, System.EventArgs e)
		{
			int first;
			int last;
			if (numFirstReceipt.ReadOnly)
			{
				first = (int)numFirstReceipt.Value;
				last = (int)numLastReceipt.Value;
				if (last ==  0 )			
				{
					last =  first;
				}
			}
			else 
			{
				first = 0;
				last = 0;
			}
			doGeneralTempReceiptSearch();
		}

		private void dgTempReceipts_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int index = dgTempReceipts.CurrentRowIndex;
			if(index>=0)
			{
				if ( ((DataView)dgTempReceipts.DataSource)[index][CN.DatePresented] !=DBNull.Value)
				{
					DateTime presented = Convert.ToDateTime(((DataView)dgTempReceipts.DataSource)[index][CN.DatePresented]);
					if (presented !=  Convert.ToDateTime("1900-01-01 12:00:00 AM"))
						txtDatePresented.Text = presented.ToString();
					else 
						txtDatePresented.Text = "";

				}
				else 
					txtDatePresented.Text = "";

				string acctNo =(string)((DataView)dgTempReceipts.DataSource)[index][CN.acctno];
				txtEmpeeNo.Text = Convert.ToInt32(((DataView)dgTempReceipts.DataSource)[index][CN.EmployeeNo]).ToString();
				txtEmpeeName.Text = (string)((DataView)dgTempReceipts.DataSource)[index][CN.EmployeeName];
			//	txtEmpeeType.Text = (string)((DataView)dgTempReceipts.DataSource)[index][CN.EmployeeType];
					 
				int test = Convert.ToInt32(((DataView)dgTempReceipts.DataSource)[index][CN.BranchNo]);
				txtBranchNo.Text = Convert.ToInt32(((DataView)dgTempReceipts.DataSource)[index][CN.BranchNo]).ToString();
				txtDateAllocated.Text = Convert.ToDateTime(((DataView)dgTempReceipts.DataSource)[index][CN.DateAlloc]).ToString();

				
				if (acctNo != "000000000000")
					txtAccountNumber.Text = acctNo;
				else 
					txtAccountNumber.Text = "";

				txtAmount.Text = Convert.ToDecimal(((DataView)dgTempReceipts.DataSource)[index][CN.Amount]).ToString(DecimalPlaces);;

				txtReceiptNo.Text = Convert.ToInt32(((DataView)dgTempReceipts.DataSource)[index][CN.ReceiptNo]).ToString();
			}
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			dgTempReceipts.DataSource = null;
			dgTempReceipts.TableStyles.Clear();
			numFirstReceipt.ReadOnly = false;
			numLastReceipt.ReadOnly = false;		
			numEmpoyeeNo.ReadOnly = false;
			numFirstReceipt.Value = 0;
			numLastReceipt.Value = 0;		
			numEmpoyeeNo.Value = 0;
			txtEmpeeNo.Text = "0";
			txtEmpeeName.Text = "";
            txtReceiptNo.Text = "0";
			txtBranchNo.Text = "0";
			txtDateAllocated.Text = "";
			txtDatePresented.Text = "";
			txtAccountNumber.Text = "000-0000-0000-0";
			txtAmount.Text =(0.00).ToString(DecimalPlaces);
			btnCancelReceipt.Enabled=false;
		}

		private void btnCancelReceipt_Click(object sender, System.EventArgs e)
		{
			int index = dgTempReceipts.CurrentRowIndex;
			if(index>=0)
			{
				int  ReceiptNo =  Convert.ToInt32(((DataView)dgTempReceipts.DataSource)[index][CN.ReceiptNo]);
				string accountNo  = (string)((DataView)dgTempReceipts.DataSource)[index][CN.acctno];
				switch (_screenType)
				{
					case  "BailiffReview":
					{
						
						if (accountNo != "000000000000")
						{
							ShowInfo("M_INVALIDVOIDTEMPRECEIPT", MessageBoxButtons.OK);
						}
						else
						{
							PaymentManager.CancelTempReceipt (ReceiptNo,out errorTxt);
							doBailiffTempReceiptSearch();
						}
						break;
					}
					case  "Enquiry":
					{
						if (accountNo != "000000000000")
						{
							ShowInfo("M_INVALIDVOIDTEMPRECEIPT", MessageBoxButtons.OK);
						}
						else 
						{
							PaymentManager.VoidTempReceipt (ReceiptNo,out errorTxt);
							doGeneralTempReceiptSearch();
						}

						break;
					}
				}

			}
		}

		private void drpAlloStaffCat_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				
				DataSet ds = null;
				if(staticLoaded && drpAlloStaffCat.SelectedIndex != 0)
				{
					string empTypeStr = (string)drpAlloStaffCat.SelectedItem;
					int index = empTypeStr.IndexOf(":");
					string empType = empTypeStr.Substring(0, index - 1);
					
					int len = empTypeStr.Length - 1;
					string empTitle = empTypeStr.Substring(index + 1, len - index);

					StringCollection staff = new StringCollection();

					ds = Login.GetSalesStaffByType(empType, 0, out errorTxt);
					if(errorTxt.Length>0)
						ShowError(errorTxt);
					else
					{
						foreach (DataTable dt in ds.Tables)
						{
							if(dt.TableName == TN.SalesStaff)
							{
								StringCollection staffMembers = new StringCollection();
								staffMembers.Add("Staff Members");

								foreach(DataRow row in dt.Rows)
								{
									string str = row.ItemArray[0].ToString()+" : "+(string)row.ItemArray[1];
									staffMembers.Add(str.ToUpper());
								}
								drpAlloStaffMemeber.DataSource = staffMembers;
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "drpEmpType_SelectedIndexChanged");
			}
			finally
			{
				StopWait();
			}	
		}

		private void drpAlloStaffMemeber_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string empNoStr = (string)drpAlloStaffMemeber.SelectedItem;
			if (empNoStr != "Staff Members")
			{
				int index = empNoStr.IndexOf(":");
				txtAlloEmpeeNo.Text = empNoStr.Substring(0, index - 1);
			}
		}

		private void numAllocFirst_Leave(object sender, System.EventArgs e)
		{
			int lastReceipt =  Convert.ToInt32(Country[CountryParameterNames.ReceiptsPerBook]) +  (int)(numAllocFirst.Value);
			numAllocLast.Value = lastReceipt;			
		}

		private void numAllocLast_Leave(object sender, System.EventArgs e)
		{

		}

		private void btnAllocate_Click(object sender, System.EventArgs e)
		{
			bool isOKToAllocate = true;
			int checkOption = 1; // Allocation check 
			int issuedCount = 0;
			PaymentManager.CheckReceiptNotIssued((int)numAllocFirst.Value, (int)numAllocLast.Value, checkOption, ref issuedCount, out errorTxt);
			if(errorTxt.Length>0)
				ShowError(errorTxt);
			else
			{
				if (issuedCount > 0)
				{
					ShowInfo("M_TEMPRECEIPTSALREADYAlLLOCATED", MessageBoxButtons.OK);
					isOKToAllocate = false;
				}
			}	
			if ((int)numAllocFirst.Value > (int)numAllocLast.Value)
			{
				isOKToAllocate = false;
			}
			if (txtAlloEmpeeNo.Text == "0")
			{
				isOKToAllocate = false;
			}
			if (dtDateIssued.Value > DateTime.Now)
			{
				isOKToAllocate = false;
				ShowInfo("M_ALLOCATIONINFUTURE", MessageBoxButtons.OK);
				dtDateIssued.Value = DateTime.Today;

			}
			if (isOKToAllocate)
			{
				PaymentManager.AllocateTempReceipt(Convert.ToInt32(txtAlloEmpeeNo.Text),Convert.ToInt32(Config.BranchCode),(int)numAllocFirst.Value, (int)numAllocLast.Value,dtDateIssued.Value, out errorTxt);
				if (errorTxt.Length == 0)
				{
					((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_TEMPRECEIPTSALAlLLOCATED", new Object[]{numAllocFirst.Value.ToString(),numAllocLast.Value.ToString() });
					
					btnAllClear_Click(sender, e);
				}

			}
		
		}

		private void btnReallocate_Click(object sender, System.EventArgs e)
		{
			bool isOKToAllocate = true;
			int checkOption = 2; // reallocation check 
			int issuedCount = 0;
			string  empeeNo = txtAlloEmpeeNo.Text;
			PaymentManager.CheckReceiptNotIssued((int)numAllocFirst.Value, (int)numAllocLast.Value, checkOption, ref issuedCount, out errorTxt);
			if(errorTxt.Length>0)
				ShowError(errorTxt);
			else
			{
				if (issuedCount > 0)
				{
					ShowInfo("M_REALLOCATINGISSUEDTEMPRECEIPTS",new Object[]{issuedCount.ToString()}, MessageBoxButtons.OK);
					isOKToAllocate = false;
				}
			}		
			if ((int)numAllocFirst.Value > (int)numAllocLast.Value)
			{
				isOKToAllocate = false;
			}
			if (txtAlloEmpeeNo.Text == "")
			{
				isOKToAllocate = false;
			}
			if (isOKToAllocate)
			{
				PaymentManager.ReallocateTempReceipt(Convert.ToInt32(txtAlloEmpeeNo.Text), Convert.ToInt32(Config.BranchCode), (int)numAllocFirst.Value, (int)numAllocLast.Value, out errorTxt);
				if (errorTxt.Length == 0)
				{
					((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_TEMPRECEIPTSALAlLLOCATED", new Object[]{numAllocFirst.Value.ToString(),numAllocLast.Value.ToString() });
					btnAllClear_Click(sender, e);
					txtAlloEmpeeNo.Text  = empeeNo;
				}
			}
		}


		private void btnAllClear_Click(object sender, System.EventArgs e)
		{
			int nextTempReceiptNo =0;

			PaymentManager.GetNextTemporaryReceptNo(ref nextTempReceiptNo, out errorTxt);
			numAllocFirst.Value =  nextTempReceiptNo + 1;
			numAllocLast.Value = Convert.ToInt32(Country[CountryParameterNames.ReceiptsPerBook]) +  (int)(numAllocFirst.Value);
			txtAlloEmpeeNo.Text = "0";
			dtDateIssued.Value = DateTime.Today;
			drpAlloStaffCat.SelectedIndex = (0);
			drpAlloStaffMemeber.SelectedIndex =  (0);
		}
		private void doGeneralTempReceiptSearch ()
		{
			numFirstReceipt.ReadOnly = true;
			numLastReceipt.ReadOnly = true;		
			numEmpoyeeNo.ReadOnly = true;
			dgTempReceipts.DataSource = null;
			dgTempReceipts.TableStyles.Clear();
			DataSet ds = PaymentManager.TemporaryReceiptEnquiry((int)numEmpoyeeNo.Value, (int)numFirstReceipt.Value, (int)numLastReceipt.Value, out errorTxt);
			DataView dvtempReceipt = ds.Tables[TN.TempReceipt].DefaultView;

            if (dvtempReceipt.Count > 0)
            { 
			dgTempReceipts.DataSource = dvtempReceipt;

			DataGridTableStyle tabStyle = new DataGridTableStyle();
			
            
            tabStyle.MappingName = dvtempReceipt.Table.TableName;
			dgTempReceipts.TableStyles.Add(tabStyle);

			tabStyle.GridColumnStyles[CN.OrigBR].Width = 0;

			tabStyle.GridColumnStyles[CN.ReceiptNo].Width = 100;
			tabStyle.GridColumnStyles[CN.ReceiptNo].HeaderText = GetResource("T_RECEIPTNO");

			tabStyle.GridColumnStyles[CN.BranchNo].Width = 80;
			tabStyle.GridColumnStyles[CN.BranchNo].HeaderText = GetResource("T_BRANCH");

			tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 80;
			tabStyle.GridColumnStyles[CN.EmployeeNo].HeaderText = GetResource("T_EMPEENO");

			tabStyle.GridColumnStyles[CN.DateAlloc].Width = 100;
			tabStyle.GridColumnStyles[CN.DateAlloc].HeaderText = GetResource("T_DATEALLOC");

			tabStyle.GridColumnStyles[CN.DateIssued].Width = 0;

			tabStyle.GridColumnStyles[CN.DatePresented].Width = 0;

			tabStyle.GridColumnStyles[CN.acctno].Width = 0;

			tabStyle.GridColumnStyles[CN.Amount].Width = 0;

			tabStyle.GridColumnStyles[CN.EmployeeName].Width = 0;
			
			btnCancelReceipt.Enabled=true;
			}
		}
		private void doBailiffTempReceiptSearch ()
		{
			numFirstReceipt.ReadOnly = true;
			numLastReceipt.ReadOnly = true;		
			numEmpoyeeNo.ReadOnly = true;
			dgTempReceipts.DataSource = null;
			dgTempReceipts.TableStyles.Clear();
			DataSet ds = PaymentManager.BailiffTemporaryReceiptEnquiry((int)numEmpoyeeNo.Value,  out errorTxt);
			DataView dvtempReceipt = ds.Tables[TN.TempReceipt].DefaultView;

			dgTempReceipts.DataSource = dvtempReceipt;

			DataGridTableStyle tabStyle = new DataGridTableStyle();
			tabStyle.MappingName = dvtempReceipt.Table.TableName;
			dgTempReceipts.TableStyles.Add(tabStyle);
			tabStyle.GridColumnStyles[CN.OrigBR].Width = 0;

			tabStyle.GridColumnStyles[CN.ReceiptNo].Width = 80;
			tabStyle.GridColumnStyles[CN.ReceiptNo].HeaderText = GetResource("T_RECEIPTNO");

			tabStyle.GridColumnStyles[CN.BranchNo].Width = 0;

			tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 0;

			tabStyle.GridColumnStyles[CN.DateAlloc].Width = 0;

			tabStyle.GridColumnStyles[CN.DateIssued].Width = 0;

			tabStyle.GridColumnStyles[CN.DatePresented].Width = 90;
			tabStyle.GridColumnStyles[CN.DatePresented].HeaderText = "Date Presented";

			tabStyle.GridColumnStyles[CN.acctno].Width = 80;
			tabStyle.GridColumnStyles[CN.acctno].HeaderText = GetResource("T_ACCOUNTNO");

			tabStyle.GridColumnStyles[CN.Amount].Width = 100;
			tabStyle.GridColumnStyles[CN.Amount].HeaderText = GetResource("T_AMOUNT");
			tabStyle.GridColumnStyles[CN.Amount].Alignment = HorizontalAlignment.Left;
			((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Amount]).Format = DecimalPlaces;

			tabStyle.GridColumnStyles[CN.EmployeeName].Width = 0;
			if (dvtempReceipt.Count >0)
			{
				btnCancelReceipt.Enabled=true;
			}
		}
	}
}
