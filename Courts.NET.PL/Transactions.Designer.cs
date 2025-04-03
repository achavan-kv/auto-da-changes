
namespace STL.PL
{
    partial class Transactions
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
            this.tcSummary = new Crownwood.Magic.Controls.TabControl();
            this.tpPaymentDetails = new Crownwood.Magic.Controls.TabPage();
            this.dgDetails = new System.Windows.Forms.DataGrid();
            this.dgSummary = new System.Windows.Forms.DataGrid();
            this.tpTransactionDetails = new Crownwood.Magic.Controls.TabPage();
            this.label63 = new System.Windows.Forms.Label();
            this.txtTotalBailFees = new System.Windows.Forms.TextBox();
            this.label45 = new System.Windows.Forms.Label();
            this.txtTotalAdmin = new System.Windows.Forms.TextBox();
            this.txtTotalInterest = new System.Windows.Forms.TextBox();
            this.label54 = new System.Windows.Forms.Label();
            this.dgTransactionsNew = new System.Windows.Forms.DataGrid();
            this.tcSummary.SuspendLayout();
            this.tpPaymentDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgSummary)).BeginInit();
            this.tpTransactionDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgTransactionsNew)).BeginInit();
            this.SuspendLayout();
            // 
            // tcSummary
            // 
            this.tcSummary.IDEPixelArea = true;
            this.tcSummary.Location = new System.Drawing.Point(0, 0);
            this.tcSummary.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tcSummary.Name = "tcSummary";
            this.tcSummary.PositionTop = true;
            this.tcSummary.SelectedIndex = 0;
            this.tcSummary.SelectedTab = this.tpTransactionDetails;
            this.tcSummary.Size = new System.Drawing.Size(1131, 406);
            this.tcSummary.TabIndex = 1;
            this.tcSummary.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tpTransactionDetails,
            this.tpPaymentDetails});
            // 
            // tpPaymentDetails
            // 
            this.tpPaymentDetails.Controls.Add(this.dgDetails);
            this.tpPaymentDetails.Controls.Add(this.dgSummary);
            this.tpPaymentDetails.Location = new System.Drawing.Point(0, 33);
            this.tpPaymentDetails.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tpPaymentDetails.Name = "tpPaymentDetails";
            this.tpPaymentDetails.Selected = false;
            this.tpPaymentDetails.Size = new System.Drawing.Size(1131, 373);
            this.tpPaymentDetails.TabIndex = 4;
            this.tpPaymentDetails.Title = "PaymentDetails";
            // 
            // dgDetails
            // 
            this.dgDetails.CaptionFont = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
            this.dgDetails.DataMember = "";
            this.dgDetails.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgDetails.Location = new System.Drawing.Point(18, 127);
            this.dgDetails.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgDetails.Name = "dgDetails";
            this.dgDetails.ReadOnly = true;
            this.dgDetails.Size = new System.Drawing.Size(963, 217);
            this.dgDetails.TabIndex = 3;
            // 
            // dgSummary
            // 
            this.dgSummary.CaptionFont = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
            this.dgSummary.DataMember = "";
            this.dgSummary.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgSummary.Location = new System.Drawing.Point(18, 20);
            this.dgSummary.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgSummary.Name = "dgSummary";
            this.dgSummary.ReadOnly = true;
            this.dgSummary.Size = new System.Drawing.Size(963, 97);
            this.dgSummary.TabIndex = 2;
            // 
            // tpTransactionDetails
            // 
            this.tpTransactionDetails.Controls.Add(this.label63);
            this.tpTransactionDetails.Controls.Add(this.txtTotalBailFees);
            this.tpTransactionDetails.Controls.Add(this.label45);
            this.tpTransactionDetails.Controls.Add(this.txtTotalAdmin);
            this.tpTransactionDetails.Controls.Add(this.txtTotalInterest);
            this.tpTransactionDetails.Controls.Add(this.label54);
            this.tpTransactionDetails.Controls.Add(this.dgTransactionsNew);
            this.tpTransactionDetails.Location = new System.Drawing.Point(0, 33);
            this.tpTransactionDetails.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tpTransactionDetails.Name = "tpTransactionDetails";
            this.tpTransactionDetails.Size = new System.Drawing.Size(1131, 373);
            this.tpTransactionDetails.TabIndex = 3;
            this.tpTransactionDetails.Title = "Transaction Details";
            // 
            // label63
            // 
            this.label63.Location = new System.Drawing.Point(224, 296);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(92, 25);
            this.label63.TabIndex = 18;
            this.label63.Text = "Total Fees";
            this.label63.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTotalBailFees
            // 
            this.txtTotalBailFees.Location = new System.Drawing.Point(228, 321);
            this.txtTotalBailFees.Name = "txtTotalBailFees";
            this.txtTotalBailFees.Size = new System.Drawing.Size(88, 31);
            this.txtTotalBailFees.TabIndex = 17;
            // 
            // label45
            // 
            this.label45.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label45.Location = new System.Drawing.Point(13, 295);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(107, 22);
            this.label45.TabIndex = 15;
            this.label45.Text = "Total Admin";
            this.label45.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTotalAdmin
            // 
            this.txtTotalAdmin.Location = new System.Drawing.Point(19, 320);
            this.txtTotalAdmin.Name = "txtTotalAdmin";
            this.txtTotalAdmin.Size = new System.Drawing.Size(88, 31);
            this.txtTotalAdmin.TabIndex = 13;
            // 
            // txtTotalInterest
            // 
            this.txtTotalInterest.Location = new System.Drawing.Point(120, 321);
            this.txtTotalInterest.Name = "txtTotalInterest";
            this.txtTotalInterest.Size = new System.Drawing.Size(88, 31);
            this.txtTotalInterest.TabIndex = 14;
            // 
            // label54
            // 
            this.label54.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label54.Location = new System.Drawing.Point(115, 295);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(120, 23);
            this.label54.TabIndex = 16;
            this.label54.Text = "Total Interest";
            this.label54.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgTransactionsNew
            // 
            this.dgTransactionsNew.CaptionFont = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
            this.dgTransactionsNew.DataMember = "";
            this.dgTransactionsNew.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgTransactionsNew.Location = new System.Drawing.Point(18, 5);
            this.dgTransactionsNew.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgTransactionsNew.Name = "dgTransactionsNew";
            this.dgTransactionsNew.ReadOnly = true;
            this.dgTransactionsNew.Size = new System.Drawing.Size(963, 292);
            this.dgTransactionsNew.TabIndex = 1;
            // 
            // Transactions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1131, 406);
            this.Controls.Add(this.tcSummary);
            this.Name = "Transactions";
            this.Text = "Transactions";
            this.tcSummary.ResumeLayout(false);
            this.tpPaymentDetails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgSummary)).EndInit();
            this.tpTransactionDetails.ResumeLayout(false);
            this.tpTransactionDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgTransactionsNew)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Crownwood.Magic.Controls.TabControl tcSummary;
        private Crownwood.Magic.Controls.TabPage tpTransactionDetails;
        public System.Windows.Forms.DataGrid dgTransactionsNew;
        private Crownwood.Magic.Controls.TabPage tpPaymentDetails;
        public System.Windows.Forms.DataGrid dgSummary;
        private System.Windows.Forms.Label label63;
        public System.Windows.Forms.TextBox txtTotalBailFees;
        private System.Windows.Forms.Label label45;
        public System.Windows.Forms.TextBox txtTotalAdmin;
        public System.Windows.Forms.TextBox txtTotalInterest;
        private System.Windows.Forms.Label label54;
        public System.Windows.Forms.DataGrid dgDetails;
    }
}