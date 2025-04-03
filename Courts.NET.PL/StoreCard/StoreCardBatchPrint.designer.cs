namespace STL.PL.StoreCard
{
    partial class StoreCardBatchPrint
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
            this.dgStatementRuns = new System.Windows.Forms.DataGridView();
            this.RunNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RunDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DatePrinted = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BatchNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgStatementRuns)).BeginInit();
            this.SuspendLayout();
            // 
            // dgStatementRuns
            // 
            this.dgStatementRuns.AllowUserToAddRows = false;
            this.dgStatementRuns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgStatementRuns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.RunNo,
            this.RunDate,
            this.DatePrinted,
            this.BatchNo});
            this.dgStatementRuns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgStatementRuns.Location = new System.Drawing.Point(0, 0);
            this.dgStatementRuns.Name = "dgStatementRuns";
            this.dgStatementRuns.ReadOnly = true;
            this.dgStatementRuns.Size = new System.Drawing.Size(792, 477);
            this.dgStatementRuns.TabIndex = 4;
            this.dgStatementRuns.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgStatementRuns_MouseUp);
            // 
            // RunNo
            // 
            this.RunNo.DataPropertyName = "RunNo";
            this.RunNo.HeaderText = "Run No";
            this.RunNo.Name = "RunNo";
            this.RunNo.ReadOnly = true;
            // 
            // RunDate
            // 
            this.RunDate.DataPropertyName = "RunDate";
            this.RunDate.HeaderText = "Run Date";
            this.RunDate.Name = "RunDate";
            this.RunDate.ReadOnly = true;
            // 
            // DatePrinted
            // 
            this.DatePrinted.DataPropertyName = "DatePrinted";
            this.DatePrinted.HeaderText = "Date Printed";
            this.DatePrinted.Name = "DatePrinted";
            this.DatePrinted.ReadOnly = true;
            // 
            // BatchNo
            // 
            this.BatchNo.DataPropertyName = "BatchNo";
            this.BatchNo.HeaderText = "Statement No";
            this.BatchNo.Name = "BatchNo";
            this.BatchNo.ReadOnly = true;
            // 
            // StoreCardBatchPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.dgStatementRuns);
            this.Name = "StoreCardBatchPrint";
            this.Text = "Store Card Batch Print";
            ((System.ComponentModel.ISupportInitialize)(this.dgStatementRuns)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgStatementRuns;
        private System.Windows.Forms.DataGridViewTextBoxColumn RunNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn RunDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn DatePrinted;
        private System.Windows.Forms.DataGridViewTextBoxColumn BatchNo;
    }
}