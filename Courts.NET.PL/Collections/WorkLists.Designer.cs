namespace STL.PL.Collections
{
    partial class WorkLists
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkLists));
            this.imageList_TreeView = new System.Windows.Forms.ImageList(this.components);
            this.imageListDrag = new System.Windows.Forms.ImageList(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuDeleteNode = new Crownwood.Magic.Menus.MenuCommand();
            this.gbMain = new System.Windows.Forms.GroupBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.treeViewEmployees = new System.Windows.Forms.TreeView();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tbWorkListTree = new System.Windows.Forms.TabPage();
            this.treeViewWorkList = new System.Windows.Forms.TreeView();
            this.tbActionTree = new System.Windows.Forms.TabPage();
            this.treeViewAction = new System.Windows.Forms.TreeView();
            this.tbActionView = new System.Windows.Forms.TabPage();
            this.btnExportActions = new System.Windows.Forms.Button();
            this.dgvActionView = new System.Windows.Forms.DataGridView();
            this.txtColumn_EmpOrType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtBranch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_Action = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_ActionDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_Strategy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_EmpeeNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tbWorkListView = new System.Windows.Forms.TabPage();
            this.btnExportWorklist = new System.Windows.Forms.Button();
            this.dgvWorkListView = new System.Windows.Forms.DataGridView();
            this.txtColumn_EmpOrType1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_Name1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_WorkList = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_WorkListDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtColumn_EmpeeNo1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.gbMain.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tbWorkListTree.SuspendLayout();
            this.tbActionTree.SuspendLayout();
            this.tbActionView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvActionView)).BeginInit();
            this.tbWorkListView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWorkListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList_TreeView
            // 
            this.imageList_TreeView.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_TreeView.ImageStream")));
            this.imageList_TreeView.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_TreeView.Images.SetKeyName(0, "user-group-48x48.png");
            this.imageList_TreeView.Images.SetKeyName(1, "user-48x48.png");
            this.imageList_TreeView.Images.SetKeyName(2, "users-48x48.png");
            this.imageList_TreeView.Images.SetKeyName(3, "commercial-building-48x48.png");
            this.imageList_TreeView.Images.SetKeyName(4, "Applications-Folder-smooth-48x48.png");
            this.imageList_TreeView.Images.SetKeyName(5, "My-Documents-48x48.png");
            this.imageList_TreeView.Images.SetKeyName(6, "Utilities-Alternate-48x48.png");
            this.imageList_TreeView.Images.SetKeyName(7, "X_mark.png");
            // 
            // imageListDrag
            // 
            this.imageListDrag.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageListDrag.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListDrag.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // menuDeleteNode
            // 
            this.menuDeleteNode.Description = "MenuItem";
            this.menuDeleteNode.Enabled = false;
            this.menuDeleteNode.Text = "Delete";
            this.menuDeleteNode.Visible = false;
            this.menuDeleteNode.Click += new System.EventHandler(this.menuDeleteNode_Click);
            // 
            // gbMain
            // 
            this.gbMain.BackColor = System.Drawing.SystemColors.Control;
            this.gbMain.CausesValidation = false;
            this.gbMain.Controls.Add(this.btnRefresh);
            this.gbMain.Controls.Add(this.btnSave);
            this.gbMain.Controls.Add(this.treeViewEmployees);
            this.gbMain.Controls.Add(this.tabControl);
            this.gbMain.Location = new System.Drawing.Point(2, -4);
            this.gbMain.Name = "gbMain";
            this.gbMain.Size = new System.Drawing.Size(788, 478);
            this.gbMain.TabIndex = 47;
            this.gbMain.TabStop = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(6, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 50;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnSave
            // 
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave.Location = new System.Drawing.Point(756, 11);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(21, 24);
            this.btnSave.TabIndex = 49;
            this.toolTip1.SetToolTip(this.btnSave, "Save");
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // treeViewEmployees
            // 
            this.treeViewEmployees.AllowDrop = true;
            this.treeViewEmployees.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewEmployees.HideSelection = false;
            this.treeViewEmployees.ImageIndex = 0;
            this.treeViewEmployees.ImageList = this.imageList_TreeView;
            this.treeViewEmployees.Location = new System.Drawing.Point(6, 46);
            this.treeViewEmployees.Name = "treeViewEmployees";
            this.treeViewEmployees.SelectedImageIndex = 0;
            this.treeViewEmployees.ShowNodeToolTips = true;
            this.treeViewEmployees.Size = new System.Drawing.Size(256, 423);
            this.treeViewEmployees.TabIndex = 48;
            this.treeViewEmployees.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewEmployees_NodeMouseClick);
            this.treeViewEmployees.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeViewEmployees_DragDrop);
            this.treeViewEmployees.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeViewEmployees_DragEnter);
            this.treeViewEmployees.DragOver += new System.Windows.Forms.DragEventHandler(this.treeViewEmployees_DragOver);
            this.treeViewEmployees.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.treeViewEmployees_GiveFeedback);
            this.treeViewEmployees.KeyUp += new System.Windows.Forms.KeyEventHandler(this.treeViewEmployees_KeyUp);
            this.treeViewEmployees.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeViewEmployees_MouseUp);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tbActionTree);
            this.tabControl.Controls.Add(this.tbWorkListTree);
            this.tabControl.Controls.Add(this.tbActionView);
            this.tabControl.Controls.Add(this.tbWorkListView);
            this.tabControl.Location = new System.Drawing.Point(259, 21);
            this.tabControl.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl.Name = "tabControl";
            this.tabControl.Padding = new System.Drawing.Point(0, 0);
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(529, 452);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl.TabIndex = 48;
            // 
            // tbWorkListTree
            // 
            this.tbWorkListTree.AutoScroll = true;
            this.tbWorkListTree.BackColor = System.Drawing.Color.White;
            this.tbWorkListTree.Controls.Add(this.treeViewWorkList);
            this.tbWorkListTree.Location = new System.Drawing.Point(4, 22);
            this.tbWorkListTree.Margin = new System.Windows.Forms.Padding(0);
            this.tbWorkListTree.Name = "tbWorkListTree";
            this.tbWorkListTree.Size = new System.Drawing.Size(521, 426);
            this.tbWorkListTree.TabIndex = 1;
            this.tbWorkListTree.Text = "Worklists";
            // 
            // treeViewWorkList
            // 
            this.treeViewWorkList.AllowDrop = true;
            this.treeViewWorkList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewWorkList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewWorkList.FullRowSelect = true;
            this.treeViewWorkList.HideSelection = false;
            this.treeViewWorkList.ImageIndex = 0;
            this.treeViewWorkList.ImageList = this.imageList_TreeView;
            this.treeViewWorkList.Location = new System.Drawing.Point(0, 0);
            this.treeViewWorkList.Name = "treeViewWorkList";
            this.treeViewWorkList.RightToLeftLayout = true;
            this.treeViewWorkList.SelectedImageIndex = 0;
            this.treeViewWorkList.Size = new System.Drawing.Size(521, 426);
            this.treeViewWorkList.TabIndex = 0;
            this.treeViewWorkList.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeViewWorkList_ItemDrag);
            this.treeViewWorkList.DragOver += new System.Windows.Forms.DragEventHandler(this.treeViewWorkList_DragOver);
            this.treeViewWorkList.DragLeave += new System.EventHandler(this.treeViewWorklist_DragLeave);
            // 
            // tbActionTree
            // 
            this.tbActionTree.AutoScroll = true;
            this.tbActionTree.BackColor = System.Drawing.Color.White;
            this.tbActionTree.Controls.Add(this.treeViewAction);
            this.tbActionTree.Location = new System.Drawing.Point(4, 22);
            this.tbActionTree.Margin = new System.Windows.Forms.Padding(0);
            this.tbActionTree.Name = "tbActionTree";
            this.tbActionTree.Padding = new System.Windows.Forms.Padding(3);
            this.tbActionTree.Size = new System.Drawing.Size(521, 426);
            this.tbActionTree.TabIndex = 0;
            this.tbActionTree.Text = "Actions Allocation";
            this.tbActionTree.UseVisualStyleBackColor = true;
            // 
            // treeViewAction
            // 
            this.treeViewAction.AllowDrop = true;
            this.treeViewAction.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewAction.ImageIndex = 0;
            this.treeViewAction.ImageList = this.imageList_TreeView;
            this.treeViewAction.Location = new System.Drawing.Point(0, 3);
            this.treeViewAction.Name = "treeViewAction";
            this.treeViewAction.RightToLeftLayout = true;
            this.treeViewAction.SelectedImageIndex = 0;
            this.treeViewAction.Size = new System.Drawing.Size(518, 417);
            this.treeViewAction.TabIndex = 1;
            this.treeViewAction.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeViewAction_ItemDrag);
            this.treeViewAction.DragOver += new System.Windows.Forms.DragEventHandler(this.treeViewAction_DragOver);
            this.treeViewAction.DragLeave += new System.EventHandler(this.treeViewAction_DragLeave);
            // 
            // tbActionView
            // 
            this.tbActionView.Controls.Add(this.btnExportActions);
            this.tbActionView.Controls.Add(this.dgvActionView);
            this.tbActionView.Location = new System.Drawing.Point(4, 22);
            this.tbActionView.Margin = new System.Windows.Forms.Padding(0);
            this.tbActionView.Name = "tbActionView";
            this.tbActionView.Size = new System.Drawing.Size(521, 426);
            this.tbActionView.TabIndex = 2;
            this.tbActionView.Text = "Actions View";
            this.tbActionView.UseVisualStyleBackColor = true;
            // 
            // btnExportActions
            // 
            this.btnExportActions.Image = ((System.Drawing.Image)(resources.GetObject("btnExportActions.Image")));
            this.btnExportActions.Location = new System.Drawing.Point(400, 4);
            this.btnExportActions.Name = "btnExportActions";
            this.btnExportActions.Size = new System.Drawing.Size(24, 24);
            this.btnExportActions.TabIndex = 83;
            this.toolTip1.SetToolTip(this.btnExportActions, "Export to Excel");
            this.btnExportActions.Click += new System.EventHandler(this.btnExportActions_Click);
            // 
            // dgvActionView
            // 
            this.dgvActionView.AllowUserToAddRows = false;
            this.dgvActionView.AllowUserToDeleteRows = false;
            this.dgvActionView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvActionView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.txtColumn_EmpOrType,
            this.txtBranch,
            this.txtColumn_Name,
            this.txtColumn_Action,
            this.txtColumn_ActionDescription,
            this.txtColumn_Strategy,
            this.txtColumn_EmpeeNo});
            this.dgvActionView.Location = new System.Drawing.Point(3, 30);
            this.dgvActionView.Name = "dgvActionView";
            this.dgvActionView.ReadOnly = true;
            this.dgvActionView.RowHeadersWidth = 24;
            this.dgvActionView.Size = new System.Drawing.Size(516, 393);
            this.dgvActionView.TabIndex = 1;
            // 
            // txtColumn_EmpOrType
            // 
            this.txtColumn_EmpOrType.DataPropertyName = "Emp_or_Type";
            this.txtColumn_EmpOrType.Frozen = true;
            this.txtColumn_EmpOrType.HeaderText = "Employee /Type";
            this.txtColumn_EmpOrType.Name = "txtColumn_EmpOrType";
            this.txtColumn_EmpOrType.ReadOnly = true;
            this.txtColumn_EmpOrType.Width = 60;
            // 
            // txtBranch
            // 
            this.txtBranch.DataPropertyName = "Branch";
            this.txtBranch.HeaderText = "Branch";
            this.txtBranch.Name = "txtBranch";
            this.txtBranch.ReadOnly = true;
            this.txtBranch.Width = 50;
            // 
            // txtColumn_Name
            // 
            this.txtColumn_Name.DataPropertyName = "name";
            this.txtColumn_Name.HeaderText = "Name";
            this.txtColumn_Name.Name = "txtColumn_Name";
            this.txtColumn_Name.ReadOnly = true;
            // 
            // txtColumn_Action
            // 
            this.txtColumn_Action.DataPropertyName = "action";
            this.txtColumn_Action.HeaderText = "Action Code";
            this.txtColumn_Action.Name = "txtColumn_Action";
            this.txtColumn_Action.ReadOnly = true;
            this.txtColumn_Action.Width = 50;
            // 
            // txtColumn_ActionDescription
            // 
            this.txtColumn_ActionDescription.DataPropertyName = "description";
            this.txtColumn_ActionDescription.HeaderText = "Description";
            this.txtColumn_ActionDescription.Name = "txtColumn_ActionDescription";
            this.txtColumn_ActionDescription.ReadOnly = true;
            this.txtColumn_ActionDescription.Width = 130;
            // 
            // txtColumn_Strategy
            // 
            this.txtColumn_Strategy.DataPropertyName = "strategy";
            this.txtColumn_Strategy.HeaderText = "Strategy";
            this.txtColumn_Strategy.Name = "txtColumn_Strategy";
            this.txtColumn_Strategy.ReadOnly = true;
            this.txtColumn_Strategy.Width = 80;
            // 
            // txtColumn_EmpeeNo
            // 
            this.txtColumn_EmpeeNo.DataPropertyName = "empeeNo";
            this.txtColumn_EmpeeNo.HeaderText = "EmpeeNo";
            this.txtColumn_EmpeeNo.Name = "txtColumn_EmpeeNo";
            this.txtColumn_EmpeeNo.ReadOnly = true;
            this.txtColumn_EmpeeNo.Visible = false;
            // 
            // tbWorkListView
            // 
            this.tbWorkListView.Controls.Add(this.btnExportWorklist);
            this.tbWorkListView.Controls.Add(this.dgvWorkListView);
            this.tbWorkListView.Location = new System.Drawing.Point(4, 22);
            this.tbWorkListView.Margin = new System.Windows.Forms.Padding(0);
            this.tbWorkListView.Name = "tbWorkListView";
            this.tbWorkListView.Size = new System.Drawing.Size(521, 426);
            this.tbWorkListView.TabIndex = 3;
            this.tbWorkListView.Text = "Worklist View";
            this.tbWorkListView.UseVisualStyleBackColor = true;
            // 
            // btnExportWorklist
            // 
            this.btnExportWorklist.Image = ((System.Drawing.Image)(resources.GetObject("btnExportWorklist.Image")));
            this.btnExportWorklist.Location = new System.Drawing.Point(400, 4);
            this.btnExportWorklist.Name = "btnExportWorklist";
            this.btnExportWorklist.Size = new System.Drawing.Size(24, 24);
            this.btnExportWorklist.TabIndex = 84;
            this.toolTip1.SetToolTip(this.btnExportWorklist, "Export to Excel");
            this.btnExportWorklist.Click += new System.EventHandler(this.btnExportWorklist_Click);
            // 
            // dgvWorkListView
            // 
            this.dgvWorkListView.AllowUserToAddRows = false;
            this.dgvWorkListView.AllowUserToDeleteRows = false;
            this.dgvWorkListView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWorkListView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.txtColumn_EmpOrType1,
            this.txtColumn_Name1,
            this.txtColumn_WorkList,
            this.txtColumn_WorkListDescription,
            this.txtColumn_EmpeeNo1});
            this.dgvWorkListView.Location = new System.Drawing.Point(3, 30);
            this.dgvWorkListView.Name = "dgvWorkListView";
            this.dgvWorkListView.ReadOnly = true;
            this.dgvWorkListView.RowHeadersWidth = 24;
            this.dgvWorkListView.Size = new System.Drawing.Size(512, 393);
            this.dgvWorkListView.TabIndex = 0;
            // 
            // txtColumn_EmpOrType1
            // 
            this.txtColumn_EmpOrType1.DataPropertyName = "Emp_Or_Type";
            this.txtColumn_EmpOrType1.Frozen = true;
            this.txtColumn_EmpOrType1.HeaderText = "Employee /Type";
            this.txtColumn_EmpOrType1.Name = "txtColumn_EmpOrType1";
            this.txtColumn_EmpOrType1.ReadOnly = true;
            this.txtColumn_EmpOrType1.Width = 60;
            // 
            // txtColumn_Name1
            // 
            this.txtColumn_Name1.DataPropertyName = "name";
            this.txtColumn_Name1.HeaderText = "Name";
            this.txtColumn_Name1.Name = "txtColumn_Name1";
            this.txtColumn_Name1.ReadOnly = true;
            this.txtColumn_Name1.Width = 140;
            // 
            // txtColumn_WorkList
            // 
            this.txtColumn_WorkList.DataPropertyName = "worklist";
            this.txtColumn_WorkList.HeaderText = "Work List";
            this.txtColumn_WorkList.Name = "txtColumn_WorkList";
            this.txtColumn_WorkList.ReadOnly = true;
            this.txtColumn_WorkList.Width = 120;
            // 
            // txtColumn_WorkListDescription
            // 
            this.txtColumn_WorkListDescription.DataPropertyName = "description";
            this.txtColumn_WorkListDescription.HeaderText = "Description";
            this.txtColumn_WorkListDescription.Name = "txtColumn_WorkListDescription";
            this.txtColumn_WorkListDescription.ReadOnly = true;
            this.txtColumn_WorkListDescription.Width = 130;
            // 
            // txtColumn_EmpeeNo1
            // 
            this.txtColumn_EmpeeNo1.DataPropertyName = "EmpeeNo";
            this.txtColumn_EmpeeNo1.HeaderText = "Employee No";
            this.txtColumn_EmpeeNo1.Name = "txtColumn_EmpeeNo1";
            this.txtColumn_EmpeeNo1.ReadOnly = true;
            this.txtColumn_EmpeeNo1.Visible = false;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // WorkLists
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 476);
            this.ControlBox = false;
            this.Controls.Add(this.gbMain);
            this.MaximizeBox = false;
            this.Name = "WorkLists";
            this.Text = "WorkLists";
            this.Load += new System.EventHandler(this.WorkLists_Load);
            this.gbMain.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tbWorkListTree.ResumeLayout(false);
            this.tbActionTree.ResumeLayout(false);
            this.tbActionView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvActionView)).EndInit();
            this.tbWorkListView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvWorkListView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbMain;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.TreeView treeViewEmployees;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tbActionTree;
        private System.Windows.Forms.TabPage tbWorkListTree;
        private System.Windows.Forms.TabPage tbActionView;
        private System.Windows.Forms.TabPage tbWorkListView;
        private System.Windows.Forms.TreeView treeViewWorkList;
        private System.Windows.Forms.ImageList imageListDrag;
        private System.Windows.Forms.TreeView treeViewAction;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.DataGridView dgvWorkListView;
        private System.Windows.Forms.DataGridView dgvActionView;
        private System.Windows.Forms.ImageList imageList_TreeView;
        public System.Windows.Forms.Button btnExportActions;
        public System.Windows.Forms.Button btnExportWorklist;
        private Crownwood.Magic.Menus.MenuCommand menuDeleteNode;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_EmpOrType;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtBranch;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_Action;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_ActionDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_Strategy;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_EmpeeNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_EmpOrType1;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_Name1;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_WorkList;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_WorkListDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtColumn_EmpeeNo1;
    }
}