namespace STL.PL.Warehouse
{
    partial class FailedDeliveriesCollections
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
            this.dgDeliveryFails = new System.Windows.Forms.DataGridView();
            this.btnReload = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.drpBranch = new System.Windows.Forms.ComboBox();
            this.lbBranch = new System.Windows.Forms.Label();
            this.drpSalesperson = new System.Windows.Forms.ComboBox();
            this.lblSalesPerson = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgDeliveryFails)).BeginInit();
            this.SuspendLayout();
            // 
            // dgDeliveryFails
            // 
            this.dgDeliveryFails.AllowUserToAddRows = false;
            this.dgDeliveryFails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgDeliveryFails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgDeliveryFails.Location = new System.Drawing.Point(23, 70);
            this.dgDeliveryFails.Name = "dgDeliveryFails";
            this.dgDeliveryFails.ReadOnly = true;
            this.dgDeliveryFails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgDeliveryFails.Size = new System.Drawing.Size(739, 371);
            this.dgDeliveryFails.TabIndex = 0;
            this.dgDeliveryFails.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgDeliveryFails_MouseUp);
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(512, 27);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(75, 23);
            this.btnReload.TabIndex = 1;
            this.btnReload.Text = "ReLoad";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(605, 27);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // drpBranch
            // 
            this.drpBranch.Enabled = false;
            this.drpBranch.FormattingEnabled = true;
            this.drpBranch.Location = new System.Drawing.Point(111, 27);
            this.drpBranch.Name = "drpBranch";
            this.drpBranch.Size = new System.Drawing.Size(55, 21);
            this.drpBranch.TabIndex = 3;
            this.drpBranch.SelectedIndexChanged += new System.EventHandler(this.drpBranch_SelectedIndexChanged);
            // 
            // lbBranch
            // 
            this.lbBranch.AutoSize = true;
            this.lbBranch.Location = new System.Drawing.Point(35, 31);
            this.lbBranch.Name = "lbBranch";
            this.lbBranch.Size = new System.Drawing.Size(70, 13);
            this.lbBranch.TabIndex = 4;
            this.lbBranch.Text = "Order Branch";
            // 
            // drpSalesperson
            // 
            this.drpSalesperson.Enabled = false;
            this.drpSalesperson.FormattingEnabled = true;
            this.drpSalesperson.Location = new System.Drawing.Point(269, 27);
            this.drpSalesperson.Name = "drpSalesperson";
            this.drpSalesperson.Size = new System.Drawing.Size(177, 21);
            this.drpSalesperson.TabIndex = 5;
            this.drpSalesperson.SelectedIndexChanged += new System.EventHandler(this.drpSalesperson_SelectedIndexChanged);
            // 
            // lblSalesPerson
            // 
            this.lblSalesPerson.AutoSize = true;
            this.lblSalesPerson.Location = new System.Drawing.Point(191, 32);
            this.lblSalesPerson.Name = "lblSalesPerson";
            this.lblSalesPerson.Size = new System.Drawing.Size(65, 13);
            this.lblSalesPerson.TabIndex = 6;
            this.lblSalesPerson.Text = "Salesperson";
            // 
            // FailedDeliveriesCollections
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 478);
            this.Controls.Add(this.lblSalesPerson);
            this.Controls.Add(this.drpSalesperson);
            this.Controls.Add(this.lbBranch);
            this.Controls.Add(this.drpBranch);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.dgDeliveryFails);
            this.Name = "FailedDeliveriesCollections";
            this.Text = "Failed Deliveries and Collections";
            ((System.ComponentModel.ISupportInitialize)(this.dgDeliveryFails)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgDeliveryFails;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.ComboBox drpBranch;
        private System.Windows.Forms.Label lbBranch;
        private System.Windows.Forms.ComboBox drpSalesperson;
        private System.Windows.Forms.Label lblSalesPerson;
    }
}