namespace STL.PL
{
    partial class PrizeVoucher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrizeVoucher));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dtDateTo = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtDateFrom = new System.Windows.Forms.DateTimePicker();
            this.drpBranch = new System.Windows.Forms.ComboBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.txtAccountNo = new STL.PL.AccountTextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtInvoiceNo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCustID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lDelete = new System.Windows.Forms.Label();
            this.lReprint = new System.Windows.Forms.Label();
            this.btnAdditional = new System.Windows.Forms.Button();
            this.btnReprint = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.dgvVouchers = new System.Windows.Forms.DataGridView();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVouchers)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.CausesValidation = false;
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.dtDateTo);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.dtDateFrom);
            this.groupBox1.Controls.Add(this.drpBranch);
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.txtAccountNo);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.txtInvoiceNo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtCustID);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 133);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search Criteria";
            // 
            // label6
            // 
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(377, 96);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 16);
            this.label6.TabIndex = 44;
            this.label6.Text = "And";
            // 
            // dtDateTo
            // 
            this.dtDateTo.CustomFormat = "dd MMM yyyy";
            this.dtDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDateTo.Location = new System.Drawing.Point(409, 94);
            this.dtDateTo.Name = "dtDateTo";
            this.dtDateTo.Size = new System.Drawing.Size(112, 20);
            this.dtDateTo.TabIndex = 43;
            this.dtDateTo.Tag = "";
            this.dtDateTo.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            // 
            // label5
            // 
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(100, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(153, 16);
            this.label5.TabIndex = 42;
            this.label5.Text = "Date Issued Between";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(445, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 16);
            this.label4.TabIndex = 41;
            this.label4.Text = "Branch:";
            // 
            // dtDateFrom
            // 
            this.dtDateFrom.CustomFormat = "dd MMM yyyy";
            this.dtDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDateFrom.Location = new System.Drawing.Point(258, 94);
            this.dtDateFrom.Name = "dtDateFrom";
            this.dtDateFrom.Size = new System.Drawing.Size(112, 20);
            this.dtDateFrom.TabIndex = 40;
            this.dtDateFrom.Tag = "";
            this.dtDateFrom.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            // 
            // drpBranch
            // 
            this.drpBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranch.Location = new System.Drawing.Point(448, 47);
            this.drpBranch.Name = "drpBranch";
            this.drpBranch.Size = new System.Drawing.Size(85, 21);
            this.drpBranch.TabIndex = 39;
            // 
            // btnClear
            // 
            this.btnClear.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClear.Location = new System.Drawing.Point(626, 77);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 18;
            this.btnClear.Text = "&Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.Location = new System.Drawing.Point(75, 48);
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.Size = new System.Drawing.Size(88, 20);
            this.txtAccountNo.TabIndex = 1;
            this.txtAccountNo.Tag = "ACCNO";
            this.txtAccountNo.Text = "000-0000-0000-0";
            // 
            // btnSearch
            // 
            this.btnSearch.CausesValidation = false;
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSearch.Location = new System.Drawing.Point(626, 37);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 17;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtInvoiceNo
            // 
            this.txtInvoiceNo.Location = new System.Drawing.Point(320, 48);
            this.txtInvoiceNo.MaxLength = 30;
            this.txtInvoiceNo.Name = "txtInvoiceNo";
            this.txtInvoiceNo.Size = new System.Drawing.Size(88, 20);
            this.txtInvoiceNo.TabIndex = 5;
            this.txtInvoiceNo.Tag = "";
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(317, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Invoice No:";
            // 
            // txtCustID
            // 
            this.txtCustID.Location = new System.Drawing.Point(195, 48);
            this.txtCustID.MaxLength = 20;
            this.txtCustID.Name = "txtCustID";
            this.txtCustID.Size = new System.Drawing.Size(88, 20);
            this.txtCustID.TabIndex = 3;
            this.txtCustID.Tag = "";
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(195, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Customer ID:";
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(72, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Account Number:";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.CausesValidation = false;
            this.groupBox2.Controls.Add(this.lDelete);
            this.groupBox2.Controls.Add(this.lReprint);
            this.groupBox2.Controls.Add(this.btnAdditional);
            this.groupBox2.Controls.Add(this.btnReprint);
            this.groupBox2.Controls.Add(this.btnDelete);
            this.groupBox2.Controls.Add(this.dgvVouchers);
            this.groupBox2.Location = new System.Drawing.Point(8, 147);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(776, 318);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Search Results";
            // 
            // lDelete
            // 
            this.lDelete.AutoSize = true;
            this.lDelete.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lDelete.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lDelete.Location = new System.Drawing.Point(90, 275);
            this.lDelete.Name = "lDelete";
            this.lDelete.Size = new System.Drawing.Size(35, 13);
            this.lDelete.TabIndex = 23;
            this.lDelete.Text = "label8";
            this.lDelete.Visible = false;
            // 
            // lReprint
            // 
            this.lReprint.AutoSize = true;
            this.lReprint.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lReprint.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lReprint.Location = new System.Drawing.Point(31, 275);
            this.lReprint.Name = "lReprint";
            this.lReprint.Size = new System.Drawing.Size(35, 13);
            this.lReprint.TabIndex = 22;
            this.lReprint.Text = "label7";
            this.lReprint.Visible = false;
            // 
            // btnAdditional
            // 
            this.btnAdditional.Enabled = false;
            this.btnAdditional.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAdditional.Location = new System.Drawing.Point(185, 266);
            this.btnAdditional.Name = "btnAdditional";
            this.btnAdditional.Size = new System.Drawing.Size(94, 43);
            this.btnAdditional.TabIndex = 21;
            this.btnAdditional.Text = "Print Unprinted Vouchers";
            this.btnAdditional.Click += new System.EventHandler(this.btnAdditional_Click);
            // 
            // btnReprint
            // 
            this.btnReprint.Enabled = false;
            this.btnReprint.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnReprint.Location = new System.Drawing.Point(347, 266);
            this.btnReprint.Name = "btnReprint";
            this.btnReprint.Size = new System.Drawing.Size(94, 43);
            this.btnReprint.TabIndex = 20;
            this.btnReprint.Text = "Reprint Vouchers";
            this.btnReprint.Click += new System.EventHandler(this.btnReprint_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnDelete.Location = new System.Drawing.Point(507, 266);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(94, 43);
            this.btnDelete.TabIndex = 19;
            this.btnDelete.Text = "Delete Vouchers";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // dgvVouchers
            // 
            this.dgvVouchers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVouchers.Location = new System.Drawing.Point(12, 21);
            this.dgvVouchers.Name = "dgvVouchers";
            this.dgvVouchers.Size = new System.Drawing.Size(752, 237);
            this.dgvVouchers.TabIndex = 0;
            this.dgvVouchers.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvVouchers_MouseUp);
            // 
            // PrizeVoucher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrizeVoucher";
            this.Text = "PrizeVoucher";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVouchers)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnClear;
        private AccountTextBox txtAccountNo;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtInvoiceNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCustID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvVouchers;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtDateFrom;
        private System.Windows.Forms.ComboBox drpBranch;
        private System.Windows.Forms.Button btnAdditional;
        private System.Windows.Forms.Button btnReprint;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.DateTimePicker dtDateTo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lDelete;
        private System.Windows.Forms.Label lReprint;
    }
}