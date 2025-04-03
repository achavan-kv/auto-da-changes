namespace STL.PL
{
    partial class WarrantyReporting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WarrantyReporting));
            this.dgResults = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.grbxReturns = new System.Windows.Forms.GroupBox();
            this.chxRepossessions = new System.Windows.Forms.CheckBox();
            this.chxCancellations = new System.Windows.Forms.CheckBox();
            this.groupDatabase = new System.Windows.Forms.GroupBox();
            this.radioDBLive = new System.Windows.Forms.RadioButton();
            this.radioDBReporting = new System.Windows.Forms.RadioButton();
            this.btnClear = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chxAccountType = new System.Windows.Forms.CheckBox();
            this.chxCategory = new System.Windows.Forms.CheckBox();
            this.chxSalesPerson = new System.Windows.Forms.CheckBox();
            this.chxBranch = new System.Windows.Forms.CheckBox();
            this.txtDummy = new System.Windows.Forms.TextBox();
            this.lDate = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnExcel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chxCash = new System.Windows.Forms.CheckBox();
            this.chxSpecial = new System.Windows.Forms.CheckBox();
            this.chxCredit = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.drpDates = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtDateTo = new System.Windows.Forms.DateTimePicker();
            this.dtDateFrom = new System.Windows.Forms.DateTimePicker();
            this.drpReport = new System.Windows.Forms.ComboBox();
            this.drpBranch = new System.Windows.Forms.ComboBox();
            this.drpSalesPerson = new System.Windows.Forms.ComboBox();
            this.drpCategory = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgResults)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.grbxReturns.SuspendLayout();
            this.groupDatabase.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgResults
            // 
            this.dgResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgResults.Location = new System.Drawing.Point(12, 263);
            this.dgResults.Name = "dgResults";
            this.dgResults.ReadOnly = true;
            this.dgResults.Size = new System.Drawing.Size(768, 202);
            this.dgResults.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.grbxReturns);
            this.groupBox1.Controls.Add(this.groupDatabase);
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.txtDummy);
            this.groupBox1.Controls.Add(this.lDate);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.btnLoad);
            this.groupBox1.Controls.Add(this.btnExcel);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.drpDates);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dtDateTo);
            this.groupBox1.Controls.Add(this.dtDateFrom);
            this.groupBox1.Controls.Add(this.drpReport);
            this.groupBox1.Controls.Add(this.drpBranch);
            this.groupBox1.Controls.Add(this.drpSalesPerson);
            this.groupBox1.Controls.Add(this.drpCategory);
            this.groupBox1.Location = new System.Drawing.Point(12, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(767, 250);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // grbxReturns
            // 
            this.grbxReturns.Controls.Add(this.chxRepossessions);
            this.grbxReturns.Controls.Add(this.chxCancellations);
            this.grbxReturns.Enabled = false;
            this.grbxReturns.Location = new System.Drawing.Point(19, 191);
            this.grbxReturns.Name = "grbxReturns";
            this.grbxReturns.Size = new System.Drawing.Size(312, 55);
            this.grbxReturns.TabIndex = 102;
            this.grbxReturns.TabStop = false;
            this.grbxReturns.Text = "Include Returns";
            // 
            // chxRepossessions
            // 
            this.chxRepossessions.AutoSize = true;
            this.chxRepossessions.Enabled = false;
            this.chxRepossessions.Location = new System.Drawing.Point(196, 25);
            this.chxRepossessions.Name = "chxRepossessions";
            this.chxRepossessions.Size = new System.Drawing.Size(92, 17);
            this.chxRepossessions.TabIndex = 62;
            this.chxRepossessions.Text = "Reposessions";
            this.chxRepossessions.UseVisualStyleBackColor = true;
            // 
            // chxCancellations
            // 
            this.chxCancellations.AutoSize = true;
            this.chxCancellations.Enabled = false;
            this.chxCancellations.Location = new System.Drawing.Point(36, 25);
            this.chxCancellations.Name = "chxCancellations";
            this.chxCancellations.Size = new System.Drawing.Size(89, 17);
            this.chxCancellations.TabIndex = 60;
            this.chxCancellations.Text = "Cancellations";
            this.chxCancellations.UseVisualStyleBackColor = true;
            // 
            // groupDatabase
            // 
            this.groupDatabase.Controls.Add(this.radioDBLive);
            this.groupDatabase.Controls.Add(this.radioDBReporting);
            this.groupDatabase.Location = new System.Drawing.Point(363, 80);
            this.groupDatabase.Name = "groupDatabase";
            this.groupDatabase.Size = new System.Drawing.Size(156, 46);
            this.groupDatabase.TabIndex = 100;
            this.groupDatabase.TabStop = false;
            this.groupDatabase.Text = "Database";
            // 
            // radioDBLive
            // 
            this.radioDBLive.Enabled = false;
            this.radioDBLive.Location = new System.Drawing.Point(86, 16);
            this.radioDBLive.Name = "radioDBLive";
            this.radioDBLive.Size = new System.Drawing.Size(64, 24);
            this.radioDBLive.TabIndex = 97;
            this.radioDBLive.Text = "Live";
            // 
            // radioDBReporting
            // 
            this.radioDBReporting.Checked = true;
            this.radioDBReporting.Location = new System.Drawing.Point(15, 16);
            this.radioDBReporting.Name = "radioDBReporting";
            this.radioDBReporting.Size = new System.Drawing.Size(86, 24);
            this.radioDBReporting.TabIndex = 96;
            this.radioDBReporting.TabStop = true;
            this.radioDBReporting.Text = "Reporting";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(578, 93);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(45, 31);
            this.btnClear.TabIndex = 73;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chxAccountType);
            this.groupBox3.Controls.Add(this.chxCategory);
            this.groupBox3.Controls.Add(this.chxSalesPerson);
            this.groupBox3.Controls.Add(this.chxBranch);
            this.groupBox3.Location = new System.Drawing.Point(353, 130);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(367, 55);
            this.groupBox3.TabIndex = 71;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Group By";
            // 
            // chxAccountType
            // 
            this.chxAccountType.AutoSize = true;
            this.chxAccountType.Location = new System.Drawing.Point(263, 25);
            this.chxAccountType.Name = "chxAccountType";
            this.chxAccountType.Size = new System.Drawing.Size(93, 17);
            this.chxAccountType.TabIndex = 63;
            this.chxAccountType.Text = "Account Type";
            this.chxAccountType.UseVisualStyleBackColor = true;
            // 
            // chxCategory
            // 
            this.chxCategory.AutoSize = true;
            this.chxCategory.Location = new System.Drawing.Point(81, 25);
            this.chxCategory.Name = "chxCategory";
            this.chxCategory.Size = new System.Drawing.Size(68, 17);
            this.chxCategory.TabIndex = 62;
            this.chxCategory.Text = "Category";
            this.chxCategory.UseVisualStyleBackColor = true;
            // 
            // chxSalesPerson
            // 
            this.chxSalesPerson.AutoSize = true;
            this.chxSalesPerson.Location = new System.Drawing.Point(165, 25);
            this.chxSalesPerson.Name = "chxSalesPerson";
            this.chxSalesPerson.Size = new System.Drawing.Size(88, 17);
            this.chxSalesPerson.TabIndex = 61;
            this.chxSalesPerson.Text = "Sales Person";
            this.chxSalesPerson.UseVisualStyleBackColor = true;
            // 
            // chxBranch
            // 
            this.chxBranch.AutoSize = true;
            this.chxBranch.Checked = true;
            this.chxBranch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chxBranch.Location = new System.Drawing.Point(15, 25);
            this.chxBranch.Name = "chxBranch";
            this.chxBranch.Size = new System.Drawing.Size(60, 17);
            this.chxBranch.TabIndex = 60;
            this.chxBranch.Text = "Branch";
            this.chxBranch.UseVisualStyleBackColor = true;
            // 
            // txtDummy
            // 
            this.txtDummy.Enabled = false;
            this.txtDummy.Location = new System.Drawing.Point(735, 149);
            this.txtDummy.Name = "txtDummy";
            this.txtDummy.Size = new System.Drawing.Size(26, 20);
            this.txtDummy.TabIndex = 70;
            this.txtDummy.Visible = false;
            // 
            // lDate
            // 
            this.lDate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lDate.Location = new System.Drawing.Point(568, 18);
            this.lDate.Name = "lDate";
            this.lDate.Size = new System.Drawing.Size(193, 16);
            this.lDate.TabIndex = 69;
            this.lDate.Text = "Delivery Date Of Item";
            this.lDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(575, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 16);
            this.label6.TabIndex = 68;
            this.label6.Text = "And";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(519, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 16);
            this.label5.TabIndex = 67;
            this.label5.Text = "Date Between";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnLoad
            // 
            this.btnLoad.CausesValidation = false;
            this.btnLoad.Image = ((System.Drawing.Image)(resources.GetObject("btnLoad.Image")));
            this.btnLoad.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnLoad.Location = new System.Drawing.Point(640, 93);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(32, 31);
            this.btnLoad.TabIndex = 66;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnExcel
            // 
            this.btnExcel.Enabled = false;
            this.btnExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnExcel.Image")));
            this.btnExcel.Location = new System.Drawing.Point(688, 92);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(32, 32);
            this.btnExcel.TabIndex = 65;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chxCash);
            this.groupBox2.Controls.Add(this.chxSpecial);
            this.groupBox2.Controls.Add(this.chxCredit);
            this.groupBox2.Location = new System.Drawing.Point(19, 130);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(312, 55);
            this.groupBox2.TabIndex = 64;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Account Type";
            // 
            // chxCash
            // 
            this.chxCash.AutoSize = true;
            this.chxCash.Checked = true;
            this.chxCash.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chxCash.Location = new System.Drawing.Point(119, 25);
            this.chxCash.Name = "chxCash";
            this.chxCash.Size = new System.Drawing.Size(50, 17);
            this.chxCash.TabIndex = 62;
            this.chxCash.Text = "Cash";
            this.chxCash.UseVisualStyleBackColor = true;
            // 
            // chxSpecial
            // 
            this.chxSpecial.AutoSize = true;
            this.chxSpecial.Checked = true;
            this.chxSpecial.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chxSpecial.Location = new System.Drawing.Point(199, 25);
            this.chxSpecial.Name = "chxSpecial";
            this.chxSpecial.Size = new System.Drawing.Size(89, 17);
            this.chxSpecial.TabIndex = 61;
            this.chxSpecial.Text = "Cash And Go";
            this.chxSpecial.UseVisualStyleBackColor = true;
            // 
            // chxCredit
            // 
            this.chxCredit.AutoSize = true;
            this.chxCredit.Checked = true;
            this.chxCredit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chxCredit.Location = new System.Drawing.Point(36, 25);
            this.chxCredit.Name = "chxCredit";
            this.chxCredit.Size = new System.Drawing.Size(53, 17);
            this.chxCredit.TabIndex = 60;
            this.chxCredit.Text = "Credit";
            this.chxCredit.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(314, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(43, 16);
            this.label8.TabIndex = 62;
            this.label8.Text = "Dates";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpDates
            // 
            this.drpDates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDates.Items.AddRange(new object[] {
            "Order Date",
            "Item Delivery Date",
            "Warranty Delivery Date"});
            this.drpDates.Location = new System.Drawing.Point(363, 16);
            this.drpDates.Name = "drpDates";
            this.drpDates.Size = new System.Drawing.Size(156, 21);
            this.drpDates.TabIndex = 61;
            this.drpDates.SelectedIndexChanged += new System.EventHandler(this.drpDates_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(284, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 18);
            this.label4.TabIndex = 56;
            this.label4.Text = "Category";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(8, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 16);
            this.label3.TabIndex = 55;
            this.label3.Text = "Sales Person";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(48, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 16);
            this.label2.TabIndex = 54;
            this.label2.Text = "Branch";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(45, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 16);
            this.label1.TabIndex = 53;
            this.label1.Text = "Report";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtDateTo
            // 
            this.dtDateTo.CustomFormat = "dd MMM yyyy";
            this.dtDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDateTo.Location = new System.Drawing.Point(608, 64);
            this.dtDateTo.Name = "dtDateTo";
            this.dtDateTo.Size = new System.Drawing.Size(112, 20);
            this.dtDateTo.TabIndex = 47;
            this.dtDateTo.Tag = "";
            this.dtDateTo.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            // 
            // dtDateFrom
            // 
            this.dtDateFrom.CustomFormat = "dd MMM yyyy";
            this.dtDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDateFrom.Location = new System.Drawing.Point(608, 38);
            this.dtDateFrom.Name = "dtDateFrom";
            this.dtDateFrom.Size = new System.Drawing.Size(112, 20);
            this.dtDateFrom.TabIndex = 45;
            this.dtDateFrom.Tag = "";
            this.dtDateFrom.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            // 
            // drpReport
            // 
            this.drpReport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpReport.Location = new System.Drawing.Point(99, 16);
            this.drpReport.Name = "drpReport";
            this.drpReport.Size = new System.Drawing.Size(156, 21);
            this.drpReport.TabIndex = 43;
            this.drpReport.SelectedIndexChanged += new System.EventHandler(this.drpReport_SelectedIndexChanged);
            // 
            // drpBranch
            // 
            this.drpBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranch.Location = new System.Drawing.Point(99, 53);
            this.drpBranch.Name = "drpBranch";
            this.drpBranch.Size = new System.Drawing.Size(156, 21);
            this.drpBranch.TabIndex = 42;
            this.drpBranch.SelectedIndexChanged += new System.EventHandler(this.drpBranch_SelectedIndexChanged);
            // 
            // drpSalesPerson
            // 
            this.drpSalesPerson.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpSalesPerson.Location = new System.Drawing.Point(99, 92);
            this.drpSalesPerson.Name = "drpSalesPerson";
            this.drpSalesPerson.Size = new System.Drawing.Size(156, 21);
            this.drpSalesPerson.TabIndex = 41;
            // 
            // drpCategory
            // 
            this.drpCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCategory.Location = new System.Drawing.Point(363, 53);
            this.drpCategory.Name = "drpCategory";
            this.drpCategory.Size = new System.Drawing.Size(156, 21);
            this.drpCategory.TabIndex = 40;
            this.drpCategory.SelectedIndexChanged += new System.EventHandler(this.drpCategory_SelectedIndexChanged);
            // 
            // WarrantyReporting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.ControlBox = false;
            this.Controls.Add(this.dgResults);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WarrantyReporting";
            this.Text = "Warranty Reporting";
            ((System.ComponentModel.ISupportInitialize)(this.dgResults)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grbxReturns.ResumeLayout(false);
            this.grbxReturns.PerformLayout();
            this.groupDatabase.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgResults;
        private System.Windows.Forms.ComboBox drpReport;
        private System.Windows.Forms.ComboBox drpBranch;
        private System.Windows.Forms.ComboBox drpSalesPerson;
        private System.Windows.Forms.ComboBox drpCategory;
        private System.Windows.Forms.DateTimePicker dtDateTo;
        private System.Windows.Forms.DateTimePicker dtDateFrom;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox drpDates;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chxCash;
        private System.Windows.Forms.CheckBox chxSpecial;
        private System.Windows.Forms.CheckBox chxCredit;
        private System.Windows.Forms.Button btnLoad;
        public System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lDate;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chxAccountType;
        private System.Windows.Forms.CheckBox chxCategory;
        private System.Windows.Forms.CheckBox chxSalesPerson;
        private System.Windows.Forms.CheckBox chxBranch;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.GroupBox groupDatabase;
        private System.Windows.Forms.RadioButton radioDBLive;
        private System.Windows.Forms.RadioButton radioDBReporting;
        private System.Windows.Forms.TextBox txtDummy;
        private System.Windows.Forms.GroupBox grbxReturns;
        private System.Windows.Forms.CheckBox chxRepossessions;
        private System.Windows.Forms.CheckBox chxCancellations;
    }
}