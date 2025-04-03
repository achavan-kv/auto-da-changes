using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Data;
using System.Web.Services.Protocols;
using STL.PL.WS9;
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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace STL.PL
{
	/// <summary>
	/// Summary description for SummaryUpdateControlTotals.
	/// </summary>
	public class SummaryUpdateControlTotals : CommonForm
	{
		private Crownwood.Magic.Controls.TabControl tcMain;
		private Crownwood.Magic.Controls.TabPage tpFinaStat;
		private Crownwood.Magic.Controls.TabPage tpOpenAccounts;
		private Crownwood.Magic.Controls.TabPage tpInterest;
		private System.Windows.Forms.DataGrid dgFSummary;
		/// <summary>
		private int _batchNo;
		private DateTime _runStart;
		private DateTime _runEnd;
		private int _openAcc;
		private decimal _openBalance;
		private int _closeAcc;
		private decimal _closeBalance;
		private int _branchNo;
		private string errorTxt;
		private System.Windows.Forms.DataGrid dgISummary;
		private System.Windows.Forms.DataGrid dgOSummary;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.Label label29;
		private System.Windows.Forms.Label label30;
		private System.Windows.Forms.Label label31;
		private System.Windows.Forms.Label label32;
		private System.Windows.Forms.DateTimePicker dtStart;
		private System.Windows.Forms.DateTimePicker dtEnd;
		private System.Windows.Forms.TextBox txtOpenAc;
		private System.Windows.Forms.TextBox txtCloseBal;
		private System.Windows.Forms.TextBox txtOpenBal;
		private System.Windows.Forms.TextBox txtCloseAc;
		public System.Windows.Forms.Button btnExcel;
		private System.Windows.Forms.TextBox txtBranchNo;
		private System.Windows.Forms.TextBox txtBatchNo;
		private System.Windows.Forms.Button btnInterfaced;
        private System.Windows.Forms.Button btnDelInterface;
		private Crownwood.Magic.Controls.TabPage tpInterestByAccount;
		private System.Windows.Forms.DataGrid dgByAccountSettled;
		private System.Windows.Forms.DataGrid dgByAccountUnsettled;
		/// </summary>
        private System.ComponentModel.Container components = null;

        private bool _useLiveDatabase;

		public SummaryUpdateControlTotals()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}
		public SummaryUpdateControlTotals(Form root, Form parent, int batchNo,
			DateTime runstart , DateTime runend , 
			int openac, decimal openbal, int closeac, 
			decimal  closebal, int branchno, bool useLiveDatabase)
		{
            _useLiveDatabase = useLiveDatabase;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			FormRoot = root;
			FormParent = parent;
			_batchNo = batchNo;
			_runStart = runstart;
			_runEnd = runend;
			_openAcc = openac;
			_openBalance = openbal;
			_closeAcc = closeac;
			_closeBalance = closebal;
			_branchNo = branchno;
			txtBatchNo.Text = _batchNo.ToString();
			dtStart.Value = _runStart;
			dtEnd.Value = _runEnd;
			if (_branchNo == 0)
			{
				txtBranchNo.Text = "ALL";
			}
			else 
			{
				txtBranchNo.Text = _branchNo.ToString();
			}
			txtOpenAc.Text = _openAcc.ToString("N0");
			txtOpenBal.Text = _openBalance.ToString(DecimalPlaces);
			txtCloseAc.Text = _closeAcc.ToString("N0");
			txtCloseBal.Text = _closeBalance.ToString(DecimalPlaces);

			populateSummaryTable("FINANCE");
			populateSummaryTable("OPEN");
			populateSummaryTable("INTEREST");
		}

		private void populateSummaryTable(string type )
		{
			try
			{
				Wait();

				//bool isData = false;
				DataGridTableStyle tabStyle = new DataGridTableStyle();
				DataGridTableStyle tabStyle2 = new DataGridTableStyle();
				DataView summary = null;
				switch (type)
				{
					case "FINANCE" :
					{
						dgFSummary.DataSource = null;
						dgFSummary.TableStyles.Clear();
						DataSet ds = EodManager.GetSummaryControlTotals(_batchNo, _branchNo, "FINANCE", _useLiveDatabase,  out errorTxt);
						summary = ds.Tables[TN.SummaryControl].DefaultView;

						dgFSummary.DataSource = summary;
						if (summary.Count > 0)

							tabStyle.MappingName = summary.Table.TableName;
						dgFSummary.TableStyles.Add(tabStyle);
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.HPValue]).Format = "N2";
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.CashValue]).Format = "N2";
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.SpecialValue]).Format = "N2";
                        ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.StoreCardValue]).Format = "N2";                    //IP - 17/02/12 - #9423 - CR8262
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Total]).Format = "N2";
						tabStyle.GridColumnStyles[CN.Source].Width = 65;
						break;
					}
					case "OPEN" :
					{
						dgOSummary.DataSource = null;
						dgOSummary.TableStyles.Clear();
						DataSet ds = EodManager.GetSummaryControlTotals(_batchNo, _branchNo, "OPEN",_useLiveDatabase,  out errorTxt);
						summary = ds.Tables[TN.SummaryControl].DefaultView;
					
						dgOSummary.DataSource = summary;

						tabStyle.MappingName = summary.Table.TableName;
						dgOSummary.TableStyles.Add(tabStyle);
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.HPValue]).Format = "N0";
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.CashValue]).Format = "N0";
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.SpecialValue]).Format = "N0";
                        ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.StoreCardValue]).Format = "N0";                     //IP - 17/02/12 - #9423 - CR8262
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Total]).Format = "N0";
						tabStyle.GridColumnStyles[CN.Source].Width = 0;


						break;
					}
					case "INTEREST" :
					{
						dgISummary.DataSource = null;
						dgISummary.TableStyles.Clear();
						DataSet ds = EodManager.GetSummaryControlTotals(_batchNo, _branchNo, "INTEREST",_useLiveDatabase,  out errorTxt);
						summary = ds.Tables[TN.SummaryControl].DefaultView;

						dgISummary.DataSource = summary;
						tabStyle.MappingName = summary.Table.TableName;
						dgISummary.TableStyles.Add(tabStyle);
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.HPValue]).Format = "N2";
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.CashValue]).Format = "N2";
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.SpecialValue]).Format = "N2";
                        ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.StoreCardValue]).Format = "N2";                    //IP - 17/02/12 - #9423 - CR8262
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Total]).Format = "N2";
						tabStyle.GridColumnStyles[CN.Source].Width = 100;
						break;
					}
					case "INTERESTBYACCOUNT" :
					{
						dgByAccountUnsettled.DataSource = null;
						dgByAccountSettled.DataSource = null;
						dgByAccountUnsettled.TableStyles.Clear();
						dgByAccountSettled.TableStyles.Clear();

						DataSet ds = EodManager.GetSummaryControlTotals(_batchNo, _branchNo, "INTERESTBYACCOUNT",_useLiveDatabase,  out errorTxt);
						DataView unsettledView = ds.Tables[TN.InterestUnsettled].DefaultView;
						DataView settledView = ds.Tables[TN.InterestSettled].DefaultView;

						dgByAccountUnsettled.DataSource = unsettledView;
						tabStyle2.MappingName = unsettledView.Table.TableName;
						dgByAccountUnsettled.TableStyles.Add(tabStyle2);
						//((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.AcctNo]).Format = "N0";
						((DataGridTextBoxColumn)tabStyle2.GridColumnStyles[CN.Interest]).Format = "N2";
						tabStyle2.GridColumnStyles[CN.AcctNo].Width = 120;
						tabStyle2.GridColumnStyles[CN.Interest].Width = 120;

						decimal interesttotal=0; string intereststringtotal ="";
						// get totals and then display them at top of tables
						foreach(DataRowView row in (DataView)dgByAccountUnsettled.DataSource)
						{
							interesttotal = interesttotal + Convert.ToDecimal(row[CN.Interest]);
						}
						intereststringtotal=String.Format("{0:c}", interesttotal);
						dgByAccountUnsettled.CaptionText = dgByAccountUnsettled.CaptionText + " Total: " + intereststringtotal;
						interesttotal = 0;

						dgByAccountSettled.DataSource = settledView;
						
						
						foreach(DataRowView row in (DataView)dgByAccountSettled.DataSource)
						{
							interesttotal = interesttotal + Convert.ToDecimal(row[CN.Interest]);
						}
						intereststringtotal=String.Format("{0:c}", interesttotal);
						dgByAccountSettled.CaptionText = dgByAccountSettled.CaptionText + " Total: " + intereststringtotal;
						
						tabStyle.MappingName = settledView.Table.TableName;
						dgByAccountSettled.TableStyles.Add(tabStyle);
						//((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.AcctNo]).Format = "N0";
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Interest]).Format = "N2";
						tabStyle.GridColumnStyles[CN.AcctNo].Width = 120;
						tabStyle.GridColumnStyles[CN.Interest].Width = 120;
						break;
					}
					default:
						break;
				}
	
				if (type != "INTERESTBYACCOUNT")
				{
					tabStyle.GridColumnStyles[CN.Description].Width = 145;
					tabStyle.GridColumnStyles[CN.Description].HeaderText = GetResource("T_DESCRIPTION");

					tabStyle.GridColumnStyles[CN.HPValue].Width = 100;
					tabStyle.GridColumnStyles[CN.HPValue].HeaderText = GetResource("T_SMRYHPVALUE");
					tabStyle.GridColumnStyles[CN.HPValue].Alignment = HorizontalAlignment.Left;
			
					tabStyle.GridColumnStyles[CN.CashValue].Width = 100;
					tabStyle.GridColumnStyles[CN.CashValue].HeaderText = GetResource("T_SMRYCASHVALUE");
					tabStyle.GridColumnStyles[CN.CashValue].Alignment = HorizontalAlignment.Left;
			
					tabStyle.GridColumnStyles[CN.SpecialValue].Width = 100;
					tabStyle.GridColumnStyles[CN.SpecialValue].HeaderText = GetResource("T_SMRYSPECIALVALUE");
					tabStyle.GridColumnStyles[CN.SpecialValue].Alignment = HorizontalAlignment.Left;

                    //IP - 17/02/12 - #9423 - CR8262
                    tabStyle.GridColumnStyles[CN.StoreCardValue].Width = 100;
					tabStyle.GridColumnStyles[CN.StoreCardValue].HeaderText = GetResource("T_SMRYSTORECARDVALUE");
					tabStyle.GridColumnStyles[CN.StoreCardValue].Alignment = HorizontalAlignment.Left;

					tabStyle.GridColumnStyles[CN.Total].Width = 100;
					tabStyle.GridColumnStyles[CN.Total].HeaderText = GetResource("T_TOTALAMT");
					tabStyle.GridColumnStyles[CN.Total].Alignment = HorizontalAlignment.Left;
		
					tabStyle.GridColumnStyles[CN.Source].HeaderText = GetResource("T_SMRYSOURCE");
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SummaryUpdateControlTotals));
            this.tcMain = new Crownwood.Magic.Controls.TabControl();
            this.tpFinaStat = new Crownwood.Magic.Controls.TabPage();
            this.btnDelInterface = new System.Windows.Forms.Button();
            this.btnInterfaced = new System.Windows.Forms.Button();
            this.dgFSummary = new System.Windows.Forms.DataGrid();
            this.tpOpenAccounts = new Crownwood.Magic.Controls.TabPage();
            this.dgOSummary = new System.Windows.Forms.DataGrid();
            this.tpInterest = new Crownwood.Magic.Controls.TabPage();
            this.dgISummary = new System.Windows.Forms.DataGrid();
            this.tpInterestByAccount = new Crownwood.Magic.Controls.TabPage();
            this.dgByAccountUnsettled = new System.Windows.Forms.DataGrid();
            this.dgByAccountSettled = new System.Windows.Forms.DataGrid();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnExcel = new System.Windows.Forms.Button();
            this.label25 = new System.Windows.Forms.Label();
            this.txtBranchNo = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.dtStart = new System.Windows.Forms.DateTimePicker();
            this.dtEnd = new System.Windows.Forms.DateTimePicker();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.txtBatchNo = new System.Windows.Forms.TextBox();
            this.txtOpenAc = new System.Windows.Forms.TextBox();
            this.txtCloseBal = new System.Windows.Forms.TextBox();
            this.txtOpenBal = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.txtCloseAc = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.tcMain.SuspendLayout();
            this.tpFinaStat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgFSummary)).BeginInit();
            this.tpOpenAccounts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgOSummary)).BeginInit();
            this.tpInterest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgISummary)).BeginInit();
            this.tpInterestByAccount.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgByAccountUnsettled)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgByAccountSettled)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcMain
            // 
            this.tcMain.IDEPixelArea = true;
            this.tcMain.Location = new System.Drawing.Point(8, 136);
            this.tcMain.Name = "tcMain";
            this.tcMain.PositionTop = true;
            this.tcMain.SelectedIndex = 0;
            this.tcMain.SelectedTab = this.tpFinaStat;
            this.tcMain.Size = new System.Drawing.Size(776, 328);
            this.tcMain.TabIndex = 15;
            this.tcMain.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpFinaStat,
            this.tpOpenAccounts,
            this.tpInterest,
            this.tpInterestByAccount});
            // 
            // tpFinaStat
            // 
            this.tpFinaStat.Controls.Add(this.btnDelInterface);
            this.tpFinaStat.Controls.Add(this.btnInterfaced);
            this.tpFinaStat.Controls.Add(this.dgFSummary);
            this.tpFinaStat.Location = new System.Drawing.Point(0, 25);
            this.tpFinaStat.Name = "tpFinaStat";
            this.tpFinaStat.Size = new System.Drawing.Size(776, 303);
            this.tpFinaStat.TabIndex = 3;
            this.tpFinaStat.Title = "Financial Statement";
            // 
            // btnDelInterface
            // 
            this.btnDelInterface.Location = new System.Drawing.Point(689, 166);
            this.btnDelInterface.Name = "btnDelInterface";
            this.btnDelInterface.Size = new System.Drawing.Size(72, 32);
            this.btnDelInterface.TabIndex = 29;
            this.btnDelInterface.Text = "Delivery Interface";
            this.btnDelInterface.Click += new System.EventHandler(this.btnDelInterface_Click);
            // 
            // btnInterfaced
            // 
            this.btnInterfaced.Location = new System.Drawing.Point(689, 118);
            this.btnInterfaced.Name = "btnInterfaced";
            this.btnInterfaced.Size = new System.Drawing.Size(72, 32);
            this.btnInterfaced.TabIndex = 28;
            this.btnInterfaced.Text = "Financial Interface";
            this.btnInterfaced.Click += new System.EventHandler(this.btnInterfaced_Click);
            // 
            // dgFSummary
            // 
            this.dgFSummary.CaptionText = "Summary Update Control Report - Financial Statement";
            this.dgFSummary.DataMember = "";
            this.dgFSummary.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgFSummary.Location = new System.Drawing.Point(24, 16);
            this.dgFSummary.Name = "dgFSummary";
            this.dgFSummary.ReadOnly = true;
            this.dgFSummary.Size = new System.Drawing.Size(646, 280);
            this.dgFSummary.TabIndex = 27;
            // 
            // tpOpenAccounts
            // 
            this.tpOpenAccounts.Controls.Add(this.dgOSummary);
            this.tpOpenAccounts.Location = new System.Drawing.Point(0, 25);
            this.tpOpenAccounts.Name = "tpOpenAccounts";
            this.tpOpenAccounts.Selected = false;
            this.tpOpenAccounts.Size = new System.Drawing.Size(776, 303);
            this.tpOpenAccounts.TabIndex = 4;
            this.tpOpenAccounts.Title = "Open Accounts";
            this.tpOpenAccounts.Visible = false;
            // 
            // dgOSummary
            // 
            this.dgOSummary.CaptionText = "Summary Update Control Report - Open Accounts";
            this.dgOSummary.DataMember = "";
            this.dgOSummary.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgOSummary.Location = new System.Drawing.Point(64, 16);
            this.dgOSummary.Name = "dgOSummary";
            this.dgOSummary.ReadOnly = true;
            this.dgOSummary.Size = new System.Drawing.Size(646, 272);
            this.dgOSummary.TabIndex = 30;
            // 
            // tpInterest
            // 
            this.tpInterest.Controls.Add(this.dgISummary);
            this.tpInterest.Location = new System.Drawing.Point(0, 25);
            this.tpInterest.Name = "tpInterest";
            this.tpInterest.Selected = false;
            this.tpInterest.Size = new System.Drawing.Size(776, 303);
            this.tpInterest.TabIndex = 5;
            this.tpInterest.Title = "Interest";
            // 
            // dgISummary
            // 
            this.dgISummary.CaptionText = "Summary Update Control Report- Interest";
            this.dgISummary.DataMember = "";
            this.dgISummary.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgISummary.Location = new System.Drawing.Point(64, 16);
            this.dgISummary.Name = "dgISummary";
            this.dgISummary.ReadOnly = true;
            this.dgISummary.Size = new System.Drawing.Size(646, 272);
            this.dgISummary.TabIndex = 30;
            // 
            // tpInterestByAccount
            // 
            this.tpInterestByAccount.Controls.Add(this.dgByAccountUnsettled);
            this.tpInterestByAccount.Controls.Add(this.dgByAccountSettled);
            this.tpInterestByAccount.Location = new System.Drawing.Point(0, 25);
            this.tpInterestByAccount.Name = "tpInterestByAccount";
            this.tpInterestByAccount.Selected = false;
            this.tpInterestByAccount.Size = new System.Drawing.Size(776, 303);
            this.tpInterestByAccount.TabIndex = 6;
            this.tpInterestByAccount.Title = "Interest By Account";
            this.tpInterestByAccount.Enter += new System.EventHandler(this.tpInterestByAccount_Enter);
            // 
            // dgByAccountUnsettled
            // 
            this.dgByAccountUnsettled.CaptionText = "Interest By Unsettled Accounts";
            this.dgByAccountUnsettled.DataMember = "";
            this.dgByAccountUnsettled.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgByAccountUnsettled.Location = new System.Drawing.Point(408, 16);
            this.dgByAccountUnsettled.Name = "dgByAccountUnsettled";
            this.dgByAccountUnsettled.ReadOnly = true;
            this.dgByAccountUnsettled.Size = new System.Drawing.Size(336, 272);
            this.dgByAccountUnsettled.TabIndex = 32;
            // 
            // dgByAccountSettled
            // 
            this.dgByAccountSettled.CaptionText = "Interest By Settled Accounts";
            this.dgByAccountSettled.DataMember = "";
            this.dgByAccountSettled.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgByAccountSettled.Location = new System.Drawing.Point(32, 16);
            this.dgByAccountSettled.Name = "dgByAccountSettled";
            this.dgByAccountSettled.ReadOnly = true;
            this.dgByAccountSettled.Size = new System.Drawing.Size(336, 272);
            this.dgByAccountSettled.TabIndex = 31;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnExcel);
            this.groupBox4.Controls.Add(this.label25);
            this.groupBox4.Controls.Add(this.txtBranchNo);
            this.groupBox4.Controls.Add(this.label26);
            this.groupBox4.Controls.Add(this.dtStart);
            this.groupBox4.Controls.Add(this.dtEnd);
            this.groupBox4.Controls.Add(this.label27);
            this.groupBox4.Controls.Add(this.label28);
            this.groupBox4.Controls.Add(this.txtBatchNo);
            this.groupBox4.Controls.Add(this.txtOpenAc);
            this.groupBox4.Controls.Add(this.txtCloseBal);
            this.groupBox4.Controls.Add(this.txtOpenBal);
            this.groupBox4.Controls.Add(this.label29);
            this.groupBox4.Controls.Add(this.label30);
            this.groupBox4.Controls.Add(this.label31);
            this.groupBox4.Controls.Add(this.txtCloseAc);
            this.groupBox4.Controls.Add(this.label32);
            this.groupBox4.Location = new System.Drawing.Point(8, 8);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(776, 120);
            this.groupBox4.TabIndex = 29;
            this.groupBox4.TabStop = false;
            // 
            // btnExcel
            // 
            this.btnExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnExcel.Image")));
            this.btnExcel.Location = new System.Drawing.Point(712, 56);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(32, 32);
            this.btnExcel.TabIndex = 48;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // label25
            // 
            this.label25.Location = new System.Drawing.Point(256, 24);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(100, 23);
            this.label25.TabIndex = 25;
            this.label25.Text = "Branch No";
            // 
            // txtBranchNo
            // 
            this.txtBranchNo.Location = new System.Drawing.Point(256, 48);
            this.txtBranchNo.Name = "txtBranchNo";
            this.txtBranchNo.ReadOnly = true;
            this.txtBranchNo.Size = new System.Drawing.Size(112, 20);
            this.txtBranchNo.TabIndex = 24;
            // 
            // label26
            // 
            this.label26.Location = new System.Drawing.Point(576, 24);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(100, 23);
            this.label26.TabIndex = 23;
            this.label26.Text = "Closing Figures";
            // 
            // dtStart
            // 
            this.dtStart.CustomFormat = "ddd dd MMM yyyy ";
            this.dtStart.Enabled = false;
            this.dtStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtStart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtStart.Location = new System.Drawing.Point(96, 56);
            this.dtStart.Name = "dtStart";
            this.dtStart.Size = new System.Drawing.Size(120, 20);
            this.dtStart.TabIndex = 22;
            this.dtStart.Value = new System.DateTime(2002, 5, 8, 0, 0, 0, 0);
            // 
            // dtEnd
            // 
            this.dtEnd.CustomFormat = "ddd dd MMM yyyy ";
            this.dtEnd.Enabled = false;
            this.dtEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtEnd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtEnd.Location = new System.Drawing.Point(96, 88);
            this.dtEnd.Name = "dtEnd";
            this.dtEnd.Size = new System.Drawing.Size(120, 20);
            this.dtEnd.TabIndex = 21;
            this.dtEnd.Value = new System.DateTime(2002, 5, 8, 0, 0, 0, 0);
            // 
            // label27
            // 
            this.label27.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.label27.Location = new System.Drawing.Point(392, 48);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(56, 16);
            this.label27.TabIndex = 12;
            this.label27.Text = "Accounts";
            // 
            // label28
            // 
            this.label28.Location = new System.Drawing.Point(16, 32);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(64, 16);
            this.label28.TabIndex = 1;
            this.label28.Text = "Batch No:";
            // 
            // txtBatchNo
            // 
            this.txtBatchNo.Location = new System.Drawing.Point(96, 32);
            this.txtBatchNo.Name = "txtBatchNo";
            this.txtBatchNo.ReadOnly = true;
            this.txtBatchNo.Size = new System.Drawing.Size(100, 20);
            this.txtBatchNo.TabIndex = 0;
            this.txtBatchNo.Text = "0";
            // 
            // txtOpenAc
            // 
            this.txtOpenAc.Location = new System.Drawing.Point(456, 48);
            this.txtOpenAc.Name = "txtOpenAc";
            this.txtOpenAc.ReadOnly = true;
            this.txtOpenAc.Size = new System.Drawing.Size(112, 20);
            this.txtOpenAc.TabIndex = 2;
            // 
            // txtCloseBal
            // 
            this.txtCloseBal.Location = new System.Drawing.Point(576, 80);
            this.txtCloseBal.Name = "txtCloseBal";
            this.txtCloseBal.ReadOnly = true;
            this.txtCloseBal.Size = new System.Drawing.Size(112, 20);
            this.txtCloseBal.TabIndex = 1;
            // 
            // txtOpenBal
            // 
            this.txtOpenBal.Location = new System.Drawing.Point(456, 80);
            this.txtOpenBal.Name = "txtOpenBal";
            this.txtOpenBal.ReadOnly = true;
            this.txtOpenBal.Size = new System.Drawing.Size(112, 20);
            this.txtOpenBal.TabIndex = 3;
            // 
            // label29
            // 
            this.label29.Location = new System.Drawing.Point(16, 64);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(64, 23);
            this.label29.TabIndex = 8;
            this.label29.Text = "Run Start :";
            // 
            // label30
            // 
            this.label30.Location = new System.Drawing.Point(16, 88);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(64, 23);
            this.label30.TabIndex = 11;
            this.label30.Text = "Run End:";
            // 
            // label31
            // 
            this.label31.Location = new System.Drawing.Point(456, 24);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(88, 23);
            this.label31.TabIndex = 13;
            this.label31.Text = "Opening Figures";
            // 
            // txtCloseAc
            // 
            this.txtCloseAc.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.txtCloseAc.Location = new System.Drawing.Point(576, 48);
            this.txtCloseAc.Name = "txtCloseAc";
            this.txtCloseAc.ReadOnly = true;
            this.txtCloseAc.Size = new System.Drawing.Size(112, 20);
            this.txtCloseAc.TabIndex = 16;
            this.txtCloseAc.TabStop = false;
            // 
            // label32
            // 
            this.label32.Location = new System.Drawing.Point(392, 80);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(48, 16);
            this.label32.TabIndex = 10;
            this.label32.Text = "Balances";
            // 
            // SummaryUpdateControlTotals
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 470);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.tcMain);
            this.Name = "SummaryUpdateControlTotals";
            this.Text = "Summary Update Control Report - Total ";
            this.tcMain.ResumeLayout(false);
            this.tpFinaStat.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgFSummary)).EndInit();
            this.tpOpenAccounts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgOSummary)).EndInit();
            this.tpInterest.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgISummary)).EndInit();
            this.tpInterestByAccount.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgByAccountUnsettled)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgByAccountSettled)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void btnExcel_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				/* save the current data grid contents to a CSV */
				string comma = ",";
				string path = "";
				DataView dv = null;
				switch (tcMain.SelectedTab.Name)
				{
					case "tpFinaStat":
						dv = (DataView)dgFSummary.DataSource;
						break;
					case "tpOpenAccounts":
						dv = (DataView)dgOSummary.DataSource;
						break;
					case "tpInterest":
						dv = (DataView)dgISummary.DataSource;
						break;
					default:
						return;
				}

				SaveFileDialog save = new SaveFileDialog();
				save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*" ;
				save.Title = "Save Journal Enquiry";
				save.CreatePrompt = true;

				if(save.ShowDialog() == DialogResult.OK)
				{
					path = save.FileName;
					FileInfo fi = new FileInfo(path);
					FileStream fs = fi.OpenWrite();
		
					string line = GetResource("T_DESCRIPTION") + comma +
						GetResource("T_SMRYHPVALUE") + comma +
						GetResource("T_SMRYCASHVALUE") + comma +
						GetResource("T_SMRYSPECIALVALUE") + comma +
						GetResource("T_TOTALAMT") + comma +
						GetResource("T_SMRYSOURCE") + comma +Environment.NewLine + Environment.NewLine;	
					byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
					fs.Write(blob,0,blob.Length);
		
					foreach(DataRowView row in dv)
					{					
						line =	row[CN.Description] + comma +
							Convert.ToString(row[CN.HPValue]) + comma +
							Convert.ToString(row[CN.CashValue]) + comma +
							Convert.ToString(row[CN.SpecialValue]) + comma +
							Convert.ToString(row[CN.Total]) + comma +
							row[CN.Source] + comma + Environment.NewLine;	

						blob = System.Text.Encoding.UTF8.GetBytes(line);
						fs.Write(blob,0,blob.Length);
					}
					fs.Close();						

					/* attempt to launch Excel. May get a COMException if Excel is not 
						* installed */
					try
					{
						/* we have to use Reflection to call the Open method because 
							* the methods have different argument lists for the 
							* different versions of Excel - JJ */
						object[] args = null;
						Excel.Application excel = new Excel.Application();

						if(excel.Version == "10.0")	/* Excel2002 */
							args = new object[]{path, 0, true, 2, "", "", true, Excel.XlPlatform.xlWindows, comma, true, false, 0, true, false, false };
						else
							args = new object[]{path, 0, true, 2, "", "", true, Excel.XlPlatform.xlWindows, comma, true, false, 0, true };

						/* Retrieve the Workbooks property */
						object wbs = excel.GetType().InvokeMember("Workbooks", BindingFlags.Public|BindingFlags.GetField|BindingFlags.GetProperty, null, excel, new Object[] {});

						/* call the Open method */
						object wb = wbs.GetType().InvokeMember("Open", BindingFlags.Public | BindingFlags.InvokeMethod, null, wbs, args);

						excel.Visible = true;
					}
					catch(COMException)
					{
						/*change back slashes to forward slashes so the path doesn't
							* get split into multiple lines */
						ShowInfo("M_EXCELNOTFOUND", new object[] {path.Replace("\\", "/")});
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
			}
		}

		private void btnInterfaced_Click(object sender, System.EventArgs e)
		{
			try
			{
				int index = dgFSummary.CurrentRowIndex;
				int branchNo = 0;
				Wait();

				if(index >= 0)
				{
					if(txtBranchNo.Text != "ALL")
						branchNo = Convert.ToInt32(txtBranchNo.Text);

					FinancialInterface fi = new FinancialInterface(FormRoot, this, 
						Convert.ToInt32(txtBatchNo.Text),
						branchNo, _useLiveDatabase); //if live or reporting db
										
					((MainForm)this.FormRoot).AddTabPage(fi);
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void btnDelInterface_Click(object sender, System.EventArgs e)
		{
			try
			{
				int index = dgFSummary.CurrentRowIndex;
				int branchNo = 0;
				Wait();

				if(index >= 0)
				{
					if(txtBranchNo.Text != "ALL")
						branchNo = Convert.ToInt32(txtBranchNo.Text);

					DeliveryInterface di = new DeliveryInterface(FormRoot, this, 
						Convert.ToInt32(txtBatchNo.Text),
						branchNo,
						_useLiveDatabase);
										
					((MainForm)this.FormRoot).AddTabPage(di);
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void tpInterestByAccount_Enter(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				// This loads from FinTrans and so takes longer than the other queries.
				// So don't want to do this when the form opens.
				if (dgByAccountUnsettled.DataSource == null)
				{
					((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_INTERESTACCOUNTS");
					populateSummaryTable("INTERESTBYACCOUNT");
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				((MainForm)this.FormRoot).statusBar1.Text = "";
				StopWait();
			}
		}

        //private void btnBroker_Click(object sender, EventArgs e)
        //{

        //        SummaryUpdateBrokerInterface si = new SummaryUpdateBrokerInterface(FormRoot, this);
										
        //        ((MainForm)this.FormRoot).AddTabPage(si);
            
        //}

	}
}
