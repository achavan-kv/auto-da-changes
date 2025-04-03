namespace STL.PL.StoreCard
{
    partial class StoreCardAccount
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StoreCardAccount));
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnNewStoreCard = new System.Windows.Forms.Button();
            this.btnReplace = new System.Windows.Forms.Button();
            this.btnPrintAgreement = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.storeCardActivation1 = new STL.PL.StoreCard.StoreCardActivate();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.storeCardPaymentDetails1 = new STL.PL.StoreCard.StoreCardPaymentDetails();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.storeCardCancellation1 = new STL.PL.StoreCard.StoreCardCancellation();
            this.button1 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCardNumber = new STL.PL.StoreCard.StoreCardTextBox();
            this.lblCardNumber = new System.Windows.Forms.Label();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.cmbStartMonth = new System.Windows.Forms.ComboBox();
            this.cmbStartYear = new System.Windows.Forms.ComboBox();
            this.lblExpiryDate = new System.Windows.Forms.Label();
            this.cmbEndMonth = new System.Windows.Forms.ComboBox();
            this.cmbEndYear = new System.Windows.Forms.ComboBox();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabTransactions = new System.Windows.Forms.TabPage();
            this.StoreCardTransactions1 = new STL.PL.StoreCard.StoreCardTransactions();
            this.cmb_cardname = new System.Windows.Forms.ComboBox();
            this.lblNameonCard = new System.Windows.Forms.Label();
            this.txt_status = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabTransactions.SuspendLayout();
            this.SuspendLayout();
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // btnNewStoreCard
            // 
            this.btnNewStoreCard.Location = new System.Drawing.Point(653, 44);
            this.btnNewStoreCard.Name = "btnNewStoreCard";
            this.btnNewStoreCard.Size = new System.Drawing.Size(66, 26);
            this.btnNewStoreCard.TabIndex = 62;
            this.btnNewStoreCard.Text = "Add Card";
            this.toolTip1.SetToolTip(this.btnNewStoreCard, "Additional Card");
            this.btnNewStoreCard.UseVisualStyleBackColor = true;
            this.btnNewStoreCard.Click += new System.EventHandler(this.btnNewStoreCard_Click);
            // 
            // btnReplace
            // 
            this.btnReplace.Enabled = false;
            this.btnReplace.Location = new System.Drawing.Point(564, 44);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(66, 26);
            this.btnReplace.TabIndex = 81;
            this.btnReplace.Text = "Replace";
            this.toolTip1.SetToolTip(this.btnReplace, "Replacement Card");
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // btnPrintAgreement
            // 
            this.btnPrintAgreement.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnPrintAgreement.BackgroundImage")));
            this.btnPrintAgreement.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPrintAgreement.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPrintAgreement.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnPrintAgreement.Location = new System.Drawing.Point(737, 44);
            this.btnPrintAgreement.Name = "btnPrintAgreement";
            this.btnPrintAgreement.Size = new System.Drawing.Size(27, 24);
            this.btnPrintAgreement.TabIndex = 55;
            this.toolTip1.SetToolTip(this.btnPrintAgreement, "Print Store Card Note");
            this.btnPrintAgreement.Click += new System.EventHandler(this.btnPrintAgreement_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage4.Controls.Add(this.storeCardActivation1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(784, 385);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Activation";
            // 
            // storeCardActivation1
            // 
            this.storeCardActivation1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.storeCardActivation1.Location = new System.Drawing.Point(3, 3);
            this.storeCardActivation1.Name = "storeCardActivation1";
            this.storeCardActivation1.Size = new System.Drawing.Size(778, 379);
            this.storeCardActivation1.TabIndex = 0;
            this.storeCardActivation1.Load += new System.EventHandler(this.storeCardActivation1_Load);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.storeCardPaymentDetails1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(784, 385);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Payment Details";
            // 
            // storeCardPaymentDetails1
            // 
            this.storeCardPaymentDetails1.AcctNo = null;
            this.storeCardPaymentDetails1.Location = new System.Drawing.Point(3, 0);
            this.storeCardPaymentDetails1.Name = "storeCardPaymentDetails1";
            this.storeCardPaymentDetails1.Size = new System.Drawing.Size(755, 392);
            this.storeCardPaymentDetails1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.storeCardCancellation1);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(784, 385);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Cancellation";
            // 
            // storeCardCancellation1
            // 
            this.storeCardCancellation1.Location = new System.Drawing.Point(2, 3);
            this.storeCardCancellation1.Name = "storeCardCancellation1";
            this.storeCardCancellation1.Size = new System.Drawing.Size(779, 437);
            this.storeCardCancellation1.TabIndex = 91;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(679, 408);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 90;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(520, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 77;
            this.label6.Text = "Status";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCardNumber
            // 
            this.txtCardNumber.Location = new System.Drawing.Point(82, 40);
            this.txtCardNumber.MaxLength = 19;
            this.txtCardNumber.Name = "txtCardNumber";
            this.txtCardNumber.PreventPaste = true;
            this.txtCardNumber.ReadOnly = true;
            this.txtCardNumber.Size = new System.Drawing.Size(116, 20);
            this.txtCardNumber.TabIndex = 75;
            this.txtCardNumber.Text = "0000-0000-0000-0000";
            // 
            // lblCardNumber
            // 
            this.lblCardNumber.AutoSize = true;
            this.lblCardNumber.Location = new System.Drawing.Point(5, 43);
            this.lblCardNumber.Name = "lblCardNumber";
            this.lblCardNumber.Size = new System.Drawing.Size(69, 13);
            this.lblCardNumber.TabIndex = 65;
            this.lblCardNumber.Text = "Card Number";
            this.lblCardNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(316, 15);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(55, 13);
            this.lblStartDate.TabIndex = 70;
            this.lblStartDate.Text = "Start Date";
            this.lblStartDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbStartMonth
            // 
            this.cmbStartMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbStartMonth.Enabled = false;
            this.cmbStartMonth.FormattingEnabled = true;
            this.cmbStartMonth.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12"});
            this.cmbStartMonth.Location = new System.Drawing.Point(373, 12);
            this.cmbStartMonth.Name = "cmbStartMonth";
            this.cmbStartMonth.Size = new System.Drawing.Size(54, 21);
            this.cmbStartMonth.TabIndex = 63;
            // 
            // cmbStartYear
            // 
            this.cmbStartYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbStartYear.Enabled = false;
            this.cmbStartYear.FormattingEnabled = true;
            this.cmbStartYear.Location = new System.Drawing.Point(444, 12);
            this.cmbStartYear.Name = "cmbStartYear";
            this.cmbStartYear.Size = new System.Drawing.Size(70, 21);
            this.cmbStartYear.TabIndex = 67;
            // 
            // lblExpiryDate
            // 
            this.lblExpiryDate.AutoSize = true;
            this.lblExpiryDate.Location = new System.Drawing.Point(309, 43);
            this.lblExpiryDate.Name = "lblExpiryDate";
            this.lblExpiryDate.Size = new System.Drawing.Size(61, 13);
            this.lblExpiryDate.TabIndex = 71;
            this.lblExpiryDate.Text = "Expiry Date";
            this.lblExpiryDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbEndMonth
            // 
            this.cmbEndMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbEndMonth.Enabled = false;
            this.cmbEndMonth.FormattingEnabled = true;
            this.cmbEndMonth.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12"});
            this.cmbEndMonth.Location = new System.Drawing.Point(373, 40);
            this.cmbEndMonth.Name = "cmbEndMonth";
            this.cmbEndMonth.Size = new System.Drawing.Size(54, 21);
            this.cmbEndMonth.TabIndex = 68;
            // 
            // cmbEndYear
            // 
            this.cmbEndYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbEndYear.Enabled = false;
            this.cmbEndYear.FormattingEnabled = true;
            this.cmbEndYear.Location = new System.Drawing.Point(444, 40);
            this.cmbEndYear.Name = "cmbEndYear";
            this.cmbEndYear.Size = new System.Drawing.Size(70, 21);
            this.cmbEndYear.TabIndex = 69;
            // 
            // tabMain
            // 
            this.tabMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabMain.Controls.Add(this.tabPage4);
            this.tabMain.Controls.Add(this.tabPage1);
            this.tabMain.Controls.Add(this.tabPage2);
            this.tabMain.Controls.Add(this.tabTransactions);
            this.tabMain.Location = new System.Drawing.Point(0, 66);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(792, 411);
            this.tabMain.TabIndex = 0;
            // 
            // tabTransactions
            // 
            this.tabTransactions.Controls.Add(this.StoreCardTransactions1);
            this.tabTransactions.Location = new System.Drawing.Point(4, 22);
            this.tabTransactions.Name = "tabTransactions";
            this.tabTransactions.Size = new System.Drawing.Size(784, 385);
            this.tabTransactions.TabIndex = 4;
            this.tabTransactions.Text = "Transactions";
            this.tabTransactions.UseVisualStyleBackColor = true;
            // 
            // StoreCardTransactions1
            // 
            this.StoreCardTransactions1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StoreCardTransactions1.Location = new System.Drawing.Point(0, 0);
            this.StoreCardTransactions1.Name = "StoreCardTransactions1";
            this.StoreCardTransactions1.Size = new System.Drawing.Size(784, 385);
            this.StoreCardTransactions1.TabIndex = 0;
            // 
            // cmb_cardname
            // 
            this.cmb_cardname.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_cardname.FormattingEnabled = true;
            this.cmb_cardname.Location = new System.Drawing.Point(82, 12);
            this.cmb_cardname.Name = "cmb_cardname";
            this.cmb_cardname.Size = new System.Drawing.Size(225, 21);
            this.cmb_cardname.TabIndex = 77;
            // 
            // lblNameonCard
            // 
            this.lblNameonCard.AutoSize = true;
            this.lblNameonCard.Location = new System.Drawing.Point(2, 15);
            this.lblNameonCard.Name = "lblNameonCard";
            this.lblNameonCard.Size = new System.Drawing.Size(75, 13);
            this.lblNameonCard.TabIndex = 75;
            this.lblNameonCard.Text = "Name on Card";
            this.lblNameonCard.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_status
            // 
            this.txt_status.Location = new System.Drawing.Point(564, 15);
            this.txt_status.Name = "txt_status";
            this.txt_status.ReadOnly = true;
            this.txt_status.Size = new System.Drawing.Size(200, 20);
            this.txt_status.TabIndex = 80;
            // 
            // StoreCardAccount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.btnReplace);
            this.Controls.Add(this.txt_status);
            this.Controls.Add(this.btnNewStoreCard);
            this.Controls.Add(this.btnPrintAgreement);
            this.Controls.Add(this.cmb_cardname);
            this.Controls.Add(this.lblNameonCard);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.txtCardNumber);
            this.Controls.Add(this.lblCardNumber);
            this.Controls.Add(this.cmbStartYear);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbStartMonth);
            this.Controls.Add(this.lblStartDate);
            this.Controls.Add(this.cmbEndYear);
            this.Controls.Add(this.cmbEndMonth);
            this.Controls.Add(this.lblExpiryDate);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StoreCardAccount";
            this.Text = "Store Card";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabTransactions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label6;
        private StoreCardTextBox txtCardNumber;
        private System.Windows.Forms.Label lblCardNumber;
        private System.Windows.Forms.Button btnNewStoreCard;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.ComboBox cmbStartMonth;
        private System.Windows.Forms.ComboBox cmbStartYear;
        private System.Windows.Forms.Label lblExpiryDate;
        private System.Windows.Forms.ComboBox cmbEndMonth;
        private System.Windows.Forms.ComboBox cmbEndYear;
        private System.Windows.Forms.Button btnPrintAgreement;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ComboBox cmb_cardname;
        private System.Windows.Forms.Label lblNameonCard;
        private StoreCardActivate storeCardActivation1;
        private System.Windows.Forms.TextBox txt_status;
        private StoreCardPaymentDetails storeCardPaymentDetails1;
        private StoreCardCancellation storeCardCancellation1;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.TabPage tabTransactions;
        private StoreCardTransactions StoreCardTransactions1;

    }
}
