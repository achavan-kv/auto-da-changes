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
using STL.Common.Constants.CountryCodes;

namespace STL.PL
{
    /// <summary>
    /// Maintenance screen to control the automatic account number generation.
    /// Each branch uses a key number to generate each new account number. This
    /// key is incremented for each account number generated. Within each branch
    /// different account types generate a different type of account number.
    /// For a specified branch number all the account types are listed with a
    /// description and the current key value. The user can update these values
    /// as required.
    /// </summary>
    public class AccountNumberCtrl : CommonForm
    {
        private DataTable _branchData;
        //private DataTable _cancelReasons;
        private string error = "";
        private StringCollection _sc = new StringCollection();
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem cmenuAccountDtls;
        private System.Windows.Forms.GroupBox gbAccount;
        private System.Windows.Forms.TextBox txtHiAllocated;
        private System.Windows.Forms.DataGrid dgAcctNoCtrl;
        private System.Windows.Forms.Label lbBranchNo;
        private System.Windows.Forms.ComboBox drpBranchNo;
        private System.Windows.Forms.GroupBox gbResults;
        private System.ComponentModel.IContainer components;
        private string _rowsRetrievedText = string.Empty;
        private System.Windows.Forms.Label lbHiAllocated;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbHiAllowed;
        private System.Windows.Forms.TextBox txtHiAllowed;
        private System.Windows.Forms.ComboBox drpAcctCategories;
        private int _lastGridIndex = 0;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button btnExit;
        private bool _dataFound;


        public AccountNumberCtrl(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
        }

        public AccountNumberCtrl(Form root, Form parent)
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
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.gbAccount = new System.Windows.Forms.GroupBox();
            this.gbResults = new System.Windows.Forms.GroupBox();
            this.dgAcctNoCtrl = new System.Windows.Forms.DataGrid();
            this.drpBranchNo = new System.Windows.Forms.ComboBox();
            this.lbBranchNo = new System.Windows.Forms.Label();
            this.drpAcctCategories = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtHiAllocated = new System.Windows.Forms.TextBox();
            this.lbHiAllocated = new System.Windows.Forms.Label();
            this.txtHiAllowed = new System.Windows.Forms.TextBox();
            this.lbHiAllowed = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.cmenuAccountDtls = new System.Windows.Forms.MenuItem();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.gbAccount.SuspendLayout();
            this.gbResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAcctNoCtrl)).BeginInit();
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
            this.menuExit.Click += new System.EventHandler(this.fileExit_Click);
            // 
            // gbAccount
            // 
            this.gbAccount.BackColor = System.Drawing.SystemColors.Control;
            this.gbAccount.Controls.Add(this.gbResults);
            this.gbAccount.Controls.Add(this.drpBranchNo);
            this.gbAccount.Controls.Add(this.lbBranchNo);
            this.gbAccount.Controls.Add(this.drpAcctCategories);
            this.gbAccount.Controls.Add(this.label1);
            this.gbAccount.Controls.Add(this.btnSave);
            this.gbAccount.Controls.Add(this.txtHiAllocated);
            this.gbAccount.Controls.Add(this.lbHiAllocated);
            this.gbAccount.Controls.Add(this.txtHiAllowed);
            this.gbAccount.Controls.Add(this.lbHiAllowed);
            this.gbAccount.Controls.Add(this.btnExit);
            this.gbAccount.Location = new System.Drawing.Point(0, 0);
            this.gbAccount.Name = "gbAccount";
            this.gbAccount.Size = new System.Drawing.Size(794, 488);
            this.gbAccount.TabIndex = 0;
            this.gbAccount.TabStop = false;
            // 
            // gbResults
            // 
            this.gbResults.Controls.Add(this.dgAcctNoCtrl);
            this.gbResults.Location = new System.Drawing.Point(8, 152);
            this.gbResults.Name = "gbResults";
            this.gbResults.Size = new System.Drawing.Size(624, 320);
            this.gbResults.TabIndex = 44;
            this.gbResults.TabStop = false;
            // 
            // dgAcctNoCtrl
            // 
            this.dgAcctNoCtrl.DataMember = "";
            this.dgAcctNoCtrl.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAcctNoCtrl.Location = new System.Drawing.Point(0, 8);
            this.dgAcctNoCtrl.Name = "dgAcctNoCtrl";
            this.dgAcctNoCtrl.ReadOnly = true;
            this.dgAcctNoCtrl.Size = new System.Drawing.Size(624, 312);
            this.dgAcctNoCtrl.TabIndex = 42;
            this.dgAcctNoCtrl.CurrentCellChanged += new System.EventHandler(this.dgAcctNoCtrl_CurrentCellChanged);
            this.dgAcctNoCtrl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgAcctNoCtrl_MouseDown);
            this.dgAcctNoCtrl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgAcctNoCtrl_MouseUp);
            // 
            // drpBranchNo
            // 
            this.drpBranchNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranchNo.Location = new System.Drawing.Point(128, 24);
            this.drpBranchNo.Name = "drpBranchNo";
            this.drpBranchNo.Size = new System.Drawing.Size(240, 21);
            this.drpBranchNo.TabIndex = 43;
            this.drpBranchNo.SelectionChangeCommitted += new System.EventHandler(this.drpBranchNo_SelectionChangeCommitted);
            // 
            // lbBranchNo
            // 
            this.lbBranchNo.Location = new System.Drawing.Point(64, 24);
            this.lbBranchNo.Name = "lbBranchNo";
            this.lbBranchNo.Size = new System.Drawing.Size(56, 24);
            this.lbBranchNo.TabIndex = 2;
            this.lbBranchNo.Text = "Branch";
            this.lbBranchNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpAcctCategories
            // 
            this.drpAcctCategories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpAcctCategories.Items.AddRange(new object[] {
            "B - 3 months",
            "C - Cash Accounts",
            "D - 4 months",
            "E - 5 months",
            "F - 2 months",
            "G - 1 month",
            "H - Credit Accounts",
            "R - Ready",
            "S - Special Accounts",
            "T - Store Card"});
            this.drpAcctCategories.Location = new System.Drawing.Point(128, 56);
            this.drpAcctCategories.Name = "drpAcctCategories";
            this.drpAcctCategories.Size = new System.Drawing.Size(240, 21);
            this.drpAcctCategories.TabIndex = 43;
            this.drpAcctCategories.SelectionChangeCommitted += new System.EventHandler(this.drpAcctCategories_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(32, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "Acct Category";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(576, 24);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(48, 24);
            this.btnSave.TabIndex = 40;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtHiAllocated
            // 
            this.txtHiAllocated.Location = new System.Drawing.Point(128, 88);
            this.txtHiAllocated.MaxLength = 10;
            this.txtHiAllocated.Name = "txtHiAllocated";
            this.txtHiAllocated.Size = new System.Drawing.Size(144, 20);
            this.txtHiAllocated.TabIndex = 41;
            // 
            // lbHiAllocated
            // 
            this.lbHiAllocated.Location = new System.Drawing.Point(16, 88);
            this.lbHiAllocated.Name = "lbHiAllocated";
            this.lbHiAllocated.Size = new System.Drawing.Size(104, 24);
            this.lbHiAllocated.TabIndex = 2;
            this.lbHiAllocated.Text = "Highest Allocated";
            this.lbHiAllocated.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtHiAllowed
            // 
            this.txtHiAllowed.Location = new System.Drawing.Point(128, 120);
            this.txtHiAllowed.MaxLength = 10;
            this.txtHiAllowed.Name = "txtHiAllowed";
            this.txtHiAllowed.Size = new System.Drawing.Size(144, 20);
            this.txtHiAllowed.TabIndex = 41;
            // 
            // lbHiAllowed
            // 
            this.lbHiAllowed.Location = new System.Drawing.Point(16, 120);
            this.lbHiAllowed.Name = "lbHiAllowed";
            this.lbHiAllowed.Size = new System.Drawing.Size(104, 24);
            this.lbHiAllowed.TabIndex = 2;
            this.lbHiAllowed.Text = "Highest Allowed";
            this.lbHiAllowed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(576, 56);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(48, 24);
            this.btnExit.TabIndex = 40;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
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
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataMember = "";
            // 
            // AccountNumberCtrl
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 488);
            this.Controls.Add(this.gbAccount);
            this.Name = "AccountNumberCtrl";
            this.Text = "Account Number Control";
            this.Load += new System.EventHandler(this.AccountNumberCtrl_Load);
            this.gbAccount.ResumeLayout(false);
            this.gbAccount.PerformLayout();
            this.gbResults.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAcctNoCtrl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
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
            txtHiAllocated.Text = string.Empty;
            txtHiAllowed.Text = string.Empty;
            errorProvider1.SetError(txtHiAllocated, "");
            errorProvider1.SetError(txtHiAllowed, "");
            drpAcctCategories.SelectedIndex = 0;
            PopulateGrid(0);
            dgAcctNoCtrl.Focus();
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
				
            if(StaticData.Tables[TN.BranchNumber]==null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.BranchNumber, null));
			
            if(StaticData.Tables[TN.CancelReasons]== null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns,TN.CancelReasons, new string[]{"CN2", "L"}));

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
        }

        //
        // Events
        //

        //This method is called once, just before the screen is displayed.
        private void AccountNumberCtrl_Load(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Account Number Control screen: Form Load";
                Wait();
                this.LoadStaticData();
                //Keep track of changes so that a close confirmation can be displayed if unsaved
                //changes exist.
                AddKeyPressedEventHandlers(gbResults);
                AddKeyPressedEventHandler(txtHiAllocated);
                AddKeyPressedEventHandler(txtHiAllowed);
                RefreshGrid();
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
            if (txtHiAllocated.Text.Length == 0)
            {
                errorProvider1.SetError(txtHiAllocated, GetResource("M_MANDATORYFIELD",new object[]{lbHiAllocated.Text}));
            }
            else if (!IsNumeric(txtHiAllocated.Text))
            {
                errorProvider1.SetError(txtHiAllocated, GetResource("M_NONNUMERIC"));
            }
            else
            {
                errorProvider1.SetError(txtHiAllocated, "");
            }
            if (txtHiAllowed.Text.Length == 0)
            {
                errorProvider1.SetError(txtHiAllowed, GetResource("M_MANDATORYFIELD",new object[]{lbHiAllowed.Text}));
            }
            else if (!IsNumeric(txtHiAllowed.Text))
            {
                errorProvider1.SetError(txtHiAllowed, GetResource("M_NONNUMERIC"));
            }
            else
            {
                errorProvider1.SetError(txtHiAllowed, "");
            }

            if (errorProvider1.GetError(txtHiAllocated).Length == 0
                && errorProvider1.GetError(txtHiAllowed).Length == 0) 
            {
                if (Convert.ToInt32(txtHiAllocated.Text) > Convert.ToInt32(txtHiAllowed.Text))
                {
                    errorProvider1.SetError(txtHiAllocated, GetResource("M_HIALLOCATEDTOOLARGE"));
                }
            }

            if (errorProvider1.GetError(txtHiAllocated).Length == 0
                && errorProvider1.GetError(txtHiAllowed).Length == 0) 
            {
                try
                {
                    Function = "Account Number Control Screen: Save";
                    Wait();
            
                    int selectedRow = dgAcctNoCtrl.CurrentRowIndex;
                    UpdateAcctNoCtrl(false);
                    if (selectedRow < 0)//Means we just created a new row so that will now be row 0
                    {
                        selectedRow = 0;
                    }
                    PopulateGrid(selectedRow);
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

        private void fileExit_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Account Number Control Screen: File - Exit";
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


        /// <summary>
        /// Makes a call to the AccountManager web service to update AcctNoCtrl table
        /// using screen values.
        /// </summary>
        private void UpdateAcctNoCtrl(bool previousChange)
        {
            //If the grid is currently empty - this is going to do an insert, and we must
            //get BranchNo from the dropdown (which is why the Save button needs to be disabled
            //if the user has selected 'All' and no data was found - unlikely!).
            try 
            {
                Wait();
                int branchNo = 0;
                string acctCat = string.Empty;
                string acctCatDesc = string.Empty;
				bool valid = true; // 67850 RD 27/01/06
                //Can't just get Branch and AcctCat values from the dropdowns because we may
                //be saving an unsaved change that was made prior to the user changing the
                //dropdown value. bool parameter is used to check for this.
                if (_dataFound && previousChange)
                {
                    branchNo = Convert.ToInt16(GetBranchNoFromGrid(_lastGridIndex));
                    acctCat = GetAcctCatFromGrid(_lastGridIndex);
                    acctCatDesc = GetAcctCatDescFromGrid(_lastGridIndex);
                }
                else
                {
                    branchNo = Convert.ToInt16(drpBranchNo.SelectedValue);
                    acctCat = drpAcctCategories.SelectedItem.ToString().Substring(0,1);
                    acctCatDesc = drpAcctCategories.SelectedItem.ToString().Substring(4);
                }
                int hiAlloc = Convert.ToInt32(txtHiAllocated.Text);
                int hiAllowed = Convert.ToInt32(txtHiAllowed.Text);
				
				// 67850- RD 27/01/06 Added check to verify value for hialloc before saving.
				if (hiAlloc > hiAllowed)
				{
					errorProvider1.SetError(txtHiAllocated, GetResource("M_HIALLOCATEDTOOLARGE"));
					valid = false;
				}	
				
				if (valid)
				{
					AccountManager.SaveAcctNoCtrl(branchNo,acctCat,acctCatDesc,hiAlloc,hiAllowed,out error);
					if(error.Length > 0)
					{
						ShowError(error);
						StopWait();
						return;
					}
					else 
					{
						this._hasdatachanged=false;
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

        private void dgAcctNoCtrl_CurrentCellChanged(object sender, System.EventArgs e)
        {
            int index = dgAcctNoCtrl.CurrentRowIndex;

            //Check if there were outstanding changes for the previously selected row
            if (_lastGridIndex != index && _hasdatachanged)
            {
                if(DialogResult.Yes==ShowInfo("M_SAVECHANGES2", MessageBoxButtons.YesNo))
                {
                    UpdateAcctNoCtrl(true);
                    //Repopulate grid (so change just made is reflected) but reposition
                    //the selected row index to the one we were moving to prior to saving
                    //the previous change
                    PopulateGrid(index);
                }
                _hasdatachanged = false;
            }

            if(index>=0)
            {
                //Now we can finally change the data that appears in the Proposal Notes TextBox to correspond
                //to the new row we are navigating to. 
                PopulateEditFields(index);
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

                //Populate the grid with ALL Account data and select the row
                //corresponding to the Account entry currently being edited
                DataSet AccountNumberCtrl = AccountManager.GetAcctNoCtrl(branchNo,out error);

                if(error.Length > 0)
                {
                    ShowError(error);
                    StopWait();
                    return;
                }
                _dataFound = false;
                string statusText = GetResource("M_ENTRIESZERO");
                foreach (DataTable dt in AccountNumberCtrl.Tables)
                {
                    if (dt.TableName == TN.AcctNoCtrl)
                    {
                        _dataFound = (dt.Rows.Count > 0);
                        if (dt.Rows.Count == 1)
                        {
                            statusText = dt.Rows.Count + GetResource("M_ENTRYLISTED");
                        }
                        else 
                        {
                            statusText = dt.Rows.Count + GetResource("M_ENTRIESLISTED");
                        }

                        if (_dataFound) 
                        {
                            DataView accountsListView = new DataView(dt);
                            dgAcctNoCtrl.DataSource = accountsListView;
                            dgAcctNoCtrl.ReadOnly = true;
                            accountsListView.AllowNew = false;
					
                            if (dgAcctNoCtrl.TableStyles.Count == 0)
                            {
                                DataGridTableStyle tabStyle = new DataGridTableStyle();
                                tabStyle.MappingName = accountsListView.Table.TableName;

                                dgAcctNoCtrl.TableStyles.Clear();
                                dgAcctNoCtrl.TableStyles.Add(tabStyle);

                                // Displayed columns
          
                                //Column Widths
                                tabStyle.GridColumnStyles[CN.BranchNo].Width = 170;
                                tabStyle.GridColumnStyles[CN.AcctCat].Width = 90;
                                tabStyle.GridColumnStyles[CN.AcctCatDesc].Width = 110;
                                tabStyle.GridColumnStyles[CN.HiAllocated].Width = 100;
                                tabStyle.GridColumnStyles[CN.HiAllowed].Width = 95;
                                //Headers
                                tabStyle.GridColumnStyles[CN.BranchNo].HeaderText = GetResource("T_BRANCH");
                                tabStyle.GridColumnStyles[CN.AcctCat].HeaderText = GetResource("T_ACCTCAT");
                                tabStyle.GridColumnStyles[CN.AcctCatDesc].HeaderText = GetResource("T_ACCTCATDESC");
                                tabStyle.GridColumnStyles[CN.HiAllocated].HeaderText = GetResource("T_HIALLOCATED");
                                tabStyle.GridColumnStyles[CN.HiAllowed].HeaderText = GetResource("T_HIALLOWED");
                            }
                            PopulateEditFields(selectRow);
                            dgAcctNoCtrl.CurrentRowIndex = selectRow;
                            dgAcctNoCtrl.Select(selectRow);
                            _lastGridIndex = selectRow;
                            btnSave.Enabled = true;
                        }
                        //if no data and 'All' branches selected 
                        //   - disable save button because we can't insert if we don't have
                        //     a branchNo to insert for..
                        else
                        {
                            txtHiAllocated.Text = string.Empty;
                            txtHiAllowed.Text = string.Empty;
                            if (branchNo == 0)
                            {
                                btnSave.Enabled = false;
                            }
                            else
                            {
                                btnSave.Enabled = true;
                            }
                        }
                    }
                }
                if (_dataFound && dgAcctNoCtrl.VisibleRowCount > selectRow) 
                {
                    dgAcctNoCtrl.CurrentRowIndex = selectRow;
                }
                _rowsRetrievedText = statusText;
                ((MainForm)this.FormRoot).statusBar1.Text = statusText;
                this._hasdatachanged=false;
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

        private void PopulateEditFields(int fromRowIndex)
        {
            int hiAllocCol = dgAcctNoCtrl.TableStyles[0].GridColumnStyles.IndexOf(dgAcctNoCtrl.TableStyles[0].GridColumnStyles[CN.HiAllocated]);
            txtHiAllocated.Text = dgAcctNoCtrl[fromRowIndex,hiAllocCol].ToString();
            int hiAllowedCol = dgAcctNoCtrl.TableStyles[0].GridColumnStyles.IndexOf(dgAcctNoCtrl.TableStyles[0].GridColumnStyles[CN.HiAllowed]);
            txtHiAllowed.Text = dgAcctNoCtrl[fromRowIndex,hiAllowedCol].ToString();
            int acctCatCol = dgAcctNoCtrl.TableStyles[0].GridColumnStyles.IndexOf(dgAcctNoCtrl.TableStyles[0].GridColumnStyles[CN.AcctCat]);
            string acctCatValue = dgAcctNoCtrl[fromRowIndex,acctCatCol].ToString();
            foreach (object o in drpAcctCategories.Items)
            {
                if (acctCatValue == o.ToString().Substring(0,1))
                {
                    drpAcctCategories.SelectedItem = o;
                    break;
                }
            }
            //The above code shouldn't count as data being changed!
            _hasdatachanged = false;
        }

        public override bool ConfirmClose()
        {
            bool status = true;

            if (this._hasdatachanged==true)
            {
                if(DialogResult.Yes==ShowInfo("M_SAVECHANGES", MessageBoxButtons.YesNo))
                {
                    UpdateAcctNoCtrl(true);
                }
            }

            return status;
        }

        private void drpBranchNo_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            //Check if there were outstanding changes for the previously selected row
            if (_hasdatachanged)
            {
                if(DialogResult.Yes==ShowInfo("M_SAVECHANGES2", MessageBoxButtons.YesNo))
                {
                    UpdateAcctNoCtrl(true);
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

        private void dgAcctNoCtrl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(dgAcctNoCtrl.CurrentRowIndex>=0)
            {
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control) 
                {
                    foreach(string s in _sc)
                        dgAcctNoCtrl.Select(Convert.ToInt32(s));
                }
                dgAcctNoCtrl.Select(dgAcctNoCtrl.CurrentCell.RowNumber);		
            }
        }


        private void dgAcctNoCtrl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(dgAcctNoCtrl.CurrentRowIndex>=0)
            {
                _sc.Clear();
                for(int i=0; i< ((DataView)dgAcctNoCtrl.DataSource).Count; i++)
                    if(dgAcctNoCtrl.IsSelected(i))
                        _sc.Add(i.ToString());			
            }
            if (e.Button == MouseButtons.Right)
            {
                dgAcctNoCtrl.Select(dgAcctNoCtrl.CurrentCell.RowNumber);
            }
        }

        private void drpAcctCategories_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            //Check if there were outstanding changes for the previously selected row
            if (_hasdatachanged)
            {
                if(DialogResult.Yes==ShowInfo("M_SAVECHANGES2", MessageBoxButtons.YesNo))
                {
                    UpdateAcctNoCtrl(true);
                }
                _hasdatachanged = false;
            }

            bool selectionFound = false;
            if (_dataFound) 
            {
                int selectedBranchNo = Convert.ToInt16(drpBranchNo.SelectedValue);
                int rowToSelect = 0;
                //Deselect any selected rows so as not to confuse the user
                for (int i=0; i<dgAcctNoCtrl.VisibleRowCount; i++)
                {
                    dgAcctNoCtrl.UnSelect(i);
                }
                //Now work out which row should be selected based on the change to the
                //AcctCategories dropdown.
                for (int i=0; i<dgAcctNoCtrl.VisibleRowCount; i++)
                {
                    if (drpAcctCategories.SelectedItem.ToString().Substring(0,1)
                        == GetAcctCatFromGrid(i).Substring(0,1)
                        && ((selectedBranchNo > 0 && Convert.ToInt16(GetBranchNoFromGrid(i)) == selectedBranchNo)
                        || (selectedBranchNo == 0)))
                    {
                        dgAcctNoCtrl.Select(i);
                        dgAcctNoCtrl.CurrentRowIndex = i;
                        rowToSelect = i;
                        _lastGridIndex = i;
                        selectionFound = true;
                        break;
                    }
                }
                if (selectionFound)
                {
                    PopulateEditFields(rowToSelect);
                }
                else
                {
                    txtHiAllocated.Text = string.Empty;
                    txtHiAllowed.Text = string.Empty;
                }
            }
        }

        private string GetBranchNoFromGrid(int index)
        {
            int branchCol = dgAcctNoCtrl.TableStyles[0].GridColumnStyles.IndexOf(dgAcctNoCtrl.TableStyles[0].GridColumnStyles[CN.BranchNo]);
            string branch = dgAcctNoCtrl[index,branchCol].ToString();
            return branch.Substring(0,branch.IndexOf(":"));
        }

        private string GetAcctCatFromGrid(int index)
        {
            int acctCatCol = dgAcctNoCtrl.TableStyles[0].GridColumnStyles.IndexOf(dgAcctNoCtrl.TableStyles[0].GridColumnStyles[CN.AcctCat]);
            return dgAcctNoCtrl[index,acctCatCol].ToString();
        }

        private string GetAcctCatDescFromGrid(int index)
        {
            int acctCatDescCol = dgAcctNoCtrl.TableStyles[0].GridColumnStyles.IndexOf(dgAcctNoCtrl.TableStyles[0].GridColumnStyles[CN.AcctCatDesc]);
            return dgAcctNoCtrl[index,acctCatDescCol].ToString();
        }

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Account Number Control Screen: Close Tab";
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
    }
}