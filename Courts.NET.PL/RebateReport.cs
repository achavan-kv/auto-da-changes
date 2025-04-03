using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Web.Services.Protocols;
using System.Diagnostics;
using STL.Common;
using System.Threading;
using System.Globalization;
using System.Reflection;
using System.Drawing.Printing;
using STL.Common.Static;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Categories;
using System.Runtime.InteropServices;
using System.Resources;
using STL.Common.Constants.TableNames;
using System.Xml;
using System.Text;
using AxSHDocVw;
using STL.PL.WS1;
using STL.PL.WS2;
using STL.PL.WS3;
using STL.PL.WS4;
using STL.PL.WS5;
using STL.PL.WS6;
using STL.PL.WS7;
using STL.PL.WS8;
using System.Collections.Specialized;
using System.IO;


namespace STL.PL
{
	/// <summary>
	/// Summary description for RebateReport.
	/// </summary>
	public class RebateReport : CommonForm
	{
        private string error = "";
        private DataTable _branchData;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button11;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private Crownwood.Magic.Controls.TabControl tcRebate;
		private Crownwood.Magic.Controls.TabPage tpReportAB;
		private Crownwood.Magic.Controls.TabPage tpReportCD;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtEndPeriodB;
		private System.Windows.Forms.TextBox txtEndPeriodA;
		private System.Windows.Forms.TextBox txtEndYearD;
		private System.Windows.Forms.TextBox txtEndPeriodD;
		private System.Windows.Forms.TextBox txtEndPeriodC;

		private string err = "";
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.ComboBox drpPeriod;
		private System.Windows.Forms.TextBox txtPeriod;
		private System.Windows.Forms.Button btnRun;
		private System.Windows.Forms.Button btnPrintReportb;
		private System.Windows.Forms.Button btnPrintReportA;
		private System.Windows.Forms.Button btnPrintAll;
		private bool staticLoaded = false;
		private System.Windows.Forms.DataGrid dgReportA;
		private System.Windows.Forms.DataGrid dgReportB;

		StringCollection headings = new StringCollection();
		private System.Windows.Forms.TextBox txtRunRepA;
		private System.Windows.Forms.TextBox txtRunRepB;
		private System.Windows.Forms.TextBox txtRunRepC;
		private System.Windows.Forms.TextBox txtRunRepD;
		private System.Windows.Forms.DataGrid dgReportD;
		private System.Windows.Forms.DataGrid dgReportC; 	

		DataSet periods = null;
		private System.Windows.Forms.Button btnExcelRepB;
		private System.Windows.Forms.Button btnExcelRepA;
		private System.Windows.Forms.Button btnExcelRepD;
		private System.Windows.Forms.Button btnExcelRepC;
		private System.Windows.Forms.Button btnPrintReportD;
		private System.Windows.Forms.Button btnPrintReportC;
        private ComboBox drpBranchNo;
        private Label label13;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public RebateReport(Form root, Form parent)
		{
			InitializeComponent();
			FormRoot = root;
			FormParent = parent;

			InitialiseStaticData();

			txtPeriod.BackColor = SystemColors.Window;
			txtRunRepA.BackColor = SystemColors.Window;
			txtRunRepB.BackColor = SystemColors.Window;
			txtRunRepC.BackColor = SystemColors.Window;
			txtRunRepD.BackColor = SystemColors.Window;
			txtEndPeriodA.BackColor = SystemColors.Window;
			txtEndPeriodB.BackColor = SystemColors.Window;
			txtEndPeriodC.BackColor = SystemColors.Window;
			txtEndPeriodD.BackColor = SystemColors.Window;
		}

		private void InitialiseStaticData()
		{
			StringCollection endDates = new StringCollection(); 	
			endDates.Add("");
			string nextPeriodEnd = "";

			try
			{
				Wait();

				XmlUtilities xml = new XmlUtilities();
				XmlDocument dropDowns = new XmlDocument();
				dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

				if(StaticData.Tables[TN.EndPeriods]==null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.EndPeriods, null));
				if(dropDowns.DocumentElement.ChildNodes.Count>0)
				{
					DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out err);
					if(err.Length>0)
						ShowError(err);
					else
					{
						foreach(DataTable dt in ds.Tables)
						{
							StaticData.Tables[dt.TableName] = dt;
						}
					}
				}

				foreach(DataRow row in ((DataTable)StaticData.Tables[TN.EndPeriods]).Rows)
				{
					headings.Add((string)row.ItemArray[1]);
				}

				periods = AccountManager.GetPeriodEndDates(out nextPeriodEnd, out err);
				
				if(err.Length > 0)
					ShowError(err);
				else
				{
					foreach(DataRow row in periods.Tables[0].Rows)
					{
						if(Convert.ToInt32(row[CN.Position]) > 0)
							endDates.Add(Convert.ToString(row[CN.EndDate]));
					}

					txtPeriod.Text = nextPeriodEnd;
					drpPeriod.DataSource = endDates;

					if(Convert.ToDateTime(txtPeriod.Text) > DateTime.Today)
						btnRun.Enabled = false;

					staticLoaded = true;
				}

                // Populate Branch dropdown
                //XmlUtilities xml = new XmlUtilities();
                //XmlDocument dropDowns = new XmlDocument();
                //dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                if (StaticData.Tables[TN.BranchNumber] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));

                if (dropDowns.DocumentElement.ChildNodes.Count > 0)
                {
                    DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out error);
                    if (error.Length > 0)
                        ShowError(error);
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            StaticData.Tables[dt.TableName] = dt;
                        }
                    }
                }

                //Now customise the dropdowns..         

                //Customise the Branch data to be displayed in the dropdown..
                if (_branchData == null)
                {
                    _branchData = ((DataTable)StaticData.Tables[TN.BranchNumber]).Clone();

                    DataRow row = _branchData.NewRow();
                    row[CN.BranchNo] = -1;
                    row[CN.BranchName] = GetResource("Country");
                    _branchData.Rows.Add(row);
                    DataRow row2 = _branchData.NewRow();
                    _branchData.NewRow();
                    row2[CN.BranchNo] = 0;
                    row2[CN.BranchName] = GetResource("AllBranches");
                    _branchData.Rows.Add(row2);
                    foreach (DataRow copyRow in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                    {
                        row = _branchData.NewRow();
                        row[CN.BranchNo] = copyRow[CN.BranchNo];
                        row[CN.BranchName] = copyRow[CN.BranchNo].ToString()
                            + " : " + copyRow[CN.BranchName].ToString();
                        _branchData.Rows.Add(row);
                    }
                }

                drpBranchNo.DataSource = _branchData;
                drpBranchNo.ValueMember = CN.BranchNo;
                drpBranchNo.DisplayMember = CN.BranchName;
                //drpBranchNo.SelectedValue = Config.BranchCode;
			}
			catch(Exception ex)
			{
				Catch(ex, "InitialiseStaticData");
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
				if (components != null) 
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
            this.btnRun = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tcRebate = new Crownwood.Magic.Controls.TabControl();
            this.tpReportAB = new Crownwood.Magic.Controls.TabPage();
            this.txtRunRepB = new System.Windows.Forms.TextBox();
            this.txtRunRepA = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnPrintReportb = new System.Windows.Forms.Button();
            this.btnExcelRepB = new System.Windows.Forms.Button();
            this.btnExcelRepA = new System.Windows.Forms.Button();
            this.txtEndPeriodB = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dgReportB = new System.Windows.Forms.DataGrid();
            this.txtEndPeriodA = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dgReportA = new System.Windows.Forms.DataGrid();
            this.btnPrintReportA = new System.Windows.Forms.Button();
            this.tpReportCD = new Crownwood.Magic.Controls.TabPage();
            this.txtRunRepD = new System.Windows.Forms.TextBox();
            this.txtRunRepC = new System.Windows.Forms.TextBox();
            this.txtEndYearD = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.btnExcelRepD = new System.Windows.Forms.Button();
            this.btnPrintReportD = new System.Windows.Forms.Button();
            this.txtEndPeriodD = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btnExcelRepC = new System.Windows.Forms.Button();
            this.btnPrintReportC = new System.Windows.Forms.Button();
            this.txtEndPeriodC = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.dgReportD = new System.Windows.Forms.DataGrid();
            this.dgReportC = new System.Windows.Forms.DataGrid();
            this.btnPrintAll = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.drpPeriod = new System.Windows.Forms.ComboBox();
            this.txtPeriod = new System.Windows.Forms.TextBox();
            this.drpBranchNo = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tcRebate.SuspendLayout();
            this.tpReportAB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgReportB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgReportA)).BeginInit();
            this.tpReportCD.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgReportD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgReportC)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRun
            // 
            this.btnRun.Enabled = false;
            this.btnRun.Location = new System.Drawing.Point(520, 8);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(58, 40);
            this.btnRun.TabIndex = 0;
            this.btnRun.Text = "Run All Reports";
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Next Period End Date For Report  Run:";
            // 
            // tcRebate
            // 
            this.tcRebate.IDEPixelArea = true;
            this.tcRebate.Location = new System.Drawing.Point(8, 56);
            this.tcRebate.Name = "tcRebate";
            this.tcRebate.PositionTop = true;
            this.tcRebate.SelectedIndex = 0;
            this.tcRebate.SelectedTab = this.tpReportAB;
            this.tcRebate.Size = new System.Drawing.Size(768, 416);
            this.tcRebate.TabIndex = 31;
            this.tcRebate.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpReportAB,
            this.tpReportCD});
            // 
            // tpReportAB
            // 
            this.tpReportAB.Controls.Add(this.txtRunRepB);
            this.tpReportAB.Controls.Add(this.txtRunRepA);
            this.tpReportAB.Controls.Add(this.label7);
            this.tpReportAB.Controls.Add(this.btnPrintReportb);
            this.tpReportAB.Controls.Add(this.btnExcelRepB);
            this.tpReportAB.Controls.Add(this.btnExcelRepA);
            this.tpReportAB.Controls.Add(this.txtEndPeriodB);
            this.tpReportAB.Controls.Add(this.label5);
            this.tpReportAB.Controls.Add(this.label3);
            this.tpReportAB.Controls.Add(this.dgReportB);
            this.tpReportAB.Controls.Add(this.txtEndPeriodA);
            this.tpReportAB.Controls.Add(this.label4);
            this.tpReportAB.Controls.Add(this.label2);
            this.tpReportAB.Controls.Add(this.dgReportA);
            this.tpReportAB.Controls.Add(this.btnPrintReportA);
            this.tpReportAB.Location = new System.Drawing.Point(0, 25);
            this.tpReportAB.Name = "tpReportAB";
            this.tpReportAB.Size = new System.Drawing.Size(768, 391);
            this.tpReportAB.TabIndex = 0;
            this.tpReportAB.Text = "Reports A and B";
            this.tpReportAB.Title = "Reports A and B";
            // 
            // txtRunRepB
            // 
            this.txtRunRepB.Enabled = false;
            this.txtRunRepB.Location = new System.Drawing.Point(87, 357);
            this.txtRunRepB.Name = "txtRunRepB";
            this.txtRunRepB.Size = new System.Drawing.Size(104, 21);
            this.txtRunRepB.TabIndex = 39;
            // 
            // txtRunRepA
            // 
            this.txtRunRepA.Enabled = false;
            this.txtRunRepA.Location = new System.Drawing.Point(87, 203);
            this.txtRunRepA.Name = "txtRunRepA";
            this.txtRunRepA.Size = new System.Drawing.Size(104, 21);
            this.txtRunRepA.TabIndex = 38;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(222, 207);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(150, 16);
            this.label7.TabIndex = 37;
            this.label7.Text = "Report A Period End Date:";
            // 
            // btnPrintReportb
            // 
            this.btnPrintReportb.Location = new System.Drawing.Point(480, 354);
            this.btnPrintReportb.Name = "btnPrintReportb";
            this.btnPrintReportb.Size = new System.Drawing.Size(93, 35);
            this.btnPrintReportb.TabIndex = 35;
            this.btnPrintReportb.Text = "Print Report B";
            this.btnPrintReportb.Click += new System.EventHandler(this.btnPrintReportb_Click);
            // 
            // btnExcelRepB
            // 
            this.btnExcelRepB.Enabled = false;
            this.btnExcelRepB.Location = new System.Drawing.Point(604, 354);
            this.btnExcelRepB.Name = "btnExcelRepB";
            this.btnExcelRepB.Size = new System.Drawing.Size(120, 34);
            this.btnExcelRepB.TabIndex = 34;
            this.btnExcelRepB.Text = "Export Report B to Excel Spreadsheet";
            this.btnExcelRepB.Click += new System.EventHandler(this.btnExcelRepB_Click);
            // 
            // btnExcelRepA
            // 
            this.btnExcelRepA.Enabled = false;
            this.btnExcelRepA.Location = new System.Drawing.Point(604, 196);
            this.btnExcelRepA.Name = "btnExcelRepA";
            this.btnExcelRepA.Size = new System.Drawing.Size(120, 36);
            this.btnExcelRepA.TabIndex = 33;
            this.btnExcelRepA.Text = "Export Report A to Excel Spreadsheet";
            this.btnExcelRepA.Click += new System.EventHandler(this.btnExcelRepA_Click);
            // 
            // txtEndPeriodB
            // 
            this.txtEndPeriodB.Enabled = false;
            this.txtEndPeriodB.Location = new System.Drawing.Point(373, 360);
            this.txtEndPeriodB.Name = "txtEndPeriodB";
            this.txtEndPeriodB.Size = new System.Drawing.Size(79, 21);
            this.txtEndPeriodB.TabIndex = 26;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(222, 364);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(150, 16);
            this.label5.TabIndex = 25;
            this.label5.Text = "Report B Period End Date:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(14, 356);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 28);
            this.label3.TabIndex = 23;
            this.label3.Text = "Report B Run Date:";
            // 
            // dgReportB
            // 
            this.dgReportB.CaptionText = "Report B: Rebates Due on New Business";
            this.dgReportB.DataMember = "";
            this.dgReportB.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgReportB.Location = new System.Drawing.Point(16, 239);
            this.dgReportB.Name = "dgReportB";
            this.dgReportB.ReadOnly = true;
            this.dgReportB.Size = new System.Drawing.Size(743, 108);
            this.dgReportB.TabIndex = 22;
            // 
            // txtEndPeriodA
            // 
            this.txtEndPeriodA.Enabled = false;
            this.txtEndPeriodA.Location = new System.Drawing.Point(373, 206);
            this.txtEndPeriodA.Name = "txtEndPeriodA";
            this.txtEndPeriodA.Size = new System.Drawing.Size(79, 21);
            this.txtEndPeriodA.TabIndex = 21;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(214, 206);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 0);
            this.label4.TabIndex = 20;
            this.label4.Text = "Report A Period End Date:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 199);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 28);
            this.label2.TabIndex = 19;
            this.label2.Text = "Report A Run Date:";
            // 
            // dgReportA
            // 
            this.dgReportA.CaptionText = "Report A: Rebates Due on Existing Business";
            this.dgReportA.DataMember = "";
            this.dgReportA.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgReportA.Location = new System.Drawing.Point(14, 11);
            this.dgReportA.Name = "dgReportA";
            this.dgReportA.ReadOnly = true;
            this.dgReportA.Size = new System.Drawing.Size(743, 173);
            this.dgReportA.TabIndex = 3;
            // 
            // btnPrintReportA
            // 
            this.btnPrintReportA.Location = new System.Drawing.Point(480, 197);
            this.btnPrintReportA.Name = "btnPrintReportA";
            this.btnPrintReportA.Size = new System.Drawing.Size(93, 35);
            this.btnPrintReportA.TabIndex = 32;
            this.btnPrintReportA.Text = "Print Report A";
            this.btnPrintReportA.Click += new System.EventHandler(this.btnPrintReportA_Click);
            // 
            // tpReportCD
            // 
            this.tpReportCD.Controls.Add(this.txtRunRepD);
            this.tpReportCD.Controls.Add(this.txtRunRepC);
            this.tpReportCD.Controls.Add(this.txtEndYearD);
            this.tpReportCD.Controls.Add(this.label11);
            this.tpReportCD.Controls.Add(this.btnExcelRepD);
            this.tpReportCD.Controls.Add(this.btnPrintReportD);
            this.tpReportCD.Controls.Add(this.txtEndPeriodD);
            this.tpReportCD.Controls.Add(this.label8);
            this.tpReportCD.Controls.Add(this.label10);
            this.tpReportCD.Controls.Add(this.btnExcelRepC);
            this.tpReportCD.Controls.Add(this.btnPrintReportC);
            this.tpReportCD.Controls.Add(this.txtEndPeriodC);
            this.tpReportCD.Controls.Add(this.label6);
            this.tpReportCD.Controls.Add(this.label9);
            this.tpReportCD.Controls.Add(this.dgReportD);
            this.tpReportCD.Controls.Add(this.dgReportC);
            this.tpReportCD.Location = new System.Drawing.Point(0, 25);
            this.tpReportCD.Name = "tpReportCD";
            this.tpReportCD.Selected = false;
            this.tpReportCD.Size = new System.Drawing.Size(768, 391);
            this.tpReportCD.TabIndex = 1;
            this.tpReportCD.Text = "Reports C and D";
            this.tpReportCD.Title = "Reports C and D";
            // 
            // txtRunRepD
            // 
            this.txtRunRepD.Enabled = false;
            this.txtRunRepD.Location = new System.Drawing.Point(80, 354);
            this.txtRunRepD.Name = "txtRunRepD";
            this.txtRunRepD.Size = new System.Drawing.Size(104, 21);
            this.txtRunRepD.TabIndex = 48;
            // 
            // txtRunRepC
            // 
            this.txtRunRepC.Enabled = false;
            this.txtRunRepC.Location = new System.Drawing.Point(80, 104);
            this.txtRunRepC.Name = "txtRunRepC";
            this.txtRunRepC.Size = new System.Drawing.Size(104, 21);
            this.txtRunRepC.TabIndex = 47;
            // 
            // txtEndYearD
            // 
            this.txtEndYearD.Location = new System.Drawing.Point(368, 368);
            this.txtEndYearD.Name = "txtEndYearD";
            this.txtEndYearD.Size = new System.Drawing.Size(79, 21);
            this.txtEndYearD.TabIndex = 46;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(222, 368);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(146, 16);
            this.label11.TabIndex = 45;
            this.label11.Text = "Report D Year End Date:";
            // 
            // btnExcelRepD
            // 
            this.btnExcelRepD.Location = new System.Drawing.Point(604, 344);
            this.btnExcelRepD.Name = "btnExcelRepD";
            this.btnExcelRepD.Size = new System.Drawing.Size(120, 40);
            this.btnExcelRepD.TabIndex = 44;
            this.btnExcelRepD.Text = "Export Report D to Excel Spreadsheet";
            this.btnExcelRepD.Click += new System.EventHandler(this.btnExcelRepD_Click);
            // 
            // btnPrintReportD
            // 
            this.btnPrintReportD.Location = new System.Drawing.Point(480, 344);
            this.btnPrintReportD.Name = "btnPrintReportD";
            this.btnPrintReportD.Size = new System.Drawing.Size(93, 35);
            this.btnPrintReportD.TabIndex = 43;
            this.btnPrintReportD.Text = "Print Report D";
            this.btnPrintReportD.Click += new System.EventHandler(this.btnPrintReportD_Click);
            // 
            // txtEndPeriodD
            // 
            this.txtEndPeriodD.Enabled = false;
            this.txtEndPeriodD.Location = new System.Drawing.Point(368, 344);
            this.txtEndPeriodD.Name = "txtEndPeriodD";
            this.txtEndPeriodD.Size = new System.Drawing.Size(79, 21);
            this.txtEndPeriodD.TabIndex = 42;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(222, 344);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(154, 16);
            this.label8.TabIndex = 41;
            this.label8.Text = "Report D Period End Date:";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(14, 352);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(60, 28);
            this.label10.TabIndex = 39;
            this.label10.Text = "Report D Run Date:";
            // 
            // btnExcelRepC
            // 
            this.btnExcelRepC.Location = new System.Drawing.Point(604, 104);
            this.btnExcelRepC.Name = "btnExcelRepC";
            this.btnExcelRepC.Size = new System.Drawing.Size(120, 40);
            this.btnExcelRepC.TabIndex = 38;
            this.btnExcelRepC.Text = "Export Report C to Excel Spreadsheet";
            this.btnExcelRepC.Click += new System.EventHandler(this.btnExcelRepC_Click);
            // 
            // btnPrintReportC
            // 
            this.btnPrintReportC.Location = new System.Drawing.Point(480, 104);
            this.btnPrintReportC.Name = "btnPrintReportC";
            this.btnPrintReportC.Size = new System.Drawing.Size(93, 35);
            this.btnPrintReportC.TabIndex = 37;
            this.btnPrintReportC.Text = "Print Report C";
            this.btnPrintReportC.Click += new System.EventHandler(this.btnPrintReportC_Click);
            // 
            // txtEndPeriodC
            // 
            this.txtEndPeriodC.Enabled = false;
            this.txtEndPeriodC.Location = new System.Drawing.Point(368, 112);
            this.txtEndPeriodC.Name = "txtEndPeriodC";
            this.txtEndPeriodC.Size = new System.Drawing.Size(79, 21);
            this.txtEndPeriodC.TabIndex = 36;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(222, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(154, 16);
            this.label6.TabIndex = 35;
            this.label6.Text = "Report C Period End Date:";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(16, 104);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 28);
            this.label9.TabIndex = 33;
            this.label9.Text = "Report C Run Date:";
            // 
            // dgReportD
            // 
            this.dgReportD.CaptionText = "Report D: Forecast to Actual since Last Year End";
            this.dgReportD.DataMember = "";
            this.dgReportD.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgReportD.Location = new System.Drawing.Point(16, 152);
            this.dgReportD.Name = "dgReportD";
            this.dgReportD.ReadOnly = true;
            this.dgReportD.Size = new System.Drawing.Size(743, 184);
            this.dgReportD.TabIndex = 32;
            // 
            // dgReportC
            // 
            this.dgReportC.CaptionText = "Report C: Forecast to Actual for Current Period End";
            this.dgReportC.DataMember = "";
            this.dgReportC.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgReportC.Location = new System.Drawing.Point(16, 14);
            this.dgReportC.Name = "dgReportC";
            this.dgReportC.ReadOnly = true;
            this.dgReportC.Size = new System.Drawing.Size(743, 82);
            this.dgReportC.TabIndex = 31;
            // 
            // btnPrintAll
            // 
            this.btnPrintAll.Location = new System.Drawing.Point(584, 8);
            this.btnPrintAll.Name = "btnPrintAll";
            this.btnPrintAll.Size = new System.Drawing.Size(55, 40);
            this.btnPrintAll.TabIndex = 1;
            this.btnPrintAll.Text = "Print All Reports";
            this.btnPrintAll.Visible = false;
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(645, 8);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(120, 40);
            this.button11.TabIndex = 18;
            this.button11.Text = "Export All Reports to Excel Spreadsheet";
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(8, 32);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(184, 16);
            this.label12.TabIndex = 33;
            this.label12.Text = "Select Period End  To View Report:";
            // 
            // drpPeriod
            // 
            this.drpPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPeriod.Location = new System.Drawing.Point(200, 32);
            this.drpPeriod.Name = "drpPeriod";
            this.drpPeriod.Size = new System.Drawing.Size(104, 21);
            this.drpPeriod.TabIndex = 32;
            this.drpPeriod.SelectedIndexChanged += new System.EventHandler(this.drpPeriod_SelectedIndexChanged);
            // 
            // txtPeriod
            // 
            this.txtPeriod.Enabled = false;
            this.txtPeriod.Location = new System.Drawing.Point(200, 8);
            this.txtPeriod.Name = "txtPeriod";
            this.txtPeriod.Size = new System.Drawing.Size(104, 20);
            this.txtPeriod.TabIndex = 34;
            // 
            // drpBranchNo
            // 
            this.drpBranchNo.FormattingEnabled = true;
            this.drpBranchNo.Location = new System.Drawing.Point(411, 27);
            this.drpBranchNo.Name = "drpBranchNo";
            this.drpBranchNo.Size = new System.Drawing.Size(103, 21);
            this.drpBranchNo.TabIndex = 35;
            this.drpBranchNo.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(326, 32);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(79, 13);
            this.label13.TabIndex = 36;
            this.label13.Text = "View Report by";
            // 
            // RebateReport
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.drpBranchNo);
            this.Controls.Add(this.txtPeriod);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.drpPeriod);
            this.Controls.Add(this.tcRebate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnPrintAll);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.button11);
            this.Name = "RebateReport";
            this.Text = "Rebate Forecasting Report";
            this.tcRebate.ResumeLayout(false);
            this.tpReportAB.ResumeLayout(false);
            this.tpReportAB.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgReportB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgReportA)).EndInit();
            this.tpReportCD.ResumeLayout(false);
            this.tpReportCD.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgReportD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgReportC)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void drpPeriod_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
                ////ISSUE 68880 rebate forecast error: SC 23/3/07. drpPeriod.SelectedIndex = 0 changed to Selectedindex > 0. 
                //if(staticLoaded && drpPeriod.SelectedIndex > 0 )
                //CR931 - IP - commented out above and replaced with below.
                if(staticLoaded && drpPeriod.SelectedIndex != 0 && drpPeriod.SelectedIndex != -1)
				{
					Wait();

					dgReportA.TableStyles.Clear();
					dgReportB.TableStyles.Clear();
					dgReportC.TableStyles.Clear();
					dgReportD.TableStyles.Clear();

                    int branchNo = Convert.ToInt32(drpBranchNo.SelectedValue);       // CR931 jec 04/04/08

                    // branchno will be -1 if Country selected, 0 if All Branches or a branch specific number
                    DataSet reports = AccountManager.GetRebateForecastReports((string)drpPeriod.SelectedItem, branchNo, out err);

					if(err.Length > 0)
						ShowError(err);
					else
					{
						foreach(DataTable dt in reports.Tables)
						{
							switch(dt.TableName)
							{
								case TN.ReportA:	
									//dgReportA.DataSource = dt;
									ProcessReportA(dt);
									break;
								case TN.ReportB:
									//dgReportB.DataSource = dt;
									ProcessReportB(dt);
									break;
								case TN.ReportC:
									//dgReportC.DataSource = dt;
									ProcessReportC(dt);
									break;
								case TN.ReportD:
									//dgReportD.DataSource = dt;
									ProcessReportD(dt);
									break;
								default:
									break;
							}
						}	
						
						txtEndPeriodA.Text = (string)drpPeriod.SelectedItem;
						txtEndPeriodB.Text = (string)drpPeriod.SelectedItem;
						txtEndPeriodC.Text = (string)drpPeriod.SelectedItem;
						txtEndPeriodD.Text = (string)drpPeriod.SelectedItem;

						foreach(DataRow row in periods.Tables[0].Rows)
						{
							if((string)row[CN.EndDate] == (string)drpPeriod.SelectedItem)
							{
								txtRunRepA.Text = (string)row[CN.RunDate];
								txtRunRepB.Text = (string)row[CN.RunDate];
								txtRunRepC.Text = (string)row[CN.RunDate];
								txtRunRepD.Text = (string)row[CN.RunDate];
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "drpPeriod_SelectedIndexChanged");
			}
			finally
			{
				StopWait();
			}			
		}

		private void ProcessReportA(DataTable dt)
		{
			dgReportA.DataSource = dt.DefaultView; 

			btnExcelRepA.Enabled = dt.Rows.Count > 0;
			int index = headings.IndexOf((string)drpPeriod.SelectedItem);

			if(dgReportA.TableStyles.Count==0)
			{	
				DataGridTableStyle tabStyle = new DataGridTableStyle();
				tabStyle.MappingName = dt.TableName;
				dgReportA.TableStyles.Add(tabStyle);

				tabStyle.GridColumnStyles[CN.ArrearsLevel].Width = 90;

				tabStyle.GridColumnStyles[CN.P1].Width = 80;
				tabStyle.GridColumnStyles[CN.P1].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P1].HeaderText = headings[index];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P1]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P2].Width = 80;
				tabStyle.GridColumnStyles[CN.P2].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P2].HeaderText = headings[index+1];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P2]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P3].Width = 80;
				tabStyle.GridColumnStyles[CN.P3].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P3].HeaderText = headings[index+2];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P3]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P4].Width = 80;
				tabStyle.GridColumnStyles[CN.P4].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P4].HeaderText = headings[index+3];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P4]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P5].Width = 80;
				tabStyle.GridColumnStyles[CN.P5].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P5].HeaderText = headings[index+4];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P5]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P6].Width = 80;
				tabStyle.GridColumnStyles[CN.P6].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P6].HeaderText = headings[index+5];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P6]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P7].Width = 80;
				tabStyle.GridColumnStyles[CN.P7].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P7].HeaderText = headings[index+6];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P7]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P8].Width = 80;
				tabStyle.GridColumnStyles[CN.P8].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P8].HeaderText = headings[index+7];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P8]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P9].Width = 80;
				tabStyle.GridColumnStyles[CN.P9].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P9].HeaderText = headings[index+8];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P9]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P10].Width = 80;
				tabStyle.GridColumnStyles[CN.P10].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P10].HeaderText = headings[index+9];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P10]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P11].Width = 80;
				tabStyle.GridColumnStyles[CN.P11].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P11].HeaderText = headings[index+10];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P11]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P12].Width = 80;
				tabStyle.GridColumnStyles[CN.P12].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P12].HeaderText = headings[index+11];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P12]).Format = DecimalPlaces;
			}
		}

		private void ProcessReportB(DataTable dt)
		{
			dgReportB.DataSource = dt.DefaultView; 

			btnExcelRepB.Enabled = dt.Rows.Count > 0;
			int index = headings.IndexOf((string)drpPeriod.SelectedItem);

			if(dgReportB.TableStyles.Count==0)
			{	
				DataGridTableStyle tabStyle = new DataGridTableStyle();
				tabStyle.MappingName = dt.TableName;
				dgReportB.TableStyles.Add(tabStyle);

				tabStyle.GridColumnStyles[CN.ArrearsLevel].Width = 90;

				tabStyle.GridColumnStyles[CN.P1].Width = 80;
				tabStyle.GridColumnStyles[CN.P1].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P1].HeaderText = headings[index];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P1]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P2].Width = 80;
				tabStyle.GridColumnStyles[CN.P2].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P2].HeaderText = headings[index+1];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P2]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P3].Width = 80;
				tabStyle.GridColumnStyles[CN.P3].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P3].HeaderText = headings[index+2];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P3]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P4].Width = 80;
				tabStyle.GridColumnStyles[CN.P4].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P4].HeaderText = headings[index+3];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P4]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P5].Width = 80;
				tabStyle.GridColumnStyles[CN.P5].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P5].HeaderText = headings[index+4];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P5]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P6].Width = 80;
				tabStyle.GridColumnStyles[CN.P6].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P6].HeaderText = headings[index+5];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P6]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P7].Width = 80;
				tabStyle.GridColumnStyles[CN.P7].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P7].HeaderText = headings[index+6];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P7]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P8].Width = 80;
				tabStyle.GridColumnStyles[CN.P8].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P8].HeaderText = headings[index+7];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P8]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P9].Width = 80;
				tabStyle.GridColumnStyles[CN.P9].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P9].HeaderText = headings[index+8];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P9]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P10].Width = 80;
				tabStyle.GridColumnStyles[CN.P10].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P10].HeaderText = headings[index+9];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P10]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P11].Width = 80;
				tabStyle.GridColumnStyles[CN.P11].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P11].HeaderText = headings[index+10];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P11]).Format = DecimalPlaces;
				tabStyle.GridColumnStyles[CN.P12].Width = 80;
				tabStyle.GridColumnStyles[CN.P12].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.P12].HeaderText = headings[index+11];
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.P12]).Format = DecimalPlaces;
			}
		}

		private void ProcessReportC(DataTable dt)
		{
			dgReportC.DataSource = dt.DefaultView; 
			
			btnExcelRepC.Enabled = dt.Rows.Count > 0;

			if(dgReportC.TableStyles.Count==0)
			{	
				DataGridTableStyle tabStyle = new DataGridTableStyle();
				tabStyle.MappingName = dt.TableName;
				dgReportC.TableStyles.Add(tabStyle);

				tabStyle.GridColumnStyles[CN.PeriodEnd].Width = 80;
				tabStyle.GridColumnStyles[CN.PeriodEnd].HeaderText = GetResource("T_PERIOD");
				
				tabStyle.GridColumnStyles[CN.Forecast].Width = 180;
				tabStyle.GridColumnStyles[CN.Forecast].HeaderText = GetResource("T_FORDECASTPERIOD");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Forecast]).Format = DecimalPlaces;

				tabStyle.GridColumnStyles[CN.Revised].Width = 140;
				tabStyle.GridColumnStyles[CN.Revised].HeaderText = GetResource("T_AGREEREVISED");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Revised]).Format = DecimalPlaces;

				tabStyle.GridColumnStyles[CN.DueDate].Width = 100;
				tabStyle.GridColumnStyles[CN.DueDate].HeaderText = GetResource("T_DUEDATE");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.DueDate]).Format = DecimalPlaces;

				tabStyle.GridColumnStyles[CN.AgreementDueDate].Width = 110;
				tabStyle.GridColumnStyles[CN.AgreementDueDate].HeaderText = GetResource("T_AGREEDUEDATE");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.AgreementDueDate]).Format = DecimalPlaces;
				
				tabStyle.GridColumnStyles[CN.Settled].Width = 140;
				tabStyle.GridColumnStyles[CN.Settled].HeaderText = GetResource("T_SETTLED");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Settled]).Format = DecimalPlaces;

				tabStyle.GridColumnStyles[CN.OutstBal].Width = 130;
				tabStyle.GridColumnStyles[CN.OutstBal].HeaderText = GetResource("T_OUTBAL");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.OutstBal]).Format = DecimalPlaces;

				tabStyle.GridColumnStyles[CN.DateLastChanged].Width = 110;
				tabStyle.GridColumnStyles[CN.DateLastChanged].HeaderText = GetResource("T_DATELASTCHANGED");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.DateLastChanged]).Format = DecimalPlaces;

				tabStyle.GridColumnStyles[CN.Threshold].Width = 80;
				tabStyle.GridColumnStyles[CN.Threshold].HeaderText = GetResource("T_THRESHOLD");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Threshold]).Format = DecimalPlaces;
				
				tabStyle.GridColumnStyles[CN.Unaccounted].Width = 110;
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Unaccounted]).Format = DecimalPlaces;

				tabStyle.GridColumnStyles[CN.Actual].Width = 80;
				tabStyle.GridColumnStyles[CN.Actual].HeaderText = GetResource("T_ACTUAL");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Actual]).Format = DecimalPlaces;
			}
		}

		private void ProcessReportD(DataTable dt)
		{
			dgReportD.DataSource = dt.DefaultView; 

			btnExcelRepD.Enabled = dt.Rows.Count > 0;

			if(dgReportD.TableStyles.Count==0)
			{	
				DataGridTableStyle tabStyle = new DataGridTableStyle();
				tabStyle.MappingName = dt.TableName;
				dgReportD.TableStyles.Add(tabStyle);

				tabStyle.GridColumnStyles[CN.PeriodEnd].Width = 80;
				tabStyle.GridColumnStyles[CN.PeriodEnd].HeaderText = GetResource("T_PERIOD");
				
				tabStyle.GridColumnStyles[CN.Forecast].Width = 160;
				tabStyle.GridColumnStyles[CN.Forecast].HeaderText = GetResource("T_FORDECASTYEAR");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Forecast]).Format = DecimalPlaces;
				
				tabStyle.GridColumnStyles[CN.Revised].Width = 140;
				tabStyle.GridColumnStyles[CN.Revised].HeaderText = GetResource("T_AGREEREVISED");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Revised]).Format = DecimalPlaces;

				tabStyle.GridColumnStyles[CN.DueDate].Width = 100;
				tabStyle.GridColumnStyles[CN.DueDate].HeaderText = GetResource("T_DUEDATE");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.DueDate]).Format = DecimalPlaces;

				tabStyle.GridColumnStyles[CN.AgreementDueDate].Width = 110;
				tabStyle.GridColumnStyles[CN.AgreementDueDate].HeaderText = GetResource("T_AGREEDUEDATE");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.AgreementDueDate]).Format = DecimalPlaces;
				
				tabStyle.GridColumnStyles[CN.Settled].Width = 140;
				tabStyle.GridColumnStyles[CN.Settled].HeaderText = GetResource("T_SETTLED");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Settled]).Format = DecimalPlaces;

				tabStyle.GridColumnStyles[CN.OutstBal].Width = 130;
				tabStyle.GridColumnStyles[CN.OutstBal].HeaderText = GetResource("T_OUTBAL");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.OutstBal]).Format = DecimalPlaces;

				tabStyle.GridColumnStyles[CN.DateLastChanged].Width = 110;
				tabStyle.GridColumnStyles[CN.DateLastChanged].HeaderText = GetResource("T_DATELASTCHANGED");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.DateLastChanged]).Format = DecimalPlaces;

				tabStyle.GridColumnStyles[CN.Threshold].Width = 80;
				tabStyle.GridColumnStyles[CN.Threshold].HeaderText = GetResource("T_THRESHOLD");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Threshold]).Format = DecimalPlaces;
				
				tabStyle.GridColumnStyles[CN.Unaccounted].Width = 110;
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Unaccounted]).Format = DecimalPlaces;

				tabStyle.GridColumnStyles[CN.Actual].Width = 80;
				tabStyle.GridColumnStyles[CN.Actual].HeaderText = GetResource("T_ACTUAL");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Actual]).Format = DecimalPlaces;
			}
		}

		private void Init()
		{
			dgReportA.DataSource = null;
			dgReportB.DataSource = null;
			dgReportC.DataSource = null;
			dgReportD.DataSource = null;

			txtEndPeriodA.Text = "";
			txtEndPeriodB.Text = "";
			txtEndPeriodC.Text = "";
			txtEndPeriodD.Text = "";

			txtRunRepA.Text = "";
			txtRunRepB.Text = "";
			txtRunRepC.Text = "";
			txtRunRepD.Text = "";

			drpPeriod.DataSource = null;

			periods = null;
			headings.Clear();
		}

		private void btnRun_Click(object sender, System.EventArgs e)
		{
			try
			{	
				Wait();
				string endPeriod = txtPeriod.Text;
				AccountManager.RunRebateForecastReports(txtPeriod.Text, out err);
				if(err.Length > 0)
					ShowError(err);
				else
				{
					Init();
					InitialiseStaticData();

					drpPeriod.SelectedIndex = drpPeriod.FindString(endPeriod);
				}
			}
			catch(Exception ex)
			{
				Catch(ex, "btnRun_Click");
			}
			finally
			{
				StopWait();
			}					
		}

		private void btnExcelRepA_Click(object sender, System.EventArgs e)
		{
			if(dgReportA.CurrentRowIndex >= 0)
			{
				DataView dv = (DataView)dgReportA.DataSource;
				WriteReportABToExcel(dv);
			}
		}

		private void WriteReportABToExcel(DataView dv)
		{
			/* save the current data grid contents to a CSV */
			string comma = ",";
			string path = "";
			string line = "";

			try
			{
				Wait();
				int index = headings.IndexOf((string)drpPeriod.SelectedItem);
					
				SaveFileDialog save = new SaveFileDialog();
				save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*" ;
				save.Title = "Save Rebate Forecast";
				save.CreatePrompt = true;

				if(save.ShowDialog() == DialogResult.OK)
				{
					path = save.FileName;
					FileInfo fi = new FileInfo(path);
					FileStream fs = fi.OpenWrite();

                    // - IP - CR931 - Add branch column if report is viewed by branch
                    int branchNo = Convert.ToInt32(drpBranchNo.SelectedValue);
                    if (branchNo >= 0)
                    {
                        line += CN.BranchNo + comma;
                    }

					line += CN.ArrearsLevel + comma;
					for(int i = index; i < 12 + index; i++)
					{
						line += headings[i] + comma;
					}

					line += Environment.NewLine + Environment.NewLine;	

					byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
					fs.Write(blob,0,blob.Length);
		
					foreach(DataRowView row in dv)
					{
                        //IP - CR931 - 07/04/07
                        if (branchNo >= 0)
                        {
                            line = (Convert.ToString(row[CN.BranchNo])) + comma;

                        }
                        else
                        {
                            line = null;
                        }
                        line += ((string)row[CN.ArrearsLevel]).Replace(",", ";") + comma +
							((decimal)row[CN.P1]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.P2]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.P3]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.P4]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.P5]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.P6]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.P7]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.P8]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.P9]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.P10]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.P11]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.P12]).ToString(DecimalPlaces).Replace(",","") + Environment.NewLine;	

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

		private void btnExcelRepB_Click(object sender, System.EventArgs e)
		{
			if(dgReportB.CurrentRowIndex >= 0)
			{
				DataView dv = (DataView)dgReportB.DataSource;
				WriteReportABToExcel(dv);
			}
		}

		private void WriteReportCDToExcel(DataView dv)
		{
			/* save the current data grid contents to a CSV */
			string comma = ",";
			string path = "";
			string line = "";

			try
			{
				Wait();
				int index = headings.IndexOf((string)drpPeriod.SelectedItem);
					
				SaveFileDialog save = new SaveFileDialog();
				save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*" ;
				save.Title = "Save Rebate Forecast";
				save.CreatePrompt = true;

				if(save.ShowDialog() == DialogResult.OK)
				{
					path = save.FileName;
					FileInfo fi = new FileInfo(path);
					FileStream fs = fi.OpenWrite();

                    // - IP - CR931 - Add branch column if report is viewed by branch
                    int branchNo = Convert.ToInt32(drpBranchNo.SelectedValue);
                    if (branchNo >= 0)
                    {
                        line += CN.BranchNo + comma;
                    }

                    line += GetResource("T_PERIOD") + comma +
						CN.Forecast + comma +
						GetResource("T_AGREEREVISED") + comma +
						GetResource("T_DUEDATE") + comma +
						GetResource("T_AGREEDUEDATE") + comma +
						GetResource("T_SETTLED") + comma +
						GetResource("T_OUTBAL") + comma +
						GetResource("T_DATELASTCHANGED") + comma +
						GetResource("T_THRESHOLD") + comma +
						CN.Unaccounted + comma +
						GetResource("T_ACTUAL") + Environment.NewLine + Environment.NewLine;	
					byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
					fs.Write(blob,0,blob.Length);
		
					foreach(DataRowView row in dv)
					{
                        //IP - CR931 - 07/04/07
                        if (branchNo >= 0)
                        {
                            line = (Convert.ToString(row[CN.BranchNo])) + comma;
                        }
                        else
                        {
                            line = null;
                        }

                        line += row[CN.PeriodEnd] + comma +
							((decimal)row[CN.Forecast]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.Revised]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.DueDate]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.AgreementDueDate]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.Settled]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.OutstBal]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.DateLastChanged]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.Threshold]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.Unaccounted]).ToString(DecimalPlaces).Replace(",","") + comma +
							((decimal)row[CN.Actual]).ToString(DecimalPlaces).Replace(",","") + Environment.NewLine;	

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

		private void btnExcelRepC_Click(object sender, System.EventArgs e)
		{
			if(dgReportC.CurrentRowIndex >= 0)
			{
				DataView dv = (DataView)dgReportC.DataSource;
				WriteReportCDToExcel(dv);
			}		
		}

		private void btnExcelRepD_Click(object sender, System.EventArgs e)
		{
			if(dgReportD.CurrentRowIndex >= 0)
			{
				DataView dv = (DataView)dgReportD.DataSource;
				WriteReportCDToExcel(dv);
			}		
		}

		private void btnPrintReportA_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				if(dgReportA.DataSource!=null)
					PrintRebateReport(dgReportA);
			}
			catch(Exception ex)
			{
				Catch(ex, "btnPrintReportA_Click");
			}	
			finally
			{
				StopWait();
			}
		}

		private void btnPrintReportb_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				if(dgReportB.DataSource!=null)
					PrintRebateReport(dgReportB);
			}
			catch(Exception ex)
			{
				Catch(ex, "btnPrintReportb_Click");
			}	
			finally
			{
				StopWait();
			}
		}

		private void btnPrintReportC_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				if(dgReportC.DataSource!=null)
					PrintRebateTab2(dgReportC);
			}
			catch(Exception ex)
			{
				Catch(ex, "btnPrintReportC_Click");
			}	
			finally
			{
				StopWait();
			}
		}

		private void btnPrintReportD_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				if(dgReportD.DataSource!=null)
					PrintRebateTab2(dgReportD);
			}
			catch(Exception ex)
			{
				Catch(ex, "btnPrintReportD_Click");
			}	
			finally
			{
				StopWait();
			}
		}

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //IP - CR931
        private void button11_Click(object sender, EventArgs e)
        {

            btnExcelRepA_Click(sender, e);
            btnExcelRepB_Click(sender, e);
            btnExcelRepC_Click(sender, e);
            btnExcelRepD_Click(sender, e);
        }
	}
}
