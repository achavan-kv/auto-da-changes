using System;
using System.Collections;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.PrivilegeClub;
using STL.Common.Static;
using STL.PL.WS1;

namespace STL.PL
{
    /// <summary>
    /// Allows the user to add account codes to an account or customer codes
    /// to a customer.
    /// </summary>
    public class AddCustAcctCodes : CommonForm
    {
        private System.Drawing.Printing.PrintDocument prtDoc;
        //private Font printFont;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.ComponentModel.IContainer components;
        private DataTable validCustCodes;
        private DataTable validAcctCodes;
        private DataView validCodesView;
        private DataView recordedCodesView;
        private DataTable recordedAcctCodes;
        private DataTable recordedCustCodes;
        private DataSet allCodes;
        private new string Error = "";
        bool accounts = false;
        bool customer = false;
        //private Crownwood.Magic.Menus.MenuControl menuMain;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuSave;
        private Crownwood.Magic.Menus.MenuCommand menuClear;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.GroupBox grpCodes;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.DataGrid dgRecorded;
        private System.Windows.Forms.DataGrid dgValid;
        private System.Windows.Forms.GroupBox grpSearch;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.Button btnAddCust;
        private System.Windows.Forms.Button btnAddAcct;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox drpAccounts;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtCustID;
        private System.Windows.Forms.Label label1;
        public STL.PL.AccountTextBox txtAccountNumber;
        private bool _readOnly = false;
        public string customerID = "";
        private DataSet accountName = null;
        private string accountNo = "";
        private string ToGet = "";
        private bool noSuchAccount = false;
        private DataSet accountCodes = null;
        private bool noSuchCust = false;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox txtReference;
        private System.Windows.Forms.Label lBankrupt;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private DataSet customerCodes = null;
        private string custCode = ""; //IP - 01/09/09 - 5.2 UAT(823)

        public AddCustAcctCodes(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
        }

        public AddCustAcctCodes(bool readOnly, string custid, string firstName, string lastName, string accountNo)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            HashMenus();
            ApplyRoleRestrictions();

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
            //TranslateControls();
            toolTip1.SetToolTip(this.btnAdd, GetResource("TT_ADDCODE"));
            toolTip1.SetToolTip(this.btnRemove, GetResource("TT_REMOVECODE"));
            toolTip1.SetToolTip(this.btnSearch, GetResource("TT_SEARCH"));

            _readOnly = readOnly;
            if (readOnly)
            {
                btnSave.Enabled = false;
                btnAdd.Enabled = false;
                btnRemove.Enabled = false;
                menuSave.Enabled = false;
                //IP -16/10/2007 (UAT329)
                btnAddAcct.Text = "Account Codes";
                //IP -16/10/2007 (UAT329)
                btnAddCust.Text = "Customer Codes";
            }
            txtCustID.Text = custid;
            txtFirstName.Text = firstName;
            txtLastName.Text = lastName;
            txtAccountNumber.Text = accountNo;
        }

        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":lBankrupt"] = this.lBankrupt;
        }

        /// <summary>
        /// KEF - Set list of valid codes
        /// </summary>
        private void FilterCodes()
        {
            if (recordedCodesView.Count > 0 || !lBankrupt.Enabled)
            {

                string filter = "Code not in (";
                foreach (DataRowView row in recordedCodesView)
                {
                    filter += "'" + (string)row["Code"] + "',";
                }

                if (!lBankrupt.Enabled)
                    filter += "'BPT'";
                else
                    filter = filter.Substring(0, filter.Length - 1);

                filter += ")";
                validCodesView.RowFilter = filter;
            }
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddCustAcctCodes));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.grpCodes = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.dgRecorded = new System.Windows.Forms.DataGrid();
            this.dgValid = new System.Windows.Forms.DataGrid();
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.lBankrupt = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtReference = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.btnAddCust = new System.Windows.Forms.Button();
            this.btnAddAcct = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.drpAccounts = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCustID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAccountNumber = new STL.PL.AccountTextBox();
            this.prtDoc = new System.Drawing.Printing.PrintDocument();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
            this.menuClear = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.grpCodes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgRecorded)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgValid)).BeginInit();
            this.grpSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAdd.Location = new System.Drawing.Point(288, 104);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add>>";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnRemove.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRemove.Location = new System.Drawing.Point(288, 160);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "<< Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSearch.Location = new System.Drawing.Point(16, 40);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(20, 20);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // grpCodes
            // 
            this.grpCodes.BackColor = System.Drawing.SystemColors.Control;
            this.grpCodes.Controls.Add(this.btnSave);
            this.grpCodes.Controls.Add(this.btnAdd);
            this.grpCodes.Controls.Add(this.btnRemove);
            this.grpCodes.Controls.Add(this.dgRecorded);
            this.grpCodes.Controls.Add(this.dgValid);
            this.grpCodes.Enabled = false;
            this.grpCodes.Location = new System.Drawing.Point(8, 128);
            this.grpCodes.Name = "grpCodes";
            this.grpCodes.Size = new System.Drawing.Size(776, 344);
            this.grpCodes.TabIndex = 4;
            this.grpCodes.TabStop = false;
            this.grpCodes.Text = "Codes";
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave.Location = new System.Drawing.Point(288, 32);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 3;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dgRecorded
            // 
            this.dgRecorded.CaptionText = "Codes Recorded";
            this.dgRecorded.DataMember = "";
            this.dgRecorded.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgRecorded.Location = new System.Drawing.Point(384, 24);
            this.dgRecorded.Name = "dgRecorded";
            this.dgRecorded.ReadOnly = true;
            this.dgRecorded.Size = new System.Drawing.Size(376, 296);
            this.dgRecorded.TabIndex = 0;
            this.dgRecorded.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgRecorded_MouseUp);
            // 
            // dgValid
            // 
            this.dgValid.CaptionText = "Valid Codes";
            this.dgValid.DataMember = "";
            this.dgValid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgValid.Location = new System.Drawing.Point(16, 24);
            this.dgValid.Name = "dgValid";
            this.dgValid.ReadOnly = true;
            this.dgValid.Size = new System.Drawing.Size(256, 296);
            this.dgValid.TabIndex = 0;
            // 
            // grpSearch
            // 
            this.grpSearch.BackColor = System.Drawing.SystemColors.Control;
            this.grpSearch.Controls.Add(this.label5);
            this.grpSearch.Controls.Add(this.txtReference);
            this.grpSearch.Controls.Add(this.label4);
            this.grpSearch.Controls.Add(this.txtFirstName);
            this.grpSearch.Controls.Add(this.btnAddCust);
            this.grpSearch.Controls.Add(this.btnAddAcct);
            this.grpSearch.Controls.Add(this.label3);
            this.grpSearch.Controls.Add(this.txtLastName);
            this.grpSearch.Controls.Add(this.btnSearch);
            this.grpSearch.Controls.Add(this.drpAccounts);
            this.grpSearch.Controls.Add(this.label2);
            this.grpSearch.Controls.Add(this.txtCustID);
            this.grpSearch.Controls.Add(this.label1);
            this.grpSearch.Controls.Add(this.txtAccountNumber);
            this.grpSearch.Controls.Add(this.lBankrupt);
            this.grpSearch.Location = new System.Drawing.Point(8, 0);
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.Size = new System.Drawing.Size(776, 128);
            this.grpSearch.TabIndex = 3;
            this.grpSearch.TabStop = false;
            this.grpSearch.Text = "Search";
            this.grpSearch.Enter += new System.EventHandler(this.grpSearch_Enter);
            // 
            // lBankrupt
            // 
            this.lBankrupt.Enabled = false;
            this.lBankrupt.Location = new System.Drawing.Point(402, 96);
            this.lBankrupt.Name = "lBankrupt";
            this.lBankrupt.Size = new System.Drawing.Size(40, 16);
            this.lBankrupt.TabIndex = 12;
            this.lBankrupt.Text = "label6";
            this.lBankrupt.Visible = false;
            // 
            // label5
            // 
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(472, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 16);
            this.label5.TabIndex = 11;
            this.label5.Text = "Reference:";
            // 
            // txtReference
            // 
            this.txtReference.Enabled = false;
            this.txtReference.Location = new System.Drawing.Point(472, 96);
            this.txtReference.MaxLength = 10;
            this.txtReference.Name = "txtReference";
            this.txtReference.Size = new System.Drawing.Size(96, 20);
            this.txtReference.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(168, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "First Name:";
            // 
            // txtFirstName
            // 
            this.txtFirstName.Location = new System.Drawing.Point(168, 96);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(136, 20);
            this.txtFirstName.TabIndex = 6;
            // 
            // btnAddCust
            // 
            this.btnAddCust.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAddCust.Location = new System.Drawing.Point(680, 72);
            this.btnAddCust.Name = "btnAddCust";
            this.btnAddCust.Size = new System.Drawing.Size(75, 32);
            this.btnAddCust.TabIndex = 9;
            this.btnAddCust.Text = "Add Cust Codes";
            this.btnAddCust.Click += new System.EventHandler(this.btnAddCust_Click);
            // 
            // btnAddAcct
            // 
            this.btnAddAcct.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAddAcct.Location = new System.Drawing.Point(680, 24);
            this.btnAddAcct.Name = "btnAddAcct";
            this.btnAddAcct.Size = new System.Drawing.Size(75, 32);
            this.btnAddAcct.TabIndex = 8;
            this.btnAddAcct.Text = "Add Acct Codes";
            this.btnAddAcct.Click += new System.EventHandler(this.btnAddAcct_Click);
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(320, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "Last Name:";
            // 
            // txtLastName
            // 
            this.txtLastName.Location = new System.Drawing.Point(320, 96);
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(136, 20);
            this.txtLastName.TabIndex = 6;
            // 
            // drpAccounts
            // 
            this.drpAccounts.DropDownWidth = 121;
            this.drpAccounts.ItemHeight = 13;
            this.drpAccounts.Location = new System.Drawing.Point(168, 40);
            this.drpAccounts.Name = "drpAccounts";
            this.drpAccounts.Size = new System.Drawing.Size(121, 21);
            this.drpAccounts.TabIndex = 4;
            this.drpAccounts.Visible = false;
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(48, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Customer ID:";
            // 
            // txtCustID
            // 
            this.txtCustID.Location = new System.Drawing.Point(48, 96);
            this.txtCustID.Name = "txtCustID";
            this.txtCustID.Size = new System.Drawing.Size(88, 20);
            this.txtCustID.TabIndex = 2;
            this.txtCustID.Leave += new System.EventHandler(this.txtCustID_Leave_1);
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(48, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Account No:";
            // 
            // txtAccountNumber
            // 
            this.txtAccountNumber.Location = new System.Drawing.Point(48, 40);
            this.txtAccountNumber.Name = "txtAccountNumber";
            this.txtAccountNumber.PreventPaste = false;
            this.txtAccountNumber.Size = new System.Drawing.Size(88, 20);
            this.txtAccountNumber.TabIndex = 0;
            this.txtAccountNumber.Text = "000-0000-0000-0";
            this.txtAccountNumber.Leave += new System.EventHandler(this.txtAccountNumber_Leave_1);
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuSave,
            this.menuClear,
            this.menuExit});
            this.menuFile.Text = "&File";
            // 
            // menuSave
            // 
            this.menuSave.Description = "MenuItem";
            this.menuSave.Text = "&Save";
            this.menuSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // menuClear
            // 
            this.menuClear.Description = "MenuItem";
            this.menuClear.Text = "&Clear";
            this.menuClear.Click += new System.EventHandler(this.menuClear_Click);
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
            this.errorProvider1.DataMember = "";
            // 
            // AddCustAcctCodes
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.grpCodes);
            this.Controls.Add(this.grpSearch);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddCustAcctCodes";
            this.ShowInTaskbar = false;
            this.Text = "Add Account / Customer Codes";
            this.grpCodes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgRecorded)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgValid)).EndInit();
            this.grpSearch.ResumeLayout(false);
            this.grpSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }

        private void GetNameThread()
        {
            try
            {
                Function = "GetNameThread";
                Wait();
                accountName = AccountManager.GetAccountName(accountNo, customerID, out Error);
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
                Function = "End of GetNameThread";
            }
        }

        private void GetName()
        {
            Function = "GetName()";
            try
            {
                //Get the customer id associated with this account number if there is one
                Wait();

                customerID = txtCustID.Text;
                accountNo = txtAccountNumber.Text.Replace("-", "");

                Thread dataThread = new Thread(new ThreadStart(GetNameThread));
                dataThread.Start();
                dataThread.Join();

                if (accountName != null)
                {
                    foreach (DataTable dt in accountName.Tables)
                        foreach (DataRow r in dt.Rows)
                        {
                            txtAccountNumber.Text = (string)r["AccountNo"];
                            txtCustID.Text = (string)r["CustomerID"];
                            txtFirstName.Text = (string)r["firstname"];
                            txtLastName.Text = (string)r["name"];
                            ((MainForm)this.FormRoot).statusBar1.Text = "";
                            txtAccountNumber.Enabled = false;
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
                Function = "End of GetName";
            }

        }


        private void GetStatic(string toGet)
        {
            this.ToGet = toGet;
            Thread dataThread = new Thread(new ThreadStart(GetStaticThread));
            dataThread.Start();
            dataThread.Join();
        }

        private void GetStaticThread()
        {
            try
            {
                Wait();
                Function = "GetStaticThread";
                Function = "GetStatic";

                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                if (StaticData.Tables[ToGet] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, ToGet, null));

                if (dropDowns.DocumentElement.ChildNodes.Count > 0)
                {
                    allCodes = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        if (ToGet == TN.CustomerCodes)
                            StaticData.Tables[TN.CustomerCodes] = allCodes.Tables["CustomerCodes"];
                        else
                            StaticData.Tables[TN.AccountCodes] = allCodes.Tables["AccountCodes"];
                    }
                }
                if (ToGet == TN.CustomerCodes)
                    validCustCodes = (DataTable)StaticData.Tables[TN.CustomerCodes];
                else
                    validAcctCodes = (DataTable)StaticData.Tables[TN.AccountCodes];
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
                Function = "End of GetStaticThread";
            }
        }

        private void AccountCodesThread()
        {
            try
            {
                Wait();
                Function = "AccountCodesThread";
                accountCodes = AccountManager.GetCodesOnAccount(accountNo, out noSuchAccount, out Error);
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
                Function = "End of AccountCodesThread";
            }
        }

        public void AccountCodes(string acctNo)
        {
            Function = "AccountCodes";
            accounts = true;
            customer = false;
            grpCodes.Text = "Account Codes";
            noSuchAccount = false;

            Wait();
            GetStatic(TN.AccountCodes);
            validCodesView = new DataView((DataTable)StaticData.Tables[TN.AccountCodes]);

            accountNo = acctNo;

            Thread dataThread = new Thread(new ThreadStart(AccountCodesThread));
            dataThread.Start();
            dataThread.Join();

            if (accountCodes != null)
            {
                if (noSuchAccount)
                {
                    dgValid.DataSource = null;
                    grpCodes.Enabled = false;
                    ShowInfo("M_NOSUCHACCOUNT");
                }
                else
                {
                    dgRecorded.TableStyles.Clear();
                    recordedAcctCodes = accountCodes.Tables[0];
                    dgValid.DataSource = validCodesView;
                    recordedCodesView = new DataView(recordedAcctCodes);
                    dgRecorded.DataSource = recordedCodesView;
                    if (dgRecorded.TableStyles.Count == 0)
                    {
                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        tabStyle.MappingName = recordedAcctCodes.TableName;
                        dgRecorded.TableStyles.Add(tabStyle);

                        tabStyle.GridColumnStyles["Code"].Width = 50;
                        tabStyle.GridColumnStyles["Code"].HeaderText = GetResource("T_CODE");
                        tabStyle.GridColumnStyles["Date Added"].Width = 80;
                        tabStyle.GridColumnStyles["Date Added"].HeaderText = GetResource("T_DATEADDED");
                        tabStyle.GridColumnStyles["Added By"].Width = 55;
                        tabStyle.GridColumnStyles["Added By"].HeaderText = GetResource("T_ADDEDBY");
                        tabStyle.GridColumnStyles["Description"].Width = 150;
                        tabStyle.GridColumnStyles["Description"].HeaderText = GetResource("T_DESCRIPTION");
                    }
                    dgRecorded.DataSource = recordedCodesView;

                    FilterCodes();

                    //Remove codes from the valid view which appear in the 
                    //recorded view
                    /*foreach(DataRowView row in recordedCodesView)
                    {
                        validCodesView.RowFilter = "Code = '" + (string)row["Code"] + "' and Description = '" + (string)row["Description"] + "'";
                        foreach(DataRowView del in validCodesView)
                            del.Delete();
                        validCodesView.RowFilter = "";
                    }
                    */

                    grpCodes.Enabled = true;
                    dgValid.TableStyles.Clear();

                    if (dgValid.TableStyles.Count == 0)
                    {
                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        tabStyle.MappingName = TN.AccountCodes;
                        dgValid.TableStyles.Add(tabStyle);

                        tabStyle.GridColumnStyles["Code"].HeaderText = GetResource("T_CODE");
                        tabStyle.GridColumnStyles["Code"].Width = 50;
                        tabStyle.GridColumnStyles["Description"].HeaderText = GetResource("T_DESCRIPTION");
                        tabStyle.GridColumnStyles["Description"].Width = 150;
                    }
                    dgValid.DataSource = validCodesView;

                    /*JJ - can't imagine why this is being done but it isn't right*/
                    //Set up the recorded codes grid
                    //dgRecorded.TableStyles.Clear();

                    //if(recordedAcctCodes!=null)
                    //	recordedAcctCodes.Clear();

                }
            }
            StopWait();
            Function = "End of AccountCodes";

        }

        private void btnAddAcct_Click(object sender, System.EventArgs e)
        {
            Function = "btnAddAcct_Click";
            try
            {
                if (txtAccountNumber.Text.Length != 0 &&
                    txtAccountNumber.Text != "000-0000-0000-0")
                {
                    AccountCodes(txtAccountNumber.Text.Replace("-", ""));
                    txtReference.Text = "";
                    txtReference.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function = "End of btnAddAcct_Click";
            }
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            Function = "btnAdd_Click";
            DataRow row;
            bool status = true;

            try
            {
                int index = dgValid.CurrentRowIndex;
                if (index >= 0 && this.ValidCodeToAdd())
                {
                    if ((string)dgValid[index, 0] == "BPT" && txtReference.Text.Length == 0)
                    {
                        errorProvider1.SetError(txtReference, GetResource("M_ENTERMANDATORY"));
                        status = false;
                    }
                    else
                    {
                        errorProvider1.SetError(txtReference, "");
                        status = true;
                    }

                    if (status)
                    {
                        //remove the selected row from the valid datagrid and put it onto the 
                        //recorded datagrid
                        if (index >= 0)
                        {
                            if (accounts)
                            {
                                row = recordedAcctCodes.NewRow();
                            }
                            else
                            {
                                row = recordedCustCodes.NewRow();
                                custCode = Convert.ToString(dgValid[index, 0]); //IP - 01/09/09 - 5.2 UAT(823)
                            }


                            row["Code"] = dgValid[index, 0];
                            row["Date Added"] = DateTime.Now;
                            row["Added By"] = Credential.UserId;
                            row["Description"] = dgValid[index, 1];

                            if (accounts)
                                recordedAcctCodes.Rows.Add(row);
                            else
                            {
                                row["Reference"] = txtReference.Text;
                                recordedCustCodes.Rows.Add(row);
                            }

                            //	validCodesView.Delete(index);
                            FilterCodes();

                            btnSave_Click(this, null);
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
                Function = "End of btnAdd_click";
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            Function = "btnRemove_Click";
            //DataRow row;
            //int cell = 0;
            try
            {
                //remove the selected row from the recorded datagrid and add it to the
                //valid datagrid
                int index = dgRecorded.CurrentRowIndex;
                if (index >= 0 && this.ValidCodeToRemove())
                {
                    /*	if(accounts)
                        {
                            cell = 3;
                            row = ((DataTable)StaticData.Tables[TN.AccountCodes]).NewRow();
                        }
                        else
                        {
                            cell = 2;
                            row = ((DataTable)StaticData.Tables[TN.CustomerCodes]).NewRow();
                        }

                        row["Code"] = dgRecorded[index, 0];
                        row["Description"] = dgRecorded[index, cell];
					
                        if(accounts)
                            ((DataTable)StaticData.Tables[TN.AccountCodes]).Rows.Add(row);
                        else
                            ((DataTable)StaticData.Tables[TN.CustomerCodes]).Rows.Add(row);
    */
                    //IP - 01/09/09 - Save the code that is being deleted into a variable.
                    custCode = ((DataView)dgRecorded.DataSource)[index][CN.Code].ToString();

                    recordedCodesView.Delete(index);

                    FilterCodes();

                    btnSave_Click(this, null);
                    txtReference.Text = "";
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function = "End of btnRemove_Click";
            }
        }

        private void CustomerCodesThread()
        {
            try
            {
                Wait();
                Function = "CustomerCodesThread";
                customerCodes = CustomerManager.GetCodesForCustomer(customerID, out noSuchCust, out Error);
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
                Function = "End of CustomerCodesThread";
            }
        }

        public void CustomerCodes(string CustID)
        {
            Function = "CustomerCodes";

            accounts = false;
            customer = true;
            grpCodes.Text = "Customer Codes";
            Wait();
            GetStatic(TN.CustomerCodes);
            validCodesView = new DataView((DataTable)StaticData.Tables[TN.CustomerCodes]);

            noSuchCust = false;

            customerID = CustID;
            Thread dataThread = new Thread(new ThreadStart(CustomerCodesThread));
            dataThread.Start();
            dataThread.Join();

            if (customerCodes != null)
            {
                if (noSuchCust)
                {
                    dgValid.DataSource = null;
                    grpCodes.Enabled = false;
                    ShowInfo("M_NOSUCHCUSTOMER");
                }
                else
                {
                    dgRecorded.TableStyles.Clear();
                    recordedCustCodes = customerCodes.Tables[0];
                    dgValid.DataSource = validCodesView;
                    recordedCodesView = new DataView(recordedCustCodes);
                    dgRecorded.DataSource = recordedCodesView;
                    if (dgRecorded.TableStyles.Count == 0)
                    {
                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        tabStyle.MappingName = recordedCustCodes.TableName;
                        dgRecorded.TableStyles.Add(tabStyle);

                        tabStyle.GridColumnStyles["Code"].Width = 50;
                        tabStyle.GridColumnStyles["Code"].HeaderText = GetResource("T_CODE");
                        tabStyle.GridColumnStyles["Date Added"].Width = 80;
                        tabStyle.GridColumnStyles["Date Added"].HeaderText = GetResource("T_DATEADDED");
                        tabStyle.GridColumnStyles["Description"].Width = 150;
                        tabStyle.GridColumnStyles["Description"].HeaderText = GetResource("T_DESCRIPTION");
                        tabStyle.GridColumnStyles["Reference"].Width = 0;
                    }
                    dgRecorded.DataSource = recordedCodesView;


                    FilterCodes();

                    /*
                    //Remove codes from the valid view which appear in the 
                    //recorded view
                    foreach(DataRowView row in recordedCodesView)
                    {
                        validCodesView.RowFilter = "Code = '" + (string)row["Code"] + "'";
                        foreach(DataRowView del in validCodesView)
                            del.Delete();
                        validCodesView.RowFilter = "";
                    }
                    */

                    grpCodes.Enabled = true;
                    dgValid.TableStyles.Clear();

                    if (dgValid.TableStyles.Count == 0)
                    {
                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        tabStyle.MappingName = TN.CustomerCodes;
                        dgValid.TableStyles.Add(tabStyle);

                        tabStyle.GridColumnStyles["Code"].Width = 50;
                        tabStyle.GridColumnStyles["Code"].HeaderText = GetResource("T_CODE");
                        tabStyle.GridColumnStyles["Description"].Width = 150;
                        tabStyle.GridColumnStyles["Description"].HeaderText = GetResource("T_DESCRIPTION");
                    }
                    dgValid.DataSource = validCodesView;
                    txtReference.Enabled = true; //allow reference to be added for bankrupt

                    /*
                    //Set up the recorded codes grid
                    dgRecorded.TableStyles.Clear();

                    if(recordedCustCodes!=null)
                        recordedCustCodes.Clear();
                    */
                }
            }
            StopWait();
            Function = "End of CustomerCodes";
        }

        private void btnAddCust_Click(object sender, System.EventArgs e)
        {
            Function = "btnAddAcct_Click";
            try
            {
                if (txtCustID.Text.Length != 0)
                {
                    CustomerCodes(txtCustID.Text);
                    txtReference.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function = "End of btnAddAcct_Click";
            }
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            Function = "btnSave_Click";

            try
            {
                //Save the recorded codes to the database
                Wait();
                DataSet ds = new DataSet();

                if (accounts)
                {
                    recordedAcctCodes.AcceptChanges();
                    Error = AccountManager.AddCodesToAccount(txtAccountNumber.Text.Replace("-", ""), recordedAcctCodes.DataSet);
                    if (Error.Length > 0)
                        ShowError(Error);
                }
                if (customer)
                {
                    recordedCustCodes.AcceptChanges();

                    //Error = CustomerManager.AddCodesToCustomer(txtCustID.Text, recordedCustCodes.DataSet);
                    Error = CustomerManager.AddCodesToCustomer(txtCustID.Text, recordedCustCodes.DataSet, custCode); //IP - 01/09/09 - 5.2 UAT(823) - Added custCode
                    if (Error.Length > 0)
                        ShowError(Error);
                }
                StopWait();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Function = "End of btnSave_Click";
            }
        }

        private void menuClear_Click(object sender, System.EventArgs e)
        {
            txtAccountNumber.Text = "000-0000-0000-0";
            txtCustID.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            grpCodes.Enabled = false;
            validCodesView.Table.Clear();
            recordedCodesView.Table.Clear();
            grpCodes.Text = "Codes";
            txtAccountNumber.Enabled = true;
        }

        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            AccountSearch page = new AccountSearch();
            page.FormRoot = this.FormRoot;
            page.FormParent = this;

            page.AddCodes = true;
            page.InitData(txtCustID.Text, txtAccountNumber.Text);

            ((MainForm)this.FormRoot).AddTabPage(page, 9);
        }

        private void grpSearch_Enter(object sender, System.EventArgs e)
        {

        }

        private void txtAccountNumber_Leave_1(object sender, System.EventArgs e)
        {
            GetName();
        }

        private void txtCustID_Leave_1(object sender, System.EventArgs e)
        {
            GetName();
        }

        private void dgRecorded_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (customer)
                {
                    int index = dgRecorded.CurrentRowIndex;

                    if (index >= 0)
                    {
                        DataView dv = (DataView)dgRecorded.DataSource;

                        txtReference.Text = (string)dv[index]["Reference"];
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private bool ValidCodeToAdd()
        {
            // Do not allow Tier1/2 codes to be added
            bool status = false;
            int index = dgValid.CurrentRowIndex;
            if (index >= 0)
            {
                if (((DataView)dgValid.DataSource)[index][CN.Code].ToString() == PCCustCodes.Tier1
                    || ((DataView)dgValid.DataSource)[index][CN.Code].ToString() == PCCustCodes.Tier2)
                {
                    // Cannot add the Tier1/2 Privilege Club customer codes
                    errorProvider1.SetError(btnAdd, GetResource("M_PCCODEADDREMOVE", new object[] { PCCustCodes.Tier1, PCCustCodes.Tier2 }));
                }
                else
                {
                    errorProvider1.SetError(btnAdd, "");
                    errorProvider1.SetError(btnRemove, "");
                    status = true;
                }
            }
            return status;
        }

        private bool ValidCodeToRemove()
        {
            // Do not allow Tier1/2 codes to be removed
            bool status = false;
            int index = dgRecorded.CurrentRowIndex;
            if (index >= 0)
            {
                if (((DataView)dgRecorded.DataSource)[index][CN.Code].ToString() == PCCustCodes.Tier1
                    || ((DataView)dgRecorded.DataSource)[index][CN.Code].ToString() == PCCustCodes.Tier2)
                {
                    // Cannot remove the Tier1/2 Privilege Club customer codes
                    errorProvider1.SetError(btnRemove, GetResource("M_PCCODEADDREMOVE", new object[] { PCCustCodes.Tier1, PCCustCodes.Tier2 }));
                }
                else
                {
                    errorProvider1.SetError(btnAdd, "");
                    errorProvider1.SetError(btnRemove, "");
                    status = true;
                }
            }
            return status;
        }

    }
}
