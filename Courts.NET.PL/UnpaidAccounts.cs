using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Crownwood.Magic.Menus;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using Blue.Cosacs.Shared;

namespace STL.PL
{
    /// <summary>
    /// Lists all unpaid accounts for a specified branch. One or more individual
    /// accounts can be selected. A cancel reason and notes can be entered and 
    /// the selected accounts then cancelled.
    /// </summary>
    public class UnpaidAccounts : CommonForm
    {
        private DataTable _branchData;
        private DataTable _cancelReasons;
        private string error = "";
        private StringCollection _sc = new StringCollection();
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem cmenuAccountDtls;
        private System.Windows.Forms.GroupBox gbAccount;
        private System.Windows.Forms.TextBox txtProposalNotes;
        private System.Windows.Forms.DataGrid dgAccounts;
        private System.Windows.Forms.Label lbBranchNo;
        private System.Windows.Forms.ComboBox drpBranchNo;
        private System.Windows.Forms.Label lbNotes;
        private System.Windows.Forms.GroupBox gbResults;
        // private System.ComponentModel.IContainer components;
        private System.Windows.Forms.TextBox txtCancelNotes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox drpCancelReason;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCancel;
        private string _rowsRetrievedText = string.Empty;
        private System.Windows.Forms.Label currentBranch;
        private System.Windows.Forms.Label allBranches;
        private System.Windows.Forms.Label allowCancel;
        private System.Windows.Forms.Button btnLoad;
        private int _lastGridIndex = 0;
        private bool allowCancelbool = false;                   // #11097


        public UnpaidAccounts(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
            //Keep track of changes so that a close confirmation can be displayed if unsaved
            //changes exist.
            AddKeyPressedEventHandlers(gbResults);
            //Determine user rights
            HashMenus();
            ApplyRoleRestrictions();
        }

        public UnpaidAccounts(Form root, Form parent)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
            //Keep track of changes so that a close confirmation can be displayed if unsaved
            //changes exist.
            AddKeyPressedEventHandlers(gbResults);
            //Determine user rights
            HashMenus();
            ApplyRoleRestrictions();

            allowCancelbool = Credential.HasPermission(CosacsPermissionEnum.CancelAccount);    // #11097
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            //if( disposing )
            //{
            //    if(components != null)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnpaidAccounts));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.gbAccount = new System.Windows.Forms.GroupBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.gbResults = new System.Windows.Forms.GroupBox();
            this.currentBranch = new System.Windows.Forms.Label();
            this.dgAccounts = new System.Windows.Forms.DataGrid();
            this.txtProposalNotes = new System.Windows.Forms.TextBox();
            this.lbNotes = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtCancelNotes = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.allBranches = new System.Windows.Forms.Label();
            this.allowCancel = new System.Windows.Forms.Label();
            this.drpBranchNo = new System.Windows.Forms.ComboBox();
            this.lbBranchNo = new System.Windows.Forms.Label();
            this.drpCancelReason = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.cmenuAccountDtls = new System.Windows.Forms.MenuItem();
            this.gbAccount.SuspendLayout();
            this.gbResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
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
            // gbAccount
            // 
            this.gbAccount.BackColor = System.Drawing.SystemColors.Control;
            this.gbAccount.Controls.Add(this.btnLoad);
            this.gbAccount.Controls.Add(this.gbResults);
            this.gbAccount.Controls.Add(this.drpBranchNo);
            this.gbAccount.Controls.Add(this.lbBranchNo);
            this.gbAccount.Controls.Add(this.drpCancelReason);
            this.gbAccount.Controls.Add(this.label2);
            this.gbAccount.Location = new System.Drawing.Point(0, 0);
            this.gbAccount.Name = "gbAccount";
            this.gbAccount.Size = new System.Drawing.Size(792, 480);
            this.gbAccount.TabIndex = 0;
            this.gbAccount.TabStop = false;
            // 
            // btnLoad
            // 
            this.btnLoad.CausesValidation = false;
            this.btnLoad.Image = ((System.Drawing.Image)(resources.GetObject("btnLoad.Image")));
            this.btnLoad.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnLoad.Location = new System.Drawing.Point(360, 24);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(32, 23);
            this.btnLoad.TabIndex = 45;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // gbResults
            // 
            this.gbResults.Controls.Add(this.currentBranch);
            this.gbResults.Controls.Add(this.dgAccounts);
            this.gbResults.Controls.Add(this.txtProposalNotes);
            this.gbResults.Controls.Add(this.lbNotes);
            this.gbResults.Controls.Add(this.btnSave);
            this.gbResults.Controls.Add(this.txtCancelNotes);
            this.gbResults.Controls.Add(this.label1);
            this.gbResults.Controls.Add(this.btnCancel);
            this.gbResults.Controls.Add(this.allBranches);
            this.gbResults.Controls.Add(this.allowCancel);
            this.gbResults.Location = new System.Drawing.Point(8, 56);
            this.gbResults.Name = "gbResults";
            this.gbResults.Size = new System.Drawing.Size(776, 416);
            this.gbResults.TabIndex = 44;
            this.gbResults.TabStop = false;
            this.gbResults.Text = "Unpaid Account Details";
            // 
            // currentBranch
            // 
            this.currentBranch.Location = new System.Drawing.Point(344, 8);
            this.currentBranch.Name = "currentBranch";
            this.currentBranch.Size = new System.Drawing.Size(80, 16);
            this.currentBranch.TabIndex = 43;
            this.currentBranch.Text = "currentBranch";
            this.currentBranch.Visible = false;
            // 
            // dgAccounts
            // 
            this.dgAccounts.DataMember = "";
            this.dgAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAccounts.Location = new System.Drawing.Point(0, 128);
            this.dgAccounts.Name = "dgAccounts";
            this.dgAccounts.ReadOnly = true;
            this.dgAccounts.Size = new System.Drawing.Size(776, 288);
            this.dgAccounts.TabIndex = 42;
            this.dgAccounts.CurrentCellChanged += new System.EventHandler(this.dgAccounts_CurrentCellChanged);
            this.dgAccounts.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgAccounts_MouseDown);
            this.dgAccounts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgAccounts_MouseUp);
            // 
            // txtProposalNotes
            // 
            this.txtProposalNotes.Location = new System.Drawing.Point(104, 24);
            this.txtProposalNotes.MaxLength = 1000;
            this.txtProposalNotes.Multiline = true;
            this.txtProposalNotes.Name = "txtProposalNotes";
            this.txtProposalNotes.Size = new System.Drawing.Size(240, 96);
            this.txtProposalNotes.TabIndex = 41;
            // 
            // lbNotes
            // 
            this.lbNotes.Location = new System.Drawing.Point(8, 32);
            this.lbNotes.Name = "lbNotes";
            this.lbNotes.Size = new System.Drawing.Size(88, 24);
            this.lbNotes.TabIndex = 2;
            this.lbNotes.Text = "Proposal Notes";
            this.lbNotes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(8, 88);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(88, 32);
            this.btnSave.TabIndex = 40;
            this.btnSave.Text = "Save Proposal Notes";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtCancelNotes
            // 
            this.txtCancelNotes.Enabled = false;
            this.txtCancelNotes.Location = new System.Drawing.Point(520, 24);
            this.txtCancelNotes.MaxLength = 1000;
            this.txtCancelNotes.Multiline = true;
            this.txtCancelNotes.Name = "txtCancelNotes";
            this.txtCancelNotes.Size = new System.Drawing.Size(240, 96);
            this.txtCancelNotes.TabIndex = 41;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(392, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "Cancellation Notes";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(424, 88);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 32);
            this.btnCancel.TabIndex = 40;
            this.btnCancel.Text = "Cancel Account(s)";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // allBranches
            // 
            this.allBranches.Location = new System.Drawing.Point(432, 8);
            this.allBranches.Name = "allBranches";
            this.allBranches.Size = new System.Drawing.Size(80, 16);
            this.allBranches.TabIndex = 43;
            this.allBranches.Text = "allBranches";
            this.allBranches.Visible = false;
            // 
            // allowCancel
            // 
            this.allowCancel.Location = new System.Drawing.Point(528, 8);
            this.allowCancel.Name = "allowCancel";
            this.allowCancel.Size = new System.Drawing.Size(80, 16);
            this.allowCancel.TabIndex = 43;
            this.allowCancel.Text = "allowCancel";
            this.allowCancel.Visible = false;
            // 
            // drpBranchNo
            // 
            this.drpBranchNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranchNo.Location = new System.Drawing.Point(112, 24);
            this.drpBranchNo.Name = "drpBranchNo";
            this.drpBranchNo.Size = new System.Drawing.Size(240, 21);
            this.drpBranchNo.TabIndex = 43;
            this.drpBranchNo.SelectionChangeCommitted += new System.EventHandler(this.drpBranchNo_SelectionChangeCommitted);
            // 
            // lbBranchNo
            // 
            this.lbBranchNo.Location = new System.Drawing.Point(48, 24);
            this.lbBranchNo.Name = "lbBranchNo";
            this.lbBranchNo.Size = new System.Drawing.Size(56, 24);
            this.lbBranchNo.TabIndex = 2;
            this.lbBranchNo.Text = "Branch";
            this.lbBranchNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpCancelReason
            // 
            this.drpCancelReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCancelReason.Location = new System.Drawing.Point(528, 24);
            this.drpCancelReason.Name = "drpCancelReason";
            this.drpCancelReason.Size = new System.Drawing.Size(240, 21);
            this.drpCancelReason.TabIndex = 43;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(432, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "Cancel Reason";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.cmenuAccountDtls});
            // 
            // cmenuAccountDtls
            // 
            this.cmenuAccountDtls.Index = 0;
            this.cmenuAccountDtls.Text = "Account Details";
            // 
            // UnpaidAccounts
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.gbAccount);
            this.Name = "UnpaidAccounts";
            this.Text = "Unpaid Accounts";
            this.Load += new System.EventHandler(this.UnpaidAccounts_Load);
            this.gbAccount.ResumeLayout(false);
            this.gbResults.ResumeLayout(false);
            this.gbResults.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Set all screen controls to their default values.
        /// </summary>
        private void RefreshGrid()
        {
            // Initial custom settings
            ClearControls(gbResults.Controls);
            PopulateGrid(0);
            dgAccounts.Focus();
        }

        /// <summary>
        /// Populate dropdowns and retrieve any static data that can be preserved for the
        /// duration of this forms usage.
        /// </summary>
        private void LoadStaticData()
        {
            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

            if (StaticData.Tables[TN.BranchNumber] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));

            if (StaticData.Tables[TN.CancelReasons] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CancelReasons, new string[] { "CN2", "L" }));

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

            //Now customise the dropdowns..         

            //Customise the Branch data to be displayed in the dropdown..
            if (_branchData == null)
            {
                _branchData = ((DataTable)StaticData.Tables[TN.BranchNumber]).Clone();

                DataRow row = _branchData.NewRow();
                row[CN.BranchNo] = 0;
                row[CN.BranchName] = GetResource("AllValues");
                _branchData.Rows.Add(row);
                foreach (DataRow copyRow in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                {
                    row = _branchData.NewRow();
                    row[CN.BranchNo] = copyRow[CN.BranchNo];
                    row[CN.BranchName] = copyRow[CN.BranchNo].ToString()
                        + " : " + copyRow[CN.BranchName].ToString();
                    _branchData.Rows.Add(row);
                }
            }

            drpBranchNo.DataSource = _branchData;
            drpBranchNo.ValueMember = CN.BranchNo;
            drpBranchNo.DisplayMember = CN.BranchName;
            drpBranchNo.SelectedValue = Config.BranchCode;

            if (_cancelReasons == null)
            {
                _cancelReasons = ((DataTable)StaticData.Tables[TN.CancelReasons]).Copy();
                drpCancelReason.DataSource = _cancelReasons;
                drpCancelReason.ValueMember = CN.Code;
                drpCancelReason.DisplayMember = CN.CodeDescript;
                if (_cancelReasons.Rows.Count > 0)
                {
                    drpCancelReason.SelectedIndex = 0;
                }
            }
            //Enable/Disable txtCancelNotes depending on Country
            //if ((bool)Country[CountryParameterNames.CancellationNotes]
            //    && this.allowCancel.Enabled)
            //    txtCancelNotes.Enabled = true;

            if ((bool)Country[CountryParameterNames.CancellationNotes]
            && allowCancelbool)            // #11097
                txtCancelNotes.Enabled = true;

            //Enable/Disable drpBranchNo ComboBox depending on user rights
            //drpBranchNo.Enabled = allBranches.Enabled;

            if (Credential.HasPermission(CosacsPermissionEnum.UnpaidAcctsAllBranches))  // #11097
            {
                drpBranchNo.Enabled = true;
            }
            else 
            {
                drpBranchNo.Enabled = false;
            }

        }

        //
        // Events
        //

        //This method is called once, just before the screen is displayed.
        private void UnpaidAccounts_Load(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Unpaid Accounts screen: Form Load";
                Wait();
                this.LoadStaticData();
                RefreshGrid();
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

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Unpaid Accounts Screen: Save";
                Wait();

                int selectedRow = dgAccounts.CurrentRowIndex;
                UpdateAccount(selectedRow);
                PopulateGrid(selectedRow);
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

        private void fileExit_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Unpaid Accounts Screen: File - Exit";
                Wait();
                CloseTab();
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


        /// <summary>
        /// Makes a call to the AccountManager web service to update Account data
        /// using screen values.
        /// </summary>
        /// <param name="index">
        /// The row index in the DataGrid that identifies which account we are going to update.
        /// </param>
        private void UpdateAccount(int index)
        {
            try
            {
                Wait();

                if (index >= 0)
                {
                    int custCol = dgAccounts.TableStyles[0].GridColumnStyles.IndexOf(dgAccounts.TableStyles[0].GridColumnStyles[CN.CustID]);
                    string custId = dgAccounts[index, custCol].ToString();
                    int acctCol = dgAccounts.TableStyles[0].GridColumnStyles.IndexOf(dgAccounts.TableStyles[0].GridColumnStyles[CN.acctno]);
                    string acctNo = dgAccounts[index, acctCol].ToString();
                    int datePropCol = dgAccounts.TableStyles[0].GridColumnStyles.IndexOf(dgAccounts.TableStyles[0].GridColumnStyles[CN.DateProp.ToLower()]);
                    DateTime dateProp = Convert.ToDateTime(dgAccounts[index, datePropCol]);

                    CreditManager.SaveProposalNotes(custId, acctNo, dateProp, txtProposalNotes.Text, out error);
                    if (error.Length > 0)
                    {
                        ShowError(error);
                        StopWait();
                        return;
                    }
                    else
                    {
                        this._hasdatachanged = false;
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
            }
        }

        private void btnLoad_Click(object sender, System.EventArgs e)
        {
            RefreshGrid();
        }

        private void dgAccounts_CurrentCellChanged(object sender, System.EventArgs e)
        {
            int index = dgAccounts.CurrentRowIndex;

            //Check if there were outstanding changes for the previously selected row
            if (_lastGridIndex != index && _hasdatachanged)
            {
                if (DialogResult.Yes == ShowInfo("M_SAVECHANGES2", MessageBoxButtons.YesNo))
                {
                    UpdateAccount(_lastGridIndex);
                    //Repopulate grid (so change just made is reflected) but reposition
                    //the selected row index to the one we were moving to prior to saving
                    //the previous change
                    PopulateGrid(index);
                }
                else //Discard changes
                {
                    _hasdatachanged = false;
                }
            }

            if (index >= 0)
            {
                //Now we can finally change the data that appears in the Proposal Notes TextBox to correspond
                //to the new row we are navigating to. 
                int notesCol = dgAccounts.TableStyles[0].GridColumnStyles.IndexOf(dgAccounts.TableStyles[0].GridColumnStyles[CN.Notes]);
                txtProposalNotes.Text = dgAccounts[index, notesCol].ToString();
                _lastGridIndex = index;
                //** NOTE: The TextBox data change will be detected by our Common code and
                //_hasdatachanged will be set to true - WE DON'T WANT THAT IN THIS CASE because
                //data hasn't actually been modified by the user - so override the flag.
                _hasdatachanged = false;
            }
        }

        private void PopulateGrid(int selectRow)
        {
            try
            {
                Wait();
                //User Rights : Minimum - "Current User" - don't need to check for this
                //                                         because the screen won't even be
                //                                         available if user doesn't at least
                //                                         have this.
                //              Next Level - "Current Branch" - allows user to see data for
                //                                              all users in the selected 
                //                                              branch (which will be the current
                //                                              user's branch if they don't have
                //                                              the ability to change branches).
                //              Top Level - "All Branches" - User can select any branch.
                int branchNo = Convert.ToInt32(drpBranchNo.SelectedValue);
                int empeeNoSale = Credential.UserId;
                if (this.currentBranch.Enabled || this.allBranches.Enabled)
                {
                    empeeNoSale = 0;//Allow all users for selected branch
                }

                //Populate the grid with ALL Account data and select the row
                //corresponding to the Account entry currently being edited
                DataSet unpaidAccounts = AccountManager.GetUnpaidAccounts(branchNo,
                    empeeNoSale,
                    out error);

                if (error.Length > 0)
                {
                    ShowError(error);
                    StopWait();
                    return;
                }
                bool dataFound = false;
                string statusText = GetResource("M_ACCOUNTSZERO");
                foreach (DataTable dt in unpaidAccounts.Tables)
                {
                    if (dt.TableName == TN.Accounts)
                    {
                        dataFound = (dt.Rows.Count > 0);
                        statusText = dt.Rows.Count + GetResource("M_ACCOUNTSLISTED");

                        if (dataFound)
                        {
                            DataView accountsListView = new DataView(dt);
                            dgAccounts.DataSource = accountsListView;
                            dgAccounts.ReadOnly = true;
                            accountsListView.AllowNew = false;

                            if (dgAccounts.TableStyles.Count == 0)
                            {
                                DataGridTableStyle tabStyle = new DataGridTableStyle();
                                tabStyle.MappingName = accountsListView.Table.TableName;

                                dgAccounts.TableStyles.Clear();
                                dgAccounts.TableStyles.Add(tabStyle);

                                // Displayed columns

                                //Column Widths
                                tabStyle.GridColumnStyles[CN.SalesPerson].Width = 150;
                                tabStyle.GridColumnStyles[CN.acctno].Width = 90;
                                tabStyle.GridColumnStyles[CN.AcctType].Width = 60;
                                tabStyle.GridColumnStyles[CN.DateAcctOpen].Width = 80;
                                tabStyle.GridColumnStyles[CN.AgrmtTotal].Width = 100;
                                tabStyle.GridColumnStyles[CN.DateLastPaid].Width = 80;
                                tabStyle.GridColumnStyles[CN.ToPay].Width = 70;
                                tabStyle.GridColumnStyles[CN.PaidPcent].Width = 40;
                                tabStyle.GridColumnStyles[CN.Notes].Width = 140;
                                tabStyle.GridColumnStyles[CN.CustID].Width = 0;
                                tabStyle.GridColumnStyles[CN.DateProp.ToLower()].Width = 0;
                                //Headers
                                tabStyle.GridColumnStyles[CN.SalesPerson].HeaderText = GetResource("T_BOOKINGSSALESPERSON");
                                tabStyle.GridColumnStyles[CN.acctno].HeaderText = GetResource("T_ACCTNO");
                                tabStyle.GridColumnStyles[CN.AcctType].HeaderText = GetResource("T_ACCOUNTTYPE");
                                tabStyle.GridColumnStyles[CN.DateAcctOpen].HeaderText = GetResource("T_DATEOPENED");
                                tabStyle.GridColumnStyles[CN.AgrmtTotal].HeaderText = GetResource("T_AGREETOTAL");
                                tabStyle.GridColumnStyles[CN.DateLastPaid].HeaderText = GetResource("T_DATELASTPAID");
                                tabStyle.GridColumnStyles[CN.ToPay].HeaderText = GetResource("T_TOPAY");
                                tabStyle.GridColumnStyles[CN.PaidPcent].HeaderText = GetResource("T_PERCENTAGEPAID2");
                                tabStyle.GridColumnStyles[CN.Notes].HeaderText = GetResource("T_NOTES");
                                //Formatting and Alignment
                                tabStyle.GridColumnStyles[CN.AgrmtTotal].Alignment = HorizontalAlignment.Right;
                                ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.AgrmtTotal]).Format = DecimalPlaces;
                                tabStyle.GridColumnStyles[CN.ToPay].Alignment = HorizontalAlignment.Right;
                                ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.ToPay]).Format = DecimalPlaces;
                            }
                            int notesCol = dgAccounts.TableStyles[0].GridColumnStyles.IndexOf(dgAccounts.TableStyles[0].GridColumnStyles[CN.Notes]);
                            txtProposalNotes.Text = dgAccounts[selectRow, notesCol].ToString();
                        }
                    }
                }
                this._hasdatachanged = false;
                btnSave.Enabled = dataFound;
                //btnCancel.Enabled = (dataFound && this.allowCancel.Enabled);
                btnCancel.Enabled = (dataFound && allowCancelbool);                 // # 11097
                txtCancelNotes.Enabled = (txtCancelNotes.Enabled && dataFound);
                txtProposalNotes.Enabled = dataFound;
                //drpCancelReason.Enabled = (dataFound && this.allowCancel.Enabled);
                drpCancelReason.Enabled = (dataFound && allowCancelbool);           // # 11097
                if (dataFound && dgAccounts.VisibleRowCount > selectRow)
                {
                    dgAccounts.CurrentRowIndex = selectRow;
                }
                _rowsRetrievedText = statusText;
                ((MainForm)this.FormRoot).statusBar1.Text = statusText;
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

        public override bool ConfirmClose()
        {
            bool status = true;

            if (this._hasdatachanged == true)
            {
                if (DialogResult.Yes == ShowInfo("M_SAVECHANGES", MessageBoxButtons.YesNo))
                {
                    UpdateAccount(dgAccounts.CurrentRowIndex);
                }
            }

            return status;
        }

        private void drpBranchNo_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            //Check if there were outstanding changes for the previously selected row
            if (_hasdatachanged)
            {
                if (DialogResult.Yes == ShowInfo("M_SAVECHANGES2", MessageBoxButtons.YesNo))
                {
                    UpdateAccount(_lastGridIndex);
                }
                _hasdatachanged = false;
            }
            RefreshGrid();
        }

        /// <summary>
        /// This is called from MainForm to put back the statusBar1.Text for cases
        /// where it gets wiped out by a previous call to AddTabPage().. DO NOT REMOVE!
        /// </summary>
        public void SetStatusBar()
        {
            if (((MainForm)this.FormRoot).statusBar1.Text.Length == 0)
            {
                ((MainForm)this.FormRoot).statusBar1.Text = _rowsRetrievedText;
            }
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            int countAccts = 0;
            int custCol = dgAccounts.TableStyles[0].GridColumnStyles.IndexOf(dgAccounts.TableStyles[0].GridColumnStyles[CN.CustID]);
            int acctCol = dgAccounts.TableStyles[0].GridColumnStyles.IndexOf(dgAccounts.TableStyles[0].GridColumnStyles[CN.acctno]);

            for (int i = 0; i < ((DataView)dgAccounts.DataSource).Count; i++)
            {
                if (dgAccounts.IsSelected(i) || i == dgAccounts.CurrentRowIndex)
                {
                    try
                    {
                        Wait();
                        string acctNo = string.Empty;
                        string custID = string.Empty;
                        string code = string.Empty;
                        decimal balance = 0;
                        bool status = true;
                        bool outstPayments = false;
                        bool cancel = true;

                        ((MainForm)this.FormRoot).statusBar1.Text = "";
                        acctNo = dgAccounts[i, acctCol].ToString();
                        code = drpCancelReason.SelectedValue.ToString();
                        if (code == "")
                        {
                            status = false;
                            ShowInfo("M_INVALIDCANCELCODE");
                        }

                        if (status)
                        {
                            AccountManager.LockAccount(acctNo, Credential.UserId.ToString(), out error);
                            if (error.Length > 0)
                                ShowError(error);
                            else
                            {
                                AccountManager.CheckAccountToCancel(acctNo, Config.CountryCode, ref balance, ref outstPayments, out error);
                                if (error.Length > 0)
                                    ShowError(error);
                                else
                                {
                                    if (outstPayments)
                                    {
                                        OutstandingPayment op = new OutstandingPayment(FormRoot);
                                        op.ShowDialog();
                                        cancel = op.rbCancel.Checked;
                                    }
                                    if (cancel)
                                    {

                                        DataSet ds = AccountManager.GetScheduledForAccount(acctNo, true, out error);
                                        if (error.Length > 0)
                                            ShowError(error);
                                        else
                                        {
                                            foreach (DataRow row in ds.Tables[0].Rows)
                                            {
                                                if ((double)row[CN.Quantity] > 0)
                                                {
                                                    ScheduleOverride sched = new ScheduleOverride(false, acctNo, 1, 0, //(string)row[CN.ItemNo],         //IP - CR1212 - TODO: Need to pass in itemID
                                                        (short)row[CN.StockLocn],
                                                        (double)row[CN.Quantity], FormRoot, this, true);
                                                    sched.ShowDialog();
                                                }
                                            }

                                            AccountManager.CancelAccount(acctNo, custID, Convert.ToInt16(Config.BranchCode),
                                                code, balance, Config.CountryCode, txtCancelNotes.Text, 0, out error);
                                            if (error.Length > 0)
                                            {
                                                ShowError(error);
                                            }
                                            else//All good, unlock the account 
                                            {
                                                countAccts++;
                                                AccountManager.UnlockAccount(acctNo, Credential.UserId, out error);
                                                if (error.Length > 0)
                                                {
                                                    status = false;
                                                    ShowError(error);
                                                }
                                            }
                                        }
                                    }
                                }
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
                    }
                }
            }
            RefreshGrid();
            ((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_CANCELSUCCESSFUL2", new object[] { countAccts });
            drpCancelReason.SelectedIndex = 0;
            txtCancelNotes.Text = string.Empty;
        }

        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            //dynamicMenus[this.Name + ":currentBranch"] = this.currentBranch;
            //dynamicMenus[this.Name + ":allBranches"] = this.allBranches;
            //dynamicMenus[this.Name + ":allowCancel"] = this.allowCancel;
        }

        private void dgAccounts_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (dgAccounts.CurrentRowIndex >= 0)
            {
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    foreach (string s in _sc)
                        dgAccounts.Select(Convert.ToInt32(s));
                }
                dgAccounts.Select(dgAccounts.CurrentCell.RowNumber);
            }

            if (e.Button == MouseButtons.Right)
            {
                DataGrid ctl = (DataGrid)sender;

                MenuCommand m1 = new MenuCommand(GetResource("P_ACCOUNT_DETAILS"));
                m1.Click += new System.EventHandler(this.cmenuAccountDtls_Click);

                m1.Enabled = cmenuAccountDtls.Enabled;
                m1.Visible = cmenuAccountDtls.Visible;

                PopupMenu popup = new PopupMenu();
                popup.Animate = Animate.Yes;
                popup.AnimateStyle = Animation.SlideHorVerPositive;
                popup.MenuCommands.AddRange(new MenuCommand[] { m1 });
                MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
            }
        }

        private void cmenuAccountDtls_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                int index = dgAccounts.CurrentRowIndex;

                if (index >= 0)
                {
                    int acctCol = dgAccounts.TableStyles[0].GridColumnStyles.IndexOf(dgAccounts.TableStyles[0].GridColumnStyles[CN.acctno]);
                    string acctNo = dgAccounts[index, acctCol].ToString();
                    // Show the accounts details screen	
                    AccountDetails details = new AccountDetails(acctNo, FormRoot, this);
                    ((MainForm)this.FormRoot).AddTabPage(details, 7);
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

        private void dgAccounts_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (dgAccounts.CurrentRowIndex >= 0)
            {
                _sc.Clear();
                for (int i = 0; i < ((DataView)dgAccounts.DataSource).Count; i++)
                    if (dgAccounts.IsSelected(i))
                        _sc.Add(i.ToString());
            }
            if (e.Button == MouseButtons.Right)
            {
                dgAccounts.Select(dgAccounts.CurrentCell.RowNumber);
            }
        }
    }
}