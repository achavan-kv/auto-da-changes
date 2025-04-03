namespace STL.PL
{
    partial class SalesCommissionEnquiry
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SalesCommissionEnquiry));
            this.lblTotalCommissionValue = new System.Windows.Forms.Label();
            this.lblTotalCommissionsValue = new System.Windows.Forms.Label();
            this.lblTotalCommissions = new System.Windows.Forms.Label();
            this.btnTransactionDetailsExcel = new System.Windows.Forms.Button();
            this.lblCommissions = new System.Windows.Forms.Label();
            this.dgvCommissions = new System.Windows.Forms.DataGridView();
            this.gbSearchOptions = new System.Windows.Forms.GroupBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.drpEmployee = new System.Windows.Forms.ComboBox();
            this.lblBranch = new System.Windows.Forms.Label();
            this.dtpDeliveryDateFrom = new System.Windows.Forms.DateTimePicker();
            this.drpBranchNo = new System.Windows.Forms.ComboBox();
            this.dtpDeliveryDateTo = new System.Windows.Forms.DateTimePicker();
            this.lblEmployee = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCommissions)).BeginInit();
            this.gbSearchOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTotalCommissionValue
            // 
            this.lblTotalCommissionValue.AutoSize = true;
            this.lblTotalCommissionValue.Location = new System.Drawing.Point(672, 448);
            this.lblTotalCommissionValue.Name = "lblTotalCommissionValue";
            this.lblTotalCommissionValue.Size = new System.Drawing.Size(0, 13);
            this.lblTotalCommissionValue.TabIndex = 147;
            // 
            // lblTotalCommissionsValue
            // 
            this.lblTotalCommissionsValue.AutoSize = true;
            this.lblTotalCommissionsValue.Location = new System.Drawing.Point(713, 448);
            this.lblTotalCommissionsValue.Name = "lblTotalCommissionsValue";
            this.lblTotalCommissionsValue.Size = new System.Drawing.Size(0, 13);
            this.lblTotalCommissionsValue.TabIndex = 146;
            // 
            // lblTotalCommissions
            // 
            this.lblTotalCommissions.AutoSize = true;
            this.lblTotalCommissions.Location = new System.Drawing.Point(581, 448);
            this.lblTotalCommissions.Name = "lblTotalCommissions";
            this.lblTotalCommissions.Size = new System.Drawing.Size(126, 13);
            this.lblTotalCommissions.TabIndex = 145;
            this.lblTotalCommissions.Text = "Total Sales Commissions:";
            // 
            // btnTransactionDetailsExcel
            // 
            this.btnTransactionDetailsExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTransactionDetailsExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnTransactionDetailsExcel.Image")));
            this.btnTransactionDetailsExcel.Location = new System.Drawing.Point(742, 115);
            this.btnTransactionDetailsExcel.Name = "btnTransactionDetailsExcel";
            this.btnTransactionDetailsExcel.Size = new System.Drawing.Size(32, 28);
            this.btnTransactionDetailsExcel.TabIndex = 144;
            this.btnTransactionDetailsExcel.Click += new System.EventHandler(this.btnTransactionDetailsExcel_Click);
            // 
            // lblCommissions
            // 
            this.lblCommissions.AutoSize = true;
            this.lblCommissions.Location = new System.Drawing.Point(13, 123);
            this.lblCommissions.Name = "lblCommissions";
            this.lblCommissions.Size = new System.Drawing.Size(128, 13);
            this.lblCommissions.TabIndex = 2;
            this.lblCommissions.Text = "Commission details results";
            // 
            // dgvCommissions
            // 
            this.dgvCommissions.AllowUserToAddRows = false;
            this.dgvCommissions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCommissions.Location = new System.Drawing.Point(13, 150);
            this.dgvCommissions.Name = "dgvCommissions";
            this.dgvCommissions.ReadOnly = true;
            this.dgvCommissions.Size = new System.Drawing.Size(767, 276);
            this.dgvCommissions.TabIndex = 1;
            // 
            // gbSearchOptions
            // 
            this.gbSearchOptions.Controls.Add(this.btnExit);
            this.gbSearchOptions.Controls.Add(this.btnSearch);
            this.gbSearchOptions.Controls.Add(this.drpEmployee);
            this.gbSearchOptions.Controls.Add(this.lblBranch);
            this.gbSearchOptions.Controls.Add(this.dtpDeliveryDateFrom);
            this.gbSearchOptions.Controls.Add(this.drpBranchNo);
            this.gbSearchOptions.Controls.Add(this.dtpDeliveryDateTo);
            this.gbSearchOptions.Controls.Add(this.lblEmployee);
            this.gbSearchOptions.Controls.Add(this.label3);
            this.gbSearchOptions.Controls.Add(this.label4);
            this.gbSearchOptions.Location = new System.Drawing.Point(12, 12);
            this.gbSearchOptions.Name = "gbSearchOptions";
            this.gbSearchOptions.Size = new System.Drawing.Size(768, 80);
            this.gbSearchOptions.TabIndex = 0;
            this.gbSearchOptions.TabStop = false;
            this.gbSearchOptions.Text = "Search Options";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(687, 49);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 151;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(687, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 150;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // drpEmployee
            // 
            this.drpEmployee.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpEmployee.Enabled = false;
            this.drpEmployee.FormattingEnabled = true;
            this.drpEmployee.Location = new System.Drawing.Point(83, 45);
            this.drpEmployee.Name = "drpEmployee";
            this.drpEmployee.Size = new System.Drawing.Size(184, 21);
            this.drpEmployee.TabIndex = 140;
            // 
            // lblBranch
            // 
            this.lblBranch.AutoSize = true;
            this.lblBranch.Location = new System.Drawing.Point(4, 27);
            this.lblBranch.Name = "lblBranch";
            this.lblBranch.Size = new System.Drawing.Size(41, 13);
            this.lblBranch.TabIndex = 149;
            this.lblBranch.Text = "Branch";
            // 
            // dtpDeliveryDateFrom
            // 
            this.dtpDeliveryDateFrom.Location = new System.Drawing.Point(291, 46);
            this.dtpDeliveryDateFrom.Name = "dtpDeliveryDateFrom";
            this.dtpDeliveryDateFrom.Size = new System.Drawing.Size(123, 20);
            this.dtpDeliveryDateFrom.TabIndex = 141;
            // 
            // drpBranchNo
            // 
            this.drpBranchNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranchNo.Enabled = false;
            this.drpBranchNo.FormattingEnabled = true;
            this.drpBranchNo.Location = new System.Drawing.Point(7, 45);
            this.drpBranchNo.Name = "drpBranchNo";
            this.drpBranchNo.Size = new System.Drawing.Size(59, 21);
            this.drpBranchNo.TabIndex = 148;
            this.drpBranchNo.SelectedIndexChanged += new System.EventHandler(this.drpBranchNo_SelectedIndexChanged);
            // 
            // dtpDeliveryDateTo
            // 
            this.dtpDeliveryDateTo.Location = new System.Drawing.Point(430, 46);
            this.dtpDeliveryDateTo.Name = "dtpDeliveryDateTo";
            this.dtpDeliveryDateTo.Size = new System.Drawing.Size(124, 20);
            this.dtpDeliveryDateTo.TabIndex = 142;
            // 
            // lblEmployee
            // 
            this.lblEmployee.AutoSize = true;
            this.lblEmployee.Location = new System.Drawing.Point(80, 27);
            this.lblEmployee.Name = "lblEmployee";
            this.lblEmployee.Size = new System.Drawing.Size(53, 13);
            this.lblEmployee.TabIndex = 144;
            this.lblEmployee.Text = "Employee";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(288, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 146;
            this.label3.Text = "Date From";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(427, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 147;
            this.label4.Text = "Date To";
            // 
            // SalesCommissionEnquiry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 488);
            this.Controls.Add(this.lblTotalCommissionValue);
            this.Controls.Add(this.lblTotalCommissionsValue);
            this.Controls.Add(this.lblTotalCommissions);
            this.Controls.Add(this.btnTransactionDetailsExcel);
            this.Controls.Add(this.lblCommissions);
            this.Controls.Add(this.dgvCommissions);
            this.Controls.Add(this.gbSearchOptions);
            this.Name = "SalesCommissionEnquiry";
            this.Text = "Sales Commission Enquiry";
            ((System.ComponentModel.ISupportInitialize)(this.dgvCommissions)).EndInit();
            this.gbSearchOptions.ResumeLayout(false);
            this.gbSearchOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbSearchOptions;
        private System.Windows.Forms.DataGridView dgvCommissions;
        private System.Windows.Forms.Label lblCommissions;
        private System.Windows.Forms.ComboBox drpEmployee;
        private System.Windows.Forms.Label lblBranch;
        private System.Windows.Forms.DateTimePicker dtpDeliveryDateFrom;
        private System.Windows.Forms.ComboBox drpBranchNo;
        private System.Windows.Forms.DateTimePicker dtpDeliveryDateTo;
        private System.Windows.Forms.Label lblEmployee;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnSearch;
        public System.Windows.Forms.Button btnTransactionDetailsExcel;
        private System.Windows.Forms.Label lblTotalCommissions;
        private System.Windows.Forms.Label lblTotalCommissionsValue;
        private System.Windows.Forms.Label lblTotalCommissionValue;
    }
}