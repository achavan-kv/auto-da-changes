using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Web.Services.Protocols;
using STL.PL.WS9;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using System.Xml;
using STL.Common;
using STL.Common.Static;
using Crownwood.Magic.Menus;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;


namespace STL.PL
{
	/// <summary>
	/// A reporting screen to list data sent to the Fact 2000 financial interface.
	/// The End Of Day (EOD) runs are listed with a run number, run date and a
	/// pass or fail status. Clicking on an EOD run lists the monetary amounts
	/// sent to FACT 2000 for each branch and interface code. The interface codes
	/// are defined by FACT 2000 and correspond to the different financial 
	/// transaction types used in CoSACS.
	/// </summary>
	public class InterfaceBreakdown : CommonForm
	{
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private System.Windows.Forms.DataGrid dgInterface;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.DataGrid dgInterfaceSummary;
		string err = "";
		string eodInterface = "UPDSMRY";
		private System.Windows.Forms.TextBox txtTotal;
		private System.Windows.Forms.Label lTotal;
		private System.Windows.Forms.Label lDetails;
		private System.Windows.Forms.TextBox txtDetailsTotal;
		bool initialLoad = true;
		string factAcctNo = "";
		int branch = 0;

		string runFilter = "";
		private Crownwood.Magic.Controls.TabControl tcMain;
		private Crownwood.Magic.Controls.TabPage tpDetails;
		private Crownwood.Magic.Controls.TabPage tpBreakdown;
		private System.Windows.Forms.DataGrid dgBreakDown;
		private System.Windows.Forms.DataGrid dgTransactions;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtBreakDownTotal;
		public System.Windows.Forms.Button btnExcelBreakdown;
		public System.Windows.Forms.Button btnExcelDetails;
		public System.Windows.Forms.Button btnExcelTrans;
		string branchFilter = "";

		public InterfaceBreakdown(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
		}

		public InterfaceBreakdown(Form root, Form parent, bool load)
		{
			FormRoot = root;
			FormParent = parent;
			InitializeComponent();
			initialLoad = load;
			LoadResults();
			txtDetailsTotal.BackColor = SystemColors.Window;
			txtBreakDownTotal.BackColor = SystemColors.Window;
		}

		public InterfaceBreakdown(Form root, Form parent, int runno, int branchNo, bool load)
		{
			FormRoot = root;
			FormParent = parent;
			InitializeComponent();
			initialLoad = load;

			runFilter = CN.Runno + " = " + runno.ToString();
			if(branchNo > 0)
				branchFilter = CN.BranchNo + " = " + branchNo.ToString(); 

			LoadResults();
			lDetails.Visible = true;
			txtDetailsTotal.Visible = true;
			txtDetailsTotal.BackColor = SystemColors.Window;
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

		private void LoadResults()
		{
			try
			{
				DataSet ds = EodManager.GetInterfaceControl(eodInterface, "", true, out err);   //UAT1010 jec 09/07/10
				if(err.Length > 0)
					ShowError(err);
				else
				{
					if(ds != null)
					{
						dgInterface.DataSource = ds.Tables["Table1"].DefaultView; 

						if(runFilter.Length > 0)
							((DataView)dgInterface.DataSource).RowFilter = runFilter;
				
						if(dgInterface.TableStyles.Count==0)
						{
							DataGridTableStyle tabStyle = new DataGridTableStyle();
							tabStyle.MappingName = ds.Tables["Table1"].TableName;
							dgInterface.TableStyles.Add(tabStyle);

							tabStyle.GridColumnStyles[CN.Interface].Width = 90;
							tabStyle.GridColumnStyles[CN.Interface].HeaderText = GetResource("T_INTERFACE");

							tabStyle.GridColumnStyles[CN.Runno].Width = 75;
							tabStyle.GridColumnStyles[CN.Runno].HeaderText = GetResource("T_RUNNO");

							tabStyle.GridColumnStyles[CN.DateStart].Width = 70;
							tabStyle.GridColumnStyles[CN.DateStart].HeaderText = GetResource("T_DATESTART");

							tabStyle.GridColumnStyles[CN.DateFinish].Width = 130;
							tabStyle.GridColumnStyles[CN.DateFinish].HeaderText = GetResource("T_DATEFINISH");

							tabStyle.GridColumnStyles[CN.Result].Width = 100;
							tabStyle.GridColumnStyles[CN.Result].HeaderText = GetResource("T_RESULT");
						}
					}
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(InterfaceBreakdown));
			this.lDetails = new System.Windows.Forms.Label();
			this.txtDetailsTotal = new System.Windows.Forms.TextBox();
			this.lTotal = new System.Windows.Forms.Label();
			this.txtTotal = new System.Windows.Forms.TextBox();
			this.dgInterfaceSummary = new System.Windows.Forms.DataGrid();
			this.dgInterface = new System.Windows.Forms.DataGrid();
			this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
			this.tcMain = new Crownwood.Magic.Controls.TabControl();
			this.tpBreakdown = new Crownwood.Magic.Controls.TabPage();
			this.txtBreakDownTotal = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.dgBreakDown = new System.Windows.Forms.DataGrid();
			this.dgTransactions = new System.Windows.Forms.DataGrid();
			this.tpDetails = new Crownwood.Magic.Controls.TabPage();
			this.btnExcelTrans = new System.Windows.Forms.Button();
			this.btnExcelBreakdown = new System.Windows.Forms.Button();
			this.btnExcelDetails = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dgInterfaceSummary)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgInterface)).BeginInit();
			this.tcMain.SuspendLayout();
			this.tpBreakdown.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgBreakDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgTransactions)).BeginInit();
			this.tpDetails.SuspendLayout();
			this.SuspendLayout();
			// 
			// lDetails
			// 
			this.lDetails.Location = new System.Drawing.Point(80, 408);
			this.lDetails.Name = "lDetails";
			this.lDetails.Size = new System.Drawing.Size(72, 16);
			this.lDetails.TabIndex = 52;
			this.lDetails.Text = "Total Value:";
			// 
			// txtDetailsTotal
			// 
			this.txtDetailsTotal.Location = new System.Drawing.Point(152, 400);
			this.txtDetailsTotal.Name = "txtDetailsTotal";
			this.txtDetailsTotal.ReadOnly = true;
			this.txtDetailsTotal.Size = new System.Drawing.Size(80, 21);
			this.txtDetailsTotal.TabIndex = 51;
			this.txtDetailsTotal.Text = "";
			// 
			// lTotal
			// 
			this.lTotal.Location = new System.Drawing.Point(0, -32);
			this.lTotal.Name = "lTotal";
			this.lTotal.Size = new System.Drawing.Size(72, 19);
			this.lTotal.TabIndex = 4;
			this.lTotal.Text = "Total Value:";
			this.lTotal.Visible = false;
			// 
			// txtTotal
			// 
			this.txtTotal.Location = new System.Drawing.Point(72, -32);
			this.txtTotal.Name = "txtTotal";
			this.txtTotal.ReadOnly = true;
			this.txtTotal.Size = new System.Drawing.Size(80, 21);
			this.txtTotal.TabIndex = 3;
			this.txtTotal.Text = "";
			this.txtTotal.Visible = false;
			// 
			// dgInterfaceSummary
			// 
			this.dgInterfaceSummary.CaptionText = "Summary Update Details";
			this.dgInterfaceSummary.DataMember = "";
			this.dgInterfaceSummary.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgInterfaceSummary.Location = new System.Drawing.Point(80, 208);
			this.dgInterfaceSummary.Name = "dgInterfaceSummary";
			this.dgInterfaceSummary.ReadOnly = true;
			this.dgInterfaceSummary.Size = new System.Drawing.Size(560, 184);
			this.dgInterfaceSummary.TabIndex = 2;
			this.dgInterfaceSummary.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgInterfaceSummary_MouseUp);
			// 
			// dgInterface
			// 
			this.dgInterface.CaptionText = "Summary Update Interface Runs";
			this.dgInterface.DataMember = "";
			this.dgInterface.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgInterface.Location = new System.Drawing.Point(80, 8);
			this.dgInterface.Name = "dgInterface";
			this.dgInterface.ReadOnly = true;
			this.dgInterface.Size = new System.Drawing.Size(560, 184);
			this.dgInterface.TabIndex = 1;
			this.dgInterface.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgInterface_MouseUp);
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
			// 
			// tcMain
			// 
			this.tcMain.IDEPixelArea = true;
			this.tcMain.Location = new System.Drawing.Point(16, 16);
			this.tcMain.Name = "tcMain";
			this.tcMain.PositionTop = true;
			this.tcMain.SelectedIndex = 0;
			this.tcMain.SelectedTab = this.tpDetails;
			this.tcMain.Size = new System.Drawing.Size(736, 456);
			this.tcMain.TabIndex = 53;
			this.tcMain.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
																					 this.tpDetails,
																					 this.tpBreakdown});
			this.tcMain.SelectionChanged += new System.EventHandler(this.tcMain_SelectionChanged);
			// 
			// tpBreakdown
			// 
			this.tpBreakdown.Controls.AddRange(new System.Windows.Forms.Control[] {
																					  this.btnExcelBreakdown,
																					  this.btnExcelTrans,
																					  this.txtBreakDownTotal,
																					  this.label1,
																					  this.dgBreakDown,
																					  this.dgTransactions});
			this.tpBreakdown.Location = new System.Drawing.Point(0, 25);
			this.tpBreakdown.Name = "tpBreakdown";
			this.tpBreakdown.Selected = false;
			this.tpBreakdown.Size = new System.Drawing.Size(736, 431);
			this.tpBreakdown.TabIndex = 4;
			this.tpBreakdown.Title = "Summary Breakdown";
			this.tpBreakdown.Visible = false;
			// 
			// txtBreakDownTotal
			// 
			this.txtBreakDownTotal.Location = new System.Drawing.Point(152, 200);
			this.txtBreakDownTotal.Name = "txtBreakDownTotal";
			this.txtBreakDownTotal.ReadOnly = true;
			this.txtBreakDownTotal.Size = new System.Drawing.Size(80, 21);
			this.txtBreakDownTotal.TabIndex = 53;
			this.txtBreakDownTotal.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(80, 208);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 54;
			this.label1.Text = "Total Value:";
			// 
			// dgBreakDown
			// 
			this.dgBreakDown.CaptionText = "Summary Update Totals Breakdown";
			this.dgBreakDown.DataMember = "";
			this.dgBreakDown.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgBreakDown.Location = new System.Drawing.Point(80, 8);
			this.dgBreakDown.Name = "dgBreakDown";
			this.dgBreakDown.ReadOnly = true;
			this.dgBreakDown.Size = new System.Drawing.Size(560, 184);
			this.dgBreakDown.TabIndex = 3;
			this.dgBreakDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgBreakDown_MouseUp);
			// 
			// dgTransactions
			// 
			this.dgTransactions.CaptionText = "Summary Update Transactions";
			this.dgTransactions.DataMember = "";
			this.dgTransactions.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgTransactions.Location = new System.Drawing.Point(80, 232);
			this.dgTransactions.Name = "dgTransactions";
			this.dgTransactions.ReadOnly = true;
			this.dgTransactions.Size = new System.Drawing.Size(560, 184);
			this.dgTransactions.TabIndex = 4;
			// 
			// tpDetails
			// 
			this.tpDetails.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.btnExcelDetails,
																					this.dgInterface,
																					this.dgInterfaceSummary,
																					this.txtTotal,
																					this.txtDetailsTotal,
																					this.lDetails,
																					this.lTotal});
			this.tpDetails.Location = new System.Drawing.Point(0, 25);
			this.tpDetails.Name = "tpDetails";
			this.tpDetails.Size = new System.Drawing.Size(736, 431);
			this.tpDetails.TabIndex = 3;
			this.tpDetails.Title = "Summary Details";
			// 
			// btnExcelTrans
			// 
			this.btnExcelTrans.Enabled = false;
			this.btnExcelTrans.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnExcelTrans.Image")));
			this.btnExcelTrans.Location = new System.Drawing.Point(672, 312);
			this.btnExcelTrans.Name = "btnExcelTrans";
			this.btnExcelTrans.Size = new System.Drawing.Size(32, 32);
			this.btnExcelTrans.TabIndex = 55;
			this.btnExcelTrans.Click += new System.EventHandler(this.btnExcelTrans_Click);
			// 
			// btnExcelBreakdown
			// 
			this.btnExcelBreakdown.Enabled = false;
			this.btnExcelBreakdown.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnExcelBreakdown.Image")));
			this.btnExcelBreakdown.Location = new System.Drawing.Point(672, 88);
			this.btnExcelBreakdown.Name = "btnExcelBreakdown";
			this.btnExcelBreakdown.Size = new System.Drawing.Size(32, 32);
			this.btnExcelBreakdown.TabIndex = 56;
			this.btnExcelBreakdown.Click += new System.EventHandler(this.btnExcelBreakdown_Click);
			// 
			// btnExcelDetails
			// 
			this.btnExcelDetails.Enabled = false;
			this.btnExcelDetails.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnExcelDetails.Image")));
			this.btnExcelDetails.Location = new System.Drawing.Point(672, 288);
			this.btnExcelDetails.Name = "btnExcelDetails";
			this.btnExcelDetails.Size = new System.Drawing.Size(32, 32);
			this.btnExcelDetails.TabIndex = 56;
			this.btnExcelDetails.Click += new System.EventHandler(this.btnExcelDetails_Click);
			// 
			// InterfaceBreakdown
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(792, 477);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.tcMain});
			this.Name = "InterfaceBreakdown";
			this.Text = "Financial Interface Report";
			((System.ComponentModel.ISupportInitialize)(this.dgInterfaceSummary)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgInterface)).EndInit();
			this.tcMain.ResumeLayout(false);
			this.tpBreakdown.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgBreakDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgTransactions)).EndInit();
			this.tpDetails.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void dgInterface_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				decimal total = 0;
				int index = dgInterface.CurrentRowIndex;

				if(index >= 0)
				{
					DataView dv = (DataView)dgInterface.DataSource;
					int runno = (int)dv[index][CN.Runno];
					
					DataSet ds = EodManager.GetInterfaceFinancial(runno, out err);
					if(err.Length > 0)
						ShowError(err);
					else
					{
						if(ds != null)
						{
							btnExcelDetails.Enabled = ds.Tables["Table1"].Rows.Count > 0;
							dgInterfaceSummary.DataSource = ds.Tables["Table1"].DefaultView; 
				
							if(branchFilter.Length > 0)
								((DataView)dgInterfaceSummary.DataSource).RowFilter = branchFilter;

							if(dgInterfaceSummary.TableStyles.Count==0)
							{
								DataGridTableStyle tabStyle = new DataGridTableStyle();
								tabStyle.MappingName = ds.Tables["Table1"].TableName;
								dgInterfaceSummary.TableStyles.Add(tabStyle);

								tabStyle.GridColumnStyles[CN.InterfaceAccount].Width = 90;
								tabStyle.GridColumnStyles[CN.InterfaceAccount].HeaderText = GetResource("T_INTERFACE");

								tabStyle.GridColumnStyles[CN.TransValue].Width = 75;
								tabStyle.GridColumnStyles[CN.TransValue].HeaderText = GetResource("T_VALUE");
								((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TransValue]).Format = DecimalPlaces;

								tabStyle.GridColumnStyles[CN.BranchNo].Width = 70;
								tabStyle.GridColumnStyles[CN.BranchNo].HeaderText = GetResource("T_BRANCH");

								tabStyle.GridColumnStyles[CN.Runno].Width = 0;
							}

							foreach(DataTable dt in ds.Tables)
							{
								foreach(DataRow row in dt.Rows)
								{
									if(DBNull.Value!=row[CN.TransValue])
										total += ((decimal)row[CN.TransValue]);
						
									txtDetailsTotal.Text = total.ToString(DecimalPlaces);
								}
							}
						}					
					}
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void dgInterfaceSummary_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Right && initialLoad)
			{
				if(dgInterfaceSummary.CurrentRowIndex >= 0)
				{
					DataGrid ctl = (DataGrid)sender;							

					MenuCommand m1 = new MenuCommand(GetResource("P_VIEWVALUES"));

					m1.Click += new System.EventHandler(this.OnViewValues);

					PopupMenu popup = new PopupMenu(); 
					popup.MenuCommands.AddRange(new MenuCommand[] {m1});
					MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));					
				}
			}
		}

		private void OnViewValues(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				int index = dgInterfaceSummary.CurrentRowIndex;

				if(index >= 0)
				{
					LoadTotals(index);
					tcMain.SelectedTab = tpBreakdown;
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

		private void LoadTotals(int index)
		{
			try
			{
				Wait();
				int runno = 0;
				int branchNo = 0;
				decimal total = 0;

				DataView dv = (DataView)dgInterfaceSummary.DataSource;
				factAcctNo = (string)dv[index][CN.InterfaceAccount];
				runno = (int)dv[index][CN.Runno];
				branchNo = (short)dv[index][CN.BranchNo];

				DataSet ds = EodManager.GetInterfaceBreakdown(runno, branchNo, factAcctNo, out err);
				if(err.Length > 0)
					ShowError(err);
				else
				{
					if(ds != null)
					{
						btnExcelBreakdown.Enabled = ds.Tables["Table1"].Rows.Count > 0;
						dgBreakDown.DataSource = ds.Tables["Table1"].DefaultView; 
				
						if(dgBreakDown.TableStyles.Count==0)
						{
							DataGridTableStyle tabStyle = new DataGridTableStyle();
							tabStyle.MappingName = ds.Tables["Table1"].TableName;
							dgBreakDown.TableStyles.Add(tabStyle);

							tabStyle.GridColumnStyles[CN.EmployeeName].Width = 130;
							tabStyle.GridColumnStyles[CN.EmployeeName].HeaderText = GetResource("T_EMPEENO");

							tabStyle.GridColumnStyles[CN.Code].Width = 90;
							tabStyle.GridColumnStyles[CN.Code].HeaderText = GetResource("T_TYPE");

							tabStyle.GridColumnStyles[CN.TransValue].Width = 90;
							tabStyle.GridColumnStyles[CN.TransValue].HeaderText = GetResource("T_VALUE");
							((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TransValue]).Format = DecimalPlaces;

							tabStyle.GridColumnStyles[CN.Runno].Width = 0;
							tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 0;
						}

						foreach(DataTable dt in ds.Tables)
						{
							foreach(DataRow row in dt.Rows)
							{
								if(DBNull.Value!=row[CN.TransValue])
									total += ((decimal)row[CN.TransValue]);
							
								txtBreakDownTotal.Text = total.ToString(DecimalPlaces);
							}
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

		private void dgBreakDown_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				Wait();
				int index = dgBreakDown.CurrentRowIndex;
				if(index >= 0)
				{
					DataView dv = (DataView)dgBreakDown.DataSource;
					int runno = (int)dv[index][CN.Runno];
					int empeeno = (int)dv[index][CN.EmployeeNo];
					string code = (string)dv[index][CN.Code];
							
					DataSet ds = EodManager.GetInterfaceTransactions(runno, empeeno, code, factAcctNo, branch, out err);
					if(err.Length > 0)
						ShowError(err);
					else
					{
						if(ds != null)
						{
							btnExcelTrans.Enabled = ds.Tables["Table1"].Rows.Count > 0;
							dgTransactions.DataSource = ds.Tables["Table1"].DefaultView; 

							if(dgTransactions.TableStyles.Count==0)
							{
								DataGridTableStyle tabStyle = new DataGridTableStyle();
								tabStyle.MappingName = ds.Tables["Table1"].TableName;
								dgTransactions.TableStyles.Add(tabStyle);

								tabStyle.GridColumnStyles[CN.Reference].Width = 90;
								tabStyle.GridColumnStyles[CN.Reference].HeaderText = GetResource("T_REFERENCE");

								tabStyle.GridColumnStyles[CN.CodeDescript].Width = 90;
								tabStyle.GridColumnStyles[CN.CodeDescript].HeaderText = GetResource("T_PAYMETHOD");

								tabStyle.GridColumnStyles[CN.TransValue].Width = 75;
								tabStyle.GridColumnStyles[CN.TransValue].HeaderText = GetResource("T_VALUE");
								((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TransValue]).Format = DecimalPlaces;
							}
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

		private void tcMain_SelectionChanged(object sender, System.EventArgs e)
		{
			switch(tcMain.SelectedTab.Name)
			{
				case "tpDetails":
					dgBreakDown.DataSource = null;
					dgTransactions.DataSource = null;
					btnExcelBreakdown.Enabled = false;
					btnExcelTrans.Enabled = false;
					txtBreakDownTotal.Text = (0).ToString(DecimalPlaces);
					break;
				case "tpBreakdown":
					int index = dgInterfaceSummary.CurrentRowIndex;
					if(index >= 0)
						LoadTotals(index);
					break;
				default:
					break;
			}

		}

		private void btnExcelDetails_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				/* save the current data grid contents to a CSV */
				string comma = ",";
				string path = "";
				DataView dv = (DataView)dgInterfaceSummary.DataSource;

				SaveFileDialog save = new SaveFileDialog();
				save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*" ;
				save.Title = "Save Summary Details";
				save.CreatePrompt = true;

				if(save.ShowDialog() == DialogResult.OK)
				{
					path = save.FileName;
					FileInfo fi = new FileInfo(path);
					FileStream fs = fi.OpenWrite();
		
					string line = GetResource("T_INTERFACE") + comma +
						GetResource("T_VALUE") + comma +
						GetResource("T_BRANCH") + comma +
						Environment.NewLine + Environment.NewLine;	
					byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
					fs.Write(blob,0,blob.Length);
		
					foreach(DataRowView row in dv)
					{					
						line =	row[CN.InterfaceAccount] + comma +
							((decimal)row[CN.TransValue]).ToString(DecimalPlaces).Replace(",","") + comma +
							Convert.ToString(row[CN.BranchNo]) + comma +
							Environment.NewLine;	

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

		private void btnExcelBreakdown_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				/* save the current data grid contents to a CSV */
				string comma = ",";
				string path = "";
				DataView dv = (DataView)dgBreakDown.DataSource;

				SaveFileDialog save = new SaveFileDialog();
				save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*" ;
				save.Title = "Save Breakdown Details";
				save.CreatePrompt = true;

				if(save.ShowDialog() == DialogResult.OK)
				{
					path = save.FileName;
					FileInfo fi = new FileInfo(path);
					FileStream fs = fi.OpenWrite();
		
					string line = GetResource("T_EMPEENO") + comma +
						GetResource("T_TYPE") + comma +
						GetResource("T_VALUE") + comma +
						Environment.NewLine + Environment.NewLine;	
					byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
					fs.Write(blob,0,blob.Length);
		
					foreach(DataRowView row in dv)
					{					
						line =	row[CN.EmployeeName] + comma +
							Convert.ToString(row[CN.Code]) + comma +
							((decimal)row[CN.TransValue]).ToString(DecimalPlaces).Replace(",","") + comma +
							Environment.NewLine;	

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

		private void btnExcelTrans_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				/* save the current data grid contents to a CSV */
				string comma = ",";
				string path = "";
				DataView dv = (DataView)dgTransactions.DataSource;

				SaveFileDialog save = new SaveFileDialog();
				save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*" ;
				save.Title = "Save Transaction Details";
				save.CreatePrompt = true;

				if(save.ShowDialog() == DialogResult.OK)
				{
					path = save.FileName;
					FileInfo fi = new FileInfo(path);
					FileStream fs = fi.OpenWrite();
		
					string line = GetResource("T_REFERENCE") + comma +
						GetResource("T_PAYMETHOD") + comma +
						GetResource("T_VALUE") + comma +
						Environment.NewLine + Environment.NewLine;	
					byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
					fs.Write(blob,0,blob.Length);
		
					foreach(DataRowView row in dv)
					{					
						line =	row[CN.Reference] + comma +
							Convert.ToString(row[CN.CodeDescript]) + comma +
							((decimal)row[CN.TransValue]).ToString(DecimalPlaces).Replace(",","") + comma +
							Environment.NewLine;	

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
	}
}
