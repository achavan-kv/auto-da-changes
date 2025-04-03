namespace STL.PL.CashLoan
{
    partial class CashLoanReferral
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
            this.txtReferralMessage = new System.Windows.Forms.TextBox();
            this.lblRefer = new System.Windows.Forms.Label();
            this.btnRefer = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtReferralMessage
            // 
            this.txtReferralMessage.BackColor = System.Drawing.SystemColors.Window;
            this.txtReferralMessage.Location = new System.Drawing.Point(44, 47);
            this.txtReferralMessage.Multiline = true;
            this.txtReferralMessage.Name = "txtReferralMessage";
            this.txtReferralMessage.ReadOnly = true;
            this.txtReferralMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReferralMessage.Size = new System.Drawing.Size(353, 224);
            this.txtReferralMessage.TabIndex = 0;
            this.txtReferralMessage.TabStop = false;
            // 
            // lblRefer
            // 
            this.lblRefer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRefer.Location = new System.Drawing.Point(1, 292);
            this.lblRefer.Name = "lblRefer";
            this.lblRefer.Size = new System.Drawing.Size(444, 17);
            this.lblRefer.TabIndex = 1;
            this.lblRefer.Text = "Do you wish to Refer to Credit Department?";
            this.lblRefer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRefer
            // 
            this.btnRefer.Location = new System.Drawing.Point(83, 328);
            this.btnRefer.Name = "btnRefer";
            this.btnRefer.Size = new System.Drawing.Size(75, 23);
            this.btnRefer.TabIndex = 2;
            this.btnRefer.Text = "Refer";
            this.btnRefer.UseVisualStyleBackColor = true;
            this.btnRefer.Click += new System.EventHandler(this.btnRefer_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(262, 328);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // CashLoanReferral
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 367);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRefer);
            this.Controls.Add(this.lblRefer);
            this.Controls.Add(this.txtReferralMessage);
            this.Name = "CashLoanReferral";
            this.Text = "Cash Loan Referral Messages";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtReferralMessage;
        private System.Windows.Forms.Label lblRefer;
        private System.Windows.Forms.Button btnRefer;
        private System.Windows.Forms.Button btnCancel;
    }
}