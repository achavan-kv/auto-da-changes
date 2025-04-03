namespace STL.PL
{
    partial class LoyaltyRedeemVoucher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoyaltyRedeemVoucher));
            this.title_lbl = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Loyalty_Voucher_dgv = new System.Windows.Forms.DataGridView();
            this.close_btn = new System.Windows.Forms.Button();
            this.Select_btn = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Custno_lbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Loyalty_Voucher_dgv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // title_lbl
            // 
            this.title_lbl.BackColor = System.Drawing.SystemColors.Control;
            this.title_lbl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.title_lbl.Enabled = false;
            this.title_lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title_lbl.Location = new System.Drawing.Point(-28, 22);
            this.title_lbl.Multiline = true;
            this.title_lbl.Name = "title_lbl";
            this.title_lbl.ReadOnly = true;
            this.title_lbl.Size = new System.Drawing.Size(404, 34);
            this.title_lbl.TabIndex = 0;
            this.title_lbl.Text = "Vouchers  Available for Redemption";
            this.title_lbl.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Customer:";
            // 
            // Loyalty_Voucher_dgv
            // 
            this.Loyalty_Voucher_dgv.AllowUserToAddRows = false;
            this.Loyalty_Voucher_dgv.AllowUserToDeleteRows = false;
            this.Loyalty_Voucher_dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.Loyalty_Voucher_dgv.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.Loyalty_Voucher_dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Loyalty_Voucher_dgv.Location = new System.Drawing.Point(5, 86);
            this.Loyalty_Voucher_dgv.Name = "Loyalty_Voucher_dgv";
            this.Loyalty_Voucher_dgv.ReadOnly = true;
            this.Loyalty_Voucher_dgv.Size = new System.Drawing.Size(473, 187);
            this.Loyalty_Voucher_dgv.TabIndex = 3;
            this.Loyalty_Voucher_dgv.DoubleClick += new System.EventHandler(this.Loyalty_Voucher_dgv_DoubleClick);
            // 
            // close_btn
            // 
            this.close_btn.Location = new System.Drawing.Point(396, 279);
            this.close_btn.Name = "close_btn";
            this.close_btn.Size = new System.Drawing.Size(75, 23);
            this.close_btn.TabIndex = 4;
            this.close_btn.Text = "Cancel";
            this.close_btn.UseVisualStyleBackColor = true;
            this.close_btn.Click += new System.EventHandler(this.close_btn_Click);
            // 
            // Select_btn
            // 
            this.Select_btn.Location = new System.Drawing.Point(12, 279);
            this.Select_btn.Name = "Select_btn";
            this.Select_btn.Size = new System.Drawing.Size(75, 23);
            this.Select_btn.TabIndex = 5;
            this.Select_btn.Text = "Select";
            this.Select_btn.UseVisualStyleBackColor = true;
            this.Select_btn.Click += new System.EventHandler(this.Select_btn_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(349, 22);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(129, 34);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // Custno_lbl
            // 
            this.Custno_lbl.AutoSize = true;
            this.Custno_lbl.Location = new System.Drawing.Point(72, 70);
            this.Custno_lbl.Name = "Custno_lbl";
            this.Custno_lbl.Size = new System.Drawing.Size(0, 13);
            this.Custno_lbl.TabIndex = 7;
            // 
            // LoyaltyRedeemVoucher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 314);
            this.Controls.Add(this.Loyalty_Voucher_dgv);
            this.Controls.Add(this.Custno_lbl);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Select_btn);
            this.Controls.Add(this.close_btn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.title_lbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "LoyaltyRedeemVoucher";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Home Club Voucher Redemption";
            ((System.ComponentModel.ISupportInitialize)(this.Loyalty_Voucher_dgv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox title_lbl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView Loyalty_Voucher_dgv;
        private System.Windows.Forms.Button close_btn;
        private System.Windows.Forms.Button Select_btn;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label Custno_lbl;
    }
}