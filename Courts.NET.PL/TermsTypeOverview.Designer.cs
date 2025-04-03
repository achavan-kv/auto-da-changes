namespace STL.PL
{
    partial class TermsTypeOverview
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TermsTypeOverview));
            this.dgOverview = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnAdjustBands = new System.Windows.Forms.Button();
            this.lAdjustDate = new System.Windows.Forms.Label();
            this.gbAdjustSC = new System.Windows.Forms.GroupBox();
            this.numAdjustSC = new System.Windows.Forms.NumericUpDown();
            this.cbAdjustSC = new System.Windows.Forms.CheckBox();
            this.gbAdjustIns = new System.Windows.Forms.GroupBox();
            this.cbAdjustIns = new System.Windows.Forms.CheckBox();
            this.numAdjustIns = new System.Windows.Forms.NumericUpDown();
            this.dtAdjustDate = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.dgOverview)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.gbAdjustSC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAdjustSC)).BeginInit();
            this.gbAdjustIns.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAdjustIns)).BeginInit();
            this.SuspendLayout();
            // 
            // dgOverview
            // 
            this.dgOverview.AllowUserToAddRows = false;
            this.dgOverview.AllowUserToDeleteRows = false;
            this.dgOverview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgOverview.Location = new System.Drawing.Point(12, 12);
            this.dgOverview.Name = "dgOverview";
            this.dgOverview.ReadOnly = true;
            this.dgOverview.Size = new System.Drawing.Size(846, 316);
            this.dgOverview.TabIndex = 38;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnAdjustBands);
            this.groupBox1.Controls.Add(this.lAdjustDate);
            this.groupBox1.Controls.Add(this.gbAdjustSC);
            this.groupBox1.Controls.Add(this.gbAdjustIns);
            this.groupBox1.Controls.Add(this.dtAdjustDate);
            this.groupBox1.Location = new System.Drawing.Point(12, 320);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(846, 89);
            this.groupBox1.TabIndex = 44;
            this.groupBox1.TabStop = false;
            // 
            // btnAdjustBands
            // 
            this.btnAdjustBands.Enabled = false;
            this.btnAdjustBands.Location = new System.Drawing.Point(589, 55);
            this.btnAdjustBands.Name = "btnAdjustBands";
            this.btnAdjustBands.Size = new System.Drawing.Size(118, 23);
            this.btnAdjustBands.TabIndex = 48;
            this.btnAdjustBands.TabStop = false;
            this.btnAdjustBands.Text = "Adjust All Bands";
            this.btnAdjustBands.Click += new System.EventHandler(this.btnAdjustBands_Click);
            // 
            // lAdjustDate
            // 
            this.lAdjustDate.Location = new System.Drawing.Point(499, 20);
            this.lAdjustDate.Name = "lAdjustDate";
            this.lAdjustDate.Size = new System.Drawing.Size(84, 21);
            this.lAdjustDate.TabIndex = 45;
            this.lAdjustDate.Text = "Start Date";
            this.lAdjustDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gbAdjustSC
            // 
            this.gbAdjustSC.Controls.Add(this.numAdjustSC);
            this.gbAdjustSC.Controls.Add(this.cbAdjustSC);
            this.gbAdjustSC.Location = new System.Drawing.Point(267, 20);
            this.gbAdjustSC.Name = "gbAdjustSC";
            this.gbAdjustSC.Size = new System.Drawing.Size(151, 58);
            this.gbAdjustSC.TabIndex = 47;
            this.gbAdjustSC.TabStop = false;
            // 
            // numAdjustSC
            // 
            this.numAdjustSC.DecimalPlaces = 3;
            this.numAdjustSC.Enabled = false;
            this.numAdjustSC.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.numAdjustSC.Location = new System.Drawing.Point(41, 23);
            this.numAdjustSC.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numAdjustSC.Name = "numAdjustSC";
            this.numAdjustSC.Size = new System.Drawing.Size(62, 20);
            this.numAdjustSC.TabIndex = 38;
            this.numAdjustSC.ValueChanged += new System.EventHandler(this.numAdjustSC_ValueChanged);
            // 
            // cbAdjustSC
            // 
            this.cbAdjustSC.AutoSize = true;
            this.cbAdjustSC.Location = new System.Drawing.Point(12, 0);
            this.cbAdjustSC.Name = "cbAdjustSC";
            this.cbAdjustSC.Size = new System.Drawing.Size(131, 17);
            this.cbAdjustSC.TabIndex = 32;
            this.cbAdjustSC.Text = "Adjust Service Charge";
            this.cbAdjustSC.UseVisualStyleBackColor = true;
            this.cbAdjustSC.CheckedChanged += new System.EventHandler(this.cbAdjustSC_CheckedChanged);
            // 
            // gbAdjustIns
            // 
            this.gbAdjustIns.Controls.Add(this.cbAdjustIns);
            this.gbAdjustIns.Controls.Add(this.numAdjustIns);
            this.gbAdjustIns.Location = new System.Drawing.Point(88, 20);
            this.gbAdjustIns.Name = "gbAdjustIns";
            this.gbAdjustIns.Size = new System.Drawing.Size(151, 59);
            this.gbAdjustIns.TabIndex = 46;
            this.gbAdjustIns.TabStop = false;
            // 
            // cbAdjustIns
            // 
            this.cbAdjustIns.AutoSize = true;
            this.cbAdjustIns.Location = new System.Drawing.Point(12, 0);
            this.cbAdjustIns.Name = "cbAdjustIns";
            this.cbAdjustIns.Size = new System.Drawing.Size(105, 17);
            this.cbAdjustIns.TabIndex = 36;
            this.cbAdjustIns.Text = "Adjust Insurance";
            this.cbAdjustIns.UseVisualStyleBackColor = true;
            this.cbAdjustIns.CheckedChanged += new System.EventHandler(this.cbAdjustIns_CheckedChanged);
            // 
            // numAdjustIns
            // 
            this.numAdjustIns.DecimalPlaces = 3;
            this.numAdjustIns.Enabled = false;
            this.numAdjustIns.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.numAdjustIns.Location = new System.Drawing.Point(41, 23);
            this.numAdjustIns.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numAdjustIns.Name = "numAdjustIns";
            this.numAdjustIns.Size = new System.Drawing.Size(62, 20);
            this.numAdjustIns.TabIndex = 37;
            this.numAdjustIns.ValueChanged += new System.EventHandler(this.numAdjustIns_ValueChanged);
            // 
            // dtAdjustDate
            // 
            this.dtAdjustDate.CustomFormat = "ddd dd MMM yyyy";
            this.dtAdjustDate.Enabled = false;
            this.dtAdjustDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtAdjustDate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtAdjustDate.Location = new System.Drawing.Point(589, 21);
            this.dtAdjustDate.Name = "dtAdjustDate";
            this.dtAdjustDate.Size = new System.Drawing.Size(118, 20);
            this.dtAdjustDate.TabIndex = 44;
            // 
            // TermsTypeOverview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(870, 416);
            this.Controls.Add(this.dgOverview);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TermsTypeOverview";
            this.Text = "Terms Type Overview";
            this.Load += new System.EventHandler(this.TermsTypeOverview_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgOverview)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.gbAdjustSC.ResumeLayout(false);
            this.gbAdjustSC.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAdjustSC)).EndInit();
            this.gbAdjustIns.ResumeLayout(false);
            this.gbAdjustIns.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAdjustIns)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgOverview;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnAdjustBands;
        private System.Windows.Forms.Label lAdjustDate;
        private System.Windows.Forms.GroupBox gbAdjustSC;
        private System.Windows.Forms.NumericUpDown numAdjustSC;
        private System.Windows.Forms.CheckBox cbAdjustSC;
        private System.Windows.Forms.GroupBox gbAdjustIns;
        private System.Windows.Forms.CheckBox cbAdjustIns;
        private System.Windows.Forms.NumericUpDown numAdjustIns;
        private System.Windows.Forms.DateTimePicker dtAdjustDate;
    }
}