namespace STL.PL
{
    partial class InstantReplacement
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstantReplacement));
            this.btnClear = new System.Windows.Forms.Button();
            this.txtCustID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtInvoiceNum = new System.Windows.Forms.TextBox();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.label4 = new System.Windows.Forms.Label();
            this.dtEnd = new System.Windows.Forms.DateTimePicker();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dgAccounts = new System.Windows.Forms.DataGrid();
            this.menuInstantReplacement = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtAccountNo = new STL.PL.AccountTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lAccountType = new System.Windows.Forms.Label();
            this.drpAccountType = new System.Windows.Forms.ComboBox();
            this.dtStart = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClear
            // 
            this.btnClear.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClear.Location = new System.Drawing.Point(429, 82);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "&Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // txtCustID
            // 
            this.txtCustID.Enabled = false;
            this.txtCustID.Location = new System.Drawing.Point(515, 44);
            this.txtCustID.MaxLength = 30;
            this.txtCustID.Name = "txtCustID";
            this.txtCustID.Size = new System.Drawing.Size(88, 20);
            this.txtCustID.TabIndex = 1;
            this.txtCustID.Tag = "";
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(512, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Customer ID";
            // 
            // txtInvoiceNum
            // 
            this.txtInvoiceNum.Enabled = false;
            this.txtInvoiceNum.Location = new System.Drawing.Point(631, 44);
            this.txtInvoiceNum.MaxLength = 15;
            this.txtInvoiceNum.Name = "txtInvoiceNum";
            this.txtInvoiceNum.Size = new System.Drawing.Size(88, 20);
            this.txtInvoiceNum.TabIndex = 0;
            this.txtInvoiceNum.Tag = "";
            this.txtInvoiceNum.Text = "0";
            this.txtInvoiceNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtInvoiceNum.Validating += new System.ComponentModel.CancelEventHandler(this.txtInvoiceNum_Validating);
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataMember = "";
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(628, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Invoice Number";
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuExit});
            this.menuFile.Text = "&File";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(128, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 16);
            this.label4.TabIndex = 43;
            this.label4.Text = "Delivery Date Between";
            // 
            // dtEnd
            // 
            this.dtEnd.CustomFormat = "ddd dd MMM yyyy";
            this.dtEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtEnd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtEnd.Location = new System.Drawing.Point(258, 44);
            this.dtEnd.Name = "dtEnd";
            this.dtEnd.Size = new System.Drawing.Size(112, 20);
            this.dtEnd.TabIndex = 3;
            this.dtEnd.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox3.CausesValidation = false;
            this.groupBox3.Controls.Add(this.dgAccounts);
            this.groupBox3.Controls.Add(this.menuInstantReplacement);
            this.groupBox3.Location = new System.Drawing.Point(8, 121);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(776, 352);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Search Results";
            // 
            // dgAccounts
            // 
            this.dgAccounts.CaptionText = "Accounts";
            this.dgAccounts.DataMember = "";
            this.dgAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAccounts.Location = new System.Drawing.Point(8, 16);
            this.dgAccounts.Name = "dgAccounts";
            this.dgAccounts.ReadOnly = true;
            this.dgAccounts.Size = new System.Drawing.Size(760, 328);
            this.dgAccounts.TabIndex = 0;
            this.dgAccounts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgAccounts_MouseUp);
            // 
            // menuInstantReplacement
            // 
            this.menuInstantReplacement.Enabled = false;
            this.menuInstantReplacement.Location = new System.Drawing.Point(248, 80);
            this.menuInstantReplacement.Name = "menuInstantReplacement";
            this.menuInstantReplacement.Size = new System.Drawing.Size(72, 16);
            this.menuInstantReplacement.TabIndex = 44;
            this.menuInstantReplacement.Text = "dummyMenu";
            this.menuInstantReplacement.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.CausesValidation = false;
            this.groupBox1.Controls.Add(this.txtAccountNo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lAccountType);
            this.groupBox1.Controls.Add(this.drpAccountType);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.dtEnd);
            this.groupBox1.Controls.Add(this.dtStart);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.txtCustID);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtInvoiceNum);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(8, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 118);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search Criteria";
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.Enabled = false;
            this.txtAccountNo.Location = new System.Drawing.Point(396, 44);
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.Size = new System.Drawing.Size(88, 20);
            this.txtAccountNo.TabIndex = 48;
            this.txtAccountNo.Tag = "ACCNO";
            this.txtAccountNo.Text = "000-0000-0000-0";
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(395, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 47;
            this.label1.Text = "Account Number";
            // 
            // lAccountType
            // 
            this.lAccountType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lAccountType.Location = new System.Drawing.Point(47, 27);
            this.lAccountType.Name = "lAccountType";
            this.lAccountType.Size = new System.Drawing.Size(40, 16);
            this.lAccountType.TabIndex = 46;
            this.lAccountType.Text = "Type";
            this.lAccountType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpAccountType
            // 
            this.drpAccountType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpAccountType.DropDownWidth = 40;
            this.drpAccountType.Location = new System.Drawing.Point(50, 44);
            this.drpAccountType.Name = "drpAccountType";
            this.drpAccountType.Size = new System.Drawing.Size(58, 21);
            this.drpAccountType.TabIndex = 45;
            this.drpAccountType.SelectedIndexChanged += new System.EventHandler(this.drpAccountType_SelectedIndexChanged);
            // 
            // dtStart
            // 
            this.dtStart.CustomFormat = "ddd dd MMM yyyy";
            this.dtStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtStart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dtStart.Location = new System.Drawing.Point(129, 44);
            this.dtStart.Name = "dtStart";
            this.dtStart.Size = new System.Drawing.Size(112, 20);
            this.dtStart.TabIndex = 2;
            this.dtStart.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // btnSearch
            // 
            this.btnSearch.CausesValidation = false;
            this.btnSearch.Enabled = false;
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSearch.Location = new System.Drawing.Point(269, 82);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // InstantReplacement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "InstantReplacement";
            this.Text = "Instant Replacement";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox txtCustID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtInvoiceNum;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGrid dgAccounts;
        private System.Windows.Forms.Label menuInstantReplacement;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtEnd;
        private System.Windows.Forms.DateTimePicker dtStart;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label2;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private System.Windows.Forms.Label lAccountType;
        private System.Windows.Forms.ComboBox drpAccountType;
        private AccountTextBox txtAccountNo;
        private System.Windows.Forms.Label label1;
    }
}