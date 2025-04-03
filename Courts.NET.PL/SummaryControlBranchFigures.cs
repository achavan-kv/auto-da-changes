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

namespace STL.PL
{
	/// <summary>
	/// Summary description for SummaryControlBranchFigures.
	/// </summary>
	public class SummaryControlBranchFigures : CommonForm
	{

		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.DateTimePicker dtStart;
		private System.Windows.Forms.TextBox txtBatchNo;
		private string errorTxt;
		private int _batchNo;
		private DateTime _runStart;
		private DateTime _runEnd;
		private int _openAcc;
		private decimal _openBalance;
		private int _closeAcc;
		private decimal _closeBalance;
		private System.Windows.Forms.DataGrid dgBranchDetails;
		private System.Windows.Forms.DateTimePicker dtEnd;
		private System.Windows.Forms.TextBox txtOpenAc;
		private System.Windows.Forms.TextBox txtclosebalance;
		private System.Windows.Forms.TextBox txtOpenBalance;
		private System.Windows.Forms.TextBox txtCloseAc;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnCmpTotal;
		private System.Windows.Forms.Button button1;

        private bool _useLiveDatabase;


		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SummaryControlBranchFigures()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}
		public SummaryControlBranchFigures( Form root, Form parent, int batchNo,
											DateTime runstart , DateTime runend , 
											int openac, decimal openbal, int closeac, 
											decimal  closebal, bool useLiveDatabase)
		{

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            _useLiveDatabase = useLiveDatabase;

			FormRoot = root;
			FormParent = parent;
			_batchNo = batchNo;
		    _runStart = runstart;
		    _runEnd = runend;
		    _openAcc = openac;
		    _openBalance = openbal;
		    _closeAcc = closeac;
		    _closeBalance = closebal;
			txtBatchNo.Text = _batchNo.ToString();
			txtOpenAc.Text = _openAcc.ToString("N0");
			txtOpenBalance.Text = _openBalance.ToString(DecimalPlaces);
			txtCloseAc.Text = _closeAcc.ToString("N0");
			txtclosebalance.Text = _closeBalance.ToString(DecimalPlaces);
			dtStart.Value = _runStart;
			dtEnd.Value = _runEnd;

			dgBranchDetails.DataSource = null;
			dgBranchDetails.TableStyles.Clear();
			DataSet ds = EodManager.GetSummaryControlBrancgFigures(Convert.ToInt32(txtBatchNo.Text), _useLiveDatabase, out errorTxt);
			DataView summary = ds.Tables[TN.SummaryControl].DefaultView;

			dgBranchDetails.DataSource = summary;

			DataGridTableStyle tabStyle = new DataGridTableStyle();
			tabStyle.MappingName = summary.Table.TableName;
			dgBranchDetails.TableStyles.Add(tabStyle);
			tabStyle.GridColumnStyles[CN.BranchNo].Width = 60;
			tabStyle.GridColumnStyles[CN.BranchNo].HeaderText = GetResource("T_BRANCH");

			tabStyle.GridColumnStyles[CN.RunOpenAC].Width = 110;
			tabStyle.GridColumnStyles[CN.RunOpenAC].HeaderText = GetResource("T_OPENACSTART");
			((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.RunOpenAC]).Format = "N0";

			tabStyle.GridColumnStyles[CN.RunOpenBalance].Width = 110;
			tabStyle.GridColumnStyles[CN.RunOpenBalance].HeaderText = GetResource("T_BALANCESTART");
			tabStyle.GridColumnStyles[CN.RunOpenBalance].Alignment = HorizontalAlignment.Left;
			((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.RunOpenBalance]).Format = DecimalPlaces;

			tabStyle.GridColumnStyles[CN.RunCloseAC].Width = 110;
			tabStyle.GridColumnStyles[CN.RunCloseAC].HeaderText = GetResource("T_OPENACCLOSE");
			((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.RunCloseAC]).Format = "N0";

			tabStyle.GridColumnStyles[CN.RunCloseBalance].Width = 130;
			tabStyle.GridColumnStyles[CN.RunCloseBalance].HeaderText = GetResource("T_BALANCEEND");
			tabStyle.GridColumnStyles[CN.RunCloseBalance].Alignment = HorizontalAlignment.Left;
			((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.RunCloseBalance]).Format = DecimalPlaces;

			tabStyle.GridColumnStyles[CN.RunMovement].Width = 100;
			tabStyle.GridColumnStyles[CN.RunMovement].HeaderText = GetResource("T_MOVEMENT");
			tabStyle.GridColumnStyles[CN.RunMovement].Alignment = HorizontalAlignment.Left;
			((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.RunMovement]).Format = DecimalPlaces;
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
			this.btnExit = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.dtStart = new System.Windows.Forms.DateTimePicker();
			this.dtEnd = new System.Windows.Forms.DateTimePicker();
			this.label4 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.txtBatchNo = new System.Windows.Forms.TextBox();
			this.txtOpenAc = new System.Windows.Forms.TextBox();
			this.txtclosebalance = new System.Windows.Forms.TextBox();
			this.txtOpenBalance = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.txtCloseAc = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.dgBranchDetails = new System.Windows.Forms.DataGrid();
			this.btnCmpTotal = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgBranchDetails)).BeginInit();
			this.SuspendLayout();
			// 
			// btnExit
			// 
			this.btnExit.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
			this.btnExit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnExit.Location = new System.Drawing.Point(680, 264);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(64, 23);
			this.btnExit.TabIndex = 15;
			this.btnExit.Text = "Exit";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.label3,
																					this.dtStart,
																					this.dtEnd,
																					this.label4,
																					this.label10,
																					this.txtBatchNo,
																					this.txtOpenAc,
																					this.txtclosebalance,
																					this.txtOpenBalance,
																					this.label11,
																					this.label12,
																					this.label13,
																					this.txtCloseAc,
																					this.label14});
			this.groupBox1.Location = new System.Drawing.Point(20, 23);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(752, 120);
			this.groupBox1.TabIndex = 21;
			this.groupBox1.TabStop = false;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(584, 16);
			this.label3.Name = "label3";
			this.label3.TabIndex = 23;
			this.label3.Text = "Closing Figures";
			// 
			// dtStart
			// 
			this.dtStart.CustomFormat = "ddd dd MMM yyyy ";
			this.dtStart.Enabled = false;
			this.dtStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtStart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.dtStart.Location = new System.Drawing.Point(120, 56);
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
			this.dtEnd.Location = new System.Drawing.Point(120, 88);
			this.dtEnd.Name = "dtEnd";
			this.dtEnd.Size = new System.Drawing.Size(120, 20);
			this.dtEnd.TabIndex = 21;
			this.dtEnd.Value = new System.DateTime(2002, 5, 8, 0, 0, 0, 0);
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
			this.label4.Location = new System.Drawing.Point(344, 40);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 16);
			this.label4.TabIndex = 12;
			this.label4.Text = "Accounts";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(40, 32);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(64, 16);
			this.label10.TabIndex = 1;
			this.label10.Text = "Batch No:";
			// 
			// txtBatchNo
			// 
			this.txtBatchNo.Location = new System.Drawing.Point(120, 32);
			this.txtBatchNo.Name = "txtBatchNo";
			this.txtBatchNo.ReadOnly = true;
			this.txtBatchNo.TabIndex = 0;
			this.txtBatchNo.Text = "0";
			// 
			// txtOpenAc
			// 
			this.txtOpenAc.Location = new System.Drawing.Point(424, 40);
			this.txtOpenAc.Name = "txtOpenAc";
			this.txtOpenAc.ReadOnly = true;
			this.txtOpenAc.Size = new System.Drawing.Size(112, 20);
			this.txtOpenAc.TabIndex = 2;
			this.txtOpenAc.Text = "";
			// 
			// txtclosebalance
			// 
			this.txtclosebalance.Location = new System.Drawing.Point(584, 72);
			this.txtclosebalance.Name = "txtclosebalance";
			this.txtclosebalance.ReadOnly = true;
			this.txtclosebalance.Size = new System.Drawing.Size(112, 20);
			this.txtclosebalance.TabIndex = 1;
			this.txtclosebalance.Text = "";
			// 
			// txtOpenBalance
			// 
			this.txtOpenBalance.Location = new System.Drawing.Point(424, 72);
			this.txtOpenBalance.Name = "txtOpenBalance";
			this.txtOpenBalance.ReadOnly = true;
			this.txtOpenBalance.Size = new System.Drawing.Size(112, 20);
			this.txtOpenBalance.TabIndex = 3;
			this.txtOpenBalance.Text = "";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(40, 64);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(64, 23);
			this.label11.TabIndex = 8;
			this.label11.Text = "Run Start :";
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(40, 88);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(64, 23);
			this.label12.TabIndex = 11;
			this.label12.Text = "Run End:";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(424, 16);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(88, 23);
			this.label13.TabIndex = 13;
			this.label13.Text = "Opening Figures";
			// 
			// txtCloseAc
			// 
			this.txtCloseAc.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.txtCloseAc.Location = new System.Drawing.Point(584, 40);
			this.txtCloseAc.Name = "txtCloseAc";
			this.txtCloseAc.ReadOnly = true;
			this.txtCloseAc.Size = new System.Drawing.Size(120, 20);
			this.txtCloseAc.TabIndex = 16;
			this.txtCloseAc.TabStop = false;
			this.txtCloseAc.Text = "";
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(344, 72);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(48, 16);
			this.label14.TabIndex = 10;
			this.label14.Text = "Balances";
			// 
			// dgBranchDetails
			// 
			this.dgBranchDetails.CaptionText = "Summary Update Control Report - Branch Figures";
			this.dgBranchDetails.DataMember = "";
			this.dgBranchDetails.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgBranchDetails.Location = new System.Drawing.Point(16, 160);
			this.dgBranchDetails.Name = "dgBranchDetails";
			this.dgBranchDetails.ReadOnly = true;
			this.dgBranchDetails.Size = new System.Drawing.Size(656, 288);
			this.dgBranchDetails.TabIndex = 20;
			this.dgBranchDetails.DoubleClick += new System.EventHandler(this.dgBranchDetails_DoubleClick);
			// 
			// btnCmpTotal
			// 
			this.btnCmpTotal.Location = new System.Drawing.Point(696, 280);
			this.btnCmpTotal.Name = "btnCmpTotal";
			this.btnCmpTotal.Size = new System.Drawing.Size(64, 32);
			this.btnCmpTotal.TabIndex = 22;
			this.btnCmpTotal.Text = "Company Totals";
			this.btnCmpTotal.Click += new System.EventHandler(this.btnCmpTotal_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(696, 232);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(64, 32);
			this.button1.TabIndex = 23;
			this.button1.Text = "Branch Totals";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// SummaryControlBranchFigures
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(792, 470);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.button1,
																		  this.btnCmpTotal,
																		  this.groupBox1,
																		  this.dgBranchDetails});
			this.Name = "SummaryControlBranchFigures";
			this.Text = "Summary Update Control Report - Branch Figures";
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgBranchDetails)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

        //private void tcMain_SelectionChanged(object sender, System.EventArgs e)
        //{
		
        //}

		private void btnCmpTotal_Click(object sender, System.EventArgs e)
		{
            SummaryUpdateControlTotals sct = new SummaryUpdateControlTotals(this.FormRoot, this, _batchNo, _runStart ,_runEnd , _openAcc, _openBalance, _closeAcc, _closeBalance, 0, _useLiveDatabase );
			((MainForm)this.FormRoot).AddTabPage(sct);
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			int branchNo = 0;
			int branchOpenAc = 0;
			int branchCloseAc = 0; 
			decimal branchOpenBal = 0;
			decimal branchCloseBal = 0;
			int index = dgBranchDetails.CurrentRowIndex;

			if(index>=0)
			{
				branchNo = Convert.ToInt32(((DataView)dgBranchDetails.DataSource)[index][CN.BranchNo]);
				branchOpenAc = Convert.ToInt32(((DataView)dgBranchDetails.DataSource)[index][CN.RunOpenAC]);
				branchCloseAc = Convert.ToInt32(((DataView)dgBranchDetails.DataSource)[index][CN.RunCloseAC]);
				branchOpenBal = Convert.ToDecimal(((DataView)dgBranchDetails.DataSource)[index][CN.RunOpenBalance]);
				branchCloseBal = Convert.ToDecimal(((DataView)dgBranchDetails.DataSource)[index][CN.RunCloseBalance]);
			}
			SummaryUpdateControlTotals sct = new SummaryUpdateControlTotals(this.FormRoot, this, _batchNo, _runStart ,_runEnd , branchOpenAc, branchOpenBal, branchCloseAc, branchCloseBal, branchNo,_useLiveDatabase );

			((MainForm)this.FormRoot).AddTabPage(sct);
		}

		private void dgBranchDetails_DoubleClick(object sender, System.EventArgs e)
		{
			button1_Click(sender, e);
		}
	}
}
