namespace STL.PL.Collections
{
    partial class SpecialArrangements
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpecialArrangements));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblTerm = new System.Windows.Forms.Label();
            this.txtTerm = new System.Windows.Forms.TextBox();
            this.dpFinPayDate = new System.Windows.Forms.DateTimePicker();
            this.dpDateAcctOpen = new System.Windows.Forms.DateTimePicker();
            this.txtPercentPaid = new System.Windows.Forms.TextBox();
            this.lblFinPayDate = new System.Windows.Forms.Label();
            this.lblPcentPaid = new System.Windows.Forms.Label();
            this.lblDateActOpen = new System.Windows.Forms.Label();
            this.txtMonthlyInstalment = new System.Windows.Forms.TextBox();
            this.lblMonthlyInstalment = new System.Windows.Forms.Label();
            this.txtArrears = new System.Windows.Forms.TextBox();
            this.lblArrears = new System.Windows.Forms.Label();
            this.txtCurrBalance = new System.Windows.Forms.TextBox();
            this.lblCurrBalance = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtHighInstalAmt = new System.Windows.Forms.TextBox();
            this.rbTermRemains = new System.Windows.Forms.RadioButton();
            this.rbExtendTerm = new System.Windows.Forms.RadioButton();
            this.dtFinalPaymentDate = new System.Windows.Forms.DateTimePicker();
            this.lblViewConsolidated = new System.Windows.Forms.Label();
            this.btnViewConsolidated = new System.Windows.Forms.Button();
            this.numNoOfIns = new System.Windows.Forms.NumericUpDown();
            this.dtReviewDate = new System.Windows.Forms.DateTimePicker();
            this.lblReviewDate = new System.Windows.Forms.Label();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.lblFinalPaymentDate = new System.Windows.Forms.Label();
            this.dtFirstPaymentDate = new System.Windows.Forms.DateTimePicker();
            this.lblFirstPaymentDate = new System.Windows.Forms.Label();
            this.drpReason = new System.Windows.Forms.ComboBox();
            this.lblReason = new System.Windows.Forms.Label();
            this.txtOddPaymentAmt = new System.Windows.Forms.TextBox();
            this.lblOddPaymentAmt = new System.Windows.Forms.Label();
            this.txtInstalmentAmt = new System.Windows.Forms.TextBox();
            this.lblInstalmentAmt = new System.Windows.Forms.Label();
            this.lblNumOfIns = new System.Windows.Forms.Label();
            this.txtArrangementAmt = new System.Windows.Forms.TextBox();
            this.lblArrangementAmt = new System.Windows.Forms.Label();
            this.drpPeriodType = new System.Windows.Forms.ComboBox();
            this.lblPeriodType = new System.Windows.Forms.Label();
            this.dgArrangementSched = new System.Windows.Forms.DataGridView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbIntChgs = new System.Windows.Forms.GroupBox();
            this.btnInterestRev = new System.Windows.Forms.Button();
            this.txtInterest = new System.Windows.Forms.TextBox();
            this.txtAdmin = new System.Windows.Forms.TextBox();
            this.btnAdminRev = new System.Windows.Forms.Button();
            this.gbAdminChgs = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbRefinance = new System.Windows.Forms.RadioButton();
            this.rbNormal = new System.Windows.Forms.RadioButton();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.cbArrSchedule = new System.Windows.Forms.ComboBox();
            this.lblArrSchedule = new System.Windows.Forms.Label();
            this.grpAddInt = new System.Windows.Forms.GroupBox();
            this.txtServiceChg = new System.Windows.Forms.TextBox();
            this.cbFreezeInt = new System.Windows.Forms.CheckBox();
            this.cbAddInt = new System.Windows.Forms.CheckBox();
            this.cbPrintSchedule = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNoOfIns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgArrangementSched)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.gbIntChgs.SuspendLayout();
            this.gbAdminChgs.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.grpAddInt.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblTerm);
            this.groupBox1.Controls.Add(this.txtTerm);
            this.groupBox1.Controls.Add(this.dpFinPayDate);
            this.groupBox1.Controls.Add(this.dpDateAcctOpen);
            this.groupBox1.Controls.Add(this.txtPercentPaid);
            this.groupBox1.Controls.Add(this.lblFinPayDate);
            this.groupBox1.Controls.Add(this.lblPcentPaid);
            this.groupBox1.Controls.Add(this.lblDateActOpen);
            this.groupBox1.Controls.Add(this.txtMonthlyInstalment);
            this.groupBox1.Controls.Add(this.lblMonthlyInstalment);
            this.groupBox1.Controls.Add(this.txtArrears);
            this.groupBox1.Controls.Add(this.lblArrears);
            this.groupBox1.Controls.Add(this.txtCurrBalance);
            this.groupBox1.Controls.Add(this.lblCurrBalance);
            this.groupBox1.Location = new System.Drawing.Point(12, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(316, 140);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Account Details";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // lblTerm
            // 
            this.lblTerm.AutoSize = true;
            this.lblTerm.Location = new System.Drawing.Point(248, 57);
            this.lblTerm.Name = "lblTerm";
            this.lblTerm.Size = new System.Drawing.Size(31, 13);
            this.lblTerm.TabIndex = 17;
            this.lblTerm.Text = "Term";
            // 
            // txtTerm
            // 
            this.txtTerm.Location = new System.Drawing.Point(251, 74);
            this.txtTerm.Name = "txtTerm";
            this.txtTerm.ReadOnly = true;
            this.txtTerm.Size = new System.Drawing.Size(31, 20);
            this.txtTerm.TabIndex = 16;
            // 
            // dpFinPayDate
            // 
            this.dpFinPayDate.CalendarMonthBackground = System.Drawing.SystemColors.Control;
            this.dpFinPayDate.Enabled = false;
            this.dpFinPayDate.Location = new System.Drawing.Point(145, 114);
            this.dpFinPayDate.Name = "dpFinPayDate";
            this.dpFinPayDate.Size = new System.Drawing.Size(134, 20);
            this.dpFinPayDate.TabIndex = 15;
            // 
            // dpDateAcctOpen
            // 
            this.dpDateAcctOpen.CalendarMonthBackground = System.Drawing.SystemColors.Control;
            this.dpDateAcctOpen.Enabled = false;
            this.dpDateAcctOpen.Location = new System.Drawing.Point(145, 34);
            this.dpDateAcctOpen.Name = "dpDateAcctOpen";
            this.dpDateAcctOpen.Size = new System.Drawing.Size(137, 20);
            this.dpDateAcctOpen.TabIndex = 14;
            // 
            // txtPercentPaid
            // 
            this.txtPercentPaid.Location = new System.Drawing.Point(145, 74);
            this.txtPercentPaid.Name = "txtPercentPaid";
            this.txtPercentPaid.ReadOnly = true;
            this.txtPercentPaid.Size = new System.Drawing.Size(100, 20);
            this.txtPercentPaid.TabIndex = 12;
            // 
            // lblFinPayDate
            // 
            this.lblFinPayDate.AutoSize = true;
            this.lblFinPayDate.Location = new System.Drawing.Point(142, 97);
            this.lblFinPayDate.Name = "lblFinPayDate";
            this.lblFinPayDate.Size = new System.Drawing.Size(96, 13);
            this.lblFinPayDate.TabIndex = 10;
            this.lblFinPayDate.Text = "Final payment date";
            // 
            // lblPcentPaid
            // 
            this.lblPcentPaid.AutoSize = true;
            this.lblPcentPaid.Location = new System.Drawing.Point(142, 57);
            this.lblPcentPaid.Name = "lblPcentPaid";
            this.lblPcentPaid.Size = new System.Drawing.Size(85, 13);
            this.lblPcentPaid.TabIndex = 9;
            this.lblPcentPaid.Text = "Percentage paid";
            // 
            // lblDateActOpen
            // 
            this.lblDateActOpen.AutoSize = true;
            this.lblDateActOpen.Location = new System.Drawing.Point(142, 19);
            this.lblDateActOpen.Name = "lblDateActOpen";
            this.lblDateActOpen.Size = new System.Drawing.Size(111, 13);
            this.lblDateActOpen.TabIndex = 8;
            this.lblDateActOpen.Text = "Date account opened";
            // 
            // txtMonthlyInstalment
            // 
            this.txtMonthlyInstalment.Location = new System.Drawing.Point(9, 113);
            this.txtMonthlyInstalment.Name = "txtMonthlyInstalment";
            this.txtMonthlyInstalment.ReadOnly = true;
            this.txtMonthlyInstalment.Size = new System.Drawing.Size(86, 20);
            this.txtMonthlyInstalment.TabIndex = 7;
            // 
            // lblMonthlyInstalment
            // 
            this.lblMonthlyInstalment.AutoSize = true;
            this.lblMonthlyInstalment.Location = new System.Drawing.Point(6, 97);
            this.lblMonthlyInstalment.Name = "lblMonthlyInstalment";
            this.lblMonthlyInstalment.Size = new System.Drawing.Size(95, 13);
            this.lblMonthlyInstalment.TabIndex = 6;
            this.lblMonthlyInstalment.Text = "Monthly Instalment";
            // 
            // txtArrears
            // 
            this.txtArrears.Location = new System.Drawing.Point(9, 74);
            this.txtArrears.Name = "txtArrears";
            this.txtArrears.ReadOnly = true;
            this.txtArrears.Size = new System.Drawing.Size(86, 20);
            this.txtArrears.TabIndex = 5;
            // 
            // lblArrears
            // 
            this.lblArrears.AutoSize = true;
            this.lblArrears.Location = new System.Drawing.Point(10, 57);
            this.lblArrears.Name = "lblArrears";
            this.lblArrears.Size = new System.Drawing.Size(40, 13);
            this.lblArrears.TabIndex = 4;
            this.lblArrears.Text = "Arrears";
            // 
            // txtCurrBalance
            // 
            this.txtCurrBalance.Location = new System.Drawing.Point(9, 34);
            this.txtCurrBalance.Name = "txtCurrBalance";
            this.txtCurrBalance.ReadOnly = true;
            this.txtCurrBalance.Size = new System.Drawing.Size(86, 20);
            this.txtCurrBalance.TabIndex = 3;
            // 
            // lblCurrBalance
            // 
            this.lblCurrBalance.AutoSize = true;
            this.lblCurrBalance.Location = new System.Drawing.Point(6, 19);
            this.lblCurrBalance.Name = "lblCurrBalance";
            this.lblCurrBalance.Size = new System.Drawing.Size(83, 13);
            this.lblCurrBalance.TabIndex = 2;
            this.lblCurrBalance.Text = "Current Balance";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtHighInstalAmt);
            this.groupBox2.Controls.Add(this.rbTermRemains);
            this.groupBox2.Controls.Add(this.rbExtendTerm);
            this.groupBox2.Controls.Add(this.dtFinalPaymentDate);
            this.groupBox2.Controls.Add(this.lblViewConsolidated);
            this.groupBox2.Controls.Add(this.btnViewConsolidated);
            this.groupBox2.Controls.Add(this.numNoOfIns);
            this.groupBox2.Controls.Add(this.dtReviewDate);
            this.groupBox2.Controls.Add(this.lblReviewDate);
            this.groupBox2.Controls.Add(this.btnCalculate);
            this.groupBox2.Controls.Add(this.lblFinalPaymentDate);
            this.groupBox2.Controls.Add(this.dtFirstPaymentDate);
            this.groupBox2.Controls.Add(this.lblFirstPaymentDate);
            this.groupBox2.Controls.Add(this.drpReason);
            this.groupBox2.Controls.Add(this.lblReason);
            this.groupBox2.Controls.Add(this.txtOddPaymentAmt);
            this.groupBox2.Controls.Add(this.lblOddPaymentAmt);
            this.groupBox2.Controls.Add(this.txtInstalmentAmt);
            this.groupBox2.Controls.Add(this.lblInstalmentAmt);
            this.groupBox2.Controls.Add(this.lblNumOfIns);
            this.groupBox2.Controls.Add(this.txtArrangementAmt);
            this.groupBox2.Controls.Add(this.lblArrangementAmt);
            this.groupBox2.Controls.Add(this.drpPeriodType);
            this.groupBox2.Controls.Add(this.lblPeriodType);
            this.groupBox2.Location = new System.Drawing.Point(12, 181);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(316, 292);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Enter SPA Details";
            // 
            // txtHighInstalAmt
            // 
            this.txtHighInstalAmt.Location = new System.Drawing.Point(179, 42);
            this.txtHighInstalAmt.Name = "txtHighInstalAmt";
            this.txtHighInstalAmt.ReadOnly = true;
            this.txtHighInstalAmt.Size = new System.Drawing.Size(100, 20);
            this.txtHighInstalAmt.TabIndex = 14;
            // 
            // rbTermRemains
            // 
            this.rbTermRemains.AutoSize = true;
            this.rbTermRemains.Location = new System.Drawing.Point(145, 15);
            this.rbTermRemains.Name = "rbTermRemains";
            this.rbTermRemains.Size = new System.Drawing.Size(93, 17);
            this.rbTermRemains.TabIndex = 85;
            this.rbTermRemains.Text = "Term Remains";
            this.rbTermRemains.UseVisualStyleBackColor = true;
            this.rbTermRemains.CheckedChanged += new System.EventHandler(this.rbTermRemains_CheckedChanged);
            // 
            // rbExtendTerm
            // 
            this.rbExtendTerm.AutoSize = true;
            this.rbExtendTerm.Checked = true;
            this.rbExtendTerm.Location = new System.Drawing.Point(9, 17);
            this.rbExtendTerm.Name = "rbExtendTerm";
            this.rbExtendTerm.Size = new System.Drawing.Size(85, 17);
            this.rbExtendTerm.TabIndex = 84;
            this.rbExtendTerm.TabStop = true;
            this.rbExtendTerm.Text = "Extend Term";
            this.rbExtendTerm.UseVisualStyleBackColor = true;
            this.rbExtendTerm.CheckedChanged += new System.EventHandler(this.rbExtendTerm_CheckedChanged);
            // 
            // dtFinalPaymentDate
            // 
            this.dtFinalPaymentDate.Enabled = false;
            this.dtFinalPaymentDate.Location = new System.Drawing.Point(158, 205);
            this.dtFinalPaymentDate.Name = "dtFinalPaymentDate";
            this.dtFinalPaymentDate.Size = new System.Drawing.Size(121, 20);
            this.dtFinalPaymentDate.TabIndex = 83;
            // 
            // lblViewConsolidated
            // 
            this.lblViewConsolidated.AutoSize = true;
            this.lblViewConsolidated.Location = new System.Drawing.Point(127, 268);
            this.lblViewConsolidated.Name = "lblViewConsolidated";
            this.lblViewConsolidated.Size = new System.Drawing.Size(93, 13);
            this.lblViewConsolidated.TabIndex = 82;
            this.lblViewConsolidated.Text = "View consolidated";
            this.lblViewConsolidated.Visible = false;
            // 
            // btnViewConsolidated
            // 
            this.btnViewConsolidated.Image = ((System.Drawing.Image)(resources.GetObject("btnViewConsolidated.Image")));
            this.btnViewConsolidated.Location = new System.Drawing.Point(239, 258);
            this.btnViewConsolidated.Name = "btnViewConsolidated";
            this.btnViewConsolidated.Size = new System.Drawing.Size(38, 32);
            this.btnViewConsolidated.TabIndex = 81;
            this.btnViewConsolidated.Visible = false;
            this.btnViewConsolidated.Click += new System.EventHandler(this.btnViewConsolidated_click);
            // 
            // numNoOfIns
            // 
            this.numNoOfIns.Location = new System.Drawing.Point(216, 68);
            this.numNoOfIns.Maximum = new decimal(new int[] {
            36,
            0,
            0,
            0});
            this.numNoOfIns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNoOfIns.Name = "numNoOfIns";
            this.numNoOfIns.Size = new System.Drawing.Size(61, 20);
            this.numNoOfIns.TabIndex = 18;
            this.numNoOfIns.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.numNoOfIns.ValueChanged += new System.EventHandler(this.numNoOfIns_ValueChanged);
            // 
            // dtReviewDate
            // 
            this.dtReviewDate.Location = new System.Drawing.Point(158, 235);
            this.dtReviewDate.Name = "dtReviewDate";
            this.dtReviewDate.Size = new System.Drawing.Size(121, 20);
            this.dtReviewDate.TabIndex = 17;
            this.dtReviewDate.ValueChanged += new System.EventHandler(this.dtReviewDate_ValueChanged);
            // 
            // lblReviewDate
            // 
            this.lblReviewDate.AutoSize = true;
            this.lblReviewDate.Location = new System.Drawing.Point(6, 239);
            this.lblReviewDate.Name = "lblReviewDate";
            this.lblReviewDate.Size = new System.Drawing.Size(69, 13);
            this.lblReviewDate.TabIndex = 16;
            this.lblReviewDate.Text = "Review Date";
            // 
            // btnCalculate
            // 
            this.btnCalculate.Location = new System.Drawing.Point(6, 263);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(75, 23);
            this.btnCalculate.TabIndex = 4;
            this.btnCalculate.Text = "Calculate";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // lblFinalPaymentDate
            // 
            this.lblFinalPaymentDate.AutoSize = true;
            this.lblFinalPaymentDate.Location = new System.Drawing.Point(6, 209);
            this.lblFinalPaymentDate.Name = "lblFinalPaymentDate";
            this.lblFinalPaymentDate.Size = new System.Drawing.Size(99, 13);
            this.lblFinalPaymentDate.TabIndex = 14;
            this.lblFinalPaymentDate.Text = "Final Payment Date";
            // 
            // dtFirstPaymentDate
            // 
            this.dtFirstPaymentDate.Location = new System.Drawing.Point(158, 178);
            this.dtFirstPaymentDate.Name = "dtFirstPaymentDate";
            this.dtFirstPaymentDate.Size = new System.Drawing.Size(121, 20);
            this.dtFirstPaymentDate.TabIndex = 13;
            this.dtFirstPaymentDate.ValueChanged += new System.EventHandler(this.dtFirstPaymentDate_valueChanged);
            // 
            // lblFirstPaymentDate
            // 
            this.lblFirstPaymentDate.AutoSize = true;
            this.lblFirstPaymentDate.Location = new System.Drawing.Point(6, 182);
            this.lblFirstPaymentDate.Name = "lblFirstPaymentDate";
            this.lblFirstPaymentDate.Size = new System.Drawing.Size(96, 13);
            this.lblFirstPaymentDate.TabIndex = 12;
            this.lblFirstPaymentDate.Text = "First Payment Date";
            // 
            // drpReason
            // 
            this.drpReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpReason.FormattingEnabled = true;
            this.drpReason.Location = new System.Drawing.Point(158, 150);
            this.drpReason.Name = "drpReason";
            this.drpReason.Size = new System.Drawing.Size(121, 21);
            this.drpReason.TabIndex = 11;
            // 
            // lblReason
            // 
            this.lblReason.AutoSize = true;
            this.lblReason.Location = new System.Drawing.Point(6, 153);
            this.lblReason.Name = "lblReason";
            this.lblReason.Size = new System.Drawing.Size(44, 13);
            this.lblReason.TabIndex = 10;
            this.lblReason.Text = "Reason";
            // 
            // txtOddPaymentAmt
            // 
            this.txtOddPaymentAmt.Location = new System.Drawing.Point(178, 124);
            this.txtOddPaymentAmt.Name = "txtOddPaymentAmt";
            this.txtOddPaymentAmt.Size = new System.Drawing.Size(100, 20);
            this.txtOddPaymentAmt.TabIndex = 9;
            this.txtOddPaymentAmt.Text = "0";
            this.txtOddPaymentAmt.Leave += new System.EventHandler(this.txtOddPaymentAmt_Leave);
            // 
            // lblOddPaymentAmt
            // 
            this.lblOddPaymentAmt.AutoSize = true;
            this.lblOddPaymentAmt.Location = new System.Drawing.Point(6, 124);
            this.lblOddPaymentAmt.Name = "lblOddPaymentAmt";
            this.lblOddPaymentAmt.Size = new System.Drawing.Size(131, 13);
            this.lblOddPaymentAmt.TabIndex = 8;
            this.lblOddPaymentAmt.Text = "Final Arrangement Amount";
            // 
            // txtInstalmentAmt
            // 
            this.txtInstalmentAmt.Location = new System.Drawing.Point(178, 93);
            this.txtInstalmentAmt.Name = "txtInstalmentAmt";
            this.txtInstalmentAmt.Size = new System.Drawing.Size(100, 20);
            this.txtInstalmentAmt.TabIndex = 7;
            this.txtInstalmentAmt.Text = "0";
            this.txtInstalmentAmt.Leave += new System.EventHandler(this.txtInstalmentAmt_Leave);
            // 
            // lblInstalmentAmt
            // 
            this.lblInstalmentAmt.AutoSize = true;
            this.lblInstalmentAmt.Location = new System.Drawing.Point(6, 96);
            this.lblInstalmentAmt.Name = "lblInstalmentAmt";
            this.lblInstalmentAmt.Size = new System.Drawing.Size(157, 13);
            this.lblInstalmentAmt.TabIndex = 6;
            this.lblInstalmentAmt.Text = "Arrangement Instalment Amount";
            // 
            // lblNumOfIns
            // 
            this.lblNumOfIns.AutoSize = true;
            this.lblNumOfIns.Location = new System.Drawing.Point(7, 68);
            this.lblNumOfIns.Name = "lblNumOfIns";
            this.lblNumOfIns.Size = new System.Drawing.Size(112, 13);
            this.lblNumOfIns.TabIndex = 4;
            this.lblNumOfIns.Text = "Number of Instalments";
            // 
            // txtArrangementAmt
            // 
            this.txtArrangementAmt.Location = new System.Drawing.Point(163, 42);
            this.txtArrangementAmt.Name = "txtArrangementAmt";
            this.txtArrangementAmt.Size = new System.Drawing.Size(114, 20);
            this.txtArrangementAmt.TabIndex = 3;
            this.txtArrangementAmt.TextChanged += new System.EventHandler(this.txtArrangementAmt_changed);
            this.txtArrangementAmt.Leave += new System.EventHandler(this.txtArrangementAmt_Leave);
            // 
            // lblArrangementAmt
            // 
            this.lblArrangementAmt.AutoSize = true;
            this.lblArrangementAmt.Location = new System.Drawing.Point(6, 45);
            this.lblArrangementAmt.Name = "lblArrangementAmt";
            this.lblArrangementAmt.Size = new System.Drawing.Size(106, 13);
            this.lblArrangementAmt.TabIndex = 2;
            this.lblArrangementAmt.Text = "Arrangement Amount";
            // 
            // drpPeriodType
            // 
            this.drpPeriodType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpPeriodType.Items.AddRange(new object[] {
            "Weekly",
            "Monthly",
            "Fortnightly"});
            this.drpPeriodType.Location = new System.Drawing.Point(158, 11);
            this.drpPeriodType.Name = "drpPeriodType";
            this.drpPeriodType.Size = new System.Drawing.Size(121, 21);
            this.drpPeriodType.TabIndex = 1;
            this.drpPeriodType.SelectedIndexChanged += new System.EventHandler(this.drpPeriodType_SelectedIndexChanged);
            // 
            // lblPeriodType
            // 
            this.lblPeriodType.AutoSize = true;
            this.lblPeriodType.Location = new System.Drawing.Point(6, 19);
            this.lblPeriodType.Name = "lblPeriodType";
            this.lblPeriodType.Size = new System.Drawing.Size(64, 13);
            this.lblPeriodType.TabIndex = 0;
            this.lblPeriodType.Text = "Period Type";
            // 
            // dgArrangementSched
            // 
            this.dgArrangementSched.AllowUserToAddRows = false;
            this.dgArrangementSched.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgArrangementSched.Location = new System.Drawing.Point(6, 19);
            this.dgArrangementSched.Name = "dgArrangementSched";
            this.dgArrangementSched.ReadOnly = true;
            this.dgArrangementSched.ShowEditingIcon = false;
            this.dgArrangementSched.Size = new System.Drawing.Size(415, 242);
            this.dgArrangementSched.TabIndex = 2;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.dgArrangementSched);
            this.groupBox3.Location = new System.Drawing.Point(353, 29);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(427, 267);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Arrangement Schedule";
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(602, 387);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 5;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(699, 387);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_click);
            // 
            // gbIntChgs
            // 
            this.gbIntChgs.Controls.Add(this.btnInterestRev);
            this.gbIntChgs.Controls.Add(this.txtInterest);
            this.gbIntChgs.Location = new System.Drawing.Point(359, 309);
            this.gbIntChgs.Name = "gbIntChgs";
            this.gbIntChgs.Size = new System.Drawing.Size(165, 50);
            this.gbIntChgs.TabIndex = 7;
            this.gbIntChgs.TabStop = false;
            this.gbIntChgs.Text = "Interest Charges";
            // 
            // btnInterestRev
            // 
            this.btnInterestRev.Location = new System.Drawing.Point(103, 19);
            this.btnInterestRev.Name = "btnInterestRev";
            this.btnInterestRev.Size = new System.Drawing.Size(56, 23);
            this.btnInterestRev.TabIndex = 4;
            this.btnInterestRev.Text = "Reverse";
            this.btnInterestRev.UseVisualStyleBackColor = true;
            this.btnInterestRev.Click += new System.EventHandler(this.btnInterestRev_click);
            // 
            // txtInterest
            // 
            this.txtInterest.Location = new System.Drawing.Point(12, 21);
            this.txtInterest.Name = "txtInterest";
            this.txtInterest.Size = new System.Drawing.Size(85, 20);
            this.txtInterest.TabIndex = 0;
            this.txtInterest.Leave += new System.EventHandler(this.txtInterest_leave);
            // 
            // txtAdmin
            // 
            this.txtAdmin.Location = new System.Drawing.Point(12, 21);
            this.txtAdmin.Name = "txtAdmin";
            this.txtAdmin.Size = new System.Drawing.Size(85, 20);
            this.txtAdmin.TabIndex = 1;
            this.txtAdmin.Leave += new System.EventHandler(this.txtAdmin_leave);
            // 
            // btnAdminRev
            // 
            this.btnAdminRev.Location = new System.Drawing.Point(103, 19);
            this.btnAdminRev.Name = "btnAdminRev";
            this.btnAdminRev.Size = new System.Drawing.Size(56, 23);
            this.btnAdminRev.TabIndex = 5;
            this.btnAdminRev.Text = "Reverse";
            this.btnAdminRev.UseVisualStyleBackColor = true;
            this.btnAdminRev.Click += new System.EventHandler(this.btnAdminRev_click);
            // 
            // gbAdminChgs
            // 
            this.gbAdminChgs.Controls.Add(this.btnAdminRev);
            this.gbAdminChgs.Controls.Add(this.txtAdmin);
            this.gbAdminChgs.Location = new System.Drawing.Point(359, 365);
            this.gbAdminChgs.Name = "gbAdminChgs";
            this.gbAdminChgs.Size = new System.Drawing.Size(165, 51);
            this.gbAdminChgs.TabIndex = 8;
            this.gbAdminChgs.TabStop = false;
            this.gbAdminChgs.Text = "Admin Charges";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rbRefinance);
            this.groupBox4.Controls.Add(this.rbNormal);
            this.groupBox4.Location = new System.Drawing.Point(12, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(316, 29);
            this.groupBox4.TabIndex = 9;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Arrangement type";
            // 
            // rbRefinance
            // 
            this.rbRefinance.AutoSize = true;
            this.rbRefinance.Location = new System.Drawing.Point(203, 9);
            this.rbRefinance.Name = "rbRefinance";
            this.rbRefinance.Size = new System.Drawing.Size(82, 17);
            this.rbRefinance.TabIndex = 1;
            this.rbRefinance.Text = "Reschedule";
            this.rbRefinance.UseVisualStyleBackColor = true;
            this.rbRefinance.CheckedChanged += new System.EventHandler(this.rbRefinance_CheckedChanged);
            // 
            // rbNormal
            // 
            this.rbNormal.AutoSize = true;
            this.rbNormal.Checked = true;
            this.rbNormal.Location = new System.Drawing.Point(107, 9);
            this.rbNormal.Name = "rbNormal";
            this.rbNormal.Size = new System.Drawing.Size(58, 17);
            this.rbNormal.TabIndex = 0;
            this.rbNormal.TabStop = true;
            this.rbNormal.Text = "Normal";
            this.rbNormal.UseVisualStyleBackColor = true;
            this.rbNormal.CheckedChanged += new System.EventHandler(this.rbNormal_checkChanged);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 476);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(792, 22);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // cbArrSchedule
            // 
            this.cbArrSchedule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbArrSchedule.FormattingEnabled = true;
            this.cbArrSchedule.Location = new System.Drawing.Point(602, 335);
            this.cbArrSchedule.Name = "cbArrSchedule";
            this.cbArrSchedule.Size = new System.Drawing.Size(158, 21);
            this.cbArrSchedule.TabIndex = 11;
            this.cbArrSchedule.SelectedIndexChanged += new System.EventHandler(this.cbArrSchedule_SelectedIndexChanged);
            // 
            // lblArrSchedule
            // 
            this.lblArrSchedule.AutoSize = true;
            this.lblArrSchedule.Location = new System.Drawing.Point(602, 315);
            this.lblArrSchedule.Name = "lblArrSchedule";
            this.lblArrSchedule.Size = new System.Drawing.Size(148, 13);
            this.lblArrSchedule.TabIndex = 12;
            this.lblArrSchedule.Text = "Select Arrangement Schedule";
            // 
            // grpAddInt
            // 
            this.grpAddInt.Controls.Add(this.txtServiceChg);
            this.grpAddInt.Controls.Add(this.cbFreezeInt);
            this.grpAddInt.Controls.Add(this.cbAddInt);
            this.grpAddInt.Location = new System.Drawing.Point(359, 416);
            this.grpAddInt.Name = "grpAddInt";
            this.grpAddInt.Size = new System.Drawing.Size(254, 53);
            this.grpAddInt.TabIndex = 13;
            this.grpAddInt.TabStop = false;
            // 
            // txtServiceChg
            // 
            this.txtServiceChg.Location = new System.Drawing.Point(162, 13);
            this.txtServiceChg.Name = "txtServiceChg";
            this.txtServiceChg.ReadOnly = true;
            this.txtServiceChg.Size = new System.Drawing.Size(85, 20);
            this.txtServiceChg.TabIndex = 15;
            // 
            // cbFreezeInt
            // 
            this.cbFreezeInt.AutoSize = true;
            this.cbFreezeInt.Location = new System.Drawing.Point(12, 31);
            this.cbFreezeInt.Name = "cbFreezeInt";
            this.cbFreezeInt.Size = new System.Drawing.Size(107, 17);
            this.cbFreezeInt.TabIndex = 14;
            this.cbFreezeInt.Text = "Freeze Int/Admin";
            this.cbFreezeInt.UseVisualStyleBackColor = true;
            // 
            // cbAddInt
            // 
            this.cbAddInt.AutoSize = true;
            this.cbAddInt.Checked = true;
            this.cbAddInt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAddInt.Location = new System.Drawing.Point(12, 13);
            this.cbAddInt.Name = "cbAddInt";
            this.cbAddInt.Size = new System.Drawing.Size(132, 17);
            this.cbAddInt.TabIndex = 0;
            this.cbAddInt.Text = "Add Additional Interest";
            this.cbAddInt.UseVisualStyleBackColor = true;
            this.cbAddInt.CheckedChanged += new System.EventHandler(this.cbAddInt_CheckedChanged);
            // 
            // cbPrintSchedule
            // 
            this.cbPrintSchedule.AutoSize = true;
            this.cbPrintSchedule.Location = new System.Drawing.Point(602, 364);
            this.cbPrintSchedule.Name = "cbPrintSchedule";
            this.cbPrintSchedule.Size = new System.Drawing.Size(158, 17);
            this.cbPrintSchedule.TabIndex = 14;
            this.cbPrintSchedule.Text = "Print Arrangement Schedule";
            this.cbPrintSchedule.UseVisualStyleBackColor = true;
            // 
            // SpecialArrangements
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 498);
            this.Controls.Add(this.cbPrintSchedule);
            this.Controls.Add(this.grpAddInt);
            this.Controls.Add(this.lblArrSchedule);
            this.Controls.Add(this.cbArrSchedule);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.gbAdminChgs);
            this.Controls.Add(this.gbIntChgs);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "SpecialArrangements";
            this.Text = "Special Arrangements";
            this.Load += new System.EventHandler(this.SpecialArrangements_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNoOfIns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgArrangementSched)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.gbIntChgs.ResumeLayout(false);
            this.gbIntChgs.PerformLayout();
            this.gbAdminChgs.ResumeLayout(false);
            this.gbAdminChgs.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.grpAddInt.ResumeLayout(false);
            this.grpAddInt.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblCurrBalance;
        private System.Windows.Forms.TextBox txtCurrBalance;
        private System.Windows.Forms.TextBox txtArrears;
        private System.Windows.Forms.Label lblArrears;
        private System.Windows.Forms.TextBox txtMonthlyInstalment;
        private System.Windows.Forms.Label lblMonthlyInstalment;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblArrangementAmt;
        private System.Windows.Forms.ComboBox drpPeriodType;
        private System.Windows.Forms.Label lblPeriodType;
        private System.Windows.Forms.TextBox txtInstalmentAmt;
        private System.Windows.Forms.Label lblInstalmentAmt;
        private System.Windows.Forms.Label lblNumOfIns;
        private System.Windows.Forms.TextBox txtArrangementAmt;
        private System.Windows.Forms.TextBox txtOddPaymentAmt;
        private System.Windows.Forms.Label lblOddPaymentAmt;
        private System.Windows.Forms.ComboBox drpReason;
        private System.Windows.Forms.Label lblReason;
        private System.Windows.Forms.DateTimePicker dtReviewDate;
        private System.Windows.Forms.Label lblReviewDate;
        private System.Windows.Forms.Label lblFinalPaymentDate;
        private System.Windows.Forms.DateTimePicker dtFirstPaymentDate;
        private System.Windows.Forms.Label lblFirstPaymentDate;
        private System.Windows.Forms.DataGridView dgArrangementSched;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.NumericUpDown numNoOfIns;
        private System.Windows.Forms.GroupBox gbIntChgs;
        private System.Windows.Forms.TextBox txtAdmin;
        private System.Windows.Forms.TextBox txtInterest;
        private System.Windows.Forms.Button btnAdminRev;
        private System.Windows.Forms.Button btnInterestRev;
        private System.Windows.Forms.GroupBox gbAdminChgs;
        private System.Windows.Forms.Label lblDateActOpen;
        private System.Windows.Forms.Label lblFinPayDate;
        private System.Windows.Forms.Label lblPcentPaid;
        private System.Windows.Forms.TextBox txtPercentPaid;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rbRefinance;
        private System.Windows.Forms.RadioButton rbNormal;
        private System.Windows.Forms.Button btnViewConsolidated;
        private System.Windows.Forms.Label lblViewConsolidated;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Label lblArrSchedule;
        private System.Windows.Forms.ComboBox cbArrSchedule;
        private System.Windows.Forms.DateTimePicker dtFinalPaymentDate;
        private System.Windows.Forms.DateTimePicker dpDateAcctOpen;
        private System.Windows.Forms.DateTimePicker dpFinPayDate;
        private System.Windows.Forms.RadioButton rbExtendTerm;
        private System.Windows.Forms.RadioButton rbTermRemains;
        private System.Windows.Forms.GroupBox grpAddInt;
        private System.Windows.Forms.CheckBox cbFreezeInt;
        private System.Windows.Forms.CheckBox cbAddInt;
        private System.Windows.Forms.TextBox txtTerm;
        private System.Windows.Forms.Label lblTerm;
        private System.Windows.Forms.TextBox txtHighInstalAmt;
        private System.Windows.Forms.TextBox txtServiceChg;
        private System.Windows.Forms.CheckBox cbPrintSchedule;
    }
}