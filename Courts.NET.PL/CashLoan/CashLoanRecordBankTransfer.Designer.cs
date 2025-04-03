namespace STL.PL.CashLoan
{
    partial class CashLoanRecordBankTransfer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CashLoanRecordBankTransfer));
            this.btnExit = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.dtBankTransferDate = new System.Windows.Forms.DateTimePicker();
            this.lblBankTransferDate = new System.Windows.Forms.Label();
            this.txtBankTransferReferenceNo = new System.Windows.Forms.TextBox();
            this.lblBakReferenceNo = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblBankAccountName = new System.Windows.Forms.Label();
            this.txtBankBranch = new System.Windows.Forms.TextBox();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.lblBankReferenceNo = new System.Windows.Forms.Label();
            this.txtBankAcctNo = new System.Windows.Forms.TextBox();
            this.txtBankAccountType = new System.Windows.Forms.TextBox();
            this.txtBank = new System.Windows.Forms.TextBox();
            this.txtLoanAmount = new System.Windows.Forms.TextBox();
            this.txtDisbursementType = new System.Windows.Forms.TextBox();
            this.txtCustId = new System.Windows.Forms.TextBox();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblBankAccountType = new System.Windows.Forms.Label();
            this.lblBankBranch = new System.Windows.Forms.Label();
            this.lBankAcctNo = new System.Windows.Forms.Label();
            this.lBank = new System.Windows.Forms.Label();
            this.lblDisbursementType = new System.Windows.Forms.Label();
            this.lblLoanAmount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.lblAcctNo = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.txtBankReferenceNo = new System.Windows.Forms.TextBox();
            this.txtBankAccountName = new System.Windows.Forms.TextBox();
            this.txtAccountNo = new STL.PL.AccountTextBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(668, 12);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 11;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSave);
            this.groupBox2.Controls.Add(this.dtBankTransferDate);
            this.groupBox2.Controls.Add(this.lblBankTransferDate);
            this.groupBox2.Controls.Add(this.txtBankTransferReferenceNo);
            this.groupBox2.Controls.Add(this.lblBakReferenceNo);
            this.groupBox2.Location = new System.Drawing.Point(412, 58);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(331, 138);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Bank Transfer Details";
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Location = new System.Drawing.Point(301, 102);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 110;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dtBankTransferDate
            // 
            this.dtBankTransferDate.CustomFormat = "ddd dd MMM yyyy";
            this.dtBankTransferDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtBankTransferDate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtBankTransferDate.Location = new System.Drawing.Point(112, 83);
            this.dtBankTransferDate.Name = "dtBankTransferDate";
            this.dtBankTransferDate.Size = new System.Drawing.Size(117, 20);
            this.dtBankTransferDate.TabIndex = 109;
            this.dtBankTransferDate.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // lblBankTransferDate
            // 
            this.lblBankTransferDate.AutoSize = true;
            this.lblBankTransferDate.Location = new System.Drawing.Point(6, 86);
            this.lblBankTransferDate.Name = "lblBankTransferDate";
            this.lblBankTransferDate.Size = new System.Drawing.Size(75, 13);
            this.lblBankTransferDate.TabIndex = 108;
            this.lblBankTransferDate.Text = "Transfer Date:";
            this.lblBankTransferDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtBankTransferReferenceNo
            // 
            this.txtBankTransferReferenceNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtBankTransferReferenceNo.Location = new System.Drawing.Point(112, 47);
            this.txtBankTransferReferenceNo.MaxLength = 30;
            this.txtBankTransferReferenceNo.Name = "txtBankTransferReferenceNo";
            this.txtBankTransferReferenceNo.Size = new System.Drawing.Size(191, 20);
            this.txtBankTransferReferenceNo.TabIndex = 107;
            // 
            // lblBakReferenceNo
            // 
            this.lblBakReferenceNo.AutoSize = true;
            this.lblBakReferenceNo.Location = new System.Drawing.Point(6, 48);
            this.lblBakReferenceNo.Name = "lblBakReferenceNo";
            this.lblBakReferenceNo.Size = new System.Drawing.Size(100, 13);
            this.lblBakReferenceNo.TabIndex = 1;
            this.lblBakReferenceNo.Text = "Reference Number:";
            this.lblBakReferenceNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtBankAccountName);
            this.groupBox1.Controls.Add(this.txtBankReferenceNo);
            this.groupBox1.Controls.Add(this.lblBankAccountName);
            this.groupBox1.Controls.Add(this.txtBankBranch);
            this.groupBox1.Controls.Add(this.txtNotes);
            this.groupBox1.Controls.Add(this.lblBankReferenceNo);
            this.groupBox1.Controls.Add(this.txtBankAcctNo);
            this.groupBox1.Controls.Add(this.txtBankAccountType);
            this.groupBox1.Controls.Add(this.txtBank);
            this.groupBox1.Controls.Add(this.txtLoanAmount);
            this.groupBox1.Controls.Add(this.txtDisbursementType);
            this.groupBox1.Controls.Add(this.txtCustId);
            this.groupBox1.Controls.Add(this.txtCustomerName);
            this.groupBox1.Controls.Add(this.lblCustomerName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lblBankAccountType);
            this.groupBox1.Controls.Add(this.lblBankBranch);
            this.groupBox1.Controls.Add(this.lBankAcctNo);
            this.groupBox1.Controls.Add(this.lBank);
            this.groupBox1.Controls.Add(this.lblDisbursementType);
            this.groupBox1.Controls.Add(this.lblLoanAmount);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(21, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(381, 414);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Banking Details";
            // 
            // lblBankAccountName
            // 
            this.lblBankAccountName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblBankAccountName.Location = new System.Drawing.Point(6, 297);
            this.lblBankAccountName.Name = "lblBankAccountName";
            this.lblBankAccountName.Size = new System.Drawing.Size(101, 18);
            this.lblBankAccountName.TabIndex = 105;
            this.lblBankAccountName.Text = "Name on Account:";
            this.lblBankAccountName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtBankBranch
            // 
            this.txtBankBranch.BackColor = System.Drawing.SystemColors.Control;
            this.txtBankBranch.Location = new System.Drawing.Point(115, 202);
            this.txtBankBranch.MaxLength = 20;
            this.txtBankBranch.Name = "txtBankBranch";
            this.txtBankBranch.ReadOnly = true;
            this.txtBankBranch.Size = new System.Drawing.Size(152, 20);
            this.txtBankBranch.TabIndex = 109;
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(113, 329);
            this.txtNotes.MaxLength = 200;
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ReadOnly = true;
            this.txtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNotes.Size = new System.Drawing.Size(257, 78);
            this.txtNotes.TabIndex = 108;
            // 
            // lblBankReferenceNo
            // 
            this.lblBankReferenceNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblBankReferenceNo.Location = new System.Drawing.Point(5, 265);
            this.lblBankReferenceNo.Name = "lblBankReferenceNo";
            this.lblBankReferenceNo.Size = new System.Drawing.Size(89, 18);
            this.lblBankReferenceNo.TabIndex = 103;
            this.lblBankReferenceNo.Text = "Bank Reference:";
            this.lblBankReferenceNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtBankAcctNo
            // 
            this.txtBankAcctNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtBankAcctNo.Location = new System.Drawing.Point(114, 234);
            this.txtBankAcctNo.MaxLength = 20;
            this.txtBankAcctNo.Name = "txtBankAcctNo";
            this.txtBankAcctNo.ReadOnly = true;
            this.txtBankAcctNo.Size = new System.Drawing.Size(184, 20);
            this.txtBankAcctNo.TabIndex = 107;
            // 
            // txtBankAccountType
            // 
            this.txtBankAccountType.BackColor = System.Drawing.SystemColors.Control;
            this.txtBankAccountType.Location = new System.Drawing.Point(115, 169);
            this.txtBankAccountType.Name = "txtBankAccountType";
            this.txtBankAccountType.ReadOnly = true;
            this.txtBankAccountType.Size = new System.Drawing.Size(184, 20);
            this.txtBankAccountType.TabIndex = 106;
            // 
            // txtBank
            // 
            this.txtBank.BackColor = System.Drawing.SystemColors.Control;
            this.txtBank.Location = new System.Drawing.Point(115, 138);
            this.txtBank.Name = "txtBank";
            this.txtBank.ReadOnly = true;
            this.txtBank.Size = new System.Drawing.Size(184, 20);
            this.txtBank.TabIndex = 104;
            // 
            // txtLoanAmount
            // 
            this.txtLoanAmount.BackColor = System.Drawing.SystemColors.Control;
            this.txtLoanAmount.Location = new System.Drawing.Point(116, 107);
            this.txtLoanAmount.Name = "txtLoanAmount";
            this.txtLoanAmount.ReadOnly = true;
            this.txtLoanAmount.Size = new System.Drawing.Size(101, 20);
            this.txtLoanAmount.TabIndex = 99;
            // 
            // txtDisbursementType
            // 
            this.txtDisbursementType.BackColor = System.Drawing.SystemColors.Control;
            this.txtDisbursementType.Location = new System.Drawing.Point(116, 77);
            this.txtDisbursementType.Name = "txtDisbursementType";
            this.txtDisbursementType.ReadOnly = true;
            this.txtDisbursementType.Size = new System.Drawing.Size(128, 20);
            this.txtDisbursementType.TabIndex = 97;
            // 
            // txtCustId
            // 
            this.txtCustId.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustId.Location = new System.Drawing.Point(116, 19);
            this.txtCustId.Name = "txtCustId";
            this.txtCustId.ReadOnly = true;
            this.txtCustId.Size = new System.Drawing.Size(122, 20);
            this.txtCustId.TabIndex = 94;
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerName.Location = new System.Drawing.Point(116, 47);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(204, 20);
            this.txtCustomerName.TabIndex = 95;
            // 
            // lblCustomerName
            // 
            this.lblCustomerName.AutoSize = true;
            this.lblCustomerName.Location = new System.Drawing.Point(7, 47);
            this.lblCustomerName.Name = "lblCustomerName";
            this.lblCustomerName.Size = new System.Drawing.Size(85, 13);
            this.lblCustomerName.TabIndex = 93;
            this.lblCustomerName.Text = "Customer Name:";
            this.lblCustomerName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(7, 329);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 24);
            this.label2.TabIndex = 92;
            this.label2.Text = "Notes:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBankAccountType
            // 
            this.lblBankAccountType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblBankAccountType.Location = new System.Drawing.Point(6, 169);
            this.lblBankAccountType.Name = "lblBankAccountType";
            this.lblBankAccountType.Size = new System.Drawing.Size(110, 18);
            this.lblBankAccountType.TabIndex = 91;
            this.lblBankAccountType.Text = "Bank Account Type:";
            this.lblBankAccountType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBankBranch
            // 
            this.lblBankBranch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblBankBranch.Location = new System.Drawing.Point(6, 199);
            this.lblBankBranch.Name = "lblBankBranch";
            this.lblBankBranch.Size = new System.Drawing.Size(73, 24);
            this.lblBankBranch.TabIndex = 90;
            this.lblBankBranch.Text = "Bank Branch:";
            this.lblBankBranch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lBankAcctNo
            // 
            this.lBankAcctNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lBankAcctNo.Location = new System.Drawing.Point(5, 228);
            this.lBankAcctNo.Name = "lBankAcctNo";
            this.lBankAcctNo.Size = new System.Drawing.Size(102, 31);
            this.lBankAcctNo.TabIndex = 87;
            this.lBankAcctNo.Text = "Bank Account No:";
            this.lBankAcctNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lBank
            // 
            this.lBank.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lBank.Location = new System.Drawing.Point(7, 136);
            this.lBank.Name = "lBank";
            this.lBank.Size = new System.Drawing.Size(38, 18);
            this.lBank.TabIndex = 86;
            this.lBank.Text = "Bank:";
            this.lBank.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDisbursementType
            // 
            this.lblDisbursementType.AutoSize = true;
            this.lblDisbursementType.Location = new System.Drawing.Point(6, 77);
            this.lblDisbursementType.Name = "lblDisbursementType";
            this.lblDisbursementType.Size = new System.Drawing.Size(101, 13);
            this.lblDisbursementType.TabIndex = 2;
            this.lblDisbursementType.Text = "Disbursement Type:";
            this.lblDisbursementType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLoanAmount
            // 
            this.lblLoanAmount.AutoSize = true;
            this.lblLoanAmount.Location = new System.Drawing.Point(7, 107);
            this.lblLoanAmount.Name = "lblLoanAmount";
            this.lblLoanAmount.Size = new System.Drawing.Size(73, 13);
            this.lblLoanAmount.TabIndex = 1;
            this.lblLoanAmount.Text = "Loan Amount:";
            this.lblLoanAmount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Customer Id:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grpSearch
            // 
            this.grpSearch.Controls.Add(this.lblAcctNo);
            this.grpSearch.Controls.Add(this.btnSearch);
            this.grpSearch.Controls.Add(this.txtAccountNo);
            this.grpSearch.Location = new System.Drawing.Point(21, 5);
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.Size = new System.Drawing.Size(626, 47);
            this.grpSearch.TabIndex = 0;
            this.grpSearch.TabStop = false;
            this.grpSearch.Text = "Search";
            // 
            // lblAcctNo
            // 
            this.lblAcctNo.AutoSize = true;
            this.lblAcctNo.Location = new System.Drawing.Point(6, 19);
            this.lblAcctNo.Name = "lblAcctNo";
            this.lblAcctNo.Size = new System.Drawing.Size(90, 13);
            this.lblAcctNo.TabIndex = 55;
            this.lblAcctNo.Text = "Account Number:";
            // 
            // btnSearch
            // 
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.Location = new System.Drawing.Point(588, 11);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(32, 32);
            this.btnSearch.TabIndex = 54;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // txtBankReferenceNo
            // 
            this.txtBankReferenceNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtBankReferenceNo.Location = new System.Drawing.Point(114, 265);
            this.txtBankReferenceNo.MaxLength = 20;
            this.txtBankReferenceNo.Name = "txtBankReferenceNo";
            this.txtBankReferenceNo.ReadOnly = true;
            this.txtBankReferenceNo.Size = new System.Drawing.Size(82, 20);
            this.txtBankReferenceNo.TabIndex = 110;
            // 
            // txtBankAccountName
            // 
            this.txtBankAccountName.BackColor = System.Drawing.SystemColors.Control;
            this.txtBankAccountName.Location = new System.Drawing.Point(113, 297);
            this.txtBankAccountName.MaxLength = 20;
            this.txtBankAccountName.Name = "txtBankAccountName";
            this.txtBankAccountName.ReadOnly = true;
            this.txtBankAccountName.Size = new System.Drawing.Size(198, 20);
            this.txtBankAccountName.TabIndex = 111;
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.Location = new System.Drawing.Point(102, 16);
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.PreventPaste = false;
            this.txtAccountNo.Size = new System.Drawing.Size(94, 20);
            this.txtAccountNo.TabIndex = 53;
            this.txtAccountNo.Text = "000-0000-0000-0";
            // 
            // CashLoanRecordBankTransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpSearch);
            this.Name = "CashLoanRecordBankTransfer";
            this.Text = "Cash Loan Record Bank Transfer";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpSearch.ResumeLayout(false);
            this.grpSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSearch;
        private System.Windows.Forms.Label lblAcctNo;
        private System.Windows.Forms.Button btnSearch;
        private AccountTextBox txtAccountNo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblLoanAmount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblDisbursementType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblBankAccountType;
        private System.Windows.Forms.Label lblBankBranch;
        private System.Windows.Forms.Label lBankAcctNo;
        private System.Windows.Forms.Label lBank;
        private System.Windows.Forms.Label lblCustomerName;
        private System.Windows.Forms.TextBox txtCustId;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.TextBox txtDisbursementType;
        private System.Windows.Forms.TextBox txtLoanAmount;
        private System.Windows.Forms.TextBox txtBank;
        private System.Windows.Forms.TextBox txtBankAccountType;
        private System.Windows.Forms.TextBox txtBankBranch;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.TextBox txtBankAcctNo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtBankTransferReferenceNo;
        private System.Windows.Forms.Label lblBakReferenceNo;
        private System.Windows.Forms.Label lblBankTransferDate;
        private System.Windows.Forms.DateTimePicker dtBankTransferDate;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label lblBankAccountName;
        private System.Windows.Forms.Label lblBankReferenceNo;
        private System.Windows.Forms.TextBox txtBankReferenceNo;
        private System.Windows.Forms.TextBox txtBankAccountName;
    }
}