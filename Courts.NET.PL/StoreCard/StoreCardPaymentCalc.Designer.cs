namespace STL.PL.StoreCard
{
    partial class StoreCardPaymentCalc
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StoreCardPaymentCalc));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBalance = new System.Windows.Forms.TextBox();
            this.txtInterest = new System.Windows.Forms.TextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnPrint = new System.Windows.Forms.Button();
            this.nud_term = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTermsResult = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtMonthlyPay = new System.Windows.Forms.TextBox();
            this.txtTotalPayments = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabFixedPay = new System.Windows.Forms.TabControl();
            this.tabFixedTerms = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTotalTerms = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMonthPayResult = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnMinPay = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.errorProvider2 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_term)).BeginInit();
            this.tabFixedPay.SuspendLayout();
            this.tabFixedTerms.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Balance";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Interest Rate";
            // 
            // txtBalance
            // 
            this.txtBalance.Location = new System.Drawing.Point(110, 18);
            this.txtBalance.Name = "txtBalance";
            this.txtBalance.Size = new System.Drawing.Size(100, 20);
            this.txtBalance.TabIndex = 1;
            this.txtBalance.TextChanged += new System.EventHandler(this.txtBalance_TextChanged);
            this.txtBalance.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPayAmount_KeyPress);
            // 
            // txtInterest
            // 
            this.txtInterest.Location = new System.Drawing.Point(110, 46);
            this.txtInterest.Name = "txtInterest";
            this.txtInterest.Size = new System.Drawing.Size(100, 20);
            this.txtInterest.TabIndex = 2;
            this.txtInterest.TextChanged += new System.EventHandler(this.txtInterest_TextChanged);
            this.txtInterest.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInterest_KeyPress);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // btnPrint
            // 
            this.errorProvider1.SetIconAlignment(this.btnPrint, System.Windows.Forms.ErrorIconAlignment.TopLeft);
            this.btnPrint.Image = ((System.Drawing.Image)(resources.GetObject("btnPrint.Image")));
            this.btnPrint.Location = new System.Drawing.Point(305, 18);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(32, 24);
            this.btnPrint.TabIndex = 16;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // nud_term
            // 
            this.nud_term.Location = new System.Drawing.Point(140, 31);
            this.nud_term.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nud_term.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nud_term.Name = "nud_term";
            this.nud_term.Size = new System.Drawing.Size(100, 20);
            this.nud_term.TabIndex = 12;
            this.nud_term.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nud_term.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nud_term.ValueChanged += new System.EventHandler(this.nud_term_ValueChanged);
            this.nud_term.Leave += new System.EventHandler(this.nud_term_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Term (Months)";
            // 
            // txtTermsResult
            // 
            this.txtTermsResult.Location = new System.Drawing.Point(156, 60);
            this.txtTermsResult.Name = "txtTermsResult";
            this.txtTermsResult.ReadOnly = true;
            this.txtTermsResult.Size = new System.Drawing.Size(100, 20);
            this.txtTermsResult.TabIndex = 20;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(36, 67);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Number of Terms";
            // 
            // txtMonthlyPay
            // 
            this.txtMonthlyPay.Location = new System.Drawing.Point(156, 24);
            this.txtMonthlyPay.Name = "txtMonthlyPay";
            this.txtMonthlyPay.Size = new System.Drawing.Size(100, 20);
            this.txtMonthlyPay.TabIndex = 6;
            this.txtMonthlyPay.TextChanged += new System.EventHandler(this.txtMonthlyPay_TextChanged);
            // 
            // txtTotalPayments
            // 
            this.txtTotalPayments.Location = new System.Drawing.Point(156, 102);
            this.txtTotalPayments.Name = "txtTotalPayments";
            this.txtTotalPayments.ReadOnly = true;
            this.txtTotalPayments.Size = new System.Drawing.Size(100, 20);
            this.txtTotalPayments.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(36, 105);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Total ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Monthly Payments";
            // 
            // tabFixedPay
            // 
            this.tabFixedPay.Controls.Add(this.tabFixedTerms);
            this.tabFixedPay.Controls.Add(this.tabPage2);
            this.tabFixedPay.Location = new System.Drawing.Point(12, 87);
            this.tabFixedPay.Name = "tabFixedPay";
            this.tabFixedPay.SelectedIndex = 0;
            this.tabFixedPay.Size = new System.Drawing.Size(303, 178);
            this.tabFixedPay.TabIndex = 13;
            this.tabFixedPay.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabFixedPay_Selected);
            // 
            // tabFixedTerms
            // 
            this.tabFixedTerms.BackColor = System.Drawing.SystemColors.Control;
            this.tabFixedTerms.Controls.Add(this.label5);
            this.tabFixedTerms.Controls.Add(this.txtTotalTerms);
            this.tabFixedTerms.Controls.Add(this.label6);
            this.tabFixedTerms.Controls.Add(this.txtMonthPayResult);
            this.tabFixedTerms.Controls.Add(this.label3);
            this.tabFixedTerms.Controls.Add(this.nud_term);
            this.tabFixedTerms.Location = new System.Drawing.Point(4, 22);
            this.tabFixedTerms.Name = "tabFixedTerms";
            this.tabFixedTerms.Padding = new System.Windows.Forms.Padding(3);
            this.tabFixedTerms.Size = new System.Drawing.Size(295, 152);
            this.tabFixedTerms.TabIndex = 0;
            this.tabFixedTerms.Text = "Fixed Terms";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(72, 111);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Total ";
            // 
            // txtTotalTerms
            // 
            this.txtTotalTerms.Location = new System.Drawing.Point(140, 108);
            this.txtTotalTerms.Name = "txtTotalTerms";
            this.txtTotalTerms.ReadOnly = true;
            this.txtTotalTerms.Size = new System.Drawing.Size(100, 20);
            this.txtTotalTerms.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(31, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Monthly Payments";
            // 
            // txtMonthPayResult
            // 
            this.txtMonthPayResult.Location = new System.Drawing.Point(140, 66);
            this.txtMonthPayResult.Name = "txtMonthPayResult";
            this.txtMonthPayResult.ReadOnly = true;
            this.txtMonthPayResult.Size = new System.Drawing.Size(100, 20);
            this.txtMonthPayResult.TabIndex = 16;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.txtTermsResult);
            this.tabPage2.Controls.Add(this.txtMonthlyPay);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.txtTotalPayments);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(295, 152);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Fixed Monthly Payments";
            // 
            // btnMinPay
            // 
            this.btnMinPay.Location = new System.Drawing.Point(222, 48);
            this.btnMinPay.Name = "btnMinPay";
            this.btnMinPay.Size = new System.Drawing.Size(115, 23);
            this.btnMinPay.TabIndex = 15;
            this.btnMinPay.Text = "Apply as Min Pay";
            this.toolTip1.SetToolTip(this.btnMinPay, "Apply as Minimum Monthly Payment ");
            this.btnMinPay.UseVisualStyleBackColor = true;
            this.btnMinPay.Click += new System.EventHandler(this.btnMinPay_Click);
            // 
            // errorProvider2
            // 
            this.errorProvider2.ContainerControl = this;
            // 
            // StoreCardPaymentCalc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 284);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnMinPay);
            this.Controls.Add(this.tabFixedPay);
            this.Controls.Add(this.txtInterest);
            this.Controls.Add(this.txtBalance);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "StoreCardPaymentCalc";
            this.Text = "Payment Calculator";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_term)).EndInit();
            this.tabFixedPay.ResumeLayout(false);
            this.tabFixedTerms.ResumeLayout(false);
            this.tabFixedTerms.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBalance;
        private System.Windows.Forms.TextBox txtInterest;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.TextBox txtTermsResult;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtMonthlyPay;
        private System.Windows.Forms.TextBox txtTotalPayments;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nud_term;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabControl tabFixedPay;
        private System.Windows.Forms.TabPage tabFixedTerms;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTotalTerms;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMonthPayResult;
        private System.Windows.Forms.Button btnMinPay;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ErrorProvider errorProvider2;
        public System.Windows.Forms.Button btnPrint;
    }
}