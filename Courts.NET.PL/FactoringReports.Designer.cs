namespace STL.PL
{
    partial class FactoringReports
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FactoringReports));
            this.dateTimeDateto = new System.Windows.Forms.DateTimePicker();
            this.dateTimeDatefrom = new System.Windows.Forms.DateTimePicker();
            this.btnLoad = new System.Windows.Forms.Button();
            this.comboReport = new System.Windows.Forms.ComboBox();
            this.dataGridViewResults = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.btnTransTypes = new System.Windows.Forms.Button();
            this.txtTransactionTypes = new System.Windows.Forms.TextBox();
            this.labelType = new System.Windows.Forms.Label();
            this.drpTransTypeCriteria = new System.Windows.Forms.ComboBox();
            this.drpTransTypes = new System.Windows.Forms.ComboBox();
            this.txtDetailsTotal = new System.Windows.Forms.TextBox();
            this.labelTotal = new System.Windows.Forms.Label();
            this.textBoxFinco = new System.Windows.Forms.TextBox();
            this.textBoxNonFinco = new System.Windows.Forms.TextBox();
            this.labelnonFinco = new System.Windows.Forms.Label();
            this.labelFinco = new System.Windows.Forms.Label();
            this.btnExcel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewResults)).BeginInit();
            this.SuspendLayout();
            // 
            // dateTimeDateto
            // 
            this.dateTimeDateto.Location = new System.Drawing.Point(264, 61);
            this.dateTimeDateto.Name = "dateTimeDateto";
            this.dateTimeDateto.Size = new System.Drawing.Size(200, 20);
            this.dateTimeDateto.TabIndex = 0;
            // 
            // dateTimeDatefrom
            // 
            this.dateTimeDatefrom.Location = new System.Drawing.Point(33, 61);
            this.dateTimeDatefrom.Name = "dateTimeDatefrom";
            this.dateTimeDatefrom.Size = new System.Drawing.Size(200, 20);
            this.dateTimeDatefrom.TabIndex = 1;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(678, 61);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(50, 32);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboReport
            // 
            this.comboReport.FormattingEnabled = true;
            this.comboReport.Items.AddRange(new object[] {
            "Factor Balances",
            "Factor Transactions"});
            this.comboReport.Location = new System.Drawing.Point(129, 25);
            this.comboReport.Name = "comboReport";
            this.comboReport.Size = new System.Drawing.Size(121, 21);
            this.comboReport.TabIndex = 3;
            this.comboReport.SelectedIndexChanged += new System.EventHandler(this.comboReport_SelectedIndexChanged);
            // 
            // dataGridViewResults
            // 
            this.dataGridViewResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewResults.Location = new System.Drawing.Point(33, 111);
            this.dataGridViewResults.Name = "dataGridViewResults";
            this.dataGridViewResults.Size = new System.Drawing.Size(735, 315);
            this.dataGridViewResults.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Report ";
            // 
            // btnTransTypes
            // 
            this.btnTransTypes.Enabled = false;
            this.btnTransTypes.Location = new System.Drawing.Point(704, 22);
            this.btnTransTypes.Name = "btnTransTypes";
            this.btnTransTypes.Size = new System.Drawing.Size(24, 23);
            this.btnTransTypes.TabIndex = 49;
            this.btnTransTypes.Text = "...";
            this.btnTransTypes.Visible = false;
            this.btnTransTypes.Click += new System.EventHandler(this.btnTransTypes_Click);
            // 
            // txtTransactionTypes
            // 
            this.txtTransactionTypes.Enabled = false;
            this.txtTransactionTypes.Location = new System.Drawing.Point(472, 22);
            this.txtTransactionTypes.Name = "txtTransactionTypes";
            this.txtTransactionTypes.ReadOnly = true;
            this.txtTransactionTypes.Size = new System.Drawing.Size(224, 20);
            this.txtTransactionTypes.TabIndex = 48;
            this.txtTransactionTypes.Visible = false;
            // 
            // labelType
            // 
            this.labelType.Enabled = false;
            this.labelType.Location = new System.Drawing.Point(256, 22);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(136, 24);
            this.labelType.TabIndex = 47;
            this.labelType.Text = "For Transaction Types";
            this.labelType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpTransTypeCriteria
            // 
            this.drpTransTypeCriteria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpTransTypeCriteria.Enabled = false;
            this.drpTransTypeCriteria.Items.AddRange(new object[] {
            "",
            "In Set"});
            this.drpTransTypeCriteria.Location = new System.Drawing.Point(398, 22);
            this.drpTransTypeCriteria.Name = "drpTransTypeCriteria";
            this.drpTransTypeCriteria.Size = new System.Drawing.Size(64, 21);
            this.drpTransTypeCriteria.TabIndex = 50;
            this.drpTransTypeCriteria.SelectionChangeCommitted += new System.EventHandler(this.drpTransTypeCriteria_SelectionChangeCommitted);
            this.drpTransTypeCriteria.SelectedIndexChanged += new System.EventHandler(this.drpTransTypeCriteria_SelectedIndexChanged);
            // 
            // drpTransTypes
            // 
            this.drpTransTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpTransTypes.Location = new System.Drawing.Point(400, 22);
            this.drpTransTypes.Name = "drpTransTypes";
            this.drpTransTypes.Size = new System.Drawing.Size(64, 21);
            this.drpTransTypes.TabIndex = 51;
            this.drpTransTypes.Visible = false;
            // 
            // txtDetailsTotal
            // 
            this.txtDetailsTotal.Location = new System.Drawing.Point(654, 432);
            this.txtDetailsTotal.Name = "txtDetailsTotal";
            this.txtDetailsTotal.ReadOnly = true;
            this.txtDetailsTotal.Size = new System.Drawing.Size(114, 20);
            this.txtDetailsTotal.TabIndex = 53;
            // 
            // labelTotal
            // 
            this.labelTotal.Location = new System.Drawing.Point(584, 432);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(72, 16);
            this.labelTotal.TabIndex = 54;
            this.labelTotal.Text = "Total Value:";
            // 
            // textBoxFinco
            // 
            this.textBoxFinco.Location = new System.Drawing.Point(429, 432);
            this.textBoxFinco.Name = "textBoxFinco";
            this.textBoxFinco.ReadOnly = true;
            this.textBoxFinco.Size = new System.Drawing.Size(114, 20);
            this.textBoxFinco.TabIndex = 55;
            // 
            // textBoxNonFinco
            // 
            this.textBoxNonFinco.Location = new System.Drawing.Point(199, 432);
            this.textBoxNonFinco.Name = "textBoxNonFinco";
            this.textBoxNonFinco.ReadOnly = true;
            this.textBoxNonFinco.Size = new System.Drawing.Size(114, 20);
            this.textBoxNonFinco.TabIndex = 56;
            // 
            // labelnonFinco
            // 
            this.labelnonFinco.Location = new System.Drawing.Point(98, 432);
            this.labelnonFinco.Name = "labelnonFinco";
            this.labelnonFinco.Size = new System.Drawing.Size(95, 18);
            this.labelnonFinco.TabIndex = 57;
            this.labelnonFinco.Text = "Non Finco Value:";
            // 
            // labelFinco
            // 
            this.labelFinco.Location = new System.Drawing.Point(334, 432);
            this.labelFinco.Name = "labelFinco";
            this.labelFinco.Size = new System.Drawing.Size(89, 18);
            this.labelFinco.TabIndex = 58;
            this.labelFinco.Text = "Finco Value:";
            // 
            // btnExcel
            // 
            this.btnExcel.Enabled = false;
            this.btnExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnExcel.Image")));
            this.btnExcel.Location = new System.Drawing.Point(736, 61);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(32, 32);
            this.btnExcel.TabIndex = 59;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // FactoringReports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(858, 547);
            this.Controls.Add(this.btnExcel);
            this.Controls.Add(this.labelFinco);
            this.Controls.Add(this.labelnonFinco);
            this.Controls.Add(this.textBoxNonFinco);
            this.Controls.Add(this.textBoxFinco);
            this.Controls.Add(this.txtDetailsTotal);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.btnTransTypes);
            this.Controls.Add(this.txtTransactionTypes);
            this.Controls.Add(this.labelType);
            this.Controls.Add(this.drpTransTypeCriteria);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridViewResults);
            this.Controls.Add(this.comboReport);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.dateTimeDatefrom);
            this.Controls.Add(this.dateTimeDateto);
            this.Controls.Add(this.drpTransTypes);
            this.Name = "FactoringReports";
            this.Text = "Factoring Reports";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimeDateto;
        private System.Windows.Forms.DateTimePicker dateTimeDatefrom;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ComboBox comboReport;
        private System.Windows.Forms.DataGridView dataGridViewResults;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnTransTypes;
        public System.Windows.Forms.TextBox txtTransactionTypes;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.ComboBox drpTransTypeCriteria;
        private System.Windows.Forms.ComboBox drpTransTypes;
        private System.Windows.Forms.TextBox txtDetailsTotal;
        private System.Windows.Forms.Label labelTotal;
        private System.Windows.Forms.TextBox textBoxFinco;
        private System.Windows.Forms.TextBox textBoxNonFinco;
        private System.Windows.Forms.Label labelnonFinco;
        private System.Windows.Forms.Label labelFinco;
        public System.Windows.Forms.Button btnExcel;
    }
}