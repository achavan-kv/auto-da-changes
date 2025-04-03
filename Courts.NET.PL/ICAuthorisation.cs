using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.PL.WS2;
using STL.PL.WS5;
using STL.PL.WS7;
using System.Data;
using STL.Common;
using STL.Common.Static;
using System.Web.Services.Protocols;
using System.Xml;
using System.Collections.Specialized;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using Crownwood.Magic.Menus;
using System.Threading;
using STL.Common.Constants.ScreenModes;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.AuditSource; //IP - 04/02/10 - CR1072 - 3.1.9
using System.Collections.Generic;



namespace STL.PL
{
    /// <summary>
    /// After a new sale has been created the delivery must be authorised before the
    /// goods can be sent to the customer. Authorisation requires various pre-requisites
    /// that vary depending upon the Agreement Terms. These include credit sanctioning
    /// for this customer, payment of a deposit where required and payment of the first
    /// instalment wehere required. This screen will list all accounts that have passed
    /// the minimum pre-requisites and can be filterd for a certain type of account 
    /// (such as cash or HP) and whether a deposit or instalment has been paid.
    /// For each account a set of holding flags is displayed that can be manually cleared
    /// by the user. Once all the holding flags are cleared then the proposal can be 
    /// cleared and the account is authorised for delivery.
    /// A summary of the credit sanction details, agreement details and the account history
    /// can be reviewed here.
    /// </summary>
    public class ICAuthorisation : CommonForm
    {
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox drpHoldFlags;
        private DataSet sorted;
        private DataSet holdFlags;
        private System.Windows.Forms.CheckBox chbIncludeHP;
        private new string Error = "";
        private System.Windows.Forms.DataGrid dgFlags;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ComboBox drpBranch;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
      //  private System.ComponentModel.IContainer components;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private string branchFilter = "";
        private string flagFilter = "";
        //private bool includeCash = false;
        private bool includeHP = false;
        private bool includeRF = false;
        //private bool includePaid = false;
        //private bool includeUnpaid = false;
        //private bool includeItems = false;
        //private bool includeGOL = false;
        private BasicCustomerDetails customerScreen = null;
        private System.Windows.Forms.CheckBox chbIncludeRF;
        private System.Windows.Forms.Label clearProposal;
        private System.Windows.Forms.GroupBox gbAccounts;
        private System.Windows.Forms.GroupBox gbSearch;
        private System.Windows.Forms.GroupBox gbFlags;
        private Crownwood.Magic.Controls.TabControl tcAccounts;
        private Crownwood.Magic.Controls.TabPage tpAccounts;
        private Crownwood.Magic.Controls.TabPage tpSummary;
        private System.Windows.Forms.DataGrid dgAccounts;
        private System.Windows.Forms.Label loadBranches;
        private Crownwood.Magic.Menus.MenuCommand menuHelp;
        private Crownwood.Magic.Menus.MenuCommand deliveryAuthoriseHelp;
        private System.Windows.Forms.Label viewProposal;
        private bool CanRevise = false;


        public ICAuthorisation(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuHelp });
        }

        public ICAuthorisation(Form root, Form parent)
        {
            FormRoot = root;
            FormParent = parent;

            InitializeComponent();

            HashMenus();
            ApplyRoleRestrictions();
            CheckExtraRoleRestriction();

            LoadStatic();

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuHelp });
            //TranslateControls();
        }

        private void CheckExtraRoleRestriction()
        {
            DataRow[] foundrows = { };
            DataSet permissions = StaticDataManager.GetDynamicMenus(Credential.UserId, "BasicCustomerDetails", out Error);
            if (permissions.Tables.Count > 0)
            {
                foundrows = permissions.Tables["Menus"].Select("Control = 'MenuRevise' and Enabled = 1 and Visible = 1");
                if (foundrows.Length > 0)
                {
                    CanRevise = true;
                }
            }
        }


        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":viewProposal"] = this.viewProposal;
            dynamicMenus[this.Name + ":clearProposal"] = this.clearProposal;
            dynamicMenus[this.Name + ":loadBranches"] = this.loadBranches;
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    if (components != null)
            //    {
            //        components.Dispose();
            //    }
            //}
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ICAuthorisation));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.viewProposal = new System.Windows.Forms.Label();
            this.menuHelp = new Crownwood.Magic.Menus.MenuCommand();
            this.deliveryAuthoriseHelp = new Crownwood.Magic.Menus.MenuCommand();
            this.gbAccounts = new System.Windows.Forms.GroupBox();
            this.tcAccounts = new Crownwood.Magic.Controls.TabControl();
            this.tpAccounts = new Crownwood.Magic.Controls.TabPage();
            this.dgAccounts = new System.Windows.Forms.DataGrid();
            this.tpSummary = new Crownwood.Magic.Controls.TabPage();
            this.gbFlags = new System.Windows.Forms.GroupBox();
            this.loadBranches = new System.Windows.Forms.Label();
            this.clearProposal = new System.Windows.Forms.Label();
            this.dgFlags = new System.Windows.Forms.DataGrid();
            this.gbSearch = new System.Windows.Forms.GroupBox();
            this.chbIncludeRF = new System.Windows.Forms.CheckBox();
            this.drpBranch = new System.Windows.Forms.ComboBox();
            this.drpHoldFlags = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chbIncludeHP = new System.Windows.Forms.CheckBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.gbAccounts.SuspendLayout();
            this.tcAccounts.SuspendLayout();
            this.tpAccounts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
            this.gbFlags.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgFlags)).BeginInit();
            this.gbSearch.SuspendLayout();
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
            // viewProposal
            // 
            this.viewProposal.Enabled = false;
            this.viewProposal.Location = new System.Drawing.Point(0, 0);
            this.viewProposal.Name = "viewProposal";
            this.viewProposal.Size = new System.Drawing.Size(100, 23);
            this.viewProposal.TabIndex = 0;
            this.viewProposal.Visible = false;
            // 
            // menuHelp
            // 
            this.menuHelp.Description = "MenuItem";
            this.menuHelp.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.deliveryAuthoriseHelp});
            this.menuHelp.Text = "&Help";
            this.menuHelp.Click += new System.EventHandler(this.menuHelp_Click);
            // 
            // deliveryAuthoriseHelp
            // 
            this.deliveryAuthoriseHelp.Description = "MenuItem";
            this.deliveryAuthoriseHelp.Text = "&About this Screen";
            this.deliveryAuthoriseHelp.Click += new System.EventHandler(this.deliveryAuthoriseHelp_Click);
            // 
            // gbAccounts
            // 
            this.gbAccounts.BackColor = System.Drawing.SystemColors.Control;
            this.gbAccounts.Controls.Add(this.tcAccounts);
            this.gbAccounts.Location = new System.Drawing.Point(8, 164);
            this.gbAccounts.Name = "gbAccounts";
            this.gbAccounts.Size = new System.Drawing.Size(776, 308);
            this.gbAccounts.TabIndex = 13;
            this.gbAccounts.TabStop = false;
            this.gbAccounts.Text = "Accounts Awaiting Clearance";
            // 
            // tcAccounts
            // 
            this.tcAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcAccounts.IDEPixelArea = true;
            this.tcAccounts.Location = new System.Drawing.Point(3, 16);
            this.tcAccounts.Name = "tcAccounts";
            this.tcAccounts.PositionTop = true;
            this.tcAccounts.SelectedIndex = 0;
            this.tcAccounts.SelectedTab = this.tpAccounts;
            this.tcAccounts.ShrinkPagesToFit = false;
            this.tcAccounts.Size = new System.Drawing.Size(770, 289);
            this.tcAccounts.TabIndex = 11;
            this.tcAccounts.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpAccounts,
            this.tpSummary});
            this.tcAccounts.SelectionChanged += new System.EventHandler(this.tcAccounts_SelectionChanged);
            // 
            // tpAccounts
            // 
            this.tpAccounts.Controls.Add(this.dgAccounts);
            this.tpAccounts.Location = new System.Drawing.Point(0, 25);
            this.tpAccounts.Name = "tpAccounts";
            this.tpAccounts.Size = new System.Drawing.Size(770, 264);
            this.tpAccounts.TabIndex = 3;
            this.tpAccounts.Title = "Accounts";
            // 
            // dgAccounts
            // 
            this.dgAccounts.DataMember = "";
            this.dgAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAccounts.Location = new System.Drawing.Point(0, 0);
            this.dgAccounts.Name = "dgAccounts";
            this.dgAccounts.ReadOnly = true;
            this.dgAccounts.Size = new System.Drawing.Size(770, 264);
            this.dgAccounts.TabIndex = 11;
            this.dgAccounts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgAccounts_MouseUp);
            // 
            // tpSummary
            // 
            this.tpSummary.Location = new System.Drawing.Point(0, 25);
            this.tpSummary.Name = "tpSummary";
            this.tpSummary.Selected = false;
            this.tpSummary.Size = new System.Drawing.Size(770, 264);
            this.tpSummary.TabIndex = 4;
            this.tpSummary.Title = "Summary";
            // 
            // gbFlags
            // 
            this.gbFlags.BackColor = System.Drawing.SystemColors.Control;
            this.gbFlags.Controls.Add(this.dgFlags);
            this.gbFlags.Controls.Add(this.loadBranches);
            this.gbFlags.Controls.Add(this.clearProposal);
            this.gbFlags.Location = new System.Drawing.Point(344, 8);
            this.gbFlags.Name = "gbFlags";
            this.gbFlags.Size = new System.Drawing.Size(440, 208);
            this.gbFlags.TabIndex = 15;
            this.gbFlags.TabStop = false;
            this.gbFlags.Text = "Hold Flags";
            // 
            // loadBranches
            // 
            this.loadBranches.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.loadBranches.Enabled = false;
            this.loadBranches.Location = new System.Drawing.Point(64, 102);
            this.loadBranches.Name = "loadBranches";
            this.loadBranches.Size = new System.Drawing.Size(38, 16);
            this.loadBranches.TabIndex = 38;
            this.loadBranches.Visible = false;
            // 
            // clearProposal
            // 
            this.clearProposal.Enabled = false;
            this.clearProposal.Location = new System.Drawing.Point(49, 102);
            this.clearProposal.Name = "clearProposal";
            this.clearProposal.Size = new System.Drawing.Size(38, 16);
            this.clearProposal.TabIndex = 37;
            this.clearProposal.Text = "label1";
            this.clearProposal.Visible = false;
            // 
            // dgFlags
            // 
            this.dgFlags.DataMember = "";
            this.dgFlags.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgFlags.Location = new System.Drawing.Point(24, 24);
            this.dgFlags.Name = "dgFlags";
            this.dgFlags.ReadOnly = true;
            this.dgFlags.Size = new System.Drawing.Size(392, 110);
            this.dgFlags.TabIndex = 14;
            // 
            // gbSearch
            // 
            this.gbSearch.BackColor = System.Drawing.SystemColors.Control;
            this.gbSearch.Controls.Add(this.chbIncludeRF);
            this.gbSearch.Controls.Add(this.drpBranch);
            this.gbSearch.Controls.Add(this.drpHoldFlags);
            this.gbSearch.Controls.Add(this.label1);
            this.gbSearch.Controls.Add(this.label2);
            this.gbSearch.Controls.Add(this.chbIncludeHP);
            this.gbSearch.Controls.Add(this.btnLoad);
            this.gbSearch.Location = new System.Drawing.Point(8, 8);
            this.gbSearch.Name = "gbSearch";
            this.gbSearch.Size = new System.Drawing.Size(336, 208);
            this.gbSearch.TabIndex = 12;
            this.gbSearch.TabStop = false;
            this.gbSearch.Text = "Account Criteria";
            // 
            // chbIncludeRF
            // 
            this.chbIncludeRF.Checked = true;
            this.chbIncludeRF.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbIncludeRF.Location = new System.Drawing.Point(120, 84);
            this.chbIncludeRF.Name = "chbIncludeRF";
            this.chbIncludeRF.Size = new System.Drawing.Size(80, 24);
            this.chbIncludeRF.TabIndex = 17;
            this.chbIncludeRF.TabStop = false;
            this.chbIncludeRF.Text = "Include RF";
            // 
            // drpBranch
            // 
            this.drpBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranch.Location = new System.Drawing.Point(8, 48);
            this.drpBranch.Name = "drpBranch";
            this.drpBranch.Size = new System.Drawing.Size(48, 21);
            this.drpBranch.TabIndex = 16;
            this.drpBranch.SelectedIndexChanged += new System.EventHandler(this.drpBranch_SelectedIndexChanged);
            // 
            // drpHoldFlags
            // 
            this.drpHoldFlags.Location = new System.Drawing.Point(120, 48);
            this.drpHoldFlags.Name = "drpHoldFlags";
            this.drpHoldFlags.Size = new System.Drawing.Size(152, 21);
            this.drpHoldFlags.TabIndex = 2;
            this.drpHoldFlags.Text = "No Restriction";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Branch";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(120, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 23);
            this.label2.TabIndex = 8;
            this.label2.Text = "Accounts Flags Outstanding";
            // 
            // chbIncludeHP
            // 
            this.chbIncludeHP.Checked = true;
            this.chbIncludeHP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbIncludeHP.Enabled = false;
            this.chbIncludeHP.Location = new System.Drawing.Point(6, 84);
            this.chbIncludeHP.Name = "chbIncludeHP";
            this.chbIncludeHP.Size = new System.Drawing.Size(80, 24);
            this.chbIncludeHP.TabIndex = 0;
            this.chbIncludeHP.TabStop = false;
            this.chbIncludeHP.Text = "Include HP";
            this.chbIncludeHP.Visible = false;
            // 
            // btnLoad
            // 
            this.btnLoad.Image = ((System.Drawing.Image)(resources.GetObject("btnLoad.Image")));
            this.btnLoad.Location = new System.Drawing.Point(288, 56);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(32, 32);
            this.btnLoad.TabIndex = 15;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // ICAuthorisation
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.gbAccounts);
            this.Controls.Add(this.gbFlags);
            this.Controls.Add(this.gbSearch);
            this.Name = "ICAuthorisation";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Authorise Instant Credit";
            this.gbAccounts.ResumeLayout(false);
            this.tcAccounts.ResumeLayout(false);
            this.tpAccounts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
            this.gbFlags.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgFlags)).EndInit();
            this.gbSearch.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion


        private void LoadAccountsThread()
        {
            try
            {
                Wait();
                Function = "LoadAccountsThread";

                /* TO DO GET NEW DATA */
                sorted = AccountManager.GetInstantCreditAwaitingClearance(branchFilter,
                    Convert.ToInt32(includeHP),
                    Convert.ToInt32(includeRF),
                    flagFilter,
                    out Error);

                if (Error.Length > 0)
                    ShowError(Error);

            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of LoadAccountsThread";
            }
        }

        private void btnLoad_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "BAccounts::GetAccountsAwaitingClearance";

                //show the hour glass
                Wait();

                if ((string)drpBranch.SelectedItem == "ALL")
                    branchFilter = "%";
                else
                    branchFilter = (string)drpBranch.SelectedItem + "%";

                if (drpHoldFlags.SelectedIndex == -1 || drpHoldFlags.SelectedIndex == 0)
                    flagFilter = "";
                else
                    flagFilter = (string)drpHoldFlags.SelectedItem;

                if(flagFilter != null && flagFilter != string.Empty)
                    flagFilter = flagFilter.Substring(0, 3);

               
                includeHP = chbIncludeHP.Checked;
                includeRF = chbIncludeRF.Checked;
               

                Thread data = new Thread(new ThreadStart(LoadAccountsThread));
                data.Start();
                data.Join();

                if (sorted != null)
                {
                    /* set the row filter to show only cash accounts with a 
                     * Payment oustanding of 0. Loop through them and DA them 
                     * automatically */
                    DataView autoClear = sorted.Tables["Table1"].DefaultView;

                    /* TO DO AUTO DA OF CHEQUE PAYMENTS */
                    //if (!(Convert.ToBoolean(Country[CountryParameterNames.ManualDAFOCAccts])))
                    //{
                    //    autoClear.RowFilter = "(Type = 'C' and PayOS = '0' and ChqOS = '0') or (Type != 'C' and InstantCredit = 'Y' and PayOS = '0' and ChqOS = '0')";
                    //}
                    //else
                    //{
                    //    autoClear.RowFilter = "(Type = 'C' and PayOS = '0' and ChqOS = '0' and FOC = 'N') or  (Type <> 'C' and InstantCredit = 'Y' and PayOS = '0' and ChqOS = '0' and FOC = 'N')";   //Manually DA FOC accounts
                    //}

                    string source = DASource.Auto; //IP - 04/02/10 - CR1072 - 3.1.9 - Added source of Delivery Authorisation.
                    /// Commenting out as now this logic is done at the server end. 
                    //for (int i = 0; i < autoClear.Count; i++)
                    //{
                    //    AccountManager.ClearProposal((string)autoClear[i]["Account"], source, out Error);
                    //    if (Error.Length > 0)
                    //        ShowError(Error);
                    //    else
                    //        autoClear.Delete(i);
                    //}
                    //autoClear.RowFilter = "";
                    /* end of auto clear - JJ */

                    dgAccounts.DataSource = sorted.Tables["Table1"].DefaultView;
                    /* TO DO NEW COLUMNS ON TABLE */
                    if (dgAccounts.TableStyles.Count == 0)
                    {
                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        tabStyle.MappingName = sorted.Tables["Table1"].TableName;
                        dgAccounts.TableStyles.Add(tabStyle);

                        tabStyle.GridColumnStyles["Account"].Width = 90;
                        tabStyle.GridColumnStyles["Account"].ReadOnly = true;
                        tabStyle.GridColumnStyles["Account"].HeaderText = GetResource("T_ACCOUNTNO");

                        tabStyle.GridColumnStyles["Type"].Width = 35;
                        tabStyle.GridColumnStyles["Type"].ReadOnly = true;
                        tabStyle.GridColumnStyles["Type"].HeaderText = GetResource("T_TYPE");

                        tabStyle.GridColumnStyles["Terms"].Width = 40;
                        tabStyle.GridColumnStyles["Terms"].ReadOnly = true;
                        tabStyle.GridColumnStyles["Terms"].HeaderText = GetResource("T_TERMS");

                        tabStyle.GridColumnStyles["Date Opened"].Width = 75;
                        tabStyle.GridColumnStyles["Date Opened"].ReadOnly = true;
                        tabStyle.GridColumnStyles["Date Opened"].HeaderText = GetResource("T_DATEOPENED");

                        tabStyle.GridColumnStyles["Sales Person"].Width = 125;
                        tabStyle.GridColumnStyles["Sales Person"].ReadOnly = true;
                        tabStyle.GridColumnStyles["Sales Person"].HeaderText = GetResource("T_EMPEENAME");

                        tabStyle.GridColumnStyles["Last Updated"].Width = 75;
                        tabStyle.GridColumnStyles["Last Updated"].ReadOnly = true;
                        tabStyle.GridColumnStyles["Last Updated"].HeaderText = GetResource("T_UPDATED");

                        tabStyle.GridColumnStyles["By"].Width = 45;
                        tabStyle.GridColumnStyles["By"].ReadOnly = true;
                        tabStyle.GridColumnStyles["By"].HeaderText = GetResource("T_BY");

                        tabStyle.GridColumnStyles["Name"].Width = 140;
                        tabStyle.GridColumnStyles["Name"].ReadOnly = true;
                        tabStyle.GridColumnStyles["Name"].HeaderText = GetResource("Name");

                        tabStyle.GridColumnStyles["SubAgreement"].Width = 90;
                        tabStyle.GridColumnStyles["SubAgreement"].ReadOnly = true;
                        tabStyle.GridColumnStyles["SubAgreement"].HeaderText = GetResource("T_SUBAGREEMENT");

                        tabStyle.GridColumnStyles["ChqOS"].Width = 0;
                        tabStyle.GridColumnStyles["PayOS"].Width = 0;

                        tabStyle.GridColumnStyles["CustID"].Width = 0;
                        tabStyle.GridColumnStyles["DateProp"].Width = 0;
                        tabStyle.GridColumnStyles["FOC"].Width = 0; //IP/JC - 16/02/10 - CR1048 4.10 (REF:3.1.36) - FOC - CR1072 
                    }

                    if (dgFlags.CurrentRowIndex >= 0)
                        ((DataView)dgFlags.DataSource).Table.Clear();

                    LoadHoldFlags();

                    int num = sorted.Tables[0].Rows.Count;
                    ((MainForm)this.FormRoot).statusBar1.Text = num.ToString() + " Accounts Loaded";
                }
                //reset the cursor
                StopWait();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void LoadStatic()
        {
            Function = "BStaticDataManager::GetDropDownData";

            StringCollection branchNos = new StringCollection();
            branchNos.Add("ALL");

            StringCollection acFlags = new StringCollection();
            acFlags.Add("No Restriction");

            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");


            /* TO DO GET NEW FLAGS */
            if (StaticData.Tables[TN.BranchNumber] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));
            if (StaticData.Tables[TN.InstantCreditFlags] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.InstantCreditFlags, new string[] { "ICF", "L" }));


            if (dropDowns.DocumentElement.ChildNodes.Count > 0)
            {
                DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    foreach (DataTable dt in ds.Tables)
                        StaticData.Tables[dt.TableName] = dt;
                }
            }

            // todo 5.0.0  uat360 select store type C or N if N hide
            // includeRF, call from drpBranch_selectedIndexChanged also


            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
            {
                branchNos.Add(Convert.ToString(row["branchno"]));
            }

            foreach (DataRow row in ((DataTable)StaticData.Tables[TN.InstantCreditFlags]).Rows)
            {
                acFlags.Add(Convert.ToString(row[CN.Code]) + " - " +Convert.ToString(row[CN.CodeDescription]));
            }

            drpHoldFlags.DataSource = acFlags;

            drpBranch.DataSource = branchNos;
            drpBranch.Text = Config.BranchCode;

            int branch = Convert.ToInt32(Config.BranchCode);
            if (loadBranches.Enabled)
                drpBranch.Enabled = true;
            else
                drpBranch.Enabled = false;

            //CR1072 Malaysia Merge (CR1048 now controled by user right)  jec 16/02/10 
            

            if (chbIncludeRF.Enabled == false)
            {
                chbIncludeRF.Visible = false;
                chbIncludeRF.Checked = false;
                chbIncludeHP.Visible = false;
                chbIncludeHP.Checked = false;
            }
            else
            {
                chbIncludeRF.Visible = true;
                chbIncludeRF.Checked = true;
                chbIncludeHP.Visible = true;
                chbIncludeHP.Checked = true;
                chbIncludeHP.Enabled = true;
            }

            // This method does not need to fire here since it has already been fired by the drpBranch_SelectedIndexChanged event which in turn was set off by setting the drpBranch.Text to Config.BranchCode
            //setRFAccountVisibility(Convert.ToInt16(branch));
        }

        private void setRFAccountVisibility(short branchNo)
        {
            if (chbIncludeRF.Enabled == true)   //CR1072 Malaysia Merge - user right
            {
                if (GetStoreType(branchNo) == "C" || branchNo == 0)
                {
                    chbIncludeRF.Checked = true;
                    chbIncludeRF.Visible = true;
                }
                else
                {
                    chbIncludeRF.Checked = false;
                    chbIncludeRF.Visible = false;
                }
            }
        }


        private string GetStoreType(short branchNo)
        {
            string storeType = AccountManager.GetStoreType(branchNo, out Error);
            if (Error.Length > 0)
            {
                ShowError(Error);
            }
            return storeType;
        }

        private void dgAccounts_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (dgAccounts.CurrentRowIndex >= 0)
            {
                dgAccounts.Select(dgAccounts.CurrentCell.RowNumber);

                if (e.Button == MouseButtons.Right)
                {
                    DataGrid ctl = (DataGrid)sender;

                    MenuCommand m1 = new MenuCommand(GetResource("P_REVISE_ACCOUNT"));
                    MenuCommand m2 = new MenuCommand(GetResource("P_ACCOUNT_DETAILS"));
                    MenuCommand m3 = new MenuCommand(GetResource("P_CLEARPROP"));
                    MenuCommand m4 = new MenuCommand(GetResource("P_VIEWPROP"));
                    MenuCommand m5 = new MenuCommand(GetResource("P_VIEWSUMMARY"));

                    m1.Click += new System.EventHandler(this.OnReviseAccount);
                    m2.Click += new System.EventHandler(this.OnAccountDetails);
                    m3.Click += new System.EventHandler(this.OnClearProposal);
                    m4.Click += new System.EventHandler(this.OnLaunchSanctioning);
                    m5.Click += new System.EventHandler(this.OnSummary);

                    PopupMenu popup = new PopupMenu();
                    if (CanRevise)
                    {
                        popup.MenuCommands.Add(m1);
                    }
                    popup.MenuCommands.Add(m2);

                    string type = (string)dgAccounts[dgAccounts.CurrentRowIndex, 1];
                    if (AT.IsCreditType(type) && Credential.HasPermission(Blue.Cosacs.Shared.CosacsPermissionEnum.AuthoriseICViewProposal)) //Acct Type Translation DSR 29/9/03
                        popup.MenuCommands.AddRange(new MenuCommand[] { m4, m5 });

                    if (clearProposal.Enabled)
                        popup.MenuCommands.AddRange(new MenuCommand[] { m3 });

                    MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                }
                else
                    LoadHoldFlags();
            }
        }

        private void OnReviseAccount(object sender, System.EventArgs e)
        {
            try
            {
                Function = "OnReviseAccount";
                Wait();
                int index = dgAccounts.CurrentRowIndex;

                if (index >= 0)
                {
                    string acctno = (string)dgAccounts[index, 0];
                    NewAccount reviseAcct = new NewAccount(acctno.Replace("-", ""), 1, null, false, FormRoot, this);
                    reviseAcct.Text = "Revise Sales Order";
                    ((MainForm)this.FormRoot).AddTabPage(reviseAcct, 6);
                    reviseAcct.SupressEvents = false;
                    if (!reviseAcct.AccountLocked)
                    {
                        reviseAcct.Confirm = false;
                        reviseAcct.CloseTab();
                    }
                }
                else
                {
                    ShowInfo("M_NOACCOUNTSELECTED");
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

        private void OnAccountDetails(object sender, System.EventArgs e)
        {
            try
            {
                Function = "OnAccountDetails";
                int index = dgAccounts.CurrentRowIndex;

                if (index >= 0)
                {
                    string acctNo = (string)dgAccounts[index, 0];
                    AccountDetails details = new AccountDetails(acctNo.Replace("-", ""), FormRoot, this);
                    ((MainForm)this.FormRoot).AddTabPage(details, 7);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void OnClearProposal(object sender, System.EventArgs e)
        {
            Function = "BAccounts::GetICFlags";
            bool clear = true;

            try
            {
                /* TO DO MAY NEED NEW FOR ARREARS */
                int index = dgAccounts.CurrentRowIndex;
                string chqOS = (string)dgAccounts[index, 7];
                string payOS = (string)dgAccounts[index, 8];

                if (chqOS == "1")
                    chqOS = "true";
                else
                    chqOS = "false";

                if (payOS == "1")
                    payOS = "true";
                else
                    payOS = "false";

                //CR1225 Instant Credit - proposal can be cleared unless doc con outstanding
                
                foreach (DataRowView row in (DataView)dgFlags.DataSource)
                {
                    //if (row["Date Cleared"] == DBNull.Value || ((string)row["Date Cleared"]).Length == 0 && row["Flag"].ToString() == "DC")
                    if ((row["Date Cleared"] == DBNull.Value || (Convert.ToString(Convert.ToDateTime(row["Date Cleared"]))).Length == 0) && row["Flag"].ToString() == "DC") //IP - 17/10/11 - #8451 - LW74069 
                    {
                        ShowInfo("M_RELEASE");
                        clear = false;
                        break;
                    }
                }

                
                if (Convert.ToBoolean(chqOS) && clear)
                {
                    if ((bool)Country[CountryParameterNames.AllowDAUnpaid])
                    {
                        if (DialogResult.No == ShowInfo("M_CHQCLEARED", MessageBoxButtons.YesNo))
                        {
                            clear = false;
                        }
                    }
                    else
                    {
                        ShowInfo("M_CHQCLEARED2");
                        clear = false;
                    }
                }
                else	/* issue 2 from 3.4.8.7 log - if chqOS no need to 
						 * show the payOS message as well */
                {
                    if (Convert.ToBoolean(payOS) && clear)
                    {
                        if ((bool)Country[CountryParameterNames.AllowDAUnpaid])
                        {
                            if (DialogResult.No == ShowInfo("M_UNPAID", MessageBoxButtons.YesNo))
                            {
                                clear = false;
                            }
                        }
                        else
                        {
                            ShowInfo("M_UNPAID2");
                            clear = false;
                        }
                    }
                }

                if (clear)
                {
                    string source = DASource.Manual; //IP - 04/02/10 - CR1072 - 3.1.9 - Added source of Delivery Authorisation.

                    AccountManager.ClearProposal((string)dgAccounts[index, 0], source, out Error);

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        ((DataView)dgAccounts.DataSource).Delete(index);
                        LoadHoldFlags();
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }
        /* TO DO - NEW HOLD FLAGS */
        private void LoadHoldFlags()
        {
            int index = dgAccounts.CurrentRowIndex;
            Function = "BLoadHoldFlags";
            Wait();

            try
            {
                if (index >= 0)
                {
                    holdFlags = AccountManager.GetICFlags((string)dgAccounts[index, 0], out Error);

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                        dgFlags.DataSource = holdFlags.Tables["HoldFlags"].DefaultView;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            StopWait();
        }

        //private void dgFlags_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    int index = dgFlags.CurrentRowIndex;

        //    if (e.Button == MouseButtons.Right)
        //    {
        //        if (dgFlags.CurrentRowIndex >= 0 &&
        //            (Convert.IsDBNull(dgFlags[index, 1]) || ((string)dgFlags[index, 1]).Length == 0))
        //        {
        //            DataGrid ctl = (DataGrid)sender;

        //            MenuCommand m1 = new MenuCommand(GetResource("P_CLEARFLAG"));

        //            m1.Click += new System.EventHandler(this.OnClearFlag);

        //            PopupMenu popup = new PopupMenu();
        //            popup.MenuCommands.AddRange(new MenuCommand[] { m1 });
        //            MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
        //        }
        //    }
        //}

        //private void OnClearFlag(object sender, System.EventArgs e)
        //{
        //    Function = "BProposal::ClearFlags";
        //    int index = dgAccounts.CurrentRowIndex;
        //    Wait();

        //    try
        //    {
        //        /* TO DO DOES THIS NEED TO CHANGE???? */
        //        string custID = (string)dgAccounts[index, 9];
        //        //DateTime dateProp = (DateTime)dgAccounts[index,10];
        //        DateTime dateProp = (DateTime)((DataView)dgAccounts.DataSource)[index]["DateProp"];
        //        string acctNo = ((DataView)dgAccounts.DataSource)[index]["Account"].ToString();
        //        string chkType = (string)dgFlags[dgFlags.CurrentRowIndex, 0];


        //        AccountManager.ClearFlag(custID, chkType, dateProp, false, acctNo, out Error);

        //        if (Error.Length > 0)
        //            ShowError(Error);
        //        else
        //            LoadHoldFlags();
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, Function);
        //    }
        //    StopWait();
        //}

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }
        private void OnLaunchSanctioning(object sender, System.EventArgs e)
        {
            Function = "OnLaunchSanctioning";

            CommonForm cf = new CommonForm();

            string acctNo = "";
            string custID = "";
            string type = "";

            int index = dgAccounts.CurrentRowIndex;
            Wait();

            try
            {
                cf.Name = "ICAuthorisation";
                if (index >= 0)
                {
                    //acctNo = (string)dgAccounts[index, 0];
                    //type = (string)dgAccounts[index, 1];
                    //custID = (string)dgAccounts[index, 9];

                    //#13584
                    acctNo = ((DataView)(dgAccounts.DataSource))[index]["Account"].ToString(); 
                    type = ((DataView)(dgAccounts.DataSource))[index]["Type"].ToString();
                    custID = ((DataView)(dgAccounts.DataSource))[index]["Custid"].ToString(); 
                }

                string[] parms = new String[3];
                parms[0] = custID;
                parms[1] = acctNo;
                parms[2] = type;
                SanctionStage1 s1 = new SanctionStage1(true, parms, SM.View, FormRoot,
                        null, customerScreen);

                ((MainForm)this.FormRoot).AddTabPage(s1, 18);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            StopWait();
        }

        private void OnSummary(object sender, System.EventArgs e)
        {
            try
            {
                tcAccounts.SelectedTab = tpSummary;
            }
            catch (Exception ex)
            {
                Catch(ex, "OnSummary");
            }
            finally
            {
                StopWait();
            }
        }

        private int destY = 8;
        private string direction = "UP";

        private void LaunchSummary()
        {
            while (Math.Abs(gbAccounts.Location.Y - destY) > 1)
            {
                int newLocation = gbAccounts.Location.Y - ((gbAccounts.Location.Y - destY) / 2);
                int newHeight = gbAccounts.Height + gbAccounts.Location.Y - newLocation;

                if (direction == "UP")
                {
                    gbAccounts.Location = new Point(gbAccounts.Location.X, newLocation);
                    gbAccounts.Height = newHeight;
                }
                else
                {
                    gbAccounts.Height = newHeight;
                    gbAccounts.Location = new Point(gbAccounts.Location.X, newLocation);
                }
                Thread.Sleep(40);
            }
            gbAccounts.Location = new Point(gbAccounts.Location.X, destY);
        }

        private void tcAccounts_SelectionChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                Thread summary = null;

                int index = dgAccounts.CurrentRowIndex;

                if (index >= 0)
                {
                    DataView dv = (DataView)dgAccounts.DataSource;
                    string type = (string)dv[index][CN.Type];
                    if (AT.IsCreditType(type))
                    {
                        switch (tcAccounts.SelectedTab.Name)
                        {
                            case "tpSummary":
                                string acctNo = (string)dv[index][CN.Account];
                                string custID = (string)dv[index][CN.CustID];
                                DateTime dateProp = (DateTime)dv[index][CN.DateProp];

                                destY = 8;
                                direction = "UP";
                                summary = new Thread(new ThreadStart(LaunchSummary));
                                summary.Start();

                                if (tpSummary.Control == null)
                                    tpSummary.Control = new ReferralSummary();

                                XmlNode LineItems = null;
                                DataSet details = CreditManager.GetReferralSummaryData(acctNo, custID, type, dateProp, out LineItems, out Error);
                                if (Error.Length > 0)
                                    ShowError(Error);

                                ((ReferralSummary)tpSummary.Control).LoadDetails(FormRoot, this, type, details, LineItems);

                                break;
                            case "tpAccounts":
                                destY = 216;
                                direction = "DOWN";
                                summary = new Thread(new ThreadStart(LaunchSummary));
                                summary.Start();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                        tcAccounts.SelectedTab = tpAccounts;
                }
                else
                    tcAccounts.SelectedTab = tpAccounts;
            }
            catch (Exception ex)
            {
                Catch(ex, "tcAccounts_SelectionChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void menuHelp_Click(object sender, System.EventArgs e)
        {

        }

        private void deliveryAuthoriseHelp_Click(object sender, System.EventArgs e)
        {
            deliveryAuthorise_HelpRequested(this, null);
        }

        private void deliveryAuthorise_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
        {
            string fileName = "Delivery_Authorisation_Screen.htm";
            LaunchHelp(fileName);
        }

        private void drpBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (drpBranch.SelectedValue.ToString() == "ALL")
                {
                    setRFAccountVisibility(0);
                }
                else
                {
                    setRFAccountVisibility(Convert.ToInt16(drpBranch.SelectedValue.ToString()));
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "drpBranch_SelectedIndexChanged");
            }
        }

    }
}
