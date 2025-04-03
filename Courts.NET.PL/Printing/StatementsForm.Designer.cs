namespace STL.PL
{
    partial class StatementsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatementsForm));
            this.lblAccountNo = new System.Windows.Forms.Label();
            this.textBoxAccountNo = new System.Windows.Forms.TextBox();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.buttonPreviewPrintStatementForAccount = new System.Windows.Forms.Button();
            this.buttonPrintStatementForAccount = new System.Windows.Forms.Button();
            this.dateToPrintStatementForAccount = new System.Windows.Forms.DateTimePicker();
            this.lblDateFromPrintStatementForAccount = new System.Windows.Forms.Label();
            this.lblDateToPrintStatementForAccount = new System.Windows.Forms.Label();
            this.dateFromPrintStatementForAccount = new System.Windows.Forms.DateTimePicker();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonPreviewPrintStatementForCustomer = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxAccountsFilter = new System.Windows.Forms.ComboBox();
            this.buttonPrintStatementForCustomer = new System.Windows.Forms.Button();
            this.dateToPrintStatementForCustomer = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dateFromPrintStatementForCustomer = new System.Windows.Forms.DateTimePicker();
            this.textBoxCustomerID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblAccountNo
            // 
            this.lblAccountNo.AutoSize = true;
            this.lblAccountNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccountNo.Location = new System.Drawing.Point(6, 32);
            this.lblAccountNo.Name = "lblAccountNo";
            this.lblAccountNo.Size = new System.Drawing.Size(87, 13);
            this.lblAccountNo.TabIndex = 0;
            this.lblAccountNo.Text = "Account Number";
            // 
            // textBoxAccountNo
            // 
            this.textBoxAccountNo.Location = new System.Drawing.Point(99, 32);
            this.textBoxAccountNo.Name = "textBoxAccountNo";
            this.textBoxAccountNo.Size = new System.Drawing.Size(149, 20);
            this.textBoxAccountNo.TabIndex = 1;
            this.textBoxAccountNo.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.buttonPreviewPrintStatementForAccount);
            this.groupBox.Controls.Add(this.buttonPrintStatementForAccount);
            this.groupBox.Controls.Add(this.dateToPrintStatementForAccount);
            this.groupBox.Controls.Add(this.lblDateFromPrintStatementForAccount);
            this.groupBox.Controls.Add(this.lblDateToPrintStatementForAccount);
            this.groupBox.Controls.Add(this.dateFromPrintStatementForAccount);
            this.groupBox.Controls.Add(this.textBoxAccountNo);
            this.groupBox.Controls.Add(this.lblAccountNo);
            this.groupBox.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox.Location = new System.Drawing.Point(12, 12);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(263, 214);
            this.groupBox.TabIndex = 2;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Print Statement for One Account";
            // 
            // buttonPreviewPrintStatementForAccount
            // 
            this.buttonPreviewPrintStatementForAccount.Enabled = false;
            this.buttonPreviewPrintStatementForAccount.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPreviewPrintStatementForAccount.Image = ((System.Drawing.Image)(resources.GetObject("buttonPreviewPrintStatementForAccount.Image")));
            this.buttonPreviewPrintStatementForAccount.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonPreviewPrintStatementForAccount.Location = new System.Drawing.Point(6, 159);
            this.buttonPreviewPrintStatementForAccount.Name = "buttonPreviewPrintStatementForAccount";
            this.buttonPreviewPrintStatementForAccount.Size = new System.Drawing.Size(61, 49);
            this.buttonPreviewPrintStatementForAccount.TabIndex = 8;
            this.buttonPreviewPrintStatementForAccount.Text = "Preview";
            this.buttonPreviewPrintStatementForAccount.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonPreviewPrintStatementForAccount.UseVisualStyleBackColor = true;
            this.buttonPreviewPrintStatementForAccount.Click += new System.EventHandler(this.buttonPreviewPrintStatementForAccount_Click);
            // 
            // buttonPrintStatementForAccount
            // 
            this.buttonPrintStatementForAccount.Enabled = false;
            this.buttonPrintStatementForAccount.Font = new System.Drawing.Font("Arial Black", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPrintStatementForAccount.Image = ((System.Drawing.Image)(resources.GetObject("buttonPrintStatementForAccount.Image")));
            this.buttonPrintStatementForAccount.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonPrintStatementForAccount.Location = new System.Drawing.Point(99, 159);
            this.buttonPrintStatementForAccount.Name = "buttonPrintStatementForAccount";
            this.buttonPrintStatementForAccount.Size = new System.Drawing.Size(149, 49);
            this.buttonPrintStatementForAccount.TabIndex = 7;
            this.buttonPrintStatementForAccount.Text = "Print";
            this.buttonPrintStatementForAccount.UseVisualStyleBackColor = true;
            this.buttonPrintStatementForAccount.Click += new System.EventHandler(this.buttonPrintStatementForAccount_Click);
            // 
            // dateToPrintStatementForAccount
            // 
            this.dateToPrintStatementForAccount.CustomFormat = "ddd dd MMM yyyy";
            this.dateToPrintStatementForAccount.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateToPrintStatementForAccount.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dateToPrintStatementForAccount.Location = new System.Drawing.Point(99, 92);
            this.dateToPrintStatementForAccount.Name = "dateToPrintStatementForAccount";
            this.dateToPrintStatementForAccount.Size = new System.Drawing.Size(149, 20);
            this.dateToPrintStatementForAccount.TabIndex = 6;
            this.dateToPrintStatementForAccount.Value = new System.DateTime(2010, 5, 26, 17, 24, 20, 0);
            // 
            // lblDateFromPrintStatementForAccount
            // 
            this.lblDateFromPrintStatementForAccount.AutoSize = true;
            this.lblDateFromPrintStatementForAccount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDateFromPrintStatementForAccount.Location = new System.Drawing.Point(37, 61);
            this.lblDateFromPrintStatementForAccount.Name = "lblDateFromPrintStatementForAccount";
            this.lblDateFromPrintStatementForAccount.Size = new System.Drawing.Size(56, 13);
            this.lblDateFromPrintStatementForAccount.TabIndex = 5;
            this.lblDateFromPrintStatementForAccount.Text = "Date From";
            // 
            // lblDateToPrintStatementForAccount
            // 
            this.lblDateToPrintStatementForAccount.AutoSize = true;
            this.lblDateToPrintStatementForAccount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDateToPrintStatementForAccount.Location = new System.Drawing.Point(47, 92);
            this.lblDateToPrintStatementForAccount.Name = "lblDateToPrintStatementForAccount";
            this.lblDateToPrintStatementForAccount.Size = new System.Drawing.Size(46, 13);
            this.lblDateToPrintStatementForAccount.TabIndex = 4;
            this.lblDateToPrintStatementForAccount.Text = "Date To";
            // 
            // dateFromPrintStatementForAccount
            // 
            this.dateFromPrintStatementForAccount.CustomFormat = "ddd dd MMM yyyy";
            this.dateFromPrintStatementForAccount.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateFromPrintStatementForAccount.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dateFromPrintStatementForAccount.Location = new System.Drawing.Point(99, 61);
            this.dateFromPrintStatementForAccount.Name = "dateFromPrintStatementForAccount";
            this.dateFromPrintStatementForAccount.Size = new System.Drawing.Size(149, 20);
            this.dateFromPrintStatementForAccount.TabIndex = 3;
            this.dateFromPrintStatementForAccount.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonPreviewPrintStatementForCustomer);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.comboBoxAccountsFilter);
            this.groupBox2.Controls.Add(this.buttonPrintStatementForCustomer);
            this.groupBox2.Controls.Add(this.dateToPrintStatementForCustomer);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.dateFromPrintStatementForCustomer);
            this.groupBox2.Controls.Add(this.textBoxCustomerID);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(281, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(263, 214);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Print Statement for a Customers Accounts";
            // 
            // buttonPreviewPrintStatementForCustomer
            // 
            this.buttonPreviewPrintStatementForCustomer.Enabled = false;
            this.buttonPreviewPrintStatementForCustomer.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPreviewPrintStatementForCustomer.Image = ((System.Drawing.Image)(resources.GetObject("buttonPreviewPrintStatementForCustomer.Image")));
            this.buttonPreviewPrintStatementForCustomer.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonPreviewPrintStatementForCustomer.Location = new System.Drawing.Point(6, 159);
            this.buttonPreviewPrintStatementForCustomer.Name = "buttonPreviewPrintStatementForCustomer";
            this.buttonPreviewPrintStatementForCustomer.Size = new System.Drawing.Size(61, 49);
            this.buttonPreviewPrintStatementForCustomer.TabIndex = 10;
            this.buttonPreviewPrintStatementForCustomer.Text = "Preview";
            this.buttonPreviewPrintStatementForCustomer.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonPreviewPrintStatementForCustomer.UseVisualStyleBackColor = true;
            this.buttonPreviewPrintStatementForCustomer.Click += new System.EventHandler(this.buttonPreviewPrintStatementForCustomer_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(41, 121);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Accounts";
            // 
            // comboBoxAccountsFilter
            // 
            this.comboBoxAccountsFilter.FormattingEnabled = true;
            this.comboBoxAccountsFilter.Items.AddRange(new object[] {
            "Owned By Customer",
            "All Accounts"});
            this.comboBoxAccountsFilter.Location = new System.Drawing.Point(99, 118);
            this.comboBoxAccountsFilter.Name = "comboBoxAccountsFilter";
            this.comboBoxAccountsFilter.Size = new System.Drawing.Size(149, 22);
            this.comboBoxAccountsFilter.TabIndex = 8;
            // 
            // buttonPrintStatementForCustomer
            // 
            this.buttonPrintStatementForCustomer.Enabled = false;
            this.buttonPrintStatementForCustomer.Font = new System.Drawing.Font("Arial Black", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPrintStatementForCustomer.Image = ((System.Drawing.Image)(resources.GetObject("buttonPrintStatementForCustomer.Image")));
            this.buttonPrintStatementForCustomer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonPrintStatementForCustomer.Location = new System.Drawing.Point(99, 159);
            this.buttonPrintStatementForCustomer.Name = "buttonPrintStatementForCustomer";
            this.buttonPrintStatementForCustomer.Size = new System.Drawing.Size(149, 49);
            this.buttonPrintStatementForCustomer.TabIndex = 7;
            this.buttonPrintStatementForCustomer.Text = "Print";
            this.buttonPrintStatementForCustomer.UseVisualStyleBackColor = true;
            this.buttonPrintStatementForCustomer.Click += new System.EventHandler(this.buttonPrintStatementForCustomer_Click);
            // 
            // dateToPrintStatementForCustomer
            // 
            this.dateToPrintStatementForCustomer.CustomFormat = "ddd dd MMM yyyy";
            this.dateToPrintStatementForCustomer.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateToPrintStatementForCustomer.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dateToPrintStatementForCustomer.Location = new System.Drawing.Point(99, 92);
            this.dateToPrintStatementForCustomer.Name = "dateToPrintStatementForCustomer";
            this.dateToPrintStatementForCustomer.Size = new System.Drawing.Size(149, 20);
            this.dateToPrintStatementForCustomer.TabIndex = 6;
            this.dateToPrintStatementForCustomer.Value = new System.DateTime(2010, 5, 26, 17, 24, 20, 0);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(37, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Date From";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(47, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Date To";
            // 
            // dateFromPrintStatementForCustomer
            // 
            this.dateFromPrintStatementForCustomer.CustomFormat = "ddd dd MMM yyyy";
            this.dateFromPrintStatementForCustomer.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateFromPrintStatementForCustomer.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dateFromPrintStatementForCustomer.Location = new System.Drawing.Point(99, 61);
            this.dateFromPrintStatementForCustomer.Name = "dateFromPrintStatementForCustomer";
            this.dateFromPrintStatementForCustomer.Size = new System.Drawing.Size(149, 20);
            this.dateFromPrintStatementForCustomer.TabIndex = 3;
            this.dateFromPrintStatementForCustomer.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // textBoxCustomerID
            // 
            this.textBoxCustomerID.Location = new System.Drawing.Point(99, 32);
            this.textBoxCustomerID.Name = "textBoxCustomerID";
            this.textBoxCustomerID.Size = new System.Drawing.Size(149, 20);
            this.textBoxCustomerID.TabIndex = 1;
            this.textBoxCustomerID.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(28, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Customer ID";
            // 
            // StatementsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox);
            this.Name = "StatementsForm";
            this.Text = "Statements";
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblAccountNo;
        private System.Windows.Forms.TextBox textBoxAccountNo;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Label lblDateFromPrintStatementForAccount;
        private System.Windows.Forms.Label lblDateToPrintStatementForAccount;
        private System.Windows.Forms.DateTimePicker dateFromPrintStatementForAccount;
        private System.Windows.Forms.Button buttonPrintStatementForAccount;
        private System.Windows.Forms.DateTimePicker dateToPrintStatementForAccount;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonPrintStatementForCustomer;
        private System.Windows.Forms.DateTimePicker dateToPrintStatementForCustomer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dateFromPrintStatementForCustomer;
        private System.Windows.Forms.TextBox textBoxCustomerID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxAccountsFilter;
        private System.Windows.Forms.Button buttonPreviewPrintStatementForAccount;
        private System.Windows.Forms.Button buttonPreviewPrintStatementForCustomer;
    }
}