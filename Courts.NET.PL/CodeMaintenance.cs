using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Web.Services.Protocols;
using STL.PL.WS5;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.PrivilegeClub;
using STL.Common.Constants.Categories;
using STL.Common;

namespace STL.PL
{
	/// <summary>
	/// Maintenance screen for the code categories used by CoSACS.
	/// This allows maintenance of code lists such as Bank Account Types,
	/// Cancellation Codes, Method of Payment, Nationality and Source of Attraction.
	/// Some categories are system defined and do not allow user maintenance.
	/// </summary>
	public class CodeMaintenance : CommonForm
	{
		private bool validateColumn = true;
		private bool pageLoaded = false;
		private int lastCategory = 0;

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.Button btnClear;

        private new string Error = "";
		private string Category = "";
		private bool staticLoaded = true;
		private DataSet ds = null;
		private DataView dv = null;
		private System.Windows.Forms.ComboBox drpCategories;
		private System.Windows.Forms.Label label1;
		//private Crownwood.Magic.Menus.MenuControl menuMain;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private Crownwood.Magic.Menus.MenuCommand menuHelp;
		private Crownwood.Magic.Menus.MenuCommand menuLaunchHelp;
		private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.GroupBox gblist;
		private System.Windows.Forms.TextBox txtDescription;
		private System.Windows.Forms.TextBox txtCode;
		private System.Windows.Forms.TextBox txtReference;
		private System.Windows.Forms.TextBox txtSortOrder;
        private System.Windows.Forms.DataGrid dgCodes;
        private Button btnEnter;
        private IContainer components;
        private TextBox txtAdditional;
        private ToolTip toolTip1;
        private PictureBox HelpIcon;
        private DataGridTableStyle tabStyle;
        private TextBox txtAdditional2;
        private CheckBox chkMmiApplicable;
        //private CheckBox cbDateRemoved;
        private int dgCodesWidth; //IP - 10/11/09 - CoSACS Improvements - Code Maintenance

		public CodeMaintenance(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
		}

		public CodeMaintenance(MainForm root)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile, menuHelp});

			this.FormRoot = root;
			this.FormParent = root;

			LoadData();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CodeMaintenance));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuHelp = new Crownwood.Magic.Menus.MenuCommand();
            this.menuLaunchHelp = new Crownwood.Magic.Menus.MenuCommand();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.HelpIcon = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.drpCategories = new System.Windows.Forms.ComboBox();
            this.btnEnter = new System.Windows.Forms.Button();
            this.dgCodes = new System.Windows.Forms.DataGrid();
            this.gblist = new System.Windows.Forms.GroupBox();
            this.txtAdditional2 = new System.Windows.Forms.TextBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.txtAdditional = new System.Windows.Forms.TextBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.txtReference = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.txtSortOrder = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.chkMmiApplicable = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.HelpIcon)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgCodes)).BeginInit();
            this.gblist.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
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
            // menuHelp
            // 
            this.menuHelp.Description = "MenuItem";
            this.menuHelp.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuLaunchHelp});
            this.menuHelp.Text = "&Help";
            // 
            // menuLaunchHelp
            // 
            this.menuLaunchHelp.Description = "MenuItem";
            this.menuLaunchHelp.Text = "&About This Screen";
            this.menuLaunchHelp.Click += new System.EventHandler(this.menuLaunchHelp_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // HelpIcon
            // 
            this.HelpIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.HelpIcon.Image = ((System.Drawing.Image)(resources.GetObject("HelpIcon.Image")));
            this.HelpIcon.Location = new System.Drawing.Point(489, 24);
            this.HelpIcon.Name = "HelpIcon";
            this.HelpIcon.Size = new System.Drawing.Size(20, 21);
            this.HelpIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.HelpIcon.TabIndex = 62;
            this.HelpIcon.TabStop = false;
            this.toolTip1.SetToolTip(this.HelpIcon, "Test");
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(8, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 472);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.groupBox2.Controls.Add(this.drpCategories);
            this.groupBox2.Controls.Add(this.HelpIcon);
            this.groupBox2.Controls.Add(this.btnEnter);
            this.groupBox2.Controls.Add(this.dgCodes);
            this.groupBox2.Controls.Add(this.gblist);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(32, 24);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(720, 424);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Categories";
            // 
            // drpCategories
            // 
            this.drpCategories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCategories.ItemHeight = 13;
            this.drpCategories.Location = new System.Drawing.Point(262, 24);
            this.drpCategories.MaxDropDownItems = 20;
            this.drpCategories.Name = "drpCategories";
            this.drpCategories.Size = new System.Drawing.Size(208, 21);
            this.drpCategories.TabIndex = 1;
            this.drpCategories.SelectedIndexChanged += new System.EventHandler(this.drpCategories_SelectedIndexChanged);
            // 
            // btnEnter
            // 
            this.btnEnter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEnter.BackColor = System.Drawing.Color.SlateBlue;
            this.btnEnter.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnter.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnEnter.Image = ((System.Drawing.Image)(resources.GetObject("btnEnter.Image")));
            this.btnEnter.Location = new System.Drawing.Point(691, 362);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(23, 22);
            this.btnEnter.TabIndex = 61;
            this.btnEnter.UseVisualStyleBackColor = false;
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // dgCodes
            // 
            this.dgCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgCodes.DataMember = "";
            this.dgCodes.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgCodes.Location = new System.Drawing.Point(5, 56);
            this.dgCodes.Name = "dgCodes";
            this.dgCodes.Size = new System.Drawing.Size(710, 284);
            this.dgCodes.TabIndex = 60;
            // 
            // gblist
            // 
            this.gblist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gblist.Controls.Add(this.chkMmiApplicable);
            //this.gblist.Controls.Add(this.chkMmiApplicable);
            this.gblist.Controls.Add(this.txtAdditional2);
            this.gblist.Controls.Add(this.btnExit);
            this.gblist.Controls.Add(this.txtAdditional);
            this.gblist.Controls.Add(this.btnDelete);
            this.gblist.Controls.Add(this.txtReference);
            this.gblist.Controls.Add(this.btnSave);
            this.gblist.Controls.Add(this.btnClear);
            this.gblist.Controls.Add(this.txtSortOrder);
            this.gblist.Controls.Add(this.txtDescription);
            this.gblist.Controls.Add(this.txtCode);
            this.gblist.Location = new System.Drawing.Point(24, 346);
            this.gblist.Name = "gblist";
            this.gblist.Size = new System.Drawing.Size(658, 72);
            this.gblist.TabIndex = 58;
            this.gblist.TabStop = false;
            // 
            // txtAdditional2
            // 
            this.txtAdditional2.BackColor = System.Drawing.SystemColors.Window;
            this.txtAdditional2.Location = new System.Drawing.Point(517, 16);
            this.txtAdditional2.MaxLength = 30;
            this.txtAdditional2.Name = "txtAdditional2";
            this.txtAdditional2.Size = new System.Drawing.Size(92, 20);
            this.txtAdditional2.TabIndex = 64;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(389, 42);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(72, 24);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "&Exit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // txtAdditional
            // 
            this.txtAdditional.BackColor = System.Drawing.SystemColors.Window;
            this.txtAdditional.Location = new System.Drawing.Point(452, 16);
            this.txtAdditional.MaxLength = 30;
            this.txtAdditional.Name = "txtAdditional";
            this.txtAdditional.Size = new System.Drawing.Size(57, 20);
            this.txtAdditional.TabIndex = 63;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(263, 42);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(72, 24);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // txtReference
            // 
            this.txtReference.BackColor = System.Drawing.SystemColors.Window;
            this.txtReference.Location = new System.Drawing.Point(374, 16);
            this.txtReference.MaxLength = 30;
            this.txtReference.Name = "txtReference";
            this.txtReference.Size = new System.Drawing.Size(72, 20);
            this.txtReference.TabIndex = 62;
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave.Location = new System.Drawing.Point(75, 42);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 36;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(144, 42);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(72, 24);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "&Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // txtSortOrder
            // 
            this.txtSortOrder.BackColor = System.Drawing.SystemColors.Window;
            this.txtSortOrder.Location = new System.Drawing.Point(296, 16);
            this.txtSortOrder.MaxLength = 30;
            this.txtSortOrder.Name = "txtSortOrder";
            this.txtSortOrder.Size = new System.Drawing.Size(72, 20);
            this.txtSortOrder.TabIndex = 61;
            // 
            // txtDescription
            // 
            this.txtDescription.BackColor = System.Drawing.SystemColors.Window;
            this.txtDescription.Location = new System.Drawing.Point(119, 16);
            this.txtDescription.MaxLength = 30;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(168, 20);
            this.txtDescription.TabIndex = 60;
            // 
            // txtCode
            // 
            this.txtCode.BackColor = System.Drawing.SystemColors.Window;
            this.txtCode.Location = new System.Drawing.Point(21, 16);
            this.txtCode.MaxLength = 30;
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(78, 20);
            this.txtCode.TabIndex = 59;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(40, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 16);
            this.label1.TabIndex = 37;
            this.label1.Text = "Category Code Filter:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // chkMmiApplicable
            // 
            this.chkMmiApplicable.AutoSize = true;
            this.chkMmiApplicable.Location = new System.Drawing.Point(617, 19);
            this.chkMmiApplicable.Name = "chkMmiApplicable";
            this.chkMmiApplicable.Size = new System.Drawing.Size(15, 14);
            this.chkMmiApplicable.TabIndex = 65;
            this.chkMmiApplicable.UseVisualStyleBackColor = true;
            // 
            // CodeMaintenance
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox1);
            this.Name = "CodeMaintenance";
            this.Text = "Code Maintenance";
            this.Load += new System.EventHandler(this.CodeMaintenance_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.CodeMaintenance_HelpRequested);
            ((System.ComponentModel.ISupportInitialize)(this.HelpIcon)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgCodes)).EndInit();
            this.gblist.ResumeLayout(false);
            this.gblist.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

		}
        #endregion

        private void groupBox1_Enter(object sender, System.EventArgs e)
        {

        }

        /// <summary>
        /// Loads Categories and category codes
        /// </summary>
        private void LoadData()
		{
			try
			{
				Wait();
				Function = "LoadData()";
				this.errorProvider1.SetError(this.dgCodes, "");

                lastCategory = drpCategories.SelectedIndex;

				if(staticLoaded)
				{
					dgCodes.DataSource = null;
					dgCodes.TableStyles.Clear();
					dgCodes.Refresh();

                    //dgCodesWidth = Convert.ToInt32(dgCodes.Width); //IP - 10/11/09 - CoSACS Improvement - Code Maintenance; 

					drpCategories.DataSource = null;
					drpCategories.Refresh();

					ds = StaticDataManager.GetAllCodesAndCategories(out Error);

					if(Error.Length>0)
					{
						ShowError(Error);
					}
					else if(ds!=null)
					{
						((MainForm)this.FormRoot).statusBar1.Text = ds.Tables[TN.CodeCat].Rows.Count + " Category(s) returned -Users should exit and reenter system to ensure updates take effect";
						ds.Tables[TN.Code].ColumnChanging += new DataColumnChangeEventHandler(this.Code_ColumnChanging);

						string str = "";
						StringCollection Categories = new StringCollection();

						foreach(DataRow row in ds.Tables[TN.CodeCat].Rows)
						{
							str = (string)row.ItemArray[0]+" : "+(string)row.ItemArray[1];
							Categories.Add(str);
						}

                        //-- UAT(5.2) - 803 -------------------------
                        foreach (DataRow row in ds.Tables[TN.Code].Rows)
                        {
                            if(row[CN.Category].ToString().Trim() == "LTA" && 
                                (row[CN.Reference] == null || row[CN.Reference].ToString().Trim() == "") )
                                row[CN.Reference] = "0";
                        }
                        //-------------------------------------------

                        //drpCategories.DataSource = Categories;  // moved to after vs2005 err

                        // filter data grid on category
                        if (dgCodes.TableStyles[TN.Code] == null)
                        {
                            dv = new DataView(ds.Tables[TN.Code]);
                            dv.AllowNew = false;
                            dv.AllowEdit = true;
                            dv.AllowDelete = false;
                            dgCodes.DataSource = dv;

                            tabStyle = new DataGridTableStyle();
                            tabStyle.MappingName = dv.Table.TableName;
                            dgCodes.TableStyles.Add(tabStyle);
                            tabStyle.GridColumnStyles[CN.Category].Width = 0;
                            tabStyle.GridColumnStyles[CN.OldCategory].Width = 0;
                            tabStyle.GridColumnStyles[CN.OldCode].Width = 0;
                            tabStyle.GridColumnStyles[CN.Code].Width = 120;
                            tabStyle.GridColumnStyles[CN.CodeDescript].Width = 160;
                            tabStyle.GridColumnStyles[CN.SortOrder].Width = 80;
                            tabStyle.GridColumnStyles[CN.Reference].Width = 80;

                            tabStyle.GridColumnStyles[CN.Code].HeaderText = GetResource("T_CODE");
                            tabStyle.GridColumnStyles[CN.CodeDescript].HeaderText = GetResource("T_CODEDSCRIPT");
                            tabStyle.GridColumnStyles[CN.SortOrder].HeaderText = GetResource("T_SORTORDER");
                            tabStyle.GridColumnStyles[CN.Reference].HeaderText = GetResource("T_REFERENCE");

                            tabStyle.GridColumnStyles[CN.Code].ReadOnly = true;

                            if (Category == CAT.CustomerCode1 || Category == CAT.CustomerCode2)
                            {
                                tabStyle.GridColumnStyles[CN.MmiApplicable].Width = 80;
                                tabStyle.GridColumnStyles[CN.MmiApplicable].HeaderText = GetResource("T_MMIAPPLICABLE");
                                tabStyle.GridColumnStyles[CN.MmiApplicable].Alignment = HorizontalAlignment.Center;
                            }
                            else
                            {
                                tabStyle.GridColumnStyles[CN.MmiApplicable].Width = 0;
                                tabStyle.GridColumnStyles[CN.MmiApplicable].HeaderText = string.Empty;
                            }
                        }
                        drpCategories.DataSource = Categories;
                        //this.drpCategories.Items[0]
						// select the first category and load into the data grid
						Reload();
                        if (lastCategory >= 0)
                        {
                            drpCategories.SelectedIndex = lastCategory;
                        }
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

		private void Reload() 
		{

           //IP - 10/11/09 - CoSACS Improvement - Code Maintenance
            HelpIcon.Visible = false;
            toolTip1.SetToolTip(HelpIcon, "");

            int dgWidth = 0;
            int columnWidthMultiplier1 = 10;
            int columnWidthMultiplier2 = 6;
            bool isMmiEnabled = Convert.ToBoolean(Country[CountryParameterNames.EnableMMI]);


            if (drpCategories.SelectedIndex == -1)
            // jec fix vs2005 err if index = -1
            {
                Category = (string)this.drpCategories.Items[0];
                Category = Category.Substring(0, Category.IndexOf(":") - 1);
                dv.RowFilter = CN.Category + " = '" + Category + "'";
                drpCategories.SelectedIndex = 0;    // jec fix vs2005 err
            }
            
            Category = (string)this.drpCategories.Items[this.drpCategories.SelectedIndex];
            Category = Category.Substring(0, Category.IndexOf(":") - 1);
            dv.RowFilter = CN.Category + " = '" + Category + "'";
			//this.lastCategory = this.drpCategories.SelectedIndex;
			// Clear the add new fields
			txtCode.Text = "";
			txtDescription.Text = "";
			txtSortOrder.Text = "";
			txtReference.Text = "";
		    txtAdditional.Text = "";
            txtAdditional2.Text = "";                                   //IP - 07/12/11 - CR1234
			this.errorProvider1.SetError(txtSortOrder, "");
			this.errorProvider1.SetError(txtReference, "");
            this.errorProvider1.SetError(txtAdditional, "");
            this.errorProvider1.SetError(txtAdditional2, "");           //IP - 07/12/11 - CR1234
            this.errorProvider1.SetError(dgCodes, "");

            //IP - 30/10/09 - CoSACS Improvements - Code Maintenance
            DataView dvCodeCat = new DataView(ds.Tables[TN.CodeCat]);
            dvCodeCat.RowFilter = CN.Category + " = '" + Category + "'";
            DataRowView drCodeCat = dvCodeCat[0];

            // default width's and descriptions
            tabStyle.GridColumnStyles[CN.Category].Width = 0;
            tabStyle.GridColumnStyles[CN.OldCategory].Width = 0;
            tabStyle.GridColumnStyles[CN.OldCode].Width = 0;
            tabStyle.GridColumnStyles[CN.Code].Width = Convert.ToInt32(drCodeCat[CN.CodeLength]) <= 15 ? Convert.ToInt32(drCodeCat[CN.CodeLength]) * columnWidthMultiplier1 : Convert.ToInt32(drCodeCat[CN.CodeLength]) * columnWidthMultiplier2;
            tabStyle.GridColumnStyles[CN.CodeDescript].Width = Convert.ToInt32(drCodeCat[CN.DescriptionLength]) <= 15 ? Convert.ToInt32(drCodeCat[CN.DescriptionLength]) * columnWidthMultiplier1 : Convert.ToInt32(drCodeCat[CN.DescriptionLength]) * columnWidthMultiplier2;
            tabStyle.GridColumnStyles[CN.SortOrder].Width = Convert.ToInt32(drCodeCat[CN.SortOrderLength]) * columnWidthMultiplier2;
            tabStyle.GridColumnStyles[CN.Reference].Width = Convert.ToInt32(drCodeCat[CN.ReferenceLength]) <= 15 ? Convert.ToInt32(drCodeCat[CN.ReferenceLength]) * columnWidthMultiplier1 : Convert.ToInt32(drCodeCat[CN.ReferenceLength]) * columnWidthMultiplier2;
            tabStyle.GridColumnStyles[CN.Additional].Width = Convert.ToInt32(drCodeCat[CN.AdditionalLength]) <= 15 ? Convert.ToInt32(drCodeCat[CN.AdditionalLength]) * columnWidthMultiplier1 : Convert.ToInt32(drCodeCat[CN.AdditionalLength]) * columnWidthMultiplier2;
            tabStyle.GridColumnStyles[CN.Additional2].Width = Convert.ToInt32(drCodeCat[CN.Additional2Length]) <= 15 ? Convert.ToInt32(drCodeCat[CN.Additional2Length]) * columnWidthMultiplier1 : Convert.ToInt32(drCodeCat[CN.Additional2Length]) * columnWidthMultiplier2; //IP - 07/12/11 - CR1234

            //IP - 25/07/11 - CR1254 - RI - #4036
            if (Convert.ToBoolean(Country[CountryParameterNames.RIDispCatAsDept]) && Convert.ToString(drCodeCat[CN.CodeHeaderText]) == "Product Category")
            {
                tabStyle.GridColumnStyles[CN.Code].HeaderText = "Product " + GetResource("T_DEPARTMENT");
            }
            else
            {
                tabStyle.GridColumnStyles[CN.Code].HeaderText = drCodeCat[CN.CodeHeaderText] != DBNull.Value ? Convert.ToString(drCodeCat[CN.CodeHeaderText]) : GetResource("T_CODE");
            }
            tabStyle.GridColumnStyles[CN.CodeDescript].HeaderText = drCodeCat[CN.DescriptionHeaderText] != DBNull.Value ? Convert.ToString(drCodeCat[CN.DescriptionHeaderText]) : GetResource("T_CODEDSCRIPT");
            tabStyle.GridColumnStyles[CN.SortOrder].HeaderText = drCodeCat[CN.SortOrderHeaderText] != DBNull.Value ? Convert.ToString(drCodeCat[CN.SortOrderHeaderText]) : GetResource("T_SORTORDER"); 
            tabStyle.GridColumnStyles[CN.Reference].HeaderText = drCodeCat[CN.ReferenceHeaderText] != DBNull.Value ? Convert.ToString(drCodeCat[CN.ReferenceHeaderText]) : GetResource("T_REFERENCE"); 
            tabStyle.GridColumnStyles[CN.Additional].HeaderText = drCodeCat[CN.AdditionalHeaderText] != DBNull.Value ? Convert.ToString(drCodeCat[CN.AdditionalHeaderText]) : GetResource("T_ADDITIONAL");
            tabStyle.GridColumnStyles[CN.Additional2].HeaderText = drCodeCat[CN.Additional2HeaderText] != DBNull.Value ? Convert.ToString(drCodeCat[CN.Additional2HeaderText]) : GetResource("T_ADDITIONAL2"); //IP - 07/12/11 - CR1234

            if (isMmiEnabled && (Category == CAT.CustomerCode1 || Category == CAT.CustomerCode2))
            {
                tabStyle.GridColumnStyles[CN.MmiApplicable].Width = 85;
                tabStyle.GridColumnStyles[CN.MmiApplicable].HeaderText = drCodeCat[CN.MmiApplicableText] != DBNull.Value ? Convert.ToString(drCodeCat[CN.MmiApplicableText]) : GetResource("T_MMIAPPLICABLE");
                chkMmiApplicable.Visible = true;
            }
            else
            {
                tabStyle.GridColumnStyles[CN.MmiApplicable].Width = 0;
                tabStyle.GridColumnStyles[CN.MmiApplicable].HeaderText = string.Empty;
                chkMmiApplicable.Visible = false;
            }


            //Set the tool tip to display.
            if (drCodeCat[CN.ToolTipText] != DBNull.Value)
            {
                toolTip1.SetToolTip(HelpIcon, Convert.ToString(drCodeCat[CN.ToolTipText]));
                HelpIcon.Visible = true;
            }

            tabStyle.GridColumnStyles[CN.Code].ReadOnly = true;
            


            //IP - 09/11/09 - CoSACS Improvement - Code Maintenance - Reduce the size of the DataGrid if the total of
            //the column widths is less than the width of the DataGrid.

            //Certain categories may have large descriptions. In this case we want to restrict the width of the description column.
            if (Convert.ToInt32(tabStyle.GridColumnStyles[CN.CodeDescript].Width) > 175)
            {
                tabStyle.GridColumnStyles[CN.CodeDescript].Width = 175;
            }

            dgWidth = Convert.ToInt32(tabStyle.GridColumnStyles[CN.Code].Width) +
                      Convert.ToInt32(tabStyle.GridColumnStyles[CN.CodeDescript].Width) +
                      Convert.ToInt32(tabStyle.GridColumnStyles[CN.SortOrder].Width) +
                      Convert.ToInt32(tabStyle.GridColumnStyles[CN.Reference].Width) +
                      Convert.ToInt32(tabStyle.GridColumnStyles[CN.Additional].Width) +
                      Convert.ToInt32(tabStyle.GridColumnStyles[CN.Additional2].Width) + 
                      (isMmiEnabled ? Convert.ToInt32(tabStyle.GridColumnStyles[CN.MmiApplicable].Width) : 0) +
                      + 50;  //IP - 07/12/11 - CR1234

            //if (dgWidth < dgCodesWidth)       // #8922 remove
            //{
            //    //dgCodes.Width= dgWidth;
            //    //gblist.Width = dgWidth;
            //}
           

            //align text fields with grid
            txtCode.Width = tabStyle.GridColumnStyles[CN.Code].Width;
		    txtReference.Width = tabStyle.GridColumnStyles[CN.Reference].Width;
            txtAdditional.Width = tabStyle.GridColumnStyles[CN.Additional].Width;
            txtAdditional2.Width = tabStyle.GridColumnStyles[CN.Additional2].Width;         //IP - 07/12/11 - CR1234
		    txtSortOrder.Width = tabStyle.GridColumnStyles[CN.SortOrder].Width;
		    txtDescription.Width = tabStyle.GridColumnStyles[CN.CodeDescript].Width;

		    Int16 length = 2;
		    
		    txtDescription.Left = txtCode.Right + length;
		    txtSortOrder.Left = txtDescription.Right + length;
		    txtReference.Left = txtSortOrder.Right + length;
            txtAdditional.Left = txtReference.Right + length;
            txtAdditional2.Left = txtAdditional.Right + length;                              //IP - 07/12/11 - CR1234
            chkMmiApplicable.Left = txtAdditional2.Right + length;
        }

        private void drpCategories_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            resetErrors();
            if (this.CheckDuplicates(null, null))
				Reload();
			else
				this.drpCategories.SelectedIndex = this.lastCategory;            
		}

		private void btnExit_Click(object sender, System.EventArgs e)
		{
			CloseTab();
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			LoadData();
            resetErrors();
		}

        private void resetErrors()
        {
            // Reset errors 
            this.errorProvider1.SetError(txtSortOrder, "");
            this.errorProvider1.SetError(txtReference, "");
            this.errorProvider1.SetError(txtAdditional, "");
            this.errorProvider1.SetError(txtAdditional2, "");               //IP - 07/12/11 - CR1234
            this.errorProvider1.SetError(txtDescription, "");
            this.errorProvider1.SetError(txtCode, "");
        }



		private void Delete()
		{
            // #8926 Code for Audit logging
            DataSet deletes = new DataSet();
            DataTable dtCodcat = new DataTable(TN.CodeCat);
            DataView dvCodeCat = new DataView(ds.Tables[TN.CodeCat]);            
            dtCodcat.Columns.Add(CN.Category);
            dtCodcat.Columns.Add(CN.CatDescript);
            DataTable dtCode = ds.Tables[TN.Code].Clone();


			try
			{
                
				Function = "Delete()";
				Wait();

				// Loop through dv and delete selected item
				for (int i = 0; i < dv.Count; i++)
				{
					if (dgCodes.IsSelected(i))
					{
						string Category = dgCodes[i, 0] as string;
						string Code = dgCodes[i, 1] as string;
                        // #8926 Code for Audit logging
                        DataView dvCode = new DataView(ds.Tables[TN.Code]);
                        dvCode.RowFilter = CN.Category + " = '" + Category + "'" + " and " + CN.Code + " = '" + Code + "'";
                        dtCode = dvCode.ToTable();

                        dvCodeCat = new DataView(ds.Tables[TN.CodeCat]);
                        dvCodeCat.RowFilter = CN.Category + " = '" + Category + "'";

                        DataRow r = dtCodcat.NewRow();
                        r[CN.Category] = Category;
                        r[CN.CatDescript] = dvCodeCat[0][1];
                        dtCodcat.Rows.InsertAt(r, 0);
                        deletes.Tables.Add(dtCode);
                        deletes.Tables.Add(dtCodcat);

						// Extract details for delete
                        StaticDataManager.CodeDelete(Code, Category, deletes, out Error);         // #8926 pass dataset
	
						if(Error.Length>0)
							ShowError(Error);
						else 
						{
							dv.AllowDelete = true;
							dv.Delete(i);
							// Do NOT accept changes here or we lose track of the rows
							// that have been added amd the rows that have been changed.
							//ds.AcceptChanges();		//commit changes to the dataset
							dv.AllowDelete = false;
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

		private void SaveUpdates()
		{
			DataSet changes = null;
            bool valid = true;
            DataView dvCodeCat = new DataView(ds.Tables[TN.CodeCat]);            

            DataTable dtCodcat = new DataTable(TN.CodeCat);
            dtCodcat.Columns.Add(CN.Category);
            dtCodcat.Columns.Add(CN.CatDescript);

			try
			{
				Function = "SaveUpdates()";
				Wait();

                if (this.CheckDuplicates(null, null))
				{
					if (ds.HasChanges(DataRowState.Modified))	//have any records been modified
					{
						changes = ds.GetChanges(DataRowState.Modified);

                        var category = Convert.ToString(changes.Tables[TN.Code].Rows[0][CN.Category]);

                        dvCodeCat = new DataView(ds.Tables[TN.CodeCat]);
                        dvCodeCat.RowFilter = CN.Category + " = '" + category + "'";
                        changes.Tables.Remove(TN.CodeCat);
                        DataRow r = dtCodcat.NewRow();
                        r[CN.Category] = category;
                        r[CN.CatDescript] = dvCodeCat[0][1];
                        dtCodcat.Rows.InsertAt(r, 0);
                        changes.Tables.Add(dtCodcat);

                        valid = CheckDiscountCodes(changes);
                        if (valid)
                        {
                            StaticDataManager.CodeUpdate(changes, out Error);
                            if (Error.Length > 0)
                                ShowError(Error);
                        }
					}

                    if (valid)
                    {
                        if (ds.HasChanges(DataRowState.Added))
                        {
                            changes = ds.GetChanges(DataRowState.Added);

                            var category=Convert.ToString(changes.Tables[TN.Code].Rows[0][CN.Category]);                            

                            dvCodeCat = new DataView(ds.Tables[TN.CodeCat]);
                            dvCodeCat.RowFilter = CN.Category + " = '" + category + "'";

                            //changes.Tables.Remove(TN.CodeCat);
                            //dtCodcat = new DataTable(TN.CodeCat);
                            //dtCodcat.Columns.Add(CN.Category);
                            //dtCodcat.Columns.Add(CN.CatDescript);

                            changes.Tables.Remove(TN.CodeCat);
                            DataRow r = dtCodcat.NewRow();
                            r[CN.Category] = category;
                            r[CN.CatDescript] = dvCodeCat[0][1];
                            dtCodcat.Rows.InsertAt(r, 0);
                            changes.Tables.Add(dtCodcat);
                            
                            valid = CheckDiscountCodes(changes);
                            if (valid)
                            {
                                StaticDataManager.CodeUpdate(changes, out Error);
                                if (Error.Length > 0)
                                    ShowError(Error);
                            }
                        }
                    }

                    if (valid)
                    {
                        if (Error.Length == 0)
                        {
                            ds.AcceptChanges();		//commit changes to the dataset
                            LoadData();
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

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			SaveUpdates();
		}

		private void btnDelete_Click(object sender, System.EventArgs e)
		{
			Delete();
		}

		private void menuExit_Click(object sender, System.EventArgs e)
		{
			CloseTab();
		}

		private void CodeMaintenance_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
		{
			string fileName = this.Name + ".htm";
			LaunchHelp(fileName);
		}

		private void menuLaunchHelp_Click(object sender, System.EventArgs e)
		{
			CodeMaintenance_HelpRequested(this, null);
		}
		protected void Code_ColumnChanging(object sender, System.Data.DataColumnChangeEventArgs e)
		{
			// Validate the Code
			try
			{
				Function = "Code Maintenance Screen: Validate Code";

				if (e.Column.ColumnName == CN.Code && validateColumn)
				{
					Wait();
					// Don't validate columns with this event that were changed by this event
					validateColumn = false;

					// Write back the formatted code
					e.ProposedValue = this.TrimCode(e.ProposedValue.ToString());
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				validateColumn = true;
				StopWait();
			}
				
		}

		private string TrimCode(string newCode)
		{
			// Do not trim if Tax rate category - code is itemno and valid to have 
			// leading zeroes or spaces    (CR776  jec)
			string returnCode=newCode;

			if (Category != "TXR")
			{

				// Trim off any leading/trailing spaces
				//string returnCode = newCode.Trim();
				returnCode = newCode.Trim();
				// Remove any embedded spaces
				returnCode = returnCode.Replace(" ","");
				// Trim off any leading zeroes
                if (returnCode.Length > 1 && Category != "WRF" && Category != "WDP" && Category != "WDL" && Category != "WDC")
                {
                    while (returnCode.Substring(0, 1) == "0" && returnCode.Length > 1)
                        returnCode = returnCode.Substring(1);
                }
			}

			return returnCode;
		}

        private bool CheckDuplicates(string newCode, string newCodedescript)
        {
            this.errorProvider1.SetError(this.dgCodes, "");

            bool isvalid = true;

            if (this.pageLoaded && dv != null)
            {
                // Check for any duplicates (regardless of whether changes have been made)
                try //AA putting in try catch in case on unhandled error
                {
                    foreach (DataRowView row in dv)
                    {
                        if (Category == "TXR" && !IsStrictNumeric(row[CN.CodeDescript].ToString()))
                        {
                            this.errorProvider1.SetError(this.dgCodes, GetResource("M_NUMERIC"));
                            isvalid = false;
                        }

                        // Check for items that can be duplicated.
                        if (Category == "WDP" || Category == "WDC" || Category == "WDL" || Category == "LT1")
                        {
                            if (dv.Table.Select("(" + CN.Code + " = '" + (string)row[CN.Code] + "' " +
                                                "and Category = '" + row[CN.Category].ToString() + "' " +
                                                "and codedescript = '" + row["codedescript"].ToString() + "')").Length > 1)
                            {
                                isvalid = false;

                            }
                        }
                        else if (Category == "ICN")
                        {
                            if (dv.Table.Select("(" + CN.Category + " = '" + (string)row[CN.Category] + "' " +
                                               "and " + CN.Reference +" = '" + row[CN.Reference].ToString() + "')").Length > 1)
                            {
                                isvalid = false;

                            }
                            else if (dv.Table.Select("(" + CN.Code + " = '" + (string)row[CN.Code] + "' " +
                                                "and Category = '" + row["Category"].ToString() + "') ").Length > 1)
                            {
                                isvalid = false;
                            }
                        }
                        else
                        {
                            if (dv.Table.Select("(" + CN.Code + " = '" + (string)row[CN.Code] + "' " +
                                                "and Category = '" + row["Category"].ToString() + "') ").Length > 1)
                            {
                                isvalid = false;
                            }
                        }


                        // Check for items that can be duplicated on new entry.
                        if (newCode == (string)row[CN.Code] && !(row["Category"].ToString() == "WDP" || Category == "WDC" || Category == "WDL")
                            || newCode == (string)row[CN.Code] && newCodedescript == row["codedescript"].ToString())
                        {
                            isvalid = false;
                        }
                    }

                    if (!isvalid)
                    {
                        this.errorProvider1.SetError(this.dgCodes, "Duplicate code. Please revise addition.");
                    }

                    

                }
                catch (Exception ex)
                {
                    Catch(ex, Function);
                }
            }
            return isvalid;
        }

		private void CodeMaintenance_Load(object sender, System.EventArgs e)
		{
			this.pageLoaded = true;
		}

        private bool CheckDiscountCodes(DataSet dsDiscountCodes)
        {
            bool valid = true;
            DataView dvDiscountCodes = new DataView(dsDiscountCodes.Tables[0]);
            dvDiscountCodes.RowFilter = CN.Category + " = 'KCD'";

            foreach (DataRowView row in dvDiscountCodes)
            {
                bool isDiscount = AccountManager.IsDiscount(row[CN.Reference].ToString(), out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    if (!isDiscount)
                    {
                        this.errorProvider1.SetError(this.dgCodes, GetResource("M_DISCOUNTINVALID"));
                        valid = false;
                        break;
                    }
                }
            }

            return valid;
        }

       private void btnEnter_Click(object sender, EventArgs e)
        {
            // Add a new row to the list for this category
            try
            {
                Wait();
                resetErrors();

                SetToUpper();

                // Default the Sort Order and Reference to zero
                string SortOrder = (txtSortOrder.Text.Trim().Length == 0) ? "0" : txtSortOrder.Text.Trim();
                string Reference = (txtReference.Text.Trim().Length == 0) ? "0" : txtReference.Text.Trim();
                string Additional = (txtAdditional.Text.Trim().Length == 0) ? "0" : txtReference.Text.Trim();
           

                if (!IsPositive(txtSortOrder.Text))
                    this.errorProvider1.SetError(txtSortOrder, GetResource("M_POSITIVENUM"));
                // check for numeric for Tax rates    CR776 jec
                else if (Category == CAT.TaxRates && !IsStrictNumeric(txtDescription.Text.Trim()))
                    this.errorProvider1.SetError(txtDescription, GetResource("M_NUMERIC"));
                // check for 1 char Code if SCH category        (jec 29/03/07)
                else
                    if (Category == CAT.SundryCharges && (txtCode.Text.Length > 1))
                    {
                        this.errorProvider1.SetError(txtCode, GetResource("M_ONECHARCODEONLY"));
                    }
                    else if (this.CheckDuplicates(txtCode.Text, txtDescription.Text))
                    {
                        DataRow row = ds.Tables[TN.Code].NewRow();
                        row[CN.Category] = this.Category.Trim();
                        row[CN.Code] = this.TrimCode(txtCode.Text);
                        row[CN.CodeDescript] = txtDescription.Text.Trim();
                        row[CN.SortOrder] = Convert.ToInt32(SortOrder);
                        row[CN.Reference] = txtReference.Text.Trim();
                        row[CN.Additional] = txtAdditional.Text.Trim();
                        row[CN.Additional2] = txtAdditional2.Text.Trim();                                                     //IP - 07/12/11 - CR1234
                        row[CN.MmiApplicable] = chkMmiApplicable.Checked;
                        ds.Tables[TN.Code].Rows.Add(row);

                        //-- UAT(5.2) - 803 -------------------------
                        if(row[CN.Reference].ToString() == "")
                            row[CN.Reference] = "0"; 
                        //-------------------------------------------

                        dgCodes.CurrentRowIndex = dv.Count - 1;
                        // clear entered text	jec
                        txtCode.Text = "";
                        txtDescription.Text = "";
                        txtSortOrder.Text = "";
                        txtReference.Text = "";
                        txtAdditional.Text = "";
                        txtAdditional2.Text = "";                                                                           //IP - 07/12/11 - CR1234
                        chkMmiApplicable.Checked = false;
                    }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        private void SetToUpper()
        {
            txtCode.Text = txtCode.Text.ToUpper();
            txtReference.Text = txtReference.Text.ToUpper();
            txtSortOrder.Text = txtSortOrder.Text.ToUpper();
        }
          
    }
}
