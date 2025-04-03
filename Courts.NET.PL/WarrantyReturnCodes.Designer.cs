namespace STL.PL
{
    partial class WarrantyReturnCodes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WarrantyReturnCodes));
            this.gbReturnCodes = new System.Windows.Forms.GroupBox();
            this.dgReturnCodes = new System.Windows.Forms.DataGridView();
            this.gbAddEdit = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lbRefund = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtReturnCode = new System.Windows.Forms.TextBox();
            this.lbManLength = new System.Windows.Forms.Label();
            this.lbWarrantyLength = new System.Windows.Forms.Label();
            this.lbcategory = new System.Windows.Forms.Label();
            this.udRefundPct = new System.Windows.Forms.NumericUpDown();
            this.udExpiredMonths = new System.Windows.Forms.NumericUpDown();
            this.udManufactLength = new System.Windows.Forms.NumericUpDown();
            this.udWarrantyLength = new System.Windows.Forms.NumericUpDown();
            this.drpCategory = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnReload = new System.Windows.Forms.Button();
            this.errorProvider2 = new System.Windows.Forms.ErrorProvider(this.components);
            this.gbReturnCodes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgReturnCodes)).BeginInit();
            this.gbAddEdit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udRefundPct)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udExpiredMonths)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udManufactLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udWarrantyLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).BeginInit();
            this.SuspendLayout();
            // 
            // gbReturnCodes
            // 
            this.gbReturnCodes.Controls.Add(this.dgReturnCodes);
            this.gbReturnCodes.Location = new System.Drawing.Point(27, 47);
            this.gbReturnCodes.Name = "gbReturnCodes";
            this.gbReturnCodes.Size = new System.Drawing.Size(742, 302);
            this.gbReturnCodes.TabIndex = 0;
            this.gbReturnCodes.TabStop = false;
            this.gbReturnCodes.Text = "Return Codes";
            // 
            // dgReturnCodes
            // 
            this.dgReturnCodes.AllowUserToAddRows = false;
            this.dgReturnCodes.AllowUserToResizeColumns = false;
            this.dgReturnCodes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgReturnCodes.Location = new System.Drawing.Point(9, 29);
            this.dgReturnCodes.MultiSelect = false;
            this.dgReturnCodes.Name = "dgReturnCodes";
            this.dgReturnCodes.ReadOnly = true;
            this.dgReturnCodes.Size = new System.Drawing.Size(725, 251);
            this.dgReturnCodes.TabIndex = 0;
            this.dgReturnCodes.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgReturnCodes_ColumnHeaderMouseClick);
            this.dgReturnCodes.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgReturnCodes_RowHeaderMouseClick);
            // 
            // gbAddEdit
            // 
            this.gbAddEdit.Controls.Add(this.btnClear);
            this.gbAddEdit.Controls.Add(this.btnAdd);
            this.gbAddEdit.Controls.Add(this.btnDelete);
            this.gbAddEdit.Controls.Add(this.lbRefund);
            this.gbAddEdit.Controls.Add(this.label2);
            this.gbAddEdit.Controls.Add(this.label1);
            this.gbAddEdit.Controls.Add(this.txtReturnCode);
            this.gbAddEdit.Controls.Add(this.lbManLength);
            this.gbAddEdit.Controls.Add(this.lbWarrantyLength);
            this.gbAddEdit.Controls.Add(this.lbcategory);
            this.gbAddEdit.Controls.Add(this.udRefundPct);
            this.gbAddEdit.Controls.Add(this.udExpiredMonths);
            this.gbAddEdit.Controls.Add(this.udManufactLength);
            this.gbAddEdit.Controls.Add(this.udWarrantyLength);
            this.gbAddEdit.Controls.Add(this.drpCategory);
            this.gbAddEdit.Location = new System.Drawing.Point(27, 355);
            this.gbAddEdit.Name = "gbAddEdit";
            this.gbAddEdit.Size = new System.Drawing.Size(742, 110);
            this.gbAddEdit.TabIndex = 1;
            this.gbAddEdit.TabStop = false;
            this.gbAddEdit.Text = "Add/Edit";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(649, 75);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(47, 23);
            this.btnClear.TabIndex = 11;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.Gray;
            this.btnAdd.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnAdd.Image = global::STL.PL.Properties.Resources.plus;
            this.btnAdd.Location = new System.Drawing.Point(714, 29);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(20, 20);
            this.btnAdd.TabIndex = 9;
            this.btnAdd.TabStop = false;
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.Gray;
            this.btnDelete.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnDelete.Image = global::STL.PL.Properties.Resources.Minus;
            this.btnDelete.Location = new System.Drawing.Point(714, 52);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(20, 20);
            this.btnDelete.TabIndex = 10;
            this.btnDelete.TabStop = false;
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lbRefund
            // 
            this.lbRefund.AutoSize = true;
            this.lbRefund.Location = new System.Drawing.Point(643, 20);
            this.lbRefund.Name = "lbRefund";
            this.lbRefund.Size = new System.Drawing.Size(53, 13);
            this.lbRefund.TabIndex = 11;
            this.lbRefund.Text = "% Refund";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(144, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Return Code";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(512, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Expired Portion (Months)";
            // 
            // txtReturnCode
            // 
            this.txtReturnCode.Location = new System.Drawing.Point(138, 42);
            this.txtReturnCode.MaxLength = 8;
            this.txtReturnCode.Name = "txtReturnCode";
            this.txtReturnCode.Size = new System.Drawing.Size(89, 20);
            this.txtReturnCode.TabIndex = 4;
            // 
            // lbManLength
            // 
            this.lbManLength.AutoSize = true;
            this.lbManLength.Location = new System.Drawing.Point(355, 20);
            this.lbManLength.Name = "lbManLength";
            this.lbManLength.Size = new System.Drawing.Size(150, 13);
            this.lbManLength.TabIndex = 8;
            this.lbManLength.Text = "Manufacturer Length (Months)";
            // 
            // lbWarrantyLength
            // 
            this.lbWarrantyLength.AutoSize = true;
            this.lbWarrantyLength.Location = new System.Drawing.Point(221, 20);
            this.lbWarrantyLength.Name = "lbWarrantyLength";
            this.lbWarrantyLength.Size = new System.Drawing.Size(130, 13);
            this.lbWarrantyLength.TabIndex = 7;
            this.lbWarrantyLength.Text = "Warranty Length (Months)";
            // 
            // lbcategory
            // 
            this.lbcategory.AutoSize = true;
            this.lbcategory.Location = new System.Drawing.Point(17, 20);
            this.lbcategory.Name = "lbcategory";
            this.lbcategory.Size = new System.Drawing.Size(49, 13);
            this.lbcategory.TabIndex = 6;
            this.lbcategory.Text = "Category";
            // 
            // udRefundPct
            // 
            this.udRefundPct.DecimalPlaces = 2;
            this.udRefundPct.Location = new System.Drawing.Point(637, 42);
            this.udRefundPct.Name = "udRefundPct";
            this.udRefundPct.Size = new System.Drawing.Size(71, 20);
            this.udRefundPct.TabIndex = 8;
            // 
            // udExpiredMonths
            // 
            this.udExpiredMonths.Location = new System.Drawing.Point(534, 42);
            this.udExpiredMonths.Name = "udExpiredMonths";
            this.udExpiredMonths.Size = new System.Drawing.Size(76, 20);
            this.udExpiredMonths.TabIndex = 7;
            // 
            // udManufactLength
            // 
            this.udManufactLength.Location = new System.Drawing.Point(389, 42);
            this.udManufactLength.Name = "udManufactLength";
            this.udManufactLength.Size = new System.Drawing.Size(72, 20);
            this.udManufactLength.TabIndex = 6;
            // 
            // udWarrantyLength
            // 
            this.udWarrantyLength.Location = new System.Drawing.Point(241, 42);
            this.udWarrantyLength.Name = "udWarrantyLength";
            this.udWarrantyLength.Size = new System.Drawing.Size(80, 20);
            this.udWarrantyLength.TabIndex = 5;
            // 
            // drpCategory
            // 
            this.drpCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpCategory.FormattingEnabled = true;
            this.drpCategory.Location = new System.Drawing.Point(17, 42);
            this.drpCategory.Name = "drpCategory";
            this.drpCategory.Size = new System.Drawing.Size(107, 21);
            this.drpCategory.TabIndex = 0;
            this.drpCategory.SelectedIndexChanged += new System.EventHandler(this.drpCategory_SelectedIndexChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(585, 18);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(63, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(685, 17);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(63, 23);
            this.btnReload.TabIndex = 3;
            this.btnReload.Text = "ReLoad";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // errorProvider2
            // 
            this.errorProvider2.ContainerControl = this;
            this.errorProvider2.Icon = ((System.Drawing.Icon)(resources.GetObject("errorProvider2.Icon")));
            // 
            // WarrantyReturnCodes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.gbAddEdit);
            this.Controls.Add(this.gbReturnCodes);
            this.Name = "WarrantyReturnCodes";
            this.Text = "Warranty Return Codes Maintenance";
            this.Load += new System.EventHandler(this.WarrantyReturnCodes_Load);
            this.gbReturnCodes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgReturnCodes)).EndInit();
            this.gbAddEdit.ResumeLayout(false);
            this.gbAddEdit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udRefundPct)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udExpiredMonths)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udManufactLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udWarrantyLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbReturnCodes;
        private System.Windows.Forms.DataGridView dgReturnCodes;
        private System.Windows.Forms.GroupBox gbAddEdit;
        private System.Windows.Forms.Label lbRefund;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtReturnCode;
        private System.Windows.Forms.Label lbManLength;
        private System.Windows.Forms.Label lbWarrantyLength;
        private System.Windows.Forms.Label lbcategory;
        private System.Windows.Forms.NumericUpDown udRefundPct;
        private System.Windows.Forms.NumericUpDown udExpiredMonths;
        private System.Windows.Forms.NumericUpDown udManufactLength;
        private System.Windows.Forms.NumericUpDown udWarrantyLength;
        private System.Windows.Forms.ComboBox drpCategory;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.ErrorProvider errorProvider2;
    }
}