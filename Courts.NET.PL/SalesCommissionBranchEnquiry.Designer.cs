namespace STL.PL
{
    partial class SalesCommissionBranchEnquiry
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SalesCommissionBranchEnquiry));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblOverallCommissionsTot = new System.Windows.Forms.Label();
            this.btnTransactionDetailsExcel = new System.Windows.Forms.Button();
            this.lblCommissions = new System.Windows.Forms.Label();
            this.dgvCommissions = new System.Windows.Forms.DataGridView();
            this.gbSearchOptions = new System.Windows.Forms.GroupBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblBranch = new System.Windows.Forms.Label();
            this.dtpDeliveryDateFrom = new System.Windows.Forms.DateTimePicker();
            this.drpBranchNo = new System.Windows.Forms.ComboBox();
            this.dtpDeliveryDateTo = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblTotCommissionableValue = new System.Windows.Forms.Label();
            this.lblTotProductCommission = new System.Windows.Forms.Label();
            this.lblTotTermsTypeCommission = new System.Windows.Forms.Label();
            this.lblTotWarrantyCommission = new System.Windows.Forms.Label();
            this.lblTotals = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblTotalCommission = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCommissions)).BeginInit();
            this.gbSearchOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblOverallCommissionsTot
            // 
            this.lblOverallCommissionsTot.AutoSize = true;
            this.lblOverallCommissionsTot.Location = new System.Drawing.Point(508, 461);
            this.lblOverallCommissionsTot.Name = "lblOverallCommissionsTot";
            this.lblOverallCommissionsTot.Size = new System.Drawing.Size(162, 13);
            this.lblOverallCommissionsTot.TabIndex = 152;
            this.lblOverallCommissionsTot.Text = "Overall Total Sales Commissions:";
            // 
            // btnTransactionDetailsExcel
            // 
            this.btnTransactionDetailsExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTransactionDetailsExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnTransactionDetailsExcel.Image")));
            this.btnTransactionDetailsExcel.Location = new System.Drawing.Point(742, 123);
            this.btnTransactionDetailsExcel.Name = "btnTransactionDetailsExcel";
            this.btnTransactionDetailsExcel.Size = new System.Drawing.Size(32, 28);
            this.btnTransactionDetailsExcel.TabIndex = 151;
            this.btnTransactionDetailsExcel.Click += new System.EventHandler(this.btnTransactionDetailsExcel_Click);
            // 
            // lblCommissions
            // 
            this.lblCommissions.AutoSize = true;
            this.lblCommissions.Location = new System.Drawing.Point(13, 131);
            this.lblCommissions.Name = "lblCommissions";
            this.lblCommissions.Size = new System.Drawing.Size(128, 13);
            this.lblCommissions.TabIndex = 150;
            this.lblCommissions.Text = "Commission details results";
            // 
            // dgvCommissions
            // 
            this.dgvCommissions.AllowUserToAddRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCommissions.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCommissions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCommissions.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCommissions.Location = new System.Drawing.Point(13, 158);
            this.dgvCommissions.Name = "dgvCommissions";
            this.dgvCommissions.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCommissions.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCommissions.Size = new System.Drawing.Size(767, 251);
            this.dgvCommissions.TabIndex = 149;
            this.dgvCommissions.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvCommissions_MouseUp);
            // 
            // gbSearchOptions
            // 
            this.gbSearchOptions.Controls.Add(this.btnExit);
            this.gbSearchOptions.Controls.Add(this.btnSearch);
            this.gbSearchOptions.Controls.Add(this.lblBranch);
            this.gbSearchOptions.Controls.Add(this.dtpDeliveryDateFrom);
            this.gbSearchOptions.Controls.Add(this.drpBranchNo);
            this.gbSearchOptions.Controls.Add(this.dtpDeliveryDateTo);
            this.gbSearchOptions.Controls.Add(this.label3);
            this.gbSearchOptions.Controls.Add(this.label4);
            this.gbSearchOptions.Location = new System.Drawing.Point(12, 20);
            this.gbSearchOptions.Name = "gbSearchOptions";
            this.gbSearchOptions.Size = new System.Drawing.Size(768, 80);
            this.gbSearchOptions.TabIndex = 148;
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
            this.dtpDeliveryDateFrom.Location = new System.Drawing.Point(79, 45);
            this.dtpDeliveryDateFrom.Name = "dtpDeliveryDateFrom";
            this.dtpDeliveryDateFrom.Size = new System.Drawing.Size(123, 20);
            this.dtpDeliveryDateFrom.TabIndex = 141;
            // 
            // drpBranchNo
            // 
            this.drpBranchNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranchNo.FormattingEnabled = true;
            this.drpBranchNo.Location = new System.Drawing.Point(7, 45);
            this.drpBranchNo.Name = "drpBranchNo";
            this.drpBranchNo.Size = new System.Drawing.Size(59, 21);
            this.drpBranchNo.TabIndex = 148;
            this.drpBranchNo.SelectedIndexChanged += new System.EventHandler(this.drpBranchNo_SelectedIndexChanged);
            // 
            // dtpDeliveryDateTo
            // 
            this.dtpDeliveryDateTo.Location = new System.Drawing.Point(218, 46);
            this.dtpDeliveryDateTo.Name = "dtpDeliveryDateTo";
            this.dtpDeliveryDateTo.Size = new System.Drawing.Size(124, 20);
            this.dtpDeliveryDateTo.TabIndex = 142;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(76, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 146;
            this.label3.Text = "Date From";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(215, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 147;
            this.label4.Text = "Date To";
            // 
            // lblTotCommissionableValue
            // 
            this.lblTotCommissionableValue.AutoSize = true;
            this.lblTotCommissionableValue.Location = new System.Drawing.Point(205, 425);
            this.lblTotCommissionableValue.Name = "lblTotCommissionableValue";
            this.lblTotCommissionableValue.Size = new System.Drawing.Size(0, 13);
            this.lblTotCommissionableValue.TabIndex = 155;
            // 
            // lblTotProductCommission
            // 
            this.lblTotProductCommission.AutoSize = true;
            this.lblTotProductCommission.Location = new System.Drawing.Point(305, 425);
            this.lblTotProductCommission.Name = "lblTotProductCommission";
            this.lblTotProductCommission.Size = new System.Drawing.Size(0, 13);
            this.lblTotProductCommission.TabIndex = 156;
            // 
            // lblTotTermsTypeCommission
            // 
            this.lblTotTermsTypeCommission.AutoSize = true;
            this.lblTotTermsTypeCommission.Location = new System.Drawing.Point(406, 425);
            this.lblTotTermsTypeCommission.Name = "lblTotTermsTypeCommission";
            this.lblTotTermsTypeCommission.Size = new System.Drawing.Size(0, 13);
            this.lblTotTermsTypeCommission.TabIndex = 157;
            // 
            // lblTotWarrantyCommission
            // 
            this.lblTotWarrantyCommission.AutoSize = true;
            this.lblTotWarrantyCommission.Location = new System.Drawing.Point(506, 425);
            this.lblTotWarrantyCommission.Name = "lblTotWarrantyCommission";
            this.lblTotWarrantyCommission.Size = new System.Drawing.Size(0, 13);
            this.lblTotWarrantyCommission.TabIndex = 158;
            // 
            // lblTotals
            // 
            this.lblTotals.AutoSize = true;
            this.lblTotals.Location = new System.Drawing.Point(9, 425);
            this.lblTotals.Name = "lblTotals";
            this.lblTotals.Size = new System.Drawing.Size(52, 13);
            this.lblTotals.TabIndex = 159;
            this.lblTotals.Text = "TOTALS:";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(608, 425);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(0, 13);
            this.lblTotal.TabIndex = 160;
            // 
            // lblTotalCommission
            // 
            this.lblTotalCommission.AutoSize = true;
            this.lblTotalCommission.Location = new System.Drawing.Point(712, 461);
            this.lblTotalCommission.Name = "lblTotalCommission";
            this.lblTotalCommission.Size = new System.Drawing.Size(0, 13);
            this.lblTotalCommission.TabIndex = 161;
            // 
            // SalesCommissionBranchEnquiry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 488);
            this.Controls.Add(this.lblTotalCommission);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.lblTotals);
            this.Controls.Add(this.lblTotWarrantyCommission);
            this.Controls.Add(this.lblTotTermsTypeCommission);
            this.Controls.Add(this.lblTotProductCommission);
            this.Controls.Add(this.lblTotCommissionableValue);
            this.Controls.Add(this.lblOverallCommissionsTot);
            this.Controls.Add(this.btnTransactionDetailsExcel);
            this.Controls.Add(this.lblCommissions);
            this.Controls.Add(this.dgvCommissions);
            this.Controls.Add(this.gbSearchOptions);
            this.Name = "SalesCommissionBranchEnquiry";
            this.Text = "Sales Commission Branch Enquiry";
            ((System.ComponentModel.ISupportInitialize)(this.dgvCommissions)).EndInit();
            this.gbSearchOptions.ResumeLayout(false);
            this.gbSearchOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblOverallCommissionsTot;
        public System.Windows.Forms.Button btnTransactionDetailsExcel;
        private System.Windows.Forms.Label lblCommissions;
        private System.Windows.Forms.DataGridView dgvCommissions;
        private System.Windows.Forms.GroupBox gbSearchOptions;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lblBranch;
        private System.Windows.Forms.DateTimePicker dtpDeliveryDateFrom;
        private System.Windows.Forms.ComboBox drpBranchNo;
        private System.Windows.Forms.DateTimePicker dtpDeliveryDateTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblTotCommissionableValue;
        private System.Windows.Forms.Label lblTotProductCommission;
        private System.Windows.Forms.Label lblTotTermsTypeCommission;
        private System.Windows.Forms.Label lblTotWarrantyCommission;
        private System.Windows.Forms.Label lblTotals;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblTotalCommission;
    }
}