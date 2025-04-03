using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Web.Services.Protocols;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading;
using System.Xml;
using STL.PL.WS1;
using STL.PL.WS2;
using STL.PL.WS3;
using STL.PL.WS4;
using STL.PL.WS5;
using STL.Common.Constants.Tags;
using STL.Common.Static;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.AccountTypes;
using Crownwood.Magic.Menus;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Elements;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ScreenModes;
using STL.Common.Constants.SanctionStages;



namespace STL.PL
{
	/// <summary>
	/// When a customer purchases goods on HP or Ready Finance they are credit
	/// sanctioned to determine their credit limit. Various sanction screens are
	/// used to enter customer details to determine credit worthiness. If
	/// the customer cannot supply all of the details and supporting documents or
	/// the application is referred to a credit officer, then the credit application
	/// is saved as an incomplete application. This screen provides a search
	/// mechanism for those applications so that they can be completed at a later date.
	/// When selecting an application from the list a set of four buttons is 
	/// displayed representing the four sections of an application: Stage One;
	/// Stage Two; Document Confirmation and Underwriter Referral. A red button
	/// indicates an incomplete section and a green button a complete section.
	/// Clicking on one of these buttons opens that section of the credit application.
	/// </summary>
	public class Incomplete : CommonForm
	{
	    private DataSet accounts;
		private DataView acctView;
		private new string Error = "";
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Data.DataSet HoldFlagSet;
		private System.Data.DataTable HoldFlags;
		private System.Data.DataColumn HoldFlag;
		private System.Data.DataColumn DateCleared;
		private System.Data.DataColumn ByUser;
		private System.Data.DataSet IncompleteSet;
		private System.Data.DataTable IncompleteAccounts;
		private System.Data.DataColumn AccountNo;
		private System.Data.DataColumn DateOpened;
		private System.Data.DataColumn SalesPerson;
		private System.Data.DataColumn CustName;
		private System.Data.DataColumn dataColumn1;
		private System.Windows.Forms.Button btnSearch;
		//private int searchClicked = 0;
		private System.Windows.Forms.ComboBox drpBranch;
		private System.Windows.Forms.DataGrid dgAccounts;
		private System.Windows.Forms.ComboBox drpHoldflags;
		private System.Windows.Forms.CheckBox viewTop;
		private STL.PL.AccountTextBox txtAccountNo;
		private STL.PL.SanctionStatus sanctionStatus1;
		private System.Windows.Forms.GroupBox groupBox2;
		//private Crownwood.Magic.Menus.MenuControl menuMain;
		private System.Data.DataSet dataSet1;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private System.Windows.Forms.CheckBox ChxOnly;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.ComponentModel.IContainer components;
		private string branch="";
		private bool _viewTop=false;
		private string accountNo="";
		private string hold="";
		private System.Windows.Forms.Label lblAll;
		private System.Windows.Forms.CheckBox ChxItems;
		private bool chxOnly=false;
		private System.Windows.Forms.Label allowCancel;
		private bool chxItems=false;
		private bool chxUnpaid=false;
        private bool pendingCashLoad = false;
        private bool cashLoanReferrals = false;
		private System.Windows.Forms.ComboBox drpReason;
		private System.Windows.Forms.CheckBox ChxUnpaid;
		private string custID = "";
		private System.Windows.Forms.Label lReason;
        private Label label4;
        private CheckBox chk_PendingClashLoan;
        private ToolTip toolTip2;
		string refCode = "";

		public Incomplete(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
		}

		public Incomplete(Form root, Form parent)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			HashMenus();
			ApplyRoleRestrictions();

			FormRoot = root;
			FormParent = parent;
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});

            InitialiseStaticData();
			//TranslateControls();
			toolTip1.SetToolTip(this.ChxOnly, GetResource("TT_IFCHECKED"));
		
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

		private void HashMenus()
		{
			dynamicMenus = new Hashtable();
			dynamicMenus[this.Name+":allowCancel"] = this.allowCancel; 
			dynamicMenus[this.Name+":drpReason"] = this.drpReason; 
		}

		/// <summary>
		/// load up branch numbers hopefully from cache
		/// </summary>
		private void InitialiseStaticData()	
		{		
			Function = "BStaticDataManager::GetDropDownData";
			try
			{
				Wait();
				//StringCollection branchNos = new StringCollection(); 	
				StringCollection ph1 = new StringCollection();
				//there is an invisible label on the form to allow for Thai translation
				ph1.Add("All -" + lblAll.Text);
				//branchNos.Add("Branches");

				StringCollection refCodes = new StringCollection();
				refCodes.Add("All");

				XmlUtilities xml = new XmlUtilities();
				XmlDocument dropDowns = new XmlDocument();
				dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
         		
				if(StaticData.Tables[TN.SanctionStages]==null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.SanctionStages,
						new string[] {"PH1", "L"}));

				if(StaticData.Tables[TN.BranchNumber]==null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.BranchNumber, null));	

				if(StaticData.Tables[TN.ReferralCodes]==null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.ReferralCodes, new string[] {"SN1","L"}));

				/*	JJ - no need for the string collection 
				foreach(DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
				{
					branchNos.Add(Convert.ToString(row["branchno"]));
				}
				*/
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
				foreach(DataRow row in ((DataTable)StaticData.Tables[TN.SanctionStages]).Rows)
				{
					string str = (string)row.ItemArray[0]+" - "+(string)row.ItemArray[1];
					ph1.Add(str.ToUpper());
				}

                ph1.Add("RL - REFERRAL LOAN QUAL");                                                    //IP - 24/10/11 - #3896 - CR1232
				foreach(DataRow row in ((DataTable)StaticData.Tables[TN.ReferralCodes]).Rows)
				{
					string str = (string)row.ItemArray[0]+" - "+(string)row.ItemArray[1];
					refCodes.Add(str.ToUpper());
				}  

				drpHoldflags.DataSource = ph1; 
				drpReason.DataSource = refCodes;

				StringCollection branchhome = new StringCollection();
				//there is an invisible label on the form to allow for Thai translation
				branchhome.Add(Config.BranchCode.ToString());
				
				drpBranch.DataSource = (DataTable)StaticData.Tables[TN.BranchNumber];
				drpBranch.DisplayMember = "branchno";
				

				int branch = Convert.ToInt32(Config.BranchCode);
				if (branch== (decimal)Country[CountryParameterNames.HOBranchNo])
				{
					drpBranch.DataSource = (DataTable)StaticData.Tables[TN.BranchNumber];
					drpBranch.Enabled= true;
				}
				else
				{
					drpBranch.DataSource=branchhome; 
					drpBranch.Enabled= false;
				}

				lReason.Visible = drpReason.Enabled;
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				
				StopWait();
			}	
		}
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Incomplete));
            this.dgAccounts = new System.Windows.Forms.DataGrid();
            this.IncompleteAccounts = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.AccountNo = new System.Data.DataColumn();
            this.DateOpened = new System.Data.DataColumn();
            this.SalesPerson = new System.Data.DataColumn();
            this.CustName = new System.Data.DataColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chk_PendingClashLoan = new System.Windows.Forms.CheckBox();
            this.ChxUnpaid = new System.Windows.Forms.CheckBox();
            this.lReason = new System.Windows.Forms.Label();
            this.drpReason = new System.Windows.Forms.ComboBox();
            this.ChxItems = new System.Windows.Forms.CheckBox();
            this.ChxOnly = new System.Windows.Forms.CheckBox();
            this.viewTop = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.drpHoldflags = new System.Windows.Forms.ComboBox();
            this.drpBranch = new System.Windows.Forms.ComboBox();
            this.txtAccountNo = new STL.PL.AccountTextBox();
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.HoldFlags = new System.Data.DataTable();
            this.HoldFlag = new System.Data.DataColumn();
            this.DateCleared = new System.Data.DataColumn();
            this.ByUser = new System.Data.DataColumn();
            this.HoldFlagSet = new System.Data.DataSet();
            this.IncompleteSet = new System.Data.DataSet();
            this.sanctionStatus1 = new STL.PL.SanctionStatus();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.allowCancel = new System.Windows.Forms.Label();
            this.lblAll = new System.Windows.Forms.Label();
            this.dataSet1 = new System.Data.DataSet();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.IncompleteAccounts)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HoldFlags)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HoldFlagSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.IncompleteSet)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            this.SuspendLayout();
            // 
            // dgAccounts
            // 
            this.dgAccounts.DataMember = "";
            this.dgAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAccounts.Location = new System.Drawing.Point(56, 56);
            this.dgAccounts.Name = "dgAccounts";
            this.dgAccounts.ReadOnly = true;
            this.dgAccounts.Size = new System.Drawing.Size(664, 272);
            this.dgAccounts.TabIndex = 3;
            this.dgAccounts.MouseHover += new System.EventHandler(this.dgAccounts_MouseHover);
            this.dgAccounts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgAccounts_MouseUp);
            // 
            // IncompleteAccounts
            // 
            this.IncompleteAccounts.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1});
            this.IncompleteAccounts.TableName = "IncompleteAccounts";
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "AccountNo";
            // 
            // AccountNo
            // 
            this.AccountNo.ColumnName = "AccountNo";
            this.AccountNo.MaxLength = 15;
            // 
            // DateOpened
            // 
            this.DateOpened.ColumnName = "DateOpened";
            this.DateOpened.MaxLength = 20;
            // 
            // SalesPerson
            // 
            this.SalesPerson.ColumnName = "SalesPerson";
            this.SalesPerson.MaxLength = 20;
            // 
            // CustName
            // 
            this.CustName.ColumnName = "Name";
            this.CustName.MaxLength = 20;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.chk_PendingClashLoan);
            this.groupBox1.Controls.Add(this.ChxUnpaid);
            this.groupBox1.Controls.Add(this.lReason);
            this.groupBox1.Controls.Add(this.drpReason);
            this.groupBox1.Controls.Add(this.ChxItems);
            this.groupBox1.Controls.Add(this.ChxOnly);
            this.groupBox1.Controls.Add(this.viewTop);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.drpHoldflags);
            this.groupBox1.Controls.Add(this.drpBranch);
            this.groupBox1.Controls.Add(this.txtAccountNo);
            this.groupBox1.Location = new System.Drawing.Point(8, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 112);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filter";
            // 
            // chk_PendingClashLoan
            // 
            this.chk_PendingClashLoan.Location = new System.Drawing.Point(230, 89);
            this.chk_PendingClashLoan.Name = "chk_PendingClashLoan";
            this.chk_PendingClashLoan.Size = new System.Drawing.Size(142, 16);
            this.chk_PendingClashLoan.TabIndex = 25;
            this.chk_PendingClashLoan.Text = "Cash Loans ";
            // 
            // ChxUnpaid
            // 
            this.ChxUnpaid.Checked = true;
            this.ChxUnpaid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChxUnpaid.Location = new System.Drawing.Point(230, 56);
            this.ChxUnpaid.Name = "ChxUnpaid";
            this.ChxUnpaid.Size = new System.Drawing.Size(152, 24);
            this.ChxUnpaid.TabIndex = 24;
            this.ChxUnpaid.Text = "Exclude Unpaid Accounts";
            // 
            // lReason
            // 
            this.lReason.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lReason.Location = new System.Drawing.Point(440, 56);
            this.lReason.Name = "lReason";
            this.lReason.Size = new System.Drawing.Size(96, 16);
            this.lReason.TabIndex = 23;
            this.lReason.Text = "Referral Reason";
            this.lReason.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpReason
            // 
            this.drpReason.Enabled = false;
            this.drpReason.Location = new System.Drawing.Point(544, 56);
            this.drpReason.Name = "drpReason";
            this.drpReason.Size = new System.Drawing.Size(192, 21);
            this.drpReason.TabIndex = 22;
            this.drpReason.Visible = false;
            // 
            // ChxItems
            // 
            this.ChxItems.Checked = true;
            this.ChxItems.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChxItems.Location = new System.Drawing.Point(16, 56);
            this.ChxItems.Name = "ChxItems";
            this.ChxItems.Size = new System.Drawing.Size(208, 24);
            this.ChxItems.TabIndex = 17;
            this.ChxItems.Text = "Exclude Accounts With No Products";
            // 
            // ChxOnly
            // 
            this.ChxOnly.Location = new System.Drawing.Point(16, 85);
            this.ChxOnly.Name = "ChxOnly";
            this.ChxOnly.Size = new System.Drawing.Size(80, 24);
            this.ChxOnly.TabIndex = 16;
            this.ChxOnly.Text = "Single Flag";
            // 
            // viewTop
            // 
            this.viewTop.Checked = true;
            this.viewTop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewTop.Location = new System.Drawing.Point(128, 90);
            this.viewTop.Name = "viewTop";
            this.viewTop.Size = new System.Drawing.Size(72, 16);
            this.viewTop.TabIndex = 15;
            this.viewTop.Text = "Top 200";
            this.viewTop.Visible = false;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(480, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 16);
            this.label3.TabIndex = 14;
            this.label3.Text = "Account no";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(184, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 16);
            this.label2.TabIndex = 13;
            this.label2.Text = "Application Status";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 16);
            this.label1.TabIndex = 12;
            this.label1.Text = "Branch";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSearch
            // 
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.Location = new System.Drawing.Point(712, 16);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(40, 32);
            this.btnSearch.TabIndex = 11;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // drpHoldflags
            // 
            this.drpHoldflags.Location = new System.Drawing.Point(296, 24);
            this.drpHoldflags.Name = "drpHoldflags";
            this.drpHoldflags.Size = new System.Drawing.Size(178, 21);
            this.drpHoldflags.TabIndex = 9;
            // 
            // drpBranch
            // 
            this.drpBranch.Location = new System.Drawing.Point(64, 24);
            this.drpBranch.Name = "drpBranch";
            this.drpBranch.Size = new System.Drawing.Size(80, 21);
            this.drpBranch.TabIndex = 8;
            this.drpBranch.Text = "comboBox1";
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.Location = new System.Drawing.Point(552, 24);
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.PreventPaste = false;
            this.txtAccountNo.Size = new System.Drawing.Size(88, 20);
            this.txtAccountNo.TabIndex = 9;
            this.txtAccountNo.Tag = "ACCNO";
            this.txtAccountNo.Text = "000-0000-0000-0";
            // 
            // toolBar1
            // 
            this.toolBar1.DropDownArrows = true;
            this.toolBar1.Location = new System.Drawing.Point(0, 0);
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ShowToolTips = true;
            this.toolBar1.Size = new System.Drawing.Size(768, 42);
            this.toolBar1.TabIndex = 10;
            // 
            // HoldFlags
            // 
            this.HoldFlags.Columns.AddRange(new System.Data.DataColumn[] {
            this.HoldFlag,
            this.DateCleared,
            this.ByUser});
            this.HoldFlags.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "HoldFlag"}, true)});
            this.HoldFlags.MinimumCapacity = 10;
            this.HoldFlags.PrimaryKey = new System.Data.DataColumn[] {
        this.HoldFlag};
            this.HoldFlags.TableName = "HoldFlags";
            // 
            // HoldFlag
            // 
            this.HoldFlag.AllowDBNull = false;
            this.HoldFlag.ColumnName = "HoldFlag";
            this.HoldFlag.MaxLength = 4;
            // 
            // DateCleared
            // 
            this.DateCleared.ColumnName = "DateCleared";
            this.DateCleared.MaxLength = 15;
            // 
            // ByUser
            // 
            this.ByUser.ColumnName = "ByUser";
            this.ByUser.MaxLength = 10;
            // 
            // HoldFlagSet
            // 
            this.HoldFlagSet.DataSetName = "NewDataSet";
            this.HoldFlagSet.Locale = new System.Globalization.CultureInfo("en-GB");
            // 
            // IncompleteSet
            // 
            this.IncompleteSet.DataSetName = "NewDataSet";
            this.IncompleteSet.Locale = new System.Globalization.CultureInfo("en-GB");
            // 
            // sanctionStatus1
            // 
            this.sanctionStatus1.AccountNo = "";
            this.sanctionStatus1.AccountType = "";
            this.sanctionStatus1.allowConversionToHP = false;
            this.sanctionStatus1.CurrentStatus = "";
            this.sanctionStatus1.CustomerID = "";
            this.sanctionStatus1.CustomerScreen = null;
            this.sanctionStatus1.DateProp = new System.DateTime(((long)(0)));
            this.sanctionStatus1.Enabled = false;
            this.sanctionStatus1.HoldProp = false;
            this.sanctionStatus1.Location = new System.Drawing.Point(632, 16);
            this.sanctionStatus1.Name = "sanctionStatus1";
            this.sanctionStatus1.ScreenMode = "Edit";
            this.sanctionStatus1.Settled = false;
            this.sanctionStatus1.Size = new System.Drawing.Size(128, 24);
            this.sanctionStatus1.TabIndex = 9;
            this.sanctionStatus1.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.sanctionStatus1);
            this.groupBox2.Controls.Add(this.dgAccounts);
            this.groupBox2.Controls.Add(this.allowCancel);
            this.groupBox2.Controls.Add(this.lblAll);
            this.groupBox2.Location = new System.Drawing.Point(8, 120);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(776, 352);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Proposals";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(480, 335);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(240, 13);
            this.label4.TabIndex = 39;
            this.label4.Text = "Note: maximum of 500 results returned per query. ";
            // 
            // allowCancel
            // 
            this.allowCancel.Enabled = false;
            this.allowCancel.Location = new System.Drawing.Point(106, 204);
            this.allowCancel.Name = "allowCancel";
            this.allowCancel.Size = new System.Drawing.Size(38, 16);
            this.allowCancel.TabIndex = 38;
            this.allowCancel.Text = "label1";
            this.allowCancel.Visible = false;
            // 
            // lblAll
            // 
            this.lblAll.Location = new System.Drawing.Point(176, 123);
            this.lblAll.Name = "lblAll";
            this.lblAll.Size = new System.Drawing.Size(104, 24);
            this.lblAll.TabIndex = 10;
            this.lblAll.Text = "All";
            this.lblAll.Visible = false;
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            this.dataSet1.Locale = new System.Globalization.CultureInfo("en-GB");
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
            // Incomplete
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Incomplete";
            this.Text = "Incomplete Credit Applications";
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.IncompleteAccounts)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HoldFlags)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HoldFlagSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.IncompleteSet)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion
		
		
		private void SearchThread()
		{
			try
			{
				Wait();
				Function = "SearchThread";
	
				accounts = AccountManager.IncompleteCredits(branch, hold, _viewTop, accountNo,
                    chxOnly, chxItems, chxUnpaid,cashLoanReferrals, pendingCashLoad, refCode, out Error);

				if(Error.Length>0)
					ShowError(Error);						
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of SearchThread";
			}
		}
		
		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			Function = "btnSearch_Click";   
			try
			{
				Wait();

				accounts = null;
				sanctionStatus1.SetInvisible();
				((MainForm)this.FormRoot).statusBar1.Text = "Searching for accounts";
				branch = drpBranch.Text;
				//_viewTop = viewTop.Checked;
				accountNo = txtAccountNo.Text.Replace("-","");
				chxOnly = ChxOnly.Checked;
				chxItems = ChxItems.Checked;
				chxUnpaid = ChxUnpaid.Checked;
                //cashLoanReferrals = chk_CashLoanReferral.Checked;
                pendingCashLoad = chk_PendingClashLoan.Checked;
				string selected=(string)drpHoldflags.SelectedItem;		
				selected = selected.Substring(0, selected.IndexOf("-") -1);
				switch (selected)
				{
					case "All": hold = "%"; break;
					case "S1": hold =SS.S1; break;
					case "S2": hold =SS.S2; break;
					case "DC": hold =SS.DC; break;
					case "R": hold = SS.R; break;
					case "AD": hold = SS.AD; break;
                    case "RL": hold = "RL"; break;                                  //IP - 24/10/11

				}

				string selectedCode = (string)drpReason.SelectedItem;
				
				if(drpReason.SelectedIndex == 0)
					refCode = "%";
				else
					refCode = selectedCode.Substring(0,2);


				Thread data = new Thread(new ThreadStart(SearchThread));
				data.Start();
				data.Join();  
				
				if(accounts!=null)
				{
					foreach (DataTable dt in accounts.Tables)
					{
						if(dt.TableName == TN.IncompleteCredits)
						{
							acctView = new DataView(dt);
							if(acctView.Count>0)
							{
								((MainForm)this.FormRoot).statusBar1.Text = acctView.Count.ToString()+ " rows returned";
								dgAccounts.DataSource = acctView;
								if(dgAccounts.TableStyles.Count==0)
								{
									DataGridTableStyle tabStyle = new DataGridTableStyle();
									tabStyle.MappingName = dt.TableName;
									dgAccounts.TableStyles.Add(tabStyle);
   						
									// Set the table style according to the user's preference
									tabStyle.GridColumnStyles["Customer ID"].Width = 90;
									tabStyle.GridColumnStyles["Customer ID"].HeaderText = GetResource("T_CUSTID");
									tabStyle.GridColumnStyles["Customer ID"].Alignment = HorizontalAlignment.Left;
									tabStyle.GridColumnStyles["Account Number"].Width = 90;
									tabStyle.GridColumnStyles["Account Number"].HeaderText = GetResource("T_ACCTNO");
									tabStyle.GridColumnStyles["Account Number"].Alignment = HorizontalAlignment.Left;
									tabStyle.GridColumnStyles["Title"].Width = 30;
									tabStyle.GridColumnStyles["Title"].HeaderText = GetResource("T_TITLE");
									tabStyle.GridColumnStyles["Title"].Alignment = HorizontalAlignment.Left;
									tabStyle.GridColumnStyles["First Name"].Width = 60;
									tabStyle.GridColumnStyles["First Name"].HeaderText = GetResource("T_FIRSTNAME");
									tabStyle.GridColumnStyles["First Name"].Alignment = HorizontalAlignment.Left;
									tabStyle.GridColumnStyles["Last Name"].Width = 118;
									tabStyle.GridColumnStyles["Last Name"].HeaderText = GetResource("T_LASTNAME");
									tabStyle.GridColumnStyles["Last Name"].Alignment = HorizontalAlignment.Left;
									tabStyle.GridColumnStyles["Type"].Width = 30;
									tabStyle.GridColumnStyles["Type"].HeaderText = GetResource("T_TYPE");
									tabStyle.GridColumnStyles["Type"].Alignment = HorizontalAlignment.Left;	
									tabStyle.GridColumnStyles["Salesperson No"].Width = 40;
									tabStyle.GridColumnStyles["Salesperson No"].HeaderText = GetResource("T_EMPEENO");
									tabStyle.GridColumnStyles["Salesperson No"].Alignment = HorizontalAlignment.Left;
									tabStyle.GridColumnStyles["Salesperson Name"].Width = 150;
									tabStyle.GridColumnStyles["Salesperson Name"].HeaderText = GetResource("T_EMPEENAME");
									tabStyle.GridColumnStyles["Salesperson Name"].Alignment = HorizontalAlignment.Left;
									tabStyle.GridColumnStyles["Date Proposal"].Width= 0;	
									/* CR 484 */
									tabStyle.GridColumnStyles[CN.DateAcctOpen].HeaderText = GetResource("T_DATEOPENED");
									tabStyle.GridColumnStyles[CN.DateAcctOpen].Width= 100;	
									/* end CR */
									
									if(drpReason.Enabled)
									{
										tabStyle.GridColumnStyles["Description"].Width = 150;
										tabStyle.GridColumnStyles["Description"].HeaderText = GetResource("T_REFERRAL");
										tabStyle.GridColumnStyles["Description"].Alignment = HorizontalAlignment.Left;
									}
									else
										tabStyle.GridColumnStyles["Description"].Width= 0;	
									
									tabStyle.GridColumnStyles["Date Changed"].Width = 100;
									tabStyle.GridColumnStyles["Date Changed"].HeaderText = GetResource("T_DATECHANGED");
									tabStyle.GridColumnStyles["Date Changed"].Alignment = HorizontalAlignment.Left;
   														
								}
							}
							else
							{
								dgAccounts.DataSource = null;
								((MainForm)this.FormRoot).statusBar1.Text = "No accounts returned";
							}
						}
					}
				}
   		
				Function = "End of btnSearch_Click";
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}	
			finally
			{
				StopWait();
			}
   	
		}

        //private void dgAccounts_Enter(object sender, System.EventArgs e)
        //{
        //    Function = "dgIncompleteAccounts_Click";
        //    try
        //    {
        //        if(dgAccounts.DataSource!=null)
        //        {					
        //            int index = dgAccounts.CurrentRowIndex;
        //            dgAccounts.Select(index);
        //            sanctionStatus1.Enabled= true;
        //            sanctionStatus1.Visible = true;
        //            sanctionStatus1.Common.FormRoot = this.FormRoot;			
        //            sanctionStatus1.AccountNo = (string)dgAccounts[dgAccounts.CurrentRowIndex, 1];
        //            sanctionStatus1.CustomerID= (string)dgAccounts[dgAccounts.CurrentRowIndex,0];
        //            sanctionStatus1.DateProp= (System.DateTime)dgAccounts[dgAccounts.CurrentRowIndex,8];
        //            sanctionStatus1.AccountType =(string)dgAccounts[dgAccounts.CurrentRowIndex, 5];
        //            sanctionStatus1.ScreenMode=SM.Edit;
        //            sanctionStatus1.Load();

        //        }
        //        Function = "End of dgIncompleteAccounts_Click";
        //    }
        //    catch(Exception ex)
        //    {
        //        Catch(ex, Function);
        //    }
        //}

		private void menuExit_Click(object sender, System.EventArgs e)
		{
			this.CloseTab();
		}

		private void dgAccounts_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Function = "dgAccounts_MouseUp";
			try
			{
				if (e.Button == MouseButtons.Right)
				{
					if(dgAccounts.CurrentRowIndex >= 0)
					{
						DataGrid ctl = (DataGrid)sender;							

						MenuCommand m1 = new MenuCommand(GetResource("P_CANCEL_ACCOUNT"));
						m1.Click += new System.EventHandler(this.OnCancelAccount);

						if(allowCancel.Enabled)
						{
							PopupMenu popup = new PopupMenu(); 
							popup.MenuCommands.AddRange(new MenuCommand[] {m1});
							MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
						}
					}
				}
				else
				{
					if(dgAccounts.DataSource!=null)
					{					
						int index = dgAccounts.CurrentRowIndex;
						if(index>=0)
						{
							dgAccounts.Select(index);
							sanctionStatus1.Enabled= true;
							sanctionStatus1.Visible = true;
							sanctionStatus1.Common.FormRoot = this.FormRoot;			
							sanctionStatus1.AccountNo = (string)dgAccounts[dgAccounts.CurrentRowIndex, 1];
							sanctionStatus1.CustomerID= (string)dgAccounts[dgAccounts.CurrentRowIndex,0];
							sanctionStatus1.DateProp= (System.DateTime)dgAccounts[dgAccounts.CurrentRowIndex,8];
							sanctionStatus1.AccountType =(string)dgAccounts[dgAccounts.CurrentRowIndex, 5];
							sanctionStatus1.ScreenMode=SM.Edit;
							sanctionStatus1.Load();
						}
					}
				}
				
				Function = "End of dgAccounts_MouseUp";
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}		
		}

		private void OnCancelAccount(object sender, System.EventArgs e)
		{
			//decimal balance = 0;
			//string cancellationCode = "B";

			try
			{
				int index = dgAccounts.CurrentRowIndex;

				accountNo = (string)dgAccounts[index, 1];
				custID = (string)dgAccounts[index,0];

				CancelAccount can = new CancelAccount(accountNo, FormRoot, this);
				can.ShowDialog();
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

        //IP - 24/02/12 - #9598 - UAT 87
        private void dgAccounts_MouseHover(object sender, EventArgs e)
        {
            if (dgAccounts.CurrentRowIndex >= 0)
            {
                var refReasons = Convert.ToString(((DataView)dgAccounts.DataSource)[dgAccounts.CurrentRowIndex][CN.Description]);

                toolTip2.SetToolTip(dgAccounts, refReasons);    
            }
        }

	}
}
