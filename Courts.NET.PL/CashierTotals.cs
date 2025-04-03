using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using AxSHDocVw;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.Branch;
using Crownwood.Magic.Menus;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using STL.PL.Utils;


namespace STL.PL
{
    /// <summary>
    /// Struct used to store the system value and the securitised value for 
    /// each payment method
    /// </summary>
    public struct PayMethodTotal
    {
        public decimal SystemValue;
        public decimal SecuritisedValue;
    }

    /// <summary>
    /// When an employee takes a payment the amount of the payment is added to
    /// their cash till to be totalled at the end of the day. This screen requests
    /// the user to enter the actual amounts totalled from the cash till. Any
    /// discrepancy between the amounts entered and the amounts totalled by the
    /// system as payments were taken will be recorded as overages and shortages.
    /// Cheques taken and cashier totals history can be reviewed here.
    /// </summary>
    public class CashierTotals : CommonForm
    {
        //private Crownwood.Magic.Menus.MenuControl menuMain;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private new string Error = "";
        private DataSet BranchTotals = null;
        private DataSet EmpTotals = null;
        private int _authorisedBy = 0;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.GroupBox groupBox1;
        private Crownwood.Magic.Controls.TabControl tcMain;
        private Crownwood.Magic.Controls.TabPage tpBranch;
        private System.Windows.Forms.DataGrid dgBranch;
        private Crownwood.Magic.Controls.TabPage tpEmployee;
        private Crownwood.Magic.Controls.TabPage tpTotals;
        private Crownwood.Magic.Controls.TabPage tpCheques;
        private System.Windows.Forms.DataGrid dgCheques;
        private Crownwood.Magic.Controls.TabPage tpHistory;
        private System.Windows.Forms.DataGrid dgHistory;
        private System.Windows.Forms.ImageList images;
        private bool loading = true;
        private Crownwood.Magic.Controls.TabPage tpSummary;
        private System.Windows.Forms.DataGrid dgSummary;
        private string diffString = "";
        private DataSet Summary = null;
        private Crownwood.Magic.Controls.TabPage tpReversal;
        private Crownwood.Magic.Controls.TabControl tcEmployee;
        private System.Windows.Forms.DataGrid dgReversal;
        private Crownwood.Magic.Menus.MenuCommand menuHelp;
        private Crownwood.Magic.Menus.MenuCommand menuLaunchhelp;
        private Control allowSummary = null;
        private ToolTip toolTip1;
        private bool Authenticated = false;
        private bool BreakDownDisplayed = false;
        private DataGridView dgViewTotals;                         //IP - 07/12/11 - CR1234
        private bool shortageOverageAllowed = true;                //IP - 07/12/11 - CR1234
        //private decimal shortDaily = 0;                            //IP - 19/12/11 - #8817 - CR1234                     
        //private decimal shortWeekly = 0;                           //IP - 19/12/11 - #8817 - CR1234
        //private decimal shortMonthly = 0;
        private Panel panel1;
        private Label lDeposit;
        private Button btnCalc;
        private Button btnExit;
        private Button btnClear;
        private Label lAuthorise;
        private Panel panel2;
        private Button btnExclude;
        private Button btnInclude;
        private ComboBox drpCashier;
        private Button btnSave;
        private Button btnEmployeePrint;
        private Button btnEmployeeLoad;
        private DateTimePicker dtEmployeeTo;
        private DateTimePicker dtEmployeeFrom;
        private Label lEmployeeTo;
        private Label lEmployeeFrom;
        private Label label5;
        private Button btnReverse;
        private Panel panel3;
        private Label lDepositTotal;
        private TextBox txtDepositTotal;
        private Label lDifferences;
        private TextBox txtTotalDifference;
        private Label lSystemTotal;
        private Label lUserTotal;
        private TextBox txtSystemTotal;
        private TextBox txtUserTotal;
        private Panel panel4;
        private Button btnBranchPrint;
        public Button btnExcel;
        private Button btnBranchLoad;
        private Label label4;
        private DateTimePicker dtBranchTo;
        private DateTimePicker dtBranchFrom;
        private Label lTo;
        private Label lFrom;
        private ComboBox drpBranch;
        private Panel panel5;
        private Label label3;
        private TextBox txtBranchTotal;
        private Panel panel6;
        private Button btnSummaryPrint;
        private DateTimePicker dtSummaryTo;
        private DateTimePicker dtSummaryFrom;
        private Label label2;
        private Label label7;
        private Label label1;
        private Button btnSummaryLoad;
        private ComboBox drpSummaryBranch;
        private Panel panel7;
        private Label label6;
        private TextBox txtChequesTotal;
        private Panel panel8;
        private Label label12;
        private TextBox txtRDepositTotal;
        private Label label16;
        private TextBox txtRDiffTotal;
        private Label label17;
        private Label label18;
        private TextBox txtRSystemTotal;
        private TextBox txtRUserTotal;                          //IP - 19/12/11 - #8817 - CR1234
        private decimal shortYearly = 0;                           //IP - 19/12/11 - #8817 - CR1234
        private List<ExchangeRate> exchangeRates;
        public bool tabClosed = false;  //#10400 - LW74981 - found error whilst fixing this issue.


        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":allowSummary"] = this.allowSummary;
            dynamicMenus[this.Name + ":lDeposit"] = this.lDeposit;
            dynamicMenus[this.Name + ":tpBranch"] = this.tpBranch;
        }

        public CashierTotals(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuHelp });

        }

        /// <summary>
        /// Does some initialisation. Note that we use the time retrieved from the
        /// server. Very important because timing is everything with cashier totals.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parent"></param>
        public CashierTotals(Form root, Form parent)
        {

            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuHelp });

            FormRoot = root;
            FormParent = parent;
            //this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            //            | System.Windows.Forms.AnchorStyles.Left)
            //            | System.Windows.Forms.AnchorStyles.Right)));
            //    this.AutoSize = true;
            HashMenus();
            ApplyRoleRestrictions();


            LoadStaticData();

            DateTime serverTime = StaticDataManager.GetServerDateTime();
            dtBranchFrom.Value = serverTime;
            dtBranchTo.Value = serverTime;

            dtEmployeeFrom.Value = serverTime;
            dtEmployeeTo.Value = serverTime;

            dtSummaryFrom.Value = serverTime;
            dtSummaryTo.Value = serverTime;

            txtUserTotal.Text = (0).ToString(DecimalPlaces);
            txtSystemTotal.Text = (0).ToString(DecimalPlaces);
            txtTotalDifference.Text = (0).ToString(DecimalPlaces);

            txtUserTotal.BackColor = SystemColors.Window;
            txtSystemTotal.BackColor = SystemColors.Window;
            txtTotalDifference.BackColor = SystemColors.Window;
            txtDepositTotal.BackColor = SystemColors.Window;

            if (!allowSummary.Enabled)
                tcMain.TabPages.Remove(tpSummary);

        }

        private void LoadStaticData()
        {
            try
            {
                Wait();

                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                if (StaticData.Tables[TN.BranchNumber] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));
                if (StaticData.Tables[TN.PayMethod] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.PayMethod, new string[] { "FPM", "L" }));

                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.SalesStaff, new string[] { Config.BranchCode, "C" }));

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

                drpBranch.DataSource = (DataTable)StaticData.Tables[TN.BranchNumber];
                drpBranch.DisplayMember = CN.BranchNo;

                /* the branch numbers table only has one column and it's 
                 * datatype is int16 so we'll have to copy it into a new
                 * structure in order to have a "Company wide" option. Annoying */
                DataTable tab = new DataTable();
                tab.Columns.AddRange(new DataColumn[] { new DataColumn(CN.BranchNo, Type.GetType("System.Int16")), new DataColumn(CN.Description) });
                DataTable temp = ((DataTable)StaticData.Tables[TN.BranchNumber]).Copy();

                DataRow n = tab.NewRow();
                n[CN.BranchNo] = -1;
                n[CN.Description] = "All Branches"; /* Tsk */
                tab.Rows.Add(n);

                foreach (DataRow r in temp.Rows)
                {
                    n = tab.NewRow();
                    n[CN.BranchNo] = r[CN.BranchNo];
                    n[CN.Description] = Convert.ToString(r[CN.BranchNo]);
                    tab.Rows.Add(n);
                }

                drpSummaryBranch.DataSource = tab.DefaultView;
                drpSummaryBranch.DisplayMember = CN.Description;

                drpSummaryBranch.SelectedIndex = drpSummaryBranch.FindStringExact(Config.BranchCode);

                drpCashier.DataSource = ((DataTable)StaticData.Tables[TN.SalesStaff]).DefaultView;
                drpCashier.DisplayMember = CN.EmployeeName;
            }
            catch (Exception ex)
            {
                Catch(ex, "LoadStaticData");
            }
            finally
            {
                StopWait();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CashierTotals));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.allowSummary = new System.Windows.Forms.Control();
            this.images = new System.Windows.Forms.ImageList(this.components);
            this.menuHelp = new Crownwood.Magic.Menus.MenuCommand();
            this.menuLaunchhelp = new Crownwood.Magic.Menus.MenuCommand();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnSave = new System.Windows.Forms.Button();
            this.lDepositTotal = new System.Windows.Forms.Label();
            this.lDifferences = new System.Windows.Forms.Label();
            this.lSystemTotal = new System.Windows.Forms.Label();
            this.lUserTotal = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tcMain = new Crownwood.Magic.Controls.TabControl();
            this.tpEmployee = new Crownwood.Magic.Controls.TabPage();
            this.tcEmployee = new Crownwood.Magic.Controls.TabControl();
            this.tpTotals = new Crownwood.Magic.Controls.TabPage();
            this.dgViewTotals = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.txtDepositTotal = new System.Windows.Forms.TextBox();
            this.txtTotalDifference = new System.Windows.Forms.TextBox();
            this.txtSystemTotal = new System.Windows.Forms.TextBox();
            this.txtUserTotal = new System.Windows.Forms.TextBox();
            this.tpCheques = new Crownwood.Magic.Controls.TabPage();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.txtChequesTotal = new System.Windows.Forms.TextBox();
            this.dgCheques = new System.Windows.Forms.DataGrid();
            this.tpHistory = new Crownwood.Magic.Controls.TabPage();
            this.dgHistory = new System.Windows.Forms.DataGrid();
            this.tpReversal = new Crownwood.Magic.Controls.TabPage();
            this.dgReversal = new System.Windows.Forms.DataGrid();
            this.panel8 = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.txtRDepositTotal = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtRDiffTotal = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.txtRSystemTotal = new System.Windows.Forms.TextBox();
            this.txtRUserTotal = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnExclude = new System.Windows.Forms.Button();
            this.btnInclude = new System.Windows.Forms.Button();
            this.drpCashier = new System.Windows.Forms.ComboBox();
            this.btnEmployeePrint = new System.Windows.Forms.Button();
            this.btnEmployeeLoad = new System.Windows.Forms.Button();
            this.dtEmployeeTo = new System.Windows.Forms.DateTimePicker();
            this.dtEmployeeFrom = new System.Windows.Forms.DateTimePicker();
            this.lEmployeeTo = new System.Windows.Forms.Label();
            this.lEmployeeFrom = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnReverse = new System.Windows.Forms.Button();
            this.tpBranch = new Crownwood.Magic.Controls.TabPage();
            this.dgBranch = new System.Windows.Forms.DataGrid();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnBranchPrint = new System.Windows.Forms.Button();
            this.btnExcel = new System.Windows.Forms.Button();
            this.btnBranchLoad = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.dtBranchTo = new System.Windows.Forms.DateTimePicker();
            this.dtBranchFrom = new System.Windows.Forms.DateTimePicker();
            this.lTo = new System.Windows.Forms.Label();
            this.lFrom = new System.Windows.Forms.Label();
            this.drpBranch = new System.Windows.Forms.ComboBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBranchTotal = new System.Windows.Forms.TextBox();
            this.tpSummary = new Crownwood.Magic.Controls.TabPage();
            this.dgSummary = new System.Windows.Forms.DataGrid();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btnSummaryPrint = new System.Windows.Forms.Button();
            this.dtSummaryTo = new System.Windows.Forms.DateTimePicker();
            this.dtSummaryFrom = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSummaryLoad = new System.Windows.Forms.Button();
            this.drpSummaryBranch = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lDeposit = new System.Windows.Forms.Label();
            this.btnCalc = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lAuthorise = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tpEmployee.SuspendLayout();
            this.tcEmployee.SuspendLayout();
            this.tpTotals.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgViewTotals)).BeginInit();
            this.panel3.SuspendLayout();
            this.tpCheques.SuspendLayout();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgCheques)).BeginInit();
            this.tpHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgHistory)).BeginInit();
            this.tpReversal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgReversal)).BeginInit();
            this.panel8.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tpBranch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgBranch)).BeginInit();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.tpSummary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgSummary)).BeginInit();
            this.panel6.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuExit});
            this.menuFile.Text = "&File";
            this.menuFile.Click += new System.EventHandler(this.menuFile_Click);
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // allowSummary
            // 
            this.allowSummary.Enabled = false;
            this.allowSummary.Location = new System.Drawing.Point(0, 0);
            this.allowSummary.Name = "allowSummary";
            this.allowSummary.Size = new System.Drawing.Size(0, 0);
            this.allowSummary.TabIndex = 0;
            this.allowSummary.Visible = false;
            // 
            // images
            // 
            this.images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("images.ImageStream")));
            this.images.TransparentColor = System.Drawing.Color.Transparent;
            this.images.Images.SetKeyName(0, "");
            // 
            // menuHelp
            // 
            this.menuHelp.Description = "MenuItem";
            this.menuHelp.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuLaunchhelp});
            this.menuHelp.Text = "&Help";
            // 
            // menuLaunchhelp
            // 
            this.menuLaunchhelp.Description = "MenuItem";
            this.menuLaunchhelp.Text = "&About This Screen";
            this.menuLaunchhelp.Click += new System.EventHandler(this.menuLaunchhelp_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.Enabled = false;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave.Location = new System.Drawing.Point(532, 29);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 60;
            this.toolTip1.SetToolTip(this.btnSave, "Save (after entering some totals)");
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lDepositTotal
            // 
            this.lDepositTotal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lDepositTotal.Location = new System.Drawing.Point(552, 11);
            this.lDepositTotal.Name = "lDepositTotal";
            this.lDepositTotal.Size = new System.Drawing.Size(100, 16);
            this.lDepositTotal.TabIndex = 18;
            this.lDepositTotal.Text = "Deposit Total";
            this.lDepositTotal.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.toolTip1.SetToolTip(this.lDepositTotal, "Excludes Store Card Transactions");
            this.lDepositTotal.Visible = false;
            // 
            // lDifferences
            // 
            this.lDifferences.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lDifferences.Location = new System.Drawing.Point(348, 11);
            this.lDifferences.Name = "lDifferences";
            this.lDifferences.Size = new System.Drawing.Size(100, 16);
            this.lDifferences.TabIndex = 16;
            this.lDifferences.Text = "Total Difference*";
            this.lDifferences.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.toolTip1.SetToolTip(this.lDifferences, "Excludes Store Card Transactions");
            this.lDifferences.Visible = false;
            // 
            // lSystemTotal
            // 
            this.lSystemTotal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lSystemTotal.Location = new System.Drawing.Point(452, 11);
            this.lSystemTotal.Name = "lSystemTotal";
            this.lSystemTotal.Size = new System.Drawing.Size(100, 16);
            this.lSystemTotal.TabIndex = 14;
            this.lSystemTotal.Text = "System Total";
            this.lSystemTotal.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.toolTip1.SetToolTip(this.lSystemTotal, "Excludes Store Card Transactions");
            this.lSystemTotal.Visible = false;
            // 
            // lUserTotal
            // 
            this.lUserTotal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lUserTotal.Location = new System.Drawing.Point(676, 11);
            this.lUserTotal.Name = "lUserTotal";
            this.lUserTotal.Size = new System.Drawing.Size(80, 16);
            this.lUserTotal.TabIndex = 13;
            this.lUserTotal.Text = "User Total";
            this.lUserTotal.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.toolTip1.SetToolTip(this.lUserTotal, "Excludes Store Card Transactions");
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.AutoSize = true;
            this.groupBox1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.groupBox1.Controls.Add(this.tcMain);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 475);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // tcMain
            // 
            this.tcMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.IDEPixelArea = true;
            this.tcMain.Location = new System.Drawing.Point(3, 50);
            this.tcMain.Name = "tcMain";
            this.tcMain.PositionTop = true;
            this.tcMain.SelectedIndex = 0;
            this.tcMain.SelectedTab = this.tpEmployee;
            this.tcMain.Size = new System.Drawing.Size(770, 422);
            this.tcMain.TabIndex = 2;
            this.tcMain.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpEmployee,
            this.tpBranch,
            this.tpSummary});
            this.tcMain.Click += new System.EventHandler(this.tcMain_Click);
            // 
            // tpEmployee
            // 
            this.tpEmployee.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tpEmployee.Controls.Add(this.tcEmployee);
            this.tpEmployee.Controls.Add(this.panel2);
            this.tpEmployee.Location = new System.Drawing.Point(0, 25);
            this.tpEmployee.Name = "tpEmployee";
            this.tpEmployee.Size = new System.Drawing.Size(770, 397);
            this.tpEmployee.TabIndex = 4;
            this.tpEmployee.Title = "Employee";
            // 
            // tcEmployee
            // 
            this.tcEmployee.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tcEmployee.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcEmployee.IDEPixelArea = true;
            this.tcEmployee.Location = new System.Drawing.Point(0, 100);
            this.tcEmployee.Name = "tcEmployee";
            this.tcEmployee.PositionTop = true;
            this.tcEmployee.SelectedIndex = 0;
            this.tcEmployee.SelectedTab = this.tpTotals;
            this.tcEmployee.Size = new System.Drawing.Size(770, 297);
            this.tcEmployee.TabIndex = 37;
            this.tcEmployee.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpTotals,
            this.tpCheques,
            this.tpHistory,
            this.tpReversal});
            this.tcEmployee.SelectionChanged += new System.EventHandler(this.tcEmployee_SelectionChanged);
            // 
            // tpTotals
            // 
            this.tpTotals.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tpTotals.Controls.Add(this.dgViewTotals);
            this.tpTotals.Controls.Add(this.panel3);
            this.tpTotals.Location = new System.Drawing.Point(0, 25);
            this.tpTotals.Name = "tpTotals";
            this.tpTotals.Size = new System.Drawing.Size(770, 272);
            this.tpTotals.TabIndex = 3;
            this.tpTotals.Title = "Totals";
            // 
            // dgViewTotals
            // 
            this.dgViewTotals.AllowUserToAddRows = false;
            this.dgViewTotals.AllowUserToDeleteRows = false;
            this.dgViewTotals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgViewTotals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgViewTotals.Location = new System.Drawing.Point(0, 0);
            this.dgViewTotals.Name = "dgViewTotals";
            this.dgViewTotals.Size = new System.Drawing.Size(770, 219);
            this.dgViewTotals.TabIndex = 11;
            this.dgViewTotals.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgViewTotals_CellEnter);
            this.dgViewTotals.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgViewTotals_CellLeave);
            this.dgViewTotals.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgViewTotals_CellValueChanged);
            this.dgViewTotals.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgViewTotals_DataError);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lDepositTotal);
            this.panel3.Controls.Add(this.txtDepositTotal);
            this.panel3.Controls.Add(this.lDifferences);
            this.panel3.Controls.Add(this.txtTotalDifference);
            this.panel3.Controls.Add(this.lSystemTotal);
            this.panel3.Controls.Add(this.lUserTotal);
            this.panel3.Controls.Add(this.txtSystemTotal);
            this.panel3.Controls.Add(this.txtUserTotal);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 219);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(770, 53);
            this.panel3.TabIndex = 12;
            // 
            // txtDepositTotal
            // 
            this.txtDepositTotal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtDepositTotal.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtDepositTotal.Location = new System.Drawing.Point(564, 27);
            this.txtDepositTotal.Name = "txtDepositTotal";
            this.txtDepositTotal.ReadOnly = true;
            this.txtDepositTotal.Size = new System.Drawing.Size(88, 23);
            this.txtDepositTotal.TabIndex = 17;
            this.txtDepositTotal.TabStop = false;
            this.txtDepositTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDepositTotal.Visible = false;
            // 
            // txtTotalDifference
            // 
            this.txtTotalDifference.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtTotalDifference.Location = new System.Drawing.Point(356, 27);
            this.txtTotalDifference.Name = "txtTotalDifference";
            this.txtTotalDifference.ReadOnly = true;
            this.txtTotalDifference.Size = new System.Drawing.Size(88, 23);
            this.txtTotalDifference.TabIndex = 15;
            this.txtTotalDifference.TabStop = false;
            this.txtTotalDifference.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTotalDifference.Visible = false;
            // 
            // txtSystemTotal
            // 
            this.txtSystemTotal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtSystemTotal.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtSystemTotal.Location = new System.Drawing.Point(460, 27);
            this.txtSystemTotal.Name = "txtSystemTotal";
            this.txtSystemTotal.ReadOnly = true;
            this.txtSystemTotal.Size = new System.Drawing.Size(88, 23);
            this.txtSystemTotal.TabIndex = 12;
            this.txtSystemTotal.TabStop = false;
            this.txtSystemTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSystemTotal.Visible = false;
            // 
            // txtUserTotal
            // 
            this.txtUserTotal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtUserTotal.Location = new System.Drawing.Point(668, 27);
            this.txtUserTotal.Name = "txtUserTotal";
            this.txtUserTotal.ReadOnly = true;
            this.txtUserTotal.Size = new System.Drawing.Size(88, 23);
            this.txtUserTotal.TabIndex = 11;
            this.txtUserTotal.TabStop = false;
            this.txtUserTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tpCheques
            // 
            this.tpCheques.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tpCheques.Controls.Add(this.panel7);
            this.tpCheques.Controls.Add(this.dgCheques);
            this.tpCheques.Location = new System.Drawing.Point(0, 25);
            this.tpCheques.Name = "tpCheques";
            this.tpCheques.Selected = false;
            this.tpCheques.Size = new System.Drawing.Size(770, 272);
            this.tpCheques.TabIndex = 4;
            this.tpCheques.Title = "Cheques";
            this.tpCheques.Visible = false;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.label6);
            this.panel7.Controls.Add(this.txtChequesTotal);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel7.Location = new System.Drawing.Point(0, 242);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(770, 30);
            this.panel7.TabIndex = 27;
            this.panel7.Paint += new System.Windows.Forms.PaintEventHandler(this.panel7_Paint);
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.Location = new System.Drawing.Point(588, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 16);
            this.label6.TabIndex = 28;
            this.label6.Text = "Total Value:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtChequesTotal
            // 
            this.txtChequesTotal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtChequesTotal.Location = new System.Drawing.Point(664, 3);
            this.txtChequesTotal.Name = "txtChequesTotal";
            this.txtChequesTotal.ReadOnly = true;
            this.txtChequesTotal.Size = new System.Drawing.Size(100, 23);
            this.txtChequesTotal.TabIndex = 27;
            this.txtChequesTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // dgCheques
            // 
            this.dgCheques.CaptionVisible = false;
            this.dgCheques.DataMember = "";
            this.dgCheques.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgCheques.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgCheques.Location = new System.Drawing.Point(0, 0);
            this.dgCheques.Name = "dgCheques";
            this.dgCheques.ReadOnly = true;
            this.dgCheques.Size = new System.Drawing.Size(770, 272);
            this.dgCheques.TabIndex = 23;
            // 
            // tpHistory
            // 
            this.tpHistory.Controls.Add(this.dgHistory);
            this.tpHistory.Location = new System.Drawing.Point(0, 25);
            this.tpHistory.Name = "tpHistory";
            this.tpHistory.Selected = false;
            this.tpHistory.Size = new System.Drawing.Size(770, 272);
            this.tpHistory.TabIndex = 5;
            this.tpHistory.Title = "History";
            // 
            // dgHistory
            // 
            this.dgHistory.CaptionText = "Cashier Totals History ";
            this.dgHistory.DataMember = "";
            this.dgHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgHistory.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgHistory.LinkColor = System.Drawing.SystemColors.Highlight;
            this.dgHistory.Location = new System.Drawing.Point(0, 0);
            this.dgHistory.Name = "dgHistory";
            this.dgHistory.ParentRowsBackColor = System.Drawing.SystemColors.Highlight;
            this.dgHistory.ParentRowsForeColor = System.Drawing.SystemColors.Window;
            this.dgHistory.ReadOnly = true;
            this.dgHistory.Size = new System.Drawing.Size(770, 272);
            this.dgHistory.TabIndex = 1;
            this.dgHistory.DoubleClick += new System.EventHandler(this.dgHistory_DoubleClick);
            this.dgHistory.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgHistory_MouseUp);
            // 
            // tpReversal
            // 
            this.tpReversal.Controls.Add(this.dgReversal);
            this.tpReversal.Controls.Add(this.panel8);
            this.tpReversal.Location = new System.Drawing.Point(0, 25);
            this.tpReversal.Name = "tpReversal";
            this.tpReversal.Selected = false;
            this.tpReversal.Size = new System.Drawing.Size(770, 272);
            this.tpReversal.TabIndex = 6;
            this.tpReversal.Title = "Reversal";
            // 
            // dgReversal
            // 
            this.dgReversal.CaptionVisible = false;
            this.dgReversal.DataMember = "";
            this.dgReversal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgReversal.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgReversal.Location = new System.Drawing.Point(0, 0);
            this.dgReversal.Name = "dgReversal";
            this.dgReversal.Size = new System.Drawing.Size(770, 223);
            this.dgReversal.TabIndex = 11;
            this.dgReversal.TabStop = false;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.label12);
            this.panel8.Controls.Add(this.txtRDepositTotal);
            this.panel8.Controls.Add(this.label16);
            this.panel8.Controls.Add(this.txtRDiffTotal);
            this.panel8.Controls.Add(this.label17);
            this.panel8.Controls.Add(this.label18);
            this.panel8.Controls.Add(this.txtRSystemTotal);
            this.panel8.Controls.Add(this.txtRUserTotal);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel8.Location = new System.Drawing.Point(0, 223);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(770, 49);
            this.panel8.TabIndex = 20;
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label12.Location = new System.Drawing.Point(551, 5);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(100, 16);
            this.label12.TabIndex = 27;
            this.label12.Text = "Deposit Total";
            this.label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtRDepositTotal
            // 
            this.txtRDepositTotal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtRDepositTotal.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtRDepositTotal.Location = new System.Drawing.Point(559, 21);
            this.txtRDepositTotal.Name = "txtRDepositTotal";
            this.txtRDepositTotal.ReadOnly = true;
            this.txtRDepositTotal.Size = new System.Drawing.Size(88, 23);
            this.txtRDepositTotal.TabIndex = 26;
            this.txtRDepositTotal.TabStop = false;
            this.txtRDepositTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label16.Location = new System.Drawing.Point(343, 5);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(100, 16);
            this.label16.TabIndex = 25;
            this.label16.Text = "Total Difference";
            this.label16.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtRDiffTotal
            // 
            this.txtRDiffTotal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtRDiffTotal.Location = new System.Drawing.Point(351, 21);
            this.txtRDiffTotal.Name = "txtRDiffTotal";
            this.txtRDiffTotal.ReadOnly = true;
            this.txtRDiffTotal.Size = new System.Drawing.Size(88, 23);
            this.txtRDiffTotal.TabIndex = 24;
            this.txtRDiffTotal.TabStop = false;
            this.txtRDiffTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label17
            // 
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label17.Location = new System.Drawing.Point(447, 5);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(100, 16);
            this.label17.TabIndex = 23;
            this.label17.Text = "System Total";
            this.label17.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label18
            // 
            this.label18.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label18.Location = new System.Drawing.Point(671, 5);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(80, 16);
            this.label18.TabIndex = 22;
            this.label18.Text = "User Total";
            this.label18.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtRSystemTotal
            // 
            this.txtRSystemTotal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtRSystemTotal.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtRSystemTotal.Location = new System.Drawing.Point(455, 21);
            this.txtRSystemTotal.Name = "txtRSystemTotal";
            this.txtRSystemTotal.ReadOnly = true;
            this.txtRSystemTotal.Size = new System.Drawing.Size(88, 23);
            this.txtRSystemTotal.TabIndex = 21;
            this.txtRSystemTotal.TabStop = false;
            this.txtRSystemTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtRUserTotal
            // 
            this.txtRUserTotal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtRUserTotal.Location = new System.Drawing.Point(663, 21);
            this.txtRUserTotal.Name = "txtRUserTotal";
            this.txtRUserTotal.ReadOnly = true;
            this.txtRUserTotal.Size = new System.Drawing.Size(88, 23);
            this.txtRUserTotal.TabIndex = 20;
            this.txtRUserTotal.TabStop = false;
            this.txtRUserTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnExclude);
            this.panel2.Controls.Add(this.btnInclude);
            this.panel2.Controls.Add(this.drpCashier);
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Controls.Add(this.btnEmployeePrint);
            this.panel2.Controls.Add(this.btnEmployeeLoad);
            this.panel2.Controls.Add(this.dtEmployeeTo);
            this.panel2.Controls.Add(this.dtEmployeeFrom);
            this.panel2.Controls.Add(this.lEmployeeTo);
            this.panel2.Controls.Add(this.lEmployeeFrom);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.btnReverse);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(770, 100);
            this.panel2.TabIndex = 12;
            // 
            // btnExclude
            // 
            this.btnExclude.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnExclude.Location = new System.Drawing.Point(340, 64);
            this.btnExclude.Name = "btnExclude";
            this.btnExclude.Size = new System.Drawing.Size(208, 24);
            this.btnExclude.TabIndex = 64;
            this.btnExclude.Text = "Exclude Last Deposits From Totals";
            this.btnExclude.Visible = false;
            this.btnExclude.Click += new System.EventHandler(this.btnExclude_Click);
            // 
            // btnInclude
            // 
            this.btnInclude.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnInclude.Location = new System.Drawing.Point(124, 64);
            this.btnInclude.Name = "btnInclude";
            this.btnInclude.Size = new System.Drawing.Size(192, 23);
            this.btnInclude.TabIndex = 63;
            this.btnInclude.Text = "Include Last Deposits In Totals";
            this.btnInclude.Visible = false;
            this.btnInclude.Click += new System.EventHandler(this.btnInclude_Click);
            // 
            // drpCashier
            // 
            this.drpCashier.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.drpCashier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCashier.Enabled = false;
            this.drpCashier.Location = new System.Drawing.Point(4, 29);
            this.drpCashier.Name = "drpCashier";
            this.drpCashier.Size = new System.Drawing.Size(152, 23);
            this.drpCashier.TabIndex = 61;
            this.drpCashier.SelectedIndexChanged += new System.EventHandler(this.drpCashier_SelectedIndexChanged);
            // 
            // btnEmployeePrint
            // 
            this.btnEmployeePrint.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnEmployeePrint.Image = ((System.Drawing.Image)(resources.GetObject("btnEmployeePrint.Image")));
            this.btnEmployeePrint.Location = new System.Drawing.Point(636, 29);
            this.btnEmployeePrint.Name = "btnEmployeePrint";
            this.btnEmployeePrint.Size = new System.Drawing.Size(32, 23);
            this.btnEmployeePrint.TabIndex = 59;
            this.btnEmployeePrint.Click += new System.EventHandler(this.btnEmployeePrint_Click);
            // 
            // btnEmployeeLoad
            // 
            this.btnEmployeeLoad.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnEmployeeLoad.Location = new System.Drawing.Point(580, 29);
            this.btnEmployeeLoad.Name = "btnEmployeeLoad";
            this.btnEmployeeLoad.Size = new System.Drawing.Size(48, 23);
            this.btnEmployeeLoad.TabIndex = 58;
            this.btnEmployeeLoad.Text = "Load";
            this.btnEmployeeLoad.Click += new System.EventHandler(this.btnEmployeeLoad_Click);
            // 
            // dtEmployeeTo
            // 
            this.dtEmployeeTo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dtEmployeeTo.CustomFormat = "ddd dd MMM yyyy HH:mm:ss";
            this.dtEmployeeTo.Enabled = false;
            this.dtEmployeeTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtEmployeeTo.Location = new System.Drawing.Point(340, 29);
            this.dtEmployeeTo.Name = "dtEmployeeTo";
            this.dtEmployeeTo.Size = new System.Drawing.Size(144, 23);
            this.dtEmployeeTo.TabIndex = 57;
            this.dtEmployeeTo.Tag = "";
            this.dtEmployeeTo.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            // 
            // dtEmployeeFrom
            // 
            this.dtEmployeeFrom.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dtEmployeeFrom.CustomFormat = "ddd dd MMM yyyy HH:mm:ss";
            this.dtEmployeeFrom.Enabled = false;
            this.dtEmployeeFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtEmployeeFrom.Location = new System.Drawing.Point(172, 29);
            this.dtEmployeeFrom.Name = "dtEmployeeFrom";
            this.dtEmployeeFrom.Size = new System.Drawing.Size(144, 23);
            this.dtEmployeeFrom.TabIndex = 56;
            this.dtEmployeeFrom.Tag = "";
            this.dtEmployeeFrom.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            // 
            // lEmployeeTo
            // 
            this.lEmployeeTo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lEmployeeTo.Location = new System.Drawing.Point(340, 13);
            this.lEmployeeTo.Name = "lEmployeeTo";
            this.lEmployeeTo.Size = new System.Drawing.Size(32, 16);
            this.lEmployeeTo.TabIndex = 55;
            this.lEmployeeTo.Text = "To:";
            // 
            // lEmployeeFrom
            // 
            this.lEmployeeFrom.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lEmployeeFrom.Location = new System.Drawing.Point(172, 13);
            this.lEmployeeFrom.Name = "lEmployeeFrom";
            this.lEmployeeFrom.Size = new System.Drawing.Size(40, 16);
            this.lEmployeeFrom.TabIndex = 54;
            this.lEmployeeFrom.Text = "From:";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.Location = new System.Drawing.Point(4, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 16);
            this.label5.TabIndex = 53;
            this.label5.Text = "Cashier";
            // 
            // btnReverse
            // 
            this.btnReverse.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnReverse.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnReverse.ForeColor = System.Drawing.SystemColors.ActiveBorder;
            this.btnReverse.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnReverse.Location = new System.Drawing.Point(500, 29);
            this.btnReverse.Name = "btnReverse";
            this.btnReverse.Size = new System.Drawing.Size(64, 24);
            this.btnReverse.TabIndex = 62;
            this.btnReverse.Text = "Reverse";
            this.btnReverse.UseVisualStyleBackColor = false;
            this.btnReverse.Visible = false;
            this.btnReverse.Click += new System.EventHandler(this.btnReverse_Click);
            // 
            // tpBranch
            // 
            this.tpBranch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tpBranch.AutoSize = true;
            this.tpBranch.Controls.Add(this.dgBranch);
            this.tpBranch.Controls.Add(this.panel4);
            this.tpBranch.Controls.Add(this.panel5);
            this.tpBranch.Location = new System.Drawing.Point(0, 25);
            this.tpBranch.Name = "tpBranch";
            this.tpBranch.Selected = false;
            this.tpBranch.Size = new System.Drawing.Size(770, 397);
            this.tpBranch.TabIndex = 3;
            this.tpBranch.Title = "Branch";
            // 
            // dgBranch
            // 
            this.dgBranch.CaptionVisible = false;
            this.dgBranch.DataMember = "";
            this.dgBranch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgBranch.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgBranch.Location = new System.Drawing.Point(0, 70);
            this.dgBranch.Name = "dgBranch";
            this.dgBranch.ReadOnly = true;
            this.dgBranch.Size = new System.Drawing.Size(770, 293);
            this.dgBranch.TabIndex = 22;
            this.dgBranch.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgBranch_MouseUp);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.btnBranchPrint);
            this.panel4.Controls.Add(this.btnExcel);
            this.panel4.Controls.Add(this.btnBranchLoad);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.dtBranchTo);
            this.panel4.Controls.Add(this.dtBranchFrom);
            this.panel4.Controls.Add(this.lTo);
            this.panel4.Controls.Add(this.lFrom);
            this.panel4.Controls.Add(this.drpBranch);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(770, 70);
            this.panel4.TabIndex = 50;
            // 
            // btnBranchPrint
            // 
            this.btnBranchPrint.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnBranchPrint.Image = ((System.Drawing.Image)(resources.GetObject("btnBranchPrint.Image")));
            this.btnBranchPrint.Location = new System.Drawing.Point(644, 39);
            this.btnBranchPrint.Name = "btnBranchPrint";
            this.btnBranchPrint.Size = new System.Drawing.Size(32, 23);
            this.btnBranchPrint.TabIndex = 52;
            this.btnBranchPrint.Click += new System.EventHandler(this.btnBranchPrint_Click);
            // 
            // btnExcel
            // 
            this.btnExcel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnExcel.Enabled = false;
            this.btnExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnExcel.Image")));
            this.btnExcel.Location = new System.Drawing.Point(684, 39);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(32, 24);
            this.btnExcel.TabIndex = 51;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // btnBranchLoad
            // 
            this.btnBranchLoad.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnBranchLoad.Location = new System.Drawing.Point(588, 39);
            this.btnBranchLoad.Name = "btnBranchLoad";
            this.btnBranchLoad.Size = new System.Drawing.Size(48, 23);
            this.btnBranchLoad.TabIndex = 50;
            this.btnBranchLoad.Text = "Load";
            this.btnBranchLoad.Click += new System.EventHandler(this.btnBranchLoad_Click);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.Location = new System.Drawing.Point(41, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 16);
            this.label4.TabIndex = 25;
            this.label4.Text = "Choose Branch";
            // 
            // dtBranchTo
            // 
            this.dtBranchTo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dtBranchTo.CustomFormat = "ddd dd MMM yyyy HH:mm";
            this.dtBranchTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtBranchTo.Location = new System.Drawing.Point(313, 40);
            this.dtBranchTo.Name = "dtBranchTo";
            this.dtBranchTo.Size = new System.Drawing.Size(144, 23);
            this.dtBranchTo.TabIndex = 24;
            this.dtBranchTo.Tag = "";
            this.dtBranchTo.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            // 
            // dtBranchFrom
            // 
            this.dtBranchFrom.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dtBranchFrom.CustomFormat = "ddd dd MMM yyyy HH:mm";
            this.dtBranchFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtBranchFrom.Location = new System.Drawing.Point(145, 40);
            this.dtBranchFrom.Name = "dtBranchFrom";
            this.dtBranchFrom.Size = new System.Drawing.Size(144, 23);
            this.dtBranchFrom.TabIndex = 23;
            this.dtBranchFrom.Tag = "";
            this.dtBranchFrom.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            // 
            // lTo
            // 
            this.lTo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lTo.Location = new System.Drawing.Point(313, 24);
            this.lTo.Name = "lTo";
            this.lTo.Size = new System.Drawing.Size(32, 16);
            this.lTo.TabIndex = 22;
            this.lTo.Text = "To:";
            // 
            // lFrom
            // 
            this.lFrom.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lFrom.Location = new System.Drawing.Point(145, 24);
            this.lFrom.Name = "lFrom";
            this.lFrom.Size = new System.Drawing.Size(40, 16);
            this.lFrom.TabIndex = 21;
            this.lFrom.Text = "From:";
            // 
            // drpBranch
            // 
            this.drpBranch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.drpBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranch.Location = new System.Drawing.Point(41, 40);
            this.drpBranch.Name = "drpBranch";
            this.drpBranch.Size = new System.Drawing.Size(80, 23);
            this.drpBranch.TabIndex = 20;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label3);
            this.panel5.Controls.Add(this.txtBranchTotal);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(0, 363);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(770, 34);
            this.panel5.TabIndex = 51;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.Location = new System.Drawing.Point(548, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 16);
            this.label3.TabIndex = 26;
            this.label3.Text = "Total Value:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBranchTotal
            // 
            this.txtBranchTotal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtBranchTotal.Location = new System.Drawing.Point(628, 5);
            this.txtBranchTotal.Name = "txtBranchTotal";
            this.txtBranchTotal.ReadOnly = true;
            this.txtBranchTotal.Size = new System.Drawing.Size(100, 23);
            this.txtBranchTotal.TabIndex = 25;
            this.txtBranchTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tpSummary
            // 
            this.tpSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tpSummary.AutoSize = true;
            this.tpSummary.Controls.Add(this.dgSummary);
            this.tpSummary.Controls.Add(this.panel6);
            this.tpSummary.Location = new System.Drawing.Point(0, 25);
            this.tpSummary.Name = "tpSummary";
            this.tpSummary.Selected = false;
            this.tpSummary.Size = new System.Drawing.Size(770, 397);
            this.tpSummary.TabIndex = 5;
            this.tpSummary.Title = "Summary Report";
            // 
            // dgSummary
            // 
            this.dgSummary.CaptionVisible = false;
            this.dgSummary.DataMember = "";
            this.dgSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgSummary.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgSummary.Location = new System.Drawing.Point(0, 63);
            this.dgSummary.Name = "dgSummary";
            this.dgSummary.ReadOnly = true;
            this.dgSummary.Size = new System.Drawing.Size(770, 334);
            this.dgSummary.TabIndex = 25;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.btnSummaryPrint);
            this.panel6.Controls.Add(this.dtSummaryTo);
            this.panel6.Controls.Add(this.dtSummaryFrom);
            this.panel6.Controls.Add(this.label2);
            this.panel6.Controls.Add(this.label7);
            this.panel6.Controls.Add(this.label1);
            this.panel6.Controls.Add(this.btnSummaryLoad);
            this.panel6.Controls.Add(this.drpSummaryBranch);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(770, 63);
            this.panel6.TabIndex = 38;
            // 
            // btnSummaryPrint
            // 
            this.btnSummaryPrint.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSummaryPrint.Image = ((System.Drawing.Image)(resources.GetObject("btnSummaryPrint.Image")));
            this.btnSummaryPrint.Location = new System.Drawing.Point(684, 22);
            this.btnSummaryPrint.Name = "btnSummaryPrint";
            this.btnSummaryPrint.Size = new System.Drawing.Size(32, 23);
            this.btnSummaryPrint.TabIndex = 45;
            this.btnSummaryPrint.Click += new System.EventHandler(this.btnSummaryPrint_Click);
            // 
            // dtSummaryTo
            // 
            this.dtSummaryTo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dtSummaryTo.CustomFormat = "ddd dd MMM yyyy HH:mm";
            this.dtSummaryTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtSummaryTo.Location = new System.Drawing.Point(317, 22);
            this.dtSummaryTo.Name = "dtSummaryTo";
            this.dtSummaryTo.Size = new System.Drawing.Size(144, 23);
            this.dtSummaryTo.TabIndex = 44;
            this.dtSummaryTo.Tag = "";
            this.dtSummaryTo.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            // 
            // dtSummaryFrom
            // 
            this.dtSummaryFrom.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dtSummaryFrom.CustomFormat = "ddd dd MMM yyyy HH:mm";
            this.dtSummaryFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtSummaryFrom.Location = new System.Drawing.Point(149, 22);
            this.dtSummaryFrom.Name = "dtSummaryFrom";
            this.dtSummaryFrom.Size = new System.Drawing.Size(144, 23);
            this.dtSummaryFrom.TabIndex = 43;
            this.dtSummaryFrom.Tag = "";
            this.dtSummaryFrom.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.Location = new System.Drawing.Point(317, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 16);
            this.label2.TabIndex = 42;
            this.label2.Text = "To:";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.Location = new System.Drawing.Point(149, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 16);
            this.label7.TabIndex = 41;
            this.label7.Text = "From:";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.Location = new System.Drawing.Point(45, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 39;
            this.label1.Text = "Choose Branch";
            // 
            // btnSummaryLoad
            // 
            this.btnSummaryLoad.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSummaryLoad.Location = new System.Drawing.Point(628, 22);
            this.btnSummaryLoad.Name = "btnSummaryLoad";
            this.btnSummaryLoad.Size = new System.Drawing.Size(48, 23);
            this.btnSummaryLoad.TabIndex = 40;
            this.btnSummaryLoad.Text = "Load";
            this.btnSummaryLoad.Click += new System.EventHandler(this.btnSummaryLoad_Click);
            // 
            // drpSummaryBranch
            // 
            this.drpSummaryBranch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.drpSummaryBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpSummaryBranch.Location = new System.Drawing.Point(45, 22);
            this.drpSummaryBranch.Name = "drpSummaryBranch";
            this.drpSummaryBranch.Size = new System.Drawing.Size(80, 23);
            this.drpSummaryBranch.TabIndex = 38;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lDeposit);
            this.panel1.Controls.Add(this.btnCalc);
            this.panel1.Controls.Add(this.btnExit);
            this.panel1.Controls.Add(this.btnClear);
            this.panel1.Controls.Add(this.lAuthorise);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(770, 34);
            this.panel1.TabIndex = 7;
            // 
            // lDeposit
            // 
            this.lDeposit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lDeposit.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lDeposit.Enabled = false;
            this.lDeposit.Location = new System.Drawing.Point(592, 9);
            this.lDeposit.Name = "lDeposit";
            this.lDeposit.Size = new System.Drawing.Size(16, 16);
            this.lDeposit.TabIndex = 21;
            this.lDeposit.Visible = false;
            // 
            // btnCalc
            // 
            this.btnCalc.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCalc.Image = ((System.Drawing.Image)(resources.GetObject("btnCalc.Image")));
            this.btnCalc.Location = new System.Drawing.Point(615, 5);
            this.btnCalc.Name = "btnCalc";
            this.btnCalc.Size = new System.Drawing.Size(32, 23);
            this.btnCalc.TabIndex = 20;
            this.btnCalc.Click += new System.EventHandler(this.btnCalc_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnExit.Location = new System.Drawing.Point(711, 5);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(48, 23);
            this.btnExit.TabIndex = 19;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Location = new System.Drawing.Point(655, 5);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(48, 23);
            this.btnClear.TabIndex = 18;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lAuthorise
            // 
            this.lAuthorise.Enabled = false;
            this.lAuthorise.Location = new System.Drawing.Point(36, 5);
            this.lAuthorise.Name = "lAuthorise";
            this.lAuthorise.Size = new System.Drawing.Size(16, 16);
            this.lAuthorise.TabIndex = 17;
            // 
            // CashierTotals
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(774, 471);
            this.Controls.Add(this.groupBox1);
            this.Name = "CashierTotals";
            this.Text = "Cashier Totals";
            this.Load += new System.EventHandler(this.CashierTotals_Load);
            this.Enter += new System.EventHandler(this.CashierTotals_Enter);
            this.groupBox1.ResumeLayout(false);
            this.tcMain.ResumeLayout(false);
            this.tcMain.PerformLayout();
            this.tpEmployee.ResumeLayout(false);
            this.tcEmployee.ResumeLayout(false);
            this.tpTotals.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgViewTotals)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tpCheques.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgCheques)).EndInit();
            this.tpHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgHistory)).EndInit();
            this.tpReversal.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgReversal)).EndInit();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tpBranch.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgBranch)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.tpSummary.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgSummary)).EndInit();
            this.panel6.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }

        private void CashierTotals_Enter(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                loading = false;

                // FR67835 Must refresh the totals
                if (dgViewTotals.DataSource != null)
                {
                    this.LoadEmployeeTotals((int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo], false);
                    _authorisedBy = 0;
                }
                if (dgCheques.DataSource != null)
                    this.LoadEmployeeTotals((int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo], true);
                if (dgHistory.DataSource != null)
                    this.LoadEmployeeHistory((int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo]);
                if (dgBranch.DataSource != null)
                    this.btnBranchLoad_Click(sender, e);
                if (dgSummary.DataSource != null)
                    this.btnSummaryLoad_Click(sender, e);
            }
            catch (Exception ex)
            {
                Catch(ex, "CashierTotals_Enter");
            }
            finally
            {
                StopWait();
            }

        }

        /// <summary>
        /// Load for the branch level tab page. Displays a summary of cashier totals 
        /// for the selected branch.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBranchLoad_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                decimal total = 0;
                BranchTotals = PaymentManager.GetCashierTotals((short)((DataRowView)drpBranch.SelectedItem)[CN.BranchNo],
                    -1,		/* denotes "all staff" */
                    dtBranchFrom.Value,
                    dtBranchTo.Value,
                    false,
                    out total,
                    out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    btnExcel.Enabled = BranchTotals.Tables[TN.CashierTotals].DefaultView.Count > 0;

                    dgBranch.DataSource = null;         // #14942       
                    dgBranch.DataSource = BranchTotals;
                    dgBranch.DataMember = TN.CashierTotals;

                    dgBranch.TableStyles.Clear();

                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                    tabStyle.MappingName = TN.CashierTotals;
                    dgBranch.TableStyles.Add(tabStyle);

                    tabStyle.GridColumnStyles[CN.EmployeeNo.ToLower()].Width = 120;
                    tabStyle.GridColumnStyles[CN.EmployeeNo.ToLower()].HeaderText = GetResource("T_EMPEENO");
                    tabStyle.GridColumnStyles[CN.TransTypeCode].Width = 120;
                    tabStyle.GridColumnStyles[CN.TransTypeCode].HeaderText = GetResource("T_TYPE");
                    tabStyle.GridColumnStyles[CN.CodeDescript].Width = 120;
                    tabStyle.GridColumnStyles[CN.CodeDescript].HeaderText = GetResource("T_PAYMETHOD");
                    tabStyle.GridColumnStyles[CN.DateTrans].Width = 120;
                    tabStyle.GridColumnStyles[CN.DateTrans].HeaderText = GetResource("T_DATE");
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.DateTrans]).Format = "g";
                    tabStyle.GridColumnStyles[CN.TransValue].Width = 120;
                    tabStyle.GridColumnStyles[CN.TransValue].HeaderText = GetResource("T_VALUE");
                    tabStyle.GridColumnStyles[CN.TransValue].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TransValue]).Format = DecimalPlaces;
                    tabStyle.GridColumnStyles[CN.PayMethod].Width = 0;
                    tabStyle.GridColumnStyles[CN.ForeignTender].Width = 0;
                    tabStyle.GridColumnStyles[CN.LocalChange].Width = 0;
                    tabStyle.GridColumnStyles[CN.Securitised].Width = 0;



                    txtBranchTotal.Text = total.ToString(DecimalPlaces);
                    ((MainForm)FormRoot).statusBar1.Text = BranchTotals.Tables[TN.CashierTotals].Rows.Count.ToString() + " Rows returned";
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnBranchLoad_Click");
            }
            finally
            {
                StopWait();
            }


        }

        private void dgBranch_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (dgBranch.CurrentRowIndex >= 0)
                dgBranch.Select(dgBranch.CurrentRowIndex);
        }

        /// <summary>
        /// resets the whole screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                dgBranch.DataSource = null;
                dgCheques.DataSource = null;
                //dgEmployee2.DataSource = null;
                dgViewTotals.DataSource = null;
                dgHistory.DataSource = null;
                dgReversal.DataSource = null;
                dgSummary.DataSource = null;
                this.btnReverse.Enabled = false;

                DateTime serverTime = StaticDataManager.GetServerDateTime();
                dtBranchFrom.Value = serverTime;
                dtBranchTo.Value = serverTime;

                dtEmployeeFrom.Value = serverTime;
                dtEmployeeFrom.Enabled = true;
                dtEmployeeTo.Value = serverTime;
                dtEmployeeTo.Enabled = true;

                drpBranch.SelectedIndex = 0;
                //drpCashier.SelectedIndex = 0;

                txtBranchTotal.Text = (0).ToString(DecimalPlaces);
                txtChequesTotal.Text = (0).ToString(DecimalPlaces);

                txtUserTotal.Text = (0).ToString(DecimalPlaces);
                txtSystemTotal.Text = (0).ToString(DecimalPlaces);
                txtTotalDifference.Text = (0).ToString(DecimalPlaces);
                txtDepositTotal.Text = (0).ToString(DecimalPlaces);

                txtSystemTotal.Visible = lSystemTotal.Visible = false;
                txtTotalDifference.Visible = lDifferences.Visible = false;

                //if (dgEmployee2.TableStyles.Count > 0)
                //    ShowBreakDown(false);
                _authorisedBy = 0;
                ((MainForm)FormRoot).statusBar1.Text = "";

                btnExcel.Enabled = false;

                btnInclude.Visible = false;
                btnExclude.Visible = false;
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

        /// <summary>
        /// Loads a cashier's totals history. This is displayed in the second
        /// tab page on the employee tab page. These records can be double clicked
        /// to show the breakdown by payment method in a popup.
        /// </summary>
        /// <param name="employee"></param>
        private void LoadEmployeeHistory(int employee)
        {
            DataSet history = PaymentManager.GetCashierTotalsHistory(employee, out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                dgHistory.DataSource = history.Tables[0].DefaultView;
                dgHistory.TableStyles.Clear();

                DataGridTableStyle tabStyle = new DataGridTableStyle();
                tabStyle.MappingName = history.Tables[0].TableName;

                AddColumnStyle(CN.ID, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                AddColumnStyle(CN.DateFrom, tabStyle, 110, true, GetResource("T_DATEFROM"), "", HorizontalAlignment.Left);
                AddColumnStyle(CN.DateTo, tabStyle, 110, true, GetResource("T_DATETO"), "", HorizontalAlignment.Left);
                AddColumnStyle(CN.UserValue, tabStyle, 80, true, GetResource("T_USERVALUE"), DecimalPlaces, HorizontalAlignment.Right);
                AddColumnStyle(CN.SystemValue, tabStyle, 80, true, GetResource("T_SYSTEMVALUE"), DecimalPlaces, HorizontalAlignment.Right);
                AddColumnStyle(CN.Deposit, tabStyle, 80, true, GetResource("T_DEPOSIT"), DecimalPlaces, HorizontalAlignment.Right);
                AddColumnStyle(CN.Difference, tabStyle, 80, true, GetResource("T_DIFFERENCE"), DecimalPlaces, HorizontalAlignment.Right);
                //AddColumnStyle(CN.Reason, tabStyle, 100, true, GetResource("T_REASON"), "", HorizontalAlignment.Left);
                dgHistory.TableStyles.Add(tabStyle);

                ((MainForm)FormRoot).statusBar1.Text = history.Tables[0].DefaultView.Count.ToString() + " Rows returned";
            }


        }

        /// <summary>
        /// For the moment this reversal functionality is not implemented because it 
        /// hasn't been properly thought through yet. Specifically, how do you cope when
        /// you reverse cashier totals when the money has already been sent to FACT and 
        /// deposited to the bank?
        /// </summary>
        /// <param name="employee"></param>
        private void LoadEmployeeReversal(int employee)
        {
            bool canReverse;
            DateTime dateFrom;
            DateTime dateTo;
            decimal diffTotal;
            decimal systemTotal;
            decimal depositTotal;
            decimal userTotal;

            DataSet reversal = PaymentManager.GetCashierTotalsReversal(
                employee, out canReverse, out dateFrom, out dateTo,
                out diffTotal, out systemTotal, out depositTotal, out userTotal,
                out Error);

            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                // Display the reverse button
                this.btnSave.Visible = false;
                //this.btnReverse.Visible = true;
                this.btnReverse.Enabled = canReverse;

                // Set the date range
                dtEmployeeFrom.Value = dateFrom;
                dtEmployeeTo.Value = dateTo;

                // Set the totals
                txtRDiffTotal.Text = diffTotal.ToString(DecimalPlaces);
                txtRSystemTotal.Text = systemTotal.ToString(DecimalPlaces);
                txtRDepositTotal.Text = depositTotal.ToString(DecimalPlaces);
                txtRUserTotal.Text = userTotal.ToString(DecimalPlaces);

                // Display the reversal list
                reversal.Tables[0].DefaultView.AllowNew = false;
                dgReversal.DataSource = reversal.Tables[0].DefaultView;
                dgReversal.TableStyles.Clear();

                DataGridTableStyle tabStyle = new DataGridTableStyle();
                tabStyle.MappingName = reversal.Tables[0].TableName;

                AddColumnStyle(CN.ID, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                AddColumnStyle(CN.DateFrom, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                AddColumnStyle(CN.DateTo, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                AddColumnStyle(CN.PayMethod, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                AddColumnStyle(CN.CodeDescription, tabStyle, 120, true, GetResource("T_PAYMETHOD"), "", HorizontalAlignment.Left);
                AddColumnStyle(CN.Difference, tabStyle, 80, true, GetResource("T_DIFFERENCE"), DecimalPlaces, HorizontalAlignment.Right);
                AddColumnStyle(CN.SystemTotal, tabStyle, 80, true, GetResource("T_SYSTEMVALUE"), DecimalPlaces, HorizontalAlignment.Right);
                AddColumnStyle(CN.Deposit, tabStyle, 80, true, GetResource("T_DEPOSIT"), DecimalPlaces, HorizontalAlignment.Right);
                AddColumnStyle(CN.UserTotal, tabStyle, 80, true, GetResource("T_USERVALUE"), DecimalPlaces, HorizontalAlignment.Right);
                AddColumnStyle(CN.Reason, tabStyle, 140, true, GetResource("T_REASON"), "", HorizontalAlignment.Left);

                dgReversal.TableStyles.Add(tabStyle);

                ((MainForm)FormRoot).statusBar1.Text = reversal.Tables[0].DefaultView.Count.ToString() + " Rows returned";

                if (!canReverse) ShowInfo("M_NOTOTALREVERSE");
            }
        }


        private void LoadEmployeeTotals(int employee, bool listCheques)
        {
            dtEmployeeTo.Value = StaticDataManager.GetServerDateTime();

            decimal total = 0;

            if (listCheques)
            {
                EmpTotals = PaymentManager.GetCashierTotals(Convert.ToInt16(Config.BranchCode),
            (int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo],
            dtEmployeeFrom.Value,
            dtEmployeeTo.Value,
            listCheques,
            out total,
            out Error);

                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    dgCheques.DataSource = EmpTotals;
                    dgCheques.DataMember = EmpTotals.Tables[1].TableName;
                    dgCheques.TableStyles.Clear();

                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                    tabStyle.MappingName = EmpTotals.Tables[1].TableName;
                    dgCheques.TableStyles.Add(tabStyle);

                    tabStyle.GridColumnStyles[CN.name].HeaderText = GetResource("T_NAME");
                    tabStyle.GridColumnStyles[CN.name].Width = 140;
                    tabStyle.GridColumnStyles[CN.AccountNumber].HeaderText = GetResource("T_ACCTNO");
                    tabStyle.GridColumnStyles[CN.AccountNumber].Width = 160;
                    tabStyle.GridColumnStyles[CN.ChequeNo].HeaderText = GetResource("T_CHEQUENO");
                    tabStyle.GridColumnStyles[CN.BankName].HeaderText = GetResource("T_BANKNAME");
                    tabStyle.GridColumnStyles[CN.BankCode].Width = 0;
                    tabStyle.GridColumnStyles[CN.BankAccountNo2].HeaderText = GetResource("T_BANKACCTNO");
                    tabStyle.GridColumnStyles[CN.TransValue].HeaderText = GetResource("T_VALUE");
                    tabStyle.GridColumnStyles[CN.TransValue].Alignment = HorizontalAlignment.Right;
                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TransValue]).Format = DecimalPlaces;
                    tabStyle.GridColumnStyles[CN.DateTrans].Width = 0;

                    txtChequesTotal.Text = total.ToString(DecimalPlaces);
                    ((MainForm)FormRoot).statusBar1.Text = EmpTotals.Tables[0].Rows.Count.ToString() + " Rows returned";
                }
            }
            else
            {
                InitialiseCashierTotals(EmpTotals);


                Client.Call(new GetCashierTotalsRequest
                {

                    DateFrom = dtEmployeeFrom.Value,
                    dateto = dtEmployeeTo.Value,
                    Empeeno = employee
                },
                   response =>
                   {
                       this.exchangeRates = response.ExchangeRates;
                       Setup(response.Cashiertotals);
                       SetupDGVs();
                       dtEmployeeTo.Enabled = false;
                   }, this);
            }



        }

        /// <summary>
        /// This will load the system values into the main datagrid on the employee tab.
        /// This is where the user will enter the cashier's totals. These totals will 
        /// then be compared to what the system thinks the cashier should have. There 
        /// are two components to this; firstly, the amount of payments taken (in the 
        /// fintrans table) since they last totalled; secondly, the amount of that 
        /// money they may already have deposited. Therefore, the amount they enter is
        /// expected to equal the amount they have taken minus the amount they have deposited.
        /// These sums are all taken from the last time the cashier saved totals. 
        /// 
        /// This method just loads the data. It's potentially confd that the same
        /// web service call is used to return data for the cheques tab as well as
        /// the cashier totals tab but with different parameters. Most of the work is done 
        /// inside InitialiseCashierTotals.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEmployeeLoad_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                DGVEnableColumns(false);
                //ShowBreakDown(false);

                _authorisedBy = 0;

                LoadLastAudit();                        //IP - 05/01/12 - #9358 - Need to set DateFrom for the employee.

                if (tcEmployee.SelectedTab == tpHistory)
                {
                    LoadEmployeeHistory((int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo]);
                }
                else if (tcEmployee.SelectedTab == tpReversal)
                {
                    LoadEmployeeReversal((int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo]);
                }
                else
                {
                    LoadEmployeeTotals((int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo], (tcEmployee.SelectedTab == tpCheques));
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnBranchLoad_Click");
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// This will set up the datagrid that is used to enter the user values when saving cashier totals.
        /// A list of the securitised and system totals for each payment method is
        /// maintained in a hashtable throughout. 
        /// A complication is that for foreign currency payments we have to add the 
        /// value of the foreign tender to the system total for that payment method
        /// but remove the value of local change from the system value of the cash 
        /// payment method. This is because when a cashier accepts a payment in 
        /// foreign currency, they will not give the customer change in the 
        /// foreign currency but in the local currency (in cash).
        /// We then loop through the payment methods and look up the deposits made
        /// by that cashier for that payment method. This is so we can later see if
        /// the value they enter is correct.
        /// To begin with the user can only see the column into which they are expected 
        /// to record their totals. This is so they don't just enter what the system
        /// wants them to enter.
        /// </summary>
        /// <param name="cashierTotals"></param>
        private void InitialiseCashierTotals(DataSet cashierTotals)
        {
            //   DataView deposits = cashierTotals.Tables[TN.CashierDeposits].DefaultView;

            /* create a tally of totals by paymethod */
            Hashtable totals = new Hashtable(((DataTable)StaticData.Tables[TN.PayMethod]).Rows.Count);

            //foreach (DataRow r in cashierTotals.Tables[TN.CashierTotals].Rows)
            //{
            //    PayMethodTotal pt;
            //    PayMethodTotal cash;
            //    pt.SecuritisedValue = pt.SystemValue = cash.SecuritisedValue = cash.SystemValue = 0;


            //    if (totals.ContainsKey((short)r[CN.PayMethod]))
            //    {
            //        pt = (PayMethodTotal)totals[(short)r[CN.PayMethod]];
            //        //d = (decimal)totals[(short)r[CN.PayMethod]];
            //        totals.Remove((short)r[CN.PayMethod]);
            //    }

            //    if ((short)r[CN.PayMethod] >= CAT.FPMForeignCurrency)		/* if it's a foreign currency transaction */
            //    {
            //        pt.SystemValue += 0; // (decimal)r[CN.ForeignTender];
            //        if ((string)r[CN.Securitised] == "Y")
            //            pt.SecuritisedValue += (decimal)r[CN.ForeignTender];

            //        //d += (decimal)r[CN.ForeignTender];
            //        if (totals.ContainsKey(Convert.ToInt16(PayMethod.Cash)))
            //        {
            //            cash = (PayMethodTotal)totals[Convert.ToInt16(PayMethod.Cash)];
            //            //cash = (decimal)totals[Convert.ToInt16(PayMethod.Cash)];
            //            totals.Remove(Convert.ToInt16(PayMethod.Cash));
            //        }
            //        cash.SystemValue -= (decimal)r[CN.LocalChange];
            //        if ((string)r[CN.Securitised] == "Y")
            //            cash.SecuritisedValue -= (decimal)r[CN.LocalChange];

            //        totals.Add(Convert.ToInt16(PayMethod.Cash), cash);
            //    }
            //    else
            //    {
            //        pt.SystemValue += (decimal)r[CN.TransValue];
            //        if ((string)r[CN.Securitised] == "Y")
            //            pt.SecuritisedValue += (decimal)r[CN.TransValue];

            //        //d += (decimal)r[CN.TransValue];
            //    }
            //    totals.Add((short)r[CN.PayMethod], pt);
            //}

            //DataTable dt = new DataTable(TN.CashierTotals);
            //dt.Columns.AddRange(new DataColumn[] {	new DataColumn(CN.Code),
            //                                         new DataColumn(CN.ReadOnly),
            //                                         new DataColumn(CN.CodeDescription),
            //                                         new DataColumn(CN.Difference),
            //                                         new DataColumn(CN.SystemValue),
            //                                         new DataColumn(CN.Deposit),
            //                                         new DataColumn(CN.UserValue), 
            //                                         new DataColumn(CN.Reason),
            //                                         new DataColumn(CN.SecuritisedValue),
            //                                         new DataColumn(CN.Reference),
            //                                         new DataColumn (CN.Additional2)});              //IP - 07/12/11 - CR1234 - column holds whether shortage/overage is allowed for a paymethod.
            //dt.Columns[CN.Code].ReadOnly = true;
            //dt.Columns[CN.CodeDescription].ReadOnly = true;
            //dt.Columns[CN.SystemValue].ReadOnly = true;
            //dt.Columns[CN.Difference].ReadOnly = true;
            //dt.Columns[CN.Deposit].ReadOnly = true;

            //foreach (DataRow r in ((DataTable)StaticData.Tables[TN.PayMethod]).Rows)
            //{
            //    if (((string)r[CN.Code]).Trim() != "0")	/* ignore 'not applicable */
            //    {
            //        PayMethodTotal sysVal;
            //        sysVal.SecuritisedValue = sysVal.SystemValue = 0;
            //        DataRow row = dt.NewRow();
            //        row[CN.Code] = r[CN.Code];
            //        row[CN.CodeDescription] = r[CN.CodeDescription];
            //        //row[CN.SystemValue] = totals[Convert.ToInt32(r[CN.Code])].ToString(DecimalPlaces);
            //        if (totals.ContainsKey(Convert.ToInt16(r[CN.Code])))
            //            sysVal = (PayMethodTotal)totals[Convert.ToInt16(r[CN.Code])];
            //        row[CN.SystemValue] = sysVal.SystemValue.ToString(DecimalPlaces);
            //        row[CN.SecuritisedValue] = sysVal.SecuritisedValue.ToString(DecimalPlaces);
            //        row[CN.UserValue] = (0).ToString(DecimalPlaces);
            //        row[CN.ReadOnly] = "N";
            //        row[CN.Reason] = "";

            //        /* need to find the value of cashier's deposits for this pay method */
            //        decimal depositTotal = 0;
            //        //deposits.RowFilter = CN.Code + " = '" + ((string)r[CN.Code]).Trim() + "' and " + CN.IncludeInCashierTotals + " = 1";
            //        //foreach (DataRowView rv in deposits)
            //        //    depositTotal += (decimal)rv[CN.DepositValue];
            //        //deposits.RowFilter = "";
            //    //    row[CN.Deposit] = depositTotal.ToString(DecimalPlaces);

            //        /* now fill in the difference:
            //         * usertotal - (systemtotal - deposit) */
            //        //row[CN.Difference] = (0 - (totals[Convert.ToInt32(r[CN.Code])] - depositTotal)).ToString(DecimalPlaces);
            //        row[CN.Difference] = (0 - (sysVal.SystemValue - depositTotal)).ToString(DecimalPlaces);
            //        row[CN.Reference] = r[CN.Reference];
            //        row[CN.Additional2] = r[CN.Additional2];                //IP - 07/12/11 - CR1234

            //        dt.Rows.Add(row);
            //    }
            //}

            //dt.ColumnChanging += new DataColumnChangeEventHandler(this.OnColumnChange);
            //dt.ColumnChanged += new DataColumnChangeEventHandler(this.OnColumnChanged);
            //dt.DefaultView.AllowNew = false;
            //dgEmployee2.DataSource = dt.DefaultView;
            //dgEmployee2.TableStyles.Clear();

            DataGridTableStyle tabStyle = new DataGridTableStyle();
            tabStyle.MappingName = TN.CashierTotals;

            AddColumnStyle(CN.ReadOnly, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
            AddColumnStyle(CN.Code, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
            AddColumnStyle(CN.CodeDescription, tabStyle, 120, true, GetResource("T_PAYMETHOD"), "", HorizontalAlignment.Left);

            AddColumnStyle(CN.Difference, tabStyle, 0, true, "", DecimalPlaces, HorizontalAlignment.Right);
            AddColumnStyle(CN.SystemValue, tabStyle, 0, true, "", DecimalPlaces, HorizontalAlignment.Right);
            AddColumnStyle(CN.Deposit, tabStyle, 0, true, "", DecimalPlaces, HorizontalAlignment.Right);
            //AddColumnStyle(CN.SecuritisedValue, tabStyle, 80, true, "", DecimalPlaces, HorizontalAlignment.Right);
            AddColumnStyle(CN.Reference, tabStyle, 0, true, "", "", HorizontalAlignment.Left);

            DataGridEditColumn aColumnEditColumn;
            aColumnEditColumn = new DataGridEditColumn(CN.ReadOnly, "Y");
            aColumnEditColumn.MappingName = CN.UserValue;
            aColumnEditColumn.HeaderText = GetResource("T_USERVALUE");
            aColumnEditColumn.Width = 80;
            aColumnEditColumn.ReadOnly = false;
            aColumnEditColumn.Format = DecimalPlaces;
            aColumnEditColumn.Alignment = HorizontalAlignment.Right;
            tabStyle.GridColumnStyles.Add(aColumnEditColumn);

            aColumnEditColumn = new DataGridEditColumn(CN.ReadOnly, "Y");
            aColumnEditColumn.MappingName = CN.Reason;
            aColumnEditColumn.HeaderText = "";
            aColumnEditColumn.Width = 0;
            aColumnEditColumn.ReadOnly = false;
            tabStyle.GridColumnStyles.Add(aColumnEditColumn);

            //dgEmployee2.TableStyles.Add(tabStyle);

        }

        public void Setup(List<CashierTotalsView> cashierTotals)
        {
            dgViewTotals.DataSource = cashierTotals;
        }

        private void SetupDGVs()
        {
            dgViewTotals.ColumnStyleInit();
            dgViewTotals.ColumnStylePreLoad("Codedescript", "Pay Method", 50, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft, readOnly: true);
            dgViewTotals.ColumnStylePreLoad("UserAmounts", null, 135, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft, "n2");
        }

        private void DGVEnableColumns(bool show)
        {
            if (show)
            {
                List<CashierTotalsView> cashierTotals = (List<CashierTotalsView>)dgViewTotals.DataSource;

                dgViewTotals.DataSource = cashierTotals;
                dgViewTotals.ColumnStyleInit();
                dgViewTotals.ColumnStylePreLoad("Codedescript", "Pay Method", 80, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft, readOnly: true);
                dgViewTotals.ColumnStylePreLoad("UserAmounts", null, 90, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft, "n2", readOnly: false);
                dgViewTotals.ColumnStylePreLoad("NetRecieptsAdj", "NetReciepts", 90, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft, "n2", readOnly: true);
                dgViewTotals.ColumnStylePreLoad("Difference", null, 90, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft, "n2", readOnly: true);
                dgViewTotals.ColumnStylePreLoad("Comments", null, 222, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft, readOnly: false);
                dgViewTotals.ColumnStylePreLoad("Payments", null, 90, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft, "n2", readOnly: true);
                dgViewTotals.ColumnStylePreLoad("Corrections", null, 90, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft, "n2", readOnly: true);
                dgViewTotals.ColumnStylePreLoad("Refunds", null, 90, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft, "n2", readOnly: true);
                dgViewTotals.ColumnStylePreLoad("PettyCash", null, 90, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft, "n2", readOnly: true);
                dgViewTotals.ColumnStylePreLoad("Remittance", null, 90, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft, "n2", readOnly: true);
                dgViewTotals.ColumnStylePreLoad("Allocation", null, 90, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft, "n2", readOnly: true);
                dgViewTotals.ColumnStylePreLoad("Disbursement", null, 90, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft, "n2", readOnly: true);

            }
            txtTotalDifference.Visible = lDifferences.Visible = show;
            txtSystemTotal.Visible = lSystemTotal.Visible = show;
            txtDepositTotal.Visible = lDepositTotal.Visible = show;


            //dgViewTotals.ColumnStylePreLoad("NetReceipts", null, 135, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft);

        }

        /// <summary>
        /// This handler is basically here to recalculate the difference between
        /// what the user has entered and what the system thinks they should have
        /// entered. A difference figure is calculated for each payment method here.
        /// We also validate that they have entered something meaningful. Payment 
        /// methods may or may not allow the entry of negative values according to 
        /// the reference field in the code table (it is feasible that someone may
        /// have done nothing that day except issue refunds by cheque and 
        /// therefore they would need to enter a negative value).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnColumnChange(object sender, System.Data.DataColumnChangeEventArgs e)
        {
            try
            {
                Wait();
                decimal userValue = 0;

                e.ProposedValue = StripCurrency(e.ProposedValue.ToString());
                if (e.Column.ColumnName == CN.UserValue)
                {
                    /* make sure it's numeric */
                    if (!IsStrictNumeric((string)e.ProposedValue) ||
                        (!Convert.ToBoolean(Convert.ToInt16(e.Row[CN.Reference])) && Convert.ToDecimal(e.ProposedValue) < 0))
                        e.ProposedValue = (0).ToString(DecimalPlaces);
                    else
                    {
                        userValue = Convert.ToDecimal(e.ProposedValue);
                        e.ProposedValue = Convert.ToDecimal(e.ProposedValue).ToString(DecimalPlaces);
                    }
                    decimal diff = userValue - (Convert.ToDecimal(StripCurrency((string)e.Row[CN.SystemValue])) - Convert.ToDecimal(StripCurrency((string)e.Row[CN.Deposit])));
                    //     ((DataView)dgEmployee2.DataSource).Table.Columns[CN.Difference].ReadOnly = false;
                    e.Row[CN.Difference] = diff.ToString(DecimalPlaces);
                    //   ((DataView)dgEmployee2.DataSource).Table.Columns[CN.Difference].ReadOnly = true;

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

        /// <summary>
        /// Similar to the above. This event fires after the column has been changed
        /// and is used to recalculate the totals shown at the bottom of the 
        /// datagrid. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnColumnChanged(object sender, System.Data.DataColumnChangeEventArgs e)
        {
            try
            {
                Wait();
                if (e.Column.ColumnName == CN.UserValue)
                {
                    UpdateCashierTotals();
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

        /// <summary>
        /// Recalculates totals for system values, user values, deposits and differences.
        /// Overloaded for optional zeroTotals parameter.
        /// </summary>
        /// <param name="lastCheck"></param>
        /// <returns></returns>
        private bool UpdateCashierTotals()
        {
            var totals = new CashierTotal((List<CashierTotalsView>)dgViewTotals.DataSource, exchangeRates);
            txtUserTotal.Text = totals.UserTotal.ToString(DecimalPlaces);
            txtSystemTotal.Text = totals.SystemTotal.ToString(DecimalPlaces);
            txtTotalDifference.Text = totals.DifferenceTotal.ToString(DecimalPlaces);
            txtDepositTotal.Text = totals.DifferenceTotal.ToString(DecimalPlaces);
            return totals.Difference;
        }

        //private bool UpdateCashierTotals(bool lastCheck, out bool zeroTotals)
        //{
        //    decimal userTotal = 0;
        //    decimal systemTotal = 0;
        //    decimal difference = 0;
        //    decimal deposit = 0;
        //    bool anyDifferences = false;
        //    diffString = "";
        //    zeroTotals = true;


        //    //  DataView dv = (DataView)dgEmployee2.DataSource;

        //    var cashierTotals = new CashierTotal((List<CashierTotalsView>)dgViewTotals.DataSource);

        //    foreach (CashierTotalsView row in cashierTotals)
        //    {
        //        userTotal += Convert.ToDecimal(cashierTotals);
        //        systemTotal += Convert.ToDecimal(row.NetReceipts);
        //        difference += Convert.ToDecimal(row.Difference);

        //        if (Convert.ToBoolean(row.IncludeInCashierTotals))
        //            deposit += Convert.ToDecimal(row.Allocation) + Convert.ToDecimal(row.PettyCash) + Convert.ToDecimal(row.Remittance) + Convert.ToDecimal(row.Disbursement);

        //        zeroTotals = (zeroTotals && userTotal == 0 && systemTotal == 0 && difference == 0 && deposit == 0);

        //        if (Convert.ToDecimal(row.Difference) != 0)
        //        {
        //            anyDifferences = true;

        //            if (lastCheck)
        //            {
        //                //IP - 07/12/11 - CR1234
        //                if (!Convert.ToBoolean(row.AllowOS)) //If a Shortage/Overage is not allowed for a Payment Method
        //                {
        //                    diffString += row.codedescript + " :Shortage/Overage not allowed" + Environment.NewLine;

        //                    row.Comments = "Shortage/overage not allowed";

        //                    shortageOverageAllowed = false; //Prevent from saving
        //                }
        //                else
        //                {
        //                    diffString += row.codedescript + Environment.NewLine;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (Convert.ToString(row.Comments) == "Shortage/overage not allowed")
        //            {
        //                row.Comments = string.Empty;
        //            }
        //        }
        //    }


        //    txtUserTotal.Text = userTotal.ToString(DecimalPlaces);
        //    txtSystemTotal.Text = systemTotal.ToString(DecimalPlaces);
        //    txtTotalDifference.Text = difference.ToString(DecimalPlaces);
        //    txtDepositTotal.Text = deposit.ToString(DecimalPlaces);

        //    return anyDifferences;
        //}

        private void tcEmployee_SelectionChanged(object sender, System.EventArgs e)
        {
            // Hide the reverse button
            this.btnSave.Visible = true;
            this.btnReverse.Visible = false;

            if (!loading)
            {
                int index = 0;
                switch (tcEmployee.SelectedIndex)
                {
                    case 0:    /* totals page */
                        lEmployeeFrom.Visible = true;
                        lEmployeeTo.Visible = true;
                        dtEmployeeFrom.Visible = true;
                        dtEmployeeTo.Visible = true;
                        dtEmployeeFrom.Enabled = false;
                        dtEmployeeTo.Enabled = false;
                        btnSave.Visible = true;
                        dtEmployeeTo.CustomFormat = "ddd dd MMM yyyy HH:mm";
                        lEmployeeTo.Text = "To:";
                        dtEmployeeTo.Location = new Point(376, dtEmployeeTo.Location.Y);
                        lEmployeeTo.Location = new Point(376, lEmployeeTo.Location.Y);
                        drpCashier.Enabled = false;
                        index = drpCashier.FindStringExact(Credential.Name);
                        if (index >= 0) drpCashier.SelectedIndex = index;
                        btnEmployeePrint.Visible = true;
                        break;

                    case 1:    /* cheques page */
                        lEmployeeFrom.Visible = false;
                        lEmployeeTo.Visible = true;
                        dtEmployeeFrom.Visible = false;
                        dtEmployeeTo.Visible = true;
                        dtEmployeeTo.Enabled = false;
                        btnSave.Visible = false;
                        dtEmployeeTo.CustomFormat = "ddd dd MMM yyyy";
                        lEmployeeTo.Text = "Date";
                        dtEmployeeTo.Location = new Point(208, dtEmployeeTo.Location.Y);
                        lEmployeeTo.Location = new Point(208, lEmployeeTo.Location.Y);
                        drpCashier.Enabled = false;
                        index = drpCashier.FindStringExact(Credential.Name);
                        if (index >= 0) drpCashier.SelectedIndex = index;
                        btnEmployeePrint.Visible = true;
                        break;

                    case 2:    /* history page */
                        lEmployeeFrom.Visible = false;
                        lEmployeeTo.Visible = false;
                        dtEmployeeFrom.Visible = false;
                        dtEmployeeTo.Visible = false;
                        btnSave.Visible = false;
                        btnEmployeePrint.Visible = true;
                        drpCashier.Enabled = true;
                        break;

                    //case 3:    /* Reversal page */
                    //	lEmployeeFrom.Visible = true;
                    //	lEmployeeTo.Visible = true;
                    //	dtEmployeeFrom.Visible = true;
                    //	dtEmployeeTo.Visible = true;
                    //	dtEmployeeFrom.Enabled = false;
                    //	dtEmployeeTo.Enabled = false;
                    //	dtEmployeeTo.CustomFormat = "ddd dd MMM yyyy HH:mm";
                    //	lEmployeeTo.Text = "To:";
                    //	dtEmployeeTo.Location = new Point(376,dtEmployeeTo.Location.Y);
                    //	lEmployeeTo.Location = new Point(376,lEmployeeTo.Location.Y);
                    //	drpCashier.Enabled = true;
                    //	btnEmployeePrint.Visible = true;
                    // Display the reverse button
                    //	this.btnSave.Visible = false;
                    //	this.btnReverse.Visible = true;
                    //	this.btnReverse.Enabled = false;
                    //	break;
                    default:
                        break;
                }
                dgHistory.DataSource = null;
                dgReversal.DataSource = null;
            }
        }


        private void btnEmployeePrint_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                //AxWebBrowser[] browsers = CreateBrowserArray(1);

                if (tcEmployee.SelectedTab == tpHistory)
                {
                    if (dgHistory.CurrentRowIndex >= 0)
                    {
                        //PrintCashierTotals(browsers[0],
                        //    false,
                        //    Convert.ToInt16(Config.BranchCode),
                        //    (int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo],
                        //    drpCashier.Text,
                        //    (DateTime)((DataView)dgHistory.DataSource)[dgHistory.CurrentRowIndex][CN.DateFrom],
                        //    (DateTime)((DataView)dgHistory.DataSource)[dgHistory.CurrentRowIndex][CN.DateTo],
                        //    false,
                        //    (int)((DataView)dgHistory.DataSource)[dgHistory.CurrentRowIndex][CN.ID]);

                        //IP - 09/12/11 - CR1234
                        PrintCashierTotals(drpCashier.Text, (int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo], (DateTime)((DataView)dgHistory.DataSource)[dgHistory.CurrentRowIndex][CN.DateFrom], (DateTime)((DataView)dgHistory.DataSource)[dgHistory.CurrentRowIndex][CN.DateTo]);
                    }
                }
                else
                {
                    btnSave_Click(this, null);

                    /*PrintCashierTotals(browsers[0],
                                        false,
                                        Convert.ToInt16(Config.BranchCode),
                                        (int)txtCashier.Tag,
                                        txtCashier.Text,
                                        dtEmployeeFrom.Value,
                                        dtEmployeeTo.Value,
                                        tcEmployee.SelectedTab==tpCheques );*/
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnTotal_Click");
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// Starts the calculator.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalc_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                Process.Start("calc.exe");
            }
            catch (Exception)
            {
                ShowInfo("M_CANTOPENCALC");
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// This method can be used to either show or hide the extra columns which 
        /// are initially hidden. It is used when a cashier has entered details
        /// which do not match what the system is expecting. In this case, authorisation 
        /// will be required and then the user will be able to look and see exactly 
        /// what it is that doesn't match up.
        /// </summary>
        /// <param name="show"></param>
        //private void ShowBreakDown(bool show)
        //{
        //    if (dgEmployee2.TableStyles.Count > 0)
        //    {
        //        DataGridTableStyle tabStyle = dgEmployee2.TableStyles[0];

        //        if (show)
        //        {

        //            tabStyle.GridColumnStyles[CN.SystemValue].Width = 80;
        //            tabStyle.GridColumnStyles[CN.SystemValue].HeaderText = GetResource("T_SYSTEMVALUE");

        //            tabStyle.GridColumnStyles[CN.Difference].Width = 80;
        //            tabStyle.GridColumnStyles[CN.Difference].HeaderText = GetResource("T_DIFFERENCE");

        //            tabStyle.GridColumnStyles[CN.Deposit].Width = 80;
        //            tabStyle.GridColumnStyles[CN.Deposit].HeaderText = GetResource("T_DEPOSIT");

        //            tabStyle.GridColumnStyles[CN.Reason].Width = 140;
        //            tabStyle.GridColumnStyles[CN.Reason].HeaderText = GetResource("T_REASON");

        //            tabStyle.GridColumnStyles[CN.CodeDescription].Width = 120;
        //        }
        //        else
        //        {

        //            tabStyle.GridColumnStyles[CN.SystemValue].Width = 0;
        //            tabStyle.GridColumnStyles[CN.SystemValue].HeaderText = "";

        //            tabStyle.GridColumnStyles[CN.Difference].Width = 0;
        //            tabStyle.GridColumnStyles[CN.Difference].HeaderText = "";

        //            tabStyle.GridColumnStyles[CN.Deposit].Width = 0;
        //            tabStyle.GridColumnStyles[CN.Deposit].HeaderText = "";

        //            tabStyle.GridColumnStyles[CN.Reason].Width = 0;
        //            tabStyle.GridColumnStyles[CN.Reason].HeaderText = "";

        //            tabStyle.GridColumnStyles[CN.CodeDescription].Width = 150;
        //        }
        //    }
        //    txtTotalDifference.Visible = lDifferences.Visible = show;
        //    txtSystemTotal.Visible = lSystemTotal.Visible = show;
        //    txtDepositTotal.Visible = lDepositTotal.Visible = show;
        //}


        /// <summary>
        /// Run this when the user attempts to save the totals they have entered.
        /// Check for safe deposits (see comment below).
        /// Check to see if there are any differences between system expectations 
        /// and user values. If there are, prompt for authorisation and then 
        /// display the other columns. The user can then either revise what they 
        /// have entered or write a comment in next to any payment methods 
        /// which do not tally. When they save again, this method will be run 
        /// again, but it will know that they have already seen the other 
        /// columns so it won't warn again. This time, if there are differences it
        /// will just make sure that each one has a comment next to it. It will
        /// then display a message telling the user what overrages or shortages will
        /// be posted, if any, then save the totals and print a summary.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                btnSave.Enabled = false;                    //IP - 28/09/12 - #10400 - LW74981

                UpdateCashierTotals();
                Wait();
                bool valid = true;
                decimal safeDeposits = 0;
                shortageOverageAllowed = true;                    //IP - 07/12/11 - CR1234
                ((MainForm)this.FormRoot).statusBar1.Text = string.Empty;

                /* Some countries (Jamaica) have an operational requirement to
                 * prevent cashier totals being performed if there are 
                 * outstanding deposits that the cashier has made to the safe
                 * during their current session */
                if (!(bool)Country[CountryParameterNames.AllowSafeDeposits])
                {
                    PaymentManager.GetOutstandingSafeDeposits((int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo], Convert.ToInt16(Config.BranchCode), out safeDeposits, out Error);
                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                        valid = false;
                    }
                    else if (safeDeposits > 0)
                    {
                        ShowInfo("M_OUTSTANDINGSAFEDEPOSITS", new object[] { safeDeposits.ToString(DecimalPlaces) });
                        valid = false;
                    }
                }

                if (valid && dgViewTotals.DataSource != null)
                {
                    bool save = false;
                    var totals = new CashierTotal((List<CashierTotalsView>)dgViewTotals.DataSource, exchangeRates);

                    // are there any differences between the User entered values 
                    // and the system values ?

                    if (totals.Difference && _authorisedBy == 0)
                    {
                        btnSave.Enabled = true;                    //IP - 28/09/12 - #10400 - LW74981

                        if (DialogResult.OK == ShowInfo("M_CASHIERTOTALSPAYMETHOD", new[] { totals.Comments }, MessageBoxButtons.OKCancel))
                        {
                            AuthorisePrompt ap = new AuthorisePrompt(this, lAuthorise, GetResource("M_CASHIERTOTALAUTH"));
                            ap.ShowDialog();
                            if (ap.Authorised)
                            {
                                _authorisedBy = ap.AuthorisedBy;
                                DGVEnableColumns(true);
                                //  ShowBreakDown(true);
                            }

                            if (lDeposit.Enabled)
                            {
                                btnInclude.Visible = true;
                                btnExclude.Visible = true;
                            }
                        }
                    }
                    else
                    {

                        if (totals.OSFailure)            //IP - 07/12/11 - CR1234
                        {
                            save = false;
                            ((MainForm)this.FormRoot).statusBar1.Text = "There are Shortages/Overages on Payment Methods which are not allowed. Please address these before saving";
                        }
                        else
                        {
                            save = true;
                        }
                        if (!totals.Difference && !totals.Zero) ((MainForm)FormRoot).statusBar1.Text = GetResource("M_CASHIERTOTALSMATCH");
                    }

                    if (save && totals.Zero)
                    {
                        // FR66560 No totals to save
                        save = false;
                        ShowInfo("M_CASHIERTOTALSEMPTY");
                    }

                    if (save)
                    {
                        /* make sure they've entered a reason for each difference */
                        bool noReason = false;
                        decimal difference = 0.0M;
                        decimal shortageTotal = 0.0M;
                        decimal overageTotal = 0.0M;

                        List<CashierTotalsView> cashierTotals = (List<CashierTotalsView>)dgViewTotals.DataSource;
                        List<CashierTotalsBreakdown> cashierTotalsbreak = new List<CashierTotalsBreakdown>();

                        foreach (CashierTotalsView r in cashierTotals)
                        {
                            difference = Convert.ToDecimal(r.Difference);
                            if (difference != 0 && (r.Comments == null || r.Comments.Trim().Length == 0))
                            {
                                noReason = true;
                                break;
                            }
                            else if (difference < 0 && r.paymethod != PayMethod.StoreCard)
                                if (r.paymethod.ToString() != PayMethod.StoreCard.ToString())
                                    shortageTotal += PaymentManager.CalcExchangeAmount(System.Convert.ToInt16(r.paymethod), 0, difference, out Error);
                                else if (difference > 0 && r.paymethod != PayMethod.StoreCard)
                                    if (r.paymethod != PayMethod.StoreCard)
                                        overageTotal += PaymentManager.CalcExchangeAmount(Convert.ToInt16(r.paymethod), 0, difference, out Error);

                            CashierTotalsBreakdown cb = new CashierTotalsBreakdown
                            {
                                deposit = Convert.ToDecimal(r.Disbursement) + Convert.ToDecimal(r.Allocation) + Convert.ToDecimal(r.Remittance),
                                difference = Convert.ToDecimal(r.Difference),
                                paymethod = Convert.ToString(r.paymethod),
                                reason = Convert.ToString(r.Comments),
                                systemtotal = Convert.ToDecimal(r.NetReceipts),
                                usertotal = Convert.ToDecimal(r.UserAmounts),
                                Allocations = r.Allocation,
                                Corrections = r.Corrections,
                                Disbursements = r.Disbursement,
                                Payments = r.Payments,
                                PettyCash = r.PettyCash,
                                Refunds = r.Refunds,
                                Remittances = r.Remittance

                            };
                            cashierTotalsbreak.Add(cb);
                        }

                        if (noReason)
                        {
                            ShowInfo("M_ENTERREASON");
                            btnSave.Enabled = true;                    //IP - 28/09/12 - #10400 - LW74981
                        }
                        else
                        {
                            // Tell the user what will happen if there is an overage or a shortage
                            bool canReverse = false;

                            if (overageTotal > 0)
                            {
                                string acctno = AccountManager.GetOveragesAccount(Convert.ToInt16(Config.BranchCode), out Error);
                                if (Error.Length > 0)
                                {
                                    ShowError(Error);
                                    btnSave.Enabled = true;                    //IP - 28/09/12 - #10400 - LW74981
                                    return;
                                }
                                else
                                {
                                    // CR695 awaiting sign off
                                    // CR695 Allow reversal of overage when equal to last shortage
                                    //canReverse = AccountManager.CanReverseOverage((int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo], overage, out Error);
                                    //if(Error.Length>0)
                                    //{
                                    //	ShowError(Error);
                                    //	return;
                                    //}
                                    //else if (canReverse)
                                    //	canReverse = (DialogResult.Yes == ShowInfo("M_OVERAGEREVERSE", new object[]{overage.ToString(DecimalPlaces), acctno}, MessageBoxButtons.YesNo));
                                    //else
                                    ShowInfo("M_OVERAGE", new object[] { overageTotal.ToString(DecimalPlaces), acctno });
                                }
                            }

                            if (shortageTotal < 0)
                            {
                                string acctno = AccountManager.GetReceivableAccount((int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo], out Error);
                                if (Error.Length > 0)
                                {
                                    ShowError(Error);
                                    btnSave.Enabled = true;                    //IP - 28/09/12 - #10400 - LW74981
                                    return;
                                }
                                else
                                {
                                    ShowInfo("M_SHORTAGE", new object[] { shortageTotal.ToString(DecimalPlaces), acctno });
                                }
                            }

                            //DataSet breakdownSet = new DataSet();
                            //breakdownSet.Tables.Add(((DataView)dgEmployee2.DataSource).Table.Copy());


                            Client.Call(new CashierTotalsSaveRequest
                            {

                                datefrom = dtEmployeeFrom.Value,
                                dateto = dtEmployeeTo.Value,
                                authorisdedby = _authorisedBy,
                                canReverse = canReverse,
                                UserTotal = Convert.ToDecimal(StripCurrency(txtUserTotal.Text)),
                                SystemTotal = Convert.ToDecimal(StripCurrency(txtSystemTotal.Text)),
                                TotalDifference = Convert.ToDecimal(StripCurrency(txtTotalDifference.Text)),
                                DepositTotal = Convert.ToDecimal(StripCurrency(txtDepositTotal.Text)),
                                branch = Convert.ToInt16(Config.BranchCode),
                                employee = (int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo],
                                breakdown = cashierTotalsbreak
                            },
                               response =>
                               {
                                   if (response.result == "OK")
                                   {
                                       MainForm.Current.ShowStatus("Totals Saved.");
                                       if (btnEmployeePrint.Visible)
                                       {
                                           PrintCashierTotals(drpCashier.Text, (int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo],
                                               this.dtEmployeeFrom.Value, this.dtEmployeeTo.Value);


                                       }

                                       this.btnClear_Click(null, null);
                                       /*									
                                           LoadLastAudit();
                                           */
                                       dgViewTotals.DataSource = null;
                                       ((MainForm)FormRoot).CheckCashierDeposits();
                                   }
                                   else
                                   {
                                       MessageBox.Show("Cashier Totals not saved please contact support or retry");
                                   }
                               }, this);
                        }


                        //PaymentManager.SaveCashierTotal(this.dtEmployeeFrom.Value,
                        //    this.dtEmployeeTo.Value,
                        //    0,
                        //    _authorisedBy,
                        //    canReverse,
                        //    Convert.ToDecimal(StripCurrency(txtUserTotal.Text)),
                        //    Convert.ToDecimal(StripCurrency(txtSystemTotal.Text)),
                        //    Convert.ToDecimal(StripCurrency(txtTotalDifference.Text)),
                        //    Convert.ToDecimal(StripCurrency(txtDepositTotal.Text)),
                        //    Convert.ToInt16(Config.BranchCode),
                        //    (int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo],
                        //    Config.CountryCode,
                        //    breakdownSet,
                        //    out Error);

                        //if (Error.Length > 0)
                        //    ShowError(Error);
                        //else
                        //{

                    }
                    else
                    {
                        btnSave.Enabled = true;                    //IP - 28/09/12 - #10400 - LW74981
                    }
                }

                if (!valid)      //IP - 28/09/12 - #10400 - LW74981
                {
                    btnSave.Enabled = true;
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

        private void dgHistory_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Wait();
                int index = dgHistory.CurrentRowIndex;

                if (index >= 0)
                {
                    dgHistory.Select(index);

                    if (e.Button == MouseButtons.Right)
                    {
                        /* popup a re-print option */
                        DataGrid ctl = (DataGrid)sender;

                        MenuCommand m1 = new MenuCommand(GetResource("P_REPRINT"));
                        m1.Click += new System.EventHandler(this.btnEmployeePrint_Click);

                        PopupMenu popup = new PopupMenu();
                        popup.Animate = Animate.Yes;
                        popup.AnimateStyle = Animation.SlideHorVerPositive;
                        popup.MenuCommands.Add(m1);
                        MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnTotal_Click");
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// retrieves the date when the cashier last ran cashier totals which 
        /// is the date from which cashier totals are measured. 
        /// </summary>
        private void LoadLastAudit()
        {
            DateTime dt = DateTimePicker.MinDateTime;
            dt = PaymentManager.GetDateLastAudit((int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo],
                out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                dtEmployeeFrom.Value = dt > DateTimePicker.MinDateTime && dt < DateTimePicker.MaxDateTime ? dt : DateTimePicker.MinDateTime;
            }
        }

        /// <summary>
        /// Find out if the logged in cashier has any outstanding safe deposits in 
        /// the load event because some countries do not even want to be able to
        /// access to the screen until these safe deposits have been reversed. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CashierTotals_Load(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                int index = drpCashier.FindStringExact(Credential.Name);
                if (index >= 0)
                    drpCashier.SelectedIndex = index;
                else
                {
                    DataTable dt = ((DataView)drpCashier.DataSource).Table;
                    DataRow r = dt.NewRow();
                    r[CN.EmployeeName] = Credential.Name;
                    r[CN.EmployeeNo] = Credential.UserId;
                    r[CN.BranchNo] = Convert.ToInt16(Config.BranchCode);
                    dt.Rows.Add(r);
                    drpCashier.DataSource = null;
                    drpCashier.DataSource = dt.DefaultView;
                    drpCashier.DisplayMember = CN.EmployeeName.ToLower();

                    index = drpCashier.FindStringExact(Credential.Name);
                    if (index >= 0)
                        drpCashier.SelectedIndex = index;
                }

                /* Some countries (Jamaica) have an operational requirement to
                 * prevent cashier totals being performed if there are 
                 * outstanding deposits that the cashier has made to the safe
                 * during their current session */
                decimal safeDeposits = 0;
                if (!(bool)Country[CountryParameterNames.AllowSafeDeposits])
                {
                    PaymentManager.GetOutstandingSafeDeposits((int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo], Convert.ToInt16(Config.BranchCode), out safeDeposits, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        if (safeDeposits > 0)
                        {
                            ShowInfo("M_OUTSTANDINGSAFEDEPOSITS", new object[] { safeDeposits.ToString(DecimalPlaces) });
                            //CloseTab();               //#10400 - LW74981 - found error whilst fixing this issue.

                            tabClosed = true;            //#10400 - LW74981 - found error whilst fixing this issue.
                        }
                    }
                }

                LoadLastAudit();

                /* remove the reversal tab, this CR has been abandoned (for now) */

                if (tabClosed != true)                   //#10400 - LW74981 - found error whilst fixing this issue.
                {
                    tcEmployee.TabPages.Remove(tpReversal);
                }

                btnEmployeePrint.Enabled = Credential.HasPermission(CosacsPermissionEnum.CashierTotalsEmployeePrint);
                btnBranchPrint.Enabled = Credential.HasPermission(CosacsPermissionEnum.CashierTotalsBranchPrint);
            }
            catch (Exception ex)
            {
                Catch(ex, "CashierTotals_Load");
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// this ensures that wherever the user clicks on the datagrid the 
        /// only editable field will be highlighted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void dgEmployee_CurrentCellChanged(object sender, System.EventArgs e)
        //{
        //    if (dgEmployee2.TableStyles[0].GridColumnStyles[dgEmployee2.CurrentCell.ColumnNumber].HeaderText != GetResource("T_USERVALUE"))
        //        dgEmployee2.CurrentCell = new DataGridCell(dgEmployee2.CurrentCell.RowNumber, dgEmployee2.CurrentCell.ColumnNumber + 1);
        //}

        /// <summary>
        /// Pop up a dialog to list the cashier totals broken down by pay method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgHistory_DoubleClick(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                if (((DataView)dgHistory.DataSource).Count > 0)
                {
                    int empeeno = (int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo];
                    int totalid = (int)((DataView)dgHistory.DataSource)[dgHistory.CurrentRowIndex][CN.ID];

                    CashierTotalsBreakdownS ctb = new CashierTotalsBreakdownS(totalid);
                    ctb.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "dgHistory_DoubleClick");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnSummaryLoad_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                Summary = PaymentManager.GetCashierTotalsSummary((short)((DataRowView)drpSummaryBranch.SelectedItem)[CN.BranchNo],
                    dtSummaryFrom.Value,
                    dtSummaryTo.Value,
                    out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    DataView summaryView = Summary.Tables[TN.CashierTotals].DefaultView;

                    /* make sure we have a row for every pay method */
                    foreach (DataRow r in ((DataTable)StaticData.Tables[TN.PayMethod]).Rows)
                    {
                        if ((string)r[CN.Code] != "0")		/*ignore "Not Applicable" */
                        {
                            summaryView.RowFilter = CN.CodeDescription + " = '" + (string)r[CN.CodeDescription] + "'";
                            if (summaryView.Count == 0)
                            {
                                DataRow newRow = summaryView.Table.NewRow();
                                newRow[CN.Code] = 0;
                                newRow[CN.CodeDescription] = r[CN.CodeDescription];
                                newRow[CN.SystemTotal] = 0;
                                newRow[CN.UserTotal] = 0;
                                newRow[CN.Deposit] = 0;
                                newRow[CN.Difference] = 0;
                                newRow[CN.SecuritisedValue] = 0;
                                summaryView.Table.Rows.Add(newRow);
                            }
                        }
                    }
                    summaryView.RowFilter = "";

                    dgSummary.DataSource = summaryView;
                    dgSummary.TableStyles.Clear();

                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                    tabStyle.MappingName = summaryView.Table.TableName;

                    AddColumnStyle(CN.Code, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.CodeDescription, tabStyle, 120, true, GetResource("T_PAYMETHOD"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.SystemTotal, tabStyle, 100, true, GetResource("T_SYSTEMVALUE"), DecimalPlaces, HorizontalAlignment.Right);
                    AddColumnStyle(CN.UserTotal, tabStyle, 100, true, GetResource("T_USERVALUE"), DecimalPlaces, HorizontalAlignment.Right);
                    AddColumnStyle(CN.Difference, tabStyle, 100, true, GetResource("T_DIFFERENCE"), DecimalPlaces, HorizontalAlignment.Right);
                    AddColumnStyle(CN.Deposit, tabStyle, 100, true, GetResource("T_DEPOSIT"), DecimalPlaces, HorizontalAlignment.Right);
                    AddColumnStyle(CN.SecuritisedValue, tabStyle, 100, true, GetResource("T_SECURITISEDVALUE"), DecimalPlaces, HorizontalAlignment.Right);
                    dgSummary.TableStyles.Add(tabStyle);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSummaryLoad_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnSummaryPrint_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                PrintCashierTotalsSummary(CreateBrowserArray(1)[0],
                    (short)((DataRowView)drpSummaryBranch.SelectedItem)[CN.BranchNo],
                    dtSummaryFrom.Value,
                    dtSummaryTo.Value);
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSummaryPrint_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void drpCashier_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            dgCheques.DataSource = null;
            //dgEmployee2.DataSource = null;
            dgViewTotals.DataSource = null;
            dgHistory.DataSource = null;
            dgReversal.DataSource = null;
            this.btnReverse.Enabled = false;

            DateTime serverTime = StaticDataManager.GetServerDateTime();
            dtEmployeeFrom.Value = serverTime;
            dtEmployeeTo.Value = serverTime;
            txtChequesTotal.Text = (0).ToString(DecimalPlaces);

            txtUserTotal.Text = (0).ToString(DecimalPlaces);
            txtSystemTotal.Text = (0).ToString(DecimalPlaces);
            txtTotalDifference.Text = (0).ToString(DecimalPlaces);
            txtDepositTotal.Text = (0).ToString(DecimalPlaces);

            txtSystemTotal.Visible = lSystemTotal.Visible = false;
            txtTotalDifference.Visible = lDifferences.Visible = false;

            if (dgViewTotals.DataSource != null)
                DGVEnableColumns(false);
            _authorisedBy = 0;

            ((MainForm)FormRoot).statusBar1.Text = "";
        }

        /// <summary>
        /// This will never run at the moment, it's on hold.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReverse_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                PaymentManager.SaveCashierTotalsReversal(
                    Config.CountryCode,
                    Convert.ToInt16(Config.BranchCode),
                    (int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo],
                    out Error);

                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    this.btnReverse.Enabled = false;
                    dgReversal.DataSource = null;

                    ((MainForm)FormRoot).CheckCashierDeposits();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnReverse_Click");
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// This will save the current results grid to a csv file of 
        /// the users chossing and then attempt to open that csv file in excel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExcel_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                /* save the current data grid contents to a CSV */
                string comma = ",";
                string path = "";

                if (dgBranch.CurrentRowIndex >= 0)
                {
                    DataView dv = ((DataSet)dgBranch.DataSource).Tables[TN.CashierTotals].DefaultView;

                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
                    save.Title = "Save Query Results";
                    save.CreatePrompt = true;

                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        path = save.FileName;
                        FileInfo fi = new FileInfo(path);
                        FileStream fs = fi.OpenWrite();

                        string line = CN.EmployeeNo + comma +
                            CN.TransTypeCode + comma +
                            CN.CodeDescript + comma +
                            CN.TransValue + comma +
                            CN.DateTrans + Environment.NewLine + Environment.NewLine;
                        byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
                        fs.Write(blob, 0, blob.Length);

                        foreach (DataRowView row in dv)
                        {
                            line = row[CN.EmployeeNo] + comma +
                                row[CN.TransTypeCode] + comma +
                                row[CN.CodeDescript] + comma +
                                ((decimal)row[CN.TransValue]).ToString(DecimalPlaces).Replace(",", "") + comma +
                                Convert.ToString(row[CN.DateTrans]) + Environment.NewLine;

                            blob = System.Text.Encoding.UTF8.GetBytes(line);
                            fs.Write(blob, 0, blob.Length);
                        }
                        fs.Close();

                        /* attempt to launch Excel. May get a COMException if Excel is not 
                            * installed */
                        try
                        {
                            /* we have to use Reflection to call the Open method because 
                                * the methods have different argument lists for the 
                                * different versions of Excel - JJ */
                            object[] args = null;
                            Excel.Application excel = new Excel.Application();

                            if (excel.Version == "10.0")	/* Excel2002 */
                                args = new object[] { path, 0, true, 2, "", "", true, Excel.XlPlatform.xlWindows, comma, true, false, 0, true, false, false };
                            else
                                args = new object[] { path, 0, true, 2, "", "", true, Excel.XlPlatform.xlWindows, comma, true, false, 0, true };

                            /* Retrieve the Workbooks property */
                            object wbs = excel.GetType().InvokeMember("Workbooks", BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty, null, excel, new Object[] { });

                            /* call the Open method */
                            object wb = wbs.GetType().InvokeMember("Open", BindingFlags.Public | BindingFlags.InvokeMethod, null, wbs, args);

                            excel.Visible = true;
                        }
                        catch (COMException)
                        {
                            /*change back slashes to forward slashes so the path doesn't
                                * get split into multiple lines */
                            ShowInfo("M_EXCELNOTFOUND", new object[] { path.Replace("\\", "/") });
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

        private void btnInclude_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                PaymentManager.IncludeDeposits((int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo], 1, out Error);
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

        private void btnExclude_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                PaymentManager.IncludeDeposits((int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo], 0, out Error);
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

        private void menuFile_Click(object sender, System.EventArgs e)
        {

        }

        private void menuLaunchhelp_Click(object sender, System.EventArgs e)
        {
            CashierTotals_HelpRequested(this, null);
        }
        private void CashierTotals_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
        {
            string fileName = "Cashier_Totals_Screen.htm";
            LaunchHelp(fileName);
        }

        /// <summary>
        /// Send off a print job for branch level cashier totals.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBranchPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Wait();
                AxWebBrowser[] browsers = CreateBrowserArray(1);
                PrintCashierTotals(browsers[0],
                    false,
                    (short)((DataRowView)drpBranch.SelectedItem)[CN.BranchNo],
                    -1,		/* denotes "all staff" */
                    drpCashier.Text,
                    dtBranchFrom.Value,
                    dtBranchTo.Value,
                    false,
                    -1);
            }
            catch (Exception ex)
            {
                Catch(ex, "btnTotal_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void tcMain_Click(object sender, EventArgs e)
        {
            if (tpBranch.Selected && !Authenticated)
            {
                AuthoriseCheck Auth = new AuthoriseCheck(this.Name, "tpBranch");

                if (!Auth.ControlPermissionCheck(Credential.User).HasValue)
                {
                    tcMain.SelectedTab = tpEmployee;
                    Auth.ShowDialog();
                    if (Auth.IsAuthorised)
                    {
                        Authenticated = true;
                        tcMain.SelectedTab = tpBranch;
                    }
                }
            }
        }

        private void dgViewTotals_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Invalid Data Entered - Please enter numeric value");
        }

        //IP - 09/12/11 - CR1234
        private void LaunchWebBrowser(string url)
        {
            var browser = CreateBrowserArray(1);
            object x = "";
            browser[0].Navigate(Config.Url + url, ref x, ref x, ref x, ref x);
        }

        private void PrintCashierTotals(string empeeName, int empeeNo, DateTime datefrom, DateTime dateTo)
        {
            //IP - 09/12/11 - CR1234
            var machineName = System.Environment.MachineName;

            //Prepare the DateFrom into a format that can be passed into the URL for printing
            var dateFromYear = Convert.ToString(datefrom.Year);
            var dateFromMonth = datefrom.Month < 10 ? "0" + Convert.ToString(datefrom.Month) : Convert.ToString(datefrom.Month);
            var dateFromDay = datefrom.Day < 10 ? "0" + Convert.ToString(datefrom.Day) : Convert.ToString(datefrom.Day);
            var dateFromTime = Convert.ToString(datefrom.TimeOfDay);

            StringBuilder sbDateFrom = new StringBuilder();
            sbDateFrom.Append(dateFromYear);
            sbDateFrom.Append("-");
            sbDateFrom.Append(dateFromMonth);
            sbDateFrom.Append("-");
            sbDateFrom.Append(dateFromDay);
            sbDateFrom.Append("T");
            sbDateFrom.Append(dateFromTime);

            //Prepare the DateFrom into a format that can be passed into the URL for printing
            var dateToYear = Convert.ToString(dateTo.Year);
            var dateToMonth = dateTo.Month < 10 ? "0" + Convert.ToString(dateTo.Month) : Convert.ToString(dateTo.Month);
            var dateToDay = dateTo.Day < 10 ? "0" + Convert.ToString(dateTo.Day) : Convert.ToString(dateTo.Day);
            var dateToTime = Convert.ToString(dateTo.TimeOfDay);

            StringBuilder sbDateTo = new StringBuilder();
            sbDateTo.Append(dateToYear);
            sbDateTo.Append("-");
            sbDateTo.Append(dateToMonth);
            sbDateTo.Append("-");
            sbDateTo.Append(dateToDay);
            sbDateTo.Append("T");
            sbDateTo.Append(dateToTime);


            LaunchWebBrowser(String.Format("Cashier/DailyReconciliation?empeeName={0}&&empeeNo={1}&&branchNo={2}&&machineName={3}&&dateFrom={4}&&dateTo={5}", empeeName, Convert.ToString(empeeNo), Convert.ToInt16(Config.BranchCode), machineName, sbDateFrom, sbDateTo));
        }

        private void dgViewTotals_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            btnSave.Enabled = true;
            toolTip1.SetToolTip(btnSave, "Save");

            if (dgViewTotals[e.ColumnIndex, e.RowIndex].Value != null && dgViewTotals[e.ColumnIndex, e.RowIndex].Value.ToString().Length > 100 && _authorisedBy > 0)
            {
                MessageBox.Show("Comments too long - reduced to 100 characters");
                dgViewTotals[e.ColumnIndex, e.RowIndex].Value = dgViewTotals[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0, 100);
            }

            UpdateCashierTotals();

        }

        //private void CalculateDifferences()
        //{

        //    List<CashierTotalsView> cashierTotals = (List<CashierTotalsView>)dgViewTotals.DataSource;
        //    decimal usertotal = 0.0m; decimal difference = 0.0m;
        //    int counter = 0; bool error = false;
        //    foreach (CashierTotalsView total in cashierTotals)
        //    {
        //        total.Difference = Convert.ToDecimal(total.UserAmounts) - Convert.ToDecimal(total.NetReceipts);
        //        usertotal += Convert.ToDecimal(total.UserAmounts);
        //        difference += Convert.ToDecimal(total.Difference);
        //        if (_authorisedBy > 0 && total.Comments != null && total.Comments.Length > 100)
        //        {
        //            total.Comments = total.Comments.Substring(0, 99);
        //            error = true;

        //        }
        //        cashierTotals[counter].Difference = total.Difference;
        //        counter++;
        //    }
        //    dgViewTotals.DataSource = cashierTotals;
        //    dgViewTotals.Refresh();
        //    if (error)
        //        MessageBox.Show("Comments too long - reduced to 100 characters");

        //    txtUserTotal.Text = Convert.ToString(usertotal); txtTotalDifference.Text = Convert.ToString(difference);
        //}

        private void dgViewTotals_CellLeave(object sender, DataGridViewCellEventArgs e)
        {

            btnSave.Enabled = true;
            toolTip1.SetToolTip(btnSave, "Save");


            UpdateCashierTotals();

        }

        private void dgViewTotals_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            UpdateCashierTotals();
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {
        }


    }
}
