namespace STL.PL.SERVICE
{
    partial class BERReplacements
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
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.btnReload = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lbBranch = new System.Windows.Forms.Label();
            this.drpBranch = new System.Windows.Forms.ComboBox();
            this.dgItems = new System.Windows.Forms.DataGridView();
            this.AccountNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SRNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Dateauth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IUPC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemDescr1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemDescr2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Custtel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AccountBranch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgItems)).BeginInit();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.statusBar.Location = new System.Drawing.Point(0, 458);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(790, 20);
            this.statusBar.TabIndex = 9;
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(578, 26);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(75, 23);
            this.btnReload.TabIndex = 8;
            this.btnReload.Text = "ReLoad";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(681, 26);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lbBranch
            // 
            this.lbBranch.AutoSize = true;
            this.lbBranch.Location = new System.Drawing.Point(27, 31);
            this.lbBranch.Name = "lbBranch";
            this.lbBranch.Size = new System.Drawing.Size(41, 13);
            this.lbBranch.TabIndex = 6;
            this.lbBranch.Text = "Branch";
            // 
            // drpBranch
            // 
            this.drpBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranch.FormattingEnabled = true;
            this.drpBranch.Location = new System.Drawing.Point(74, 28);
            this.drpBranch.Name = "drpBranch";
            this.drpBranch.Size = new System.Drawing.Size(55, 21);
            this.drpBranch.TabIndex = 5;
            this.drpBranch.SelectedIndexChanged += new System.EventHandler(this.drpBranch_SelectedIndexChanged);
            // 
            // dgItems
            // 
            this.dgItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AccountNumber,
            this.SRNo,
            this.Dateauth,
            this.IUPC,
            this.ItemDescr1,
            this.ItemDescr2,
            this.CustomerName,
            this.Custtel,
            this.CustId,
            this.AccountBranch});
            this.dgItems.Location = new System.Drawing.Point(30, 64);
            this.dgItems.Name = "dgItems";
            this.dgItems.ReadOnly = true;
            this.dgItems.Size = new System.Drawing.Size(726, 382);
            this.dgItems.TabIndex = 0;
            this.dgItems.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgItems_MouseUp);
            // 
            // AccountNumber
            // 
            this.AccountNumber.DataPropertyName = "AcctNo";
            this.AccountNumber.HeaderText = "Account Number";
            this.AccountNumber.Name = "AccountNumber";
            this.AccountNumber.ReadOnly = true;
            // 
            // SRNo
            // 
            this.SRNo.DataPropertyName = "ServiceRequestNo";
            this.SRNo.HeaderText = "Request Number";
            this.SRNo.Name = "SRNo";
            this.SRNo.ReadOnly = true;
            this.SRNo.Width = 70;
            // 
            // Dateauth
            // 
            this.Dateauth.DataPropertyName = "DateAuth";
            this.Dateauth.HeaderText = "Date Authorised";
            this.Dateauth.Name = "Dateauth";
            this.Dateauth.ReadOnly = true;
            this.Dateauth.Width = 70;
            // 
            // IUPC
            // 
            this.IUPC.DataPropertyName = "IUPC";
            this.IUPC.HeaderText = "IUPC";
            this.IUPC.Name = "IUPC";
            this.IUPC.ReadOnly = true;
            this.IUPC.Width = 70;
            // 
            // ItemDescr1
            // 
            this.ItemDescr1.DataPropertyName = "Itemdescr1";
            this.ItemDescr1.HeaderText = "Item Description 1";
            this.ItemDescr1.Name = "ItemDescr1";
            this.ItemDescr1.ReadOnly = true;
            this.ItemDescr1.Width = 120;
            // 
            // ItemDescr2
            // 
            this.ItemDescr2.DataPropertyName = "Itemdescr2";
            this.ItemDescr2.HeaderText = "Item Description 2";
            this.ItemDescr2.Name = "ItemDescr2";
            this.ItemDescr2.ReadOnly = true;
            this.ItemDescr2.Width = 250;
            // 
            // CustomerName
            // 
            this.CustomerName.DataPropertyName = "CustomerName";
            this.CustomerName.HeaderText = "Customer Name";
            this.CustomerName.Name = "CustomerName";
            this.CustomerName.ReadOnly = true;
            this.CustomerName.Width = 150;
            // 
            // Custtel
            // 
            this.Custtel.DataPropertyName = "CusTel";
            this.Custtel.HeaderText = "Contact Numbers";
            this.Custtel.Name = "Custtel";
            this.Custtel.ReadOnly = true;
            this.Custtel.Width = 250;
            // 
            // CustId
            // 
            this.CustId.DataPropertyName = "CustId";
            this.CustId.HeaderText = "Customer ID";
            this.CustId.Name = "CustId";
            this.CustId.ReadOnly = true;
            // 
            // AccountBranch
            // 
            this.AccountBranch.DataPropertyName = "Branch";
            this.AccountBranch.HeaderText = "Branch";
            this.AccountBranch.Name = "AccountBranch";
            this.AccountBranch.ReadOnly = true;
            this.AccountBranch.Visible = false;
            // 
            // BERReplacements
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 478);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.lbBranch);
            this.Controls.Add(this.drpBranch);
            this.Controls.Add(this.dgItems);
            this.Name = "BERReplacements";
            this.Text = "Service BER Replacements";
            ((System.ComponentModel.ISupportInitialize)(this.dgItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgItems;
        private System.Windows.Forms.Label lbBranch;
        private System.Windows.Forms.ComboBox drpBranch;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnReload;
        public System.Windows.Forms.StatusBar statusBar;
        private System.Windows.Forms.DataGridViewTextBoxColumn AccountNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn SRNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Dateauth;
        private System.Windows.Forms.DataGridViewTextBoxColumn IUPC;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemDescr1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemDescr2;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Custtel;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustId;
        private System.Windows.Forms.DataGridViewTextBoxColumn AccountBranch;
    }
}