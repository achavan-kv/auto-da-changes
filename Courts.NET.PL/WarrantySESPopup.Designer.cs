namespace STL.PL
{
    partial class WarrantySESPopup
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
            this.dgvWarrantyItems = new System.Windows.Forms.DataGridView();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblWarrantySecondChance = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWarrantyItems)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvWarrantyItems
            // 
            this.dgvWarrantyItems.AllowUserToAddRows = false;
            this.dgvWarrantyItems.AllowUserToDeleteRows = false;
            this.dgvWarrantyItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWarrantyItems.Location = new System.Drawing.Point(20, 35);
            this.dgvWarrantyItems.Name = "dgvWarrantyItems";
            this.dgvWarrantyItems.ReadOnly = true;
            this.dgvWarrantyItems.Size = new System.Drawing.Size(498, 184);
            this.dgvWarrantyItems.TabIndex = 0;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(20, 225);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(443, 225);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblWarrantySecondChance
            // 
            this.lblWarrantySecondChance.AutoSize = true;
            this.lblWarrantySecondChance.Location = new System.Drawing.Point(17, 9);
            this.lblWarrantySecondChance.Name = "lblWarrantySecondChance";
            this.lblWarrantySecondChance.Size = new System.Drawing.Size(355, 13);
            this.lblWarrantySecondChance.TabIndex = 3;
            this.lblWarrantySecondChance.Text = "This customer has the following items available to purchase warranties for:";
            // 
            // WarrantySESPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 260);
            this.Controls.Add(this.lblWarrantySecondChance);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.dgvWarrantyItems);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WarrantySESPopup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WarrantySESPopup_FormClosing);
            this.Load += new System.EventHandler(this.WarrantySESPopup_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvWarrantyItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvWarrantyItems;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblWarrantySecondChance;
    }
}