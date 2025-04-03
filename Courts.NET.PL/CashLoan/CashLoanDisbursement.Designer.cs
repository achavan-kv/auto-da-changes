namespace STL.PL.CashLoan
{
    partial class CashLoanDisbursement
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtBankBranch = new System.Windows.Forms.TextBox();
            this.txtCardNo = new System.Windows.Forms.TextBox();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.drpBankAccountType = new System.Windows.Forms.ComboBox();
            this.lblBankAccountType = new System.Windows.Forms.Label();
            this.lblBankBranch = new System.Windows.Forms.Label();
            this.mtb_CardNo = new System.Windows.Forms.MaskedTextBox();
            this.lDisbursementType = new System.Windows.Forms.Label();
            this.drpCardType = new System.Windows.Forms.ComboBox();
            this.lCardType = new System.Windows.Forms.Label();
            this.drpBank = new System.Windows.Forms.ComboBox();
            this.drpDisbursementType = new System.Windows.Forms.ComboBox();
            this.lBankAcctNo = new System.Windows.Forms.Label();
            this.lBank = new System.Windows.Forms.Label();
            this.txtBankAcctNo = new System.Windows.Forms.TextBox();
            this.lCardNo = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnPrintReceipt = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblCashierPaymentReminder = new System.Windows.Forms.Label();
            this.txtAcctNo = new STL.PL.AccountTextBox();
            this.txtCustId = new System.Windows.Forms.TextBox();
            this.lblDisburse = new System.Windows.Forms.Label();
            this.lblCustId = new System.Windows.Forms.Label();
            this.txtDisburseAmt = new System.Windows.Forms.TextBox();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.lblAccountNo = new System.Windows.Forms.Label();
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.txtBankAccountName = new System.Windows.Forms.TextBox();
            this.lblBankAccountName = new System.Windows.Forms.Label();
            this.txtBankReferenceNo = new System.Windows.Forms.TextBox();
            this.lblBankReferenceNo = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtBankAccountName);
            this.groupBox2.Controls.Add(this.txtBankBranch);
            this.groupBox2.Controls.Add(this.lblBankAccountName);
            this.groupBox2.Controls.Add(this.txtCardNo);
            this.groupBox2.Controls.Add(this.txtBankReferenceNo);
            this.groupBox2.Controls.Add(this.txtNotes);
            this.groupBox2.Controls.Add(this.lblBankReferenceNo);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.drpBankAccountType);
            this.groupBox2.Controls.Add(this.lblBankAccountType);
            this.groupBox2.Controls.Add(this.lblBankBranch);
            this.groupBox2.Controls.Add(this.mtb_CardNo);
            this.groupBox2.Controls.Add(this.lDisbursementType);
            this.groupBox2.Controls.Add(this.drpCardType);
            this.groupBox2.Controls.Add(this.lCardType);
            this.groupBox2.Controls.Add(this.drpBank);
            this.groupBox2.Controls.Add(this.drpDisbursementType);
            this.groupBox2.Controls.Add(this.lBankAcctNo);
            this.groupBox2.Controls.Add(this.lBank);
            this.groupBox2.Controls.Add(this.txtBankAcctNo);
            this.groupBox2.Controls.Add(this.lCardNo);
            this.groupBox2.Location = new System.Drawing.Point(380, 17);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(409, 439);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Disbursement Method";
            // 
            // txtBankBranch
            // 
            this.txtBankBranch.BackColor = System.Drawing.SystemColors.Window;
            this.txtBankBranch.Location = new System.Drawing.Point(126, 224);
            this.txtBankBranch.MaxLength = 20;
            this.txtBankBranch.Name = "txtBankBranch";
            this.txtBankBranch.Size = new System.Drawing.Size(152, 20);
            this.txtBankBranch.TabIndex = 88;
            // 
            // txtCardNo
            // 
            this.txtCardNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtCardNo.Location = new System.Drawing.Point(126, 114);
            this.txtCardNo.MaxLength = 30;
            this.txtCardNo.Name = "txtCardNo";
            this.txtCardNo.Size = new System.Drawing.Size(152, 20);
            this.txtCardNo.TabIndex = 87;
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(126, 362);
            this.txtNotes.MaxLength = 200;
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNotes.Size = new System.Drawing.Size(257, 54);
            this.txtNotes.TabIndex = 86;
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(6, 359);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 24);
            this.label1.TabIndex = 85;
            this.label1.Text = "Notes";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpBankAccountType
            // 
            this.drpBankAccountType.BackColor = System.Drawing.SystemColors.Window;
            this.drpBankAccountType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBankAccountType.Location = new System.Drawing.Point(126, 187);
            this.drpBankAccountType.Name = "drpBankAccountType";
            this.drpBankAccountType.Size = new System.Drawing.Size(184, 21);
            this.drpBankAccountType.TabIndex = 84;
            // 
            // lblBankAccountType
            // 
            this.lblBankAccountType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblBankAccountType.Location = new System.Drawing.Point(6, 186);
            this.lblBankAccountType.Name = "lblBankAccountType";
            this.lblBankAccountType.Size = new System.Drawing.Size(102, 18);
            this.lblBankAccountType.TabIndex = 83;
            this.lblBankAccountType.Text = "Bank Account Type";
            this.lblBankAccountType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBankBranch
            // 
            this.lblBankBranch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblBankBranch.Location = new System.Drawing.Point(6, 220);
            this.lblBankBranch.Name = "lblBankBranch";
            this.lblBankBranch.Size = new System.Drawing.Size(70, 24);
            this.lblBankBranch.TabIndex = 81;
            this.lblBankBranch.Text = "Bank Branch";
            this.lblBankBranch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mtb_CardNo
            // 
            this.mtb_CardNo.Location = new System.Drawing.Point(126, 114);
            this.mtb_CardNo.Mask = "XXXX-XXXX-XXXX-0000";
            this.mtb_CardNo.Name = "mtb_CardNo";
            this.mtb_CardNo.Size = new System.Drawing.Size(151, 20);
            this.mtb_CardNo.TabIndex = 80;
            this.mtb_CardNo.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            // 
            // lDisbursementType
            // 
            this.lDisbursementType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lDisbursementType.Location = new System.Drawing.Point(6, 42);
            this.lDisbursementType.Name = "lDisbursementType";
            this.lDisbursementType.Size = new System.Drawing.Size(114, 20);
            this.lDisbursementType.TabIndex = 74;
            this.lDisbursementType.Text = "Disbursement Type";
            this.lDisbursementType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpCardType
            // 
            this.drpCardType.BackColor = System.Drawing.SystemColors.Window;
            this.drpCardType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCardType.Items.AddRange(new object[] {
            ""});
            this.drpCardType.Location = new System.Drawing.Point(126, 78);
            this.drpCardType.Name = "drpCardType";
            this.drpCardType.Size = new System.Drawing.Size(104, 21);
            this.drpCardType.TabIndex = 77;
            // 
            // lCardType
            // 
            this.lCardType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lCardType.Location = new System.Drawing.Point(6, 81);
            this.lCardType.Name = "lCardType";
            this.lCardType.Size = new System.Drawing.Size(58, 15);
            this.lCardType.TabIndex = 73;
            this.lCardType.Text = "Card Type";
            this.lCardType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpBank
            // 
            this.drpBank.BackColor = System.Drawing.SystemColors.Window;
            this.drpBank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBank.Location = new System.Drawing.Point(126, 149);
            this.drpBank.Name = "drpBank";
            this.drpBank.Size = new System.Drawing.Size(184, 21);
            this.drpBank.TabIndex = 78;
            // 
            // drpDisbursementType
            // 
            this.drpDisbursementType.BackColor = System.Drawing.SystemColors.Window;
            this.drpDisbursementType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpDisbursementType.Location = new System.Drawing.Point(126, 41);
            this.drpDisbursementType.Name = "drpDisbursementType";
            this.drpDisbursementType.Size = new System.Drawing.Size(128, 21);
            this.drpDisbursementType.TabIndex = 76;
            this.drpDisbursementType.SelectedIndexChanged += new System.EventHandler(this.drpPayMethod_SelectedIndexChanged);
            // 
            // lBankAcctNo
            // 
            this.lBankAcctNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lBankAcctNo.Location = new System.Drawing.Point(6, 264);
            this.lBankAcctNo.Name = "lBankAcctNo";
            this.lBankAcctNo.Size = new System.Drawing.Size(94, 16);
            this.lBankAcctNo.TabIndex = 72;
            this.lBankAcctNo.Text = "Bank Account No";
            this.lBankAcctNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lBank
            // 
            this.lBank.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lBank.Location = new System.Drawing.Point(6, 152);
            this.lBank.Name = "lBank";
            this.lBank.Size = new System.Drawing.Size(38, 18);
            this.lBank.TabIndex = 71;
            this.lBank.Text = "Bank";
            this.lBank.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtBankAcctNo
            // 
            this.txtBankAcctNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtBankAcctNo.Location = new System.Drawing.Point(126, 260);
            this.txtBankAcctNo.MaxLength = 20;
            this.txtBankAcctNo.Name = "txtBankAcctNo";
            this.txtBankAcctNo.Size = new System.Drawing.Size(184, 20);
            this.txtBankAcctNo.TabIndex = 79;
            // 
            // lCardNo
            // 
            this.lCardNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lCardNo.Location = new System.Drawing.Point(6, 118);
            this.lCardNo.Name = "lCardNo";
            this.lCardNo.Size = new System.Drawing.Size(96, 16);
            this.lCardNo.TabIndex = 75;
            this.lCardNo.Text = "Cheque / Card No";
            this.lCardNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 459);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(906, 22);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(807, 198);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 10;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnPrintReceipt
            // 
            this.btnPrintReceipt.Location = new System.Drawing.Point(807, 99);
            this.btnPrintReceipt.Name = "btnPrintReceipt";
            this.btnPrintReceipt.Size = new System.Drawing.Size(75, 35);
            this.btnPrintReceipt.TabIndex = 9;
            this.btnPrintReceipt.Text = "Print Receipt";
            this.btnPrintReceipt.UseVisualStyleBackColor = true;
            this.btnPrintReceipt.Click += new System.EventHandler(this.btnPrintReceipt_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblCashierPaymentReminder);
            this.groupBox1.Controls.Add(this.txtAcctNo);
            this.groupBox1.Controls.Add(this.txtCustId);
            this.groupBox1.Controls.Add(this.lblDisburse);
            this.groupBox1.Controls.Add(this.lblCustId);
            this.groupBox1.Controls.Add(this.txtDisburseAmt);
            this.groupBox1.Controls.Add(this.txtCustomerName);
            this.groupBox1.Controls.Add(this.lblAccountNo);
            this.groupBox1.Controls.Add(this.lblCustomerName);
            this.groupBox1.Location = new System.Drawing.Point(30, 17);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(344, 439);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Disburse";
            // 
            // lblCashierPaymentReminder
            // 
            this.lblCashierPaymentReminder.AutoSize = true;
            this.lblCashierPaymentReminder.Location = new System.Drawing.Point(8, 227);
            this.lblCashierPaymentReminder.Name = "lblCashierPaymentReminder";
            this.lblCashierPaymentReminder.Size = new System.Drawing.Size(0, 13);
            this.lblCashierPaymentReminder.TabIndex = 9;
            this.lblCashierPaymentReminder.Visible = false;
            // 
            // txtAcctNo
            // 
            this.txtAcctNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtAcctNo.Location = new System.Drawing.Point(122, 127);
            this.txtAcctNo.Name = "txtAcctNo";
            this.txtAcctNo.PreventPaste = false;
            this.txtAcctNo.ReadOnly = true;
            this.txtAcctNo.Size = new System.Drawing.Size(100, 20);
            this.txtAcctNo.TabIndex = 8;
            this.txtAcctNo.Text = "000-0000-0000-0";
            // 
            // txtCustId
            // 
            this.txtCustId.BackColor = System.Drawing.SystemColors.Window;
            this.txtCustId.Location = new System.Drawing.Point(122, 37);
            this.txtCustId.Name = "txtCustId";
            this.txtCustId.ReadOnly = true;
            this.txtCustId.Size = new System.Drawing.Size(122, 20);
            this.txtCustId.TabIndex = 0;
            // 
            // lblDisburse
            // 
            this.lblDisburse.AutoSize = true;
            this.lblDisburse.Location = new System.Drawing.Point(8, 182);
            this.lblDisburse.Name = "lblDisburse";
            this.lblDisburse.Size = new System.Drawing.Size(105, 13);
            this.lblDisburse.TabIndex = 7;
            this.lblDisburse.Text = "Amount to Customer:";
            // 
            // lblCustId
            // 
            this.lblCustId.AutoSize = true;
            this.lblCustId.Location = new System.Drawing.Point(8, 37);
            this.lblCustId.Name = "lblCustId";
            this.lblCustId.Size = new System.Drawing.Size(68, 13);
            this.lblCustId.TabIndex = 1;
            this.lblCustId.Text = "Customer ID:";
            // 
            // txtDisburseAmt
            // 
            this.txtDisburseAmt.BackColor = System.Drawing.SystemColors.Window;
            this.txtDisburseAmt.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDisburseAmt.Location = new System.Drawing.Point(122, 182);
            this.txtDisburseAmt.Name = "txtDisburseAmt";
            this.txtDisburseAmt.ReadOnly = true;
            this.txtDisburseAmt.Size = new System.Drawing.Size(100, 26);
            this.txtDisburseAmt.TabIndex = 6;
            this.txtDisburseAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.SystemColors.Window;
            this.txtCustomerName.Location = new System.Drawing.Point(122, 82);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(204, 20);
            this.txtCustomerName.TabIndex = 2;
            // 
            // lblAccountNo
            // 
            this.lblAccountNo.AutoSize = true;
            this.lblAccountNo.Location = new System.Drawing.Point(8, 130);
            this.lblAccountNo.Name = "lblAccountNo";
            this.lblAccountNo.Size = new System.Drawing.Size(90, 13);
            this.lblAccountNo.TabIndex = 5;
            this.lblAccountNo.Text = "Account Number:";
            // 
            // lblCustomerName
            // 
            this.lblCustomerName.AutoSize = true;
            this.lblCustomerName.Location = new System.Drawing.Point(8, 82);
            this.lblCustomerName.Name = "lblCustomerName";
            this.lblCustomerName.Size = new System.Drawing.Size(85, 13);
            this.lblCustomerName.TabIndex = 3;
            this.lblCustomerName.Text = "Customer Name:";
            // 
            // txtBankAccountName
            // 
            this.txtBankAccountName.BackColor = System.Drawing.SystemColors.Window;
            this.txtBankAccountName.Enabled = false;
            this.txtBankAccountName.Location = new System.Drawing.Point(126, 328);
            this.txtBankAccountName.MaxLength = 30;
            this.txtBankAccountName.Name = "txtBankAccountName";
            this.txtBankAccountName.Size = new System.Drawing.Size(198, 20);
            this.txtBankAccountName.TabIndex = 102;
            // 
            // lblBankAccountName
            // 
            this.lblBankAccountName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblBankAccountName.Location = new System.Drawing.Point(6, 328);
            this.lblBankAccountName.Name = "lblBankAccountName";
            this.lblBankAccountName.Size = new System.Drawing.Size(109, 18);
            this.lblBankAccountName.TabIndex = 101;
            this.lblBankAccountName.Text = "Name on Account";
            this.lblBankAccountName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtBankReferenceNo
            // 
            this.txtBankReferenceNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtBankReferenceNo.Enabled = false;
            this.txtBankReferenceNo.Location = new System.Drawing.Point(126, 295);
            this.txtBankReferenceNo.MaxLength = 10;
            this.txtBankReferenceNo.Name = "txtBankReferenceNo";
            this.txtBankReferenceNo.Size = new System.Drawing.Size(82, 20);
            this.txtBankReferenceNo.TabIndex = 100;
            // 
            // lblBankReferenceNo
            // 
            this.lblBankReferenceNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblBankReferenceNo.Location = new System.Drawing.Point(6, 295);
            this.lblBankReferenceNo.Name = "lblBankReferenceNo";
            this.lblBankReferenceNo.Size = new System.Drawing.Size(85, 18);
            this.lblBankReferenceNo.TabIndex = 99;
            this.lblBankReferenceNo.Text = "Bank Reference";
            this.lblBankReferenceNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CashLoanDisbursement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(906, 481);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnPrintReceipt);
            this.Controls.Add(this.groupBox1);
            this.Name = "CashLoanDisbursement";
            this.Text = "Cash Loan Disbursement";
            this.Load += new System.EventHandler(this.CashLoanDisbursement_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCustId;
        private System.Windows.Forms.Label lblCustId;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Label lblCustomerName;
        private System.Windows.Forms.Label lblAccountNo;
        private System.Windows.Forms.TextBox txtDisburseAmt;
        private System.Windows.Forms.Label lblDisburse;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnPrintReceipt;
        private System.Windows.Forms.Button btnExit;
        public AccountTextBox txtAcctNo;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.MaskedTextBox mtb_CardNo;
        private System.Windows.Forms.ComboBox drpCardType;
        private System.Windows.Forms.Label lCardType;
        private System.Windows.Forms.ComboBox drpBank;
        private System.Windows.Forms.ComboBox drpDisbursementType;
        private System.Windows.Forms.Label lBankAcctNo;
        private System.Windows.Forms.Label lBank;
        private System.Windows.Forms.TextBox txtBankAcctNo;
        private System.Windows.Forms.Label lCardNo;
        private System.Windows.Forms.Label lDisbursementType;
        private System.Windows.Forms.Label lblBankBranch;
        private System.Windows.Forms.Label lblBankAccountType;
        private System.Windows.Forms.ComboBox drpBankAccountType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.TextBox txtCardNo;
        private System.Windows.Forms.TextBox txtBankBranch;
        private System.Windows.Forms.Label lblCashierPaymentReminder;
        private System.Windows.Forms.TextBox txtBankAccountName;
        private System.Windows.Forms.Label lblBankAccountName;
        private System.Windows.Forms.TextBox txtBankReferenceNo;
        private System.Windows.Forms.Label lblBankReferenceNo;
    }
}