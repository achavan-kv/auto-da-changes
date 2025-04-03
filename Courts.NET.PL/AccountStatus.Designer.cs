namespace STL.PL
{
    partial class AccountStatus
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
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.chkCancelled = new System.Windows.Forms.CheckBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblTo = new System.Windows.Forms.Label();
            this.lblFrom = new System.Windows.Forms.Label();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.grpData = new System.Windows.Forms.GroupBox();
            this.dgAcctDelStatus = new System.Windows.Forms.DataGridView();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.grpOptions.SuspendLayout();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAcctDelStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.chkCancelled);
            this.grpOptions.Controls.Add(this.btnExit);
            this.grpOptions.Controls.Add(this.btnSearch);
            this.grpOptions.Controls.Add(this.lblTo);
            this.grpOptions.Controls.Add(this.lblFrom);
            this.grpOptions.Controls.Add(this.dtTo);
            this.grpOptions.Controls.Add(this.dtFrom);
            this.grpOptions.Location = new System.Drawing.Point(12, 12);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(768, 101);
            this.grpOptions.TabIndex = 0;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Search Criteria";
            // 
            // chkCancelled
            // 
            this.chkCancelled.AutoSize = true;
            this.chkCancelled.Location = new System.Drawing.Point(356, 45);
            this.chkCancelled.Name = "chkCancelled";
            this.chkCancelled.Size = new System.Drawing.Size(114, 17);
            this.chkCancelled.TabIndex = 6;
            this.chkCancelled.Text = "Exclude Cancelled";
            this.chkCancelled.UseVisualStyleBackColor = true;
            this.chkCancelled.Click += new System.EventHandler(this.chkCancelled_Click);
            this.chkCancelled.MouseHover += new System.EventHandler(this.chkCancelled_MouseHover);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(670, 41);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(563, 41);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(232, 16);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(49, 13);
            this.lblTo.TabIndex = 3;
            this.lblTo.Text = "Date To:";
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(69, 16);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(59, 13);
            this.lblFrom.TabIndex = 2;
            this.lblFrom.Text = "Date From:";
            // 
            // dtTo
            // 
            this.dtTo.Location = new System.Drawing.Point(204, 42);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(112, 20);
            this.dtTo.TabIndex = 1;
            this.dtTo.MouseHover += new System.EventHandler(this.dtTo_MouseHover);
            // 
            // dtFrom
            // 
            this.dtFrom.Location = new System.Drawing.Point(46, 42);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(112, 20);
            this.dtFrom.TabIndex = 0;
            this.dtFrom.MouseHover += new System.EventHandler(this.dtFrom_MouseHover);
            // 
            // grpData
            // 
            this.grpData.Controls.Add(this.dgAcctDelStatus);
            this.grpData.Location = new System.Drawing.Point(12, 119);
            this.grpData.Name = "grpData";
            this.grpData.Size = new System.Drawing.Size(768, 346);
            this.grpData.TabIndex = 1;
            this.grpData.TabStop = false;
            this.grpData.Text = "Search Results";
            // 
            // dgAcctDelStatus
            // 
            this.dgAcctDelStatus.AllowUserToAddRows = false;
            this.dgAcctDelStatus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgAcctDelStatus.Location = new System.Drawing.Point(6, 19);
            this.dgAcctDelStatus.Name = "dgAcctDelStatus";
            this.dgAcctDelStatus.ReadOnly = true;
            this.dgAcctDelStatus.Size = new System.Drawing.Size(756, 321);
            this.dgAcctDelStatus.TabIndex = 0;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // AccountStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.grpData);
            this.Controls.Add(this.grpOptions);
            this.Name = "AccountStatus";
            this.Text = "AccountStatus";
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.grpData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAcctDelStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.CheckBox chkCancelled;
        private System.Windows.Forms.GroupBox grpData;
        private System.Windows.Forms.DataGridView dgAcctDelStatus;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}