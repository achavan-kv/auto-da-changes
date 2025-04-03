using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using STL.Common.Constants.Elements;
using STL.PL.WS7;
using STL.PL.WS5;
using System.Web.Services.Protocols;
using System.Data;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Tags;
using Crownwood.Magic.Menus;
using System.IO;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using STL.Common;
using Blue.Cosacs.Shared;
using STL.Common.Constants.EquifaxScorecard;  ////CR equifax score card,

namespace STL.PL
{
	/// <summary>
	/// Maintenance screen that lists all the rules used by the credit
	/// sanctioning process to either award a customer with a credit limit
	/// or to decline the customer credit. The rules are either scoring 
	/// rules or referal rules. The scoring rules add a number of points to
	/// the customer score when a condition is met, such as a certain age
	/// range or a certain occupation. The referral rules add a referral code
	/// to the application and force it to be referred when a certain condition
	/// is met, such as a low disposable income. The system will either
	/// calculate a suitable credit limit from the total score, or decline the
	/// application if the score is too low.
	/// </summary>
	public class ScoringRules : CommonForm
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		//private Crownwood.Magic.Menus.MenuControl menuMain;
		private Crownwood.Magic.Menus.MenuCommand menuFile;
		private Crownwood.Magic.Menus.MenuCommand menuRules;
		private Crownwood.Magic.Menus.MenuCommand menuSave;
		private Crownwood.Magic.Menus.MenuCommand menuExit;
		private Crownwood.Magic.Menus.MenuCommand menuNewRule;
		private Crownwood.Magic.Menus.MenuCommand menuDeleteRule;
		private Crownwood.Magic.Menus.MenuCommand menuEditRule;
        private System.Windows.Forms.ComboBox drpRuleType;
		private System.Windows.Forms.Button btnDeleteRule;
		private System.Windows.Forms.Button btnEditRule;
		private Crownwood.Magic.Menus.MenuCommand menuExport;
		private Crownwood.Magic.Menus.MenuCommand menuImport;
		private System.Windows.Forms.DataGrid dgRules;
		private XmlDocument rulesDoc = null;
        private System.Windows.Forms.ErrorProvider errorProvider1;
		private new string Error = "";
		private DataSet staticData = null;
		private DataTable rulesTable = null;
		private System.Windows.Forms.ComboBox drpCountry;
		private string countryLoaded = "";
		private string regionLoaded = "";
        private string scorecardloaded = "";
		private System.Windows.Forms.Button btnViewRule;
		private Crownwood.Magic.Menus.MenuCommand menuViewRule;
		private System.Windows.Forms.ImageList menuIcons;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown numDecline;
		private System.Windows.Forms.NumericUpDown numRefer;
		private System.Windows.Forms.NumericUpDown numBureauMax;
		private System.Windows.Forms.NumericUpDown numBureauMin;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtRegion;
		private System.Windows.Forms.Label label8;
        private Label label9;
  
        private Button btnSave;
        private Label label10;
        private ComboBox cmb_scorecardtype;
        private Button btnNewRule;
        private Label lblIntercept;
        private NumericUpDown numInterceptScore;
		private System.ComponentModel.IContainer components;

        //private void HashMenus()
        //{
        //    dynamicMenus = new Hashtable();
        //    dynamicMenus[this.Name + ":menuFile"] = Credential.HasPermission(CosacsPermissionEnum.ImportScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":menuSave"] = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":btnSave"] = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":btnNewRule"] = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":btnEditRule"] = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":btnDeleteRule"] = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":menuEditRule"] = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":menuNewRule"] = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":menuDeleteRule"] = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":menuRules"] = Credential.HasPermission(CosacsPermissionEnum.ImportScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":menuExport"] = Credential.HasPermission(CosacsPermissionEnum.ImportScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":menuImport"] = Credential.HasPermission(CosacsPermissionEnum.ImportScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":drpCountry"] = Credential.HasPermission(CosacsPermissionEnum.ImportScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":btnViewRule"] = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":menuViewRule"] = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":dgRules"] = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093

        //    dynamicMenus[this.Name + ":numDecline"] = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":numRefer"] = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":numBureauMin"] = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":numBureauMax"] = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
        //    dynamicMenus[this.Name + ":numInterceptScore"] = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
        //}

		public ScoringRules(TranslationDummy d )
		{
			InitializeComponent();
			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile, menuRules});
		}

		public ScoringRules(Form root, Form parent)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			FormRoot = root;
			FormParent = parent;
            //HashMenus();
            //this.ApplyRoleRestrictions();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuRules });
            this.menuRules.Enabled = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules) || Credential.HasPermission(CosacsPermissionEnum.ImportScoringRules); ;
            this.menuExport.Enabled = Credential.HasPermission(CosacsPermissionEnum.ImportScoringRules);
            this.menuImport.Enabled = Credential.HasPermission(CosacsPermissionEnum.ImportScoringRules);
            this.btnNewRule.Enabled = this.btnNewRule.Visible = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);
            numBureauMin.Enabled = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
            numBureauMax.Enabled = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
            numDecline.Enabled = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
            numRefer.Enabled = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
            numInterceptScore.Enabled = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);     // #12093
			//TranslateControls();

			//initialise the rules document
			rulesDoc = new XmlDocument();
			rulesDoc.LoadXml("<"+Elements.Rules+"/>");
			rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.Country));
			rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.DeclineScore));
			rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.ReferScore));
			rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.BureauMinimum));
			rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.BureauMaximum));
			rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.Region));
            rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.InterceptScore));      //IP - 05/10/12 - #11409 - CR11404

			rulesDoc.DocumentElement.Attributes[Tags.Country].Value = Config.CountryCode;
			rulesDoc.DocumentElement.Attributes[Tags.DeclineScore].Value = (0).ToString();
			rulesDoc.DocumentElement.Attributes[Tags.ReferScore].Value = (0).ToString();
			rulesDoc.DocumentElement.Attributes[Tags.BureauMinimum].Value = (0).ToString();
			rulesDoc.DocumentElement.Attributes[Tags.BureauMaximum].Value = (0).ToString();
			rulesDoc.DocumentElement.Attributes[Tags.Region].Value = "";
            rulesDoc.DocumentElement.Attributes[Tags.InterceptScore].Value = (0).ToString();                        //IP - 05/10/12 - #11409 - CR11404

			LoadCountries();

			menuMain = new Crownwood.Magic.Menus.MenuControl();
			menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile, menuRules});
			drpRuleType.SelectedIndex = 0;

            var permissionImportScoringRules = Credential.HasPermission(CosacsPermissionEnum.ImportScoringRules);
            var permissionCreateEditScoringRules = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);

            menuFile.Visible = permissionImportScoringRules;
            menuSave.Visible = permissionCreateEditScoringRules;
            btnSave.Visible = permissionCreateEditScoringRules;
            btnNewRule.Visible = permissionCreateEditScoringRules;
            btnEditRule.Visible = false;
            btnDeleteRule.Visible = false;
            menuEditRule.Visible = permissionCreateEditScoringRules;
            menuNewRule.Visible = permissionCreateEditScoringRules;
            menuDeleteRule.Visible = permissionCreateEditScoringRules;
            menuRules.Visible = permissionImportScoringRules;
            menuExport.Visible = permissionImportScoringRules;
            menuImport.Visible = permissionImportScoringRules;
            drpCountry.Visible = permissionImportScoringRules;
            btnViewRule.Visible = false;
            menuViewRule.Visible = permissionCreateEditScoringRules;
            dgRules.Visible = permissionCreateEditScoringRules;

            numDecline.Visible = permissionCreateEditScoringRules;
            numRefer.Visible = permissionCreateEditScoringRules;
            numBureauMin.Visible = permissionCreateEditScoringRules;
            numBureauMax.Visible = permissionCreateEditScoringRules;
            numInterceptScore.Visible = permissionCreateEditScoringRules;

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

                if (StaticData.Tables[TN.ScoreCards] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ScoreCards, new string[] { "SCT", "L" })); //SC CR1034 Behavioural Scoring 15/02/2010

                if (((DataTable)StaticData.Tables[TN.ScoreCards]).Rows.Count == 0 || ((DataTable)StaticData.Tables[TN.Countries]).Rows.Count == 0)
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


                //IP - 08/04/10 - CR1034 - Removed
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
				this.LoadRules();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScoringRules));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.cmb_scorecardtype = new System.Windows.Forms.ComboBox();
            this.btnNewRule = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.txtRegion = new System.Windows.Forms.TextBox();
            this.numBureauMax = new System.Windows.Forms.NumericUpDown();
            this.numBureauMin = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numRefer = new System.Windows.Forms.NumericUpDown();
            this.numDecline = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.drpCountry = new System.Windows.Forms.ComboBox();
            this.drpRuleType = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnViewRule = new System.Windows.Forms.Button();
            this.btnDeleteRule = new System.Windows.Forms.Button();
            this.btnEditRule = new System.Windows.Forms.Button();
            this.dgRules = new System.Windows.Forms.DataGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
            this.menuIcons = new System.Windows.Forms.ImageList(this.components);
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuRules = new Crownwood.Magic.Menus.MenuCommand();
            this.menuNewRule = new Crownwood.Magic.Menus.MenuCommand();
            this.menuDeleteRule = new Crownwood.Magic.Menus.MenuCommand();
            this.menuEditRule = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExport = new Crownwood.Magic.Menus.MenuCommand();
            this.menuImport = new Crownwood.Magic.Menus.MenuCommand();
            this.menuViewRule = new Crownwood.Magic.Menus.MenuCommand();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.lblIntercept = new System.Windows.Forms.Label();
            this.numInterceptScore = new System.Windows.Forms.NumericUpDown();
            //this.numInterceptScore = new System.Windows.Forms.NumericUpDown();
            //this.lblIntercept = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBureauMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBureauMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRefer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDecline)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgRules)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numInterceptScore)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.lblIntercept);
            this.groupBox1.Controls.Add(this.numInterceptScore);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.cmb_scorecardtype);
            this.groupBox1.Controls.Add(this.btnNewRule);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtRegion);
            this.groupBox1.Controls.Add(this.numBureauMax);
            this.groupBox1.Controls.Add(this.numBureauMin);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.numRefer);
            this.groupBox1.Controls.Add(this.numDecline);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.drpCountry);
            this.groupBox1.Controls.Add(this.drpRuleType);
            this.groupBox1.Location = new System.Drawing.Point(8, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 112);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Rule Type";
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave.Location = new System.Drawing.Point(728, 36);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 57;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click_1);
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(152, 64);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(88, 16);
            this.label10.TabIndex = 56;
            this.label10.Text = "ScoreCard";
            // 
            // cmb_scorecardtype
            // 
            this.cmb_scorecardtype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_scorecardtype.Location = new System.Drawing.Point(152, 80);
            this.cmb_scorecardtype.Name = "cmb_scorecardtype";
            this.cmb_scorecardtype.Size = new System.Drawing.Size(144, 21);
            this.cmb_scorecardtype.TabIndex = 55;
            this.cmb_scorecardtype.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // btnNewRule
            // 
            this.btnNewRule.Enabled = false;
            this.btnNewRule.Location = new System.Drawing.Point(635, 37);
            this.btnNewRule.Name = "btnNewRule";
            this.btnNewRule.Size = new System.Drawing.Size(48, 23);
            this.btnNewRule.TabIndex = 54;
            this.btnNewRule.Text = "New";
            this.btnNewRule.Visible = false;
            this.btnNewRule.Click += new System.EventHandler(this.btnNewRule_Click);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(24, 64);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 16);
            this.label8.TabIndex = 49;
            this.label8.Text = "Region";
            // 
            // txtRegion
            // 
            this.txtRegion.Location = new System.Drawing.Point(24, 80);
            this.txtRegion.MaxLength = 3;
            this.txtRegion.Name = "txtRegion";
            this.txtRegion.Size = new System.Drawing.Size(40, 20);
            this.txtRegion.TabIndex = 48;
            this.txtRegion.Leave += new System.EventHandler(this.txtRegion_Leave);
            // 
            // numBureauMax
            // 
            this.numBureauMax.Enabled = false;
            this.numBureauMax.Location = new System.Drawing.Point(432, 80);
            this.numBureauMax.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numBureauMax.Name = "numBureauMax";
            this.numBureauMax.Size = new System.Drawing.Size(88, 20);
            this.numBureauMax.TabIndex = 47;
            // 
            // numBureauMin
            // 
            this.numBureauMin.Enabled = false;
            this.numBureauMin.Location = new System.Drawing.Point(320, 80);
            this.numBureauMin.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numBureauMin.Name = "numBureauMin";
            this.numBureauMin.Size = new System.Drawing.Size(88, 20);
            this.numBureauMin.TabIndex = 46;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(432, 64);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 16);
            this.label6.TabIndex = 45;
            this.label6.Text = "Bureau Maximum";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(320, 64);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(96, 16);
            this.label7.TabIndex = 44;
            this.label7.Text = "Bureau Minimum";
            // 
            // numRefer
            // 
            this.numRefer.Enabled = false;
            this.numRefer.Location = new System.Drawing.Point(432, 40);
            this.numRefer.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numRefer.Name = "numRefer";
            this.numRefer.Size = new System.Drawing.Size(88, 20);
            this.numRefer.TabIndex = 43;
            // 
            // numDecline
            // 
            this.numDecline.Enabled = false;
            this.numDecline.Location = new System.Drawing.Point(320, 40);
            this.numDecline.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numDecline.Name = "numDecline";
            this.numDecline.Size = new System.Drawing.Size(88, 20);
            this.numDecline.TabIndex = 42;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(432, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 16);
            this.label5.TabIndex = 41;
            this.label5.Text = "Refer if score <";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(320, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 16);
            this.label4.TabIndex = 40;
            this.label4.Text = "Decline if score <";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(152, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 16);
            this.label3.TabIndex = 39;
            this.label3.Text = "New Rule Type";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(24, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 16);
            this.label2.TabIndex = 38;
            this.label2.Text = "Country";
            // 
            // drpCountry
            // 
            this.drpCountry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCountry.Enabled = false;
            this.drpCountry.Location = new System.Drawing.Point(24, 40);
            this.drpCountry.Name = "drpCountry";
            this.drpCountry.Size = new System.Drawing.Size(120, 21);
            this.drpCountry.TabIndex = 37;
            this.drpCountry.Visible = false;
            this.drpCountry.SelectedIndexChanged += new System.EventHandler(this.drpCountry_SelectedIndexChanged);
            // 
            // drpRuleType
            // 
            this.drpRuleType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpRuleType.Items.AddRange(new object[] {
            "Referral Rules",
            "Scoring Rules"});
            this.drpRuleType.Location = new System.Drawing.Point(152, 40);
            this.drpRuleType.Name = "drpRuleType";
            this.drpRuleType.Size = new System.Drawing.Size(144, 21);
            this.drpRuleType.TabIndex = 0;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(152, 64);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 16);
            this.label9.TabIndex = 52;
            this.label9.Text = "ScoreCard";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.btnViewRule);
            this.groupBox2.Controls.Add(this.btnDeleteRule);
            this.groupBox2.Controls.Add(this.btnEditRule);
            this.groupBox2.Controls.Add(this.dgRules);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(8, 112);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(776, 360);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Current Rules";
            // 
            // btnViewRule
            // 
            this.btnViewRule.Enabled = false;
            this.btnViewRule.Location = new System.Drawing.Point(704, 24);
            this.btnViewRule.Name = "btnViewRule";
            this.btnViewRule.Size = new System.Drawing.Size(48, 23);
            this.btnViewRule.TabIndex = 5;
            this.btnViewRule.Text = "View";
            this.btnViewRule.Visible = false;
            this.btnViewRule.Click += new System.EventHandler(this.btnView_Click);
            // 
            // btnDeleteRule
            // 
            this.btnDeleteRule.Enabled = false;
            this.btnDeleteRule.Location = new System.Drawing.Point(704, 88);
            this.btnDeleteRule.Name = "btnDeleteRule";
            this.btnDeleteRule.Size = new System.Drawing.Size(48, 23);
            this.btnDeleteRule.TabIndex = 2;
            this.btnDeleteRule.Text = "Delete";
            this.btnDeleteRule.Visible = false;
            this.btnDeleteRule.Click += new System.EventHandler(this.btnDeleteRule_Click);
            // 
            // btnEditRule
            // 
            this.btnEditRule.Enabled = false;
            this.btnEditRule.Location = new System.Drawing.Point(704, 56);
            this.btnEditRule.Name = "btnEditRule";
            this.btnEditRule.Size = new System.Drawing.Size(48, 23);
            this.btnEditRule.TabIndex = 3;
            this.btnEditRule.Text = "Edit";
            this.btnEditRule.Visible = false;
            this.btnEditRule.Click += new System.EventHandler(this.btnEditRule_Click);
            // 
            // dgRules
            // 
            this.dgRules.DataMember = "";
            this.dgRules.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgRules.Location = new System.Drawing.Point(64, 32);
            this.dgRules.Name = "dgRules";
            this.dgRules.ReadOnly = true;
            this.dgRules.Size = new System.Drawing.Size(568, 304);
            this.dgRules.TabIndex = 4;
            this.dgRules.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgRules_MouseUp);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(144, 104);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(384, 72);
            this.label1.TabIndex = 6;
            this.label1.Text = "You do not have sufficient permissions to view the current scoring rules.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.Enabled = false;
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
            this.menuIcons.Images.SetKeyName(4, "");
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.ImageIndex = 0;
            this.menuExit.ImageList = this.menuIcons;
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuRules
            // 
            this.menuRules.Description = "MenuItem";
            this.menuRules.Enabled = false;
            this.menuRules.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuNewRule,
            this.menuDeleteRule,
            this.menuEditRule,
            this.menuExport,
            this.menuImport,
            this.menuViewRule});
            this.menuRules.Text = "&Rules";
            
            // 
            // menuNewRule
            // 
            this.menuNewRule.Description = "MenuItem";
            this.menuNewRule.Enabled = false;
            this.menuNewRule.Text = "&New Rule";
            this.menuNewRule.Visible = false;
            // 
            // menuDeleteRule
            // 
            this.menuDeleteRule.Description = "MenuItem";
            this.menuDeleteRule.Enabled = false;
            this.menuDeleteRule.ImageIndex = 4;
            this.menuDeleteRule.ImageList = this.menuIcons;
            this.menuDeleteRule.Text = "&Delete Rule";
            this.menuDeleteRule.Visible = false;
            // 
            // menuEditRule
            // 
            this.menuEditRule.Description = "MenuItem";
            this.menuEditRule.Enabled = false;
            this.menuEditRule.Text = "&Edit Rule";
            this.menuEditRule.Visible = false;
            // 
            // menuExport
            // 
            this.menuExport.Description = "MenuItem";
            this.menuExport.Enabled = false;
            this.menuExport.ImageIndex = 3;
            this.menuExport.ImageList = this.menuIcons;
            this.menuExport.Text = "E&xport Rules";
            
            this.menuExport.Click += new System.EventHandler(this.menuExport_Click);
            // 
            // menuImport
            // 
            this.menuImport.Description = "MenuItem";
            this.menuImport.Enabled = false;
            this.menuImport.ImageIndex = 2;
            this.menuImport.ImageList = this.menuIcons;
            this.menuImport.Text = "&Import Rules";
            
            this.menuImport.Click += new System.EventHandler(this.menuImport_Click);
            // 
            // menuViewRule
            // 
            this.menuViewRule.Description = "MenuItem";
            this.menuViewRule.Enabled = false;
            this.menuViewRule.Text = "&View Rule";
            this.menuViewRule.Visible = false;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataMember = "";
            // 
            // numInterceptScore
            // 
            /* this.numInterceptScore.Enabled = false;
             this.numInterceptScore.Location = new System.Drawing.Point(544, 81);
             this.numInterceptScore.Maximum = new decimal(new int[] {
             1000,
             0,
             0,
             0});
             this.numInterceptScore.Name = "numInterceptScore";
             this.numInterceptScore.Size = new System.Drawing.Size(88, 20);
             this.numInterceptScore.TabIndex = 58;*/

            //////////////////Equifax 
            this.numInterceptScore.Enabled = false;
            this.numInterceptScore.Location = new System.Drawing.Point(544, 81);
            this.numInterceptScore.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.numInterceptScore.Minimum = new decimal(new int[] {
            20000,
            0,
            0,
            -2147483648});
            this.numInterceptScore.Name = "numInterceptScore";
            this.numInterceptScore.Size = new System.Drawing.Size(88, 20);
            this.numInterceptScore.TabIndex = 58;

            ///////////////////////////// 
            // lblIntercept
            // 
            this.lblIntercept.Location = new System.Drawing.Point(541, 62);
            this.lblIntercept.Name = "lblIntercept";
            this.lblIntercept.Size = new System.Drawing.Size(96, 16);
            this.lblIntercept.TabIndex = 59;
            this.lblIntercept.Text = "Intercept Score";
            // 
            // ScoringRules
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ScoringRules";
            this.Text = "Customise Scoring Rules";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBureauMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBureauMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRefer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDecline)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgRules)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numInterceptScore)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private bool LoadStatic()
		{
			bool status = true;
		
			Function = "WStaticDataManager::GetDropDowns()";
			
			XmlUtilities xml = new XmlUtilities();
			XmlDocument dropDowns = new XmlDocument();
			dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

			if(StaticData.Tables[TN.Bank]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.Bank, null));
			if(StaticData.Tables[TN.EmploymentStatus]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.EmploymentStatus, new string[] {"ES1","L"}));
			if(StaticData.Tables[TN.PayFrequency]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.PayFrequency, new string[] {"PF1","L"}));
			if(StaticData.Tables[TN.MaritalStatus]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.MaritalStatus, new string[] {"MS1","L"}));
			if(StaticData.Tables[TN.Occupation]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.Occupation, new string[] {"WT1","L"}));
			if(StaticData.Tables[TN.ResidentialStatus]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.ResidentialStatus, new string[] {"RS1","L"}));
			if(StaticData.Tables[TN.Title]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.Title, new string[] { "TTL", "L"}));
			if(StaticData.Tables[TN.BankAccountType]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.BankAccountType, new string[] {"BA2","L"}));
			if(StaticData.Tables[TN.Nationality]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.Nationality, new string[] {"NA2","L"}));
			if(StaticData.Tables[TN.EthnicGroup]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.EthnicGroup, new string[] {"EG1","L"}));
			if(StaticData.Tables[TN.ReferralCodes]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.ReferralCodes, new string[] {"SN1","L"}));
			if(StaticData.Tables[TN.PropertyType]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.PropertyType, new string[]{"PT1", "L"}));
			if(StaticData.Tables[TN.Gender]==null)
				dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.Gender, new string[]{"GEN", "L"}));
            if (StaticData.Tables[TN.FlagCustomerStatus] == null)  //CR equifax score card
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.FlagCustomerStatus, new string[] { "CST", "L" }));
            if (StaticData.Tables[TN.MobileNumber] == null)  //CR equifax score card
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.MobileNumber, new string[] { "MBN", "L" }));

            if (dropDowns.DocumentElement.ChildNodes.Count>0)
			{
				DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
				if(Error.Length>0)
					ShowError(Error);
				else
				{
					foreach(DataTable dt in ds.Tables)
					{
						StaticData.Tables[dt.TableName] = dt;
					}
				}
			}
			if(staticData==null)
			{			
				staticData = StaticDataManager.GetScoringOperands(out Error);
				if(Error.Length>0)
				{
					ShowError(Error);
					status = false;
				}
			}
			return status;
		}

        //CR equifax score card, Check for IsEquifaxScorecard used
        public bool IsEquifaxScorecard()
        {
            bool IsEquifaxScorecard = false;
            if (cmb_scorecardtype.SelectedValue.ToString() == EquifaxScorecardStatus.EquifaxApplicant || cmb_scorecardtype.SelectedValue.ToString() == EquifaxScorecardStatus.EquifaxBehavioural)
            {
                IsEquifaxScorecard = true;
            }
            return IsEquifaxScorecard;
        }

        private void btnNewRule_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "btnNewRule_Click()";
				Wait();
				bool status = true;

				status = LoadStatic();

				if(status)
				{
					ScoringRule rule = new ScoringRule(rulesDoc, staticData, drpRuleType.SelectedIndex, FormRoot, this, IsEquifaxScorecard());
					rule.ShowDialog();
					this.RefreshGrid();
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

		private void RefreshGrid()
		{
			Function = "RefreshGrid()";
			if(rulesTable==null)
			{
				rulesTable = new DataTable(Elements.Rules);
				rulesTable.Columns.AddRange(new DataColumn[] {new DataColumn(Tags.RuleName), new DataColumn(Tags.Type), new DataColumn(Tags.Result)});

				if(dgRules.TableStyles.Count==0)
				{
					dgRules.DataSource = rulesTable.DefaultView;
					DataGridTableStyle tabStyle = new DataGridTableStyle();
					tabStyle.MappingName = rulesTable.TableName;
					dgRules.TableStyles.Add(tabStyle);
			
					tabStyle.GridColumnStyles[Tags.RuleName].Width = 350;
					tabStyle.GridColumnStyles[Tags.RuleName].HeaderText = GetResource("T_RULENAME");
					tabStyle.GridColumnStyles[Tags.Type].Width = 90;
					tabStyle.GridColumnStyles[Tags.Type].HeaderText = GetResource("T_RULETYPE");
					tabStyle.GridColumnStyles[Tags.Result].Width = 90;
					tabStyle.GridColumnStyles[Tags.Result].HeaderText = GetResource("T_RULERESULT");
				}
			}
            //CR equifax score card
            else
            {

                if (IsEquifaxScorecard())
                {
                    rulesTable = new DataTable(Elements.Rules);
                    rulesTable.Columns.AddRange(new DataColumn[] { new DataColumn(Tags.RuleName), new DataColumn(Tags.Type), new DataColumn(Tags.Result) });
                    if (dgRules.TableStyles.Count > 0)
                    {
                        dgRules.TableStyles.RemoveAt(0);
                        dgRules.DataSource = rulesTable.DefaultView;
                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        tabStyle.MappingName = rulesTable.TableName;
                        dgRules.TableStyles.Add(tabStyle);

                        tabStyle.GridColumnStyles[Tags.RuleName].Width = 350;
                        tabStyle.GridColumnStyles[Tags.RuleName].HeaderText = GetResource("T_RULENAME");
                        tabStyle.GridColumnStyles[Tags.Type].Width = 90;
                        tabStyle.GridColumnStyles[Tags.Type].HeaderText = GetResource("T_RULETYPE");
                        tabStyle.GridColumnStyles[Tags.Result].Width = 90;
                        tabStyle.GridColumnStyles[Tags.Result].HeaderText = GetResource("T_RULEWEIGHT");
                    }
                }

            }
            rulesTable.Clear();

			try
			{
				numDecline.Value = Convert.ToInt32(rulesDoc.DocumentElement.Attributes[Tags.DeclineScore].Value);
				numRefer.Value = Convert.ToInt32(rulesDoc.DocumentElement.Attributes[Tags.ReferScore].Value);
			}
			catch(NullReferenceException)
			{
				rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.DeclineScore));
				rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.ReferScore));
				rulesDoc.DocumentElement.Attributes[Tags.DeclineScore].Value = (0).ToString();
				rulesDoc.DocumentElement.Attributes[Tags.ReferScore].Value = (0).ToString();
				numRefer.Value = 0;
				numDecline.Value = 0;
			}

			try
			{
				numBureauMin.Value = Convert.ToInt32(rulesDoc.DocumentElement.Attributes[Tags.BureauMinimum].Value);
				numBureauMax.Value = Convert.ToInt32(rulesDoc.DocumentElement.Attributes[Tags.BureauMaximum].Value);
			}
			catch(NullReferenceException)
			{
				rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.BureauMinimum));
				rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.BureauMaximum));
				rulesDoc.DocumentElement.Attributes[Tags.BureauMinimum].Value = (0).ToString();
				rulesDoc.DocumentElement.Attributes[Tags.BureauMaximum].Value = (0).ToString();
				numBureauMin.Value = 0;
				numBureauMax.Value = 0;
			}

            //CR equifax score card
            if (IsEquifaxScorecard())
                this.numInterceptScore.DecimalPlaces = 15;
            else
                this.numInterceptScore.DecimalPlaces = 0;

            //IP - 05/10/12 - #11409 - CR11404
            try
            {
                if (IsEquifaxScorecard())
                    numInterceptScore.Value = Convert.ToDecimal(rulesDoc.DocumentElement.Attributes[Tags.InterceptScore].Value);
                else
                    numInterceptScore.Value = Convert.ToInt32(rulesDoc.DocumentElement.Attributes[Tags.InterceptScore].Value);
            }
            catch
            {
                rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.InterceptScore));
                rulesDoc.DocumentElement.Attributes[Tags.InterceptScore].Value = (0).ToString();
                numInterceptScore.Value = 0;
            }

			foreach(XmlNode rule in rulesDoc.DocumentElement.ChildNodes)
			{
				if(rule.Name==Elements.Rule)
				{	
					DataRow r = rulesTable.NewRow();
					r[Tags.RuleName] = rule.Attributes[Tags.RuleName].Value;
					r[Tags.Type] = rule.Attributes[Tags.Type].Value;
					r[Tags.Result] = rule.Attributes[Tags.Result].Value;
					rulesTable.Rows.Add(r);
				}
			}
			if(rulesTable.Rows.Count>0)
			{
				dgRules.CurrentRowIndex = 0;
				dgRules.Select(0);
			}
		}

		private void btnEditRule_Click(object sender, System.EventArgs e)
		{
			try
			{
				//JJ - cannot use the datagrid current row index to access the 
				//corresponding child node because the datagrid may have been 
				//sorted. Instead we must get the right datarow and search the 
				//Xml document for it. 

				Function = "btnEditRule_Click";
				int index = dgRules.CurrentRowIndex;
				int ruleType = 0;
				if(index>=0)
				{
					LoadStatic();
					XmlNode match=null;
					DataRowView drv = ((DataView)dgRules.DataSource)[index];
					foreach(XmlNode rule in rulesDoc.DocumentElement.ChildNodes)
					{
						if( rule.Attributes[Tags.RuleName].Value == (string)drv[Tags.RuleName] &&
							rule.Attributes[Tags.Type].Value == (string)drv[Tags.Type] &&
							rule.Attributes[Tags.Result].Value == (string)drv[Tags.Result] )
						{
							match = rule;
							break;
						}
					}
					if(match!=null)
					{
						switch(match.Attributes[Tags.Type].Value)
						{
							case "R": ruleType = 0;
								break;
							case "S": ruleType = 1;
								break;
							default:
								break;
						}
						ScoringRule sr = new ScoringRule(rulesDoc, this.staticData, ruleType, match, false, FormRoot, this, IsEquifaxScorecard());
						sr.ShowDialog();
						this.RefreshGrid();
					}
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void btnDeleteRule_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "btnDeleteRule_Click";
				int index = dgRules.CurrentRowIndex;
				if(index>=0)
				{
					XmlNode rule = rulesDoc.DocumentElement.ChildNodes[index];
					rule.ParentNode.RemoveChild(rule);
					this.RefreshGrid();
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void Save()
		{
			try
			{
				Function = "btnSave_Click";
				Wait();
                if (dgRules.VisibleRowCount > 0) //Only save if > 09999 rows
                {


                    if (!rulesDoc.DocumentElement.HasAttribute(Tags.ScoreType)) //SC CR1034 Behavioural Scoring 15/02/2010 Set new scorecard attribute in xml.
                    {
                        XmlAttribute scoretype = rulesDoc.CreateAttribute(Tags.ScoreType);
                        scoretype.Value = cmb_scorecardtype.SelectedValue.ToString();
                        rulesDoc.DocumentElement.Attributes.Append(scoretype);
                    }
                    else
                    {
                        rulesDoc.DocumentElement.Attributes[Tags.ScoreType].Value = cmb_scorecardtype.SelectedValue.ToString();
                    }


                    rulesDoc.DocumentElement.Attributes[Tags.DeclineScore].Value = numDecline.Value.ToString();
                    rulesDoc.DocumentElement.Attributes[Tags.ReferScore].Value = numRefer.Value.ToString();

                    rulesDoc.DocumentElement.Attributes[Tags.BureauMinimum].Value = numBureauMin.Value.ToString();
                    rulesDoc.DocumentElement.Attributes[Tags.BureauMaximum].Value = numBureauMax.Value.ToString();
                    rulesDoc.DocumentElement.Attributes[Tags.InterceptScore].Value = numInterceptScore.Value.ToString();          //IP - 05/10/12 - #11409 - CR11404


                    if (!rulesDoc.DocumentElement.HasAttribute(Tags.Region))
                        rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.Region));

                    rulesDoc.DocumentElement.Attributes[Tags.Region].Value = txtRegion.Text;

                    CreditManager.SaveScoringRules((string)((DataRowView)drpCountry.SelectedItem)[CN.Code],
                                                    rulesDoc.DocumentElement,
                                                    txtRegion.Text,
                                                    out Error);
                    if (Error.Length > 0)
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
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
            Save();
            //CloseTab(); SC CR1034 15/02/10
		}
		
		private void menuExport_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "menuExport_Click";

				MemoryStream fin = new MemoryStream();
				SaveFileDialog save = new SaveFileDialog();
				save.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*" ;
                save.Title = "Save " + cmb_scorecardtype.Text + " Rules Document";
                save.CreatePrompt = true;

                if (!rulesDoc.DocumentElement.HasAttribute(Tags.ScoreType)) //SC CR1034 Behavioural Scoring 15/02/2010 Save scorcard attribute if not already there.
                {
                    XmlAttribute scoretype = rulesDoc.CreateAttribute(Tags.ScoreType);
                    scoretype.Value = cmb_scorecardtype.SelectedValue.ToString();
                    rulesDoc.DocumentElement.Attributes.Append(scoretype);
                }

                //IP - 08/04/10 - CR1034 - Removed
                //if (!rulesDoc.DocumentElement.HasAttribute(Tags.ScoreType)) //SC CR1034 Behavioural Scoring 15/02/2010 Save scorcard attribute if not already there.
                //{
                //    XmlAttribute scoretype = rulesDoc.CreateAttribute(Tags.ScoreType);
                //    scoretype.Value = cmb_scorecardtype.SelectedValue.ToString();
                //    rulesDoc.DocumentElement.Attributes.Append(scoretype);
                //}

				if(save.ShowDialog() == DialogResult.OK)
				{
					//save the document to a memory stream
                    rulesDoc.Save(fin);
					//encrypt the memory stream and save it to file
					EncryptData(fin, save.FileName);

                    MessageBox.Show("Current rules successfully exported and saved.", "Rules saved successfully", MessageBoxButtons.OK);
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void EncryptData(MemoryStream fin, string outName)
		{ 
			//create the file stream to write the encrypted output to
			FileStream fout = new FileStream(outName, FileMode.OpenOrCreate, FileAccess.Write);
			FileStream keys = new FileStream(outName+".key", FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

			//point to the begining of the memory stream
			fin.Position = 0;

			//create a DESEncryption object
			DES des = new DESCryptoServiceProvider();    
			des.GenerateIV();
			des.GenerateKey();

			keys.Write(des.Key, 0, des.Key.Length);
			keys.Write(des.IV, 0, des.IV.Length);
			keys.Close();

			//Create and encryptor
			ICryptoTransform encrypt = des.CreateEncryptor(des.Key, des.IV);

			//Use a crypto stream to encrypt the output
			CryptoStream cryptostream = new CryptoStream(fout, encrypt, CryptoStreamMode.Write);
			cryptostream.Write(fin.GetBuffer(), 0, (fin.GetBuffer()).Length);
			cryptostream.Close();		
		}

		private void menuImport_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "menuImport_Click";

				OpenFileDialog open = new OpenFileDialog();
				open.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*" ;
                open.Title = "Import " + cmb_scorecardtype.Text.ToString() + " Rules Document";

                if (open.ShowDialog() == DialogResult.OK)
                {
                    DecryptData(open.FileName, ref rulesDoc);
                    /*#if(DEBUG)
                                        string s= rulesDoc.DocumentElement.Attributes[Tags.Country].Value ;
                                        string sa=(string)((DataRowView)drpCountry.SelectedItem)[CN.Code];
                    #endif*/
                    if ((bool)Country[CountryParameterNames.LoggingEnabled])
                    {
                        string s = rulesDoc.DocumentElement.Attributes[Tags.Country].Value;
                        string sa = (string)((DataRowView)drpCountry.SelectedItem)[CN.Code];
                    }

                    if (Checkimport(rulesDoc))  //SC CR1034 Behavioural Scoring 15/02/2010
                    {
                        this.RefreshGrid();

                        /* flag the document as newly imported */
                        rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.NewImport));
                        rulesDoc.DocumentElement.Attributes[Tags.NewImport].Value = Boolean.TrueString;

                        /* record the import file name */
                        rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.FileName));
                        rulesDoc.DocumentElement.Attributes[Tags.FileName].Value = open.FileName;

                        Save();

                        rulesDoc.DocumentElement.Attributes[Tags.NewImport].Value = Boolean.FalseString;

                        ShowInfo("M_IMPORTRULESOK");
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private bool Checkimport(XmlDocument rulesDoc) //SC CR1034 Behavioural Scoring 15/02/2010 Checks if loading wrong scorecard for selected type and wrong country.
        {
            bool ok = true;

            if (rulesDoc.DocumentElement.Attributes[Tags.Country].Value !=
                (string)((DataRowView)drpCountry.SelectedItem)[CN.Code])
            {

                ShowInfo("M_RULESWRONGCOUNTRY");
                countryLoaded = "";
                regionLoaded = "";
                scorecardloaded = "";
                this.LoadRules();
                ok = false;
            }

            if (!(!rulesDoc.DocumentElement.HasAttribute(Tags.ScoreType) && cmb_scorecardtype.SelectedValue.ToString() == "A") &&
                rulesDoc.DocumentElement.Attributes[Tags.ScoreType].Value.ToString().ToUpper().Trim() != cmb_scorecardtype.SelectedValue.ToString())
            {
                if (MessageBox.Show("The scorecard you are loading does not match the category selected." +
                    Environment.NewLine +
                    "Are you sure you want to import these rules under " + cmb_scorecardtype.Text + "?" +
                     Environment.NewLine +
                     Environment.NewLine +
                    "Click OK to continue.", "Scorecard type Mismatch", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    countryLoaded = "";
                    regionLoaded = "";
                    scorecardloaded = "";
                    this.LoadRules();
                    ok = false;
                }
            }
            return ok;
        }

        //IP - 08/04/10 - CR1034 - Removed
        //private bool Checkimport(XmlDocument rulesDoc) //SC CR1034 Behavioural Scoring 15/02/2010 Checks if loading wrong scorecard for selected type and wrong country.
        //{
        //    bool ok = true;

        //    if (rulesDoc.DocumentElement.Attributes[Tags.Country].Value !=
        //        (string)((DataRowView)drpCountry.SelectedItem)[CN.Code])
        //    {

        //        ShowInfo("M_RULESWRONGCOUNTRY");
        //        countryLoaded = "";
        //        regionLoaded = "";
        //        scorecardloaded = "";
        //        this.LoadRules();
        //        ok = false;
        //    }

        //    if (!(!rulesDoc.DocumentElement.HasAttribute(Tags.ScoreType) && cmb_scorecardtype.SelectedValue.ToString() == "A") &&
        //        rulesDoc.DocumentElement.Attributes[Tags.ScoreType].Value.ToString().ToUpper().Trim() != cmb_scorecardtype.SelectedValue.ToString())
                
        //    {
        //        if (MessageBox.Show("The scorecard you are loading does not match the category selected." +
        //            Environment.NewLine + 
        //            "Are you sure you want to import these rules under " + cmb_scorecardtype.Text + "?" +
        //             Environment.NewLine +
        //             Environment.NewLine + 
        //            "Click OK to continue.", "Scorecard type Mismatch", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
        //        {
        //            countryLoaded = "";
        //            regionLoaded = "";
        //            scorecardloaded = "";
        //            this.LoadRules();
        //            ok = false;
        //        }
        //    }
        //    return ok;
        //}
		private void DecryptData(string inName, ref XmlDocument rules)
		{  
			CryptoStream cryptostreamDecr=null;
			try
			{
				//Create the file stream to read the encrypted data into
				FileStream fin = new FileStream(inName, FileMode.Open, FileAccess.Read);
				FileStream keys = new FileStream(inName+".key", FileMode.Open, FileAccess.Read);

				byte [] key = new byte[8];
				byte [] iv = new byte[8];

				keys.Read(key, 0, 8);
				keys.Read(iv, 0, 8);
				keys.Close();

				//create a DESEncryption object
				DESCryptoServiceProvider des = new DESCryptoServiceProvider();

				//Create a decryption object
				ICryptoTransform decrypt = des.CreateDecryptor(key, iv);

				//use the crypto stream to decrypt the data
				cryptostreamDecr = new CryptoStream(fin,decrypt,CryptoStreamMode.Read);

				//load the xml doc from the crypto stream directly
				rules.Load(cryptostreamDecr);
			}
			finally
			{
				cryptostreamDecr.Close();
				cryptostreamDecr.Clear();				
			}
		}

		private void dgRules_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int index = dgRules.CurrentRowIndex;

			if(index>=0)
			{
				dgRules.Select(dgRules.CurrentCell.RowNumber);

				if (e.Button == MouseButtons.Right)
				{
					DataGrid ctl = (DataGrid)sender;

					MenuCommand m1 = new MenuCommand(GetResource("P_EDIT"));
					MenuCommand m2 = new MenuCommand(GetResource("P_DELETE"));
					MenuCommand m3 = new MenuCommand(GetResource("P_VIEW"));
					m1.Click += new System.EventHandler(this.btnEditRule_Click);
					m2.Click += new System.EventHandler(this.btnDeleteRule_Click);
					m3.Click += new System.EventHandler(this.btnView_Click);

                    var permissionCreateEditScoringRules = Credential.HasPermission(CosacsPermissionEnum.CreateEditScoringRules);

                    m1.Enabled = permissionCreateEditScoringRules;
                    m2.Enabled = permissionCreateEditScoringRules;
                    m3.Enabled = permissionCreateEditScoringRules;

                    menuEditRule.Enabled = permissionCreateEditScoringRules;
                    menuDeleteRule.Enabled = permissionCreateEditScoringRules;
                    menuViewRule.Enabled = permissionCreateEditScoringRules;

					m1.Enabled = menuEditRule.Enabled;
					m2.Enabled = menuDeleteRule.Enabled;
					m3.Enabled = menuViewRule.Enabled;
					m2.ImageList = menuIcons;
					m2.ImageIndex = 4;

					PopupMenu popup = new PopupMenu();
					popup.Animate = Animate.Yes;
					popup.AnimateStyle = Animation.SlideHorVerPositive;
					popup.MenuCommands.AddRange(new MenuCommand[] {m1,m2,m3});
					MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
				}
			}
		}

        private void LoadRules()
        {

            try
            {
                if (countryLoaded != (string)((DataRowView)drpCountry.SelectedItem)[CN.Code] ||
                    regionLoaded != txtRegion.Text ||
                    scorecardloaded != cmb_scorecardtype.SelectedValue.ToString()) //SC CR1034 Behavioural Scoring 15/02/2010 Reload if scorecard changes.
                {
                    XmlNode r = CreditManager.GetScoringRules((string)((DataRowView)drpCountry.SelectedItem)[CN.Code], Convert.ToChar(cmb_scorecardtype.SelectedValue),
                                                                txtRegion.Text,
                                                                out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        if (r != null)
                        {
                            r = rulesDoc.ImportNode(r, true);
                            rulesDoc.ReplaceChild(r, rulesDoc.DocumentElement);
                            RefreshGrid();
                        }
                        else
                        {
                            rulesDoc.LoadXml("<" + Elements.Rules + "/>");
                            rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.Country));
                            rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.DeclineScore));
                            rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.ReferScore));
                            rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.BureauMinimum));
                            rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.BureauMaximum));
                            rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.InterceptScore));                              //IP - 05/10/12 - #11409 - CR11404
                            rulesDoc.DocumentElement.Attributes[Tags.Country].Value = (string)((DataRowView)drpCountry.SelectedItem)[CN.Code];
                            rulesDoc.DocumentElement.Attributes[Tags.DeclineScore].Value = (0).ToString();
                            rulesDoc.DocumentElement.Attributes[Tags.ReferScore].Value = (0).ToString();
                            rulesDoc.DocumentElement.Attributes[Tags.BureauMinimum].Value = (0).ToString();
                            rulesDoc.DocumentElement.Attributes[Tags.BureauMaximum].Value = (0).ToString();
                            rulesDoc.DocumentElement.Attributes[Tags.InterceptScore].Value = (0).ToString();                                        //IP - 05/10/12 - #11409 - CR11404
                            numRefer.Value = 0;
                            numDecline.Value = 0;
                            RefreshGrid();
                        }
                    }

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error Loading Rules - please re-import or rekey");
                dgRules.DataSource = null;
                rulesTable.Clear();
                dgRules.DataSource = rulesTable;
                //Catch(ex, Function);
                rulesDoc.LoadXml("<" + Elements.Rules + "/>");
                rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.Country));
                rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.DeclineScore));
                rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.ReferScore));
                rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.BureauMinimum));
                rulesDoc.DocumentElement.Attributes.Append(rulesDoc.CreateAttribute(Tags.BureauMaximum));
                rulesDoc.DocumentElement.Attributes[Tags.Country].Value = (string)((DataRowView)drpCountry.SelectedItem)[CN.Code];
                rulesDoc.DocumentElement.Attributes[Tags.DeclineScore].Value = (0).ToString();
                rulesDoc.DocumentElement.Attributes[Tags.ReferScore].Value = (0).ToString();
                rulesDoc.DocumentElement.Attributes[Tags.BureauMinimum].Value = (0).ToString();
                rulesDoc.DocumentElement.Attributes[Tags.BureauMaximum].Value = (0).ToString();
                rulesDoc.DocumentElement.Attributes[Tags.InterceptScore].Value = (0).ToString();                                                         //IP - 05/10/12 - #11409 - CR11404
                numRefer.Value = 0;
                numDecline.Value = 0;
                RefreshGrid();
            }
            finally
            {
                countryLoaded = (string)((DataRowView)drpCountry.SelectedItem)[CN.Code];
                regionLoaded = txtRegion.Text;
                scorecardloaded = cmb_scorecardtype.SelectedValue.ToString();
            }
        }

		private void drpCountry_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				Function = "drpCountry_SelectedIndexChanged";
				Wait();

				this.LoadRules();
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

		private void btnView_Click(object sender, System.EventArgs e)
        {
            try
            {
                //JJ - cannot use the datagrid current row index to access the 
                //corresponding child node because the datagrid may have been 
                //sorted. Instead we must get the right datarow and search the 
                //Xml document for it. 

                Function = "btnViewRule_Click";
                int index = dgRules.CurrentRowIndex;
                int ruleType = 0;
                if (index >= 0)
                {
                    LoadStatic();
                    XmlNode match = null;
                    DataRowView drv = ((DataView)dgRules.DataSource)[index];
                    foreach (XmlNode rule in rulesDoc.DocumentElement.ChildNodes)
                    {
                        if (rule.Attributes[Tags.RuleName].Value == (string)drv[Tags.RuleName] &&
                            rule.Attributes[Tags.Type].Value == (string)drv[Tags.Type] &&
                            rule.Attributes[Tags.Result].Value == (string)drv[Tags.Result])
                        {
                            match = rule;
                            break;
                        }
                    }
                    if (match != null)
                    {
                        switch (match.Attributes[Tags.Type].Value)
                        {
                            case "R": ruleType = 0;
                                break;
                            case "S": ruleType = 1;
                                break;
                            default:
                                break;
                        }
                        ScoringRule sr = new ScoringRule(rulesDoc, this.staticData, ruleType, match, true, FormRoot, this, IsEquifaxScorecard());
                        //ScoringRule sr = new ScoringRule(rulesDoc, this.staticData, ruleType, rule, true, FormRoot, this);
                        sr.ShowDialog();
                        this.RefreshGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }


		private void menuExit_Click(object sender, System.EventArgs e)
		{
			this.CloseTab();
		}

		public override bool ConfirmClose()
		{
			if(menuSave.Enabled)
				this.Save();
			return true;
		}

		private void txtRegion_Leave(object sender, System.EventArgs e)
		{
			try
			{
				Wait();
				this.LoadRules();
			}
			catch(Exception ex)
			{
				Catch(ex, "txtRegion_Leave");
			}
			finally
			{
				StopWait();
			}
		}

        private void cmb_scorecardtype_SelectedValueChanged(object sender, EventArgs e) //SC CR1034 Behavioural Scoring 15/02/2010
        {
            if (cmb_scorecardtype.Items.Count > 0 && cmb_scorecardtype.SelectedValue.ToString() != "")
            {
                LoadRules();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            Save();
        }

       


	}
}
