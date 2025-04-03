using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.Common.Constants.CashLoans;

namespace STL.PL.CashLoan
{
    //#CR-7- Extend Referrals to Cash Loans
    public partial class CashLoanReferralNotQualify : CommonForm
    {
        private Button btnCancel;
        private Button btnRefer;
        private Label lblRefer;
        private Label label1;
        public bool Return = false;
        public CashLoanReferralNotQualify()
        {
            InitializeComponent();
            Return = false;
        }

        private void btnRefer_Click(object sender, EventArgs e)
        {
            Return = true;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Return = false;
            Close();
        }

        private void InitializeComponent()
        {
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRefer = new System.Windows.Forms.Button();
            this.lblRefer = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(178, 92);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 26);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "No";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRefer
            // 
            this.btnRefer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefer.Location = new System.Drawing.Point(88, 92);
            this.btnRefer.Name = "btnRefer";
            this.btnRefer.Size = new System.Drawing.Size(75, 25);
            this.btnRefer.TabIndex = 5;
            this.btnRefer.Text = "Yes";
            this.btnRefer.UseVisualStyleBackColor = true;
            this.btnRefer.Click += new System.EventHandler(this.btnRefer_Click);
            // 
            // lblRefer
            // 
            this.lblRefer.AutoSize = true;
            this.lblRefer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRefer.ForeColor = System.Drawing.Color.Red;
            this.lblRefer.Location = new System.Drawing.Point(51, 21);
            this.lblRefer.Name = "lblRefer";
            this.lblRefer.Size = new System.Drawing.Size(280, 17);
            this.lblRefer.TabIndex = 4;
            this.lblRefer.Text = "Customer Does Not Qualify for Cash Loan. ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(91, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(173, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = " Do you wish to refer [Y/N]";
            // 
            // CashLoanReferralNotQualify
            // 
            this.ClientSize = new System.Drawing.Size(376, 148);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRefer);
            this.Controls.Add(this.lblRefer);
            this.Name = "CashLoanReferralNotQualify";
            this.ResumeLayout(false);
            this.PerformLayout();

        }


    }
}
