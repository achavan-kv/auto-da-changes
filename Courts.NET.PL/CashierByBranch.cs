using System;
using System.Drawing;
using System.Collections;
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

namespace STL.PL
{
	/// <summary>
	/// Overages and shortages shown for each cashier in each branch. Cashiers
	/// are listed for the branch selected. For each cashier the total overage
	/// amount and the total shortage amount is shown for the date range selected.
	/// Clicking on a cashier shows a breakdown of the overages and shortages on
	/// each day for each pay method. (Note that is is possible to have an overage
	/// for one paymethod on the same day as a shortage for another paymethod.)
	/// </summary>
	public class CashierByBranch : CommonForm
	{
		private string error = "";
		private bool _userChanged;
		private DateTime _serverDate;
		private DataTable _cashierBreakdownList = null;

		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuSave;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private System.Windows.Forms.ComboBox drpBranchNo;
		private System.Windows.Forms.GroupBox gbBranch;
		private System.Windows.Forms.Label lBranch;
		private System.Windows.Forms.DataGrid dgBranch;
		private System.Windows.Forms.GroupBox gbCashier;
		private System.Windows.Forms.DataGrid dgCashier;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.DateTimePicker dtDateTo;
		private System.Windows.Forms.DateTimePicker dtDateFrom;
		private System.Windows.Forms.Label lTo;
		private System.Windows.Forms.Label lFrom;
		private System.Windows.Forms.TextBox txtCashier;
		private System.Windows.Forms.Label lShortageTot;
		private System.Windows.Forms.TextBox txtShortageTot;
		private System.Windows.Forms.Label lCashier;
		private System.Windows.Forms.Label lOverageTot;
		private System.Windows.Forms.TextBox txtOverageTot;
		private System.Windows.Forms.TextBox txtEmpeeNo;
		private System.Windows.Forms.Label lEmpeeNo;
		private System.Windows.Forms.Label lShortageAcct;
		private System.Windows.Forms.ToolTip ttCashier;
		private STL.PL.AccountTextBox txtShortageAcct;
		private System.ComponentModel.IContainer components;

		public CashierByBranch(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
		}

		public CashierByBranch(Form root, Form parent)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			FormRoot = root;
			FormParent = parent;

			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
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
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.ttCashier = new System.Windows.Forms.ToolTip(this.components);
            this.gbCashier = new System.Windows.Forms.GroupBox();
            this.lShortageAcct = new System.Windows.Forms.Label();
            this.txtEmpeeNo = new System.Windows.Forms.TextBox();
            this.lEmpeeNo = new System.Windows.Forms.Label();
            this.lOverageTot = new System.Windows.Forms.Label();
            this.txtOverageTot = new System.Windows.Forms.TextBox();
            this.txtCashier = new System.Windows.Forms.TextBox();
            this.lShortageTot = new System.Windows.Forms.Label();
            this.txtShortageTot = new System.Windows.Forms.TextBox();
            this.lCashier = new System.Windows.Forms.Label();
            this.dgCashier = new System.Windows.Forms.DataGrid();
            this.gbBranch = new System.Windows.Forms.GroupBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.dtDateTo = new System.Windows.Forms.DateTimePicker();
            this.dtDateFrom = new System.Windows.Forms.DateTimePicker();
            this.lTo = new System.Windows.Forms.Label();
            this.lFrom = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lBranch = new System.Windows.Forms.Label();
            this.drpBranchNo = new System.Windows.Forms.ComboBox();
            this.dgBranch = new System.Windows.Forms.DataGrid();
            this.txtShortageAcct = new STL.PL.AccountTextBox();
            this.gbCashier.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgCashier)).BeginInit();
            this.gbBranch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgBranch)).BeginInit();
            this.SuspendLayout();
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuSave,
            this.menuExit});
            this.menuFile.Text = "&File";
            // 
            // menuSave
            // 
            this.menuSave.Description = "MenuItem";
            this.menuSave.Text = "&Save";
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            // 
            // gbCashier
            // 
            this.gbCashier.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbCashier.BackColor = System.Drawing.SystemColors.Control;
            this.gbCashier.Controls.Add(this.txtShortageAcct);
            this.gbCashier.Controls.Add(this.lShortageAcct);
            this.gbCashier.Controls.Add(this.txtEmpeeNo);
            this.gbCashier.Controls.Add(this.lEmpeeNo);
            this.gbCashier.Controls.Add(this.lOverageTot);
            this.gbCashier.Controls.Add(this.txtOverageTot);
            this.gbCashier.Controls.Add(this.txtCashier);
            this.gbCashier.Controls.Add(this.lShortageTot);
            this.gbCashier.Controls.Add(this.txtShortageTot);
            this.gbCashier.Controls.Add(this.lCashier);
            this.gbCashier.Controls.Add(this.dgCashier);
            this.gbCashier.Location = new System.Drawing.Point(8, 224);
            this.gbCashier.Name = "gbCashier";
            this.gbCashier.Size = new System.Drawing.Size(776, 248);
            this.gbCashier.TabIndex = 1;
            this.gbCashier.TabStop = false;
            this.gbCashier.Text = "Cashier Breakdown";
            // 
            // lShortageAcct
            // 
            this.lShortageAcct.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lShortageAcct.Location = new System.Drawing.Point(8, 80);
            this.lShortageAcct.Name = "lShortageAcct";
            this.lShortageAcct.Size = new System.Drawing.Size(80, 16);
            this.lShortageAcct.TabIndex = 70;
            this.lShortageAcct.Text = "SHO Account";
            this.lShortageAcct.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtEmpeeNo
            // 
            this.txtEmpeeNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtEmpeeNo.Location = new System.Drawing.Point(96, 56);
            this.txtEmpeeNo.MaxLength = 80;
            this.txtEmpeeNo.Name = "txtEmpeeNo";
            this.txtEmpeeNo.ReadOnly = true;
            this.txtEmpeeNo.Size = new System.Drawing.Size(64, 20);
            this.txtEmpeeNo.TabIndex = 67;
            this.txtEmpeeNo.TabStop = false;
            // 
            // lEmpeeNo
            // 
            this.lEmpeeNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lEmpeeNo.Location = new System.Drawing.Point(16, 56);
            this.lEmpeeNo.Name = "lEmpeeNo";
            this.lEmpeeNo.Size = new System.Drawing.Size(72, 16);
            this.lEmpeeNo.TabIndex = 68;
            this.lEmpeeNo.Text = "Employee No";
            this.lEmpeeNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lOverageTot
            // 
            this.lOverageTot.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lOverageTot.Location = new System.Drawing.Point(16, 152);
            this.lOverageTot.Name = "lOverageTot";
            this.lOverageTot.Size = new System.Drawing.Size(72, 16);
            this.lOverageTot.TabIndex = 65;
            this.lOverageTot.Text = "OVE Total";
            this.lOverageTot.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtOverageTot
            // 
            this.txtOverageTot.BackColor = System.Drawing.SystemColors.Window;
            this.txtOverageTot.Location = new System.Drawing.Point(96, 152);
            this.txtOverageTot.MaxLength = 10;
            this.txtOverageTot.Name = "txtOverageTot";
            this.txtOverageTot.ReadOnly = true;
            this.txtOverageTot.Size = new System.Drawing.Size(96, 20);
            this.txtOverageTot.TabIndex = 66;
            // 
            // txtCashier
            // 
            this.txtCashier.BackColor = System.Drawing.SystemColors.Window;
            this.txtCashier.Location = new System.Drawing.Point(96, 32);
            this.txtCashier.MaxLength = 80;
            this.txtCashier.Name = "txtCashier";
            this.txtCashier.ReadOnly = true;
            this.txtCashier.Size = new System.Drawing.Size(160, 20);
            this.txtCashier.TabIndex = 62;
            this.txtCashier.TabStop = false;
            // 
            // lShortageTot
            // 
            this.lShortageTot.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lShortageTot.Location = new System.Drawing.Point(16, 128);
            this.lShortageTot.Name = "lShortageTot";
            this.lShortageTot.Size = new System.Drawing.Size(72, 16);
            this.lShortageTot.TabIndex = 61;
            this.lShortageTot.Text = "SHO Total";
            this.lShortageTot.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtShortageTot
            // 
            this.txtShortageTot.BackColor = System.Drawing.SystemColors.Window;
            this.txtShortageTot.Location = new System.Drawing.Point(96, 128);
            this.txtShortageTot.MaxLength = 10;
            this.txtShortageTot.Name = "txtShortageTot";
            this.txtShortageTot.ReadOnly = true;
            this.txtShortageTot.Size = new System.Drawing.Size(96, 20);
            this.txtShortageTot.TabIndex = 64;
            // 
            // lCashier
            // 
            this.lCashier.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lCashier.Location = new System.Drawing.Point(40, 32);
            this.lCashier.Name = "lCashier";
            this.lCashier.Size = new System.Drawing.Size(48, 16);
            this.lCashier.TabIndex = 63;
            this.lCashier.Text = "Cashier";
            this.lCashier.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dgCashier
            // 
            this.dgCashier.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgCashier.CaptionVisible = false;
            this.dgCashier.DataMember = "";
            this.dgCashier.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgCashier.Location = new System.Drawing.Point(264, 16);
            this.dgCashier.Name = "dgCashier";
            this.dgCashier.ReadOnly = true;
            this.dgCashier.Size = new System.Drawing.Size(504, 224);
            this.dgCashier.TabIndex = 0;
            // 
            // gbBranch
            // 
            this.gbBranch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbBranch.BackColor = System.Drawing.SystemColors.Control;
            this.gbBranch.Controls.Add(this.btnLoad);
            this.gbBranch.Controls.Add(this.dtDateTo);
            this.gbBranch.Controls.Add(this.dtDateFrom);
            this.gbBranch.Controls.Add(this.lTo);
            this.gbBranch.Controls.Add(this.lFrom);
            this.gbBranch.Controls.Add(this.btnExit);
            this.gbBranch.Controls.Add(this.btnClear);
            this.gbBranch.Controls.Add(this.lBranch);
            this.gbBranch.Controls.Add(this.drpBranchNo);
            this.gbBranch.Controls.Add(this.dgBranch);
            this.gbBranch.Location = new System.Drawing.Point(8, 0);
            this.gbBranch.Name = "gbBranch";
            this.gbBranch.Size = new System.Drawing.Size(776, 224);
            this.gbBranch.TabIndex = 0;
            this.gbBranch.TabStop = false;
            this.gbBranch.Text = "Cashiers";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(536, 32);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(48, 23);
            this.btnLoad.TabIndex = 40;
            this.btnLoad.Text = "Load";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // dtDateTo
            // 
            this.dtDateTo.CustomFormat = "dd MMM yyyy";
            this.dtDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDateTo.Location = new System.Drawing.Point(352, 32);
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
            this.dtDateFrom.Location = new System.Drawing.Point(216, 32);
            this.dtDateFrom.Name = "dtDateFrom";
            this.dtDateFrom.Size = new System.Drawing.Size(112, 20);
            this.dtDateFrom.TabIndex = 38;
            this.dtDateFrom.Tag = "";
            this.dtDateFrom.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            this.dtDateFrom.ValueChanged += new System.EventHandler(this.dtDateFrom_ValueChanged);
            // 
            // lTo
            // 
            this.lTo.Location = new System.Drawing.Point(352, 16);
            this.lTo.Name = "lTo";
            this.lTo.Size = new System.Drawing.Size(72, 16);
            this.lTo.TabIndex = 37;
            this.lTo.Text = "End Date";
            // 
            // lFrom
            // 
            this.lFrom.Location = new System.Drawing.Point(216, 16);
            this.lFrom.Name = "lFrom";
            this.lFrom.Size = new System.Drawing.Size(88, 16);
            this.lFrom.TabIndex = 36;
            this.lFrom.Text = "Start Date";
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.Location = new System.Drawing.Point(686, 64);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(48, 23);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(616, 32);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(48, 23);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lBranch
            // 
            this.lBranch.Location = new System.Drawing.Point(96, 16);
            this.lBranch.Name = "lBranch";
            this.lBranch.Size = new System.Drawing.Size(48, 16);
            this.lBranch.TabIndex = 2;
            this.lBranch.Text = "Branch";
            // 
            // drpBranchNo
            // 
            this.drpBranchNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranchNo.Location = new System.Drawing.Point(96, 32);
            this.drpBranchNo.Name = "drpBranchNo";
            this.drpBranchNo.Size = new System.Drawing.Size(96, 21);
            this.drpBranchNo.TabIndex = 0;
            this.drpBranchNo.SelectedIndexChanged += new System.EventHandler(this.drpBranchNo_SelectedIndexChanged);
            // 
            // dgBranch
            // 
            this.dgBranch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgBranch.DataMember = "";
            this.dgBranch.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgBranch.Location = new System.Drawing.Point(96, 64);
            this.dgBranch.Name = "dgBranch";
            this.dgBranch.ReadOnly = true;
            this.dgBranch.Size = new System.Drawing.Size(568, 152);
            this.dgBranch.TabIndex = 0;
            this.dgBranch.TabStop = false;
            this.dgBranch.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgBranch_MouseUp);
            // 
            // txtShortageAcct
            // 
            this.txtShortageAcct.BackColor = System.Drawing.SystemColors.Window;
            this.txtShortageAcct.Location = new System.Drawing.Point(96, 80);
            this.txtShortageAcct.MaxLength = 20;
            this.txtShortageAcct.Name = "txtShortageAcct";
            this.txtShortageAcct.PreventPaste = false;
            this.txtShortageAcct.ReadOnly = true;
            this.txtShortageAcct.Size = new System.Drawing.Size(96, 20);
            this.txtShortageAcct.TabIndex = 71;
            this.txtShortageAcct.Text = "000-0000-0000-0";
            // 
            // CashierByBranch
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.gbCashier);
            this.Controls.Add(this.gbBranch);
            this.Name = "CashierByBranch";
            this.Text = "Overages and Shortages";
            this.Load += new System.EventHandler(this.CashierByBranch_Load);
            this.gbCashier.ResumeLayout(false);
            this.gbCashier.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgCashier)).EndInit();
            this.gbBranch.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgBranch)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		//
		// Local Methods
		//

		private void ResetScreen(bool setDefault)
		{
			// Set the screen for initial entry

			// Disable change events before clearing all controls
			this._userChanged = false;

			// Preserve search fields
			int selectedBranch = this.drpBranchNo.SelectedIndex;									
			DateTime dateFrom = this.dtDateFrom.Value;
			DateTime dateTo = this.dtDateTo.Value;

			// Clear all controls
			ClearControls(this.Controls);

			if (setDefault)
			{
				// Initial custom settings
				// Set to this branch for the last 24 hours
				int i = this.drpBranchNo.FindStringExact(Config.BranchCode);
				this.drpBranchNo.SelectedIndex = i >= 0 ? i : 0;									
				this.dtDateFrom.Value = _serverDate.AddDays(-1);
				this.dtDateTo.Value = _serverDate;
			}
			else
			{
				// Restore search fields
				this.drpBranchNo.SelectedIndex = selectedBranch;									
				this.dtDateFrom.Value = dateFrom;
				this.dtDateTo.Value = dateTo;
			}

			// Enable the search fields and disable the Cashier list
			this.drpBranchNo.Enabled = true;
			this.dtDateFrom.Enabled = true;
			this.dtDateTo.Enabled = true;
			this.dgBranch.Enabled = false;
			this.gbCashier.Enabled = false;
			((MainForm)this.FormRoot).statusBar1.Text = "";

			// Initial focus
			this.drpBranchNo.Focus();
			// Enable change events
			this._userChanged = true;

		}  // End of ResetScreen

		private void LoadStaticData()
		{
			// Don't trigger any change events
			this._userChanged = false;

			this._serverDate = StaticDataManager.GetServerDate();
			this.dtDateFrom.MaxDate = this._serverDate;
			this.dtDateTo.MaxDate = this._serverDate;

			XmlUtilities xml = new XmlUtilities();
			XmlDocument dropDowns = new XmlDocument();
			dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
				
			if(StaticData.Tables[TN.BranchNumber]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.BranchNumber, null));
				
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
			
			drpBranchNo.DataSource = StaticData.Tables[TN.BranchNumber];
			drpBranchNo.DisplayMember = CN.BranchNo;
			drpBranchNo.ValueMember = CN.BranchNo;

			this._userChanged = true;
		}

		private bool LoadBranch()
		{
			bool cashierFound = false;
			string statusText = GetResource("M_CASHIERSZERO");
			DataSet cashierSet = null;
			DataView cashierListView = null;

			// Make sure the screen is reset
			ResetScreen(false);

			// Abort if the selected branch no is blank (or not a number)
			if (this.drpBranchNo.SelectedValue.ToString() == "" || !IsNumeric(this.drpBranchNo.SelectedValue.ToString()))
				return false;

			// Abort if the Start Date is after the End Date
			if (this.dtDateFrom.Value > this.dtDateTo.Value)
			{
				ShowInfo("M_STARTDATEAFTEREND");
				return false;
			}

			// Load the list of Cashiers with an overage or shortage for this branch
			((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_LOADINGDATA");
			cashierSet = PaymentManager.GetBranchCashierList(Convert.ToInt16(this.drpBranchNo.SelectedValue), this.dtDateFrom.Value, this.dtDateTo.Value, out error);
			if(error.Length > 0)
			{
				ShowError(error);
				return false;
			}

			// Don't trigger any change events
			this._userChanged = false;

			foreach (DataTable cashierDetails in cashierSet.Tables)
			{
				if (cashierDetails.TableName == TN.CashierByBranch)
				{
					// Display list of Cashiers
					cashierFound = (cashierDetails.Rows.Count > 0);
					statusText = cashierDetails.Rows.Count + GetResource("M_CASHIERSLISTED");

					cashierListView = new DataView(cashierDetails);
					cashierListView.AllowNew = false;
					dgBranch.DataSource = cashierListView;
					cashierListView.Sort = CN.EmployeeName + " ASC ";
					
					if (dgBranch.TableStyles.Count == 0)
					{
						DataGridTableStyle tabStyle = new DataGridTableStyle();
						tabStyle.MappingName = cashierListView.Table.TableName;

						dgBranch.TableStyles.Clear();
						dgBranch.TableStyles.Add(tabStyle);
						dgBranch.DataSource = cashierListView;

						// Hidden columns
						tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 0;
						tabStyle.GridColumnStyles[CN.AccountNumber].Width = 0;

						// Displayed columns
						tabStyle.GridColumnStyles[CN.EmployeeName].Width = 300;
						tabStyle.GridColumnStyles[CN.EmployeeName].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.EmployeeName].HeaderText = GetResource("T_CASHIERNAME");

						tabStyle.GridColumnStyles[CN.Shortage].Width = 110;
						tabStyle.GridColumnStyles[CN.Shortage].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.Shortage].HeaderText = GetResource("T_CASHIERSHORTAGE");
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Shortage]).Format = DecimalPlaces;

						tabStyle.GridColumnStyles[CN.Overage].Width = 110;
						tabStyle.GridColumnStyles[CN.Overage].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.Overage].HeaderText = GetResource("T_CASHIEROVERAGE");
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Overage]).Format = DecimalPlaces;
					}

					// Enable the cashier list
					this.dgBranch.Enabled = true;
				}
				else if (cashierDetails.TableName == TN.CashierBreakdown)
				{
					// This is the breakdown of all the Cashiers for this Branch
					// Keep a private copy to display from later
					this._cashierBreakdownList = cashierDetails;
				}
			}

			((MainForm)this.FormRoot).statusBar1.Text = statusText;
			this._userChanged = true;
			return cashierFound;
		}

		private void ShowBreakdown (int index)
		{
			DataView cashierList = (DataView)dgBranch.DataSource;
			DataRow  cashier	 = cashierList[index].Row;
			this.dgBranch.Select(index);

			// Copy the simple fields
			this.txtCashier.Text		= cashier[CN.EmployeeName].ToString();
			this.txtEmpeeNo.Text		= cashier[CN.EmployeeNo].ToString();
			this.txtShortageAcct.Text	= cashier[CN.AccountNumber].ToString();
			this.txtShortageTot.Text	= ((decimal)cashier[CN.Shortage]).ToString(DecimalPlaces);
			this.txtOverageTot.Text		= ((decimal)cashier[CN.Overage]).ToString(DecimalPlaces);

			// Display the breakdown list filtered by this EmpeeNo
			DataView breakdownListView = new DataView(this._cashierBreakdownList);
			breakdownListView.AllowNew = false;
			breakdownListView.Sort = CN.DateTo + "," + CN.TransTypeCode + "," + CN.PayMethod + "," + CN.Difference + " ASC ";
			// Apply the filter and show the count of the transactions left in the list
			breakdownListView.RowFilter = CN.EmployeeNo + " = '" + cashier[CN.EmployeeNo].ToString() + "'";
			((MainForm)this.FormRoot).statusBar1.Text = breakdownListView.Count + GetResource("M_TRANSACTIONSLISTED");
			dgCashier.DataSource = breakdownListView;
					
			if (dgCashier.TableStyles.Count == 0)
			{
				DataGridTableStyle tabStyle = new DataGridTableStyle();
				tabStyle.MappingName = breakdownListView.Table.TableName;

				dgCashier.TableStyles.Clear();
				dgCashier.TableStyles.Add(tabStyle);
				dgCashier.DataSource = breakdownListView;

				// Hidden columns
				tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 0;

				// Displayed columns
				tabStyle.GridColumnStyles[CN.DateTo].Width = 70;
				tabStyle.GridColumnStyles[CN.DateTo].ReadOnly = true;
				tabStyle.GridColumnStyles[CN.DateTo].HeaderText = GetResource("T_DATE");

				tabStyle.GridColumnStyles[CN.TransTypeCode].Width = 40;
				tabStyle.GridColumnStyles[CN.TransTypeCode].ReadOnly = true;
				tabStyle.GridColumnStyles[CN.TransTypeCode].HeaderText = GetResource("T_TYPE");

				tabStyle.GridColumnStyles[CN.PayMethod].Width = 120;
				tabStyle.GridColumnStyles[CN.PayMethod].ReadOnly = true;
				tabStyle.GridColumnStyles[CN.PayMethod].HeaderText = GetResource("T_PAYMETHOD");

				tabStyle.GridColumnStyles[CN.Difference].Width = 100;
				tabStyle.GridColumnStyles[CN.Difference].ReadOnly = true;
				tabStyle.GridColumnStyles[CN.Difference].HeaderText = GetResource("T_AMOUNT1");
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Difference]).Format = DecimalPlaces;

				tabStyle.GridColumnStyles[CN.Reason].Width = 400;
				tabStyle.GridColumnStyles[CN.Reason].ReadOnly = true;
				tabStyle.GridColumnStyles[CN.Reason].HeaderText = GetResource("T_REASON");
			}

			// Enable the Cashier details
			this.gbCashier.Enabled = true;
		}

		//
		// Events
		//

		private void CashierByBranch_Load(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Branch Overages and Shortages Screen: Form Load";
				Wait();

				this.ttCashier.SetToolTip(dgBranch, GetResource("TT_CASHIERBREAKDOWN"));
				this.LoadStaticData();
				this.ResetScreen(true);
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
				Function = "Branch Overages and Shortages Screen: Reset Screen";
				Wait();
				this.ResetScreen(true);
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
				Function = "Branch Overages and Shortages Screen: Close Tab";
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
			// Load the cashiers for this branch
			try
			{
				Function = "Branch Overages and Shortages Screen: Load Branch";
				Wait();

				if (this.LoadBranch()) ShowBreakdown(0);
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

		private void drpBranchNo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Branch Overages and Shortages Screen: Selected Branch changed";
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

		private void dtDateFrom_ValueChanged(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Branch Overages and Shortages Screen: Date From changed";
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
				Function = "Branch Overages and Shortages Screen: Date To changed";
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

		private void dgBranch_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				Function = "Branch Overages and Shortages Screen: Cashier clicked";
				Wait();

				int index = dgBranch.CurrentRowIndex;
				if (index >= 0 && e.Button == MouseButtons.Left)
				{
					this.ShowBreakdown(index);
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