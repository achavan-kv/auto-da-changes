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
	/// Summary description for SummaryReportControlMain.
	/// </summary>
	public class SummaryReportControlMain : CommonForm
	{
		private decimal _firstBatchNo = 0;
		private decimal _lastBatchNo = 0;

		private System.Windows.Forms.NumericUpDown numLastBatchNo;
		private System.Windows.Forms.NumericUpDown numFirstBatchNo;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnFind;
		private string errorTxt;
		private System.Windows.Forms.DataGrid dgSummaryDetails;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnBranchFigures;
        private RadioButton radioLive;
        private RadioButton radioReporting;
        private GroupBox groupBox2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SummaryReportControlMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}
		public SummaryReportControlMain(Form root, Form parent)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			FormRoot = root;
			FormParent = parent;
			numFirstBatchNo.Value = 0;
			numLastBatchNo.Value = 0;
			getSummarydetails();
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
            this.dgSummaryDetails = new System.Windows.Forms.DataGrid();
            this.numLastBatchNo = new System.Windows.Forms.NumericUpDown();
            this.numFirstBatchNo = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnFind = new System.Windows.Forms.Button();
            this.btnBranchFigures = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioLive = new System.Windows.Forms.RadioButton();
            this.radioReporting = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgSummaryDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLastBatchNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFirstBatchNo)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgSummaryDetails
            // 
            this.dgSummaryDetails.CaptionText = "Summary Update Control Report Batches";
            this.dgSummaryDetails.DataMember = "";
            this.dgSummaryDetails.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgSummaryDetails.Location = new System.Drawing.Point(24, 104);
            this.dgSummaryDetails.Name = "dgSummaryDetails";
            this.dgSummaryDetails.ReadOnly = true;
            this.dgSummaryDetails.Size = new System.Drawing.Size(744, 360);
            this.dgSummaryDetails.TabIndex = 17;
            this.dgSummaryDetails.DoubleClick += new System.EventHandler(this.dgSummaryDetails_DoubleClick);
            // 
            // numLastBatchNo
            // 
            this.numLastBatchNo.Location = new System.Drawing.Point(117, 56);
            this.numLastBatchNo.Maximum = new decimal(new int[] {
            -727379969,
            232,
            0,
            0});
            this.numLastBatchNo.Name = "numLastBatchNo";
            this.numLastBatchNo.Size = new System.Drawing.Size(104, 20);
            this.numLastBatchNo.TabIndex = 24;
            // 
            // numFirstBatchNo
            // 
            this.numFirstBatchNo.Location = new System.Drawing.Point(117, 24);
            this.numFirstBatchNo.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numFirstBatchNo.Name = "numFirstBatchNo";
            this.numFirstBatchNo.Size = new System.Drawing.Size(104, 20);
            this.numFirstBatchNo.TabIndex = 23;
            this.numFirstBatchNo.ValueChanged += new System.EventHandler(this.numFirstBatchNo_ValueChanged);
            this.numFirstBatchNo.Leave += new System.EventHandler(this.numFirstBatchNo_Leave);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(5, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 16);
            this.label2.TabIndex = 21;
            this.label2.Text = "Last Batch No:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(5, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 16);
            this.label1.TabIndex = 20;
            this.label1.Text = "First Batch No:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(261, 40);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(72, 24);
            this.btnFind.TabIndex = 22;
            this.btnFind.Text = "Find";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnBranchFigures
            // 
            this.btnBranchFigures.Location = new System.Drawing.Point(608, 40);
            this.btnBranchFigures.Name = "btnBranchFigures";
            this.btnBranchFigures.Size = new System.Drawing.Size(104, 24);
            this.btnBranchFigures.TabIndex = 25;
            this.btnBranchFigures.Text = "Branch Figures";
            this.btnBranchFigures.Click += new System.EventHandler(this.btnBranchFigures_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numFirstBatchNo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numLastBatchNo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnFind);
            this.groupBox1.Location = new System.Drawing.Point(32, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(343, 88);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Enquiry Details ";
            // 
            // radioLive
            // 
            this.radioLive.Location = new System.Drawing.Point(389, 64);
            this.radioLive.Name = "radioLive";
            this.radioLive.Size = new System.Drawing.Size(56, 24);
            this.radioLive.TabIndex = 34;
            this.radioLive.Text = "Live";
            this.radioLive.CheckedChanged += new System.EventHandler(this.radioLive_CheckedChanged);
            // 
            // radioReporting
            // 
            this.radioReporting.Checked = true;
            this.radioReporting.Location = new System.Drawing.Point(389, 32);
            this.radioReporting.Name = "radioReporting";
            this.radioReporting.Size = new System.Drawing.Size(72, 24);
            this.radioReporting.TabIndex = 33;
            this.radioReporting.TabStop = true;
            this.radioReporting.Text = "Reporting";
            this.radioReporting.CheckedChanged += new System.EventHandler(this.radioReporting_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(381, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(98, 88);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Database";
            // 
            // SummaryReportControlMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.radioLive);
            this.Controls.Add(this.radioReporting);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnBranchFigures);
            this.Controls.Add(this.dgSummaryDetails);
            this.Name = "SummaryReportControlMain";
            this.Text = "Summary Update Control Report";
            ((System.ComponentModel.ISupportInitialize)(this.dgSummaryDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLastBatchNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFirstBatchNo)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void btnFind_Click(object sender, System.EventArgs e)
		{
			bool doSearch =  true; 
			if (numLastBatchNo.Value < numFirstBatchNo.Value)
			{
				doSearch = false;
				numFirstBatchNo.Value = this._firstBatchNo;
				numLastBatchNo.Value = this._lastBatchNo;
			}
			if (doSearch)
			{
				getSummarydetails();
			}
		}
		private void getSummarydetails()
		{
         Function = "getSummarydetails()";
         try
         {
            dgSummaryDetails.DataSource = null;
            dgSummaryDetails.TableStyles.Clear();
            DataSet ds = EodManager.GetSummaryUpdateControlDetails((int)numFirstBatchNo.Value, (int)numLastBatchNo.Value, radioLive.Checked, out errorTxt);
            DataView summary = ds.Tables[TN.SummaryControl].DefaultView;

            dgSummaryDetails.DataSource = summary;

            DataGridTableStyle tabStyle = new DataGridTableStyle();
            tabStyle.MappingName = summary.Table.TableName;
            dgSummaryDetails.TableStyles.Add(tabStyle);
            tabStyle.GridColumnStyles[CN.BatchNo].Width = 75;
            tabStyle.GridColumnStyles[CN.BatchNo].HeaderText = GetResource("T_RUNNO");

            tabStyle.GridColumnStyles[CN.RunStart].Width = 70;
            tabStyle.GridColumnStyles[CN.RunStart].HeaderText = GetResource("T_RUNSTART");

            tabStyle.GridColumnStyles[CN.RunEnd].Width = 70;
            tabStyle.GridColumnStyles[CN.RunEnd].HeaderText = GetResource("T_RUNEND");

            tabStyle.GridColumnStyles[CN.RunOpenAC].Width = 100;
            tabStyle.GridColumnStyles[CN.RunOpenAC].HeaderText = GetResource("T_OPENACSTART");
            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.RunOpenAC]).Format = "N0";

            tabStyle.GridColumnStyles[CN.RunOpenBalance].Width = 100;
            tabStyle.GridColumnStyles[CN.RunOpenBalance].HeaderText = GetResource("T_BALANCESTART");
            tabStyle.GridColumnStyles[CN.RunOpenBalance].Alignment = HorizontalAlignment.Left;
            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.RunOpenBalance]).Format = DecimalPlaces;

            tabStyle.GridColumnStyles[CN.RunCloseAC].Width = 100;
            tabStyle.GridColumnStyles[CN.RunCloseAC].HeaderText = GetResource("T_OPENACCLOSE");
            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.RunCloseAC]).Format = "N0";

            tabStyle.GridColumnStyles[CN.RunCloseBalance].Width = 100;
            tabStyle.GridColumnStyles[CN.RunCloseBalance].HeaderText = GetResource("T_BALANCEEND");
            tabStyle.GridColumnStyles[CN.RunCloseBalance].Alignment = HorizontalAlignment.Left;
            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.RunCloseBalance]).Format = DecimalPlaces;

            tabStyle.GridColumnStyles[CN.RunMovement].Width = 100;
            tabStyle.GridColumnStyles[CN.RunMovement].HeaderText = GetResource("T_MOVEMENT");
            tabStyle.GridColumnStyles[CN.RunMovement].Alignment = HorizontalAlignment.Left;
            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.RunMovement]).Format = DecimalPlaces;

            int lastrow = summary.Count - 1;
            if (lastrow >= 0 && _lastBatchNo == 0)
            {
               _firstBatchNo = numFirstBatchNo.Value = Convert.ToInt32(((DataView)dgSummaryDetails.DataSource)[lastrow][CN.BatchNo]);
               _lastBatchNo = numLastBatchNo.Value = Convert.ToInt32(((DataView)dgSummaryDetails.DataSource)[0][CN.BatchNo]);
            }
         }
         catch (Exception ex)
         {
            Catch(ex, Function);
         }
		}

		private void btnBranchFigures_Click(object sender, System.EventArgs e)
		{

			int index = dgSummaryDetails.CurrentRowIndex;
			if(index>=0)
			{
				int batchNo = Convert.ToInt32(((DataView)dgSummaryDetails.DataSource)[index][CN.BatchNo]);
				DateTime runstart = Convert.ToDateTime(((DataView)dgSummaryDetails.DataSource)[index][CN.RunStart]);
				DateTime runend = Convert.ToDateTime(((DataView)dgSummaryDetails.DataSource)[index][CN.RunEnd]);
				int openac = Convert.ToInt32(((DataView)dgSummaryDetails.DataSource)[index][CN.RunOpenAC]);
				int closeac = Convert.ToInt32(((DataView)dgSummaryDetails.DataSource)[index][CN.RunCloseAC]);
				decimal openbal = Convert.ToDecimal(((DataView)dgSummaryDetails.DataSource)[index][CN.RunOpenBalance]);
				decimal closebal = Convert.ToDecimal(((DataView)dgSummaryDetails.DataSource)[index][CN.RunCloseBalance]);
				
				SummaryControlBranchFigures sbf = new SummaryControlBranchFigures(this.FormRoot, this, batchNo, runstart ,runend , openac, openbal, closeac, closebal, radioLive.Checked);

				((MainForm)this.FormRoot).AddTabPage(sbf);		
			}
		}

		private void dgSummaryDetails_DoubleClick(object sender, System.EventArgs e)
		{
			btnBranchFigures_Click(sender, e);
		}

		private void numFirstBatchNo_ValueChanged(object sender, System.EventArgs e)
		{
			if (numFirstBatchNo.Value <= _lastBatchNo)
				numLastBatchNo.Value = numFirstBatchNo.Value;
		}

		private void numFirstBatchNo_Leave(object sender, System.EventArgs e)
		{
			if (numFirstBatchNo.Value <= _lastBatchNo)
				numLastBatchNo.Value = numFirstBatchNo.Value;
		}

        private void radioReporting_CheckedChanged(object sender, EventArgs e)
        {
            getSummarydetails();  
        }

        private void radioLive_CheckedChanged(object sender, EventArgs e)
        {
            getSummarydetails();
        }
	}
}
