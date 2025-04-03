using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Web.Services.Protocols;
using STL.Common.Constants.ColumnNames;
using System.Data;
using STL.PL.WS5;
using STL.PL.WS7;
using STL.Common.Static;
using STL.Common.Constants.TableNames;
using STL.Common;
using Blue.Cosacs.Shared;

namespace STL.PL
{
	/// <summary>
	/// Maintenance screen that lists the credit limits to be awarded
	/// for certain combinations of credit score and customer income.
	/// The highest credit limit will be awarded where the credit score
	/// and the customer income exceeds both these values in the list.
	/// Two categories of credit limit are maintained. One for electrical
	/// goods and one for furniture goods. The category used is determined
	/// by the majority of the expenditure on the customer account.
	/// </summary>
	public class ScoringMatrix : CommonForm
	{
		//private Crownwood.Magic.Menus.MenuControl menuMain;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuMatrix;
		private Crownwood.Magic.Menus.MenuCommand menuSave;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private Crownwood.Magic.Menus.MenuCommand menuImport;
		private Crownwood.Magic.Menus.MenuCommand menuExport;
		private System.Windows.Forms.ImageList menuIcons;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox drpCountry;
		private System.Windows.Forms.Button btnSave;
		private System.ComponentModel.IContainer components;
		private new string Error = "";
		private System.Windows.Forms.DataGrid dgMatrix;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtRegion;
		private System.Windows.Forms.Label label2;
		private DataSet matrix = null;
        private ComboBox cmb_scorecardtype;
        private Label label3;
		private bool NewImport = false;

		public ScoringMatrix(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile, menuMatrix});
		}

		public ScoringMatrix(Form root, Form parent)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			FormRoot = root;
			FormParent = parent;
			HashMenus();
			this.ApplyRoleRestrictions();

			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile, menuMatrix});
            label1.Visible = !Credential.HasPermission(CosacsPermissionEnum.EditScoringMatrix);     // #12094
            menuExport.Visible = menuExport.Enabled = dgMatrix.Visible = dgMatrix.Enabled = Credential.HasPermission(CosacsPermissionEnum.ImportScorebandMatrix);     // #12094
            menuImport.Enabled = Credential.HasPermission(CosacsPermissionEnum.ImportScorebandMatrix);     // #12094
            dgMatrix.Visible = dgMatrix.Enabled = Credential.HasPermission(CosacsPermissionEnum.EditScoringMatrix);     // #12094
			
            LoadCountries(); //CR1034
			LoadMatrix();
			//TranslateControls();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScoringMatrix));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
            this.menuIcons = new System.Windows.Forms.ImageList(this.components);
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuMatrix = new Crownwood.Magic.Menus.MenuCommand();
            this.menuImport = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExport = new Crownwood.Magic.Menus.MenuCommand();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmb_scorecardtype = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtRegion = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.drpCountry = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgMatrix = new System.Windows.Forms.DataGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgMatrix)).BeginInit();
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
            this.menuSave.Enabled = false;
            this.menuSave.ImageIndex = 1;
            this.menuSave.ImageList = this.menuIcons;
            this.menuSave.Text = "&Save";
            this.menuSave.Visible = false;
            this.menuSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // menuIcons
            // 
            this.menuIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("menuIcons.ImageStream")));
            this.menuIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.menuIcons.Images.SetKeyName(0, "");
            this.menuIcons.Images.SetKeyName(1, "");
            this.menuIcons.Images.SetKeyName(2, "");
            this.menuIcons.Images.SetKeyName(3, "");
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.ImageIndex = 0;
            this.menuExit.ImageList = this.menuIcons;
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuMatrix
            // 
            this.menuMatrix.Description = "MenuItem";
            this.menuMatrix.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuImport,
            this.menuExport});
            this.menuMatrix.Text = "&Matrix";
            // 
            // menuImport
            // 
            this.menuImport.Description = "MenuItem";
            this.menuImport.ImageIndex = 2;
            this.menuImport.ImageList = this.menuIcons;
            this.menuImport.Text = "&Import";
            this.menuImport.Click += new System.EventHandler(this.menuImport_Click);
            // 
            // menuExport
            // 
            this.menuExport.Description = "MenuItem";
            this.menuExport.Enabled = false;
            this.menuExport.ImageIndex = 3;
            this.menuExport.ImageList = this.menuIcons;
            this.menuExport.Text = "&Export";
            this.menuExport.Visible = false;
            this.menuExport.Click += new System.EventHandler(this.menuExport_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.cmb_scorecardtype);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtRegion);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.drpCountry);
            this.groupBox1.Location = new System.Drawing.Point(8, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 80);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Country";
            // 
            // cmb_scorecardtype
            // 
            this.cmb_scorecardtype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_scorecardtype.Location = new System.Drawing.Point(304, 38);
            this.cmb_scorecardtype.Name = "cmb_scorecardtype";
            this.cmb_scorecardtype.Size = new System.Drawing.Size(168, 21);
            this.cmb_scorecardtype.TabIndex = 53;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(304, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 18);
            this.label3.TabIndex = 54;
            this.label3.Text = "ScoreCard";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(112, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 16);
            this.label2.TabIndex = 52;
            this.label2.Text = "Country";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(40, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 16);
            this.label8.TabIndex = 51;
            this.label8.Text = "Region";
            // 
            // txtRegion
            // 
            this.txtRegion.Location = new System.Drawing.Point(40, 40);
            this.txtRegion.MaxLength = 3;
            this.txtRegion.Name = "txtRegion";
            this.txtRegion.Size = new System.Drawing.Size(40, 20);
            this.txtRegion.TabIndex = 50;
            this.txtRegion.Leave += new System.EventHandler(this.txtRegion_Leave);
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.Enabled = false;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Location = new System.Drawing.Point(600, 32);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 37;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // drpCountry
            // 
            this.drpCountry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCountry.Enabled = false;
            this.drpCountry.Location = new System.Drawing.Point(112, 40);
            this.drpCountry.Name = "drpCountry";
            this.drpCountry.Size = new System.Drawing.Size(168, 21);
            this.drpCountry.TabIndex = 0;
            this.drpCountry.Visible = false;
            this.drpCountry.SelectedIndexChanged += new System.EventHandler(this.drpCountry_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.dgMatrix);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(8, 80);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(776, 392);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Scoring Matrix";
            // 
            // dgMatrix
            // 
            this.dgMatrix.DataMember = "";
            this.dgMatrix.Enabled = false;
            this.dgMatrix.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgMatrix.Location = new System.Drawing.Point(112, 64);
            this.dgMatrix.Name = "dgMatrix";
            this.dgMatrix.Size = new System.Drawing.Size(512, 272);
            this.dgMatrix.TabIndex = 0;
            this.dgMatrix.Visible = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(184, 160);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(384, 72);
            this.label1.TabIndex = 7;
            this.label1.Text = "You do not have sufficient permissions to view the current scoring rules.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ScoringMatrix
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 476);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ScoringMatrix";
            this.Text = "Customise Credit Matrices";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgMatrix)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void menuExit_Click(object sender, System.EventArgs e)
		{
			CloseTab();
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
            Save("");
            //CloseTab();// CR1034
		}

		private bool Save(string fileName)
		{
            bool status = true;
            try
            {
                Function = "Save()";
                Wait();
                CreditManager.SaveRFScoringMatrix(fileName, (string)((DataRowView)drpCountry.SelectedItem)[CN.Code], Convert.ToChar(cmb_scorecardtype.SelectedValue), txtRegion.Text, matrix, NewImport, out Error);
                if (Error.Length > 0)
                {
                    status = false;
                    ShowError(Error);
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
			return status;
		}

		private void menuImport_Click(object sender, System.EventArgs e)
		{
			int index = 0;
			string country = "";
            char scorecard = ' ';
            bool load = true;

			try
			{
				Function = "menuImport_Click";

				OpenFileDialog open = new OpenFileDialog();
				open.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*" ;
                open.Title = "Import RF " + cmb_scorecardtype.Text + " Scoring Matrix";

				if(open.ShowDialog() == DialogResult.OK)
				{
					DataSet		temp = new DataSet();
					temp.ReadXml(open.FileName, XmlReadMode.Auto);					
					if(temp.Tables[TN.ScoringMatrix].Rows.Count>0)
					{
						country = (string)temp.Tables[TN.ScoringMatrix].Rows[0][CN.CountryCode];
                        scorecard = Convert.ToChar(temp.Tables[TN.ScoringMatrix].Rows[0][CN.ScoringCard]);
						try
						{
							txtRegion.Text = (string)temp.Tables[TN.ScoringMatrix].Rows[0][CN.Region];
						}
						catch(Exception){};


					}

                    if (scorecard != Convert.ToChar(cmb_scorecardtype.SelectedValue)) //SC CR1034 Behavioural Scoring 15/02/2010 Checks if loading wrong scorecard for selected type and wrong country.
                    {
                        if (MessageBox.Show("The Scorecard Matrix you are loading does not match the category selected." +
                     Environment.NewLine +
                     "Are you sure you want to import this matrix under " + cmb_scorecardtype.Text + "?" +
                      Environment.NewLine +
                      Environment.NewLine +
                     "Click OK to continue.", "Scorecard type Mismatch", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                        {
                            load = false;
                        }
                    }


                    if (country.Trim().ToUpper() != Config.CountryCode)      // #12092 jec 23/01/13
                    {
                        ShowInfo("M_MATRIXWRONGCOUNTRY");
                        load = false;
                    }

                    if (load) 
                    {
                        matrix = temp;
                        foreach (DataRow r in ((DataTable)StaticData.Tables[TN.Countries]).Rows)
                        {
                            if ((string)r[CN.Code] == country.Trim().ToUpper())     // #12092 jec 23/01/13)
                                break;
                            index++;
                        }
                        drpCountry.SelectedIndex = index;
                        NewImport = true;
                        Save(open.FileName);
                        MessageBox.Show("New matrix successfully imported and saved to database.", "Matrix saved successfully", MessageBoxButtons.OK);
                        NewImport = false;
                    }
				}
				dgMatrix.DataSource = matrix.Tables[TN.ScoringMatrix];
               
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void menuExport_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "menuExport_Click";

				SaveFileDialog save = new SaveFileDialog();
				save.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*" ;
				save.Title = "Save Exported RF " + cmb_scorecardtype.Text + " Matrix as";

				//JJ - if this matrix has not been saved then the country column 
				//will be set to DBNull rather than the right country which will 
				//cause problems when it comes to importing it. Loop through and
				//set the country column
				string country = (string)((DataRowView)drpCountry.SelectedItem)[CN.Code];
				string region = txtRegion.Text;

                matrix.Tables[TN.ScoringMatrix].Columns.Add(CN.ScoringCard, typeof(System.Char));
				
				foreach(DataTable dt in matrix.Tables)
					if(dt.TableName==TN.ScoringMatrix)
						foreach(DataRow r in dt.Rows)
						{
							r[CN.CountryCode] = country;
							r[CN.Region] = region;
                            r[CN.ScoringCard] = Convert.ToChar(cmb_scorecardtype.SelectedValue);
						}

				if(save.ShowDialog() == DialogResult.OK)
				{
					matrix.WriteXml(save.FileName, XmlWriteMode.WriteSchema);
                    MessageBox.Show("Current matrix successfully exported and saved.", "Matrix saved successfully", MessageBoxButtons.OK);
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void LoadCountries()
		{
			try
			{
				Function = "LoadCountries()";
				Wait();

				int index = 0;
				bool found = false;
				
				Function = "WStaticDataManager::GetDropDowns()";
			
				XmlUtilities xml = new XmlUtilities();
				XmlDocument dropDowns = new XmlDocument();
				dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

				if(StaticData.Tables[TN.Countries]==null)
					dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.Countries, new string[] {"CTY", "L"}));

                if (StaticData.Tables[TN.ScoreCards] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ScoreCards, new string[] { "SCT", "L" }));

                if (dropDowns.DocumentElement.ChildNodes.Count > 0)
                {
                    DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            StaticData.Tables[dt.TableName] = dt;
                        }
                    }
                }


                cmb_scorecardtype.DataSource = (DataTable)StaticData.Tables[TN.ScoreCards]; //SC CR1034 Behavioural Scoring 15/02/2010 Get new scorecard types.
                cmb_scorecardtype.DisplayMember = CN.CodeDescription;
                cmb_scorecardtype.ValueMember = CN.Code;
                this.cmb_scorecardtype.SelectedValueChanged += new System.EventHandler(this.cmb_scorecardtype_SelectedValueChanged);


                //IP - 09/04/10 - CR1034 - Removed
                //cmb_scorecardtype.DataSource = (DataTable)StaticData.Tables[TN.ScoreCards]; //SC CR1034 Behavioural Scoring 15/02/2010 Get new scorecard types.
                //cmb_scorecardtype.DisplayMember = CN.CodeDescription;
                //cmb_scorecardtype.ValueMember = CN.Code;
                //this.cmb_scorecardtype.SelectedValueChanged += new System.EventHandler(this.cmb_scorecardtype_SelectedValueChanged);


				drpCountry.DataSource = (DataTable)StaticData.Tables[TN.Countries];
				drpCountry.DisplayMember = CN.CodeDescription;	


				foreach(DataRow r in ((DataTable)StaticData.Tables[TN.Countries]).Rows)
				{
					if((string)r[CN.Code] == Config.CountryCode)
					{
						found = true;
						break;
					}
					index++;
				}
				if(found)
					drpCountry.SelectedIndex = index;	
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



		private void LoadMatrix()
		{
			try
			{
                Function = "LoadMatrix()";
                Wait();
                matrix = CreditManager.GetRFScoringMatrix((string)((DataRowView)drpCountry.SelectedItem)[CN.Code], Convert.ToChar(cmb_scorecardtype.SelectedValue), txtRegion.Text, out Error); //SC CR1034 Behavioural Scoring 15/02/2010 
                if (Error.Length > 0)
                    ShowError(Error);
				else
				{
					foreach(DataTable dt in matrix.Tables)
					{
						if(dt.TableName==TN.ScoringMatrix)
						{
							dgMatrix.TableStyles.Clear();
							dgMatrix.DataSource = dt;
							SetTabStyle();
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

		private void SetTabStyle()
		{
			if(dgMatrix.TableStyles.Count==0)
			{
				DataGridTableStyle tabStyle = new DataGridTableStyle();
				tabStyle.MappingName = matrix.Tables[TN.ScoringMatrix].TableName;
				dgMatrix.TableStyles.Add(tabStyle);
			
				tabStyle.GridColumnStyles[CN.CountryCode].Width = 0;
				tabStyle.GridColumnStyles[CN.CountryCode].ReadOnly = true;

				tabStyle.GridColumnStyles[CN.Region].Width = 0;
				tabStyle.GridColumnStyles[CN.Region].ReadOnly = true;
				
				tabStyle.GridColumnStyles[CN.Score].Width = 50;
				tabStyle.GridColumnStyles[CN.FurnitureLimit].Width = 116;
				tabStyle.GridColumnStyles[CN.ElectricalLimit].Width = 116;
				tabStyle.GridColumnStyles[CN.Income].Width = 145;
                
				tabStyle.GridColumnStyles[CN.Score].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.FurnitureLimit].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.ElectricalLimit].Alignment = HorizontalAlignment.Right;
				tabStyle.GridColumnStyles[CN.Income].Alignment = HorizontalAlignment.Right;

				tabStyle.GridColumnStyles[CN.CountryCode].HeaderText = GetResource("T_COUNTRYCODE");
				tabStyle.GridColumnStyles[CN.Score].HeaderText = GetResource("T_SCORE");
				tabStyle.GridColumnStyles[CN.FurnitureLimit].HeaderText = GetResource("T_FURNITURELIM");
				tabStyle.GridColumnStyles[CN.ElectricalLimit].HeaderText = GetResource("T_ELECTRICALLIM");
				tabStyle.GridColumnStyles[CN.Income].HeaderText = GetResource("T_INCOME");

				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.FurnitureLimit]).Format = DecimalPlaces;
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.ElectricalLimit]).Format = DecimalPlaces;
				((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Income]).Format = DecimalPlaces;

				tabStyle.GridColumnStyles[CN.CountryCode].NullText = (string)((DataRowView)drpCountry.SelectedItem)[CN.Code];
				tabStyle.GridColumnStyles[CN.Score].NullText = "0";
				tabStyle.GridColumnStyles[CN.FurnitureLimit].NullText = (0).ToString(DecimalPlaces);
				tabStyle.GridColumnStyles[CN.ElectricalLimit].NullText = (0).ToString(DecimalPlaces);
				tabStyle.GridColumnStyles[CN.Income].NullText = (0).ToString(DecimalPlaces);
			}
		}

		private void drpCountry_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			LoadMatrix();
		}

		private void HashMenus()
		{
			dynamicMenus = new Hashtable();
			dynamicMenus[this.Name+":menuSave"] = this.menuSave;
			dynamicMenus[this.Name+":btnSave"] = this.btnSave;
			dynamicMenus[this.Name+":drpCountry"] = this.drpCountry;
            dynamicMenus[this.Name + ":dgMatrix"] = Credential.HasPermission(CosacsPermissionEnum.EditScoringMatrix);     // #12094
            dynamicMenus[this.Name + ":menuExport"] = Credential.HasPermission(CosacsPermissionEnum.ImportScorebandMatrix);     // #12094
            dynamicMenus[this.Name + ":menuImport"] = Credential.HasPermission(CosacsPermissionEnum.ImportScorebandMatrix);     // #12094
		}

		private void txtRegion_Leave(object sender, System.EventArgs e)
		{
			LoadMatrix();
		}

        private void cmb_scorecardtype_SelectedValueChanged(object sender, EventArgs e)
        {
            LoadMatrix();//SC CR1034 Behavioural Scoring 15/02/2010 
        }
	}
}
