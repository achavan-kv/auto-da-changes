using Crownwood.Magic.Controls;
namespace STL.PL
{
    partial class StrategyConfiguration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StrategyConfiguration));
            this.newStrategy = new System.Windows.Forms.TextBox();
            this.Strategies = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbManual = new System.Windows.Forms.CheckBox();
            this.chkAllocations = new System.Windows.Forms.CheckBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.saveAs = new System.Windows.Forms.Button();
            this.activate = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.strategyCode = new System.Windows.Forms.TextBox();
            this.CreateNew = new System.Windows.Forms.Button();
            this.tabControlStrategies = new Crownwood.Magic.Controls.TabControl();
            this.tabSortOrder = new Crownwood.Magic.Controls.TabPage();
            this.btnSaveSortOrder = new System.Windows.Forms.Button();
            this.cmbSortAscDesc = new System.Windows.Forms.ComboBox();
            this.cmbSortColumnName = new System.Windows.Forms.ComboBox();
            this.btnAddSortOrder = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.pnlSortOrderDGV = new System.Windows.Forms.Panel();
            this.dgvSortOrder = new System.Windows.Forms.DataGridView();
            this.txtColumn_codedescript = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_SortOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_SortColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmbColumn_AscDesc = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbEmpType = new System.Windows.Forms.ComboBox();
            this.tabPageEntryConditions = new Crownwood.Magic.Controls.TabPage();
            this.lbStrategy = new System.Windows.Forms.Label();
            this.drpStrategy = new System.Windows.Forms.ComboBox();
            this.lblYValue = new System.Windows.Forms.Label();
            this.lblXValue = new System.Windows.Forms.Label();
            this.AddEntryConditions = new System.Windows.Forms.Button();
            this.RemoveEntryConditions = new System.Windows.Forms.Button();
            this.EntryConditionsOr = new System.Windows.Forms.Button();
            this.EntryConditionsValue2 = new System.Windows.Forms.TextBox();
            this.EntryConditionsValue1 = new System.Windows.Forms.TextBox();
            this.EntryConditionsOperators = new System.Windows.Forms.ComboBox();
            this.dataGridChosenEntryConditions = new System.Windows.Forms.DataGridView();
            this.DataGridPossibleEntryConditions = new System.Windows.Forms.DataGridView();
            this.tabPageSteps = new Crownwood.Magic.Controls.TabPage();
            this.btnViewSMS = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.previousStrategies = new System.Windows.Forms.ComboBox();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.RemoveSteps = new System.Windows.Forms.Button();
            this.AddSteps = new System.Windows.Forms.Button();
            this.StepsConditionsActions = new System.Windows.Forms.ListBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.dataGridChosenStepConditions = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.Letters = new System.Windows.Forms.ComboBox();
            this.V2onSteps = new System.Windows.Forms.TextBox();
            this.V1onSteps = new System.Windows.Forms.TextBox();
            this.OperatorsOnSteps = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPageExitConditions = new Crownwood.Magic.Controls.TabPage();
            this.lblYValueExit = new System.Windows.Forms.Label();
            this.lblXValueExit = new System.Windows.Forms.Label();
            this.exitStrategy = new System.Windows.Forms.ComboBox();
            this.RemoveExitConditions = new System.Windows.Forms.Button();
            this.AddExitConditions = new System.Windows.Forms.Button();
            this.ExitConditionsV2 = new System.Windows.Forms.TextBox();
            this.ExitConditionsV1 = new System.Windows.Forms.TextBox();
            this.ExitConditionsOperators = new System.Windows.Forms.ComboBox();
            this.dataGridChosenExitConditions = new System.Windows.Forms.DataGridView();
            this.dataGridPossibleExitConditions = new System.Windows.Forms.DataGridView();
            this.tabPageActions = new Crownwood.Magic.Controls.TabPage();
            this.removeActions = new System.Windows.Forms.Button();
            this.addActions = new System.Windows.Forms.Button();
            this.dgChosenActions = new System.Windows.Forms.DataGridView();
            this.dgPossibleActions = new System.Windows.Forms.DataGridView();
            this.tabWorkList = new Crownwood.Magic.Controls.TabPage();
            this.pnlWorkListDGV = new System.Windows.Forms.Panel();
            this.dgvWorkList = new System.Windows.Forms.DataGridView();
            this.txtColumn_WorkList = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_WorkListDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkColumn_Status = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.txtColumn_Strategy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSaveWorkList = new System.Windows.Forms.Button();
            this.btnAddWorkList = new System.Windows.Forms.Button();
            this.txtWorkListDesc = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtWorkListCode = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.menuDeleteSort = new Crownwood.Magic.Menus.MenuCommand();
            this.menuDeleteWorklist = new Crownwood.Magic.Menus.MenuCommand();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.tabControlStrategies.SuspendLayout();
            this.tabSortOrder.SuspendLayout();
            this.pnlSortOrderDGV.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSortOrder)).BeginInit();
            this.panel1.SuspendLayout();
            this.tabPageEntryConditions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridChosenEntryConditions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridPossibleEntryConditions)).BeginInit();
            this.tabPageSteps.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridChosenStepConditions)).BeginInit();
            this.tabPageExitConditions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridChosenExitConditions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPossibleExitConditions)).BeginInit();
            this.tabPageActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgChosenActions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgPossibleActions)).BeginInit();
            this.tabWorkList.SuspendLayout();
            this.pnlWorkListDGV.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWorkList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // newStrategy
            // 
            this.newStrategy.Location = new System.Drawing.Point(80, 15);
            this.newStrategy.MaxLength = 128;
            this.newStrategy.Name = "newStrategy";
            this.newStrategy.ReadOnly = true;
            this.newStrategy.Size = new System.Drawing.Size(136, 20);
            this.newStrategy.TabIndex = 40;
            // 
            // Strategies
            // 
            this.Strategies.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Strategies.DropDownWidth = 121;
            this.Strategies.ItemHeight = 13;
            this.Strategies.Location = new System.Drawing.Point(315, 13);
            this.Strategies.Name = "Strategies";
            this.Strategies.Size = new System.Drawing.Size(236, 21);
            this.Strategies.TabIndex = 39;
            this.Strategies.SelectedIndexChanged += new System.EventHandler(this.Strategies_SelectedIndexChanged);
            this.Strategies.SelectionChangeCommitted += new System.EventHandler(this.Strategies_SelectionChangeCommitted);
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(6, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 14);
            this.label3.TabIndex = 43;
            this.label3.Text = "New Strategy";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(220, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 17);
            this.label1.TabIndex = 44;
            this.label1.Text = "Existing Strategy";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbManual);
            this.groupBox1.Controls.Add(this.chkAllocations);
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.saveAs);
            this.groupBox1.Controls.Add(this.activate);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.strategyCode);
            this.groupBox1.Controls.Add(this.CreateNew);
            this.groupBox1.Controls.Add(this.Strategies);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.newStrategy);
            this.groupBox1.Location = new System.Drawing.Point(28, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(739, 70);
            this.groupBox1.TabIndex = 45;
            this.groupBox1.TabStop = false;
            // 
            // cbManual
            // 
            this.cbManual.AutoSize = true;
            this.cbManual.Location = new System.Drawing.Point(197, 43);
            this.cbManual.Name = "cbManual";
            this.cbManual.Size = new System.Drawing.Size(90, 17);
            this.cbManual.TabIndex = 67;
            this.cbManual.Text = "Action Driven";
            this.cbManual.UseVisualStyleBackColor = true;
            this.cbManual.CheckedChanged += new System.EventHandler(this.cbManual_CheckedChanged);
            this.cbManual.MouseHover += new System.EventHandler(this.cbManual_MouseHover);
            // 
            // chkAllocations
            // 
            this.chkAllocations.AutoSize = true;
            this.chkAllocations.Location = new System.Drawing.Point(356, 44);
            this.chkAllocations.Name = "chkAllocations";
            this.chkAllocations.Size = new System.Drawing.Size(77, 17);
            this.chkAllocations.TabIndex = 66;
            this.chkAllocations.Text = "Allocations";
            this.chkAllocations.UseVisualStyleBackColor = true;
            this.chkAllocations.CheckedChanged += new System.EventHandler(this.chkAllocations_CheckedChanged);
            this.chkAllocations.MouseHover += new System.EventHandler(this.chkAllocations_MouseHover);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(654, 38);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 65;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSave
            // 
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave.Location = new System.Drawing.Point(700, 11);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(21, 24);
            this.btnSave.TabIndex = 64;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // saveAs
            // 
            this.saveAs.Location = new System.Drawing.Point(654, 12);
            this.saveAs.Name = "saveAs";
            this.saveAs.Size = new System.Drawing.Size(40, 23);
            this.saveAs.TabIndex = 52;
            this.saveAs.Text = "Copy";
            this.saveAs.UseVisualStyleBackColor = true;
            this.saveAs.Click += new System.EventHandler(this.saveAs_Click);
            // 
            // activate
            // 
            this.activate.Location = new System.Drawing.Point(559, 12);
            this.activate.Name = "activate";
            this.activate.Size = new System.Drawing.Size(83, 23);
            this.activate.TabIndex = 47;
            this.activate.Text = "De-activate";
            this.activate.UseVisualStyleBackColor = true;
            this.activate.Click += new System.EventHandler(this.activate_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 13);
            this.label5.TabIndex = 46;
            this.label5.Text = "New Strategy Code";
            // 
            // strategyCode
            // 
            this.strategyCode.Location = new System.Drawing.Point(112, 41);
            this.strategyCode.MaxLength = 5;
            this.strategyCode.Name = "strategyCode";
            this.strategyCode.ReadOnly = true;
            this.strategyCode.Size = new System.Drawing.Size(63, 20);
            this.strategyCode.TabIndex = 45;
            // 
            // CreateNew
            // 
            this.CreateNew.Location = new System.Drawing.Point(559, 38);
            this.CreateNew.Name = "CreateNew";
            this.CreateNew.Size = new System.Drawing.Size(83, 23);
            this.CreateNew.TabIndex = 49;
            this.CreateNew.Text = "Create New";
            this.CreateNew.Click += new System.EventHandler(this.CreateNew_Click);
            // 
            // tabControlStrategies
            // 
            this.tabControlStrategies.IDEPixelArea = true;
            this.tabControlStrategies.Location = new System.Drawing.Point(28, 108);
            this.tabControlStrategies.Name = "tabControlStrategies";
            this.tabControlStrategies.PositionTop = true;
            this.tabControlStrategies.SelectedIndex = 5;
            this.tabControlStrategies.SelectedTab = this.tabSortOrder;
            this.tabControlStrategies.Size = new System.Drawing.Size(739, 356);
            this.tabControlStrategies.TabIndex = 46;
            this.tabControlStrategies.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.tabPageEntryConditions,
            this.tabPageSteps,
            this.tabPageExitConditions,
            this.tabPageActions,
            this.tabWorkList,
            this.tabSortOrder});
            this.tabControlStrategies.SelectionChanged += new System.EventHandler(this.tabControlStrategies_SelectionChanged);
            this.tabControlStrategies.Validating += new System.ComponentModel.CancelEventHandler(this.tabControlStrategies_Validating);
            // 
            // tabSortOrder
            // 
            this.tabSortOrder.Controls.Add(this.btnSaveSortOrder);
            this.tabSortOrder.Controls.Add(this.cmbSortAscDesc);
            this.tabSortOrder.Controls.Add(this.cmbSortColumnName);
            this.tabSortOrder.Controls.Add(this.btnAddSortOrder);
            this.tabSortOrder.Controls.Add(this.label12);
            this.tabSortOrder.Controls.Add(this.pnlSortOrderDGV);
            this.tabSortOrder.Location = new System.Drawing.Point(0, 25);
            this.tabSortOrder.Name = "tabSortOrder";
            this.tabSortOrder.Size = new System.Drawing.Size(739, 331);
            this.tabSortOrder.TabIndex = 5;
            this.tabSortOrder.Title = "Sort Order";
            // 
            // btnSaveSortOrder
            // 
            this.btnSaveSortOrder.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSaveSortOrder.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveSortOrder.Image")));
            this.btnSaveSortOrder.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSaveSortOrder.Location = new System.Drawing.Point(712, 6);
            this.btnSaveSortOrder.Name = "btnSaveSortOrder";
            this.btnSaveSortOrder.Size = new System.Drawing.Size(21, 24);
            this.btnSaveSortOrder.TabIndex = 71;
            this.btnSaveSortOrder.Click += new System.EventHandler(this.btnSaveSortOrder_Click);
            // 
            // cmbSortAscDesc
            // 
            this.cmbSortAscDesc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSortAscDesc.FormattingEnabled = true;
            this.cmbSortAscDesc.Location = new System.Drawing.Point(240, 11);
            this.cmbSortAscDesc.Name = "cmbSortAscDesc";
            this.cmbSortAscDesc.Size = new System.Drawing.Size(102, 23);
            this.cmbSortAscDesc.TabIndex = 70;
            // 
            // cmbSortColumnName
            // 
            this.cmbSortColumnName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSortColumnName.FormattingEnabled = true;
            this.cmbSortColumnName.Location = new System.Drawing.Point(100, 11);
            this.cmbSortColumnName.Name = "cmbSortColumnName";
            this.cmbSortColumnName.Size = new System.Drawing.Size(137, 23);
            this.cmbSortColumnName.TabIndex = 69;
            // 
            // btnAddSortOrder
            // 
            this.btnAddSortOrder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnAddSortOrder.Image = ((System.Drawing.Image)(resources.GetObject("btnAddSortOrder.Image")));
            this.btnAddSortOrder.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAddSortOrder.Location = new System.Drawing.Point(362, 11);
            this.btnAddSortOrder.Name = "btnAddSortOrder";
            this.btnAddSortOrder.Size = new System.Drawing.Size(40, 24);
            this.btnAddSortOrder.TabIndex = 68;
            this.btnAddSortOrder.Click += new System.EventHandler(this.btnAddSortOrder_Click);
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(9, 11);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(87, 20);
            this.label12.TabIndex = 64;
            this.label12.Text = "Column Name";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlSortOrderDGV
            // 
            this.pnlSortOrderDGV.BackColor = System.Drawing.Color.Silver;
            this.pnlSortOrderDGV.Controls.Add(this.dgvSortOrder);
            this.pnlSortOrderDGV.Controls.Add(this.panel1);
            this.pnlSortOrderDGV.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlSortOrderDGV.Location = new System.Drawing.Point(0, 54);
            this.pnlSortOrderDGV.Name = "pnlSortOrderDGV";
            this.pnlSortOrderDGV.Size = new System.Drawing.Size(739, 277);
            this.pnlSortOrderDGV.TabIndex = 58;
            // 
            // dgvSortOrder
            // 
            this.dgvSortOrder.AllowUserToAddRows = false;
            this.dgvSortOrder.AllowUserToDeleteRows = false;
            this.dgvSortOrder.AllowUserToResizeRows = false;
            this.dgvSortOrder.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSortOrder.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.txtColumn_codedescript,
            this.txtColumn_SortOrder,
            this.txtColumn_SortColumnName,
            this.cmbColumn_AscDesc});
            this.dgvSortOrder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSortOrder.Location = new System.Drawing.Point(0, 36);
            this.dgvSortOrder.MultiSelect = false;
            this.dgvSortOrder.Name = "dgvSortOrder";
            this.dgvSortOrder.RowHeadersWidth = 24;
            this.dgvSortOrder.Size = new System.Drawing.Size(739, 241);
            this.dgvSortOrder.TabIndex = 58;
            this.dgvSortOrder.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvSortOrder_RowHeaderMouseDoubleClick);
            this.dgvSortOrder.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvSortOrder_MouseUp);
            // 
            // txtColumn_codedescript
            // 
            this.txtColumn_codedescript.DataPropertyName = "codedescript";
            this.txtColumn_codedescript.HeaderText = "Employee Description";
            this.txtColumn_codedescript.Name = "txtColumn_codedescript";
            // 
            // txtColumn_SortOrder
            // 
            this.txtColumn_SortOrder.DataPropertyName = "SortOrder";
            this.txtColumn_SortOrder.HeaderText = "Sort Sequence";
            this.txtColumn_SortOrder.Name = "txtColumn_SortOrder";
            this.txtColumn_SortOrder.ReadOnly = true;
            this.txtColumn_SortOrder.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.txtColumn_SortOrder.Width = 85;
            // 
            // txtColumn_SortColumnName
            // 
            this.txtColumn_SortColumnName.DataPropertyName = "ColumnName";
            this.txtColumn_SortColumnName.HeaderText = "Sort Column Name";
            this.txtColumn_SortColumnName.Name = "txtColumn_SortColumnName";
            this.txtColumn_SortColumnName.ReadOnly = true;
            this.txtColumn_SortColumnName.Width = 160;
            // 
            // cmbColumn_AscDesc
            // 
            this.cmbColumn_AscDesc.DataPropertyName = "AscDesc";
            this.cmbColumn_AscDesc.HeaderText = "Sort Order";
            this.cmbColumn_AscDesc.Items.AddRange(new object[] {
            "ASC",
            "DESC"});
            this.cmbColumn_AscDesc.MinimumWidth = 50;
            this.cmbColumn_AscDesc.Name = "cmbColumn_AscDesc";
            this.cmbColumn_AscDesc.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cmbColumn_AscDesc.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmbEmpType);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(739, 36);
            this.panel1.TabIndex = 60;
            // 
            // cmbEmpType
            // 
            this.cmbEmpType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmpType.Enabled = false;
            this.cmbEmpType.FormattingEnabled = true;
            this.cmbEmpType.Location = new System.Drawing.Point(6, 7);
            this.cmbEmpType.Name = "cmbEmpType";
            this.cmbEmpType.Size = new System.Drawing.Size(209, 23);
            this.cmbEmpType.TabIndex = 59;
            this.cmbEmpType.SelectedIndexChanged += new System.EventHandler(this.cmbEmpType_SelectedIndexChanged);
            // 
            // tabPageEntryConditions
            // 
            this.tabPageEntryConditions.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageEntryConditions.Controls.Add(this.lbStrategy);
            this.tabPageEntryConditions.Controls.Add(this.drpStrategy);
            this.tabPageEntryConditions.Controls.Add(this.lblYValue);
            this.tabPageEntryConditions.Controls.Add(this.lblXValue);
            this.tabPageEntryConditions.Controls.Add(this.AddEntryConditions);
            this.tabPageEntryConditions.Controls.Add(this.RemoveEntryConditions);
            this.tabPageEntryConditions.Controls.Add(this.EntryConditionsOr);
            this.tabPageEntryConditions.Controls.Add(this.EntryConditionsValue2);
            this.tabPageEntryConditions.Controls.Add(this.EntryConditionsValue1);
            this.tabPageEntryConditions.Controls.Add(this.EntryConditionsOperators);
            this.tabPageEntryConditions.Controls.Add(this.dataGridChosenEntryConditions);
            this.tabPageEntryConditions.Controls.Add(this.DataGridPossibleEntryConditions);
            this.tabPageEntryConditions.Location = new System.Drawing.Point(0, 25);
            this.tabPageEntryConditions.Name = "tabPageEntryConditions";
            this.tabPageEntryConditions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageEntryConditions.Selected = false;
            this.tabPageEntryConditions.Size = new System.Drawing.Size(739, 331);
            this.tabPageEntryConditions.TabIndex = 0;
            this.tabPageEntryConditions.Title = "Entry Conditions";
            this.tabPageEntryConditions.Validating += new System.ComponentModel.CancelEventHandler(this.tabPageEntryConditions_Validating);
            // 
            // lbStrategy
            // 
            this.lbStrategy.AutoSize = true;
            this.lbStrategy.Location = new System.Drawing.Point(516, 161);
            this.lbStrategy.Name = "lbStrategy";
            this.lbStrategy.Size = new System.Drawing.Size(50, 15);
            this.lbStrategy.TabIndex = 69;
            this.lbStrategy.Text = "Strategy";
            // 
            // drpStrategy
            // 
            this.drpStrategy.Enabled = false;
            this.drpStrategy.FormattingEnabled = true;
            this.drpStrategy.Location = new System.Drawing.Point(567, 156);
            this.drpStrategy.Name = "drpStrategy";
            this.drpStrategy.Size = new System.Drawing.Size(162, 23);
            this.drpStrategy.TabIndex = 68;
            // 
            // lblYValue
            // 
            this.lblYValue.AutoSize = true;
            this.lblYValue.Location = new System.Drawing.Point(429, 164);
            this.lblYValue.Name = "lblYValue";
            this.lblYValue.Size = new System.Drawing.Size(46, 15);
            this.lblYValue.TabIndex = 53;
            this.lblYValue.Text = "Y Value";
            // 
            // lblXValue
            // 
            this.lblXValue.AutoSize = true;
            this.lblXValue.Location = new System.Drawing.Point(342, 163);
            this.lblXValue.Name = "lblXValue";
            this.lblXValue.Size = new System.Drawing.Size(46, 15);
            this.lblXValue.TabIndex = 52;
            this.lblXValue.Text = "X Value";
            // 
            // AddEntryConditions
            // 
            this.AddEntryConditions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.AddEntryConditions.Image = ((System.Drawing.Image)(resources.GetObject("AddEntryConditions.Image")));
            this.AddEntryConditions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.AddEntryConditions.Location = new System.Drawing.Point(19, 154);
            this.AddEntryConditions.Name = "AddEntryConditions";
            this.AddEntryConditions.Size = new System.Drawing.Size(40, 24);
            this.AddEntryConditions.TabIndex = 51;
            this.AddEntryConditions.Click += new System.EventHandler(this.AddEntryConditions_Click);
            // 
            // RemoveEntryConditions
            // 
            this.RemoveEntryConditions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.RemoveEntryConditions.Image = ((System.Drawing.Image)(resources.GetObject("RemoveEntryConditions.Image")));
            this.RemoveEntryConditions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.RemoveEntryConditions.Location = new System.Drawing.Point(66, 154);
            this.RemoveEntryConditions.Name = "RemoveEntryConditions";
            this.RemoveEntryConditions.Size = new System.Drawing.Size(40, 24);
            this.RemoveEntryConditions.TabIndex = 50;
            this.RemoveEntryConditions.Click += new System.EventHandler(this.RemoveEntryConditions_Click);
            // 
            // EntryConditionsOr
            // 
            this.EntryConditionsOr.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.EntryConditionsOr.Location = new System.Drawing.Point(134, 154);
            this.EntryConditionsOr.Name = "EntryConditionsOr";
            this.EntryConditionsOr.Size = new System.Drawing.Size(39, 24);
            this.EntryConditionsOr.TabIndex = 43;
            this.EntryConditionsOr.Tag = "";
            this.EntryConditionsOr.Text = "OR";
            this.EntryConditionsOr.Click += new System.EventHandler(this.EntryConditionsOr_Click);
            // 
            // EntryConditionsValue2
            // 
            this.EntryConditionsValue2.Location = new System.Drawing.Point(481, 156);
            this.EntryConditionsValue2.MaxLength = 12;
            this.EntryConditionsValue2.Name = "EntryConditionsValue2";
            this.EntryConditionsValue2.ReadOnly = true;
            this.EntryConditionsValue2.Size = new System.Drawing.Size(26, 23);
            this.EntryConditionsValue2.TabIndex = 42;
            this.EntryConditionsValue2.TextChanged += new System.EventHandler(this.EntryConditionsValue2_TextChanged);
            // 
            // EntryConditionsValue1
            // 
            this.EntryConditionsValue1.Location = new System.Drawing.Point(392, 156);
            this.EntryConditionsValue1.MaxLength = 12;
            this.EntryConditionsValue1.Name = "EntryConditionsValue1";
            this.EntryConditionsValue1.ReadOnly = true;
            this.EntryConditionsValue1.Size = new System.Drawing.Size(26, 23);
            this.EntryConditionsValue1.TabIndex = 41;
            this.EntryConditionsValue1.TextChanged += new System.EventHandler(this.EntryConditionsValue1_TextChanged);
            // 
            // EntryConditionsOperators
            // 
            this.EntryConditionsOperators.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EntryConditionsOperators.DropDownWidth = 121;
            this.EntryConditionsOperators.Enabled = false;
            this.EntryConditionsOperators.ItemHeight = 15;
            this.EntryConditionsOperators.Items.AddRange(new object[] {
            "Select an operator",
            "Greater than >",
            "Less than <",
            "Greater than or equal to >=",
            "Less than or equal to <=",
            "Not equal to <>",
            "Equal to =",
            "Between"});
            this.EntryConditionsOperators.Location = new System.Drawing.Point(188, 156);
            this.EntryConditionsOperators.Name = "EntryConditionsOperators";
            this.EntryConditionsOperators.Size = new System.Drawing.Size(145, 23);
            this.EntryConditionsOperators.TabIndex = 40;
            this.EntryConditionsOperators.SelectedIndexChanged += new System.EventHandler(this.EntryConditionsOperators_SelectedIndexChanged);
            this.EntryConditionsOperators.SelectionChangeCommitted += new System.EventHandler(this.EntryConditionsOperators_SelectionChangeCommitted);
            // 
            // dataGridChosenEntryConditions
            // 
            this.dataGridChosenEntryConditions.AllowUserToAddRows = false;
            this.dataGridChosenEntryConditions.AllowUserToDeleteRows = false;
            this.dataGridChosenEntryConditions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridChosenEntryConditions.Location = new System.Drawing.Point(16, 190);
            this.dataGridChosenEntryConditions.Name = "dataGridChosenEntryConditions";
            this.dataGridChosenEntryConditions.ReadOnly = true;
            this.dataGridChosenEntryConditions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridChosenEntryConditions.Size = new System.Drawing.Size(713, 138);
            this.dataGridChosenEntryConditions.TabIndex = 1;
            this.dataGridChosenEntryConditions.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridChosenEntryConditions_RowEnter);
            // 
            // DataGridPossibleEntryConditions
            // 
            this.DataGridPossibleEntryConditions.AllowUserToAddRows = false;
            this.DataGridPossibleEntryConditions.AllowUserToDeleteRows = false;
            this.DataGridPossibleEntryConditions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridPossibleEntryConditions.Location = new System.Drawing.Point(16, 6);
            this.DataGridPossibleEntryConditions.Name = "DataGridPossibleEntryConditions";
            this.DataGridPossibleEntryConditions.ReadOnly = true;
            this.DataGridPossibleEntryConditions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DataGridPossibleEntryConditions.Size = new System.Drawing.Size(713, 138);
            this.DataGridPossibleEntryConditions.TabIndex = 0;
            this.DataGridPossibleEntryConditions.Sorted += new System.EventHandler(this.DataGridPossibleEntryConditions_Sorted);
            this.DataGridPossibleEntryConditions.Click += new System.EventHandler(this.DataGridPossibleEntryConditions_Click);
            // 
            // tabPageSteps
            // 
            this.tabPageSteps.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageSteps.Controls.Add(this.btnViewSMS);
            this.tabPageSteps.Controls.Add(this.label6);
            this.tabPageSteps.Controls.Add(this.previousStrategies);
            this.tabPageSteps.Controls.Add(this.btnDown);
            this.tabPageSteps.Controls.Add(this.btnUp);
            this.tabPageSteps.Controls.Add(this.RemoveSteps);
            this.tabPageSteps.Controls.Add(this.AddSteps);
            this.tabPageSteps.Controls.Add(this.StepsConditionsActions);
            this.tabPageSteps.Controls.Add(this.label10);
            this.tabPageSteps.Controls.Add(this.label9);
            this.tabPageSteps.Controls.Add(this.dataGridChosenStepConditions);
            this.tabPageSteps.Controls.Add(this.label4);
            this.tabPageSteps.Controls.Add(this.Letters);
            this.tabPageSteps.Controls.Add(this.V2onSteps);
            this.tabPageSteps.Controls.Add(this.V1onSteps);
            this.tabPageSteps.Controls.Add(this.OperatorsOnSteps);
            this.tabPageSteps.Controls.Add(this.label2);
            this.tabPageSteps.Location = new System.Drawing.Point(0, 25);
            this.tabPageSteps.Name = "tabPageSteps";
            this.tabPageSteps.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSteps.Selected = false;
            this.tabPageSteps.Size = new System.Drawing.Size(739, 331);
            this.tabPageSteps.TabIndex = 1;
            this.tabPageSteps.Title = "Steps";
            // 
            // btnViewSMS
            // 
            this.btnViewSMS.Location = new System.Drawing.Point(557, 161);
            this.btnViewSMS.Name = "btnViewSMS";
            this.btnViewSMS.Size = new System.Drawing.Size(80, 23);
            this.btnViewSMS.TabIndex = 68;
            this.btnViewSMS.Text = "View SMS";
            this.btnViewSMS.Visible = false;
            this.btnViewSMS.Click += new System.EventHandler(this.btnViewSMS_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(451, 138);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 15);
            this.label6.TabIndex = 67;
            this.label6.Text = "Previous Strategies";
            // 
            // previousStrategies
            // 
            this.previousStrategies.Enabled = false;
            this.previousStrategies.FormattingEnabled = true;
            this.previousStrategies.Location = new System.Drawing.Point(557, 135);
            this.previousStrategies.Name = "previousStrategies";
            this.previousStrategies.Size = new System.Drawing.Size(162, 23);
            this.previousStrategies.TabIndex = 66;
            this.previousStrategies.SelectedIndexChanged += new System.EventHandler(this.previousStrategies_SelectedIndexChanged);
            // 
            // btnDown
            // 
            this.btnDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnDown.Image = global::STL.PL.Properties.Resources.small_down5;
            this.btnDown.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnDown.Location = new System.Drawing.Point(3, 252);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(20, 20);
            this.btnDown.TabIndex = 65;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnUp.Image = global::STL.PL.Properties.Resources.small_up5;
            this.btnUp.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnUp.Location = new System.Drawing.Point(3, 226);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(20, 20);
            this.btnUp.TabIndex = 64;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // RemoveSteps
            // 
            this.RemoveSteps.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.RemoveSteps.Image = ((System.Drawing.Image)(resources.GetObject("RemoveSteps.Image")));
            this.RemoveSteps.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.RemoveSteps.Location = new System.Drawing.Point(72, 153);
            this.RemoveSteps.Name = "RemoveSteps";
            this.RemoveSteps.Size = new System.Drawing.Size(40, 24);
            this.RemoveSteps.TabIndex = 63;
            this.RemoveSteps.Click += new System.EventHandler(this.RemoveSteps_Click);
            // 
            // AddSteps
            // 
            this.AddSteps.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.AddSteps.Image = ((System.Drawing.Image)(resources.GetObject("AddSteps.Image")));
            this.AddSteps.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.AddSteps.Location = new System.Drawing.Point(26, 153);
            this.AddSteps.Name = "AddSteps";
            this.AddSteps.Size = new System.Drawing.Size(40, 24);
            this.AddSteps.TabIndex = 62;
            this.AddSteps.Click += new System.EventHandler(this.AddSteps_Click);
            // 
            // StepsConditionsActions
            // 
            this.StepsConditionsActions.FormattingEnabled = true;
            this.StepsConditionsActions.ItemHeight = 15;
            this.StepsConditionsActions.Location = new System.Drawing.Point(26, 21);
            this.StepsConditionsActions.Name = "StepsConditionsActions";
            this.StepsConditionsActions.Size = new System.Drawing.Size(693, 79);
            this.StepsConditionsActions.TabIndex = 61;
            this.StepsConditionsActions.SelectedValueChanged += new System.EventHandler(this.StepsConditionsActions_SelectedValueChanged);
            this.StepsConditionsActions.Enter += new System.EventHandler(this.StepsConditionsActions_Enter);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(174, 138);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 15);
            this.label10.TabIndex = 57;
            this.label10.Text = "Operators";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(655, 169);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(39, 15);
            this.label9.TabIndex = 55;
            this.label9.Text = "Go To";
            // 
            // dataGridChosenStepConditions
            // 
            this.dataGridChosenStepConditions.AllowUserToAddRows = false;
            this.dataGridChosenStepConditions.AllowUserToDeleteRows = false;
            this.dataGridChosenStepConditions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridChosenStepConditions.Location = new System.Drawing.Point(26, 196);
            this.dataGridChosenStepConditions.Name = "dataGridChosenStepConditions";
            this.dataGridChosenStepConditions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridChosenStepConditions.Size = new System.Drawing.Size(693, 110);
            this.dataGridChosenStepConditions.TabIndex = 50;
            this.dataGridChosenStepConditions.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridChosenStepConditions_RowEnter);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(145, 164);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Letter, SMS, etc";
            // 
            // Letters
            // 
            this.Letters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Letters.Enabled = false;
            this.Letters.FormattingEnabled = true;
            this.Letters.Location = new System.Drawing.Point(236, 161);
            this.Letters.Name = "Letters";
            this.Letters.Size = new System.Drawing.Size(302, 23);
            this.Letters.TabIndex = 5;
            // 
            // V2onSteps
            // 
            this.V2onSteps.Location = new System.Drawing.Point(419, 135);
            this.V2onSteps.Name = "V2onSteps";
            this.V2onSteps.ReadOnly = true;
            this.V2onSteps.Size = new System.Drawing.Size(26, 23);
            this.V2onSteps.TabIndex = 4;
            this.V2onSteps.TextChanged += new System.EventHandler(this.V2onSteps_TextChanged);
            // 
            // V1onSteps
            // 
            this.V1onSteps.Location = new System.Drawing.Point(387, 135);
            this.V1onSteps.Name = "V1onSteps";
            this.V1onSteps.ReadOnly = true;
            this.V1onSteps.Size = new System.Drawing.Size(26, 23);
            this.V1onSteps.TabIndex = 3;
            this.V1onSteps.TextChanged += new System.EventHandler(this.V1onSteps_TextChanged);
            // 
            // OperatorsOnSteps
            // 
            this.OperatorsOnSteps.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OperatorsOnSteps.Enabled = false;
            this.OperatorsOnSteps.FormattingEnabled = true;
            this.OperatorsOnSteps.Items.AddRange(new object[] {
            "Select an operator",
            "Greater than >",
            "Less than <",
            "Greater than or equal to >=",
            "Less than or equal to <=",
            "Equal to =",
            "Not equal to <>",
            "Between"});
            this.OperatorsOnSteps.Location = new System.Drawing.Point(236, 135);
            this.OperatorsOnSteps.Name = "OperatorsOnSteps";
            this.OperatorsOnSteps.Size = new System.Drawing.Size(145, 23);
            this.OperatorsOnSteps.TabIndex = 2;
            this.OperatorsOnSteps.SelectedIndexChanged += new System.EventHandler(this.OperatorsOnSteps_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(167, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Available Conditions / Actions";
            // 
            // tabPageExitConditions
            // 
            this.tabPageExitConditions.Controls.Add(this.lblYValueExit);
            this.tabPageExitConditions.Controls.Add(this.lblXValueExit);
            this.tabPageExitConditions.Controls.Add(this.exitStrategy);
            this.tabPageExitConditions.Controls.Add(this.RemoveExitConditions);
            this.tabPageExitConditions.Controls.Add(this.AddExitConditions);
            this.tabPageExitConditions.Controls.Add(this.ExitConditionsV2);
            this.tabPageExitConditions.Controls.Add(this.ExitConditionsV1);
            this.tabPageExitConditions.Controls.Add(this.ExitConditionsOperators);
            this.tabPageExitConditions.Controls.Add(this.dataGridChosenExitConditions);
            this.tabPageExitConditions.Controls.Add(this.dataGridPossibleExitConditions);
            this.tabPageExitConditions.Location = new System.Drawing.Point(0, 25);
            this.tabPageExitConditions.Name = "tabPageExitConditions";
            this.tabPageExitConditions.Selected = false;
            this.tabPageExitConditions.Size = new System.Drawing.Size(739, 331);
            this.tabPageExitConditions.TabIndex = 2;
            this.tabPageExitConditions.Title = "Exit Conditions";
            // 
            // lblYValueExit
            // 
            this.lblYValueExit.AutoSize = true;
            this.lblYValueExit.Location = new System.Drawing.Point(482, 164);
            this.lblYValueExit.Name = "lblYValueExit";
            this.lblYValueExit.Size = new System.Drawing.Size(46, 15);
            this.lblYValueExit.TabIndex = 58;
            this.lblYValueExit.Text = "Y Value";
            // 
            // lblXValueExit
            // 
            this.lblXValueExit.AutoSize = true;
            this.lblXValueExit.Location = new System.Drawing.Point(403, 163);
            this.lblXValueExit.Name = "lblXValueExit";
            this.lblXValueExit.Size = new System.Drawing.Size(46, 15);
            this.lblXValueExit.TabIndex = 57;
            this.lblXValueExit.Text = "X Value";
            // 
            // exitStrategy
            // 
            this.exitStrategy.FormattingEnabled = true;
            this.exitStrategy.Location = new System.Drawing.Point(583, 156);
            this.exitStrategy.Name = "exitStrategy";
            this.exitStrategy.Size = new System.Drawing.Size(136, 23);
            this.exitStrategy.TabIndex = 56;
            this.exitStrategy.SelectedValueChanged += new System.EventHandler(this.exitStrategy_SelectedValueChanged);
            // 
            // RemoveExitConditions
            // 
            this.RemoveExitConditions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.RemoveExitConditions.Image = ((System.Drawing.Image)(resources.GetObject("RemoveExitConditions.Image")));
            this.RemoveExitConditions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.RemoveExitConditions.Location = new System.Drawing.Point(72, 153);
            this.RemoveExitConditions.Name = "RemoveExitConditions";
            this.RemoveExitConditions.Size = new System.Drawing.Size(40, 24);
            this.RemoveExitConditions.TabIndex = 55;
            this.RemoveExitConditions.Click += new System.EventHandler(this.RemoveExitConditions_Click);
            // 
            // AddExitConditions
            // 
            this.AddExitConditions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.AddExitConditions.Image = ((System.Drawing.Image)(resources.GetObject("AddExitConditions.Image")));
            this.AddExitConditions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.AddExitConditions.Location = new System.Drawing.Point(26, 153);
            this.AddExitConditions.Name = "AddExitConditions";
            this.AddExitConditions.Size = new System.Drawing.Size(40, 24);
            this.AddExitConditions.TabIndex = 54;
            this.AddExitConditions.Click += new System.EventHandler(this.AddExitConditions_Click);
            // 
            // ExitConditionsV2
            // 
            this.ExitConditionsV2.Location = new System.Drawing.Point(526, 156);
            this.ExitConditionsV2.MaxLength = 12;
            this.ExitConditionsV2.Name = "ExitConditionsV2";
            this.ExitConditionsV2.ReadOnly = true;
            this.ExitConditionsV2.Size = new System.Drawing.Size(26, 23);
            this.ExitConditionsV2.TabIndex = 51;
            this.ExitConditionsV2.TextChanged += new System.EventHandler(this.ExitConditionsV2_TextChanged);
            // 
            // ExitConditionsV1
            // 
            this.ExitConditionsV1.Location = new System.Drawing.Point(452, 156);
            this.ExitConditionsV1.MaxLength = 12;
            this.ExitConditionsV1.Name = "ExitConditionsV1";
            this.ExitConditionsV1.ReadOnly = true;
            this.ExitConditionsV1.Size = new System.Drawing.Size(26, 23);
            this.ExitConditionsV1.TabIndex = 50;
            this.ExitConditionsV1.TextChanged += new System.EventHandler(this.ExitConditionsV1_TextChanged);
            // 
            // ExitConditionsOperators
            // 
            this.ExitConditionsOperators.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ExitConditionsOperators.DropDownWidth = 121;
            this.ExitConditionsOperators.Enabled = false;
            this.ExitConditionsOperators.ItemHeight = 15;
            this.ExitConditionsOperators.Items.AddRange(new object[] {
            "Select an operator",
            "Greater than >",
            "Less than <",
            "Greater than or equal to >=",
            "Less than or equal to <=",
            "Equal to =",
            "Not equal to <>",
            "Between"});
            this.ExitConditionsOperators.Location = new System.Drawing.Point(244, 156);
            this.ExitConditionsOperators.Name = "ExitConditionsOperators";
            this.ExitConditionsOperators.Size = new System.Drawing.Size(145, 23);
            this.ExitConditionsOperators.TabIndex = 49;
            this.ExitConditionsOperators.SelectedIndexChanged += new System.EventHandler(this.ExitConditionsOperators_SelectedIndexChanged);
            // 
            // dataGridChosenExitConditions
            // 
            this.dataGridChosenExitConditions.AllowUserToAddRows = false;
            this.dataGridChosenExitConditions.AllowUserToDeleteRows = false;
            this.dataGridChosenExitConditions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridChosenExitConditions.Location = new System.Drawing.Point(26, 190);
            this.dataGridChosenExitConditions.Name = "dataGridChosenExitConditions";
            this.dataGridChosenExitConditions.ReadOnly = true;
            this.dataGridChosenExitConditions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridChosenExitConditions.Size = new System.Drawing.Size(693, 138);
            this.dataGridChosenExitConditions.TabIndex = 46;
            this.dataGridChosenExitConditions.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridChosenExitConditions_RowEnter);
            // 
            // dataGridPossibleExitConditions
            // 
            this.dataGridPossibleExitConditions.AllowUserToAddRows = false;
            this.dataGridPossibleExitConditions.AllowUserToDeleteRows = false;
            this.dataGridPossibleExitConditions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridPossibleExitConditions.Location = new System.Drawing.Point(26, 9);
            this.dataGridPossibleExitConditions.Name = "dataGridPossibleExitConditions";
            this.dataGridPossibleExitConditions.ReadOnly = true;
            this.dataGridPossibleExitConditions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridPossibleExitConditions.Size = new System.Drawing.Size(693, 138);
            this.dataGridPossibleExitConditions.TabIndex = 45;
            this.dataGridPossibleExitConditions.Sorted += new System.EventHandler(this.dataGridPossibleExitConditions_Sorted);
            this.dataGridPossibleExitConditions.Click += new System.EventHandler(this.dataGridPossibleExitConditions_Click);
            // 
            // tabPageActions
            // 
            this.tabPageActions.Controls.Add(this.removeActions);
            this.tabPageActions.Controls.Add(this.addActions);
            this.tabPageActions.Controls.Add(this.dgChosenActions);
            this.tabPageActions.Controls.Add(this.dgPossibleActions);
            this.tabPageActions.Location = new System.Drawing.Point(0, 25);
            this.tabPageActions.Name = "tabPageActions";
            this.tabPageActions.Selected = false;
            this.tabPageActions.Size = new System.Drawing.Size(739, 331);
            this.tabPageActions.TabIndex = 3;
            this.tabPageActions.Title = "Actions";
            // 
            // removeActions
            // 
            this.removeActions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.removeActions.Image = ((System.Drawing.Image)(resources.GetObject("removeActions.Image")));
            this.removeActions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.removeActions.Location = new System.Drawing.Point(72, 154);
            this.removeActions.Name = "removeActions";
            this.removeActions.Size = new System.Drawing.Size(40, 24);
            this.removeActions.TabIndex = 53;
            this.removeActions.Click += new System.EventHandler(this.removeActions_Click);
            // 
            // addActions
            // 
            this.addActions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.addActions.Image = ((System.Drawing.Image)(resources.GetObject("addActions.Image")));
            this.addActions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.addActions.Location = new System.Drawing.Point(26, 154);
            this.addActions.Name = "addActions";
            this.addActions.Size = new System.Drawing.Size(40, 24);
            this.addActions.TabIndex = 52;
            this.addActions.Click += new System.EventHandler(this.addActions_Click);
            // 
            // dgChosenActions
            // 
            this.dgChosenActions.AllowUserToAddRows = false;
            this.dgChosenActions.AllowUserToDeleteRows = false;
            this.dgChosenActions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgChosenActions.Location = new System.Drawing.Point(26, 190);
            this.dgChosenActions.Name = "dgChosenActions";
            this.dgChosenActions.ReadOnly = true;
            this.dgChosenActions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgChosenActions.Size = new System.Drawing.Size(693, 138);
            this.dgChosenActions.TabIndex = 2;
            // 
            // dgPossibleActions
            // 
            this.dgPossibleActions.AllowUserToAddRows = false;
            this.dgPossibleActions.AllowUserToDeleteRows = false;
            this.dgPossibleActions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgPossibleActions.Location = new System.Drawing.Point(26, 6);
            this.dgPossibleActions.Name = "dgPossibleActions";
            this.dgPossibleActions.ReadOnly = true;
            this.dgPossibleActions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgPossibleActions.Size = new System.Drawing.Size(693, 138);
            this.dgPossibleActions.TabIndex = 1;
            this.dgPossibleActions.Sorted += new System.EventHandler(this.dgPossibleActions_Sorted);
            // 
            // tabWorkList
            // 
            this.tabWorkList.Controls.Add(this.pnlWorkListDGV);
            this.tabWorkList.Controls.Add(this.btnSaveWorkList);
            this.tabWorkList.Controls.Add(this.btnAddWorkList);
            this.tabWorkList.Controls.Add(this.txtWorkListDesc);
            this.tabWorkList.Controls.Add(this.label8);
            this.tabWorkList.Controls.Add(this.txtWorkListCode);
            this.tabWorkList.Controls.Add(this.label7);
            this.tabWorkList.Location = new System.Drawing.Point(0, 25);
            this.tabWorkList.Name = "tabWorkList";
            this.tabWorkList.Selected = false;
            this.tabWorkList.Size = new System.Drawing.Size(739, 331);
            this.tabWorkList.TabIndex = 4;
            this.tabWorkList.Title = "Work List";
            // 
            // pnlWorkListDGV
            // 
            this.pnlWorkListDGV.BackColor = System.Drawing.Color.Silver;
            this.pnlWorkListDGV.Controls.Add(this.dgvWorkList);
            this.pnlWorkListDGV.Location = new System.Drawing.Point(1, 65);
            this.pnlWorkListDGV.Name = "pnlWorkListDGV";
            this.pnlWorkListDGV.Size = new System.Drawing.Size(737, 263);
            this.pnlWorkListDGV.TabIndex = 66;
            // 
            // dgvWorkList
            // 
            this.dgvWorkList.AllowUserToAddRows = false;
            this.dgvWorkList.AllowUserToDeleteRows = false;
            this.dgvWorkList.AllowUserToResizeRows = false;
            this.dgvWorkList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWorkList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.txtColumn_WorkList,
            this.txtColumn_Description,
            this.txtColumn_WorkListDesc,
            this.chkColumn_Status,
            this.txtColumn_Strategy});
            this.dgvWorkList.Location = new System.Drawing.Point(5, 27);
            this.dgvWorkList.MultiSelect = false;
            this.dgvWorkList.Name = "dgvWorkList";
            this.dgvWorkList.ReadOnly = true;
            this.dgvWorkList.RowHeadersWidth = 24;
            this.dgvWorkList.Size = new System.Drawing.Size(727, 231);
            this.dgvWorkList.TabIndex = 61;
            this.dgvWorkList.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvWorkList_RowHeaderMouseDoubleClick);
            this.dgvWorkList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvWorkList_MouseUp);
            // 
            // txtColumn_WorkList
            // 
            this.txtColumn_WorkList.DataPropertyName = "WorkListCode";
            this.txtColumn_WorkList.HeaderText = "Work List Code";
            this.txtColumn_WorkList.Name = "txtColumn_WorkList";
            this.txtColumn_WorkList.ReadOnly = true;
            this.txtColumn_WorkList.Width = 160;
            // 
            // txtColumn_Description
            // 
            this.txtColumn_Description.DataPropertyName = "description";
            this.txtColumn_Description.HeaderText = "Description";
            this.txtColumn_Description.Name = "txtColumn_Description";
            this.txtColumn_Description.ReadOnly = true;
            this.txtColumn_Description.Width = 350;
            // 
            // txtColumn_WorkListDesc
            // 
            this.txtColumn_WorkListDesc.DataPropertyName = "WorkList";
            this.txtColumn_WorkListDesc.HeaderText = "Work List Desc";
            this.txtColumn_WorkListDesc.Name = "txtColumn_WorkListDesc";
            this.txtColumn_WorkListDesc.ReadOnly = true;
            this.txtColumn_WorkListDesc.Visible = false;
            this.txtColumn_WorkListDesc.Width = 5;
            // 
            // chkColumn_Status
            // 
            this.chkColumn_Status.DataPropertyName = "Status";
            this.chkColumn_Status.HeaderText = "Status";
            this.chkColumn_Status.Name = "chkColumn_Status";
            this.chkColumn_Status.ReadOnly = true;
            this.chkColumn_Status.Visible = false;
            // 
            // txtColumn_Strategy
            // 
            this.txtColumn_Strategy.DataPropertyName = "Strategy";
            this.txtColumn_Strategy.HeaderText = "Strategy";
            this.txtColumn_Strategy.Name = "txtColumn_Strategy";
            this.txtColumn_Strategy.ReadOnly = true;
            this.txtColumn_Strategy.Visible = false;
            // 
            // btnSaveWorkList
            // 
            this.btnSaveWorkList.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSaveWorkList.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveWorkList.Image")));
            this.btnSaveWorkList.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSaveWorkList.Location = new System.Drawing.Point(712, 6);
            this.btnSaveWorkList.Name = "btnSaveWorkList";
            this.btnSaveWorkList.Size = new System.Drawing.Size(21, 24);
            this.btnSaveWorkList.TabIndex = 65;
            this.btnSaveWorkList.Click += new System.EventHandler(this.btnSaveWorkList_Click);
            // 
            // btnAddWorkList
            // 
            this.btnAddWorkList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnAddWorkList.Image = ((System.Drawing.Image)(resources.GetObject("btnAddWorkList.Image")));
            this.btnAddWorkList.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAddWorkList.Location = new System.Drawing.Point(325, 33);
            this.btnAddWorkList.Name = "btnAddWorkList";
            this.btnAddWorkList.Size = new System.Drawing.Size(40, 24);
            this.btnAddWorkList.TabIndex = 63;
            this.btnAddWorkList.Click += new System.EventHandler(this.btnAddWorkList_Click);
            // 
            // txtWorkListDesc
            // 
            this.txtWorkListDesc.Location = new System.Drawing.Point(100, 35);
            this.txtWorkListDesc.MaxLength = 30;
            this.txtWorkListDesc.Name = "txtWorkListDesc";
            this.txtWorkListDesc.Size = new System.Drawing.Size(201, 23);
            this.txtWorkListDesc.TabIndex = 22;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(9, 35);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(87, 20);
            this.label8.TabIndex = 21;
            this.label8.Text = "Description";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtWorkListCode
            // 
            this.txtWorkListCode.Location = new System.Drawing.Point(100, 11);
            this.txtWorkListCode.MaxLength = 10;
            this.txtWorkListCode.Name = "txtWorkListCode";
            this.txtWorkListCode.Size = new System.Drawing.Size(74, 23);
            this.txtWorkListCode.TabIndex = 20;
            this.txtWorkListCode.TextChanged += new System.EventHandler(this.txtWorkListCode_TextChanged);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(9, 11);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 20);
            this.label7.TabIndex = 19;
            this.label7.Text = "Work List Code";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // menuDeleteSort
            // 
            this.menuDeleteSort.Description = "MenuItem";
            this.menuDeleteSort.Enabled = false;
            this.menuDeleteSort.Text = "Delete";
            this.menuDeleteSort.Visible = false;
            this.menuDeleteSort.Click += new System.EventHandler(this.menuDeleteSort_Click);
            // 
            // menuDeleteWorklist
            // 
            this.menuDeleteWorklist.Description = "MenuItem";
            this.menuDeleteWorklist.Enabled = false;
            this.menuDeleteWorklist.Text = "Delete";
            this.menuDeleteWorklist.Visible = false;
            this.menuDeleteWorklist.Click += new System.EventHandler(this.menuDeleteWorklist_Click);
            // 
            // StrategyConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 476);
            this.Controls.Add(this.tabControlStrategies);
            this.Controls.Add(this.groupBox1);
            this.Name = "StrategyConfiguration";
            this.Text = "Strategy Configuration";
            this.Load += new System.EventHandler(this.StrategyConfiguration_Load);
            this.Shown += new System.EventHandler(this.StrategyConfiguration_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControlStrategies.ResumeLayout(false);
            this.tabSortOrder.ResumeLayout(false);
            this.pnlSortOrderDGV.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSortOrder)).EndInit();
            this.panel1.ResumeLayout(false);
            this.tabPageEntryConditions.ResumeLayout(false);
            this.tabPageEntryConditions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridChosenEntryConditions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridPossibleEntryConditions)).EndInit();
            this.tabPageSteps.ResumeLayout(false);
            this.tabPageSteps.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridChosenStepConditions)).EndInit();
            this.tabPageExitConditions.ResumeLayout(false);
            this.tabPageExitConditions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridChosenExitConditions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPossibleExitConditions)).EndInit();
            this.tabPageActions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgChosenActions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgPossibleActions)).EndInit();
            this.tabWorkList.ResumeLayout(false);
            this.tabWorkList.PerformLayout();
            this.pnlWorkListDGV.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvWorkList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TextBox newStrategy;
        private System.Windows.Forms.ComboBox Strategies;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        //private System.Windows.Forms.TabControl tabControlStrategies;
       private TabControl tabControlStrategies;
        //private System.Windows.Forms.TabPage tabPageEntryConditions;
       private TabPage tabPageEntryConditions;
        //private System.Windows.Forms.TabPage tabPageSteps;
       private TabPage tabPageSteps;
        private System.Windows.Forms.DataGridView dataGridChosenEntryConditions;
       private System.Windows.Forms.DataGridView DataGridPossibleEntryConditions;
        private System.Windows.Forms.Button EntryConditionsOr;
        public System.Windows.Forms.TextBox EntryConditionsValue2;
        public System.Windows.Forms.TextBox EntryConditionsValue1;
        private System.Windows.Forms.ComboBox EntryConditionsOperators;
       //private System.Windows.Forms.TabPage tabPageExitConditions;
       private TabPage tabPageExitConditions;
       public System.Windows.Forms.TextBox ExitConditionsV2;
       public System.Windows.Forms.TextBox ExitConditionsV1;
       private System.Windows.Forms.ComboBox ExitConditionsOperators;
       private System.Windows.Forms.DataGridView dataGridChosenExitConditions;
       private System.Windows.Forms.DataGridView dataGridPossibleExitConditions;
       private System.Windows.Forms.ComboBox Letters;
       private System.Windows.Forms.TextBox V2onSteps;
       private System.Windows.Forms.TextBox V1onSteps;
       private System.Windows.Forms.ComboBox OperatorsOnSteps;
       private System.Windows.Forms.Label label2;
       private System.Windows.Forms.Label label4;
       private System.Windows.Forms.DataGridView dataGridChosenStepConditions;
       private System.Windows.Forms.Label label9;
       private System.Windows.Forms.Label label10;
       private System.Windows.Forms.ListBox StepsConditionsActions;
       private System.Windows.Forms.Button CreateNew;
       private System.Windows.Forms.Button RemoveEntryConditions;
       private System.Windows.Forms.Button AddEntryConditions;
       private System.Windows.Forms.Button RemoveExitConditions;
       private System.Windows.Forms.Button AddExitConditions;
       private System.Windows.Forms.Button RemoveSteps;
       private System.Windows.Forms.Button AddSteps;
       private System.Windows.Forms.Button btnSave;
       private System.Windows.Forms.ErrorProvider errorProvider1;
       private System.Windows.Forms.Button saveAs;
       private System.Windows.Forms.Label label5;
       private System.Windows.Forms.TextBox strategyCode;
       private TabPage tabPageActions;
       private System.Windows.Forms.DataGridView dgPossibleActions;
       private System.Windows.Forms.DataGridView dgChosenActions;
       private System.Windows.Forms.Button removeActions;
       private System.Windows.Forms.Button addActions;
       private System.Windows.Forms.Button activate;
       private System.Windows.Forms.ComboBox exitStrategy;
       private System.Windows.Forms.Button btnUp;
       private System.Windows.Forms.Button btnDown;
       private System.Windows.Forms.Label label6;
       private System.Windows.Forms.ComboBox previousStrategies;
        private System.Windows.Forms.Button btnDelete;
        private TabPage tabWorkList;
        private TabPage tabSortOrder;
        private System.Windows.Forms.TextBox txtWorkListDesc;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtWorkListCode;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnAddWorkList;
        private System.Windows.Forms.Button btnSaveWorkList;
        private System.Windows.Forms.Panel pnlSortOrderDGV;
        private System.Windows.Forms.ComboBox cmbEmpType;
        private System.Windows.Forms.DataGridView dgvSortOrder;
        private System.Windows.Forms.Button btnAddSortOrder;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cmbSortAscDesc;
        private System.Windows.Forms.ComboBox cmbSortColumnName;
        private System.Windows.Forms.Panel pnlWorkListDGV;
        private System.Windows.Forms.DataGridView dgvWorkList;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_WorkList;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_WorkListDesc;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chkColumn_Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_Strategy;
        private System.Windows.Forms.Button btnSaveSortOrder;
        private System.Windows.Forms.Label lblYValue;
        private System.Windows.Forms.Label lblXValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_codedescript;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_SortOrder;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_SortColumnName;
        private System.Windows.Forms.DataGridViewComboBoxColumn cmbColumn_AscDesc;
        private Crownwood.Magic.Menus.MenuCommand menuDeleteSort;
        private Crownwood.Magic.Menus.MenuCommand menuDeleteWorklist;
        private System.Windows.Forms.CheckBox chkAllocations;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblYValueExit;
        private System.Windows.Forms.Label lblXValueExit;
        private System.Windows.Forms.Label lbStrategy;
        private System.Windows.Forms.ComboBox drpStrategy;
        private System.Windows.Forms.CheckBox cbManual;
        private System.Windows.Forms.Button btnViewSMS;
        private System.Windows.Forms.Panel panel1;
    }
}