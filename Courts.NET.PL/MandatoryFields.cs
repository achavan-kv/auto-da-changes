using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using STL.PL.WS5;
using STL.Common;
using System.Web.Services.Protocols;
using STL.Common.Static;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using System.Xml;



namespace STL.PL
{
	/// <summary>
	/// Maintenance screen to define which fields are mandatory on the credit
	/// sanction screens. This varies from country to country and can be amended
	/// by the users. The fields are listed per sanction screen and a tick can be 
	/// placed next to each field that should be mandatory.
	/// The sanction screen highlight all mandatory fields and do not allow a
	/// credit application to be completed until all mandatory fields have been
	/// entered.
	/// </summary>
	public class MandatoryFields : CommonForm
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox drpScreens;
		private System.Windows.Forms.DataGrid dgFields;
		private System.Windows.Forms.Button btnSave;
		private new string Error = "";
		private DataSet fields = null;
		//private Crownwood.Magic.Menus.MenuControl menuMain;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuSave;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private IContainer components;
        private ErrorProvider errorProvider1;

		private bool staticLoaded = false;

		public MandatoryFields(TranslationDummy d)
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
		}

		public MandatoryFields()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MandatoryFields));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dgFields = new System.Windows.Forms.DataGrid();
            this.drpScreens = new System.Windows.Forms.ComboBox();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgFields)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dgFields);
            this.groupBox1.Controls.Add(this.drpScreens);
            this.groupBox1.Location = new System.Drawing.Point(8, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 472);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Location = new System.Drawing.Point(24, 32);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 36;
            this.btnSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(64, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 16);
            this.label1.TabIndex = 8;
            this.label1.Text = "Screen:";
            // 
            // dgFields
            // 
            this.dgFields.CaptionText = "Mandatory Fields";
            this.dgFields.DataMember = "";
            this.dgFields.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgFields.Location = new System.Drawing.Point(96, 80);
            this.dgFields.Name = "dgFields";
            this.dgFields.Size = new System.Drawing.Size(584, 368);
            this.dgFields.TabIndex = 7;
            // 
            // drpScreens
            // 
            this.drpScreens.Location = new System.Drawing.Point(64, 40);
            this.drpScreens.Name = "drpScreens";
            this.drpScreens.Size = new System.Drawing.Size(160, 21);
            this.drpScreens.TabIndex = 6;
            this.drpScreens.SelectedIndexChanged += new System.EventHandler(this.drpScreens_SelectedIndexChanged);
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
            this.menuSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // MandatoryFields
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MandatoryFields";
            this.Text = "Customise Mandatory Fields";
            this.Load += new System.EventHandler(this.MandatoryFields_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgFields)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void menuExit_Click(object sender, System.EventArgs e)
		{
			CloseTab();
		}

		private void menuSave_Click(object sender, System.EventArgs e)
		{
            bool valid = true;

			try
			{
				Function = "menuSave_Click()";
				Wait();

                errorProvider1.SetError(dgFields, string.Empty);

				if(fields.HasChanges(DataRowState.Modified))	//have any records been modified
				{
					//Extract just the records that have changed to cut down traffic
					DataSet changes = fields.GetChanges(DataRowState.Modified);

                    //IP - Check the records changed to ensure that the selections are valid.
                    foreach (DataTable dt in changes.Tables)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            //Display an error if a field is selected as mandatory and either invisible or disabled.
                            if (Convert.ToBoolean(row["mandatory"]) == true
                                    && (Convert.ToBoolean(row["visible"]) == false || Convert.ToBoolean(row["enabled"]) == false))
                            {
                                errorProvider1.SetError(dgFields, GetResource("M_MANDATORYANDINVISIBLEDISABLED"));
                                valid = false;
                            }
                            //Display an error if a field is seen as invisible and enabled.
                            if(Convert.ToBoolean(row["visible"])== false && Convert.ToBoolean(row["enabled"])== true)
                            {
                                errorProvider1.SetError(dgFields, GetResource("M_INVISIBLEANDENABLED"));
                                valid = false;
                            }

                        }
                    }

                    if(valid)
                    {
					    StaticDataManager.SaveMandatoryFields(changes, out Error);
                        if (Error.Length > 0)
                        {
                            ShowError(Error);
                        }
                        else
                        {
                            fields.AcceptChanges();		//commit changes to the dataset
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

		private void drpScreens_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			DataView dv = null;
            
			try
			{
				Wait();
				Function = "MandatoryFields()";

				if(staticLoaded)
				{
					string item = "";
					item = (string)((DataRowView)drpScreens.SelectedItem)[CN.Screen];

					fields = StaticDataManager.GetMandatoryFields(Config.CountryCode, item, out Error);
					if(Error.Length>0)
						ShowError(Error);
					else
					{
						if(fields!=null)
						{
							((MainForm)this.FormRoot).statusBar1.Text = fields.Tables[0].Rows.Count + " Row(s) returned.";

							dv = new DataView(fields.Tables[0]);
							dv.AllowNew = false;
							dgFields.DataSource = dv;

							if(dgFields.TableStyles.Count==0)
							{
								DataGridTableStyle tabStyle = new DataGridTableStyle();
								tabStyle.MappingName = dv.Table.TableName;
								dgFields.TableStyles.Add(tabStyle);
								tabStyle.GridColumnStyles["description"].Width = 350;
								tabStyle.GridColumnStyles["description"].ReadOnly = true;
								tabStyle.GridColumnStyles["description"].HeaderText = GetResource("T_DESCRIPTION");
								tabStyle.GridColumnStyles["control"].Width = 0;
								tabStyle.GridColumnStyles["screen"].Width = 0;
								tabStyle.GridColumnStyles["country"].Width = 0;
								tabStyle.GridColumnStyles["enabled"].Width = 50;
								tabStyle.GridColumnStyles["enabled"].HeaderText = GetResource("T_ENABLED");
								tabStyle.GridColumnStyles["visible"].Width = 50;
								tabStyle.GridColumnStyles["visible"].HeaderText = GetResource("T_VISIBLE");
								tabStyle.GridColumnStyles["mandatory"].Width = 50;
								tabStyle.GridColumnStyles["mandatory"].HeaderText = GetResource("T_MANDATORY");
								((DataGridBoolColumn)tabStyle.GridColumnStyles["enabled"]).AllowNull = false;
								((DataGridBoolColumn)tabStyle.GridColumnStyles["visible"]).AllowNull = false;
								((DataGridBoolColumn)tabStyle.GridColumnStyles["mandatory"]).AllowNull = false;
							}
						}
						else
						{
							((MainForm)this.FormRoot).statusBar1.Text = "0 Row(s) returned.";
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

		private void LoadStatic()
		{
			Function = "MandatoryFields::LoadStatic()";

			XmlUtilities xml = new XmlUtilities();
			XmlDocument screens = new XmlDocument();
			screens.LoadXml("<DROP_DOWNS></DROP_DOWNS>");
			
			if(StaticData.Tables[TN.Screens] == null)
				screens.DocumentElement.AppendChild(xml.CreateDropDownNode(screens,TN.Screens, null));

			if(screens.DocumentElement.ChildNodes.Count>0)
			{
				DataSet ds = StaticDataManager.GetScreens(screens.DocumentElement, out Error);
				if(Error.Length>0)
					ShowError(Error);
				else
				{
					foreach(DataTable dt in ds.Tables)
					{
						switch(dt.TableName)
						{
							default: StaticData.Tables[dt.TableName] = dt;
								break;
						}						
					}
				}
			}

			drpScreens.DataSource = (DataTable)StaticData.Tables[TN.Screens];
			drpScreens.DisplayMember = CN.Screen;

			staticLoaded = true;
		}

		private void MandatoryFields_Load(object sender, System.EventArgs e)
		{
			try
			{
				Function = "MandatoryFields_Load";
				Wait();
				LoadStatic();
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
