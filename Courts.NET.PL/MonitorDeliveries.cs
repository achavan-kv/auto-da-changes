using System;
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
	/// Summary description for NewdeliveriesDeposits.
	/// </summary>
	public class MonitorDeliveries : CommonForm
	{
		private string error = "";
		private bool _userChanged;
		private DateTime _serverDate;

		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private System.Windows.Forms.ComboBox drpWarehouseNo;
		private System.Windows.Forms.Label lBranch;
		private System.Windows.Forms.DataGrid dgDeliveries;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.DateTimePicker dtDateTo;
		private System.Windows.Forms.DateTimePicker dtDateFrom;
		private System.Windows.Forms.Label lTo;
		private System.Windows.Forms.Label lFrom;
		private System.Windows.Forms.ToolTip ttdeliveries;
		private System.Windows.Forms.ComboBox drpLoadOperand;
		private System.Windows.Forms.GroupBox gbSelections;
		private System.Windows.Forms.CheckBox cbSecuritised;
		private System.Windows.Forms.CheckBox cbNonSecuritised;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox gbDeliveries;
        public System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.NumericUpDown bufferNo;
        private System.Windows.Forms.TextBox tbTotalValue;
        private System.Windows.Forms.Label label2;
		private System.ComponentModel.IContainer components;


		public MonitorDeliveries(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
		}

		public MonitorDeliveries(Form root, Form parent)
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MonitorDeliveries));
			this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
			this.gbDeliveries = new System.Windows.Forms.GroupBox();
			this.tbTotalValue = new System.Windows.Forms.TextBox();
			this.bufferNo = new System.Windows.Forms.NumericUpDown();
			this.btnExcel = new System.Windows.Forms.Button();
			this.gbSelections = new System.Windows.Forms.GroupBox();
			this.cbSecuritised = new System.Windows.Forms.CheckBox();
			this.cbNonSecuritised = new System.Windows.Forms.CheckBox();
			this.btnLoad = new System.Windows.Forms.Button();
			this.dtDateTo = new System.Windows.Forms.DateTimePicker();
			this.dtDateFrom = new System.Windows.Forms.DateTimePicker();
			this.lTo = new System.Windows.Forms.Label();
			this.lFrom = new System.Windows.Forms.Label();
			this.lBranch = new System.Windows.Forms.Label();
			this.drpWarehouseNo = new System.Windows.Forms.ComboBox();
			this.dgDeliveries = new System.Windows.Forms.DataGrid();
			this.drpLoadOperand = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.ttdeliveries = new System.Windows.Forms.ToolTip(this.components);
			this.gbDeliveries.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bufferNo)).BeginInit();
			this.gbSelections.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgDeliveries)).BeginInit();
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
			this.menuExit.Click += new System.EventHandler(this.fileExit_Click);
			// 
			// gbDeliveries
			// 
			this.gbDeliveries.BackColor = System.Drawing.SystemColors.Control;
			this.gbDeliveries.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.tbTotalValue,
																					   this.bufferNo,
																					   this.btnExcel,
																					   this.gbSelections,
																					   this.btnLoad,
																					   this.dtDateTo,
																					   this.dtDateFrom,
																					   this.lTo,
																					   this.lFrom,
																					   this.lBranch,
																					   this.drpWarehouseNo,
																					   this.dgDeliveries,
																					   this.drpLoadOperand,
																					   this.label1,
																					   this.label2});
			this.gbDeliveries.Location = new System.Drawing.Point(8, 0);
			this.gbDeliveries.Name = "gbDeliveries";
			this.gbDeliveries.Size = new System.Drawing.Size(776, 480);
			this.gbDeliveries.TabIndex = 0;
			this.gbDeliveries.TabStop = false;
			// 
			// tbTotalValue
			// 
			this.tbTotalValue.Location = new System.Drawing.Point(640, 448);
			this.tbTotalValue.Name = "tbTotalValue";
			this.tbTotalValue.TabIndex = 50;
			this.tbTotalValue.Text = "";
			this.tbTotalValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// bufferNo
			// 
			this.bufferNo.Location = new System.Drawing.Point(208, 32);
			this.bufferNo.Maximum = new System.Decimal(new int[] {
																	 99999999,
																	 0,
																	 0,
																	 0});
			this.bufferNo.Name = "bufferNo";
			this.bufferNo.Size = new System.Drawing.Size(112, 20);
			this.bufferNo.TabIndex = 49;
			// 
			// btnExcel
			// 
			this.btnExcel.Enabled = false;
			this.btnExcel.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnExcel.Image")));
			this.btnExcel.Location = new System.Drawing.Point(712, 64);
			this.btnExcel.Name = "btnExcel";
			this.btnExcel.Size = new System.Drawing.Size(32, 32);
			this.btnExcel.TabIndex = 48;
			this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
			// 
			// gbSelections
			// 
			this.gbSelections.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.cbSecuritised,
																					   this.cbNonSecuritised});
			this.gbSelections.Location = new System.Drawing.Point(32, 96);
			this.gbSelections.Name = "gbSelections";
			this.gbSelections.Size = new System.Drawing.Size(712, 56);
			this.gbSelections.TabIndex = 43;
			this.gbSelections.TabStop = false;
			this.gbSelections.Text = "Accounts to Include";
			// 
			// cbSecuritised
			// 
			this.cbSecuritised.Location = new System.Drawing.Point(8, 24);
			this.cbSecuritised.Name = "cbSecuritised";
			this.cbSecuritised.Size = new System.Drawing.Size(80, 16);
			this.cbSecuritised.TabIndex = 44;
			this.cbSecuritised.Text = "Securitised";
			// 
			// cbNonSecuritised
			// 
			this.cbNonSecuritised.Location = new System.Drawing.Point(104, 24);
			this.cbNonSecuritised.Name = "cbNonSecuritised";
			this.cbNonSecuritised.Size = new System.Drawing.Size(104, 16);
			this.cbNonSecuritised.TabIndex = 45;
			this.cbNonSecuritised.Text = "Non Securitised";
			// 
			// btnLoad
			// 
			this.btnLoad.Location = new System.Drawing.Point(640, 72);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(48, 23);
			this.btnLoad.TabIndex = 40;
			this.btnLoad.Text = "Run";
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// dtDateTo
			// 
			this.dtDateTo.CustomFormat = "dd MMM yyyy";
			this.dtDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtDateTo.Location = new System.Drawing.Point(480, 64);
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
			this.dtDateFrom.Location = new System.Drawing.Point(208, 64);
			this.dtDateFrom.Name = "dtDateFrom";
			this.dtDateFrom.Size = new System.Drawing.Size(112, 20);
			this.dtDateFrom.TabIndex = 38;
			this.dtDateFrom.Tag = "";
			this.dtDateFrom.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
			this.dtDateFrom.ValueChanged += new System.EventHandler(this.dtDateFrom_ValueChanged);
			// 
			// lTo
			// 
			this.lTo.Location = new System.Drawing.Point(344, 72);
			this.lTo.Name = "lTo";
			this.lTo.Size = new System.Drawing.Size(136, 16);
			this.lTo.TabIndex = 37;
			this.lTo.Text = "Delivery Authorisation To";
			// 
			// lFrom
			// 
			this.lFrom.Location = new System.Drawing.Point(32, 72);
			this.lFrom.Name = "lFrom";
			this.lFrom.Size = new System.Drawing.Size(144, 16);
			this.lFrom.TabIndex = 36;
			this.lFrom.Text = "Delivery Authorisation From";
			// 
			// lBranch
			// 
			this.lBranch.Location = new System.Drawing.Point(368, 40);
			this.lBranch.Name = "lBranch";
			this.lBranch.Size = new System.Drawing.Size(104, 16);
			this.lBranch.TabIndex = 2;
			this.lBranch.Text = "Branch/Warehouse";
			// 
			// drpWarehouseNo
			// 
			this.drpWarehouseNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpWarehouseNo.Location = new System.Drawing.Point(480, 32);
			this.drpWarehouseNo.Name = "drpWarehouseNo";
			this.drpWarehouseNo.Size = new System.Drawing.Size(88, 21);
			this.drpWarehouseNo.TabIndex = 0;
			this.drpWarehouseNo.SelectionChangeCommitted += new System.EventHandler(this.drpWarehouseNo_SelectionChangeCommitted);
			// 
			// dgDeliveries
			// 
			this.dgDeliveries.DataMember = "";
			this.dgDeliveries.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgDeliveries.Location = new System.Drawing.Point(32, 168);
			this.dgDeliveries.Name = "dgDeliveries";
			this.dgDeliveries.ReadOnly = true;
			this.dgDeliveries.Size = new System.Drawing.Size(712, 272);
			this.dgDeliveries.TabIndex = 0;
			this.dgDeliveries.TabStop = false;
			this.dgDeliveries.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgDeliveries_MouseUp);
			// 
			// drpLoadOperand
			// 
			this.drpLoadOperand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpLoadOperand.Items.AddRange(new object[] {
																">",
																"="});
			this.drpLoadOperand.Location = new System.Drawing.Point(136, 32);
			this.drpLoadOperand.Name = "drpLoadOperand";
			this.drpLoadOperand.Size = new System.Drawing.Size(48, 21);
			this.drpLoadOperand.TabIndex = 0;
			this.drpLoadOperand.SelectionChangeCommitted += new System.EventHandler(this.drpLoadOperand_SelectionChangeCommitted);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(56, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Load Number";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(576, 456);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Total Value";
			// 
			// MonitorDeliveries
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(792, 477);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.gbDeliveries});
			this.Name = "MonitorDeliveries";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Monitor Deliveries";
			this.Load += new System.EventHandler(this.MonitorDeliveries_Load);
			this.gbDeliveries.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.bufferNo)).EndInit();
			this.gbSelections.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgDeliveries)).EndInit();
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
            decimal selectedBufferNo = this.bufferNo.Value;
			int selectedBranch = this.drpWarehouseNo.SelectedIndex;
			int selectedOperand = this.drpLoadOperand.SelectedIndex;						
			DateTime dateFrom = this.dtDateFrom.Value;
			DateTime dateTo = this.dtDateTo.Value;
            bool cbNonSecuritisedChecked = cbNonSecuritised.Checked;
            bool cbSecuritisedChecked = cbSecuritised.Checked;

			// Clear all controls
			ClearControls(this.Controls);

			if (setDefault)
			{
				// Initial custom settings
				// Set to this branch for the last 24 hours
                bufferNo.Value = 0;
				int i = this.drpWarehouseNo.FindStringExact(Config.BranchCode);
				this.drpWarehouseNo.SelectedIndex = i >= 0 ? i : 0;
				this.drpLoadOperand.SelectedIndex = 0;		
				this.dtDateFrom.Value = _serverDate.AddDays(-1);
				this.dtDateTo.Value = _serverDate;
                cbNonSecuritised.Checked = true;
                cbSecuritised.Checked = true;
			}
			else
			{
				// Restore search fields
                bufferNo.Value = selectedBufferNo;
				this.drpWarehouseNo.SelectedIndex = selectedBranch;
				this.drpLoadOperand.SelectedIndex = selectedOperand;					
				this.dtDateFrom.Value = dateFrom;
				this.dtDateTo.Value = dateTo;
                cbNonSecuritised.Checked = cbNonSecuritisedChecked;
                cbSecuritised.Checked = cbSecuritisedChecked;
			}

			// Enable the search fields and disable the deliveries list
			this.drpWarehouseNo.Enabled = true;
            bufferNo.Enabled = true;
            this.drpLoadOperand.Enabled = true;
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
			this.dtDateTo.MaxDate = this._serverDate.AddDays(1).AddSeconds(-1);

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
            StringCollection WarehouseNos = new StringCollection(); 	

            foreach(DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
            {
                WarehouseNos.Add(Convert.ToString(row[CN.BranchNo]));
            }

            drpWarehouseNo.DataSource = WarehouseNos;
            drpWarehouseNo.Text = Config.BranchCode;
		}

		private bool LoadDeliveries()
		{
			bool deliveriesFound = false;
			string statusText = GetResource("M_DELIVERIESZERO");
			DataSet deliveriesSet = null;
			DataView deliveriesListView = null;

			//RRD 18/08/05 - End Date needs to have Hours:Min:Sec set to 23:59:59
			//dtDateTo.Value = dtDateTo.Value.AddDays(1).AddSeconds(-1);

			// Abort if the Start Date is after the End Date
			if (this.dtDateFrom.Value > this.dtDateTo.Value)
			{
				ShowInfo("M_STARTDATEAFTEREND");
				return false;
			}

			((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_LOADINGDATA");
            int includeNonSec = cbNonSecuritised.Checked ? 1 : 0;
            int includeSec = cbSecuritised.Checked ? 1 : 0;

            string operand = drpLoadOperand.Items[drpLoadOperand.SelectedIndex].ToString();

			deliveriesSet = AccountManager.GetOutstandingDeliveries(
                (Int32)bufferNo.Value,
                Int32.Parse(drpWarehouseNo.SelectedValue.ToString()),
                this.dtDateFrom.Value,
                this.dtDateTo.Value,
                includeSec,
                includeNonSec,
                operand,
                out error);

			if(error.Length > 0)
			{
				ShowError(error);
				return false;
			}

			foreach (DataTable deliveriesDetails in deliveriesSet.Tables)
			{
				if (deliveriesDetails.TableName == TN.MonitorDeliveries)
				{
					// Display list of deliveriess
					deliveriesFound = (deliveriesDetails.Rows.Count > 0);
					statusText = deliveriesDetails.Rows.Count + GetResource("M_DELIVERIESLISTED");
                    btnExcel.Enabled = deliveriesFound;

					deliveriesListView = new DataView(deliveriesDetails);
					deliveriesListView.AllowNew = false;
					dgDeliveries.DataSource = deliveriesListView;
                    deliveriesListView.Sort = CN.AcctNo + " ASC ";
					
					if (dgDeliveries.TableStyles.Count == 0)
					{
						DataGridTableStyle tabStyle = new DataGridTableStyle();
						tabStyle.MappingName = deliveriesListView.Table.TableName;

						dgDeliveries.TableStyles.Clear();
						dgDeliveries.TableStyles.Add(tabStyle);
						dgDeliveries.DataSource = deliveriesListView;

                        // Displayed columns
                        tabStyle.GridColumnStyles[CN.AcctNo].Width = 90;
                        tabStyle.GridColumnStyles[CN.AcctNo].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.AcctNo].HeaderText = GetResource("T_ACCTNO");
                        tabStyle.GridColumnStyles[CN.DelOrColl].Width = 60;
                        tabStyle.GridColumnStyles[CN.DelOrColl].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.DelOrColl].HeaderText = GetResource("T_DELORCOLL");
                        tabStyle.GridColumnStyles[CN.DateAuthorised].Width = 90;
                        tabStyle.GridColumnStyles[CN.DateAuthorised].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.DateAuthorised].HeaderText = GetResource("T_DATEAUTH");
                        tabStyle.GridColumnStyles[CN.BuffNo].Width = 70;
                        tabStyle.GridColumnStyles[CN.BuffNo].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.BuffNo].HeaderText = GetResource("T_DELNOTENO");
						// 68009 RD 22/03/2006 Removed as not in CR and user do not require
                        //tabStyle.GridColumnStyles[CN.StockLocn].Width = 90;
                        //tabStyle.GridColumnStyles[CN.StockLocn].ReadOnly = true;
                        //tabStyle.GridColumnStyles[CN.StockLocn].HeaderText = GetResource("T_STOCKLOCN");
                        tabStyle.GridColumnStyles[CN.ItemNo].Width = 70;
                        tabStyle.GridColumnStyles[CN.ItemNo].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.ItemNo].HeaderText = GetResource("T_ITEMNO");

                        //IP - 07/07/11 - CR1254 - RI - #4018
                        if (Convert.ToBoolean(Country[CountryParameterNames.RIDispCourtsCode]))
                        {
                            tabStyle.GridColumnStyles[CN.CourtsCode].Width = 70;
                            tabStyle.GridColumnStyles[CN.CourtsCode].ReadOnly = true;
                            tabStyle.GridColumnStyles[CN.CourtsCode].HeaderText = GetResource("T_COURTSCODE");
                        }
                        else
                        {
                            tabStyle.GridColumnStyles[CN.CourtsCode].Width = 0;
                        }


                        tabStyle.GridColumnStyles[CN.OrdVal].Width = 70;
                        tabStyle.GridColumnStyles[CN.OrdVal].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.OrdVal].HeaderText = GetResource("T_VALUE");
                        tabStyle.GridColumnStyles[CN.OrdVal].Alignment = HorizontalAlignment.Right;
                        ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.OrdVal]).Format = DecimalPlaces;
                        tabStyle.GridColumnStyles[CN.Total].Width = 0;
                        tabStyle.GridColumnStyles[CN.DateDNPrinted].Width = 90;
                        tabStyle.GridColumnStyles[CN.DateDNPrinted].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.DateDNPrinted].HeaderText = GetResource("T_DATEDNPRINTED");
                        tabStyle.GridColumnStyles[CN.PrintedBy].Width = 60;
                        tabStyle.GridColumnStyles[CN.PrintedBy].ReadOnly = true;
                        tabStyle.GridColumnStyles[CN.PrintedBy].HeaderText = GetResource("T_PRINTEDBY");
                    }
				}
			}

			((MainForm)this.FormRoot).statusBar1.Text = statusText;

            if (deliveriesFound)
            {
                tbTotalValue.Text = ((decimal)deliveriesSet.Tables[TN.MonitorDeliveries].Rows[0][CN.Total]).ToString(DecimalPlaces);
            }
            else 
            {
                tbTotalValue.Text = ((decimal)0).ToString(DecimalPlaces);
            }
            dgDeliveries.Focus();

			return deliveriesFound;
		}

		//
		// Events
		//

		private void MonitorDeliveries_Load(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Monitor Outstanding Deliveries Screen: Form Load";
				Wait();

				this.LoadStaticData();
                if (dgDeliveries.VisibleRowCount == 0)
                {
                    this.ResetScreen(true);
                }
                // Initial focus
                if (dgDeliveries.VisibleRowCount == 0)
                {
                    this.bufferNo.Focus();
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

		private void btnLoad_Click(object sender, System.EventArgs e)
		{
			// Load the deliveriess for this branch
			try
			{
				Function = "Monitor Outstanding Deliveries Screen: Load Branch";
				Wait();

				LoadDeliveries();
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
				Function = "Monitor Outstanding Deliveries Screen: Date From changed";
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
				Function = "Monitor Outstanding Deliveries Screen: Date To changed";
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

                if(dgDeliveries.CurrentRowIndex >= 0)
                {
                    DataView dv = (DataView)dgDeliveries.DataSource;

                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*" ;
                    save.Title = "Save Deliveries";
                    save.CreatePrompt = true;

                    if(save.ShowDialog() == DialogResult.OK)
                    {
                        path = save.FileName;
                        FileInfo fi = new FileInfo(path);
                        FileStream fs = fi.OpenWrite();
			
                        //Write heading line..
                        string line = string.Empty;
                        if (Convert.ToBoolean(Country[CountryParameterNames.RIDispCourtsCode]))                                 //IP - 20/07/11 - #4337 - Export Courts Code
                        {
                            line = line + GetResource("T_ACCTNO") + comma +
                               GetResource("T_DELORCOLL") + comma +
                               GetResource("T_DATEAUTH") + comma +
                               GetResource("T_LOADNO") + comma +
                                // 68405 jec 16/08/06  GetResource("T_STOCKLOCN") + comma +
                               GetResource("T_ITEMNO") + comma +
                               GetResource("T_COURTSCODE") + comma +
                               GetResource("T_VALUE") + comma +
                               GetResource("T_DATEDNPRINTED") + comma +
                               GetResource("T_PRINTEDBY") + Environment.NewLine + Environment.NewLine;
                        }
                        else
                        {

                            line = line + GetResource("T_ACCTNO") + comma +
                                GetResource("T_DELORCOLL") + comma +
                                GetResource("T_DATEAUTH") + comma +
                                GetResource("T_LOADNO") + comma +
                                // 68405 jec 16/08/06  GetResource("T_STOCKLOCN") + comma +
                                GetResource("T_ITEMNO") + comma +
                                GetResource("T_VALUE") + comma +
                                GetResource("T_DATEDNPRINTED") + comma +
                                GetResource("T_PRINTEDBY") + Environment.NewLine + Environment.NewLine;
                        }

                        byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
                        fs.Write(blob,0,blob.Length);
			
                        foreach(DataRowView row in dv)
                        {					
                            line = string.Empty;

                            if (Convert.ToBoolean(Country[CountryParameterNames.RIDispCourtsCode]))                                  //IP - 20/07/11 - #4337 - Export Courts Code
                            {
                                line += "'" + row[CN.AcctNo].ToString().Replace(",", "") + "'" + comma +
                                  row[CN.DelOrColl].ToString().Replace(",", "") + comma +
                                  Convert.ToString(row[CN.DateAuthorised]).Replace(",", "") + comma +
                                  row[CN.BuffNo].ToString().Replace(",", "") + comma +
                                    // 68405  jec 16/08/06        row[CN.StockLocn].ToString().Replace(",","") + comma +
                                  row[CN.ItemNo].ToString().Replace(",", "") + comma +
                                  row[CN.CourtsCode].ToString().Replace(",", "") + comma +                              
                                  ((decimal)row[CN.OrdVal]).ToString(DecimalPlaces).Replace(",", "") + comma +
                                  row[CN.DateDNPrinted].ToString().Replace(",", "") + comma +
                                  row[CN.PrintedBy].ToString().Replace(",", "") +
                                  Environment.NewLine;
                            }
                            else
                            {
                                line += "'" + row[CN.AcctNo].ToString().Replace(",", "") + "'" + comma +
                                    row[CN.DelOrColl].ToString().Replace(",", "") + comma +
                                    Convert.ToString(row[CN.DateAuthorised]).Replace(",", "") + comma +
                                    row[CN.BuffNo].ToString().Replace(",", "") + comma +
                                    // 68405  jec 16/08/06        row[CN.StockLocn].ToString().Replace(",","") + comma +
                                    row[CN.ItemNo].ToString().Replace(",", "") + comma +
                                    ((decimal)row[CN.OrdVal]).ToString(DecimalPlaces).Replace(",", "") + comma +
                                    row[CN.DateDNPrinted].ToString().Replace(",", "") + comma +
                                    row[CN.PrintedBy].ToString().Replace(",", "") +
                                    Environment.NewLine;
                            }
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

        private void drpWarehouseNo_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Monitor Outstanding Deliveries Screen: Selected Branch changed";
                Wait();

                this.ResetScreen(false);
                //We may need to change what is displayed or hidden in response
                //to this change in selection so clear the TableStyles so a new
                //entry (and therefore grid layout) will be created.
                dgDeliveries.TableStyles.Clear();
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

        private void drpLoadOperand_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Monitor Outstanding Deliveries Screen: Selected Operand changed";
                Wait();

                this.ResetScreen(false);
                //We may need to change what is displayed or hidden in response
                //to this change in selection so clear the TableStyles so a new
                //entry (and therefore grid layout) will be created.
                dgDeliveries.TableStyles.Clear();
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

        private void fileExit_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Monitor Deliveries Screen: File - Exit";
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
		// 68009 RD 22/03/06 Added option to check account details by right click as was missing
		private void dgDeliveries_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				Function = "dgDeliveries_MouseUp";
				Wait();

				if(dgDeliveries.CurrentRowIndex>=0)
				{
					dgDeliveries.Select(dgDeliveries.CurrentCell.RowNumber);

					if (e.Button == MouseButtons.Right)
					{
						DataGrid ctl = (DataGrid)sender;

						MenuCommand m1 = new MenuCommand(GetResource("P_ACCOUNT_DETAILS"));
						
						m1.Click += new System.EventHandler(this.OnAccountDetails);
						
						PopupMenu popup = new PopupMenu(); 
						popup.MenuCommands.Add(m1);

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
				Function = "End of dgDeliveries_MouseUp";
			}
		}
		/// <summary>
		/// 68009 RD 22/03/06 Added as was missing from the screen.
		/// dgAccounts Context Menu, "Account Details", calls this screen.
		/// Details of the selected account are displayed.
		/// </summary>
		//private void OnAccountDetails(object sender, System.EventArgs e)
		private void OnAccountDetails(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				int index = dgDeliveries.CurrentRowIndex;

				if(index >= 0)
				{	
					string newAcNo = (string)dgDeliveries[dgDeliveries.CurrentRowIndex, 0];  // non-formatted account number
					string str = newAcNo;
					// Show the accounts details screen	
					AccountDetails details = new AccountDetails(newAcNo, FormRoot, this);
					((MainForm)this.FormRoot).AddTabPage(details,7);
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