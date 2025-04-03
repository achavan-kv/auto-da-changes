using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
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
    /// Summary description for TransferTransaction.
    /// </summary>
    public class CashierDisbursments : CommonForm
    {
        //private Crownwood.Magic.Menus.MenuControl menuMain;
        private Control allowBranchFloats;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.GroupBox gbMain;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private new string Error = "";
        private DataTable dtDeposits = null;
        DataSet deposits = null;
        //private decimal UnexportedTotals = 0;
        //private decimal UnexportedDeposits = 0;
        new bool loaded = false;
        private int sourceIndex = -1;
        private int destIndex = -1;
        private System.Windows.Forms.ImageList menuIcons;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.ComponentModel.IContainer components;
        bool refresh = false;
        private Control authPettyCash = null;
        private DataView Deposits = null;
        private string noSafeFilter = CN.Code + " not = 'SAF'";
        private string excOtherCodes = CN.Code + " not = 'CLD'";                     //IP - 07/03/12 - #9741 - UAT119
        private System.Windows.Forms.ImageList imageList1;
        private Crownwood.Magic.Controls.TabControl tcMain;
        private Crownwood.Magic.Controls.TabPage tpEnter;
        private ComboBox drpPayMethod;
        private Label lPayMethod;
        private Label label10;
        private TextBox txtReference;
        private TextBox txtTotal;
        private TextBox txtEmpNumber;
        private TextBox txtValue;
        private Label lUserTotal;
        private Button btnEnter;
        private Label label2;
        private ComboBox drpCashier;
        private Label label6;
        private Label label1;
        private DataGrid dgDeposits;
        private Button btnSave;
        private ComboBox drpDeposit;
        private Label label5;
        private Crownwood.Magic.Controls.TabPage tpView;
        private CheckBox chxBranchFloats;
        private Button btnRefresh;
        private CheckBox chxPostedToFACT;
        private CheckBox chxWholeCountry;
        private ComboBox drpDeposit2;
        private Label label11;
        private Label label4;
        private TextBox txtEmpNumber2;
        private ComboBox drpCashier2;
        private Label label7;
        private DataGrid dgPrevious;
        private Button btnLoad;
        private DateTimePicker dtTo;
        private DateTimePicker dtFrom;
        private Label lTo;
        private Label lFrom;
        private Crownwood.Magic.Controls.TabPage tpUncashed;
        private Button btnLoadUncashed;
        private DataGrid dgUncashed;
        public Button btnExcel;
        private Label label3;
        private ComboBox drpPaymentMethod;
        private TextBox txtTot;
        private Label lblTot;
        private bool CashierTotalled = false;

        public CashierDisbursments(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
        }

        public CashierDisbursments(Form root, Form parent)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
            FormRoot = root;
            FormParent = parent;
            txtTotal.BackColor = SystemColors.Window;
            txtTotal.Text = (0).ToString(DecimalPlaces);
            DateTime serverTime = StaticDataManager.GetServerDateTime();
            dtTo.Value = serverTime.AddMinutes(20);
            dtFrom.Value = serverTime.AddDays(-1);

            toolTip1.SetToolTip(this.btnRefresh, GetResource("TT_REFRESH"));
            toolTip1.SetToolTip(this.dgPrevious, GetResource("TT_REVERSAL"));

            HashMenus();
            ApplyRoleRestrictions();

            /* the following lines of code appear unnecessary but they were the only way
             * I could find to correct a bizarre problem with the tab page selection - JJ */
            tcMain.SelectedTab = tpUncashed;
            //tcMain.SelectedTab = tpCashierTotals;
            tcMain.SelectedTab = tpView;
            tcMain.SelectedTab = tpEnter;
        }

        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":tpView"] = this.tpView;
            //dynamicMenus[this.Name+":tpCashierTotals"] = this.tpCashierTotals;
            dynamicMenus[this.Name + ":drpCashier"] = this.drpCashier;
            dynamicMenus[this.Name + ":txtEmpNumber"] = this.txtEmpNumber;
            dynamicMenus[this.Name + ":authPettyCash"] = this.authPettyCash;
            dynamicMenus[this.Name + ":allowBranchFloats"] = this.allowBranchFloats;
        }

        private void LoadStaticData()
        {
            try
            {
                Wait();

                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                if (StaticData.Tables[TN.Deposits] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Deposits, null));
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

                DataTable salesStaff = ((DataTable)StaticData.Tables[TN.SalesStaff]).Copy();
                DataRow r = salesStaff.NewRow();
                r[CN.EmployeeNo] = 0;
                r[CN.EmployeeName] = "All for this branch";
                r[CN.BranchNo] = Convert.ToInt16(Config.BranchCode);
                salesStaff.Rows.InsertAt(r, 0);
                //IP - 29/07/09 - UAT(642/749) - Added column EmployeeType
                //salesStaff.Columns.Add(new DataColumn(CN.EmployeeType, Type.GetType("System.String")));           //IP - 23/01/13 - #12086
                //foreach (DataRow rw in salesStaff.Rows)
                //    rw[CN.EmployeeType] = "C";

                DataTable salesStaff2 = ((DataTable)StaticData.Tables[TN.SalesStaff]).Copy();

                //salesStaff2.Columns.Add(new DataColumn(CN.EmployeeType, Type.GetType("System.String")));        //IP - 23/01/13 - #12086
                //foreach (DataRow rw in salesStaff2.Rows)
                //    rw[CN.EmployeeType] = "C";

                DataTable depositTypes = ((DataTable)StaticData.Tables[TN.Deposits]).Copy();
                r = depositTypes.NewRow();
                r[CN.Code] = "-1";
                r[CN.CodeDescription] = "All deposit types";
                r[CN.IsDeposit] = 0;
                depositTypes.Rows.InsertAt(r, 0);

                drpDeposit.DataSource = ((DataTable)StaticData.Tables[TN.Deposits]).DefaultView;
                drpDeposit.DisplayMember = CN.CodeDescription;

                ((DataView)drpDeposit.DataSource).RowFilter = noSafeFilter + " and " + excOtherCodes;           //IP - 07/03/12 - #9741 - UAT119

                drpDeposit2.DataSource = depositTypes;
                drpDeposit2.DisplayMember = CN.CodeDescription;

                drpCashier.DataSource = salesStaff2;
                drpCashier.DisplayMember = CN.EmployeeName.ToLower();

                drpCashier2.DataSource = salesStaff;
                drpCashier2.DisplayMember = CN.EmployeeName.ToLower();

                drpPayMethod.DataSource = (DataTable)StaticData.Tables[TN.PayMethod];
                drpPayMethod.DisplayMember = CN.CodeDescription;

                //IP - 15/12/11 - #8810 - CR1234 - 
                DataTable paymentMethods = ((DataTable)StaticData.Tables[TN.PayMethod]).Copy();
                DataRow row = paymentMethods.NewRow();
                row[CN.Code] = "-1";
                row[CN.CodeDescript] = "All";
                paymentMethods.Rows.InsertAt(row, 0);

                drpPaymentMethod.DataSource = paymentMethods;
                drpPaymentMethod.DisplayMember = CN.CodeDescription;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CashierDisbursments));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.allowBranchFloats = new System.Windows.Forms.Control();
            this.menuIcons = new System.Windows.Forms.ImageList(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.authPettyCash = new System.Windows.Forms.Control();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.gbMain = new System.Windows.Forms.GroupBox();
            this.tcMain = new Crownwood.Magic.Controls.TabControl();
            this.tpUncashed = new Crownwood.Magic.Controls.TabPage();
            this.btnLoadUncashed = new System.Windows.Forms.Button();
            this.dgUncashed = new System.Windows.Forms.DataGrid();
            this.tpEnter = new Crownwood.Magic.Controls.TabPage();
            this.drpPayMethod = new System.Windows.Forms.ComboBox();
            this.lPayMethod = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtReference = new System.Windows.Forms.TextBox();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.txtEmpNumber = new System.Windows.Forms.TextBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.lUserTotal = new System.Windows.Forms.Label();
            this.btnEnter = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.drpCashier = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dgDeposits = new System.Windows.Forms.DataGrid();
            this.btnSave = new System.Windows.Forms.Button();
            this.drpDeposit = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tpView = new Crownwood.Magic.Controls.TabPage();
            this.lblTot = new System.Windows.Forms.Label();
            this.txtTot = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.drpPaymentMethod = new System.Windows.Forms.ComboBox();
            this.btnExcel = new System.Windows.Forms.Button();
            this.chxBranchFloats = new System.Windows.Forms.CheckBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.chxPostedToFACT = new System.Windows.Forms.CheckBox();
            this.chxWholeCountry = new System.Windows.Forms.CheckBox();
            this.drpDeposit2 = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtEmpNumber2 = new System.Windows.Forms.TextBox();
            this.drpCashier2 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.dgPrevious = new System.Windows.Forms.DataGrid();
            this.btnLoad = new System.Windows.Forms.Button();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.lTo = new System.Windows.Forms.Label();
            this.lFrom = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.gbMain.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tpUncashed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgUncashed)).BeginInit();
            this.tpEnter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDeposits)).BeginInit();
            this.tpView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgPrevious)).BeginInit();
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
            // allowBranchFloats
            // 
            this.allowBranchFloats.Enabled = false;
            this.allowBranchFloats.Location = new System.Drawing.Point(0, 0);
            this.allowBranchFloats.Name = "allowBranchFloats";
            this.allowBranchFloats.Size = new System.Drawing.Size(0, 0);
            this.allowBranchFloats.TabIndex = 0;
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
            this.menuIcons.Images.SetKeyName(5, "");
            this.menuIcons.Images.SetKeyName(6, "");
            // 
            // authPettyCash
            // 
            this.authPettyCash.Enabled = false;
            this.authPettyCash.Location = new System.Drawing.Point(0, 0);
            this.authPettyCash.Name = "authPettyCash";
            this.authPettyCash.Size = new System.Drawing.Size(0, 0);
            this.authPettyCash.TabIndex = 0;
            this.authPettyCash.Visible = false;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            // 
            // gbMain
            // 
            this.gbMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbMain.BackColor = System.Drawing.SystemColors.Control;
            this.gbMain.Controls.Add(this.tcMain);
            this.gbMain.Controls.Add(this.btnExit);
            this.gbMain.Controls.Add(this.btnClear);
            this.gbMain.Location = new System.Drawing.Point(8, 0);
            this.gbMain.Name = "gbMain";
            this.gbMain.Size = new System.Drawing.Size(776, 472);
            this.gbMain.TabIndex = 0;
            this.gbMain.TabStop = false;
            // 
            // tcMain
            // 
            this.tcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tcMain.IDEPixelArea = true;
            this.tcMain.Location = new System.Drawing.Point(8, 48);
            this.tcMain.Name = "tcMain";
            this.tcMain.PositionTop = true;
            this.tcMain.SelectedIndex = 0;
            this.tcMain.SelectedTab = this.tpEnter;
            this.tcMain.Size = new System.Drawing.Size(760, 416);
            this.tcMain.TabIndex = 14;
            this.tcMain.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpEnter,
            this.tpView,
            this.tpUncashed});
            // 
            // tpUncashed
            // 
            this.tpUncashed.Controls.Add(this.btnLoadUncashed);
            this.tpUncashed.Controls.Add(this.dgUncashed);
            this.tpUncashed.Location = new System.Drawing.Point(0, 25);
            this.tpUncashed.Name = "tpUncashed";
            this.tpUncashed.Selected = false;
            this.tpUncashed.Size = new System.Drawing.Size(760, 391);
            this.tpUncashed.TabIndex = 5;
            this.tpUncashed.Title = "Outstanding Income";
            // 
            // btnLoadUncashed
            // 
            this.btnLoadUncashed.Location = new System.Drawing.Point(592, 69);
            this.btnLoadUncashed.Name = "btnLoadUncashed";
            this.btnLoadUncashed.Size = new System.Drawing.Size(40, 20);
            this.btnLoadUncashed.TabIndex = 25;
            this.btnLoadUncashed.Text = "Load";
            this.btnLoadUncashed.Click += new System.EventHandler(this.btnLoadUncashed_Click);
            // 
            // dgUncashed
            // 
            this.dgUncashed.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgUncashed.CaptionText = "Cashiers with outstanding income";
            this.dgUncashed.DataMember = "";
            this.dgUncashed.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgUncashed.Location = new System.Drawing.Point(136, 67);
            this.dgUncashed.Name = "dgUncashed";
            this.dgUncashed.ReadOnly = true;
            this.dgUncashed.Size = new System.Drawing.Size(496, 256);
            this.dgUncashed.TabIndex = 26;
            // 
            // tpEnter
            // 
            this.tpEnter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tpEnter.Controls.Add(this.drpPayMethod);
            this.tpEnter.Controls.Add(this.lPayMethod);
            this.tpEnter.Controls.Add(this.label10);
            this.tpEnter.Controls.Add(this.txtReference);
            this.tpEnter.Controls.Add(this.txtTotal);
            this.tpEnter.Controls.Add(this.txtEmpNumber);
            this.tpEnter.Controls.Add(this.txtValue);
            this.tpEnter.Controls.Add(this.lUserTotal);
            this.tpEnter.Controls.Add(this.btnEnter);
            this.tpEnter.Controls.Add(this.label2);
            this.tpEnter.Controls.Add(this.drpCashier);
            this.tpEnter.Controls.Add(this.label6);
            this.tpEnter.Controls.Add(this.label1);
            this.tpEnter.Controls.Add(this.dgDeposits);
            this.tpEnter.Controls.Add(this.btnSave);
            this.tpEnter.Controls.Add(this.drpDeposit);
            this.tpEnter.Controls.Add(this.label5);
            this.tpEnter.Location = new System.Drawing.Point(0, 25);
            this.tpEnter.Name = "tpEnter";
            this.tpEnter.Size = new System.Drawing.Size(760, 391);
            this.tpEnter.TabIndex = 0;
            this.tpEnter.Title = "Enter Disbursements";
            // 
            // drpPayMethod
            // 
            this.drpPayMethod.BackColor = System.Drawing.SystemColors.Window;
            this.drpPayMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPayMethod.Location = new System.Drawing.Point(360, 40);
            this.drpPayMethod.Name = "drpPayMethod";
            this.drpPayMethod.Size = new System.Drawing.Size(104, 23);
            this.drpPayMethod.TabIndex = 4;
            // 
            // lPayMethod
            // 
            this.lPayMethod.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lPayMethod.Location = new System.Drawing.Point(360, 24);
            this.lPayMethod.Name = "lPayMethod";
            this.lPayMethod.Size = new System.Drawing.Size(80, 16);
            this.lPayMethod.TabIndex = 58;
            this.lPayMethod.Text = "Pay Method";
            this.lPayMethod.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(224, 64);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(71, 16);
            this.label10.TabIndex = 56;
            this.label10.Text = "Reference";
            // 
            // txtReference
            // 
            this.txtReference.Location = new System.Drawing.Point(224, 80);
            this.txtReference.MaxLength = 30;
            this.txtReference.Name = "txtReference";
            this.txtReference.Size = new System.Drawing.Size(152, 23);
            this.txtReference.TabIndex = 9;
            this.txtReference.Leave += new System.EventHandler(this.txtReference_Leave);
            // 
            // txtTotal
            // 
            this.txtTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtTotal.Location = new System.Drawing.Point(632, 352);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(88, 23);
            this.txtTotal.TabIndex = 0;
            this.txtTotal.TabStop = false;
            // 
            // txtEmpNumber
            // 
            this.txtEmpNumber.Enabled = false;
            this.txtEmpNumber.Location = new System.Drawing.Point(32, 80);
            this.txtEmpNumber.Name = "txtEmpNumber";
            this.txtEmpNumber.Size = new System.Drawing.Size(64, 23);
            this.txtEmpNumber.TabIndex = 2;
            this.txtEmpNumber.Leave += new System.EventHandler(this.txtEmpNumber_Leave);
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(224, 40);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(100, 23);
            this.txtValue.TabIndex = 3;
            this.txtValue.Leave += new System.EventHandler(this.txtValue_Leave);
            // 
            // lUserTotal
            // 
            this.lUserTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lUserTotal.Location = new System.Drawing.Point(592, 352);
            this.lUserTotal.Name = "lUserTotal";
            this.lUserTotal.Size = new System.Drawing.Size(40, 23);
            this.lUserTotal.TabIndex = 47;
            this.lUserTotal.Text = "Total";
            // 
            // btnEnter
            // 
            this.btnEnter.Location = new System.Drawing.Point(672, 40);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(48, 23);
            this.btnEnter.TabIndex = 6;
            this.btnEnter.Text = "Enter";
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(32, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 16);
            this.label2.TabIndex = 45;
            this.label2.Text = "Emp No";
            // 
            // drpCashier
            // 
            this.drpCashier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCashier.DropDownWidth = 121;
            this.drpCashier.Enabled = false;
            this.drpCashier.Location = new System.Drawing.Point(32, 40);
            this.drpCashier.Name = "drpCashier";
            this.drpCashier.Size = new System.Drawing.Size(160, 23);
            this.drpCashier.TabIndex = 0;
            this.drpCashier.SelectedIndexChanged += new System.EventHandler(this.drpCashier_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(32, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 23);
            this.label6.TabIndex = 42;
            this.label6.Text = "Cashier";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(224, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 16);
            this.label1.TabIndex = 41;
            this.label1.Text = "Value";
            // 
            // dgDeposits
            // 
            this.dgDeposits.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgDeposits.CaptionVisible = false;
            this.dgDeposits.DataMember = "";
            this.dgDeposits.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgDeposits.Location = new System.Drawing.Point(32, 120);
            this.dgDeposits.Name = "dgDeposits";
            this.dgDeposits.Size = new System.Drawing.Size(688, 224);
            this.dgDeposits.TabIndex = 0;
            this.dgDeposits.TabStop = false;
            this.dgDeposits.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgDeposits_MouseUp);
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave.Location = new System.Drawing.Point(680, 72);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 10;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // drpDeposit
            // 
            this.drpDeposit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDeposit.Location = new System.Drawing.Point(496, 40);
            this.drpDeposit.Name = "drpDeposit";
            this.drpDeposit.Size = new System.Drawing.Size(144, 23);
            this.drpDeposit.TabIndex = 5;
            this.drpDeposit.SelectedIndexChanged += new System.EventHandler(this.drpDeposit_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(496, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 16);
            this.label5.TabIndex = 30;
            this.label5.Text = "Disbursement Type";
            // 
            // tpView
            // 
            this.tpView.Controls.Add(this.lblTot);
            this.tpView.Controls.Add(this.txtTot);
            this.tpView.Controls.Add(this.label3);
            this.tpView.Controls.Add(this.drpPaymentMethod);
            this.tpView.Controls.Add(this.btnExcel);
            this.tpView.Controls.Add(this.chxBranchFloats);
            this.tpView.Controls.Add(this.btnRefresh);
            this.tpView.Controls.Add(this.chxPostedToFACT);
            this.tpView.Controls.Add(this.chxWholeCountry);
            this.tpView.Controls.Add(this.drpDeposit2);
            this.tpView.Controls.Add(this.label11);
            this.tpView.Controls.Add(this.label4);
            this.tpView.Controls.Add(this.txtEmpNumber2);
            this.tpView.Controls.Add(this.drpCashier2);
            this.tpView.Controls.Add(this.label7);
            this.tpView.Controls.Add(this.dgPrevious);
            this.tpView.Controls.Add(this.btnLoad);
            this.tpView.Controls.Add(this.dtTo);
            this.tpView.Controls.Add(this.dtFrom);
            this.tpView.Controls.Add(this.lTo);
            this.tpView.Controls.Add(this.lFrom);
            this.tpView.Enabled = false;
            this.tpView.Location = new System.Drawing.Point(0, 25);
            this.tpView.Name = "tpView";
            this.tpView.Selected = false;
            this.tpView.Size = new System.Drawing.Size(760, 391);
            this.tpView.TabIndex = 3;
            this.tpView.Title = "View Disbursements";
            this.tpView.Visible = false;
            // 
            // lblTot
            // 
            this.lblTot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTot.Location = new System.Drawing.Point(552, 357);
            this.lblTot.Name = "lblTot";
            this.lblTot.Size = new System.Drawing.Size(34, 20);
            this.lblTot.TabIndex = 95;
            this.lblTot.Text = "Total";
            // 
            // txtTot
            // 
            this.txtTot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtTot.Location = new System.Drawing.Point(592, 354);
            this.txtTot.Name = "txtTot";
            this.txtTot.ReadOnly = true;
            this.txtTot.Size = new System.Drawing.Size(88, 23);
            this.txtTot.TabIndex = 94;
            this.txtTot.TabStop = false;
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(514, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 16);
            this.label3.TabIndex = 93;
            this.label3.Text = "Pay Method";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // drpPaymentMethod
            // 
            this.drpPaymentMethod.BackColor = System.Drawing.SystemColors.Window;
            this.drpPaymentMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPaymentMethod.Location = new System.Drawing.Point(517, 32);
            this.drpPaymentMethod.Name = "drpPaymentMethod";
            this.drpPaymentMethod.Size = new System.Drawing.Size(104, 23);
            this.drpPaymentMethod.TabIndex = 92;
            // 
            // btnExcel
            // 
            this.btnExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnExcel.Image")));
            this.btnExcel.Location = new System.Drawing.Point(646, 77);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(32, 32);
            this.btnExcel.TabIndex = 91;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // chxBranchFloats
            // 
            this.chxBranchFloats.Location = new System.Drawing.Point(72, 101);
            this.chxBranchFloats.Name = "chxBranchFloats";
            this.chxBranchFloats.Size = new System.Drawing.Size(104, 20);
            this.chxBranchFloats.TabIndex = 56;
            this.chxBranchFloats.Text = "Branch Floats";
            // 
            // btnRefresh
            // 
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnRefresh.ImageIndex = 4;
            this.btnRefresh.ImageList = this.menuIcons;
            this.btnRefresh.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRefresh.Location = new System.Drawing.Point(726, 32);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(24, 24);
            this.btnRefresh.TabIndex = 55;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // chxPostedToFACT
            // 
            this.chxPostedToFACT.Location = new System.Drawing.Point(72, 79);
            this.chxPostedToFACT.Name = "chxPostedToFACT";
            this.chxPostedToFACT.Size = new System.Drawing.Size(129, 20);
            this.chxPostedToFACT.TabIndex = 50;
            this.chxPostedToFACT.Text = "Posted to FACT";
            // 
            // chxWholeCountry
            // 
            this.chxWholeCountry.Location = new System.Drawing.Point(72, 56);
            this.chxWholeCountry.Name = "chxWholeCountry";
            this.chxWholeCountry.Size = new System.Drawing.Size(104, 24);
            this.chxWholeCountry.TabIndex = 54;
            this.chxWholeCountry.Text = "Whole country";
            // 
            // drpDeposit2
            // 
            this.drpDeposit2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDeposit2.Location = new System.Drawing.Point(72, 32);
            this.drpDeposit2.Name = "drpDeposit2";
            this.drpDeposit2.Size = new System.Drawing.Size(144, 23);
            this.drpDeposit2.TabIndex = 52;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(72, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(144, 16);
            this.label11.TabIndex = 53;
            this.label11.Text = "Disbursement Type";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(424, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 16);
            this.label4.TabIndex = 49;
            this.label4.Text = "Emp No";
            // 
            // txtEmpNumber2
            // 
            this.txtEmpNumber2.Location = new System.Drawing.Point(424, 32);
            this.txtEmpNumber2.Name = "txtEmpNumber2";
            this.txtEmpNumber2.Size = new System.Drawing.Size(64, 23);
            this.txtEmpNumber2.TabIndex = 47;
            this.txtEmpNumber2.Leave += new System.EventHandler(this.txtEmpNumber_Leave);
            // 
            // drpCashier2
            // 
            this.drpCashier2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCashier2.DropDownWidth = 121;
            this.drpCashier2.Location = new System.Drawing.Point(256, 32);
            this.drpCashier2.Name = "drpCashier2";
            this.drpCashier2.Size = new System.Drawing.Size(160, 23);
            this.drpCashier2.TabIndex = 46;
            this.drpCashier2.SelectedIndexChanged += new System.EventHandler(this.drpCashier2_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(256, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 23);
            this.label7.TabIndex = 48;
            this.label7.Text = "Cashier";
            // 
            // dgPrevious
            // 
            this.dgPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgPrevious.CaptionVisible = false;
            this.dgPrevious.DataMember = "";
            this.dgPrevious.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dgPrevious.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgPrevious.Location = new System.Drawing.Point(64, 129);
            this.dgPrevious.Name = "dgPrevious";
            this.dgPrevious.ReadOnly = true;
            this.dgPrevious.Size = new System.Drawing.Size(616, 215);
            this.dgPrevious.TabIndex = 22;
            this.dgPrevious.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgPrevious_MouseUp);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(646, 32);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(48, 23);
            this.btnLoad.TabIndex = 20;
            this.btnLoad.Text = "Load";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // dtTo
            // 
            this.dtTo.CustomFormat = "ddd dd MMM yyyy HH:mm";
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTo.Location = new System.Drawing.Point(424, 80);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(144, 23);
            this.dtTo.TabIndex = 18;
            this.dtTo.Tag = "";
            this.dtTo.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            // 
            // dtFrom
            // 
            this.dtFrom.CustomFormat = "ddd dd MMM yyyy HH:mm";
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFrom.Location = new System.Drawing.Point(256, 80);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(144, 23);
            this.dtFrom.TabIndex = 17;
            this.dtFrom.Tag = "";
            this.dtFrom.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            // 
            // lTo
            // 
            this.lTo.Location = new System.Drawing.Point(424, 64);
            this.lTo.Name = "lTo";
            this.lTo.Size = new System.Drawing.Size(32, 16);
            this.lTo.TabIndex = 16;
            this.lTo.Text = "To:";
            // 
            // lFrom
            // 
            this.lFrom.Location = new System.Drawing.Point(256, 64);
            this.lFrom.Name = "lFrom";
            this.lFrom.Size = new System.Drawing.Size(40, 16);
            this.lFrom.TabIndex = 15;
            this.lFrom.Text = "From:";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(720, 16);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(48, 23);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(664, 16);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(48, 23);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataMember = "";
            // 
            // CashierDisbursments
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.gbMain);
            this.Name = "CashierDisbursments";
            this.Text = "Cashier Disbursements";
            this.Load += new System.EventHandler(this.CashierDeposits_Load);
            this.gbMain.ResumeLayout(false);
            this.tcMain.ResumeLayout(false);
            this.tpUncashed.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgUncashed)).EndInit();
            this.tpEnter.ResumeLayout(false);
            this.tpEnter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDeposits)).EndInit();
            this.tpView.ResumeLayout(false);
            this.tpView.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgPrevious)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }

        /// <summary>
        /// This checks whether cashier totals match cashier deposits for this 
        /// cashier and is run when the screen is closed. I think this is pretty 
        /// pointless now given that this screen is really only used for 
        /// cashier disbursements now and not typically deposits.
        /// </summary>
        private void DoTotalsMatch()
        {
            DataSet ds = PaymentManager.GetUnexportedCashierTotals(Convert.ToInt32(Config.BranchCode), out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string errors = "";
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        errors += (string)r[CN.CodeDescription] + Environment.NewLine;
                    }
                    ShowWarning(GetResource("M_CASHIERNOTMATCH", new object[] { errors }));
                }
            }
        }

        /// <summary>
        /// This will just set up the cashier drop down with the currently logged
        /// in user and call the drop down handler which will do the actual
        /// data retrieval.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CashierDeposits_Load(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                LoadStaticData();
                txtValue.Text = (0).ToString(DecimalPlaces);
                if (!txtEmpNumber.Enabled)
                {
                    txtEmpNumber.Text = Credential.UserId.ToString();
                    txtEmpNumber_Leave(txtEmpNumber, null);
                    ((DataView)drpDeposit.DataSource).RowFilter = CN.IsDeposit + " in(2,3)";
                }

                loaded = true;

                drpCashier_SelectedIndexChanged(null, null);
                chxBranchFloats.Enabled = Credential.HasPermission(CosacsPermissionEnum.CashierBranchFloat);

            }
            catch (Exception ex)
            {
                Catch(ex, "CashierDeposits_Load");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            CloseTab();
        }

        /// <summary>
        /// This performs validation on the value of the disbursement entered.
        /// Must be numeric.
        /// Must be positive (floats are now done in the cashier deposits screen).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtValue_Leave(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                if (!IsStrictNumeric(StripCurrency(txtValue.Text)))
                {
                    errorProvider1.SetError(txtValue, GetResource("M_NONNUMERIC"));
                    txtValue.Select(0, txtValue.Text.Length);
                    txtValue.Focus();
                }
                else
                {
                    /* make sure it's positive */
                    if (Convert.ToDecimal(StripCurrency(txtValue.Text)) < 0)
                    {
                        errorProvider1.SetError(txtValue, GetResource("M_POSITIVENUM"));
                        txtValue.Select(0, txtValue.Text.Length);
                        txtValue.Focus();
                    }
                    else
                    {
                        errorProvider1.SetError(txtValue, "");
                        if (txtValue.Text.Length > 0)
                            txtValue.Text = Convert.ToDecimal(StripCurrency(txtValue.Text)).ToString(DecimalPlaces);
                        else
                            txtValue.Text = (0).ToString(DecimalPlaces);
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "txtValue_Leave");
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// This executes when the user attempts to enter the disbursement/deposit.
        /// It will ensure that they are not trying to disburse a quantity 
        /// greater than the amount they have available for deposit for a particular 
        /// payment method. This check will be omitted if the deposit is being 
        /// made to the safe (i.e. if this is an increase in branch float). 
        /// It will also ensure that they have entered a unique
        /// reference if required. For petty cash disbursements authorisation will
        /// be required.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnter_Click(object sender, System.EventArgs e)
        {
            try
            {
                bool status = true;
                bool unique = false;
                Wait();

                decimal total = Convert.ToDecimal(StripCurrency(txtTotal.Text));
                decimal val = Convert.ToDecimal(StripCurrency(txtValue.Text));
                short isDeposit = (short)((DataRowView)drpDeposit.SelectedItem)[CN.IsDeposit];
                bool refUnique = Convert.ToBoolean(((DataRowView)drpDeposit.SelectedItem)[CN.ReferenceUnique]);
                string payMethod = (string)((DataRowView)drpPayMethod.SelectedItem)[CN.Code];
                string code = (string)((DataRowView)drpDeposit.SelectedItem)[CN.Code];

                if (val > 0 && ((DataView)drpDeposit.DataSource).Count > 0)
                {
                    if (code != "SAF")
                    {
                        /* make sure that the deposit value is within range 
                         * if it's a deposit */
                        decimal forDeposit = 0;
                        Deposits.RowFilter = CN.Code + " = '" + payMethod + "'";
                        foreach (DataRowView r in Deposits)
                            forDeposit += (decimal)r[CN.ForDeposit];

                        if (dgDeposits.DataSource != null)
                        {
                            foreach (DataRowView r in (DataView)dgDeposits.DataSource)
                            {
                                string pm = (string)r[CN.PayMethod];
                                if (pm == payMethod)
                                    forDeposit -= (decimal)r[CN.Value];
                            }
                        }

                        if (val > forDeposit)
                        {
                            ShowInfo("M_DEPOSITTOOHIGH", new Object[] { val.ToString(DecimalPlaces), forDeposit.ToString(DecimalPlaces) });
                            //val = forDeposit; //KEF don't want to enter row if deposit is too high
                            val = 0;
                            txtValue.Text = val.ToString(DecimalPlaces);
                        }
                    }

                    if (val > 0)
                    {
                        /* make sure a reference has been entered if necessary */
                        if (!txtReference.ReadOnly)
                        {
                            if (txtReference.Text.Trim().Length == 0)
                            {
                                errorProvider1.SetError(txtReference, GetResource("M_ENTERMANDATORY"));
                                txtReference.Focus();
                                status = false;
                            }
                            else
                            {
                                if (refUnique && (bool)Country[CountryParameterNames.DepositUniqueReference])
                                {
                                    /* make sure reference is unique */
                                    unique = PaymentManager.IsDepositReferenceUnique(txtReference.Text, out Error);
                                    if (Error.Length > 0)
                                        ShowError(Error);
                                    else
                                    {
                                        if (!unique)
                                        {
                                            errorProvider1.SetError(txtReference, GetResource("M_NOTUNIQUE"));
                                            status = false;
                                        }
                                        else
                                        {
                                            if (dtDeposits != null)
                                            {
                                                foreach (DataRow r in dtDeposits.Rows)
                                                {
                                                    if (txtReference.Text == (string)r[CN.Reference])
                                                    {
                                                        unique = false;
                                                        status = false;
                                                        break;
                                                    }
                                                }
                                                if (!unique)
                                                    errorProvider1.SetError(txtReference, GetResource("M_NOTUNIQUE"));
                                                else
                                                    errorProvider1.SetError(txtReference, "");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                            errorProvider1.SetError(txtReference, "");

                        if (status)
                        {
                            if (isDeposit == 2 && !txtEmpNumber.Enabled)			/* petty cash */
                            {
                                /* need authorisation */
                                AuthorisePrompt auth = new AuthorisePrompt(this, authPettyCash, "Authorisation is requierd for Petty Cash Deposits");
                                auth.ShowDialog();
                                status = auth.Authorised;
                            }
                        }

                        if (status)
                        {
                            if (dtDeposits == null)
                            {
                                dtDeposits = new DataTable(TN.Deposits);
                                dtDeposits.Columns.AddRange(new DataColumn[] {	  new DataColumn(CN.TransTypeCode),
																				  new DataColumn(CN.CodeDescription),
																				  new DataColumn(CN.Value, Type.GetType("System.Decimal")),
																				  new DataColumn(CN.EmployeeName),
																				  new DataColumn(CN.EmployeeNo, Type.GetType("System.Int32")),
																				  new DataColumn(CN.DateDeposit, Type.GetType("System.DateTime")),
																				  new DataColumn(CN.Runno, Type.GetType("System.Int32")),
																				  new DataColumn(CN.EmployeeNoEntered, Type.GetType("System.Int32")),
																				  new DataColumn(CN.BranchNo, Type.GetType("System.Int16")),
																				  new DataColumn(CN.Reference),
																				  new DataColumn(CN.PayMethod),
																				  new DataColumn(CN.PayMethodDescription),
																				  new DataColumn(CN.IsCashierFloat, Type.GetType("System.Int16")),
																				  new DataColumn(CN.IncludeInCashierTotals, Type.GetType("System.Int16"))});
                            }

                            DataRow r = dtDeposits.NewRow();

                            r[CN.TransTypeCode] = ((DataRowView)drpDeposit.SelectedItem)[CN.Code];
                            r[CN.CodeDescription] = ((DataRowView)drpDeposit.SelectedItem)[CN.CodeDescription];

                            r[CN.Value] = MoneyStrToDecimal(txtValue.Text);

                            if (CashierTotalled)
                                r[CN.IncludeInCashierTotals] = 0;
                            else
                                r[CN.IncludeInCashierTotals] = 1;

                            r[CN.EmployeeName] = ((DataRowView)drpCashier.SelectedItem)[CN.EmployeeName];
                            r[CN.EmployeeNo] = ((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo];
                            r[CN.IsCashierFloat] = 0;
                            r[CN.DateDeposit] = StaticDataManager.GetServerDateTime();
                            r[CN.Runno] = code == "SAF" ? -1 : 0;		/* branch floats to the safe must not go to FACT */
                            r[CN.EmployeeNoEntered] = Credential.UserId;
                            r[CN.BranchNo] = Convert.ToInt16(Config.BranchCode);
                            r[CN.Reference] = txtReference.Text;
                            r[CN.PayMethod] = ((DataRowView)drpPayMethod.SelectedItem)[CN.Code];
                            r[CN.PayMethodDescription] = ((DataRowView)drpPayMethod.SelectedItem)[CN.CodeDescription];

                            total += (decimal)r[CN.Value];
                            txtTotal.Text = total.ToString(DecimalPlaces);

                            dtDeposits.DefaultView.AllowNew = false;

                            dtDeposits.Rows.Add(r);
                            dgDeposits.DataSource = dtDeposits.DefaultView;

                            if (dgDeposits.TableStyles.Count == 0)
                            {
                                DataGridTableStyle tabStyle = new DataGridTableStyle();
                                tabStyle.MappingName = TN.Deposits;
                                AddColumnStyle(CN.EmployeeName, tabStyle, 180, true, GetResource("T_NAME"), "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.EmployeeNo, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.PayMethod, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.PayMethodDescription, tabStyle, 100, true, GetResource("T_PAYMETHOD"), "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.Code, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.CodeDescription, tabStyle, 100, true, GetResource("T_DEPOSITTYPE"), "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.Value, tabStyle, 120, true, GetResource("T_VALUE"), DecimalPlaces, HorizontalAlignment.Right);
                                AddColumnStyle(CN.BranchNo, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.Reference, tabStyle, 140, true, GetResource("T_REFERENCE"), "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.IsCashierFloat, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                AddColumnStyle(CN.IncludeInCashierTotals, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                                dgDeposits.TableStyles.Add(tabStyle);
                            }
                            txtReference.Text = "";
                            txtValue.Text = (0).ToString(DecimalPlaces);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnEnter_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private int oldCashier = -1;

        /// <summary>
        /// Runs when a new cashier is selected / entered. It will check whether the
        /// cashier has totalled yet and then retrieve permissions to see if the new 
        /// cashier is allowed to perform branch floats. If the user is allowed to 
        /// perform branch floats, safe will appear in the deposit type drop down, 
        /// otherwise it will not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void drpCashier_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                if (loaded &&
                    drpCashier.SelectedIndex != sourceIndex)
                {
                    int i = sourceIndex = drpCashier.SelectedIndex;

                    int empeeno = (int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo];

                    if (empeeno != oldCashier)
                    {
                        oldCashier = empeeno;

                        txtEmpNumber.Text = ((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo].ToString();
                        //string empeetype = (string)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeType];       //IP - 23/01/13 - #12086

                        /* find out if this cashier has already submitted totals or not */
                        CashierTotalled = PaymentManager.HasCashierTotalled((int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo],
                            out Error);
                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            RefreshDeposits();
                        }

                        /* work out if the newly selected cashier has the permission to 
                         * make branch floats to the safe */
                        allowBranchFloats.Enabled = false;
                        DataSet menus = StaticDataManager.GetDynamicMenus(Credential.UserId, this.Name, out Error);
                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            if (menus != null)
                            {
                                foreach (DataTable tab in menus.Tables)
                                    foreach (DataRow row in tab.Rows)
                                        if ((string)row["Control"] == "allowBranchFloats")
                                        {
                                            string key = (string)row["Screen"] + ":" + (string)row["Control"];
                                            object o = dynamicMenus[key];
                                            if (o != null)
                                                ((Control)o).Enabled = Convert.ToBoolean(row["Enabled"]);
                                        }
                            }
                        }
                        if (allowBranchFloats.Enabled)
                            //((DataView)drpDeposit.DataSource).RowFilter = "";
                            ((DataView)drpDeposit.DataSource).RowFilter = excOtherCodes;                                //IP - 07/03/12 - #9741 - UAT119
                        else
                            //((DataView)drpDeposit.DataSource).RowFilter = noSafeFilter;
                            ((DataView)drpDeposit.DataSource).RowFilter = noSafeFilter + " and " + excOtherCodes;        //IP - 07/03/12 - #9741 - UAT119

                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "drpCashier_SelectedIndexChanged");
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// This will retrieve the outstanding deposits for a particular cashier.
        /// This is requierd so we can check whether they are trying to disburse
        /// more than they have available. 
        /// </summary>
        private void RefreshDeposits()
        {
            short branchno = Convert.ToInt16(Config.BranchCode);
            DataSet ds = PaymentManager.GetCashierOutstandingIncomeByPayMethod((int)((DataRowView)drpCashier.SelectedItem)[CN.EmployeeNo], branchno, out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
                Deposits = ds.Tables[TN.CashierOutstandingIncome].DefaultView;
        }

        /// <summary>
        /// This is a context menu on the disbursements datagrid to provide the option
        /// to delete a disbursement. This only works to delete disbursements which are
        /// in the data grid but have yet been saved to the database which is just a 
        /// matter of removing from the datagrid again. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgDeposits_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Wait();

                int index = dgDeposits.CurrentRowIndex;

                if (index >= 0)
                {
                    dgDeposits.Select(index);

                    if (e.Button == MouseButtons.Right)
                    {
                        /* popup a re-print option */
                        DataGrid ctl = (DataGrid)sender;

                        MenuCommand m1 = new MenuCommand(GetResource("P_DELETE"));
                        m1.Click += new System.EventHandler(this.OnRemoveDeposit);

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
                Catch(ex, "dgDeposits_MouseUp");
            }
            finally
            {
                StopWait();
            }
        }

        private void OnRemoveDeposit(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                decimal total = MoneyStrToDecimal(txtTotal.Text);

                int index = dgDeposits.CurrentRowIndex;

                if (index >= 0)
                {
                    total -= (decimal)((DataView)dgDeposits.DataSource)[index][CN.Value];
                    ((DataView)dgDeposits.DataSource).Delete(index);
                    txtTotal.Text = total.ToString(DecimalPlaces);

                    RefreshDeposits();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "OnRemoveDeposit");
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// This is used to void disbursements which have already been saved to the 
        /// database. Deposits that have been interfaced to FACT will have to be reversed
        /// rather than just flagged as void.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnVoidDeposit(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                int index = dgPrevious.CurrentRowIndex;
                bool reverse = false;

                if (index >= 0)
                {
                    // Allow reversal if posted to FACT (RunNo > 0)
                    // Note: RunNo can be -1 for system setup to safe. This RunNo
                    // is not posted to FACT and must not be reversed.
                    if ((int)((DataView)dgPrevious.DataSource)[index][CN.Runno] > 0)
                    {
                        if (DialogResult.Yes == ShowInfo("M_REVERSEDEPOSIT", MessageBoxButtons.YesNo))
                        {
                            reverse = true;
                        }
                    }
                    PaymentManager.VoidCashierDeposit((int)((DataView)dgPrevious.DataSource)[index][CN.DepositID],
                        DateTime.Now,
                        reverse,
                        out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        this.btnLoad_Click(null, null);
                        RefreshDeposits();
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "OnRemoveDeposit");
            }
            finally
            {
                StopWait();
            }
        }

        private void drpCashier2_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            txtEmpNumber2.Text = "";
        }

        /// <summary>
        /// Leave event on the text box that allows the user to enter a courts person
        /// who does not appear in the list of cashiers. The id entered will be used to 
        /// look up the employees details and add them to the drop down. When they
        /// are added to the drop down, the drop down event will then fire and take
        /// care of the rest of the work that needs doing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtEmpNumber_Leave(object sender, System.EventArgs e)
        {
            try
            {
                TextBox empNum = (TextBox)sender;
                ComboBox cashier = null;

                if (empNum.Name == "txtEmpNumber")
                    cashier = drpCashier;
                else
                    cashier = drpCashier2;

                Wait();
                string err = "";
                string employee = "";
                bool status = true;
                DataSet ds = null;

                if (empNum.Text.Length > 0)
                {
                    if (!IsNumeric(empNum.Text))
                    {
                        empNum.Select(0, empNum.Text.Length);
                        errorProvider1.SetError(empNum, GetResource("M_NONNUMERIC"));
                        empNum.Focus();
                        status = false;
                    }
                    else
                    {
                        errorProvider1.SetError(empNum, "");
                    }
                }
                else
                    status = false;

                if (status)
                {
                    ds = Login.GetEmployeeDetails(Convert.ToInt32(empNum.Text), err);
                    if (err.Length > 0)
                        ShowError(err);
                    else
                    {
                        if (ds != null)
                        {
                            foreach (DataTable dt in ds.Tables)
                            {
                         
                                foreach (DataRow row in dt.Rows)
                                {

                                    DataTable salesStaff = (DataTable)cashier.DataSource;
                                    DataRow r = salesStaff.NewRow();

                                    //r[CN.EmployeeName] = (string)row[CN.EmployeeName];  //IP - 23/01/13 - #12086
                                    //IP - 23/01/13 - #12086
                                    //r[CN.EmployeeType] = (string)row[CN.EmployeeType]; //- UAT(5.2)-642 //IP - 29/07/09 - UAT(642/749) - reinstating original code. 
                                    //employee = (string)row[CN.EmployeeName];          //IP - 23/01/13 - #12086

                                    r[CN.EmployeeNo] = Convert.ToInt32(empNum.Text);
                                    employee = Convert.ToString(r[CN.EmployeeNo]) + " : " + (string)row[CN.EmployeeName];  //IP - 23/01/13 - #12086
                                    r[CN.EmployeeName] = employee;

                                    int b = cashier.FindString(employee);    //IP - 23/01/13 - #12086

                                    if (b == -1)         //IP - 23/01/13 - #12086 - only add if doesn't exist already
                                    {
                                        salesStaff.Rows.Add(r);
                                    }

                                    salesStaff.DefaultView.Sort = "EmpeeNo ASC";     //IP - 23/01/13 - #12086

                                    cashier.DataSource = salesStaff;

  
                                }
                            }

                            int i = cashier.FindString(employee);
                            if (i != -1)
                                cashier.SelectedIndex = i;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "txtEmpNumber_Leave");
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// This is used to load previous deposits / disbursements. This forms
        /// the second tab page in the tab control. Numerous criteria can be 
        /// supplied to fine tune the request. It is from this tab page that
        /// previous deposits can be voided.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, System.EventArgs e)
        {
            DateTime toDate;
            try
            {
                Wait();

                if (!refresh)
                    toDate = dtTo.Value;
                else
                    toDate = StaticDataManager.GetServerDateTime();

                short branchNo = Convert.ToInt16(Config.BranchCode);
                if (chxWholeCountry.Checked)
                    branchNo = 0;

                short postedtofact = chxPostedToFACT.Checked ? (short)1 : (short)0;

                var paymentMethod = Convert.ToInt32(((DataRowView)drpPaymentMethod.SelectedItem)[CN.Code]);


                DataSet previous = PaymentManager.GetCashierDeposits((int)((DataRowView)drpCashier2.SelectedItem)[CN.EmployeeNo],
                    postedtofact,
                    dtFrom.Value,
                    toDate,
                    branchNo,
                    (string)((DataRowView)drpDeposit2.SelectedItem)[CN.Code],
                    chxBranchFloats.Checked && Credential.HasPermission(CosacsPermissionEnum.CashierBranchFloat) ,
                    paymentMethod,                  //IP - 15/12/11 - #8810 - CR1234
                    out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    dgPrevious.DataSource = previous.Tables[0].DefaultView;

                    // Add an unbound stand-alone icon column to mark reversals
                    previous.Tables[0].DefaultView.Table.Columns.Add("Icon");

                    if (dgPrevious.TableStyles.Count == 0)
                    {
                        DataGridTableStyle tabStyle = new DataGridTableStyle();
                        tabStyle.MappingName = previous.Tables[0].TableName;

                        // Icon column to mark reversals
                        DataGridIconColumn iconColumn = new DataGridIconColumn(imageList1.Images[0], imageList1.Images[1], CN.IsReversed, "1", "2");
                        iconColumn.HeaderText = "";
                        iconColumn.MappingName = "Icon";
                        iconColumn.Width = imageList1.Images[0].Size.Width;
                        tabStyle.GridColumnStyles.Add(iconColumn);

                        AddColumnStyle(CN.Deposit, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.DateDeposit, tabStyle, 120, true, GetResource("T_DATEDEPOSIT"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.EmployeeNo, tabStyle, 40, true, GetResource("T_EMPEENO"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.Code, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.CodeDescription, tabStyle, 120, true, GetResource("T_PAYMETHOD"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.TransTypeCode, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                        //AddColumnStyle(CN.Description, tabStyle, 120, true, GetResource("T_DEPOSITTYPE"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.Description, tabStyle, 120, true, GetResource("T_DISBURSEMENTTYPE"), "", HorizontalAlignment.Left);               //IP - 8/12/11 - CR1234
                        AddColumnStyle(CN.Runno, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.DepositValue, tabStyle, 100, true, GetResource("T_VALUE"), DecimalPlaces, HorizontalAlignment.Right);
                        AddColumnStyle(CN.BranchNo, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.Reference, tabStyle, 140, false, GetResource("T_REFERENCE"), "", HorizontalAlignment.Left);
                        AddColumnStyle(CN.IsReversed, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                        dgPrevious.TableStyles.Add(tabStyle);
                    }

                    //IP - 15/12/11 - #8809 - CR1234 - Cashier Disbursements Total
                    var depositTot = 0.0;
                    foreach (DataRow dr in previous.Tables[0].Rows)
                    {
                        depositTot += Convert.ToDouble(dr["depositvalue"]);
                    }

                    txtTot.Text = depositTot.ToString(DecimalPlaces);

                    ((MainForm)FormRoot).statusBar1.Text = previous.Tables[0].Rows.Count.ToString() + " Rows returned";

                    refresh = false;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnLoad_Click");
            }
            finally
            {
                StopWait();
            }
        }

        /// <summary>
        /// Override function which will step in and run if the screen is closing for
        /// any reason. If there are unsaved deposits the user will be prompted and
        /// asked if they want to save them or not.
        /// </summary>
        /// <returns></returns>
        public override bool ConfirmClose()
        {
            if (dgDeposits.DataSource != null)
            {
                if (DialogResult.Yes == ShowInfo("M_SAVEDEPOSITS", MessageBoxButtons.YesNo))
                    btnSave_Click(null, null);
            }
            DoTotalsMatch();
            return true;
        }


        /// <summary>
        /// Does what you would expect really.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                if (dgDeposits.DataSource != null)
                {
                    if (deposits == null)
                    {
                        deposits = new DataSet();
                        deposits.Tables.Add(((DataView)dgDeposits.DataSource).Table);
                    }

                    PaymentManager.SaveCashierDisbursements(deposits, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        //IP - 22/12/11 - #8811 - CR1234
                        DataTable cashierDisbursements = new DataTable();
                        cashierDisbursements = ((DataView)dgDeposits.DataSource).Table.Copy();

                        var cashier = Convert.ToString(((DataRowView)drpCashier.SelectedItem)[CN.EmployeeName]);

                        NewPrintCashierDisbursementReceipt(cashierDisbursements, cashier);

                        ((DataView)dgDeposits.DataSource).Table.Clear();
                        dgDeposits.DataSource = null;
                        txtTotal.Text = (0).ToString(DecimalPlaces);
                        RefreshDeposits();
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
        /// Context menu which allows you to void previous deposits.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgPrevious_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Wait();

                int index = dgPrevious.CurrentRowIndex;

                if (index >= 0)
                {
                    dgPrevious.Select(index);

                    if (e.Button == MouseButtons.Right && (((DataView)dgPrevious.DataSource)[index][CN.IsReversed]).ToString() != "1")
                    {
                        // A disbursement can be reversed only if it has not already been reversed
                        // If it has been delivered to FACT, then this can also be reversed on FACT
                        DataGrid ctl = (DataGrid)sender;

                        MenuCommand m1 = new MenuCommand(GetResource("P_DELETE"));
                        m1.Click += new System.EventHandler(this.OnVoidDeposit);

                        PopupMenu popup = new PopupMenu();
                        popup.Animate = Animate.Yes;
                        popup.AnimateStyle = Animation.SlideHorVerPositive;
                        popup.MenuCommands.Add(m1);
                        MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "dgDeposits_MouseUp");
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
                dgPrevious.DataSource = null;
                dgDeposits.DataSource = null;
                txtTotal.Text = (0).ToString(DecimalPlaces);
                RefreshDeposits();
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
        /// The third tab page provides a view of unexported (to FACT) cashier totals.
        /// I think this is a slightly stupid place to have this. There may be some
        /// forgotten sense in it's being here, who knows.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void btnCashierTotals_Click(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        Wait();
        //        decimal total = 0;
        //        DataSet cashierTotals = PaymentManager.GetUnexportedCashierTotals(Convert.ToInt16(Config.BranchCode),
        //            out total,
        //            out Error);
        //        if(Error.Length>0)
        //            ShowError(Error);
        //        else
        //        {
        //            dgCashierTotals.DataSource = cashierTotals.Tables[0].DefaultView;
        //            dgCashierTotals.TableStyles.Clear();

        //            DataGridTableStyle tabStyle = new DataGridTableStyle();
        //            tabStyle.MappingName = cashierTotals.Tables[0].TableName;
        //            dgCashierTotals.TableStyles.Add(tabStyle);

        //            tabStyle.GridColumnStyles[CN.ID].Width = 0;
        //            tabStyle.GridColumnStyles[CN.DateTo].Width = 100;
        //            tabStyle.GridColumnStyles[CN.DateTo].HeaderText = GetResource("T_DATE");
        //            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.DateTo]).Format = "dd/MM/yyyy HH:mm";
        //            tabStyle.GridColumnStyles[CN.BranchNo].Width = 40;
        //            tabStyle.GridColumnStyles[CN.BranchNo].HeaderText = GetResource("T_BRANCH");
        //            tabStyle.GridColumnStyles[CN.EmployeeNo.ToLower()].Width = 40;
        //            tabStyle.GridColumnStyles[CN.EmployeeNo.ToLower()].HeaderText = GetResource("T_EMPEENO");
        //            tabStyle.GridColumnStyles[CN.EmployeeName].Width = 140;
        //            tabStyle.GridColumnStyles[CN.EmployeeName].HeaderText = GetResource("T_EMPEENAME");
        //            tabStyle.GridColumnStyles[CN.UserTotal].Width = 100;
        //            tabStyle.GridColumnStyles[CN.UserTotal].HeaderText = GetResource("T_CASHIERTOTAL");
        //            tabStyle.GridColumnStyles[CN.UserTotal].Alignment = HorizontalAlignment.Right;
        //            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.UserTotal]).Format = DecimalPlaces;

        //            tabStyle.GridColumnStyles[CN.SystemTotal].Width = 100;
        //            tabStyle.GridColumnStyles[CN.SystemTotal].HeaderText = GetResource("T_SYSTEMVALUE");
        //            tabStyle.GridColumnStyles[CN.SystemTotal].Alignment = HorizontalAlignment.Right;
        //            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.SystemTotal]).Format = DecimalPlaces;

        //            tabStyle.GridColumnStyles[CN.Deposit].Width = 100;
        //            tabStyle.GridColumnStyles[CN.Deposit].HeaderText = GetResource("T_DEPOSIT");
        //            tabStyle.GridColumnStyles[CN.Deposit].Alignment = HorizontalAlignment.Right;
        //            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Deposit]).Format = DecimalPlaces;

        //            tabStyle.GridColumnStyles[CN.Difference].Width = 100;
        //            tabStyle.GridColumnStyles[CN.Difference].HeaderText = GetResource("T_DIFFERENCE");
        //            tabStyle.GridColumnStyles[CN.Difference].Alignment = HorizontalAlignment.Right;
        //            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.Difference]).Format = DecimalPlaces;

        //            this.txtUnexportedTotal.Text = total.ToString(DecimalPlaces);
        //            ((MainForm)FormRoot).statusBar1.Text = cashierTotals.Tables[0].Rows.Count.ToString() + " Rows returned";
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        Catch(ex, "btnCashierTotals_Click");
        //    }
        //    finally
        //    {
        //        StopWait();
        //    }
        //}

        /// <summary>
        /// Does some validation when the deposit type is changes to make sure
        /// that the payment method disbursements / petty cash is always cash
        /// and to enable the reference field where references are required. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void drpDeposit_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            bool status = true;
            try
            {
                if (loaded &&
                    drpDeposit.SelectedIndex != destIndex)
                {
                    Wait();

                    destIndex = drpDeposit.SelectedIndex;

                    short isDeposit = (short)((DataRowView)drpDeposit.SelectedItem)[CN.IsDeposit];
                    string code = (string)((DataRowView)drpDeposit.SelectedItem)[CN.Code];
                    bool refMandatory = Convert.ToBoolean(((DataRowView)drpDeposit.SelectedItem)[CN.ReferenceMandatory]);

                    if (status)
                    {
                        if (isDeposit == 2 || isDeposit == 3)	/* disbursement / petty cash */
                        {
                            /* make sure payment method is cash */
                            foreach (DataRowView r in ((DataTable)drpPayMethod.DataSource).DefaultView)
                                if ((string)r[CN.Code] == "1")
                                    drpPayMethod.SelectedItem = r;
                            //drpPayMethod.Enabled = false;							
                        }
                        //else
                        //	drpPayMethod.Enabled = true;
                    }

                    if (status)
                    {
                        txtReference.ReadOnly = !refMandatory;
                        txtReference.Text = "";
                        errorProvider1.SetError(txtReference, "");
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "drpDeposit_SelectedIndexChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void txtReference_Leave(object sender, System.EventArgs e)
        {
            if (txtReference.Text.Length > 0)
                errorProvider1.SetError(txtReference, "");
        }

        /// <summary>
        /// When the cashier totals are displayed it is the summary record 
        /// which is displayed (i.e. the record from the cashier totals table).
        /// To access the underlying breakdown by payment method (i.e the 
        /// corresponding records from the cashiertotalsbreakdown table) you 
        /// can double click the datagrid to launch a popup screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void dgCashierTotals_DoubleClick(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        Wait();
        //        int index = dgCashierTotals.CurrentRowIndex;

        //        if(index >=0 )
        //        {
        //            dgCashierTotals.Select(index);

        //            int totalid = (int)((DataView)dgCashierTotals.DataSource)[index][CN.ID];

        //            CashierTotalsBreakdown ctb = new CashierTotalsBreakdown(totalid);
        //            ctb.ShowDialog();
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        Catch(ex, "dgCashierTotals_DoubleClick");
        //    }
        //    finally
        //    {
        //        StopWait();
        //    }
        //}

        /// <summary>
        /// Since this screen is the dumping ground for all review functions without a 
        /// home, you can also right click the cashier totals grid and view a history
        /// of when the cashier has opened the cash drawer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void dgCashierTotals_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    try
        //    {
        //        Wait();
        //        int index = dgCashierTotals.CurrentRowIndex;

        //        if(index >=0 )
        //        {
        //            dgCashierTotals.Select(index);

        //            if(e.Button == MouseButtons.Right)
        //            {
        //                DataGrid ctl = (DataGrid)sender;

        //                MenuCommand m1 = new MenuCommand(GetResource("P_CASHTILLHISTORY"));
        //                m1.Click += new System.EventHandler(this.OnCashTillHistory);

        //                PopupMenu popup = new PopupMenu();
        //                popup.Animate = Animate.Yes;
        //                popup.AnimateStyle = Animation.SlideHorVerPositive;
        //                popup.MenuCommands.Add(m1);
        //                MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        Catch(ex, "dgCashierTotals_MouseUp");
        //    }
        //    finally
        //    {
        //        StopWait();
        //    }
        //}

        //private void OnCashTillHistory(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        Wait();

        //        int i = dgCashierTotals.CurrentRowIndex;

        //        if(i>=0)
        //        {
        //            int user = (int)((DataView)dgCashierTotals.DataSource)[i][CN.EmployeeNo];

        //            DataSet ds = PaymentManager.LoadCashDrawerOpen(user, out Error);
        //            if(Error.Length > 0)
        //                ShowError(Error);
        //            else
        //            {
        //                if(ds.Tables[0].Rows.Count>0)
        //                {
        //                    CashTillHistory cth = new CashTillHistory(ds.Tables[0]);
        //                    cth.ShowDialog();
        //                }
        //                else
        //                {
        //                    ShowInfo("M_NOCASHTILLHISTORY");
        //                }
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        Catch(ex, "OnCashTillHistory");
        //    }
        //    finally
        //    {
        //        StopWait();
        //    }
        //}

        private void btnRefresh_Click(object sender, System.EventArgs e)
        {
            refresh = true;
            btnLoad_Click(this, null);
        }

        /// <summary>
        /// This is the load function for the fourth tab page which displays
        /// cashiers with outstanding income i.e. money that needs to be 
        /// deposited. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadUncashed_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                DataSet cashiers = PaymentManager.GetCashiersWithOutstandingPayments(Convert.ToInt16(Config.BranchCode), out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    dgUncashed.DataSource = cashiers.Tables[0].DefaultView;
                    dgUncashed.TableStyles.Clear();

                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                    tabStyle.MappingName = cashiers.Tables[0].TableName;
                    AddColumnStyle(CN.EmployeeNo, tabStyle, 100, true, GetResource("T_EMPEENO"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.EmployeeName, tabStyle, 250, true, GetResource("T_EMPEENAME"), "", HorizontalAlignment.Left);
                    dgUncashed.TableStyles.Add(tabStyle);

                    ((MainForm)FormRoot).statusBar1.Text = cashiers.Tables[0].Rows.Count.ToString() + " Rows returned";
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnLoadUncashed_Click");
            }
            finally
            {
                StopWait();
            }
        }

        //IP - 8/12/11 - CR1234 
        private void btnExcel_Click(object sender, EventArgs e)
        {

            try
            {
                Wait();

                /* save the current data grid contents to a CSV */
                string comma = ",";
                string path = "";

                if (dgPrevious.CurrentRowIndex >= 0)
                {
                    DataView dv = (DataView)dgPrevious.DataSource;

                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
                    save.Title = "Save Cashier Disbursements";
                    save.CreatePrompt = true;

                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        path = save.FileName;
                        FileInfo fi = new FileInfo(path);
                        FileStream fs = fi.OpenWrite();

                        string line = "Date Of Deposit" + comma +
                             "Employee No" + comma +
                            "Pay Method" + comma +
                            "Disbursement Type" + comma +
                            "Value" + comma +
                            "Reference" + Environment.NewLine + Environment.NewLine;

                        byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
                        fs.Write(blob, 0, blob.Length);

                        foreach (DataRowView row in dv)
                        {
                            line = row[CN.DateDeposit] + comma +
                                row[CN.EmployeeNo] + comma +
                                row[CN.CodeDescription] + comma +
                                row[CN.Description] + comma +
                               ((decimal)row[CN.DepositValue]).ToString(DecimalPlaces).Replace(",", "") + comma +
                                row[CN.Reference] + Environment.NewLine;


                            blob = System.Text.Encoding.UTF8.GetBytes(line);
                            fs.Write(blob, 0, blob.Length);
                        }
                        fs.Close();
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
}