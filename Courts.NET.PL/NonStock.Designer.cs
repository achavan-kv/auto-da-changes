namespace STL.PL
{
    partial class NonStock
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NonStock));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.dgPriceDetails = new System.Windows.Forms.DataGridView();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dgItemDetails = new System.Windows.Forms.DataGridView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbTax = new System.Windows.Forms.Label();
            this.gbApply = new System.Windows.Forms.GroupBox();
            this.rbUndo = new System.Windows.Forms.RadioButton();
            this.rbBranch = new System.Windows.Forms.RadioButton();
            this.rbNonCourts = new System.Windows.Forms.RadioButton();
            this.rbCourts = new System.Windows.Forms.RadioButton();
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.udCostPrice = new System.Windows.Forms.NumericUpDown();
            this.udDutyFreePrice = new System.Windows.Forms.NumericUpDown();
            this.udCashPrice = new System.Windows.Forms.NumericUpDown();
            this.udCreditPrice = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbEndDate = new System.Windows.Forms.Label();
            this.dtEndDate = new System.Windows.Forms.DateTimePicker();
            this.lbStartDate = new System.Windows.Forms.Label();
            this.dtStartDate = new System.Windows.Forms.DateTimePicker();
            this.lbTaxRate = new System.Windows.Forms.Label();
            this.txtTaxRate = new System.Windows.Forms.TextBox();
            this.cbRemoveItem = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.lblTaxable = new System.Windows.Forms.Label();
            this.cbTaxable = new System.Windows.Forms.CheckBox();
            this.lblDeleted = new System.Windows.Forms.Label();
            this.chxDeleted = new System.Windows.Forms.CheckBox();
            this.txtSupplierName = new System.Windows.Forms.TextBox();
            this.txtSupplierCode = new System.Windows.Forms.TextBox();
            this.txtItemDescr1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtItemDescr2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.drpItemCategory = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProvider2 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgPriceDetails)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgItemDetails)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.gbApply.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udCostPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udDutyFreePrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udCashPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udCreditPrice)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.dgPriceDetails);
            this.groupBox5.Location = new System.Drawing.Point(383, 290);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(397, 179);
            this.groupBox5.TabIndex = 16;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Price Details";
            // 
            // dgPriceDetails
            // 
            this.dgPriceDetails.AllowUserToAddRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgPriceDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgPriceDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgPriceDetails.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgPriceDetails.Location = new System.Drawing.Point(7, 20);
            this.dgPriceDetails.MultiSelect = false;
            this.dgPriceDetails.Name = "dgPriceDetails";
            this.dgPriceDetails.ReadOnly = true;
            this.dgPriceDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgPriceDetails.Size = new System.Drawing.Size(384, 153);
            this.dgPriceDetails.TabIndex = 0;
            this.dgPriceDetails.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgPriceDetails_RowHeaderMouseClick);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.dgItemDetails);
            this.groupBox4.Location = new System.Drawing.Point(383, 73);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(397, 210);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Item Details";
            // 
            // dgItemDetails
            // 
            this.dgItemDetails.AllowUserToAddRows = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgItemDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgItemDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgItemDetails.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgItemDetails.Location = new System.Drawing.Point(7, 20);
            this.dgItemDetails.MultiSelect = false;
            this.dgItemDetails.Name = "dgItemDetails";
            this.dgItemDetails.ReadOnly = true;
            this.dgItemDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgItemDetails.Size = new System.Drawing.Size(384, 180);
            this.dgItemDetails.TabIndex = 0;
            this.dgItemDetails.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgItemDetails_RowHeaderMouseClick);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbTax);
            this.groupBox3.Controls.Add(this.gbApply);
            this.groupBox3.Controls.Add(this.udCostPrice);
            this.groupBox3.Controls.Add(this.udDutyFreePrice);
            this.groupBox3.Controls.Add(this.udCashPrice);
            this.groupBox3.Controls.Add(this.udCreditPrice);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Location = new System.Drawing.Point(13, 299);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(363, 166);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Selected Price Details";
            // 
            // lbTax
            // 
            this.lbTax.AutoSize = true;
            this.lbTax.ForeColor = System.Drawing.Color.Red;
            this.lbTax.Location = new System.Drawing.Point(123, 11);
            this.lbTax.Name = "lbTax";
            this.lbTax.Size = new System.Drawing.Size(114, 13);
            this.lbTax.TabIndex = 17;
            this.lbTax.Text = "Prices Inclusive of Tax";
            this.lbTax.Click += new System.EventHandler(this.lbTax_Click);
            // 
            // gbApply
            // 
            this.gbApply.Controls.Add(this.rbUndo);
            this.gbApply.Controls.Add(this.rbBranch);
            this.gbApply.Controls.Add(this.rbNonCourts);
            this.gbApply.Controls.Add(this.rbCourts);
            this.gbApply.Controls.Add(this.rbAll);
            this.gbApply.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gbApply.Location = new System.Drawing.Point(198, 29);
            this.gbApply.Name = "gbApply";
            this.gbApply.Size = new System.Drawing.Size(157, 114);
            this.gbApply.TabIndex = 12;
            this.gbApply.TabStop = false;
            this.gbApply.Text = "Select  to Apply changes to";
            // 
            // rbUndo
            // 
            this.rbUndo.AutoSize = true;
            this.rbUndo.Location = new System.Drawing.Point(7, 91);
            this.rbUndo.Name = "rbUndo";
            this.rbUndo.Size = new System.Drawing.Size(54, 17);
            this.rbUndo.TabIndex = 4;
            this.rbUndo.TabStop = true;
            this.rbUndo.Text = "Undo ";
            this.rbUndo.UseVisualStyleBackColor = true;
            this.rbUndo.CheckedChanged += new System.EventHandler(this.rbUndo_CheckedChanged);
            // 
            // rbBranch
            // 
            this.rbBranch.AutoSize = true;
            this.rbBranch.Location = new System.Drawing.Point(7, 73);
            this.rbBranch.Name = "rbBranch";
            this.rbBranch.Size = new System.Drawing.Size(126, 17);
            this.rbBranch.TabIndex = 3;
            this.rbBranch.TabStop = true;
            this.rbBranch.Text = "Selected Branch only";
            this.rbBranch.UseVisualStyleBackColor = true;
            this.rbBranch.CheckedChanged += new System.EventHandler(this.rbBranch_CheckedChanged);
            // 
            // rbNonCourts
            // 
            this.rbNonCourts.AutoSize = true;
            this.rbNonCourts.Location = new System.Drawing.Point(7, 55);
            this.rbNonCourts.Name = "rbNonCourts";
            this.rbNonCourts.Size = new System.Drawing.Size(126, 17);
            this.rbNonCourts.TabIndex = 2;
            this.rbNonCourts.TabStop = true;
            this.rbNonCourts.Text = "Non Courts Branches";
            this.rbNonCourts.UseVisualStyleBackColor = true;
            this.rbNonCourts.CheckedChanged += new System.EventHandler(this.rbNonCourts_CheckedChanged);
            // 
            // rbCourts
            // 
            this.rbCourts.AutoSize = true;
            this.rbCourts.Location = new System.Drawing.Point(7, 37);
            this.rbCourts.Name = "rbCourts";
            this.rbCourts.Size = new System.Drawing.Size(103, 17);
            this.rbCourts.TabIndex = 1;
            this.rbCourts.TabStop = true;
            this.rbCourts.Text = "Courts Branches";
            this.rbCourts.UseVisualStyleBackColor = true;
            this.rbCourts.CheckedChanged += new System.EventHandler(this.rbCourts_CheckedChanged);
            // 
            // rbAll
            // 
            this.rbAll.AutoSize = true;
            this.rbAll.Location = new System.Drawing.Point(7, 18);
            this.rbAll.Name = "rbAll";
            this.rbAll.Size = new System.Drawing.Size(84, 17);
            this.rbAll.TabIndex = 0;
            this.rbAll.TabStop = true;
            this.rbAll.Text = "All Branches";
            this.rbAll.UseVisualStyleBackColor = true;
            this.rbAll.CheckedChanged += new System.EventHandler(this.rbAll_CheckedChanged);
            // 
            // udCostPrice
            // 
            this.udCostPrice.DecimalPlaces = 2;
            this.udCostPrice.Location = new System.Drawing.Point(114, 105);
            this.udCostPrice.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.udCostPrice.Name = "udCostPrice";
            this.udCostPrice.Size = new System.Drawing.Size(76, 20);
            this.udCostPrice.TabIndex = 11;
            this.udCostPrice.ValueChanged += new System.EventHandler(this.udCostPrice_ValueChanged);
            // 
            // udDutyFreePrice
            // 
            this.udDutyFreePrice.DecimalPlaces = 2;
            this.udDutyFreePrice.Location = new System.Drawing.Point(114, 82);
            this.udDutyFreePrice.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.udDutyFreePrice.Name = "udDutyFreePrice";
            this.udDutyFreePrice.Size = new System.Drawing.Size(76, 20);
            this.udDutyFreePrice.TabIndex = 10;
            this.udDutyFreePrice.ValueChanged += new System.EventHandler(this.udDutyFreePrice_ValueChanged);
            // 
            // udCashPrice
            // 
            this.udCashPrice.DecimalPlaces = 2;
            this.udCashPrice.Location = new System.Drawing.Point(113, 59);
            this.udCashPrice.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.udCashPrice.Name = "udCashPrice";
            this.udCashPrice.Size = new System.Drawing.Size(76, 20);
            this.udCashPrice.TabIndex = 9;
            this.udCashPrice.ValueChanged += new System.EventHandler(this.udCashPrice_ValueChanged);
            // 
            // udCreditPrice
            // 
            this.udCreditPrice.DecimalPlaces = 2;
            this.udCreditPrice.Location = new System.Drawing.Point(113, 36);
            this.udCreditPrice.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.udCreditPrice.Name = "udCreditPrice";
            this.udCreditPrice.Size = new System.Drawing.Size(76, 20);
            this.udCreditPrice.TabIndex = 8;
            this.udCreditPrice.ValueChanged += new System.EventHandler(this.udCreditPrice_ValueChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(53, 108);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(55, 13);
            this.label12.TabIndex = 7;
            this.label12.Text = "Cost Price";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 85);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(102, 13);
            this.label11.TabIndex = 6;
            this.label11.Text = "Unit Price Duty Free";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(27, 62);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(80, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Unit Price Cash";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(33, 39);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Unit Price HP";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbEndDate);
            this.groupBox2.Controls.Add(this.dtEndDate);
            this.groupBox2.Controls.Add(this.lbStartDate);
            this.groupBox2.Controls.Add(this.dtStartDate);
            this.groupBox2.Controls.Add(this.lbTaxRate);
            this.groupBox2.Controls.Add(this.txtTaxRate);
            this.groupBox2.Controls.Add(this.cbRemoveItem);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtItemNo);
            this.groupBox2.Controls.Add(this.lblTaxable);
            this.groupBox2.Controls.Add(this.cbTaxable);
            this.groupBox2.Controls.Add(this.lblDeleted);
            this.groupBox2.Controls.Add(this.chxDeleted);
            this.groupBox2.Controls.Add(this.txtSupplierName);
            this.groupBox2.Controls.Add(this.txtSupplierCode);
            this.groupBox2.Controls.Add(this.txtItemDescr1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtItemDescr2);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.drpItemCategory);
            this.groupBox2.Location = new System.Drawing.Point(13, 73);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(363, 222);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Selected Item Details";
            // 
            // lbEndDate
            // 
            this.lbEndDate.AutoSize = true;
            this.lbEndDate.Location = new System.Drawing.Point(49, 200);
            this.lbEndDate.Name = "lbEndDate";
            this.lbEndDate.Size = new System.Drawing.Size(52, 13);
            this.lbEndDate.TabIndex = 28;
            this.lbEndDate.Text = "End Date";
            this.toolTip1.SetToolTip(this.lbEndDate, "Last Day that item can be sold");
            // 
            // dtEndDate
            // 
            this.dtEndDate.Location = new System.Drawing.Point(106, 198);
            this.dtEndDate.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtEndDate.Name = "dtEndDate";
            this.dtEndDate.ShowCheckBox = true;
            this.dtEndDate.Size = new System.Drawing.Size(149, 20);
            this.dtEndDate.TabIndex = 27;
            this.toolTip1.SetToolTip(this.dtEndDate, "Last Day that item can be sold");
            this.dtEndDate.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtEndDate.ValueChanged += new System.EventHandler(this.dtEndDate_ValueChanged);
            // 
            // lbStartDate
            // 
            this.lbStartDate.AutoSize = true;
            this.lbStartDate.Location = new System.Drawing.Point(46, 175);
            this.lbStartDate.Name = "lbStartDate";
            this.lbStartDate.Size = new System.Drawing.Size(55, 13);
            this.lbStartDate.TabIndex = 26;
            this.lbStartDate.Text = "Start Date";
            // 
            // dtStartDate
            // 
            this.dtStartDate.Location = new System.Drawing.Point(106, 171);
            this.dtStartDate.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtStartDate.Name = "dtStartDate";
            this.dtStartDate.ShowCheckBox = true;
            this.dtStartDate.Size = new System.Drawing.Size(149, 20);
            this.dtStartDate.TabIndex = 25;
            this.dtStartDate.ValueChanged += new System.EventHandler(this.dtStartDate_ValueChanged);
            // 
            // lbTaxRate
            // 
            this.lbTaxRate.AutoSize = true;
            this.lbTaxRate.Location = new System.Drawing.Point(128, 129);
            this.lbTaxRate.Name = "lbTaxRate";
            this.lbTaxRate.Size = new System.Drawing.Size(46, 13);
            this.lbTaxRate.TabIndex = 24;
            this.lbTaxRate.Text = "Tax rate";
            // 
            // txtTaxRate
            // 
            this.txtTaxRate.Location = new System.Drawing.Point(180, 126);
            this.txtTaxRate.Name = "txtTaxRate";
            this.txtTaxRate.Size = new System.Drawing.Size(33, 20);
            this.txtTaxRate.TabIndex = 23;
            // 
            // cbRemoveItem
            // 
            this.cbRemoveItem.AutoSize = true;
            this.cbRemoveItem.Location = new System.Drawing.Point(232, 21);
            this.cbRemoveItem.Name = "cbRemoveItem";
            this.cbRemoveItem.Size = new System.Drawing.Size(113, 17);
            this.cbRemoveItem.TabIndex = 22;
            this.cbRemoveItem.Text = "Remove New item";
            this.cbRemoveItem.UseVisualStyleBackColor = true;
            this.cbRemoveItem.CheckedChanged += new System.EventHandler(this.cbRemove_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Item Number";
            // 
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(106, 20);
            this.txtItemNo.MaxLength = 8;
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.Size = new System.Drawing.Size(100, 20);
            this.txtItemNo.TabIndex = 20;
            this.txtItemNo.Leave += new System.EventHandler(this.txtItemNo_Leave);
            // 
            // lblTaxable
            // 
            this.lblTaxable.AutoSize = true;
            this.lblTaxable.Location = new System.Drawing.Point(55, 129);
            this.lblTaxable.Name = "lblTaxable";
            this.lblTaxable.Size = new System.Drawing.Size(45, 13);
            this.lblTaxable.TabIndex = 19;
            this.lblTaxable.Text = "Taxable";
            // 
            // cbTaxable
            // 
            this.cbTaxable.AutoSize = true;
            this.cbTaxable.Location = new System.Drawing.Point(106, 129);
            this.cbTaxable.Name = "cbTaxable";
            this.cbTaxable.Size = new System.Drawing.Size(15, 14);
            this.cbTaxable.TabIndex = 18;
            this.cbTaxable.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.cbTaxable.UseVisualStyleBackColor = true;
            this.cbTaxable.CheckedChanged += new System.EventHandler(this.cbTaxable_CheckedChanged);
            // 
            // lblDeleted
            // 
            this.lblDeleted.AutoSize = true;
            this.lblDeleted.Location = new System.Drawing.Point(53, 149);
            this.lblDeleted.Name = "lblDeleted";
            this.lblDeleted.Size = new System.Drawing.Size(44, 13);
            this.lblDeleted.TabIndex = 17;
            this.lblDeleted.Text = "Deleted";
            // 
            // chxDeleted
            // 
            this.chxDeleted.AutoSize = true;
            this.chxDeleted.Location = new System.Drawing.Point(106, 149);
            this.chxDeleted.Name = "chxDeleted";
            this.chxDeleted.Size = new System.Drawing.Size(15, 14);
            this.chxDeleted.TabIndex = 16;
            this.chxDeleted.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.chxDeleted.UseVisualStyleBackColor = true;
            this.chxDeleted.CheckedChanged += new System.EventHandler(this.chxDeleted_CheckedChanged);
            // 
            // txtSupplierName
            // 
            this.txtSupplierName.Location = new System.Drawing.Point(287, 123);
            this.txtSupplierName.MaxLength = 40;
            this.txtSupplierName.Name = "txtSupplierName";
            this.txtSupplierName.Size = new System.Drawing.Size(69, 20);
            this.txtSupplierName.TabIndex = 4;
            this.txtSupplierName.Visible = false;
            // 
            // txtSupplierCode
            // 
            this.txtSupplierCode.Location = new System.Drawing.Point(287, 148);
            this.txtSupplierCode.MaxLength = 18;
            this.txtSupplierCode.Name = "txtSupplierCode";
            this.txtSupplierCode.Size = new System.Drawing.Size(69, 20);
            this.txtSupplierCode.TabIndex = 6;
            this.txtSupplierCode.Visible = false;
            // 
            // txtItemDescr1
            // 
            this.txtItemDescr1.Location = new System.Drawing.Point(106, 45);
            this.txtItemDescr1.MaxLength = 25;
            this.txtItemDescr1.Name = "txtItemDescr1";
            this.txtItemDescr1.Size = new System.Drawing.Size(242, 20);
            this.txtItemDescr1.TabIndex = 0;
            this.txtItemDescr1.Leave += new System.EventHandler(this.txtItemDescr1_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Item Description 1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(207, 151);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Supplier Code";
            this.label5.Visible = false;
            // 
            // txtItemDescr2
            // 
            this.txtItemDescr2.Location = new System.Drawing.Point(106, 70);
            this.txtItemDescr2.MaxLength = 40;
            this.txtItemDescr2.Name = "txtItemDescr2";
            this.txtItemDescr2.Size = new System.Drawing.Size(242, 20);
            this.txtItemDescr2.TabIndex = 2;
            this.txtItemDescr2.Leave += new System.EventHandler(this.txtItemDescr2_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(234, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Supplier";
            this.label4.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Item Description 2";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Item Category";
            // 
            // drpItemCategory
            // 
            this.drpItemCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpItemCategory.FormattingEnabled = true;
            this.drpItemCategory.Location = new System.Drawing.Point(106, 97);
            this.drpItemCategory.Name = "drpItemCategory";
            this.drpItemCategory.Size = new System.Drawing.Size(149, 21);
            this.drpItemCategory.TabIndex = 8;
            this.drpItemCategory.SelectedIndexChanged += new System.EventHandler(this.drpItemCategory_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnExport);
            this.groupBox1.Controls.Add(this.btnImport);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.btnLoad);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(767, 53);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " ";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(106, 19);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Visible = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(7, 19);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 5;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Visible = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(683, 19);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(60, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(377, 19);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(65, 23);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.toolTip1.SetToolTip(this.btnClear, "Clear Selected Item details and Selected Price details");
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(576, 19);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(60, 23);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "Reload";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // errorProvider2
            // 
            this.errorProvider2.ContainerControl = this;
            this.errorProvider2.Icon = ((System.Drawing.Icon)(resources.GetObject("errorProvider2.Icon")));
            // 
            // NonStock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "NonStock";
            this.Text = "Non Stock Item Maintenance";
            this.Load += new System.EventHandler(this.NonStock_Load);
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgPriceDetails)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgItemDetails)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gbApply.ResumeLayout(false);
            this.gbApply.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udCostPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udDutyFreePrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udCashPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udCreditPrice)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSupplierCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSupplierName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtItemDescr2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtItemDescr1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox drpItemCategory;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ErrorProvider errorProvider2;
        private System.Windows.Forms.NumericUpDown udCostPrice;
        private System.Windows.Forms.NumericUpDown udDutyFreePrice;
        private System.Windows.Forms.NumericUpDown udCashPrice;
        private System.Windows.Forms.NumericUpDown udCreditPrice;
        private System.Windows.Forms.Label lblDeleted;
        private System.Windows.Forms.CheckBox chxDeleted;
        private System.Windows.Forms.CheckBox cbTaxable;
        private System.Windows.Forms.Label lblTaxable;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.DataGridView dgItemDetails;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.DataGridView dgPriceDetails;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtItemNo;
        private System.Windows.Forms.GroupBox gbApply;
        private System.Windows.Forms.RadioButton rbBranch;
        private System.Windows.Forms.RadioButton rbNonCourts;
        private System.Windows.Forms.RadioButton rbCourts;
        private System.Windows.Forms.RadioButton rbAll;
        private System.Windows.Forms.RadioButton rbUndo;
        private System.Windows.Forms.Label lbTax;
        private System.Windows.Forms.CheckBox cbRemoveItem;
        private System.Windows.Forms.Label lbTaxRate;
        private System.Windows.Forms.TextBox txtTaxRate;
        private System.Windows.Forms.Label lbEndDate;
        private System.Windows.Forms.DateTimePicker dtEndDate;
        private System.Windows.Forms.Label lbStartDate;
        private System.Windows.Forms.DateTimePicker dtStartDate;
        private System.Windows.Forms.ToolTip toolTip1;
       
    }
}