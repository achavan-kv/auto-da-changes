namespace STL.PL
{
    partial class CommissionsReport
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommissionsReport));
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpCommDateTo = new System.Windows.Forms.DateTimePicker();
            this.dtpCommDateFrom = new System.Windows.Forms.DateTimePicker();
            this.dgvCommissionsDetail = new System.Windows.Forms.DataGridView();
            this.dgvCommissionsHeader = new System.Windows.Forms.DataGridView();
            this.chkStandardCommission = new System.Windows.Forms.CheckBox();
            this.chkSPIFF = new System.Windows.Forms.CheckBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnExcelHeader = new System.Windows.Forms.Button();
            this.btnExcelDetail = new System.Windows.Forms.Button();
            this.lblDetailLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.drpBranchNo = new System.Windows.Forms.ComboBox();
            this.lbRolePermission = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCommissionsDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCommissionsHeader)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(316, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Date To";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(125, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Date From";
            // 
            // dtpCommDateTo
            // 
            this.dtpCommDateTo.Location = new System.Drawing.Point(368, 17);
            this.dtpCommDateTo.Name = "dtpCommDateTo";
            this.dtpCommDateTo.Size = new System.Drawing.Size(124, 20);
            this.dtpCommDateTo.TabIndex = 10;
            // 
            // dtpCommDateFrom
            // 
            this.dtpCommDateFrom.Location = new System.Drawing.Point(187, 17);
            this.dtpCommDateFrom.Name = "dtpCommDateFrom";
            this.dtpCommDateFrom.Size = new System.Drawing.Size(123, 20);
            this.dtpCommDateFrom.TabIndex = 9;
            // 
            // dgvCommissionsDetail
            // 
            this.dgvCommissionsDetail.AllowUserToAddRows = false;
            this.dgvCommissionsDetail.AllowUserToDeleteRows = false;
            this.dgvCommissionsDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCommissionsDetail.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCommissionsDetail.Location = new System.Drawing.Point(3, 235);
            this.dgvCommissionsDetail.MultiSelect = false;
            this.dgvCommissionsDetail.Name = "dgvCommissionsDetail";
            this.dgvCommissionsDetail.ReadOnly = true;
            this.dgvCommissionsDetail.RowHeadersWidth = 20;
            this.dgvCommissionsDetail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCommissionsDetail.Size = new System.Drawing.Size(749, 240);
            this.dgvCommissionsDetail.TabIndex = 13;
            // 
            // dgvCommissionsHeader
            // 
            this.dgvCommissionsHeader.AllowUserToAddRows = false;
            this.dgvCommissionsHeader.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this.dgvCommissionsHeader.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCommissionsHeader.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCommissionsHeader.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCommissionsHeader.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvCommissionsHeader.Location = new System.Drawing.Point(3, 71);
            this.dgvCommissionsHeader.MultiSelect = false;
            this.dgvCommissionsHeader.Name = "dgvCommissionsHeader";
            this.dgvCommissionsHeader.ReadOnly = true;
            this.dgvCommissionsHeader.RowHeadersWidth = 20;
            this.dgvCommissionsHeader.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCommissionsHeader.Size = new System.Drawing.Size(749, 147);
            this.dgvCommissionsHeader.TabIndex = 14;
            this.dgvCommissionsHeader.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dgvCommissionsHeader_MouseClick);
            this.dgvCommissionsHeader.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCommissionsHeader_CellContentClick);
            // 
            // chkStandardCommission
            // 
            this.chkStandardCommission.Checked = true;
            this.chkStandardCommission.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkStandardCommission.Location = new System.Drawing.Point(513, 12);
            this.chkStandardCommission.Name = "chkStandardCommission";
            this.chkStandardCommission.Size = new System.Drawing.Size(86, 32);
            this.chkStandardCommission.TabIndex = 15;
            this.chkStandardCommission.Text = "Standard Commission";
            this.chkStandardCommission.UseVisualStyleBackColor = true;
            // 
            // chkSPIFF
            // 
            this.chkSPIFF.AutoSize = true;
            this.chkSPIFF.Checked = true;
            this.chkSPIFF.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSPIFF.Location = new System.Drawing.Point(605, 20);
            this.chkSPIFF.Name = "chkSPIFF";
            this.chkSPIFF.Size = new System.Drawing.Size(55, 17);
            this.chkSPIFF.TabIndex = 16;
            this.chkSPIFF.Text = "SPIFF";
            this.chkSPIFF.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(666, 16);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 17;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnExcelHeader
            // 
            this.btnExcelHeader.Enabled = false;
            this.btnExcelHeader.Image = ((System.Drawing.Image)(resources.GetObject("btnExcelHeader.Image")));
            this.btnExcelHeader.Location = new System.Drawing.Point(758, 184);
            this.btnExcelHeader.Name = "btnExcelHeader";
            this.btnExcelHeader.Size = new System.Drawing.Size(32, 32);
            this.btnExcelHeader.TabIndex = 138;
            this.btnExcelHeader.Click += new System.EventHandler(this.btnExcelHeader_Click);
            // 
            // btnExcelDetail
            // 
            this.btnExcelDetail.Enabled = false;
            this.btnExcelDetail.Image = ((System.Drawing.Image)(resources.GetObject("btnExcelDetail.Image")));
            this.btnExcelDetail.Location = new System.Drawing.Point(758, 444);
            this.btnExcelDetail.Name = "btnExcelDetail";
            this.btnExcelDetail.Size = new System.Drawing.Size(32, 32);
            this.btnExcelDetail.TabIndex = 139;
            this.btnExcelDetail.Click += new System.EventHandler(this.btnExcelDetail_Click);
            // 
            // lblDetailLabel
            // 
            this.lblDetailLabel.AutoSize = true;
            this.lblDetailLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetailLabel.Location = new System.Drawing.Point(2, 221);
            this.lblDetailLabel.Name = "lblDetailLabel";
            this.lblDetailLabel.Size = new System.Drawing.Size(144, 13);
            this.lblDetailLabel.TabIndex = 140;
            this.lblDetailLabel.Text = "Sales Commission Detail";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(2, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 13);
            this.label2.TabIndex = 141;
            this.label2.Text = "Sales Commissions";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.drpBranchNo);
            this.groupBox1.Controls.Add(this.dtpCommDateFrom);
            this.groupBox1.Controls.Add(this.btnLoad);
            this.groupBox1.Controls.Add(this.dtpCommDateTo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.chkStandardCommission);
            this.groupBox1.Controls.Add(this.chkSPIFF);
            this.groupBox1.Location = new System.Drawing.Point(5, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(747, 48);
            this.groupBox1.TabIndex = 142;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filter Options";
            // 
            // drpBranchNo
            // 
            this.drpBranchNo.FormattingEnabled = true;
            this.drpBranchNo.Location = new System.Drawing.Point(58, 18);
            this.drpBranchNo.Name = "drpBranchNo";
            this.drpBranchNo.Size = new System.Drawing.Size(61, 21);
            this.drpBranchNo.TabIndex = 19;
            this.drpBranchNo.SelectedIndexChanged += new System.EventHandler(this.drpBranchNo_SelectedIndexChanged);
            // 
            // lbRolePermission
            // 
            this.lbRolePermission.AutoSize = true;
            this.lbRolePermission.Enabled = false;
            this.lbRolePermission.ForeColor = System.Drawing.SystemColors.Control;
            this.lbRolePermission.Location = new System.Drawing.Point(130, 55);
            this.lbRolePermission.Name = "lbRolePermission";
            this.lbRolePermission.Size = new System.Drawing.Size(27, 13);
            this.lbRolePermission.TabIndex = 18;
            this.lbRolePermission.Text = "All   ";
            this.lbRolePermission.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Branch";
            // 
            // CommissionsReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.lbRolePermission);
            this.Controls.Add(this.dgvCommissionsHeader);
            this.Controls.Add(this.dgvCommissionsDetail);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnExcelDetail);
            this.Controls.Add(this.btnExcelHeader);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblDetailLabel);
            this.Name = "CommissionsReport";
            this.Text = "Sales Commissions Report by CSR";
            ((System.ComponentModel.ISupportInitialize)(this.dgvCommissionsDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCommissionsHeader)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpCommDateTo;
        private System.Windows.Forms.DateTimePicker dtpCommDateFrom;
        private System.Windows.Forms.DataGridView dgvCommissionsDetail;
        private System.Windows.Forms.DataGridView dgvCommissionsHeader;
        private System.Windows.Forms.CheckBox chkStandardCommission;
        private System.Windows.Forms.CheckBox chkSPIFF;
        private System.Windows.Forms.Button btnLoad;
        public System.Windows.Forms.Button btnExcelHeader;
        public System.Windows.Forms.Button btnExcelDetail;
        private System.Windows.Forms.Label lblDetailLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbRolePermission;
        private System.Windows.Forms.ComboBox drpBranchNo;
        private System.Windows.Forms.Label label1;
    }
}