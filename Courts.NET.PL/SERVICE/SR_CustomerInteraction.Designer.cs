namespace STL.PL.SERVICE
{
    partial class SR_CustomerInteraction
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SR_CustomerInteraction));
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnCustSearch = new System.Windows.Forms.Button();
            this.txtCustId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgCustomerInteractionLog = new System.Windows.Forms.DataGridView();
            this.grpEntry = new System.Windows.Forms.GroupBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.drpAccountNo = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.drpServiceRequestNo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnAddInteraction = new System.Windows.Forms.Button();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.dtInteraction = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.drpInteractionType = new System.Windows.Forms.ComboBox();
            this.txtInteractionDesc = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chxIncludeAssocAccounts = new System.Windows.Forms.CheckBox();
            this.chxIncludeSettled = new System.Windows.Forms.CheckBox();
            this.dgAccounts = new System.Windows.Forms.DataGrid();
            this.dgAccountsTableStyle = new System.Windows.Forms.DataGridTableStyle();
            this.SRNo = new System.Windows.Forms.DataGridTextBoxColumn();
            this.AccountNo = new System.Windows.Forms.DataGridTextBoxColumn();
            this.DateOpened = new System.Windows.Forms.DataGridTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridTextBoxColumn();
            this.AgreementTotal = new System.Windows.Forms.DataGridTextBoxColumn();
            this.Balance = new System.Windows.Forms.DataGridTextBoxColumn();
            this.Arrears = new System.Windows.Forms.DataGridTextBoxColumn();
            this.HolderJoint = new System.Windows.Forms.DataGridTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridTextBoxColumn();
            this.Strategy = new System.Windows.Forms.DataGridTextBoxColumn();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuHelp = new Crownwood.Magic.Menus.MenuCommand();
            this.mnuLaunchHelp = new Crownwood.Magic.Menus.MenuCommand();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.grpSearch.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgCustomerInteractionLog)).BeginInit();
            this.grpEntry.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // grpSearch
            // 
            this.grpSearch.BackColor = System.Drawing.SystemColors.Control;
            this.grpSearch.Controls.Add(this.txtCustomerName);
            this.grpSearch.Controls.Add(this.label3);
            this.grpSearch.Controls.Add(this.btnSave);
            this.grpSearch.Controls.Add(this.btnClear);
            this.grpSearch.Controls.Add(this.btnCustSearch);
            this.grpSearch.Controls.Add(this.txtCustId);
            this.grpSearch.Controls.Add(this.label5);
            this.grpSearch.Location = new System.Drawing.Point(8, 0);
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.Size = new System.Drawing.Size(776, 52);
            this.grpSearch.TabIndex = 6;
            this.grpSearch.TabStop = false;
            this.grpSearch.Text = "Customer Search";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Location = new System.Drawing.Point(343, 20);
            this.txtCustomerName.MaxLength = 30;
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(248, 20);
            this.txtCustomerName.TabIndex = 127;
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(285, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 19);
            this.label3.TabIndex = 128;
            this.label3.Text = "Name";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(622, 19);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(48, 23);
            this.btnSave.TabIndex = 126;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(700, 19);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(48, 23);
            this.btnClear.TabIndex = 125;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnCustSearch
            // 
            this.btnCustSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnCustSearch.Image")));
            this.btnCustSearch.Location = new System.Drawing.Point(247, 15);
            this.btnCustSearch.Name = "btnCustSearch";
            this.btnCustSearch.Size = new System.Drawing.Size(32, 30);
            this.btnCustSearch.TabIndex = 54;
            this.btnCustSearch.Click += new System.EventHandler(this.btnCustSearch_Click);
            // 
            // txtCustId
            // 
            this.txtCustId.Location = new System.Drawing.Point(104, 20);
            this.txtCustId.MaxLength = 30;
            this.txtCustId.Name = "txtCustId";
            this.txtCustId.Size = new System.Drawing.Size(137, 20);
            this.txtCustId.TabIndex = 29;
            // 
            // label5
            // 
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(25, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 19);
            this.label5.TabIndex = 30;
            this.label5.Text = "Customer Id";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.dgCustomerInteractionLog);
            this.groupBox1.Location = new System.Drawing.Point(8, 52);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 128);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Customer Interaction Log";
            // 
            // dgCustomerInteractionLog
            // 
            this.dgCustomerInteractionLog.AllowUserToAddRows = false;
            this.dgCustomerInteractionLog.AllowUserToDeleteRows = false;
            this.dgCustomerInteractionLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgCustomerInteractionLog.Location = new System.Drawing.Point(8, 19);
            this.dgCustomerInteractionLog.MultiSelect = false;
            this.dgCustomerInteractionLog.Name = "dgCustomerInteractionLog";
            this.dgCustomerInteractionLog.ReadOnly = true;
            this.dgCustomerInteractionLog.Size = new System.Drawing.Size(760, 103);
            this.dgCustomerInteractionLog.TabIndex = 0;
            this.dgCustomerInteractionLog.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgCustomerInteractionLog_RowHeaderMouseClick);
            this.dgCustomerInteractionLog.SelectionChanged += new System.EventHandler(this.dgCustomerInteractionLog_SelectionChanged);
            // 
            // grpEntry
            // 
            this.grpEntry.BackColor = System.Drawing.SystemColors.Control;
            this.grpEntry.Controls.Add(this.btnNew);
            this.grpEntry.Controls.Add(this.drpAccountNo);
            this.grpEntry.Controls.Add(this.label6);
            this.grpEntry.Controls.Add(this.drpServiceRequestNo);
            this.grpEntry.Controls.Add(this.label4);
            this.grpEntry.Controls.Add(this.btnAddInteraction);
            this.grpEntry.Controls.Add(this.txtComments);
            this.grpEntry.Controls.Add(this.dtInteraction);
            this.grpEntry.Controls.Add(this.label2);
            this.grpEntry.Controls.Add(this.drpInteractionType);
            this.grpEntry.Controls.Add(this.txtInteractionDesc);
            this.grpEntry.Controls.Add(this.label1);
            this.grpEntry.Enabled = false;
            this.grpEntry.Location = new System.Drawing.Point(8, 180);
            this.grpEntry.Name = "grpEntry";
            this.grpEntry.Size = new System.Drawing.Size(776, 139);
            this.grpEntry.TabIndex = 8;
            this.grpEntry.TabStop = false;
            this.grpEntry.Text = "New Customer Interaction";
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(721, 15);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(48, 23);
            this.btnNew.TabIndex = 132;
            this.btnNew.Text = "New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // drpAccountNo
            // 
            this.drpAccountNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpAccountNo.ItemHeight = 13;
            this.drpAccountNo.Location = new System.Drawing.Point(631, 66);
            this.drpAccountNo.Name = "drpAccountNo";
            this.drpAccountNo.Size = new System.Drawing.Size(137, 21);
            this.drpAccountNo.TabIndex = 129;
            // 
            // label6
            // 
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(628, 90);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(117, 19);
            this.label6.TabIndex = 131;
            this.label6.Text = "Service Request No";
            this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // drpServiceRequestNo
            // 
            this.drpServiceRequestNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpServiceRequestNo.ItemHeight = 13;
            this.drpServiceRequestNo.Location = new System.Drawing.Point(631, 112);
            this.drpServiceRequestNo.Name = "drpServiceRequestNo";
            this.drpServiceRequestNo.Size = new System.Drawing.Size(137, 21);
            this.drpServiceRequestNo.TabIndex = 128;
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(628, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 19);
            this.label4.TabIndex = 130;
            this.label4.Text = "Account No";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // btnAddInteraction
            // 
            this.btnAddInteraction.BackColor = System.Drawing.Color.SlateBlue;
            this.btnAddInteraction.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddInteraction.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnAddInteraction.Image = global::STL.PL.Properties.Resources.plus;
            this.btnAddInteraction.Location = new System.Drawing.Point(747, 16);
            this.btnAddInteraction.Name = "btnAddInteraction";
            this.btnAddInteraction.Size = new System.Drawing.Size(22, 22);
            this.btnAddInteraction.TabIndex = 123;
            this.btnAddInteraction.TabStop = false;
            this.btnAddInteraction.UseVisualStyleBackColor = false;
            this.btnAddInteraction.Click += new System.EventHandler(this.btnAddInteraction_Click);
            // 
            // txtComments
            // 
            this.txtComments.Location = new System.Drawing.Point(8, 44);
            this.txtComments.MaxLength = 400;
            this.txtComments.Multiline = true;
            this.txtComments.Name = "txtComments";
            this.txtComments.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtComments.Size = new System.Drawing.Size(614, 89);
            this.txtComments.TabIndex = 36;
            // 
            // dtInteraction
            // 
            this.dtInteraction.CustomFormat = "ddd dd MMM yyyy";
            this.dtInteraction.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtInteraction.Location = new System.Drawing.Point(38, 18);
            this.dtInteraction.Name = "dtInteraction";
            this.dtInteraction.Size = new System.Drawing.Size(131, 20);
            this.dtInteraction.TabIndex = 34;
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(6, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 19);
            this.label2.TabIndex = 35;
            this.label2.Text = "Date";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // drpInteractionType
            // 
            this.drpInteractionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpInteractionType.ItemHeight = 13;
            this.drpInteractionType.Location = new System.Drawing.Point(244, 17);
            this.drpInteractionType.Name = "drpInteractionType";
            this.drpInteractionType.Size = new System.Drawing.Size(76, 21);
            this.drpInteractionType.TabIndex = 33;
            this.drpInteractionType.SelectedIndexChanged += new System.EventHandler(this.drpInteractionType_SelectedIndexChanged);
            // 
            // txtInteractionDesc
            // 
            this.txtInteractionDesc.Location = new System.Drawing.Point(343, 17);
            this.txtInteractionDesc.MaxLength = 30;
            this.txtInteractionDesc.Name = "txtInteractionDesc";
            this.txtInteractionDesc.ReadOnly = true;
            this.txtInteractionDesc.Size = new System.Drawing.Size(366, 20);
            this.txtInteractionDesc.TabIndex = 31;
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(176, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 19);
            this.label1.TabIndex = 32;
            this.label1.Text = "Interaction";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox3.Controls.Add(this.chxIncludeAssocAccounts);
            this.groupBox3.Controls.Add(this.chxIncludeSettled);
            this.groupBox3.Controls.Add(this.dgAccounts);
            this.groupBox3.Location = new System.Drawing.Point(8, 319);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(776, 154);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Customer Accounts";
            // 
            // chxIncludeAssocAccounts
            // 
            this.chxIncludeAssocAccounts.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.chxIncludeAssocAccounts.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.chxIncludeAssocAccounts.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chxIncludeAssocAccounts.Location = new System.Drawing.Point(380, 23);
            this.chxIncludeAssocAccounts.Name = "chxIncludeAssocAccounts";
            this.chxIncludeAssocAccounts.Size = new System.Drawing.Size(176, 16);
            this.chxIncludeAssocAccounts.TabIndex = 3;
            this.chxIncludeAssocAccounts.Text = "Include Associated Accounts";
            this.chxIncludeAssocAccounts.UseVisualStyleBackColor = false;
            this.chxIncludeAssocAccounts.CheckedChanged += new System.EventHandler(this.chxIncludeAssocAccounts_CheckedChanged);
            // 
            // chxIncludeSettled
            // 
            this.chxIncludeSettled.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.chxIncludeSettled.CheckState = System.Windows.Forms.CheckState.Unchecked;
            this.chxIncludeSettled.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.chxIncludeSettled.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chxIncludeSettled.Location = new System.Drawing.Point(596, 23);
            this.chxIncludeSettled.Name = "chxIncludeSettled";
            this.chxIncludeSettled.Size = new System.Drawing.Size(152, 16);
            this.chxIncludeSettled.TabIndex = 2;
            this.chxIncludeSettled.Text = "Include Settled Accounts";
            this.chxIncludeSettled.UseVisualStyleBackColor = false;
            this.chxIncludeSettled.CheckedChanged += new System.EventHandler(this.chxIncludeSettled_CheckedChanged);
            // 
            // dgAccounts
            // 
            this.dgAccounts.DataMember = "";
            this.dgAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAccounts.Location = new System.Drawing.Point(8, 19);
            this.dgAccounts.Name = "dgAccounts";
            this.dgAccounts.ReadOnly = true;
            this.dgAccounts.Size = new System.Drawing.Size(760, 128);
            this.dgAccounts.TabIndex = 1;
            this.dgAccounts.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
            this.dgAccountsTableStyle});
            this.dgAccounts.TabStop = false;
            //   this.dgAccounts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgAccounts_MouseUp);
            // 
            // dgAccountsTableStyle
            // 
            this.dgAccountsTableStyle.DataGrid = this.dgAccounts;
            this.dgAccountsTableStyle.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
            this.SRNo,
            this.AccountNo,
            this.DateOpened,
            this.Type,
            this.AgreementTotal,
            this.Balance,
            this.Arrears,
            this.HolderJoint,
            this.Status,
            this.Strategy});
            this.dgAccountsTableStyle.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgAccountsTableStyle.MappingName = "CustomerAccounts";
            // 
            // SRNo
            // 
            this.SRNo.Format = "";
            this.SRNo.FormatInfo = null;
            this.SRNo.HeaderText = "SR No";
            this.SRNo.MappingName = "ServiceRequestNo";
            this.SRNo.Width = 75;
            // 
            // AccountNo
            // 
            this.AccountNo.Format = "";
            this.AccountNo.FormatInfo = null;
            this.AccountNo.HeaderText = "Account No";
            this.AccountNo.MappingName = "acctno";
            this.AccountNo.Width = 75;
            // 
            // DateOpened
            // 
            this.DateOpened.Format = "";
            this.DateOpened.FormatInfo = null;
            this.DateOpened.HeaderText = "Date Opened";
            this.DateOpened.MappingName = "dateacctopen";
            this.DateOpened.Width = 95;
            // 
            // Type
            // 
            this.Type.Format = "";
            this.Type.FormatInfo = null;
            this.Type.HeaderText = "Type";
            this.Type.MappingName = "accttype";
            this.Type.Width = 40;
            // 
            // AgreementTotal
            // 
            this.AgreementTotal.Format = "";
            this.AgreementTotal.FormatInfo = null;
            this.AgreementTotal.HeaderText = "Agreement Total";
            this.AgreementTotal.MappingName = "agrmttotal";
            this.AgreementTotal.Width = 95;
            // 
            // Balance
            // 
            this.Balance.Format = "";
            this.Balance.FormatInfo = null;
            this.Balance.HeaderText = "Balance";
            this.Balance.MappingName = "outstbal";
            this.Balance.Width = 83;
            // 
            // Arrears
            // 
            this.Arrears.Format = "";
            this.Arrears.FormatInfo = null;
            this.Arrears.HeaderText = "Arrears";
            this.Arrears.MappingName = "arrears";
            this.Arrears.Width = 75;
            // 
            // HolderJoint
            // 
            this.HolderJoint.Format = "";
            this.HolderJoint.FormatInfo = null;
            this.HolderJoint.HeaderText = "Holder/Joint";
            this.HolderJoint.MappingName = "hldorjnt";
            this.HolderJoint.Width = 75;
            // 
            // Status
            // 
            this.Status.Format = "";
            this.Status.FormatInfo = null;
            this.Status.HeaderText = "Status";
            this.Status.MappingName = "currstatus";
            this.Status.Width = 40;
            // 
            // Strategy
            // 
            this.Strategy.Format = "";
            this.Strategy.FormatInfo = null;
            this.Strategy.HeaderText = "Strategy";
            this.Strategy.MappingName = "strategy";
            this.Strategy.Width = 50;
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.Description = "MenuItem";
            this.menuHelp.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.mnuLaunchHelp});
            this.menuHelp.Text = "&Help";
            // 
            // mnuLaunchHelp
            // 
            this.mnuLaunchHelp.Description = "MenuItem";
            this.mnuLaunchHelp.Text = "&About this Screen";
            this.mnuLaunchHelp.Click += new System.EventHandler(this.mnuLaunchHelp_Click);
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuExit});
            this.menuFile.Text = "&File";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // SR_CustomerInteraction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.grpEntry);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpSearch);
            this.Name = "SR_CustomerInteraction";
            this.Text = "Service Customer Interaction";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SR_CustomerInteraction_FormClosing);
            this.Load += new System.EventHandler(this.SR_CustomerInteraction_Load);
            this.grpSearch.ResumeLayout(false);
            this.grpSearch.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgCustomerInteractionLog)).EndInit();
            this.grpEntry.ResumeLayout(false);
            this.grpEntry.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAccounts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSearch;
        private System.Windows.Forms.TextBox txtCustId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnCustSearch;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox grpEntry;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGrid dgAccounts;
        private System.Windows.Forms.TextBox txtInteractionDesc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox drpInteractionType;
        private System.Windows.Forms.TextBox txtComments;
        private System.Windows.Forms.DateTimePicker dtInteraction;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chxIncludeAssocAccounts;
        private System.Windows.Forms.CheckBox chxIncludeSettled;
        private System.Windows.Forms.Button btnAddInteraction;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox drpAccountNo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox drpServiceRequestNo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView dgCustomerInteractionLog;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private Crownwood.Magic.Menus.MenuCommand menuHelp;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private Crownwood.Magic.Menus.MenuCommand mnuLaunchHelp;
        private System.Windows.Forms.DataGridTableStyle dgAccountsTableStyle;
        private System.Windows.Forms.DataGridTextBoxColumn SRNo;
        private System.Windows.Forms.DataGridTextBoxColumn AccountNo;
        private System.Windows.Forms.DataGridTextBoxColumn DateOpened;
        private System.Windows.Forms.DataGridTextBoxColumn Type;
        private System.Windows.Forms.DataGridTextBoxColumn AgreementTotal;
        private System.Windows.Forms.DataGridTextBoxColumn Balance;
        private System.Windows.Forms.DataGridTextBoxColumn Arrears;
        private System.Windows.Forms.DataGridTextBoxColumn HolderJoint;
        private System.Windows.Forms.DataGridTextBoxColumn Status;
        private System.Windows.Forms.DataGridTextBoxColumn Strategy;
        private System.Windows.Forms.Button btnNew;
    }
}
