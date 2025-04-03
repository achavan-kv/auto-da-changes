namespace STL.PL
{
    partial class ReprintInvoice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReprintInvoice));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.grpSearchInvoice = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtAccountNumber = new STL.PL.AccountTextBox();
            this.txtOrdInvoiceNo = new System.Windows.Forms.TextBox();
            this.lblAccountNo = new System.Windows.Forms.Label();
            this.lblorderinvoiceno = new System.Windows.Forms.Label();
            this.dtDateTo = new System.Windows.Forms.DateTimePicker();
            this.dtDateFrom = new System.Windows.Forms.DateTimePicker();
            this.lblinvoiceDateto = new System.Windows.Forms.Label();
            this.lblInvoicedatefrom = new System.Windows.Forms.Label();
            this.lBranch = new System.Windows.Forms.Label();
            this.drpBranchNo = new System.Windows.Forms.ComboBox();
            this.grpInvoiceDetails = new System.Windows.Forms.GroupBox();
            this.dgAccounts = new System.Windows.Forms.DataGridView();
            this.chkTick = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.chkSelectall = new System.Windows.Forms.CheckBox();
            this.grpSearchInvoice.SuspendLayout();
            this.grpInvoiceDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
            this.SuspendLayout();
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuExit});
            this.menuFile.Text = "&File";
            this.menuFile.Click += new System.EventHandler(this.menuFile_Click);
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // grpSearchInvoice
            // 
            this.grpSearchInvoice.Controls.Add(this.btnClear);
            this.grpSearchInvoice.Controls.Add(this.btnSearch);
            this.grpSearchInvoice.Controls.Add(this.txtAccountNumber);
            this.grpSearchInvoice.Controls.Add(this.txtOrdInvoiceNo);
            this.grpSearchInvoice.Controls.Add(this.lblAccountNo);
            this.grpSearchInvoice.Controls.Add(this.lblorderinvoiceno);
            this.grpSearchInvoice.Controls.Add(this.dtDateTo);
            this.grpSearchInvoice.Controls.Add(this.dtDateFrom);
            this.grpSearchInvoice.Controls.Add(this.lblinvoiceDateto);
            this.grpSearchInvoice.Controls.Add(this.lblInvoicedatefrom);
            this.grpSearchInvoice.Controls.Add(this.lBranch);
            this.grpSearchInvoice.Controls.Add(this.drpBranchNo);
            this.grpSearchInvoice.Location = new System.Drawing.Point(12, 12);
            this.grpSearchInvoice.Name = "grpSearchInvoice";
            this.grpSearchInvoice.Size = new System.Drawing.Size(776, 96);
            this.grpSearchInvoice.TabIndex = 0;
            this.grpSearchInvoice.TabStop = false;
            this.grpSearchInvoice.Text = "Search Invoice Details";
            // 
            // btnClear
            // 
            this.btnClear.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClear.Location = new System.Drawing.Point(685, 42);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 27);
            this.btnClear.TabIndex = 52;
            this.btnClear.Text = "&Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.CausesValidation = false;
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSearch.Location = new System.Drawing.Point(588, 43);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 27);
            this.btnSearch.TabIndex = 51;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtAccountNumber
            // 
            this.txtAccountNumber.Location = new System.Drawing.Point(491, 48);
            this.txtAccountNumber.Name = "txtAccountNumber";
            this.txtAccountNumber.PreventPaste = false;
            this.txtAccountNumber.ReadOnly = true;
            this.txtAccountNumber.Size = new System.Drawing.Size(88, 20);
            this.txtAccountNumber.TabIndex = 50;
            this.txtAccountNumber.TabStop = false;
            this.txtAccountNumber.Text = "000-0000-0000-0";
            // 
            // txtOrdInvoiceNo
            // 
            this.txtOrdInvoiceNo.Location = new System.Drawing.Point(373, 48);
            this.txtOrdInvoiceNo.MaxLength = 17;
            this.txtOrdInvoiceNo.Name = "txtOrdInvoiceNo";
            this.txtOrdInvoiceNo.Size = new System.Drawing.Size(100, 20);
            this.txtOrdInvoiceNo.TabIndex = 48;
            this.txtOrdInvoiceNo.TextChanged += new System.EventHandler(this.txtOrdInvoiceNo_TextChanged);
            this.txtOrdInvoiceNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtOrdInvoiceNo_KeyDown);
            this.txtOrdInvoiceNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtOrdInvoiceNo_KeyPress);
            // 
            // lblAccountNo
            // 
            this.lblAccountNo.Location = new System.Drawing.Point(503, 24);
            this.lblAccountNo.Name = "lblAccountNo";
            this.lblAccountNo.Size = new System.Drawing.Size(64, 16);
            this.lblAccountNo.TabIndex = 47;
            this.lblAccountNo.Text = "Account No";
            this.lblAccountNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblorderinvoiceno
            // 
            this.lblorderinvoiceno.Location = new System.Drawing.Point(376, 25);
            this.lblorderinvoiceno.Name = "lblorderinvoiceno";
            this.lblorderinvoiceno.Size = new System.Drawing.Size(89, 16);
            this.lblorderinvoiceno.TabIndex = 46;
            this.lblorderinvoiceno.Text = "Ord/Invoice No";
            this.lblorderinvoiceno.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtDateTo
            // 
            this.dtDateTo.CustomFormat = "dd MMM yyyy";
            this.dtDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDateTo.Location = new System.Drawing.Point(238, 47);
            this.dtDateTo.Name = "dtDateTo";
            this.dtDateTo.Size = new System.Drawing.Size(112, 20);
            this.dtDateTo.TabIndex = 45;
            this.dtDateTo.Tag = "";
            this.dtDateTo.Value = new System.DateTime(2002, 5, 8, 14, 5, 24, 830);
            // 
            // dtDateFrom
            // 
            this.dtDateFrom.CustomFormat = "dd MMM yyyy";
            this.dtDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDateFrom.Location = new System.Drawing.Point(106, 47);
            this.dtDateFrom.Name = "dtDateFrom";
            this.dtDateFrom.Size = new System.Drawing.Size(112, 20);
            this.dtDateFrom.TabIndex = 44;
            this.dtDateFrom.Tag = "";
            this.dtDateFrom.Value = new System.DateTime(2018, 12, 25, 23, 59, 59, 0);
            // 
            // lblinvoiceDateto
            // 
            this.lblinvoiceDateto.Location = new System.Drawing.Point(242, 24);
            this.lblinvoiceDateto.Name = "lblinvoiceDateto";
            this.lblinvoiceDateto.Size = new System.Drawing.Size(96, 16);
            this.lblinvoiceDateto.TabIndex = 43;
            this.lblinvoiceDateto.Text = "Invoice Date To";
            this.lblinvoiceDateto.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblInvoicedatefrom
            // 
            this.lblInvoicedatefrom.Location = new System.Drawing.Point(110, 23);
            this.lblInvoicedatefrom.Name = "lblInvoicedatefrom";
            this.lblInvoicedatefrom.Size = new System.Drawing.Size(99, 16);
            this.lblInvoicedatefrom.TabIndex = 42;
            this.lblInvoicedatefrom.Text = "Invoice Date From";
            this.lblInvoicedatefrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lBranch
            // 
            this.lBranch.Location = new System.Drawing.Point(18, 24);
            this.lBranch.Name = "lBranch";
            this.lBranch.Size = new System.Drawing.Size(64, 16);
            this.lBranch.TabIndex = 41;
            this.lBranch.Text = "Branch No";
            this.lBranch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpBranchNo
            // 
            this.drpBranchNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranchNo.Location = new System.Drawing.Point(16, 48);
            this.drpBranchNo.Name = "drpBranchNo";
            this.drpBranchNo.Size = new System.Drawing.Size(72, 21);
            this.drpBranchNo.TabIndex = 40;
            // 
            // grpInvoiceDetails
            // 
            this.grpInvoiceDetails.Controls.Add(this.dgAccounts);
            this.grpInvoiceDetails.Controls.Add(this.btnCancel);
            this.grpInvoiceDetails.Controls.Add(this.btnPrint);
            this.grpInvoiceDetails.Controls.Add(this.chkSelectall);
            this.grpInvoiceDetails.Location = new System.Drawing.Point(12, 111);
            this.grpInvoiceDetails.Name = "grpInvoiceDetails";
            this.grpInvoiceDetails.Size = new System.Drawing.Size(776, 378);
            this.grpInvoiceDetails.TabIndex = 1;
            this.grpInvoiceDetails.TabStop = false;
            this.grpInvoiceDetails.Text = "Invoice Details";
            // 
            // dgAccounts
            // 
            this.dgAccounts.AllowUserToAddRows = false;
            this.dgAccounts.AllowUserToDeleteRows = false;
            this.dgAccounts.AllowUserToResizeColumns = false;
            this.dgAccounts.AllowUserToResizeRows = false;
            this.dgAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgAccounts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chkTick});
            this.dgAccounts.Location = new System.Drawing.Point(16, 41);
            this.dgAccounts.Name = "dgAccounts";
            this.dgAccounts.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.dgAccounts.Size = new System.Drawing.Size(754, 299);
            this.dgAccounts.TabIndex = 6;
            // 
            // chkTick
            // 
            this.chkTick.HeaderText = "Tick";
            this.chkTick.Name = "chkTick";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(652, 346);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(562, 346);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 4;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // chkSelectall
            // 
            this.chkSelectall.AutoSize = true;
            this.chkSelectall.Location = new System.Drawing.Point(21, 18);
            this.chkSelectall.Name = "chkSelectall";
            this.chkSelectall.Size = new System.Drawing.Size(70, 17);
            this.chkSelectall.TabIndex = 2;
            this.chkSelectall.Text = "Select All";
            this.chkSelectall.UseVisualStyleBackColor = true;
            this.chkSelectall.Visible = false;
            this.chkSelectall.CheckedChanged += new System.EventHandler(this.chkSelectall_CheckedChanged);
            // 
            // ReprintInvoice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(901, 501);
            this.Controls.Add(this.grpInvoiceDetails);
            this.Controls.Add(this.grpSearchInvoice);
            this.Name = "ReprintInvoice";
            this.Text = "ReprintInvoice";
            this.Load += new System.EventHandler(this.ReprintInvoice_Load);
            this.Leave += new System.EventHandler(this.ReprintInvoice_Leave);
            this.grpSearchInvoice.ResumeLayout(false);
            this.grpSearchInvoice.PerformLayout();
            this.grpInvoiceDetails.ResumeLayout(false);
            this.grpInvoiceDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSearchInvoice;
        private System.Windows.Forms.DateTimePicker dtDateTo;
        private System.Windows.Forms.DateTimePicker dtDateFrom;
        private System.Windows.Forms.Label lblinvoiceDateto;
        private System.Windows.Forms.Label lblInvoicedatefrom;
        private System.Windows.Forms.Label lBranch;
        private System.Windows.Forms.ComboBox drpBranchNo;
        private System.Windows.Forms.TextBox txtOrdInvoiceNo;
        private System.Windows.Forms.Label lblAccountNo;
        private System.Windows.Forms.Label lblorderinvoiceno;
        public AccountTextBox txtAccountNumber;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox grpInvoiceDetails;
        private System.Windows.Forms.CheckBox chkSelectall;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.DataGridView dgAccounts;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chkTick;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
    }
}