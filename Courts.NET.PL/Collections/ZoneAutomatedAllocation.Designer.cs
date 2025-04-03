using STL.Common.Constants.ColumnNames;

namespace STL.PL.Collections
{
    partial class ZoneAutomatedAllocation
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ZoneAutomatedAllocation));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tpZoneSetup = new System.Windows.Forms.TabPage();
            this.pnlRules = new System.Windows.Forms.Panel();
            this.lblZoneCrirerea = new System.Windows.Forms.Label();
            this.btnSaveAllRule = new System.Windows.Forms.Button();
            this.btnOrClause = new System.Windows.Forms.Button();
            this.btnSaveRule = new System.Windows.Forms.Button();
            this.dgvRules = new System.Windows.Forms.DataGridView();
            this.cmbColumn_columnName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.cmbColumn_Operator = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.txtColumn_Query = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_orClause = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlAddressFields = new System.Windows.Forms.Panel();
            this.lblColumnValue = new System.Windows.Forms.Label();
            this.lblColumns = new System.Windows.Forms.Label();
            this.btnAddRow = new System.Windows.Forms.Button();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.lstAddressFields = new System.Windows.Forms.ListBox();
            this.pnlZone = new System.Windows.Forms.Panel();
            this.lblZoneDescr = new System.Windows.Forms.Label();
            this.lblZoneCode = new System.Windows.Forms.Label();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnApplyAll = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAddZone = new System.Windows.Forms.Button();
            this.btnDeleteZone = new System.Windows.Forms.Button();
            this.txtZoneDescription = new System.Windows.Forms.TextBox();
            this.txtZoneCode = new System.Windows.Forms.TextBox();
            this.cmbZones = new System.Windows.Forms.ComboBox();
            this.tpUnzonedAddress = new System.Windows.Forms.TabPage();
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.dgvUnzonedAddress = new System.Windows.Forms.DataGridView();
            this.txtColumn_CustId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_CusAddr1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_CusAddr2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_CusAddr3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_CusPostCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_HolderJoint = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tpEmployeeAlloc = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSaveAllEmpAlloc = new System.Windows.Forms.Button();
            this.btnSaveEmpAlloc = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvFilter = new System.Windows.Forms.TreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.dgvEmployee = new System.Windows.Forms.DataGridView();
            this.dgvEmployeeHeader = new System.Windows.Forms.Label();
            this.dgvEmpZoneAllocation = new System.Windows.Forms.DataGridView();
            this.chkColumn_IsAllocated = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.txtColumn_ZoneBranch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Bailiffs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Accounts = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_AllocOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkColumn_Reallocate = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.txtColumn_BranchOrZoneNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_EmployeeNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_EmployeeType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvEmpZoneAllocationHeader = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuDeleteZoneCriteria = new Crownwood.Magic.Menus.MenuCommand();
            this.txtColumn_EmployeeDetail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmbColumn_MinAccounts = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.cmbColumn_MaxAccounts = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.cmbColumn_AllocationRank = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.txtColumn_Allocated = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControlMain.SuspendLayout();
            this.tpZoneSetup.SuspendLayout();
            this.pnlRules.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRules)).BeginInit();
            this.pnlAddressFields.SuspendLayout();
            this.pnlZone.SuspendLayout();
            this.tpUnzonedAddress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnzonedAddress)).BeginInit();
            this.tpEmployeeAlloc.SuspendLayout();
            this.panel1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployee)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmpZoneAllocation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tpZoneSetup);
            this.tabControlMain.Controls.Add(this.tpUnzonedAddress);
            this.tabControlMain.Controls.Add(this.tpEmployeeAlloc);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Margin = new System.Windows.Forms.Padding(0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.Padding = new System.Drawing.Point(0, 0);
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(802, 487);
            this.tabControlMain.TabIndex = 49;
            // 
            // tpZoneSetup
            // 
            this.tpZoneSetup.BackColor = System.Drawing.Color.Transparent;
            this.tpZoneSetup.Controls.Add(this.pnlRules);
            this.tpZoneSetup.Controls.Add(this.pnlAddressFields);
            this.tpZoneSetup.Controls.Add(this.pnlZone);
            this.tpZoneSetup.Location = new System.Drawing.Point(4, 22);
            this.tpZoneSetup.Margin = new System.Windows.Forms.Padding(0);
            this.tpZoneSetup.Name = "tpZoneSetup";
            this.tpZoneSetup.Padding = new System.Windows.Forms.Padding(3);
            this.tpZoneSetup.Size = new System.Drawing.Size(794, 461);
            this.tpZoneSetup.TabIndex = 0;
            this.tpZoneSetup.Text = "Zone Setup";
            this.tpZoneSetup.UseVisualStyleBackColor = true;
            // 
            // pnlRules
            // 
            this.pnlRules.BackColor = System.Drawing.SystemColors.Control;
            this.pnlRules.Controls.Add(this.lblZoneCrirerea);
            this.pnlRules.Controls.Add(this.btnSaveAllRule);
            this.pnlRules.Controls.Add(this.btnOrClause);
            this.pnlRules.Controls.Add(this.btnSaveRule);
            this.pnlRules.Controls.Add(this.dgvRules);
            this.pnlRules.Location = new System.Drawing.Point(4, 138);
            this.pnlRules.Name = "pnlRules";
            this.pnlRules.Size = new System.Drawing.Size(763, 280);
            this.pnlRules.TabIndex = 70;
            // 
            // lblZoneCrirerea
            // 
            this.lblZoneCrirerea.AutoSize = true;
            this.lblZoneCrirerea.Location = new System.Drawing.Point(6, 11);
            this.lblZoneCrirerea.Name = "lblZoneCrirerea";
            this.lblZoneCrirerea.Size = new System.Drawing.Size(70, 13);
            this.lblZoneCrirerea.TabIndex = 84;
            this.lblZoneCrirerea.Text = "Zone criterea";
            // 
            // btnSaveAllRule
            // 
            this.btnSaveAllRule.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSaveAllRule.Location = new System.Drawing.Point(456, 57);
            this.btnSaveAllRule.Name = "btnSaveAllRule";
            this.btnSaveAllRule.Size = new System.Drawing.Size(72, 23);
            this.btnSaveAllRule.TabIndex = 83;
            this.btnSaveAllRule.Text = "Save All";
            this.btnSaveAllRule.UseVisualStyleBackColor = true;
            this.btnSaveAllRule.Click += new System.EventHandler(this.btnSaveAllRule_Click);
            // 
            // btnOrClause
            // 
            this.btnOrClause.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.errorProvider1.SetIconAlignment(this.btnOrClause, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
            this.btnOrClause.Location = new System.Drawing.Point(327, 2);
            this.btnOrClause.Name = "btnOrClause";
            this.btnOrClause.Size = new System.Drawing.Size(43, 23);
            this.btnOrClause.TabIndex = 72;
            this.btnOrClause.Text = "OR";
            this.toolTip1.SetToolTip(this.btnOrClause, "To join the rows selected in the grid with OR clause");
            this.btnOrClause.UseVisualStyleBackColor = true;
            this.btnOrClause.Click += new System.EventHandler(this.btnOrClause_Click);
            this.btnOrClause.Leave += new System.EventHandler(this.btnOrClause_Leave);
            // 
            // btnSaveRule
            // 
            this.btnSaveRule.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSaveRule.Location = new System.Drawing.Point(456, 28);
            this.btnSaveRule.Name = "btnSaveRule";
            this.btnSaveRule.Size = new System.Drawing.Size(72, 23);
            this.btnSaveRule.TabIndex = 82;
            this.btnSaveRule.Text = "Save";
            this.btnSaveRule.UseVisualStyleBackColor = true;
            this.btnSaveRule.Click += new System.EventHandler(this.btnSaveRule_Click);
            // 
            // dgvRules
            // 
            this.dgvRules.AllowUserToAddRows = false;
            this.dgvRules.AllowUserToDeleteRows = false;
            this.dgvRules.AllowUserToResizeRows = false;
            this.dgvRules.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvRules.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRules.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cmbColumn_columnName,
            this.cmbColumn_Operator,
            this.txtColumn_Query,
            this.txtColumn_orClause});
            this.dgvRules.Location = new System.Drawing.Point(6, 28);
            this.dgvRules.Name = "dgvRules";
            this.dgvRules.RowHeadersWidth = 24;
            this.dgvRules.ShowEditingIcon = false;
            this.dgvRules.Size = new System.Drawing.Size(429, 222);
            this.dgvRules.TabIndex = 70;
            this.dgvRules.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvRules_CellFormatting);
            this.dgvRules.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvRules_MouseUp);
            // 
            // cmbColumn_columnName
            // 
            this.cmbColumn_columnName.DataPropertyName = "column_name";
            this.cmbColumn_columnName.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.cmbColumn_columnName.DisplayStyleForCurrentCellOnly = true;
            this.cmbColumn_columnName.HeaderText = "Column Name";
            this.cmbColumn_columnName.Name = "cmbColumn_columnName";
            this.cmbColumn_columnName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cmbColumn_columnName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // cmbColumn_Operator
            // 
            this.cmbColumn_Operator.DataPropertyName = "operator";
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbColumn_Operator.DefaultCellStyle = dataGridViewCellStyle1;
            this.cmbColumn_Operator.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.cmbColumn_Operator.DisplayStyleForCurrentCellOnly = true;
            this.cmbColumn_Operator.HeaderText = "Operator";
            this.cmbColumn_Operator.Items.AddRange(new object[] {
            "Like",
            "Not Like"});
            this.cmbColumn_Operator.MaxDropDownItems = 3;
            this.cmbColumn_Operator.Name = "cmbColumn_Operator";
            this.cmbColumn_Operator.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cmbColumn_Operator.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // txtColumn_Query
            // 
            this.txtColumn_Query.DataPropertyName = "Query";
            this.txtColumn_Query.HeaderText = "Value";
            this.txtColumn_Query.Name = "txtColumn_Query";
            // 
            // txtColumn_orClause
            // 
            this.txtColumn_orClause.DataPropertyName = "or_clause";
            this.txtColumn_orClause.HeaderText = "OR";
            this.txtColumn_orClause.Name = "txtColumn_orClause";
            this.txtColumn_orClause.ReadOnly = true;
            // 
            // pnlAddressFields
            // 
            this.pnlAddressFields.BackColor = System.Drawing.SystemColors.Control;
            this.pnlAddressFields.Controls.Add(this.lblColumnValue);
            this.pnlAddressFields.Controls.Add(this.lblColumns);
            this.pnlAddressFields.Controls.Add(this.btnAddRow);
            this.pnlAddressFields.Controls.Add(this.txtValue);
            this.pnlAddressFields.Controls.Add(this.lstAddressFields);
            this.pnlAddressFields.Location = new System.Drawing.Point(352, 4);
            this.pnlAddressFields.Name = "pnlAddressFields";
            this.pnlAddressFields.Size = new System.Drawing.Size(415, 130);
            this.pnlAddressFields.TabIndex = 68;
            // 
            // lblColumnValue
            // 
            this.lblColumnValue.AutoSize = true;
            this.lblColumnValue.Location = new System.Drawing.Point(57, 106);
            this.lblColumnValue.Name = "lblColumnValue";
            this.lblColumnValue.Size = new System.Drawing.Size(71, 13);
            this.lblColumnValue.TabIndex = 76;
            this.lblColumnValue.Text = "Column value";
            // 
            // lblColumns
            // 
            this.lblColumns.AutoSize = true;
            this.lblColumns.Location = new System.Drawing.Point(50, 3);
            this.lblColumns.Name = "lblColumns";
            this.lblColumns.Size = new System.Drawing.Size(78, 13);
            this.lblColumns.TabIndex = 75;
            this.lblColumns.Text = "Column Names";
            // 
            // btnAddRow
            // 
            this.btnAddRow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnAddRow.Image = ((System.Drawing.Image)(resources.GetObject("btnAddRow.Image")));
            this.btnAddRow.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAddRow.Location = new System.Drawing.Point(11, 98);
            this.btnAddRow.Name = "btnAddRow";
            this.btnAddRow.Size = new System.Drawing.Size(40, 24);
            this.btnAddRow.TabIndex = 74;
            this.btnAddRow.Click += new System.EventHandler(this.btnAddRow_Click);
            // 
            // txtValue
            // 
            this.txtValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtValue.Location = new System.Drawing.Point(134, 101);
            this.txtValue.MaxLength = 30;
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(194, 20);
            this.txtValue.TabIndex = 73;
            // 
            // lstAddressFields
            // 
            this.lstAddressFields.FormattingEnabled = true;
            this.lstAddressFields.Location = new System.Drawing.Point(134, 3);
            this.lstAddressFields.Name = "lstAddressFields";
            this.lstAddressFields.Size = new System.Drawing.Size(194, 95);
            this.lstAddressFields.TabIndex = 67;
            // 
            // pnlZone
            // 
            this.pnlZone.BackColor = System.Drawing.SystemColors.Control;
            this.pnlZone.Controls.Add(this.lblZoneDescr);
            this.pnlZone.Controls.Add(this.lblZoneCode);
            this.pnlZone.Controls.Add(this.btnImport);
            this.pnlZone.Controls.Add(this.btnExport);
            this.pnlZone.Controls.Add(this.btnApplyAll);
            this.pnlZone.Controls.Add(this.btnApply);
            this.pnlZone.Controls.Add(this.label1);
            this.pnlZone.Controls.Add(this.btnAddZone);
            this.pnlZone.Controls.Add(this.btnDeleteZone);
            this.pnlZone.Controls.Add(this.txtZoneDescription);
            this.pnlZone.Controls.Add(this.txtZoneCode);
            this.pnlZone.Controls.Add(this.cmbZones);
            this.pnlZone.Location = new System.Drawing.Point(4, 4);
            this.pnlZone.Name = "pnlZone";
            this.pnlZone.Size = new System.Drawing.Size(342, 130);
            this.pnlZone.TabIndex = 67;
            // 
            // lblZoneDescr
            // 
            this.lblZoneDescr.AutoSize = true;
            this.lblZoneDescr.Location = new System.Drawing.Point(14, 79);
            this.lblZoneDescr.Name = "lblZoneDescr";
            this.lblZoneDescr.Size = new System.Drawing.Size(60, 13);
            this.lblZoneDescr.TabIndex = 85;
            this.lblZoneDescr.Text = "Description";
            this.lblZoneDescr.UseMnemonic = false;
            // 
            // lblZoneCode
            // 
            this.lblZoneCode.AutoSize = true;
            this.lblZoneCode.Location = new System.Drawing.Point(14, 55);
            this.lblZoneCode.Name = "lblZoneCode";
            this.lblZoneCode.Size = new System.Drawing.Size(32, 13);
            this.lblZoneCode.TabIndex = 84;
            this.lblZoneCode.Text = "Code";
            // 
            // btnImport
            // 
            this.btnImport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnImport.Location = new System.Drawing.Point(249, 98);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(79, 23);
            this.btnImport.TabIndex = 83;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnExport
            // 
            this.btnExport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnExport.Location = new System.Drawing.Point(249, 74);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(79, 23);
            this.btnExport.TabIndex = 82;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnApplyAll
            // 
            this.btnApplyAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnApplyAll.Location = new System.Drawing.Point(249, 50);
            this.btnApplyAll.Name = "btnApplyAll";
            this.btnApplyAll.Size = new System.Drawing.Size(79, 23);
            this.btnApplyAll.TabIndex = 81;
            this.btnApplyAll.Text = "Apply All";
            this.btnApplyAll.UseVisualStyleBackColor = true;
            this.btnApplyAll.Click += new System.EventHandler(this.btnApplyAll_Click);
            // 
            // btnApply
            // 
            this.btnApply.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnApply.Location = new System.Drawing.Point(249, 26);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(79, 23);
            this.btnApply.TabIndex = 80;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 71;
            this.label1.Text = "Zone";
            // 
            // btnAddZone
            // 
            this.btnAddZone.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnAddZone.Location = new System.Drawing.Point(17, 101);
            this.btnAddZone.Name = "btnAddZone";
            this.btnAddZone.Size = new System.Drawing.Size(50, 23);
            this.btnAddZone.TabIndex = 70;
            this.btnAddZone.Text = "Add";
            this.btnAddZone.UseVisualStyleBackColor = true;
            this.btnAddZone.Click += new System.EventHandler(this.btnAddZone_Click);
            // 
            // btnDeleteZone
            // 
            this.btnDeleteZone.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnDeleteZone.Location = new System.Drawing.Point(84, 101);
            this.btnDeleteZone.Name = "btnDeleteZone";
            this.btnDeleteZone.Size = new System.Drawing.Size(50, 23);
            this.btnDeleteZone.TabIndex = 69;
            this.btnDeleteZone.Text = "Delete";
            this.btnDeleteZone.UseVisualStyleBackColor = true;
            this.btnDeleteZone.Click += new System.EventHandler(this.btnDeleteZone_Click);
            // 
            // txtZoneDescription
            // 
            this.txtZoneDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtZoneDescription.Location = new System.Drawing.Point(111, 73);
            this.txtZoneDescription.MaxLength = 30;
            this.txtZoneDescription.Name = "txtZoneDescription";
            this.txtZoneDescription.Size = new System.Drawing.Size(124, 20);
            this.txtZoneDescription.TabIndex = 68;
            // 
            // txtZoneCode
            // 
            this.txtZoneCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtZoneCode.Location = new System.Drawing.Point(111, 49);
            this.txtZoneCode.MaxLength = 4;
            this.txtZoneCode.Name = "txtZoneCode";
            this.txtZoneCode.Size = new System.Drawing.Size(124, 20);
            this.txtZoneCode.TabIndex = 67;
            // 
            // cmbZones
            // 
            this.cmbZones.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbZones.FormattingEnabled = true;
            this.cmbZones.Location = new System.Drawing.Point(57, 25);
            this.cmbZones.Name = "cmbZones";
            this.cmbZones.Size = new System.Drawing.Size(178, 21);
            this.cmbZones.TabIndex = 66;
            this.cmbZones.SelectedIndexChanged += new System.EventHandler(this.cmbZones_SelectedIndexChanged);
            // 
            // tpUnzonedAddress
            // 
            this.tpUnzonedAddress.BackColor = System.Drawing.Color.Transparent;
            this.tpUnzonedAddress.Controls.Add(this.btnExportExcel);
            this.tpUnzonedAddress.Controls.Add(this.btnLoad);
            this.tpUnzonedAddress.Controls.Add(this.dgvUnzonedAddress);
            this.tpUnzonedAddress.Location = new System.Drawing.Point(4, 22);
            this.tpUnzonedAddress.Margin = new System.Windows.Forms.Padding(0);
            this.tpUnzonedAddress.Name = "tpUnzonedAddress";
            this.tpUnzonedAddress.Size = new System.Drawing.Size(794, 461);
            this.tpUnzonedAddress.TabIndex = 2;
            this.tpUnzonedAddress.Text = "Unzoned Address";
            this.tpUnzonedAddress.UseVisualStyleBackColor = true;
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnExportExcel.Image")));
            this.btnExportExcel.Location = new System.Drawing.Point(4, 6);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(24, 24);
            this.btnExportExcel.TabIndex = 82;
            this.toolTip1.SetToolTip(this.btnExportExcel, "Export to Excel");
            this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnLoad.Location = new System.Drawing.Point(624, 6);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(147, 23);
            this.btnLoad.TabIndex = 81;
            this.btnLoad.Text = "Load Unzoned Addresses";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // dgvUnzonedAddress
            // 
            this.dgvUnzonedAddress.AllowUserToAddRows = false;
            this.dgvUnzonedAddress.AllowUserToDeleteRows = false;
            this.dgvUnzonedAddress.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUnzonedAddress.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.txtColumn_CustId,
            this.txtColumn_CusAddr1,
            this.txtColumn_CusAddr2,
            this.txtColumn_CusAddr3,
            this.txtColumn_CusPostCode,
            this.txtColumn_HolderJoint});
            this.dgvUnzonedAddress.Location = new System.Drawing.Point(4, 33);
            this.dgvUnzonedAddress.Name = "dgvUnzonedAddress";
            this.dgvUnzonedAddress.ReadOnly = true;
            this.dgvUnzonedAddress.RowHeadersWidth = 24;
            this.dgvUnzonedAddress.ShowEditingIcon = false;
            this.dgvUnzonedAddress.Size = new System.Drawing.Size(768, 413);
            this.dgvUnzonedAddress.TabIndex = 2;
            this.dgvUnzonedAddress.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvUnzonedAddress_MouseUp);
            // 
            // txtColumn_CustId
            // 
            this.txtColumn_CustId.DataPropertyName = "custid";
            this.txtColumn_CustId.HeaderText = "Customer ID";
            this.txtColumn_CustId.Name = "txtColumn_CustId";
            this.txtColumn_CustId.ReadOnly = true;
            this.txtColumn_CustId.Width = 110;
            // 
            // txtColumn_CusAddr1
            // 
            this.txtColumn_CusAddr1.DataPropertyName = "cusaddr1";
            this.txtColumn_CusAddr1.HeaderText = "Address 1";
            this.txtColumn_CusAddr1.Name = "txtColumn_CusAddr1";
            this.txtColumn_CusAddr1.ReadOnly = true;
            this.txtColumn_CusAddr1.Width = 172;
            // 
            // txtColumn_CusAddr2
            // 
            this.txtColumn_CusAddr2.DataPropertyName = "cusaddr2";
            this.txtColumn_CusAddr2.HeaderText = "Address 2";
            this.txtColumn_CusAddr2.Name = "txtColumn_CusAddr2";
            this.txtColumn_CusAddr2.ReadOnly = true;
            this.txtColumn_CusAddr2.Width = 170;
            // 
            // txtColumn_CusAddr3
            // 
            this.txtColumn_CusAddr3.DataPropertyName = "cusaddr3";
            this.txtColumn_CusAddr3.HeaderText = "Address 3";
            this.txtColumn_CusAddr3.Name = "txtColumn_CusAddr3";
            this.txtColumn_CusAddr3.ReadOnly = true;
            this.txtColumn_CusAddr3.Width = 170;
            // 
            // txtColumn_CusPostCode
            // 
            this.txtColumn_CusPostCode.DataPropertyName = "cuspocode";
            this.txtColumn_CusPostCode.HeaderText = "Post Code";
            this.txtColumn_CusPostCode.Name = "txtColumn_CusPostCode";
            this.txtColumn_CusPostCode.ReadOnly = true;
            this.txtColumn_CusPostCode.Width = 120;
            // 
            // txtColumn_HolderJoint
            // 
            this.txtColumn_HolderJoint.DataPropertyName = "hldorjnt";
            this.txtColumn_HolderJoint.HeaderText = "HolderJoint";
            this.txtColumn_HolderJoint.Name = "txtColumn_HolderJoint";
            this.txtColumn_HolderJoint.ReadOnly = true;
            this.txtColumn_HolderJoint.Visible = false;
            // 
            // tpEmployeeAlloc
            // 
            this.tpEmployeeAlloc.BackColor = System.Drawing.Color.Transparent;
            this.tpEmployeeAlloc.Controls.Add(this.panel1);
            this.tpEmployeeAlloc.Controls.Add(this.splitContainer1);
            this.tpEmployeeAlloc.Location = new System.Drawing.Point(4, 22);
            this.tpEmployeeAlloc.Margin = new System.Windows.Forms.Padding(0);
            this.tpEmployeeAlloc.Name = "tpEmployeeAlloc";
            this.tpEmployeeAlloc.Size = new System.Drawing.Size(794, 461);
            this.tpEmployeeAlloc.TabIndex = 1;
            this.tpEmployeeAlloc.Text = "Employee Allocation";
            this.tpEmployeeAlloc.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSaveAllEmpAlloc);
            this.panel1.Controls.Add(this.btnSaveEmpAlloc);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(794, 21);
            this.panel1.TabIndex = 79;
            // 
            // btnSaveAllEmpAlloc
            // 
            this.btnSaveAllEmpAlloc.Location = new System.Drawing.Point(703, -1);
            this.btnSaveAllEmpAlloc.Name = "btnSaveAllEmpAlloc";
            this.btnSaveAllEmpAlloc.Size = new System.Drawing.Size(81, 23);
            this.btnSaveAllEmpAlloc.TabIndex = 2;
            this.btnSaveAllEmpAlloc.Text = "Save All";
            this.btnSaveAllEmpAlloc.UseVisualStyleBackColor = true;
            this.btnSaveAllEmpAlloc.Click += new System.EventHandler(this.btnSaveAllEmpAlloc_Click);
            // 
            // btnSaveEmpAlloc
            // 
            this.btnSaveEmpAlloc.Location = new System.Drawing.Point(603, -1);
            this.btnSaveEmpAlloc.Name = "btnSaveEmpAlloc";
            this.btnSaveEmpAlloc.Size = new System.Drawing.Size(81, 23);
            this.btnSaveEmpAlloc.TabIndex = 1;
            this.btnSaveEmpAlloc.Text = "Save";
            this.btnSaveEmpAlloc.UseVisualStyleBackColor = true;
            this.btnSaveEmpAlloc.Click += new System.EventHandler(this.btnSaveEmpAlloc_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvFilter);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(794, 461);
            this.splitContainer1.SplitterDistance = 226;
            this.splitContainer1.TabIndex = 78;
            // 
            // tvFilter
            // 
            this.tvFilter.BackColor = System.Drawing.SystemColors.Menu;
            this.tvFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvFilter.CheckBoxes = true;
            this.tvFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFilter.Location = new System.Drawing.Point(0, 22);
            this.tvFilter.Name = "tvFilter";
            this.tvFilter.Size = new System.Drawing.Size(226, 439);
            this.tvFilter.TabIndex = 76;
            this.tvFilter.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvFilter_AfterCheck);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(226, 22);
            this.label2.TabIndex = 77;
            this.label2.Text = "Employee Filter";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.dgvEmployee);
            this.splitContainer2.Panel1.Controls.Add(this.dgvEmployeeHeader);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dgvEmpZoneAllocation);
            this.splitContainer2.Panel2.Controls.Add(this.dgvEmpZoneAllocationHeader);
            this.splitContainer2.Size = new System.Drawing.Size(564, 461);
            this.splitContainer2.SplitterDistance = 202;
            this.splitContainer2.TabIndex = 0;
            // 
            // dgvEmployee
            // 
            this.dgvEmployee.AllowUserToAddRows = false;
            this.dgvEmployee.AllowUserToDeleteRows = false;
            this.dgvEmployee.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEmployee.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.txtColumn_EmployeeDetail,
            this.cmbColumn_MinAccounts,
            this.cmbColumn_MaxAccounts,
            this.cmbColumn_AllocationRank,
            this.txtColumn_Allocated});
            this.dgvEmployee.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEmployee.Location = new System.Drawing.Point(0, 22);
            this.dgvEmployee.Name = "dgvEmployee";
            this.dgvEmployee.RowHeadersVisible = false;
            this.dgvEmployee.RowHeadersWidth = 24;
            this.dgvEmployee.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployee.ShowEditingIcon = false;
            this.dgvEmployee.Size = new System.Drawing.Size(564, 180);
            this.dgvEmployee.TabIndex = 73;
            this.dgvEmployee.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvEmployee_DataError);
            this.dgvEmployee.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvEmployee_EditingControlShowing);
            this.dgvEmployee.SelectionChanged += new System.EventHandler(this.dgvEmployee_SelectionChanged);
            // 
            // dgvEmployeeHeader
            // 
            this.dgvEmployeeHeader.BackColor = System.Drawing.SystemColors.Control;
            this.dgvEmployeeHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dgvEmployeeHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvEmployeeHeader.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvEmployeeHeader.ForeColor = System.Drawing.Color.Black;
            this.dgvEmployeeHeader.Location = new System.Drawing.Point(0, 0);
            this.dgvEmployeeHeader.Name = "dgvEmployeeHeader";
            this.dgvEmployeeHeader.Size = new System.Drawing.Size(564, 22);
            this.dgvEmployeeHeader.TabIndex = 74;
            this.dgvEmployeeHeader.Text = " Employees";
            this.dgvEmployeeHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgvEmpZoneAllocation
            // 
            this.dgvEmpZoneAllocation.AllowUserToAddRows = false;
            this.dgvEmpZoneAllocation.AllowUserToDeleteRows = false;
            this.dgvEmpZoneAllocation.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvEmpZoneAllocation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEmpZoneAllocation.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chkColumn_IsAllocated,
            this.txtColumn_ZoneBranch,
            this.txtColumn_Type,
            this.Bailiffs,
            this.Accounts,
            this.txtColumn_AllocOrder,
            this.chkColumn_Reallocate,
            this.txtColumn_BranchOrZoneNo,
            this.txtColumn_EmployeeNo,
            this.txtColumn_EmployeeType});
            this.dgvEmpZoneAllocation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEmpZoneAllocation.Location = new System.Drawing.Point(0, 22);
            this.dgvEmpZoneAllocation.MultiSelect = false;
            this.dgvEmpZoneAllocation.Name = "dgvEmpZoneAllocation";
            this.dgvEmpZoneAllocation.RowHeadersVisible = false;
            this.dgvEmpZoneAllocation.RowHeadersWidth = 24;
            this.dgvEmpZoneAllocation.ShowEditingIcon = false;
            this.dgvEmpZoneAllocation.Size = new System.Drawing.Size(564, 233);
            this.dgvEmpZoneAllocation.TabIndex = 3;
            this.dgvEmpZoneAllocation.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvZoneAllocation_CellFormatting);
            this.dgvEmpZoneAllocation.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvEmpZoneAllocation_CellValueChanged);
            this.dgvEmpZoneAllocation.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvEmpZoneAllocation_CurrentCellDirtyStateChanged);
            this.dgvEmpZoneAllocation.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvEmpZoneAllocation_DataError);
            // 
            // chkColumn_IsAllocated
            // 
            this.chkColumn_IsAllocated.DataPropertyName = "IsAllocated";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.NullValue = false;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Silver;
            this.chkColumn_IsAllocated.DefaultCellStyle = dataGridViewCellStyle2;
            this.chkColumn_IsAllocated.FillWeight = 25F;
            this.chkColumn_IsAllocated.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkColumn_IsAllocated.Frozen = true;
            this.chkColumn_IsAllocated.HeaderText = "";
            this.chkColumn_IsAllocated.MinimumWidth = 25;
            this.chkColumn_IsAllocated.Name = "chkColumn_IsAllocated";
            this.chkColumn_IsAllocated.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.chkColumn_IsAllocated.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.chkColumn_IsAllocated.Width = 25;
            // 
            // txtColumn_ZoneBranch
            // 
            this.txtColumn_ZoneBranch.DataPropertyName = "BranchOrZone";
            this.txtColumn_ZoneBranch.HeaderText = "Zone or Branch";
            this.txtColumn_ZoneBranch.Name = "txtColumn_ZoneBranch";
            this.txtColumn_ZoneBranch.ReadOnly = true;
            this.txtColumn_ZoneBranch.Width = 135;
            // 
            // txtColumn_Type
            // 
            this.txtColumn_Type.DataPropertyName = "BranchOrZoneType";
            this.txtColumn_Type.HeaderText = "Type";
            this.txtColumn_Type.Name = "txtColumn_Type";
            this.txtColumn_Type.ReadOnly = true;
            this.txtColumn_Type.Width = 60;
            // 
            // Bailiffs
            // 
            this.Bailiffs.DataPropertyName = "Bailiffs";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Bailiffs.DefaultCellStyle = dataGridViewCellStyle3;
            this.Bailiffs.HeaderText = "Bailiffs";
            this.Bailiffs.Name = "Bailiffs";
            this.Bailiffs.ReadOnly = true;
            this.Bailiffs.Width = 45;
            // 
            // Accounts
            // 
            this.Accounts.DataPropertyName = "Accounts";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Accounts.DefaultCellStyle = dataGridViewCellStyle4;
            this.Accounts.HeaderText = "Accounts";
            this.Accounts.Name = "Accounts";
            this.Accounts.ReadOnly = true;
            this.Accounts.Width = 60;
            // 
            // txtColumn_AllocOrder
            // 
            this.txtColumn_AllocOrder.DataPropertyName = "AllocOrder";
            this.txtColumn_AllocOrder.HeaderText = "Allocation Order";
            this.txtColumn_AllocOrder.Name = "txtColumn_AllocOrder";
            this.txtColumn_AllocOrder.ReadOnly = true;
            this.txtColumn_AllocOrder.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.txtColumn_AllocOrder.Width = 60;
            // 
            // chkColumn_Reallocate
            // 
            this.chkColumn_Reallocate.DataPropertyName = "Reallocate";
            this.chkColumn_Reallocate.HeaderText = "Reallocate";
            this.chkColumn_Reallocate.Name = "chkColumn_Reallocate";
            this.chkColumn_Reallocate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.chkColumn_Reallocate.Width = 75;
            // 
            // txtColumn_BranchOrZoneNo
            // 
            this.txtColumn_BranchOrZoneNo.DataPropertyName = "BranchOrZoneNo";
            this.txtColumn_BranchOrZoneNo.HeaderText = "BranchOrZoneNo";
            this.txtColumn_BranchOrZoneNo.Name = "txtColumn_BranchOrZoneNo";
            this.txtColumn_BranchOrZoneNo.ReadOnly = true;
            this.txtColumn_BranchOrZoneNo.Visible = false;
            // 
            // txtColumn_EmployeeNo
            // 
            this.txtColumn_EmployeeNo.DataPropertyName = "EmpeeNo";
            this.txtColumn_EmployeeNo.HeaderText = "EmployeeNo";
            this.txtColumn_EmployeeNo.Name = "txtColumn_EmployeeNo";
            this.txtColumn_EmployeeNo.ReadOnly = true;
            this.txtColumn_EmployeeNo.Visible = false;
            // 
            // txtColumn_EmployeeType
            // 
            this.txtColumn_EmployeeType.DataPropertyName = "EmpeeType";
            this.txtColumn_EmployeeType.HeaderText = "EmpeeType";
            this.txtColumn_EmployeeType.Name = "txtColumn_EmployeeType";
            this.txtColumn_EmployeeType.ReadOnly = true;
            this.txtColumn_EmployeeType.Visible = false;
            // 
            // dgvEmpZoneAllocationHeader
            // 
            this.dgvEmpZoneAllocationHeader.BackColor = System.Drawing.SystemColors.Control;
            this.dgvEmpZoneAllocationHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dgvEmpZoneAllocationHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvEmpZoneAllocationHeader.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvEmpZoneAllocationHeader.ForeColor = System.Drawing.Color.Black;
            this.dgvEmpZoneAllocationHeader.Location = new System.Drawing.Point(0, 0);
            this.dgvEmpZoneAllocationHeader.Name = "dgvEmpZoneAllocationHeader";
            this.dgvEmpZoneAllocationHeader.Size = new System.Drawing.Size(564, 22);
            this.dgvEmpZoneAllocationHeader.TabIndex = 75;
            this.dgvEmpZoneAllocationHeader.Text = " Zone / Branch Allocation";
            this.dgvEmpZoneAllocationHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // menuDeleteZoneCriteria
            // 
            this.menuDeleteZoneCriteria.Description = "MenuItem";
            this.menuDeleteZoneCriteria.Enabled = false;
            this.menuDeleteZoneCriteria.Text = "Delete";
            this.menuDeleteZoneCriteria.Visible = false;
            this.menuDeleteZoneCriteria.Click += new System.EventHandler(this.menuDeleteZoneCriteria_Click);
            // 
            // txtColumn_EmployeeDetail
            // 
            this.txtColumn_EmployeeDetail.DataPropertyName = "concatDesc";
            this.txtColumn_EmployeeDetail.HeaderText = "Employee";
            this.txtColumn_EmployeeDetail.Name = "txtColumn_EmployeeDetail";
            this.txtColumn_EmployeeDetail.ReadOnly = true;
            this.txtColumn_EmployeeDetail.Width = 170;
            // 
            // cmbColumn_MinAccounts
            // 
            this.cmbColumn_MinAccounts.DataPropertyName = "minAccounts";
            this.cmbColumn_MinAccounts.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.cmbColumn_MinAccounts.HeaderText = "Min. Accounts";
            this.cmbColumn_MinAccounts.MaxDropDownItems = 5;
            this.cmbColumn_MinAccounts.Name = "cmbColumn_MinAccounts";
            this.cmbColumn_MinAccounts.Width = 88;
            // 
            // cmbColumn_MaxAccounts
            // 
            this.cmbColumn_MaxAccounts.DataPropertyName = "maxAccounts";
            this.cmbColumn_MaxAccounts.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.cmbColumn_MaxAccounts.HeaderText = "Max. Accounts";
            this.cmbColumn_MaxAccounts.MaxDropDownItems = 5;
            this.cmbColumn_MaxAccounts.Name = "cmbColumn_MaxAccounts";
            this.cmbColumn_MaxAccounts.Width = 88;
            // 
            // cmbColumn_AllocationRank
            // 
            this.cmbColumn_AllocationRank.DataPropertyName = "allocationRank";
            this.cmbColumn_AllocationRank.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.cmbColumn_AllocationRank.HeaderText = "Allocation Rank";
            this.cmbColumn_AllocationRank.MaxDropDownItems = 5;
            this.cmbColumn_AllocationRank.Name = "cmbColumn_AllocationRank";
            // 
            // txtColumn_Allocated
            // 
            this.txtColumn_Allocated.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.txtColumn_Allocated.DataPropertyName = "allocation";
            this.txtColumn_Allocated.HeaderText = "Accounts Allocated";
            this.txtColumn_Allocated.Name = "txtColumn_Allocated";
            this.txtColumn_Allocated.ReadOnly = true;
            this.txtColumn_Allocated.Width = 76;
            // 
            // ZoneAutomatedAllocation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(802, 487);
            this.ControlBox = false;
            this.Controls.Add(this.tabControlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ZoneAutomatedAllocation";
            this.Text = "Zone Automated Allocation";
            this.Load += new System.EventHandler(this.ZoneAutomatedAllocation_Load);
            this.tabControlMain.ResumeLayout(false);
            this.tpZoneSetup.ResumeLayout(false);
            this.pnlRules.ResumeLayout(false);
            this.pnlRules.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRules)).EndInit();
            this.pnlAddressFields.ResumeLayout(false);
            this.pnlAddressFields.PerformLayout();
            this.pnlZone.ResumeLayout(false);
            this.pnlZone.PerformLayout();
            this.tpUnzonedAddress.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnzonedAddress)).EndInit();
            this.tpEmployeeAlloc.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployee)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmpZoneAllocation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tpZoneSetup;
        private System.Windows.Forms.TabPage tpEmployeeAlloc;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Panel pnlZone;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAddZone;
        private System.Windows.Forms.Button btnDeleteZone;
        private System.Windows.Forms.TextBox txtZoneDescription;
        private System.Windows.Forms.TextBox txtZoneCode;
        private System.Windows.Forms.ComboBox cmbZones;
        private System.Windows.Forms.Panel pnlAddressFields;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.ListBox lstAddressFields;
        private System.Windows.Forms.Button btnAddRow;
        private System.Windows.Forms.Panel pnlRules;
        private System.Windows.Forms.DataGridView dgvRules;
        private System.Windows.Forms.Button btnOrClause;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnApplyAll;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnSaveAllRule;
        private System.Windows.Forms.Button btnSaveRule;
        private System.Windows.Forms.TabPage tpUnzonedAddress;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.DataGridView dgvUnzonedAddress;
        private System.Windows.Forms.Button btnSaveAllEmpAlloc;
        private System.Windows.Forms.Button btnSaveEmpAlloc;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_CustId;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_CusAddr1;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_CusAddr2;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_CusAddr3;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_CusPostCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_HolderJoint;
        public System.Windows.Forms.Button btnExportExcel;
        private System.Windows.Forms.Label lblZoneDescr;
        private System.Windows.Forms.Label lblZoneCode;
        private System.Windows.Forms.Label lblColumnValue;
        private System.Windows.Forms.Label lblColumns;
        private System.Windows.Forms.Label lblZoneCrirerea;
        private Crownwood.Magic.Menus.MenuCommand menuDeleteZoneCriteria;
        private System.Windows.Forms.DataGridViewComboBoxColumn cmbColumn_columnName;
        private System.Windows.Forms.DataGridViewComboBoxColumn cmbColumn_Operator;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_Query;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_orClause;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView tvFilter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView dgvEmployee;
        private System.Windows.Forms.Label dgvEmployeeHeader;
        private System.Windows.Forms.DataGridView dgvEmpZoneAllocation;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chkColumn_IsAllocated;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_ZoneBranch;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Bailiffs;
        private System.Windows.Forms.DataGridViewTextBoxColumn Accounts;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_AllocOrder;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chkColumn_Reallocate;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_BranchOrZoneNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_EmployeeNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_EmployeeType;
        private System.Windows.Forms.Label dgvEmpZoneAllocationHeader;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_EmployeeDetail;
        private System.Windows.Forms.DataGridViewComboBoxColumn cmbColumn_MinAccounts;
        private System.Windows.Forms.DataGridViewComboBoxColumn cmbColumn_MaxAccounts;
        private System.Windows.Forms.DataGridViewComboBoxColumn cmbColumn_AllocationRank;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_Allocated;
    }
}