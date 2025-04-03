using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using STL.PL.WS2;
using STL.PL.WS5;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.EOD;
using System.Web.Services.Protocols;
using System.Xml;
using System.Collections.Specialized;
using mshtml;
using System.Threading;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;

namespace STL.PL
{
	/// <summary>
	/// Summary description for EODUserInterface.
	/// </summary>
	public class EODUserInterface : CommonForm
	{
		private System.Windows.Forms.GroupBox grpCodes;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.GroupBox grpSearch;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.TextBox txtConfigurations;
		private System.Windows.Forms.ComboBox drpConfigurations;
		private System.Windows.Forms.Button btnSetCosacs;
		private System.Windows.Forms.DataGrid dgConfiguration;
		private System.Windows.Forms.DataGrid dgOptions;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Label lStatus;
		private string error = "";
		StringCollection configurations = new StringCollection();
		StringCollection options = new StringCollection();
		private bool staticLoaded = false;

		private DataView dvAvailableOptions;
		private DataView dvRecordedOptions;
		private DataTable dtRecordedOptions;
		private System.Windows.Forms.Button btnUp;
		private System.Windows.Forms.Button btnDown;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.ProgressBar pbInterface;
		private System.Windows.Forms.ProgressBar pbEODRun;
		private System.Windows.Forms.Button btnRun;
		DateTime start = Date.blankDate;
		private System.Windows.Forms.Timer tInterface;
		private System.ComponentModel.IContainer components;

		private int runLength = 0;
		private int totalRunTime = 0;
		private bool processCompleted = false;
		private int optionTimer = 0;
		private System.Windows.Forms.Label lInterfaceProgress;
		private System.Windows.Forms.Label lEODProgress;
		private string optionCode = "";
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.DataGrid dgOnceAdHoc;
		private System.Windows.Forms.DataGrid dgDailyAdHoc;
		private System.Windows.Forms.Label lEmployeeFrom;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.DateTimePicker dtEODStartDate;
		private System.Windows.Forms.ComboBox drpFrequency;
        private Button btnDelete;
        private int optionIndex = 0;
        private bool refresh = true;
        private Button btnFACTOptions;
        private Button btnWTRSetup;
        private bool reRun = false;

        public EODUserInterface(Form root, Form parent)
		{
			InitializeComponent();

			FormRoot = root;
			FormParent = parent;

            dynamicMenus = new Hashtable();
            HashMenus();
            ApplyRoleRestrictions();
		}

        private void HashMenus()
        {
            dynamicMenus[this.Name + ":btnDelete"] = this.btnDelete;
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

		private void LoadStaticData()
		{
            string activeConfiguration = "";
            try
			{
				Function = "LoadStaticData()";
			
				DateTime serverTime = StaticDataManager.GetServerDateTime();
				dtEODStartDate.Value = serverTime;

				XmlUtilities xml = new XmlUtilities();
				XmlDocument dropDowns = new XmlDocument();
				dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
			
				StringCollection options = new StringCollection(); 	
				options.Add("");

				if(StaticData.Tables[TN.EODOptions]==null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.EODOptions, new string[]{"EDC", "L"}));

				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.EODConfigurations, null));

				if(dropDowns.DocumentElement.ChildNodes.Count>0)
				{
					DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out error);
					if(error.Length>0)
						ShowError(error);
					else
					{
						foreach(DataTable dt in ds.Tables)
							StaticData.Tables[dt.TableName] = dt;
					}
				}

				dvAvailableOptions = new DataView((DataTable)StaticData.Tables[TN.EODOptions]);

                // If Behavioural Scoring is automatically applied then don't need the Apply rescore option
                if (Country[CountryParameterNames.BehaveApplyEodImmediate].ToString() == "True")
                {
                    foreach (DataRow row in dvAvailableOptions.Table.Rows)
                    {
                        if (row["code"].ToString() == "BHSRescoreAP")
                        {
                            row.Delete();
                            
                        }

                    }
                }
                dvAvailableOptions.Table.AcceptChanges();
                            
                // Filter data so that only reference values of 0 are visible for all non-Singapore countries
                if (((string)Country["countrycode"]).Trim() != "S")
                {
                    dvAvailableOptions.RowFilter = "reference = 0";
                }
              
                dgOptions.DataSource = dvAvailableOptions;
               
				dgOptions.TableStyles.Clear();
				LoadAvailableOptions();

                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.EODConfigurations]).Rows)
                {
                    options.Add((string)row[TN.ConfigurationName]);
                    if (Convert.ToInt16(row[CN.IsActive]) > 0)
                        activeConfiguration = (string)row[TN.ConfigurationName];
                }

				drpConfigurations.DataSource = options;
                SetUpFrequency();
				LoadAdHocScripts();
				staticLoaded = true;

                int i = drpConfigurations.FindStringExact(activeConfiguration);
                if (i > 0)
                    drpConfigurations.SelectedIndex = i;
                else
                    drpConfigurations_SelectedIndexChanged(this, null);
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of LoadStaticData()";
			}
		}

		private void LoadAvailableOptions()
		{
			try
			{
				if(dgOptions.TableStyles.Count==0)
				{
					DataGridTableStyle tabStyle = new DataGridTableStyle();
					tabStyle.MappingName = dvAvailableOptions.Table.TableName;
					dgOptions.TableStyles.Add(tabStyle);

                    //tabStyle.GridColumnStyles[CN.CodeDescription].Width = 220;
                    tabStyle.GridColumnStyles[CN.CodeDescription].Width = 240; //IP - 22/04/08 - UAT(223) v.5.1
					tabStyle.GridColumnStyles[CN.CodeDescription].HeaderText = "";
					tabStyle.GridColumnStyles[CN.CodeDescription].ReadOnly = true;

					tabStyle.GridColumnStyles[CN.Code].Width = 0;
					tabStyle.GridColumnStyles[CN.Reference].Width = 0;
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EODUserInterface));
            this.grpCodes = new System.Windows.Forms.GroupBox();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.dgConfiguration = new System.Windows.Forms.DataGrid();
            this.dgOptions = new System.Windows.Forms.DataGrid();
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.btnWTRSetup = new System.Windows.Forms.Button();
            this.btnFACTOptions = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.drpFrequency = new System.Windows.Forms.ComboBox();
            this.dtEODStartDate = new System.Windows.Forms.DateTimePicker();
            this.lEmployeeFrom = new System.Windows.Forms.Label();
            this.btnRun = new System.Windows.Forms.Button();
            this.lStatus = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnSetCosacs = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtConfigurations = new System.Windows.Forms.TextBox();
            this.drpConfigurations = new System.Windows.Forms.ComboBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.pbInterface = new System.Windows.Forms.ProgressBar();
            this.pbEODRun = new System.Windows.Forms.ProgressBar();
            this.tInterface = new System.Windows.Forms.Timer(this.components);
            this.lInterfaceProgress = new System.Windows.Forms.Label();
            this.lEODProgress = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgOnceAdHoc = new System.Windows.Forms.DataGrid();
            this.dgDailyAdHoc = new System.Windows.Forms.DataGrid();
            this.grpCodes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgConfiguration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgOptions)).BeginInit();
            this.grpSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgOnceAdHoc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgDailyAdHoc)).BeginInit();
            this.SuspendLayout();
            // 
            // grpCodes
            // 
            this.grpCodes.BackColor = System.Drawing.SystemColors.Control;
            this.grpCodes.Controls.Add(this.btnDown);
            this.grpCodes.Controls.Add(this.btnUp);
            this.grpCodes.Controls.Add(this.btnAdd);
            this.grpCodes.Controls.Add(this.btnRemove);
            this.grpCodes.Controls.Add(this.dgConfiguration);
            this.grpCodes.Controls.Add(this.dgOptions);
            this.grpCodes.Location = new System.Drawing.Point(12, 96);
            this.grpCodes.Name = "grpCodes";
            this.grpCodes.Size = new System.Drawing.Size(776, 208);
            this.grpCodes.TabIndex = 6;
            this.grpCodes.TabStop = false;
            // 
            // btnDown
            // 
            this.btnDown.Enabled = false;
            this.btnDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnDown.Image = ((System.Drawing.Image)(resources.GetObject("btnDown.Image")));
            this.btnDown.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnDown.Location = new System.Drawing.Point(728, 136);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(40, 24);
            this.btnDown.TabIndex = 5;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Enabled = false;
            this.btnUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnUp.Image = ((System.Drawing.Image)(resources.GetObject("btnUp.Image")));
            this.btnUp.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnUp.Location = new System.Drawing.Point(728, 80);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(40, 24);
            this.btnUp.TabIndex = 4;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Enabled = false;
            this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAdd.Location = new System.Drawing.Point(344, 80);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(40, 24);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Enabled = false;
            this.btnRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnRemove.Image = ((System.Drawing.Image)(resources.GetObject("btnRemove.Image")));
            this.btnRemove.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRemove.Location = new System.Drawing.Point(344, 136);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(40, 24);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // dgConfiguration
            // 
            this.dgConfiguration.AllowSorting = false;
            this.dgConfiguration.CaptionText = "Configuration Options";
            this.dgConfiguration.DataMember = "";
            this.dgConfiguration.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgConfiguration.Location = new System.Drawing.Point(416, 16);
            this.dgConfiguration.Name = "dgConfiguration";
            this.dgConfiguration.ReadOnly = true;
            this.dgConfiguration.Size = new System.Drawing.Size(296, 184);
            this.dgConfiguration.TabIndex = 0;
            this.dgConfiguration.CurrentCellChanged += new System.EventHandler(this.dgConfiguration_CurrentCellChanged);
            this.dgConfiguration.DoubleClick += new System.EventHandler(this.dgConfiguration_DoubleClick);
            // 
            // dgOptions
            // 
            this.dgOptions.CaptionText = "Available Options";
            this.dgOptions.DataMember = "";
            this.dgOptions.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgOptions.Location = new System.Drawing.Point(16, 16);
            this.dgOptions.Name = "dgOptions";
            this.dgOptions.ReadOnly = true;
            this.dgOptions.Size = new System.Drawing.Size(296, 184);
            this.dgOptions.TabIndex = 0;
            this.dgOptions.DoubleClick += new System.EventHandler(this.dgOptions_DoubleClick);
            // 
            // grpSearch
            // 
            this.grpSearch.BackColor = System.Drawing.SystemColors.Control;
            this.grpSearch.Controls.Add(this.btnWTRSetup);
            this.grpSearch.Controls.Add(this.btnFACTOptions);
            this.grpSearch.Controls.Add(this.btnDelete);
            this.grpSearch.Controls.Add(this.label2);
            this.grpSearch.Controls.Add(this.drpFrequency);
            this.grpSearch.Controls.Add(this.dtEODStartDate);
            this.grpSearch.Controls.Add(this.lEmployeeFrom);
            this.grpSearch.Controls.Add(this.btnRun);
            this.grpSearch.Controls.Add(this.lStatus);
            this.grpSearch.Controls.Add(this.btnSave);
            this.grpSearch.Controls.Add(this.btnSetCosacs);
            this.grpSearch.Controls.Add(this.label1);
            this.grpSearch.Controls.Add(this.label3);
            this.grpSearch.Controls.Add(this.txtConfigurations);
            this.grpSearch.Controls.Add(this.drpConfigurations);
            this.grpSearch.Location = new System.Drawing.Point(12, 0);
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.Size = new System.Drawing.Size(776, 96);
            this.grpSearch.TabIndex = 5;
            this.grpSearch.TabStop = false;
            // 
            // btnWTRSetup
            // 
            this.btnWTRSetup.Location = new System.Drawing.Point(148, 69);
            this.btnWTRSetup.Name = "btnWTRSetup";
            this.btnWTRSetup.Size = new System.Drawing.Size(137, 23);
            this.btnWTRSetup.TabIndex = 41;
            this.btnWTRSetup.Text = "WeeklyTrading Report";
            this.btnWTRSetup.UseVisualStyleBackColor = true;
            this.btnWTRSetup.Click += new System.EventHandler(this.btnWTRSetup_Click);
            // 
            // btnFACTOptions
            // 
            this.btnFACTOptions.Location = new System.Drawing.Point(28, 69);
            this.btnFACTOptions.Name = "btnFACTOptions";
            this.btnFACTOptions.Size = new System.Drawing.Size(114, 23);
            this.btnFACTOptions.TabIndex = 40;
            this.btnFACTOptions.Text = "FACT2000 Options";
            this.btnFACTOptions.UseVisualStyleBackColor = true;
            this.btnFACTOptions.Click += new System.EventHandler(this.btnFACTOptions_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnDelete.Location = new System.Drawing.Point(291, 12);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(51, 24);
            this.btnDelete.TabIndex = 38;
            this.btnDelete.Tag = "";
            this.btnDelete.Text = "Delete";
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(360, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 16);
            this.label2.TabIndex = 37;
            this.label2.Text = "Frequency";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // drpFrequency
            // 
            this.drpFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpFrequency.DropDownWidth = 121;
            this.drpFrequency.ItemHeight = 13;
            this.drpFrequency.Location = new System.Drawing.Point(432, 36);
            this.drpFrequency.Name = "drpFrequency";
            this.drpFrequency.Size = new System.Drawing.Size(160, 21);
            this.drpFrequency.TabIndex = 36;
            // 
            // dtEODStartDate
            // 
            this.dtEODStartDate.CustomFormat = "ddd dd MMM yyyy HH:mm:ss";
            this.dtEODStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtEODStartDate.Location = new System.Drawing.Point(432, 13);
            this.dtEODStartDate.Name = "dtEODStartDate";
            this.dtEODStartDate.Size = new System.Drawing.Size(160, 20);
            this.dtEODStartDate.TabIndex = 35;
            this.dtEODStartDate.Tag = "";
            this.dtEODStartDate.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            // 
            // lEmployeeFrom
            // 
            this.lEmployeeFrom.Location = new System.Drawing.Point(364, 21);
            this.lEmployeeFrom.Name = "lEmployeeFrom";
            this.lEmployeeFrom.Size = new System.Drawing.Size(60, 16);
            this.lEmployeeFrom.TabIndex = 34;
            this.lEmployeeFrom.Text = "Start Date";
            this.lEmployeeFrom.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnRun
            // 
            this.btnRun.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRun.Location = new System.Drawing.Point(668, 48);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(96, 25);
            this.btnRun.TabIndex = 28;
            this.btnRun.Tag = "";
            this.btnRun.Text = "Run";
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // lStatus
            // 
            this.lStatus.BackColor = System.Drawing.Color.Lime;
            this.lStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lStatus.Location = new System.Drawing.Point(332, 72);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(176, 16);
            this.lStatus.TabIndex = 27;
            this.lStatus.Text = "CoSACS Is Currently Open";
            this.lStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnSave
            // 
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave.Location = new System.Drawing.Point(618, 16);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 26;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSetCosacs
            // 
            this.btnSetCosacs.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSetCosacs.Location = new System.Drawing.Point(668, 16);
            this.btnSetCosacs.Name = "btnSetCosacs";
            this.btnSetCosacs.Size = new System.Drawing.Size(96, 24);
            this.btnSetCosacs.TabIndex = 25;
            this.btnSetCosacs.Tag = "";
            this.btnSetCosacs.Text = "Close CoSACS";
            this.btnSetCosacs.Click += new System.EventHandler(this.btnSetCosacs_Click);
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 16);
            this.label1.TabIndex = 8;
            this.label1.Text = "Existing Configurations";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(25, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "New Configurations";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtConfigurations
            // 
            this.txtConfigurations.Location = new System.Drawing.Point(132, 43);
            this.txtConfigurations.MaxLength = 12;
            this.txtConfigurations.Name = "txtConfigurations";
            this.txtConfigurations.Size = new System.Drawing.Size(136, 20);
            this.txtConfigurations.TabIndex = 6;
            this.txtConfigurations.TextChanged += new System.EventHandler(this.txtConfigurations_TextChanged);
            // 
            // drpConfigurations
            // 
            this.drpConfigurations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpConfigurations.DropDownWidth = 121;
            this.drpConfigurations.ItemHeight = 13;
            this.drpConfigurations.Location = new System.Drawing.Point(132, 13);
            this.drpConfigurations.Name = "drpConfigurations";
            this.drpConfigurations.Size = new System.Drawing.Size(136, 21);
            this.drpConfigurations.TabIndex = 4;
            this.drpConfigurations.SelectedIndexChanged += new System.EventHandler(this.drpConfigurations_SelectedIndexChanged);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // pbInterface
            // 
            this.pbInterface.Location = new System.Drawing.Point(303, 432);
            this.pbInterface.Name = "pbInterface";
            this.pbInterface.Size = new System.Drawing.Size(481, 16);
            this.pbInterface.Step = 1;
            this.pbInterface.TabIndex = 7;
            this.pbInterface.Visible = false;
            // 
            // pbEODRun
            // 
            this.pbEODRun.Location = new System.Drawing.Point(303, 456);
            this.pbEODRun.Name = "pbEODRun";
            this.pbEODRun.Size = new System.Drawing.Size(481, 16);
            this.pbEODRun.Step = 1;
            this.pbEODRun.TabIndex = 8;
            this.pbEODRun.Visible = false;
            // 
            // lInterfaceProgress
            // 
            this.lInterfaceProgress.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lInterfaceProgress.Location = new System.Drawing.Point(8, 432);
            this.lInterfaceProgress.Name = "lInterfaceProgress";
            this.lInterfaceProgress.Size = new System.Drawing.Size(289, 16);
            this.lInterfaceProgress.TabIndex = 9;
            this.lInterfaceProgress.Visible = false;
            // 
            // lEODProgress
            // 
            this.lEODProgress.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lEODProgress.Location = new System.Drawing.Point(8, 456);
            this.lEODProgress.Name = "lEODProgress";
            this.lEODProgress.Size = new System.Drawing.Size(240, 16);
            this.lEODProgress.TabIndex = 10;
            this.lEODProgress.Text = "EOD Progress.....";
            this.lEODProgress.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgOnceAdHoc);
            this.groupBox1.Controls.Add(this.dgDailyAdHoc);
            this.groupBox1.Location = new System.Drawing.Point(12, 304);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 112);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            // 
            // dgOnceAdHoc
            // 
            this.dgOnceAdHoc.CaptionText = "One Off Ad Hoc Scripts/Executables";
            this.dgOnceAdHoc.DataMember = "";
            this.dgOnceAdHoc.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgOnceAdHoc.Location = new System.Drawing.Point(416, 16);
            this.dgOnceAdHoc.Name = "dgOnceAdHoc";
            this.dgOnceAdHoc.ReadOnly = true;
            this.dgOnceAdHoc.Size = new System.Drawing.Size(296, 88);
            this.dgOnceAdHoc.TabIndex = 8;
            // 
            // dgDailyAdHoc
            // 
            this.dgDailyAdHoc.CaptionText = "Daily Ad Hoc Scripts/Executables";
            this.dgDailyAdHoc.DataMember = "";
            this.dgDailyAdHoc.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgDailyAdHoc.Location = new System.Drawing.Point(16, 16);
            this.dgDailyAdHoc.Name = "dgDailyAdHoc";
            this.dgDailyAdHoc.ReadOnly = true;
            this.dgDailyAdHoc.Size = new System.Drawing.Size(296, 88);
            this.dgDailyAdHoc.TabIndex = 7;
            // 
            // EODUserInterface
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lEODProgress);
            this.Controls.Add(this.lInterfaceProgress);
            this.Controls.Add(this.pbInterface);
            this.Controls.Add(this.grpCodes);
            this.Controls.Add(this.grpSearch);
            this.Controls.Add(this.pbEODRun);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EODUserInterface";
            this.Text = "End of Day";
            this.Enter += new System.EventHandler(this.EODUserInterface_Enter);
            this.grpCodes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgConfiguration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgOptions)).EndInit();
            this.grpSearch.ResumeLayout(false);
            this.grpSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgOnceAdHoc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgDailyAdHoc)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void drpConfigurations_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            DateTime startDate;
            int frequency = 0;

            try
			{
				if(staticLoaded)
				{
					Wait();

					dvAvailableOptions = new DataView((DataTable)StaticData.Tables[TN.EODOptions]);

                    DataSet options = EodManager.GetEodOptionListDetails(drpConfigurations.Text, 
                                                out startDate, out frequency, out error);
					if(error.Length > 0)
						ShowError(error);

					if(options != null)
					{
                        if(!Convert.IsDBNull(startDate))
                            dtEODStartDate.Value = startDate;

                        if (frequency > 0)
                            drpFrequency.SelectedValue = frequency.ToString();
                        else
                            drpFrequency.SelectedIndex = 0;

                        btnRun.Text = "Run";
                        btnAdd.Enabled = true;
                        btnRemove.Enabled = true;
                        btnDown.Enabled = true;
                        btnUp.Enabled = true;
                        dgConfiguration.TableStyles.Clear();
						dtRecordedOptions = options.Tables[0];					
						dgOptions.DataSource = dvAvailableOptions;
						dvRecordedOptions = new DataView(dtRecordedOptions);
						dvRecordedOptions.AllowEdit = true;
						dgConfiguration.DataSource = dvRecordedOptions;
					
						if(dgConfiguration.TableStyles.Count==0)
						{
							DataGridTableStyle tabStyle = new DataGridTableStyle();
							tabStyle.MappingName = dtRecordedOptions.TableName;
							tabStyle.AllowSorting = false;

							dgConfiguration.TableStyles.Add(tabStyle);

							tabStyle.GridColumnStyles[CN.CodeDescription].Width = 200;
							tabStyle.GridColumnStyles[CN.CodeDescription].HeaderText = "";
							tabStyle.GridColumnStyles[CN.CodeDescription].ReadOnly = true;

                            //tabStyle.GridColumnStyles[CN.Status].Width = 50;
                            tabStyle.GridColumnStyles[CN.Status].Width = 57; //IP - 22/04/08 - UAT(223) v.5.1
							tabStyle.GridColumnStyles[CN.Status].HeaderText = "";
							tabStyle.GridColumnStyles[CN.Status].ReadOnly = true;

							tabStyle.GridColumnStyles[CN.SortOrder].Width = 0;
							tabStyle.GridColumnStyles[CN.OptionCode].Width = 0;
							tabStyle.GridColumnStyles[CN.AvgRunTime].Width = 0;
						}

						dgConfiguration.DataSource = dvRecordedOptions;

						FilterCodes();

						dgOptions.TableStyles.Clear();
						LoadAvailableOptions();
                        
						dgOptions.DataSource = dvAvailableOptions;

                        SetFieldBias(drpConfigurations.Text == "DEFAULT", false);
                        btnDelete.Enabled = (drpConfigurations.Text != "DEFAULT");
                        lEODProgress.Visible = false;
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
				Function = "End of LoadStaticData()";
			}
		}

		private void FilterCodes()
		{
            if (dvRecordedOptions.Count > 0)
            {

                string filter = "Code not in (";
                foreach (DataRowView row in dvRecordedOptions)
                {
                    filter += "'" + (string)row[CN.OptionCode] + "',";
                    if ((string)row[CN.OptionCode] == "SUMRYDATAFUL")
                    {
                        filter += "'SUMRYDATA',";
                    }
                    if ((string)row[CN.OptionCode] == "SUMRYDATA")
                    {
                        filter += "'SUMRYDATAFUL',";
                    }
                    if ((string)row[CN.OptionCode] == "BACKAF1")
                    {
                        filter += "'BACKAD1',";
                    }
                    if ((string)row[CN.OptionCode] == "BACKAD1")
                    {
                        filter += "'BACKAF1',";
                    }
                    if ((string)row[CN.OptionCode] == "BACKBF1")
                    {
                        filter += "'BACKBD1',";
                    }
                    if ((string)row[CN.OptionCode] == "BACKBD1")
                    {
                        filter += "'BACKBF1',";
                    }
                }

                filter = filter.Substring(0, filter.Length - 1);

                filter += ")";
                dvAvailableOptions.RowFilter = filter;
                // Filter data so that only reference values of 0 are visible for all non-Singapore countries
                if (((string)Country["countrycode"]).Trim() != "S")
                {
                    filter += " AND reference = 0";
                    dvAvailableOptions.RowFilter = filter;
                }
            }
            else
            {
                dvAvailableOptions.RowFilter = "";
                if (((string)Country["countrycode"]).Trim() != "S")
                {
                    dvAvailableOptions.RowFilter = "reference = 0";
                }
            }
		}

		private void EODUserInterface_Enter(object sender, System.EventArgs e)
		{
            if (refresh)
            {
                SystemStatus();
                LoadStaticData();
            }
		}

		private void btnUp_Click(object sender, System.EventArgs e)
		{
            int x = 0;
            ClearRunStatus();
            for (int i = dvRecordedOptions.Count-1; i >=0 ; i--)
			{
				if (dgConfiguration.IsSelected(i) && i > 0)
				{
					DataRow row = dtRecordedOptions.NewRow();
					
					row[CN.CodeDescription] = dvRecordedOptions[i][CN.CodeDescription];
					row[CN.SortOrder] = dvRecordedOptions[i][CN.SortOrder];
					row[CN.OptionCode] = dvRecordedOptions[i][CN.OptionCode];
					row[CN.AvgRunTime] = dvRecordedOptions[i][CN.AvgRunTime];
					row[CN.Status] = dvRecordedOptions[i][CN.Status];
					
					dtRecordedOptions.Rows.Remove(dvRecordedOptions[i].Row);
					dtRecordedOptions.Rows.InsertAt(row, i - 1);
					dtRecordedOptions.AcceptChanges();

                    x = i - 1;
				}
			}

            if (dvRecordedOptions.Count > 0)
            {
                dgConfiguration.Select(x);
                DataGridCell cell = new DataGridCell(x, 0);
                dgConfiguration.CurrentCell = cell;
            }
		}

		private void btnDown_Click(object sender, System.EventArgs e)
		{
            int x = 0;
            ClearRunStatus();
            for (int i = dvRecordedOptions.Count-1; i >=0 ; i--)
			{
				if (dgConfiguration.IsSelected(i) && i < dvRecordedOptions.Count-1)
				{
					DataRow row = dtRecordedOptions.NewRow();
					
					row[CN.CodeDescription] = dvRecordedOptions[i][CN.CodeDescription];
					row[CN.SortOrder] = dvRecordedOptions[i][CN.SortOrder];
					row[CN.OptionCode] = dvRecordedOptions[i][CN.OptionCode];
					row[CN.AvgRunTime] = dvRecordedOptions[i][CN.AvgRunTime];
					row[CN.Status] = dvRecordedOptions[i][CN.Status];
					
					dtRecordedOptions.Rows.Remove(dvRecordedOptions[i].Row);
					dtRecordedOptions.Rows.InsertAt(row, i + 1);
					dtRecordedOptions.AcceptChanges();

                    x = i + 1;
				}
			}

            if (x != 0)
            {
                dgConfiguration.Select(x);
                DataGridCell cell = new DataGridCell(x, 0);
                dgConfiguration.CurrentCell = cell;
            }
		}


		private void btnAdd_Click(object sender, System.EventArgs e)
		{
            int index = dgOptions.CurrentRowIndex;
            AddReport(index);
		}

        private void AddReport(int index)
        {
            Function = "btnAdd_Click";

            try
            {
                if (drpConfigurations.Text == "DEFAULT" && txtConfigurations.Text.Trim().Length == 0)
                {
                    ShowInfo("M_EODSSAVEDEFAULT");
                    SetFieldBias(true, false);
                }
                else
                {
                    ClearRunStatus();
                    
                    if (index >= 0)
                    {
                        //string optionCode = dgOptions[index, 0].ToString();
                        //remove the selected row from the available options datagrid 
                        //and put it onto the recorded datagrid
                        DataRow row = dtRecordedOptions.NewRow();
                        row[CN.CodeDescription] = dgOptions[index, 1];
                        row[CN.SortOrder] = Convert.ToInt32(0);
                        row[CN.OptionCode] = dgOptions[index, 0];
                        row[CN.AvgRunTime] = Convert.ToInt32(0);
                        row[CN.Status] = "";
                        row[CN.ReRunNo] = Convert.ToInt32(0);       //RI jec 05/04/11
                        dtRecordedOptions.Rows.Add(row);

                        FilterCodes();
                        SetFieldBias(false, false);

                        /* uat282 01/05/08 rdb if rebate report call for create report summary data also
                         * Removing issue as part of UAT 166 AA 18 May2010 - Users can set configuration order any way they want to. 
                        if (optionCode == "REBATEDATA")
                        {
                            int addIndex = -1;

                            DataView dvOptions = (DataView)dgOptions.DataSource;
                            for (int i = 0; i < dvOptions.Count; i++ )
                            {
                                if (dvOptions[i]["code"].ToString() == "SUMRYDATAFUL")
                                {
                                    addIndex = i;
                                    break;
                                }
                            }
                            if(addIndex > -1)
                                AddReport(addIndex);
                        }*/
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function = "End of btnAdd_click";
            }
        }


		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			Function = "btnRemove_Click";
			try
			{
                if (drpConfigurations.Text == "DEFAULT" && txtConfigurations.Text.Trim().Length == 0)
                {
                    ShowInfo("M_EODSSAVEDEFAULT");
                    SetFieldBias(true, false);
                }
                else
                {
                    ClearRunStatus();
                    //remove the selected row from the recorded datagrid and 
                    // add it to the available options datagrid
                    for (int i = dvRecordedOptions.Count - 1; i >= 0; i--)
                    {
                        if (dgConfiguration.IsSelected(i))
                        {
                            dtRecordedOptions.Rows.Remove(dvRecordedOptions[i].Row);
                            dtRecordedOptions.AcceptChanges();
                            FilterCodes();
                        }
                    }
                    SetFieldBias(false, false);
                }
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				Function = "End of btnRemove_Click";
			}
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			bool status = true;
			string configurationName = "";
			Function = "btnSave_Click";
			try
			{
				Wait();

                // checking that the FACT files have been removed successfully. If they exist this will cause a problem running the interface
                string oldfilename = "";
                oldfilename = (string)Country[CountryParameterNames.fact2000driveandddirectory] + "\\bmsfcint.dat";
                string message = "";
                if (File.Exists(oldfilename))
                {
                    message = GetResource("M_EODFACTFILEEXISTS") + oldfilename;
                    MessageBox.Show(message, (string)Messages.List["M_INFORMATION"], MessageBoxButtons.OK, MessageBoxIcon.Information);
                    status = false;
                }

                oldfilename = (string)Country[CountryParameterNames.fact2000driveandddirectory] + "\\bmsffint.dat";

                if (File.Exists(oldfilename))
                {
                    message = GetResource("M_EODFACTFILEEXISTS") + oldfilename;
                    MessageBox.Show(message, (string)Messages.List["M_INFORMATION"], MessageBoxButtons.OK, MessageBoxIcon.Information);
                    status = false;
                }
                oldfilename = (string)Country[CountryParameterNames.fact2000driveandddirectory] + "\\FACTAUTO.TXT";
                if (File.Exists(oldfilename))
                {
                    message = GetResource("M_EODFACTFILEEXISTS") + oldfilename;
                    MessageBox.Show(message, (string)Messages.List["M_INFORMATION"], MessageBoxButtons.OK, MessageBoxIcon.Information);
                    status = false;
                }

                if (drpConfigurations.Text == "DEFAULT" && txtConfigurations.Text.Trim().Length == 0)
                {
                    ShowInfo("M_EODSSAVEDEFAULT");
                    SetFieldBias(true, false);
                    status = false;
                }

                if (txtConfigurations.Text.Trim().ToUpper() == "DEFAULT")
                {
                    ShowInfo("M_EODSSAVEDEFAULT");
                    SetFieldBias(true, false);
                    status = false;
                }

                if(drpConfigurations.Text.Length == 0 && txtConfigurations.Text.Trim().Length == 0)
				{
					errorProvider1.SetError(drpConfigurations, GetResource("M_INVALIDCONFIGURATION"));
					errorProvider1.SetError(txtConfigurations, GetResource("M_INVALIDCONFIGURATION"));
					status = false;
				}
				else
				{
					errorProvider1.SetError(drpConfigurations, "");
					errorProvider1.SetError(txtConfigurations, "");
				}

				if(status)
				{
					if(dvRecordedOptions.Count == 0)
					{
						errorProvider1.SetError(dgConfiguration, GetResource("M_EODINVALIDOPTION"));
						status = false;
					}
					else
					{
						errorProvider1.SetError(dgConfiguration, "");
					}
				}

        
                if (status)
                {
                    if (Convert.ToInt32(drpFrequency.SelectedValue.ToString()) == 0)
                    {
                        errorProvider1.SetError(drpFrequency, GetResource("M_EODINVALIDFREQUENCY"));
                        status = false;
                    }
                    else
                    {
                        errorProvider1.SetError(drpFrequency, "");
                    }
                }

                if (status)
                    status = CheckDiskSpace();

				if(status)
				{
					SortOptions();
                    if (txtConfigurations.Text.Trim().Length > 0)
                        configurationName = txtConfigurations.Text.Trim().Replace(' ', '_'); // LW 70559 - spaces not allowed
                    else
                        configurationName = drpConfigurations.Text;

                    FormatConfigurationName(ref configurationName);

					int freqType = Convert.ToInt32(drpFrequency.SelectedValue.ToString());
					int startDate = GetStartDate();
					int startTime = GetStartTime();

					dtRecordedOptions.AcceptChanges();
					EodManager.SaveEODConfigurationOptions(configurationName, 
						Config.CountryCode, freqType, startDate, startTime, 
						dtRecordedOptions.DataSet, dtEODStartDate.Value, out error);
					if(error.Length > 0)
						ShowError(error);
					else
					{
						Clear();
                        refresh = true;
						EODUserInterface_Enter(this, null);
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
				Function = "End of btnSave_Click";
			}
		}

		private void Clear()
		{
			txtConfigurations.Text = "";
			dvRecordedOptions.Table.Clear();
			dtRecordedOptions.Clear();
		}

		private void InterfaceProgress()
		{
			optionTimer = 0;
			
			pbInterface.Value = 0;
            pbInterface.Maximum = runLength < 0 ? 600 : runLength; //NM - LW 70624 , 30/01/2009

			tInterface.Start();
			tInterface.Interval = 1000; // Fire tick event every second
			tInterface.Tick += new EventHandler(OnTick);

			start = DateTime.Now;
		}

		private void OnTick(object sender, EventArgs e)
		{
            string status = "";

            optionTimer++;

			TimeSpan ts = DateTime.Now.Subtract(start);

			pbInterface.PerformStep();  //Increment progress bar for interface
			pbEODRun.PerformStep(); //Increment progress bar for entire EOD process

			if(optionTimer % 30 == 0)
				status = EodManager.GetEodOptionStatus(drpConfigurations.Text, optionCode, out error);

			processCompleted = (status == EODResult.Pass || status == EODResult.Fail || status == EODResult.Warning);
			if(processCompleted)
			{
				pbInterface.Value = runLength;

				if(status == EODResult.Pass )
					status = GetResource("L_PASS");

                if (status == EODResult.Warning)
                    status = GetResource("L_WARNING");
                

                if (status == EODResult.Fail)       //CR1030 jec allow AD_HOC scripts to fail
                {
                    reRun = true;
                    status = GetResource("L_FAIL");
                }

                //IP - 18/11/2010 - UAT 5.2 - UAT(675)
                //if(optionCode == "ADHOC")
                //    status = "";


				dvRecordedOptions[optionIndex][CN.Status] = status;
				dvRecordedOptions.Table.AcceptChanges();

				tInterface.Stop();

				if(optionIndex < dvRecordedOptions.Count-1)
				{
					optionIndex ++;
					StartEOD(false);
				}
				else
				{
					pbEODRun.Value = totalRunTime;
					tInterface.Dispose();
					btnSetCosacs_Click(this, null);

					lInterfaceProgress.Visible = false;
					pbInterface.Visible = false;
					pbEODRun.Visible = false;

					lEODProgress.Text = GetResource("M_EODRUNCOMPLETE");
                    SetFieldBias(false, true);
                    dvRecordedOptions.RowFilter = "";

                    if (reRun)
                    {
                        EodManager.SetReRunStatus(drpConfigurations.Text, "RERUN", out error);
                        if (error.Length > 0)
                            ShowError(error);
                        else
                        {
                            btnRun.Text = "Restart Failed Run";
                        }
                    }
                    else
                        btnRun.Text = "Run";
				}
			}
		}

		private void btnRun_Click(object sender, System.EventArgs e)
		{
			Function = "btnRun_Click";
			string configurationName = "";
			bool status = true;

			try
			{
                var scStatementsExists = false;     //#12341 - CR11571
                var addScStatementsJob = false;     //#12341 - CR11571
                var scStatementsAdded = false;      //#12341 - CR11571

                foreach (DataRowView row in dvRecordedOptions )
                {
                    if (Convert.ToString(row["OptionCode"]) == "COS FACT") //IP - 12/10/10 - UAT5.4 - UAT(12)
                    {
                       status = CheckFactFiles();
                    }

                    if (Convert.ToString(row["OptionCode"]) == "STStatements") //#12341 - CR11571
                    {
                        scStatementsExists = true;
                    }

                    //if (row["OptionCode"] == "COLLCOMMNS")
                    //{ //Check if recent run of commissions not recent. 
                    //    status = CheckLastCollCommissionRun();
                    //}
                }

                //#12341 - CR11571 - If StoreCard Statements job not in configuration, need to check if it needs
                //to be run. If it does, we need to add this job.
                if (!scStatementsExists)
                {
                    addScStatementsJob = EodManager.CheckToAddSCStatementsOption(out error);

                    if (error.Length > 0)
                    {
                        ShowError(error);
                        status = false;
                    }
                    else
                    {
                        if (addScStatementsJob)
                        {

                            if (DialogResult.Yes == ShowInfo("M_RUNSTORECARDSTATEMENTS".ToString(),new object[]{Country[CountryParameterNames.StoreCardMaxDaysEODRun].ToString()}, MessageBoxButtons.YesNo))
                            {
                                scStatementsAdded = true;

                                DataRow row = dtRecordedOptions.NewRow();

                                row[CN.CodeDescription] = "Store Card Statements"; 
                                row[CN.SortOrder] = 0;
                                row[CN.OptionCode] = "STStatementsAUTO";            //#13820
                                row[CN.AvgRunTime] = 0;
                                row[CN.Status] = "";

                                dtRecordedOptions.Rows.Add(row);
                                dtRecordedOptions.AcceptChanges();
                            }
                        }
                    }
                }
                
                if(dvRecordedOptions.Count > 0 && status)
				{
                    status = CheckDiskSpace();

                    if (status)
                    {
                        if ((bool)Country[CountryParameterNames.SystemOpen])
                        {
                            ShowInfo("M_EODSYSTEMOPEN");
                            status = false;
                        }
                    }

					if(status)
					{
                        if(DialogResult.OK == ShowInfo("M_EODWARNING", MessageBoxButtons.OKCancel))
						{
                            if (!reRun)
                            {
                                Wait();
                                if (drpConfigurations.Text.Length == 0 && txtConfigurations.Text.Trim().Length == 0)
                                    configurationName = "SPECIALRUN";
                                else
                                {
                                    if (txtConfigurations.Text.Trim().Length > 0)
                                        configurationName = txtConfigurations.Text.Trim().Replace(' ', '_'); // LW 70559 - spaces not allowed
                                    else
                                        configurationName = drpConfigurations.Text;
                                }

                                SortOptions();

                                FormatConfigurationName(ref configurationName);

                                int freqType = 1;
                                int startDate = GetStartDate();
                                int startTime = GetStartTime();

                                dtRecordedOptions.AcceptChanges();
                                
                                EodManager.SaveEODConfigurationOptions(configurationName,
                                    Config.CountryCode, freqType, startDate, startTime,
                                    dtRecordedOptions.DataSet, dtEODStartDate.Value, out error);
                                if (error.Length > 0)
                                {
                                    ShowError(error);
                                    status = false;
                                }
                                else
                                {
                                    Clear();
                                    refresh = true;
                                    EODUserInterface_Enter(this, null);
                                    drpConfigurations.Text = configurationName;
                                }
                            }

                            if (status)
                            {
                                optionIndex = 0;
                                totalRunTime = 0;

                                if (reRun)
                                {
                                    dvRecordedOptions.RowFilter = CN.Status + " = 'FAIL'";
                                    ClearRunStatus();
                                }

                                for (int i = 0; i < dvRecordedOptions.Count; i++)
                                    totalRunTime += Convert.ToInt32(dvRecordedOptions[i][CN.AvgRunTime]);

                                pbEODRun.Value = 0;
                                pbEODRun.Maximum = totalRunTime < 0 ? 600 : totalRunTime; //NM - LW 70624 , 30/01/2009

                                StartEOD(true);


                                //#13820 - Commented out
                                //if (scStatementsAdded) //#12341 - CR11571
                                //{
                                //    var indexToDelete = 0;
                                //    var found = false;

                                //    for(int i = 0; i < dtRecordedOptions.Rows.Count; i++)
                                //    {
                                //        if (Convert.ToString(dtRecordedOptions.Rows[i][CN.OptionCode]).Trim() == "STStatements")
                                //        {
                                //            indexToDelete = i;
                                //            found = true;
                                //            break;
                                //        }
                                //    }

                                //    if (found)
                                //    {
                                //        dtRecordedOptions.Rows.RemoveAt(indexToDelete);
                                //        EodManager.RemoveOption(configurationName, "STStatements");
                                //    }

                                //}

                                //IP - 24/01/13 - #11445 - Start the Hub Service when finished running jobs
                                StartService("Blue Bridge Cosacs Hub", 30000);      

                            }
						}
					}
				}
				else
					ShowInfo("M_EODRUNINFO");
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				StopWait();
				Function = "End of btnSave_Click";
			}
		}

        //private bool CheckLastCollCommissionRun()
        //{
        //    int DaysSinceLastCollectionsCommmissionsRun = 1000;
        //    string err;
        //    STL.PL.WS12.WCollectionsManager CollectionsManager = new STL.PL.WS12.WCollectionsManager();
        //    CollectionsManager.CheckCommissionDays(out DaysSinceLastCollectionsCommmissionsRun, out err);
        //    if (DaysSinceLastCollectionsCommmissionsRun < 1000)
        //      if (DialogResult.Cancel == ShowInfo("M_EODWARNING", MessageBoxButtons.OKCancel))
        //          return false;

        //    return true;

        //}

        private bool CheckFactFiles()
        {
            bool status = true;
            // checking that the FACT files have been removed successfully. If they exist this will cause a problem running the interface
            string oldfilename = "";
            oldfilename = (string)Country[CountryParameterNames.fact2000driveandddirectory] + "\\bmsfcint.dat";
            string message = "";
            if (File.Exists(oldfilename))
            {
                message = GetResource("M_EODFACTFILEEXISTS") + oldfilename;
                MessageBox.Show(message, (string)Messages.List["M_INFORMATION"], MessageBoxButtons.OK, MessageBoxIcon.Information);
                status = false;
            }

            oldfilename = (string)Country[CountryParameterNames.fact2000driveandddirectory] + "\\bmsffint.dat";

            if (File.Exists(oldfilename))
            {
                message = GetResource("M_EODFACTFILEEXISTS") + oldfilename;
                MessageBox.Show(message, (string)Messages.List["M_INFORMATION"], MessageBoxButtons.OK, MessageBoxIcon.Information);
                status = false;
            }
            oldfilename = (string)Country[CountryParameterNames.fact2000driveandddirectory] + "\\FACTAUTO.TXT";
            if (File.Exists(oldfilename))
            {
                message = GetResource("M_EODFACTFILEEXISTS") + oldfilename;
                MessageBox.Show(message, (string)Messages.List["M_INFORMATION"], MessageBoxButtons.OK, MessageBoxIcon.Information);
                status = false;
            }

            return status;
        }

        private void SortOptions()
        {
            // uat283 rdb  01/05/08 if Rebate Report in list always make it first option
            // if we only have one row or it is the first row we have no work to do
            // find if the option exists first
            //LW72260 RM the rebate data should only happen before summary and back up, changes so these will always be last.


            if (dvRecordedOptions.Count > 1)
            {
                for (int rowIndex = 0; rowIndex < dvRecordedOptions.Count; rowIndex++)
                {
                    if (dvRecordedOptions[rowIndex][CN.OptionCode].ToString() == "REBATEDATA")
                    {
                        dvRecordedOptions[rowIndex][CN.SortOrder] = dvRecordedOptions.Count + 1;

                    }
                    else if (dvRecordedOptions[rowIndex][CN.OptionCode].ToString() == "SUMRYDATA")
                    {
                        dvRecordedOptions[rowIndex][CN.SortOrder] = dvRecordedOptions.Count + 2;

                    }
                    else if (dvRecordedOptions[rowIndex][CN.OptionCode].ToString() == "SUMRYDATAFUL")
                    {
                        dvRecordedOptions[rowIndex][CN.SortOrder] = dvRecordedOptions.Count + 3;

                    }

                    else if (dvRecordedOptions[rowIndex][CN.OptionCode].ToString() == "BACKAD1")
                    {
                        dvRecordedOptions[rowIndex][CN.SortOrder] = dvRecordedOptions.Count + 4;

                    }
                    else if (dvRecordedOptions[rowIndex][CN.OptionCode].ToString() == "BACKAF1")
                    {
                        dvRecordedOptions[rowIndex][CN.SortOrder] = dvRecordedOptions.Count + 5;

                    }

                    else
                        dvRecordedOptions[rowIndex][CN.SortOrder] = Convert.ToInt16(rowIndex);
                }
            }

        }

		private void StartEOD(bool firstCall)
		{
			runLength = 0;
			optionCode = "";
			processCompleted = false;
			optionTimer = 0;

			lInterfaceProgress.Visible = true;
			lEODProgress.Visible = true;
			pbInterface.Visible = true;
			pbEODRun.Visible = true;

			if(optionIndex >= 0 && optionIndex < dvRecordedOptions.Count )
            {
                lInterfaceProgress.Text = "Running " + (string)dvRecordedOptions[optionIndex][CN.CodeDescription] + ".....";
			    runLength = Convert.ToInt32(dvRecordedOptions[optionIndex][CN.AvgRunTime]);
			    optionCode = (string)dvRecordedOptions[optionIndex][CN.OptionCode];
            }

			InterfaceProgress();

			if(firstCall)
			{
                reRun = false;
                lEODProgress.Text = "EOD Progress.....";
                SetFieldBias(true, true);
                EodManager.EODStartJob(drpConfigurations.Text, Config.CountryCode, out error);
				if(error.Length > 0)
					ShowError(error);
			}
		}

		private void LoadAdHocScripts()
		{
			DataSet scripts = EodManager.GetEODAdHocScripts(out error);
			if(error.Length > 0)
				ShowError(error);
			else
			{
				if(scripts != null)
				{
					dgDailyAdHoc.DataSource = scripts.Tables[0].DefaultView;					
					if(dgDailyAdHoc.TableStyles.Count==0)
					{
						DataGridTableStyle tabStyleDaily = new DataGridTableStyle();
						tabStyleDaily.MappingName = scripts.Tables[0].TableName;
						tabStyleDaily.AllowSorting = false;
						dgDailyAdHoc.TableStyles.Add(tabStyleDaily);

                        //tabStyleDaily.GridColumnStyles[CN.ScriptName].Width = 220;
                        tabStyleDaily.GridColumnStyles[CN.ScriptName].Width = 256; //IP - 22/04/08 - UAT(223) v.5.1
						tabStyleDaily.GridColumnStyles[CN.ScriptName].HeaderText = "";
						tabStyleDaily.GridColumnStyles[CN.ScriptName].ReadOnly = true;
					}

					dgOnceAdHoc.DataSource = scripts.Tables[1].DefaultView;					
					if(dgOnceAdHoc.TableStyles.Count==0)
					{
						DataGridTableStyle tabStyleOnce = new DataGridTableStyle();
						tabStyleOnce.MappingName = scripts.Tables[1].TableName;
						tabStyleOnce.AllowSorting = false;
						dgOnceAdHoc.TableStyles.Add(tabStyleOnce);

                        //tabStyleOnce.GridColumnStyles[CN.ScriptName].Width = 220;
                        tabStyleOnce.GridColumnStyles[CN.ScriptName].Width = 257; //IP - 22/04/08 - UAT(223) v.5.1
						tabStyleOnce.GridColumnStyles[CN.ScriptName].HeaderText = "";
						tabStyleOnce.GridColumnStyles[CN.ScriptName].ReadOnly = true;
					}
				}
			}
		}

		private void btnSetCosacs_Click(object sender, System.EventArgs e)
		{

			string status = "True";

            ArrayList al = new ArrayList(); //IP - 20/09/10 - UAT5.2 Log - UAT(1004)
            bool continueOPenClose = true;  //IP - 20/09/10 - UAT5.2 Log - UAT(1004)

			if((bool)Country[CountryParameterNames.SystemOpen])
				status = "False";

            //IP - 20/09/10 - UAT5.2 Log - UAT(1004) - When closing CoSACS check if there are any open windows
            if (Convert.ToBoolean(Country[CountryParameterNames.SystemOpen]) == true)
            {
                foreach (Crownwood.Magic.Controls.TabPage t in ((MainForm)FormRoot).MainTabControl.TabPages)
                {
                    if (t.Title != "End of Day" && t.Title != "Main")
                    {
                        al.Add(t.Title);
                    }
                }

                if (al.Count > 0)
                {
                    continueOPenClose = false;
                }
                else    //IP - 24/01/13 - #11445 - Stop the Hub Service when closing CoSACS
                {
                    StopService("Blue Bridge Cosacs Hub", 30000);
                    lInterfaceProgress.Visible = true;
                    lInterfaceProgress.Text = "Service Blue Bridge Cosacs Hub has been stopped";
                }
            }

            if (continueOPenClose == true) //IP - 20/09/10 - UAT5.2 Log - UAT(1004)
            {
                ((MainForm)FormRoot).EnableAllMenus(Convert.ToBoolean(status));

                StaticDataManager.SetSystemStatus(Config.CountryCode, status, out error);
                if (error.Length > 0)
                    ShowError(error);
                else
                {
                    DataSet ds = StaticDataManager.GetCountryMaintenanceParameters(Config.CountryCode, out error);
                    if (error.Length > 0)
                        ShowError(error);
                    else
                    {
                        ((MainForm)FormRoot).RootCountry = new CountryParameterCollection(ds.Tables[TN.CountryParameters]);
                        var dsCache = StaticDataManager.GetStcokItemCache(out error);

                        if(error.Length > 0)
                            ShowError(error);
                        else if (dsCache != null && dsCache.Tables.Count > 0)
                        {
                            StockItemCache.Invalidate(dsCache.Tables[0]);
                        }

                        if (status == "True") //IP - 24/01/13 - #11445 - Start the Hub Service when opening CoSACS
                        {
                            StartService("Blue Bridge Cosacs Hub", 30000);
                            lInterfaceProgress.Text = "Service Blue Bridge Cosacs Hub has been started";
                        }
                    }
                    SystemStatus();
                }
            }
            else
            {
                //IP - 20/09/10 - UAT5.2 Log - UAT(1004)
                //Display message to inform the user that all open windows must be closed before closing CoSACS.
                ShowInfo("M_CLOSEALLWINDOWSEOD");
            }
		}

		private void SystemStatus()
		{
			if(!(bool)Country[CountryParameterNames.SystemOpen])
			{
				btnSetCosacs.Text = "Open CoSACS";
				lStatus.Text = "CoSACS Is Currently Closed";
				lStatus.BackColor = System.Drawing.Color.Red;
			}
			else
			{
				btnSetCosacs.Text = "Close CoSACS";
				lStatus.Text = "CoSACS Is Currently Open";
				lStatus.BackColor = System.Drawing.Color.Lime;
			}
		}

		private void SetUpFrequency()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("Frequency");
			dt.Columns.Add("Code");

            DataRow newRow;
            newRow = dt.NewRow();
            newRow["Frequency"] = "";
            newRow["Code"] = 0;
            dt.Rows.Add(newRow);

			newRow = dt.NewRow();
			newRow["Frequency"] = GetResource("Once");
			newRow["Code"] = 1;
			dt.Rows.Add(newRow);

			newRow = dt.NewRow();
			newRow["Frequency"] = GetResource("Daily");
			newRow["Code"] = 4;
			dt.Rows.Add(newRow);

            //IP - 04/03/08 - Livewire(69582)
            //Add new entry 'Not Scheduled' to 'Frequency' drop down
            //for configurations that will not be scheduled to run.

            newRow = dt.NewRow();
            newRow["Frequency"] = GetResource("Not Scheduled");
            newRow["Code"] = 8;
            dt.Rows.Add(newRow);

			drpFrequency.DataSource = dt;
			drpFrequency.DisplayMember = "Frequency";
			drpFrequency.ValueMember = "Code";
		}

		private int GetStartDate()
		{
			string dateString = "";
			string tmpString = "";
			
			tmpString = dtEODStartDate.Value.Year.ToString();
			dateString += tmpString;
			
			if(dtEODStartDate.Value.Month < 10)
				tmpString = "0" + dtEODStartDate.Value.Month.ToString();
			else
				tmpString = dtEODStartDate.Value.Month.ToString();

			dateString += tmpString;

			if(dtEODStartDate.Value.Day < 10)
				tmpString = "0" + dtEODStartDate.Value.Day.ToString();
			else
				tmpString = dtEODStartDate.Value.Day.ToString();

			dateString += tmpString;

			return Convert.ToInt32(dateString);
		}

		private int GetStartTime()
		{
			string timeString = "";
			string tmpString = "";
			
			if(dtEODStartDate.Value.Hour < 10)
				tmpString = "0" + dtEODStartDate.Value.Hour.ToString();
			else
				tmpString = dtEODStartDate.Value.Hour.ToString();
			
			timeString += tmpString;
			
			if(dtEODStartDate.Value.Minute < 10)
				tmpString = "0" + dtEODStartDate.Value.Minute.ToString();
			else
				tmpString = dtEODStartDate.Value.Minute.ToString();
			
			timeString += tmpString;

			if(dtEODStartDate.Value.Second < 10)
				tmpString = "0" + dtEODStartDate.Value.Second.ToString();
			else
				tmpString = dtEODStartDate.Value.Second.ToString();
			
			timeString += tmpString;

			return Convert.ToInt32(timeString);
		}

        private void SetFieldBias(bool value, bool setAll)
        {
            btnUp.Enabled = !value;
            btnDown.Enabled = !value;

            if (setAll)
            {
                btnAdd.Enabled = !value;
                btnRemove.Enabled = !value;
                btnDelete.Enabled = !value;
                btnSave.Enabled = !value;
                drpConfigurations.Enabled = !value;
                txtConfigurations.Enabled = !value;
                btnDelete.Enabled = !value;
                btnRun.Enabled = !value;
                btnSetCosacs.Enabled = !value;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            bool status = true;

            try
            {
                if (drpConfigurations.Text.Length == 0)
                {
                    errorProvider1.SetError(drpConfigurations, GetResource("M_EODDELETECONFIG"));
                    status = false;
                }
                else
                    errorProvider1.SetError(drpConfigurations, "");

                if (status)
                {
                    EodManager.DeleteEODConfiguration(drpConfigurations.Text, out error);
                    if (error.Length > 0)
                        ShowError(error);
                    else
                    {
                        Clear();
                        refresh = true;
                        EODUserInterface_Enter(this, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of btnDelete_Click";
            }
        }

        private void LoadPreviousRuns(string eodOption, string eodDescription)
        {
            refresh = false;
            EODPreviousRuns eodPrev = new EODPreviousRuns(FormRoot, this, eodOption, eodDescription);
            ((MainForm)this.FormRoot).AddTabPage(eodPrev, 7);
        }

        private void dgOptions_DoubleClick(object sender, EventArgs e)
        {
            Wait();

            if (((DataView)dgOptions.DataSource).Count > 0)
            {
                string option = (string)((DataView)dgOptions.DataSource)[dgOptions.CurrentRowIndex][CN.Code];
                string descr = (string)((DataView)dgOptions.DataSource)[dgOptions.CurrentRowIndex][CN.CodeDescription];
                LoadPreviousRuns(option, descr);
            }
        }

        private void dgConfiguration_DoubleClick(object sender, EventArgs e)
        {
            if (((DataView)dgConfiguration.DataSource).Count > 0)
            {
                string option = (string)((DataView)dgConfiguration.DataSource)[dgConfiguration.CurrentRowIndex][CN.OptionCode];
                string descr = (string)((DataView)dgConfiguration.DataSource)[dgConfiguration.CurrentRowIndex][CN.CodeDescription];
                LoadPreviousRuns(option, descr);
            }            
        }

        private void ClearRunStatus()
        {
            if (dvRecordedOptions != null)
            {
                foreach (DataRowView row in dvRecordedOptions)
                    row[CN.Status] = "";
            }
        }

        private bool CheckDiskSpace()
        {
            bool enoughSpace = true;
            bool pathError = false;
            

            foreach (DataRowView row in dvRecordedOptions)
            {
                if ((string)row[CN.OptionCode] == "BACKAD1" ||
                   (string)row[CN.OptionCode] == "BACKAF1" ||
                   (string)row[CN.OptionCode] == "BACKBD1" ||
                   (string)row[CN.OptionCode] == "BACKBF1")
                {
                    EodManager.CheckDiskSpace((string)row[CN.OptionCode],
                        out enoughSpace, out pathError, out error);
                    if (error.Length > 0)
                        ShowError(error);
                    else
                    {
                        if (pathError)
                        {
                            ShowInfo("M_EODBACKUPPATH", new object[] { (string)row[CN.OptionCode]});
                            enoughSpace = false;
                            break;
                        }
                        if (!enoughSpace) 
                        {
                            ShowInfo("M_EODBACKUP");
                            break;
                        }
                    }
                }
            }
            return enoughSpace;
        }

        public override bool ConfirmClose()
        {
            bool status = true;
            try
            {
                EodManager.DeleteEODConfiguration("SPECIALRUN", out error);
                if (error.Length > 0)
                {
                    ShowError(error);
                    status = false;
                }
                
                if(status)
                {
                    EodManager.SetReRunStatus(drpConfigurations.Text, "", out error);
                    if (error.Length > 0)
                        ShowError(error);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }

            return true;
        }

        //private void label4_Click(object sender, EventArgs e)
        //{

        //}

        private void txtConfigurations_TextChanged(object sender, EventArgs e)
        {
            btnAdd.Enabled = true;
            btnRemove.Enabled = true;
            btnDown.Enabled = true;
            btnUp.Enabled = true;

        }

        private void btnFACTOptions_Click(object sender, EventArgs e)
        {

            FactAuto fa = new FactAuto(FormRoot, this);
            ((MainForm)this.FormRoot).AddTabPage(fa);
        }

        private void FormatConfigurationName(ref string configurationName)
        {
            string temp = null;
            foreach (char letter in configurationName)
            {
                if (letter.ToString().Equals(" "))
                    temp += "_";
                else
                    temp += letter;
            }
            configurationName = temp;
        }

        private void dgConfiguration_CurrentCellChanged(object sender, EventArgs e)
        {
            int i = dgConfiguration.CurrentRowIndex;

            dgConfiguration.Select(i);
        }

        // Set the ReRun    jec 05/04/11
        public void SetReRunNo(int reRunNo)
        {
            int i = dgConfiguration.CurrentRowIndex;
            dvRecordedOptions[i][CN.ReRunNo] = reRunNo;            
        }

        //IP - 12/08/11 - Weekly Trading Report
        private void btnWTRSetup_Click(object sender, EventArgs e)
        {
            WTRSetup wtr = new WTRSetup(FormRoot, this);                   
            ((MainForm)this.FormRoot).AddTabPage(wtr);
        }

        //IP - 24/01/13 - #11445 - Start a Windows Service
        public void StartService(string serviceName, int timeoutMilliseconds)
        {
              //#12156
              var exists =  EodManager.CheckServiceExists(serviceName,out error);

              if (error.Length > 0)
                  ShowError(error);
              else
              {

                  if (exists)
                  {
                      ServiceController service = new ServiceController(serviceName);

                      TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                      if (service.Status == ServiceControllerStatus.Stopped)
                      {
                          service.Start();
                      }

                      service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                  }
              }
        }

        //IP - 24/01/13 - #11445 - Stop a Windows Service
        public void StopService(string serviceName, int timeoutMilliseconds)
        {
            //#12156
           var exists =  EodManager.CheckServiceExists(serviceName,out error);

           if (error.Length > 0)
               ShowError(error);
           else
           {
               if (exists)
               {
                   ServiceController service = new ServiceController(serviceName);

                   TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                   if (service.Status == ServiceControllerStatus.Running ||
                       service.Status == ServiceControllerStatus.StartPending ||
                       service.Status == ServiceControllerStatus.ContinuePending)
                   {
                       service.Stop();
                   }

                   service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
               }
           }
        
        }

       
	}
}
