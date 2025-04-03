using System;
using System.Configuration;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using STL.Common.Static;
using System.Data;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.FTransaction;
using System.Xml;
using Crownwood.Magic.Menus;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace STL.PL
{
	/// <summary>
	/// Displays agreement and order details based on various selection criteria.
	/// </summary>
	public class MonitorBookings : CommonForm
	{
		private string error = "";
		private bool _userChanged;
		private bool _allBranches = false;
		private bool _allEmployees = false;
		private DateTime _serverDate;

		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private System.Windows.Forms.ComboBox drpBranchNo;
		private System.Windows.Forms.Label lBranch;
		private System.Windows.Forms.DataGrid dgBookings;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.DateTimePicker dtDateTo;
		private System.Windows.Forms.DateTimePicker dtDateFrom;
		private System.Windows.Forms.Label lTo;
		private System.Windows.Forms.Label lFrom;
		private System.Windows.Forms.ToolTip ttbookings;
		private System.Windows.Forms.ComboBox drpSalesPerson;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox cbHp;
		private System.Windows.Forms.CheckBox cbCash;
		private System.Windows.Forms.CheckBox cbRf;
		private System.Windows.Forms.CheckBox cbPaidTaken;
		private System.Windows.Forms.CheckBox cbSecuritised;
		private System.Windows.Forms.CheckBox cbNonSecuritised;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox gbBookings;
		private System.Windows.Forms.CheckBox cbRollUpResults;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbEmpNo;
		public System.Windows.Forms.Button btnExcel;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.CheckBox cbLiveData;
		private System.ComponentModel.IContainer components;


		public MonitorBookings(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});

			HashMenus();
			ApplyRoleRestrictions();
		}

		public MonitorBookings(Form root, Form parent)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			FormRoot = root;
			FormParent = parent;

			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});

			HashMenus();
			ApplyRoleRestrictions();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MonitorBookings));
			this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
			this.gbBookings = new System.Windows.Forms.GroupBox();
			this.cbLiveData = new System.Windows.Forms.CheckBox();
			this.btnLoad = new System.Windows.Forms.Button();
			this.btnExcel = new System.Windows.Forms.Button();
			this.tbEmpNo = new System.Windows.Forms.TextBox();
			this.cbRollUpResults = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cbHp = new System.Windows.Forms.CheckBox();
			this.cbCash = new System.Windows.Forms.CheckBox();
			this.cbRf = new System.Windows.Forms.CheckBox();
			this.cbPaidTaken = new System.Windows.Forms.CheckBox();
			this.cbSecuritised = new System.Windows.Forms.CheckBox();
			this.cbNonSecuritised = new System.Windows.Forms.CheckBox();
			this.dtDateTo = new System.Windows.Forms.DateTimePicker();
			this.dtDateFrom = new System.Windows.Forms.DateTimePicker();
			this.lTo = new System.Windows.Forms.Label();
			this.lFrom = new System.Windows.Forms.Label();
			this.btnExit = new System.Windows.Forms.Button();
			this.btnClear = new System.Windows.Forms.Button();
			this.lBranch = new System.Windows.Forms.Label();
			this.drpBranchNo = new System.Windows.Forms.ComboBox();
			this.dgBookings = new System.Windows.Forms.DataGrid();
			this.drpSalesPerson = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.ttbookings = new System.Windows.Forms.ToolTip(this.components);
			this.gbBookings.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgBookings)).BeginInit();
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
			this.menuExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// gbBookings
			// 
			this.gbBookings.BackColor = System.Drawing.SystemColors.Control;
			this.gbBookings.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.cbLiveData,
																					 this.btnLoad,
																					 this.btnExcel,
																					 this.tbEmpNo,
																					 this.cbRollUpResults,
																					 this.groupBox1,
																					 this.dtDateTo,
																					 this.dtDateFrom,
																					 this.lTo,
																					 this.lFrom,
																					 this.btnExit,
																					 this.btnClear,
																					 this.lBranch,
																					 this.drpBranchNo,
																					 this.dgBookings,
																					 this.drpSalesPerson,
																					 this.label1,
																					 this.label2});
			this.gbBookings.Location = new System.Drawing.Point(8, 0);
			this.gbBookings.Name = "gbBookings";
			this.gbBookings.Size = new System.Drawing.Size(776, 480);
			this.gbBookings.TabIndex = 0;
			this.gbBookings.TabStop = false;
			// 
			// cbLiveData
			// 
			this.cbLiveData.Location = new System.Drawing.Point(616, 96);
			this.cbLiveData.Name = "cbLiveData";
			this.cbLiveData.Size = new System.Drawing.Size(144, 24);
			this.cbLiveData.TabIndex = 50;
			this.cbLiveData.Text = "Use LIVE database";
			// 
			// btnLoad
			// 
			this.btnLoad.CausesValidation = false;
			this.btnLoad.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnLoad.Image")));
			this.btnLoad.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnLoad.Location = new System.Drawing.Point(576, 32);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(32, 32);
			this.btnLoad.TabIndex = 49;
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// btnExcel
			// 
			this.btnExcel.Enabled = false;
			this.btnExcel.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnExcel.Image")));
			this.btnExcel.Location = new System.Drawing.Point(640, 32);
			this.btnExcel.Name = "btnExcel";
			this.btnExcel.Size = new System.Drawing.Size(32, 32);
			this.btnExcel.TabIndex = 48;
			this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
			// 
			// tbEmpNo
			// 
			this.tbEmpNo.Location = new System.Drawing.Point(280, 24);
			this.tbEmpNo.Name = "tbEmpNo";
			this.tbEmpNo.Size = new System.Drawing.Size(64, 20);
			this.tbEmpNo.TabIndex = 45;
			this.tbEmpNo.Text = "";
			// 
			// cbRollUpResults
			// 
			this.cbRollUpResults.Location = new System.Drawing.Point(616, 120);
			this.cbRollUpResults.Name = "cbRollUpResults";
			this.cbRollUpResults.TabIndex = 44;
			this.cbRollUpResults.Text = "Roll-up Results";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.cbHp,
																					this.cbCash,
																					this.cbRf,
																					this.cbPaidTaken,
																					this.cbSecuritised,
																					this.cbNonSecuritised});
			this.groupBox1.Location = new System.Drawing.Point(16, 88);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(560, 56);
			this.groupBox1.TabIndex = 43;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Accounts to Include";
			// 
			// cbHp
			// 
			this.cbHp.Location = new System.Drawing.Point(88, 24);
			this.cbHp.Name = "cbHp";
			this.cbHp.Size = new System.Drawing.Size(40, 16);
			this.cbHp.TabIndex = 47;
			this.cbHp.Text = "HP";
			// 
			// cbCash
			// 
			this.cbCash.Location = new System.Drawing.Point(16, 24);
			this.cbCash.Name = "cbCash";
			this.cbCash.Size = new System.Drawing.Size(56, 16);
			this.cbCash.TabIndex = 43;
			this.cbCash.Text = "Cash";
			// 
			// cbRf
			// 
			this.cbRf.Location = new System.Drawing.Point(152, 24);
			this.cbRf.Name = "cbRf";
			this.cbRf.Size = new System.Drawing.Size(40, 16);
			this.cbRf.TabIndex = 48;
			this.cbRf.Text = "RF";
			// 
			// cbPaidTaken
			// 
			this.cbPaidTaken.Location = new System.Drawing.Point(216, 24);
			this.cbPaidTaken.Name = "cbPaidTaken";
			this.cbPaidTaken.Size = new System.Drawing.Size(104, 16);
			this.cbPaidTaken.TabIndex = 46;
			this.cbPaidTaken.Text = "Paid and Taken";
			// 
			// cbSecuritised
			// 
			this.cbSecuritised.Location = new System.Drawing.Point(344, 24);
			this.cbSecuritised.Name = "cbSecuritised";
			this.cbSecuritised.Size = new System.Drawing.Size(80, 16);
			this.cbSecuritised.TabIndex = 44;
			this.cbSecuritised.Text = "Securitised";
			// 
			// cbNonSecuritised
			// 
			this.cbNonSecuritised.Location = new System.Drawing.Point(448, 24);
			this.cbNonSecuritised.Name = "cbNonSecuritised";
			this.cbNonSecuritised.Size = new System.Drawing.Size(104, 16);
			this.cbNonSecuritised.TabIndex = 45;
			this.cbNonSecuritised.Text = "Non Securitised";
			// 
			// dtDateTo
			// 
			this.dtDateTo.CustomFormat = "dd MMM yyyy";
			this.dtDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtDateTo.Location = new System.Drawing.Point(440, 56);
			this.dtDateTo.Name = "dtDateTo";
			this.dtDateTo.Size = new System.Drawing.Size(112, 20);
			this.dtDateTo.TabIndex = 39;
			this.dtDateTo.Tag = "";
			this.dtDateTo.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
			this.dtDateTo.ValueChanged += new System.EventHandler(this.dtDateTo_ValueChanged);
			// 
			// dtDateFrom
			// 
			this.dtDateFrom.CustomFormat = "dd MMM yyyy";
			this.dtDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtDateFrom.Location = new System.Drawing.Point(440, 24);
			this.dtDateFrom.Name = "dtDateFrom";
			this.dtDateFrom.Size = new System.Drawing.Size(112, 20);
			this.dtDateFrom.TabIndex = 38;
			this.dtDateFrom.Tag = "";
			this.dtDateFrom.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
			this.dtDateFrom.ValueChanged += new System.EventHandler(this.dtDateFrom_ValueChanged);
			// 
			// lTo
			// 
			this.lTo.Location = new System.Drawing.Point(352, 56);
			this.lTo.Name = "lTo";
			this.lTo.Size = new System.Drawing.Size(72, 16);
			this.lTo.TabIndex = 37;
			this.lTo.Text = "End Date";
			this.lTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lFrom
			// 
			this.lFrom.Location = new System.Drawing.Point(360, 24);
			this.lFrom.Name = "lFrom";
			this.lFrom.Size = new System.Drawing.Size(64, 16);
			this.lFrom.TabIndex = 36;
			this.lFrom.Text = "Start Date";
			this.lFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnExit
			// 
			this.btnExit.Location = new System.Drawing.Point(696, 56);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(56, 24);
			this.btnExit.TabIndex = 7;
			this.btnExit.Text = "Exit";
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(696, 24);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(56, 24);
			this.btnClear.TabIndex = 6;
			this.btnClear.Text = "Clear";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// lBranch
			// 
			this.lBranch.Location = new System.Drawing.Point(16, 24);
			this.lBranch.Name = "lBranch";
			this.lBranch.Size = new System.Drawing.Size(64, 16);
			this.lBranch.TabIndex = 2;
			this.lBranch.Text = "Branch No";
			this.lBranch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// drpBranchNo
			// 
			this.drpBranchNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpBranchNo.Location = new System.Drawing.Point(88, 24);
			this.drpBranchNo.Name = "drpBranchNo";
			this.drpBranchNo.Size = new System.Drawing.Size(72, 21);
			this.drpBranchNo.TabIndex = 0;
			this.drpBranchNo.SelectionChangeCommitted += new System.EventHandler(this.drpBranchNo_SelectionChangeCommitted);
			// 
			// dgBookings
			// 
			this.dgBookings.DataMember = "";
			this.dgBookings.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgBookings.Location = new System.Drawing.Point(8, 152);
			this.dgBookings.Name = "dgBookings";
			this.dgBookings.ReadOnly = true;
			this.dgBookings.Size = new System.Drawing.Size(760, 312);
			this.dgBookings.TabIndex = 0;
			this.dgBookings.TabStop = false;
			// 
			// drpSalesPerson
			// 
			this.drpSalesPerson.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpSalesPerson.Location = new System.Drawing.Point(88, 56);
			this.drpSalesPerson.Name = "drpSalesPerson";
			this.drpSalesPerson.Size = new System.Drawing.Size(256, 21);
			this.drpSalesPerson.TabIndex = 0;
			this.drpSalesPerson.SelectionChangeCommitted += new System.EventHandler(this.drpSalesPerson_SelectionChangeCommitted);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Salesperson";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(200, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Employee No";
			// 
			// MonitorBookings
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(792, 477);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.gbBookings});
			this.Name = "MonitorBookings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Monitor Bookings";
			this.Load += new System.EventHandler(this.MonitorBookings_Load);
			this.gbBookings.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgBookings)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		//
		// Local Methods
		//
		private void HashMenus()
		{
			dynamicMenus = new Hashtable();
			dynamicMenus[this.Name+":cbLiveData"] = this.cbLiveData;
		}

		private void ResetScreen(bool setDefault)
		{
			// Set the screen for initial entry

			// Disable change events before clearing all controls
			this._userChanged = false;

			// Preserve search fields
			string specificEmpNo = tbEmpNo.Text;
			int selectedBranch = this.drpBranchNo.SelectedIndex;
			int selectedSalesperson = this.drpSalesPerson.SelectedIndex;						
			DateTime dateFrom = this.dtDateFrom.Value;
			DateTime dateTo = this.dtDateTo.Value;
			bool cbCashChecked = cbCash.Checked;
			bool cbHpChecked = cbHp.Checked;
			bool cbNonSecuritisedChecked = cbNonSecuritised.Checked;
			bool cbPaidTakenChecked = cbPaidTaken.Checked;
			bool cbRfChecked = cbRf.Checked;
			bool cbSecuritisedChecked = cbSecuritised.Checked;
			bool cbRollUpResultsChecked = cbRollUpResults.Checked;
			bool cbLiveDataChecked = cbLiveData.Checked;

			// Clear all controls
			ClearControls(this.Controls);
			this.dgBookings.CaptionText = "";

			if (setDefault)
			{
				// Initial custom settings
				// Set to this branch for the last 24 hours
				tbEmpNo.Text = string.Empty;
				int i = this.drpBranchNo.FindStringExact(Config.BranchCode);
				this.drpBranchNo.SelectedIndex = i >= 0 ? i : 0;
				i = this.drpSalesPerson.FindStringExact(Credential.UserId.ToString());
				this.drpSalesPerson.SelectedIndex = i >= 0 ? i : 0;					
				this.dtDateFrom.Value = _serverDate.AddDays(-1);
				this.dtDateTo.Value = _serverDate;
				cbCash.Checked = true;
				cbHp.Checked = true;
				cbNonSecuritised.Checked = true;
				cbPaidTaken.Checked = true;
				cbRf.Checked = true;
				cbSecuritised.Checked = true;
				cbRollUpResults.Checked = false;
			}
			else
			{
				// Restore search fields
				tbEmpNo.Text = specificEmpNo;
				this.drpBranchNo.SelectedIndex = selectedBranch;
				this.drpSalesPerson.SelectedIndex = selectedSalesperson;					
				this.dtDateFrom.Value = dateFrom;
				this.dtDateTo.Value = dateTo;
				cbCash.Checked = cbCashChecked;
				cbHp.Checked = cbHpChecked;
				cbNonSecuritised.Checked = cbNonSecuritisedChecked;
				cbPaidTaken.Checked = cbPaidTakenChecked;
				cbRf.Checked = cbRfChecked;
				cbSecuritised.Checked = cbSecuritisedChecked;
				cbRollUpResults.Checked = cbRollUpResultsChecked;
				cbLiveData.Checked = cbLiveDataChecked;
			}

			// Enable the search fields and disable the bookings list
			this.drpBranchNo.Enabled = true;
			this.drpSalesPerson.Enabled = true;
			this.dtDateFrom.Enabled = true;
			this.dtDateTo.Enabled = true;
			((MainForm)this.FormRoot).statusBar1.Text = "";

			// Enable change events
			this._userChanged = true;

		}  // End of ResetScreen

		private void LoadStaticData()
		{
			this._serverDate = StaticDataManager.GetServerDate();
			this.dtDateFrom.MaxDate = this._serverDate;
			this.dtDateTo.MaxDate = this._serverDate;

			XmlUtilities xml = new XmlUtilities();
			XmlDocument dropDowns = new XmlDocument();
			dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
				
			if(StaticData.Tables[TN.BranchNumber]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.BranchNumber, null));
	
			//We won't use this data but we will create a new DataTable using its structure
			//later on.
			if(StaticData.Tables[TN.SalesStaff]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.SalesStaff, new string[] {Config.BranchCode, "S"}));
			
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

			//Now customise the dropdowns, the Branch dropdown needs an 'ALL' type
			//entry added..
			StringCollection branchNos = new StringCollection(); 	
			branchNos.Add("ALL");

			foreach(DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
			{
				branchNos.Add(Convert.ToString(row[CN.BranchNo]));
			}

			drpBranchNo.DataSource = branchNos;
			drpBranchNo.Text = Config.BranchCode;
		
			PopulateSalespersonDropDown();
		}

		private void PopulateSalespersonDropDown() 
		{
			XmlUtilities xml = new XmlUtilities();
			XmlDocument dropDowns = new XmlDocument();
			
			//Populate our list of Sales Staff - using a similar method to
			//the static table approach - except we have specific parameters that
			//we want to pass to the Web Method that retrieves the data.

			//Start with a fresh empty list of tables that we want to populate
			dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

			//Request TN.SalesStaff type data (with different parameters to standard)
			string passBranchNo = this.drpBranchNo.SelectedValue.ToString();
			if (passBranchNo == (string)STL.Common.Static.Messages.List["L_ALL"])
			{
				passBranchNo = "0";
			}
			dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.SalesStaff, new string[] {passBranchNo, "S"}));
            
			DataTable dtEmpNames = ((DataTable)StaticData.Tables[TN.SalesStaff]).Clone();
            
			DataRow newRow = dtEmpNames.NewRow();
			newRow[CN.BranchNo] = 0;
			newRow[CN.EmployeeNo] = 0;
			newRow[CN.EmployeeName] = STL.Common.Static.Messages.List["L_ALL"];
			dtEmpNames.Rows.Add(newRow);

			if (dropDowns.DocumentElement.ChildNodes.Count > 0)
			{
				DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out error);
				if (error.Length > 0)
					ShowError(error);
				else
				{
					foreach (DataTable dt in ds.Tables)
					{
						if (dt.TableName == TN.SalesStaff)//Only expecting this Table 
						{
							foreach(DataRow row in (dt.Rows))
							{
								newRow = dtEmpNames.NewRow();
								newRow[CN.BranchNo] = row[CN.BranchNo];
								newRow[CN.EmployeeNo] = row[CN.EmployeeNo];
								newRow[CN.EmployeeName] = row[CN.EmployeeName];
								dtEmpNames.Rows.Add(newRow);
							}
							break;
						}
					}
				}
			}

			drpSalesPerson.DataSource = dtEmpNames;
			//CN.EmployeeName contains "EmpeeName" but stored proc uses "empeename" so hard coded here;
            drpSalesPerson.DisplayMember = CN.EmployeeName;
			//CN.EmployeeNo contains "EmpeeNo" but stored proc uses "empeeno" so hard coded here;
			drpSalesPerson.ValueMember = "empeeno";
			drpSalesPerson.SelectedValue = Credential.UserId.ToString();
			if (drpSalesPerson.Items.Count > 0 && drpSalesPerson.SelectedIndex < 0) 
			{
				drpSalesPerson.SelectedIndex = 0;
			}
		}

		private bool LoadBookings()
		{
			bool bookingsFound = false;
			string statusText = GetResource("M_BOOKINGSZERO");
			DataSet bookingsSet = null;
			DataView bookingsListView = null;
			//Force the grid style to be recreated everytime since column width of
			//Value field changes depending on Rollup checkbox
			dgBookings.TableStyles.Clear();


			// Abort if the Start Date is after the End Date
			if (this.dtDateFrom.Value > this.dtDateTo.Value)
			{
				ShowInfo("M_STARTDATEAFTEREND");
				return false;
			}

			//Error if non-numeric data entered in employee number field..
			if (tbEmpNo.Text.Length > 0) 
			{
				if(!IsNumeric(tbEmpNo.Text) || int.Parse(tbEmpNo.Text) <= 0)
				{
					ShowInfo("M_INVALIDEMPNO");
					return false;
				}
			}

			((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_LOADINGDATA");
			string passBranchNo = this.drpBranchNo.SelectedValue.ToString();
			string passEmpNo = this.drpSalesPerson.SelectedValue.ToString();
			//Override drop down salesperson selection if a specific
			//employee number has been entered..
			if (tbEmpNo.Text.Length > 0)
			{
				passEmpNo = tbEmpNo.Text;
			}
			_allBranches = (passBranchNo == (string)STL.Common.Static.Messages.List["L_ALL"]);
			_allEmployees = (passEmpNo == "0");
            
			//"0" signifies 'all branches' to the stored procedure
			if (_allBranches)
			{
				passBranchNo = "0";
			}

			int includeCash = cbCash.Checked ? 1 : 0;
			int includeHp = cbHp.Checked ? 1 : 0;
			int includeNonSec = cbNonSecuritised.Checked ? 1 : 0;
			int includePaidTaken = cbPaidTaken.Checked ? 1 : 0;
			int includeRf = cbRf.Checked ? 1 : 0;
			int includeSec = cbSecuritised.Checked ? 1 : 0;
			int rollUpResults = cbRollUpResults.Checked ? 1 : 0;  
			int liveDatabase = cbLiveData.Checked ? 1 : 0;  
          
			//GAJ 04/08/05 - End Date needs to have Hours:Min:Sec set to 23:59:59
			DateTime adjustedDateTo = dtDateTo.Value.AddDays(1).AddSeconds(-1);

			bookingsSet = AccountManager.GetBookings(
				passBranchNo,
				passEmpNo,
				this.dtDateFrom.Value,
				adjustedDateTo,
				includeCash,
				includeHp,
				includeNonSec,
				includePaidTaken,
				includeRf,
				includeSec,
				rollUpResults,
				liveDatabase,
				out error);

			if(error.Length > 0)
			{
				ShowError(error);
				return false;
			}

			// Caption for live DB or reporting DB
			if (cbLiveData.Checked)
				this.dgBookings.CaptionText = GetResource("T_REPORTDBLIVE");
			else
				this.dgBookings.CaptionText = GetResource("T_REPORTDBREPORTING");

			// List the report results
			foreach (DataTable bookingsDetails in bookingsSet.Tables)
			{
				if (bookingsDetails.TableName == TN.MonitorBookings)
				{
					// Display list of bookingss
					bookingsFound = (bookingsDetails.Rows.Count > 0);
					statusText = bookingsDetails.Rows.Count + GetResource("M_BOOKINGSLISTED");
					btnExcel.Enabled = bookingsFound;

					bookingsListView = new DataView(bookingsDetails);
					bookingsListView.AllowNew = false;
					dgBookings.DataSource = bookingsListView;
					
					if (dgBookings.TableStyles.Count == 0)
					{
						DataGridTableStyle tabStyle = new DataGridTableStyle();
						tabStyle.MappingName = bookingsListView.Table.TableName;

						dgBookings.TableStyles.Clear();
						dgBookings.TableStyles.Add(tabStyle);
						dgBookings.DataSource = bookingsListView;

						// Displayed columns
						if (_allBranches)
						{
							tabStyle.GridColumnStyles[CN.BranchName].Width = 150;
							tabStyle.GridColumnStyles[CN.BranchName].ReadOnly = true;
							tabStyle.GridColumnStyles[CN.BranchName].HeaderText = GetResource("T_BRANCH");
						}
						else 
						{
							tabStyle.GridColumnStyles[CN.BranchName].Width = 0;
						}

						// 67915 RD 15/03/06 Added to split 'Salesperson" into two columns: "Salesperson ID" and "Salesperson Name"
						if (_allEmployees)
						{
							tabStyle.GridColumnStyles[CN.EmployeeName].Width = 150;
							tabStyle.GridColumnStyles[CN.EmployeeName].ReadOnly = true;
							tabStyle.GridColumnStyles[CN.EmployeeName].HeaderText = GetResource("T_BOOKINGSSALESPERSON");

							tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 80;
							tabStyle.GridColumnStyles[CN.EmployeeNo].ReadOnly = true;
							tabStyle.GridColumnStyles[CN.EmployeeNo].HeaderText = GetResource("T_BOOKINGSSALESPERSONID");
						}
						else 
						{
							tabStyle.GridColumnStyles[CN.EmployeeName].Width = 0;
							tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 0;
						}

						tabStyle.GridColumnStyles[CN.AcctType].Width = 60;
						tabStyle.GridColumnStyles[CN.AcctType].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.AcctType].HeaderText = GetResource("T_ACCOUNTTYPE");
						tabStyle.GridColumnStyles[CN.AcctType].Alignment = HorizontalAlignment.Center;

						if (cbRollUpResults.Checked)
						{
							tabStyle.GridColumnStyles[CN.Value].Width = 110;
						}
						else 
						{
							tabStyle.GridColumnStyles[CN.Value].Width = 70;
						}
						tabStyle.GridColumnStyles[CN.Value].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.Value].HeaderText = GetResource("T_VALUE"); 
						tabStyle.GridColumnStyles[CN.Value].Alignment = HorizontalAlignment.Right;
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Value]).Format = DecimalPlaces;
                        
						tabStyle.GridColumnStyles[CN.Cancelled].Width = 70;
						tabStyle.GridColumnStyles[CN.Cancelled].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.Cancelled].HeaderText = GetResource("T_BOOKINGSCANCELLED");
						tabStyle.GridColumnStyles[CN.Cancelled].Alignment = HorizontalAlignment.Right;
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Cancelled]).Format = DecimalPlaces;

						tabStyle.GridColumnStyles[CN.Revised].Width = 70;
						tabStyle.GridColumnStyles[CN.Revised].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.Revised].Alignment = HorizontalAlignment.Right;
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Revised]).Format = DecimalPlaces;

						tabStyle.GridColumnStyles[CN.SalesTax].Width = 70;
						tabStyle.GridColumnStyles[CN.SalesTax].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.SalesTax].HeaderText = GetResource("T_BOOKINGSSALESTAX");
						tabStyle.GridColumnStyles[CN.SalesTax].Alignment = HorizontalAlignment.Right;
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.SalesTax]).Format = DecimalPlaces;

						tabStyle.GridColumnStyles[CN.Balance].Width = 70;
						tabStyle.GridColumnStyles[CN.Balance].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.Balance].HeaderText = GetResource("T_BOOKINGSBALANCE");
						tabStyle.GridColumnStyles[CN.Balance].Alignment = HorizontalAlignment.Right;
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Balance]).Format = DecimalPlaces;

						tabStyle.GridColumnStyles[CN.SuperShield].Width = 70;
						tabStyle.GridColumnStyles[CN.SuperShield].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.SuperShield].HeaderText = GetResource("T_BOOKINGSSUPERSHIELD");
						tabStyle.GridColumnStyles[CN.SuperShield].Alignment = HorizontalAlignment.Right;
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.SuperShield]).Format = DecimalPlaces;

						tabStyle.GridColumnStyles[CN.Discount].Width = 70;
						tabStyle.GridColumnStyles[CN.Discount].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.Discount].HeaderText = GetResource("T_DISCOUNT");
						tabStyle.GridColumnStyles[CN.Discount].Alignment = HorizontalAlignment.Right;
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Discount]).Format = DecimalPlaces;
					}
				}
				if (rollUpResults == 1) 
				{
					DataRow row;
					for (int i= bookingsDetails.Rows.Count - 1; i>=0; i--)
					{
						row = bookingsDetails.Rows[i];
						if (row[CN.AcctType] == DBNull.Value)
						{
							row[CN.AcctType] = string.Empty;
						}
						if (_allEmployees && row[CN.EmployeeName] == DBNull.Value)
						{
							row[CN.EmployeeName] = GetResource("BranchTotal");
						}
						if (_allBranches && row[CN.BranchName] == DBNull.Value)
						{
							row[CN.BranchName] = GetResource("GrandTotal");
							row[CN.EmployeeName] = string.Empty;
						}
						//Remove the row if it is redundant based on the fields being
						//displayed - eg. A grand total for branch is irrelevant if
						//branch isn't being displayed.
						if (!_allEmployees && row[CN.EmployeeName].ToString() == string.Empty)
						{
							bookingsDetails.Rows.Remove(row);
						}
						else if (!_allBranches && row[CN.BranchName].ToString() == string.Empty)
						{
							bookingsDetails.Rows.Remove(row);
						}
					}
                    
				}
			}

			((MainForm)this.FormRoot).statusBar1.Text = statusText;

			dgBookings.Focus();
			return bookingsFound;
		}

		//
		// Events
		//

		private void MonitorBookings_Load(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Monitor Bookings Screen: Form Load";
				Wait();

				this.LoadStaticData();
				if (dgBookings.VisibleRowCount == 0)
				{
					this.ResetScreen(true);
				}
				//Initial focus
				if (dgBookings.VisibleRowCount == 0)
				{
					this.drpBranchNo.Focus();
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

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Monitor Bookings Screen: Reset Screen";
				Wait();
				this.ResetScreen(true);
				PopulateSalespersonDropDown();
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

		private void btnExit_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Monitor Bookings Screen: Close Tab";
				Wait();
				CloseTab();
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

		private void btnLoad_Click(object sender, System.EventArgs e)
		{
			// Load the bookingss for this branch
			try
			{
				Function = "Monitor Bookings Screen: Load Branch";
				Wait();

				LoadBookings();
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

		private void dtDateFrom_ValueChanged(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Monitor Bookings Screen: Date From changed";
				Wait();
				if (this._userChanged) this.ResetScreen(false);
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

		private void dtDateTo_ValueChanged(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Monitor Bookings Screen: Date To changed";
				Wait();

				if (this._userChanged) this.ResetScreen(false);
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

		private void btnExcel_Click(object sender, System.EventArgs e)
		{
			try
			{
				Wait();

				/* save the current data grid contents to a CSV */
				string comma = ",";
				string path = "";

				if(dgBookings.CurrentRowIndex >= 0)
				{
					DataView dv = (DataView)dgBookings.DataSource;

					SaveFileDialog save = new SaveFileDialog();
					save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*" ;
					save.Title = "Save Bookings";
					save.CreatePrompt = true;

					if(save.ShowDialog() == DialogResult.OK)
					{
						path = save.FileName;
						FileInfo fi = new FileInfo(path);
						FileStream fs = fi.OpenWrite();
			
						//Write heading line..
						string line = string.Empty;
						if (_allBranches)
						{
							line += GetResource("T_BRANCH") + comma;
						}
						if (_allEmployees)
						{
                            line += GetResource("T_BOOKINGSSALESPERSONID") + comma;
                            line += GetResource("T_BOOKINGSSALESPERSON") + comma;
						}
						line = line + GetResource("T_ACCOUNTTYPE") + comma +
							GetResource("T_VALUE") + comma +
							GetResource("T_BOOKINGSCANCELLED") + comma +
							CN.Revised + comma +
							GetResource("T_BOOKINGSSALESTAX") + comma +
							GetResource("T_BOOKINGSBALANCE") + comma +
							GetResource("T_BOOKINGSSUPERSHIELD") + comma +
							GetResource("T_DISCOUNT") + Environment.NewLine + Environment.NewLine;	
						byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
						fs.Write(blob,0,blob.Length);
			
						foreach(DataRowView row in dv)
						{					
							line = string.Empty;
							if (_allBranches)
							{
								line+= row[CN.BranchName] + comma;
							}
							if (_allEmployees)
							{
                                line += row[CN.EmployeeNo] + comma;
                                line += row[CN.EmployeeName] + comma;
                            }
							line +=	row[CN.AcctType] + comma +
								((decimal)row[CN.Value]).ToString(DecimalPlaces).Replace(",","") + comma +
								((decimal)row[CN.Cancelled]).ToString(DecimalPlaces).Replace(",","") + comma +
								((decimal)row[CN.Revised]).ToString(DecimalPlaces).Replace(",","") + comma +
								((decimal)row[CN.SalesTax]).ToString(DecimalPlaces).Replace(",","") + comma +
								((decimal)row[CN.Balance]).ToString(DecimalPlaces).Replace(",","") + comma +
								((decimal)row[CN.SuperShield]).ToString(DecimalPlaces).Replace(",","") + comma +
								((decimal)row[CN.Discount]).ToString(DecimalPlaces).Replace(",","") + 
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

		private void drpBranchNo_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Monitor Bookings Screen: Selected Branch changed";
				Wait();
				PopulateSalespersonDropDown();
				this.ResetScreen(false);
				//We may need to change what is displayed or hidden in response
				//to this change in selection so clear the TableStyles so a new
				//entry (and therefore grid layout) will be created.
				dgBookings.TableStyles.Clear();
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

		private void drpSalesPerson_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Monitor Bookings Screen: Selected Salesperson changed";
				Wait();
				this.ResetScreen(false);
				//We may need to change what is displayed or hidden in response
				//to this change in selection so clear the TableStyles so a new
				//entry (and therefore grid layout) will be created.
				dgBookings.TableStyles.Clear();
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
