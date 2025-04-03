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
    public class CommissionMaintenance : CommonForm
    {
        private string _errorTxt = "";
        //private bool _staticLoaded; // = false;
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
        private IContainer components;

        public CommissionMaintenance(Form root, Form parent)
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
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tcMain = new Crownwood.Magic.Controls.TabControl();
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
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.tcMain.SuspendLayout();
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
            this.SuspendLayout();
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
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
            this.tpBailiffCommision});
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
            this.drpEmpCatCommBasis.Size = new System.Drawing.Size(232, 21);
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
            this.numCCollectionPc.Size = new System.Drawing.Size(80, 21);
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
            this.drpCActivity.Size = new System.Drawing.Size(56, 21);
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
            this.drpCStatus.Size = new System.Drawing.Size(56, 21);
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
            this.numCMin.Size = new System.Drawing.Size(80, 21);
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
            this.numCMax.Size = new System.Drawing.Size(80, 21);
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
            this.numCArrearsAllo.Size = new System.Drawing.Size(80, 21);
            this.numCArrearsAllo.TabIndex = 150;
            // 
            // numCArreRepossPer
            // 
            this.numCArreRepossPer.DecimalPlaces = 2;
            this.numCArreRepossPer.Location = new System.Drawing.Point(112, 136);
            this.numCArreRepossPer.Name = "numCArreRepossPer";
            this.numCArreRepossPer.Size = new System.Drawing.Size(80, 21);
            this.numCArreRepossPer.TabIndex = 140;
            // 
            // numCRepossPer
            // 
            this.numCRepossPer.DecimalPlaces = 2;
            this.numCRepossPer.Location = new System.Drawing.Point(112, 184);
            this.numCRepossPer.Name = "numCRepossPer";
            this.numCRepossPer.Size = new System.Drawing.Size(80, 21);
            this.numCRepossPer.TabIndex = 160;
            // 
            // numCCommPer
            // 
            this.numCCommPer.DecimalPlaces = 2;
            this.numCCommPer.Location = new System.Drawing.Point(112, 112);
            this.numCCommPer.Name = "numCCommPer";
            this.numCCommPer.Size = new System.Drawing.Size(80, 21);
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
            this.tpBailiffCommision.Title = "Bailiff Commission Maintenance";
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
            this.numBCollectionPc.Size = new System.Drawing.Size(80, 21);
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
            this.drpBActivity.Size = new System.Drawing.Size(56, 21);
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
            this.drpBStatus.Size = new System.Drawing.Size(56, 21);
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
            this.numBArreRepossPer.Size = new System.Drawing.Size(80, 21);
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
            this.txtBEmpNo.Size = new System.Drawing.Size(80, 21);
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
            this.numBMin.Size = new System.Drawing.Size(80, 21);
            this.numBMin.TabIndex = 170;
            // 
            // numBRepossPer
            // 
            this.numBRepossPer.DecimalPlaces = 2;
            this.numBRepossPer.Location = new System.Drawing.Point(112, 200);
            this.numBRepossPer.Name = "numBRepossPer";
            this.numBRepossPer.Size = new System.Drawing.Size(80, 21);
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
            this.numBMax.Size = new System.Drawing.Size(80, 21);
            this.numBMax.TabIndex = 180;
            // 
            // numBCommPer
            // 
            this.numBCommPer.DecimalPlaces = 2;
            this.numBCommPer.Location = new System.Drawing.Point(112, 128);
            this.numBCommPer.Name = "numBCommPer";
            this.numBCommPer.Size = new System.Drawing.Size(80, 21);
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
            this.drpEmpName.Size = new System.Drawing.Size(232, 21);
            this.drpEmpName.TabIndex = 0;
            this.drpEmpName.TabStop = false;
            this.drpEmpName.SelectionChangeCommitted += new System.EventHandler(this.drpEmpName_SelectionChangeCommitted);
            // 
            // drpEmpCatBailMain
            // 
            this.drpEmpCatBailMain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmpCatBailMain.Location = new System.Drawing.Point(16, 24);
            this.drpEmpCatBailMain.Name = "drpEmpCatBailMain";
            this.drpEmpCatBailMain.Size = new System.Drawing.Size(232, 21);
            this.drpEmpCatBailMain.TabIndex = 0;
            this.drpEmpCatBailMain.TabStop = false;
            this.drpEmpCatBailMain.SelectionChangeCommitted += new System.EventHandler(this.drpEmpCatBailMain_SelectionChangeCommitted);
            // 
            // CommissionMaintenance
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox2);
            this.Name = "CommissionMaintenance";
            this.Text = "Commission Maintenance";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.tcMain.ResumeLayout(false);
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
                    string str = string.Format("{0} : {1}", row[0], row[1]);
                    empTypes1.Add(str.ToUpper());
                    empTypes2.Add(str.ToUpper());
                }

                drpEmpCatBailMain.DataSource = empTypes1;
                drpEmpCatCommBasis.DataSource = empTypes2;

                StringCollection empMembers = new StringCollection();
                empMembers.Add(GetResource("L_STAFF"));
                drpEmpName.DataSource = empMembers;

                LoadDefaultCommissionBasis();
                LoadStaffMembers();
                LoadBailiffCommissionBasis();

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
            int index = empTypeStr.IndexOf(":");

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
            int index = empTypeStr.IndexOf(":");

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
            int index = empNoStr.IndexOf(":");

            if (empNoStr.Trim() != GetResource("L_STAFF") && index > 0)
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

    }
}
