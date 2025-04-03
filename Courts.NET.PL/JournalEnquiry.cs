using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using STL.PL.WS5;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using System.Xml;
using STL.Common.Static;
using Crownwood.Magic.Menus;
using System.Collections.Specialized;
using System.Data;
using System.Web.Services.Protocols;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;


namespace STL.PL
{
	/// <summary>
	/// A reporting screen for financial transactions. The user may filter
	/// by date range, transaction reference number range and branch number.
	/// The report can be printed or saved to a CSV file that can be loaded
	/// into Excel.
	/// </summary>
	public class JournalEnquiry : CommonForm
	{
		//private Crownwood.Magic.Menus.MenuControl menuMain;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.DateTimePicker dtDateLast;
		private System.Windows.Forms.DateTimePicker dtDateFirst;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox drpBranch;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.GroupBox groupBox2;
		public System.Windows.Forms.Button btnPrint;
		private DataSet dropDownDS = null;
		private string err = "";
		private System.Windows.Forms.TextBox txtLastRef;
		private System.Windows.Forms.TextBox txtEmpNo;
		private System.Windows.Forms.TextBox txtFirstRef;
		private int combination = 0;
		private System.Windows.Forms.Button btnToggle;
		private System.Windows.Forms.DataGrid dgAccounts;
		private int branch = 0;
		private System.Windows.Forms.TextBox txtTotal;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox groupBox3;
		public System.Windows.Forms.Button btnExcel;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public JournalEnquiry(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
		}

		public JournalEnquiry(Form root, Form parent)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
			FormRoot = root;
			FormParent = parent;
			LoadStatic();

			dtDateFirst.Value = new DateTime(DateTime.Now.Year, 
												DateTime.Now.Month,
												DateTime.Now.Day, 0,0,0);

			dtDateLast.Value = new DateTime(DateTime.Now.Year, 
											DateTime.Now.Month,
											DateTime.Now.Day, 23,59,59);
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(JournalEnquiry));
			this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnExcel = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.txtTotal = new System.Windows.Forms.TextBox();
			this.btnToggle = new System.Windows.Forms.Button();
			this.btnPrint = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.dgAccounts = new System.Windows.Forms.DataGrid();
			this.btnLoad = new System.Windows.Forms.Button();
			this.drpBranch = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.txtLastRef = new System.Windows.Forms.TextBox();
			this.txtEmpNo = new System.Windows.Forms.TextBox();
			this.txtFirstRef = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.dtDateFirst = new System.Windows.Forms.DateTimePicker();
			this.dtDateLast = new System.Windows.Forms.DateTimePicker();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
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
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.btnExcel,
																					this.label7,
																					this.txtTotal,
																					this.btnToggle,
																					this.btnPrint,
																					this.groupBox2,
																					this.btnLoad,
																					this.drpBranch,
																					this.label6,
																					this.label5,
																					this.label4,
																					this.label3,
																					this.txtLastRef,
																					this.txtEmpNo,
																					this.txtFirstRef,
																					this.label1,
																					this.label2,
																					this.dtDateFirst,
																					this.dtDateLast,
																					this.groupBox3});
			this.groupBox1.Location = new System.Drawing.Point(8, 2);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(776, 472);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// btnExcel
			// 
			this.btnExcel.Enabled = false;
			this.btnExcel.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnExcel.Image")));
			this.btnExcel.Location = new System.Drawing.Point(704, 112);
			this.btnExcel.Name = "btnExcel";
			this.btnExcel.Size = new System.Drawing.Size(32, 32);
			this.btnExcel.TabIndex = 47;
			this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
			// 
			// label7
			// 
			this.label7.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.label7.Location = new System.Drawing.Point(416, 120);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(72, 16);
			this.label7.TabIndex = 45;
			this.label7.Text = "Total Value:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// txtTotal
			// 
			this.txtTotal.Location = new System.Drawing.Point(496, 120);
			this.txtTotal.Name = "txtTotal";
			this.txtTotal.ReadOnly = true;
			this.txtTotal.Size = new System.Drawing.Size(80, 20);
			this.txtTotal.TabIndex = 44;
			this.txtTotal.Text = "0";
			// 
			// btnToggle
			// 
			this.btnToggle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnToggle.Location = new System.Drawing.Point(136, 112);
			this.btnToggle.Name = "btnToggle";
			this.btnToggle.Size = new System.Drawing.Size(96, 23);
			this.btnToggle.TabIndex = 43;
			this.btnToggle.Tag = "Enable";
			this.btnToggle.Text = "Disable Dates";
			this.btnToggle.Click += new System.EventHandler(this.btnToggle_Click);
			// 
			// btnPrint
			// 
			this.btnPrint.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnPrint.Image")));
			this.btnPrint.Location = new System.Drawing.Point(656, 112);
			this.btnPrint.Name = "btnPrint";
			this.btnPrint.Size = new System.Drawing.Size(32, 32);
			this.btnPrint.TabIndex = 8;
			this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.dgAccounts});
			this.groupBox2.Location = new System.Drawing.Point(16, 152);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(744, 312);
			this.groupBox2.TabIndex = 42;
			this.groupBox2.TabStop = false;
			// 
			// dgAccounts
			// 
			this.dgAccounts.DataMember = "";
			this.dgAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgAccounts.Location = new System.Drawing.Point(24, 24);
			this.dgAccounts.Name = "dgAccounts";
			this.dgAccounts.ReadOnly = true;
			this.dgAccounts.Size = new System.Drawing.Size(704, 272);
			this.dgAccounts.TabIndex = 0;
			// 
			// btnLoad
			// 
			this.btnLoad.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnLoad.Image")));
			this.btnLoad.Location = new System.Drawing.Point(608, 112);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(32, 32);
			this.btnLoad.TabIndex = 7;
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// drpBranch
			// 
			this.drpBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpBranch.Items.AddRange(new object[] {
														   "900"});
			this.drpBranch.Location = new System.Drawing.Point(688, 72);
			this.drpBranch.Name = "drpBranch";
			this.drpBranch.Size = new System.Drawing.Size(72, 21);
			this.drpBranch.TabIndex = 6;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(384, 56);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(72, 16);
			this.label6.TabIndex = 39;
			this.label6.Text = "First Ref No:";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(488, 56);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(72, 16);
			this.label5.TabIndex = 38;
			this.label5.Text = "Last Ref No:";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(592, 56);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(80, 16);
			this.label4.TabIndex = 37;
			this.label4.Text = "Employee No:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(688, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 16);
			this.label3.TabIndex = 36;
			this.label3.Text = "Branch:";
			// 
			// txtLastRef
			// 
			this.txtLastRef.Location = new System.Drawing.Point(488, 72);
			this.txtLastRef.Name = "txtLastRef";
			this.txtLastRef.Size = new System.Drawing.Size(64, 20);
			this.txtLastRef.TabIndex = 4;
			this.txtLastRef.Text = "0";
			// 
			// txtEmpNo
			// 
			this.txtEmpNo.Location = new System.Drawing.Point(592, 72);
			this.txtEmpNo.Name = "txtEmpNo";
			this.txtEmpNo.Size = new System.Drawing.Size(64, 20);
			this.txtEmpNo.TabIndex = 5;
			this.txtEmpNo.Text = "0";
			// 
			// txtFirstRef
			// 
			this.txtFirstRef.Location = new System.Drawing.Point(384, 72);
			this.txtFirstRef.Name = "txtFirstRef";
			this.txtFirstRef.Size = new System.Drawing.Size(64, 20);
			this.txtFirstRef.TabIndex = 3;
			this.txtFirstRef.Text = "0";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 16);
			this.label1.TabIndex = 31;
			this.label1.Text = "First Date:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(200, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 16);
			this.label2.TabIndex = 30;
			this.label2.Text = "Last Date:";
			// 
			// dtDateFirst
			// 
			this.dtDateFirst.CustomFormat = "ddd dd MMM yyyy HH:mm";
			this.dtDateFirst.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtDateFirst.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.dtDateFirst.Location = new System.Drawing.Point(24, 72);
			this.dtDateFirst.Name = "dtDateFirst";
			this.dtDateFirst.Size = new System.Drawing.Size(144, 20);
			this.dtDateFirst.TabIndex = 1;
			this.dtDateFirst.Value = new System.DateTime(2002, 5, 8, 14, 5, 0, 0);
			// 
			// dtDateLast
			// 
			this.dtDateLast.CustomFormat = "ddd dd MMM yyyy HH:mm";
			this.dtDateLast.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtDateLast.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.dtDateLast.Location = new System.Drawing.Point(200, 72);
			this.dtDateLast.Name = "dtDateLast";
			this.dtDateLast.Size = new System.Drawing.Size(144, 20);
			this.dtDateLast.TabIndex = 2;
			this.dtDateLast.Value = new System.DateTime(2002, 5, 8, 14, 5, 0, 0);
			this.dtDateLast.ValueChanged += new System.EventHandler(this.dtDateLast_ValueChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Location = new System.Drawing.Point(16, 40);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(336, 104);
			this.groupBox3.TabIndex = 46;
			this.groupBox3.TabStop = false;
			// 
			// JournalEnquiry
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(792, 477);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox1});
			this.Name = "JournalEnquiry";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Transaction Journal Enquiry";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void LoadStatic()
		{		
			txtTotal.BackColor = SystemColors.Window;
			
			StringCollection branchNos = new StringCollection(); 	
			branchNos.Add("Branch");
			
			XmlUtilities xml = new XmlUtilities();
			XmlDocument dropDowns = new XmlDocument();
			dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
			
			if(StaticData.Tables[TN.BranchNumber] == null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.BranchNumber, null));

			if(dropDowns.DocumentElement.ChildNodes.Count>0)
			{
				dropDownDS = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out err);
				if(err.Length>0)
					ShowError(err);
				else
				{
					foreach(DataTable dt in dropDownDS.Tables)
						StaticData.Tables[dt.TableName] = dt;
				}
			}
		
			foreach(DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
			{
				branchNos.Add(Convert.ToString(row["branchno"]));
			}
		
			drpBranch.DataSource = branchNos;
		}

		private void menuExit_Click(object sender, System.EventArgs e)
		{
			CloseTab();
		}

		private void btnLoad_Click(object sender, System.EventArgs e)
		{
			DataSet ds = null;
			try
			{
				Wait();

				if(txtFirstRef.Text == "") 
					txtFirstRef.Text = "0";
				
				if(txtLastRef.Text == "") 
					txtLastRef.Text = "0";
				
				if(txtEmpNo.Text == "") 
					txtEmpNo.Text = "0";

				SearchCombination();
				
				if(drpBranch.SelectedIndex > 0)
					branch = Convert.ToInt32((string)drpBranch.SelectedItem);

				ds = PaymentManager.JournalEnquiryGet(dtDateFirst.Value, dtDateLast.Value,
					Convert.ToInt32(txtFirstRef.Text), Convert.ToInt32(txtLastRef.Text),
					Convert.ToInt32(txtEmpNo.Text), branch, combination, out err);
				
				if(err.Length > 0)
					ShowError(err);
				else
				{
					if(ds != null)
					{
						dgAccounts.DataSource = ds.Tables[0].DefaultView; 
						btnExcel.Enabled = ds.Tables[0].DefaultView.Count > 0;
					
						if(dgAccounts.TableStyles.Count==0)
						{
							DataGridTableStyle tabStyle = new DataGridTableStyle();
							tabStyle.MappingName = ds.Tables[0].TableName;
							dgAccounts.TableStyles.Add(tabStyle);

							tabStyle.GridColumnStyles[CN.BranchNo].Width = 60;
							tabStyle.GridColumnStyles[CN.BranchNo].HeaderText = GetResource("T_BRANCH");

							tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 90;
							tabStyle.GridColumnStyles[CN.EmployeeNo].HeaderText = GetResource("T_EMPEENO");

							tabStyle.GridColumnStyles[CN.DateTrans].Width = 90;
							tabStyle.GridColumnStyles[CN.DateTrans].HeaderText = GetResource("T_DATE");

							tabStyle.GridColumnStyles[CN.TransRefNo].Width = 90;
							tabStyle.GridColumnStyles[CN.TransRefNo].HeaderText = GetResource("T_TRANSREFNO");
						
							tabStyle.GridColumnStyles[CN.AcctNo].Width = 90;
							tabStyle.GridColumnStyles[CN.AcctNo].HeaderText = GetResource("T_ACCOUNTNO");
						
							tabStyle.GridColumnStyles[CN.TransTypeCode].Width = 60;
							tabStyle.GridColumnStyles[CN.TransTypeCode].HeaderText = GetResource("T_TYPE");

							tabStyle.GridColumnStyles[CN.TransValue].Width = 80;
							tabStyle.GridColumnStyles[CN.TransValue].HeaderText = GetResource("T_VALUE");
							tabStyle.GridColumnStyles[CN.TransValue].Alignment = HorizontalAlignment.Right;
							((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TransValue]).Format = DecimalPlaces;

							tabStyle.GridColumnStyles[CN.CodeDescript].Width = 90;
							tabStyle.GridColumnStyles[CN.CodeDescript].HeaderText = GetResource("T_PAYMETHOD");
						}
						
						CalculateTotal(ds);
						((MainForm)this.FormRoot).statusBar1.Text = ds.Tables[0].Rows.Count + " Row(s) returned.";
					}
					else
					{
						((MainForm)this.FormRoot).statusBar1.Text = "0 Row(s) returned.";
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

		private void SearchCombination()
		{
			if(dtDateFirst.Enabled && txtFirstRef.Text == "0" && txtEmpNo.Text == "0" && drpBranch.SelectedIndex == 0)
				combination = 1;

			if(Convert.ToInt32(txtFirstRef.Text) > 0 && !dtDateFirst.Enabled && txtEmpNo.Text == "0" && drpBranch.SelectedIndex == 0)
				combination = 2;

			if(Convert.ToInt32(txtEmpNo.Text) > 0 && !dtDateFirst.Enabled && txtFirstRef.Text == "0" && drpBranch.SelectedIndex == 0)
				combination = 3;

			if(drpBranch.SelectedIndex > 0 && !dtDateFirst.Enabled && txtFirstRef.Text == "0" && txtEmpNo.Text == "0")
				combination = 4;

			if(dtDateFirst.Enabled && Convert.ToInt32(txtFirstRef.Text) > 0 && txtEmpNo.Text == "0" && drpBranch.SelectedIndex == 0)
		        combination = 5;

			if(dtDateFirst.Enabled && txtFirstRef.Text == "0" && Convert.ToInt32(txtEmpNo.Text) > 0 && drpBranch.SelectedIndex == 0)
				combination = 6;

			if(dtDateFirst.Enabled && txtFirstRef.Text == "0" && txtEmpNo.Text == "0" && drpBranch.SelectedIndex > 0)
				combination = 7;
			
			if(!dtDateFirst.Enabled && Convert.ToInt32(txtFirstRef.Text) > 0 && Convert.ToInt32(txtEmpNo.Text) > 0 && drpBranch.SelectedIndex == 0)
		        combination = 8;

			if(!dtDateFirst.Enabled && Convert.ToInt32(txtFirstRef.Text) > 0 && txtEmpNo.Text == "0" && drpBranch.SelectedIndex > 0)
				combination = 9;

			if(dtDateFirst.Enabled && Convert.ToInt32(txtFirstRef.Text) > 0 && Convert.ToInt32(txtEmpNo.Text) > 0 && drpBranch.SelectedIndex == 0)
				combination = 10;

			if(dtDateFirst.Enabled && Convert.ToInt32(txtFirstRef.Text) > 0 && txtEmpNo.Text == "0" && drpBranch.SelectedIndex > 0)
				combination = 11;

			if(dtDateFirst.Enabled && txtFirstRef.Text == "0" && Convert.ToInt32(txtEmpNo.Text) > 0 && drpBranch.SelectedIndex > 0)
				combination = 12;

			if(!dtDateFirst.Enabled && Convert.ToInt32(txtFirstRef.Text) > 0 && Convert.ToInt32(txtEmpNo.Text) > 0 && drpBranch.SelectedIndex > 0)
				combination = 13;

			if(dtDateFirst.Enabled && Convert.ToInt32(txtFirstRef.Text) > 0 && Convert.ToInt32(txtEmpNo.Text) > 0 && drpBranch.SelectedIndex > 0)
				combination = 14;

			if(!dtDateFirst.Enabled && txtFirstRef.Text == "0" && Convert.ToInt32(txtEmpNo.Text) > 0 && drpBranch.SelectedIndex > 0)
				combination = 15;
		}

		private void btnToggle_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "btnToggle_Click";

				Wait();

				if((string)btnToggle.Tag == "Enable")
				{
					btnToggle.Tag = "Disable";
					btnToggle.Text = "Enable Dates";
					dtDateFirst.Enabled = false;
					dtDateLast.Enabled = false;
				}
				else
				{
					btnToggle.Tag = "Enable";
					btnToggle.Text = "Disable Dates";
					dtDateFirst.Enabled = true;
					dtDateLast.Enabled = true;
				}
				
				StopWait();
			}			
			catch(Exception ex)
			{
				Catch(ex, Function);
			}	
			finally
			{
				StopWait();
				Function = "End of btnToggle_Click";
			}
		}

		private void btnPrint_Click(object sender, System.EventArgs e)
		{
			PrintJournalEnquiry(((CommonForm)FormRoot).CreateBrowserArray(1)[0],dtDateFirst.Value, dtDateLast.Value,
					Convert.ToInt32(txtFirstRef.Text), Convert.ToInt32(txtLastRef.Text),
					Convert.ToInt32(txtEmpNo.Text), branch, combination);
		}

		private void CalculateTotal(DataSet ds)
		{
			decimal total = 0;

			foreach(DataRow r in ds.Tables[0].Rows)
			{
				total += ((decimal)r[CN.TransValue]);
			}

			txtTotal.Text = total.ToString(DecimalPlaces);
		}

		private void dtDateLast_ValueChanged(object sender, System.EventArgs e)
		{
		
		}

		private void btnExcel_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				/* save the current data grid contents to a CSV */
				string comma = ",";
				string path = "";

				if(dgAccounts.CurrentRowIndex >= 0)
				{
					DataView dv = (DataView)dgAccounts.DataSource;

					SaveFileDialog save = new SaveFileDialog();
					save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*" ;
					save.Title = "Save Journal Enquiry";
					save.CreatePrompt = true;

					if(save.ShowDialog() == DialogResult.OK)
					{
						path = save.FileName;
						FileInfo fi = new FileInfo(path);
						FileStream fs = fi.OpenWrite();
			
						string line = CN.BranchNo + comma +
							CN.EmployeeNo + comma +
							CN.DateTrans + comma +
							CN.TransRefNo + comma +
							CN.AcctNo + comma +
							CN.TransTypeCode + comma +
							CN.TransValue + comma +
							CN.CodeDescript + Environment.NewLine + Environment.NewLine;	
						byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
						fs.Write(blob,0,blob.Length);
			
						foreach(DataRowView row in dv)
						{					
							line =	row[CN.BranchNo] + comma +
								row[CN.EmployeeNo] + comma +
								Convert.ToString(row[CN.DateTrans]) + comma +
								row[CN.TransRefNo] + comma +
								"'"+row[CN.AcctNo]+"'" + comma +
								row[CN.TransTypeCode] + comma +
								((decimal)row[CN.TransValue]).ToString(DecimalPlaces).Replace(",","") + comma +
								row[CN.CodeDescript] + Environment.NewLine;	

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
	}
}
