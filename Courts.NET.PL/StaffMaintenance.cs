using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Values;
using STL.Common.Static;
using STL.PL.WS1;
using Blue.Cosacs.Shared;



namespace STL.PL
{
    /// <summary>
    /// Maintenance screen to record the details of each employee. An employee
    /// type can be selected and all employees of that type are listed. A
    /// current employee can be selected for update or a new employee added.
    /// Each employee is assigned a unique employee number and a password.
    /// Details include the branch where they are located and whether or not
    /// they can sell duty free.
    /// </summary>
    public class StaffMaintenance : CommonForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private new string Error = "";
        private bool staticLoaded = false;
        //private int selected; // = 0;
        string password = null;
        bool history = false;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private IContainer components;
        private GroupBox groupBox3;
        private Button btnNew;
        private Label lbl_outstanding;
        private DataGridView dgLogonHistory;
        private Button btnLogonHistory;
        private Button btnResetPassword;
        private Button btnTerminate;
        private ComboBox drpEmpType;
        private Label label2;
        private Button btnClear;
        private DataGridView dgEmployees;
        private Button btnSave;
        private GroupBox gb_info;
        private Label lblLastName;
        private TextBox txtLastName;
        private Label lblFirstName;
        private Label lblBranch;
        private TextBox txtEmpName;
        private TextBox txtFirstName;
        private Label label1;
        private ComboBox drpDutyFree;
        private Label label3;
        private Label label11;
        private Label label5;
        private TextBox tbUpliftCommRate;
        private Label label6;
        private Label label10;
        private Label label7;
        private CheckBox cbDefaultRate;
        private NumericUpDown txtRows;
        private TextBox txtFACTEmpeeNo;
        private TextBox txtEmpeeno;
        private Label lblFactEmpNo;
        private ComboBox drpBranch;
        private Label label4;
        private ComboBox drpEmpType1;
        private TextBox txtLoggedIn;
        private TextBox txtPassword;
        private Label label9;
        private TextBox txtConfirmPassword;
        private Label lblPassword;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        //private string empeeName = "";
        private bool ChangePassword;
        private DataTable st;       // #8870

        public StaffMaintenance(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });


        }

        public StaffMaintenance(Form root, Form parent)
        {
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            InitialiseStaticData();
            //dtDueDate.Value = DateTime.Today;

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });


            HashMenus();

            ApplyRoleRestrictions();

            //IP - Hide the logon history grid when displaying the screen.
            btnLogonHistory.Enabled = false;
            dgLogonHistory.Hide();
            //dgEmployees.Height = 320;         // #10112
            txtFACTEmpeeNo.Visible = (bool)Country[CountryParameterNames.showFactEmpNo];
            lblFactEmpNo.Visible = (bool)Country[CountryParameterNames.showFactEmpNo];
        }


        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":btnLogonHistory"] = this.btnLogonHistory;
        }


        private void InitialiseStaticData()
        {
            try
            {
                Function = "BStaticDataManager::GetDropDownData";
                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                StringCollection staffTypes = new StringCollection();
                staffTypes.Add("Staff Types");

                StringCollection empTypes = new StringCollection();

                empTypes.Add("Staff Types");

                StringCollection branchNos = new StringCollection();
                branchNos.Add("Branch");

                if (StaticData.Tables[TN.EmployeeTypes] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.EmployeeTypes,
                        new string[] { "ET1", "L" }));

                if (StaticData.Tables[TN.BranchNumber] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));

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

                foreach (DataRow row in CollectionsManager.RolesGet((int)CosacsPermissionEnum.Bailiff).Tables[0].Rows)
                {
                    string str = string.Format("{0} : {1}", row[0], row[1]);
                    empTypes.Add(str.ToUpper());

                    //staffTypes.Add(((string)row.ItemArray[0]).ToUpper()); 
                    //IP - 02/05/08 - UAT(275)
                    staffTypes.Add(str.ToUpper());
                }
                drpEmpType.DataSource = empTypes;
                drpEmpType1.DataSource = staffTypes;



                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                {
                    branchNos.Add(Convert.ToString(row[CN.BranchNo]));
                }

                drpBranch.DataSource = branchNos;

                staticLoaded = true;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StaffMaintenance));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.lbl_outstanding = new System.Windows.Forms.Label();
            this.dgLogonHistory = new System.Windows.Forms.DataGridView();
            this.btnLogonHistory = new System.Windows.Forms.Button();
            this.btnResetPassword = new System.Windows.Forms.Button();
            this.btnTerminate = new System.Windows.Forms.Button();
            this.drpEmpType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.dgEmployees = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.gb_info = new System.Windows.Forms.GroupBox();
            this.lblLastName = new System.Windows.Forms.Label();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.lblBranch = new System.Windows.Forms.Label();
            this.txtEmpName = new System.Windows.Forms.TextBox();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.drpDutyFree = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbUpliftCommRate = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cbDefaultRate = new System.Windows.Forms.CheckBox();
            this.txtRows = new System.Windows.Forms.NumericUpDown();
            this.txtFACTEmpeeNo = new System.Windows.Forms.TextBox();
            this.txtEmpeeno = new System.Windows.Forms.TextBox();
            this.lblFactEmpNo = new System.Windows.Forms.Label();
            this.drpBranch = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.drpEmpType1 = new System.Windows.Forms.ComboBox();
            this.txtLoggedIn = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgLogonHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgEmployees)).BeginInit();
            this.gb_info.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRows)).BeginInit();
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
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox3.Controls.Add(this.btnNew);
            this.groupBox3.Controls.Add(this.lbl_outstanding);
            this.groupBox3.Controls.Add(this.dgLogonHistory);
            this.groupBox3.Controls.Add(this.btnLogonHistory);
            this.groupBox3.Controls.Add(this.btnResetPassword);
            this.groupBox3.Controls.Add(this.btnTerminate);
            this.groupBox3.Controls.Add(this.drpEmpType);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.btnClear);
            this.groupBox3.Controls.Add(this.dgEmployees);
            this.groupBox3.Controls.Add(this.btnSave);
            this.groupBox3.Controls.Add(this.gb_info);
            this.groupBox3.Location = new System.Drawing.Point(8, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(776, 472);
            this.groupBox3.TabIndex = 61;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Staff Maintenance";
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(396, 12);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(136, 23);
            this.btnNew.TabIndex = 124;
            this.btnNew.TabStop = false;
            this.btnNew.Text = "New User";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // lbl_outstanding
            // 
            this.lbl_outstanding.AutoSize = true;
            this.lbl_outstanding.ForeColor = System.Drawing.Color.Red;
            this.lbl_outstanding.Location = new System.Drawing.Point(434, 75);
            this.lbl_outstanding.Name = "lbl_outstanding";
            this.lbl_outstanding.Size = new System.Drawing.Size(265, 13);
            this.lbl_outstanding.TabIndex = 119;
            this.lbl_outstanding.Text = "Cashier totals outstanding. Changes can not be saved.";
            this.lbl_outstanding.Visible = false;
            // 
            // dgLogonHistory
            // 
            this.dgLogonHistory.AllowUserToAddRows = false;
            this.dgLogonHistory.AllowUserToDeleteRows = false;
            this.dgLogonHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgLogonHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgLogonHistory.Location = new System.Drawing.Point(16, 323);
            this.dgLogonHistory.Name = "dgLogonHistory";
            this.dgLogonHistory.ReadOnly = true;
            this.dgLogonHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgLogonHistory.ShowEditingIcon = false;
            this.dgLogonHistory.ShowRowErrors = false;
            this.dgLogonHistory.Size = new System.Drawing.Size(368, 142);
            this.dgLogonHistory.TabIndex = 117;
            // 
            // btnLogonHistory
            // 
            this.btnLogonHistory.Location = new System.Drawing.Point(539, 41);
            this.btnLogonHistory.Name = "btnLogonHistory";
            this.btnLogonHistory.Size = new System.Drawing.Size(128, 23);
            this.btnLogonHistory.TabIndex = 18;
            this.btnLogonHistory.Text = "Show Logon History";
            this.btnLogonHistory.UseVisualStyleBackColor = true;
            this.btnLogonHistory.Visible = false;
            this.btnLogonHistory.Click += new System.EventHandler(this.btnLogonHistory_Click);
            // 
            // btnResetPassword
            // 
            this.btnResetPassword.Enabled = false;
            this.btnResetPassword.Location = new System.Drawing.Point(538, 12);
            this.btnResetPassword.Name = "btnResetPassword";
            this.btnResetPassword.Size = new System.Drawing.Size(129, 24);
            this.btnResetPassword.TabIndex = 17;
            this.btnResetPassword.TabStop = false;
            this.btnResetPassword.Text = "Reset Password";
            this.btnResetPassword.Click += new System.EventHandler(this.btnResetPassword_Click);
            // 
            // btnTerminate
            // 
            this.btnTerminate.Enabled = false;
            this.btnTerminate.Location = new System.Drawing.Point(396, 40);
            this.btnTerminate.Name = "btnTerminate";
            this.btnTerminate.Size = new System.Drawing.Size(136, 24);
            this.btnTerminate.TabIndex = 16;
            this.btnTerminate.TabStop = false;
            this.btnTerminate.Text = "Terminate Employment";
            this.btnTerminate.Click += new System.EventHandler(this.btnTerminate_Click);
            // 
            // drpEmpType
            // 
            this.drpEmpType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmpType.ItemHeight = 13;
            this.drpEmpType.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.drpEmpType.Location = new System.Drawing.Point(62, 27);
            this.drpEmpType.Name = "drpEmpType";
            this.drpEmpType.Size = new System.Drawing.Size(304, 21);
            this.drpEmpType.TabIndex = 20;
            this.drpEmpType.SelectedIndexChanged += new System.EventHandler(this.drpEmpType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(9, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 16);
            this.label2.TabIndex = 93;
            this.label2.Text = "Job Title";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(673, 41);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 19;
            this.btnClear.TabStop = false;
            this.btnClear.Text = "&Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // dgEmployees
            // 
            this.dgEmployees.AllowUserToAddRows = false;
            this.dgEmployees.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgEmployees.Location = new System.Drawing.Point(16, 68);
            this.dgEmployees.Name = "dgEmployees";
            this.dgEmployees.ReadOnly = true;
            this.dgEmployees.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgEmployees.Size = new System.Drawing.Size(368, 248);
            this.dgEmployees.TabIndex = 0;
            this.dgEmployees.TabStop = false;
            this.dgEmployees.Text = "Current Employees";
            this.dgEmployees.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgEmployees_CellClick);
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Location = new System.Drawing.Point(724, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 15;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // gb_info
            // 
            this.gb_info.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gb_info.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gb_info.Controls.Add(this.lblLastName);
            this.gb_info.Controls.Add(this.txtLastName);
            this.gb_info.Controls.Add(this.lblFirstName);
            this.gb_info.Controls.Add(this.lblBranch);
            this.gb_info.Controls.Add(this.txtEmpName);
            this.gb_info.Controls.Add(this.txtFirstName);
            this.gb_info.Controls.Add(this.label1);
            this.gb_info.Controls.Add(this.drpDutyFree);
            this.gb_info.Controls.Add(this.label3);
            this.gb_info.Controls.Add(this.label11);
            this.gb_info.Controls.Add(this.label5);
            this.gb_info.Controls.Add(this.tbUpliftCommRate);
            this.gb_info.Controls.Add(this.label6);
            this.gb_info.Controls.Add(this.label10);
            this.gb_info.Controls.Add(this.label7);
            this.gb_info.Controls.Add(this.cbDefaultRate);
            this.gb_info.Controls.Add(this.txtRows);
            this.gb_info.Controls.Add(this.txtFACTEmpeeNo);
            this.gb_info.Controls.Add(this.txtEmpeeno);
            this.gb_info.Controls.Add(this.lblFactEmpNo);
            this.gb_info.Controls.Add(this.drpBranch);
            this.gb_info.Controls.Add(this.label4);
            this.gb_info.Controls.Add(this.drpEmpType1);
            this.gb_info.Controls.Add(this.txtLoggedIn);
            this.gb_info.Controls.Add(this.txtPassword);
            this.gb_info.Controls.Add(this.label9);
            this.gb_info.Controls.Add(this.txtConfirmPassword);
            this.gb_info.Controls.Add(this.lblPassword);
            this.gb_info.Location = new System.Drawing.Point(393, 91);
            this.gb_info.Name = "gb_info";
            this.gb_info.Size = new System.Drawing.Size(377, 388);
            this.gb_info.TabIndex = 125;
            this.gb_info.TabStop = false;
            // 
            // lblLastName
            // 
            this.lblLastName.AutoSize = true;
            this.lblLastName.Location = new System.Drawing.Point(129, 121);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(58, 13);
            this.lblLastName.TabIndex = 153;
            this.lblLastName.Text = "Last Name";
            this.lblLastName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtLastName
            // 
            this.txtLastName.Location = new System.Drawing.Point(195, 118);
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(100, 20);
            this.txtLastName.TabIndex = 130;
            // 
            // lblFirstName
            // 
            this.lblFirstName.AutoSize = true;
            this.lblFirstName.Location = new System.Drawing.Point(130, 95);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(57, 13);
            this.lblFirstName.TabIndex = 152;
            this.lblFirstName.Text = "First Name";
            this.lblFirstName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblBranch
            // 
            this.lblBranch.Location = new System.Drawing.Point(83, 66);
            this.lblBranch.Name = "lblBranch";
            this.lblBranch.Size = new System.Drawing.Size(104, 16);
            this.lblBranch.TabIndex = 140;
            this.lblBranch.Text = "Branch Number";
            this.lblBranch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtEmpName
            // 
            this.txtEmpName.Enabled = false;
            this.txtEmpName.Location = new System.Drawing.Point(195, 144);
            this.txtEmpName.MaxLength = 20;
            this.txtEmpName.Name = "txtEmpName";
            this.txtEmpName.Size = new System.Drawing.Size(168, 20);
            this.txtEmpName.TabIndex = 131;
            // 
            // txtFirstName
            // 
            this.txtFirstName.Location = new System.Drawing.Point(195, 92);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(100, 20);
            this.txtFirstName.TabIndex = 129;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(83, 145);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 16);
            this.label1.TabIndex = 141;
            this.label1.Text = "Employee Name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpDutyFree
            // 
            this.drpDutyFree.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDutyFree.FormattingEnabled = true;
            this.drpDutyFree.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.drpDutyFree.Location = new System.Drawing.Point(195, 232);
            this.drpDutyFree.Name = "drpDutyFree";
            this.drpDutyFree.Size = new System.Drawing.Size(42, 21);
            this.drpDutyFree.TabIndex = 134;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(99, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 16);
            this.label3.TabIndex = 142;
            this.label3.Text = "Employee No";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(54, 362);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(126, 13);
            this.label11.TabIndex = 151;
            this.label11.Text = "Uplift Commission % Rate";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(131, 179);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 16);
            this.label5.TabIndex = 143;
            this.label5.Text = "Type";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbUpliftCommRate
            // 
            this.tbUpliftCommRate.Location = new System.Drawing.Point(195, 360);
            this.tbUpliftCommRate.MaxLength = 7;
            this.tbUpliftCommRate.Name = "tbUpliftCommRate";
            this.tbUpliftCommRate.Size = new System.Drawing.Size(40, 20);
            this.tbUpliftCommRate.TabIndex = 139;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(27, 205);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(160, 16);
            this.label6.TabIndex = 144;
            this.label6.Text = "Maximum Number of Rows";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(5, 333);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(230, 23);
            this.label10.TabIndex = 150;
            this.label10.Text = "Assign Default Bailiff Commission Rates ";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(75, 232);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 16);
            this.label7.TabIndex = 145;
            this.label7.Text = "Can Sell Duty Free";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbDefaultRate
            // 
            this.cbDefaultRate.Location = new System.Drawing.Point(251, 333);
            this.cbDefaultRate.Name = "cbDefaultRate";
            this.cbDefaultRate.Size = new System.Drawing.Size(16, 24);
            this.cbDefaultRate.TabIndex = 138;
            // 
            // txtRows
            // 
            this.txtRows.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txtRows.Location = new System.Drawing.Point(195, 205);
            this.txtRows.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.txtRows.Name = "txtRows";
            this.txtRows.Size = new System.Drawing.Size(72, 20);
            this.txtRows.TabIndex = 133;
            // 
            // txtFACTEmpeeNo
            // 
            this.txtFACTEmpeeNo.Location = new System.Drawing.Point(195, 11);
            this.txtFACTEmpeeNo.MaxLength = 4;
            this.txtFACTEmpeeNo.Name = "txtFACTEmpeeNo";
            this.txtFACTEmpeeNo.Size = new System.Drawing.Size(88, 20);
            this.txtFACTEmpeeNo.TabIndex = 126;
            // 
            // txtEmpeeno
            // 
            this.txtEmpeeno.Location = new System.Drawing.Point(195, 37);
            this.txtEmpeeno.MaxLength = 10;
            this.txtEmpeeno.Name = "txtEmpeeno";
            this.txtEmpeeno.Size = new System.Drawing.Size(88, 20);
            this.txtEmpeeno.TabIndex = 127;
            this.txtEmpeeno.Leave += new System.EventHandler(this.txtEmpeeno_Leave);
            // 
            // lblFactEmpNo
            // 
            this.lblFactEmpNo.Location = new System.Drawing.Point(67, 12);
            this.lblFactEmpNo.Name = "lblFactEmpNo";
            this.lblFactEmpNo.Size = new System.Drawing.Size(120, 16);
            this.lblFactEmpNo.TabIndex = 149;
            this.lblFactEmpNo.Text = "FACT Employee No";
            this.lblFactEmpNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpBranch
            // 
            this.drpBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranch.Location = new System.Drawing.Point(195, 61);
            this.drpBranch.Name = "drpBranch";
            this.drpBranch.Size = new System.Drawing.Size(80, 21);
            this.drpBranch.TabIndex = 128;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(107, 260);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 16);
            this.label4.TabIndex = 148;
            this.label4.Text = "Logged In";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpEmpType1
            // 
            this.drpEmpType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmpType1.Location = new System.Drawing.Point(195, 174);
            this.drpEmpType1.Name = "drpEmpType1";
            this.drpEmpType1.Size = new System.Drawing.Size(168, 21);
            this.drpEmpType1.TabIndex = 132;
            // 
            // txtLoggedIn
            // 
            this.txtLoggedIn.Location = new System.Drawing.Point(195, 259);
            this.txtLoggedIn.MaxLength = 1;
            this.txtLoggedIn.Name = "txtLoggedIn";
            this.txtLoggedIn.Size = new System.Drawing.Size(40, 20);
            this.txtLoggedIn.TabIndex = 135;
            // 
            // txtPassword
            // 
            this.txtPassword.Enabled = false;
            this.txtPassword.Location = new System.Drawing.Point(195, 286);
            this.txtPassword.MaxLength = 10;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(112, 20);
            this.txtPassword.TabIndex = 136;
            this.txtPassword.Leave += new System.EventHandler(this.txtPassword_Leave);
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(75, 310);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(112, 16);
            this.label9.TabIndex = 147;
            this.label9.Text = "Confirm Password";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.Enabled = false;
            this.txtConfirmPassword.Location = new System.Drawing.Point(195, 312);
            this.txtConfirmPassword.MaxLength = 10;
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.PasswordChar = '*';
            this.txtConfirmPassword.Size = new System.Drawing.Size(112, 20);
            this.txtConfirmPassword.TabIndex = 137;
            // 
            // lblPassword
            // 
            this.lblPassword.Location = new System.Drawing.Point(67, 285);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(120, 16);
            this.lblPassword.TabIndex = 146;
            this.lblPassword.Text = "Change Password";
            this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // StaffMaintenance
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox3);
            this.Name = "StaffMaintenance";
            this.Text = "Staff Maintenance";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgLogonHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgEmployees)).EndInit();
            this.gb_info.ResumeLayout(false);
            this.gb_info.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion


        private void InitError()
        {
            errorProvider1.SetError(this.txtEmpeeno, "");
            errorProvider1.SetError(this.drpBranch, "");
            errorProvider1.SetError(this.txtPassword, "");
            errorProvider1.SetError(this.txtConfirmPassword, "");
            errorProvider1.SetError(this.tbUpliftCommRate, "");
        }

        private void SetUpdate(bool canUpdate)
        {
            btnTerminate.Enabled = canUpdate;
            btnResetPassword.Enabled = canUpdate;
        }

        private void InitData()
        {
            this.InitError();
            ClearControls(this.Controls);
            ((MainForm)this.FormRoot).statusBar1.Text = "";

            //drpEmpType1.DataSource = null;
            //dgEmployees.DataSource = null;
            btnLogonHistory.Enabled = false;
            lbl_outstanding.Visible = false;
            btnLogonHistory.Text = "Show Logon Histroy"; //UAT(5.2) - 587 - NM
            history = false; //UAT(5.2) - 587 - NM
            password = null;
            ChangePassword = false;
            EnablePassword(false);
            this.SetUpdate(false);
            EnableControls(false);
            st = null;          // #8870
        }

        private void InitEmployee()
        {
            this.InitError();
            txtEmpeeno.Text = "";
            txtFACTEmpeeNo.Text = "";
            drpBranch.Text = "";
            txtEmpName.Text = "";

            if (drpEmpType1.DataSource != null)
                drpEmpType1.SelectedIndex = drpEmpType.SelectedIndex;

            //txtDutyFree.Text = "";
            drpDutyFree.Text = ""; //IP - 02/05/08 - UAT(275)
            txtLoggedIn.Text = "";
            txtPassword.Text = "";
            txtConfirmPassword.Text = "";
            txtRows.Value = 0;
            this.SetUpdate(false);
        }

        private void DisplayEmployee(int employeeNo)
        {
            decimal total;

            this.InitEmployee();
            txtEmpeeno.Text = employeeNo.ToString();
            DataSet empSet = new DataSet();
            empSet = Login.GetEmployeeDetails(employeeNo, Error);
            SetButtons(true);


            if (Error.Length > 0)
                ShowError(Error);
            else if (empSet != null)
            {
                foreach (DataTable empDetails in empSet.Tables)
                {
                    foreach (DataRow row in empDetails.Rows)
                    {
                        txtFACTEmpeeNo.Text = (string)row[CN.FACTEmployeeNo];
                        drpBranch.Text = row[CN.BranchNo].ToString();
                        txtEmpName.Text = (string)row[CN.EmployeeName];
                        txtFirstName.Text = (string)row[CN.FirstName];
                        txtLastName.Text = (string)row[CN.LastName];
                        //txtDutyFree.Text = (string)row[CN.DutyFree];
                        drpDutyFree.Text = (string)row[CN.DutyFree]; //IP - 02/05/08 - UAT(275)
                        password = (string)row[CN.Password];                // #8620 jec 10/11/11
                        //IP - 02/05/08 - UAT(275)
                        string role = row[CN.EmployeeType].ToString();
                        //IP - 20/05/08 - Credit Collections - Cater for (3) character employee type.
                        //role = role.Substring(0, 1);


                        //IP - 06/05/08 - UAT(275) - started the loop at position '1' as 
                        //want to ignore entry 'Staff Type' as when retrieving the role for a 
                        //'Salesperson', the first entry 'Staff Type' was incorrectly being selected.
                        for (int i = 1; i < drpEmpType1.Items.Count; i++)
                        {
                            //IP - 20/05/08 - Credit Collections - Cater for (3) character employee type.
                            int index = drpEmpType1.Items[i].ToString().IndexOf(":");

                            //if(drpEmpType1.Items[i].ToString().Substring(0,1) == role)
                            if (drpEmpType1.Items[i].ToString().Substring(0, index - 1) == role)
                            {
                                drpEmpType1.SelectedIndex = i;
                                break;

                            }
                        }

                        drpEmpType.SelectedIndex = drpEmpType1.SelectedIndex;
                        tbUpliftCommRate.Text = row[CN.UpliftCommissionRate].ToString();

                        if (Convert.ToBoolean(row[CN.LoggedIn]))
                            txtLoggedIn.Text = "Y";
                        else
                            txtLoggedIn.Text = "N";

                        this.SetUpdate(true);
                    }
                }

                PaymentManager.GetCashierTotals(0, Convert.ToInt32(employeeNo.ToString()), Date.blankDate, DateTime.Today, false, out total, out Error);

                if (total != 0) //IP - 01/10/10 - UAT(47) UAT5.4 - CR1117
                {
                    SetButtons(false);
                }
            }
        }

        private void SetButtons(bool enabled)
        {
            btnSave.Enabled = enabled;
            btnResetPassword.Enabled = enabled;
            btnTerminate.Enabled = enabled;
            lbl_outstanding.Visible = !enabled;
        }


        private void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                bool saveRecord = true;
                short loggedIn = 0;

                // Validate Courts Employee No is all numeric
                if (!IsStrictNumeric(txtEmpeeno.Text) || txtEmpeeno.Text.Trim().Length == 0)
                {
                    saveRecord = false;
                    errorProvider1.SetError(this.txtEmpeeno, GetResource("M_NUMERIC"));
                }
                else
                    errorProvider1.SetError(this.txtEmpeeno, "");


                // Validate Branch No is all numeric (sometimes has blank or 'Branch' in dropdown)
                if (!IsStrictNumeric(drpBranch.Text) || drpBranch.Text.Trim().Length == 0)
                {
                    saveRecord = false;
                    errorProvider1.SetError(this.drpBranch, GetResource("M_NUMERIC"));
                }
                else
                    errorProvider1.SetError(this.drpBranch, "");


                if (Config.CountryCode == "H")
                {
                    // Validate password is all numeric
                    if (!IsStrictNumeric(txtPassword.Text) && ((string)txtPassword.Text).Length > 0)
                    {
                        saveRecord = false;
                        errorProvider1.SetError(this.txtPassword, GetResource("M_PASSWORDCHECKNUMERIC"));
                    }
                    else
                        errorProvider1.SetError(this.txtPassword, "");
                }

                errorProvider1.SetError(txtPassword, "");
                errorProvider1.SetError(txtConfirmPassword, "");

                if (txtPassword.Enabled)
                {
                    var minLength = Country.GetCountryParameterValue<Int32>(CountryParameterNames.PasswordMinLength);
                    if (txtPassword.Text.Length < minLength)
                    {
                        saveRecord = false;
                        errorProvider1.SetError(txtPassword, GetResource("M_PASSWORDMINLENGTH", minLength));
                    }
                    else if (txtPassword.Text != txtConfirmPassword.Text)
                    {
                        saveRecord = false;
                        errorProvider1.SetError(this.txtConfirmPassword, GetResource("M_PASSWORDCHECKFAILED"));
                    }
                }
                // Validate Uplift Comm rate
                if (tbUpliftCommRate.Text.Trim() == "" || !IsStrictNumeric(tbUpliftCommRate.Text))
                {
                    errorProvider1.SetError(this.tbUpliftCommRate, GetResource("M_NUMERIC"));
                    saveRecord = false;
                }
                else
                {
                    //IP - 22/03/11 - #3299 - Commented out the below as now need to be able to save Uplift Commission with decimals.
                    //intCommRate = Convert.ToInt16(Convert.ToDecimal(tbUpliftCommRate.Text));
                    //decCommRate = Convert.ToDecimal(tbUpliftCommRate.Text);

                    //if (intCommRate != decCommRate)
                    //{
                    //    errorProvider1.SetError(this.tbUpliftCommRate, GetResource("M_DECIMALINVALID"));
                    //    saveRecord = false;
                    //}

                    //else
                    // Uplift rate must be between +200 and -200
                    if (Convert.ToInt16(Convert.ToDecimal(tbUpliftCommRate.Text)) > VL.Plus200 || Convert.ToInt16(Convert.ToDecimal(tbUpliftCommRate.Text)) < VL.Minus200)
                    {
                        errorProvider1.SetError(this.tbUpliftCommRate, GetResource("M_VALUEBETWEEN", new object[] { VL.Plus200, VL.Minus200 }));
                        saveRecord = false;
                    }
                    else
                        errorProvider1.SetError(this.tbUpliftCommRate, "");
                }


                if (saveRecord)
                {
                    // The old password is hidden from view and is only overwritten
                    // if the entered password is not blank
                    if (((string)txtPassword.Text).Length > 0)
                        password = txtPassword.Text;

                    if (txtLoggedIn.Text == "Y") loggedIn = 1;

                    //IP - 05/05/08 - UAT(275)
                    string empTypeSelected = Convert.ToString(drpEmpType1.SelectedItem);
                    //IP - 20/05/08 - Credit Collections, previously was only taking the
                    //first character of the 'empTypeSelected' string as the employee type. 
                    //Employee Type can now be upto (3) characters long therefore changed to cater for this.

                    int index = empTypeSelected.IndexOf(":");
                    string empType = empTypeSelected.Substring(0, index - 1);
                    //string empType = Convert.ToString(empTypeSelected[0]);

                    //Login.SaveStaff((short)Convert.ToInt32(drpBranch.SelectedItem),
                    //    Convert.ToInt32(txtEmpeeno.Text), txtFACTEmpeeNo.Text, txtEmpName.Text,
                    //    (string)drpEmpType1.SelectedItem, password,
                    //    txtDutyFree.Text, (int)txtRows.Value, loggedIn, cbDefaultRate.Checked, Convert.ToInt16(Convert.ToDecimal(tbUpliftCommRate.Text)), Error);

                    //IP - 02/05/08 - UAT(275)- Added 'drpDutyFree.Text
                    Login.SaveStaff(ChangePassword, (short)Convert.ToInt32(drpBranch.SelectedItem),
                        Convert.ToInt32(txtEmpeeno.Text), txtFACTEmpeeNo.Text, txtFirstName.Text, txtLastName.Text,
                        empType, password,
                        drpDutyFree.Text, (int)txtRows.Value, loggedIn, cbDefaultRate.Checked, Convert.ToSingle(Math.Round(Convert.ToSingle(tbUpliftCommRate.Text), 4)), Credential.UserId, Error);   //#9782

                    if (Error.Length > 0) ShowError(Error);

                    this.InitData();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSave_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                CloseTab();
            }
            catch (Exception ex)
            {
                Catch(ex, "menuExit_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                InitData();
            }
            catch (Exception ex)
            {
                Catch(ex, "btnClear_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnTerminate_Click(object sender, System.EventArgs e)
        {
            try
            {
                decimal total = 0;

                Wait();
                PaymentManager.GetCashierTotals(0, Convert.ToInt32(txtEmpeeno.Text), Date.blankDate, DateTime.Today, false, out total, out Error);
                if (total != 0)
                    ShowInfo("M_TERMINATED", new object[] { txtEmpeeno.Text });

                else
                {
                    Login.TerminateEmployee(Convert.ToInt32(txtEmpeeno.Text), Credential.UserId, out Error);



                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        DataView dv = new DataView((DataTable)dgEmployees.DataSource);
                        //DataView dv = ((DataView)dgEmployees.DataSource).Table;
                        int count = dv.Count;

                        foreach (DataGridViewRow dgEmpRow in dgEmployees.SelectedRows)
                        {
                            // Only interested in selected rows
                            //if (dgEmployees.SelectedRows)
                            //{

                            dv[dgEmpRow.Index][CN.EmployeeNo] = -1;
                            //}
                        }

                        for (int i = count - 1; i >= 0; i--)
                        {
                            if ((int)dv[i][CN.EmployeeNo] == -1)
                                dv.Delete(i);

                            InitData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnTerminate_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnResetPassword_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                EnablePassword(true);
                password = null;
                ChangePassword = true;
                this.txtPassword.Text = "";
                this.txtConfirmPassword.Text = "";
                this.btnSave_Click(sender, e);
            }
            catch (Exception ex)
            {
                Catch(ex, "btnResetPassword_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void EnablePassword(bool enabled)
        {
            txtPassword.Enabled = txtConfirmPassword.Enabled = enabled;
        }

        //private void btnSearch_Click(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        Wait();
        //        DataSet ds = null;
        //        dgEmployees.DataSource = ds.Tables[TN.Allocations].DefaultView;

        //        //if(dgEmployees.TableStyles.Count==0)
        //        //{
        //        //    DataGridTableStyle tabStyle = new DataGridTableStyle();
        //        //    tabStyle.MappingName = ds.Tables[TN.Allocations].TableName;
        //        //    dgEmployees.s.Add(tabStyle);

        //        dgEmployees.Columns["AcctType"].Width = 0;
        //        dgEmployees.Columns["TermsType"].Width = 0;
        //        dgEmployees.Columns["DateAcctOpen"].Width = 0;
        //        dgEmployees.Columns["AgrmtTotal"].Width = 0;
        //        dgEmployees.Columns["DateLastPaid"].Width = 0;
        //        dgEmployees.Columns["OutstBal"].Width = 0;
        //        dgEmployees.Columns["HighstStatus"].Width = 0;
        //        dgEmployees.Columns["CurrStatus"].Width = 0;
        //        dgEmployees.Columns["Custid"].Width = 0;

        //        dgEmployees.Columns["Acctno"].Width = 90;
        //        dgEmployees.Columns["Acctno"].HeaderText = GetResource("T_ACCTNO");

        //        dgEmployees.Columns["Arrears"].Width = 70;
        //        dgEmployees.Columns["Arrears"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        //        dgEmployees.Columns["Arrears"].HeaderText = GetResource("T_ARREARS");

        //        dgEmployees.Columns["Title"].Width = 40;
        //        dgEmployees.Columns["Title"].HeaderText = GetResource("T_TITLE");

        //        dgEmployees.Columns["Name"].Width = 90;
        //        dgEmployees.Columns["Name"].HeaderText = GetResource("T_LASTNAME");

        //        dgEmployees.Columns["Firstname"].Width = 90;
        //        dgEmployees.Columns["Firstname"].HeaderText = GetResource("T_FIRSTNAME");

        //        dgEmployees.Columns["HomeTel"].Width = 60;
        //        dgEmployees.Columns["HomeTel"].HeaderText = GetResource("T_PHONE");

        //        dgEmployees.Columns["WorkTel"].Width = 60;
        //        dgEmployees.Columns["WorkTel"].HeaderText = GetResource("T_WORKPHONE");

        //        dgEmployees.Columns["Ext"].Width = 40;
        //        dgEmployees.Columns["Ext"].HeaderText = GetResource("T_EXT");

        //        dgEmployees.Columns["NoCanAllocate"].Width = 0;

        //        int num = ds.Tables[0].Rows.Count;
        //        ((MainForm)this.FormRoot).statusBar1.Text = num.ToString() + " Accounts Loaded";

        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, "btnSearch_Click");
        //    }
        //    finally
        //    {
        //        StopWait();
        //    }
        //}

        private void dgEmployees_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            try
            {
                if (e.RowIndex != -1)
                {
                    EnableControls(true);
                    Wait();
                    // use correct index when view sorted (jec 68072)
                    btnLogonHistory.Enabled = true;

                    //IP - Clear status bar when a different employee is selected.
                    ((MainForm)this.FormRoot).statusBar1.Text = "";

                    //DataView empView = (DataView)dgEmployees.DataSource;

                    //empView.BeginInit();

                    BindingManagerBase bm = dgEmployees.BindingContext[dgEmployees.DataSource];
                    DataRow UserRow = ((DataRowView)bm.Current).Row;

                    //for (int i = empView.Count - 1; i >= 0; i--)
                    //{
                    //IP - If the same row has not been selected
                    //then hide the logon history grid.
                    //if (selected != -1 && dgEmployees.CurrentRow.Index != selected)
                    //{
                    if (UserRow["empeeno"] != null)
                    {
                        this.DisplayEmployee(Convert.ToInt32(UserRow["empeeno"]));
                        txtEmpeeno.Enabled = false;             // #8870
                        if (history)
                        {
                            LogonHistory();
                        }
                    }

                    //}
                    //if (dgEmployees.CurrentRow.Index == i)
                    //{
                    //this.DisplayEmployee(Convert.ToInt32(UserRow[");
                    //    //IP - store the index of the selected row.
                    //    selected = i;
                    //}
                    EnablePassword(false);
                }
                // code removed - incorrect index used when view sorted (jec 68072)
                /*	int index = dgEmployees.CurrentRowIndex;

                    if(index >= 0)
                    {
                        DataView empView = (DataView)dgEmployees.DataSource;
                        this.DisplayEmployee(Convert.ToInt32(empView.Table.Rows[index][CN.EmployeeNo]));
                    }*/
                //}

            }
            catch (Exception ex)
            {
                Catch(ex, "dgEmployees_MouseUp");
            }
            finally
            {
                //   StopWait();
            }
        }



        private void txtEmpeeno_Leave(object sender, System.EventArgs e)
        {
            var found = false;      // #8870
            var rowindex = 0;
            try
            {
                Wait();
                // Validate Courts Employee No is all numeric
                if (!IsStrictNumeric(txtEmpeeno.Text) || txtEmpeeno.Text.Trim().Length == 0)
                {
                    errorProvider1.SetError(this.txtEmpeeno, GetResource("M_NUMERIC"));
                    this.SetUpdate(false);
                }
                else
                {
                    errorProvider1.SetError(this.txtEmpeeno, "");
                    this.DisplayEmployee(Convert.ToInt32(txtEmpeeno.Text));

                    // Now find & set selected Row      // #8870
                    DataView currentView = new DataView(st);
                    if (st != null)  //no rows 
                    {
                        foreach (DataRowView rowView in currentView)
                        {
                            if (Convert.ToInt32(rowView[CN.EmployeeNo]) == Convert.ToInt32(txtEmpeeno.Text))
                            {
                                found = true;
                                break;
                            }
                            rowindex++;
                        }
                    }

                    if (found)
                    {
                        dgEmployees.ClearSelection();
                        dgEmployees.CurrentCell = dgEmployees.Rows[rowindex].Cells[0];
                        dgEmployees.Rows[rowindex].Selected = true;
                    }

                }
            }
            catch (Exception ex)
            {
                Catch(ex, "txtEmpeeno_Leave");
            }
            finally
            {
                StopWait();
            }
        }

        private void EnableControls(bool enable)
        {
            gb_info.Enabled = enable;
            txtEmpeeno.Enabled = enable;            // #8870
            btnLogonHistory.Enabled = enable;
            btnSave.Enabled = enable;
        }

        private void drpEmpType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                //IP - 02/05/08 - UAT(275)
                if (((System.Windows.Forms.ComboBox)(sender)).Focused)
                {
                    InitEmployee();
                }

                Wait();
                EnableControls(false);
                dgEmployees.DataSource = null;
                DataSet ds = null;
                ChangePassword = false;

                //selected = -1;

                //IP - Hide the logon history grid if it
                //is displayed when a different employee
                //type has been selected, and increase the 
                //height of the employee grid.
                //dgEmployees.Height = 320;         // #10112
                dgLogonHistory.Hide();

                if (staticLoaded && drpEmpType.SelectedIndex != 0)
                {

                    string empTypeStr = (string)drpEmpType.SelectedItem;
                    int index = empTypeStr.IndexOf(":");
                    string empType = empTypeStr.Substring(0, index - 1);

                    int len = empTypeStr.Length - 1;
                    string empTitle = empTypeStr.Substring(index + 1, len - index);

                    StringCollection staff = new StringCollection();

                    ds = Login.GetSalesStaffByType(empType, 0, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            if (dt.TableName == TN.SalesStaff)
                            {
                                //var st = dt.Copy();
                                st = dt.Copy();         // #8870
                                st.Columns.Remove("NoCanAllocate");

                                if (st.Rows.Count > 0)
                                {
                                    MainForm.Current.ShowStatus(st.Rows.Count.ToString() + " rows returned");

                                    dgEmployees.DataSource = st;
                                    //if(dgEmployees.TableStyles.Count==0)
                                    //{
                                    //    DataGridTableStyle tabStyle = new DataGridTableStyle();
                                    //    tabStyle.MappingName = dt.TableName;
                                    //    dgEmployees.TableStyles.Add(tabStyle);
                                    dgEmployees.ColumnHeadersHeight = 45;               // #10112
                                    dgEmployees.Columns[CN.EmployeeNo].Width = 55;
                                    dgEmployees.Columns[CN.EmployeeNo].HeaderText = GetResource("T_EMPEENO");
                                    dgEmployees.Columns[CN.EmployeeNo].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                                    dgEmployees.Columns[CN.FACTEmployeeNo].Width = 60;
                                    dgEmployees.Columns[CN.FACTEmployeeNo].HeaderText = GetResource("T_FACTEMPEENO");
                                    dgEmployees.Columns[CN.FACTEmployeeNo].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                                    dgEmployees.Columns[CN.FACTEmployeeNo].Visible = (bool)Country[CountryParameterNames.showFactEmpNo];
                                    dgEmployees.Columns[CN.EmployeeName].Width = 180;
                                    dgEmployees.Columns[CN.EmployeeName].HeaderText = GetResource("T_EMPEENAME");
                                    dgEmployees.Columns[CN.EmployeeName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


                                }
                            }
                        }
                        drpEmpType1.SelectedIndex = drpEmpType.SelectedIndex;
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "drpEmpType_SelectedIndexChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void txtPassword_Leave(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                if (Config.CountryCode == "H")
                {
                    // Validate password is all numeric
                    if (!IsStrictNumeric(txtPassword.Text) && ((string)txtPassword.Text).Length > 0)
                    {
                        errorProvider1.SetError(this.txtPassword, GetResource("M_PASSWORDCHECKNUMERIC"));
                    }
                    else
                        errorProvider1.SetError(this.txtPassword, "");
                }
                var minLength = Country.GetCountryParameterValue<Int32>(CountryParameterNames.PasswordMinLength);
                if (txtPassword.Text.Length < Country.GetCountryParameterValue<Int32>(CountryParameterNames.PasswordMinLength))
                {
                    errorProvider1.SetError(txtPassword, GetResource("M_PASSWORDMINLENGTH", minLength));
                }
                else
                {
                    errorProvider1.SetError(txtPassword, "");
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "txtPassword_Leave");
            }
            finally
            {
                StopWait();
            }
        }

        //private void txtConfirmPassword_Leave(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        Wait();
        //        if (txtPassword.Text != txtConfirmPassword.Text)
        //            errorProvider1.SetError(this.txtConfirmPassword, GetResource("M_PASSWORDCHECKFAILED"));
        //        else
        //            errorProvider1.SetError(this.txtConfirmPassword, "");
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, "txtConfirmPassword_Leave");
        //    }
        //    finally
        //    {
        //        StopWait();
        //    }
        //}

        //private void OnAccountDetails(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        Wait();
        //        int index = dgEmployees.CurrentRow.Index;

        //        if (index >= 0)
        //        {
        //            string acctNo = (string)dgEmployees[index, 0].Value.ToString();
        //            AccountDetails details = new AccountDetails(acctNo, FormRoot, this);
        //            ((MainForm)this.FormRoot).AddTabPage(details, 7);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, "OnAccountDetails");
        //    }
        //    finally
        //    {
        //        StopWait();
        //    }
        //}

        //private void OnFollowUpDetails(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        Wait();
        //        int index = dgEmployees.CurrentRow.Index;

        //        if (index >= 0)
        //        {
        //            string acctNo = (string)dgEmployees[index, 0].Value.ToString();
        //            BailActions actions = new BailActions(acctNo, FormRoot, this);
        //            actions.ShowDialog();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, "OnFollowUpDetails");
        //    }
        //    finally
        //    {
        //        StopWait();
        //    }
        //}

        //IP - 03/10/2007 - button click event loads the selected employees
        //logon history.
        private void btnLogonHistory_Click(object sender, EventArgs e)
        {
            if (history)
            {
                btnLogonHistory.Text = "Show Logon Histroy";
                history = false;
                //dgEmployees.Height = 320;         // #10112
                dgLogonHistory.Hide();
            }
            else
            {
                //------ UAT(5.2) - 586 ----------------------
                int temp = 0;
                if (Int32.TryParse(txtEmpeeno.Text, out temp) == false)
                {
                    MessageBox.Show("Please select an employee");
                    return;
                }
                //--------------------------------------------

                btnLogonHistory.Text = "Hide Logon Histroy";
                history = true;
                LogonHistory();
            }
        }

        //IP - 03/10/2007 - button click event loads the selected employees
        //logon history.
        private void LogonHistory()
        {
            try
            {
                int empeeno;

                //Converting the string Employee No to an int
                empeeno = Convert.ToInt32(txtEmpeeno.Text);

                DataSet ds = new DataSet();
                DataView logonHist = new DataView();

                //Retrieve the logon details for an employee and store
                //them in a dataset.
                ds = Login.GetEmployeeLogonHistory(empeeno, out Error);
                //Add the dataset to a dataview, specifying the table.
                logonHist = ds.Tables[TN.LogonHistory].DefaultView;

                //Display the logon history of the selected employee
                //if there are details to display.
                if (logonHist.Count > 0)
                {
                    //Set the source of the DataGridView to the dataview.
                    dgLogonHistory.DataSource = logonHist;

                    //Format the columns displayed.
                    dgLogonHistory.Columns[CN.EmployeeNo].Width = 55;
                    dgLogonHistory.Columns[CN.Action].Width = 40;
                    dgLogonHistory.Columns[CN.MachineLoggedOn].Width = 100;
                    dgLogonHistory.Columns[CN.DateAction].Width = 115;
                    dgLogonHistory.Columns[CN.EmployeeType].Width = 65;  //IP - 23/11/2007
                    //dgLogonHistory.ReadOnly = true;

                    //Adjust the height of the 'Current Employees' grid.
                    //dgEmployees.Height = 160;         // #10112
                    //Display the logon history grid.
                    dgLogonHistory.Show();
                }
                //If the selected employee does not have any logon history to display
                //then display a message on the status bar.
                else
                {
                    ((MainForm)this.FormRoot).statusBar1.Text = "The selected employee does not have any logon history to display";
                    dgLogonHistory.DataSource = null;
                }

            }
            catch (Exception ex)
            {

                Catch(ex, "btnLogonHistory_Click");
            }


        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Wait();
            InitData();
            EnableControls(true);
            txtPassword.Enabled = txtConfirmPassword.Enabled = true;
            ChangePassword = true;
        }



    }
}
