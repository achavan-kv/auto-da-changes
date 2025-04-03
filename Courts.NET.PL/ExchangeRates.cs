using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Xml;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants;
using STL.Common.Constants.Categories;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.ExchangeRate;
using Crownwood.Magic.Menus;
using STL.Common.Constants.FTransaction;

namespace STL.PL
{
	/// <summary>
	/// Maintenance screen for foreign currency exchange rates. All of the 
	/// current rates are listed. A new rate for a foreign currency can be
	/// entered. Each new rate will adopt the current date and time as its
	/// start date. The history for each currency can be reviewed here. This
	/// shows each value, when it started and the employee that changed it.
	/// </summary>
	public class ExchangeRates : STL.PL.CommonForm
	{
		//
		// Local properties
		//
		private string _error = "";
		private decimal _curExchangeRate = -1.0M;

		//
		// Change event control
		//
		private bool _userChanged = false;

		//
		// Form data
		//
		private Crownwood.Magic.Controls.TabPage tbUpdate;
		private Crownwood.Magic.Controls.TabPage tbView;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.GroupBox gbMain;
		private System.Windows.Forms.GroupBox gbCurrent;
		private System.Windows.Forms.GroupBox gbHistory;
		private System.Windows.Forms.Button btnEnter;
		private System.Windows.Forms.Button btnSave;
		private Crownwood.Magic.Controls.TabControl tcRates;
		private System.Windows.Forms.Label lDateFrom;
		private System.Windows.Forms.Label lCurrency2;
		private System.Windows.Forms.DataGrid dgHistory;
		private System.Windows.Forms.Label lExchangeRate;
		private System.Windows.Forms.Label lCurrency1;
		private System.Windows.Forms.TextBox txtExchangeRate;
		private System.Windows.Forms.DataGrid dgCurrent;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.Label lDateTo;
		private System.Windows.Forms.DateTimePicker dtDateTo;
		private System.Windows.Forms.DateTimePicker dtDateFrom;
		private System.Windows.Forms.ComboBox drpCurrency2;
		private System.Windows.Forms.ComboBox drpCurrency1;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuSave;
		private Crownwood.Magic.Menus.MenuCommand menuClear;
		private Crownwood.Magic.Menus.MenuCommand menuPrint;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ToolTip ttExchangeRates;
		private System.ComponentModel.IContainer components = null;

		//
		// Form Constructors
		//
		public ExchangeRates(TranslationDummy d)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile, menuSave});
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile, menuClear});
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile, menuExit});
			//menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile, menuPrint});
		}

		public ExchangeRates(Form root, Form parent)
		{

			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Set up
			FormRoot = root;
			FormParent = parent;

            //IP - 11/12/08 - UAT(609) - Menus were being duplicated.
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile});

            //menuMain = new Crownwood.Magic.Menus.MenuControl();
            //menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile, menuSave});
            //menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile, menuClear});
            //menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile, menuExit});
            //menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile, menuPrint});
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if ( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ExchangeRates));
			this.gbMain = new System.Windows.Forms.GroupBox();
			this.btnExit = new System.Windows.Forms.Button();
			this.btnClear = new System.Windows.Forms.Button();
			this.tcRates = new Crownwood.Magic.Controls.TabControl();
			this.tbView = new Crownwood.Magic.Controls.TabPage();
			this.btnLoad = new System.Windows.Forms.Button();
			this.lDateTo = new System.Windows.Forms.Label();
			this.lDateFrom = new System.Windows.Forms.Label();
			this.dtDateTo = new System.Windows.Forms.DateTimePicker();
			this.dtDateFrom = new System.Windows.Forms.DateTimePicker();
			this.lCurrency2 = new System.Windows.Forms.Label();
			this.drpCurrency2 = new System.Windows.Forms.ComboBox();
			this.gbHistory = new System.Windows.Forms.GroupBox();
			this.dgHistory = new System.Windows.Forms.DataGrid();
			this.tbUpdate = new Crownwood.Magic.Controls.TabPage();
			this.lExchangeRate = new System.Windows.Forms.Label();
			this.lCurrency1 = new System.Windows.Forms.Label();
			this.btnEnter = new System.Windows.Forms.Button();
			this.txtExchangeRate = new System.Windows.Forms.TextBox();
			this.drpCurrency1 = new System.Windows.Forms.ComboBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.gbCurrent = new System.Windows.Forms.GroupBox();
			this.dgCurrent = new System.Windows.Forms.DataGrid();
			this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
			this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
			this.menuClear = new Crownwood.Magic.Menus.MenuCommand();
			this.menuPrint = new Crownwood.Magic.Menus.MenuCommand();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.ttExchangeRates = new System.Windows.Forms.ToolTip(this.components);
			this.gbMain.SuspendLayout();
			this.tcRates.SuspendLayout();
			this.tbView.SuspendLayout();
			this.gbHistory.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgHistory)).BeginInit();
			this.tbUpdate.SuspendLayout();
			this.gbCurrent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgCurrent)).BeginInit();
			this.SuspendLayout();
			// 
			// gbMain
			// 
			this.gbMain.BackColor = System.Drawing.SystemColors.Control;
			this.gbMain.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.btnExit,
																				 this.btnClear,
																				 this.tcRates});
			this.gbMain.Location = new System.Drawing.Point(8, 0);
			this.gbMain.Name = "gbMain";
			this.gbMain.Size = new System.Drawing.Size(776, 472);
			this.gbMain.TabIndex = 3;
			this.gbMain.TabStop = false;
			// 
			// btnExit
			// 
			this.btnExit.Location = new System.Drawing.Point(696, 16);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(48, 23);
			this.btnExit.TabIndex = 0;
			this.btnExit.TabStop = false;
			this.btnExit.Text = "Exit";
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(616, 16);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(48, 23);
			this.btnClear.TabIndex = 0;
			this.btnClear.TabStop = false;
			this.btnClear.Text = "Clear";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// tcRates
			// 
			this.tcRates.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tcRates.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.tcRates.IDEPixelArea = true;
			this.tcRates.Location = new System.Drawing.Point(8, 56);
			this.tcRates.Name = "tcRates";
			this.tcRates.PositionTop = true;
			this.tcRates.SelectedIndex = 0;
			this.tcRates.SelectedTab = this.tbUpdate;
			this.tcRates.Size = new System.Drawing.Size(760, 416);
			this.tcRates.TabIndex = 0;
			this.tcRates.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
																					  this.tbUpdate,
																					  this.tbView});
			this.tcRates.Click += new System.EventHandler(this.tcRates_Click);
			// 
			// tbView
			// 
			this.tbView.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.btnLoad,
																				 this.lDateTo,
																				 this.lDateFrom,
																				 this.dtDateTo,
																				 this.dtDateFrom,
																				 this.lCurrency2,
																				 this.drpCurrency2,
																				 this.gbHistory});
			this.tbView.Location = new System.Drawing.Point(0, 25);
			this.tbView.Name = "tbView";
			this.tbView.Selected = false;
			this.tbView.Size = new System.Drawing.Size(760, 391);
			this.tbView.TabIndex = 4;
			this.tbView.Title = "View Rates";
			// 
			// btnLoad
			// 
			this.btnLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnLoad.Location = new System.Drawing.Point(592, 24);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(88, 23);
			this.btnLoad.TabIndex = 140;
			this.btnLoad.Text = "Load";
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// lDateTo
			// 
			this.lDateTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lDateTo.Location = new System.Drawing.Point(328, 48);
			this.lDateTo.Name = "lDateTo";
			this.lDateTo.Size = new System.Drawing.Size(32, 16);
			this.lDateTo.TabIndex = 0;
			this.lDateTo.Text = "To";
			this.lDateTo.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// lDateFrom
			// 
			this.lDateFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lDateFrom.Location = new System.Drawing.Point(280, 24);
			this.lDateFrom.Name = "lDateFrom";
			this.lDateFrom.Size = new System.Drawing.Size(80, 16);
			this.lDateFrom.TabIndex = 0;
			this.lDateFrom.Text = "Date From";
			this.lDateFrom.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// dtDateTo
			// 
			this.dtDateTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.dtDateTo.Location = new System.Drawing.Point(376, 48);
			this.dtDateTo.Name = "dtDateTo";
			this.dtDateTo.Size = new System.Drawing.Size(128, 20);
			this.dtDateTo.TabIndex = 130;
			// 
			// dtDateFrom
			// 
			this.dtDateFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.dtDateFrom.Location = new System.Drawing.Point(376, 24);
			this.dtDateFrom.Name = "dtDateFrom";
			this.dtDateFrom.Size = new System.Drawing.Size(128, 20);
			this.dtDateFrom.TabIndex = 120;
			// 
			// lCurrency2
			// 
			this.lCurrency2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lCurrency2.Location = new System.Drawing.Point(16, 24);
			this.lCurrency2.Name = "lCurrency2";
			this.lCurrency2.Size = new System.Drawing.Size(80, 16);
			this.lCurrency2.TabIndex = 0;
			this.lCurrency2.Text = "Currency";
			this.lCurrency2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// drpCurrency2
			// 
			this.drpCurrency2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpCurrency2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.drpCurrency2.Location = new System.Drawing.Point(112, 24);
			this.drpCurrency2.Name = "drpCurrency2";
			this.drpCurrency2.Size = new System.Drawing.Size(152, 21);
			this.drpCurrency2.TabIndex = 110;
			// 
			// gbHistory
			// 
			this.gbHistory.BackColor = System.Drawing.SystemColors.Control;
			this.gbHistory.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.dgHistory});
			this.gbHistory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.gbHistory.Location = new System.Drawing.Point(8, 96);
			this.gbHistory.Name = "gbHistory";
			this.gbHistory.Size = new System.Drawing.Size(736, 280);
			this.gbHistory.TabIndex = 2;
			this.gbHistory.TabStop = false;
			this.gbHistory.Text = "Current and History Rates";
			// 
			// dgHistory
			// 
			this.dgHistory.CaptionVisible = false;
			this.dgHistory.DataMember = "";
			this.dgHistory.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgHistory.Location = new System.Drawing.Point(8, 24);
			this.dgHistory.Name = "dgHistory";
			this.dgHistory.Size = new System.Drawing.Size(720, 248);
			this.dgHistory.TabIndex = 150;
			// 
			// tbUpdate
			// 
			this.tbUpdate.Controls.AddRange(new System.Windows.Forms.Control[] {
																				   this.lExchangeRate,
																				   this.lCurrency1,
																				   this.btnEnter,
																				   this.txtExchangeRate,
																				   this.drpCurrency1,
																				   this.btnSave,
																				   this.gbCurrent});
			this.tbUpdate.Location = new System.Drawing.Point(0, 24);
			this.tbUpdate.Name = "tbUpdate";
			this.tbUpdate.Size = new System.Drawing.Size(760, 392);
			this.tbUpdate.TabIndex = 3;
			this.tbUpdate.Title = "Update Rates";
			// 
			// lExchangeRate
			// 
			this.lExchangeRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lExchangeRate.Location = new System.Drawing.Point(296, 48);
			this.lExchangeRate.Name = "lExchangeRate";
			this.lExchangeRate.Size = new System.Drawing.Size(96, 16);
			this.lExchangeRate.TabIndex = 0;
			this.lExchangeRate.Text = "Exchange Rate";
			this.lExchangeRate.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// lCurrency1
			// 
			this.lCurrency1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lCurrency1.Location = new System.Drawing.Point(24, 48);
			this.lCurrency1.Name = "lCurrency1";
			this.lCurrency1.Size = new System.Drawing.Size(80, 16);
			this.lCurrency1.TabIndex = 0;
			this.lCurrency1.Text = "Currency";
			this.lCurrency1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// btnEnter
			// 
			this.btnEnter.Enabled = false;
			this.btnEnter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnEnter.Location = new System.Drawing.Point(552, 48);
			this.btnEnter.Name = "btnEnter";
			this.btnEnter.Size = new System.Drawing.Size(64, 24);
			this.btnEnter.TabIndex = 30;
			this.btnEnter.Text = "Enter";
			this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
			// 
			// txtExchangeRate
			// 
			this.txtExchangeRate.Location = new System.Drawing.Point(408, 48);
			this.txtExchangeRate.MaxLength = 10;
			this.txtExchangeRate.Name = "txtExchangeRate";
			this.txtExchangeRate.Size = new System.Drawing.Size(96, 20);
			this.txtExchangeRate.TabIndex = 20;
			this.txtExchangeRate.Text = "";
			this.txtExchangeRate.Leave += new System.EventHandler(this.txtExchangeRate_Leave);
			// 
			// drpCurrency1
			// 
			this.drpCurrency1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpCurrency1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.drpCurrency1.Location = new System.Drawing.Point(120, 48);
			this.drpCurrency1.Name = "drpCurrency1";
			this.drpCurrency1.Size = new System.Drawing.Size(152, 21);
			this.drpCurrency1.TabIndex = 10;
			this.drpCurrency1.SelectedIndexChanged += new System.EventHandler(this.drpCurrency1_SelectedIndexChanged);
			// 
			// btnSave
			// 
			this.btnSave.BackgroundImage = ((System.Drawing.Bitmap)(resources.GetObject("btnSave.BackgroundImage")));
			this.btnSave.Enabled = false;
			this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnSave.Location = new System.Drawing.Point(664, 48);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(24, 24);
			this.btnSave.TabIndex = 40;
			this.btnSave.TabStop = false;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// gbCurrent
			// 
			this.gbCurrent.BackColor = System.Drawing.SystemColors.Control;
			this.gbCurrent.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.dgCurrent});
			this.gbCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.gbCurrent.Location = new System.Drawing.Point(8, 128);
			this.gbCurrent.Name = "gbCurrent";
			this.gbCurrent.Size = new System.Drawing.Size(736, 232);
			this.gbCurrent.TabIndex = 1;
			this.gbCurrent.TabStop = false;
			this.gbCurrent.Text = "Current Rates";
			// 
			// dgCurrent
			// 
			this.dgCurrent.CaptionVisible = false;
			this.dgCurrent.DataMember = "";
			this.dgCurrent.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgCurrent.Location = new System.Drawing.Point(8, 24);
			this.dgCurrent.Name = "dgCurrent";
			this.dgCurrent.Size = new System.Drawing.Size(720, 200);
			this.dgCurrent.TabIndex = 50;
			this.dgCurrent.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgCurrent_MouseUp);
			// 
			// menuExit
			// 
			this.menuExit.Description = "MenuItem";
			this.menuExit.ImageIndex = 1;
			this.menuExit.Text = "E&xit";
            this.menuExit.Click += new EventHandler(btnExit_Click);
			// 
			// menuFile
			// 
			this.menuFile.Description = "MenuItem";
			this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
																							this.menuSave,
																							this.menuExit,
																							this.menuClear});
			this.menuFile.Text = "&File";
			// 
			// menuSave
			// 
			this.menuSave.Description = "MenuItem";
			this.menuSave.ImageIndex = 2;
			this.menuSave.Text = "&Save";
            this.menuSave.Click += new EventHandler(btnSave_Click);
			// 
			// menuClear
			// 
			this.menuClear.Description = "MenuItem";
			this.menuClear.Text = "&Clear";
            this.menuClear.Click += new EventHandler(btnClear_Click);
			// 
			// menuPrint
			// 
			this.menuPrint.Description = "MenuItem";
			this.menuPrint.Text = "&Print";
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// ExchangeRates
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(792, 477);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.gbMain});
			this.Name = "ExchangeRates";
			this.Text = "Exhange Rates";
			this.Load += new System.EventHandler(this.ExchangeRates_Load);
			this.gbMain.ResumeLayout(false);
			this.tcRates.ResumeLayout(false);
			this.tbView.ResumeLayout(false);
			this.gbHistory.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgHistory)).EndInit();
			this.tbUpdate.ResumeLayout(false);
			this.gbCurrent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgCurrent)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		//
		// Local Procedures
		//

		private void ResetScreen()
		{
			// Set the screen for initial entry and after clearing

			// Reset change events first before clearing all controls
			this._userChanged = false;

			// Clear all controls
			ClearControls(this.Controls);
			((MainForm)this.FormRoot).statusBar1.Text = "";

			// Initial custom settings

			// Update tab
			this._curExchangeRate = -1.0M;
			this.btnEnter.Enabled = false;
			this.btnSave.Enabled = false;

			// View tab
			this.dtDateFrom.Value = Date.blankDate;
			this.dtDateTo.Value = Date.blankDate;

			// Initial focus
			if (this.tcRates.SelectedIndex == 0) this.drpCurrency1.Focus();
			else this.drpCurrency2.Focus();

			// Enable change events
			this._userChanged = true;

		}  // End of ResetScreen


        //private bool ValidMoneyField(TextBox moneyField, out decimal moneyValue)
        //{
        //    // Validate a money field for the Country format
        //    return ValidMoneyField(moneyField, out moneyValue, DecimalPlaces);
        //}  // End of ValidMoneyField

		private bool ValidMoneyField(TextBox moneyField, out decimal moneyValue, string decimalPlaces)
		{
			// Validate a money field for a custom format (for Exchange Rates)
			// Check a blank or zero money value entered
			moneyValue = 0.0M;
			moneyField.Text = moneyField.Text.Trim();
			if (!IsStrictMoney(moneyField.Text))
			{
				ShowInfo("M_NUMERIC");
				// Trap the focus in this field
				moneyField.Focus();
				return false;
			}

			// Reformat
			moneyValue = MoneyStrToDecimal(moneyField.Text, decimalPlaces);
			moneyField.Text = moneyValue.ToString(decimalPlaces);

			return true;
		}  // End of ValidMoneyField

		private bool LoadCurrent()
		{
			// Load the Current Rates
			string statusText = GetResource("M_RATESZERO");
			DataSet ExchangeRateSet = null;
			DataView ExchangeRateView = null;

			// Make sure the screen is reset
			ResetScreen();
			((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_LOADINGDATA");

			// Load the current Exchange Rates
			ExchangeRateSet = PaymentManager.GetExchangeRates(out _error);

			if (_error.Length > 0)
			{
				ShowError(_error);
				return false;
			}

			foreach (DataTable RateDetails in ExchangeRateSet.Tables)
			{
				if (RateDetails.TableName == TN.ExchangeRates)
				{
					// Display list of Exchange Rates
					statusText = RateDetails.Rows.Count + GetResource("M_RATESLISTED");

					ExchangeRateView = new DataView(RateDetails);
					ExchangeRateView.AllowNew = false;
					ExchangeRateView.Sort = CN.Currency + " ASC ";
					dgCurrent.DataSource = ExchangeRateView;

					if (dgCurrent.TableStyles.Count == 0)
					{
						DataGridTableStyle tabStyle = new DataGridTableStyle();
						tabStyle.MappingName = ExchangeRateView.Table.TableName;

						int numCols = ExchangeRateView.Table.Columns.Count;

						// Add an unbound stand-alone icon column to mark new entries
						ExchangeRateView.Table.Columns.Add("Icon");
						DataGridIconColumn iconColumn = new DataGridIconColumn(imageList1.Images[0], CN.Status, RateStatus.Current);
						iconColumn.HeaderText = "";
						iconColumn.MappingName = "Icon";
						iconColumn.Width = imageList1.Images[0].Size.Width;
						tabStyle.GridColumnStyles.Add(iconColumn);

						DataGridTextBoxColumn aColumnTextColumn ;
						for(int i = 0; i < numCols; ++i)
						{
							aColumnTextColumn = new DataGridTextBoxColumn();
							aColumnTextColumn.HeaderText = ExchangeRateView.Table.Columns[i].ColumnName;
							aColumnTextColumn.MappingName = ExchangeRateView.Table.Columns[i].ColumnName;
							tabStyle.GridColumnStyles.Add(aColumnTextColumn);
						}



						dgCurrent.TableStyles.Clear();
						dgCurrent.TableStyles.Add(tabStyle);
						dgCurrent.DataSource = ExchangeRateView;

						// Hidden columns
						tabStyle.GridColumnStyles[CN.Status].Width = 0;

						// Displayed columns
						tabStyle.GridColumnStyles[CN.Code].Width = 50;
						tabStyle.GridColumnStyles[CN.Code].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.Code].HeaderText = GetResource("T_CODE");

						tabStyle.GridColumnStyles[CN.Currency].Width = 170;
						tabStyle.GridColumnStyles[CN.Currency].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.Currency].HeaderText = GetResource("T_CURRENCY");

						tabStyle.GridColumnStyles[CN.ExchangeRate].Width = 90;
						tabStyle.GridColumnStyles[CN.ExchangeRate].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.ExchangeRate].HeaderText = GetResource("T_RATE");
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.ExchangeRate]).Format = RateFormat.DecimalPlaces;

						tabStyle.GridColumnStyles[CN.DateFrom].Width = 130;
						tabStyle.GridColumnStyles[CN.DateFrom].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.DateFrom].HeaderText = GetResource("T_DATEFROM");

						tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 80;
						tabStyle.GridColumnStyles[CN.EmployeeNo].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.EmployeeNo].HeaderText = GetResource("T_EMPEENO");

						tabStyle.GridColumnStyles[CN.EmployeeName].Width = 180;
						tabStyle.GridColumnStyles[CN.EmployeeName].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.EmployeeName].HeaderText = GetResource("T_EMPEENAME");
					}
					else
					{
						// Add an unbound stand-alone icon column to mark new entries
						ExchangeRateView.Table.Columns.Add("Icon");
					}
				}
				if (this.tcRates.SelectedIndex == 0)
                    ((MainForm)this.FormRoot).statusBar1.Text = statusText;
				else
					((MainForm)this.FormRoot).statusBar1.Text = "";
			}	

			return true;
		}  // End of LoadCurrent

		private bool LoadHistory()
		{
			// Load a filtered list of Current and History Rates for viewing
			string statusText = GetResource("M_RATESZERO");
			DataSet ExchangeRateSet = null;
			DataView ExchangeRateView = null;

            //UAT 605 - Error in Exchange Rate Maintenance Screen
            if (drpCurrency2.SelectedItem == null || drpCurrency2.SelectedItem.ToString().Trim() == "")
                return false;

            // Make sure the screen is reset
			((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_LOADINGDATA");

			// Load the current and History Exchange Rates
			ExchangeRateSet = PaymentManager.GetExchangeRateHistory(((DataRowView)drpCurrency2.SelectedItem)[CN.Code].ToString(), this.dtDateFrom.Value, this.dtDateTo.Value, out _error);

			if (_error.Length > 0)
			{
				ShowError(_error);
				return false;
			}

			foreach (DataTable RateDetails in ExchangeRateSet.Tables)
			{
				if (RateDetails.TableName == TN.ExchangeRates)
				{
					// Display list of Exchange Rates
					statusText = RateDetails.Rows.Count + GetResource("M_RATESLISTED");

					// Add new entry column for user entry of new rates

					ExchangeRateView = new DataView(RateDetails);
					ExchangeRateView.AllowNew = false;
					ExchangeRateView.Sort = CN.Currency + " ASC ";
					dgHistory.DataSource = ExchangeRateView;

					if (dgHistory.TableStyles.Count == 0)
					{
						DataGridTableStyle tabStyle = new DataGridTableStyle();
						tabStyle.MappingName = ExchangeRateView.Table.TableName;

						int numCols = ExchangeRateView.Table.Columns.Count;

						// Add an unbound stand-alone icon column to mark new entries
						ExchangeRateView.Table.Columns.Add("Icon");
						DataGridIconColumn iconColumn = new DataGridIconColumn(imageList1.Images[1], CN.Status, RateStatus.History);
						iconColumn.HeaderText = "";
						iconColumn.MappingName = "Icon";
						iconColumn.Width = imageList1.Images[0].Size.Width;
						tabStyle.GridColumnStyles.Add(iconColumn);

						DataGridTextBoxColumn aColumnTextColumn ;
						for(int i = 0; i < numCols; ++i)
						{
							aColumnTextColumn = new DataGridTextBoxColumn();
							aColumnTextColumn.HeaderText = ExchangeRateView.Table.Columns[i].ColumnName;
							aColumnTextColumn.MappingName = ExchangeRateView.Table.Columns[i].ColumnName;
							tabStyle.GridColumnStyles.Add(aColumnTextColumn);
						}

						dgHistory.TableStyles.Clear();
						dgHistory.TableStyles.Add(tabStyle);
						dgHistory.DataSource = ExchangeRateView;

						// Hidden columns
						tabStyle.GridColumnStyles[CN.Status].Width = 0;

						// Displayed columns
						tabStyle.GridColumnStyles[CN.Code].Width = 50;
						tabStyle.GridColumnStyles[CN.Code].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.Code].HeaderText = GetResource("T_CODE");

						tabStyle.GridColumnStyles[CN.Currency].Width = 170;
						tabStyle.GridColumnStyles[CN.Currency].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.Currency].HeaderText = GetResource("T_CURRENCY");

						tabStyle.GridColumnStyles[CN.ExchangeRate].Width = 90;
						tabStyle.GridColumnStyles[CN.ExchangeRate].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.ExchangeRate].HeaderText = GetResource("T_RATE");
						((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.ExchangeRate]).Format = RateFormat.DecimalPlaces;

						tabStyle.GridColumnStyles[CN.DateFrom].Width = 130;
						tabStyle.GridColumnStyles[CN.DateFrom].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.DateFrom].HeaderText = GetResource("T_DATEFROM");

						tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 80;
						tabStyle.GridColumnStyles[CN.EmployeeNo].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.EmployeeNo].HeaderText = GetResource("T_EMPEENO");

						tabStyle.GridColumnStyles[CN.EmployeeName].Width = 180;
						tabStyle.GridColumnStyles[CN.EmployeeName].ReadOnly = true;
						tabStyle.GridColumnStyles[CN.EmployeeName].HeaderText = GetResource("T_EMPEENAME");
					}
					else
					{
						// Add an unbound stand-alone icon column to mark new entries
						ExchangeRateView.Table.Columns.Add("Icon");
					}
				}
				((MainForm)this.FormRoot).statusBar1.Text = statusText;
			}	

			return true;
		}  // End of LoadHistory

		private void SetEnterButton()
		{
			// Enter button is only enabled when the Currency and Rate value are entered
			this.btnEnter.Enabled = (this.drpCurrency1.Text != "" && this._curExchangeRate >= 0.0M);
		}  // End of SetEnterButton

		private bool CanSave()
		{
			// Check whether any new rows are in the list
			DataView currentView = (DataView)dgCurrent.DataSource;
			bool rowFound = false;
			foreach (DataRowView rowView in currentView)
			{
				rowFound = rowFound || ((string)rowView[CN.Status] == RateStatus.Edit);
			}
			return rowFound;
		}  // End of CanSave

		private void PromptChanges(string msgName)
		{
			// Check whether to save changes
			if (this.CanSave())
			{
				if (DialogResult.Yes == ShowInfo(msgName, MessageBoxButtons.YesNo))
				{
					this.SaveChanges();
					this.ResetScreen();
				}
			}
		}  // End of PromptChanges

		private void SaveChanges()
		{
			DataSet exchangeRateSet = null;
			exchangeRateSet = ((DataView)dgCurrent.DataSource).Table.DataSet;

			_error = PaymentManager.SaveExchangeRates(exchangeRateSet);

			if (_error.Length > 0) ShowError(_error);
		}  // End of SaveChanges

		//
		// Form Events
		//

		private void ExchangeRates_Load(object sender, System.EventArgs e)
		{
			// Initial Form Load
			try
			{
				Function = "Exchange Rate Screen: Form Load";
				Wait();

				this.ttExchangeRates.SetToolTip(btnEnter, GetResource("TT_ADDROW"));
				this.ttExchangeRates.SetToolTip(btnSave, GetResource("TT_SAVE"));
				this.ttExchangeRates.SetToolTip(dgCurrent, GetResource("TT_ROWEDIT"));
				this.ttExchangeRates.SetToolTip(dgHistory, GetResource("TT_ROWCURRENT"));

				this.LoadCurrent();

#region Get drop down data
				//Get the required static data for the drop down lists
			
				XmlUtilities xml = new XmlUtilities();
				XmlDocument dropDowns = new XmlDocument();
				dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

				if (StaticData.Tables[TN.Currency] == null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.PayMethod, new string[]{CAT.FintransPayMethod, "L"}));

				if (dropDowns.DocumentElement.ChildNodes.Count > 0)
				{
					DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out _error);
					if (_error.Length > 0)
						ShowError(_error);
					else
					{
						foreach (DataTable dt in ds.Tables)
						{
							StaticData.Tables[dt.TableName] = dt;
						}
					}
				}

#endregion

				// Take a copy of the PayMethod table and delete everthing except foreign currencies
				DataTable dtPayMethod = ((DataTable)StaticData.Tables[TN.PayMethod]).Copy();

				for (int i = dtPayMethod.Rows.Count -1; i >= 0; i--)
				{
					if (PayMethod.IsForeignCurrencyPayMethod(System.Convert.ToInt16(dtPayMethod.Rows[i][CN.Code])) == false)
					{
						// Not a Foreign Currency Pay Method so delete
						dtPayMethod.Rows.RemoveAt(i);
					}
				}

				if (dtPayMethod.Rows.Count == 0)
				{
					ShowInfo("M_NOEXCHANGERATE");
                    //CloseTab(); //Commented UAT 605 - Error in Exchange Rate Maintenance Screen
				}
				else
				{
					// A blank currency option should be in the combo box
					if ((string)dtPayMethod.Rows[0][CN.CodeDescript] != "")
					{
						DataRow row = dtPayMethod.NewRow();
						row[CN.Code] = "";
						row[CN.CodeDescript] = "";
						dtPayMethod.Rows.InsertAt(row,0);
					}

					this.drpCurrency1.DataSource = dtPayMethod;
					this.drpCurrency1.DisplayMember = CN.CodeDescription;
					this.drpCurrency2.DataSource = dtPayMethod;
					this.drpCurrency2.DisplayMember = CN.CodeDescription;
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
				((MainForm)this.FormRoot).statusBar1.Text = "";
			}
			finally
			{
				StopWait();
			}

		}  // End of ExchangeRates_Load

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			// Clear Form Button - prompt to save changes
			try
			{
				Function = "Exchange Rate Screen: Click Clear button";
				Wait();

				// Check whether to save changes
				this.PromptChanges("M_SAVECLEAR");
				this.ResetScreen();
				this.LoadCurrent();
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
			}
		}  // End of btnClear_Click

		private void btnExit_Click(object sender, System.EventArgs e)
		{
			// Exit Form Button - prompt to save changes
			try
			{
				Function = "Exchange Rate Screen: Click Exit button";
				Wait();

				// Check whether to save changes
				this.PromptChanges("M_SAVECHANGES");

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
		}  // End of btnExit_Click

		private void tcRates_Click(object sender, System.EventArgs e)
		{
			// Set up when entering a different tab
			try
			{
				Function = "Exchange Rate Screen: Click Tab";
				Wait();

				((MainForm)this.FormRoot).statusBar1.Text = "";
				if (this.tcRates.SelectedIndex == 0) this.drpCurrency1.Focus();
				else this.drpCurrency2.Focus();

			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
			}
		}  // End of tcRates_Click

		private void drpCurrency1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			// Validate the Currency
			try
			{
				Function = "Exchange Rate Screen: Validate Currency";
				Wait();

				this.SetEnterButton();
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
			}
		}  // End of drpCurrency1_SelectedIndexChanged

		private void txtExchangeRate_Leave(object sender, System.EventArgs e)
		{
			// Validate the Exchange Rate
			try
			{
				Function = "Exchange Rate Screen: Validate Exchange Value";

				if (!this._userChanged ||
					!this.ValidMoneyField(this.txtExchangeRate, out _curExchangeRate, RateFormat.DecimalPlaces)) return;

				Wait();
				if (_curExchangeRate < 0.0M)
				{
					this.btnEnter.Enabled = false;
					ShowInfo("M_POSITIVENUM");
					this.txtExchangeRate.Focus();
				}
				else this.SetEnterButton();
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
			}

		}  // End of txtExchangeRate_Leave

		private void btnEnter_Click(object sender, System.EventArgs e)
		{
			// Add a new rate to the current list displayed
			// but do NOT save to the DB
			try
			{
				Function = "Exchange Rate Screen: Click Enter button";
				Wait();
				
				// Check whether to replace a row in edit for the same currency
				DataView currentView = (DataView)dgCurrent.DataSource;
				DataRowView rowViewFound = null;
				foreach (DataRowView rowView in currentView)
				{
					if ((string)rowView[CN.Status] == RateStatus.Edit &&
						(string)rowView[CN.Code] == ((DataRowView)this.drpCurrency2.SelectedItem)[CN.Code].ToString())
					{
						rowViewFound = rowView;
					}
				}

				if (rowViewFound != null)
				{
					// Update the matching edit row found
					rowViewFound[CN.ExchangeRate] = this._curExchangeRate;
					rowViewFound[CN.DateFrom] = StaticDataManager.GetServerDateTime();
				}
				else
				{
					// Add the new row as an edit row
					currentView.AllowNew = true;
					DataRowView rowView = currentView.AddNew();
					rowView[CN.Code] = ((DataRowView)this.drpCurrency2.SelectedItem)[CN.Code].ToString();
					rowView[CN.Currency] = ((DataRowView)this.drpCurrency2.SelectedItem)[CN.CodeDescript].ToString();
					rowView[CN.ExchangeRate] = this._curExchangeRate;
					rowView[CN.DateFrom] = StaticDataManager.GetServerDateTime();
					rowView[CN.EmployeeNo] = Credential.UserId.ToString();
					rowView[CN.EmployeeName] = Credential.Name;
					rowView[CN.Status] = RateStatus.Edit;
					rowView.EndEdit();
					currentView.AllowNew = false;
				}

				// Reset the input fields
				this.drpCurrency2.SelectedIndex = 0;
				this.txtExchangeRate.Text = "";
				this._curExchangeRate = -1.0M;

				// Enable the save button
				this.btnSave.Enabled = true;

				// Set focus (do not want it on the Save button)
				this.drpCurrency2.Focus();

			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
			}

		}  // End of btnEnter_Click

		private void dgCurrent_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// Current rate context menu to remove a new row not yet saved on the DB
			try
			{
				Function = "Exchange Rate Screen: Right click on Exchange Rate List";
				Wait();

				int index = dgCurrent.CurrentRowIndex;
				DataView currentView = (DataView)dgCurrent.DataSource;

				if (index >= 0)
				{
					if (e.Button == MouseButtons.Right && (string)currentView[index][CN.Status] == RateStatus.Edit)
					{
						DataGrid ctl = (DataGrid)sender;

						MenuCommand m1 = new MenuCommand(GetResource("P_REMOVE"));
						m1.Click += new System.EventHandler(this.cmenuRemove_Click);
						
						PopupMenu popup = new PopupMenu();
						popup.Animate = Animate.Yes;
						popup.AnimateStyle = Animation.SlideHorVerPositive;

						popup.MenuCommands.AddRange(new MenuCommand[] {m1});
						MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
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
		}  // End of dgCurrent_MouseUp

		private void cmenuRemove_Click(object sender, System.EventArgs e)
		{
			// Remove a new rate not yet saved on the DB
			try
			{
				Function = "Exchange Rate Screen: Remove context menu";
				Wait();

				int index = dgCurrent.CurrentRowIndex;
				DataView currentView = (DataView)dgCurrent.DataSource;

				if (index >= 0)
				{
					if ((string)currentView[index][CN.Status] == RateStatus.Edit)
					{
						currentView[index].Delete();
					}
				}

				// Disable Save button if no changes in list
				this.btnSave.Enabled = this.CanSave();
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
			}
		}  // End of cmenuRemove_Click

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			// Save the new rows added by the user
			try
			{
				Function = "Exchange Rate Screen: Click Save button";
				Wait();

				this.SaveChanges();
				this.ResetScreen();
				this.LoadCurrent();
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
			}
		}  // End of btnSave_Click

		private void btnLoad_Click(object sender, System.EventArgs e)
		{
			// Load the Current and History exchange rates for the view tab
			try
			{
				Function = "Exchange Rate Screen: Click Load button";
				Wait();

				this.LoadHistory();
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
			}
		}  // End of btnLoad_Click

	}
}

