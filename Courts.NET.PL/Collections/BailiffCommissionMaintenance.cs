using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Data;
using System.Web.Services.Protocols;
using STL.PL.WS1;
using STL.PL.WS2;
using STL.PL.WS3;
using STL.PL.WS4;
using STL.PL.WS5;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using System.Collections.Specialized;
using System.Xml;
using STL.Common.Constants.Elements;
using STL.Common.Constants.Tags;
using Crownwood.Magic.Menus;
using STL.Common.Constants.ItemTypes;
using STL.Common.Static;
using System.Text;
using Blue.Cosacs.Shared;

namespace STL.PL
{
    /// <summary>
    /// Maintenance of commission and collection rates. Two tabs are provided
    /// to maintain a set of default rates and to maintain rates defined for
    /// specific employees. The rates are split by the status code for an account
    /// (typically 3, 4, 5 and 6), and by the activity (Whole Payment,
    /// Part Payment, Repossession).
    /// For each status code and activity different rates are defined for the 
    /// collection fee (charged to the customer account), for the commission amount
    /// (paid to the Bailiff) and for a repossession amount (paid to the Bailiff).
    /// </summary>
    public class BailiffCommissionMaintenance : CommonForm
    {
        private string _errorTxt = "";
        //private bool _staticLoaded = false;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.GroupBox groupBox2;
        private Crownwood.Magic.Controls.TabControl tcMain;
        private Crownwood.Magic.Controls.TabPage tpDefaultCommission;
        private System.Windows.Forms.ComboBox drpEmpCatCommBasis;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox drpCActivity;
        private System.Windows.Forms.ComboBox drpCStatus;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown numCMin;
        private System.Windows.Forms.NumericUpDown numCMax;
        private System.Windows.Forms.CheckBox chkCDebit;
        private System.Windows.Forms.NumericUpDown numCArrearsAllo;
        private System.Windows.Forms.NumericUpDown numCArreRepossPer;
        private System.Windows.Forms.NumericUpDown numCRepossPer;
        private System.Windows.Forms.NumericUpDown numCCommPer;
        private System.Windows.Forms.Button btnSaveCommBasis;
        private System.Windows.Forms.Button btnDeleteCommBasis;
        private System.Windows.Forms.DataGrid dgDefaultCommBasis;
        private Crownwood.Magic.Controls.TabPage tpBailiffCommision;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox drpBActivity;
        private System.Windows.Forms.ComboBox drpBStatus;
        private System.Windows.Forms.NumericUpDown numBArrearsAllo;
        private System.Windows.Forms.NumericUpDown numBArreRepossPer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtBEmpNo;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.CheckBox chkBDebit;
        private System.Windows.Forms.NumericUpDown numBMin;
        private System.Windows.Forms.NumericUpDown numBRepossPer;
        private System.Windows.Forms.NumericUpDown numBMax;
        private System.Windows.Forms.NumericUpDown numBCommPer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button btnSaveBailComm;
        private System.Windows.Forms.Button btnDeleteBailComm;
        private System.Windows.Forms.DataGrid dgBailiffCommBasis;
        private System.Windows.Forms.ComboBox drpEmpName;
        private System.Windows.Forms.ComboBox drpEmpCatBailMain;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.NumericUpDown numCCollectionPc;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.NumericUpDown numBCollectionPc;
        private System.Windows.Forms.Button btnApplyAll;
        private Crownwood.Magic.Controls.TabPage tpCallerCommission;
        private DataGridView dgvCollectionCommn;
        private NumericUpDown numPcArrsColl;
        private NumericUpDown numPcOfWorklist;
        private Label lblPcOfWorklist;
        private Label lblMaxValColl;
        private NumericUpDown numNoOfCalls;
        private Label lblNoOfCalls;
        private NumericUpDown numMaxValColl;
        private NumericUpDown numNoOfDaysSinceAction;
        private Label lblTimeFrameDays;
        private NumericUpDown numCommOnArrsPaidPc;
        private Label lblCommOnArrearsPaid;
        private Label lblCommOnAmtPaid;
        private NumericUpDown numCommOnAmtPaidPc;
        private NumericUpDown numCommSetVal;
        private Label lblCommSetVal;
        private NumericUpDown numMinMnthsArrs;
        private NumericUpDown numMaxMnthsArrs;
        private Button btnDeleteCollCommn;
        private Button btnSaveCollCommn;
        private GroupBox grpCollCommRates;
        private Label lblPcArrsCol;
        private Label lblMinMnthsArrs;
        private Label lblMaxMnthsArrs;
        private Label lblMinValColl;
        private NumericUpDown numMinValColl;
        private Label lblPcOfCalls;
        private NumericUpDown numPcOfCalls;
        private NumericUpDown numMaxBal;
        private NumericUpDown numMinBal;
        private Label lblMinBal;
        private Button btnNewCollCommn;
        private GroupBox grpCollCommCriteria;
        private CheckedListBox checkedListBox1;
        private Label lblEmpType;
        private ComboBox drpEmpCollComm;
        private Label lblCommType;
        private Label lblMaxBal;
        private RadioButton rbTeam;
        private TextBox txtRuleName;
        private Label lblRuleDescription;
        private Label lblAction;
        private RadioButton rbIndividual;
        private CheckedListBox chkListActions;
        private IContainer components;

        //IP - 11/06/10 - CR1083 - Collection Commissions
        private int CollCommnIndex
        {
            get { return dgvCollectionCommn.CurrentRow.Index; }

        }


        //IP - 16/06/10 - CR1083 - Collection Commissions
        private bool NewRule;
        private int LastSavedCommnIndex;
        private Label lblTimeFrame;
        private NumericUpDown numTimeFrameDays;
        private NumericUpDown numCommOnFeePc;
        private Label lblCommOnFee;
        private DataGridViewTextBoxColumn ID;
        private DataGridViewTextBoxColumn RuleName;
        private DataGridViewTextBoxColumn CommissionType;
        private DataGridViewTextBoxColumn PcentArrearsColl;
        private DataGridViewTextBoxColumn PcentOfCalls;
        private DataGridViewTextBoxColumn PcentOfWorklist;
        private DataGridViewTextBoxColumn NoOfCalls;
        private DataGridViewTextBoxColumn NoOfDays;
        private DataGridViewTextBoxColumn TimeFrameDays;
        private DataGridViewTextBoxColumn MinBal;
        private DataGridViewTextBoxColumn MaxBal;
        private DataGridViewTextBoxColumn MinValColl;
        private DataGridViewTextBoxColumn MaxValColl;
        private DataGridViewTextBoxColumn MinMnthsArrears;
        private DataGridViewTextBoxColumn MaxMnthsArrears;
        private DataGridViewTextBoxColumn PcentCommOnArrears;
        private DataGridViewTextBoxColumn PcentCommOnPaid;
        private DataGridViewTextBoxColumn PcentCommOnFee;
        private DataGridViewTextBoxColumn CommSetVal;

        private DataView dvCollCommnActions = null; //IP - 15/06/10 - CR1083 - Collection Commissions

        public BailiffCommissionMaintenance(Form root, Form parent)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            InitialiseStaticData();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tcMain = new Crownwood.Magic.Controls.TabControl();
            this.tpBailiffCommision = new Crownwood.Magic.Controls.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numBCollectionPc = new System.Windows.Forms.NumericUpDown();
            this.label21 = new System.Windows.Forms.Label();
            this.drpBActivity = new System.Windows.Forms.ComboBox();
            this.drpBStatus = new System.Windows.Forms.ComboBox();
            this.numBArrearsAllo = new System.Windows.Forms.NumericUpDown();
            this.numBArreRepossPer = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.txtBEmpNo = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.chkBDebit = new System.Windows.Forms.CheckBox();
            this.numBMin = new System.Windows.Forms.NumericUpDown();
            this.numBRepossPer = new System.Windows.Forms.NumericUpDown();
            this.numBMax = new System.Windows.Forms.NumericUpDown();
            this.numBCommPer = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.btnSaveBailComm = new System.Windows.Forms.Button();
            this.btnDeleteBailComm = new System.Windows.Forms.Button();
            this.dgBailiffCommBasis = new System.Windows.Forms.DataGrid();
            this.drpEmpName = new System.Windows.Forms.ComboBox();
            this.drpEmpCatBailMain = new System.Windows.Forms.ComboBox();
            this.tpDefaultCommission = new Crownwood.Magic.Controls.TabPage();
            this.btnApplyAll = new System.Windows.Forms.Button();
            this.drpEmpCatCommBasis = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.numCCollectionPc = new System.Windows.Forms.NumericUpDown();
            this.drpCActivity = new System.Windows.Forms.ComboBox();
            this.drpCStatus = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.numCMin = new System.Windows.Forms.NumericUpDown();
            this.numCMax = new System.Windows.Forms.NumericUpDown();
            this.chkCDebit = new System.Windows.Forms.CheckBox();
            this.numCArrearsAllo = new System.Windows.Forms.NumericUpDown();
            this.numCArreRepossPer = new System.Windows.Forms.NumericUpDown();
            this.numCRepossPer = new System.Windows.Forms.NumericUpDown();
            this.numCCommPer = new System.Windows.Forms.NumericUpDown();
            this.btnSaveCommBasis = new System.Windows.Forms.Button();
            this.btnDeleteCommBasis = new System.Windows.Forms.Button();
            this.dgDefaultCommBasis = new System.Windows.Forms.DataGrid();
            this.tpCallerCommission = new Crownwood.Magic.Controls.TabPage();
            this.numPcArrsColl = new System.Windows.Forms.NumericUpDown();
            this.btnNewCollCommn = new System.Windows.Forms.Button();
            this.numNoOfDaysSinceAction = new System.Windows.Forms.NumericUpDown();
            this.numMinBal = new System.Windows.Forms.NumericUpDown();
            this.lblMinBal = new System.Windows.Forms.Label();
            this.numMinValColl = new System.Windows.Forms.NumericUpDown();
            this.grpCollCommRates = new System.Windows.Forms.GroupBox();
            this.numCommOnFeePc = new System.Windows.Forms.NumericUpDown();
            this.lblCommOnFee = new System.Windows.Forms.Label();
            this.lblCommOnArrearsPaid = new System.Windows.Forms.Label();
            this.numCommOnArrsPaidPc = new System.Windows.Forms.NumericUpDown();
            this.lblCommOnAmtPaid = new System.Windows.Forms.Label();
            this.numCommOnAmtPaidPc = new System.Windows.Forms.NumericUpDown();
            this.lblCommSetVal = new System.Windows.Forms.Label();
            this.numCommSetVal = new System.Windows.Forms.NumericUpDown();
            this.btnDeleteCollCommn = new System.Windows.Forms.Button();
            this.numPcOfCalls = new System.Windows.Forms.NumericUpDown();
            this.btnSaveCollCommn = new System.Windows.Forms.Button();
            this.numMinMnthsArrs = new System.Windows.Forms.NumericUpDown();
            this.lblTimeFrameDays = new System.Windows.Forms.Label();
            this.numNoOfCalls = new System.Windows.Forms.NumericUpDown();
            this.numPcOfWorklist = new System.Windows.Forms.NumericUpDown();
            this.lblPcOfWorklist = new System.Windows.Forms.Label();
            this.dgvCollectionCommn = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RuleName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CommissionType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PcentArrearsColl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PcentOfCalls = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PcentOfWorklist = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NoOfCalls = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NoOfDays = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimeFrameDays = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MinBal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxBal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MinValColl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxValColl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MinMnthsArrears = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxMnthsArrears = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PcentCommOnArrears = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PcentCommOnPaid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PcentCommOnFee = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CommSetVal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpCollCommCriteria = new System.Windows.Forms.GroupBox();
            this.lblTimeFrame = new System.Windows.Forms.Label();
            this.numTimeFrameDays = new System.Windows.Forms.NumericUpDown();
            this.lblMinMnthsArrs = new System.Windows.Forms.Label();
            this.lblMaxMnthsArrs = new System.Windows.Forms.Label();
            this.numMaxBal = new System.Windows.Forms.NumericUpDown();
            this.chkListActions = new System.Windows.Forms.CheckedListBox();
            this.lblMinValColl = new System.Windows.Forms.Label();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.lblPcArrsCol = new System.Windows.Forms.Label();
            this.lblEmpType = new System.Windows.Forms.Label();
            this.drpEmpCollComm = new System.Windows.Forms.ComboBox();
            this.lblCommType = new System.Windows.Forms.Label();
            this.lblMaxBal = new System.Windows.Forms.Label();
            this.lblPcOfCalls = new System.Windows.Forms.Label();
            this.rbTeam = new System.Windows.Forms.RadioButton();
            this.txtRuleName = new System.Windows.Forms.TextBox();
            this.lblRuleDescription = new System.Windows.Forms.Label();
            this.lblNoOfCalls = new System.Windows.Forms.Label();
            this.lblAction = new System.Windows.Forms.Label();
            this.numMaxMnthsArrs = new System.Windows.Forms.NumericUpDown();
            this.rbIndividual = new System.Windows.Forms.RadioButton();
            this.lblMaxValColl = new System.Windows.Forms.Label();
            this.numMaxValColl = new System.Windows.Forms.NumericUpDown();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox2.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tpBailiffCommision.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBCollectionPc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBArrearsAllo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBArreRepossPer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBRepossPer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBCommPer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgBailiffCommBasis)).BeginInit();
            this.tpDefaultCommission.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCCollectionPc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCArrearsAllo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCArreRepossPer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCRepossPer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCCommPer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgDefaultCommBasis)).BeginInit();
            this.tpCallerCommission.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPcArrsColl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNoOfDaysSinceAction)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinBal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinValColl)).BeginInit();
            this.grpCollCommRates.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCommOnFeePc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCommOnArrsPaidPc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCommOnAmtPaidPc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCommSetVal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPcOfCalls)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinMnthsArrs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNoOfCalls)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPcOfWorklist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCollectionCommn)).BeginInit();
            this.grpCollCommCriteria.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeFrameDays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxBal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxMnthsArrs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxValColl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.tcMain);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox2.Location = new System.Drawing.Point(8, -8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(776, 480);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            // 
            // tcMain
            // 
            this.tcMain.IDEPixelArea = true;
            this.tcMain.Location = new System.Drawing.Point(8, 16);
            this.tcMain.Name = "tcMain";
            this.tcMain.PositionTop = true;
            this.tcMain.SelectedIndex = 0;
            this.tcMain.SelectedTab = this.tpDefaultCommission;
            this.tcMain.Size = new System.Drawing.Size(776, 464);
            this.tcMain.TabIndex = 0;
            this.tcMain.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpDefaultCommission,
            this.tpBailiffCommision,
            this.tpCallerCommission});
            this.tcMain.SelectionChanged += new System.EventHandler(this.tcMain_SelectionChanged);
            // 
            // tpBailiffCommision
            // 
            this.tpBailiffCommision.Controls.Add(this.groupBox1);
            this.tpBailiffCommision.Controls.Add(this.dgBailiffCommBasis);
            this.tpBailiffCommision.Controls.Add(this.drpEmpName);
            this.tpBailiffCommision.Controls.Add(this.drpEmpCatBailMain);
            this.tpBailiffCommision.Location = new System.Drawing.Point(0, 25);
            this.tpBailiffCommision.Name = "tpBailiffCommision";
            this.tpBailiffCommision.Selected = false;
            this.tpBailiffCommision.Size = new System.Drawing.Size(776, 439);
            this.tpBailiffCommision.TabIndex = 4;
            this.tpBailiffCommision.Title = "Commission Maintenance";
            this.tpBailiffCommision.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numBCollectionPc);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.drpBActivity);
            this.groupBox1.Controls.Add(this.drpBStatus);
            this.groupBox1.Controls.Add(this.numBArrearsAllo);
            this.groupBox1.Controls.Add(this.numBArreRepossPer);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.txtBEmpNo);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.chkBDebit);
            this.groupBox1.Controls.Add(this.numBMin);
            this.groupBox1.Controls.Add(this.numBRepossPer);
            this.groupBox1.Controls.Add(this.numBMax);
            this.groupBox1.Controls.Add(this.numBCommPer);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.btnSaveBailComm);
            this.groupBox1.Controls.Add(this.btnDeleteBailComm);
            this.groupBox1.Location = new System.Drawing.Point(552, 64);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(216, 368);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // numBCollectionPc
            // 
            this.numBCollectionPc.DecimalPlaces = 2;
            this.numBCollectionPc.Location = new System.Drawing.Point(112, 104);
            this.numBCollectionPc.Name = "numBCollectionPc";
            this.numBCollectionPc.Size = new System.Drawing.Size(80, 23);
            this.numBCollectionPc.TabIndex = 120;
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(16, 104);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(88, 16);
            this.label21.TabIndex = 40;
            this.label21.Text = "Collection %";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpBActivity
            // 
            this.drpBActivity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBActivity.Items.AddRange(new object[] {
            "",
            "P",
            "R",
            "W"});
            this.drpBActivity.Location = new System.Drawing.Point(112, 72);
            this.drpBActivity.Name = "drpBActivity";
            this.drpBActivity.Size = new System.Drawing.Size(56, 23);
            this.drpBActivity.TabIndex = 110;
            // 
            // drpBStatus
            // 
            this.drpBStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBStatus.Items.AddRange(new object[] {
            "",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
            this.drpBStatus.Location = new System.Drawing.Point(112, 48);
            this.drpBStatus.MaxDropDownItems = 15;
            this.drpBStatus.Name = "drpBStatus";
            this.drpBStatus.Size = new System.Drawing.Size(56, 23);
            this.drpBStatus.TabIndex = 100;
            // 
            // numBArrearsAllo
            // 
            this.numBArrearsAllo.DecimalPlaces = 2;
            this.numBArrearsAllo.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.numBArrearsAllo.Location = new System.Drawing.Point(112, 176);
            this.numBArrearsAllo.Name = "numBArrearsAllo";
            this.numBArrearsAllo.Size = new System.Drawing.Size(80, 21);
            this.numBArrearsAllo.TabIndex = 150;
            // 
            // numBArreRepossPer
            // 
            this.numBArreRepossPer.DecimalPlaces = 2;
            this.numBArreRepossPer.Location = new System.Drawing.Point(112, 152);
            this.numBArreRepossPer.Name = "numBArreRepossPer";
            this.numBArreRepossPer.Size = new System.Drawing.Size(80, 23);
            this.numBArreRepossPer.TabIndex = 140;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.label4.Location = new System.Drawing.Point(8, 176);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 16);
            this.label4.TabIndex = 26;
            this.label4.Text = "Arrears at alloc %";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(8, 152);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(96, 15);
            this.label16.TabIndex = 25;
            this.label16.Text = "Arrears at repo %";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBEmpNo
            // 
            this.txtBEmpNo.Location = new System.Drawing.Point(112, 24);
            this.txtBEmpNo.Name = "txtBEmpNo";
            this.txtBEmpNo.ReadOnly = true;
            this.txtBEmpNo.Size = new System.Drawing.Size(80, 23);
            this.txtBEmpNo.TabIndex = 0;
            this.txtBEmpNo.TabStop = false;
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(16, 24);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(88, 16);
            this.label19.TabIndex = 23;
            this.label19.Text = "Bailiff number";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkBDebit
            // 
            this.chkBDebit.Location = new System.Drawing.Point(160, 280);
            this.chkBDebit.Name = "chkBDebit";
            this.chkBDebit.Size = new System.Drawing.Size(24, 16);
            this.chkBDebit.TabIndex = 190;
            // 
            // numBMin
            // 
            this.numBMin.Location = new System.Drawing.Point(112, 224);
            this.numBMin.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numBMin.Name = "numBMin";
            this.numBMin.Size = new System.Drawing.Size(80, 23);
            this.numBMin.TabIndex = 170;
            // 
            // numBRepossPer
            // 
            this.numBRepossPer.DecimalPlaces = 2;
            this.numBRepossPer.Location = new System.Drawing.Point(112, 200);
            this.numBRepossPer.Name = "numBRepossPer";
            this.numBRepossPer.Size = new System.Drawing.Size(80, 23);
            this.numBRepossPer.TabIndex = 160;
            // 
            // numBMax
            // 
            this.numBMax.Location = new System.Drawing.Point(112, 248);
            this.numBMax.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numBMax.Name = "numBMax";
            this.numBMax.Size = new System.Drawing.Size(80, 23);
            this.numBMax.TabIndex = 180;
            // 
            // numBCommPer
            // 
            this.numBCommPer.DecimalPlaces = 2;
            this.numBCommPer.Location = new System.Drawing.Point(112, 128);
            this.numBCommPer.Name = "numBCommPer";
            this.numBCommPer.Size = new System.Drawing.Size(80, 23);
            this.numBCommPer.TabIndex = 130;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 280);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 16);
            this.label3.TabIndex = 15;
            this.label3.Text = "Debit customer account";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(16, 128);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 16);
            this.label7.TabIndex = 7;
            this.label7.Text = "Commission %";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(48, 72);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(56, 16);
            this.label10.TabIndex = 6;
            this.label10.Text = "Activity";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(32, 48);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(72, 16);
            this.label14.TabIndex = 5;
            this.label14.Text = "Status code";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(16, 200);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(88, 16);
            this.label15.TabIndex = 13;
            this.label15.Text = "Repossession %";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(48, 248);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(56, 16);
            this.label17.TabIndex = 10;
            this.label17.Text = "Max value";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(40, 224);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(64, 16);
            this.label18.TabIndex = 8;
            this.label18.Text = "Min value";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSaveBailComm
            // 
            this.btnSaveBailComm.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSaveBailComm.Location = new System.Drawing.Point(32, 320);
            this.btnSaveBailComm.Name = "btnSaveBailComm";
            this.btnSaveBailComm.Size = new System.Drawing.Size(64, 24);
            this.btnSaveBailComm.TabIndex = 200;
            this.btnSaveBailComm.Text = "Save";
            this.btnSaveBailComm.Click += new System.EventHandler(this.btnSaveBailComm_Click);
            // 
            // btnDeleteBailComm
            // 
            this.btnDeleteBailComm.Location = new System.Drawing.Point(128, 320);
            this.btnDeleteBailComm.Name = "btnDeleteBailComm";
            this.btnDeleteBailComm.Size = new System.Drawing.Size(64, 24);
            this.btnDeleteBailComm.TabIndex = 0;
            this.btnDeleteBailComm.TabStop = false;
            this.btnDeleteBailComm.Text = "Delete";
            this.btnDeleteBailComm.Click += new System.EventHandler(this.btnDeleteBailComm_Click);
            // 
            // dgBailiffCommBasis
            // 
            this.dgBailiffCommBasis.CaptionText = "Commission Basis for Employees ";
            this.dgBailiffCommBasis.DataMember = "";
            this.dgBailiffCommBasis.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgBailiffCommBasis.Location = new System.Drawing.Point(8, 64);
            this.dgBailiffCommBasis.Name = "dgBailiffCommBasis";
            this.dgBailiffCommBasis.ReadOnly = true;
            this.dgBailiffCommBasis.Size = new System.Drawing.Size(536, 368);
            this.dgBailiffCommBasis.TabIndex = 0;
            this.dgBailiffCommBasis.TabStop = false;
            this.dgBailiffCommBasis.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgBailiffCommBasis_MouseUp);
            // 
            // drpEmpName
            // 
            this.drpEmpName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmpName.Location = new System.Drawing.Point(280, 24);
            this.drpEmpName.Name = "drpEmpName";
            this.drpEmpName.Size = new System.Drawing.Size(232, 23);
            this.drpEmpName.TabIndex = 0;
            this.drpEmpName.TabStop = false;
            this.drpEmpName.SelectionChangeCommitted += new System.EventHandler(this.drpEmpName_SelectionChangeCommitted);
            // 
            // drpEmpCatBailMain
            // 
            this.drpEmpCatBailMain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmpCatBailMain.Location = new System.Drawing.Point(16, 24);
            this.drpEmpCatBailMain.Name = "drpEmpCatBailMain";
            this.drpEmpCatBailMain.Size = new System.Drawing.Size(232, 23);
            this.drpEmpCatBailMain.TabIndex = 0;
            this.drpEmpCatBailMain.TabStop = false;
            this.drpEmpCatBailMain.SelectionChangeCommitted += new System.EventHandler(this.drpEmpCatBailMain_SelectionChangeCommitted);
            // 
            // tpDefaultCommission
            // 
            this.tpDefaultCommission.Controls.Add(this.btnApplyAll);
            this.tpDefaultCommission.Controls.Add(this.drpEmpCatCommBasis);
            this.tpDefaultCommission.Controls.Add(this.groupBox3);
            this.tpDefaultCommission.Controls.Add(this.dgDefaultCommBasis);
            this.tpDefaultCommission.Location = new System.Drawing.Point(0, 25);
            this.tpDefaultCommission.Name = "tpDefaultCommission";
            this.tpDefaultCommission.Size = new System.Drawing.Size(776, 439);
            this.tpDefaultCommission.TabIndex = 3;
            this.tpDefaultCommission.Title = "Commission Basis";
            // 
            // btnApplyAll
            // 
            this.btnApplyAll.Location = new System.Drawing.Point(632, 24);
            this.btnApplyAll.Name = "btnApplyAll";
            this.btnApplyAll.Size = new System.Drawing.Size(96, 24);
            this.btnApplyAll.TabIndex = 1;
            this.btnApplyAll.TabStop = false;
            this.btnApplyAll.Text = "Apply to All";
            this.btnApplyAll.Click += new System.EventHandler(this.btnApplyAll_Click);
            // 
            // drpEmpCatCommBasis
            // 
            this.drpEmpCatCommBasis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmpCatCommBasis.Location = new System.Drawing.Point(16, 24);
            this.drpEmpCatCommBasis.Name = "drpEmpCatCommBasis";
            this.drpEmpCatCommBasis.Size = new System.Drawing.Size(232, 23);
            this.drpEmpCatCommBasis.TabIndex = 0;
            this.drpEmpCatCommBasis.TabStop = false;
            this.drpEmpCatCommBasis.SelectionChangeCommitted += new System.EventHandler(this.drpEmpCatCommBasis_SelectionChangeCommitted);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label20);
            this.groupBox3.Controls.Add(this.numCCollectionPc);
            this.groupBox3.Controls.Add(this.drpCActivity);
            this.groupBox3.Controls.Add(this.drpCStatus);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.numCMin);
            this.groupBox3.Controls.Add(this.numCMax);
            this.groupBox3.Controls.Add(this.chkCDebit);
            this.groupBox3.Controls.Add(this.numCArrearsAllo);
            this.groupBox3.Controls.Add(this.numCArreRepossPer);
            this.groupBox3.Controls.Add(this.numCRepossPer);
            this.groupBox3.Controls.Add(this.numCCommPer);
            this.groupBox3.Controls.Add(this.btnSaveCommBasis);
            this.groupBox3.Controls.Add(this.btnDeleteCommBasis);
            this.groupBox3.Location = new System.Drawing.Point(552, 64);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(216, 368);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(16, 88);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(88, 16);
            this.label20.TabIndex = 39;
            this.label20.Text = "Collection %";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numCCollectionPc
            // 
            this.numCCollectionPc.DecimalPlaces = 2;
            this.numCCollectionPc.Location = new System.Drawing.Point(112, 88);
            this.numCCollectionPc.Name = "numCCollectionPc";
            this.numCCollectionPc.Size = new System.Drawing.Size(80, 23);
            this.numCCollectionPc.TabIndex = 120;
            // 
            // drpCActivity
            // 
            this.drpCActivity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCActivity.Items.AddRange(new object[] {
            "",
            "P",
            "R",
            "W"});
            this.drpCActivity.Location = new System.Drawing.Point(112, 56);
            this.drpCActivity.Name = "drpCActivity";
            this.drpCActivity.Size = new System.Drawing.Size(56, 23);
            this.drpCActivity.TabIndex = 110;
            // 
            // drpCStatus
            // 
            this.drpCStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCStatus.Items.AddRange(new object[] {
            "",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
            this.drpCStatus.Location = new System.Drawing.Point(112, 32);
            this.drpCStatus.MaxDropDownItems = 15;
            this.drpCStatus.Name = "drpCStatus";
            this.drpCStatus.Size = new System.Drawing.Size(56, 23);
            this.drpCStatus.TabIndex = 100;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(16, 272);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(128, 16);
            this.label13.TabIndex = 35;
            this.label13.Text = "Debit customer account";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.label1.Location = new System.Drawing.Point(8, 160);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 16);
            this.label1.TabIndex = 34;
            this.label1.Text = "Arrears at alloc %";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 136);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 15);
            this.label2.TabIndex = 33;
            this.label2.Text = "Arrears at repo %";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(16, 112);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 16);
            this.label5.TabIndex = 29;
            this.label5.Text = "Commission %";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(48, 56);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 16);
            this.label6.TabIndex = 28;
            this.label6.Text = "Activity";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(32, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 16);
            this.label8.TabIndex = 27;
            this.label8.Text = "Status code";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(16, 184);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 16);
            this.label9.TabIndex = 32;
            this.label9.Text = "Repossession %";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(48, 232);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 16);
            this.label11.TabIndex = 31;
            this.label11.Text = "Max value";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(40, 208);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(64, 16);
            this.label12.TabIndex = 30;
            this.label12.Text = "Min value";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numCMin
            // 
            this.numCMin.Location = new System.Drawing.Point(112, 208);
            this.numCMin.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numCMin.Name = "numCMin";
            this.numCMin.Size = new System.Drawing.Size(80, 23);
            this.numCMin.TabIndex = 170;
            // 
            // numCMax
            // 
            this.numCMax.Location = new System.Drawing.Point(112, 232);
            this.numCMax.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numCMax.Name = "numCMax";
            this.numCMax.Size = new System.Drawing.Size(80, 23);
            this.numCMax.TabIndex = 180;
            // 
            // chkCDebit
            // 
            this.chkCDebit.Location = new System.Drawing.Point(160, 272);
            this.chkCDebit.Name = "chkCDebit";
            this.chkCDebit.Size = new System.Drawing.Size(24, 16);
            this.chkCDebit.TabIndex = 190;
            // 
            // numCArrearsAllo
            // 
            this.numCArrearsAllo.DecimalPlaces = 2;
            this.numCArrearsAllo.Location = new System.Drawing.Point(112, 160);
            this.numCArrearsAllo.Name = "numCArrearsAllo";
            this.numCArrearsAllo.Size = new System.Drawing.Size(80, 23);
            this.numCArrearsAllo.TabIndex = 150;
            // 
            // numCArreRepossPer
            // 
            this.numCArreRepossPer.DecimalPlaces = 2;
            this.numCArreRepossPer.Location = new System.Drawing.Point(112, 136);
            this.numCArreRepossPer.Name = "numCArreRepossPer";
            this.numCArreRepossPer.Size = new System.Drawing.Size(80, 23);
            this.numCArreRepossPer.TabIndex = 140;
            // 
            // numCRepossPer
            // 
            this.numCRepossPer.DecimalPlaces = 2;
            this.numCRepossPer.Location = new System.Drawing.Point(112, 184);
            this.numCRepossPer.Name = "numCRepossPer";
            this.numCRepossPer.Size = new System.Drawing.Size(80, 23);
            this.numCRepossPer.TabIndex = 160;
            // 
            // numCCommPer
            // 
            this.numCCommPer.DecimalPlaces = 2;
            this.numCCommPer.Location = new System.Drawing.Point(112, 112);
            this.numCCommPer.Name = "numCCommPer";
            this.numCCommPer.Size = new System.Drawing.Size(80, 23);
            this.numCCommPer.TabIndex = 130;
            // 
            // btnSaveCommBasis
            // 
            this.btnSaveCommBasis.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSaveCommBasis.Location = new System.Drawing.Point(32, 320);
            this.btnSaveCommBasis.Name = "btnSaveCommBasis";
            this.btnSaveCommBasis.Size = new System.Drawing.Size(64, 24);
            this.btnSaveCommBasis.TabIndex = 200;
            this.btnSaveCommBasis.Text = "Save";
            this.btnSaveCommBasis.Click += new System.EventHandler(this.btnSaveCommBasis_Click);
            // 
            // btnDeleteCommBasis
            // 
            this.btnDeleteCommBasis.Location = new System.Drawing.Point(128, 320);
            this.btnDeleteCommBasis.Name = "btnDeleteCommBasis";
            this.btnDeleteCommBasis.Size = new System.Drawing.Size(64, 24);
            this.btnDeleteCommBasis.TabIndex = 0;
            this.btnDeleteCommBasis.TabStop = false;
            this.btnDeleteCommBasis.Text = "Delete";
            this.btnDeleteCommBasis.Click += new System.EventHandler(this.btnDeleteCommBasis_Click);
            // 
            // dgDefaultCommBasis
            // 
            this.dgDefaultCommBasis.CaptionText = "Default Commission Basis";
            this.dgDefaultCommBasis.DataMember = "";
            this.dgDefaultCommBasis.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgDefaultCommBasis.Location = new System.Drawing.Point(8, 64);
            this.dgDefaultCommBasis.Name = "dgDefaultCommBasis";
            this.dgDefaultCommBasis.ReadOnly = true;
            this.dgDefaultCommBasis.Size = new System.Drawing.Size(536, 368);
            this.dgDefaultCommBasis.TabIndex = 0;
            this.dgDefaultCommBasis.TabStop = false;
            this.dgDefaultCommBasis.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgDefaultCommBasis_MouseUp);
            // 
            // tpCallerCommission
            // 
            this.tpCallerCommission.Controls.Add(this.numPcArrsColl);
            this.tpCallerCommission.Controls.Add(this.btnNewCollCommn);
            this.tpCallerCommission.Controls.Add(this.numNoOfDaysSinceAction);
            this.tpCallerCommission.Controls.Add(this.numMinBal);
            this.tpCallerCommission.Controls.Add(this.lblMinBal);
            this.tpCallerCommission.Controls.Add(this.numMinValColl);
            this.tpCallerCommission.Controls.Add(this.grpCollCommRates);
            this.tpCallerCommission.Controls.Add(this.btnDeleteCollCommn);
            this.tpCallerCommission.Controls.Add(this.numPcOfCalls);
            this.tpCallerCommission.Controls.Add(this.btnSaveCollCommn);
            this.tpCallerCommission.Controls.Add(this.numMinMnthsArrs);
            this.tpCallerCommission.Controls.Add(this.lblTimeFrameDays);
            this.tpCallerCommission.Controls.Add(this.numNoOfCalls);
            this.tpCallerCommission.Controls.Add(this.numPcOfWorklist);
            this.tpCallerCommission.Controls.Add(this.lblPcOfWorklist);
            this.tpCallerCommission.Controls.Add(this.dgvCollectionCommn);
            this.tpCallerCommission.Controls.Add(this.grpCollCommCriteria);
            this.tpCallerCommission.Location = new System.Drawing.Point(0, 25);
            this.tpCallerCommission.Name = "tpCallerCommission";
            this.tpCallerCommission.Selected = false;
            this.tpCallerCommission.Size = new System.Drawing.Size(776, 439);
            this.tpCallerCommission.TabIndex = 5;
            this.tpCallerCommission.Title = "Caller Commission ";
            // 
            // numPcArrsColl
            // 
            this.numPcArrsColl.DecimalPlaces = 2;
            this.numPcArrsColl.Location = new System.Drawing.Point(158, 112);
            this.numPcArrsColl.Name = "numPcArrsColl";
            this.numPcArrsColl.Size = new System.Drawing.Size(80, 23);
            this.numPcArrsColl.TabIndex = 13;
            // 
            // btnNewCollCommn
            // 
            this.btnNewCollCommn.Location = new System.Drawing.Point(513, 282);
            this.btnNewCollCommn.Name = "btnNewCollCommn";
            this.btnNewCollCommn.Size = new System.Drawing.Size(75, 23);
            this.btnNewCollCommn.TabIndex = 46;
            this.btnNewCollCommn.Text = "New";
            this.btnNewCollCommn.UseVisualStyleBackColor = true;
            this.btnNewCollCommn.Click += new System.EventHandler(this.btnNewCollCommn_Click);
            // 
            // numNoOfDaysSinceAction
            // 
            this.numNoOfDaysSinceAction.Location = new System.Drawing.Point(157, 209);
            this.numNoOfDaysSinceAction.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numNoOfDaysSinceAction.Name = "numNoOfDaysSinceAction";
            this.numNoOfDaysSinceAction.Size = new System.Drawing.Size(80, 23);
            this.numNoOfDaysSinceAction.TabIndex = 25;
            this.numNoOfDaysSinceAction.ValueChanged += new System.EventHandler(this.numNoOfDays_ValueChanged);
            // 
            // numMinBal
            // 
            this.numMinBal.DecimalPlaces = 2;
            this.numMinBal.Location = new System.Drawing.Point(158, 233);
            this.numMinBal.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numMinBal.Name = "numMinBal";
            this.numMinBal.Size = new System.Drawing.Size(80, 23);
            this.numMinBal.TabIndex = 42;
            // 
            // lblMinBal
            // 
            this.lblMinBal.AutoSize = true;
            this.lblMinBal.Location = new System.Drawing.Point(7, 239);
            this.lblMinBal.Name = "lblMinBal";
            this.lblMinBal.Size = new System.Drawing.Size(72, 15);
            this.lblMinBal.TabIndex = 40;
            this.lblMinBal.Text = "Min Balance";
            // 
            // numMinValColl
            // 
            this.numMinValColl.DecimalPlaces = 2;
            this.numMinValColl.Location = new System.Drawing.Point(158, 257);
            this.numMinValColl.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numMinValColl.Name = "numMinValColl";
            this.numMinValColl.Size = new System.Drawing.Size(80, 23);
            this.numMinValColl.TabIndex = 21;
            // 
            // grpCollCommRates
            // 
            this.grpCollCommRates.Controls.Add(this.numCommOnFeePc);
            this.grpCollCommRates.Controls.Add(this.lblCommOnFee);
            this.grpCollCommRates.Controls.Add(this.lblCommOnArrearsPaid);
            this.grpCollCommRates.Controls.Add(this.numCommOnArrsPaidPc);
            this.grpCollCommRates.Controls.Add(this.lblCommOnAmtPaid);
            this.grpCollCommRates.Controls.Add(this.numCommOnAmtPaidPc);
            this.grpCollCommRates.Controls.Add(this.lblCommSetVal);
            this.grpCollCommRates.Controls.Add(this.numCommSetVal);
            this.grpCollCommRates.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.grpCollCommRates.Location = new System.Drawing.Point(513, 6);
            this.grpCollCommRates.Name = "grpCollCommRates";
            this.grpCollCommRates.Size = new System.Drawing.Size(253, 114);
            this.grpCollCommRates.TabIndex = 38;
            this.grpCollCommRates.TabStop = false;
            this.grpCollCommRates.Text = "Commission Rates";
            // 
            // numCommOnFeePc
            // 
            this.numCommOnFeePc.DecimalPlaces = 2;
            this.numCommOnFeePc.Location = new System.Drawing.Point(175, 59);
            this.numCommOnFeePc.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numCommOnFeePc.Name = "numCommOnFeePc";
            this.numCommOnFeePc.Size = new System.Drawing.Size(80, 23);
            this.numCommOnFeePc.TabIndex = 33;
            // 
            // lblCommOnFee
            // 
            this.lblCommOnFee.AutoSize = true;
            this.lblCommOnFee.Location = new System.Drawing.Point(23, 67);
            this.lblCommOnFee.Name = "lblCommOnFee";
            this.lblCommOnFee.Size = new System.Drawing.Size(95, 15);
            this.lblCommOnFee.TabIndex = 32;
            this.lblCommOnFee.Text = "Comm % on Fee";
            // 
            // lblCommOnArrearsPaid
            // 
            this.lblCommOnArrearsPaid.AutoSize = true;
            this.lblCommOnArrearsPaid.Location = new System.Drawing.Point(23, 19);
            this.lblCommOnArrearsPaid.Name = "lblCommOnArrearsPaid";
            this.lblCommOnArrearsPaid.Size = new System.Drawing.Size(140, 15);
            this.lblCommOnArrearsPaid.TabIndex = 26;
            this.lblCommOnArrearsPaid.Text = "Comm % on Arrears Paid";
            // 
            // numCommOnArrsPaidPc
            // 
            this.numCommOnArrsPaidPc.DecimalPlaces = 2;
            this.numCommOnArrsPaidPc.Location = new System.Drawing.Point(175, 11);
            this.numCommOnArrsPaidPc.Name = "numCommOnArrsPaidPc";
            this.numCommOnArrsPaidPc.Size = new System.Drawing.Size(80, 23);
            this.numCommOnArrsPaidPc.TabIndex = 27;
            // 
            // lblCommOnAmtPaid
            // 
            this.lblCommOnAmtPaid.AutoSize = true;
            this.lblCommOnAmtPaid.Location = new System.Drawing.Point(23, 43);
            this.lblCommOnAmtPaid.Name = "lblCommOnAmtPaid";
            this.lblCommOnAmtPaid.Size = new System.Drawing.Size(147, 15);
            this.lblCommOnAmtPaid.TabIndex = 28;
            this.lblCommOnAmtPaid.Text = "Comm % on Amount Paid";
            // 
            // numCommOnAmtPaidPc
            // 
            this.numCommOnAmtPaidPc.DecimalPlaces = 2;
            this.numCommOnAmtPaidPc.Location = new System.Drawing.Point(175, 35);
            this.numCommOnAmtPaidPc.Name = "numCommOnAmtPaidPc";
            this.numCommOnAmtPaidPc.Size = new System.Drawing.Size(80, 23);
            this.numCommOnAmtPaidPc.TabIndex = 29;
            this.numCommOnAmtPaidPc.ValueChanged += new System.EventHandler(this.numCommOnAmtPaid_ValueChanged);
            // 
            // lblCommSetVal
            // 
            this.lblCommSetVal.AutoSize = true;
            this.lblCommSetVal.Location = new System.Drawing.Point(23, 91);
            this.lblCommSetVal.Name = "lblCommSetVal";
            this.lblCommSetVal.Size = new System.Drawing.Size(95, 15);
            this.lblCommSetVal.TabIndex = 30;
            this.lblCommSetVal.Text = "Comm Set Value";
            // 
            // numCommSetVal
            // 
            this.numCommSetVal.Location = new System.Drawing.Point(175, 83);
            this.numCommSetVal.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numCommSetVal.Name = "numCommSetVal";
            this.numCommSetVal.Size = new System.Drawing.Size(80, 23);
            this.numCommSetVal.TabIndex = 31;
            // 
            // btnDeleteCollCommn
            // 
            this.btnDeleteCollCommn.Location = new System.Drawing.Point(691, 282);
            this.btnDeleteCollCommn.Name = "btnDeleteCollCommn";
            this.btnDeleteCollCommn.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteCollCommn.TabIndex = 37;
            this.btnDeleteCollCommn.Text = "Delete";
            this.btnDeleteCollCommn.UseVisualStyleBackColor = true;
            this.btnDeleteCollCommn.Click += new System.EventHandler(this.btnDeleteCollCommn_Click);
            // 
            // numPcOfCalls
            // 
            this.numPcOfCalls.DecimalPlaces = 2;
            this.numPcOfCalls.Location = new System.Drawing.Point(158, 136);
            this.numPcOfCalls.Name = "numPcOfCalls";
            this.numPcOfCalls.Size = new System.Drawing.Size(80, 23);
            this.numPcOfCalls.TabIndex = 15;
            // 
            // btnSaveCollCommn
            // 
            this.btnSaveCollCommn.Location = new System.Drawing.Point(602, 282);
            this.btnSaveCollCommn.Name = "btnSaveCollCommn";
            this.btnSaveCollCommn.Size = new System.Drawing.Size(75, 23);
            this.btnSaveCollCommn.TabIndex = 36;
            this.btnSaveCollCommn.Text = "Save";
            this.btnSaveCollCommn.UseVisualStyleBackColor = true;
            this.btnSaveCollCommn.Click += new System.EventHandler(this.btnSaveCollCommn_Click);
            // 
            // numMinMnthsArrs
            // 
            this.numMinMnthsArrs.Location = new System.Drawing.Point(158, 281);
            this.numMinMnthsArrs.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numMinMnthsArrs.Name = "numMinMnthsArrs";
            this.numMinMnthsArrs.Size = new System.Drawing.Size(80, 23);
            this.numMinMnthsArrs.TabIndex = 33;
            // 
            // lblTimeFrameDays
            // 
            this.lblTimeFrameDays.AutoSize = true;
            this.lblTimeFrameDays.Location = new System.Drawing.Point(7, 214);
            this.lblTimeFrameDays.Name = "lblTimeFrameDays";
            this.lblTimeFrameDays.Size = new System.Drawing.Size(151, 15);
            this.lblTimeFrameDays.TabIndex = 24;
            this.lblTimeFrameDays.Text = "Payment Days Since Action";
            // 
            // numNoOfCalls
            // 
            this.numNoOfCalls.Location = new System.Drawing.Point(158, 184);
            this.numNoOfCalls.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numNoOfCalls.Name = "numNoOfCalls";
            this.numNoOfCalls.Size = new System.Drawing.Size(80, 23);
            this.numNoOfCalls.TabIndex = 19;
            // 
            // numPcOfWorklist
            // 
            this.numPcOfWorklist.DecimalPlaces = 2;
            this.numPcOfWorklist.Location = new System.Drawing.Point(158, 160);
            this.numPcOfWorklist.Name = "numPcOfWorklist";
            this.numPcOfWorklist.Size = new System.Drawing.Size(80, 23);
            this.numPcOfWorklist.TabIndex = 17;
            this.numPcOfWorklist.Leave += new System.EventHandler(this.numPcOfWorklist_Leave);
            // 
            // lblPcOfWorklist
            // 
            this.lblPcOfWorklist.AutoSize = true;
            this.lblPcOfWorklist.Location = new System.Drawing.Point(7, 166);
            this.lblPcOfWorklist.Name = "lblPcOfWorklist";
            this.lblPcOfWorklist.Size = new System.Drawing.Size(77, 15);
            this.lblPcOfWorklist.TabIndex = 16;
            this.lblPcOfWorklist.Text = "% of Worklist";
            // 
            // dgvCollectionCommn
            // 
            this.dgvCollectionCommn.AllowUserToAddRows = false;
            this.dgvCollectionCommn.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCollectionCommn.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.RuleName,
            this.CommissionType,
            this.PcentArrearsColl,
            this.PcentOfCalls,
            this.PcentOfWorklist,
            this.NoOfCalls,
            this.NoOfDays,
            this.TimeFrameDays,
            this.MinBal,
            this.MaxBal,
            this.MinValColl,
            this.MaxValColl,
            this.MinMnthsArrears,
            this.MaxMnthsArrears,
            this.PcentCommOnArrears,
            this.PcentCommOnPaid,
            this.PcentCommOnFee,
            this.CommSetVal});
            this.dgvCollectionCommn.Location = new System.Drawing.Point(3, 309);
            this.dgvCollectionCommn.Name = "dgvCollectionCommn";
            this.dgvCollectionCommn.ReadOnly = true;
            this.dgvCollectionCommn.Size = new System.Drawing.Size(759, 124);
            this.dgvCollectionCommn.TabIndex = 4;
            this.dgvCollectionCommn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvCollectionCommn_MouseUp);
            // 
            // ID
            // 
            this.ID.DataPropertyName = "ID";
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Visible = false;
            // 
            // RuleName
            // 
            this.RuleName.DataPropertyName = "RuleName";
            this.RuleName.HeaderText = "Rule Name";
            this.RuleName.Name = "RuleName";
            this.RuleName.ReadOnly = true;
            // 
            // CommissionType
            // 
            this.CommissionType.DataPropertyName = "CommissionType";
            this.CommissionType.HeaderText = "Commission Type";
            this.CommissionType.Name = "CommissionType";
            this.CommissionType.ReadOnly = true;
            // 
            // PcentArrearsColl
            // 
            this.PcentArrearsColl.DataPropertyName = "PcentArrearsColl";
            this.PcentArrearsColl.HeaderText = "% Arrears Collected";
            this.PcentArrearsColl.Name = "PcentArrearsColl";
            this.PcentArrearsColl.ReadOnly = true;
            // 
            // PcentOfCalls
            // 
            this.PcentOfCalls.DataPropertyName = "PcentOfCalls";
            this.PcentOfCalls.HeaderText = "% of Calls";
            this.PcentOfCalls.Name = "PcentOfCalls";
            this.PcentOfCalls.ReadOnly = true;
            // 
            // PcentOfWorklist
            // 
            this.PcentOfWorklist.DataPropertyName = "PcentOfWorklist";
            this.PcentOfWorklist.HeaderText = "% of Worklist";
            this.PcentOfWorklist.Name = "PcentOfWorklist";
            this.PcentOfWorklist.ReadOnly = true;
            // 
            // NoOfCalls
            // 
            this.NoOfCalls.DataPropertyName = "NoOfCalls";
            this.NoOfCalls.HeaderText = "No of Calls";
            this.NoOfCalls.Name = "NoOfCalls";
            this.NoOfCalls.ReadOnly = true;
            // 
            // NoOfDays
            // 
            this.NoOfDays.DataPropertyName = "NoOfDaysSinceAction";
            this.NoOfDays.HeaderText = "Payment Days Since Action";
            this.NoOfDays.Name = "NoOfDays";
            this.NoOfDays.ReadOnly = true;
            // 
            // TimeFrameDays
            // 
            this.TimeFrameDays.DataPropertyName = "TimeFrameDays";
            this.TimeFrameDays.HeaderText = "Time Frame (Days)";
            this.TimeFrameDays.Name = "TimeFrameDays";
            this.TimeFrameDays.ReadOnly = true;
            // 
            // MinBal
            // 
            this.MinBal.DataPropertyName = "MinBal";
            this.MinBal.HeaderText = "Min Balance";
            this.MinBal.Name = "MinBal";
            this.MinBal.ReadOnly = true;
            // 
            // MaxBal
            // 
            this.MaxBal.DataPropertyName = "MaxBal";
            this.MaxBal.HeaderText = "Max Balance";
            this.MaxBal.Name = "MaxBal";
            this.MaxBal.ReadOnly = true;
            // 
            // MinValColl
            // 
            this.MinValColl.DataPropertyName = "MinValColl";
            this.MinValColl.HeaderText = "Min Value Collected";
            this.MinValColl.Name = "MinValColl";
            this.MinValColl.ReadOnly = true;
            // 
            // MaxValColl
            // 
            this.MaxValColl.DataPropertyName = "MaxValColl";
            this.MaxValColl.HeaderText = "Max Value Collected";
            this.MaxValColl.Name = "MaxValColl";
            this.MaxValColl.ReadOnly = true;
            // 
            // MinMnthsArrears
            // 
            this.MinMnthsArrears.DataPropertyName = "MinMnthsArrears";
            this.MinMnthsArrears.HeaderText = "Min Months In Arrears";
            this.MinMnthsArrears.Name = "MinMnthsArrears";
            this.MinMnthsArrears.ReadOnly = true;
            // 
            // MaxMnthsArrears
            // 
            this.MaxMnthsArrears.DataPropertyName = "MaxMnthsArrears";
            this.MaxMnthsArrears.HeaderText = "Max Months In Arrears";
            this.MaxMnthsArrears.Name = "MaxMnthsArrears";
            this.MaxMnthsArrears.ReadOnly = true;
            // 
            // PcentCommOnArrears
            // 
            this.PcentCommOnArrears.DataPropertyName = "PcentCommOnArrears";
            this.PcentCommOnArrears.HeaderText = "Comm % on Arrears Paid";
            this.PcentCommOnArrears.Name = "PcentCommOnArrears";
            this.PcentCommOnArrears.ReadOnly = true;
            // 
            // PcentCommOnPaid
            // 
            this.PcentCommOnPaid.DataPropertyName = "PcentCommOnAmtPaid";
            this.PcentCommOnPaid.HeaderText = "Comm % on Amount Paid";
            this.PcentCommOnPaid.Name = "PcentCommOnPaid";
            this.PcentCommOnPaid.ReadOnly = true;
            // 
            // PcentCommOnFee
            // 
            this.PcentCommOnFee.DataPropertyName = "PcentCommOnFee";
            this.PcentCommOnFee.HeaderText = "Comm % on Fee";
            this.PcentCommOnFee.Name = "PcentCommOnFee";
            this.PcentCommOnFee.ReadOnly = true;
            // 
            // CommSetVal
            // 
            this.CommSetVal.DataPropertyName = "CommSetVal";
            this.CommSetVal.HeaderText = "Comm Set Value";
            this.CommSetVal.Name = "CommSetVal";
            this.CommSetVal.ReadOnly = true;
            // 
            // grpCollCommCriteria
            // 
            this.grpCollCommCriteria.Controls.Add(this.lblTimeFrame);
            this.grpCollCommCriteria.Controls.Add(this.numTimeFrameDays);
            this.grpCollCommCriteria.Controls.Add(this.lblMinMnthsArrs);
            this.grpCollCommCriteria.Controls.Add(this.lblMaxMnthsArrs);
            this.grpCollCommCriteria.Controls.Add(this.numMaxBal);
            this.grpCollCommCriteria.Controls.Add(this.chkListActions);
            this.grpCollCommCriteria.Controls.Add(this.lblMinValColl);
            this.grpCollCommCriteria.Controls.Add(this.checkedListBox1);
            this.grpCollCommCriteria.Controls.Add(this.lblPcArrsCol);
            this.grpCollCommCriteria.Controls.Add(this.lblEmpType);
            this.grpCollCommCriteria.Controls.Add(this.drpEmpCollComm);
            this.grpCollCommCriteria.Controls.Add(this.lblCommType);
            this.grpCollCommCriteria.Controls.Add(this.lblMaxBal);
            this.grpCollCommCriteria.Controls.Add(this.lblPcOfCalls);
            this.grpCollCommCriteria.Controls.Add(this.rbTeam);
            this.grpCollCommCriteria.Controls.Add(this.txtRuleName);
            this.grpCollCommCriteria.Controls.Add(this.lblRuleDescription);
            this.grpCollCommCriteria.Controls.Add(this.lblNoOfCalls);
            this.grpCollCommCriteria.Controls.Add(this.lblAction);
            this.grpCollCommCriteria.Controls.Add(this.numMaxMnthsArrs);
            this.grpCollCommCriteria.Controls.Add(this.rbIndividual);
            this.grpCollCommCriteria.Controls.Add(this.lblMaxValColl);
            this.grpCollCommCriteria.Controls.Add(this.numMaxValColl);
            this.grpCollCommCriteria.Location = new System.Drawing.Point(5, 5);
            this.grpCollCommCriteria.Name = "grpCollCommCriteria";
            this.grpCollCommCriteria.Size = new System.Drawing.Size(507, 304);
            this.grpCollCommCriteria.TabIndex = 39;
            this.grpCollCommCriteria.TabStop = false;
            this.grpCollCommCriteria.Text = "Criteria";
            this.grpCollCommCriteria.Enter += new System.EventHandler(this.grpCollCommCriteria_Enter);
            // 
            // lblTimeFrame
            // 
            this.lblTimeFrame.AutoSize = true;
            this.lblTimeFrame.Location = new System.Drawing.Point(239, 209);
            this.lblTimeFrame.Name = "lblTimeFrame";
            this.lblTimeFrame.Size = new System.Drawing.Size(106, 15);
            this.lblTimeFrame.TabIndex = 50;
            this.lblTimeFrame.Text = "Time Frame (Days)";
            // 
            // numTimeFrameDays
            // 
            this.numTimeFrameDays.Location = new System.Drawing.Point(407, 200);
            this.numTimeFrameDays.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numTimeFrameDays.Name = "numTimeFrameDays";
            this.numTimeFrameDays.Size = new System.Drawing.Size(80, 23);
            this.numTimeFrameDays.TabIndex = 49;
            // 
            // lblMinMnthsArrs
            // 
            this.lblMinMnthsArrs.AutoSize = true;
            this.lblMinMnthsArrs.Location = new System.Drawing.Point(1, 283);
            this.lblMinMnthsArrs.Name = "lblMinMnthsArrs";
            this.lblMinMnthsArrs.Size = new System.Drawing.Size(125, 15);
            this.lblMinMnthsArrs.TabIndex = 32;
            this.lblMinMnthsArrs.Text = "Min Months In Arrears";
            // 
            // lblMaxMnthsArrs
            // 
            this.lblMaxMnthsArrs.AutoSize = true;
            this.lblMaxMnthsArrs.Location = new System.Drawing.Point(239, 282);
            this.lblMaxMnthsArrs.Name = "lblMaxMnthsArrs";
            this.lblMaxMnthsArrs.Size = new System.Drawing.Size(126, 15);
            this.lblMaxMnthsArrs.TabIndex = 34;
            this.lblMaxMnthsArrs.Text = "Max Months In Arrears";
            // 
            // numMaxBal
            // 
            this.numMaxBal.DecimalPlaces = 2;
            this.numMaxBal.Location = new System.Drawing.Point(407, 225);
            this.numMaxBal.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numMaxBal.Name = "numMaxBal";
            this.numMaxBal.Size = new System.Drawing.Size(80, 23);
            this.numMaxBal.TabIndex = 44;
            // 
            // chkListActions
            // 
            this.chkListActions.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.chkListActions.FormattingEnabled = true;
            this.chkListActions.Location = new System.Drawing.Point(239, 119);
            this.chkListActions.Name = "chkListActions";
            this.chkListActions.Size = new System.Drawing.Size(249, 68);
            this.chkListActions.TabIndex = 48;
            // 
            // lblMinValColl
            // 
            this.lblMinValColl.AutoSize = true;
            this.lblMinValColl.Location = new System.Drawing.Point(2, 260);
            this.lblMinValColl.Name = "lblMinValColl";
            this.lblMinValColl.Size = new System.Drawing.Size(113, 15);
            this.lblMinValColl.TabIndex = 20;
            this.lblMinValColl.Text = "Min Value Collected";
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(-118, -246);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(120, 94);
            this.checkedListBox1.TabIndex = 47;
            // 
            // lblPcArrsCol
            // 
            this.lblPcArrsCol.AutoSize = true;
            this.lblPcArrsCol.Location = new System.Drawing.Point(2, 115);
            this.lblPcArrsCol.Name = "lblPcArrsCol";
            this.lblPcArrsCol.Size = new System.Drawing.Size(110, 15);
            this.lblPcArrsCol.TabIndex = 12;
            this.lblPcArrsCol.Text = "% Arrears Collected";
            // 
            // lblEmpType
            // 
            this.lblEmpType.AutoSize = true;
            this.lblEmpType.Location = new System.Drawing.Point(1, 14);
            this.lblEmpType.Name = "lblEmpType";
            this.lblEmpType.Size = new System.Drawing.Size(88, 15);
            this.lblEmpType.TabIndex = 4;
            this.lblEmpType.Text = "Employee Type";
            // 
            // drpEmpCollComm
            // 
            this.drpEmpCollComm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmpCollComm.FormattingEnabled = true;
            this.drpEmpCollComm.Location = new System.Drawing.Point(153, 11);
            this.drpEmpCollComm.Name = "drpEmpCollComm";
            this.drpEmpCollComm.Size = new System.Drawing.Size(232, 23);
            this.drpEmpCollComm.TabIndex = 3;
            this.drpEmpCollComm.SelectedIndexChanged += new System.EventHandler(this.drpEmpCollComm_SelectedIndexChanged);
            // 
            // lblCommType
            // 
            this.lblCommType.AutoSize = true;
            this.lblCommType.Location = new System.Drawing.Point(2, 69);
            this.lblCommType.Name = "lblCommType";
            this.lblCommType.Size = new System.Drawing.Size(103, 15);
            this.lblCommType.TabIndex = 9;
            this.lblCommType.Text = "Commission Type";
            // 
            // lblMaxBal
            // 
            this.lblMaxBal.AutoSize = true;
            this.lblMaxBal.Location = new System.Drawing.Point(239, 234);
            this.lblMaxBal.Name = "lblMaxBal";
            this.lblMaxBal.Size = new System.Drawing.Size(73, 15);
            this.lblMaxBal.TabIndex = 43;
            this.lblMaxBal.Text = "Max Balance";
            // 
            // lblPcOfCalls
            // 
            this.lblPcOfCalls.AutoSize = true;
            this.lblPcOfCalls.Location = new System.Drawing.Point(2, 139);
            this.lblPcOfCalls.Name = "lblPcOfCalls";
            this.lblPcOfCalls.Size = new System.Drawing.Size(59, 15);
            this.lblPcOfCalls.TabIndex = 14;
            this.lblPcOfCalls.Text = "% of Calls";
            // 
            // rbTeam
            // 
            this.rbTeam.AutoSize = true;
            this.rbTeam.Location = new System.Drawing.Point(230, 69);
            this.rbTeam.Name = "rbTeam";
            this.rbTeam.Size = new System.Drawing.Size(55, 19);
            this.rbTeam.TabIndex = 8;
            this.rbTeam.Text = "Team";
            this.rbTeam.UseVisualStyleBackColor = true;
            // 
            // txtRuleName
            // 
            this.txtRuleName.Enabled = false;
            this.txtRuleName.Location = new System.Drawing.Point(153, 40);
            this.txtRuleName.MaxLength = 50;
            this.txtRuleName.Name = "txtRuleName";
            this.txtRuleName.Size = new System.Drawing.Size(232, 23);
            this.txtRuleName.TabIndex = 6;
            // 
            // lblRuleDescription
            // 
            this.lblRuleDescription.AutoSize = true;
            this.lblRuleDescription.Location = new System.Drawing.Point(2, 48);
            this.lblRuleDescription.Name = "lblRuleDescription";
            this.lblRuleDescription.Size = new System.Drawing.Size(65, 15);
            this.lblRuleDescription.TabIndex = 5;
            this.lblRuleDescription.Text = "Rule Name";
            // 
            // lblNoOfCalls
            // 
            this.lblNoOfCalls.AutoSize = true;
            this.lblNoOfCalls.Location = new System.Drawing.Point(1, 187);
            this.lblNoOfCalls.Name = "lblNoOfCalls";
            this.lblNoOfCalls.Size = new System.Drawing.Size(65, 15);
            this.lblNoOfCalls.TabIndex = 18;
            this.lblNoOfCalls.Text = "No of Calls";
            // 
            // lblAction
            // 
            this.lblAction.AutoSize = true;
            this.lblAction.Location = new System.Drawing.Point(239, 100);
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new System.Drawing.Size(77, 15);
            this.lblAction.TabIndex = 10;
            this.lblAction.Text = "Action Taken";
            // 
            // numMaxMnthsArrs
            // 
            this.numMaxMnthsArrs.Location = new System.Drawing.Point(407, 275);
            this.numMaxMnthsArrs.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numMaxMnthsArrs.Name = "numMaxMnthsArrs";
            this.numMaxMnthsArrs.Size = new System.Drawing.Size(80, 23);
            this.numMaxMnthsArrs.TabIndex = 35;
            // 
            // rbIndividual
            // 
            this.rbIndividual.AutoSize = true;
            this.rbIndividual.Checked = true;
            this.rbIndividual.Location = new System.Drawing.Point(154, 69);
            this.rbIndividual.Name = "rbIndividual";
            this.rbIndividual.Size = new System.Drawing.Size(77, 19);
            this.rbIndividual.TabIndex = 7;
            this.rbIndividual.TabStop = true;
            this.rbIndividual.Text = "Individual";
            this.rbIndividual.UseVisualStyleBackColor = true;
            // 
            // lblMaxValColl
            // 
            this.lblMaxValColl.AutoSize = true;
            this.lblMaxValColl.Location = new System.Drawing.Point(239, 260);
            this.lblMaxValColl.Name = "lblMaxValColl";
            this.lblMaxValColl.Size = new System.Drawing.Size(114, 15);
            this.lblMaxValColl.TabIndex = 22;
            this.lblMaxValColl.Text = "Max Value Collected";
            // 
            // numMaxValColl
            // 
            this.numMaxValColl.DecimalPlaces = 2;
            this.numMaxValColl.Location = new System.Drawing.Point(407, 250);
            this.numMaxValColl.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numMaxValColl.Name = "numMaxValColl";
            this.numMaxValColl.Size = new System.Drawing.Size(80, 23);
            this.numMaxValColl.TabIndex = 23;
            this.numMaxValColl.ValueChanged += new System.EventHandler(this.numMaxValColl_ValueChanged);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // BailiffCommissionMaintenance
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox2);
            this.Name = "BailiffCommissionMaintenance";
            this.Text = "Collection Commission Maintenance";
            this.groupBox2.ResumeLayout(false);
            this.tcMain.ResumeLayout(false);
            this.tpBailiffCommision.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBCollectionPc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBArrearsAllo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBArreRepossPer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBRepossPer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBCommPer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgBailiffCommBasis)).EndInit();
            this.tpDefaultCommission.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numCCollectionPc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCArrearsAllo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCArreRepossPer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCRepossPer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCCommPer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgDefaultCommBasis)).EndInit();
            this.tpCallerCommission.ResumeLayout(false);
            this.tpCallerCommission.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPcArrsColl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNoOfDaysSinceAction)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinBal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinValColl)).EndInit();
            this.grpCollCommRates.ResumeLayout(false);
            this.grpCollCommRates.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCommOnFeePc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCommOnArrsPaidPc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCommOnAmtPaidPc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCommSetVal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPcOfCalls)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinMnthsArrs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNoOfCalls)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPcOfWorklist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCollectionCommn)).EndInit();
            this.grpCollCommCriteria.ResumeLayout(false);
            this.grpCollCommCriteria.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeFrameDays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxBal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxMnthsArrs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxValColl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void InitialiseStaticData()
        {
            try
            {
                Function = "CommissionMaintenance::InitialiseStaticData";
                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                // Need two copies of the drop down data source, otherwise
                // both drop downs will change selection at the same time.
                StringCollection empTypes1 = new StringCollection();
                StringCollection empTypes2 = new StringCollection();


                if (StaticData.Tables[TN.EmployeeTypes] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.EmployeeTypes,
                        new string[] { "ET1", "L" }));

                //IP - 09/06/10 - CR1083
                if (StaticData.Tables[TN.CollectionActions] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CollectionActions, new string[] { "FUA", "L" }));

                if (dropDowns.DocumentElement.ChildNodes.Count > 0)
                {
                    DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out _errorTxt);
                    if (_errorTxt.Length > 0)
                        ShowError(_errorTxt);
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                            StaticData.Tables[dt.TableName] = dt;
                    }
                }

                foreach (DataRow row in CollectionsManager.RolesGet((int)CosacsPermissionEnum.Bailiff).Tables[0].Rows)
                {
                        string str = string.Format("{0} : {1}",row[0],row[1]);
                        empTypes1.Add(str.ToUpper());
                        empTypes2.Add(str.ToUpper());
                }

                //#11243
                foreach (DataRow row in CollectionsManager.RolesGet((int)CosacsPermissionEnum.TelephoneCaller).Tables[0].Rows)
                {
                    string str = string.Format("{0} : {1}", row[0], row[1]);
                    if (!(empTypes1.Contains(str.ToUpper())))
                    {
                        empTypes1.Add(str.ToUpper());
                        empTypes2.Add(str.ToUpper());
                    }
                }

                //IP - 09/06/10 - CR1083 - Collection Commissions
                DataView dvCollectionActions = ((DataTable)StaticData.Tables[TN.CollectionActions]).DefaultView;
                dvCollectionActions.Sort = CN.Code + " ASC ";

                chkListActions.Items.Add("ALL");

                foreach (DataRowView drv in dvCollectionActions)
                {
                    string str = (string)drv[CN.Code] + " : " + (string)drv[CN.CodeDescript];
                    chkListActions.Items.Add(str.ToUpper());

                }

                drpEmpCatBailMain.DataSource = empTypes1;
                drpEmpCatCommBasis.DataSource = empTypes2;

                //IP - 09/06/10 - CR1083 - Collection Commissions
                drpEmpCollComm.DataSource = empTypes2;


                StringCollection empMembers = new StringCollection();
                empMembers.Add(GetResource("L_STAFF"));
                drpEmpName.DataSource = empMembers;

                LoadDefaultCommissionBasis();
                LoadStaffMembers();
                LoadBailiffCommissionBasis();
                LoadCollectionCommissions(false);                //IP - 09/06/10 - CR1083 - Collection Commissions

                //_staticLoaded = true;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }
        private void ClearDefaultError()
        {
            this.errorProvider1.SetError(this.drpCStatus, "");
            this.errorProvider1.SetError(this.drpCActivity, "");
        }

        private void ClearBailiffError()
        {
            this.errorProvider1.SetError(this.txtBEmpNo, "");
            this.errorProvider1.SetError(this.drpBStatus, "");
            this.errorProvider1.SetError(this.drpBActivity, "");
        }

        private void ClearDefaultFields()
        {
            drpCStatus.Text = "";
            drpCActivity.Text = "";
            numCCollectionPc.Value = 0;
            numCCommPer.Value = 0;
            numCArreRepossPer.Value = 0;
            numCArrearsAllo.Value = 0;
            numCRepossPer.Value = 0;
            numCMin.Value = 0;
            numCMax.Value = 0;
            chkCDebit.Checked = false;
            ClearDefaultError();
        }

        private void ClearBailiffFields()
        {
            txtBEmpNo.Text = "";
            drpBStatus.Text = "";
            drpBActivity.Text = "";
            numBCollectionPc.Value = 0;
            numBCommPer.Value = 0;
            numBArreRepossPer.Value = 0;
            numBArrearsAllo.Value = 0;
            numBRepossPer.Value = 0;
            numBMin.Value = 0;
            numBMax.Value = 0;
            chkBDebit.Checked = false;
            ClearBailiffError();
        }

        private bool ValidDefault()
        {
            bool status = true;
            if (drpCStatus.Text.Trim().Length == 0)
            {
                this.errorProvider1.SetError(this.drpCStatus, GetResource("M_ENTERMANDATORY"));
                status = false;
            }
            else this.errorProvider1.SetError(this.drpCStatus, "");

            if (drpCActivity.Text.Trim().Length == 0)
            {
                this.errorProvider1.SetError(this.drpCActivity, GetResource("M_ENTERMANDATORY"));
                status = false;
            }
            else this.errorProvider1.SetError(this.drpCActivity, "");

            return status;
        }

        private bool ValidBailiff(out int empeeNo)
        {
            empeeNo = 0;
            if (txtBEmpNo.Text.Trim().Length > 0)
                if (IsPositive(txtBEmpNo.Text))
                    empeeNo = Convert.ToInt32(txtBEmpNo.Text);

            bool status = true;
            if (empeeNo < 1)
            {
                this.errorProvider1.SetError(this.txtBEmpNo, GetResource("M_ENTERMANDATORY"));
                status = false;
            }
            else this.errorProvider1.SetError(this.txtBEmpNo, "");

            if (drpBStatus.Text.Trim().Length == 0)
            {
                this.errorProvider1.SetError(this.drpBStatus, GetResource("M_ENTERMANDATORY"));
                status = false;
            }
            else this.errorProvider1.SetError(this.drpBStatus, "");

            if (drpBActivity.Text.Trim().Length == 0)
            {
                this.errorProvider1.SetError(this.drpBActivity, GetResource("M_ENTERMANDATORY"));
                status = false;
            }
            else this.errorProvider1.SetError(this.drpBActivity, "");

            return status;
        }

        private void LoadStaffMembers()
        {
            ClearBailiffFields();
            dgBailiffCommBasis.DataSource = null;
            dgBailiffCommBasis.TableStyles.Clear();

            drpEmpName.DataSource = null;
            DataSet ds = null;
            string empTypeStr = (string)drpEmpCatBailMain.SelectedItem;
            int index = string.IsNullOrEmpty(empTypeStr) ? 0 : empTypeStr.IndexOf(":");

            if (index > 0)
            {
                string empType = empTypeStr.Substring(0, index - 1);
                int len = empTypeStr.Length - 1;
                string empTitle = empTypeStr.Substring(index + 1, len - index);

                StringCollection staffMembers = new StringCollection();
                staffMembers.Add(GetResource("L_STAFF"));

                ds = Login.GetSalesStaffByType(empType, 0, out _errorTxt);
                if (_errorTxt.Length > 0)
                    ShowError(_errorTxt);
                else
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (dt.TableName == TN.SalesStaff)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                string str = row.ItemArray[0].ToString() + " : " + (string)row.ItemArray[1];
                                staffMembers.Add(str.ToUpper());
                            }
                        }
                    }
                }
                drpEmpName.DataSource = staffMembers;
                drpEmpName.SelectedIndex = 0;
            }
        }

        private void LoadDefaultCommissionBasis()
        {
            ClearDefaultFields();
            dgDefaultCommBasis.DataSource = null;
            dgDefaultCommBasis.TableStyles.Clear();
            string empTypeStr = (string)drpEmpCatCommBasis.SelectedItem;

            int index = string.IsNullOrEmpty(empTypeStr) ? 0 : empTypeStr.IndexOf(":");

            if (index > 0)
            {
                string empType = empTypeStr.Substring(0, index - 1);
                DataSet ds = PaymentManager.GetDefaultCommissionBasis(empType, out _errorTxt);
                DataView dvtempReceipt = ds.Tables[TN.BailiffCommission].DefaultView;

                dgDefaultCommBasis.DataSource = dvtempReceipt;

                DataGridTableStyle tabStyle = new DataGridTableStyle();
                tabStyle.MappingName = dvtempReceipt.Table.TableName;
                dgDefaultCommBasis.TableStyles.Add(tabStyle);

                tabStyle.GridColumnStyles[CN.CountryCode].Width = 0;
                tabStyle.GridColumnStyles[CN.EmployeeType].Width = 0;

                tabStyle.GridColumnStyles[CN.StatusCode].Width = 40;
                tabStyle.GridColumnStyles[CN.StatusCode].HeaderText = GetResource("T_STATUS");

                tabStyle.GridColumnStyles[CN.CollectionType].Width = 40;
                tabStyle.GridColumnStyles[CN.CollectionType].HeaderText = GetResource("T_TYPE");

                tabStyle.GridColumnStyles[CN.CollectionPercent].Width = 55;
                tabStyle.GridColumnStyles[CN.CollectionPercent].HeaderText = GetResource("T_COLLECTIONPC");

                tabStyle.GridColumnStyles[CN.CommissionPercent].Width = 55;
                tabStyle.GridColumnStyles[CN.CommissionPercent].HeaderText = GetResource("T_COMMISSIONPC");

                tabStyle.GridColumnStyles[CN.RepPercent].Width = 55;
                tabStyle.GridColumnStyles[CN.RepPercent].HeaderText = GetResource("T_ARREARSREPOPC");

                tabStyle.GridColumnStyles[CN.AllocPercent].Width = 55;
                tabStyle.GridColumnStyles[CN.AllocPercent].HeaderText = GetResource("T_ALLOCPC");

                tabStyle.GridColumnStyles[CN.RepossPercent].Width = 55;
                tabStyle.GridColumnStyles[CN.RepossPercent].HeaderText = GetResource("T_REPOPC");

                tabStyle.GridColumnStyles[CN.MinValue].Width = 55;
                tabStyle.GridColumnStyles[CN.MinValue].HeaderText = GetResource("T_MIN");

                tabStyle.GridColumnStyles[CN.MaxValue].Width = 55;
                tabStyle.GridColumnStyles[CN.MaxValue].HeaderText = GetResource("T_MAX");

                tabStyle.GridColumnStyles[CN.DebitAccount].Width = 40;
                tabStyle.GridColumnStyles[CN.DebitAccount].HeaderText = GetResource("T_DEBIT");
            }
        }

        private void LoadBailiffCommissionBasis()
        {
            ClearBailiffFields();
            dgBailiffCommBasis.DataSource = null;
            dgBailiffCommBasis.TableStyles.Clear();
            string empNoStr = (string)drpEmpName.SelectedItem;
            int index = string.IsNullOrEmpty(empNoStr) ? 0 : empNoStr.IndexOf(":");

            if (index > 0 && empNoStr.Trim() != GetResource("L_STAFF"))
            {
                int empNo = Convert.ToInt32(empNoStr.Substring(0, index - 1));
                // Must enter the empeeno in the field so the user can create
                // the first entry for this empeeno if no data is returned below.
                this.txtBEmpNo.Text = empNo.ToString();

                DataSet ds = PaymentManager.GetBailiffCommissionBasis(empNo, out _errorTxt);
                DataView dvtempReceipt = ds.Tables[TN.BailiffCommission].DefaultView;

                dgBailiffCommBasis.DataSource = dvtempReceipt;

                DataGridTableStyle tabStyle = new DataGridTableStyle();
                tabStyle.MappingName = dvtempReceipt.Table.TableName;
                dgBailiffCommBasis.TableStyles.Add(tabStyle);

                tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 0;

                tabStyle.GridColumnStyles[CN.StatusCode].Width = 40;
                tabStyle.GridColumnStyles[CN.StatusCode].HeaderText = GetResource("T_STATUS");

                tabStyle.GridColumnStyles[CN.CollectionType].Width = 40;
                tabStyle.GridColumnStyles[CN.CollectionType].HeaderText = GetResource("T_TYPE");

                tabStyle.GridColumnStyles[CN.CollectionPercent].Width = 55;
                tabStyle.GridColumnStyles[CN.CollectionPercent].HeaderText = GetResource("T_COLLECTIONPC");

                tabStyle.GridColumnStyles[CN.CommissionPercent].Width = 55;
                tabStyle.GridColumnStyles[CN.CommissionPercent].HeaderText = GetResource("T_COMMISSIONPC");

                tabStyle.GridColumnStyles[CN.RepPercent].Width = 55;
                tabStyle.GridColumnStyles[CN.RepPercent].HeaderText = GetResource("T_ARREARSREPOPC");

                tabStyle.GridColumnStyles[CN.AllocPercent].Width = 55;
                tabStyle.GridColumnStyles[CN.AllocPercent].HeaderText = GetResource("T_ALLOCPC");

                tabStyle.GridColumnStyles[CN.RepossPercent].Width = 55;
                tabStyle.GridColumnStyles[CN.RepossPercent].HeaderText = GetResource("T_REPOPC");

                tabStyle.GridColumnStyles[CN.MinValue].Width = 55;
                tabStyle.GridColumnStyles[CN.MinValue].HeaderText = GetResource("T_MIN");

                tabStyle.GridColumnStyles[CN.MaxValue].Width = 55;
                tabStyle.GridColumnStyles[CN.MaxValue].HeaderText = GetResource("T_MAX");

                tabStyle.GridColumnStyles[CN.DebitAccount].Width = 40;
                tabStyle.GridColumnStyles[CN.DebitAccount].HeaderText = GetResource("T_DEBIT");
            }
        }


        private void drpEmpCatCommBasis_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            try
            {
                Function = "drpEmpCatCommBasis_SelectionChangeCommitted";
                Wait();
                LoadDefaultCommissionBasis();
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

        private void drpEmpCatBailMain_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            try
            {
                Function = "drpEmpCatBailMain_SelectionChangeCommitted";
                Wait();
                LoadStaffMembers();
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

        private void drpEmpName_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            try
            {
                Function = "drpEmpName_SelectionChangeCommitted";
                Wait();
                LoadBailiffCommissionBasis();
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

        private void dgDefaultCommBasis_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Function = "dgDefaultCommBasis_MouseUp";
                Wait();

                int index = dgDefaultCommBasis.CurrentRowIndex;
                if (index >= 0)
                {
                    drpCStatus.Text = (string)((DataView)dgDefaultCommBasis.DataSource)[index][CN.StatusCode];
                    drpCActivity.Text = (string)((DataView)dgDefaultCommBasis.DataSource)[index][CN.CollectionType];
                    numCCollectionPc.Value = Convert.ToDecimal(((DataView)dgDefaultCommBasis.DataSource)[index][CN.CollectionPercent]);
                    numCCommPer.Value = Convert.ToDecimal(((DataView)dgDefaultCommBasis.DataSource)[index][CN.CommissionPercent]);
                    numCMin.Value = Convert.ToDecimal(((DataView)dgDefaultCommBasis.DataSource)[index][CN.MinValue]);
                    numCMax.Value = Convert.ToDecimal(((DataView)dgDefaultCommBasis.DataSource)[index][CN.MaxValue]);
                    numCArreRepossPer.Value = Convert.ToDecimal(((DataView)dgDefaultCommBasis.DataSource)[index][CN.RepPercent]);
                    numCRepossPer.Value = Convert.ToDecimal(((DataView)dgDefaultCommBasis.DataSource)[index][CN.RepossPercent]);
                    numCArrearsAllo.Value = Convert.ToDecimal(((DataView)dgDefaultCommBasis.DataSource)[index][CN.AllocPercent]);
                    if (Convert.ToInt16(((DataView)dgDefaultCommBasis.DataSource)[index][CN.DebitAccount]) == 1)
                        chkCDebit.Checked = true;
                    else
                        chkCDebit.Checked = false;

                    ClearDefaultError();
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

        private void dgBailiffCommBasis_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Function = "dgBailiffCommBasis_MouseUp";
                Wait();

                int index = dgBailiffCommBasis.CurrentRowIndex;
                if (index >= 0)
                {
                    txtBEmpNo.Text = (string)((DataView)dgBailiffCommBasis.DataSource)[index][CN.EmployeeNo].ToString();
                    drpBStatus.Text = (string)((DataView)dgBailiffCommBasis.DataSource)[index][CN.StatusCode];
                    drpBActivity.Text = (string)((DataView)dgBailiffCommBasis.DataSource)[index][CN.CollectionType];
                    numBCollectionPc.Value = Convert.ToDecimal(((DataView)dgBailiffCommBasis.DataSource)[index][CN.CollectionPercent]);
                    numBCommPer.Value = Convert.ToDecimal(((DataView)dgBailiffCommBasis.DataSource)[index][CN.CommissionPercent]);
                    numBMin.Value = Convert.ToDecimal(((DataView)dgBailiffCommBasis.DataSource)[index][CN.MinValue]);
                    numBMax.Value = Convert.ToDecimal(((DataView)dgBailiffCommBasis.DataSource)[index][CN.MaxValue]);
                    numBArreRepossPer.Value = Convert.ToDecimal(((DataView)dgBailiffCommBasis.DataSource)[index][CN.RepPercent]);
                    numBRepossPer.Value = Convert.ToDecimal(((DataView)dgBailiffCommBasis.DataSource)[index][CN.RepossPercent]);
                    numBArrearsAllo.Value = Convert.ToDecimal(((DataView)dgBailiffCommBasis.DataSource)[index][CN.AllocPercent]);
                    if (Convert.ToInt16(((DataView)dgBailiffCommBasis.DataSource)[index][CN.DebitAccount]) == 1)
                        chkBDebit.Checked = true;
                    else
                        chkBDebit.Checked = false;

                    ClearBailiffError();
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

        private void btnSaveCommBasis_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "btnSaveCommBasis_Click";
                Wait();

                if (ValidDefault())
                {
                    string empTypeStr = (string)drpEmpCatCommBasis.SelectedItem;
                    int index = empTypeStr.IndexOf(":");
                    string empType = empTypeStr.Substring(0, index - 1);
                    short debit = chkCDebit.Checked ? (short)1 : (short)0;

                    PaymentManager.SaveCommissionBasis(numCArrearsAllo.Value, drpCActivity.Text, numCCollectionPc.Value, numCCommPer.Value,
                        Config.CountryCode, debit, empType, numCMax.Value, numCMin.Value,
                        numCRepossPer.Value, numCArreRepossPer.Value, drpCStatus.Text, out _errorTxt);

                    if (_errorTxt.Length > 0) ShowInfo(_errorTxt);
                    else LoadDefaultCommissionBasis();
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

        private void btnDeleteCommBasis_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "btnDeleteCommBasis_Click";
                Wait();

                if (ValidDefault())
                {
                    string empTypeStr = (string)drpEmpCatCommBasis.SelectedItem;
                    int index = empTypeStr.IndexOf(":");
                    string empType = empTypeStr.Substring(0, index - 1);

                    PaymentManager.DeleteCommissionBasis(Config.CountryCode, drpCStatus.Text,
                        drpCActivity.Text, empType, out _errorTxt);

                    if (_errorTxt.Length > 0) ShowInfo(_errorTxt);
                    else LoadDefaultCommissionBasis();
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

        private void btnSaveBailComm_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "btnSaveBailComm_Click";
                Wait();

                int empeeNo = 0;
                if (ValidBailiff(out empeeNo))
                {
                    string empTypeStr = (string)drpEmpCatBailMain.SelectedItem;
                    int index = empTypeStr.IndexOf(":");
                    string empType = empTypeStr.Substring(0, index - 1);
                    short debit = chkBDebit.Checked ? (short)1 : (short)0;

                    PaymentManager.SaveBailiffCommissionBasis(empeeNo, numBArrearsAllo.Value,
                        drpBActivity.Text, numBCollectionPc.Value, numBCommPer.Value, debit, empType, numBMax.Value,
                        numBMin.Value, numBRepossPer.Value, numBArreRepossPer.Value, drpBStatus.Text,
                        out _errorTxt);

                    if (_errorTxt.Length > 0) ShowInfo(_errorTxt);
                    else LoadBailiffCommissionBasis();
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

        private void btnDeleteBailComm_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "btnDeleteBailComm_Click";
                Wait();

                int empeeNo = 0;
                if (ValidBailiff(out empeeNo))
                {
                    PaymentManager.DeleteBailiffCommissionBasis(empeeNo, drpBStatus.Text,
                        drpBActivity.Text, out _errorTxt);

                    if (_errorTxt.Length > 0) ShowInfo(_errorTxt);
                    else LoadBailiffCommissionBasis();
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

        private void btnApplyAll_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "btnApplyAll_Click";
                Wait();

                int empeeNo = 0;
                if (ValidDefault())
                {
                    // Apply to all employees of this type
                    empeeNo = -1;
                    string empTypeStr = (string)drpEmpCatCommBasis.SelectedItem;
                    int index = empTypeStr.IndexOf(":");
                    string empType = empTypeStr.Substring(0, index - 1);
                    short debit = chkCDebit.Checked ? (short)1 : (short)0;

                    PaymentManager.SaveBailiffCommissionBasis(empeeNo, numCArrearsAllo.Value,
                        drpCActivity.Text, numCCollectionPc.Value, numCCommPer.Value, debit, empType, numCMax.Value,
                        numCMin.Value, numCRepossPer.Value, numCArreRepossPer.Value, drpCStatus.Text,
                        out _errorTxt);

                    if (_errorTxt.Length > 0) ShowInfo(_errorTxt);
                    else LoadDefaultCommissionBasis();
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

        private void tcMain_SelectionChanged(object sender, EventArgs e)
        {

        }

        //IP - 09/06/10 - CR1083 - Collection Commissions
        private void LoadCollectionCommissions(bool selectLastCommnSaved)
        {
            txtRuleName.Enabled = false;
            btnSaveCollCommn.Enabled = true;
            btnNewCollCommn.Enabled = true;
            btnDeleteCollCommn.Enabled = false;

            ClearCollectionCommissionFields(null);

            //dgvCollectionCommn.DataSource = null;

            if (drpEmpCollComm.SelectedIndex >= 0)
            {
                string empTypeStr = Convert.ToString(drpEmpCollComm.SelectedItem);
                int index = empTypeStr.IndexOf(":");
                string empType = empTypeStr.Substring(0, index - 1);

                DataSet dsCollCommnRulesAndActions = PaymentManager.GetCollectionCommissionRules(empType, out _errorTxt);
                DataView dvCollectionCommission = dsCollCommnRulesAndActions.Tables[TN.CollectionCommission].DefaultView;

                dgvCollectionCommn.DataSource = dvCollectionCommission;

                dvCollCommnActions = dsCollCommnRulesAndActions.Tables[TN.CollectionCommissionActions].DefaultView;

                if (selectLastCommnSaved)
                {

                    dgvCollectionCommn.Rows[LastSavedCommnIndex].Selected = true;
                    dgvCollectionCommn.CurrentCell = dgvCollectionCommn.Rows[LastSavedCommnIndex].Cells[CN.RuleName];
                    dgvCollectionCommn_MouseUp(null, null);
                }
                else
                {
                    dgvCollectionCommn.ClearSelection();
                    DisableCollectionCommissionFields(true);    //IP - 15/07/10 - CR1083 - Collection Commissions

                }


            }

        }

        //IP - 09/06/10 - CR1083 - Collection Commissions
        private void ClearCollectionCommissionFields(object sender)
        {
            Control ctr = (Control)sender;

            //IP - 16/07/10 - CR1083 - Collection Commissions - Clear specific fields if rule based on percentage of worklist completed.
            if (ctr != null)
            {
                if (ctr.Name == "numPcOfWorklist")
                {
                    numPcArrsColl.Value = 0;
                    numPcOfCalls.Value = 0;
                    numNoOfCalls.Value = 0;
                    numNoOfDaysSinceAction.Value = 0;
                    numTimeFrameDays.Value = 0;
                    numMinBal.Value = 0;
                    numMaxBal.Value = 0;
                    numMinValColl.Value = 0;
                    numMaxValColl.Value = 0;
                    numMinMnthsArrs.Value = 0;
                    numMaxMnthsArrs.Value = 0;
                    numCommOnArrsPaidPc.Value = 0;
                    numCommOnAmtPaidPc.Value = 0;
                    numCommOnFeePc.Value = 0;
                }
            }
            else
            {
                txtRuleName.Text = "";
                rbIndividual.Checked = true;
                numPcArrsColl.Value = 0;
                numPcOfCalls.Value = 0;
                numPcOfWorklist.Value = 0;
                numNoOfCalls.Value = 0;
                numNoOfDaysSinceAction.Value = 0;
                numTimeFrameDays.Value = 0;
                numMinBal.Value = 0;
                numMaxBal.Value = 0;
                numMinValColl.Value = 0;
                numMaxValColl.Value = 0;
                numMinMnthsArrs.Value = 0;
                numMaxMnthsArrs.Value = 0;
                numCommOnArrsPaidPc.Value = 0;
                numCommOnAmtPaidPc.Value = 0;
                numCommOnFeePc.Value = 0;
                numCommSetVal.Value = 0;

                //Uncheck all checked items in the check box list.
                foreach (int i in chkListActions.CheckedIndices)
                {
                    chkListActions.SetItemCheckState(i, CheckState.Unchecked);
                }
            }


        }

        //IP - 09/06/10 - CR1083 - Collection Commissions
        private bool ValidCollectionCommission()
        {
            bool status = true;

            if (txtRuleName.Text.Trim().Length == 0)
            {
                this.errorProvider1.SetError(this.txtRuleName, GetResource("M_ENTERMANDATORY"));
                status = false;
            }
            else
            {
                this.errorProvider1.SetError(this.txtRuleName, "");
            }


            if (numMinBal.Value > numMaxBal.Value)
            {
                this.errorProvider1.SetError(this.numMinBal, GetResource("M_MINBAL"));
                status = false;
            }
            else
            {
                this.errorProvider1.SetError(this.numMinBal, "");
            }


            if (numMaxBal.Value < numMinBal.Value)
            {
                this.errorProvider1.SetError(this.numMaxBal, GetResource("M_MAXBAL"));
                status = false;
            }
            else
            {
                this.errorProvider1.SetError(this.numMaxBal, "");
            }

            if (numMinValColl.Value > numMaxValColl.Value)
            {
                this.errorProvider1.SetError(this.numMinValColl, GetResource("M_MINVALCOLL"));
                status = false;
            }
            else
            {
                this.errorProvider1.SetError(this.numMinValColl, "");
            }

            if (numMaxValColl.Value < numMinValColl.Value)
            {
                this.errorProvider1.SetError(this.numMaxValColl, GetResource("M_MAXVALCOLL"));
                status = false;
            }
            else
            {
                this.errorProvider1.SetError(this.numMaxValColl, "");
            }

            if (numMinMnthsArrs.Value > numMaxMnthsArrs.Value)
            {
                this.errorProvider1.SetError(this.numMinMnthsArrs, GetResource("M_MINMNTHSARRS"));
                status = false;
            }
            else
            {
                this.errorProvider1.SetError(this.numMinMnthsArrs, "");
            }

            if (numMaxMnthsArrs.Value < numMinMnthsArrs.Value)
            {
                this.errorProvider1.SetError(this.numMaxMnthsArrs, GetResource("M_MAXMNTHSARRS"));
                status = false;

            }
            else
            {
                this.errorProvider1.SetError(this.numMaxMnthsArrs, "");
            }

            if (NewRule && ((numCommOnAmtPaidPc.Value > 0 && numCommOnArrsPaidPc.Value > 0 && numCommOnFeePc.Value > 0)
            || (numCommOnAmtPaidPc.Value > 0 && numCommSetVal.Value > 0)
            || (numCommOnArrsPaidPc.Value > 0 && numCommSetVal.Value > 0)
            || (numCommOnFeePc.Value > 0 && numCommSetVal.Value > 0)))
            {
                if (DialogResult.No == ShowInfo("M_MULTIPLECOLLCOMMNRATES", MessageBoxButtons.YesNo))
                {
                    status = false;
                }
            }

            //IP - 09/07/10 - UAT(1094) UAT5.2
            if (chkListActions.CheckedItems.Count == 0)
            {
                this.errorProvider1.SetError(this.chkListActions, GetResource("M_SELECTCOLLCOMMNRULEACTION"));
                status = false;
            }
            else if (chkListActions.CheckedItems.Count > 1 && chkListActions.CheckedItems.Contains("ALL"))
            {
                this.errorProvider1.SetError(this.chkListActions, GetResource("M_SELECTCOLLCOMMNRULEACTION2"));
                status = false;
            }
            else
            {
                this.errorProvider1.SetError(this.chkListActions, "");
            }




            return status;
        }

        private void numCommOnAmtPaid_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numMaxValColl_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numNoOfDays_ValueChanged(object sender, EventArgs e)
        {

        }

        //IP - 09/06/10 - CR1083 - Collection Commissions
        private void btnSaveCollCommn_Click(object sender, EventArgs e)
        {
            if (ValidCollectionCommission())
            {
                string actionTakenStr = string.Empty;
                int indexAction = 0;
                int id = 0;
                string actionTaken = string.Empty;

                string empTypeStr = Convert.ToString(drpEmpCollComm.SelectedItem);
                int indexEmpType = empTypeStr.IndexOf(":");
                string empType = empTypeStr.Substring(0, indexEmpType - 1);

                char commissionType = rbIndividual.Checked ? 'I' : 'T';


                if (NewRule)
                {
                    id = -1;
                }
                else
                {
                    id = Convert.ToInt32(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.ID]);
                    LastSavedCommnIndex = dgvCollectionCommn.CurrentRow.Index; //Save the current index, so when the data is re-loaded, this row is selected.
                }


                string[] actionArr = new string[chkListActions.CheckedItems.Count];

                for (int i = 0; i < chkListActions.CheckedItems.Count; i++)
                {
                    actionTakenStr = Convert.ToString(chkListActions.CheckedItems[i]);

                    if (actionTakenStr != "ALL")
                    {
                        indexAction = actionTakenStr.IndexOf(":");
                        actionTaken = actionTakenStr.Substring(0, indexAction - 1);
                    }
                    else
                    {
                        actionTaken = "ALL";
                    }

                    actionArr[i] = actionTaken;
                }

                //Delete actions for an existing rule before inserting the actions again.
                if (id != -1)
                {
                    PaymentManager.DeleteCollectionCommissionRuleActions(id, out _errorTxt);

                    if (_errorTxt.Length > 0)
                    {
                        ShowInfo(_errorTxt);
                    }
                }

                PaymentManager.SaveCollectionCommissionRule(id, txtRuleName.Text.Trim(),
                                                            empType,
                                                            commissionType, actionArr,
                                                            Convert.ToSingle(numPcArrsColl.Value),
                                                            Convert.ToSingle(numPcOfCalls.Value),
                                                            Convert.ToSingle(numPcOfWorklist.Value),
                                                            Convert.ToInt32(numNoOfCalls.Value),
                                                            Convert.ToInt32(numNoOfDaysSinceAction.Value),
                                                            Convert.ToInt32(numTimeFrameDays.Value),
                                                            numMinBal.Value,
                                                            numMaxBal.Value,
                                                            numMinValColl.Value,
                                                            numMaxValColl.Value,
                                                            Convert.ToInt32(numMinMnthsArrs.Value),
                                                            Convert.ToInt32(numMaxMnthsArrs.Value),
                                                            Convert.ToSingle(numCommOnArrsPaidPc.Value),
                                                            Convert.ToSingle(numCommOnAmtPaidPc.Value),
                                                            Convert.ToSingle(numCommOnFeePc.Value),
                                                            numCommSetVal.Value,
                                                            out _errorTxt);


                if (_errorTxt.Length > 0)
                {
                    ShowInfo(_errorTxt);
                }
                else
                {
                    LoadCollectionCommissions(!NewRule);
                }

            }
        }

        //IP - 10/06/10 - CR1083 - Collection Commissions
        private void dgvCollectionCommn_MouseUp(object sender, MouseEventArgs e)
        {
            if (dgvCollectionCommn.Rows.Count > 0)
            {
                NewRule = false;

                //Clear fields and errorproviders before selecting and populating fields for next selected row.
                ClearCollectionCommissionFields(null);
                ClearCollectionCommissionErrors();

                btnSaveCollCommn.Enabled = true;
                btnNewCollCommn.Enabled = true;
                btnDeleteCollCommn.Enabled = true;

                string actionTakenStr = string.Empty;
                string actionTaken = string.Empty;
                int indexAction = 0;

                dvCollCommnActions.RowFilter = "";

                txtRuleName.Text = Convert.ToString(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.RuleName]);

                if (Convert.ToChar(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.CommissionType]) == 'I')
                {
                    rbIndividual.Checked = true;
                }
                else
                {
                    rbTeam.Checked = true;
                }

                numPcArrsColl.Value = Convert.ToDecimal(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.PcentArrearsColl]);
                numPcOfCalls.Value = Convert.ToDecimal(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.PcentOfCalls]);
                numPcOfWorklist.Value = Convert.ToDecimal(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.PcentOfWorklist]);

                numNoOfCalls.Value = Convert.ToDecimal(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.NoOfCalls]);
                numNoOfDaysSinceAction.Value = Convert.ToDecimal(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.NoOfDaysSinceAction]);
                numTimeFrameDays.Value = Convert.ToDecimal(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.TimeFrameDays]);

                numMinBal.Value = Convert.ToDecimal(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.MinBal]);
                numMaxBal.Value = Convert.ToDecimal(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.MaxBal]);

                numMinValColl.Value = Convert.ToDecimal(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.MinValColl]);
                numMaxValColl.Value = Convert.ToDecimal(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.MaxValColl]);

                numMinMnthsArrs.Value = Convert.ToDecimal(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.MinMnthsArrears]);
                numMaxMnthsArrs.Value = Convert.ToDecimal(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.MaxMnthsArrears]);

                numCommOnArrsPaidPc.Value = Convert.ToDecimal(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.PcentCommOnArrears]);
                numCommOnAmtPaidPc.Value = Convert.ToDecimal(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.PcentCommOnAmtPaid]);
                numCommOnFeePc.Value = Convert.ToDecimal(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.PcentCommOnFee]);
                numCommSetVal.Value = Convert.ToDecimal(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.CommSetVal]);

                //If there are any actions for the selected rule then need to set these in the check box list.
                dvCollCommnActions.RowFilter = CN.ParentID + " = '" + Convert.ToInt32(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.ID]) + "'";

                if (dvCollCommnActions.Count > 0)
                {
                    foreach (DataRowView drv in dvCollCommnActions)
                    {
                        for (int i = 0; i < chkListActions.Items.Count; i++)
                        {
                            actionTakenStr = Convert.ToString(chkListActions.Items[i]);

                            if (actionTakenStr != "ALL")
                            {
                                indexAction = actionTakenStr.IndexOf(":");
                                actionTaken = actionTakenStr.Substring(0, indexAction - 1);
                            }
                            else
                            {
                                actionTaken = "ALL";
                            }

                            if (Convert.ToString(drv[CN.ActionTaken]) == actionTaken)
                            {
                                chkListActions.SetItemChecked(i, true);
                            }

                        }
                    }
                }

                //IP - 15/07/10 - CR1083 - Collection Commissions
                if (numPcOfWorklist.Value > 0)
                {
                    DisableCollectionCommissionFields(false);
                }
                else
                {
                    DisableCollectionCommissionFields(true);
                }


            }

        }

        //IP - 10/06/10 - CR1083 - Collection Commissions
        private void ClearCollectionCommissionErrors()
        {
            this.errorProvider1.SetError(this.txtRuleName, "");
            this.errorProvider1.SetError(this.numMinBal, "");
            this.errorProvider1.SetError(this.numMaxBal, "");
            this.errorProvider1.SetError(this.numMinValColl, "");
            this.errorProvider1.SetError(this.numMaxValColl, "");
            this.errorProvider1.SetError(this.numMinMnthsArrs, "");
            this.errorProvider1.SetError(this.numMaxMnthsArrs, "");
            this.errorProvider1.SetError(this.chkListActions, "");
        }

        ////IP - 10/06/10 - CR1083 - Collection Commissions
        //private void btnClearCollCommBasis_Click(object sender, EventArgs e)
        //{
        //    txtRuleName.Enabled = false;
        //    EnableFields(false);

        //    ClearCollectionCommissionFields();
        //    ClearCollectionCommissionErrors();


        //}

        private void btnDeleteCollCommn_Click(object sender, EventArgs e)
        {
            if (dgvCollectionCommn.CurrentRow.Index >= 0)
            {
                int id = Convert.ToInt32(((DataView)dgvCollectionCommn.DataSource).Table.Rows[CollCommnIndex][CN.ID]);

                if (DialogResult.Yes == ShowInfo("M_DELETERULE", MessageBoxButtons.YesNo))
                {
                    PaymentManager.DeleteCollectionCommissionRule(id, out _errorTxt);

                    if (_errorTxt.Length > 0)
                    {
                        ShowInfo(_errorTxt);
                    }
                    else
                    {
                        LoadCollectionCommissions(false);
                    }
                }

            }


        }

        //IP - 16/06/10 - CR1083 - Collection Commissions
        private void btnNewCollCommn_Click(object sender, EventArgs e)
        {
            NewRule = true;

            txtRuleName.Enabled = true;
            btnSaveCollCommn.Enabled = true;

            ClearCollectionCommissionFields(null);
            ClearCollectionCommissionErrors();

            DisableCollectionCommissionFields(true); //IP - 15/07/10 - CR1083 - Collection Commissions

            btnSaveCollCommn.Enabled = true;
            btnNewCollCommn.Enabled = true;
            btnDeleteCollCommn.Enabled = false;

            dgvCollectionCommn.ClearSelection();
        }

        //IP - 16/06/10 - CR1083 - Collection Commissions
        //private void EnableFields(bool enable)
        //{
        //    btnNewCollCommn.Enabled = !enable;
        //    btnDeleteCollCommn.Enabled = enable;
        //}

        //IP - 16/06/10 - CR1083 - Collection Commissions
        private void drpEmpCollComm_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearCollectionCommissionErrors();  //IP - 15/07/10 - CR1083 - Collection Commissions
            LoadCollectionCommissions(false);
        }

        private void grpCollCommCriteria_Enter(object sender, EventArgs e)
        {

        }

        //IP - 15/07/10 - If a value > 0 entered then need to disable fields.
        private void numPcOfWorklist_Leave(object sender, EventArgs e)
        {
            string actionStr = string.Empty;

            if (numPcOfWorklist.Value > 0)
            {
                DisableCollectionCommissionFields(false);
                ClearCollectionCommissionFields(sender);


                //Uncheck all checked items in the check box list.
                foreach (int i in chkListActions.CheckedIndices)
                {
                    chkListActions.SetItemCheckState(i, CheckState.Unchecked);
                }

                for (int i = 0; i < chkListActions.Items.Count; i++)
                {
                    actionStr = Convert.ToString(chkListActions.Items[i]);

                    if (actionStr == "ALL")
                    {
                        chkListActions.SetItemChecked(i, true);

                    }

                }


            }
            else
            {
                DisableCollectionCommissionFields(true);

                //Uncheck all checked items in the check box list.
                foreach (int i in chkListActions.CheckedIndices)
                {
                    chkListActions.SetItemCheckState(i, CheckState.Unchecked);
                }
            }
        }

        //IP - 15/07/10 - CR1083 - Collection Commissions
        private void DisableCollectionCommissionFields(bool disable)
        {
            numPcArrsColl.Enabled = disable;
            numPcOfCalls.Enabled = disable;
            numNoOfCalls.Enabled = disable;
            numNoOfDaysSinceAction.Enabled = disable;
            numTimeFrameDays.Enabled = disable;
            numMinBal.Enabled = disable;
            numMaxBal.Enabled = disable;
            numMinValColl.Enabled = disable;
            numMaxValColl.Enabled = disable;
            numMinMnthsArrs.Enabled = disable;
            numMaxMnthsArrs.Enabled = disable;
            numCommOnArrsPaidPc.Enabled = disable;
            numCommOnAmtPaidPc.Enabled = disable;
            numCommOnFeePc.Enabled = disable;
            chkListActions.Enabled = disable;

        }







    }
}
