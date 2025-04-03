namespace STL.PL.StoreCard
{
    partial class StoreCardPaymentDetails
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StoreCardPaymentDetails));
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtMinPayment = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPendingInterest = new System.Windows.Forms.TextBox();
            this.dtp_StatementDate = new System.Windows.Forms.DateTimePicker();
            this.dtNotePrinted = new System.Windows.Forms.DateTimePicker();
            this.lblNotePrinted = new System.Windows.Forms.Label();
            this.txtStoreCardAvailable = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtStoreCardLimit = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dateNotePrinted = new System.Windows.Forms.DateTimePicker();
            this.lblDateNotePrinted = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtBalance = new System.Windows.Forms.TextBox();
            this.lblInterestRate = new System.Windows.Forms.Label();
            this.lblDueDate = new System.Windows.Forms.Label();
            this.dtp_DatePaymentDue = new System.Windows.Forms.DateTimePicker();
            this.lblBalance = new System.Windows.Forms.Label();
            this.txtInterestRate = new System.Windows.Forms.TextBox();
            this.txtArrears = new System.Windows.Forms.TextBox();
            this.lblArrears = new System.Windows.Forms.Label();
            this.txtNotPrinted = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkFixed = new System.Windows.Forms.CheckBox();
            this.ComboContactMethod = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboStatements = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPaymentMethod = new System.Windows.Forms.Label();
            this.cmb_InterestOption = new System.Windows.Forms.ComboBox();
            this.comboPaymentMethod = new System.Windows.Forms.ComboBox();
            this.lblStatement = new System.Windows.Forms.Label();
            this.comboStatementDates = new System.Windows.Forms.ComboBox();
            this.btnPrintStatement = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.txtAccountStatus = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btn_calc = new System.Windows.Forms.Button();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtMinPayment);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.txtPendingInterest);
            this.groupBox4.Controls.Add(this.dtp_StatementDate);
            this.groupBox4.Controls.Add(this.dtNotePrinted);
            this.groupBox4.Controls.Add(this.lblNotePrinted);
            this.groupBox4.Controls.Add(this.txtStoreCardAvailable);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.txtStoreCardLimit);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.dateNotePrinted);
            this.groupBox4.Controls.Add(this.lblDateNotePrinted);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.txtBalance);
            this.groupBox4.Controls.Add(this.lblInterestRate);
            this.groupBox4.Controls.Add(this.lblDueDate);
            this.groupBox4.Controls.Add(this.dtp_DatePaymentDue);
            this.groupBox4.Controls.Add(this.lblBalance);
            this.groupBox4.Controls.Add(this.txtInterestRate);
            this.groupBox4.Controls.Add(this.txtArrears);
            this.groupBox4.Controls.Add(this.lblArrears);
            this.groupBox4.Controls.Add(this.txtNotPrinted);
            this.groupBox4.Location = new System.Drawing.Point(425, 14);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(276, 350);
            this.groupBox4.TabIndex = 61;
            this.groupBox4.TabStop = false;
            // 
            // txtMinPayment
            // 
            this.txtMinPayment.Location = new System.Drawing.Point(134, 85);
            this.txtMinPayment.Name = "txtMinPayment";
            this.txtMinPayment.ReadOnly = true;
            this.txtMinPayment.Size = new System.Drawing.Size(100, 20);
            this.txtMinPayment.TabIndex = 74;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(49, 88);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 13);
            this.label9.TabIndex = 75;
            this.label9.Text = "Min Payment";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(33, 288);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(84, 13);
            this.label8.TabIndex = 73;
            this.label8.Text = "Pending Interest";
            this.toolTip1.SetToolTip(this.label8, "Interest will be charged if full balance not paid off by Due Date");
            // 
            // txtPendingInterest
            // 
            this.txtPendingInterest.Location = new System.Drawing.Point(134, 285);
            this.txtPendingInterest.Name = "txtPendingInterest";
            this.txtPendingInterest.ReadOnly = true;
            this.txtPendingInterest.Size = new System.Drawing.Size(121, 20);
            this.txtPendingInterest.TabIndex = 72;
            this.toolTip1.SetToolTip(this.txtPendingInterest, "Interest will be charged if full balance not paid off by Due Date");
            // 
            // dtp_StatementDate
            // 
            this.dtp_StatementDate.Enabled = false;
            this.dtp_StatementDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtp_StatementDate.Location = new System.Drawing.Point(134, 219);
            this.dtp_StatementDate.Name = "dtp_StatementDate";
            this.dtp_StatementDate.Size = new System.Drawing.Size(121, 20);
            this.dtp_StatementDate.TabIndex = 71;
            // 
            // dtNotePrinted
            // 
            this.dtNotePrinted.Enabled = false;
            this.dtNotePrinted.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtNotePrinted.Location = new System.Drawing.Point(134, 318);
            this.dtNotePrinted.Name = "dtNotePrinted";
            this.dtNotePrinted.Size = new System.Drawing.Size(121, 20);
            this.dtNotePrinted.TabIndex = 70;
            // 
            // lblNotePrinted
            // 
            this.lblNotePrinted.AutoSize = true;
            this.lblNotePrinted.Location = new System.Drawing.Point(51, 321);
            this.lblNotePrinted.Name = "lblNotePrinted";
            this.lblNotePrinted.Size = new System.Drawing.Size(66, 13);
            this.lblNotePrinted.TabIndex = 68;
            this.lblNotePrinted.Text = "Note Printed";
            // 
            // txtStoreCardAvailable
            // 
            this.txtStoreCardAvailable.Location = new System.Drawing.Point(134, 120);
            this.txtStoreCardAvailable.Name = "txtStoreCardAvailable";
            this.txtStoreCardAvailable.ReadOnly = true;
            this.txtStoreCardAvailable.Size = new System.Drawing.Size(100, 20);
            this.txtStoreCardAvailable.TabIndex = 65;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(67, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 66;
            this.label3.Text = "Available";
            // 
            // txtStoreCardLimit
            // 
            this.txtStoreCardLimit.Location = new System.Drawing.Point(134, 19);
            this.txtStoreCardLimit.Name = "txtStoreCardLimit";
            this.txtStoreCardLimit.Size = new System.Drawing.Size(100, 20);
            this.txtStoreCardLimit.TabIndex = 63;
            this.txtStoreCardLimit.TextChanged += new System.EventHandler(this.txtStoreCardLimit_TextChanged);
            this.txtStoreCardLimit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtStoreCardLimit_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(89, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 64;
            this.label2.Text = "Limit";
            // 
            // dateNotePrinted
            // 
            this.dateNotePrinted.Enabled = false;
            this.dateNotePrinted.Location = new System.Drawing.Point(104, 374);
            this.dateNotePrinted.Name = "dateNotePrinted";
            this.dateNotePrinted.Size = new System.Drawing.Size(121, 20);
            this.dateNotePrinted.TabIndex = 62;
            this.dateNotePrinted.Visible = false;
            // 
            // lblDateNotePrinted
            // 
            this.lblDateNotePrinted.AutoSize = true;
            this.lblDateNotePrinted.Location = new System.Drawing.Point(5, 377);
            this.lblDateNotePrinted.Name = "lblDateNotePrinted";
            this.lblDateNotePrinted.Size = new System.Drawing.Size(92, 13);
            this.lblDateNotePrinted.TabIndex = 61;
            this.lblDateNotePrinted.Text = "Date Note Printed";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 222);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 13);
            this.label5.TabIndex = 59;
            this.label5.Text = "Next Statement Date";
            // 
            // txtBalance
            // 
            this.txtBalance.Location = new System.Drawing.Point(134, 52);
            this.txtBalance.Name = "txtBalance";
            this.txtBalance.ReadOnly = true;
            this.txtBalance.Size = new System.Drawing.Size(100, 20);
            this.txtBalance.TabIndex = 3;
            // 
            // lblInterestRate
            // 
            this.lblInterestRate.AutoSize = true;
            this.lblInterestRate.Location = new System.Drawing.Point(49, 189);
            this.lblInterestRate.Name = "lblInterestRate";
            this.lblInterestRate.Size = new System.Drawing.Size(68, 13);
            this.lblInterestRate.TabIndex = 28;
            this.lblInterestRate.Text = "Interest Rate";
            // 
            // lblDueDate
            // 
            this.lblDueDate.AutoSize = true;
            this.lblDueDate.Location = new System.Drawing.Point(20, 255);
            this.lblDueDate.Name = "lblDueDate";
            this.lblDueDate.Size = new System.Drawing.Size(97, 13);
            this.lblDueDate.TabIndex = 58;
            this.lblDueDate.Text = "Payment Due Date";
            // 
            // dtp_DatePaymentDue
            // 
            this.dtp_DatePaymentDue.Enabled = false;
            this.dtp_DatePaymentDue.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtp_DatePaymentDue.Location = new System.Drawing.Point(134, 252);
            this.dtp_DatePaymentDue.Name = "dtp_DatePaymentDue";
            this.dtp_DatePaymentDue.Size = new System.Drawing.Size(121, 20);
            this.dtp_DatePaymentDue.TabIndex = 57;
            this.dtp_DatePaymentDue.ValueChanged += new System.EventHandler(this.datepaymentdue_ValueChanged);
            // 
            // lblBalance
            // 
            this.lblBalance.AutoSize = true;
            this.lblBalance.Location = new System.Drawing.Point(71, 55);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(46, 13);
            this.lblBalance.TabIndex = 7;
            this.lblBalance.Text = "Balance";
            // 
            // txtInterestRate
            // 
            this.txtInterestRate.Location = new System.Drawing.Point(134, 186);
            this.txtInterestRate.Name = "txtInterestRate";
            this.txtInterestRate.ReadOnly = true;
            this.txtInterestRate.Size = new System.Drawing.Size(100, 20);
            this.txtInterestRate.TabIndex = 27;
            // 
            // txtArrears
            // 
            this.txtArrears.Location = new System.Drawing.Point(134, 153);
            this.txtArrears.Name = "txtArrears";
            this.txtArrears.ReadOnly = true;
            this.txtArrears.Size = new System.Drawing.Size(100, 20);
            this.txtArrears.TabIndex = 19;
            // 
            // lblArrears
            // 
            this.lblArrears.AutoSize = true;
            this.lblArrears.Location = new System.Drawing.Point(77, 156);
            this.lblArrears.Name = "lblArrears";
            this.lblArrears.Size = new System.Drawing.Size(40, 13);
            this.lblArrears.TabIndex = 20;
            this.lblArrears.Text = "Arrears";
            // 
            // txtNotPrinted
            // 
            this.txtNotPrinted.Location = new System.Drawing.Point(104, 374);
            this.txtNotPrinted.Name = "txtNotPrinted";
            this.txtNotPrinted.ReadOnly = true;
            this.txtNotPrinted.Size = new System.Drawing.Size(121, 20);
            this.txtNotPrinted.TabIndex = 62;
            this.txtNotPrinted.Text = "Not Printed";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkFixed);
            this.groupBox3.Controls.Add(this.ComboContactMethod);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.comboStatements);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.txtPaymentMethod);
            this.groupBox3.Controls.Add(this.cmb_InterestOption);
            this.groupBox3.Controls.Add(this.comboPaymentMethod);
            this.groupBox3.Location = new System.Drawing.Point(19, 14);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(400, 193);
            this.groupBox3.TabIndex = 60;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Payment Details";
            // 
            // chkFixed
            // 
            this.chkFixed.AutoSize = true;
            this.chkFixed.Enabled = false;
            this.chkFixed.Location = new System.Drawing.Point(288, 56);
            this.chkFixed.Name = "chkFixed";
            this.chkFixed.Size = new System.Drawing.Size(89, 17);
            this.chkFixed.TabIndex = 54;
            this.chkFixed.Text = "Fixed Interest";
            this.chkFixed.UseVisualStyleBackColor = true;
            // 
            // ComboContactMethod
            // 
            this.ComboContactMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboContactMethod.FormattingEnabled = true;
            this.ComboContactMethod.Location = new System.Drawing.Point(97, 139);
            this.ComboContactMethod.Name = "ComboContactMethod";
            this.ComboContactMethod.Size = new System.Drawing.Size(185, 21);
            this.ComboContactMethod.TabIndex = 53;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 147);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 13);
            this.label7.TabIndex = 52;
            this.label7.Text = "Contact Method";
            // 
            // comboStatements
            // 
            this.comboStatements.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStatements.FormattingEnabled = true;
            this.comboStatements.Location = new System.Drawing.Point(97, 98);
            this.comboStatements.Name = "comboStatements";
            this.comboStatements.Size = new System.Drawing.Size(185, 21);
            this.comboStatements.TabIndex = 51;
            this.comboStatements.SelectedIndexChanged += new System.EventHandler(this.comboStatements_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 50;
            this.label4.Text = "Statements";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 44;
            this.label1.Text = "Interest Option";
            // 
            // txtPaymentMethod
            // 
            this.txtPaymentMethod.AutoSize = true;
            this.txtPaymentMethod.Location = new System.Drawing.Point(13, 213);
            this.txtPaymentMethod.Name = "txtPaymentMethod";
            this.txtPaymentMethod.Size = new System.Drawing.Size(87, 13);
            this.txtPaymentMethod.TabIndex = 45;
            this.txtPaymentMethod.Text = "Payment Method";
            this.txtPaymentMethod.Visible = false;
            // 
            // cmb_InterestOption
            // 
            this.cmb_InterestOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_InterestOption.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cmb_InterestOption.FormattingEnabled = true;
            this.cmb_InterestOption.Location = new System.Drawing.Point(97, 54);
            this.cmb_InterestOption.Name = "cmb_InterestOption";
            this.cmb_InterestOption.Size = new System.Drawing.Size(185, 21);
            this.cmb_InterestOption.TabIndex = 43;
            this.cmb_InterestOption.SelectedIndexChanged += new System.EventHandler(this.comboInterestOption_SelectedIndexChanged);
            // 
            // comboPaymentMethod
            // 
            this.comboPaymentMethod.FormattingEnabled = true;
            this.comboPaymentMethod.Location = new System.Drawing.Point(104, 210);
            this.comboPaymentMethod.Name = "comboPaymentMethod";
            this.comboPaymentMethod.Size = new System.Drawing.Size(185, 21);
            this.comboPaymentMethod.TabIndex = 46;
            this.comboPaymentMethod.Visible = false;
            // 
            // lblStatement
            // 
            this.lblStatement.AutoSize = true;
            this.lblStatement.Location = new System.Drawing.Point(20, 238);
            this.lblStatement.Name = "lblStatement";
            this.lblStatement.Size = new System.Drawing.Size(90, 13);
            this.lblStatement.TabIndex = 65;
            this.lblStatement.Text = "Statement History";
            // 
            // comboStatementDates
            // 
            this.comboStatementDates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStatementDates.FormattingEnabled = true;
            this.comboStatementDates.Location = new System.Drawing.Point(116, 237);
            this.comboStatementDates.Name = "comboStatementDates";
            this.comboStatementDates.Size = new System.Drawing.Size(185, 21);
            this.comboStatementDates.TabIndex = 64;
            // 
            // btnPrintStatement
            // 
            this.btnPrintStatement.Location = new System.Drawing.Point(314, 235);
            this.btnPrintStatement.Name = "btnPrintStatement";
            this.btnPrintStatement.Size = new System.Drawing.Size(105, 23);
            this.btnPrintStatement.TabIndex = 63;
            this.btnPrintStatement.Text = "Reprint Statement";
            this.btnPrintStatement.UseVisualStyleBackColor = true;
            this.btnPrintStatement.Click += new System.EventHandler(this.btnPrintStatement_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(720, 25);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.TabIndex = 82;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(26, 305);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 83;
            this.label6.Text = "Account Status";
            // 
            // txtAccountStatus
            // 
            this.txtAccountStatus.Location = new System.Drawing.Point(116, 302);
            this.txtAccountStatus.Name = "txtAccountStatus";
            this.txtAccountStatus.ReadOnly = true;
            this.txtAccountStatus.Size = new System.Drawing.Size(185, 20);
            this.txtAccountStatus.TabIndex = 84;
            // 
            // btn_calc
            // 
            this.btn_calc.Image = global::STL.PL.Properties.Resources.Calc;
            this.btn_calc.Location = new System.Drawing.Point(707, 63);
            this.btn_calc.Name = "btn_calc";
            this.btn_calc.Size = new System.Drawing.Size(37, 29);
            this.btn_calc.TabIndex = 85;
            this.btn_calc.UseVisualStyleBackColor = true;
            this.btn_calc.Click += new System.EventHandler(this.btn_calc_Click);
            // 
            // StoreCardPaymentDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_calc);
            this.Controls.Add(this.txtAccountStatus);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblStatement);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.comboStatementDates);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.btnPrintStatement);
            this.Controls.Add(this.groupBox3);
            this.Name = "StoreCardPaymentDetails";
            this.Size = new System.Drawing.Size(755, 392);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.DateTimePicker dateNotePrinted;
        private System.Windows.Forms.Label lblDateNotePrinted;
        private System.Windows.Forms.Label label5;
        //private System.Windows.Forms.DateTimePicker datepaymentdueStatementDate;
        private System.Windows.Forms.TextBox txtBalance;
        private System.Windows.Forms.Label lblInterestRate;
        private System.Windows.Forms.Label lblDueDate;
        private System.Windows.Forms.DateTimePicker dtp_DatePaymentDue;
        private System.Windows.Forms.Label lblBalance;
        private System.Windows.Forms.TextBox txtInterestRate;
        private System.Windows.Forms.TextBox txtArrears;
        private System.Windows.Forms.Label lblArrears;
        private System.Windows.Forms.TextBox txtNotPrinted;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox ComboContactMethod;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboStatements;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label txtPaymentMethod;
        private System.Windows.Forms.ComboBox cmb_InterestOption;
        private System.Windows.Forms.ComboBox comboPaymentMethod;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtStoreCardAvailable;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtStoreCardLimit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnPrintStatement;
        private System.Windows.Forms.Label lblStatement;
        private System.Windows.Forms.ComboBox comboStatementDates;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label lblNotePrinted;
        private System.Windows.Forms.DateTimePicker dtNotePrinted;
        private System.Windows.Forms.CheckBox chkFixed;
        private System.Windows.Forms.TextBox txtAccountStatus;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dtp_StatementDate;
        private System.Windows.Forms.Button btn_calc;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtPendingInterest;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox txtMinPayment;
        private System.Windows.Forms.Label label9;
    }
}
